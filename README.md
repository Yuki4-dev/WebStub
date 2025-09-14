# WebStub 
Web開発者向けのWindowsデスクトップアプリです。指定ポートでローカルサーバを立ち上げ、リクエストが来た際にエディター内のJavaScriptを実行してレスポンスを返します。Web開発のローカルサーバスタブとして活用できます。

## WinUI3

#### Dark
<img width="1611" height="909" alt="DARK" src="https://github.com/user-attachments/assets/2a669a71-0c8e-4396-acf6-1cc7554b4701" />

#### Light
<img width="1630" height="893" alt="LIGHT" src="https://github.com/user-attachments/assets/bb0578df-44ed-4580-b1a7-3949bb390fb2" />

## WPF
![image](https://github.com/user-attachments/assets/aa98ef73-fabd-4581-8a31-e2a3731a0bc1)

### 機能 
 - 指定したポートでローカルサーバを立ち上げます。
 - リクエスト内容を受け取るJavascriptを動的に実行し、任意のレスポンスを返却します。
 - ファイルパスを入力することで、ファイルの内容を読み取り返却することもできます。
 - リクエスト内容やレスポンスをログとして出力するので確認できます。

## 例

### ローカルサーバ起動
![image](https://github.com/user-attachments/assets/6e6cfd03-9696-41a6-bb0b-33e0cafef7f2)

### リクエスト送信

```
curl -X GET http://localhost:8080/sample/get

﻿Access Sample Get!
```

```
curl -X POST http://localhost:8080/sample/post -H "Content-Type: application/json" -d "{\"key1\":\"value1\", \"key2\":\"value2\"}"

Access Sample Post! -> value1
```

### LOG
```
[Info]2025/02/03 21:48:47 : Server start. (Method : OpenServerAsync, line : 68, path : WebStub\ViewModel\WebStubViewModel.cs )
[Info]2025/02/03 21:48:47 : Open server -> http://localhost:8080/ (Method : ListenAsync, line : 29, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:48:47 : Listen server -> http://localhost:8080/ (Method : ListenAsync, line : 34, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:52 : Recieve Url -> http://localhost:8080/sample/get (Method : Server, line : 60, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:52 : Remote EndPoint -> [::1]:52516 (Method : Server, line : 61, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:52 : Local EndPoint -> [::1]:8080 (Method : Server, line : 62, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:52 : Content Length -> 0 (Method : Server, line : 63, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:52 : Request -> {"Method":"GET","Uri":"/sample/get","Body":"","Headers":[{"Key":"Accept","Values":["*/*"]},{"Key":"Host","Values":["localhost:8080"]},{"Key":"User-Agent","Values":["curl/8.10.1"]}],"Cookies":[],"Parameters":[]} (Method : Server, line : 106, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:52 : Run JavaScript. (Method : Server, line : 111, path : WebStub\ViewModel\WebStubViewModel.cs )
[Info]2025/02/03 21:49:52 : JavaScript result -> {"body":"Access Sample Get!","status":200} (Method : Server, line : 115, path : WebStub\ViewModel\WebStubViewModel.cs )
[Info]2025/02/03 21:49:52 : Response -> {"Status":200,"Body":"Access Sample Get!","Headers":[],"Cookies":[]} (Method : Server, line : 109, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:57 : Recieve Url -> http://localhost:8080/sample/post (Method : Server, line : 60, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:57 : Remote EndPoint -> [::1]:52525 (Method : Server, line : 61, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:57 : Local EndPoint -> [::1]:8080 (Method : Server, line : 62, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:57 : Content Length -> 34 (Method : Server, line : 63, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:57 : Request -> {"Method":"POST","Uri":"/sample/post","Body":"{\u0022key1\u0022:\u0022value1\u0022, \u0022key2\u0022:\u0022value2\u0022}","Headers":[{"Key":"Content-Length","Values":["34"]},{"Key":"Content-Type","Values":["application/json"]},{"Key":"Accept","Values":["*/*"]},{"Key":"Host","Values":["localhost:8080"]},{"Key":"User-Agent","Values":["curl/8.10.1"]}],"Cookies":[],"Parameters":[]} (Method : Server, line : 106, path : WebStub\Services\HttpService.cs )
[Info]2025/02/03 21:49:57 : Run JavaScript. (Method : Server, line : 111, path : WebStub\ViewModel\WebStubViewModel.cs )
[Info]2025/02/03 21:49:57 : JavaScript result -> {"body":"Access Sample Post! -> value1","status":200} (Method : Server, line : 115, path : WebStub\ViewModel\WebStubViewModel.cs )
[Info]2025/02/03 21:49:57 : Response -> {"Status":200,"Body":"Access Sample Post! -\u003E value1","Headers":[],"Cookies":[]} (Method : Server, line : 109, path : WebStub\Services\HttpService.cs )
```
