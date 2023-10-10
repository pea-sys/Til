# Windows10 で解凍処理が止まる・遅い時の対処方法

Windows で巨大な圧縮ファイルを解凍する時に、不自然に進捗度が止まる場合がある。  
また、カーネル等のビッグプロジェクトをコンパイルする場合にも同様の問題が起きがち。

![5](https://github.com/pea-sys/Til/assets/49807271/7c90d513-978b-4591-8564-43c715ca590e)

そんな時は、まずタスクマネージャーを見る。
![5](https://github.com/pea-sys/Til/assets/49807271/7adcaec1-ac59-489e-b635-a53e373daeb4)

見てみると、Antimalware Sevice Executable プロセスが CPU リソースを大量に使用しています。  
一方、解凍ツールの PeaZip には CPU リソースが全然回ってません。

このプロセスは、Windows Defender の一部です。  
一時的にセキュリティリスクが上がりますが、リアルタイム保護を無効にすることで解凍処理がスムーズに進むようになります。  
自己責任で実施お願いします。

- Windows ロゴを右クリックして設定を選択
- 更新とセキュリティを選択
  ![5](https://github.com/pea-sys/Til/assets/49807271/f1d9c9e1-d2dd-410f-b383-6e13b534eaac)

- Windows セキュリティを選択
- Windows セキュリティを開くを選択
  ![7](https://github.com/pea-sys/Til/assets/49807271/55c9a57c-b13d-4a46-9e15-ae707926acd4)
- 設定の管理を選択
  ![8](https://github.com/pea-sys/Til/assets/49807271/5f984b7a-a8b6-4e4e-8bea-bed3a629c5fc)
- リアルタイム保護をオンからオフに変更する
  ![9](https://github.com/pea-sys/Til/assets/49807271/c8deb953-bc99-443a-b24c-9ce4957e6fac)

希望の動作が終わったら、リアルタイム保護を有効にするか PC を再起動します。

以上。
