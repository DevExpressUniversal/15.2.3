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

using System.Collections.Generic;
using System.ComponentModel.Design;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using System.Drawing;
using System.ComponentModel;
using System;
namespace DevExpress.DashboardWin.Native {
	public abstract class DashboardItemLayoutHistoryItem : IHistoryItem {
		DashboardLayoutGroup previousLayout;
		DashboardLayoutGroup nextLayout;
		public abstract string Caption { get; }
		protected abstract IEnumerable<string> PropertyNames { get; }
		protected virtual DashboardItem InsertedDashboardItem { get { return null; } }
		protected virtual DashboardItem RemovedDashboardItem { get { return null; } }		
		protected DashboardItemLayoutHistoryItem() {
		}
		public void Undo(DashboardDesigner designer) {
			Dashboard dashboard = designer.Dashboard;
			designer.DragAreaScrollableControl.DragArea.BeginUpdate();
			dashboard.BeginUpdate();
			try {
				PerformUndoAction(designer);
				dashboard.LayoutRoot = previousLayout;
			}
			finally {
				dashboard.EndUpdate();
			}
			designer.DragAreaScrollableControl.DragArea.EndUpdate();
		}
		public void Redo(DashboardDesigner designer) {
			Dashboard dashboard = designer.Dashboard;
			if(previousLayout == null)
				previousLayout = dashboard.LayoutRoot != null ? (DashboardLayoutGroup)dashboard.LayoutRoot.CopyTree() : null;
			IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
			designer.DragAreaScrollableControl.DragArea.BeginUpdate();
			DesignerTransaction transaction = designerHost != null ? designerHost.CreateTransaction(Caption) : null;
			try {
				AddDashboardItemToDesignerHost(dashboard, InsertedDashboardItem);
				dashboard.BeginUpdate();
				try {
					if (PropertyNames == null)
						throw new InvalidOperationException();
					IComponentChangeService changeService = dashboard.GetService<IComponentChangeService>();
					if (changeService != null) {
						foreach (string propertyName in PropertyNames) {
							PropertyDescriptor property = Helper.GetProperty(dashboard, propertyName);
							changeService.OnComponentChanging(dashboard, property);
						}
					}
					PerformRedoAction(designer);
					if (changeService != null) {
						foreach (string propertyName in PropertyNames) {
							PropertyDescriptor property = Helper.GetProperty(dashboard, propertyName);
							changeService.OnComponentChanged(dashboard, property, null, null);
						}
					}
					PropertyDescriptor layoutPropertyDescriptor = Helper.GetProperty(dashboard, "Layout");
					if (changeService != null)
						changeService.OnComponentChanging(dashboard, layoutPropertyDescriptor);
					if(nextLayout != null) 
						dashboard.LayoutRoot = nextLayout;
					else { 
						Size clientSize = designer.Viewer.GetClientSize();
						RebuildLayout(dashboard, clientSize.Width, clientSize.Height);
						nextLayout = (DashboardLayoutGroup)dashboard.LayoutRoot.CopyTree();
					}
					if (changeService != null)
						changeService.OnComponentChanged(dashboard, layoutPropertyDescriptor, null, null);
				} finally {
					designer.Dashboard.EndUpdate();
				}
				RemoveDashboardItemFromDesignerHost(dashboard, RemovedDashboardItem);
			} finally {
				if(transaction != null)
					transaction.Commit();
			}
			designer.DragAreaScrollableControl.DragArea.EndUpdate();
		}
		void AddDashboardItemToDesignerHost(Dashboard dashboard, DashboardItem insertedDashboardItem) {
			if(insertedDashboardItem != null) {
				IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
				if(designerHost != null) {
					insertedDashboardItem.ComponentName = Helper.CreateDashboardComponentName(dashboard, insertedDashboardItem.GetType());
					designerHost.Container.Add(insertedDashboardItem, insertedDashboardItem.ComponentName);
				}
			}
		}
		protected void RemoveDashboardItemFromDesignerHost(Dashboard dashboard, DashboardItem removedDashboardItem) {
			if(removedDashboardItem != null) {
				IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
				if(designerHost != null)
					designerHost.DestroyComponent(removedDashboardItem);
			}
		}
		protected abstract void PerformUndoAction(DashboardDesigner designer);
		protected abstract void PerformRedoAction(DashboardDesigner designer);
		protected virtual void RebuildLayout(Dashboard dashboard, int width, int height) {
			dashboard.RebuildLayout(width, height);
		}
	}
}
