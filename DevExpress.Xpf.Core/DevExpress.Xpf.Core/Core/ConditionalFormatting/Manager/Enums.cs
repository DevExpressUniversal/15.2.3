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
using System.ComponentModel;
using System.Windows.Data;
using DevExpress.Xpf.Core.ConditionalFormatting;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public enum FieldEditorMode {
		[Description("ConditionalFormatting_Manager_FieldNameMode")]
		FieldName,
		[Description("ConditionalFormatting_Manager_ExpressionMode")]
		Expression
	}
	public enum DataBarFillMode {
		[Description("ConditionalFormatting_Manager_FillModeSolid")]
		SolidFill,
		[Description("ConditionalFormatting_Manager_FillModeGradient")]
		GradientFill
	}
	public enum DataBarBorderMode {
		[Description("ConditionalFormatting_Manager_DataBarBorder")]
		Border,
		[Description("ConditionalFormatting_Manager_DataBarNoBorder")]
		NoBorder
	}
	public enum TopBottomOperatorType {
		[Description("ConditionalFormatting_Manager_Top")]
		Top,
		[Description("ConditionalFormatting_Manager_Bottom")]
		Bottom
	}
	public enum AboveBelowOperatorType {
		[Description("ConditionalFormatting_Manager_AboveMode")]
		Above,
		[Description("ConditionalFormatting_Manager_BelowMode")]
		Below
	}
	public enum IconValueType {
		[Description("ConditionalFormatting_Manager_PercentMode")]
		Percent,
		[Description("ConditionalFormatting_Manager_NumberMode")]
		Number
	}
	public enum IconComparisonType {
		[Description(">=")]
		GreaterOrEqual,
		[Description(">")]
		Greater
	}
	public enum DataValueType {
		[Description("ConditionalFormatting_Manager_Numeric")]
		Numeric,
		[Description("ConditionalFormatting_Manager_Date")]
		DateTime
	}
	public class EnumNameConverter : IValueConverter {
		public bool UseLocalizer { get; set; }
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(targetType != typeof(string))
				return null;
			string description = DevExpress.Utils.EnumExtensions.GetEnumItemDisplayText(value);
			if(UseLocalizer)
				description = ConditionalFormattingLocalizer.GetString((ConditionalFormattingStringId)Enum.Parse(typeof(ConditionalFormattingStringId), description, false));
			return description;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
}
