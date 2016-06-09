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
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.SpreadsheetSource.Implementation;
namespace DevExpress.SpreadsheetSource.Xls {
	using DevExpress.Office.Utils;
	using DevExpress.XtraExport.Xls;
	using DevExpress.Utils;
	#region XlsContentType
	internal enum XlsContentType {
		None,
		WorkbookGlobals,
		Sheet,
		Chart,
		ChartSheet,
		MacroSheet,
		VisualBasicModule,
		Workspace,
		SheetCustomView,
		ChartCustomView,
		ChartSheetCustomView,
		MacroSheetCustomView,
		VisualBasicModuleCustomView,
	}
	#endregion
	public partial class XlsSpreadsheetSource : SpreadsheetSourceBase {
		const string workbookStreamName = "Workbook";
		PackageFileReader packageFileReader;
		BinaryReader baseReader;
		XlsReader workbookReader;
		IXlsSourceCommandFactory commandFactory;
		XlsSourceDataReader dataReader;
		IWorksheet currentSheet = null;
		protected internal XlsReader WorkbookReader { get { return this.workbookReader; } }
		protected internal IXlsSourceCommandFactory CommandFactory { get { return this.commandFactory; } }
		protected internal XlsSourceDataReader DataReader { get { return dataReader; } }
		protected internal IWorksheet CurrentSheet { get { return currentSheet; } }
		public XlsSpreadsheetSource(string fileName, ISpreadsheetSourceOptions options)
			: base(fileName, options) {
			InitializeWorkbookReader();
			ReadWorkbookGlobals();
			ReadTableDefinitions();
		}
		public XlsSpreadsheetSource(Stream stream, ISpreadsheetSourceOptions options) 
			: base(stream, options) {
			InitializeWorkbookReader();
			ReadWorkbookGlobals();
			ReadTableDefinitions();
		}
		void InitializeWorkbookReader() {
			this.packageFileReader = new PackageFileReader(InputStream, true);
			this.baseReader = this.packageFileReader.GetCachedPackageFileReader(workbookStreamName);
			if(this.baseReader == null)
				throw new InvalidFileException("Workbook stream not found!");
			this.workbookReader = new XlsReader(this.baseReader);
			this.commandFactory = new XlsSourceCommandFactory();
		}
		internal void SetupRC4Decryptor(string password, byte[] salt) {
			((IDisposable)this.workbookReader).Dispose();
			this.workbookReader = new XlsRC4EncryptedReader(this.baseReader, password, salt);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.commandFactory = null;
				CloseDataReader();
				if(this.workbookReader != null) {
					((IDisposable)this.workbookReader).Dispose();
					this.workbookReader = null;
				}
				if(this.packageFileReader != null) {
					this.packageFileReader.Dispose();
					this.packageFileReader = null;
				}
			}
			base.Dispose(disposing);
		}
		public override SpreadsheetDocumentFormat DocumentFormat {
			get { return SpreadsheetDocumentFormat.Xls; }
		}
		public override int MaxColumnCount {
			get { return XlsDefs.MaxColumnCount; }
		}
		public override int MaxRowCount {
			get { return XlsDefs.MaxRowCount; }
		}
		public override ISpreadsheetDataReader GetDataReader(IWorksheet worksheet) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			CloseDataReader();
			this.dataReader = new XlsSourceDataReader(this);
			this.dataReader.Open(worksheet, null);
			return this.dataReader;
		}
		public override ISpreadsheetDataReader GetDataReader(IWorksheet worksheet, XlCellRange range) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			CloseDataReader();
			this.dataReader = new XlsSourceDataReader(this);
			this.dataReader.Open(worksheet, range);
			return this.dataReader;
		}
		void CloseDataReader() {
			if(this.dataReader != null) {
				if(!this.dataReader.IsClosed)
					this.dataReader.Close();
				this.dataReader = null;
			}
		}
	}
}
