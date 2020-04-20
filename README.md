```C#
/// все это должно помочь легко управлять любыми видами прогресса на сцене и разными видами прогресс баров
/// то есть поможет изи следить и за прогрессом фактическим (триггеря/вызывая необходимые события/методы при достижении прогресса)
/// и за прогрессом визуальным 
/// + с минимальным количеством кода в классах использующих прогрессы
/// + с максимально шаблонным и однотипным кодом в классах использующих прогрессы
/// без необходимости делать из него сервис
/// удобство в том, что независимо от типа и вида прогресса и прогресс бара обращение к нему производится одними и теми же способами
/// в случае одиночных прогресс баров - обращение с ними происходит строго через три дженерик события
/// в случае множества прогресс баров - обращение через методы интерфейса посредством их вызова из экземпляра класса в котором нужно следить и визуализировать прогресс
/// (например здоровье машинки которых в сцене может быть много - управление прогресс баром производится из объекта машинки без какой либо необходимости менять код прогресс бара)
/// 
/// то есть в итоге независимо от типа и вида прогресс бара обращение с ним будет максимально однотипным
/// все прогрессы будут работать по шаблону == быстрее будем работать)
/// 
/// по идее все должно быть так))
/// 
/// 
/// КАК ПОЛЬЗОВАТЬСЯ :
/// допустим нужен новый прогресс бар в сцене например как прогрессбар здоровья дерева
/// создаем новый скрипт и просто наследуем его от нужного типа прогресс бара
/// в нашем случае LineProgressBar
/// 
/// в итоге мы должны получить ПУСТОЙ скрипт
public class HealthExampleTreeProgressBar : LineProgressBar<HealthExampleTreeProgressBar>
{
}
/// этот скрипт вешаем на прогресс бар и вставляем в поле нужную картинку которая исполняет роль прогрессбара (то есть заполняется/убавляется с помощью свойства fillAmount)
/// сам прогресс бар готов, инвертировать визуальное направление прогресса можно с помощью RevertVisual
/// далее в нашем случае например в скрипте дерева нам нужно инициализировать его с помощью события следущим образом
/// InitializeProgressIn<TreeHealthProgressBar>
/// 
public class ExampleTree : IEventSubscriber, IEventPublisherWithParams
{
    private float maxHealth;
    private float currentHealth;

    InitializeProgressIn<HealthExampleTreeProgressBar> healthInitEvent = new InitializeProgressIn<HealthExampleTreeProgressBar>();
    UpdateProgressIn<HealthExampleTreeProgressBar> healthUpdateEvent = new UpdateProgressIn<HealthExampleTreeProgressBar>();

    void OnEnable()
    {
        SubscribeToNecessaryEvents();
    }
    
    public void SubscribeToNecessaryEvents()
    {
        EventAggregator.Subscribe<OnProgressFinishedIn<HealthExampleTreeProgressBar>>(Cut);
    }

    void Cut()
    {
        //DEATH
    }

    void Start()
    {
        InitializeHealthBar();
    }

    void InitializeHealthBar()
    {
        healthInitEvent.MinValue = 0;
        healthInitEvent.MaxValue = maxHealth;
        healthInitEvent.CurrentValue = maxHealth; // пока что currentHealth должен быть равен maxHealth
                                                  // я поработаю над тем чтоб было без разницы но пока в этом нужды не было

        PublishWithParams(healthInitEvent);
    }

    void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthUpdateEvent.CurrentValue = currentHealth;
        PublishWithParams(healthUpdateEvent);
    }

    public void PublishWithParams<T>(T publishedEvent) where T : class
    {
        EventAggregator.Publish(publishedEvent);
    }
}

/// И похожий пример контроллера в случае если например нужно срубить несколько деревьев
public class StageExampleTreeProgressBar : LineProgressBar<StageExampleTreeProgressBar>
{
}
public class ExampleTreeController : IEventSubscriber, IEventPublisherWithParams, IEventPublisherWithOutParams
{
    private int treesCountToWin;
    private int cuttedTreesCount;

    InitializeProgressIn<StageExampleTreeProgressBar> stageInitEvent = new InitializeProgressIn<StageExampleTreeProgressBar>();
    UpdateProgressIn<StageExampleTreeProgressBar> stageUpdateEvent = new UpdateProgressIn<StageExampleTreeProgressBar>();

    void OnEnable()
    {
        SubscribeToNecessaryEvents();
    }

    public void SubscribeToNecessaryEvents()
    {
        EventAggregator.Subscribe<OnProgressFinishedIn<HealthExampleTreeProgressBar>>(InstantiateNewTree); // подписка на событие завершения прогресса в HealthBar

        EventAggregator.Subscribe<OnProgressFinishedIn<HealthExampleTreeProgressBar>>(FinishGame);         // подписка на событие завершения прогресса в StageBar
    }

    void Start()
    {
        InitializeStageBar();
    }

    void InitializeStageBar()
    {
        stageInitEvent.MinValue = 0;
        stageInitEvent.MaxValue = treesCountToWin;
        stageInitEvent.CurrentValue = 0;

        PublishWithParams(stageInitEvent);
    }

    void InstantiateNewTree()
    {
        cuttedTreesCount++;
        //Instantiate new tree
        UpdateStageProgress();
    }

    void UpdateStageProgress()
    {
        stageUpdateEvent.CurrentValue = cuttedTreesCount;
        PublishWithParams(stageUpdateEvent);
    }

    void FinishGame()
    {
        //Finish
        Publish<OnFinish>(); // публикуем глобальное событие финиша
    }

    public void PublishWithParams<T>(T publishedEvent) where T : class
    {
        EventAggregator.Publish(publishedEvent);
    }

    public void Publish<T>() where T : IEventBase
    {
        EventAggregator.Publish<T>();
    }
}
```
# Unity Event Aggregator

