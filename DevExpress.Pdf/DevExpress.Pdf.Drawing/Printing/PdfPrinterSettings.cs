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
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Pdf {
	public enum PdfPrintScaleMode { Fit, ActualSize, CustomScale }
	public enum PdfPrintPageOrientation { Auto, Portrait, Landscape }
	public class PdfPrinterSettings {
		internal const float MaxScale = 500;
		internal const float MinScale = 10;
		int[] pageNumbers;
		PdfPrintScaleMode scaleMode;
		float scale;
		PdfPrintPageOrientation pageOrientation;
		PrinterSettings settings;
		public PrinterSettings Settings { get { return settings; } }
		public int[] PageNumbers {
			get { return pageNumbers; }
			set {
				if (pageNumbers != value)
					pageNumbers = value;
				settings.PrintRange = PrintRange.SomePages;
			}
		}
		public PdfPrintScaleMode ScaleMode {
			get { return scaleMode; }
			set {
				if (!scaleMode.Equals(value)) {
					scaleMode = value;
				}
			}
		}
		public float Scale {
			get { return scale; }
			set {
				value = value < MinScale ? MinScale : value;
				value = value > MaxScale ? MaxScale : value;
				if (scale != value)
					scale = value;
			}
		}
		public int PrintingDpi {
			get {
				PrinterResolution res = settings.DefaultPageSettings.PrinterResolution;
				return Math.Max(res.X, res.Y);
			}
			set {
				settings.DefaultPageSettings.PrinterResolution = new PrinterResolution() { X = value, Y = value };
			}
		}
		public PdfPrintPageOrientation PageOrientation {
			get { return pageOrientation; }
			set {
				if (pageOrientation != value) {
					pageOrientation = value;
					switch (value) {
						case PdfPrintPageOrientation.Landscape:
							if (!settings.DefaultPageSettings.Landscape)
								settings.DefaultPageSettings.Landscape = true;
							break;
						case PdfPrintPageOrientation.Portrait:
							if (settings.DefaultPageSettings.Landscape)
								settings.DefaultPageSettings.Landscape = false;
							break;
					}
				}
			}
		}
		internal int PrinterMaxDpi {
			get {
				int result = 0;
				foreach (PrinterResolution printerResolution in settings.PrinterResolutions) {
					int resolution = printerResolution.X;
					if (printerResolution.Y > 0 && (resolution <= 0 || (resolution > printerResolution.Y && resolution > 0)))
						resolution = printerResolution.Y;
					result = Math.Max(result, resolution);
				}
				return result;
			}
		}
		public PdfPrinterSettings()
			: this(null) {
		}
		public PdfPrinterSettings(PrinterSettings settings)
			: this(settings, null, PdfPrintPageOrientation.Auto, 100f, PdfPrintScaleMode.Fit) {
		}
		internal PdfPrinterSettings(PrinterSettings settings, int[] pageNumbers, PdfPrintPageOrientation pageOrientation, float scale, PdfPrintScaleMode scaleMode) {
			this.settings = settings ?? new PrinterSettings() { PrintRange = PrintRange.AllPages };
			this.pageNumbers = pageNumbers;
			this.pageOrientation = pageOrientation;
			this.scale = scale;
			this.scaleMode = scaleMode;
		}
		internal void SetPageNumbers(int currentPageNumber, int pageCount, string pageRangeText = null) {
			pageNumbers = GetPageNumbers(currentPageNumber, pageCount, pageRangeText);
		}
		internal int[] GetPageNumbers(int currentPageNumber, int pageCount, string pageRangeText = null) {
			switch (settings.PrintRange) {
				case PrintRange.CurrentPage:
				case PrintRange.Selection:
					return new int[] { currentPageNumber };
				case PrintRange.SomePages:
					if (String.IsNullOrEmpty(pageRangeText)) {
						if (pageNumbers != null && pageNumbers.Length > 0)
							return pageNumbers;
						int from = Math.Min(Math.Max(settings.FromPage, 1), pageCount);
						int to = settings.ToPage > 0 && settings.ToPage < pageCount ? settings.ToPage : pageCount;
						return CreateArray(from, to);
					}
					return GetPageNumbers(pageRangeText, pageCount);
				default:
					return GetPageNumbers("", pageCount);
			}
		}
		int[] GetPageNumbers(string rangeText, int maxValue) {
			if (String.IsNullOrEmpty(rangeText))
				return CreateArray(1, maxValue);
			List<int> result = new List<int>();
			foreach (string value in rangeText.Split(','))
				result.AddRange(ParseElement(value, maxValue));
			return result.ToArray();
		}
		IEnumerable<int> ParseElement(string s, int maxIndex) {
			string[] items = s.Split('-');
			int valueFrom;
			if (!Int32.TryParse(items[0], out valueFrom))
				valueFrom = Int32.MaxValue;
			if (items.Length == 1)
				if (valueFrom <= maxIndex && valueFrom > 0)
					return new int[] { valueFrom };
				else
					return new int[0];
			int valueTo;
			if (!(items[1].Length > 0 && Int32.TryParse(items[1], out valueTo)))
				valueTo = Int32.MaxValue;
			return CreateArray(Math.Min(valueFrom, valueTo), Math.Min(Math.Max(valueFrom, valueTo), maxIndex));
		}
		int[] CreateArray(int from, int to) {
			if (from > to)
				return new int[0];
			from = from < 1 ? 1 : from;
			to = to < 1 ? 1 : to;
			int[] result = new int[to - from + 1];
			for (int i = 0; i < result.Length; i++)
				result[i] = from + i;
			return result;
		}
	}
}
