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
using System.Linq;
using System.Text;
using DevExpress.XtraReports;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Printing;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Parameters;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Printing.Native;
namespace DevExpress.XtraReports.UI {
	public class ReportPrintToolWpf : IReportPrintTool {
		#region Fields And Properties
		IReport report;
		DXWindow previewWindow;
		bool disposed;
		XtraReportPreviewModel model;
		XtraReportPreviewModel Model {
			get {
				return model != null ? model : new XtraReportPreviewModel(report);
			}
		}
		#endregion
		#region Constructors
		public ReportPrintToolWpf(IReport report) : this(report, null) { }
		internal ReportPrintToolWpf(IReport report, XtraReportPreviewModel model) {
			if(report == null)
				throw new ArgumentNullException("report");
			this.report = report;
			this.model = model;
			report.PrintTool = this;
		}
		#endregion
		#region Public Methods
		public void ShowPreview(Window ownerWindow) {
			ShowPreviewCore(ownerWindow, false);
		}
		public void ShowPreviewDialog(Window ownerWindow) {
			ShowPreviewCore(ownerWindow, true);
		}
		#endregion
		#region IReportPrintTool Members
		public void Print() {
			Model.PrintDirect();
		}
		public void Print(string printerName) {
			Model.PrintDirect(printerName);
		}
		public bool? PrintDialog() {
			return Model.PrintDialog();
		}
		void IReportPrintTool.ShowPreview(object ownerWindow) {		  
			ShowPreview(ownerWindow as Window);
		}
		void IReportPrintTool.ClosePreview() {
			if(previewWindow != null) {
				previewWindow.Close();
			}
		}
		void IReportPrintTool.ShowPreviewDialog(object ownerWindow) {
			ShowPreviewDialog(ownerWindow as Window);
		}
		bool? IReportPrintTool.ShowPageSetupDialog(object ownerWindow) {
			return Model.ShowPageSetupDialog(ownerWindow as Window);
		}
		bool IReportPrintTool.ApproveParameters(Parameter[] parameters, bool requestParameters) {
			return false;
		}
		List<ParameterInfo> IReportPrintTool.ParametersInfo {
			get {
				return null;
			}
		}
		#endregion
		#region IReportPrintTool not supported methods
		void IReportPrintTool.ShowPreview() {
			throw new NotSupportedException("Use ShowPreview(object ownerWindow) method instead.");
		}
		void IReportPrintTool.ShowPreviewDialog() {
			throw new NotSupportedException("Use ShowPreviewDialog(object ownerWindow) method instead.");
		}
		void IReportPrintTool.ShowRibbonPreview(object lookAndFeel) {
			throw new NotSupportedException();
		}
		void IReportPrintTool.ShowRibbonPreview() {
			throw new NotSupportedException();
		}
		void IReportPrintTool.CloseRibbonPreview() {
			throw new NotSupportedException();
		}
		void IReportPrintTool.ShowRibbonPreviewDialog(object lookAndFeel) {
			throw new NotSupportedException();
		}
		void IReportPrintTool.ShowRibbonPreviewDialog() {
			throw new NotSupportedException();
		}
		#endregion
		#region Methods
		void ShowPreviewCore(Window ownerWindow, bool isModal) {
			if(ownerWindow == null)
				throw new ArgumentNullException("ownerWindow");
			CreateDocumentIfEmpty(true);
			previewWindow = CreatePreviewWindow(ownerWindow);
			if(isModal) {
				previewWindow.ShowDialog();
			} else {
				previewWindow.Show();
			}
		}
		DXWindow CreatePreviewWindow(Window ownerWindow) {
			DocumentPreviewWindow window = new DocumentPreviewWindow() { Owner = ownerWindow, Model = Model };
			return window;
		}
		void CreateDocumentIfEmpty(bool buildPagesInBackground) {
			if(report.PrintingSystemBase.Document.PageCount == 0) {
				report.CreateDocument(buildPagesInBackground);
			}
		}		
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		void Dispose(bool disposing) {
			if(!disposed) {
				if(disposing) {
					if(report.PrintTool == this)
						report.PrintTool = null;
				}
				disposed = true;
			}
		}
		#endregion
	}
}
