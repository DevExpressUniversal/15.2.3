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
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Xaml {
	#region XamlImporter
	public class XamlImporter : RichEditDestinationAndXmlBasedImporter, IXamlImporter {
		#region FontWeightTable
		static internal readonly Dictionary<string, DefaultBoolean> fontWeightTable = CreateFontWeightTable();
		static Dictionary<string, DefaultBoolean> CreateFontWeightTable() {
			Dictionary<string, DefaultBoolean> result = new Dictionary<string, DefaultBoolean>();
			result.Add("Thin", DefaultBoolean.False);
			result.Add("Extralight", DefaultBoolean.False);
			result.Add("Ultralight", DefaultBoolean.False);
			result.Add("Light", DefaultBoolean.False);
			result.Add("Normal", DefaultBoolean.False);
			result.Add("Regular", DefaultBoolean.False);
			result.Add("Medium", DefaultBoolean.False);
			result.Add("DemiBold", DefaultBoolean.False);
			result.Add("SemiBold", DefaultBoolean.False);
			result.Add("Bold", DefaultBoolean.True);
			result.Add("ExtraBold", DefaultBoolean.True);
			result.Add("UltraBold", DefaultBoolean.True);
			result.Add("Black", DefaultBoolean.True);
			result.Add("Heavy", DefaultBoolean.True);
			result.Add("ExtraBlack", DefaultBoolean.True);
			result.Add("UltraBlack", DefaultBoolean.True);
			return result;
		}
		#endregion
		#region FontStyleTable
		static internal readonly Dictionary<string, DefaultBoolean> fontStyleTable = CreateFontStyleTable();
		static Dictionary<string, DefaultBoolean> CreateFontStyleTable() {
			Dictionary<string, DefaultBoolean> result = new Dictionary<string, DefaultBoolean>();
			result.Add("Normal", DefaultBoolean.False);
			result.Add("Italic", DefaultBoolean.True);
			result.Add("Oblique", DefaultBoolean.True);
			return result;
		}
		#endregion
		#region TextAlignmentTable
		static internal readonly Dictionary<ParagraphAlignment, string> textAlignmentTable = CreateTextAlignmentTable();
		static Dictionary<ParagraphAlignment, string> CreateTextAlignmentTable() {
			Dictionary<ParagraphAlignment, string> result = new Dictionary<ParagraphAlignment, string>();
			result.Add(ParagraphAlignment.Left, "Left");
			result.Add(ParagraphAlignment.Center, "Center");
			result.Add(ParagraphAlignment.Right, "Right");
			result.Add(ParagraphAlignment.Justify, "Justify");
			return result;
		}
		#endregion
		readonly XamlTablesImportHelper tablesImportHelper;
		public XamlImporter(DocumentModel documentModel, XamlDocumentImporterOptions options)
			: base(documentModel, options) {
			this.tablesImportHelper = new XamlTablesImportHelper(PieceTable, this);
		}
		public XamlTablesImportHelper TablesImportHelper { get { return tablesImportHelper; } }
		public override string RelationsNamespace { get { return String.Empty; } }
		public override string DocumentRootFolder { get; set; }
		public override DevExpress.Office.OpenXmlRelationCollection DocumentRelations { get { return null; } }
		protected override void PrepareOfficeTheme() {
		}
		public void Import(Stream stream) {
			XmlReader reader = ReadToRootElement(stream);
			if (reader == null)
				return;
			ImportMainDocument(reader, stream);
		}
		protected internal virtual XmlReader ReadToRootElement(Stream stream) {
			long position = stream.Position;
			XmlReader reader = CreateXmlReader(stream);
			if (ReadToRootElement(reader, "FlowDocument"))
				return reader;
			stream.Seek(position, SeekOrigin.Begin);
			reader = CreateXmlReader(stream);
			if (ReadToRootElement(reader, "Section"))
				return reader;
			return null;
		}
		protected override Destination CreateMainDocumentDestination() {
			return new SectionDestination(this);
		}
		public override void ThrowInvalidFile() {
			throw new Exception("Invalid Xaml file");
		}
		public override string ReadAttribute(XmlReader reader, string attributeName) {
			return reader.GetAttribute(attributeName);
		}
		public override string ReadAttribute(XmlReader reader, string attributeName, string ns) {
			return reader.GetAttribute(attributeName, ns);
		}
		protected internal Color GetBrushColorValue(XmlReader reader, string attributeName) {
			return GetBrushColorValue(reader, attributeName, DXColor.Empty);
		}
		protected internal Color GetBrushColorValue(XmlReader reader, string attributeName, Color defaultValue) {
			string value = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			if (value.Trim().StartsWith("#"))
				return ParseColor(value, defaultValue);
			else
				return GetWpEnumValue(reader, attributeName, DXColor.PredefinedColors, defaultValue);
		}
		protected internal virtual Color ParseColor(string value, Color defaultValue) {
			if (value.Length <= 1)
				return defaultValue;
			int argb;
			if (!Int32.TryParse(value.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out argb))
				return defaultValue;
			if (argb <= 0xFFFFFF)
				argb = (int)((uint)argb | 0xFF000000);
			return DXColor.FromArgb(argb);
		}
		public override bool ConvertToBool(string value) {
			if (value == "True")
				return true;
			if (value == "False")
				return false;
			ThrowInvalidFile();
			return false;
		}
		protected internal int ParseMetricIntegerToModelUnits(string str) {
			UnitValueParser parser = new UnitValueParser();
			DXUnit originalValue = parser.TryParse(str);
			UnitConverter converter = new UnitConverter(UnitConverter);
			return converter.ToModelUnits(originalValue);
		}
		protected internal MarginsInfo ReadThickness(XmlReader reader, string attributeName) {
			string str = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(str))
				return new MarginsInfo();
			string[] parts = str.Split(',');
			switch (parts.Length) {
				case 1:
					return ParseThickness(parts[0], parts[0], parts[0], parts[0]);
				case 2:
					return ParseThickness(parts[0], parts[1], parts[0], parts[1]);
				case 4:
					return ParseThickness(parts[0], parts[1], parts[2], parts[3]);
				default:
					return new MarginsInfo();
			}
		}
		protected internal MarginsInfo ParseThickness(string left, string top, string right, string bottom) {
			MarginsInfo result = new MarginsInfo();
			result.Left = ParseMetricIntegerToModelUnits(left);
			result.Top = ParseMetricIntegerToModelUnits(top);
			result.Right = ParseMetricIntegerToModelUnits(right);
			result.Bottom = ParseMetricIntegerToModelUnits(bottom);
			return result;
		}
	}
	#endregion
}
