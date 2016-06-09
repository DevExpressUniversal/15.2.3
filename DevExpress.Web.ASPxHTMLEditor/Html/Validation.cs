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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;
using System.Linq;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public enum AttrValueType { Text, CData, Enum, Flag }
	public enum ElementType { Valid, Forbidden, OptionallyForbidden, Deprecated, Replaceable }
	public enum CorrectionBehavior { None, SkipTag, SkipTagAndContent }
	public class Group {
		#region Nested Types
		private class AnyGroup : Group {
			public override bool Contains(Node node) {
				return true;
			}
		}
		private class EmptyGroup : Group {
			public override bool Contains(Node node) {
				return false;
			}
		}
		private class PCDataGroup : Group {
			public override bool Contains(Node node) {
				return node.IsPCData;
			}
		}
		#endregion
		#region Predefined Groups
		public static readonly Group
			Any = new AnyGroup(),
			Empty = new EmptyGroup(),
			PCData = new PCDataGroup(),
			SpecialExtra = new Group("#special.extra", E.ObjectElement, E.Applet, E.Img, E.Map, E.IFrame, E.Canvas, E.Command, E.Figure),
			SpecialBasic = new Group("#special.basic", E.Br, E.Span, E.Bdo),
			Special = new Group("#special", SpecialBasic, SpecialExtra),
			FontStyleExtra = new Group("#fontstyle.extra", E.Big, E.Small, E.Font, E.BaseFont),
			FontStyleBasic = new Group("#fontstyle.basic", E.TT, E.I, E.B, E.U, E.S, E.Strike),
			FontStyle = new Group("#fontstyle", FontStyleBasic, FontStyleExtra),
			PhraseExtra = new Group("#phrase.extra", E.Sub, E.Sup),
			PhraseBasic = new Group("#phrase.basic", E.Em, E.Strong, E.Dfn, E.Code, E.Q, E.Samp, E.Kbd, E.Var, E.Cite, E.Abbr, E.Acronym),
			Phrase = new Group("#phrase", PhraseBasic, PhraseExtra),
			InlineForms = new Group("#inline.forms", E.Input, E.Select, E.TextArea, E.Label, E.Button, E.Datalist, E.Keygen, E.Meter, E.Output),
			MiscInline = new Group("#misc.inline", E.Ins, E.Del, E.Script, E.Progress),
			Misc = new Group("#misc", "noscript", MiscInline),
			Inline = new Group("#inline", "a", Special, FontStyle, Phrase, InlineForms, E.Bdi, E.Mark, E.Wbr, E.Track, E.Time),
			InlineAndTextLevel = new Group("#Inline", PCData, Inline, MiscInline),
			Heading = new Group("#heading", E.H1, E.H2, E.H3, E.H4, E.H5, E.H6),
			Lists = new Group("#lists", E.UL, E.OL, E.DL, E.Menu, E.Dir),
			BlockText = new Group("#blocktext", E.Pre, E.Hr, E.BlockQuote, E.Address, E.Center, E.NoFrames),
			Block = new Group("#block", E.P, Heading, E.Div, E.Section, E.Article, E.Aside, Lists, BlockText, E.IsIndex, E.FieldSet, E.Table, E.Footer, E.Header, E.Hgroup, E.Nav),
			Flow = new Group("#Flow", PCData, Block, E.Form, Inline, Misc, E.Video, E.Audio, E.Embed, E.Details),
			AContent = new Group("#a.content", PCData, Special, FontStyle, Phrase, InlineForms, MiscInline),
			PreContent = new Group("#pre.content", PCData, E.A, SpecialBasic, FontStyleBasic, PhraseBasic, InlineForms, MiscInline),
			FormContent = new Group("#form.content", PCData, Block, Inline, Misc),
			ButtonContent = new Group("#button.content", PCData, E.P, Heading, E.Div, Lists, BlockText,
				E.Table, E.Br, E.Span, E.Bdo, E.ObjectElement, E.Applet, E.Img, E.Map, FontStyle, Phrase, Misc),
			HeadMisc = new Group("#head.misc", E.Script, E.Style, E.Meta, E.Link, E.ObjectElement, E.IsIndex),
			Media = new Group("#special.media", E.Track, E.Source, InlineAndTextLevel);
		#endregion
		private string name;
		private ArrayList members;
		protected Group() {
		}
		public Group(string name, params object[] members) {
			this.name = name;
			this.members = new ArrayList(members);
		}
		public bool IsEmpty {
			get { return this.members != null && this.members.Count == 1 && this.members[0] is EmptyGroup; }
		}
		internal string Name {
			get { return name; }
		}
		public virtual bool Contains(Node node) {
			string nodeName = node.Name;
			for(int i = 0; i < this.members.Count; i++) {
				object member = this.members[i];
				Group g = member as Group;
				if(g != null) {
					if(g.Contains(node))
						return true;
				} else if(nodeName != null && string.Equals((string)nodeName, member as string, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}
	}
	public delegate bool DeclarationRule(DtdElementDeclaration declaration, Stack<Node> stack);
	public class AttList {
		public static readonly AttList
			Attrs,
			AttrsAndTextAlign,
			Cell,
			Col,
			DataCellHAlign,
			Empty,
			HeaderCellHAlign;
		private DtdAttributeDeclaration[] attrDecls;
		static AttList() {
			#region Predefined attribute lists
			Attrs = new AttList(
				DtdAttributeDeclRepository.Attrs, null
			);
			AttrsAndTextAlign = new AttList(
				DtdAttributeDeclRepository.Attrs,
				DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.TextAlign)
			);
			Cell = new AttList(
				DtdAttributeDeclRepository.Attrs,
				DtdAttributeDeclRepository.DataCellHAlign,
				DtdAttributeDeclRepository.GetDecl(A.VerticalAlign, DTD.XHTML, AttrValueType.Enum, false, null, V.CellVAlign),
				DtdAttributeDeclRepository.GetDecl(A.Abbr, DTD.XHTML),
				DtdAttributeDeclRepository.GetDecl(A.Axis, DTD.XHTML),
				DtdAttributeDeclRepository.GetDecl(A.Headers, DTD.XHTML | DTD.HTML5),
				DtdAttributeDeclRepository.GetDecl(A.Scope, DTD.XHTML, AttrValueType.Enum, false, null, V.Scope),
				DtdAttributeDeclRepository.GetDecl(A.RowSpan, DTD.XHTML | DTD.HTML5, AttrValueType.Text, false, V.RowSpanDefault),
				DtdAttributeDeclRepository.GetDecl(A.ColSpan, DTD.XHTML | DTD.HTML5, AttrValueType.Text, false, V.ColSpanDefault),
				DtdAttributeDeclRepository.GetDecl(A.NoWrap, DTD.XHTML, AttrValueType.Flag, false, null),
				DtdAttributeDeclRepository.GetDecl(A.BgColor, DTD.XHTML),
				DtdAttributeDeclRepository.GetDecl(A.Width, DTD.XHTML),
				DtdAttributeDeclRepository.GetDecl(A.Height, DTD.XHTML)
			);
			Col = new AttList(
				DtdAttributeDeclRepository.Attrs,
				DtdAttributeDeclRepository.DataCellHAlign,
				DtdAttributeDeclRepository.GetDecl(A.VerticalAlign, DTD.XHTML, AttrValueType.Enum, false, null, V.CellVAlign),
				DtdAttributeDeclRepository.GetDecl(A.Span, DTD.XHTML | DTD.HTML5),
				DtdAttributeDeclRepository.GetDecl(A.Width, DTD.XHTML)
			);
			DataCellHAlign = new AttList(
				DtdAttributeDeclRepository.DataCellHAlign, null
			);
			Empty = new AttList();
			HeaderCellHAlign = new AttList(
				DtdAttributeDeclRepository.HeaderCellHAlign
			);
			#endregion
		}
		public DtdAttributeDeclaration[] AttributeDeclarations {
			get { return attrDecls; }
		}
		public AttList(params DtdAttributeDeclaration[] attrDecls) {
			if(attrDecls != null) {
				this.attrDecls = new DtdAttributeDeclaration[attrDecls.Length];
				Array.Copy(attrDecls, this.attrDecls, attrDecls.Length);
			}
		}
		public AttList(DtdAttributeDeclaration[] group, params DtdAttributeDeclaration[] attrDecls) {
			int declCount = group.Length;
			if(attrDecls != null)
				declCount += attrDecls.Length;
			this.attrDecls = new DtdAttributeDeclaration[declCount];
			Array.Copy(group, this.attrDecls, group.Length);
			if(attrDecls != null)
				Array.Copy(attrDecls, 0, this.attrDecls, group.Length, attrDecls.Length);
		}
		public AttList(DtdAttributeDeclaration[] group1, DtdAttributeDeclaration[] group2, params DtdAttributeDeclaration[] attrDecls) {
			int declCount = group1.Length + group2.Length;
			if(attrDecls != null)
				declCount += attrDecls.Length;
			this.attrDecls = new DtdAttributeDeclaration[declCount];
			Array.Copy(group1, this.attrDecls, group1.Length);
			Array.Copy(group2, 0, this.attrDecls, group1.Length, group2.Length);
			if(attrDecls != null)
				Array.Copy(attrDecls, 0, this.attrDecls, group1.Length + group2.Length, attrDecls.Length);
		}
	}
	public class DtdAttributeDeclaration {
		private const string Amp = "&amp;";
		private const int AttributeValueExtraCapacity = 16;
		private const int CapturedFragmentCapacity = 10;
		private const string EventHandlerNames =
			"|" + A.OnLoad + "|" + A.OnUnload + "|" + A.OnSubmit + "|" + A.OnReset +
			"|" + A.OnClick + "|" + A.OnDblClick + "|" + A.OnMouseDown + "|" + A.OnMouseUp + "|" + A.OnMouseOver + "|" + A.OnMouseMove + "|" + A.OnMouseOut +
			"|" + A.OnKeyPress + "|" + A.OnKeyDown + "|" + A.OnKeyUp + "|" + A.OnFocus + "|" + A.OnBlur + "|" + A.OnSelect + "|" + A.OnChange + "|";
		private const string JavaScriptProtocol = "javascript:";
		private const string VBScriptProtocol = "vbscript:";
		private string name;
		private AttrValueType valueType;
		private bool isRequired;
		private string defaultValue;
		private string[] possibleValues;
		private bool isPrefix;
		private DTD dtds;
		public DtdAttributeDeclaration(string name, DTD dtds, AttrValueType valueType, bool isRequired, string defaultValue, string[] possibleValues) {
			this.name = name;
			this.valueType = valueType;
			this.isRequired = isRequired;
			this.defaultValue = defaultValue;
			this.possibleValues = possibleValues;
			this.isPrefix = name.EndsWith("-", StringComparison.InvariantCultureIgnoreCase);
			this.dtds = dtds;
		}
		public string DefaultValue {
			get { return defaultValue; }
		}
		public bool IsEventHandler {
			get { return EventHandlerNames.IndexOf('|' + this.name + '|') >= 0; }
		}
		public bool IsHref {
			get { return this.name == A.Href; }
		}
		public bool IsRequired {
			get { return isRequired; }
		}
		public bool IsPrefix {
			get { return isPrefix; }
		}
		public string Name {
			get { return name; }
		}
		public bool CheckDeclaration(AllowedDocumentType documentType) {
			if(documentType == AllowedDocumentType.Both)
				return true;
			DTD dtd = documentType == AllowedDocumentType.HTML5 ? DTD.HTML5 : DTD.XHTML;
			return (this.dtds & dtd) == dtd;
		}
		internal AttrValueType ValueType {
			get { return valueType; }
		}
		internal string[] PossibleValues {
			get { return possibleValues; }
		}
		public static bool ContainsScriptProtocol(string attrValue) {
			return attrValue != null && (attrValue.IndexOf(JavaScriptProtocol, StringComparison.InvariantCultureIgnoreCase) == 0 ||
				attrValue.IndexOf(VBScriptProtocol, StringComparison.InvariantCultureIgnoreCase) == 0);
		}
		public static string GetValidValue(DtdAttributeDeclaration attrDecl, string value) {
			return GetValidValue(attrDecl, value, null);
		}
		public static string GetValidValue(DtdAttributeDeclaration attrDecl, string value, HtmlEditorContentElementFiltering contentFiltering) {
			if(attrDecl.valueType == AttrValueType.Flag)
				return attrDecl.Name;
			if(string.IsNullOrEmpty(value))
				return attrDecl.isRequired ? attrDecl.defaultValue : null;
			else {
				if(contentFiltering != null && contentFiltering.StyleAttributes.Length > 0 && attrDecl.Name.Equals(A.Style, StringComparison.InvariantCultureIgnoreCase)) {
					List<string> styleAttrs = contentFiltering.StyleAttributes.ToList<string>();
					List<string> whiteList = new List<string>();
					foreach(string attrName in styleAttrs) {
						Regex regex = new Regex("\\s*" + attrName + "\\s*:\\s*[^; ]*[; ]", RegexOptions.Singleline);
						if(contentFiltering.StyleAttributeFilterMode == HtmlEditorFilterMode.BlackList)
							value = regex.Replace(value, string.Empty);
						else {
							MatchCollection matches = regex.Matches(value);
							foreach(Match matche in matches)
								whiteList.Add(matche.Value);
						}
					}
					if(contentFiltering.StyleAttributeFilterMode == HtmlEditorFilterMode.WhiteList) {
						value = string.Empty;
						foreach(string listItem in whiteList)
							value += listItem;
					}
				}
				if(attrDecl.valueType == AttrValueType.Text || attrDecl.valueType == AttrValueType.CData)
					return value;
				else if(attrDecl.valueType == AttrValueType.Enum) {
					value = value.ToLowerInvariant();
					for(int i = 0; i < attrDecl.possibleValues.Length; i++) {
						if(string.Equals(value, attrDecl.possibleValues[i], StringComparison.InvariantCulture)) {
							return !attrDecl.isRequired && string.Equals(attrDecl.defaultValue, value, StringComparison.InvariantCulture) ? null : value;
						}
					}
					return attrDecl.isRequired ? attrDecl.defaultValue : null;
				} else
					throw new InvalidOperationException();
			}
		}
	}
	public class DtdHtmlElementDeclaration : DtdElementDeclaration {
		public static readonly DtdHtmlElementDeclaration Instance = new DtdHtmlElementDeclaration();
		private const string NonAutonomousElementNames = "|" + E.Col + "|" + E.ColGroup + "|" + E.TR + "|" + E.TD + "|" + E.TH + "|" +
			E.TBody + "|" + E.THead + "|" + E.TFoot + "|" + E.Caption + "|" + E.LI + "|" + E.DT + "|" + E.DD + "|" + E.Param + "|" +
			E.Option + "|" + E.OptGroup + "|" + E.Legend + "|";
		private DtdHtmlElementDeclaration()
			: base(E.Html, DTD.XHTML | DTD.HTML5, new AttList(
				DtdAttributeDeclRepository.GlobalAttributes,
				DtdAttributeDeclRepository.GetDecl(A.Manifest, DTD.HTML5, AttrValueType.Text)
			), ValidationBehavior.HtmlValidationBehavior,
			Group.Any) {
		}
		public override bool CanContain(Node element) {
			return element.IsPCData || NonAutonomousElementNames.IndexOf('|' + element.Name + '|') < 0;
		}
	}
	public class DtdElementDeclaration {
		private const string FormElementNames = "|" + E.Form + "|" + E.Label + "|" + E.Input + "|" + E.Select + "|" + E.Option + "|" +
			E.OptGroup + "|" + E.Button + "|" + E.TextArea + "|" + E.Datalist + "|" + E.OptGroup + "|" + E.Keygen + "|" + 
			E.Progress + "|" + E.Output + "|";
		public DtdElementDeclaration(string name, DTD dtds, params object[] content)
			: this(name, dtds, AttList.Empty, ValidationBehavior.Valid, new Group(null, content)) { }
		public DtdElementDeclaration(string name, DTD dtds, AttList attList, params object[] content)
			: this(name, dtds, attList, ValidationBehavior.Valid, new Group(null, content)) { }
		public DtdElementDeclaration(string name, DTD dtds, ValidationBehavior validationBehavior, params object[] content)
			: this(name, dtds, AttList.Empty, validationBehavior, new Group(null, content)) { }
		public DtdElementDeclaration(string name, DTD dtds, AttList attList, ValidationBehavior validationBehavior, params object[] content)
			: this(name, dtds, attList, validationBehavior, new Group(null, content)) { }
		private DtdElementDeclaration(string name, DTD dtds, AttList attList, ValidationBehavior validationBehavior, Group content) {
			Name = name;
			Content = content;
			ValidationBehavior = validationBehavior;
			AttList = attList;
			DTDs = dtds;
		}
		protected internal virtual bool SpecialValidation(Stack<Node> stack, DtdElementDeclaration declaration) {
			return true;
		}
		public Group Content { get; private set; }
		public DTD DTDs { get; private set; }
		public AttList AttList { get; private set; }
		public bool IsEmpty {
			get { return Content.IsEmpty; }
		}
		public bool IsScript {
			get { return Name == E.Script; }
		}
		public bool IsIFrame {
			get { return Name == E.IFrame; }
		}
		public bool IsYouTubeIFrame(Node node) {
			if(IsIFrame) {
				Attribute attr = node.GetAttribute("src");
				if(attr != null && !string.IsNullOrEmpty(attr.Value))
					return Regex.Matches(attr.Value, "((youtube(-nocookie)?\\.com\\/((watch\\?v\\=|embed\\/|v\\/)))|(youtu\\.be\\/))", RegexOptions.Singleline).Count > 0;
			}
			return false;
		}
		public bool IsFormElement {
			get { return FormElementNames.IndexOf('|' + Name + '|') >= 0; }
		}
		public bool IsHTML5MediaElement {
			get { return (Name == E.Video || Name == E.Audio || Name == E.Embed || Name == E.Source || Name == E.Track); }
		}
		public bool IsWholeRegimeDocument {
			get { return (Name == E.Head || Name == E.Title || Name == E.BaseElement || Name == E.Meta || Name == E.Style || Name == E.Link || Name == E.Body); }
		}
		public bool IsObjectElement {
			get { return Name == E.ObjectElement; }
		}
		public bool IsEmbedElement {
			get { return Name == E.Embed; }
		}
		public string Name { get; private set; }
		public ValidationBehavior ValidationBehavior { get; private set; }
		public virtual bool CanContain(Node element) {
			return Content.Contains(element);
		}
	}
	public class ValidationBehavior {
		#region Specific Validation Behaviors
		sealed class _UElementValidationBehavior : ValidationBehavior {
			public _UElementValidationBehavior()
				: base(ElementType.Deprecated, CorrectionBehavior.None) {
			}
			public override string NewNodeName {
				get { return E.Span; }
			}
			public override void TransformNode(Node node) {
				node.Rename(E.Span);
				AppendOrCreateStyleAttribute(node, "text-decoration: underline;");
			}
		}
		static bool TagIsAlone(Stack<Node> stack, Node node) {
			if(node.ParentNode == null || node.ParentNode.ChildNodes == null)
				return true;
			return node.ParentNode.ChildNodes.Where(t => t != null).Count(t => string.Equals(t.Name, node.Name)) == 0;
		}
		static bool AttributeIsAlone(Node node, string attributeName) {
			if(node.attributes.items == null){
				return false;
			}
			return node.attributes.items.Where(t => t != null).Count(t => t.Name == attributeName) == 1;
		}
		static bool TagHasAttribute(Node node, string attributeName) {
			return node.GetAttribute(attributeName) != null;
		}
		static bool TagHasReqiredAttribute(Stack<Node> stack, Node node) {
			return true;
		}
		static bool TagLocatedBehaind(Stack<Node> stack, string firstNodeName, string secondNodeName) {
			return GetIndexByName(stack.items, firstNodeName) == GetIndexByName(stack.items, secondNodeName) - 1;
		}
		static int GetIndexByName(Node[] stackNodes, string declarationName) {
			for(int i = 0; i < stackNodes.Length; i++) {
				if(stackNodes[i].Name == declarationName) {
					return i;
				}
			}
			return -1;
		}
		protected internal virtual bool IsTagValid(Stack<Node> stack, Node node) {
			return true;
		}
		sealed class _HtmlValidationBehavior : ValidationBehavior {
			public _HtmlValidationBehavior()
				: base(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTagAndContent) {
			}
			protected internal override bool IsTagValid(Stack<Node> stack, Node node) {
				return TagIsAlone(stack, node);
			}
		}
		sealed class _TitleValidationBehavior : ValidationBehavior {
			public _TitleValidationBehavior()
				: base(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTag) {
			}
			protected internal override bool IsTagValid(Stack<Node> stack, Node node) {
				return TagIsAlone(stack, node);
			}
		}
		sealed class _NoScriptValidationBehavior : ValidationBehavior {
			public _NoScriptValidationBehavior()
				: base(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTagAndContent) {
			}
			protected internal override bool IsTagValid(Stack<Node> stack, Node node) {
				return TagIsAlone(stack, node);
			}
		}
		sealed class _HeadValidationBehavior : ValidationBehavior {
			public _HeadValidationBehavior()
				: base(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTagAndContent) {
			}
			protected internal override bool IsTagValid(Stack<Node> stack, Node node) {
				return TagIsAlone(stack, node);
			}
		}
		sealed class _BaseTagValidationBehavior : ValidationBehavior {
			public _BaseTagValidationBehavior()
				: base(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTag) {
			}
			protected internal override bool IsTagValid(Stack<Node> stack, Node node) {
				bool hasRequiredAttributes = AttributeIsAlone(node, A.Href) || AttributeIsAlone(node, A.Target);
				return hasRequiredAttributes && TagIsAlone(stack, node);
			}
		}
		sealed class LinkTagValidationBehavior : ValidationBehavior {
			public LinkTagValidationBehavior()
				: base(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTag) {
			}
			protected internal override bool IsTagValid(Stack<Node> stack, Node node) {
				bool hasRequiredAttributes = AttributeIsAlone(node, A.Href) && AttributeIsAlone(node, A.Target);
				return hasRequiredAttributes && TagIsAlone(stack, node);
			}
		}
		sealed class _MetaTagValidationBehavior : ValidationBehavior {
			public _MetaTagValidationBehavior()
				: base(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTag) {
			}
			protected internal override bool IsTagValid(Stack<Node> stack, Node node) {
				bool hasName = TagHasAttribute(node, A.Name);
				bool hasHttpEquiv = TagHasAttribute(node, A.HttpEquiv);
				bool hasContent = TagHasAttribute(node, A.Content);
				bool hasCharset = TagHasAttribute(node, A.Charset);
				bool hasAnoverMetaTagsWithCharset = false;
				if((hasName || hasHttpEquiv) && !hasContent)
					return false;
				else
					if(!(hasName || hasHttpEquiv) && hasContent)
						return false;
				if(hasCharset && node.ParentNode != null) {
					hasAnoverMetaTagsWithCharset = node.ParentNode.ChildNodes.Where(t => t != null && string.Equals(t.Name, node.Name) && t.GetAttribute(A.Charset) != null).Count() != 0;
					if(hasAnoverMetaTagsWithCharset)
						return false;
				}
				if(!(hasName || hasCharset || hasHttpEquiv))
					return false;
				if(hasName && (hasCharset || hasHttpEquiv))
					return false;
				if(hasCharset && (hasName || hasHttpEquiv))
					return false;
				if(hasHttpEquiv && (hasCharset || hasName))
					return false;
				return true;
			}
		}
		sealed class _SAndStrikeElementsValidationBehavior : ValidationBehavior {
			public _SAndStrikeElementsValidationBehavior()
				: base(ElementType.Deprecated, CorrectionBehavior.None) {
			}
			public override string NewNodeName {
				get { return E.Span; }
			}
			public override void TransformNode(Node node) {
				node.Rename(E.Span);
				AppendOrCreateStyleAttribute(node, "text-decoration: line-through;");
			}
		}
		sealed class _FontElementValidationBehavior : ValidationBehavior {
			private const int BaseFontNondimensionalSize = 3;
			private static readonly string[] PixelSizes = new string[] { "10px", "13px", "16px", "18px", "24px", "32px", "48px" };
			public _FontElementValidationBehavior()
				: base(ElementType.Deprecated, CorrectionBehavior.None) {
			}
			public override string NewNodeName {
				get { return E.Span; }
			}
			public override void TransformNode(Node node) {
				node.Rename(E.Span);
				Attribute size = node.GetAttribute(A.Size);
				Attribute family = node.GetAttribute(A.Face);
				Attribute color = node.GetAttribute(A.Color);
				string sizeResultStr = null;
				string familyResultStr = null;
				string colorResultStr = null;
				if(size != null && !string.IsNullOrEmpty(size.Value)) {
					string sizeStrValue = size.Value;
					int sizeNumValue;
					char firstSizeValueChar = sizeStrValue[0];
					if(firstSizeValueChar == '+' || firstSizeValueChar == '-') {
						int increment;
						sizeNumValue = BaseFontNondimensionalSize;
						if(Int32.TryParse(sizeStrValue.Substring(1), out increment))
							sizeNumValue += (firstSizeValueChar == '+') ? increment : -increment;
					} else {
						if(!Int32.TryParse(sizeStrValue, out sizeNumValue))
							sizeNumValue = BaseFontNondimensionalSize;
					}
					if(sizeNumValue <= 0)
						sizeNumValue = 1;
					if(sizeNumValue > 7)
						sizeNumValue = 7;
					sizeResultStr = PixelSizes[sizeNumValue - 1];
					node.RemoveAttribute(size);
				}
				if(family != null && !string.IsNullOrEmpty(family.Value)) {
					familyResultStr = family.Value;
					node.RemoveAttribute(family);
				}
				if(color != null && !string.IsNullOrEmpty(color.Value)) {
					colorResultStr = color.Value;
					node.RemoveAttribute(color);
				}
				StringBuilder styleAttrValue = new StringBuilder();
				if(sizeResultStr != null) {
					if(familyResultStr != null) {
						styleAttrValue.Append("font: ");
						styleAttrValue.Append(sizeResultStr);
						styleAttrValue.Append(' ');
						styleAttrValue.Append(familyResultStr);
					} else {
						styleAttrValue.Append("font-size: ");
						styleAttrValue.Append(sizeResultStr);
					}
					styleAttrValue.Append(';');
				} else {
					if(familyResultStr != null) {
						styleAttrValue.Append("font-family: ");
						styleAttrValue.Append(familyResultStr);
						styleAttrValue.Append(';');
					}
				}
				if(colorResultStr != null) {
					if(styleAttrValue.Length > 0)
						styleAttrValue.Append(' ');
					styleAttrValue.Append("color: ");
					styleAttrValue.Append(colorResultStr);
					styleAttrValue.Append(';');
				}
				AppendOrCreateStyleAttribute(node, styleAttrValue.ToString());
			}
		}
		sealed class _CenterElementValidationBehavior : ValidationBehavior {
			public _CenterElementValidationBehavior()
				: base(ElementType.Deprecated, CorrectionBehavior.None) {
			}
			public override string NewNodeName {
				get { return E.Div; }
			}
			public override void TransformNode(Node node) {
				node.Rename(E.Div);
				AppendOrCreateStyleAttribute(node, "text-align: center;");
			}
		}
		sealed class _IElementValidationBehavior : ValidationBehavior {
			public _IElementValidationBehavior()
				: base(ElementType.Replaceable, CorrectionBehavior.None) {
			}
			public override string NewNodeName {
				get { return E.Em; }
			}
			public override void TransformNode(Node node) {
				node.Rename(E.Em);
			}
		}
		sealed class _BElementValidationBehavior : ValidationBehavior {
			public _BElementValidationBehavior()
				: base(ElementType.Replaceable, CorrectionBehavior.None) {
			}
			public override string NewNodeName {
				get { return E.Strong; }
			}
			public override void TransformNode(Node node) {
				node.Rename(E.Strong);
			}
		}
		#endregion
		public static readonly ValidationBehavior
			Valid = new ValidationBehavior(ElementType.Valid, CorrectionBehavior.None),
			ForbiddenTag = new ValidationBehavior(ElementType.Forbidden, CorrectionBehavior.SkipTag),
			ForbiddenTagAndItsContent = new ValidationBehavior(ElementType.Forbidden, CorrectionBehavior.SkipTagAndContent),
			OptionallyForbiddenTag = new ValidationBehavior(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTag),
			OptionallyForbiddenTagAndItsContent = new ValidationBehavior(ElementType.OptionallyForbidden, CorrectionBehavior.SkipTagAndContent),
			UElementValidationBehavior = new _UElementValidationBehavior(),
			SAndStrikeElementsValidationBehavior = new _SAndStrikeElementsValidationBehavior(),
			FontElementValidationBehavior = new _FontElementValidationBehavior(),
			CenterElementValidationBehavior = new _CenterElementValidationBehavior(),
			IElementValidationBehavior = new _IElementValidationBehavior(),
			HtmlValidationBehavior = new _HtmlValidationBehavior(),
			NoScriptValidationBehavior = new _NoScriptValidationBehavior(),
			HeadValidationBehavior = new _HeadValidationBehavior(),
			TitleValidationBehavior = new _TitleValidationBehavior(),
			BaseTagValidationBehavior = new _BaseTagValidationBehavior(),
			MetaTagValidationBehavior = new _MetaTagValidationBehavior(),
			BElementValidationBehavior = new _BElementValidationBehavior();
		private ElementType elementType;
		private CorrectionBehavior correctionBehavior;
		internal ValidationBehavior(ElementType elementType, CorrectionBehavior correctionBehavior) {
			this.elementType = elementType;
			this.correctionBehavior = correctionBehavior;
		}
		public ElementType ElementType {
			get { return elementType; }
		}
		public CorrectionBehavior CorrectionBehavior {
			get { return correctionBehavior; }
		}
		public virtual string NewNodeName {
			get { return null; }
		}
		public virtual void TransformNode(Node node) {
			throw new NotSupportedException("Not supported by the base ValidationBehavior. Expected to be implemented in derived types.");
		}
		internal static void AppendOrCreateStyleAttribute(Node node, string styleAttrValue) {
			Attribute styleAttr = node.GetAttribute(A.Style);
			if(styleAttr != null) {
				string existentStyleAttrValue = styleAttr.Value.Trim();
				if(existentStyleAttrValue.Length > 0 && existentStyleAttrValue[existentStyleAttrValue.Length - 1] != ';')
					existentStyleAttrValue += ';';
				styleAttrValue = existentStyleAttrValue + " " + styleAttrValue;
				styleAttr.Assign(A.Style, styleAttrValue);
			}
			node.AddAttribute(A.Style, styleAttrValue);
		}
	}
	public static class DtdAttributeDeclRepository {
		public static readonly DtdAttributeDeclaration[]
			Attrs, CoreAttrs, DataCellHAlign, Events, FormEvents, InputEvents, Focus, HeaderCellHAlign, I18n, MediaEvents, GlobalAttributes;
		internal static Dictionary<int, DtdAttributeDeclaration> attrDecls = new Dictionary<int, DtdAttributeDeclaration>();
		static DtdAttributeDeclRepository() {
			#region Predefined attribute declaration lists
			GlobalAttributes = new DtdAttributeDeclaration[]{
				GetDecl(A.AccessKey, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Class, DTD.XHTML | DTD.HTML5),
				GetDecl(A.ContentEditable, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Dir, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Hidden, DTD.XHTML | DTD.HTML5),
				GetDecl(A.ID, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Lang, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Spellcheck, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Style, DTD.XHTML | DTD.HTML5),
				GetDecl(A.TabIndex, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Title, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Translate, DTD.XHTML | DTD.HTML5)
			};
			CoreAttrs = new DtdAttributeDeclaration[] {
				GetDecl(A.ID, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Class, DTD.XHTML | DTD.HTML5),
				GetDecl(A.Style, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.Title, DTD.XHTML | DTD.HTML5),
				GetDecl(A.AccessKey, DTD.XHTML | DTD.HTML5),
				GetDecl(A.TabIndex, DTD.XHTML | DTD.HTML5, AttrValueType.Text, false, null),
				GetDecl(A.ContextMenu, DTD.HTML5),
				GetDecl(A.Draggable, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] { "true", "false", "auto" }),
				GetDecl(A.Dropzone, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] { "copy", "move", "link" }),
				GetDecl(A.Hidden, DTD.HTML5, AttrValueType.Flag),
				GetDecl(A.Spellcheck, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] { "true", "false" }),
				GetDecl(A.DataPrefix, DTD.HTML5),
				GetDecl(A.ContentEditable, DTD.HTML5)
			};
			DataCellHAlign = new DtdAttributeDeclaration[] {
				GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.CellHAlign),
				GetDecl(A.Char, DTD.XHTML),
				GetDecl(A.CharOffset, DTD.XHTML)
			};
			HeaderCellHAlign = new DtdAttributeDeclaration[] {
				GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.CellHAlign),
				GetDecl(A.Char, DTD.XHTML),
				GetDecl(A.CharOffset, DTD.XHTML)
			};
			I18n = new DtdAttributeDeclaration[] {
				GetDecl(A.Lang, DTD.XHTML | DTD.HTML5),
				GetDecl(A.XmlLang, DTD.XHTML),
				GetDecl(A.Dir, DTD.XHTML | DTD.HTML5, AttrValueType.Enum, false, null, V.Direction)
			};
			Events = new DtdAttributeDeclaration[] {
				GetDecl(A.OnClick, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDblClick, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDrag, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDragEnd, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDragEnter, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDragLeave, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDragOver, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDragStart, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDrop, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnMouseDown, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnMouseUp, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnMouseOver, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnMouseMove, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnMouseOut, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnMouseWheel, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnScroll, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnKeyPress, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnKeyDown, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnKeyUp, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
			};
			MediaEvents = new DtdAttributeDeclaration[] {
				GetDecl(A.OnCanPlay, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnCanPlayThrough, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnDurationChange, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnEmptied, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnEnded, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnError, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnLoadeddata, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnLoadedmetadata, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnLoadstart, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnPause, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnPlay, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnPlaying, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnProgress, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnRatechange, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnReadystatechange, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnSeeked, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnSeeking, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnStalled, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnSuspend, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnTimeupdate, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnVolumechange, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnWaiting, DTD.HTML5, AttrValueType.CData, false, null),
			};
			InputEvents = new DtdAttributeDeclaration[] {
				GetDecl(A.OnBlur, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnChange, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnContextMenu, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnFocus, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnFormChange, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnFormInput, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnInput, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnInvalid, DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnReset, DTD.XHTML, AttrValueType.CData, false, null),
				GetDecl(A.OnSelect, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnSubmit, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null)
			};
			Focus = new DtdAttributeDeclaration[] {
				GetDecl(A.OnFocus, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null),
				GetDecl(A.OnBlur, DTD.XHTML | DTD.HTML5, AttrValueType.CData, false, null)
			};
			FormEvents = new DtdAttributeDeclaration[Focus.Length + InputEvents.Length];
			Array.Copy(Focus, FormEvents, Focus.Length);
			Array.Copy(InputEvents, 0, FormEvents, Focus.Length, InputEvents.Length);
			Attrs = new DtdAttributeDeclaration[CoreAttrs.Length + I18n.Length + Events.Length];
			Array.Copy(CoreAttrs, Attrs, CoreAttrs.Length);
			Array.Copy(I18n, 0, Attrs, CoreAttrs.Length, I18n.Length);
			Array.Copy(Events, 0, Attrs, CoreAttrs.Length + I18n.Length, Events.Length);
			#endregion
		}
		public static DtdAttributeDeclaration GetDecl(string name, DTD dtds) {
			return GetDecl(name, dtds, AttrValueType.Text, false, null, null);
		}
		public static DtdAttributeDeclaration GetDecl(string name, DTD dtds, AttrValueType valueType) {
			return GetDecl(name, dtds, valueType, false, null, null);
		}
		public static DtdAttributeDeclaration GetDecl(string name, DTD dtds, AttrValueType valueType, bool isRequired, string defaultValue) {
			return GetDecl(name, dtds, valueType, isRequired, defaultValue, null);
		}
		public static DtdAttributeDeclaration GetDecl(string name, DTD dtds, AttrValueType valueType, bool isRequired, string defaultValue, string[] possibleValues) {
			int hashCode = GetAttrDeclHashCode(name, dtds, valueType, isRequired, defaultValue, possibleValues);
			DtdAttributeDeclaration decl;
			if(!attrDecls.TryGetValue(hashCode, out decl)) {
				lock(attrDecls) {
					if(!attrDecls.TryGetValue(hashCode, out decl)) {
						decl = new DtdAttributeDeclaration(name, dtds, valueType, isRequired, defaultValue, possibleValues);
						attrDecls.Add(hashCode, decl);
					}
				}
			}
			return decl;
		}
		internal static int GetAttrDeclHashCode(string name, DTD dtds, AttrValueType valueType, bool isRequired, string defaultValue, string[] possibleValues) {
			int hashCode = name.GetHashCode() ^ ((int)valueType + (isRequired ? 0 : 2));
			if(defaultValue != null)
				hashCode ^= defaultValue.GetHashCode();
			if(possibleValues != null)
				for(int i = 0; i < possibleValues.Length; i++)
					hashCode ^= possibleValues[i].GetHashCode();
			hashCode ^= dtds.GetHashCode();
			return hashCode;
		}
	}
}
