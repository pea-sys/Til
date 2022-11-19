# png と apng の見分け方

ZopfliPng に apng を渡すとアニメーションではなくなってしまうため、  
.NET Framework で png と apng の判定をどのようにしたらいいか調べました。  
gif と同じように判定しようと思っていましたが、どうも出来ないようで。 GIF の場合は、CanAnimate でアニメーション有か判断できます。

```csharp
private bool IsPNG(string filePath)
        {
            ImageCodecInfo[] decoders = ImageCodecInfo.GetImageDecoders();
            Bitmap bmp = new Bitmap(filePath);
            foreach (ImageCodecInfo ici in decoders)
            {
                //apng判定不可。CanAnimateがfalseになる。
                //apngは対象外にしたい！
                if (ici.FormatDescription == "PNG" && !ImageAnimator.CanAnimate(bmp))
                {
                    return true;
                }
            }
            return false;
        }
```

apng は新しめのフォーマットなので、もう古い環境では原始的な方法で判定できないかもしれません。  
Zopfli のリポジトリの issue にも要望は挙がっていますが、対応は期待できません。  
https://github.com/google/zopfli/issues/118  
ImageMagick も似たような Issue がありますが長い討論の末、対応しているようです。  
https://github.com/ImageMagick/ImageMagick/issues/24

取り合えず、ここでは原始的な方法で判定することを考えます。

■ PNG フォーマット

|                        | PNG       | ファイル全体構造     |
| ---------------------- | --------- | -------------------- |
|                        | サイズ    | 解説                 |
| PNG ファイルシグネチャ | 8 バイト  | PNG であることを示す |
| IHDR チャンク          | 25 バイト | イメージヘッダ       |
| ・                     |           |
| ・                     |           |
| ・                     |           |
| IDAT チャンク          | 可変長    | イメージデータ       |
| ・                     |           |
| ・                     |           |
| ・                     |           |
| IEND チャンク          | 12 バイト | イメージ終端         |

※抜粋 https://www.setsuki.com/hsp/ext/png.htm

実際に小さい png ファイルをバイナリエディタで開きます。  
![miniping_dump](https://user-images.githubusercontent.com/49807271/201511516-187bda4c-8c3f-4911-80cc-9cb8505ab193.png)

- APNG のフォーマット  
  基本的には png を拡張した形になる。

![720px-Apng_assembling svg](https://user-images.githubusercontent.com/49807271/201511605-c48a2133-622c-4c74-937c-e17af63a5803.png)
※WIki 抜粋

apng ファイルをバイナリエディタで開きます。  
![2](https://user-images.githubusercontent.com/49807271/201511876-047cef5d-d9bf-4acf-b1fa-589cd8d119e6.png)

元の png と apng の違いを判定するには IDAT チャンクの前に acTL チャンクがあることを確認すれば良さそうです。

```csharp

        private bool IsPNG(string filePath)
        {
            string asciiData = Encoding.ASCII.GetString(File.ReadAllBytes(filePath));
            if (asciiData.Substring(1,3)=="PNG")
            {
                // IDATチャンクの前にacTLがあればapngと判定
                // apngは対象外にしたい！
                long idatPos = asciiData.IndexOf("IDAT");
                long acTLPos = asciiData.IndexOf("acTL");
                if (acTLPos != -1 && acTLPos < idatPos)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
```
