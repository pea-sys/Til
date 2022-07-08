## MSBuild の使用

次のページのチュートリアル実施ログです。

[チュートリアル: MSBuild の使用](https://learn.microsoft.com/ja-jp/visualstudio/msbuild/walkthrough-using-msbuild?view=vs-2022)

- MSBuild : Microsoft および Visual Studio のビルド プラットフォーム

- VisualStudio で WindowsForm で BuildApp プロジェクトを作成
  ![1](https://github.com/pea-sys/Til/assets/49807271/4ebaccdb-978d-4e3e-a95e-a88a0609d87f)

### ■ ターゲットとタスクを追加

- BuildApp.csproj ファイルをメモ帳で開いて Target セクションを追加

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <Target Name="HelloWorld">
    <Message Text="Hello"></Message>  <Message Text="World"></Message>
  </Target>
  <PropertyGroup>
```

- MSBuild で Target を指定してビルド

```
C:\Users\user\source\repos\MSBuldTutorial\BuildApp>msbuild C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj -t:HelloWorld
MSBuild version 17.5.1+f6fdcf537 for .NET Framework
2023/06/12 19:50:35 にビルドを開始しました。
Included response file: C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.rsp

ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット)。
HelloWorld:
  Hello
  World
プロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット) のビ ルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:00.27
```

### ■ プロパティ値を取得

以下のような感じで取得できる。

```
$(PropertyName)
```

```xml
<Target Name="HelloWorld">
  <Message Text="Configuration is $(Configuration)" />
  <Message Text="MSBuildToolsPath is $(MSBuildToolsPath)" />
</Target>
```

- MSBuild で Target を指定してビルド

```
C:\Users\user\source\repos\MSBuldTutorial\BuildApp>msbuild C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj -t:HelloWorld
MSBuild version 17.5.1+f6fdcf537 for .NET Framework
2023/06/12 20:08:11 にビルドを開始しました。
Included response file: C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.rsp

ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット)。
HelloWorld:
  Configuration is Debug
  MSBuildToolsPath is C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64
プロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット) のビ ルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:00.26
```

### ■ コマンドラインからプロパティを設定

プロパティは、コマンド ライン スイッチの -property または -p を使用してコマンド ラインで定義できる

```
C:\Users\user\source\repos\MSBuldTutorial\BuildApp>msbuild C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj -t:HelloWorld -p:Configuration=Release
MSBuild version 17.5.1+f6fdcf537 for .NET Framework
2023/06/12 20:13:13 にビルドを開始しました。
Included response file: C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.rsp

ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット)。
HelloWorld:
  Configuration is Release
  MSBuildToolsPath is C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64
プロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット) のビ ルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:00.25
```

## ■ 特殊文字

%等の制御文字を文字列として扱いたい場合は、代替表現を使用する

```xml
  <Target Name="HelloWorld">
    <Message Text="%24(Configuration) is %22$(Configuration)%22" />
  </Target>
```

```
C:\Users\user\source\repos\MSBuldTutorial\BuildApp>msbuild C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj -t:HelloWorld -p:Configuration=Release
MSBuild version 17.5.1+f6fdcf537 for .NET Framework
2023/06/12 20:15:40 にビルドを開始しました。
Included response file: C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.rsp

ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット)。
HelloWorld:
  $(Configuration) is "Release"
プロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット) のビ ルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー
```

### ■ 項目の種類の値を確認

```xml
<Message Text="Compile item type contains  @(Compile, '%0A')" />
```

```
C:\Users\user\source\repos\MSBuldTutorial\BuildApp>msbuild C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj -t:HelloWorld
MSBuild version 17.5.1+f6fdcf537 for .NET Framework
2023/06/12 20:20:27 にビルドを開始しました。
Included response file: C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.rsp

ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット)。
HelloWorld:
  Compile item type contains  Form1.cs
  Form1.Designer.cs
  Program.cs
  Properties\AssemblyInfo.cs
  Properties\Resources.Designer.cs
  Properties\Settings.Designer.cs
プロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット) のビ ルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:00.25
```

### ■ 項目の追加

項目を追加する場合、Include 要素を使用します。  
images フォルダ配下の jpg を項目に追加します。  
プロジェクトファイルの中身を以下のように編集。

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <ItemGroup>
    <Photo Include="images/*jpg" />
  </ItemGroup>
  <Target Name="HelloWorld">
    <Message Text="Photo item type contains @(Photo)" />
  </Target>
```

```
C:\Users\user\source\repos\MSBuldTutorial\BuildApp>msbuild C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj -t:HelloWorld
MSBuild version 17.5.1+f6fdcf537 for .NET Framework
2023/06/12 21:06:16 にビルドを開始しました。
Included response file: C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.rsp

ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット)。
HelloWorld:
  Photo item type contains images\shokupan.jpg
プロジェクト "C:\Users\user\source\repos\MSBuldTutorial\BuildApp\BuildApp\buildapp.csproj" (HelloWorld ターゲット) のビ ルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:00.24
```
