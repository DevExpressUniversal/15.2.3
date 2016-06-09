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

using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Commands {
	#region SnapMergeTableElementCommandBase (abstract class)
	public abstract class SnapMergeTableElementCommandBase : MergeTableElementCommandBase {
		protected SnapMergeTableElementCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!state.Enabled)
				return;
			SelectedCellsCollection cells = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			SnapPieceTable pieceTable = (SnapPieceTable)ActivePieceTable;
			DocumentModel.BeginUpdate();
			try {
				for (int i = 0; i < cells.RowsCount; i++) {
					if (!TableCommandsHelper.IsWholeSelectionInOneBookmark(pieceTable, cells[i], cells.TopLeftCell)) {
						state.Enabled = false;
						break;
					}
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override void PerformModifyModel() {
			((SnapDocumentModel)DocumentModel).SelectionInfo.PerformModifyModelBySelection(base.PerformModifyModel);
			EnsureCaretVisible();
		}
	}
	#endregion
	#region SnapMergeTableElementMenuCommand
	public class SnapMergeTableElementMenuCommand : SnapMergeTableElementCommandBase {
		public SnapMergeTableElementMenuCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.MergeTableElement; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
	#region SnapMergeTableCellsCommand
	public class SnapMergeTableCellsCommand : SnapMergeTableElementCommandBase {
		public SnapMergeTableCellsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.MergeTableCells; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandsRestriction(state, Options.DocumentCapabilities.Tables, state.Enabled);
		}
	}
	#endregion
}
