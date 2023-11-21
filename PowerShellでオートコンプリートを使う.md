# PowerShellでオートコンプリートを使う

### ■環境
* Windows11 22H2
* PowerShell v7.4.0  
https://github.com/PowerShell/PowerShell
* inshellisense  
https://github.com/microsoft/inshellisense

### ■導入
```
PS C:\Users\masami> node --version
v20.9.0
PS C:\Users\masami> npm install -g @microsoft/inshellisense

added 83 packages in 19s

24 packages are looking for funding
  run `npm fund` for details
npm notice
npm notice New minor version of npm available! 10.1.0 -> 10.2.4
npm notice Changelog: https://github.com/npm/cli/releases/tag/v10.2.4
npm notice Run npm install -g npm@10.2.4 to update!
npm notice
PS C:\Users\masami> inshellisense bind
Select your desired shell for keybinding creation
  bash
  zsh
  fish
  powershell
> pwsh

✓ successfully created new bindings
```

### ■使用方法
```
PS C:\Users\masami> is -s powershell
```
その後はインテリセンスが機能するようになります  
オートコンプリートはこちらのリポジトリを使用している模様  

https://github.com/withfig/autocomplete

[例]

![1](https://github.com/pea-sys/Til/assets/49807271/da377e02-692c-4157-ad5e-7de0cd6061ed)

![1](https://github.com/pea-sys/Til/assets/49807271/de4ed437-16a9-44a5-9852-605149a7fd0f)

![1](https://github.com/pea-sys/Til/assets/49807271/72d1b8b3-ac47-4750-9d9d-e42e3d66fcf6)