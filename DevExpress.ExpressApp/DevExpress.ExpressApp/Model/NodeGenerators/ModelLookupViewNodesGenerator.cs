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
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	public static class ModelLookupViewNodesGeneratorHelper {
		private const string LookupListViewIdSuffix = "_LookupListView";
		public static string GetLookupListViewId(Type type) {
			return ModelNodesGeneratorSettings.GetIdPrefix(type) + LookupListViewIdSuffix;
		}
		public static void GenerateModel(IModelViews views, IModelClass classInfo) {
			Guard.ArgumentNotNull(views, "views");
			Guard.ArgumentNotNull(classInfo, "classInfo");
			if(classInfo.TypeInfo == null) {
				throw new InvalidOperationException("classInfo.TypeInfo == null");
			}
			IModelListView listView = views.AddNode<IModelListView>(GetLookupListViewId(classInfo.TypeInfo.Type));
			listView.ModelClass = classInfo;
			listView.SetValue<bool>(ModelViewsNodesGenerator.IsLookupListView, true);
			listView.IsGroupPanelVisible = false;
			listView.AutoExpandAllGroups = false;
			listView.IsFooterVisible = false;
		}
	}
}
