using System;
using Newtonsoft.Json.Linq;

namespace Instagram.Scraper
{
	public class Account
	{
		public Account()
		{
		}

		public long id;
		public string username;
		public string fullName;
		public string profilePicUrl;
		public string biography;
		public string externalUrl;
		public int followsCount;
		public int followedByCount;
		public int mediaCount;
		public bool isPrivate;
		public bool isVerified;

		public static Account fromAccountPage(JToken token)
		{
			return new Account()
			{
				username = (string)token["username"],
				followsCount = (int)token["follows"]["count"],
				followedByCount = (int)token["followed_by"]["count"],
				profilePicUrl = (string)token["profile_pic_url"],
				id = (long)token["id"],
				biography = (string)token["biography"],
				fullName = (string)token["full_name"],
				mediaCount = (int)token["media"]["count"],
				isPrivate = (bool)token["is_private"],
				externalUrl = (string)token["external_url"],
				isVerified = (bool)token["is_verified"]
			};
		}

		public static Account fromMediaPage(JToken token)
		{
			return new Account()
			{
				username = (string)token["username"],
				profilePicUrl = (string)token["profile_pic_url"],
				id = (long)token["id"],
				fullName = (string)token["full_name"],
				isPrivate = (bool)token["is_private"]
			};
		}

		public static Account fromSearchPage(JToken token)
		{
			return new Account()
			{
				username = (string)token["username"],
				profilePicUrl = (string)token["profile_pic_url"],
				id = (long)token["pk"],
				fullName = (string)token["full_name"],
				isPrivate = (bool)token["is_private"],
				isVerified = (bool)token["is_verified"],
				followedByCount = (int)token["follower_count"]
			};
		}
	}
}
