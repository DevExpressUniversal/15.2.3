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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.History {
	public class RestoreSelectionHistoryItem : RichEditHistoryItem {
		public RestoreSelectionHistoryItem(SelectionState selectionState, Section section)
			: base(selectionState.PieceTable) {
			SelectionState = selectionState;
			Section = section;
		}
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public new SnapPieceTable PieceTable { get { return (SnapPieceTable)base.PieceTable; } }
		public SelectionState SelectionState { get; set; }
		public Section Section { get; set; }
		public override bool ChangeModified { get { return false; } }
		protected override void RedoCore() {
			SetSelection(SelectionState.Interval);
		}
		protected override void UndoCore() {
			SetSelection(SelectionState.Interval);
		}
		protected virtual void SetSelection(DocumentLogInterval selectionInterval) {
			ValidateActivePieceTable();
			Selection selection = DocumentModel.Selection;
			selection.ClearSelectionInTable();
			selection.Start = selectionInterval.Start;
			selection.End = selectionInterval.Start + selectionInterval.Length;
			selection.SetStartCell(selectionInterval.Start);
			selection.UpdateTableSelectionStart(selectionInterval.Start);
			selection.UpdateTableSelectionEnd(selectionInterval.Start + selectionInterval.Length);
			DocumentModel.SelectionInfo.CheckCurrentSnapBookmark(true, false);
		}
		protected virtual void ValidateActivePieceTable() {
			DocumentModel.SetActivePieceTable(PieceTable, Section);
		}
	}
}
