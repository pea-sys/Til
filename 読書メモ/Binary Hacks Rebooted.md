## ダイナミック隣家が共通ライブラリを検索するディレクトリ

次の順で検索する

- PT_DYAMIC セグメントの中の DT_RPATH の値。
- 環境変数 LD_LIBRARY_PATH の値。ただし、セキュリティ実行モードだと無視される。
- PT_DYNAMIC セグメントの中の DT_RUNPATH の値。
- /etc/ld.so.cache の中身。
- /lib および/usr/lib。６４ビットアーキテクチャの場合、/lib64 と/usr/lib64 を使う場合がある。

## dlopen によるライブラリの実行時ロードとその応用テクニック

- LD_PRELOAD で既存のライブラリ関数をラップ可能(`malloc`にログ追加等)

```c
// mallocのラッパ関数
void *malloc(size_t size) {
  // 初回呼び出し時にreal_mallocを初期化
  if (real_malloc == NULL) {
    // RTLD_NEXTにより、これより後にロードされた共有ライブラリのmallocをみつける
    real_malloc = dlsym(RTLD_NEXT, "malloc");

    if (real_malloc == NULL) {
      fprintf(stderr, "dlsym error: %s\n", dlerror());
      exit(1);
    }
  }

  // ちなみに、（実装依存だが）ここの出力先をstdoutにすると
  // 内部でバッファリングのためにmallocが呼ばれて無限再帰になってしまう
  fprintf(stderr, "malloc: size=%zu\n", size);

  // オリジナルのmallocを呼び出す
  return real_malloc(size);
}
```

- main 関数も同様にラップ可能。main 関数の前にサンドボックス初期化処理を入れることで、サンドボックス内で execve のようなシステムコールを許可するような穴をあけなくて済む。

## IFUNC を使って実行時に実装を切り替える

- main の前に呼ばれうる関数は、`__attribute__((constructor))`, resolve_function,C++のグローバル変数のコンストラクタ

## TLS の仕組みを理解する

- Local Executable アクセスモデル
- Initial Executable アクセスモデル
- General Dynamic アクセスモデル
- Local Dynamic アクセスモデル

## 補助ベクトルを使ってプロセスに情報を渡す

- Linux の実行可能バイナリは起動時にコマンドライン引数・環境変数・補助ベクトル情報を受け取る
- 補助ベクトルはシグナル配送に必要なスタックサイズやシステムのページサイズ、プログラムヘッダのポインタが含まれる

## sold を使って依存する共有ライブラリを後からリンクする

sold は動的リンクされたアプリケーションやライブラリに、それらが依存する共有ライブラリを後からリンクするためのソフトウェア

## glibc を Hack する

通常、ELF バイナリはダイナミックリンカへのパスをメタデータとして保持している。
ELF バイナリをシェルから起動すると自動的にメタデータで指定されたダイナミックリンカが呼び出されて、ELF バイナリを起動するための処理を行います。
しかし、シェルから起動して実行したいバイナリを引数に渡せば、メタデータを無視できる

## patchelf で ELF バイナリのフィールドを書き換える

patchelf とは ELF バイナリを編集するためのツール。このツールを使うことでソースコードを再コンパイルすることなく挙動が変えられる。

- PT_INTERP：ELF バイナリを起動するダイナミックリンカを指定
- DT_RUNPATH：起動時に依存する共有ライブラリを探すパスを指定
- DT_NEEDED：依存する共有ライブラリを指定

## LIFE を使って ELF バイナリを書き換える

LIEF とは ELF,PE,Mach-O などのバイナリファイルを編集できるツール。シンボルのリネーム、関数の中身の書き換え等の操作が可能。
ハッシュテーブルの再構築し差し替える作業は難しいので、自分で実装するのは避けるべき。

## PT_NOTE を利用したバイナリパッチ

PT_NOTE タイプのプログラムヘッダは書き換えても問題ないことが多いため、簡単なバイナリパッチに利用できるケースが多い。

## DWARF Hack

特に意義はなさそうなので本章はスキップ

## 実行可能ファイルとその起動方法

Linux は wasm を実行可能ファイルとしてサポートしていないが、binfmt_misc に新しいバイナリタイプとして登録することで実行可能になる

## CRIU を使ってプロセスを保存、再開する

