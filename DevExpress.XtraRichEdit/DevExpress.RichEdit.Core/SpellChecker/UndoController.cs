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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraSpellChecker;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.SpellChecker {
	#region RichEditUndoController
	public class RichEditUndoController : IUndoController {
		readonly IRichEditControl control;
		public RichEditUndoController(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		protected IRichEditControl Control { get { return control; } }
		public IUndoItem GetUndoItemForReplace() {
			return new ReplaceUndoItem(Control);
		}
		public IUndoItem GetUndoItemForSilentReplace() {
			return new SilentReplaceUndoItem(Control);
		}
		public IUndoItem GetUndoItemForDelete() {
			return new DeleteUndoItem(Control);
		}
		public IUndoItem GetUndoItemForIgnore() {
			return new IgnoreUndoItem(Control);
		}
		public IUndoItem GetUndoItemForIgnoreAll() {
			return new IgnoreAllUndoItem(Control);
		}
	}
	#endregion
	#region UndoItemBase (abstract class)
	public abstract class UndoItemBase : IUndoItem {
		#region Fields
		readonly IRichEditControl control;
		DocumentPosition startPosition;
		DocumentPosition finishPosition;
		string oldText;
		#endregion
		protected UndoItemBase(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		protected IRichEditControl Control { get { return control; } }
		public Position StartPosition {
			get { return startPosition; }
			set {
				if (value != null) {
					DocumentPosition pos = value as DocumentPosition;
					if (pos == null)
						Exceptions.ThrowArgumentException("ActualPosition", value);
					startPosition = pos;
				}
				else
					startPosition = null;
			}
		}
		public Position FinishPosition {
			get { return finishPosition; }
			set {
				if (value != null) {
					DocumentPosition pos = value as DocumentPosition;
					if (pos == null)
						Exceptions.ThrowArgumentException("ActualPosition", value);
					finishPosition = pos;
				}
				else
					startPosition = null;
			}
		}
		internal DocumentPosition StartPositionInternal { get { return startPosition; } }
		internal DocumentPosition FinishPositionInternal { get { return finishPosition; } }
		public string OldText { get { return oldText; } set { oldText = value; } }
		public virtual bool NeedRecheckWord { get { return false; } }
		public virtual bool ShouldUpdateItemPosition { get { return false; } }
		#endregion
		public void DoUndo() {
			DoUndoCore();
		}
		protected abstract void DoUndoCore();
	}
	#endregion
	#region UndoItem
	public class UndoItem : UndoItemBase {
		public UndoItem(IRichEditControl control)
			: base(control) {
		}
		protected override void DoUndoCore() {
			InvalidatePositions();
			RichEditCommand command = Control.InnerControl.CreateCommand(RichEditCommandId.Undo);
			command.Execute();
			InvalidatePositions();
		}
		protected void InvalidatePositions() {
			StartPositionInternal.InvalidatePosition();
			FinishPositionInternal.InvalidatePosition();
		}
	}
	#endregion
	#region ReplaceUndoItem
	public class ReplaceUndoItem : UndoItem {
		public ReplaceUndoItem(IRichEditControl control)
			: base(control) {
		}
		public override bool NeedRecheckWord { get { return true; } }
		protected override void DoUndoCore() {
			InvalidatePositions();
			if (ShouldUndo())
				base.DoUndoCore();
		}
		bool ShouldUndo() {
			if (StartPositionInternal == null || FinishPositionInternal == null)
				return false;
			PieceTable pieceTable = Control.InnerControl.DocumentModel.ActivePieceTable;
			DocumentModelPosition startModelPos = StartPositionInternal.Position;
			DocumentModelPosition endModelPos = FinishPositionInternal.Position;
			string newText = pieceTable.GetPlainText(startModelPos, endModelPos);
			return !String.Equals(newText, OldText, StringComparison.Ordinal);
		}
	}
	#endregion
	#region SilentReplaceUndoItem
	public class SilentReplaceUndoItem : UndoItem {
		public SilentReplaceUndoItem(IRichEditControl control)
			: base(control) {
		}
		public override bool NeedRecheckWord { get { return false; } }
		public override bool ShouldUpdateItemPosition { get { return true; } }
	}
	#endregion
	#region DeleteUndoItem
	public class DeleteUndoItem : UndoItem {
		public DeleteUndoItem(IRichEditControl control)
			: base(control) {
		}
		public override bool NeedRecheckWord { get { return true; } }
	}
	#endregion
	#region IgnoreUndoItem
	public class IgnoreUndoItem : UndoItemBase {
		public IgnoreUndoItem(IRichEditControl control)
			: base(control) {
		}
		public override bool NeedRecheckWord { get { return true; } }
		protected PieceTable PieceTable { get { return Control.InnerControl.DocumentModel.ActivePieceTable; } }
		protected IgnoreListManager IgnoreListManager {
			get {
				SpellCheckerController controller = Control.InnerControl.Formatter.SpellCheckerController as SpellCheckerController;
				if (controller == null)
					return null;
				return controller.IgnoreListManager;
			}
		}
		protected override void DoUndoCore() {
			if (IgnoreListManager == null)
				return;
			IIgnoreList ignoreList = IgnoreListManager;
			ignoreList.Remove(StartPosition, FinishPosition, OldText);
			if (StartPositionInternal != null && FinishPositionInternal != null)
				ResetDocumentLayout(StartPositionInternal.Position.RunIndex, FinishPositionInternal.Position.RunIndex);
		}
		protected void ResetDocumentLayout(RunIndex startIndex, RunIndex endIndex) {
			Control.InnerControl.DocumentModel.ResetSpellCheck(startIndex, endIndex);
		}
	}
	#endregion
	#region IgnoreAllUndoItem
	public class IgnoreAllUndoItem : IgnoreUndoItem {
		public IgnoreAllUndoItem(IRichEditControl control)
			: base(control) {
		}
		protected override void DoUndoCore() {
			if (IgnoreListManager == null)
				return;
			IgnoreListManager.Remove(OldText);
			ResetDocumentLayout(RunIndex.Zero, RunIndex.MaxValue);
		}
	}
	#endregion
}
