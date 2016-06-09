#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.XtraEditors.Native;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.GotoDateForm.btnOK")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.GotoDateForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.GotoDateForm.lblDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.GotoDateForm.lblShowIn")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.GotoDateForm.grpGroup")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.GotoDateForm.edtDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.GotoDateForm.cbShowIn")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	#region GotoDateForm
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class GotoDateForm : XtraForm {
		#region Fields
		SchedulerViewRepository views;
		DateTime date;
		SchedulerViewType targetView;
		#endregion
		public GotoDateForm() {
			InitializeComponent();
		}
		public GotoDateForm(SchedulerViewRepository views, DateTime date)
			: this(views, date, GetTargetView(views)) {
		}
		public GotoDateForm(SchedulerViewRepository views, DateTime date, SchedulerViewType viewType) {
			this.views = views;
			this.date = date;
			InitializeComponent();
			this.Icon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.GoToDate, Assembly.GetExecutingAssembly());
			UpdateShowInComboBox();
			edtDate.DateTime = date;
			this.targetView = viewType;
			cbShowIn.EditValue = targetView;
		}
		#region Properties
		protected SchedulerViewRepository Views { get { return views; } }
		public DateTime Date { get { return date; } set { date = value; } }
		public SchedulerViewType TargetView { get { return targetView; } set { targetView = value; } }
		#endregion
		public virtual void SetMenuManager(Utils.Menu.IDXMenuManager menuManager) {
			MenuManagerUtils.SetMenuManager(Controls, menuManager);
		}
		static SchedulerViewType GetTargetView(SchedulerViewRepository views) {
			return views != null && views.Count > 0 ? views[0].Control.ActiveView.Type : SchedulerViewType.Day;
		}
		protected void UpdateShowInComboBox() {
			ImageComboBoxItemCollection items = cbShowIn.Properties.Items;
			for (int i = 0; i < views.Count; i++) {
				SchedulerViewBase view = views[i];
				if (view.Enabled)
					items.Add(new ImageComboBoxItem(view.DisplayName, view.Type, -1));
			}
		}
		private void btnOK_Click(object sender, System.EventArgs e) {
			TargetView = (SchedulerViewType)cbShowIn.EditValue;
			Date = edtDate.DateTime;
		}
	}
	#endregion
}
