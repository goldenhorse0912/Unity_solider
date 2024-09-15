using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class ResourceObjectUI : CachedMonoBehaviour {
        static readonly Vector3 LocalUpPosition = new(0, 175, 0);
        [SerializeField]
        Image icon;

        [SerializeField]
        TextMeshProUGUI amountText;

        public void ShowText (Resource typeOfResource, int amount) {
            icon.sprite = UIManager.GetSpriteByTypeOfResource (typeOfResource);
            amountText.text = $"+{amount}";
            DOTween.Sequence ()
                   .Append (CachedTransform.DOLocalMove (LocalUpPosition, 0.5f).SetEase (Ease.OutBack))
                   .Join (CachedTransform.DOScale (Vector3.one, 0.5f).From (Vector3.zero).SetEase (Ease.OutBack))
                   .AppendInterval (0.75f)
                   .Append (CachedTransform.DOScale (Vector3.zero, 0.2f))
                   .AppendCallback (Destroy);
        }

        void Destroy () {
            Destroy (CachedGameObject);
        }
    }
}