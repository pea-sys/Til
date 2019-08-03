#!/usr/bin/python3.4
from __future__ import print_function
from googleapiclient.discovery import build
from httplib2 import Http
from oauth2client import file, client, tools
from time import sleep
from datetime import datetime, date, timedelta
import os
import math
## 特定のアドレスから送られてきたメールのタイトルにshutdownの文字列が含まれていた場合にPC電源OFFします
## ガラケーしか持ち歩いていないので個人的にメール一択
class GmailAPI:
    def __init__(self):
        # If modifying these scopes, delete the file token.json.
        self._SCOPES = r'https://www.googleapis.com/auth/gmail.readonly'

    def ConnectGmail(self):
        store = file.Storage('token.json')
        creds = store.get()
        if not creds or creds.invalid:
            flow = client.flow_from_clientsecrets('googleusercontent.com.json', self._SCOPES)
            creds = tools.run_flow(flow, store)
        service = build('gmail', 'v1', http=creds.authorize(Http()))

        return service

    def GetMessageList(self,DateFrom,DateTo,MessageFrom):

        #APIに接続
        service = self.ConnectGmail()

        MessageList = []

        query = ''
        # 検索用クエリを指定する
        if DateFrom != None and DateFrom !="":
            query += 'after:' + DateFrom + ' '
        if DateTo != None  and DateTo !="":
            query += 'before:' + DateTo + ' '
        if MessageFrom != None and MessageFrom !="":
            query += 'From:' + MessageFrom + ' '

        # メールIDの一覧を取得する(最大100件)
        messageIDlist = service.users().messages().list(userId='me',maxResults=10,q=query).execute()
        #該当するメールが存在しない場合は、処理中断
        if messageIDlist['resultSizeEstimate'] == 0:
            print("Message is not found")
            return MessageList
        #メッセージIDを元に、メールの詳細情報を取得
        for message in messageIDlist['messages']:
            row = {}
            row['ID'] = message['id']
            MessageDetail = service.users().messages().get(userId='me',id=message['id']).execute()
            for header in MessageDetail['payload']['headers']:
                #日付、送信元、件名を取得する
                if header['name'] == 'Date':
                    row['Date'] = header['value']
                elif header['name'] == 'From':
                    row['From'] = header['value']
                elif header['name'] == 'Subject':
                    row['Subject'] = header['value']
            MessageList.append(row)
        return MessageList

if __name__ == '__main__':
    startDate = datetime.now()
    print(datetime(2004, 4, 16,0 ,0, 0).timestamp())
    print(startDate.strftime("%Y/%m/%d %H:%M:%S"))
    print(startDate.timestamp())
    core = GmailAPI()
    while True:
        # Gmail API を使用して、日付でメールの検索を行おうとするとアメリカのタイムゾーンで処理される
        # Unixタイムスタンプ（1970年1月1日午前0時0分0秒からの経過秒数）でも時刻を指定することが可能
        # https://webty.jp/staffblog/production/post-163/
        #messages = core.GetMessageList(DateFrom=("{0:%Y-%m-%d %H:%M:%S}".format(startDate)),
        #                               DateTo=("{0:%Y-%m-%d %H:%M:%S}".format(startDate + timedelta(days=1))),
        #                               MessageFrom='xxxxxx@ezweb.ne.jp')
        messages = core.GetMessageList(DateFrom=(str(math.floor(startDate.timestamp()))),
                                       DateTo=None,
                                       MessageFrom='xxxxxx@ezweb.ne.jp')
        # 結果を出力
        if len(messages) > 0:
            print(messages[0])  # 最後のメッセージ取得

            if 'shutdown' in messages[0]['Subject']:
                break

        print(datetime.now().strftime("%Y/%m/%d %H:%M:%S") + 'sleep')
        sleep(300) #5分置きに監視

    print('shutdown')
    # Windowsの終了
    os.system('shutdown -s -f')

