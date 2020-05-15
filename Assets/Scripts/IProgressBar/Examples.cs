
/// SceneLineProgressBar/SceneStageProgressBar:
/// в случае нужды прогресс бара в сцене например как прогрессбар здоровья дерева которое нужно срубить
/// создаем новый скрипт и просто наследуем его от нужного типа прогресс бара
/// в нашем случае LineProgressBar
/// 
/// в итоге мы должны получить ПУСТОЙ скрипт
public class HealthExampleTreeProgressBar : SceneLineProgressBar<HealthExampleTreeProgressBar>
{
}
/// этот скрипт вешаем на прогресс бар и вставляем в поле нужную картинку 
/// которая исполняет роль прогрессбара (то есть будет заполняться/убавляться)
/// 
/// в зависимости от необходимости можно установить тип обновления прогресс бара
/// SmoothType.None - стоит по дефолту, обновление прогресса и фактического и визуального
/// происходит сразу после вызова соответствующего метода/события
/// SmoothType.VisuallyOnly - фактический прогресс будет обновлен моментально
/// и значит если CurrentValue достигло нужного значения OnProgressFinished будет вызван моментально
/// НО визуально прогресс будет плавно обновлен в течение времени ~ Duration
/// SmoothType.ActuallyAndVisually - прогресс будет плавно обновлен в течение времени ~ Duration и визульно и фактически
/// и значит если CurrentValue в контроллере (тот кто вызывает метод/событие обновления)
/// достигло нужного значения, OnProgressFinished будет вызван с задержкой ~ Duration
///
/// инвертировать направление заполнения прогресса (слева-направо/справа-налево или сверху-вниз/снизу-вверх)
/// можно с помощью свойства Fill Direction
/// инвертировать направление визуального прогресса ОТНОСИТЕЛЬНО фактического
/// можно установив VisualBehaviour == Reverted
/// далее в нашем случае например в скрипте дерева нам нужно воспроизводить все необходимые манипуляции с этим прогресс баром 
/// с помощью событий которые он предоставляет
/// 
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

        healthInitData.MinValue = 0;
        healthInitData.MaxValue = maxHealth;
        healthInitData.CurrentValue = maxHealth;

        HealthExampleTreeProgressBar.InitializeProgress(healthInitData);
    }

    // ну и естественно нам нужно обновить прогресс бар, что мы и делаем при каждом изменении прогресса

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthUpdateData.CurrentValue = currentHealth;
        HealthExampleTreeProgressBar.UpdateProgress(healthUpdateData);
    }
}
/// И похожий пример контроллера в случае если например нужно срубить несколько деревьев
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
        HealthExampleTreeProgressBar.OnProgressFinished += delegate { InstantiateNewTree(); }; // при срубе каждого из деревьев спавнится новое которое в свою очередь
        // инициализирует прогресс бар здоровья дерева по новой
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

/// с GameObjectLineProgressBar все проще
/// создаем прогресс бар и вешаем на него такой же пустой скрипт
/// цепляем этот прогресс бар на объект и в классе объекта создаем ссылку на этот бар
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