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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  using Xml;
  public class VBExpressionCodeGen : TokenExpressionCodeGenBase
  {
	CodeGen _HtmlCodeGen = null;
	public VBExpressionCodeGen(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	string DeleteLastEOL(string str)
	{
	  if (str == null || str == String.Empty || str.Length < 3)
		return String.Empty;
	  int len = str.Length - 2;
	  if (len <= 0)
	  {
		return String.Empty;
	  }
	  str = str.Substring(0, len);
	  return str;
	}
	bool IsNotNull(NodeList list)
	{
	  return list != null && list.Count != 0;
	}
	bool ParentIsValidForArrayBoundsGeneration(Expression exp)
	{
	  if (exp == null)
		return false;
	  while (true)
	  {
		LanguageElement parent = exp.Parent as Expression;
		if (parent == null)
		  return false;
		if (parent.ElementType == LanguageElementType.ObjectCreationExpression || parent.ElementType == LanguageElementType.ArrayCreateExpression)
		  return true;
		exp = parent as Expression;
	  }
	}
	void GenerateTypeCharacter(string name)
	{
	  if (string.IsNullOrEmpty(name))
		return;
	  switch (name.ToLower())
	  {
		case "integer":
		  Write(FormattingTokenType.Percent);
		  break;
		case "long":
		  Write(FormattingTokenType.Ampersand);
		  break;
		case "single":
		  Write(FormattingTokenType.Exclamation);
		  break;
		case "double":
		  Write(FormattingTokenType.NumberSign);
		  break;
		case "decimal":
		  Write(FormattingTokenType.At);
		  break;
		case "string":
		  Write(FormattingTokenType.DollarSign);
		  break;
		default:
		  break;
	  }
	}
	void GeneratesIntoTail(IIntoContainingElement element)
	{
	  if (element == null)
		return;
	  Write(FormattingTokenType.Into);
	  GenerateElementCollection(element.IntoElements, FormattingTokenType.Comma);
	}
	void GenerateLambdaFunctionExpression(LambdaFunctionExpression expression)
	{
	  Write(FormattingTokenType.Function);
	  GenerateParameters(expression.Parameters);
	  if (expression.Type != null)
	  {
		Write(FormattingTokenType.As);
		CodeGen.GenerateElement(expression.Type);
	  }
	  if (expression.NodeCount < 1)
		return;
	  bool isMultyLine = true;
	  Expression expr = expression.Nodes[0] as Expression;
	  isMultyLine = !(expression.NodeCount == 1 && expr != null) || expression.Type != null;
	  if (isMultyLine)
	  {
		CodeGen.AddNewLineIfNeeded();
		using (GetIndent())
		  GenerateElementCollection(expression.Nodes, FormattingTokenType.None, true);
		Write(FormattingTokenType.End, FormattingTokenType.Function);
	  }
	  else
	  {
		CodeGen.AddWSIfNeeded();
		CodeGen.GenerateElement(expr);
	  }
	}
	void WrapQuery()
	{
	  if (Options.WrappingAlignment.WrapQueryExpression)
		Write(FormattingTokenType.LineContinuation);
	  else
		CodeGen.AddWSIfNeeded();
	}
	void GenerateJoinExpressionBase(JoinExpressionBase expression)
	{
	  if (expression == null)
		return;
	  CodeGen.GenerateElement(expression.InExpression);
	  WrapQuery();
	  if (expression.JoinExpressions != null && expression.JoinExpressions.Count > 0)
	  {
		if (Options.WrappingAlignment.WrapQueryExpression)
		  GenerateElementCollection(expression.JoinExpressions, FormattingTokenType.LineContinuation);
		else
		  GenerateElementCollection(expression.JoinExpressions);
		WrapQuery();
	  }
	  Write(FormattingTokenType.On);
	  if (Options.WrappingAlignment.WrapQueryExpression)
		GenerateElementCollection(expression.EqualsExpressions, new FormattingTokenType[] { FormattingTokenType.LineContinuation, FormattingTokenType.And });
	  else
		GenerateElementCollection(expression.EqualsExpressions, FormattingTokenType.And);
	}
	void GenerateAwaitExpression(AwaitExpression awaitExpression)
	{
	  Write(FormattingTokenType.Await);
	  CodeGen.GenerateElement(awaitExpression.SourceExpression);
	}
	void GenerateQueryaleCollectionReferenceExpression(QueryableCollectionReferenceExpression expression)
	{
	  if (expression == null)
		return;
	  CodeGen.GenerateElement(expression.Expression);
	}
	CodeGen HtmlCodeGen
	{
	  get
	  {
		if (_HtmlCodeGen == null)
		  _HtmlCodeGen = new HtmlXmlCodeGen(Options);
		return _HtmlCodeGen;
	  }
	}
	protected override bool GenerateMacroExpression(Expression expression)
	{
	  return false;
	}
	protected override bool GenerateSpecificExpression(Expression expression)
	{
	  switch (expression.ElementType)
	  {
		case LanguageElementType.DirectCastExpression:
		  GenerateDirectCastExpression(expression as DirectCastExpression);
		  return true;
		case LanguageElementType.CTypeExpression:
		  GenerateCTypeExpression(expression as CTypeExpression);
		  return true;
		case LanguageElementType.CastTargetExpression:
		  GenerateCastTargetExpression(expression as CastTargetExpression);
		  return true;
		case LanguageElementType.Is:
		  GenerateIsExpression(expression as Is);
		  return true;
		case LanguageElementType.IsNot:
		  GenerateTypeCheck(expression as IsNot);
		  return true;
		case LanguageElementType.ObjectCollectionInitializer:
		  GenerateObjectCollectionInitializer(expression as ObjectCollectionInitializer);
		  return true;
		case LanguageElementType.VbXmlExpression:
		  GenerateXmlExpression(expression as XmlExpression);
		  return true;
		case LanguageElementType.AggregateExpression:
		  GenerateAggregateExpression(expression as AggregateExpression);
		  return true;
		case LanguageElementType.SkipExpression:
		  GenerateSkipExpression(expression as SkipExpression);
		  return true;
		case LanguageElementType.SkipWhileExpression:
		  GenerateSkipWhileExpression(expression as SkipWhileExpression);
		  return true;
		case LanguageElementType.TakeExpression:
		  GenerateTakeExpression(expression as TakeExpression);
		  return true;
		case LanguageElementType.TakeWhileExpression:
		  GenerateTakeWhileExpression(expression as TakeWhileExpression);
		  return true;
		case LanguageElementType.XmlAttributeReference:
		  GenerateXmlAttributeReferenceExpression(expression as XmlAttributeReferenceExpression);
		  return true;
		case LanguageElementType.XmlElementReference:
		  GenerateXmlElementReferenceExpression(expression as XmlElementReferenceExpression);
		  return true;
		case LanguageElementType.ReDimExpression:
		  GenerateReDimExpression(expression as ReDimExpression);
		  return true;
		case LanguageElementType.LambdaFunctionExpression:
		  GenerateLambdaFunctionExpression(expression as LambdaFunctionExpression);
		  return true;
		case LanguageElementType.GetXmlNamespaceOperator:
		  GenerateMethodCallExpression(expression as MethodCallExpression);
		  return true;
		case LanguageElementType.AwaitExpression:
		  GenerateAwaitExpression(expression as AwaitExpression);
		  return true;
		case LanguageElementType.QueryableCollectionReferenceExpression:
		  GenerateQueryaleCollectionReferenceExpression(expression as QueryableCollectionReferenceExpression);
		  return true;
	  }
	  return false;
	}
	protected override void GenerateBinaryOperator(BinaryOperatorExpression expression)
	{
	  FormattingTokenType binaryType = GetBinaryOperatorToken(expression.BinaryOperator);
	  if (binaryType == FormattingTokenType.None && expression.OperatorText != null)
	  {
		string opText = String.Format(" {0} ", expression.OperatorText);
		Write(opText);
		return;
	  }
	  Write(binaryType);
	}
	protected override void GenerateLogicalOperation(LogicalOperation expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  CodeGen.AddWSIfNeeded();
	  FormattingTokenType binaryType = GetBinaryOperatorToken(expression.BinaryOperator);
	  if (binaryType == FormattingTokenType.None && expression.OperatorText != null)
	  {
		string opText = String.Format(" {0} ", expression.OperatorText);
		Write(opText);
		return;
	  }
	  Write(binaryType);
	  CodeGen.AddWSIfNeeded();
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override string GetBinaryOperatorText(BinaryOperatorType operatorType)
	{
	  return string.Empty;
	}
	protected override FormattingTokenType GetBinaryOperatorToken(BinaryOperatorType op)
	{
	  switch (op)
	  {
		case BinaryOperatorType.Add:
		  return FormattingTokenType.Plus;
		case BinaryOperatorType.Assign:
		  return FormattingTokenType.Equal;
		case BinaryOperatorType.BitwiseAnd:
		  return FormattingTokenType.And;
		case BinaryOperatorType.BitwiseOr:
		  return FormattingTokenType.Or;
		case BinaryOperatorType.BooleanAnd:
		  return FormattingTokenType.AndAlso;
		case BinaryOperatorType.BooleanOr:
		  return FormattingTokenType.OrElse;
		case BinaryOperatorType.ExclusiveOr:
		  return FormattingTokenType.Xor; 
		case BinaryOperatorType.Divide:
		  return FormattingTokenType.Slash; 
		case BinaryOperatorType.GreaterThan:
		  return FormattingTokenType.GreaterThen; 
		case BinaryOperatorType.GreaterThanOrEqual:
		  return FormattingTokenType.GreaterThenEqual; 
		case BinaryOperatorType.IdentityEquality:
		  return FormattingTokenType.Equal; 
		case BinaryOperatorType.IdentityInequality:
		  return FormattingTokenType.LessThanGreaterThan;
		case BinaryOperatorType.LessThan:
		  return FormattingTokenType.LessThan; 
		case BinaryOperatorType.LessThanOrEqual:
		  return FormattingTokenType.LessThanEqual;
		case BinaryOperatorType.Modulus:
		  return FormattingTokenType.Mod;
		case BinaryOperatorType.Multiply:
		  return FormattingTokenType.Asterisk;
		case BinaryOperatorType.Subtract:
		  return FormattingTokenType.Minus;
		case BinaryOperatorType.ValueEquality:
		  return FormattingTokenType.Equal;
		case BinaryOperatorType.Like:
		  return FormattingTokenType.Like;
		case BinaryOperatorType.Concatenation:
		  return FormattingTokenType.Ampersand;
		case BinaryOperatorType.Exponentiation:
		  return FormattingTokenType.Caret;
		case BinaryOperatorType.IntegerDivision:
		  return FormattingTokenType.BackSlash;
		case BinaryOperatorType.ShiftLeft:
		  return FormattingTokenType.LessThanLessThan;
		case BinaryOperatorType.ShiftRight:
		  return FormattingTokenType.GreatThanGreatThan;
	  }
	  return FormattingTokenType.None;
	}
	protected override void GenerateTypedElementReferenceExpression(TypedElementReferenceExpression expression)
	{
	  if (expression == null)
		return;
	  GenerateElementReferenceExpression(expression);
	  Write(expression.TypeCharacter.ToString());
	}
	protected override void GenerateMyClassExpression(MyClassExpression expression)
	{
	  Write(FormattingTokenType.MyClass);
	}
	protected override void GenerateElementReferenceExpression(ElementReferenceExpression expression)
	{
	  if (expression == null)
		return;
	  if (expression.IsKey)
		Write(FormattingTokenType.Key); 
	  base.GenerateElementReferenceExpression(expression);
	}
	protected override void GenerateManagedArrayCreateExpression(ManagedArrayCreateExpression expression)
	{
	}
	protected override void GenerateCppQualifiedElementReference(CppQualifiedElementReference expression)
	{
	}
	protected override void GenerateDeleteArrayExpression(DeleteArrayExpression expression)
	{
	}
	protected override void GenerateComplexExpression(ComplexExpression expression)
	{
	}
	protected override void GenerateExpressionTypeArgument(ExpressionTypeArgument expression)
	{
	}
	protected override void GenerateDeleteExpression(DeleteExpression expression)
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
	protected override void GeneratePointerElementReference(PointerElementReference expression)
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
	protected override void GenerateQualifiedMethodReference(QualifiedMethodReference expression)
	{
	}
	protected override void GeneratePointerMethodReference(PointerMethodReference expression)
	{
	}
	protected override void GenerateGenericTypeArguments(TypeReferenceExpressionCollection arguments)
	{
	  Write(FormattingTokenType.ParenOpen);
	  Write(FormattingTokenType.Of);
	  GenerateElementCollection(arguments, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	}
	protected override void GenerateEqualsExpression(EqualsExpression expression)
	{
	  if (expression == null)
		return;
	  CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.Equals);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override void GenerateGroupByExpression(GroupByExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Group);
	  GenerateElementCollection(expression.GroupList, FormattingTokenType.Comma);
	  WrapQuery();
	  Write(FormattingTokenType.By);
	  GenerateElementCollection(expression.ByList, FormattingTokenType.Comma);
	  WrapQuery();
	  GeneratesIntoTail(expression);
	}
	protected override void GenerateJoinIntoExpression(JoinIntoExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Group);
	  Write(FormattingTokenType.Join);
	  GenerateJoinExpressionBase(expression);
	  WrapQuery();
	  GeneratesIntoTail(expression);
	}
	protected override void GenerateJoinExpression(JoinExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Join);
	  GenerateJoinExpressionBase(expression);
	}
	protected override void GenerateAddressOfExpression(AddressOfExpression expression)
	{
	  Write(FormattingTokenType.AddressOf); 
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected override void GenerateArgumentDirectionExpression(ArgumentDirectionExpression expression)
	{
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected override void GenerateArrayCreateExpression(ArrayCreateExpression expression)
	{
	  Write(FormattingTokenType.New);
	  CodeGen.GenerateElement(expression.BaseType);
	  bool lBaseTypeIsArray = expression.BaseType != null && expression.BaseType.IsArrayType;
	  if (!lBaseTypeIsArray)
		Write(FormattingTokenType.ParenOpen);
	  if (expression.Dimensions != null && expression.Dimensions.Count > 0)
	  {
		if (lBaseTypeIsArray)
		  Write(FormattingTokenType.ParenOpen);
		GenerateElementCollection(expression.Dimensions, FormattingTokenType.Comma);
		if (lBaseTypeIsArray)
		  Write(FormattingTokenType.ParenClose);
	  }
	  if (!lBaseTypeIsArray)
		Write(FormattingTokenType.ParenClose);
	  CodeGen.GenerateElement(expression.Initializer);
	}
	protected override void GenerateArrayInitializerExpression(ArrayInitializerExpression expression)
	{
	  bool breakBefore = Options.LineBreaks.PlaceOpenBraceOnNewLineForArrayObjectAndCollInitializers;
	  bool breakAfter = Options.LineBreaks.PlaceCloseBraceOnNewLineForArrayObjectAndCollInitializers;
	  using (GetIndent(breakBefore))
	  {
		if (breakBefore)
		  Write(FormattingTokenType.LineContinuation);
		Write(FormattingTokenType.CurlyBraceOpen);
		GenerateElementCollection(expression.Initializers, FormattingTokenType.Comma);
		if (breakAfter)
		  Write(FormattingTokenType.LineContinuation);
		Write(FormattingTokenType.CurlyBraceClose);
	  }
	}
	protected override void GenerateAssignmentExpression(AssignmentExpression expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  if (expression.AssignmentOperator == AssignmentOperatorType.PercentEquals)
	  {
		GenerateAssignmentOperatorText(CodeGen, AssignmentOperatorType.Assignment);
		CodeGen.GenerateElement(expression.LeftSide);
		Write(FormattingTokenType.Mod);
	  }
	  else
		GenerateAssignmentOperatorText(CodeGen, expression.AssignmentOperator);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override void GenerateBaseReferenceExpression(BaseReferenceExpression expression)
	{
	  Write(FormattingTokenType.MyBase);
	}
	protected override void GenerateConditionalExpression(ConditionalExpression expression)
	{
	  Write(FormattingTokenType.If);
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.Condition);
	  if (expression.TrueExpression != null)
	  {
		Write(FormattingTokenType.Comma);
		CodeGen.GenerateElement(expression.TrueExpression);
	  }
	  if (expression.FalseExpression != null)
	  {
		Write(FormattingTokenType.Comma);
		CodeGen.GenerateElement(expression.FalseExpression);
	  }
	  Write(FormattingTokenType.ParenClose);
	}
	protected override void GenerateIndexerExpression(IndexerExpression expression)
	{
	  CodeGen.GenerateElement(expression.Qualifier);
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(expression.Arguments, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	}
	protected override void GenerateObjectCreationExpression(ObjectCreationExpression expression)
	{
	  Write(FormattingTokenType.New);
	  if (expression.ObjectType != null)
	  {
		CodeGen.GenerateElement(expression.ObjectType);
		Write(FormattingTokenType.ParenOpen);
		GenerateElementCollection(expression.Arguments, FormattingTokenType.Comma);
		Write(FormattingTokenType.ParenClose);
	  }
	  CodeGen.GenerateElement(expression.ObjectInitializer);
	}
	protected override void GenerateBooleanLiteral(bool value)
	{
	  Write(value ? FormattingTokenType.True : FormattingTokenType.False);
	}
	protected override bool IsEscapedString(string value)
	{
	  return false;
	}
	protected override void GenerateStringLiteral(string value)
	{
	  GenerateStringLiteralCore(value, false);
	}
	protected override void GenerateStringLiteralWithEscapeSequence(string value)
	{
	  GenerateStringLiteralCore(value, true);
	}
	protected override void GenerateCharLiteral(char value)
	{
	  if (value == char.MinValue)
	  {
		Write(FormattingTokenType.Char, FormattingTokenType.Dot);
		Write("MinValue");
	  }
	  else
		if (Char.IsLetterOrDigit(value) || Char.IsPunctuation(value))
		  Write("\"" + value + "\"C");
		else
		  Write(String.Format("Microsoft.VisualBasic.ChrW({0})", (int)value));
	}
	protected override void GenerateNullLiteral()
	{
	  Write(FormattingTokenType.Nothing);
	}
	protected override void GenerateNumberLiteral(string name, object value, PrimitiveType type)
	{
	  if (name != null && name != String.Empty)
		Write(name);
	  else
	  {
		string s = GetStringValue(value);
		if (s == null)
		  return;
		s += VBPrimitiveTypeUtils.GetTypeSuffix(value, type);
		Write(s);
	  }
	}
	protected override void GenerateDateTime(object value)
	{
	  if ((DateTime)value == DateTime.MinValue)
		Write("#1/1/1#");
	  else
	  {
		DateTime dtValue = (DateTime)value;
		Write(FormattingTokenType.NumberSign);
		bool dateWasGenerated = false;
		if (dtValue.Year != 1 || dtValue.Month != 1 || dtValue.Day != 1)
		{
		  Write(String.Format("{0}/{1}/{2}", dtValue.Month, dtValue.Day, dtValue.Year));
		  dateWasGenerated = true;
		}
		if (dtValue.Hour > 0 || dtValue.Minute > 0)
		{
		  if (dateWasGenerated)
			Write(" ");
		  Write(String.Format("{0}:{1}", dtValue.Hour, dtValue.Minute));
		  if (dtValue.Second > 0)
			Write(String.Format(":{0}", dtValue.Second));
		}
		Write(FormattingTokenType.NumberSign);
	  }
	}
	protected override void GenerateThisReferenceExpression(ThisReferenceExpression expression)
	{
	  Write(FormattingTokenType.Me);
	}
	protected override void GenerateTypeCastExpression(TypeCastExpression expression)
	{
	  GenerateCTypeExpression(expression.Target, expression.TypeReference);
	}
	protected override void GenerateTypeOfExpression(TypeOfExpression expression)
	{
	  Write(FormattingTokenType.GetType, FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.TypeReference);
	  Write(FormattingTokenType.ParenClose);
	}
	protected override void GenerateTypeOfIsExpression(TypeOfIsExpression expression)
	{
	  Write(FormattingTokenType.TypeOf);
	  CodeGen.GenerateElement(expression.Expression);
	  Write(FormattingTokenType.Is);
	  CodeGen.GenerateElement(expression.TypeReference);
	}
	protected override void GenerateTypeReferenceExpression(TypeReferenceExpression expression)
	{
	  if (expression.BaseType != null)
		CodeGen.GenerateElement(expression.BaseType);
	  if (expression.Name != null && expression.Name.Length > 0)
	  {
		if (expression.BaseType != null)
		  Write(FormattingTokenType.Dot);
		if (expression.IsTypeCharacter && expression.BaseType == null)
		  GenerateTypeCharacter(expression.Name);
		else
		  GenerateTypeString(expression.Name);
	  }
	  if (expression.IsGeneric)
		GenerateGenericTypeArguments(expression.TypeArguments);
	  if (expression.IsNullable)
		Write(FormattingTokenType.Question);
	  else
		if (expression.IsUnbound)
		{
		  int arity = expression.TypeArity;
		  Write(FormattingTokenType.ParenOpen, FormattingTokenType.Of);
		  for (int i = 0; i < arity - 1; i++)
			Write(FormattingTokenType.Comma);
		  Write(FormattingTokenType.ParenClose);
		}
		else
		  if (expression.IsArrayType)
		  {
			bool isValidForArrayBounds = ParentIsValidForArrayBoundsGeneration(expression);
			Write(FormattingTokenType.ParenOpen);
			int rank = expression.Rank;
			ExpressionCollection coll = expression.ArrayBounds;
			int arrayBounds = 0;
			if (isValidForArrayBounds && coll != null)
			  arrayBounds = coll.Count;
			for (int i = 0; i < rank; i++)
			{
			  if (i != 0)
				Write(FormattingTokenType.Comma);
			  if (arrayBounds <= i)
				continue;
			  if (i != 0 && Options.Spacing.AfterComma)
				CodeGen.AddWSIfNeeded();
			  CodeGen.GenerateElement(coll[i]);
			}
			Write(FormattingTokenType.ParenClose);
		  }
	}
	protected override void GenerateLogicalInversion(LogicalInversion expression)
	{
	  Write(FormattingTokenType.Not);
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected override void GenerateTypeCheck(TypeCheck expression)
	{
	  Write(FormattingTokenType.TypeOf);
	  CodeGen.GenerateElement(expression.LeftSide);
	  if (expression is IsNot)
		Write(FormattingTokenType.IsNot);
	  else
		Write(FormattingTokenType.Is);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override void GenerateUnaryDecrement(UnaryDecrement expression)
	{
	  CodeGen.GenerateElement(expression.Expression);
	  if (expression.IsPostOperator)
		Write(" -= 1");
	}
	protected override void GenerateUnaryIncrement(UnaryIncrement expression)
	{
	  CodeGen.GenerateElement(expression.Expression);
	  if (expression.IsPostOperator)
		Write(" += 1");
	}
	protected override void GenerateConditionalTypeCast(ConditionalTypeCast expression)
	{
	  if (expression == null)
		return;
	  Expression leftSide = expression.LeftSide;
	  Expression rightSide = expression.RightSide;
	  if (expression.IsIfOperator)
	  {
		Write(FormattingTokenType.IIf, FormattingTokenType.ParenOpen, FormattingTokenType.TypeOf);
		CodeGen.GenerateElement(leftSide);
		Write(FormattingTokenType.Is);
		CodeGen.GenerateElement(rightSide);
		Write(FormattingTokenType.Comma, FormattingTokenType.CType, FormattingTokenType.ParenOpen);
		CodeGen.GenerateElement(leftSide);
		Write(FormattingTokenType.Comma);
		CodeGen.GenerateElement(rightSide);
		Write(FormattingTokenType.ParenClose, FormattingTokenType.Comma, FormattingTokenType.Nothing, FormattingTokenType.ParenClose);
	  }
	  else
	  {
		Write(FormattingTokenType.TryCast, FormattingTokenType.ParenOpen);
		CodeGen.GenerateElement(leftSide);
		Write(FormattingTokenType.Comma);
		CodeGen.GenerateElement(rightSide);
		Write(FormattingTokenType.ParenClose);
	  }
	}
	protected override void GenerateParenthesizedExpression(ParenthesizedExpression expression)
	{
	  if (expression.Expression is TypeCastExpression)
		CodeGen.GenerateElement(expression.Expression);
	  else
		base.GenerateParenthesizedExpression(expression);
	}
	protected override void GenerateIsNot(IsNot expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.IsNot);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override void GenerateAttributeVariableInitializer(AttributeVariableInitializer expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.ColonEquals);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override void GenerateFromExpression(FromExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.From);
	  GenerateElementCollection(expression.InExpressions, FormattingTokenType.Comma);
	}
	protected override void GenerateInExpression(InExpression expression)
	{
	  if (expression == null)
		return;
	  if (expression.QueryIdent != null)
	  {
		CodeGen.GenerateElement(expression.QueryIdent);
		if (expression.InExpressionType == InExpressionType.LetExpression)
		  Write(FormattingTokenType.Equal);
		else
		  Write(FormattingTokenType.In);
	  }
	  CodeGen.GenerateElement(expression.Expression);
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
	  GenerateElementCollection(expression.ReturnedElements, FormattingTokenType.Comma);
	}
	protected override void GenerateIntoExpression(IntoExpression expression)
	{
	}
	protected override void GenerateArgumentDirection(ArgumentDirection direction)
	{
	}
	protected override void GenerateCheckedExpression(CheckedExpression expression)
	{
	}
	protected override void GenerateUncheckedExpression(UncheckedExpression expression)
	{
	}
	protected override void GenerateSizeOfExpression(SizeOfExpression expression)
	{
	}
	protected override void GenerateAnonymousMethodExpression(AnonymousMethodExpression expression)
	{
	}
	protected override void GenerateDefaultValueExpression(DefaultValueExpression expression)
	{
	}
	protected override void GenerateNullCoalescingExpression(NullCoalescingExpression expression)
	{
		if (expression == null)
			return;
		Write(FormattingTokenType.If);
		Write(FormattingTokenType.ParenOpen);
		CodeGen.GenerateElement(expression.LeftSide);
		Write(FormattingTokenType.Comma);
		CodeGen.GenerateElement(expression.RightSide);
		Write(FormattingTokenType.ParenClose);
	}
	protected override void GenerateQualifiedAliasExpression(QualifiedAliasExpression expression)
	{
	}
	protected override void GenerateMemberInitializerExpression(MemberInitializerExpression expression)
	{
	  if (expression.IsKey)
		Write(FormattingTokenType.Key);
	  Write(FormattingTokenType.Dot);
	  Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.Equal);
	  CodeGen.GenerateElement(expression.Value);
	}
	protected override void GenerateObjectInitializerExpression(ObjectInitializerExpression expression)
	{
	  if (expression.ElementType == LanguageElementType.ObjectCollectionInitializer)
		Write(FormattingTokenType.From);
	  else
		Write(FormattingTokenType.With);
	  Write(FormattingTokenType.CurlyBraceOpen);
	  GenerateElementCollection(expression.Initializers, FormattingTokenType.Comma);
	  Write(FormattingTokenType.CurlyBraceClose);
	}
	protected override void GenerateLambdaExpression(LambdaExpression expression)
	{
	  if (expression.IsAsynchronous)
		Write(FormattingTokenType.Async);
	  if (expression.IsFunction)
		Write(FormattingTokenType.Function);
	  else
		Write(FormattingTokenType.Sub);
	  GenerateParameters(expression.Parameters);
	  bool multiLine = expression.Range.LineCount > 1 || expression.NodeCount > 1;
	  if (multiLine)
		CodeGen.AddNewLineIfNeeded();
	  else
		CodeGen.AddWSIfNeeded();
	  using (GetIndent())
		GenerateElementCollection(expression.Nodes, FormattingTokenType.None, multiLine);
	  if (multiLine)
	  {
		Write(FormattingTokenType.End);
		if (expression.IsFunction)
		  Write(FormattingTokenType.Function);
		else
		  Write(FormattingTokenType.Sub);
	  }
	}
	protected override void GenerateQueryExpression(QueryExpression expression)
	{
	  if (expression == null || expression.DetailNodes == null || expression.DetailNodeCount == 0)
		return;
	  GenerateElementCollection(expression.DetailNodes);
	}
	protected override void GenerateDistinctExpression(DistinctExpression expression)
	{
	  Write(FormattingTokenType.Distinct);
	}
	protected override void GenerateLetExpression(LetExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Let);
	  GenerateElementCollection(expression.Declarations, FormattingTokenType.Comma);
	}
	protected override void GenerateFunctionPointerTypeReference(FunctionPointerTypeReference expression)
	{
	}
	protected override void GenerateRelationalOperator(RelationalOperator relationalOperator)
	{
	  BinaryOperatorType op = RelationalOperation.GetBinaryOperatorType(relationalOperator);
	  Write(GetBinaryOperatorToken(op));
	}
	protected override void GenerateReferenceExpressionBase(ReferenceExpressionBase expression)
	{
	  GenerateReferenceExpressionBase(expression, FormattingTokenType.None, FormattingTokenType.None, FormattingTokenType.Dot);
	}
	protected virtual void GenerateXmlElementReferenceExpression(XmlElementReferenceExpression expression)
	{
	  if (expression == null)
		return;
	  if (expression.IsKey)
		Write(FormattingTokenType.Key);
	  FormattingTokenType delimiter = FormattingTokenType.Do;
	  if (expression.XmlReferenceType == XmlReferenceType.TripleDot)
	  {
		delimiter = FormattingTokenType.DotDotDot;
	  }
	  GenerateReferenceExpressionBase(expression, FormattingTokenType.LessThan, FormattingTokenType.GreaterThen, delimiter);
	}
	protected virtual void GenerateXmlAttributeReferenceExpression(XmlAttributeReferenceExpression expression)
	{
	  if (expression == null)
		return;
	  if (expression.IsKey)
		Write(FormattingTokenType.Key); 
	  GenerateReferenceExpressionBase(expression, FormattingTokenType.At, FormattingTokenType.None, FormattingTokenType.Dot);
	}
	protected virtual void GenerateXmlExpression(XmlExpression expression)
	{
	  if (expression == null || expression.Nodes == null || expression.NodeCount == 0)
		return;
	  for (int i = 0; i < expression.NodeCount; i++)
	  {
		LanguageElement element = expression.Nodes[i] as LanguageElement;
		if (element == null)
		  continue;
		if (HtmlCodeGen == null)
		  return;
		string str = HtmlCodeGen.GenerateCode(element);
		if (str != null)
		{
		  str = DeleteLastEOL(str);
		  if (str != null && str != String.Empty)
		  {
			Write(str);
		  }
		}
	  }
	}
	protected virtual void GenerateIsExpression(Is expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  Write(FormattingTokenType.Is);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected void GenerateReDimExpression(ReDimExpression expression)
	{
	  if (expression == null)
		return;
	  CodeGen.GenerateElement(expression.Expression);
	  GenerateElementCollection(expression.Modifiers);
	}
	protected void GenerateAggregateExpression(AggregateExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Aggregate);
	  GenerateElementCollection(expression.InExpressions, FormattingTokenType.Comma);
	  WrapQuery();
	  if (IsNotNull(expression.QueryOperators))
	  {
		if (Options.WrappingAlignment.WrapQueryExpression)
		  GenerateElementCollection(expression.QueryOperators, FormattingTokenType.LineContinuation);
		else
		  GenerateElementCollection(expression.QueryOperators);
		WrapQuery();
	  }
	  GeneratesIntoTail(expression);
	}
	protected void GenerateSkipExpression(SkipExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Skip);
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected void GenerateSkipWhileExpression(SkipWhileExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Skip);
	  Write(FormattingTokenType.While);
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected void GenerateTakeExpression(TakeExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Take);
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected void GenerateTakeWhileExpression(TakeWhileExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.Take);
	  Write(FormattingTokenType.While);
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected void GenerateObjectCollectionInitializer(ObjectCollectionInitializer expression)
	{
	  Write(FormattingTokenType.From);
	  Write(FormattingTokenType.CurlyBraceOpen);
	  GenerateElementCollection(expression.Initializers, FormattingTokenType.Comma);
	  Write(FormattingTokenType.CurlyBraceClose);
	}
	protected void GenerateStringLiteralCore(string value, bool canHasEscapeConstants)
	{
	  if (value == null)
		return;
	  if (value.Length == 0)
		Write("\"\"");
	  else
	  {
		string s = value;
		if (canHasEscapeConstants)
		{
		  s = s.Replace(@"\""", @""" + Chr(34) + """);
		  s = s.Replace(@"\\", @"\");
		  s = s.Replace(@"\r\n", @""" + vbCrLf + """);
		  s = s.Replace(@"\r", @""" + vbCr + """);
		  s = s.Replace(@"\n", @""" + vbLf + """);
		  s = s.Replace(@"\t", @""" + vbTab + """);
		}
		Write("\"" + s + "\"");
	  }
	}
	protected void GenerateDirectCastExpression(DirectCastExpression expression)
	{
	  Write(FormattingTokenType.DirectCast, FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.Target);
	  Write(FormattingTokenType.Comma);
	  CodeGen.GenerateElement(expression.TypeReference);
	  Write(FormattingTokenType.ParenClose);
	}
	protected void GenerateCTypeExpression(CTypeExpression expression)
	{
	  GenerateCTypeExpression(expression.Target, expression.TypeReference);
	}
	protected void GenerateCTypeExpression(Expression target, TypeReferenceExpression type)
	{
	  Write(FormattingTokenType.CType);
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(target);
	  Write(FormattingTokenType.Comma);
	  CodeGen.GenerateElement(type);
	  Write(FormattingTokenType.ParenClose);
	}
	protected void GenerateCastTargetExpression(CastTargetExpression expression)
	{
	  string castKeyword = expression.CastKeyword;
	  if (String.IsNullOrEmpty(castKeyword))
		return;
	  Write(expression.CastKeyword);
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.Target);
	  Write(FormattingTokenType.ParenClose);
	}
	protected override FormattingTokenType GetOperatorToken(UnaryOperatorExpression expression)
	{
	  switch (expression.UnaryOperator)
	  {
		case UnaryOperatorType.UnaryNegation:
		  return FormattingTokenType.Minus;
		case UnaryOperatorType.UnaryPlus:
		  return FormattingTokenType.Plus;
		case UnaryOperatorType.LogicalNot:
		  return FormattingTokenType.Not;
		case UnaryOperatorType.OnesComplement:
		  break;
		case UnaryOperatorType.PointerDereference:
		  break;
		case UnaryOperatorType.TrackingReference:
		  break;
	  }
	  return FormattingTokenType.None;
	}
	public void GenerateTypeString(string type)
	{
	  switch (type)
	  {
		case "bool":
		  Write(FormattingTokenType.Boolean);
		  break;
		case "int":
		  Write(FormattingTokenType.Integer);
		  break;
		case "byte":
		  Write(FormattingTokenType.Byte);
		  break;
		case "string":
		  Write(FormattingTokenType.String);
		  break;
		case "float":
		  Write(FormattingTokenType.Single);
		  break;
		default:
		  Write(type);
		  break;
	  }
	}
	public static void GenerateAssignmentOperatorText(CodeGen codeGen, AssignmentOperatorType op)
	{
	  switch (op)
	  {
		case AssignmentOperatorType.Assignment:
		  codeGen.Write(FormattingTokenType.Equal);
		  break;
		case AssignmentOperatorType.BackSlashEquals:
		  codeGen.Write(FormattingTokenType.BackSlashEqual);
		  break;
		case AssignmentOperatorType.BitAndEquals:
		  codeGen.Write(FormattingTokenType.AmpersandEqual);
		  break;
		case AssignmentOperatorType.BitOrEquals:
		  codeGen.Write(FormattingTokenType.VerticalBarEqual);
		  break;
		case AssignmentOperatorType.MinusEquals:
		  codeGen.Write(FormattingTokenType.MinusEqual);
		  break;
		case AssignmentOperatorType.PlusEquals:
		  codeGen.Write(FormattingTokenType.PlusEqual);
		  break;
		case AssignmentOperatorType.ShiftLeftEquals:
		  codeGen.Write(FormattingTokenType.LessThanLessThanEqual);
		  break;
		case AssignmentOperatorType.ShiftRightEquals:
		  codeGen.Write(FormattingTokenType.GreatThanGreatThanEqual);
		  break;
		case AssignmentOperatorType.SlashEquals:
		  codeGen.Write(FormattingTokenType.SlashEqual); 
		  break;
		case AssignmentOperatorType.StarEquals:
		  codeGen.Write(FormattingTokenType.AsteriskEqual);
		  break;
		case AssignmentOperatorType.XorEquals:
		  codeGen.Write(FormattingTokenType.CaretEqual);
		  break;
	  }
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.TypeOf:
		case FormattingTokenType.As:
		case FormattingTokenType.New:
		case FormattingTokenType.AddressOf:
		case FormattingTokenType.Not:
		case FormattingTokenType.IsNot:
		case FormattingTokenType.Await:
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
		case FormattingTokenType.As:
		case FormattingTokenType.IsNot:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Plus:
		case FormattingTokenType.PlusEqual:
		case FormattingTokenType.Equal:
		case FormattingTokenType.Ampersand:
		case FormattingTokenType.VerticalBar:
		case FormattingTokenType.AmpersandAmpersand:
		case FormattingTokenType.VerticalBarVerticalBar:
		case FormattingTokenType.Caret:
		case FormattingTokenType.Slash:
		case FormattingTokenType.GreaterThen:
		case FormattingTokenType.GreaterThenEqual:
		case FormattingTokenType.EqualEqual:
		case FormattingTokenType.LessThan:
		case FormattingTokenType.LessThanEqual:
		case FormattingTokenType.Percent:
		case FormattingTokenType.Asterisk:
		case FormattingTokenType.Minus:
		case FormattingTokenType.EqualEqualEqual:
		case FormattingTokenType.ExclamationEqualEqual:
		case FormattingTokenType.In:
		case FormattingTokenType.GreatThanGreatThanGreatThan:
		case FormattingTokenType.GreatThanGreatThan:
		case FormattingTokenType.LessThanLessThan:
		case FormattingTokenType.ExclamationEqual:
		  if (ContextMatch(LanguageElementType.BinaryOperatorExpression) && Options.WrappingAlignment.WrapBeforeOperatorInBinaryExpression)
		  {
			Write(FormattingTokenType.LineContinuation);
			if (CodeGen != null && CodeGen.TokenGen != null && CodeGen.TokenGen.FormattingElements != null)
			  result.AddRange(CodeGen.TokenGen.FormattingElements);
		  }
		  break;
		case FormattingTokenType.And:
		case FormattingTokenType.Or:
		case FormattingTokenType.OrElse:
		  if (ContextMatch(LanguageElementType.LogicalOperation) && Options.WrappingAlignment.WrapBeforeOperatorInLogicalExpression)
		  {
			Write(FormattingTokenType.LineContinuation);
			if (CodeGen != null && CodeGen.TokenGen != null && CodeGen.TokenGen.FormattingElements != null)
			  result.AddRange(CodeGen.TokenGen.FormattingElements);
		  }
		  break;
	  }
	  return result;
	}
  }
}
