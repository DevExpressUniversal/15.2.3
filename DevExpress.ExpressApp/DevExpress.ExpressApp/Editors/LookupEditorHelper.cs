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
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Core;
namespace DevExpress.ExpressApp.Editors {
	public interface ILookupPopupFrameTemplate : IFrameTemplate {
		void SetStartSearchString(string searchString);
		bool IsSearchEnabled {
			get;
			set;
		}
		void FocusFindEditor();
	}
	public class CustomCreateCollectionSourceEventArgs : EventArgs {
		public CollectionSourceBase CollectionSource { get; set; }
	}
	public class LookupEditorHelper : ObjectEditorHelperBase {
		public const string PermanentCriteriaName = "PermanentLookupCriteria";
		public static int GetDisplayNameAvailableObjectsMax = 50;
		private Int32 smallCollectionItemCount = 25;
		private String dataSourceProperty = "";
		private DataSourcePropertyIsNullMode dataSourcePropertyIsNullMode;
		private String dataSourcePropertyIsNullCriteria;
		protected String dataSourceCriteria = "";
		protected IMemberInfo dataSourceCriteriaMemberInfo;
		private IObjectSpace objectSpace;
		private XafApplication application;
		private IModelListView lookupListViewModel;
		private IModelMemberViewItem viewItemModel;
		private LookupEditorMode editorMode;
		private IModelView view;
		private long prevCollectionHashCode = 0;
		private long prevObjectHashCode = 0;
		private bool canFilter = false;
		private void SetCollectionSourceDefaultFilter(CollectionSourceBase result, Object editingObject) {
			if(CanFilter(result, editingObject)) {
				if(editorMode == LookupEditorMode.AllItemsWithSearch) {
					result.Criteria[FilterController.FullTextSearchCriteriaName] = null;
				}
				else {
					result.Criteria[FilterController.FullTextSearchCriteriaName] = CollectionSourceBase.EmptyCollectionCriteria;
				}
			}
		}
		private long GetObjectHashCode(Object obj) {
			return (obj == null) ? 0 : obj.GetHashCode();
		}
		private long GetCollectionHashCode(CollectionSourceBase collectionSource) {
			collectionSource.InitCollection();
			if(collectionSource.OriginalCollection != null) {
				return collectionSource.OriginalCollection.GetHashCode();
			}
			else {
				return 0;
			}
		}
		private bool CanFilter(CollectionSourceBase collectionSource, Object editingObject) {
			if( 
					(editingObject == null) || (prevObjectHashCode != GetObjectHashCode(editingObject))
					||
					!String.IsNullOrEmpty(dataSourceProperty) && (prevCollectionHashCode != GetCollectionHashCode(collectionSource))
			) {
				prevCollectionHashCode = GetCollectionHashCode(collectionSource);
				prevObjectHashCode = GetObjectHashCode(editingObject);
				canFilter = collectionSource.CanApplyCriteria;
				if(canFilter) {
					if(editorMode == LookupEditorMode.Auto) {
						if(ReferenceEquals(collectionSource.Criteria[FilterController.FullTextSearchCriteriaName], CollectionSourceBase.EmptyCollectionCriteria)) {
							canFilter = true;
						}
						else {
							canFilter = (collectionSource.Collection != null) && (collectionSource.GetCount() >= smallCollectionItemCount);
						}
					}
					else if(editorMode == LookupEditorMode.AllItems) {
						canFilter = false;
					}
					else {
						canFilter = IsSearchEditorMode();
					}
				}
			}
			return canFilter;
		}
		private void ApplyCriteria(CollectionSourceBase listViewCollectionSource, CollectionSourceBase collectionSource) {
			collectionSource.Criteria.Clear();
			listViewCollectionSource.Criteria.CopyTo(collectionSource.Criteria);
			FilterWithObjectsProcessor criteriaProcessor = new FilterWithObjectsProcessor(ObjectSpace);
			foreach(CriteriaOperator criteriaOperator in collectionSource.Criteria.Values) {
				criteriaProcessor.Process(criteriaOperator, FilterWithObjectsProcessorMode.ObjectToObject);
			}
		}
		public bool IsSearchEditorMode() {
			if(editorMode == LookupEditorMode.Search || editorMode == LookupEditorMode.AllItemsWithSearch || smallCollectionItemCount == 0) {
				return true;
			}
			return false;
		}
		private void AddCriteriaDependentPropeties(List<String> properties) {
			CriteriaWrapper criteriaWrapper = null;
			if(IsPropertyDataSource) {
				if(dataSourcePropertyIsNullMode == DataSourcePropertyIsNullMode.CustomCriteria) {
					if(!String.IsNullOrEmpty(dataSourcePropertyIsNullCriteria)) {
						criteriaWrapper = InitCriteriaWrapper(LookupObjectType, dataSourcePropertyIsNullCriteria, null, null);
						AddPropeties(properties, criteriaWrapper);
					}
				}
			}
			criteriaWrapper = InitCriteriaWrapper(LookupObjectType, dataSourceCriteria, null, null);
			AddPropeties(properties, criteriaWrapper);
		}
		private void AddPropeties(List<String> properties, CriteriaWrapper criteriaWrapper) {
			if(criteriaWrapper != null) {
				foreach(String property in criteriaWrapper.EditableParameters.Keys) {
					if(!properties.Contains(property)) {
						properties.Add(property);
					}
				}
			}
		}
		private CollectionSourceBase CreateCollectionSourceCore(Object editingObject, Boolean strictlyClientMode) {
			CollectionSourceBase result = null;
			CriteriaWrapper criteriaWrapper = null;
			if(IsPropertyDataSource && (editingObject != null)) {
				ITypeInfo editingTypeInfo = XafTypesInfo.Instance.FindTypeInfo(editingObject.GetType());
				IMemberInfo dataSourceMemberDescriptor = editingTypeInfo.FindMember(dataSourceProperty);
				if(dataSourceMemberDescriptor == null) {
					throw new MemberNotFoundException(editingObject.GetType(), dataSourceProperty);
				}
				CollectionSourceMode collectionSourceMode = CollectionSourceMode.Normal;
				if(application != null) {
					collectionSourceMode = application.DefaultCollectionSourceMode;
				}
				if(dataSourceMemberDescriptor.IsList
					&& dataSourceMemberDescriptor.ListElementTypeInfo != null
					&& dataSourceMemberDescriptor.ListElementTypeInfo.IsInterface
					&& dataSourceMemberDescriptor.ListElementTypeInfo.IsDomainComponent) {
					collectionSourceMode = CollectionSourceMode.Proxy;
				}
				result = new LookupEditPropertyCollectionSource(
					objectSpace, editingObject.GetType(), editingObject, dataSourceMemberDescriptor,
					dataSourcePropertyIsNullMode, dataSourcePropertyIsNullCriteria, collectionSourceMode);
				criteriaWrapper = InitCriteriaWrapper(dataSourceMemberDescriptor.ListElementType, dataSourceCriteria, dataSourceCriteriaMemberInfo, editingObject);
			}
			else {
				String listViewID = null;
				if((LookupListViewModel != null) && (LookupListViewModel.Id != null)) {
					listViewID = LookupListViewModel.Id;
				}
				CollectionSourceMode collectionSourceMode = CollectionSourceMode.Normal;
				if(application != null) {
					collectionSourceMode = application.DefaultCollectionSourceMode;
				}
				if(LookupObjectTypeInfo.IsInterface && LookupObjectTypeInfo.IsDomainComponent) {
					collectionSourceMode = CollectionSourceMode.Proxy;
				}
				result = CreateCollectionSourceCore(ObjectSpace, LookupObjectType, listViewID, collectionSourceMode, strictlyClientMode);
				criteriaWrapper = InitCriteriaWrapper(LookupObjectType, dataSourceCriteria, dataSourceCriteriaMemberInfo, editingObject);
			}
			result.SetIsCriteriaLocked(true);
			SetCriteriaToCollectionSource(result, criteriaWrapper);
			return result;
		}
		private CollectionSourceBase CreateCollectionSourceCore(IObjectSpace objectSpace, Type lookupObjectType, String listViewID, CollectionSourceMode collectionSourceMode, Boolean strictlyClientMode) {
			CustomCreateCollectionSourceEventArgs args = new CustomCreateCollectionSourceEventArgs();
			if(CustomCreateCollectionSource != null) {
				CustomCreateCollectionSource(this, args);
			}
			if(args.CollectionSource == null) {
				if(strictlyClientMode) {
					args.CollectionSource = Application.CreateCollectionSource(objectSpace, lookupObjectType, listViewID, CollectionSourceDataAccessMode.Client, collectionSourceMode);
				}
				else {
					args.CollectionSource = Application.CreateCollectionSource(objectSpace, lookupObjectType, listViewID, collectionSourceMode);
				}
			}
			return args.CollectionSource;
		}
		protected void SetCriteriaToCollectionSource(CollectionSourceBase collectionSource, CriteriaWrapper criteriaWrapper) {
			if(criteriaWrapper != null) {
				collectionSource.Criteria[PermanentCriteriaName] = criteriaWrapper.CriteriaOperator;
			}
		}
		protected CriteriaWrapper InitCriteriaWrapper(Type objectType, String criteriaString, IMemberInfo criteriaMemberInfo, Object editingObject) {
			CriteriaWrapper result = null;
			if(criteriaMemberInfo != null) {
				CriteriaOperator criteria = (CriteriaOperator)criteriaMemberInfo.GetValue(editingObject);
				if((editingObject != null) && (objectType != null)) {
					result = new LocalizedCriteriaWrapper(objectType, criteria, editingObject);
				}
				else if(objectType != null) {
					result = new LocalizedCriteriaWrapper(objectType, criteria);
				}
			}
			else if(!String.IsNullOrEmpty(criteriaString)) {
				if((editingObject != null) && (objectType != null)) {
					result = new LocalizedCriteriaWrapper(objectType, criteriaString, editingObject);
				}
				else if(objectType != null) {
					result = new LocalizedCriteriaWrapper(objectType, criteriaString);
				}
			}
			return result;
		}
		public LookupEditorHelper(XafApplication application, IObjectSpace objectSpace, ITypeInfo lookupObjectTypeInfo, IModelMemberViewItem viewItemModel)
			: base(lookupObjectTypeInfo, viewItemModel) {
			this.application = application;
			this.viewItemModel = viewItemModel;
			SetObjectSpace(objectSpace);
			if(viewItemModel != null) {
				if(viewItemModel.Application != null) {
					smallCollectionItemCount = viewItemModel.Application.Options.LookupSmallCollectionItemCount;
				}
				dataSourceProperty = viewItemModel.DataSourceProperty;
				dataSourcePropertyIsNullMode = viewItemModel.DataSourcePropertyIsNullMode;
				dataSourcePropertyIsNullCriteria = viewItemModel.DataSourcePropertyIsNullCriteria;
				dataSourceCriteria = viewItemModel.DataSourceCriteria;
				if((viewItemModel.ParentView != null) && (viewItemModel.ParentView.AsObjectView != null) && (viewItemModel.ParentView.AsObjectView.ModelClass != null)) {
					dataSourceCriteriaMemberInfo = viewItemModel.ParentView.AsObjectView.ModelClass.TypeInfo.FindMember(viewItemModel.DataSourceCriteriaProperty);
				}
				editorMode = viewItemModel.LookupEditorMode;
				view = viewItemModel.View;
			}
		}
		public void SetObjectSpace(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
		public CollectionSourceBase CreateCollectionSource(Object editingObject) {
			CollectionSourceBase result = CreateCollectionSourceCore(editingObject, false);
			SetCollectionSourceDefaultFilter(result, editingObject);
			return result;
		}
		public void SetDataType(Object editingObject) {
			if(editingObject != null && viewItemModel != null && viewItemModel.ModelMember != null && viewItemModel.ModelMember.MemberInfo != null) {
				DataTypePropertyAttribute dataTypePropertyAttribute = viewItemModel.ModelMember.MemberInfo.FindAttribute<DataTypePropertyAttribute>();
				if(dataTypePropertyAttribute != null) {
					ITypeInfo memberOwnerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(editingObject.GetType());
					IMemberInfo dataTypeMemberInfo = memberOwnerTypeInfo.FindMember(dataTypePropertyAttribute.Name);
					if(dataTypeMemberInfo != null) {
						Type dataType = (Type)dataTypeMemberInfo.GetValue(editingObject);
						view = application.FindModelView(application.FindLookupListViewId(dataType));
						lookupPropertyName = XafTypesInfo.Instance.FindTypeInfo(dataType).DefaultMember.Name;
						LookupObjectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(dataType);
					}
				}
			}
		}
		public ListView CreateListView(Object editingObject) {
			return CreateListView(editingObject, null);
		}
		public Boolean CanFilterDataSource(CollectionSourceBase listViewCollectionSource, Object editingObject) {
			using(CollectionSourceBase collectionSource = CreateCollectionSourceCore(editingObject, true)) {
				if(listViewCollectionSource != null) {
					ApplyCriteria(listViewCollectionSource, collectionSource);
				}
				return CanFilter(collectionSource, editingObject);
			}
		}
		public IModelTemplate GetLookupTemplateModel(IFrameTemplate template) {
			return application.GetTemplateCustomizationModel(template);
		}
		public IModelListView LookupListViewModel {
			get {
				if(lookupListViewModel == null) {
					if(view != null) {
						lookupListViewModel = view as IModelListView;
					}
					else {
						lookupListViewModel = application.FindModelView(application.FindLookupListViewId(LookupObjectType)) as IModelListView;
					}
				}
				return lookupListViewModel;
			}
			set {
				lookupListViewModel = value;
			}
		}
		public IModelMemberViewItem Model {
			get { return viewItemModel; }
		}
		public Boolean IsPropertyDataSource {
			get { return !string.IsNullOrEmpty(dataSourceProperty); }
		}
		public IList<String> MasterProperties {
			get {
				List<String> result = new List<String>();
				if(IsPropertyDataSource) {
					if(dataSourceProperty.LastIndexOf('.') > 0) {
						result.Add(dataSourceProperty.Remove(dataSourceProperty.LastIndexOf('.')));
					}
				}
				if(viewItemModel.PropertyName.LastIndexOf('.') > 0) {
					result.Add(viewItemModel.PropertyName.Remove(viewItemModel.PropertyName.LastIndexOf('.')));
				}
				AddCriteriaDependentPropeties(result);
				return result.ToArray();
			}
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public Int32 SmallCollectionItemCount {
			get { return smallCollectionItemCount; }
			set {
				smallCollectionItemCount = value;
			}
		}
		public IModelView LookupView {
			get {
				return view;
			}
			set {
				view = value;
			}
		}
		public XafApplication Application {
			get { return application; }
		}
		public LookupEditorMode EditorMode {
			get {
				return editorMode;
			}
			set {
				prevCollectionHashCode = 0;
				prevObjectHashCode = 0;
				editorMode = value;
			}
		}
		public event EventHandler<CustomCreateCollectionSourceEventArgs> CustomCreateCollectionSource;
		public ListView CreateListView(object editingObject, ListEditor listEditor) {
			if(LookupListViewModel == null) {
				throw new InvalidOperationException("LookupListViewModel == null");
			}
			CollectionSourceBase collectionSource = CreateCollectionSourceCore(editingObject, false);
			ListView listView = application.CreateListView(LookupListViewModel, collectionSource, false, listEditor);
			SetCollectionSourceDefaultFilter(collectionSource, editingObject);
			return listView;			
		}
	}
}
