# Ruby on Rails 環境構築

Ruby 3.2.X に必要

```
sudo apt install -y libssl-dev libreadline-dev zlib1g-dev libyaml-dev
```

Ruby のインストールに必要

```
sudo apt install -y gcc g++ make
```

rbenv インストール

```
cd
git clone https://github.com/sstephenson/rbenv.git .rbenv
git clone https://github.com/sstephenson/ruby-build.git ~/.rbenv/plugins/ruby-build
```

環境変数の設定

```
echo "# For rbenv" >> ~/.bash_profile
echo 'export PATH="$HOME/.rbenv/bin:$PATH"' >> ~/.bash_profile
echo 'eval "$(~/.rbenv/bin/rbenv init -)"' >> ~/.bash_profile
exec $SHELL -l
```

インストール可能な Ruby の確認

```
rbenv install -l
3.1.6
3.2.4
3.3.4
jruby-9.4.8.0
mruby-3.3.0
picoruby-3.0.0
truffleruby-24.0.1
truffleruby+graalvm-24.0.1
```

Ruby インストール

```
rbenv install 3.3.4
```

rbenv の設定

```
rbenv global 3.3.4
ruby -v
ruby 3.3.4 (2024-07-09 revision be1089c8ec) [x86_64-linux]
```

rails のインストール

```
gem install rails
rails -v
Rails 7.1.3.4
```
