# Windows に gitbucket をインストール

gitbucket は git の web プラットフォームです。  
同じような web サービスとして、もっとも有名なのは github です。  
ただ、社の方針としてイントラネット運用が求められるケースもあり、
そんな時はイントラネットで運用可能な gitbucket や gitlab が選ばれます。

※  
 こちらの記事を元にインストールを行ってみます。  
 https://programming-style.com/blog/gitbucket-install-win/  
 元記事の方が分かりやすいです  
 tomcat の設定は、元記事から少し修正が必要。

## ■Apache 導入

http サーバーをインストールします。

- 1.次のページからインストーラーをダウンロード
  https://www.apachelounge.com/download/

- 2.Apache のインストール  
  ダウンロードファイルを解凍すると、Apache24 という名前のフォルダがあります。これを C ドライブの直下に配置します。

* 3.環境変数の Path に C:\Apache24\bin を追加します。

* 4.コマンドプロンプトから apache のバージョン確認

```
httpd -v
Server version: Apache/2.4.54 (Win64)
Apache Lounge VS16 Server built:   Jun 22 2022 09:58:15
```

- 5.設定の書き換え  
  C:\Apache24\conf\httpd.conf を編集します  
  [変更前]

```
#ServerName www.example.com:80
```

[変更後]

```
ServerName localhost:80
```

- 5.設定ファイルの文法チェック

```
httpd -t
Syntax OK
```

## ■Tomcat 導入

gitbucket は JVM 上で動作するので、サーブレットコンテナである Tomcat をインストールします

- 1.次のページからインストーラーをダウンロード  
  https://tomcat.apache.org/download-90.cgi

* 2.Tomcat のインストール  
  ダウンロードファイルを解凍すると、apache-tomcat-9.0.68 という名前のフォルダがあります。これを C ドライブの直下に配置します。

* 3.jre のインストール(デフォルトのインストール先で OK)  
  https://www.oracle.com/java/technologies/downloads/#jdk19-windows

* 3.環境変数の追加  
  CATALINA_HOME・・・C:\apache-tomcat-9.0.68  
  JRE_HOME・・・C:\Program Files\Java\jdk-19\

* 4.Tomcat の起動と停止

```
C:\apache-tomcat-9.0.68\bin\startup.bat
Using CATALINA_BASE:   "C:\apache-tomcat-9.0.68"
Using CATALINA_HOME:   "C:\apache-tomcat-9.0.68"
Using CATALINA_TMPDIR: "C:\apache-tomcat-9.0.68\temp"
Using JRE_HOME:        "C:\Program Files\Java\jdk-19\"
Using CLASSPATH:       "C:\apache-tomcat-9.0.68\bin\bootstrap.jar;C:\apache-tomcat-9.0.68\bin\tomcat-juli.jar"
Using CATALINA_OPTS:   ""


C:\Users\user>C:\apache-tomcat-9.0.68\bin\shutdown.bat
Using CATALINA_BASE:   "C:\apache-tomcat-9.0.68"
Using CATALINA_HOME:   "C:\apache-tomcat-9.0.68"
Using CATALINA_TMPDIR: "C:\apache-tomcat-9.0.68\temp"
Using JRE_HOME:        "C:\Program Files\Java\jdk-19\"
Using CLASSPATH:       "C:\apache-tomcat-9.0.68\bin\bootstrap.jar;C:\apache-tomcat-9.0.68\bin\tomcat-juli.jar"
Using CATALINA_OPTS:   ""
NOTE: Picked up JDK_JAVA_OPTIONS:  --add-opens=java.base/java.lang=ALL-UNNAMED --add-opens=java.base/java.io=ALL-UNNAMED --add-opens=java.base/java.util=ALL-UNNAMED --add-opens=java.base/java.util.concurrent=ALL-UNNAMED --add-opens=java.rmi/sun.rmi.transport=ALL-UNNAMED
```

## ■gitbucket 導入

- 1.次のページからインストーラーをダウンロード(gitbucket.war
  )  
   https://github.com/gitbucket/gitbucket/releases

* 2.gitbucket のインストール  
  ダウンロードファイルを好きな場所に格納します。

* 3.gitbucket を起動します

```
java -jar C:\gitbucket\gitbucket.war
2022-10-25 06:05:15.763:INFO::main: Logging initialized @247ms to org.eclipse.jetty.util.log.StdErrLog
2022-10-25 06:05:15.905:WARN:oejsh.ContextHandler:main: Empty contextPath
・・・
2022-10-25 06:05:32.881:INFO:oejsh.ContextHandler:main: Started o.e.j.w.WebAppContext@3f5dfe69{/,file:///C:/Users/user/.gitbucket/tmp/webapp/,AVAILABLE}{file:/C:/gitbucket/gitbucket.war}
2022-10-25 06:05:32.974:INFO:oejs.AbstractConnector:main: Started ServerConnector@1bc6a36e{HTTP/1.1, (http/1.1)}{0.0.0.0:8080}
2022-10-25 06:05:32.975:INFO:oejs.Server:main: Started @17493ms
```

- 4.ブラウザから次の url にアクセス  
  http://localhost:8080

- 5.Signin をクリックして、Username,Password に root と入力してログイン

