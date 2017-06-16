﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace InstagramScraper
{
	public static class Instagram
	{
		private const int MAX_COMMENTS_PER_REQUEST = 300;
		private const string characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private static HttpClient HttpClient = new System.Net.Http.HttpClient() { Timeout = TimeSpan.FromSeconds(90) };

		public static async Task<Account> getAccount(string userName)
		{
			var response = await HttpClient.GetAsync(Endpoints.getAccountJsonLink(userName));

			if (!response.IsSuccessStatusCode)
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
				{
					throw new InstagramNotFoundException($"Account with given username {userName} does not exist.");
				}

				throw new InstagramException($"Response code is {response.StatusCode}. Something went wrong. Please report issue.");
			}

			var contents = await response.Content.ReadAsStringAsync();
			var token = JObject.Parse(contents);
			if (token == null)
			{
				throw new InstagramNotFoundException($"Account with given username {userName} does not exist.");
			}

			return Account.fromAccountPage(token["user"]);
		}

		public static string GenerateRandomString(int length = 10)
		{
			var random = new Random();
			var charactersLength = characters.Length;
			var randomString = new System.Text.StringBuilder();
			for (var i = 0; i < length; i++)
			{
				randomString.Append(characters[random.Next(0, charactersLength - 1)]);
			}
			return randomString.ToString();
		}

		public static async Task<Account> getAccountById(long id)
		{
			string contents = string.Empty;
			var request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, Endpoints.INSTAGRAM_QUERY_URL);
			var csrftoken = GenerateRandomString();
			request.Headers.Add("Cookie", $"csrftoken={csrftoken}");
			request.Headers.Add("X-Csrftoken", csrftoken);
			request.Headers.Add("Referer", "https://www.instagram.com/");
            request.Content = new FormUrlEncodedContent(new [] {
                new System.Collections.Generic.KeyValuePair<string,string>("q", $"{Endpoints.getAccountJsonInfoLinkByAccountId(id)}")
            });

			var response = await HttpClient.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
				{
					throw new InstagramNotFoundException($"Account with given id {id} does not exist.");
				}

				throw new InstagramException($"Response code is {response.StatusCode}. Something went wrong. Please report issue.");
			}

			contents = await response.Content.ReadAsStringAsync();

			if (string.IsNullOrWhiteSpace(contents))
			{
				throw new InstagramNotFoundException($"Account with given id {id} does not exist.");
			}

			var token = JObject.Parse(contents);
			if (token == null && !Equals(token["status"], "ok"))
			{
				throw new InstagramNotFoundException($"Account with given id {id} did not return an OK status.  Status was {token["status"]}.");
			}

			return Account.fromAccountPage(token);
		}

		// private static string getContentsFromUrl($parameters)
		// {
		// 	if (!function_exists('curl_init')) {
		// 		return false;
		// 	}
		// 	$random = self::generateRandomString();
		// 	$ch = curl_init();
		// 	curl_setopt($ch, CURLOPT_URL, Endpoints::INSTAGRAM_QUERY_URL);
		// 	curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
		// 	curl_setopt($ch, CURLOPT_POST, 1);
		// 	curl_setopt($ch, CURLOPT_POSTFIELDS, 'q=' . $parameters);
		// 	$headers = array();
		// 	$headers[] = "Cookie:  csrftoken=$random;";
		// 	$headers[] = "X-Csrftoken: $random";
		// 	$headers[] = "Referer: https://www.instagram.com/";
		// 	curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
		// 	$output = curl_exec($ch);
		// 	curl_close($ch);
		// 	return $output;
		// }

		// private static string generateRandomString($length = 10)
		// {
		// 	$characters = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
		// 	$charactersLength = strlen($characters);
		// 	$randomString = '';
		// 	for ($i = 0; $i < $length; $i++) {
		// 		$randomString .= $characters[rand(0, $charactersLength - 1)];
		// 	}
		// 	return $randomString;
		// }

		// public static Media[] getMedias($username, $count = 20, $maxId = '')
		// {
		// 	$index = 0;
		// 	$medias = [];
		// 	$isMoreAvailable = true;
		// 	while ($index < $count && $isMoreAvailable) {
		// 		$response = Request::get(Endpoints::getAccountMediasJsonLink($username, $maxId));
		// 		if ($response->code !== 200) {
		// 			throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 		}

		// 		$arr = json_decode($response->raw_body, true);
		// 		if (!is_array($arr)) {
		// 			throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 		}
		// 		if (count($arr['items']) === 0) {
		// 			return [];
		// 		}
		// 		foreach ($arr['items'] as $mediaArray) {
		// 			if ($index === $count) {
		// 				return $medias;
		// 			}
		// 			$medias[] = Media::fromApi($mediaArray);
		// 			$index++;
		// 		}
		// 		if (count($arr['items']) == 0) {
		// 			return $medias;
		// 		}
		// 		$maxId = $arr['items'][count($arr['items']) - 1]['id'];
		// 		$isMoreAvailable = $arr['more_available'];
		// 	}
		// 	return $medias;
		// }

		// public static function getPaginateMedias($username, $maxId = '')
		// {
		// 	$hasNextPage = true;
		// 	$medias = [];

		// 	$toReturn = [
		// 		'medias' => $medias,
		// 		'maxId' => $maxId,
		// 		'hasNextPage' => $hasNextPage
		// 	];

		// 	$response = Request::get(Endpoints::getAccountMediasJsonLink($username, $maxId));

		// 	if ($response->code !== 200) {
		// 		throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 	}

		// 	$arr = json_decode($response->raw_body, true);

		// 	if (!is_array($arr)) {
		// 		throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 	}

		// 	if (count($arr['items']) === 0) {
		// 		return $toReturn;
		// 	}

		// 	foreach ($arr['items'] as $mediaArray) {
		// 		$medias[] = Media::fromApi($mediaArray);
		// 	}

		// 	$maxId = $arr['items'][count($arr['items']) - 1]['id'];
		// 	$hasNextPage = $arr['more_available'];

		// 	$toReturn = [
		// 		'medias' => $medias,
		// 		'maxId' => $maxId,
		// 		'hasNextPage' => $hasNextPage
		// 	];

		// 	return $toReturn;
		// }

        public static async Task<Media> getMediaByCode(string mediaCode)
		{
			return await Instagram.getMediaByUrl(Endpoints.getMediaPageLink(mediaCode));
		}

        public static async Task<Media> getMediaByUrl(string mediaUrl)
		{
			string empty = string.Empty;
			HttpResponseMessage async = await Instagram.HttpClient.GetAsync(string.Format("{0}/?__a=1", (object)mediaUrl));
			if (async.StatusCode == HttpStatusCode.NotFound)
				throw new InstagramNotFoundException("Media with given code does not exist or account is private.");
			if (!async.IsSuccessStatusCode)
				throw new InstagramException(string.Format("Response code is {0}. Something went wrong. Please report issue.", (object)async.StatusCode));
			JObject jobject = JObject.Parse(await async.Content.ReadAsStringAsync());
			if (jobject == null)
				throw new InstagramException("Response decoding failed. Returned data corrupted or this library outdated. Please report issue.");
			string str1 = "graphql";
            string str2 = "shortcode_media";
			if (jobject[str1] == null)
				throw new InstagramException("Media with this code does not exist. Returned data corrupted or this library outdated. Please report issue.");
			if (jobject[str1][str2] == null)
				throw new InstagramException("Media with this code does not exist. Returned data corrupted or this library outdated. Please report issue.");
			return Media.fromMediaPage(jobject[str1][str2]);
		}

		public static async Task<ICollection<Media>> getMediasByTag(string tag, int count = 12, string maxID = "")
		{
			var index = 0;
			var medias = new List<Media>();
			var hasNextPage = true;
			while (index < count && hasNextPage)
			{
				var contents = string.Empty;
				var requestUrl = Endpoints.getMediasJsonByTagLink(tag.Replace("#", ""), maxID ?? "");
				var response = await HttpClient.GetAsync(requestUrl);

				if (!response.IsSuccessStatusCode)
				{
					throw new InstagramException($"Response code is {response.StatusCode}. Something went wrong. Please report issue.");
				}

				contents = await response.Content.ReadAsStringAsync();

				var token = JObject.Parse(contents);
				if (token == null)
				{
					throw new InstagramNotFoundException($"Response decoding failed. Returned data corrupted or this library outdated. Please report issue.");
				}

				if ((int)token["tag"]["media"]["count"] == 0)
				{
					return new Media[] { };
				}

				var nodes = token["tag"]["media"]["nodes"];
				foreach (var mediaArray in nodes)
				{
					if (index == count)
					{
						break;
					}

					try
					{
						medias.Add(Media.fromTagPage(mediaArray));
						index++;
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Caught Exception while parsing instagram post {mediaArray["id"]} for hashtag {tag} at {DateTime.UtcNow}: {ex.ToString()}");
					}
				}

				if (nodes.Count() == 0)
				{
					break;
				}

				maxID = (string)token["tag"]["media"]["page_info"]["end_cursor"];
				hasNextPage = (bool)token["tag"]["media"]["page_info"]["has_next_page"];
			}

            foreach (IGrouping<long, Media> grouping in medias.GroupBy<Media, long>((Func<Media, long>)(m => m.ownerId))) //.AsParallel<IGrouping<long, Media>>())
			{
				IGrouping<long, Media> a = grouping;
				long key = a.Key;
				Media media = a.FirstOrDefault<Media>();
				if (media != null)
				{
					Media mediaByCode = await Instagram.getMediaByCode(media.code);
                    if (mediaByCode != null && mediaByCode.owner != null)
                    {
						a.ToList<Media>().ForEach(b => b.owner = mediaByCode.owner);
					}
				}
				a = (IGrouping<long, Media>)null;
			}

			return (ICollection<Media>)medias.Where<Media>((Func<Media, bool>)(a => a.owner != null)).ToArray<Media>();
		}

		// public static function getPaginateMediasByTag($tag, $maxId = '')
		// {
		// 	$hasNextPage = true;
		// 	$medias = [];

		// 	$toReturn = [
		// 		'medias' => $medias,
		// 		'maxId' => $maxId,
		// 		'hasNextPage' => $hasNextPage
		// 	];

		// 	$response = Request::get(Endpoints::getMediasJsonByTagLink($tag, $maxId));

		// 	if ($response->code !== 200) {
		// 		throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 	}

		// 	$arr = json_decode($response->raw_body, true);

		// 	if (!is_array($arr)) {
		// 		throw new InstagramException('Response decoding failed. Returned data corrupted or this library outdated. Please report issue');
		// 	}

		// 	if (count($arr['tag']['media']['count']) === 0) {
		// 		return $toReturn;
		// 	}

		// 	$nodes = $arr['tag']['media']['nodes'];

		// 	if (count($nodes) == 0) {
		// 		return $toReturn;
		// 	}

		// 	foreach ($nodes as $mediaArray) {
		// 		$medias[] = Media::fromTagPage($mediaArray);
		// 	}

		// 	$maxId       = $arr['tag']['media']['page_info']['end_cursor'];
		// 	$hasNextPage = $arr['tag']['media']['page_info']['has_next_page'];
		// 	$count       = $arr['tag']['media']['count'];

		// 	$toReturn = [
		// 		'medias'      => $medias,
		// 		'count'       => $count,
		// 		'maxId'       => $maxId,
		// 		'hasNextPage' => $hasNextPage,
		// 	];

		// 	return $toReturn;
		// }

		// public static function searchAccountsByUsername($username)
		// {
		// 	$response = Request::get(Endpoints::getGeneralSearchJsonLink($username));
		// 	if ($response->code === 404) {
		// 		throw new InstagramNotFoundException('Account with given username does not exist.');
		// 	}
		// 	if ($response->code !== 200) {
		// 		throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 	}

		// 	$jsonResponse = json_decode($response->raw_body, true);
		// 	if (!isset($jsonResponse['status']) || $jsonResponse['status'] != 'ok') {
		// 		throw new InstagramException('Response code is not equal 200. Something went wrong. Please report issue.');
		// 	}
		// 	if (!isset($jsonResponse['users']) || count($jsonResponse['users']) == 0) {
		// 		return [];
		// 	}

		// 	$accounts = [];
		// 	foreach ($jsonResponse['users'] as $jsonAccount) {
		// 		$accounts[] = Account::fromSearchPage($jsonAccount['user']);
		// 	}
		// 	return $accounts;
		// }

		// public static function searchTagsByTagName($tag)
		// {
		// 	$response = Request::get(Endpoints::getGeneralSearchJsonLink($tag));
		// 	if ($response->code === 404) {
		// 		throw new InstagramNotFoundException('Account with given username does not exist.');
		// 	}
		// 	if ($response->code !== 200) {
		// 		throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 	}

		// 	$jsonResponse = json_decode($response->raw_body, true);
		// 	if (!isset($jsonResponse['status']) || $jsonResponse['status'] != 'ok') {
		// 		throw new InstagramException('Response code is not equal 200. Something went wrong. Please report issue.');
		// 	}

		// 	if (!isset($jsonResponse['hashtags']) || count($jsonResponse['hashtags']) == 0) {
		// 		return [];
		// 	}
		// 	$hashtags = [];
		// 	foreach ($jsonResponse['hashtags'] as $jsonHashtag) {
		// 		$hashtags[] = Tag::fromSearchPage($jsonHashtag['hashtag']);
		// 	}
		// 	return $hashtags;
		// }

		// public static function getTopMediasByTagName($tagName)
		// {
		// 	$response = Request::get(Endpoints::getMediasJsonByTagLink($tagName, ''));
		// 	if ($response->code === 404) {
		// 		throw new InstagramNotFoundException('Account with given username does not exist.');
		// 	}
		// 	if ($response->code !== 200) {
		// 		throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 	}
		// 	$jsonResponse = json_decode($response->raw_body, true);
		// 	$medias = [];
		// 	foreach ($jsonResponse['tag']['top_posts']['nodes'] as $mediaArray) {
		// 		$medias[] = Media::fromTagPage($mediaArray);
		// 	}
		// 	return $medias;
		// }

		// public static function getMediaById($mediaId)
		// {
		// 	$mediaLink = Media::getLinkFromId($mediaId);
		// 	return self::getMediaByUrl($mediaLink);
		// }

		// public static function getMediaCommentsById($mediaId, $count = 10, $maxId = null)
		// {
		// 	$code = Media::getCodeFromId($mediaId);
		// 	return self::getMediaCommentsByCode($code, $count, $maxId);
		// }

		// public static function getMediaCommentsByCode($code, $count = 10, $maxId = null)
		// {
		// 	$remain = $count;
		// 	$comments = [];
		// 	$index = 0;
		// 	$hasPrevious = true;
		// 	while ($hasPrevious && $index < $count) {
		// 		if ($remain > self::MAX_COMMENTS_PER_REQUEST) {
		// 			$numberOfCommentsToRetreive = self::MAX_COMMENTS_PER_REQUEST;
		// 			$remain -= self::MAX_COMMENTS_PER_REQUEST;
		// 			$index += self::MAX_COMMENTS_PER_REQUEST;
		// 		} else {
		// 			$numberOfCommentsToRetreive = $remain;
		// 			$index += $remain;
		// 			$remain = 0;
		// 		}
		// 		if (!isset($maxId)) {
		// 			$parameters = Endpoints::getLastCommentsByCodeLink($code, $numberOfCommentsToRetreive);

		// 		} else {
		// 			$parameters = Endpoints::getCommentsBeforeCommentIdByCode($code, $numberOfCommentsToRetreive, $maxId);
		// 		}
		// 		$jsonResponse = json_decode(self::getContentsFromUrl($parameters), true);
		// 		$nodes = $jsonResponse['comments']['nodes'];
		// 		foreach ($nodes as $commentArray) {
		// 			$comments[] = Comment::fromApi($commentArray);
		// 		}
		// 		$hasPrevious = $jsonResponse['comments']['page_info']['has_previous_page'];
		// 		$numberOfComments = $jsonResponse['comments']['count'];
		// 		if ($count > $numberOfComments) {
		// 			$count = $numberOfComments;
		// 		}
		// 		if (sizeof($nodes) == 0) {
		// 			return $comments;
		// 		}
		// 		$maxId = $nodes[sizeof($nodes) - 1]['id'];
		// 	}
		// 	return $comments;
		// }

		// public static function getLocationTopMediasById($facebookLocationId)
		// {
		// 	$response = Request::get(Endpoints::getMediasJsonByLocationIdLink($facebookLocationId));
		// 	if ($response->code === 404) {
		// 		throw new InstagramNotFoundException('Location with this id doesn\'t exist');
		// 	}
		// 	if ($response->code !== 200) {
		// 		throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 	}
		// 	$jsonResponse = json_decode($response->raw_body, true);
		// 	$nodes = $jsonResponse['location']['top_posts']['nodes'];
		// 	$medias = [];
		// 	foreach ($nodes as $mediaArray) {
		// 		$medias[] = Media::fromTagPage($mediaArray);
		// 	}
		// 	return $medias;
		// }

		// public static function getLocationMediasById($facebookLocationId, $quantity = 12, $offset = '')
		// {
		// 	$index = 0;
		// 	$medias = [];
		// 	$hasNext = true;
		// 	while ($index < $quantity && $hasNext) {
		// 		$response = Request::get(Endpoints::getMediasJsonByLocationIdLink($facebookLocationId, $offset));
		// 		if ($response->code !== 200) {
		// 			throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 		}
		// 		$arr = json_decode($response->raw_body, true);
		// 		$nodes = $arr['location']['media']['nodes'];
		// 		foreach ($nodes as $mediaArray) {
		// 			if ($index === $quantity) {
		// 				return $medias;
		// 			}
		// 			$medias[] = Media::fromTagPage($mediaArray);
		// 			$index++;
		// 		}
		// 		if (count($nodes) == 0) {
		// 			return $medias;
		// 		}
		// 		$hasNext = $arr['location']['media']['page_info']['has_next_page'];
		// 		$offset = $arr['location']['media']['page_info']['end_cursor'];
		// 	}
		// 	return $medias;
		// }

		// public static function getLocationById($facebookLocationId)
		// {
		// 	$response = Request::get(Endpoints::getMediasJsonByLocationIdLink($facebookLocationId));
		// 	if ($response->code === 404) {
		// 		throw new InstagramNotFoundException('Location with this id doesn\'t exist');
		// 	}
		// 	if ($response->code !== 200) {
		// 		throw new InstagramException('Response code is ' . $response->code . '. Body: ' . $response->body . ' Something went wrong. Please report issue.');
		// 	}
		// 	$jsonResponse = json_decode($response->raw_body, true);
		// 	return Location::makeLocation($jsonResponse['location']);
		// }
	}
}
