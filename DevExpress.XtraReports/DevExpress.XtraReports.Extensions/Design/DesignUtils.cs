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
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design
{
	public static class ExceptionHelper {
		public static bool IsCriticalException(Exception ex) {
			return ex is NullReferenceException || ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException  || ex is IndexOutOfRangeException || ex is AccessViolationException;
		}
	}
	public static class DesignPropertyNames {
		public const string ShowExportWarnings = "ShowExportWarnings";
		public const string ShowPrintingWarnings = "ShowPrintingWarnings";
		public const string DrawGrid = "DrawGrid";
		public const string DrawWatermark = "DrawWatermark";
		public const string GridSize = "GridSize";
		public const string SnapToGrid = "SnapToGrid";
		public const string PreviewRowCount = "PreviewRowCount";
		public const string UserDesignerLocked = "UserDesignerLocked";
		public const string Expanded = "Expanded";
	}
	public static class XRCursors {
		static Cursor rightArrow = ResLoader.LoadCursor("Images.Cursors.RightArrow.cur", typeof(LocalResFinder));
		static Cursor leftArrow = ResLoader.LoadCursor("Images.Cursors.LeftArrow.cur", typeof(LocalResFinder));
		static Cursor upArrow = ResLoader.LoadCursor("Images.Cursors.UpArrow.cur", typeof(LocalResFinder));
		static Cursor downArrow = ResLoader.LoadCursor("Images.Cursors.DownArrow.cur", typeof(LocalResFinder));
		static Cursor canAssignBindingCursor = ResLoader.LoadColoredCursor(Screen.PrimaryScreen.BitsPerPixel == 32 || Screen.PrimaryScreen.BitsPerPixel == 24 ? 
			"Images.Cursors.CanAssignBindingTC.cur" : "Images.Cursors.CanAssignBinding.cur", typeof(LocalResFinder));
		static Cursor canAssignFormattingComponentCursor = ResLoader.LoadColoredCursor("Images.Cursors.CanAssignFormattingComponent.cur", typeof(LocalResFinder));
		static public Cursor RightArrow { get { return rightArrow; }
		}
		static public Cursor LeftArrow { get { return leftArrow; }
		}
		static public Cursor UpArrow { get { return upArrow; }
		}
		static public Cursor DownArrow { get { return downArrow; }
		}
		static public Cursor CanAssignBindingCursor { get { return canAssignBindingCursor; }
		}
		static public Cursor CanAssignFormattingComponentCursor { get { return canAssignFormattingComponentCursor; }
		}
	}
	public static class XRBitmaps {
		static ImageCollection bandsReportsIcons = ImageHelper.CreateImageCollectionFromResources(LocalResFinder.GetFullName("Images.BandsReportsIcons.png"), LocalResFinder.Assembly, new Size(16, 16));
		static ImageCollection reportExplorerIcons = ImageHelper.CreateImageCollectionFromResources(LocalResFinder.GetFullName("Images.ReportExplorerIcons.png"), LocalResFinder.Assembly, new Size(16, 16));
		static ImageCollection dockPanelsIcons = ImageHelper.CreateImageCollectionFromResources(LocalResFinder.GetFullName("UserDesigner.DockPanelsIcons.png"), LocalResFinder.Assembly, new Size(16, 16));
		static ImageCollection mainToolBarIcons = GetMainToolBarIcons();
		static ImageCollection layoutToolBarIcons = GetLayoutToolBarIcons();
		static ImageCollection mdiIcons = ImageHelper.CreateImageCollectionFromResources(LocalResFinder.GetFullName("UserDesigner.Mdi.png"), LocalResFinder.Assembly, new Size(16, 16), Color.Magenta);
		static Bitmap bmpNewReportWizard = ResLoader.LoadBitmap("UserDesigner.NewReportWizard_16x16.png", typeof(LocalResFinder), Color.Empty);
		static Bitmap bmpZoomIn = ResLoader.LoadBitmap("UserDesigner.ZoomIn_16x16.png", typeof(LocalResFinder), Color.Magenta);
		static Bitmap bmpZoomOut = ResLoader.LoadBitmap("UserDesigner.ZoomOut_16x16.png", typeof(LocalResFinder), Color.Magenta);
		static Bitmap bmpZoomMask = ResLoader.LoadBitmap("Images.ZoomMask.bmp", typeof(LocalResFinder), Color.Empty);
		static Bitmap validate = ResLoader.LoadBitmap("Images.Validate_16x16.png", typeof(LocalResFinder), Color.Empty);
		static Bitmap bandCaptionCollapsed = ResLoader.LoadBitmap("Images.BandCaptionCollapsed.png", typeof(LocalResFinder), Color.Transparent);
		static Bitmap bandCaptionExpanded = ResLoader.LoadBitmap("Images.BandCaptionExpanded.png", typeof(LocalResFinder), Color.Transparent);
		static Bitmap sqlDataSource = ResLoader.LoadBitmap("Bitmaps256.SqlDataSource.bmp", typeof(DevExpress.DataAccess.ResFinder), Color.Magenta);
		static public Bitmap BandCaptionCollapsed {
			get { return bandCaptionCollapsed; }
		}
		static public Bitmap BandCaptionExpanded {
			get { return bandCaptionExpanded; }
		}
		static public Bitmap SqlDataSource {
			get { return sqlDataSource; }
		}
		static public Bitmap Validate { get { return validate; } }
		static public Bitmap SubBand			 { get { return (Bitmap)bandsReportsIcons.Images[0]; } }
		static public Bitmap Report			  { get { return (Bitmap)bandsReportsIcons.Images[1]; } }
		static public Bitmap DetailReport		{ get { return (Bitmap)bandsReportsIcons.Images[2]; } }
		static public Bitmap WinControlContainer { get { return (Bitmap)bandsReportsIcons.Images[3]; } }
		static public Bitmap TopMarginBand	   { get { return (Bitmap)bandsReportsIcons.Images[4]; } }
		static public Bitmap BottomMarginBand	{ get { return (Bitmap)bandsReportsIcons.Images[5]; } }
		static public Bitmap DetailBand		  { get { return (Bitmap)bandsReportsIcons.Images[6]; } }
		static public Bitmap GroupHeaderBand	 { get { return (Bitmap)bandsReportsIcons.Images[7]; } }
		static public Bitmap GroupFooterBand	 { get { return (Bitmap)bandsReportsIcons.Images[8]; } }
		static public Bitmap PageHeaderBand	  { get { return (Bitmap)bandsReportsIcons.Images[9]; } }
		static public Bitmap ReportFooterBand	{ get { return (Bitmap)bandsReportsIcons.Images[10]; } }
		static public Bitmap ReportHeaderBand	{ get { return (Bitmap)bandsReportsIcons.Images[11]; } }
		static public Bitmap PageFooterBand	  { get { return (Bitmap)bandsReportsIcons.Images[12]; } }
		static public Bitmap None				{ get { return (Bitmap)bandsReportsIcons.Images[13]; } }
		static public Bitmap XRControl { get { return (Bitmap)reportExplorerIcons.Images[0]; } }
		static public Bitmap XRLabel { get { return (Bitmap)reportExplorerIcons.Images[1]; } }
		static public Bitmap XRCheckBox { get { return (Bitmap)reportExplorerIcons.Images[2]; } }
		static public Bitmap XRRichText { get { return (Bitmap)reportExplorerIcons.Images[3]; } }
		static public Bitmap XRPictureBox { get { return (Bitmap)reportExplorerIcons.Images[4]; } }
		static public Bitmap XRPanel { get { return (Bitmap)reportExplorerIcons.Images[5]; } }
		static public Bitmap XRTable { get { return (Bitmap)reportExplorerIcons.Images[6]; } }
		static public Bitmap XRTableCell { get { return (Bitmap)reportExplorerIcons.Images[7]; } }
		static public Bitmap XRTableRow { get { return (Bitmap)reportExplorerIcons.Images[8]; } }
		static public Bitmap XRLine { get { return (Bitmap)reportExplorerIcons.Images[9]; } }
		static public Bitmap XRShape { get { return (Bitmap)reportExplorerIcons.Images[10]; } }
		static public Bitmap XRBarCode { get { return (Bitmap)reportExplorerIcons.Images[11]; } }
		static public Bitmap XRZipCode { get { return (Bitmap)reportExplorerIcons.Images[12]; } }
		static public Bitmap XRChart { get { return (Bitmap)reportExplorerIcons.Images[13]; } }
		static public Bitmap XRPivotGrid { get { return (Bitmap)reportExplorerIcons.Images[14]; } }
		static public Bitmap XRPageInfo { get { return (Bitmap)reportExplorerIcons.Images[15]; } }
		static public Bitmap XRPageBreak { get { return (Bitmap)reportExplorerIcons.Images[16]; } }
		static public Bitmap XRCrossBandLine { get { return (Bitmap)reportExplorerIcons.Images[17]; } }
		static public Bitmap XRCrossBandBox { get { return (Bitmap)reportExplorerIcons.Images[18]; } }
		static public Bitmap XRSubReport { get { return (Bitmap)reportExplorerIcons.Images[19]; } }
		static public Bitmap XRSparkline { get { return (Bitmap)reportExplorerIcons.Images[20]; } }
		static public Bitmap XRGauge { get { return (Bitmap)reportExplorerIcons.Images[21]; } }
		static public Bitmap ControlStyle { get { return (Bitmap)reportExplorerIcons.Images[22]; } }
		static public Bitmap ExternalControlStyle { get { return (Bitmap)reportExplorerIcons.Images[23]; } }
		static public Bitmap FormattingRule { get { return (Bitmap)reportExplorerIcons.Images[24]; } }
		static public Bitmap FieldListDockPanel { get { return (Bitmap)dockPanelsIcons.Images[0]; } }
		static public Bitmap GroupSortDockPanel { get { return (Bitmap)dockPanelsIcons.Images[1]; } }
		static public Bitmap PropertyGridDockPanel { get { return (Bitmap)dockPanelsIcons.Images[2]; } }
		static public Bitmap ReportExplorerDockPanel { get { return (Bitmap)dockPanelsIcons.Images[3]; } }
		static public Bitmap ToolBoxDockPanel { get { return (Bitmap)dockPanelsIcons.Images[4]; } }
		static public Bitmap ErrorListDockPanel { get { return (Bitmap)dockPanelsIcons.Images[5]; } }
		static public Bitmap ZoomIn { get { return bmpZoomIn; } }
		static public Bitmap ZoomOut { get { return bmpZoomOut; } }
		static public Bitmap ZoomMask { get { return bmpZoomMask; }	}
		static public Bitmap NewReportWizard { get { return bmpNewReportWizard; } }
		static public Bitmap New { get { return (Bitmap)mainToolBarIcons.Images[0]; } }
		static public Bitmap Open { get { return (Bitmap)mainToolBarIcons.Images[1]; } }
		static public Bitmap Save { get { return (Bitmap)mainToolBarIcons.Images[2]; } }
		static public Bitmap Cut { get { return (Bitmap)mainToolBarIcons.Images[3]; } }
		static public Bitmap Copy { get { return (Bitmap)mainToolBarIcons.Images[4]; } }
		static public Bitmap Paste { get { return (Bitmap)mainToolBarIcons.Images[5]; } }
		static public Bitmap Undo { get { return (Bitmap)mainToolBarIcons.Images[6]; } }
		static public Bitmap Redo { get { return (Bitmap)mainToolBarIcons.Images[7]; } }
		static public Bitmap AlignToGrid { get { return (Bitmap)layoutToolBarIcons.Images[0]; } }
		static public Bitmap BringToFront { get { return (Bitmap)layoutToolBarIcons.Images[21]; } }
		static public Bitmap SendToBack { get { return (Bitmap)layoutToolBarIcons.Images[22]; } }
		static public ImageCollection GetMdiToolBarIcons() {
			return mdiIcons;
		}
		static public ImageCollection GetMainToolBarIcons() {
			ImageCollection mainToolBarIcons = ImageCollectionHelper.CreateVoidImageCollection();
			ImageCollectionHelper.AddImagesToCollectionFromResources(mainToolBarIcons, LocalResFinder.GetFullName("UserDesigner.New_16x16.png"), LocalResFinder.Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(mainToolBarIcons, "DevExpress.XtraPrinting.Images.Open_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(mainToolBarIcons, "DevExpress.XtraPrinting.Images.Save_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(mainToolBarIcons, LocalResFinder.GetFullName("UserDesigner.Cut_16x16.png"), LocalResFinder.Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(mainToolBarIcons, LocalResFinder.GetFullName("UserDesigner.Copy_16x16.png"), LocalResFinder.Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(mainToolBarIcons, LocalResFinder.GetFullName("UserDesigner.Paste_16x16.png"), LocalResFinder.Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(mainToolBarIcons, LocalResFinder.GetFullName("UserDesigner.Undo_16x16.png"), LocalResFinder.Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(mainToolBarIcons, LocalResFinder.GetFullName("UserDesigner.Redo_16x16.png"), LocalResFinder.Assembly);
			return mainToolBarIcons;
		}
		static public ImageCollection GetFormattingToolBarIcons() {
			ImageCollection formattingToolBarIcons = ImageCollectionHelper.CreateVoidImageCollection();
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.Bold_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.Italic_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.Underline_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.FontColor_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.Highlight_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.AlignLeft_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.AlignCenter_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.AlignRight_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			ImageCollectionHelper.AddImagesToCollectionFromResources(formattingToolBarIcons, "DevExpress.XtraPrinting.Images.AlignJustify_16x16.png", typeof(XtraPrinting.PrintingSystem).Assembly);
			return formattingToolBarIcons;
		}
		static public ImageCollection GetLayoutToolBarIcons() {
			ImageCollection layoutToolBarIcons = ImageHelper.CreateImageCollectionFromResources(LocalResFinder.GetFullName("UserDesigner.LayoutToolBar.png"), LocalResFinder.Assembly, new Size(16, 16));
			return layoutToolBarIcons;
		}
	}
	public class ViewInfo
	{
		bool isReady;
		public bool IsReady { 
			get { return isReady; }
			set { 
				if(value != isReady) {
					if(value) Calculate();
					else Reset();
				}
			}
		}
		protected virtual void Reset() {
			isReady = false;
		}
		protected virtual void Calculate() {
			if(isReady == false) {
				isReady = true;
				OnCalculate();
			}
		}
		protected virtual void OnCalculate() {
		}
	}
	public static class WindowStyle {
		public const int
			WS_CLIPSIBLINGS = 0x04000000,
			WS_CLIPCHILDREN = 0x02000000;
	}
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	public struct POINT
	{ 
		public int X, Y;
	}
	public static class DesignMethods {
		public static bool IsDesignerInTransaction(IDesignerHost host) {
			bool result = host.InTransaction;
			if (result) {
				Type type = host.GetType();
				PropertyInfo pi = type.GetProperty("IsClosingTransaction", BindingFlags.NonPublic | BindingFlags.Instance);
				if (pi != null)
					result = !(bool)pi.GetValue(host, null);
			}
			return result;
		}
	}
	public static class ParentSearchHelper {
		public static XRControl FindParent(XtraReport rootReport, Type controlType, IServiceProvider provider) {
			return CapturePaintService.GetDragBounds(provider).IsEmpty ? GetParentByPrimarySelection(rootReport, controlType, provider) : GetParentByMousePosition(rootReport, controlType, provider);
		}
		static XRControl GetParentByMousePosition(XtraReport rootReport, Type controlType, IServiceProvider provider) {
			CapturePaintService capturePaintSvc = (CapturePaintService)provider.GetService(typeof(CapturePaintService));
			System.Diagnostics.Debug.Assert(capturePaintSvc != null);
			IBandViewInfoService bandViewSvc = (IBandViewInfoService)provider.GetService(typeof(IBandViewInfoService));
			System.Diagnostics.Debug.Assert(bandViewSvc != null);
			XRControl controlParent = bandViewSvc.GetControlByScreenPoint(capturePaintSvc.StartPos);
			return (controlParent == null) ? GetParentByPrimarySelection(rootReport, controlType, provider) :
				ValidateControlParent(rootReport, controlType, controlParent, provider);
		}
		static XRControl GetParentByPrimarySelection(XtraReport rootReport, Type controlType, IServiceProvider provider) {
			ISelectionService selectionService = provider.GetService(typeof(ISelectionService)) as ISelectionService;
			return ValidateControlParent(rootReport, controlType, selectionService.PrimarySelection as XRControl, provider);
		}
		static XRControl ValidateControlParent(XtraReport rootReport, Type controlType, XRControl controlParent, IServiceProvider provider) {
			XtraReportBase report = controlParent != null ? controlParent.Report : rootReport;
			while(controlParent != null) {
				IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				XRControlDesignerBase designer = host.GetDesigner(controlParent) as XRControlDesignerBase;
				if(designer == null ? controlParent.CanAddControl(controlType, null) : designer.CanAddControl(controlType))
					return controlParent;
				controlParent = controlParent.Parent;
			}
			Band band = report.Bands[BandKind.Detail];
			return (band != null) ? band : report.Bands[0];
		}
	}
}
