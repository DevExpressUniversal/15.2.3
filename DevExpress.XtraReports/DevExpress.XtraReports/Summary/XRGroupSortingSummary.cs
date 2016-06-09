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
using System.Text;
using DevExpress.XtraReports.Serialization;
using System.ComponentModel;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native.Summary;
using System.Collections;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UI {
	[
	TypeConverter(typeof(DevExpress.XtraReports.Design.XRGroupSortingSummaryConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGroupSortingSummary")
	]
	public class XRGroupSortingSummary {
		const bool defaultEnabled = false;
		const SortingSummaryFunction defaultFunction = SortingSummaryFunction.Sum;
		const XRColumnSortOrder defaultSortOrder = XRColumnSortOrder.Ascending;
		const bool defaultIgnoreNullValues = false;
		GroupHeaderBand band;
		bool enabled = defaultEnabled;
		SortingSummaryFunction function = defaultFunction;
		string fieldName = string.Empty;
		XRColumnSortOrder sortOrder = defaultSortOrder;
		bool ignoreNullValues = defaultIgnoreNullValues;
		ValuesRowsContainer valuesInfo = new ValuesRowsContainer();
		public XRGroupSortingSummary() {
		}
		#region properties
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGroupSortingSummary.Enabled"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,]
		public bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGroupSortingSummary.Function"),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public SortingSummaryFunction Function {
			get { return function; }
			set { function = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGroupSortingSummary.FieldName"),
		Editor("DevExpress.XtraReports.Design.FieldNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(DevExpress.XtraReports.Design.FieldNameConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public string FieldName {
			get { return fieldName == null ? string.Empty : fieldName; }
			set { fieldName = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGroupSortingSummary.SortOrder"),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public XRColumnSortOrder SortOrder {
			get { return sortOrder; }
			set { sortOrder = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGroupSortingSummary.IgnoreNullValues"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(defaultIgnoreNullValues),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public bool IgnoreNullValues {
			get { return ignoreNullValues; }
			set { ignoreNullValues = value; }
		}
		internal GroupHeaderBand Band {
			get { return band; }
			set { band = value; }
		}
		internal ValuesRowsContainer ValuesInfo { get { return valuesInfo; } }
		bool AreEventsEnabled { get { return Function == SortingSummaryFunction.Custom; } }
		#endregion
		internal bool ShouldSerialize() {
			return
				ShouldSerializeEnabled() ||
				ShouldSerializeFunction() ||
				ShouldSerializeFieldName() ||
				ShouldSerializeSortOrder() ||
				ShouldSerializeIgnoreNullValues();
		}
		bool ShouldSerializeEnabled() {
			return enabled != defaultEnabled;
		}
		bool ShouldSerializeFunction() {
			return function != defaultFunction;
		}
		bool ShouldSerializeFieldName() {
			return !string.IsNullOrEmpty(fieldName);
		}
		bool ShouldSerializeSortOrder() {
			return sortOrder != defaultSortOrder;
		}
		bool ShouldSerializeIgnoreNullValues() {
			return ignoreNullValues != defaultIgnoreNullValues;
		}
		object GetNativeValue(IList values) {
			bool handled = false;
			object result = FireSummaryGetResult(ref handled);
			return handled ? result : SummaryHelper.CalcResult(Function, ValuesInfo.Values);
		}
		object FireSummaryGetResult(ref bool handled) {
			GroupSortingSummaryGetResultEventArgs e = new GroupSortingSummaryGetResultEventArgs(ValuesInfo.Values.ToArray());
			if (AreEventsEnabled)
				band.OnSortingSummaryGetResult(e);
			handled = e.Handled;
			return e.Result;
		}
		void Reset() {
			ValuesInfo.Clear();
			if (AreEventsEnabled)
				band.OnSortingSummaryReset(EventArgs.Empty);
		}
		internal void OnDataRowChanged(object row, object fieldValue, int rowHandle) {
			if (!IgnoreNullValues || (fieldValue != null && !(fieldValue is DBNull)))
				AddValue(fieldValue, rowHandle);
			if (AreEventsEnabled)
				band.OnSortingSummaryRowChanged(new GroupSortingSummaryRowChangedEventArgs(row, fieldValue));
		}
		internal object OnGroupFinished() {
			object value = GetNativeValue(ValuesInfo.Values);
			Reset();
			return value;
		}
		internal void AddValue(object value, int rowIndex) {
			ValuesInfo.Add(value, rowIndex);
		}
	}
}
