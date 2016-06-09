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
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Xaml
#else
namespace DevExpress.CodeParser.Xaml
#endif
{
  using Xml;
  public class Tokens
	{
		public const int EOF = 0;
		public const int EQUALSTOKEN = 4;
		public const int STRINGLITERAL = 10;
		public const int CLOSEBRACE = 6;
		public const int CLOSEBRACKET = 9;
		public const int OPENBRACE = 5;
		public const int PATH = 12;
		public const int OPENBRACKET = 8;
		public const int COMMA = 2;
		public const int COLON = 3;
		public const int IDENTIFIER = 7;
		public const int INTEGERLITERAL = 11;
		public const int DOT = 1;
		public const int MaxTokens = 13;
		public static int[] Keywords = {
		};
	}
  partial class MarkupExtensionParser
  {
	protected override void HandlePragmas()
	{
	}
		void Parser()
	{
		MarkupExtensionExpression result = null;
		MarkupExtensionExpr(out result);
		SetContext(result);
	}
	void MarkupExtensionExpr(out MarkupExtensionExpression result)
	{
		result = new MarkupExtensionExpression();
		result.SetRange(la.Range);
		Expression expression = null;
		Expression elRef;
		Expect(Tokens.OPENBRACE );
		if (la.Type == Tokens.IDENTIFIER )
		{
			Name(out elRef);
			result.Qualifier = elRef;
			result.Name = tToken.Value;
			result.NameRange = tToken.Range;
		}
		while (StartOf(1))
		{
			if (IsInitializer())
			{
				Initializer(out expression);
				result.AddInitializer(expression); 
			}
			else
			{
				Expression(out expression);
				result.AddArgument(expression); 
			}
			if (la.Type == Tokens.COMMA )
			{
				Get();
			}
		}
		Expect(Tokens.CLOSEBRACE );
		result.SetRange(GetRange(result, tToken.Range));
	}
	void PartialName(Expression source, out Expression result)
	{
		result = source;
		QualifiedElementReference qualifiedEleRef = null;
		if (la.Type == Tokens.DOT )
		{
			Get();
		}
		else if (la.Type == Tokens.COLON )
		{
			Get();
		}
		else
			SynErr(14);
		qualifiedEleRef = new QualifiedElementReference(la.Value);
		qualifiedEleRef.NameRange = la.Range;
		qualifiedEleRef.SetRange(GetRange(source.Range, la.Range));
		qualifiedEleRef.Qualifier = result;
		result = qualifiedEleRef;
		Expect(Tokens.IDENTIFIER );
		while (la.Type == Tokens.DOT  || la.Type == Tokens.COLON )
		{
			if (la.Type == Tokens.DOT )
			{
				Get();
			}
			else
			{
				Get();
			}
			qualifiedEleRef = new QualifiedElementReference(la.Value);
			qualifiedEleRef.NameRange = la.Range;
			qualifiedEleRef.SetRange(GetRange(source.Range, la.Range));
			qualifiedEleRef.Qualifier = result;
			result = qualifiedEleRef;
			Expect(Tokens.IDENTIFIER );
		}
	}
	void SimpleName(out Expression result)
	{
		result = null;
		SourceRange startRange = la.Range;
		string value = la.Value;
		Expect(Tokens.IDENTIFIER );
		if (la.Type == Tokens.PATH )
		{
			Get();
			PrimitiveExpression primitive = new PrimitiveExpression(value + tToken.Value, GetRange(startRange, tToken.Range));
			primitive.PrimitiveType = PrimitiveType.Path;
			result = primitive;
			return;
		}
		if (la.Value == @"/")
		{
		  while(la.Value == @"/" || la.Type == Tokens.DOT)
		  {
			value += la.Value;
			Get();
			if(la.Type == Tokens.IDENTIFIER)
			{
			  value += la.Value;
			  Get();
			}
		  }
		  PrimitiveExpression primitive = new PrimitiveExpression(value, GetRange(startRange, tToken.Range));
		  primitive.PrimitiveType = PrimitiveType.Path;
		  primitive.PrimitiveValue = value;
		  result = primitive;
		}
		else
		{
		if(la.Type == Tokens.COLON)
		  result = new QualifiedAliasExpression(tToken.Value);
		else
		  result = new ElementReferenceExpression(tToken.Value);
		result.NameRange = tToken.Range;
		result.SetRange(tToken.Range);
		}
	}
	void Name(out Expression result)
	{
		result = null;
		SimpleName(out result);
		if (la.Type == Tokens.DOT  || la.Type == Tokens.COLON )
		{
			PartialName(result, out result);
		}
	}
	void Initializer(out Expression expr)
	{
		AttributeVariableInitializer varInit = new AttributeVariableInitializer();
		varInit.SetRange(la.Range);
		expr = varInit;
		Expression init = null;
		Expression name = null;
		Name(out name);
		varInit.Name = la.Value; 
		Expect(Tokens.EQUALSTOKEN );
		Expression(out init);
		varInit.LeftSide = name;
		varInit.RightSide = init;
		varInit.SetRange(GetRange(varInit, tToken));
	}
	void Expression(out Expression expression)
	{
		expression = null;
		Expression name = null;
		MarkupExtensionExpression markupExtensionExpression = null;
		if (la.Type == Tokens.STRINGLITERAL  || la.Type == Tokens.INTEGERLITERAL  || la.Type == Tokens.PATH )
		{
			Literal(out expression);
		}
		else if (la.Type == Tokens.OPENBRACKET )
		{
			Indexer(null, out expression);
		}
		else if (la.Type == Tokens.IDENTIFIER )
		{
			Name(out name);
			expression = name; 
		}
		else if (la.Type == Tokens.OPENBRACE )
		{
			MarkupExtensionExpr(out markupExtensionExpression);
			expression = markupExtensionExpression; 
		}
		else
			SynErr(15);
		while (la.Type == Tokens.DOT  || la.Type == Tokens.COLON  || la.Type == Tokens.OPENBRACKET )
		{
			if (la.Type == Tokens.OPENBRACKET )
			{
				Indexer(expression, out expression);
			}
			else
			{
				PartialName(expression, out expression);
			}
		}
	}
	void Indexer(Expression source, out Expression expr)
	{
		IndexerExpression indexer = new IndexerExpression(source);
		expr = indexer;
		Expression expression = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.OPENBRACKET );
		Expression(out expression);
		indexer.AddArgument(expression); 
		while (la.Type == Tokens.COMMA )
		{
			Get();
			Expression(out expression);
			indexer.AddArgument(expression); 
		}
		Expect(Tokens.CLOSEBRACKET );
		if (source != null)
		 indexer.SetRange(GetRange(source.Range, tToken.Range));
		else
		  indexer.SetRange(GetRange(startRange, tToken.Range));
	}
	void Literal(out Expression result)
	{
		PrimitiveExpression primitive = new PrimitiveExpression(la.Value, la.Range);
		result = primitive;
		primitive.IsVerbatimStringLiteral = false;
		if (la.Type == Tokens.STRINGLITERAL )
		{
			Get();
			primitive.PrimitiveType = PrimitiveType.String;
			primitive.PrimitiveValue = tToken.Value.Trim('\'');
		}
		else if (la.Type == Tokens.INTEGERLITERAL )
		{
			Get();
			primitive.PrimitiveType = PrimitiveType.Int32;
			int intVal = 0;
			int.TryParse(la.Value, out intVal);
			primitive.PrimitiveValue = intVal;
		}
		else if (la.Type == Tokens.PATH )
		{
			Get();
			primitive.PrimitiveType = PrimitiveType.Path;
			int intVal = 0;
			int.TryParse(la.Value, out intVal);
			primitive.PrimitiveValue = tToken.Value.Trim();
		}
		else
			SynErr(16);
	}
	void Parse()
	{
	  la = new Token();
	  la.Value = "";
	  Get();
	  		Parser();
	  Expect(0);
	  if (Context != null)
		Context.SetRange(GetRange(Context, tToken));
	  CloseContext();
	}
	protected override bool[,] CreateSetArray()
	{
	  return new bool[,] { 		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,T,x,T, T,x,T,T, T,x,x}
 };
	}
  }
  public class MarkupExtensionParserErrors : ParserErrorsBase
  {
	protected override string GetSyntaxErrorText(int n)
	{
	  string s;
	  switch (n)
	  {
					case 0: s = "EOF expected"; break;
			case 1: s = "DOT expected"; break;
			case 2: s = "COMMA expected"; break;
			case 3: s = "COLON expected"; break;
			case 4: s = "EQUALS expected"; break;
			case 5: s = "OPENBRACE expected"; break;
			case 6: s = "CLOSEBRACE expected"; break;
			case 7: s = "IDENTIFIER expected"; break;
			case 8: s = "OPENBRACKET expected"; break;
			case 9: s = "CLOSEBRACKET expected"; break;
			case 10: s = "STRINGLITERAL expected"; break;
			case 11: s = "INTEGERLITERAL expected"; break;
			case 12: s = "PATH expected"; break;
			case 13: s = "??? expected"; break;
			case 14: s = "invalid PartialName"; break;
			case 15: s = "invalid Expression"; break;
			case 16: s = "invalid Literal"; break;
		default:
		  s = "error " + n;
		  break;
	  }
	  return s;
	}
  }
}
