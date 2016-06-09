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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Html {
	#region AnchorTag
	public class AnchorTag : TagBase {
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("href"),HrefKeyword);
			table.Add(ConvertKeyToUpper("title"), TitleKeyword);
			table.Add(ConvertKeyToUpper("target"), TargetKeyword);
			table.Add(ConvertKeyToUpper("name"), NameKeyword);
			table.Add(ConvertKeyToUpper("hreflang"), HrefLangKeyword);
			table.Add(ConvertKeyToUpper("type"), TypeKeyword);
			table.Add(ConvertKeyToUpper("rel"), RelKeyword);
			table.Add(ConvertKeyToUpper("ref"), RefKeyword);
			table.Add(ConvertKeyToUpper("charset"), CharsetKeyword);
			return table;
		}
		static protected internal void HrefKeyword(HtmlImporter importer, string value, TagBase tag) {
			AnchorTag anchor = (AnchorTag)tag;
			anchor.HasHrefKeyword = true;
			int n = value.IndexOf('#');
			if(n == 0)
				anchor.Bookmark = value.TrimStart('#');
			else {
				string hier, fragment;
				if(n == -1) {
					hier = value;
					fragment = String.Empty;
				}
				else {
					hier = value.Substring(0, n);
					fragment = value.Substring(n);
				}
				anchor.NavigateUri = HyperlinkUriHelper.EnsureUriIsValid(hier);
				anchor.Bookmark = fragment.TrimStart('#');
			}
		}
		static internal void NameKeyword(HtmlImporter importer, string value, TagBase tag) {
			((AnchorTag)tag).AnchorName = value;
		}
		static internal void HrefLangKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void TypeKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void RelKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void RefKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void CharsetKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void TitleKeyword(HtmlImporter importer, string value, TagBase tag) {
			((AnchorTag)tag).Tooltip = value;
		}
		static internal void TargetKeyword(HtmlImporter importer, string value, TagBase tag) {
			((AnchorTag)tag).Target = value;
		}
		string anchorName;
		string tooltip;
		string target;
		string bookmark;
		string navigateUri;
		bool hasHrefKeyword;
		HtmlInputPosition oldPosition;
		public AnchorTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		protected internal string AnchorName { get { return anchorName; } set { anchorName = value; } }
		protected internal string Tooltip { get { return tooltip; } set { tooltip = value; } }
		protected internal string Target { get { return target; } set { target = value; } }
		protected internal string Bookmark { get { return bookmark; } set { bookmark = value; } }
		protected internal string NavigateUri { get { return navigateUri; } set { navigateUri = value; } }
		protected internal bool HasHrefKeyword { get { return hasHrefKeyword; } set { hasHrefKeyword = value; } }
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void FunctionalTagProcess() {
			Importer.IsEmptyLine = false;
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			if (hasHrefKeyword) {
				int styleIndex = DocumentModel.CharacterStyles.GetStyleIndexByName(CharacterStyleCollection.HyperlinkStyleName);
				MergedCharacterProperties styleProperties = DocumentModel.CharacterStyles[styleIndex].GetMergedCharacterProperties();
				CharacterFormattingOptions.Mask styleOptions = styleProperties.Options.Value;
				Importer.Position.CharacterStyleIndex = styleIndex;
				Importer.Position.CharacterFormatting.ResetUse(styleOptions);
			}
			ParagraphFormattingOptions result = base.ApplyCssProperties();
			return result;
		}
		protected internal override void OpenTagProcessCore() {
			if (navigateUri != null || bookmark != null)
				Importer.LastOpenAnchorTagIndex = Importer.TagsStack.Count - 1;
			if (String.IsNullOrEmpty(anchorName))
				anchorName = this.Id;
			if (Importer.ProcessHyperlink != null)
				Importer.ProcessHyperlinkEnd();
			if (!String.IsNullOrEmpty(anchorName))
				Importer.ProcessBookmarkStart(anchorName);
			if (navigateUri != null || bookmark != null) {				
				SaveCurrentPosition();
				Importer.ProcessHyperlinkStart(CreateHyperlinkInfo());
			}
		}
		protected void SaveCurrentPosition() {
				this.oldPosition = new HtmlInputPosition(Importer.PieceTable);
				this.oldPosition.CopyFrom(Importer.Position);
		}
		protected internal override void EmptyTagProcess() {
			FindKeywordInAttributeTable();
			if(String.IsNullOrEmpty(this.Id))
				return;
			PieceTable pieceTable = Importer.PieceTable;
			DocumentLogPosition endLogPosition = pieceTable.DocumentEndLogPosition;
			pieceTable.CreateBookmark(endLogPosition, 0, Id);
		}
		protected internal override void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			ImportFieldInfo fieldInfo = Importer.ProcessHyperlink;
			if (fieldInfo != null) {
				Importer.ProcessHyperlinkEnd();
				if (fieldInfo.CodeEndIndex + 1 == fieldInfo.ResultEndIndex)
					RemoveEmptyHyperlink(indexOfDeletedTag, fieldInfo);
			}
			if (Importer.ProcessBookmarks.Count > 0)
				Importer.ProcessBookmarkEnd();
		}
		void RemoveEmptyHyperlink(int indexOfDeletedTag, ImportFieldInfo fieldInfo) {
			Importer.PieceTable.RemoveField(fieldInfo.Field);
			int count = fieldInfo.ResultEndIndex - fieldInfo.CodeStartIndex + 1;
			Importer.DocumentModel.UnsafeEditor.DeleteRuns(Importer.PieceTable, fieldInfo.CodeStartIndex, count);
			DocumentModelPosition pos = DocumentModelPosition.FromRunStart(Importer.PieceTable, fieldInfo.CodeStartIndex);
			HtmlInputPosition position = Importer.Position;
			position.LogPosition = pos.LogPosition;
			position.ParagraphIndex = pos.ParagraphIndex;
			AnchorTag tag = Importer.TagsStack[indexOfDeletedTag].Tag as AnchorTag;
			if (tag != null)
				position.CopyFrom(tag.oldPosition);
		}
		protected virtual HyperlinkInfo CreateHyperlinkInfo() {
			HyperlinkInfo result = new HyperlinkInfo();
			result.Anchor = Bookmark != null ? Bookmark : String.Empty;
			result.NavigateUri = NavigateUri != null ? NavigateUri : String.Empty;
			result.Target = Target != null ? Target : String.Empty;
			result.ToolTip = Tooltip != null ? Tooltip : String.Empty;
			return result;
		}
	}
	#endregion
	#region HtmlBookmarkInfo
	public class HtmlBookmarkInfo : ImportBookmarkInfo {
	}
	#endregion
	public class HtmlTableInfo : TableInfo {
		readonly CellsRowSpanCollection cellsRowSpanCollection;
		int columnIndex;
		public HtmlTableInfo(Table table)
			: base(table) {
			this.cellsRowSpanCollection = new CellsRowSpanCollection();
		}
		public CellsRowSpanCollection CellsRowSpanCollection { get { return cellsRowSpanCollection; } }
		public int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		public void MoveNextRow() {
			columnIndex = 0;
		}
	}
	#region BaseTag
	public class BaseTag : TagBase {
		#region Field
		string baseUri;
		#endregion
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("href"), BaseUriKeyword);
			table.Add(ConvertKeyToUpper("target"), TargetUriKeyword);
			return table;
		}
		static internal void BaseUriKeyword(HtmlImporter importer, string value, TagBase tag) {
			BaseTag baseTag = (BaseTag)tag;
			baseTag.baseUri = value;
		}
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		static internal void TargetUriKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		public BaseTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void OpenTagProcessCore() {
			if (baseUri != null)
				Importer.BaseUri = baseUri;
		}
	}
	#endregion
	#region BgsoundTag
	public class BgsoundTag : TagBase {
		public BgsoundTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region ScriptTag
	public class ScriptTag : IgnoredTag {
		public ScriptTag(HtmlImporter importer)
			: base(importer) {
		}
	}
	#endregion
	#region NoScriptTag
	public class NoScriptTag : TagBase {
		public NoScriptTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
}
