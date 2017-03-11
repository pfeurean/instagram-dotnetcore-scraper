using System;
using Newtonsoft.Json.Linq;

namespace InstagramScraper
{
	public class Comment
	{
		public Comment()
		{
		}

		public string text;
		public string createdAt;
		public string id;
		public Account user;

		public static Comment fromApi(JToken token)
		{
			return new Comment
			{
				text = (string)token["text"],
				createdAt = (string)token["created_at"],
				id = (string)token["id"],
				user = Account.fromAccountPage((string)token["user"])
			};
		}
	}
}
