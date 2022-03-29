using System.Collections.Generic;


namespace ET
{
	public static class RealmGateAddressHelper
	{
		public static StartSceneConfig GetGate(int zone,long accountId)
		{
			List<StartSceneConfig> zoneGates = StartSceneConfigCategory.Instance.Gates[zone];
			
			//int n = RandomHelper.RandomNumber(0, zoneGates.Count);//随机获取一个gate网关地址
			int n = accountId.GetHashCode() % zoneGates.Count;//根据账号哈希值取模来获取gate网关地址

			return zoneGates[n];
		}

		//获取Realm服务器的配置
		public static StartSceneConfig GetRealm(int zone)
		{
			StartSceneConfig zoneRealm = StartSceneConfigCategory.Instance.Realms[zone];
			return zoneRealm;
		}
	}
}
