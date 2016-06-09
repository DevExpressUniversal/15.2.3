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
using System.Drawing;
using System.Collections;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export.Text;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting.Export {
	public class TextLayoutBuilder : LayoutBuilder, ITableExportProvider {
		static string ProcessText(string text, string separator) {
			const string QuotationMark = "\"";
			const string DoubleQuotationMark = "\"\"";
			const string NewLineOneChar = "\n";
			if(text.Contains(separator) || text.Contains(QuotationMark) || text.Contains(Environment.NewLine) || text.Contains(NewLineOneChar))
				return string.Format("{1}{0}{1}", text.Replace(QuotationMark, DoubleQuotationMark), QuotationMark);
			return text;
		}
		static void ProcessTexts(ITextLayoutTable texts, string separator) {
			for(int i = 0; i < texts.Count; i++)
				texts[i] = ProcessText(texts[i], separator);
		}
		ExportContext exportContext;
		protected TextBrickViewData fCurrentData;
		TextExportMode mode;
		string separator;
		bool quoteStringsWithSeparators;
		bool shouldSplitText;
		ExportContext ITableExportProvider.ExportContext { get { return exportContext; } }
		BrickViewData ITableExportProvider.CurrentData { get { return fCurrentData; } }
		public TextLayoutBuilder(Document document, ExportContext exportContext, string separator, bool quoteStringsWithSeparators, TextExportMode mode, bool shouldSplitText)
			: base(document) {
			this.exportContext = exportContext;
			this.separator = separator;
			this.quoteStringsWithSeparators = quoteStringsWithSeparators;
			this.mode = mode;
			this.shouldSplitText = shouldSplitText;
		}
		internal protected string GetText(string text, object textValue) {
			if(mode == TextExportMode.Value && textValue != null)
				return textValue.ToString();
			return text;
		}
		protected override ILayoutControl[] GetBrickLayoutControls(Brick brick, RectangleF rect) {
			BrickExporter exporter = BrickBaseExporter.GetExporter(exportContext, brick) as BrickExporter;
			return ToLayoutControls(exporter.GetTextData(exportContext, rect), brick);
		}
		protected override ILayoutControl CreateLayoutControl(BrickViewData data, Brick brick) {
			fCurrentData = (TextBrickViewData)data;
			((BrickExporter)BrickBaseExporter.GetExporter(brick.PrintingSystem, fCurrentData.TableCell)).FillTextTableCell(this, shouldSplitText);
			ITextLayoutTable texts = fCurrentData.Texts;
			if(texts != null) {
				HotkeyPrefixHelper.PreprocessHotkeyPrefixesInTextLayoutTable(texts, data.Style);
				if(quoteStringsWithSeparators)
					ProcessTexts(texts, separator);
				return data;
			}
			return null;
		}
		void ITableExportProvider.SetCellText(object textValue, string hyperLink) {
			if(object.ReferenceEquals(textValue, string.Empty))
				fCurrentData.Texts = SimpleTextLayoutTable.Empty;
			if(textValue is string[] && ((string[])textValue).Length > 1)
				fCurrentData.Texts = new TextLayoutTable((string[])textValue);
			if(textValue is string)
				fCurrentData.Texts = new SimpleTextLayoutTable((string)textValue);
		}
		void ITableExportProvider.SetCellImage(System.Drawing.Image image, string imageSrc, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds, Size imageSize, PaddingInfo padding, string hyperLink) {
		}
		void ITableExportProvider.SetCellShape(Color lineColor, XtraReports.UI.LineDirection lineDirection, System.Drawing.Drawing2D.DashStyle lineStyle, float lineWidth, PaddingInfo padding, string hyperlink) {
		}
	}
	public class TextBrickViewData : BrickViewData {
		#region inner classes
		public class XComparer : IComparer {
			public static readonly XComparer Instance = new XComparer();
			private XComparer() { 
			}
			public int Compare(object x, object y) {
				TextBrickViewData layoutControl1 = (TextBrickViewData)x;
				TextBrickViewData layoutControl2 = (TextBrickViewData)y;
				return Math.Sign(layoutControl1.Bounds.X - layoutControl2.Bounds.X);
			}
		}
		public class YComparer : IComparer {
			public static readonly YComparer Instance = new YComparer();
			private YComparer() { 
			}
			public int Compare(object x, object y) {
				TextBrickViewData layoutControl1 = (TextBrickViewData)x;
				TextBrickViewData layoutControl2 = (TextBrickViewData)y;
				return Math.Sign(layoutControl1.Bounds.Y - layoutControl2.Bounds.Y);
			}
		}
		#endregion
		ITextLayoutTable texts;
		public ITextLayoutTable Texts { get { return texts; } set { texts = value; } }
		public bool IsCompoundControl { get { return Texts != null && Texts.Count > 1; } }
		public TextBrickViewData(BrickStyle style, RectangleF bounds, ITableCell tableCell)
			: base(style, bounds, tableCell) {
		}
		public TextLayoutItem GetLayoutItem(int row) {
			return new TextLayoutItem(this.Texts[row], this);
		}
	}
}
