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
namespace DevExpress.CodeRush.StructuralParser.Html
#else
namespace DevExpress.CodeParser.Html
#endif
{
  using Xml;
  public enum LanguageKind
  {
	Unknown,
	Xml,
	Html,
	Razor
  }
  public class HtmlTokensCategoryHelper : ITokenCategoryHelper
  {
	public TokenCategory GetUncategorizedTokenCategory(Token token)
	{
	  return TokenCategory.Unknown;
	}
	public TokenCollection GetTokens(string code)
	{
			return GetTokens(code, DotNetLanguageType.Unknown);
	}
		public TokenCollection GetTokens(string code, DotNetLanguageType codeEmbeddingLanguage)
		{
			return HtmlTokensHelper.GetTokens(code, LanguageKind.Html, codeEmbeddingLanguage);
		}
	public string GetTokenCategory(Token token)
	{
	  return HtmlTokensHelper.GetTokenCategory(token);
	}
	public string GetTokenCategory(Token token, ParserVersion version)
	{
	  return HtmlTokensHelper.GetTokenCategory(token);
	}
  }
  public class XmlTokensCategoryHelper : ITokenCategoryHelper
  {
	public TokenCategory GetUncategorizedTokenCategory(Token token)
	{
	  return TokenCategory.Unknown;
	}
	public TokenCollection GetTokens(string code)
	{
	  return HtmlTokensHelper.GetTokens(code, LanguageKind.Xml);
	}
	public string GetTokenCategory(Token token)
	{
	  return HtmlTokensHelper.GetTokenCategory(token);
	}
	public string GetTokenCategory(Token token, ParserVersion version)
	{
	  return HtmlTokensHelper.GetTokenCategory(token);
	}
  }
  public class HtmlTokensHelper
  {
	static void CalculatePositions(TokenCollection tokens, string code)
	{
	  int pos = 0;
	  foreach (Token token in tokens)
	  {
		string value = token.Value;
		pos = code.IndexOf(value, pos);
		token.StartPosition = pos;
		pos += value.Length;
		token.EndPosition = pos;
	  }
	}
	public static TokenCollection GetTokens(string code)
	{
	  return GetTokens(code, LanguageKind.Html);
	}
	public static TokenCollection GetTokens(string code, LanguageKind languageKind)
	{
			return GetTokens(code, languageKind, DotNetLanguageType.Unknown);
	}
		public static TokenCollection GetTokens(string code, LanguageKind languageKind, DotNetLanguageType codeEmbeddingDefaultLanguage)
		{
			if (code == null || code.Length == 0)
				return null;
			ISourceReader reader = new SourceStringReader(code, 1, 1);
			HtmlParser parser = new HtmlParser(languageKind == LanguageKind.Razor, languageKind == LanguageKind.Xml, codeEmbeddingDefaultLanguage);
			parser.SetTokensCategory = true;
			parser.Parse(reader);
			TokenCollection result = parser.SavedTokens;
	  if (languageKind == LanguageKind.Razor)
		CalculatePositions(result, code);
			return result;
		}
	public static string GetTokenCategory(Token token)
	{
	  CategorizedToken htmlToken = token as CategorizedToken;
	  if (htmlToken != null)
		return htmlToken.GetTokenCategory();
	  return "Text";
	}
  }
}
