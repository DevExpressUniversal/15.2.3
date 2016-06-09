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
using DevExpress.Office;
using DevExpress.Office.Utils;
using System.Collections;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region AutomaticStylesDestination
	public class AutomaticStylesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("page-layout", OnPageLayout);
			result.Add("style", OnStyle);
			result.Add("list-style", OnListStyle);
			return result;
		}
		public AutomaticStylesDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnPageLayout(OpenDocumentTextImporter importer, XmlReader reader) {
			return new PageLayoutStyleDestination(importer);
		}
		static Destination OnStyle(OpenDocumentTextImporter importer, XmlReader reader) {
			return new AutomaticStyleDestination(importer);
		}
		static Destination OnListStyle(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListStyleDestination(importer);
		}
	}
	#endregion
	#region AutomaticStyleDestination
	public class AutomaticStyleDestination : StyleDestinationBase {
		public AutomaticStyleDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}		
		protected internal override void ImportParagraphStyle() {
			if (Importer.ParagraphAutoStyles.ContainsKey(StyleName)) {
				Debug.Assert(false, "Tried to overwrite existing paragraph Auto Style", StyleName);
				return;
			}
			ParagraphAutoStyleInfo info = new ParagraphAutoStyleInfo(ParagraphFormatting, CharacterFormatting, ParentName);
			info.Tabs = Tabs;
			info.Breaks = ParagraphBreaks;
			info.MasterPageName = MasterPageName;
			Importer.ParagraphAutoStyles.Add(StyleName, info);
		}
		protected internal override void ImportCharacterStyle() {
			if (!Importer.CharacterAutoStyles.ContainsKey(StyleName)) {
				CharacterAutoStyleInfo characterAutoStyleInfo = new CharacterAutoStyleInfo(CharacterFormatting, ParentName);
				Importer.CharacterAutoStyles.Add(StyleName, characterAutoStyleInfo);
			}
			else {
				Debug.Assert(false, "Tried to overwrite existing character Auto Style", StyleName);
			}
		}
	}
	#endregion
	#region Style Infos 
	#region CharacterAutoStyleInfo
	public class CharacterAutoStyleInfo {
		string parentStyleName = String.Empty;
		CharacterFormattingBase characterFormatting;
		public CharacterAutoStyleInfo(CharacterFormattingBase characterFormatting, string parent) {
			this.parentStyleName = parent;
			Guard.ArgumentNotNull(characterFormatting, "characterFormatting");
			this.characterFormatting = characterFormatting;
		}
		public CharacterFormattingBase CharacterFormatting { get { return characterFormatting; } }
		public string ParentStyleName { get { return parentStyleName; }  }
	}
	#endregion
	#region ParagraphBreaksInfo
	public class ParagraphBreaksInfo {
		ParagraphBreakType breakBefore = ParagraphBreakType.None;
		ParagraphBreakType breakAfter = ParagraphBreakType.None;
		public ParagraphBreakType BreakBefore { get { return breakBefore; } set { breakBefore = value; } }
		public ParagraphBreakType BreakAfter { get { return breakAfter; } set { breakAfter = value; } }
		public void Clear() {
			this.breakAfter = ParagraphBreakType.None;
			this.breakBefore = ParagraphBreakType.None;
		}
		public void CopyFrom(ParagraphBreaksInfo source) {
			this.breakBefore = source.breakBefore;
			this.breakAfter = source.breakAfter;
		}
		public void Reset() {
			this.breakBefore = ParagraphBreakType.None;
			this.breakAfter = ParagraphBreakType.None;
		}
	}
	#endregion
	#region ParagraphAutoStyleInfo
	public class ParagraphAutoStyleInfo : CharacterAutoStyleInfo {
		ParagraphFormattingBase paragraphFormatting;
		TabFormattingInfo tabs;
		ParagraphBreaksInfo breaks;
		string masterPageName = String.Empty;
		public ParagraphAutoStyleInfo(ParagraphFormattingBase paragraphFormatting, CharacterFormattingBase charaterFormatting, string parent)
			: base(charaterFormatting, parent) {
			Guard.ArgumentNotNull(paragraphFormatting, "paragraphFormatting");
			this.paragraphFormatting = paragraphFormatting;
		}
		public ParagraphFormattingBase ParagraphFormatting { get { return paragraphFormatting; } }
		public TabFormattingInfo Tabs { get { return tabs; } set { tabs = value; } }
		public ParagraphBreaksInfo Breaks { get { return breaks; } set { breaks = value; } }
		public string MasterPageName { get { return masterPageName; } set { masterPageName = value; } }
	}
	#endregion
	#region SectionStyleInfo
	public class SectionStyleInfo {
		ColumnStyles columns;
		int leftMargin;
		int rightMargin;
		public SectionStyleInfo() {
			this.columns = new ColumnStyles();
		}
		public ColumnStyles Columns { get { return columns; } }
		public int LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		public int RightMargin { get { return rightMargin; } set { rightMargin = value; } }
	}
	#endregion
	#endregion
	#region Auto Styles
	public class ParagraphAutoStyles : Dictionary<string, ParagraphAutoStyleInfo> {
	}
	public class FrameAutoStyles : Dictionary<string, FloatingObjectImportInfo> {
	}
	public class CharacterAutoStyles : Dictionary<string, CharacterAutoStyleInfo> {
	}
	public class SectionAutoStyles : Dictionary<string, SectionStyleInfo> {
	}
	public class TableAutoStyles : Dictionary<string, TablePropertiesInfo> {
	}
	public class TableColumnAutoStyles : Dictionary<string, TableColumnWidthInfo> {
	}
	public class TableRowsAutoStyles : Dictionary<string, TableRowInfo> {
	}
	public class TableCellsAutoStyles : Dictionary<string, TableCellProperties> {
	}
	#endregion
}
