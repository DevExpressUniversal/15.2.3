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

using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Commands {
	#region SnapInsertTableRowAboveCommand
	public class SnapInsertTableRowAboveCommand : InsertTableRowAboveCommand {
		public SnapInsertTableRowAboveCommand(IRichEditControl control)
			: base(control) {
		}
		protected override InsertTableRowCoreCommandBase CreateInsertTableRowCommand(XtraRichEdit.IRichEditControl control) {
			return new SnapInsertTableRowAboveCoreCommand(control);
		}
	}
	#endregion
	#region SnapInsertTableRowAboveCoreCommand
	public class SnapInsertTableRowAboveCoreCommand : InsertTableRowAboveCoreCommand {
		public SnapInsertTableRowAboveCoreCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override TableRow InsertRow(TableRow row) {
			TableRow result = base.InsertRow(row);
			if (((SnapDocumentModel)ActivePieceTable.DocumentModel).IsDefaultStyle(row.Table.TableStyle.StyleName)) {
				if (result != null) {
					result.Cells.ForEach(e => BorderRemover.RemoveBorder(e.Properties.Borders.BottomBorder));
					row.Cells.ForEach(e => BorderRemover.RemoveBorder(e.Properties.Borders.TopBorder));
				}
			}
			return result;
		}
		protected internal override void PerformModifyModel() {
			((SnapDocumentModel)DocumentModel).SelectionInfo.PerformModifyModelBySelection(base.PerformModifyModel);
			EnsureCaretVisible();
		}
	}
	#endregion
	#region SnapInsertTableRowBelowCommand
	public class SnapInsertTableRowBelowCommand : InsertTableRowBelowCommand {
		public SnapInsertTableRowBelowCommand(IRichEditControl control)
			: base(control) {
		}
		protected override InsertTableRowCoreCommandBase CreateInsertTableRowCommand(XtraRichEdit.IRichEditControl control) {
			return new SnapInsertTableRowBelowCoreCommand(control);
		}
	}
	#endregion
	#region SnapInsertTableRowBelowCoreCommand
	public class SnapInsertTableRowBelowCoreCommand : InsertTableRowBelowCoreCommand {
		public SnapInsertTableRowBelowCoreCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override TableRow InsertRow(TableRow row) {
			TableRow result = base.InsertRow(row);
			if (((SnapDocumentModel)ActivePieceTable.DocumentModel).IsDefaultStyle(row.Table.TableStyle.StyleName)) {
				if (result != null) {
					result.Cells.ForEach(e => BorderRemover.RemoveBorder(e.Properties.Borders.TopBorder));
					row.Cells.ForEach(e => BorderRemover.RemoveBorder(e.Properties.Borders.BottomBorder));
				}
			}
			return result;
		}
		protected internal override void PerformModifyModel() {
			((SnapDocumentModel)DocumentModel).SelectionInfo.PerformModifyModelBySelection(base.PerformModifyModel);
			EnsureCaretVisible();
		}
	}
	#endregion
	#region BorderRemover
	public static class BorderRemover {
		public static void RemoveBorder(BorderBase borders) {
			borders.BeginUpdate();
			borders.Style = BorderLineStyle.None;
			borders.EndUpdate();
		}
	}
	#endregion
}
