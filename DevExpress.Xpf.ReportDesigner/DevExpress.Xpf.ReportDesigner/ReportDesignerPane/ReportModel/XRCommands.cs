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
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Office.Internal;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public class XRCommands : BaseXRCommands {
		readonly XtraReportModel report;
		readonly DelegateCommand showPropertiesCommand;
		readonly DelegateCommand<BandKind> insertBandCommand;
		readonly DelegateCommand<object> insertDetailReportCommand;
		readonly DelegateCommand insertTableCellCommand;
		readonly DelegateCommand insertTableRowCommand;
		readonly DelegateCommand insertTableColumn;
		readonly DelegateCommand deleteTableRowCommand;
		readonly DelegateCommand deleteTableColumnCommand;
		readonly DelegateCommand deleteTableCellCommand;
		readonly DelegateCommand insertSubBandCommand;
		readonly DelegateCommand clearCommand;
		readonly DelegateCommand<string> addNewScriptCommand;
		readonly DelegateCommand<IReportDesignerUI> loadFileCommand;
		readonly DelegateCommand<string> addNewStyleCommand;
		readonly DelegateCommand<XRSubreportDiagramItem> openSubreportCommand;
		readonly DelegateCommand runWizardCommand;
		public XRCommands(XtraReportModel report) {
			this.report = report;
			showPropertiesCommand = new DelegateCommand(() => report.Owner.Do(x => x.ShowProperties()), false);
			runWizardCommand = new DelegateCommand(() => report.Owner.Do(x => x.RunWizard()), false);
			insertBandCommand = new DelegateCommand<BandKind>(DoInsertBand, x => CanInsertBand(x), false);
			insertDetailReportCommand = new DelegateCommand<object>(DoInsertDetailReport, false);
			insertSubBandCommand = new DelegateCommand(DoInsertSubBand, CanInsertSubBand, false);
			insertTableCellCommand = new DelegateCommand(DoInsertTableCell, false);
			insertTableRowCommand = new DelegateCommand(DoInsertTableRow, false);
			insertTableColumn = new DelegateCommand(DoInsertTableColumn, false);
			deleteTableRowCommand = new DelegateCommand(DoDeleteTableRow, false);
			deleteTableColumnCommand = new DelegateCommand(DoDeleteTableColumn, false);
			deleteTableCellCommand = new DelegateCommand(DoDeleteTableCell, false);
			clearCommand = new DelegateCommand(DoClear, false);
			addNewScriptCommand = new DelegateCommand<string>(DoAddNewScript, false);
			loadFileCommand = new DelegateCommand<IReportDesignerUI>(DoLoadFile, false);
			addNewStyleCommand = new DelegateCommand<string>(DoAddNewStyle, false);
			openSubreportCommand = new DelegateCommand<XRSubreportDiagramItem>(DoOpenSubreport, false);
			this.report.Controls.CollectionChanged += OnBandsCollectionChanged;
		}
		bool CanInsertBand(BandKind bandKind) {
			if(bandKind == BandKind.ReportHeader || bandKind == BandKind.ReportFooter)
				return GetContainerToInsert(bandKind).Return(container => container.Items.OfType<BandDiagramItem>().All(x => x.BandKind != bandKind), () => false);
			return !IsGlobalBand(bandKind) || !IsGlobalBandExists(bandKind);
		}
		bool CanInsertSubBand() {
			var selectedModel = XRModelBase.GetXRModel(report.DiagramItem.Diagram.PrimarySelection) as XRControlModelBase;
			if(selectedModel == null) return false;
			var selectedBand = selectedModel.XRObject.Band;
			return !(selectedBand is SubBand) && selectedBand.BandKind != BandKind.TopMargin && selectedBand.BandKind != BandKind.BottomMargin;
		}
		DiagramContainer GetContainerToInsert(BandKind newBandKind) {
			var primarySelection = report.DiagramItem.Diagram.PrimarySelection;
			var selectedModel = XRModelBase.GetXRModel(primarySelection);
			Band selectedBand = primarySelection is XRDiagramRoot ? ((XtraReportModel)selectedModel).XRObject.Band : ((XRControlModelBase)selectedModel).XRObject.Band;
			if(selectedBand == null)
				return null;
			return IsGlobalBand(newBandKind) || selectedBand.Report == report.XRObject
				? (DiagramContainer)report.DiagramItem
				: ((BandModel)report.Factory.GetModel(selectedBand.Report)).DiagramItem;
		}
		void DoInsertSubBand() {
			var selectedModel = (XRControlModelBase)XRModelBase.GetXRModel(report.DiagramItem.Diagram.PrimarySelection);
			var selectedBand = selectedModel.XRObject.Band;
			var selectedBandModel = (BandModel)report.Factory.GetModel(selectedBand);
			var newSubBand = new SubBand();
			var newSubBandDiagramItem = report.Factory.GetModel(newSubBand, true).DiagramItem;
			report.DiagramItem.Diagram.DrawItem(newSubBandDiagramItem, selectedBandModel.DiagramItem, new Rect(new Point(0.0, selectedBandModel.DiagramItem.Height), new Size(0.0, BoundsConverter.ToDouble(newSubBand.HeightF, newSubBand.Dpi))), 0);
		}
		void DoInsertDetailReport(object dataSource) {
			var newBand = (XtraReportBase)XtraReport.CreateBand(BandKind.DetailReport);
			var newDetailBand = XtraReport.CreateBand(BandKind.Detail);
			newBand.Bands.Add(newDetailBand);
			var newBandDiagramItem = report.Factory.GetModel(newBand).DiagramItem;
			report.DiagramItem.Diagram.DrawItem(newBandDiagramItem, GetContainerToInsert(BandKind.DetailReport), new Rect(new Size(0.0, BoundsConverter.ToDouble(newDetailBand.HeightF, newDetailBand.Dpi))));
		}
		void DoInsertBand(BandKind newBandKind) {
			var newBand = XtraReport.CreateBand(newBandKind);
			var newBandDiagramItem = report.Factory.GetModel(newBand).DiagramItem;
			int? index = newBandKind == BandKind.SubBand ? (int?)0 : null;
			report.DiagramItem.Diagram.DrawItem(newBandDiagramItem, GetContainerToInsert(newBandKind), new Rect(new Size(0.0, BoundsConverter.ToDouble(newBand.HeightF, newBand.Dpi))), index);
		}
		bool IsGlobalBand(BandKind bandKind) {
			return bandKind == BandKind.BottomMargin || bandKind == BandKind.TopMargin || bandKind == BandKind.PageHeader || bandKind == BandKind.PageFooter;
		}
		bool IsGlobalBandExists(BandKind bandKind) {
			return report.XRObject.Bands[bandKind] != null;
		}
		void OnBandsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			insertBandCommand.RaiseCanExecuteChanged();
		}
		XRControlModelBase PrimarySelection { get { return (XRControlModelBase)XRModelBase.GetXRModel(report.DiagramItem.Diagram.PrimarySelection); } }
		XRDiagramControl Diagram { get { return report.DiagramItem.Diagram; } }
		void DoInsertTableCell() {
			var tableRowModel = (XRTableRowModel)report.Factory.GetModel(((XRTableCellModel)PrimarySelection).XRObject.Row);
			var newTableCellDiagramItem = report.Factory.GetModel(new XRTableCell()).DiagramItem;
			report.DiagramItem.Diagram.DrawItem(newTableCellDiagramItem, tableRowModel.DiagramItem, newTableCellDiagramItem.ActualBounds());
		}
		void DoInsertTableRow() {
			var cellsCount = ((XRTableCellModel)PrimarySelection).XRObject.Row.Cells.Count;
			var tableModel = (XRTableModel)report.Factory.GetModel(((XRTableCellModel)PrimarySelection).XRObject.Row.Table);
			var newTableRow = new XRTableRow();
			for(int i = cellsCount; --i >= 0; )
				newTableRow.Cells.Add(new XRTableCell());
			var newTableRowDiagramItem = report.Factory.GetModel(newTableRow).DiagramItem;
			report.DiagramItem.Diagram.DrawItem(newTableRowDiagramItem, tableModel.DiagramItem, newTableRowDiagramItem.ActualBounds());
		}
		void DoInsertTableColumn() {
			var tableModel = (XRTableModel)report.Factory.GetModel(((XRTableCellModel)PrimarySelection).XRObject.Row.Table);
			foreach(XRTableRowModel row in tableModel.Controls) {
				var newTableCellDiagramItem = ((XRTableCellModel)report.Factory.GetModel(new XRTableCell())).DiagramItem;
				var tableRowDiagramItem = ((XRTableRowModel)report.Factory.GetModel(row.XRObject)).DiagramItem;
				report.DiagramItem.Diagram.DrawItem(newTableCellDiagramItem, tableRowDiagramItem, newTableCellDiagramItem.ActualBounds());
			}
		}
		void DoDeleteTableRow() {
			var tableModel = (XRTableModel)report.Factory.GetModel(((XRTableCellModel)PrimarySelection).XRObject.Row.Table);
			if(tableModel.Controls.Count == 1)
				Diagram.SelectItem(tableModel.DiagramItem);
			else
				Diagram.SelectItem(((XRTableRowModel)report.Factory.GetModel(((XRTableCellModel)PrimarySelection).XRObject.Row)).DiagramItem);
			Diagram.DeleteSelectedItems();
		}
		void DoDeleteTableColumn() {
			int cellToRemoveIndex = ((XRTableCellModel)PrimarySelection).XRObject.Row.Controls.IndexOf(PrimarySelection.XRObject);
			var tableModel = (XRTableModel)report.Factory.GetModel(((XRTableCellModel)PrimarySelection).XRObject.Row.Table);
			int rowsToDelete = 0;
			foreach(var row in tableModel.Controls) {
				if(row.Controls.Count <= cellToRemoveIndex)
					continue;
				if(row.Controls.Count == 1) {
					rowsToDelete++;
					if(!row.DiagramItem.IsSelected)
						Diagram.SelectItem(row.DiagramItem, true);
				} else {
					var tableCellDiagramItem = row.Controls[cellToRemoveIndex].DiagramItem;
					if(!tableCellDiagramItem.IsSelected)
						Diagram.SelectItem(tableCellDiagramItem, true);
				}
			}
			if(rowsToDelete == tableModel.Controls.Count)
				Diagram.SelectItem(tableModel.DiagramItem);
			Diagram.DeleteSelectedItems();
		}
		void DoDeleteTableCell() {
			if(((XRTableRowModel)report.Factory.GetModel(((XRTableCellModel)PrimarySelection).XRObject.Row)).Controls.Count == 1)
				DoDeleteTableRow();
			else
				Diagram.DeleteSelectedItems();
		}
		void DoClear() {
			Diagram.SelectionModel.SetPropertyValueEx((XRRichText x) => x.Text, string.Empty);
		}
		void DoAddNewScript(string eventName) {
			XRScriptsBase scripts = ((XRControl)report.SelectedModel.XRObject).Scripts;
			if(scripts == null)
				return;
			string procName = scripts.GetProcName(eventName);
			if(string.IsNullOrEmpty(procName))
				procName = scripts.GetDefaultPropertyValue(eventName);
			string baseName = scripts.GetDefaultPropertyValue(eventName);
			procName = baseName;
			int index = 1;
			while(report.Scripts.Contains(procName))
				procName = string.Format("{0}_{1}", baseName, index++);
			scripts.SetPropertyValue(eventName, procName);
			report.Scripts.Add(procName);
			string script = scripts.GenerateDefaultEventScript(eventName, procName);
			report.ScriptsSource += "\r\n" + script;
		}
		void DoLoadFile(IReportDesignerUI designerUI) {
			FileDialogFilterCollection filters = new FileDialogFilterCollection();
			var descriptions = new string[] {
				XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_RtfFiles),
				XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_OpenXmlFiles),
				XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_TextFiles),
				XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_HtmlFiles)
			};
			var extensions = new string[][] {
				new string[] { "rtf" },
				new string[] { "docx" },
				new string[] { "txt" },
				new string[] { "html", "htm" }
			};
			for(int i = 0; i < descriptions.Length; i++)
				filters.Add(new FileDialogFilter(descriptions[i], extensions[i]));
			designerUI.DoWithOpenFileDialogService(dialog => {
				dialog.Filter = filters.CreateFilterString();
				if(dialog.ShowDialog()) {
					XRRichText richText = new XRRichText();
					richText.LoadFile(dialog.GetFullFileName());
					Diagram.SelectionModel.SetPropertyValueEx((XRRichText x) => x.Rtf, richText.Rtf);
					Diagram.InvalidateRenderLayer();
				}
			});
		}
		void DoAddNewStyle(string name) {
			var style = new XRControlStyle();
			report.XRObject.StyleSheet.Add(style);
			report.Controls.Add(report.Factory.GetModel(style));
			PrimarySelection.XRObject.Styles.EvenStyle = style;
		}
		void DoOpenSubreport(XRSubreportDiagramItem diagramItem) {
			report.Owner.Do(x => x.OpenSubreport(diagramItem));
		}
		public override ICommand ShowProperties { get { return showPropertiesCommand; } }
		public override ICommand<BandKind> InsertBand { get { return insertBandCommand; } }
		public override ICommand<object> InsertDetailReport { get { return insertDetailReportCommand; } }
		public override ICommand InsertSubBand { get { return insertSubBandCommand; } }
		public override ICommand InsertTableCell { get { return insertTableCellCommand; } }
		public override ICommand InsertTableColumn { get { return insertTableColumn; } }
		public override ICommand InsertTableRow { get { return insertTableRowCommand; } }
		public override ICommand DeleteTableRow { get { return deleteTableRowCommand; } }
		public override ICommand DeleteTableColumn { get { return deleteTableColumnCommand; } }
		public override ICommand DeleteTableCell { get { return deleteTableCellCommand; } }
		public override ICommand Clear { get { return clearCommand; } }
		public override ICommand<string> AddNewScript { get { return addNewScriptCommand; } }
		public override ICommand<IReportDesignerUI> LoadFile { get { return loadFileCommand; } }
		public override ICommand<string> AddNewStyle { get { return addNewStyleCommand; } }
		public override ICommand<XRSubreportDiagramItem> OpenSubreport { get { return openSubreportCommand; } }
		public override ICommand RunWizard { get { return runWizardCommand; } }
	}
}
