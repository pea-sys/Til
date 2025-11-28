- TAP ドキュメント
  https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap

- Rx ドキュメント(タイミングに関連することを行う場合、一般的にデータフロー・ブロックよりも優れている)
  https://introtorx.com/chapters/why-reactive-extensions-for-dotnet
  名前空間：`System.Reactive`

* TPL Dataflow
  https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library?redirectedfrom=MSDN
  名前空間：`System.Threading.Tasks.Dataflow.`

* 追加の並行コレクション
  名前空間：`System.Threading.Channels`

* 不変性コレクション
  名前空間：`System.Collections.Immutable`
* 深くネストされた例外の展開

```cs
try
{
  var multiplyBlock = new TransformBlock<int, int>(item =>
  {
    if (item == 1)
      throw new InvalidOperationException("Blech.");
    return item * 2;
  });
  var subtractBlock = new TransformBlock<int, int>(item => item - 2);
  multiplyBlock.LinkTo(subtractBlock,
      new DataflowLinkOptions { PropagateCompletion = true });

  multiplyBlock.Post(1);
  subtractBlock.Completion.Wait();
}
catch (AggregateException exception)
{
  AggregateException ex = exception.Flatten();
  Trace.WriteLine(ex.InnerException);
}
```

- Thread と BackgroundWorker は使わない（時代遅れ）

* Timeout 処理の例

```cs
async Task<string> DownloadStringWithTimeout(HttpClient client, string uri)
{
  using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
  Task<string> downloadTask = client.GetStringAsync(uri);
  Task timeoutTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);

  Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);
  if (completedTask == timeoutTask)
    return null;
  return await downloadTask;
}
```

- 非同期処理を同期で実装する

```cs
interface IMyAsyncInterface
{
  Task DoSomethingAsync();
}

class MySynchronousImplementation : IMyAsyncInterface
{
  public Task DoSomethingAsync()
  {
    try
    {
      DoSomethingSynchronously();
      return Task.CompletedTask;
    }
    catch (Exception ex)
    {
      return Task.FromException(ex);
    }
  }
}
```

- 進捗状況を報告する

```cs
async Task MyMethodAsync(IProgress<double> progress = null)
{
  bool done = false;
  double percentComplete = 0;
  while (!done)
  {
    ...
    progress?.Report(percentComplete);
  }
}

async Task CallMyMethodAsync()
{
  var progress = new Progress<double>();
  progress.ProgressChanged += (sender, args) =>
  {
    ...
  };
  await MyMethodAsync(progress);
}
```

- タスクセットの完了を待つ

```Cs
Task task1 = Task.Delay(TimeSpan.FromSeconds(1));
Task task2 = Task.Delay(TimeSpan.FromSeconds(2));
Task task3 = Task.Delay(TimeSpan.FromSeconds(1));

await Task.WhenAll(task1, task2, task3);
```

```cs
Task<int> task1 = Task.FromResult(3);
Task<int> task2 = Task.FromResult(5);
Task<int> task3 = Task.FromResult(7);

int[] results = await Task.WhenAll(task1, task2, task3);

// "results" contains { 3, 5, 7 }
```

```cs
async Task ThrowNotImplementedExceptionAsync()
{
  throw new NotImplementedException();
}

async Task ThrowInvalidOperationExceptionAsync()
{
  throw new InvalidOperationException();
}

async Task ObserveOneExceptionAsync()
{
  var task1 = ThrowNotImplementedExceptionAsync();
  var task2 = ThrowInvalidOperationExceptionAsync();

  try
  {
    await Task.WhenAll(task1, task2);
  }
  catch (Exception ex)
  {
    // "ex" is either NotImplementedException or InvalidOperationException.
    ...
  }
}

async Task ObserveAllExceptionsAsync()
{
  var task1 = ThrowNotImplementedExceptionAsync();
  var task2 = ThrowInvalidOperationExceptionAsync();

  Task allTasks = Task.WhenAll(task1, task2);
  try
  {
    await allTasks;
  }
  catch
  {
    AggregateException allExceptions = allTasks.Exception;
    ...
  }
}
```

- タスクの完了を待つ

```cs
// Returns the length of data at the first URL to respond.
async Task<int> FirstRespondingUrlAsync(HttpClient client,
    string urlA, string urlB)
{
  // Start both downloads concurrently.
  Task<byte[]> downloadTaskA = client.GetByteArrayAsync(urlA);
  Task<byte[]> downloadTaskB = client.GetByteArrayAsync(urlB);

  // Wait for either of the tasks to complete.
  Task<byte[]> completedTask =
      await Task.WhenAny(downloadTaskA, downloadTaskB);

  // Return the length of the data retrieved from that URL.
  byte[] data = await completedTask;
  return data.Length;
}
```

- ValueTask を消費する

```cs
ValueTask<int> MethodAsync();

async Task ConsumingMethodAsync()
{
  int value = await MethodAsync();
}
```

```cs
ValueTask<int> MethodAsync();

async Task ConsumingMethodAsync()
{
  Task<int> task = MethodAsync().AsTask();
  ... // other concurrent work
  int value = await task;
  int anotherValue = await task;
}
```

- 非同期ストリームの作成

```cs
async IAsyncEnumerable<string> GetValuesAsync(HttpClient client)
{
  int offset = 0;
  const int limit = 10;
  while (true)
  {
    // Get the current page of results and parse them.
    string result = await client.GetStringAsync(
        $"https://example.com/api/values?offset={offset}&limit={limit}");
    string[] valuesOnThisPage = result.Split('\n');

    // Produce the results for this page.
    foreach (string value in valuesOnThisPage)
      yield return value;

    // If this is the last page, we're done.
    if (valuesOnThisPage.Length != limit)
      break;

    // Otherwise, proceed to the next page.
    offset += limit;
  }
}
```

- 非同期ストリームのコンシューマ

```cs
IAsyncEnumerable<string> GetValuesAsync(HttpClient client);

public async Task ProcessValueAsync(HttpClient client)
{
  await foreach (string value in GetValuesAsync(client))
  {
    Console.WriteLine(value);
  }
}
```

```Cs
IAsyncEnumerable<string> GetValuesAsync(HttpClient client);

public async Task ProcessValueAsync(HttpClient client)
{
  await foreach (string value in GetValuesAsync(client).ConfigureAwait(false))
  {
    await Task.Delay(100).ConfigureAwait(false); // asynchronous work
    Console.WriteLine(value);
  }
}
```

- 非同期ストリームで LINQ を使う

```cs
IAsyncEnumerable<int> values = SlowRange().WhereAwait(
    async value =>
    {
      // Do some asynchronous work to determine
      //  if this element should be included.
      await Task.Delay(10);
      return value % 2 == 0;
    });

await foreach (int result in values)
{
  Console.WriteLine(result);
}

// Produce sequence that slows down as it progresses.
async IAsyncEnumerable<int> SlowRange()
{
  for (int i = 0; i != 10; ++i)
  {
    await Task.Delay(i * 100);
    yield return i;
  }
}
```

- 非同期ストリームとキャンセル

