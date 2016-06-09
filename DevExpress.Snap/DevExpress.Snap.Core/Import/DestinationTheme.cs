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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Import {
	#region ThemesDestination
	public class ThemesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("theme", OnThemes);
			result.Add("activeTheme", OnActiveTheme);
			return result;
		}
		string activeTheme;
		static Destination OnActiveTheme(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new PropertyDestination<string>(importer, str => { ((ThemesDestination)importer.PeekDestination()).activeTheme = str; });
		}
		static Destination OnThemes(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ThemeDestination((SnapImporter)importer);
		}
		public ThemesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (!String.IsNullOrEmpty(activeTheme))
				Importer.DocumentModel.ActiveTheme = Importer.DocumentModel.Themes.GetThemeByName(activeTheme);
		}
	}
	#endregion
	#region ThemeDestination
	public class ThemeDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("styles", OnStyles);
			result.Add("name", OnName);
			return result;
		}
		static Destination OnName(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new PropertyDestination<string>(importer, (themeName) => { ((ThemeDestination)importer.PeekDestination()).theme.SetNameCore(themeName); });
		}
		static Destination OnStyles(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ThemeStylesDestination(importer, ((ThemeDestination)importer.PeekDestination()).theme);
		}
		Theme theme;
		public ThemeDestination(SnapImporter importer)
			: base(importer) {
			this.theme = new Theme(Importer.DocumentModel);
		}
		internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string name = Importer.ReadDxStringAttr("name", reader);
			this.theme.Name = name;
			string imageId = Importer.ReadDxStringAttr("imageId", reader);
			if (!String.IsNullOrEmpty(imageId)) {
				OfficeImage image = Importer.LookupImageByRelationId(imageId, Importer.DocumentRootFolder);
				theme.SetIconCore(image);
			}
			string nativeName = Importer.ReadDxStringAttr("nativeName", reader);
			if (!String.IsNullOrEmpty(nativeName))
				theme.NativeName = nativeName;
			Importer.TableStyleInfosStack.Push(new OpenXmlStyleInfoCollection());
			Importer.TableCellStyleInfosStack.Push(new OpenXmlStyleInfoCollection());
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			Importer.TableCellStyleInfosStack.Pop();
			Importer.TableStyleInfosStack.Pop();
			theme.IsLoaded = true;
			ThemeCollection themes = Importer.DocumentModel.Themes;
			string themeName = !String.IsNullOrEmpty(theme.NativeName) ? theme.NativeName : theme.Name;
			Theme defaultTheme = themes.GetThemeByName(themeName);
			if (defaultTheme != null) {
				defaultTheme.CopyFrom(theme);
				defaultTheme.Name = theme.Name;
				defaultTheme.IncreaseVersion();
			}
			else
				themes.Add(theme);
		}
	}
	#endregion
	#region ThemeStylesDestination
	public class ThemeStylesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("style", OnStyle);
			return result;
		}
		static Destination OnStyle(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ThemeStyleDestination(importer, ((ThemeStylesDestination)importer.PeekDestination()).theme);
		}
		readonly Theme theme;
		public ThemeStylesDestination(WordProcessingMLBaseImporter importer, Theme theme)
			: base(importer) {
			this.theme = theme;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region ThemeStyleDestination
	public class ThemeStyleDestination : StyleDestination {
		Theme theme;
		public ThemeStyleDestination(WordProcessingMLBaseImporter importer, Theme theme)
			: base(importer) {
			this.theme = theme;
		}
		protected internal override int ImportTableStyleCore() {
			TableStyle style = CreateTableStyle();
			theme.TableStyles.Add(style);
			return theme.TableStyles.Count - 1;
		}
		protected internal override int ImportTableCellStyleCore() {
			TableCellStyle style = CreateTableCellStyle();
			theme.TableCellStyles.Add(style);
			return theme.TableCellStyles.Count - 1;
		}
		protected internal override TableCellStyle GetParentTableCellStyle(int index) {
			if (index >= 0 && index < theme.TableCellStyles.Count)
				return theme.TableCellStyles[index];
			return null;
		}
		protected internal override TableStyle GetParentTableStyle(int index) {
			if (index >= 0 && index < theme.TableStyles.Count)
				return theme.TableStyles[index];
			return null;
		}
	}
	#endregion
}