CRIU はプロセスを保存・再開するツール。再開時は ns_last_pid ファイルを利用して同じプロセス ID で動作する。
criu-us を使い pid ネームスペースを unshare することで同一の pid で複数起動することも可能。

## procfs / sysfs の基本を把握する

- fd ディレクトリを使うと、プロセスが開いているファイルに対して読み書きできる。
- mem,maps ファイルを用いると、procfs を通じて好きなプロセスのメモリを読み書きできる。
- drop_cache に特定の内容を書き込むとカーネルのメモリキャッシュを削除できる。1:ページキャッシュ、2:inode,dentry, 3:両方

```
ch03_os/22_various_executables - [main●] » free -mh
               total        used        free      shared  buff/cache   available
Mem:           3.8Gi       632Mi       3.2Gi        16Mi       216Mi       3.2Gi
Swap:          1.0Gi          0B       1.0Gi
ch03_os/22_various_executables - [main●] » echo 3 | sudo tee /proc/sys/vm/drop_caches
3
ch03_os/22_various_executables - [main●] » free -mh
               total        used        free      shared  buff/cache   available
Mem:           3.8Gi       614Mi       3.2Gi        16Mi       165Mi       3.2Gi
Swap:          1.0Gi          0B       1.0Gi
```

※sync コマンドと合わせてダーティキャッシュを消す場合が多い

## 特定のプロセスに見せるファイルを差し替える

- バインドマウントでファイルやディレクトリを一時的に置き換える(システム全体が対象)
- マウントネームスペースとバインドマウントで特定のプロセスのみ置換を行う

## FUSE を使ってファイルシステムを自作する

- FUSE とは、ユーザー空間内で独自のファイルシステムを作ることができる仕組み
- FUSE はユーザースペースの処理を挟むので、一般にパフォーマンスが劣っている
- 適切にキャッシュを設定・FUSE デーモンのマルチスレッド化・DAX・FUSE Passthrogh によりパフォーマンス向上が望める

## 特殊なメモリ領域 vsyscall と DSO

・vsyscall は一部のシステムコールの呼び出しを高速化するために導入。ユーザーランドとカーネルのデータの受け渡し領域として利用し、システムコール呼び出しを低減させる
・vsyscall はデータの受け渡しだけでなく、データを取得する関数も含んでいるため、vsyscall にある関数を呼び出すことでシステムコールを経由せずにシステムコールと同等の処理が実行できる

- 現在は vsyscall を共有ライブラリ化した vDSO が主流で vsyscall はデフォルトで無効になっているディストリビューションが多い

## KVM を使ってハイパーバイザを作成する

- KVN は Linux で仮想化技術を使うためのカーネルモジュール。Intel VT-x や AMD-V のようなハードウェア仮想化支援機能を使って VM を実行するため、エミュレータよりもオーバーヘッドが少ない。

* KVM を用いる場合は、VM のアーキテクチャはホストと同じアーキテクチャである必要がある
* KVM は/dev/kvn に対して ioctl で API を呼び出すことで操作できる

## Unikernel アプリケーションを OS として動かす

- Unikernel とは、VM を前提としてアプリケーションに必要な機能に最適化されたカーネルを作り、アタックサーフェスの減少によるセキュリティの向上、ファイルサイズや消費メモリサイズの効率化。起動時間の高速化、そしてパフォーマンス向上を目指す OS のアーキテクチャのこと

## Linux ネームスペースでプロセスを分離する

- コンテナはプロセスの実行環境と定義できる
- コンテナを作る要素技術
  - Linux ネームスペース・・・プロセスの集合ごとにアクセスできるカーネルリソースを分離する機能
  - cgroup
  - chroot と pivot_root
  - ケーバビリティなど

## Linux ネームスペース

ネームスペースの種類

- マウント：ファイルシステムの分離
- UTS:hostname, domain name の分離
- IPC:System V IPC オブジェクトや POSIX message queue の分離
- ネットワーク：IP アドレスやルーティングテーブルのようなネットワーク資源の分離
- PID:プロセス ID の集合を分離
- Control Group(cgroup):cgroup を分離。ネストされたコンテナなどに利用できる
- ユーザー：ユーザー ID,グループ ID、ケーパビリティの分離
- Time:CLOCK_MONOTONIC や CLOCK_BOOTTIME の分離

