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
using System.Net;
using DevExpress.XtraMap.Native;
using System.ComponentModel;
namespace DevExpress.XtraMap {
	public abstract class MultiScaleTileSource : MapDisposableObject {
		readonly long imageWidth;
		readonly long imageHeight;
		readonly int tileWidth;
		readonly int tileHeight;
		readonly int maxZoomLevel;
		readonly double tileCountRatio;
		readonly ICacheOptionsProvider cacheOptionsProvider;
		TileSourceCache cache;
		readonly object cacheLocker = new object();
		protected internal TileSourceCache Cache {
			get { return cache; }
			private set {
				if(cache != null) 
					UnsibscribeCacheEvents();
				cache = value;
				if(cache != null) {
					SibscribeCacheEvents();
					cache.CacheOptionsUpdated();
				}
			}
		}
		protected internal virtual string Referer { get { return ""; } }
		protected internal virtual string TilePrefix { get { return ""; } }
		protected internal long ImageWidth { get { return imageWidth; } }
		protected internal long ImageHeight { get { return imageHeight; } }
		internal int TileWidth { get { return tileWidth; } }
		internal int TileHeight { get { return tileHeight; } }
		internal double TileCountRatio { get { return tileCountRatio; } }
		internal int MaxZoomLevel { get { return maxZoomLevel; } }
		internal CacheOptions CacheOptions {
			get {
				if(cacheOptionsProvider != null)
					return cacheOptionsProvider.CacheOptions;
				return null;
			}
		}
		internal event MapWebRequestEventHandler WebRequest;
		protected MultiScaleTileSource(long imageWidth, long imageHeight, int tileWidth, int tileHeight, ICacheOptionsProvider cacheOptionsProvider) {
			DevExpress.Utils.Guard.ArgumentPositive(tileWidth, "tileWidth");
			DevExpress.Utils.Guard.ArgumentPositive(tileHeight, "tileHeight");
			this.imageWidth = imageWidth;
			this.imageHeight = imageHeight;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
			this.cacheOptionsProvider = cacheOptionsProvider;
			if((this.imageWidth != 0.0) && (this.tileHeight != 0.0))
				this.tileCountRatio = (double)imageHeight / (double)imageWidth * ((double)tileWidth / (double)tileHeight);
			this.maxZoomLevel = (int)Math.Log(ImageWidth / TileWidth, 2.0);
			CreateCache();
		}
		protected override void DisposeOverride() {
			DisposeCache();
		}
		void DisposeCache() {
			lock(cacheLocker) {
				if(Cache != null) {
					Cache.Dispose();
					Cache = null;
				}
			}
		}
		void UnsibscribeCacheEvents(){
			if(CacheOptions != null)
				CacheOptions.PropertyChanged -= CacheOptionsPropertyChanged;
		}
		void SibscribeCacheEvents(){
			if(CacheOptions != null)
				CacheOptions.PropertyChanged += CacheOptionsPropertyChanged;
		}
		void CreateCache() {
			if(Cache == null)
				Cache = new TileSourceCache(this);
		}
		void CacheOptionsPropertyChanged(object sender, PropertyChangedEventArgs e) {
			Cache.CacheOptionsUpdated();
		}
		void RaiseWebRequest(MapWebRequestEventArgs e) {
			if(WebRequest != null)
				WebRequest(this, e);
		}
		protected abstract Uri GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY);
		protected internal void DisposeInternalResources() {
			DisposeCache();
		}
		protected internal void CreateInternalResources() {
			CreateCache();
		}
		internal TileImageSource GetTileImageSource(TileIndex index) {
			lock (cacheLocker) {
				if(TileIndex.IsInvalid(index) || Cache == null)
					return null;
				TileImageSource tileSource = Cache.Retrieve(index);
				if(tileSource == null) {
					Uri uri = GetTileLayers(index.Level, index.X, index.Y);
					tileSource = TileImageSource.GetTileImageSource(index, uri, new IntSize(tileWidth, tileHeight), Referer);
					if(tileSource != null)
						Cache.Push(tileSource);
				} else if(tileSource is DiskImageSource)
					Cache.Push(tileSource);
				return tileSource;
			}
		}
		internal TileImageSource PeekTileImageSource(TileIndex index) {
			TileImageSource source = Cache != null ? Cache.Retrieve(index) : null;
			if((source != null) && (source.Source != null) && (source.Status == TileStatus.Ready))
				return source;
			return null;
		}
		internal void OnWebRequest(MapWebRequestEventArgs e){
			RaiseWebRequest(e);
		}
	}
	public abstract class MapTileSourceBase : MultiScaleTileSource {
		const int tileLevelCorrection = 8;
		int currentSubdomainIndex = 0;
		protected MapTileSourceBase(int imageWidth, int imageHeight, int tileWidth, int tileHeight, ICacheOptionsProvider cacheOptionsProvider)
			: base(imageWidth, imageHeight, tileWidth, tileHeight, cacheOptionsProvider) {
		}
		protected int GetSubdomainIndex(int subdomainCount) {
			currentSubdomainIndex = currentSubdomainIndex + 2 > subdomainCount ? 0 : currentSubdomainIndex + 1;
			return currentSubdomainIndex;
		}
		protected override Uri GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY) {
			if(tileLevel > 0 && tileLevel <= MaxZoomLevel)
				return GetTileByZoomLevel(tileLevel, tilePositionX, tilePositionY);
			return null;
		}
		public abstract Uri GetTileByZoomLevel(int zoomLevel, int tilePositionX, int tilePositionY);
	}
}
