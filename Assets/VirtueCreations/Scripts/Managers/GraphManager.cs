using System;
using BayatGames.SaveGameFree;
using TMPro;
using UnityEngine;

namespace VIRTUE {
    public class GraphManager : MonoBehaviour {
        float _deltaTime;
        float _gameTimer;
        string FileName => $"GameTimer{FileExtensions.JSON}";
        bool FileExists => SaveGame.Exists (FileName);

        [SerializeField]
        TextMeshProUGUI uiText;

        void Reset () {
            uiText = GetComponent<TextMeshProUGUI> ();
            if (uiText != null) {
                uiText.text = $"{CountFPS ()}\n{Timer ()}";
            }
        }

        void OnEnable () {
            PauseEvent.AddListener (SaveData);
        }

        void OnDisable () {
            PauseEvent.RemoveListener (SaveData);
        }

        void Awake () {
            _deltaTime = 0f;
            _gameTimer = 0f;
        }

        void Start () {
            LoadData ();
        }

        void Update () {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            _gameTimer += Time.deltaTime;
            uiText.text = $"{CountFPS ()}\n{Timer ()}";
        }

        string Timer () {
            var seconds = (int)(_gameTimer % 60);
            var minutes = (int)(_gameTimer / 60) % 60;
            return $"{minutes:00} : {seconds:00}";
        }

        string CountFPS () {
            var msec = _deltaTime * 1000.0f;
            var fps = 1.0f / _deltaTime;
            return $"{msec:0.0} ms\n({fps:0.} fps)";
        }

        void SaveData () {
            SaveGame.Save (FileName, _gameTimer);
        }

        void LoadData () {
            _gameTimer = FileExists ? SaveGame.Load<float> (FileName) : 0f;
        }
    }
}