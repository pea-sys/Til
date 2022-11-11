# git-sizer を使ってみる

git-sizer は Github 謹製のツールです。  
https://github.com/github/git-sizer

上記、ReadMe の内容は、リポジトリ管理のチェックリストにも使用出来ると思います。

git は差分管理ではなく、スナップショットでバージョン管理しているため、変更が多く、大きなファイルは git での管理に向いていません。サービスプロバイダーとしても、バージョン管理戦略をきちんと考えて欲しいと思っているのかもしれません。

git-sizer は Git リポジトリのさまざまなサイズ メトリックを計算し、問題や不便を引き起こす可能性のあるものにフラグを付けます。

## [手順]

---

前提条件

- Git コマンドラインクライアントがインストール済であること

---

- 1.次のページからリリースファイルをインストール  
  https://github.com/github/git-sizer/releases

- 2.コマンドプロンプトを起動し、調査したいリポジトリに移動　　
  ※私の場合は、PowerToys を調査

```
cd Powertoys
```

- 3.git-sizer を実行(引数なし)

```
C:\Users\user\source\repos\PowerToys>git-sizer
Processing blobs: 38347
Processing trees: 69523
Processing commits: 7299
Matching commits to trees: 7299
Processing annotated tags: 0
Processing references: 105
| Name                         | Value     | Level of concern               |
| ---------------------------- | --------- | ------------------------------ |
| Biggest objects              |           |                                |
| * Blobs                      |           |                                |
|   * Maximum size         [1] |  16.0 MiB | *                              |
|                              |           |                                |
| Biggest checkouts            |           |                                |
| * Number of directories  [2] |  4.07 k   | **                             |
| * Maximum path depth     [3] |    14     | *                              |
| * Maximum path length    [4] |   195 B   | *                              |

[1]  78cbafb4a7ad5f21dbc2c8db5fc60ce26198e686 (refs/tags/v0.18.2:doc/images/Launcher/QuickStart.gif)
[2]  55a97916a5ed6ae593394639b404aa3a509aec36 (ce942b0585fa7fbc253b4a929cd2ece387b7d7e6^{tree})
[3]  4a376789172b8c336bb48e4f54dedf04acc7b900 (refs/remotes/origin/releaseChecklist^{tree})
[4]  1d3fe97965ac7a8147e201ae25da46e4bafaca81 (5ddfbc1f9a27fdd5cf712ceafa8cf2be56807e07^{tree})
```

- 4.オプションなしだと問題がある項目だけ出力されます。  
  --verbose で全件見れます。

```
C:\Users\user\source\repos\PowerToys>git-sizer --verbose
Processing blobs: 38347
Processing trees: 69523
Processing commits: 7299
Matching commits to trees: 7299
Processing annotated tags: 0
Processing references: 105
| Name                         | Value     | Level of concern               |
| ---------------------------- | --------- | ------------------------------ |
| Overall repository size      |           |                                |
| * Commits                    |           |                                |
|   * Count                    |  7.30 k   |                                |
|   * Total size               |  4.31 MiB |                                |
| * Trees                      |           |                                |
|   * Count                    |  69.5 k   |                                |
|   * Total size               |  24.9 MiB |                                |
|   * Total tree entries       |   636 k   |                                |
| * Blobs                      |           |                                |
|   * Count                    |  38.3 k   |                                |
|   * Total size               |   762 MiB |                                |
| * Annotated tags             |           |                                |
|   * Count                    |     0     |                                |
| * References                 |           |                                |
|   * Count                    |   105     |                                |
|     * Branches               |     1     |                                |
|     * Tags                   |    70     |                                |
|     * Remote-tracking refs   |    34     |                                |
|                              |           |                                |
| Biggest objects              |           |                                |
| * Commits                    |           |                                |
|   * Maximum size         [1] |  23.2 KiB |                                |
|   * Maximum parents      [2] |     2     |                                |
| * Trees                      |           |                                |
|   * Maximum entries      [3] |   302     |                                |
| * Blobs                      |           |                                |
|   * Maximum size         [4] |  16.0 MiB | *                              |
|                              |           |                                |
| History structure            |           |                                |
| * Maximum history depth      |  4.71 k   |                                |
| * Maximum tag depth          |     0     |                                |
|                              |           |                                |
| Biggest checkouts            |           |                                |
| * Number of directories  [5] |  4.07 k   | **                             |
| * Maximum path depth     [6] |    14     | *                              |
| * Maximum path length    [7] |   195 B   | *                              |
| * Number of files        [7] |  3.18 k   |                                |
| * Total size of files    [8] |  74.2 MiB |                                |
| * Number of symlinks         |     0     |                                |
| * Number of submodules   [9] |     4     |                                |

[1]  d9c4abe0df9a88b9c2c93f6fd5ddc10e60e83bed
[2]  a3280f8a0d71823cb9363bbf23369160974ac2c3 (refs/remotes/origin/dev/snickler/upgrade-net7-rc1)
[3]  036d40bff679b2505e9353b0df7304b3e4185f7f (f1ce98eb07f6c95222f7ed591ecfef712bef48ba:PythonHome/Lib)
[4]  78cbafb4a7ad5f21dbc2c8db5fc60ce26198e686 (refs/tags/v0.18.2:doc/images/Launcher/QuickStart.gif)
[5]  55a97916a5ed6ae593394639b404aa3a509aec36 (ce942b0585fa7fbc253b4a929cd2ece387b7d7e6^{tree})
[6]  4a376789172b8c336bb48e4f54dedf04acc7b900 (refs/remotes/origin/releaseChecklist^{tree})
[7]  1d3fe97965ac7a8147e201ae25da46e4bafaca81 (5ddfbc1f9a27fdd5cf712ceafa8cf2be56807e07^{tree})
[8]  7a870255a0e866a13b1f48bdc18c7e5a14a38a99 (refs/tags/v0.18.2^{tree})
[9]  2d599521b0aceeaf8f32dc1c68be63a440b83116 (refs/remotes/origin/releaseChecklist:deps)
```

- 5.Maximum size のコミットを参照します

```
C:\Users\user\source\repos\PowerToys>git show a3280f8a0d71823cb9363bbf23369160974ac2c3
commit a3280f8a0d71823cb9363bbf23369160974ac2c3 (origin/dev/snickler/upgrade-net7-rc1)
Merge: 568f38b80 49007369c
Author: Jeremy Sinclair <4016293+snickler@users.noreply.github.com>
Date:   Fri Nov 4 17:14:22 2022 -0400

    Merge branch 'main' into dev/snickler/upgrade-net7-rc1
```

dotnet ランタイムの変更みたいですね。  
https://github.com/microsoft/PowerToys/pull/20979

[備考]  
github アクションとかで git-sizer を実行するのも良いかもしれない
