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
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsChartDefs
	public static class XlsChartDefs {
		public const int PointIndexOfSeries = 0xffff;
		public const int MaxPointIndex = 31999;
		public const int AxisIndex = 0;
		public const int MajorGridlinesIndex = 1;
		public const int MinorGridlinesIndex = 2;
		public const int SurfaceIndex = 3;
		public const int DropLinesIndex = 0;
		public const int HiLowLinesIndex = 1;
		public const int SeriesLinesIndex = 2;
		public const int LeaderLinesIndex = 3;
		public const int AxisContext = 0x0000;
		public const int MajorGridlinesContext = 0x0001;
		public const int MinorGridlinesContext = 0x0002;
		public const int SurfaceContext = 0x0003;
		public const int DropLinesContext = 0x0000;
		public const int HiLowLinesContext = 0x0001;
		public const int LeaderLinesContext = 0x0002;
		public const int SeriesLinesContext = 0x0003;
		public const int DataContext = 0x0000;
		public const int MarkerContext = 0x0001;
		public const int FrameContext = 0x0000;
		public const int PlotAreaFrameContext = 0x0001;
		public const int ChartAreaFrameContext = 0x0002;
		public const int CategoryAxisInstance = 0x0000;
		public const int ValueAxisInstance = 0x0001;
		public const int SeriesAxisInstance = 0x0002;
		public const int XAxisInstance = 0x0003;
		public const int LegendOfDataTableContext = 0x0000;
		public const int LegendOfChartContext = 0x0001;
		public const int DataLabelContext = 0x0005;
	}
	#endregion
	#region XlsChartObjectKind
	public enum XlsChartObjectKind {
		DisplayUnits = 0x0010,
		FontCache = 0x0011,
		ExtendedDataLabel = 0x0012
	}
	#endregion
	#region XlsChartPrintSize
	public enum XlsChartPrintSize {
		Default = 0,
		AllPage = 1,
		AllPageKeepAspect = 2,
		DefinedByChart = 3
	}
	#endregion
	#region XlsChartCachedValue
	public class XlsChartCachedValue {
		public XlsChartCachedValue(int index, VariantValue value) {
			Index = index;
			Value = value;
		}
		public int Index { get; private set; }
		public VariantValue Value { get; private set; }
	}
	#endregion
}
