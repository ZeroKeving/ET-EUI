using System;


namespace ET
{
    public static class LoginHelper
    {
        /// <summary>
        /// 登录游戏
        /// </summary>
        /// <param name="zoneScene"></param>
        /// <param name="address"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password)
        {
            A2C_LoginAccout a2C_LoginAccout = null;//服务器回复的登录消息
            Session accountSession = null;//登录服务器会话链接

            try
            {
                accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));//使用address地址来连接登录服务器
                password = MD5Helper.StringMD5(password);//MD5加密
                a2C_LoginAccout = (A2C_LoginAccout)await accountSession.Call(new C2A_LoginAccount() { AccountName = account, Password = password });//向登录服务器发送账号密码信息，并等待服务器返回消息
            }
            catch (Exception e)
            {
                accountSession?.Dispose();//断开连接
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;//弹出网络错误码
            }

            if (a2C_LoginAccout.Error != ErrorCode.ERR_Success)//如果判定登录请求失败
            {
                accountSession?.Dispose();//断开连接
                return a2C_LoginAccout.Error;//登录失败的提示
            }

            zoneScene.AddComponent<SessionComponent>().Session = accountSession;//将连接进行保存
            zoneScene.GetComponent<SessionComponent>().Session.AddComponent<PingComponent>();//增加一个ping组件(会每隔一定时间给服务器发送一条心跳包，防止服务器将其断开)

            //将登录信息保存到客户端登录信息组件
            zoneScene.GetComponent<AccountInfoComponent>().Token = a2C_LoginAccout.Token;
            zoneScene.GetComponent<AccountInfoComponent>().AccountId = a2C_LoginAccout.AccountId;


