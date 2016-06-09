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
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public enum DashboardFormatConditionTopBottomType { Top, Bottom }
	public enum DashboardFormatConditionComparisonType { Greater, GreaterOrEqual }
	public enum DashboardFormatConditionAboveBelowType { Above, AboveOrEqual, Below, BelowOrEqual }
	public enum DashboardFormatConditionValueType { Number, Percent, Automatic }
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum DashboardFormatCondition {
		Greater, GreaterOrEqual, Less, LessOrEqual, Equal, NotEqual, Between, NotBetween, BetweenOrEqual, NotBetweenOrEqual, ContainsText
	};
	public enum FormatConditionIntersectionLevelMode { Auto, FirstLevel, LastLevel, AllLevels, SpecificLevel }
	internal enum DashboardFormatConditionUniqueDuplicateType { Unique, Duplicate }
	static class FormatRuleEnumExtensions {
		internal static SummaryItemTypeEx ToSummaryItemType(this DashboardFormatConditionUniqueDuplicateType type) {
			return type == DashboardFormatConditionUniqueDuplicateType.Duplicate ? SummaryItemTypeEx.Duplicate : SummaryItemTypeEx.Unique;
		}
		public static SummaryItemTypeEx ToSummaryItemType(this DashboardFormatConditionTopBottomType type, DashboardFormatConditionValueType valueType) {
			switch(valueType) {
				case DashboardFormatConditionValueType.Number:
					return type == DashboardFormatConditionTopBottomType.Top ? SummaryItemTypeEx.Top : SummaryItemTypeEx.Bottom;
				case DashboardFormatConditionValueType.Percent:
					return type == DashboardFormatConditionTopBottomType.Top ? SummaryItemTypeEx.TopPercent : SummaryItemTypeEx.BottomPercent;
				default:
					throw new InvalidEnumArgumentException("ValueType", (int)valueType, typeof(DashboardFormatConditionValueType));
			}
		}
		public static string ToSign(this DashboardFormatConditionComparisonType type) {
			return type == DashboardFormatConditionComparisonType.Greater ? ">" : ">=";
		}
		public static string ToSign(this DashboardFormatCondition type) {
			switch(type) {
				case DashboardFormatCondition.Equal: return "=";
				case DashboardFormatCondition.NotEqual: return "≠";
				case DashboardFormatCondition.Less: return "<";
				case DashboardFormatCondition.Greater: return ">";
				case DashboardFormatCondition.GreaterOrEqual: return "≥";
				case DashboardFormatCondition.LessOrEqual: return "≤";
				case DashboardFormatCondition.Between:
				case DashboardFormatCondition.NotBetween: return "()";
				case DashboardFormatCondition.BetweenOrEqual:
				case DashboardFormatCondition.NotBetweenOrEqual: return "[]";
			}
			return string.Empty;
		}
	}
}
