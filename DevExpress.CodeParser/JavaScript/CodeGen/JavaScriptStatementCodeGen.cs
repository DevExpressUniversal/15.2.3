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
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
  public class JavaScriptStatementCodeGen : TokenCStatementCodeGenBase
  {
	public JavaScriptStatementCodeGen(CodeGen codeGen)
	  : base(codeGen)
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
	#region Methods for generation statements, which are'nt present in JavaScript
	protected override void GenerateCheckedStatement(Checked statement)
	{
	}
	protected override void GenerateConstructorInitializer(ConstructorInitializer initializer)
	{
	}
	protected override void GenerateEventAddStatement(EventAdd statement)
	{
	}
	protected override void GenerateEventRaiseStatement(EventRaise statement)
	{
	}
	protected override void GenerateEventRemoveStatement(EventRemove statement)
	{
	}
	protected override void GenerateFixedStatement(Fixed statement)
	{
	}
	protected override void GenerateLockStatement(Lock statement)
	{
	}
	protected override void GeneratePropertyAccessorGetStatement(Get statement)
	{
	}
	protected override void GeneratePropertyAccessorSetStatement(Set statement)
	{
	}
	protected override void GenerateRaiseEventStatement(RaiseEvent statement)
	{
	}
	protected override void GenerateUncheckedStatement(Unchecked statement)
	{
	}
	protected override void GenerateYieldBreakStatement(YieldBreak statement)
	{
	}
	protected override void GenerateYieldReturnStatement(YieldReturn statement)
	{
	}
	#endregion
	#region GenerateBreakStatement
	protected override void GenerateBreakStatement(Break statement)
	{
	  Write(FormattingTokenType.Break);
	  if (!string.IsNullOrEmpty(statement.Name) &&
		  statement.Name != "break")
		Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.Semicolon);
	}
	#endregion
	#region GenerateContinueStatement
	protected override void GenerateContinueStatement(Continue statement)
	{
	  Write(FormattingTokenType.Continue);
	  if (!string.IsNullOrEmpty(statement.Name) &&
		  statement.Name != "continue")
		Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.Semicolon);
	}
	#endregion
	#region GenerateEmptyStatement
	protected override void GenerateEmptyStatement(EmptyStatement statement)
	{
	  Write(FormattingTokenType.Semicolon);
	}
	#endregion
	#region GenerateForEachStatement
	protected override void GenerateForEachStatement(ForEach statement)
	{
	  if (statement.DetailNodeCount < 2)
		return;
	  Write(FormattingTokenType.For);
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement((LanguageElement)statement.DetailNodes[0]);
	  Write(FormattingTokenType.In);
	  CodeGen.GenerateElement((LanguageElement)statement.DetailNodes[1]);
	  Write(FormattingTokenType.ParenClose);
	  GenerateBlockStatementContents(statement);
	}
	#endregion
	#region GenerateThrowStatement
	protected override void GenerateThrowStatement(Throw statement)
	{
	  Write(FormattingTokenType.Throw);
	  CodeGen.GenerateElement(statement.Expression);
	  Write(FormattingTokenType.Semicolon);
	}
	#endregion
	#region GenerateWithStatement
	protected override void GenerateWithStatement(With statement)
	{
	  GenerateParentingStatement(statement, FormattingTokenType.With, FormattingTokenType.None);
	}
	#endregion
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.CurlyBraceClose:
		  if (Context.ElementType == LanguageElementType.Block || Context.ElementType == LanguageElementType.Switch)
			Code.DecreaseIndent();
		  break;
		case FormattingTokenType.Default:
			Code.WriteLine();
		  break;
	  }
	  return result;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Semicolon:
		  if (Context.ElementType != LanguageElementType.For)
			result.AddNewLine();
		  break;
		case FormattingTokenType.CurlyBraceOpen:
		  if (Context.ElementType == LanguageElementType.Block || Context.ElementType == LanguageElementType.Switch)
			Code.IncreaseIndent();
		  break;
		case FormattingTokenType.With:
		case FormattingTokenType.For:
		case FormattingTokenType.Throw:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Continue:
		  if (!string.IsNullOrEmpty(Context.Name) &&
			  Context.Name != "continue")
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Break:
		  if (!string.IsNullOrEmpty(Context.Name) &&
			  Context.Name != "break")
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
  }
}
