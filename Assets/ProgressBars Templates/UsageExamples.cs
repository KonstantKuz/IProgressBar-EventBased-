
/// Usage examples
///
/// Lets imagine we need a simple progress bar to handle 'health' of the tree we need to cut off
/// We need to create new empty script that inherits from SceneLineProgressBar
/// with itself as generic argument 
public class HealthExampleTreeProgressBar : SceneLineProgressBar<HealthExampleTreeProgressBar>
{
}
/// Add this script to your progress bar and fill all properties
/// All properties have tooltips for best understanding what they representing
/// Next we create our tree to handle his 'health' using  our new progress bar
public class ExampleTree
{
    private float maxHealth;
    private float currentHealth;

    InitialData<HealthExampleTreeProgressBar> healthInitData;
    UpdateData<HealthExampleTreeProgressBar> healthUpdateData;

    private void OnEnable()
    {
        HealthExampleTreeProgressBar.OnProgressFinished += delegate { FallDown(); } ;
    }
    
    private void FallDown()
    {
        //Fall
    }

    private void Start()
    {
        InitializeHealthBar();
    }

    public void InitializeHealthBar()
    {
        currentHealth = maxHealth;

        // All of this properties also have explanations in IProgressBar.cs
        // so if you want you can go to realization and study the work of progress bars deeper
        // to initialize progress you simply need to set up InitialData of HealthExampleTreeProgressBar
        // and send it using InitializeProgress event
        
        healthInitData.MinValue = 0;
        healthInitData.MaxValue = maxHealth;
        healthInitData.CurrentValue = maxHealth;

        HealthExampleTreeProgressBar.InitializeProgress(healthInitData);
    }
    
    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        // On all changes of actual value of some progress 
        // you need to send this changes to appropriate progress bar
        healthUpdateData.CurrentValue = currentHealth;
        HealthExampleTreeProgressBar.UpdateProgress(healthUpdateData);
    }
}
/// There is similar controller to scene where we need to cut a few of trees
public class StageExampleTreeProgressBar : SceneStageProgressBar<StageExampleTreeProgressBar>
{
}
public class ExampleTreeController
{
    private int felledTreesCountToWin;
    private int felledTreesCount;

    InitialData<StageExampleTreeProgressBar> stageInitData;
    UpdateData<StageExampleTreeProgressBar> stageUpdateData;

    private void OnEnable()
    { 
        // as you can see, when we cut one of our trees, we create a new
        // which initializes the HealthExampleTreeProgressBar again
        HealthExampleTreeProgressBar.OnProgressFinished += delegate { InstantiateNewTree(); };
        StageExampleTreeProgressBar.OnProgressFinished += delegate { FinishGame(); };
    }

    private void Start()
    {
        InitializeStageBar();
    }

    private void InitializeStageBar()
    {
        stageInitData.MinValue = 0;
        stageInitData.MaxValue = felledTreesCountToWin;
        stageInitData.CurrentValue = 0;

        StageExampleTreeProgressBar.InitializeProgress(stageInitData);
    }

    private void InstantiateNewTree()
    {
        felledTreesCount++;
        ExampleTree nextTree = new ExampleTree();
        nextTree.InitializeHealthBar();
        
        UpdateStageProgress();
    }

    private void UpdateStageProgress()
    {
        stageUpdateData.CurrentValue = felledTreesCount;
        StageExampleTreeProgressBar.UpdateProgress(stageUpdateData);
    }

    private void FinishGame()
    {
        //Finish
    }
}

/// With GameObject progress bar logic is the same as with others

public class ExampleGOLineProgressBar : GOLineProgressBar
{
}
public class ExampleGOWithHealth
{
    public ExampleGOLineProgressBar healthBar;
    private float currentHP;
    private float maxHP;

    private void OnEnable()
    {
        healthBar.OnProgressFinished += Death;
    }

    private void Death()
    {
        //Death
    }

    private void Start()
    {
        InitializeHealthBar();
    }

    private void InitializeHealthBar()
    {
        healthBar.Initialize(0, maxHP, currentHP);
    }

    private void ApplyDamage(float damage)
    {
        currentHP -= damage;
        healthBar.UpdateCurrentProgress(currentHP);
    }
}