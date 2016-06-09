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
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.Utils;
using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public static class XlsStreams {
		public static readonly string WorkbookStreamName = "Workbook";
		public static readonly string BookStreamName = "Book";
		public static readonly string XmlStreamName = "XML";
		public static readonly string VbaProjectRootStorageName = "_VBA_PROJECT_CUR\\";
		public static readonly string SummaryStreamName = "\x0005SummaryInformation";
		public static readonly string DocSummaryStreamName = "\x0005DocumentSummaryInformation";
		public static readonly string CustomXMlStreamName = "MsoDataStore";
	}
	public class XlsImporter : DocumentModelImporter, IDisposable {
		public const string InvalidFileMessage = "Invalid Xls file";
		#region Fields
		readonly DocumentImporterOptions options;
		XlsContentBuilder contentBuilder;
		XlsImportStyleSheet styleSheet;
		#endregion
		public XlsImporter(DocumentModel documentModel, XlsDocumentImporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
			this.styleSheet = new XlsImportStyleSheet(this);
		}
		#region Properties
		protected internal XlsContentBuilder ContentBuilder { get { return this.contentBuilder; } }
		protected internal XlsDocumentImporterOptions Options { get { return (XlsDocumentImporterOptions)this.options; } }
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public XlsImportStyleSheet StyleSheet { get { return styleSheet; } }
		#endregion
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				if (this.contentBuilder != null)
					this.contentBuilder.Dispose();
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void Import(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			DocumentModel.BeginSetContent();
			DocumentModel.DataContext.PushCurrentLimits(XlsDefs.MaxColumnCount, XlsDefs.MaxRowCount);
			try {
				ImportWorkbook(stream);
				if((DocumentModel.SheetCount == 0) && Options.CreateDefaultDocumentOnContentError) {
					Worksheet sheet = new Worksheet(DocumentModel);
					sheet.VisibleState = SheetVisibleState.Visible;
					DocumentModel.Sheets.Add(sheet);
				}
			}
#if !DISCOVER
			catch {
				if(Options.CreateEmptyDocumentOnLoadError)
					DocumentModel.SetMainDocumentEmptyContentCore();
				throw;
			}
#endif
			finally {
				DocumentModel.DataContext.PopCurrentLimits();
				DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument);
			}
		}
		protected internal virtual void ImportWorkbook(Stream stream) {
			ILogService logService = DocumentModel.GetService<ILogService>();
			if(logService != null)
				logService.Clear();
			this.contentBuilder = new XlsContentBuilder(stream, StyleSheet, Options);
			ContentBuilder.ReadWorkbookSubstreams();
			ContentBuilder.SetupWorkbookWindowProperties();
			ContentBuilder.CreatePivotCaches();
			ContentBuilder.GeneratePivotItemsAndWholeRange(); 
			ContentBuilder.ReadVbaProjectContent();
			ContentBuilder.ReadOlePropertiesContent(XlsStreams.SummaryStreamName);
			ContentBuilder.ReadOlePropertiesContent(XlsStreams.DocSummaryStreamName);
			ContentBuilder.SetupRealTimeData();
			ContentBuilder.ReadCustomXmlData();
		}
		#region errors
		internal static void ThrowInvalidXlsFile() {
			throw new Exception(InvalidFileMessage);
		}
		internal static void ThrowInvalidXlsFile(string reason) {
			throw new Exception(InvalidFileMessage + " : " + reason);
		}
		public override void ThrowInvalidFile() {
			throw new Exception(InvalidFileMessage);
		}
		public virtual void ThrowInvalidFile(string reason) {
			throw new Exception(InvalidFileMessage + " : " + reason);
		}
		#endregion
	}
}
