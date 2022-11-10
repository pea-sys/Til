# Git-lfs を使ってみる

GitHub は 100 MB を超えるファイルをブロックします。

この制限を超えたファイルを追跡するには、Git Large File Storage (Git LFS) を使用する必要があります。

Github のオフィシャルページにあるように、あまりリポジトリサイズを大きくすると警告のメールが届くこともあるようです。

> リポジトリサイズの制限
> 理想的には 1GB 未満、強くは 5GB 未満を推奨します。小さいリポジトリはクローンが早く、作業やメンテナンスがしやすくなります。あなたのリポジトリが過度に私たちのインフラに影響を与える場合、GitHub サポートから是正措置を求めるメールが届くことがあります。

Git はファイルの差分管理ではなく、スナップショットをとることでバージョン管理を行うため、大きなファイルを扱うと Git サーバの容量を浪費してしまいます。  
Git-lfs を使うことで、大きなファイルをリポジトリとは異なるリモートサーバに格納することができます。リポジトリ側にはそのポイントのみが保存されます。  
ただし、無料アカウントだとリモートサーバに格納できるファイルが 1GB までの制限があります。

### [手順]

- 1.git-lfs.github.com に移動し、 [ダウンロード] をクリックします。
- 2.git-lfs-windows-X.X.X.exe を実行し、インストールします。

* 3.gitbush を開き、git-lfs がインストールされているか確認します。

```
$ git lfs install
Git LFS initialized.
```

- 4.gitbash のカレントディレクトリを Git LFS を利用したい既存のリポジトリに移動します。

- 5.ここでは png を Git lfs にアップロードするように設定します。

```
$ git lfs track "*.png"
Tracking "*.png"
```

- 6.リポジトリに png ファイルを追加

```
git add .
git commit -m "add png"
git push origin imgbot
Uploading LFS objects: 100% (2/2), 406 KB | 0 B/s, done.
Enumerating objects: 9, done.
Counting objects: 100% (9/9), done.
Delta compression using up to 4 threads
Compressing objects: 100% (5/5), done.
Writing objects: 100% (6/6), 685 bytes | 228.00 KiB/s, done.
Total 6 (delta 1), reused 0 (delta 0), pack-reused 0
remote: Resolving deltas: 100% (1/1), completed with 1 local object.
To https://github.com/pea-sys/GitHubActionsExperiments.git
   f1a3fc0..9313492  imgbot -> imgbot

```

- 7.Github で対象ファイルを見ると「Stored with Git LFS」と表示されています。  
  ![github com_pea-sys_GitHubActionsExperiments_blob_imgbot_images_pngwing com png](https://user-images.githubusercontent.com/49807271/199713613-054d34cc-73d7-469b-8060-6a520e859e2b.png)

* 8.リポジトリ直下の.gitattributes を見ると次のルールが記載されています。
  この規則を削ると、対象ファイルの追跡はなくなります。

```
*.png filter=lfs diff=lfs merge=lfs -text
```

- 9.特定のリポジトリで lfs の使用を辞めたい場合は次のコマンドを実行します

```
$ git lfs uninstall
Hooks for this repository have been removed.
Global Git LFS configuration has been removed.
```

- 10.すでにアップロード済みの Git LFS オブジェクトをリポジトリから削除するには、リポジトリを削除して再作成します。 リポジトリを削除すると、関連する Issue、Star、フォークもすべて削除されます。
