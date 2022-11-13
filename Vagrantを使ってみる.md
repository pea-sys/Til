# Vagrant を触ってみる

Vagrant は仮想環境の構築・設定を容易に行えるように作られた
VirtualBox や VMWare 等のフロントツールです。

Qiita を参考に試しに使ってみます。  
https://qiita.com/kanazwk/items/6d0c62480580a570cbb5

[手順]

- 1.Vagrant をインストール  
  https://developer.hashicorp.com/vagrant/downloads

* 2.C:\HashiCorp\Vagrant\bin を環境変数に設定

* 3.VirtualBox をインストール  
  https://www.oracle.com/jp/virtualization/technologies/vm/downloads/virtualbox-downloads.html

* 4.CentOS7 box の追加

```
vagrant box add bento/centos-7.7 --provider virtualbox
==> box: Loading metadata for box 'bento/centos-7.7'
    box: URL: https://vagrantcloud.com/bento/centos-7.7
==> box: Adding box 'bento/centos-7.7' (v202005.12.0) for provider: virtualbox
    box: Downloading: https://vagrantcloud.com/bento/boxes/centos-7.7/versions/202005.12.0/providers/virtualbox.box
    box:
==> box: Successfully added box 'bento/centos-7.7' (v202005.12.0) for 'virtualbox'!

```

- 5.vagrant 管理用フォルダの作成と初期化

```
C:\Users\user>mkdir centos
C:\Users\user>cd centos
C:\Users\user\centos>vagrant init bento/centos-7.7
```

- 6.vagrantfile を編集し、プライベートネットワークを構築します。

```
config.vm.network "private_network", ip: "192.168.33.11"
```

- 7.仮想マシンを起動

```
C:\Users\user\centos>vagrant up
Bringing machine 'default' up with 'virtualbox' provider...
==> default: Importing base box 'bento/centos-7.7'...
==> default: Matching MAC address for NAT networking...
==> default: Checking if box 'bento/centos-7.7' version '202005.12.0' is up to date...
==> default: Setting the name of the VM: centos_default_1668117723361_92753
Vagrant is currently configured to create VirtualBox synced folders with
the `SharedFoldersEnableSymlinksCreate` option enabled. If the Vagrant
guest is not trusted, you may want to disable this option. For more
information on this option, please refer to the VirtualBox manual:

  https://www.virtualbox.org/manual/ch04.html#sharedfolders

This option can be disabled globally with an environment variable:

  VAGRANT_DISABLE_VBOXSYMLINKCREATE=1

or on a per folder basis within the Vagrantfile:

  config.vm.synced_folder '/host/path', '/guest/path', SharedFoldersEnableSymlinksCreate: false
==> default: Clearing any previously set network interfaces...
==> default: Preparing network interfaces based on configuration...
    default: Adapter 1: nat
    default: Adapter 2: hostonly
==> default: Forwarding ports...
    default: 22 (guest) => 2222 (host) (adapter 1)
==> default: Booting VM...
==> default: Waiting for machine to boot. This may take a few minutes...
    default: SSH address: 127.0.0.1:2222
    default: SSH username: vagrant
    default: SSH auth method: private key
    default: Warning: Connection reset. Retrying...
    default: Warning: Connection aborted. Retrying...
    default: Warning: Connection reset. Retrying...
    default: Warning: Connection aborted. Retrying...
    default: Warning: Connection reset. Retrying...
    default:
    default: Vagrant insecure key detected. Vagrant will automatically replace
    default: this with a newly generated keypair for better security.
    default:
    default: Inserting generated public key within guest...
    default: Removing insecure key from the guest if it's present...
    default: Key inserted! Disconnecting and reconnecting using new SSH key...
==> default: Machine booted and ready!
==> default: Checking for guest additions in VM...
==> default: Configuring and enabling network interfaces...
==> default: Mounting shared folders...
    default: /vagrant => C:/Users/user/centos
```

- 8.SSH 接続してみます  
  id :root  
  password:vagrant  
  でログインできます。

```
This system is built by the Bento project by Chef Software
More information can be found at https://github.com/chef/bento
[root@localhost ~]#
```

- 9.Timezone を Asia/Tokyo にします

```
[root@localhost ~]# timedatectl set-timezone Asia/Tokyo
```

- 10.日時を確認します

