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
using System.Windows.Forms;
namespace DevExpress.XtraMap {
	public abstract class ColorLegendBase : ItemsLayerLegend {
	}
	public enum LegendItemsSortOrder { Ascending, Descending }
	public class ColorListLegend : ColorLegendBase, IImageCollectionHelper {
		LegendItemsSortOrder sortOrder = LegendItemsSortOrder.Descending;
		object imageList;
		[DefaultValue(LegendItemsSortOrder.Descending), 
		Category(SRCategoryNames.Appearance),
#if !SL
	DevExpressXtraMapLocalizedDescription("ColorListLegendSortOrder"),
#endif
]
		public LegendItemsSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(sortOrder == value)
					return;
				this.sortOrder = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ColorListLegendCustomItems"),
#endif
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(CollectionConverter)),
		Editor("DevExpress.XtraMap.Design.MapColorLegendItemCollectionEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor))
		]
		public new MapLegendItemCollection CustomItems {
			get { return base.CustomItems; }
		}
		[DefaultValue(null),
		Category(SRCategoryNames.Appearance),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ImageList {
			get { return imageList; }
			set {
				if (imageList == value)
					return;
				UnsubscribeImageCollectionEvents(imageList);
				imageList = value;
				SubscribeImageCollectionEvents(imageList);
				OnImageListChanged();
			}
		}
		#region IImageCollectionHelper members
		Control IImageCollectionHelper.OwnerControl {
			get { return Map != null ? Map.OwnedControl as MapControl : null; }
		}
		#endregion
		void UnsubscribeImageCollectionEvents(object imageCollection) {
			ImageCollection oldImageCollection = imageCollection as ImageCollection;
			if (oldImageCollection != null)
				oldImageCollection.Changed -= ImageListChanged;
		}
		void SubscribeImageCollectionEvents(object imageCollection) {
			ImageCollection newImageCollection = imageCollection as ImageCollection;
			if (newImageCollection != null)
				newImageCollection.Changed += ImageListChanged;
		}
		void ImageListChanged(object sender, EventArgs e) {
			OnImageListChanged();
		}
		void OnImageListChanged() {
			if (ImageList != null)
				OnChanged();
		}
		protected override void AddInnerItems(IList<MapLegendItemBase> items) {
			int count = items.Count;
			for(int i = count - 1; i >= 0; i--) {
				MapLegendItemBase item = items[i];
				InnerItems.Add(item);
				RaiseItemCreating(item, i);
			}
		}
		protected override IList<MapLegendItemBase> GetCustomItems() {
			List<MapLegendItemBase> list = new List<MapLegendItemBase>(CustomItems);
			list.Reverse();
			return list;
		}
		public override string ToString() {
			return "(ColorListLegend)";
		}
	}
	public class ColorScaleLegend : ColorLegendBase {
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ColorScaleLegendCustomItems"),
#endif
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(CollectionConverter)),
		Editor("DevExpress.XtraMap.Design.MapColorLegendItemCollectionEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor))
		]
		public new MapLegendItemCollection CustomItems {
			get { return base.CustomItems; }
		}
		public override string ToString() {
			return "(ColorScaleLegend)";
		}
	}
}
