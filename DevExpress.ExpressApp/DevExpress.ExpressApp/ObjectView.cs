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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System.ComponentModel;
namespace DevExpress.ExpressApp {
	public abstract class ObjectView : CompositeView {
		public const string InfoAllowDelete = "Info.AllowDelete";
		public const string InfoAllowEdit = "Info.AllowEdit";
		public const string InfoAllowNew = "Info.AllowNew";
		private ITypeInfo objectTypeInfo;
		private Int32 editorsRefreshingDisableCount;
		private void UnsubscribeFromObjectSpace() {
			if(ObjectSpace != null) {
				ObjectSpace.Reloaded -= new EventHandler(objectSpace_Reloaded);
				ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
			}
		}
		protected void DisableEditorsRefreshing() {
			editorsRefreshingDisableCount++;
		}
		protected void EnableEditorsRefreshing() {
			editorsRefreshingDisableCount--;
			if(editorsRefreshingDisableCount < 0) {
				editorsRefreshingDisableCount = 0;
			}
		}
		protected void RefreshViewItemByPropertyName(String propertyName) {
			foreach(ViewItem item in Items) {
				PropertyEditor editor = item as PropertyEditor;
				if((editor != null) && (editor.PropertyName == propertyName)) {
					editor.Refresh();
				}
			}
		}
		private void objectSpace_ObjectChanged(Object sender, ObjectChangedEventArgs e) {
			if(editorsRefreshingDisableCount == 0) {
				DisableEditorsRefreshing();
				try {
					if(e.Object == CurrentObject) {
						RefreshViewItemByPropertyName(e.PropertyName);
					}
					else {
						RefreshViewItemByChangedObject(e.Object);
					}
				}
				finally {
					EnableEditorsRefreshing();
				}
			}
		}
		private void RefreshViewItemByChangedObject(Object changedObject) {
			foreach(ViewItem item in Items) {
				if(NeedToRefreshViewItem(item, changedObject)) {
					item.Refresh();
				}
			}
		}
		private Boolean NeedToRefreshViewItem(ViewItem item, Object changedObject) {
			PropertyEditor propertyEditor = item as PropertyEditor;
			Boolean result = (propertyEditor != null && propertyEditor.Control != null && propertyEditor.ControlValue == changedObject);
			if(result) {
				DetailPropertyEditor detailPropertyEditor = item as DetailPropertyEditor;
				if(detailPropertyEditor != null && detailPropertyEditor.DetailView != null) {
					result = (detailPropertyEditor.DetailView.editorsRefreshingDisableCount == 0);
				}
			}
			return result;
		}
		private void SetObjectType(ITypeInfo objectTypeInfo) {
			this.objectTypeInfo = objectTypeInfo;
		}
		private void objectSpace_Reloaded(Object sender, EventArgs e) {
			if(ObjectSpace != null) {
				OnObjectSpaceReloaded();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal String keyMemberValueForPendingLoading;
		protected internal virtual String GetObjectCaptionFormat() {
			if(Model != null) {
				return Model.ModelClass.ObjectCaptionFormat;
			}
			else {
				return "";
			}
		}
		protected override void InitializeItem(ViewItem item) {
			item.CurrentObject = CurrentObject;
			base.InitializeItem(item);
		}
		protected override ViewItem CreateItem(IModelViewItem info) {
			if(Application != null) {
				Type objType = (ObjectTypeInfo != null) ? ObjectTypeInfo.Type : null;
				Boolean needProtectedContentEditor = false;
				if((objType != null) && (info is IModelPropertyEditor)) {
					needProtectedContentEditor = !DataManipulationRight.CanRead(objType, ((IModelPropertyEditor)info).PropertyName, CurrentObject, null, ObjectSpace); 
				}
				return Application.EditorFactory.CreateDetailViewEditor(needProtectedContentEditor, info, objType, Application, ObjectSpace);
			}
			else {
				return null;
			}
		}
		protected override void UpdateSecurityModifiers() {
			if(objectTypeInfo != null && !(SecuritySystem.Instance is DevExpress.ExpressApp.Security.IRequestSecurity)) {
				AllowEdit.SetItemValue(SecurityReadOnlyItemName, DataManipulationRight.CanEdit(ObjectTypeInfo.Type, "", null, null, ObjectSpace));
			}
			else {
				AllowEdit.RemoveItem(SecurityReadOnlyItemName);
			}
		}
		protected abstract void OnObjectSpaceReloaded();
		protected override void DisposeCore()
		{
			UnsubscribeFromObjectSpace();
			objectTypeInfo = null;
			base.DisposeCore();
		}
		protected override void OnCurrentObjectChanged() {
			keyMemberValueForPendingLoading = null;
			base.OnCurrentObjectChanged();
		}
		protected override ViewShortcut CreateShortcutCore() {
			Guard.ArgumentNotNull(ObjectTypeInfo, "ObjectTypeInfo");
			if(IsDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			String keyValue = null;
			if((CurrentObject != null) && (ObjectSpace != null)) {
				keyValue = ObjectSpace.GetKeyValueAsString(CurrentObject);
			}
			else if(!String.IsNullOrEmpty(keyMemberValueForPendingLoading)) {
				keyValue = keyMemberValueForPendingLoading;
			}
			return new ViewShortcut(ObjectTypeInfo.Type, keyValue, Id, ScrollPosition);
		}
		protected override void LoadModelCore() {
			if(Model != null) {
				Guard.ArgumentNotNull(Model.ModelClass, "Model.ModelClass");
				ITypeInfo infoObjectType = Model.ModelClass.TypeInfo;
				if(infoObjectType != null) {
					SetObjectType(infoObjectType);
				}
				AllowDelete.SetItemValue(InfoAllowDelete, Model.AllowDelete);
				AllowEdit.SetItemValue(InfoAllowEdit, Model.AllowEdit);
				AllowNew.SetItemValue(InfoAllowNew, Model.AllowNew);
			}
			else {
				SetObjectType(null);
				AllowDelete.RemoveItem(InfoAllowDelete);
				AllowEdit.RemoveItem(InfoAllowEdit);
				AllowNew.RemoveItem(InfoAllowNew);
			}
			base.LoadModelCore();
		}
		protected override void CloseCore() {
			base.CloseCore();
			UnsubscribeFromObjectSpace();
		}
		protected ObjectView(XafApplication application, IObjectSpace objectSpace, Boolean isRoot)
			: base(objectSpace, application, isRoot) {
			if(objectSpace != null) {
				objectSpace.Reloaded += new EventHandler(objectSpace_Reloaded);
				objectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
			}
		}
		public override Boolean IsSameObjectSpace(View view) {
			ObjectView oView = view as ObjectView;
			return oView != null && oView.ObjectSpace == this.ObjectSpace;
		}
		public override String GetCurrentObjectCaption() {
			String result = null;
			if((CurrentObject != null) && (Model != null) && !ObjectSpace.IsDisposedObject(CurrentObject) && !ObjectSpace.IsDeletedObject(CurrentObject)) {
				result = String.Format(new ObjectFormatter(), GetObjectCaptionFormat(), CurrentObject).Replace("\r\n", " ");
			}
			return result;
		}
		public override ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
			set {
				objectTypeInfo = value;
				UpdateSecurityModifiers();
			}
		}
		public new IModelObjectView Model {
			get { return base.Model as IModelObjectView; }
		}
	}
}
