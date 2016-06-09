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
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Commands {
	#region SelectionBasedCommandBase
	public abstract class SelectionBasedCommandBase : RichEditCommand {
		protected SelectionBasedCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal virtual DocumentModelPosition CalculateStartPosition(SelectionItem item, bool allowSelectionExpanding) {
			return item.CalculateStartPosition(allowSelectionExpanding);
		}
		protected internal virtual DocumentModelPosition CalculateEndPosition(SelectionItem item, bool allowSelectionExpanding) {
			return item.CalculateEndPosition(allowSelectionExpanding);
		}
	}
	#endregion
	#region SelectionBasedPropertyChangeCommandBase (abstract class)
	public abstract class SelectionBasedPropertyChangeCommandBase : SelectionBasedCommandBase {
		protected SelectionBasedPropertyChangeCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal virtual bool ValidateUIState(ICommandUIState state) {
			return true;
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			if (!ValidateUIState(state))
				return;
			NotifyBeginCommandExecution(state);
			try {
				Control.BeginUpdate();
				try {
					ModifyDocumentModel(state);
					if (!DocumentModel.IsUpdateLocked)
						ActiveView.EnsureCaretVisible();
					else
						DocumentModel.DeferredChanges.EnsureCaretVisible = true;
				}
				finally {
					Control.EndUpdate();
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ModifyDocumentModel(ICommandUIState state) {
			DocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				ModifyDocumentModelCore(state);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void ModifyDocumentModelCore(ICommandUIState state) {
			List<SelectionItem> items = GetSelectionItems();
			int count = items.Count;
			DocumentModelChangeActions actions = DocumentModelChangeActions.None;
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				bool selectionValid = ValidateSelectionInterval(item);
				DocumentModelPosition start = CalculateStartPosition(item, true);
				DocumentModelPosition end = CalculateEndPosition(item, true);
				if (selectionValid)
					actions |= ChangeProperty(start, end, state);
			}
			ActivePieceTable.ApplyChangesCore(actions, RunIndex.DontCare, RunIndex.DontCare);
		}
		protected internal override bool CanEditSelection() {
			List<SelectionItem> items = GetSelectionItems();
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				DocumentModelPosition start = CalculateStartPosition(item, true);
				DocumentModelPosition end = CalculateEndPosition(item, true);
				if (!ActivePieceTable.CanEditRange(start.LogPosition, end.LogPosition))
					return false;
			}
			return true;
		}
		protected internal virtual List<SelectionItem> GetSelectionItems() {
			return DocumentModel.Selection.Items;
		}
		protected internal virtual bool ValidateSelectionInterval(SelectionItem item) {
			return true;
		}
		protected internal abstract DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state);
	}
	#endregion
}
