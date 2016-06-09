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
using System.Drawing;
using System.Linq;
using DevExpress.XtraMap.Native;
using System.ComponentModel;
namespace DevExpress.XtraMap {
	public abstract class MapDataProviderBase : MapDisposableObject, ICacheOptionsProvider, IOwnedElement {
		MapTileSourceBase tileSource;
		LayerBase layer;
		readonly CacheOptions cacheOptions;
		protected internal virtual bool IsUpdateEnabled { get { return !IsDisposed && layer != null && !layer.IsDesignMode; } }
		protected internal bool ShouldUpdateTileSource { get; set; }
		protected internal LayerBase Layer { get { return layer; } }
		protected internal virtual Size BaseSizeInPixels { get { return new Size(InnerMap.DefaultMapSize, InnerMap.DefaultMapSize); } }
		protected internal virtual bool ShouldShowInvalidKeyMessage { get { return false; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapDataProviderBaseProjection")]
#endif
		public abstract ProjectionBase Projection { get; }
		[Category(SRCategoryNames.Data), Browsable(false)]
		public MapTileSourceBase TileSource {
			get { return tileSource; }
			set {
				if (tileSource == value)
					return;
				RaiseTileSourceChanged(tileSource, value);
				ReleaseTileSource();
				tileSource = value;
				tileSource.WebRequest += OnTileWebRequest;
			}
		}
		internal event EventHandler<GenericPropertyChangedEventArgs<MapTileSourceBase>> TileSourceChanged;
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapDataProviderBaseWebRequest")]
#endif
		public event MapWebRequestEventHandler WebRequest;
		protected MapDataProviderBase() {
			cacheOptions = new CacheOptions();
		}
		void ReleaseTileSource() {
			if(tileSource != null) {
				tileSource.WebRequest -= OnTileWebRequest;
				tileSource.Dispose();
				tileSource = null;
			}
		}
		void OnTileWebRequest(object sender, MapWebRequestEventArgs e) {
			RaiseWebRequest(e);
		}
		protected internal bool CanUpdateTileSource() {
			return ShouldUpdateTileSource && IsUpdateEnabled;
		}
		protected void RaiseWebRequest(MapWebRequestEventArgs e) {
			if(WebRequest != null)
				WebRequest(this, e);
		}
		protected void Invalidate() {
			if(IsUpdateEnabled)
				layer.InvalidateRender();
		}
		#region ICacheOptionsProvider Members
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapDataProviderBaseCacheOptions"),
#endif
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public CacheOptions CacheOptions { get { return cacheOptions; } }
		#endregion
		#region IOwnedElement Members
		object IOwnedElement.Owner {
			get { return layer; }
			set {
				if (layer != value)
					layer = value as LayerBase;
			}
		}
		#endregion
		void RaiseTileSourceChanged(MapTileSourceBase oldValue, MapTileSourceBase newValue) {
			if (TileSourceChanged != null)
				TileSourceChanged(this, new GenericPropertyChangedEventArgs<MapTileSourceBase>(oldValue, newValue));
		}
		protected override void DisposeOverride() {
			this.layer = null;
			ReleaseTileSource();
		}
		protected internal virtual void PrepareData() {
		}
		protected internal void DisposeInternalResources() {
			if(tileSource != null)
				tileSource.DisposeInternalResources();
		}
		protected internal void CreateInternalResources() {
			if(tileSource != null)
				tileSource.CreateInternalResources();
		}
		public abstract MapSize GetMapSizeInPixels(double zoomLevel);
	}
}
namespace DevExpress.XtraMap.Native {
	public class GenericPropertyChangedEventArgs<Type> : EventArgs {
		readonly Type oldValue;
		readonly Type newValue;
		public Type OldValue { get { return oldValue; } }
		public Type NewValue { get { return newValue; } }
		internal GenericPropertyChangedEventArgs(Type oldValue, Type newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
}
