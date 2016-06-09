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

using System.ComponentModel.Design;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public abstract class DataItemHistoryItem : IHistoryItem {
		readonly DataDashboardItem dashboardItem;
		readonly DataItem dataItem;
		protected DataDashboardItem DashboardItem { get { return dashboardItem; } }
		protected DataItem DataItem { get { return dataItem; } }
		protected string DataItemCaption { get { return dataItem.DisplayName; } }
		public abstract string Caption { get; }
		protected DataItemHistoryItem(DataDashboardItem dashboardItem, DataItem dataItem) {
			this.dashboardItem = dashboardItem;
			this.dataItem = dataItem;
		}
		void RefreshDesigner(DashboardDesigner designer) {
			DashboardViewer viewer = designer.Viewer;
			if(designer.SelectedDashboardItem == dashboardItem)
				designer.DragAreaScrollableControl.DragArea.Invalidate();
			else
				designer.SelectedDashboardItem = dashboardItem;
		}
		public void Undo(DashboardDesigner designer) {
			PerformUndo();
			RefreshDesigner(designer);
		}
		public void Redo(DashboardDesigner designer) {
			Dashboard dashboard = designer.Dashboard;
			IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
			DesignerTransaction transaction = designerHost != null ? designerHost.CreateTransaction(Caption) : null;
			try {
				IComponentChangeService changeService = dashboard.GetService<IComponentChangeService>();
				if(changeService != null)
					changeService.OnComponentChanging(dashboardItem, null);
				PerformRedo();
				if(changeService != null)
					changeService.OnComponentChanged(dashboardItem, null, null, null);
			}
			finally {
				if(transaction != null)
					transaction.Commit();
			}
			RefreshDesigner(designer);
		}
		protected abstract void PerformUndo();
		protected abstract void PerformRedo();
	}
}
