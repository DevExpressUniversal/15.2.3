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
using DevExpress.XtraBars;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraEditors.Repository;
using System.Drawing;
namespace DevExpress.XtraPrinting.Preview.Native {
	public class PrintBarManagerConfigurator : BarManagerConfigurator {
		#region static
		#region tests
#if DEBUGTEST
		static bool CanCreateScaleItem = true;
		public static bool CanCreateSaveOpenItems = true;
#endif
#endregion
		public static void AssignTextEditToZoomBarEditItem(BarManager manager, ZoomBarEditItemBase zoomItem, RepositoryItemTextEdit textEdit) {
			((System.ComponentModel.ISupportInitialize)(textEdit)).BeginInit();
			manager.RepositoryItems.Add(textEdit);
			zoomItem.Edit = textEdit;
			zoomItem.Width = 70;
			zoomItem.EditValue = "100%";
			((System.ComponentModel.ISupportInitialize)(textEdit)).EndInit();
		}
		#endregion
		internal const string LayoutVersionKey = "Software\\Developer Express\\XtraPrinting\\PrintBarManager";
		internal const string LayoutVersionName = "PreviewLayoutVersion";
		internal const string LayoutVersionValue = "1.1";
		Bar previewBar;
		Bar menuBar;
		public PrintBarManager PrintBarManager { get { return manager as PrintBarManager; } }
		public PrintBarManagerConfigurator(PrintBarManager manager) : base (manager) {
		}
		public override void ConfigInternal() {
			previewBar = AddBar(PrintBarManagerBarNames.Toolbar, 0, 0, BarDockStyle.Top, PreviewLocalizer.GetString(PreviewStringId.BarText_Toolbar));	   
			PrintBarManager.PreviewBar = previewBar;
			AddStatusBar(PrintBarManagerBarNames.StatusBar, 0, 0, BarDockStyle.Bottom, PreviewLocalizer.GetString(PreviewStringId.BarText_StatusBar));
			menuBar = AddMainMenuBar(PrintBarManagerBarNames.MainMenu, 0, 0, BarDockStyle.Top, PreviewLocalizer.GetString(PreviewStringId.BarText_MainMenu));
			AddStatusPanelItems();
			CreatePreviewBarItems();
			if(menuBar != null)
				CreateMenuItems();
			CreateDropDownMenusItems();
		}
		protected override Bar CreateBar() {
			return new PreviewBar();
		}
		public void AddStatusPanelItems() {
			Bar previewStatus = manager.StatusBar;
			System.Diagnostics.Debug.Assert(previewStatus != null);
			AddBarItem(previewStatus, PrintPreviewStaticItemFactory.CreateStaticItem(StatusPanelID.PageOfPages), -1, false);
			BarStaticItem staticItem = new BarStaticItem();
			staticItem.Visibility = BarItemVisibility.OnlyInRuntime;
			staticItem.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			AddBarItem(previewStatus, staticItem, -1, true);
			AddBarItem(previewStatus, ProgressBarEditItem.CreateInstance(150, 12, BarItemVisibility.Never), -1, false);
			BarItem barItem = PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.StopPageBuilding);
			barItem.Visibility = BarItemVisibility.Never;
			AddBarItem(previewStatus, barItem, -1, false);
			AddBarItem(previewStatus, PrintPreviewBarItemFactory.CreateVerticalSpaceButton(BarItemLinkAlignment.Left), -1, false);
			PrintPreviewStaticItem zoomFactorItem = PrintPreviewStaticItemFactory.CreateStaticItem(StatusPanelID.ZoomFactor);
			zoomFactorItem.TextAlignment = StringAlignment.Far;
			AddBarItem(previewStatus, zoomFactorItem, -1, true);
			AddBarItem(manager.StatusBar, ZoomTrackBarEditItem.CreateInstance(140), -1, false);
		}
		void CreatePreviewBarItems() {
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.DocumentMap), 19, false);
			new ParametersPrintBarManagerUpdater(PrintBarManager, this).ConfigInternal();
			new ThumbnailsPrintBarManagerUpdater(PrintBarManager, this).ConfigInternal();
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.Find), 20, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.Customize), 14, true);
#if DEBUGTEST
			if(CanCreateSaveOpenItems)
