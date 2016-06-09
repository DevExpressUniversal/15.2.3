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

using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public interface ILegendDataProvider {
		IList<MapLegendItemBase> CreateItems(MapLegendBase legend);
	}
	public abstract class DataProviderLegend : MapLegendBase {
		bool shouldLoadItems = false;
		protected abstract internal ILegendDataProvider DataProvider { get; }
		protected bool ShouldLoadItems { get { return shouldLoadItems; } set { shouldLoadItems = value; } }
		protected void ClearItems() {
			InnerItems.Clear();
		}
		protected internal virtual void PopulateItems() {
			ClearItems();
			if(DataProvider == null) return;
			IList<MapLegendItemBase> items = DataProvider.CreateItems(this);
			AddInnerItems(items);
		}
		protected virtual void RaiseItemCreating(MapLegendItemBase item, int index) {
			if(Map != null) Map.RaiseLegendItemCreating(new LegendItemCreatingEventArgs(this, index, item));
		}
		protected virtual void AddInnerItems(IList<MapLegendItemBase> items) {
			for(int i = 0; i < items.Count; i++) {
				MapLegendItemBase item = items[i];
				InnerItems.Add(item);
				RaiseItemCreating(item, i);
			}
		}
		protected internal override bool CanDisplayItems() {
			if(DataProvider == null)
				return CustomItems.Count > 0;
			IList<MapLegendItemBase> items = GetItems();
			return items != null ? items.Count > 0 : false;
		}
		protected internal override IList<MapLegendItemBase> GetItems() {
			if(DataProvider == null)
				return GetCustomItems();
			EnsureItemsLoaded();
			return InnerItems;
		}
		protected virtual IList<MapLegendItemBase> GetCustomItems() {
			return CustomItems;
		}
		protected internal override void EnsureItemsLoaded() {
			if(shouldLoadItems) {
				PopulateItems();
				shouldLoadItems = false;
			}
			base.EnsureItemsLoaded();
		}
		protected internal override void Invalidate() {
			ShouldLoadItems = true;
		}
	}
	public abstract class ItemsLayerLegend : DataProviderLegend {
		MapItemsLayerBase layer;
		protected internal override ILegendDataProvider DataProvider { get { return Layer as ILegendDataProvider; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ItemsLayerLegendLayer"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(null),
		TypeConverter("DevExpress.XtraMap.Design.MapItemsLayerTypeConverter, " + AssemblyInfo.SRAssemblyMapDesign)
		]
		public MapItemsLayerBase Layer {
			get { return layer; }
			set {
				if (Object.Equals(layer, value))
					return;
				this.layer = value;
				OnLayerChanged();
			}
		}
		protected internal override bool ActualVisible {
			get {
				bool visible = base.ActualVisible;
				if(visible && Layer != null)
					return Layer.CheckVisibility();
				return visible;
			}
		}
		protected virtual void OnLayerChanged() {
			if(Layer == null)
				ClearItems();
			DoInvalidateUpdate();
		}
		internal void DoInvalidateUpdate() {
			Invalidate();
			OnChanged();
		}
		protected internal override bool CanDisplayItems() {
			if(Layer != null) {
				IColorizerLegendDataProvider dynamicProvider = Layer.Colorizer as IColorizerLegendDataProvider;
				if(dynamicProvider != null)
					return true; 
			}
			return base.CanDisplayItems();
		}
	}
}