wsl でネームスペースを使う場合は設定変更が必要

```
[wsl2]
nestedVirtualization=true
```

```
Users/masami » echo $$
558
Users/masami » ps aux
USER         PID %CPU %MEM    VSZ   RSS TTY      STAT START   TIME COMMAND
root           1  4.0  0.3  21872 13044 ?        Ss   19:58   0:01 /sbin/init
root           2  0.0  0.0   2616  1440 ?        Sl   19:58   0:00 /init
root           7  0.0  0.0   2616   132 ?        Sl   19:58   0:00 plan9 --control-socket 6 --log-level 4 --server-fd 7
root          54  1.3  0.3  42096 15548 ?        S<s  19:58   0:00 /usr/lib/systemd/systemd-journald
root         100  1.0  0.1  24124  6216 ?        Ss   19:58   0:00 /usr/lib/systemd/systemd-udevd
root         113  0.0  0.0 152936  2220 ?        Ssl  19:58   0:00 snapfuse /var/lib/snapd/snaps/core22_1663.snap /snap/
root         114  0.0  0.0 227832   840 ?        Ssl  19:58   0:00 snapfuse /var/lib/snapd/snaps/core22_1722.snap /snap/
root         116  0.0  0.0 152936   168 ?        Ssl  19:58   0:00 snapfuse /var/lib/snapd/snaps/snapd_23258.snap /snap/
root         124 15.2  0.3 526756 12472 ?        Ssl  19:58   0:03 snapfuse /var/lib/snapd/snaps/snapd_23545.snap /snap/
systemd+     171  0.9  0.3  21452 11948 ?        Ss   19:58   0:00 /usr/lib/systemd/systemd-resolved
systemd+     172  0.6  0.1  91020  6520 ?        Ssl  19:58   0:00 /usr/lib/systemd/systemd-timesyncd
root         235  0.0  0.0   4236  2768 ?        Ss   19:58   0:00 /usr/sbin/cron -f -P
message+     236  0.3  0.1   9588  5112 ?        Ss   19:58   0:00 @dbus-daemon --system --address=systemd: --nofork --n
root         246  4.2  0.8 1469160 32448 ?       Ssl  19:58   0:00 /usr/lib/snapd/snapd
root         247  0.5  0.2  17976  8392 ?        Ss   19:58   0:00 /usr/lib/systemd/systemd-logind
root         252  0.6  0.3 1756096 14272 ?       Ssl  19:58   0:00 /usr/libexec/wsl-pro-service -vv
root         267  0.0  0.0   3160  1076 hvc0     Ss+  19:58   0:00 /sbin/agetty -o -p -- \u --noclear --keep-baud - 1152
syslog       270  0.3  0.1 222508  5324 ?        Ssl  19:58   0:00 /usr/sbin/rsyslogd -n -iNONE
root         287  0.0  0.0   3116  1196 tty1     Ss+  19:58   0:00 /sbin/agetty -o -p -- \u --noclear - linux
root         308  1.0  0.5 107012 22596 ?        Ssl  19:58   0:00 /usr/bin/python3 /usr/share/unattended-upgrades/unatt
postgres     392  0.2  0.7 221028 30988 ?        Ss   19:58   0:00 /usr/lib/postgresql/17/bin/postgres -D /var/lib/postg
postgres     409  0.0  0.2 221164  8512 ?        Ss   19:58   0:00 postgres: 17/main: checkpointer
postgres     410  0.0  0.1 221176  5900 ?        Ss   19:58   0:00 postgres: 17/main: background writer
postgres     458  0.0  0.2 221028 10344 ?        Ss   19:58   0:00 postgres: 17/main: walwriter
postgres     459  0.0  0.2 222596  8276 ?        Ss   19:58   0:00 postgres: 17/main: autovacuum launcher
postgres     460  0.0  0.1 222604  6768 ?        Ss   19:58   0:00 postgres: 17/main: logical replication launcher
root         467  0.3  0.1  17276  6616 ?        Ss   19:58   0:00 /usr/lib/systemd/systemd-timedated
root         550  0.0  0.0   2616   120 ?        Ss   19:58   0:00 /init
root         551  0.0  0.0   2616   120 ?        S    19:58   0:00 /init
masami       558  4.5  0.1   9192  7684 pts/0    Ss   19:58   0:00 -zsh
root         559  0.1  0.1   6696  4684 pts/1    Ss   19:58   0:00 /bin/login -f
masami       591  0.0  0.0   8436   764 ?        Ss   19:58   0:00 ssh-agent
masami       622  1.2  0.2  20324 11592 ?        Ss   19:58   0:00 /usr/lib/systemd/systemd --user
masami       623  0.0  0.0  21144  1724 ?        S    19:58   0:00 (sd-pam)
masami       673  1.6  0.1   8200  6740 pts/1    S+   19:58   0:00 -zsh
masami       777  0.0  0.0   8336  3836 pts/0    R+   19:58   0:00 ps aux
Users/masami » sudo unshare --pid --mount --fork /bin/bash
[sudo] password for masami:
root@Ubuntu2404-WSL2:/mnt/c/Users/masami# echo $$
1
root@Ubuntu2404-WSL2:/mnt/c/Users/masami# mount -t proc procfs /proc
root@Ubuntu2404-WSL2:/mnt/c/Users/masami# ps aux
USER         PID %CPU %MEM    VSZ   RSS TTY      STAT START   TIME COMMAND
root           1  0.0  0.1   5016  4180 pts/2    S    20:00   0:00 /bin/bash
root           9  0.0  0.1   8332  4344 pts/2    R+   20:00   0:00 ps aux
```

