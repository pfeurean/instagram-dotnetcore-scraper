using System;
namespace InstagramScraper
{
	public static class Endpoints
	{
		public const string BASE_URL = @"https://www.instagram.com";
		public const string ACCOUNT_PAGE = @"https://www.instagram.com/{username}";
		public const string MEDIA_LINK = @"https://www.instagram.com/p/{code}";
		public const string ACCOUNT_MEDIAS = @"https://www.instagram.com/{username}/media?max_id={max_id}";
		public const string ACCOUNT_JSON_INFO = @"https://www.instagram.com/{username}/?__a=1";
		public const string MEDIA_JSON_INFO = @"https://www.instagram.com/p/{code}/?__a=1";
		public const string MEDIA_JSON_BY_LOCATION_ID = @"https://www.instagram.com/explore/locations/{{facebookLocationId}}/?__a=1&max_id={{maxId}}";
		public const string MEDIA_JSON_BY_TAG = @"https://www.instagram.com/explore/tags/{tag}/?__a=1&max_id={max_id}";
		public const string GENERAL_SEARCH = @"https://www.instagram.com/web/search/topsearch/?query={query}";
		public const string ACCOUNT_JSON_INFO_BY_ID = @"ig_user({userId}){id,username,external_url,full_name,profile_pic_url,biography,followed_by{count},follows{count},media{count},is_private,is_verified}";
		public const string LAST_COMMENTS_BY_CODE = @"ig_shortcode({{code}}){comments.last({{count}}){count,nodes{id,created_at,text,user{id,profile_pic_url,username,follows{count},followed_by{count},biography,full_name,media{count},is_private,external_url,is_verified}},page_info}}";
		public const string COMMENTS_BEFORE_COMMENT_ID_BY_CODE = @"ig_shortcode({{code}}){comments.before({{commentId}},{{count}}){count,nodes{id,created_at,text,user{id,profile_pic_url,username,follows{count},followed_by{count},biography,full_name,media{count},is_private,external_url,is_verified}},page_info}}";
		public const string LAST_LIKES_BY_CODE = @"ig_shortcode({{code}}){likes{nodes{id,user{id,profile_pic_url,username,follows{count},followed_by{count},biography,full_name,media{count},is_private,external_url,is_verified}},page_info}}";

		public const string INSTAGRAM_QUERY_URL = @"https://www.instagram.com/query/";
		public const string INSTAGRAM_CDN_URL = @"https://scontent.cdninstagram.com/";

		public static string GetAccountPageLink(string userName)
		{
			return Endpoints.ACCOUNT_PAGE.Replace(@"{username}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(userName));
		}

		public static string getAccountJsonLink(string userName)
		{
			return Endpoints.ACCOUNT_JSON_INFO.Replace(@"{username}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(userName));
		}

		public static string getAccountJsonInfoLinkByAccountId(long id)
		{
			return Endpoints.ACCOUNT_JSON_INFO_BY_ID.Replace("{userId}", id.ToString());
		}

		public static string getAccountMediasJsonLink(string userName, string maxId = @"")
		{
			return Endpoints.ACCOUNT_MEDIAS
							.Replace("{username}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(userName))
							.Replace("{max_id}", maxId);
		}

		public static string getMediaPageLink(string code)
		{
			return Endpoints.MEDIA_LINK.Replace(@"{code}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(code));
		}

		public static string getMediaJsonLink(string code)
		{
			return Endpoints.MEDIA_JSON_INFO.Replace(@"{code}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(code));
		}

		public static string getMediasJsonByLocationIdLink(string facebookLocationId, string maxId = @"")
		{
			return Endpoints.MEDIA_JSON_BY_LOCATION_ID
							.Replace(@"{{facebookLocationId}}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(facebookLocationId))
				            .Replace(@"{{maxId}}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(maxId));
		}

		public static string getMediasJsonByTagLink(string tag, string maxId = @"")
		{
			return Endpoints.MEDIA_JSON_BY_TAG
				            .Replace(@"{tag}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(tag))		    
				            .Replace(@"{max_id}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(maxId));
		}

		public static string getGeneralSearchJsonLink(string query)
		{
		    return Endpoints.GENERAL_SEARCH.Replace(@"{query}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(query));
		}

		public static string getLastCommentsByCodeLink(string code, int count)
		{
			return Endpoints.LAST_COMMENTS_BY_CODE
				            .Replace(@"{{code}}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(code))
				            .Replace(@"{{count}}", count.ToString());
		}

		public static string getCommentsBeforeCommentIdByCode(string code, int count, string commentId)
		{
			return Endpoints.COMMENTS_BEFORE_COMMENT_ID_BY_CODE
				            .Replace(@"{{code}}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(code))
				            .Replace(@"{{count}}", count.ToString())
				            .Replace(@"{{commentId}}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(commentId));
		}

		public static string getLastLikesByCodeLink(string code)
		{
			return Endpoints.LAST_LIKES_BY_CODE.Replace(@"{{code}}", System.Text.Encodings.Web.UrlEncoder.Default.Encode(code));
		}	
	}
}