```cs
using var cts = new CancellationTokenSource(500);
CancellationToken token = cts.Token;
await foreach (int result in SlowRange(token))
{
  Console.WriteLine(result);
}

// Produce sequence that slows down as it progresses.
async IAsyncEnumerable<int> SlowRange(
    [EnumeratorCancellation] CancellationToken token = default)
{
  for (int i = 0; i != 10; ++i)
  {
    await Task.Delay(i * 100, token);
    yield return i;
  }
}
```

- データの並列処理

```cs
void RotateMatrices(IEnumerable<Matrix> matrices, float degrees,
    CancellationToken token)
{
  Parallel.ForEach(matrices,
      new ParallelOptions { CancellationToken = token },
      matrix => matrix.Rotate(degrees));
}
```

- パラレル・アグリゲーション

```cs
// Note: this is not the most efficient implementation.
// This is just an example of using a lock to protect shared state.
int ParallelSum(IEnumerable<int> values)
{
  object mutex = new object();
  int result = 0;
  Parallel.ForEach(source: values,
      localInit: () => 0,
      body: (item, state, localValue) => localValue + item,
      localFinally: localValue =>
      {
        lock (mutex)
          result += localValue;
      });
  return result;
}
```

```cs
int ParallelSum(IEnumerable<int> values)
{
  return values.AsParallel().Sum();
}
```

```cs
int ParallelSum(IEnumerable<int> values)
{
  return values.AsParallel().Aggregate(
      seed: 0,
      func: (sum, item) => sum + item
  );
}
```

- 並列呼び出し

```cs
void DoAction20Times(Action action, CancellationToken token)
{
  Action[] actions = Enumerable.Repeat(action, 20).ToArray();
  Parallel.Invoke(new ParallelOptions { CancellationToken = token }, actions);
}
```

- 動的並列性

```cs
Task task = Task.Factory.StartNew(
    () => Thread.Sleep(TimeSpan.FromSeconds(2)),
    CancellationToken.None,
    TaskCreationOptions.None,
    TaskScheduler.Default);
Task continuation = task.ContinueWith(
    t => Trace.WriteLine("Task is done"),
    CancellationToken.None,
    TaskContinuationOptions.None,
    TaskScheduler.Default);
// The "t" argument to the continuation is the same as "task".
```

- 並列 LINQ

```cs
IEnumerable<int> MultiplyBy2(IEnumerable<int> values)
{
  return values.AsParallel().AsOrdered().Select(value => value * 2);
}
int ParallelSum(IEnumerable<int> values)
{
  return values.AsParallel().Sum();
}
```

- ブロックのリンク

```cs
var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
var subtractBlock = new TransformBlock<int, int>(item => item - 2);

var options = new DataflowLinkOptions { PropagateCompletion = true };
multiplyBlock.LinkTo(subtractBlock, options);

...

// The first block's completion is automatically propagated to the second block.
multiplyBlock.Complete();
await subtractBlock.Completion;
```

- エラーの伝播

```cs
try
{
  var multiplyBlock = new TransformBlock<int, int>(item =>
  {
    if (item == 1)
      throw new InvalidOperationException("Blech.");
    return item * 2;
  });
  var subtractBlock = new TransformBlock<int, int>(item => item - 2);
  multiplyBlock.LinkTo(subtractBlock,
      new DataflowLinkOptions { PropagateCompletion = true });
  multiplyBlock.Post(1);
  await subtractBlock.Completion;
}
catch (AggregateException)
{
  // The exception is caught here.
}
```

- ブロックの連結を解除する

```cs
var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
var subtractBlock = new TransformBlock<int, int>(item => item - 2);

IDisposable link = multiplyBlock.LinkTo(subtractBlock);
multiplyBlock.Post(1);
multiplyBlock.Post(2);

// Unlink the blocks.
// The data posted above may or may not have already gone through the link.
// In real-world code, consider a using block rather than calling Dispose.
link.Dispose();
```

- スロットリング・ブロック

```cs
var sourceBlock = new BufferBlock<int>();
var options = new DataflowBlockOptions { BoundedCapacity = 1 };
var targetBlockA = new BufferBlock<int>(options);
var targetBlockB = new BufferBlock<int>(options);

sourceBlock.LinkTo(targetBlockA);
sourceBlock.LinkTo(targetBlockB);
```

- データフロー・ブロックによる並列処理

```cs
var multiplyBlock = new TransformBlock<int, int>(
    item => item * 2,
    new ExecutionDataflowBlockOptions
    {
      MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
    });
var subtractBlock = new TransformBlock<int, int>(item => item - 2);
multiplyBlock.LinkTo(subtractBlock);
```

- カスタムブロックの作成

```cs
IPropagatorBlock<int, int> CreateMyCustomBlock()
{
  var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
  var addBlock = new TransformBlock<int, int>(item => item + 2);
  var divideBlock = new TransformBlock<int, int>(item => item / 2);

  var flowCompletion = new DataflowLinkOptions { PropagateCompletion = true };
  multiplyBlock.LinkTo(addBlock, flowCompletion);
  addBlock.LinkTo(divideBlock, flowCompletion);

  return DataflowBlock.Encapsulate(multiplyBlock, divideBlock);
}
```

- .NET イベントを変換する

```cs
var progress = new Progress<int>();
IObservable<EventPattern<int>> progressReports =
    Observable.FromEventPattern<int>(
        handler => progress.ProgressChanged += handler,
        handler => progress.ProgressChanged -= handler);
progressReports.Subscribe(data => Trace.WriteLine("OnNext: " + data.EventArgs));

var timer = new System.Timers.Timer(interval: 1000) { Enabled = true };
IObservable<EventPattern<object>> ticks =
    Observable.FromEventPattern(timer, nameof(Timer.Elapsed));
ticks.Subscribe(data => Trace.WriteLine("OnNext: "
    + ((ElapsedEventArgs)data.EventArgs).SignalTime));
```

- 通知をコンテキストに送信する

```cs
private void Button_Click(object sender, RoutedEventArgs e)
{
  SynchronizationContext uiContext = SynchronizationContext.Current;
  Trace.WriteLine($"UI thread is {Environment.CurrentManagedThreadId}");
  Observable.Interval(TimeSpan.FromSeconds(1))
      .ObserveOn(uiContext)
      .Subscribe(x => Trace.WriteLine(
          $"Interval {x} on thread {Environment.CurrentManagedThreadId}"));
}
```

```cs
SynchronizationContext uiContext = SynchronizationContext.Current;
Trace.WriteLine($"UI thread is {Environment.CurrentManagedThreadId}");
Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
        handler => (s, a) => handler(s, a),
        handler => MouseMove += handler,
        handler => MouseMove -= handler)
    .Select(evt => evt.EventArgs.GetPosition(this))
    .ObserveOn(Scheduler.Default)
    .Select(position =>
    {
      // Complex calculation
      Thread.Sleep(100);
      var result = position.X + position.Y;
      var thread = Environment.CurrentManagedThreadId;
      Trace.WriteLine($"Calculated result {result} on thread {thread}");
      return result;
    })
    .ObserveOn(uiContext)
    .Subscribe(x => Trace.WriteLine(
        $"Result {x} on thread {Environment.CurrentManagedThreadId}"));
```

