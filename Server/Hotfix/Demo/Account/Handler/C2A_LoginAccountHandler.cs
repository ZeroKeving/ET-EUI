
using System;
using System.Text.RegularExpressions;

namespace ET
{
    /// <summary>
    /// 客户端与服务器通信时的账号登录消息处理：普通回复消息处理<接受消息类型,回复消息类型>
    /// </summary>
    public class C2A_LoginAccountHandler : AMRpcHandler<C2A_LoginAccount, A2C_LoginAccout>
    {
        protected override async ETTask Run(Session session, C2A_LoginAccount request, A2C_LoginAccout response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)//这条请求消息是否请求到的是登录服务器,如果不是则断开链接
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();//断开连接
                return;
            }

            session.RemoveComponent<SessionAcceptTimeoutComponent>();//移除链接5秒超时组件（代表链接通过了验证，如果没有通过验证该组件5秒后会断开链接）

            if (session.GetComponent<SessionLockingComponent>() != null)//如果会话组件锁不为空的情况下，说明服务器接收到客户端多次请求
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;//返回登录请求多次错误码
                reply();//委托发送回复消息response
                session.Disconnect().Coroutine();//延迟1秒后断开连接
                return;
            }

            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))//判断账号密码是否为空
            {
                response.Error = ErrorCode.ERR_LoginInfoIsNull;//返回登录信息错误码
                reply();//委托发送回复消息response
                session.Disconnect().Coroutine();//延迟1秒后断开连接
                return;
            }

            if (!Regex.IsMatch(request.AccountName.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Za-z].*).{6,15}$"))//通过正则表达式，判断账号是否属于数字加大小写字母组合而成,如果不是则
            {
                response.Error = ErrorCode.ERR_LoginAccountNameFormError;//返回登录账号格式错误码
                reply();//委托发送回复消息response
                session.Disconnect().Coroutine();//延迟1秒后断开连接
                return;
            }

            if (!Regex.IsMatch(request.Password.Trim(), @"^[A-Za-z0-9]+$"))//判断密码是否属于数字加大小写字母组合而成,如果不是则
            {
                response.Error = ErrorCode.ERR_LoginPasswordFormError;//返回登录密码格式错误码
                reply();//委托发送回复消息response
                session.Disconnect().Coroutine();//延迟1秒后断开连接
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())//会话组件锁(因为下面用到了异步逻辑),同一个用户的会话组件重复访问时会抢这把锁
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.AccountName.Trim().GetHashCode()))//协程锁，所有用户都会抢这把锁，传入唯一id,用户账号名的哈希值（防止不同地址的用户同时用相同的账号去登录）
                {
                    //查询数据库中是否有这个请求登录的账号
                    var accountInfoList = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<Account>(d => d.AccountName.Equals(request.AccountName.Trim()));//DomainZone获取的是游戏在第几个服务器
                    Account account = null;
                    if (accountInfoList != null && accountInfoList.Count > 0)//如果返回值大于0，则表示查询到了
                    {
                        account = accountInfoList[0];
                        if (account.AccountType == (int)AccountType.BlackList)//如果账号在黑名单中
                        {
                            response.Error = ErrorCode.ERR_LoginBlackListError;//返回登录账号属于黑名单错误码
                            reply();//委托发送回复消息response
                            session.Disconnect().Coroutine();//延迟1秒后断开连接
                            account?.Dispose();//临时的实体变量要记得释放
                            return;
                        }

                        if (!account.Password.Equals(request.Password))//判断密码是否正确，如果不是则
                        {
                            response.Error = ErrorCode.ERR_LoginPasswordError;//返回登录密码错误码
                            reply();//委托发送回复消息response
                            session.Disconnect().Coroutine();//延迟1秒后断开连接
                            account?.Dispose();//临时的实体变量要记得释放
                            return;
                        }

                        session.AddChild(account);//把账号信息放入会话里

                    }
                    else//如果没有查询到就自动注册
                    {
                        account = session.AddChild<Account>();//创建出新的Account实体
                        account.AccountName = request.AccountName.Trim();//删除头部和尾部的空字符
                        account.Password = request.Password.Trim();
                        account.CreateTime = TimeHelper.ServerNow();//创建时间
                        account.AccountType = (int)AccountType.General;//普通类型账号
                        await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save<Account>(account);//将账号信息存入数据库
                    }

                    StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "LoginCenter");//获取登录中心服务器的配置
                    long loginCenterInstanceId = startSceneConfig.InstanceId;//获取登录中心服务器的实例id
                    L2A_LoginAccountResponse loginAccountResponse = (L2A_LoginAccountResponse)await ActorMessageSenderComponent.Instance.Call(loginCenterInstanceId, new A2L_LoginAccountRequest() { AccountId = account.Id });//向登录中心服务器发送消息并等待回复
                    if (loginAccountResponse.Error != ErrorCode.ERR_Success)//如果发生了错误则断开连接
                    {
                        response.Error = loginAccountResponse.Error;//将账号中心服务器的错误发回给客户端
                        reply();//委托发送回复消息response
                        session?.Disconnect().Coroutine();//延迟1秒后断开连接
                        account.Dispose();//释放临时的实体变量
                        return;
                    }

                    //顶号流程
                    long accountSessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(account.Id);//获取登录会话组件里当前登录账号的会话id
                    Session otherSession = Game.EventSystem.Get(accountSessionInstanceId) as Session;//使用会话id获取会话
                    otherSession?.Send(new A2C_Disconnect() { Error = 0 });//向客户端发送顶号断线信息
                    otherSession?.Disconnect().Coroutine();//延迟1秒后断开连接
                    session.DomainScene().GetComponent<AccountSessionsComponent>().Add(account.Id, session.InstanceId);//将新的会话添加进登录会话组件里
                    session.AddComponent<AccountCheckOutTimeComponent, long>(account.Id);//10分钟后踢出不活跃用户（应对特殊情况，例如手机没电了，没有及时通知服务器掉线）

                    string Token = TimeHelper.ServerNow().ToString() + RandomHelper.RandomNumber(int.MinValue, int.MaxValue).ToString();//创建一个登录标志令牌
                    session.DomainScene().GetComponent<TokenComponent>().Remove(account.Id);//移除当前登录id过去曾经使用过的令牌
                    session.DomainScene().GetComponent<TokenComponent>().Add(account.Id, Token);//添加当前登录id的登录令牌

                    response.AccountId = account.Id;
                    response.Token = Token;

                    reply();//委托发送回复消息response
                    account?.Dispose();//释放临时的实体变量

                }

            }

        }
    }
}