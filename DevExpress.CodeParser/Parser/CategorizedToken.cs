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
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum TokenLanguage
  {
	Unknown,
	CSharp,
	Basic,
	Html,
	Css,
	Xml,
	JavaScript,
	FSharp,
	CPlusPlus
  }
	public enum TokenCategory
	{
	Text,
	Keyword,
	Operator,
	PreprocessorKeyword,
	String,
	Number,
	Comment,
	XmlComment,
	Identifier,
	CssComment,
	CssKeyword,
	CssPropertyName,
	CssPropertyValue,
	CssSelector,
	CssStringValue,
	HtmlServerSideScript,
	HtmlElementName,
	HtmlEntity,
	HtmlOperator,
	HtmlComment,
	HtmlAttributeName,
	HtmlAttributeValue,
	HtmlString,
	HtmlTagDelimiter,
	Unknown
	}
	public class CategorizedToken : Token
	{
	TokenCategory _Category = TokenCategory.Unknown;
	TokenLanguage _Language = TokenLanguage.Unknown;
	bool _WasCheckedCategorizedToken;
	public bool WasCheckedCategorizedToken
	{
	  get { return _WasCheckedCategorizedToken; }
	  set { _WasCheckedCategorizedToken = value; }
	}
	public CategorizedToken() :  this(-1, -1, -1, -1, 6, null)
	{
	}
	public CategorizedToken(TokenLanguage language) : this()
	{
	  _Language = language;
	}
	public CategorizedToken(int startPosition, int endPosition, int line, int column, int endLine, int endColumn, int tokenType, string value)
	  : base(startPosition, endPosition, line, column, endLine, endColumn, tokenType, value)
	{
	}
	public CategorizedToken(int startPosition, int endPosition, int line, int column, int endLine, int endColumn, int tokenType, string value, TokenCategory category)
	  : base(startPosition, endPosition, line, column, endLine, endColumn, tokenType, value)
	{
	  _Category = category;
	}
	public CategorizedToken(int line, int column, int endLine, int endColumn, int tokenType, string value)
			: base(-1, -1, line, column, endLine, endColumn, tokenType, value)
		{
		}
		protected override void CloneDataFrom(Token source, ElementCloneOptions options)
		{
	  if (source == null)
		return;
			base.CloneDataFrom(source, options);
	  CategorizedToken categorizedToken = source as CategorizedToken;
	  if (categorizedToken == null)
		return;
	  _Category = categorizedToken._Category;
	  _Language = categorizedToken._Language;
		}
	public string GetTokenCategory()
	{
	  switch (_Category)
	  {
		case TokenCategory.Keyword:
		  return "Keyword";
		case TokenCategory.Operator:
		  return "Operator";
		case TokenCategory.PreprocessorKeyword:
		  return "Preprocessor Keyword";
		case TokenCategory.String:
		  return "String";
		case TokenCategory.Number:
		  return "Number";
		case TokenCategory.Comment:
		  return "Comment";
		case TokenCategory.XmlComment:
		  return "XML Doc Tag";
		case TokenCategory.Identifier:
		  return "Identifier";
		case TokenCategory.CssComment:
		  return "CSS Comment";
		case TokenCategory.CssKeyword:
		  return "CSS Keyword";
		case TokenCategory.CssPropertyName:
		  return "CSS Property Name";
		case TokenCategory.CssPropertyValue:
		  return "CSS Property Value";
		case TokenCategory.CssSelector:
		  return "CSS Selector";
		case TokenCategory.CssStringValue:
		  return "CSS String Value";
		case TokenCategory.HtmlServerSideScript:
		  return "HTML Server-Side Script";
		case TokenCategory.HtmlElementName:
		  return "HTML Element Name";
		case TokenCategory.HtmlEntity:
		  return "HTML Entity";
		case TokenCategory.HtmlOperator:
		  return "HTML Operator";
		case TokenCategory.HtmlComment:
		  return "HTML Comment";
		case TokenCategory.HtmlAttributeName:
		  return "HTML Attribute Name";
		case TokenCategory.HtmlAttributeValue:
		  return "HTML Attribute Value";
		case TokenCategory.HtmlString:
		  return "HTML String";
		case TokenCategory.HtmlTagDelimiter:
		  return "HTML Tag Delimiter";
	  }
	  return "Text";
	}
	public override Token Clone(ElementCloneOptions options)
	{
	  CategorizedToken token = new CategorizedToken();
	  token.CloneDataFrom(this, options);
	  return token;
	}
	public TokenCategory Category
	{
	  get { return _Category; }
	  set { _Category = value; }
	}
	public TokenLanguage Language
	{
	  get { return _Language; }
	  set { _Language = value; }
	}
	}
}
