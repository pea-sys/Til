# Windows DefenferのCPU使用率を制限する
Windowsで大量ファイル出力が発生する処理(ベンチマーキング、コンパイル、巨大リポジトリのクローン時など)実施時にタスクマネージャーを見ると、WindowsDefenderがCPUリソースを大量に使用していることが確認できる。
![1](https://github.com/pea-sys/Til/assets/49807271/4de08569-5459-485d-aaf2-3d366acb9d1d)
CPUリソースをWindowsDefender以外に渡したい場合は、PowerShellにて管理者権限で下記コマンドを実行することでCPUの使用率上限値を設定できる。  
デフォルトでは50%になっているので、それ以下にすることで効果が見込める。
```
Set-MpPreference -ScanAvgCPULoadFactor <percentage>
```
10%に設定する場合

```
Set-MpPreference -ScanAvgCPULoadFactor 10
```