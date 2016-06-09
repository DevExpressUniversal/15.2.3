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

#if OPENDOCUMENT
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Export.OpenDocument;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml;
using DevExpress.Office.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument { 
	#region TableCellPropertiesDestination
	public class TableCellPropertiesDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		static readonly Dictionary<string, XlBorderLineStyle> lineStyleTable = CreateLineStyleTable();
		static readonly Dictionary<string, XlReadingOrder> readingOrderTable = CreateReadingOrderTable();
		static readonly Dictionary<string, XlVerticalAlignment> verticalAlignmentTable = CreateVerticalAlignmentTable();
		static Dictionary<string, XlBorderLineStyle> CreateLineStyleTable() {
			Dictionary<string, XlBorderLineStyle> result = new Dictionary<string, XlBorderLineStyle>();
			result.Add("dashed", XlBorderLineStyle.Dashed);
			result.Add("dotted", XlBorderLineStyle.Dotted);
			result.Add("double", XlBorderLineStyle.Double);
			return result;
		}
		static Dictionary<string, XlReadingOrder> CreateReadingOrderTable() {
			Dictionary<string, XlReadingOrder> result = DictionaryUtils.CreateBackTranslationTable<XlReadingOrder>(OpenDocumentExporter.ReadingOrderTable);
			result.Add("lr-tb",XlReadingOrder.LeftToRight);
			result.Add("rl-tb", XlReadingOrder.RightToLeft);
			result.Add("tb-rl", XlReadingOrder.RightToLeft);
			result.Add("tb-lr", XlReadingOrder.LeftToRight);
			result.Add("tb", XlReadingOrder.Context);
			return result;
		}
		static Dictionary<string, XlVerticalAlignment> CreateVerticalAlignmentTable() {
			Dictionary<string, XlVerticalAlignment> result = DictionaryUtils.CreateBackTranslationTable<XlVerticalAlignment>(OpenDocumentExporter.VerticalAlignmentTable);
			return result;
		}
		#endregion
		#region Types
		delegate void LineStyleSwitcher(XlBorderLineStyle style);
		delegate void LineColorSwitcher(int colorModelInfoIndex);
		#endregion
		#region Fields
		CellFormatBase format;
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		DocumentCache Cache { get { return Importer.DocumentModel.Cache; } }
		public TableCellPropertiesDestination(CellFormatBase format, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.format = format;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int fillInfoIndex = ReadFillInfo(reader);
			format.ApplyFill = FillInfoCache.DefaultItemIndex != fillInfoIndex;
			format.AssignFillIndex(fillInfoIndex);
			int borderInfoIndex = ReadBorderInfo(reader);
			format.ApplyBorder = BorderInfoCache.DefaultItemIndex != borderInfoIndex;
			format.AssignBorderIndex(borderInfoIndex);
			int alignmentInfoIndex = ReadAlignmentInfo(reader);
			format.ApplyAlignment = CellAlignmentInfoCache.DefaultItemIndex != alignmentInfoIndex;
			format.AssignAlignmentIndex(alignmentInfoIndex);
			ReadProtectionInfo(reader);
		}
		#region ReadFill
		int ReadFillInfo(XmlReader reader) {
			FillInfo fillInfo = format.FillInfo.Clone();
			Color color = Importer.GetColor(reader, "fo:background-color", DXColor.Empty);
			if (color != DXColor.Empty) {
				fillInfo.ForeColorIndex = Importer.RegisterColor(color);
				fillInfo.PatternType = XlPatternType.Solid;
			}
			return Cache.FillInfoCache.GetItemIndex(fillInfo);
		}
		#endregion
		#region ReadBorders
		int ReadBorderInfo(XmlReader reader) {
			BorderInfo borderInfo = format.BorderInfo.Clone();
			LineColorSwitcher setColor = (x) => {
				borderInfo.LeftColorIndex = x;
				borderInfo.TopColorIndex = x;
				borderInfo.RightColorIndex = x;
				borderInfo.BottomColorIndex = x;
			};
			LineStyleSwitcher setLineStyle = (x) => {
				borderInfo.LeftLineStyle = x;
				borderInfo.TopLineStyle = x;
				borderInfo.RightLineStyle = x;
				borderInfo.BottomLineStyle = x;
			};
			ReadBorderCore("fo:border", setColor, setLineStyle, reader);
			setColor = (x) => { borderInfo.LeftColorIndex = x; };
			setLineStyle = (x) => { borderInfo.LeftLineStyle = x; };
			ReadBorderCore("fo:border-left", setColor, setLineStyle, reader);
			setColor = (x) => { borderInfo.TopColorIndex = x; };
			setLineStyle = (x) => { borderInfo.TopLineStyle = x; };
			ReadBorderCore("fo:border-top", setColor, setLineStyle, reader);
			setColor = (x) => { borderInfo.RightColorIndex = x; };
			setLineStyle = (x) => { borderInfo.RightLineStyle = x; };
			ReadBorderCore("fo:border-right", setColor, setLineStyle, reader);
			setColor = (x) => { borderInfo.BottomColorIndex = x; };
			setLineStyle = (x) => { borderInfo.BottomLineStyle = x; };
			ReadBorderCore("fo:border-bottom", setColor, setLineStyle, reader);
			setColor = (x) => { borderInfo.DiagonalColorIndex = x; };
			setLineStyle = (x) => { borderInfo.DiagonalUpLineStyle = x; };
			ReadBorderCore("style:diagonal-bl-tr", setColor, setLineStyle, reader);
			setColor = (x) => { borderInfo.DiagonalColorIndex = x; };
			setLineStyle = (x) => { borderInfo.DiagonalDownLineStyle = x; };
			ReadBorderCore("style:diagonal-tl-br", setColor, setLineStyle, reader);
			return Cache.BorderInfoCache.GetItemIndex(borderInfo);
		}
		void ReadBorderCore(string attribute, LineColorSwitcher setColorIndex, LineStyleSwitcher setLineStyle, XmlReader reader) {
			string border = Importer.ReadAttribute(reader, attribute);
			if (string.IsNullOrEmpty(border) || border.Equals("none", System.StringComparison.OrdinalIgnoreCase))
				return;
			string[] borderParams = border.Split(' ');
			if (borderParams.Length != 3)
				return;
			double lineWidth = Importer.ParseLength(borderParams[0], 0);
			string lineType = borderParams[1].ToLowerInvariant();
			Color color = Importer.ParseColor(borderParams[2], DXColor.Empty);
			int modelColorInfoIndex = Importer.RegisterColor(color);
			XlBorderLineStyle lineStyle;
			if (!lineStyleTable.TryGetValue(lineType, out lineStyle))
				lineStyle = XlBorderLineStyle.None;
			switch (lineStyle) {
				case XlBorderLineStyle.Double:
				case XlBorderLineStyle.Dotted:
					break;
				case XlBorderLineStyle.Dashed:
					lineStyle = GetLineStyleByWidth(lineWidth, XlBorderLineStyle.Dashed, XlBorderLineStyle.MediumDashed, XlBorderLineStyle.MediumDashed);
					break;
				default:
					lineStyle = GetLineStyleByWidth(lineWidth, XlBorderLineStyle.Thin, XlBorderLineStyle.Medium, XlBorderLineStyle.Thick);
					break;
			}
			setLineStyle(lineStyle);
			setColorIndex(modelColorInfoIndex);
		}
		XlBorderLineStyle GetLineStyleByWidth(double lineWidth, XlBorderLineStyle ifThin, XlBorderLineStyle ifMedium, XlBorderLineStyle ifThick) {
			lineWidth = Importer.DocumentModel.UnitConverter.ModelUnitsToPointsF((float)lineWidth);
			if (lineWidth <= 1)
				return ifThin;
			if (lineWidth <= 2)
				return ifMedium;
			return ifThick;
		}
		#endregion
		#region ReadAlignment
		int ReadAlignmentInfo(XmlReader reader) {
			CellAlignmentInfo alignmentInfo = format.AlignmentInfo.Clone();
			string wrapOption = Importer.GetAttribute(reader, "fo:wrap-option", "no-wrap");
			if (wrapOption.Equals("wrap", System.StringComparison.OrdinalIgnoreCase))
				alignmentInfo.WrapText = true;
			XlVerticalAlignment alignment;
			string verticalAlignment = Importer.GetAttribute(reader, "style:vertical-align", "automatic");
			if (verticalAlignmentTable.TryGetValue(verticalAlignment, out alignment))
				alignmentInfo.VerticalAlignment = alignment;
			XlReadingOrder order;
			string writingMode = Importer.GetAttribute(reader, "style:writing-mode", "page").ToLowerInvariant();
			if (readingOrderTable.TryGetValue(writingMode, out order))
				alignmentInfo.ReadingOrder = order;
			alignmentInfo.TextRotation = (int)Importer.GetAngle(reader, "style:rotation-angle", alignmentInfo.TextRotation);
			alignmentInfo.ShrinkToFit = Importer.GetBoolean(reader, "style:shrink-to-fit", alignmentInfo.ShrinkToFit);
			if (Importer.GetBoolean(reader, "style:repeat-content", false))
				alignmentInfo.HorizontalAlignment = XlHorizontalAlignment.Fill;
			return Cache.CellAlignmentInfoCache.GetItemIndex(alignmentInfo);
		}
		#endregion
		#region ReadProtection
		void ReadProtectionInfo(XmlReader reader) { 
			string protection = Importer.GetAttribute(reader, "style:cell-protect", "protected").ToLowerInvariant();
			if (!protection.Contains("protected"))
				format.Protection.Locked = false;
			if (protection.Contains("hidden"))
				format.Protection.Hidden = true;
		}
		#endregion
	}
	#endregion
	#region ParagraphPropertiesDestination
	public class ParagraphPropertiesDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		static readonly Dictionary<string, XlHorizontalAlignment> horizontalAlignmentTable = CreateHorizontalAlignmentTable();
		static Dictionary<string, XlHorizontalAlignment> CreateHorizontalAlignmentTable() {
			Dictionary<string, XlHorizontalAlignment> result = new Dictionary<string, XlHorizontalAlignment>();
			result.Add("start", XlHorizontalAlignment.Left);
			result.Add("end", XlHorizontalAlignment.Right);
			result.Add("left", XlHorizontalAlignment.Left);
			result.Add("right", XlHorizontalAlignment.Right);
			result.Add("center", XlHorizontalAlignment.Center);
			result.Add("justify", XlHorizontalAlignment.Justify);
			result.Add("default", XlHorizontalAlignment.General);
			return result;
		}
		#endregion
		#region Fields
		CellFormatBase format;
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		DocumentCache Cache { get { return Importer.DocumentModel.Cache; } }
		public ParagraphPropertiesDestination(CellFormatBase format, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.format = format;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			if (format == null)
				return;
			int alignmentInfoIndex = ReadAlignmentInfo(reader);
			format.ApplyAlignment = CellAlignmentInfoCache.DefaultItemIndex != alignmentInfoIndex;
			format.AssignAlignmentIndex(alignmentInfoIndex);
		}
		#region ReadAlignment
		int ReadAlignmentInfo(XmlReader reader) {
			CellAlignmentInfo alignmentInfo = format.AlignmentInfo.Clone();
			if (format.Alignment.Horizontal != XlHorizontalAlignment.Fill) {
				XlHorizontalAlignment alignment = XlHorizontalAlignment.General;
				string horizontalAlignment = Importer.ReadAttribute(reader, "fo:text-align"); 
				if (!string.IsNullOrEmpty(horizontalAlignment) && horizontalAlignmentTable.TryGetValue(horizontalAlignment, out alignment))
					alignmentInfo.HorizontalAlignment = alignment;
				if (alignment == XlHorizontalAlignment.Left)
					alignmentInfo.Indent = GetIndentInModelUnits(Importer.GetLength(reader, "fo:margin-left", 0));
				else
					alignmentInfo.Indent = GetIndentInModelUnits(Importer.GetLength(reader, "fo:margin-right", 0));
			}
			return Cache.CellAlignmentInfoCache.GetItemIndex(alignmentInfo);
		}
		byte GetIndentInModelUnits(float length) {
			length = Importer.DocumentModel.UnitConverter.ModelUnitsToCentimetersF(length);
			return (byte)Math.Round(length / 0.353);
		}
		#endregion
	}
	#endregion
	#region TextPropertiesDestination
	public class TextPropertiesDestination : LeafElementDestination {
		CellFormatBase format;
		DocumentCache Cache { get { return Importer.DocumentModel.Cache; } }
		public TextPropertiesDestination(CellFormatBase format, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.format = format;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			if (format == null)
				return;
			int fontInfoIndex = ReadFontInfo(reader);
			format.ApplyFont = RunFontInfoCache.DefaultItemIndex != fontInfoIndex;
			format.AssignFontIndex(fontInfoIndex);
		}
		int ReadFontInfo(XmlReader reader) {
			double temp;
			string attribute;
			RunFontInfo fontInfo = format.FontInfo.Clone();
			attribute = Importer.ReadAttribute(reader, "fo:font-weight");
			if (double.TryParse(attribute, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
				fontInfo.Bold = temp > 500;
			else
				fontInfo.Bold = "bold".Equals(attribute);
			fontInfo.ColorIndex = Importer.RegisterColor(Importer.GetColor(reader, "fo:color", DXColor.Empty));
			attribute = Importer.ReadAttribute(reader, "fo:font-style");
			if (!string.IsNullOrEmpty(attribute))
				fontInfo.Italic = "italic".Equals(attribute) || "oblique".Equals(attribute);
			attribute = Importer.ReadAttribute(reader, "style:font-name");
			if (Importer.TryGetFontName(attribute, out attribute)) {
				fontInfo.Name = attribute;
				fontInfo.SchemeStyle = XlFontSchemeStyles.None;
			}
			attribute = Importer.ReadAttribute(reader, "style:text-position");
			if (!string.IsNullOrEmpty(attribute))
				if (double.TryParse(attribute.Substring(0, attribute.Length - 1), NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
					fontInfo.Script = temp > 0 ? XlScriptType.Superscript : temp == 0 ? XlScriptType.Baseline : XlScriptType.Subscript;
				else
					if ("super".Equals(attribute))
						fontInfo.Script = XlScriptType.Superscript;
					else
						if ("sub".Equals(attribute))
							fontInfo.Script = XlScriptType.Subscript;
			attribute = Importer.ReadAttribute(reader, "fo:font-size");
			if (!string.IsNullOrEmpty(attribute) && attribute.Length >= 3)
				if (double.TryParse(attribute.Substring(0, attribute.Length - 2), NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
					fontInfo.Size = temp;
			attribute = Importer.ReadAttribute(reader, "style:text-line-through-style");
			if (!string.IsNullOrEmpty(attribute))
				fontInfo.StrikeThrough = !"none".Equals(attribute);
			attribute = Importer.ReadAttribute(reader, "style:text-underline-type");
			if (!string.IsNullOrEmpty(attribute)) {
				if ("double".Equals(attribute))
					fontInfo.Underline = XlUnderlineType.Double;
				else
					if ("single".Equals(attribute))
						fontInfo.Underline = XlUnderlineType.Single;
			}
			return Cache.FontInfoCache.GetItemIndex(fontInfo);
		}
	}
	#endregion
	#region TableColumnPropertiesDestination
	public class TableColumnPropertiesDestination : LeafElementDestination {
		#region Fields
		OdsColumnFormat format;
		#endregion
		public TableColumnPropertiesDestination(OdsColumnFormat format, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.format = format;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			format.ReadAttributes(reader, Importer);
		}
	}
	#endregion
	#region TableRowPropertiesDestination
	public class TableRowPropertiesDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		#endregion
		#region Fields
		OdsRowFormat format;
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TableRowPropertiesDestination(OdsRowFormat format, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.format = format;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			format.ReadAttributes(reader, Importer);
		}
	}
	#endregion
	#region TablePropertiesDestination
	public class TablePropertiesDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		#endregion
		#region Fields
		OdsTableFormat format;
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TablePropertiesDestination(OdsTableFormat format, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.format = format;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			format.ReadAttributes(reader, Importer);
		}
	}
	#endregion
	#region StyleDestinationBase (abstract class)
	public abstract class StyleDestinationBase : ElementDestination {
		#region Static
		static readonly Dictionary<string, StyleFamilyType> styleFamilyByStringTable = DictionaryUtils.CreateBackTranslationTable<StyleFamilyType>(OpenDocumentExporter.StyleFamilyTypeTable);
		protected static void AddCommonHandlers(ElementHandlerTable table) {
			table.Add("table-cell-properties", OnTableCellProperties);
			table.Add("table-column-properties", OnTableColumnProperties);
			table.Add("table-row-properties", OnTableRowProperties);
			table.Add("table-properties", OnTableProperties);
			table.Add("text-properties", OnTextProperties);
			table.Add("paragraph-properties", OnParagraphProperties);
		}
		static StyleDestinationBase GetThis(OpenDocumentWorkbookImporter importer) {
			return (StyleDestinationBase)importer.PeekDestination();
		}
		#endregion
		StyleFamilyType type;
		protected StyleDestinationBase(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Properties
		protected object Format { get; set; }
		protected StyleFamilyType Type { get { return type; } set { type = value; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string family = reader.GetAttribute("style:family");
			if (string.IsNullOrEmpty(family))
				family = "none";
			family = family.ToLowerInvariant();
			if (!styleFamilyByStringTable.TryGetValue(family, out type))
				type = StyleFamilyType.None;
			CreateStyle();
		}
		public override void ProcessElementClose(XmlReader reader) {
			RegisterStyle();
		}
		protected abstract void CreateStyle();
		protected abstract void RegisterStyle();
		#region Handlers
		static Destination OnTableCellProperties(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StyleDestinationBase destination = GetThis(importer);
			CellFormatBase format = (CellFormatBase)destination.Format;
			return new TableCellPropertiesDestination(format, importer);
		}
		static Destination OnParagraphProperties(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StyleDestinationBase destination = GetThis(importer);
			CellFormatBase format = destination.Format as CellFormatBase;
			return new ParagraphPropertiesDestination(format, importer);
		}
		static Destination OnTextProperties(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StyleDestinationBase destination = GetThis(importer);
			CellFormatBase format = destination.Format as CellFormatBase;
			return new TextPropertiesDestination(format, importer);
		}
		static Destination OnTableRowProperties(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StyleDestinationBase destination = GetThis(importer);
			OdsRowFormat format = (OdsRowFormat)destination.Format;
			return new TableRowPropertiesDestination(format, importer);
		}
		static Destination OnTableColumnProperties(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StyleDestinationBase destination = GetThis(importer);
			OdsColumnFormat format = (OdsColumnFormat)destination.Format;
			return new TableColumnPropertiesDestination(format, importer);
		}
		static Destination OnTableProperties(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StyleDestinationBase destination = GetThis(importer);
			OdsTableFormat format = (OdsTableFormat)destination.Format;
			return new TablePropertiesDestination(format, importer);
		}
		#endregion
	}
	#endregion
	#region DefaultStyleDestination
	public class DefaultStyleDestination : StyleDestinationBase {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public DefaultStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
		}
		protected override void CreateStyle() {
			switch (Type) {
				case StyleFamilyType.TableCell:
					Format = new CellFormat(Importer.DocumentModel);
					break;
				case StyleFamilyType.TableColumn:
					Format = new OdsColumnFormat();
					break;
				case StyleFamilyType.TableRow:
					Format = new OdsRowFormat();
					break;
				case StyleFamilyType.Table:
					Format = new OdsTableFormat();
					break;
				default:
					Type = StyleFamilyType.None;
					break;
			}
		}
		protected override void RegisterStyle() {
			switch (Type) {
				case StyleFamilyType.TableCell:
					Importer.RegisterDefaultCellFormat((CellFormat)Format);
					break;
				case StyleFamilyType.TableColumn:
					Importer.RegisterDefaultColumnFormat((OdsColumnFormat)Format);
					break;
				case StyleFamilyType.TableRow:
					Importer.RegisterDefaultRowFormat((OdsRowFormat)Format);
					break;
				case StyleFamilyType.Table:
					Importer.RegisterDefaultTableFormat((OdsTableFormat)Format);
					break;
				default:
					break;
			}
		}
	}
	#endregion
	#region AutoStyleDestination
	public class AutoStyleDestination : StyleDestinationBase {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			result.Add("map", OnMap);
			return result;
		}
		protected static AutoStyleDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (AutoStyleDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		string name;
		string dataStyleName;
		string parentStyleName;
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected string Name { get { return name; } }
		protected string DataStyleName { get { return dataStyleName; } }
		protected string ParentStyleName { get { return parentStyleName; } }
		public AutoStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			name = reader.GetAttribute("style:name");
			dataStyleName = reader.GetAttribute("style:data-style-name");
			parentStyleName = reader.GetAttribute("style:parent-style-name");
			base.ProcessElementOpen(reader);
		}
		protected override void CreateStyle() {
			switch (Type) {
				case StyleFamilyType.TableCell:
					Format = Importer.CreateCellFormat(parentStyleName);
					break;
				case StyleFamilyType.TableColumn:
					Format = Importer.CreateColumnFormat(parentStyleName);
					break;
				case StyleFamilyType.TableRow:
					Format = Importer.CreateRowFormat(parentStyleName);
					break;
				case StyleFamilyType.Table:
					Format = Importer.CreateTableFormat(parentStyleName);
					break;
				default:
					Type = StyleFamilyType.None;
					break;
			}
		}
		protected override void RegisterStyle() {
			switch (Type) {
				case StyleFamilyType.TableCell:
					Importer.RegisterCellFormat(name, dataStyleName, parentStyleName, (CellFormat)Format);
					break;
				case StyleFamilyType.TableColumn:
					Importer.RegisterColumnFormat(name, (OdsColumnFormat)Format);
					break;
				case StyleFamilyType.TableRow:
					Importer.RegisterRowFormat(name, (OdsRowFormat)Format);
					break;
				case StyleFamilyType.Table:
					Importer.RegisterTableFormat(name, (OdsTableFormat)Format);
					break;
				default:
					break;
			}
		}
		#region Handlers
		static Destination OnMap(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			AutoStyleDestination destination = GetThis(importer);
			string styleName = destination.name;
			CellFormat cellFormat = (CellFormat)destination.Format;
			return new TableCellStyleMapDestination(importer, cellFormat, styleName);
		}
		#endregion
	}
	#endregion
	#region StyleDestination
	public class StyleDestination : AutoStyleDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		string name;
		public StyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			name = reader.GetAttribute("style:display-name");
			if (string.IsNullOrEmpty(name))
				name = Name;
			base.ProcessElementClose(reader);
		}
		protected override void CreateStyle() {
			switch (Type) {
				case StyleFamilyType.TableCell:
					Format = Importer.CreateCellStyleFormat(ParentStyleName);
					break;
				case StyleFamilyType.TableColumn:
					Format = Importer.CreateColumnFormat(ParentStyleName);
					break;
				case StyleFamilyType.TableRow:
					Format = Importer.CreateRowFormat(ParentStyleName);
					break;
				case StyleFamilyType.Table:
					Format = Importer.CreateTableFormat(ParentStyleName);
					break;
				default:
					Type = StyleFamilyType.None;
					break;
			}
		}
		protected override void RegisterStyle() {
			switch (Type) {
				case StyleFamilyType.TableCell:
					Importer.RegisterCellStyle(name, DataStyleName, (CellStyleFormat)Format);
					break;
				case StyleFamilyType.TableColumn:
					Importer.RegisterColumnFormat(name, (OdsColumnFormat)Format);
					break;
				case StyleFamilyType.TableRow:
					Importer.RegisterRowFormat(name, (OdsRowFormat)Format);
					break;
				case StyleFamilyType.Table:
					Importer.RegisterTableFormat(name, (OdsTableFormat)Format);
					break;
				default:
					break;
			}
		}
	}
	#endregion
	#region DataStyleMapDestination
	public class DataStyleMapDestination : LeafElementDestination {
		StringBuilder conditonStringBuilder;
		public DataStyleMapDestination(OpenDocumentWorkbookImporter importer, StringBuilder conditionStringBuilder)
			: base(importer) {
			this.conditonStringBuilder = conditionStringBuilder;
		}
		protected string ApplyStyleName { get; set; }
		protected string Condition { get; set; }
		public override void ProcessElementOpen(XmlReader reader) {
			ApplyStyleName = Importer.ReadAttribute(reader, "style:apply-style-name");
			Condition = Importer.ReadAttribute(reader, "style:condition");
			ParseCondition();
		}
		protected virtual void ParseCondition() {
			if (!Condition.StartsWith("value()", StringComparison.OrdinalIgnoreCase))
				return;
			string conditionFormatString;
			if (!Importer.TryGetFormatString(ApplyStyleName, out conditionFormatString))
				return;
			Condition = Condition.Remove(0, 7);
			Condition = "[" + Condition + "]" + conditionFormatString + ";";
			conditonStringBuilder.Append(Condition);
		}
	}
	#endregion
	#region TableCellStyleMapDestination
	public class TableCellStyleMapDestination : LeafElementDestination {
		CellFormat cellFormat;
		string styleName;
		public TableCellStyleMapDestination(OpenDocumentWorkbookImporter importer, CellFormat cellFormat, string styleName)
			: base(importer) {
			this.styleName = styleName;
			this.cellFormat = cellFormat;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string applyStyleName = Importer.ReadAttribute(reader, "style:apply-style-name");
			string condition = Importer.ReadAttribute(reader, "style:condition");
			string baseCellAddress = reader.GetAttribute("style:base-cell-address");
			Importer.ConditionalFormattings.Register(cellFormat, styleName, applyStyleName, baseCellAddress, condition, Importer);
		}
	}
	#endregion
	#region FontFaceDeclsDestination
	public class FontFaceDeclsDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("font-face", OnFontFace);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public FontFaceDeclsDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnFontFace(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new FontFaceDestination(importer);
		}
		#endregion
	}
	#endregion
	#region FontFaceDestination
	public class FontFaceDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public FontFaceDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.RegisterFontName(Importer.ReadAttribute(reader, "style:name"), Importer.ReadAttribute(reader, "svg:font-family"));
		}
	}
	#endregion
}
#endif
