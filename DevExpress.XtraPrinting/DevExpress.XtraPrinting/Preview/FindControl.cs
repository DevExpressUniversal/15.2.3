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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Native.Navigation;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using System.Reflection;
using DevExpress.XtraPrinting.Localization;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Preview {
	[DXToolboxItem(false)]
	public class FindControl : XtraUserControl {
		#region fields
		bool isRTLChanged = false;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup grpButtons;
		private DevExpress.XtraEditors.DropDownButton ddBtnParameters;
		private DevExpress.XtraEditors.TextEdit textEdit;
		private DevExpress.XtraEditors.LabelControl lbSearch;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraEditors.SimpleButton btnClose;
		private DevExpress.XtraEditors.SimpleButton btnFindNext;
		private DevExpress.XtraEditors.SimpleButton btnFindPrev;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private System.ComponentModel.IContainer components = null;
		BarManager barManager;
		PopupMenu popupMenu;
		SearchHelperBase searchHelper;
		readonly BarCheckItem caseSensitive;
		readonly BarCheckItem wholeWords;
		#endregion
		public TextEdit TextEdit { get { return textEdit; } }
		public SelectionService SelectionService { get; set; }
		public Func<PrintingSystemBase> PrintingSystem { get; set; }
		public Action RaiseRequestClose { get; set; }
		public FindControl() {
			InitializeComponent();
			this.BackColor = Color.Transparent;
			ddBtnParameters.Image = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraPrinting.Images.SearchSettingsButton.png"));
			barManager = new BarManager();
			barManager.Controller = new DevExpress.XtraBars.BarAndDockingController();
			barManager.Controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			barManager.Form = this;
			UpdateButtonsState();
			popupMenu = new PopupMenu(barManager);
			popupMenu.BeginUpdate();
			try {
				caseSensitive = new BarCheckItem(barManager);
				caseSensitive.Caption = new System.ComponentModel.ComponentResourceManager(typeof(FindControl)).GetString("caseSensitiveBarCheckItem.Text");
				popupMenu.AddItem(caseSensitive);
				wholeWords = new BarCheckItem(barManager);
				wholeWords.Caption = new System.ComponentModel.ComponentResourceManager(typeof(FindControl)).GetString("wholeWordsBarCheckItem.Text");
				popupMenu.AddItem(wholeWords);
			} finally {
				popupMenu.EndUpdate();
			}
			ddBtnParameters.DropDownControl = popupMenu;
			ddBtnParameters.LostFocus += (sender, e) => ddBtnParameters.HideDropDown();
			caseSensitive.CheckedChanged += OnFindOptionChanged;
			wholeWords.CheckedChanged += OnFindOptionChanged;
			textEdit.TextChanged += (sender, e) => {
				UpdateButtonsState();
				searchHelper.ResetSearchResults();
			};
			textEdit.KeyDown += (sender, e) => {
				if(e.KeyCode == Keys.Enter && btnFindNext.Enabled)
					FindText(SearchDirection.Down);
			};
			searchHelper = new XtraPrinting.Native.Navigation.SearchHelperBase();
		}
		public void Reset() {
			searchHelper.Reset();
		}
		public void FocusTextEditor() {
			textEdit.Focus();
		}
		public void SetTextSelection() {
			if(!string.IsNullOrEmpty(textEdit.Text)) {
				textEdit.SelectionStart = 0;
				textEdit.SelectionLength = textEdit.Text.Length;
			}
		}
		public int GetValidHeight() {
			return Math.Max(Bounds.Height, (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize.Height);
		}
		void OnFindOptionChanged(object sender, ItemClickEventArgs e) {
			searchHelper.ResetSearchResults();
		}
		const int WM_KEYDOWN = 0x100;
		protected override bool ProcessKeyPreview(ref Message m) {
			if(m.Msg == WM_KEYDOWN && (int)m.WParam == (int)Keys.Escape && RaiseRequestClose != null) {
				RaiseRequestClose();
				return true;
			}
			return base.ProcessKeyPreview(ref m);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(popupMenu != null) {
					popupMenu.Dispose();
					popupMenu = null;
				}
				if(barManager != null) {
					barManager.Dispose();
					barManager = null;
				}
				if(components != null) components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindControl));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.btnFindNext = new DevExpress.XtraEditors.SimpleButton();
			this.btnFindPrev = new DevExpress.XtraEditors.SimpleButton();
			this.ddBtnParameters = new DevExpress.XtraEditors.DropDownButton();
			this.textEdit = new DevExpress.XtraEditors.TextEdit();
			this.lbSearch = new DevExpress.XtraEditors.LabelControl();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.textEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.btnClose);
			this.layoutControl1.Controls.Add(this.btnFindNext);
			this.layoutControl1.Controls.Add(this.btnFindPrev);
			this.layoutControl1.Controls.Add(this.ddBtnParameters);
			this.layoutControl1.Controls.Add(this.textEdit);
			this.layoutControl1.Controls.Add(this.lbSearch);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(274, 143, 1003, 849);
			this.layoutControl1.Root = this.grpButtons;
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.Name = "btnClose";
			this.btnClose.StyleController = this.layoutControl1;
			this.btnClose.Click += new System.EventHandler(this.OnBtnCloseClick);
			resources.ApplyResources(this.btnFindNext, "btnFindNext");
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.StyleController = this.layoutControl1;
			this.btnFindNext.Click += new System.EventHandler(this.OnBtnFindNextClick);
			resources.ApplyResources(this.btnFindPrev, "btnFindPrev");
			this.btnFindPrev.Name = "btnFindPrev";
			this.btnFindPrev.StyleController = this.layoutControl1;
			this.btnFindPrev.Click += new System.EventHandler(this.OnBtnFindPrevClick);
			this.ddBtnParameters.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
			resources.ApplyResources(this.ddBtnParameters, "ddBtnParameters");
			this.ddBtnParameters.Name = "ddBtnParameters";
			this.ddBtnParameters.StyleController = this.layoutControl1;
			resources.ApplyResources(this.textEdit, "textEdit");
			this.textEdit.Name = "textEdit";
			this.textEdit.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbSearch, "lbSearch");
			this.lbSearch.Name = "lbSearch";
			this.lbSearch.StyleController = this.layoutControl1;
			this.grpButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem6});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 0);
			this.grpButtons.Name = "Root";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition1.Width = 33D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 5D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 164D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 5D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition5.Width = 46D;
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition6.Width = 5D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition7.Width = 69D;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition8.Width = 5D;
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition9.Width = 69D;
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition10.Width = 8D;
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition11.Width = 69D;
			this.grpButtons.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5,
			columnDefinition6,
			columnDefinition7,
			columnDefinition8,
			columnDefinition9,
			columnDefinition10,
			columnDefinition11});
			rowDefinition1.Height = 22D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1});
			this.grpButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.grpButtons.Size = new System.Drawing.Size(478, 22);
			this.grpButtons.TextVisible = false;
			this.layoutControlItem1.Control = this.lbSearch;
			this.layoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem1.Size = new System.Drawing.Size(33, 22);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem1.TrimClientAreaToControl = false;
			this.layoutControlItem2.Control = this.textEdit;
			this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem2.Location = new System.Drawing.Point(38, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
			this.layoutControlItem2.Size = new System.Drawing.Size(164, 22);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem2.TrimClientAreaToControl = false;
			this.layoutControlItem3.Control = this.ddBtnParameters;
			this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem3.Location = new System.Drawing.Point(207, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem3.Size = new System.Drawing.Size(46, 22);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem3.TrimClientAreaToControl = false;
			this.layoutControlItem4.Control = this.btnFindPrev;
			this.layoutControlItem4.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem4.Location = new System.Drawing.Point(258, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 6;
			this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem4.Size = new System.Drawing.Size(69, 22);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem4.TrimClientAreaToControl = false;
			this.layoutControlItem5.Control = this.btnFindNext;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem5.Location = new System.Drawing.Point(332, 0);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 8;
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem5.Size = new System.Drawing.Size(69, 22);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem5.TrimClientAreaToControl = false;
			this.layoutControlItem6.Control = this.btnClose;
			this.layoutControlItem6.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem6.Location = new System.Drawing.Point(409, 0);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.ColumnIndex = 10;
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem6.Size = new System.Drawing.Size(69, 22);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem6.TrimClientAreaToControl = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.MinimumSize = new System.Drawing.Size(468, 22);
			this.Name = "FindControl";
			this.Load += new System.EventHandler(this.FindControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.textEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void UpdateButtonsState() {
			bool enabled = !String.IsNullOrEmpty(textEdit.Text);
			btnFindNext.Enabled = enabled;
			btnFindPrev.Enabled = enabled;
		}
		void OnBtnFindNextClick(object sender, EventArgs e) {
			FindText(SearchDirection.Down);
		}
		void OnBtnFindPrevClick(object sender, EventArgs e) {
			FindText(SearchDirection.Up);
		}
		void OnBtnCloseClick(object sender, EventArgs e) {
			if(RaiseRequestClose != null)
				RaiseRequestClose();
		}
		void FindText(SearchDirection direction) {
			if(!FindNext(textEdit.Text, direction, wholeWords.Checked, caseSensitive.Checked))
				XtraMessageBox.Show(LookAndFeel, FindForm(), new System.ComponentModel.ComponentResourceManager(typeof(FindControl)).GetString("noMatches.Text"),
					 PreviewStringId.Msg_Caption.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		bool FindNext(string text, SearchDirection searchDirection, bool matchWholeWord, bool matchCase) {
			if(PrintingSystem == null) return false;
			SelectionService.ResetSelectedBricks();
			SelectionService.OnSearchStarted();
			PrintingSystemBase ps = PrintingSystem();
			BrickPagePair pair = searchHelper.CircleFindNext(ps, text, searchDirection, matchWholeWord, matchCase);
			if(pair != null)
				SelectionService.SelectBrick(pair.GetPage(ps.Pages), pair.GetBrick(ps.Pages));
			SelectionService.InvalidateControl();
			return pair != null;
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			isRTLChanged = true;
		}
		private void FindControl_Load(object sender, EventArgs e) {
			InitializeLayout();  
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			if(isRTLChanged)
				DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(grpButtons);
		}
		void InitializeGroupButtonsLayout() {
			int btnFindPrevBestWidth = btnFindPrev.CalcBestSize().Width;
			int btnFindNextBestWidth = btnFindNext.CalcBestSize().Width;
			int btnCloseBestWidth = btnClose.CalcBestSize().Width;
			if(btnFindPrevBestWidth <= btnFindPrev.Width && btnFindNextBestWidth <= btnFindNext.Width && btnCloseBestWidth <= btnClose.Width)
				return;
			int btnsActualSize = Math.Max(btnFindPrevBestWidth, Math.Max(btnFindNextBestWidth, btnCloseBestWidth));
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[6].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[8].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[10].Width = btnsActualSize;
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[4].Width = Math.Max(ddBtnParameters.CalcBestSize().Width, ddBtnParameters.Width);
		}
	}
}
