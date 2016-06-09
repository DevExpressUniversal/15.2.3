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
using System.Text;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public class CSharpExpressionCodeGen : TokenCExpressionCodeGenBase
	{
		public CSharpExpressionCodeGen(CodeGen codeGen)
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
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Ident:
		  if (ContextMatch(LanguageElementType.MarkupExtensionExpression))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Colon:
		  if (ContextMatch(LanguageElementType.MarkupExtensionExpression))
		  {
			result.AddWhiteSpace();
		  }
		  if (ContextMatch(LanguageElementType.AttributeVariableInitializer))
		  {
			if (Options.Spacing.AfterColon)
			  result.AddWhiteSpace();
		  }
		  else
			if(!ContextMatch(LanguageElementType.ConditionalExpression) && Options.Spacing.AroundOneCharOperators)
			  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Ampersand:
		  if (Options.Spacing.AroundUnsafeOperators)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.LessThan:
		  if (Options.Spacing.WithinTypeArgumentAngles && (Context is ReferenceExpressionBase))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenOpen:
		  if (Options.Spacing.WithinCheckedUncheckedParentheses &&
			(ContextMatch(LanguageElementType.CheckedExpression) || ContextMatch(LanguageElementType.UncheckedExpression)))
			result.AddWhiteSpace();
		  else if (Options.Spacing.WithinTypeofSizeofParentheses &&
			(ContextMatch(LanguageElementType.CheckedExpression) || ContextMatch(LanguageElementType.UncheckedExpression)))
			result.AddWhiteSpace();
		  else if (Options.Spacing.WithinDefaultParentheses && ContextMatch(LanguageElementType.DefaultValueExpression))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Is:
		case FormattingTokenType.As:
		case FormattingTokenType.OrderBy:
		case FormattingTokenType.Group:
		case FormattingTokenType.Let:
		case FormattingTokenType.By:
		case FormattingTokenType.Into:
		case FormattingTokenType.In:
		case FormattingTokenType.Where:
		case FormattingTokenType.Select:
		case FormattingTokenType.On:
		case FormattingTokenType.Join:
		case FormattingTokenType.Stackalloc:
		case FormattingTokenType.Out:
		case FormattingTokenType.Ref:
		case FormattingTokenType.Equals:
		case FormattingTokenType.From:
		case FormattingTokenType.Await:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.EqualGreaterThen:
		  bool isSimleExpression = Context.NodeCount > 0 && Context.Nodes[0] is Expression;
		  bool placeOpenBraceOnNewLine = Options.LineBreaks.PlaceOpenBraceOnNewLineForAnonymousMethods && !isSimleExpression;
		  if (Options.Spacing.AroundLambdaExpressionOperator && !placeOpenBraceOnNewLine)
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
		case FormattingTokenType.Ident:
		  if (ContextMatch(LanguageElementType.MarkupExtensionExpression))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Question:
		  if (ContextMatch(LanguageElementType.TypeReferenceExpression) && Options.Spacing.BeforeNullableTypeOperator)
			  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.GreaterThen:
		  if (Options.Spacing.WithinTypeArgumentAngles && (Context is ReferenceExpressionBase))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.LessThan:
		  if (Options.Spacing.BeforeTypeArgumentAngles && (Context is ReferenceExpressionBase))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.On:
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.BracketOpen:
		 if (Options.Spacing.BeforeArrayRankBrackets)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.In:
		case FormattingTokenType.By:
		case FormattingTokenType.Is:
		case FormattingTokenType.As:
		case FormattingTokenType.Ascending:
		case FormattingTokenType.Descending:
		case FormattingTokenType.Equals:
		case FormattingTokenType.Into:
		  result.AddWhiteSpace();
		  break;
		case  FormattingTokenType.Colon:
		  if (ContextMatch(LanguageElementType.AttributeVariableInitializer) && Options.Spacing.BeforeColon)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.EqualGreaterThen:
		  if (Options.Spacing.AroundLambdaExpressionOperator)
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
		#region GenerateAddressOfExpression
		protected override void GenerateAddressOfExpression(AddressOfExpression expression)
		{
			Write(FormattingTokenType.Ampersand);
			CodeGen.GenerateElement(expression.Expression);
		}
		#endregion	
		#region NullMethods
		#region GenerateManagedArrayCreateExpression
		protected override void GenerateManagedArrayCreateExpression(ManagedArrayCreateExpression expression)
		{
		}
		#endregion
		protected override void GenerateDeleteArrayExpression(DeleteArrayExpression expression )
		{
		}
		protected override void GenerateComplexExpression(ComplexExpression expression)
		{
		}
		protected override void GenerateDeleteExpression(DeleteExpression expression )
		{
		}
		protected override void GenerateElaboratedTypeReference(ElaboratedTypeReference expression)
		{
		}
		protected override void GenerateManagedObjectCreationExpression(ManagedObjectCreationExpression expression)
		{
		}
		protected override void GenerateParametizedArrayCreateExpression(ParametrizedArrayCreateExpression expression)
		{
		}
		protected override void GenerateParametizedObjectCreationExpression(ParametrizedObjectCreationExpression expression)
		{
		}
		protected override void GenerateQualifiedNestedReference(QualifiedNestedReference expression)
		{			
		}
		protected override void GenerateQualifiedNestedTypeReference(QualifiedNestedTypeReference expression)
		{			
		}
		protected override void GenerateQualifiedTypeReferenceExpression(QualifiedTypeReferenceExpression expression)
		{			
		}
		protected override void GenerateQualifiedMethodReference (QualifiedMethodReference expression)
		{
		}		
		protected override void GeneratePointerMethodReference (PointerMethodReference expression)
		{
		}
		#endregion
		#region GenerateCppQualifiedElementReference
		protected override void GenerateCppQualifiedElementReference(CppQualifiedElementReference expression)
		{		
		}
		#endregion
	protected override void GenerateAttributeVariableInitializer(AttributeVariableInitializer expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  if (Context.Parent.ElementType == LanguageElementType.MarkupExtensionExpression)
		Write(FormattingTokenType.Equal);
	  else
		Write(FormattingTokenType.Colon);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override void GenerateMarkupExtensionExpression(MarkupExtensionExpression expression)
	{
	  Write(FormattingTokenType.CurlyBraceOpen);
	  if (expression.Qualifier != null)
		CodeGen.GenerateElement(expression.Qualifier);
	  Code.Write(" ");
	  if (expression.ArgumentCount > 0)
		GenerateExpressionList(expression.Arguments, " ");
	  if (expression.InitializerCount > 0)
	  {
		if (expression.ArgumentCount > 0)
		  Write(FormattingTokenType.Comma);
		GenerateExpressionList(expression.Initializers);
	  }
	  Write(FormattingTokenType.CurlyBraceClose);
	}
		protected override void GenerateGenericTypeArguments(TypeReferenceExpressionCollection arguments)
		{
	  Write(FormattingTokenType.LessThan);
			GenerateExpressionList(arguments);
	  Write(FormattingTokenType.GreaterThen);
		}
		private  void GenerateJoinExpressionBase(JoinExpressionBase expression)
		{
			if (expression == null)
				return;
			Write(FormattingTokenType.Join);
			CodeGen.GenerateElement(expression.InExpression);
	  Write(FormattingTokenType.On);
			if (expression.EqualsExpressions != null && expression.EqualsExpressions.Count != 0)
				GenerateExpressionList(expression.EqualsExpressions, FormattingTokenType.Comma);
		}
		protected override void GenerateArrayCreateExpression(ArrayCreateExpression expression)
		{
	  if (!expression.IsStackAlloc)
		Write(FormattingTokenType.New);
	  else
		Write(FormattingTokenType.Stackalloc);
			TypeReferenceExpression baseType = expression.BaseType;
			bool baseTypeIsArray = baseType != null && baseType.IsArrayType;
			int dimensionsCount = -1;
			if (expression.Dimensions != null)
				dimensionsCount = expression.Dimensions.Count;
	  bool hasDimensions = expression.Dimensions != null && expression.Dimensions.Count > 0;
	  if (dimensionsCount > 0 && baseTypeIsArray)
		GenerateArrayCreateWithDimensionsAndBaseArrayType(expression, baseType);
	  else
	  {
		CodeGen.GenerateElement(baseType);
		if (!baseTypeIsArray)
		  Write(FormattingTokenType.BracketOpen);
		if (hasDimensions)
		{
		  if (baseTypeIsArray)
			Write(FormattingTokenType.BracketOpen);
		  GenerateExpressionList(expression.Dimensions);
		  if (baseTypeIsArray)
			Write(FormattingTokenType.BracketClose);
		}
		if (!baseTypeIsArray)
		  Write(FormattingTokenType.BracketClose);
	  }
	  Expression initializer = expression.Initializer;
	  if (initializer == null)
		return;
	  CodeGen.GenerateElement(initializer);
		}
		private void GenerateArrayCreateWithDimensionsAndBaseArrayType(ArrayCreateExpression expression, TypeReferenceExpression baseType)
		{
			if (baseType == null || expression == null)
				return;
			TypeReferenceExpression arrayBaseType = baseType.Clone() as TypeReferenceExpression;
			TypeReferenceExpression nonArrayBaseType = null;
	  while (arrayBaseType.BaseType != null && arrayBaseType.BaseType.IsArrayType)
		arrayBaseType = arrayBaseType.BaseType;
			nonArrayBaseType = arrayBaseType.BaseType;
			arrayBaseType.BaseType= null;
			arrayBaseType.Rank = arrayBaseType.Rank;
			CodeGen.GenerateElement(nonArrayBaseType);
	  Write(FormattingTokenType.BracketOpen);
	  GenerateExpressionList(expression.Dimensions);
	  Write(FormattingTokenType.BracketClose);
			CodeGen.GenerateElement(arrayBaseType);
		}
	protected override void GenerateObjectCreationExpression(ObjectCreationExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.New);
	  CodeGen.GenerateElement(expression.ObjectType);
	  Expression init = expression.ObjectInitializer;
	  ExpressionCollection arguments = expression.Arguments;
	  if ((arguments != null && arguments.Count != 0) || init == null || !expression.ParensRange.IsEmpty) 
	  {
		Write(FormattingTokenType.ParenOpen);
		GenerateElementCollection(expression.Arguments, FormattingTokenType.Comma);
		Write(FormattingTokenType.ParenClose);
	  }
	  if (init == null)
		return;
	  CodeGen.GenerateElement(init);
	}
		#region GenerateMethodCallExpression
	protected override void GenerateMethodCallExpression(MethodCallExpression expression)
	{
	  CodeGen.GenerateElement(expression.Qualifier);
	  Write( FormattingTokenType.ParenOpen);
	  GenerateElementCollection(expression.Arguments, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	}
		#endregion
		#region GenerateArgumentDirection
		protected override void GenerateArgumentDirection(ArgumentDirection direction)
		{
			switch (direction)
			{
				case ArgumentDirection.Out:
		  Write(FormattingTokenType.Out);
					break;
				case ArgumentDirection.Ref:
		  Write(FormattingTokenType.Ref);
					break;
			}
		}
		#endregion
		#region GenerateArgumentDirectionExpression
		protected override void GenerateArgumentDirectionExpression(ArgumentDirectionExpression expression)
		{
			GenerateArgumentDirection(expression.Direction);
			CodeGen.GenerateElement(expression.Expression);
		}
		#endregion		
		#region GenerateBaseReferenceExpression
		protected override void GenerateBaseReferenceExpression(BaseReferenceExpression expression)
		{
	  Write(FormattingTokenType.Base);
		}
		#endregion
		#region GenerateCheckedExpression
		protected override void GenerateCheckedExpression(CheckedExpression expression)
		{
			Write(FormattingTokenType.Checked);
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.Expression);
	  Write(FormattingTokenType.ParenClose);
		}
		#endregion
		protected override void GenerateBooleanLiteral(bool value)
		{
	  if (value)
		Write(FormattingTokenType.True);
	  else
		Write(FormattingTokenType.False);
		}
		protected override bool IsEscapedString(string value)
		{
			if (value == null)
				return false;
			if (value.StartsWith("@\""))
				return true;
			for (int i = 0; i < value.Length; i++)
				switch (value[i])
				{
					case '\0':
					case '\t':
					case '\r':
					case '\n':
					case '\u2028':
					case '\u2029':
					case '\\':
						return true;
				}
			return false;
		}
		protected override void GenerateStringLiteral(string value)
		{
			if (value == null)
				return;
			if (value.Length == 0)
				Write("\"\"");
			else
			{
				string val = ReplaceEscapeChars(value);
				Write("\"" + val + "\"");
			}
		}
	protected override bool GenerateSpecificExpression(Expression expression)
	{
	  switch (expression.ElementType)
	  {
		case LanguageElementType.LambdaFunctionExpression:
		  GenerateLambdaExpression(expression as LambdaExpression);
		  return true;
		case LanguageElementType.AwaitExpression:
		  GenerateAwaitExpression(expression as AwaitExpression);
		  return true;
	  }
	  return false;
	}
	void GenerateAwaitExpression(AwaitExpression awaitExpression)
	{
	  Write(FormattingTokenType.Await);
	  CodeGen.GenerateElement(awaitExpression.SourceExpression);
	}
		string ReplaceEscapeChars(string value)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
				sb.Append(ReplaceChar(value[i]));
			return sb.ToString();
		}
		string ReplaceChar(char c)
		{
			switch (c)
			{
				case '\0':
					return "\\0";
				case '\t':
					return "\\t";
				case '\r':
					return "\\r";
				case '\n':
					return "\\n";
				case '\u2028':
					return "\\u2028";
				case '\u2029':
					return "\\u2029";
				case '\\':
					return "\\\\";
			}
			return c.ToString();
		}
		protected override void GenerateCharLiteral(char value)
		{
			if (value == char.MinValue)
				Write("'\\0'");
			else
			{
				string s = ReplaceChar(value);
				Write("'" + s + "'");
			}
		}
		protected override void GenerateNullLiteral()
		{
	  Write(FormattingTokenType.Null);
		}
	protected override void GenerateNumberLiteral(string name, object value, PrimitiveType type)
	{
	  if (!string.IsNullOrEmpty(name))
	  {
		Write(name);
		return;
	  }
	  string s = GetStringValue(value);
	  if (s == null)
		return;
	  s += CSharpPrimitiveTypeUtils.GetTypeSuffix(value, type);
	  Write(s);
	}
		#region GenerateTypeOfExpression
		protected override void GenerateTypeOfExpression(TypeOfExpression expression)
		{
	  Write(FormattingTokenType.TypeOf);
	  Write(FormattingTokenType.ParenOpen);
			CodeGen.GenerateElement(expression.TypeReference);
	  Write(FormattingTokenType.ParenClose);
		}
		#endregion
		#region GenerateTypeReferenceExpression
		protected override void GenerateTypeReferenceExpression(TypeReferenceExpression expression)
		{
			base.GenerateTypeReferenceExpression(expression);
			if (expression.IsUnbound)
			{
				int arity = expression.TypeArity;
		Write(FormattingTokenType.LessThan);
		for (int i = 0; i < arity - 1; i++)
		  Write(FormattingTokenType.Comma, i);
		Write(FormattingTokenType.GreaterThen);
			}
	  if (expression.IsNullable)
		Write(FormattingTokenType.Question);
		}
		#endregion
		#region GenerateUncheckedExpression
		protected override void GenerateUncheckedExpression(UncheckedExpression expression)
		{
	  Write(FormattingTokenType.Unchecked);
	  Write(FormattingTokenType.ParenOpen);
			CodeGen.GenerateElement(expression.Expression);
	  Write(FormattingTokenType.ParenClose);
		}
		#endregion
		#region GenerateTypeOfIsExpression
		protected override void GenerateTypeOfIsExpression(TypeOfIsExpression expression)
		{
			if (expression == null || expression.Expression == null || expression.TypeReference == null)
				return;
			CodeGen.GenerateElement(expression.Expression);
	  Write(FormattingTokenType.Is);
			CodeGen.GenerateElement(expression.TypeReference);
		}
		#endregion
		#region GenerateTypeCheck
		protected override void GenerateTypeCheck(TypeCheck expression)
		{
			CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.Is);
			CodeGen.GenerateElement(expression.RightSide);
		}
		#endregion
		#region GenerateConditionalTypeCast
		protected override void GenerateConditionalTypeCast(ConditionalTypeCast expression)
		{
			CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.As);
			CodeGen.GenerateElement(expression.RightSide);
		}
		#endregion
		protected override void GenerateDefaultValueExpression(DefaultValueExpression expression)
		{
	  Write(FormattingTokenType.Default);
	  Write(FormattingTokenType.ParenOpen);
			CodeGen.GenerateElement(expression.TypeReference);
	  Write(FormattingTokenType.ParenClose);
		}
		protected override void GenerateNullCoalescingExpression(NullCoalescingExpression expression)
		{
			CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.QuestionQuestion);
			CodeGen.GenerateElement(expression.RightSide);
		}
		protected override void GenerateQualifiedAliasExpression(QualifiedAliasExpression expression)
		{
	  if (expression.IsGlobal)
		Write(FormattingTokenType.Global);
	  else
		Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.ColonColon);
		}
		protected override void GenerateReferenceExpressionBase(ReferenceExpressionBase expression, string delimiter)
		{
			Expression lSource = expression.Qualifier;
			if (lSource != null)
			{
				CodeGen.GenerateElement(lSource);
		if (lSource.ElementType != LanguageElementType.QualifiedAliasExpression)
		  Write(FormattingTokenType.Dot);
			}
	  Write(FormattingTokenType.Ident);
	  if (expression.IsGeneric)
				GenerateGenericTypeArguments(expression.TypeArguments);
		}
	protected override void GeneratePointerElementReference(PointerElementReference expression)
		{
	  Expression source = expression.Qualifier;
			if (source != null)
			{
				CodeGen.GenerateElement(source);
		Write(FormattingTokenType.MinusGreaterThen);
			}
	  Write(FormattingTokenType.Ident);
			if (expression.IsGeneric)
				GenerateGenericTypeArguments(expression.TypeArguments);
		}
		protected override void GenerateMemberInitializerExpression(MemberInitializerExpression expression)
		{
			if (expression == null)
				return;
	  Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.Equal);
	  CodeGen.GenerateElement(expression.Value as LanguageElement);
		}
		protected override void GenerateObjectInitializerExpression(ObjectInitializerExpression expression)
		{
	  Write(FormattingTokenType.CurlyBraceOpen); 
	  GenerateElementCollection(expression.Initializers, FormattingTokenType.Comma);
	  Write(FormattingTokenType.CurlyBraceClose);
		}
	protected override void GenerateLambdaExpression(LambdaExpression expression)
	{
	  if (expression == null)
		return;
	  if (expression.IsAsynchronous)
		Write(FormattingTokenType.Async);
	  if (expression.ParameterCount > 0)
	  {
		bool hasParens = !expression.ParamOpenRange.IsEmpty && !expression.ParamCloseRange.IsEmpty;
		bool needParens = expression.ParameterCount > 1 || hasParens || !(expression.Parameters[0] is LambdaImplicitlyTypedParam);
		if (needParens)
		  GenerateParameters(expression.Parameters);
		else
		  CodeGen.GenerateElement(expression.Parameters[0]);
	  }
	  else
	  {
		Write(FormattingTokenType.ParenOpen);
		Write(FormattingTokenType.ParenClose);
	  }
	  Write(FormattingTokenType.EqualGreaterThen);
	  bool isSimleExpression = expression.NodeCount > 0 && expression.Nodes[0] is Expression;
	  if (isSimleExpression)
		CodeGen.GenerateElement(expression.Nodes[0] as LanguageElement);
	  else
	  {
		Write(FormattingTokenType.CurlyBraceOpen);
		GenerateElementCollection(expression.Nodes);
		Write(FormattingTokenType.CurlyBraceClose);
	  }
	}
		protected override void GenerateFromExpression(FromExpression expression)
		{
			if (expression == null)
				return;
			Write(FormattingTokenType.From);
			GenerateElementCollection(expression.InExpressions, FormattingTokenType.Comma, null);
		}
		protected override void GenerateJoinExpression(JoinExpression expression)
		{
			if (expression == null)
				return;
			GenerateJoinExpressionBase(expression);
		}
		protected override void GenerateJoinIntoExpression(JoinIntoExpression expression)
		{
			if (expression == null)
				return;
			GenerateJoinExpressionBase(expression);
			if (expression.IntoElements != null && expression.IntoElements.Count != 0)
			{
		Write(FormattingTokenType.Into);
				GenerateElementCollection(expression.IntoElements, FormattingTokenType.Comma, null);
			}
		}
		protected override void GenerateEqualsExpression(EqualsExpression expression)
		{
			if (expression == null || expression.LeftSide == null || expression.RightSide == null)
				return;
	  CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.Equals);
	  CodeGen.GenerateElement(expression.RightSide);
		}
		protected override void GenerateQueryExpression(QueryExpression expression)
		{
			if (expression == null)
				return;
	  FormattingTokenType type = !Options.WrappingAlignment.WrapQueryExpression && !SaveFormat && !Options.General.KeepExistingWhiteSpace
		? FormattingTokenType.WhiteSpace
		: FormattingTokenType.None;
	  GenerateElementCollection(expression.DetailNodes, type);
		}
		protected override void GenerateLetExpression(LetExpression expression)
		{
			if (expression == null)
				return;
			Write(FormattingTokenType.Let);
	  if (expression.Declarations != null && expression.Declarations.Count != 0)
		GenerateElementCollection(expression.Declarations, FormattingTokenType.Comma);
		}
		protected override void GenerateWhereExpression(WhereExpression expression)
		{
			if (expression == null)
				return;
			Write(FormattingTokenType.Where);
			CodeGen.GenerateElement(expression.WhereClause);
		}
		protected override void GenerateOrderingExpression(OrderingExpression expression)
		{
			if (expression == null)
				return;
			CodeGen.GenerateElement(expression.Ordering);
			OrderingType order = expression.Order;
			if (order != OrderingType.Default)
			{
				if (order == OrderingType.Ascending)
					Write(FormattingTokenType.Ascending);
				else
					Write(FormattingTokenType.Descending);
			}
		}
		protected override void GenerateOrderByExpression(OrderByExpression expression)
		{
			if (expression == null)
				return;
			Write(FormattingTokenType.OrderBy);
			GenerateElementCollection(expression.Orderings, FormattingTokenType.Comma);
		}
		protected override void GenerateSelectExpression(SelectExpression expression)
		{
			if (expression == null)
				return;
			Write(FormattingTokenType.Select);
	  if (expression.ReturnedElements != null && expression.ReturnedElements.Count != 0)
		GenerateElementCollection(expression.ReturnedElements, FormattingTokenType.Comma);
		}
		protected override void GenerateGroupByExpression(GroupByExpression expression)
		{
			if (expression == null)
				return;
			Write(FormattingTokenType.Group);
	  GenerateElementCollection(expression.GroupList, FormattingTokenType.Comma);
			Write(FormattingTokenType.By);
	  GenerateElementCollection(expression.ByList, FormattingTokenType.Comma);
		}
		protected override void GenerateIntoExpression(IntoExpression expression)
		{
			if (expression == null)
				return;
			Write(FormattingTokenType.Into);
	  CodeGen.GenerateElement(expression.IntoTarget);
		}
		protected override void GenerateInExpression(InExpression expression)
		{
			if (expression == null)
				return;
			if (expression.QueryIdent != null)
			{
		CodeGen.GenerateElement(expression.QueryIdent);
				Write(FormattingTokenType.In);
			}
			CodeGen.GenerateElement(expression.Expression);
		}
		protected override void GenerateDistinctExpression(DistinctExpression expression){}
	protected override void GenerateExpressionList(ExpressionCollectionBase expressions)
	{
	  GenerateExpressionList(expressions, FormattingTokenType.Comma);
	}
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (!(element is ConditionalExpression) || !Options.WrappingAlignment.WrapTernaryExpression)
		return false;
	  Code.DecreaseIndent();
	  return true;
	}
	}
}
