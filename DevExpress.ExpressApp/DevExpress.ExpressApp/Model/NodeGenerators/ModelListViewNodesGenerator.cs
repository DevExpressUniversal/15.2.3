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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelListViewNodesGenerator {
		private const string ListViewIdSuffix = "_ListView";
		public static string GetListViewId(Type type) {
			return ModelNodesGeneratorSettings.GetIdPrefix(type) + ListViewIdSuffix;
		}
		public static void GenerateModel(IModelViews views, IModelClass classInfo) {
			Guard.ArgumentNotNull(views, "views");
			Guard.ArgumentNotNull(classInfo, "classInfo");
			if(classInfo.TypeInfo == null) {
				throw new InvalidOperationException("classInfo.TypeInfo == null");
			}
			IModelListView listView = views.AddNode<IModelListView>(GetListViewId(classInfo.TypeInfo.Type));
			listView.ModelClass = classInfo;
			GenerateNodes(listView, classInfo);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void GenerateNodes(IModelListView listViewInfo, IModelClass modelClass) {
			if(listViewInfo.Columns == null) {
				((ModelNode)listViewInfo).AddNode<IModelColumns>("Columns");
			}
			if(listViewInfo.BandsLayout == null) {
				((ModelNode)listViewInfo).AddNode<IModelBandsLayout>("BandsLayout");
			}
			if((String.IsNullOrEmpty(listViewInfo.Id) || !((ModelNode)listViewInfo).HasValue(((ModelNode)listViewInfo).KeyValueName))) {
				listViewInfo.Id = GetListViewId(modelClass.TypeInfo.Type);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void GenerateNodes(IModelListView listViewInfo) {
			ModelNodesDefaultInterfaceGenerator.Instance.GenerateNodes((ModelNode)listViewInfo);
		}
	}
	public enum ViewGenerationMode { None, ListView, LookupListView }
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[ModelGenerateContentAction]
	public class ModelListViewColumnsNodesGenerator : ModelListViewColumnsNodesGeneratorBase<IModelColumn, IModelColumn> {
		protected override IModelList<IModelColumn> GetColumns(IModelListView listViewModel) {
			return listViewModel.Columns;
		}
		protected override IModelColumn GetElementByName(IModelList<IModelColumn> columns, string propertyName) {
			return columns[propertyName];
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class ModelListViewColumnsNodesGeneratorBase<ColumnType, CollectionItemType> : ModelNodesGeneratorBase where ColumnType : IModelColumn where CollectionItemType : IModelNode {
		public static Int32 MaxRecursionDeep = 40;
		private Int32 generateColumnsCallCount = 0;
		internal const string GeneratedIndexValueName = "GeneratedIndex";
		private IMemberInfo parentMemberInfo;
		private int displayMemberColumnWidth = 400;
		private int shortColumnWidth = 50;
		private int defaultColumnWidth = 70;
		private ViewGenerationMode generationMode = ViewGenerationMode.None;
		private bool IsShortValueType(Type type) {
			return (type == typeof(int)) || (type == typeof(float))
				|| (type == typeof(double)) || (type == typeof(decimal))
				|| (type == typeof(char)) || (type == typeof(bool));
		}
		private int GetDefaultColumnWidthInternal(IModelMember propertyNode) {
			bool isDisplayMember = ((IModelClass)propertyNode.Parent.Parent).DefaultProperty == propertyNode.Name;
			if(isDisplayMember) {
				return displayMemberColumnWidth;
			}
			else if(IsShortValueType(propertyNode.Type)) {
				return shortColumnWidth;
			}
			return defaultColumnWidth;
		}
		private Boolean IsParentProperty(IModelMember memberInfo) {
			if((parentMemberInfo != null) && parentMemberInfo.IsAssociation) {
				IMemberInfo childMemberDescriptor = parentMemberInfo.ListElementTypeInfo.FindMember(memberInfo.Name);
				if(childMemberDescriptor == parentMemberInfo.AssociatedMemberInfo) {
					return true;
				}
			}
			return false;
		}
		private Boolean? IsVisibleInView(IModelMember memberInfo) {
			if(generationMode == ViewGenerationMode.ListView) {
				return memberInfo.IsVisibleInListView;
			}
			if(generationMode == ViewGenerationMode.LookupListView) {
				return memberInfo.IsVisibleInLookupListView;
			}
			return null;
		}
		private Boolean IsVisibleInViewTrue(IModelMember memberInfo) {
			return IsVisibleInView(memberInfo).HasValue && IsVisibleInView(memberInfo).Value;
		}
		private Boolean IsVisibleInViewFalse(IModelMember memberInfo) {
			return IsVisibleInView(memberInfo).HasValue && !IsVisibleInView(memberInfo).Value;
		}
		private Boolean IsPotentiallyVisibleColumn(IModelMember memberInfo, String keyName) {
			return !memberInfo.MemberInfo.IsDelayed && !IsVisibleInViewFalse(memberInfo) && (memberInfo.Name != keyName) && !IsParentProperty(memberInfo);
		}
		private Boolean ShouldExpandMembers(IModelMember propertyNode) {
			if(propertyNode == null) {
				throw new ArgumentNullException("propertyNode");
			}
			IMemberInfo memberDescriptor = propertyNode.MemberInfo;
			if(memberDescriptor == null) {
				return false;
			}
			ExpandObjectMembersAttribute expandMembersAttr = memberDescriptor.FindAttribute<ExpandObjectMembersAttribute>();
			if(expandMembersAttr != null && String.IsNullOrEmpty(expandMembersAttr.MemberName)) {
				return ((expandMembersAttr.ExpandingMode & ExpandObjectMembers.InListView) == ExpandObjectMembers.InListView);
			}
			Boolean isExpandInListView =
				((ExpandObjectMembersAttribute.AggregatedObjectMembersDefaultExpandingMode & ExpandObjectMembers.InListView) == ExpandObjectMembers.InListView);
			return memberDescriptor.IsAggregated && isExpandInListView;
		}
		private IModelMember GetSubstitutionMember(IModelMember propertyNode, out string substitutionMemberName) {
			substitutionMemberName = String.Empty;
			Guard.ArgumentNotNull(propertyNode, "propertyNode");
			IMemberInfo memberInfo = propertyNode.MemberInfo;
			if(memberInfo == null) {
				return null;
			}
			IModelMember result = null;
			ExpandObjectMembersAttribute expandMembersAttr = memberInfo.FindAttribute<ExpandObjectMembersAttribute>();
			if(expandMembersAttr != null && ((expandMembersAttr.ExpandingMode & ExpandObjectMembers.InListView) == ExpandObjectMembers.InListView)) {
				substitutionMemberName = string.Format("{0}.{1}", propertyNode.Name, expandMembersAttr.MemberName);
				result = propertyNode.ModelClass.FindMember(substitutionMemberName);
			}
			return result;
		}
		private void GenerateColumns(ITypeInfo generatedClassTypeInfo, IModelList<CollectionItemType> modelColumns, IModelClass modelClass, String leadingPropertyPath,
				String displayMemberName, Int32 displayMemberIndex, Boolean updateVisibleIndex, ref Int32 index) {
			if(generateColumnsCallCount >= MaxRecursionDeep) {
				throw new InfiniteRecursionException(string.Format(
					SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.InfiniteRecursionDetectedInGenerateColumns),
					modelClass.Name, generateColumnsCallCount));
			}
			try {
				generateColumnsCallCount++;
				if(!String.IsNullOrEmpty(leadingPropertyPath) && !leadingPropertyPath.EndsWith(".")) {
					leadingPropertyPath = leadingPropertyPath + ".";
				}
				Boolean hasVisibleColumns = false;
				String keyName = modelClass.KeyProperty;
				List<IMemberInfo> potentionalColumns = new List<IMemberInfo>();
				List<IModelMember> propertiesToShow = CollectPropertiesToShowInternal(modelClass);
				foreach(IModelMember member in propertiesToShow) {
					IModelMember modelMember = member;
					string memberName = modelMember.Name;
					string substitutionMemberName;
					IModelMember substitutionMember = GetSubstitutionMember(member, out substitutionMemberName);
					if(substitutionMember != null) {
						memberName = substitutionMemberName;
						modelMember = substitutionMember;
					}
					if(ShouldExpandMembers(modelMember)) {
						if(modelClass.Parent != null) {
							IModelClass aggregatedModelClass = ((IModelBOModel)modelClass.Parent).GetClass(modelMember.Type);
							if(aggregatedModelClass != null) {
								Int32 aggregateDisplayMemberIndex = -1;
								String aggregateDisplayMemberName = ModelViewsNodesGenerator.FindDisplayPropertyForTypeInternal(aggregatedModelClass);
								if(!String.IsNullOrEmpty(aggregateDisplayMemberName)) {
									aggregateDisplayMemberIndex = index;
									index++;
								}
								GenerateColumns(generatedClassTypeInfo, modelColumns, aggregatedModelClass, leadingPropertyPath + modelMember.Name, aggregateDisplayMemberName, aggregateDisplayMemberIndex, updateVisibleIndex, ref index);
							}
						}
					}
					else if(modelMember.MemberInfo.IsVisible && modelColumns[leadingPropertyPath + memberName] == null) {
						ColumnType columnInfo = CreateColumnInfo((ModelNode)modelColumns, leadingPropertyPath + memberName);
						Boolean isPotentialVisible = IsPotentiallyVisibleColumn(modelMember, keyName);
						columnInfo.Width = GetDefaultColumnWidthInternal(columnInfo.ModelMember);
						if((displayMemberIndex >= 0)
							&& ((displayMemberName == memberName) || (modelClass.DefaultProperty == memberName))
							&& !IsVisibleInViewFalse(modelMember)) {
							if(displayMemberIndex < index) {
								columnInfo.Index = displayMemberIndex;
							}
							else {
								columnInfo.Index = index;
								index++;
							}
							if(String.IsNullOrEmpty(leadingPropertyPath)
									&& typeof(IComparable).IsAssignableFrom(modelMember.MemberInfo.MemberType)) {
								columnInfo.SortIndex = 0;
								columnInfo.SortOrder = ColumnSortOrder.Ascending;
							}
							displayMemberIndex = -1;
							hasVisibleColumns = true;
						}
						else {
							if(IsVisibleInViewTrue(modelMember)
								||
								updateVisibleIndex
								&&
								(0 <= modelMember.MemberInfo.Size)
								&&
								(modelMember.MemberInfo.Size <= ModelMemberLogic.MaxShortStringLength)
								&&
								isPotentialVisible
							) {
								columnInfo.Index = index;
								index++;
								hasVisibleColumns = true;
							}
						}
						if(isPotentialVisible) {
							potentionalColumns.Add(modelMember.MemberInfo);
						}
					}
				}
				if(!hasVisibleColumns && updateVisibleIndex) {
					foreach(IMemberInfo memberInfo in potentionalColumns) {
						CollectionItemType columnInfo = modelColumns[leadingPropertyPath + memberInfo.Name];
						if(columnInfo != null) {
							columnInfo.Index = index;
							index++;
							hasVisibleColumns = true;
						}
					}
				}
				if(generatedClassTypeInfo.IsInterface) {	
					IModelClass[] interfaceNodes = ModelClassLogic.GetInterfaces(modelClass);
					foreach(IModelClass interfaceNode in interfaceNodes) {
						GenerateColumns(generatedClassTypeInfo, modelColumns, interfaceNode, leadingPropertyPath,
							displayMemberName, displayMemberIndex, !hasVisibleColumns && updateVisibleIndex, ref index);
					}
				}
				else {
					if(modelClass.BaseClass != null) {
						GenerateColumns(generatedClassTypeInfo, modelColumns, modelClass.BaseClass, leadingPropertyPath,
							displayMemberName, displayMemberIndex, !hasVisibleColumns && updateVisibleIndex, ref index);
					}
				}
			}
			finally {
				generateColumnsCallCount--;
			}
		}
		protected abstract ColumnType GetElementByName(IModelList<CollectionItemType> columns, string propertyName);
		private ColumnType AddLookupColumn(IModelListView listViewInfo, string propertyName, ref Int32 index) {
			ColumnType result = GetElementByName(GetColumns(listViewInfo), propertyName);
			if(result == null) {
				result = CreateMemberViewItemInternal<ColumnType>((ModelNode)GetColumns(listViewInfo), propertyName);
				result.Width = 50;
				result.Index = index;
				index++;
			}
			return result;
		}
		protected void GenerateColumns(ITypeInfo generatedClassTypeInfo, IModelList<CollectionItemType> columnsInfo, IModelClass classNode) {
			Int32 index = 0;
			Int32 displayMemberIndex = 0;
			String displayMemberName = ModelViewsNodesGenerator.FindDisplayPropertyForTypeInternal(classNode);
			if(!String.IsNullOrEmpty(displayMemberName)) {
				IModelMember memberInfo = classNode.FindMember(displayMemberName);
				if((memberInfo != null) && !IsVisibleInViewFalse(memberInfo)) {
					displayMemberIndex = memberInfo.Index.HasValue ? memberInfo.Index.Value : 0;
					if(displayMemberIndex == 0) {
						index = 1;
					}
				}
			}
			GenerateColumns(generatedClassTypeInfo, columnsInfo, classNode, "", displayMemberName, displayMemberIndex, true, ref index);
		}
		protected ColumnType CreateColumnInfo(ModelNode parent, string propertyName) {
			ColumnType columnInfo = CreateMemberViewItemInternal<ColumnType>(parent, propertyName);
			columnInfo.Index = -1;
			return columnInfo;
		}
		protected void GenerateColumns(IModelListView viewInfo) {
			GenerateColumns(viewInfo.ModelClass.TypeInfo, GetColumns(viewInfo), viewInfo.ModelClass);
		}
		protected abstract IModelList<CollectionItemType> GetColumns(IModelListView listViewModel);
		protected void GenerateListViewColumns(IModelListView viewInfo) {
			generationMode = ViewGenerationMode.ListView;
			GenerateColumns(viewInfo);
			generationMode = ViewGenerationMode.None;
		}
		protected void GenerateLookupListViewColumns(IModelListView viewInfo) {
			generationMode = ViewGenerationMode.LookupListView;
			Int32 index = 0;
			IModelClass classInfo = viewInfo.ModelClass;
			if(!String.IsNullOrEmpty(classInfo.FriendlyKeyProperty)) {
				IModelMember member = classInfo.FindMember(classInfo.FriendlyKeyProperty);
				if((member != null) && !IsVisibleInViewFalse(member)) {
					AddLookupColumn(viewInfo, classInfo.FriendlyKeyProperty, ref index);
				}
			}
			String displayPropertyName = ModelViewsNodesGenerator.FindDisplayPropertyForTypeInternal(classInfo);
			if(!String.IsNullOrEmpty(displayPropertyName)) {
				IModelMember member = classInfo.FindMember(displayPropertyName);
				if((member != null) && !IsVisibleInViewFalse(member)) {
					ColumnType captionColumn = AddLookupColumn(viewInfo, displayPropertyName, ref index);
					captionColumn.Width = 340;
					captionColumn.SortIndex = 0;
					captionColumn.SortOrder = ColumnSortOrder.Ascending;
					if(displayPropertyName.IndexOf('.') > 0) {
						captionColumn.Caption = displayPropertyName;
					}
				}
			}
			foreach(IModelMember prop in classInfo.AllMembers) {
				if(IsVisibleInViewTrue(prop)) {
					AddLookupColumn(viewInfo, prop.Name, ref index);
				}
			}
			if(GetColumns(viewInfo).Count == 0) {
				GenerateColumns(viewInfo);
			}
			generationMode = ViewGenerationMode.None;
		}
		protected override void GenerateNodesCore(ModelNode node) {
			IModelListView viewInfo = (IModelListView)node.Parent;
			if(viewInfo.GetValue<bool>(ModelViewsNodesGenerator.IsLookupListView)) {
				parentMemberInfo = null;
				GenerateLookupListViewColumns(viewInfo);
			}
			else {
				parentMemberInfo = viewInfo.GetValue<IMemberInfo>(ModelViewsNodesGenerator.NestedListViewMemberInfo);
				GenerateListViewColumns(viewInfo);
			}
			if(node.IsInFirstLayer) {
				foreach(IModelColumn column in GetColumns(viewInfo)) {
					((ModelNode)column).SetValue<int?>(GeneratedIndexValueName, column.Index);
					((ModelNode)column).ClearValue(ModelValueNames.Index);
				}
			}
		}
		private List<IModelMember> CollectPropertiesToShowInternal(IModelClass classNode) {
			List<IModelMember> result = new List<IModelMember>();
			if(classNode != null) {
				ICollection<IModelMember> properties = classNode.OwnMembers;
				foreach(IModelMember propertyNode in properties) {
					if(SimpleTypes.IsSimpleType(propertyNode.Type) || IsBinaryImage(propertyNode.MemberInfo)) {
						result.Add(propertyNode);
					}
					else {
						Type type = propertyNode.Type;
						if((type != null) && (propertyNode.MemberInfo != null) && !propertyNode.MemberInfo.MemberTypeInfo.IsListType && (type.IsClass || type.IsInterface)) {
							result.Add(propertyNode);
						}
					}
				}
			}
			return result;
		}
		private static string GetViewIdInternal(IModelMemberViewItem model) {
			IModelMember modelMember = model.ModelMember;
			if(modelMember == null) {
				string viewId = model.ParentView != null ? model.ParentView.Id : "null";
				DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(modelMember, string.Format("ModelMember. ViewItem: '{0}', View: '{1}'", model.Id, viewId));
			}
			if(ListPropertyEditor.IsMemberListPropertyEditorCompatible(modelMember)) {
				return ModelNestedListViewNodesGeneratorHelper.GetNestedListViewId(modelMember.MemberInfo);
			}
			return null;
		}
		internal static P CreateMemberViewItemInternal<P>(ModelNode parent, string propertyName) where P : IModelMemberViewItem {
			P result = parent.AddNode<P>(propertyName);
			result.PropertyName = propertyName;
			result.SetValue<string>("View_ID", GetViewIdInternal(result));
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void ForceGenerateListViewColumns(IModelListView modelListView) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(modelListView, "modelListView");
			ModelNode node = (ModelNode)modelListView;
			if(!node.IsNewNode) {
				throw new ArgumentException("This operation can be performed only for new node.");
			}
			if(node.IsMaster) {
				throw new ArgumentException("This operation cannot be performed for master node.");
			}
			if(node.IsSeparate) {
				throw new ArgumentException("This operation cannot be performed for separate node.");
			}
			if(node.IsInFirstLayer) {
				throw new ArgumentException("This operation cannot be performed for generated node.");
			}
			ModelListViewColumnsNodesGenerator generator = new ModelListViewColumnsNodesGenerator();
			generator.GenerateNodesCore((ModelNode)modelListView.Columns);
		}
#if DebugTest
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void DebugTest_SetDisplayMemberColumnWidth(int value) {
			displayMemberColumnWidth = value;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void DebugTest_SetShortColumnWidth(int value) {
			shortColumnWidth = value;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void DebugTest_SetDefaultColumnWidth(int value) {
			defaultColumnWidth = value;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int DebugTest_GetDefaultColumnWidth(IModelMember propertyNode) {
			return GetDefaultColumnWidthInternal(propertyNode);
		}
#endif
	}
}
