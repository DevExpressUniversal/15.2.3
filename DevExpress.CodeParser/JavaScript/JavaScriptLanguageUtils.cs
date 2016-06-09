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
using System.Collections.Generic;
using System.IO;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
  sealed class JavaScriptLanguageUtils : LanguageUtils
  {
	public override Tokenizer CreateTokenizer()
	{
	  return new JavaScriptTokenizer();
	}
	public override ParserBase CreateParser(ParserVersion version, EmbededLanguageKind languageKind, DotNetLanguageType embededLanguage)
	{
	  return new JavaScriptParser();
	}
	public override CodeGen CreateCodeGen()
	{
	  CodeGen codeGen = new JavaScriptCodegen();
	  if (codeGen != null)
		codeGen.Code = new CodeWriter(new StringWriter(), new CodeGenOptions(ParserLanguageID.JavaScript));
	  return codeGen;
	}
	public override ElementBuilder CreateElementBuilder()
	{
	  return new ElementBuilder();
	}
	class JavaScriptTokenizer : Tokenizer
	{
	  private TokenCategory GetTokenCategoryInternal(Token token)
	  {
		if (token == null)
		  return TokenCategory.Text;
		int tokenType = token.Type;
		switch (tokenType)
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
			return TokenCategory.Keyword;
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
			return TokenCategory.Operator;
		  case Tokens.INTEGERLITERAL:
		  case Tokens.DECIMALLITERAL:
			return TokenCategory.Number;
		  case Tokens.IDENTIFIER:
			return TokenCategory.Identifier;
		  case Tokens.STRINGLITERAL:
			return TokenCategory.String;
		  case Tokens.MULTILINECOMMENT:
		  case Tokens.SINGLELINECOMMENT:
			return TokenCategory.Comment;
		  case Tokens.EOF:
			return TokenCategory.Unknown;
		}
		return TokenCategory.Text;
	  }
	  public override TokenCategory GetUncategorizedTokenCategory(Token token)
	  {
		return GetTokenCategoryInternal(token);
	  }
	  public override IList<CategorizedToken> GetTokens(string code, EmbededLanguageKind languageKind, DotNetLanguageType codeEmbeddingDefaultLanguage)
	  {
		List<CategorizedToken> result = new List<CategorizedToken>();
		if (string.IsNullOrEmpty(code))
		  return result.AsReadOnly();
		SourceStringReader reader = new SourceStringReader(code, 1, 1);
		JavaScriptScanner scanner = new JavaScriptScanner(reader);
		CategorizedToken currentToken = scanner.Scan() as CategorizedToken;
		while (currentToken != null && currentToken.Type != Tokens.EOF)
		{
		  currentToken.Category = GetTokenCategoryInternal(currentToken);
		  result.Add(currentToken);
		  currentToken = scanner.Scan() as CategorizedToken;
		}
		return result.AsReadOnly();
	  }
	}
  }
}
