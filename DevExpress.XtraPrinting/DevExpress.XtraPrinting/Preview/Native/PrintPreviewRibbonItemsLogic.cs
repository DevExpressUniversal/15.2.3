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
using DevExpress.XtraBars.Ribbon;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Preview.Native.Galleries;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Control;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
using System.Collections;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraPrinting.Preview.Native {
	public class RibbonPreviewItemsLogic : RibbonPreviewItemsLogicBase {
		public RibbonPreviewItemsLogic(RibbonBarManager manager, object contextSpecifier)
			: base(manager, contextSpecifier) {
		}
		protected override void InitPopupControlContainers() {
			base.InitPopupControlContainers();
			PrintPreviewGalleryDropDownBase gallery = new PrintPreviewPageOrientationGalleryDropDown(this, components);
			gallery.Ribbon = RibbonBarManager.Ribbon;
			gallery.Gallery.ShowScrollBar = XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			AssignDropDownControl(PrintingSystemCommand.PageOrientation, gallery);
			gallery = new PrintPreviewPaperSizeGalleryDropDown(this, components);
			gallery.Ribbon = RibbonBarManager.Ribbon;
			gallery.Gallery.ShowScrollBar = XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			AssignDropDownControl(PrintingSystemCommand.PaperSize, gallery);
			gallery = new PrintPreviewPageMarginsGalleryDropDown(this, components);
			gallery.Ribbon = RibbonBarManager.Ribbon;
			gallery.Gallery.ShowScrollBar = XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			AssignDropDownControl(PrintingSystemCommand.PageMargins, gallery);
		}
	}
	public class RibbonPreviewItemsLogicBase : PreviewItemsLogicBase {
		#region static
		static PrintPreviewBarItem CreateZoomToItem(float zoomFactor) {
			return new RuntimePrintPreviewBarItem(PreviewItemsLogicBase.ZoomFactorToString(zoomFactor), PrintingSystemCommand.Zoom, zoomFactor);
		}
		static PrintPreviewBarItem CreateZoomCommandItem(PrintingSystemCommand command, PreviewStringId stringID) {
			return new RuntimePrintPreviewBarItem(PreviewLocalizer.GetString(stringID), command, null);
		}
		#endregion
		object contextSpecifier;
		ZoomBarEditItem zoomBarEditItem;
		RepositoryItemTextEdit zoomEdit;
		RibbonImageCollection images;
		protected RibbonBarManager RibbonBarManager { get { return (RibbonBarManager)Manager; } }
		public RibbonImageCollection ImageCollection { get { return images; } set { images = value; } }
		protected internal override ZoomBarEditItem ZoomItem { get { return zoomBarEditItem; } }
		public RibbonPreviewItemsLogicBase(RibbonBarManager manager, object contextSpecifier)
			: base(manager) {
			this.contextSpecifier = contextSpecifier;
		}
		public override void UpdateCommands() {
			base.UpdateCommands();
			UpdateButtonImages();
		}
		 void UpdateButtonImages() {
			foreach(BarItem barItem in Manager.Items) {
				PrintPreviewBarItem printPreviewBarItem = barItem as PrintPreviewBarItem;
				if(printPreviewBarItem != null) {
					PrintPreviewExportGalleryDropDown dropDown = printPreviewBarItem.DropDownControl as PrintPreviewExportGalleryDropDown;
					if(dropDown != null) {
						dropDown.UpdateButtonImage();
					}
				}
			}
		}
		protected override bool CanHandleEvent(PrintPreviewBarItem barItem) {
			return base.CanHandleEvent(barItem) && ((ISupportContextSpecifier)barItem).HasSameContext(this.contextSpecifier);
		}
		public override BarItem GetBarItemByCommand(PrintingSystemCommand command) {
			BarItem item = GetBarItemByCommand(Manager, command, contextSpecifier);
			return item != null ? item : dummyItem;
		}
		protected internal override bool IsCommandVisible(CommandSetItem commandSetItem) {
			return base.IsCommandVisible(commandSetItem) || commandSetItem.IsLowPriority;
		}
		protected override bool IsCommadSetItemEnabled(CommandSetItem commandSetItem) {
			return base.IsCommadSetItemEnabled(commandSetItem) && base.IsCommandVisible(commandSetItem);
		}
		protected override PopupControl CreateExportMenuPopupControl(PrintingSystemCommand[] commands, PrintingSystemCommand parentCommand) {
			PrintPreviewExportGalleryDropDown gallery = new PrintPreviewExportGalleryDropDown(this, commands, parentCommand, components);
			gallery.Ribbon = RibbonBarManager.Ribbon;
			gallery.Gallery.ShowScrollBar = XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			if(!Manager.IsDesignMode)
				gallery.UpdateButtonImage();
			return gallery;
		}
		protected override void InitPopupControlContainers() {
			base.InitPopupControlContainers();
			AssignDropDownControl(PrintingSystemCommand.Zoom, new PrintPreviewPopupMenu(this, -1, CreateZoomListButtons(), components));
		}
		protected override void HandleToolCommand(PrintingSystemCommand command, bool down) {
			PrintControl.ExecCommand(command, new object[] { true });
		}
		BarItem[] CreateZoomListButtons() {
			zoomEdit = new RepositoryItemTextEdit();
			zoomBarEditItem = new RuntimeZoomBarEditItem();
			zoomBarEditItem.Caption = PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ZoomExact_Caption);
			PrintBarManagerConfigurator.AssignTextEditToZoomBarEditItem(Manager, zoomBarEditItem, zoomEdit);
			BarItem[] items = new BarItem[] {
					CreateZoomCommandItem(PrintingSystemCommand.ZoomToPageWidth, PreviewStringId.MenuItem_ZoomPageWidth),
					CreateZoomCommandItem(PrintingSystemCommand.ZoomToTextWidth, PreviewStringId.MenuItem_ZoomTextWidth),
					CreateZoomCommandItem(PrintingSystemCommand.ZoomToWholePage, PreviewStringId.MenuItem_ZoomWholePage),
					CreateZoomCommandItem(PrintingSystemCommand.ZoomToTwoPages, PreviewStringId.MenuItem_ZoomTwoPages),
					null,
					CreateZoomToItem(5),
					CreateZoomToItem(2),
					CreateZoomToItem(1.5f),
					CreateZoomToItem(1),
					CreateZoomToItem(0.75f),
					CreateZoomToItem(0.5f),
					CreateZoomToItem(0.25f),
					null,
					zoomBarEditItem
				};
			foreach(BarItem item in items) 
				AddBarItemToContainer(item);
			return items;
		}
		void AddBarItemToContainer(BarItem item) {
			if(item != null) {
				if(item is ISupportContextSpecifier)
					((ISupportContextSpecifier)item).ContextSpecifier = this.contextSpecifier;
				Manager.Items.Add(item);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(zoomEdit != null) {
						zoomEdit.Dispose();
						zoomEdit = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
	}
}
