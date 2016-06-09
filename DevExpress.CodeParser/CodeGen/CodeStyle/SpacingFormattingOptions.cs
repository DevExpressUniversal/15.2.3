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
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  public class SpacingFormattingOptions : FormattingOptions
  {
	bool _BeforeOpeningParenthesisOfAMethodWithParameters;
	bool _BeforeOpeningParenthesisOfAMethodWithoutParameters;
	bool _WithinMethodDeclarationParentheses;
	bool _WithinEmptyMethodDeclarationParentheses;
	bool _BeforeOpeningParenthesisOfAMethodCallWithArguments;
	bool _BeforeOpeningParenthesisOfAMethodCallWithoutArguments;
	bool _WithinArgumentsListParentheses;
	bool _WithinEmptyArgumentsListParentheses;
	bool _BeforeTypeParameterAngles;
	bool _WithinTypeParameterAngles;
	bool _BeforeTypeArgumentAngles;
	bool _WithinTypeArgumentAngles;
	bool _BeforeTypeParameterConstraintColon;
	bool _AfterTypeParameterConstraintColon;
	bool _BeforeTypeParameterParenthesis;
	bool _WithinTypeParameterParenthesis;
	bool _BeforeTypeArgumentParenthesis;
	bool _WithinTypeArgumentParenthesis;
	bool _AroundOneCharOperators;
	bool _AroundTwoCharOperators;
	bool _AroundUnsafeOperators;
	bool _AroundTernaryQuestionOperator;
	bool _AroundTernaryColonOperator;
	bool _AroundLambdaExpressionOperator;
	bool _BeforeUnsafePointerDeclarationOperator;
	bool _BeforeNullableTypeOperator;
	bool _BeforeOpenSquareBracket;
	bool _WithinSquareBrackets;
	bool _WithinEmptySquareBrackets;
	bool _WithinExpressionParentheses;
	bool _WithinTypeCastParentheses;
	bool _AfterTypeCastParentheses;
	bool _WithinSingleLineAccessor;
	bool _WithinSingleLineMethod;
	bool _WithinSingleLineAnonymousMethod;
	bool _AroundEqualsInNamespaceAliasDeclaration;
	bool _BeforeOpenCurlyBraceOnTheSameLine;
	bool _BeforeComma;
	bool _AfterComma;
	bool _BeforeDot;
	bool _AfterDot;
	bool _BeforeColon;
	bool _AfterColon;
	bool _AroundColonStatement;
	bool _BeforeColonInLabels;
	bool _AfterColonForAncestorsListInTypeDeclaration;
	bool _BeforeColonForAncestorsListInTypeDeclaration;
	bool _AfterSemicolonInForStatement;
	bool _BeforeSemicolonInForStatement;
	bool _BeforeSemicolon;
	bool _AfterSemicolon;
	bool _BeforeColonInCaseStatement;
	bool _WithinAttributeBrackets;
	bool _BeforeAttributeTargetColon;
	bool _AfterAttributeTargetColon;
	bool _BeforeArrayRankBrackets;
	bool _WithinArrayRankBrackets;
	bool _WithinArrayRankEmptyBrackets;
	bool _BeforeArrayRankParentheses;
	bool _WithinArrayRankParentheses;
	bool _WithinArrayRankEmptyParentheses;
	bool _BeforeIfParentheses;
	bool _BeforeForForeachParentheses;
	bool _BeforeWhileParentheses;
	bool _BeforeSwitchParentheses;
	bool _BeforeCatchParentheses;
	bool _BeforeUsingLockParentheses;
	bool _BeforeTypeofSizeofParentheses;
	bool _BeforeFixedParentheses;
	bool _BeforeCheckedUncheckedParentheses;
	bool _BeforeDefaultParentheses;
	bool _WithinIfParentheses;
	bool _WithinForForeachParentheses;
	bool _WithinWhileParentheses;
	bool _WithinSwitchParentheses;
	bool _WithinCatchParentheses;
	bool _WithinUsingLockParentheses;
	bool _WithinTypeofSizeofParentheses;
	bool _WithinFixedParentheses;
	bool _WithinCheckedUncheckedParentheses;
	bool _WithinDefaultParentheses;
	bool _WithinArrayInitializerBraces;
	bool _BeforeOpeningBraceInProperty;
	bool _WithinBracesInPropertyDeclaration;
	bool _WithinBracesInPropertyAccessors;
	bool _BeforeOpeningBraceInAccessor;
	public override void Load(FormattingRuleCollection rules)
	{
	  _BeforeOpeningParenthesisOfAMethodWithParameters = GetOptionByName(Rules.Spacing.Method.BeforeOpeningParenthesisOfAMethodWithParameters, rules);
	  _BeforeOpeningParenthesisOfAMethodWithoutParameters = GetOptionByName(Rules.Spacing.Method.BeforeOpeningParenthesisOfAMethodWithoutParameters, rules);
	  _WithinMethodDeclarationParentheses = GetOptionByName(Rules.Spacing.Method.WithinMethodDeclarationParentheses, rules);
	  _WithinEmptyMethodDeclarationParentheses = GetOptionByName(Rules.Spacing.Method.WithinEmptyMethodDeclarationParentheses, rules);
	  _BeforeOpeningParenthesisOfAMethodCallWithArguments = GetOptionByName(Rules.Spacing.MethodCall.BeforeOpeningParenthesisOfAMethodCallWithArguments, rules);
	  _BeforeOpeningParenthesisOfAMethodCallWithoutArguments = GetOptionByName(Rules.Spacing.MethodCall.BeforeOpeningParenthesisOfAMethodCallWithoutArguments, rules);
	  _WithinArgumentsListParentheses = GetOptionByName(Rules.Spacing.MethodCall.WithinArgumentsListParentheses, rules);
	  _WithinEmptyArgumentsListParentheses = GetOptionByName(Rules.Spacing.MethodCall.WithinEmptyArgumentsListParentheses, rules);
	  _BeforeTypeParameterAngles = GetOptionByName(Rules.Spacing.TypeParameters.BeforeTypeParameterAngles, rules);
	  _WithinTypeParameterAngles = GetOptionByName(Rules.Spacing.TypeParameters.WithinTypeParameterAngles, rules);
	  _BeforeTypeArgumentAngles = GetOptionByName(Rules.Spacing.TypeArguments.BeforeTypeArgumentAngles, rules);
	  _WithinTypeArgumentAngles = GetOptionByName(Rules.Spacing.TypeArguments.WithinTypeArgumentAngles, rules);
	  _BeforeTypeParameterConstraintColon = GetOptionByName(Rules.Spacing.TypeParameters.BeforeTypeParameterConstraintColon, rules);
	  _AfterTypeParameterConstraintColon = GetOptionByName(Rules.Spacing.TypeParameters.AfterTypeParameterConstraintColon, rules);
	  _BeforeTypeParameterParenthesis = GetOptionByName(Rules.Spacing.TypeParameters.BeforeTypeParameterParenthesis, rules);
	  _WithinTypeParameterParenthesis = GetOptionByName(Rules.Spacing.TypeParameters.WithinTypeParameterParenthesis, rules);
	  _BeforeTypeArgumentParenthesis = GetOptionByName(Rules.Spacing.TypeArguments.BeforeTypeArgumentParenthesis, rules);
	  _WithinTypeArgumentParenthesis = GetOptionByName(Rules.Spacing.TypeArguments.WithinTypeArgumentParenthesis, rules);
	  _AroundOneCharOperators = GetOptionByName(Rules.Spacing.Operators.AroundOneCharOperators, rules);
	  _AroundTwoCharOperators = GetOptionByName(Rules.Spacing.Operators.AroundTwoCharOperators, rules);
	  _AroundUnsafeOperators = GetOptionByName(Rules.Spacing.Operators.AroundUnsafeOperators, rules);
	  _AroundTernaryQuestionOperator = GetOptionByName(Rules.Spacing.Operators.AroundTernaryQuestionOperator, rules);
	  _AroundTernaryColonOperator = GetOptionByName(Rules.Spacing.Operators.AroundTernaryColonOperator, rules);
	  _AroundLambdaExpressionOperator = GetOptionByName(Rules.Spacing.Operators.AroundLambdaExpressionOperator, rules);
	  _BeforeUnsafePointerDeclarationOperator = GetOptionByName(Rules.Spacing.Operators.BeforeUnsafePointerDeclarationOperator, rules);
	  _BeforeNullableTypeOperator = GetOptionByName(Rules.Spacing.Operators.BeforeNullableTypeOperator, rules);
	  _BeforeOpenSquareBracket = GetOptionByName(Rules.Spacing.SquareBracket.BeforeOpenSquareBracket, rules);
	  _WithinSquareBrackets = GetOptionByName(Rules.Spacing.SquareBracket.WithinSquareBrackets, rules);
	  _WithinEmptySquareBrackets = GetOptionByName(Rules.Spacing.SquareBracket.WithinEmptySquareBrackets, rules);
	  _WithinExpressionParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.ExpressionParentheses, rules);
	  _WithinTypeCastParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.TypeCastParentheses, rules);
	  _AfterTypeCastParentheses = GetOptionByName(Rules.Spacing.Parentheses.AfterTypeCastParentheses, rules);
	  _WithinSingleLineAccessor = GetOptionByName(Rules.Spacing.WithinSingleLineAccessor, rules);
	  _WithinSingleLineMethod = GetOptionByName(Rules.Spacing.WithinSingleLineMethod, rules);
	  _WithinSingleLineAnonymousMethod = GetOptionByName(Rules.Spacing.WithinSingleLineAnonymousMethod, rules);
	  _AroundEqualsInNamespaceAliasDeclaration = GetOptionByName(Rules.Spacing.AroundEqualsInNamespaceAliasDeclaration, rules);
	  _BeforeOpenCurlyBraceOnTheSameLine = GetOptionByName(Rules.Spacing.BeforeOpenCurlyBraceOnTheSameLine, rules);
	  _BeforeComma = GetOptionByName(Rules.Spacing.PunctuationMarks.BeforeComma, rules);
	  _AfterComma = GetOptionByName(Rules.Spacing.PunctuationMarks.AfterComma, rules);
	  _BeforeDot = GetOptionByName(Rules.Spacing.PunctuationMarks.BeforeDot, rules);
	  _AfterDot = GetOptionByName(Rules.Spacing.PunctuationMarks.AfterDot, rules);
	  _BeforeColon = GetOptionByName(Rules.Spacing.PunctuationMarks.BeforeColon, rules);
	  _AfterColon = GetOptionByName(Rules.Spacing.PunctuationMarks.AfterColon, rules);
	  _AroundColonStatement = GetOptionByName(Rules.Spacing.PunctuationMarks.AroundColonStatement, rules);
	  _BeforeColonInLabels = GetOptionByName(Rules.Spacing.PunctuationMarks.BeforeColonInLabels, rules);
	  _AfterColonForAncestorsListInTypeDeclaration = GetOptionByName(Rules.Spacing.PunctuationMarks.AfterColonForAncestorsListInTypeDeclaration, rules);
	  _BeforeColonForAncestorsListInTypeDeclaration = GetOptionByName(Rules.Spacing.PunctuationMarks.BeforeColonForAncestorsListInTypeDeclaration, rules);
	  _AfterSemicolonInForStatement = GetOptionByName(Rules.Spacing.PunctuationMarks.AfterSemicolonInForStatement, rules);
	  _BeforeSemicolonInForStatement = GetOptionByName(Rules.Spacing.PunctuationMarks.BeforeSemicolonInForStatement, rules);
	  _BeforeSemicolon = GetOptionByName(Rules.Spacing.PunctuationMarks.BeforeSemicolon, rules);
	  _AfterSemicolon = false; 
	  _BeforeColonInCaseStatement = GetOptionByName(Rules.Spacing.PunctuationMarks.BeforeColonInCaseStatement, rules);
	  _WithinAttributeBrackets = GetOptionByName(Rules.Spacing.Attributes.WithinAttributeBrackets, rules);
	  _BeforeAttributeTargetColon = GetOptionByName(Rules.Spacing.Attributes.BeforeAttributeTargetColon, rules);
	  _AfterAttributeTargetColon = GetOptionByName(Rules.Spacing.Attributes.AfterAttributeTargetColon, rules);
	  _BeforeArrayRankBrackets = GetOptionByName(Rules.Spacing.Array.BeforeArrayRankBrackets, rules);
	  _WithinArrayRankBrackets = GetOptionByName(Rules.Spacing.Array.WithinArrayRankBrackets, rules);
	  _WithinArrayRankEmptyBrackets = GetOptionByName(Rules.Spacing.Array.WithinArrayRankEmptyBrackets, rules);
	  _BeforeArrayRankParentheses = GetOptionByName(Rules.Spacing.Array.BeforeArrayRankParentheses, rules);
	  _WithinArrayRankParentheses = GetOptionByName(Rules.Spacing.Array.WithinArrayRankParentheses, rules);
	  _WithinArrayRankEmptyParentheses = GetOptionByName(Rules.Spacing.Array.WithinArrayRankEmptyParentheses, rules);
	  _BeforeIfParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.IfParentheses, rules);
	  _BeforeForForeachParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.ForForeachParentheses, rules);
	  _BeforeWhileParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.WhileParentheses, rules);
	  _BeforeSwitchParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.SwitchParentheses, rules);
	  _BeforeCatchParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.CatchParentheses, rules);
	  _BeforeUsingLockParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.UsingLockParentheses, rules);
	  _BeforeTypeofSizeofParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.TypeofSizeofParentheses, rules);
	  _BeforeFixedParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.FixedParentheses, rules);
	  _BeforeCheckedUncheckedParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.CheckedUncheckedParentheses, rules);
	  _BeforeDefaultParentheses = GetOptionByName(Rules.Spacing.Parentheses.Before.DefaultParentheses, rules);
	  _WithinIfParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.IfParentheses, rules);
	  _WithinForForeachParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.ForForeachParentheses, rules);
	  _WithinWhileParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.WhileParentheses, rules);
	  _WithinSwitchParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.SwitchParentheses, rules);
	  _WithinCatchParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.CatchParentheses, rules);
	  _WithinUsingLockParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.UsingLockParentheses, rules);
	  _WithinTypeofSizeofParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.TypeofSizeofParentheses, rules);
	  _WithinFixedParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.FixedParentheses, rules);
	  _WithinCheckedUncheckedParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.CheckedUncheckedParentheses, rules);
	  _WithinDefaultParentheses = GetOptionByName(Rules.Spacing.Parentheses.Within.DefaultParentheses, rules);
	  _WithinArrayInitializerBraces = GetOptionByName(Rules.Spacing.Array.WithinArrayInitializerBraces, rules);
	  _BeforeOpeningBraceInProperty = GetOptionByName(Rules.Spacing.Property.BeforeOpeningBraceInDeclaration, rules);
	  _WithinBracesInPropertyDeclaration = GetOptionByName(Rules.Spacing.Property.WithinBracesInDeclaration, rules);
	  _WithinBracesInPropertyAccessors = GetOptionByName(Rules.Spacing.Property.WithinBracesInAccessor, rules);
	  _BeforeOpeningBraceInAccessor = GetOptionByName(Rules.Spacing.Property.BeforeOpeningBraceInAccessor, rules);
	}
	public override void LoadDefault(ParserLanguageID language)
	{
	  _AfterAttributeTargetColon = true;
	  _AfterColonForAncestorsListInTypeDeclaration = true;
	  _BeforeColonForAncestorsListInTypeDeclaration = true;
	  _AfterComma = true;
	  _AfterSemicolonInForStatement = true;
	  _BeforeForForeachParentheses = true;
	  _BeforeUsingLockParentheses = true;
	  _BeforeWhileParentheses = true;
	  _BeforeIfParentheses = true;
	  _AroundTernaryQuestionOperator = true;
	  _AroundTernaryColonOperator = true;
	  _AroundEqualsInNamespaceAliasDeclaration = true;
	  _AroundOneCharOperators = true;
	  _AroundTwoCharOperators = true;
	  _AfterTypeParameterConstraintColon = true;
	  _BeforeTypeParameterConstraintColon = true;
	  _BeforeDefaultParentheses = true;
	  _BeforeSwitchParentheses = true;
	  _BeforeCatchParentheses = true;
	  _AroundLambdaExpressionOperator = true;
	  _BeforeOpenCurlyBraceOnTheSameLine = true;
	  _WithinArrayInitializerBraces = true;
	  _BeforeOpeningBraceInProperty = true;
	  _WithinBracesInPropertyDeclaration = true;
	  _WithinBracesInPropertyAccessors = true;
	  _BeforeOpeningBraceInAccessor = true;
	  _AfterSemicolon = true;
	  _AfterColon = true;
	}
	public bool BeforeOpeningParenthesisOfAMethodWithParameters
	{
	  get
	  {
		return _BeforeOpeningParenthesisOfAMethodWithParameters;
	  }
	}
	public bool BeforeOpeningParenthesisOfAMethodWithoutParameters
	{
	  get
	  {
		return _BeforeOpeningParenthesisOfAMethodWithoutParameters;
	  }
	}
	public bool WithinMethodDeclarationParentheses
	{
	  get
	  {
		return _WithinMethodDeclarationParentheses;
	  }
	}
	public bool WithinEmptyMethodDeclarationParentheses
	{
	  get
	  {
		return _WithinEmptyMethodDeclarationParentheses;
	  }
	}
	public bool BeforeOpeningParenthesisOfAMethodCallWithArguments
	{
	  get
	  {
		return _BeforeOpeningParenthesisOfAMethodCallWithArguments;
	  }
	}
	public bool BeforeOpeningParenthesisOfAMethodCallWithoutArguments
	{
	  get
	  {
		return _BeforeOpeningParenthesisOfAMethodCallWithoutArguments;
	  }
	}
	public bool WithinArgumentsListParentheses
	{
	  get
	  {
		return _WithinArgumentsListParentheses;
	  }
	}
	public bool WithinEmptyArgumentsListParentheses
	{
	  get
	  {
		return _WithinEmptyArgumentsListParentheses;
	  }
	}
	public bool BeforeTypeParameterAngles
	{
	  get
	  {
		return _BeforeTypeParameterAngles;
	  }
	}
	public bool WithinTypeParameterAngles
	{
	  get
	  {
		return _WithinTypeParameterAngles;
	  }
	}
	public bool BeforeTypeArgumentAngles
	{
	  get
	  {
		return _BeforeTypeArgumentAngles;
	  }
	}
	public bool WithinTypeArgumentAngles
	{
	  get
	  {
		return _WithinTypeArgumentAngles;
	  }
	}
	public bool BeforeTypeParameterConstraintColon
	{
	  get
	  {
		return _BeforeTypeParameterConstraintColon;
	  }
	}
	public bool AfterTypeParameterConstraintColon
	{
	  get
	  {
		return _AfterTypeParameterConstraintColon;
	  }
	}
	public bool BeforeTypeParameterParenthesis
	{
	  get
	  {
		return _BeforeTypeParameterParenthesis;
	  }
	}
	public bool WithinTypeParameterParenthesis
	{
	  get
	  {
		return _WithinTypeParameterParenthesis;
	  }
	}
	public bool BeforeTypeArgumentParenthesis
	{
	  get
	  {
		return _BeforeTypeArgumentParenthesis;
	  }
	}
	public bool WithinTypeArgumentParenthesis
	{
	  get
	  {
		return _WithinTypeArgumentParenthesis;
	  }
	}
	public bool AroundOneCharOperators
	{
	  get
	  {
		return _AroundOneCharOperators;
	  }
	}
	public bool AroundTwoCharOperators
	{
	  get
	  {
		return _AroundTwoCharOperators;
	  }
	}
	public bool AroundUnsafeOperators
	{
	  get
	  {
		return _AroundUnsafeOperators;
	  }
	}
	public bool AroundTernaryQuestionOperator
	{
	  get
	  {
		return _AroundTernaryQuestionOperator;
	  }
	}
	public bool AroundTernaryColonOperator
	{
	  get
	  {
		return _AroundTernaryColonOperator;
	  }
	}
	public bool AroundLambdaExpressionOperator
	{
	  get
	  {
		return _AroundLambdaExpressionOperator;
	  }
	}
	public bool BeforeUnsafePointerDeclarationOperator
	{
	  get
	  {
		return _BeforeUnsafePointerDeclarationOperator;
	  }
	}
	public bool BeforeNullableTypeOperator
	{
	  get
	  {
		return _BeforeNullableTypeOperator;
	  }
	}
	public bool BeforeOpenSquareBracket
	{
	  get
	  {
		return _BeforeOpenSquareBracket;
	  }
	}
	public bool WithinSquareBrackets
	{
	  get
	  {
		return _WithinSquareBrackets;
	  }
	}
	public bool WithinEmptySquareBrackets
	{
	  get
	  {
		return _WithinEmptySquareBrackets;
	  }
	}
	public bool WithinExpressionParentheses
	{
	  get
	  {
		return _WithinExpressionParentheses;
	  }
	}
	public bool WithinTypeCastParentheses
	{
	  get
	  {
		return _WithinTypeCastParentheses;
	  }
	}
	public bool AfterTypeCastParentheses
	{
	  get
	  {
		return _AfterTypeCastParentheses;
	  }
	}
	public bool WithinSingleLineAccessor
	{
	  get
	  {
		return _WithinSingleLineAccessor;
	  }
	}
	public bool WithinSingleLineMethod
	{
	  get
	  {
		return _WithinSingleLineMethod;
	  }
	}
	public bool WithinSingleLineAnonymousMethod
	{
	  get
	  {
		return _WithinSingleLineAnonymousMethod;
	  }
	}
	public bool AroundEqualsInNamespaceAliasDeclaration
	{
	  get
	  {
		return _AroundEqualsInNamespaceAliasDeclaration;
	  }
	}
	public bool BeforeOpenCurlyBraceOnTheSameLine
	{
	  get
	  {
		return _BeforeOpenCurlyBraceOnTheSameLine;
	  }
	}
	public bool BeforeComma
	{
	  get
	  {
		return _BeforeComma;
	  }
	}
	public bool AfterComma
	{
	  get
	  {
		return _AfterComma;
	  }
	}
	public bool BeforeDot
	{
	  get
	  {
		return _BeforeDot;
	  }
	}
	public bool AfterDot
	{
	  get
	  {
		return _AfterDot;
	  }
	}
	public bool BeforeColon
	{
	  get
	  {
		return _BeforeColon;
	  }
	}
	public bool AfterColon
	{
	  get
	  {
		return _AfterColon;
	  }
	}
	public bool AroundColonStatement
	{
	  get
	  {
		return _AroundColonStatement;
	  }
	}
	public bool BeforeColonInLabels
	{
	  get
	  {
		return _BeforeColonInLabels;
	  }
	}
	public bool AfterColonForAncestorsListInTypeDeclaration
	{
	  get
	  {
		return _AfterColonForAncestorsListInTypeDeclaration;
	  }
	}
	public bool BeforeColonForAncestorsListInTypeDeclaration
	{
	  get
	  {
		return _BeforeColonForAncestorsListInTypeDeclaration;
	  }
	}
	public bool AfterSemicolonInForStatement
	{
	  get
	  {
		return _AfterSemicolonInForStatement;
	  }
	}
	public bool BeforeSemicolonInForStatement
	{
	  get
	  {
		return _BeforeSemicolonInForStatement;
	  }
	}
	public bool BeforeSemicolon
	{
	  get
	  {
		return _BeforeSemicolon;
	  }
	}
	public bool AfterSemicolon
	{
	  get
	  {
		return _AfterSemicolon;
	  }
	}
	public bool BeforeColonInCaseStatement
	{
	  get
	  {
		return _BeforeColonInCaseStatement;
	  }
	}
	public bool WithinAttributeBrackets
	{
	  get
	  {
		return _WithinAttributeBrackets;
	  }
	}
	public bool BeforeAttributeTargetColon
	{
	  get
	  {
		return _BeforeAttributeTargetColon;
	  }
	}
	public bool AfterAttributeTargetColon
	{
	  get
	  {
		return _AfterAttributeTargetColon;
	  }
	}
	public bool BeforeArrayRankBrackets
	{
	  get
	  {
		return _BeforeArrayRankBrackets;
	  }
	}
	public bool WithinArrayRankBrackets
	{
	  get
	  {
		return _WithinArrayRankBrackets;
	  }
	}
	public bool WithinArrayRankEmptyBrackets
	{
	  get
	  {
		return _WithinArrayRankEmptyBrackets;
	  }
	}
	public bool BeforeArrayRankParentheses
	{
	  get
	  {
		return _BeforeArrayRankParentheses;
	  }
	}
	public bool WithinArrayRankParentheses
	{
	  get
	  {
		return _WithinArrayRankParentheses;
	  }
	}
	public bool WithinArrayRankEmptyParentheses
	{
	  get
	  {
		return _WithinArrayRankEmptyParentheses;
	  }
	}
	public bool WithinArrayInitializerBraces
	{
	  get
	  {
		return _WithinArrayInitializerBraces;
	  }
	}
	public bool BeforeIfParentheses
	{
	  get
	  {
		return _BeforeIfParentheses;
	  }
	}
	public bool BeforeForForeachParentheses
	{
	  get
	  {
		return _BeforeForForeachParentheses;
	  }
	}
	public bool BeforeWhileParentheses
	{
	  get
	  {
		return _BeforeWhileParentheses;
	  }
	}
	public bool BeforeSwitchParentheses
	{
	  get
	  {
		return _BeforeSwitchParentheses;
	  }
	}
	public bool BeforeCatchParentheses
	{
	  get
	  {
		return _BeforeCatchParentheses;
	  }
	}
	public bool BeforeUsingLockParentheses
	{
	  get
	  {
		return _BeforeUsingLockParentheses;
	  }
	}
	public bool BeforeTypeofSizeofParentheses
	{
	  get
	  {
		return _BeforeTypeofSizeofParentheses;
	  }
	}
	public bool BeforeFixedParentheses
	{
	  get
	  {
		return _BeforeFixedParentheses;
	  }
	}
	public bool BeforeCheckedUncheckedParentheses
	{
	  get
	  {
		return _BeforeCheckedUncheckedParentheses;
	  }
	}
	public bool BeforeDefaultParentheses
	{
	  get
	  {
		return _BeforeDefaultParentheses;
	  }
	}
	public bool WithinIfParentheses
	{
	  get
	  {
		return _WithinIfParentheses;
	  }
	}
	public bool WithinForForeachParentheses
	{
	  get
	  {
		return _WithinForForeachParentheses;
	  }
	}
	public bool WithinWhileParentheses
	{
	  get
	  {
		return _WithinWhileParentheses;
	  }
	}
	public bool WithinSwitchParentheses
	{
	  get
	  {
		return _WithinSwitchParentheses;
	  }
	}
	public bool WithinCatchParentheses
	{
	  get
	  {
		return _WithinCatchParentheses;
	  }
	}
	public bool WithinUsingLockParentheses
	{
	  get
	  {
		return _WithinUsingLockParentheses;
	  }
	}
	public bool WithinTypeofSizeofParentheses
	{
	  get
	  {
		return _WithinTypeofSizeofParentheses;
	  }
	}
	public bool WithinFixedParentheses
	{
	  get
	  {
		return _WithinFixedParentheses;
	  }
	}
	public bool WithinCheckedUncheckedParentheses
	{
	  get
	  {
		return _WithinCheckedUncheckedParentheses;
	  }
	}
	public bool WithinDefaultParentheses
	{
	  get
	  {
		return _WithinDefaultParentheses;
	  }
	}
	public bool BeforeOpeningBraceInProperty
	{
	  get
	  {
		return _BeforeOpeningBraceInProperty;
	  }
	}
	public bool WithinBracesInPropertyDeclaration
	{
	  get
	  {
		return _WithinBracesInPropertyDeclaration;
	  }
	}
	public bool WithinBracesInPropertyAccessors
	{
	  get
	  {
		return _WithinBracesInPropertyAccessors;
	  }
	}
	public bool BeforeOpeningBraceInAccessor
	{
	  get
	  {
		return _BeforeOpeningBraceInAccessor;
	  }
	}
  }
}
