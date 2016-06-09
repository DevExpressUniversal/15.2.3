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

using System.Collections.Generic;
using System.IO;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  sealed class VBLanguageUtils : LanguageUtils
  {
	public override ParserBase CreateParser(ParserVersion version, EmbededLanguageKind languageKind, DotNetLanguageType embededLanguage)
	{
	  if (version > ParserVersion.VS2008)
		return new VB10Parser();
	  else
		return new VB90Parser();
	}
	public override CodeGen CreateCodeGen()
	{
	  CodeGen codeGen = new VB90CodeGen();
	  if (codeGen != null)
		codeGen.Code = new CodeWriter(new StringWriter(), new CodeGenOptions(ParserLanguageID.Basic));
	  return codeGen;
	}
	public override ElementBuilder CreateElementBuilder()
	{
	  return new VBElementBuilder();
	}
	public override Tokenizer CreateTokenizer()
	{
	  return new VBTokenizer();
	}
	class VBTokenizer : Tokenizer
	{
	  const string STR_XMLDocTag = "XML Doc Tag";
	  static bool IsString(int type)
	  {
		return type == Tokens.StringLiteral ||
		  type == Tokens.CharacterLiteral;
	  }
	  static bool IsNumber(int type)
	  {
		return type == Tokens.IntegerLiteral ||
		  type == Tokens.FloatingPointLiteral;
	  }
	  static bool IsComment(int type)
	  {
		return type == Tokens.SingleLineComment;
	  }
	  static bool IsKeyword(int type)
	  {
		if (type == Tokens.Compare)
		  return false;
		return VB90Parser.IsKeyword(type);
	  }
	  public override TokenCategory GetUncategorizedTokenCategory(Token token)
	  {
		if (token == null)
		  return TokenCategory.Text;
		if (IsKeyword(token.Type))
		  return TokenCategory.Keyword;
		if (VB90Parser.IsOperator(token.Type))
		  return TokenCategory.Operator;
		if (VB90Parser.IsPreprocessorDirective(token.Type))
		  return TokenCategory.PreprocessorKeyword;
		if (IsString(token.Type))
		  return TokenCategory.String;
		if (IsComment(token.Type))
		  return TokenCategory.Comment;
		if (token.Type == Tokens.SingleLineXmlComment)
		  return TokenCategory.XmlComment;
		if (token.Type == Tokens.Identifier)
		{
		  string value = token.Value.ToLower();
		  if (value != null &&
			  (value == "string" || value == "object" ||
			  value == "getxmlnamespace"))
			return TokenCategory.Keyword;
		  return TokenCategory.Identifier;
		}
		return TokenCategory.Text;
	  }
	  public override string GetTokenCategory(Token token, ParserVersion version)
	  {
		if (token == null)
		  return "Text";
		CategorizedToken catToken = token as CategorizedToken;
		if (catToken == null)
		  return "Text";
		if (token.Type == Tokens.Identifier)
		{
		  string value = token.Value.ToLower();
		  if (value != null &&
				  (value == "string" || value == "object" ||
				  ((int)version >= (int)ParserVersion.VS2008) && value == "getxmlnamespace"))
			return "Keyword";
		}
		else if (token.Type == Tokens.SingleLineXmlComment)
		{
		  try
		  {
			string xmlTagName = StructuralParserServicesHolder.GetXMLTagName();
			if (xmlTagName != null)
			  return xmlTagName;
			return STR_XMLDocTag;
		  }
		  catch
		  {
			return STR_XMLDocTag;
		  }
		}
		return catToken.GetTokenCategory();
	  }
	  public override IList<CategorizedToken> GetTokens(string code, EmbededLanguageKind languageKind, DotNetLanguageType codeEmbeddingDefaultLanguage)
	  {
		if (string.IsNullOrEmpty(code))
		  return new List<CategorizedToken>().AsReadOnly();
		ISourceReader reader = new SourceStringReader(code, 1, 1);
		VB10Parser parser = new VB10Parser();
		parser.SetTokensCategory = true;
		parser.Parse(reader);
		TokenCollection tokens = parser.SavedTokens;
		int tokenCount = parser.SavedTokens.Count;
		List<CategorizedToken> result = new List<CategorizedToken>(tokenCount);
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
