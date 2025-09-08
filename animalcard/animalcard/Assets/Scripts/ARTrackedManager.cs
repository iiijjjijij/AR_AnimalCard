using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTrackedManager : MonoBehaviour
{
    [Header("AR Components")]
    public ARTrackedImageManager trackedImageManager;

    [Header("Prefabs")]
    public List<GameObject> prefabList = new List<GameObject>();

    [Header("UI")]
    public GameObject hintUI;

    private Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
    private Camera arCamera;
    private bool firstCardDetected = false;

    void Start()
    {
        arCamera = Camera.main;

        // Prefab 초기화
        foreach (var prefab in prefabList)
        {
            if (!prefabDict.ContainsKey(prefab.name))
            {
                var instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                instance.SetActive(false);

                // Collider 확인
                if (instance.GetComponent<Collider>() == null)
                    instance.AddComponent<BoxCollider>();

                prefabDict.Add(prefab.name, instance);
            }
        }

        if (hintUI != null)
            hintUI.SetActive(true);
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 새로 추가된 이미지
        foreach (var trackedImage in eventArgs.added)
        {
            HandleFirstCardUI();
            UpdateImage(trackedImage);
        }

        // 업데이트된 이미지
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        // 제거된 이미지
        foreach (var trackedImage in eventArgs.removed)
        {
            string name = trackedImage.referenceImage.name;
            if (prefabDict.TryGetValue(name, out var obj))
                obj.SetActive(false);
        }
    }

    private void HandleFirstCardUI()
    {
        if (!firstCardDetected)
        {
            firstCardDetected = true;
            if (hintUI != null)
                hintUI.SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        if (prefabDict.TryGetValue(name, out var obj))
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                obj.SetActive(true);
                obj.transform.position = trackedImage.transform.position;

                // 카메라가 보는 방향으로 회전
                Quaternion offsetRotation = Quaternion.Euler(0, 180, 0);
                obj.transform.rotation = trackedImage.transform.rotation * offsetRotation;
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }
}
