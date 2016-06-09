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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  using VBScanner = VB90Scanner;
  using ParserBase = VB90Parser;
  public class VBTokensCategoryHelper : ITokenCategoryHelper
  {
	public TokenCategory GetUncategorizedTokenCategory(Token token)
	{
	  return VBTokensHelper.GetUncategorizedTokenCategory(token);
	}
	public TokenCollection GetTokens(string code)
	{
	  return VBTokensHelper.GetTokens(code);
	}
	public string GetTokenCategory(Token token)
	{
	  return VBTokensHelper.GetTokenCategory(token);
	}
	public string GetTokenCategory(Token token, ParserVersion version)
	{
	  return VBTokensHelper.GetTokenCategory(token, version);
	}
  }
	public class VBTokensHelper
	{
	const string STR_XMLDocTag = "XML Doc Tag";
		public static TokenCollection GetTokens(string code)
		{
			if (code == null || code.Length == 0)
				return null;
	  ISourceReader reader = new SourceStringReader(code, 1, 1);
	  VB10Parser parser = new VB10Parser();
	  parser.SetTokensCategory = true;
	  parser.Parse(reader);
	  return parser.SavedTokens;
		}
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
			return ParserBase.IsKeyword(type);
		}
	public static string GetTokenCategory(Token token)
	{
	  return GetTokenCategory(token, ParserVersion.VS2008);
	}
	public static TokenCategory GetUncategorizedTokenCategory(Token token)
	{
	  if (token == null)
		return TokenCategory.Text;
			if (IsKeyword(token.Type))
		return TokenCategory.Keyword;
			if (ParserBase.IsOperator(token.Type))
		return TokenCategory.Operator;
			if (ParserBase.IsPreprocessorDirective(token.Type))
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
		public static string GetTokenCategory(Token token, ParserVersion version)
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
			} else if (token.Type == Tokens.SingleLineXmlComment)
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
	}
}
