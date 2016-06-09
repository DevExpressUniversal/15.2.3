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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	public class ModelDetailViewNodesGenerator {
		private const string DetailViewIdSuffix = "_DetailView";
		public static string GetDetailViewId(Type type) {
			return ModelNodesGeneratorSettings.GetIdPrefix(type) + DetailViewIdSuffix;
		}
		public static void GenerateModel(IModelViews views, IModelClass classInfo) {
			Guard.ArgumentNotNull(views, "views");
			Guard.ArgumentNotNull(classInfo, "classInfo");
			if(classInfo.TypeInfo == null) {
				throw new InvalidOperationException("classInfo.TypeInfo == null");
			}
			IModelDetailView detailView = views.AddNode<IModelDetailView>(GetDetailViewId(classInfo.TypeInfo.Type));
			detailView.ModelClass = classInfo;
		}
	}
	[ModelGenerateContentAction()]
	public sealed class ModelDetailViewItemsNodesGenerator : ModelNodesGeneratorBase {
		private void GenerateDetailViewPropertiesCore(ITypeInfo generatedClassTypeInfo, IModelClass modelClass, IModelViewItems items, string leadingPropertyPath, bool includeKeyProperty, IList<IMemberInfo> nestedListViewMembers, IList<IMemberInfo> expandingMembers) {
			if(!string.IsNullOrEmpty(leadingPropertyPath) && !leadingPropertyPath.EndsWith(".")) {
				leadingPropertyPath = leadingPropertyPath + ".";
			}
			ITypeInfo typeInfo = modelClass.TypeInfo;
			foreach(IModelMember member in modelClass.AllMembers) {
				IModelMember memberNodeWrapper = member;
				string memberName = memberNodeWrapper.Name;
				string substitutionMemberName;
				IModelMember substitutionMember = GetSubstitutionMember(member, out substitutionMemberName);
				if(substitutionMember != null) {
					memberName = substitutionMemberName;
					memberNodeWrapper = substitutionMember;
				}
				IMemberInfo memberInfo = typeInfo.FindMember(memberName);
				if(GetShouldGenerateMember(memberNodeWrapper, memberInfo)) {
					bool isPropertyEditorGenerated = false;
					if(ShouldExpandMembers(memberInfo)) {
						if((modelClass.Parent != null)
							&& (expandingMembers.IndexOf(memberInfo) < 0)) {
							expandingMembers.Add(memberInfo);
							try {
								IModelClass aggregatedClassInfo = ((IModelBOModel)modelClass.Parent).GetClass(memberInfo.MemberType);
								if(aggregatedClassInfo != null) {
									GenerateDetailViewPropertiesCore(generatedClassTypeInfo, aggregatedClassInfo, items, leadingPropertyPath + memberInfo.Name, false, nestedListViewMembers, expandingMembers);
									isPropertyEditorGenerated = true;
								}
							}
							finally {
								expandingMembers.Remove(memberInfo);
							}
						}
					}
					if(!isPropertyEditorGenerated) {
						if((!memberInfo.IsKey || includeKeyProperty) && items[leadingPropertyPath + memberInfo.Name] == null) {
							CreateEditor(items, leadingPropertyPath + memberInfo.Name);
						}
					}
				}
			}
		}
		protected override void GenerateNodesCore(ModelNode node) {
			IList<IMemberInfo> nestedListViewMembers = new List<IMemberInfo>();
			IModelObjectView modelObjectView = node.Parent as IModelObjectView;
			if(modelObjectView != null) {
				IModelClass modelClass = modelObjectView.ModelClass;
				GenerateDetailViewPropertiesCore(modelClass.TypeInfo, modelClass, (IModelViewItems)node, "", true, nestedListViewMembers, new List<IMemberInfo>());
			}
		}
		private static bool GetShouldGenerateMember(IModelMember propertyNode, IMemberInfo memberInfo) {
			bool shouldGenerateMember =
				(memberInfo != null)
				&&
				(propertyNode.Name != "This")
				&&
				memberInfo.IsVisible
				&&
				!memberInfo.IsReferenceToOwner
				&&
				(
					SimpleTypes.IsSimpleType(memberInfo.MemberType)
					||
					memberInfo.MemberType.IsClass
					||
					memberInfo.MemberType.IsInterface
				);
			return shouldGenerateMember;
		}
		private static bool ShouldExpandMembers(IMemberInfo memberDescriptor) {
			bool isExpandInDetailView =
				((ExpandObjectMembersAttribute.AggregatedObjectMembersDefaultExpandingMode & ExpandObjectMembers.InDetailView) == ExpandObjectMembers.InDetailView);
			bool expandMembers = memberDescriptor.IsAggregated && isExpandInDetailView;
			ExpandObjectMembersAttribute expandMembersAttr = memberDescriptor.FindAttribute<ExpandObjectMembersAttribute>();
			if(expandMembersAttr != null && String.IsNullOrEmpty(expandMembersAttr.MemberName)) {
				if((expandMembersAttr.ExpandingMode & ExpandObjectMembers.InDetailView) == ExpandObjectMembers.InDetailView) {
					expandMembers = true;
				}
				else {
					expandMembers = false;
				}
			}
			return expandMembers;
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
			if(expandMembersAttr != null && ((expandMembersAttr.ExpandingMode & ExpandObjectMembers.InDetailView) == ExpandObjectMembers.InDetailView)) {
				substitutionMemberName = string.Format("{0}.{1}", propertyNode.Name, expandMembersAttr.MemberName);
				result = propertyNode.ModelClass.FindMember(substitutionMemberName);
			}
			return result;
		}
		private static IModelPropertyEditor CreateEditor(IModelViewItems items, string propertyName) {
			return ModelListViewColumnsNodesGenerator.CreateMemberViewItemInternal<IModelPropertyEditor>((ModelNode)items, propertyName);
		}
	}
}
