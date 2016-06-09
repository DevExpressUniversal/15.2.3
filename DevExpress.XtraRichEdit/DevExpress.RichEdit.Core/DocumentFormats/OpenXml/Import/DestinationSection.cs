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
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing.Printing;
#if !SL
using System.Drawing.Printing;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region SectionDestinationBase (abstract class)
	public abstract class SectionDestinationBase : ElementDestination {
		protected SectionDestinationBase(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected static Destination OnColumns(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ColumnsDestination(importer);
		}
		protected static Destination OnLineNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionLineNumberingDestination(importer);
		}
		protected static Destination OnPageNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionPageNumberingDestination(importer);
		}
		protected static Destination OnPaperSource(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionPaperSourceDestination(importer);
		}
		protected static Destination OnMargins(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionMarginsDestination(importer);
		}
		protected static Destination OnPageSize(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionPageSizeDestination(importer);
		}
		protected static Destination OnTextDirectionOpenXml(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateOpenXmlSectionTextDirectionDestination();
		}
		protected static Destination OnTextDirectionWordML(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateWordMLSectionTextDirectionDestination();
		}
		protected static Destination OnVerticalAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionVerticalAlignmentDestination(importer);
		}
		protected static Destination OnSectionStartType(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionStartTypeDestination(importer);
		}
		protected static Destination OnSectionDifferentFirstPage(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionDifferentFirstPageDestination(importer);
		}
		protected static Destination OnFootNoteProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionLevelFootNotePropertiesDestination(importer, importer.CurrentSection.FootNote);
		}
		protected static Destination OnEndNoteProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SectionLevelEndNotePropertiesDestination(importer, importer.CurrentSection.EndNote);
		}
	}
	#endregion
	#region SectionDestination (abstract class)
	public abstract class SectionDestination : SectionDestinationBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("cols", OnColumns);
			result.Add("lnNumType", OnLineNumbering);
			result.Add("pgNumType", OnPageNumbering);
			result.Add("paperSrc", OnPaperSource);
			result.Add("pgMar", OnMargins);
			result.Add("pgSz", OnPageSize);
			result.Add("textDirection", OnTextDirectionOpenXml);
			result.Add("textFlow", OnTextDirectionWordML);
			result.Add("vAlign", OnVerticalAlignment);
			result.Add("type", OnSectionStartType);
			result.Add("headerReference", OnSectionHeaderReference);
			result.Add("footerReference", OnSectionFooterReference);
			result.Add("titlePg", OnSectionDifferentFirstPage);
			result.Add("footnotePr", OnFootNoteProperties);
			result.Add("endnotePr", OnEndNoteProperties);
			return result;
		}
		protected SectionDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnSectionHeaderReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return new HeaderReferenceDestination(importer);
			return new EmptyDestination(importer);
		}
		static Destination OnSectionFooterReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return new FooterReferenceDestination(importer);
			return new EmptyDestination(importer);
		}
	}
	#endregion
	#region ColumnsDestination
	public class ColumnsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("col", OnColumn);
			return result;
		}
		ColumnInfoCollection columnInfos;
		public ColumnsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected SectionColumns Columns { get { return Importer.CurrentSection.Columns; } }
		static ColumnsDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ColumnsDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Columns.EqualWidthColumns = Importer.GetWpSTOnOffValue(reader, "equalWidth", false);
			Columns.DrawVerticalSeparator = Importer.GetWpSTOnOffValue(reader, "sep", false);
			int columnCount = Importer.GetWpSTIntegerValue(reader, "num", Int32.MinValue);
			if (columnCount > 0)
				Columns.ColumnCount = columnCount;
			int spacing = Importer.GetWpSTIntegerValue(reader, "space", Int32.MinValue);
			if (spacing != Int32.MinValue)
				Columns.Space = UnitConverter.TwipsToModelUnits(spacing);
			if (!Columns.EqualWidthColumns)
				this.columnInfos = Columns.GetColumns();
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (this.columnInfos != null) {
				if (columnInfos.Count > 0)
					Columns.SetColumns(columnInfos);
				else
					Columns.EqualWidthColumns = true;
				this.columnInfos = null;
			}
		}
		static Destination OnColumn(WordProcessingMLBaseImporter importer, XmlReader reader) {
			ColumnInfoCollection columnInfos = GetThis(importer).columnInfos;
			if (columnInfos != null)
				return new ColumnDestination(importer, columnInfos);
			else
				return null;
		}
	}
	#endregion
	#region ColumnDestination
	public class ColumnDestination : LeafElementDestination {
		readonly ColumnInfoCollection columnInfos;
		public ColumnDestination(WordProcessingMLBaseImporter importer, ColumnInfoCollection columnInfos)
			: base(importer) {
			Guard.ArgumentNotNull(columnInfos, "columnInfos");
			this.columnInfos = columnInfos;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ColumnInfo columnInfo = new ColumnInfo();
			columnInfo.Width = Importer.GetWpSTIntegerValue(reader, "w", Int32.MinValue);
			if (columnInfo.Width <= 0)
				return;
			columnInfo.Width = UnitConverter.TwipsToModelUnits(columnInfo.Width);
			columnInfo.Space = Importer.GetWpSTIntegerValue(reader, "space", Int32.MinValue);
			columnInfo.Space = Math.Max(0, UnitConverter.TwipsToModelUnits(columnInfo.Space));
			columnInfos.Add(columnInfo);
		}
	}
	#endregion
	#region SectionLineNumberingDestination
	public class SectionLineNumberingDestination : LeafElementDestination {
		public SectionLineNumberingDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			SectionLineNumbering lineNumbering = Importer.CurrentSection.LineNumbering;
			lineNumbering.StartingLineNumber = Math.Max(1, Importer.GetWpSTIntegerValue(reader, "start", Int32.MinValue));
			WordProcessingMLValue countByAttribute = new WordProcessingMLValue("countBy", "count-by");
			lineNumbering.Step = Math.Max(0, Importer.GetWpSTIntegerValue(reader, Importer.GetWordProcessingMLValue(countByAttribute), Int32.MinValue));
			lineNumbering.Distance = UnitConverter.TwipsToModelUnits(Math.Max(0, Importer.GetWpSTIntegerValue(reader, "distance", Int32.MinValue)));
			lineNumbering.NumberingRestartType = Importer.GetWpEnumValue(reader, "restart", OpenXmlExporter.lineNumberingRestartTable, LineNumberingRestart.NewPage);
		}
	}
	#endregion
	#region SectionPageNumberingDestination
	public class SectionPageNumberingDestination : LeafElementDestination {
		public SectionPageNumberingDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			SectionPageNumbering pageNumbering = Importer.CurrentSection.PageNumbering;
			pageNumbering.StartingPageNumber = Math.Max(0, Importer.GetWpSTIntegerValue(reader, "start", Int32.MinValue));
			pageNumbering.NumberingFormat = Importer.GetWpEnumValue(reader, "fmt", OpenXmlExporter.pageNumberingFormatTable, NumberingFormat.Decimal);
			WordProcessingMLValue chapSepAttribute = new WordProcessingMLValue("chapSep", "chap-sep");
			pageNumbering.ChapterSeparator = Importer.GetWpEnumValue(reader, Importer.GetWordProcessingMLValue(chapSepAttribute), OpenXmlExporter.chapterSeparatorsTable, '.');
		}
	}
	#endregion
	#region SectionPaperSourceDestination
	public class SectionPaperSourceDestination : LeafElementDestination {
		public SectionPaperSourceDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			SectionGeneralSettings settings = Importer.CurrentSection.GeneralSettings;
			settings.FirstPagePaperSource = Math.Max(1, Importer.GetWpSTIntegerValue(reader, "first", Int32.MinValue));
			settings.OtherPagePaperSource = Math.Max(1, Importer.GetWpSTIntegerValue(reader, "other", Int32.MinValue));
		}
	}
	#endregion
	#region SectionMarginsDestination
	public class SectionMarginsDestination : LeafElementDestination {
		public SectionMarginsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value;
			SectionMargins margins = Importer.CurrentSection.Margins;
			value = Importer.GetWpSTIntegerValue(reader, "left", Int32.MinValue);
			if (value != Int32.MinValue)
				margins.Left = UnitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "right", Int32.MinValue);
			if (value != Int32.MinValue)
				margins.Right = UnitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "top", Int32.MinValue);
			if (value != Int32.MinValue)
				margins.Top = UnitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "bottom", Int32.MinValue);
			if (value != Int32.MinValue)
				margins.Bottom = UnitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "header", Int32.MinValue);
			if (value != Int32.MinValue)
				margins.HeaderOffset = UnitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "footer", Int32.MinValue);
			if (value != Int32.MinValue)
				margins.FooterOffset = UnitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "gutter", Int32.MinValue);
			if (value != Int32.MinValue)
				margins.Gutter = UnitConverter.TwipsToModelUnits(value);
		}
	}
	#endregion
	#region SectionPageSizeDestination
	public class SectionPageSizeDestination : LeafElementDestination {
		public SectionPageSizeDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value;
			PageInfo pageInfo = new PageInfo();
			value = Importer.GetWpSTIntegerValue(reader, "w", Int32.MinValue);
			if (value != Int32.MinValue)
				pageInfo.Width = UnitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "h", Int32.MinValue);
			if (value != Int32.MinValue)
				pageInfo.Height = UnitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "code", Int32.MinValue);
			if (value != Int32.MinValue)
				pageInfo.PaperKind = (PaperKind)value;
			string orientation = reader.GetAttribute("orient", Importer.WordProcessingNamespaceConst);
			pageInfo.Landscape = (orientation == "landscape");
			pageInfo.ValidatePaperKind(DocumentModel.UnitConverter);
			Importer.CurrentSection.Page.CopyFrom(pageInfo);
		}
	}
	#endregion
	#region SectionTextDirectionDestination
	public class SectionTextDirectionDestination : LeafElementDestination {
		public SectionTextDirectionDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.CurrentSection.GeneralSettings.TextDirection = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.textDirectionTable, TextDirection.LeftToRightTopToBottom);
		}
	}
	#endregion
	#region SectionVerticalAlignmentDestination
	public class SectionVerticalAlignmentDestination : LeafElementDestination {
		public SectionVerticalAlignmentDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.CurrentSection.GeneralSettings.VerticalTextAlignment = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.verticalAlignmentTable, VerticalAlignment.Top);
		}
	}
	#endregion
	#region SectionStartTypeDestination
	public class SectionStartTypeDestination : LeafElementDestination {
		public SectionStartTypeDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.CurrentSection.GeneralSettings.StartType = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.sectionStartTypeTable, SectionStartType.NextPage);
		}
	}
	#endregion
	#region SectionDifferentFirstPageDestination
	public class SectionDifferentFirstPageDestination : LeafElementDestination {
		public SectionDifferentFirstPageDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.CurrentSection.GeneralSettings.DifferentFirstPage = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
}
