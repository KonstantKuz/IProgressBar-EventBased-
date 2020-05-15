using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextExample : MonoBehaviour
{
    [SerializeField] private int sceneIndex = 0;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex));
    }
}
