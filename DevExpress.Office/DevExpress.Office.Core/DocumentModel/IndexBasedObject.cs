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
using DevExpress.Office.Utils;
using DevExpress.Office.Localization;
using System.Diagnostics;
namespace DevExpress.Office {
	#region UndoableIndexBasedObject<T>
	public abstract class UndoableIndexBasedObject<T, TActions> : IIndexBasedObject<TActions>, IBatchUpdateable, IBatchUpdateHandler, IBatchInit, IBatchInitHandler, ISupportsSizeOf
		where T : ICloneable<T>, ISupportsCopyFrom<T>, ISupportsSizeOf
		where TActions : struct {
		#region Fields
		readonly IDocumentModelPart documentModelPart;
		int index;
		BatchUpdateHelper<T> batchUpdateHelper;
		#endregion
		protected UndoableIndexBasedObject(IDocumentModelPart documentModelPart) {
			Guard.ArgumentNotNull(documentModelPart, "documentModelPart");
			this.documentModelPart = documentModelPart;
		}
		#region Properties
		public T Info { get { return IsUpdateLocked ? batchUpdateHelper.DeferredNotifications : InfoCore; } }
		protected T DeferredInfo { get { return batchUpdateHelper.DeferredNotifications; } }
		public IDocumentModelPart DocumentModelPart { get { return documentModelPart; } }
		public IDocumentModel DocumentModel { get { return documentModelPart.DocumentModel; } }
		public T InfoCore { get { return GetCache(DocumentModel)[Index]; } }
		protected internal abstract UniqueItemsCache<T> GetCache(IDocumentModel documentModel);
		public int Index { get { return index; } }
		public void SuppressDirectNotifications() {
			System.Diagnostics.Debug.Assert(IsUpdateLocked);
			batchUpdateHelper.SuppressDirectNotifications();
		}
		protected internal void ResumeDirectNotifications() {
			System.Diagnostics.Debug.Assert(IsUpdateLocked);
			batchUpdateHelper.ResumeDirectNotifications();
		}
		protected internal bool IsDirectNotificationsEnabled { get { return !IsUpdateLocked || batchUpdateHelper.IsDirectNotificationsEnabled; } }
		public void SuppressIndexRecalculationOnEndInit() {
			System.Diagnostics.Debug.Assert(IsUpdateLocked);
			batchUpdateHelper.SuppressIndexRecalculationOnEndInit();
		}
		protected internal void ResumeIndexRecalculationOnEndInit() {
			System.Diagnostics.Debug.Assert(IsUpdateLocked);
			batchUpdateHelper.ResumeIndexRecalculationOnEndInit();
		}
		protected internal bool IsIndexRecalculationOnEndInitEnabled { get { return !IsUpdateLocked || batchUpdateHelper.IsIndexRecalculationOnEndInitEnabled; } }
		#endregion
		public void SetIndexInitial(int value) {
			this.index = value;
			System.Diagnostics.Debug.Assert(index < GetCache(DocumentModel).Count);
		}
		#region IIndexBasedObject Members
		int IIndexBasedObject<TActions>.GetIndex() {
			return index;
		}
		void IIndexBasedObject<TActions>.SetIndex(int value, TActions changeActions) {
			if (index == value)
				return;
			OnIndexChanging();
			index = value;
			System.Diagnostics.Debug.Assert(index < GetCache(DocumentModel).Count);
			ApplyChanges(changeActions);
			OnIndexChanged();
		}
		protected internal abstract void ApplyChanges(TActions changeActions);
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			if (batchUpdateHelper == null)
				batchUpdateHelper = new BatchUpdateHelper<T>(this);
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			if (!(batchUpdateHelper is BatchUpdateHelper<T>))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndUpdate);
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper != null && batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchInit implementation
		public void BeginInit() {
			if (batchUpdateHelper == null)
				batchUpdateHelper = new BatchInitHelper<T>(this);
			if (!(batchUpdateHelper is BatchInitHelper<T>))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidBeginInit);
			batchUpdateHelper.BeginUpdate();
		}
		public void EndInit() {
			if (!(batchUpdateHelper is BatchInitHelper<T>))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndInit);
			batchUpdateHelper.EndUpdate();
		}
		public void CancelInit() {
			if (!(batchUpdateHelper is BatchInitHelper<T>))
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
		protected internal virtual void OnFirstBeginUpdateCore() {
			batchUpdateHelper.DeferredNotifications = InfoCore.Clone();
		}
		protected internal virtual void OnLastCancelUpdateCore() {
			if (!(batchUpdateHelper is BatchUpdateHelper<T>))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndUpdate);
			batchUpdateHelper = null;
		}
		protected internal virtual void OnLastEndUpdateCore() {
			if (!(batchUpdateHelper is BatchUpdateHelper<T>))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndUpdate);
			if (!ReplaceInfo(batchUpdateHelper.DeferredNotifications, GetBatchUpdateChangeActions())) {
				if (batchUpdateHelper.FakeAssignDetected)
					NotifyFakeAssign();
			}
			batchUpdateHelper = null;
		}
		#endregion
		#region IBatchInitHandler implementation
		void IBatchInitHandler.OnFirstBeginInit() {
			batchUpdateHelper.DeferredNotifications = InfoCore.Clone();
		}
		void IBatchInitHandler.OnBeginInit() {
		}
		void IBatchInitHandler.OnEndInit() {
		}
		void IBatchInitHandler.OnCancelInit() {
		}
		void IBatchInitHandler.OnLastEndInit() {
			if (batchUpdateHelper.IsIndexRecalculationOnEndInitEnabled)
				this.index = GetInfoIndex(batchUpdateHelper.DeferredNotifications);
			System.Diagnostics.Debug.Assert(index < GetCache(DocumentModel).Count);
			batchUpdateHelper = null;
		}
		void IBatchInitHandler.OnLastCancelInit() {
			if (batchUpdateHelper.IsIndexRecalculationOnEndInitEnabled)
				this.index = GetInfoIndex(batchUpdateHelper.DeferredNotifications);
			System.Diagnostics.Debug.Assert(index < GetCache(DocumentModel).Count);
			batchUpdateHelper = null;
		}
		#endregion
		protected internal delegate TActions SetPropertyValueDelegate<U>(T info, U newValue);
		protected internal virtual void SetPropertyValue<U>(SetPropertyValueDelegate<U> setter, U newValue) {
			T info = GetInfoForModification();
			TActions changeActions = setter(info, newValue);
			ReplaceInfo(info, changeActions);
		}
		public T GetInfoForModification() {
			if (IsUpdateLocked)
				return batchUpdateHelper.DeferredNotifications;
			else
				return Info.Clone();
		}
		public bool ReplaceInfo(T newValue, TActions changeActions) {
			if (IsUpdateLocked) {
				return false;
			}
			int newIndex = GetInfoIndex(newValue);
			return ChangeIndex(newIndex, changeActions);
		}
		bool ChangeIndex(int newIndex, TActions changeActions) {
			IDocumentModel documentModel = DocumentModel;
			System.Diagnostics.Debug.Assert(newIndex < GetCache(documentModel).Count);
			if (newIndex != Index) {
				ChangeIndexCore(newIndex, changeActions);
				return true;
			}
			else
				return false;
		}
		public virtual void ChangeIndexCore(int newIndex, TActions changeActions) {
			IDocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				OnBeginAssign();
				try {
					IndexChangedHistoryItemCore<TActions> item = CreateIndexChangedHistoryItem();
					System.Diagnostics.Debug.Assert(item != null);
					item.OldIndex = this.index;
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
		protected virtual IndexChangedHistoryItemCore<TActions> CreateIndexChangedHistoryItem() {
			return new IndexChangedHistoryItem<TActions>(DocumentModelPart, this);
		}
		public void CopyFromCore(UndoableIndexBasedObject<T, TActions> obj) {
			if (this.Index != obj.Index)
				((IIndexBasedObject<TActions>)this).SetIndex(obj.Index, GetBatchUpdateChangeActions());
		}
		protected internal int GetInfoIndex(T value) {
			return GetCache(DocumentModel).GetItemIndex(value);
		}
		public virtual void CopyFrom(UndoableIndexBasedObject<T, TActions> obj) {
			if (obj.DocumentModel != DocumentModel)
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidCopyFromDocumentModel);
			if (this.Index != obj.Index)
				ChangeIndex(obj.Index, GetBatchUpdateChangeActions());
			else
				NotifyFakeAssign();
		}
		public void CopyFrom(T newInfo) {
			T info = GetInfoForModification();
			info.CopyFrom(newInfo);
			if (!ReplaceInfo(info, GetBatchUpdateChangeActions()))
				NotifyFakeAssign();
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
		protected internal virtual void OnIndexChanging() {
		}
		protected internal virtual void OnIndexChanged() {
		}
		public abstract TActions GetBatchUpdateChangeActions();
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region IndexBasedObject<TInfo, TOptions> (abstract class)
	public abstract class IndexBasedObject<TInfo, TOptions> : IBatchUpdateable, IBatchUpdateHandler, ISupportsSizeOf
		where TInfo : ICloneable<TInfo>, ISupportsCopyFrom<TInfo>, ISupportsSizeOf
		where TOptions : ICloneable<TOptions>, ISupportsCopyFrom<TOptions>, ISupportsSizeOf {
		#region Fields
		readonly IDocumentModel documentModel;
		BatchUpdateHelper<TInfo, TOptions> batchUpdateHelper;
		int infoIndex;
		int optionsIndex;
		#endregion
		protected IndexBasedObject(IDocumentModel documentModel, int formattingInfoIndex, int formattingOptionsIndex) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			if (!InfoCache.IsIndexValid(formattingInfoIndex))
				Exceptions.ThrowArgumentException("formattingInfoIndex", formattingInfoIndex);
			if (!OptionsCache.IsIndexValid(formattingOptionsIndex))
				Exceptions.ThrowArgumentException("formattingOptionsIndex", formattingOptionsIndex);
			this.infoIndex = formattingInfoIndex;
			this.optionsIndex = formattingOptionsIndex;
		}
		#region Properties
		public TInfo Info { get { return IsUpdateLocked ? batchUpdateHelper.DeferredInfoNotifications : InfoCore; } }
		public TOptions Options { get { return IsUpdateLocked ? batchUpdateHelper.DeferredOptionsNotifications : OptionsCore; } }
		public IDocumentModel DocumentModel { get { return documentModel; } }
		protected internal TInfo InfoCore { get { return InfoCache[InfoIndex]; } }
		protected internal TOptions OptionsCore { get { return OptionsCache[OptionsIndex]; } }
		protected internal abstract UniqueItemsCache<TInfo> InfoCache { get; }
		protected internal abstract UniqueItemsCache<TOptions> OptionsCache { get; }
		public int InfoIndex { get { return infoIndex; } set { infoIndex = value; } }
		public int OptionsIndex { get { return optionsIndex; } set { optionsIndex = value; } }
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			if (batchUpdateHelper == null)
				batchUpdateHelper = new BatchUpdateHelper<TInfo, TOptions>(this);
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper != null && batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			batchUpdateHelper.DeferredInfoNotifications = InfoCore.Clone();
			batchUpdateHelper.DeferredOptionsNotifications = OptionsCore.Clone();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			ReplaceInfo(batchUpdateHelper.DeferredInfoNotifications, batchUpdateHelper.DeferredOptionsNotifications);
			batchUpdateHelper = null;
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			batchUpdateHelper = null;
		}
		#endregion
		protected internal delegate void SetPropertyValueDelegate<U>(TInfo info, U newValue);
		protected internal delegate void SetOptionsValueDelegate(TOptions options, bool newValue);
		protected internal virtual void SetPropertyValue<U>(SetPropertyValueDelegate<U> setter, U newValue, SetOptionsValueDelegate optionsSetter) {
			TInfo info = GetInfoForModification();
			TOptions options = GetOptionsForModification();
			setter(info, newValue);
			optionsSetter(options, true);
			ReplaceInfo(info, options);
		}
		public void SetIndexInitial(int infoIndex, int optionsIndex) {
			this.infoIndex = infoIndex;
			this.optionsIndex = optionsIndex;
			System.Diagnostics.Debug.Assert(infoIndex < InfoCache.Count);
			System.Diagnostics.Debug.Assert(optionsIndex < OptionsCache.Count);
		}
		public TInfo GetInfoForModification() {
			if (IsUpdateLocked)
				return batchUpdateHelper.DeferredInfoNotifications;
			else
				return Info.Clone();
		}
		public TOptions GetOptionsForModification() {
			if (IsUpdateLocked)
				return batchUpdateHelper.DeferredOptionsNotifications;
			else
				return Options.Clone();
		}
		public void ReplaceInfo(TInfo newInfo, TOptions newOptions) {
			if (IsUpdateLocked)
				return;
			ReplaceInfoCore(GetInfoIndex(newInfo), GetOptionsIndex(newOptions));
		}
		protected void ReplaceInfoCore(int newInfoIndex, int newOptionsIndex) {
			this.optionsIndex = newOptionsIndex;
			this.infoIndex = newInfoIndex;
		}
		protected internal int GetInfoIndex(TInfo value) {
			return InfoCache.GetItemIndex(value);
		}
		protected internal int GetOptionsIndex(TOptions value) {
			return OptionsCache.GetItemIndex(value);
		}
		protected internal void OnChanging() {
		}
		protected internal void OnChanged() {
		}
		public override bool Equals(object obj) {
			IndexBasedObject<TInfo, TOptions> other = obj as IndexBasedObject<TInfo, TOptions>;
			if (other == null)
				return false;
			if (Object.ReferenceEquals(other.DocumentModel, this.DocumentModel))
				return InfoIndex == other.InfoIndex && OptionsIndex == other.OptionsIndex;
			else
				return PropertyEquals(other);
		}
		protected abstract bool PropertyEquals(IndexBasedObject<TInfo, TOptions> other);
		public override int GetHashCode() {
			return InfoIndex ^ OptionsIndex;
		}
		public void CopyFrom(IndexBasedObject<TInfo, TOptions> obj) {
			CopyFromCore(obj.Info, obj.Options);
		}
		public void CopyFromCore(TInfo newInfo, TOptions newOptions) {
			TInfo info = GetInfoForModification();
			TOptions options = GetOptionsForModification();
			info.CopyFrom(newInfo);
			options.CopyFrom(newOptions);
			ReplaceInfo(info, options);
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
}
