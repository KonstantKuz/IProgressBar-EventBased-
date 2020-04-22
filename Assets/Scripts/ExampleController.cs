using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleController : MonoBehaviour
{
    float progress1 = 0;
    float progress2 = 100;

    InitialData<FirstExampleProgressBar> firstInitialData;
    InitialData<SecondExampleProgressBar> secondInitialData;

    UpdateData<FirstExampleProgressBar> firstUpdateData;
    UpdateData<SecondExampleProgressBar> secondUpdateData;

    private void OnEnable()
    {
        FirstExampleProgressBar.OnProgressFinished += delegate { Finish("<color=green> Finish on green progressBar! </color>"); };
        SecondExampleProgressBar.OnProgressFinished += delegate { Finish("<color=red> Finish on red progressBar! </color>"); };
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeFirstProgressBar();
        InitializeSecondProgressBar();
    }

    void InitializeFirstProgressBar()
    {
        firstInitialData.MinValue = 0;
        firstInitialData.MaxValue = 100;
        firstInitialData.CurrentValue = progress1;

        FirstExampleProgressBar.InitializeProgress.Invoke(firstInitialData);    
    }

    void InitializeSecondProgressBar()
    {
        secondInitialData.MinValue = 0;
        secondInitialData.MaxValue = 100;
        secondInitialData.CurrentValue = progress2;

        SecondExampleProgressBar.InitializeProgress.Invoke(secondInitialData);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.D))
        {
            progress1 += Time.deltaTime * 50f;

            firstUpdateData.CurrentValue = progress1;
            FirstExampleProgressBar.UpdateProgress.Invoke(firstUpdateData);

            progress2 += Time.deltaTime * 50f;

            secondUpdateData.CurrentValue = progress2;
            SecondExampleProgressBar.UpdateProgress.Invoke(secondUpdateData);
        }
        if(Input.GetKey(KeyCode.A))
        {
            progress1 -= Time.deltaTime * 50f;

            firstUpdateData.CurrentValue = progress1;
            FirstExampleProgressBar.UpdateProgress.Invoke(firstUpdateData);

            progress2 -= Time.deltaTime * 50f;

            secondUpdateData.CurrentValue = progress2;
            SecondExampleProgressBar.UpdateProgress.Invoke(secondUpdateData);
        }

        progress1 = Mathf.Clamp(progress1, 0, 100);
        progress2 = Mathf.Clamp(progress2, 0, 100);
    }

    void Finish(string message)
    {
        Debug.Log(message);
    }
}
