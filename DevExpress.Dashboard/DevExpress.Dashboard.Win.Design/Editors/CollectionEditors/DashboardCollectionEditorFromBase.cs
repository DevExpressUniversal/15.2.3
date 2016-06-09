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
using DevExpress.Utils.UI;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Design {
	abstract class DashboardCollectionEditorFormBase : CollectionEditorFormBase, IEditorContext {
		protected DashboardDesigner DashboardDesigner { get { return ServiceProvider.GetService<SelectedContextService>().Designer; } }
		protected Dashboard Dashboard { get { return DashboardDesigner.Dashboard; } }
		protected IComponentChangeService ChangeService { get { return ServiceProvider.GetService<IComponentChangeService>(); } }
		IServiceProvider ServiceProvider { get { return collectionEditorContent.propertyGrid.ServiceProvider; } }
		protected DashboardCollectionEditorFormBase(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
		}
		protected void RebuildDashboardLayout() {
			IComponentChangeService changeService = ServiceProvider.GetService<IComponentChangeService>();
			if (changeService != null) {
				PropertyDescriptor layoutProperty = TypeDescriptor.GetProperties(Dashboard)["LayoutRoot"];
				changeService.OnComponentChanging(Dashboard, layoutProperty);
				object oldLayoutRoot = Dashboard.LayoutRoot;
				Dashboard.RebuildLayout();
				changeService.OnComponentChanged(Dashboard, layoutProperty, oldLayoutRoot, Dashboard.LayoutRoot);
			}
		}
		protected void SyncronizeDashboardDataConnections() {
			IComponentChangeService changeService = ServiceProvider.GetService<IComponentChangeService>();
			if (changeService != null) {
				PropertyDescriptor dcProperty = TypeDescriptor.GetProperties(Dashboard)["DataConnections"];
				changeService.OnComponentChanging(Dashboard, dcProperty);
				Dashboard.SyncronizeDataConnections();
				changeService.OnComponentChanged(Dashboard, dcProperty, null, null);
			}
		}
		#region IEditorContext
		object IEditorContext.EditorContext { get { return collectionEditorContent.tv.SelectedNode.Tag; } }
		#endregion
	}
}