- ウィンドウとバッファでイベントデータをグループ化する

```cs
Observable.Interval(TimeSpan.FromSeconds(1))
    .Buffer(2)
    .Subscribe(x => Trace.WriteLine(
        $"{DateTime.Now.Second}: Got {x[0]} and {x[1]}"));

Observable.Interval(TimeSpan.FromSeconds(1))
    .Window(2)
    .Subscribe(group =>
    {
      Trace.WriteLine($"{DateTime.Now.Second}: Starting new group");
      group.Subscribe(
          x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x}"),
          () => Trace.WriteLine($"{DateTime.Now.Second}: Ending group"));
    });

private void Button_Click(object sender, RoutedEventArgs e)
{
  Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
          handler => (s, a) => handler(s, a),
          handler => MouseMove += handler,
          handler => MouseMove -= handler)
      .Buffer(TimeSpan.FromSeconds(1))
      .Subscribe(x => Trace.WriteLine(
          $"{DateTime.Now.Second}: Saw {x.Count} items."));
}
```

- スロットリングとサンプリングでイベントストリームを調整する

```cs
private void Button_Click(object sender, RoutedEventArgs e)
{
  Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
          handler => (s, a) => handler(s, a),
          handler => MouseMove += handler,
          handler => MouseMove -= handler)
      .Select(x => x.EventArgs.GetPosition(this))
      .Throttle(TimeSpan.FromSeconds(1))
      .Subscribe(x => Trace.WriteLine(
          $"{DateTime.Now.Second}: Saw {x.X + x.Y}"));
}

private void Button_Click(object sender, RoutedEventArgs e)
{
  Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
          handler => (s, a) => handler(s, a),
          handler => MouseMove += handler,
          handler => MouseMove -= handler)
      .Select(x => x.EventArgs.GetPosition(this))
      .Sample(TimeSpan.FromSeconds(1))
      .Subscribe(x => Trace.WriteLine(
          $"{DateTime.Now.Second}: Saw {x.X + x.Y}"));
}
```

- タイムアウト

```Cs
void GetWithTimeout(HttpClient client)
{
  client.GetStringAsync("http://www.example.com/").ToObservable()
      .Timeout(TimeSpan.FromSeconds(1))
      .Subscribe(
          x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.Length}"),
          ex => Trace.WriteLine(ex));
}

private void Button_Click(object sender, RoutedEventArgs e)
{
  IObservable<Point> clicks =
      Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
          handler => (s, a) => handler(s, a),
          handler => MouseDown += handler,
          handler => MouseDown -= handler)
      .Select(x => x.EventArgs.GetPosition(this));

  Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
          handler => (s, a) => handler(s, a),
          handler => MouseMove += handler,
          handler => MouseMove -= handler)
      .Select(x => x.EventArgs.GetPosition(this))
      .Timeout(TimeSpan.FromSeconds(1), clicks)
      .Subscribe(
          x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.X},{x.Y}"),
          ex => Trace.WriteLine(ex));
}
```

- 非同期メソッドの単体テスト

```cs
[TestMethod]
public async Task MyMethodAsync_ReturnsFalse()
{
  var objectUnderTest = ...;
  bool result = await objectUnderTest.MyMethodAsync();
  Assert.IsFalse(result);
}

[TestMethod]
public void MyMethodAsync_ReturnsFalse()
{
  AsyncContext.Run(async () =>
  {
    var objectUnderTest = ...;
    bool result = await objectUnderTest.MyMethodAsync();
    Assert.IsFalse(result);
  });
}
```

- 失敗が予想される非同期メソッドの単体テスト

```cs
[TestMethod]
[ExpectedException(typeof(DivideByZeroException))]
public async Task Divide_WhenDenominatorIsZero_ThrowsDivideByZero()
{
  await MyClass.DivideAsync(4, 0);
}
```

上記だと、どのステップで例外が発生したか特定できない。
xUnit の ThrowsAsync チックにやりたい場合は以下を定義

```Cs
/// <summary>
/// Ensures that an asynchronous delegate throws an exception.
/// </summary>
/// <typeparam name="TException">
/// The type of exception to expect.
/// </typeparam>
/// <param name="action">The asynchronous delegate to test.</param>
/// <param name="allowDerivedTypes">
/// Whether derived types should be accepted.
/// </param>
public static async Task<TException> ThrowsAsync<TException>(Func<Task> action,
    bool allowDerivedTypes = true)
    where TException : Exception
{
  try
  {
    await action();
    var name = typeof(Exception).Name;
    Assert.Fail($"Delegate did not throw expected exception {name}.");
    return null;
  }
  catch (Exception ex)
  {
    if (allowDerivedTypes && !(ex is TException))
      Assert.Fail($"Delegate threw exception of type {ex.GetType().Name}" +
          $", but {typeof(TException).Name} or a derived type was expected.");
    if (!allowDerivedTypes && ex.GetType() != typeof(TException))
      Assert.Fail($"Delegate threw exception of type {ex.GetType().Name}" +
          $", but {typeof(TException).Name} was expected.");
    return (TException)ex;
  }
}
```

- 非同期 void メソッドの単体テスト
  テストする前に`async void` メソッドを`async Task`に書き換えることを第一に検討する

Nito.AsyncExNuGet パッケージ

```cs
// Not a recommended solution; see the rest of this section.
[TestMethod]
public void MyMethodAsync_DoesNotThrow()
{
  AsyncContext.Run(() =>
  {
    var objectUnderTest = new Sut(); // ...;
    objectUnderTest.MyVoidMethodAsync();
  });
}
```

- データフローメッシュの単体テスト

```cs
[TestMethod]
public async Task MyCustomBlock_AddsOneToDataItems()
{
  var myCustomBlock = CreateMyCustomBlock();

  myCustomBlock.Post(3);
  myCustomBlock.Post(13);
  myCustomBlock.Complete();

  Assert.AreEqual(4, myCustomBlock.Receive());
  Assert.AreEqual(14, myCustomBlock.Receive());
  await myCustomBlock.Completion;
}
```

例外

```cs
[TestMethod]
public async Task MyCustomBlock_Fault_DiscardsDataAndFaults()
{
  var myCustomBlock = CreateMyCustomBlock();

  myCustomBlock.Post(3);
  myCustomBlock.Post(13);
  (myCustomBlock as IDataflowBlock).Fault(new InvalidOperationException());

  try
  {
    await myCustomBlock.Completion;
  }
  catch (AggregateException ex)
  {
    AssertExceptionIs<InvalidOperationException>(
        ex.Flatten().InnerException, false);
  }
}

public static void AssertExceptionIs<TException>(Exception ex,
    bool allowDerivedTypes = true)
{
  if (allowDerivedTypes && !(ex is TException))
    Assert.Fail($"Exception is of type {ex.GetType().Name}, but " +
        $"{typeof(TException).Name} or a derived type was expected.");
  if (!allowDerivedTypes && ex.GetType() != typeof(TException))
    Assert.Fail($"Exception is of type {ex.GetType().Name}, but " +
        $"{typeof(TException).Name} was expected.");
}
```