cgroup v2 の代表的なコントローラ

- CPUSet:CPU コアやメモリノード
- CPU:CPU サイクル
- IO:I/O リソース
- Memory:メモリ
- PID:プロセス数
- Device:デバイスファイルの作成やアクセス

wsl で cgroup v2 を使う場合は config 設定する

```
[wsl2]
localhostForwarding=true
kernelCommandLine = cgroup_no_v1=all
```

```
Users/masami » cat /sys/fs/cgroup/cgroup.controllers
cpuset cpu io memory hugetlb pids rdma misc
Users/masami » sudo mkdir /sys/fs/cgroup/my-test
[sudo] password for masami:
Users/masami » echo $$ | sudo tee /sys/fs/cgroup/my-test/cgroup.procs
493
Users/masami » cat /proc/$$/cgroup
0::/my-test
Users/masami » systemd-cgls
CGroup /:
-.slice
├─user.slice
│ └─user-1002.slice
│   ├─user@1002.service …
│   │ └─init.scope
│   │   ├─553 /usr/lib/systemd/systemd --user
│   │   └─554 (sd-pam)
│   └─session-1.scope
│     ├─494 /bin/login -f
│     └─601 -zsh
├─init.scope
│ ├─  1 /sbin/init
│ ├─  2 /init
│ ├─  7 plan9 --control-socket 6 --log-level 4 --server-fd 7 --pipe-fd 9 --log-truncate
│ ├─491 /init
│ ├─492 /init
│ └─525 ssh-agent
├─system.slice
│ ├─systemd-udevd.service …
│ │ └─udev
│ │   └─100 /usr/lib/systemd/systemd-udevd
│ ├─cron.service
│ │ └─231 /usr/sbin/cron -f -P
│ ├─snap-snapd-23545.mount
│ │ └─123 snapfuse /var/lib/snapd/snaps/snapd_23545.snap /snap/snapd/23545 -o ro,nodev,allow_other,suid
│ ├─systemd-journald.service
│ │ └─54 /usr/lib/systemd/systemd-journald
│ ├─unattended-upgrades.service
│ │ └─277 /usr/bin/python3 /usr/share/unattended-upgrades/unattended-upgrade-shutdown --wait-for-signal
│ ├─snap-core22-1663.mount
│ │ └─118 snapfuse /var/lib/snapd/snaps/core22_1663.snap /snap/core22/1663 -o ro,nodev,allow_other,suid
│ ├─snapd.service
│ │ └─239 /usr/lib/snapd/snapd
```

メモリ使用量制限

```
Users/masami » cat /sys/fs/cgroup/my-test/memory.max
max
tee: /sys/fs/cgroup/my-test/memory.max: Invalid argument
Users/masami » echo '128M' | sudo tee /sys/fs/cgroup/my-test/memory.max
128M
Users/masami » cat /sys/fs/cgroup/my-test/memory.max
134217728
```