![localhost_8080_signin_redirect=%2F](https://user-images.githubusercontent.com/49807271/197631007-cc626c43-aebc-4fd2-8ee8-398502aa04a2.png)

- 5.gitbucket を実行しているコマンドプロンプトを閉じて、gitbucket を終了します

## ■gitbucket を Tomcat 上で起動する

- 1.Tomcat を起動

```
C:\apache-tomcat-9.0.68\bin\startup.bat
```

- 2.C:\apache-tomcat-9.0.68\webapps 配下に gitbucket.war を格納
  少し待つと C:\apache-tomcat-9.0.68\webapps\gitbucket フォルダが作成されます

- 3.次の URL にアクセスします  
   http://localhost:8080/gitbucket/

## ■apache をプロキシサーバーにする

- 1.C:\Apache24\conf\httpd.conf を編集します

[変更前]

```conf
#LoadModule proxy_module modules/mod_proxy.so
#LoadModule proxy_ajp_module modules/mod_proxy_ajp.so
```

[変更後]

```conf
LoadModule proxy_module modules/mod_proxy.so
LoadModule proxy_ajp_module modules/mod_proxy_ajp.so
・・・
# 末尾
# Configure proxy server of gitbucket
<IfModule proxy_module>
<IfModule proxy_ajp_module>
  AllowEncodedSlashes NoDecode
  ProxyPreserveHost On
  ProxyPass /gitbucket ajp://localhost:8009/gitbucket
  ProxyPassReverse /gitbucket ajp://localhost:8009/gitbucket
</IfModule>
</IfModule>
```

- 2.Tomecat の web サーバー機能を無効化します  
  C:\apache-tomcat-9.0.68\conf\server.xml を編集します

  [変更前]

  ```xml
  <Connector port="8080" protocol="HTTP/1.1"
               connectionTimeout="20000"
               redirectPort="8443" />

  ・・・
  <Connector protocol="AJP/1.3"
               address="::1"
               port="8009"
               redirectPort="8443" />
  ```

  [変更後]

  ```xml
  <!--<Connector port="8080" protocol="HTTP/1.1"
               connectionTimeout="20000"
               redirectPort="8443" />-->
    <Connector protocol="AJP/1.3"
               address="::1"
               port="8009"
               redirectPort="8443" secretRequired="false"/>
  ```

* 3.apache を起動します

```
httpd

```

- 4.tomcat を起動します

```
C:\apache-tomcat-9.0.68\bin\startup.bat
Using CATALINA_BASE:   "C:\apache-tomcat-9.0.68"
Using CATALINA_HOME:   "C:\apache-tomcat-9.0.68"
Using CATALINA_TMPDIR: "C:\apache-tomcat-9.0.68\temp"
Using JRE_HOME:        "C:\Program Files\Java\jdk-19\"
Using CLASSPATH:       "C:\apache-tomcat-9.0.68\bin\bootstrap.jar;C:\apache-tomcat-9.0.68\bin\tomcat-juli.jar"
```

- 5.次の URL にアクセスします。  
  http://localhost/gitbucket/

・・・

## ■gitbucket で使用する DB を H2 から postgres に変更

- 1.postgreSQL をインストール  
  https://www.postgresql.org/download/windows/

- 2.gitbucket 用のアカウントを追加

```sql
psql -U postgres
ユーザー postgres のパスワード:
psql (14.5)
"help"でヘルプを表示します。

postgres=# create role gitbucket with login password 'gitbucket';
CREATE ROLE
```

- 3.gitbucket 用の DB 作成

```sql
create database gitbucket encoding 'UTF-8';
```

- 4.gitbucket の設定編集
  設定ファイルの場所は最初に gitbucket を実行したユーザーによって変わる場合があります。

Admin

```
C:\Windows\System32\config\systemprofile\.gitbucket\database.conf
```

User

```

```

[変更前]

```conf
db {
  url = "jdbc:h2:${DatabaseHome};MVCC=true"
  user = "sa"
  password = "sa"
#  connectionTimeout = 30000
#  idleTimeout = 600000
#  maxLifetime = 1800000
#  minimumIdle = 10
#  maximumPoolSize = 10
}
```

[変更後]

```conf
db {
  url = "jdbc:postgresql://localhost:5432/gitbucket"
  user = "gitbucket"
  password = "gitbucket"
#  connectionTimeout = 30000
#  idleTimeout = 600000
#  maxLifetime = 1800000
#  minimumIdle = 10
#  maximumPoolSize = 10
}

```

- 5.tomcat を再起動します

- 6.gitbucket にアクセスして、HelloWorld という名前のリポジトリを作成します  
  ![localhost_8080_gitbucket_root_HelloWorld](https://user-images.githubusercontent.com/49807271/198031023-a2f31fc8-b35f-493b-97bd-39256e68843a.png)

- 7.psql で postgres にデータが登録できているか確認します

```sql
C:\Windows\system32>psql -U gitbucket -d gitbucket
ユーザー gitbucket のパスワード:
psql (14.5)
"help"でヘルプを表示します。

gitbucket=> table repository;
 user_name | repository_name | private | description | default_branch |     registered_date     |      updated_date       |   last_activity_date    | origin_user_name | origin_repository_name | parent_user_name | parent_repository_name | external_issues_url | external_wiki_url | allow_fork | wiki_option | issues_option |       merge_options        | default_merge_option | safe_mode
-----------+-----------------+---------+-------------+----------------+-------------------------+-------------------------+-------------------------+------------------+------------------------+------------------+------------------------+---------------------+-------------------+------------+-------------+---------------+----------------------------+----------------------+-----------
 root      | HelloWorld      | f       |             | master         | 2022-10-26 21:49:36.824 | 2022-10-26 21:49:36.824 | 2022-10-26 21:49:36.824 |                  |                        |                  |                        |                     |                   | t          | PUBLIC      | PUBLIC        | merge-commit,squash,rebase | merge-commit         | t
(1 行)
```

※後から管理者アカウントのページから DB テーブル参照できることに気づきました