#endif
				new SaveOpenPrintBarManagerUpdater(PrintBarManager, this).ConfigInternal();
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.Print), 0, true);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.PrintDirect), 1, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.PageSetup), 2, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.EditPageHF), 15, false);
#if DEBUGTEST
			if(CanCreateScaleItem)
#endif
				new ScalePrintBarManagerUpdater(PrintBarManager, this).ConfigInternal();
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.HandTool), 16, true);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.Magnifier), 3, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.ZoomOut), 5, true);
			CreateZoomItem();
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.ZoomIn), 4, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.ShowFirstPage), 7, true);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.ShowPrevPage), 8, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.ShowNextPage), 9, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.ShowLastPage), 10, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.MultiplePages), 11, true);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.FillBackground), 12, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.Watermark), 21, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.ExportFile), 18, true);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.SendFile), 17, false);
			AddBarItem(previewBar, PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.ClosePreview), 13, true);
		}
		void CreateMenuItems() {
			BarSubItem miFile = AddBarSubItem(menuBar, new PrintPreviewSubItem(PrintingSystemCommand.File), PreviewLocalizer.GetString(PreviewStringId.MenuItem_File), "miFile", string.Empty, -1, false);
			BarSubItem miView = AddBarSubItem(menuBar, new PrintPreviewSubItem(PrintingSystemCommand.View), PreviewLocalizer.GetString(PreviewStringId.MenuItem_View), "miView", string.Empty, -1, false);
			BarSubItem miBackground = AddBarSubItem(menuBar, new PrintPreviewSubItem(PrintingSystemCommand.Background), PreviewLocalizer.GetString(PreviewStringId.MenuItem_Background), "miBackground", string.Empty, -1, false);
			AddLink(miFile, PrintingSystemCommand.PageSetup, false);
			AddLink(miFile, PrintingSystemCommand.Print, false);
			AddLink(miFile, PrintingSystemCommand.PrintDirect, false);
			AddLink(miFile, PrintingSystemCommand.ExportFile, true);
			AddLink(miFile, PrintingSystemCommand.SendFile, false);
			AddLink(miFile, PrintingSystemCommand.ClosePreview, true);
			BarSubItem miPageLayout = AddBarSubItem(miView, new PrintPreviewSubItem(PrintingSystemCommand.PageLayout), PreviewLocalizer.GetString(PreviewStringId.MenuItem_PageLayout), "miPageLayout", string.Empty, -1, true);
			PrintPreviewBarItem pageLayoutFacing = new PrintPreviewBarItem(PrintingSystemCommand.PageLayoutFacing);
			AddBarItem(miPageLayout, pageLayoutFacing, PreviewLocalizer.GetString(PreviewStringId.MenuItem_ViewFacing), "miPageLayoutFacing", string.Empty, -1, false);
			pageLayoutFacing.ButtonStyle = BarButtonStyle.Check;
			pageLayoutFacing.GroupIndex = 100;
			PrintPreviewBarItem pageLayoutContinuous = new PrintPreviewBarItem(PrintingSystemCommand.PageLayoutContinuous);
			AddBarItem(miPageLayout, pageLayoutContinuous, PreviewLocalizer.GetString(PreviewStringId.MenuItem_ViewContinuous), "miPageLayoutContinuous", string.Empty, -1, false);
			pageLayoutContinuous.ButtonStyle = BarButtonStyle.Check;
			pageLayoutContinuous.GroupIndex = 100;
			AddBarItem(miView, new BarToolbarsListItem(), "Bars", "miToolbars", string.Empty, -1, true);
			AddLink(miBackground, PrintingSystemCommand.FillBackground, false);
			AddLink(miBackground, PrintingSystemCommand.Watermark, false);
		}
		void AddLink(BarLinksHolder holder, PrintingSystemCommand command, bool beginGroup) {
			BarItem item = PrintBarManager.GetBarItemByCommand(command);
			if(item != null)
				AddLink(holder, item, beginGroup);
		}
		void CreateZoomItem() {
			PrintPreviewRepositoryItemComboBox comboBox = new PrintPreviewRepositoryItemComboBox();
			comboBox.BeginInit();
			comboBox.AutoHeight = true;
			comboBox.AutoComplete = false;
			comboBox.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
			comboBox.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
			comboBox.EndInit();
			ZoomBarEditItem zoomItem = new ZoomBarEditItem();
			AddBarItem(previewBar, zoomItem, PreviewLocalizer.GetString(PreviewStringId.TB_TTip_Zoom), "bbiZoom", PreviewLocalizer.GetString(PreviewStringId.TB_TTip_Zoom), -1, false);
			AssignTextEditToZoomBarEditItem(PrintBarManager, zoomItem, comboBox);
			AddComponentToContainer(comboBox);
		}
		void CreateDropDownMenusItems() {
			PrintPreviewBarCheckItem[] dropDownMenusItems = new PrintPreviewBarCheckItem[] {
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_PdfDocument), PrintingSystemCommand.ExportPdf),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_HtmDocument), PrintingSystemCommand.ExportHtm),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_MhtDocument), PrintingSystemCommand.ExportMht),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_RtfDocument), PrintingSystemCommand.ExportRtf),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_XlsDocument), PrintingSystemCommand.ExportXls),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_XlsxDocument), PrintingSystemCommand.ExportXlsx),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_CsvDocument), PrintingSystemCommand.ExportCsv),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_TxtDocument), PrintingSystemCommand.ExportTxt),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_GraphicDocument), PrintingSystemCommand.ExportGraphic),
												 									 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_PdfDocument), PrintingSystemCommand.SendPdf),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_MhtDocument), PrintingSystemCommand.SendMht),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_RtfDocument), PrintingSystemCommand.SendRtf),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_XlsDocument), PrintingSystemCommand.SendXls),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_XlsxDocument), PrintingSystemCommand.SendXlsx),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_CsvDocument), PrintingSystemCommand.SendCsv),
	   																				 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_TxtDocument), PrintingSystemCommand.SendTxt),
																					 new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_GraphicDocument), PrintingSystemCommand.SendGraphic)};
			foreach(BarItem item in dropDownMenusItems)
				AddBarItem(null, item, item.Caption, "", item.Caption, -1, false);
		}
	}
	public class PrintPreviewStaticItemFactory {
		#region static
		static readonly Dictionary<StatusPanelID, PrintPreviewStaticItemFactory> factories = new Dictionary<StatusPanelID, PrintPreviewStaticItemFactory>();
		static PrintPreviewStaticItemFactory() {
			factories[StatusPanelID.ZoomFactor] = new PrintPreviewStaticItemFactory(StatusPanelID.ZoomFactor, DevExpress.XtraBars.BarStaticItemSize.Content, 0, BarItemLinkAlignment.Right, null);
			factories[StatusPanelID.PageOfPages] = new PrintPreviewStaticItemFactory(StatusPanelID.PageOfPages, DevExpress.XtraBars.BarStaticItemSize.Content, 0, BarItemLinkAlignment.Default, null);
			factories[StatusPanelID.PageOfPages].caption = PreviewLocalizer.GetString(PreviewStringId.SB_PageNone);
			factories[StatusPanelID.PageOfPages].indents = new Pair<int, int>(1, 1);
			factories[StatusPanelID.ZoomFactorText] = new PrintPreviewStaticItemFactory(StatusPanelID.ZoomFactorText, DevExpress.XtraBars.BarStaticItemSize.None, 0, BarItemLinkAlignment.Right, null);
			factories[StatusPanelID.ZoomFactorText].caption = PreviewItemsLogicBase.ZoomFactorToString(1);
		}
		static string GetString(PreviewStringId? id) {
			return id.HasValue ? PreviewLocalizer.GetString(id.Value) : string.Empty;
		}
		public static PrintPreviewStaticItem CreateStaticItem(StatusPanelID statusPanelID) {
			return factories[statusPanelID].CreateItem();
		}
		public static string GetCaption(PrintPreviewStaticItem item) {
			PreviewStringId? id = factories[item.StatusPanelID].captionStringID;
			return GetString(id);
		}
		#endregion
		StatusPanelID statusPanelID;
		PreviewStringId? captionStringID;
		DevExpress.XtraBars.BarStaticItemSize autoSize;
		BarItemLinkAlignment alignment;
		string caption = string.Empty;
		int width;
		Pair<int, int> indents = new Pair<int, int>(0, 0);
		public PrintPreviewStaticItemFactory(StatusPanelID statusPanelID, DevExpress.XtraBars.BarStaticItemSize autoSize, int width, BarItemLinkAlignment alignment, PreviewStringId? captionStringID) {
			this.statusPanelID = statusPanelID;
			this.captionStringID = captionStringID;
			this.autoSize = autoSize;
			this.width = width;
			this.alignment = alignment;
		}
		public PrintPreviewStaticItem CreateItem() {
			PrintPreviewStaticItem barItem = new PrintPreviewStaticItem(statusPanelID);
			barItem.Caption = !string.IsNullOrEmpty(caption) ? caption : 
				GetString(captionStringID);
			barItem.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			barItem.AutoSize = autoSize;
			barItem.Width = width;
			barItem.LeftIndent = indents.First;
			barItem.RightIndent = indents.Second;
			barItem.Alignment = alignment;
			return barItem;
		}
	}
	public abstract class BarItemFactory {
		PrintingSystemCommand command;
		public PrintingSystemCommand Command { get { return command; } }
		protected BarItemFactory(PrintingSystemCommand command) {
			this.command = command;
		}
		public abstract BarItem CreateBarItem();
	}
	public class PrintPreviewBarItemFactory : BarItemFactory {
		public static BarButtonItem CreateVerticalSpaceButton(BarItemLinkAlignment aligment) {
			BarButtonItem barItem = new BarButtonItem();
			barItem.Alignment = aligment;
			barItem.Enabled = false;
			barItem.Visibility = BarItemVisibility.OnlyInRuntime;
			return barItem;
		}
		PreviewStringId captionID;
		PreviewStringId hintID; 
		BarButtonStyle style;
		public BarButtonStyle Style { get { return style; } }
		public PrintPreviewBarItemFactory(PrintingSystemCommand command, PreviewStringId captionID, PreviewStringId hintID, BarButtonStyle style) : base(command) {
			this.captionID = captionID;
			this.hintID = hintID;
			this.style = style;
		}
		public override BarItem CreateBarItem() {
			PrintPreviewBarItem item = new PrintPreviewBarItem(PreviewLocalizer.GetString(captionID), Command);
			item.Hint = PreviewLocalizer.GetString(hintID);
			item.ButtonStyle = style;
			return item;
		}
	}
	public static class PreviewBarItemsCreator {
		static Dictionary<PrintingSystemCommand, BarItemFactory> factories = new Dictionary<PrintingSystemCommand,BarItemFactory>();
		static Dictionary<PrintingSystemCommand, BarButtonStyle> styles = new Dictionary<PrintingSystemCommand, BarButtonStyle>();
		static PreviewBarItemsCreator() {
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.DocumentMap, PreviewStringId.TB_TTip_Map, PreviewStringId.TB_TTip_Map, BarButtonStyle.Check));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Thumbnails, PreviewStringId.TB_TTip_Thumbnails, PreviewStringId.TB_TTip_Thumbnails, BarButtonStyle.Check));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Parameters, PreviewStringId.TB_TTip_Parameters, PreviewStringId.TB_TTip_Parameters, BarButtonStyle.Check));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Find, PreviewStringId.TB_TTip_Search, PreviewStringId.TB_TTip_Search, BarButtonStyle.Check));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.HandTool, PreviewStringId.TB_TTip_HandTool, PreviewStringId.TB_TTip_HandTool, BarButtonStyle.Check));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Customize, PreviewStringId.TB_TTip_Customize, PreviewStringId.TB_TTip_Customize, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Print, PreviewStringId.MenuItem_Print, PreviewStringId.TB_TTip_Print, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.PrintDirect, PreviewStringId.MenuItem_PrintDirect, PreviewStringId.TB_TTip_PrintDirect, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.PageSetup, PreviewStringId.MenuItem_PageSetup, PreviewStringId.TB_TTip_PageSetup, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.EditPageHF, PreviewStringId.TB_TTip_EditPageHF, PreviewStringId.TB_TTip_EditPageHF, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Magnifier, PreviewStringId.TB_TTip_Magnifier, PreviewStringId.TB_TTip_Magnifier, BarButtonStyle.Check));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.ZoomOut, PreviewStringId.TB_TTip_ZoomOut, PreviewStringId.TB_TTip_ZoomOut, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.ZoomIn, PreviewStringId.TB_TTip_ZoomIn, PreviewStringId.TB_TTip_ZoomIn, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.ShowFirstPage, PreviewStringId.TB_TTip_FirstPage, PreviewStringId.TB_TTip_FirstPage, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.ShowPrevPage, PreviewStringId.TB_TTip_PreviousPage, PreviewStringId.TB_TTip_PreviousPage, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.ShowNextPage, PreviewStringId.TB_TTip_NextPage, PreviewStringId.TB_TTip_NextPage, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.ShowLastPage, PreviewStringId.TB_TTip_LastPage, PreviewStringId.TB_TTip_LastPage, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.MultiplePages, PreviewStringId.TB_TTip_MultiplePages, PreviewStringId.TB_TTip_MultiplePages, BarButtonStyle.DropDown));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.FillBackground, PreviewStringId.MenuItem_BackgrColor, PreviewStringId.TB_TTip_Backgr, BarButtonStyle.DropDown));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Watermark, PreviewStringId.MenuItem_Watermark, PreviewStringId.TB_TTip_Watermark, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.ExportFile, PreviewStringId.TB_TTip_Export, PreviewStringId.TB_TTip_Export, BarButtonStyle.DropDown));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.SendFile, PreviewStringId.TB_TTip_Send, PreviewStringId.TB_TTip_Send, BarButtonStyle.DropDown));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.ClosePreview, PreviewStringId.MenuItem_Exit, PreviewStringId.TB_TTip_Close, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Scale, PreviewStringId.TB_TTip_Scale, PreviewStringId.TB_TTip_Scale, BarButtonStyle.DropDown));
			AddStyle(PrintingSystemCommand.ExportPdf, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.ExportHtm, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.ExportMht, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.ExportRtf, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.ExportXls, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.ExportXlsx, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.ExportCsv, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.ExportTxt, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.ExportGraphic, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.SendPdf, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.SendMht, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.SendRtf, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.SendXls, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.SendXlsx, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.SendCsv, BarButtonStyle.Default); 
			AddStyle(PrintingSystemCommand.SendTxt, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.SendGraphic, BarButtonStyle.Default);
			AddStyle(PrintingSystemCommand.Pointer, BarButtonStyle.Check);
			AddStyle(PrintingSystemCommand.PageOrientation, BarButtonStyle.DropDown);
			AddStyle(PrintingSystemCommand.PaperSize, BarButtonStyle.DropDown);
			AddStyle(PrintingSystemCommand.PageMargins, BarButtonStyle.DropDown);
			AddStyle(PrintingSystemCommand.Zoom, BarButtonStyle.DropDown);
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Open, PreviewStringId.RibbonPreview_Open_Caption, PreviewStringId.TB_TTip_Open, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.Save, PreviewStringId.RibbonPreview_Save_Caption, PreviewStringId.TB_TTip_Save, BarButtonStyle.Default));
			AddFactory(new PrintPreviewBarItemFactory(PrintingSystemCommand.StopPageBuilding, PreviewStringId.SB_TTip_Stop, PreviewStringId.SB_TTip_Stop, BarButtonStyle.Default));
		}
		public static BarItem CreateBarItem(PrintingSystemCommand command) {
			return factories[command].CreateBarItem();
		}
		public static BarButtonStyle GetStyle(PrintingSystemCommand command) {
			return styles[command];
		}
		static void AddFactory(PrintPreviewBarItemFactory factory) {
			AddStyle(factory.Command, factory.Style);
			factories[factory.Command] = factory;
		}
		static void AddStyle(PrintingSystemCommand command, BarButtonStyle style) {
			styles[command] = style;
		}
	}
}
