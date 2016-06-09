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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using VB;
  using CSharp;
  using JavaScript;
  using Html;
  using Css;
  using Xml;
  public abstract class LanguageUtils
  {
	const ParserVersion ENUM_LastParsertVersion = ParserVersion.VS2014;
	internal LanguageUtils()
	{
	}
	public static ParserLanguageID GetLanguageId(string fileNameOrFileExtension)
	{
	  if (string.IsNullOrEmpty(fileNameOrFileExtension))
		return ParserLanguageID.None;
	  int dotIndex = fileNameOrFileExtension.LastIndexOf('.');
	  string fileExtensionValue;
	  if (dotIndex < 0)
		fileExtensionValue = fileNameOrFileExtension;
	  else
		fileExtensionValue = fileNameOrFileExtension.Remove(0, dotIndex + 1);
	  fileExtensionValue = fileExtensionValue.ToLower();
	  switch (fileExtensionValue)
	  {
		case "css":
		  return ParserLanguageID.Css;
		case "cs":
		case "csproj":
		  return ParserLanguageID.CSharp;
		case "vb":
		case "vbproj":
		  return ParserLanguageID.Basic;
		case "h":
		case "hh":
		case "cpp":
		case "vcproj":
		  return ParserLanguageID.Cpp;
		case "js":
		  return ParserLanguageID.JavaScript;
		case "xml":
		  return ParserLanguageID.Xml;
		case "htm":
		case "html":
		case "cshtml":
		case "vbhtml":
		case "asax":
		case "aspx":
		case "ascx":
		case "ashx":
		case "asmx":
		case "master":
		  return ParserLanguageID.Html;
		case "xaml":
		  return ParserLanguageID.Xaml;
		default:
		  return ParserLanguageID.None;
	  }
	}
	public static LanguageUtils Create(ParserLanguageID languageId)
	{
	  switch(languageId)
	  {
		case ParserLanguageID.Basic:
		  return new VBLanguageUtils();
		case ParserLanguageID.CSharp:
		  return new CSharpLanguageUtils();
		case ParserLanguageID.JavaScript:
		  return new JavaScriptLanguageUtils();
		case ParserLanguageID.Html:
		  return new HtmlLanguageUtils();
		case ParserLanguageID.Xaml:
		  return new XamlLanguageUtils();
		case ParserLanguageID.Xml:
		  return new XmlLanguageUtils();
		case ParserLanguageID.Css:
		  return new CssLanguageUtils();
	  }
	  return null;
	}
	public static LanguageUtils Create(string fileNameOrFileExtension)
	{
	  ParserLanguageID parserLanguage = GetLanguageId(fileNameOrFileExtension);
	  return Create(parserLanguage);
	}
	public static Tokenizer GetTokenizer(ParserLanguageID languageId)
	{
	  LanguageUtils utils = Create(languageId);
	  if (utils == null)
		return null;
	  return utils.CreateTokenizer();
	}
	public static Tokenizer GetTokenizer(string fileExtension)
	{
	  ParserLanguageID languageId = GetLanguageId(fileExtension);
	  return GetTokenizer(languageId);
	}
	public ParserBase CreateParser()
	{
	  return CreateParser(ENUM_LastParsertVersion, EmbededLanguageKind.Asp, DotNetLanguageType.CSharp);
	}
	public abstract Tokenizer CreateTokenizer();
	public abstract ParserBase CreateParser(ParserVersion version, EmbededLanguageKind languageKind, DotNetLanguageType embededLanguage);
	public abstract CodeGen CreateCodeGen();
	public abstract ElementBuilder CreateElementBuilder();
  }
}
