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
using System;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public class CSharpDirectiveCodeGen : DirectiveCodeGenBase 
	{
		public CSharpDirectiveCodeGen(CodeGen codeGen) : base(codeGen) 
		{
		}
	# region  GenerateCode
	public override void GenerateCode(CodeWriter writer, LanguageElement languageElement, bool calculateIndent)
	{
	  if (writer == null)
		throw new ArgumentNullException("writer");
	  if (CodeGen == null)
		throw new ArgumentNullException("CodeGen is Null");
	  PushCodeWriter();
	  SetCodeWriter(writer);
	  if (calculateIndent)
		CalculateIndent(languageElement);
	  CodeGen.GenerateElement(languageElement);
	  if (calculateIndent)
		ResetIndent();
	  PopCodeWriter();
	}
	#endregion
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (element == null)
		return false;
	  if (element.ElementType == LanguageElementType.Region && Options.BlankLines.InsideRegionDirectives ||
		element.ElementType == LanguageElementType.EndRegionDirective && Options.BlankLines.AroundRegionDirectives)
	  {
		  CodeGen.AddNewLine(2);
		  return true;
	  }
	  if (element.ElementType == LanguageElementType.Region)
	  {
		CodeGen.AddNewLineIfNeeded();
		return true;
	  }
	  return false;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.DefineDirective:
		case FormattingTokenType.UndefDirective:
		case FormattingTokenType.IfDirective :
		case FormattingTokenType.ElifDirective:
		case FormattingTokenType.LineDirective:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  int newLineCount = 0;
	  switch (tokenType)
	  {
		case FormattingTokenType.DefineDirective:
		case FormattingTokenType.IfDirective:
		case FormattingTokenType.ElifDirective:
		case FormattingTokenType.ElseDirective:
		case FormattingTokenType.EndIfDirective:
		case FormattingTokenType.UndefDirective:
		case FormattingTokenType.ErrorDirective:
		case FormattingTokenType.WarningDirective:
		case FormattingTokenType.PragmaDirective:
		case FormattingTokenType.LineDirective:
		  newLineCount = 1;
		  break;
		case FormattingTokenType.RegionDirective:
		  newLineCount = Options.BlankLines.AroundRegionDirectives ? 2 : 1;
		  break;
		case FormattingTokenType.EndRegionDirective:
		  newLineCount = Options.BlankLines.InsideRegionDirectives ? 2 : 1;
		  break;
	  }
	  bool lastLineEmpty = string.IsNullOrEmpty(Code.LastLine);
	  if (newLineCount == 1)
	  {
		if (!lastLineEmpty)
		  result.AddNewLine(newLineCount);
	  }
	  else if (newLineCount == 2)
	  {
		if (lastLineEmpty)
		{
		  newLineCount--;
		  string[] lines = Code.Code.Split('\n'); 
		  if (lines.Length > 1 && string.IsNullOrEmpty(lines[lines.Length - 2].Trim()))
			newLineCount--;
		}
		if (newLineCount > 0)
		  result.AddNewLine(newLineCount);
	  }
	  return result;
	}
		protected override void GenerateDefineDirective(DefineDirective directive) 
		{
			if (directive == null)
				return;
			Write(FormattingTokenType.DefineDirective);
	  Write(FormattingTokenType.Ident);
		}
		protected override void GenerateIfDirective(IfDirective directive) 
		{
	  Write(FormattingTokenType.IfDirective);
			Write(directive.Expression);
		}
		protected override void GenerateElifDirective(ElifDirective directive) 
		{
	  Write(FormattingTokenType.ElifDirective);
			Write(directive.Expression);
		}
		protected override void GenerateElseDirective(ElseDirective directive) 
		{
	  Write(FormattingTokenType.ElseDirective);
		}
		protected override void GenerateEndIfDirective(EndIfDirective directive) 
		{
	  Write(FormattingTokenType.EndIfDirective);
		}
		protected override void GenerateUndefDirective(UndefDirective directive) 
		{
			Write(FormattingTokenType.UndefDirective);
			Write(directive.Symbol);
		}
		protected override void GenerateErrorDirective(ErrorDirective directive) 
		{
	  Write(FormattingTokenType.ErrorDirective);
	  if (!string.IsNullOrEmpty(directive.Expression) && !SaveFormat)
		CodeGen.TokenGen.TokenGenArgs.UserFormattingElements.AddWhiteSpace();
	  Write(directive.Expression);
		}
		protected override void GenerateWarningDirective(WarningDirective directive) 
		{
	  Write(FormattingTokenType.WarningDirective);
	  if (!string.IsNullOrEmpty(directive.Text) && !SaveFormat)
		CodeGen.TokenGen.TokenGenArgs.UserFormattingElements.AddWhiteSpace();
			Write(directive.Text);
		}
	protected override void GeneratePragmaDirective(PragmaDirective directive)
	{
	  Write(FormattingTokenType.PragmaDirective);
	  if (!string.IsNullOrEmpty(directive.Text) && !SaveFormat)
		CodeGen.TokenGen.TokenGenArgs.UserFormattingElements.AddWhiteSpace();
	  Write(directive.Text);
	}
	protected override void GenerateLineDirective(LineDirective directive) 
		{
	  Write(FormattingTokenType.LineDirective);
	  if ((directive.Hidden || directive.Default || directive.LineNumber > 0) && !SaveFormat)
		CodeGen.TokenGen.TokenGenArgs.UserFormattingElements.AddWhiteSpace();
	  if (directive.Hidden)
	  {
		Write("hidden");
		return;
	  }
	  if (directive.Default)
	  {
		Write("default");
		return;
	  }
	  if(directive.LineNumber > 0)
		Write(directive.LineNumber.ToString());
			if (directive.FileName != null)
				Write(" \"" + directive.FileName + "\"");
		}
		protected override void GenerateRegion(RegionDirective directive) 
		{
	  Write(FormattingTokenType.RegionDirective);
	  if (!string.IsNullOrEmpty(directive.Name) && !SaveFormat)
		CodeGen.TokenGen.TokenGenArgs.UserFormattingElements.AddWhiteSpace();
			Write(FormattingTokenType.Ident);
		}
		protected override void GenerateEndRegion(EndRegionDirective directive) 
		{
	  Write(FormattingTokenType.EndRegionDirective);
	  if (!string.IsNullOrEmpty(directive.Message) && !SaveFormat)
		CodeGen.TokenGen.TokenGenArgs.UserFormattingElements.AddWhiteSpace();
	  Write(directive.Message);
		}
		protected override void GenerateIfDefDirective(IfDefDirective directive)
		{
		}
		protected override void GenerateIfnDefDirective(IfnDefDirective directive)
		{
		}
		protected override void GenerateIncludeDirective(IncludeDirective directive)
		{
		}
		protected override void GenerateImportDirective(ImportDirective directive)
		{
		}
	}
}
