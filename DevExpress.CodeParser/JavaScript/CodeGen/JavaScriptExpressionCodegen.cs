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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
	public class JavaScriptExpressionCodeGen : TokenCExpressionCodeGenBase
	{
		public JavaScriptExpressionCodeGen(CodeGen codeGen)
			: base(codeGen)
		{
		}
 		#region Methods for generation expressions, which are'nt present in JavaScript
		protected override void GenerateArgumentDirection(ArgumentDirection direction)
		{
		}
		protected override void GenerateBaseReferenceExpression(BaseReferenceExpression expression)
		{
		}
		protected override void GenerateCheckedExpression(CheckedExpression expression)
		{
		}
		protected override void GenerateConditionalTypeCast(ConditionalTypeCast expression)
		{
		}
		protected override void GenerateCppQualifiedElementReference(CppQualifiedElementReference expression)
		{
		}
		protected override void GenerateDefaultValueExpression(DefaultValueExpression expression)
		{
		}
		protected override void GenerateDeleteArrayExpression(DeleteArrayExpression expression)
		{
		}
		protected override void GenerateDistinctExpression(DistinctExpression expression)
		{
		}
		protected override void GenerateElaboratedTypeReference(ElaboratedTypeReference expression)
		{
		}
		protected override void GenerateEqualsExpression(EqualsExpression expression)
		{
		}
		protected override void GenerateFromExpression(FromExpression expression)
		{
		}
		protected override void GenerateGenericTypeArguments(TypeReferenceExpressionCollection arguments)
		{
		}
		protected override void GenerateGroupByExpression(GroupByExpression expression)
		{
		}
		protected override void GenerateInExpression(InExpression expression)
		{
		}
		protected override void GenerateIntoExpression(IntoExpression expression)
		{
		}
		protected override void GenerateJoinExpression(JoinExpression expression)
		{
		}
		protected override void GenerateJoinIntoExpression(JoinIntoExpression expression)
		{
		}
		protected override void GenerateLambdaExpression(LambdaExpression expression)
		{
		}
		protected override void GenerateLetExpression(LetExpression expression)
		{
		}
		protected override void GenerateManagedArrayCreateExpression(ManagedArrayCreateExpression expression)
		{
		}
		protected override void GenerateManagedObjectCreationExpression(ManagedObjectCreationExpression expression)
		{
		}
		protected override void GenerateNullCoalescingExpression(NullCoalescingExpression expression)
		{
		}
		protected override void GenerateOrderByExpression(OrderByExpression expression)
		{
		}
		protected override void GenerateOrderingExpression(OrderingExpression expression)
		{
		}
		protected override void GenerateParametizedArrayCreateExpression(ParametrizedArrayCreateExpression expression)
		{
		}
		protected override void GenerateParametizedObjectCreationExpression(ParametrizedObjectCreationExpression expression)
		{
		}
		protected override void GeneratePointerElementReference(PointerElementReference expression)
		{
		}
		protected override void GeneratePointerMethodReference(PointerMethodReference expression)
		{
		}
		protected override void GenerateQualifiedAliasExpression(QualifiedAliasExpression expression)
		{
		}
		protected override void GenerateQualifiedMethodReference(QualifiedMethodReference expression)
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
		protected override void GenerateQueryExpression(QueryExpression expression)
		{
		}
		protected override void GenerateSelectExpression(SelectExpression expression)
		{
		}
		protected override void GenerateUncheckedExpression(UncheckedExpression expression)
		{
		}
		protected override void GenerateWhereExpression(WhereExpression expression)
		{
		}
		#endregion
	#region GenerateArgumentDirectionExpression
	protected override void GenerateArgumentDirectionExpression(ArgumentDirectionExpression expression)
	{
	  if (expression == null)
		return;
	  CodeGen.GenerateElement(expression.Expression);
	}
	#endregion
	private void GenerateAnonymousConstructorExpression(AnonymousConstructorExpression expression)
	{
	  Write(FormattingTokenType.New);
	  GenerateAnonymousMethodExpression(expression);
	}
	private void GenerateParenthesizedTypeReferenceExpression(ParenthesizedTypeReferenceExpression expression)
	{
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.Expression);
	  Write(FormattingTokenType.ParenClose);
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
	#region GenerateAnonymousMethodExpression
	protected override void GenerateAnonymousMethodExpression(AnonymousMethodExpression expression)
		{
			Write(FormattingTokenType.Function);
			if (!string.IsNullOrEmpty(expression.Name))
				Write(FormattingTokenType.Ident);
			GenerateParameters(expression.Parameters);			
	  Write(FormattingTokenType.CurlyBraceOpen);
	  GenerateElementCollection(expression.Nodes);	  
	  Write(FormattingTokenType.CurlyBraceClose);
		}
		#endregion
		#region GenerateArrayInitializerExpression
		protected override void GenerateArrayInitializerExpression(ArrayInitializerExpression expression)
		{
			if (expression == null)
				return;
	  Write(FormattingTokenType.BracketOpen);
	  GenerateElementCollection(expression.Initializers,FormattingTokenType.Comma);
	  Write(FormattingTokenType.BracketClose);
		}
		#endregion
		#region GenerateExpression
		protected override bool GenerateExpression(Expression expression)
		{
			bool result = false;
			if (expression == null)
				return result;
	  switch (expression.ElementType)
	  {
		case LanguageElementType.EmptyArrayElementExpression:
		  GenerateEmptyArrayElementExpression(expression as EmptyArrayElementExpression);
		  return true;
		case LanguageElementType.AnonymousConstructorExpression:
		  GenerateAnonymousConstructorExpression(expression as AnonymousConstructorExpression);
		  return true;
		case LanguageElementType.ParenthesizedTypeReferenceExpression:
		  GenerateParenthesizedTypeReferenceExpression(expression as ParenthesizedTypeReferenceExpression);
		  return true;
	  }
			return base.GenerateExpression(expression);
		}
		#endregion
		#region GenerateBooleanLiteral
		protected override void GenerateBooleanLiteral(bool value)
		{
			if (value)
		Write(FormattingTokenType.True);
			else 
		Write(FormattingTokenType.False);
		}
		#endregion
		#region GenerateCharLiteral
		protected override void GenerateCharLiteral(char value)
		{
	  Write(String.Format("'{0}'", value));
		}
		#endregion
		#region GenerateComplexExpression
		protected override void GenerateComplexExpression(ComplexExpression expression)
		{
			if (expression == null) 
				return;
			ExpressionCollection nestedExpressions = expression.Expressions;
			if (nestedExpressions == null || nestedExpressions.Count <= 0)
				return;
	  GenerateElementCollection(nestedExpressions, FormattingTokenType.Comma);
		}
		#endregion
		#region GenerateDeleteExpression
		protected override void GenerateDeleteExpression(DeleteExpression expression)
		{
			if (expression == null)
				return;
	  Write(FormattingTokenType.Delete);
	  CodeGen.GenerateElement(expression.Expression);
		}
		#endregion
		#region GenerateEmptyArrayElementExpression
		protected void GenerateEmptyArrayElementExpression(EmptyArrayElementExpression expression)
		{
			if (expression == null)
				return;			
			string commas = new String(',', expression.EmptyElementsCount - 1);
			Write(commas);
		}
		#endregion
		#region GenerateMemberInitializerExpression
		protected override void GenerateMemberInitializerExpression(MemberInitializerExpression expression)
		{
			if (expression == null || expression.Name == null)
				return;
			Write(String.Format("{0} : ", expression.Name));
			CodeGen.GenerateElement(expression.Value);
		}
		#endregion
		#region GenerateNullLiteral
		protected override void GenerateNullLiteral()
		{
			Write(FormattingTokenType.Null);
		}
		#endregion
		#region GenerateNumberLiteral
		protected override void GenerateNumberLiteral(string name, object value, PrimitiveType type)
		{
			Write(name);
		}
		#endregion
		#region GenerateObjectInitializerExpression
		protected override void GenerateObjectInitializerExpression(ObjectInitializerExpression expression)
		{
			if (expression == null)
				return;	
			Write(FormattingTokenType.CurlyBraceOpen);
	  GenerateElementCollection(expression.Initializers, FormattingTokenType.Comma);
			Write(FormattingTokenType.CurlyBraceClose);
		}
		#endregion
		#region GenerateStringLiteral
		protected override void GenerateStringLiteral(string value)
		{
			Write(value);
		}
		#endregion
		#region GenerateTypeCheck
		protected override void GenerateTypeCheck(TypeCheck expression)
		{
			if (expression == null)
				return;
	  CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.InstanceOf);
	  CodeGen.GenerateElement(expression.RightSide);
		}
		#endregion
		#region GenerateTypeOfExpression
		protected override void GenerateTypeOfExpression(TypeOfExpression expression)
		{
			if (expression == null)
				return;
	  Write(FormattingTokenType.TypeOf);
	  CodeGen.GenerateElement(expression.Expression);
		}
		#endregion
		#region GenerateUnaryOperatorExpression
		protected override void GenerateUnaryOperatorExpression(UnaryOperatorExpression expression)
		{
			if (expression == null)
				return;
			if (expression.OperatorText != "void")
				base.GenerateUnaryOperatorExpression(expression);
			else
			{
		Write(FormattingTokenType.Void);
		CodeGen.GenerateElement(expression.Expression);
			}
		}
		#endregion
		#region IsEscapedString
		protected override bool IsEscapedString(string value)
		{
			return true;
		}
		#endregion
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
				case FormattingTokenType.Ident:
					if (Context.ElementType == LanguageElementType.AnonymousMethodExpression)
						result.AddWhiteSpace();
					break;
		case FormattingTokenType.CurlyBraceClose:
		  if (Context.ElementType == LanguageElementType.AnonymousMethodExpression)
			Code.DecreaseIndent();
		  break;
		case FormattingTokenType.GreatThanGreatThanGreatThan:
		case FormattingTokenType.InstanceOf:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.CurlyBraceOpen:
		  if (Context.ElementType == LanguageElementType.AnonymousMethodExpression)
			Code.IncreaseIndent();
		  break;
		case FormattingTokenType.Comma:
		case FormattingTokenType.Delete:
		case FormattingTokenType.GreatThanGreatThanGreatThan:
		case FormattingTokenType.New:
		case FormattingTokenType.TypeOf:
		case FormattingTokenType.InstanceOf:
		case FormattingTokenType.Void:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override void GenerateParameters(ICollection parameters)
	{
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(parameters, GetCommaDelimeter());
	  Write(FormattingTokenType.ParenClose);
	}
	}
}
