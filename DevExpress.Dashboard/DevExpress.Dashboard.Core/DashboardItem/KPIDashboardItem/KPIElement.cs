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

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public abstract class KpiElement : NamedDataItemContainer {
		const string actualValueName = "ActualValue";
		const string targetValueName = "TargetValue";
		readonly DeltaOptions deltaOptions;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("KpiElementDeltaOptions"),
#endif
		Category(CategoryNames.Data),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DeltaOptions DeltaOptions { get { return deltaOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("KpiElementActualValue"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure ActualValue {
			get { return GetMeasure(actualValueName); }
			set { SetDataItem(actualValueName, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("KpiElementTargetValue"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure TargetValue {
			get { return GetMeasure(targetValueName); }
			set { SetDataItem(targetValueName, value); }
		}
		internal string UniqueName {
			get {
				Measure value = ActualValue ?? TargetValue;
				return value != null ? value.ActualId : String.Empty;
			}
		}
		protected internal override string DefaultName { get { return KpiElementCaptionProvider.GetCaption(ActualValue, TargetValue); } }
		protected KpiElement(Measure actualValue, Measure targetValue)
			: base(new DataItemDescription[] { 
				new DataItemDescription(actualValueName, DashboardLocalizer.GetString(DashboardStringId.ActualValueCaption), actualValue), 
				new DataItemDescription(targetValueName, DashboardLocalizer.GetString(DashboardStringId.TargetValueCaption), targetValue)}) {
			deltaOptions = new DeltaOptions(this);
		}
		protected internal override void Assign(DataItemContainer dataItemContainer) {
			base.Assign(dataItemContainer);
			KpiElement kpiElement = dataItemContainer as KpiElement;
			if(kpiElement != null) {
				ActualValue = kpiElement.ActualValue;
				TargetValue = kpiElement.TargetValue;
				deltaOptions.Assign(kpiElement.deltaOptions);
			}
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			deltaOptions.SaveToXml(element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			deltaOptions.LoadFromXml(element);
		}
		protected internal override DataItemContainerActualContent GetActualContent() {
			DataItemContainerActualContent content = new DataItemContainerActualContent();
			if(ActualValue != null)
				content.Measures.Add(ActualValue);
			if(TargetValue != null)
				content.Measures.Add(TargetValue);
			content.IsDelta = true;
			content.DeltaOptions = deltaOptions;
			return content;
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			return SummaryTypeInfo.Number;
		}
	}
}
