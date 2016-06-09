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
  [Flags]
  public enum ParserLanguageID
  {
	None = 0,
	CSharp = 1,
	Basic = 2,
	Cpp = 4,
	Html = 8,
	JavaScript = 16,
	Xaml = 32,
	Xml = 64,
	Css = 128,
	FSharp = 256
  }
  public static class ParserLanguage
  {
	#region Constants
		const string STR_Cs = "cs";
		const string STR_DotCs = "." + STR_Cs;
	const string STR_Csproj = "csproj";
	const string STR_DotCSProj = ".csproj";
		const string STR_Vb = "vb";
		const string STR_DotVb = "." + STR_Vb;
	const string STR_Vbproj = "vbproj";
		const string STR_DotVBProj = ".vbproj";
	const string STR_H = "h";
	const string STR_DotH = ".h";
	const string STR_Hh = "hh";
	const string STR_DotHh = ".hh";
		const string STR_Cpp = "cpp";
		const string STR_DotCpp = "." + STR_Cpp;
		const string STR_CPPproj = "vcproj";
		const string STR_DotCppProj = ".vcproj";
		const string STR_Xaml = "xaml";
		const string STR_DotXaml = "." + STR_Xaml;
	const string STR_Js = "js";
	const string STR_DotJs = "." + STR_Js;
	const string STR_Html = "html";
	const string STR_DotHtml = "." + STR_Html;
	const string STR_Htm = "htm";
	const string STR_DotHtm = "." + STR_Htm;
	const string STR_Xml = "xml";
	const string STR_DotXml = "." + STR_Xml;
	const string STR_Css = "css";
	const string STR_DotCss = "." + STR_Css;
	#endregion
	public static string ToString(ParserLanguageID language)
		{
			return language.ToString();
	}
	public static ParserLanguageID FromString(string language)
		{
	  if (string.IsNullOrEmpty(language))
		return ParserLanguageID.None;
	  if (String.Compare(language, "InternalHTML", StringComparison.CurrentCultureIgnoreCase) == 0)
		return ParserLanguageID.Html;
	  try
	  {
		return (ParserLanguageID)Enum.Parse(typeof(ParserLanguageID), language, true);
	  }
	  catch {}
	  return ParserLanguageID.None;
	}
	public static ParserLanguageID FromFileExtension(string extension)
		{
	  switch (extension.ToLower())
			{
				case STR_Cs:
				case STR_DotCs:
				case STR_Csproj:
				case STR_DotCSProj:
					return ParserLanguageID.CSharp;
				case STR_Vb:
				case STR_DotVb:
				case STR_Vbproj:
				case STR_DotVBProj:
		  return ParserLanguageID.Basic;
		case STR_H:
		case STR_DotH:
		case STR_Hh:
		case STR_DotHh:
				case STR_Cpp:
				case STR_DotCpp:
				case STR_CPPproj:
				case STR_DotCppProj:
		  return ParserLanguageID.Cpp;
				case STR_Xaml:
				case STR_DotXaml:
		  return ParserLanguageID.Xaml;
		case STR_Js:
		case STR_DotJs:
		  return ParserLanguageID.JavaScript;
		case STR_Html:
		case STR_DotHtml:
		case STR_Htm:
		case STR_DotHtm:
		  return ParserLanguageID.Html;
		case STR_Xml:
		case STR_DotXml:
		  return ParserLanguageID.Xml;
		case STR_Css:
		case STR_DotCss:
		  return ParserLanguageID.Xml;
			}
	  return ParserLanguageID.None;
	}
  }
}