- System.Reactive 可観測性の単体テスト

```cs
public interface IHttpService
{
  IObservable<string> GetString(string url);
}

public class MyTimeoutClass
{
  private readonly IHttpService _httpService;

  public MyTimeoutClass(IHttpService httpService)
  {
    _httpService = httpService;
  }

  public IObservable<string> GetStringWithTimeout(string url)
  {
    return _httpService.GetString(url)
        .Timeout(TimeSpan.FromSeconds(1));
  }
}

class SuccessHttpServiceStub : IHttpService
{
  public IObservable<string> GetString(string url)
  {
    return Observable.Return("stub");
  }
}

[TestMethod]
public async Task MyTimeoutClass_SuccessfulGet_ReturnsResult()
{
  var stub = new SuccessHttpServiceStub();
  var my = new MyTimeoutClass(stub);

  var result = await my.GetStringWithTimeout("http://www.example.com/")
      .SingleAsync();

  Assert.AreEqual("stub", result);
}

private class FailureHttpServiceStub : IHttpService
{
  public IObservable<string> GetString(string url)
  {
    return Observable.Throw<string>(new HttpRequestException());
  }
}

[TestMethod]
public async Task MyTimeoutClass_FailedGet_PropagatesFailure()
{
  var stub = new FailureHttpServiceStub();
  var my = new MyTimeoutClass(stub);

  await ThrowsAsync<HttpRequestException>(async () =>
  {
    await my.GetStringWithTimeout("http://www.example.com/")
        .SingleAsync();
  });
}
```

- スケジューリングを偽装した System.Reactive 可観測性の単体テスト

```cs
public interface IHttpService
{
  IObservable<string> GetString(string url);
}

public class MyTimeoutClass
{
  private readonly IHttpService _httpService;

  public MyTimeoutClass(IHttpService httpService)
  {
    _httpService = httpService;
  }

  public IObservable<string> GetStringWithTimeout(string url,
      IScheduler scheduler = null)
  {
    return _httpService.GetString(url)
        .Timeout(TimeSpan.FromSeconds(1), scheduler ?? Scheduler.Default);
  }
}

private class SuccessHttpServiceStub : IHttpService
{
  public IScheduler Scheduler { get; set; }
  public TimeSpan Delay { get; set; }

  public IObservable<string> GetString(string url)
  {
    return Observable.Return("stub")
        .Delay(Delay, Scheduler);
  }
}

[TestMethod]
public void MyTimeoutClass_SuccessfulGetShortDelay_ReturnsResult()
{
  var scheduler = new TestScheduler();
  var stub = new SuccessHttpServiceStub
  {
    Scheduler = scheduler,
    Delay = TimeSpan.FromSeconds(0.5),
  };
  var my = new MyTimeoutClass(stub);
  string result = null;

  my.GetStringWithTimeout("http://www.example.com/", scheduler)
      .Subscribe(r => { result = r; });

  scheduler.Start();

  Assert.AreEqual("stub", result);
}

[TestMethod]
public void MyTimeoutClass_SuccessfulGetLongDelay_ThrowsTimeoutException()
{
  var scheduler = new TestScheduler();
  var stub = new SuccessHttpServiceStub
  {
    Scheduler = scheduler,
    Delay = TimeSpan.FromSeconds(1.5),
  };
  var my = new MyTimeoutClass(stub);
  Exception result = null;

  my.GetStringWithTimeout("http://www.example.com/", scheduler)
      .Subscribe(_ => Assert.Fail("Received value"), ex => { result = ex; });

  scheduler.Start();

  Assert.IsInstanceOfType(result, typeof(TimeoutException));
}
```

- "Completed "イベントを持つ "非同期 "メソッドの非同期ラッパー
  旧非同期コードとのインタフェース

```cs
public static Task<string> DownloadStringTaskAsync(this WebClient client,
    Uri address)
{
  var tcs = new TaskCompletionSource<string>();

  // The event handler will complete the task and unregister itself.
  DownloadStringCompletedEventHandler handler = null;
  handler = (_, e) =>
  {
    client.DownloadStringCompleted -= handler;
    if (e.Cancelled)
      tcs.TrySetCanceled();
    else if (e.Error != null)
      tcs.TrySetException(e.Error);
    else
      tcs.TrySetResult(e.Result);
  };

  // Register for the event and *then* start the operation.
  client.DownloadStringCompleted += handler;
  client.DownloadStringAsync(address);

  return tcs.Task;
}
```

- "Begin/End "メソッドの非同期ラッパー

```cs
public static Task<WebResponse> GetResponseAsync(this WebRequest client)
{
  return Task<WebResponse>.Factory.FromAsync(client.BeginGetResponse,
      client.EndGetResponse, null);
}
```

- 何でも使える非同期ラッパー

```cs
public interface IMyAsyncHttpService
{
  void DownloadString(Uri address, Action<string, Exception> callback);
}

public static Task<string> DownloadStringAsync(
    this IMyAsyncHttpService httpService, Uri address)
{
  var tcs = new TaskCompletionSource<string>();
  httpService.DownloadString(address, (result, exception) =>
  {
    if (exception != null)
      tcs.TrySetException(exception);
    else
      tcs.TrySetResult(result);
  });
  return tcs.Task;
}
```

- 並列コードの非同期ラッパー
  UI コードで UI スレッドを使いたくない場合

```cs
await Task.Run(() => Parallel.ForEach(...));
```

- System.Reactive observable の非同期ラッパー

```cs
IObservable<int> observable = ...;
int lastElement = await observable.LastAsync();
// or:  int lastElement = await observable;
```

- 非同期ストリームとデータフローメッシュ

```cs
public static class DataflowExtensions
{
  public static bool TryReceiveItem<T>(this ISourceBlock<T> block, out T value)
  {
    if (block is IReceivableSourceBlock<T> receivableSourceBlock)
      return receivableSourceBlock.TryReceive(out value);

    try
    {
      value = block.Receive(TimeSpan.Zero);
      return true;
    }
    catch (TimeoutException)
    {
      // There is no item available right now.
      value = default;
      return false;
    }
    catch (InvalidOperationException)
    {
      // The block is complete and there are no more items.
      value = default;
      return false;
    }
  }

  public static async IAsyncEnumerable<T> ReceiveAllAsync<T>(
      this ISourceBlock<T> block,
      [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    while (await block
        .OutputAvailableAsync(cancellationToken).ConfigureAwait(false))
    {
      while (block.TryReceiveItem(out var value))
      {
        yield return value;
      }
    }
  }
}
```

```cs
var multiplyBlock = new TransformBlock<int, int>(value => value * 2);

multiplyBlock.Post(5);
multiplyBlock.Post(2);
multiplyBlock.Complete();

await foreach (int item in multiplyBlock.ReceiveAllAsync())
{
  Console.WriteLine(item);
}
```

