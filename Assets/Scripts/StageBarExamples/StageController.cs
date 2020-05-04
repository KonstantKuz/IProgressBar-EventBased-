using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    private int currentStage = 0;
    private int stageToWin = 10;

    InitialData<ExampleStageBar> initData;
    UpdateData<ExampleStageBar> updateData;

    private void OnEnable()
    {
        ExampleStageBar.OnProgressFinished += delegate { Win(); };
    }

    private void Win()
    {
        Debug.Log("<color=green> Win! </color>");
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeStageBar();
    }

    private void InitializeStageBar()
    {
        initData.MinValue = 0;
        initData.MaxValue = stageToWin;
        initData.CurrentValue = currentStage;

        ExampleStageBar.InitializeProgress(initData);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            currentStage++;
            UpdateStageBar();
        }
    }

    private void UpdateStageBar()
    {
        updateData.CurrentValue = currentStage;
        ExampleStageBar.UpdateProgress(updateData);
    }
}
