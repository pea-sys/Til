# Tauri のチュートリアルアプリ

クロスプラットフォームのアプリケーションフレームワークの tauri を使ってみます。  
flutter とどちらを学ぼうか迷いましたが、よりデスクトップアプリにフォーカスした tauri の方が自分の用途に合ってると感じ、  
こちらを選択しました。  
2022 年 6 月に安定版がリリースされました。Rust 製です。エレクトロンの後継のようなポジションになると思います。

- [前提条件](https://tauri.app/v1/guides/getting-started/prerequisites)
- [クイックスタート](https://tauri.app/v1/guides/getting-started/setup/)  
  ※備考  
  実行にあたっては、前提条件で指定されているコンポーネントを
  厳密にバージョンを守ってインストールする必要があります。  
  そうしないと、tauri のインストールに失敗する可能性が高まります。

---

- 本格的に使用する場合、事前に以下に目を通しておいた方が良いでしょう。  

  https://tauri.app/v1/references/architecture/

- 配布可能なバイナリは以下コマンドで作成。

```
cargo build --release
```
