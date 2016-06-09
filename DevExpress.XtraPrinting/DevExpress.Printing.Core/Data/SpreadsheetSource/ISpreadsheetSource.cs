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
using DevExpress.Utils;
namespace DevExpress.SpreadsheetSource {
	using DevExpress.SpreadsheetSource.Xls;
	using DevExpress.SpreadsheetSource.Xlsx;
	using DevExpress.SpreadsheetSource.Csv;
	#region SpreadsheetDocumentFormat
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Spelling", "DF1000")]
	public enum SpreadsheetDocumentFormat {
		Xls,
		Xlsx,
		Xlsm,
		Csv
	}
	#endregion
	#region ISpreadsheetSource
	public interface ISpreadsheetSource : IDisposable {
		IWorksheetCollection Worksheets { get; }
		IDefinedNamesCollection DefinedNames { get; }
		ITablesCollection Tables { get; }
		SpreadsheetDocumentFormat DocumentFormat { get; }
		int MaxColumnCount { get; }
		int MaxRowCount { get; }
		ISpreadsheetSourceOptions Options { get; }
		ISpreadsheetDataReader GetDataReader(IWorksheet worksheet);
		ISpreadsheetDataReader GetDataReader(XlCellRange range);
		ISpreadsheetDataReader GetDataReader(IWorksheet worksheet, XlCellRange range);
		ISpreadsheetDataReader GetDataReader(IDefinedName definedName);
		ISpreadsheetDataReader GetDataReader(IWorksheet worksheet, IDefinedName definedName);
	}
	#endregion
	#region SpreadsheetSourceFactory
	public static class SpreadsheetSourceFactory {
		public static ISpreadsheetSource CreateSource(string fileName) {
			return CreateSource(fileName, DetectFormat(fileName), new SpreadsheetSourceOptions());
		}
		public static ISpreadsheetSource CreateSource(string fileName, ISpreadsheetSourceOptions options) {
			return CreateSource(fileName, DetectFormat(fileName), options);
		}
		public static ISpreadsheetSource CreateSource(string fileName, SpreadsheetDocumentFormat documentFormat) {
			return CreateSource(fileName, documentFormat, new SpreadsheetSourceOptions());
		}
		public static ISpreadsheetSource CreateSource(string fileName, SpreadsheetDocumentFormat documentFormat, ISpreadsheetSourceOptions options) {
			switch(documentFormat) {
				case SpreadsheetDocumentFormat.Xls:
					return new XlsSpreadsheetSource(fileName, options);
				case SpreadsheetDocumentFormat.Xlsx:
				case SpreadsheetDocumentFormat.Xlsm:
					return new XlsxSpreadsheetSource(fileName, options);
				case SpreadsheetDocumentFormat.Csv:
					return new CsvSpreadsheetSource(fileName, CsvSpreadsheetSourceOptions.ConvertToCsvOptions(options));
			}
			return null;
		}
		public static ISpreadsheetSource CreateSource(Stream stream, SpreadsheetDocumentFormat documentFormat) {
			return CreateSource(stream, documentFormat, new SpreadsheetSourceOptions());
		}
		public static ISpreadsheetSource CreateSource(Stream stream, SpreadsheetDocumentFormat documentFormat, ISpreadsheetSourceOptions options) {
			switch(documentFormat) {
				case SpreadsheetDocumentFormat.Xls:
					return new XlsSpreadsheetSource(stream, options);
				case SpreadsheetDocumentFormat.Xlsx:
				case SpreadsheetDocumentFormat.Xlsm:
					return new XlsxSpreadsheetSource(stream, options);
				case SpreadsheetDocumentFormat.Csv:
					return new CsvSpreadsheetSource(stream, CsvSpreadsheetSourceOptions.ConvertToCsvOptions(options));
			}
			return null;
		}
		static SpreadsheetDocumentFormat DetectFormat(string fileName) {
			string extension = Path.GetExtension(fileName);
			if(StringExtensions.CompareInvariantCultureIgnoreCase(extension, ".xls") == 0)
				return SpreadsheetDocumentFormat.Xls;
			if(StringExtensions.CompareInvariantCultureIgnoreCase(extension, ".xlsx") == 0)
				return SpreadsheetDocumentFormat.Xlsx;
			if(StringExtensions.CompareInvariantCultureIgnoreCase(extension, ".xlsm") == 0)
				return SpreadsheetDocumentFormat.Xlsm;
			if(StringExtensions.CompareInvariantCultureIgnoreCase(extension, ".csv") == 0)
				return SpreadsheetDocumentFormat.Csv;
			throw new ArgumentException("Unknown extension. Can't detect document format.");
		}
	}
	#endregion
	#region InvalidFileException
	public class InvalidFileException : Exception {
		public InvalidFileException(string message)
			: base(message) {
		}
	}
	#endregion
	#region EncryptedFileException
	public enum EncryptedFileError {
		PasswordRequired,
		WrongPassword,
		EncryptionTypeNotSupported
	}
	public class EncryptedFileException : Exception {
		public EncryptedFileException(EncryptedFileError error, string message)
			: base(message) {
			Error = error;
		}
		public EncryptedFileError Error { get; private set; }
	}
	#endregion
}
namespace DevExpress.SpreadsheetSource.Implementation {
	using DevExpress.Office.Utils;
	#region SpreadsheetSourceBase (abstract)
	public abstract class SpreadsheetSourceBase : ISpreadsheetSource {
		readonly WorksheetCollection worksheets = new WorksheetCollection();
		readonly DefinedNamesCollection definedNames = new DefinedNamesCollection();
		readonly TablesCollection tables = new TablesCollection();
		readonly ISpreadsheetSourceOptions options;
		readonly Dictionary<int, string> numberFormatCodes = new Dictionary<int, string>();
		readonly List<int> numberFormatIds = new List<int>();
		readonly ChunkedArray<string> sharedStrings = new ChunkedArray<string>(8192, 8192*64);
		string fileName;
		Stream inputStream;
		protected SpreadsheetSourceBase(string fileName, ISpreadsheetSourceOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(fileName, "fileName");
			Guard.ArgumentNotNull(options, "options");
			this.fileName = fileName;
			this.options = options;
			this.inputStream = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
		protected SpreadsheetSourceBase(Stream stream, ISpreadsheetSourceOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			this.fileName = string.Empty;
			this.inputStream = stream;
			this.options = options;
		}
		protected string FileName { get { return fileName; } }
		protected Stream InputStream { get { return inputStream; } }
		protected internal WorksheetCollection InnerWorksheets { get { return worksheets; } }
		protected internal DefinedNamesCollection InnerDefinedNames { get { return definedNames; } }
		protected internal TablesCollection InnerTables { get { return tables; } }
		protected internal Dictionary<int, string> NumberFormatCodes { get { return numberFormatCodes; } }
		protected internal List<int> NumberFormatIds { get { return numberFormatIds; } }
		protected internal ChunkedArray<string> SharedStrings { get { return sharedStrings; } }
		#region ISpreadsheetSource Members
		public IWorksheetCollection Worksheets {
			get { return worksheets; }
		}
		public IDefinedNamesCollection DefinedNames {
			get { return definedNames; }
		}
		public ITablesCollection Tables {
			get { return tables; }
		}
		public ISpreadsheetSourceOptions Options {
			get { return options; }
		}
		public abstract SpreadsheetDocumentFormat DocumentFormat { get; }
		public abstract int MaxColumnCount { get; }
		public abstract int MaxRowCount { get; }
		public abstract ISpreadsheetDataReader GetDataReader(IWorksheet worksheet);
		public abstract ISpreadsheetDataReader GetDataReader(IWorksheet worksheet, XlCellRange range);
		public virtual ISpreadsheetDataReader GetDataReader(XlCellRange range) {
			Guard.ArgumentNotNull(range, "range");
			IWorksheet sheet = null;
			if(!string.IsNullOrEmpty(range.SheetName))
				sheet = Worksheets[range.SheetName];
			if(sheet == null)
				throw new InvalidOperationException("Unable to determine source worksheet.");
			return GetDataReader(sheet, range);
		}
		public virtual ISpreadsheetDataReader GetDataReader(IDefinedName definedName) {
			Guard.ArgumentNotNull(definedName, "definedName");
			XlCellRange range = definedName.Range;
			if(range == null)
				throw new InvalidOperationException("Unable to determine source range.");
			IWorksheet sheet = null;
			if(!string.IsNullOrEmpty(range.SheetName))
				sheet = Worksheets[range.SheetName];
			else if(!string.IsNullOrEmpty(definedName.Scope))
				sheet = Worksheets[definedName.Scope];
			if(sheet == null)
				throw new InvalidOperationException("Unable to determine source worksheet.");
			return GetDataReader(sheet, range);
		}
		public virtual ISpreadsheetDataReader GetDataReader(IWorksheet worksheet, IDefinedName definedName) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			Guard.ArgumentNotNull(definedName, "definedName");
			XlCellRange range = definedName.Range;
			if(range == null)
				throw new InvalidOperationException("Unable to determine source range.");
			if(!string.IsNullOrEmpty(range.SheetName) && worksheet.Name != range.SheetName)
				throw new InvalidOperationException("Defined name refers to another worksheet.");
			if(!string.IsNullOrEmpty(definedName.Scope) && worksheet.Name != definedName.Scope)
				throw new InvalidOperationException("Defined name scoped to another worksheet.");
			return GetDataReader(worksheet, range);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing && this.inputStream != null) {
				if(!string.IsNullOrEmpty(this.fileName))
					this.inputStream.Dispose();
				this.inputStream = null;
			}
		}
		#endregion
	}
	#endregion
}
