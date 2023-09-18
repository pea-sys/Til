# wsl 起動時のメッセージ抑制

wsl 起動時に次のメッセージが出る

```
PS C:\Windows\System32> wsl
Welcome to Ubuntu 22.04.2 LTS (GNU/Linux 5.15.90.1-microsoft-standard-WSL2 x86_64)

 * Documentation:  https://help.ubuntu.com
 * Management:     https://landscape.canonical.com
 * Support:        https://ubuntu.com/advantage

 * Strictly confined Kubernetes makes edge and IoT secure. Learn how MicroK8s
   just raised the bar for easy, resilient and secure K8s cluster deployment.

   https://ubuntu.com/engage/secure-kubernetes-at-the-edge

This message is shown once a day. To disable it please create the /home/masami/.hushlogin file.
```

記載のある通り、指定場所に/hashlogin ファイルを作れば良い。

```
masami@DESKTOP-L18OTEK:/mnt/c/Windows/System32$ touch /home/masami/.hushlogin
```

確認

```
masami@DESKTOP-L18OTEK:/mnt/c/Windows/System32$ exit
logout
PS C:\Windows\System32> wsl
masami@DESKTOP-L18OTEK:/mnt/c/Windows/System32$
```
