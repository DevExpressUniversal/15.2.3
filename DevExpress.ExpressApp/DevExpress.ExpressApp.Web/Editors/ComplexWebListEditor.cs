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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Core;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors {
	public class WebColumnBaseColumnWrapper : ColumnWrapper {
		private WebColumnBase column;
		public WebColumnBaseColumnWrapper(WebColumnBase column) {
			this.column = column;
		}
		public override string Caption {
			get { return column.Caption; }
			set { column.Caption = value; }
		}
		public override int VisibleIndex {
			get { return column.VisibleIndex; }
			set { column.VisibleIndex = value; }
		}
		public override bool Visible {
			get { return column.Visible; }
		}
		public override bool AllowVisibleIndexStore {
			get { return false; }
		}
		public WebColumnBase Column {
			get { return column; }
		}
	}
	public class ComplexWebListEditorOptionsModelSynchronizer : ModelSynchronizer<ComplexWebListEditor, IModelListView> {
		public ComplexWebListEditorOptionsModelSynchronizer(ComplexWebListEditor listEditor, IModelListView model)
			: base(listEditor, model) {
		}
		protected override void ApplyModelCore() {
			if(Model is IModelListViewWeb) {
				Control.CanSelectRows = ((IModelListViewWeb)Model).ShowSelectionColumn;
			}
		}
		public override void SynchronizeModel() {
		}
	}
	public class CustomCreateCellControlEventArgs : HandledEventArgs {
		private WebPropertyEditor propertyEditor;
		private String targetPropertyName;
		private Object targetObject;
		private Control cellControl;
		public CustomCreateCellControlEventArgs(WebPropertyEditor propertyEditor, String targetPropertyName, Object targetObject) {
			this.propertyEditor = propertyEditor;
			this.targetPropertyName = targetPropertyName;
			this.targetObject = targetObject;
		}
		public WebPropertyEditor PropertyEditor {
			get { return propertyEditor; }
		}
		public String TargetPropertyName {
			get { return targetPropertyName; }
		}
		public Object TargetObject {
			get { return targetObject; }
		}
		public Control CellControl {
			get { return cellControl; }
			set { cellControl = value; }
		}
	}
	public abstract class ComplexWebListEditor : ColumnsListEditor, IComplexListEditor, IDetailViewItemsHolder, ISupportInplaceEdit, IProcessCallbackComplete {
		private DataItemTemplateFactory dataItemTemplateFactory = null;
		private CollectionSourceBase collectionSource;
		private XafApplication application;
		private bool allowSelectRows = true;
		private List<IMemberInfo> keyMembersInfo;
		private List<Object> selectedObjectsCache = new List<object>();
		private Boolean isSelectedObjectsCacheActual = false;
		public ComplexWebListEditor(IModelListView info)
			: base(info) {
			dataItemTemplateFactory = CreateDataItemTemplateFactory();
			dataItemTemplateFactory.QueryProtectedContentEditorState += dataItemTemplateFactory_QueryProtectedContentEditorState;
			ModelSynchronizer optionsModelSynchronizer = new ComplexWebListEditorOptionsModelSynchronizer(this, info);
			optionsModelSynchronizer.ApplyModel();
			keyMembersInfo = new List<IMemberInfo>();
		}
		public abstract IList<object> GetControlSelectedObjects();
		public abstract void SetControlSelectedObjects(IList<object> objects);
		public override void Refresh() {
			DropSelectedObjectsCache();
		}
		public WebPropertyEditor FindPropertyEditor(string propertyName, ViewEditMode viewEditMode) {
			if(dataItemTemplateFactory != null) {
				return dataItemTemplateFactory.FindPropertyEditor(propertyName, viewEditMode);
			}
			return null;
		}
		public WebPropertyEditor FindPropertyEditor(IModelMemberViewItem modelDetailViewItem, ViewEditMode viewEditMode) {
			return FindPropertyEditor(modelDetailViewItem.Id, viewEditMode);
		}
		public override void BreakLinksToControls() {
			if(dataItemTemplateFactory != null) {
				foreach(WebPropertyEditor propertyEditor in dataItemTemplateFactory.PropertyEditors) {
					propertyEditor.BreakLinksToControl(false);
				}
			}
			if(Control != null && Control is Control) {
				((Control)Control).Load -= new EventHandler(control_Load);
				((Control)Control).Unload -= new EventHandler(control_Unload);
			}
			base.BreakLinksToControls();
		}
		public override void Dispose() {
			if(dataItemTemplateFactory != null) {
				dataItemTemplateFactory.QueryProtectedContentEditorState -= dataItemTemplateFactory_QueryProtectedContentEditorState;
				dataItemTemplateFactory.Dispose();
				dataItemTemplateFactory = null;
			}
			if(collectionSource != null) {
				collectionSource.CollectionChanged -= new EventHandler(collectionSource_CollectionChanged);
				ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
				collectionSource = null;
			}
			CustomCreateCellControl = null;
			base.Dispose();
		}
		public void SetTemporarySelectedObject(object value) {
			if(value != null) {
				if(CanSelectRows) {
					SetControlSelectedObjects(new Object[] { value });
				}
				else {
					DropSelectedObjectsCache();
					selectedObjectsCache.Add(value);
					OnSelectionChanged();
				}
			}
		}
		public void ClearTemporarySelectedObject() {
			if(CanSelectRows) {
				((ISupportSelectionOperations)this).UnselectAll();
			}
			else {
				DropSelectedObjectsCache();
			}
			OnSelectionChanged();
		}
		public virtual bool CanSelectRows {
			get { return allowSelectRows; }
			set { allowSelectRows = value; }
		}
		public override IList GetSelectedObjects() {
			FillSelectedObjectsCache();
			return selectedObjectsCache.ToArray();
		}
		internal static bool CanConvertFromToString(Type type) {
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if(underlyingType != null) {
				type = underlyingType;
			}
			return TypeDescriptor.GetConverter(type).CanConvertTo(typeof(string)) &&
				TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
		}
		protected internal XafApplication Application {
			get { return application; }
			set { application = value; }
		}
		protected internal IObjectSpace ObjectSpace {
			get { return CollectionSource != null ? CollectionSource.ObjectSpace : null; }
		}
		protected internal IList<IMemberInfo> KeyMembersInfo {
			get { return keyMembersInfo; }
		}
		protected internal object GetObjectByKey(object key) {
			if(collectionSource != null) {
				if(collectionSource.ObjectTypeInfo.IsPersistent && (collectionSource.DataAccessMode != CollectionSourceDataAccessMode.DataView)) {
					if(ObjectSpace != null && !ObjectSpace.IsDisposed) {
						return ObjectSpaceHelper.FindObjectByKey(ObjectSpace, collectionSource.ObjectTypeInfo.Type, key);
					}
				}
				else {
					foreach(object obj in collectionSource.List) {
						Object keyValue = collectionSource.ObjectSpace.GetKeyValue(obj);
						if(KeyValueEquals(keyValue, key)) {
							return obj;
						}
					}
				}
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void CreatePropertyEditors() {
			foreach(IModelMemberViewItem memberViewItem in Model.Columns) {
				if(memberViewItem.ModelMember != null) {
					CreatePropertyEditorIfNeed(memberViewItem, ViewEditMode.View);
					CreatePropertyEditorIfNeed(memberViewItem, ViewEditMode.Edit);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected WebPropertyEditor CreatePropertyEditorIfNeed(IModelMemberViewItem memberViewItem, ViewEditMode mode) {
			WebPropertyEditor result = dataItemTemplateFactory.FindPropertyEditor(memberViewItem, mode);
			if(result == null) {
				result = CreatePropertyEditorCore(memberViewItem, mode);
			}
			return result;
		}
		protected void UpdateKeyMemberInfo() {
			keyMembersInfo.Clear();
			if((collectionSource != null) && (collectionSource.ObjectSpace != null)) {
				foreach(IMemberInfo keyMemberInfo in collectionSource.ObjectTypeInfo.KeyMembers) {
					if(keyMemberInfo.IsPublic) {
						keyMembersInfo.Add(keyMemberInfo);
					}
				}
			}
		}
		protected virtual void UpdateCollectionDependentProperties() {
			UpdateKeyMemberInfo();
		}
		protected virtual void OnCollectionChanged() {
			UpdateCollectionDependentProperties();
		}
		protected virtual internal WebPropertyEditor CreatePropertyEditor(IModelMemberViewItem modelDetailViewItem) {
			return dataItemTemplateFactory.CreatePropertyEditorCore(modelDetailViewItem, application, ObjectTypeInfo, ObjectSpace);
		}
		protected virtual WebDataSource CreateWebDataSource(Object collection) {
			WebDataSource result = new WebDataSource(ObjectSpace, ObjectTypeInfo, collection);
			result.View.ObjectUpdated += new EventHandler<EventArgs>(View_ObjectUpdated);
			return result;
		}
		protected virtual void OnCommitChanges() {
			if(CommitChanges != null) {
				CommitChanges(this, EventArgs.Empty);
			}
		}
		protected CollectionSourceBase CollectionSource {
			get { return collectionSource; }
		}
		protected override bool HasProtectedContent(string propertyName) {
			return !(ObjectTypeInfo.FindMember(propertyName) == null || DataManipulationRight.CanRead(ObjectType, propertyName, null, CollectionSource, ObjectSpace));
		}
		protected override void OnSelectionChanged() {
			DropSelectedObjectsCache();
			base.OnSelectionChanged();
		}
		protected ITemplate CreateDefaultColumnTemplate(IModelColumn columnInfo, IDataItemTemplateInfoProvider dataItemTemplateInfoProvider, ViewEditMode viewEditMode) {
			CreatePropertyEditorCore(columnInfo, viewEditMode);
			DataItemTemplateBase result = dataItemTemplateFactory.CreateColumnTemplate(columnInfo, dataItemTemplateInfoProvider, application, ObjectTypeInfo, ObjectSpace, viewEditMode);
			if(result != null) {
				result.CustomCreateCellControl += new EventHandler<CustomCreateCellControlEventArgs>(dataItemTemplate_CustomCreateCellControl);
			}
			return result;
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			if(Control is Control) {
				((Control)Control).Load += new EventHandler(control_Load);
				((Control)Control).Unload += new EventHandler(control_Unload);
			}
		}
		protected ReadOnlyCollection<WebPropertyEditor> PropertyEditors {
			get {
				if(dataItemTemplateFactory != null) {
					return dataItemTemplateFactory.PropertyEditors;
				}
				return null;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual DataItemTemplateFactory CreateDataItemTemplateFactory() {
			return new DataItemTemplateFactory();
		}
		protected DataItemTemplateFactory DataItemTemplateFactory {
			get { return dataItemTemplateFactory; }
		}
		public override string[] RequiredProperties {
			get {
				if(collectionSource != null && collectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
					List<String> result = new List<String>(base.RequiredProperties);
					foreach(IModelColumn column in Model.Columns) {
						MediaDataObjectAttribute mediaDataObjectAttribute = column.ModelMember.MemberInfo.MemberTypeInfo.FindAttribute<MediaDataObjectAttribute>(false);
						if(mediaDataObjectAttribute != null) {
							string mediaDataKeyProperty = column.PropertyName + "." + mediaDataObjectAttribute.MediaDataKeyProperty;
							if(!result.Contains(mediaDataKeyProperty)) {
								result.Add(mediaDataKeyProperty);
							}
						}
					}
					return result.ToArray();
				}
				else {
					return base.RequiredProperties;
				}
			}
		}
		private IList GetSelectedObjectsCore() {
			List<Object> selectedObjects = new List<Object>();
			if(CanSelectRows) {
				IList<object> controlSelectedObjects = GetControlSelectedObjects();
				selectedObjects.AddRange(controlSelectedObjects);
			}
			return selectedObjects;
		}
		private void FillSelectedObjectsCache() {
			if(!isSelectedObjectsCacheActual) {
				IList selectedObjects = GetSelectedObjectsCore();
				foreach(object selectedObject in selectedObjects) {
					if(!selectedObjectsCache.Contains(selectedObject)) {
						selectedObjectsCache.Add(selectedObject);
					}
				}
				isSelectedObjectsCacheActual = true;
			}
		}
		private void DropSelectedObjectsCache() {
			if(isSelectedObjectsCacheActual) {
				selectedObjectsCache.Clear();
				isSelectedObjectsCacheActual = false;
			}
		}
		private WebPropertyEditor CreatePropertyEditorCore(IModelMemberViewItem modelDetailViewItem, ViewEditMode viewEditMode) {
			WebPropertyEditor result = CreatePropertyEditor(modelDetailViewItem);
			if(result != null) {
				result.ViewEditMode = viewEditMode;
				dataItemTemplateFactory.AddEditor(modelDetailViewItem, result);
			}
			return result;
		}
		private void control_Unload(object sender, EventArgs e) {
			if(CanSelectRows) {
				FillSelectedObjectsCache();
			}
		}
		private void control_Load(object sender, EventArgs e) {
			DropSelectedObjectsCache();
		}
		private void View_ObjectUpdated(object sender, EventArgs e) {
			OnCommitChanges();
		}
		private void collectionSource_CollectionChanged(object sender, EventArgs e) {
			OnCollectionChanged();
		}
		private void ObjectSpace_Reloaded(object sender, EventArgs e) {
			if(PropertyEditors != null) {
				foreach(WebPropertyEditor editor in PropertyEditors) {
					editor.CurrentObject = ObjectSpace.GetObject(editor.CurrentObject);
				}
			}
		}
		private Boolean KeyValueEquals(Object val1, Object val2) {
			Boolean result = true;
			if(keyMembersInfo.Count > 1) {
				for(Int32 i = 0; i < keyMembersInfo.Count; i++) {
					if(!((IList)val1)[i].Equals(((IList)val2)[i])) {
						result = false;
						break;
					}
				}
			}
			else {
				result = val1.Equals(val2);
			}
			return result;
		}
		private void dataItemTemplate_CustomCreateCellControl(object sender, CustomCreateCellControlEventArgs e) {
			if(CustomCreateCellControl != null) {
				CustomCreateCellControl(this, e);
			}
		}
		private void dataItemTemplateFactory_QueryProtectedContentEditorState(object sender, QueryProtectedContentEditorStateEventArgs e) {
			e.HasProtected = HasProtectedContent(e.PropertyName);
		}
		public event EventHandler<CustomCreateCellControlEventArgs> CustomCreateCellControl;
		#region IComplexListEditor Members
		public virtual void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			this.collectionSource = collectionSource;
			this.application = application;
			UpdateCollectionDependentProperties();
			collectionSource.CollectionChanged += new EventHandler(collectionSource_CollectionChanged);
			ObjectSpace.Reloaded += ObjectSpace_Reloaded;
			CreatePropertyEditors();
		}
		#endregion
		#region IDetailViewItemsHolder Members
		public IList<ViewItem> Items {
			get {
				List<ViewItem> result = new List<ViewItem>();
				if(dataItemTemplateFactory != null) {
					foreach(ViewItem item in dataItemTemplateFactory.PropertyEditors) {
						result.Add(item);
					}
				}
				return result;
			}
		}
		#endregion
		#region ISupportInplaceEdit Members
		public event EventHandler<EventArgs> CommitChanges;
		#endregion
		#region IProcessCallbackComplete Members
		public virtual void ProcessCallbackComplete() { }
		#endregion
		#region Obsolete 15.1
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected void CheckSelectionChanged() {
			if(Control != null) {
				List<object> selectedObjects = new List<object>();
				if(CanSelectRows) {
					selectedObjects.AddRange(GetControlSelectedObjects());
				}
				bool raiseSelectionChanged = selectedObjectsCache.Count != selectedObjects.Count;
				if(!raiseSelectionChanged) {
					foreach(object obj in selectedObjects) {
						if(!selectedObjectsCache.Contains(obj)) {
							raiseSelectionChanged = true;
							break;
						}
					}
				}
				if(raiseSelectionChanged) {
					OnSelectionChanged();
				}
			}
		}
		#endregion
#if DebugTest
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public WebPropertyEditor DebugTest_CreatePropertyEditor(IModelMemberViewItem modelDetailViewItem) {
			return CreatePropertyEditor(modelDetailViewItem);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public XafApplication DebugTest_Application {
			get { return Application; }
			set { Application = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		internal void DebugTest_CreatePropertyEditors() {
			CreatePropertyEditors();
		}
#endif
	}
}
