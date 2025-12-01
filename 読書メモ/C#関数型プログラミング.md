- 検証

```cs
public static bool IsValid<T>(this T @this, params Func<T,bool>[] rules) =>
    rules.All(x => x(@this));

public static bool IsInvalid<T>(this T @this, params Func<T,bool>[] rules) =>
    !@this.IsValid(rules);
```

```cs
#//EXAMPLE
public bool IsPasswordValid(string password) =>
    password.IsValid(
        x => x.Length > 6,
        x => x.Length <= 20,
        x => x.Any(y => Char.IsLower(y)),
        x => x.Any(y => Char.IsUpper(y)),
        x => x.Any(y => Char.IsSymbol(y)),
        x => !x.Contains("Justin", StringComparison.OrdinalIgnoreCase)
            && !x.Contains("Bieber", StringComparison.OrdinalIgnoreCase)
    )
```

- パターンマッチング

```cs
public static MatchValueOrDefault<TInput, TOutput> Match<TInput, TOutput>(
  this TInput @this,
  params (Func<TInput, bool>,
  Func<TInput, TOutput>)[] predicates)
{
    var match = predicates.FirstOrDefault(x => x.Item1(@this));
    var returnValue = match?.Item2(@this);
    return new MatchValueOrDefault<TInput, TOutput>(returnValue, @this);
}

public class MatchValueOrDefault<TInput, TOutput>
{
    private readonly TOutput value;
    private readonly TInput originalValue;

    public MatchValueOrDefault(TOutput value, TInput originalValue)
    {
        this.value = value;
        this.originalValue = originalValue;
    }
}

public TOutput DefaultMatch(Func<TInput, TOutput> defaultMatch)
{
    if (EqualityComparer<TOutput>.Default.Equals(default, this.value))
    {
        return defaultMatch(this.originalValue);
    }
    else
    {
        return this.value;
    }
}
```

```cs
//EXAMPLE
var inputValue = 25000M;
var updatedValue = inputValue.MatchValueOrDefault(
    (x => x <= 12570, x => x),
    (x => x <= 50270, x => x * 0.8M),
    (x => x <= 150000, x => x * 0.6M)
).DefaultMatch(x => x * 0.55M);
```

- 辞書の LOOKUP

```cs
public static class ExtensionMethods
{
    public static Func<TKey, TValue> ToLookup<TKey,TValue>(
      this IDictionary<TKey,TValue> @this)
    {
        return x => @this.TryGetValue(x, out TValue? value) ? value : default;
    }

    public static Func<TKey, TValue> ToLookup<TKey,TValue>(
      this IDictionary<TKey,TValue> @this,
      TValue defaultVal)
    {
        return x => @this.ContainsKey(x) ? @this[x] : defaultVal;
    }
}
```

```cs
//EXAMPLE
var doctorLookup = new []
{
    ( 1, "William Hartnell" ),
    ( 2, "Patrick Troughton" ),
    ( 3, "Jon Pertwee" ),
    ( 4, "Tom Baker" )
}.ToDictionary(x => x.Item1, x => x.Item2)
    .ToLookup("An Unknown Actor");

var fifthDoctorInfo = $"The 5th Doctor was played by {doctorLookup(5)}";
```

- 値の解析

```cs
public static class ExtensionMethods
{
    public static int ToIntOrDefault(this object @this, int defaultVal = 0) =>
        int.TryParse(@this?.ToString() ?? string.Empty, out var parsedValue)
            ? parsedValue
            : defaultVal;

    public static string ToStringOrDefault(
        this object @this,
        string defaultVal = "") =>
        string.IsNullOrWhiteSpace(@this?.ToString() ?? string.Empty)
            ? defaultVal
            : @this.ToString();
}
```

```cs
//EXAMPLE
public Settings GetSettings() =>
    new Settings
    {
        NumberOfRetries = ConfigurationManager.AppSettings["NumberOfRetries"]
            .ToIntOrDefault(5),
        HourToStartPollingAt =
            ConfigurationManager.AppSettings["HourToStartPollingAt"]
            .ToIntOrDefault(0),
        AlertEmailAddress = ConfigurationManager.AppSettings["AlertEmailAddress"]
             .ToStringOrDefault("test@thecompany.net"),
        ServerName = ConfigurationManager.AppSettings["ServerName"]
            .ToStringOrDefault("TestServer"),

    };
```

- 条件付き繰り返し

```cs
public static class ExtensionMethods
{
    public static T AggregateUntil<T>(
      this T @this,
      Func<T,bool> endCondition,
      Func<T,T> update) =>
        endCondition(@this)
             ? @this
             : AggregateUntil(update(@this), endCondition, update);
}
```

