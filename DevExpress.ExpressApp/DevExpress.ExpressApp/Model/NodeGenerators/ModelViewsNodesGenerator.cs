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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelViewsNodesGenerator : ModelNodesGeneratorBase {
		protected internal const string IsLookupListView = "IsLookupView";
		protected internal const string NestedListViewMemberInfo = "NestedViewMemberInfo";
		protected virtual void GenerateListView(IModelViews views, IModelClass classInfo) {
			ModelListViewNodesGenerator.GenerateModel(views, classInfo);
		}
		protected virtual void GenerateDetailView(IModelViews views, IModelClass classInfo) {
			ModelDetailViewNodesGenerator.GenerateModel(views, classInfo);
		}
		protected override void GenerateNodesCore(ModelNode node) {
			IModelViews views = (IModelViews)node;
			foreach (IModelClass modelClass in node.Application.BOModel) {
				GenerateListView(views, modelClass);
				GenerateDetailView(views, modelClass);
				ModelLookupViewNodesGeneratorHelper.GenerateModel(views, modelClass);
				foreach(IModelMember modelMember in modelClass.AllMembers) {
					ModelNestedListViewNodesGeneratorHelper.GenerateViewIfNeed(modelMember);
				}
			}
			foreach (IEditorTypeRegistration editorRegistration in ((IModelSources)node.Application).EditorDescriptors.ListEditorRegistrations) {
					if (editorRegistration != null && editorRegistration.ElementType == typeof(Object) && editorRegistration.IsDefaultEditor) {
						views.DefaultListEditor = editorRegistration.EditorType;
				}
			}
		}
		public static string FindDisplayPropertyName(IModelClass classNode) {
			return FindDisplayPropertyForTypeInternal(classNode);
		}
		internal static string FindDisplayPropertyForTypeInternal(IModelClass classNode) {
			if((classNode != null) && (!string.IsNullOrEmpty(classNode.DefaultProperty))) {
				ITypeInfo typeInfo = classNode.TypeInfo;
				if(typeInfo != null) {
					string propertyName = classNode.DefaultProperty;
					IMemberInfo memberInfo = typeInfo.FindMember(propertyName);
					if(memberInfo == null) {
						throw new ArgumentException(string.Format(
							"Cannot find the '{0}' member node within the '{1}' class",
							propertyName, classNode.Name));
					}
					if(SimpleTypes.IsSimpleType(memberInfo.MemberType) || memberInfo.MemberType == typeInfo.Type) {
						return memberInfo.Name;
					}
					else {
						string displayPropertyName = FindDisplayPropertyForTypeInternal(
							((IModelBOModel)classNode.Parent).GetClass(memberInfo.MemberType));
						if(!string.IsNullOrEmpty(displayPropertyName)) {
							return memberInfo.Name + '.' + displayPropertyName;
						}
						else {
							Tracing.Tracer.LogWarning("Cannot retrieve a display property for the ({1}).{2} property " +
								"because it references the \"{0}\" type which doesn't have a display property",
								memberInfo.MemberType, typeInfo.Name, propertyName);
						}
					}
				}
			}
			return null;
		}
	}
}
