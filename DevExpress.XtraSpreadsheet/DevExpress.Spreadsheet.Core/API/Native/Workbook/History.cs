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

namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Office.Model;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using DevExpress.Spreadsheet;
	#region NativeHistory
	partial class NativeHistory : SpreadsheetHistory  {
		ModelWorkbook workbook;
		public NativeHistory(ModelWorkbook workbook) {
			this.workbook = workbook;
		}
		#region SpreadsheetHistory Members
		public void Undo() {
			workbook.History.Undo();
		}
		public void Redo() {
			workbook.History.Redo();
		}
		public void Clear() {
			workbook.History.Clear();
		}
		public int Count {
			get { return workbook.History.Count; }
		}
		public bool IsEnabled {
			get { return workbook.IsNormalHistory; }
			set {
				if(value == workbook.IsNormalHistory)
					return;
				if (workbook.History != null && workbook.History.TransactionLevel > 0)
					throw new System.InvalidOperationException("History can not be switched off when an active transaction exists.");
				if(value)
					workbook.SwitchToNormalHistory(true);
				else
					workbook.SwitchToEmptyHistory(true);
			}
		}
		#endregion
	}
	#endregion
}
