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
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.Native;
using DevExpress.Utils.Serializing;
using System.Drawing.Design;
using DevExpress.XtraReports.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Controls;
using System.Reflection;
using System.Diagnostics;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class DesignBarManagerConfiguratorBase : BarManagerConfigurator {
		#region static
		protected static CommandBarItem CreateCommandBarItem(ReportCommand command) {
			if(XRDesignItemsLogicBase.IsColorPopupCommand(command))
				return new CommandColorBarItem();
			else
				return new CommandBarItem();
		}
		#endregion
		CommandBarItemHashTable commandBarItemTable = new CommandBarItemHashTable();
		public XRDesignBarManager XRDesignBarManager {
			get { return manager as XRDesignBarManager; }
		}
		public DesignBarManagerConfiguratorBase(XRDesignBarManager manager)
			: base(manager) {
		}
		public void AddCommandBarItem(CommandBarItem barItem, ReportStringId captionId, ReportStringId hintId, int imageIndex, ReportCommand command, BarShortcut shortcut) {
			AddBarItem(barItem, ReportLocalizer.GetString(captionId), string.Empty, ReportLocalizer.GetString(hintId), imageIndex);
			barItem.Command = command;
			barItem.ItemShortcut = shortcut;
			commandBarItemTable.Add(command, barItem);
		}
		public void AddCommandBarItem(BarLinksHolder barLinksHolder, CommandBarItem barItem, string caption, int id, string name, string hint, int imageIndex, bool beginGroup, ReportCommand command, BarShortcut shortcut) {
			AddBarItem(barLinksHolder, barItem, caption, name, hint, imageIndex, beginGroup);
			barItem.Command = command;
			barItem.ItemShortcut = shortcut;
			commandBarItemTable.Add(command, barItem);
		}
		public void AddCommandBarItem(BarLinksHolder barLinksHolder, CommandBarItem barItem, string caption, int id, string name, string hint, Bitmap glyph, bool beginGroup, ReportCommand command, BarShortcut shortcut) {
			if(glyph != null)
				barItem.Glyph = glyph;
			AddCommandBarItem(barLinksHolder, barItem, caption, id, name, hint, -1, beginGroup, command, shortcut);
		}
		protected void AddCommandItemsToHolders(BarLinksHolder[] barLinksHolders, ReportCommand[] commands) {
			bool beginGroup = false;
			for(int i = 0; i < commands.Length; i++) {
				if(commands[i] == ReportCommand.None)
					continue;
				beginGroup = i > 0 && commands[i - 1] == ReportCommand.None;
				BarItem barItem = commandBarItemTable[commands[i]];
				if(barItem != null)
					AddLink(barLinksHolders[i], barItem, beginGroup);
			}
		}
	}
	public class DesignBarManagerConfigurator : DesignBarManagerConfiguratorBase {
		internal const string LayoutVersionKey = "Software\\Developer Express\\XtraReports\\XRDesignBarManager";
		internal const string LayoutVersionName = "DesignLayoutVersion";
		internal const string LayoutVersionValue = "1.3";
		Bar formattingToolBar;
		Bar layoutToolBar;
		Bar statusBar;
		Bar mainMenuBar;
		Bar toolBar;
		public DesignBarManagerConfigurator(XRDesignBarManager manager)
			: base(manager) {
			manager.ItemPress += manager_ItemPress;
		}
		void manager_ItemPress(object sender, ItemClickEventArgs e) {
			if(XRDesignBarManager == null || XRDesignBarManager.XRDesignPanel == null)
				return;
			XRDesignBarManager.XRDesignPanel.CommitPropertyGridData();
		}
		public override void ConfigInternal() {
			CreateBars();
			CreateItems();
		}
		protected virtual void CreateBars() {
			mainMenuBar = AddMainMenuBar(XRDesignBarManagerBarNames.MainMenu, 0, 0, DevExpress.XtraBars.BarDockStyle.Top, ReportLocalizer.GetString(ReportStringId.UD_Capt_MainMenuName));
			toolBar = AddBar(XRDesignBarManagerBarNames.Toolbar, 0, 1, DevExpress.XtraBars.BarDockStyle.Top, ReportLocalizer.GetString(ReportStringId.UD_Capt_ToolbarName));
			formattingToolBar = AddBar(XRDesignBarManagerBarNames.FormattingToolbar, 1, 1, DevExpress.XtraBars.BarDockStyle.Top, ReportLocalizer.GetString(ReportStringId.UD_Capt_FormattingToolbarName));
			layoutToolBar = AddBar(XRDesignBarManagerBarNames.LayoutToolbar, 0, 2, DevExpress.XtraBars.BarDockStyle.Top, ReportLocalizer.GetString(ReportStringId.UD_Capt_LayoutToolbarName));
			statusBar = AddStatusBar(XRDesignBarManagerBarNames.StatusBar, 0, 0, DevExpress.XtraBars.BarDockStyle.Bottom, ReportLocalizer.GetString(ReportStringId.UD_Capt_StatusBarName));
		}
		protected override Bar CreateBar() {
			return new DesignBar();
		}
		void CreateItems() {
			CreateIndependentItems();
			CreateDependentItems();
		}
		void CreateIndependentItems() {
			CreateFormattingBarItems();
			CreateLayoutBarItems();
			CreateToolBarItems();
			CreateStatusBarItems();
		}
		void CreateDependentItems() {
			CreateMainMenuItems();
		}
		#region CreateBarItems
		void CreateFormattingBarItems() {
			BarEditItem beiFontName = new DevExpress.XtraBars.BarEditItem();
			RepositoryItemComboBox ricbFontName = new DevExpress.XtraReports.UserDesigner.RecentlyUsedItemsComboBox();
			BarEditItem beiFontSize = new DevExpress.XtraBars.BarEditItem();
			RepositoryItemComboBox ricbFontSize = new DesignRepositoryItemComboBox();
			((System.ComponentModel.ISupportInitialize)(ricbFontSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(ricbFontName)).BeginInit();
			manager.RepositoryItems.Add(ricbFontName);
			manager.RepositoryItems.Add(ricbFontSize);
			AddBarItem(formattingToolBar, beiFontName, ReportLocalizer.GetString(ReportStringId.UD_TTip_FormatFontName), "beiFontName", ReportLocalizer.GetString(ReportStringId.UD_TTip_FormatFontName), -1, false);
			beiFontName.Edit = ricbFontName;
			beiFontName.Width = 120;
			ricbFontName.AutoHeight = false;
			ricbFontName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
			ricbFontName.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
			ricbFontName.DropDownRows = 12;
			AddBarItem(formattingToolBar, beiFontSize, ReportLocalizer.GetString(ReportStringId.UD_TTip_FormatFontSize), "beiFontSize", ReportLocalizer.GetString(ReportStringId.UD_TTip_FormatFontSize), -1, false);
			beiFontSize.Edit = ricbFontSize;
			beiFontSize.Width = 55;
			ricbFontSize.AutoHeight = false;
			ricbFontSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
			ricbFontSize.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
			string[] hints = {
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_FontBold), ReportLocalizer.GetString(ReportStringId.UD_Hint_FontItalic), ReportLocalizer.GetString(ReportStringId.UD_Hint_FontUnderline), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_ForegroundColor), ReportLocalizer.GetString(ReportStringId.UD_Hint_BackGroundColor), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_JustifyLeft), ReportLocalizer.GetString(ReportStringId.UD_Hint_JustifyCenter), ReportLocalizer.GetString(ReportStringId.UD_Hint_JustifyRight), ReportLocalizer.GetString(ReportStringId.UD_Hint_JustifyJustify), null
							 };
			string[] captions = {
									ReportLocalizer.GetString(ReportStringId.UD_Capt_FontBold), ReportLocalizer.GetString(ReportStringId.UD_Capt_FontItalic), ReportLocalizer.GetString(ReportStringId.UD_Capt_FontUnderline), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_ForegroundColor), ReportLocalizer.GetString(ReportStringId.UD_Capt_BackGroundColor), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_JustifyLeft), ReportLocalizer.GetString(ReportStringId.UD_Capt_JustifyCenter), ReportLocalizer.GetString(ReportStringId.UD_Capt_JustifyRight), ReportLocalizer.GetString(ReportStringId.UD_Capt_JustifyJustify), null
								};
			ReportCommand[] commands = {	
									   ReportCommand.FontBold, ReportCommand.FontItalic, ReportCommand.FontUnderline, ReportCommand.None,
									   ReportCommand.ForeColor, ReportCommand.BackColor, ReportCommand.None,
									   ReportCommand.JustifyLeft, ReportCommand.JustifyCenter, ReportCommand.JustifyRight, ReportCommand.JustifyJustify, ReportCommand.None
								   };
			BarShortcut[] shortcuts = new BarShortcut[] {
															new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B), new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I), new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U), null,
															null, null, null, 
															null, null, null, null, null
														};
			AddItemsToLinksHolder(formattingToolBar, captions, commands, 0, hints, shortcuts, 0);
			AddComponentToContainer(ricbFontName);
			AddComponentToContainer(ricbFontSize);
			((System.ComponentModel.ISupportInitialize)(ricbFontName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(ricbFontSize)).EndInit();
			XRDesignBarManager.FontNameBox = ricbFontName;
			XRDesignBarManager.FontSizeBox = ricbFontSize;
			XRDesignBarManager.FontNameEdit = beiFontName;
			XRDesignBarManager.FontSizeEdit = beiFontSize;
			XRDesignBarManager.FormattingToolbar = formattingToolBar;
			XRDesignBarManager.Toolbar = toolBar;
			XRDesignBarManager.LayoutToolbar = layoutToolBar;
		}
		void CreateLayoutBarItems() {
			string[] hints = { 
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_AlignToGrid), null, 
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_AlignLefts), ReportLocalizer.GetString(ReportStringId.UD_Hint_AlignCenters), ReportLocalizer.GetString(ReportStringId.UD_Hint_AlignRights), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_AlignTops), ReportLocalizer.GetString(ReportStringId.UD_Hint_AlignMiddles), ReportLocalizer.GetString(ReportStringId.UD_Hint_AlignBottoms), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_MakeSameSizeWidth), ReportLocalizer.GetString(ReportStringId.UD_Hint_MakeSameSizeSizeToGrid), ReportLocalizer.GetString(ReportStringId.UD_Hint_MakeSameSizeHeight), ReportLocalizer.GetString(ReportStringId.UD_Hint_MakeSameSizeBoth), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_SpacingMakeEqual), ReportLocalizer.GetString(ReportStringId.UD_Hint_SpacingIncrease), ReportLocalizer.GetString(ReportStringId.UD_Hint_SpacingDecrease), ReportLocalizer.GetString(ReportStringId.UD_Hint_SpacingRemove), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_SpacingMakeEqual), ReportLocalizer.GetString(ReportStringId.UD_Hint_SpacingIncrease), ReportLocalizer.GetString(ReportStringId.UD_Hint_SpacingDecrease), ReportLocalizer.GetString(ReportStringId.UD_Hint_SpacingRemove), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_CenterInFormHorizontally), ReportLocalizer.GetString(ReportStringId.UD_Hint_CenterInFormVertically), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_OrderBringToFront), ReportLocalizer.GetString(ReportStringId.UD_Hint_OrderSendToBack), null 
							 };
			string[] captions = { 
									ReportLocalizer.GetString(ReportStringId.UD_Capt_AlignToGrid), null, 
									ReportLocalizer.GetString(ReportStringId.UD_Capt_AlignLefts), ReportLocalizer.GetString(ReportStringId.UD_Capt_AlignCenters), ReportLocalizer.GetString(ReportStringId.UD_Capt_AlignRights), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_AlignTops), ReportLocalizer.GetString(ReportStringId.UD_Capt_AlignMiddles), ReportLocalizer.GetString(ReportStringId.UD_Capt_AlignBottoms), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_MakeSameSizeWidth), ReportLocalizer.GetString(ReportStringId.UD_Capt_MakeSameSizeSizeToGrid), ReportLocalizer.GetString(ReportStringId.UD_Capt_MakeSameSizeHeight), ReportLocalizer.GetString(ReportStringId.UD_Capt_MakeSameSizeBoth), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_SpacingMakeEqual), ReportLocalizer.GetString(ReportStringId.UD_Capt_SpacingIncrease), ReportLocalizer.GetString(ReportStringId.UD_Capt_SpacingDecrease), ReportLocalizer.GetString(ReportStringId.UD_Capt_SpacingRemove), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_SpacingMakeEqual), ReportLocalizer.GetString(ReportStringId.UD_Capt_SpacingIncrease), ReportLocalizer.GetString(ReportStringId.UD_Capt_SpacingDecrease), ReportLocalizer.GetString(ReportStringId.UD_Capt_SpacingRemove), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_CenterInFormHorizontally), ReportLocalizer.GetString(ReportStringId.UD_Capt_CenterInFormVertically), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_OrderBringToFront), ReportLocalizer.GetString(ReportStringId.UD_Capt_OrderSendToBack), null 
								};
			ReportCommand[] commands = {		
									   ReportCommand.AlignToGrid, ReportCommand.None, 
									   ReportCommand.AlignLeft, ReportCommand.AlignVerticalCenters, ReportCommand.AlignRight, ReportCommand.None,
									   ReportCommand.AlignTop, ReportCommand.AlignHorizontalCenters, ReportCommand.AlignBottom, ReportCommand.None,
									   ReportCommand.SizeToControlWidth, ReportCommand.SizeToGrid, ReportCommand.SizeToControlHeight, ReportCommand.SizeToControl, ReportCommand.None,
									   ReportCommand.HorizSpaceMakeEqual, ReportCommand.HorizSpaceIncrease, ReportCommand.HorizSpaceDecrease, ReportCommand.HorizSpaceConcatenate, ReportCommand.None,
									   ReportCommand.VertSpaceMakeEqual, ReportCommand.VertSpaceIncrease, ReportCommand.VertSpaceDecrease, ReportCommand.VertSpaceConcatenate, ReportCommand.None,
									   ReportCommand.CenterHorizontally, ReportCommand.CenterVertically, ReportCommand.None,
									   ReportCommand.BringToFront, ReportCommand.SendToBack, ReportCommand.None
								   };
			BarShortcut[] shortcuts = new BarShortcut[] {null, null, 
															null, null, null, null, 
															null, null, null, null, 
															null, null, null, null, null, 
															null, null, null, null, null, 
															null, null, null, null, null, 
															null, null, null, 
															null, null, null, 
			};
			AddItemsToLinksHolder(layoutToolBar, captions, commands, 0, hints, shortcuts, 17);
		}
		void CreateToolBarItems() {
			AddCommandBarItem(toolBar, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_NewReport), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_NewReport), 9, false, ReportCommand.NewReport, new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
			string[] hints = {   
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_OpenFile), ReportLocalizer.GetString(ReportStringId.UD_Hint_SaveFile), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_Cut), ReportLocalizer.GetString(ReportStringId.UD_Hint_Copy), ReportLocalizer.GetString(ReportStringId.UD_Hint_Paste), null,
								 ReportLocalizer.GetString(ReportStringId.UD_Hint_Undo), ReportLocalizer.GetString(ReportStringId.UD_Hint_Redo), null,
			};
			string[] captions = {   
									ReportLocalizer.GetString(ReportStringId.UD_Capt_OpenFile), ReportLocalizer.GetString(ReportStringId.UD_Capt_SaveFile), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_Cut), ReportLocalizer.GetString(ReportStringId.UD_Capt_Copy), ReportLocalizer.GetString(ReportStringId.UD_Capt_Paste), null,
									ReportLocalizer.GetString(ReportStringId.UD_Capt_Undo), ReportLocalizer.GetString(ReportStringId.UD_Capt_Redo), null,
			};
			ReportCommand[] commands = {   
									   ReportCommand.OpenFile, ReportCommand.SaveFile, ReportCommand.None, ReportCommand.Cut, ReportCommand.Copy, ReportCommand.Paste, ReportCommand.None,
									   ReportCommand.Undo, ReportCommand.Redo, ReportCommand.None,
			};
			BarShortcut[] shortcuts = new BarShortcut[] {   
															new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O), new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S), null,
															new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X), new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C), new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V), null,
															new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z), new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y), null,
			};
			AddItemsToLinksHolder(toolBar, captions, commands, 0, hints, shortcuts, 10);
		}
		void CreateStatusBarItems() {
			BarStaticItem hintStaticItem = new BarStaticItem();
			AddBarItem(statusBar, hintStaticItem, string.Empty, "bsiHint", "", -1, false);
			hintStaticItem.TextAlignment = System.Drawing.StringAlignment.Near;
			hintStaticItem.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
			hintStaticItem.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			XRDesignBarManager.HintStaticItem = hintStaticItem;
		}
		void CreateMainMenuItems() {
			BarSubItem bsiFile = AddBarSubItem(mainMenuBar, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_File), "msiFile", string.Empty, -1, false);
			BarSubItem bsiEdit = AddBarSubItem(mainMenuBar, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_Edit), "msiEdit", string.Empty, -1, false);
			CreateViewSubMenu();
			BarSubItem bsiFormat = AddBarSubItem(mainMenuBar, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_Format), "msiFormat", string.Empty, -1, false);
			AddCommandItemsToHolders(new BarLinksHolder[] { bsiFormat, bsiFormat, null }, new ReportCommand[] { ReportCommand.ForeColor, ReportCommand.BackColor, ReportCommand.None });
			BarSubItem bsiFont = AddBarSubItem(bsiFormat, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_Font), "msiFormat", string.Empty, -1, true);
			BarSubItem bsiJustify = AddBarSubItem(bsiFormat, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_Justify), "msiFormat", string.Empty, -1, false);
			BarSubItem bsiAlign = AddBarSubItem(bsiFormat, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_Align), "msiFormat", string.Empty, -1, true);
			BarSubItem bsiMakeSameSize = AddBarSubItem(bsiFormat, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_MakeSameSize), "msiFormat", string.Empty, -1, false);
			BarSubItem bsiHorizontalSpacing = AddBarSubItem(bsiFormat, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_HorizontalSpacing), "msiFormat", string.Empty, -1, true);
			BarSubItem bsiVerticalSpacing = AddBarSubItem(bsiFormat, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_VerticalSpacing), "msiFormat", string.Empty, -1, false);
			BarSubItem bsiCenterInForm = AddBarSubItem(bsiFormat, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_CenterInForm), "msiFormat", string.Empty, -1, true);
			BarSubItem bsiOrder = AddBarSubItem(bsiFormat, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_Order), "msiFormat", string.Empty, -1, true);
			AddCommandBarItem(null, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_NewWizardReport), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_NewWizardReport), XRBitmaps.NewReportWizard, false, ReportCommand.NewReportWizard, new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W));
			AddCommandBarItem(null, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_SaveFileAs), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_SaveFileAs), -1, false, ReportCommand.SaveFileAs, null);
			AddCommandBarItem(null, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_Exit), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_Exit), -1, true, ReportCommand.Exit, null);
			AddCommandBarItem(null, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_Delete), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_Delete), -1, false, ReportCommand.Delete, null);
			AddCommandBarItem(null, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_SelectAll), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_SelectAll), -1, true, ReportCommand.SelectAll, new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
			ReportCommand[] commands = new ReportCommand[] {
													   ReportCommand.NewReport, ReportCommand.NewReportWizard, ReportCommand.OpenFile, ReportCommand.None,
													   ReportCommand.SaveFile, ReportCommand.SaveFileAs, ReportCommand.None,
													   ReportCommand.Exit, ReportCommand.None,
													   ReportCommand.Undo, ReportCommand.Redo, ReportCommand.None,
													   ReportCommand.Cut, ReportCommand.Copy, ReportCommand.Paste, ReportCommand.Delete, ReportCommand.None,
													   ReportCommand.SelectAll, ReportCommand.None,
													   ReportCommand.FontBold, ReportCommand.FontItalic, ReportCommand.FontUnderline, ReportCommand.None,
													   ReportCommand.JustifyLeft, ReportCommand.JustifyCenter, ReportCommand.JustifyRight, ReportCommand.JustifyJustify, ReportCommand.None,
													   ReportCommand.AlignLeft, ReportCommand.AlignVerticalCenters, ReportCommand.AlignRight, ReportCommand.None, ReportCommand.AlignTop, ReportCommand.AlignHorizontalCenters, ReportCommand.AlignBottom, ReportCommand.None, ReportCommand.AlignToGrid, ReportCommand.None,
													   ReportCommand.SizeToControlWidth, ReportCommand.SizeToGrid, ReportCommand.SizeToControlHeight, ReportCommand.SizeToControl, ReportCommand.None,
													   ReportCommand.HorizSpaceMakeEqual, ReportCommand.HorizSpaceIncrease, ReportCommand.HorizSpaceDecrease, ReportCommand.HorizSpaceConcatenate, ReportCommand.None,
													   ReportCommand.VertSpaceMakeEqual, ReportCommand.VertSpaceIncrease, ReportCommand.VertSpaceDecrease, ReportCommand.VertSpaceConcatenate, ReportCommand.None,
													   ReportCommand.CenterHorizontally, ReportCommand.CenterVertically, ReportCommand.None,
													   ReportCommand.BringToFront, ReportCommand.SendToBack, ReportCommand.None
												   };
			BarSubItem[] barSubItems = new BarSubItem[] {
															bsiFile, bsiFile, bsiFile, null,
															bsiFile, bsiFile, null,
															bsiFile, null,
															bsiEdit, bsiEdit, null,
															bsiEdit, bsiEdit, bsiEdit, bsiEdit, null,
															bsiEdit, null,
															bsiFont, bsiFont, bsiFont, null,
															bsiJustify, bsiJustify, bsiJustify, bsiJustify, null,
															bsiAlign, bsiAlign, bsiAlign, null, bsiAlign, bsiAlign, bsiAlign, null, bsiAlign, null,
															bsiMakeSameSize, bsiMakeSameSize, bsiMakeSameSize, bsiMakeSameSize, null,
															bsiHorizontalSpacing, bsiHorizontalSpacing, bsiHorizontalSpacing, bsiHorizontalSpacing, null,
															bsiVerticalSpacing, bsiVerticalSpacing, bsiVerticalSpacing, bsiVerticalSpacing, null,
															bsiCenterInForm, bsiCenterInForm, null,
															bsiOrder, bsiOrder, null
														};
			AddCommandItemsToHolders(barSubItems, commands);
		}
		void CreateViewSubMenu() {
			BarSubItem bsiView = AddBarSubItem(mainMenuBar, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_View), "msiTabButtons", string.Empty, -1, false);
			AddBarItem(bsiView, new BarReportTabButtonsListItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_TabButtonsList), "", string.Empty, -1, false);
			BarSubItem bsiBarsList = AddBarSubItem(bsiView, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_ToolbarsList), "", string.Empty, -1, true);
			XRBarToolbarsListItem barToolbarsListItem = new XRBarToolbarsListItem();
			AddBarItem(bsiBarsList, barToolbarsListItem, ReportLocalizer.GetString(ReportStringId.UD_Group_ToolbarsList), "", string.Empty, -1, false);
			BarSubItem bsiDockPanelsList = AddBarSubItem(bsiView, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_DockPanelsList), "", string.Empty, -1, true);
			BarToolbarsListItem barDockPanelsListItem = new BarDockPanelsListItem();
			AddBarItem(bsiDockPanelsList, barDockPanelsListItem, ReportLocalizer.GetString(ReportStringId.UD_Group_DockPanelsList), "", string.Empty, -1, false);
		}
		#endregion //CreateBarItems
		void AddItemsToLinksHolder(BarLinksHolder barLinksHolder, string[] captions, ReportCommand[] commands, int id, string[] hints, BarShortcut[] shortcuts, int firstImageIndex) {
			bool beginGroup = false;
			for(int i = 0; i < commands.Length; i++) {
				if(commands[i] == ReportCommand.None)
					continue;
				beginGroup = i > 0 && commands[i - 1] == ReportCommand.None;
				AddCommandBarItem(barLinksHolder, CreateCommandBarItem(commands[i]), captions[i], 0, "bbi" + i.ToString(), hints[i], firstImageIndex, beginGroup, commands[i], shortcuts[i]);
				firstImageIndex++;
			}
		}
	}
	public abstract class CommandBarItemDesignBarManagerConfigurator : DesignBarManagerConfiguratorBase {
		ReportCommand command;
		ReportStringId captionId;
		ReportStringId hintId;
		System.Windows.Forms.Keys keys;
		string imageResource;
		CommandBarItem commandBarItem;
		protected CommandBarItem CommandBarItem {
			get {
				if(commandBarItem == null) {
					commandBarItem = new CommandBarItem();
					if(!string.IsNullOrEmpty(imageResource)) {
						ImageCollectionHelper.AddImagesToCollectionFromResources(XRDesignBarManager.ImageCollection, LocalResFinder.GetFullName(imageResource), LocalResFinder.Assembly);
						AddCommandBarItem(commandBarItem, captionId, hintId, XRDesignBarManager.Images.Count - 1, command, new BarShortcut(keys));
					} else
						AddCommandBarItem(commandBarItem, captionId, hintId, -1, command, new BarShortcut(keys));
				}
				return commandBarItem;
			}
		}
		public override bool UpdateNeeded {
			get {
				return XRDesignBarManager.GetBarItemsByReportCommand(command).Length == 0;
			}
		}
		protected CommandBarItemDesignBarManagerConfigurator(XRDesignBarManager manager, ReportCommand command, ReportStringId captionId, ReportStringId hintId, System.Windows.Forms.Keys keys, string imageResource)
			: base(manager) {
			this.command = command;
			this.captionId = captionId;
			this.hintId = hintId;
			this.keys = keys;
			this.imageResource = imageResource;
		}
		protected CommandBarItemDesignBarManagerConfigurator(XRDesignBarManager manager, ReportCommand command, ReportStringId captionId, ReportStringId hintId, System.Windows.Forms.Keys keys)
			: this(manager, command, captionId, hintId, keys, string.Empty) {
		}
		protected static bool ProcessItemLinks(BarLinksHolder barLinksHolder, DevExpress.Utils.Function2<bool, BarItemLinkCollection, int> predicate) {
			if(barLinksHolder != null) {
				foreach(BarItemLink barItemLink in barLinksHolder.ItemLinks) {
					if(ProcessItemLinks(barItemLink.Item as BarLinksHolder, predicate))
						return true;
					for(int i = 0; i < barLinksHolder.ItemLinks.Count; i++) {
						if(predicate(barLinksHolder.ItemLinks, i))
							return true;
					}
				}
			}
			return false;
		}
	}
	public class CloseBarItemDesignBarManagerConfigurator : CommandBarItemDesignBarManagerConfigurator {
		public CloseBarItemDesignBarManagerConfigurator(XRDesignBarManager manager)
			: base(manager, ReportCommand.Close, ReportStringId.UD_Capt_Close, ReportStringId.UD_Hint_Close, System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4) {
		}
		public override void ConfigInternal() {
			ProcessItemLinks(XRDesignBarManager.MainMenu, delegate(BarItemLinkCollection itemLinks, int i) {
				CommandBarItem item = itemLinks[i].Item as CommandBarItem;
				if(item != null && item.Command == ReportCommand.Exit) {
					BarItemLink barItemLink = itemLinks.Insert(i, CommandBarItem);
					barItemLink.BeginGroup = true;
					return true;
				}
				return false;
			});
		}
	}
	public class SaveAllBarItemDesignBarManagerConfigurator : CommandBarItemDesignBarManagerConfigurator {
		public SaveAllBarItemDesignBarManagerConfigurator(XRDesignBarManager manager)
			: base(manager, ReportCommand.SaveAll, ReportStringId.UD_Capt_SaveAll, ReportStringId.UD_Hint_SaveAll, System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L, "UserDesigner.SaveAll_16x16.png") {
		}
		public override void ConfigInternal() {
			ProcessItemLinks(XRDesignBarManager.Toolbar, delegate(BarItemLinkCollection itemLinks, int i) {
				CommandBarItem item = itemLinks[i].Item as CommandBarItem;
				if(item != null && item.Command == ReportCommand.SaveFile) {
					itemLinks.Insert(i + 1, CommandBarItem);
					return true;
				}
				return false;
			});
			ProcessItemLinks(XRDesignBarManager.MainMenu, delegate(BarItemLinkCollection itemLinks, int i) {
				CommandBarItem item = itemLinks[i].Item as CommandBarItem;
				if(item != null && item.Command == ReportCommand.SaveFileAs) {
					itemLinks.Insert(i + 1, CommandBarItem);
					return true;
				}
				return false;
			});
		}
	}
	public class MdiDesignBarManagerConfigurator : DesignBarManagerConfiguratorBase {
		int baseImageIndex;
		public override bool UpdateNeeded {
			get {
				return XRDesignBarManager.GetBarItemsByReportCommand(ReportCommand.MdiCascade).Length == 0;
			}
		}
		public MdiDesignBarManagerConfigurator(XRDesignBarManager manager) : base(manager) {
		}
		public override void ConfigInternal() {
			UpdateImageList();
			UpdateMainMenu();
		}
		void UpdateImageList() {
			baseImageIndex = XRDesignBarManager.Images.Count;
			XRDesignBarManager.Images.AddRange(XRBitmaps.GetMdiToolBarIcons().Images);
		}
		void UpdateMainMenu() {
			Bar mainMenu = XRDesignBarManager.MainMenu;
			if(mainMenu == null)
				return;
			BarSubItem bsiWindow = AddBarSubItem(mainMenu, new BarSubItem(), ReportLocalizer.GetString(ReportStringId.UD_Group_Window), "msiWindow", string.Empty, -1, false);
			CommandBarCheckItem commandBarCheckItem = new CommandBarCheckItem();
			commandBarCheckItem.Checked = true;
			commandBarCheckItem.CheckedCommand = ReportCommand.ShowTabbedInterface;
			commandBarCheckItem.UncheckedCommand = ReportCommand.ShowWindowInterface;
			AddBarItem(bsiWindow, commandBarCheckItem, ReportLocalizer.GetString(ReportStringId.UD_Capt_TabbedInterface), "msiWindows", ReportLocalizer.GetString(ReportStringId.UD_Hint_TabbedInterface), -1, true);
			AddCommandBarItem(null, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_MdiCascade), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_MdiCascade), baseImageIndex, false, ReportCommand.MdiCascade, null);
			AddCommandBarItem(null, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_MdiTileHorizontal), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_MdiTileHorizontal), baseImageIndex + 1, false, ReportCommand.MdiTileHorizontal, null);
			AddCommandBarItem(null, new CommandBarItem(), ReportLocalizer.GetString(ReportStringId.UD_Capt_MdiTileVertical), 0, string.Empty, ReportLocalizer.GetString(ReportStringId.UD_Hint_MdiTileVertical), baseImageIndex + 2, true, ReportCommand.MdiTileVertical, null);
			ReportCommand[] commands = new ReportCommand[] { ReportCommand.MdiCascade, ReportCommand.MdiTileHorizontal, ReportCommand.MdiTileVertical, ReportCommand.None };
			BarSubItem[] barSubItems = new BarSubItem[] { bsiWindow, bsiWindow, bsiWindow, null };
			AddCommandItemsToHolders(barSubItems, commands);
			AddBarItem(bsiWindow, new BarMdiChildrenListItem(), "Windows", "msiWindows", string.Empty, -1, true);
		}
	}
	public class ZoomDesignBarManagerConfigurator : DesignBarManagerConfiguratorBase {
		Bar zoomToolBar;
		int baseImageIndex;
		public override bool UpdateNeeded {
			get {
				foreach(Bar bar in XRDesignBarManager.Bars) {
					if(bar.BarName == XRDesignBarManagerBarNames.ZoomBar)
						return false;
				}
				return true;
			}
		}
		public ZoomDesignBarManagerConfigurator(XRDesignBarManager manager)
			: base(manager) {
		}
		public override void ConfigInternal() {
			UpdateImageList();
			CreateZoomBar();
			CreateZoomBarItems();
		}
		void UpdateImageList() {
			baseImageIndex = XRDesignBarManager.Images.Count;
			ImageCollectionHelper.AddImagesToCollectionFromResources(XRDesignBarManager.ImageCollection, LocalResFinder.GetFullName("UserDesigner.ZoomOut_16x16.png"), LocalResFinder.Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(XRDesignBarManager.ImageCollection, LocalResFinder.GetFullName("UserDesigner.ZoomIn_16x16.png"), LocalResFinder.Assembly);
		}
		void CreateZoomBar() {
			zoomToolBar = AddBar(XRDesignBarManagerBarNames.ZoomBar, 1, 2, DevExpress.XtraBars.BarDockStyle.Top, ReportLocalizer.GetString(ReportStringId.UD_Capt_ZoomToolbarName));
		}
		void CreateZoomBarItems() {
			AddCommandBarItem(zoomToolBar,
				CreateCommandBarItem(ReportCommand.ZoomOut), 
				ReportLocalizer.GetString(ReportStringId.UD_Capt_ZoomOut),
				0, 
				"bbiZoomOut", 
				ReportLocalizer.GetString(ReportStringId.UD_Hint_ZoomOut), 
				baseImageIndex + 0,
				false, 
				ReportCommand.ZoomOut,
				new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Subtract));
			CreateZoomItem();
			AddCommandBarItem(zoomToolBar,
				CreateCommandBarItem(ReportCommand.ZoomIn),
				ReportLocalizer.GetString(ReportStringId.UD_Capt_ZoomIn),
				0,
				"bbiZoomIn",
				ReportLocalizer.GetString(ReportStringId.UD_Hint_ZoomIn), 
				baseImageIndex + 1,
				false,
				ReportCommand.ZoomIn,
				new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Add));
		}
		void CreateZoomItem() {
			XRZoomBarEditItem zoomItem = new XRZoomBarEditItem();
			AddBarItem(zoomToolBar, zoomItem, ReportLocalizer.GetString(ReportStringId.UD_Capt_Zoom), "bbiZoom", ReportLocalizer.GetString(ReportStringId.UD_Hint_Zoom), -1, false);
			XRDesignBarManager.ZoomItem = zoomItem;
			DesignRepositoryItemComboBox comboBox = new DesignRepositoryItemComboBox();
			((System.ComponentModel.ISupportInitialize)(comboBox)).BeginInit();
			XRDesignBarManager.RepositoryItems.Add(comboBox);
			zoomItem.Edit = comboBox;
			zoomItem.Width = 70;
			zoomItem.EditValue = "100%";
			comboBox.AutoHeight = true;
			comboBox.AutoComplete = false;
			comboBox.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
			comboBox.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
			AddComponentToContainer(comboBox);
			((System.ComponentModel.ISupportInitialize)(comboBox)).EndInit();
		}
	}
}
