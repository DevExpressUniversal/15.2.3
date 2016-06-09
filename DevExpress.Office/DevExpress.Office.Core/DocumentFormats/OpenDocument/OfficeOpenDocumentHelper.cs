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
using System.Text;
using System.Collections.Generic;
using System.Xml;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office {
	#region OfficeParagraphAlignment
	public enum OfficeParagraphAlignment {
		Left = 0,
		Right = 1,
		Center = 2,
		Justify = 3
	}
	#endregion
	#region OpenDocumentParagraphBreakType
	public enum OpenDocumentParagraphBreakType { None, Page, Column }
	#endregion
	#region TextDirection
	public enum TextDirection {
		LeftToRightTopToBottom = 0,		
		TopToBottomRightToLeft = 1,		
		TopToBottomLeftToRightRotated = 2, 
		BottomToTopLeftToRight = 3,		
		LeftToRightTopToBottomRotated = 4, 
		TopToBottomRightToLeftRotated = 5  
	}
	#endregion
	public enum OpenDocumentTableAlignment {
		Both,
		Center,
		Distribute,
		Left,
		NumTab, 
		Right
	}
	#region OpenDocumentVerticalAlignment
	public enum OpenDocumentVerticalAlignment {
		Top,
		Both,
		Center,
		Bottom
	}
	#endregion
	#region OfficeOpenDocumentHelper
	public class OfficeOpenDocumentHelper {
		const string OpenDocumentVersion = "1.0";
		#region Constants
		#region NameSpacePrefix
		public const string DrawNsPrefix = "draw";
		public const string FoNsPrefix = "fo";
		public const string ManifestNsPrefix = "manifest";
		public const string OfficeNsPrefix = "office";
		public const string StyleNsPrefix = "style";
		public const string SvgNsPrefix = "svg";
		public const string TextNsPrefix = "text";
		public const string XlinkNsPrefix = "xlink";
		public const string TableNsPrefix = "table";
		public const string DcNsPrefix = "dc";
		public const string OOWNsPrefix = "oow";
		#endregion
		#region Namespace
		public const string ElementNs = "xmlns";
		const string OpenOfficeRootNS = "http://openoffice.org/";
		const string W3OrgRootNs = "http://www.w3.org/";
		public const string OpenDocumentRootNs = "urn:oasis:names:tc:opendocument:xmlns:";
		public const string OfficeNamespace = OpenDocumentRootNs + "office:" + OpenDocumentVersion;
		public const string TextNamespace = OpenDocumentRootNs + "text:" + OpenDocumentVersion;
		public const string StyleNamespace = OpenDocumentRootNs + "style:" + OpenDocumentVersion;
		public const string ManifestNamespace = OpenDocumentRootNs + "manifest:" + OpenDocumentVersion;
		public const string FoNamespace = OpenDocumentRootNs + "xsl-fo-compatible:" + OpenDocumentVersion;
		public const string SvgNamespace = OpenDocumentRootNs + "svg-compatible:" + OpenDocumentVersion;
		public const string OOWNamespace = OpenOfficeRootNS + "2004/writer";
		public const string DrawNamespace = OpenDocumentRootNs + "drawing:" + OpenDocumentVersion;
		public const string DcNamespace = "http://purl.org/dc/elements/1.1/";
		public const string XlinkNamespace = W3OrgRootNs + "1999/xlink";
		public const string TableNamespace = OpenDocumentRootNs + "table:" + OpenDocumentVersion;
		#endregion
		public const string ElementDocument = "document";
		public const string ElementDocumentContent = "document-content";
		public const string ElementDocumentStyles = "document-styles";
		public const string ElementDocumentMeta = "document-meta";
		public const string ManifestMediaType = "application/vnd.oasis.opendocument.text";
		#endregion
		#region Translation Tables
		static readonly TranslationTable<OfficeParagraphAlignment> paragraphAlignmentTable = CreateOfficeParagraphAlignmentTable();
		static readonly TranslationTable<bool> fontStyleTable = CreateFontStyleTable();
		static readonly TranslationTable<bool> fontBoldTable = CreateFontBoldTable();
		static readonly TranslationTable<bool> fontCaseTable = CreateFontCaseTable();
		static readonly TranslationTable<Char> listNumberSeparatorTable = CreateListNumberSeparatorTable();
		static readonly TranslationTable<bool> columnSeparatorStyleTable = CreateColumnSeparatorStyleTable();
		static readonly TranslationTable<bool> fontUnderLineWidthTable = CreateUnderLineBoldTable();
		static readonly TranslationTable<CharacterFormattingScript> characterScriptTable = CreateCharacterScriptTable();
		static readonly TranslationTable<OpenDocumentTableAlignment> tableRowAlignmentTable = CreateTableRowAlignmentTable();
		static readonly TranslationTable<OpenDocumentVerticalAlignment> verticalAlignmentTable = CreateVerticalAlignmentTable();
		static readonly TranslationTable<TextDirection> textDirectionTable = CreateTextDirectionTable();
		public static TranslationTable<OfficeParagraphAlignment> ParagraphAlignmentTable { get { return paragraphAlignmentTable; } }
		public static TranslationTable<bool> FontStyleTable { get { return fontStyleTable; } }
		public static TranslationTable<bool> FontBoldTable { get { return fontBoldTable; } }
		public static TranslationTable<bool> FontCaseTable { get { return fontCaseTable; } }
		public static TranslationTable<bool> ColumnSeparatorStyleTable { get { return columnSeparatorStyleTable; } }
		public static TranslationTable<Char> ListNumberSeparatorTable { get { return listNumberSeparatorTable; } }
		public static TranslationTable<bool> FontUnderLineWidthTable { get { return fontUnderLineWidthTable; } }
		public static TranslationTable<CharacterFormattingScript> CharacterScriptTable { get { return characterScriptTable; } }
		public static TranslationTable<TextDirection> TextDirectionTable { get { return textDirectionTable; } }
		public static TranslationTable<OpenDocumentVerticalAlignment> VerticalAlignmentTable { get { return verticalAlignmentTable; } }
		public static TranslationTable<OpenDocumentTableAlignment> TableRowAlignmentTable { get { return tableRowAlignmentTable; } }
		static TranslationTable<OfficeParagraphAlignment> CreateOfficeParagraphAlignmentTable() {
			TranslationTable<OfficeParagraphAlignment> result = new TranslationTable<OfficeParagraphAlignment>();
			result.Add(OfficeParagraphAlignment.Left, "left");
			result.Add(OfficeParagraphAlignment.Right, "right");
			result.Add(OfficeParagraphAlignment.Center, "center");
			result.Add(OfficeParagraphAlignment.Justify, "justify");
			result.Add(OfficeParagraphAlignment.Left, "start");
			result.Add(OfficeParagraphAlignment.Right, "end");
			return result;
		}
		static TranslationTable<bool> CreateFontStyleTable() {
			TranslationTable<bool> result = new TranslationTable<bool>();
			result.Add(false, "normal");
			result.Add(true, "italic");
			result.Add(true, "oblique");
			return result;
		}
		static TranslationTable<bool> CreateFontBoldTable() {
			TranslationTable<bool> result = new TranslationTable<bool>();
			result.Add(false, "normal");
			result.Add(true, "bold");
			result.Add(true, "100");
			result.Add(true, "200");
			result.Add(true, "300");
			result.Add(true, "400");
			result.Add(true, "500");
			result.Add(true, "600");
			result.Add(true, "700");
			result.Add(true, "800");
			result.Add(true, "900");
			return result;
		}
		static TranslationTable<bool> CreateFontCaseTable() {
			TranslationTable<bool> result = new TranslationTable<bool>();
			result.Add(false, "none");
			result.Add(true, "uppercase");
			return result;
		}
		private static TranslationTable<Char> CreateListNumberSeparatorTable() {
			TranslationTable<Char> result = new TranslationTable<Char>();
			result.Add(Characters.TabMark, "listtab");
			result.Add(' ', "space");
			result.Add(Char.MinValue, "nothing");
			return result;
		}
		static TranslationTable<bool> CreateColumnSeparatorStyleTable() {
			TranslationTable<bool> result = new TranslationTable<bool>();
			result.Add(false, "none");
			result.Add(true, "solid");
			result.Add(true, "dotted");
			result.Add(true, "dashed");
			result.Add(true, "dot-dashed");
			return result;
		}
		static TranslationTable<bool> CreateUnderLineBoldTable() {
			TranslationTable<bool> result = new TranslationTable<bool>();
			result.Add(false, "auto");
			result.Add(true, "bold");
			result.Add(true, "thick");
			result.Add(true, "medium");
			return result;
		}
		static TranslationTable<CharacterFormattingScript> CreateCharacterScriptTable() {
			TranslationTable<CharacterFormattingScript> result = new TranslationTable<CharacterFormattingScript>();
			result.Add(CharacterFormattingScript.Normal, "0% 100%");
			result.Add(CharacterFormattingScript.Subscript, "sub");
			result.Add(CharacterFormattingScript.Superscript, "super");
			return result;
		}
		static TranslationTable<OpenDocumentVerticalAlignment> CreateVerticalAlignmentTable() {
			TranslationTable<OpenDocumentVerticalAlignment> result = new TranslationTable<OpenDocumentVerticalAlignment>();
			result.Add(OpenDocumentVerticalAlignment.Bottom, "bottom");
			result.Add(OpenDocumentVerticalAlignment.Center, "middle");
			result.Add(OpenDocumentVerticalAlignment.Top, "top");
			return result;
		}
		static TranslationTable<OpenDocumentTableAlignment> CreateTableRowAlignmentTable() {
			TranslationTable<OpenDocumentTableAlignment> result = new TranslationTable<OpenDocumentTableAlignment>();
			result.Add(OpenDocumentTableAlignment.Left, "left");
			result.Add(OpenDocumentTableAlignment.Right, "right");
			result.Add(OpenDocumentTableAlignment.Center, "center");
			result.Add(OpenDocumentTableAlignment.Both, "margins"); 
			return result;
		}
		static TranslationTable<TextDirection> CreateTextDirectionTable() {
			TranslationTable<TextDirection> result = new TranslationTable<TextDirection>();
			result.Add(TextDirection.TopToBottomRightToLeft, "tb-rl"); 
			result.Add(TextDirection.LeftToRightTopToBottom, "lr");
			return result;
		}
		public static T GetFoEnumValue<T>(XmlReader reader, string attributeName, TranslationTable<T> table, T defaultValue) where T : struct {
			return GetEnumValue(reader, attributeName, FoNamespace, table, defaultValue);
		}
		public static T GetStyleEnumValue<T>(XmlReader reader, string attributeName, TranslationTable<T> table, T defaultValue) where T : struct {
			return GetEnumValue(reader, attributeName, StyleNamespace, table, defaultValue);
		}
		public static T GetEnumValue<T>(XmlReader reader, string attributeName, string ns, TranslationTable<T> table, T defaultValue) where T : struct {
			string value = reader.GetAttribute(attributeName, ns);
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			return table.GetEnumValue(value, defaultValue);
		}
		#endregion
		public OfficeOpenDocumentHelper(bool val) {
		}
	}
	#endregion
}
