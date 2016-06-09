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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using DevExpress.Xpf.GridData;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Native {
	public abstract class IndicatorFormatConditionInfo : FormatConditionBaseInfo {
		public string SelectiveExpression { get; set; }
		IndicatorFormatBase IndicatorFormat { get { return FormatCore as IndicatorFormatBase; } }
		public IndicatorFormatConditionInfo() {
			selectiveColumnInfo = new ConditionalFormattingColumnInfo(() => SelectiveExpression);
		}
		decimal? actualMin;
		decimal? actualMax;
		public override IEnumerable<ConditionalFormatSummaryType> GetSummaries() {
			if(IndicatorFormat != null) {
				if(actualMin == null)
					yield return ConditionalFormatSummaryType.Min;
				if(actualMax == null)
					yield return ConditionalFormatSummaryType.Max;
			}
		}
		public void OnMinMaxChanged(object newValue, bool isMax) {
			decimal? actualValue = IndicatorFormatConditionInfo.GetParsedDecimalValue(newValue);
			if(isMax)
				actualMax = actualValue;
			else
				actualMin = actualValue;
		}
		public static decimal? GetParsedDecimalValue(object value) {
			decimal? actualValue = IndicatorFormatBase.GetDecimalValue(value);
			if(!actualValue.HasValue && value != null) {
				decimal parsedValue;
				if(Decimal.TryParse(value.ToString(), out parsedValue))
					actualValue = parsedValue;
			}
			return actualValue;
		} 
		readonly IColumnInfo selectiveColumnInfo;
		public override IEnumerable<IColumnInfo> GetUnboundColumnInfo() {
			IEnumerable<IColumnInfo> info = base.GetUnboundColumnInfo();
			if(selectiveColumnInfo.UnboundExpression != null)
				info = info.Concat(new[] { selectiveColumnInfo });
			return info;
		}
		public override Brush CoerceBackground(Brush value, FormatValueProvider provider) {
			return Coerce(value, provider, IndicatorFormat.CoerceBackground);
		}
		public override DataBarFormatInfo CoerceDataBarFormatInfo(DataBarFormatInfo value, FormatValueProvider provider) {
			return Coerce(value, provider, IndicatorFormat.CoerceDataBarFormatInfo);
		}
		T Coerce<T>(T value, FormatValueProvider provider, Func<T, FormatValueProvider, Nullable<decimal>, Nullable<decimal>, T> coerceAction) {
			if(selectiveColumnInfo.UnboundExpression == null || provider.GetSelectiveValue(selectiveColumnInfo.FieldName))
				return coerceAction(value, provider, actualMin, actualMax);
			return value;
		}
	}
	public class ColorScaleFormatConditionInfo : IndicatorFormatConditionInfo {
		public override string OwnerPredefinedFormatsPropertyName { get { return "PredefinedColorScaleFormats"; } }
		public override ConditionalFormatMask FormatMask { get { return ConditionalFormatMask.Background; } }
	}
	public class DataBarFormatConditionInfo : IndicatorFormatConditionInfo {
		public override string OwnerPredefinedFormatsPropertyName { get { return "PredefinedDataBarFormats"; } }
		public override ConditionalFormatMask FormatMask { get { return ConditionalFormatMask.DataBarOrIcon; } }
	}
	public class IconSetFormatConditionInfo : IndicatorFormatConditionInfo {
		public override string OwnerPredefinedFormatsPropertyName { get { return "PredefinedIconSetFormats"; } }
		IconSetFormat IconFormat { get { return FormatCore as IconSetFormat; } }
		public override IEnumerable<ConditionalFormatSummaryType> GetSummaries() {
			if(IconFormat.ElementThresholdType == ConditionalFormattingValueType.Percent)
				return base.GetSummaries();
			return Enumerable.Empty<ConditionalFormatSummaryType>();
		}
		public override ConditionalFormatMask FormatMask { get { return ConditionalFormatMask.DataBarOrIcon; } }
	}
}