```cs
//EXAMPLE
var gameState = new State
{
    IsAlive = true,
    HitPoints = 100
};

var endState = gameState.AggregateUntil(
    x => x.HitPoints <= 0,
    x => {
        var message = this.ComposeMessageToUser(x);
        var userInput = this.InteractWithUser(message);
        return this.UpdateState(x, userInput);
    });
```

- フォーク・コンビネーター

```cs
public static class ForkExtensionMethods
{
    public static TEnd Fork<TStart, TMiddle, TEnd>(
       this TStart @this,
       Func<TMiddle, TEnd> joinFunction,
       params Func<TStart, TMiddle>[] prongs
    )
{
    var intermediateValues = prongs.Select(x => x(@this));
    var returnValue = joinFunction(intermediateValues);
    return returnValue;
}
```

```cs
//EXAMPLE
var personData = this.personRepository.GetPerson(24601);
var description = personData.Fork(
    prongs => string.Join(Environment.NewLine, prongs),
    x => "My name is " + x.FirstName + " " + x.LastName,
    x => "I am " + x.Age + " years old.",
    x => "I live in " + x.Address.Town
)

// This might, for example, produce:
//
// My name is Jean Valjean
// I am 30 years old
// I live in Montreuil-sur-mer
```

- オルト・コンビネーター

```cs
public static TOut Alt<TIn, TOut>(
    this TIn @this,
    params Func<TIn, TOut>[] args) =>
    args.Select(x => x(@this))
    .First(x => x != null);
```

```cs
//EXAMPLE
public Person GetEmployee(int empId) =>
    empId.Alt(
        x => this.employeeDbRepo.GetById(x),
        x => this.ActiveDirectoryClient.GetById(x),
        x => this.EmergencyBackupCsvClient.GetById(x)
    );
```

- Compose

```cs
public static class ComposeExtensionMethods
{
    public static Func<TIn, NewTOut> Compose<TIn, OldTOut, NewTOut>(
        this Func<TIn, OldTOut> @this,
        Func<OldTOut, NewTOut> f) =>
            x => f(@this(x));
}
```

```cs
var formatDecimal = (decimal x) => x
    .Map(x => Math.Round(x, 2))
    .Map(x => $"{x} degrees");

var input = 100M;
var celsiusToFahrenheit = (decimal x) => x.Map(x => x - 32)
    .Map(x => x * 5)
    .Map(x => x / 9);
var fToCFormatted = celsiusToFahrenheit.Compose(formatDecimal);
var output = fToCFormatted(input);
Console.WriteLine(output);

var input2 = 37.78M;
var celsiusToFahrenheit =    (decimal x) =>
    x.Map(x => x * 9)
    .Map(x => x / 5)
    .Map(x => x + 32);
var cToFFormatted = celsiusToFahrenheit.Compose(formatDecimal);
var output2 = cToFFormatted(input2);
Console.WriteLine(output2);
```

\*トランスデュース

```cs
public static TFinalOut Transduce<TIn, TFilterOut, TFinalOut>(
    this IEnumerable<TIn> @this,
    Func<IEnumerable<TIn>, IEnumerable<TFilterOut>> transformer,
    Func<IEnumerable<TFilterOut>, TFinalOut> aggregator) =>
        aggregator(transformer(@this));
```

```cs
//EXAMPLE
var numbers = new [] { 4, 8, 15, 16, 23, 42 };

// N.B - I could make this a single line with brackets, but
// I find this more readable, and it's functionally identical due
// to lazy evaluation of enumerables
var transformer = (IEnumerable<int> x) => x
    .Select(y => y + 5)
    .Select(y => y * 10)
    .Where(y => y > 100);

var aggregator = (IEnumerable<int> x) => string.Join(", ", x);

var output = numbers.Transduce(transformer, aggregator);
Console.WriteLine("Output = " + output);
// Output = 130, 200, 210, 280, 470
```

```cs
public static class TransducerExtensionMethod
{
    public static Func<IEnumerable<TIn>, TO2> ToTransducer<TIn, TO1, TO2>(
        this Func<IEnumerable<TIn>,
        IEnumerable<TO1>> @this,
        Func<IEnumerable<TO1>, TO2> aggregator) =>
            x => aggregator(@this(x));
}
```

```cs
var numbers = new [] { 4, 8, 15, 16, 23, 42 };
var transformer = (IEnumerable<int> x) => x
    .Select(y => y + 5)
    .Select(y => y * 10)
    .Where(y => y > 100);

var aggregator = (IEnumerable<int> x) => string.Join(", ", x);

var transducer = transformer.ToTransducer(aggregator);
var output2 = transducer(numbers);
Console.WriteLine("Output = " + output2);
```

