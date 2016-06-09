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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region MouseCursorCalculator
	public class MouseCursorCalculator {
		readonly SpreadsheetView view;
		public MouseCursorCalculator(SpreadsheetView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public SpreadsheetView View { get { return view; } }
		public virtual SpreadsheetCursor Calculate(SpreadsheetHitTestResult hitTestResult, Point physicalPoint) {
			if (hitTestResult == null) {
				return SpreadsheetCursors.Default;
			}
			return CalculateCore(hitTestResult);
		}
		protected internal virtual SpreadsheetCursor CalculateHotZoneCursor(SpreadsheetHitTestResult result) {
			HotZone hotZone = View.CalculateHotZone(result.LogicalPoint);
			return CalculateHotZoneCursorCore(hotZone);
		}
		protected internal virtual SpreadsheetCursor CalculateHotZoneCursorCore(HotZone hotZone) {
			if (hotZone != null && View.Control.InnerControl.IsEditable)
				return hotZone.Cursor;
			else
				return SpreadsheetCursors.Default;
		}
		protected internal virtual SpreadsheetCursor CalculateCore(SpreadsheetHitTestResult result) {
			SpreadsheetCursor cursor = CalculateHotZoneCursor(result);
			if (cursor != SpreadsheetCursors.Default)
				return cursor;
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(View.DocumentModel.ActiveSheet, View.Control.InnerControl);
			if (selectionManager.ShouldSelectComment(result))
				return SpreadsheetCursors.Default;
			else if (selectionManager.ShouldSelectPicture(result) && !selectionManager.IsHyperlinkActive(result)) {
				if (View.DocumentModel.BehaviorOptions.DragAllowed && View.DocumentModel.BehaviorOptions.Drawing.MoveAllowed)
					return SpreadsheetCursors.SizeAll;
				else
					return SpreadsheetCursors.Default;
			}
			else if (selectionManager.ShouldResizeColumn(result, false))
				return SpreadsheetCursors.ResizeColumn;
			else if (selectionManager.ShouldSelectColumn(result))
				return SpreadsheetCursors.SelectColumn;
			else if (selectionManager.ShouldResizeRow(result, false))
				return SpreadsheetCursors.ResizeRow;
			else if (selectionManager.ShouldSelectRow(result))
				return SpreadsheetCursors.SelectRow;
			else if (selectionManager.IsHyperlinkActive(result))
				return SpreadsheetCursors.Hand;
			return SpreadsheetCursors.Default;
		}
	}
	#endregion
}