```
[root@localhost ~]# timedatectl
      Local time: Fri 2022-11-11 20:58:32 JST
  Universal time: Fri 2022-11-11 11:58:32 UTC
        RTC time: Fri 2022-11-11 20:58:32
       Time zone: Asia/Tokyo (JST, +0900)
     NTP enabled: yes
NTP synchronized: yes
 RTC in local TZ: yes
      DST active: n/a

Warning: The system is configured to read the RTC time in the local time zone.
         This mode can not be fully supported. It will create various problems
         with time zone changes and daylight saving time adjustments. The RTC
         time is never updated, it relies on external facilities to maintain it.
         If at all possible, use RTC in UTC by calling
         'timedatectl set-local-rtc 0'.
```

11.警告通り、set-local-rtc 0 します。

```
[root@localhost ~]# timedatectl set-local-rtc 0
[root@localhost ~]# timedatectl
      Local time: Fri 2022-11-11 21:04:42 JST
  Universal time: Fri 2022-11-11 12:04:42 UTC
        RTC time: Fri 2022-11-11 12:04:42
       Time zone: Asia/Tokyo (JST, +0900)
     NTP enabled: yes
NTP synchronized: yes
 RTC in local TZ: no
      DST active: n/a
```

12.パッケージグループのインストールとアップデート

```
yum -y group install infrastructure-server-environment
yum -y group install developer-workstation-environment
```

- 13.パッケージの更新

```
yum -y update
```

- 14.DKMS のインストール

```
yum install epel-release
yum install dkms
```

- 15.仮想マシンの停止

```
vagrant halt
```

- 16.仮想マシン名の確認

```
vagrant status
Current machine states:

default                   poweroff (virtualbox)
```

- 17.イメージの保存

```
C:\Users\user\centos>vagrant package default --output centos7_master.box
==> default: Clearing any previously set forwarded ports...
==> default: Exporting VM...
==> default: Compressing package to: C:/Users/user/centos/centos7_master.box
```

- 18.開発環境のデプロイテスト

```
vagrant box add jupiter centos7_master.box
==> box: Box file was not detected as metadata. Adding it directly...
==> box: Adding box 'jupiter' (v0) for provider:
    box: Unpacking necessary files from: file://C:/Users/user/centos/centos7_master.box
    box:
==> box: Successfully added box 'jupiter' (v0) for 'virtualbox'!
```

- 19.テスト用フォルダを作成し、vagrant ファイルを作成

```
mkdir ..\centos-test
cd ..\centos-test
vagrant init jupiter
```

- 20.仮想マシンを起動します

```
vagrant up
```

- 21.SSH で接続します

```
Last login: Fri Nov 11 20:39:59 2022 from 192.168.33.1

This system is built by the Bento project by Chef Software
More information can be found at https://github.com/chef/bento
[root@localhost ~]#
```

- 22.設定が引き継がれていることを確認

```
[root@localhost ~]# timedatectl
      Local time: Sat 2022-11-12 08:13:02 JST
  Universal time: Fri 2022-11-11 23:13:02 UTC
        RTC time: Fri 2022-11-11 23:13:02
       Time zone: Asia/Tokyo (JST, +0900)
     NTP enabled: yes
NTP synchronized: yes
 RTC in local TZ: no
      DST active: n/a
```

- 23.仮想マシンの終了

```
vagrant halt
```

- 24.アクティブな仮想マシンを確認

```
vagrant global-status
id       name    provider   state    directory
-------------------------------------------------------------------------
fded233  default virtualbox poweroff C:/Users/user/centos
8448129  default virtualbox poweroff C:/Users/user/centos-test
```

- 25.仮想マシンの破棄

```
C:\Users\user>vagrant destroy fded233
    default: Are you sure you want to destroy the 'default' VM? [y/N] y
==> default: Destroying VM and associated drives...

C:\Users\user>vagrant destroy 8448129
    default: Are you sure you want to destroy the 'default' VM? [y/N] y
```

- 26.box の一覧表示

```
vagrant box list
bento/centos-7.7 (virtualbox, 202005.12.0)
jupiter          (virtualbox, 0)
```

- 27.box の削除

```
C:\Users\user>vagrant box remove jupiter
Removing box 'jupiter' (v0) with provider 'virtualbox'...

C:\Users\user>vagrant box remove bento/centos-7.7
Removing box 'bento/centos-7.7' (v202005.12.0) with provider 'virtualbox'...
```
