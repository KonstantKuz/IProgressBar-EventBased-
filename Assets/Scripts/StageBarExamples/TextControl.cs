using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextControl : MonoBehaviour
{
    [SerializeField] private GameObject editorText;
    [SerializeField] private GameObject playModeText;

    // Start is called before the first frame update
    void Start()
    {
        editorText.SetActive(false);
        playModeText.SetActive(true);
    }
}
