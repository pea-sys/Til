# Windows10 で docker 導入時に起動エラー

原因不明だが、以下の作業をやったらエラー解消されたのでメモ

[事象]  
docker が起動しない

![docker起動失敗](https://user-images.githubusercontent.com/49807271/230686004-8482ec46-7321-435a-babf-dd13a3cb4d7f.png)

[環境]

- Windows10
- wsl2 が導入されるより以前に docker を使っていたかもしれない

[対処]

- Hyper-V の有効化  
  ![HV](https://user-images.githubusercontent.com/49807271/230686457-aca03c32-0d93-420f-944e-baecf4b376bf.png)

- wsl の更新

```
wsl --update
```

- DockerForWindows の再インストール
  https://docs.docker.com/desktop/install/windows-install/

- Docker のクリーンアップ  
  ![クリーンアップ](https://user-images.githubusercontent.com/49807271/230690130-cd5e2b1e-db31-4aa5-af61-f37ee5d1aac4.png)

どれが決め手になったか分かりませんが、これで起動するようになりました。
