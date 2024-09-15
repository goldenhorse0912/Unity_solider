using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VIRTUE;

public class FollowCursor : CachedMonoBehaviour
{
    public float speed;

    private void Update()
    {
        CachedTransform.position = Vector3.Lerp(CachedTransform.position, Input.mousePosition, speed * Time.deltaTime);
    }
}