# Vagrant で Windows の仮想マシンを作る

Windows の仮想マシンを作る場合、ライセンスが必要だと思っていましたが、ライセンスフリーの評価版を使用することで作成可能なようです。  
Vagrant の 1.6 以下までは Vagrant-Windows プラグインを入れる必要がありましたが、今は Vagrant がネイティブに Windows をサポートしています。

[環境]  
・Windows10 22H2(64bit)  
※Windows11 からは、HyperV がインストールされており、手軽に仮想マシンを立ち上げられるようになっているようです。

[手順]  
※VirtualBox と Vagrant はインストール済みであることを前提とします  
※後片付けが出来たか確認するために、最初にディスクの空き容量を確認

- 1.Vagrant のバージョン確認

```
vagrant -v
Vagrant 2.3.3
```

- 2.フォルダ作成

```
mkdir c:\Vagranttest
cd c:\Vagranttest
```

- 3.VagrantFile を作成します  
  ユーザーが公開している windows-server を使用します。
  https://app.vagrantup.com/gusztavvargadr/boxes/windows-server  
  ここから検索できます。  
  https://app.vagrantup.com/boxes/search

  ```
  vagrant init gusztavvargadr/windows-server
  ```

* 4.仮想マシンの作成(20GB 程度の容量を必要とします)

```
vagrant up
Bringing machine 'default' up with 'virtualbox' provider...
==> default: Box 'gusztavvargadr/windows-server' could not be found. Attempting to find and install...
    default: Box Provider: virtualbox
    default: Box Version: >= 0
==> default: Loading metadata for box 'gusztavvargadr/windows-server'
    default: URL: https://vagrantcloud.com/gusztavvargadr/windows-server
==> default: Adding box 'gusztavvargadr/windows-server' (v2102.0.2208) for provider: virtualbox
    default: Downloading: https://vagrantcloud.com/gusztavvargadr/boxes/windows-server/versions/2102.0.2208/providers/virtualbox.box
    default:
    default: Calculating and comparing box checksum...
==> default: Successfully added box 'gusztavvargadr/windows-server' (v2102.0.2208) for 'virtualbox'!
==> default: Importing base box 'gusztavvargadr/windows-server'...
==> default: Matching MAC address for NAT networking...
==> default: Checking if box 'gusztavvargadr/windows-server' version '2102.0.2208' is up to date...
==> default: Setting the name of the VM: Vagranttest_default_1668820176233_27605
==> default: Clearing any previously set network interfaces...
==> default: Preparing network interfaces based on configuration...
    default: Adapter 1: nat
==> default: Forwarding ports...
    default: 3389 (guest) => 53389 (host) (adapter 1)
    default: 5985 (guest) => 55985 (host) (adapter 1)
    default: 5986 (guest) => 55986 (host) (adapter 1)
    default: 22 (guest) => 2222 (host) (adapter 1)
==> default: Running 'pre-boot' VM customizations...
==> default: Booting VM...
==> default: Waiting for machine to boot. This may take a few minutes...
    default: WinRM address: 127.0.0.1:55985
    default: WinRM username: vagrant
    default: WinRM execution_time_limit: PT2H
    default: WinRM transport: negotiate
==> default: Machine booted and ready!
[default] GuestAdditions 6.1.36 running --- OK.
==> default: Checking for guest additions in VM...
==> default: Mounting shared folders...
    default: /vagrant => C:/Vagranttest
```

- 5.VirtualBox からアクセスし、ライセンスを確認します。
  特にそれらしい記載がありません。  
  ![1](https://user-images.githubusercontent.com/49807271/202828202-05699189-5e36-43bb-b5e2-5057c4422f51.png)

- 6.winver にはライセンス情報が載っていませんでしたが、デスクトップに載っていました。6 っカ月で使用できなくなります。イメージによって初回起動時には表示されない場合もあるようなので、表示されない方は一回再起動をしてみるといいかもしれません。  
  ![2](https://user-images.githubusercontent.com/49807271/202828286-15c61ccd-6d7b-4b8e-8a1d-88256f15ecf8.png)

* 7.後片付けします。仮想マシンの終了。

```
vagrant halt
```

- 8.仮想マシンステータスの確認

```
>vagrant global-status
id       name    provider   state    directory
-------------------------------------------------------------------------
f354c4f  default virtualbox running  C:/Users/user/centos
c0a06f6  default virtualbox poweroff c:/Vagranttest
```

- 9.今回作成したものと以前作成したものがありますが、両方消します。

```
vagrant destroy c0a06f6
```

- 10.box 一覧の表示

```
vagrant box list
bento/centos-7.7              (virtualbox, 202005.12.0)
gusztavvargadr/windows-server (virtualbox, 2102.0.2208)
```

- 11.box 削除

```
vagrant box remove gusztavvargadr/windows-server
```

- 12.最後に実験前のディスク空き容量にほぼ戻っていることを確認

## ■ メモ

最初は WindowsXP の仮想マシンを作ろうとしていましたが(サイズが小さいので)、エラーが出ました。調べた限りでは Ming の問題のようです。  
 一応、下記の 2 通りの解決策を実施しましたが、解決しませんでした。  
 https://github.com/hashicorp/vagrant/issues/3869
