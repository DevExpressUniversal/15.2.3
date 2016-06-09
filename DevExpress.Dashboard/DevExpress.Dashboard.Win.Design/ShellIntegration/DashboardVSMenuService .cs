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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.Data;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.UI.Native;
using DevExpress.Design.VSIntegration;
using DevExpress.Utils.UI;
using Microsoft.VisualStudio.CommandBars;
namespace DevExpress.DashboardWin.Design {
	class DashboardVSMenuItem : VSToolWindowMenuItem {
		protected override Type ResFinderType {
			get { return typeof(DevExpress.DashboardWin.Design.ResFinder); }
		}
		public DashboardVSMenuItem(string caption, string bitmapResourceName, VSToolWindow toolWindow)
			: base(caption, bitmapResourceName, toolWindow) {
		}
	}
	abstract class CustomDashboardMenuItem : VSMenuItem {
		protected IServiceProvider ServiceProvider { get; set; }
		protected override Type ResFinderType {
			get { return typeof(DevExpress.DashboardWin.Design.ResFinder); }
		}
		public CustomDashboardMenuItem(IServiceProvider serviceProvider, string caption, string bitmapResourceName)
			: base(caption, bitmapResourceName) {
			this.ServiceProvider = serviceProvider;
		}		
	}
	class LoadDashboardMenuItem : CustomDashboardMenuItem {
		const string Caption = "Load Dashboard...";
		const string BitmapResourceName = "LoadDashboard";
		public LoadDashboardMenuItem(IServiceProvider serviceProvider)
			: base(serviceProvider, Caption, BitmapStorage.GetResourceName(BitmapResourceName)) {
		}
		protected override void Create(CommandBarControls parentCollection) {
			base.Create(parentCollection);
			Button.BeginGroup = true;
		}
		protected override void OnButtonClick() {
			DashboardDesigner designer = ServiceProvider.GetService<SelectedContextService>().Designer;
			UndoEngine undoEngine = ServiceProvider.GetService<UndoEngine>();
			bool previousEnabled = undoEngine.Enabled;
			undoEngine.Enabled = false;
			try {
				designer.OpenDashboard(null, null);
			}
			finally {
				undoEngine.Enabled = previousEnabled;
			}
		}
	}
	class SaveDashboardAsMenuItem : CustomDashboardMenuItem {
		const string Caption = "Save Dashboard As...";
		const string BitmapResourceName = "SaveDashboardAs";
		public SaveDashboardAsMenuItem(IServiceProvider serviceProvider)
			: base(serviceProvider, Caption, BitmapStorage.GetResourceName(BitmapResourceName)) {
		}
		protected override void OnButtonClick() {
			DashboardDesigner designer = ServiceProvider.GetService<SelectedContextService>().Designer;
			designer.SaveDashboard(null);
		}
	}
	class EditDataSourcesMenuItem : CustomDashboardMenuItem {
		const string Caption = "Edit Data Sources...";
		const string BitmapResourceName = "EditDataSources";
		public EditDataSourcesMenuItem(IServiceProvider serviceProvider)
			: base(serviceProvider, Caption, BitmapStorage.GetResourceName(BitmapResourceName)) {
		}
		protected override void OnButtonClick() {
			DashboardDesigner designer = ServiceProvider.GetService<SelectedContextService>().Designer;
			Dashboard dashboard = designer.Dashboard;
			EditorHelper.EditValue(dashboard, "DataSources", ServiceProvider);
		}
	}
}
