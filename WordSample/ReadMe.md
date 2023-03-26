# Word のテキストを読み込む

Word の Grep 検索、Grep 置換をしたいと思い、事前調査してみました。  
取り合えず C#でやってみましたが、後から考えるとシェルで良かったかなと。

C#で word を取り扱う場合、COM コンポーネントの参照を追加すれば良い

- Microsoft Office 16 Object Library
- Microsoft Word 16 Object Library

注意点は、Office ファイルを開きっぱなしにしていないか、word プロセスが生き残っていないかチェックする必要がある。
