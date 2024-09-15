using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VIRTUE
{
    public class CameraController : CachedMonoBehaviour
    {
        Vector3 _camStartPos;
        Transform _cameraParent;
        public CameraClampArea_SO clampArea;
        public AudioSource bgMusic;
        public float speed, handSpeed;

        

#if UNITY_EDITOR
        /*void OnDrawGizmos () {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine (new Vector3 (clampArea.minX, 0, clampArea.minZ), new Vector3 (clampArea.maxX, 0, clampArea.minZ));
            Gizmos.DrawLine (new Vector3 (clampArea.minX, 0, clampArea.maxZ), new Vector3 (clampArea.maxX, 0, clampArea.maxZ));
            Gizmos.DrawLine (new Vector3 (clampArea.minX, 0, clampArea.minZ), new Vector3 (clampArea.minX, 0, clampArea.maxZ));
            Gizmos.DrawLine (new Vector3 (clampArea.maxX, 0, clampArea.minZ), new Vector3 (clampArea.maxX, 0, clampArea.maxZ));
        }*/
#endif

        void Start()
        {
            _cameraParent = CachedTransform.parent;
            _camStartPos = transform.position;
            DOTween.To(FadeInVolume, 0, 1, 1f);
        }

        void FadeInVolume(float value)
        {
            bgMusic.volume = value;
        }

        void Update()
        {
            HandleTouches();
        }

        public TextMeshProUGUI speedText;

        public void HandleTouches()
        {
            var h = ETCInput.GetAxis("Horizontal");
            var v = ETCInput.GetAxis("Vertical");
            var newPosition = CachedTransform.position + Vector3.zero.With(-h, 0, -v) * (speed * Time.deltaTime);
            var clampedX = Mathf.Clamp(newPosition.x, clampArea.minX, clampArea.maxX);
            var clampedZ = Mathf.Clamp(newPosition.z, clampArea.minZ, clampArea.maxZ);
            CachedTransform.position = new Vector3(clampedX, CachedTransform.position.y, clampedZ);
            speedText.text = speed.ToString();
        }

        public void SetSpeed(float amount)
        {
            speed += amount;
        }

        public void ResetCamPos()
        {
            CachedTransform.position = _camStartPos;
        }

        public void ShakeCamera()
        {
            _cameraParent.DOKill();
            _cameraParent.DOPunchPosition(Vector3.one * .2f, .3f, 20);
        }

        public void MoveCameraBase(Action onReach)
        {
            CachedTransform.DOMove(_camStartPos, 200f).SetSpeedBased().SetUpdate(true).OnComplete(() => { onReach?.Invoke(); });
        }
    }
}