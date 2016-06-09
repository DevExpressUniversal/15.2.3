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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region ConditionalFormattingDuplicateValuesRuleViewModel
	public class ConditionalFormattingDuplicateValuesRuleViewModel : ConditionalFormattingViewModelBase {
		ConditionalFormattingDuplicateValuesCondition? condition = ConditionalFormattingDuplicateValuesCondition.Duplicate;
		public ConditionalFormattingDuplicateValuesRuleViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		public ConditionalFormattingDuplicateValuesCondition? Condition {
			get { return condition; }
			set {
				if (Condition == value)
					return;
				this.condition = value;
				OnPropertyChanged("Condition");
			}
		}
	}
	#endregion
	#region ConditionalFormattingDuplicateValuesCondition
	public enum ConditionalFormattingDuplicateValuesCondition {
		Duplicate,
		Unique
	}
	#endregion
	#region ConditionalFormattingDateOccurringRuleViewModel
	public class ConditionalFormattingDateOccurringRuleViewModel : ConditionalFormattingViewModelBase {
		readonly static Dictionary<XtraSpreadsheetStringId, ConditionalFormattingTimePeriod> timePeriodStringIdTable = CreateTimePeriodStringIdTable();
		static Dictionary<string, ConditionalFormattingTimePeriod> timePeriodStringTable = CreateTimePeriodStringTable(); 
		string condition = String.Empty;
		public ConditionalFormattingDateOccurringRuleViewModel(ISpreadsheetControl control)
			: base(control) {
			timePeriodStringTable = CreateTimePeriodStringTable(); 
		}
		public IEnumerable<string> Conditions { get { return timePeriodStringTable.Keys; } }
		public string Condition {
			get { return condition; }
			set {
				if (Condition == value)
					return;
				this.condition = value;
				OnPropertyChanged("Condition");
			}
		}
		public static string TimePeriodToString(ConditionalFormattingTimePeriod value) {
			foreach (string key in timePeriodStringTable.Keys)
				if (value == timePeriodStringTable[key])
					return key;
			return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_TimePeriod_Yesterday);
		}
		public static ConditionalFormattingTimePeriod StringToTimePeriod(string value) {
			ConditionalFormattingTimePeriod result;
			if (timePeriodStringTable.TryGetValue(value, out result))
				return result;
			else
				return ConditionalFormattingTimePeriod.Yesterday;
		}
		static Dictionary<XtraSpreadsheetStringId, ConditionalFormattingTimePeriod> CreateTimePeriodStringIdTable() {
			Dictionary<XtraSpreadsheetStringId, ConditionalFormattingTimePeriod> result = new Dictionary<XtraSpreadsheetStringId, ConditionalFormattingTimePeriod>();
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_Yesterday, ConditionalFormattingTimePeriod.Yesterday);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_Today, ConditionalFormattingTimePeriod.Today);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_Tomorrow, ConditionalFormattingTimePeriod.Tomorrow);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_Last7Days, ConditionalFormattingTimePeriod.Last7Days);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_LastWeek, ConditionalFormattingTimePeriod.LastWeek);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_ThisWeek, ConditionalFormattingTimePeriod.ThisWeek);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_NextWeek, ConditionalFormattingTimePeriod.NextWeek);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_LastMonth, ConditionalFormattingTimePeriod.LastMonth);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_ThisMonth, ConditionalFormattingTimePeriod.ThisMonth);
			result.Add(XtraSpreadsheetStringId.Caption_TimePeriod_NextMonth, ConditionalFormattingTimePeriod.NextMonth);
			return result;
		}
		static Dictionary<string, ConditionalFormattingTimePeriod> CreateTimePeriodStringTable() {
			Dictionary<string, ConditionalFormattingTimePeriod> result = new Dictionary<string, ConditionalFormattingTimePeriod>();
			foreach (XtraSpreadsheetStringId key in timePeriodStringIdTable.Keys)
				result[XtraSpreadsheetLocalizer.GetString(key)] = timePeriodStringIdTable[key];
			return result;
		}
	}
	#endregion
	#region ConditionalFormattingTimePeriodCondition
	public enum ConditionalFormattingTimePeriodCondition {
		Yesterday,
		Today,
		Tomorrow,
		Last7Days,
		LastWeek,
		ThisWeek,
		NextWeek,
		LastMonth,
		NextMonth,
		ThisMonth,
	}
	#endregion
}
