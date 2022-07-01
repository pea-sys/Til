## 結果

| コマンド                     | .git フォルダサイズ | 用途                            |
| ---------------------------- | ------------------- | ------------------------------- |
| clone                        | 425MB               | 全ての差分を取得                |
| clone --depth 1              | 186MB               | 直近差分のみ取得                |
| clone --depth 1 -branch main | 18.7MB              | main ブランチの直近差分のみ取得 |

クローン速度は下段に行くほど倍以上早くなります。

大きいリポジトリの場合、
何も考えずに git clone を行うと完了まで長時間待つことになる

```
PS C:\Users\user\source\repos> git clone https://github.com/dotnet/aspnetcore.git
Cloning into 'aspnetcore'...
remote: Enumerating objects: 689408, done.
remote: Counting objects: 100% (541/541), done.
remote: Compressing objects: 100% (330/330), done.
remote: Total 689408 (delta 245), reused 445 (delta 204), pack-reused 688867
Receiving objects: 100% (689408/689408), 404.68 MiB | 1.67 MiB/s, done.
Resolving deltas: 100% (520806/520806), done.
Updating files: 100% (15360/15360), done.
```

depth オプションにより、直近のコミットのみを取得する、シャロークローンを行った場合。

```
PS C:\Users\user\source\repos> git clone https://github.com/dotnet/aspnetcore.git --depth 1
Cloning into 'aspnetcore'...
remote: Enumerating objects: 16996, done.
remote: Counting objects: 100% (16996/16996), done.
remote: Compressing objects: 100% (13016/13016), done.
remote: Total 16996 (delta 4892), reused 7469 (delta 3168), pack-reused 0
Receiving objects: 100% (16996/16996), 16.21 MiB | 2.16 MiB/s, done.
Resolving deltas: 100% (4892/4892), done.
Updating files: 100% (15360/15360), done.
```

後から差分が必要になった場合は、深さを指定するか unshallow を指定することで差分が取れるので特に困ることはない

```
PS C:\Users\user\source\repos> git clone https://github.com/dotnet/aspnetcore.git --depth 1 --branch main
Cloning into 'aspnetcore'...
remote: Enumerating objects: 16996, done.
remote: Counting objects: 100% (16996/16996), done.
remote: Compressing objects: 100% (13016/13016), done.
remote: Total 16996 (delta 4892), reused 7470 (delta 3168), pack-reused 0
Receiving objects: 100% (16996/16996), 16.21 MiB | 2.70 MiB/s, done.
Resolving deltas: 100% (4892/4892), done.
Updating files: 100% (15360/15360), done.
```
