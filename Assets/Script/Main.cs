using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class Main : MonoBehaviour
{
		public void LoadMap ()
		{
				Item item = JsonConvert.DeserializeObject<Item> ("{\"item_name\" : \"undo\",\"amount\" : 1}");
				Debug.Log (item.item_name);

				Application.LoadLevelAsync ("Map");
		}
	
		public class Item
		{
				public int item_id { get; set; }
		
				public string item_name {
						get { 
								return name;
						}
						set {
								name = value;
						} 
				}
		
				public string name { get; set; }
		
				public string description { get; set; }
		
				public int amount { get; set; }
		
				public int price { get; set; }
		}
}
