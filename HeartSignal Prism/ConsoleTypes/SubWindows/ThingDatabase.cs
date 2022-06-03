using System.Collections.Generic;

namespace HeartSignal
{
	public static class ThingDatabase
	{
		public static Dictionary<string,ThingData> thingDatabase = new Dictionary<string,ThingData>();
		public static readonly object DatabaseSyncObj = new object();
		public delegate void ThingUpdate();
		public struct ThingData
		{
			public string desc;
			public string name;
			public Dictionary<string, List<string>> actionDatabase;
			public ThingUpdate updateEvent;
			public ThingData(string Name,string Desc)
			{
				desc = Desc;
				this.name = Name;
				actionDatabase = new Dictionary<string, List<string>>();
				updateEvent = null;
			}
		}
		
		public static void Examine(string id)
		{
            
			NetworkManager.SendNetworkMessage("look " + id);

		} 
		public static void GetData(string id)
		{
                    
			NetworkManager.SendNetworkMessage("data " + id);
        
		}
	}
}