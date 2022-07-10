- prepare-commit-msg でメッセージにブランチ名を挿入する

- git/hook 配下に prepare-commit-msg スクリプトを配置する

```
#!/bin/bash

readonly COMMIT_MSG_FILE=$1
readonly COMMIT_SOURCE=$2

case "${COMMIT_SOURCE}" in
  commit) # use -c/-C/--amend
    : # 何もしない
  ;;
  *)
    # ブランチ名取得
    branch_name=$(git name-rev --name-only HEAD)
    # ブランチ名をコミットメッセージの前に挿入する
    perl -p -i.bak -e "s/^/(${branch_name})/g"  $COMMIT_MSG_FILE
  ;;
esac
```

- git bash で commit と実行

```pwsh
PS C:\Users\user\source\repos\playground> git add .
warning: in the working copy of 'prepare-commit-msg', LF will be replaced by CRLF the next time Git touches it
PS C:\Users\user\source\repos\playground> git commit -m "git-client-hook pre-commit-msg"
[main d5efa86] (main)git-client-hook pre-commit-msg
 2 files changed, 23 insertions(+), 40 deletions(-)
 create mode 100644 ReadMe.md
PS C:\Users\user\source\repos\playground> git log
commit d5efa86c18c909304eb7a2cfde5c24169e918eea (HEAD -> main)
Author: pea-sys <pea98258@gmail.com>
Date:   Sat Jun 10 22:15:22 2023 +0900

    (main)git-client-hook pre-commit-msg
```

ちゃんと、ブランチ名(main)が挿入されている。

- tortoiseGit でも同様にブランチ名が挿入されている。

![TortoiseGit](https://github.com/pea-sys/Til/assets/49807271/6cacfd17-5584-4b34-b955-55cf9c0abb64)

![TortoiseGit](https://github.com/pea-sys/Til/assets/49807271/137fa072-b9db-42d5-9140-f97de2bd02d6)
