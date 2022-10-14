# WebPack

WebPack は依存関係のあるモジュールを解析して、1 つのファイルにまとめてくれます。ファイルをまとめることで、HTML ファイルにインクルードする必要があるのは、バンドルされた js ファイル１つのみになり、ファイルロード時間の削減が期待できます。  
また、Uglify(シンボルの簡略表記)と Minify(冗長な記述の最適化)も Webpack に付属しています。

ここでは試験的に簡単な動作確認を行います。  
[手順]

## ■webpack 導入

- 1.[npm](https://nodejs.org/en/download/) をインストールします

* 2.コマンドプロンプトを起動して、次のコマンドを入力

```
mkdir my-first-webpack
cd my-first-webpack
npm init -y
mkdir src
```

- 3.webpack の依存関係をインストール

```
npm install --save-dev webpack
npm install --save-dev webpack-cli
```

- 4.src フォルダにシンプルな javascript ファイルを格納します

```js
alert("Hello World");
```

- 5.package.json の script 項目を次のように編集します

```json
"scripts": {
    "build": "webpack --mode development"
  }
```

- 6.webpack を実行

```
npm run build
```

- 7.dist ディレクトリに main.js が生成されていることを確認します

* 8.myfirstwebpack フォルダ直下に webpack.config.js を作成します

```js
const webpack = require("webpack");
const path = require("path");
const config = {
  entry: "./src/index.js",
  output: {
    path: path.resolve(__dirname, "dist"),
    filename: "bundle.js",
  },
};
module.exports = config;
```

- 9.webpack を実行  
  config で output を bundle.js にしているので、dist フォルダに bundle.js が出力されます。

```
npm run build
```

## ■es6 のトランスパイル

webpack は Javascript ファイルのみを理解します。　　 ES6 コード、PNG ファイル、Less など、他の種類のコードやファイルを webpack が理解できるようにするには、それらのファイルの種類に応じたローダーを webpack の設定ファイルに追加する必要があります。

- 10.ES6 コードのローダーである babel の依存関係をインストール

```
npm install --save-dev babel-loader @babel/core @babel/preset-env
```

webpack の設定ファイルに babel-loader を追加する前に、  
babel を設定する必要があります。Babel には.babelrc という設定ファイルがあります。

- 11.myfirstwebpack フォルダ直下に.babelrc を作成します。  
  babel が最新バージョンの JS をトランスパイルできるようにします。

```
{
  presets: ['@babel/preset-env']
}
```

- 12.webpack.config.js にルールを追加します。  
  node_modules は一般的にトランスパイル不要とされている気がしますが、
  必ずしもそうでもないので注意が必要です。
  ここでは exclude に node_module を指定して、トランスパイルの対象外にします。

```js
const webpack = require("webpack");
const path = require("path");
const config = {
  entry: "./src/index.js",
  output: {
    path: path.resolve(__dirname, "dist"),
    filename: "bundle.js",
  },
  module: {
    rules: [
      {
        test: /\.(js)$/,
        exclude: /node_modules/,
        use: "babel-loader",
      },
    ],
  },
};
module.exports = config;
```

- 13. ES6 のコードを使って index.js を編集します

```js
const hello = () => {
  alert("Hello World");
};
hello();
```

- 14.webpack を実行

```
npm run build
```

ES6 が古い JavaScript にトランスパイルされます。

```
var hello = function hello() {
    alert("Hello World");
```

## ■jsx のトランスパイル

- 15.React の依存関係をインストール

```
npm install --save react react-dom
```

- 16.index.js を次のように編集

```js
import React from "react";
import ReactDOM from "react-dom";
class App extends React.Component {
  render() {
    return <div>Hello {this.props.name}<div>;
  }
}
var mountNode = document.getElementById("app");
ReactDOM.render(<App name="world">, mountNode);
```

- 17.dist フォルダに index.html ファイルを配置

```html
<!DOCTYPE html>
<html>
  <head>
    <title>React starter app</title>
  </head>
  <body>
    <div id="app"></div>
    <script src="bundle.js"></script>
  </body>
</html>
```

- 18.React の JSX をトランスパイルするためのプラグインをインストール

```
npm install --save-dev @babel/preset-react
```

- 19..babelrc を編集します

```
{
  presets: [
    '@babel/preset-env',
    '@babel/preset-react'
  ]
}
```

- 20.webpack で jsx ファイルが解釈できるように、webpack.config.js の test 項目に jsx を追加

```js
const webpack = require("webpack");
const path = require("path");
const config = {
  entry: "./src/index.js",
  output: {
    path: path.resolve(__dirname, "dist"),
    filename: "bundle.js",
  },
  module: {
    rules: [
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: "babel-loader",
      },
    ],
  },
  resolve: {
    extensions: [".js", ".jsx"],
  },
};
module.exports = config;
```

## ■css のトランスパイル

- 21.css ローダーのインストール

```
npm install --save-dev style-loader css-loader
```

- 22.webpack.config.js のルールを追加

```js
const webpack = require("webpack");
const path = require("path");
const config = {
  entry: "./src/index.js",
  output: {
    path: path.resolve(__dirname, "dist"),
    filename: "bundle.js",
  },
  module: {
    rules: [
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: "babel-loader",
      },
      {
        test: /\.css$/,
        use: ["style-loader", "css-loader"],
      },
    ],
  },
  resolve: {
    extensions: [".js", ".jsx"],
  },
};
module.exports = config;
```

- 23.src フォルダに styles.css を作成

```css
body {
  color: green;
}
```

- 24.index.js ファイルの先頭に以下を追記

```js
require("./styles.css");
```
