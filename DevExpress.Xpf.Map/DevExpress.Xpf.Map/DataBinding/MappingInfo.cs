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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using DevExpress.Data;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map {
	public abstract class MapItemMappingInfoBase : MapDependencyObject {
		public static readonly DependencyProperty LatitudeProperty = DependencyPropertyManager.Register("Latitude",
			typeof(string), typeof(MapItemMappingInfoBase), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty LongitudeProperty = DependencyPropertyManager.Register("Longitude",
			typeof(string), typeof(MapItemMappingInfoBase), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty XCoordinateProperty = DependencyPropertyManager.Register("XCoordinate",
			typeof(string), typeof(MapItemMappingInfoBase), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty YCoordinateProperty = DependencyPropertyManager.Register("YCoordinate",
			typeof(string), typeof(MapItemMappingInfoBase), new PropertyMetadata(null, NotifyPropertyChanged));
		[Category(Categories.Data)]
		public string Latitude {
			get { return (string)GetValue(LatitudeProperty); }
			set { SetValue(LatitudeProperty, value); }
		}
		[Category(Categories.Data)]
		public string Longitude {
			get { return (string)GetValue(LongitudeProperty); }
			set { SetValue(LongitudeProperty, value); }
		}
		[Category(Categories.Data)]
		public string XCoordinate {
			get { return (string)GetValue(XCoordinateProperty); }
			set { SetValue(XCoordinateProperty, value); }
		}
		[Category(Categories.Data)]
		public string YCoordinate {
			get { return (string)GetValue(YCoordinateProperty); }
			set { SetValue(YCoordinateProperty, value); }
		}
		protected internal virtual void FillActualMappings(MappingDictionary mappings, SourceCoordinateSystem coordinateSystem) {
			XCoordinateMapping xMapping = GetXCoordinateMapping(coordinateSystem);
			if (xMapping != null)
				mappings.Add(MappingType.CoordX, xMapping);
			YCoordinateMapping yMapping = GetYCoordinateMapping(coordinateSystem);
			if (yMapping != null)
				mappings.Add(MappingType.CoordY, yMapping);
		}
		XCoordinateMapping GetXCoordinateMapping(SourceCoordinateSystem coordinateSystem) {
			if (coordinateSystem.GetSourcePointType() == CoordPointType.Geo) {
				if (!string.IsNullOrEmpty(Longitude))
					return new XCoordinateMapping(Longitude, coordinateSystem);
				if (!string.IsNullOrEmpty(XCoordinate))
					return new XCoordinateMapping(XCoordinate, coordinateSystem);
			}
			else {
				if (!string.IsNullOrEmpty(XCoordinate))
					return new XCoordinateMapping(XCoordinate, coordinateSystem);
				if (!string.IsNullOrEmpty(Longitude))
					return new XCoordinateMapping(Longitude, coordinateSystem);
			}
			return null;
		}
		YCoordinateMapping GetYCoordinateMapping(SourceCoordinateSystem coordinateSystem) {
			if (coordinateSystem.GetSourcePointType() == CoordPointType.Geo) {
				if (!string.IsNullOrEmpty(Latitude))
					return new YCoordinateMapping(Latitude, coordinateSystem);
				if (!string.IsNullOrEmpty(YCoordinate))
					return new YCoordinateMapping(YCoordinate, coordinateSystem);
			}
			else {
				if (!string.IsNullOrEmpty(YCoordinate))
					return new YCoordinateMapping(YCoordinate, coordinateSystem);
				if (!string.IsNullOrEmpty(Latitude))
					return new YCoordinateMapping(Latitude, coordinateSystem);
			}
			return null;
		}
	}
	public class MapItemMappingInfo : MapItemMappingInfoBase {
		protected override MapDependencyObject CreateObject() {
			return new MapItemMappingInfo();
		}
	}
	public class MapBubbleMappingInfo : MapItemMappingInfoBase {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
			typeof(string), typeof(MapBubbleMappingInfo), new PropertyMetadata(null, NotifyPropertyChanged));
		[Category(Categories.Data)]
		public string Value {
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new MapBubbleMappingInfo();
		}
		protected internal override void FillActualMappings(MappingDictionary mappings, SourceCoordinateSystem coordinateSystem) {
			base.FillActualMappings(mappings, coordinateSystem);
			if (!string.IsNullOrEmpty(Value))
				mappings.Add(MappingType.Value, new ChartItemValueMapping(Value));
		}
	}
	public class MapPieMappingInfo : MapItemMappingInfoBase {
		public static readonly DependencyProperty SegmentIdProperty = DependencyProperty.Register("SegmentId",
			typeof(string), typeof(MapPieMappingInfo), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty SegmentValueProperty = DependencyProperty.Register("SegmentValue",
			typeof(string), typeof(MapPieMappingInfo), new PropertyMetadata(null, NotifyPropertyChanged));
		[Category(Categories.Data)]
		public string SegmentId {
			get { return (string)GetValue(SegmentIdProperty); }
			set { SetValue(SegmentIdProperty, value); }
		}
		[Category(Categories.Data)]
		public string SegmentValue {
			get { return (string)GetValue(SegmentValueProperty); }
			set { SetValue(SegmentValueProperty, value); }
		}
		protected internal override void FillActualMappings(MappingDictionary mappings, SourceCoordinateSystem coordinateSystem) {
			base.FillActualMappings(mappings, coordinateSystem);
			if (!string.IsNullOrEmpty(SegmentId))
				mappings.Add(MappingType.SegmentId, new PieSegmentIdMapping(SegmentId));
			if (!string.IsNullOrEmpty(SegmentValue))
				mappings.Add(MappingType.Value, new ChartItemValueMapping(SegmentValue));
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPieMappingInfo();
		}
	}
	public class MapItemAttributeMapping : MapDependencyObject {
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name",
		   typeof(string), typeof(MapItemAttributeMapping), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty MemberProperty = DependencyProperty.Register("Member",
		   typeof(string), typeof(MapItemAttributeMapping), new PropertyMetadata(null, NotifyPropertyChanged));
		[Category(Categories.Data)]
		public string Name {
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		[Category(Categories.Data)]
		public string Member {
			get { return (string)GetValue(MemberProperty); }
			set { SetValue(MemberProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new MapItemAttributeMapping();
		}
	}
	public class MapItemAttributeMappingCollection : ObservableCollection<MapItemAttributeMapping> {
	}
}
