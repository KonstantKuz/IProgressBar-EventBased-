```C#
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
public class ExampleTree
{
    private float maxHealth;
    private float currentHealth;

    InitialData<HealthExampleTreeProgressBar> healthInitData;
    UpdateData<HealthExampleTreeProgressBar> healthUpdateData;

    void OnEnable()
    {
        SubscribeToNecessaryEvents();
    }

    public void SubscribeToNecessaryEvents()
    {
        HealthExampleTreeProgressBar.OnProgressFinished += Cut;
    }

    void Cut(OnFinishProgress<HealthExampleTreeProgressBar> stageCompleteData)
    {
        //DEATH
    }

    void Start()
    {
        InitializeHealthBar();
    }

    void InitializeHealthBar()
    {
        healthInitData.MinValue = 0;
        healthInitData.MaxValue = maxHealth;
        healthInitData.CurrentValue = maxHealth; 

        HealthExampleTreeProgressBar.InitializeProgress.Invoke(healthInitData);
    }

    void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthUpdateData.CurrentValue = currentHealth;
        HealthExampleTreeProgressBar.UpdateProgress.Invoke(healthUpdateData);
    }

}
/// И похожий пример контроллера в случае если например нужно срубить несколько деревьев
public class StageExampleTreeProgressBar : LineProgressBar<StageExampleTreeProgressBar>
{
}
public class ExampleTreeController
{
    private int treesCountToWin;
    private int cuttedTreesCount;

    InitialData<StageExampleTreeProgressBar> stageInitData;
    UpdateData<StageExampleTreeProgressBar> stageUpdateData;

    void OnEnable()
    {
        SubscribeToNecessaryEvents();
    }

    public void SubscribeToNecessaryEvents()
    {
        StageExampleTreeProgressBar.OnProgressFinished += FinishGame;
    }

    void Start()
    {
        InitializeStageBar();
    }

    void InitializeStageBar()
    {
        stageInitData.MinValue = 0;
        stageInitData.MaxValue = treesCountToWin;
        stageInitData.CurrentValue = 0;

        StageExampleTreeProgressBar.InitializeProgress.Invoke(stageInitData);
    }

    void InstantiateNewTree()
    {
        cuttedTreesCount++;
        //Instantiate new tree
        UpdateStageProgress();
    }

    void UpdateStageProgress()
    {
        stageUpdateData.CurrentValue = cuttedTreesCount;
        StageExampleTreeProgressBar.UpdateProgress.Invoke(stageUpdateData);
    }

    void FinishGame(OnFinishProgress<StageExampleTreeProgressBar> stageCompleteData)
    {
        //Finish
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