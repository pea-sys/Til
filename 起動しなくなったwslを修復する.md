## 起動しなくなった wsl を修復する

何度も wsl に OS をインストールする作業を重ねていると
たまに wsl が起動できなくなる場面に遭遇する。

```
PS C:\Users\user> wsl --install Ubuntu-22.04
インストール中: Ubuntu 22.04 LTS
Ubuntu 22.04 LTS がインストールされました。
Ubuntu 22.04 LTS を起動しています...
ディスク 'C:\Users\user\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu22.04LTS_79rhkp1fndgsc\LocalState\ext4.vhdx' を WSL2 にアタッチできませんでした: 指定されたファイルが見つかりません。
Error code: Wsl/Service/CreateInstance/MountVhd/ERROR_FILE_NOT_FOUND
ディスク 'C:\Users\user\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu22.04LTS_79rhkp1fndgsc\LocalState\ext4.vhdx' を WSL2 にアタッチできませんでした: 指定されたファイルが見つかりません。
Error code: Wsl/Service/CreateInstance/MountVhd/ERROR_FILE_NOT_FOUND
ディスク 'C:\Users\user\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu22.04LTS_79rhkp1fndgsc\LocalState\ext4.vhdx' を WSL2 にアタッチできませんでした: 指定されたファイルが見つかりません。
Error code: Wsl/Service/CreateInstance/MountVhd/ERROR_FILE_NOT_FOUND
Press any key to continue...
```

まず、ディストリビューションをアンインストール

```
PS C:\Users\user> wsl -l
Linux 用 Windows サブシステム ディストリビューション:
Ubuntu-22.04 (既定)
PS C:\Users\user> wsl --unregister Ubuntu22.04
登録解除。
指定された名前のディストリビューションはありません。
Error code: Wsl/Service/WSL_E_DISTRO_NOT_FOUND
```

- MicroSoft Store で Ubuntu や Linux 用 Windows サブシステムをアンインストール
- コントロールパネルの Windows の機能の有効化または無効化で Linux 用 Windows サブシステムにチェックがついていたら外す

- 再起動後に、コントロールパネルの Windows の機能の有効化または無効化で Linux 用 Windows サブシステムにチェックをつける
- MicroSoft Store で Ubuntu や Linux 用 Windows サブシステムをインストール
  ※ MicroSoft Store 経由で Linux 用 Windows サブシステムをインストールしなかった場合、「wsl --update」を実行
