# Embulk を使ってみる

こちらを参考に進めていきます。  
https://qiita.com/da-sugi/items/13d4ead19c86d783ebb4

- 1.CentOS7 box の追加

```
C:\Users\user>vagrant box add bento/centos-7.7 --provider virtualbox
==> box: Loading metadata for box 'bento/centos-7.7'
    box: URL: https://vagrantcloud.com/bento/centos-7.7
==> box: Adding box 'bento/centos-7.7' (v202005.12.0) for provider: virtualbox
    box: Downloading: https://vagrantcloud.com/bento/boxes/centos-7.7/versions/202005.12.0/providers/virtualbox.box
    box:
==> box: Successfully added box 'bento/centos-7.7' (v202005.12.0) for 'virtualbox'!
```

- 2.仮想マシンのセットアップと SSH 接続

```
vagrant up
```

- 3.SSH 接続

```
vagrant ssh
```

- 4.Embulk に java8 が必要なのでインストール

```
sudo yum install -y java-1.8.0-openjdk
```

- 5.java のバージョン確認

```
[vagrant@localhost ~]$ java -version
openjdk version "1.8.0_352"
OpenJDK Runtime Environment (build 1.8.0_352-b08)
OpenJDK 64-Bit Server VM (build 25.352-b08, mixed mode)
```

- 6.Embulk インストール ＆ パスを通す

```
[vagrant@localhost ~]$ chmod +x ~/.embulk/bin/embulk
[vagrant@localhost ~]$ echo 'export PATH="$HOME/.embulk/bin:$PATH"' >> ~/.bashrc
[vagrant@localhost ~]$ source ~/.bashrc

```

- 7.バージョン確認

```
[vagrant@localhost ~]$ embulk --version
embulk 0.9.24
```

- 8.mkbundle で gem を管理できるようにする  
  gem とは Ruby のパッケージ管理システムです

```
[vagrant@localhost ~]$ embulk mkbundle bundle_dir
2022-11-12 04:06:15.438 +0000: Embulk v0.9.24
Initializing bundle_dir...
  Creating Gemfile
  Creating .bundle/config
  Creating embulk/input/example.rb
  Creating embulk/output/example.rb
  Creating embulk/filter/example.rb
```

- 9.外部プラグインを追加するため、gem ファイルを編集

```
source 'https://rubygems.org/'
gem 'embulk'
+ gem 'embulk-filter-eval'
```

- 10.プラグインをインストール

```
 embulk bundle
2022-11-12 05:04:37.040 +0000: Embulk v0.9.24
Fetching gem metadata from https://rubygems.org/........
Fetching gem metadata from https://rubygems.org/.
Resolving dependencies.....
Using bundler 1.16.0
Fetching msgpack 1.4.1 (java)
Installing msgpack 1.4.1 (java)
Fetching embulk 0.10.37 (java)
Installing embulk 0.10.37 (java)
Fetching embulk-filter-eval 0.1.0
Installing embulk-filter-eval 0.1.0
Bundle complete! 2 Gemfile dependencies, 4 gems now installed.
Bundled gems are installed into `/home/vagrant/bundle_dir`
```

- 11.サンプルファイル作成

```
[vagrant@localhost ~]$ embulk example embulk-sample
2022-11-12 05:10:29.685 +0000: Embulk v0.9.24
Creating embulk-sample directory...
  Creating embulk-sample/
  Creating embulk-sample/csv/
  Creating embulk-sample/csv/sample_01.csv.gz
  Creating embulk-sample/seed.yml

Run following subcommands to try embulk:

   1. embulk guess embulk-sample/seed.yml -o config.yml
   2. embulk preview config.yml
   3. embulk run config.yml
```

- 12.設定ファイルの雛形を作成  
  元となる CSV があれば、embulk guess コマンドで大体の設定を行ったファイルを作成してくれる

