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
using DevExpress.Office.History;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Office;
namespace DevExpress.Snap.Core.History {
	public class SnapDocumentHistory : RichEditDocumentHistory {
		SelectionState selectionState;
		bool insideUndoCurrent;
		public SnapDocumentHistory(SnapDocumentModel documentModel) : base(documentModel) {			
		}
		protected new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public ModificationTracker CreateModificationTracker() {
			int actualCurrentIndex = GetActualCurrentIndex();
			if (Transaction == null)
				return new ModificationTracker(this, actualCurrentIndex);
			return new ModificationTracker(this, actualCurrentIndex, Transaction.Count - 1);
		}
		EventHandler historyCutted;
		public event EventHandler HistoryCutted { add { historyCutted += value; } remove { historyCutted = Delegate.Remove(historyCutted, value) as EventHandler; } }
		protected override void OnCutOffHistory() {
			base.OnCutOffHistory();
			if (historyCutted != null)
				historyCutted(this, EventArgs.Empty);
		}
		protected internal override void BeginUndoCurrent() {
			this.insideUndoCurrent = true;
		}
		protected internal override void EndUndoCurrent() {
			this.insideUndoCurrent = false;
		}
		protected internal int GetActualCurrentIndex() {
			return insideUndoCurrent ? CurrentIndex - 1 : CurrentIndex;
		}
		public override HistoryItem BeginTransaction() {
			HistoryItem result = base.BeginTransaction();
			if (TransactionLevel == 1)
				selectionState = new SelectionState(DocumentModel.Selection.PieceTable, new DocumentLogInterval(DocumentModel.Selection.Start, DocumentModel.Selection.End - DocumentModel.Selection.Start));
			return result;
		}
		public override HistoryItem EndTransaction() {
			HistoryItem result = base.EndTransaction();
			if (TransactionLevel == 0)
				selectionState = null;
			return result;
		}
		public override void Add(HistoryItem item) {
			if (Transaction != null && Transaction.Count == 0)
				AddRestoreSelectionHistoryItem();
			base.Add(item);
		}
		void AddRestoreSelectionHistoryItem() {
			PieceTable pieceTable = selectionState.PieceTable;
			Section section = null;
			if (pieceTable.IsHeaderFooter) {
				SectionHeaderFooterBase headerFooter = pieceTable.ContentType as SectionHeaderFooterBase;
				if (headerFooter != null) {
					SectionIndex sectionIndex = headerFooter.GetSectionIndex();
					int index = ((IConvertToInt<SectionIndex>)sectionIndex).ToInt();
					SectionCollection sections = pieceTable.DocumentModel.Sections;
					if (index >= 0 && index < sections.Count)
						section = sections[sectionIndex];
				}
			}
			base.Add(new RestoreSelectionHistoryItem(selectionState, section));
		}
		public void ChangeSavedSelectionState(SelectionState selectionState) {
			Guard.ArgumentNotNull(selectionState, "selectionState");
			this.selectionState = selectionState;
		}
	}
	public class SelectionState {
		readonly DocumentLogInterval interval;
		readonly PieceTable pieceTable;
		public SelectionState(PieceTable pieceTable, DocumentLogInterval interval) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(interval, "interval");
			this.pieceTable = pieceTable;
			this.interval = interval;
		}
		public DocumentLogInterval Interval { get { return interval; } }
		public PieceTable PieceTable { get { return pieceTable; } }
	}
	public class ModificationTracker : IDisposable, ICloneable<ModificationTracker> {
		int unmodifiedIndex;
		int unmodifiedTransactionIndex;
		SnapDocumentHistory history;
		public ModificationTracker(SnapDocumentHistory history, int unmodifiedIndex) : this(history, unmodifiedIndex, -1) {
		}
		public ModificationTracker(SnapDocumentHistory history, int unmodifiedIndex, int unmodifiedTransactionIndex) {
			Guard.ArgumentNotNull(history, "history");
			this.history = history;
			this.unmodifiedIndex = unmodifiedIndex;
			this.unmodifiedTransactionIndex = unmodifiedTransactionIndex;
			SubscribeHistoryEvents();
		}
		void SubscribeHistoryEvents() {
			history.HistoryCutted += new EventHandler(OnHistoryCutted);
		}
		void UnsubscribeHistoryEvents() {
			history.HistoryCutted -= new EventHandler(OnHistoryCutted);
		}
		void OnHistoryCutted(object sender, EventArgs e) {
			if (unmodifiedIndex > history.CurrentIndex) {
				unmodifiedIndex = DocumentHistory.ForceModifiedIndex;
				unmodifiedTransactionIndex = -1;
				UnsubscribeHistoryEvents();
			}
		}
		public bool Modified { get { return history.IsModified(unmodifiedIndex, unmodifiedTransactionIndex); } }
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (history != null)
					UnsubscribeHistoryEvents();
			}
			history = null;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ModificationTracker() {
			Dispose(false);
		}
		public ModificationTracker Clone() {
			return new ModificationTracker(this.history, this.unmodifiedIndex, this.unmodifiedTransactionIndex);
		}
	}
}
