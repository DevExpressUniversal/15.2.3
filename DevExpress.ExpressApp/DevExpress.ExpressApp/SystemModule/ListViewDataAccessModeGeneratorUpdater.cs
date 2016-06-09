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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.SystemModule {
	public class ListViewDataAccessModeGeneratorUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
		private readonly Type listEditorType;
		List<CollectionSourceDataAccessMode> allowedDataAcessMode = null;
		public ListViewDataAccessModeGeneratorUpdater(Type listEditorType)
			: this(listEditorType, null) {
		}
		public ListViewDataAccessModeGeneratorUpdater(Type listEditorType, List<CollectionSourceDataAccessMode> allowedDataAcessMode) {
			Guard.ArgumentNotNull(listEditorType, "listEditorType");
			this.listEditorType = listEditorType;
			this.allowedDataAcessMode = allowedDataAcessMode;
		}
		public override void UpdateNode(ModelNode node) {
			IModelViews modelViews = (IModelViews)node;
			foreach(IModelView modelView in modelViews) {
				IModelListView modelListView = modelView as IModelListView;
				if(modelListView != null && listEditorType.IsAssignableFrom(modelListView.EditorType)
					&& (allowedDataAcessMode == null || !allowedDataAcessMode.Contains(modelListView.DataAccessMode))) {
					modelListView.DataAccessMode = CollectionSourceDataAccessMode.Client;
				}
			}
		}
	}
}
