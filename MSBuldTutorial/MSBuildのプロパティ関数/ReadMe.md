```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <Path1>c:\users\</Path1>
    <Today>$([System.DateTime]::Now)</Today>
    <NewGuid>$([System.Guid]::NewGuid())</NewGuid>
    <GitVersionHeightWithOffset>$([System.Text.RegularExpressions.Regex]::Replace("$(PrereleaseVersion)", "^.*?(\d+)$", "$1", "System.Text.RegularExpressions.RegexOptions.ECMAScript"))</GitVersionHeightWithOffset>
    <Value1>$([MSBuild]::ValueOrDefault('$(UndefinedValue)', 'a'))</Value1>
  </PropertyGroup>


  <Target Name = "StringProperty">
    <Message Text ="$(Path1.Substring(0,3))" />
  </Target>
  <Target Name = "StaticProperty">
    <Message Text ="$(Today)" />
    <Message Text ="$(NewGuid)" />
  </Target>
  <Target Name = "ValueOrDefault">
     <Message Text="Value1 = $(Value1)" />
  </Target>
</Project>
```

```
PS C:\Users\user\source\repos\Til> C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj -t:StringProperty
Microsoft (R) Build Engine バージョン 4.8.4084.0
[Microsoft .NET Framework、バージョン 4.0.30319.42000]
Copyright (C) Microsoft Corporation. All rights reserved.

2023/06/16 21:23:48 にビルドを開始しました。
ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj" (StringProperty ターゲット)。
StringProperty:
  c:\users\
  c:\
プロジェクト "C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj" (StringProperty ターゲット) のビルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:01.81
```

```
PS C:\Users\user\source\repos\Til> C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj -t:StaticProperty
Microsoft (R) Build Engine バージョン 4.8.4084.0
[Microsoft .NET Framework、バージョン 4.0.30319.42000]
Copyright (C) Microsoft Corporation. All rights reserved.

2023/06/16 21:31:42 にビルドを開始しました。
ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj" (StaticProperty ターゲット)。
StaticProperty:
  2023/06/16 21:31:42
  9310d5cc-3050-490f-b0e4-1eeddcebad45
プロジェクト "C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj" (StaticProperty ターゲット) のビルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:01.65
```

```
PS C:\Users\user> C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj -t:ValueOrDefault
Microsoft (R) Build Engine バージョン 4.8.4084.0
[Microsoft .NET Framework、バージョン 4.0.30319.42000]
Copyright (C) Microsoft Corporation. All rights reserved.

2023/06/17 20:41:45 にビルドを開始しました。
ノード 1 上のプロジェクト "C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj" (ValueOrDefault ターゲット)。
ValueOrDefault:
  Value1 = a
プロジェクト "C:\Users\user\source\repos\MSBuildのプロパティ関数\sample.csproj" (ValueOrDefault ターゲット) のビルドが完了しました。


ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:01.43
```
