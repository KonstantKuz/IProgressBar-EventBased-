
public interface IProgressBar
{
    /// <summary>
    /// Sets automatically
    /// Required for a one-time call OnProgressFinished event on GOProgressBar/SceneProgressBar.
    /// </summary>
    bool Finished { get; }
    
    /// <summary>
    /// MinValue represents the bottom border of progress.
    /// MinValue should always be less than MaxValue.
    /// Represents 0% of progress in percentage ratio.
    /// </summary>
    float MinValue { get; }
    
    /// <summary>
    /// MaxValue represents the top border of progress.
    /// Represents 100% of progress in percentage ratio.
    /// </summary>
    float MaxValue { get; }

    float CurrentValue { get; }
    
    VisualBehaviour VisualBehaviour { get; }
    FillDirection FillDirection { get; }
    SmoothType SmoothType { get; }
    float SmoothDuration { get; }
    
    /// <summary>
    /// Set automatically
    /// If CurrentValue == MaxValue -> Decrease = true -> OnProgressFinished will be called only when CurrentProgress() == 0.
    /// If CurrentValue == MinValue -> Decrease = false -> OnProgressFinished wil be called only when CurrentProgress() == 1.
    /// </summary>
    bool Decrease { get; }
    
    /// <summary>
    /// Returns the current progress value in percentage ratio (0, 1).
    /// Uses formula : (CurrentValue - MinValue)/(MaxValue - MinValue)
    /// </summary>
    /// <returns>(CurrentValue - MinValue)/(MaxValue - MinValue)</returns>
    float CurrentProgress();

    void InitializeVisual();
    void UpdateVisual();
    
    void CheckProgress();
}
public enum FillDirection
{
    /// <summary> MinValue of progress will be on left/top border of rect and moves from or to this border </summary>
    Original,
    
    /// <summary> MinValue of progress will be on right/bottom border of rect and moves from or to this border </summary>
    Reverted,
}
public enum VisualBehaviour
{
    /// <summary> Visually progress behaves as like as actual progress </summary>
    SameAsActual,
    
    /// <summary> Visually progress behaves as inverted relative to actual progress </summary>
    InvertedRelativeToActual,
}
public enum SmoothType
{
    /// <summary> Update progress actually and visually instantly </summary>
    None = 0,
    
    /// <summary> Update smoothly visually only </summary>
    VisuallyOnly = 1,
    
    /// <summary> Update smoothly progress actually and visually </summary>
    ActuallyAndVisually = 2,
}