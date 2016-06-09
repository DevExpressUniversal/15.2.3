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
using System.ComponentModel.Design;
namespace DevExpress.XtraBars.Docking2010.Base {
	[ToolboxItem(false)]
	public class BaseComponent : Component, DevExpress.Utils.Base.IBaseObject, ISupportInitialize, DevExpress.Utils.Base.ISupportBatchUpdate {
		protected BaseComponent(IContainer container) {
			if(container != null)
				container.Add(this);
			OnCreate();
		}
		protected sealed override void Dispose(bool disposing) {
			if(disposing) {
				if(!isDisposingCore) {
					LockComponentBeforeDisposing();
					try {
						isDisposingCore = true;
						OnDispose();
					}
					finally { AfterComponentDisposing(); }
				}
			}
			base.Dispose(disposing);
		}
		bool isDisposingCore;
		[Browsable(false)]
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected virtual void OnCreate() { }
		protected virtual void LockComponentBeforeDisposing() { }
		protected virtual void AfterComponentDisposing() { }
		protected virtual void OnDispose() { }
		protected virtual void OnInitialized() { }
		protected virtual void OnLayoutChanged() { }
		#region ISupportInitialize Members
		int initializing = 0;
		int initialized = 0;
		[Browsable(false)]
		public bool IsInitializing {
			get { return initializing > 0; }
		}
		[Browsable(false)]
		public bool IsInitialized {
			get { return initializing == 0 && initialized > 0; }
		}
		void ISupportInitialize.BeginInit() {
			initializing++;
		}
		void ISupportInitialize.EndInit() {
			if(--initializing == 0) {
				InitializeCore();
				LayoutChanged();
			}
		}
		protected void ResetInitialization() {
			initialized = 0;
		}
		protected void InitializeCore() {
			if(0 == initialized++)
				OnInitialized();
		}
		#endregion
		int lockUpdateCounter;
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
		protected virtual bool IsLayoutChangeRestricted {
			get { return IsUpdateLocked || IsInitializing || IsDisposing; }
		}
		protected void OnUnlockUpdate() {
			LayoutChanged();
		}
		protected void SetValue<T>(ref T field, T value) {
			if(object.Equals(field, value)) return;
			field = value;
			LayoutChanged();
		}
		protected void SetValue<T>(ref T field, T value, System.Action valueChangedCallback) {
			if(object.Equals(field, value)) return;
			field = value;
			if(valueChangedCallback != null)
				valueChangedCallback();
		}
		protected void SetValue<T>(ref T field, T value, System.Action<T, T> valueChangedCallback) {
			if(object.Equals(field, value)) return;
			T prevValue = field;
			field = value;
			if(valueChangedCallback != null)
				valueChangedCallback(prevValue, value);
		}
		IComponentLayoutChangedTracker changedTracker;
		protected bool IsLayoutChangedTracking {
			get { return changedTracker != null; }
		}
		public void LayoutChanged() {
			if(IsLayoutChangeRestricted) return;
			if(changedTracker != null) {
				changedTracker.QueueLayoutChanged();
				return;
			}
			using(BatchUpdate.Enter(this, true)) {
				if(lockChanging == 0) FireChanging();
				OnLayoutChanged();
				if(lockChanging == 0) FireChanged();
			}
		}
		#region IComponentLayoutChangedTracker
		protected internal IComponentLayoutChangedTracker TrackLayoutChanged() {
			return changedTracker ?? new ComponentLayoutChangedTracker(this);
		}
		class ComponentLayoutChangedTracker : IComponentLayoutChangedTracker {
			BaseComponent component;
			int trackCounter;
			int queuedLayoutChangedCount;
			public ComponentLayoutChangedTracker(BaseComponent component) {
				this.component = component;
				if(component != null) {
					component.changedTracker = this;
					trackCounter++;
				}
			}
			public void QueueLayoutChanged() {
				queuedLayoutChangedCount++;
			}
			void System.IDisposable.Dispose() {
				if(component != null) {
					if(--trackCounter == 0) {
						component.changedTracker = null;
						if(queuedLayoutChangedCount > 0)
							component.LayoutChanged();
					}
				}
				this.component = null;
			}
		}
		#endregion IComponentLayoutChangedTracker
		#region IComponentChangeService
		int lockChanging = 0;
		protected System.IDisposable LockComponentChangeNotifications() {
			return new ComponentChangeLocker(this);
		}
		protected void FireChanging() {
			IComponentChangeService service = GetComponentChangeService();
			if(service != null)
				service.OnComponentChanging(this, null);
		}
		protected void FireChanged() {
			IComponentChangeService service = GetComponentChangeService();
			if(service != null)
				service.OnComponentChanged(this, null, null, null);
		}
		protected IComponentChangeService GetComponentChangeService() {
			return GetService<IComponentChangeService>();
		}
		protected IDesignerHost GetDesignerHostService() {
			return GetService<IDesignerHost>();
		}
		protected TService GetService<TService>() where TService : class {
			return GetService(typeof(TService)) as TService;
		}
		class ComponentChangeLocker : System.IDisposable {
			BaseComponent component;
			public ComponentChangeLocker(BaseComponent component) {
				this.component = component;
				if(component != null)
					component.lockChanging++;
			}
			void System.IDisposable.Dispose() {
				if(component != null)
					component.lockChanging--;
				this.component = null;
			}
		}
		#endregion IComponentChangeService
	}
}
