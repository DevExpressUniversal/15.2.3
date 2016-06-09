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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{	
	public class AspElementBuilder: ElementBuilder
	{
	public AspElementBuilder()
		{
		}
		#region BuildHtmlElement
		public virtual HtmlElement BuildHtmlElement(string name, bool isEmptyTag, bool hasCloseTag)
		{
			HtmlElement element = new HtmlElement();
			element.Name = name;
			element.IsEmptyTag = isEmptyTag;
			element.HasCloseTag = hasCloseTag;
			return element;
		}
		#endregion
		#region BuildHtmlText
		public virtual HtmlText BuildHtmlText(string name)
		{
			HtmlText htmlText = new HtmlText();
			htmlText.Name = name;
			return htmlText;
		}
		#endregion
		#region BuildHtmlAttribute
		public virtual HtmlAttribute BuildHtmlAttribute(string name, string value, QuoteType quoteType, Expression exp)
		{
			HtmlAttribute htmlAttribute = new HtmlAttribute();
			if (value != null && value != String.Empty)
			{
				htmlAttribute.Value = value;
				htmlAttribute.HasValue = true;
			}
			else
			{
				htmlAttribute.HasValue = false;
			}
			htmlAttribute.Name = name;
			htmlAttribute.InlineExpression = exp;
			return htmlAttribute;
		}
		#endregion		
		#region BuildHtmlScriptDefinition
		public virtual HtmlScriptDefinition BuildHtmlScriptDefinition(string scriptText, DotNetLanguageType languageType)
		{
			HtmlScriptDefinition script = new HtmlScriptDefinition();
			script.HasCloseTag = true;
			script.Name = "script";			
			script.ScriptText = scriptText;
			script.Language = languageType;
			return script;
		}
		#endregion
		#region BuildHtmlScriptDefinition
		public virtual HtmlStyleDefinition BuildHtmlStyleDefinition(string styleText, DotNetLanguageType languageType)
		{
			HtmlStyleDefinition script = new HtmlStyleDefinition();
			script.HasCloseTag = true;
			script.Name = "style";
			script.StyleText= styleText;			
			return script;
		}
		#endregion		
		#region BuildAspDirective
		public virtual AspDirective BuildAspDirective(string name)
		{
			AspDirective element = new AspDirective();
			element.Name = name;
			element.IsEmptyTag = false;
			element.HasCloseTag = false;
			return element;
		}
		#endregion		
		#region BuildAspCodeEmbedding
		public virtual AspCodeEmbedding BuildAspCodeEmbedding(string code)
		{
			AspCodeEmbedding element = new AspCodeEmbedding();
			element.Code = code;
			element.IsEmptyTag = false;
			element.HasCloseTag = false;
			return element;
		}
		#endregion
		#region BuildBaseXmlNode
		public virtual BaseXmlNode BuildBaseXmlNode(string name)
		{
			BaseXmlNode element = new BaseXmlNode();
			element.Name = name;
			return element;
		}
		#endregion
		#region BuildXmlDecl
		public virtual XmlDecl BuildXmlDecl(string version, string encording, string standAlone)
		{
			XmlDecl element = new XmlDecl();
			element.Version = version;
			element.Encoding = encording;
			element.StandAlone = standAlone;
			return element;
		}
		#endregion
		#region BuildNewExternalIDSystemLink
		public virtual NewExternalIDSystemLink BuildNewExternalIDSystemLink(string systemURI)
		{
			NewExternalIDSystemLink element = new NewExternalIDSystemLink();
			element.SystemURI = systemURI;
			return element;
		}
		#endregion
		#region BuildNewExternalIDPublicLink
		public virtual NewExternalIDPublicLink BuildNewExternalIDPublicLink(string systemURI, string publicID)
		{
			NewExternalIDPublicLink element = new NewExternalIDPublicLink();
			element.SystemURI = systemURI;
			element.PublicID = publicID;
			return element;
		}
		#endregion
		#region BildXmlDocTypeDecl
		public virtual XmlDocTypeDecl BuildXmlDocTypeDecl(string name)
		{
			XmlDocTypeDecl element = new XmlDocTypeDecl();
			element.Name = name;
			return element;
		}
		#endregion		
		#region BuildXmlElementDecl
		public virtual XmlElementDecl BuildXmlElementDecl(string name)
		{
			XmlElementDecl element = new XmlElementDecl();
			element.Name = name;
			return element;
		}
		#endregion		
		#region BuildXmlEmptyContentSpec
		public virtual XmlEmptyContentSpec BuildXmlEmptyContentSpec()
		{
			XmlEmptyContentSpec element = new XmlEmptyContentSpec();
			return element;
		}
		#endregion
		#region	BuildXmlAnyContentSpec
		public virtual XmlAnyContentSpec BuildXmlAnyContentSpec()
		{
			XmlAnyContentSpec element = new XmlAnyContentSpec();
			return element;
		}
		#endregion
		#region BuildXmlMixedContentSpec
		public virtual XmlMixedContentSpec BuildXmlMixedContentSpec()
		{
			XmlMixedContentSpec element = new XmlMixedContentSpec();
			return element;
		}
		#endregion
		#region BuildXmlName
		public virtual XmlName BaseXmlName(string name)
		{
			XmlName element = new XmlName();
			element.Name = name;
			return element;
		}
		#endregion		
		#region BuildXmlNameReference
		public virtual XmlNameReference BuildXmlNameReference(string name)
		{
			XmlNameReference element = new XmlNameReference();
			element.Name = name;
			return element;
		}
		#endregion
		#region BuildXmlNamedContentParticle
		public virtual XmlNamedContentParticle BuildXmlNamedContentParticle()
		{
			XmlNamedContentParticle element = new XmlNamedContentParticle();
			return element;
		}
		#endregion		
		#region BuildXmlChoiceContentParticle
		public virtual XmlChoiceContentParticle BuildXmlChoiceContentParticle(RepeatCount repeatCount)
		{
			XmlChoiceContentParticle element = new XmlChoiceContentParticle();
			element.RepeatCount = repeatCount;
			return element;
		}
		#endregion	
		#region BuildXmlSequenceContentParticle
		public virtual XmlSequenceContentParticle BuildXmlSequenceContentParticle(RepeatCount repeatCount)
		{
			XmlSequenceContentParticle element = new XmlSequenceContentParticle();
			element.RepeatCount = repeatCount;
			return element;
		}
		#endregion
		#region BuildXmlChildrenContentSpec
		public virtual XmlChildrenContentSpec BuildXmlChildrenContentSpec()
		{
			XmlChildrenContentSpec element = new XmlChildrenContentSpec();
			return element;
		}
		public virtual XmlChildrenContentSpec BuildXmlChildrenContentSpec(XmlContentParticle source)
		{
			XmlChildrenContentSpec element = new XmlChildrenContentSpec();
			element.Source = source;
			return element;
		}
		#endregion				
		#region BuildXmlNotationDecl
		public virtual XmlNotationDecl BuildXmlNotationDecl(string name, NewExternalIDLink notationLink)
		{
			XmlNotationDecl element = new XmlNotationDecl();
			element.NotationLink = notationLink;
			return element;
		}
		#endregion		
		#region BuildXmlAttributeListDeclaration
		public virtual XmlAttributeListDeclaration BuildXmlAttributeListDeclaration(string name)
		{
			XmlAttributeListDeclaration element = new XmlAttributeListDeclaration();
			element.Name = name;
			return element;
		}
		#endregion
		#region BuildXmlAttribute
		public virtual XmlAttribute BuildXmlAttribute(string name, string value)
		{
			XmlAttribute element = new XmlAttribute();
			element.Name = name;
			element.Value = value;
			return element;
		}
		#endregion
		#region BuildXmlAttributeDeclaration
		public virtual XmlAttributeDeclaration BuildXmlAttributeDeclaration(string name,		AttributeType attributeType, DefaultAttributeValueType defaultAttributeValueType, string defaultAttributeValue)
		{
			XmlAttributeDeclaration element = new XmlAttributeDeclaration();
			element.Name = name;
			element.AttributeType = attributeType;
			element.DefaultAttributeValueType = defaultAttributeValueType;
			element.DefaultAttributeValue = defaultAttributeValue;
			return element;
		}
		#endregion
		#region BuildXmlEntityDecl
		public virtual XmlEntityDecl BuildXmlEntityDecl(string name, bool isParameterEntity, string immediateValue, NewExternalIDLink externalLinkValue, string nDataValue)
		{
			XmlEntityDecl element = new XmlEntityDecl();
			element.Name = name;
			element.IsParameterEntity = isParameterEntity;
			element.ImmediateValue = immediateValue;
			element.ExternalLinkValue = externalLinkValue;
			element.NDataValue = nDataValue;
			return element;
		}
		#endregion		
		#region BuildXmlProcessingInstruction
		public virtual XmlProcessingInstruction BuildXmlProcessingInstruction(string name, string instructionText)
		{
			XmlProcessingInstruction element = new XmlProcessingInstruction();
			element.Name = name;
			element.InstructionText = instructionText;
			return element;
		}
		#endregion
		#region BuildXmlCharReference
		public virtual XmlCharReference BuildXmlCharReference(string name)
		{
			XmlCharReference element = new XmlCharReference();
			element.Name = name;
			return element;
		}
		#endregion
		#region BuildXmlReference
		public virtual XmlReference BuildXmlReference(string name)
		{
			XmlReference element = new XmlReference();
			element.Name = name;
			return element;
		}
		#endregion		
		#region BuildXmlCharacterData
		public virtual XmlCharacterData BuildXmlCharacterData(string name)
		{
			XmlCharacterData element = new XmlCharacterData();
			element.Name = name;
			return element;
		}		 
		#endregion
		#region BuildCssStyleSheet
		public virtual CssStyleSheet BuildCssStyleSheet()
		{
			CssStyleSheet element = new CssStyleSheet();
			return element;
		}
		#endregion
		#region BuildCssStyleRule
		public virtual CssStyleRule BuildCssStyleRule()
		{
			CssStyleRule element = new CssStyleRule();
			return element;
		}
		#endregion
		#region BuildCssPropertyDeclaration
		public virtual CssPropertyDeclaration BuildCssPropertyDeclaration(string name, bool isEqual, bool isImport)
		{
			CssPropertyDeclaration element = new CssPropertyDeclaration();
			element.Name = name;
			element.IsEqual = isEqual;
			element.IsImportant = isImport;
			return element;
		}
		#endregion
		#region BulidCssSelector
		public virtual CssSelector BuildCssSelector(string name, CssSelectorType selectorType, CssSelector ancestor)
		{
			CssSelector element = new CssSelector();
			element.Name = name;
			element.SelectorType = selectorType;
	  element.SetParent(ancestor);
			return element;
		}
		#endregion
		#region BuildCssAttributeSelector
		public virtual CssAttributeSelector BuildCssAttributeSelector(string name, string value, AttributeSelectorEqualityType equalityType)
		{
			CssAttributeSelector element = new CssAttributeSelector();
			element.Name = name;
			element.Value = value;
			element.EqualityType = equalityType;
			return element;
		}
		public virtual CssAttributeSelector BuildCssAttributeSelector(string name, string value, AttributeSelectorEqualityType equalityType, bool isStringValue)
		{
			CssAttributeSelector element = BuildCssAttributeSelector(name, value, equalityType);
			element.IsStringValue = isStringValue;
			return element;
		}
		#endregion
		#region BuildCssPseudoSelector
		public virtual CssPseudoSelector BuildCssPseudoSelector(string name)
		{
			CssPseudoSelector element = new CssPseudoSelector();
			element.Name = name;
			return element;
		}
		#endregion
		#region BuildCssPseudoFunctionSelector
		public virtual CssPseudoFunctionSelector BuildCssPseudoFunctionSelector(string name, string param)
		{
			CssPseudoFunctionSelector element = new CssPseudoFunctionSelector();
			element.Name = name;
			element.Param = param;
			return element;
		}
		#endregion
		#region BuildCssIdentifierReference
		public virtual CssIdentifierReference BuildCssIdentifierReference(string name, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssIdentifierReference element = new CssIdentifierReference();
			element.Name = name;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssFunctionReference
		public virtual CssFunctionReference BuildCssFunctionReference(string name, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssFunctionReference element = new CssFunctionReference();
			element.Name = name;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssStringLiteral
		public virtual CssStringLiteral BuildCssStringLiteral(string name, string value, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssStringLiteral element = new CssStringLiteral();
			element.Name = name;
			element.Value	= value;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssURIReference
		public virtual CssURIReference BuildCssURIReference(string name, string URI, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssURIReference element = new CssURIReference();
			element.Name = name;
			element.URI	= URI;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssColorReference
		public virtual CssColorReference BuildCssColorReference(string name, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssColorReference element = new CssColorReference();
			element.Name = name;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssNumberLiteral
		public virtual CssNumberLiteral BuildCssNumberLiteral(string name, object value, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssNumberLiteral element = new CssNumberLiteral();
			element.Name = name;
			element.Value = value;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssLengthLiteral
		public virtual CssLengthLiteral BuildCssLengthLiteral(string name, object value, CssMeasureUnit unit, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssLengthLiteral element = new CssLengthLiteral();
			element.Name = name;
			element.Value = value;
			element.Unit = unit;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssPercentLiteral
		public virtual CssPercentLiteral BuildCssPercentLiteral(string name, object value, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssPercentLiteral element = new CssPercentLiteral();
			element.Name = name;
			element.Value = value;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssTimeLiteral
		public virtual CssTimeLiteral BuildCssTimeLiteral(string name, object value, CssMeasureUnit unit, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssTimeLiteral element = new CssTimeLiteral();
			element.Name = name;
			element.Value = value;
			element.Unit = unit;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssAngleLiteral
		public virtual CssAngleLiteral BuildCssAngleLiteral(string name, object value, CssMeasureUnit unit, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssAngleLiteral element = new CssAngleLiteral();
			element.Name = name;
			element.Value = value;
			element.Unit = unit;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssFrequencyLiteral
		public virtual CssFrequencyLiteral BuildCssFrequencyLiteral(string name, object value, CssMeasureUnit unit, ExpressionDelimiter delimiter, PrecedingSign precedingSign)
		{
			CssFrequencyLiteral element = new CssFrequencyLiteral();
			element.Name = name;
			element.Value = value;
			element.Unit = unit;
			element.Delimiter = delimiter;
			element.Sign = precedingSign;
			return element;
		}
		#endregion
		#region BuildCssCharsetDeclaration
		public virtual CssCharsetDeclaration BuildCssCharsetDeclaration(string charset)
		{
			CssCharsetDeclaration element = new CssCharsetDeclaration();
			element.Charset = charset;			
			return element;
		}
		#endregion
		#region BuildCssImportDirective
		public virtual CssImportDirective BuildCssImportDirective(string source)
		{
			CssImportDirective element = new CssImportDirective();
			element.Source= source;			
			return element;
		}
		#endregion
		#region BuildCssMediaDirective
		public virtual CssMediaDirective BuildCssMediaDirective()
		{
			CssMediaDirective element = new CssMediaDirective();	
			return element;
		}
		#endregion
		#region BuildCssPageStyle
		public virtual CssPageStyle BuildCssPageStyle(string name)
		{
			CssPageStyle element = new CssPageStyle();
			element.Name = name;
			return element;
		}
		#endregion
		#region BuildCssCommentTag
		public virtual CssCommentTag BuildCssCommentTag(CommentTagKind kind)
		{
			CssCommentTag element = new CssCommentTag();
			element.Kind= kind;
			return element;
		}
		#endregion
		#region BuildXmlElement
		public virtual XmlElement BuildXmlElement(string name)
		{
			XmlElement element = new XmlElement();
			element.Name = name;			
			return element;
		}
		#endregion		
		#region AddHtmlAttribute
		public HtmlAttribute AddHtmlAttribute(HtmlElement parent, string attributeName, string attributeValue, QuoteType attributeQuoteType, Expression attributeExp)
		{
			HtmlAttribute element = BuildHtmlAttribute(attributeName, attributeValue, attributeQuoteType, attributeExp);
			AddDetailNode(parent, element);
			return element;
		}
		#endregion
		#region AddXmlAttributeDeclaration
		public XmlAttributeDeclaration AddXmlAttribute(XmlAttributeListDeclaration parent, string attributeName, AttributeType attributeType, DefaultAttributeValueType defaultAttributeValueType, string defaultAttributeValue)
		{
			XmlAttributeDeclaration attribute = BuildXmlAttributeDeclaration(attributeName, attributeType, defaultAttributeValueType, defaultAttributeValue);
			AddNode(parent, attribute);
			parent.AttributesDecl.Add(attribute);
			return attribute;
		}		
		#endregion
		#region AddXmlAttribute
		public XmlAttribute AddXmlAttribute(XmlElement parent, string attributeName, string attributeValue)
		{
			XmlAttribute attribute = BuildXmlAttribute(attributeName, attributeValue);
			AddDetailNode(parent, attribute);
			return attribute;
		}
		#endregion
		#region AddXmlAttribute
		public CssAttributeSelector AddXmlAttribute(XmlElement parent, string attributeName, string attributeValue, AttributeSelectorEqualityType attributeEqualityType, bool isStringAttributeValue)
		{
			CssAttributeSelector attribute = BuildCssAttributeSelector(attributeName, attributeValue, attributeEqualityType, isStringAttributeValue);
			AddDetailNode(parent, attribute);
			return attribute;
		}
		public CssAttributeSelector AddXmlAttribute(XmlElement parent, string attributeName, string attributeValue, AttributeSelectorEqualityType attributeEqualityType)
		{
			CssAttributeSelector attribute = BuildCssAttributeSelector(attributeName, attributeValue, attributeEqualityType);
			AddDetailNode(parent, attribute);
			return attribute;
		}
		#endregion
		#region AddXmlElement
		public XmlElement AddXmlElement(LanguageElement parent, string xmlElementName)
		{
			XmlElement element = BuildXmlElement(xmlElementName);
			AddNode(parent, element);
			return element;
		}
		#endregion
		#region AddXmlElement
		public HtmlElement AddHtmlElement(LanguageElement parent, string htmlElementName, bool isEmptyHtmlTag, bool hasCloseHtmlTag)
		{
			HtmlElement element = BuildHtmlElement(htmlElementName, isEmptyHtmlTag, hasCloseHtmlTag);
			AddNode(parent, element);
			return element;
		}
		#endregion
		public Comment AddHtmlComment(string commentText)
		{
			Comment comment = this.BuildComment(commentText, CommentType.MultiLine);
			AddNode(null, comment);
			return comment;
		}
	}
}
