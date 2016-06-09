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
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Internal;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Import.Html {
	#region HtmlSpecialSymbolTable
	public class HtmlSpecialSymbolTable : Dictionary<string, char> {
	}
	#endregion
	#region HtmlElementType
	public enum HtmlElementType {
		Content,
		OpenTag,
		CloseTag,
		EmptyTag,
		Comment,
	}
	#endregion
	#region HtmlElement
	public abstract class HtmlElement {
		string rawText = String.Empty;
		#region Properties
		public string RawText { get { return rawText; } set { rawText = value; } }
		public abstract HtmlElementType ElementType { get; }
		#endregion        
	}
	#endregion
	public enum HtmlTagNameID {
		Unknown,
		LI,
		TD,
		TR,
		TH,
		Table,
		NumberingList,
		BulletList,
		Style,
		Abbr,
		Acronym,
		Address,
		Area,
		BaseFont,
		Bdo,
		BgSound,
		Button,
		Cite,
		Dd,
		Del,
		Dfn,
		Dl,
		Dt,
		Embed,
		Fieldset,
		Form,
		Frame,
		FrameSet,
		Hr,
		Iframe,
		Input,
		Ins,
		Kbd,
		Label,
		Legend,
		Map,
		Nobr,
		Noembed,
		NoFrames,
		NoScript,
		Object,
		OptGroup,
		Option,
		Param,
		Q,
		Samp,
		Select,
		TextArea,
		TT,
		Var,
		Wbr,
		Xmp,
		Html,
		Head,
		Base,
		Meta,
		Title,
		Link,
		Anchor,
		Body,
		Bold,
		Italic,
		Underline,
		Paragraph,
		Strong,
		Big,
		Small,
		Preformatted,
		Font,
		LineBreak,
		Emphasized,
		Img,
		Heading1,
		Heading2,
		Heading3,
		Heading4,
		Heading5,
		Heading6,
		SuperScript,
		SubScript,
		Center,
		S,
		Strike,
		Code,
		Span,
		Div,
		Script,
		Blockquote,
		Caption,
		Thead,
		Tfoot,
		Tbody,
		Col,
		ColGroup,
	};
	#region Tag
	public class Tag : HtmlElement {
		HtmlTagNameID name;
		HtmlElementType type;
		List<Attribute> attributes = new List<Attribute>();
		public Tag(HtmlElementType type) {
			this.type = type;
			this.name = HtmlTagNameID.Unknown;
		}
		#region Propeties
		public HtmlTagNameID NameID { get { return name; } set { name = value; } }
		public List<Attribute> Attributes { get { return attributes; } }
		public override HtmlElementType ElementType { get { return type; } }
		#endregion
		protected internal Tag CopyFrom(Tag tag) {
			this.NameID = tag.NameID;
			this.attributes = tag.Attributes;
			return tag;
		}
		internal void AppendCharToName(StringBuilder sb, char ch) {
			sb.Append(Char.ToUpperInvariant(ch));
		}
#if DEBUGTEST || DEBUG
		public override string ToString() {
			if (type == HtmlElementType.CloseTag)
				return String.Format("</{0}>", NameID);
			else
				return String.Format("<{0}>", NameID);
		}
#endif
	}
	#endregion
	#region Comment
	public class Comment : HtmlElement {
		string commentText = String.Empty;
		public string CommentText { get { return commentText; } set { commentText = value; } }
		public override HtmlElementType ElementType { get { return HtmlElementType.Comment; } }
#if DEBUGTEST || DEBUG
		public override string ToString() {
			return String.Format("<!-- {0} -->", CommentText);
		}
#endif
	}
	#endregion
	#region Content
	public class Content : HtmlElement {
		string contentText = String.Empty;
		public string ContentText { get { return contentText; } set { contentText = value; } }
		public override HtmlElementType ElementType { get { return HtmlElementType.Content; } }
#if DEBUGTEST || DEBUG
		public override string ToString() {
			return String.Format("{0}", ContentText);
		}
#endif
	}
	#endregion
	#region Attribute
	public class Attribute {
		string name = String.Empty;
		string value = String.Empty;
		public string Name { get { return name; } set { name = value; } }
		public string Value { get { return value; } set { this.value = value; } }
	}
	#endregion
	#region ParserState
	public class ParserState {
		States state;
		States parentState;
		public ParserState(States state) {
			this.state = state;
		}
		public States State { get { return state; } }
		public States ParentState { get { return parentState; } }
		protected internal void ChangeState(States newState) {
			parentState = State;
			state = newState;
		}
	}
	#endregion
	#region States
	public enum States {
		StartState,
		ReadContent,
		ReadTag,
		ReadOpenTag,
		ReadEmptyTag,
		ReadComment,
		ReadCommentContent,
		ReadTagName,
		ReadAttributeName,
		WaitAttributeName,
		WaitAttrNameOrEqualSymbol,
		WaitAttributeValue,
		ReadAttributeValue,
		ReadValueInQuotes,
		ReadValueInApostrophe,
		EndTag,
		StopReading
	}
	#endregion
	#region HtmlParser
	public class HtmlParser {
		#region Field
		readonly static HtmlTagNameIDTable htmlTagNameIDTable = CreateHtmlTagNameTable();
		IStringBuilder rawText;
		StringBuilder attrText;
		StringBuilder commentText;
		string specialSymbol;
		HtmlElementType elementType;
		ParserState parserState;
		HtmlElement element;
		Tag parserTag;
		StringBuilder parserTagName;
		byte[] encodingDetectionBuffer;
		int encodingDetectionBufferIndex;
		#endregion
		public HtmlParser() {
			this.encodingDetectionBufferIndex = 0;
			this.encodingDetectionBuffer = new byte[10000];
			this.rawText = new SmartStringBuilder();
			this.attrText = new StringBuilder();
			this.commentText = new StringBuilder();
			this.specialSymbol = String.Empty;
			this.parserState = new ParserState(States.StartState);
		}
		#region Properties
		protected internal IStringBuilder RawText { get { return rawText; } }
		protected internal StringBuilder AttrText { get { return attrText; } }
		protected internal StringBuilder CommentText { get { return commentText; } }
		protected internal string SpecialSymbol { get { return specialSymbol; } set { specialSymbol = value; } }
		protected internal HtmlElementType ElementType { get { return elementType; } set { elementType = value; } }
		protected internal ParserState ParserState { get { return parserState; } }
		protected internal HtmlElement Element { get { return element; } set { element = value; } }
		protected internal Tag ParserTag { get { return parserTag; } set { parserTag = value; } }
		protected internal StringBuilder ParserTagName { get { return parserTagName; } set { parserTagName = value; } }
		protected internal Attribute Attr { get { return ParserTag.Attributes[ParserTag.Attributes.Count - 1]; } }
		bool IsEncodingDetectionBufferFull { get { return encodingDetectionBufferIndex >= encodingDetectionBuffer.Length; } }
		protected internal virtual HtmlTagNameIDTable HtmlTagNameIDTable { get { return htmlTagNameIDTable; } }
		#endregion
		#region CreateHtmlTagNameTable
		static HtmlTagNameIDTable CreateHtmlTagNameTable() {
			HtmlTagNameIDTable htmlKeywordTable = new HtmlTagNameIDTable();
			htmlKeywordTable.Add(ConvertKeyToUpper("abbr"), HtmlTagNameID.Abbr);
			htmlKeywordTable.Add(ConvertKeyToUpper("acronym"), HtmlTagNameID.Acronym);
			htmlKeywordTable.Add(ConvertKeyToUpper("address"), HtmlTagNameID.Address);
			htmlKeywordTable.Add(ConvertKeyToUpper("area"), HtmlTagNameID.Area);
			htmlKeywordTable.Add(ConvertKeyToUpper("basefont"), HtmlTagNameID.BaseFont);
			htmlKeywordTable.Add(ConvertKeyToUpper("bdo"), HtmlTagNameID.Bdo);
			htmlKeywordTable.Add(ConvertKeyToUpper("bgsound"), HtmlTagNameID.BgSound);
			htmlKeywordTable.Add(ConvertKeyToUpper("Button"), HtmlTagNameID.Button);
			htmlKeywordTable.Add(ConvertKeyToUpper("cite"), HtmlTagNameID.Cite);
			htmlKeywordTable.Add(ConvertKeyToUpper("dd"), HtmlTagNameID.Dd);
			htmlKeywordTable.Add(ConvertKeyToUpper("del"), HtmlTagNameID.Del);
			htmlKeywordTable.Add(ConvertKeyToUpper("dfn"), HtmlTagNameID.Dfn);
			htmlKeywordTable.Add(ConvertKeyToUpper("dl"), HtmlTagNameID.Dl);
			htmlKeywordTable.Add(ConvertKeyToUpper("dt"), HtmlTagNameID.Dt);
			htmlKeywordTable.Add(ConvertKeyToUpper("embed"), HtmlTagNameID.Embed);
			htmlKeywordTable.Add(ConvertKeyToUpper("fieldset"), HtmlTagNameID.Fieldset);
			htmlKeywordTable.Add(ConvertKeyToUpper("form"), HtmlTagNameID.Form);
			htmlKeywordTable.Add(ConvertKeyToUpper("frame"), HtmlTagNameID.Frame);
			htmlKeywordTable.Add(ConvertKeyToUpper("frameset"), HtmlTagNameID.FrameSet);
			htmlKeywordTable.Add(ConvertKeyToUpper("hr"), HtmlTagNameID.Hr);
			htmlKeywordTable.Add(ConvertKeyToUpper("iframe"), HtmlTagNameID.Iframe);
			htmlKeywordTable.Add(ConvertKeyToUpper("input"), HtmlTagNameID.Input);
			htmlKeywordTable.Add(ConvertKeyToUpper("ins"), HtmlTagNameID.Ins);
			htmlKeywordTable.Add(ConvertKeyToUpper("kbd"), HtmlTagNameID.Kbd);
			htmlKeywordTable.Add(ConvertKeyToUpper("label"), HtmlTagNameID.Label);
			htmlKeywordTable.Add(ConvertKeyToUpper("legend"), HtmlTagNameID.Legend);
			htmlKeywordTable.Add(ConvertKeyToUpper("map"), HtmlTagNameID.Map);
			htmlKeywordTable.Add(ConvertKeyToUpper("nobr"), HtmlTagNameID.Nobr);
			htmlKeywordTable.Add(ConvertKeyToUpper("noembed"), HtmlTagNameID.Noembed);
			htmlKeywordTable.Add(ConvertKeyToUpper("noframes"), HtmlTagNameID.NoFrames);
			htmlKeywordTable.Add(ConvertKeyToUpper("noscript"), HtmlTagNameID.NoScript);
			htmlKeywordTable.Add(ConvertKeyToUpper("object"), HtmlTagNameID.Object);
			htmlKeywordTable.Add(ConvertKeyToUpper("optgroup"), HtmlTagNameID.OptGroup);
			htmlKeywordTable.Add(ConvertKeyToUpper("option"), HtmlTagNameID.Option);
			htmlKeywordTable.Add(ConvertKeyToUpper("param"), HtmlTagNameID.Param);
			htmlKeywordTable.Add(ConvertKeyToUpper("q"), HtmlTagNameID.Q);
			htmlKeywordTable.Add(ConvertKeyToUpper("samp"), HtmlTagNameID.Samp);
			htmlKeywordTable.Add(ConvertKeyToUpper("select"), HtmlTagNameID.Select);
			htmlKeywordTable.Add(ConvertKeyToUpper("textarea"), HtmlTagNameID.TextArea);
			htmlKeywordTable.Add(ConvertKeyToUpper("tt"), HtmlTagNameID.TT);
			htmlKeywordTable.Add(ConvertKeyToUpper("var"), HtmlTagNameID.Var);
			htmlKeywordTable.Add(ConvertKeyToUpper("wbr"), HtmlTagNameID.Wbr);
			htmlKeywordTable.Add(ConvertKeyToUpper("xmp"), HtmlTagNameID.Xmp);
			htmlKeywordTable.Add(ConvertKeyToUpper("html"), HtmlTagNameID.Html);
			htmlKeywordTable.Add(ConvertKeyToUpper("head"), HtmlTagNameID.Head);
			htmlKeywordTable.Add(ConvertKeyToUpper("base"), HtmlTagNameID.Base);
			htmlKeywordTable.Add(ConvertKeyToUpper("meta"), HtmlTagNameID.Meta);
			htmlKeywordTable.Add(ConvertKeyToUpper("title"), HtmlTagNameID.Title);
			htmlKeywordTable.Add(ConvertKeyToUpper("link"), HtmlTagNameID.Link);
			htmlKeywordTable.Add(ConvertKeyToUpper("a"), HtmlTagNameID.Anchor);
			htmlKeywordTable.Add(ConvertKeyToUpper("body"), HtmlTagNameID.Body);
			htmlKeywordTable.Add(ConvertKeyToUpper("b"), HtmlTagNameID.Bold);
			htmlKeywordTable.Add(ConvertKeyToUpper("i"), HtmlTagNameID.Italic);
			htmlKeywordTable.Add(ConvertKeyToUpper("u"), HtmlTagNameID.Underline);
			htmlKeywordTable.Add(ConvertKeyToUpper("p"), HtmlTagNameID.Paragraph);
			htmlKeywordTable.Add(ConvertKeyToUpper("strong"), HtmlTagNameID.Strong);
			htmlKeywordTable.Add(ConvertKeyToUpper("big"), HtmlTagNameID.Big);
			htmlKeywordTable.Add(ConvertKeyToUpper("small"), HtmlTagNameID.Small);
			htmlKeywordTable.Add(ConvertKeyToUpper("pre"), HtmlTagNameID.Preformatted);
			htmlKeywordTable.Add(ConvertKeyToUpper("font"), HtmlTagNameID.Font);
			htmlKeywordTable.Add(ConvertKeyToUpper("br"), HtmlTagNameID.LineBreak);
			htmlKeywordTable.Add(ConvertKeyToUpper("em"), HtmlTagNameID.Emphasized);
			htmlKeywordTable.Add(ConvertKeyToUpper("img"), HtmlTagNameID.Img);
			htmlKeywordTable.Add(ConvertKeyToUpper("h1"), HtmlTagNameID.Heading1);
			htmlKeywordTable.Add(ConvertKeyToUpper("h2"), HtmlTagNameID.Heading2);
			htmlKeywordTable.Add(ConvertKeyToUpper("h3"), HtmlTagNameID.Heading3);
			htmlKeywordTable.Add(ConvertKeyToUpper("h4"), HtmlTagNameID.Heading4);
			htmlKeywordTable.Add(ConvertKeyToUpper("h5"), HtmlTagNameID.Heading5);
			htmlKeywordTable.Add(ConvertKeyToUpper("h6"), HtmlTagNameID.Heading6);
			htmlKeywordTable.Add(ConvertKeyToUpper("sup"), HtmlTagNameID.SuperScript);
			htmlKeywordTable.Add(ConvertKeyToUpper("sub"), HtmlTagNameID.SubScript);
			htmlKeywordTable.Add(ConvertKeyToUpper("center"), HtmlTagNameID.Center);
			htmlKeywordTable.Add(ConvertKeyToUpper("table"), HtmlTagNameID.Table);
			htmlKeywordTable.Add(ConvertKeyToUpper("tr"), HtmlTagNameID.TR);
			htmlKeywordTable.Add(ConvertKeyToUpper("th"), HtmlTagNameID.TH);
			htmlKeywordTable.Add(ConvertKeyToUpper("td"), HtmlTagNameID.TD);
			htmlKeywordTable.Add(ConvertKeyToUpper("li"), HtmlTagNameID.LI);
			htmlKeywordTable.Add(ConvertKeyToUpper("ol"), HtmlTagNameID.NumberingList);
			htmlKeywordTable.Add(ConvertKeyToUpper("ul"), HtmlTagNameID.BulletList);
			htmlKeywordTable.Add(ConvertKeyToUpper("s"), HtmlTagNameID.S);
			htmlKeywordTable.Add(ConvertKeyToUpper("strike"), HtmlTagNameID.Strike);
			htmlKeywordTable.Add(ConvertKeyToUpper("code"), HtmlTagNameID.Code);
			htmlKeywordTable.Add(ConvertKeyToUpper("span"), HtmlTagNameID.Span);
			htmlKeywordTable.Add(ConvertKeyToUpper("div"), HtmlTagNameID.Div);
			htmlKeywordTable.Add(ConvertKeyToUpper("script"), HtmlTagNameID.Script);
			htmlKeywordTable.Add(ConvertKeyToUpper("blockquote"), HtmlTagNameID.Blockquote);
			htmlKeywordTable.Add(ConvertKeyToUpper("caption"), HtmlTagNameID.Caption);
			htmlKeywordTable.Add(ConvertKeyToUpper("THEAD"), HtmlTagNameID.Thead);
			htmlKeywordTable.Add(ConvertKeyToUpper("TFOOT"), HtmlTagNameID.Tfoot);
			htmlKeywordTable.Add(ConvertKeyToUpper("TBODY"), HtmlTagNameID.Tbody);
			htmlKeywordTable.Add(ConvertKeyToUpper("COL"), HtmlTagNameID.Col);
			htmlKeywordTable.Add(ConvertKeyToUpper("COLGROUP"), HtmlTagNameID.ColGroup);
			htmlKeywordTable.Add(ConvertKeyToUpper("Style"), HtmlTagNameID.Style);
			return htmlKeywordTable;
		}
		#endregion
		protected internal static string ConvertKeyToUpper(string key) {
			return key.ToUpper(CultureInfo.InvariantCulture);
		}
		protected internal HtmlElement ParseNext(StreamReader stream) {
			return ParseNextCore(stream);
		}
		public void ParseNextScript(StreamReader stream, out HtmlElement scriptContent, out HtmlElement closeScriptTag) {
			ParseScriptContent(stream, out scriptContent, out closeScriptTag);
		}
		void ParseScriptContent(StreamReader stream, out HtmlElement scriptContent, out HtmlElement closeScriptTag) {
			scriptContent = null;
			closeScriptTag = null;
			ChunkedStringBuilder rawText = new ChunkedStringBuilder();
			while (true) {
				int intChar = stream.Read();
				if (intChar < 0) {
					if (rawText.Length == 0)
						return;
					break;
				}
				char ch = (char)intChar;
				bool tagEnd = ch == '<' && stream.Peek() == '/';
				if (tagEnd) {
					RawText.Clear();
					if (TryParseCloseScriptTag(stream, out closeScriptTag))
						break;
					else {
						rawText.Append(RawText.ToString());
						RawText.Clear();
					}
				}
				else
					rawText.Append(ch);
			}
			Content content = new Content();
			string text = rawText.ToString();
			content.ContentText = text;
			content.RawText = text;
			scriptContent = content;
		}
		bool TryParseCloseScriptTag(StreamReader stream, out HtmlElement element) {
			element = null;
			if (stream.EndOfStream)
				return false;
			ParserState.ChangeState(States.ReadTag);
			RawText.Append('<');
			if (IsReceivedElement(stream)) {
				Tag tag = Element as Tag;
				if (tag != null && tag.NameID == HtmlTagNameID.Script) {
					element = Element;
					return true;
				}
			}
			return false;
		}
		public HtmlElement ParseNextCore(StreamReader stream) {
			ParserState.ChangeState(States.StartState);
			rawText.Clear();
			int intChar = stream.Peek();
			if (intChar < 0)
				return null;
			if (IsReceivedElement(stream))
				return Element;
			else
				return CreateContentElement();
		}
		public bool IsReceivedElement(StreamReader stream) {
			int intChar = stream.Peek();
			while (intChar >= 0) {
				char ch = (char)intChar;
				switch (ParserState.State) {
					case States.StartState:
						StartState(ch);
						break;
					case States.ReadContent:
						ReadContent(ch);
						break;
					case States.ReadTag:
						ReadTag(ch);
						break;
					case States.ReadTagName:
						ReadTagName(ch);
						break;
					case States.ReadComment:
						ReadComment(ch);
						break;
					case States.ReadEmptyTag:
						ReadEmptyTag(ch);
						break;
					case States.WaitAttributeName:
						WaitAttributeName(ch, ParserTag);
						break;
					case States.ReadAttributeName:
						ReadAttributeName(ch);
						break;
					case States.WaitAttrNameOrEqualSymbol:
						WaitAttrNameOrEqualSymbol(ch);
						break;
					case States.WaitAttributeValue:
						WaitAttributeValue(ch);
						break;
					case States.ReadAttributeValue:
						ReadAttributeValue(ch);
						break;
					case States.ReadValueInApostrophe:
						ReadValueInApostrophe(ch);
						break;
					case States.ReadValueInQuotes:
						ReadValueInQuotes(ch);
						break;
				}
				if (IsEndState(stream))
					return true;
				ch = Read(stream);
				intChar = stream.Peek();
			}
			return false;
		}
		protected internal bool IsEndState(StreamReader stream) {
			if (ParserState.State == States.EndTag) {
				Read(stream);
				Element.RawText = RawText.ToString();
				return true;
			}
			else if (ParserState.State == States.StopReading) {
				Element.RawText = RawText.ToString();
				return true;
			}
			return false;
		}
		protected internal void StartState(char ch) {
			if (ch == '<')
				ParserState.ChangeState(States.ReadTag);
			else
				ParserState.ChangeState(States.ReadContent);
		}
		protected internal void ReadContent(char ch) {
			ToggleReadContentState(ch);
		}
		protected internal void ReadTag(char ch) {
			switch (ch) {
				case '!':
					ParserState.ChangeState(States.ReadComment);
					break;
				case '/':
					ParserTag = new Tag(HtmlElementType.CloseTag);
					ParserTagName = new StringBuilder();
					ParserState.ChangeState(States.ReadTagName);
					break;
				default:
					if (!Char.IsLetter(ch) && ch != '?') {
						ParserState.ChangeState(States.ReadContent);
						ToggleReadContentState(ch);
					}
					else {
						ParserTag = new Tag(HtmlElementType.OpenTag);
						ParserTagName = new StringBuilder();
						ParserState.ChangeState(States.ReadTagName);
						ParserTag.AppendCharToName(parserTagName, ch);
					}
					break;
			}
		}
		protected internal void ReadTagName(char ch) {
			if (IsTagClosed(ch))
				return;
			if (ch == '<')
				EndContentElement();
			else if (Char.IsWhiteSpace(ch)) {
				ParserTag.NameID = GetTagNameID(ParserTagName.ToString());
				ParserState.ChangeState(States.WaitAttributeName);
			}
			else
				ParserTag.AppendCharToName(parserTagName, ch);
		}
		protected internal void ReadComment(char ch) {
			if (ch == '>') {
				string comment = CommentText.ToString();
				if (comment.StartsWith("--")) {
					if (!comment.EndsWith("--"))
						return;
					comment = CutCommentBash(comment);
					EndComment(comment);
				}
				else
					EndComment(comment);
			}
			else
				CommentText.Append(ch);
		}
		private string CutCommentBash(string comment) {
			if (comment.Length < 4)
				return comment;
			return comment.Remove(comment.Length - 2, 2).Remove(0, 2);
		}
		protected internal void EndComment(string comment) {
			Element = CreateCommentElement(comment);
			CommentText.Length = 0;
			ParserState.ChangeState(States.EndTag);
		}
		protected internal void ReadEmptyTag(char ch) {
			if (ch == '>') {
				Tag emptyTag = new Tag(HtmlElementType.EmptyTag);
				ParserTag.NameID = GetTagNameID(ParserTagName.ToString());
				emptyTag.CopyFrom(ParserTag);
				ParserTagName = new StringBuilder();
				Element = CreateTagElement(emptyTag);
				ParserState.ChangeState(States.EndTag);
			}
		}
		protected internal void WaitAttributeName(char ch, Tag tag) {
			if (IsTagClosed(ch))
				return;
			if (Char.IsWhiteSpace(ch))
				return;
			if (ch == '<') {
				ForceEndElement();
				return;
			}
			Attribute attr = new Attribute();
			AttrText.Length = 0;
			List<Attribute> attributes = tag.Attributes;
			attributes.Add(attr);
			ParserState.ChangeState(States.ReadAttributeName);
			if (ch == '=')
				ParserState.ChangeState(States.WaitAttributeValue);
			else
				AttrText.Append(ch);
		}
		void ForceEndElement() {
			CreateTagElement();
			ParserState.ChangeState(States.StopReading);
		}
		protected internal void ReadAttributeName(char ch) {
			if (IsTagClosed(ch)) {
				Attr.Name = AttrText.ToString().ToUpper(CultureInfo.InvariantCulture);
			}
			else if (ch == '<') {
				ForceEndElement();
			}
			else if (Char.IsWhiteSpace(ch)) {
				Attr.Name = AttrText.ToString().ToUpper(CultureInfo.InvariantCulture);
				ParserState.ChangeState(States.WaitAttrNameOrEqualSymbol);
			}
			else if (ch == '=') {
				Attr.Name = AttrText.ToString().ToUpper(CultureInfo.InvariantCulture);
				ParserState.ChangeState(States.WaitAttributeValue);
			}
			else
				AttrText.Append(ch);
		}
		protected internal void WaitAttrNameOrEqualSymbol(char ch) {
			if (IsTagClosed(ch))
				return;
			if (Char.IsWhiteSpace(ch))
				return;
			if (ch == '=')
				ParserState.ChangeState(States.WaitAttributeValue);
			else if (ch == '<') {
				ForceEndElement();
			}
			else
				WaitAttributeName(ch, ParserTag);
		}
		protected internal void WaitAttributeValue(char ch) {
			AttrText.Length = 0;
			if (IsTagClosed(ch))
				return;
			if (Char.IsWhiteSpace(ch))
				return;
			switch (ch) {
				case '<':
					ForceEndElement();
					break;
				case '\'':
					ParserState.ChangeState(States.ReadValueInApostrophe);
					break;
				case '\"':
					ParserState.ChangeState(States.ReadValueInQuotes);
					break;
				default:
					ParserState.ChangeState(States.ReadAttributeValue);
					AttrText.Append(ch);
					break;
			}
		}
		protected internal void ReadAttributeValue(char ch) {
			if (ProcessTagClose(ch)) {
				Attr.Value = AttrText.ToString().Trim();
			}
			else if (ch == '<') {
				ForceEndElement();
			}
			else if (Char.IsWhiteSpace(ch)) {
				Attr.Value = AttrText.ToString().Trim();
				ParserState.ChangeState(States.WaitAttributeName);
			}
			else
				AttrText.Append(ch);
		}
		protected internal void ReadValueInApostrophe(char ch) {
			if (ch == '\'') {
				ParserState.ChangeState(States.WaitAttributeName);
				Attr.Value = AttrText.ToString().Trim();
			}
			else
				AttrText.Append(ch);
		}
		protected internal void ReadValueInQuotes(char ch) {
			if (ch == '\"') {
				ParserState.ChangeState(States.WaitAttributeName);
				Attr.Value = AttrText.ToString().Trim();
			}
			else
				AttrText.Append(ch);
		}
		protected internal bool IsTagClosed(char ch) {
			if (ProcessTagClose(ch))
				return true;
			if (ch == '/' && ParserTag.ElementType == HtmlElementType.OpenTag) {
				ParserState.ChangeState(States.ReadEmptyTag);
				return true;
			}
			return false;
		}
		protected internal bool ProcessTagClose(char ch) {
			if (ch == '>') {
				CreateTagElement();
				ParserState.ChangeState(States.EndTag);
				return true;
			}
			return false;
		}
		void CreateTagElement() {
			Element = CreateTagElement(ParserTag);
			if (ParserTagName.Length != 0) {
				ParserTag.NameID = GetTagNameID(ParserTagName.ToString());
				ParserTagName.Length = 0;
			}
		}
		void ToggleReadContentState(char ch) {
			if (ch == '<')
					EndContentElement();
			else
				ParserState.ChangeState(States.ReadContent);
		}
		void EndContentElement() {
			Element = CreateContentElement();
			ParserState.ChangeState(States.StopReading);
		}
		protected internal char Read(StreamReader stream) {
			char ch = (char)stream.Read();
			if (!IsEncodingDetectionBufferFull)
				AppendToEncodingDetectionBuffer((byte)ch);
			RawText.Append(ch);
			return ch;
		}
		protected internal void AppendToEncodingDetectionBuffer(byte value) {
			encodingDetectionBuffer[encodingDetectionBufferIndex] = value;
			encodingDetectionBufferIndex++;
		}
		public Encoding DetectEncoding() {
			InternalEncodingDetector encodingDetector = new InternalEncodingDetector();
			return encodingDetector.Detect(encodingDetectionBuffer, 0, Math.Min(encodingDetectionBuffer.Length, encodingDetectionBufferIndex));
		}
		protected internal virtual HtmlElement CreateContentElement() {
			Content element = new Content();
			string text = ReplaceSpecialSymbol(RawText.ToString());
			element.ContentText = text;
			element.RawText = text;
			return element;
		}
		protected internal virtual HtmlElement CreateCommentElement(string comment) {
			Comment element = new Comment();
			element.CommentText = comment;
			element.RawText = ReplaceSpecialSymbol(RawText.ToString());
			return element;
		}
		protected internal virtual HtmlElement CreateTagElement(Tag tag) {
			tag.RawText = ReplaceSpecialSymbol(RawText.ToString());
			return tag;
		}
		StringBuilder text = new StringBuilder();
		string ReplaceSpecialSymbolCore(string rawText, int startIndex, bool isPrevWhiteSpace) {
			text.Length = 0;
			text.Append(rawText, 0, startIndex);
			for (int i = startIndex; i < rawText.Length; i++) {
				char ch = rawText[i];
				if (ch == '\n' || ch == '\r' || ch == '\t' || ch == ' ' || ch == '\0') {
					if (!isPrevWhiteSpace) {
						text.Append(' ');
						isPrevWhiteSpace = true;
					}
				}
				else {
					isPrevWhiteSpace = false;
					text.Append(ch);
				}
			}
			return text.ToString();
		}
		string ReplaceSpecialSymbol(string rawText) {
			bool isPrevWhiteSpace = false;
			for (int i = 0; i < rawText.Length; i++) {
				char ch = rawText[i];
				bool isSpecialWhiteSpace = ch == '\n' || ch == '\r' || ch == '\t' || ch == '\0';
				if (isSpecialWhiteSpace || ch == ' ') {
					if (isPrevWhiteSpace || isSpecialWhiteSpace)
						return ReplaceSpecialSymbolCore(rawText, i, isPrevWhiteSpace);
					isPrevWhiteSpace = true;
				} else
					isPrevWhiteSpace = false;
			}
			return rawText;
		}
		protected internal HtmlTagNameID GetTagNameID(string name) {
			HtmlTagNameID result;
			if (HtmlTagNameIDTable.TryGetValue(name, out result))
				return result;
			return HtmlTagNameID.Unknown;
		}
	}
	#endregion
}
