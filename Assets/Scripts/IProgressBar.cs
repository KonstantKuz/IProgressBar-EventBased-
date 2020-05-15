using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public interface IProgressBar
{
    bool Finished { get; }         // необходимо для одноразового вызова события OnProgressFinished
    
    
    float MinValue { get; }        // MinValue олицетворяет нижнюю границу прогресса
                                   // в зависимости от поведения прогресса (увеличение/уменьшение)
                                   // будет производиться соответствующий отсчет (от минимального/к минимальному)
                                   // MinValue всегда должен быть меньше MaxValue и в процентном соотношении олицетворяет 0%

    float MaxValue { get; }        // MaxValue олицетворяет верхнюю границу прогресса
                                   // в зависимости от поведения прогресса (увеличение/уменьшение) 
                                   // будет производиться соответствующий отсчет (к максимальному/от максимального)
                                   // процентном соотношении олицетворяет 100%

    float CurrentValue { get; }    // при инициализации значение CurrentValue нужно задавать в зависимости от того какое поведение у прогресса
                                   // если прогресс увеличивается - логично что текущее значение должно равняться минимальному значению
                                   // если прогресс уменьшается - логично что текущее значение должно равняться максимальному значению


    VisualBehaviour VisualBehaviour { get; }    // инвертирует поведение прогресс бара визуально
                                                // означает что визуально прогресс будет вести себя противоположно фактическому прогрессу
                                                // если прогресс фактически увеличивается RevertVisual == true
                                                // то визуально прогресс будет уменьшаться
                                   
    FillDirection FillDirection { get; } // меняет начальное направление движения прогресса
                                         // слева-направо или справа-налево

    bool Decrease { get; }         // необходимо для проверки поведения фактического прогресса
                                   // устанавливается автоматически при инициализации
                                   // и означает что если CurrentValue == MaxValue значит от прогресса ожидается уменьшение 
                                   // и вызов события OnProgressFinished будет осуществлен только в случае CurrentProgress == 0
                                   // в случае CurrentValue == MinValue ожидается увеличение
                                   // и вызов события OnProgressFinished будет осуществлен только в случае CurrentProgress == 1


    float CurrentProgress();       // значение прогресса считается по формуле
                                   // (CurrentValue - MinValue)/(MaxValue - MinValue)
                                   // которое в итоге возвращает значение прогресса в промежутке от 0 до 1

    void UpdateUI();               // метод в котором обновляется визуал прогресса который в идеале может быть каким угодно)

    void CheckProgress();          // метод в котором производится проверка текущего значения прогресса
                                   // и триггерится событие OnProgressFinished

                                   // примечание : например тип TreeHealthIndicator и тип SheepShaveProgressBar могут легко находиться на одной сцене
                                   // и легко управляться разными сущностями с помощью событий
                                   // но только в случае если TreeHealthIndicator и SheepShaveProgressBar в сцене находятся по одному экземпляру
}

/// SceneProgressBar - интерфейс необходимый для реализации уникальных для конкретной сцены прогресс баров и работы с ним посредством событий
/// такие прогресс бары должны управляться ИСКЛЮЧИТЕЛЬНО посредством событий таких как
/// InitializeProgress UpdateProgress 
/// OnProgressFinished 
public enum FillDirection
{
    Original,
    Reverted,
}

public enum VisualBehaviour
{
    AsActual,
    Reverted
}

public interface SceneProgressBar<T> : IProgressBar where T : class
{
    OnFinishProgress<T> FinishProgress { get; }

    void Initialize(InitialData<T> initData);                // в качестве параметров initData и progressData 
    void UpdateCurrentProgress(UpdateData<T> progressData);  // выступают контейнеры InitialData<КонкретныйПрогрессБар> и UpdateData<КонкретныйПрогрессБар>
                                                             // в которых содержатся необходимые параметры
}
public struct InitialData<SceneProgressBar>
{
    public float MinValue;
    public float MaxValue;
    public float CurrentValue;
}
public struct UpdateData<SceneProgressBar>
{
    public float CurrentValue;     // при любом изменении прогресса и соответственно при вызове события обновления
                                   // необходимо передавать текущее значение конкретной величины
                                   // например в случае прогресс баром здоровья в параметр должно записываться текущее здоровье
}
public struct OnFinishProgress<SceneProgressBar>
{
}


