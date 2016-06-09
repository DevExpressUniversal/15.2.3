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
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxTreeList;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public class ASPxTreeListModelSynchronizer : ModelSynchronizer<ASPxTreeList, IModelListView> {
		public ASPxTreeListModelSynchronizer(ASPxTreeList treeList, IModelListView model)
			: base(treeList, model) {
		}
		protected override void ApplyModelCore() {
			Control.SettingsBehavior.AllowSort = ((IModelOptionsWeb)Model.Application.Options).ListViewAllowSort;
			Control.SettingsEditing.AllowNodeDragDrop = false;
			Control.SettingsPager.Position = ((IModelListViewTreeListWeb)Model).PagerPosition;
		}
		public override void SynchronizeModel() {
		}
		public static void ApplyTreeListModel(IModelListView model, ASPxTreeList treeList) {
			Guard.ArgumentNotNull(model, "model");
			Guard.ArgumentNotNull(treeList, "treeList");
			treeList.Settings.ShowFooter = model.IsFooterVisible;
			treeList.SettingsBehavior.AllowSort = ((IModelOptionsWeb)model.Application.Options).ListViewAllowSort;
			treeList.SettingsEditing.AllowNodeDragDrop = false;
			treeList.ID = WebIdHelper.GetListEditorControlId(model.Id); 
		}
		public static void SaveTreeListModel(IModelListView model, ASPxTreeList treeList) {
			Guard.ArgumentNotNull(model, "model");
			Guard.ArgumentNotNull(treeList, "treeList");
			model.IsFooterVisible = treeList.Settings.ShowFooter;
		}
		public static void ApplyPagerModel(IModelListView model, ASPxTreeList treeList) {
			Guard.ArgumentNotNull(model, "model");
			Guard.ArgumentNotNull(treeList, "treeList");
			IModelListViewWeb listViewWebModel = (IModelListViewWeb)model;
			treeList.PageIndex = listViewWebModel.PageIndex;
			treeList.SettingsPager.PageSize = listViewWebModel.PageSize > 0 ? listViewWebModel.PageSize : PagerModelSynchronizer.DefaultPageSize;
			treeList.SettingsPager.Position = ((IModelListViewTreeListWeb)model).PagerPosition;
		}
		public static void SavePagerModel(IModelListView model, ASPxTreeList treeList) {
			Guard.ArgumentNotNull(model, "model");
			Guard.ArgumentNotNull(treeList, "treeList");
			IModelListViewWeb listViewWebModel = (IModelListViewWeb)model;
			listViewWebModel.PageIndex = treeList.PageIndex;
			listViewWebModel.PageSize = treeList.SettingsPager.PageSize;
		}
	}
	public class ASPxTreeListDataBoundModelSyncronizer : ModelSynchronizer<ASPxTreeListEditor, IModelListViewWeb> {
		private PagerModelSynchronizer pagerModelSynchronizer;
		public ASPxTreeListDataBoundModelSyncronizer(ASPxTreeListEditor treeListEditor, IModelListViewWeb model)
			: base(treeListEditor, model) {
				pagerModelSynchronizer = new PagerModelSynchronizer(treeListEditor, model);
				treeListEditor.DataBinder.DataBound += ModelSynchronizer_ApplyModel;
				treeListEditor.TreeList.PageIndexChanged += Control_Changed;
		}
		protected override void ApplyModelCore() {
			pagerModelSynchronizer.ApplyModel();
		}
		public override void SynchronizeModel() {
			pagerModelSynchronizer.SynchronizeModel();
		}
		public override void Dispose() {
			if (Control.TreeList != null) {
				Control.TreeList.PageIndexChanged -= Control_Changed;
			}
			if (Control.DataBinder != null) {
				Control.DataBinder.DataBound -= ModelSynchronizer_ApplyModel;
			}
			if (pagerModelSynchronizer != null) {
				pagerModelSynchronizer.Dispose();
				pagerModelSynchronizer = null;
			}
			base.Dispose();
		}
	}
}
