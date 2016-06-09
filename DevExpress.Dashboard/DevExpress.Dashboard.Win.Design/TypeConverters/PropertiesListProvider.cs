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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NamesList = System.Collections.Generic.List<string>;
namespace DevExpress.DashboardWin.Design {
	class DimensionPropertiesProvider : PropertiesListProvider<Dimension> {
		const string NumericFormatProp = "NumericFormat";
		const string IsDiscreteNumericScaleProp = "IsDiscreteNumericScale";
		const string TextGroupIntervalProp = "TextGroupInterval";
		const string DateTimeFormatProp = "DateTimeFormat";
		const string DateTimeGroupIntervalProp = "DateTimeGroupInterval";
		protected override void FillExcluded(NamesList propNames) {
			base.FillExcluded(propNames);
			DataSourceInfo dataSourceInfo =  Context.GetService<SelectedContextService>().DataSourceInfo;
			IDashboardDataSource ds = dataSourceInfo != null ? dataSourceInfo.DataSource : null;
			if(ds != null) {
				if(ds.GetIsOlap())
					propNames.AddRange(new string[] { DateTimeGroupIntervalProp, TextGroupIntervalProp, IsDiscreteNumericScaleProp });
				switch(dataSourceInfo.GetFieldType(Value.DataMember)) {
					case DataFieldType.DateTime:
						propNames.AddRange(new string[] { NumericFormatProp, IsDiscreteNumericScaleProp, TextGroupIntervalProp });
						break;
					case DataFieldType.Text:
						propNames.AddRange(new string[] { NumericFormatProp, IsDiscreteNumericScaleProp, DateTimeFormatProp, DateTimeGroupIntervalProp });
						break;
					case DataFieldType.Decimal:
					case DataFieldType.Float:
					case DataFieldType.Integer:
						propNames.AddRange(new string[] { DateTimeFormatProp, DateTimeGroupIntervalProp, TextGroupIntervalProp });
						break;
				}
			}
		}
		public DimensionPropertiesProvider(ITypeDescriptorContext context, Dimension value, Attribute[] attributes, IEnumerable<string> defaultExcludedPropNames = null) :
			base(context, value, attributes, defaultExcludedPropNames) { }
	}
	class MeasurePropertiesProvider : PropertiesListProvider<Measure> {
		protected override void FillExcluded(NamesList propNames) {
			base.FillExcluded(propNames);
			DataSourceInfo ds = Context.GetService<SelectedContextService>().DataSourceInfo;
			if(ds != null && ds.DataSource!=null && ds.DataSource.GetIsOlap())
				propNames.AddRange(new string[] { "SummaryType" });
		}
		public MeasurePropertiesProvider(ITypeDescriptorContext context, Measure value, Attribute[] attributes, IEnumerable<string> defaultExcludedPropNames = null) :
			base(context, value, attributes, defaultExcludedPropNames) { }
	}
	class SimpleSeriesPropertiesProvider : PropertiesListProvider<SimpleSeries> {
		protected override void FillExcluded(NamesList propNames) {
			base.FillExcluded(propNames);
			if(SimpleSeriesConverter.IsRangeFilterDashboardItem(Context))
				propNames.AddRange(new string[] { "PlotOnSecondaryAxis" });
		}
		public SimpleSeriesPropertiesProvider(ITypeDescriptorContext context, SimpleSeries value, Attribute[] attributes, IEnumerable<string> defaultExcludedPropNames = null) :
			base(context, value, attributes, defaultExcludedPropNames) { }
	}
}
