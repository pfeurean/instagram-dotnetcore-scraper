﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace InstagramScraper
{
    public class Media
    {
        private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        public static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public Media()
        {
        }

        public string id;
        public DateTime createdTime;
        public string type;
        public string link;
        //public string imageLowResolutionUrl;
        //public string imageThumbnailUrl;
        //public string imageStandardResolutionUrl;
        //public string imageHighResolutionUrl;
        public string imageUrl;
        public string caption;
        public string captionIsEdited;
        public string isAd;
        public string videoLowResolutionUrl;
        public string videoStandardResolutionUrl;
        public string videoLowBandwidthUrl;
        public int videoViews;
        public string code;
        public Account owner;
        public long ownerId;
        public int likesCount;
        public string locationId;
        public string locationName;
        public int commentsCount;
        public int height;
        public int width;

        public static Media fromApi(JToken token)
        {
            // $instance = new self();
            // $instance->id = $mediaArray['id'];
            // $instance->type = $mediaArray['type'];
            // $instance->createdTime = $mediaArray['created_time'];
            // $instance->code = $mediaArray['code'];
            // $instance->link = $mediaArray['link'];
            // $instance->commentsCount = $mediaArray['comments']['count'];
            // $instance->likesCount = $mediaArray['likes']['count'];
            // $images = self::getImageUrls($mediaArray['images']['standard_resolution']['url']);
            // $instance->imageLowResolutionUrl = $images['low'];
            // $instance->imageThumbnailUrl = $images['thumbnail'];
            // $instance->imageStandardResolutionUrl = $images['standard'];
            // $instance->imageHighResolutionUrl = $images['high'];
            // if (isset($mediaArray['caption'])) {
            // 	$instance->caption = $mediaArray['caption']['text'];
            // }
            // if ($instance->type === 'video') {
            // 	if (isset($mediaArray['video_views'])) {
            // 		$instance->videoViews = $mediaArray['video_views'];
            // 	}
            // 	$instance->videoLowResolutionUrl = $mediaArray['videos']['low_resolution']['url'];
            // 	$instance->videoStandardResolutionUrl = $mediaArray['videos']['standard_resolution']['url'];
            // 	$instance->videoLowBandwidthUrl = $mediaArray['videos']['low_bandwidth']['url'];
            // }
            // if (isset($mediaArray['location']['id'])) {
            // 	$instance->locationId = $mediaArray['location']['id'];
            // }
            // if (isset($mediaArray['location']['name'])) {
            // 	$instance->locationName = $mediaArray['location']['name'];
            // }
            // return $instance;


            var media = new Media();
            return media;
        }

        public static Media fromMediaPage(JToken token)
        {
            Media media = new Media()
            {
                code = (string)token["shortcode"],
                link = Endpoints.getMediaPageLink((string)token["shortcode"]),
                ownerId = (long)token["owner"]["id"],
                id = (string)token["id"],
                type = ((bool)token["is_video"]) ? "video" : "image",
                height = (int)token["dimensions"]["height"],
                width = (int)token["dimensions"]["width"],
                imageUrl = (string)token["display_url"]
            };

            media.owner = Account.fromMediaPage(token["owner"]);

            return media;
        }

        public static Media fromTagPage(JToken token)
        {
            var media = new Media()
            {
                code = (string)token["shortcode"],
                link = Endpoints.getMediaPageLink((string)token["shortcode"]),
                commentsCount = (int)token["edge_media_to_comment"]["count"],
                likesCount = (int)token["edge_liked_by"]["count"],
                ownerId = (long)token["owner"]["id"],
                createdTime = _epoch.AddSeconds((int)token["taken_at_timestamp"]),
                id = (string)token["id"],
                type = "image",
                caption = (((JArray)token["edge_media_to_caption"]["edges"]).Count > 0) ? token["edge_media_to_caption"]["edges"][0]["node"]["text"].Value<string>() : "",
                height = (int)token["dimensions"]["height"],
                width = (int)token["dimensions"]["width"],
                imageUrl = (string)token["display_url"]
            };

            //var images = getImageUrls((string)token["display_src"]);

            //media.imageStandardResolutionUrl = images["standard"];
            //media.imageLowResolutionUrl = images["low"];
            //media.imageHighResolutionUrl = images["high"];
            //media.imageThumbnailUrl = images["thumbnail"];

            if ((bool)token["is_video"])
            {
                media.type = "video";
                media.videoViews = (int)token["video_view_count"];
            }

            return media;
        }

        public static int getIdFromCode(string code)
        {
            var id = 0;
            for (var i = 0; i < code.Length; i++)
            {
                id = id * 64 + alphabet.IndexOf(code[i]);
            }
            return id;
        }

        //private static Dictionary<string, string> getImageUrls(string imageUrl)
        //{
        //	var uri = new Uri(imageUrl);
        //	var parts = uri.AbsolutePath.Split('/');
        //	var imageName = parts[parts.Length - 1];
        //	var urls = new Dictionary<string, string> {
        //		{ "thumbnail", $"{Endpoints.INSTAGRAM_CDN_URL}t/s150x150/{imageName}" },
        //		{ "low", $"{Endpoints.INSTAGRAM_CDN_URL}t/s320x320/{imageName}" },
        //		{ "standard", $"{Endpoints.INSTAGRAM_CDN_URL}t/s640x640/{imageName}"},
        //		{ "high", $"{Endpoints.INSTAGRAM_CDN_URL}{imageName}" }
        //	};
        //	return urls;
        //}

        public static string getLinkFromId(string id)
        {
            var code = Media.getCodeFromId(id);
            return Endpoints.getMediaPageLink(code);
        }

        public static string getCodeFromId(string id)
        {
            var parts = id.Split('_');
            var num = Int32.Parse(parts[0]);
            var code = "";
            while (num > 0)
            {
                var remainder = num % 64;
                num = (num - remainder) / 64;
                code = alphabet[remainder] + code;
            };
            return code;
        }
    }
}
