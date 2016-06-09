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
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	public static class ModelNestedListViewNodesGeneratorHelper {
		public static string GetNestedListViewId(IMemberInfo memberInfo) {
			return GetNestedListViewId(memberInfo.LastMember.Owner.Type, memberInfo.LastMember.Name);
		}
		public static string GetNestedListViewId(Type type, string propertyName) {
			Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
			return string.Format("{0}_{1}_ListView", ModelNodesGeneratorSettings.GetIdPrefix(type), propertyName);
		}
		private static IModelListView GenerateModel(IModelViews views, IModelMember modelMember) {
			IModelBOModel boModel = views.Application.BOModel;
			IMemberInfo memberInfo = modelMember.MemberInfo;
			IModelListView listViewInfo = views.AddNode<IModelListView>(GetNestedListViewId(memberInfo));
			listViewInfo.ModelClass = boModel.GetClass(memberInfo.ListElementTypeInfo.Type);
			listViewInfo.SetValue<IMemberInfo>(ModelViewsNodesGenerator.NestedListViewMemberInfo, memberInfo);
			listViewInfo.DataAccessMode = CollectionSourceDataAccessMode.Client;
			ModelListViewNodesGenerator.GenerateNodes(listViewInfo);
			return listViewInfo;
		}
		internal static void GenerateViewIfNeed(IModelMember modelMember) {
			if(ListPropertyEditor.IsMemberListPropertyEditorCompatible(modelMember)) {
				string nestedListViewId = GetNestedListViewId(modelMember.MemberInfo);
				if(modelMember.Application.Views[nestedListViewId] == null) {
					GenerateModel(modelMember.Application.Views, modelMember);
				}
			}
		}
	}
}
