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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
  sealed class CSharpLanguageUtils : LanguageUtils
  {
	public override Tokenizer CreateTokenizer()
	{
	  return new CSharpTokenizer();
	}
	public override ParserBase CreateParser(ParserVersion version, EmbededLanguageKind languageKind, DotNetLanguageType embededLanguage)
	{
	  CSharp30Parser parser = new CSharp30Parser();
	  if (version > ParserVersion.VS2005)
		parser.WorkAsCharp30Parser = true;
	  return parser;
	}
	public override CodeGen CreateCodeGen()
	{
	  return new CSharpCodeGen();
	}
	public override ElementBuilder CreateElementBuilder()
	{
	  return new ElementBuilder();
	}
	class CSharpTokenizer : Tokenizer
	{
	  public override IList<CategorizedToken> GetTokens(string code, EmbededLanguageKind languageKind, DotNetLanguageType codeEmbeddingDefaultLanguage)
	  {
		if (string.IsNullOrEmpty(code))
		  return null;
		ISourceReader reader = new SourceStringReader(code, 1, 1);
		CSharp30Parser parser = new CSharp30Parser();
		parser.WorkAsCharp30Parser = true;
		parser.SetTokensCategory = true;
		parser.Parse(reader);
		TokenCollection tokens = parser.SavedTokens;
		int tokenCount = tokens.Count;
		List<CategorizedToken> result = new List<CategorizedToken>();
		for (int i = 0; i < tokenCount; i++)
		{
		  CategorizedToken categorizedToken = (CategorizedToken)tokens[i];
		  result.Add(categorizedToken);
		}
		return result.AsReadOnly();
	  }
	}
  }
}
