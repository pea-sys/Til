# DuckDB を使ってみる

DuckDB はインプロセス SQL OLAP データベースであり、これを使用するアプリケーションと同じプロセス内で実行されます。  
DuckDB のデータインポート・エクスポートを試します。

https://duckdb.org/docs/archive/0.9.2/

[環境]

- Ubuntu=22.04
- DuckDB=0.9.2
- MySQL=8.0.36-0
- PostgreSQL=14.10
- SQLite=3.37.2

### ダウンロード

```sh
root@masami-L:/usr/bin# wget https://github.com/duckdb/duckdb/releases/download/v0.9.2/duckdb_cli-linux-amd64.zip
root@masami-L:/usr/bin# unzip duckdb_cli-linux-amd64.zip
root@masami-L:/usr/bin# rm  duckdb_cli-linux-amd64.zip
```

### 起動とヘルプ

```sh
root@masami-L:/usr/bin# ./duckdb
v0.9.2 3c695d7ba9
Enter ".help" for usage hints.
Connected to a transient in-memory database.
Use ".open FILENAME" to reopen on a persistent database.
D
```

説明にある通り、デフォルトではインメモリなのでデータ永続化はされません。  
FILENAME オプションを渡すと永続化されます。

### CSV インポート

適当な csv ファイルを作成します

```
root@masami-L:/usr/bin# cat input.csv
id,name
1,Tom Mitch
2,Nick Cave
3,Destroyer
4,Thunder Cat
5,S.Carey
6,Underworld
7,TheoParrish
8,DaftPunk
9,Kings of Convinience
10,Bon Iver
```

csv ファイル読取

```
root@masami-L:/usr/bin# ./duckdb
v0.9.2 3c695d7ba9
Enter ".help" for usage hints.
Connected to a transient in-memory database.
Use ".open FILENAME" to reopen on a persistent database.
D SELECT * FROM read_csv('input.csv',AUTO_DETECT=TRUE);
┌───────┬──────────────────────┐
│  id   │         name         │
│ int64 │       varchar        │
├───────┼──────────────────────┤
│     1 │ Tom Mitch            │
│     2 │ Nick Cave            │
│     3 │ Destroyer            │
│     4 │ Thunder Cat          │
│     5 │ S.Carey              │
│     6 │ Underworld           │
│     7 │ TheoParrish          │
│     8 │ DaftPunk             │
│     9 │ Kings of Convinience │
│    10 │ Bon Iver             │
├───────┴──────────────────────┤
│ 10 rows            2 columns │
└──────────────────────────────┘
D
```

csv ファイルから新しいテーブルを作成

```
D CREATE TABLE new_tbl AS SELECT * FROM read_csv('input.csv',AUTO_DETECT=TRUE);
```

既存テーブルに csv を取り込む

```
D INSERT INTO new_tbl SELECT * FROM read_csv('input.csv', AUTO_DETECT=TRUE);
```

### CSV エクスポート

テーブルから csv ファイルをエクスポート

```
D COPY new_tbl TO 'output.csv' (HEADER, DELIMITER ',');
D SELECT * FROM read_csv('output.csv',AUTO_DETECT=TRUE);
┌───────┬──────────────────────┐
│  id   │         name         │
│ int64 │       varchar        │
├───────┼──────────────────────┤
│     1 │ Tom Mitch            │
│     2 │ Nick Cave            │
│     3 │ Destroyer            │
│     4 │ Thunder Cat          │
│     5 │ S.Carey              │
│     6 │ Underworld           │
│     7 │ TheoParrish          │
│     8 │ DaftPunk             │
│     9 │ Kings of Convinience │
│    10 │ Bon Iver             │
│     1 │ Tom Mitch            │
│     2 │ Nick Cave            │
│     3 │ Destroyer            │
│     4 │ Thunder Cat          │
│     5 │ S.Carey              │
│     6 │ Underworld           │
│     7 │ TheoParrish          │
│     8 │ DaftPunk             │
│     9 │ Kings of Convinience │
│    10 │ Bon Iver             │
├───────┴──────────────────────┤
│ 20 rows            2 columns │
```

### Parquet のインポート

tmp/load に parquet をダウンロードします

```
root@masami-L:/tmp/load# wget https://docs.snowflake.com/ja/_downloads/0c1e6c4f4140561029eeb20afdd02664/cities.parquet
--2024-02-05 21:35:51--  https://docs.snowflake.com/ja/_downloads/0c1e6c4f4140561029eeb20afdd02664/cities.parquet
docs.snowflake.com (docs.snowflake.com) をDNSに問いあわせています... 65.8.161.105, 65.8.161.115, 65.8.161.112, ...
docs.snowflake.com (docs.snowflake.com)|65.8.161.105|:443 に接続しています... 接続しました。
HTTP による接続要求を送信しました、応答を待っています... 200 OK
長さ: 866 [application/octet-stream]
‘cities.parquet’ に保存中
```

Parquet ファイルからデータを読み取る

