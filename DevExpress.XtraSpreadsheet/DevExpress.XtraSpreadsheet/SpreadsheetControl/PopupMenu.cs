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

using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
#if SL || WPF
namespace DevExpress.Xpf.Spreadsheet {
#else
namespace DevExpress.XtraSpreadsheet {
#endif
	#region SpreadsheetControl
	public partial class SpreadsheetControl {
		internal SpreadsheetMenuType GetMenuType(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult.HeaderBox != null)
				return GetHeaderMenuType(hitTestResult.HeaderBox.BoxType);
			else if (DocumentModel.ActiveSheet.Selection.IsDrawingSelected) 
				return GetDrawingMenuType();
			else if (DocumentModel.ActiveSheet.TryGetPivotTable(hitTestResult.CellPosition) != null)
				return SpreadsheetMenuType.PivotTable;
			else
				return SpreadsheetMenuType.Cell;
		}
		SpreadsheetMenuType GetHeaderMenuType(HeaderBoxType headerBoxType) {
			if (headerBoxType == HeaderBoxType.ColumnHeader)
				return SpreadsheetMenuType.ColumnHeading;
			if (headerBoxType == HeaderBoxType.RowHeader)
				return SpreadsheetMenuType.RowHeading;
			System.Diagnostics.Debug.Assert(headerBoxType == HeaderBoxType.SelectAllButton);
			return SpreadsheetMenuType.SelectAllButton;
		}
		SpreadsheetMenuType GetDrawingMenuType() {
			Worksheet activeSheet = DocumentModel.ActiveSheet;
			SheetViewSelection selection = activeSheet.Selection;
			if (selection.IsDrawingMultiSelection)
				return SpreadsheetMenuType.DrawingObjects;
			DrawingObjectType drawingType = activeSheet.DrawingObjects[selection.SelectedDrawingIndexes[0]].DrawingType;
			if (drawingType == DrawingObjectType.Chart)
				return SpreadsheetMenuType.Chart;
			System.Diagnostics.Debug.Assert(drawingType == DrawingObjectType.Picture);
			return SpreadsheetMenuType.Picture;
		}
	}
	#endregion
}
