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
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region CalculationIterationsEnabledHistoryItem
	public class CalculationIterationsEnabledHistoryItem : SpreadsheetBooleanHistoryItem {
		#region Fields
		readonly CalculationOptions calculationOptions;
		#endregion
		public CalculationIterationsEnabledHistoryItem(CalculationOptions calculationOptions, bool oldValue, bool newValue)
			: base(calculationOptions.DocumentModel, oldValue, newValue) {
			this.calculationOptions = calculationOptions;
		}
		protected override void UndoCore() {
			calculationOptions.SetIterationsEnabledCore(OldValue);
		}
		protected override void RedoCore() {
			calculationOptions.SetIterationsEnabledCore(NewValue);
		}
	}
	#endregion
	#region CalculationPrecisionAsDicplayedHistoryItem
	public class CalculationPrecisionAsDisplayedHistoryItem : SpreadsheetBooleanHistoryItem {
		#region Fields
		CalculationOptions CalculationOptions { get { return Workbook.Properties.CalculationOptions; } }
		#endregion
		public CalculationPrecisionAsDisplayedHistoryItem(DocumentModel documentModel, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
		}
		protected override void UndoCore() {
			CalculationOptions.SetPrecisionAsDisplayedCore(OldValue);
		}
		protected override void RedoCore() {
			CalculationOptions.SetPrecisionAsDisplayedCore(NewValue);
		}
	}
	#endregion
	public class MarkupDependentsForRecalculationHistoryItem : SpreadsheetHistoryItem {
		readonly CellRangeBase cellRange;
		public MarkupDependentsForRecalculationHistoryItem(DocumentModel documentModel, CellRangeBase cellRange)
			: base(documentModel) {
				this.cellRange = cellRange;
		}
		protected override void RedoCore() {
			Workbook.CalculationChain.MarkupDependentsForRecalculation(cellRange);
		}
		protected override void UndoCore() {
			Workbook.CalculationChain.MarkupDependentsForRecalculation(cellRange);
		}
	}
}
