using System;

public interface GameObjectProgressBar : IProgressBar
{
    Action OnProgressFinished { get; set; }

    void Initialize(float minValue, float maxValue, float currentValue);
    void UpdateCurrentProgress(float currentValue);
}