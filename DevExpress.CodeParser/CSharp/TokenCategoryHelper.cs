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
  public class CSharpTokensCategoryHelper : ITokenCategoryHelper
  {
	public TokenCategory GetUncategorizedTokenCategory(Token token)
	{
	  return CSharpTokensHelper.GetUncategorizedTokenCategory(token);
	}
	public TokenCollection GetTokens(string code)
	{
	  return CSharpTokensHelper.GetTokens(code);
	}
	public string GetTokenCategory(Token token)
	{
	  return CSharpTokensHelper.GetTokenCategory(token);
	}
	public string GetTokenCategory(Token token, ParserVersion version)
	{
	  return CSharpTokensHelper.GetTokenCategory(token, version);
	}
  }
	public class CSharpTokensHelper
	{
		#region GetUncategorizedTokenCategory
		public static TokenCategory GetUncategorizedTokenCategory(Token token)
		{
			if (token == null)
				return TokenCategory.Text;
			switch (token.Type)
			{
				case Tokens.ABSTRACT:
				case Tokens.AS:
				case Tokens.BASE:
				case Tokens.BOOL:
				case Tokens.BREAK:
				case Tokens.BYTE:
				case Tokens.CASE:
				case Tokens.CATCH:
				case Tokens.CHAR:
				case Tokens.CHECKED:
				case Tokens.CLASS:
				case Tokens.CONST:
				case Tokens.CONTINUE:
				case Tokens.DECIMAL:
				case Tokens.DEFAULT:
				case Tokens.DELEGATE:
				case Tokens.DO:
				case Tokens.DOUBLE:
				case Tokens.ELSE:
				case Tokens.ENUM:
				case Tokens.EVENT:
				case Tokens.EXPLICIT:
				case Tokens.EXTERN:
				case Tokens.FALSE:
				case Tokens.FINALLY:
				case Tokens.FIXED:
				case Tokens.FLOAT:
				case Tokens.FOR:
				case Tokens.FOREACH:
				case Tokens.GOTO:
				case Tokens.IFCLAUSE:
				case Tokens.IMPLICIT:
				case Tokens.IN:
				case Tokens.INT:
				case Tokens.INTERFACE:
				case Tokens.INTERNAL:
				case Tokens.IS:
				case Tokens.LOCK:
				case Tokens.LONG:
				case Tokens.NAMESPACE:
				case Tokens.NEW:
				case Tokens.NULL:
				case Tokens.OPERATOR:
				case Tokens.OUT:
				case Tokens.OVERRIDE:
				case Tokens.PARAMS:
				case Tokens.PRIVATE:
				case Tokens.PROTECTED:
				case Tokens.PUBLIC:
				case Tokens.READONLY:
				case Tokens.REF:
				case Tokens.RETURN:
				case Tokens.SBYTE:
				case Tokens.SEALED:
				case Tokens.SHORT:
				case Tokens.SIZEOF:
				case Tokens.STACKALLOC:
				case Tokens.STATIC:
				case Tokens.STRUCT:
				case Tokens.SWITCH:
				case Tokens.THIS:
				case Tokens.THROW:
				case Tokens.TRUE:
				case Tokens.TRY:
				case Tokens.TYPEOF:
				case Tokens.UINT:
				case Tokens.ULONG:
				case Tokens.UNCHECKED:
				case Tokens.UNSAFE:
				case Tokens.USHORT:
				case Tokens.USINGKW:
				case Tokens.VIRTUAL:
				case Tokens.VOID:
				case Tokens.VOLATILE:
				case Tokens.WHILE:
					return TokenCategory.Keyword;
				case Tokens.AND:
				case Tokens.ANDASSGN:
				case Tokens.ASSGN:
				case Tokens.COLON:
				case Tokens.COMMA:
				case Tokens.DEC:
				case Tokens.DIVASSGN:
				case Tokens.DOT:
				case Tokens.DBLCOLON:
				case Tokens.EQ:
				case Tokens.GT:
				case Tokens.GTEQ:
				case Tokens.INC:
				case Tokens.LSHASSGN:
				case Tokens.LT:
				case Tokens.LTLT:
				case Tokens.MINUS:
				case Tokens.MINUSASSGN:
				case Tokens.MODASSGN:
				case Tokens.NEQ:
				case Tokens.NOT:
				case Tokens.ORASSGN:
				case Tokens.PLUS:
				case Tokens.PLUSASSGN:
				case Tokens.QUESTION:
				case Tokens.TILDE:
				case Tokens.TIMES:
				case Tokens.TIMESASSGN:
				case Tokens.XORASSGN:
				case Tokens.POINTERTOMEMBER:
				case Tokens.DBLOR:
				case Tokens.DBLAND:
				case Tokens.OR:
				case Tokens.XOR:
				case Tokens.LOWOREQ:
				case Tokens.DIV:
				case Tokens.MOD:
				case Tokens.POINT:
					return TokenCategory.Operator;
				case Tokens.DEFINE:
				case Tokens.UNDEF:
				case Tokens.IFDIR:
				case Tokens.ELIF:
				case Tokens.ELSEDIR:
				case Tokens.ENDIF:
				case Tokens.LINE:
				case Tokens.ERROR:
				case Tokens.WARNING:
				case Tokens.REGION:
				case Tokens.ENDREG:
					return TokenCategory.PreprocessorKeyword;
				case Tokens.CHARCON:
				case Tokens.STRINGCON:
					return TokenCategory.String;
				case Tokens.INTCON:
				case Tokens.REALCON:
					return TokenCategory.Number;
				case Tokens.SINGLELINECOMMENT:
				case Tokens.MULTILINECOMMENT:
					return TokenCategory.Comment;
				case Tokens.MULTILINEXML:
				case Tokens.SINGLELINEXML:
					return TokenCategory.XmlComment;
				case Tokens.IDENT:
					if (token.Value == "string" || token.Value == "object")
						return TokenCategory.Keyword;
					else
						return TokenCategory.Identifier;
		case Tokens.LINETERMINATOR:
		case Tokens.EOF:
		  return TokenCategory.Unknown;
				default:
					return TokenCategory.Text;
			}
		}
		#endregion
	#region GetTokens
	public static TokenCollection GetTokens(string code)
		{
	  if (string.IsNullOrEmpty(code))
		return null;
	  ISourceReader reader = new SourceStringReader(code, 1, 1);
	  CSharp30Parser parser = new CSharp30Parser();
	  parser.SetTokensCategory = true;
	  parser.Parse(reader);
	  return parser.SavedTokens;
	}
	#endregion
	public static string GetTokenCategory(Token token, ParserVersion version)
	{
	  if (token == null)
		return "Text";
	  string category = GetTokenCategory(token);
	  if (version >= ParserVersion.VS2010)
		if (String.Compare(token.Value, "dynamic", StringComparison.CurrentCulture) == 0)
		  return "Keyword";
	  if (version >= ParserVersion.VS2008)
		if (String.Compare(token.Value, "var", StringComparison.CurrentCulture) == 0)
		  return "Keyword";
	  return category;
	}
	#region GetTokenCategory
	public static string GetTokenCategory(Token token)
		{
			if (token == null)
				return "Text";
			if (token is CategorizedToken)
			{
				switch ((token as CategorizedToken).Category)
				{
					case TokenCategory.Keyword:
						return "Keyword";
					case TokenCategory.Identifier:
						return "Identifier";
					case TokenCategory.Number:
						return "Number";
					case TokenCategory.Operator:
						return "Operator";
					case TokenCategory.PreprocessorKeyword:
						return "Preprocessor Keyword";
					case TokenCategory.String:
						return "String";
					case TokenCategory.Comment:
						return "Comment";
					case TokenCategory.XmlComment:
			string xmlTagName = StructuralParserServicesHolder.GetXMLTagName();
			if (xmlTagName != null)
			  return xmlTagName;
			return "XML Doc Tag";
					default:
						return "Text";
				}
			}
			else
				return "Text";
	}
	#endregion
  }
}
