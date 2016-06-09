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
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  public enum FormattingCategory
  {
	General,
	BlankLines,
	LineBreaks,
	Indentation,
	Spacing,
	WrappingAndAlignment,
	SortingAndOrdering,
	OtherAndLanguageSpecific
  }
  public enum FormattingOption
  {
	Boolean,
	Integer,
	Editable
  }
  public static class Rules
  {
	#region General
	public static class General
	{
	  public const string AdjustCodeStyleOnAutoformat = "Adjust code style on autoformat";
	  public const string KeepExistingBlankLines = "Keep existing blank lines";
	  public const string KeepExistingLineBreaks = "Keep existing line breaks";
	  public const string KeepExpressionsIndent = "Keep expressions indent";
	  public const string KeepExistingWhiteSpace = "Keep existing white space";
	}
	#endregion
	#region BlankLines
	public static class BlankLines
	{
	  public const string AfterFileOptionsSection = "After file options section";
	  public const string AfterImportedNamespacesSection = "After imported namespaces section";
	  public const string AfterNamespaces = "After namespaces";
	  public const string AfterTypeDeclarations = "After type declarations";
	  public const string AfterSingleLineMembers = "After single line members";
	  public const string AfterMultiLineMembers = "After multi line members";
	  public const string AroundRegionDirectives = "Around region directives";
	  public const string InsideRegionDirectives = "Inside region directives";
	  public const string AfterGlobalAttributes = "After global attributes";
	  public const string BetweenDifferentImportedNamespacesGroups = "Between different imported namespaces groups";
	  public const string AfterProcessingInstructions = "After processing instructions";
	  public const string MaximumBlankLinesBetweenTags = "Maximum blank lines between tags";
	}
	#endregion
	#region LineBreaks
	public static class LineBreaks
	{
	  #region Attributes
	  public static class Attributes
	  {
		public const string PlaceTypeAttributeOnSeparateLine = "Place type attribute on separate line";
		public const string PlaceMethodAttributeOnSeparateLine = "Place method attribute on separate line";
		public const string PlacePropertyAttributeOnSeparateLine = "Place property attribute on separate line";
		public const string PlaceEventAttributeOnSeparateLine = "Place event attribute on separate line";
		public const string PlaceFieldConstantAttributeOnSeparateLine = "Place field/constant attribute on separate line";
		public const string PlaceEnumElementAttributeOnSeparateLine = "Place enum element attribute on separate line";
		public const string PlaceMultipleAttributesOnTheirOwnLine = "Place multiple attributes on their own line";
	  }
	  #endregion
	  #region OpenBrace
	  public static class OpenBrace
	  {
		public const string Namespaces = "Place open brace on new line for namespaces";
		public const string TypeDeclarations = "Place open brace on new line for type declarations";
		public const string Methods = "Place open brace on new line for methods";
		public const string AnonymousMethods = "Place open brace on new line for anonymous methods";
		public const string CodeBlocks = "Place open brace on new line for code blocks";
		public const string AnonymousTypes = "Place open brace on new line for anonymous types";
		public const string ArrayObjectAndCollInitializers = "Place open brace on new line for array, object and collection initializers";
		public const string LambdaExpressions = "Place open brace on new line for lambda expressions";
		public const string Properties = "Place open brace on new line for properties";
		public const string BlocksUnderCaseStatement = "Place open brace on new line for block under case statement";
	  }
	  #endregion
	  #region CloseBrace
	  public static class CloseBrace
	  {
		public const string Namespaces = "Place close brace on new line for namespaces";
		public const string TypeDeclarations = "Place close brace on new line for type declarations";
		public const string Methods = "Place close brace on new line for methods";
		public const string AnonymousMethods = "Place close brace on new line for anonymous methods";
		public const string CodeBlocks = "Place close brace on new line for code blocks";
		public const string AnonymousTypes = "Place close brace on new line for anonymous types";
		public const string ArrayObjectAndCollInitializers = "Place close brace on new line for array, object and collection initializers";
		public const string LambdaExpressions = "Place close brace on new line for lambda expressions";
		public const string Properties = "Place close brace on new line for properties";
		public const string BlocksUnderCaseStatement = "Place close brace on new line for block under case statement";
	  }
	  #endregion
	  #region Statements
	  public static class Statements
	  {
		public const string PlaceSimpleEmbeddedStatementOnSingleLine = "Place single embedded statement on single line";
		public const string PlaceElseStatementOnNewLine = "Place 'else' statement on new line";
		public const string PlaceWhileStatementOnNewLine = "Place 'while' statement on new line";
		public const string PlaceCatchStatementOnNewLine = "Place 'catch' statement on new line";
		public const string PlaceFinallyStatementOnNewLine = "Place 'finally' statement on new line";
		public const string PlaceIfStatementFollowedByElseOnNewLine = "Place an 'if' statement that follows the 'else' one on a new line";
	  }
	  #endregion
	  #region Members
	  public static class Members
	  {
		public const string PlaceAbstractInterfaceMemberOnSingleLine = "Place abstract/interface member on single line";
		public const string PlaceAutoImplementedPropertyOnSingleLine = "Place auto-implemented properties on single line";
		public const string PlaceSimpleMemberOnSingleLine = "Place simple member on single line";
		public const string PlaceSimpleAccessorOnSingleLine = "Place simple accessor on single line";
		public const string PlaceSimpleAnonymousMethodOnSingleLine = "Place simple anonymous method on single line";
		public const string PlaceConstructorInitializerOnSameLine = "Place constructor initializer on same line";
	  }
	  #endregion
	}
	#endregion
	#region Indentation
	public static class Indentation
	{
	  public const string OpenAndCloseBraces = "Indent open and close braces";
	  public const string CodeBlockContents = "Indent code block contents";
	  public const string AnonymousMethodContents = "Indent anonymous method contents";
	  public const string ArrayObjectAndCollectionInitializerContents = "Indent array, object and collection initializer contents";
	  public const string CaseStatementFromSwitchStatement = "Indent case statement from switch statement";
	  public const string CaseStatementContents = "Indent case statement contents";
	  public const string NestedUsingStatements = "Indent nested 'using' statements";
	  public const string IndentLabels = "Indent labels";
	  public const string IndentComments = "Indent comments";
	}
	#endregion
	#region Spacing
	public static class Spacing
	{
	  public const string WithinSingleLineAccessor = "Within single line accessor";
	  public const string WithinSingleLineMethod = "Within single line method";
	  public const string WithinSingleLineAnonymousMethod = "Within single line anonymous method";
	  public const string AroundEqualsInNamespaceAliasDeclaration = "Around '=' in namespace alias declaration";
	  public const string BeforeOpenCurlyBraceOnTheSameLine = "Before open curly brace on the same line";
	  #region Method
	  public static class Method
	  {
		public const string BeforeOpeningParenthesisOfAMethodWithParameters = "Before opening parenthesis of a method with parameters";
		public const string BeforeOpeningParenthesisOfAMethodWithoutParameters = "Before opening parenthesis of a method without parameters";
		public const string WithinMethodDeclarationParentheses = "Within method declaration parentheses";
		public const string WithinEmptyMethodDeclarationParentheses = "Within empty method declaration parentheses";
	  }
	  #endregion
	  #region Property
	  public static class Property
	  {
		public const string BeforeOpeningBraceInDeclaration = "Before opening braces of a property if on single line";
		public const string BeforeOpeningBraceInAccessor = "Before opening braces of an accessor if on single line";
		public const string WithinBracesInDeclaration = "Within property braces if on single line";
		public const string WithinBracesInAccessor = "Within accessors braces if on single line";
	  }
	  #endregion
	  #region MethodCall
	  public static class MethodCall
	  {
		public const string BeforeOpeningParenthesisOfAMethodCallWithArguments = "Before opening parenthesis of a method call with arguments";
		public const string BeforeOpeningParenthesisOfAMethodCallWithoutArguments = "Before opening parenthesis of a method call without arguments";
		public const string WithinArgumentsListParentheses = "Within argument list parentheses";
		public const string WithinEmptyArgumentsListParentheses = "Within empty argument list parentheses";
	  }
	  #endregion
	  #region TypeParameters
	  public static class TypeParameters
	  {
		public const string BeforeTypeParameterAngles = "Before type parameter angles";
		public const string WithinTypeParameterAngles = "Within type parameter angles";
		public const string BeforeTypeParameterConstraintColon = "Before type parameter constraint colon";
		public const string AfterTypeParameterConstraintColon = "After type parameter constraint colon";
		public const string BeforeTypeParameterParenthesis = "Before type parameter parenthesis";
		public const string WithinTypeParameterParenthesis = "Within type parameter parenthesis";
	  }
	  #endregion
	  #region TypeArguments
	  public static class TypeArguments
	  {
		public const string BeforeTypeArgumentAngles = "Before type argument angles";
		public const string WithinTypeArgumentAngles = "Within type argument angles";
		public const string BeforeTypeArgumentParenthesis = "Before type argument parenthesis";
		public const string WithinTypeArgumentParenthesis = "Within type argument parenthesis";
	  }
	  #endregion
	  #region Operators
	  public static class Operators
	  {
		public const string AroundOneCharOperators = "Around one char operators (=, +, *, ^, |)";
		public const string AroundTwoCharOperators = "Around two char operators (+=, >>, <=)";
		public const string AroundUnsafeOperators = "Around unsafe operators (&&, *, ->)";
		public const string AroundTernaryQuestionOperator = "Around ternary '?' operator";
		public const string AroundTernaryColonOperator = "Around ternary ':' operator";
		public const string AroundLambdaExpressionOperator = "Around lambda expression operator '=>'";
		public const string BeforeUnsafePointerDeclarationOperator = "Before unsafe pointer declaration operator";
		public const string BeforeNullableTypeOperator = "Before nullable type operator '?'";
	  }
	  #endregion
	  #region SquareBracket
	  public static class SquareBracket
	  {
		public const string BeforeOpenSquareBracket = "Before open square bracket";
		public const string WithinSquareBrackets = "Within square brackets";
		public const string WithinEmptySquareBrackets = "Within empty square brackets";
	  }
	  #endregion
	  #region Parentheses
	  public static class Parentheses
	  {
		public const string AfterTypeCastParentheses = "After type cast parentheses";
		#region Within
		public static class Within
		{
		  public const string ExpressionParentheses = "Within expression parentheses";
		  public const string TypeCastParentheses = "Within type cast parentheses";
		  public const string IfParentheses = "Within 'if' parentheses";
		  public const string ForForeachParentheses = "Within 'for', 'foreach' parentheses";
		  public const string WhileParentheses = "Within 'while' parentheses";
		  public const string SwitchParentheses = "Within 'switch' parentheses";
		  public const string CatchParentheses = "Within 'catch' parentheses";
		  public const string UsingLockParentheses = "Within 'using', 'lock' parentheses";
		  public const string TypeofSizeofParentheses = "Within 'typeof', 'sizeof' parentheses";
		  public const string FixedParentheses = "Within 'fixed' parentheses";
		  public const string CheckedUncheckedParentheses = "Within 'checked', 'unchecked' parentheses";
		  public const string DefaultParentheses = "Within 'default' parentheses";
		}
		#endregion
		#region Before
		public static class Before
		{
		  public const string IfParentheses = "Before 'if' parentheses";
		  public const string ForForeachParentheses = "Before 'for', 'foreach' parentheses";
		  public const string WhileParentheses = "Before 'while' parentheses";
		  public const string SwitchParentheses = "Before 'switch' parentheses";
		  public const string CatchParentheses = "Before 'catch' parentheses";
		  public const string UsingLockParentheses = "Before 'using', 'lock' parentheses";
		  public const string FixedParentheses = "Before 'fixed' parentheses";
		  public const string CheckedUncheckedParentheses = "Before 'checked', 'unchecked' parentheses";
		  public const string TypeofSizeofParentheses = "Before 'typeof', 'sizeof' parentheses";
		  public const string DefaultParentheses = "Before 'default' parentheses";
		}
		#endregion
	  }
	  #endregion
	  #region PunctuationMarks
	  public static class PunctuationMarks
	  {
		public const string BeforeComma = "Before comma";
		public const string AfterComma = "After comma";
		public const string BeforeDot = "Before dot";
		public const string AfterDot = "After dot";
		public const string BeforeColon = "Before colon";
		public const string AfterColon = "After colon";
		public const string AroundColonStatement = "Around colon statement";
		public const string BeforeColonInLabels = "Before colon in labels";
		public const string AfterColonForAncestorsListInTypeDeclaration = "After colon for ancestors list in type declaration";
		public const string BeforeColonForAncestorsListInTypeDeclaration = "Before colon for ancestors list in type declaration";
		public const string AfterSemicolonInForStatement = "After semicolon in 'for' statement";
		public const string BeforeSemicolonInForStatement = "Before semicolon in 'for' statement";
		public const string BeforeSemicolon = "Before semicolon";
		public const string AfterSemicolon = "After semicolon";
		public const string BeforeColonInCaseStatement = "Before colon in case statement";
	  }
	  #endregion
	  #region Attributes
	  public static class Attributes
	  {
		public const string WithinAttributeBrackets = "Within attribute brackets/angles";
		public const string BeforeAttributeTargetColon = "Before attribute target colon";
		public const string AfterAttributeTargetColon = "After attribute target colon";
	  }
	  #endregion
	  #region Array
	  public static class Array
	  {
		public const string BeforeArrayRankBrackets = "Before array rank brackets";
		public const string WithinArrayRankBrackets = "Within array rank brackets";
		public const string WithinArrayRankEmptyBrackets = "Within array rank empty brackets";
		public const string BeforeArrayRankParentheses = "Before array rank parentheses";
		public const string WithinArrayRankParentheses = "Within array rank parentheses";
		public const string WithinArrayRankEmptyParentheses = "Within array rank empty parentheses";
		public const string WithinArrayInitializerBraces = "Within array initializer braces";
	  }
	  #endregion
	}
	#endregion
	#region WrappingAlignment
	public static class WrappingAlignment
	{
	  public const string WrapLongLines = "Wrap long lines";
	  public const string RightMarginColumnSize = "Right margin column size";
	  public const string WrapFormalParameters = "Wrap formal parameters";
	  public const string WrapFirstFormalParameter = "Wrap first formal parameter";
	  public const string AlignWithFirstFormalParameter = "Align with first formal parameter";
	  public const string WrapBeforeOpenBraceInDeclaration = "Wrap before open brace in declaration";
	  public const string WrapBeforeCloseBraceInDeclaration = "Wrap before close brace in declaration";
	  public const string WrapInvocationArguments = "Wrap invocation arguments";
	  public const string WrapFirstInvocationArgument = "Wrap first invocation argument";
	  public const string AlignWithFirstInvocationArgument = "Align with first invocation argument";
	  public const string WrapBeforeOpenBraceInInvocation = "Wrap before open brace in invocation";
	  public const string WrapBeforeCloseBraceInInvocation = "Wrap before close brace in invocation";
	  public const string WrapForStatementHeader = "Wrap 'for' statement header";
	  public static class Expressions
	  {
		public const string WrapTernaryExpression = "Wrap ternary expression";
		public const string WrapLogicalOperatorExpression = "Wrap logical operator expression";
		public const string WrapBinaryOperatorExpression = "Wrap binary operator expression";
		public const string WrapBeforeOperatorInLogicalExpression = "Wrap before operator in logical expression";
		public const string WrapBeforeOperatorInBinaryExpression = "Wrap before operator in binary expression";
		public const string AlignWithFirstBinaryExpressionItem = "Align with first binary expression item";
		public const string AlignWithFirstLogicalExpressionItem = "Align with first logical expression item";
		public const string WrapQueryExpression = "Wrap query expression item";
		public const string AlignWithFirstQueryExpressionItem = "Align with first query expression item";
	  }
	  public static class TypeParameters
	  {
		public const string WrapTypeParameters = "Wrap multiple type parameters";
		public const string WrapFirstTypeParameter = "Wrap first type parameter";
		public const string AlignWithFirstTypeParameter = "Align with first type parameter";
		public static class Constraints
		{
		  public const string WrapTypeParameterConstraints = "Wrap multiple type parameter constraints";
		  public const string WrapFirstTypeParameterConstraint = "Wrap first type parameter constraint";
		  public const string AlignWithFirstTypeParameterConstraint = "Align with first type parameter constraint";
		}
	  }
	  public static class ImplementsHandles
	  {
		public const string WrapImplementsHandlesSectionItems = "Wrap implements/handles section items";
		public const string WrapFirstImplementsHandlesSectionItem = "Wrap first implements/handles section item";
		public const string AlignWithFirstImplementsHandlesSectionItem = "Align with first implements/handles section item";
	  }
	  public static class ArrayAndObjectInitializers
	  {
		public const string WrapArrayObjectAndCollectionInitializers = "Wrap array, object and collection initializers";
		public const string WrapFirstArrayObjectAndCollectionInitializer = "Wrap first array, object and collection initializer";
		public const string AlignWithFirstArrayObjectOrCollectionInitializerItem = "Align with first array, object or collection initializer item";
	  }
	  public static class AnonymousTypes
	  {
		public const string WrapMembersInAnonymousType = "Wrap members in anonymous type";
		public const string WrapFirstMemberInAnonymousType = "Wrap first member in anonymous type";
		public const string AlignWithFirstMemberInAnonymousType = "Align with first member in anonymous type";
	  }
	  public static class MultiVariable
	  {
		public const string WrapMultiVariableDeclaration = "Wrap multi-variable declaration";
		public const string WrapFirstMultiVariableDeclaration = "Wrap first multi-variable declaration";
		public const string AlignWithFirstMultiVariableDeclarationItem = "Align with first multi-variable declaration item";
	  }
	  public static class TypeAncestors
	  {
		public const string WrapAncestorsListInTypeDeclaration = "Wrap ancestors list in type declaration";
		public const string WrapFirstAncestorsListItem = "Wrap first ancestors list item";
		public const string AlignWithFirstAncestorsListItem = "Align with first ancestors list item";
		public const string WrapBeforeColonForAncestorsList = "Wrap before colon for ancestors list";
	  }
	}
	#endregion
	public static class Other
	{
	  public const string AddLineFeedAtEndOfFile = "Add line feed at end of file";
	}
  }
}
