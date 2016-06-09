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
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Control;
namespace DevExpress.XtraReports.Design {
	class TabControlRibbonLogicCreator : TabControlLogicCreator {
		XRDesignRibbonController ribbonController;
		public TabControlRibbonLogicCreator(XRDesignRibbonController ribbonController) {
			this.ribbonController = ribbonController;
		}
		public override TabControlLogic CreateInstance(ReportTabControl tabControl, IServiceProvider servProvider) {
			if(ribbonController.RibbonControl != null)
				return new TabControlRibbonLogic(tabControl, servProvider, ribbonController);
			tabControl.ShowPrintPreviewBar = false;
			tabControl.ShowTabButtons = false;
			return null;
		}
	}
	class TabControlRibbonLogic : TabControlLogic {
		string status = string.Empty;
		XRDesignRibbonController designController;
		internal override BarManager BarManager {
			get { return designController.RibbonControl != null ? designController.RibbonControl.Manager : null; }
		}
		public TabControlRibbonLogic(ReportTabControl tabControl, IServiceProvider servProvider, XRDesignRibbonController designController)
			: base(tabControl, servProvider) {
			this.designController = designController;
		}
		void XRDesignPanel_Activated(object sender, EventArgs e) {
			designController.XRDesignPanel.Activated -= new EventHandler(XRDesignPanel_Activated);
		}
		public override void OnPrintControlCreated() {
			UpdateRibbonController(designController.PrintRibbonController, tabControl.PreviewControl);
			if(tabControl.SelectedIndex == TabIndices.Preview)
				InitZoomItem(tabControl.PreviewControl);
		}
		void UpdateRibbonController(PrintRibbonController printRibbonController, PrintControl printControl) {
			if(printRibbonController != null && printControl != null && printRibbonController.PrintControl != printControl)
				printRibbonController.PrintControl = printControl;
		}
		public override void OnBrowserVisible() {
			base.OnBrowserVisible();
			UpdateStatusCore(ReportLocalizer.GetString(ReportStringId.RibbonXRDesign_StatusBar_HtmlProcessing));
			designController.SetBarItemLocked(designController.GetBarItemBy(StatusPanelID.PageOfPages), true);
		}
		public override void OnBrowserUpdated() {
			UpdateStatusCore(ReportLocalizer.GetString(ReportStringId.RibbonXRDesign_StatusBar_HtmlDone));
		}
		public override void OnDesignerVisible() {
			base.OnDesignerVisible();
			UpdateStatusCore(status);
			UpdateStopBarItemVisibility();
		}
		public override void OnPreviewVisible() {
			UpdateRibbonController(designController.PrintRibbonController, tabControl.PreviewControl);
			base.OnPreviewVisible();
			designController.SetBarItemLocked(designController.GetBarItemBy(StatusPanelID.PageOfPages), false);
		}
		public override void UpdateStatus(string status) {
			this.status = status;
			UpdateStatusCore(status);
		}
		void UpdateStatusCore(string status) {
			BarStaticItem item = designController.GetBarItemBy(StatusPanelID.PageOfPages);
			if(item != null) {
				item.Caption = status;
				item.Hint = string.Empty;
			}
		}
		protected override ZoomTrackBarEditItem CreateZoomTrackBar() {
			return designController.GetBarItemBy(PrintingSystemCommand.ZoomTrackBar) as ZoomTrackBarEditItem;
		}
		protected override ZoomService CreateZoomService() {
			if(designController.XRDesignPanel != null)
				return designController.XRDesignPanel.GetService(typeof(ZoomService)) as ZoomService;
			return null;
		}
		protected override BarStaticItem GetZoomFactorTextStaticItem() {
			return designController.GetBarItemBy(StatusPanelID.ZoomFactorText);
		}
		protected override void SetBarItemLocked(BarItem item, bool locked) {
			designController.SetBarItemLocked(item, locked);
		}
		protected override void EnableCommand(PrintingSystemCommand command, bool enabled) {
			designController.EnableCommand(command, enabled);
		}
		void UpdateStopBarItemVisibility() {
			BarItem item = designController.GetBarItemBy(PrintingSystemCommand.StopPageBuilding);
			if(item != null)
				item.Visibility = BarItemVisibility.Never;
		}
	}
}
