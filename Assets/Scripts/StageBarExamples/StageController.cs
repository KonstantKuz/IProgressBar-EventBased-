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
        ExampleStageBar.OnProgressFinished += delegate { StartCoroutine(Win()); };
    }

    private void OnDisable()
    {
        ExampleStageBar.OnProgressFinished -= delegate { StartCoroutine(Win()); };
    }

    private IEnumerator Win()
    {
        Debug.Log("<color=green> Win! </color>");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Random reinitialization stagebar");
        RandomInitialize();
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

    private void RandomInitialize()
    {
        currentStage = 0;
        stageToWin = Random.Range(2, 10);

        InitializeStageBar();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            currentStage++;
            UpdateStageBar();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            currentStage--;
            UpdateStageBar();
        }

        currentStage = Mathf.Clamp(currentStage, 0, stageToWin);
    }

    private void UpdateStageBar()
    {
        updateData.CurrentValue = currentStage;
        ExampleStageBar.UpdateProgress(updateData);
    }
}
