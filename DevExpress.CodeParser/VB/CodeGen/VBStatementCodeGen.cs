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
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif 
{
  using ContinueVB = VB.VBContinue;
  using Exit = VB.ExitStatement;
  using System.Collections.Generic;
	public class VBStatementCodeGen : StatementCodeGenBase
	{
		#region VBStatementCodeGen
		public VBStatementCodeGen(CodeGen codeGen) : base(codeGen) 
		{
		}
		#endregion
	FormattingTokenType GetExitTarget(Exit statement)
	{
	  if (statement == null)
		return FormattingTokenType.None;
	  ExitKind kind = statement.ExitKind;
	  string resultStr = String.Empty;
	  switch (kind)
	  {
		case ExitKind.Do:
		  return FormattingTokenType.Do;
		case ExitKind.For:
		  return FormattingTokenType.For;
		case ExitKind.Function:
		  return FormattingTokenType.Function;
		case ExitKind.Property:
		  return FormattingTokenType.Property;
		case ExitKind.Select:
		  return FormattingTokenType.Select;
		case ExitKind.Sub:
		  return FormattingTokenType.Sub;
		case ExitKind.Try:
		  return FormattingTokenType.Try;
		case ExitKind.While:
		  return FormattingTokenType.While;
	  }
	  return FormattingTokenType.None;
	}
	FormattingTokenType GetOptionType(OptionType type)
	{
	  string returnString = String.Empty;
	  switch (type)
	  {
		case OptionType.Compare:
		  return FormattingTokenType.Compare;
		case OptionType.Explicit:
		  return FormattingTokenType.Explicit;
		case OptionType.Strict:
		  return FormattingTokenType.Strict;
		case OptionType.Infer:
		  return FormattingTokenType.Infer;
	  }
	  return FormattingTokenType.None;
	}
	FormattingTokenType GetOptionState(OptionState state)
	{
	  switch (state)
	  {
		case OptionState.On:
		  return FormattingTokenType.On;
		case OptionState.Off:
		  return FormattingTokenType.Off;
		case OptionState.Binary:
		  return FormattingTokenType.Binary;
		case OptionState.Text:
		  return FormattingTokenType.Text;
	  }
	  return FormattingTokenType.None;
	}
	void GenerateStatementEnd(FormattingTokenType keyword)
	{
	  CodeGen.AddNewLineIfNeeded();
	  Write(FormattingTokenType.End, keyword);
	}
	void CloseTryStatementIfNeeded(Statement statement)
	{
	  LanguageElement element = statement.NextCodeSibling;
	  if (element != null && (element is Catch || element is Finally))
	  {
		CodeGen.AddNewLineIfNeeded();
		return;
	  }
	  Write(FormattingTokenType.End, FormattingTokenType.Try);
	}
	void GenerateDoCondition(Do statement)
	{
	  bool isPrecondition = statement.PreCondition;
	  if (!isPrecondition)
		Write(FormattingTokenType.Loop);
	  DoConditionType type = statement.ConditionType;
	  if (type == DoConditionType.Until)
		Write(FormattingTokenType.Until);
	  else
		Write(FormattingTokenType.While);
	  CodeGen.GenerateElement(statement.Condition);
	}
	void CloseIfStatementIfNeeded(Statement statement)
	{
	  IfElse ifElse = statement as IfElse;
	  if (ifElse != null && ifElse.IsOneLine)
		return;
	  LanguageElement element = statement.NextCodeSibling;
	  if (element != null && element is Else || element is ElseIf)
		return;
	  CodeGen.AddNewLineIfNeeded();
	  Write(FormattingTokenType.End);
	  CodeGen.AddWSIfNeeded();
	  Write(FormattingTokenType.If);
	}
	bool GenerateLastComments(List<Comment> lastComments)
	{
	  if (lastComments == null || lastComments.Count == 0)
		return false;
	  CodeGen.AddWSIfNeeded();
	  foreach (Comment lastComment in lastComments)
		CodeGen.GenerateElement(lastComment);
	  return true;
	}
	void GenerateIfParentingStatement(IfElse statement, FormattingTokenType keyword)
	{
	  Write(keyword);
	  List<Comment> lastComments = GenerateWithoutLastComment(statement.DetailNodes);
	  if (keyword != FormattingTokenType.Else)
		Write(FormattingTokenType.Then);
	  if (GenerateLastComments(lastComments) || !(statement is IfElse && (statement.IsOneLine || statement.NodeCount < 1)))
		CodeGen.AddNewLineIfNeeded();
	  else
		CodeGen.AddWSIfNeeded();
	  using(GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  CloseIfStatementIfNeeded(statement);
	}
	void GenerateEventAccessor(EventAccessor statement, FormattingTokenType accessorName)
	{
	  CodeGen.AddNewLineIfNeeded();
	  Write(accessorName);
	  GenerateParameters(statement.Parameters);
	  using (GetIndent())
	  {
		CodeGen.AddNewLineIfNeeded();
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  }
	  Write(FormattingTokenType.End, accessorName);
	}
	void GeneratePropertyAccessorStatement(PropertyAccessor statement, FormattingTokenType keyword)
	{
	  if(!statement.GenerateCodeBlock)
		return;
	  Property property = statement.Parent as Property;
	  if (property != null && property.IsAbstract)
		return;
	  if (property == null || property.Visibility != statement.Visibility)
		((VBMemberCodeGen)CodeGen.MemberGen).GenerateMemberVisibility(statement.Visibility);
	  Write(keyword);
	  if (statement.DetailNodeCount > 0)
		GenerateParameters(statement.DetailNodes);
	  CodeGen.AddNewLineIfNeeded();
	  using(GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  GenerateStatementEnd(keyword);
	}
	List<Comment> GenerateWithoutLastComment(NodeList coll)
	{
	  if (coll == null)
		return null;
	  int lastIndex = coll.Count - 1;
	  if (lastIndex < 0)
		return null;
	  List<Comment> result = new List<Comment>();
	  int lastElementIndex = 0;
	  for (int i = lastIndex; i >= 0; i--)
	  {
		Comment comment = coll[i] as Comment;
		if (comment == null)
		{
		  lastElementIndex = i;
		  break;
		}
		result.Add(comment);
	  }
	  NodeList nodeForGen = new NodeList();
	  for (int i = 0; i < lastElementIndex; i++)
		nodeForGen.Add(coll[i] as LanguageElement);
	  BaseElement lastElementClone = coll[lastElementIndex] as BaseElement;
	  lastElementClone = lastElementClone.Clone();
	  nodeForGen.Add(lastElementClone);
	  GenerateElementCollection(nodeForGen);
	  return result;
	}
	protected void GenerateEndStatement(End statement)
	{
	  Write(FormattingTokenType.End);
	}
	protected void GenerateCallStatement(CallStatement statement)
	{
	  Write(FormattingTokenType.Call);
	  CodeGen.GenerateElement(statement.CalledExpression);
	}
		protected void GenerateEraseStatement(Erase statement)
		{
			if (statement == null)
				return;
			Write(FormattingTokenType.Erase); 
			GenerateElementCollection(statement.DetailNodes, FormattingTokenType.Comma);
		}
		protected void GenerateResumeStatement(Resume statement)
		{
			if (statement == null)
				return;
			Write(FormattingTokenType.Resume);
			string lable = statement.Label;
			if (lable != null || lable != String.Empty)
				Write(lable);
			if (statement.HasNextClause)
				Write(FormattingTokenType.Next);
		}
		protected void GenerateExitStatement(Exit statement)
		{
			if (statement == null)
				return;
			FormattingTokenType exitTarget = GetExitTarget(statement);
	  Write(FormattingTokenType.Exit, exitTarget);
		}
		protected void GenerateAddHandlerStatement(AddHandler statement)
		{
			if (statement == null)
				return;
			Write(FormattingTokenType.AddHandler);
			CodeGen.GenerateElement(statement.Expression);
			Write(FormattingTokenType.Comma);
			CodeGen.GenerateElement(statement.AddressExpression);
		}
		protected void GenerateRemoveHandlerStatement(RemoveHandler statement)
		{
			if (statement == null)
				return;
			Write(FormattingTokenType.RemoveHandler);
			CodeGen.GenerateElement(statement.Expression);
			Write(FormattingTokenType.Comma);
			CodeGen.GenerateElement(statement.AddressExpression);
		}
		protected void GenerateOnErrorStatement(OnError statement)
		{
			if (statement == null)
				return;
			Write(FormattingTokenType.On, FormattingTokenType.Error);
			Write(statement.Statement);
		}
		protected void GenerateLabelStatement(Label statement)
		{
			if (statement == null)
				return;
	  using(GetClearIndent(!Options.Indention.IndentLabels))
			  Write(FormattingTokenType.Ident, FormattingTokenType.Colon);
		}
		protected virtual void GenerateStopStatement(Stop statement)
		{
	  Write(FormattingTokenType.Stop);
		}
		protected virtual void GenerateReDimStatement(ReDim statement)
		{
			if (statement == null)
				return;
			Write(FormattingTokenType.ReDim);
			if (statement.HasPreserve)
				Write(FormattingTokenType.Preserve);
			GenerateElementCollection(statement.Expressions, FormattingTokenType.Comma);
		}
		protected override void GenerateEventRaiseStatement(EventRaise statement)
		{
	  GenerateEventAccessor(statement, FormattingTokenType.RaiseEvent);
		}
	protected override void GenerateOptionStatement(OptionStatement statement)
	{
	  if (statement == null)
		return;
	  Write(FormattingTokenType.Option);
	  Write(GetOptionType(statement.Type));
	  Write(GetOptionState(statement.State));
	}
	protected override bool GenerateStatement(Statement statement)
	{
	  if (statement == null)
		return false;
	  if (base.GenerateStatement(statement))
		return true;
	  switch (statement.ElementType)
	  {
		case LanguageElementType.ReDim:
		  GenerateReDimStatement(statement as ReDim);
		  return true;
		case LanguageElementType.Stop:
		  GenerateStopStatement(statement as Stop);
		  return true;
		case LanguageElementType.OnError:
		  GenerateOnErrorStatement(statement as OnError);
		  return true;
		case LanguageElementType.Label:
		  GenerateLabelStatement(statement as Label);
		  return true;
		case LanguageElementType.AddHandler:
		  GenerateAddHandlerStatement(statement as AddHandler);
		  return true;
		case LanguageElementType.RemoveHandler:
		  GenerateRemoveHandlerStatement(statement as RemoveHandler);
		  return true;
		case LanguageElementType.Exit:
		  GenerateExitStatement(statement as Exit);
		  return true;
		case LanguageElementType.End:
		  GenerateEndStatement(statement as End);
		  return true;
		case LanguageElementType.Resume:
		  GenerateResumeStatement(statement as Resume);
		  return true;
		case LanguageElementType.Erase:
		  GenerateEraseStatement(statement as Erase);
		  return true;
		case LanguageElementType.CallStatement:
		  GenerateCallStatement(statement as CallStatement);
		  return true;
	  }
	  return false;
	}
		protected override void GenerateMethodCallStatement(MethodCall statement) 
		{
			CodeGen.GenerateElement(statement.Qualifier);
			GenerateParameters(statement.Arguments);
		}
		protected override void GenerateAbortStatement(Abort statement) 
		{
		}
		protected override void GenerateThrowStatement(Throw statement) 
		{
			Write(FormattingTokenType.Throw);
			CodeGen.GenerateElement(statement.Expression);
		}
		protected override void GenerateBreakStatement(Break statement) 
		{
	  Write(FormattingTokenType.Exit);
			LanguageElement parent = statement.Parent;
	  if (parent is VBFor)
		Write(FormattingTokenType.For);
	  else if (parent is While)
		Write(FormattingTokenType.While);
	  else if (parent is Do)
		Write(FormattingTokenType.Do);
	  else if (parent is Case)
		Write(FormattingTokenType.Select);
		}
	protected override void GenerateContinueStatement(Continue statement) 
		{
			VBContinue continueVb = statement as ContinueVB;
			if (statement == null)
				return;
			Write(FormattingTokenType.Continue);
	  ContinueKind kind = continueVb.ContinueKind;			
			switch (kind)
			{
				case ContinueKind.Do:
		  Write(FormattingTokenType.Do);
		  break;
				case ContinueKind.For:
		  Write(FormattingTokenType.For);
		  break;
				case ContinueKind.While:
		  Write(FormattingTokenType.While);
		  break;
			}
		}
		protected override void GenerateStatementStatement(Statement statement) 
		{
	  if (statement.FirstDetail is AwaitExpression)
		CodeGen.GenerateElement(statement.FirstDetail);
	  else
		Write(FormattingTokenType.Ident);
		}
	protected override void GenerateGotoStatement(Goto statement)
	{
	  Write(FormattingTokenType.GoTo);
	  Write(statement.Label);
	}
		protected override void GenerateReturnStatement(Return statement) 
		{
			Write(FormattingTokenType.Return); 
			CodeGen.GenerateElement(statement.Expression);
		}
		protected override void GenerateAssignmentStatement(Assignment statement) 
		{
			CodeGen.GenerateElement(statement.LeftSide);
			if (statement.AssignmentOperator == AssignmentOperatorType.PercentEquals) 
			{
				VBExpressionCodeGen.GenerateAssignmentOperatorText(CodeGen, AssignmentOperatorType.Assignment);
				CodeGen.GenerateElement(statement.LeftSide);
		Write(FormattingTokenType.Mod);
			}
			else
				VBExpressionCodeGen.GenerateAssignmentOperatorText(CodeGen, statement.AssignmentOperator);
			CodeGen.GenerateElement(statement.Expression);
		}
		protected override void GenerateBlockStatement(Block statement) 
		{
	  GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
		}
		protected override void GenerateCheckedStatement(Checked statement) 
		{
		}
		protected override void GenerateUncheckedStatement(Unchecked statement) 
		{
		}
		protected override void GenerateSwitchStatement(Switch statement) 
		{
	  Write(FormattingTokenType.Select, FormattingTokenType.Case);
	  GenerateElementCollection(statement.DetailNodes, FormattingTokenType.Comma);
	  CodeGen.AddNewLineIfNeeded();
	  using(GetIndent(Options.Indention.CaseStatementFromSwitchStatement))
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  GenerateStatementEnd(FormattingTokenType.Select);
		}
		protected override void GenerateCaseStatement(Case statement) 
		{
			if (statement.IsDefault)
				Write(FormattingTokenType.Case, FormattingTokenType.Else);
			else 
			{
				Write(FormattingTokenType.Case);
				if (statement.Expression != null)
					CodeGen.GenerateElement(statement.Expression);
				else
		  GenerateElementCollection(statement.CaseClauses.Clauses, FormattingTokenType.Comma);
			}
			CodeGen.AddNewLineIfNeeded();
	  using(GetIndent(Options.Indention.CaseStatementContents))
			  GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  CodeGen.AddNewLineIfNeeded();
	}
		protected override void GenerateTryStatement(Try statement) 
		{
			Write(FormattingTokenType.Try);
	  using (GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
			CloseTryStatementIfNeeded(statement);
		}
	protected override void GenerateCatchStatement(Catch statement)
	{
	  Write(FormattingTokenType.Catch);
	  CodeGen.GenerateElement(statement.Exception);
	  LanguageElement cond = statement.Condition;
	  if (cond != null)
	  {
		Write(FormattingTokenType.When);
		CodeGen.GenerateElement(cond);
	  }
	  CodeGen.AddNewLineIfNeeded();
	  using(GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  CloseTryStatementIfNeeded(statement);
	}
		protected override void GenerateFinallyStatement(Finally statement) 
		{
			Write(FormattingTokenType.Finally);
	  CodeGen.AddNewLineIfNeeded();
	  using(GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
			CloseTryStatementIfNeeded(statement);
		}
		protected override void GenerateFixedStatement(Fixed statement) 
		{
		}
		protected override void GenerateLockStatement(Lock statement) 
		{
	  Write(FormattingTokenType.SyncLock);
	  GenerateElementCollection(statement.DetailNodes, FormattingTokenType.Comma);
	  CodeGen.AddNewLineIfNeeded();
	  using(GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  GenerateStatementEnd(FormattingTokenType.SyncLock);
		}
		protected override void GenerateUsingStatement(UsingStatement statement) 
		{
			if (statement == null)
				return;
			Write(FormattingTokenType.Using);
			GenerateElementCollection(statement.Initializers, FormattingTokenType.Comma);
	  CodeGen.AddNewLineIfNeeded();
	  using(GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
			GenerateStatementEnd(FormattingTokenType.Using);
		}
		protected override void GenerateDoStatement(Do statement) 
		{
			if (statement == null)
				return;
			Write(FormattingTokenType.Do);
			bool isPrecondition = statement.PreCondition;
	  if (isPrecondition)
		GenerateDoCondition(statement);
	  CodeGen.AddNewLineIfNeeded();
	  using(GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  if (isPrecondition)
		Write(FormattingTokenType.Loop);
	  else
			  GenerateDoCondition(statement);
		}
	protected override void GenerateForStatement(For statement)
	{
	  if (statement == null)
		return;
	  bool isSimpleFor = false;
	  VB.VBFor basicFor = null;
	  Expression condition = statement.Condition;
	  LanguageElementList initList = statement.Initializers;
	  LanguageElementList incrementList = statement.Incrementors;
	  if (((initList != null && initList.Count == 1) && (incrementList == null || incrementList.Count <= 1)) ||
		(condition == null || ((condition.Nodes == null || condition.NodeCount == 0) &&
		(condition.DetailNodes == null || condition.DetailNodeCount == 0))))
	  {
		isSimpleFor = true;
		basicFor = statement as VB.VBFor;
	  }
	  if (isSimpleFor && basicFor != null)
	  {
		Write(FormattingTokenType.For);
		CodeElement elem = initList[0] as CodeElement;
		CodeGen.GenerateElement(elem);
		Write(FormattingTokenType.To);
		elem = basicFor.ToExpression;
		if (elem != null)
		  CodeGen.GenerateElement(elem);
		elem = basicFor.StepExpression;
		if (elem != null)
		{
		  Write(FormattingTokenType.Step); 
		  CodeGen.GenerateElement(elem);
		}
		CodeGen.AddNewLineIfNeeded();
		using (GetIndent())
		  GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
		Write(FormattingTokenType.Next);
	  }
	  else
	  {
		bool oldCheck = VBCodeGen.CheckForDetailNodes;
		VBCodeGen.CheckForDetailNodes = false;
		GenerateElementCollection(initList);
		VBCodeGen.CheckForDetailNodes = oldCheck;
		CodeGen.AddNewLineIfNeeded();
		Write(FormattingTokenType.While);
		CodeGen.GenerateElement(condition);
		CodeGen.AddNewLineIfNeeded();
		using (GetIndent())
		{
		  GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
		  GenerateElementCollection(incrementList);
		}
		GenerateStatementEnd(FormattingTokenType.While);
	  }
	}
	protected override void GenerateForEachStatement(ForEach statement) 
		{
			Write(FormattingTokenType.For, FormattingTokenType.Each); 
	  LanguageElement variable = null;
	  LanguageElement collection = null;
	  int detailNodeCount = statement.DetailNodeCount;
	  if (detailNodeCount > 0)
		variable = (LanguageElement)statement.DetailNodes[0];
	  if (detailNodeCount > 1)
		collection = ((LanguageElement)statement.DetailNodes[1]);
	  CodeGen.GenerateElement(variable);
			Write(FormattingTokenType.In);
	  CodeGen.GenerateElement(collection);
	  CodeGen.AddNewLineIfNeeded();
	  using(GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  Write(FormattingTokenType.Next);
		}
	protected override void GenerateWhileStatement(While statement)
	{
	  Write(FormattingTokenType.While);
	  GenerateElementCollection(statement.DetailNodes);
	  CodeGen.AddNewLineIfNeeded();
	  using (GetIndent())
		GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  GenerateStatementEnd(FormattingTokenType.While);
	}
		protected override void GenerateIfStatement(If statement) 
		{
			GenerateIfParentingStatement(statement, FormattingTokenType.If);
		}
		protected override void GenerateElseIfStatement(ElseIf statement)
		{
			GenerateIfParentingStatement(statement, FormattingTokenType.ElseIf);
		}
		protected override void GenerateElseStatement(Else statement) 
		{
	  GenerateIfParentingStatement(statement, FormattingTokenType.Else);
		}
		protected override void GenerateEventAddStatement(EventAdd statement) 
		{
	  GenerateEventAccessor(statement, FormattingTokenType.AddHandler);
		}
	protected override void GenerateEventRemoveStatement(EventRemove statement) 
		{
	  GenerateEventAccessor(statement, FormattingTokenType.RemoveHandler);
		}
		protected override void GeneratePropertyAccessorGetStatement(Get statement) 
		{
			GeneratePropertyAccessorStatement(statement, FormattingTokenType.Get);
		}
		protected override void GeneratePropertyAccessorSetStatement(Set statement) 
		{
			GeneratePropertyAccessorStatement(statement, FormattingTokenType.Set);
		}
		protected override void GenerateWithStatement(With statement)
		{
			Write(FormattingTokenType.With);
			CodeGen.GenerateElement(statement.Expression);
	  CodeGen.AddNewLineIfNeeded();
	  GenerateElementCollection(statement.Nodes, FormattingTokenType.None, true);
	  GenerateStatementEnd(FormattingTokenType.With);
		}
		protected override void GenerateRaiseEventStatement(RaiseEvent statement)
		{
			Write(FormattingTokenType.RaiseEvent);
			CodeGen.GenerateElement(statement.Expression);
		}
		protected override void GenerateYieldBreakStatement(YieldBreak statement)
		{
		}
		protected override void GenerateYieldReturnStatement(YieldReturn statement)
		{
		}
		protected override void GenerateEmptyStatement(EmptyStatement statement)
		{
	  GenerateElementCollection(statement.Nodes);
		}
		protected override void GenerateConstructorInitializer(ConstructorInitializer initializer)
		{
			if (initializer.IsBase)
				Write(FormattingTokenType.MyBase);
			else
				Write(FormattingTokenType.MyClass);
	  Write(FormattingTokenType.Dot, FormattingTokenType.New);
			GenerateParameters(initializer.Arguments);
		}
		protected VBCodeGen VBCodeGen
		{
			get
			{
				return (VBCodeGen)CodeGen;
			}
		}
	public void GenerateCaseClause(CaseClause clause)
	{
	  if (clause == null)
		return;
	  if (clause.IsComparisonClause)
	  {
		Write(FormattingTokenType.Is);
		CodeGen.GenerateElement(clause.StartExpression);
	  }
	  else if (clause.IsRangeCheckClause)
	  {
		CodeGen.GenerateElement(clause.StartExpression);
		Write(FormattingTokenType.To);
		CodeGen.GenerateElement(clause.EndExpression);
	  }
	  else
		CodeGen.GenerateElement(clause.StartExpression);
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.If:
		case FormattingTokenType.ElseIf:
		case FormattingTokenType.Else:
		case FormattingTokenType.Each:
		case FormattingTokenType.In:
		case FormattingTokenType.End:
		case FormattingTokenType.RaiseEvent:
		case FormattingTokenType.Select:
		case FormattingTokenType.Case:
		case FormattingTokenType.Catch:
		case FormattingTokenType.For:
		case FormattingTokenType.Throw:
		case FormattingTokenType.Call:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Then:
		case FormattingTokenType.In:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ElseIf:
			ElseIf elseIf = Context as ElseIf;
			if (elseIf != null)
			{
			  ElementList list = elseIf.Parent as ElementList;
			  if (list == null || (list.NodeCount > 0 && list.Nodes[0] != elseIf))
			  {
				if (!elseIf.IsOneLine)
				  result.AddNewLine();
				else
				  result.AddWhiteSpace();
			  }
			}
		  break;
		case FormattingTokenType.Next:
		  result.AddNewLine();
		  break;
	  }
	  return result;
	}
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (Options.BlankLines.AfterFileOptionsSection)
	  {
		OptionStatement statement = element as OptionStatement;
		if (statement != null)
		{
		  if (!(statement.NextCodeSibling is OptionStatement))
			CodeGen.AddNewLine(2);
		  return true;
		}
	  }
	  return base.GenerateElementTail(element);
	}
  }
}
