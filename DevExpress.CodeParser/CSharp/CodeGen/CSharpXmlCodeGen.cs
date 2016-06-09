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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
  public class CSharpXmlCodeGen : XmlCodeGenBase
  {
	const string STR_Prefix = "///";
	bool _LastXMLTextIsEmpty;
	public CSharpXmlCodeGen(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected override void GenerateXmlPrefix()
	{
	  Code.Write(STR_Prefix);
	}
	#region Generate
	public override void GenerateElement(LanguageElement element)
	{
	  if (element == null)
		return;
	  switch (element.ElementType)
	  {
		case LanguageElementType.XmlDocComment:
		  GenerateXmlDocComment(element as XmlDocComment);
		  break;
		case LanguageElementType.XmlDocAttribute:
		  GenerateXmlDocAttribute(element as XmlAttribute);
		  break;
		case LanguageElementType.XmlDocText:
		  GenerateXmlDocText(element as XmlText);
		  break;
		case LanguageElementType.XmlCharacterData:
		  GenerateXmlCharacterData(element as XmlCharacterData);
		  break;
		case LanguageElementType.XmlDocElement:
		  GenerateXmlDocElement(element as XmlElement);
		  break;
		case LanguageElementType.XmlReference:
		  GenerateXmlReference(element as XmlReference);
		  break;
	  }
	}
	private void GenerateXmlReference(XmlReference xmlReference)
	{
	  Write(FormattingTokenType.Ampersand);
	  Write(xmlReference.Name);
	  Write(FormattingTokenType.Semicolon);
	}
	#endregion
	#region GenerateXmlDocComment
	protected override void GenerateXmlDocComment(XmlDocComment xmlComment)
	{
	  if (xmlComment == null || CodeGen.IsSkiped(xmlComment))
		return;
	  if(SaveFormat)
		CodeGen.Code.IncreaseAlignment();
	  if (xmlComment.NodeCount == 0)
	  {
		CodeGen.TokenGen.TokenGenArgs.Reset();
		Code.Write(xmlComment.Name, false);
		CodeGen.AddNewLineIfNeeded();
	  }
	  else
		GenerateElementCollection(xmlComment.Nodes, FormattingTokenType.None);
	  if(SaveFormat)
		CodeGen.Code.DecreaseAlignment();
	  CodeGen.AddSkiped(xmlComment);
	}
	#endregion
	#region GenerateXmlDocElement
	protected override void GenerateXmlDocElement(XmlElement xmlDocElement)
	{
	  if (xmlDocElement == null)
		return;
	  CodeWritePrevFormattingElements(FormattingTokenType.Ident);
	  GenerateXmlElementOpenTag(xmlDocElement);
	  if (!xmlDocElement.HasCloseTag)
		return;
	  GenerateElementCollection(xmlDocElement.Nodes, FormattingTokenType.None);
	  GenerateXmlElementCloseTag(xmlDocElement);
	  CodeWriteNextFormattingElements(FormattingTokenType.Ident);
	  if (SaveFormat && !(xmlDocElement.Parent is XmlDocComment && xmlDocElement.NextCodeSibling == null))
		CodeGen.Code.WriteLine();
	}
	#endregion
	bool NeedGenerateStartPrefixForOpenTag(XmlElement xmlDocElement)
	{
	  if (xmlDocElement == null)
		return false;
	  if (xmlDocElement.Range.IsEmpty)
		return true;
	  LanguageElement prevContext = CodeGen.GetPrevContext();
	  LanguageElement previousSibling = xmlDocElement.PreviousSibling;
	  if (previousSibling != null)
	  {
		if (previousSibling.Range.IsEmpty || previousSibling.EndLine != xmlDocElement.StartLine ||
		_LastXMLTextIsEmpty && previousSibling is XmlText)
		  return true;
	  }
	  else
		if (prevContext == null || prevContext is XmlDocComment ||
		prevContext.Range.IsEmpty || prevContext.StartLine < xmlDocElement.StartLine)
		  return true;
	  return false;
	}
	#region GenerateXmlElementOpenTag
	protected override void GenerateXmlElementOpenTag(XmlElement xmlDocElement)
	{
	  if (xmlDocElement == null)
		return;
	  if (NeedGenerateStartPrefixForOpenTag(xmlDocElement))
	  {
		GenerateXmlPrefix();
		Code.Write(" ");
	  }
	  Code.Write("<");
	  Code.Write(xmlDocElement.Name);
	  if (xmlDocElement.XmlAttributes != null &&
	  xmlDocElement.XmlAttributes.Count > 0)
		Code.Write(" ");
	  GenerateElementCollection(xmlDocElement.XmlAttributes, FormattingTokenType.None);
	  if (xmlDocElement.HasCloseTag)
		Code.Write(">");
	  else
		Code.Write("/>");
	}
	#endregion
	bool NeedGenerateStartPrefixForCloseTag(XmlElement element)
	{
	  if (element == null)
		return false;
	  LanguageElement lastChild = element.LastChild;
	  return lastChild != null &&
		(lastChild.Range.End.Line != element.Range.End.Line && 
		!lastChild.Range.IsEmpty && !element.Range.IsEmpty ||
		lastChild is XmlText && _LastXMLTextIsEmpty);
	}
	#region GenerateXmlElementCloseTag    
	protected override void GenerateXmlElementCloseTag(XmlElement xmlDocElement)
	{
	  if (xmlDocElement == null)
		return;
	  if (NeedGenerateStartPrefixForCloseTag(xmlDocElement))
	  {
		GenerateXmlPrefix();
		Code.Write(" ");
	  }
	  Code.Write("<");
	  Code.Write("/");
	  Code.Write(xmlDocElement.Name);
	  Code.Write(">");
	}
	#endregion
	#region GenerateXmlAttribute
	private void GenerateXmlDocAttribute(XmlAttribute element)
	{
	  if (element == null)
		return;
	  CodeWritePrevFormattingElements(FormattingTokenType.Ident);
	  Code.Write(element.Name);
	  Code.Write("=");
	  GenerateQuote();
	  Code.Write(element.Value);
	  GenerateQuote();
	  CodeWriteNextFormattingElements(FormattingTokenType.Ident);
	}
	#endregion
	bool ContainsValuableText(XmlText xmlDocText)
	{
	  if (xmlDocText == null)
		return false;
	  string text = xmlDocText.Text;
	  if (string.IsNullOrEmpty(text))
		return false;
	  text = text.Trim();
	  return !string.IsNullOrEmpty(text);
	}
	private void GenerateXmlCharacterData(XmlCharacterData xmlCharacterData)
	{
	  if (xmlCharacterData == null)
		return;
	  TokenGen.TokenGenArgs.Reset();
	  string text = @" <![CDATA[
" + xmlCharacterData.Name + @"]]>";
	  GenerateXmlPrefix();
	  WriteXmlString(text, xmlCharacterData);
	}
	void WriteXmlString(string str, LanguageElement element)
	{
	  string[] lDocTextLines = StringHelper.SplitLines(str, false);
	  _LastXMLTextIsEmpty = false;
	  for (int i = 0; i < lDocTextLines.Length; i++)
	  {
		if (i == 0)
		  Code.Write(lDocTextLines[i]);
		else
		{
		  string trimmed = lDocTextLines[i].TrimStart();
		  if (!(string.IsNullOrEmpty(trimmed) && i == lDocTextLines.Length - 1))
		  {
			Code.WriteLine();
			GenerateXmlPrefix();
			if (!string.IsNullOrEmpty(trimmed))
			  Code.Write(" " + trimmed);
		  }
		  else
		  {
			_LastXMLTextIsEmpty = true;
			LanguageElement allComment = element.Parent;
			if (!(allComment is XmlDocComment) || allComment.LastChild != element as LanguageElement)
			  Code.WriteLine();
		  }
		}
	  }
	  CodeWriteNextFormattingElements(FormattingTokenType.Ident);
	}
	#region GenerateXmlDocText
	protected override void GenerateXmlDocText(XmlText xmlDocText)
	{
	  if (xmlDocText == null)
		return;
	  TokenGen.TokenGenArgs.Reset();
	  string[] lDocTextLines = StringHelper.SplitLines(xmlDocText.Text, false);
	  _LastXMLTextIsEmpty = false;
	  for (int i = 0; i < lDocTextLines.Length; i++)
	  {
		if (i == 0)
		  Code.Write(lDocTextLines[i]);
		else
		{
		  string trimmed = lDocTextLines[i].TrimStart();
		  if (!(string.IsNullOrEmpty(trimmed) && i == lDocTextLines.Length - 1))
		  {
			Code.WriteLine();
			GenerateXmlPrefix();
			if (!string.IsNullOrEmpty(trimmed))
			  Code.Write(" " + trimmed);
		  }
		  else
		  {
			_LastXMLTextIsEmpty = true;
			LanguageElement allComment = xmlDocText.Parent;
			if (!(allComment is XmlDocComment) || allComment.LastChild != xmlDocText as LanguageElement)
			  Code.WriteLine();
		  }
		}
	  }
	  CodeWriteNextFormattingElements(FormattingTokenType.Ident);
	}
	#endregion
	internal override bool IsXmlGenElement(LanguageElement element)
	{
	  if (element == null)
		return false;
	  LanguageElementType type = element.ElementType;
	  return type == LanguageElementType.XmlDocComment
	  || type == LanguageElementType.XmlDocElement
	  || type == LanguageElementType.XmlDocAttribute
	  || type == LanguageElementType.XmlDocText
	  || type == LanguageElementType.XmlReference
	  || type == LanguageElementType.XmlCharacterData;
	}
	#region  GenerateCode
	public override void GenerateCode(CodeWriter writer, LanguageElement languageElement, bool calculateIndent)
	{
	  if (writer == null)
		throw new ArgumentNullException("writer");
	  if (CodeGen == null)
		throw new ArgumentNullException("CodeGen is Null");
	  PushCodeWriter();
	  SetCodeWriter(writer);
	  if (calculateIndent)
		CalculateIndent(languageElement);
	  CodeGen.GenerateElement(languageElement);
	  if (calculateIndent)
		ResetIndent();
	  PopCodeWriter();
	}
	#endregion
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (element == null || !IsXmlGenElement(element))
		return false;
	  if (element.ElementType == LanguageElementType.XmlDocElement
	  && Context != null && ContextMatch(LanguageElementType.XmlDocComment))
	  {
		CodeGen.AddNewLineIfNeeded();
		return true;
	  }
	  return false;
	}
  }
}
