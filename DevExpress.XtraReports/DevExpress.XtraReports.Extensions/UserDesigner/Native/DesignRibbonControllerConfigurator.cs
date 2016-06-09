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
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class XRDesignRibbonControllerConfigurator : RibbonControllerConfiguratorBase {
		#region static
		static XRDesignRibbonControllerConfigurator defaultInstance = new XRDesignRibbonControllerConfigurator(null, null, null);
		static readonly ReportCommand[] commandsOrder = new ReportCommand[] {
			ReportCommand.AlignToGrid,
			ReportCommand.AlignLeft, ReportCommand.AlignVerticalCenters, ReportCommand.AlignRight,
			ReportCommand.AlignTop, ReportCommand.AlignHorizontalCenters, ReportCommand.AlignBottom,
			ReportCommand.SizeToControlWidth, ReportCommand.SizeToGrid, ReportCommand.SizeToControlHeight, ReportCommand.SizeToControl,
			ReportCommand.HorizSpaceMakeEqual, ReportCommand.HorizSpaceIncrease, ReportCommand.HorizSpaceDecrease, ReportCommand.HorizSpaceConcatenate,
			ReportCommand.VertSpaceMakeEqual, ReportCommand.VertSpaceIncrease, ReportCommand.VertSpaceDecrease, ReportCommand.VertSpaceConcatenate, 
			ReportCommand.CenterHorizontally, ReportCommand.CenterVertically, 
			ReportCommand.BringToFront, ReportCommand.SendToBack,
		};
		static readonly ReportCommand[] commandsWithShortcuts = { 
			ReportCommand.FontBold, ReportCommand.FontItalic, ReportCommand.FontUnderline,
			ReportCommand.NewReport,
			ReportCommand.OpenFile, ReportCommand.SaveFile, ReportCommand.SaveAll,
			ReportCommand.Cut, ReportCommand.Copy, ReportCommand.Paste,
			ReportCommand.Undo, ReportCommand.Redo,
			ReportCommand.NewReportWizard,
			ReportCommand.SelectAll,
			ReportCommand.ZoomOut,
			ReportCommand.ZoomIn,
		};
		static readonly System.Windows.Forms.Keys[] commandShortcuts = { 
			System.Windows.Forms.Keys.B,  System.Windows.Forms.Keys.I,  System.Windows.Forms.Keys.U,
			System.Windows.Forms.Keys.N,
			System.Windows.Forms.Keys.O,  System.Windows.Forms.Keys.S,  System.Windows.Forms.Keys.L,
			System.Windows.Forms.Keys.X,  System.Windows.Forms.Keys.C,  System.Windows.Forms.Keys.V,
			System.Windows.Forms.Keys.Z,  System.Windows.Forms.Keys.Y,
			System.Windows.Forms.Keys.W,
			System.Windows.Forms.Keys.A,
			System.Windows.Forms.Keys.Subtract,
			System.Windows.Forms.Keys.Add,
		};
		public static Dictionary<string, Image> GetImagesFromAssembly() {
			Dictionary<string, Image> images = RibbonControllerConfiguratorBase.GetImagesFromAssembly(LocalResFinder.Assembly, RibbonImagesResourcePath, defaultInstance.RibbonImagesNamePrefix); ;
			ExtractFromContinuousImage(images);
			return images;
		}
		static void ExtractFromContinuousImage(Dictionary<string, Image> images) {
			DevExpress.Utils.ImageCollection imageCollection = ImageCollectionHelper.CreateVoidImageCollection();
			imageCollection.Images.AddRange(XRBitmaps.GetLayoutToolBarIcons().Images);
			for(int i = 0; i < commandsOrder.Length; i++) {
				string key = defaultInstance.RibbonImagesNamePrefix + commandsOrder[i].ToString();
				if(!images.ContainsKey(key))
					images[key] = imageCollection.Images[i];
			}
		}
		internal static void LocalizeStrings(RibbonControl ribbonControl) {
			defaultInstance.LocalizeStrings<XRDesignRibbonPageGroup, CommandBarItem, ZoomRuntimeCommandBarItem, XRDesignRibbonPageGroupKind>(ribbonControl);
			if(GetFontNameEditor(ribbonControl.Manager) != null)
				defaultInstance.LocalizeSuperToolTipStrings(ribbonControl, GetFontNameEditor(ribbonControl.Manager).SuperTip, defaultInstance.RibbonPrefix + "FontName");
			if(GetFontSizeEditor(ribbonControl.Manager) != null)
				defaultInstance.LocalizeSuperToolTipStrings(ribbonControl, GetFontSizeEditor(ribbonControl.Manager).SuperTip, defaultInstance.RibbonPrefix + "FontSize");
			foreach(BarItem item in ribbonControl.Items)
				if(item is BarDockPanelsListItem) {
					item.Caption = defaultInstance.ConditionalLocalizeString(item.Caption, ribbonControl, defaultInstance.RibbonPrefix + WindowsItemName + CaptionSuffix);
					defaultInstance.LocalizeSuperToolTipStrings(ribbonControl, item.SuperTip, defaultInstance.RibbonPrefix + WindowsItemName);
				} else if(item is ScriptsCommandBarItem) {
					item.Caption = defaultInstance.ConditionalLocalizeString(item.Caption, ribbonControl, defaultInstance.RibbonPrefix + ScriptsItemName + CaptionSuffix);
					defaultInstance.LocalizeSuperToolTipStrings(ribbonControl, item.SuperTip, defaultInstance.RibbonPrefix + ScriptsItemName);
				}
		}
		static BarItem CreateItemByCommand(ReportCommand command) {
			CommandBarItem item = XRDesignItemsLogicBase.IsColorPopupCommand(command) ? new CommandColorBarItem() : new CommandBarItem();
			item.Command = command;
			return item;
		}
		internal static BarEditItem GetFontNameEditor(BarManager manager) {
			foreach(BarItem item in manager.Items)
				if(item is BarEditItem && ((BarEditItem)item).Edit is XtraEditors.Repository.RepositoryItemFontEdit)
					return (BarEditItem)item;
			return null;
		}
		internal static BarEditItem GetFontSizeEditor(BarManager manager) {
			foreach(BarItem item in manager.Items)
				if(item is BarEditItem && ((BarEditItem)item).Edit is DesignRepositoryItemComboBox)
					return (BarEditItem)item;
			return null;
		}
		static void AddLink(BarItemLinkCollection itemLinks, BarItem item, bool beginGroup) {
			itemLinks.Add(item).BeginGroup = beginGroup;
		}
		static void AddLink(BarItemLinkCollection itemLinks, BarItem item, bool beginGroup, bool buttonGroup) {
			itemLinks.Add(item, beginGroup, string.Empty, string.Empty, buttonGroup);
		}
		#endregion
		static string RibbonImagesResourcePath = LocalResFinder.GetFullName("UserDesigner.RibbonImages.");
		const string WindowsItemName = "Windows";
		const string ScriptsItemName = "Scripts";
		RibbonPage designPage, htmlPage;
		RibbonPage toolboxPage;
		BarEditItem fontNameEditItem;
		BarEditItem fontSizeEditItem;
		ScriptsCommandBarItem scriptItem;
		BarDockPanelsListItem windowsItem;
		BarItem newReportDropDownBarItem;
		BarItem newReportBarItem;
		BarItem saveReportBarItem;
		BarItem saveAllReportsBarItem;
		BarItem openReportBarItem;
		public XRDesignRibbonControllerConfigurator(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> ribbonImages)
			: base(ribbonControl, ribbonStatusBar, ribbonImages) {
			RibbonPrefix = "RibbonXRDesign_";
			RibbonImagesNamePrefix = "RibbonUserDesigner_";
			ReferencedNamePrefix = "ReportStringId.";
		}
		#region items creation
		protected override void CreateItems() {
			CreateLayoutBarItems();
			CreateFormattingBarItems();
			CreateMainBarItems();
			CreateZoomItems();
			CreateFontItems();
			CreateWindowsItem();
			CreateScriptItem();
			CreateHtmlItems();
			AssignShortcuts();
			CreateApplicationMenu();
		}
		void CreateApplicationMenu() {
			ApplicationMenu applicationMenu = new ApplicationMenu();
			applicationMenu.ShowRightPane = true;
			applicationMenu.Ribbon = this.ribbonControl;
			applicationMenu.ItemLinks.AddRange(new BarItem[] { newReportDropDownBarItem, openReportBarItem, saveReportBarItem, saveAllReportsBarItem });
			applicationMenu.ItemLinks.Add(AddCloseBarItem(), true);
			applicationMenu.ItemLinks.Add(AddBarItem(ReportCommand.Exit), true);
			BarManagerConfigurator.AddToContainer(this.ribbonControl.Container, applicationMenu);
			this.ribbonControl.ApplicationButtonDropDownControl = applicationMenu;
		}
		BarItem AddCloseBarItem() {
			BarItem item = AddBarItem(ReportCommand.Close);
			item.ItemShortcut = new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4);
			return item;
		}
		void CreateLayoutBarItems() {
			AddBarItem(ReportCommand.AlignToGrid);
			AddBarItem(ReportCommand.AlignLeft);
			AddBarItem(ReportCommand.AlignVerticalCenters);
			AddBarItem(ReportCommand.AlignRight);
			AddBarItem(ReportCommand.AlignTop);
			AddBarItem(ReportCommand.AlignHorizontalCenters);
			AddBarItem(ReportCommand.AlignBottom);
			AddBarItem(ReportCommand.SizeToControlWidth);
			AddBarItem(ReportCommand.SizeToGrid);
			AddBarItem(ReportCommand.SizeToControlHeight);
			AddBarItem(ReportCommand.SizeToControl);
			AddBarItem(ReportCommand.HorizSpaceMakeEqual);
			AddBarItem(ReportCommand.HorizSpaceIncrease);
			AddBarItem(ReportCommand.HorizSpaceDecrease);
			AddBarItem(ReportCommand.HorizSpaceConcatenate);
			AddBarItem(ReportCommand.VertSpaceMakeEqual);
			AddBarItem(ReportCommand.VertSpaceIncrease);
			AddBarItem(ReportCommand.VertSpaceDecrease);
			AddBarItem(ReportCommand.VertSpaceConcatenate);
			AddBarItem(ReportCommand.CenterHorizontally);
			AddBarItem(ReportCommand.CenterVertically);
			AddBarItem(ReportCommand.BringToFront);
			AddBarItem(ReportCommand.SendToBack);
		}
		void CreateFormattingBarItems() {
			AddBarItem(ReportCommand.FontBold);
			AddBarItem(ReportCommand.FontItalic);
			AddBarItem(ReportCommand.FontUnderline);
			AddBarItem(ReportCommand.ForeColor);
			AddBarItem(ReportCommand.BackColor);
			AddBarItem(ReportCommand.JustifyLeft);
			AddBarItem(ReportCommand.JustifyCenter);
			AddBarItem(ReportCommand.JustifyRight);
			AddBarItem(ReportCommand.JustifyJustify);
		}
		void CreateMainBarItems() {
			newReportDropDownBarItem = AddBarItem(ReportCommand.NewReport, BarButtonStyle.DropDown);
			saveReportBarItem = AddBarItem(ReportCommand.SaveFile, BarButtonStyle.DropDown);
			saveAllReportsBarItem = AddBarItem(ReportCommand.SaveAll);
			openReportBarItem = AddBarItem(ReportCommand.OpenFile);
			newReportBarItem = AddBarItem(ReportCommand.NewReport);
			newReportBarItem.Description = ReportLocalizer.GetString(ReportStringId.RibbonXRDesign_NewReport_Description);
			BarItem newReportWizardBarItem = AddBarItem(ReportCommand.NewReportWizard);
			newReportWizardBarItem.Description = ReportLocalizer.GetString(ReportStringId.RibbonXRDesign_NewReportWizard_Description);
			BarItem saveFileBarItem = AddBarItem(ReportCommand.SaveFile);
			saveFileBarItem.Description = ReportLocalizer.GetString(ReportStringId.RibbonXRDesign_SaveFile_Description);
			BarItem saveFileAsBarItem = AddBarItem(ReportCommand.SaveFileAs);
			saveFileAsBarItem.Description = ReportLocalizer.GetString(ReportStringId.RibbonXRDesign_SaveFileAs_Description);
			AddBarItems(ReportCommand.Cut, ReportCommand.Copy, ReportCommand.Paste, ReportCommand.Undo, ReportCommand.Redo);
		}
		void CreateZoomItems() {
			AddBarItem(ReportCommand.Zoom, BarButtonStyle.DropDown);
			AddBarItem(ReportCommand.ZoomIn);
			AddBarItem(ReportCommand.ZoomOut);
		}
		void CreateFontItems() {
			fontNameEditItem = CreateBarEditItem(new RecentlyUsedItemsComboBox(), 140, "FontName");
			fontSizeEditItem = CreateBarEditItem(new DesignRepositoryItemComboBox(), 50, "FontSize");
		}
		protected void CreateScriptItem() {
			scriptItem = new ScriptsCommandBarItem();
			scriptItem.Caption = GetLocalizedString(defaultInstance.RibbonPrefix + ScriptsItemName + CaptionSuffix);
			AddBarItem(scriptItem, ScriptsItemName, RibbonItemStyles.Default, -1, null);
		}
		void CreateWindowsItem() {
			windowsItem = new BarDockPanelsListItem();
			AddBarItem(windowsItem, WindowsItemName, RibbonItemStyles.Default, -1, null);
		}
		void CreateHtmlItems() {
			AddBarItems(ReportCommand.HtmlBackward, ReportCommand.HtmlForward, ReportCommand.HtmlHome, ReportCommand.HtmlRefresh, ReportCommand.HtmlFind);
		}
		void AssignShortcuts() {
			for(int i = 0; i < commandsWithShortcuts.Length; i++) {
				foreach(BarItem item in XRDesignItemsLogicBase.GetBarItemsByReportCommand(ribbonControl.Manager, commandsWithShortcuts[i])) {
					if(item is BarButtonItem && ((BarButtonItem)item).ButtonStyle == BarButtonStyle.DropDown)
						continue;
					item.ItemShortcut = new BarShortcut(System.Windows.Forms.Keys.Control | commandShortcuts[i]);
				}
			}
		}
		BarEditItem CreateBarEditItem(RepositoryItem rItem, int width, string name) {
			BarEditItem editItem = new BarEditItem();
			editItem.Edit = rItem;
			editItem.Width = width;
			AddBarItem(editItem, name, RibbonItemStyles.Default, -1, null);
			return editItem;
		}
		void AddBarItems(params ReportCommand[] commands) {
			foreach(ReportCommand command in commands)
				AddBarItem(command);
		}
		BarItem AddBarItem(ReportCommand command, BarButtonStyle barButtonStyle) {
			BarButtonItem item = (BarButtonItem)CreateItemByCommand(command);
			item.ButtonStyle = barButtonStyle;
			AddBarItem(item, command.ToString(), RibbonItemStyles.Default, -1, null);
			return item;
		}
		BarItem AddBarItem(ReportCommand command) {
			BarItem barItem = CreateItemByCommand(command);
			AddBarItem(barItem, command.ToString(), RibbonItemStyles.Default, -1, null);
			return barItem;
		}
		#endregion
		#region groups creation
		protected override void CreatePageGroups() {
			designPage = CreateRibbonPage(typeof(XRDesignRibbonPage));
			ribbonControl.Pages.Add(designPage);
			CreateReportGroup();
			CreateEditGroup();
			CreateFontGroup();
			CreateAlignmentGroup();
			CreateSizeAndLayoutGroup();
			CreateZoomGroup();
			CreateViewGroup();
			CreateScriptGroup(designPage);
			toolboxPage = CreateRibbonPage(typeof(XRToolboxRibbonPage));
			toolboxPage.Text = ReportStringId.RibbonXRDesign_ToolboxControlsPage.GetString();
			XRToolboxPageCategory categoryControls = new XRToolboxPageCategory() { Visible = false };
			BarManagerConfigurator.AddToContainer(ribbonControl.Container, categoryControls);
			ribbonControl.PageCategories.Add(categoryControls);
			categoryControls.Pages.Add(toolboxPage);
			htmlPage = CreateRibbonPage(typeof(XRHtmlRibbonPage));
			ribbonControl.Pages.Add(htmlPage);
			CreateHtmlNavigationGroup();
		}
		void CreateReportGroup() {
			RibbonPageGroup group = CreatePageGroup(designPage, XRDesignRibbonPageGroupKind.Report, "Report");
			AddLinks(group, ReportCommand.NewReport, ReportCommand.OpenFile, ReportCommand.SaveFile, ReportCommand.SaveAll);
		}
		void CreateEditGroup() {
			RibbonPageGroup group = CreatePageGroup(designPage, XRDesignRibbonPageGroupKind.Edit, "Edit");
			AddLinks(group, ReportCommand.Cut, ReportCommand.Copy, ReportCommand.Paste);
			AddLink(group, ReportCommand.Undo, true);
			AddLink(group, ReportCommand.Redo, false);
		}
		void CreateFontGroup() {
			RibbonPageGroup group = CreatePageGroup(designPage, XRDesignRibbonPageGroupKind.Font, "Font");
			AddLink(group.ItemLinks, fontNameEditItem, false, true);
			AddButtonGroup(group, ReportCommand.BackColor, ReportCommand.ForeColor);
			AddLink(group.ItemLinks, fontSizeEditItem, false, true);
			AddButtonGroup(group, ReportCommand.FontBold, ReportCommand.FontItalic, ReportCommand.FontUnderline);
			AddButtonGroup(group, ReportCommand.JustifyLeft, ReportCommand.JustifyCenter, ReportCommand.JustifyRight, ReportCommand.JustifyJustify);
		}
		void CreateAlignmentGroup() {
			RibbonPageGroup group = CreatePageGroup(designPage, XRDesignRibbonPageGroupKind.Alignment, "AlignVerticalCenters");
			AddButtonGroup(group, ReportCommand.AlignToGrid, ReportCommand.AlignLeft, ReportCommand.AlignVerticalCenters, ReportCommand.AlignRight);
			AddButtonGroup(group, ReportCommand.AlignTop, ReportCommand.AlignHorizontalCenters, ReportCommand.AlignBottom);
		}
		void CreateSizeAndLayoutGroup() {
			RibbonPageGroup group = CreatePageGroup(designPage, XRDesignRibbonPageGroupKind.SizeAndLayout, "SizeToControl");
			AddButtonGroup(group, ReportCommand.SizeToGrid, ReportCommand.SizeToControlWidth, ReportCommand.SizeToControlHeight, ReportCommand.SizeToControl);
			AddButtonGroup(group, ReportCommand.HorizSpaceMakeEqual, ReportCommand.HorizSpaceIncrease, ReportCommand.HorizSpaceDecrease, ReportCommand.HorizSpaceConcatenate);
			AddButtonGroup(group, ReportCommand.VertSpaceMakeEqual, ReportCommand.VertSpaceIncrease, ReportCommand.VertSpaceDecrease, ReportCommand.VertSpaceConcatenate);
			AddButtonGroup(group, ReportCommand.CenterHorizontally, ReportCommand.CenterVertically, ReportCommand.BringToFront, ReportCommand.SendToBack);
		}
		void CreateZoomGroup() {
			RibbonPageGroup group = CreatePageGroup(designPage, XRDesignRibbonPageGroupKind.Zoom, "Zoom");
			AddLinks(group, ReportCommand.ZoomOut, ReportCommand.Zoom, ReportCommand.ZoomIn);
		}
		void CreateViewGroup() {
			RibbonPageGroup group = CreatePageGroup(designPage, XRDesignRibbonPageGroupKind.View, "Windows");
			AddLink(group.ItemLinks, windowsItem, false);
		}
		protected void CreateScriptGroup(RibbonPage page) {
			RibbonPageGroup group = CreatePageGroup(page, XRDesignRibbonPageGroupKind.Scripts, "Scripts");
			AddLink(group.ItemLinks, scriptItem, false);
		}
		void CreateHtmlNavigationGroup() {
			RibbonPageGroup group = CreatePageGroup(htmlPage, XRDesignRibbonPageGroupKind.HtmlNavigation, "Find");
			AddLinks(group, ReportCommand.HtmlBackward, ReportCommand.HtmlForward);
			AddLink(group, ReportCommand.HtmlHome, true);
			AddLink(group, ReportCommand.HtmlRefresh, false);
			AddLink(group, ReportCommand.HtmlFind, true);
		}
		XRDesignRibbonPageGroup CreatePageGroup(RibbonPage ribbonPage, XRDesignRibbonPageGroupKind kind, string glyphName) {
			XRDesignRibbonPageGroup group = CreatePageGroup<XRDesignRibbonPageGroup, XRDesignRibbonPageGroupKind>(ribbonPage, kind, glyphName);
			return group;
		}
		void AddLinks(RibbonPageGroup pageGroup, params ReportCommand[] commands) {
			AddLinks(pageGroup.ItemLinks, commands);
		}
		void AddLinks(BarItemLinkCollection itemLinks, params ReportCommand[] commands) {
			foreach(ReportCommand command in commands)
				AddLink(itemLinks, command, false);
		}
		void AddButtonGroup(RibbonPageGroup pageGroup, params ReportCommand[] commands) {
			BarButtonGroup buttonGroup = new XRDesignBarButtonGroup();
			AddLinks(buttonGroup.ItemLinks, commands);
			AddBarItem(buttonGroup);
			pageGroup.ItemLinks.Add(buttonGroup);
		}
		void AddLink(RibbonPageGroup pageGroup, ReportCommand command, bool beginGroup) {
			AddLink(pageGroup.ItemLinks, command, beginGroup);
		}
		void AddLink(BarItemLinkCollection itemLinks, ReportCommand command, bool beginGroup) {
			BarItem[] items = XRDesignItemsLogicBase.GetBarItemsByReportCommand(ribbonControl.Manager, command);
			if(items.Length > 0)
				AddLink(itemLinks, items[0], beginGroup);
		}
		#endregion
		protected override string GetLocalizedString(string str) {
			return ReportLocalizer.GetString((ReportStringId)Enum.Parse(typeof(ReportStringId), str));
		}
		protected override string GetDefaultLocalizedString(string str) {
#if DEBUGTEST
			System.Diagnostics.Debug.WriteLineIf(!Enum.IsDefined(typeof(ReportStringId), str), str);
#endif
			return Enum.IsDefined(typeof(ReportStringId), str) ? ReportLocalizer.Default.GetLocalizedString((ReportStringId)Enum.Parse(typeof(ReportStringId), str)) : null;
		}
		protected override string BarItemCommandToString(BarItem item) {
			return ((CommandBarItem)item).Command.ToString();
		}
		protected override string GetTextForRibbonPage(RibbonPage page) {
			return page is XRDesignRibbonPage ? PageText : page is XRHtmlRibbonPage ? RibbonPrefix + "HtmlPageText" : null;
		}
	}
	public class ScriptsRibbonUpdater : XRDesignRibbonControllerConfigurator {
		public static bool GetUpdateNeeded(RibbonBarManager manager) {
			foreach(BarItem barItem in manager.Items) {
				ISupportReportCommand item = barItem as ISupportReportCommand;
				if(item != null && item.Command == ReportCommand.ShowScriptsTab)
					return false;
			}
			return true;
		}
		public ScriptsRibbonUpdater(RibbonControl ribbonControl, RibbonStatusBar statusBar, Dictionary<string, Image> ribbonImages)
			: base(ribbonControl, statusBar, ribbonImages) {
		}
		protected override void CreateItems() {
			CreateScriptItem();
		}
		protected override void CreatePageGroups() {
			foreach(RibbonPage page in ribbonControl.Pages) {
				if(page is XRDesignRibbonPage) {
					CreateScriptGroup(page);
					break;
				}
			}
		}
	}
}
