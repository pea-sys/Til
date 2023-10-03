# wsl に fish を導入

wsl に fish を導入。wsl 固有の操作はありません。  
https://fishshell.com/

```sh
masami@DESKTOP-L18OTEK:/mnt/c/Windows/system32$ sudo apt-add-repository ppa:fish-shell/release-3
[sudo] password for masami:
Repository: 'deb https://ppa.launchpadcontent.net/fish-shell/release-3/ubuntu/ jammy main'
Description:
This archive contains releases made from the Fish 3.x series.

To install fish, run the following commands:

sudo apt-add-repository ppa:fish-shell/release-3
sudo apt update
sudo apt install fish
More info: https://launchpad.net/~fish-shell/+archive/ubuntu/release-3
Adding repository.
Press [ENTER] to continue or Ctrl-c to cancel.
Adding deb entry to /etc/apt/sources.list.d/fish-shell-ubuntu-release-3-jammy.list
Adding disabled deb-src entry to /etc/apt/sources.list.d/fish-shell-ubuntu-release-3-jammy.list
Adding key to /etc/apt/trusted.gpg.d/fish-shell-ubuntu-release-3.gpg with fingerprint 59FDA1CE1B84B3FAD89366C027557F056DC33CA5
Get:1 http://security.ubuntu.com/ubuntu jammy-security InRelease [110 kB]
Hit:2 http://archive.ubuntu.com/ubuntu jammy InRelease
Get:3 http://archive.ubuntu.com/ubuntu jammy-updates InRelease [119 kB]
Get:4 http://security.ubuntu.com/ubuntu jammy-security/main amd64 Packages [822 kB]
Get:5 https://ppa.launchpadcontent.net/fish-shell/release-3/ubuntu jammy InRelease [17.6 kB]
Get:6 http://archive.ubuntu.com/ubuntu jammy-backports InRelease [109 kB]
Get:7 http://archive.ubuntu.com/ubuntu jammy-updates/main amd64 Packages [1022 kB]
Get:8 http://security.ubuntu.com/ubuntu jammy-security/main Translation-en [171 kB]
Get:9 http://security.ubuntu.com/ubuntu jammy-security/main amd64 c-n-f Metadata [11.3 kB]
Get:10 http://security.ubuntu.com/ubuntu jammy-security/restricted amd64 Packages [899 kB]
Get:11 http://security.ubuntu.com/ubuntu jammy-security/restricted Translation-en [145 kB]
Get:12 http://security.ubuntu.com/ubuntu jammy-security/universe amd64 Packages [786 kB]
Get:13 http://security.ubuntu.com/ubuntu jammy-security/universe Translation-en [144 kB]
Get:14 http://security.ubuntu.com/ubuntu jammy-security/universe amd64 c-n-f Metadata [16.7 kB]
Get:15 https://ppa.launchpadcontent.net/fish-shell/release-3/ubuntu jammy/main amd64 Packages [580 B]
Get:16 http://archive.ubuntu.com/ubuntu jammy-updates/main Translation-en [229 kB]
Get:17 http://archive.ubuntu.com/ubuntu jammy-updates/main amd64 c-n-f Metadata [15.6 kB]
Get:18 http://archive.ubuntu.com/ubuntu jammy-updates/restricted amd64 Packages [915 kB]
Get:19 http://archive.ubuntu.com/ubuntu jammy-updates/restricted Translation-en [147 kB]
Get:20 http://archive.ubuntu.com/ubuntu jammy-updates/universe amd64 Packages [987 kB]
Get:21 https://ppa.launchpadcontent.net/fish-shell/release-3/ubuntu jammy/main Translation-en [288 B]
Get:22 http://archive.ubuntu.com/ubuntu jammy-updates/universe Translation-en [216 kB]
Get:23 http://archive.ubuntu.com/ubuntu jammy-updates/universe amd64 c-n-f Metadata [21.9 kB]
Fetched 6903 kB in 8s (876 kB/s)
Reading package lists... Done
masami@DESKTOP-L18OTEK:/mnt/c/Windows/system32$ sudo apt update
Hit:1 http://archive.ubuntu.com/ubuntu jammy InRelease
Hit:2 http://security.ubuntu.com/ubuntu jammy-security InRelease
Hit:3 http://archive.ubuntu.com/ubuntu jammy-updates InRelease
Hit:4 http://archive.ubuntu.com/ubuntu jammy-backports InRelease
Hit:5 https://ppa.launchpadcontent.net/fish-shell/release-3/ubuntu jammy InRelease
Reading package lists... Done
Building dependency tree... Done
Reading state information... Done
54 packages can be upgraded. Run 'apt list --upgradable' to see them.
masami@DESKTOP-L18OTEK:/mnt/c/Windows/system32$ sudo apt install fish
Reading package lists... Done
Building dependency tree... Done
Reading state information... Done
The following additional packages will be installed:
  libpcre2-32-0 xsel
Suggested packages:
  xdg-utils
The following NEW packages will be installed:
  fish libpcre2-32-0 xsel
0 upgraded, 3 newly installed, 0 to remove and 54 not upgraded.
Need to get 2729 kB of archives.
After this operation, 16.3 MB of additional disk space will be used.
Do you want to continue? [Y/n] y
Get:1 http://archive.ubuntu.com/ubuntu jammy-updates/main amd64 libpcre2-32-0 amd64 10.39-3ubuntu0.1 [194 kB]
Get:2 https://ppa.launchpadcontent.net/fish-shell/release-3/ubuntu jammy/main amd64 fish amd64 3.6.1-1~jammy [2515 kB]
Get:3 http://archive.ubuntu.com/ubuntu jammy/universe amd64 xsel amd64 1.2.0+git9bfc13d.20180109-3 [20.5 kB]
Fetched 2729 kB in 4s (716 kB/s)
Selecting previously unselected package libpcre2-32-0:amd64.
(Reading database ... 30235 files and directories currently installed.)
Preparing to unpack .../libpcre2-32-0_10.39-3ubuntu0.1_amd64.deb ...
Unpacking libpcre2-32-0:amd64 (10.39-3ubuntu0.1) ...
Selecting previously unselected package fish.
Preparing to unpack .../fish_3.6.1-1~jammy_amd64.deb ...
Unpacking fish (3.6.1-1~jammy) ...
Selecting previously unselected package xsel.
Preparing to unpack .../xsel_1.2.0+git9bfc13d.20180109-3_amd64.deb ...
Unpacking xsel (1.2.0+git9bfc13d.20180109-3) ...
Setting up xsel (1.2.0+git9bfc13d.20180109-3) ...
Setting up libpcre2-32-0:amd64 (10.39-3ubuntu0.1) ...
Setting up fish (3.6.1-1~jammy) ...
Processing triggers for man-db (2.10.2-1) ...
Processing triggers for libc-bin (2.35-0ubuntu3.1) ...
masami@DESKTOP-L18OTEK:/mnt/c/Windows/system32$ fish
Welcome to fish, the friendly interactive shell
Type help for instructions on how to use fish
masami@DESKTOP-L18OTEK /m/c/W/system32> which fish
/usr/bin/fish
masami@DESKTOP-L18OTEK /m/c/W/system32> chsh -s /usr/bin/fish
Password:
masami@DESKTOP-L18OTEK /m/c/W/system32> curl -sL https://raw.githubusercontent.com/jorgebucaran/fisher/main/functions/fi
sher.fish | source && fisher install jorgebucaran/fisher
fisher install version 4.4.4
Fetching https://api.github.com/repos/jorgebucaran/fisher/tarball/HEAD
Installing jorgebucaran/fisher
           /home/masami/.config/fish/functions/fisher.fish
           /home/masami/.config/fish/completions/fisher.fish
Installed 1 plugin/s
masami@DESKTOP-L18OTEK /m/c/W/system32>
```
