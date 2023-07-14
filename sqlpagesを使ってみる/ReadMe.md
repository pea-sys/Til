# SqlPages を使ってみる

sqlPages は SQL の記述のみで Web サイトを構築するツールです。
簡易的なダッシュボードを高速に構築するのことに適しています。

https://sql.ophir.dev/get%20started.sql

- 以下のページから SqlPages のサーバをダウンロードします。
  https://github.com/lovasoa/SQLpage/releases

* Web サイトのルートフォルダーを作成し、そこにサーバープログラムを配置して起動します

```
[2023-07-12T22:21:35Z INFO  sqlpage::app_config] No DATABASE_URL, using the default sqlite database './sqlpage.db'
[2023-07-12T22:21:35Z INFO  sqlpage::webserver::database] Connecting to database: sqlite://sqlpage.db
[2023-07-12T22:21:35Z INFO  sqlpage::webserver::database] Not applying database migrations because 'sqlpage/migrations' does not exist
[2023-07-12T22:21:35Z INFO  sqlpage] Starting server on 0.0.0.0:8080
[2023-07-12T22:21:35Z INFO  actix_server::builder] starting 2 workers
[2023-07-12T22:21:35Z INFO  actix_server::server] Actix runtime found; starting in Actix runtime
```

- Web サイトのルートフォルダに index.sql ファイルを作成

```sql
SELECT 'list' AS component, 'Popular websites' AS title;

SELECT 'Hello' AS title, 'world' AS description, 'https://wikipedia.org' AS link;
```

-　 http://localhost:8080/ にアクセス

![page1](https://github.com/pea-sys/Til/assets/49807271/7f2bbd01-74b5-4b68-9c46-46a59f3a7d04)

ちょいと触った感想としては、発想は面白いけど、導入メリットは浮かばなかった。
