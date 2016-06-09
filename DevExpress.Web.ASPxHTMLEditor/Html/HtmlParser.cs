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
using System.IO;
using System.Xml;
using System.Linq;
using DevExpress.Web.Internal;
using System.Web;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class HtmlParser : XmlReader, IHtmlScannerContext {
		internal enum State { Initial, Markup, Text, CData, PseudoStartTagOpened, InnerTag, InvalidNesting, Attribute, AttributeValue, EndTag, EndOfFile }
		internal enum CDataType { Style = 1, Script = 2 }
		internal const int NodeStackGrowth = 10;
		private const int RequiredAttributesPresenceListInitialCapacity = 64;
		private StringReader inputReader;
		private bool skipWhitespaceNodes;
		internal HtmlScanner scanner;
#if !DebugTest
		private State state;
#else
		private State stateCore;
		internal virtual State state { get { return stateCore; } set { stateCore = value; } }
#endif
		internal Stack<Node> stack;
		internal Node node;
		private char pseudoStartTagFirstInnerChar;
		internal string parsedEndTagName;
		internal CDataType currentCDataType;
		internal bool isCDataParsed;
		internal Node invalidNestedNode;
		internal int validNestingDepth;
		private Stack endTagTransformQueries;
		internal bool skippingNodeWithContent;
		private int depthToSkipTo;
		internal RequiredAttributePresenceList requiredAttributePresenceList;
		internal Attribute attribute;
		internal int attributeIndex;
		internal State savedState;
		private XmlNameTable nameTable;
		public HtmlParser() {
			Initialize();
			this.scanner = new HtmlScanner(this);
		}
		public int AbsolutePosition {
			get { return this.scanner.AbsolutePosition; }
		}
		public StringReader InputReader {
			get { return this.inputReader; }
			set {
				this.inputReader = value;
				OnInitialize();
			}
		}
		public IHtmlValidationSettings HtmlValidationSettings { get; set; }
		public bool SkipWhitespaceNodes {
			get { return skipWhitespaceNodes; }
			set { skipWhitespaceNodes = value; }
		}
		public override bool Read() {
			bool parsed = false;
			while(!parsed) {
				switch(this.state) {
					case State.Initial:
						this.scanner.ScanChar();
						this.state = State.Markup;
						goto case State.Markup;
					case State.Markup:
						if(this.node.IsEmpty)
							Pop();
						parsed = ParseMarkup();
						break;
					case State.Text:
						Pop();
						goto case State.Markup;
					case State.CData:
						if(this.isCDataParsed) {
							Pop();
							goto case State.Markup;
						}
						parsed = ParseCData(this.currentCDataType);
						break;
					case State.PseudoStartTagOpened:
						parsed = ParseText(this.pseudoStartTagFirstInnerChar);
						break;
					case State.InnerTag:
						XmlNodeType topNodeType = this.stack.TopItemUnsafe.Type;
						if(topNodeType == XmlNodeType.Text || topNodeType == XmlNodeType.Whitespace)
							Pop();
						this.state = State.Markup;
						parsed = ParseTag(this.scanner.LastChar);
						break;
					case State.InvalidNesting:
						if(this.stack.Count <= this.validNestingDepth) {
							if(this.invalidNestedNode != null) {
								Push(this.invalidNestedNode);
								XmlNodeType invalidNestedNodeType = this.invalidNestedNode.Type;
								this.state = invalidNestedNodeType == XmlNodeType.Text ? this.state = State.InnerTag : State.Markup;
								parsed = true;
								break;
							} else {
								this.state = State.Markup;
								parsed = false;
								break;
							}
						} else {
							bool isSkipppingBeforePop = this.skippingNodeWithContent;
							Pop();
							parsed = !isSkipppingBeforePop;
						}
						break;
					case State.Attribute:
					case State.AttributeValue:
						this.state = State.Markup;
						goto case State.Markup;
					case State.EndTag:
						if(this.parsedEndTagName == this.node.Name) {
							Pop();
							this.state = State.Markup;
							goto case State.Markup;
						}
						Pop();
						parsed = true;
						break;
					case State.EndOfFile:
						Close();
						return false;
					default:
						throw new InvalidOperationException();
				}
				if(parsed && (this.skippingNodeWithContent || this.node.Type == XmlNodeType.Whitespace && this.skipWhitespaceNodes))
					parsed = false;
			}
			return true;
		}
		protected virtual void Initialize() {
			this.state = State.Initial;
			this.stack = new Stack<Node>(NodeStackGrowth);
			this.node = Push(E.Document, XmlNodeType.Document, null);
			this.node.IsEmpty = false;
			this.pseudoStartTagFirstInnerChar = '\0';
			this.parsedEndTagName = null;
			this.currentCDataType = 0;
			this.isCDataParsed = false;
			this.invalidNestedNode = null;
			this.validNestingDepth = -1;
			this.endTagTransformQueries = new Stack();
			this.skippingNodeWithContent = false;
			this.depthToSkipTo = 0;
			this.requiredAttributePresenceList = new RequiredAttributePresenceList(RequiredAttributesPresenceListInitialCapacity);
			this.attribute = null;
			this.attributeIndex = 0;
			this.savedState = State.Initial;
			this.nameTable = new NameTable();
		}
		private Node Push(Node node) {
			Node newNode = Push(null, XmlNodeType.None, null);
			return this.node = node.CopyTo(newNode);
		}
		internal Node Push(string name, XmlNodeType type, string value) {
			Node node = this.stack.PushSlot();
			if(node == null)
				this.stack.TopItemUnsafe = node = new Node();
			node.Assign(name, type, value);
			return this.node = node;
		}
		internal void Pop() {
			if(this.stack.Count > 1) {
				this.node = this.stack.Pop();
				if(this.skippingNodeWithContent && this.stack.Count == this.depthToSkipTo)
					this.skippingNodeWithContent = false;
			}
		}
		#region Parse Methods
		internal bool ParseMarkup() {
			char ch = this.scanner.LastChar;
			if(ch == '<') {
				ch = this.scanner.ScanChar();
				return ParseTag(ch);
			} else if(ch != HtmlScanner.EOF) {
				string nodeName = this.node.Name.ToUpperInvariant();
				if(nodeName == "SCRIPT") {
					this.state = State.CData;
					this.isCDataParsed = false;
					this.currentCDataType = CDataType.Script;
					return false;
				} else if(nodeName == "STYLE") {
					this.state = State.CData;
					this.isCDataParsed = false;
					this.currentCDataType = CDataType.Style;
					return false;
				} else {
					return ParseText(ch);
				}
			}
			this.state = State.EndOfFile;
			return false;
		}
		internal bool ParseTag(char ch) {
			bool parsed = false;
			if(ch == '%') {
				parsed = ParseAspNetDirective();
			} else if(ch == '!') {
				ch = this.scanner.ScanChar();
				if(ch == '-')
					parsed = ParseComment();
				else if(ch == '[')
					parsed = ParseConditionalExpression();
				else
					this.scanner.ScanTo('>');
			} else if(ch == '?') {
				this.scanner.ScanChar();
				parsed = ParseProcessingInstruction();
			} else if(ch == '/') {
				parsed = ParseEndTag();
			} else {
				parsed = ParseStartTag(ch);
			}
			return parsed;
		}
		internal virtual bool ParseStartTag(char ch) {
			if(HtmlScanner.TagTerminators.IndexOf(ch) >= 0) {
				this.pseudoStartTagFirstInnerChar = (ch != '<') ? ch : '\0';
				this.state = State.PseudoStartTagOpened;
				return false;
			}
			string name = this.scanner.ScanNameToken(HtmlScanner.TagTerminators).ToLowerInvariant();
			Node node = Push(name, XmlNodeType.Element, null);
			node.IsEmpty = false;
			DtdElementDeclaration elementDecl = DtdXhtml10Trans.FindElementDecl(name, HtmlValidationSettings.AllowedDocumentType, HtmlValidationSettings.AllowHTML5MediaElements, HtmlValidationSettings.AllowObjectAndEmbedElements, HtmlValidationSettings.ContentElementFiltering);
			ch = this.scanner.SkipWhitespaces();
			this.requiredAttributePresenceList.Clear();
			while(ch != '>' && ch != HtmlScanner.EOF) {
				if(ch == '/') {
					this.node.IsEmpty = true;
					ch = this.scanner.ScanChar();
					ch = this.scanner.SkipWhitespaces();
					if(ch != '>') {
						this.scanner.ScanTo('>');
						return false;
					}
					break;
				} else if(ch == '<')
					break;
				string attrName = this.scanner.ScanNameToken(HtmlScanner.AttributeNameTerminators).ToLowerInvariant();
				if(attrName.Length == 0) {
					ch = this.scanner.ScanChar();
					continue;
				}
				ch = this.scanner.SkipWhitespaces();
				string attrValue = null;
				char quote = '\0';
				if(ch == '=' || ch == '\'' || ch == '\"') {
					if(ch == '=') {
						this.scanner.ScanChar();
						ch = this.scanner.SkipWhitespaces();
					}
					if(ch == '\'' || ch == '\"') {
						quote = ch;
						ch = this.scanner.ScanChar();
						this.scanner.SkipWhitespaces();
						attrValue = this.scanner.ScanAttributeValue(quote);
					} else if(ch != '>') {
						this.scanner.SkipWhitespaces();
						attrValue = this.scanner.ScanToken(HtmlScanner.AttributeValueTerminators);
					}
				}
				if(elementDecl != null && attrName.Length > 0) {
					Attribute attr = node.AddAttribute(attrName, attrValue);
					if(attr != null) {
						PrepareAttribute(elementDecl, attr);
						if(!ValidateAttribute(elementDecl, attr, HtmlValidationSettings.ContentElementFiltering))
							node.RemoveAttribute(attr);
					}
				}
				ch = this.scanner.SkipWhitespaces();
			}
			if(elementDecl != null)
				SetUpRequiredAttributes(node, elementDecl);
			if(ch == '>')
				this.scanner.ScanChar();
			bool isValid = ValidateNode(node, elementDecl);
			if(isValid)
				AddNodeToParentChildList(node);
			return isValid;
		}
		internal void PrepareAttribute(DtdElementDeclaration decl, Attribute attr) {
			if(HtmlValidationSettings.ResourcePathMode != ResourcePathMode.NotSet) {
				string attributeName = GetResourceAttributeName(decl);
				if(!string.IsNullOrEmpty(attributeName) && attr.Name == attributeName)
					attr.Value = UrlProcessor.PrepareUrl(HtmlValidationSettings.ResourcePathMode, attr.Value);
			}
		}
		string GetResourceAttributeName(DtdElementDeclaration decl) {
			switch(decl.Name) {
				case E.Video:
				case E.Audio:
				case E.Embed:
				case E.Img:
				case E.IFrame:
					return A.Src;
				case E.A:
					return A.Href;
				case E.ObjectElement:
					return A.Data;
			}
			return null;
		}
		internal bool ValidateAttribute(DtdElementDeclaration decl, Attribute attr) {
			return ValidateAttribute(decl, attr, null);
		}
		internal bool ValidateAttribute(DtdElementDeclaration decl, Attribute attr, HtmlEditorContentElementFiltering contentFiltering) {
			if(decl.AttList.AttributeDeclarations != null) {
				for(int i = 0; i < decl.AttList.AttributeDeclarations.Length; i++) {
					DtdAttributeDeclaration attrDecl = decl.AttList.AttributeDeclarations[i];
					if((attrDecl.Name == attr.Name || attrDecl.IsPrefix && attr.Name.StartsWith(attrDecl.Name)) &&
						(attrDecl.CheckDeclaration(HtmlValidationSettings.AllowedDocumentType) ||
							decl.IsHTML5MediaElement && HtmlValidationSettings.AllowHTML5MediaElements ||
							decl.IsWholeRegimeDocument && HtmlValidationSettings.AllowEditFullDocument ||
							HtmlValidationSettings.AllowObjectAndEmbedElements && (decl.IsObjectElement || decl.IsEmbedElement)
							)
						) {
						this.requiredAttributePresenceList[i] = true;
						if(!ValidateAttributeValidationSettings(attrDecl, attr, contentFiltering))
							return false;
						attr.Value = DtdAttributeDeclaration.GetValidValue(attrDecl, attr.Value, contentFiltering);
						if(attr.Value != null)
							return true;
					}
				}
			}
			return false;
		}
		private bool ValidateAttributeValidationSettings(DtdAttributeDeclaration attrDecl, Attribute attr, HtmlEditorContentElementFiltering contentFiltering) {
			if(!HtmlValidationSettings.AllowScripts && (attrDecl.IsEventHandler || attrDecl.IsHref && DtdAttributeDeclaration.ContainsScriptProtocol(attr.Value)))
				return false;
			if(!HtmlValidationSettings.AllowIdAttributes && attrDecl.Name.Equals(A.ID, StringComparison.InvariantCultureIgnoreCase))
				return false;
			if(!HtmlValidationSettings.AllowStyleAttributes && attrDecl.Name.Equals(A.Style, StringComparison.InvariantCultureIgnoreCase))
				return false;
			if(contentFiltering != null && contentFiltering.Attributes.Length > 0) {
				List<string> attrs = contentFiltering.Attributes.ToList<string>();
				foreach(string attrName in attrs) {
					if(attr.Name.Equals(attrName, StringComparison.InvariantCultureIgnoreCase))
						return contentFiltering.AttributeFilterMode == HtmlEditorFilterMode.WhiteList;
				}
				if(contentFiltering.AttributeFilterMode == HtmlEditorFilterMode.WhiteList)
					return false;
			}
			return true;
		}
		private void SetUpRequiredAttributes(Node node, DtdElementDeclaration decl) {
			DtdAttributeDeclaration[] attrDecls = decl.AttList.AttributeDeclarations;
			if(attrDecls == null)
				return;
			for(int i = 0; i < attrDecls.Length; i++) {
				if(!this.requiredAttributePresenceList[i]) {
					DtdAttributeDeclaration attrDecl = attrDecls[i];
					if(attrDecl.IsRequired)
						node.AddAttribute(attrDecl.Name, attrDecl.DefaultValue);
				}
			}
		}
		internal virtual bool ParseEndTag() {
			this.state = State.EndTag;
			this.scanner.ScanChar();
			this.scanner.SkipWhitespaces();
			string name = this.scanner.ScanNameToken(HtmlScanner.TagTerminators).ToLowerInvariant();
			char ch = this.scanner.SkipWhitespaces();
			if(ch != '>')
				this.scanner.ScanTo('>');
			else
				this.scanner.ScanChar();
			if(this.endTagTransformQueries.Count > 0) {
				EndTagTransformQuery query = (EndTagTransformQuery)this.endTagTransformQueries.Peek();
				if(query.Depth == this.stack.Count) {
					name = query.NewTagName;
					this.endTagTransformQueries.Pop();
				}
			}
			this.parsedEndTagName = name;
			this.node = this.stack.TopItemUnsafe;
			for(int i = this.stack.Count - 1; i > 1; i--) {
				if(name == this.stack[i].Name)
					return true;
			}
			this.state = State.Markup;
			return false;
		}
		internal virtual bool ParseText(char ch) {
			StringBuilder sb = new StringBuilder();
			bool isWhitespace = HtmlScanner.IsWhitespace(ch);
			if(this.state == State.PseudoStartTagOpened) {
				isWhitespace = false;
				sb.Append('<');
				if(ch != '\0') {
					if(ch != '&') {
						sb.Append(ch);
						ch = this.scanner.ScanChar();
					}
				} else
					ch = '<';
			}
			this.state = State.Text;
			while(ch != HtmlScanner.EOF) {
				if(ch == '<') {
					ch = this.scanner.ScanChar();
					if(ch == '/' || char.IsLetter(ch) || ch == '!' || ch == '?' || ch == '%') {
						this.state = State.InnerTag;
						break;
					} else {
						isWhitespace = false;
						sb.Append("<");
						if(ch != '<') {
							if(ch == '&')
								sb.Append("&amp;");
							else
								sb.Append(ch);
							ch = this.scanner.ScanChar();
						}
					}
				} else if(ch == '&') {
					string entityReference = this.scanner.ScanEntityReference();
					if(!string.IsNullOrEmpty(entityReference))
						isWhitespace = false;
					sb.Append(entityReference);
					ch = this.scanner.LastChar;
				} else {
					if(isWhitespace && !HtmlScanner.IsWhitespace(ch))
						isWhitespace = false;
					sb.Append(ch);
					ch = this.scanner.ScanChar();
				}
			}
			string value = sb.ToString();
			Node node = Push(null, isWhitespace ? XmlNodeType.Whitespace : XmlNodeType.Text, value);
			return isWhitespace ? true : ValidateNesting(node);
		}
		internal virtual bool ParseComment() {
			char ch = this.scanner.ScanChar();
			if(ch != '-') {
				Push(null, XmlNodeType.Text, "<!-");
				this.state = State.Text;
				return true;
			}
			if(this.scanner.PeekChar() == '[')
				return ParseConditionalExpression();
			string commentValue = this.scanner.ScanTo("-->");
			int i, j;
			while((i = commentValue.IndexOf("--")) >= 0) {
				for(j = i + 2; j < commentValue.Length && commentValue[j] == '-'; j++);
				commentValue = (i != 0 ? commentValue.Substring(0, i + 1) : string.Empty) + commentValue.Substring(j);
			}
			if(commentValue.Length > 0) {
				if(commentValue[commentValue.Length - 1] != ' ')
					commentValue += ' ';
				if(commentValue[0] != ' ')
					commentValue = ' ' + commentValue;
			}
			Push(null, XmlNodeType.Comment, commentValue);
			return true;
		}
		internal virtual bool ParseCData(CDataType type) {
			StringBuilder sb = new StringBuilder();
			bool isScript = type == CDataType.Script;
			for(;;) {
				string value = isScript ? this.scanner.ScanScript() : this.scanner.ScanCSS();
				sb.Append(value);
				if(this.scanner.LastChar != '<' || this.scanner.PeekChar() == '/')
					break;
			}
			this.isCDataParsed = true;
			Push(null, XmlNodeType.CDATA, sb.ToString());
			return true;
		}
		internal virtual bool ParseProcessingInstruction() {
			string name = this.scanner.ScanNameToken(HtmlScanner.ProcessingInstructionTerminators);
			bool isValid = !string.IsNullOrEmpty(name) && name != "xml";
			string value = this.scanner.ScanTo('>');
			if(value.Length > 0)
				value = value.Substring(0, value.Length - 1);
			if(!isValid)
				return false;
			Push(nameTable.Add(name), XmlNodeType.ProcessingInstruction, value);
			return true;
		}
		internal virtual bool ParseAspNetDirective() {
			this.scanner.ScanTo("%>");
			return false;
		}
		internal virtual bool ParseConditionalExpression() {
			this.scanner.ScanChar(); 
			char ch = this.scanner.SkipWhitespaces();
			string name = this.scanner.ScanToken(HtmlScanner.CDataTerminators);
			if(!"CDATA".Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
				this.scanner.ScanTo(">");
				return false;
			}
			ch = this.scanner.SkipWhitespaces();
			if(ch != '[') {
				this.scanner.ScanTo(">");
				return false;
			}
			string value = this.scanner.ScanTo("]]>");
			Push(null, XmlNodeType.CDATA, value);
			return true;
		}
		#endregion
		#region Validation
		private void AddNodeToParentChildList(Node node) {
			Node parentNode;
			Node buf = new Node();
			node.CopyTo(buf);
			if(this.stack != null && this.stack.count > 1 && !buf.IsPCData) {
				parentNode = this.stack[this.stack.count - 2];
				parentNode.ChildNodes.Add(buf);
				buf.ParentNode = parentNode;
				node.ParentNode = parentNode;
			}
		}
		internal bool ValidateNode(Node node, DtdElementDeclaration decl) {
			if(node.Type == XmlNodeType.Whitespace)
				return true;
			bool isInvalidNode = !ValidateNodeCore(node, decl);
			if(isInvalidNode && !this.skippingNodeWithContent)
				Pop();
			return isInvalidNode ? false : ValidateNesting(node);
		}
		private bool ValidateNodeCore(Node node, DtdElementDeclaration decl) {
			if(decl == null)
				return false;
			if(decl.Name == "html" && Depth > 1)
				return false;
			ValidationBehavior validationBehavior = decl.ValidationBehavior;
			CorrectionBehavior correctionBehavior = CorrectionBehavior.None;
			bool nodeTransformRequired = false;
			if(validationBehavior.ElementType == ElementType.Forbidden)
				correctionBehavior = validationBehavior.CorrectionBehavior;
			else if(validationBehavior.ElementType == ElementType.OptionallyForbidden) {
				if(decl.IsScript && !HtmlValidationSettings.AllowScripts ||
					decl.IsIFrame && !HtmlValidationSettings.AllowIFrames && !decl.IsYouTubeIFrame(node) || decl.IsYouTubeIFrame(node) && !HtmlValidationSettings.AllowYouTubeVideoIFrames ||
					!HtmlValidationSettings.AllowFormElements && decl.IsFormElement ||
					!HtmlValidationSettings.AllowEditFullDocument && decl.IsWholeRegimeDocument)
					correctionBehavior = validationBehavior.CorrectionBehavior;
			} else if(validationBehavior.ElementType == ElementType.Deprecated) {
				correctionBehavior = validationBehavior.CorrectionBehavior;
				if(HtmlValidationSettings.AllowedDocumentType == AllowedDocumentType.HTML5 || HtmlValidationSettings.UpdateDeprecatedElements)
					nodeTransformRequired = true;
			} else if(validationBehavior.ElementType == ElementType.Replaceable) {
				correctionBehavior = validationBehavior.CorrectionBehavior;
				if(HtmlValidationSettings.UpdateBoldItalic)
					nodeTransformRequired = true;
			}
			bool isValid = true;
			if(nodeTransformRequired) {
				validationBehavior.TransformNode(node);
				this.endTagTransformQueries.Push(new EndTagTransformQuery(validationBehavior.NewNodeName, this.stack.Count));
			} else if(correctionBehavior == CorrectionBehavior.SkipTag)
				isValid = false;
			else if(correctionBehavior == CorrectionBehavior.SkipTagAndContent) {
				if(!this.skippingNodeWithContent) {
					this.skippingNodeWithContent = true;
					this.depthToSkipTo = this.stack.Count - 1;
				}
				isValid = false;
			}
			if(isValid) {
				if(!node.IsEmpty)
					node.IsEmpty = DtdXhtml10Trans.IsElementEmpty(decl);
			}
			if(!decl.ValidationBehavior.IsTagValid(stack, node))
				isValid = false;
			return isValid;
		}
		private bool ValidateNesting(Node node) {
			int lastButOneIndex = this.stack.Count - 2;
			if(lastButOneIndex == 1) {
				bool htmlCanContain = DtdHtmlElementDeclaration.Instance.CanContain(node);
				if(!htmlCanContain)
					Pop();
				return htmlCanContain;
			}
			int i;
			for(i = lastButOneIndex; i > 0; i--) {
				Node parent = this.stack[i];
				if(parent.IsEmpty)
					continue;
				if(DtdXhtml10Trans.IsChildValid(parent, node, HtmlValidationSettings.AllowedDocumentType, HtmlValidationSettings.AllowHTML5MediaElements, HtmlValidationSettings.AllowObjectAndEmbedElements, HtmlValidationSettings.ContentElementFiltering, HtmlValidationSettings.AllowEditFullDocument))
					break;
			}
			if(i == lastButOneIndex) {
				return true;
			} else if(i == 0) {
				Pop();
				return false;
			} else  {
				this.state = State.InvalidNesting;
				this.invalidNestedNode = node;
				Pop();
				this.validNestingDepth = i + 1;
				return false;
			}
		}
		#endregion
		#region XmlReader
		public override string this[string name, string namespaceURI] {
			get { return this.GetAttribute(name); }
		}
		public override int AttributeCount {
			get { return (node.Type != XmlNodeType.Element && node.Type != XmlNodeType.DocumentType) ? 0 : node.AttributesCount; }
		}
		public override string BaseURI {
			get { return string.Empty; }
		}
		public override int Depth {
			get {
				if(state == State.Attribute)
					return this.stack.Count;
				else if(state == State.AttributeValue)
					return this.stack.Count + 1;
				else
					return this.stack.Count - 1;
			}
		}
		public override bool EOF {
			get { return this.state == State.EndOfFile; }
		}
		public override bool HasValue {
			get { return state == State.Attribute || state == State.AttributeValue || node.Value != null; }
		}
		public override bool IsDefault {
			get { return false; }
		}
		public override bool IsEmptyElement {
			get { return (state == State.Markup || state == State.Attribute || state == State.AttributeValue) ? node.IsEmpty : false; }
		}
		public override string LocalName {
			get {
				if(state == State.Attribute)
					return attribute.Name;
				else if(state == State.AttributeValue)
					return null;
				else
					return node.Name;
			}
		}
		public override XmlNameTable NameTable {
			get { return this.nameTable; }
		}
		public override string NamespaceURI {
			get { return string.Empty; }
		}
		public override string Name {
			get { return LocalName; }
		}
		public override XmlNodeType NodeType {
			get {
				if(state == State.Attribute)
					return XmlNodeType.Attribute;
				else if(state == State.AttributeValue)
					return XmlNodeType.Text;
				else if(state == State.EndTag || state == State.InvalidNesting)
					return XmlNodeType.EndElement;
				return node.Type;
			}
		}
		public override string Prefix {
			get { return string.Empty; }
		}
		public override char QuoteChar {
			get { return this.attribute != null ? '\"' : '\0'; }
		}
		public override string Value {
			get { return (state == State.Attribute || state == State.AttributeValue) ? attribute.Value : node.Value; }
		}
		public override ReadState ReadState {
			get {
				if(state == State.Initial)
					return ReadState.Initial;
				else if(state == State.EndOfFile)
					return ReadState.EndOfFile;
				else
					return ReadState.Interactive;
			}
		}
		public override string XmlLang {
			get { return string.Empty; }
		}
		public override XmlSpace XmlSpace {
			get { return XmlSpace.None; }
		}
		public override void Close() {
			this.InputReader.Close();
		}
		public override string GetAttribute(int i) {
			if(this.state != State.Attribute && this.state != State.AttributeValue) {
				Attribute attr = this.node.GetAttribute(i);
				if(attr != null)
					return attr.Value;
			}
			throw new InvalidOperationException();
		}
		public override string GetAttribute(string name) {
			if(this.state != State.Attribute && this.state != State.AttributeValue) {
				int i = this.node.GetAttributeIndexByName(name);
				if(i >= 0)
					return this.node.GetAttribute(i).Value;
			}
			return null;
		}
		public override string GetAttribute(string name, string namespaceURI) {
			return GetAttribute(name);
		}
		public override string LookupNamespace(string prefix) {
			return null;
		}
		public override void MoveToAttribute(int i) {
			Attribute attr = this.node.GetAttribute(i);
			if(attr == null)
				throw new InvalidOperationException();
			this.attribute = attr;
			this.attributeIndex = i;
			this.SaveState();
			this.state = State.Attribute;
		}
		public override bool MoveToAttribute(string name) {
			int i = this.node.GetAttributeIndexByName(name);
			if(i < 0)
				return false;
			else {
				MoveToAttribute(i);
				return true;
			}
		}
		public override bool MoveToAttribute(string name, string ns) {
			return MoveToAttribute(name);
		}
		public override bool MoveToElement() {
			if(this.state == State.Attribute || this.state == State.AttributeValue) {
				RestoreState();
				this.attribute = null;
				return true;
			} else
				return (this.node.Type == XmlNodeType.Element);
		}
		public override bool MoveToFirstAttribute() {
			if(this.node.AttributesCount > 0) {
				MoveToAttribute(0);
				return true;
			}
			return false;
		}
		public override bool MoveToNextAttribute() {
			if(this.state != State.Attribute && this.state != State.AttributeValue)
				return MoveToFirstAttribute();
			else if(this.attributeIndex < this.node.AttributesCount - 1) {
				MoveToAttribute(this.attributeIndex + 1);
				return true;
			} else
				return false;
		}
		public override bool ReadAttributeValue() {
			if(this.state == State.Attribute) {
				this.state = State.AttributeValue;
				return true;
			} else if(this.state == State.AttributeValue)
				return false;
			else
				throw new InvalidOperationException();
		}
		public override string ReadString() {
			if(this.node.Type != XmlNodeType.Element)
				return this.node.Value;
			StringBuilder sb = new StringBuilder();
			while(Read()) {
				switch(NodeType) {
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						sb.Append(this.node.Value);
						continue;
					default:
						return sb.ToString();
				}
			}
			return sb.ToString();
		}
		public override string ReadInnerXml() {
			using(StringWriter strWriter = new StringWriter()) {
				using(XmlTextWriter xmlWriter = new XmlTextWriter(strWriter)) {
					if(NodeType == XmlNodeType.Attribute) {
						strWriter.Write(Value);
					} else if(NodeType == XmlNodeType.Element) {
						Read();
						while(!EOF && this.NodeType != XmlNodeType.EndElement)
							xmlWriter.WriteNode(this, true);
						Read();
					}
				}
				return strWriter.ToString();
			}
		}
		public override string ReadOuterXml() {
			using(StringWriter strWriter = new StringWriter()) {
				using(XmlTextWriter xmlWriter = new XmlTextWriter(strWriter)) {
					xmlWriter.Formatting = Formatting.Indented;
					xmlWriter.WriteNode(this, true);
					xmlWriter.Close();
					return xmlWriter.ToString();
				}
			}
		}
		public override void ResolveEntity() {
			throw new NotSupportedException();
		}
		private void SaveState() {
			this.savedState = this.state;
		}
		private void RestoreState() {
			this.state = this.savedState;
		}
		#endregion
		#region IHtmlScannerContext
		private event EventHandler Initialization;
		event EventHandler IHtmlScannerContext.Initialization {
			add { this.Initialization += value; }
			remove { }
		}
		XmlNameTable IHtmlScannerContext.NameTable {
			get { return nameTable; }
		}
		private void OnInitialize() {
			Initialize();
			if(this.Initialization != null)
				this.Initialization(this, EventArgs.Empty);
		}
		#endregion
	}
}
