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
using DevExpress.XtraReports.UI;
using System.Data;
using System.IO;
using DevExpress.XtraPrinting;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Templates;
namespace DevExpress.XtraReports.Native.Templates {
	public class TemplateTool {
		int pageIndex;
		bool wasStopped;
		XtraReport report;
		public TemplateTool(XtraReport report) {
			this.report = report;
		}
		public Template SaveReportTemplate(string description, string name, int pageIndex) {
			Template template = new Template();
			string dataSourceSchema = report.DataSourceSchema == null ? string.Empty : report.DataSourceSchema;
			try {
				if(report.DataSource is DataSet)
					report.DataSourceSchema = ((DataSet)report.DataSource).GetXmlSchema();
				using(MemoryStream stream = new MemoryStream()) {
					report.SaveLayoutToXml(stream);
					template.LayoutBytes = stream.GetBuffer();
				}
				template.Description = description;
				template.Name = name;
				template.IconBytes = GetImage(pageIndex);
				template.Rating = 0;
			} finally {
				report.DataSourceSchema = dataSourceSchema;
			}
			return template;
		}
		public byte[] GetImage(int pageIndex) {
			wasStopped = false;
			exception = string.Empty;
			report.PrintingSystem.DocumentChanged += new EventHandler(PrintingSystem_DocumentChanged);
			report.PrintingSystem.CreateDocumentException += new DevExpress.XtraPrinting.ExceptionEventHandler(OnCreateDocumentException);
			this.pageIndex = Math.Max(1, pageIndex);
			try {
				report.CreateDocument(true);
				do {
					Application.RaiseIdle(EventArgs.Empty);
					Application.DoEvents();
				} while (report.Document.IsCreating);
			} finally {
				report.PrintingSystem.DocumentChanged -= new EventHandler(PrintingSystem_DocumentChanged);
				report.PrintingSystem.CreateDocumentException -= new DevExpress.XtraPrinting.ExceptionEventHandler(OnCreateDocumentException);
			}
			if(!wasStopped && report.Document.PageCount > 0) {
				int index = Math.Min(report.Document.Pages.Count, this.pageIndex);
				ImageExportOptions imExpOpt = new ImageExportOptions();
				imExpOpt.ExportMode = ImageExportMode.SingleFilePageByPage;
				imExpOpt.Format = ImageFormat.Png;
				imExpOpt.Resolution = 100;
				imExpOpt.PageRange = index.ToString();
				imExpOpt.PageBorderWidth = 0;
				using(MemoryStream stream = new MemoryStream()) {
					report.ExportToImage(stream, imExpOpt);
					return stream.ToArray();
				}
			}
			return null;
		}
		string exception = string.Empty;
		void OnCreateDocumentException(object sender, DevExpress.XtraPrinting.ExceptionEventArgs args) {
			StopPageBuilding();
			args.Handled = true;
			exception = args.Exception.Message;
		}
		public void GetImageAsync(int pageIndex, Action<byte[], string> callback) {
			StopPageBuilding();
			string errorMessage = string.Empty;
			Thread thread = new Thread(new ThreadStart(delegate() {
				byte[] result;
				try {
					result = GetImage(pageIndex);
					errorMessage = exception;
				} catch (Exception exception) {
					result = null;
					errorMessage = exception.Message;
				}
				callback(result, errorMessage);
			}));
			thread.Start();
		}
		public void StopPageBuilding() {
			report.PrintingSystem.Document.StopPageBuilding();
			wasStopped = true;
		}
		void PrintingSystem_DocumentChanged(object sender, EventArgs e) {
			PrintingSystemBase ps = (PrintingSystemBase)sender;
			if(ps.Pages.Count >= pageIndex + 1)
				ps.Document.StopPageBuilding();
		}
		public XtraReport LoadReportTemplate(Template template) {
			return null;
		}
	}
}
