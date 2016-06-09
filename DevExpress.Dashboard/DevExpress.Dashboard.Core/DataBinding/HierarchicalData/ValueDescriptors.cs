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

using System.Collections.Generic;
using DevExpress.DashboardCommon.DataBinding.HierarchicalData;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon {
	public class DimensionValue {
		ServerDimensionValue serverValue;
		string displayText;
		internal DimensionValue(ServerDimensionValue serverValue, string displayText) {
			this.serverValue = serverValue;
			this.displayText = displayText;
		}
		public object Value { get { return serverValue.Value; } }
		public object UniqueValue { get { return serverValue.UniqueValue; } }
		public string DisplayText { get { return displayText; } }
		public override bool Equals(object obj) {
			DimensionValue dimensionValue = obj as DimensionValue;
			if(dimensionValue != null)
				return this.serverValue.Equals(dimensionValue.serverValue);
			return false;
		}
		public override int GetHashCode() {
			return serverValue.GetHashCode();
		}
	}
	public class MeasureValue {
		readonly object value;
		ValueFormatViewModel format;
		string displayText;
		internal MeasureValue(object value, ValueFormatViewModel format) {
			this.value = value;
			this.format = format;
		}
		public object Value { get { return value; } }
		public string DisplayText { 
			get {
				if(displayText == null) 
					displayText = FormatterBase.CreateFormatter(format).Format(value);
				return displayText; 
			} 
		}
	}
	public class DeltaValue {
		MeasureValue absoluteVariation;
		MeasureValue percentVariation;
		MeasureValue percentOfTarget;
		MeasureValue actualValue;
		MeasureValue targetValue;
		MeasureValue displayValue;
		MeasureValue displaySubValue1;
		MeasureValue displaySubValue2;
		bool isGood;
		IndicatorType indicatorType;
		internal DeltaValue(MeasureValue absoluteVariation, MeasureValue percentVariation, MeasureValue percentOfTarget,
			MeasureValue actualValue, MeasureValue targetValue, bool isGood, IndicatorType indicatorType, DeltaValueType deltaValueType) {
			this.absoluteVariation = absoluteVariation;
			this.percentVariation = percentVariation;
			this.percentOfTarget = percentOfTarget;
			this.actualValue = actualValue;
			this.targetValue = targetValue;
			this.isGood = isGood;
			this.indicatorType = indicatorType;
			switch(deltaValueType) {
				case DeltaValueType.ActualValue:
					this.displayValue = actualValue;
					this.displaySubValue1 = absoluteVariation;
					this.displaySubValue2 = percentVariation;
					break;
				case DeltaValueType.AbsoluteVariation:
					this.displayValue = absoluteVariation;
					this.displaySubValue1 = actualValue;
					this.displaySubValue2 = percentVariation;
					break;
				case DeltaValueType.PercentVariation:
					this.displayValue = percentVariation;
					this.displaySubValue1 = actualValue;
					this.displaySubValue2 = absoluteVariation;
					break;
				case DeltaValueType.PercentOfTarget:
					this.displayValue = percentOfTarget;
					this.displaySubValue1 = actualValue;
					this.displaySubValue2 = absoluteVariation;
					break;
			}
		}
		public MeasureValue AbsoluteVariation { get { return absoluteVariation; } }
		public MeasureValue PercentVariation { get { return percentVariation; } }
		public MeasureValue PercentOfTarget { get { return percentOfTarget; } }
		public MeasureValue ActualValue { get { return actualValue; } }
		public MeasureValue TargetValue { get { return targetValue; } }
		public MeasureValue DisplayValue { get { return displayValue; } }
		public MeasureValue DisplaySubValue1 { get { return displaySubValue1; } }
		public MeasureValue DisplaySubValue2 { get { return displaySubValue2; } }
		public bool IsGood { get { return isGood; } }
		public IndicatorType IndicatorType { get { return indicatorType; } }
	}
	public static class DashboardDataAxisNames {
		public const string ChartArgumentAxis = "Argument";
		public const string ChartSeriesAxis = "Series";
		public const string PivotColumnAxis = "Column";
		public const string PivotRowAxis = "Row";
		public const string SparklineAxis = "Sparkline";
		public const string DefaultAxis = "Default";
		internal static int MostPriority(List<string> axisNames) {
			int defaultAxisIndex = axisNames.IndexOf(DefaultAxis);
			if(defaultAxisIndex >= 0) return defaultAxisIndex;
			int chartArgumentAxisIndex = axisNames.IndexOf(ChartArgumentAxis);
			if(chartArgumentAxisIndex >= 0) return chartArgumentAxisIndex;
			int pivotColumnAxisIndex = axisNames.IndexOf(PivotColumnAxis);
			if(pivotColumnAxisIndex >= 0) return pivotColumnAxisIndex;
			return -1;
		}
	}
}