- タップ

```cs
public static class Extensions
{
    public static T Tap<T>(this T @this, Action<T> action)
    {
        action(@this);
        return @this;
    }
}
```

```cs
#EXAMPLE
var input = 100M;
var fahrenheitToCelsius = (decimal x) => x.Map(x => x - 32)
    .Map(x => x * 5)
    .Map(x => x / 9)
    .Tap(x => this.logger.LogInformation("the un-rounded value is " + x))
    .Map(x => Math.Round(x, 2))
    .Map(x => $"{x} degrees");
var output = fahrenheitToCelsius(input);
Console.WriteLine(output);
// 37.78 degrees
```

- Try/Catch

```cs
public class ExecutionResult<T>
{
    public T Result { get; init; }
    public Exception Error { get; init; }
}

public static class Extensions
{
    public static ExtensionResult<TOut> MapWithTryCatch<TIn,TOut>(
        this TIn @this,
        Func<TIn,TOut> f)
    {
        try
        {
         var result = f(@this);
         return new ExecutionResult<TOut>
         {
             Result = result
         };
        }
        catch(Exception e)
        {
            return new ExecutionResult<TOut>
            {
                Error = e
            };
        }
    }
}
public static T OnError<T>(
    this ExecutionResult<T> @this,
    Action<Exception> errorHandler)
    {
    if (@this.Error != null)
        errorHandler(@this.Error);
        return @this.Result;
    }
```

```cs
//EXAMPLE
public IEnumerable<Snack> GetSnackByTypeId(int typeId) =>
    typeId.MapWithTryCatch(DataStore.GetSnackByType)
        .OnError(e => this.Logger.LogError(e, "We ran out of custard creams!"));
```

- 列挙型を更新

```cs
public static class Extensions
{
    public static IEnumerable<T> ReplaceAt(this IEnumerable<T> @this,
        int loc,
        Func<T, T> replacement) =>
        @this.Select((x, i) => i == loc ? replacement(x) : x);
}
```

```cs
var sourceData = new []
{
    "Hello", "Doctor", "Yesterday", "Today", "Tomorrow", "Continue"
}

var updatedData = sourceData.ReplaceAt(1, x => x + " Who");
var finalString = string.Join(" ", updatedData);
// Hello Doctor Who Yesterday Today Tomorrow Continue
```

```cs
public static class ReplaceWhenExtensions
{
    public static IEnumerable<T> ReplaceWhen<T>(this IEnumerable<T> @this,
        Func<T, bool> shouldReplace,
        Func<T, T> replacement) =>
        @this.Select(x => shouldReplace(x) ? replacement(x) : x);
}
```

```cs
//EXAMPLE
var sourceData = this.DataStore.GetBoardGames();

var updatedData = sourceData.ReplaceWhen(
    x => x.NumberOfPlayersAllowed.Contains(1),
    x => x with { Tags = x.Tags.Append("solo") });
this.DataStore.Save(updatedData);
```

- データベース検索

```cs
public abstract class PersonLookupResult
{
    public int Id { get; set; }
}

public class PersonFound : PersonLookupResult
{
    public Person Person { get; set; }
}

public class PersonNotFound : PersonLookupResult
{

}

public class ErrorWhileSearchingPerson : PersonLookupResult
{
    public Exception Error { get; set; }
}
```

```cs
//EXAMPLE
public PersonLookupResult  GetPerson(int id)
{
    try
    {
        var personFromDb = this.Db.Person.Lookup(id);
        return personFromDb == null
            ? new PersonNotFound { Id = id }
            : new PersonFound
                {
                    Person = personFromDb,
                    Id = id
                };
    }
    catch(Exception e)
    {
        return new ErrorWhileSearchingPerson
        {
            Id = id,
            Error = e
        }
    }
}
```

- MaybeResult

```cs
public Maybe<IEnumerable<DoctorWho>> GetAllDoctors()
{
    try
    {
         using var conn = this._connectionFactory.Make();
        // Dapper query to the db
        var data = conn.Query<Doctor>(
            "SELECT * FROM [dbo].[Doctors]");
            return data == null || !data.Any()
                ? new Nothing<IEnumerable<DoctorWho>>();
                : new Something<IEnumerable<DoctorWho>>(data);
    }
    catch(Exception e)
    {
        this.logger.LogError(e, "Error getting doctor " + doctorNumber);
        return new Error<IEnumerable<DoctorWho>>(e);
    }

}
```

