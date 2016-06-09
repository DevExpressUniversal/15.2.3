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
using System.Collections;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class StatementCodeGenBase : LanguageElementCodeGenBase
  {
	public StatementCodeGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected virtual bool GenerateStatement(Statement statement)
	{
	  if (statement == null)
		return false;
	  switch (statement.ElementType)
	  {
		case LanguageElementType.MethodCall:
		  GenerateMethodCallStatement(statement as MethodCall);
		  return true;
		case LanguageElementType.Abort:
		  GenerateAbortStatement(statement as Abort);
		  return true;
		case LanguageElementType.Throw:
		  GenerateThrowStatement(statement as Throw);
		  return true;
		case LanguageElementType.Break:
		  GenerateBreakStatement(statement as Break);
		  return true;
		case LanguageElementType.Continue:
		  GenerateContinueStatement(statement as Continue);
		  return true;
		case LanguageElementType.Statement:
		  GenerateStatementStatement(statement as Statement);
		  return true;
		case LanguageElementType.Goto:
		  GenerateGotoStatement(statement as Goto);
		  return true;
		case LanguageElementType.Return:
		  GenerateReturnStatement(statement as Return);
		  return true;
		case LanguageElementType.Assignment:
		  GenerateAssignmentStatement(statement as Assignment);
		  return true;
		case LanguageElementType.Block:
		  GenerateBlockStatement(statement as Block);
		  return true;
		case LanguageElementType.Checked:
		  GenerateCheckedStatement(statement as Checked);
		  return true;
		case LanguageElementType.Unchecked:
		  GenerateUncheckedStatement(statement as Unchecked);
		  return true;
		case LanguageElementType.Switch:
		  GenerateSwitchStatement(statement as Switch);
		  return true;
		case LanguageElementType.Case:
		  GenerateCaseStatement(statement as Case);
		  return true;
		case LanguageElementType.Try:
		  GenerateTryStatement(statement as Try);
		  return true;
		case LanguageElementType.Catch:
		  GenerateCatchStatement(statement as Catch);
		  return true;
		case LanguageElementType.Finally:
		  GenerateFinallyStatement(statement as Finally);
		  return true;
		case LanguageElementType.Fixed:
		  GenerateFixedStatement(statement as Fixed);
		  return true;
		case LanguageElementType.Lock:
		  GenerateLockStatement(statement as Lock);
		  return true;
		case LanguageElementType.UsingStatement:
		  GenerateUsingStatement(statement as UsingStatement);
		  return true;
		case LanguageElementType.Do:
		  GenerateDoStatement(statement as Do);
		  return true;
		case LanguageElementType.For:
		  GenerateForStatement(statement as For);
		  return true;
		case LanguageElementType.ForEach:
		  GenerateForEachStatement(statement as ForEach);
		  return true;
		case LanguageElementType.While:
		  GenerateWhileStatement(statement as While);
		  return true;
		case LanguageElementType.If:
		  GenerateIfStatement(statement as If);
		  return true;
		case LanguageElementType.ElseIf:
		  GenerateElseIfStatement(statement as ElseIf);
		  return true;
		case LanguageElementType.Else:
		  GenerateElseStatement(statement as Else);
		  return true;
		case LanguageElementType.EventAdd:
		  GenerateEventAddStatement(statement as EventAdd);
		  return true;
		case LanguageElementType.EventRemove:
		  GenerateEventRemoveStatement(statement as EventRemove);
		  return true;
		case LanguageElementType.EventRaise:
		  GenerateEventRaiseStatement(statement as EventRaise);
		  return true;
		case LanguageElementType.PropertyAccessorGet:
		  GeneratePropertyAccessorGetStatement(statement as Get);
		  return true;
		case LanguageElementType.PropertyAccessorSet:
		  GeneratePropertyAccessorSetStatement(statement as Set);
		  return true;
		case LanguageElementType.With:
		  GenerateWithStatement(statement as With);
		  return true;
		case LanguageElementType.RaiseEvent:
		  GenerateRaiseEventStatement(statement as RaiseEvent);
		  return true;
		case LanguageElementType.YieldBreak:
		  GenerateYieldBreakStatement(statement as YieldBreak);
		  return true;
		case LanguageElementType.YieldReturn:
		  GenerateYieldReturnStatement(statement as YieldReturn);
		  return true;
		case LanguageElementType.EmptyStatement:
		  GenerateEmptyStatement(statement as EmptyStatement);
		  return true;
		case LanguageElementType.OptionStatement:
		  GenerateOptionStatement(statement as OptionStatement);
		  return true;
		case LanguageElementType.UnsafeStatement:
		  GenerateUnsafeStatement(statement as UnsafeStatement);
		  return true;
	  }
	  return false;
	}
	protected virtual void GenerateUnsafeStatement(UnsafeStatement statement)
	{
	}
	protected abstract void GenerateMethodCallStatement(MethodCall statement);
	protected abstract void GenerateAbortStatement(Abort statement);
	protected abstract void GenerateThrowStatement(Throw statement);
	protected abstract void GenerateBreakStatement(Break statement);
	protected abstract void GenerateContinueStatement(Continue statement);
	protected abstract void GenerateStatementStatement(Statement statement);
	protected abstract void GenerateGotoStatement(Goto statement);
	protected abstract void GenerateReturnStatement(Return statement);
	protected abstract void GenerateAssignmentStatement(Assignment statement);
	protected abstract void GenerateBlockStatement(Block statement);
	protected abstract void GenerateCheckedStatement(Checked statement);
	protected abstract void GenerateUncheckedStatement(Unchecked statement);
	protected abstract void GenerateSwitchStatement(Switch statement);
	protected abstract void GenerateCaseStatement(Case statement);
	protected abstract void GenerateTryStatement(Try statement);
	protected abstract void GenerateCatchStatement(Catch statement);
	protected abstract void GenerateFinallyStatement(Finally statement);
	protected abstract void GenerateFixedStatement(Fixed statement);
	protected abstract void GenerateLockStatement(Lock statement);
	protected abstract void GenerateUsingStatement(UsingStatement statement);
	protected abstract void GenerateDoStatement(Do statement);
	protected abstract void GenerateForStatement(For statement);
	protected abstract void GenerateForEachStatement(ForEach statement);
	protected abstract void GenerateWhileStatement(While statement);
	protected abstract void GenerateIfStatement(If statement);
	protected abstract void GenerateElseIfStatement(ElseIf statement);
	protected abstract void GenerateElseStatement(Else statement);
	protected abstract void GenerateEventAddStatement(EventAdd statement);
	protected abstract void GenerateEventRaiseStatement(EventRaise statement);
	protected abstract void GenerateEventRemoveStatement(EventRemove statement);
	protected abstract void GeneratePropertyAccessorGetStatement(Get statement);
	protected abstract void GeneratePropertyAccessorSetStatement(Set statement);
	protected abstract void GenerateWithStatement(With statement);
	protected abstract void GenerateRaiseEventStatement(RaiseEvent statement);
	protected abstract void GenerateYieldBreakStatement(YieldBreak statement);
	protected abstract void GenerateYieldReturnStatement(YieldReturn statement);
	protected abstract void GenerateEmptyStatement(EmptyStatement statement);
	protected abstract void GenerateConstructorInitializer(ConstructorInitializer initializer);
	protected abstract void GenerateOptionStatement(OptionStatement statement);
	public override void GenerateElement(LanguageElement languageElement)
	{
	  if (languageElement == null)
		return;
	  if (languageElement is Statement)
		GenerateStatement(languageElement as Statement);
	  else
		if (languageElement is ConstructorInitializer)
		  GenerateConstructorInitializer(languageElement as ConstructorInitializer);
	}
  }
}
