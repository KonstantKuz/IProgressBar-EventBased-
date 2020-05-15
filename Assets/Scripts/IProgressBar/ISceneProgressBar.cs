
public interface SceneProgressBar<T> : IProgressBar where T : class
{
    OnFinishProgress<T> FinishProgress { get; }

    void Initialize(InitialData<T> initData); 
    void UpdateCurrentProgress(UpdateData<T> progressData); 
}

public struct InitialData<SceneProgressBar>
{
    public float MinValue;
    public float MaxValue;
    public float CurrentValue;
}
public struct UpdateData<SceneProgressBar>
{
    public float CurrentValue;
}
public struct OnFinishProgress<SceneProgressBar>
{
}