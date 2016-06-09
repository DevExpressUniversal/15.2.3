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
using System.Windows;
namespace DevExpress.Xpf.Map {
	public abstract class MultiScaleTileSource : DependencyObject {
		TimeSpan tileBlendTime;
		long imageWidth;
		long imageHeight;
		int tileWidth;
		int tileHeight;
		int tileOverlap;
		int zoomLevelOffset;
		int maxZoomLevel;
		internal long ImageWidth { get { return imageWidth; } }
		internal long ImageHeight { get { return imageHeight; } }
		internal int TileWidth { get { return tileWidth; } }
		internal int TileHeight { get { return tileHeight; } }
		internal int TileOverlap { get { return tileOverlap; } }
		internal TimeSpan BlendTime { get { return tileBlendTime; } }
		internal int ZoomLevelOffset { get { return zoomLevelOffset; } }
		internal int MaxZoomLevel { get { return maxZoomLevel; } }
		protected TimeSpan TileBlendTime {
			get { return tileBlendTime; }
			set { tileBlendTime = value; }
		}
		internal event MapWebRequestEventHandler TileWebRequest;
		public MultiScaleTileSource(int imageWidth, int imageHeight, int tileWidth, int tileHeight, int tileOverlap)
			: this((long)imageWidth, (long)imageHeight, tileWidth, tileHeight, tileOverlap) {
		}
		public MultiScaleTileSource(long imageWidth, long imageHeight, int tileWidth, int tileHeight, int tileOverlap) {
			this.imageWidth = imageWidth;
			this.imageHeight = imageHeight;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
			this.tileOverlap = tileOverlap;
			tileBlendTime = TimeSpan.FromSeconds(0.5);
			zoomLevelOffset = Convert.ToInt32(Math.Log((double)TileWidth, 2.0));
			maxZoomLevel = Convert.ToInt32(Math.Log((double)ImageWidth, 2.0));
		}
		void RaiseTileWebRequest(MapWebRequestEventArgs e) {
			if (TileWebRequest != null)
				TileWebRequest(this, e);
		}
		internal Uri GetTileImageSource(int tileLevel, int tilePositionX, int tilePositionY) {
			List<object> tileImageLayerSources = new List<object>();
			GetTileLayers(tileLevel, tilePositionX, tilePositionY, tileImageLayerSources);
			return tileImageLayerSources.Count > 0 ? tileImageLayerSources[0] as Uri : null;
		}
		internal void OnTileWebRequest(MapWebRequestEventArgs e) {
			RaiseTileWebRequest(e);			
		}
		protected abstract void GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY, IList<object> tileImageLayerSources);
	}
}
