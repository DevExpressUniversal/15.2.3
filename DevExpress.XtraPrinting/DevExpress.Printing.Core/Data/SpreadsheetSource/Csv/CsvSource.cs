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
namespace DevExpress.SpreadsheetSource.Csv {
	using DevExpress.Utils;
	public class CsvSpreadsheetSource : SpreadsheetSourceBase {
		CsvSourceDataReader dataReader = null;
		public CsvSpreadsheetSource(string fileName, ISpreadsheetSourceOptions options)
			: base(fileName, options) {
			InitializeWorkbookStructure();
		}
		public CsvSpreadsheetSource(Stream stream, ISpreadsheetSourceOptions options) 
			: base(stream, options) {
			InitializeWorkbookStructure();
		}
		protected internal CsvSpreadsheetSourceOptions InnerOptions { 
			get { return Options as CsvSpreadsheetSourceOptions; }
		}
		public override SpreadsheetDocumentFormat DocumentFormat {
			get { return SpreadsheetDocumentFormat.Csv; }
		}
		public override int MaxRowCount {
			get { return Int32.MaxValue; }
		}
		public override int MaxColumnCount {
			get { return Int32.MaxValue; }
		}
		void InitializeWorkbookStructure() {
			InnerWorksheets.Add(new Worksheet("Sheet", XlSheetVisibleState.Visible));
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				CloseDataReader();
			}
			base.Dispose(disposing);
		}
		public override ISpreadsheetDataReader GetDataReader(IWorksheet worksheet) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			CloseDataReader();
			this.dataReader = new CsvSourceDataReader(this, this.InputStream);
			this.dataReader.Open(worksheet, null);
			return this.dataReader;
		}
		public override ISpreadsheetDataReader GetDataReader(IWorksheet worksheet, XlCellRange range) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			CloseDataReader();
			this.dataReader = new CsvSourceDataReader(this, this.InputStream);
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
