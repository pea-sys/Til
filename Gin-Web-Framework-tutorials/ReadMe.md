# Gin Web Framework

Go の軽量 Web フレームワークのチュートリアル

https://gin-gonic.com/ja/docs/quickstart/

---

[環境]

- Ubuntu 22.04

---

- フォルダ作成

```sh
root@masami-L /u/src# mkdir gin-tutorial
root@masami-L /u/src# cd gin-tutorial
```

- 新しい Go モジュールを初期化します

```sh
root@masami-L /u/s/gin-tutorial [1]# go mod init github.com/pea-sys/gin-tutorial
go: creating new go.mod: module github.com/pea-sys/gin-tutorial
```

- gin をインストールします

```sh
root@masami-L /u/s/gin-tutorial# go get github.com/gin-gonic/gin
go: added github.com/bytedance/sonic v1.9.1
go: added github.com/chenzhuoyu/base64x v0.0.0-20221115062448-fe3a3abad311
go: added github.com/gabriel-vasile/mimetype v1.4.2
go: added github.com/gin-contrib/sse v0.1.0
go: added github.com/gin-gonic/gin v1.9.1
go: added github.com/go-playground/locales v0.14.1
go: added github.com/go-playground/universal-translator v0.18.1
go: added github.com/go-playground/validator/v10 v10.14.0
go: added github.com/goccy/go-json v0.10.2
go: added github.com/json-iterator/go v1.1.12
go: added github.com/klauspost/cpuid/v2 v2.2.4
go: added github.com/leodido/go-urn v1.2.4
go: added github.com/mattn/go-isatty v0.0.19
go: added github.com/modern-go/concurrent v0.0.0-20180306012644-bacd9c7ef1dd
go: added github.com/modern-go/reflect2 v1.0.2
go: added github.com/pelletier/go-toml/v2 v2.0.8
go: added github.com/twitchyliquid64/golang-asm v0.15.1
go: added github.com/ugorji/go/codec v1.2.11
go: added golang.org/x/arch v0.3.0
go: added golang.org/x/crypto v0.9.0
go: added golang.org/x/net v0.10.0
go: added golang.org/x/sys v0.8.0
go: added golang.org/x/text v0.9.0
go: added google.golang.org/protobuf v1.30.0
go: added gopkg.in/yaml.v3 v3.0.1
```

- 開始用テンプレートを取得

```sh
root@masami-L /u/s/gin-tutorial# curl https://raw.githubusercontent.com/gin-gonic/examples/master/basic/main.go > main.go
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed
100  1588  100  1588    0     0   2532      0 --:--:-- --:--:-- --:--:--  2532
```

- main.go の中身を確認

```go
package main

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

var db = make(map[string]string)

func setupRouter() *gin.Engine {
	// Disable Console Color
	// gin.DisableConsoleColor()
	r := gin.Default()

	// Ping test
	r.GET("/ping", func(c *gin.Context) {
		c.String(http.StatusOK, "pong")
	})

	// Get user value
	r.GET("/user/:name", func(c *gin.Context) {
		user := c.Params.ByName("name")
		value, ok := db[user]
		if ok {
			c.JSON(http.StatusOK, gin.H{"user": user, "value": value})
		} else {
			c.JSON(http.StatusOK, gin.H{"user": user, "status": "no value"})
		}
	})

	// Authorized group (uses gin.BasicAuth() middleware)
	// Same than:
	// authorized := r.Group("/")
	// authorized.Use(gin.BasicAuth(gin.Credentials{
	//	  "foo":  "bar",
	//	  "manu": "123",
	//}))
	authorized := r.Group("/", gin.BasicAuth(gin.Accounts{
		"foo":  "bar", // user:foo password:bar
		"manu": "123", // user:manu password:123
	}))

	/* example curl for /admin with basicauth header
	   Zm9vOmJhcg== is base64("foo:bar")

		curl -X POST \
	  	http://localhost:8080/admin \
	  	-H 'authorization: Basic Zm9vOmJhcg==' \
	  	-H 'content-type: application/json' \
	  	-d '{"value":"bar"}'
	*/
	authorized.POST("admin", func(c *gin.Context) {
		user := c.MustGet(gin.AuthUserKey).(string)

		// Parse JSON
		var json struct {
			Value string `json:"value" binding:"required"`
		}

		if c.Bind(&json) == nil {
			db[user] = json.Value
			c.JSON(http.StatusOK, gin.H{"status": "ok"})
		}
	})

	return r
}

func main() {
	r := setupRouter()
	// Listen and Server in 0.0.0.0:8080
	r.Run(":8080")
}
```

- プロジェクト実行

```sh
root@masami-L /u/s/gin-tutorial# go run main.go
[GIN-debug] [WARNING] Creating an Engine instance with the Logger and Recovery middleware already attached.

[GIN-debug] [WARNING] Running in "debug" mode. Switch to "release" mode in production.
 - using env:   export GIN_MODE=release
 - using code:  gin.SetMode(gin.ReleaseMode)

[GIN-debug] GET    /ping                     --> main.setupRouter.func1 (3 handlers)
[GIN-debug] GET    /user/:name               --> main.setupRouter.func2 (3 handlers)
[GIN-debug] POST   /admin                    --> main.setupRouter.func3 (4 handlers)
[GIN-debug] [WARNING] You trusted all proxies, this is NOT safe. We recommend you to set a value.
Please check https://pkg.go.dev/github.com/gin-gonic/gin#readme-don-t-trust-all-proxies for details.
[GIN-debug] Listening and serving HTTP on :8080
```

- ブラウザから「localhost:8080/ping」にアクセスすると、サーバにログが表示される

```sh
[GIN] 2024/01/29 - 23:01:54 | 200 |      68.872µs |       127.0.0.1 | GET      "/ping"
```

無事に GIN を使用した API が叩けました