public enum SmoothType
{
    /// <summary> Update progress and visual instantly </summary>
    None = 0,
    /// <summary> Update smoothly visual only </summary>
    VisuallyOnly = 1,
    /// </summary> Update smoothly progress and visual </summary>
    ActuallyAndVisually = 2,
}

public class SceneLineProgressBar<T> : MonoBehaviour, SceneProgressBar<T> where T : class
{
    [SerializeField] private Image progressBarImage;

    [SerializeField] private VisualBehaviour visualBehaviour;
    [SerializeField] private FillDirection fillDirection;
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public VisualBehaviour VisualBehaviour { get { return visualBehaviour; } }
    public FillDirection FillDirection { get { return fillDirection; } }
    public bool Finished { get; private set; }
    public bool Decrease { get; private set; }
    
    public OnFinishProgress<T> FinishProgress { get; private set; }
    
    public static Action<InitialData<T>> InitializeProgress;
    public static Action<UpdateData<T>> UpdateProgress;
    public static Action<OnFinishProgress<T>> OnProgressFinished;         // подписку проще осуществлять с помощью delegate
                                                                          // пр-р : КонкретныйПрогрессБар.OnProgressFinished += delegate { метод/функция с заранее определенными параметрами };

    [SerializeField] private SmoothType smoothType = SmoothType.None;
    [SerializeField] private float duration = 0;
    private WaitForFixedUpdate waitForFixedFrame = new WaitForFixedUpdate();

    private void OnEnable()
    {
        InitializeProgress += Initialize;
        UpdateProgress += UpdateCurrentProgress;
        //SetUpSmoothing += SetSmoothing;
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

        SetUpProgressImage();

        Debug.Log($"Initialized {typeof(T)} with Values (click for full details)" +
                  $"\n MinValue = {MinValue}, MaxValue = {MaxValue}, CurrentValue = {CurrentValue}." +
                  $"\n Is this progress decreasing?={Decrease}." +
                  $"\n Visual behaviour == {VisualBehaviour}" +
                  $"\n Fill direction == {FillDirection}");

        UpdateUI();
    }

    private void SetUpProgressImage()
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

    public void UpdateUI()
    {
        progressBarImage.fillAmount = CurrentVisualProgress();
    }

    public float CurrentVisualProgress()
    {
        float CurrentVisualProgress;

        if (VisualBehaviour == VisualBehaviour.Reverted)
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

    public void UpdateCurrentProgress(UpdateData<T> progressData)
    {
        switch (smoothType)
        {
            case SmoothType.None:
                {
                    CurrentValue = progressData.CurrentValue;
                    CheckProgress();
                    UpdateUI();
                }
                break;
            case SmoothType.VisuallyOnly:
                {
                    CurrentValue = progressData.CurrentValue;
                    CheckProgress();
                    StartCoroutine(SmoothUpdateUI());
                }
                break;
            case SmoothType.ActuallyAndVisually:
                {
                    StartCoroutine(SmoothUpdateCurrentProgress(progressData.CurrentValue));
                }
                break;
        }
    }

    private IEnumerator SmoothUpdateCurrentProgress(float currentValue)
    {
        float timeElapsed = 0;
        float startTime = Time.time;

        while(CurrentValue != currentValue)
        {
            timeElapsed = Time.time - startTime;

            CurrentValue = Mathf.MoveTowards(CurrentValue, currentValue, timeElapsed / duration);

            CheckProgress();

            UpdateUI();

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

            progressBarImage.fillAmount = Mathf.MoveTowards(progressBarImage.fillAmount, CurrentVisualProgress(), timeElapsed / (duration * 1250));

            yield return waitForFixedFrame;
        }
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
                    Debug.Log($"Finish was triggered in {typeof(T)} with CurrentProgress() == 0");
                    OnProgressFinished?.Invoke(FinishProgress);
                    Finished = true;
                }
            }
            else
            {
                if (CurrentProgress() >= 1)
                {
                    Debug.Log($"Finish was triggered in {typeof(T)} with CurrentProgress() == 1");
                    OnProgressFinished?.Invoke(FinishProgress);
                    Finished = true;
                }
            }
        }
    }
}

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

    [SerializeField] private VisualBehaviour visualBehaviour;
    [SerializeField] private FillDirection fillDirection;
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public VisualBehaviour VisualBehaviour { get { return visualBehaviour; } }
    public FillDirection FillDirection { get { return fillDirection; } }
    public bool Finished { get; private set; }
    public bool Decrease { get; private set; }
    
    public OnFinishProgress<T> FinishProgress { get; private set; }

    public static Action<InitialData<T>> InitializeProgress;
    public static Action<UpdateData<T>> UpdateProgress;
    public static Action<OnFinishProgress<T>> OnProgressFinished;

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
            Decrease = true;

        Debug.Log($"Initialized {typeof(T)} with Values (click for full details)" +
                  $"\n MinValue = {MinValue}, MaxValue = {MaxValue}, CurrentValue = {CurrentValue}." +
                  $"\n Is this progress decreasing?={Decrease}." +
                  $"\n Visual behaviour == {VisualBehaviour}" +
                  $"\n Fill direction == {FillDirection}");
        
        InitializeUI();
    }

    private void InitializeUI()
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
        }

        if (fillDirection == FillDirection.Reverted)
        {
            System.Array.Reverse(stagePoints);
        }

        SetStagePointSprite(0, currentStageSprite);
    }

    public void UpdateUI()
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

        if (VisualBehaviour == VisualBehaviour.Reverted)
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

        UpdateUI();
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

