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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class DeltaMap : ChoroplethMap {
		const string ActualValueKey = "ActualValue";
		const string TargetValueKey = "TargetValue";
		readonly DeltaOptions deltaOptions;
		readonly NameBox actualValueNameBox = new NameBox("ActualValueName");
		readonly NameBox deltaNameBox = new NameBox("DeltaName");
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaMapActualValue"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure ActualValue {
			get { return (Measure)GetDataItem(ActualValueKey); }
			set { SetDataItem(ActualValueKey, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaMapTargetValue"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure TargetValue {
			get { return (Measure)GetDataItem(TargetValueKey); }
			set { SetDataItem(TargetValueKey, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaMapDeltaOptions"),
#endif
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DeltaOptions DeltaOptions { get { return deltaOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaMapActualValueName"),
#endif
		DefaultValue(null),
		Localizable(true)
		]
		public string ActualValueName { get { return actualValueNameBox.Name; } set { actualValueNameBox.Name = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaMapDeltaName"),
#endif
		DefaultValue(null),
		Localizable(true)
		]
		public string DeltaName { get { return deltaNameBox.Name; } set { deltaNameBox.Name = value; } }
		protected internal override string DefaultName {
			get { return !string.IsNullOrEmpty(DefaultDeltaName) ? DefaultDeltaName : (!string.IsNullOrEmpty(DefaultActualValueName) ? DefaultActualValueName : DefaultTargetValueName); }
		}
		protected internal override string LayerId { get { return "GridDeltaColumn"; } }
		string DefaultDeltaName { 
			get {
				if(ActualValue != null && TargetValue != null)
					return string.Format(DashboardLocalizer.GetString(DashboardStringId.FormatStringKpiElementCaption), DefaultActualValueName, DefaultTargetValueName);
				return string.Empty;
			}
		}
		string DefaultActualValueName {
			get {
				Measure actualValue = ActualValue;
				return actualValue != null ? actualValue.DisplayName : string.Empty;
			}
		}
		string DefaultTargetValueName {
			get {
				Measure targetValue = TargetValue;
				return targetValue != null ? targetValue.DisplayName : string.Empty;
			}
		}
		internal string ActualValueDisplayName { get { return actualValueNameBox.DisplayName; } }
		internal string DeltaDisplayName { get { return deltaNameBox.DisplayName; } }
		public DeltaMap()
			: this(null, null) {
		}
		public DeltaMap(Measure actualValue, Measure targetValue)
			: base(new DataItemDescription[] { 
				new DataItemDescription(ActualValueKey, DashboardLocalizer.GetString(DashboardStringId.ActualValueCaption), actualValue),
				new DataItemDescription(TargetValueKey, DashboardLocalizer.GetString(DashboardStringId.TargetValueCaption), targetValue) }) {
			deltaOptions = new DeltaOptions(this);
			actualValueNameBox.NameChanged += (sender, e) => OnChanged(ChangeReason.View);
			actualValueNameBox.RequestDefaultName += (sender, e) => e.DefaultName = DefaultActualValueName;
			deltaNameBox.NameChanged += (sender, e) => OnChanged(ChangeReason.View);
			deltaNameBox.RequestDefaultName += (sender, e) => e.DefaultName = DefaultDeltaName;
		}
		protected override DataItemContainer CreateInstance() {
			return new DeltaMap();
		}
		protected internal override void Assign(DataItemContainer dataItemContainer) {
			base.Assign(dataItemContainer);
			DeltaMap layer = dataItemContainer as DeltaMap;
			if(layer != null)
				deltaOptions.Assign(layer.deltaOptions);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			deltaOptions.SaveToXml(element);
			actualValueNameBox.SaveToXml(element);
			deltaNameBox.SaveToXml(element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			deltaOptions.LoadFromXml(element);
			actualValueNameBox.LoadFromXml(element);
			deltaNameBox.LoadFromXml(element);
		}
		protected internal override EditNameDescription GetEditNameDescription() {
			if (ActualValue != null && TargetValue != null)
				return new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionMapValues), 
					new IEditNameProvider[] { this, actualValueNameBox, deltaNameBox });
			return null;
		}
		protected internal override DataItemContainerActualContent GetActualContent() {
			DataItemContainerActualContent content = new DataItemContainerActualContent();
			if(ActualValue != null) {
				content.Measures.Add(ActualValue);
				content.DeltaActualValue = ActualValue;
			}
			if(TargetValue != null) {
				content.Measures.Add(TargetValue);
				content.DeltaTargetValue = TargetValue;
			}
			content.IsDelta = true;
			content.DeltaOptions = deltaOptions;
			return content;
		}
	}
}
