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
	DashboardItemType(DashboardItemType.GeoPointMap)
	]
	public class GeoPointMapDashboardItem : GeoPointMapDashboardItemBase {
		const string xmlValue = "Value";
		readonly DataItemBox<Measure> valueBox;
		readonly GeoPointHolder valueHolder;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GeoPointMapDashboardItemValue"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DefaultValue(null)
		]
		public Measure Value {
			get { return valueBox.DataItem; }
			set { valueBox.DataItem = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new MapLegend Legend { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new WeightedLegend WeightedLegend { get { return null; } }
		protected internal override bool IsMapReady { get { return base.IsMapReady && Value != null; } }
		protected override IEnumerable<DataDashboardItemDescription> MapDescriptions {
			get {
				foreach(DataDashboardItemDescription description in base.MapDescriptions)
					yield return description;
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.SingleNumericMeasure, valueHolder);
			}
		}
		public GeoPointMapDashboardItem()
			: base() {
			valueHolder = new GeoPointHolder(this);
			valueBox = InitializeMeasureBox(valueHolder, xmlValue);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new GeoPointMapDashboardItemViewModel(this);
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			if(measure == Value)
				return SummaryTypeInfo.Text;
			return base.GetSummaryTypeInfo(measure);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			valueBox.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			valueBox.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			valueBox.OnEndLoading();
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.AddMeasure(Value);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			foreach(Measure measure in description.Measures)
				if(Value == null)
					Value = measure;
				else
					AssignMeasure(measure, HiddenMeasures);
		}
		protected internal override void FillClientDataDataMembers(InternalMapDataMembersContainer dataMembers) {
			base.FillClientDataDataMembers(dataMembers);
			dataMembers.AddMeasure(Value.ActualId);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			if(Value != null) 
				builder.AddMeasure(Value);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			if (Value != null)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), new[] { Value }));
		}
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return base.GetQueryVisibleMeasures().Append(Value).NotNull();
		}
	}
}
