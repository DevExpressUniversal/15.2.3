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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraNavBar {
	public partial class NavPaneOptionsForm : XtraForm {
		PaneCustomizationHelper customizationHelper;
		public NavPaneOptionsForm()
			: this(null, null) {
		}
		public NavPaneOptionsForm(NavBarControl navBar, PaneCustomizationHelper hr) {
			this.NavBar = navBar;
			this.customizationHelper = hr;
			InitializeComponent();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			Initialize();
		}
		void Initialize() {
			if(NavBar == null)
				return;
			ApplyLocalization();
			InitializeSourceState();
			InitializeLocation();
			InitializeCheckList();
		}
		void ApplyLocalization() {
			Text = NavBarLocalizationHelper.GetString(NavBarStringId.NavPaneMenuPaneOptions).Replace("&", string.Empty);
			btnMoveUp.Text = NavBarLocalizationHelper.GetString(NavBarStringId.NavPaneOptionsFormMoveUp);
			btnMoveDown.Text = NavBarLocalizationHelper.GetString(NavBarStringId.NavPaneOptionsFormMoveDown);
			btnFont.Text = NavBarLocalizationHelper.GetString(NavBarStringId.NavPaneOptionsFormFont);
			btnReset.Text = NavBarLocalizationHelper.GetString(NavBarStringId.NavPaneOptionsFormReset);
			btnOk.Text = NavBarLocalizationHelper.GetString(NavBarStringId.NavPaneOptionsFormOk);
			btnCancel.Text = NavBarLocalizationHelper.GetString(NavBarStringId.NavPaneOptionsFormCancel);
			labelControl.Text = NavBarLocalizationHelper.GetString(NavBarStringId.NavPaneOptionsFormDescription);
		}
		void InitializeSourceState() {
			NavBar.InitNavBarSourceStateInfo();
		}
		void InitializeLocation() {
			Form parentForm = NavBar.FindForm();
			if(parentForm == null) {
				StartPosition = FormStartPosition.CenterScreen;
				return;
			}
			int x = parentForm.Location.X + parentForm.Width / 2 - Width / 2;
			int y = parentForm.Location.Y + parentForm.Height / 2 - Height / 2;
			Location = new Point(x, y);
		}
		void InitializeCheckList() {
			if(NavBar.Groups.Count == 0)
				return;
			groupsCheckList.Items.Clear();
			foreach(NavBarGroup group in NavBar.Groups) {
				GroupListBoxItemInfo item = new GroupListBoxItemInfo(group);
				item.CheckState = group.IsVisible ? CheckState.Checked : CheckState.Unchecked;
				groupsCheckList.Items.Add(item);
			}
			groupsCheckList.SelectedIndex = 0;
			UpdateButtons();
		}
		void UpdateButtons() {
			int pos = groupsCheckList.SelectedIndex;
			if(pos == -1) {
				btnFont.Enabled = btnMoveUp.Enabled = btnMoveDown.Enabled = false;
				return;
			}
			btnMoveUp.Enabled = pos > 0;
			btnFont.Enabled = CanEditGroupFont();
			btnMoveDown.Enabled = pos < groupsCheckList.Items.Count - 1;
		}
		void MoveItemUp() {
			MoveItem(true);
		}
		void MoveItemDown() {
			MoveItem(false);
		}
		void MoveItem(bool up) {
			int pos = groupsCheckList.SelectedIndex;
			CheckedListBoxItem item = groupsCheckList.Items[pos];
			groupsCheckList.Items.Remove(item);
			int newPos = pos + (up ? -1 : 1);
			groupsCheckList.Items.Insert(newPos, item);
			groupsCheckList.SelectedIndex = newPos;
		}
		#region Handlers
		void btnMoveUp_Click(object sender, EventArgs e) {
			MoveItemUp();
			UpdateButtons();
		}
		void btnMoveDown_Click(object sender, EventArgs e) {
			MoveItemDown();
			UpdateButtons();
		}
		void btnFont_Click(object sender, EventArgs e) {
			Font font = GetItemsFont(GetSelectedGroup());
			if(font == null)
				return;
			GroupListBoxItemInfo itemInfo = (GroupListBoxItemInfo)groupsCheckList.SelectedItem;
			itemInfo.Font = font;
		}
		bool isResetClicked = false;
		void btnReset_Click(object sender, EventArgs e) {
			UpdateButtons();
			ResetSettings();
			this.isResetClicked = true;
		}
		void groupsCheckList_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateButtons();
		}
		void btnOk_Click(object sender, EventArgs e) {
			if(ShouldRaiseNavPaneOptionsResetEvent) RaiseNavPaneOptionsReset();
			DialogResult = DialogResult.OK;
		}
		bool ShouldRaiseNavPaneOptionsResetEvent {
			get {
				NavBarCustomizationInfo res = CreateCustomizationInfo();
				return isResetClicked && NavBar.StateInfo.Equals(res);
			}
		}
		#endregion
		#region Helpers
		NavBarCustomizationInfo CreateCustomizationInfo() {
			return customizationHelper.Fill(groupsCheckList);
		}
		Font GetItemsFont(NavBarGroup group) {
			Font res = null;
			Font initFont = customizationHelper.GetGroupFont(group);
			using(FontDialog fontDialog = new FontDialog()) {
				if(initFont != null) fontDialog.Font = initFont;
				if(fontDialog.ShowDialog() == DialogResult.OK)
					res = fontDialog.Font;
			}
			return res;
		}
		bool CanEditGroupFont() {
			NavBarGroup group = GetSelectedGroup();
			if(group == null)
				return false;
			bool res = group.GroupStyle != NavBarGroupStyle.ControlContainer;
			NavPaneOptionsCanEditGroupFontEventArgs e = new NavPaneOptionsCanEditGroupFontEventArgs(group, res);
			NavBar.RaiseNavPaneOptionsCanEditGroupFont(e);
			return e.Result;
		}
		void ResetSettings() {
			customizationHelper.Restore(groupsCheckList, NavBar.StateInfo);
			UpdateButtons();
		}
		NavBarGroup GetSelectedGroup() {
			int pos = groupsCheckList.SelectedIndex;
			if(pos == -1)
				return null;
			GroupListBoxItemInfo info = (GroupListBoxItemInfo)groupsCheckList.Items[pos];
			return info.Group;
		}
		void RaiseNavPaneOptionsReset() {
			NavPaneOptionsResetEventArgs e = new NavPaneOptionsResetEventArgs(NavBar);
			NavBar.RaiseNavPaneOptionsReset(e);
		}
		#endregion
		#region Public
		public NavBarCustomizationInfo CustomizationInfo {
			get { return CreateCustomizationInfo(); }
		}
		#endregion
		NavBarControl NavBar { get; set; }
	}
}
