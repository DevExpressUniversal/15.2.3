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
using System.Reflection;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils;
using System.IO;
using System.ComponentModel.Design;
namespace DevExpress.XtraPrinting.Preview.Native {
	public class PrintRibbonControllerConfigurator : RibbonControllerConfiguratorBase {
		#region static
		static readonly Dictionary<string, Image> images = new Dictionary<string, Image>();
		static readonly string[] groupImageNames = new string[] { "PrintDirect", "PageMargins", "Find", "Zoom", "Watermark", "ExportFile", "Document", "ClosePreview" };
		static PrintRibbonControllerConfigurator defaultInstance = new PrintRibbonControllerConfigurator(null, null, null);
		public static Dictionary<string, Image> GetImagesFromAssembly() {
			return RibbonControllerConfiguratorBase.GetImagesFromAssembly(Assembly.GetExecutingAssembly(), RibbonImagesResourcePath, defaultInstance.RibbonImagesNamePrefix);
		}
		public static string GetGroupImageName(PrintPreviewRibbonPageGroupKind kind) {
			return defaultInstance.RibbonImagesNamePrefix + GetGroupImageNameInternal(kind);
		}
		static string GetGroupImageNameInternal(PrintPreviewRibbonPageGroupKind kind) {
			return groupImageNames[(int)kind];
		}
		public static Image GetImageFromResource(string imageName) {
			if(!images.ContainsKey(imageName)) {
				System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RibbonImagesResourcePath + imageName + ".png");
				images[imageName] = stream != null ? Bitmap.FromStream(stream) : null;
			}
			return images[imageName];
		}
		internal static Image GetImageLargeFromResource(string imageName) {
			return GetImageFromResource(GetLargeImageName(imageName));
		}
		internal static string GetCommandCaption(string alias) {
			return defaultInstance.GetLocalizedString(RibbonPreviewPrefix + alias + CaptionSuffix); 
		}
		internal static string GetCommandDescription(string alias) {
			return defaultInstance.GetLocalizedString(RibbonPreviewPrefix + alias + DescriptionSuffix);
		}
		internal static string GetGalleryItemCaption(string galleryItemAlias) {
			return GetGalleryItemElement(galleryItemAlias, CaptionSuffix);
		}
		internal static string GetGalleryItemDescription(string galleryItemAlias) {
			return GetGalleryItemElement(galleryItemAlias, DescriptionSuffix);
		}
		static string GetGalleryItemElement(string galleryItemAlias, string suffix) {
			return defaultInstance.GetLocalizedString(RibbonPreviewGalleryItemPrefix + galleryItemAlias + suffix);
		}
		public static void LocalizeStrings(RibbonControl ribbonControl) {
			defaultInstance.LocalizeStrings<PrintPreviewRibbonPageGroup, PrintPreviewBarItem, RuntimePrintPreviewBarItem, PrintPreviewRibbonPageGroupKind>(ribbonControl); 
		}
		#endregion
		const int ToolButtonsGroupIndex = 1;
		const string RibbonPreviewPrefix = "RibbonPreview_";
		const string RibbonPreviewGalleryItemPrefix = RibbonPreviewPrefix + "GalleryItem_";
		const string RibbonImagesResourcePath = "DevExpress.XtraPrinting.Images.Ribbon.";
		protected RibbonPage previewPage;
		internal RibbonPage PreviewPage { get { return previewPage; } }
		public PrintRibbonControllerConfigurator(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> ribbonImages)
			: base(ribbonControl, ribbonStatusBar, ribbonImages) {
			RibbonPrefix = RibbonPreviewPrefix;
			RibbonImagesNamePrefix = "RibbonPrintPreview_";
			ReferencedNamePrefix = "PreviewStringId.";
		}
		#region groups creation
		protected override void CreatePageGroups() {
			previewPage = CreateRibbonPage(typeof(PrintPreviewRibbonPage));
			ribbonControl.Pages.Add(previewPage);
			((PrintPreviewRibbonPage)previewPage).ContextSpecifier = this.ContextSpecifier;
#if DEBUGTEST
			if(PrintBarManagerConfigurator.CanCreateSaveOpenItems)
#endif
				CreateDocumentGroup();
			CreatePrintGroup();
			CreatePageSetupGroup();
			CreateNavigationGroup();
			CreateZoomGroup();
			CreateBackgroundGroup();
			CreateExportGroup();
			CreateCloseGroup();			
		}
		protected void CreateDocumentGroup() {
			PrintPreviewRibbonPageGroup group = CreatePageGroup(PrintPreviewRibbonPageGroupKind.Document);
			AddLink(group, PrintingSystemCommand.Open);
			AddLink(group, PrintingSystemCommand.Save);
		}
		void CreatePrintGroup() {
			PrintPreviewRibbonPageGroup group = CreatePageGroup(PrintPreviewRibbonPageGroupKind.Print);
			AddLink(group, PrintingSystemCommand.Print);
			AddLink(group, PrintingSystemCommand.PrintDirect);
			AddLink(group, PrintingSystemCommand.Customize);
			AddLink(group, PrintingSystemCommand.Parameters);
		}
		void CreatePageSetupGroup() {
			PrintPreviewRibbonPageGroup group = CreatePageGroup(PrintPreviewRibbonPageGroupKind.PageSetup);
			group.ShowCaptionButton = true;
			group.SuperTip = CreateSuperToolTip(RibbonPageGroupPrefix + group.Kind.ToString());
			AddLink(group, PrintingSystemCommand.EditPageHF);
			AddLink(group, PrintingSystemCommand.Scale);
			AddLink(group, PrintingSystemCommand.PageMargins);
			AddLink(group, PrintingSystemCommand.PageOrientation);
			AddLink(group, PrintingSystemCommand.PaperSize);
		}
		void CreateNavigationGroup() {
			PrintPreviewRibbonPageGroup group = CreatePageGroup(PrintPreviewRibbonPageGroupKind.Navigation);
			AddLink(group, PrintingSystemCommand.Find);
			AddLink(group, PrintingSystemCommand.Thumbnails);
			AddLink(group, PrintingSystemCommand.DocumentMap);
			AddLink(group, PrintingSystemCommand.ShowFirstPage, true);
			AddLink(group, PrintingSystemCommand.ShowPrevPage);
			AddLink(group, PrintingSystemCommand.ShowNextPage);
			AddLink(group, PrintingSystemCommand.ShowLastPage);
		}
		void CreateZoomGroup() {
			PrintPreviewRibbonPageGroup group = CreatePageGroup(PrintPreviewRibbonPageGroupKind.Zoom);
			AddLink(group, PrintingSystemCommand.Pointer);
			AddLink(group, PrintingSystemCommand.HandTool);
			AddLink(group, PrintingSystemCommand.Magnifier);
			AddLink(group, PrintingSystemCommand.MultiplePages);
			AddLink(group, PrintingSystemCommand.ZoomOut);
			AddLink(group, PrintingSystemCommand.Zoom);
			AddLink(group, PrintingSystemCommand.ZoomIn);
		}
		void CreateBackgroundGroup() {
			PrintPreviewRibbonPageGroup group = CreatePageGroup(PrintPreviewRibbonPageGroupKind.Background);
			AddLink(group, PrintingSystemCommand.FillBackground);
			AddLink(group, PrintingSystemCommand.Watermark);
		}
		void CreateExportGroup() {
			PrintPreviewRibbonPageGroup group = CreatePageGroup(PrintPreviewRibbonPageGroupKind.Export);
			AddLink(group, PrintingSystemCommand.ExportFile);
			AddLink(group, PrintingSystemCommand.SendFile);
		}
		void CreateCloseGroup() {
			PrintPreviewRibbonPageGroup group = CreatePageGroup(PrintPreviewRibbonPageGroupKind.Close);
			AddLink(group, PrintingSystemCommand.ClosePreview);
		}
		PrintPreviewRibbonPageGroup CreatePageGroup(PrintPreviewRibbonPageGroupKind kind) {
			PrintPreviewRibbonPageGroup group = CreatePageGroup<PrintPreviewRibbonPageGroup, PrintPreviewRibbonPageGroupKind>(previewPage, kind, GetGroupImageNameInternal(kind));
			group.ContextSpecifier = this.ContextSpecifier;
			return group;
		}
		void AddLink(RibbonPageGroup pageGroup, PrintingSystemCommand command, bool beginGroup) {
			pageGroup.ItemLinks.Add(GetBarItemByCommand(command), beginGroup);
		}
		BarItem GetBarItemByCommand(PrintingSystemCommand command) {
			return PreviewItemsLogicBase.GetBarItemByCommand(ribbonControl.Manager, command, this.ContextSpecifier);
		}
		BarItem GetBarItemByStatusPanelID(StatusPanelID statusPanelID) {
			return PreviewItemsLogicBase.GetBarItemByStatusPanelID(ribbonControl.Manager, statusPanelID);
		}
		protected void AddLink(RibbonPageGroup pageGroup, PrintingSystemCommand command) {
			AddLink(pageGroup, command, false);   
		}
		#endregion
		#region items creation
		protected override void CreateItems() {
			AddBarItem(PrintingSystemCommand.DocumentMap, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Parameters, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Find, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Thumbnails, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Customize, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Print, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.PrintDirect, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.PageSetup, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.EditPageHF, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Scale, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Pointer, RibbonItemStyles.SmallWithoutText, ToolButtonsGroupIndex);
			AddBarItem(PrintingSystemCommand.HandTool, RibbonItemStyles.SmallWithoutText, ToolButtonsGroupIndex);
			AddBarItem(PrintingSystemCommand.Magnifier, RibbonItemStyles.SmallWithoutText, ToolButtonsGroupIndex);
			AddBarItem(PrintingSystemCommand.ZoomOut, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.ZoomIn, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Zoom, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.ShowFirstPage, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.ShowPrevPage, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.ShowNextPage, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.ShowLastPage, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.MultiplePages, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.FillBackground, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.Watermark, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.ExportFile, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.SendFile, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.ClosePreview, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.PageOrientation, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.PaperSize, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.PageMargins, RibbonItemStyles.Default);
			AddBarItem(PrintingSystemCommand.SendPdf);
			AddBarItem(PrintingSystemCommand.SendTxt);
			AddBarItem(PrintingSystemCommand.SendCsv);
			AddBarItem(PrintingSystemCommand.SendMht);
			AddBarItem(PrintingSystemCommand.SendXls);
			AddBarItem(PrintingSystemCommand.SendXlsx);
			AddBarItem(PrintingSystemCommand.SendRtf);
			AddBarItem(PrintingSystemCommand.SendGraphic);
			AddBarItem(PrintingSystemCommand.ExportPdf);
			AddBarItem(PrintingSystemCommand.ExportHtm);
			AddBarItem(PrintingSystemCommand.ExportTxt);
			AddBarItem(PrintingSystemCommand.ExportCsv);
			AddBarItem(PrintingSystemCommand.ExportMht);
			AddBarItem(PrintingSystemCommand.ExportXls);
			AddBarItem(PrintingSystemCommand.ExportXlsx);
			AddBarItem(PrintingSystemCommand.ExportRtf);
			AddBarItem(PrintingSystemCommand.ExportGraphic);
#if DEBUGTEST
			if(PrintBarManagerConfigurator.CanCreateSaveOpenItems)
#endif
			{
				AddBarItem(PrintingSystemCommand.Open);
				AddBarItem(PrintingSystemCommand.Save);
			}
			AddStatusPanelItems();
		}
		protected void AddStatusPanelItems() {
			if(ribbonStatusBar == null)
				return;
			AddStatusPanelItem(StatusPanelID.PageOfPages, false);
			BarStaticItem staticItem = new BarStaticItem();
			staticItem.Visibility = BarItemVisibility.OnlyInRuntime;
			AddStatusPanelItem(staticItem, true);
			AddStatusPanelItem(ProgressBarEditItem.CreateInstance(150, 12, BarItemVisibility.Never), false);
			BarItem barItem = PreviewBarItemsCreator.CreateBarItem(PrintingSystemCommand.StopPageBuilding);
			barItem.Visibility = BarItemVisibility.Never;
			AddStatusPanelItem(barItem, false);
			AddStatusPanelItem(PrintPreviewBarItemFactory.CreateVerticalSpaceButton(BarItemLinkAlignment.Left), false);
			AddStatusPanelItem(StatusPanelID.ZoomFactorText, false);
			ZoomTrackBarEditItem zoomTrackBarEditItem = ZoomTrackBarEditItem.CreateInstance(140);
			zoomTrackBarEditItem.RepositoryItem.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			AddStatusPanelItem(zoomTrackBarEditItem, false);
		}
		void AddStatusPanelItem(StatusPanelID id, bool beginGroup) {
			if(GetBarItemByStatusPanelID(id) == null)
				AddStatusPanelItem(PrintPreviewStaticItemFactory.CreateStaticItem(id), beginGroup);
		}
		void AddStatusPanelItem(BarItem item, bool beginGroup) {
			AddBarItem(item);
			if(ribbonStatusBar != null)
				ribbonStatusBar.ItemLinks.Add(item, beginGroup);
		}
		protected void AddBarItem(PrintingSystemCommand command) {
			AddBarItem(CreateItemByCommand(command), command.ToString(), RibbonItemStyles.Default, -1, string.Empty);
		}
		void AddBarItem(PrintingSystemCommand command, RibbonItemStyles ribbonStyle) {
			AddBarItem(command, ribbonStyle, -1);
		}
		void AddBarItem(PrintingSystemCommand command, RibbonItemStyles ribbonStyle, int groupIndex) {
			AddBarItem(CreateItemByCommand(command), command.ToString(), ribbonStyle, groupIndex, null);
		}
		PrintPreviewBarItem CreateItemByCommand(PrintingSystemCommand command) {
			PrintPreviewBarItem item = new PrintPreviewBarItem(command);
			item.ContextSpecifier = this.ContextSpecifier;
			item.ButtonStyle = PreviewBarItemsCreator.GetStyle(command);
			return item;
		}
		protected override string GetLocalizedString(string str) {
			return PreviewLocalizer.GetString((PreviewStringId)Enum.Parse(typeof(PreviewStringId), str));
		}
		protected override string GetDefaultLocalizedString(string str) {
			return Enum.IsDefined(typeof(PreviewStringId), str) ? PreviewLocalizer.Default.GetLocalizedString((PreviewStringId)Enum.Parse(typeof(PreviewStringId), str)) : null;
		}
		protected override string BarItemCommandToString(BarItem item) {
			return ((PrintPreviewBarItem)item).Command.ToString();
		}
		protected override string GetTextForRibbonPage(RibbonPage page) {
			return page is PrintPreviewRibbonPage ? PageText : null;
		}
		#endregion
	}
}
