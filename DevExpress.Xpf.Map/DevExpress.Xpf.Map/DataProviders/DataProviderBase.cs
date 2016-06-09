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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public abstract class MapDataProviderBase : MapDependencyObject {
		internal static readonly DependencyPropertyKey TileSourcePropertyKey = DependencyPropertyManager.RegisterReadOnly("TileSource", typeof(MapTileSourceBase), typeof(MapDataProviderBase), new PropertyMetadata(null, TileSourcePropertyChanged));
		public static readonly DependencyProperty TileSourceProperty = TileSourcePropertyKey.DependencyProperty;
		static void TileSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapDataProviderBase provider = d as MapDataProviderBase;
			if (provider != null)
				provider.OnTileSourceChanged(e.OldValue as MapTileSourceBase, e.NewValue as MapTileSourceBase);
		}
		[Category(Categories.Data)]
		public MapTileSourceBase TileSource {
			get { return (MapTileSourceBase)GetValue(TileSourceProperty); }
		}
		public event MapWebRequestEventHandler WebRequest;
		internal event TileSourceChangedEventHandler TileSourceChanged;
		protected internal virtual bool ShouldShowInvalidKeyMessage { get { return false; } }
		public abstract ProjectionBase Projection { get; }
		void RaiseTileSourceChanged(MapTileSourceBase oldSource, MapTileSourceBase newSource) {
			if (TileSourceChanged != null)
				TileSourceChanged(this, new TileSourceChangedEventArgs(oldSource, newSource));
		}
		void OnTileSourceChanged(MapTileSourceBase oldSource, MapTileSourceBase newSource) {
			RaiseTileSourceChanged(oldSource, newSource);			
			if (oldSource != null)
				oldSource.TileWebRequest -= OnTileWebRequest;
			if (newSource != null)
				newSource.TileWebRequest += OnTileWebRequest;
		}	   
		void OnTileWebRequest(object sender, MapWebRequestEventArgs e) {
			RaiseWebRequest(e);
		}
		protected void RaiseWebRequest(MapWebRequestEventArgs e) {
			if (WebRequest != null)
				WebRequest(this, e);
		}
		protected internal virtual string GetInvalidKeyMessage() {
			return string.Empty;
		}
		public void SetTileSource(MapTileSourceBase value) {
			this.SetValue(TileSourcePropertyKey, value);
		}
		public abstract Size GetMapSizeInPixels(double zoomLevel);
	}
	public abstract class MapTileSourceBase : MultiScaleTileSource {
		const int tileLevelCorrection = 8;
		int currentSubdomainIndex = 0;
		public MapTileSourceBase(int imageWidth, int imageHeight, int tileWidth, int tileHeight) : base(imageWidth, imageHeight, tileWidth, tileHeight, 0) {
		}
		int ConvertTileLevelToZoomLevel(int tileLevel) {
			return tileLevel - tileLevelCorrection;
		}
		protected int GetSubdomainIndex(int subdomainCount) {
			currentSubdomainIndex = currentSubdomainIndex + 2 > subdomainCount ? 0 : currentSubdomainIndex + 1;
			return currentSubdomainIndex;			
		}
		protected override void GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY, IList<object> tileImageLayerSources) {
			int zoomLevel = ConvertTileLevelToZoomLevel(tileLevel);
			if (zoomLevel > 0 && zoomLevel < 25) {
				Uri tile = this.GetTileByZoomLevel(zoomLevel, tilePositionX, tilePositionY);
				if (tile != null)
					tileImageLayerSources.Add(tile);
			}
		}
		public abstract Uri GetTileByZoomLevel(int zoomLevel, int tilePositionX, int tilePositionY);
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class TileSourceChangedEventArgs : EventArgs {
		readonly MapTileSourceBase oldValue;
		readonly MapTileSourceBase newValue;
		public MapTileSourceBase OldValue { get { return oldValue;} }
		public MapTileSourceBase NewValue { get { return newValue;} }
		internal TileSourceChangedEventArgs(MapTileSourceBase oldValue, MapTileSourceBase newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
	public delegate void TileSourceChangedEventHandler(object sender, TileSourceChangedEventArgs e);
}
