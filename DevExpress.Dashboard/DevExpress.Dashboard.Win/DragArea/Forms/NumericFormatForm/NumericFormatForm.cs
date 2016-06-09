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
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardWin.Native {
	public partial class NumericFormatForm : DashboardForm {
		readonly DashboardDesigner designer;
		readonly DataDashboardItem dashboardItem;
		readonly DataItem dataItem;
		readonly DataItemNumericFormat currentNumericFormat;
		readonly Locker locker = new Locker();
		readonly decimal previewValue = 1234567890.123M;
		DataItemNumericFormat NumericFormat {
			get {
				DataItemNumericFormatType formatType = ((NumericFormatFormatTypeItem)cbeFormatType.SelectedItem).FormatType;
				DataItemNumericUnit unit = ((NumericFormatUnitItem)cbeUnit.SelectedItem).Unit;
				int precision = Convert.ToInt32(sePrecision.Value);
				string currencyCultureName = currencyChooser.CurrencyCulture;
				bool includeGroupSeparator = cheIncludeGroupSeparator.Checked;
				if(formatType == currentNumericFormat.FormatType && unit == currentNumericFormat.Unit &&
					precision == currentNumericFormat.Precision && currencyCultureName == currentNumericFormat.CurrencyCultureName
					&& includeGroupSeparator == currentNumericFormat.IncludeGroupSeparator)
					return null;
				DataItemNumericFormat numericFormat = new DataItemNumericFormat(null);
				numericFormat.FormatType = formatType;
				numericFormat.Unit = unit;
				numericFormat.Precision = precision;
				numericFormat.CurrencyCultureName = currencyCultureName;
				numericFormat.IncludeGroupSeparator = includeGroupSeparator;
				return numericFormat;
			}
		}
		string CurrentCurrencyCultureName {
			get {
				return currentNumericFormat.CurrencyCultureName;
			}
		}
		public NumericFormatForm() {
			InitializeComponent();
		}
		public NumericFormatForm(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem)
			: this() {
			this.dataItem = dataItem;
			this.dashboardItem = dashboardItem;
			this.designer = designer;
			currentNumericFormat = dataItem.NumericFormat;
			LookAndFeel.ParentLookAndFeel = designer.LookAndFeel;
			mePreview.Initialize(previewValue);
			locker.Lock();
			try {
				foreach(DataItemNumericFormatType formatType in Enum.GetValues(typeof(DataItemNumericFormatType))) {
					cbeFormatType.Properties.Items.Add(new NumericFormatFormatTypeItem(formatType));
				}
				cbeFormatType.SelectedItem = new NumericFormatFormatTypeItem(currentNumericFormat.FormatType);
				foreach(DataItemNumericUnit unit in Enum.GetValues(typeof(DataItemNumericUnit))) {
					cbeUnit.Properties.Items.Add(new NumericFormatUnitItem(unit));
				}
				cbeUnit.SelectedItem = new NumericFormatUnitItem(currentNumericFormat.Unit);
				sePrecision.Value = currentNumericFormat.Precision;
				cheIncludeGroupSeparator.Checked = currentNumericFormat.IncludeGroupSeparator;
				currencyChooser.Initialize(CurrentCurrencyCultureName, DashboardWinLocalizer.GetString(DashboardWinStringId.DataItemCurrencyUseDashboardCurrency));
			}
			finally {
				locker.Unlock();
			}
			UpdateFormState();
		}
		void ButtonOKClick(object sender, EventArgs e) {
			DataItemNumericFormat numericFormat = NumericFormat;
			if(numericFormat != null) {
				NumericFormatHistoryItem historyItem = new NumericFormatHistoryItem(dashboardItem, dataItem, numericFormat);
				historyItem.Redo(designer);
				designer.History.Add(historyItem);
			}
		}
		void NumericFormatPropertyChanged(object sender, EventArgs e) {
			UpdateFormState();
		}
		bool UnitEnabled(DataItemNumericFormatType formatType) {
			return (formatType == DataItemNumericFormatType.Currency || formatType == DataItemNumericFormatType.Number) ? true : false;
		}
		bool PrecisionEnabled(DataItemNumericFormatType formatType, DataItemNumericUnit unit) {
			bool measuredInUnits = formatType == DataItemNumericFormatType.Currency ||
				formatType == DataItemNumericFormatType.Number;
			return (formatType != DataItemNumericFormatType.Auto && formatType != DataItemNumericFormatType.General) && 
				(!measuredInUnits || unit != DataItemNumericUnit.Auto);
		}
		bool GroupSeparatorEnabled(DataItemNumericFormatType formatType) {
			return (formatType == DataItemNumericFormatType.General || 
				formatType == DataItemNumericFormatType.Scientific ||
				formatType == DataItemNumericFormatType.Auto) ? false : true;
		}
		bool CurrencyControlEnabled(DataItemNumericFormatType formatType) {
			return formatType == DataItemNumericFormatType.Currency;
		}
		void UpdateElementState() {
			DataItemNumericFormatType formatType = ((NumericFormatFormatTypeItem)cbeFormatType.SelectedItem).FormatType;
			DataItemNumericUnit unit = ((NumericFormatUnitItem)cbeUnit.SelectedItem).Unit;
			cbeUnit.Enabled = UnitEnabled(formatType);
			sePrecision.Enabled = PrecisionEnabled(formatType, unit);
			currencyChooser.StateEnabled(CurrencyControlEnabled(formatType));
			cheIncludeGroupSeparator.Enabled = GroupSeparatorEnabled(formatType);
		}
		void UpdatePreview() {
			mePreview.UpdatePreview(dataItem, NumericFormat ?? currentNumericFormat);
		}
		void UpdateFormState() {
			if(!locker.IsLocked) {
				UpdateElementState();
				UpdatePreview();
			}
		}
	}
}
