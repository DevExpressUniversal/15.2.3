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
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.TreeListEditors.Win {
	public class CategorizedListEditor : GridListEditor {
		public const string CategoryPropertyName = "Category";
		private const string UpdateCriteriaMethodName = "UpdateCriteria";
		private Locker updateCriteriaLocker;
		private object categoryKey;
		private Boolean isCriteriaUpdating;
		private LayoutManager layoutManager;
		private string criteriaPropertyName;
		private string categoriesListViewId;
		private CollectionSourceBase categoriesDataSource;
		private ListView categoriesListView;
		private CategorizedListEditorTypeDescriptionProvider typeDescriptionProvider;
		private void objectTreeList_NodesReloading(object sender, EventArgs e) {
			updateCriteriaLocker.Lock();
		}
		private void objectTreeList_NodesReloaded(object sender, EventArgs e) {
			updateCriteriaLocker.Unlock();
		}
		private void updateCriteriaLocker_LockedChanged(object sender, LockedChangedEventArgs e) {
			if(!e.Locked && e.PendingCalls.Contains(UpdateCriteriaMethodName)) {
				UpdateCriteria();
			}
		}
		private void ObjectSpace_Reloaded(object sender, EventArgs e) {
			UpdateCriteria();
		}
		private void categoriesListView_SelectionChanged(object sender, EventArgs e) {
			UpdateCriteria();
			UpdateColumns();
		}
		private void SubscribeToTreeList() {
			if(ObjectTreeList != null) {
				ObjectTreeList.NodesReloading += new EventHandler(objectTreeList_NodesReloading);
				ObjectTreeList.NodesReloaded += new EventHandler(objectTreeList_NodesReloaded);
			}
		}
		private void UnsubscribeFromTreeList() {
			if(ObjectTreeList != null) {
				ObjectTreeList.NodesReloading -= new EventHandler(objectTreeList_NodesReloading);
				ObjectTreeList.NodesReloaded -= new EventHandler(objectTreeList_NodesReloaded);
			}
		}
		private void UpdateCriteria() {
			isCriteriaUpdating = true;
			try {
				updateCriteriaLocker.Call(UpdateCriteriaMethodName);
				if(!updateCriteriaLocker.Locked) {
					if(CategoriesListView != null) {
						object currentCategory = CategoriesListView.CurrentObject;
						if(currentCategory != null) {
							ItemsDataSource.Criteria[CategoryPropertyName] = new BinaryOperator(criteriaPropertyName, CategoriesListView.ObjectSpace.GetKeyValue(currentCategory));
						}
						else {
							ItemsDataSource.Criteria[CategoryPropertyName] = new NullOperator(CategoryPropertyName);
						}
					}
				}
			}
			finally {
				isCriteriaUpdating = false;
			}
		}
		private void UpdateColumns() {
			Type itemType = ItemsDataSource.ObjectTypeInfo.Type;
			if(typeof(IVariablePropertiesCategorizedItem).IsAssignableFrom(itemType) && CategoriesListView.CurrentObject != null) {
				IPropertyDescriptorContainer newCategory = (IPropertyDescriptorContainer)CategoriesListView.CurrentObject;
				object newCategoryKey = CategoriesListView.ObjectSpace.GetKeyValue(newCategory);
				if(categoryKey != newCategoryKey) {
					SaveModel();
					FocusedObject = null;
					typeDescriptionProvider.Setup(newCategory.PropertyDescriptors);
					string newCategoryViewID = itemType.Name + "_" + newCategoryKey.ToString() + "_ListView";
					IModelViews viewsNode = (IModelViews)Model.Parent;
					IModelListView newViewNode = (IModelListView)viewsNode[newCategoryViewID];
					if(newViewNode == null) {
						newViewNode = (IModelListView)((ModelNode)Model).Clone(newCategoryViewID);
					}
					SetModel(newViewNode);
					ApplyModel();
					ItemsDataSource.DisplayableProperties = string.Join(";", RequiredProperties);
					categoryKey = newCategoryKey;
				}
			}
		}
		protected CollectionSourceBase ItemsDataSource {
			get { return CollectionSource; }
		}
		protected CollectionSourceBase CategoriesDataSource {
			get { return categoriesDataSource; }
		}
		protected override object CreateControlsCore() {
			if(layoutManager == null) {
				layoutManager = Application.CreateLayoutManager(true);
				categoriesListView = Application.CreateListView(categoriesListViewId, CategoriesDataSource, false);
				categoriesListView.Caption = CategoryPropertyName;
				categoriesListView.SelectionChanged += new EventHandler(categoriesListView_SelectionChanged);
				categoriesListView.CreateControls();
				if(ObjectTreeList != null) {
					ObjectTreeList.OptionsSelection.MultiSelect = false;
				}
				ViewItemsCollection viewItems = new ViewItemsCollection();
				viewItems.Add(new ControlViewItem("1", categoriesListView.Control));
				viewItems.Add(new ControlViewItem("2", base.CreateControlsCore()));
				layoutManager.LayoutControls(Model.SplitLayout, viewItems);
				SubscribeToTreeList();
			}
			return layoutManager.Container;
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
			CategorizedListEditorDataSource categorizedListEditorDataSource;
			if(dataSource != null) {
				categorizedListEditorDataSource = new CategorizedListEditorDataSource(dataSource, ObjectType);
			}
			else {
				categorizedListEditorDataSource = null;
			}
			Type itemType = ItemsDataSource.ObjectTypeInfo.Type;
			if(typeof(IVariablePropertiesCategorizedItem).IsAssignableFrom(itemType) && CategoriesListView != null && CategoriesListView.CurrentObject != null) {
				IPropertyDescriptorContainer currentCategory = (IPropertyDescriptorContainer)CategoriesListView.CurrentObject;
				typeDescriptionProvider.Setup(currentCategory.PropertyDescriptors);
			}
			base.AssignDataSourceToControl(categorizedListEditorDataSource);
		}
		public CategorizedListEditor(IModelListView info)
			: base(info) {
			updateCriteriaLocker = new Locker();
			updateCriteriaLocker.LockedChanged += new EventHandler<LockedChangedEventArgs>(updateCriteriaLocker_LockedChanged);
		}
		public override void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			base.Setup(collectionSource, application);
			IObjectSpace objectSpace = collectionSource.ObjectSpace;
			objectSpace.Reloaded += new EventHandler(ObjectSpace_Reloaded);
			ITypeInfo typeInfo = collectionSource.ObjectTypeInfo;
			IMemberInfo categoryPropertyInfo = typeInfo.FindMember(CategoryPropertyName);
			if(categoryPropertyInfo == null) {
				string message = string.Format("The {0} type does not declare the public {1} property.", typeInfo.FullName, CategoryPropertyName);
				throw new InvalidOperationException(message);
			}
			Type categoryType = categoryPropertyInfo.MemberType;
			categoriesListViewId = application.FindListViewId(categoryType);
			if(string.IsNullOrEmpty(categoriesListViewId)) {
				string message = string.Format("Cannot find ListView for the Category property type {0} in the Application Model. Make sure the property is of the business object type registered in the application.", categoryType.FullName);
				throw new InvalidOperationException(message);
			}
			if(categoriesListViewId == Model.Id) {
				string message = string.Format("The default category ListView ({0}) is the same as the container ListView. To avoid recursion, provide different EditorType settings for it.", categoriesListViewId);
				throw new InvalidOperationException(message);
			}
			categoriesDataSource = application.CreateCollectionSource(objectSpace, categoryType, categoriesListViewId);
			criteriaPropertyName = CategoryPropertyName + "." + objectSpace.GetKeyPropertyName(categoryType);
			typeDescriptionProvider = new CategorizedListEditorTypeDescriptionProvider(typeInfo.Type);
			typeDescriptionProvider.AddProvider();
		}
		public override void SaveModel() {
			base.SaveModel();
			if(layoutManager != null) {
				layoutManager.SaveModel();
			}
		}
		public override void Dispose() {
			try {
				UnsubscribeFromTreeList();
				if(typeDescriptionProvider != null) {
					typeDescriptionProvider.RemoveProvider();
					typeDescriptionProvider = null;
				}
				if(layoutManager != null) {
					layoutManager.Dispose();
					layoutManager = null;
				}
				if(categoriesListView != null) {
					categoriesListView.SelectionChanged -= new EventHandler(categoriesListView_SelectionChanged);
					categoriesListView.Dispose();
					categoriesListView = null;
				}
				if(categoriesDataSource != null) {
					categoriesDataSource.Dispose();
					categoriesDataSource = null;
				}
				if(ItemsDataSource != null) {
					ItemsDataSource.ObjectSpace.Reloaded -= new EventHandler(ObjectSpace_Reloaded);
				}
				updateCriteriaLocker.LockedChanged -= new EventHandler<LockedChangedEventArgs>(updateCriteriaLocker_LockedChanged);
			}
			finally {
				base.Dispose();
			}
		}
		public ListView CategoriesListView {
			get { return categoriesListView; }
		}
		public ObjectTreeList ObjectTreeList {
			get {
				if(CategoriesListView != null && CategoriesListView.Editor != null) {
					return CategoriesListView.Editor.Control as ObjectTreeList;
				}
				return null;
			}
		}
		public override object FocusedObject {
			get { return base.FocusedObject; }
			set {
				if(ObjectTreeList != null && value != null) {
					ITreeNode category = ((ICategorizedItem)value).Category;
					if(!isCriteriaUpdating && category != null) {
						ObjectTreeList.FocusedObject = category;
					}
				}
				base.FocusedObject = value;
			}
		}
	}
	public class CategorizedListEditorDataSource : BindingSource {
		private Type objectType;
		public CategorizedListEditorDataSource(Object dataSource, Type objectType)
			: base(dataSource, "") {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			this.objectType = objectType;
		}
		public override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			PropertyDescriptorCollection result;
			ITypedList dataSourceTypedList = ListHelper.GetList(DataSource) as ITypedList;
			if(dataSourceTypedList != null) {
				result = dataSourceTypedList.GetItemProperties(listAccessors);
			}
			else {
				result = new PropertyDescriptorCollection(null);
			}
			if(listAccessors == null) {
				PropertyDescriptorCollection additionalProperties = TypeDescriptor.GetProperties(objectType);
				foreach(PropertyDescriptor item in additionalProperties) {
					if(item.IsBrowsable && result.Find(item.Name, false) == null) {
						result.Add(item);
					}
				}
			}
			return result;
		}
	}
	internal class CategorizedListEditorTypeDescriptionProvider : TypeDescriptionProvider {
		private Type objectType;
		private CategorizedListEditorCustomTypeDescriptor objectTypeDescriptor;
		public CategorizedListEditorTypeDescriptionProvider(Type objectType) {
			this.objectType = objectType;
		}
		public void Setup(PropertyDescriptorCollection properties) {
			objectTypeDescriptor = new CategorizedListEditorCustomTypeDescriptor(objectType, properties);
		}
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
			if(this.objectType == objectType && objectTypeDescriptor != null) {
				return objectTypeDescriptor;
			}
			return base.GetTypeDescriptor(objectType, instance);
		}
		public void AddProvider() {
			TypeDescriptor.AddProvider(this, objectType);
		}
		public void RemoveProvider() {
			TypeDescriptor.RemoveProvider(this, objectType);
		}
	}
	internal class CategorizedListEditorCustomTypeDescriptor : CustomTypeDescriptor {
		private PropertyDescriptorCollection properties;
		public CategorizedListEditorCustomTypeDescriptor(Type objectType, PropertyDescriptorCollection properties) {
			this.properties = new PropertyDescriptorCollection(null);
			foreach(PropertyDescriptor propertyDescriptor in properties) {
				this.properties.Add(new CategorizedListEditorPropertyDescriptor(objectType, propertyDescriptor));
			}
		}
		public override PropertyDescriptorCollection GetProperties() {
			return properties;
		}
	}
	internal class CategorizedListEditorPropertyDescriptor : PropertyDescriptor {
		private Type objectType;
		private PropertyDescriptor descriptor;
		private bool TryGetPropertyDescriptor(IVariablePropertiesCategorizedItem obj, out PropertyDescriptor propertyDescriptor) {
			try {
				propertyDescriptor = obj.GetPropertyDescriptorContainer().PropertyDescriptors[descriptor.Name];
			}
			catch {
				propertyDescriptor = null;
			}
			return propertyDescriptor != null;
		}
		public CategorizedListEditorPropertyDescriptor(Type objectType, PropertyDescriptor descriptor)
			: base(descriptor) {
			this.objectType = objectType;
			this.descriptor = descriptor;
		}
		public override object GetValue(object theObject) {
			IVariablePropertiesCategorizedItem obj = (IVariablePropertiesCategorizedItem)theObject;
			PropertyDescriptor propertyDescriptor;
			if(TryGetPropertyDescriptor(obj, out propertyDescriptor)) {
				return propertyDescriptor.GetValue(obj.PropertyValueStore);
			}
			return null;
		}
		public override void SetValue(object theObject, object theValue) {
			IVariablePropertiesCategorizedItem obj = (IVariablePropertiesCategorizedItem)theObject;
			PropertyDescriptor propertyDescriptor;
			if(TryGetPropertyDescriptor(obj, out propertyDescriptor)) {
				propertyDescriptor.SetValue(obj.PropertyValueStore, theValue);
			}
		}
		public override bool CanResetValue(object component) {
			return descriptor.CanResetValue(component);
		}
		public override void ResetValue(object component) {
			descriptor.ResetValue(component);
		}
		public override bool ShouldSerializeValue(object component) {
			return descriptor.ShouldSerializeValue(component);
		}
		public override Type ComponentType {
			get { return objectType; }
		}
		public override Type PropertyType {
			get { return descriptor.PropertyType; }
		}
		public override bool IsReadOnly {
			get { return descriptor.IsReadOnly; }
		}
	}
}
