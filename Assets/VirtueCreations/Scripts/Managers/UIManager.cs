using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] Transform coinUiTransform;

        Vector3 _iconScale;

        [SerializeField] Transform resourceUIParent;

        [SerializeField] Transform resourceObjectUIParent;

        [SerializeField] List<ResourceUI> resourceUIs;

        Dictionary<Resource, int> _resourceObjectCollectHistory;

        static string FileName => $"Resources{FileExtensions.JSON}";
        static bool FileExists => SaveGame.Exists(FileName);

        [Button]
        public void Init10k()
        {
            var length = resourceUIs.Count;
            for (int i = 0; i < length; i++)
            {
                resourceUIs[i].SetAmount(10000);
            }
        }

        void Awake()
        {
            _resourceObjectCollectHistory = new Dictionary<Resource, int>();
            _iconScale = coinUiTransform.localScale;
        }

        void Start()
        {
            LoadData();
        }

        public Transform hand;
        public float handSpeed;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (hand.gameObject.activeInHierarchy)
                {
                    hand.gameObject.Hide();
                }
                else
                {
                    hand.gameObject.Show();
                }
            }

            hand.transform.position = Vector3.Lerp(hand.transform.position, Input.mousePosition, handSpeed * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.M))
            {
                resourceUIs[(int)Resource.Coin].Add(10);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                MainButtons.Instance.ChangeEnergyAmount(10);
            }
        }

        void OnEnable()
        {
            PauseEvent.AddListener(SaveData);
        }

        void OnDisable()
        {
            PauseEvent.RemoveListener(SaveData);
        }

        void SaveData()
        {
            var length = resourceUIs.Count;
            var resources = new Dictionary<Resource, int>();
            for (var i = 0; i < length; i++)
            {
                resources.Add(resourceUIs[i].typeOfResource, resourceUIs[i].amount);
            }

            SaveGame.Save(FileName, resources);
        }

        void LoadData()
        {
            // var length = Manager.Instance.AssetManager.ResourceSprites.Count;
            /*var length = resourceUIs.Count;
            for (var i = 0; i < length; i++) {
                // var resourceUI = Instantiate (Manager.Instance.AssetManager.ResourceUIPrefab, resourceUIParent);
                var resourceUI = resourceUIs[i];
                // resourceUI.icon.sprite = Manager.Instance.AssetManager.ResourceSprites[i];
                // resourceUI.typeOfResource = i.ToEnum<Resource> ();
                resourceUI.SetAmount (0);
                // resourceUI.gameObject.Hide ();
                resourceUIs.Add (resourceUI);
            }*/
            if (FileExists && Helper.GetBool(PlayerPrefsKey.TUTORIALCOMPLETED, false))
            {
                var resources = SaveGame.Load<Dictionary<Resource, int>>(FileName);
                var i = 0;
                foreach (var resource in resources)
                {
                    resourceUIs[i].SetAmount(resource.Value);
                    // resourceUIs[i].gameObject.Hide ();
                    i++;
                }
            }
            else
            {
                //set default coins value to 30
                resourceUIs[0].SetAmount(25);
            }

            resourceUIs[0].Show();
        }

        public void CollectResourceObject(Resource typeOfResource, int amount)
        {
            var resourceType = typeOfResource;
            resourceUIs[resourceType.ToInt()].Add(amount);
            TryAddResourceValue(resourceType, amount);
            DOTween.Kill(resourceType);
            DOVirtual.DelayedCall(.7f, () =>
            {
                var resourceObjectUI = Instantiate(Manager.Instance.AssetManager.ResourceObjectUIPrefab, resourceObjectUIParent);
                resourceObjectUI.ShowText(resourceType, TryGetResourceValue(resourceType));
                TryRemoveResourceValue(resourceType);
            }).SetId(resourceType);
        }

        internal void ShowResourceUI(Resource typeOfResource)
        {
            resourceUIs[typeOfResource.ToInt()].Show();
        }

        int TryGetResourceValue(Resource typeofResource) => _resourceObjectCollectHistory.TryGetValue(typeofResource, out var amount) ? amount : 0;

        void TryAddResourceValue(Resource typeofResource, int amount)
        {
            if (_resourceObjectCollectHistory.ContainsKey(typeofResource))
            {
                _resourceObjectCollectHistory[typeofResource] += amount;
            }
            else
            {
                _resourceObjectCollectHistory.Add(typeofResource, amount);
            }
        }

        void TryRemoveResourceValue(Resource typeofResource)
        {
            if (_resourceObjectCollectHistory.ContainsKey(typeofResource))
            {
                _resourceObjectCollectHistory[typeofResource] = 0;
            }
        }

        public static Sprite GetSpriteByTypeOfResource(Resource typeOfResource) => Manager.Instance.AssetManager.ResourceSprites[typeOfResource.ToInt()];

        public int GetAmountByTypeOfResource(Resource typeOfResource)
        {
            ShowResourceUI(typeOfResource);
            return resourceUIs[typeOfResource.ToInt()].amount;
        }

        public void SetAmountByTypeOfResource(Resource typeOfResource, int amount)
        {
            resourceUIs[typeOfResource.ToInt()].SetAmount(amount);
        }

        public void AddAmountByTypeOfResource(Resource typeOfResource, int amount)
        {
            resourceUIs[typeOfResource.ToInt()].Add(amount);
            TweenCoinUi();
            if (TutorialScript.Instance.coinsChecked && !Helper.GetBool(PlayerPrefsKey.TROOPBUILDINGUPCOMPLETE, false))
            {
                return;
            }

            MainButtons.Instance.RefreshAllButtons();
        }

        public void AddAmountWithCounter(Resource typeOfResource, int amount)
        {
            resourceUIs[typeOfResource.ToInt()].AddWithCounter(amount, () =>
            {
                TweenCoinUi();
                if (TutorialScript.Instance.coinsChecked && !Helper.GetBool(PlayerPrefsKey.TROOPBUILDINGUPCOMPLETE, false))
                {
                    return;
                }

                MainButtons.Instance.RefreshAllButtons();
            });
        }

        public void AddAmountByTypeOfResourceSimple(Resource typeOfResource, int amount)
        {
            resourceUIs[typeOfResource.ToInt()].AddWithCounter(amount, () =>
            {
                TweenCoinUi();
                MainButtons.Instance.RefreshButtonsExceptEnergyUp();
            });
        }

        void TweenCoinUi()
        {
            coinUiTransform.DOKill();
            coinUiTransform.DOScale(_iconScale * 1.3f, .1f).SetUpdate(true).From(_iconScale).SetLoops(2, LoopType.Yoyo);
        }

        public void ShowCoinsOnly()
        {
            for (var i = 0; i < resourceUIs.Count; i++)
            {
                resourceUIs[i].Hide();
            }

            resourceUIs[0].Show();
        }
    }
}