- System.Reactive オブザーバブルとデータフローメッシュ

```cs
IObservable<DateTimeOffset> ticks =
    Observable.Interval(TimeSpan.FromSeconds(1))
        .Timestamp()
        .Select(x => x.Timestamp)
        .Take(5);

var display = new ActionBlock<DateTimeOffset>(x => Trace.WriteLine(x));
ticks.Subscribe(display.AsObserver());

try
{
  display.Completion.Wait();
  Trace.WriteLine("Done.");
}
catch (Exception ex)
{
  Trace.WriteLine(ex);
}
```

- System.Reactive オブザーバブルを非同期ストリームに変換する

```cs
IObservable<long> observable =
    Observable.Interval(TimeSpan.FromSeconds(1));

// WARNING: May consume unbounded memory; see discussion!
IAsyncEnumerable<long> enumerable =
    observable.ToAsyncEnumerable();
```

境界付きチャネル

```cs
// WARNING: May discard items; see discussion!
public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
    this IObservable<T> observable, int bufferSize)
{
  var bufferOptions = new BoundedChannelOptions(bufferSize)
  {
    FullMode = BoundedChannelFullMode.DropOldest,
  };
  Channel<T> buffer = Channel.CreateBounded<T>(bufferOptions);
  using (observable.Subscribe(
      value => buffer.Writer.TryWrite(value),
      error => buffer.Writer.Complete(error),
      () => buffer.Writer.Complete()))
  {
    await foreach (T item in buffer.Reader.ReadAllAsync())
      yield return item;
  }
}
```

| 特徴                             | Channel | BlockingCollection ＜ T ＞ | BufferBlock ＜ T ＞ | AsyncProducer-ConsumerQueue ＜ T ＞ | AsyncCollection ＜ T ＞ |
| -------------------------------- | ------- | -------------------------- | ------------------- | ----------------------------------- | ----------------------- |
| キュー・セマンティクス           | ✓       | ✓                          | ✓                   | ✓                                   | ✓                       |
| スタック／バッグのセマンティクス | ✗       | ✓                          | ✗                   | ✗                                   | ✓                       |
| 同期 API                         | ✓       | ✓                          | ✓                   | ✓                                   | ✓                       |
| 非同期 API                       | ✓       | ✗                          | ✓                   | ✓                                   | ✓                       |
| 満杯になったらアイテムを落とす   | ✓       | ✗                          | ✗                   | ✗                                   | ✗                       |
| マイクロソフトによるテスト済み   | ✓       | ✓                          | ✓                   | ✗                                   | ✗                       |

- 不変性スタックとキュー

```cs
ImmutableStack<int> stack = ImmutableStack<int>.Empty;
stack = stack.Push(13);
ImmutableStack<int> biggerStack = stack.Push(7);

// Displays "7" followed by "13".
foreach (int item in biggerStack)
  Trace.WriteLine(item);

// Only displays "13".
foreach (int item in stack)
  Trace.WriteLine(item);
```

```cs
ImmutableQueue<int> queue = ImmutableQueue<int>.Empty;
queue = queue.Enqueue(13);
queue = queue.Enqueue(7);

// Displays "13" followed by "7".
foreach (int item in queue)
  Trace.WriteLine(item);

int nextItem;
queue = queue.Dequeue(out nextItem);
// Displays "13".
Trace.WriteLine(nextItem);
```

- 不変性リスト

```cs
ImmutableList<int> list = ImmutableList<int>.Empty;
list = list.Insert(0, 13);
list = list.Insert(0, 7);

// Displays "7" followed by "13".
foreach (int item in list)
  Trace.WriteLine(item);

list = list.RemoveAt(1);

// The best way to iterate over an ImmutableList<T>.
foreach (var item in list)
  Trace.WriteLine(item);

// This will also work, but it will be much slower.
for (int i = 0; i != list.Count; ++i)
  Trace.WriteLine(list[i]);
```

- 不変性セット

```cs
ImmutableSortedSet<int> sortedSet = ImmutableSortedSet<int>.Empty;
sortedSet = sortedSet.Add(13);
sortedSet = sortedSet.Add(7);

// Displays "7" followed by "13".
foreach (int item in sortedSet)
  Trace.WriteLine(item);
int smallestItem = sortedSet[0];
// smallestItem == 7

sortedSet = sortedSet.Remove(7);
```

- 不変性辞書

```cs
ImmutableDictionary<int, string> dictionary =
    ImmutableDictionary<int, string>.Empty;
dictionary = dictionary.Add(10, "Ten");
dictionary = dictionary.Add(21, "Twenty-One");
dictionary = dictionary.SetItem(10, "Diez");

// Displays "10Diez" and "21Twenty-One" in an unpredictable order.
foreach (KeyValuePair<int, string> item in dictionary)
  Trace.WriteLine(item.Key + item.Value);

string ten = dictionary[10];
// ten == "Diez"

dictionary = dictionary.Remove(21);
```

- スレッドセーフ辞書

```cs
var dictionary = new ConcurrentDictionary<int, string>();
string newValue = dictionary.AddOrUpdate(0,
    key => "Zero",
    (key, oldValue) => "Zero");
```

- キューをブロックする

```cs
private readonly BlockingCollection<int> _blockingQueue =
    new BlockingCollection<int>();
```

- 非同期キュー

```cs
Channel<int> queue = Channel.CreateUnbounded<int>();

// Producer code
ChannelWriter<int> writer = queue.Writer;
await writer.WriteAsync(7);
await writer.WriteAsync(13);
writer.Complete();

// Consumer code
// Displays "7" followed by "13".
ChannelReader<int> reader = queue.Reader;
await foreach (int value in reader.ReadAllAsync())
  Trace.WriteLine(value);
```

```cs
var _asyncQueue = new BufferBlock<int>();

// Producer code
await _asyncQueue.SendAsync(7);
await _asyncQueue.SendAsync(13);
_asyncQueue.Complete();

// Consumer code
// Displays "7" followed by "13".
while (await _asyncQueue.OutputAvailableAsync())
  Trace.WriteLine(await _asyncQueue.ReceiveAsync());
```

```cs
while (true)
{
  int item;
  try
  {
    item = await _asyncQueue.ReceiveAsync();
  }
  catch (InvalidOperationException)
  {
    break;
  }
  Trace.WriteLine(item);
}
```

- 非同期キュー

```cs
Channel<int> queue = Channel.CreateUnbounded<int>();

// Producer code
ChannelWriter<int> writer = queue.Writer;
await writer.WriteAsync(7);
await writer.WriteAsync(13);
writer.Complete();

// Consumer code
// Displays "7" followed by "13".
ChannelReader<int> reader = queue.Reader;
await foreach (int value in reader.ReadAllAsync())
  Trace.WriteLine(value);
```

```cs
var _asyncQueue = new BufferBlock<int>();

// Producer code
await _asyncQueue.SendAsync(7);
await _asyncQueue.SendAsync(13);
_asyncQueue.Complete();

// Consumer code
// Displays "7" followed by "13".
while (await _asyncQueue.OutputAvailableAsync())
  Trace.WriteLine(await _asyncQueue.ReceiveAsync());
```

