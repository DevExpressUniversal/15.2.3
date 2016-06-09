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
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.Office.History {
	#region DocumentHistory
	public class DocumentHistory : IDisposable {
		#region Fields
		public const int ForceModifiedIndex = -2;
		bool isDisposed;
		List<HistoryItem> items;
		int unmodifiedIndex = -1;
		int currentIndex = -1;
		IDocumentModel documentModel;
		CompositeHistoryItem transaction;
		NotificationIdGenerator idGenerator;
		int transactionLevel;
		int disableCount;
		bool suppressRaiseOperationComplete;
		#endregion
		public DocumentHistory(IDocumentModel documentModel) {
			this.items = new List<HistoryItem>();
			this.documentModel = documentModel;
			this.idGenerator = CreateIdGenerator();
		}
		#region Properties
		public bool IsDisposed { get { return isDisposed; } }
		public List<HistoryItem> Items { get { return items; } }
		internal bool IsHistoryDisabled { get { return disableCount > 0; } }
		public HistoryItem this[int index] { get { return items[index]; } }
		public int Count { get { return items.Count; } }
		public int CurrentIndex { get { return currentIndex; } set { currentIndex = value; } }
		public int UnmodifiedIndex { get { return unmodifiedIndex; } set { unmodifiedIndex = value; } }
		public HistoryItem Current { get { return (CurrentIndex >= 0 && CurrentIndex < Count) ? this[CurrentIndex] : null; } }
		public int TransactionLevel { get { return transactionLevel; } }
		public bool CanUndo { get { return CurrentIndex >= 0; } }
		public bool CanRedo { get { return Count > 0 && CurrentIndex < Count - 1; } }
		public CompositeHistoryItem Transaction { get { return transaction; } }
		public virtual bool Modified {
			get {
				return IsModified(unmodifiedIndex);
			}
			set {
				if (value == Modified)
					return;
				if (value)
					unmodifiedIndex = ForceModifiedIndex;
				else
					unmodifiedIndex = CurrentIndex;
				RaiseModifiedChanged();
			}
		}
		public bool SuppressRaiseOperationComplete { get { return suppressRaiseOperationComplete; } set { suppressRaiseOperationComplete = value; } }
		#endregion
		protected IDocumentModel DocumentModel { get { return documentModel; } }
		protected internal void SetTransaction(CompositeHistoryItem value) {
			transaction = value;
		}
		protected internal void SetTransactionLevel(int value) {
			transactionLevel = value;
		}
		protected internal virtual void SetModifiedTextAppended(bool forceRaiseModifiedChanged) {
			if (Modified)
				return;
			unmodifiedIndex--;
			RaiseModifiedChanged();
		}
		protected virtual NotificationIdGenerator CreateIdGenerator() {
			return new NotificationIdGenerator();
		}
		public int GetNotificationId() {
			return idGenerator.GenerateId();
		}
		bool previousModifiedValue;
		protected internal virtual void BeginTrackModifiedChanged() {
			this.previousModifiedValue = Modified;
		}
		protected internal virtual void EndTrackModifiedChanged() {
			if (previousModifiedValue != Modified)
				RaiseModifiedChanged();
		}
		void ClearCore(bool disposeOnlyCutOffItems) {
			DisposeContent(disposeOnlyCutOffItems);
			if (transactionLevel > 0)
				transaction = CreateCompositeHistoryItem();
			items.Clear();
			BeginTrackModifiedChanged();
			try {
				currentIndex = -1;
				unmodifiedIndex = -1;
			}
			finally {
				EndTrackModifiedChanged();
			}
		}
		public void SmartClear() {
			ClearCore(true);
		}
		public void Clear() {
			ClearCore(false);
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				DisposeContent(false);
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region Events
		#region OperationCompleted
		EventHandler onOperationCompleted;
		public event EventHandler OperationCompleted { add { onOperationCompleted += value; } remove { onOperationCompleted -= value; } }
		public virtual void RaiseOperationCompleted() {
			if (onOperationCompleted != null)
				onOperationCompleted(this, EventArgs.Empty);
		}
		#endregion
		#region ModifiedChanged
		EventHandler onModifiedChanged;
		public event EventHandler ModifiedChanged { add { onModifiedChanged += value; } remove { onModifiedChanged -= value; } }
		public virtual void RaiseModifiedChanged() {
			if (onModifiedChanged != null)
				onModifiedChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void DisposeContent(bool cutOffItemsOnly) {
			if (transaction != null)
				transaction.Dispose();
			if (cutOffItemsOnly)
				CutOffHistory();
			else {
				int count = Count;
				for (int i = 0; i < count; i++)
					this[i].Dispose();
			}
		}
		public void Undo() {
			if (!CanUndo)
				return;
			DisableHistory();
			try {
				documentModel.BeginUpdate();
				try {
					BeginTrackModifiedChanged();
					try {
						UndoCore();
					}
					finally {
						EndTrackModifiedChanged();
					}
				}
				finally {
					documentModel.EndUpdate();
				}
			}
			finally {
				EnableHistory();
			}
		}
		protected internal virtual void OnEndUndoCore() {
		}
		protected internal virtual void UndoCore() {
#if DEBUG
			int currentIndexBefore = this.currentIndex;
			int countBefore = Count;
#endif
			BeginUndoCurrent();
			Current.Undo();
#if DEBUG
			System.Diagnostics.Debug.Assert(this.currentIndex == currentIndexBefore);
			System.Diagnostics.Debug.Assert(countBefore == Count);
#endif
			this.currentIndex--;
			EndUndoCurrent();
			OnEndUndoCore();
			RaiseOperationCompleted();
		}
		protected internal virtual void BeginUndoCurrent() {
		}
		protected internal virtual void EndUndoCurrent() {
		}
		public void Redo() {
			if (!CanRedo)
				return;
			DisableHistory();
			try {
				documentModel.BeginUpdate();
				try {
					BeginTrackModifiedChanged();
					try {
						RedoCore();
					}
					finally {
						EndTrackModifiedChanged();
					}
				}
				finally {
					documentModel.EndUpdate();
				}
			}
			finally {
				EnableHistory();
			}
		}
		protected internal virtual void RedoCore() {
			this.currentIndex++;
#if DEBUG
			int currentIndexBefore = this.currentIndex;
			int countBefore = Count;
#endif
			Current.Redo();
#if DEBUG
			System.Diagnostics.Debug.Assert(this.currentIndex == currentIndexBefore);
			System.Diagnostics.Debug.Assert(countBefore == Count);
#endif
			RaiseOperationCompleted();
		}
		public void DisableHistory() {
			disableCount++;
		}
		public void EnableHistory() {
			if (disableCount > 0)
				disableCount--;
		}
		public virtual HistoryItem BeginTransaction() {
			if (transactionLevel == 0)
				transaction = CreateCompositeHistoryItem();
			transactionLevel++;
			return transaction;
		}
		public virtual HistoryItem EndTransaction() {
			if (transactionLevel > 0) {
				transactionLevel--;
				if (transactionLevel == 0) {
					HistoryItem result = CommitTransaction();
					transaction = null;
					return result;
				}
			}
			return transaction;
		}
		protected internal virtual HistoryItem CommitAsSingleItem() {
			System.Diagnostics.Debug.Assert(transaction.Count == 1);
			return CommitAsSingleItemCore(transaction[0]);
		}
		protected internal virtual HistoryItem CommitAsSingleItemCore(HistoryItem singleItem) {
			Add(singleItem);
			return singleItem;
		}
		public virtual HistoryItem CommitTransaction() {
			int itemsCount = transaction.Count;
			if (itemsCount > 0) {
				if (itemsCount == 1) {
					HistoryItem singleItem = CommitAsSingleItem();
					if (singleItem != null)
						return singleItem;
				}
				Add(transaction);
			}
			return transaction;
		}
		void InternalAdd(HistoryItem item) {
			if (!IsHistoryDisabled) {
				CutOffHistory();
				items.Add(item);
				BeginTrackModifiedChanged();
				try {
					currentIndex++;
				}
				finally {
					EndTrackModifiedChanged();
				}
			}
			if (!suppressRaiseOperationComplete)
				RaiseOperationCompleted();
		}
		void CutOffHistory() {
			int index = currentIndex + 1;
			if (index < Count)
				OnCutOffHistory();
			while (index < Count) {
				this[index].Dispose();
				items.RemoveAt(index);
			}
			if (unmodifiedIndex > currentIndex)
				unmodifiedIndex = ForceModifiedIndex;
		}
		protected virtual void OnCutOffHistory() {
		}
		public bool IsModified(int unmodifiedIndex) {
			return IsModified(unmodifiedIndex, -1);
		}
		public bool IsModified(int unmodifiedIndex, int unmodifiedTransactionIndex) {
			if (CurrentIndex == unmodifiedIndex) {
				if (unmodifiedTransactionIndex >= 0 && CheckIsTransactionChangeModifiedBackward(CurrentIndex, unmodifiedTransactionIndex))
					return true;
				return false;
			}
			if (unmodifiedIndex < -1)
				return true;
			if (unmodifiedIndex < CurrentIndex) {
				unmodifiedIndex++;
				if (unmodifiedTransactionIndex >= 0) {
					if (CheckIsTransactionChangeModifiedForward(unmodifiedIndex, unmodifiedTransactionIndex + 1))
						return true;
					unmodifiedIndex++;
				}
				for (int i = unmodifiedIndex ; i <= CurrentIndex && i < Count; i++) {
					if (items[i].ChangeModified)
						return true;
				}
			}
			else {
				if (unmodifiedTransactionIndex >= 0) {
					CheckIsTransactionChangeModifiedBackward(unmodifiedIndex, unmodifiedTransactionIndex);
						unmodifiedIndex--;
				}
				for (int i = CurrentIndex + 1; i <= unmodifiedIndex && i < Count; i++) {
					if (items[i].ChangeModified)
						return true;
				}
			}
			return false;
		}
		bool CheckIsTransactionChangeModifiedForward(int index, int startInnerIndex) {
			if (index < 0)
				return false;
			CompositeHistoryItem transaction = Items[index] as CompositeHistoryItem;
			if (transaction == null) {
				System.Diagnostics.Debug.Assert(false);
				return false;
			}
			List<HistoryItem> items = transaction.Items;
			int count = items.Count;				
			for (int i = startInnerIndex; i < count; i++) {
				if (items[i].ChangeModified)
					return true;
			}
			return false;
		}
		bool CheckIsTransactionChangeModifiedBackward(int index, int startInnerIndex) {
			if (index < 0)
				return false;
			CompositeHistoryItem transaction = Items[index] as CompositeHistoryItem;
			if (transaction == null) {
				System.Diagnostics.Debug.Assert(false);
				return false;
			}
			List<HistoryItem> items = transaction.Items;			
			for (int i = startInnerIndex; i >= 0; i--) {
				if (items[i].ChangeModified)
					return true;
			}
			return false;
		}
		public virtual void Add(HistoryItem item) {
			if (TransactionLevel != 0) {
				Transaction.AddItem(item);
			}
			else
				InternalAdd(item);
		}
		public virtual void AddEmptyOperation() {
		}
		public virtual bool HasChangesInCurrentTransaction() {
			if (transactionLevel <= 0)
				return false;
			else
				return transaction.Count > 0;
		}
		public virtual HistoryItem BeginSyntaxHighlight() {
			return transaction;
		}
		public virtual void EndSyntaxHighlight() {
		}
		protected internal virtual CompositeHistoryItem CreateCompositeHistoryItem() {
			return new CompositeHistoryItem(documentModel.MainPart);
		}
	}
	#endregion
	#region NotificationIdGenerator
	public class NotificationIdGenerator {
		public const int EmptyId = 0;
		int lastId = Int32.MinValue;
		public virtual int GenerateId() {
			lastId++;
			if (lastId == EmptyId)
				lastId++;
			return lastId;
		}
	}
	#endregion
}
