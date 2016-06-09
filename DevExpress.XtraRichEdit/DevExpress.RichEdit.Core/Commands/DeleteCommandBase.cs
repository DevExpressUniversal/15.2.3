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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
namespace DevExpress.XtraRichEdit.Commands {
	#region DeleteCommandBase (abstract class)
	public abstract class DeleteCommandBase : RichEditSelectionCommand {
		protected DeleteCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return IsContentEditable && CanEditSelection();
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected internal override void PerformModifyModel() {
			DocumentModel.BeginUpdate();
			try {
				ModifyModel();
				DocumentModel.CheckIntegrity();
			}
			finally {
				DocumentModel.EndUpdate();
			}
			ActiveView.EnforceFormattingCompleteForVisibleArea();
		}
		protected internal virtual void DeleteContentCore(DocumentLogPosition selectionStart, int selectionLength, bool documentLastParagraphSelected) {
			DeleteContentCore(selectionStart, selectionLength, documentLastParagraphSelected, false);
		}
		protected internal virtual void DeleteContentCore(DocumentLogPosition selectionStart, int selectionLength, bool documentLastParagraphSelected, bool backspacePressed) {
			if (backspacePressed)
				ActivePieceTable.DeleteBackContent(selectionStart, selectionLength, documentLastParagraphSelected);
			else
				ActivePieceTable.DeleteContent(selectionStart, selectionLength, documentLastParagraphSelected);
			DocumentModel.Selection.Start = selectionStart;
			DocumentModel.Selection.End = selectionStart;
		}
		protected internal virtual bool ValidateSelectionRanges(SelectionRangeCollection sorted) {
			return true;
		}
		protected internal abstract void ModifyModel();
	}
	#endregion
}
