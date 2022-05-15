## プロジェクトのコード変換(C#↔VB)

プロジェクトを指定して一括で C#↔VB に変換する方法を調べました。  
ブラウザ上でソースコード変換するサービスは沢山ありますが、
ソースコードを貼り付けたり、ファイルが別れている場合に、コンパイルに失敗したりするので、使い勝手は良いとは言えません。
今回、VisualStudio 上から一括変換できる拡張ツールを見つけたので、
その実行手順を記します。

- 1.github の[ぺージ](https://github.com/icsharpcode/CodeConverter/releases)から「ICSharpCode.CodeConverter.VsExtension.VS2017.vsix」をダウンロードします。

- 2.ダウンロードしたインストーラーを起動し、インストールを行います。

- 3.変換したいプロジェクトを VisualStudio で開きます。

- 4.変換したいプロジェクトのコンテキストメニューを右クリックで開き、ConvertTo 〇〇を選択して、実行します。

![1](https://user-images.githubusercontent.com/49807271/168452837-976d8f86-309c-4140-8835-909fbfefe885.jpg)

※プロジェクト構成ファイルも作成されます

以上
