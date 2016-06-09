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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using CSharp;
  using VB;
  using Html;
  using Xml;
  using JavaScript;
  using Css;
  public static class ParserFactory
  {
	const ParserVersion DefaultParserVersion = ParserVersion.VS2010;
	public static ParserBase CreateParserForFileExtension(string extension)
	{
	  return CreateParserForFileExtension(extension, DefaultParserVersion);
	}
	public static ParserBase CreateParserForFileExtension(string extension, ParserVersion parserVersion)
	{
	  return CreateParser(ParserLanguage.FromFileExtension(extension), parserVersion);
	}
	public static ParserBase CreateParser(string language)
	{
	  return CreateParser(language, DefaultParserVersion);
	}
	public static ParserBase CreateParser(string language, ParserVersion parserVersion)
	{
	  return CreateParser(ParserLanguage.FromString(language), parserVersion);
	}
	public static ParserBase CreateParser(ParserLanguageID languageID)
	{
	  return CreateParser(languageID, DefaultParserVersion);
	}
	public static ParserBase CreateParser(ParserLanguageID languageID, ParserVersion parserVersion)
	{
	  switch(languageID)
	  {
		case ParserLanguageID.None:
		  return null;
		case ParserLanguageID.CSharp:
		  CSharp30Parser parser = new CSharp30Parser();
		  if (parserVersion == ParserVersion.VS2008 || 
			parserVersion == ParserVersion.VS2010 ||
			parserVersion == ParserVersion.VS2012)
			parser.WorkAsCharp30Parser = true;
		  return parser;
		case ParserLanguageID.Basic:
		  VB10Parser vbparser = new VB10Parser();
				  vbparser.VsVersion = parserVersion;
				  return vbparser;
		case ParserLanguageID.JavaScript:
		  return new JavaScriptParser();
		case ParserLanguageID.Html:
		  return new HtmlParser();
		case ParserLanguageID.Xaml:
		  return new HtmlParser(true);
		case ParserLanguageID.Xml:
		  return new NewXmlParser();
		case ParserLanguageID.Css:
		  return new CssParser();
	  }
	  return null;
	}
  }
	public static class TokenCategoryHelperFactory {
		public static ITokenCategoryHelper CreateHelperForFileExtensions(string extension) {
			if (extension.ToLower() == "css" || extension.ToLower() == ".css")
				return CreateHelper(ParserLanguageID.Css);
			return CreateHelper(ParserLanguage.FromFileExtension(extension));
		}
		public static ITokenCategoryHelper CreateHelper(ParserLanguageID languageID) {
			switch (languageID) {
				case ParserLanguageID.None:
					return null;
				case ParserLanguageID.CSharp:
					return new CSharpTokensCategoryHelper();
				case ParserLanguageID.Basic:
					return new VBTokensCategoryHelper();
				case ParserLanguageID.JavaScript:
					return new JavaScriptTokensCategoryHelper();
				case ParserLanguageID.Html:
					return new HtmlTokensCategoryHelper();
				case ParserLanguageID.Xaml:
					return new HtmlTokensCategoryHelper();
				case ParserLanguageID.Xml:
					return new XmlTokensCategoryHelper();
				case ParserLanguageID.Css:
					return new CssTokensCategoryHelper();
			}
			return null;
		}
	}
}
