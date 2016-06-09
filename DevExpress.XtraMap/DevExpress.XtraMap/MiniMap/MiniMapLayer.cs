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

using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap {
	public abstract class MiniMapLayerBase : MapDisposableObject {
		string name = string.Empty;
		protected internal LayerBase InnerLayer { get; set; }
		protected internal MiniMap MiniMap { get { return InnerLayer != null ? ((IOwnedElement)InnerLayer).Owner as MiniMap : null; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapLayerBaseName"),
#endif
 DefaultValue(""), Category(SRCategoryNames.Map)]
		public string Name {
			get { return name; }
			set {
				if(value == null) value = string.Empty;
				if(string.Compare(name, value, StringComparison.InvariantCulture) == 0)
					return;
				string oldName = name;
				this.name = value;
				RaiseNameChanged(name, oldName);
			}
		}
		protected MiniMapLayerBase() {
			InnerLayer = CreateInnerLayer();
		}
		protected abstract LayerBase CreateInnerLayer();
		protected override void DisposeOverride() {
			if(InnerLayer != null) {
				InnerLayer.Dispose();
				InnerLayer = null;
			}
			base.DisposeOverride();
		}
		internal event NameChangedEventHandler NameChanged;
		protected internal virtual void RaiseNameChanged(string name, string oldName) {
			if(NameChanged != null) {
				NameChangedEventArgs args = new NameChangedEventArgs(name, oldName);
				NameChanged(this, args);
			}
		}
		public override string ToString() {
			return "(MiniMapLayerBase)";
		}
		protected internal void SetClientSize(Size size) {
			InnerLayer.OnSetClientSize(size);
		}
		protected internal virtual void UpdateLayersImages(object imageList) {
		}
		protected internal virtual void ViewportUpdated() {
			InnerLayer.ViewportUpdated();
		}
	}
	public class MiniMapVectorItemsLayer : MiniMapLayerBase {
		protected internal VectorItemsLayer ItemsLayer { get { return (VectorItemsLayer)base.InnerLayer; } }
		[Category(SRCategoryNames.Data), DefaultValue(null),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapVectorItemsLayerData"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.MapDataAdapterPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)]
		public IMapDataAdapter Data {
			get { return ItemsLayer != null ? ItemsLayer.Data : null; }
			set {
				if(ItemsLayer != null)
					ItemsLayer.Data = value;
			}
		}
		[Category(SRCategoryNames.Appearance),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapVectorItemsLayerItemStyle"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter))]
		public MapItemStyle ItemStyle { get { return ItemsLayer.ItemStyle; } }
		[Category(SRCategoryNames.Appearance),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapVectorItemsLayerItemImageIndex"),
#endif
		DefaultValue(MapPointer.DefaultImageIndex)]
		public int ItemImageIndex {
			get { return ItemsLayer.ItemImageIndex; }
			set { ItemsLayer.ItemImageIndex = value; }
		}
		public MiniMapVectorItemsLayer() {
		}
		protected override LayerBase CreateInnerLayer() {
			return new VectorItemsLayer() { EnableSelection = false };
		}
		protected internal override void UpdateLayersImages(object imageList) {
			ItemsLayer.UpdateImageHolders(imageList);
		}
		public override string ToString() {
			return "(MiniMapVectorItemsLayer)";
		}
	}
	public class MiniMapImageTilesLayer : MiniMapLayerBase {
		protected internal ImageTilesLayer TilesLayer { get { return (ImageTilesLayer)base.InnerLayer; } }
		[DefaultValue(null), Category(SRCategoryNames.Data),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapImageTilesLayerDataProvider"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraMap.Design.DataProviderPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))]
		public MapDataProviderBase DataProvider {
			get { return TilesLayer != null ? TilesLayer.DataProvider : null; }
			set {
				if (TilesLayer != null) {
					if (TilesLayer.DataProvider == value)
						return;
					TilesLayer.DataProvider = value;
					OnDataProviderChanged();
				}
			}
		}
		public MiniMapImageTilesLayer() { 
		}
		void OnDataProviderChanged() {
			SubscribeToTileSourceChanged();
			ResetErrorPanel();
		}
		void SubscribeToTileSourceChanged() {
			if(DataProvider != null)
				DataProvider.TileSourceChanged += OnTileSourceChanged;
		}
		void UnsubscribeFromTileSourceChanged() {
			if (DataProvider != null)
				DataProvider.TileSourceChanged -= OnTileSourceChanged;
		}
		void OnTileSourceChanged(object sender, GenericPropertyChangedEventArgs<MapTileSourceBase> e) {
			ResetErrorPanel();
		}
		void ResetErrorPanel() {
			if (MiniMap != null)
				MiniMap.ResetErrorPanel();
		}
		protected override void DisposeOverride() {
			UnsubscribeFromTileSourceChanged();
			base.DisposeOverride();
		}
		protected override LayerBase CreateInnerLayer() {
			return new ImageTilesLayer();
		}
		public override string ToString() {
			return "(MiniMapImageTilesLayer)";
		}
	}
}
