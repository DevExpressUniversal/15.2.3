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
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraEditors.Native;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.ilbStyles")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.btnAdd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.btnClose")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.edtNewStyleName")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.components")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.btnDelete")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.btnReset")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.lblSelectStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.DefineNewStylesForm.lblNewStyleName")]
#endregion
namespace DevExpress.XtraScheduler.Forms {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class DefineNewStylesForm : DevExpress.XtraEditors.XtraForm {
		SchedulerPrintStyleCollection printStyles;
		DefineNewStylesForm() {
			InitializeComponent();
		}
		public DefineNewStylesForm(DevExpress.XtraEditors.XtraForm parent, SchedulerPrintStyleCollection printStyles) {
			this.LookAndFeel.ParentLookAndFeel = parent.LookAndFeel.ParentLookAndFeel;
			InitializeComponent();
			this.printStyles = printStyles;
			ilbStyles.ImageList = SchedulerPrintStyle.SmallImageList;
			UpdatePrintStylesList();
		}
		public SchedulerPrintStyleCollection PrintStyles { get { return printStyles; } }
		public virtual void SetMenuManager(Utils.Menu.IDXMenuManager menuManager) {
			MenuManagerUtils.SetMenuManager(Controls, menuManager);
		}
		protected virtual void SubscribeEvents() {
			this.ilbStyles.SelectedIndexChanged += new System.EventHandler(this.ilbStyles_SelectedIndexChanged);
		}
		protected virtual void UnsubscribeEvents() {
			this.ilbStyles.SelectedIndexChanged -= new System.EventHandler(this.ilbStyles_SelectedIndexChanged);
		}
		void AddStyleItem(SchedulerPrintStyle printStyle) {
			int index = printStyle.GetImageListIndex();
			ilbStyles.Items.Add(printStyle, index);
		}
		void RemoveSelectedItem() {
			UnsubscribeEvents();
			ilbStyles.SuspendLayout();
			int selectedIndex = ilbStyles.SelectedIndex;
			ilbStyles.Items.RemoveAt(selectedIndex);
			ilbStyles.SelectedIndex = selectedIndex < ilbStyles.ItemCount ?
				selectedIndex : ilbStyles.ItemCount - 1;
			ilbStyles.ResumeLayout();
			SubscribeEvents();
			UpdateSelectedItem();
		}
		void UpdateSelectedItem() {
			SchedulerPrintStyle style = (SchedulerPrintStyle)ilbStyles.SelectedValue;
			if (IsBaseStyle(style))
				btnDelete.Enabled = false;
			else
				btnDelete.Enabled = true;
			int nCopy = 1;
			string newStyleName = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Format_CopyOf), style.DisplayName);
			while (DisplayNameExists(newStyleName)) {
				nCopy++;
				newStyleName = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Format_CopyNOf), nCopy, style.DisplayName);
			}
			edtNewStyleName.Text = newStyleName;
		}
		private bool IsBaseStyle(SchedulerPrintStyle style) {
			return style.BaseStyle;
		}
		void UpdatePrintStylesList() {
			UnsubscribeEvents();
			int count = PrintStyles.Count;
			ilbStyles.Items.Clear();
			for (int i = 0; i < count; i++) {
				SchedulerPrintStyle printStyle = PrintStyles[i];
				AddStyleItem(printStyle);
			}
			SubscribeEvents();
			ilbStyles.SelectedIndex = 0;
			UpdateSelectedItem();
		}
		bool DisplayNameExists(string displayName) {
			return PrintStyles.IsDisplayNameExists(displayName);
		}
		void ilbStyles_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateSelectedItem();
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			if (DisplayNameExists(edtNewStyleName.Text)) {
				String msg = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Msg_PrintStyleNameExists), edtNewStyleName.Text);
				DevExpress.XtraEditors.XtraMessageBox.Show(this.LookAndFeel, this, msg, SchedulerLocalizer.GetString(SchedulerStringId.Msg_Warning), MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			SchedulerPrintStyle style = (SchedulerPrintStyle)(ilbStyles.SelectedValue);
			SchedulerPrintStyle newStyle = (SchedulerPrintStyle)style.Clone(false);
			newStyle.DisplayName = edtNewStyleName.Text;
			PrintStyles.Add(newStyle);
			AddStyleItem(newStyle);
			UpdateSelectedItem();
			UpdatePrintStylesList();
		}
		private void btnReset_Click(object sender, System.EventArgs e) {
			SchedulerPrintStyle style = (SchedulerPrintStyle)(ilbStyles.SelectedValue);
			style.Reset();
		}
		private void btnDelete_Click(object sender, System.EventArgs e) {
			SchedulerPrintStyle style = (SchedulerPrintStyle)(ilbStyles.SelectedValue);
			RemoveSelectedItem();
			PrintStyles.DeleteStyle(style);
		}
		private void btnClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}
	}
}
