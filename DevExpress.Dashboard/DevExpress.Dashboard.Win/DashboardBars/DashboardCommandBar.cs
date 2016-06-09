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

using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.XtraBars.Commands;
using System;
namespace DevExpress.DashboardWin.Bars {
	public abstract class DashboardCommandBar : ControlCommandBasedBar<DashboardDesigner, DashboardCommandId> {
		readonly Locker locker = new Locker();
		bool manualChangedVisibility = false;
		public override DashboardDesigner Control {
			get { return base.Control; }
			set {
				UnsubscribeEvents();
				base.Control = value;
				SubscribeEvents();
			}
		}
		IServiceProvider ServiceProvider { get { return Control; } }
		protected DashboardCommandBar() {
			VisibleChanged += (sender, e) => {
				if (!locker.IsLocked)
					manualChangedVisibility = Visible;
			};
		}
		protected override void Dispose(bool disposing) {		 
			if (disposing)
				UnsubscribeEvents();
			base.Dispose(disposing);
		}
		protected virtual bool CheckBarVisibility(DashboardItemViewer viewer) {
			return true;
		}
		void OnLayoutItemSelected(object sender, DashboardLayoutItemSelectedEventArgs e) {
			locker.Lock();
			try {
				if (manualChangedVisibility)
					Visible = true;
				else if (!Control.IsDashboardVSDesignMode) {
					if (Control.BarManagerVisible) {
						DashboardItemViewer itemViewer = null;
						if(e.SelectedItem != null)
							itemViewer = e.SelectedItem.ItemViewer;
						Visible = CheckBarVisibility(itemViewer);
					}
					else
						Visible = false;
				}
			}
			finally {
				locker.Unlock();
			}
		}
		void SubscribeEvents() {
			if (ServiceProvider != null) {
				IDashboardLayoutSelectionService selectionService = ServiceProvider.RequestService<IDashboardLayoutSelectionService>();
				if (selectionService != null)
					selectionService.ItemSelected += OnLayoutItemSelected;
			}
		}
		void UnsubscribeEvents() {
			if (ServiceProvider != null) {
				IDashboardLayoutSelectionService selectionService = ServiceProvider.RequestService<IDashboardLayoutSelectionService>();
				if (selectionService != null)
					selectionService.ItemSelected -= OnLayoutItemSelected;
			}
		}
	}
}
