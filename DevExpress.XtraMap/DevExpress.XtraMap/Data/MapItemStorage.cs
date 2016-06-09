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
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.XtraMap.Design;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class MapItemStorage : MapDataAdapterBase, IMapCoordSystemProvider {
		bool isReady;
		protected override bool IsReady { get { return isReady; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemStorageItems"),
#endif
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(CollectionConverter)),
		Editor("DevExpress.XtraMap.Design.MapItemCollectionEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor))
		]
		public MapItemCollection Items { get { return base.InnerItems; } }
		#region ISupportGetMapCoordinateSystem implementation
		MapCoordinateSystem IMapCoordSystemProvider.GetMapCoordSystem() {
			return Layer == null ? null : (Layer.Map == null ? null : Layer.Map.CoordinateSystem);
		}
		#endregion
		protected override void LoadData(IMapItemFactory factory) {
			foreach (MapItem item in Items)
				item.RegisterLayoutUpdate();
			this.isReady = true;
		}
		protected override object GetItemSourceObject(MapItem item) {
			return item;
		}
		protected override MapItem GetItemBySourceObject(object sourceObject) {
			return null;
		}
		protected override bool IsCSCompatibleTo(MapCoordinateSystem mapCS) {
			return true;
		}
		protected override void SetLayer(MapItemsLayerBase layer) {
			base.SetLayer(layer);
			this.isReady = false;
		}
		protected internal override MapItemType GetDefaultMapItemType() {
			return MapItemType.Unknown;
		}
		public override string ToString() {
			return "(MapItemStorage)";
		}
	}
}
