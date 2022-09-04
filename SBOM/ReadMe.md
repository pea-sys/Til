# SBOM

SBOM とは特定の製品に含まれるすべてのソフトウェアコンポーネント、ライセンス、依存関係を一覧化したものです。  
2022 年 7 月に MicroSoft が社内で使用していた SBOM 生成ツールを[GitHub](https://github.com/microsoft/sbom-tool#run-the-tool)で公開しました。  
私の関わっているプロジェクトでもコンポーネントのバージョン管理はしていますが、全部手動で行っているので、使えるものなら使ってみたいと思い、ちょいと触ってみました。  
気になるところとしては、デバイスのファームウェアもパッケージに同梱しているので、それらの情報もちゃんと拾ってくれるかどうかといったところ。

[参考資料]

- [わかる、作れる、活用できる！ソフトウェア構成表「SBOM」のすべて](https://www.youtube.com/watch?v=GbncYacj0tM&list=LL&ab_channel=%E6%97%A5%E7%AB%8B%E3%82%BD%E3%83%AA%E3%83%A5%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%82%BA%E3%83%97%E3%83%AD%E3%83%A2%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3Ch)
- [SPDX 仕様](https://spdx.github.io/spdx-spec/)

### ファイル生成方法

- 1.microsoft の sbom-tool リポジトリのリリースページからツールをダウンロードする。  
  ※2022/09/03 現在は v0.1.13 が安定版。それ以降の v0.2.2 まではバグがあるようで、ファイル生成はできませんでした。

- 2.ドキュメントに沿って、ダウンロードした exe ファイルを引数付きで実行(名前空間は適当)

```
C:\Users\user\Downloads\sbom-tool-win-x64.exe generate -b C:\Users\user\source\repos\Telnet.Server\Telnet.Server\bin\Debug\net6.0 -bc C:\Users\user\source\repos\Telnet.Server\Telnet.Server -pn Telnet.Server -pv 1.0.0 -nsb https://companyName.com/teamName/Telnet.Server
```

- 3.出力フォルダに SBOM である spdx.json が出力されていることを確認

```
└_manifest
  └spdx_2.2
    ├manifest.spdx.json
    └manifest.spdx.json.sha256
```

ちなみに json から csv への変換処理は自分で書いてもいいし、適当なツールやサービスで比較的簡単に可能。  
そのうち、html での出力機能とかも追加されると思います。
