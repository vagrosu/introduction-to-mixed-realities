using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField] private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> _spawnedPrefabs = new();
    private ARTrackedImageManager _trackedImageManager;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        _trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        foreach (var prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            _spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    private void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
            CheckPosition(trackedImage);
        }
        foreach (var trackedImage in eventArgs.removed)
        {
            _spawnedPrefabs[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        var name = trackedImage.referenceImage.name;
        var position = trackedImage.transform.position;

        GameObject prefab = _spawnedPrefabs[name];
        prefab.transform.position = position;
        prefab.SetActive(true);
    }

    private void CheckPosition(ARTrackedImage trackedImage)
    {
        var name = trackedImage.referenceImage.name;
        var position = _spawnedPrefabs[name].transform.position;

        foreach (var prefab in _spawnedPrefabs)
        {
            if (prefab.Key != name)
            {
                if (Vector3.Distance(prefab.Value.transform.position, position) < 0.25)
                {
                    _animator.SetTrigger("TrAttack");
                }
                else
                {
                    _animator.SetTrigger("TrIdle");
                }
            }
        }
    }
}
