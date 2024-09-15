using UnityEngine;

public class Flag : MonoBehaviour {
    MeshFilter _mf;
    [SerializeField]
    float repeatInterval = 0.05f;
    [SerializeField]
    int currentMeshIndex;
    [SerializeField]
    Mesh[] allMeshes;

    void Awake () {
        TryGetComponent (out _mf);
    }

    void Start () {
        InvokeRepeating (nameof (UpdateFlagMesh), repeatInterval, repeatInterval);
    }

    void UpdateFlagMesh () {
        _mf.sharedMesh = allMeshes[currentMeshIndex];
        currentMeshIndex++;
        if (currentMeshIndex >= allMeshes.Length) {
            currentMeshIndex = 0;
        }
    }
}