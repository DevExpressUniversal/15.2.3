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
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Model;
using System.Text;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region SpanDestination
	public class SpanDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("span", OnSpanDestination);
			result.Add("tab", OnTabDestination);
			result.Add("line-break", OnLineBreakDestination);
			result.Add("bookmark", OnBookmarkDestination);
			result.Add("bookmark-start", OnBookmarkStartDestination);
			result.Add("bookmark-end", OnBookmarkEndDestination);
			result.Add("frame", OnFrameDestination);
			result.Add("rect", OnFrameDestination);
			result.Add("custom-shape", OnFrameDestination);
			result.Add("s", OnSpacesDestination);
			result.Add("a", OnHyperlinkDestination);
			result.Add("annotation", OnAnnotationDestination);
			FieldHandlers.AddFieldHandlers(result);
			return result;
		}
		static Destination OnSpanDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new SpanDestination(importer);
		}
		static Destination OnTabDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TabMarkDestination(importer);
		}
		static Destination OnLineBreakDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new LineBreakDestination(importer);
		}
		static Destination OnBookmarkDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new BookmarkElementDestination(importer);
		}
		static Destination OnBookmarkStartDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new BookmarkStartElementDestination(importer);
		}
		static Destination OnBookmarkEndDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new BookmarkEndElementDestination(importer);
		}
		static Destination OnFrameDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FrameDestination(importer);
		}
		static Destination OnSpacesDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new SpacesDestination(importer);
		}
		static Destination OnHyperlinkDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
		static Destination OnAnnotationDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new AnnotationDestination(importer);
		}
		string styleName = String.Empty;
		public SpanDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			SaveFormating();
		}
		private void SaveFormating() {
			Importer.InputPosition.SaveCharacterFormatting();
		}
		private void RestoreFormating() {
			Importer.InputPosition.RestoreCharacterFormatting();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected CharacterStyleCollection CharacterStyles { get { return Importer.DocumentModel.CharacterStyles; } }
		protected string StyleName { get { return styleName; } }
		public override void ProcessElementOpen(XmlReader reader) {
			this.styleName = ImportHelper.GetTextStringAttribute(reader, "style-name");
			if (!String.IsNullOrEmpty(styleName))
				MergeStyle(styleName);
		}
		public override void ProcessElementClose(XmlReader reader) {
			RestoreFormating();
		}
		protected internal virtual void MergeStyle(string styleName){
			CharacterAutoStyleInfo characterAutoStyleInfo;
			if (Importer.CharacterAutoStyles.TryGetValue(styleName, out characterAutoStyleInfo)) {
				MergeCommonStyle(characterAutoStyleInfo.ParentStyleName);
				CharacterFormattingBase autoStyle = characterAutoStyleInfo.CharacterFormatting;
				Importer.InputPosition.CharacterFormatting.CopyFrom(autoStyle);
			}
			else {
				MergeCommonStyle(styleName);
			}
		}
		protected internal virtual void MergeCommonStyle(string styleName) {
			if (String.IsNullOrEmpty(styleName) || !DocumentFormatsHelper.ShouldApplyCharacterStyle(Importer.DocumentModel))
				return; 
			int styleIndex = GetCharacterStyleIndexByName(styleName);
			Importer.InputPosition.CharacterStyleIndex = styleIndex;
		}
		protected internal virtual int GetCharacterStyleIndexByName(string name) {
			return Math.Max(0, Importer.DocumentModel.CharacterStyles.GetStyleIndexByName(name));
		}
		public override bool ProcessText(XmlReader reader) {
			string textContent = reader.Value;
			textContent = ImportHelper.RemoveRedundantSpaces(textContent, true);
			if (!String.IsNullOrEmpty(textContent)) {
				Importer.PieceTable.InsertTextCore(Importer.InputPosition, textContent);
			}
			return true;
		}
	}
	#endregion
	#region LineBreakDestination
	public class LineBreakDestination : LeafElementDestination {
		public LineBreakDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			switch(Importer.Options.LineBreakSubstitute) {
				case LineBreakSubstitute.Space:
					Importer.PieceTable.InsertTextCore(Importer.InputPosition, new String(Characters.Space, 1));
					break;
				case LineBreakSubstitute.Paragraph:
					Importer.PieceTable.InsertParagraphCore(Importer.InputPosition);
					break;
				default:
			Importer.PieceTable.InsertTextCore(Importer.InputPosition, new String(Characters.LineBreak, 1));
					break;
			}
		}
	}
	#endregion
	#region SpacesDestination
	public class SpacesDestination : LeafElementDestination {
		public SpacesDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int count = ImportHelper.GetTextIntegerAttribute(reader, "c", Int32.MinValue);
			if (count == Int32.MinValue)
				count = 1;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < count; i++) {
				sb.Append(' ');
			}
			Importer.PieceTable.InsertTextCore(Importer.InputPosition, sb.ToString());
		}
	}
	#endregion
}
