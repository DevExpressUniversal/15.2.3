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
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Commands;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DashboardWin.Native;
namespace DevExpress.DashboardWin.Bars {
	public abstract class DashboardRibbonPageGroup : ControlCommandBasedRibbonPageGroup<DashboardDesigner, DashboardCommandId> {
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			IDashboardDesignerSelectionService dashboardItemSelectionService = Control.RequestServiceStrictly<IDashboardDesignerSelectionService>();
			dashboardItemSelectionService.DashboardItemSelected += OnDashboardItemSelected;
			IDataSourceSelectionService dataSourceSelectionService = Control.RequestServiceStrictly<IDataSourceSelectionService>();
			dataSourceSelectionService.DataSourceSelected += OnDataSourceSelected;
			OnDashboardItemSelected(dashboardItemSelectionService.SelectedDashboardItem);
			OnDataSourceSelected(dataSourceSelectionService.SelectedDataSourceInfo.DataSource);
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			if(Control != null) {
				IDashboardDesignerSelectionService dashboardItemSelectionService = Control.RequestService<IDashboardDesignerSelectionService>();
				if(dashboardItemSelectionService != null)
					dashboardItemSelectionService.DashboardItemSelected -= OnDashboardItemSelected;
				IDataSourceSelectionService dataSourceSelectionService = Control.RequestService<IDataSourceSelectionService>();
				if(dataSourceSelectionService != null)
					dataSourceSelectionService.DataSourceSelected -= OnDataSourceSelected;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				UnsubscribeControlEvents();
 			base.Dispose(disposing);
		}
		void OnDataSourceSelected(object sender, DataSourceSelectedEventArgs e) {
			OnDataSourceSelected(e.DataSource);
		}
		void OnDashboardItemSelected(object sender, DashboardItemSelectedEventArgs e) {
			OnDashboardItemSelected(e.SelectedDashboardItem);
		}
		protected virtual void OnDashboardItemSelected(DashboardItem dashboardItem) { }
		protected virtual void OnDataSourceSelected(IDashboardDataSource dataSource) { }
	}
	public abstract class DashboardBarButtonItem : ControlCommandBarButtonItem<DashboardDesigner, DashboardCommandId> {
		public virtual bool AddToQuickAccess { get { return false; } }
	}
	public abstract class CommandBarCheckItem : ControlCommandBarCheckItem<DashboardDesigner, DashboardCommandId> {
		protected virtual bool ShouldRefreshSuperTip { get { return false; } }
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			base.OnControlUpdateUI(sender, e);
			if (ShouldRefreshSuperTip)
				UpdateSuperTipAndShortCut();
		} 
	}
	public abstract class DashboardCommandGalleryBarItem : ControlCommandGalleryBarItem<DashboardDesigner, DashboardCommandId> {
		protected override bool DropDownGalleryShowGroupCaption { get { return true; } }
		protected DashboardCommandGalleryBarItem() {
			AllowDrawArrow = true;
		}
		protected abstract void PrepareGallery();
		protected override void OnControlChanged() {
			base.OnControlChanged();
			InRibbonGallery gallery = Gallery;
			gallery.BeginUpdate();
			try {
				gallery.Groups.Clear();
				gallery.ImageSize = new Size(32, 32);
				PrepareGallery();
			}
			finally {
				gallery.EndUpdate();
			}
		}
		protected override void InvokeCommand() {
			base.InvokeCommand();
			Command command = CreateCommand();
			if(command != null)
				if(command.CanExecute())
					command.ForceExecute(CreateGalleryItemUIState());
		}
	}
	public abstract class CommandBarEditItem : ControlCommandBarEditItem<DashboardDesigner, DashboardCommandId, int> {
	}
	public abstract class CommandBarSubItem : ControlCommandBarSubItem<DashboardDesigner, DashboardCommandId> {
		protected CommandBarSubItem() {
			PaintStyle = BarItemPaintStyle.CaptionInMenu;
		}
	}
}
