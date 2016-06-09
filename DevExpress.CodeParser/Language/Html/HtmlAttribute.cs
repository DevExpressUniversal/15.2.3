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
  public enum QuoteType
  {
	None,
	SingleQuote,
	DoubleQuote
  }
  public class HtmlAttribute : XmlAttribute, IHtmlAttribute, IMarkupElement
  {
	bool _HasValue = false;
	QuoteType _QuoteType = QuoteType.None;
	Expression _InlineExpression = null;
	CssPropertyDeclarationCollection _StyleProperties = null;
	bool _ValueIsAspCodeEmbedding = false;
	LanguageElementCollection _ScriptCode;
	DotNetLanguageType _ScriptLanguage = DotNetLanguageType.Unknown;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is HtmlAttribute))
		return;
	  HtmlAttribute attributeSource = (HtmlAttribute)source;
	  _HasValue = attributeSource._HasValue;
	  _QuoteType = attributeSource._QuoteType;
	  if (attributeSource._InlineExpression != null)
	  {
		_InlineExpression = ParserUtils.GetCloneFromNodeList(DetailNodes, attributeSource.DetailNodes, attributeSource.InlineExpression) as Expression;
		if (_InlineExpression == null)
		{
		  _InlineExpression = attributeSource._InlineExpression.Clone() as Expression;
		  _InlineExpression.SetParent(this);
		}
		else
		  _InlineExpression = null;
	  }
	  if (attributeSource._StyleProperties != null)
		_StyleProperties = attributeSource._StyleProperties.DeepClone(options) as CssPropertyDeclarationCollection;
	  else
		_StyleProperties = null;
	  if (attributeSource._ScriptCode != null)
		_ScriptCode = attributeSource._ScriptCode.DeepClone(options) as LanguageElementCollection;
	  else
		_ScriptCode = null;
	  _ScriptLanguage = attributeSource._ScriptLanguage;
	  _ValueIsAspCodeEmbedding = attributeSource.ValueIsAspCodeEmbedding;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  HtmlAttribute lClone = new HtmlAttribute();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	#region AttributeQuoteType
	public QuoteType AttributeQuoteType
	{
	  get
	  {
		return _QuoteType;
	  }
	  set
	  {
		_QuoteType = value;
	  }
	}
	#endregion
	#region ValueIsAspCodeEmbedding
	public bool ValueIsAspCodeEmbedding
	{
	  get
	  {
		return _ValueIsAspCodeEmbedding;
	  }
	  set
	  {
		_ValueIsAspCodeEmbedding = value;
	  }
	}
	#endregion
	#region HasValue
	public bool HasValue
	{
	  get
	  {
		return _HasValue;
	  }
	  set
	  {
		_HasValue = value;
	  }
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.HtmlAttribute;
	  }
	}
	#endregion
	#region InlineExpression
	public Expression InlineExpression
	{
	  get
	  {
		return _InlineExpression;
	  }
	  set
	  {
		_InlineExpression = value;
	  }
	}
	#endregion
	#region StyleProperties
	public CssPropertyDeclarationCollection StyleProperties
	{
	  get
	  {
		return _StyleProperties;
	  }
	  set
	  {
		_StyleProperties = value;
	  }
	}
	#endregion
	#region ScriptCode
	public LanguageElementCollection ScriptCode
	{
	  get { return _ScriptCode; }
	  set { _ScriptCode = value; }
	}
	#endregion
	#region ScriptLanguage
	public DotNetLanguageType ScriptLanguage
	{
	  get { return _ScriptLanguage; }
	  set { _ScriptLanguage = value; }
	}
	#endregion
	#region InlineExpression
	IExpression IHtmlAttribute.InlineExpression
	{
	  get { return _InlineExpression; }
	}
	#endregion
	#region ScriptLanguage
	DotNetLanguageType IHtmlAttribute.ScriptLanguage
	{
	  get { return _ScriptLanguage; }
	}
	#endregion
  }
}