```
[vagrant@localhost ~]$ embulk guess ./embulk-sample/seed.yml -o config.yml
2022-11-12 05:12:31.647 +0000: Embulk v0.9.24
2022-11-12 05:12:33.330 +0000 [WARN] (main): DEPRECATION: JRuby org.jruby.embed.ScriptingContainer is directly injected.
2022-11-12 05:12:37.552 +0000 [INFO] (main): Gem's home and path are set by default: "/home/vagrant/.embulk/lib/gems"
2022-11-12 05:12:39.351 +0000 [INFO] (main): Started Embulk v0.9.24
2022-11-12 05:12:39.661 +0000 [INFO] (0001:guess): Listing local files at directory '/home/vagrant/embulk-sample/csv' filtering filename by prefix 'sample_'
2022-11-12 05:12:39.662 +0000 [INFO] (0001:guess): "follow_symlinks" is set false. Note that symbolic links to directories are skipped.
2022-11-12 05:12:39.663 +0000 [INFO] (0001:guess): Loading files [/home/vagrant/embulk-sample/csv/sample_01.csv.gz]
2022-11-12 05:12:39.687 +0000 [INFO] (0001:guess): Try to read 32,768 bytes from input source
2022-11-12 05:12:39.846 +0000 [INFO] (0001:guess): Loaded plugin embulk (0.9.24)
2022-11-12 05:12:39.890 +0000 [INFO] (0001:guess): Loaded plugin embulk (0.9.24)
2022-11-12 05:12:39.953 +0000 [INFO] (0001:guess): Loaded plugin embulk (0.9.24)
2022-11-12 05:12:39.981 +0000 [INFO] (0001:guess): Loaded plugin embulk (0.9.24)
in:
  type: file
  path_prefix: /home/vagrant/embulk-sample/csv/sample_
  decoders:
  - {type: gzip}
  parser:
    charset: UTF-8
    newline: LF
    type: csv
    delimiter: ','
    quote: '"'
    escape: '"'
    null_string: 'NULL'
    trim_if_not_quoted: false
    skip_header_lines: 1
    allow_extra_columns: false
    allow_optional_columns: false
    columns:
    - {name: id, type: long}
    - {name: account, type: long}
    - {name: time, type: timestamp, format: '%Y-%m-%d %H:%M:%S'}
    - {name: purchase, type: timestamp, format: '%Y%m%d'}
    - {name: comment, type: string}
out: {type: stdout}

Created 'config.yml' file.
```

- 13.実行する(out: {type: stdout}となっているので標準出力するだけ)

```
[vagrant@localhost ~]$ embulk run config.yml
OpenJDK 64-Bit Server VM warning: If the number of processors is expected to increase from one, then you should configure the number of parallel GC threads appropriately using -XX:ParallelGCThreads=N
2022-11-12 05:14:59.477 +0000: Embulk v0.9.24
2022-11-12 05:15:02.337 +0000 [WARN] (main): DEPRECATION: JRuby org.jruby.embed.ScriptingContainer is directly injected.
2022-11-12 05:15:11.659 +0000 [INFO] (main): Gem's home and path are set by default: "/home/vagrant/.embulk/lib/gems"
2022-11-12 05:15:16.031 +0000 [INFO] (main): Started Embulk v0.9.24
2022-11-12 05:15:16.558 +0000 [INFO] (0001:transaction): Listing local files at directory '/home/vagrant/embulk-sample/csv' filtering filename by prefix 'sample_'
2022-11-12 05:15:16.560 +0000 [INFO] (0001:transaction): "follow_symlinks" is set false. Note that symbolic links to directories are skipped.
2022-11-12 05:15:16.564 +0000 [INFO] (0001:transaction): Loading files [/home/vagrant/embulk-sample/csv/sample_01.csv.gz]
2022-11-12 05:15:16.827 +0000 [INFO] (0001:transaction): Using local thread executor with max_threads=2 / tasks=1
2022-11-12 05:15:16.868 +0000 [INFO] (0001:transaction): {done:  0 / 1, running: 0}
1,32864,2015-01-27 19:23:49,20150127,embulk
2,14824,2015-01-27 19:01:23,20150127,embulk jruby
3,27559,2015-01-28 02:20:02,20150128,Embulk "csv" parser plugin
4,11270,2015-01-29 11:54:36,20150129,
2022-11-12 05:15:17.692 +0000 [INFO] (0001:transaction): {done:  1 / 1, running: 0}
2022-11-12 05:15:17.761 +0000 [INFO] (main): Committed.
2022-11-12 05:15:17.771 +0000 [INFO] (main): Next config diff: {"in":{"last_path":"/home/vagrant/embulk-sample/csv/sample_01.csv.gz"},"out":{}}
```

- 14.ファイルの中身と上記出力の一致確認

```
[vagrant@localhost csv]$ zcat sample_01.csv.gz
id,account,time,purchase,comment
1,32864,2015-01-27 19:23:49,20150127,embulk
2,14824,2015-01-27 19:01:23,20150127,embulk jruby
3,27559,2015-01-28 02:20:02,20150128,"Embulk ""csv"" parser plugin"
4,11270,2015-01-29 11:54:36,20150129,NULL
```

[参考]

- https://kageura.hatenadiary.jp/entry/embulk1
