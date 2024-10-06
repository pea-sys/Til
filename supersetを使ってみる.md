# apache superset を使ってみる

apache superset はデータ探索および視覚化プラットフォームです。

https://superset.apache.org/

superset を docker イメージを使用して起動します。

```
git clone --depth=1  https://github.com/apache/superset.git
cd superset
docker compose -f docker-compose-image-tag.yml up
・・・
[+] Running 6/6
 ✔ Container superset_db           Created                                            0.4s
 ✔ Container superset_cache        Created                                            0.4s
 ✔ Container superset_app          Created                                            0.1s
 ✔ Container superset_init         Created                                            0.1s
 ✔ Container superset_worker       Created                                            0.1s
 ✔ Container superset_worker_beat  Created                                            0.1s
```

DB とテーブルを作成します

```
docker exec -it superset_db bash
root@55c442e52341:/# createdb -U superset sample
root@55c442e52341:/# psql -U superset -d sample

sample=# CREATE TABLE transactions (
   time TIMESTAMPTZ,
   block_id INT,
   hash TEXT,
   size INT,
   weight INT,
   is_coinbase BOOLEAN,
   output_total BIGINT,
   output_total_usd DOUBLE PRECISION,
   fee BIGINT,
   fee_usd DOUBLE PRECISION,
   details JSONB
);
CREATE TABLE
sample=# CREATE INDEX hash_idx ON public.transactions USING HASH (hash);
CREATE INDEX block_idx ON public.transactions (block_id);
CREATE UNIQUE INDEX time_hash_idx ON public.transactions (time, hash);
CREATE INDEX
CREATE INDEX
CREATE INDEX
sample=# GRANT SELECT ON transactions TO PUBLIC;
GRANT
sample=#\q
```

wget をインストール後、サンプルデータをダウンロード

```
apt-get update && apt-get install -y wget
apt-get update && apt-get install -y unzip
wget https://assets.timescale.com/docs/downloads/bitcoin-blockchain/bitcoin_sample.zip
root@55c442e52341:/# unzip bitcoin_sample.zip
Archive:  bitcoin_sample.zip
  inflating: tutorial_bitcoin_sample.csv
```

データを取り込みます

```
root@55c442e52341:/# psql -U superset -d sample
sample=# \COPY transactions FROM 'tutorial_bitcoin_sample.csv' CSV HEADER;
COPY 2719085
```

次ノページにアクセスするとログイン画面にリダイレクトします

http://localhost:8088/

![1](https://github.com/user-attachments/assets/dc0667c8-68c3-4ac4-8e2a-a8aa9cbe3a72)

USERNAME と PASSWORD に「admin」を入力するとログインできます。

最初からサンプル用のデータベースコネクションが用意されています。

![2](https://github.com/user-attachments/assets/76a978f7-3292-4be1-8b41-ce647eaba9c8)

折角なので、作成した DB のコネクションを追加します。

Connect database を選択します

![3](https://github.com/user-attachments/assets/d06e0a0d-a6f8-43bd-a5b3-2bb285a6dfec)

PostgreSQL を選択します

![3](https://github.com/user-attachments/assets/a5e4109e-544a-43ed-8354-2069b7216b71)

Dataset を作成します

![2](https://github.com/user-attachments/assets/b489146e-55a1-4f35-83c9-af8aeedd91d6)

SQL Alchemy 経由で接続します
![4](https://github.com/user-attachments/assets/fb8ce9f5-295b-4bf1-abb6-c8724d4a5161)

URI`postgresql://examples:examples@db:5432/sample`を入力して  
TEST CONNECTION を実行し、「Connection looks good!」が表示されたら、
CONNECT を選択します

![5](https://github.com/user-attachments/assets/86ee2157-66bd-4ae9-9200-b98374b0acc4)

データセットを追加します  
最初に作った transaction テーブルを対象にします

![5](https://github.com/user-attachments/assets/0e0341bf-622f-42a1-9964-fbcb0b661903)

適用にグラフを作成し、ダッシュボードに追加します
![8](https://github.com/user-attachments/assets/18ec969c-2f55-4935-9abd-262ea1a55cde)

ダッシュボードタブから確認できるようになりました

![9](https://github.com/user-attachments/assets/64a245d7-bbda-415e-b163-d6fa0f5acce5)

ダッシュボードページの公開やダッシュボードレイアウトの変更はダッシュボードページから実施可能です
