#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Editors {
	public abstract class ViewItem : IDisposable {
		private Boolean isDisposed;
		private String id;
		private Object control;
		private Object currentObject;
		private CompositeView view;
		private ITypeInfo objectTypeInfo;
		protected Boolean IsDisposed {
			get { return isDisposed; }
		}
		protected void CheckIsDisposed() {
			if(isDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
		protected abstract object CreateControlCore();
		protected virtual void SaveModelCore() { }
		protected virtual void SetTestTag() { }
		protected virtual void OnControlCreating() {
			if(ControlCreating != null) {
				ControlCreating(this, EventArgs.Empty);
			}
		}
		protected virtual void OnControlCreated() {
			if(ControlCreated != null) {
				ControlCreated(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCurrentObjectChanging() {
			if(CurrentObjectChanging != null) {
				CurrentObjectChanging(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCurrentObjectChanged() {
			if(CurrentObjectChanged != null) {
				CurrentObjectChanged(this, EventArgs.Empty);
			}
		}
		protected internal virtual void UpdateErrorMessage(ErrorMessages errorMessages) {
		}
		protected void ResetControl() {
			control = null;
		}
		protected virtual void Dispose(bool disposing) { }
		public ViewItem(Type objectType, string id) {
			this.ObjectType = objectType;
			this.id = id;
		}
		public void Dispose() {
			if(isDisposed) {
				return;
			}
			isDisposed = true;
			SafeExecutor executor = new SafeExecutor(this);
			executor.Execute(delegate() {
				Dispose(true);
			});
			IDisposable disposable = control as IDisposable;
			BreakLinksToControl(true);
			if(disposable != null) {
				executor.Dispose(disposable);
			}
			CurrentObjectChanging = null;
			CurrentObjectChanged = null;
			ControlCreating = null;
			ControlCreated = null;
			currentObject = null;
			control = null;
			view = null;
			executor.ThrowExceptionIfAny();
		}
		public void CreateControl() {
			CheckIsDisposed();
			OnControlCreating();
			control = CreateControlCore();
			SetTestTag();
			OnControlCreated();
		}
		public virtual void BreakLinksToControl(bool unwireEventsOnly) {
			if(!unwireEventsOnly) {
				if(control is IDisposable) {
					IDisposable disposable = (IDisposable)control;
					disposable.Dispose();
				}
				ResetControl();
			}
		}
		public void SaveModel() {
			CheckIsDisposed();
			SaveModelCore();
		}
		public virtual void RefreshDataSource() {
			CheckIsDisposed();
		}
		public virtual void Refresh() {
			CheckIsDisposed();
		}
		public void Refresh(Boolean refreshDataSource) {
			if(refreshDataSource) {
				RefreshDataSource();
			}
			Refresh();
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ViewItemId")]
#endif
		public virtual string Id {
			get { return id; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ViewItemControl")]
#endif
		public Object Control {
			get { return control; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ViewItemCaption")]
#endif
		public virtual string Caption {
			get { return ""; }
			set { }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ViewItemIsCaptionVisible")]
#endif
		public virtual bool IsCaptionVisible {
			get { return false; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ViewItemObjectType")]
#endif
		public Type ObjectType {
			get { return (objectTypeInfo != null) ? objectTypeInfo.Type : null; }
			set {
				CheckIsDisposed();
				objectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(value);
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ViewItemObjectTypeInfo")]
#endif
		public ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
			set { objectTypeInfo = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ViewItemCurrentObject")]
#endif
		public object CurrentObject {
			get { return currentObject; }
			set {
				CheckIsDisposed();
				if(currentObject != value) {
					OnCurrentObjectChanging();
					currentObject = value;
					if((currentObject != null) && (ObjectTypeInfo == null)) {
						ObjectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(currentObject.GetType());
					}
					OnCurrentObjectChanged();
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ViewItemView")]
#endif
		public CompositeView View {
			get { return view; }
			set { view = value; }
		}
		public event EventHandler<EventArgs> CurrentObjectChanging;
		public event EventHandler<EventArgs> CurrentObjectChanged;
		public event EventHandler<EventArgs> ControlCreating;
		public event EventHandler<EventArgs> ControlCreated;
	}
}
