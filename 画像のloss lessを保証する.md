# 画像の loss less を保証する

PowerToys にコントリビュートする際に、そういえば zopfli はアルゴリズム的に loss less は立証はされているものの、パラメータも色々あり、やり方によっては loss もあると思うので、手順の正しさを証明するためにも、成果物が loss less であることを保証する必要があるなと思い、やり方を書き留めておくことにしました。

https://github.com/microsoft/PowerToys/pull/21521

と言っても、滅茶苦茶単純です。
もっと楽な方法ないか探してます。  
cli でまとめてやりたい。

[手順]

- 1.変更前のリポジトリをクローンする
- 2.変更後のリポジトリをクローンする
- 3.WinMerge をダウンロードする
- 4.両リポジトリを選択し、WinMerge で開く,画像バイナリは差分ありと出ます  
  ![1](https://user-images.githubusercontent.com/49807271/198817553-4b03ff62-5287-475a-a4a2-dabe324d631f.png)

- 5.各画像の差分を見ると差分はないと出ます。  
  つまり、バイナリ差分はあるけど見た目は同じということです。  
  ※WinMerge は png の差分比較をサポートしている
  ![diff](https://user-images.githubusercontent.com/49807271/198817615-9263c098-1846-4cc2-ac59-02df821df25d.jpg)
