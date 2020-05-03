﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public interface IProgressBar
{
    float CurrentProgress();       // значение прогресса считается по формуле
                                   // (CurrentValue - MinValue)/(MaxValue - MinValue)
                                   // которое в итоге возвращает значение прогресса в промежутке от 0 до 1

                                   // actual progress in percentage (returns value between 0 and 1)
                                   // calculates using formula (CurrentValuue - MinValue)/(MaxValue - MinValue)


    bool Decrease { get; }         // необходимо для проверки поведения фактического прогресса
                                   // устанавливается автоматически при инициализации
                                   // и означает что если CurrentValue == MaxValue значит от прогресса ожидается уменьшение 
                                   // и вызов события OnProgressFinished будет осуществлен только в случае CurrentProgress == 0
                                   // в случае CurrentValue == MinValue ожидается увеличение
                                   // и вызов события OnProgressFinished будет осуществлен только в случае CurrentProgress == 1
                                   
                                   // representation of actual progress behaviour
                                   // should be sets automatically on initialization
                                   // if CurrentValue == MaxValue progress will be decreasing (need to set Decrease = true)
                                   // and OnProgressFinished will be called only when CurrentProgress() == 0
                                   // if CurrentValue == MinValue progress will be increasing (need to set Decrease = false)
                                   // and OnProgressFinished will be called only when CurrentProgress() == 1


    bool Finished { get; }         // private and needs just for make only one call OnProgressFinished
    
    
    float MinValue { get; }        // MinValue олицетворяет нижнюю границу прогресса
                                   // в зависимости от поведения прогресса (увеличение/уменьшение)
                                   // будет производиться соответствующий отсчет (от минимального/к минимальному)
                                   // MinValue всегда должен быть меньше MaxValue и в процентном соотношении олицетворяет 0%

                                   // MinValue presented as bottom point of progress
                                   // in dependency from progress beahviour (increases or decreases)
                                   // progress will be calculate from MinValue or to MinValue
                                   // MinValue always should be less than MaxValue
                                   // in percents presented as 0% (or actually 0 as CurrentProgress())

    float MaxValue { get; }        // MaxValue олицетворяет верхнюю границу прогресса
                                   // в зависимости от поведения прогресса (увеличение/уменьшение) 
                                   // будет производиться соответствующий отсчет (к максимальному/от максимального)
                                   // процентном соотношении олицетворяет 100%
                                   
                                   // MaxValue presented as top point of progress
                                   // in dependency from progress beahviour (increases or decreases)
                                   // in percents presented as 100% (or actually 1 as CurrentProgress())

    float CurrentValue { get; }    // при инициализации значение CurrentValue нужно задавать в зависимости от того какое поведение у прогресса
                                   // если прогресс увеличивается - логично что текущее значение должно равняться минимальному значению
                                   // если прогресс уменьшается - логично что текущее значение должно равняться максимальному значению
                                   
                                   // in dependency from progress beahviour (increases or decreases)
                                   // CurrentValue should be соответствующим
                                   // if progress increases - CurrentValue sholud be equals to MinValue
                                   // if progress decreases - CurrentValue should be equals to MaxValue


    bool RevertVisual { get; }     // инвертирует поведение прогресс бара визуально
                                   
                                   // just for invert visual representaion of progress

    
    void UpdateUI();               // метод в котором обновляется визуал прогресса который в идеале может быть каким угодно)
                                   
                                   // method in wich visual representation of progress should be updated

    void CheckProgress();          // метод в котором производится проверка текущего значения прогресса
                                   // и триггерится событие OnProgressFinished
                                   
                                   // method in wich checks CurrentProgress()
                                   // and calls OnProgressFinished

                                   // примечание : например тип TreeHealthIndicator и тип SheepShaveProgressBar могут легко находиться на одной сцене
                                   // и легко управляться разными сущностями с помощью событий
                                   // но только в случае если TreeHealthIndicator и SheepShaveProgressBar в сцене находятся по одному экземпляру
}

/// SceneProgressBar - интерфейс необходимый для реализации уникальных для конкретной сцены прогресс баров и работы с ним посредством событий
/// такие прогресс бары должны управляться ИСКЛЮЧИТЕЛЬНО посредством событий таких как
/// InitializeProgress UpdateProgress 
/// OnProgressFinished 
/// SceneProgressBar - needs for unique progress bars wiht only one instance on whole scene
/// initialization and update of this type of progress bars 
/// makes only using соответствующие static events InitializeProgress & UpdateProgress

public interface SceneProgressBar<T> : IProgressBar where T : class
{
    OnFinishProgress<T> FinishProgress { get; }

