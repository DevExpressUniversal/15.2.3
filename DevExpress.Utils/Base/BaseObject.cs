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
using System.ComponentModel;
namespace DevExpress.Utils.Base {
	public class BaseObject : IBaseObject, ISupportBatchUpdate {
		int lockUpdateCounter = 0;
		bool isDisposingCore = false;
		public event EventHandler Changed;
		public event EventHandler Disposed;
		protected BaseObject() {
			OnCreate();
		}
		public void Dispose() {
			if(!IsDisposing) {
				isDisposingCore = true;
				BeginUpdate();
				OnDispose();
				CancelUpdate();
				RaiseDisposed(EventArgs.Empty);
				Changed = null;
				Disposed = null;
				GC.SuppressFinalize(this);
			}
		}
		protected virtual void OnCreate() { }
		protected virtual void OnDispose() { }
		[Browsable(false)]
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		public void BeginUpdate() {
			lockUpdateCounter++;
		}
		public void EndUpdate() {
			if(--lockUpdateCounter == 0) OnUnlockUpdate();
		}
		public void CancelUpdate() {
			lockUpdateCounter--;
		}
		[Browsable(false)]
		public bool IsUpdateLocked {
			get { return lockUpdateCounter > 0; }
		}
		protected void OnUnlockUpdate() {
			OnObjectChanged();
		}
		protected void OnObjectChanged() {
			OnObjectChanged(EventArgs.Empty);
		}
		protected void OnObjectChanged(string propertyName) {
			OnObjectChanged(new PropertyChangedEventArgs(propertyName));
		}
		protected void OnObjectChanged(EventArgs e) {
			if(IsUpdateLocked || IsDisposing) return;
			BeginUpdate();
			OnUpdateObjectCore();
			RaiseChanged(e);
			CancelUpdate();
		}
		protected void SetValue<T>(ref T field, T value, string propertyName) {
			if(object.Equals(field, value)) return;
			field = value;
			OnObjectChanged(propertyName);
		}
		protected virtual void OnUpdateObjectCore() { }
		protected void RaiseChanged(EventArgs e) {
			if(Changed != null) Changed(this, e);
		}
		protected void RaiseDisposed(EventArgs e) {
			if(Disposed != null) Disposed(this, e);
		}
	}
}
