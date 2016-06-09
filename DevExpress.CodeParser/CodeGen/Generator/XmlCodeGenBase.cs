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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class XmlCodeGenBase : LanguageElementCodeGenBase
  {
	XmlNode _LastGeneratedNode;
	#region XmlCodeGenBase
	public XmlCodeGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	  _LastGeneratedNode = null;
	}
	#endregion
	void GenerateReference(XmlReference xmlReference)
	{
	  if (xmlReference == null)
		return;
	  Code.Write("&{0};", xmlReference.Name);
	}
	void GenerateBaseXmlNode(BaseXmlNode baseXmlNode)
	{
	  if (baseXmlNode == null)
		return;
	  switch (baseXmlNode.XmlNodeType)
	  {
		case XmlNodeType.Reference:
		  GenerateReference(baseXmlNode as XmlReference);
		  break;
	  }
	}
	protected void GenerateQuote()
	{
	  Code.Write("\"");
	}
	protected abstract void GenerateXmlPrefix();
	protected virtual void GenerateXmlDocComment(XmlDocComment xmlComment)
	{
	  _LastGeneratedNode = null;
	  if (xmlComment == null)
		return;
	  if (xmlComment.NodeCount == 0)
	  {
		GenerateEmptyComment(xmlComment);
		return;
	  }
	  StartXmlCommentLine(xmlComment);
	  GenerateXmlNodes(xmlComment.Nodes);
	}
	protected virtual void StartXmlCommentLine(XmlDocComment xmlComment)
	{
	  if (xmlComment == null)
		return;
	  Code.Indent();
	}
	protected virtual void GenerateEmptyComment(XmlDocComment xmlComment)
	{
	  if (xmlComment != null && xmlComment.Name != null)
		Code.Write(xmlComment.Name);
	}
	protected virtual void GenerateXmlNodes(NodeList xmlNodes)
	{
	  if (xmlNodes == null || xmlNodes.Count == 0)
		return;
	  for (int i = 0; i < xmlNodes.Count; i++)
	  {
		XmlNode lNode = xmlNodes[i] as XmlNode;
		if (lNode == null)
		  continue;
		GenerateXmlNode(lNode);
	  }
	}
	protected virtual void GenerateXmlNode(XmlNode xmlNode)
	{
	  if (xmlNode == null)
		return;
	  switch (xmlNode.ElementType)
	  {
		case LanguageElementType.XmlDocElement:
		  if (xmlNode is BaseXmlNode)
			GenerateBaseXmlNode(xmlNode as BaseXmlNode);
		  else
			GenerateXmlDocElement(xmlNode as XmlElement);
		  break;
		case LanguageElementType.XmlDocText:
		  GenerateXmlDocText(xmlNode as XmlText);
		  break;
	  }
	}
	protected virtual void GenerateXmlDocElement(XmlElement xmlDocElement)
	{
	  if (xmlDocElement == null)
		return;
	  GenerateXmlElementOpenTag(xmlDocElement);
	  GenerateXmlNodes(xmlDocElement.Nodes);
	  GenerateXmlElementCloseTag(xmlDocElement);
	}
	protected virtual void GenerateXmlDocText(XmlText xmlDocText)
	{
	  if (xmlDocText == null)
		return;
	  string commentText = xmlDocText.Text;
	  if (!String.IsNullOrEmpty(commentText) && (xmlDocText.Parent is XmlDocComment))
		commentText = commentText.TrimStart(' ', '\r', '\n');
	  string[] lDocTextLines = StringHelper.SplitLines(commentText);
	  bool lEndsWithTerminator = StringHelper.EndsWithLineTerminator(commentText);
	  string lLineText = String.Empty;
	  for (int i = 0; i < lDocTextLines.Length; i++)
	  {
		if (i == 0 && !(xmlDocText.Parent is XmlDocComment))
		  Code.Write(lDocTextLines[i]);
		else
		{
		  lLineText = " " + lDocTextLines[i].TrimStart();
		  Code.WriteLine();
		  GenerateXmlPrefix();
		  Code.Write(lLineText);
		}
	  }
	  if (lEndsWithTerminator)
	  {
		Code.WriteLine();
		GenerateXmlPrefix();
		Code.Write(" ");
	  }
	  _LastGeneratedNode = xmlDocText;
	}
	protected virtual void GenerateXmlElementOpenTag(XmlElement xmlDocElement)
	{
	  if (xmlDocElement == null)
		return;
	  if (_LastGeneratedNode == null || _LastGeneratedNode.ElementType != LanguageElementType.XmlDocText)
	  {
		if (_LastGeneratedNode != null)
		  Code.WriteLine();
		GenerateXmlPrefix();
		Code.Write(" ");
	  }
	  Code.Write("<");
	  Code.Write(xmlDocElement.Name);
	  if (xmlDocElement.XmlAttributes != null && xmlDocElement.XmlAttributes.Count > 0)
		Code.Write(" ");
	  GenerateXmlAttributesList(xmlDocElement.XmlAttributes);
	  Code.Write(">");
	  _LastGeneratedNode = xmlDocElement;
	}
	protected virtual void GenerateXmlElementCloseTag(XmlElement xmlDocElement)
	{
	  if (xmlDocElement == null)
		return;
	  Code.Write("<");
	  Code.Write("/");
	  Code.Write(xmlDocElement.Name);
	  Code.Write(">");
	  _LastGeneratedNode = xmlDocElement;
	}
	protected virtual void GenerateXmlAttributesList(NodeList xmlAttributes)
	{
	  if (xmlAttributes == null)
		return;
	  for (int i = 0; i < xmlAttributes.Count; i++)
	  {
		GenerateXmlAttribute(xmlAttributes[i] as XmlAttribute);
		if (i < xmlAttributes.Count - 1)
		  Code.Write(", ");
	  }
	}
	protected virtual void GenerateXmlAttribute(XmlAttribute xmlAttribute)
	{
	  if (xmlAttribute == null)
		return;
	  Code.Write(xmlAttribute.Name);
	  Code.Write("=");
	  GenerateQuote();
	  Code.Write(xmlAttribute.Value);
	  GenerateQuote();
	}
	internal virtual bool IsXmlGenElement(LanguageElement element)
	{
	  return element is XmlDocComment;
	}
	public override void GenerateElement(LanguageElement element)
	{
	  if (element == null)
		return;
	  if (element is XmlDocComment)
		GenerateXmlDocComment(element as XmlDocComment);
	  else
		if (element is XmlElement && element.ElementType == LanguageElementType.XmlDocElement)
		  GenerateXmlDocElement(element as XmlElement);
	}
  }
}
