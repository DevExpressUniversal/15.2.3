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
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Preview.Native;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraBars.Design;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPrinting.Design {
	public class PrintBarManagerDesigner : SpecializedBarManagerDesigner {		
		#region static
		public static void SetPrintControl(IPrintPreviewControl printPreviewControl, IDesignerHost host) {
			PrintControl printControl = DevExpress.XtraPrinting.Native.DesignHelpers.FindInheritedComponent(host.Container, typeof(PrintControl)) as PrintControl;
			if(printControl == null) {
				printControl = DevExpress.XtraPrinting.Native.DesignHelpers.CreateComponent(host, typeof(DocumentViewer)) as PrintControl;
				System.Diagnostics.Debug.Assert(printControl != null);
				if(printControl != null)
					printControl.Dock = DockStyle.Fill;
			} else {
				printPreviewControl.PrintControl = printControl;
			}
		}
		#endregion
		PrintBarManager PrintBarManager { get { return (PrintBarManager)Component; } }
		public PrintBarManagerDesigner() {
		}
		protected override BarManagerConfigurator[] CreateUpdaters() {
			return new BarManagerConfigurator[] { 
				new ScalePrintBarManagerUpdater(PrintBarManager),
				new SaveOpenPrintBarManagerUpdater(PrintBarManager),
				new ThumbnailsPrintBarManagerUpdater(PrintBarManager),
				new ParametersPrintBarManagerUpdater(PrintBarManager),
				new StatusPanelUpdater(PrintBarManager),
				new XlsxBarManagerUpdater(PrintBarManager)
			};
		}
		protected override void InitSpecializedBarManager() {
			((PrintBarManager)Manager).Initialize(null);
			SetPrintControl(PrintBarManager, DesignerHost);
		}
		protected override BarManagerActionList CreateBarManagerActionList() {
			return new PrintBarManagerActionList(this);
		}
		protected override void OnAboutClick(object sender, EventArgs e) {
			PrintingSystem.About();
		}
		protected override DXAboutActionList GetAboutAction() { return new DXAboutActionList(Component, new MethodInvoker(PrintingSystem.About)); }
	}
	public class XlsxBarManagerUpdater : PrintBarManagerUpdaterBase {
		protected override PrintingSystemCommand[] UpdateCommands { get { return new PrintingSystemCommand[] { PrintingSystemCommand.ExportXlsx }; } }
		public XlsxBarManagerUpdater(PrintBarManager manager)
			: base(manager) {
		}
		protected override void CreateUpdateItems() {
			PrintPreviewBarCheckItem item =
				new PrintPreviewBarCheckItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_XlsxDocument), PrintingSystemCommand.ExportXlsx);
			AddBarItem(null, item, item.Caption, "", item.Caption, -1, false);
		}
	}
	public class StatusPanelUpdater : PrintBarManagerUpdaterBase {
		public static bool ShouldUpdate(BarManager manager, StatusPanelID[] ids) {
			foreach(StatusPanelID id in ids) {
				if(PreviewItemsLogicBase.GetBarItemByStatusPanelID(manager, id) == null)
					return true;
			}
					return false;
			}
		public static void ClearStatusItems(BarItemLinkCollection itemLinks) {
			ArrayList links = new ArrayList(itemLinks);
			foreach(BarItemLink link in links) {
				BarEditItem barEditItem = link.Item as BarEditItem;
				if(barEditItem != null && barEditItem.Edit != null)
					barEditItem.Edit.Dispose();
				if(link.Item != null)
					link.Item.Dispose();
				link.Dispose();
		}
			}
		public override bool UpdateNeeded {
			get {
				return ShouldUpdate(this.PrintBarManager, new StatusPanelID[] { StatusPanelID.PageOfPages, StatusPanelID.Progress, StatusPanelID.ZoomFactor });
			}
		}
		public StatusPanelUpdater(PrintBarManager manager)
			: base(manager) {
		}
		public StatusPanelUpdater(PrintBarManager manager, PrintBarManagerConfigurator parentConfigurator)
			: base(manager, parentConfigurator) {
		}
		protected override void CreateUpdateItems() {
			ClearStatusItems(this.PrintBarManager.StatusBar.ItemLinks);
			this.parentConfigurator.AddStatusPanelItems();
		}
	}
}
