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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class Tokenizer
  {
	internal Tokenizer()
	{
	}
	public virtual TokenCategory GetUncategorizedTokenCategory(Token token)
	{
	  CategorizedToken catToken = token as CategorizedToken;
	  if (catToken == null)
		return TokenCategory.Text;
	  return catToken.Category;
	}
	public virtual string GetTokenCategory(Token token, ParserVersion version)
	{
	  CategorizedToken catToken = token as CategorizedToken;
	  if (catToken == null)
		return string.Empty;
	  return catToken.GetTokenCategory();
	}
	public abstract IList<CategorizedToken> GetTokens(string code, EmbededLanguageKind languageKind, DotNetLanguageType embeddingDefaultLanguage);
	public IList<CategorizedToken> GetTokens(string code, DotNetLanguageType codeEmbeddingDefaultLanguage)
	{
	  return GetTokens(code, EmbededLanguageKind.Asp, codeEmbeddingDefaultLanguage);
	}
	public IList<CategorizedToken> GetTokens(string code)
	{
	  return GetTokens(code, EmbededLanguageKind.Asp, DotNetLanguageType.CSharp);
	}
	public string GetTokenCategory(Token token)
	{
	  return GetTokenCategory(token, ParserVersion.VS2014);
	}
  }
}
