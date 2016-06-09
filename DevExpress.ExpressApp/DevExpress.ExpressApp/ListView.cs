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
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public enum MasterDetailMode { ListViewOnly, ListViewAndDetailView }
	public enum NewItemRowPosition { None, Top, Bottom };
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
	public class DefaultListViewOptionsAttribute : Attribute {
		public const NewItemRowPosition DefaultNewItemRowPosition = NewItemRowPosition.None;
		public const MasterDetailMode DefaultMasterDetailMode = MasterDetailMode.ListViewOnly;
		public static readonly DefaultListViewOptionsAttribute Default = new DefaultListViewOptionsAttribute();
		private NewItemRowPosition newItemRowPosition = DefaultNewItemRowPosition;
		private bool allowEdit = false;
		private MasterDetailMode masterDetailMode = DefaultMasterDetailMode;
		public DefaultListViewOptionsAttribute() {
		}
		public DefaultListViewOptionsAttribute(MasterDetailMode masterDetailMode) {
			this.masterDetailMode = masterDetailMode;
		}
		public DefaultListViewOptionsAttribute(bool allowEdit, NewItemRowPosition newItemRowPosition) {
			this.allowEdit = allowEdit;
			this.newItemRowPosition = newItemRowPosition;
		}
		public DefaultListViewOptionsAttribute(MasterDetailMode masterDetailMode, bool allowEdit, NewItemRowPosition newItemRowPosition) {
			this.masterDetailMode = masterDetailMode;
			this.allowEdit = allowEdit;
			this.newItemRowPosition = newItemRowPosition;
		}
		public NewItemRowPosition NewItemRowPosition {
			get { return newItemRowPosition; }
		}
		public bool AllowEdit {
			get { return allowEdit; }
		}
		public MasterDetailMode MasterDetailMode {
			get { return masterDetailMode; }
		}
	}
	public interface ISupportNewItemRowPosition {
		NewItemRowPosition NewItemRowPosition { get; set; }
	}
	public class PropertyCollectionSourceLink {
		private Type masterObjectType;
		private object masterObjectKey;
		private IMemberInfo collectionMemberDescriptor;
		private Boolean linkNewObjectToParentImmediately;
		public PropertyCollectionSourceLink(Type masterObjectType, object masterObjectKey, IMemberInfo collectionMemberDescriptor)
			: this(masterObjectType, masterObjectKey, collectionMemberDescriptor, true) {
		}
		public PropertyCollectionSourceLink(Type masterObjectType, object masterObjectKey, IMemberInfo collectionMemberDescriptor, Boolean linkNewObjectToParentImmediately) {
			this.masterObjectType = masterObjectType;
			this.masterObjectKey = masterObjectKey;
			this.collectionMemberDescriptor = collectionMemberDescriptor;
			this.linkNewObjectToParentImmediately = linkNewObjectToParentImmediately;
		}
		public PropertyCollectionSource GetPropertyCollectionSource(XafApplication application, IObjectSpace objectSpace) {
			return application.CreatePropertyCollectionSource(objectSpace, masterObjectType, objectSpace.GetObjectByKey(masterObjectType, masterObjectKey), collectionMemberDescriptor, "");
		}
		public Type MasterObjectType {
			get {
				return masterObjectType;
			}
		}
		public object MasterObjectKey {
			get {
				return masterObjectKey;
			}
		}
		public Boolean LinkNewObjectToParentImmediately {
			get {
				return linkNewObjectToParentImmediately;
			}
			set {
				linkNewObjectToParentImmediately = value;
			}
		}
		public IMemberInfo CollectionMemberDescriptor {
			get {
				return collectionMemberDescriptor;
			}
		}
	}
	public class Link {
		private ListView listView;
		private PropertyCollectionSourceLink propertyCollectionSourceLink;
		private void UnsubscribeFromEvents() {
			if(listView != null) {
				listView.Disposing -= new System.ComponentModel.CancelEventHandler(listView_Disposing);
				PropertyCollectionSource propertyCollectionSource = listView.CollectionSource as PropertyCollectionSource;
				if(listView.ObjectSpace != null) {
					listView.ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
				}
				if(propertyCollectionSource != null) {
					propertyCollectionSource.MasterObjectChanged -= new EventHandler(propertyCollectionSource_MasterObjectChanged);
				}
			}
		}
		private void InitMasterObjectKey() {
			if(listView != null) { 
				PropertyCollectionSource propertyCollectionSource = listView.CollectionSource as PropertyCollectionSource;
				if(propertyCollectionSource != null && propertyCollectionSource.MasterObject != null) {
					object masterObjectKey = listView.ObjectSpace.GetKeyValue(propertyCollectionSource.MasterObject);
					if(masterObjectKey != null) {
						propertyCollectionSourceLink = new PropertyCollectionSourceLink(propertyCollectionSource.MasterObjectType, masterObjectKey, propertyCollectionSource.MemberInfo);
					}
				}
			}
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			InitMasterObjectKey();
		}
		private void propertyCollectionSource_MasterObjectChanged(object sender, EventArgs e) {
			InitMasterObjectKey();
		}
		private void listView_Disposing(object sender, CancelEventArgs e) {
			UnsubscribeFromEvents();
		}
		public Link(ListView listView) {
			ListView = listView;
		}
		public PropertyCollectionSourceLink PropertyCollectionSourceLink {
			get {
				return propertyCollectionSourceLink;
			}
		}
		public ListView ListView {
			get { return listView; }
			set {
				UnsubscribeFromEvents();
				listView = value;
				if(listView != null) {
					PropertyCollectionSource propertyCollectionSource = listView.CollectionSource as PropertyCollectionSource;
					if(propertyCollectionSource != null) {
						listView.Disposing += new System.ComponentModel.CancelEventHandler(listView_Disposing);
						listView.ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
						propertyCollectionSource.MasterObjectChanged += new EventHandler(propertyCollectionSource_MasterObjectChanged);
						InitMasterObjectKey();
					}
				}
			}
		}
	}
	public class CreateCustomCurrentObjectDetailViewEventArgs : EventArgs {
		private DetailView detailView;
		private string detailViewId;
		private DetailView currentDetailView;
		private IModelListView modelListView;
		private Type listViewObjectType;
		private object listViewCurrentObject;
		public CreateCustomCurrentObjectDetailViewEventArgs(IModelListView modelListView, Type listViewObjectType, object listViewCurrentObject, DetailView currentDetailView) {
			this.modelListView = modelListView;
			this.listViewObjectType = listViewObjectType;
			this.listViewCurrentObject = listViewCurrentObject;
			this.currentDetailView = currentDetailView;
		}
		public DetailView DetailView {
			get { return detailView; }
			set { detailView = value; }
		}
		public string DetailViewId {
			get { return detailViewId; }
			set { detailViewId = value; }
		}
		public DetailView CurrentDetailView {
			get { return currentDetailView; }
		}
		public IModelListView ListViewModel {
			get { return modelListView; }
		}
		public Type ListViewObjectType {
			get { return listViewObjectType; }
		}
		public object ListViewCurrentObject {
			get { return listViewCurrentObject; }
		}
	}
	public class ListView : ObjectView {
		public const String ListViewControlID = "ListViewControlID";
		public const String DetailViewControlID = "DetailViewControlID";
		public const String ListViewCriteriaName = "ListViewCriteria";
		public const string InfoAllowLink = "Info.AllowLink";
		public const string InfoAllowUnlink = "Info.AllowUnlink";
		private static IList emptyReadOnlyCollection = new ReadOnlyCollection<object>(new object[0] { });
		private ListEditor editor;
		private Frame editFrame;
		private CollectionSourceBase collectionSource;
		private String detailViewId;
		private Object currentObject_BeforeCollectionChanged;
		private Object currentObject_BeforeFocusedObjectChanged;
		private Boolean isCollectionChangedHandling;
		private MasterDetailMode masterDetailMode;
		private BoolList allowLink = new BoolList(true, BoolListOperatorType.And);
		private BoolList allowUnlink = new BoolList(true, BoolListOperatorType.And);
		private void DoOnListEditorFocusedObjectChanged() {
			if(!isCollectionChangedHandling && (currentObject_BeforeFocusedObjectChanged != CurrentObject)) {
				UpdateEditFrame(CurrentObject);
				OnCurrentObjectChanged();
				currentObject_BeforeFocusedObjectChanged = CurrentObject;
			}
		}
		private void allowLink_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			OnAllowLinkChanged();
		}
		private void allowUnlink_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			OnAllowUnlinkChanged();
		}
		private void listEditor_FocusedObjectChanging(Object sender, CancelEventArgs e) {
			e.Cancel = !OnQueryCanChangeCurrentObject();
		}
		private void listEditor_FocusedObjectChanged(Object sender, EventArgs e) {
			DoOnListEditorFocusedObjectChanged();
		}
		private void listEditor_SelectionChanged(Object sender, EventArgs e) {
			OnSelectionChanged();
		}
		private void listEditor_SelectionTypeChanged(Object sender, EventArgs e) {
			OnSelectionTypeChanged();
		}
		private void listEditor_ObjectChanged(Object sender, EventArgs e) {
			if(CurrentObject != null) {
				ObjectSpace.SetModified(ObjectSpace.GetObject(CurrentObject));
			}
			OnObjectChanged();
		}
		private void listEditor_ProcessSelectedItem(Object sender, EventArgs e) {
			OnProcessSelectedItem();
		}
		private void listEditor_ValidateObject(Object sender, ValidateObjectEventArgs e) {
			OnValidateObject(e);
		}
		private void ColumnsListEditor_ColumnAdded(Object sender, EventArgs e) {
			RefreshCollectionSourceDisplayableProperties();
		}
		private void collectionSource_CollectionChanging(Object sender, EventArgs e) {
			currentObject_BeforeCollectionChanged = CurrentObject;
		}
		private void collectionSource_CollectionChanged(Object sender, EventArgs e) {
			UpdateSecurityModifiers();
			isCollectionChangedHandling = true;
			try {
				RefreshCollectionSourceDisplayableProperties();
				RefreshEditorDataSource();
				RestorePreviousCurrentObject();
			}
			finally {
				isCollectionChangedHandling = false;
				DoOnListEditorFocusedObjectChanged();
			}
		}
		private void RestorePreviousCurrentObject() {
			if(((SelectionType & SelectionType.FocusedObject) == SelectionType.FocusedObject) && (currentObject_BeforeCollectionChanged != null)) {
				CurrentObject = GetObject(currentObject_BeforeCollectionChanged);
			}
			currentObject_BeforeCollectionChanged = null;
		}
		private void collectionSource_CollectionReloading(Object sender, EventArgs e) {
			currentObject_BeforeCollectionChanged = CurrentObject;
		}
		private void collectionSource_CollectionReloaded(Object sender, EventArgs e) {
			RestorePreviousCurrentObject();
		}
		private void UpdateEditFrame(Object currentObject) {
			if((MasterDetailMode == MasterDetailMode.ListViewAndDetailView) &&
				((SelectionType & SelectionType.FocusedObject) == SelectionType.FocusedObject) &&
				((Application == null) || Application.SupportMasterDetailMode)) {
				DetailView currentDetailView = EditView;
				CreateCustomCurrentObjectDetailViewEventArgs args = new CreateCustomCurrentObjectDetailViewEventArgs(Model, ObjectTypeInfo.Type, CurrentObject, currentDetailView);
				if(CreateCustomCurrentObjectDetailView != null) {
					CreateCustomCurrentObjectDetailView(this, args);
				}
				if(args.DetailView == null) {
					String detailViewId = args.DetailViewId;
					if (String.IsNullOrEmpty(detailViewId) && Model != null) {
						if(Model.MasterDetailView != null) {
							detailViewId = Model.MasterDetailView.Id;
						}
						else if(Model.DetailView != null) {
							detailViewId = Model.DetailView.Id;
						}
					}
					if(String.IsNullOrEmpty(detailViewId)) {
						if((CurrentObject != null) && (CurrentObject.GetType() != ObjectTypeInfo.Type)) {
							detailViewId = Application.GetDetailViewId(CurrentObject.GetType());
						}
					}
					if(String.IsNullOrEmpty(detailViewId) && (Application != null)) {
						detailViewId = Application.GetDetailViewId(ObjectTypeInfo.Type);
					}
					if((currentDetailView != null) && (currentDetailView.Id == detailViewId)) {
						args.DetailView = currentDetailView;
					}
					else {
						args.DetailView = Application.CreateDetailView(ObjectSpace, detailViewId, false,
							ObjectSpace.GetObject(currentObject));
						OnEditViewCreated(args.DetailView);
					}
				}
				if(currentDetailView != args.DetailView) {
					if(editFrame == null) {
						editFrame = Application.CreateFrame(TemplateContext.View);
					}
					editFrame.SetView(args.DetailView, null);
					UpdateNestedFrameItem(editFrame);
					if(IsControlCreated) {
						editFrame.View.CreateControls();
						LayoutManager.ReplaceControl(DetailViewControlID, editFrame.View.Control);
					}
				}
				if(editFrame != null) {
					((DetailView)editFrame.View).CurrentObject = ((DetailView)editFrame.View).ObjectSpace.GetObject(currentObject);
				}
			}
		}
		private void UpdateNestedFrameItem(Frame frame) {
			NestedFrameItem nestedFrameItem = (NestedFrameItem)FindItem(DetailViewControlID);
			if (nestedFrameItem != null) {
				nestedFrameItem.SetFrame(frame);
			}
		}
		private void DisposeEditFrame() {
			if(editFrame != null) {
				editFrame.Dispose();
				editFrame = null;
				UpdateNestedFrameItem(null);
			}
		}
		private void CreateListViewItems() {
			ItemsCollection.Add(new ListEditorViewItem(Editor));
			if((MasterDetailMode == MasterDetailMode.ListViewAndDetailView) && (editFrame != null)) {
				ViewsOrder viewsOrder = (Model != null) ? Model.SplitLayout.ViewsOrder : ViewsOrder.ListViewDetailView;
				NestedFrameItem nestedFrameItem = new NestedFrameItem(DetailViewControlID, editFrame);
				if(viewsOrder == ViewsOrder.ListViewDetailView) {
					ItemsCollection.Add(nestedFrameItem);
				}
				else if(viewsOrder == ViewsOrder.DetailViewListView) {
					ItemsCollection.Insert(0, DetailViewControlID, nestedFrameItem);
				}
			}
		}
		private void UnsubscribeFromCollectionSource() {
			if(collectionSource != null) {
				collectionSource.CollectionChanging -= new EventHandler(collectionSource_CollectionChanging);
				collectionSource.CollectionChanged -= new EventHandler(collectionSource_CollectionChanged);
				collectionSource.CollectionReloading -= new EventHandler(collectionSource_CollectionReloading);
				collectionSource.CollectionReloaded -= new EventHandler(collectionSource_CollectionReloaded);
			}
		}
		private void UnsubscribeFromEditor() {
			if(editor != null) {
				editor.FocusedObjectChanging -= new EventHandler<CancelEventArgs>(listEditor_FocusedObjectChanging);
				editor.FocusedObjectChanged -= new EventHandler(listEditor_FocusedObjectChanged);
				editor.SelectionChanged -= new EventHandler(listEditor_SelectionChanged);
				editor.SelectionTypeChanged -= new EventHandler(listEditor_SelectionTypeChanged);
				editor.ObjectChanged -= new EventHandler(listEditor_ObjectChanged);
				editor.ProcessSelectedItem -= new EventHandler(listEditor_ProcessSelectedItem);
				editor.ValidateObject -= new EventHandler<ValidateObjectEventArgs>(listEditor_ValidateObject);
				if(editor is ColumnsListEditor) {
					((ColumnsListEditor)editor).ColumnAdded -= ColumnsListEditor_ColumnAdded;
				}
			}
		}
		private void UpdateListEditorAllowEdit(ListEditor listEditor) {
			if(listEditor != null) {
				listEditor.AllowEdit = AllowEdit;
			}
		}
		protected override Boolean IsItemsChangingSupported() {
			return false;
		}
		protected void RefreshCollectionSourceDisplayableProperties() {
			if(editor != null) {
				List<String> displayableMembers = new List<String>(editor.RequiredProperties);
				CollectionSource source = collectionSource as CollectionSource;
				if((source == null)
					||
					((source.DataAccessMode != CollectionSourceDataAccessMode.Server) || !source.IsAsyncServerMode)
					&&
					(source.DataAccessMode != CollectionSourceDataAccessMode.DataView)
				) {
					if(editor.Model != null) {
						foreach(IModelColumn columnInfo in editor.Model.Columns) {
							if(!displayableMembers.Contains(columnInfo.PropertyName)) {
								displayableMembers.Add(columnInfo.PropertyName);
							}
						}
					}
				}
				if(collectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
					displayableMembers = ListView.CorrectMemberNamesForDataViewMode(ObjectTypeInfo, displayableMembers, true, true);
				}
				collectionSource.DisplayableProperties = String.Join(";", displayableMembers.ToArray());
			}
		}
		protected void RefreshEditorDataSource() {
			if(editor != null) {
				if(!editor.SupportsDataAccessMode(collectionSource.DataAccessMode) ) {
					throw new InvalidOperationException(String.Format("The '{0}' used in the '{1}' does not support {2} Mode.", editor.GetType().FullName, model.Id, collectionSource.DataAccessMode));
				}
				editor.DataSource = collectionSource.Collection;
			}
		}
		protected virtual void OnObjectChanged() {
			if(ObjectChanged != null) {
				ObjectChanged(this, EventArgs.Empty);
			}
		}
		protected override void OnAllowEditChanged() {
			base.OnAllowEditChanged();
			UpdateListEditorAllowEdit(editor);
		}
		protected virtual void OnAllowLinkChanged() {
			if(AllowLinkChanged != null) {
				AllowLinkChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnAllowUnlinkChanged() {
			if(AllowUnlinkChanged != null) {
				AllowUnlinkChanged(this, EventArgs.Empty);
			}
		}
		protected override void OnObjectSpaceReloaded() {
		}
		protected virtual void OnProcessSelectedItem() {
			if(ProcessSelectedItem != null) {
				ProcessSelectedItem(this, EventArgs.Empty);
			}
		}
		protected override Object CreateControlsCore() {
			if(Editor != null) {
				if(ItemsCollection.Values.Count == 0) {
					CreateListViewItems();
				}
				if(LayoutManager == null) {
					CreateViewItemControls();
					return Editor.Control;
				}
				return base.CreateControlsCore();
			}
			else {
				return null;
			}
		}
		protected override void SaveModelCore() {
			if(editFrame != null) {
				editFrame.SaveModel();
			}
			if(Editor != null) {
				Editor.SaveModel();
			}
			if(LayoutManager != null) {
				LayoutManager.SaveModel();
			}
		}
		protected override void LoadModelCore() {
				Editor = null;
			DisposeEditFrame();
			DisposeViewControl();
			ItemsCollection.Clear();
			base.LoadModelCore();
			if(Model != null) {
				AllowLink.SetItemValue(InfoAllowLink, Model.AllowLink);
				AllowUnlink.SetItemValue(InfoAllowUnlink, Model.AllowUnlink);
				String criteria = Model.Criteria;
				if(!String.IsNullOrEmpty(criteria)) {
					if(collectionSource.CanApplyCriteria) {
						collectionSource.SetCriteria(ListViewCriteriaName, criteria);
					}
				}
				else {
					if(collectionSource.Criteria.ContainsKey(ListViewCriteriaName)) {
						collectionSource.Criteria.Remove(ListViewCriteriaName);
					}
				}
				List<SortProperty> sorting = new List<SortProperty>();
				foreach(IModelSortProperty sortProperty in Model.Sorting) {
					sorting.Add(new SortProperty(sortProperty.PropertyName, sortProperty.Direction));
				}
				collectionSource.Sorting = sorting;
				Int32 topReturnedObjects = Model.TopReturnedObjects;
				if(topReturnedObjects >= 0) {
					collectionSource.TopReturnedObjects = topReturnedObjects;
				}
				if(Model.DetailView != null) {
					detailViewId = Model.DetailView.Id;
				}
			}
			if(Application != null) {
				if(String.IsNullOrEmpty(detailViewId)) {
					detailViewId = Application.FindDetailViewId(ObjectTypeInfo.Type);
				}
					Editor = Application.CreateListEditor(collectionSource, Model);
					Editor.Name = Caption;
				UpdateEditFrame(CurrentObject);
				CreateListViewItems();
			}
		}
		protected override bool GetIsLayoutSimple() {
			return true;
		}
		protected override void RefreshCore() {
			if(Editor != null) {
				RefreshCollectionSourceDisplayableProperties();
				RefreshEditorDataSource();
				Editor.Refresh();
			}
		}
		protected override void UpdateSecurityModifiers() {
			if(collectionSource != null && !(SecuritySystem.Instance is DevExpress.ExpressApp.Security.IRequestSecurity)) {
				AllowEdit.SetItemValue(SecurityReadOnlyItemName, DataManipulationRight.CanEdit(collectionSource.ObjectTypeInfo.Type, "", null, collectionSource, ObjectSpace));
			}
			else {
				base.UpdateSecurityModifiers();
			}
		}
		protected override void DisposeCore() {
			currentObject_BeforeCollectionChanged = null;
			currentObject_BeforeFocusedObjectChanged = null;
			EditorChanging = null;
			EditorChanged = null;
			ObjectChanged = null;
			EditViewCreated = null;
			CreateCustomCurrentObjectDetailView = null;
			ProcessSelectedItem = null;
			Editor = null;
			AllowLinkChanged = null;
			AllowUnlinkChanged = null;
			DisposeEditFrame();
			DisposeViewControl();
			UnsubscribeFromCollectionSource();
			if(collectionSource != null) {
				collectionSource.Dispose();
				collectionSource = null;
			}
			base.DisposeCore();
		}
		protected virtual void OnEditorChanging() {
			if(EditorChanging != null) {
				EditorChanging(this, EventArgs.Empty);
			}
		}
		protected virtual void OnEditorChanged() {
			if(EditorChanged != null) {
				EditorChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnValidateObject(ValidateObjectEventArgs e) {
			if(ValidateObject != null) {
				ValidateObject(this, e);
			}
		}
		protected virtual void OnEditViewCreated(DetailView detailView) {
			if(EditViewCreated != null) {
				EditViewCreated(this, new DetailViewCreatedEventArgs(detailView));
			}
		}
		protected override ViewShortcut CreateShortcutCore() {
			ViewShortcut result = base.CreateShortcutCore();
			result.ObjectKey = "";
			return result;
		}
		protected override void CloseCore() {
			base.CloseCore();
			UnsubscribeFromEditor();
			UnsubscribeFromCollectionSource();
		}
		public ListView(CollectionSourceBase collectionSource, XafApplication application, bool isRoot)
			: base(application, collectionSource.ObjectSpace, isRoot) {
			if(collectionSource == null) {
				throw new ArgumentNullException("collectionSource");
			}
			allowLink.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(allowLink_ResultValueChanged);
			allowUnlink.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(allowUnlink_ResultValueChanged);
			this.collectionSource = collectionSource;
			collectionSource.CollectionChanging += new EventHandler(collectionSource_CollectionChanging);
			collectionSource.CollectionChanged += new EventHandler(collectionSource_CollectionChanged);
			collectionSource.CollectionReloading += new EventHandler(collectionSource_CollectionReloading);
			collectionSource.CollectionReloaded += new EventHandler(collectionSource_CollectionReloaded);
			ObjectTypeInfo = collectionSource.ObjectTypeInfo;
			if(collectionSource != null) {
				AllowEdit["DataAccessMode"] = (collectionSource.DataAccessMode != CollectionSourceDataAccessMode.DataView);
			}
		}
		public ListView(IModelListView modelListView, CollectionSourceBase collectionSource, XafApplication application, bool isRoot)
			: this(collectionSource, application, isRoot) {
			SetModel(modelListView);
		}
		public ListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot, XafApplication application)
			: this(collectionSource, application, isRoot) {
			if(listEditor != null) {	 
				model = listEditor.Model;
				Editor = listEditor;
			}
		}
		public ListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot)
			: this(collectionSource, listEditor, isRoot, null) {
		}
		public ListView(CollectionSourceBase collectionSource, ListEditor listEditor)
			: this(collectionSource, listEditor, false) {
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewCurrentObject")]
#endif
		public override Object CurrentObject {
			get {
				Object result = null;
				if(Editor != null) {
					result = Editor.FocusedObject;
				}
				return result;
			}
			set {
				if(value != null) {
					Guard.CheckObjectFromObjectSpace(ObjectSpace, value);
				}
				if(Editor == null) {
					throw new InvalidOperationException("Editor is null");
				}
				Object obj = value;
				if((collectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) && (obj != null) && !(obj is XafDataViewRecord)) {
					obj = ((XafDataView)collectionSource.OriginalCollection).FindDataViewRecordByObject(obj);
				}
				Editor.FocusedObject = obj;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Object GetCurrentTypedObject() {
			Object obj = CurrentObject;
			if(collectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
				obj = ObjectSpace.GetObject(obj);
			}
			return obj;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Object GetObject(Object obj) {
			Object result = null;
			if(collectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
				result = ((XafDataView)collectionSource.OriginalCollection).FindDataViewRecordByObject(obj);
			}
			else {
				result = ObjectSpace.GetObject(obj);
			}
			return result;
		}
		public override void RefreshDataSource() {
			base.RefreshDataSource();
			if(collectionSource != null) {
				collectionSource.Reload();
			}
		}
		public BoolList AllowLink {
			get { return allowLink; }
		}
		public BoolList AllowUnlink {
			get { return allowUnlink; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewSelectedObjects")]
#endif
		public override IList SelectedObjects {
			get {
				if(Editor != null) {
					return ArrayList.ReadOnly(new ArrayList(Editor.GetSelectedObjects()));
				}
				else {
					return emptyReadOnlyCollection;
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewSelectionType")]
#endif
		public override SelectionType SelectionType {
			get {
				if(Editor == null) {
					return base.SelectionType;
				}
				else {
					return Editor.SelectionType;
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewModel")]
#endif
		public new IModelListView Model {
			get { return (IModelListView)base.Model; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewEditor")]
#endif
		public ListEditor Editor {
			get { return editor; }
			set {
				Boolean isEditorChanged = (editor != value);
				if(isEditorChanged) {
					OnEditorChanging();
				}
				UnsubscribeFromEditor();
				if(editor != null) {
					if(value == null) {
						editor.BreakLinksToControls();
					}
					editor.Dispose();
				}
				editor = value;
				if(editor != null) {
					UpdateListEditorAllowEdit(editor);
					editor.ProcessSelectedItem += new EventHandler(listEditor_ProcessSelectedItem);
					editor.FocusedObjectChanged += new EventHandler(listEditor_FocusedObjectChanged);
					editor.FocusedObjectChanging += new EventHandler<CancelEventArgs>(listEditor_FocusedObjectChanging);
					editor.SelectionChanged += new EventHandler(listEditor_SelectionChanged);
					editor.SelectionTypeChanged += new EventHandler(listEditor_SelectionTypeChanged);
					editor.ObjectChanged += new EventHandler(listEditor_ObjectChanged);
					editor.ValidateObject += new EventHandler<ValidateObjectEventArgs>(listEditor_ValidateObject);
					if(editor is ColumnsListEditor) {
						((ColumnsListEditor)editor).ColumnAdded += ColumnsListEditor_ColumnAdded;
					}
					RefreshCollectionSourceDisplayableProperties();
					RefreshEditorDataSource();
				}
				if(isEditorChanged) {
					OnEditorChanged();
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewEditView")]
#endif
		public DetailView EditView {
			get { return (editFrame == null) ? null : (DetailView)editFrame.View; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Frame EditFrame {
			get {
				return editFrame;
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewCaption")]
#endif
		public override string Caption {
			get { return base.Caption; }
			set {
				base.Caption = value;
				if(Editor != null) {
					Editor.Name = value;
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewDetailViewId")]
#endif
		public string DetailViewId {
			get { return detailViewId; }
			set { detailViewId = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewCollectionSource")]
#endif
		public CollectionSourceBase CollectionSource { get { return collectionSource; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MasterDetailMode MasterDetailMode {
			get {
				if(Model != null) {
					return Model.MasterDetailMode;
				}
				else {
					return masterDetailMode;
				}
			}
			set {
				masterDetailMode = value;
				if(Model != null) {
					Model.MasterDetailMode = value;
				}
			}
		}
		public event EventHandler EditorChanging;
		public event EventHandler EditorChanged;
		public event EventHandler ObjectChanged;
		public event EventHandler<CreateCustomCurrentObjectDetailViewEventArgs> CreateCustomCurrentObjectDetailView;
		public event EventHandler ProcessSelectedItem;
		public event EventHandler<ValidateObjectEventArgs> ValidateObject;
		public event EventHandler<DetailViewCreatedEventArgs> EditViewCreated;
		public event EventHandler AllowLinkChanged;
		public event EventHandler AllowUnlinkChanged;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static List<String> CorrectMemberNamesForDataViewMode(ITypeInfo objectTypeInfo, IList<String> memberNames, Boolean addKeyMembers, Boolean addBraces) {
			List<String> result = new List<String>();
			foreach(String memberName in memberNames) {
				IMemberInfo memberInfo = objectTypeInfo.FindMember(memberName);
				if((memberInfo != null)
					&&
					(memberInfo.IsPersistent || memberInfo.IsAliased || !String.IsNullOrWhiteSpace(memberInfo.Expression))
					&&
					((memberInfo.MemberTypeInfo == null) || !memberInfo.MemberTypeInfo.IsDomainComponent)
				) {
					if(addBraces) {
						result.Add("[" + memberName + "]");
					}
					else {
						result.Add(memberName);
					}
				}
			}
			if(addKeyMembers) {
				foreach(IMemberInfo keyMemberInfo in objectTypeInfo.KeyMembers) {
					String keyMemberName = keyMemberInfo.Name;
					if(addBraces) {
						keyMemberName = "[" + keyMemberName + "]";
					}
					if(!result.Contains(keyMemberName)) {
						result.Add(keyMemberName);
					}
				}
			}
			return result;
		}
	}
}
