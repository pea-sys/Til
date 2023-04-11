# Docker/Kubernetes 実践コンテナ開発入門メモ

次の書籍を読んだ時のメモです。

- Dockrt/Kubernetes 実践コンテナ開発入門 著者:山田明憲 出版社:技術評論社

---

### ■docker image pull の例の jenkins イメージのタグ変更

```shell
docker image pull jenkins
Using default tag: latest
Error response from daemon: manifest for jenkins:latest not found: manifest unknown: manifest unknown
```

latest タグが存在しないため、存在するタグに変更する
タグは以下から確認出来る。  
https://hub.docker.com/r/jenkins/jenkins

```
docker image pull jenkins/jenkins:lts-jdk11
lts-jdk11: Pulling from jenkins/jenkins
3e440a704568: Pull complete
4e7ee60831ad: Pull complete
d81b0ac1dd8f: Pull complete
aa4d217f8f45: Pull complete
328c85129d49: Pull complete
df967cd9dca8: Pull complete
159cf70711c2: Pull complete
Digest: sha256:aacbb5797dd210cc048038d9d3e5ab5795ea018fad843ffc1888c547911819ce
Status: Downloaded newer image for jenkins/jenkins:lts-jdk11===========>     ]  69.63MB/76.93MB
docker.io/jenkins/jenkins:lts-jdk11
```

■Compose による複数コンテナの実行における docker-compose.yml の jenkins イメージのタグ修正

適用なタグを指定します。

```shell
version: "3"
services:
  master:
    container_name: master
    image: jenkins:2.60.3
    ports:
      - 8080:8080
    volumes:
      - ./jenkins_home:/var/jenkins_home
```

■apt update でタイムアウト

Dockerfile でイメージの古いタグを指定すると apt update がタイムアウトする

### Dockerfile

```
FROM ubuntu:16.04

RUN apt update
```

ビルドすると次のような出力でビルドが中断される

```
 > [2/6] RUN apt update:
#5 0.529
#5 0.529 WARNING: apt does not have a stable CLI interface. Use with caution in scripts.
#5 0.529
#5 2.145 Get:1 http://security.ubuntu.com/ubuntu xenial-security InRelease [99.8 kB]
#5 2.170 Get:2 http://archive.ubuntu.com/ubuntu xenial InRelease [247 kB]
#5 122.8 Ign:1 http://security.ubuntu.com/ubuntu xenial-security InRelease
#5 123.1 Ign:2 http://archive.ubuntu.com/ubuntu xenial InRelease
#5 123.9 Get:3 http://security.ubuntu.com/ubuntu xenial-security Release [98.8 kB]
#5 124.3 Get:4 http://archive.ubuntu.com/ubuntu xenial-updates InRelease [99.8 kB]
#5 244.4 Err:3 http://security.ubuntu.com/ubuntu xenial-security Release
#5 244.4   Connection timed out [IP: 185.125.190.36 80]
#5 244.9 Ign:4 http://archive.ubuntu.com/ubuntu xenial-updates InRelease
#5 246.2 Get:5 http://archive.ubuntu.com/ubuntu xenial-backports InRelease [97.4 kB]
#5 366.8 Ign:5 http://archive.ubuntu.com/ubuntu xenial-backports InRelease
#5 367.3 Get:6 http://archive.ubuntu.com/ubuntu xenial Release [246 kB]
#5 488.3 Err:6 http://archive.ubuntu.com/ubuntu xenial Release
#5 488.3   Connection timed out [IP: 185.125.190.36 80]
```

イメージのタグを新しいものに更新すれば解決する。

■docker swarm で repository does not exist

```
Error response from daemon: pull access denied for docker18.05.0-ce-dind, repository does not exist or may require 'docker login': denied: requested access to the resource is denied
```

docker login では治らず  
docker hub でパッケージを検索するとリポジトリが存在しなかったので  
rancher/dind に置き換えました。
