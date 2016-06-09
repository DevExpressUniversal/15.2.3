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

using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.SpreadsheetSource.Implementation;
using DevExpress.SpreadsheetSource.Xlsx.Import;
using DevExpress.Utils;
using System;
using System.IO;
namespace DevExpress.SpreadsheetSource.Xlsx {
	#region XlsSpreadsheetSource
	public class XlsxSpreadsheetSource : SpreadsheetSourceBase {
		#region Fields
		XlsxSpreadsheetSourceImporter importer;
		XlsxSourceDataReader dataReader;
		#endregion
		public XlsxSpreadsheetSource(string fileName, ISpreadsheetSourceOptions options)
			: base(fileName, options) {
			CreateImporter();
			ReadWorkbookData();
		}
		public XlsxSpreadsheetSource(Stream stream, ISpreadsheetSourceOptions options)
			: base(stream, options) {
			CreateImporter();
			ReadWorkbookData();
		}
		#region Properties
		public override SpreadsheetDocumentFormat DocumentFormat { get { return SpreadsheetDocumentFormat.Xlsx; } }
		public override int MaxColumnCount { get { return XlCellPosition.MaxColumnCount; } }
		public override int MaxRowCount { get { return XlCellPosition.MaxRowCount; } }
		internal XlsxSourceDataReader DataReader { get { return dataReader; } }
		protected internal bool UseDate1904 { get; set; }
		#endregion
		void CreateImporter() {
			importer = new XlsxSpreadsheetSourceImporter(this);
		}
		void ReadWorkbookData() {
			importer.Import(InputStream);
		}
		public override ISpreadsheetDataReader GetDataReader(IWorksheet worksheet) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			CloseDataReader();
			this.dataReader = new XlsxSourceDataReader(importer);
			this.dataReader.Open(worksheet, null);
			return this.dataReader;
		}
		public override ISpreadsheetDataReader GetDataReader(IWorksheet worksheet, XlCellRange range) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			CloseDataReader();
			this.dataReader = new XlsxSourceDataReader(importer);
			this.dataReader.Open(worksheet, range);
			return this.dataReader;
		}
		void CloseDataReader() {
			if (this.dataReader != null) {
				if (!this.dataReader.IsClosed)
					this.dataReader.Close();
				this.dataReader = null;
			}
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			if (disposing && this.importer != null) {
				importer.Dispose();
				importer = null;
			}
			base.Dispose(disposing);
		}
		#endregion
	}
	#endregion
}
