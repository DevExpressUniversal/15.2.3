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
using System.Globalization;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  internal class FormattingElementsWriter
  {
	public static void WriteSupportElement(CodeGen codeGen, IFormattingElement element)
	{
	  if (element == null)
		return;
	  bool oldValue = codeGen.GeneratingInSupportElement;
	  codeGen.GeneratingInSupportElement = true;
	  try
	  {
		if (WriteFormattingElement(codeGen, element as FormattingElement))
		  return;
		if (WriteComment(codeGen, element as Comment))
		  return;
		if (WritePpDirective(codeGen, element as PreprocessorDirective))
		  return;
		if (WriteFormattingText(codeGen, element as FormattingText))
		  return;
	  }
	  finally
	  {
		codeGen.GeneratingInSupportElement = oldValue;
	  }
	}
	public static void WriteEOL(CodeGen codeGen)
	{
	  codeGen.WriteEOL();
	}
	static bool WriteComment(CodeGen codeGen, Comment comment)
	{
	  if (comment == null)
		return false;
	  codeGen.GenerateElement(comment);
	  codeGen.AddSkiped(comment);
	  return true;
	}
	static bool WriteFormattingText(CodeGen codeGen, FormattingText formattingText)
	{
	  if (formattingText == null)
		return false;
	  string text = formattingText.Text;
	  if (String.IsNullOrEmpty(text))
		return true;
	  codeGen.Code.WriteClearFormat(text);
	  return true;
	}
	static bool WritePpDirective(CodeGen codeGen, PreprocessorDirective element)
	{
	  if (element == null)
		return false;
	  int indentLevel = codeGen.Code.IndentLevel;
	  if (element.ElementType != LanguageElementType.Region && element.ElementType != LanguageElementType.EndRegionDirective)
		codeGen.Code.IndentLevel = 0;
	  codeGen.GenerateElement(element);
	  codeGen.Code.IndentLevel = indentLevel;
	  return true;
	}
	static void WriteIndent(CodeGen codeGen, CheckedIndent indent)
	{
	  string text = new string('\t', indent.CountConsecutive) + new string(' ', indent.SpaceCount);
	  string indentSpaces = codeGen.Code.IndentString;
	  if (indentSpaces.Length >= text.Length)
		return;
	  text = text.Substring(indentSpaces.Length);
	  codeGen.Code.Write(text);
	}
	static bool WriteFormattingElement(CodeGen codeGen, FormattingElement formatingElement)
	{
	  if (formatingElement == null)
		return false;
	  CheckedIndent indent = formatingElement as CheckedIndent;
	  if (indent != null)
	  {
		WriteIndent(codeGen, indent);
		return true;
	  }
	  for (int i = 0; i < formatingElement.CountConsecutive; i++)
	  {
		switch (formatingElement.Type)
		{
		  case FormattingElementType.WS:
			WriteWS(codeGen);
			break;
		  case FormattingElementType.Tab:
			WriteTab(codeGen);
			break;
		  case FormattingElementType.CR:
			WriteCR(codeGen);
			break;
		  case FormattingElementType.EOL:
			WriteEOL(codeGen);
			break;
		  case FormattingElementType.IncreaseIndent:
			IncreaseIndent(codeGen);
			break;
		  case FormattingElementType.DecreaseIndent:
			DecreaseIndent(codeGen);
			break;
		  case FormattingElementType.IncreaseAlignment:
			IncreaseAlignment(codeGen);
			break;
		  case FormattingElementType.DecreaseAlignment:
			DecreaseAlignment(codeGen);
			break;
		  case FormattingElementType.ClearIndent:
			codeGen.Code.ClearIndent();
			break;
		  case FormattingElementType.RestoreIndent:
			codeGen.Code.RestoreIndent();
			break;
		  default:
			return false;
		}
	  }
	  return true;
	}
	static void WriteWS(CodeGen codeGen)
	{
	  codeGen.WriteWS();
	}
	static void WriteTab(CodeGen codeGen)
	{
	  codeGen.WriteTab();
	}
	static void WriteCR(CodeGen codeGen)
	{
	}
	static void IncreaseIndent(CodeGen codeGen)
	{
	  codeGen.IncreaseIndent();
	}
	static void DecreaseIndent(CodeGen codeGen)
	{
	  codeGen.DecreaseIndent();
	}
	static void IncreaseAlignment(CodeGen codeGen)
	{
	  codeGen.Code.IncreaseAlignment();
	}
	static void DecreaseAlignment(CodeGen codeGen)
	{
	  codeGen.Code.DecreaseAlignment();
	}
  }
}
