﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnerTube.Models;

namespace LightTube.Database
{
	public class LTPlaylist
	{
		public string Id;
		public string Name;
		public string Description;
		public PlaylistVisibility Visibility;
		public List<string> VideoIds;
		public string Author;
		public DateTimeOffset LastUpdated;

		public async Task<YoutubePlaylist> ToYoutubePlaylist()
		{
			List<Thumbnail> t = new();
			if (VideoIds.Count > 0)
				t.Add(new Thumbnail { Url = $"https://i.ytimg.com/vi_webp/{VideoIds.First()}/maxresdefault.webp" });
			YoutubePlaylist playlist = new();
			playlist.Id = Id;
			playlist.Title = Name;
			playlist.Description = Description;
			playlist.VideoCount = VideoIds.Count.ToString();
			playlist.ViewCount = "0";
			playlist.LastUpdated = "Last updated " + LastUpdated.ToString(); //todo: format this
			playlist.Thumbnail = t.ToArray();
			playlist.Channel = new Channel
			{
				Name = Author,
				Id = GenerateChannelId(),
				SubscriberCount = "0 subscribers",
				Avatars = Array.Empty<Thumbnail>()
			};
			playlist.Videos = (await DatabaseManager.Playlists.GetPlaylistVideos(Id)).Select(x =>
			{
				x.Index = VideoIds.IndexOf(x.Id) + 1;
				return x;
			}).Cast<DynamicItem>().ToArray();
			playlist.ContinuationKey = null;
			return playlist;
		}

		private string GenerateChannelId()
		{
			StringBuilder sb = new("LTU-" + Author.Trim() + "_");

			string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
			Random rng = new(Author.GetHashCode());
			while (sb.Length < 32) sb.Append(alphabet[rng.Next(0, alphabet.Length)]);
			return sb.ToString();
		}
	}

	public enum PlaylistVisibility
	{
		PRIVATE,
		UNLISTED,
		VISIBLE
	}
}