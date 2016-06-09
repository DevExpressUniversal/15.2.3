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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region FontsDestination
	public class FontsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("font", OnFont);
			return result;
		}
		static Destination OnFont(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FontDestination(importer, new ImportFontInfo());
		}
		#endregion
		public FontsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region RunPropertiesDestination
	public class RunPropertiesDestination : FontDestinationBase {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("b", OnBold);
			result.Add("charset", OnCharset);
			result.Add("color", OnColor);
			result.Add("condense", OnCondense);
			result.Add("extend", OnExtend);
			result.Add("family", OnFontFamily);
			result.Add("i", OnItalic);
			result.Add("name", OnFontName);
			result.Add("outline", OnOutline);
			result.Add("scheme", OnScheme);
			result.Add("shadow", OnShadow);
			result.Add("strike", OnStrikeThrough);
			result.Add("sz", OnFontSize);
			result.Add("u", OnUnderline);
			result.Add("vertAlign", OnVerticalAlignment);
			result.Add("rFont", OnRunFontName);
			return result;
		}
		static RunPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (RunPropertiesDestination)importer.PeekDestination();
		}
		static Destination OnRunFontName(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FontNameDestination(importer, GetThis(importer).ImportFontInfo);
		}
		#endregion
		public RunPropertiesDestination(SpreadsheetMLBaseImporter importer, RunFontInfo fontInfo)
			: base(importer, new ImportFontInfo()) {
			Guard.ArgumentNotNull(fontInfo, "fontInfo");
			FontInfo = fontInfo;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected override RunFontInfo GetFontInfo() {
			return FontInfo;
		}
	}
	#endregion
	#region FontDestinationBase (abstract class)
	public abstract class FontDestinationBase : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("b", OnBold);
			result.Add("charset", OnCharset);
			result.Add("color", OnColor);
			result.Add("condense", OnCondense);
			result.Add("extend", OnExtend);
			result.Add("family", OnFontFamily);
			result.Add("i", OnItalic);
			result.Add("name", OnFontName);
			result.Add("outline", OnOutline);
			result.Add("scheme", OnScheme);
			result.Add("shadow", OnShadow);
			result.Add("strike", OnStrikeThrough);
			result.Add("sz", OnFontSize);
			result.Add("u", OnUnderline);
			result.Add("vertAlign", OnVerticalAlignment);
			return result;
		}
		static FontDestinationBase GetThis(SpreadsheetMLBaseImporter importer) {
			return (FontDestinationBase)importer.PeekDestination();
		}
		protected static Destination OnBold(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BoldDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnCharset(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CharsetDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			FontDestinationBase destination = GetThis(importer);
			destination.ImportFontInfo.ColorIndex = ColorModelInfo.DefaultColorIndex;
			return new ColorDestination(importer, destination.colorInfo);
		}
		protected static Destination OnCondense(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CondenseDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnExtend(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExtendDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnFontFamily(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FontFamilyDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnItalic(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ItalicDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnFontName(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FontNameDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnOutline(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FontOutlineDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnScheme(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SchemeDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnShadow(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShadowDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnStrikeThrough(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new StrikeThroughDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnFontSize(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FontSizeDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnUnderline(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new UnderlineDestination(importer, GetThis(importer).ImportFontInfo);
		}
		protected static Destination OnVerticalAlignment(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new VerticalAlignmentDestination(importer, GetThis(importer).ImportFontInfo);
		}
		#endregion
		RunFontInfo fontInfo;
		readonly ImportFontInfo importFontInfo;
		readonly ColorModelInfo colorInfo;
		protected FontDestinationBase(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer) {
			this.importFontInfo = importFontInfo;
			this.colorInfo = new ColorModelInfo();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public ImportFontInfo ImportFontInfo { get { return importFontInfo; } }
		public RunFontInfo FontInfo { get { return fontInfo; } set { fontInfo = value; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			FontInfo = GetFontInfo();
			CalculateFontInfoPropertiesWithoutColorIndex();
			if (importFontInfo.ColorIndex.HasValue) {
				importFontInfo.ColorIndex = Importer.DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(colorInfo);
				FontInfo.ColorIndex = importFontInfo.ColorIndex.Value;
			} 
		}
		void CalculateFontInfoPropertiesWithoutColorIndex() {
			if (ImportFontInfo.Bold.HasValue)
				FontInfo.Bold = ImportFontInfo.Bold.Value;
			if (ImportFontInfo.Condense.HasValue)
				FontInfo.Condense = ImportFontInfo.Condense.Value;
			if (ImportFontInfo.Extend.HasValue)
				FontInfo.Extend = ImportFontInfo.Extend.Value;
			if (ImportFontInfo.Italic.HasValue)
				FontInfo.Italic = ImportFontInfo.Italic.Value;
			if (ImportFontInfo.Outline.HasValue)
				FontInfo.Outline = ImportFontInfo.Outline.Value;
			if (ImportFontInfo.Shadow.HasValue)
				FontInfo.Shadow = ImportFontInfo.Shadow.Value;
			if (ImportFontInfo.StrikeThrough.HasValue)
				FontInfo.StrikeThrough = ImportFontInfo.StrikeThrough.Value;
			if (ImportFontInfo.SchemeStyle.HasValue)
				FontInfo.SchemeStyle = ImportFontInfo.SchemeStyle.Value;
			else
				FontInfo.SchemeStyle = XlFontSchemeStyles.None;
			if (ImportFontInfo.Script.HasValue)
				FontInfo.Script = ImportFontInfo.Script.Value;
			if (ImportFontInfo.Underline.HasValue)
				FontInfo.Underline = ImportFontInfo.Underline.Value;
			if (ImportFontInfo.Charset.HasValue)
				FontInfo.Charset = ImportFontInfo.Charset.Value;
			if (ImportFontInfo.FontFamily.HasValue)
				FontInfo.FontFamily = ImportFontInfo.FontFamily.Value;
			if (ImportFontInfo.Size.HasValue)
				FontInfo.Size = ImportFontInfo.Size.Value;
			if (!String.IsNullOrEmpty(ImportFontInfo.Name))
				FontInfo.Name = ImportFontInfo.Name;
		}
		protected virtual RunFontInfo GetFontInfo() {
			return Importer.DocumentModel.GetDefaultRunFontInfoZeroItemInCache().Clone();
		}
	}
	#endregion
	#region FontDestination
	public class FontDestination : FontDestinationBase {
		public FontDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (!ImportFontInfo.ColorIndex.HasValue)
				FontInfo.ColorIndex = Importer.DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(ColorModelInfo.CreateAutomatic());
			Importer.StyleSheet.RegisterFont(FontInfo);
		}
	}
	#endregion
	#region FontPropertiesLeafElementDestination (abstract class)
	public abstract class FontPropertiesLeafElementDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly ImportFontInfo importFontInfo;
		protected FontPropertiesLeafElementDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer) {
			Guard.ArgumentNotNull(importFontInfo, "importFontInfo");
			this.importFontInfo = importFontInfo;
		}
		public ImportFontInfo ImportFontInfo { get { return importFontInfo; } }
	}
	#endregion
	#region BoldDestination
	public class BoldDestination : FontPropertiesLeafElementDestination {
		public BoldDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.Bold = Importer.GetOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region CharsetDestination
	public class CharsetDestination : FontPropertiesLeafElementDestination {
		public CharsetDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int charset = Importer.GetWpSTIntegerValue(reader, "val", Int32.MinValue);
			if (charset < 0 || charset > 255)
				Importer.ThrowInvalidFile("Chartset out of range 0...255");
			ImportFontInfo.Charset = charset;
		}
	}
	#endregion
	#region CondenseDestination
	public class CondenseDestination : FontPropertiesLeafElementDestination {
		public CondenseDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.Condense = Importer.GetOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region ExtendDestination
	public class ExtendDestination : FontPropertiesLeafElementDestination {
		public ExtendDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.Extend = Importer.GetOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region FontFamilyDestination
	public class FontFamilyDestination : FontPropertiesLeafElementDestination {
		public FontFamilyDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int fontFamily = Importer.GetWpSTIntegerValue(reader, "val", 2);
			if (fontFamily < 0 || fontFamily > 255)
				Importer.ThrowInvalidFile("FontFamily out of range 0..255");
			if(fontFamily > 5) {
				int family = (fontFamily & 0xf0) >> 4;
				int pitch = fontFamily & 0x0f;
				if((family >= 1 && family <= 5) && (pitch >= 0 && pitch <= 2))
					fontFamily = family;
			}
			ImportFontInfo.FontFamily = fontFamily;
		}
	}
	#endregion
	#region ItalicDestination
	public class ItalicDestination : FontPropertiesLeafElementDestination {
		public ItalicDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.Italic = Importer.GetOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region FontNameDestination
	public class FontNameDestination : FontPropertiesLeafElementDestination {
		public FontNameDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string fontName = Importer.ReadAttribute(reader, "val");
			ImportFontInfo.Name = String.IsNullOrEmpty(fontName) ? "Calibri" : fontName;
		}
	}
	#endregion
	#region OutlineDestination
	public class FontOutlineDestination : FontPropertiesLeafElementDestination {
		public FontOutlineDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.Outline = Importer.GetOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region SchemeDestination
	public class SchemeDestination : FontPropertiesLeafElementDestination {
		static Dictionary<XlFontSchemeStyles, string> fontSchemeStyleTable = CreateFontSchemeStyleTable();
		static Dictionary<XlFontSchemeStyles, string> CreateFontSchemeStyleTable() {
			Dictionary<XlFontSchemeStyles, string> result = new Dictionary<XlFontSchemeStyles, string>();
			result.Add(XlFontSchemeStyles.None, "none");
			result.Add(XlFontSchemeStyles.Minor, "minor");
			result.Add(XlFontSchemeStyles.Major, "major");
			return result;
		}
		public SchemeDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.SchemeStyle = Importer.GetWpEnumValue<XlFontSchemeStyles>(reader, "val", fontSchemeStyleTable, XlFontSchemeStyles.None);
		}
	}
	#endregion
	#region ShadowDestination
	public class ShadowDestination : FontPropertiesLeafElementDestination {
		public ShadowDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.Shadow = Importer.GetOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region StrikeThroughDestination
	public class StrikeThroughDestination : FontPropertiesLeafElementDestination {
		public StrikeThroughDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.StrikeThrough = Importer.GetOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region FontSizeDestination
	public class FontSizeDestination : FontPropertiesLeafElementDestination {
		public FontSizeDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			double fontSize = Importer.GetWpDoubleValue(reader, "val", 11.0d);	 
			if (fontSize > 409.55)
				fontSize = 409.55;
			ImportFontInfo.Size = fontSize;
		}
	}
	#endregion
	#region UnderlineDestination
	public class UnderlineDestination : FontPropertiesLeafElementDestination {
		static Dictionary<XlUnderlineType, string> underlineTypeTable = CreateUnderlineTypeTable();
		static Dictionary<XlUnderlineType, string> CreateUnderlineTypeTable() {
			Dictionary<XlUnderlineType, string> result = new Dictionary<XlUnderlineType, string>();
			result.Add(XlUnderlineType.Single, "single");
			result.Add(XlUnderlineType.Double, "double");
			result.Add(XlUnderlineType.SingleAccounting, "singleAccounting");
			result.Add(XlUnderlineType.DoubleAccounting, "doubleAccounting");
			result.Add(XlUnderlineType.None, "none");
			return result;
		}
		public UnderlineDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.Underline = Importer.GetWpEnumValue<XlUnderlineType>(reader, "val", underlineTypeTable, XlUnderlineType.Single);
		}
	}
	#endregion
	#region VerticalAlignment
	public class VerticalAlignmentDestination : FontPropertiesLeafElementDestination {
		#region Static Members
		readonly static Dictionary<XlScriptType, string> verticalAlignmentRunTypeTable = CreateVerticalAlignmentRunTypeTable();
		static Dictionary<XlScriptType, string> CreateVerticalAlignmentRunTypeTable() {
			Dictionary<XlScriptType, string> result = new Dictionary<XlScriptType, string>();
			result.Add(XlScriptType.Baseline, "baseline");
			result.Add(XlScriptType.Subscript, "subscript");
			result.Add(XlScriptType.Superscript, "superscript");
			return result;
		}
		#endregion
		public VerticalAlignmentDestination(SpreadsheetMLBaseImporter importer, ImportFontInfo importFontInfo)
			: base(importer, importFontInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportFontInfo.Script = Importer.GetWpEnumValue<XlScriptType>(reader, "val", verticalAlignmentRunTypeTable, XlScriptType.Baseline);
		}
	}
	#endregion
	#region DifferentialFontDestination
	public class DifferentialFontDestination : FontDestinationBase {
		readonly DifferentialFormat differentialFormat;
		public DifferentialFontDestination(SpreadsheetMLBaseImporter importer, DifferentialFormat differentialFormat)
			: base(importer, new ImportFontInfo()) {
			this.differentialFormat = differentialFormat;
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			int formatIndex = Importer.DocumentModel.Cache.FontInfoCache.AddItem(FontInfo);
			differentialFormat.AssignFontIndex(formatIndex);
			CalculateDifferentialFormatFontOptions(); 
		}
		void CalculateDifferentialFormatFontOptions() {
			MultiOptionsInfo info = differentialFormat.MultiOptionsInfo.Clone();
			info.ApplyFontBold = ImportFontInfo.Bold.HasValue;
			info.ApplyFontCondense = ImportFontInfo.Condense.HasValue;
			info.ApplyFontExtend = ImportFontInfo.Extend.HasValue;
			info.ApplyFontItalic = ImportFontInfo.Italic.HasValue;
			info.ApplyFontOutline = ImportFontInfo.Outline.HasValue;
			info.ApplyFontShadow = ImportFontInfo.Shadow.HasValue;
			info.ApplyFontStrikeThrough = ImportFontInfo.StrikeThrough.HasValue;
			info.ApplyFontSchemeStyle = ImportFontInfo.SchemeStyle.HasValue;
			info.ApplyFontScript = ImportFontInfo.Script.HasValue;
			info.ApplyFontUnderline = ImportFontInfo.Underline.HasValue;
			info.ApplyFontCharset = ImportFontInfo.Charset.HasValue;
			info.ApplyFontFamily = ImportFontInfo.FontFamily.HasValue;
			info.ApplyFontSize = ImportFontInfo.Size.HasValue;
			info.ApplyFontColor = ImportFontInfo.ColorIndex.HasValue;
			info.ApplyFontName = !String.IsNullOrEmpty(ImportFontInfo.Name);
			differentialFormat.AssignMultiOptionsIndex(info.PackedValues);
		}
	}
	#endregion
}
