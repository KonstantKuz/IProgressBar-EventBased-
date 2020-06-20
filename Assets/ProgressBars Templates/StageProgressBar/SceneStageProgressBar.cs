using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SceneStageProgressBar<T> : MonoBehaviour, SceneProgressBar<T> where T : class
{
    [SerializeField] private Sprite nextStageSprite = null;
    [SerializeField] private Sprite currentStageSprite = null;
    [SerializeField] private Sprite completeStageSprite = null;
    [SerializeField] private GameObject stagePointImagePrefab = null;
    [SerializeField] private GameObject stagePointsParentPanel = null;
    [SerializeField] private bool Animate = false;

    private Image[] stagePoints;
    private protected int AnimationHash = 0;

    [Tooltip("Represents visual behaviour of progress.")]
    [SerializeField] private VisualBehaviour visualBehaviour;
    
    [Tooltip("Represents place where will be placed MinValue." +
             "Simple horizontal SceneLineProgressBar with Reverted fill direction and increasing progress" +
             "will be filled from right corner of ProgressBarImage rect to its left corner.")]
    [SerializeField] private FillDirection fillDirection;
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public VisualBehaviour VisualBehaviour { get { return visualBehaviour; } }
    public FillDirection FillDirection { get { return fillDirection; } }
    
    public SmoothType SmoothType { get { throw new Exception("Stage progress bars not implements smoothing."); } }
    public float SmoothDuration { get { throw new Exception("Stage progress bars not implements  smoothing."); } }
    
    public bool Finished { get; private set; }
    public bool Decrease { get; private set; }
    
    public OnFinishProgress<T> FinishProgress { get; private set; }

    public static Action<InitialData<T>> InitializeProgress;
    public static Action<UpdateData<T>> UpdateProgress;
    public static Action<OnFinishProgress<T>> OnProgressFinished;           // Subscription is done using delegate
                                                                            // ex : SomeProgressBar.OnProgressFinished += delegate { DoSomething(); };
                                                                            // ex : SomeProgressBar.OnProgressFinished += delegate { DoSomething("SomeParam"); };
                                                                            
    private void OnEnable()
    {
        InitializeProgress += Initialize;
        UpdateProgress += UpdateCurrentProgress;
    }

    private void OnDisable()
    {
        InitializeProgress -= Initialize;
        UpdateProgress -= UpdateCurrentProgress;
    }

    public void Initialize(InitialData<T> initializationData)
    {
        FinishProgress = new OnFinishProgress<T>();

        Finished = false;
        MinValue = initializationData.MinValue;
        MaxValue = initializationData.MaxValue;
        CurrentValue = initializationData.CurrentValue;
        if (CurrentProgress() >= 1)
        {
            Decrease = true;
        }

        Debug.Log
        ($"Initialized {typeof(T)} with Values (click for full details)" +
         $"\n MinValue = {MinValue}, MaxValue = {MaxValue}, CurrentValue = {CurrentValue}." +
         $"\n Is this progress decreasing?={Decrease}." +
         $"\n Visual behaviour == {VisualBehaviour}" +
         $"\n Fill direction == {FillDirection}");
        
        InitializeVisual();
    }

    public void InitializeVisual()
    {
        if(stagePoints != null)
        {
            for (int i = 0; i < stagePoints.Length; i++)
            {
                Destroy(stagePoints[i].gameObject);
            }
        }

        stagePoints = new Image[(int)MaxValue];

        for (int i = 0; i < MaxValue; i++)
        {
            stagePoints[i] = Instantiate(stagePointImagePrefab, stagePointsParentPanel.transform).GetComponent<Image>();
            stagePoints[i].preserveAspect = true;
            SetStagePointSprite(i, nextStageSprite);
        }

        if (fillDirection == FillDirection.Reverted)
        {
            System.Array.Reverse(stagePoints);
        }

        SetStagePointSprite(0, currentStageSprite);
    }

    public void UpdateVisual()
    {
        int completeStageIndex = (int)CurrentVisualValue() - 1;
        int currentStageIndex = (int)CurrentVisualValue();
        int nextStageIndex = (int)CurrentVisualValue() + 1;

        if (IndexIsNotOutOfRange(nextStageIndex, 1, stagePoints.Length))
        {
            SetStagePointSprite(nextStageIndex, nextStageSprite);
        }
        if (IndexIsNotOutOfRange(completeStageIndex, 0, stagePoints.Length))
        {
            SetStagePointSprite(completeStageIndex, completeStageSprite);

            if (Animate)
            {
                AnimateStagePoint(completeStageIndex);
            }
        }
        if (IndexIsNotOutOfRange(currentStageIndex, 0, stagePoints.Length))
        {
            SetStagePointSprite(currentStageIndex, currentStageSprite);
        }
    }
    
    public float CurrentVisualValue()
    {
        float CurrentVisualValue;

        if (VisualBehaviour == VisualBehaviour.InvertedRelativeToActual)
        {
            CurrentVisualValue = MaxValue - (CurrentValue-1);
        }
        else
        {
            CurrentVisualValue = CurrentValue;
        }
        
        return CurrentVisualValue;
    }

    private bool IndexIsNotOutOfRange(int index, int min, int max)
    {
        return (index >= min && index < max);
    }

    private void SetStagePointSprite(int pointIndex, Sprite sprite)
    {
        stagePoints[pointIndex].sprite = sprite;
    }

    private void AnimateStagePoint(int completeStageIndex)
    {
        Animator completeStagePointAnimator = stagePoints[completeStageIndex].GetComponent<Animator>();
        if (AnimationHash == 0 || completeStagePointAnimator == null)
        {
            Debug.LogWarning("If you want to animate stage point set 'AnimationHash' in stage bar script and add an Animator component to stage point prefab with necessary animation.");
        }
        else
        {
            completeStagePointAnimator.CrossFadeInFixedTime(AnimationHash, 0, 0, 0);
        }
    }

    public void UpdateCurrentProgress(UpdateData<T> progressData)
    {
        CurrentValue = progressData.CurrentValue;

        CheckProgress();

        UpdateVisual();
    }

    public float CurrentProgress()
    {
        float CurrentProgress = (CurrentValue - MinValue) / (MaxValue - MinValue);
        //Debug.Log($"Current progress in {this} == {CurrentProgress}");
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
                    Debug.Log($"Finish was triggered in {typeof(T)} with CurrentProgress() <= 0");
                    OnProgressFinished?.Invoke(FinishProgress);
                    Finished = true;
                }
            }
            else
            {
                if (CurrentProgress() >= 1)
                {
                    Debug.Log($"Finish was triggered in {typeof(T)} with CurrentProgress() >= 1");
                    OnProgressFinished?.Invoke(FinishProgress);
                    Finished = true;
                }
            }
        }
    }
}
