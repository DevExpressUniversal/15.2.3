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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Drawing;
using System.IO;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	#region HeaderFooterLayoutCalculator
	public class HeaderFooterLayoutCalculator {
		#region Properties
		readonly Worksheet sheet;
		Rectangle pageBounds;
		#endregion
		public HeaderFooterLayoutCalculator(Worksheet sheet, Rectangle pageBounds) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.pageBounds = pageBounds;
		}
		#region Properties
		protected internal Margins Margins { get { return sheet.Margins; } }
		protected internal HeaderFooterOptions HeaderFooter { get { return sheet.HeaderFooter; } }
		#endregion
		public HeaderFooterLayout CalculateHeaderFooter() {
			if (!ShouldCalculateLayout())
				return null;
			Rectangle headerBounds = CalculateHeaderFooterBounds();
			IHeaderFooterFormatTagProvider tagProvider = new HeaderFooterFormatTagProvider(sheet);
			return new HeaderFooterLayout(headerBounds, HeaderFooter, tagProvider);
		}
		bool ShouldCalculateLayout() {
			if (!String.IsNullOrEmpty(HeaderFooter.FirstHeader) || !String.IsNullOrEmpty(HeaderFooter.EvenHeader) ||
				!String.IsNullOrEmpty(HeaderFooter.OddHeader) || !String.IsNullOrEmpty(HeaderFooter.FirstFooter) ||
				!String.IsNullOrEmpty(HeaderFooter.EvenFooter) || !String.IsNullOrEmpty(HeaderFooter.OddFooter))
				return true;
			return false;
		}
		Rectangle CalculateHeaderFooterBounds() {
			DocumentModelUnitToLayoutUnitConverter unitConverter = sheet.Workbook.ToDocumentLayoutUnitConverter;
			int left = pageBounds.Left + unitConverter.ToLayoutUnits(Margins.Left);
			int top = pageBounds.Top + unitConverter.ToLayoutUnits(Margins.Header);
			int right = pageBounds.Right - unitConverter.ToLayoutUnits(Margins.Right);
			int bottom = pageBounds.Bottom - unitConverter.ToLayoutUnits(Margins.Footer);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Layout {
	#region HeaderFooterFormatTagProvider
	public class HeaderFooterFormatTagProvider : IHeaderFooterFormatTagProvider {
		#region Fields
		readonly Worksheet sheet;
		string filePath;
		string fileName;
		#endregion
		public HeaderFooterFormatTagProvider(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.filePath = GetFilePath();
			this.fileName = GetFileName();
		}
		#region Properties
		public string CurrentPage { get { return "&P"; } }
		public string TotalPages { get { return "&N"; } }
		public string CurrentDate { get { return DateTime.Now.ToShortDateString(); } }
		public string CurrentTime { get { return DateTime.Now.ToShortTimeString(); } }
		public string FilePath { get { return filePath; } }
		public string FileName { get { return fileName; } }
		public string SheetName { get { return sheet.Name; } }
		#endregion
		string GetActualFullFileName() {
			WorkbookSaveOptions documentSaveOptions = sheet.Workbook.DocumentSaveOptions;
			string currentFileName = documentSaveOptions.CurrentFileName;
			return String.IsNullOrEmpty(currentFileName) ? documentSaveOptions.DefaultFileName : currentFileName;
		}
		string GetFilePath() {
			string fileName = GetActualFullFileName();
			if (String.IsNullOrEmpty(fileName))
				return String.Empty;
			string directoryName = Path.GetDirectoryName(fileName);
			if (string.IsNullOrEmpty(directoryName))
				return String.Empty;
			return Path.GetFullPath(Path.GetDirectoryName(fileName)) + "\\";
		}
		string GetFileName() {
			string fileName = GetActualFullFileName();
			return Path.GetFileNameWithoutExtension(fileName);
		}
	}
	#endregion
	#region HeaderFooterPageCounter
	public class HeaderFooterPageCounter {
		#region Fields
		string currentSheetName;
		int currentIndex;
		#endregion
		public int GetPageIndex(Worksheet sheet) {
			string sheetName = sheet.Name;
			if (currentSheetName == sheetName)
				return ++currentIndex;
			this.currentSheetName = sheetName;
			PrintSetup setup = sheet.PrintSetup;
			this.currentIndex = setup.UseFirstPageNumber ? setup.FirstPageNumber : currentIndex + 1;
			return currentIndex;
		}
	}
	#endregion
}
