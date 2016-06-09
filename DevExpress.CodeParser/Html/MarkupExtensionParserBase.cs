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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Xaml
#else
namespace DevExpress.CodeParser.Xaml
#endif
{
  public partial class MarkupExtensionParser : TokenCategorizedParserBase
  {
	public MarkupExtensionParser()
	{
	  parserErrors = new MarkupExtensionParserErrors();
	  set = CreateSetArray();
	  maxTokens = Tokens.MaxTokens;
	}
	Expression ParseExpression(ISourceReader reader)
	{
	  try
	  {
		scanner = new MarkupExtensionScanner(reader);
		la = new Token();
		la.Value = "";
		Get();
		MarkupExtensionExpression result = null;
		MarkupExtensionExpr(out result);
		return result;
	  }
	  finally
	  {
		CleanUpParser();
		if (reader != null)
		{
		  reader.Dispose();
		  reader = null;
		}
	  }
	}
	bool IsInitializer()
	{
	  if (la.Type != Tokens.IDENTIFIER)
		return false;
	  ResetPeek();
	  int nextType = Peek().Type;
	  bool lastTokenWasIdent = nextType == Tokens.IDENTIFIER;
	  while (nextType != Tokens.EQUALSTOKEN && nextType != Tokens.EOF)
	  {
		switch (nextType)
		{
		  case Tokens.IDENTIFIER:
			if (lastTokenWasIdent)
			  return false;
			lastTokenWasIdent = true;
			break;
		  case Tokens.DOT:
		  case Tokens.COLON:
			if (!lastTokenWasIdent)
			  return false;
			lastTokenWasIdent = false;
			break;
		  default:
			return false;
		}
		nextType = Peek().Type;
	  }
	  return true;
	}
	protected override TokenCategory GetTokenCategory(CategorizedToken token)
	{
	  return TokenCategory.Identifier;
	}
	public override ExpressionParserBase CreateExpressionParser()
	{
	  return new MarkupExtensionExpressionParser(this);
	}
	public override string Language
	{
	  get { return "MarkupExtension"; }
	}
	protected override void Get()
	{
	  base.Get();
	  if (la.Type == Tokens.MaxTokens)
	  {
		la.Type = Tokens.STRINGLITERAL;
		return;
	  }
	  if (tToken.Type == Tokens.EQUALSTOKEN && la.Type == Tokens.DOT)
	  {
		la.Type = Tokens.STRINGLITERAL;
		return;
	  }
	}
	class MarkupExtensionExpressionParser : ExpressionParserBase
	{
	  public MarkupExtensionExpressionParser(ParserBase parser) : base(parser)
	  {
	  }
	  public override Expression Parse(ISourceReader reader)
	  {
		MarkupExtensionParser parser = new MarkupExtensionParser();
		return parser.ParseExpression(reader);
	  }
	}
  }
}
