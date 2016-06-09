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
  public partial class JavaScriptParser : FormattingParserBase
	{
	bool _InsideForEachLoop;
		bool _PreviousTokenWasComment;
	CommentCollection _Comments = new CommentCollection();
	const int _MaxMult = 200;
		public JavaScriptParser() 
		{
			parserErrors = new JavaScriptParserErrors();
			set = CreateSetArray();
			maxTokens = Tokens.MaxTokens;
		}
		#region CreateUnaryIncrement
		private Expression CreateUnaryIncrement(Expression sourceExpression, Token operatorToken, bool isPostfix)
		{
			if (sourceExpression == null || operatorToken == null)
				return sourceExpression;
			UnaryIncrement unaryIncrement = new UnaryIncrement(operatorToken, sourceExpression, isPostfix);
			if (isPostfix)
				unaryIncrement.SetRange(GetRange(sourceExpression, operatorToken));
			else
				unaryIncrement.SetRange(GetRange(operatorToken, sourceExpression));
			ProcessExpressionForUnary(sourceExpression);
			return unaryIncrement;
		}
		void ProcessExpressionForUnary(Expression expr)
		{
			ElementReferenceExpression refExpr = expr as ElementReferenceExpression;
			if (refExpr != null)
				refExpr.IsModified = true;
		}
		#endregion
		#region CreateUnaryDecrement
		private Expression CreateUnaryDecrement(Expression sourceExpression, Token operatorToken, bool isPostfix)
		{
			if (sourceExpression == null || operatorToken == null)
				return sourceExpression;
			UnaryDecrement unaryDecrement = new UnaryDecrement(operatorToken, sourceExpression, isPostfix);
			if (isPostfix)
				unaryDecrement.SetRange(GetRange(sourceExpression, operatorToken));
			else
				unaryDecrement.SetRange(GetRange(operatorToken, sourceExpression));
			ProcessExpressionForUnary(sourceExpression);
			return unaryDecrement;
		}
		#endregion
		#region GetBinaryOperatorType
		private BinaryOperatorType GetBinaryOperatorType(Token operatorToken)
		{
			BinaryOperatorType operatorType = BinaryOperatorType.None;
			switch (operatorToken.Type)
			{
				case Tokens.PLUS:
					operatorType = BinaryOperatorType.Add; 
					break;
				case Tokens.MINUS:
					operatorType = BinaryOperatorType.Subtract; 
					break;
				case Tokens.SLASH:
					operatorType = BinaryOperatorType.Divide; 
					break;
				case Tokens.PERCENTSYMBOL:
					operatorType = BinaryOperatorType.Modulus; 
					break;
				case Tokens.ASTERISK:
					operatorType = BinaryOperatorType.Multiply; 
					break;
				case Tokens.SHIFTLEFT:
					operatorType = BinaryOperatorType.ShiftLeft; 
					break;
				case Tokens.SHIFTRIGHT:
					operatorType = BinaryOperatorType.ShiftRight; 
					break;
				case Tokens.TRIPLESHIFTRIGHT:
					operatorType = BinaryOperatorType.UnsignedShiftRight; 
					break;
				case Tokens.IN:
					operatorType = BinaryOperatorType.In; 
					break;
			}
			return operatorType;
		}
		#endregion
		#region SetVariableProperties
		void SetVariableProperties(BaseVariable variable, SourceRange nameRange, SourceRange varRange, SourceRange startRange, SourceRange endRange)
		{
			if (variable == null)
				return;
			variable.NameRange = nameRange;
			variable.TypeRange = varRange;
			variable.SetRange(GetRange(startRange, endRange));
		}
		#endregion
		#region GetMethodReference
		private Expression GetMethodReference(Expression source)
		{
			MethodReferenceExpression methodReference = null;
			if (source is IHasQualifier)
			{
		Expression qualifier = (source as IHasQualifier).Qualifier;
		if (source is IndexerExpression)
		  qualifier = source;
				methodReference = new MethodReferenceExpression(qualifier, source.Name, source.NameRange);
				methodReference.AddNode(qualifier);
			}
			else
				methodReference = new MethodReferenceExpression(source.Name, source.NameRange);
			methodReference.SetRange(source.Range);
	  if (source is AnonymousMethodExpression)
		methodReference.AddDetailNode(source);
			return methodReference;
		}
		#endregion
		#region GetLogicalOperatorType
		private LogicalOperator GetLogicalOperatorType(Token operatorToken)
		{
			LogicalOperator operatorType = LogicalOperator.None;
			switch (operatorToken.Type)
			{
				case Tokens.BITAND:
					operatorType = LogicalOperator.And;
					break;
				case Tokens.BITOR:
					operatorType = LogicalOperator.Or;
					break;
				case Tokens.ANDAND:
					operatorType = LogicalOperator.ShortCircuitAnd;
					break;
				case Tokens.OROR:
					operatorType = LogicalOperator.ShortCircuitOr;
					break;
				case Tokens.XORSYMBOL:
					operatorType = LogicalOperator.ExclusiveOr;
					break;
			}
			return operatorType;
		}
		#endregion
		#region GetRelationalOperatorType
		private RelationalOperator GetRelationalOperatorType(Token operatorToken)
		{
			RelationalOperator operatorType = RelationalOperator.None;
			switch (operatorToken.Type)
			{
				case Tokens.DOUBLEEQUALS:
					operatorType = RelationalOperator.Equality;
					break;
				case Tokens.NOTEQUALS:
					operatorType = RelationalOperator.Inequality;
					break;
				case Tokens.TRIPLEEQUALS:
					operatorType = RelationalOperator.StrictEquality;
					break;
				case Tokens.NOTDOUBLEEQUALS:
					operatorType = RelationalOperator.StrictInequality;
					break;
				case Tokens.LESSTHAN:
					operatorType = RelationalOperator.LessThan;
					break;
				case Tokens.GREATERTHAN:
					operatorType = RelationalOperator.GreaterThan;
					break;
				case Tokens.LESSOREQUAL:
					operatorType = RelationalOperator.LessOrEqual;
					break;
				case Tokens.GREATEROREQUAL:
					operatorType = RelationalOperator.GreaterOrEqual;
					break;
			}
			return operatorType;
		}
		#endregion
		#region GetOperatorType
		private AssignmentOperatorType GetOperatorType(Token operatorToken)
		{
			AssignmentOperatorType operatorType = AssignmentOperatorType.None;
			switch (operatorToken.Type)
			{
				case Tokens.EQUALSSYMBOL:
					operatorType = AssignmentOperatorType.Assignment;
					break;
				case Tokens.MULEQUAL:
					operatorType = AssignmentOperatorType.StarEquals;
					break;
				case Tokens.DIVEQUAL:
					operatorType = AssignmentOperatorType.SlashEquals;
					break;
				case Tokens.MODEQUAL:
					operatorType = AssignmentOperatorType.PercentEquals;
					break;
				case Tokens.PLUSEQUAL:
					operatorType = AssignmentOperatorType.PlusEquals;
					break;
				case Tokens.MINUSEQUAL:
					operatorType = AssignmentOperatorType.MinusEquals;
					break;
				case Tokens.SHIFTLEFTEQUAL:
					operatorType = AssignmentOperatorType.ShiftLeftEquals;
					break;
				case Tokens.SHIFTRIGHTEQUAL:
					operatorType = AssignmentOperatorType.ShiftRightEquals;
					break;
				case Tokens.ANDEQUAL:
					operatorType = AssignmentOperatorType.BitAndEquals;
					break;
				case Tokens.OREQUAL:
					operatorType = AssignmentOperatorType.BitOrEquals;
					break;
				case Tokens.XOREQUAL:
					operatorType = AssignmentOperatorType.XorEquals;
					break;
				case Tokens.TRIPLESHIFTRIGHTEQUAL:
					operatorType = AssignmentOperatorType.UnsignedShiftRightEquals;
					break;
			}
			return operatorType;
		}
		#endregion
		#region CreateImplicitVariable
		ImplicitVariable CreateImplicitVariable(String name, Expression initializer, SourceRange operatorRange)
		{
			ImplicitVariable result = new ImplicitVariable(name, initializer);
			result.AddDetailNode(initializer);
			result.OperatorRange = operatorRange;
			return result;
		}
		#endregion
	#region TokenIsComment
	private bool TokenIsComment(int tokenType)
	{
	  return tokenType == Tokens.SINGLELINECOMMENT || tokenType == Tokens.MULTILINECOMMENT;
	}
	#endregion
	private void SetCommentProperties(Token commentToken, string commentTokenValue, int textStartOffset, int textEndOffset, CommentType commentType, Comment comment)
	{
	  comment.SetTextStartOffset(textStartOffset);
	  comment.StartPos = commentToken.StartPosition;
	  comment.EndPos = commentToken.EndPosition;
	  comment.SetTextStartOffset(textStartOffset);
	  comment.CommentType = commentType;
	  comment.Name = commentTokenValue.Substring(textStartOffset, commentTokenValue.Length - (textStartOffset + textEndOffset));
	  comment.SetRange(commentToken.Range);
	}
	private void AddMultiLineXmlComment(Token commentToken)
	{
	  if (commentToken == null)
		return;
	  string commentTokenValue = commentToken.Value;
	  int textStartOffset = 0;
	  int textEndOffset = 2;
	  CommentType commentType = CommentType.MultiLine;
	  XmlDocComment comment = new XmlDocComment();
	  SetCommentProperties(commentToken, commentTokenValue, textStartOffset, textEndOffset, commentType, comment);
	  Comments.Add(comment);
	}
	private void AddMultiLineComment(Token commentToken)
	{
	  if (commentToken == null)
		return;
	  string commentTokenValue = commentToken.Value;
	  if (commentTokenValue.StartsWith("/**"))
	  {
		AddMultiLineXmlComment(commentToken);
		return;
	  }
	  int textStartOffset = 2;
	  int textEndOffset = 2;
	  CommentType commentType = CommentType.MultiLine;
	  Comment comment = new Comment();
	  SetCommentProperties(commentToken, commentTokenValue, textStartOffset, textEndOffset, commentType, comment);
	  Comments.Add(comment);
	}
	private void AddSingleLineXmlComment(Token commentToken)
	{
	  if (commentToken == null)
		return;
	  if (_PreviousTokenWasComment)
	  {
		XmlDocComment previousComment = Comments[Comments.Count - 1] as XmlDocComment;
		if (previousComment != null && previousComment.CommentType == CommentType.SingleLine)
		{
		  previousComment.InternalName += "\r\n";
		  int spacesCount = 0;
		  spacesCount = commentToken.Range.Start.Offset - 1;
		  int newLinesCount = 0;
		  newLinesCount = commentToken.Range.Start.Line - previousComment.Range.End.Line - 1;
		  string appendHeightStr = new String('\n', newLinesCount);
		  string appendStr = new String('\t', spacesCount);
		  previousComment.InternalName += appendHeightStr + appendStr;
		  string commentValue = commentToken.Value.Substring(0, commentToken.Value.Length);
		  previousComment.InternalName += commentValue;
		  previousComment.SetRange(GetRange(previousComment, commentToken));
		  return;
		}
	  }
	  string commentTokenValue = commentToken.Value;
	  int textStartOffset = 0;
	  int textEndOffset = 0;
	  CommentType commentType = CommentType.SingleLine;
	  XmlDocComment comment = new XmlDocComment();
	  SetCommentProperties(commentToken, commentTokenValue, textStartOffset, textEndOffset, commentType, comment);
	  Comments.Add(comment);
	}
	private void AddSingleLineComment(Token commentToken)
	{
	  if (commentToken == null)
		return;
	  string commentTokenValue = commentToken.Value;
	  if (commentTokenValue.StartsWith("///"))
	  {
		AddSingleLineXmlComment(commentToken);
		return;
	  }
	  int textStartOffset = 2;
	  int textEndOffset = 0;
	  CommentType commentType = CommentType.SingleLine;
	  Comment comment = new Comment();
	  SetCommentProperties(commentToken, commentTokenValue, textStartOffset, textEndOffset, commentType, comment);
	  Comments.Add(comment);
	}
	#region AddCommentNode
	private void AddCommentNode(Token commentToken)
	{
	  if (commentToken == null)
		return;
	  if (commentToken.Type == Tokens.MULTILINECOMMENT)
		AddMultiLineComment(commentToken);
	  else
		AddSingleLineComment(commentToken);
	}
	#endregion
		#region SetMethodProperties
		protected void SetMethodProperties(Method method, SourceRange nameRange, SourceRange parenOpenRange, SourceRange parenCloseRange, LanguageElementCollection parameters)
		{
			if (method == null)
				return;
			method.ValidVisibilities = new MemberVisibility[]{};
			method.NameRange = nameRange;
			method.ParamOpenRange = parenOpenRange;
			method.ParamCloseRange = parenCloseRange;
			method.SetParensRange(GetRange(parenOpenRange, parenCloseRange));
			method.MethodType = MethodTypeEnum.Function;
			if (parameters != null && parameters.Count > 0)
				for (int i = 0; i < parameters.Count; i++)
				{
					method.Parameters.Add(parameters[i]);
					method.AddDetailNode(parameters[i]);
				}
		}
		#endregion
		#region CreateParam
		protected Param CreateParam(Token paramToken)
		{
			Param parameter = new LambdaImplicitlyTypedParam(paramToken.Value);
			parameter.SetRange(paramToken.Range);
			parameter.NameRange = paramToken.Range;
			parameter.Direction = ArgumentDirection.In;
			return parameter;
		}
		#endregion
		#region IsLabelledStatement
		protected bool IsLabelledStatement()
		{
			if (la.Type != Tokens.IDENTIFIER)
				return false;
			ResetPeek();
			return Peek().Type == Tokens.COLON;
		}
		#endregion
		#region CreateTypeReference
		protected TypeReferenceExpression CreateTypeReference(Token idToken)
		{
			if (idToken == null)
				return null;
			TypeReferenceExpression result = new TypeReferenceExpression(idToken.Value, idToken.Range);
			result.NameRange = idToken.Range;
			return result;
		}
		#endregion
		#region CreateTypeReference
		protected TypeReferenceExpression CreateTypeReference(TypeReferenceExpression qualifier, Token idToken)
		{
			TypeReferenceExpression result = CreateTypeReference(idToken);
			if (qualifier == null || result == null)
				return result;
			result.SetBaseTypeAfterCreation(qualifier);
			result.SetRange(GetRange(qualifier, result));
			return result;
		}
		#endregion
		#region CreateVariable
		protected Variable CreateVariable(string name, SourceRange nameRange, Expression initializer, SourceRange varRange, SourceRange startRange, SourceRange endRange, SourceRange operatorRange)
		{
			Variable result = CreateImplicitVariable(name, initializer, operatorRange);
			if (result != null)
				SetVariableProperties(result, nameRange, varRange, startRange, endRange);
			return result;
		}
		#endregion
		#region IsForEachLoop
	protected bool IsForEachLoop()
	{
	  if (la.Type == Tokens.IN)
		return true;
	  ResetPeek();
	  Token nextToken = Peek();
	  int lparCount = 1;
	  bool wasIn = false;
	  while (nextToken != null && nextToken.Type != Tokens.EOF)
	  {
		int nextType = nextToken.Type;
		if (!wasIn && nextType == Tokens.EQUALSSYMBOL)
		  return false;
		if (nextType == Tokens.LPAR)
		  lparCount++;
		if (nextType == Tokens.RPAR)
		{
		  lparCount--;
		  if (lparCount == 0)
			break;
		}
		if (nextType == Tokens.SEMICOLON)
		  return false;
		if (nextType == Tokens.IN)
		  wasIn = true;
		nextToken = Peek();
	  }
	  return wasIn;
	}
		#endregion
		#region CreateElementReference
		protected ElementReferenceExpression CreateElementReference(Token idToken)
		{
			ElementReferenceExpression result = new ElementReferenceExpression(idToken.Value, idToken.Range);
			result.SetRange(idToken.Range);
			return result;
		}
		#endregion
		#region CreateElementReference
		protected ElementReferenceExpression CreateElementReference(Expression source, Token idToken)
		{
			if (source == null || idToken == null)
				return null;
			ElementReferenceExpression result = new QualifiedElementReference(source, idToken.Value, idToken.Range);
			result.AddNode(source);
			result.SetRange(GetRange(source, idToken));
			return result;
		}
		#endregion
		#region CreateAssignmentExpression
		protected AssignmentExpression CreateAssignmentExpression(Expression leftPart, Expression rightPart, Token operatorToken)
		{
			if (leftPart == null || rightPart == null || operatorToken == null)
				return null;
			AssignmentExpression result = new AssignmentExpression(leftPart, operatorToken, rightPart);
			AssignmentOperatorType operatorType = AssignmentOperatorType.None;
			operatorType = GetOperatorType(operatorToken);
			result.AssignmentOperator = operatorType;
			result.SetRange(GetRange(leftPart, rightPart));
			return result;
		}
		#endregion
		#region CreateConditionalExpression
		protected Expression CreateConditionalExpression(Expression condition, Expression trueExpression, Expression falseExpression)
		{
			if (condition == null || trueExpression == null || falseExpression == null)
				return condition;
			ConditionalExpression conditionalExpression = new ConditionalExpression(condition, trueExpression, falseExpression);
			conditionalExpression.SetRange(GetRange(condition, falseExpression));
			return conditionalExpression;
		}
		#endregion
		#region CreateLogicalOperation
		protected Expression CreateLogicalOperation(Expression leftSide,Expression rightSide, Token operatorToken)
		{
			if (leftSide == null || rightSide == null || operatorToken == null)
				return leftSide;
			LogicalOperation logicalOperation = new LogicalOperation(leftSide, operatorToken, rightSide);
			logicalOperation.LogicalOperator = GetLogicalOperatorType(operatorToken);
			logicalOperation.SetRange(GetRange(leftSide, rightSide));
			return logicalOperation;
		}
		#endregion
		#region CreateRelationalOperation
		protected Expression CreateRelationalOperation(Expression leftSide, Expression rightSide, Token operatorToken)
		{
			if (leftSide == null || rightSide == null || operatorToken == null)
				return leftSide;
			RelationalOperation relationalOperation = new RelationalOperation(leftSide, operatorToken, rightSide);
			relationalOperation.RelationalOperator = GetRelationalOperatorType(operatorToken);
			relationalOperation.SetRange(GetRange(leftSide, rightSide));
			return relationalOperation;
		}
		#endregion
		#region CreateBinaryOperatorExpression
		protected Expression CreateBinaryOperatorExpression(Expression leftSide, Expression rightSide, Token operatorToken)
		{
			if (leftSide == null || rightSide == null || operatorToken == null)
				return leftSide;
			BinaryOperatorExpression binary = new BinaryOperatorExpression(leftSide, operatorToken, rightSide);
			binary.BinaryOperator = GetBinaryOperatorType(operatorToken);
			binary.SetRange(GetRange(leftSide, rightSide));
			return binary;
		}
		#endregion
		#region CreateTypeCheckExpression
		protected Expression CreateTypeCheckExpression(Expression leftSide, Expression rightSide, Token operatorToken)
		{
			if (leftSide == null || rightSide == null || operatorToken == null)
				return leftSide;
			TypeCheck typeCheck = new TypeCheck(leftSide, operatorToken, rightSide);
			typeCheck.SetRange(GetRange(leftSide, rightSide));
			return typeCheck;
		}
		#endregion
		#region CreateUnaryPostfixIncrement
		protected Expression CreateUnaryPostfixIncrement(Expression sourceExpression, Token operatorToken)
		{
			return CreateUnaryIncrement(sourceExpression, operatorToken, true);
		}
		#endregion
		#region CreateUnaryPrefixIncrement
		protected Expression CreateUnaryPrefixIncrement(Expression sourceExpression, Token operatorToken)
		{
			return CreateUnaryIncrement(sourceExpression, operatorToken, false);
		}
		#endregion
		#region CreateUnaryPostfixDecrement
		protected Expression CreateUnaryPostfixDecrement(Expression sourceExpression, Token operatorToken)
		{
			return CreateUnaryDecrement(sourceExpression, operatorToken, true);
		}
		#endregion
		#region CreateUnaryPrefixDecrement
		protected Expression CreateUnaryPrefixDecrement(Expression sourceExpression, Token operatorToken)
		{
			return CreateUnaryDecrement(sourceExpression, operatorToken, false);
		}
		#endregion
		#region CreateUnaryExpression
		protected Expression CreateUnaryExpression(Expression sourceExpression, Token operatorToken)
		{
			if (sourceExpression == null || operatorToken == null)
				return sourceExpression;
			UnaryOperatorExpression unaryOperatorExpression = new UnaryOperatorExpression(operatorToken, sourceExpression, false);
			unaryOperatorExpression.SetRange(GetRange(operatorToken, sourceExpression));
			return unaryOperatorExpression;
		}
		#endregion
		#region CreateDeleteExpression
		protected Expression CreateDeleteExpression(Expression sourceExpression, Token operatorToken)
		{
			if (sourceExpression == null || operatorToken == null)
				return sourceExpression;
			DeleteExpression deleteExpression = new DeleteExpression(sourceExpression);
			deleteExpression.AddDetailNode(sourceExpression);
			deleteExpression.SetRange(GetRange(operatorToken, sourceExpression));
			return deleteExpression;
		}
		#endregion
		#region CreateLogicalInversion
		protected Expression CreateLogicalInversion(Expression sourceExpression, Token operatorToken)
		{
			if (sourceExpression == null || operatorToken == null)
				return sourceExpression;
			LogicalInversion logicalInversion = new LogicalInversion(operatorToken, sourceExpression, false);
			logicalInversion.SetRange(GetRange(operatorToken, sourceExpression));
			return logicalInversion;
		}
		#endregion
		#region CreateTypeOfExpression
		protected Expression CreateTypeOfExpression(Expression sourceExpression, Token operatorToken)
		{
			if (sourceExpression == null || operatorToken == null)
				return sourceExpression;
			TypeOfExpression typeOfExpression = new TypeOfExpression(sourceExpression);
			typeOfExpression.AddDetailNode(sourceExpression);
			typeOfExpression.SetRange(GetRange(operatorToken, sourceExpression));
			return typeOfExpression;
		}
		#endregion
		#region CreateThisExpression
		protected Expression CreateThisExpression(Token thisToken)
		{
			if (thisToken == null)
				return null;
			ThisReferenceExpression thisReferenceExpression = new ThisReferenceExpression(thisToken.Value);
			thisReferenceExpression.SetRange(thisToken.Range);
			return thisReferenceExpression;
		}
		#endregion
		#region CreateParenthesizedExpression
		protected Expression CreateParenthesizedExpression(Expression sourceExpression, SourceRange startRange, SourceRange endRange)
		{
			if (sourceExpression == null)
				return null;
			ParenthesizedExpression parenthesizedExpression = new ParenthesizedExpression(sourceExpression);
			parenthesizedExpression.SetRange(GetRange(startRange, endRange));
			return parenthesizedExpression;
		}
		#endregion
		#region CreateObjectInitializer
		protected Expression CreateObjectInitializer(ExpressionCollection initializers, SourceRange startRange, SourceRange endRange)
		{
			ObjectInitializerExpression objectInitializerExpression = new ObjectInitializerExpression();
			objectInitializerExpression.SetRange(GetRange(startRange, endRange));
			if (initializers != null && initializers.Count > 0)
				for (int i = 0; i < initializers.Count; i++)
					objectInitializerExpression.AddInitializer(initializers[i]);
			return objectInitializerExpression;
		}
		#endregion
		#region CreateMemberInitializer
		protected Expression CreateMemberInitializer(Token nameToken, Expression initializerValue)
		{
			if (nameToken == null || initializerValue == null)
				return null;
			MemberInitializerExpression memberInitializerExpression = new MemberInitializerExpression();
			memberInitializerExpression.Name = nameToken.Value;
			memberInitializerExpression.NameRange = nameToken.Range;
			memberInitializerExpression.Value = initializerValue;
			memberInitializerExpression.AddDetailNode(initializerValue);
			memberInitializerExpression.SetRange(GetRange(nameToken, initializerValue));
			return memberInitializerExpression;
		}
		#endregion
		#region CreateArrayInitializerExpression
		protected Expression CreateArrayInitializerExpression(SourceRange startRange, SourceRange endRange, ExpressionCollection initializers)
		{
			ArrayInitializerExpression arrayInitializerExpression = new ArrayInitializerExpression();
			arrayInitializerExpression.SetRange(GetRange(startRange, endRange));
			if (initializers != null && initializers.Count > 0)
				for (int i = 0; i < initializers.Count; i++)
				{
					arrayInitializerExpression.Initializers.Add(initializers[i]);
					arrayInitializerExpression.AddDetailNode(initializers[i]);
				}
			return arrayInitializerExpression;
		}
		#endregion
		#region CreateObjectCreationExpression
		protected ObjectCreationExpression CreateObjectCreationExpression(TypeReferenceExpression objectType, ExpressionCollection arguments, SourceRange lparRange, SourceRange rparRange, SourceRange startRange, SourceRange endRange)
		{
			if (objectType == null)
				return null;
			ObjectCreationExpression objectCreationExpression = new ObjectCreationExpression(objectType);
			objectCreationExpression.AddNode(objectType);
			objectCreationExpression.SetRange(GetRange(startRange, endRange));
			objectCreationExpression.SetParensRange(GetRange(lparRange, rparRange));
			if (arguments != null)
				for (int i = 0; i < arguments.Count; i++)
					objectCreationExpression.AddArgument(arguments[i]);
			return objectCreationExpression;
		}
		#endregion
		#region CreateAnonymousMethod
		protected AnonymousMethodExpression CreateAnonymousMethod(SourceRange startRange, SourceRange lparRange, SourceRange rparRange, LanguageElementCollection parameters, Token nameToken)
		{
			AnonymousMethodExpression result = new AnonymousMethodExpression();
			result.SetRange(startRange);
			result.ParamOpenRange = lparRange;
			result.ParamCloseRange = rparRange;
			if (parameters != null)
				for (int i = 0; i < parameters.Count; i++)
					result.AddParameter(parameters[i] as Param);
	  if (nameToken != null)
	  {
		result.Name = nameToken.Value;
		result.NameRange = nameToken.Range;
	  }
			return result;
		}
		#endregion
	#region CreateAnonymousConstructor
		protected AnonymousConstructorExpression CreateAnonymousConstructor(SourceRange startRange, SourceRange lparRange, SourceRange rparRange, LanguageElementCollection parameters, Token nameToken)
		{
			AnonymousConstructorExpression result = new AnonymousConstructorExpression();
			result.SetRange(startRange);
			result.ParamOpenRange = lparRange;
			result.ParamCloseRange = rparRange;
			if (parameters != null)
				for (int i = 0; i < parameters.Count; i++)
					result.AddParameter(parameters[i] as Param);
	  if (nameToken != null)
	  {
		result.Name = nameToken.Value;
		result.NameRange = nameToken.Range;
	  }
			return result;
		}
		#endregion
		#region CreateIndexerExpression
		protected Expression CreateIndexerExpression(Expression source, Expression argument, SourceRange endRange)
		{
			if (source == null || argument == null)
				return null;
			IndexerExpression indexerExpression = new IndexerExpression(source);
			indexerExpression.Arguments = new ExpressionCollection();
			indexerExpression.Arguments.Add(argument);
			indexerExpression.AddDetailNode(argument);
			indexerExpression.SetRange(GetRange(source, endRange));
			return indexerExpression;
		}
		#endregion
		#region CreateMethodCallExpression
		protected MethodCallExpression CreateMethodCallExpression(Expression source, ExpressionCollection arguments, SourceRange lparRange, SourceRange rparRange)
		{
			if (source == null)
				return null;
	  Expression methodReference = null;
	  if (source is ElementReferenceExpression)
		methodReference = GetMethodReference(source);
	  else
		methodReference = source;
			MethodCallExpression methodCallExpression = new MethodCallExpression(methodReference);
			methodCallExpression.AddNode(methodReference);
				methodCallExpression.SetParensRange(GetRange(lparRange, rparRange));
				methodCallExpression.SetRange(GetRange(source, rparRange));
			if (arguments != null)
				for (int i = 0; i < arguments.Count; i++)
					methodCallExpression.AddArgument(arguments[i]);
			return methodCallExpression;
		}
		#endregion
		#region Get
		protected override void Get()
		{
			_PreviousTokenWasComment = false;
			base.Get();
			Token oldT = tToken;
	  while (TokenIsComment(la.Type))
			{
				AddCommentNode(la);
				_PreviousTokenWasComment = true;
				base.Get();
			}
			tToken = oldT;
		}
		#endregion
	#region CleanUpParser
	protected override void CleanUpParser()
	{
	  base.CleanUpParser();
	  _Comments.Clear();
	}
	#endregion
	#region Peek
	protected new Token Peek()
		{
			Token result = scanner.Peek();
			int tokenType = result.Type;
			while (TokenIsComment(tokenType))
			{
				result = scanner.Peek();
				tokenType = result.Type;
			}
			return result;
		}
		#endregion
		#region PrepareForParsing
		protected void PrepareForParsing(ISourceReader reader)
		{
			scanner = new JavaScriptScanner(reader);
			la = new Token();
			la.Value = "";
			Get();
		}
		#endregion
		#region DisposeReader
		protected void DisposeReader(ref ISourceReader reader)
		{
			if (reader != null)
			{
				reader.Dispose();
				reader = null;
			}
		}
		#endregion
		protected bool InsideForEachLoop
		{
			get	{ return _InsideForEachLoop; }
			set
			{
				_InsideForEachLoop = value;
			}
		}
			public LanguageElement Parse(ISourceReader reader)
		{
			try
			{
				scanner = new JavaScriptScanner(reader);
				if (!(RootNode is SourceFile))
					OpenContext(GetSourceFile("dsf"));
				Parse();
		AfterParsing();
				return RootNode;
			}
			finally
			{
				CleanUpParser();
			}
		}
		protected override LanguageElement DoParse(ParserContext parserContext, ISourceReader reader)
		{
			LanguageElement context = parserContext.Context;
			if (context == null)
				return null;
			SetRootNode(context);
	  if (context is SourceFile)
		((SourceFile)context).SetDocument(parserContext.Document);
	  AfterParsing();
			return Parse(reader);
		}
		public TypeReferenceExpression ParseTypeReferenceExpression(ISourceReader reader)
		{
			try
			{
				PrepareForParsing(reader);
				TypeReferenceExpression result = null;
				TypeReference(out result);
		AfterParsing(result);
		ClearFormattingParsingElements();
				return result;
			}
			finally
			{
				CleanUpParser();
				DisposeReader(ref reader);
			}
		}
		public Expression ParseExpression(ISourceReader reader)
		{
			try
			{
				PrepareForParsing(reader);
				Expression result = null;
				Expression(out result);
		AfterParsing(result);
				return result;
			}
			finally
			{
				CleanUpParser();
				DisposeReader(ref reader);
			}
		}
	public override IExpressionInverter CreateExpressionInverter()
	{
	  return new JavaScriptExpressionInverter();
	}
		public override string Language
		{
			get
			{
				return "JavaScript";
			}
		}
	public override CommentCollection Comments
	{
	  get { return _Comments; }
	}
	protected override TokenCategory GetTokenCategory(CategorizedToken token)
	{
	  return JavaScriptTokensHelper.GetUncategorizedTokenCategory(token);
	}
  }
}
