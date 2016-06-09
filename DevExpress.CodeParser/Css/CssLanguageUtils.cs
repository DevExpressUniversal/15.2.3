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
namespace DevExpress.CodeRush.StructuralParser.Css
#else
namespace DevExpress.CodeParser.Css
#endif
{
  using Xml;
  sealed class CssLanguageUtils : LanguageUtils
  {
	public override Tokenizer CreateTokenizer()
	{
	  return new CssTokenizer();
	}
	public override ParserBase CreateParser(ParserVersion version, EmbededLanguageKind languageKind, DotNetLanguageType embededLanguage)
	{
	  return new CssParser();
	}
	public override CodeGen CreateCodeGen()
	{
	  CodeGen codeGen = new HtmlXmlCodeGen();
	  if (codeGen != null)
		codeGen.Code = new CodeWriter(new StringWriter(), new CodeGenOptions(ParserLanguageID.Css));
	  return codeGen;
	}
	public override ElementBuilder CreateElementBuilder()
	{
	  return new AspElementBuilder();
	}
	public class CssTokenizer : Tokenizer
	{
	  public override IList<CategorizedToken> GetTokens(string code, EmbededLanguageKind languageKind, DotNetLanguageType codeEmbeddingDefaultLanguage)
	  {
		List<CategorizedToken> result = new List<CategorizedToken>();
		if (string.IsNullOrEmpty(code))
		  return result.AsReadOnly();
		ISourceReader reader = new SourceStringReader(code, 1, 1);
		CssParser parser = new CssParser();
		parser.SetTokensCategory = true;
		parser.Parse(reader);
		TokenCollection tokens = parser.SavedTokens;
		int tokenCount = tokens.Count;
		result.Capacity = tokenCount;
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
