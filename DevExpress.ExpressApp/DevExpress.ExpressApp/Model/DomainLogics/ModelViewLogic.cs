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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.DomainLogics {
	[DomainLogic(typeof(IModelViews))]
	public static class ModelViewsDomainLogic {
		public const string DataSourcePropertyPath = "Application.Views";
		public static IModelDetailView GetDefaultDetailView<T>(IModelViews modelViews) {
			return (IModelDetailView)modelViews[ModelDetailViewNodesGenerator.GetDetailViewId(typeof(T))];
		}
		public static IModelListView GetDefaultListView<T>(IModelViews modelViews) {
			return (IModelListView)modelViews[ModelListViewNodesGenerator.GetListViewId(typeof(T))];
		}
	}
	[DomainLogic(typeof(IModelView))]
	public static class ModelViewDomainLogic {
		public static IModelObjectView Get_AsObjectView(IModelView modelView) {
			return modelView as IModelObjectView;
		}
	}
	[DomainLogic(typeof(IModelListView))]
	public static class ModelListViewLogic {
		public static bool Get_FilterEnabled(IModelListView modelView) {
			return !String.IsNullOrEmpty(modelView.Filter);
		}
		public static bool Get_AllowEdit(IModelListView modelView) {
			return modelView.ModelClass != null ? modelView.ModelClass.DefaultListViewAllowEdit : false;
		}
		public static void Set_FreezeColumnIndices(IModelListView modelView, bool value) {
			if(!((ModelNode)modelView).IsNewValueModified) {
				if(value) {
					foreach(IModelColumn column in modelView.Columns) {
						((ModelNode)column).SetValueCore<int?>(ModelValueNames.Index, column.Index, false);
					}
				}
				else {
					foreach(IModelColumn column in modelView.Columns) {
						if(!((ModelNode)column).IsNewValueModified) {
							((ModelNode)column).ClearValue(ModelValueNames.Index);
						}
					}
				}
			}
			((ModelNode)modelView).SetValue<bool>("FreezeColumnIndices", value);
		}
		public static Boolean Get_UseServerMode(IModelListView listViewModel) {
			return (listViewModel.DataAccessMode == CollectionSourceDataAccessMode.Server);
		}
		public static void Set_UseServerMode(IModelListView listViewModel, Boolean value) {
			listViewModel.DataAccessMode = value ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client;
		}
	}
	[DomainLogic(typeof(IModelDetailView))]
	public static class ModelDetailViewLogic {
		public static bool Get_AllowEdit(IModelDetailView modelView) {
			return true;
		}
		public static void Set_FreezeLayout(IModelDetailView modelView, bool value) {
			if(!((ModelNode)modelView).IsNewValueModified) {
				Guard.Assert(!((ModelNode)modelView).IsMaster, "'modelView' node must be a layer (not a master)");  
				if(value) {
					ModelNode copiedLayout = ((ModelNode)modelView.Layout).Clone("Layout_Copy");
					modelView.Layout.Remove();
					copiedLayout.Id = "Layout";
				}
				else {
					((ModelNode)modelView.Layout).UndoSelf();
				}
				((ModelNode)modelView).Master.Reset();
			}
			((ModelNode)modelView).SetValue<bool>("FreezeLayout", value);
		}
		private static void CloneAllLayout(IModelViewLayout layout) {
			List<ModelNode> ferstLevel = new List<ModelNode>();
			foreach(IModelViewLayoutElement item in ModelLayoutGroupLogic.GetLayoutItems<IModelViewLayoutElement>(layout)) {
				if(((ModelNode)item).Parent is IModelViewLayout) {
					ferstLevel.Add((ModelNode)item);
				}
			}
			ModelNode node = (ModelNode)layout;
			foreach(ModelNode item in ferstLevel) {
				ModelNode newItem = item.Clone("newItem");
				((IModelNode)item).Remove();
				newItem.Id = item.Id;
			}
		}
	}
	[DomainLogic(typeof(IModelObjectView))]
	public static class ModelObjectViewLogic {
		public const string ModelViewsByClassCriteria = "(AsObjectView Is Not Null) And (AsObjectView.ModelClass Is Not Null) And ('@This.ModelClass' Is Not Null) And (IsAssignableFromViewModelClass('@This.ModelClass.TypeInfo', AsObjectView))";
		public static String Get_Caption(IModelObjectView modelView) {
			return modelView.ModelClass != null ? modelView.ModelClass.Caption : "";
		}
	}
	[DomainLogic(typeof(IModelMemberViewItem))]
	public class ModelMemberViewItemDomainLogic {
		private string cachedPropertyName;
		private IModelMember cachedModelMember;
		public IModelMember Get_ModelMember(IModelMemberViewItem model) {
			string propertyName = model.PropertyName;
			if(cachedModelMember == null || propertyName != cachedPropertyName) {
				cachedPropertyName = propertyName;
				IModelObjectView modelObjectView = model.ParentView as IModelObjectView;
				IModelClass modelClass = modelObjectView != null ? modelObjectView.ModelClass : null;
				cachedModelMember = modelClass != null && !string.IsNullOrEmpty(propertyName) ? modelClass.FindMember(propertyName) : null;
			}
			return cachedModelMember;
		}
		public static int Get_MaxLength(IModelMemberViewItem model) {
			if(model.ModelMember != null) {
				int result = model.ModelMember.Size;
				return result < 0 ? 0 : result;
			}
			return int.MaxValue;
		}
		public static int Get_ImageEditorCustomHeight(IModelMemberViewItem model) {
			if(model.ModelMember != null) {
				return model.ModelMember.ListViewImageEditorCustomHeight;
			}
			return int.MaxValue;
		}
		public static int Get_ImageEditorFixedWidth(IModelMemberViewItem model) {
			if(model.ModelMember != null) {
				return model.ModelMember.DetailViewImageEditorFixedWidth;
			}
			return int.MaxValue;
		}
		public static int Get_ImageEditorFixedHeight(IModelMemberViewItem model) {
			if(model.ModelMember != null) {
				return model.ModelMember.DetailViewImageEditorFixedHeight;
			}
			return int.MaxValue;
		}
		public static DevExpress.Persistent.Base.ImageEditorMode Get_ImageEditorMode(IModelMemberViewItem model) {
			if(model.ModelMember != null) {
				if(model is IModelPropertyEditor) {
					return model.ModelMember.DetailViewImageEditorMode;
				}
				if(model is IModelColumn) {
					return model.ModelMember.ListViewImageEditorMode;
				}
			}
			return ImageEditorMode.PictureEdit;
		}
		public static IModelList<IModelView> Get_Views(IModelMemberViewItem model) {
			return new CalculatedModelNodeList<IModelView>(ViewNamesCalculator.GetViews(model, false));
		}
		public static IModelView Get_ParentView(IModelMemberViewItem modelItem) {
			IModelView result = null;
			IModelNode parent = modelItem.Parent;
			while(parent != null) {
				result = parent as IModelView;
				if(result != null) {
					break;
				}
				parent = parent.Parent;
			}
			return result;
		}
		private static String CorrectPropertyName(IModelMemberViewItem modelMemberViewItem, String propertyName) {
			String result = "";
			if(!String.IsNullOrEmpty(propertyName)) {
				if(modelMemberViewItem.ParentView is IModelObjectView) {
					ITypeInfo typeInfo = ((IModelObjectView)modelMemberViewItem.ParentView).ModelClass.TypeInfo;
					List<IMemberInfo> members = new List<IMemberInfo>(typeInfo.FindMember(modelMemberViewItem.PropertyName).GetPath());
					if(members.Count > 1) {
						members.RemoveAt(members.Count - 1);
						foreach(IMemberInfo memberInfo in members) {
							result += memberInfo.Name + ".";
						}
					}
					result += propertyName;
				}
			}
			return result;
		}
		public static String Get_DataSourceProperty(IModelMemberViewItem modelMemberViewItem) {
			String result = "";
			if(modelMemberViewItem.ModelMember != null) {
				result = CorrectPropertyName(modelMemberViewItem, modelMemberViewItem.ModelMember.DataSourceProperty);
			}
			return result;
		}
		public static String Get_DataSourceCriteriaProperty(IModelMemberViewItem modelMemberViewItem) {
			String result = "";
			if(modelMemberViewItem.ModelMember != null) {
				result = CorrectPropertyName(modelMemberViewItem, modelMemberViewItem.ModelMember.DataSourceCriteriaProperty);
			}
			return result;
		}
		public static Boolean Get_AllowClear(IModelMemberViewItem modelMemberViewItem) {
			Boolean result = true;
			if(modelMemberViewItem.ModelMember != null) {
				result = modelMemberViewItem.ModelMember.AllowClear;
			}
			return result;
		}
	}
	public static class ViewNamesCalculator {
		private static Dictionary<string, List<IModelView>> viewCache = new Dictionary<string, List<IModelView>>();
		private static Dictionary<Type, string> modelClassIdByType = new Dictionary<Type, string>();
		private static bool viewCacheEnabled = false;
		public static IList<IModelView> GetViews(IModelMemberViewItem modelMemberViewItem, bool getListViewsOnly) {
			Guard.ArgumentNotNull(modelMemberViewItem, "modelMemberViewItem");
			IModelMember modelMember = modelMemberViewItem.ModelMember;
			if(modelMember != null) {
				bool collectViewsForAllClasses = false;
				string modelClassId = null;
				Type elementType = null;
				if((modelMember.MemberInfo != null) && modelMember.MemberInfo.IsList) {
					ITypeInfo listElementTypeInfo = modelMember.MemberInfo.ListElementTypeInfo;
					if(listElementTypeInfo != null && listElementTypeInfo.Type != null) {
						elementType = listElementTypeInfo.Type;
					}
					else {
						collectViewsForAllClasses = true;
					}
				}
				else {
					elementType = modelMember.Type;
				}
				if(elementType != null) {
					if(!CacheEnabled || !modelClassIdByType.TryGetValue(elementType, out modelClassId)) {
						IModelClass modelClass = modelMemberViewItem.Application.BOModel.GetClass(elementType);
						if(modelClass != null) {
							modelClassId = ((ModelNode)modelClass).Id;
						}
						if(CacheEnabled) {
							modelClassIdByType[elementType] = modelClassId;
						}
					}
				}
				bool targetViewTypeIsListView = !(typeof(DetailPropertyEditor).IsAssignableFrom(modelMemberViewItem.PropertyEditorType) || typeof(IObjectPropertyEditor).IsAssignableFrom(modelMemberViewItem.PropertyEditorType));
				if(CacheEnabled && !collectViewsForAllClasses) {
					if(viewCache.Count == 0) {
						CreateViewsCache(modelMemberViewItem, targetViewTypeIsListView);
					}
					return GetInCache(modelClassId, getListViewsOnly, collectViewsForAllClasses, targetViewTypeIsListView);
				}
				else {
					if(collectViewsForAllClasses) {
						return new List<IModelView>(EnumerateTargetViews(modelMemberViewItem.Application.Views, targetViewTypeIsListView)).AsReadOnly();
					}
					else if(!string.IsNullOrEmpty(modelClassId) && (targetViewTypeIsListView || !getListViewsOnly)) {
						List<IModelView> list = new List<IModelView>();
						foreach(IModelView modelView in EnumerateTargetViews(modelMemberViewItem.Application.Views, targetViewTypeIsListView)) {
							IModelClass modelViewClass = ((IModelObjectView)modelView).ModelClass;
							if(modelViewClass != null && modelViewClass.TypeInfo.IsAssignableFrom(XafTypesInfo.Instance.FindTypeInfo(elementType))) {
								list.Add(modelView);
							}
						}
						return SortByInheritanceHierarchy(list, modelMemberViewItem.Application.BOModel.GetClass(elementType)).AsReadOnly();
					}
				}
			}
			return new IModelView[0];
		}
		public static List<IModelView> SortByInheritanceHierarchy(List<IModelView> list, IModelClass modelClass) {
			List<IModelView> result = new List<IModelView>();
			IModelClass currentClass = modelClass;
			while(currentClass != null) {
				foreach(IModelObjectView modelView in list) {
					if(modelView.ModelClass != null && currentClass.Name == modelView.ModelClass.Name) {
						result.Add(modelView);
					}
				}
				currentClass = currentClass.BaseClass;
			}
			return result;
		}
		private static IEnumerable<IModelView> EnumerateTargetViews(IModelViews modelViews, bool targetViewTypeIsListView) {
			foreach(IModelView modelView in modelViews) {
				if((targetViewTypeIsListView && modelView is IModelListView) || (!targetViewTypeIsListView && modelView is IModelDetailView)) {
					yield return modelView;
				}
			}
		}
		private static IList<IModelView> GetInCache(string modelClassId, bool getListViewsOnly, bool collectViewsForAllClasses, bool targetViewTypeIsListView) {
			if(!string.IsNullOrEmpty(modelClassId) && (targetViewTypeIsListView || !getListViewsOnly)) {
				List<IModelView> internalResult = null;
				List<IModelView> result = new List<IModelView>();
				if(viewCache.TryGetValue(modelClassId, out internalResult)) {
					foreach(IModelView modelView in internalResult) {
						if((targetViewTypeIsListView && modelView is IModelListView) || (!targetViewTypeIsListView && modelView is IModelDetailView)) {
							result.Add(modelView);
						}
					}
				}
				return result.AsReadOnly();
			}
			return new IModelView[0];
		}
		private static void CreateViewsCache(IModelMemberViewItem modelMemberViewItem, bool targetViewTypeIsListView) {
			modelClassIdByType = new Dictionary<Type, string>();
			foreach(IModelView modelView in modelMemberViewItem.Application.Views) {
				if(modelView is IModelObjectView) {
					IModelClass modelViewClass = ((IModelObjectView)modelView).ModelClass;
					if(modelViewClass != null) {
						string modelViewClassId = ((ModelNode)modelViewClass).Id;
						if(!viewCache.ContainsKey(modelViewClassId)) {
							viewCache[modelViewClassId] = new List<IModelView>();
						}
						viewCache[modelViewClassId].Add(modelView);
					}
				}
			}
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		internal static bool CacheEnabled {
			get {
				return viewCacheEnabled;
			}
			set {
				viewCacheEnabled = value;
				if(!viewCacheEnabled) {
					ClearCache();
				}
			}
		}
		private static void ClearCache() {
			modelClassIdByType.Clear();
			foreach(List<IModelView> item in viewCache.Values) {
				item.Clear();
			}
			viewCache.Clear();
		}
	}
	[DomainLogic(typeof(IModelColumn))]
	public static class ModelColumnDomainLogic {
		public static DevExpress.Persistent.Base.ImageEditorMode Get_ImageEditorMode(IModelColumn model) {
			if(model.ModelMember != null) {
				return model.ModelMember.ListViewImageEditorMode;
			}
			return ImageEditorMode.PictureEdit;
		}
		public static GroupInterval Get_GroupInterval(IModelColumn model) {
			if(model.ModelMember != null) {
				return model.ModelMember.GroupInterval;
			}
			return GroupInterval.None;
		}
		public static int? Get_Index(IModelColumn model) {
			if(model.Parent != null && model.Parent.Parent != null) {
				IModelListView listView = model.Parent.Parent as IModelListView;
				if(listView != null && !listView.FreezeColumnIndices) {
					int? generatedIndex = model.GetValue<int?>(ModelListViewColumnsNodesGenerator.GeneratedIndexValueName);
					if(generatedIndex != null && generatedIndex.HasValue) {
						return generatedIndex.Value;
					}
					return null;
				}
			}
			return -1;
		}
		public static Type Get_PropertyEditorType(IModelColumn model) {
			if(model.ModelMember != null) {
				string columnFieldName = model.FieldName;
				if(columnFieldName != model.ModelMember.Name && ((IModelListView)model.ParentView).DataAccessMode == CollectionSourceDataAccessMode.DataView) {
					IModelMember fieldMember = model.ModelMember.ModelClass.FindMember(columnFieldName);
					if(fieldMember != null) {
						return ModelMemberLogic.Get_PropertyEditorType(fieldMember);
					}
				}
				return model.ModelMember.PropertyEditorType;
			}
			return null;
		}
		public static string Get_FieldName(IModelColumn model) {
			if(model.ModelMember != null) {
				MediaDataObjectAttribute mediaDataAttribute = model.ModelMember.MemberInfo.MemberTypeInfo.FindAttribute<MediaDataObjectAttribute>(false);
				if(mediaDataAttribute != null) {
					return model.PropertyName + "." + mediaDataAttribute.MediaDataBindingProperty;
				}
				else {
					ITypeInfo classTypeInfo = ((IModelListView)(model.ParentView)).ModelClass.TypeInfo;
					IMemberInfo displayableMemberDescriptor = ReflectionHelper.FindDisplayableMemberDescriptor(classTypeInfo, model.PropertyName);
					if(displayableMemberDescriptor != null) {
						if(!string.IsNullOrEmpty(model.LookupProperty)) {
							IMemberInfo lookupPropertyMemberDescriptor = ReflectionHelper.FindDisplayableMemberDescriptor(displayableMemberDescriptor.MemberTypeInfo, model.LookupProperty);
							if(lookupPropertyMemberDescriptor != null) {
								displayableMemberDescriptor = XafTypesInfo.Instance.CreatePath(displayableMemberDescriptor, lookupPropertyMemberDescriptor);
							}
						}
						return displayableMemberDescriptor.BindingName;
					}
					return model.PropertyName;
				}
			}
			else {
				return model.PropertyName;
			}
		}
	}
	[DomainLogic(typeof(IModelColumns))]
	public static class ModelColumnsDomainLogic {
		public static int? Get_Index(IModelColumns model) {
			return 0;
		}
		public static IList<IModelColumn> GetVisibleColumns(IModelColumns model) {
			IList<IModelColumn> visibleColumns = new List<IModelColumn>();
			foreach(IModelColumn column in model) {
				if(!column.Index.HasValue || column.Index > -1) {
					visibleColumns.Add(column);
				}
			}
			return visibleColumns;
		}
		public static IList<IModelColumn> Get_VisibleColumns(IModelColumns model) {
			return GetVisibleColumns(model);
		}
	}
}