    void Initialize(InitialData<T> initData);                // в качестве параметров initData и progressData
    void UpdateCurrentProgress(UpdateData<T> progressData);  // выступают контейнеры InitialData<КонкретныйПрогрессБар> и UpdateData<КонкретныйПрогрессБар>
                                                             // в которых содержатся необходимые параметры
                                                             
                                                             // parameters  initData and progressData represented as containers with necessary values
                                                             // InitialData<ConcreteProgressBar> UpdateData<ConcreteProgressBar>
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
                                   
                                   // on any change of progress should be called UpdateProgress
                                   // for example in case with health progress bar in parameter CurrentValue should be written current health
}
public struct OnFinishProgress<SceneProgressBar>
{
}


public enum SmoothType
{
    /// <summary> Update actual progress and visual instantly </summary>
    None = 0,
    /// <summary> Update smoothly visual only </summary>
    VisuallyOnly = 1,
    /// </summary> Update smoothly actual progress and visual </summary>
    ActuallyAndVisually = 2,
}

public class SceneLineProgressBar<T> : MonoBehaviour, SceneProgressBar<T> where T : class
{
    [SerializeField] private Image progressBarImage;

    [SerializeField] private bool revert;
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public bool RevertVisual { get { return revert; } }
    public bool Finished { get; private set; }
    public bool Decrease { get; private set; }
    
    public OnFinishProgress<T> FinishProgress { get; private set; }
    
    public static Action<InitialData<T>> InitializeProgress;
    public static Action<UpdateData<T>> UpdateProgress;
    public static Action<OnFinishProgress<T>> OnProgressFinished;         // подписку проще осуществлять с помощью delegate
                                                                          // пр-р : КонкретныйПрогрессБар.OnProgressFinished += delegate { метод/функция с заранее определенными параметрами };
                                                                          //
                                                                          // subscribe only using delegate : ConcreteProgressBar.OnProgressFinished += { SomeFunc(0, Vector3.forward); };
                                                                          
