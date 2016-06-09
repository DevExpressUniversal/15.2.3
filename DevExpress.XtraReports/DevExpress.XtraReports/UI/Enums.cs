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
using System.Text;
using DevExpress.Data;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.UI
{
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum SnappingMode { SnapLines, SnapToGrid }
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum GroupFooterUnion {
		None,
		WithLastDetail
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum GroupUnion {
		None,
		WholePage,
		WithFirstDetail
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum ValueSuppressType {
		Leave,
		Suppress,
		SuppressAndShrink,
		[Obsolete("This API is now obsolete.")]
		MergeByValue,
		[Obsolete("This API is now obsolete.")]
		MergeByTag
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum ProcessDuplicatesMode {
		Leave,
		Merge,
		Suppress,
		SuppressAndShrink
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum ProcessDuplicatesTarget { 
		Value,
		Tag,
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum ChartImageType {
		Metafile,
		Bitmap
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum GaugeImageType {
		Metafile,
		Bitmap
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum SummaryFunc {
		Avg,
		Count,
		Sum,
		RunningSum,
		Percentage,
		Max,
		Min,
		Median,
		Var,
		VarP,
		StdDev,
		StdDevP,
		DAvg,
		DCount,
		DSum,
		DVar,
		DVarP,
		DStdDev,
		DStdDevP,
		RecordNumber,
		Custom
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum SortingSummaryFunction {
		Avg,
		Count,
		Sum,
		Max,
		Min,
		Median,
		Var,
		VarP,
		StdDev,
		StdDevP,
		DAvg,
		DCount,
		DSum,
		DVar,
		DVarP,
		DStdDev,
		DStdDevP,
		Custom
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum ReportUnit {
		HundredthsOfAnInch,
		TenthsOfAMillimeter,
		Pixels
	}
	public static class ReportUnitExtensions {
		public static float ToDpi(this ReportUnit reportUnit) {
			switch(reportUnit) {
				case ReportUnit.HundredthsOfAnInch: return GraphicsDpi.HundredthsOfAnInch;
				case ReportUnit.TenthsOfAMillimeter: return GraphicsDpi.TenthsOfAMillimeter;
				case ReportUnit.Pixels: return GraphicsDpi.DeviceIndependentPixel;
			}
			throw new NotSupportedException();
		}
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum SummaryRunning {
		None,
		Group,
		Report,
		Page
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum XRDockStyle {
		None,
		Top,
		Bottom,
		Fill
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum PageBreak {
		None,
		BeforeBand,
		BeforeBandExceptFirstEntry,
		AfterBand,
		AfterBandExceptLastEntry
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum WinControlDrawMethod {
		UseWMPaint = 0,
		UseWMPrint = 1,
		UseWMPaintRecursive = 2,
		UseWMPrintRecursive = 3
	}	
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum WinControlImageType {
		Metafile = 0,
		Bitmap = 1
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum WinControlPrintMode {
		Default,
		AsImage,
		AsBricks
	}
	[Obsolete("The DevExpress.XtraReports.UI.XRBarCodeOrientation class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.BarCodeOrientation class instead.")]
	public enum XRBarCodeOrientation {
		Normal,
		UpsideDown,
		RotateLeft,
		RotateRight
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum MultiColumnMode {
		None,
		UseColumnCount,
		UseColumnWidth
	}
	[Obsolete("Use the DevExpress.XtraPrinting.ColumnLayout enumeration instead.")]
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum ColumnDirection {
		DownThenAcross,
		AcrossThenDown
	}
	public enum XRRichTextStreamType { 
		RtfText, 
		PlainText,
		HtmlText,
		XmlText
	}
}
namespace DevExpress.XtraReports {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
	public enum ScriptLanguage {
		CSharp = 0,
		VisualBasic = 1,
		JScript = 2
	}
}
namespace DevExpress.XtraReports.Native {
	public enum BrickPresentation { Runtime, Design, LayoutView }
}
