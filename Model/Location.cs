using System;
using Newtonsoft.Json.Linq;

namespace Instagram.Scraper
{
	public class Location
	{
		public Location()
		{
		}

		public string id;
		public string name;
		public string lat;
		public string lng;		

		public static Location makeLocation(JToken token)
		{
			return new Location
			{
				id = (string) token["id"],
				name = (string) token["name"],
				lat = (string) token["lat"],
				lng = (string) token["lng"]
			};
		}
	}
}