- キューのスロットル

```cs
Channel<int> queue = Channel.CreateBounded<int>(1);
ChannelWriter<int> writer = queue.Writer;

// This Write completes immediately.
await writer.WriteAsync(7);

// This Write (asynchronously) waits for the 7 to be removed
// before it enqueues the 13.
await writer.WriteAsync(13);

writer.Complete();
```

```cs
var queue = new BufferBlock<int>(
    new DataflowBlockOptions { BoundedCapacity = 1 });

// This Send completes immediately.
await queue.SendAsync(7);

// This Send (asynchronously) waits for the 7 to be removed
// before it enqueues the 13.
await queue.SendAsync(13);

queue.Complete();
```

```cs
var queue = new BufferBlock<int>(
    new DataflowBlockOptions { BoundedCapacity = 1 });

// This Send completes immediately.
await queue.SendAsync(7);

// This Send (asynchronously) waits for the 7 to be removed
// before it enqueues the 13.
await queue.SendAsync(13);

queue.Complete();
```

```cs
var queue = new AsyncProducerConsumerQueue<int>(maxCount: 1);

// This Enqueue completes immediately.
await queue.EnqueueAsync(7);

// This Enqueue (asynchronously) waits for the 7 to be removed
// before it enqueues the 13.
await queue.EnqueueAsync(13);

queue.CompleteAdding();
```

```cs
var queue = new BlockingCollection<int>(boundedCapacity: 1);

// This Add completes immediately.
queue.Add(7);

// This Add waits for the 7 to be removed before it adds the 13.
queue.Add(13);

queue.CompleteAdding();
```

- サンプリングキュー

```cs
Channel<int> queue = Channel.CreateBounded<int>(
    new BoundedChannelOptions(1)
    {
      FullMode = BoundedChannelFullMode.DropOldest,
    });
ChannelWriter<int> writer = queue.Writer;

// This Write completes immediately.
await writer.WriteAsync(7);

// This Write also completes immediately.
// The 7 is discarded unless a consumer has already retrieved it.
await writer.WriteAsync(13);
```

```cs
Channel<int> queue = Channel.CreateBounded<int>(
    new BoundedChannelOptions(1)
    {
      FullMode = BoundedChannelFullMode.DropWrite,
    });
ChannelWriter<int> writer = queue.Writer;

// This Write completes immediately.
await writer.WriteAsync(7);

// This Write also completes immediately.
// The 13 is discarded unless a consumer has already retrieved the 7.
await writer.WriteAsync(13);
```

- 非同期スタックとバッグ

```cs
var _asyncStack = new AsyncCollection<int>(
    new ConcurrentStack<int>());
var _asyncBag = new AsyncCollection<int>(
    new ConcurrentBag<int>());
```

- ブロック／非同期キュー

```cs
var queue = new BufferBlock<int>();

// Producer code
await queue.SendAsync(7);
await queue.SendAsync(13);
queue.Complete();

// Consumer code for a single consumer
while (await queue.OutputAvailableAsync())
  Trace.WriteLine(await queue.ReceiveAsync());

// Consumer code for multiple consumers
while (true)
{
  int item;
  try
  {
    item = await queue.ReceiveAsync();
  }
  catch (InvalidOperationException)
  {
    break;
  }

  Trace.WriteLine(item);
}
```

```cs
ActionBlock<int> queue = new ActionBlock<int>(item => Trace.WriteLine(item));

// Asynchronous producer code
await queue.SendAsync(7);
await queue.SendAsync(13);

// Synchronous producer code
queue.Post(7);
queue.Post(13);
queue.Complete();
```

- キャンセル要請の発行

```cs
private async void StartButton_Click(object sender, RoutedEventArgs e)
{
  StartButton.IsEnabled = false;
  CancelButton.IsEnabled = true;
  try
  {
    _cts = new CancellationTokenSource();
    CancellationToken token = _cts.Token;
    await Task.Delay(TimeSpan.FromSeconds(5), token);
    MessageBox.Show("Delay completed successfully.");
  }
  catch (OperationCanceledException)
  {
    MessageBox.Show("Delay was canceled.");
  }
  catch (Exception)
  {
    MessageBox.Show("Delay completed with error.");
    throw;
  }
  finally
  {
    StartButton.IsEnabled = true;
    CancelButton.IsEnabled = false;
  }
}

private void CancelButton_Click(object sender, RoutedEventArgs e)
{
  _cts.Cancel();
  CancelButton.IsEnabled = false;
}
```

- ポーリングによるキャンセル要請への対応

```cs
public int CancelableMethod(CancellationToken cancellationToken)
{
  for (int i = 0; i != 100000; ++i)
  {
    Thread.Sleep(1); // Some calculation goes here.
    if (i % 1000 == 0)
      cancellationToken.ThrowIfCancellationRequested();
  }
  return 42;
}
```

- タイムアウトによるキャンセル

```cs
async Task IssueTimeoutAsync()
{
  using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
  CancellationToken token = cts.Token;
  await Task.Delay(TimeSpan.FromSeconds(10), token);
}
```

- 非同期コードをキャンセルする

```cs
public async Task<int> CancelableMethodAsync(CancellationToken cancellationToken)
{
  await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
  return 42;
}
```

- パラレルコードをキャンセルする

```Cs
void RotateMatrices(IEnumerable<Matrix> matrices, float degrees,
    CancellationToken token)
{
  Parallel.ForEach(matrices,
      new ParallelOptions { CancellationToken = token },
      matrix => matrix.Rotate(degrees));
}
IEnumerable<int> MultiplyBy2(IEnumerable<int> values,
    CancellationToken cancellationToken)
{
  return values.AsParallel()
      .WithCancellation(cancellationToken)
      .Select(item => item * 2);
}
```

- System.Reactive コードをキャンセルする

```cs
private IDisposable _mouseMovesSubscription;

private void StartButton_Click(object sender, RoutedEventArgs e)
{
  IObservable<Point> mouseMoves = Observable
      .FromEventPattern<MouseEventHandler, MouseEventArgs>(
          handler => (s, a) => handler(s, a),
          handler => MouseMove += handler,
          handler => MouseMove -= handler)
      .Select(x => x.EventArgs.GetPosition(this));
  _mouseMovesSubscription = mouseMoves.Subscribe(value =>
  {
    MousePositionLabel.Content = "(" + value.X + ", " + value.Y + ")";
  });
}

private void CancelButton_Click(object sender, RoutedEventArgs e)
{
  if (_mouseMovesSubscription != null)
    _mouseMovesSubscription.Dispose();
}
```

```cs
CancellationToken cancellationToken = ...
IObservable<int> observable = ...
int lastElement = await observable.TakeLast(1).ToTask(cancellationToken);
// or: int lastElement = await observable.ToTask(cancellationToken);
```

- データフローメッシュをキャンセルする

