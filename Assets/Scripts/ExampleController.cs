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
        FirstExampleProgressBar.OnProgressFinished += delegate { Finish("Finish on green progressBar! "); };
        SecondExampleProgressBar.OnProgressFinished += delegate { Finish("Finish on red progressBar! "); };
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

        if (progress1 > 100)
            progress1 = 100;
        if (progress1 < 0)
            progress1 = 0;
        if (progress2 > 100)
            progress2 = 100;
        if (progress2 < 0)
            progress2 = 0;
    }

    void Finish(string message)
    {
        Debug.Log(message);
    }
}
