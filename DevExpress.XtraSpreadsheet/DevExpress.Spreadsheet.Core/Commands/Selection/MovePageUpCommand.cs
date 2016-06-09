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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout.Engine;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SelectionMovePageUpCommand
	public class SelectionMovePageUpCommand : SelectionMovePageUpDownCommand {
		public SelectionMovePageUpCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.SelectionMovePageUp; } }
		protected override void SetupAnchor(DocumentLayoutAnchor anchor, PageGrid rows) {
			anchor.CellPosition = new CellPosition(ActiveSheet.ActiveView.ScrolledTopLeftCell.Column, rows.ActualFirst.ModelIndex);
			anchor.VerticalFarAlign = true;
		}
		protected override void CalculateFinalLayout() {
			InnerControl.ResetDocumentLayout();
		}
	}
	#endregion
	#region SelectionMovePageUpDownCommand
	public abstract class SelectionMovePageUpDownCommand : SpreadsheetSelectionCommand {
		protected SelectionMovePageUpDownCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract void SetupAnchor(DocumentLayoutAnchor anchor, PageGrid rows);
		protected internal override bool ChangeSelection() {
			DocumentLayout documentLayout = this.InnerControl.DesignDocumentLayout;
			PageGrid rows = documentLayout.Pages[documentLayout.Pages.Count - 1].GridRows;
			int previousRowIndex = Selection.ActiveCell.Row;
			int verticalOffset = Selection.ActiveCell.Row - rows.ActualFirst.ModelIndex;
			DocumentLayoutAnchor anchor = new DocumentLayoutAnchor();
			SetupAnchor(anchor, rows);
			InnerControl.CalculateDocumentLayout(anchor);
			CalculateFinalLayout();
			documentLayout = this.InnerControl.DesignDocumentLayout;
			rows = documentLayout.Pages[documentLayout.Pages.Count - 1].GridRows;
			int rowIndex = Math.Max(rows.ActualFirstIndex, Math.Min(rows.ActualLastIndex - 1, rows.ActualFirstIndex + verticalOffset));
			int rowModelIndex = rows[rowIndex].ModelIndex;
			return ChangeSelection(new CellPosition(Selection.ActiveCell.Column, rowModelIndex), rowModelIndex - previousRowIndex);
		}
		protected virtual bool ChangeSelection(CellPosition position, int modelRowsOffset) {
			return Selection.SetSelection(position);
		}
		protected virtual void CalculateFinalLayout() {
		}
		protected override void RecalculateLayoutEnsureActiveCellVisible(DocumentLayout documentLayout) {
		}
	}
	#endregion
}