    [SerializeField] private SmoothType smoothType = SmoothType.None;
    [SerializeField] private float duration = 0;
    private WaitForFixedUpdate waitForFixedFrame = new WaitForFixedUpdate();

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
            $"\n Is this progress visual reverted?= {RevertVisual}");

        UpdateUI();
    }

    public void UpdateUI()
    {
        progressBarImage.fillAmount = CurrentVisualProgress();
    }

    public float CurrentVisualProgress()
    {
        float CurrentVisualProgress;

        if (RevertVisual)
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

public class ScenePointsProgressBar<T> : MonoBehaviour, SceneProgressBar<T> where T : class
{
    [SerializeField] private Sprite currentStageSprite;
    [SerializeField] private Sprite completeStageSprite;
    [SerializeField] private GameObject stagePointImagePrefab;
    [SerializeField] private GameObject stagePointsParentPanel;
    [SerializeField] private bool Animate;

    private Image[] stagePoints;
    private int AnimationHash = Animator.StringToHash("StageComplete");

    [SerializeField] private bool revert;
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public bool RevertVisual { get { return revert; } }
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
            $"\n Is this progress visual reverted?= {RevertVisual}");
        //логика инициализации и обновления визуала должны быть единственным изменением в коде шаблона ПОМИМО замены имени класса

        InitializeUI();  // в этом прогресс баре как и возможно в других требуется дополнительно логика инициализации визуала

        UpdateUI();
    }

    private void InitializeUI()
    {
        stagePoints = new Image[(int)MaxValue];

        for (int i = 0; i < MaxValue; i++)
        {
            stagePoints[i] = Instantiate(stagePointImagePrefab, stagePointsParentPanel.transform).GetComponent<Image>();
        }

        if (RevertVisual)
        {
            System.Array.Reverse(stagePoints);            // магия реверса визула тут происходит совсем по другому
                                                          // однако код реализации интерфейсов по большей части не изменен в обоих шаблонах
                                                          // если вдруг понадобится еще один нетипичный вид прогресс бара
                                                          // и если я окажусь прав, то написать его по шаблону не составит большого труда
                                                          // а даже если и составит то все равно самое главное это то
                                                          // что работать с ним в итоге все равно можно будет так же как и с этими двумя
        }

        stagePoints[0].sprite = currentStageSprite;
    }

    public void UpdateUI()
    {
        if (CurrentValue != 0)
        {
            int completeStageIndex = (int)CurrentValue - 1;
            int currentStageIndex = (int)CurrentValue;

            stagePoints[completeStageIndex].sprite = completeStageSprite;
            if(Animate)
                stagePoints[completeStageIndex].GetComponent<Animator>().Play(AnimationHash);

            if (CurrentValue != MaxValue)
                stagePoints[currentStageIndex].sprite = currentStageSprite;

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

    [SerializeField] private bool revert;
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public bool RevertVisual { get { return revert; } }
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
            Decrease = true;

        Debug.Log($"Initialized {gameObject.name} with Values (click for full details)" +
            $"\n MinValue = {MinValue}, MaxValue = {MaxValue}, CurrentValue = {CurrentValue}." +
            $"\n Is this progress decreasing?={Decrease}." +
            $"\n Is this progress visual reverted?= {RevertVisual}");

        UpdateUI();
    }

    public void UpdateUI()
    {
        progressBarImage.fillAmount = CurrentVisualProgress();
    }

    public float CurrentVisualProgress()
    {
        float CurrentVisualProgress;

        if (RevertVisual)
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



/// КАК ПОЛЬЗОВАТЬСЯ SceneLineProgressBar/ScenePointsProgressBar:
/// допустим нужен новый прогресс бар в сцене например как прогрессбар здоровья дерева которое нужно срубить
/// создаем новый скрипт и просто наследуем его от нужного типа прогресс бара
/// в нашем случае LineProgressBar
/// 
/// в итоге мы должны получить ПУСТОЙ скрипт
/// 
/// How to use SceneLineProgressBar/ScenePointsProgressBar
/// if you need to control progress of something in scene
/// and the progress can be only one in scene (for example health of tree that we need to chop)
/// create new fully epmty script and inheirit it from SceneLineProgressBar just like that
/// 
public class HealthExampleTreeProgressBar : SceneLineProgressBar<HealthExampleTreeProgressBar>
{
}

/// put this script to progress bar gameobject and fill all properties
/// if you need to smooth progress you can set it using SmoothType
/// 
/// SmoothType.None - visually and actually progress will be updated instantly
/// 
/// SmoothType.VisuallyOnly - progress will be updated smoothly visually only
/// OnProgressFinished will be called instantly if CurrentValue will be 
/// 
/// SmoothType.ActuallyAndVisually - progress will be updated smoothly visually and actually
/// 
/// этот скрипт вешаем на прогресс бар и вставляем в поле нужную картинку 
/// которая исполняет роль прогрессбара (то есть заполняется/убавляется с помощью свойства fillAmount)
/// в зависимости от необходимости можно установить тип обновления прогресс бара
/// SmoothType.None - стоит по дефолту, обновление прогресса и фактического и визуального происходит сразу после вызова соответствующего метода/события
/// SmoothType.VisuallyOnly - фактический прогресс будет обновлен моментально
/// и значит если CurrentValue достигло нужного значения OnProgressFinished будет вызван моментально
/// НО визуально прогресс будет плавно обновлен в течение времени ~ Duration
/// SmoothType.ActuallyAndVisually - прогресс будет плавно обновлен в течение времени == Duration и визульно и фактически
/// и значит если CurrentValue в контроллере (тот кто вызывает метод/событие обновления)
/// достигло нужного значения, OnProgressFinished будет вызван с задержкой ~ Duration
/// сам прогресс бар готов, инвертировать визуальное направление прогресса можно с помощью RevertVisual
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
        SubscribeToNecessaryEvents();
    }

    private void SubscribeToNecessaryEvents()
    {
        HealthExampleTreeProgressBar.OnProgressFinished += delegate { FallDown(); } ;
    }

    private void FallDown()
    {
        //Fall
    }

    private void Start()
    {
        currentHealth = maxHealth;

        InitializeHealthBar();
    }

    private void InitializeHealthBar()
    {
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
public class StageExampleTreeProgressBar : SceneLineProgressBar<StageExampleTreeProgressBar>
{
}
public class ExampleTreeController
{
    private int treesCountToWin;
    private int cuttedTreesCount;

    InitialData<StageExampleTreeProgressBar> stageInitData;
    UpdateData<StageExampleTreeProgressBar> stageUpdateData;

    private void OnEnable()
    {
        SubscribeToNecessaryEvents();
    }

    private void SubscribeToNecessaryEvents()
    {
        HealthExampleTreeProgressBar.OnProgressFinished += delegate { InstantiateNewTree(); };  // при срубе каждого из деревьев спавнится новое которое в свою очередь
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
        stageInitData.MaxValue = treesCountToWin;
        stageInitData.CurrentValue = 0;

        StageExampleTreeProgressBar.InitializeProgress(stageInitData);
    }

    private void InstantiateNewTree()
    {
        cuttedTreesCount++;
        //Instantiate new tree
        UpdateStageProgress();
    }

    private void UpdateStageProgress()
    {
        stageUpdateData.CurrentValue = cuttedTreesCount;
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