```cs
IPropagatorBlock<int, int> CreateMyCustomBlock(
    CancellationToken cancellationToken)
{
  var blockOptions = new ExecutionDataflowBlockOptions
  {
    CancellationToken = cancellationToken
  };
  var multiplyBlock = new TransformBlock<int, int>(item => item * 2,
      blockOptions);
  var addBlock = new TransformBlock<int, int>(item => item + 2,
      blockOptions);
  var divideBlock = new TransformBlock<int, int>(item => item / 2,
      blockOptions);

  var flowCompletion = new DataflowLinkOptions
  {
    PropagateCompletion = true
  };
  multiplyBlock.LinkTo(addBlock, flowCompletion);
  addBlock.LinkTo(divideBlock, flowCompletion);

  return DataflowBlock.Encapsulate(multiplyBlock, divideBlock);
}
```

- キャンセル要求の注入

```cs
async Task<HttpResponseMessage> GetWithTimeoutAsync(HttpClient client,
    string url, CancellationToken cancellationToken)
{
  using CancellationTokenSource cts = CancellationTokenSource
      .CreateLinkedTokenSource(cancellationToken);
  cts.CancelAfter(TimeSpan.FromSeconds(2));
  CancellationToken combinedToken = cts.Token;

  return await client.GetAsync(url, combinedToken);
}
```

- 他のキャンセル・システムとの相互運用性

```cs
async Task<PingReply> PingAsync(string hostNameOrAddress,
    CancellationToken cancellationToken)
{
  using var ping = new Ping();
  Task<PingReply> task = ping.SendPingAsync(hostNameOrAddress);
  using CancellationTokenRegistration _ = cancellationToken
      .Register(() => ping.SendAsyncCancel());
  return await task;
}
```

- 非同期インタフェースと継承

```cs
interface IMyAsyncInterface
{
  Task<int> CountBytesAsync(HttpClient client, string url);
}

class MyAsyncClass : IMyAsyncInterface
{
  public async Task<int> CountBytesAsync(HttpClient client, string url)
  {
    var bytes = await client.GetByteArrayAsync(url);
    return bytes.Length;
  }
}

async Task UseMyInterfaceAsync(HttpClient client, IMyAsyncInterface service)
{
  var result = await service.CountBytesAsync(client, "http://www.example.com");
  Trace.WriteLine(result);
}
```

- 非同期コンストラクション：ファクトリー

```cs
class MyAsyncClass
{
  private MyAsyncClass()
  {
  }

  private async Task<MyAsyncClass> InitializeAsync()
  {
    await Task.Delay(TimeSpan.FromSeconds(1));
    return this;
  }

  public static Task<MyAsyncClass> CreateAsync()
  {
    var result = new MyAsyncClass();
    return result.InitializeAsync();
  }
}
```

- 非同期の構築：非同期初期化パターン

```cs
/// <summary>
/// Marks a type as requiring asynchronous initialization
/// and provides the result of that initialization.
/// </summary>
public interface IAsyncInitialization
{
  /// <summary>
  /// The result of the asynchronous initialization of this instance.
  /// </summary>
  Task Initialization { get; }
}

class MyFundamentalType : IMyFundamentalType, IAsyncInitialization
{
  public MyFundamentalType()
  {
    Initialization = InitializeAsync();
  }

  public Task Initialization { get; private set; }

  private async Task InitializeAsync()
  {
    // Asynchronously initialize this instance.
    await Task.Delay(TimeSpan.FromSeconds(1));
  }
}
public static class AsyncInitialization
{
  public static Task WhenAllInitializedAsync(params object[] instances)
  {
    return Task.WhenAll(instances
        .OfType<IAsyncInitialization>()
        .Select(x => x.Initialization));
  }
}
private async Task InitializeAsync()
{
 // Asynchronously wait for all 3 instances to initialize, if necessary.
 await AsyncInitialization.WhenAllInitializedAsync(_fundamental,
     _anotherType, _yetAnother);

 // Do our own initialization (synchronous or asynchronous).
 ...
}
```

- 非同期イベント

```cs
public class MyEventArgs : EventArgs, IDeferralSource
{
  private readonly DeferralManager _deferrals = new DeferralManager();

  ... // Your own constructors and properties

  public IDisposable GetDeferral()
  {
    return _deferrals.DeferralSource.GetDeferral();
  }

  internal Task WaitForDeferralsAsync()
  {
    return _deferrals.WaitForDeferralsAsync();
  }
}

public event EventHandler<MyEventArgs> MyEvent;

private async Task RaiseMyEventAsync()
{
  EventHandler<MyEventArgs> handler = MyEvent;
  if (handler == null)
    return;

  var args = new MyEventArgs(...);
  handler(this, args);
  await args.WaitForDeferralsAsync();
}
```

- 非同期廃棄

```cs
class MyClass : IAsyncDisposable
{
  public async ValueTask DisposeAsync()
  {
    await Task.Delay(TimeSpan.FromSeconds(2));
  }
}

await using (var myClass = new MyClass())
{
  ...
} // DisposeAsync is invoked (and awaited) here.

var myClass = new MyClass();
await using (myClass.ConfigureAwait(false))
{
  ...
} // DisposeAsync is invoked (and awaited) here with ConfigureAwait(false).
```

- ブロック・ロック

```cs
class MyClass
{
  // This lock protects the _value field.
  private readonly object _mutex = new object();

  private int _value;

  public void Increment()
  {
    lock (_mutex)
    {
      _value = _value + 1;
    }
  }
}
```

- 非同期ロック

```cs
class MyClass
{
  // This lock protects the _value field.
  private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);

  private int _value;

  public async Task DelayAndIncrementAsync()
  {
    await _mutex.WaitAsync();
    try
    {
      int oldValue = _value;
      await Task.Delay(TimeSpan.FromSeconds(oldValue));
      _value = oldValue + 1;
    }
    finally
    {
      _mutex.Release();
    }
  }
}
```

- シグナルをブロックする

```cs
class MyClass
{
  private readonly ManualResetEventSlim _initialized =
      new ManualResetEventSlim();

  private int _value;

  public int WaitForInitialization()
  {
    _initialized.Wait();
    return _value;
  }

  public void InitializeFromAnotherThread()
  {
    _value = 13;
    _initialized.Set();
  }
}
```

- 非同期シグナル

```cs
class MyClass
{
  private readonly TaskCompletionSource<object> _initialized =
      new TaskCompletionSource<object>();

  private int _value1;
  private int _value2;

  public async Task<int> WaitForInitializationAsync()
  {
    await _initialized.Task;
    return _value1 + _value2;
  }

  public void Initialize()
  {
    _value1 = 13;
    _value2 = 17;
    _initialized.TrySetResult(null);
  }
}
```

- スロットリング

```cs
async Task<string[]> DownloadUrlsAsync(HttpClient client,
    IEnumerable<string> urls)
{
  using var semaphore = new SemaphoreSlim(10);
  Task<string>[] tasks = urls.Select(async url =>
  {
    await semaphore.WaitAsync();
    try
    {
      return await client.GetStringAsync(url);
    }
    finally
    {
      semaphore.Release();
    }
  }).ToArray();
  return await Task.WhenAll(tasks);
}
```

