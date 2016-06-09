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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Localization;
namespace DevExpress.XtraBars.Customization {
	[Description("Allows you to implement a custom Customization window for a BarManager component."),
	 DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation), DXToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "Customization.CustomizationControl")
	]
	public class CustomizationControl : System.Windows.Forms.UserControl {
		public System.Windows.Forms.Timer timer1;
		public DevExpress.XtraEditors.SimpleButton btClose;
		public XtraTabPage tpOptions;
		public XtraTabPage tpCommands;
		public XtraTabPage tpToolbars;
		public DevExpress.XtraEditors.CheckedListBoxControl toolBarsList;
		public DevExpress.XtraEditors.SimpleButton btNewBar;
		public DevExpress.XtraEditors.SimpleButton btDeleteBar;
		public DevExpress.XtraEditors.LabelControl lbDescription;
		public DevExpress.XtraBars.Customization.ItemsListBox lbCommands;
		public DevExpress.XtraEditors.ListBoxControl lbCategories;
		public System.Windows.Forms.Label lCommands;
		public System.Windows.Forms.Label lCategories;
		public DevExpress.XtraEditors.CheckEdit cbOptionsShowFullMenus;
		public DevExpress.XtraEditors.CheckEdit cbOptions_showFullMenusAfterDelay;
		public DevExpress.XtraEditors.SimpleButton btOptions_Reset;
		public DevExpress.XtraEditors.CheckEdit cbOptions_showTips;
		public DevExpress.XtraEditors.CheckEdit cbOptions_ShowShortcutInTips;
		public System.Windows.Forms.Label lbDescCaption;
		public LabelControl lbOptions_Other;
		public LabelControl lbOptions_PCaption;
		public XtraTabControl tabControl;
		public System.Windows.Forms.Label lbCategoriesCaption;
		public System.Windows.Forms.Label lbCommandsCaption;
		public System.Windows.Forms.Label lbToolbarCaption;
		public LabelControl lbDivider3;
		public DevExpress.XtraEditors.SimpleButton btResetBar;
		private XtraTabPage customResources;
		public System.Windows.Forms.Label lbNBDlgCaption;
		public DevExpress.XtraEditors.TextEdit tbNBDlgName;
		public DevExpress.XtraEditors.SimpleButton btNBDlgCancel;
		public DevExpress.XtraEditors.SimpleButton btNBDlgOk;
		public System.Windows.Forms.Panel pnlNBDlg;
		public DevExpress.XtraEditors.SimpleButton btRenameBar;
		public DevExpress.XtraEditors.CheckEdit cbOptions_largeIcons;
		public System.Windows.Forms.Label lbOptions_MenuAnimation;
		public DevExpress.XtraEditors.ComboBoxEdit cbOptions_MenuAnimation;
		private System.ComponentModel.IContainer components;
		public CustomizationControl() {
			InitializeComponent();
			tabControl.TabPages.Remove(customResources);
			this.lbNBDlgCaption.Text = BarLocalizer.Active.GetLocalizedString(BarString.ToolbarNameCaption);
		}
		public virtual CustomizationControl Clone() {
			return Activator.CreateInstance(this.GetType()) as CustomizationControl;
		}
		public virtual string WindowCaption { get { return GetLocalizedString(BarString.CustomizeWindowCaption); } }
		public virtual string GetLocalizedString(BarString str) {
			return BarLocalizer.Active.GetLocalizedString(str);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
			}
			base.Dispose(disposing);
			if(components != null) 
				components.Dispose();
			components = null;
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizationControl));
			this.btNewBar = new DevExpress.XtraEditors.SimpleButton();
			this.lbCategoriesCaption = new System.Windows.Forms.Label();
			this.lbCommandsCaption = new System.Windows.Forms.Label();
			this.btClose = new DevExpress.XtraEditors.SimpleButton();
			this.lbToolbarCaption = new System.Windows.Forms.Label();
			this.lbDescCaption = new System.Windows.Forms.Label();
			this.lbDivider3 = new DevExpress.XtraEditors.LabelControl();
			this.tpToolbars = new DevExpress.XtraTab.XtraTabPage();
			this.btRenameBar = new DevExpress.XtraEditors.SimpleButton();
			this.btResetBar = new DevExpress.XtraEditors.SimpleButton();
			this.btDeleteBar = new DevExpress.XtraEditors.SimpleButton();
			this.toolBarsList = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.lbDescription = new DevExpress.XtraEditors.LabelControl();
			this.tpOptions = new DevExpress.XtraTab.XtraTabPage();
			this.cbOptions_MenuAnimation = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lbOptions_MenuAnimation = new System.Windows.Forms.Label();
			this.cbOptions_largeIcons = new DevExpress.XtraEditors.CheckEdit();
			this.cbOptions_ShowShortcutInTips = new DevExpress.XtraEditors.CheckEdit();
			this.cbOptions_showTips = new DevExpress.XtraEditors.CheckEdit();
			this.lbOptions_Other = new DevExpress.XtraEditors.LabelControl();
			this.btOptions_Reset = new DevExpress.XtraEditors.SimpleButton();
			this.cbOptions_showFullMenusAfterDelay = new DevExpress.XtraEditors.CheckEdit();
			this.cbOptionsShowFullMenus = new DevExpress.XtraEditors.CheckEdit();
			this.lbOptions_PCaption = new DevExpress.XtraEditors.LabelControl();
			this.lCommands = new System.Windows.Forms.Label();
			this.lCategories = new System.Windows.Forms.Label();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tpCommands = new DevExpress.XtraTab.XtraTabPage();
			this.lbCommands = new DevExpress.XtraBars.Customization.ItemsListBox();
			this.lbCategories = new DevExpress.XtraEditors.ListBoxControl();
			this.customResources = new DevExpress.XtraTab.XtraTabPage();
			this.pnlNBDlg = new System.Windows.Forms.Panel();
			this.lbNBDlgCaption = new System.Windows.Forms.Label();
			this.tbNBDlgName = new DevExpress.XtraEditors.TextEdit();
			this.btNBDlgCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btNBDlgOk = new DevExpress.XtraEditors.SimpleButton();
			this.timer1 = new System.Windows.Forms.Timer();
			this.tpToolbars.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.toolBarsList)).BeginInit();
			this.tpOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_MenuAnimation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_largeIcons.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_ShowShortcutInTips.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_showTips.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_showFullMenusAfterDelay.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptionsShowFullMenus.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.tpCommands.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbCommands)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCategories)).BeginInit();
			this.customResources.SuspendLayout();
			this.pnlNBDlg.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbNBDlgName.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btNewBar, "btNewBar");
			this.btNewBar.Name = "btNewBar";
			resources.ApplyResources(this.lbCategoriesCaption, "lbCategoriesCaption");
			this.lbCategoriesCaption.BackColor = System.Drawing.Color.Transparent;
			this.lbCategoriesCaption.Name = "lbCategoriesCaption";
			resources.ApplyResources(this.lbCommandsCaption, "lbCommandsCaption");
			this.lbCommandsCaption.BackColor = System.Drawing.Color.Transparent;
			this.lbCommandsCaption.Name = "lbCommandsCaption";
			resources.ApplyResources(this.btClose, "btClose");
			this.btClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btClose.Name = "btClose";
			resources.ApplyResources(this.lbToolbarCaption, "lbToolbarCaption");
			this.lbToolbarCaption.BackColor = System.Drawing.Color.Transparent;
			this.lbToolbarCaption.Name = "lbToolbarCaption";
			resources.ApplyResources(this.lbDescCaption, "lbDescCaption");
			this.lbDescCaption.BackColor = System.Drawing.Color.Transparent;
			this.lbDescCaption.Name = "lbDescCaption";
			resources.ApplyResources(this.lbDivider3, "lbDivider3");
			this.lbDivider3.LineVisible = true;
			this.lbDivider3.Name = "lbDivider3";
			this.tpToolbars.Controls.Add(this.btRenameBar);
			this.tpToolbars.Controls.Add(this.btResetBar);
			this.tpToolbars.Controls.Add(this.btDeleteBar);
			this.tpToolbars.Controls.Add(this.btNewBar);
			this.tpToolbars.Controls.Add(this.lbToolbarCaption);
			this.tpToolbars.Controls.Add(this.toolBarsList);
			this.tpToolbars.Name = "tpToolbars";
			resources.ApplyResources(this.tpToolbars, "tpToolbars");
			resources.ApplyResources(this.btRenameBar, "btRenameBar");
			this.btRenameBar.Name = "btRenameBar";
			resources.ApplyResources(this.btResetBar, "btResetBar");
			this.btResetBar.Name = "btResetBar";
			resources.ApplyResources(this.btDeleteBar, "btDeleteBar");
			this.btDeleteBar.Name = "btDeleteBar";
			resources.ApplyResources(this.toolBarsList, "toolBarsList");
			this.toolBarsList.Name = "toolBarsList";
			resources.ApplyResources(this.lbDescription, "lbDescription");
			this.lbDescription.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lbDescription.Appearance.BackColor")));
			this.lbDescription.Name = "lbDescription";
			this.tpOptions.Controls.Add(this.cbOptions_MenuAnimation);
			this.tpOptions.Controls.Add(this.lbOptions_MenuAnimation);
			this.tpOptions.Controls.Add(this.cbOptions_largeIcons);
			this.tpOptions.Controls.Add(this.cbOptions_ShowShortcutInTips);
			this.tpOptions.Controls.Add(this.cbOptions_showTips);
			this.tpOptions.Controls.Add(this.lbOptions_Other);
			this.tpOptions.Controls.Add(this.btOptions_Reset);
			this.tpOptions.Controls.Add(this.cbOptions_showFullMenusAfterDelay);
			this.tpOptions.Controls.Add(this.cbOptionsShowFullMenus);
			this.tpOptions.Controls.Add(this.lbOptions_PCaption);
			this.tpOptions.Name = "tpOptions";
			resources.ApplyResources(this.tpOptions, "tpOptions");
			resources.ApplyResources(this.cbOptions_MenuAnimation, "cbOptions_MenuAnimation");
			this.cbOptions_MenuAnimation.Name = "cbOptions_MenuAnimation";
			this.cbOptions_MenuAnimation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbOptions_MenuAnimation.Properties.Buttons"))))});
			this.cbOptions_MenuAnimation.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lbOptions_MenuAnimation, "lbOptions_MenuAnimation");
			this.lbOptions_MenuAnimation.BackColor = System.Drawing.Color.Transparent;
			this.lbOptions_MenuAnimation.Name = "lbOptions_MenuAnimation";
			resources.ApplyResources(this.cbOptions_largeIcons, "cbOptions_largeIcons");
			this.cbOptions_largeIcons.Name = "cbOptions_largeIcons";
			this.cbOptions_largeIcons.Properties.Caption = resources.GetString("cbOptions_largeIcons.Properties.Caption");
			resources.ApplyResources(this.cbOptions_ShowShortcutInTips, "cbOptions_ShowShortcutInTips");
			this.cbOptions_ShowShortcutInTips.Name = "cbOptions_ShowShortcutInTips";
			this.cbOptions_ShowShortcutInTips.Properties.Caption = resources.GetString("cbOptions_ShowShortcutInTips.Properties.Caption");
			resources.ApplyResources(this.cbOptions_showTips, "cbOptions_showTips");
			this.cbOptions_showTips.Name = "cbOptions_showTips";
			this.cbOptions_showTips.Properties.Caption = resources.GetString("cbOptions_showTips.Properties.Caption");
			this.lbOptions_Other.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lbOptions_Other.Appearance.BackColor")));
			resources.ApplyResources(this.lbOptions_Other, "lbOptions_Other");
			this.lbOptions_Other.LineVisible = true;
			this.lbOptions_Other.Name = "lbOptions_Other";
			resources.ApplyResources(this.btOptions_Reset, "btOptions_Reset");
			this.btOptions_Reset.Name = "btOptions_Reset";
			resources.ApplyResources(this.cbOptions_showFullMenusAfterDelay, "cbOptions_showFullMenusAfterDelay");
			this.cbOptions_showFullMenusAfterDelay.Name = "cbOptions_showFullMenusAfterDelay";
			this.cbOptions_showFullMenusAfterDelay.Properties.Caption = resources.GetString("cbOptions_showFullMenusAfterDelay.Properties.Caption");
			resources.ApplyResources(this.cbOptionsShowFullMenus, "cbOptionsShowFullMenus");
			this.cbOptionsShowFullMenus.Name = "cbOptionsShowFullMenus";
			this.cbOptionsShowFullMenus.Properties.Caption = resources.GetString("cbOptionsShowFullMenus.Properties.Caption");
			this.lbOptions_PCaption.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lbOptions_PCaption.Appearance.BackColor")));
			resources.ApplyResources(this.lbOptions_PCaption, "lbOptions_PCaption");
			this.lbOptions_PCaption.LineVisible = true;
			this.lbOptions_PCaption.Name = "lbOptions_PCaption";
			resources.ApplyResources(this.lCommands, "lCommands");
			this.lCommands.Name = "lCommands";
			resources.ApplyResources(this.lCategories, "lCategories");
			this.lCategories.Name = "lCategories";
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tpToolbars;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tpToolbars,
			this.tpCommands,
			this.customResources,
			this.tpOptions});
			this.tpCommands.Controls.Add(this.lCommands);
			this.tpCommands.Controls.Add(this.lCategories);
			this.tpCommands.Controls.Add(this.lbCommandsCaption);
			this.tpCommands.Controls.Add(this.lbCategoriesCaption);
			this.tpCommands.Controls.Add(this.lbDescription);
			this.tpCommands.Controls.Add(this.lbDivider3);
			this.tpCommands.Controls.Add(this.lbDescCaption);
			this.tpCommands.Controls.Add(this.lbCommands);
			this.tpCommands.Controls.Add(this.lbCategories);
			this.tpCommands.Name = "tpCommands";
			resources.ApplyResources(this.tpCommands, "tpCommands");
			this.lbCommands.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lbCommands.Appearance.BackColor")));
			this.lbCommands.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.lbCommands, "lbCommands");
			this.lbCommands.Name = "lbCommands";
			this.lbCommands.HorizontalScrollbar = true;
			resources.ApplyResources(this.lbCategories, "lbCategories");
			this.lbCategories.Name = "lbCategories";
			this.customResources.Controls.Add(this.pnlNBDlg);
			this.customResources.Name = "customResources";
			resources.ApplyResources(this.customResources, "customResources");
			this.pnlNBDlg.Controls.Add(this.lbNBDlgCaption);
			this.pnlNBDlg.Controls.Add(this.tbNBDlgName);
			this.pnlNBDlg.Controls.Add(this.btNBDlgCancel);
			this.pnlNBDlg.Controls.Add(this.btNBDlgOk);
			resources.ApplyResources(this.pnlNBDlg, "pnlNBDlg");
			this.pnlNBDlg.Name = "pnlNBDlg";
			resources.ApplyResources(this.lbNBDlgCaption, "lbNBDlgCaption");
			this.lbNBDlgCaption.Name = "lbNBDlgCaption";
			resources.ApplyResources(this.tbNBDlgName, "tbNBDlgName");
			this.tbNBDlgName.Name = "tbNBDlgName";
			this.btNBDlgCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btNBDlgCancel, "btNBDlgCancel");
			this.btNBDlgCancel.Name = "btNBDlgCancel";
			this.btNBDlgOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btNBDlgOk, "btNBDlgOk");
			this.btNBDlgOk.Name = "btNBDlgOk";
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.btClose);
			this.Name = "CustomizationControl";
			resources.ApplyResources(this, "$this");
			this.tpToolbars.ResumeLayout(false);
			this.tpToolbars.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.toolBarsList)).EndInit();
			this.tpOptions.ResumeLayout(false);
			this.tpOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_MenuAnimation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_largeIcons.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_ShowShortcutInTips.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_showTips.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptions_showFullMenusAfterDelay.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOptionsShowFullMenus.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tpCommands.ResumeLayout(false);
			this.tpCommands.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbCommands)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCategories)).EndInit();
			this.customResources.ResumeLayout(false);
			this.pnlNBDlg.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tbNBDlgName.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
	}
	[Obsolete("You should use CustomizationControl + BarLocalizer instead of XtraBarsLocalizationManager"), ToolboxItem(false)]
	public class XtraBarsLocalizationManager : CustomizationControl {
	}
}
