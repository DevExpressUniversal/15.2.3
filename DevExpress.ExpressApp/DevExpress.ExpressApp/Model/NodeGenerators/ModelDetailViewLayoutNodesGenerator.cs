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
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	[ModelGenerateContentAction]
	public class ModelDetailViewLayoutNodesGenerator : ModelNodesGeneratorBase {
		class UniqueIdProvider {
			HashSet<string> usedUniqueIds = new HashSet<string>();
			Dictionary<string, int> usedIds = new Dictionary<string, int>();
			public string GetUniqueId(string id) {
				if(!usedIds.ContainsKey(id)) {
					usedIds.Add(id, 1);
				}
				string uniqueId = id;
				while(usedUniqueIds.Contains(uniqueId)) {
					int idUsesCount = usedIds[id];
					idUsesCount++;
					uniqueId = string.Format("{0}_{1}", id, idUsesCount);
					usedIds[id] = idUsesCount;
				}
				usedUniqueIds.Add(uniqueId);
				return uniqueId;
			}
		}
		public const string STR_col1 = "_col1";
		public const string STR_col2 = "_col2";
		public const string LayoutGroupNameSuffix = "_Group";
		public const string TabsLayoutGroupName = "Tabs";
		public const string SizeableEditorsLayoutGroupName = "SizeableEditors";
		public static int EditorsMaxCountForLayoutInFlow = 4;
		public const string MainLayoutGroupName = "Main";
		public const string SimpleEditorsLayoutGroupName = "SimpleEditors";
		public const string CollectionsLayoutGroupName = "RelatedDetails";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool ForceShowGroupCaptions = false;
		public static bool GenerateTabsForCollections = true;
		protected override void GenerateNodesCore(ModelNode node) {
			IModelCompositeView compositeView = (IModelCompositeView)node.Parent;
			if(compositeView.Items.Count == 0) {
				return;
			}
			IModelLayoutGroup result = CreateLayoutGroup(node, MainLayoutGroupName, 0, FlowDirection.Vertical, false);
			List<IModelViewItem> simpleEditors = new List<IModelViewItem>();
			List<IModelViewItem> sizeableEditors = new List<IModelViewItem>();
			List<IModelViewItem> listProperties = new List<IModelViewItem>();
			Dictionary<DetailViewLayoutAttribute, List<IModelViewItem>> customLayoutGroups = new Dictionary<DetailViewLayoutAttribute, List<IModelViewItem>>();
			foreach(IModelPropertyEditor propertyEditor in GetPropertyEditors(compositeView)) {
				IMemberInfo memberInfo = propertyEditor.ModelMember.MemberInfo;
				DetailViewLayoutAttribute attr = memberInfo.FindAttribute<DetailViewLayoutAttribute>();
				if(IsCustomLayoutGroup(attr)) {
					AddToCustomLayoutGroups(customLayoutGroups, propertyEditor, attr);
				}
				else if(memberInfo.MemberTypeInfo.IsListType && !IsBinaryImage(memberInfo) || (attr != null && attr.GroupType == LayoutGroupType.TabbedGroup)) {
					listProperties.Add(propertyEditor);
				}
				else if(memberInfo.MemberType == typeof(string) && propertyEditor.RowCount > 0 || (attr != null && attr.GroupType == LayoutGroupType.SizableEditorsGroup)) {
					sizeableEditors.Add(propertyEditor);
				}
				else {
					simpleEditors.Add(propertyEditor);
				}
			}
			int layoutElementIndex = 0;
			if(simpleEditors.Count > 0) {
				IModelClass modelClass = (node.Parent is IModelObjectView) ? ((IModelObjectView)node.Parent).ModelClass : null;
				CreateSimpleEditorsLayoutGroup(result, layoutElementIndex, modelClass, simpleEditors);
				layoutElementIndex++;
			}
			if(sizeableEditors.Count > 0) {
				LayoutInFlow(result, SizeableEditorsLayoutGroupName, layoutElementIndex, sizeableEditors, FlowDirection.Vertical, false);
				layoutElementIndex++;
			}
			if(listProperties.Count > 0) {
				CreateListPropertiesLayoutGroup(result, listProperties, layoutElementIndex, null);
				layoutElementIndex++;
			}
			CreateCustomLayoutGroups(result, layoutElementIndex, customLayoutGroups);
		}
		private static void CreateListPropertiesLayoutGroup(IModelNode result, List<IModelViewItem> listProperties, int layoutElementIndex, string groupName) {
			if(listProperties.Count == 1) {
				groupName = groupName != null ? groupName : listProperties[0].Id + LayoutGroupNameSuffix;
				IModelNode complexPropertiesLayoutingInfo = LayoutInFlow(result, groupName, layoutElementIndex, listProperties, FlowDirection.Horizontal, true);
				((ModelNode)complexPropertiesLayoutingInfo).ClearValue("Caption");
			}
			else {
				if(GenerateTabsForCollections) {
					groupName = groupName != null ? groupName : TabsLayoutGroupName;
					CreateTabsLayoutGroup(result, groupName, layoutElementIndex, listProperties);
				}
				else {
					groupName = groupName != null ? groupName : CollectionsLayoutGroupName;
					IModelLayoutGroup complexPropertiesLayoutingInfo = LayoutInFlow(result, groupName, layoutElementIndex, listProperties, FlowDirection.Vertical, true);
					complexPropertiesLayoutingInfo.Caption = CaptionHelper.ConvertCompoundName(CollectionsLayoutGroupName);
				}
			}
		}
		private static void MoveLayoutItems(IModelNode result, int index) {
			for(int i = index; i < result.NodeCount; i++) {
				result.GetNode(i).Index++;
			}
		}
		private static IModelLayoutGroup GetLayoutGroup(IModelLayoutGroup baseNode, string groupId) {
			IModelLayoutGroup result = null;
			foreach(IModelLayoutGroup node in baseNode.GetNodes<IModelLayoutGroup>()) {
				if(node.Id == groupId) {
					result = node;
				}
				else {
					result = GetLayoutGroup(node, groupId);
				}
				if(result != null) {
					break;
				}
			}
			return result;
		}
		private static Boolean AddToExistingLayoutGroup(IModelLayoutGroup baseNode, Dictionary<DetailViewLayoutAttribute, List<IModelViewItem>> customEditorsLayout, DetailViewLayoutAttribute key) {
			Boolean result = false;
			IModelLayoutGroup defaultGroup = GetLayoutGroup(baseNode, key.GroupId);
			if(defaultGroup != null) {
				foreach(IModelViewItem editor in customEditorsLayout[key]) {
					IndexAttribute attr = ((IModelPropertyEditor)editor).ModelMember.MemberInfo.FindAttribute<IndexAttribute>();
					int index = attr != null ? attr.Index : defaultGroup.NodeCount;
					MoveLayoutItems(defaultGroup, index);
					CreateLayoutItem(defaultGroup, editor, index, null);
				}
				result = true;
			}
			return result;
		}
		private static void CreateCustomLayoutGroups(IModelLayoutGroup result, int layoutElementIndex, Dictionary<DetailViewLayoutAttribute, List<IModelViewItem>> customEditorsLayout) {
			foreach(DetailViewLayoutAttribute key in customEditorsLayout.Keys.OrderBy(x => x.GroupIndex)) {
				if(!AddToExistingLayoutGroup(result, customEditorsLayout, key)) {
					int index = layoutElementIndex;
					if(key.GroupIndex >= 0) {
						index = key.GroupIndex;
						MoveLayoutItems(result, index);
					}
					if(key.GroupType == LayoutGroupType.SimpleEditorsGroup) {
						if(customEditorsLayout[key].Count > EditorsMaxCountForLayoutInFlow) {
							LayoutInTwoColumns(result, key.GroupId, index, customEditorsLayout[key]);
							layoutElementIndex++;
						}
						else {
							LayoutInFlow(result, key.GroupId, index, customEditorsLayout[key], FlowDirection.Vertical, false);
							layoutElementIndex++;
						}
					}
					else if(key.GroupType == LayoutGroupType.SizableEditorsGroup) {
						LayoutInFlow(result, key.GroupId, index, customEditorsLayout[key], FlowDirection.Vertical, false);
						layoutElementIndex++;
					}
					else if(key.GroupType == LayoutGroupType.TabbedGroup) {
						CreateListPropertiesLayoutGroup(result, customEditorsLayout[key], index, key.GroupId);
						layoutElementIndex++;
					}
				}
			}
		}
		private static void AddToCustomLayoutGroups(Dictionary<DetailViewLayoutAttribute, List<IModelViewItem>> customEditorsGroups, IModelPropertyEditor propertyEditor, DetailViewLayoutAttribute attr) {
			bool editorWasAdded = false;
			foreach(DetailViewLayoutAttribute key in customEditorsGroups.Keys) {
				if(key.GroupId == attr.GroupId) {
					if(key.GroupType == attr.GroupType && key.GroupIndex == attr.GroupIndex) {
						customEditorsGroups[key].Add(propertyEditor);
						editorWasAdded = true;
						break;
					}
					else {
						throw new InvalidOperationException(string.Format("All parameters for the '{0}' group must be identical", attr.GroupId));
					}
				}
			}
			if(!editorWasAdded) {
				customEditorsGroups.Add(attr, new List<IModelViewItem>() { propertyEditor });
			}
		}
		private static Boolean IsCustomLayoutGroup(DetailViewLayoutAttribute attr) {
			return attr != null
				&& attr.GroupId != null
				&& attr.GroupId != SimpleEditorsLayoutGroupName
				&& attr.GroupId != CollectionsLayoutGroupName
				&& attr.GroupId != SizeableEditorsLayoutGroupName
				&& attr.GroupId != TabsLayoutGroupName;
		}
		private static IEnumerable<IModelPropertyEditor> GetPropertyEditors(IModelCompositeView modelCompositeView) {
			List<ModelNode> unsortedViewItems = ((ModelNode)modelCompositeView.Items).GetUnsortedChildren();
			foreach(ModelNode viewItem in unsortedViewItems) {
				IModelPropertyEditor propertyEditor = viewItem as IModelPropertyEditor;
				if(propertyEditor != null) {
					if(propertyEditor.ModelMember == null) {
						throw new InvalidOperationException(string.Format("ModelMember property of the {0} property editor cannot be null.", viewItem.Id));
					}
					if(!propertyEditor.ModelMember.IsVisibleInDetailView.HasValue || propertyEditor.ModelMember.IsVisibleInDetailView.Value) {
						yield return propertyEditor;
					}
				}
			}
		}
		private static IModelLayoutGroup CreateLayoutGroup(IModelNode node, string id, int index, FlowDirection direction, bool showCaption) {
			IModelLayoutGroup result = node.AddNode<IModelLayoutGroup>(id);
			result.Index = index;
			result.Direction = direction;
			result.ShowCaption = showCaption;
			return result;
		}
		private static void CreateSimpleEditorsLayoutGroup(IModelNode node, int index, IModelClass modelClass, List<IModelViewItem> editors) {
			IModelViewLayoutElement result = CreateLayoutGroup(node, SimpleEditorsLayoutGroupName, index, FlowDirection.Vertical, false);
			HashSet<string> editorIds = new HashSet<string>();
			editors.ForEach(modelPropertyEditor => editorIds.Add(modelPropertyEditor.Id));
			UniqueIdProvider idProvider = new UniqueIdProvider();
			int layoutElementIndex = 0;
			foreach(ITypeInfo typeInfo in GetTypesInfoForSimpleEditorsLayoutGroupsGeneration(modelClass.TypeInfo)) {
				string layoutGroupId = idProvider.GetUniqueId(ReflectionHelper.GetShortClassName(typeInfo.Name));
				if(editorIds.Contains(layoutGroupId)) {
					layoutGroupId += LayoutGroupNameSuffix;
				}
				CreateDeclaredPropertiesLayoutGroup(result, layoutGroupId, layoutElementIndex, typeInfo, editors);
				layoutElementIndex++;
			}
		}
		private static List<ITypeInfo> GetTypesInfoForSimpleEditorsLayoutGroupsGeneration(ITypeInfo typeInfo) {
			List<ITypeInfo> result = new List<ITypeInfo>();
			if(typeInfo != null && typeInfo.IsInterface) {
				result.Add(typeInfo);
				foreach(ITypeInfo baseInterfaceInfo in typeInfo.ImplementedInterfaces) {
					result.Add(baseInterfaceInfo);
				}
			}
			else {
				ITypeInfo currentTypeInfo = typeInfo;
				while(currentTypeInfo != null) {
					result.Add(currentTypeInfo);
					currentTypeInfo = currentTypeInfo.Base;
				}
			}
			return result;
		}
		private static void CreateDeclaredPropertiesLayoutGroup(IModelViewLayoutElement node, string id, int index, ITypeInfo typeInfo, List<IModelViewItem> editors) {
			List<IModelViewItem> declaredEditors = new List<IModelViewItem>();
			List<IModelViewItem> localEditors = new List<IModelViewItem>(editors);
			foreach(IModelViewItem editor in localEditors) {
				if(IsPathStartFromPropertyOfThisClass(editor.Id, typeInfo)) {
					declaredEditors.Add(editor);
					editors.Remove(editor);
				}
			}
			IModelLayoutGroup result = null;
			if(declaredEditors.Count > EditorsMaxCountForLayoutInFlow) {
				result = LayoutInTwoColumns(node, id, index, declaredEditors);
			}
			else if(declaredEditors.Count > 0) {
				result = LayoutInFlow(node, id, index, declaredEditors, FlowDirection.Vertical, ForceShowGroupCaptions);
			}
			if(typeInfo.IsInterface && typeInfo.IsDomainComponent && result != null) {
				TypesInfo typesInfo = XafTypesInfo.Instance as TypesInfo;
				if(typesInfo != null) {
					Type generatedEntityType = typesInfo.GetGeneratedEntityType(typeInfo.Type);
					if(generatedEntityType != null) {
						((IModelLayoutGroup)result).Caption = CaptionHelper.ConvertCompoundName(ReflectionHelper.GetShortClassName(generatedEntityType.Name));
					}
				}
			}
		}
		private static bool IsPathStartFromPropertyOfThisClass(string propertyPath, ITypeInfo typeInfo) {
			int pos = propertyPath.IndexOf('.');
			if(pos > -1) {
				propertyPath = propertyPath.Substring(0, pos);
			}
			IMemberInfo memberInfo = typeInfo.FindMember(propertyPath);
			return memberInfo != null && memberInfo.Owner == typeInfo;
		}
		private static IModelLayoutGroup LayoutInTwoColumns(IModelNode node, string id, int index, List<IModelViewItem> editors) {
			if(editors.Count < 2) {
				throw new ArgumentException("editors.Count < 2", "editors");
			}
			IModelLayoutGroup result = CreateLayoutGroup(node, id, index, FlowDirection.Horizontal, true);
			result.Caption = CaptionHelper.ConvertCompoundName(id);
			int columnPropertiesCount = (editors.Count >> 1);
			if((editors.Count & 1) == 1) {
				columnPropertiesCount++;
			}
			List<IModelViewItem> columnEditors = new List<IModelViewItem>();
			List<IModelViewItem> columnEditors2 = new List<IModelViewItem>();
			for(int i = 0; i < editors.Count; i++) {
				DetailViewLayoutAttribute attr = ((IModelPropertyEditor)editors[i]).ModelMember.MemberInfo.FindAttribute<DetailViewLayoutAttribute>();
				if(((attr == null || attr.ColumnPosition == null) && i < columnPropertiesCount) || (attr != null && attr.ColumnPosition == LayoutColumnPosition.Left)) {
					columnEditors.Add(editors[i]);
				}
				else {
					columnEditors2.Add(editors[i]);
				}
			}
			if(columnEditors.Count > 0) {
				LayoutInFlow(result, id + STR_col1, 0, columnEditors, FlowDirection.Vertical, false);
			}
			if(columnEditors2.Count > 0) {
				LayoutInFlow(result, id + STR_col2, 1, columnEditors2, FlowDirection.Vertical, false);
			}
			return result;
		}
		private static IModelLayoutGroup LayoutInFlow(IModelNode node, string id, int index, List<IModelViewItem> editors, FlowDirection direction, bool showCaption) {
			IModelLayoutGroup result = CreateLayoutGroup(node, id, index, direction, showCaption);
			SortEditors(editors); 
			int layoutElementIndex = 0;
			foreach(IModelViewItem editor in editors) {
				CreateLayoutItem(result, editor, layoutElementIndex, null);
				layoutElementIndex++;
			}
			return result;
		}
		private static void CreateTabsLayoutGroup(IModelNode parentNode, string id, int index, List<IModelViewItem> editors) {
			IModelTabbedGroup result = parentNode.AddNode<IModelTabbedGroup>(id);
			result.Index = index;
			int layoutElementIndex = 0;
			foreach(IModelViewItem editor in editors) {
				IModelNode groupItem = CreateLayoutGroup(result, editor.Id, layoutElementIndex, FlowDirection.Vertical, true);
				CreateLayoutItem(groupItem, editor, 0, false);
				layoutElementIndex++;
			}
		}
		private static void CreateLayoutItem(IModelNode parentNode, IModelViewItem editor, int index, bool? showCaption) {
			IModelLayoutViewItem item = parentNode.AddNode<IModelLayoutViewItem>(editor.Id);
			item.ViewItem = editor;
			item.ShowCaption = showCaption;
			item.Index = index;
		}
		private const string SortStabilizerValue = "__SortStabilizerValue__";
		private static void SortEditors(List<IModelViewItem> editors) {
			for(int i = 0; i < editors.Count; ++i) {
				editors[i].SetValue<int>(SortStabilizerValue, i);
			}
			editors.Sort(Compare);
			for(int i = 0; i < editors.Count; ++i) {
				editors[i].ClearValue(SortStabilizerValue);
			}
		}
		private static int Compare(IModelViewItem x, IModelViewItem y) {
			if(object.ReferenceEquals(x, y)) return 0;
			int result = 0;
			if(x is IModelPropertyEditor && y is IModelPropertyEditor) {
				result = Comparer<int>.Default.Compare(((IModelPropertyEditor)x).RowCount, ((IModelPropertyEditor)y).RowCount);
			}
			if(result == 0) {
				result = Comparer<int?>.Default.Compare(x.Index, y.Index);
			}
			if(result == 0) {
				result = Comparer<int>.Default.Compare(x.GetValue<int>(SortStabilizerValue), y.GetValue<int>(SortStabilizerValue));
			}
			return result;
		}
#if DebugTest
		internal const string DebugTest_SortStabilizerValue = SortStabilizerValue;
		internal static int DebugTest_Compare(IModelViewItem x, IModelViewItem y) {
			return Compare(x, y);
		}
#endif
	}
}
