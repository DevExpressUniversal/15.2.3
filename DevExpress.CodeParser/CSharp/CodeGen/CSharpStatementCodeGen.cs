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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
  public class CSharpStatementCodeGen : TokenCStatementCodeGenBase
  {
	public CSharpStatementCodeGen(CodeGen codeGen)
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
	public override bool GenerateElementTail(LanguageElement statement)
	{
	  Statement element = statement as Statement;
	  if (element == null)
		return false;
	  LanguageElement codeSibling = element.NextSibling;
	  if (codeSibling == null)
		return true;
	  if (element.ElementType == LanguageElementType.If
		&& (codeSibling.ElementType == LanguageElementType.Else || codeSibling.ElementType == LanguageElementType.ElseIf))
	  {
		if (element.NodeCount < 2 && CodeGen.Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine)
		{
		  CodeGen.AddWSIfNeeded();
		  return true;
		}
		if (!CodeGen.Options.LineBreaks.PlaceElseStatementOnNewLine)
		  return true;
	  }
	  if (element is PropertyAccessor)
	  {
		Property property = element.Parent as Property;
		if (property != null)
		{
		  if (property.IsAbstract || property.IsAutoImplemented)
			return true;
		  LanguageElement parent = property.Parent;
		  if (parent != null && parent.ElementType == LanguageElementType.Interface)
			return true;
		}
	  }
	  if (element is Try)
	  {
		if (codeSibling is Catch && !Options.LineBreaks.PlaceCatchStatementOnNewLine)
		  return true;
		if (codeSibling is Finally && !Options.LineBreaks.PlaceFinallyStatementOnNewLine)
		  return true;
	  }
	  if (element is Catch && codeSibling is Finally && !Options.LineBreaks.PlaceFinallyStatementOnNewLine)
		return true;
	  CodeGen.AddNewLineIfNeeded();
	  return true;
	}
	void GenerateEventAccessorStatement(EventAccessor statement, FormattingTokenType keyword)
	{
	  Event ev = statement.Parent as Event;
	  if (ev == null)
		return;
	  Interface parentInterface = statement.GetParentClassInterfaceOrStruct() as Interface;
	  if (ev.IsAbstract || parentInterface != null)
	  {
		Write(keyword);
		Write(FormattingTokenType.Semicolon);
		return;
	  }
	  Write(keyword);
	  if (statement.GenerateCodeBlock)
	  {
		Write(FormattingTokenType.CurlyBraceOpen);
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None);
		Write(FormattingTokenType.CurlyBraceClose);
	  }
	  if (statement.HasEndingSemicolon)
		Write(FormattingTokenType.Semicolon);
	}
	void GeneratePropertyAccessorStatement(PropertyAccessor statement, FormattingTokenType keyword)
	{
	  Property property = statement.Parent as Property;
	  Interface parentInterface = statement.GetParentClassInterfaceOrStruct() as Interface;
	  if (property == null || property.Visibility != statement.Visibility)
		CodeGenHelper.GenerateVisibility(this, statement.Visibility);
	  Write(keyword);
	  if ((property != null && (property.IsAbstract || property.IsAutoImplemented)) || parentInterface != null)
	  {
		Write(FormattingTokenType.Semicolon);
		return;
	  }
	  if (statement.GenerateCodeBlock)
	  {
		Write(FormattingTokenType.CurlyBraceOpen);
		GenerateElementCollection(statement.Nodes);
		Write(FormattingTokenType.CurlyBraceClose);
	  }
	  if(statement.HasEndingSemicolon)
		Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateThrowStatement(Throw statement)
	{
	  Write(FormattingTokenType.Throw);
	  CodeGen.GenerateElement(statement.Expression);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateCheckedStatement(Checked statement)
	{
	  Write(FormattingTokenType.Checked);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateUncheckedStatement(Unchecked statement)
	{
	  Write(FormattingTokenType.Unchecked);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateFixedStatement(Fixed statement)
	{
	  Write(FormattingTokenType.Fixed);
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(statement.DetailNodes, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateLockStatement(Lock statement)
	{
	  Write(FormattingTokenType.Lock);
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(statement.DetailNodes, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateEventAddStatement(EventAdd statement)
	{
	  GenerateEventAccessorStatement(statement, FormattingTokenType.Add);
	}
	protected override void GenerateEventRemoveStatement(EventRemove statement)
	{
	  GenerateEventAccessorStatement(statement, FormattingTokenType.Remove);
	}
	protected override void GeneratePropertyAccessorGetStatement(Get statement)
	{
	  GeneratePropertyAccessorStatement(statement, FormattingTokenType.Get);
	}
	protected override void GeneratePropertyAccessorSetStatement(Set statement)
	{
	  GeneratePropertyAccessorStatement(statement, FormattingTokenType.Set);
	}
	protected override void GenerateStatementStatement(Statement statement)
		{
	  if (statement.Name == ";")
	  {
		Write(FormattingTokenType.Semicolon);
		return;
	  }
			base.GenerateStatementStatement(statement);
		}
	protected override void GenerateWithStatement(With statement)
	{
	}
	protected override void GenerateEventRaiseStatement(EventRaise statement)
	{
	}
	protected override void GenerateRaiseEventStatement(RaiseEvent statement)
	{
	  CodeGen.GenerateElement(statement.Expression);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateYieldBreakStatement(YieldBreak statement)
	{
	  Write(FormattingTokenType.Yield);
	  Write(FormattingTokenType.Break);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateYieldReturnStatement(YieldReturn statement)
	{
	  Write(FormattingTokenType.Yield);
	  Write(FormattingTokenType.Return);
	  CodeGen.GenerateElement(statement.Expression);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateConstructorInitializer(ConstructorInitializer initializer)
	{
	  Write(FormattingTokenType.Colon);
	  if (initializer.IsBase)
		Write(FormattingTokenType.Base);
	  else
		Write(FormattingTokenType.This);
	  GenerateParameters(initializer.Arguments);
	}
		protected override void GenerateMethodCallStatement(MethodCall statement) 
		{
	  if (statement.Qualifier != null)
		CodeGen.GenerateElement(statement.Qualifier);
	  else
		if (!String.IsNullOrEmpty(statement.Name))
		  Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(statement.Arguments, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	  Write(FormattingTokenType.Semicolon);
		}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch(tokenType)
	  {
		case FormattingTokenType.Yield:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Throw:
		  Throw throwStatement = Context as Throw;
		  if (throwStatement != null && throwStatement.Expression != null)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Colon:
		  if (Context.ElementType == LanguageElementType.ConstructorInitializer)
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	}
}
