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
  using CodeStyle.Formatting;
  public class CSharpMemberCodeGen : MemberCodeGenBase
  {
	public CSharpMemberCodeGen(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	bool CanGenerateMethodVisibility(Method method)
	{
	  if (method == null)
		return false;
	  if (method.IsConstructor)
		return !method.IsStatic;
	  if (method.IsDestructor)
		return false;
	  return true;
	}
	bool CanGenerateVarType(Variable var)
	{
	  if (var == null)
		return false;
	  UsingStatement usingStatement = CodeGen.GetPrevContext() as UsingStatement;
	  if (usingStatement != null && usingStatement.Initializers != null)
		return usingStatement.Initializers.IndexOf(var) < 1;
	  return true;
	}
	string RemoveTags(string str)
	{
	  if (string.IsNullOrEmpty(str))
		return str;
	  str = str.Replace("«Caret»", "");
	  str = str.Replace("«BlockAnchor»", "");
	  return str;
	}
	void GenerateArgumentDirection(ArgumentDirection ad)
	{
	  switch (ad)
	  {
		case ArgumentDirection.In:
		  return;
		case ArgumentDirection.Out:
		  Write(FormattingTokenType.Out);
		  break;
		case ArgumentDirection.Ref:
		  Write(FormattingTokenType.Ref);
		  break;
		case ArgumentDirection.ParamArray:
		  Write(FormattingTokenType.Params);
		  break;
	  }
	}
	void GenerateBaseVariable(Variable var)
	{
	  if (var.PreviousVariable == null)
	  {
		CodeGenHelper.GenerateVisibility(this);
		if (var.IsConst)
		{
		  CodeGenHelper.GenerateAccessSpecifiers(this);
		  Write(FormattingTokenType.Const);
		}
		else
		  CodeGenHelper.GenerateAccessSpecifiers(this);
		if (var.IsFixedSizeBuffer)
		  Write(FormattingTokenType.Fixed);
		if (var.ElementType == LanguageElementType.ImplicitVariable)
		  Write(FormattingTokenType.Var);
		else if (CanGenerateVarType(var))
		  GenerateMemberType(var);
		if (var.NextVariable != null)
		{
		  bool needUpdateAlignment = Options.WrappingAlignment.AlignWithFirstMultiVariableDeclarationItem
			&& !Options.WrappingAlignment.WrapFirstMultiVariableDeclaration;
		  CodeGen.TokenGen.NeedIncreaseAlignment = needUpdateAlignment;
		}
	  }
	  Write(FormattingTokenType.Ident);
	}
	void GenerateMemberType(Member member)
	{
	  if (member == null)
		return;
	  TypeReferenceExpression lTypeRef = ParserUtils.GetElementType(member);
	  if (lTypeRef != null)
	  {
		CodeGen.GenerateElement(lTypeRef);
		if (!string.IsNullOrEmpty(member.Name))
		  CodeGen.AddWSIfNeeded();
		return;
	  }
	  if (!string.IsNullOrEmpty(member.MemberType))
	  {
		Write(member.MemberType);
		if (!string.IsNullOrEmpty(member.Name))
		  CodeGen.AddWSIfNeeded();
		return;
	  }
	  if (member is BaseVariable)
		Write(FormattingTokenType.Object);
	  else
		Write(FormattingTokenType.Void);
	}
	void GenerateMethodName()
	{
	  Method method = Context as Method;
	  if (method == null)
		return;
	  string methodName = method.Name;
	  methodName = RemoveTags(methodName);
	  if (!methodName.StartsWith("~") && method.MethodType == MethodTypeEnum.Destructor)
		Write(FormattingTokenType.Tilde);
	  Write(FormattingTokenType.Ident);
	}
	void GenerateOverloadOperatorSignature(Method method)
	{
	  if (method.IsExplicitCast)
		Write(FormattingTokenType.Explicit);
	  else if (method.IsImplicitCast)
		Write(FormattingTokenType.Implicit);
	  else
		GenerateMemberType(method);
	  Write(FormattingTokenType.Operator);
	  if (method.IsExplicitCast || method.IsImplicitCast)
		GenerateMemberType(method);
	  else
		Write(CSharpOperatorHelper.GetOperatorToken(method.OverloadsOperator));
	}
	void GenerateMethodParameters(Method method)
	{
	  if (method.IsGeneric)
		CodeGen.GenerateElement(method.GenericModifier);
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(method.Parameters, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	  if (method.IsGeneric)
		CodeGen.GenerateElement(method.GenericModifier);
	}
	void GenetateVarTail(Variable var)
	{
	  if (var.IsDetailNode)
		return;
	  if (var.NextVariable == null)
		Write(FormattingTokenType.Semicolon);
	  else
		Write(FormattingTokenType.Comma);
	}
	protected override void GenerateInitializedVolatile(InitializedVolatile var)
	{
	}
	protected override void GenerateConstVolatile(ConstVolatile member)
	{
	}
	protected override void GenerateVolatile(Volatile member)
	{
	}
	protected override void GenerateMethodPrototype(MethodPrototype member)
	{
	  return;
	}
	protected override void GenerateMethod(Method method)
	{
	  if (method.Parent == null || method.Parent.ElementType != LanguageElementType.Interface)
		if (CanGenerateMethodVisibility(method))
		  CodeGenHelper.GenerateVisibility(this);
	  CodeGenHelper.GenerateAccessSpecifiers(this);
	  MethodTypeEnum methodType = method.MethodType;
	  if (method.IsClassOperator)
		GenerateOverloadOperatorSignature(method);
	  else
	  {
		if (methodType == MethodTypeEnum.Function || methodType == MethodTypeEnum.Void)
		  GenerateMemberType(method);
		GenerateMethodName();
	  }
	  GenerateMethodParameters(method);
	  if (method.IsAbstract ||
		(method.Parent != null && method.Parent.ElementType == LanguageElementType.Interface) || !method.GenerateCodeBlock || method.IsExtern)
	  {
		Write(FormattingTokenType.Semicolon);
		return;
	  }
	  if (methodType == MethodTypeEnum.Constructor && method.HasInitializer)
	  {
		bool codeBlockContents = CodeGen.Options.Indention.CodeBlockContents;
		if (codeBlockContents)
		  Code.IncreaseIndent();
		CodeGen.GenerateElement(method.Initializer);
		if (codeBlockContents)
		  Code.DecreaseIndent();
	  }
	  Write(FormattingTokenType.CurlyBraceOpen);
	  GenerateElementCollection(method.Nodes, FormattingTokenType.None, false, LanguageElementType.ConstructorInitializer);
	  Write(FormattingTokenType.CurlyBraceClose);
	}
	protected override void GenerateEvent(Event member)
	{
	  if (member.Parent == null || member.Parent.ElementType != LanguageElementType.Interface)
	  {
		CodeGenHelper.GenerateVisibility(this);
		CodeGenHelper.GenerateAccessSpecifiers(this);
	  }
	  Write(FormattingTokenType.Event);
	  GenerateMemberType(member);
	  Write(FormattingTokenType.Ident);
	  if (member.Initializer != null)
	  {
		Write(FormattingTokenType.Equal);
		CodeGen.GenerateElement(member.Initializer);
	  }
	  if (member.Adder != null || member.Remover != null)
	  {
		if (member.IsAbstract || (member.Parent != null && member.Parent.ElementType == LanguageElementType.Interface))
		{
		  Write(FormattingTokenType.Semicolon);
		  return;
		}
		Write(FormattingTokenType.CurlyBraceOpen);
		GenerateElementCollection(member.Nodes, FormattingTokenType.None);
		Write(FormattingTokenType.CurlyBraceClose);
	  }
	  else
		Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateProperty(Property property)
	{
	  if (property.Parent == null || property.Parent.ElementType != LanguageElementType.Interface)
	  {
		CodeGenHelper.GenerateVisibility(this);
		CodeGenHelper.GenerateAccessSpecifiers(this);
	  }
	  GenerateMemberType(property);
	  if (property.IsIndexed)
	  {
		if (property.ImplementsExpressions != null)
		{
		  GenerateElementCollection(property.ImplementsExpressions, FormattingTokenType.Dot);
		  Write(FormattingTokenType.Dot);
		}
		Write(FormattingTokenType.This);
		Write(FormattingTokenType.BracketOpen);
		GenerateElementCollection(property.Parameters, FormattingTokenType.Comma);
		Write(FormattingTokenType.BracketClose);
	  }
	  else
	  {
		ExpressionCollection implementsExpressions = property.ImplementsExpressions;
		if (implementsExpressions != null && implementsExpressions.Count > 0)
		  GenerateElementCollection(implementsExpressions, FormattingTokenType.Dot);
		else
		  Write(FormattingTokenType.Ident);
	  }
	  Write(FormattingTokenType.CurlyBraceOpen);
	  if (property.IsAutoImplemented && !property.HasGetter)
	  {
		Write(FormattingTokenType.Get);
		Write(FormattingTokenType.Semicolon);
	  }
	  GenerateElementCollection(property.Nodes, FormattingTokenType.None);
	  if (property.IsAutoImplemented && !property.HasSetter)
	  {
		Write(FormattingTokenType.Set);
		Write(FormattingTokenType.Semicolon);
	  }
	  Write(FormattingTokenType.CurlyBraceClose);
	}
	protected override void GenerateDelegate(DelegateDefinition member)
	{
	  CodeGenHelper.GenerateVisibility(this);
	  CodeGenHelper.GenerateAccessSpecifiers(this);
	  Write(FormattingTokenType.Delegate);
	  GenerateMemberType(member);
	  Write(FormattingTokenType.Ident);
	  if (member.IsGeneric)
		CodeGen.GenerateElement(member.GenericModifier);
	  GenerateParameters(member.Parameters);
	  if (member.IsGeneric)
		CodeGen.GenerateElement(member.GenericModifier);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateConst(Const var)
	{
	  GenerateBaseVariable(var);
	  Write(FormattingTokenType.Equal);
	  CodeGen.GenerateElement(var.Value);
	  GenetateVarTail(var);
	}
	protected override void GenerateVariable(Variable var)
	{
	  if (var == null)
		return;
	  GenerateBaseVariable(var);
	  if (var.IsFixedSizeBuffer)
	  {
		Write(FormattingTokenType.BracketOpen);
		CodeGen.GenerateElement(var.FixedSize);
		Write(FormattingTokenType.BracketClose);
	  }
	  GenetateVarTail(var);
	}
	protected override void GenerateInitializedVariable(InitializedVariable var)
	{
	  if (var == null)
		return;
	  GenerateBaseVariable(var);
	  Write(FormattingTokenType.Equal);
	  CodeGen.GenerateElement(var.Expression);
	  GenetateVarTail(var);
	}
	protected override void GenerateParameter(Param param)
	{
	  GenerateArgumentDirection(param.Direction);
	  GenerateMemberType(param);
	  Write(FormattingTokenType.Ident);
	  if (!param.IsOptional)
		return;
	  Write(FormattingTokenType.Equal);
	  CodeGen.GenerateElement(param.DefaultValueExpression);
	}
	protected override void GenerateEnumElement(EnumElement element)
	{
	  Write(FormattingTokenType.Ident);
	  if (element.ValueExpression != null)
	  {
		Write(FormattingTokenType.Equal);
		CodeGen.GenerateElement(element.ValueExpression);
	  }
	  if (element.NextSibling == null && element.HasComma)
		Write(FormattingTokenType.Comma);
	}
	protected override void GenerateImplicitVariable(ImplicitVariable member)
	{
	  if (member == null)
		return;
	  GenerateBaseVariable(member);
	  Expression initial = member.Expression;
	  if (initial != null)
	  {
		Write(FormattingTokenType.Equal);
		CodeGen.GenerateElement(initial);
	  }
	  GenetateVarTail(member);
	}
	protected override void GenerateExtensionMethodParam(ExtensionMethodParam member)
	{
	  GenerateArgumentDirection(member.Direction);
	  Write(FormattingTokenType.This);
	  GenerateMemberType(member);
	  Write(FormattingTokenType.Ident);
	}
	protected override void GenerateLambdaImplicitlyTypedParam(LambdaImplicitlyTypedParam member)
	{
	  if (member == null)
		return;
	  Write(FormattingTokenType.Ident);
	}
	protected override void GenerateQueryIdent(QueryIdent member)
	{
	  if (member == null)
		return;
	  if (member.MemberTypeReference != null)
		CodeGen.GenerateElement(member.MemberTypeReference);
	  Write(FormattingTokenType.Ident);
	  if (member.Expression == null)
		return;
	  Write(FormattingTokenType.Equal);
	  CodeGen.GenerateElement(member.Expression);
	}
	protected override void GenerateMultiVars(Variable var)
	{
	  LanguageElementCollection coll = new LanguageElementCollection();
	  while (var != null)
	  {
		coll.Add(var);
		var = var.NextVariable;
	  }
	  GenerateElementCollection(coll, FormattingTokenType.Comma);
	}
	public override void GenerateGenericModifier(GenericModifier generic)
	{
	  if (generic == null)
		return;
	  if (ElementWasGenerated(generic))
	  {
		if (CodeGenHelper.HasTypeParameterConstraints(generic))
		  GenerateElementCollection(generic.TypeParameters, FormattingTokenType.None);
		RemoveGeneratedElement(generic);
		return;
	  }
	  Write(FormattingTokenType.LessThan);
	  GenerateElementCollection(generic.TypeParameters, FormattingTokenType.Comma);
	  Write(FormattingTokenType.GreaterThen);
	  AddGeneratedElement(generic);
	}
	public override void GenerateTypeParameter(TypeParameter parameter)
	{
	  if (parameter == null)
		return;
	  if (ElementWasGenerated(parameter))
	  {
		if (!CodeGenHelper.HasTypeParameterConstraints(parameter))
		  return;
		Write(FormattingTokenType.Where, CodeGen.Contexts.Count - 3, 0, null);
		Write(FormattingTokenType.Ident);
		Write(FormattingTokenType.Colon, CodeGen.Contexts.Count - 3, 0, null);
		GenerateElementCollection(parameter.Constraints, FormattingTokenType.Comma);
		RemoveGeneratedElement(parameter);
		return;
	  }
	  if (parameter.Direction == TypeParameterDirection.In)
		Write(FormattingTokenType.In);
	  else if (parameter.Direction == TypeParameterDirection.Out)
		Write(FormattingTokenType.Out);
	  Write(FormattingTokenType.Ident);
	  AddGeneratedElement(parameter);
	}
	public override void GenerateTypeParameterConstraint(TypeParameterConstraint constraint)
	{
	  if (constraint == null)
		return;
	  switch (constraint.ElementType)
	  {
		case LanguageElementType.ClassTypeParameterConstraint:
		  Write(FormattingTokenType.Class);
		  break;
		case LanguageElementType.StructTypeParameterConstraint:
		  Write(FormattingTokenType.Struct);
		  break;
		case LanguageElementType.NewTypeParameterConstraint:
		  Write(FormattingTokenType.New);
		  Write(FormattingTokenType.ParenOpen);
		  Write(FormattingTokenType.ParenClose);
		  break;
		case LanguageElementType.NamedTypeParameterConstraint:
		  CodeGen.GenerateElement(((NamedTypeParameterConstraint)constraint).TypeReference);
		  break;
		default:
		  Write(FormattingTokenType.Ident);
		  break;
	  }
	}
	public override void GenerateTypeParameterCollection(TypeParameterCollection collection)
	{
	  if (collection == null)
		return;
	  int lCount = collection.Count;
	  for (int i = 0; i < lCount; i++)
	  {
		CodeGen.GenerateElement(collection[i]);
		if (i < collection.Count - 1)
		  Write(FormattingTokenType.Comma);
	  }
	}
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
	public override bool IsMemberGenElement(LanguageElement element)
	{
	  if (element == null)
		return false;
	  if (element is Member || element is GenericModifier || element is TypeParameter || element is TypeParameterConstraint)
		return true;
	  return false;
	}
	public override bool GenerateElementTail(LanguageElement element)
	{
	  Member member = element as Member;
	  if (member == null || member.IsDetailNode)
		return false;
	  if (member.Parent != null)
	  {
		LanguageElementType parentType = member.Parent.ElementType;
		if (parentType == LanguageElementType.Class || parentType == LanguageElementType.Interface || parentType == LanguageElementType.Struct)
		  if (member.NextSibling != null)
		  {
			bool isMultiLineMember = member.Range.LineCount > 1;
			bool blankLinesAddAfterMultiLineMembers = Options.BlankLines.AfterMultiLineMembers && isMultiLineMember;
			bool blankLinesAddAfterSingleLineMembers = Options.BlankLines.AfterSingleLineMembers && !isMultiLineMember;
			if (blankLinesAddAfterMultiLineMembers || blankLinesAddAfterSingleLineMembers)
			{
			  CodeGen.AddNewLine(2);
			  return true;
			}
		  }
	  }
	  if (element.ElementType == LanguageElementType.EnumElement)
		return false;
	  Variable var = element as Variable;
	  if (var != null && var.NextVariable != null)
		return false;
	  LanguageElement codeSibling = element.NextSibling;
	  if (codeSibling == null)
		return true;
	  CodeGen.AddNewLineIfNeeded();
	  return true;
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Ident:
		  QueryIdent queryIdent = Context as QueryIdent;
		  if (queryIdent != null && queryIdent.MemberTypeReference != null)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Where:
		  GenericModifier parent = Context.Parent as GenericModifier;
		  if (parent == null || parent.DetailNodeCount == 0)
			break;
		  bool isFirst = parent.DetailNodes.IndexOf(Context) == 0;
		  if (isFirst)
		  {
			if (!Options.WrappingAlignment.WrapFirstTypeParameterConstraint)
			  result.AddWhiteSpace();
		  }
		  else
			if (!Options.WrappingAlignment.WrapTypeParameterConstraints)
			  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.BracketOpen:
		  if (Options.Spacing.BeforeOpenSquareBracket)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Comma:
		  if (Options.Spacing.BeforeComma)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.LessThan:
		  if (Options.Spacing.BeforeTypeParameterAngles)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.GreaterThen:
		  if (Options.Spacing.WithinTypeParameterAngles)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Colon:
		  if (Context is TypeParameter && Options.Spacing.BeforeTypeParameterConstraintColon)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Base:
		case FormattingTokenType.This:
		  if (ContextMatch(LanguageElementType.ConstructorInitializer) && !Options.LineBreaks.PlaceConstructorInitializerOnSameLine)
			result.AddNewLine();
		  break;
		case FormattingTokenType.ParenOpen:
		  if (ContextMatch(LanguageElementType.Method))
		  {
			if (Options.WrappingAlignment.WrapBeforeOpenBraceInDeclaration)
			{
			  result.AddNewLine();
			  IncreaseIndent();
			}
			else
			{
			  bool hasParams = ((Method)Context).ParameterCount > 0;
			  if ((Options.Spacing.BeforeOpeningParenthesisOfAMethodWithParameters && hasParams) ||
				(Options.Spacing.BeforeOpeningParenthesisOfAMethodWithoutParameters && !hasParams))
				result.AddWhiteSpace();
			}
		  }
		  break;
		case FormattingTokenType.ParenClose:
		  if (ContextMatch(LanguageElementType.Method))
		  {
			if (Options.WrappingAlignment.WrapBeforeCloseBraceInDeclaration)
			  result.AddNewLine();
			else
			{
			  bool hasParams = ((Method)Context).ParameterCount > 0;
			  if (Options.Spacing.WithinMethodDeclarationParentheses && hasParams)
				result.AddWhiteSpace();
			}
		  }
		  break;
	  }
	  return result;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Void:
		case FormattingTokenType.Object:
		case FormattingTokenType.Abstract:
		case FormattingTokenType.Extern:
		case FormattingTokenType.Override:
		case FormattingTokenType.ReadOnly:
		case FormattingTokenType.Sealed:
		case FormattingTokenType.Static:
		case FormattingTokenType.Unsafe:
		case FormattingTokenType.Virtual:
		case FormattingTokenType.Volatile:
		case FormattingTokenType.In:
		case FormattingTokenType.Out:
		case FormattingTokenType.Ref:
		case FormattingTokenType.Params:
		case FormattingTokenType.Fixed:
		case FormattingTokenType.Event:
		case FormattingTokenType.Delegate:
		case FormattingTokenType.Where:
		case FormattingTokenType.Var:
		case FormattingTokenType.Implicit:
		case FormattingTokenType.Explicit:
		case FormattingTokenType.Operator:
		case FormattingTokenType.Partial:
		case FormattingTokenType.Async:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.LessThan:
		  if (Options.Spacing.WithinTypeParameterAngles)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.New:
		  if (!ContextMatch(LanguageElementType.NewTypeParameterConstraint))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Colon:
		  if (Context is TypeParameter && Options.Spacing.AfterTypeParameterConstraintColon)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.This:
		  if (Context.ElementType == LanguageElementType.ExtensionMethodParam)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenOpen:
		  if (ContextMatch(LanguageElementType.Method))
		  {
			bool hasParams = ((Method)Context).ParameterCount > 0;
			if ((Options.Spacing.WithinMethodDeclarationParentheses && hasParams) ||
			  (Options.Spacing.WithinEmptyMethodDeclarationParentheses && !hasParams))
			  result.AddWhiteSpace();
		  }
		  break;
		case FormattingTokenType.ParenClose:
		  if (ContextMatch(LanguageElementType.Method) && Options.WrappingAlignment.WrapBeforeOpenBraceInDeclaration)
			Code.DecreaseIndent();
		  break;
		case FormattingTokenType.Const:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
  }
}
