using System;
using Newtonsoft.Json.Linq;

namespace Instagram.Scraper
{
	public class Tag
	{
		public Tag()
		{
		}

		public int mediaCount;
		public string name;
		public string id;

		public static Tag fromSearchPage(JToken token)
		{
			return new Tag
			{
				mediaCount = (int)token["media_count"],
				name = (string)token["name"],
				id = (string)token["id"]
			};
		}
	}
}
