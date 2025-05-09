## リソースレコードのタイプ

| タイプ | 内容                                           |
| ------ | ---------------------------------------------- |
| A      | ドメイン名の IPv4 アドレスを指定               |
| AAAA   | ドメイン名の IPv6 アドレスを指定               |
| NS     | ゾーンの権威サーバーのホスト名を指定           |
| MX     | ドメイン名宛の電子メールの配送先と優先度を指定 |

## 有用な DNS チェックサイト

### Zonemaster

ドメインの総合的な診断
https://www.zonemaster.net/

### DNSViz

DNSSEC の信頼の連鎖を確認

http://dnsviz.net/

### dnscheck.jp

DNS の設定チェック
https://dnscheck.jp/

## サーバーの監視

### nagios

権威サーバーフルリボゾルバーの死活監視
https://www.nagios.org/

### DSC

トラフィック監視
https://www.dns-oarc.net/tools/dsc

## 攻撃への対策

### DNS リフレクター

- フルリゾルバー：サービスを提供するネットワークからのアクセスを許可する。
- 権威サーバー：RRL の適用(同一宛先に対する応答が所定の頻度を越えたら応答しない)

### ランダムサブドメイン攻撃

- オープンリゾルバーのアクセス制限
- ホームルーターに IP53B を実施
- フルリゾルバーでフィルタリングや問い合わせレート制限

### BIND の脆弱性を突いた攻撃

- ソフトウェアを最新版に保つ
- BIND とは別の DNS システムを並行運用

### キャッシュポイズニングの対策

- ソースポートランダマイゼーションを使用
- DNS クッキーの導入

### 登録情報の不正書き換えによるドメイン名ハイジャック

- アカウント管理の適正化(二要素認証や証明書等)
- レジストリロックの利用
- 不正書き換えの検知(Whois 情報、権威サーバの委任情報の監視)

## よりよい DNS 運用のために

- DNSSEC・・・DNS の応答に偽造困難な電子署名を追加
- DNS クッキー・・・HTTP クッキーと同じ仕組み

## DNS の設定・運用に関するノウハウ

- lame delegation・・・委任元(親)ゾーンに委任情報として登録されている権威サーバーが。委任先(子)ゾーンの権威サーバーとして動作していない状態
  対策は権威サーバ設定時の確認に留まらず、継続的な監視を行う
