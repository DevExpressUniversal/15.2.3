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
using System.Linq;
using System.Reflection;
using System.IO;
using System.Drawing;
using DevExpress.Utils;
using System.Globalization;
using System.Net;
using DevExpress.Utils.Controls;
using DevExpress.XtraMap.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap.Native {
	public interface IImageUriLoader {
		void LoadImage(Uri uri);
	}
	public interface IImageUriLoaderClient {
		void OnImageLoaded(Image image);
	}
	public class ImageCacheContainer : MapDisposableObject {
		static ImageCache cache;
		protected ImageCache Cache { get { return cache; } }
		public ImageCacheContainer() {
			if(cache == null)
				cache = CreateImageCache();
		}
		internal static void ReleaseCache() {
			if(cache != null) {
				cache.Dispose();
				cache = null;
			}
		}
		protected override void DisposeOverride() {
			ReleaseCache();
			base.DisposeOverride();
		}
		protected virtual ImageCache CreateImageCache() {
			return new ImageCache();
		}
		public static void QueryImage(Uri imageUri, IImageUriLoaderClient client) {
			if(cache == null)
				cache = new ImageCache();
			cache.QueryImage(imageUri, client);
		}
	}
	public class ImageCache : MapDisposableObject {
		Dictionary<Uri, ImageCacheItem> items = new Dictionary<Uri, ImageCacheItem>();
		protected internal Dictionary<Uri, ImageCacheItem> Items { get { return items; } }
		public virtual void QueryImage(Uri imageUri, IImageUriLoaderClient client) {
			ImageCacheItem item = null;
			if(!Items.TryGetValue(imageUri, out item)) {
				item = CreateCacheItem(imageUri);
				Items.Add(imageUri, item);
			}
			item.RegisterClient(client);
		}
		protected virtual ImageCacheItem CreateCacheItem(Uri imageUri) {
			return new ImageCacheItem(imageUri);
		}
		protected override void DisposeOverride() {
			foreach(ImageCacheItem item in Items.Values) {
				item.Dispose();
			}
			Items.Clear();
			items = null;
		}
	}
	public class ImageCacheItem : MapDisposableObject, IImageUriLoaderClient {
		readonly Uri imageUri;
		Image loadedImage;
		List<IImageUriLoaderClient> clients = new List<IImageUriLoaderClient>();
		ImageUriLoader loader;
		protected bool CanStartLoadImage { get { return LoadedImage == null && Clients.Count == 0; } }
		public Uri ImageUri { get { return imageUri; } }
		public Image LoadedImage { get { return loadedImage; } }
		public List<IImageUriLoaderClient> Clients { get { return clients; } }
		public ImageCacheItem(Uri imageUri) {
			Guard.ArgumentNotNull(imageUri, "imageUri");
			this.imageUri = imageUri;
		}
		public void RegisterClient(IImageUriLoaderClient client) {
			if(CanStartLoadImage)
				StartLoadImage();
			if(LoadedImage != null)
				NotifyClient(client);
			else
				Clients.Add(client);
		}
		protected virtual void StartLoadImage() {
			if(loader == null) {
				loader = new ImageUriLoader(this);
			}
			loader.LoadImage(imageUri);
		}
		void IImageUriLoaderClient.OnImageLoaded(Image image) {
			if(LoadedImage == null) {
				this.loadedImage = image;
				if(loader != null) {
					loader.Dispose();
					loader = null;
				}
				lock(Clients) {
					foreach(IImageUriLoaderClient client in Clients) {
						NotifyClient(client);
					}
				}
				Clients.Clear();
			}
		}
		void NotifyClient(IImageUriLoaderClient client) {
			client.OnImageLoaded(LoadedImage);
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			Clients.Clear();
			if(loader != null) {
				loader.Dispose();
				loader = null;
			}
			if(loadedImage != null) {
				loadedImage.Dispose();
				loadedImage = null;
			}
		}
	}
	public class ImageUriLoader : MapDisposableObject, IImageUriLoader, IWebStreamClient {
		IImageUriLoaderClient client;
		MapWebLoader webLoader;
		protected MapWebLoader WebLoader {
			get {
				if(webLoader == null) {
					webLoader = new MapWebLoader();
					SubscribeWebClientEvents();
				}
				return webLoader;
			}
		}
		public ImageUriLoader(IImageUriLoaderClient client) {
			this.client = client;
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			if(webLoader != null) {
				UnsubscribeWebClientEvents();
				webLoader.Dispose();
				webLoader = null;
			}
			client = null;
		}
		public virtual void LoadImage(Uri imageUri) {
			if(imageUri == null) {
				NotifyClientOnLoad(null);
				return;
			}
			if(MapUtils.IsFileUri(imageUri)) {
				LoadFromFile(imageUri);
				return;
			}
			LoadFromWeb(imageUri);
		}
		protected virtual void LoadFromFile(Uri imageUri) {
			Image result = LoadImageFromFile(imageUri.LocalPath);
			NotifyClientOnLoad(result);
		}
		protected virtual void LoadFromWeb(Uri imageUri) {
			WebLoader.ReadAsync(imageUri);
		}
		void SubscribeWebClientEvents() {
			if(webLoader != null)
				webLoader.LoadComlete += OnOpenReadCompleted;
		}
		void UnsubscribeWebClientEvents() {
			if(webLoader != null)
				webLoader.LoadComlete -= OnOpenReadCompleted;
		}
		void OnOpenReadCompleted(object sender, MapWebLoaderEventArgs e) {
			if(MapUtils.IsOperationComplete(e)) {
				IWebStreamClient streamClient = (IWebStreamClient)this;
				streamClient.OnStreamOpenComplete(e.Stream);
			}
		}
		void NotifyClientOnLoad(Image image) {
			client.OnImageLoaded(image);
		}
		void IWebStreamClient.OnStreamOpenComplete(Stream stream) {
			Image result = LoadImageFromStream(stream);
			NotifyClientOnLoad(result);
		}
		Image LoadImageFromFile(string filePath) {
			try {
				return ImageHelper.LoadImageFromFileEx(filePath);
			} catch {
				return null;
			}
		}
		Image LoadImageFromStream(Stream stream) {
			if(stream == null || stream == Stream.Null)
				return null;
			Image result;
			try {
				result = Image.FromStream(stream);
			} catch {
				result = null;
			} finally {
				stream.Close();
			}
			return result;
		}
	}
	public interface IWebStreamClient {
		void OnStreamOpenComplete(Stream stream);
	}
}
