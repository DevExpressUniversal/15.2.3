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
using DevExpress.Utils.Controls;
using DevExpress.XtraCharts;
using System.ComponentModel;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.XtraCharts.Printing {
	public class ChartOptionsPrint : BaseOptions, IXtraSupportShouldSerialize {
		const PrintSizeMode defaultSizeMode = PrintSizeMode.None;
		const PrintImageFormat defaultImageFormat = PrintImageFormat.Bitmap;
		PrintSizeMode sizeMode = defaultSizeMode;
		PrintImageFormat imageFormat = defaultImageFormat;
		Chart chart;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartOptionsPrintSizeMode"),
#endif
		XtraSerializableProperty
		]
		public PrintSizeMode SizeMode {
			get { return sizeMode; }
			set {
				if (sizeMode != value) {
					sizeMode = value;
					UpdatePrinterSizeMode();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartOptionsPrintImageFormat"),
#endif
		XtraSerializableProperty
		]
		public PrintImageFormat ImageFormat {
			get { return imageFormat; }
			set {
				if (imageFormat != value) {
					imageFormat = value;
					UpdatePrinterImageFormat();
				}
			}
		}
		internal ChartOptionsPrint(Chart chart) {
			this.chart = chart;
		}
		#region XtraSerializing
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			if (propertyName == "SizeMode")
				return ShouldSerializeSizeMode();
			if (propertyName == "ImageFormat")
				return ShouldSerializeImageFormat();
			return true;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeSizeMode() {
			return SizeMode != defaultSizeMode;
		}
		bool ShouldSerializeImageFormat() {
			return imageFormat != defaultImageFormat;
		}
		internal new bool ShouldSerialize() {
			return ShouldSerializeSizeMode() || ShouldSerializeImageFormat();
		}
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			ChartOptionsPrint printOptions = options as ChartOptionsPrint;
			if (printOptions == null)
				return;
			SizeMode = printOptions.SizeMode;
			ImageFormat = printOptions.ImageFormat;
		}
		public override string ToString() {
			return "(OptionsPrint)";
		}
		void UpdatePrinterSizeMode() {
			if (chart != null && chart.Printer != null) 
				chart.Printer.SizeMode = sizeMode;
		}
		void UpdatePrinterImageFormat() {
			if (chart != null && chart.Printer != null)
				chart.Printer.ImageFormat = imageFormat;
		}
	}
}