public interface GameObjectProgressBar : IProgressBar
{
    Action OnProgressFinished { get; set; }

    void Initialize(float minValue, float maxValue, float currentValue);
    void UpdateCurrentProgress(float currentValue);
}

public class GOLineProgressBar : MonoBehaviour, GameObjectProgressBar
{
    [SerializeField] private Image progressBarImage;

    [SerializeField] private VisualBehaviour visualBehaviour;
    [SerializeField] private FillDirection fillDirection;
    
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public VisualBehaviour VisualBehaviour { get { return visualBehaviour; } }
    public FillDirection FillDirection { get { return fillDirection; } }
    public bool Finished { get; private set; }
    public bool Decrease { get; private set; }
    
    public Action OnProgressFinished { get; set; }

    [SerializeField] private SmoothType smoothType = SmoothType.None;
    [SerializeField] private float duration = 0;
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
        
        SetUpProgressImage();

        Debug.Log($"Initialized {transform.parent.name} with Values (click for full details)" +
                  $"\n MinValue = {MinValue}, MaxValue = {MaxValue}, CurrentValue = {CurrentValue}." +
                  $"\n Is this progress decreasing?={Decrease}." +
                  $"\n Visual behaviour == {VisualBehaviour}" +
                  $"\n Fill direction == {FillDirection}");

        UpdateUI();
    }
    
    private void SetUpProgressImage()
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

    public void UpdateUI()
    {
        progressBarImage.fillAmount = CurrentVisualProgress();
    }

    public float CurrentVisualProgress()
    {
        float CurrentVisualProgress;

        if (VisualBehaviour == VisualBehaviour.Reverted)
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
                    UpdateUI();
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

            CurrentValue = Mathf.MoveTowards(CurrentValue, currentValue, timeElapsed / duration);

            CheckProgress();

            UpdateUI();

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

            progressBarImage.fillAmount = Mathf.MoveTowards(progressBarImage.fillAmount, CurrentVisualProgress(), timeElapsed / duration);

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
                    Debug.Log($"Finish was triggered in {gameObject.name} with CurrentProgress() == 0");
                    OnProgressFinished?.Invoke();
                    Finished = true;
                }
            }
            else
            {
                if (CurrentProgress() >= 1)
                {
                    Debug.Log($"Finish was triggered in {gameObject.name} with CurrentProgress() == 1");
                    OnProgressFinished?.Invoke();
                    Finished = true;
                }
            }
        }
    }
}



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
    [SerializeField] private ExampleGOLineProgressBar healthBar;
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