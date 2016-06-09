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
using System.Text;
using System.IO;
namespace DevExpress.XtraExport {
	public abstract class ExportExcelProvider : ExportCustomProvider, IExportStyleManagerCreator {
		ExportStyleManagerBase styleCache;
		public ExportExcelProvider(string fileName)
			: base(fileName) {
			InitializeSheetName(string.Empty, Path.GetFileNameWithoutExtension(fileName));
			styleCache = ExportStyleManagerBase.GetInstance(fileName, null, this);
		}
		public ExportExcelProvider(Stream stream, string sheetName, DevExpress.XtraPrinting.WorkbookColorPaletteCompliance workbookColorPaletteCompliance)
			: base(stream) {
			InitializeSheetName(sheetName, stream is FileStream ? Path.GetFileNameWithoutExtension(((FileStream)stream).Name) : ExportCustomProvider.StreamModeName);
			styleCache = ExportStyleManagerBase.GetInstance(string.Empty, stream, this);
		}
		protected ExportExcelProvider(Stream stream)
			: this(stream, string.Empty, DevExpress.XtraPrinting.WorkbookColorPaletteCompliance.ReducePaletteForExactColors) {
		}
		protected ExportStyleManagerBase StyleCache { get { return styleCache; } }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				styleCache.DisposeInstance();
		}
		protected abstract ExportExcelProvider CreateInstance(Stream stream);
		protected abstract ExportExcelProvider CreateInstance(string fileName);
		protected abstract ExportStyleManagerBase CreateExportStyleManager(string fileName, Stream stream);
		protected abstract void InitializeSheetName(string sheetName, string defaultSheetName);
		protected ExportExcelProvider CloneCore(string fileName, Stream stream) {
			return IsStreamMode ? CreateInstance(GetCloneStream(stream)) : CreateInstance(GetCloneFileName(fileName));
		}
		protected Stream GetCloneStream(Stream stream) {
			return stream != null ? stream : Stream;
		}
		protected string GetCloneFileName(string fileName) {
			return string.IsNullOrEmpty(fileName) ? FileName : fileName;
		}
		protected abstract int RegisterFormat(string formatString);
		protected int GetFormatId(ExportCacheCellStyle style) {
			if(string.IsNullOrEmpty(style.XlsxFormatString)) {
				if(string.IsNullOrEmpty(style.FormatString))
					return style.PreparedCellType;
				DevExpress.Export.Xl.XlExportNumberFormatConverter converter = new DevExpress.Export.Xl.XlExportNumberFormatConverter();
				ExcelNumberFormat format = converter.Convert(style.FormatString, style.PreparedCellType == XlsConsts.DateTimeFormat, System.Windows.Forms.Application.CurrentCulture);
				if(format == null)
					return style.PreparedCellType;
				return format.Id == -1 ? RegisterFormat(format.FormatString) : format.Id;
			} else
				return RegisterFormat(style.XlsxFormatString);
		}
		#region IExportStyleManagerCreator Members
		ExportStyleManagerBase IExportStyleManagerCreator.CreateInstance(string fileName, Stream stream) {
			return CreateExportStyleManager(fileName, stream);
		}
		#endregion
	}
	public enum ExcelBorderLineStyle : byte {
		None = 0x0,
		Thin = 0x1,
		Medium = 0x2,
		Dashed = 0x3,
		Dotted = 0x4,
		Thick = 0x5,
		Double = 0x6,
		Hair = 0x7,
		MediumDashed = 0x8,
		DashDot = 0x9,
		MediumDashDot = 0xA,
		DashDotDot = 0xB,
		MediumDashDotDot = 0xC,
		SlantDashDot = 0xD,	
	}
	public static class ExcelHelper {
		public static ExcelBorderLineStyle ConvertBorderStyle(int borderWidth, DevExpress.XtraPrinting.BorderDashStyle borderStyle) {
			if(borderWidth == 1) {
				switch(borderStyle) {
					case DevExpress.XtraPrinting.BorderDashStyle.Dash:
						return ExcelBorderLineStyle.Dashed;
					case DevExpress.XtraPrinting.BorderDashStyle.DashDot:
						return ExcelBorderLineStyle.DashDot;
					case DevExpress.XtraPrinting.BorderDashStyle.DashDotDot:
						return ExcelBorderLineStyle.DashDotDot;
					case DevExpress.XtraPrinting.BorderDashStyle.Solid:
						return ExcelBorderLineStyle.Thin;
				}
			}
			switch(borderStyle) {
				case DevExpress.XtraPrinting.BorderDashStyle.Dash:
					return ExcelBorderLineStyle.MediumDashed;
				case DevExpress.XtraPrinting.BorderDashStyle.DashDot:
					return ExcelBorderLineStyle.MediumDashDot;
				case DevExpress.XtraPrinting.BorderDashStyle.DashDotDot:
					return ExcelBorderLineStyle.MediumDashDotDot;
				case DevExpress.XtraPrinting.BorderDashStyle.Solid:
					return (borderWidth >= 3) ? ExcelBorderLineStyle.Thick : ExcelBorderLineStyle.Medium;
				case DevExpress.XtraPrinting.BorderDashStyle.Double:
					return ExcelBorderLineStyle.Double;
				case DevExpress.XtraPrinting.BorderDashStyle.Dot:
					return ExcelBorderLineStyle.Dotted;
			}
			return ExcelBorderLineStyle.Thin;
		}
		public static int GetBorderWidth(int borderWidth, DevExpress.XtraPrinting.BorderDashStyle borderStyle) {
			ExcelBorderLineStyle excelStyle = ConvertBorderStyle(borderWidth, borderStyle);
			switch(excelStyle) {
				case ExcelBorderLineStyle.Thick:
				case ExcelBorderLineStyle.Double:
					return 3;
				case ExcelBorderLineStyle.Medium:
				case ExcelBorderLineStyle.MediumDashDot:
				case ExcelBorderLineStyle.MediumDashDotDot:
				case ExcelBorderLineStyle.MediumDashed:
				case ExcelBorderLineStyle.SlantDashDot:
					return 2;
				case ExcelBorderLineStyle.Dashed:
				case ExcelBorderLineStyle.Dotted:
				case ExcelBorderLineStyle.DashDot:
				case ExcelBorderLineStyle.DashDotDot:
				case ExcelBorderLineStyle.Thin:
				case ExcelBorderLineStyle.Hair:
				default:
					return 1;
			}
		}
	}
}
