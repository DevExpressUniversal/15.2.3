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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Mouse {
	#region MouseCursorCalculator
	public class MouseCursorCalculator {
		readonly RichEditView view;
		public MouseCursorCalculator(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public RichEditView View { get { return view; } }
		protected bool IsEditable { get { return view.Control.InnerControl.IsEditable; } }
		public virtual RichEditCursor Calculate(RichEditHitTestResult hitTestResult, Point physicalPoint) {			
			if (hitTestResult == null) {
				hitTestResult = view.CalculateNearestPageHitTest(physicalPoint, false);
				if (hitTestResult != null)
					return CalculateHotZoneCursor(hitTestResult);
				return RichEditCursors.Default;
			}
			return CalculateCore(hitTestResult);
		}
		protected internal virtual RichEditCursor CalculateHotZoneCursor(RichEditHitTestResult result) {
			HotZone hotZone = View.SelectionLayout.CalculateHotZone(result, View);
			if (hotZone == null) {
				EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(View.DocumentModel.ActivePieceTable);
				if (selectionManager.ShouldSelectCommentMoreButton(result))
					return RichEditCursors.Hand;
				if (selectionManager.ShouldSelectComment(result))
					return RichEditCursors.IBeam;
				if (selectionManager.ShouldSelectFloatingObject(result) && IsEditable)
					return RichEditCursors.SizeAll;
			}
			return CalculateHotZoneCursorCore(hotZone);
		}
		protected internal virtual RichEditCursor CalculateHotZoneCursorCore(HotZone hotZone) {
			if (hotZone != null && View.Control.InnerControl.IsEditable)
				return hotZone.Cursor;
			else
				return RichEditCursors.Default;
		}
		protected internal virtual RichEditCursor CalculateCore(RichEditHitTestResult result) {
			RichEditCursor cursor = CalculateHotZoneCursor(result);
			if (cursor != RichEditCursors.Default)
				return cursor;
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(View.DocumentModel.ActivePieceTable);
			TableRowViewInfoBase tableRow = selectionManager.CalculateTableRowToResize(result);
			if (selectionManager.ShouldResizeTableRow(View.Control, result, tableRow))
				return RichEditCursors.ResizeTableRow;
			VirtualTableColumn virtualColumn = selectionManager.CalculateTableCellsToResizeHorizontally(result);
			if (selectionManager.ShouldResizeTableCellsHorizontally(View.Control, result, virtualColumn))
				return RichEditCursors.ResizeTableColumn;
			else if (selectionManager.ShouldSelectEntireTableRow(result))
				return RichEditCursors.SelectRow;
			else if (selectionManager.ShouldSelectEntireTableCell(result))
				return RichEditCursors.SelectTableCell;
			else if (selectionManager.ShouldSelectEntireTableColumn(result))
				return RichEditCursors.SelectTableColumn;
			else if (selectionManager.ShouldSelectFloatingObject(result))
				return IsEditable ? RichEditCursors.SizeAll : CalculateCharacterMouseCursor(result);
			else if (selectionManager.ShouldSelectEntireRow(result))
				return RichEditCursors.SelectRow;
			else if (selectionManager.ShouldSelectToTheEndOfRow(result))
				return CalculateCharacterMouseCursor(result);
			else
				return CalculateCharacterMouseCursor(result);
		}
		protected internal virtual RichEditCursor CalculateCharacterMouseCursor(RichEditHitTestResult hitTestResult) {
			CharacterBox character = hitTestResult.Character;
			if (character.GetRun(hitTestResult.PieceTable).FontItalic)
				return RichEditCursors.IBeamItalic;
			else
				return RichEditCursors.IBeam;
		}
	}
	#endregion
}
