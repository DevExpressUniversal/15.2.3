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

using DevExpress.DataAccess.Localization;
namespace DevExpress.DataAccess.Native {
	public static class FileNameFilterStrings {
		static string fileNameFilterFormat = "{0} {1}";
		static string extensionsFormat = "({0})|{0}";
		static string xmlExtensions = "*.xml;*.xsd";
		static string excelExtensions = "*.xls;*.xlsx;*.xlsm;";
		static string csvExtensions = "*.csv";
		static readonly string allExcelExtensions = excelExtensions + ';' + csvExtensions;
		static readonly string xmlFormattedExtensions = FormatExtensions(xmlExtensions);
		static readonly string excelFormattedExtensions = FormatExtensions(excelExtensions);
		static readonly string csvFormattedExtensions = FormatExtensions(csvExtensions);
		static readonly string allExcelFormattedExtensions = FormatExtensions(allExcelExtensions);
		public static string Xml {
			get { return FormatFilter(DataAccessLocalizer.GetString(DataAccessStringId.XmlFileStrategy_FileNameFilter), xmlFormattedExtensions); }
		}
		public static string ExcelCsv {
			get { return string.Join("|", AllSupportedExcel, Excel, Csv); }
		}
		static string AllSupportedExcel {
			get { return FormatFilter(DataAccessLocalizer.GetString(DataAccessStringId.FileNameFilter_AllFormats), allExcelFormattedExtensions); }
		}
		static string Excel {
			get { return FormatFilter(DataAccessLocalizer.GetString(DataAccessStringId.FileNameFilter_Excel), excelFormattedExtensions); }
		}
		static string Csv {
			get { return FormatFilter(DataAccessLocalizer.GetString(DataAccessStringId.FileNameFilter_CSV), csvFormattedExtensions); }
		}
		static string FormatExtensions(string extensions) {
			return string.Format(extensionsFormat, extensions);
		}
		static string FormatFilter(string filterName, string filterExtensions) {
			return string.Format(fileNameFilterFormat, filterName, filterExtensions);
		}
	}
}
