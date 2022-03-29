
using System;
using System.Text.RegularExpressions;

namespace ET
{
    /// <summary>
    /// �ͻ����������ͨ��ʱ���˺ŵ�¼��Ϣ������ͨ�ظ���Ϣ����<������Ϣ����,�ظ���Ϣ����>
    /// </summary>
    public class C2A_LoginAccountHandler : AMRpcHandler<C2A_LoginAccount, A2C_LoginAccout>
    {
        protected override async ETTask Run(Session session, C2A_LoginAccount request, A2C_LoginAccout response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)//����������Ϣ�Ƿ����󵽵��ǵ�¼������,���������Ͽ�����
            {
                Log.Error($"�����Scene���󣬵�ǰSceneΪ��{session.DomainScene().SceneType}");
                session.Dispose();//�Ͽ�����
                return;
            }

            session.RemoveComponent<SessionAcceptTimeoutComponent>();//�Ƴ�����5�볬ʱ�������������ͨ������֤�����û��ͨ����֤�����5����Ͽ����ӣ�

            if (session.GetComponent<SessionLockingComponent>() != null)//����Ự�������Ϊ�յ�����£�˵�����������յ��ͻ��˶������
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;//���ص�¼�����δ�����
                reply();//ί�з��ͻظ���Ϣresponse
                session.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                return;
            }

            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))//�ж��˺������Ƿ�Ϊ��
            {
                response.Error = ErrorCode.ERR_LoginInfoIsNull;//���ص�¼��Ϣ������
                reply();//ί�з��ͻظ���Ϣresponse
                session.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                return;
            }

            if (!Regex.IsMatch(request.AccountName.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Za-z].*).{6,15}$"))//ͨ��������ʽ���ж��˺��Ƿ��������ּӴ�Сд��ĸ��϶���,���������
            {
                response.Error = ErrorCode.ERR_LoginAccountNameFormError;//���ص�¼�˺Ÿ�ʽ������
                reply();//ί�з��ͻظ���Ϣresponse
                session.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                return;
            }

            if (!Regex.IsMatch(request.Password.Trim(), @"^[A-Za-z0-9]+$"))//�ж������Ƿ��������ּӴ�Сд��ĸ��϶���,���������
            {
                response.Error = ErrorCode.ERR_LoginPasswordFormError;//���ص�¼�����ʽ������
                reply();//ί�з��ͻظ���Ϣresponse
                session.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())//�Ự�����(��Ϊ�����õ����첽�߼�),ͬһ���û��ĻỰ����ظ�����ʱ���������
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.AccountName.Trim().GetHashCode()))//Э�����������û������������������Ψһid,�û��˺����Ĺ�ϣֵ����ֹ��ͬ��ַ���û�ͬʱ����ͬ���˺�ȥ��¼��
                {
                    //��ѯ���ݿ����Ƿ�����������¼���˺�
                    var accountInfoList = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<Account>(d => d.AccountName.Equals(request.AccountName.Trim()));//DomainZone��ȡ������Ϸ�ڵڼ���������
                    Account account = null;
                    if (accountInfoList != null && accountInfoList.Count > 0)//�������ֵ����0�����ʾ��ѯ����
                    {
                        account = accountInfoList[0];
                        if (account.AccountType == (int)AccountType.BlackList)//����˺��ں�������
                        {
                            response.Error = ErrorCode.ERR_LoginBlackListError;//���ص�¼�˺����ں�����������
                            reply();//ί�з��ͻظ���Ϣresponse
                            session.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                            account?.Dispose();//��ʱ��ʵ�����Ҫ�ǵ��ͷ�
                            return;
                        }

                        if (!account.Password.Equals(request.Password))//�ж������Ƿ���ȷ�����������
                        {
                            response.Error = ErrorCode.ERR_LoginPasswordError;//���ص�¼���������
                            reply();//ί�з��ͻظ���Ϣresponse
                            session.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                            account?.Dispose();//��ʱ��ʵ�����Ҫ�ǵ��ͷ�
                            return;
                        }

                        session.AddChild(account);//���˺���Ϣ����Ự��

                    }
                    else//���û�в�ѯ�����Զ�ע��
                    {
                        account = session.AddChild<Account>();//�������µ�Accountʵ��
                        account.AccountName = request.AccountName.Trim();//ɾ��ͷ����β���Ŀ��ַ�
                        account.Password = request.Password.Trim();
                        account.CreateTime = TimeHelper.ServerNow();//����ʱ��
                        account.AccountType = (int)AccountType.General;//��ͨ�����˺�
                        await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save<Account>(account);//���˺���Ϣ�������ݿ�
                    }

                    StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "LoginCenter");//��ȡ��¼���ķ�����������
                    long loginCenterInstanceId = startSceneConfig.InstanceId;//��ȡ��¼���ķ�������ʵ��id
                    L2A_LoginAccountResponse loginAccountResponse = (L2A_LoginAccountResponse)await ActorMessageSenderComponent.Instance.Call(loginCenterInstanceId, new A2L_LoginAccountRequest() { AccountId = account.Id });//���¼���ķ�����������Ϣ���ȴ��ظ�
                    if (loginAccountResponse.Error != ErrorCode.ERR_Success)//��������˴�����Ͽ�����
                    {
                        response.Error = loginAccountResponse.Error;//���˺����ķ������Ĵ��󷢻ظ��ͻ���
                        reply();//ί�з��ͻظ���Ϣresponse
                        session?.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                        account.Dispose();//�ͷ���ʱ��ʵ�����
                        return;
                    }

                    //��������
                    long accountSessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(account.Id);//��ȡ��¼�Ự����ﵱǰ��¼�˺ŵĻỰid
                    Session otherSession = Game.EventSystem.Get(accountSessionInstanceId) as Session;//ʹ�ûỰid��ȡ�Ự
                    otherSession?.Send(new A2C_Disconnect() { Error = 0 });//��ͻ��˷��Ͷ��Ŷ�����Ϣ
                    otherSession?.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                    session.DomainScene().GetComponent<AccountSessionsComponent>().Add(account.Id, session.InstanceId);//���µĻỰ��ӽ���¼�Ự�����
                    session.AddComponent<AccountCheckOutTimeComponent, long>(account.Id);//10���Ӻ��߳�����Ծ�û���Ӧ����������������ֻ�û���ˣ�û�м�ʱ֪ͨ���������ߣ�

                    string Token = TimeHelper.ServerNow().ToString() + RandomHelper.RandomNumber(int.MinValue, int.MaxValue).ToString();//����һ����¼��־����
                    session.DomainScene().GetComponent<TokenComponent>().Remove(account.Id);//�Ƴ���ǰ��¼id��ȥ����ʹ�ù�������
                    session.DomainScene().GetComponent<TokenComponent>().Add(account.Id, Token);//��ӵ�ǰ��¼id�ĵ�¼����

                    response.AccountId = account.Id;
                    response.Token = Token;

                    reply();//ί�з��ͻظ���Ϣresponse
                    account?.Dispose();//�ͷ���ʱ��ʵ�����

                }

            }

        }
    }
}