PID のコントロールで fork bomb をの被害を防ぐ

```
Users/masami » :(){ :|:& };:
[2] 1238 1239
[2]  + 1238 done       : |
       1239 done       :
PS C:\Users\masami> wsl
Users/masami » echo 20 | sudo tee /sys/fs/cgroup/my-test/pids.max
[sudo] password for masami:
20
Users/masami » :(){ :|:& };:
[2] 2419 2420
[2]  + 2419 done       : |
       2420 done       :
Users/masami » :: fork failed: resource temporarily unavailable
:: fork failed: resource temporarily unavailable
:: fork failed: resource temporarily unavailable
```

## chroot/pivot_root でルートディレクトリを切り替える

- chroot は現在のプロセスと子プロセスたちのルートディレクトリを変えるシステムコール
- chroot は脱出方法がいくつもあるため、コンテナなどのセキュリティ用途には使用できない

* pivot_root は実行したプロセスのルートファイルシステムを置き換えるシステムコール
* pivot_root は脱出方法はないが、pivot_root を実行した後に元のルートファイルを覗き見る方法が存在するため、PID のネームスペースの分離、cgroup の device controller を使う

## 一般ユーザーが root のように振る舞う方法

### setuid を設定する

- sudo コマンドで setuid(アクセス権フラグ)をセットする（ファイル所有者の権限で実行）
  下記ファイルの所有者は root なので root 権限で実行される

```
Users/masami » ls -l /usr/bin/sudo
-rwsr-xr-x 1 root root 277936 Apr  8  2024 /usr/bin/sudo
```

コピーすると、sudo としての機能をもたなくなる

```
Users/masami » cp /usr/bin/sudo my-sudo
Users/masami » ls -l ./my-sudo
-rwxrwxrwx 1 masami masami 277936 Jan 19 08:34 ./my-sudo
Users/masami » ./my-sudo touch /foo
sudo: ./my-sudo must be owned by uid 0 and have the setuid bit set
```

### 必要なケーパビリティを付与

root 権限を細分化したもので、スレッドごとに付与できる権限

代表的な Linux ケーパビリティ

- CAP_SYS_ADMIN：システム管理のための多くの操作
- CAP_NET_AD：ネットワーク管理の操作
- CAP_CHOWN：ファイルの所有者の変更
- CAP_SETUID：プロセスのユーザー ID に関する捜査
- CAP_SYS_CHROOT：chroot の呼び出し
- CAP_SYS_PTRACE：ptrace の呼び出し
- CAP_SYS_RAWIO：I/O ポートやメモリへの直接アクセス

* CAP_SYSLOG：syslog に関する特権操作
* CAP_DAC_READ_SEARCH：ファイルやディレクトリの読み取り権限などのチェックをバイパス

```
Users/masami » cp $(which cat) my-cat
Users/masami » ls -l ./my-cat
-rwxrwxrwx 1 masami masami 39384 Jan 19 08:49 ./my-cat
Users/masami » ls -l /etc/sudoers
-r--r----- 1 root root 1800 Jan 30  2024 /etc/sudoers
Users/masami » ./my-cat /etc/sudoers
./my-cat: /etc/sudoers: Permission denied
Users/masami » sudo setcap cap_dac_read_search=ep ./my-cat
Users/masami » ./my-cat /etc/sudoers
#
# This file MUST be edited with the 'visudo' command as root.
・・・

```

Docker コンテナのケーパビリティ

- 通常の root 権限に比べて、出来ることがかなり限られている
- docker run に--cap_add=SYSLOG を渡すと dmesg が見られるようになったり、追加での権限付与は可能
- --privileged フラグで全てのケーパビリティを持った特権コンテナが作れる(Docker in docker などで使用)

### ユーザーネームスペースの中で root になる

限定的な状況で一般ユーザーでも root として振る舞える方法

## rootless コンテナの使い方とそのしくみ

- rootless コンテナは、非特権ユーザーがコンテナを作ることが出来る技術です
- Podman が該当する。Docker と異なり、非特権ユーザーの操作で完結する
- コンテナ内では root ユーザーとして振る舞うことが出来る
- rootless コンテナは上記動作をユーザーネームスペースで実現している

