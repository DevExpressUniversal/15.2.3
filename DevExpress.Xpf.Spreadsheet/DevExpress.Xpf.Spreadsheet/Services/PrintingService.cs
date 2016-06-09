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
using System.Windows;
using DevExpress.Office.Printing;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.XamlExport;
using DevExpress.XtraPrinting;
using DevExpress.XtraSpreadsheet.Printing;
namespace DevExpress.XtraSpreadsheet.Services {
	public interface ISpreadsheetPrintingService {
		void ShowPrintPreview(ISpreadsheetControl control);
		void ShowRibbonPrintPreview(ISpreadsheetControl control);
	}
}
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	public class SpreadsheetPrintingService : ISpreadsheetPrintingService {
		static SpreadsheetPrintingService() {
			BrickXamlExporterFactory.RegisterExporter(typeof(OfficeRectBrick), typeof(OfficeRectBrickXamlExporter));
		}
		public void ShowPrintPreview(ISpreadsheetControl control) {
			ShowPrintPreviewCore(control, false);
		}
		public void ShowRibbonPrintPreview(ISpreadsheetControl control) {
			ShowPrintPreviewCore(control, true);
		}
		void ShowPrintPreviewCore(ISpreadsheetControl control, bool showRibbonPreview) {
			LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(control.InnerControl);
			LinkPreviewModel model = new SpreadsheetLinkPreviewModel(link);
			link.CreateDocument(false);
			Window previewWindow;
			if (showRibbonPreview)
				previewWindow = new RibbonDocumentPreviewWindow() { Model = model };
			else
				previewWindow = new DocumentPreviewWindow() { Model = model };
			previewWindow.Closed += (s, e) => {
				model.Dispose();
				link.Dispose();
			};
			previewWindow.ShowDialog();
		}
	}
	public class SpreadsheetLinkPreviewModel : LinkPreviewModel {
		public SpreadsheetLinkPreviewModel() {
		}
		public SpreadsheetLinkPreviewModel(DevExpress.Xpf.Printing.LinkBase link)
			: base(link) {
		}
		protected override bool CanExport(object parameter) {
			bool result = base.CanExport(parameter);
			if(!result)
				return false;
			ExportFormat format = GetExportFormatByExportParameter(parameter);
			return format == ExportFormat.Image || format == ExportFormat.Pdf;
		}
#if !SL
		protected override bool CanSend(object parameter) {
			bool result = base.CanSend(parameter);
			if (!result)
				return false;
			if (parameter == null)
				return true;
			try {
				ExportFormat format;
				if (parameter is ExportFormat)
					format = (ExportFormat)parameter;
				else
					format = (ExportFormat)Enum.Parse(typeof(ExportFormat), parameter.ToString());
				return format == ExportFormat.Image || format == ExportFormat.Pdf;
			}
			catch {
				return false;
			}
		}
		protected override bool CanSave(object parameter) {
			return false;
		}
		protected override bool CanOpen(object parameter) {
			return false;
		}
#endif
		protected override bool CanPageSetup(object parameter) {
			return false;
		}
		protected override bool CanScale(object parameter) {
			return false;
		}
		protected override bool CanShowSearchPanel(object parameter) {
			return false;
		}
	}
}
