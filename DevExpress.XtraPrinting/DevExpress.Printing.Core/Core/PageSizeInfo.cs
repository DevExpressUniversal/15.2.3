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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Globalization;
#if SL
using DevExpress.Xpf.Drawing.Printing;
#else
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class PageSizeInfo {
		#region Nested Types
		public class PageSize {
			readonly SizeF size;
			readonly GraphicsUnit unit;
			readonly float dpi;
			public SizeF Size {
				get { return size; }
			}
			public GraphicsUnit Unit {
				get { return unit; }
			}
			public float Dpi {
				get { return dpi; }
			}
			public PageSize(SizeF size, GraphicsUnit unit) {
				this.size = size;
				this.unit = unit;
				this.dpi = GraphicsDpi.UnitToDpi(unit);
			}
			public SizeF GetPageSize(float targetDpi) {
				if(targetDpi == Dpi)
					return Size;
				return GraphicsUnitConverter.Convert(Size, Dpi, targetDpi);
			}
			public SizeF GetPageSize(GraphicsUnit targetUnit) {
				if(targetUnit == Unit)
					return Size;
				return GraphicsUnitConverter.Convert(Size, Unit, targetUnit);
			}
		}
		#endregion
		public readonly static Size DefaultSize = new Size(850, 1100);
		readonly static Dictionary<PaperKind, PageSize> pageSizes = CreatePageSizeDictionary();
#if SL
		public static IEnumerable<PaperKind> AvailablePaperKinds {
			get { return pageSizes.Keys.Concat(new[] { PaperKind.Custom }); }
		}
#else
		public static PaperSize GetAppropriatePaperSize(PrinterSettings.PaperSizeCollection paperSizes, PaperSize paperSize) {
			return GetAppropriatePaperSize(paperSizes, paperSize.Kind, paperSize.Width, paperSize.Height);
		}
		public static PaperSize GetAppropriatePaperSize(PrinterSettings.PaperSizeCollection paperSizes, PaperKind paperKind, int paperWidth, int paperHeight) {
			foreach(PaperSize paperSize in paperSizes)
				if((paperKind != PaperKind.Custom && GetRawKind(paperSize) == paperKind) ||
					(paperSize.Width == paperWidth && paperSize.Height == paperHeight))
					return paperSize;
			return null;
		}
		internal static PaperSize GetPaperSize(ReadonlyPageData pageData, PrinterSettings.PaperSizeCollection paperSizes) {
			PaperSize paperSize = GetAppropriatePaperSize(paperSizes, pageData.PaperKind, pageData.Size.Width, pageData.Size.Height);
			if(paperSize == null)
				return new PaperSize(pageData.PaperKind.ToString(), pageData.Size.Width, pageData.Size.Height);
			return paperSize;
		}
		internal static PaperKind GetRawKind(PaperSize paperSize) {
			return (PaperKind)paperSize.RawKind;
		}
		public static Size GetPageSize(string paperName, string printerName, Size defaultValue) {
			try {
				var settings = new PrinterSettings() { PrinterName = printerName };
				foreach(PaperSize paperSize in settings.PaperSizes) {
					if(Object.Equals(paperSize.PaperName, paperName))
						return new Size(paperSize.Width, paperSize.Height);
				}
			} catch {
			}
			return defaultValue;
		}
#endif
		public static Size GetPageSize(PaperKind paperKind) {
			return GetPageSize(paperKind, GraphicsDpi.HundredthsOfAnInch, DefaultSize);
		}
		public static Size GetPageSize(PaperKind paperKind, Size defaultValue) {
			return GetPageSize(paperKind, GraphicsDpi.HundredthsOfAnInch, defaultValue);
		}
		public static Size GetPageSize(PaperKind paperKind, GraphicsUnit targetUnit) {
			return GetPageSize(paperKind, targetUnit, DefaultSize);
		}
		public static Size GetPageSize(PaperKind paperKind, float targetDpi) {
			return GetPageSize(paperKind, targetDpi, DefaultSize);
		}
		public static Size GetPageSize(PaperKind paperKind, GraphicsUnit targetUnit, Size defaultValue) {
			return GetPageSize(paperKind, GraphicsDpi.UnitToDpi(targetUnit), defaultValue);
		}
		public static Size GetPageSize(PaperKind paperKind, float targetDpi, Size defaultValue) {
			return Size.Round(GetPageSizeF(paperKind, targetDpi, defaultValue));
		}
		public static SizeF GetPageSizeF(PaperKind paperKind, float targetDpi) {
			return GetPageSizeF(paperKind, targetDpi, DefaultSize);
		}
		public static SizeF GetPageSizeF(PaperKind paperKind, float targetDpi, Size defaultValue) {
			PageSize result;
			SizeF pageSize = (pageSizes.TryGetValue(paperKind, out result)) ? result.GetPageSize(targetDpi) : defaultValue;
			return pageSize;
		}
		static Dictionary<PaperKind, PageSize> CreatePageSizeDictionary() {
			var pageSizes = new Dictionary<PaperKind, PageSize>();
			try {
				Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Printing.Core.PaperKind.xml");
				XmlDocument doc = new XmlDocument();
				doc.Load(stream);
				stream.Close();
				XmlNodeList nodes = doc.GetElementsByTagName("PaperKind");
				foreach(XmlNode node in nodes) {
					string name = node.ChildNodes[1].InnerXml;
					string width = node.ChildNodes[2].InnerXml;
					string height = node.ChildNodes[3].InnerXml;
					try {
						PaperKind paperKind = (PaperKind)Enum.Parse(typeof(PaperKind), name, false);
						PageSize pageSizeData = CreatePageSizeData(width, height);
						pageSizes.Add(paperKind, pageSizeData);
					} catch(ArgumentException) {
					}
				}
			} catch {
			}
			return pageSizes;
		}
		static PageSize CreatePageSizeData(string width, string height) {
			var widthParts = width.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var heightParts = height.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if(widthParts.Length != 2 || widthParts.Length != 2 || widthParts.Last() != heightParts.Last())
				throw new FormatException("page size");
			var pageSize = new SizeF(float.Parse(widthParts.First(), CultureInfo.InvariantCulture), float.Parse(heightParts.First(), CultureInfo.InvariantCulture));
			var pageUnit = CreatePageUnit(widthParts.Last());
			return new PageSize(pageSize, pageUnit);
		}
		static GraphicsUnit CreatePageUnit(string pageUnit) {
			switch(pageUnit.ToLower()) {
				case "mm":
					return GraphicsUnit.Millimeter;
				case "in":
					return GraphicsUnit.Inch;
				default:
					throw new NotSupportedException("pageUnit");
			}
		}
	}
}
