#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DevExpress.Xpf.Core.Native;
using System.Windows.Threading;
namespace DevExpress.Xpf.Map {
	public delegate void MapWebRequestEventHandler(object sender, MapWebRequestEventArgs e);
	[NonCategorized]
	public class MapWebRequestEventArgs : EventArgs {
		readonly Uri uri;
		readonly WebClient client;
		public Uri Uri {
			get {
				return uri;
			}
		}
		public ICredentials Credentials {
			get {
				return client.Credentials;
			}
			set {
				client.Credentials = value;
			}
		}
		public bool UseDefaultCredentials {
			get {
				return client.UseDefaultCredentials;
			}
			set {
				client.UseDefaultCredentials = value;
			}
		}
		public Encoding Encoding {
			get {
				return client.Encoding;
			}
			set {
				client.Encoding = value;
			}
		}
		public WebHeaderCollection Headers {
			get {
				return client.Headers;
			}
			set {
				client.Headers = value;
			}
		}
		public IWebProxy Proxy {
			get {
				return client.Proxy;
			}
			set {
				client.Proxy = value;
			}
		}
		public MapWebRequestEventArgs(Uri uri, WebClient client) {
			this.uri = uri;
			this.client = client;
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class TileLoadedEventArgs : EventArgs {
		readonly byte[] streamSource;
		readonly TilePositionData tilePosition;
		readonly bool downloadFailed;
		readonly string uri;		
		public byte[] StreamSource { get { return streamSource; } }
		public TilePositionData TilePosition { get { return tilePosition; } }
		public bool DownloadFailed { get { return downloadFailed; } }
		public string Uri { get { return uri; } }
		public TileLoadedEventArgs(byte[] streamSource, TilePositionData tilePosition, bool downloadFailed, string uri) {
			this.streamSource = streamSource;
			this.tilePosition = tilePosition;
			this.downloadFailed = downloadFailed;
			this.uri = uri;
		}
	}
	public delegate void TileLoadedEventHandler(object sender, TileLoadedEventArgs e);
	public class DownloadQueue {
		const int maxDownloadCount = 8;
		const int connectionTimeout = 5;
		enum DownloadStatus {
			Wating,
			InProgress,
			Completed,
			Cancelled
		}
		class DownloadItem {
			internal static int Compare(DownloadItem x, DownloadItem y) {
				if (x == null || y == null)
					throw new ArgumentNullException(x == null ? "x" : "y");
				if (x.Status == DownloadStatus.InProgress && y.Status != DownloadStatus.InProgress || y.Status == DownloadStatus.Completed || y.Status == DownloadStatus.Cancelled)
					return -1;
				if (y.Status == DownloadStatus.InProgress && x.Status != DownloadStatus.InProgress || x.Status == DownloadStatus.Completed || x.Status == DownloadStatus.Cancelled)
					return 1;
				return x.Distance.CompareTo(y.Distance);
			}
			readonly Uri uri;
			readonly TilePositionData position;
			DownloadStatus status = DownloadStatus.Wating;
			WebClient webClient = null;
			int distance;
			DateTime lastDownloadTime;
			public Uri Uri { get { return uri; } }
			public TilePositionData Position { get { return position; } }
			public DownloadStatus Status { get { return status; } set { status = value; } }
			public int Distance { get { return distance; } set { distance = value; } }
			public DateTime LastDownloadTime { get { return lastDownloadTime; } }
			public event TileLoadedEventHandler Loaded;
			public event MapWebRequestEventHandler WebRequest;
			public DownloadItem(Uri uri, TilePositionData position) {
				this.uri = uri;
				this.position = position;
			}
			bool IsTileEmpty() {
				if(webClient == null)
					return false;
				if(webClient.ResponseHeaders.ToString().Contains("X-VE-Tile-Info"))
					foreach(string value in webClient.ResponseHeaders.GetValues("X-VE-Tile-Info"))
						if(value.ToLower() == "no-tile")
							return true;
				return false;
			}
			void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
				webClient.DownloadDataCompleted -= new DownloadDataCompletedEventHandler(DownloadDataCompleted);
				if (Status == DownloadStatus.Cancelled || e.Cancelled)
					return;
				bool error = e.Error == null ? IsTileEmpty() : true;
				CompleteDownload(error, e.Error == null ? e.Result : null);
			}
			void RaiseWebRequest() {
				if (WebRequest != null)
					WebRequest(this, new MapWebRequestEventArgs(uri, webClient));
			}
			void CompleteDownload(bool error, byte[] result) {
				lastDownloadTime = DateTime.Now;
				Status = DownloadStatus.Completed;
				if (Loaded != null)
					if (result != null && result.Length > 8)
						Loaded(this, new TileLoadedEventArgs(result, Position, error, Uri.ToString()));
					else
						Loaded(this, new TileLoadedEventArgs(result, Position, true, Uri.ToString()));
			}
			public void Download() {
				Status = DownloadStatus.InProgress;
				webClient = new WebClient();
				RaiseWebRequest();
				webClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCompleted);
				webClient.DownloadDataAsync(Uri);
			}
			public void CancelDownload() {
				if (webClient != null)
					webClient.CancelAsync();
				Status = DownloadStatus.Cancelled;
			}
		}
		readonly List<string> uriDictionary = new List<string>();
		readonly List<DownloadItem> queue = new List<DownloadItem>();
		readonly DispatcherTimer timer;
		public bool IsTileQueueEmpty { get { return queue.Count == 0; } }
		public event TileLoadedEventHandler TileLoaded;
		public event MapWebRequestEventHandler WebRequest;
		public DownloadQueue() {
			this.timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(connectionTimeout), IsEnabled = false };
			this.timer.Tick += timer_Tick;
		}
		void timer_Tick(object sender, EventArgs e) {
			Update();
		}
		void Update() {
			int count = 0;
			foreach (DownloadItem item in queue.ToArray()) {
				switch (item.Status) {
					case DownloadStatus.InProgress:
						count++;
						break;
					case DownloadStatus.Wating:
						BeginDownload(item);
						if (item.Status == DownloadStatus.InProgress)
							count++;
						break;
					case DownloadStatus.Completed:
					default:
						continue;
				}
				if (count > maxDownloadCount)
					break;
			}
			if (queue.Count > 0)
				timer.Start();
			else timer.Stop();
		}
		bool BeginDownload(DownloadItem item) {
			if ((DateTime.Now - item.LastDownloadTime).TotalSeconds < connectionTimeout) 
				return false;				
			item.Loaded += ItemLoaded;
			item.WebRequest += OnWebRequest;
			item.Download();
			return true;
		}
		void RaiseWebRequest(MapWebRequestEventArgs e) {
			if (WebRequest != null)
				WebRequest(this, e);
		}
		void OnWebRequest(object sender, MapWebRequestEventArgs e) {
			RaiseWebRequest(e);
		}
		void ItemLoaded(object sender, TileLoadedEventArgs e) {
			DownloadItem item = sender as DownloadItem;
			if (item != null) {
				string key = GetKey(item.Uri, item.Position);
				if (e.DownloadFailed) {
					if (item.Uri.IsAbsoluteUri && item.Uri.IsFile) {
						queue.Remove(item);
						uriDictionary.Remove(key);
						item.Loaded -= ItemLoaded;
						item.WebRequest -= OnWebRequest;
					} else 
						item.Status = DownloadStatus.Wating;					
				} else {
					queue.Remove(item);
					uriDictionary.Remove(key);
					if (TileLoaded != null)
						TileLoaded(this, e);
					item.Loaded -= ItemLoaded;
					item.WebRequest -= OnWebRequest;
				}
			}
			Update();
		}
		void Sort(TilePositionData centerPosition) {
			queue.Sort(DownloadItem.Compare);
		}
		int CalculateDistance(TilePositionData startPosition, TilePositionData endPosition) {
			return Math.Abs(endPosition.PositionX - startPosition.PositionX) + Math.Abs(endPosition.PositionY - endPosition.PositionY);
		}
		public void Push(Uri uri, TilePositionData position) {
			string key = GetKey(uri, position);
			if(!uriDictionary.Contains(key)) {
				queue.Add(new DownloadItem(uri, position));
				uriDictionary.Add(key);
			}
		}
		string GetKey(Uri uri, TilePositionData position) {
			return uri.ToString() + position.ToString();
		}
		public void Refresh(TilePositionData centerPosition) {
			foreach (DownloadItem item in queue.ToArray())
				if (item.Status == DownloadStatus.Wating && (item.Position.Level != centerPosition.Level)) {
					uriDictionary.Remove(GetKey(item.Uri, centerPosition));
					queue.Remove(item);
				}
				else
					item.Distance = CalculateDistance(centerPosition, item.Position);
			Sort(centerPosition);
			Update();
		}
		public void Clear() {
			uriDictionary.Clear();
			foreach (DownloadItem item in queue)
				item.CancelDownload();
			queue.Clear();
		}
	}
}
