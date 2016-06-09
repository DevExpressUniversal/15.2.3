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
using DevExpress.Utils;
using DevExpress.Map;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap.Native {
	public sealed class MapItemMappingNames {
		private MapItemMappingNames() { }
		public const string Type = "Type";
		public const string XCoordinate = "XCoordinate";
		public const string YCoordinate = "YCoordinate";
		public const string Text = "Text";
		public const string ImageIndex = "ImageIndex";
		public const string GroupIndex = "GroupIndex";
		public const string Value = "Value";
		public const string Argument = "Argument";
	}
	public interface ILayerDataManagerProvider {
		LayerDataManager DataManager { get; }
	}
	public class MappingCollection : NamedItemNotificationCollection<MapItemMappingBase> {
		protected override string GetItemName(MapItemMappingBase item) {
			return item.Name;
		}
	}
	public abstract class MapItemMappingBase {
		string member = string.Empty;
		public abstract string Name { get; }
		public virtual string Member { get { return member; } set { member = value; } }
		protected internal SourceCoordinateSystem CoordinateSystem {
			get;
			set;
		}
		protected MapItemMappingBase() {
			this.member = Name;
		}
		bool ShouldUseDefaultValue(object userObject) {
			return userObject == null || userObject is DBNull;
		}
		protected abstract void SetValueInternal(IMapDataItem item, object obj);
		protected abstract void SetDefaultValueInternal(IMapDataItem item, object obj);
		public void SetValue(IMapDataItem item, object obj) {
			try {
				if(ShouldUseDefaultValue(obj))
					SetDefaultValueInternal(item, obj);
				else
					SetValueInternal(item, obj);
			} catch {
				;
			}
		}
	}
	public class YCoordinateMapping : MapItemMappingBase {
		public override string Name { get { return MapItemMappingNames.YCoordinate; } }
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation coordItem = item as ISupportCoordLocation;
			if(coordItem != null) {
				coordItem.Location = CoordinateSystem.CreatePoint(coordItem.Location.GetX(), 0.0);
			}
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation coordItem = item as ISupportCoordLocation;
			if(coordItem != null) {
				coordItem.Location = CoordinateSystem.CreatePoint(coordItem.Location.GetX(), MapUtils.ConvertToDouble(obj));
			}
		}
	}
	public class XCoordinateMapping : MapItemMappingBase {
		public override string Name { get { return MapItemMappingNames.XCoordinate; } }
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation coordItem = item as ISupportCoordLocation;
			if(coordItem != null) {
				coordItem.Location = CoordinateSystem.CreatePoint(0.0, coordItem.Location.GetY()); 
			}
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation coordItem = item as ISupportCoordLocation;
			if(coordItem != null) {
				coordItem.Location = CoordinateSystem.CreatePoint(MapUtils.ConvertToDouble(obj), coordItem.Location.GetY()); 
			}
		}
	}
	public class TextMapping : MapItemMappingBase {
		public override string Name { get { return MapItemMappingNames.Text; } }
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			MapPointer pointerElement = item as MapPointer;
			if(pointerElement != null)
				pointerElement.Text = string.Empty;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			MapPointer pointerElement = item as MapPointer;
			if(pointerElement != null)
				pointerElement.Text = obj.ToString();
		}
	}
	public class TypeMapping : MapItemMappingBase {
		MapItemType type = MapItemType.Unknown;
		public override string Name { get { return MapItemMappingNames.Type; } }
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			this.type = MapItemType.Unknown;
		}
		public MapItemType Type { get { return type; } }
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			int typeInt = Convert.ToInt32(obj);
			this.type = (MapItemType)typeInt;
		}
	}
	public class ImageIndexMapping : MapItemMappingBase {
		public override string Name { get { return MapItemMappingNames.ImageIndex; } }
		public ImageIndexMapping() {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			MapPointer pointer = item as MapPointer;
			if(pointer != null)
				pointer.ImageIndex = MapPointer.DefaultImageIndex;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			MapPointer pointer = item as MapPointer;
			if(pointer != null)
				pointer.ImageIndex = Convert.ToInt32(obj);
		}
	}
	public class ChartItemGroupKeyMapping : MapItemMappingBase {
		public override string Name { get { return MapItemMappingNames.GroupIndex; } }
		public ChartItemGroupKeyMapping() {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			IMapChartGroupDataItem chartItem = item as IMapChartGroupDataItem;
			if(chartItem != null) chartItem.GroupKey = null;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			IMapChartGroupDataItem chartItem = item as IMapChartGroupDataItem;
			if(chartItem != null) chartItem.GroupKey = obj;
		}
	}
	public class ChartItemArgumentMapping : MapItemMappingBase {
		public override string Name { get { return MapItemMappingNames.Argument; } }
		public ChartItemArgumentMapping() {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			IMapChartItem element = item as IMapChartItem;
			if(element == null) return;
			IMapChartDataItem chartItem = item as IMapChartDataItem;
			if(chartItem != null) chartItem.Argument = null;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			IMapChartItem element = item as IMapChartItem;
			if(element == null) return;
			IMapChartDataItem chartItem = item as IMapChartDataItem;
			if(chartItem != null) chartItem.Argument = obj;
		}
	}
	public class ChartItemValueMapping : MapItemMappingBase {
		public override string Name { get { return MapItemMappingNames.Value; } }
		public ChartItemValueMapping() {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			IMapChartDataItem chartItem = item as IMapChartDataItem;
			if(chartItem != null) chartItem.Value = double.NaN;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			IMapChartDataItem chartItem = item as IMapChartDataItem;
			if(chartItem != null) chartItem.Value = MapUtils.ConvertToDouble(obj);
		}
	}
	public class PieSegmentArgumentMapping : MapItemMappingBase {
		public override string Name { get { return MapItemMappingNames.Argument; } }
		public PieSegmentArgumentMapping() {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			PieSegment segment = item as PieSegment;
			if(segment != null) segment.Argument = null;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			PieSegment segment = item as PieSegment;
			if(segment != null) segment.Argument = obj;
		}
	}
}
