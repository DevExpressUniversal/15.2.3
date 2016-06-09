#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
namespace DevExpress.DashboardWin.Bars {
	public abstract class DataDashboardItemToolsBar : DashboardCommandBar {
		protected internal abstract CommandBasedBarItemBuilder MasterFilterBuilder { get; }
		protected internal abstract CommandBasedBarItemBuilder DrillDownBuilder { get; }
		protected internal abstract CommandBasedBarItemBuilder TargetDimensionsBuilder { get; }
		protected internal virtual CommandBasedBarItemBuilder MasterFilterOptionsBuilder { get { return new MasterFilterOptionsItemBuilder(); } }
		protected DataDashboardItemToolsBar() {
			Visible = false;
		}
	}
	public class SelectDashboardBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public SelectDashboardBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandSelectDashboardCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandSelectDashboardDescription);
			Glyph = ImageHelper.GetImage("Bars.NewDashboard_16x16");
			LargeGlyph = ImageHelper.GetImage("Bars.NewDashboard_32x32");
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new SelectDashboardCommand(viewer).Execute();
		}
	}
	public class SelectDashboardItemGroupBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public SelectDashboardItemGroupBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandSelectDashboardItemGroupCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandSelectDashboardItemGroupDescription);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new SelectDashboardItemGroupCommand(viewer).Execute();
		}
	}
	public class PrintPreviewItemBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public PrintPreviewItemBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandPrintPreview);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandPrintPreview);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new PrintPreviewItemCommand(viewer, itemViewer).Execute();
		}
	}
	public class ExportItemToPdfBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public ExportItemToPdfBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandExportToPdf);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandExportToPdf);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new ExportItemToPdfCommand(viewer, itemViewer).Execute();
		}
	}
	public class ExportItemToImageBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public ExportItemToImageBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandExportToImage);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandExportToImage);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new ExportItemToImageCommand(viewer, itemViewer).Execute();
		}
	}
	public class ExportItemToExcelBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public ExportItemToExcelBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandExportToExcel);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandExportToExcel);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new ExportItemToExcelCommand(viewer, itemViewer).Execute();
		}
	}
	public class SelectedItemElementBarItem : BarCheckItem, IDashboardViewerCommandBarItem {
		readonly int elementIndex;
		public SelectedItemElementBarItem(int elementIndex)
			: base() {
			this.elementIndex = elementIndex;
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new SetSelectedElementCommand(viewer, itemViewer, elementIndex).Execute();
		}
	}
	public class ClearMasterFilterBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public ClearMasterFilterBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandClearMasterFilter);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandClearMasterFilter);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new ClearMasterFilterCommand(viewer, itemViewer).Execute();
		}
	}
	public class ClearSelectionBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public ClearSelectionBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandClearSelection);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandClearSelection);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new ClearSelectionCommand(viewer, itemViewer).Execute();
		}
	}
	public class DrillUpBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public DrillUpBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDrillUp);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDrillUp);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new DrillUpCommand(viewer, itemViewer).Execute();
		}
	}
	public class MapInitialExtentBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public MapInitialExtentBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandMapInitialExtentCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.CommandMapInitialExtentCaption);
		}
		void IDashboardViewerCommandBarItem.ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new MapInitialExtentCommand(viewer, itemViewer).Execute();
		}
	}
}