```
root@masami-L:/usr/bin# ./duckdb
v0.9.2 3c695d7ba9
Enter ".help" for usage hints.
Connected to a transient in-memory database.
Use ".open FILENAME" to reopen on a persistent database.
D SELECT * FROM read_parquet('../../tmp/load/cities.parquet');
┌───────────────┬────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│   continent   │                                                                  country                                                                   │
│    varchar    │                                                   struct("name" varchar, city varchar[])                                                   │
├───────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ Europe        │ {'name': France, 'city': [Paris, Nice, Marseilles, Cannes]}                                                                                │
│ Europe        │ {'name': Greece, 'city': [Athens, Piraeus, Hania, Heraklion, Rethymnon, Fira]}                                                             │
│ North America │ {'name': Canada, 'city': [Toronto, Vancouver, St. John's, Saint John, Montreal, Halifax, Winnipeg, Calgary, Saskatoon, Ottawa, Yellowkni…  │
└───────────────┴────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

クエリの結果を使用して新しいテーブルを作成する

```
D CREATE TABLE new_tbl AS SELECT * FROM read_parquet('../../tmp/load/cities.parquet');
D SELECT * FROM new_tbl;
┌───────────────┬────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│   continent   │                                                                  country                                                                   │
│    varchar    │                                                   struct("name" varchar, city varchar[])                                                   │
├───────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ Europe        │ {'name': France, 'city': [Paris, Nice, Marseilles, Cannes]}                                                                                │
│ Europe        │ {'name': Greece, 'city': [Athens, Piraeus, Hania, Heraklion, Rethymnon, Fira]}                                                             │
│ North America │ {'name': Canada, 'city': [Toronto, Vancouver, St. John's, Saint John, Montreal, Halifax, Winnipeg, Calgary, Saskatoon, Ottawa, Yellowkni…  │
└───────────────┴────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

クエリから既存のテーブルにデータをロードする

```
D INSERT INTO new_tbl SELECT * FROM read_parquet('../../tmp/load/cities.parquet');
D SELECT * FROM new_tbl;
┌───────────────┬────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│   continent   │                                                                  country                                                                   │
│    varchar    │                                                   struct("name" varchar, city varchar[])                                                   │
├───────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ Europe        │ {'name': France, 'city': [Paris, Nice, Marseilles, Cannes]}                                                                                │
│ Europe        │ {'name': Greece, 'city': [Athens, Piraeus, Hania, Heraklion, Rethymnon, Fira]}                                                             │
│ North America │ {'name': Canada, 'city': [Toronto, Vancouver, St. John's, Saint John, Montreal, Halifax, Winnipeg, Calgary, Saskatoon, Ottawa, Yellowkni…  │
│ Europe        │ {'name': France, 'city': [Paris, Nice, Marseilles, Cannes]}                                                                                │
│ Europe        │ {'name': Greece, 'city': [Athens, Piraeus, Hania, Heraklion, Rethymnon, Fira]}                                                             │
│ North America │ {'name': Canada, 'city': [Toronto, Vancouver, St. John's, Saint John, Montreal, Halifax, Winnipeg, Calgary, Saskatoon, Ottawa, Yellowkni…  │
└───────────────┴────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
D
```

### Parquet のエクスポート

```
D COPY new_tbl TO 'output.parquet' (FORMAT PARQUET);
```

### パケットファイルのクエリ

```
D SELECT * FROM read_parquet('../../tmp/load/cities.parquet');
┌───────────────┬──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│   continent   │                                                           country                                                            │
│    varchar    │                                            struct("name" varchar, city varchar[])                                            │
├───────────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ Europe        │ {'name': France, 'city': [Paris, Nice, Marseilles, Cannes]}                                                                  │
│ Europe        │ {'name': Greece, 'city': [Athens, Piraeus, Hania, Heraklion, Rethymnon, Fira]}                                               │
│ North America │ {'name': Canada, 'city': [Toronto, Vancouver, St. John's, Saint John, Montreal, Halifax, Winnipeg, Calgary, Saskatoon, Ott…  │
└───────────────┴──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
D SELECT * FROM read_parquet('../../tmp/load/cities.parquet') WHERE continent = 'Europe';
┌───────────┬────────────────────────────────────────────────────────────────────────────────┐
│ continent │                                    country                                     │
│  varchar  │                     struct("name" varchar, city varchar[])                     │
├───────────┼────────────────────────────────────────────────────────────────────────────────┤
│ Europe    │ {'name': France, 'city': [Paris, Nice, Marseilles, Cannes]}                    │
│ Europe    │ {'name': Greece, 'city': [Athens, Piraeus, Hania, Heraklion, Rethymnon, Fira]} │
└───────────┴────────────────────────────────────────────────────────────────────────────────┘
```

HTTP(S) 経由で Parquet ファイルをロードするには、httpfs 拡張子が必要です

```
D INSTALL httpfs;
```

httpfs 拡張機能をロード

```
D LOAD httpfs;
```

http(s)で Parquet ファイルを読み取ることができます

```
D SELECT * FROM read_parquet('https://docs.snowflake.com/ja/_downloads/0c1e6c4f4140561029eeb20afdd02664/cities.parquet');
┌───────────────┬──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│   continent   │                                                           country                                                            │
│    varchar    │                                            struct("name" varchar, city varchar[])                                            │
├───────────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ Europe        │ {'name': France, 'city': [Paris, Nice, Marseilles, Cannes]}                                                                  │
│ Europe        │ {'name': Greece, 'city': [Athens, Piraeus, Hania, Heraklion, Rethymnon, Fira]}                                               │
│ North America │ {'name': Canada, 'city': [Toronto, Vancouver, St. John's, Saint John, Montreal, Halifax, Winnipeg, Calgary, Saskatoon, Ott…  │
└───────────────┴──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
D
```

### JSON のインポート

任意の json ファイルを作成

```json
{
  "name": "sample",
  "version": "1.0.0",
  "description": "sample",
  "main": "index.js",
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1"
  },
  "author": "",
  "license": "ISC"
}
```

JSON ファイルからデータを読み取る

```
D SELECT * FROM read_json_auto('../../tmp/load/package.json');
┌─────────┬─────────┬─────────────┬──────────┬─────────────────────────────────────────────────────┬─────────┬─────────┐
│  name   │ version │ description │   main   │                       scripts                       │ author  │ license │
│ varchar │ varchar │   varchar   │ varchar  │                struct(test varchar)                 │ varchar │ varchar │
├─────────┼─────────┼─────────────┼──────────┼─────────────────────────────────────────────────────┼─────────┼─────────┤
│ sample  │ 1.0.0   │ sample      │ index.js │ {'test': echo "Error: no test specified" && exit 1} │         │ ISC     │
└─────────┴─────────┴─────────────┴──────────┴─────────────────────────────────────────────────────┴─────────┴─────────┘
```

クエリの結果を使用して新しいテーブルを作成

```
D CREATE TABLE new_tbl AS SELECT * FROM read_json_auto('../../tmp/load/package.json');
```

JSON のエクスポート

```
D COPY new_tbl TO 'output.json';
D SELECT * FROM read_json_auto('output.json');
┌─────────┬─────────┬─────────────┬──────────┬─────────────────────────────────────────────────────┬─────────┬─────────┐
│  name   │ version │ description │   main   │                       scripts                       │ author  │ license │
│ varchar │ varchar │   varchar   │ varchar  │                struct(test varchar)                 │ varchar │ varchar │
├─────────┼─────────┼─────────────┼──────────┼─────────────────────────────────────────────────────┼─────────┼─────────┤
│ sample  │ 1.0.0   │ sample      │ index.js │ {'test': echo "Error: no test specified" && exit 1} │         │ ISC     │
└─────────┴─────────┴─────────────┴──────────┴─────────────────────────────────────────────────────┴─────────┴─────────┘
```

### Excel のインポート

拡張機能の準備

```
D INSTALL spatial;
D LOAD spatial;
```

任意の Excel ファイルをダウンロード

```
root@masami-L:/tmp/load# wget -O sample.xlsx https://note.com/api/v2/attachments/download/dc8995242302067e68efdb2af1e5d45c
--2024-02-06 20:32:38--  https://note.com/api/v2/attachments/download/dc8995242302067e68efdb2af1e5d45c
note.com (note.com) をDNSに問いあわせています... 18.172.52.90, 18.172.52.29, 18.172.52.34, ...
note.com (note.com)|18.172.52.90|:443 に接続しています... 接続しました。
HTTP による接続要求を送信しました、応答を待っています... 200 OK
長さ: 特定できません [application/octet-stream]
‘sample.xlsx’ に保存中

sample.xlsx                             [ <=>                                                                ] 199.18K  1.23MB/s    in 0.2s

2024-02-06 20:32:39 (1.23 MB/s) - ‘sample.xlsx’ へ保存終了 [203957]
```

Excel シートのインポート

```
D SELECT * FROM st_read('../../tmp/load/sample.xlsx');
┌──────────────────┬────────────┬──────────────┬──────────────┬───┬──────────────────────┬────────────────────┬─────────┬──────────────────┐
│      Field1      │   Field2   │    Field3    │    Field4    │ … │        Field6        │       Field7       │ Field8  │      Field9      │
│     varchar      │  varchar   │   varchar    │   varchar    │   │       varchar        │      varchar       │ varchar │     varchar      │
├──────────────────┼────────────┼──────────────┼──────────────┼───┼──────────────────────┼────────────────────┼─────────┼──────────────────┤
│ 登録日トウロクビ │ ユーザーID │ 名前ナマエ   │ 性別セイベツ │ … │ 利用金額リヨウキン…  │ 誕生日タンジョウビ │         │                  │
│ entrydate        │ userid     │ name         │ seibetsu     │ … │ totalmoney           │ birthday           │         │ 基準日キジュンビ │
│ 1905/07/12       │ OD77412    │ オソミハ     │ men          │ … │ 52000                │ 1905/06/10         │         │ 2020/08/05       │
│ 1905/07/11       │ QS19455    │ トムアシテキ │ men          │ … │ 59000                │ 1905/05/02         │         │                  │
│ 1905/07/11       │ HY60274    │ オンウ       │ men          │ … │ 71000                │ 1905/05/19         │         │                  │
│ 1905/07/12       │ TA49387    │ レコスソニ   │ men          │ … │ 63000                │ 1905/04/23         │         │                  │
│ 1905/07/11       │ PK56576    │ コミトハム   │ men          │ … │ 87000                │ 1905/04/22         │         │                  │
│ 1905/07/11       │ ZJ39522    │ ヘフテイキ   │ men          │ … │ 24000                │ 1905/06/09         │         │                  │
│ 1905/07/11       │ YW53474    │ ヤヲハラフ   │ men          │ … │ 18000                │ 1905/05/14         │         │                  │
│ 1905/07/12       │ YJ12191    │ セレヤヒル   │ women        │ … │ 45000                │ 1905/04/21         │         │                  │
│ 1905/07/12       │ FG18898    │ クワチシ     │ women        │ … │ 18000                │ 1905/05/08         │         │                  │
│ 1905/07/11       │ WC58285    │ ヌヨユノミン │ men          │ … │ 99000                │ 1905/05/22         │         │                  │
│ 1905/07/12       │ OG30413    │ ネテ         │ men          │ … │ 90000                │ 1905/05/30         │         │                  │
│ 1905/07/11       │ HQ81381    │ テソノ       │ men          │ … │ 62000                │ 1905/05/24         │         │                  │
│ 1905/07/12       │ MM62525    │ チフン       │ women        │ … │ 60000                │ 1905/05/25         │         │                  │
│ 1905/07/12       │ VT80396    │ ノヘフネニ   │ men          │ … │ 50000                │ 1905/05/23         │         │                  │
│ 1905/07/11       │ BT12011    │ イワヌヤウワ │ men          │ … │ 95000                │ 1905/05/12         │         │                  │
│ 1905/07/12       │ DG44874    │ ノヌヘ       │ men          │ … │ 91000                │ 1905/05/12         │         │                  │
│ 1905/07/11       │ NX23141    │ ノサスムン   │ men          │ … │ 6000                 │ 1905/05/22         │         │                  │
│ 1905/07/12       │ KY48305    │ テナロヒハ   │ men          │ … │ 98000                │ 1905/05/05         │         │                  │
│     ·            │    ·       │    ·         │  ·           │ · │   ·                  │     ·              │    ·    │        ·         │
│     ·            │    ·       │    ·         │  ·           │ · │   ·                  │     ·              │    ·    │        ·         │
│     ·            │    ·       │    ·         │  ·           │ · │   ·                  │     ·              │    ·    │        ·         │
│ 1905/07/12       │ XD25401    │ ユラハユ     │ men          │ … │ 93000                │ 1905/04/29         │         │                  │
│ 1905/07/11       │ GU76667    │ シウカアセ   │ women        │ … │ 56000                │ 1905/05/14         │         │                  │
│ 1905/07/12       │ BQ53705    │ マシ         │ women        │ … │ 82000                │ 1905/05/04         │         │                  │
│ 1905/07/11       │ FM33490    │ リキフウ     │ men          │ … │ 76000                │ 1905/06/01         │         │                  │
│ 1905/07/12       │ RV41434    │ ルンチヘナ   │ women        │ … │ 66000                │ 1905/05/24         │         │                  │
│ 1905/07/12       │ TG78218    │ ルミムモホヌ │ women        │ … │ 76000                │ 1905/05/27         │         │                  │
│ 1905/07/12       │ AT86514    │ ニツソレヌ   │ men          │ … │ 76000                │ 1905/05/13         │         │                  │
│ 1905/07/11       │ RP89813    │ ハニロノ     │ women        │ … │ 33000                │ 1905/05/09         │         │                  │
│ 1905/07/12       │ TR68946    │ ンヲミルチ   │ men          │ … │ 26000                │ 1905/06/08         │         │                  │
│ 1905/07/11       │ CJ79326    │ アオサソ     │ men          │ … │ 50000                │ 1905/05/15         │         │                  │
│ 1905/07/12       │ TO19429    │ セホソレト   │ men          │ … │ 50000                │ 1905/06/02         │         │                  │
│ 1905/07/12       │ EX79908    │ ナメチラタヒ │ men          │ … │ 75000                │ 1905/05/04         │         │                  │
│ 1905/07/11       │ VX95673    │ ヒフ         │ women        │ … │ 32000                │ 1905/04/26         │         │                  │
│ 1905/07/12       │ ZT79232    │ ソキサヌ     │ women        │ … │ 12000                │ 1905/05/31         │         │                  │
│ 1905/07/12       │ YP74864    │ タヌ         │ women        │ … │ 81000                │ 1905/05/01         │         │                  │
│ 1905/07/11       │ GF97839    │ ソハンヲ     │ women        │ … │ 20000                │ 1905/04/23         │         │                  │
│ 1905/07/11       │ CQ74870    │ ワエ         │ men          │ … │ 81000                │ 1905/05/07         │         │                  │
│ 1905/07/11       │ LC41081    │ イミオマチワ │ women        │ … │ 78000                │ 1905/05/14         │         │                  │
│ 1905/07/11       │ QL31417    │ ミノヒテホテ │ women        │ … │ 23000                │ 1905/05/07         │         │                  │
│ 1905/07/12       │ AU19486    │ モチシタロ   │ women        │ … │ 8000                 │ 1905/06/02         │         │                  │
├──────────────────┴────────────┴──────────────┴──────────────┴───┴──────────────────────┴────────────────────┴─────────┴──────────────────┤
│ 1001 rows (40 shown)                                                                                                 9 columns (8 shown) │
└──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

Excel ワークシートの名前を指定

```
D SELECT * FROM st_read('../../tmp/load/sample.xlsx',layer='カタカナランダム');
┌─────────┬──────────────┬──────────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬──────────┬──────────┐
│ Field1  │    Field2    │    Field3    │ Field4  │ Field5  │ Field6  │ Field7  │ Field8  │ Field9  │ Field10 │ Field11 │ Field12  │ Field13  │
│ varchar │   varchar    │   varchar    │ varchar │ varchar │ varchar │ varchar │ varchar │ varchar │ varchar │ varchar │ varchar  │ varchar  │
├─────────┼──────────────┼──────────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼──────────┼──────────┤
│ No      │ Name         │ ALL          │ 1       │ 2       │ 3       │ 4       │ 5       │ 6       │         │         │ カタカナ │ カタカナ │
│ 1       │ サレ         │ サレヘルトン │ サ      │ レ      │ ヘ      │ ル      │ ト      │ ン      │         │         │ 1        │ ア       │
│ 2       │ シヌヲヘフ   │ シヌヲヘフニ │ シ      │ ヌ      │ ヲ      │ ヘ      │ フ      │ ニ      │         │         │ 2        │ イ       │
│ 3       │ オソミハ     │ オソミハケム │ オ      │ ソ      │ ミ      │ ハ      │ ケ      │ ム      │         │         │ 3        │ ウ       │
│ 4       │ トムアシテキ │ トムアシテキ │ ト      │ ム      │ ア      │ シ      │ テ      │ キ      │         │         │ 4        │ エ       │
│ 5       │ オンウ       │ オンウルハコ │ オ      │ ン      │ ウ      │ ル      │ ハ      │ コ      │         │         │ 5        │ オ       │
│ 6       │ レコスソニ   │ レコスソニキ │ レ      │ コ      │ ス      │ ソ      │ ニ      │ キ      │         │         │ 6        │ カ       │
│ 7       │ コミトハム   │ コミトハムヌ │ コ      │ ミ      │ ト      │ ハ      │ ム      │ ヌ      │         │         │ 7        │ キ       │
│ 8       │ ヘフテイキ   │ ヘフテイキス │ ヘ      │ フ      │ テ      │ イ      │ キ      │ ス      │         │         │ 8        │ ク       │
│ 9       │ ヤヲハラフ   │ ヤヲハラフハ │ ヤ      │ ヲ      │ ハ      │ ラ      │ フ      │ ハ      │         │         │ 9        │ ケ       │
│ 10      │ セレヤヒル   │ セレヤヒルフ │ セ      │ レ      │ ヤ      │ ヒ      │ ル      │ フ      │         │         │ 10       │ コ       │
│ 11      │ クワチシ     │ クワチシコエ │ ク      │ ワ      │ チ      │ シ      │ コ      │ エ      │         │         │ 11       │ サ       │
│ 12      │ ヌヨユノミン │ ヌヨユノミン │ ヌ      │ ヨ      │ ユ      │ ノ      │ ミ      │ ン      │         │         │ 12       │ シ       │
│ 13      │ ネテ         │ ネテンオセメ │ ネ      │ テ      │ ン      │ オ      │ セ      │ メ      │         │         │ 13       │ ス       │
│ 14      │ テソノ       │ テソノソコモ │ テ      │ ソ      │ ノ      │ ソ      │ コ      │ モ      │         │         │ 14       │ セ       │
│ 15      │ チフン       │ チフンスヨシ │ チ      │ フ      │ ン      │ ス      │ ヨ      │ シ      │         │         │ 15       │ ソ       │
│ 16      │ ノヘフネニ   │ ノヘフネニケ │ ノ      │ ヘ      │ フ      │ ネ      │ ニ      │ ケ      │         │         │ 16       │ タ       │
│ 17      │ イワヌヤウワ │ イワヌヤウワ │ イ      │ ワ      │ ヌ      │ ヤ      │ ウ      │ ワ      │         │         │ 17       │ チ       │
│ 18      │ ノヌヘ       │ ノヌヘタツカ │ ノ      │ ヌ      │ ヘ      │ タ      │ ツ      │ カ      │         │         │ 18       │ ツ       │
│ 19      │ ノサスムン   │ ノサスムンロ │ ノ      │ サ      │ ス      │ ム      │ ン      │ ロ      │         │         │ 19       │ テ       │
│ ·       │    ·         │      ·       │ ·       │ ·       │ ·       │ ·       │ ·       │ ·       │    ·    │    ·    │ ·        │ ·        │
│ ·       │    ·         │      ·       │ ·       │ ·       │ ·       │ ·       │ ·       │ ·       │    ·    │    ·    │ ·        │ ·        │
│ ·       │    ·         │      ·       │ ·       │ ·       │ ·       │ ·       │ ·       │ ·       │    ·    │    ·    │ ·        │ ·        │
│ 982     │ ユラハユ     │ ユラハユニヒ │ ユ      │ ラ      │ ハ      │ ユ      │ ニ      │ ヒ      │         │         │          │          │
│ 983     │ シウカアセ   │ シウカアセク │ シ      │ ウ      │ カ      │ ア      │ セ      │ ク      │         │         │          │          │
│ 984     │ マシ         │ マシメクフヲ │ マ      │ シ      │ メ      │ ク      │ フ      │ ヲ      │         │         │          │          │
│ 985     │ リキフウ     │ リキフウイラ │ リ      │ キ      │ フ      │ ウ      │ イ      │ ラ      │         │         │          │          │
│ 986     │ ルンチヘナ   │ ルンチヘナナ │ ル      │ ン      │ チ      │ ヘ      │ ナ      │ ナ      │         │         │          │          │
│ 987     │ ルミムモホヌ │ ルミムモホヌ │ ル      │ ミ      │ ム      │ モ      │ ホ      │ ヌ      │         │         │          │          │
│ 988     │ ニツソレヌ   │ ニツソレヌユ │ ニ      │ ツ      │ ソ      │ レ      │ ヌ      │ ユ      │         │         │          │          │
│ 989     │ ハニロノ     │ ハニロノリメ │ ハ      │ ニ      │ ロ      │ ノ      │ リ      │ メ      │         │         │          │          │
│ 990     │ ンヲミルチ   │ ンヲミルチソ │ ン      │ ヲ      │ ミ      │ ル      │ チ      │ ソ      │         │         │          │          │
│ 991     │ アオサソ     │ アオサソルイ │ ア      │ オ      │ サ      │ ソ      │ ル      │ イ      │         │         │          │          │
│ 992     │ セホソレト   │ セホソレトレ │ セ      │ ホ      │ ソ      │ レ      │ ト      │ レ      │         │         │          │          │
│ 993     │ ナメチラタヒ │ ナメチラタヒ │ ナ      │ メ      │ チ      │ ラ      │ タ      │ ヒ      │         │         │          │          │
│ 994     │ ヒフ         │ ヒフニラヲキ │ ヒ      │ フ      │ ニ      │ ラ      │ ヲ      │ キ      │         │         │          │          │
│ 995     │ ソキサヌ     │ ソキサヌトヲ │ ソ      │ キ      │ サ      │ ヌ      │ ト      │ ヲ      │         │         │          │          │
│ 996     │ タヌ         │ タヌンリハミ │ タ      │ ヌ      │ ン      │ リ      │ ハ      │ ミ      │         │         │          │          │
│ 997     │ ソハンヲ     │ ソハンヲヘミ │ ソ      │ ハ      │ ン      │ ヲ      │ ヘ      │ ミ      │         │         │          │          │
│ 998     │ ワエ         │ ワエアヘンリ │ ワ      │ エ      │ ア      │ ヘ      │ ン      │ リ      │         │         │          │          │
│ 999     │ イミオマチワ │ イミオマチワ │ イ      │ ミ      │ オ      │ マ      │ チ      │ ワ      │         │         │          │          │
│ 1000    │ ミノヒテホテ │ ミノヒテホテ │ ミ      │ ノ      │ ヒ      │ テ      │ ホ      │ テ      │         │         │          │          │
│ 1001    │ モチシタロ   │ モチシタロヨ │ モ      │ チ      │ シ      │ タ      │ ロ      │ ヨ      │         │         │          │          │
├─────────┴──────────────┴──────────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴──────────┴──────────┤
│ 1002 rows (40 shown)                                                                                                             13 columns │
└─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

新しいテーブルの作成

```
D CREATE TABLE new_tbl AS
> SELECT * FROM st_read('../../tmp/load/sample.xlsx',layer='カタカナランダム');
D SELECT * FROM new_tbl LIMIT 5;
┌─────────┬──────────────┬──────────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬──────────┬──────────┐
│ Field1  │    Field2    │    Field3    │ Field4  │ Field5  │ Field6  │ Field7  │ Field8  │ Field9  │ Field10 │ Field11 │ Field12  │ Field13  │
│ varchar │   varchar    │   varchar    │ varchar │ varchar │ varchar │ varchar │ varchar │ varchar │ varchar │ varchar │ varchar  │ varchar  │
├─────────┼──────────────┼──────────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼──────────┼──────────┤
│ No      │ Name         │ ALL          │ 1       │ 2       │ 3       │ 4       │ 5       │ 6       │         │         │ カタカナ │ カタカナ │
│ 1       │ サレ         │ サレヘルトン │ サ      │ レ      │ ヘ      │ ル      │ ト      │ ン      │         │         │ 1        │ ア       │
│ 2       │ シヌヲヘフ   │ シヌヲヘフニ │ シ      │ ヌ      │ ヲ      │ ヘ      │ フ      │ ニ      │         │         │ 2        │ イ       │
│ 3       │ オソミハ     │ オソミハケム │ オ      │ ソ      │ ミ      │ ハ      │ ケ      │ ム      │         │         │ 3        │ ウ       │
│ 4       │ トムアシテキ │ トムアシテキ │ ト      │ ム      │ ア      │ シ      │ テ      │ キ      │         │         │ 4        │ エ       │
└─────────┴──────────────┴──────────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴──────────┴──────────┘
```

既存のテーブルへのロード

```
D INSERT INTO new_tbl SELECT * FROM st_read('../../tmp/load/sample.xlsx',layer='カタカナランダム');

D SELECT COUNT(Field1) FROM new_tbl;
┌───────────────┐
│ count(Field1) │
│     int64     │
├───────────────┤
│          2004 │
└───────────────┘
```

### MySQL インポート

実行中の MySQL データベースに対してクエリを直接実行するには、mysql 拡張機能が必要です

```
D INSTALL mysql;
D LOAD mysql;
```

MySQL にアタッチ用 DB を作成

```
root@masami-L:/bin# mysqladmin -u root -p create sample
Enter password:
```

次のコマンドを使用して MySQL データベースに接続できます

```
D ATTACH 'host=localhost user=root port=3306 database=sample' AS mysql_db (TYPE mysql_scanner, READ_ONLY);
Error: IO Error: Failed to connect to MySQL database with parameters "host=localhost user=root port=3306 database=sample": Can't connect to local MySQL server through socket '/tmp/mysql.sock' (2)

```

ソケットファイルを介した接続でエラーが起きているため、
ソケットファイルを探します。

```
root@masami-L:/bin# mysqladmin variables | grep socket
| mysqlx_socket                                            | /var/run/mysqld/mysqlx.sock                                                                                                                        |
| performance_schema_max_socket_classes                    | 10                                                                                                                          |
| performance_schema_max_socket_instances                  | -1                                                                                                |
| socket                                                   | /var/run/mysqld/mysqld.sock                                                              |

```

ソケットを指定

```
D H 'host=localhost user=root port=3306 database=sample socket=/var/run/mysqld/mysqlx.sock' AS mysql_db (TYPE mysql_scanner, READ_ONLY);
Error: IO Error: Failed to connect to MySQL database with parameters "host=localhost user=root port=3306 database=sample socket=/var/run/mysqld/mysqlx.sock": Protocol mismatch; server version = 11, client version = 10
```

調べてみた感じでは、MySQL の X プラグイン の無効化が必要なようだ。
MySQL の設定ファイルを次のように変更し、X プラグインを無効にします  
設定ファイルの格納場所は環境によりけり、  
私の場合は

- etc/mysql/my.cnf

でした

```
[mysqld]
mysqlx=0
```

MySQL サーバを再起動して、再度ソケットファイルを検索

```
root@masami-L:/# systemctl restart mysql
root@masami-L:/bin# mysqladmin variables | grep socket
| performance_schema_max_socket_classes                    | 10                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| performance_schema_max_socket_instances                  | -1                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| socket                                                   | /var/run/mysqld/mysqld.sock                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
```

DuckDB から MySQL サーバーにアクセスします

```
D ATTACH 'host=localhost user=root password=************* port=3306 database=sample socket=/var/run/mysqld/mysqld.sock' AS mysql_db (TYPE mysql_scanner, READ_ONLY);
D use mysqlscanner;
D use mysql_db;
```

アクセスはできるようになりました。
ここでは簡単のため、デフォルトポートを指定。

読み取り権限でアタッチしているのでデータベース作成コマンドは失敗します。

```
D CREATE TABLE tbl(id INTEGER, name VARCHAR);
Error: Invalid Error: Cannot execute statement of type "CREATE" on database "mysql_db" which is attached in read-only mode!
```

書き込み権限ありで再度アタッチします  
DETACH コマンドでもいいですが、面倒なので DuckDB を再起動しました。

```
D ATTACH 'host=localhost user=root password=********** port=3306 database=sample socket=/var/run/mysqld/mysqld.sock' AS mysql_db (TYPE mysq
D use mysql_db;
D CREATE TABLE tbl(id INTEGER, name VARCHAR);
D INSERT INTO tbl VALUES (42, 'DuckDB');
D SELECT * FROM tbl;
┌───────┬─────────┐
│  id   │  name   │
│ int32 │ varchar │
├───────┼─────────┤
│    42 │ DuckDB  │
└───────┴─────────┘
```

試しに MySQL の cli からデータが登録されているか確認します

```sql
mysql> SHOW TABLES;
+------------------+
| Tables_in_sample |
+------------------+
| tbl              |
+------------------+
1 row in set (0.00 sec)

mysql> select * from tbl;
+------+--------+
| id   | name   |
+------+--------+
|   42 | DuckDB |
+------+--------+
1 row in set (0.00 sec)
```

大丈夫そう。

### PostgreSQL インポート

アタッチ用の DB を作成する

```
root@masami-L:~# su - postgres
postgres@masami-L:~$ psql -U postgres -p 5432 -d postgres -c "CREATE DATABASE sample TEMPLATE = template0 ENCODING = 'UTF-8' LC_COLLATE = 'en_US.UTF-8' LC_CTYPE = 'en_US.UTF-8';"
CREATE DATABASE
postgres@masami-L:~$ psql -U postgres -d sample
psql (14.10 (Ubuntu 14.10-0ubuntu0.22.04.1))
Type "help" for help.
sample=# insert into tbl(col1) select i from generate_series(1, 10) as i;
INSERT 0 10
sample=# table tbl;
 col1
------
    1
    2
    3
    4
    5
    6
    7
    8
    9
   10
(10 rows)
```

DuckDB に拡張機能のインストールとロードを行う

```
masami@masami-L /bin> ./duckdb
v0.9.2 3c695d7ba9
Enter ".help" for usage hints.
Connected to a transient in-memory database.
Use ".open FILENAME" to reopen on a persistent database.
D INSTALL postgres;
D LOAD postgres;
```

アタッチする

```
D ATTACH 'host=localhost port=5432 dbname=sample user=postgres password=**********' AS test (TYPE
Error: IO Error: Unable to connect to Postgres at host=localhost port=5432 dbname=sample user=postgres password=***********: connection to server at "localhost" (127.0.0.1), port 5432 failed: FATAL:  password authentication failed for user "postgres"
connection to server at "localhost" (127.0.0.1), port 5432 failed: FATAL:  password authentication failed for user "postgres"
```

また接続できず。

今回は動かしたいだけなので pg_hba.conf を暫定的に書き換えます。  
諸々の設定値を trust にしてしまいます。

[変更前]

```
local   all             postgres                                peer
```

[変更後]

```
local   all             postgres                                trust
```

Postgresql サービスを再起動

```
root@masami-L /e/p/1/main# systemctl restart postgresql
```

認証設定をザルにしたので、DuckDB からアクセスしてみます

```
D ATTACH 'host=localhost port=5432 user=postgres dbname=sample' AS test (TYPE postgres);
D SELECT * FROM test.tbl;
┌───────┐
│ col1  │
│ int32 │
├───────┤
│     1 │
│     2 │
│     3 │
│     4 │
│     5 │
│     6 │
│     7 │
│     8 │
│     9 │
│    10 │
└───────┘
D USE test;
D SHOW TABLES;
┌─────────┐
│  name   │
│ varchar │
├─────────┤
│ tbl     │
└─────────┘
```

アクセスできるようになりました

### SQLite インポート

アタッチ用の DB を作成する

```
root@masami-L /bin# sqlite3 testdb.sqlite3
SQLite version 3.37.2 2022-01-06 13:25:41
Enter ".help" for usage hints.
sqlite> create table dist (num, name);
sqlite> insert into dist values(1, 'redhat');
sqlite> sqlite> .databases
main: /usr/bin/testdb.sqlite3 r/w
```

sqlite 拡張機能をロード

```
D INSTALL sqlite;
D LOAD sqlite;
```

sqlite_scan 関数を使用して SQLite からテーブルをクエリ

```
D SELECT * FROM sqlite_scan('testdb.sqlite3', 'dist');
┌──────┬────────┐
│ num  │  name  │
│ blob │  blob  │
├──────┼────────┤
│ 1    │ redhat │
└──────┴────────┘
```

データベース全体にアタッチする

```
D ATTACH 'testdb.sqlite3' AS test (TYPE sqlite);
D SELECT * FROM test.dist;
┌──────┬────────┐
│ num  │  name  │
│ blob │  blob  │
├──────┼────────┤
│ 1    │ redhat │
└──────┴────────┘
D USE test;
D SHOW TABLES;
┌─────────┐
│  name   │
│ varchar │
├─────────┤
│ dist    │
└─────────┘
```

外部データベースにアタッチするかどうかは使い方とパフォーマンス特性によるかなと思います。  
そのうち、DuckDB の性能評価をしたいと思います。
