using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Main : MonoBehaviour
{
		public void LoadMap ()
		{
				string args = "{\"parameter\" :{\"token\" : \"99107\",\"items\" : [{\"item_name\" : \"undo\",\"amount\" : 1},{\"item_name\" : \"gem\",\"amount\" : 14}]}}";

				Dictionary<string, ItemParameter> param1 = JsonConvert.DeserializeObject<Dictionary<string, ItemParameter>> (args);
				List<Item> itemList = param1 ["parameter"].items;
				foreach (Item item in itemList)		
						Debug.Log (item.item_name);

				Application.LoadLevelAsync ("Map");
		}

		public class Parameter
		{
				public string token { get; set; }
		}

		public class ItemParameter : Parameter
		{
				public List<Item> items { get; set; }
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
