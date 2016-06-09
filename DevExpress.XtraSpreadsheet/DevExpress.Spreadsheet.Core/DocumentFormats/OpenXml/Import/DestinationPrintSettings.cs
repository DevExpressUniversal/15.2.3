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
using System.Xml;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing.Printing;
#if !SL
using System.Drawing.Printing;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PrintSettingsDestination
	public class PrintSettingsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("headerFooter", OnHeaderFooter);
			result.Add("pageMargins", OnPageMargins);
			result.Add("pageSetup", OnPageSetup);
			return result;
		}
		static PrintSettingsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PrintSettingsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PrintSettings printSettings;
		public PrintSettingsDestination(SpreadsheetMLBaseImporter importer, PrintSettings printSettings)
			: base(importer) {
			this.printSettings = printSettings;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnHeaderFooter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new HeaderFooterDestination(importer, GetThis(importer).printSettings.HeaderFooter);
		}
		static Destination OnPageMargins(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PrintSettingsMarginsDestination(importer, GetThis(importer).printSettings.PageMargins);
		}
		static Destination OnPageSetup(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PageSetupDestination(importer, GetThis(importer).printSettings.PageSetup);
		}
		#endregion
	}
	#endregion
	#region MarginsDestinationBase (abstract class)
	public abstract class MarginsDestinationBase : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly Model.Margins margins;
		protected MarginsDestinationBase(SpreadsheetMLBaseImporter importer, Model.Margins margins)
			: base(importer) {
			this.margins = margins;
		}
		protected virtual string LeftTagName { get { return "left"; } }
		protected virtual string RightTagName { get { return "right"; } }
		protected virtual string TopTagName { get { return "top"; } }
		protected virtual string BottomTagName { get { return "bottom"; } }
		protected virtual string HeaderTagName { get { return "header"; } }
		protected virtual string FooterTagName { get { return "footer"; } }
		public override void ProcessElementOpen(XmlReader reader) {
			margins.BeginUpdate();
			try {
				margins.Left = ReadRequiredMarginValue(reader, LeftTagName);
				margins.Top = ReadRequiredMarginValue(reader, TopTagName);
				margins.Right = ReadRequiredMarginValue(reader, RightTagName);
				margins.Bottom = ReadRequiredMarginValue(reader, BottomTagName);
				margins.Header = ReadRequiredMarginValue(reader, HeaderTagName);
				margins.Footer = ReadRequiredMarginValue(reader, FooterTagName);
			}
			finally {
				margins.EndUpdate();
			}
		}
		internal static int InchesToModelUnits(SpreadsheetMLBaseImporter importer, float value) {
			return (int)Math.Round(importer.DocumentModel.UnitConverter.InchesToModelUnitsF(value));
		}
		int ReadRequiredMarginValue(XmlReader reader, string name) {
			float value = Importer.GetWpSTFloatValue(reader, name, float.MinValue);
			if (value == float.MinValue)
				Importer.ThrowInvalidFile("Required margin value is not specified");
			return InchesToModelUnits(Importer, value);
		}
	}
	#endregion
	#region SheetMarginsDestination (3.3.1.60)
	public class SheetMarginsDestination : MarginsDestinationBase {
		public SheetMarginsDestination(SpreadsheetMLBaseImporter importer, Model.Margins margins)
			: base(importer, margins) {
		}
	}
	#endregion
	#region PrintSettingsMarginsDestination (5.7.2.134)
	public class PrintSettingsMarginsDestination : MarginsDestinationBase {
		public PrintSettingsMarginsDestination(SpreadsheetMLBaseImporter importer, Model.Margins margins)
			: base(importer, margins) {
		}
		protected override string LeftTagName { get { return "l"; } }
		protected override string RightTagName { get { return "r"; } }
		protected override string TopTagName { get { return "t"; } }
		protected override string BottomTagName { get { return "b"; } }
	}
	#endregion
	#region PageSetupDestination
	public class PageSetupDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PrintSetup printSetup;
		static readonly Dictionary<string, ModelCommentsPrintMode> commentsPrintModeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.commentsPrintModeTable);
		static readonly Dictionary<string, ModelErrorsPrintMode> errorsPrintModeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.errorsPrintModeTable);
		static readonly Dictionary<string, PagePrintOrder> pagePrintOrderTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.pagePrintOrderTable);
		static readonly Dictionary<string, ModelPageOrientation> pageOrientationTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.pageOrientationTable);
		public PageSetupDestination(SpreadsheetMLBaseImporter importer, PrintSetup printSetup)
			: base(importer) {
			Guard.ArgumentNotNull(printSetup, "printSetup");
			this.printSetup = printSetup;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			printSetup.BeginUpdate();
			try {
				ReadPageSetupProperties(reader);
			}
			finally {
				printSetup.EndUpdate();
			}
		}
		void ReadPageSetupProperties(XmlReader reader) {
			PaperKind paperKind = (PaperKind)Importer.GetWpSTIntegerValue(reader, "paperSize", -1);
			if (paperKind != (PaperKind)(-1))
				printSetup.PaperKind = paperKind;
			printSetup.CommentsPrintMode = Importer.GetWpEnumValue(reader, "cellComments", commentsPrintModeTable, ModelCommentsPrintMode.None);
			printSetup.ErrorsPrintMode = Importer.GetWpEnumValue(reader, "errors", errorsPrintModeTable, ModelErrorsPrintMode.Displayed);
			printSetup.PagePrintOrder = Importer.GetWpEnumValue(reader, "pageOrder", pagePrintOrderTable, PagePrintOrder.DownThenOver);
			printSetup.Orientation = Importer.GetWpEnumValue(reader, "orientation", pageOrientationTable, ModelPageOrientation.Default);
			printSetup.Scale = Math.Min(400, Math.Max(10, Importer.GetWpSTIntegerValue(reader, "scale", 100)));
			printSetup.BlackAndWhite = Importer.GetWpSTOnOffValue(reader, "blackAndWhite", false);
			printSetup.Draft = Importer.GetWpSTOnOffValue(reader, "draft", false);
			printSetup.UseFirstPageNumber = Importer.GetWpSTOnOffValue(reader, "useFirstPageNumber", false);
			printSetup.UsePrinterDefaults = Importer.GetWpSTOnOffValue(reader, "usePrinterDefaults", true);
			printSetup.Copies = Math.Max(1, Importer.GetWpSTIntegerValue(reader, "copies", 1));
			printSetup.FirstPageNumber = Math.Max(1, Importer.GetWpSTIntegerValue(reader, "firstPageNumber", 1));
			printSetup.FitToWidth = Math.Max(0, Importer.GetWpSTIntegerValue(reader, "fitToWidth", 1));
			printSetup.FitToHeight = Math.Max(0, Importer.GetWpSTIntegerValue(reader, "fitToHeight", 1));
			int horizontalDpi = Importer.GetWpSTIntegerValue(reader, "horizontalDpi", 600);
			if (horizontalDpi <= 0)
				horizontalDpi = 600;
			int verticalDpi = Importer.GetWpSTIntegerValue(reader, "verticalDpi", 600);
			if (verticalDpi <= 0)
				verticalDpi = 600;
			printSetup.HorizontalDpi = horizontalDpi;
			printSetup.VerticalDpi = verticalDpi;
		}
	}
	#endregion
	#region SheetPageSetupPropertiesDestination
	public class SheetPageSetupPropertiesDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PrintSetup printSetup;
		public SheetPageSetupPropertiesDestination(SpreadsheetMLBaseImporter importer, PrintSetup printSetup)
			: base(importer) {
			Guard.ArgumentNotNull(printSetup, "printSetup");
			this.printSetup = printSetup;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			printSetup.BeginUpdate();
			try {
				ReadPageSetupProperties(reader);
			}
			finally {
				printSetup.EndUpdate();
			}
		}
		void ReadPageSetupProperties(XmlReader reader) {
			printSetup.AutoPageBreaks = Importer.GetWpSTOnOffValue(reader, "autoPageBreaks", true);
			printSetup.FitToPage = Importer.GetWpSTOnOffValue(reader, "fitToPage", false);
		}
	}
	#endregion
	#region SheetOutlinePropertiesDestination
	public class SheetOutlinePropertiesDestination :LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly GroupAndOutlineProperties outlineProperties;
		public SheetOutlinePropertiesDestination(SpreadsheetMLBaseImporter importer, GroupAndOutlineProperties outlineProperties)
			: base(importer) {
			Guard.ArgumentNotNull(outlineProperties, "outlineProperties");
			this.outlineProperties = outlineProperties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			outlineProperties.BeginUpdate();
			try {
				ReadOutlineProperties(reader);
			}
			finally {
				outlineProperties.EndUpdate();
			}
		}
		void ReadOutlineProperties(XmlReader reader) {
			outlineProperties.ApplyStyles = Importer.GetOnOffValue(reader, "applyStyles", false);
			outlineProperties.ShowColumnSumsRight =  Importer.GetOnOffValue(reader, "summaryRight", true);
			outlineProperties.ShowRowSumsBelow = Importer.GetOnOffValue(reader, "summaryBelow", true);
		}
	}
	#endregion
	#region PrintOptionsDestination
	public class PrintOptionsDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PrintSetup printSetup;
		public PrintOptionsDestination(SpreadsheetMLBaseImporter importer, PrintSetup printSetup)
			: base(importer) {
			Guard.ArgumentNotNull(printSetup, "printSetup");
			this.printSetup = printSetup;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			printSetup.BeginUpdate();
			try {
				ReadPageSetupProperties(reader);
			}
			finally {
				printSetup.EndUpdate();
			}
		}
		void ReadPageSetupProperties(XmlReader reader) {
			printSetup.PrintGridLines = Importer.GetWpSTOnOffValue(reader, "gridLines", false);
			printSetup.PrintGridLinesSet = Importer.GetWpSTOnOffValue(reader, "gridLinesSet", true);
			printSetup.PrintHeadings = Importer.GetWpSTOnOffValue(reader, "headings", false);
			printSetup.CenterHorizontally = Importer.GetWpSTOnOffValue(reader, "horizontalCentered", false);
			printSetup.CenterVertically = Importer.GetWpSTOnOffValue(reader, "verticalCentered", false);
		}
	}
	#endregion
	#region HeaderFooterDestination
	public class HeaderFooterDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("evenHeader", OnEvenHeader);
			result.Add("evenFooter", OnEvenFooter);
			result.Add("firstHeader", OnFirstHeader);
			result.Add("firstFooter", OnFirstFooter);
			result.Add("oddHeader", OnOddHeader);
			result.Add("oddFooter", OnOddFooter);
			return result;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811")]
		static HeaderFooterDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (HeaderFooterDestination)importer.PeekDestination();
		}
		#endregion
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823")]
		readonly HeaderFooterOptions options;
		public HeaderFooterDestination(SpreadsheetMLBaseImporter importer, HeaderFooterOptions options)
			: base(importer) {
			this.options = options;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnEvenHeader(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			HeaderFooterDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.options.EvenHeader = value; return true; });
		}
		static Destination OnEvenFooter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			HeaderFooterDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.options.EvenFooter = value; return true; });
		}
		static Destination OnFirstHeader(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			HeaderFooterDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.options.FirstHeader = value; return true; });
		}
		static Destination OnFirstFooter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			HeaderFooterDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.options.FirstFooter = value; return true; });
		}
		static Destination OnOddHeader(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			HeaderFooterDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.options.OddHeader = value; return true; });
		}
		static Destination OnOddFooter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			HeaderFooterDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.options.OddFooter = value; return true; });
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			this.options.BeginUpdate();
			this.options.AlignWithMargins = Importer.GetWpSTOnOffValue(reader, "alignWithMargins", true);
			this.options.DifferentFirst = Importer.GetWpSTOnOffValue(reader, "differentFirst", false);
			this.options.DifferentOddEven = Importer.GetWpSTOnOffValue(reader, "differentOddEven", false);
			this.options.ScaleWithDoc = Importer.GetWpSTOnOffValue(reader, "scaleWithDoc", true);
		}
		public override void ProcessElementClose(XmlReader reader) {
			this.options.EndUpdate();
		}
	}
	#endregion
}
