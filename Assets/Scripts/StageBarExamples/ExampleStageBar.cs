using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleStageBar : SceneStageProgressBar<ExampleStageBar>
{
    private void Awake()
    {
        AnimationHash = Animator.StringToHash("StageComplete");
    }
}
