#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.BubbleMap)
	]
	public class BubbleMapDashboardItem : GeoPointMapDashboardItemBase {
		const string xmlWeight = "Weight";
		const string xmlColor = "Color";
		readonly DataItemBox<Measure> weightBox;
		readonly WeightHolder weightHolder;
		readonly DataItemBox<Measure> colorBox;
		readonly ColorHolder colorHolder;
		readonly MapColorizerInternal colorizerInternal;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("BubbleMapDashboardItemWeight"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DefaultValue(null)
		]
		public Measure Weight {
			get { return weightBox.DataItem; }
			set { weightBox.DataItem = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("BubbleMapDashboardItemColor"),
#endif
 Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DefaultValue(null)
		]
		public Measure Color {
			get { return colorBox.DataItem; }
			set { colorBox.DataItem = value; }
		}
		[
		Editor(TypeNames.TypeListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMapPaletteConverter),
		DefaultValue(null)
		]
		public MapPalette ColorPalette {
			get { return colorizerInternal.Palette; }
			set { colorizerInternal.Palette = value; }
		}
		[
		Editor(TypeNames.TypeListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMapScaleConverter),
		DefaultValue(null)
		]
		public MapScale ColorScale {
			get { return colorizerInternal.Scale; }
			set { colorizerInternal.Scale = value; }
		}
		protected internal override bool IsMapReady { get { return base.IsMapReady && (Weight != null || Color != null); } }
		protected override IEnumerable<DataDashboardItemDescription> MapDescriptions {
			get {
				foreach(DataDashboardItemDescription description in base.MapDescriptions)
					yield return description;
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemWeight),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.SingleNumericMeasure, weightHolder);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemColor),
				   DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.GeoPointColor, colorHolder);
			}
		}
		public BubbleMapDashboardItem()
			: base() {
			weightHolder = new WeightHolder(this);
			weightBox = InitializeMeasureBox(weightHolder, xmlWeight);
			colorHolder = new ColorHolder(this);
			colorBox = InitializeMeasureBox(colorHolder, xmlColor);
			colorizerInternal = new MapColorizerInternal(this);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new BubbleMapDashboardItemViewModel(this);
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			if(measure == Weight || measure == Color)
				return SummaryTypeInfo.Number;
			return base.GetSummaryTypeInfo(measure);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			weightBox.SaveToXml(element);
			colorBox.SaveToXml(element);
			colorizerInternal.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			weightBox.LoadFromXml(element);
			colorBox.LoadFromXml(element);
			colorizerInternal.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			weightBox.OnEndLoading();
			colorBox.OnEndLoading();
			colorizerInternal.OnEndLoading();
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.AddMeasure(Weight);
			description.AddMeasure(Color);
			description.Legend = Legend;
			description.WeightedLegend = WeightedLegend;
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			Legend.CopyFrom(description.Legend);
			WeightedLegend.CopyFrom(description.WeightedLegend);
			foreach(Measure measure in description.Measures)
				if(Weight == null)
					Weight = measure;
				else if(Color == null)
					Color = measure;
				else
					AssignMeasure(measure, HiddenMeasures);
		}
		protected internal override void FillClientDataDataMembers(InternalMapDataMembersContainer dataMembers) {
			base.FillClientDataDataMembers(dataMembers);
			Measure weight = Weight;
			if(weight != null) 
				dataMembers.AddMeasure(weight.ActualId);
			Measure color = Color;
			if(color != null) 
				dataMembers.AddMeasure(color.ActualId);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			if(Weight != null)
				builder.AddMeasure(Weight);
			if(Color != null)
				builder.AddMeasure(Color);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			if(Weight != null)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemWeight), new[] { Weight }));
			if(Color != null)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemColor), new[] { Color }));
		}
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			foreach(Measure measure in base.GetQueryVisibleMeasures())
				yield return measure;
			if(Weight != null)
				yield return Weight;
			if(Color != null)
				yield return Color;
		}
	}
}
