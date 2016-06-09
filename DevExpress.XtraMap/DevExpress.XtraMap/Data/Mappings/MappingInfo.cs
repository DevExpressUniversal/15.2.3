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
using DevExpress.XtraMap.Native;
using System.Diagnostics.CodeAnalysis;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap {
	public abstract class MapItemMappingInfoBase : ILayerDataManagerProvider {
		LayerDataManager dataManager;
		protected MapItemMappingInfoBase(LayerDataManager dataManager) {
			this.dataManager = dataManager;
		}
		protected void OnChanged() {
			if(dataManager != null) dataManager.OnMappingsChanged();
		}
		#region ILayerDataManagerProvider Members
		LayerDataManager ILayerDataManagerProvider.DataManager { get { return dataManager; } }
		#endregion
		protected internal abstract void FillActualMappings(MappingCollection mappings, SourceCoordinateSystem coordinateSystem);
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MapItemMappingInfo : MapItemMappingInfoBase {
		string latitude = string.Empty;
		string longitude = string.Empty;
		string xCoordinate = string.Empty;
		string yCoordinate = string.Empty;
		string type = string.Empty;
		string text = string.Empty;
		string imageIndex = string.Empty;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemMappingInfoLatitude"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public string Latitude {
			get { return latitude; }
			set {
				if(latitude == value)
					return;
				latitude = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemMappingInfoLongitude"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public string Longitude {
			get { return longitude; }
			set {
				if(longitude == value)
					return;
				longitude = value;
				OnChanged();
			}
		}
		[Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public string XCoordinate {
			get { return xCoordinate; }
			set {
				if (xCoordinate == value)
					return;
				xCoordinate = value;
				OnChanged();
			}
		}
		[Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public string YCoordinate {
			get { return yCoordinate; }
			set {
				if (yCoordinate == value)
					return;
				yCoordinate = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemMappingInfoType"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public string Type {
			get { return type; }
			set {
				if(type == value)
					return;
				type = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemMappingInfoText"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public string Text {
			get { return text; }
			set {
				if(text == value)
					return;
				text = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemMappingInfoImageIndex"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public string ImageIndex {
			get { return imageIndex; }
			set {
				if(imageIndex == value)
					return;
				imageIndex = value;
				OnChanged();
			}
		}
		internal MapItemMappingInfo(LayerDataManager dataManager)
			: base(dataManager) {
		}
		XCoordinateMapping GetXCoordinateMapping(SourceCoordinateSystem coordinateSystem) {
			if (coordinateSystem.GetSourcePointType() == CoordPointType.Geo) {
				if (!string.IsNullOrEmpty(Longitude))
					return new XCoordinateMapping() { Member = Longitude, CoordinateSystem = coordinateSystem };
				if (!string.IsNullOrEmpty(XCoordinate))
					return new XCoordinateMapping() { Member = XCoordinate, CoordinateSystem = coordinateSystem };
			} else {
				if (!string.IsNullOrEmpty(XCoordinate))
					return new XCoordinateMapping() { Member = XCoordinate, CoordinateSystem = coordinateSystem };
				if (!string.IsNullOrEmpty(Longitude))
					return new XCoordinateMapping() { Member = Longitude, CoordinateSystem = coordinateSystem };
			}
			return null;
		}
		YCoordinateMapping GetYCoordinateMapping(SourceCoordinateSystem coordinateSystem) {
			if (coordinateSystem.GetSourcePointType() == CoordPointType.Geo) {
				if (!string.IsNullOrEmpty(Latitude))
					return new YCoordinateMapping() { Member = Latitude, CoordinateSystem = coordinateSystem };
				if (!string.IsNullOrEmpty(YCoordinate))
					return new YCoordinateMapping() { Member = YCoordinate, CoordinateSystem = coordinateSystem };
			} else {
				if (!string.IsNullOrEmpty(YCoordinate))
					return new YCoordinateMapping() { Member = YCoordinate, CoordinateSystem = coordinateSystem };
				if (!string.IsNullOrEmpty(Latitude))
					return new YCoordinateMapping() { Member = Latitude, CoordinateSystem = coordinateSystem };
			}
			return null;
		}
		protected internal override void FillActualMappings(MappingCollection mappings, SourceCoordinateSystem coordinateSystem) {
			if(!string.IsNullOrEmpty(Type))
				mappings.Add(new TypeMapping() { Member = Type });
			XCoordinateMapping xMapping = GetXCoordinateMapping(coordinateSystem);
			if (xMapping != null)
				mappings.Add(xMapping);
			YCoordinateMapping yMapping = GetYCoordinateMapping(coordinateSystem);
			if (yMapping != null)
				mappings.Add(yMapping);
			if(!string.IsNullOrEmpty(Text))
				mappings.Add(new TextMapping() { Member = Text });
			if(!string.IsNullOrEmpty(ImageIndex))
				mappings.Add(new ImageIndexMapping { Member = ImageIndex });
		}
		public override string ToString() {
			return "(MapItemMappingInfo)";
		}
	}
	public abstract class MapChartItemMappingInfo : MapItemMappingInfo {
		string groupKey = string.Empty;
		string valueMapping = string.Empty;
		protected string GroupKey {
			get { return groupKey; }
			set {
				if(string.Equals(groupKey, value))
					return;
				groupKey = value;
				OnChanged();
			}
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new string Type { get { return string.Empty; } set { ; } }
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)]
		new public string ImageIndex { get; set; }
		[Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapChartItemMappingInfoValue")
#else
	Description("")
#endif
]
		public string Value {
			get { return valueMapping; }
			set {
				if(string.Equals(valueMapping, value))
					return;
				valueMapping = value;
				OnChanged();
			}
		}
		internal MapChartItemMappingInfo(LayerDataManager dataManager)
			: base(dataManager) {
		}
		protected internal override void FillActualMappings(MappingCollection mappings, SourceCoordinateSystem coordinateSystem) {
			base.FillActualMappings(mappings, coordinateSystem);
			if(!string.IsNullOrEmpty(Value))
				mappings.Add(new ChartItemValueMapping() { Member = Value });
		}
	}
	public class MapBubbleMappingInfo : MapChartItemMappingInfo {
		[Category(SRCategoryNames.Data), DefaultValue(""),
		NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapBubbleMappingInfoBubbleGroup")
#else
	Description("")
#endif
]
		public string BubbleGroup { get { return base.GroupKey; } set { base.GroupKey = value; } }
		internal MapBubbleMappingInfo(LayerDataManager dataManager)
			: base(dataManager) {
		}
		protected internal override void FillActualMappings(MappingCollection mappings, SourceCoordinateSystem coordinateSystem) {
			base.FillActualMappings(mappings, coordinateSystem);
			if(!string.IsNullOrEmpty(BubbleGroup))
				mappings.Add(new ChartItemGroupKeyMapping() { Member = BubbleGroup });
		}
		public override string ToString() {
			return "(MapBubbleMappingInfo)";
		}
	}
	public class MapPieMappingInfo : MapChartItemMappingInfo {
		[Category(SRCategoryNames.Data), DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPieMappingInfoPieSegment")
#else
	Description("")
#endif
]
		public string PieSegment { get { return base.GroupKey; } set { base.GroupKey = value; } }
		internal MapPieMappingInfo(LayerDataManager dataManager)
			: base(dataManager) {
		}
		protected internal override void FillActualMappings(MappingCollection mappings, SourceCoordinateSystem coordinateSystem) {
			base.FillActualMappings(mappings, coordinateSystem);
			if(!string.IsNullOrEmpty(GroupKey))
				mappings.Add(new PieSegmentArgumentMapping() { Member = PieSegment });
		}
		public override string ToString() {
			return "(MapPieMappingInfo)";
		}
	}
}
