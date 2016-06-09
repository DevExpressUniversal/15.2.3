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
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Map;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map.Native {
	public enum MappingType {
		CoordX,
		CoordY,
		Id,
		Value,
		SegmentId
	}
	public class MappingDictionary : Dictionary<MappingType, MapItemMappingBase> {
	}
	public abstract class MapItemMappingBase {
		readonly string member;
		public string Member { get { return member; } }
		public SourceCoordinateSystem CoordinateSystem { get; set; }
		protected MapItemMappingBase(string member) {
			this.member = member;
		}
		bool ShouldUseDefaultValue(object userObject) {
			return userObject == null || userObject is DBNull;
		}
		protected double ConvertToDouble(object val) {
			double result = 0.0f;
			string s = val as string;
			if (!String.IsNullOrEmpty(s)) {
				if (double.TryParse(s, out result))
					return result;
				return double.Parse(s, CultureInfo.InvariantCulture);
			}
			else
				return Convert.ToDouble(val);
		}
		protected abstract void SetValueInternal(IMapDataItem item, object obj);
		protected abstract void SetDefaultValueInternal(IMapDataItem item, object obj);
		public void SetValue(IMapDataItem item, object obj) {
			try {
				if (ShouldUseDefaultValue(obj))
					SetDefaultValueInternal(item, obj);
				else
					SetValueInternal(item, obj);
			}
			catch {
			}
		}
	}
	public class ChartItemIdMapping : MapItemMappingBase {
		public ChartItemIdMapping(string member) : base(member) {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			IMapChartItem element = item as IMapChartItem;
			if (element == null)
				return;
			IMapChartDataItem chartItem = item as IMapChartDataItem;
			if (chartItem != null)
				chartItem.Argument = null;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			IMapChartItem element = item as IMapChartItem;
			if (element == null)
				return;
			IMapChartDataItem chartItem = item as IMapChartDataItem;
			if (chartItem != null)
				chartItem.Argument = obj;
		}
	}
	public class ChartItemValueMapping : MapItemMappingBase {
		public ChartItemValueMapping(string member) : base(member) {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			IMapChartDataItem chartItem = item as IMapChartDataItem;
			if (chartItem != null)
				chartItem.Value = double.NaN;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			IMapChartDataItem chartItem = item as IMapChartDataItem;
			if (chartItem != null)
				chartItem.Value = ConvertToDouble(obj);
		}
	}
	public class LatitudeMapping : MapItemMappingBase {
		public LatitudeMapping(string member)
			: base(member) {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation geoItem = item as ISupportCoordLocation;
			if (geoItem != null)
				geoItem.Location = new GeoPoint(0.0, geoItem.Location.GetX());
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation geoItem = item as ISupportCoordLocation;
			if (geoItem != null)
				geoItem.Location = new GeoPoint(ConvertToDouble(obj), geoItem.Location.GetX());
		}
	}
	public class LongitudeMapping : MapItemMappingBase {
		public LongitudeMapping(string member)
			: base(member) {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation geoItem = item as ISupportCoordLocation;
			if (geoItem != null) {
				geoItem.Location = new GeoPoint(geoItem.Location.GetY(), 0.0);
			}
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation geoItem = item as ISupportCoordLocation;
			if (geoItem != null) {
				geoItem.Location = new GeoPoint(geoItem.Location.GetY(), ConvertToDouble(obj));
			}
		}
	}
	public class YCoordinateMapping : MapItemMappingBase {
		public YCoordinateMapping(string member, SourceCoordinateSystem coordinateSystem)
			: base(member) {
			CoordinateSystem = coordinateSystem;
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation coordItem = item as ISupportCoordLocation;
			if (coordItem != null)
				coordItem.Location = CoordinateSystem.CreatePoint(coordItem.Location.GetX(), 0.0);
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation coordItem = item as ISupportCoordLocation;
			if (coordItem != null)
				coordItem.Location = CoordinateSystem.CreatePoint(coordItem.Location.GetX(), ConvertToDouble(obj));
		}
	}
	public class XCoordinateMapping : MapItemMappingBase {
		public XCoordinateMapping(string member, SourceCoordinateSystem coordinateSystem)
			: base(member) {
			CoordinateSystem = coordinateSystem;
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation coordItem = item as ISupportCoordLocation;
			if (coordItem != null)
				coordItem.Location = CoordinateSystem.CreatePoint(0.0, coordItem.Location.GetY());
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			ISupportCoordLocation coordItem = item as ISupportCoordLocation;
			if (coordItem != null)
				coordItem.Location = CoordinateSystem.CreatePoint(ConvertToDouble(obj), coordItem.Location.GetY());
		}
	}
	public class PieSegmentIdMapping : MapItemMappingBase {
		public PieSegmentIdMapping(string member) : base(member) {
		}
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			PieSegment segment = item as PieSegment;
			if (segment != null) segment.SegmentId = null;
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			PieSegment segment = item as PieSegment;
			if (segment != null) segment.SegmentId = obj;
		}
	}
}
