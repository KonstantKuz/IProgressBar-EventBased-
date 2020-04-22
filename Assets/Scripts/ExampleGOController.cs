using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleGOController : MonoBehaviour
{
    [SerializeField] private ExampleGOProgressBar exampleBar;

    private float currHP;

    private void OnEnable()
    {
        exampleBar.OnProgressFinished += Death;
    }
    // Start is called before the first frame update
    void Start()
    {
        currHP = 100;
        InitializeBar();
    }

    void InitializeBar()
    {
        exampleBar.Initialize(0, currHP, currHP);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            currHP -= Time.deltaTime * 50f;
            exampleBar.UpdateCurrentProgress(currHP);
        }
        if (Input.GetKey(KeyCode.C))
        {
            currHP += Time.deltaTime * 50f;
            exampleBar.UpdateCurrentProgress(currHP);
        }

        currHP = Mathf.Clamp(currHP, 0, 100);
    }

    void Death()
    {
        Debug.Log("<color=yellow> Finish in GO progress bar! </color>");
    }
}