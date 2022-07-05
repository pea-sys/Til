## MSBuild プロジェクト ファイルのゼロからの作成

次のページのチュートリアル実施ログです。

[チュートリアル: MSBuild プロジェクト ファイルのゼロからの作成](https://learn.microsoft.com/ja-jp/visualstudio/msbuild/walkthrough-creating-an-msbuild-project-file-from-scratch?view=vs-2022)

通常 Visual Studio で自動生成している XML 形式のプロジェクトファイルをゼロから作成することで、プロジェクトファイルの中身を理解する。

以下内容で HelloWorld.cs を作成

```cs
using System;

class HelloWorld
{
    static void Main()
    {
#if DebugConfig
        Console.WriteLine("WE ARE IN THE DEBUG CONFIGURATION");
#endif

        Console.WriteLine("Hello, world!");
    }
}
```

csc でコンパイル

```
PS C:\Users\user\source\repos\MSBuldTutorial\HelloWorld> C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.cs
Microsoft (R) Visual C# Compiler version 4.8.4084.0
for C# 5
Copyright (C) Microsoft Corporation. All rights reserved.

This compiler is provided as part of the Microsoft (R) .NET Framework, but only supports language versions up to C# 5, which is no longer the latest version. For compilers that support newer versions of the C# programming language, see http://go.microsoft.com/fwlink/?LinkID=533240
```

最小構成のプロジェクトファイルを作成  
※チュートリアルに記載はないが、Project タグに xmlns 要素が必要

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <ItemGroup>
    <Compile Include="helloworld.cs"/>
  </ItemGroup>
  <Target Name="Build">
    <Csc Sources="@(Compile)"/>
  </Target>
</Project>
```

MSBuild でコンパイル

```
PS C:\Users\user\source\repos> C:\Windows\Microsoft.NET\Framework64\v3.5\MSBuild.exe C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj -t:Build
Microsoft (R) Build Engine Version 3.5.30729.9141
[Microsoft .NET Framework, Version 2.0.50727.9168]
Copyright (C) Microsoft Corporation 2007. All rights reserved.

Build started 2023/06/13 6:18:27.

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:05.51
```

プロパティとターゲットタスクの追加

```xml
<Project  xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <AssemblyName>MSBuildSample</AssemblyName>
    <OutputPath>Bin\</OutputPath>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="helloworld.cs" />
  </ItemGroup>
  <Target Name="Build">
    <MakeDir Directories="$(OutputPath)" Condition="!Exists('$(OutputPath)')" />
    <Csc Sources="@(Compile)" OutputAssembly="$(OutputPath)$(AssemblyName).exe" />
  </Target>
</Project>
```

MSBuild でコンパイル。Bin フォルダが作成され、MSBuildSample.exe が配置されます。

```
PS C:\Users\user\source\repos> C:\Windows\Microsoft.NET\Framework64\v3.5\MSBuild.exe C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj -t:Build
Microsoft (R) Build Engine Version 3.5.30729.9141
[Microsoft .NET Framework, Version 2.0.50727.9168]
Copyright (C) Microsoft Corporation 2007. All rights reserved.

Build started 2023/06/13 6:23:37.
Project "C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj" on node 0 (Build target(s)).
  ディレクトリ "Bin\" を作成しています。
Done Building Project "C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj" (Build target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.75
```

MSBuildSample.exe を実行

```
PS C:\Users\user\source\repos> C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\Bin\MSBuildSample.exe
Hello, world!
```

古いファイルを削除する Clean ターゲットと Rebuild ターゲットを追加

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <AssemblyName>MSBuildSample</AssemblyName>
    <OutputPath>Bin\</OutputPath>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="helloworld.cs"/>
  </ItemGroup>
  <Target Name="Build">
    <MakeDir Directories="$(OutputPath)" Condition="!Exists('$(OutputPath)')" />
    <Csc Sources="@(Compile)" OutputAssembly="$(OutputPath)$(AssemblyName).exe" />
  </Target>
  <Target Name="Clean" >
    <Delete Files="$(OutputPath)$(AssemblyName).exe" />
  </Target>
  <Target Name="Rebuild" DependsOnTargets="Clean;Build" />
</Project>
```

クリーンの動作確認

```
PS C:\Users\user\source\repos> C:\Windows\Microsoft.NET\Framework64\v3.5\MSBuild.exe C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj -t:Clean
Microsoft (R) Build Engine Version 3.5.30729.9141
[Microsoft .NET Framework, Version 2.0.50727.9168]
Copyright (C) Microsoft Corporation 2007. All rights reserved.

Build started 2023/06/13 6:49:46.
Project "C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj" on node 0 (Clean target(s)).
  ファイル "Bin\MSBuildSample.exe" を削除しています。
Done Building Project "C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj" (Clean target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.34
```

再度コンパイル

```
PS C:\Users\user\source\repos> cd C:\Users\user\source\repos\MSBuldTutorial\HelloWorld
PS C:\Users\user\source\repos\MSBuldTutorial\HelloWorld> C:\Windows\Microsoft.NET\Framework64\v3.5\MSBuild.exe
Microsoft (R) Build Engine Version 3.5.30729.9141
[Microsoft .NET Framework, Version 2.0.50727.9168]
Copyright (C) Microsoft Corporation 2007. All rights reserved.

Build started 2023/06/13 6:51:38.

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.60
```

インクリメンタルビルドするには、Inputs 属性をセットします。

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <AssemblyName>MSBuildSample</AssemblyName>
    <OutputPath>Bin\</OutputPath>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="helloworld.cs"/>
  </ItemGroup>
  <Target Name="Build" Inputs="@(Compile)" Outputs="$(OutputPath)$(AssemblyName).exe">
    <MakeDir Directories="$(OutputPath)" Condition="!Exists('$(OutputPath)')" />
    <Csc Sources="@(Compile)" OutputAssembly="$(OutputPath)$(AssemblyName).exe" />
  </Target>
  <Target Name="Clean" >
    <Delete Files="$(OutputPath)$(AssemblyName).exe" />
  </Target>
  <Target Name="Rebuild" DependsOnTargets="Clean;Build" />
</Project>
```

コンパイルすると、変更がないのでコンパイルスキップします。

```
PS C:\Users\user\source\repos\MSBuldTutorial\HelloWorld> C:\Windows\Microsoft.NET\Framework64\v3.5\MSBuild.exe
Microsoft (R) Build Engine Version 3.5.30729.9141
[Microsoft .NET Framework, Version 2.0.50727.9168]
Copyright (C) Microsoft Corporation 2007. All rights reserved.

Build started 2023/06/13 6:56:00.
Project "C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj" on node 0 (default targets).
Skipping target "Build" because all output files are up-to-date with respect to the input files.
Done Building Project "C:\Users\user\source\repos\MSBuldTutorial\HelloWorld\HelloWorld.csproj" (default targets).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.29
```
