using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GOLineProgressBar : MonoBehaviour, GameObjectProgressBar
{
    [Tooltip("Image that will be visualize progress.")]
    [SerializeField] private Image progressBarImage;

    [Tooltip("Represents visual behaviour of progress.")]
    [SerializeField] private VisualBehaviour visualBehaviour;
    
    [Tooltip("Represents place where will be placed MinValue." +
             "Simple horizontal SceneLineProgressBar with Reverted fill direction and increasing progress" +
             "will be filled from right corner of ProgressBarImage rect to its left corner.")]
    [SerializeField] private FillDirection fillDirection;
    [Tooltip("Represents how and how long will be smoothed progress in this progress bar.")]
    [SerializeField] private SmoothType smoothType = SmoothType.None;
    [SerializeField] private float smoothDuration = 0;
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public VisualBehaviour VisualBehaviour { get { return visualBehaviour; } }
    public FillDirection FillDirection { get { return fillDirection; } }
    public SmoothType SmoothType { get { return smoothType ; } }
    public float SmoothDuration { get { return smoothDuration; } }
    public bool Finished { get; private set; }
    public bool Decrease { get; private set; }
    
    public Action OnProgressFinished { get; set; }

    private WaitForFixedUpdate waitForFixedFrame = new WaitForFixedUpdate();

    public void Initialize(float minValue, float maxValue, float currentValue)
    {
        Finished = false;
        MinValue = minValue;
        MaxValue = maxValue;
        CurrentValue = currentValue;
        if (CurrentProgress() >= 1)
        {
            Decrease = true;
        }
        
        InitializeVisual();

        Debug.Log
        ($"Initialized on GameObject {gameObject.name} with Values (click for full details)" +
         $"\n MinValue = {MinValue}, MaxValue = {MaxValue}, CurrentValue = {CurrentValue}." +
         $"\n Is this progress decreasing?={Decrease}." +
         $"\n Visual behaviour == {VisualBehaviour}" +
         $"\n Fill direction == {FillDirection}");

        UpdateVisual();
    }
    
    public void InitializeVisual()
    {
        progressBarImage.type = Image.Type.Filled;
        progressBarImage.fillMethod = Image.FillMethod.Horizontal;
        if (fillDirection == FillDirection.Original)
        {
            progressBarImage.fillOrigin = 0;
        }
        else
        {
            progressBarImage.fillOrigin = 1;
        }
    }

    public void UpdateVisual()
    {
        progressBarImage.fillAmount = CurrentVisualProgress();
    }

    public float CurrentVisualProgress()
    {
        float CurrentVisualProgress;

        if (VisualBehaviour == VisualBehaviour.InvertedRelativeToActual)
        {
            float CurrentRevertedValue = MaxValue - CurrentValue;
            CurrentVisualProgress = (CurrentRevertedValue - MinValue) / (MaxValue - MinValue);

        }
        else
        {
            CurrentVisualProgress = (CurrentValue - MinValue) / (MaxValue - MinValue);
        }

        return CurrentVisualProgress;
    }

    public void UpdateCurrentProgress(float currentValue)
    {
        switch (smoothType)
        {
            case SmoothType.None:
                {
                    CurrentValue = currentValue;
                    CheckProgress();
                    UpdateVisual();
                }
                break;
            case SmoothType.VisuallyOnly:
                {
                    CurrentValue = currentValue;
                    CheckProgress();
                    StartCoroutine(SmoothUpdateUI());
                }
                break;
            case SmoothType.ActuallyAndVisually:
                {
                    StartCoroutine(SmoothUpdateCurrentProgress(currentValue));
                }
                break;
        }
    }

    private IEnumerator SmoothUpdateCurrentProgress(float currentValue)
    {
        float timeElapsed = 0;
        float startTime = Time.time;

        while (CurrentValue != currentValue)
        {
            timeElapsed = Time.time - startTime;

            CurrentValue = Mathf.MoveTowards(CurrentValue, currentValue, timeElapsed / SmoothDuration);

            CheckProgress();

            UpdateVisual();

            yield return waitForFixedFrame;
        }
    }

    private IEnumerator SmoothUpdateUI()
    {
        float timeElapsed = 0;
        float startTime = Time.time;

        while (progressBarImage.fillAmount != CurrentVisualProgress())
        {
            timeElapsed = Time.time - startTime;

            progressBarImage.fillAmount = Mathf.MoveTowards(progressBarImage.fillAmount, CurrentVisualProgress(), timeElapsed / SmoothDuration);

            yield return waitForFixedFrame;
        }
    }

    public float CurrentProgress()
    {
        float CurrentProgress = (CurrentValue - MinValue) / (MaxValue - MinValue);
        return CurrentProgress;
    }

    public void CheckProgress()
    {
        if (!Finished)
        {
            if (MinValue > MaxValue)
            {
                Debug.LogError("MinValue needs to be greater than MaxValue");
            }
            if (Decrease)
            {
                if (CurrentProgress() <= 0)
                {
                    Debug.Log($"Finish was triggered on GameObject {gameObject.name} with CurrentProgress() == 0");
                    OnProgressFinished?.Invoke();
                    Finished = true;
                }
            }
            else
            {
                if (CurrentProgress() >= 1)
                {
                    Debug.Log($"Finish was triggered on GameObject {gameObject.name} with CurrentProgress() == 1");
                    OnProgressFinished?.Invoke();
                    Finished = true;
                }
            }
        }
    }
}