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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum HtmlElementType
  {
	Unknown,
	A,
	Abbr,
	Acronym,
	Address,
	Applet,
	Area,
	B,
	Base,
	Basefont,
	Bdo,
	Big,
	Blockquote,
	Body,
	Br,
	Button,
	Caption,
	Center,
	Cite,
	Code,
	Col,
	Colgroup,
	Dd,
	Del,
	Dfn,
	Dir,
	Div,
	Dl,
	Dt,
	Em,
	Fieldset,
	Font,
	Form,
	Frame,
	Frameset,
	H1,
	H2,
	H3,
	H4,
	H5,
	H6,
	Head,
	Hr,
	Html,
	I,
	Iframe,
	Img,
	Input,
	Ins,
	Isindex,
	Kbd,
	Label,
	Legend,
	Li,
	Link,
	Map,
	Menu,
	Meta,
	Noframes,
	Noscript,
	Object,
	Ol,
	Optgroup,
	Option,
	P,
	Param,
	Pre,
	Q,
	S,
	Samp,
	Script,
	Select,
	Small,
	Span,
	Strike,
	Strong,
	Style,
	Sub,
	Sup,
	Table,
	Tbody,
	Td,
	Textarea,
	Tfoot,
	Th,
	Thead,
	Title,
	Tr,
	Tt,
	U,
	Ul,
	Var
  }
  public class HtmlElement : XmlElement, IHtmlElement, IMarkupElement
  {
	HtmlElementType _HtmlElementType;
	bool _IsEmptyTag = false;
	SourceRange _InnerRange = SourceRange.Empty;
	SourceRange _NameRange = SourceRange.Empty;
	SourceRange _CloseTagNameRange = SourceRange.Empty;
	HtmlAttributeCollection _Attributes = null;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is HtmlElement))
		return;
	  HtmlElement lSource = (HtmlElement)source;
	  _NameRange = lSource.NameRange;
	  _HtmlElementType = lSource._HtmlElementType;
	  _IsEmptyTag = lSource._IsEmptyTag;
	  _InnerRange = lSource.InnerRange;
	  _CloseTagNameRange = lSource.CloseTagNameRange;
	  _Attributes = new HtmlAttributeCollection();
	  ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Attributes, lSource.Attributes);
	}
	#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	  _InnerRange = InnerRange;
	  _CloseTagNameRange = CloseTagNameRange;
	}
	#region GetAttributeValue
	protected string GetAttributeValue(string name)
	{
	  if (name == null)
		return String.Empty;
	  HtmlAttribute attribute = this.Attributes[name];
	  if (attribute != null && attribute.Value != null)
		return attribute.Value;
	  else
		return String.Empty;
	}
	#endregion
	#region SetAttributeValue
	protected void SetAttributeValue(string name, string value, QuoteType quoteType)
	{
	  if (name == null || value == null)
		return;
	  HtmlAttribute attribute = this.Attributes[name];
	  if (attribute == null)
	  {
		attribute = new HtmlAttribute();
		attribute.Name = name;
		attribute.AttributeQuoteType = quoteType;
		AddDetailNode(attribute);
		Attributes.Add(attribute);
	  }
	  attribute.Value = value;
	}
	#endregion
	protected void SetAttributeValue(string name, string value)
	{
	  SetAttributeValue(name, value, QuoteType.DoubleQuote);
	}
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  return ImageIndex.XmlElement;
	}
	#endregion
	#region FindHtmlElementByType
	public HtmlElement FindHtmlElementByType(HtmlElementType type)
	{
	  if (NodeCount == 0)
		return null;
	  for (int i = 0; i < NodeCount; i++)
	  {
		if (Nodes[i] is HtmlElement)
		{
		  HtmlElement htmlNode = Nodes[i] as HtmlElement;
		  if (htmlNode.HtmlElementType == type)
			return htmlNode;
		  else
		  {
			HtmlElement tempNode = htmlNode.FindHtmlElementByType(type);
			if (tempNode != null)
			  return tempNode;
		  }
		}
	  }
	  return null;
	}
	#endregion
	#region AddAttribute
	public void AddAttribute(string name, string value)
	{
	  SetAttributeValue(name, value);
	}
	public void AddAttribute(string name, string value, QuoteType quoteType)
	{
	  SetAttributeValue(name, value, quoteType);
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  HtmlElement lClone = new HtmlElement();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.HtmlElement;
	  }
	}
	#endregion
	#region IsEmptyTag
	public bool IsEmptyTag
	{
	  get
	  {
		return _IsEmptyTag;
	  }
	  set
	  {
		_IsEmptyTag = value;
	  }
	}
	#endregion
	#region HtmlElementType
	public HtmlElementType HtmlElementType
	{
	  get
	  {
		return _HtmlElementType;
	  }
	  set
	  {
		_HtmlElementType = value;
	  }
	}
	#endregion
	#region InnerRange
	public SourceRange InnerRange
	{
	  get
	  {
		return GetTransformedRange(_InnerRange);
	  }
	  set
	  {
		ClearHistory();
		_InnerRange = value;
	  }
	}
	#endregion
	#region Attributes
	public HtmlAttributeCollection Attributes
	{
	  get
	  {
		if (_Attributes == null)
		  _Attributes = new HtmlAttributeCollection();
		return _Attributes;
	  }
	}
	#endregion
	#region NameRange
	public override SourceRange NameRange
	{
	  get
	  {
		return base.GetTransformedRange(this._NameRange);
	  }
	  set
	  {
		ClearHistory();
		this._NameRange = (TextRange)value;
	  }
	}
	#endregion
	#region CloseTagNameRange
	public virtual SourceRange CloseTagNameRange
	{
	  get
	  {
		return base.GetTransformedRange(this._CloseTagNameRange);
	  }
	  set
	  {
		ClearHistory();
		this._CloseTagNameRange = (TextRange)value;
	  }
	}
	 #endregion
	[Category("Block Delimiters")]
	public override bool HasDelimitedBlock
	{
	  get
	  {
		return base.HasDelimitedBlock && HasCloseTag && !IsEmptyTag;
	  }
	}
	IHtmlAttributeCollection IHtmlElement.Attributes
	{
	  get
	  {
		return _Attributes;
	  }
	}
  }
}
