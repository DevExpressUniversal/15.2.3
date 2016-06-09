#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Design {
	class DashboardItemCollectionEditorForm : CollectionEditorForm {
		#region inner
		class DashboardItemCollectionEditorContentControl : CustomCollectionEditorContentControl {
			public DashboardItemCollectionEditorContentControl(IServiceProvider provider, DevExpress.Utils.UI.CollectionEditor collectionEditor, MenuItemController menuItemController)
				: base(provider, collectionEditor, menuItemController) {
			}
			protected override void OnCreateItem(object sender, EventArgs e) {
				Type type = sender as Type;
				DashboardItem dashboardItem = CreateInstance(type) as DashboardItem;
				IComponentChangeService changeService = Context.GetService<IComponentChangeService>();
				ComponentChanger.OnComponentChanging(changeService, dashboardItem);
				dashboardItem.ComponentName = ((IComponent)dashboardItem).Site.Name;
				dashboardItem.Name = DashboardDesigner.Dashboard.DashboardItemCaptionGenerator.GenerateName(dashboardItem);
				ComponentChanger.OnComponentChanged(changeService, dashboardItem);
				AddInstance(dashboardItem);
			}
		}
		#endregion
		const string DashboardItemsTitle = "Dashboard Items";
		readonly static DashboardItemMenuController menuController = new DashboardItemMenuController();
		public override string Text { get { return DashboardItemsTitle; } }
		protected override MenuItemController MenuItemController { get { return menuController; } }
		public DashboardItemCollectionEditorForm(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
		}
		protected override Utils.UI.CollectionEditorContentControl CreateCollectionEditorContentControl(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor) {
			return new DashboardItemCollectionEditorContentControl(provider, collectionEditor, MenuItemController);
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			if (DialogResult == DialogResult.OK)
				RebuildDashboardLayout();
		}
	}
}