```cs
//EXAMPLE
// Great chaps.  All of them!
var doc = this.DoctorRepository.GetAllDoctors();
var message = doc switch
{
    Nothing<IEnumerable<DoctorWho>> _ => "No Doctors found!",
    Something<IEnumerable<DoctorWho>> s => "The Doctors were played by: " +
        string.Join(Environment.NewLine, s.Value.Select(x => x.ActorName),
    Error<IEnumerable<DoctorWho>> e => "An error occurred: " e.Error.Message
};
```

```cs
public static Maybe<TOut> Bind<TIn, TOut>(
    this Maybe<TIn> @this,
    Func<TIn, TOut> f)
{
    try
    {
        Maybe<TOut> updatedValue = @this switch
        {
            Something<TIn> s when
                !EqualityComparer<TIn>.Default.Equals(s.Value, default) =>
                    new Something<TOut>(f(s.Value)),
            Something<TIn> s when
                s.GetType().GetGenericArguments()[0].IsPrimitive =>
                    new Something<TOut>(f(s.Value)),
            Something<TIn> _ => new UnhandledNothing<TOut>(),
            Nothing<TIn> _ => new Nothing<TOut>(),
            UnhandledNothing<TIn> _ => new UnhandledNothing<TOut>(),
            Error<TIn> e => new Error<TOut>(e.ErrorMessage),
            UnhandledError<TIn> e => new UnhandledError<TOut>(e.CapturedError),
            _ => new Error<TOut>(
                new Exception("New Maybe state that isn't coded for!: " +
                    @this.GetType()))
        };
        return updatedValue;
    }
    catch (Exception e)
    {
        return new UnhandledError<TOut>(e);
    }
}

public static class MaybeLoggingExtensions
{

 public static Maybe<T> OnNothing(this Maybe<T> @this, Action a)
    {
        if(@this is UnhandledNothing<T> _)
        {
            a();
            return new Nothing<T>();
        }

        return @this;
    }

    public static Maybe<T> OnError(this Maybe<T> @this, Action<Exception> a)
    {
        if(@this is UnhandledError<T> e)
        {
            a(e.CapturedError);
            return new Error<T>(e.CapturedError);
        }

        return @this;
    }
}
```

```cs
//EXAMPLvar idMaybe  idValue.ToMaybe();
var transOne = idMaybe.Bind(x => transformationOne(x))
    .OnNothing(() => this.Logger.LogWarning("Nothing happened one");
var transTwo = transOne.Bind(x => transformationTwo(x))
    .OnNothing(() => this.Logger.LogWarning("Nothing happened two");
var returnValue = transTwo.Bind(x => transformationThree(x))
    .OnNothing(() => this.Logger.LogWarning("Nothing happened three");E

```

- State

```cs
public static class StateMonadExtensions
{

public static State<TS, TV> ToState<TS, TV>(this TS @this, TV value) =>
    new(@this, value);

public static State<TS, TV> Update<TS, TV>(
        this State<TS, TV> @this,
        Func<TS, TS> f
    ) => new(f(@this.CurrentState), @this.CurrentValue);
}
```

```cs
//EXAMPLE
public IEnumerable<Order> MakeOrderReport(string userName) =>
    this.connFactory.MakeDbConnection().ToState(userName)
        .Bind((s, x) => this.customerRepo.GetCustomer(s, x))
        .Bind((s, x) => this.orderRepo.GetCustomreOrders(s, x.Id))
```

- While

```cs
public static class FunctionalExtensions
{
    public static T IterateUntil<T>(
        this T @this,
        Func<T, T> updateFunction,
        Func<T, bool> endCondition)
    {
        var currentThis = @this;

        while (!endCondition(currentThis))
        {
            currentThis = updateFunction(currentThis);
        }

        return currentThis;
    }
}
```

```cs
//EXAMPLE
public int PlaySnakesAndLaddersTrampolining(int noPlayers, IDieRoll die)
{
    var state = new GameState
    {
        CurrentPlayer = 1,
        Players = Enumerable.Range(1, noPlayers)
            .Select(x => (x, 1))
            .Select(x => new Player
            {
                Number = x.Item1,
                Position = x.Item2
            }),
        NumberOfPlayers = noPlayers
    };

    var finalState = state.IterateUntil(x =>
    {
        var roll = die.Roll();

        var newState = state with
        {
            CurrentPlayer = roll == 6
                ? state.CurrentPlayer
                : state.CurrentPlayer == state.NumberOfPlayers
                    ? 1
                    : state.CurrentPlayer + 1,
            Players = state.Players.Select(x =>
                x.Number == state.CurrentPlayer
                    ? UpdatePlayer(x, roll)
                    : x
            ).ToArray()
        };

        return newState;
    }, x => x.Players.Any(y => y.Position >= 100));

    return finalState.Players.First(x => x.Position >= 100).Number;
}
```
