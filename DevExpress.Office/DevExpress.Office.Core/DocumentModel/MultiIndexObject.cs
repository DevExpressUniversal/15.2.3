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
using DevExpress.Office.Localization;
using DevExpress.Office.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
namespace DevExpress.Office {
	#region MultiIndexObject<TOwner, TActions> (abstract class)
	public abstract class MultiIndexObject<TOwner, TActions> : IBatchUpdateable, IBatchInit, IBatchUpdateHandler, IBatchInitHandler, ISupportsSizeOf 
		where TOwner : MultiIndexObject<TOwner, TActions>
		where TActions : struct {
		MultiIndexBatchUpdateHelper batchUpdateHelper;
		public IDocumentModel DocumentModel { get { return GetDocumentModel(); } }
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		protected MultiIndexBatchUpdateHelper BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper != null && batchUpdateHelper.IsUpdateLocked; } }
		protected abstract IDocumentModel GetDocumentModel();
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			if (batchUpdateHelper == null)
				batchUpdateHelper = CreateBatchUpdateHelper();
			if (!(batchUpdateHelper is MultiIndexBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidBeginUpdate);
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			if (!(batchUpdateHelper is MultiIndexBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndUpdate);
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			if (!(batchUpdateHelper is MultiIndexBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndUpdate);
			batchUpdateHelper.CancelUpdate();
		}
		#endregion
		#region IBatchInit implementation
		public void BeginInit() {
			if (batchUpdateHelper == null)
				batchUpdateHelper = CreateBatchInitHelper();
			if (!(batchUpdateHelper is MultiIndexBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidBeginInit);
			batchUpdateHelper.BeginUpdate();
		}
		public void EndInit() {
			if (!(batchUpdateHelper is MultiIndexBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndInit);
			batchUpdateHelper.EndUpdate();
		}
		public void CancelInit() {
			if (!(batchUpdateHelper is MultiIndexBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndInit);
			batchUpdateHelper.CancelUpdate();
		}
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdateCore();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdateCore();
		}
		#endregion
		#region IBatchInitHandler implementation
		void IBatchInitHandler.OnFirstBeginInit() {
			int count = IndexAccessors.Length;
			for (int i = 0; i < count; i++)
				IndexAccessors[i].InitializeDeferredInfo(GetOwner());
		}
		void IBatchInitHandler.OnBeginInit() {
		}
		void IBatchInitHandler.OnEndInit() {
		}
		void IBatchInitHandler.OnCancelInit() {
		}
		void IBatchInitHandler.OnLastEndInit() {
			OnLastEndInitCore();
		}
		void IBatchInitHandler.OnLastCancelInit() {
			OnLastEndInitCore();
		}
		void OnLastEndInitCore() {
			if (batchUpdateHelper.IsIndexRecalculationOnEndInitEnabled) {
				int count = IndexAccessors.Length;
				for (int i = 0; i < count; i++)
					IndexAccessors[i].SetIndex(GetOwner(), IndexAccessors[i].GetDeferredInfoIndex(GetOwner()));
			}
#if DEBUG
			int accessorCount = IndexAccessors.Length;
			for (int i = 0; i < accessorCount; i++)
				System.Diagnostics.Debug.Assert(IndexAccessors[i].IsIndexValid(GetOwner(), IndexAccessors[i].GetIndex(GetOwner())));
#endif
			batchUpdateHelper = null;
		}
		#endregion
		protected internal virtual void OnFirstBeginUpdateCore() {
			batchUpdateHelper.Transaction = new HistoryTransaction(DocumentModel.History);
			int count = IndexAccessors.Length;
			for (int i = 0; i < count; i++)
				IndexAccessors[i].InitializeDeferredInfo(GetOwner());
			BatchUpdateHelper.BeginUpdateDeferredChanges();
		}
		protected internal virtual void OnLastCancelUpdateCore() {
			BatchUpdateHelper.CancelUpdateDeferredChanges();
			batchUpdateHelper.Transaction.Dispose();
			batchUpdateHelper = null;
		}
		protected internal virtual void OnLastEndUpdateCore() {
			BatchUpdateHelper.EndUpdateDeferredChanges();
			bool isFakeAssign = true;
			int count = IndexAccessors.Length;
			for (int i = 0; i < count; i++)
				isFakeAssign = !IndexAccessors[i].ApplyDeferredChanges(GetOwner()) && isFakeAssign;
			if (isFakeAssign) {
				if (batchUpdateHelper.FakeAssignDetected)
					NotifyFakeAssign();
			}
			batchUpdateHelper.Transaction.Dispose();
			batchUpdateHelper = null;
		}
		protected internal virtual void NotifyFakeAssign() {
			if (IsUpdateLocked)
				batchUpdateHelper.FakeAssignDetected = true;
			else {
				DocumentModel.BeginUpdate();
				try {
					OnBeginAssign();
					try {
					}
					finally {
						OnEndAssign();
					}
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		protected internal virtual void OnBeginAssign() {
		}
		protected internal virtual void OnEndAssign() {
		}
		#region Index management
		protected internal delegate TActions SetPropertyValueDelegate<TInfo, U>(TInfo info, U newValue);
		protected internal virtual void SetPropertyValue<TInfo, U>(IIndexAccessor<TOwner, TInfo, TActions> indexHolder, SetPropertyValueDelegate<TInfo, U> setter, U newValue) where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
			DocumentModel.BeginUpdate();
			try {
				SetPropertyValueCore(indexHolder, setter, newValue);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected virtual void SetPropertyValueCore<TInfo, U>(IIndexAccessor<TOwner, TInfo, TActions> indexHolder, SetPropertyValueDelegate<TInfo, U> setter, U newValue) where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
			TInfo info = GetInfoForModification(indexHolder);
			TActions changeActions = setter(info, newValue);
			ReplaceInfo(indexHolder, info, changeActions);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected internal delegate TActions SetPropertyValueDelegateForStruct<TInfo, U>(ref TInfo info, U newValue);
		protected internal virtual void SetPropertyValueForStruct<TInfo, U>(IIndexAccessor<TOwner, TInfo, TActions> indexHolder, SetPropertyValueDelegateForStruct<TInfo, U> setter, U newValue) where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
			TInfo info = GetInfoForModification(indexHolder);
			TActions changeActions = setter(ref info, newValue);
			ReplaceInfoForFlags(indexHolder, info, changeActions);
		}
		public virtual TInfo GetInfoForModification<TInfo>(IIndexAccessor<TOwner, TInfo, TActions> indexHolder) where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
			if (IsUpdateLocked)
				return indexHolder.GetDeferredInfo(BatchUpdateHelper);
			else
				return indexHolder.GetInfo(GetOwner()).Clone();
		}
		public virtual bool ReplaceInfo<TInfo>(IIndexAccessor<TOwner, TInfo, TActions> indexHolder, TInfo newValue, TActions changeActions) where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
			if (IsUpdateLocked) {
				return false;
			}
			int newIndex = indexHolder.GetInfoIndex(GetOwner(), newValue);
			return ChangeIndex(indexHolder, newIndex, changeActions);
		}
		protected internal virtual int ValidateNewIndexBeforeReplaceInfo(int newValue) {
			return newValue;
		}
		public virtual bool ReplaceInfoForFlags<TInfo>(IIndexAccessor<TOwner, TInfo, TActions> indexHolder, TInfo newValue, TActions changeActions) where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
			if (IsUpdateLocked) {
				indexHolder.SetDeferredInfo(BatchUpdateHelper, newValue);
				return false;
			}
			int newIndex = indexHolder.GetInfoIndex(GetOwner(), newValue);
			return ChangeIndex(indexHolder, newIndex, changeActions);
		}
		bool ChangeIndex(IIndexAccessorBase<TOwner, TActions> indexHolder, int newIndex, TActions changeActions) {
			System.Diagnostics.Debug.Assert(indexHolder.IsIndexValid(GetOwner(), newIndex));
			if (newIndex != indexHolder.GetIndex(GetOwner())) {
				ChangeIndexCore(indexHolder, newIndex, changeActions);
				return true;
			}
			else
				return false;
		}
		public virtual void ChangeIndexCore(IIndexAccessorBase<TOwner, TActions> indexHolder, int newIndex, TActions changeActions) {
			IDocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				OnBeginAssign();
				try {
					IndexChangedHistoryItemCore<TActions> item = indexHolder.CreateHistoryItem(GetOwner());
					System.Diagnostics.Debug.Assert(item != null);
					item.OldIndex = indexHolder.GetIndex(GetOwner());
					item.NewIndex = newIndex;
					item.ChangeActions = changeActions;
					documentModel.History.Add(item);
					item.Execute();
				}
				finally {
					OnEndAssign();
				}
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		public void SetIndexCore<TInfo>(IIndexAccessor<TOwner, TInfo, TActions> indexHolder, int value, TActions changeActions) where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
			if (indexHolder.GetIndex(GetOwner()) == value)
				return;
			indexHolder.SetIndex(GetOwner(), value);
			System.Diagnostics.Debug.Assert(indexHolder.IsIndexValid(GetOwner(), indexHolder.GetIndex(GetOwner())));
			ApplyChanges(changeActions);
		}
		#endregion
		public void CloneCore(TOwner clone) {
			int count = IndexAccessors.Length;
			for (int i = 0; i < count; i++) {
				IIndexAccessorBase<TOwner, TActions> accessor = IndexAccessors[i];
				accessor.SetIndex(clone, accessor.GetIndex(GetOwner()));
			}
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
		void CopyFromSameModel(MultiIndexObject<TOwner, TActions> obj) {
			if (!Object.ReferenceEquals(this.DocumentModel, obj.DocumentModel))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidCopyFromDocumentModel);
			int count = IndexAccessors.Length;
			for (int i = 0; i < count; i++) {
				if (!ChangeIndex(IndexAccessors[i], obj.IndexAccessors[i].GetIndex(obj.GetOwner()), GetBatchUpdateChangeActions()))
					NotifyFakeAssign();
			}
		}
		public virtual void CopyFrom(MultiIndexObject<TOwner, TActions> obj) {
			if (Object.ReferenceEquals(this.DocumentModel, obj.DocumentModel)) {
				CopyFromSameModel(obj);
				return;
			}
			try {
				System.Diagnostics.Debug.Assert(!this.IsUpdateLocked);
				this.BeginUpdate();
				CopyFromDeferred(obj);
			}
			finally {
				this.EndUpdate();
			}
		}
		public void CopyFromDeferred(MultiIndexObject<TOwner, TActions> obj) {
			int count = IndexAccessors.Length;
			for (int i = 0; i < count; i++) {
				IIndexAccessorBase<TOwner, TActions> accessor = IndexAccessors[i];
				accessor.CopyDeferredInfo(GetOwner(), obj.GetOwner());
			}
		}
		public override bool Equals(object obj) {
			MultiIndexObject<TOwner, TActions> other = obj as MultiIndexObject<TOwner, TActions>;
			if (other == null)
				return false;
			int count = IndexAccessors.Length;
			for (int i = 0; i < count; i++) {
				IIndexAccessorBase<TOwner, TActions> accessor = IndexAccessors[i];
				if (accessor.GetIndex(GetOwner()) != accessor.GetIndex(other.GetOwner()))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			CombinedHashCode code = new CombinedHashCode();
			int count = IndexAccessors.Length;
			for (int i = 0; i < count; i++)
				code.AddInt( IndexAccessors[i].GetIndex(GetOwner()) );
			return code.CombinedHash32;
		}
		protected abstract IIndexAccessorBase<TOwner, TActions>[] IndexAccessors { get; }
		public int IndexAccessorsCount { get { return IndexAccessors.Length; } }
		public abstract TOwner GetOwner();
		protected abstract MultiIndexBatchUpdateHelper CreateBatchUpdateHelper();
		protected abstract MultiIndexBatchUpdateHelper CreateBatchInitHelper();
		public abstract TActions GetBatchUpdateChangeActions();
		protected internal abstract void ApplyChanges(TActions actions);
	}
	#endregion
	#region MultiIndexBatchUpdateHelper
	public class MultiIndexBatchUpdateHelper : BatchUpdateHelper {
		HistoryTransaction transaction;
		int suppressIndexRecalculationOnEndInitCount;
		bool fakeAssignDetected;
		public MultiIndexBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		internal HistoryTransaction Transaction { get { return transaction; } set { transaction = value; } }
		public bool FakeAssignDetected { get { return fakeAssignDetected; } set { fakeAssignDetected = value; } }
		public bool IsIndexRecalculationOnEndInitEnabled { get { return suppressIndexRecalculationOnEndInitCount == 0; } }
		public void SuppressIndexRecalculationOnEndInit() {
			suppressIndexRecalculationOnEndInitCount++;
		}
		public void ResumeIndexRecalculationOnEndInit() {
			suppressIndexRecalculationOnEndInitCount--;
		}
		public virtual void BeginUpdateDeferredChanges() {
		}
		public virtual void CancelUpdateDeferredChanges() {
		}
		public virtual void EndUpdateDeferredChanges() {
		}
	}
	#endregion
	#region IIndexAccessorBase<TOwner, TActions>
	public interface IIndexAccessorBase<TOwner, TActions>
		where TOwner : MultiIndexObject<TOwner, TActions>
		where TActions : struct {
		int GetIndex(TOwner owner);
		void SetIndex(TOwner owner, int value);
		bool IsIndexValid(TOwner owner, int index);
		int GetDeferredInfoIndex(TOwner owner);
		void InitializeDeferredInfo(TOwner owner);
		void CopyDeferredInfo(TOwner owner, TOwner from);
		bool ApplyDeferredChanges(TOwner owner);
		IndexChangedHistoryItemCore<TActions> CreateHistoryItem(TOwner owner);
	}
	#endregion
	#region IIndexAccessor
	[SuppressMessage("Microsoft.Design", "CA1005")]
	public interface IIndexAccessor<TOwner, TInfo, TActions> : IIndexAccessorBase<TOwner, TActions>
		where TOwner : MultiIndexObject<TOwner, TActions>
		where TInfo : ICloneable<TInfo>, ISupportsSizeOf
		where TActions : struct {
		int GetInfoIndex(TOwner owner, TInfo value);
		TInfo GetInfo(TOwner owner);
		TInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper);
		void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, TInfo info);
	}
	#endregion
}
