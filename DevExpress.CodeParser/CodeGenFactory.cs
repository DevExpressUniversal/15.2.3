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

using System.IO;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using CSharp;
  using VB;
  using JavaScript;
  public static class CodeGenFactory
  {
	public static CodeGen CreateCodeGenForFileExtension(string extension)
	{
	  return CreateCodeGen(ParserLanguage.FromFileExtension(extension));
	}
	public static CodeGen CreateCodeGen(string language)
	{
	  return CreateCodeGen(ParserLanguage.FromString(language));
	}
	public static CodeGen CreateCodeGen(ParserLanguageID languageID)
	{
		CodeGen codeGen = FindCodeGenerator(languageID);
		if(codeGen != null)
			codeGen.Code = new CodeWriter(new StringWriter(), new CodeGenOptions(languageID));
		return codeGen;
	}
	static CodeGen FindCodeGenerator(ParserLanguageID languageID){
	  switch (languageID)
	  {
		case ParserLanguageID.None:
		  return null;
		case ParserLanguageID.CSharp:
		  return new CSharpCodeGen();
		case ParserLanguageID.Basic:
		  return new VBCodeGen();
		case ParserLanguageID.JavaScript:
		  return new JavaScriptCodegen();
	  }
	  return null;
	}
  }
}
