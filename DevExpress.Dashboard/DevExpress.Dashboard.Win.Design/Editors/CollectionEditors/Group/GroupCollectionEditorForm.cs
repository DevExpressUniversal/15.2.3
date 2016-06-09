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

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using DevExpress.Utils.UI;
namespace DevExpress.DashboardWin.Design {
	class GroupCollectionEditorForm : DashboardCollectionEditorFormBase {
		#region
		class GroupCollectionEditorContentControl : CollectionEditorContentControl {
			public GroupCollectionEditorContentControl(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor)
				: base(provider, collectionEditor) {
			}
			protected override void OnClick(EventArgs e) {
				base.OnClick(e);
				DashboardItemGroup itemGroup = (DashboardItemGroup)CreateInstance(typeof(DashboardItemGroup));
				IComponentChangeService changeService = Context.GetService<IComponentChangeService>();
				ComponentChanger.OnComponentChanging(changeService, itemGroup);
				itemGroup.ComponentName = ((IComponent)itemGroup).Site.Name;
				DashboardDesigner designer = propertyGrid.ServiceProvider.GetService<SelectedContextService>().Designer;
				itemGroup.Name = designer.Dashboard.GroupCaptionGenerator.GenerateName(itemGroup);
				ComponentChanger.OnComponentChanged(changeService, itemGroup);
				AddInstance(itemGroup);
			}
			protected override void DisposeInstanceOnFinish(object instance) {
				DashboardItemGroup itemGroup = instance as DashboardItemGroup;
				Dashboard dashboard = Context.Instance as Dashboard;
				if(itemGroup != null && dashboard != null) {
					IComponentChangeService changeService = GetService<IComponentChangeService>();
					if(changeService != null) {
						foreach(DashboardItem dashboardItem in dashboard.Items) {
							if(dashboardItem.Group == itemGroup) {
								PropertyDescriptor property = TypeDescriptor.GetProperties(dashboardItem)["Group"];
								changeService.OnComponentChanging(dashboardItem, property);
								dashboardItem.Group = null;
								changeService.OnComponentChanged(dashboardItem, property, itemGroup, null);
							}
						}
					}
				}
				base.DisposeInstanceOnFinish(instance);
			}
		}
		#endregion
		const string DashboardItemGroupsTitle = "Groups";
		public override string Text { get { return DashboardItemGroupsTitle; } }
		public GroupCollectionEditorForm(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
		}
		protected override CollectionEditorContentControl CreateCollectionEditorContentControl(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor) {
			return new GroupCollectionEditorContentControl(provider, collectionEditor);
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			if (DialogResult == DialogResult.OK)
				RebuildDashboardLayout();
		}
	}
}
