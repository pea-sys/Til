- pre-commit フックを使用して、ビルドの通らないコミットを封じる

```mermaid
flowchart TD
    A[Commit Action] --> B{is exist pre-commit-hook?}
    B -->|Yes| C{BuildResult} --> |Success| D[Commit]
    B -->|No| E[Commit]
    C -->|Failed|F[Commit Cancel]

```

- git/hook 配下に pre-commit スクリプトを配置する

```
#!/bin/sh


echo "dotnet build"

dotnet clean; dotnet build HelloWorld/HelloWorld.csproj
rc=$?

if [[ $rc != 0 ]] ; then
    echo -e "build failed"
    exit $rc
fi

exit 0
```

■ メリット

- Git ホスティングサーバーにビルド環境不要
- コミットログが汚れない

■ デメリット

- 大規模開発で雑なコミットが出来ない
- コミットに時間が掛かる

* クライアントサイドフックなので、リポジトリの管理ファイルに含まれない(コントリビューターに強制できない)

■ 参考

- ビルド成功

![build_success](https://github.com/pea-sys/Til/assets/49807271/002b4b30-655b-4287-83c6-c1f83261b32c)

- ビルドエラー

![build_error](https://github.com/pea-sys/Til/assets/49807271/b2f7604a-9b6f-46f7-b613-b80b7bc869d9)
