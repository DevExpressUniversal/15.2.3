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
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Xml
#else
namespace DevExpress.CodeParser.Xml
#endif
{
  using XmlType = XmlNodeType;
	public class HtmlXmlNodesCodeGen: HtmlXmlCodeGenBase
	{
		public HtmlXmlNodesCodeGen(CodeGen codeGen)
			: base(codeGen)
		{
		}	
		public override void GenerateElement(LanguageElement languageElement)
		{
			if (languageElement == null)
				return;
			if (languageElement is Comment)
			{
				GenerateHtmlXmlComment(languageElement as Comment);
				return;
			}
			if (languageElement is ISnippetCodeElement)
				CodeGen.SnippetGen.GenerateElement(languageElement);
			else if (GenerateHtmlXmlNode(languageElement as XmlNode))
				return;
			GenerateCssNode(languageElement as BaseCssElement);
		}
		protected override bool GenerateHtmlXmlNode(XmlNode xmlNode)
		{
			if (xmlNode == null)
				return false;
			if (GenerateHtmlNodes(xmlNode as XmlNode))
				return true;
			return  GenerateXmlNodes(xmlNode as XmlNode);			
		}
		public void GenerateHtmlXmlComment(Comment comment)
		{
			if (comment == null)
				return;
			Code.WriteLine(comment.Name);
		}
	public void GenerateRazorModelDirective(RazorModelDirective modelDirective)
	{
	  Code.WriteLine("@model {0}", modelDirective.Model);
	}
	private void GenerateRazorSection(RazorSection razorSection)
	{
	  Code.WriteLine("@section {0}", razorSection.Name);
	  if (razorSection.DotNetLanguageType == DotNetLanguageType.CSharp)
		Code.WriteLine("{");
	  Code.WriteLine(razorSection.Code);
	  if (razorSection.DotNetLanguageType == DotNetLanguageType.CSharp)
		Code.WriteLine("}");
	  else if (razorSection.DotNetLanguageType == DotNetLanguageType.VisualBasic)
		Code.WriteLine("End Section");
	}
		#region GenerateCssNode
		protected bool GenerateCssNode(BaseCssElement element)
		{
			if (element == null)
				return false;
			switch (element.ElementType)
			{
		case LanguageElementType.CssStyleSheet:
					GenetrateCssStyleSheet(element as CssStyleSheet);
					return true;
		case LanguageElementType.CssStyleRule:
					GenerateCssStyleRule(element as CssStyleRule);
					return true;
		case LanguageElementType.CssPropertyDeclaration:
					GenerateCssPropertyDeclaration(element as CssPropertyDeclaration);
					return true;
		case LanguageElementType.CssSelector:
					GenerateCssSelector(element as CssSelector);
					return true;
		case LanguageElementType.CssAttribute:
					GenerateCssAttributeSelector(element as CssAttributeSelector);
					return true;			
				case LanguageElementType.CssPseudoId:
					GenerateCssPseudoSelector(element as CssPseudoSelector);
					return true;
		case LanguageElementType.CssPseudoFunction:
					GenerateCssPseudoFunctionSelector(element as CssPseudoFunctionSelector );
					return true;
		case LanguageElementType.CssIdentifierReference:
					GenerateCssIdentifierReference(element as CssIdentifierReference);
					return true;
		case LanguageElementType.CssFunctionReference:
					GenerateCssFunctionReference(element as CssFunctionReference);
					return true;
		case LanguageElementType.CssStringLiteral:
					GenerateCssStringLiteral(element as CssStringLiteral);
					return true;
		case LanguageElementType.CssURI:
					GenerateCssURIReference (element as CssURIReference);
					return true;
		case LanguageElementType.CssColorReference:
					GenerateCssColorReference(element as CssColorReference);
					return true;
		case LanguageElementType.CssNumberLiteral:
					GenerateCssNumberLiteral(element as CssNumberLiteral);
					return true;
		case LanguageElementType.CssLengthLiteral:
					GenerateCssLengthLiteral(element as CssLengthLiteral);
					return true;
		case LanguageElementType.CssPercentLiteral:
					GenerateCssPercentLiteral(element as CssPercentLiteral);
					return true;
		case LanguageElementType.CssTimeLiteral:
					GenerateCssTimeLiteral(element as CssTimeLiteral);
					return true;
		case LanguageElementType.CssAngleLiteral:
					GenerateCssAngleLiteral(element as CssAngleLiteral);
					return true;
		case LanguageElementType.CssFrequencyLiteral:
					GenerateCssFrequencyLiteral(element as CssFrequencyLiteral);
					return true;
		case LanguageElementType.CssCharsetDeclaration:
					GenerateCssCharsetDeclaration(element as CssCharsetDeclaration);
					return true;
		case LanguageElementType.CssImportDirective:
					GenerateCssImportDirective(element as CssImportDirective);
					return true;
		case LanguageElementType.CssMediaDirective:
					GenerateCssMediaDirective(element as CssMediaDirective);
					return true;
		case LanguageElementType.CssPageStyleDefinition:
					GenerateCssPageStyle(element as CssPageStyle);
					return true;
		case LanguageElementType.CssCommentTag:
					GenerateCssCommentTag(element as CssCommentTag);
					return true;
			}
			return false;
		}
		#endregion
		#region Generate CSS
		public void GenetrateCssStyleSheet(CssStyleSheet element)
		{
			if (element == null)
				return;
			GenerateDescendantList(element.Nodes, "");
		}
		public void GenerateCssStyleRule(CssStyleRule element)
		{
			if (element == null)
				return;
			GenerateDescendantList(element.Selectors, ", ", false, true, false, false);
			Code.WriteLine();
			Code.WriteLine("{");
			Code.IncreaseIndent();
			GenerateDescendantList(element.Properties, ";", false, true);
			Code.DecreaseIndent();
			Code.WriteLine("}");
		}
		public void GenerateCssPropertyDeclaration(CssPropertyDeclaration element)
		{
			if (element == null)
				return;
			if (element.Name != null && element.Name != String.Empty)
				Code.Write(element.Name);
			if (element.IsEqual)
				Code.Write(" = ");
			else
				Code.Write(": ");
			GenerateCssTermCollection(element.Values);
			if (element.IsImportant)
				Code.Write("!important");
		}
		public void GenerateCssSelector(CssSelector element)
		{
			if (element == null)
				return;
			WriteCssSelector(element);
		}
		void WriteCssSelector(CssSelector element)
		{
			if (element == null)
				return;
			bool isFirst = true;
			for(;;)
			{
				if (element.ElementType == LanguageElementType.CssAttribute && element is CssAttributeSelector)
					WriteCssAttributeSelector(element as CssAttributeSelector);
				else if (element.ElementType == LanguageElementType.CssPseudoId && element is CssPseudoSelector)
					WriteCssPseudoSelector(element as CssPseudoSelector);
		else if (element.ElementType == LanguageElementType.CssPseudoFunction && element is CssPseudoFunctionSelector)
					WriteCssPseudoFunctionSelector(element as CssPseudoFunctionSelector);
		else if (element.ElementType == LanguageElementType.CssSelector)
					GenerateSelectorType(element.SelectorType, element.Name, isFirst);
				if (isFirst)
					isFirst = false;
				if (element.Nodes == null || element.NodeCount == 0)
					break;
				element = element.ChildSelector as CssSelector;
				if (element == null)
					break;
			}
		}
		void GenerateSelectorType(CssSelectorType type, string name, bool isFirst)
		{
			string str = String.Empty;
			switch(type)
			{
				case CssSelectorType.CombinedSibling:
					str = "+";
					break;
				case CssSelectorType.CombinedChild:
					str = ">";
					break;
				case CssSelectorType.Class:
					str = AddString(".", name);
					break;
				case CssSelectorType.Hash:
					str = AddString("#", name);
					break;
				case CssSelectorType.Name:
					if (isFirst)
						str = name;
					else
						str = AddString(" ", name);						
					break;
				default:
					str = name;
					break;
			}
			if (str != null && str != String.Empty)
				Code.Write(str);
		}
	string AddString(string str, string addStr)
		{
			if (str == null || str.Length == 0 || addStr == null || addStr.Length == 0)
				return str;
			return str + addStr;
		}
		public void GenerateCssAttributeSelector(CssAttributeSelector element)
		{
			if (element == null)
				return;
			WriteCssSelector(element);
		}
		void WriteCssAttributeSelector(CssAttributeSelector element)
		{
			Code.Write("[ {0}", element.Name);
			GenerateCssAttrValue(element);
			Code.WriteLine("]");
		}		
		void GenerateCssAttrValue(CssAttributeSelector element)
		{
			if (element == null)
				return;
			string str = string.Empty;
			switch(element.EqualityType)
			{
				case AttributeSelectorEqualityType.ExactMatch:
					str = "=";
					break;
				case AttributeSelectorEqualityType.SpaceSeparatedMatch:
					str = "~=";
					break;
				case AttributeSelectorEqualityType.HyphenSeparatedMatch:
					str = "|=";
					break;
			}
			if (element.Value != null)
				str += element.Value;
			if (str != String.Empty)
				Code.Write(str);
		}
		public void GenerateCssPseudoSelector(CssPseudoSelector element)
		{
			if (element == null)
				return;
			WriteCssSelector(element);
		}
		void WriteCssPseudoSelector(CssPseudoSelector element)
		{
			Code.Write(": {0}", element.Name);
		}
		public void GenerateCssPseudoFunctionSelector(CssPseudoFunctionSelector element)
		{
			if (element == null)
				return;
			WriteCssSelector(element);
		}
		void WriteCssPseudoFunctionSelector(CssPseudoFunctionSelector element)
		{
			Code.Write(": {0} ({1})", element.Name, element.Param);
		}		
		public void GenerateCssIdentifierReference(CssIdentifierReference element)
		{
			if (element == null)
				return;			
			GenerateTermElement(element);
		}
		public void GenerateCssFunctionReference(CssFunctionReference element)
		{
			if (element == null)
				return;
			GenerateTermElement(element);
			Code.Write("(");
			GenerateCssTermCollection(element.Parameters);
			Code.Write(")");
		}
		void GenerateCssTermCollection(CssTermCollection coll)
		{
			if (coll == null || coll.Count == 0)
				return;
			for(int i = 0; i < coll.Count; i++)
			{
				GenerateElement(coll[i] as LanguageElement);
			}
		}
		public void GenerateCssStringLiteral(CssStringLiteral element)
		{
			if (element == null)
				return;			
			if (element.Value != null && element.Value != String.Empty)
				Code.Write(element.Value);
		}
		public void GenerateCssURIReference (CssURIReference element)
		{
			if (element == null)
				return;			
			if (element.URI != null && element.URI != String.Empty)
				Code.Write(element.URI);
		}
		public void GenerateCssColorReference(CssColorReference element)
		{
			if (element == null)
				return;			
			GenerateTermElement(element);
		}
		public void GenerateCssNumberLiteral(CssNumberLiteral element)
		{
			if (element == null)
				return;
			GenerateNumberLiteralBase(element);
		}
		void GenerateTermElement(CssTerm element)
		{
			GenerateTermElement(element, false);
		}
		void GenerateCssMeasuredNumber(CssMeasuredNumber element)
		{
			GenerateCssMeasuredNumber(element, false);
		}
		void GenerateNumberLiteralBase(CssNumberLiteral element)
		{
			GenerateNumberLiteralBase(element, false);
		}
		void GenerateNumberLiteralBase(CssNumberLiteral element, bool addNewLine)
		{
			if (element == null)
				return;
			GenerateTermDelimiter(element);
			GeneratePrecedingSign(element.Sign);			
			if (element.Value != null)
				GenerateCssValue(element);
			if (addNewLine)
				Code.WriteLine();
		}		
		void GenerateCssMeasuredNumber(CssMeasuredNumber element, bool addNewLine)
		{
			if (element == null)
				return;
			GenerateNumberLiteralBase(element, false);
			GenerateCssMeasureUnit(element.Unit);
			if (addNewLine)
				Code.WriteLine();
		}
		void GenerateTermElement(CssTerm element, bool addNewLine)
		{
			if (element == null)
				return;
			GenerateTermDelimiter(element);
			GeneratePrecedingSign(element.Sign);
			if (element.Name != null && element.Name != String.Empty)
				Code.Write(element.Name);
			if (addNewLine)
				Code.WriteLine();			
		}
		void GenerateCssValue(object element)
		{
			if (element == null)
				return;
			string str = String.Empty;
			if (element is string)
				str = element as string;
			else
				str = element.ToString();
			if (str != null && str != String.Empty)
				Code.Write(str);
		}
		#region GenerateCssMeasureUnit
		void GenerateCssMeasureUnit(CssMeasureUnit unit)
		{
			string str = String.Empty;
			switch(unit)
			{			
				case CssMeasureUnit.Percent:
					str = "%";
					break;
				case CssMeasureUnit.Grad:
					str = "grad";
					break;
				case CssMeasureUnit.Rad:
					str = "rad";
					break;
				case CssMeasureUnit.Deg:
					str = "deg";
					break;
				case CssMeasureUnit.kHz:
					str = "khz";
					break;
				case CssMeasureUnit.Hz:
					str = "hz";
					break;
				case CssMeasureUnit.Px:
					str = "px";
					break;
				case CssMeasureUnit.Cm:
					str = "cm";
					break;
				case CssMeasureUnit.Mm:
					str = "mm";
					break;
				case CssMeasureUnit.In:
					str = "in";
					break;
				case CssMeasureUnit.Pt:
					str = "pt";
					break;
				case CssMeasureUnit.Pc:
					str = "pc";
					break;
				case CssMeasureUnit.Em:
					str = "em";
					break;
				case CssMeasureUnit.Ex:
					str = "ex";
					break;
				case CssMeasureUnit.Ms:
					str = "ms";
					break;
				case CssMeasureUnit.S:
					str = "s";
					break;
			}
			#endregion
			if (str != null && str != String.Empty)
				Code.Write(str);
		}
		void GeneratePrecedingSign(PrecedingSign sign)
		{
			string str = String.Empty;
			if (sign == PrecedingSign.Minus)
				str = "-";
			else if (sign == PrecedingSign.Plus)
				str = "+";
			Code.Write(str);
		}
		public void GenerateCssLengthLiteral(CssLengthLiteral element)
		{
			if (element == null)
				return;
			GenerateCssMeasuredNumber(element);
		}
		public void GenerateCssPercentLiteral(CssPercentLiteral element)
		{
			if (element == null)
				return;
			GenerateCssMeasuredNumber(element);
		}
		public void GenerateCssTimeLiteral(CssTimeLiteral element)
		{
			if (element == null)
				return;
			GenerateCssMeasuredNumber(element);
		}
		public void GenerateCssAngleLiteral(CssAngleLiteral element)
		{
			if (element == null)
				return;
			GenerateCssMeasuredNumber(element);
		}
		public void GenerateCssFrequencyLiteral(CssFrequencyLiteral element)
		{
			if (element == null)
				return;
			GenerateCssMeasuredNumber(element);
		}
		void GenerateTermDelimiter(CssTerm element)
		{
			if (element == null)
				return;
			WriteTermDelimiter(element.Delimiter);
		}
		void WriteTermDelimiter(ExpressionDelimiter element)
		{
			string str = String.Empty;
			switch(element)
			{
				case ExpressionDelimiter.Whitespace:				
					str = " ";
					break;
				case ExpressionDelimiter.Comma:
					str = ", ";
					break;
				case ExpressionDelimiter.Slash:
					str = "/";
					break;
				case ExpressionDelimiter.Equals:
					str = "=";
					break;
			}
			if (str != null && str != String.Empty)
				Code.Write(str);
		}
		public void GenerateCssCharsetDeclaration(CssCharsetDeclaration element)
		{
			if (element == null)
				return;
			Code.WriteLine("@charset {0};", element.Charset);
		}
		public void GenerateCssImportDirective(CssImportDirective element)
		{
			if (element == null)
				return;
			Code.Write("@import {0}", element.Source);
			GenerateStringCollection(element.SupportedMediaTypes, ", ");
			Code.WriteLine(";");
		}
		public void GenerateCssMediaDirective(CssMediaDirective element)
		{
			if (element == null)
				return;
			GenerateStringCollection(element.MediaTypes, ", ");
			Code.WriteLine("@media\n{");
			GenerateDescendantList(element.DetailNodes);
			Code.WriteLine("}");
		}			
		public void GenerateCssCommentTag(CssCommentTag element)
		{
			if (element == null)
				return;
			string str = String.Empty;
			if (element.Kind == CommentTagKind.Open)
				str = "<!--";
			else if (element.Kind == CommentTagKind.Close)
				str = "-->";
			Code.Write(str);
		}
		public void GenerateCssPageStyle(CssPageStyle element)
		{
			if (element == null)
				return;
			Code.Write("@page");
			if (element.Name != null && element.Name != String.Empty)
				Code.WriteLine(": {0} ", element.Name);
			Code.WriteLine("{");
			GenerateDescendantList(element.Properties, ", "); 
			Code.WriteLine("}");
		}
		#endregion
		#region GenerateHtmlNodes
		private bool GenerateHtmlNodes(XmlNode xmlNode)
		{
			if (xmlNode == null)
				return false;
			switch (xmlNode.ElementType)
			{
				case LanguageElementType.HtmlAttribute:
					GenerateHtmlAttribute(xmlNode as HtmlAttribute);
					return true;
				case LanguageElementType.HtmlElement:
					GenerateHtmlElement(xmlNode as HtmlElement);
					return true;
				case LanguageElementType.HtmlScript:
					GenerateHtmlScript(xmlNode as HtmlScriptDefinition);
					return true;
				case LanguageElementType.HtmlStyle:
					GenerateHtmlStyle(xmlNode as HtmlStyleDefinition);
					return true;
				case LanguageElementType.HtmlText:
					GenerateHtmlText(xmlNode as HtmlText);
					return true;
		case LanguageElementType.RazorModelDirective:
		  GenerateRazorModelDirective(xmlNode as RazorModelDirective);
		  return true;
				case LanguageElementType.AspCodeEmbedding:
					GenerateAspCodeEmbedding(xmlNode as AspCodeEmbedding);
					return true;
				case LanguageElementType.AspDirective:
					GenerateAspDirective(xmlNode as AspDirective);
					return true;
				case LanguageElementType.RegisterDirective:
					GenerateRegisterDirective(xmlNode as RegisterDirective);
					return true;
				case LanguageElementType.PageDirective:
					GeneratePageDirective(xmlNode as PageDirective);
					return true;
				case LanguageElementType.AspImportDirective:
					GenerateAspDirective(xmlNode as AspDirective);
					return true;
				case LanguageElementType.MasterDirective:
					GenerateAspDirective(xmlNode as AspDirective);
					return true;
				case LanguageElementType.ControlDirective:
					GenerateAspDirective(xmlNode as AspDirective);
					return true;
				case LanguageElementType.ServerControlElement:
					GenerateServerControlElement(xmlNode as ServerControlElement);
					return true;
				case LanguageElementType.ContentPlaceHolder:
					GenerateHtmlElement(xmlNode as HtmlElement);
					return true;
		case LanguageElementType.RazorSectionElement:
		  GenerateRazorSection(xmlNode as RazorSection);
		  return true;
			}
			return false;
		}
		#endregion
		private bool GenerateXmlNodes(XmlNode xmlNode)
		{
			if (xmlNode == null)
				return false;
			if (xmlNode is BaseXmlNode)
				GenerateBaseXmlNodes(xmlNode as BaseXmlNode);
			return false;
		}
		#region GenerateBaseXmlNodes
		private bool GenerateBaseXmlNodes(BaseXmlNode xmlNode)
		{
			if (xmlNode == null)
				return false;
			switch (xmlNode.XmlNodeType)
			{
				case XmlType.AnyContentSpec:
					GenerateXmlAnyContentSpec(xmlNode as XmlAnyContentSpec);
					return true;
				case XmlType.AttributeDecl:
					GenerateXmlAttributeDecl(xmlNode as XmlAttributeDeclaration);
					return true;
				case XmlType.AttributeListDecl:
					GenerateXmlAttributeListDecl(xmlNode as XmlAttributeListDeclaration);
					return true;
				case XmlType.CharacterData:
					GenerateXmlCharacterData(xmlNode as XmlCharacterData);
					return true;
				case XmlType.CharReference:
					GenerateXmlCharReference(xmlNode as XmlCharReference);
					return true;
				case XmlType.ChildrenContentSpec:
					GenerateXmlChildrenContentSpec(xmlNode as XmlChildrenContentSpec);
					return true;
				case XmlType.CPChoice:
					GenerateXmlCPChoice(xmlNode as XmlChoiceContentParticle);
					return true;
				case XmlType.CPSequence:
					GenerateXmlCPSequence(xmlNode as XmlSequenceContentParticle);
					return true;
				case XmlType.Decl:
					GenerateXmlDecl(xmlNode as XmlDecl);
					return true;
				case XmlType.DocTypeDecl:
					GenerateXmlDocTypeDecl(xmlNode as XmlDocTypeDecl);
					return true;
				case XmlType.ElementDecl:
					GenerateXmlElementDecl(xmlNode as XmlElementDecl);
					return true;
				case XmlType.EmptyContentSpec:
					GenerateXmlEmptyContentSpec(xmlNode as XmlEmptyContentSpec);
					return true;
				case XmlType.EntityDecl:
					GenerateXmlEntityDecl(xmlNode as XmlEntityDecl);
					return true;
				case XmlType.ExternalPublicID:
					GenerateXmlExternalPublicID(xmlNode as NewExternalIDPublicLink);
					return true;
				case XmlType.ExternalSystemID:
					GenerateXmlExternalSystemID(xmlNode as NewExternalIDSystemLink);
					return true;
				case XmlType.MixedContentSpec:
					GenerateXmlMixedContentSpec(xmlNode as XmlMixedContentSpec);
					return true;
				case XmlType.Name:
					GenerateXmlName(xmlNode as XmlName);
					return true;
				case XmlType.NamedCP:
					GenerateXmlNamedCP(xmlNode as XmlNamedContentParticle);
					return true;
				case XmlType.NameReference:
					GenerateXmlNameReference(xmlNode as XmlNameReference);
					return true;
				case XmlType.NotationDecl:
					GenerateXmlNotationDecl(xmlNode as XmlNotationDecl);
					return true;
				case XmlType.ProcessingInstruction:
					GenerateXmlProcessingInstruction(xmlNode as XmlProcessingInstruction);
					return true;
				case XmlType.Reference:
					GenerateXmlReference(xmlNode as XmlReference);
					return true;
			}
			return false;
		}
		#endregion
		#region Methods of CodeGen for HTML Nodes
		public void GenerateHtmlAttribute(HtmlAttribute atr)
		{
			string delimiter = GetAttributeDelimiters(atr);
	  string value = atr.Value;
	  if (atr.DetailNodeCount > 0 && atr.DetailNodes[0] is MarkupExtensionExpression)
	  {
		value = GenerateMarkupExtensionExpression((MarkupExtensionExpression)atr.DetailNodes[0]);
	  }
	  string aspEmbCodeBeg = string.Empty;
	  string aspEmbCodeEnd = string.Empty;
			if (atr.ValueIsAspCodeEmbedding)
			{
					aspEmbCodeBeg = "<%";
					aspEmbCodeEnd = "%>";
			}
	  Code.Write(@" {0}={3}{2}{1}{2}{4}", atr.Name, value, delimiter, aspEmbCodeBeg, aspEmbCodeEnd);
		}
	public string GenerateMarkupExtensionExpression(MarkupExtensionExpression expression)
	{
	  CSharp.CSharpCodeGen codeGen = new CSharp.CSharpCodeGen(Options);
	  string result = codeGen.ExpressionGen.GenerateCode(expression);
	  return result;
	}
		string GetAttributeDelimiters(HtmlAttribute atr)
		{
			switch (atr.AttributeQuoteType)
			{
				case QuoteType.SingleQuote:
					return "\'";
				case QuoteType.DoubleQuote:
					return "\"";
				case QuoteType.None:
					return String.Empty;
			}
			return String.Empty;
		}
		bool ShouldAddNewLine(HtmlElement element)
		{
			if (element == null)
				return false;
			if (element.NodeCount == 0)
				return true;
			return !(element.Nodes[0] is HtmlText);
		}
		public void GenerateHtmlElement(HtmlElement element)
		{
			if (element == null)
				return;
			GenerateElementBeg(element, ShouldAddNewLine(element));
			if (!element.IsEmptyTag)
			{
				bool isIncreaseIndent = IncreaseIndent(element);
				GenerateNodesList(element.Nodes, isIncreaseIndent);
				GenerateElementEnd(element);
			}
		}
		bool IncreaseIndent(HtmlElement element)
		{
			if (element == null)
				return false;
			bool isText = false;
			NodeList list = element.Nodes;
			if (list != null && list.Count != 0)
			{
				LanguageElement currentElement = list[0] as LanguageElement;
				if (currentElement is XmlNode && currentElement.ElementType == LanguageElementType.HtmlText)
					isText = true;
			}
			if (element.HasCloseTag && !isText)
				return true;
			return false;
		}
		#region GenerateElementBeg
		void GenerateElementBeg(HtmlElement element, bool addNewLine)
		{
			if (element.IsEmptyTag)
				GenerateElementBeg(element, "<", "/>", addNewLine);
			else
				GenerateElementBeg(element, "<", ">", addNewLine);			
		}
		void GenerateElementBeg(HtmlElement element)
		{
			GenerateElementBeg(element, true);
		}
		void GenerateElementBeg(HtmlElement element, string openSign, string closeSign, bool addNewLine)
		{
			GenerateElementBeg(element, openSign, closeSign, addNewLine, String.Empty);
		}
		void GenerateElementBeg(HtmlElement element, string openSign, string closeSign, bool addNewLine, string elementName)
		{
			string name = elementName;
			if (elementName == null || elementName == String.Empty)
				name = GetHtmlElementName(element);
			Code.Write("{0}{1}",openSign, name);
			GenerateHtmlAttributesList(element.XmlAttributes);
			Code.Write(closeSign);
			if (addNewLine)
				Code.WriteLine();
		}
		#endregion
		#region GetDirectiveName
		string GetHtmlElementName(HtmlElement element)
		{
			if (element == null)
				return String.Empty;
			string name = element.Name;
			LanguageElementType elementType = element.ElementType;
			switch(elementType)
			{
				case LanguageElementType.RegisterDirective:
					name = "Register";
					break;
				case LanguageElementType.PageDirective:
					name = "Page";
					break;
				case LanguageElementType.ControlDirective:
					name = "Control";
					break;
				case LanguageElementType.MasterDirective:
					name = "Master";
					break;
				case LanguageElementType.ImportDirective:
					name = "Import";
					break;
				case LanguageElementType.HtmlScript:
					name = "SCRIPT";
					break;
				case LanguageElementType.HtmlStyle:
					name = "STYLE";
					break;
			}
			return name;
		}
		#endregion
		#region GenerateElementEnd
		void GenerateElementEnd(HtmlElement element)
		{
			GenerateElementEnd(element, "</", ">" );
		}
		void GenerateElementEnd(HtmlElement element, string elementName)
		{
			GenerateElementEnd(element, "</", ">", elementName);
		}
		void GenerateElementEnd(HtmlElement element, string openSign, string closeSign)
		{
			GenerateElementEnd(element, openSign, closeSign, String.Empty);
		}
		void GenerateElementEnd(HtmlElement element, string openSign, string closeSign, string elementName)
		{
			if (!element.HasCloseTag)
				return;
			string name = elementName;
			if (elementName == null || elementName == String.Empty)
				name = element.Name;
			bool shouldUseIndent = true;
			if (element.NodeCount > 0)
				shouldUseIndent = !(element.Nodes[element.NodeCount - 1] is HtmlText);
			if (shouldUseIndent)
				Code.WriteLine("{0}{1}{2}", openSign, name, closeSign);
			else
			{
				Code.WriteClearFormat(String.Format("{0}{1}{2}", openSign, name, closeSign));
				Code.WriteLine();
			}
		}
		#endregion
		#region GenerateHtmlAttributesList
		void GenerateHtmlAttributesList(NodeList attributes)
		{
			if (attributes == null || attributes.Count == 0)
				return;
			for (int i = 0; i < attributes.Count; i++)
				GenerateHtmlAttribute(attributes[i] as HtmlAttribute);
		}
		#endregion
		#region GenerateNodesList
		void GenerateNodesList(NodeList elements, bool isIncreaseIndent)
		{
			if (elements == null || elements.Count == 0)
				return;
			LanguageElement currentElement = null;
			if (isIncreaseIndent)
				Code.IncreaseIndent();
			int count = elements.Count;
			for (int i = 0; i < count; i++)
			{
				currentElement = elements[i] as LanguageElement;
				if (currentElement == null)
					continue;
				GenerateElement(currentElement);				
			}
			if (isIncreaseIndent)
				Code.DecreaseIndent();			
		}
		#endregion
		public void GenerateHtmlScript(HtmlScriptDefinition script)
		{
			if (script == null)
				return;
			GenerateElementBeg(script, "<", ">", false, "script");		
			if
				(
					(
						(script.Nodes == null || script.NodeCount == 0)
						||				
						(IsUnknowLanguage(script))
					)
					&&
					(script.ScriptText != null && script.ScriptText != String.Empty)
				)				
				Code.WriteClearFormat(script.ScriptText);				
			else if (script.Nodes != null || script.NodeCount != 0)
			{
				Code.WriteLine();
				Code.IncreaseIndent();
				DotNetLanguageType languageType = GetLanguageType(script);
				CodeGen codeGen = GetScriptCodeGen(languageType);
				GenerateNodesFromOtherLanguages(codeGen, script.Nodes);
				Code.DecreaseIndent();
			}
			GenerateElementEnd(script, "script");
		}
		void GenerateNodesFromOtherLanguages(CodeGen codeGen, NodeList nodes)
		{
			for(int i = 0; i < nodes.Count; i++)
				codeGen.GenerateElement(nodes[i] as LanguageElement);
		}
		string GetScriptLanguage(DotNetLanguageType languageType)
		{			
			string languageName = String.Empty;
			switch(languageType)
			{
				case DotNetLanguageType.CSharp:
					languageName = "CSharp";
					break;
				case DotNetLanguageType.VisualBasic:
					languageName = "Basic";
					break;
			}
			return languageName;
		}
		bool IsUnknowLanguage(HtmlScriptDefinition element)
		{
			if (GetLanguageType(element) == DotNetLanguageType.Unknown)
				return true;
			return false;
		}
		DotNetLanguageType GetLanguageType(HtmlScriptDefinition element)
		{
			DotNetLanguageType languageType = element.Language;
			if (languageType != DotNetLanguageType.Unknown)
				return languageType;
			if (element.FileNode == null)
				return DotNetLanguageType.Unknown;
			SourceFile sourceFile = element.FileNode as SourceFile;
			if (sourceFile == null)
				return DotNetLanguageType.Unknown;
			languageType = sourceFile.AspPageLanguage;
			return languageType;
		}
	CodeGen GetCodeGenerator(string languageName)
	{
	  switch (languageName)
	  {
		case "CSharp":
		  return new CSharp.CSharpCodeGen();
		case "Basic":
		  return new VB.VBCodeGen();
	  }
	  return null;
	}
		CodeGen GetScriptCodeGen(DotNetLanguageType languageType)
		{
			string languageName = GetScriptLanguage(languageType);
			if (languageName == null && languageName == String.Empty)
				return null;
	  CodeGen codeGen = GetCodeGenerator(languageName);
			if (codeGen == null)
				return CodeGen;
			codeGen.Code = CodeGen.Code;
			return codeGen;
		}
		public void GenerateHtmlStyle(HtmlStyleDefinition style)
		{
			if (style == null)
				return;
			GenerateElementBeg(style, "<", ">", true, "style");			
			if ((style.Nodes == null || style.NodeCount == 0) 
				&& style.StyleText != null && style.StyleText != String.Empty)
				Code.WriteClearFormat(style.StyleText);				
			else if (style.Nodes != null || style.NodeCount != 0)
			{
				Code.IncreaseIndent();
				GenerateDescendantList(style.Nodes, "");
				Code.DecreaseIndent();
			}
			GenerateElementEnd(style,"style");
		}
		public void GenerateRegisterDirective(RegisterDirective element)
		{
			if (element == null)
				return;
			GenerateAspDirective(element);
		}
		public void GeneratePageDirective(PageDirective element)
		{
			if (element == null)
				return;
			GenerateAspDirective(element);
		}
		public void GenerateServerControlElement(ServerControlElement element)
		{
			if (element == null)
				return;
			GenerateHtmlElement(element);
		}
		public void GenerateHtmlText(HtmlText text)
		{
			if (text == null)
				return;
			string nodeText = text.Text;
			Code.WriteClearFormat(nodeText);
		}		
		public void GenerateAspCodeEmbedding(AspCodeEmbedding code)
		{
	  string embeddingStart = "<%";
	  string embeddingEnd = "%>";
	  if (code.IsRazorEmbedding)
	  {
		embeddingStart = "@";
		embeddingEnd = String.Empty;
	  }
	  Code.WriteLine("{0}{1}{2}", embeddingStart, code.Code, embeddingEnd);
		}		
		public void GenerateAspDirective(AspDirective directive)
		{
			GenerateElementBeg(directive, "<%@ ", " %>", true);
		}
		#endregion
		#region Methods of CodeGen for XML Nodes
		public void GenerateXmlAnyContentSpec(XmlAnyContentSpec spec)
		{
			Code.Write("ANY");
		}
		public void GenerateXmlAttributeDecl(XmlAttributeDeclaration decl)
		{
			Code.Write(decl.Name);
			WriteAttribute(decl);
			GenerateDefaultAttributeValueType(decl);
		}
		void GenerateDefaultAttributeValueType(XmlAttributeDeclaration decl)
		{
			if (decl == null)
				return;
			string str = String.Empty;
			switch(decl.DefaultAttributeValueType)
			{
				case DefaultAttributeValueType.Required:
					str = "#REQUIRED" ;
					break;
				case DefaultAttributeValueType.Implied:
					str = "#IMPLIED";
					break;
				case DefaultAttributeValueType.Fixed:
					str = "#FIXED";
					if (decl.DefaultAttributeValue != null || decl.DefaultAttributeValue != String.Empty)
						str += decl.DefaultAttributeValue;
					break;				
			}
			if (str != null && str != String.Empty)
				Code.Write(" {0}", str);
		}
		void WriteAttribute(XmlAttributeDeclaration decl)
		{
			string str = string.Empty;
			switch(decl.AttributeType)
			{
				case AttributeType.CData:
					str = "CDATA";
					break;
				case AttributeType.IdRef:
					str = "IDREF";
					break;
				case AttributeType.IdRefs:
					str = "IDREFS";
					break;
				case AttributeType.Entity:
					str = "ENTITY";
					break;
				case AttributeType.Entities:
					str = "ENTITIES";
					break;
				case AttributeType.NmToken:
					str = "NMTOKEN";
					break;
				case AttributeType.NmTokens:
					str = "NMTOKENS";
					break;
				case AttributeType.Id:
					str = "ID";
					break;		
				case AttributeType.Enumeration:
				case AttributeType.Notation:
					GenerateAttlistEnumeration(decl);
					return;
			}
			if (str != String.Empty)
				Code.Write(" {0}", str);
		}
		void GenerateAttlistEnumeration(XmlAttributeDeclaration decl)
		{
			if(decl.AttributeType == AttributeType.Notation)
				Code.Write(" NOTATION");
			Code.Write(" (");
			GenerateStringCollection(decl.EnumerationMembers," | ");
			Code.Write(")");	
		}
		void GenerateStringCollection(StringCollection coll, string delimiter)
		{
			if (coll == null || coll.Count == 0)
				return;
			for(int i = 0; i < coll.Count; i++)
			{				
				if (coll[i] == null || coll[i] == String.Empty)
					continue;
				if (i != 0)
					Code.Write(delimiter);
				Code.Write(coll[i]);
			}
		}
		public void GenerateXmlAttributeListDecl(XmlAttributeListDeclaration decl)
		{
			Code.Write("<!ATTLIST {0}", decl.Name);
			GenerateDescendantList(decl.AttributesDecl, true);			
			Code.WriteLine(">");			
		}
		public void GenerateXmlCharacterData(XmlCharacterData data)
		{
			Code.Write("<![CDATA[\n");
			Code.Write(data.Name,false);
			Code.WriteLine("]]>",false);			
		}
		public void GenerateXmlCharReference(XmlCharReference data)
		{
			Code.Write("{0};",data.Name);
		}
		public void GenerateXmlChildrenContentSpec(XmlChildrenContentSpec spec)
		{
			GenerateDescendantList(spec.DetailNodes);			
		}
		public void GenerateXmlCPChoice(XmlChoiceContentParticle part)
		{
			GenerateDescendantList(part.DetailNodes,"|");
			GenerateRepeatCount(part);
		}
		public void GenerateXmlCPSequence(XmlSequenceContentParticle part)
		{
			GenerateDescendantList(part.DetailNodes,", ");
			GenerateRepeatCount(part);
		}
		void GenerateRepeatCount(XmlContentParticle content)
		{
			string result = String.Empty;
			switch(content.RepeatCount)
			{
				case RepeatCount.ZeroOrOnce:
					result = "?";
					break;
				case RepeatCount.ZeroOrMore:
					result = "*";
					break;
				case RepeatCount.OnceOrMore:
					result = "+";
					break;
			}
			if (result != String.Empty)
				Code.Write(result);			
		}
		public void GenerateXmlDecl(XmlDecl decl)
		{
			Code.Write("<?xml");
			WriteXmlString(" version=", decl.Version);
			WriteXmlString(" standalone=", decl.StandAlone);
			WriteXmlString(" encording=", decl.Encoding);
			Code.WriteLine(" ?>");
		}
		void WriteXmlString(string name, string str)
		{
			if (str != null && str != String.Empty && name != null)
				Code.Write("{0}{1}",name, str);
		}
		void WriteString(string str)
		{
			if (str != null && str != String.Empty)
				Code.Write(" {0}",str);
		}
		public void GenerateXmlDocTypeDecl(XmlDocTypeDecl decl)
		{
			Code.Write("<!DOCTYPE {0}", decl.Name);
			GenerateDescendantList(decl.DetailNodes, true);
			if (decl.Nodes != null && decl.NodeCount != 0)
			{
				Code.WriteLine("[");
				GenerateDescendantList(decl.Nodes,"", false);
				Code.Write("]");
			}
			Code.WriteLine(">");
		}
		#region GenerateXmlElementDecl
		public void GenerateXmlElementDecl(XmlElementDecl decl)
		{
			Code.Write("<!ELEMENT {0}", decl.Name);
			GenerateDescendantList(decl.DetailNodes, true);
			Code.WriteLine(">");
		}	
		void GenerateDescendantList(NodeList list, string delimiter, bool writeFirstDelimiter)
		{
			GenerateDescendantList(list, delimiter, writeFirstDelimiter, false);
		}
		void GenerateDescendantList(NodeList list, string delimiter, bool writeFirstDelimiter, bool writeDelimiterAfterElements)
		{
			GenerateDescendantList(list, delimiter, writeFirstDelimiter, writeDelimiterAfterElements, true);
		}
		void GenerateDescendantList(NodeList list, string delimiter, bool writeFirstDelimiter, bool writeDelimiterAfterElements, bool writeLastDelimiter)
		{
			GenerateDescendantList(list, delimiter, writeFirstDelimiter, writeDelimiterAfterElements, writeLastDelimiter, true);
		}
		void GenerateDescendantList(NodeList list, string delimiter, bool writeFirstDelimiter, bool writeDelimiterAfterElements, bool writeLastDelimiter, bool addNewLine)
		{
			if(list.Count == 0)
				return;		
			LanguageElement currentElement = null;
			int count = list.Count;
			for(int i = 0; i < count; i++)
			{
				currentElement = list[i] as LanguageElement;
				if (currentElement == null)
					continue;				
				if (writeFirstDelimiter && !writeDelimiterAfterElements)
				{
					Code.Write(delimiter);				
				}
				else
					writeFirstDelimiter = true;				
				GenerateElement(currentElement);
				if (writeDelimiterAfterElements && !(!writeLastDelimiter && i == (count - 1)))
					if (addNewLine)
						Code.WriteLine(delimiter);
					else
						Code.Write(delimiter);
			}
		}
		void GenerateDescendantList(NodeList list, string delimiter)
		{
			GenerateDescendantList(list, delimiter, false);
		}
		void GenerateDescendantList(NodeList list)
		{
			GenerateDescendantList(list, " ");
		}
		void GenerateDescendantList(NodeList list, bool writeFirstDelimiter)
		{
			GenerateDescendantList(list, " ", writeFirstDelimiter);
		}
		#endregion
		public void GenerateXmlEmptyContentSpec(XmlEmptyContentSpec spec)
		{
			Code.Write("EMPTY");
		}
		public void GenerateXmlEntityDecl(XmlEntityDecl decl)
		{
			Code.Write("<!ENTITY");
			if (decl.IsParameterEntity)
				Code.Write(" %");
			Code.Write(" {0}", decl.Name);
			if (decl.ImmediateValue != null && decl.ImmediateValue != String.Empty)
				Code.Write(" {0}", decl.ImmediateValue);
			if (decl.ExternalLinkValue != null)
			{
				Code.Write(" ");
				GenerateHtmlXmlNode(decl.ExternalLinkValue);
			}
			if (decl.NDataValue != null && decl.NDataValue != String.Empty)
				Code.Write(" NDATA {0}", decl.NDataValue);
			Code.WriteLine(">");
		}
		public void GenerateXmlExternalPublicID(NewExternalIDPublicLink link)
		{
			Code.Write("PUBLIC {0}", link.PublicID);
			WriteString(link.SystemURI);
		}
		public void GenerateXmlExternalSystemID(NewExternalIDSystemLink link)
		{
			Code.Write("SYSTEM {0}", link.SystemURI);			
		}
		public void GenerateXmlMixedContentSpec(XmlMixedContentSpec spec)
		{
			Code.Write("(");
			Code.Write("#PCDATA");			
			GenerateDescendantList(spec.DetailNodes, " | ", true);
			Code.Write(")");
		}
		public void GenerateXmlName(XmlName name)
		{
			Code.Write(name.Name);		
		}
		public void GenerateXmlNamedCP(XmlNamedContentParticle part)
		{
			GenerateDescendantList(part.DetailNodes);
		}
		public void GenerateXmlNameReference(XmlNameReference reference)
		{
			Code.Write("%{0};", reference.Name);
		}
		public void GenerateXmlNotationDecl(XmlNotationDecl decl)
		{
			Code.Write("<!NOTATION {0}", decl.Name);
			if (decl.NotationLink != null)
			{
				Code.Write(" ");
				GenerateHtmlXmlNode(decl.NotationLink);
			}
			Code.WriteLine(">");
		}
		public void GenerateXmlProcessingInstruction(XmlProcessingInstruction instr)
		{
	  if (instr.DetailNodeCount == 0)
		Code.WriteLine("<?{0} {1}?>", instr.Name, instr.InstructionText);
	  else
	  {
		Code.Write("<?{0}", instr.Name);
		GenerateNodesList(instr.DetailNodes, false);
		Code.WriteLine("?>");
	  }
		}
		public void GenerateXmlReference(XmlReference reference)
		{
			Code.Write("&{0};",reference.Name);
		}
		#endregion
	}
}