## ユーザーネームスペース内で各種のネームスペースを作成する

- uid_map ファイルは一度しか書き込めない。二度目以降は失敗する。
- map ファイルに「0 1000 1」と書き込んだ場合、ユーザーネームスペース内で０から始まるユーザー ID を、ユーザーネームスペース外のユーザー ID1000 から始まるユーザー ID に１つ分対応させることになる。
  「0 1000 3」の場合は、1000 ～ 1002 が対応する。

* ユーザーネームスペース内のマウントネームスペースには、マウント可能なファイルシステムの種類に大きな制限がある
* ユーザーネームスペース内のネットワークスペースは外部のネットワークにつなげる簡単な方法がない。rootful コンテナの場合は、root 権限で veth を利用しネームスペース内外のネットワークをつなぐ。
  rootless コンテナである Podman は slirp4nets(ファイルディスクリプタの受け渡し)を使用し、ネットワークネームスペース内外の接続を可能にしている
* rootless コンテナは well-known ポートがリッスンができない。ネットワークパフォーマンスが低下するという問題がある

## /proc/PID/root からコンテナ内のファイルに直接アクセス

- docker コンテナ内部のファイルにアクセスする最も一般的な方法は、docker exec と docker cp だが、procfs 上の/proc/PID/root にアクセスする方法もある。これは Docker コンテナのルートディレクトリのシンボリックリンクのようなものである。

```
PID=$(docker inspect -f '{{.State.Pid}}' mycon)
sudo ls /proc/$PID/root
```

## dbg サニタイザを使ってみる

サニタイズの種類

- Address Sanitizer:Use-after-free,バッファオーバーフロー
- Leak Sanitizer:メモリリーク
- Undefined Behavior Sanitize;未定義動作
- Thread Sanitizer: データレース
- Memory Sanitizer:未初期化メモリの使用
- Control Flow Integerity:プログラムの制御フローを破壊しうる未定義動作

asan

```
ch05_debugger_tracer/46_sanitizer_1 - [main●] » clang use-after-free.c -o use-after-free
ch05_debugger_tracer/46_sanitizer_1 - [main●] » ./use-after-free
1534861583
ch05_debugger_tracer/46_sanitizer_1 - [main●] » clang use-after-free.c -fsanitize=address -o use-after-free-asan
ch05_debugger_tracer/46_sanitizer_1 - [main●] » ./use-after-free-asan
=================================================================
==1889==ERROR: AddressSanitizer: heap-use-after-free on address 0x502000000010 at pc 0x555c48c317da bp 0x7ffce0855120 sp 0x7ffce0855118
READ of size 4 at 0x502000000010 thread T0
    #0 0x555c48c317d9 in main (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0x1047d9) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03)
    #1 0x7fed407e81c9 in __libc_start_call_main csu/../sysdeps/nptl/libc_start_call_main.h:58:16
    #2 0x7fed407e828a in __libc_start_main csu/../csu/libc-start.c:360:3
    #3 0x555c48b58344 in _start (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0x2b344) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03)

0x502000000010 is located 0 bytes inside of 4-byte region [0x502000000010,0x502000000014)
freed by thread T0 here:
    #0 0x555c48bf2efa in free (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0xc5efa) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03)
    #1 0x555c48c3179c in main (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0x10479c) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03)
    #2 0x7fed407e81c9 in __libc_start_call_main csu/../sysdeps/nptl/libc_start_call_main.h:58:16
    #3 0x7fed407e828a in __libc_start_main csu/../csu/libc-start.c:360:3
    #4 0x555c48b58344 in _start (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0x2b344) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03)

previously allocated by thread T0 here:
    #0 0x555c48bf3193 in malloc (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0xc6193) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03)
    #1 0x555c48c31748 in main (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0x104748) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03)
    #2 0x7fed407e81c9 in __libc_start_call_main csu/../sysdeps/nptl/libc_start_call_main.h:58:16
    #3 0x7fed407e828a in __libc_start_main csu/../csu/libc-start.c:360:3
    #4 0x555c48b58344 in _start (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0x2b344) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03)

SUMMARY: AddressSanitizer: heap-use-after-free (/usr/src/binary-hacks-rebooted/ch05_debugger_tracer/46_sanitizer_1/use-after-free-asan+0x1047d9) (BuildId: daceaf13cd1f999ad4c3563dbbbb43c37228dc03) in main
Shadow bytes around the buggy address:
  0x501ffffffd80: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
  0x501ffffffe00: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
  0x501ffffffe80: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
  0x501fffffff00: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
  0x501fffffff80: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
=>0x502000000000: fa fa[fd]fa fa fa fa fa fa fa fa fa fa fa fa fa
  0x502000000080: fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa
  0x502000000100: fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa
  0x502000000180: fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa
  0x502000000200: fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa
  0x502000000280: fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa fa
Shadow byte legend (one shadow byte represents 8 application bytes):
  Addressable:           00
  Partially addressable: 01 02 03 04 05 06 07
  Heap left redzone:       fa
  Freed heap region:       fd
  Stack left redzone:      f1
  Stack mid redzone:       f2
  Stack right redzone:     f3
  Stack after return:      f5
  Stack use after scope:   f8
  Global redzone:          f9
  Global init order:       f6
  Poisoned by user:        f7
  Container overflow:      fc
  Array cookie:            ac
  Intra object redzone:    bb
  ASan internal:           fe
  Left alloca redzone:     ca
  Right alloca redzone:    cb
==1889==ABORTING
```

