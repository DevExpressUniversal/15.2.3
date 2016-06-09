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

using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap {
	public class SupportCallbackCollection<T> : DXCollection<T>, IChangedCallbackOwner, IBatchUpdateable, IBatchUpdateHandler {
		Action callback;
		BatchUpdateHelper batchUpdateHelper;
		bool needRaiseCallback = false;
		protected internal Action Callback { get { return callback; } }
		protected SupportCallbackCollection() : this(DXCollectionUniquenessProviderType.MinimizeMemoryUsage) {
		}
		protected SupportCallbackCollection(DXCollectionUniquenessProviderType type)
			: base(type) {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region IChangedCallbackOwner members
		void IChangedCallbackOwner.SetParentCallback(Action callback) {
			this.callback = callback;
		}
		#endregion
		#region IBatchUpdateable members
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		#endregion
		#region IBatchUpdateHandler members
		void IBatchUpdateHandler.OnBeginUpdate() {
			OnBeginUpdate();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
			OnCancelUpdate();
		}
		void IBatchUpdateHandler.OnEndUpdate() {
			OnEndUpdate();
		}
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdate();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdate();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdate();
		}
		#endregion
		void SetAndRaiseExistingObjectsCallbacks() {
			foreach(T obj in InnerList) {
				SetObjectCallback(obj, ItemCallback);
				ItemCallback();
			}
		}
		void ResetExistingObjectsCallbacks() {
			foreach(T obj in InnerList)
				SetObjectCallback(obj, null);
		}
		void SetObjectCallback(T obj, Action callback) {
			IChangedCallbackOwner owner = obj as IChangedCallbackOwner;
			if (owner != null) owner.SetParentCallback(callback);
		}
		void RaiseCallback(Action callback) {
			if(callback == null) return;
			if(!IsUpdateLocked)
				callback();   
		}
		protected virtual void OnBeginUpdate() {
		}
		protected virtual void OnCancelUpdate() {
		}
		protected virtual void OnEndUpdate() {
		}
		protected virtual void OnFirstBeginUpdate() {
		}
		protected virtual void OnLastCancelUpdate() {
			needRaiseCallback = false;
		}
		protected virtual void OnLastEndUpdate() {
			RaiseCallback(callback);
			if(needRaiseCallback) {
				RaiseCallback(ItemCallbackCore);
				needRaiseCallback = false;
			}
		}
		protected virtual void ItemCallbackCore() {
		}
		protected override void OnInsertComplete(int index, T value) {
			base.OnInsertComplete(index, value);
			SetObjectCallback(value, ItemCallback);
			RaiseCallback(callback);
		}
		protected override void OnRemoveComplete(int index, T value) {
			base.OnRemoveComplete(index, value);
			SetObjectCallback(value, null);
			RaiseCallback(callback);
		}
		protected override bool OnClear() {
			ResetExistingObjectsCallbacks();
			return base.OnClear();
		}
		protected override void OnClearComplete() {
			RaiseCallback(callback);
			base.OnClearComplete();
		}
		protected internal void ItemCallback() {
			if(!IsUpdateLocked)
				ItemCallbackCore();
			else
				needRaiseCallback = true;
		}
		public override void AddRange(ICollection collection) {
			if(collection == null || collection.Count <= 0)
				return;
			BeginUpdate();
			try {
				AddRangeCore(collection);
			} finally {
				EndUpdate();
			}
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public interface IChangedCallbackOwner {
		void SetParentCallback(Action callback);
	}
}
