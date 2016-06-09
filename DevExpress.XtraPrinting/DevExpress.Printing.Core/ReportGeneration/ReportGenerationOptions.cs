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

using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.ReportGeneration {
	public class ReportGenerationOptions {
		DefaultBoolean printColumnHeaders;
		DefaultBoolean printTotalSummaryFooter;
		DefaultBoolean printGroupSummaryFooter;
		DefaultBoolean printGroupRows;
		DefaultBoolean printHorizontalLines;
		DefaultBoolean printVerticalLines;
		DefaultBoolean usePrintAppearances;
		DefaultBoolean enablePrintAppearanceEvenRow;
		DefaultBoolean enablePrintAppearanceOddRow;
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.True)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean PrintColumnHeaders{
			get { return printColumnHeaders; }
			set { printColumnHeaders = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.True)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean PrintGroupRows{
			get { return printGroupRows; }
			set { printGroupRows = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.True)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean PrintTotalSummaryFooter{
			get { return printTotalSummaryFooter; }
			set { printTotalSummaryFooter = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.True)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean PrintGroupSummaryFooter{
			get { return printGroupSummaryFooter; }
			set { printGroupSummaryFooter = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.True)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean PrintHorizontalLines { get { return printHorizontalLines; } set { printHorizontalLines = value; } }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.True)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean PrintVerticalLines { get { return printVerticalLines; } set { printVerticalLines = value; } }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.False)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean UsePrintAppearances{
			get { return usePrintAppearances; }
			set { usePrintAppearances = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.True)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnablePrintAppearanceEvenRow{
			get { return enablePrintAppearanceEvenRow; }
			set { enablePrintAppearanceEvenRow = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.True)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnablePrintAppearanceOddRow{
			get { return enablePrintAppearanceOddRow; }
			set { enablePrintAppearanceOddRow = value; }
		}
	}
	internal class ReportGeneratorOptionsUtils {
		public static DefaultBoolean GetActualOptionValue(DefaultBoolean currentValue, bool condition){
			return condition ? DefaultBoolean.False : currentValue;
		}
	}
}
