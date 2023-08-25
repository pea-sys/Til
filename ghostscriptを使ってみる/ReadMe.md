# ghostscript を使ってみる

ghostscript は、PDF ファイルのインタープリタです。
今回は PDF のサイズ削減を目的に ghostscript を使用してみます。

- インストーラのダウンロードリンク  
  https://www.ghostscript.com/releases/gsdnld.html

試した結果、ghostscript の圧縮効果は想像以上に高かったです。  
パラメータによる品質やサイズに差異はあまりないけど、これは試した pdf ファイルのせいかもしれない。  
調べた範囲では ebook を推奨する方が多い。

| ソフト      | 方法                   | サイズ(KB) | ファイル名       |
| ----------- | ---------------------- | ---------- | ---------------- |
| word        | デフォルトエクスポート | 106        | word_default.pdf |
| word        | 最小エクスポート       | 81         | word_min.pdf     |
| ghostscript | ebook                  | 26         | gs_ebook.pdf     |
| ghostscript | screen                 | 26         | gs_screen.pdf    |
| ghostscript | printer                | 31         | gs_printer.pdf   |
| ghostscript | prepress               | 26         | gs_prepress.pdf  |
| ghostscript | default                | 26         | gs_default.pdf   |

[コマンド例]

```
>"ghostscript 10.01.2.LNK" -sDEVICE=pdfwrite -dPDFSETTINGS=/default -dNOPAUSE -sOutputFile="gs_default.pdf" word_min.pdf
```

ghostscript のパラメータ変更点は次の通り

-dPDFSETTINGS=configuration
「蒸留器パラメータ」を次の定義済み設定のいずれかにプリセットします。

- /screen  
  Acrobat Distiller (バージョン X まで) の「画面最適化」設定と同様の低解像度出力を選択します。

- /ebook  
  Acrobat Distiller (バージョン X まで) の「eBook」設定と同様の中解像度の出力を選択します。

- /printer  
  Acrobat Distiller の「印刷最適化」（バージョン X まで）設定と同様の出力を選択します。

- /prepress  
  Acrobat Distiller の「Prepress Optimized」（バージョン X まで）設定と同様の出力を選択します。

- /default  
  さまざまな用途に役立つ出力を選択しますが、場合によっては出力ファイルが大きくなります。
