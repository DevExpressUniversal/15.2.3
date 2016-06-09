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

using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraReports.UI;
using DevExpress.DashboardCommon.Export;
using System.Windows.Forms;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.DashboardExport;
using System.IO;
using System.Drawing;
using System;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native.Printing {
	public class DashboardReportPrintTool : ReportPrintTool, ICommandHandler {
		readonly IReportHolder reportHolder;
		readonly IExportOptionsOwner options;
		readonly IDashboardExportUIProvider uiProvider;
		readonly IDashboardExportItem exportItem;
		public DashboardReportPrintTool(IReportHolder reportHolder, IDashboardExportItem exportItem, IExportOptionsOwner options, IDashboardExportUIProvider uiProvider)
			: base(reportHolder.Report) {
			this.reportHolder = reportHolder;
			this.options = options;
			this.uiProvider = uiProvider;
			this.exportItem = exportItem;
			PrintingSystem.AddCommandHandler(this);
			DashboardWinHelper.SetPrintPreviewCommandsVisibility(this, DashboardWinHelper.GetPrintPreviewHiddenCommands(exportItem.ExportItemType));
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.EditPageHF, CommandVisibility.All);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.Parameters, CommandVisibility.None);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SubmitParameters, CommandVisibility.None);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.DocumentMap, CommandVisibility.None);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.Customize, CommandVisibility.All);
			PrintingSystem.PageSettingsChanged += (s, e) => {
				UpdateReport();
			};
			PrintingSystem.AfterMarginsChange += (s, e) => {
				UpdateReport();
			};
		}
		public bool CanHandleCommand(PrintingSystemCommand command, IPrintControl printControl) {
			return command == PrintingSystemCommand.EditPageHF || command == PrintingSystemCommand.Customize;
		}
		public void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl pc, ref bool handled) {
			if(command == PrintingSystemCommand.Customize) {
				if(uiProvider.ShowPrintPreviewExportForm(String.Empty, exportItem.ExportItemType, options))
					UpdateReport();
			}
			if(command == PrintingSystemCommand.EditPageHF) {
				PrintControl printControl = (PrintControl)pc;
				DashboardExportReport report = (DashboardExportReport)reportHolder.Report;
				using(Form form = PageHeaderFooterAccessor.GetEditorForm(report.PageHeaderFooter, report.Images)) {
					ISupportLookAndFeel lookAndFeelSupport = form as ISupportLookAndFeel;
					if(lookAndFeelSupport != null)
						lookAndFeelSupport.LookAndFeel.ParentLookAndFeel = printControl.LookAndFeel;
					if(form != null && form.ShowDialog(printControl) != DialogResult.Cancel) {
						int index = printControl.SelectedPageIndex;
						UpdateReport();
						printControl.SelectedPageIndex = index;
					}
				}
			}
		}
		void UpdateReport() {
			PrintingSystemBase ps = reportHolder.Report.PrintingSystemBase;
			DashboardReportOptions opts = options.GetActual().GetOptions();
			opts.PageOptions.PaperOptions.UseCustomMargins = true;
			opts.PageOptions.PaperOptions.PaperKind = ps.PageSettings.PaperKind;
			opts.PageOptions.PaperOptions.Landscape = ps.PageSettings.Landscape;
			opts.PageOptions.PaperOptions.CustomMargins = GraphicsUnitConverter.Convert(ps.PageMargins, 100, 96);
			opts.PageOptions.ScalingOptions.ScaleFactor = ps.Document.ScaleFactor;
			opts.AutoPageOptions.AutoRotate = false;
			opts.AutoPageOptions.AutoFitToPageSize = false;
			using(MemoryStream stream = new MemoryStream()) {
				ps.Watermark.SaveToStream(stream);
				Color color = ps.Graph.PageBackColor;
				reportHolder.Update(exportItem.CreateExportInfo(opts));
				stream.Position = 0;
				ps.Watermark.RestoreFromStream(stream);
				ps.Graph.PageBackColor = color;
			}
		}
	}
}