libc_start_main

```
ch05_debugger_tracer/46_sanitizer_1 - [main●] » clang integer-overflow.c -fsanitize=undefined -o integer-overflow-ubsan.bin
ch05_debugger_tracer/46_sanitizer_1 - [main●] » ./integer-overflow-ubsan.bin
a = 2147483647
argc = 1
integer-overflow.c:10:17: runtime error: signed integer overflow: 2147483647 + 1 cannot be represented in type 'int'
SUMMARY: UndefinedBehaviorSanitizer: undefined-behavior integer-overflow.c:10:17
a + argc = -2147483648
```

- Rust の場合、上記のサニタイズが検出する問題をコンパイル時または言語レベルで補償している
- Valgring:JIT コンパイルを用いたメモリでバッギングツール。実行時にコードを挿入するのが特徴。

## Linux パフォーマンスことはじめ

次の通り設定すると、非特権ユーザーでほぼすべてのイベントが観測できる

```
 echo -1 | sudo tee /proc/sys/kernel/perf_event_paranoid
```

## seccomp でプロセスの使えるシステムコールを制限する

- seccomp は Linux カーネルが提供するセキュリティ機能で、ユーザープロセスが実行するシステムコールをフィルタリングする手段を提供します

```
ch06_security/53_seccomp - [main●] » sudo apt install libseccomp-dev
ch06_security/53_seccomp - [main●] » cat good_write.c
#include <string.h>
#include <unistd.h>
void untrusted_func(void) {
  char msg[] = "Hello!\n";
  write(1, msg, strlen(msg));
}
ch06_security/53_seccomp - [main●] » gcc good_write.c sandbox.c -o good_write -l seccomp
ch06_security/53_seccomp - [main●] » ./good_write
Hello!
ch06_security/53_seccomp - [main●] » cat bad_open.c
#include <fcntl.h>
void untrusted_func(void) { int fd = open("./secret_token.txt", O_RDONLY); }
ch06_security/53_seccomp - [main●] » gcc bad_open.c sandbox.c -o bad_open -l seccomp
ch06_security/53_seccomp - [main●] » ./bad_open
[1]    1248 invalid system call (core dumped)  ./bad_open
```

## Landlock で非特権プロセスサンドボックスを作る

- Linux5.3 から、landbox という Stackable LSM ベースの新しいサンドボックス機能が加わった
- Landlock は seccomp 同様に非 root ユーザーで利用可能なサンドボックス API

* 制限したいリソースの粒度でルールを設定できる(特定のファイルの書き込み権限のみ等)
* 注意点として制限できないシステムコールがある(chmode,chown 等)

## Clang CFI によって不正な制御フローを検知する

- Control Flow Integrity(CFI)とは制御フローを不正に操作する種の攻撃に対し、不正な操作を検知・無効かする防御機構を指します。

* Asan は開発時、CFI は実運用での利用を想定

