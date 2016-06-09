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
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
  public class JavaScriptExpressionInverter : ExpressionInverterBase
  {
	protected override Expression InvertIsExpression(Is expression)
	{
	  return DefaultInvert(expression);
	}
	protected override Expression InvertIsNotExpression(IsNot expression)
	{
	  return DefaultInvert(expression);
	}
  }
  public class JavaScriptTokensCategoryHelper : ITokenCategoryHelper
  {
	public TokenCategory GetUncategorizedTokenCategory(Token token)
	{
	  return JavaScriptTokensHelper.GetUncategorizedTokenCategory(token);
	}
	public TokenCollection GetTokens(string code)
	{
	  return JavaScriptTokensHelper.GetTokens(code);
	}
	public string GetTokenCategory(Token token)
	{
	  return JavaScriptTokensHelper.GetTokenCategory(token);
	}
	public string GetTokenCategory(Token token, ParserVersion version)
	{
	  return JavaScriptTokensHelper.GetTokenCategory(token);
	}
  }
  public class JavaScriptTokensHelper
  {
	#region JavaScriptTokensHelper
	public JavaScriptTokensHelper()
	{
	}
	#endregion
	#region GetFirstToken
	public static Token GetFirstToken(string number)
	{
	  TokenCollection tokenCollection = GetTokens(number);
	  if (tokenCollection == null || tokenCollection.Count == 0)
		return null;
	  return tokenCollection[0];
	}
	#endregion
	#region IsIdentifier
	public static Boolean IsIdentifier(int type)
	{
	  return type == Tokens.IDENTIFIER;
	}
	#endregion
	#region IsString
	public static bool IsString(int type)
	{
	  return type == Tokens.STRINGLITERAL;
	}
	#endregion
	#region IsComment
	public static Boolean IsComment(int type)
	{
	  switch (type)
	  {
		case Tokens.SINGLELINECOMMENT:
		case Tokens.MULTILINECOMMENT:
		  return true;
		default:
		  return false;
	  }
	}
	#endregion
	#region IsKeyword
	public static Boolean IsKeyword(string keyword)
	{
	  Token token = GetFirstToken(keyword);
	  if (token == null)
		return false;
	  if (keyword == "debugger")
		return true;
	  return IsKeyword(token.Type);
	}
	#endregion
	#region IsKeyword
	public static Boolean IsKeyword(int type)
	{
	  switch (type)
	  {
		case Tokens.BREAK:
		case Tokens.CASE:
		case Tokens.CATCH:
		case Tokens.CONTINUE:
		case Tokens.DEFAULT:
		case Tokens.DELETE:
		case Tokens.DO:
		case Tokens.ELSE:
		case Tokens.FINALLY:
		case Tokens.FOR:
		case Tokens.FUNCTION:
		case Tokens.IFKEYWORD:
		case Tokens.IN:
		case Tokens.INSTANCEOF:
		case Tokens.NEW:
		case Tokens.RETURN:
		case Tokens.SWITCH:
		case Tokens.THIS:
		case Tokens.THROW:
		case Tokens.TRY:
		case Tokens.TYPEOF:
		case Tokens.VAR:
		case Tokens.VOID:
		case Tokens.WHILE:
		case Tokens.WITH:
		case Tokens.TRUE:
		case Tokens.FALSE:
		case Tokens.NULL:
		  return true;
		default:
		  return false;
	  }
	}
	#endregion
	#region IsNumber
	public static Boolean IsNumber(string number)
	{
	  Token token = GetFirstToken(number);
	  if (token == null)
		return false;
	  return IsNumber(token.Type);
	}
	#endregion
	#region IsNumber
	public static Boolean IsNumber(int type)
	{
	  switch (type)
	  {
		case Tokens.INTEGERLITERAL:
		case Tokens.DECIMALLITERAL:
		  return true;
		default:
		  return false;
	  }
	}
	#endregion
	#region IsOperator
	public static Boolean IsOperator(int type)
	{
	  switch (type)
	  {
		case Tokens.COMMA:
		case Tokens.LESSTHAN:
		case Tokens.GREATERTHAN:
		case Tokens.LESSOREQUAL:
		case Tokens.GREATEROREQUAL:
		case Tokens.DOUBLEEQUALS:
		case Tokens.NOTEQUALS:
		case Tokens.TRIPLEEQUALS:
		case Tokens.NOTDOUBLEEQUALS:
		case Tokens.PLUS:
		case Tokens.MINUS:
		case Tokens.ASTERISK:
		case Tokens.PERCENTSYMBOL:
		case Tokens.PLUSPLUS:
		case Tokens.MINUSMINUS:
		case Tokens.SHIFTLEFT:
		case Tokens.SHIFTRIGHT:
		case Tokens.TRIPLESHIFTRIGHT:
		case Tokens.BITAND:
		case Tokens.BITOR:
		case Tokens.XORSYMBOL:
		case Tokens.EXCLAMATIONSYMBOL:
		case Tokens.TILDE:
		case Tokens.ANDAND:
		case Tokens.OROR:
		case Tokens.QUESTIONSYMBOL:
		case Tokens.COLON:
		case Tokens.EQUALSSYMBOL:
		case Tokens.PLUSEQUAL:
		case Tokens.MINUSEQUAL:
		case Tokens.MULEQUAL:
		case Tokens.MODEQUAL:
		case Tokens.SHIFTLEFTEQUAL:
		case Tokens.SHIFTRIGHTEQUAL:
		case Tokens.TRIPLESHIFTRIGHTEQUAL:
		case Tokens.ANDEQUAL:
		case Tokens.OREQUAL:
		case Tokens.XOREQUAL:
		case Tokens.SLASH:
		case Tokens.DIVEQUAL:
		  return true;
		default:
		  return false;
	  }
	}
	#endregion
	#region GetTokenCategory
	public static string GetTokenCategory(Token token)
	{
	  CategorizedToken categorizedToken = token as CategorizedToken;
	  if (categorizedToken != null)
		return categorizedToken.GetTokenCategory();
	  if (token == null)
		return "Text";
	  int tokenType = token.Type;
	  if (IsKeyword(tokenType))
		return "Keyword";
	  if (IsOperator(tokenType))
		return "Operator";
	  if (IsNumber(tokenType))
		return "Number";
	  if (IsIdentifier(tokenType))
		return "Identifier";
	  if (IsString(tokenType))
		return "String";
	  if (IsComment(tokenType))
		return "Comment";
	  return "Text";
	}
	#endregion
	#region GetTokens
	public static TokenCollection GetTokens(string text)
	{
	  TokenCollection result = new TokenCollection();
	  if (text == null || text.Length == 0)
		return result;
	  SourceStringReader reader = new SourceStringReader(text, 1, 1);
	  JavaScriptScanner scanner = new JavaScriptScanner(reader);
	  CategorizedToken currentToken = scanner.Scan() as CategorizedToken;
	  while (currentToken != null && currentToken.Type != Tokens.EOF)
	  {
		currentToken.Category = GetUncategorizedTokenCategory(currentToken);
		result.Add(currentToken);
		currentToken = scanner.Scan() as CategorizedToken;
	  }
	  return result;
	}
	#endregion
	#region GetUncategorizedTokenCategory
	public static TokenCategory GetUncategorizedTokenCategory(Token token)
	{
	  if (token == null)
		return TokenCategory.Text;
	  int tokenType = token.Type;
	  if (IsKeyword(tokenType))
		return TokenCategory.Keyword;
	  if (IsOperator(tokenType))
		return TokenCategory.Operator;
	  if (IsNumber(tokenType))
		return TokenCategory.Number;
	  if (IsIdentifier(tokenType))
		return TokenCategory.Identifier;
	  if (IsString(tokenType))
		return TokenCategory.String;
	  if (IsComment(tokenType))
		return TokenCategory.Comment;
	  if (token.Type == Tokens.EOF)
		return TokenCategory.Unknown;
	  return TokenCategory.Text;
	}
	#endregion
  }
  public class JavaScriptTokens : LanguageTokensBase
  {
	protected override System.Collections.Specialized.StringCollection CreateKeywords()
	{
	  return null;
	}
	public override bool IsComment(int type)
	{
	  return JavaScriptTokensHelper.IsComment(type);
	}
	public override bool IsDirective(int type)
	{
	  return false;
	}
	public override bool IsIdentifier(string word)
	{
	  Token token = JavaScriptTokensHelper.GetFirstToken(word);
	  return token != null && JavaScriptTokensHelper.IsIdentifier(token.Type);
	}
	public override bool IsKeyword(string word)
	{
	  Token token = JavaScriptTokensHelper.GetFirstToken(word);
	  return token != null && JavaScriptTokensHelper.IsKeyword(token.Type);
	}
	public override bool IsNumber(int type)
	{
	  return JavaScriptTokensHelper.IsKeyword(type);
	}
	public override bool IsStandardType(string word)
	{
	  return false;
	}
	public override bool IsStringLiteral(int type)
	{
	  return type == Tokens.STRINGLITERAL;
	}
	public override bool IsXmlDocComment(int type)
	{
	  return false;
	}
	public override int ToTokenType(string token)
	{
	  Token tokens = JavaScriptTokensHelper.GetFirstToken(token);
	  if (token != null)
		return tokens.Type;
	  else
		return -1;
	}
  }
}
