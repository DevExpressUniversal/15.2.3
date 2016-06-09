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
using System.Globalization;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Css
#else
namespace DevExpress.CodeParser.Css
#endif
{
  using Xml;
  using Diagnostics;
  public partial class CssParser : XmlLanguageParserBase
  {
	CommentCollection _Comments;
	static bool[,] _CssSet;
	public CssParser()
	{
	  parserErrors = new CssParserErrors();
	  if (_CssSet == null)
		_CssSet = CreateSetArray();
	  set = _CssSet;
	  maxTokens = Tokens.MaxTokens;
	  _Comments = new CommentCollection();
	}
	CssPseudoClassType GetPseudoClassType(string name)
	{
	  if (string.IsNullOrEmpty(name))
		return CssPseudoClassType.Unknown;
	  switch (name.ToLower())
	  {
		case "not":
		  return CssPseudoClassType.Negation;
		case "matches":
		  return CssPseudoClassType.MatchesAny;
		case "dir":
		  return CssPseudoClassType.Dir;
		case "lang":
		  return CssPseudoClassType.Lang;
		case "any-link":
		  return CssPseudoClassType.Hyperlink;
		case "link":
		  return CssPseudoClassType.NotVisitedLink;
		case "visited":
		  return CssPseudoClassType.VisitedLink;
		case "local-link":
		  return CssPseudoClassType.LocalLink;
		case "target":
		  return CssPseudoClassType.Target;
		case "scope":
		  return CssPseudoClassType.Scope;
		case "current":
		  return CssPseudoClassType.Current;
		case "past":
		  return CssPseudoClassType.Past;
		case "future":
		  return CssPseudoClassType.Future;
		case "active":
		  return CssPseudoClassType.Active;
		case "hover":
		  return CssPseudoClassType.Hover;
		case "focus":
		  return CssPseudoClassType.Focus;
		case "enabled":
		  return CssPseudoClassType.Enabled;
		case "disabled":
		  return CssPseudoClassType.Disabled;
		case "checked":
		  return CssPseudoClassType.SelectedOption;
		case "indeterminate":
		  return CssPseudoClassType.IndeterminateValue;
		case "default":
		  return CssPseudoClassType.DefaultOption;
		case "in-range":
		  return CssPseudoClassType.InRange;
		case "out-of-range":
		  return CssPseudoClassType.OutOfRange;
		case "required":
		  return CssPseudoClassType.Required;
		case "optional":
		  return CssPseudoClassType.Optional;
		case "read-only":
		  return CssPseudoClassType.ReadOnly;
		case "read-write":
		  return CssPseudoClassType.ReadWrite;
		case "root":
		  return CssPseudoClassType.Root;
		case "empty":
		  return CssPseudoClassType.Empty;
		case "first-child":
		  return CssPseudoClassType.FirstChild;
		case "nth-child":
		  return CssPseudoClassType.NthChild;
		case "last-child":
		  return CssPseudoClassType.LastChild;
		case "nth-last-child":
		  return CssPseudoClassType.NthLastChild;
		case "only-child":
		  return CssPseudoClassType.OnlyChild;
		case "first-of-type":
		  return CssPseudoClassType.FirstOfType;
		case "nth-of-type":
		  return CssPseudoClassType.NthOfType;
		case "last-of-type":
		  return CssPseudoClassType.LastOfType;
		case "nth-last-of-type":
		  return CssPseudoClassType.NthLastOfType;
		case "only-of-type":
		  return CssPseudoClassType.OnlyOfType;
		case "nth-match":
		  return CssPseudoClassType.NthMatch;
		case "column":
		  return CssPseudoClassType.Column;
		case "nth-column":
		  return CssPseudoClassType.NthColumn;
		case "nth-last-column":
		  return CssPseudoClassType.NthLastColumn;
		default:
		  return CssPseudoClassType.Unknown;
	  }
	}
	object GetValueFromString(string strValue, int suffixPos)
	{
	  Decimal result = Decimal.Zero;
	  try
	  {
		string rawValue = strValue.Substring(0, suffixPos);
		result = Convert.ToDecimal(rawValue, NumberFormatInfo.InvariantInfo);
	  }
	  catch
	  {
		return null;
	  }
	  return result;
	}
	void ConnectSelectors(CssSelector parent, CssSelector child)
	{
	  if (child == null)
		return;
	  if (parent != null)
	  {
		child.SetParent(parent);
		parent.ChildSelector = child;
		parent.AddNode(child);
		parent.SetRange(GetRange(parent, child));
	  }
	  CssSelector currentParent = child.Parent as CssSelector;
	  while (currentParent != null)
	  {
		currentParent.SetRange(GetRange(currentParent, child));
		currentParent = currentParent.Parent as CssSelector;
	  }
	}
	void GetMeasuredLiteralValue(string strValue, out Object decValue, out CssMeasureUnit unit)
	{
	  decValue = null;
	  unit = CssMeasureUnit.None;
	  if (strValue == null ||
		  strValue.Length < 2)
		return;
	  int suffixPos = -1;
	  if ((suffixPos = strValue.IndexOf("grad")) != -1)
	  {
		unit = CssMeasureUnit.Grad;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("rad")) != -1)
	  {
		unit = CssMeasureUnit.Rad;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("deg")) != -1)
	  {
		unit = CssMeasureUnit.Deg;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("khz")) != -1)
	  {
		unit = CssMeasureUnit.kHz;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("hz")) != -1)
	  {
		unit = CssMeasureUnit.Hz;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("px")) != -1)
	  {
		unit = CssMeasureUnit.Px;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("cm")) != -1)
	  {
		unit = CssMeasureUnit.Cm;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("mm")) != -1)
	  {
		unit = CssMeasureUnit.Mm;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("in")) != -1)
	  {
		unit = CssMeasureUnit.In;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("pt")) != -1)
	  {
		unit = CssMeasureUnit.Pt;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("pc")) != -1)
	  {
		unit = CssMeasureUnit.Pc;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("em")) != -1)
	  {
		unit = CssMeasureUnit.Em;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("ex")) != -1)
	  {
		unit = CssMeasureUnit.Ex;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("ms")) != -1)
	  {
		unit = CssMeasureUnit.Ms;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	  if ((suffixPos = strValue.IndexOf("s")) != -1)
	  {
		unit = CssMeasureUnit.S;
		decValue = GetValueFromString(strValue, suffixPos);
		return;
	  }
	}
	bool IsSpecialNameWithoutAt(string name)
	{
	  return la.Type == Tokens.IDENT &&
			 la.Value == name;
	}
	bool IsSpecialName(string name)
	{
	  if (la.Type != Tokens.AT)
		return false;
	  ResetPeek();
	  Token peek = Peek();
	  return peek.Type == Tokens.IDENT &&
			 peek.Value == name;
	}
	void AddCommentNode(Token commentToken)
	{
	  Comment comment = new Comment();
	  CommentType type = (commentToken.Type == Tokens.SINGLELINECOMMENT) ? CommentType.SingleLine : CommentType.MultiLine;
	  string value = commentToken.Value;
	  bool hasOpenMultiLineTag = value.StartsWith("/*");
	  const int startIdx = 2;
	  comment.SetCommentType(type);
	  comment.SetTextStartOffset(startIdx);
	  if (value.Length > startIdx)
		value = value.Substring(startIdx);
	  else
		value = string.Empty;
	  int multiLineCommentClosePos = value.LastIndexOf("*/");
	  bool hasCloseMultiLineTag = multiLineCommentClosePos >= 0;
	  if (type == CommentType.MultiLine &&
		  hasCloseMultiLineTag)
		value = value.Substring(0, multiLineCommentClosePos);
	  if (hasOpenMultiLineTag &&
		  !hasCloseMultiLineTag)
		comment.IsUnfinished = true;
	  comment.SetRange(commentToken.Range);
	  comment.StartPos = commentToken.StartPosition;
	  comment.EndPos = commentToken.EndPosition;
	  comment.InternalName = GlobalStringStorage.Intern(value);
	  Comments.Add(comment);
	}
	void SetCategoryIfNeeded(TokenCategory category)
	{
	  if (!SetTokensCategory)
		return;
	  CategorizedToken categorizedToken = tToken as CategorizedToken;
	  if (categorizedToken != null)
		categorizedToken.Category = category;
	}
	protected override void Get()
	{
	  base.Get();
	  Token oldT = tToken;
	  while (la.Match(Tokens.SINGLELINECOMMENT) ||
			 la.Match(Tokens.MULTILINECOMMENT))
	  {
		oldT = tToken;
		AddCommentNode(la);
		base.Get();
	  }
	  tToken = oldT;
	}
	protected override TokenCategory GetTokenCategory(CategorizedToken token)
	{
	  if (token.Type == 0)
		return TokenCategory.Unknown;
	  switch (token.Type)
	  {
		case Tokens.SEMICOLON:
		case Tokens.COMMA:
		case Tokens.CURLYOPEN:
		case Tokens.CURLYCLOSE:
		case Tokens.COLON:
		case Tokens.SLASH:
		case Tokens.PLUS:
		case Tokens.GREATER:
		case Tokens.MINUS:
		case Tokens.DOT:
		case Tokens.STAR:
		case Tokens.BRACKETOPEN:
		case Tokens.BRACKETCLOSE:
		case Tokens.EQUALSIGN:
		case Tokens.INCLUDES:
		case Tokens.DASHMATCH:
		case Tokens.PARENCLOSE:
		  return TokenCategory.Text;
		case Tokens.STRING:
		  return TokenCategory.CssStringValue;
		case Tokens.NUMBER:
		case Tokens.PERCENTAGE:
		case Tokens.LENGTH:
		case Tokens.EMS:
		case Tokens.EXS:
		case Tokens.ANGLE:
		case Tokens.TIME:
		case Tokens.FREQ:
		case Tokens.IDENT:
		case Tokens.HASH:
		case Tokens.URI:
		  return TokenCategory.CssPropertyValue;
		case Tokens.SINGLELINECOMMENT:
		case Tokens.MULTILINECOMMENT:
		  return TokenCategory.CssComment;
		case Tokens.AT:
		case Tokens.IMPORTANTSYM:
		  return TokenCategory.CssKeyword;
		default:
		  return TokenCategory.Text;
	  }
	}
	protected override LanguageElement DoParse(ParserContext parserContext, ISourceReader reader)
	{
	  LanguageElement context = parserContext.Context;
	  if (context == null)
		return null;
	  SetRootNode(context);
	  if (context is SourceFile)
		((SourceFile)context).SetDocument(parserContext.Document);
	  return Parse(reader);
	}
	public LanguageElement Parse(ISourceReader reader)
	{
	  try
	  {
		scanner = new CssScanner(reader);
		Parse();
		BindComments();
		return RootNode;
	  }
	  finally
	  {
		CleanUpParser();
	  }
	}
	public ArrayList ParseAttributeStyleDefinition(ISourceReader reader)
	{
	  try
	  {
		CssPropertyDeclaration propertyDecl = null;
		ArrayList result = new ArrayList();
		scanner = new CssScanner(reader);
		la = new Token();
		la.Value = "";
		Get();
		while (la.Type == Tokens.IDENT)
		{
		  Declaration(out propertyDecl);
		  if (la.Type == Tokens.SEMICOLON)
			Get();
		  if (propertyDecl == null)
			continue;
		  if (tToken.Type == Tokens.SEMICOLON)
			propertyDecl.SetRange(GetRange(propertyDecl, tToken));
		  result.Add(propertyDecl);
		}
		return result;
	  }
	  catch (Exception e)
	  {
		Log.SendException(e);
		return null;
	  }
	  finally
	  {
		CleanUpParser();
	  }
	}
	protected override void CleanUpParser()
	{
	  base.CleanUpParser();
	  _Comments.Clear();
	}
	public override CommentCollection Comments { get { return _Comments; } }
  }
}
