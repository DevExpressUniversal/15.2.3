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

using System.Windows;
using System.Windows.Media;
using System;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public abstract class IndicatorFormatBase : Freezable {
		internal static decimal? GetDecimalValue(object value) {
			if(value == null)
				return null;
			TypeCode typeCode = Type.GetTypeCode(value.GetType());
			if(typeCode == TypeCode.DateTime)
				return ((DateTime)value).Ticks;
			if(IsNumericTypeCode(typeCode)) {
				try {
					return Convert.ToDecimal(value);
				}
				catch(OverflowException) {
					return null;
				}
			}
			return null;
		}
		internal static bool IsNumericTypeCode(TypeCode typeCode) {
			return typeCode >= TypeCode.SByte && typeCode <= TypeCode.Decimal;
		}
		internal static bool IsNumericOrDateTimeTypeCode(TypeCode typeCode) {
			return IsNumericTypeCode(typeCode) || typeCode == TypeCode.DateTime;
		}
		internal static decimal? GetSummaryValue(FormatValueProvider provider, ConditionalFormatSummaryType summaryType, decimal? value) {
			return value ?? GetSummaryValue(provider, summaryType);
		}
		internal static decimal? GetSummaryValue(FormatValueProvider provider, ConditionalFormatSummaryType summaryType) {
			return GetDecimalValue(provider.GetTotalSummaryValue(summaryType));
		}
		internal static decimal GetNormalizedValue(decimal value, decimal min, decimal max) {
			if(value > max) return max;
			if(value < min) return min;
			return value;
		}
		public virtual Brush CoerceBackground(Brush value, FormatValueProvider provider, decimal? minValue, decimal? maxValue) {
			return value;
		}
		public virtual DataBarFormatInfo CoerceDataBarFormatInfo(DataBarFormatInfo value, FormatValueProvider provider, decimal? minValue, decimal? maxValue) {
			return value;
		}
	}
}