            return ErrorCode.ERR_Success;//登录成功的提示
        }

        /// <summary>
        /// 获取游戏服务器区服信息
        /// </summary>
        /// <param name="zoneScene"></param>
        /// <returns></returns>
        public static async ETTask<int> GetServerInfos(Scene zoneScene)
        {
            A2C_GetSeverInfos a2C_GetSeverInfos = null;

            try
            {
                //向登录服务器发送获取服务器区服信息请求并等待回复
                a2C_GetSeverInfos = (A2C_GetSeverInfos)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetSeverInfos()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                });

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;//返回网络错误码
            }

            if (a2C_GetSeverInfos.Error != ErrorCode.ERR_Success)//如果有错误信息,就返回错误信息
            {
                return a2C_GetSeverInfos.Error;
            }

            foreach (var serverInfoProto in a2C_GetSeverInfos.serverInfoList)//将消息里的游戏服务器区服遍历出来
            {
                ServerInfo serverInfo = zoneScene.GetComponent<ServerInfosComponent>().AddChild<ServerInfo>();//创建一个实体挂在游戏区服信息组件下面
                serverInfo.FromMessage(serverInfoProto);//将消息的内容提取到实体内
                zoneScene.GetComponent<ServerInfosComponent>().Add(serverInfo);//将遍历出来的游戏服务器区服添加进游戏服务器区服组件内
            }

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 创建游戏角色
        /// </summary>
        /// <param name="zoneScene"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async ETTask<int> CreateRole(Scene zoneScene, string name)
        {
            A2C_CreateRole a2C_CreateRole = null;

            try
            {
                a2C_CreateRole = (A2C_CreateRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_CreateRole()//向登录服务器发送消息
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                    Name = name,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;//网络错误
            }

            if (a2C_CreateRole.Error != ErrorCode.ERR_Success)//是否报错
            {
                Log.Error(a2C_CreateRole.Error.ToString());
                return a2C_CreateRole.Error;
            }

            //将服务器返回的角色信息添加到角色信息组件
            RoleInfo newRoleInfo = zoneScene.GetComponent<RoleInfosComponent>().AddChild<RoleInfo>();
            newRoleInfo.FormMessage(a2C_CreateRole.RoleInfo);
            zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Add(newRoleInfo);

            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 删除选中的游戏角色
        /// </summary>
        /// <param name="zoneScene"></param>
        /// <returns></returns>
        public static async ETTask<int> DeletRole(Scene zoneScene)
        {
            A2C_DeleteRole a2C_DeleteRole = null;
            try
            {
                a2C_DeleteRole = (A2C_DeleteRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_DeleteRole()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                    RoleInfoId = zoneScene.GetComponent<RoleInfosComponent>().CurrentRoleId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;//网络错误
            }

            if (a2C_DeleteRole.Error != ErrorCode.ERR_Success)//是否报错
            {
                Log.Error(a2C_DeleteRole.Error.ToString());
                return a2C_DeleteRole.Error;
            }

            int index = zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.FindIndex((info) => { return info.Id == a2C_DeleteRole.DeletedRoleInfoId; });//在角色列表里找到指定角色的索引

            zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.RemoveAt(index);//删除指定索引的角色

            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 删除该服务器所有游戏角色
        /// </summary>
        /// <param name="zoneScene"></param>
        /// <returns></returns>
        public static async ETTask<int> DeletAllRole(Scene zoneScene)
        {
            A2C_DeleteAllRole a2C_DeleteAllRole = null;
            try
            {
                a2C_DeleteAllRole = (A2C_DeleteAllRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_DeleteAllRole()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;//网络错误
            }

            if (a2C_DeleteAllRole.Error != ErrorCode.ERR_Success)//是否报错
            {
                Log.Error(a2C_DeleteAllRole.Error.ToString());
                return a2C_DeleteAllRole.Error;
            }

            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 获取游戏角色
        /// </summary>
        /// <param name="zoneScene"></param>
        /// <returns></returns>
        public static async ETTask<int> GetRoles(Scene zoneScene)
        {
            A2C_GetRoles a2C_GetRoles = null;

            try
            {
                a2C_GetRoles = (A2C_GetRoles)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRoles()//向客户端发送获取游戏角色请求
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_GetRoles.Error != ErrorCode.ERR_Success)//判断是否报错
            {
                Log.Error(a2C_GetRoles.Error.ToString());
                return a2C_GetRoles.Error;
            }

            if (a2C_GetRoles.RoleInfo == null || a2C_GetRoles.RoleInfo.Count == 0)//如果传回来没有查询到角色信息
            {
                return HintCode.RoleIsNull;//返回提示该账号未创建角色
            }

            zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Clear();//清理角色信息组件
            foreach (var roleInfoRroto in a2C_GetRoles.RoleInfo)//将获取到的消息添加到角色信息组件
            {
                RoleInfo roleInfo = zoneScene.GetComponent<RoleInfosComponent>().AddChild<RoleInfo>();
                roleInfo.FormMessage(roleInfoRroto);
                zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Add(roleInfo);
            }
            zoneScene.GetComponent<RoleInfosComponent>().CurrentRoleId = a2C_GetRoles.RoleInfo[0].Id;//将当前默认登录的角色设置为第一个角色

            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 获取Realm网关负载均衡服务器的令牌
        /// </summary>
        /// <param name="zoneScene"></param>
        /// <returns></returns>
        public static async ETTask<int> GetRealmKey(Scene zoneScene)
        {
            A2C_GetRealmKey a2C_GetRealmKey = null;

            try
            {
                a2C_GetRealmKey = (A2C_GetRealmKey)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRealmKey()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                });

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_GetRealmKey.Error != ErrorCode.ERR_Success)//判断是否报错
            {
                Log.Error(a2C_GetRealmKey.Error.ToString());
                return a2C_GetRealmKey.Error;
            }

            //账号信息组件保存返回回来的Realm网关负载均衡服务器的令牌和地址
            zoneScene.GetComponent<AccountInfoComponent>().RealmKey = a2C_GetRealmKey.RealmKey;
            zoneScene.GetComponent<AccountInfoComponent>().RealmAddress = a2C_GetRealmKey.RealmAddress;
            zoneScene.GetComponent<SessionComponent>().Session.Dispose();//获取Realm网关负载均衡服务器的令牌和地址后，就要吧登录服务器的会话连接断开

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 进入游戏
        /// </summary>
        /// <param name="zoneScene"></param>
        /// <returns></returns>
        public static async ETTask<int> EnterGame(Scene zoneScene)
        {
            string realmAddress = zoneScene.GetComponent<AccountInfoComponent>().RealmAddress;
            //流程1：连接Realm,获取分配的Gate网关
            R2C_LoginRealm r2C_LoginRealm = null;

            Session session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(realmAddress));//向Realm服务器发起通信

            try
            {
                r2C_LoginRealm = (R2C_LoginRealm)await session.Call(new C2R_LoginRealm()//请求分配Gate网关
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    RealmTokenKey = zoneScene.GetComponent<AccountInfoComponent>().RealmKey,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                session?.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }
            session?.Dispose();//断开连接

            if(r2C_LoginRealm.Error != ErrorCode.ERR_Success)//返回报错
            {
                return r2C_LoginRealm.Error;
            }

            //流程2：开始连接Gate网关
            Log.Warning($"GateAddress:{r2C_LoginRealm.GateAddress}");
            Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2C_LoginRealm.GateAddress));//向Gate网关服务器发起连接
            gateSession.AddComponent<PingComponent>();//增加一个ping组件(会每隔一定时间给服务器发送一条心跳包，防止服务器将其断开)
            zoneScene.GetComponent<SessionComponent>().Session = gateSession;//将其连接保存

            long currentRoleId = zoneScene.GetComponent<RoleInfosComponent>().CurrentRoleId;
            G2C_LoginGameGate g2C_LoginGameGate = null;
            try
            {
                long accountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId;
                //登录Gate网关服务器
                g2C_LoginGameGate = (G2C_LoginGameGate)await gateSession.Call(new C2G_LoginGameGate()
                {
                    Key = r2C_LoginRealm.GateSessionKey,
                    AccountId = accountId,
                    RoleId = currentRoleId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();//断开连接
                return ErrorCode.ERR_NetWorkError;
            }

            if(g2C_LoginGameGate.Error != ErrorCode.ERR_Success)//如果报错
            {
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return g2C_LoginGameGate.Error;
            }

            Log.Debug("登录Gate成功");

            //流程3：角色正式进入游戏逻辑服


            return ErrorCode.ERR_Success;
        }
    }
}