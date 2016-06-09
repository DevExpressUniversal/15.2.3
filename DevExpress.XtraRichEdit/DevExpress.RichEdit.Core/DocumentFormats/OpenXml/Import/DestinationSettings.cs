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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using System.Collections.Generic;
using DevExpress.Office.Model;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region DocumentSettingsDestination
	public class DocumentSettingsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("autoHyphenation", OnAutoHyphenation);
			result.Add("defaultTabStop", OnDefaultTabStop);
			result.Add("evenAndOddHeaders", OnDifferentOddAndEvenPages);
			result.Add("documentProtection", OnDocumentProtection);
			result.Add("docVars", OnDocumentVariables);
			result.Add("footnotePr", OnFootNoteProperties);
			result.Add("endnotePr", OnEndNoteProperties);
			result.Add("displayBackgroundShape", OnDisplayBackgroundShape);
			result.Add("themeFontLang", onThemeFontLang);
			result.Add("clrSchemeMapping", onClrSchemeMapping);
			result.Add("styleLockTheme", onStyleLockTheme);
			return result;
		}
		public DocumentSettingsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnAutoHyphenation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentSettingsAutoHyphenationDestination(importer);
		}
		static Destination OnDefaultTabStop(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentSettingsDefaultTabStopDestination(importer);
		}
		static Destination OnDifferentOddAndEvenPages(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentSettingsDifferentOddAndEvenPagesDestination(importer);
		}
		static Destination OnDocumentProtection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentProtectionDestination(importer);
		}
		static Destination OnDocumentVariables(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentVariablesDestination(importer);
		}
		static Destination OnFootNoteProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentLevelFootNotePropertiesDestination(importer, importer.DocumentModel.Sections.First.FootNote);
		}
		static Destination OnEndNoteProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentLevelEndNotePropertiesDestination(importer, importer.DocumentModel.Sections.First.EndNote);
		}
		static Destination OnDisplayBackgroundShape(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DisplayBackgroundShapeDestination(importer);
		}
		static Destination onThemeFontLang(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ThemeFontLangDestination(importer);
		}
		static Destination onClrSchemeMapping(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ClrSchemeMappingDestination(importer);
		}
		static Destination onStyleLockTheme(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StyleLockThemeDestination(importer);
		}
	}
	#endregion
	#region DocumentSettingsAutoHyphenationDestination
	public class DocumentSettingsAutoHyphenationDestination : LeafElementDestination {
		public DocumentSettingsAutoHyphenationDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.DocumentProperties.HyphenateDocument = Importer.GetWpSTOnOffValue(reader, "val", false);
		}
	}
	#endregion
	#region DocumentSettingsDefaultTabStopDestination
	public class DocumentSettingsDefaultTabStopDestination : LeafElementDestination {
		public DocumentSettingsDefaultTabStopDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "val", Int32.MinValue);
			if (value > 0)
				Importer.DocumentModel.DocumentProperties.DefaultTabWidth = Math.Max(1, UnitConverter.TwipsToModelUnits(value));
		}
	}
	#endregion
	#region DocumentSettingsDifferentOddAndEvenPagesDestination
	public class DocumentSettingsDifferentOddAndEvenPagesDestination : LeafElementDestination {
		public DocumentSettingsDifferentOddAndEvenPagesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.DocumentProperties.DifferentOddAndEvenPages = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region DocumentProtectionDestination
	public class DocumentProtectionDestination : LeafElementDestination {
		public DocumentProtectionDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			DocumentProtectionProperties properties = Importer.DocumentModel.ProtectionProperties;
			string val = Importer.ReadAttribute(reader, "hash");
			if (!String.IsNullOrEmpty(val))
				properties.PasswordHash = Convert.FromBase64String(val);
			val = Importer.ReadAttribute(reader, "salt");
			if (!String.IsNullOrEmpty(val))
				properties.PasswordPrefix = Convert.FromBase64String(val);
			val = Importer.ReadAttribute(reader, "unprotectPassword");
			if (!String.IsNullOrEmpty(val)) {
				if (val != "00000000") {
					int hash;
					if (Int32.TryParse(val, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hash)) {
						uint value = (uint)hash;
						hash = (int)(((value << 24) & 0xFF000000) | ((value << 8) & 0x00FF0000) | ((value >> 8) & 0x0000FF00) | ((value >> 24) & 0x000000FF));
						properties.Word2003PasswordHash = BitConverter.GetBytes(hash);
					}
				}
			}
			val = Importer.ReadAttribute(reader, "edit");
			if (val == "readOnly" || val == "read-only")
				properties.ProtectionType = DocumentProtectionType.ReadOnly;
			properties.EnforceProtection = Importer.GetWpSTOnOffValue(reader, "enforcement", false);
			properties.HashAlgorithmType = (HashAlgorithmType)Importer.GetWpSTIntegerValue(reader, "cryptAlgorithmSid", 0);
			properties.HashIterationCount = Importer.GetWpSTIntegerValue(reader, "cryptSpinCount", 1);
		}
	}
	#endregion
	#region DocumentVariablesDestination
	public class DocumentVariablesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("docVar", OnDocumentVariable);
			return result;
		}
		public DocumentVariablesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnDocumentVariable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentVariableDestination(importer);
		}
	}
	#endregion
	#region DocumentVariableDestination
	public class DocumentVariableDestination : LeafElementDestination {
		public DocumentVariableDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = Importer.ReadAttribute(reader, "name");
			string val = Importer.ReadAttribute(reader, "val");
			if (!String.IsNullOrEmpty(name) && val != null) {
				val = Importer.DecodeXmlChars(val);
				if (name != DocumentProperties.UpdateDocVariableFieldsBeforePrintDocVarName)
					Importer.DocumentModel.Variables.Add(name, val);
				else
					Importer.DocumentModel.DocumentProperties.SetUpdateFieldsBeforePrintFromDocVar(val);
			}
		}
	}
	#endregion
	#region DisplayBackgroundShapeDestination
	public class DisplayBackgroundShapeDestination : LeafElementDestination {
		public DisplayBackgroundShapeDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.DocumentProperties.DisplayBackgroundShape = Importer.GetWpSTOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region ClrSchemeMappingDestination
	public class ClrSchemeMappingDestination: LeafElementDestination {
		public ClrSchemeMappingDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Dictionary<SchemeColorValues, ThemeColorIndex> mapping = Importer.DocumentModel.OfficeTheme.Colors.SchemeColorValuesToThemeColorIndexTranslationTable;
			lock (mapping) {
				mapping.Clear();
				mapping.Add(SchemeColorValues.Background1, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "bg1", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Text1, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "t1", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Background2, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "bg2", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Text2, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "t2", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Accent1, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "accent1", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Accent2, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "accent2", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Accent3, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "accent3", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Accent4, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "accent4", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Accent5, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "accent5", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Accent6, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "accent6", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.Hyperlink, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "hyperlink", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
				mapping.Add(SchemeColorValues.FollowedHyperlink, Importer.GetWpEnumValue<ThemeColorIndex>(reader, "followedHyperlink", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None));
			}
		}
	}
	#endregion
	#region ThemeFontLangDestination
	public class ThemeFontLangDestination : LeafElementDestination {
		public ThemeFontLangDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.ThemeFontLangInfo = Importer.ReadLanguage(reader);
		}
	}
	#endregion
	#region StyleLockThemeDestination
	public class StyleLockThemeDestination : LeafElementDestination {
		public StyleLockThemeDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.StyleLockTheme = Importer.GetWpSTOnOffValue(reader, "styleLockTheme");
		}
	}
	#endregion
}