```
ch06_security/58_clang_cfi - [main●] » clang -flto -fvisibility=hidden -fsanitize=cfi -fno-sanitize-trap=cfi func_ptr.c -o func_ptr
ch06_security/58_clang_cfi - [main●] » ./func_ptr
int_int_1 = 0x55b8e5406910
int_int_2 = 0x55b8e5406918
 char_int = 0x55b8e5406830

Input ptr: 0x55b8e5406910
int_int_1
ch06_security/58_clang_cfi - [main●] » ./func_ptr
int_int_1 = 0x5566276d9910
int_int_2 = 0x5566276d9918
 char_int = 0x5566276d9830

Input ptr: 0x5566276d9830
func_ptr.c:27:3: runtime error: control flow integrity check for type 'int (int)' failed during indirect function call
(/usr/src/binary-hacks-rebooted/ch06_security/58_clang_cfi/func_ptr+0x2f830): note: f_char_int defined here
SUMMARY: UndefinedBehaviorSanitizer: undefined-behavior func_ptr.c:27:3
ch06_security/58_clang_cfi - [main●] » ./func_ptr
int_int_1 = 0x564d77bd4910
int_int_2 = 0x564d77bd4918
 char_int = 0x564d77bd4830

Input ptr: 1
func_ptr.c:27:3: runtime error: control flow integrity check for type 'int (int)' failed during indirect function call
0x000000000001: note: (unknown) defined here
func_ptr.c:27:3: note: check failed in /usr/src/binary-hacks-rebooted/ch06_security/58_clang_cfi/func_ptr, destination function located in (unknown)
SUMMARY: UndefinedBehaviorSanitizer: undefined-behavior func_ptr.c:27:3
```

- オプション -fsanitize-cfi-cross-dso を付与すると、検査すべきアドレスが、共有ライブラリなどの別ファイルに存在している場合でも、検査を行います

## ファジングの概要と分類

ファジング：ランダムテストを発展させたソフトウェアのテスト手法。

- ブラックボックスファジング：プログラムの内部状態を取得せず、ブラックボックスとして扱う手法。ランダムテストに最も近い手法。
- ホワイトボックスファジング：シンボリック実行を用いて、プログラムの振る舞いを SMT 上の制約式として表し、それを解くことによって特定の実行パスを通る入力の生成を試みる

* グレイボックスファジング：コンパイル時の計装やエミュレータ、DBI などにより実行時のプログラムの内部状態をフィードバックとして取得できるようにする

## Row Hammer:DRAM の脆弱性に対する攻撃手法

- RAW HAMMER とは、メモリ上のビットを不正に反転させる攻撃手法。特定の手順でメモリ読み込み命令を連続実行すると、メモリの近隣アドレスの別のビットが反転する。
  特定の row に対して連続してデータ読み込みを行うと、電圧が短時間に上下を繰り返し、その結果、近隣のビットが電磁的な影響を受けて反転する。
- RAW HAMMER の対策として TRR という緩和策がある、Raw Hammer の疑いがあるアクセスパターンを検知した場合に影響を受ける row のメモリを書き直す
- TRRRespass は、TPP を回避して攻撃可能な手法。完全な解決には至っていない。

## CPU の脆弱性に対する攻撃手法

- Meltdown：OutOfOrder 実行とキャッシュの観察を組み合わせてユーザー空間からカーネル空間のメモリ内容を推定する攻撃手法
- Spectre:投機的実行を利用して情報を漏洩させる攻撃手法の総称

## LD_PRELOAD を使ってメモリアロケータを入れ替える

- TCMalloc:Google が中心になって開発している。マルチスレッド環境下で性能が良い。メモリ使用量のプロファイル機能が付いている。
- mimalloc:MicroSoft が中心になって開発している。関数型プログラミング koka や lean のランタイムシステムで使うために開発。
- jemalloc:FreeBSD の libc で使われている。メモリのフラグメンテーションを避けることを主眼に開発。

LD_PRELOAD を使ってメモリアロケータを入れ替えることが出来る。これによりパフォーマンスが大きく改善する場合がある。

## ABI と呼び出し規約を理解する

- 異なるシステムが連携するためのバイナリに関する決まり事を ABI(Application Binary Interface)と呼ぶ。別々にコンパイルしたファイルをリンクしたときに正常に動作するのは、ABI が一貫しているおかげ。
