# agency_api

<br> 
## 使用C#语言 restfulf风格的WebApi 开发的代理接口 用于转发前端请求
<br> 

###使用
`使用VS2012及以上版本打开项目，编译发布到iis服务器`
<br>
 
### 格式
## post：http://127.0.0.1/发布应用程序名称/Api/Agency/post?url=需要代理接口的地址
#  参数：json格式
<br>

## get: http://127.0.0.1/发布应用程序名称/Api/Agency/get?url=需要代理接口的地址?param1=str1&param2=str2



### 栗子
## http://127.0.0.1/agency/Api/Agency/Post?url=http://www.baidu.com/weather/getWeather
   {longitude:100.1,latitude:22.1}
