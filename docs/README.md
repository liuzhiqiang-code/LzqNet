展示微服务各个组件之间最佳拆分实践+最简+标准

# 第一步
## docker-compose
应用程序的镜像化部署 + postgres，redis，redis-commander，jaeger，loki，promtail，prometheus，grafana,nginx等
用了nginx所有内部服务全部用http协议，nginx用https协议

## 微服务公用技术栈项目
src/InfrastructureServices
LzqNet.ApiGateway     网关    yarp+RateLimiter限流

LzqNet.SwaggerUI   SwaggerUI统一入口
SwaggerUI 内部代理方式内网取到  各服务OpenApiJson，集中展示接口文档
各服务OpenApiJson都不暴露外网+授权访问OpenApiJson

LzqNet.Identity   认证服务器  
各服务认证 + 单点登录  + jwt

LzqNet.DCC    配置中心    现在主要是当个SDK使用，后面看情况切入consul或其他配置中心
所有项目配置文件统一放在这里管理     
统一AddJsonFile引入     测试环境，业务可在自己appsettings里覆盖配置

LzqNet.Extensions    公用扩展类库
封装 HealthCheck+配置中心+日志+认证授权+可观测性等一些 业务服务公用的中间件

LzqNet.HealthCheckUI    健康检查UI界面     https协议
基础资源检查   postgres，redis，redis-commander，jaeger，loki等云资源
服务状态检查   验证各服务是否正常运行

src/Contracts 各业务服务接口请求/响应报文  类库
src/Callers  各项目相互调用的SDK库，各业务服务之间相互调用的服务都通过Caller抽象出来，避免业务服务内部重复维护
src/BusinessServices   各业务服务，如订单，库存，采购等   http+内网+授权访问+相互访问不走网关+token穿透  保证安全性及响应效率

# 第二步
引入授权模块 LzqNet.Auth  前后端一起，标准OAuth2.0授权码模式，微信登录等方式
rambbitmq，kafka等消息队列
分布式事务  Saga模式

# 第三步
引入支付模块 LzqNet.Payment
支付成功事件通过RabbitMQ/Kafka通知订单服务
使用事务性消息保证数据一致性
幂等性处理（防止重复支付）
分布式事务（Saga模式）
支付渠道的适配器模式
对账补偿机制