- スレッドプールへの仕事のスケジューリング

```cs
Task<int> task = Task.Run(async () =>
{
  await Task.Delay(TimeSpan.FromSeconds(2));
  return 13;
});
```

- タスク・スケジューラでコードを実行する

```cs
TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
```

```cs
var schedulerPair = new ConcurrentExclusiveSchedulerPair();
TaskScheduler concurrent = schedulerPair.ConcurrentScheduler;
TaskScheduler exclusive = schedulerPair.ExclusiveScheduler;
```

```cs
var schedulerPair = new ConcurrentExclusiveSchedulerPair(
    TaskScheduler.Default, maxConcurrencyLevel: 8);
TaskScheduler scheduler = schedulerPair.ConcurrentScheduler;
```

- 並列コードのスケジューリング

```cs
void RotateMatrices(IEnumerable<IEnumerable<Matrix>> collections, float degrees)
{
  var schedulerPair = new ConcurrentExclusiveSchedulerPair(
      TaskScheduler.Default, maxConcurrencyLevel: 8);
  TaskScheduler scheduler = schedulerPair.ConcurrentScheduler;
  ParallelOptions options = new ParallelOptions { TaskScheduler = scheduler };
  Parallel.ForEach(collections, options,
      matrices => Parallel.ForEach(matrices, options,
          matrix => matrix.Rotate(degrees)));
}
```

- スケジューラを使ったデータフローの同期化

```cs
var options = new ExecutionDataflowBlockOptions
{
  TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext(),
};
var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
var displayBlock = new ActionBlock<int>(
    result => ListBox.Items.Add(result), options);
multiplyBlock.LinkTo(displayBlock);
```

- 共有リソースの初期化

```cs
public sealed class AsyncLazy<T>
{
  private readonly object _mutex;
  private readonly Func<Task<T>> _factory;
  private Lazy<Task<T>> _instance;

  public AsyncLazy(Func<Task<T>> factory)
  {
    _mutex = new object();
    _factory = RetryOnFailure(factory);
    _instance = new Lazy<Task<T>>(_factory);
  }

  private Func<Task<T>> RetryOnFailure(Func<Task<T>> factory)
  {
    return async () =>
    {
      try
      {
        return await factory().ConfigureAwait(false);
      }
      catch
      {
        lock (_mutex)
        {
          _instance = new Lazy<Task<T>>(_factory);
        }
        throw;
      }
    };
  }

  public Task<T> Task
  {
    get
    {
      lock (_mutex)
        return _instance.Value;
    }
  }
}

static int _simpleValue;
static readonly AsyncLazy<int> MySharedAsyncInteger =
  new AsyncLazy<int>(() => Task.Run(async () =>
  {
    await Task.Delay(TimeSpan.FromSeconds(2));
    return _simpleValue++;
  }));

async Task GetSharedIntegerAsync()
{
  int sharedValue = await MySharedAsyncInteger.Task;
}
}
```

- System.Reactive 遅延評価

```cs
void SubscribeWithDefer()
{
  var invokeServerObservable = Observable.Defer(
      () => GetValueAsync().ToObservable());
  invokeServerObservable.Subscribe(_ => { });
  invokeServerObservable.Subscribe(_ => { });

  Console.ReadKey();
}

async Task<int> GetValueAsync()
{
  Console.WriteLine("Calling server...");
  await Task.Delay(TimeSpan.FromSeconds(2));
  Console.WriteLine("Returning result...");
  return 13;
}
```

- 非同期データバインディング

```cs
class BindableTask<T> : INotifyPropertyChanged
{
  private readonly Task<T> _task;

  public BindableTask(Task<T> task)
  {
    _task = task;
    var _ = WatchTaskAsync();
  }

  private async Task WatchTaskAsync()
  {
    try
    {
      await _task;
    }
    catch
    {
    }

    OnPropertyChanged("IsNotCompleted");
    OnPropertyChanged("IsSuccessfullyCompleted");
    OnPropertyChanged("IsFaulted");
    OnPropertyChanged("Result");
  }

  public bool IsNotCompleted { get { return !_task.IsCompleted; } }
  public bool IsSuccessfullyCompleted
  {
    get { return _task.Status == TaskStatus.RanToCompletion; }
  }
  public bool IsFaulted { get { return _task.IsFaulted; } }
  public T Result
  {
    get { return IsSuccessfullyCompleted ? _task.Result : default; }
  }

  public event PropertyChangedEventHandler PropertyChanged;

  protected virtual void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}
```

- 暗黙の状態

```cs
internal sealed class AsyncLocalGuidStack
{
  private readonly AsyncLocal<ImmutableStack<Guid>> _operationIds =
      new AsyncLocal<ImmutableStack<Guid>>();

  private ImmutableStack<Guid> Current =>
      _operationIds.Value ?? ImmutableStack<Guid>.Empty;

  public IDisposable Push(Guid value)
  {
    _operationIds.Value = Current.Push(value);
    return new PopWhenDisposed(this);
  }

  private void Pop()
  {
    ImmutableStack<Guid> newValue = Current.Pop();
    if (newValue.IsEmpty)
      newValue = null;
    _operationIds.Value = newValue;
  }

  public IEnumerable<Guid> Values => Current;

  private sealed class PopWhenDisposed : IDisposable
  {
    private AsyncLocalGuidStack _stack;

    public PopWhenDisposed(AsyncLocalGuidStack stack) =>
        _stack = stack;

    public void Dispose()
    {
      _stack?.Pop();
      _stack = null;
    }
  }
}

private static AsyncLocalGuidStack _operationIds = new AsyncLocalGuidStack();

async Task DoLongOperationAsync()
{
  using (_operationIds.Push(Guid.NewGuid()))
    await DoSomeStepOfOperationAsync();
}

async Task DoSomeStepOfOperationAsync()
{
  await Task.Delay(100); // some async work

  // Do some logging here.
  Trace.WriteLine("In operation: " +
      string.Join(":", _operationIds.Values));
}
```

- スロットリングの進行状況を更新する

```cs
public static class ObservableProgress
{
  // Note: this must be called from the UI thread.
  public static (IObservable<T>, IProgress<T>) CreateForUi<T>(
      TimeSpan? sampleInterval = null)
  {
    var (observable, progress) = Create<T>();
    observable = observable
        .Sample(sampleInterval ?? TimeSpan.FromMilliseconds(100))
        .ObserveOn(SynchronizationContext.Current);
    return (observable, progress);
  }
}
// For simplicity, this code updates a label directly.
// In a real-world MVVM application, those assignments
//  would instead be updating a ViewModel property
//  which is data-bound to the actual UI.
private async void StartButton_Click(object sender, RoutedEventArgs e)
{
  MyLabel.Content = "Starting...";
  var (observable, progress) = ObservableProgress.CreateForUi<int>();
  string result;
  using (observable.Subscribe(value => MyLabel.Content = value))
    result = await Task.Run(() => Solve(progress));
  MyLabel.Content = $"Done! Result: {result}";
}
```