## About

Simple .net event aggregator for Unity engine. 

## Where to use it

In every project that is event heavy or when you need to decouple your publishers from your subscibers.

## How to use it

import the namespace:  
```C#
using EventAggregation;
```

You can get access to the EventAggregator at anytime by just typing `EventAggregator`.

### 1. Publish an event

When you need to publish an event:  
```C#
EventAggregator.Publish(MyEventType())
```

### 2. Subscribe to an event

In any part of your script or in Unity's callbacks (Start, Awake, et...):  
```C#
EventAggregator.Subscribe<MyEventType>(MyMethodName)
```

### 3. Custom event arguments

Events are defined by a custom class, which can hold additional data. The class needs to implement the `IEventBase` interface. You can extend, inherit and derive from this class as you want as long as it implements the interface.

You can add your custom classes to the `Events.cs` file to keep things organized.

#### a. Create your own class

```C#
public class MyEventType : MyEventTypeBase, IEventBase
{
    public int Index { get; set; }
    public Coordinates Coordinates { get; set; }
}

public abstract class MyEventTypeBase
{
    public List<int> Numbers {get; set; }
}
```

#### b. Passing arguments

```C#
var myType = new MyEventType() 
{
    Numbers = new List<int>() { 0, 1, 2 },
    Index = 42,
    Coordinates = new Coordinates()
}
EventAggregator.Publish(myType);
```

#### c. Get the data
```C#
private void MyMethodName(IEventData eventData)
{
    var arg = eventData as MyEventType;
    if (arg != null)
    {
        // Do whatever you want with your data
    }
}
```

### 4. Miscellaneous

You can unsubscribe from any event at anytime by calling:
```C#
EventAggregator.Unsubscribe<MyEventType>(MyMethodName);
```

You can check if your event is correctly registered by calling:
```C#
bool b = EventAggregator.IsRegistered<MyEventType>(MyMethodName);
```

You can delete all the subscribers by calling:
```C#
EventAggregator.UnsubscribeAll();
```

You can delete all the subscribers from a designated type by calling:
```C#
EventAggregator.UnsubscribeAll<MyEventType>();
```