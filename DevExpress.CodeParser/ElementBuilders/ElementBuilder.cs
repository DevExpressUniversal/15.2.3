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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ElementBuilder
	{
		const string STR_CodeElementTerminator = "\r\n";
		const string STR_SystemInt32 = "System.Int32";  
		private LanguageElementCollection _TopLevelElements = new LanguageElementCollection();
		private ElementList _RootElement = new ElementList();
	public ElementBuilder()
		{
		}
		protected void AddTopElement(LanguageElement element)
		{
			if (element == null)
				return;
			_RootElement.AddNode(element);
		}
		protected void InsertTopElement(int index, LanguageElement element)
		{
			if (element == null)
				return;
			_RootElement.InsertNode(index, element);
		}
		protected int GetIndexOfTopElement(LanguageElement element)
		{
			if (element == null || _RootElement.Nodes == null)
				return -1;
			return _RootElement.Nodes.IndexOf(element);
		}
		protected string GetSimpleTypeName(string typeName)
		{
	  if (StructuralParserServicesHolder.IsBuiltInType(typeName))
		return StructuralParserServicesHolder.GetSimpleTypeName(typeName);
			return typeName;
		}
		protected string GenerateElement(LanguageElement element)
		{
	  return GenerateElement(element, 0);
		}
	protected string GenerateElement(LanguageElement element, string languageID)
	{
	  return GenerateElement(element, languageID, 0);
	}
	protected string GenerateElement(LanguageElement element, int precedingWhiteSpaceCount)
	{
	  return StructuralParserServicesHolder.GenerateElement(element, precedingWhiteSpaceCount);
	}
	protected string GenerateElement(LanguageElement element, string languageID, int precedingWhiteSpaceCount)
	{
	  return StructuralParserServicesHolder.GenerateElement(element, languageID, precedingWhiteSpaceCount);
	}
	protected void SetMethodType(Method method, string type)
	{
	  if (method == null)
		return;
	  if (string.IsNullOrEmpty(type))
		method.MethodType = MethodTypeEnum.Void;
	  else
		method.MethodType = MethodTypeEnum.Function;				
	}
		#region AddParam
		protected virtual Param AddParam(MemberWithParameters parent, Param param)
		{
			if (parent == null)			
				AddTopElement(param);
			else
			{				
				parent.AddDetailNode(param);
				parent.Parameters.Add(param);
			}			
			return param;
		}
		#endregion		
	protected virtual Expression AddMethodArgument(MethodCall parent, Expression argument)
	{
	  if (parent == null)
		AddTopElement(argument);
	  else
	  {
		parent.AddDetailNode(argument);
		parent.Arguments.Add(argument);
	  }
	  return argument;
	}
		#region AddSource(IHasQualifier parent, object source)
		protected virtual Expression AddSource(IHasQualifier parent, object source)
		{
			if (parent == null)
				return null;						
			Expression lSource = GetExpression(source);
			if (lSource == null)
				return null;						
			parent.Qualifier = lSource;
			return lSource;
		}
		#endregion		
		#region AddAttributeSection
		protected virtual AttributeSection AddAttributeSection(CodeElement target, AttributeSection section)
		{
			if (target == null)
				return (AttributeSection)AddNode(null, section);			
			target.AddAttributeSection(section);
			if (target.Parent == null)
			{
				int lSectionIdx = GetIndexOfTopElement(target);
				if (lSectionIdx < 0)
					AddTopElement(section);
				else
					InsertTopElement(lSectionIdx, section);					
			}				
			return section;
		}
		#endregion
		#region AddAttribute
		protected virtual Attribute AddAttribute(AttributeSection parent, Attribute attr)
		{
			if (parent == null)
				return (Attribute)AddNode(null, attr);
			parent.AttributeCollection.Add(attr);
			parent.AddNode(attr);
			attr.SetTarget(parent.TargetNode);
			return attr;
		}
		#endregion
		#region AddConstructorInitializer
		public virtual ConstructorInitializer AddConstructorInitializer(Method parent, ConstructorInitializer initializer)
		{
			if (initializer == null)
				return null;
			if (parent == null)
				return (ConstructorInitializer)AddNode(null, initializer);
			if (parent.HasInitializer)			
				parent.RemoveNode(parent.Initializer);
			parent.InsertNode(0, initializer);
			return initializer;
		}
		#endregion
		#region AddComment
		protected virtual Comment AddComment(CodeElement parent, Comment comment)
		{
			if (parent == null || comment == null)
				return null;			
			parent.AddCommentNode(comment);
			return comment;
		}
		#endregion
		#region AddThrow
		public virtual Throw AddThrow(LanguageElement parent, Throw @throw)
		{
			if (parent == null || @throw == null)
				return null;
			parent.AddNode(@throw);
			return @throw;
		}
		#endregion
		#region IsAnIdentifier
		protected virtual bool IsAnIdentifier(string text)
		{
			if (text == null || text == "")
				return false;
			if (Char.IsNumber(text[0]))
				return false;
			foreach(Char lChar in text)
				if (!Char.IsLetterOrDigit(lChar))
					return false;
			return true;
		}
		#endregion
	#region GetTypeReferenceExpression
	protected virtual TypeReferenceExpression GetTypeReferenceExpression(object expression)
	{
	  if (expression == null)
		return null;
	  if (expression is TypeReferenceExpression)
		return (TypeReferenceExpression)expression;
	  if (expression is string && IsAnIdentifier((string)expression))
		return BuildTypeReference((string)expression);
	  else
		return null;
	}
	#endregion
		#region AddDetailNodes
		protected virtual LanguageElementCollectionBase AddDetailNodes(LanguageElement parent, LanguageElementCollectionBase children)
		{
			if (parent == null)
				return null;
			for (int i = 0; i < children.Count; i++)
				AddDetailNode(parent, children[i] as LanguageElement);
			return children;
		}
		#endregion
		#region AddDetailNode
		public virtual LanguageElement AddDetailNode(LanguageElement parent, LanguageElement child)
		{
			if (parent == null)
				return null;
			parent.AddDetailNode(child);
			return child;
		}
		#endregion
		#region AddNodes
		public virtual LanguageElementCollectionBase AddNodes(LanguageElement parent, LanguageElementCollectionBase children)
		{			
			for (int i = 0; i < children.Count; i++)
				AddNode(parent, children[i] as LanguageElement);
			return children;
		}
		#endregion
		#region AddNode
		public virtual LanguageElement AddNode(LanguageElement parent, LanguageElement child)
		{
			if (parent == null)
				AddTopElement(child);				
			else 
				parent.AddNode(child);
			return child;
		}
		#endregion		
		#region GetExpression
		public virtual Expression GetExpression(object expression)
		{
			if (expression == null)
				return null;
			if (expression is Expression)
				return (Expression)expression;
			if (expression is string && IsAnIdentifier((string)expression))
				return BuildElementReference((string)expression);
			else
				return BuildPrimitive(expression.ToString());
		}
		#endregion
		#region GetSnippetExpressions
		public virtual ExpressionCollection GetSnippetExpressions(string[] values)
		{
			ExpressionCollection expresions = new ExpressionCollection();
			if (values != null)
			{
				int count = values.Length;
				for (int i = 0; i < count; i++)
					expresions.Add(BuildSnippetExpression(values[i]));
			}
			return expresions;
		}
		#endregion
		#region GetSnippetExpressions
		public virtual ExpressionCollection GetSnippetExpressions(LanguageElementCollection elements)
		{
			ExpressionCollection expresions = new ExpressionCollection();
			if (elements != null)
			{
				int count = elements.Count;
				for (int i = 0; i < count; i++)
					expresions.Add(BuildSnippetExpression(elements[i].Name));
			}
			return expresions;
		}
		#endregion
	#region GetInitializer
	public virtual LanguageElement GetInitializer(object initializer)
	{
	  if (initializer == null)
		return null;
	  if (initializer is LanguageElement)
		return (LanguageElement)initializer;
	  return BuildPrimitive(initializer.ToString());
	}
	#endregion
		#region ClearTopLevelElements
		public void ClearTopLevelElements()
		{
			_TopLevelElements.Clear();
			_RootElement.Nodes.Clear();
		}
		#endregion
	#region GenerateCode(string languageID, int precedingWhiteSpaceCount)
	public string GenerateCode(string languageID, int precedingWhiteSpaceCount)
	{
	  if (languageID == String.Empty)		
		return GenerateElement(_RootElement, precedingWhiteSpaceCount);
	  else
		return GenerateElement(_RootElement, languageID, precedingWhiteSpaceCount);
	}
	#endregion
		#region GenerateCode(string languageID)
		public string GenerateCode(string languageID)
		{
			if (languageID == String.Empty)		
				return GenerateElement(_RootElement);
			else 
				return GenerateElement(_RootElement, languageID);
		}
		#endregion
	#region GenerateCode()
		public string GenerateCode()
		{
			return GenerateCode(String.Empty);
		}
		#endregion
	public virtual DefineDirective AddDefineDirective(LanguageElement parent)
	{
	  return (DefineDirective)AddNode(parent, BuildDefineDirective());
	}
	public virtual DefineDirective BuildDefineDirective()
	{
	  return new DefineDirective();
	}
	public virtual UnaryOperatorExpression OpPointerDereference(object identifier)
	{
	  return new UnaryOperatorExpression(UnaryOperatorType.PointerDereference, GetExpression(identifier), false);
	}
	#region OpPlusPlus(object identifier)
		public virtual UnaryIncrement OpPlusPlus(object identifier)
		{
			return OpPlusPlus(identifier, true);
		}
		#endregion
		#region OpPlusPlus(object identifier, bool isPostIncrement)
		public virtual UnaryIncrement OpPlusPlus(object identifier, bool isPostIncrement)
		{
			UnaryIncrement lNewUnaryIncrement = new UnaryIncrement(GetExpression(identifier), isPostIncrement);
			return lNewUnaryIncrement;
		}
		#endregion
		#region OpMinusMinus(object identifier)
		public virtual UnaryDecrement OpMinusMinus(object identifier)
		{
			return OpMinusMinus(identifier, true);
		}
		#endregion
		#region OpMinusMinus(object identifier, bool isPostDecrement)
		public virtual UnaryDecrement OpMinusMinus(object identifier, bool isPostDecrement)
		{
			UnaryDecrement lNewUnaryDecrement = new UnaryDecrement(GetExpression(identifier), isPostDecrement);
			return lNewUnaryDecrement;
		}
		#endregion
		#region Op(object leftSide, string operatorText, object rightSide)
		public virtual BinaryOperatorExpression Op(object leftSide, string operatorText, object rightSide)
		{
			BinaryOperatorExpression lExpression = new BinaryOperatorExpression();
			lExpression.LeftSide = GetExpression(leftSide);
			lExpression.RightSide = GetExpression(rightSide);
			lExpression.OperatorText = operatorText;
			return lExpression;
		}
		#endregion
		#region Op(object leftSide, string operatorText, object rightSide)
		public virtual BinaryOperatorExpression Op(object leftSide, BinaryOperatorType op, object rightSide)
		{
			BinaryOperatorExpression lExpression = new BinaryOperatorExpression();
			lExpression.LeftSide = GetExpression(leftSide);
			lExpression.RightSide = GetExpression(rightSide);
			lExpression.OperatorText = String.Empty;
			lExpression.BinaryOperator = op;
			return lExpression;
		}
		#endregion
		#region Op(object leftSide, RelationalOperator operatorType, object rightSide)
		public virtual RelationalOperation Op(object leftSide, RelationalOperator operatorType, object rightSide)
		{
			RelationalOperation lExpression = new RelationalOperation(GetExpression(leftSide), operatorType, GetExpression(rightSide));
			return lExpression;
		}
		#endregion
		#region OpMinus
		public virtual BinaryOperatorExpression OpMinus(object leftSide, object rightSide)
		{
			return Op(leftSide, BinaryOperatorType.Subtract, rightSide);
		}
		#endregion
		#region OpPlus
		public virtual BinaryOperatorExpression OpPlus(object leftSide, object rightSide)
		{
			return Op(leftSide, BinaryOperatorType.Add, rightSide);
		}
		#endregion
		#region OpTimes
		public virtual BinaryOperatorExpression OpTimes(object leftSide, object rightSide)
		{
			return Op(leftSide, BinaryOperatorType.Multiply, rightSide);
		}
		#endregion
		#region OpDivide
		public virtual BinaryOperatorExpression OpDivide(object leftSide, object rightSide)
		{
			return Op(leftSide, BinaryOperatorType.Divide, rightSide);
		}
		#endregion
	public virtual BinaryOperatorExpression OpModulus(object leftSide, object rightSide)
	{
	  return Op(leftSide, BinaryOperatorType.Modulus, rightSide);
	}
		#region OpEquals
		public virtual RelationalOperation OpEquals(object leftSide, object rightSide)
		{
			return Op(leftSide, RelationalOperator.Equality, rightSide);
		}
		#endregion
		#region OpNotEquals
		public virtual RelationalOperation OpNotEquals(object leftSide, object rightSide)
		{
			return Op(leftSide, RelationalOperator.Inequality, rightSide);
		}
		#endregion
		#region OpGreaterOrEqual
		public virtual RelationalOperation OpGreaterOrEqual(object leftSide, object rightSide)
		{
			return Op(leftSide, RelationalOperator.GreaterOrEqual, rightSide);
		}
		#endregion
		#region OpGreaterThan
		public virtual RelationalOperation OpGreaterThan(object leftSide, object rightSide)
		{
			return Op(leftSide, RelationalOperator.GreaterThan, rightSide);
		}
		#endregion
		#region OpLessOrEqual
		public virtual RelationalOperation OpLessOrEqual(object leftSide, object rightSide)
		{
			return Op(leftSide, RelationalOperator.LessOrEqual, rightSide);
		}
		#endregion
		#region OpLessThan
		public virtual RelationalOperation OpLessThan(object leftSide, object rightSide)
		{
			return Op(leftSide, RelationalOperator.LessThan, rightSide);
		}
		#endregion
		#region OpLike
		public virtual RelationalOperation OpLike(object leftSide, object rightSide)
		{
			return Op(leftSide, RelationalOperator.Like, rightSide);
		}
		#endregion
	public virtual LogicalInversion AddLogicalInversion(LanguageElement parent)
	{
	  return (LogicalInversion)AddNode(parent, BuildLogicalInversion());
	}
	public virtual LogicalInversion AddLogicalInversion(LanguageElement parent, object expression)
	{
	  return (LogicalInversion)AddNode(parent, BuildLogicalInversion(expression));
	}
	public virtual LogicalInversion BuildLogicalInversion()
	{
	  return new LogicalInversion();
	}
	public virtual LogicalInversion BuildLogicalInversion(object expression)
	{
	  return new LogicalInversion(GetExpression(expression));
	}
		#region AddQualifiedElementReference
		public virtual ElementReferenceExpression AddQualifiedElementReference(LanguageElement parent, object source, string name)
		{
			return (ElementReferenceExpression)AddNode(parent, BuildQualifiedElementReference(source, name));			
		}
		#endregion
		#region AddMemberAccessReference
		public virtual ElementReferenceExpression AddMemberAccessReference(LanguageElement parent, object source, string name, MemberAccesOperatorType operatorType)
		{
			return (ElementReferenceExpression)AddNode(parent, BuildMemberAccessReference(source, name, operatorType));
		}
		#endregion
		#region AddObjectCreationExpression
		public virtual ObjectCreationExpression AddObjectCreationExpression(LanguageElement parent, string typeName, string[] arguments)
		{
			return (ObjectCreationExpression)AddNode(parent, BuildObjectCreationExpression(typeName, arguments));
		}
		#endregion
		#region AddObjectCreationExpression
		public virtual ObjectCreationExpression AddObjectCreationExpression(LanguageElement parent, string typeName, ExpressionCollection arguments)
		{
			return (ObjectCreationExpression)AddNode(parent, BuildObjectCreationExpression(typeName, arguments));
		}
		#endregion
		#region AddManagedObjectCreationExpression
		public virtual ObjectCreationExpression AddManagedObjectCreationExpression(LanguageElement parent, string typeName, string[] arguments)
		{
			return (ObjectCreationExpression)AddNode(parent, BuildManagedObjectCreationExpression(typeName, arguments));
		}
		#endregion
		#region AddManagedObjectCreationExpression
		public virtual ObjectCreationExpression AddManagedObjectCreationExpression(LanguageElement parent, string typeName, ExpressionCollection arguments)
		{
			return (ObjectCreationExpression)AddNode(parent, BuildManagedObjectCreationExpression(typeName, arguments));
		}
		#endregion
		#region AddAssignmentExpression
		public virtual AssignmentExpression AddAssignmentExpression(LanguageElement parent, object left, object right, AssignmentOperatorType operatorType)
		{
			return (AssignmentExpression)AddNode(parent, BuildAssignmentExpression(left, right, operatorType));
		}
		#endregion
	public virtual ParenthesizedExpression AddParenthesizedExpression(LanguageElement parent, object expression)
	{
	  return (ParenthesizedExpression)AddNode(parent, BuildParenthesizedExpression(expression));
	}
	public virtual ParenthesizedExpression BuildParenthesizedExpression(object expression)
	{
	  return new ParenthesizedExpression(GetExpression(expression));
	}
	public virtual ObjectInitializerExpression AddObjectInitializerExpression(LanguageElement parent)
	{
	  return (ObjectInitializerExpression)AddNode(parent, BuildObjectInitializerExpression());
	}
	public virtual ObjectInitializerExpression BuildObjectInitializerExpression()
	{
	  return new ObjectInitializerExpression();
	}
	public MemberInitializerExpression AddMemberInitializerExpression(LanguageElement parent, string name, object value)
	{
	  return (MemberInitializerExpression)AddNode(parent, BuildMemberInitializerExpression(name, value));
	}
	public MemberInitializerExpression AddMemberInitializerExpression(LanguageElement parent)
	{
	  return (MemberInitializerExpression)AddNode(parent, BuildMemberInitializerExpression());
	}
	public MemberInitializerExpression BuildMemberInitializerExpression(string name, object value)
	{
	  return new MemberInitializerExpression(name, GetExpression(value));
	}
	public MemberInitializerExpression BuildMemberInitializerExpression() 
	{
	  return new MemberInitializerExpression();
	}
	public MemberAccessExpression BuildMemberAccessExpression()
	{
	  return new MemberAccessExpression();
	}
	public virtual ArrayInitializerExpression AddArrayInitializerExpression(LanguageElement parent)
	{
	  return (ArrayInitializerExpression)AddNode(parent, BuildArrayInitializerExpression());
	}
	public virtual ArrayInitializerExpression AddArrayInitializerExpression(LanguageElement parent, string[] expressions)
	{
	  return (ArrayInitializerExpression)AddNode(parent, BuildArrayInitializerExpression(expressions));
	}
	public virtual ArrayInitializerExpression AddArrayInitializerExpression(LanguageElement parent, ExpressionCollection expressions)
	{
	  return (ArrayInitializerExpression)AddNode(parent, BuildArrayInitializerExpression(expressions));
	}
	public virtual ArrayInitializerExpression BuildArrayInitializerExpression()
	{
	  return new ArrayInitializerExpression();
	}
	public virtual ArrayInitializerExpression BuildArrayInitializerExpression(string[] expressions)
	{
	  return BuildArrayInitializerExpression(BuildExpressionCollection(expressions));
	}
	public virtual ArrayInitializerExpression BuildArrayInitializerExpression(ExpressionCollection expressions)
	{
	  if (expressions == null || expressions.Count == 0)
		return BuildArrayInitializerExpression();
	  return new ArrayInitializerExpression(expressions);
	}
	public virtual ArrayCreateExpression AddArrayCreateExpression(LanguageElement parent)
	{
	  return (ArrayCreateExpression)AddNode(parent, BuildArrayCreateExpression());
	}
	public virtual ArrayCreateExpression AddArrayCreateExpression(LanguageElement parent, TypeReferenceExpression type)
	{
	  return (ArrayCreateExpression)AddNode(parent, BuildArrayCreateExpression(type));
	}
	public virtual ArrayCreateExpression AddArrayCreateExpression(LanguageElement parent, TypeReferenceExpression type, ExpressionCollection dimensions)
	{
	  return (ArrayCreateExpression)AddNode(parent, BuildArrayCreateExpression(type, dimensions));
	}
	public virtual ArrayCreateExpression AddArrayCreateExpression(LanguageElement parent, TypeReferenceExpression type, ExpressionCollection dimensions, ArrayInitializerExpression initializer)
	{
	  return (ArrayCreateExpression)AddNode(parent, BuildArrayCreateExpression(type, dimensions, initializer));
	}
	public virtual ArrayCreateExpression BuildArrayCreateExpression()
	{
	  return new ArrayCreateExpression();
	}
	public virtual ArrayCreateExpression BuildArrayCreateExpression(TypeReferenceExpression type)
	{
	  return new ArrayCreateExpression(type);
	}
	public virtual ArrayCreateExpression BuildArrayCreateExpression(TypeReferenceExpression type, ExpressionCollection dimensions)
	{
	  return new ArrayCreateExpression(type, dimensions);
	}
	public virtual ArrayCreateExpression BuildArrayCreateExpression(TypeReferenceExpression type, ExpressionCollection dimensions, ArrayInitializerExpression initializer)
	{
	  return new ArrayCreateExpression(type, dimensions, initializer);
	}
	#region AddTypeCast
		public virtual TypeCastExpression AddTypeCast(LanguageElement parent, object type, object expression)
		{
			return (TypeCastExpression)AddNode(parent, BuildTypeCast(type, expression));
		}
		#endregion
	public virtual TypeCheck AddTypeCheck(LanguageElement parent, object expression, object type)
	{
	  return (TypeCheck)AddNode(parent, BuildTypeCheck(expression, type));
	}
	public virtual TypeCheck BuildTypeCheck(object expression, object type)
	{
	  return new TypeCheck(GetExpression(expression), GetExpression(type));
	}
	public virtual ConditionalTypeCast AddConditionalTypeCast(LanguageElement parent, object expression, object type)
	{
	  return (ConditionalTypeCast)AddNode(parent, BuildConditionalTypeCast(expression, type));
	}
	public virtual ConditionalTypeCast BuildConditionalTypeCast(object expression, object type)
	{
	  return new ConditionalTypeCast(GetExpression(expression), GetExpression(type));
	}
	public virtual TypeOfExpression AddTypeOfExpression(LanguageElement parent, TypeReferenceExpression type)
	{
	  return (TypeOfExpression)AddNode(parent, BuildTypeOfExpression(type));
	}
	public virtual TypeOfExpression AddTypeOfExpression(LanguageElement parent, Expression type)
	{
	  return (TypeOfExpression)AddNode(parent, BuildTypeOfExpression(type));
	}
	public virtual TypeOfExpression BuildTypeOfExpression(TypeReferenceExpression type)
	{
	  return new TypeOfExpression(type);
	}
	public virtual TypeOfExpression BuildTypeOfExpression(Expression type)
	{ 
	  return new TypeOfExpression(type);
	}
	public virtual SizeOfExpression AddSizeOfExpression(LanguageElement parent, TypeReferenceExpression type)
	{
	  return (SizeOfExpression)AddNode(parent, BuildSizeOfExpression(type));
	}
	public virtual SizeOfExpression AddSizeOfExpression(LanguageElement parent, Expression type)
	{
	  return (SizeOfExpression)AddNode(parent, BuildSizeOfExpression(type));
	}
	public virtual SizeOfExpression BuildSizeOfExpression(TypeReferenceExpression type)
	{
	  return new SizeOfExpression(type);
	}
	public virtual SizeOfExpression BuildSizeOfExpression(Expression type)
	{
	  return new SizeOfExpression(type);
	}
	public virtual DefaultValueExpression AddDefaultValueExpression(LanguageElement parent, TypeReferenceExpression type)
	{
	  return (DefaultValueExpression)AddNode(parent, BuildDefaultValueExpression(type));
	}
	public virtual DefaultValueExpression BuildDefaultValueExpression(TypeReferenceExpression type)
	{
	  return new DefaultValueExpression(type);
	}
	public virtual AnonymousMethodExpression AddAnonymousMethodExpression(LanguageElement parent)
	{
	  return (AnonymousMethodExpression)AddNode(parent, BuildAnonymousMethodExpression());
	}
	public virtual AnonymousMethodExpression BuildAnonymousMethodExpression()
	{
	  return new AnonymousMethodExpression();
	}
	public virtual LambdaExpression AddLambdaExpression(LanguageElement parent)
	{
	  return (LambdaExpression)AddNode(parent, BuildLambdaExpression());
	}
	public virtual LambdaExpression BuildLambdaExpression()
	{
	  return new LambdaExpression();
	}
	public virtual LambdaImplicitlyTypedParam AddLambdaImplicitlyTypedParam(LanguageElement parent, string name)
	{
	  return (LambdaImplicitlyTypedParam)AddNode(parent, BuildLambdaImplicitlyTypedParam(name));
	}
	public virtual LambdaImplicitlyTypedParam BuildLambdaImplicitlyTypedParam(string name)
	{
	  return new LambdaImplicitlyTypedParam(name);
	}
	public virtual IndexerExpression BuildIndexerExpression(Expression expression)
	{
	  return new IndexerExpression(expression);
	}
	public virtual IndexerExpression BuildIndexerExpression(Expression expression, string[] arguments)
	{
	  ExpressionCollection lCollection = BuildExpressionCollection();
	  foreach (var name in arguments)
		lCollection.Add(GetExpression(name));
	  return BuildIndexerExpression(expression, lCollection);
	}
	public virtual IndexerExpression BuildIndexerExpression(Expression expression, ExpressionCollection arguments)
	{
	  IndexerExpression lExpression = new IndexerExpression(expression);
	  lExpression.AddArguments(arguments);
	  return lExpression;
	}
	public virtual TypeParameter AddTypeParameter(LanguageElement parent, string name)
	{
	  return (TypeParameter)AddNode(parent, BuildTypeParameter(name));
	}
	public virtual TypeParameter BuildTypeParameter(string name)
	{
	  return new TypeParameter(name);
	}
	public virtual ClassTypeParameterConstraint AddClassTypeParameterConstraint(LanguageElement parent, string name, SourceRange range)
	{
	  return (ClassTypeParameterConstraint)AddNode(parent, BuildClassTypeParameterConstraint(name, range));
	}
	public virtual ClassTypeParameterConstraint AddClassTypeParameterConstraint(LanguageElement parent)
	{
	  return (ClassTypeParameterConstraint)AddNode(parent, BuildClassTypeParameterConstraint());
	}
	public virtual ClassTypeParameterConstraint BuildClassTypeParameterConstraint()
	{
	  return BuildClassTypeParameterConstraint("class", SourceRange.Empty);
	}
	public virtual ClassTypeParameterConstraint BuildClassTypeParameterConstraint(string name, SourceRange range)
	{
	  return new ClassTypeParameterConstraint(name, range);
	}
	public virtual StructTypeParameterConstraint AddStructTypeParameterConstraint(LanguageElement parent)
	{
	  return (StructTypeParameterConstraint)AddNode(parent, BuildStructTypeParameterConstraint());
	}
	public virtual StructTypeParameterConstraint AddStructTypeParameterConstraint(LanguageElement parent, string name, SourceRange range)
	{
	  return (StructTypeParameterConstraint)AddNode(parent, BuildStructTypeParameterConstraint(name, range));
	}
	public virtual StructTypeParameterConstraint BuildStructTypeParameterConstraint()
	{
	  return BuildStructTypeParameterConstraint("struct", SourceRange.Empty);
	}
	public virtual StructTypeParameterConstraint BuildStructTypeParameterConstraint(string name, SourceRange range)
	{
	  return new StructTypeParameterConstraint(name, range);
	}
	public virtual NewTypeParameterConstraint AddNewTypeParameterConstraint(LanguageElement parent)
	{
	  return (NewTypeParameterConstraint)AddNode(parent, BuildNewTypeParameterConstraint());
	}
	public virtual NewTypeParameterConstraint AddNewTypeParameterConstraint(LanguageElement parent, string name, SourceRange range)
	{
	  return (NewTypeParameterConstraint)AddNode(parent, BuildNewTypeParameterConstraint(name, range));
	}
	public virtual NewTypeParameterConstraint BuildNewTypeParameterConstraint()
	{
	  return BuildNewTypeParameterConstraint("new()", SourceRange.Empty);
	}
	public virtual NewTypeParameterConstraint BuildNewTypeParameterConstraint(string name, SourceRange range)
	{
	  return new NewTypeParameterConstraint(name, range);
	}
	public virtual NamedTypeParameterConstraint AddNamedTypeParameterConstraint(LanguageElement parent, object expression)
	{
	  return (NamedTypeParameterConstraint)AddNode(parent, BuildNamedTypeParameterConstraint(expression));
	}
	public virtual NamedTypeParameterConstraint BuildNamedTypeParameterConstraint(object expression)
	{
	  return new NamedTypeParameterConstraint(GetTypeReferenceExpression(expression));
	}
	public virtual GenericModifier AddGenericModifier(LanguageElement parent, TypeParameterCollection types)
	{
	  return (GenericModifier)AddNode(parent, BuildGenericModifier(types));
	}
	public virtual GenericModifier AddGenericModifier(LanguageElement parent, string[] names)
	{
	  return (GenericModifier)AddNode(parent, BuildGenericModifier(names));
	}
	public virtual GenericModifier BuildGenericModifier(TypeParameterCollection types) 
	{
	  return new GenericModifier(types);
	}
	public virtual GenericModifier BuildGenericModifier(string[] names)
	{
	  return new GenericModifier(BuildTypeParameterCollection(names));
	}
	public virtual TypeParameterCollection BuildTypeParameterCollection()
	{
	  return new TypeParameterCollection();
	}
	public virtual TypeParameterCollection BuildTypeParameterCollection(string[] names)
	{
	  TypeParameterCollection collection = BuildTypeParameterCollection();
	  foreach (var name in names)
		collection.Add(new TypeParameter(name));
	  return collection;
	}
	public virtual ExpressionCollection BuildExpressionCollection()
	{
	  return new ExpressionCollection();
	}
	public virtual ExpressionCollection BuildExpressionCollection(string[] names)
	{
	  ExpressionCollection expressions = BuildExpressionCollection();
	  foreach (var name in names)
		expressions.Add(BuildPrimitive(name));
	  return expressions;
	}
	public virtual LanguageElementCollection BuildLanguageElementCollection()
	{
	  return new LanguageElementCollection();
	}
	public virtual TypeReferenceExpressionCollection BuildTypeReferenceExpressionCollection()
	{
	  return new TypeReferenceExpressionCollection();
	}
	public virtual ElementList BuildElementList()
	{
	  return new ElementList();
	}
	public virtual ElementList BuildElementList(LanguageElementCollection list)
	{
	  return new ElementList(list);
	}
	public virtual ElementList BuildElementList(NodeList list)
	{
	  return new ElementList(list);
	}
	public virtual CaseClausesList BuildCaseClausesList()
	{
	  return new CaseClausesList();
	}
	public virtual CommentCollection BuildCommentCollection()
	{
	  return new CommentCollection();	
	}
		#region AddNamespaceReference
		public NamespaceReference AddNamespaceReference(LanguageElement parent, string namespaceName)
		{
			return (NamespaceReference)AddNode(parent, BuildNamespaceReference(namespaceName));
		}
		#endregion
		#region AddNamespaceReference
		public NamespaceReference AddNamespaceReference(LanguageElement parent, string aliasName, string expression)
		{
			return (NamespaceReference)AddNode(parent, BuildNamespaceReference(aliasName, expression));
		}
		#endregion
	public virtual Namespace AddNamespace(LanguageElement parent, string name) 
	{
	  return (Namespace)AddNode(parent, BuildNamespace(name));
	}
	public virtual Namespace BuildNamespace(string name)
	{
	  return new Namespace(name);
	}
		#region AddClass
		public Class AddClass(LanguageElement parent, string className)
		{
			return (Class)AddNode(parent, BuildClass(className));
		}
		#endregion
		#region AddStruct
		public Struct AddStruct(LanguageElement parent, string structName)
		{
			return (Struct)AddNode(parent, BuildStruct(structName));
		}
		#endregion
		#region AddInterface
		public Interface AddInterface(LanguageElement parent, string interfaceName)
		{
			return (Interface)AddNode(parent, BuildInterface(interfaceName));
		}
		#endregion
		#region AddInterfaceClass
		public Interface AddInterfaceClass(LanguageElement parent, string interfaceName)
		{
			return (Interface)AddNode(parent, BuildInterfaceClass(interfaceName));
		}
		#endregion
		#region AddInterfaceStruct
		public Interface AddInterfaceStruct(LanguageElement parent, string interfaceName)
		{
			return (Interface)AddNode(parent, BuildInterfaceStruct(interfaceName));
		}
		#endregion
		#region AddDelegateDefinition(LanguageElement parent, string delegateName)
		public DelegateDefinition AddDelegateDefinition(LanguageElement parent, string delegateName)
		{
			return (DelegateDefinition)AddNode(parent, BuildDelegateDefinition(delegateName));
		}
		#endregion
		#region AddDelegateDefinition(LanguageElement parent, string delegateName, LanguageElementCollection parameters)
		public DelegateDefinition AddDelegateDefinition(LanguageElement parent, string delegateName, LanguageElementCollection parameters)
		{
			return (DelegateDefinition)AddNode(parent, BuildDelegateDefinition(delegateName, parameters));
		}
		#endregion
	public DelegateDefinition AddDelegateDefinition(LanguageElement parent, string delegateName, string delegateType)
	{
	  return (DelegateDefinition)AddNode(parent, BuildDelegateDefinition(delegateName, delegateType));
	}
	public DelegateDefinition AddDelegateDefinition(LanguageElement parent, string delegateName, string delegateType, LanguageElementCollection parameters)
	{
	  return (DelegateDefinition)AddNode(parent, BuildDelegateDefinition(delegateName, delegateType, parameters));
	}
	public DelegateDefinition AddDelegateDefinition(LanguageElement parent, string delegateName, TypeReferenceExpression delegateType)
	{
	  return (DelegateDefinition)AddNode(parent, BuildDelegateDefinition(delegateName, delegateType));
	}
	public DelegateDefinition AddDelegateDefinition(LanguageElement parent, string delegateName, TypeReferenceExpression delegateType, LanguageElementCollection parameters)
	{
	  return (DelegateDefinition)AddNode(parent, BuildDelegateDefinition(delegateName, delegateType, parameters));
	}
	public virtual DelegateDefinition BuildDelegateDefinition(string delegateName, string delegateType)
	{
	  DelegateDefinition delegateDefinition = BuildDelegateDefinition(delegateName);
	  delegateDefinition.MemberType = delegateType;
	  if (delegateType != null)
	  {
		TypeReferenceExpression type = BuildTypeReference(delegateType);
		if (type != null)
		{
		  delegateDefinition.MemberTypeReference = type;
		  delegateDefinition.AddDetailNode(type);
		}
	  }
	  delegateDefinition.MemberType = delegateType;
	  delegateDefinition.MemberTypeReference = BuildTypeReference(delegateType);
	  return delegateDefinition;
	}
	public virtual DelegateDefinition BuildDelegateDefinition(string delegateName, string delegateType, LanguageElementCollection parameters)
	{
	  DelegateDefinition delegateDefinition = BuildDelegateDefinition(delegateName, parameters);
	  delegateDefinition.MemberType = delegateType;
	  if (delegateType != null)
	  {
		TypeReferenceExpression type = BuildTypeReference(delegateType);
		if (type != null)
		{
		  delegateDefinition.MemberTypeReference = type;
		  delegateDefinition.AddDetailNode(type);
		}
	  }
	  return delegateDefinition;
	}
	public virtual DelegateDefinition BuildDelegateDefinition(string delegateName, TypeReferenceExpression delegateType)
	{
	  DelegateDefinition delegateDefinition = BuildDelegateDefinition(delegateName);
	  delegateDefinition.MemberType = delegateType.ToString();
	  delegateDefinition.MemberTypeReference = delegateType;
	  delegateDefinition.AddDetailNode(delegateType);
	  return delegateDefinition;
	}
	public virtual DelegateDefinition BuildDelegateDefinition(string delegateName, TypeReferenceExpression delegateType, LanguageElementCollection parameters)
	{
	  DelegateDefinition delegateDefinition = BuildDelegateDefinition(delegateName, parameters);
	  delegateDefinition.MemberType = delegateType.ToString();
	  delegateDefinition.MemberTypeReference = delegateType;
	  delegateDefinition.AddDetailNode(delegateType);
	  return delegateDefinition;
	}
	public virtual Enumeration AddEnumeration(LanguageElement parent)
	{
	  return (Enumeration)AddNode(parent, BuildEnumeration());
	}
	public virtual Enumeration AddEnumeration(LanguageElement parent, string name)
	{
	  return (Enumeration)AddNode(parent, BuildEnumeration(name));
	}
	public virtual Enumeration BuildEnumeration()
	{
	  return new Enumeration();
	}
	public virtual Enumeration BuildEnumeration(string name)
	{
	  return new Enumeration(name);
	}
		#region AddBreak
		public Break AddBreak(LanguageElement parent)
		{
			return (Break)AddNode(parent, BuildBreak());
		}
		#endregion
		#region AddAbort
		public Abort AddAbort(LanguageElement parent)
		{
			return (Abort)AddNode(parent, BuildAbort());
		}
		#endregion
		#region AddContinue
		public Continue AddContinue(LanguageElement parent)
		{
			return (Continue)AddNode(parent, BuildContinue());
		}
		#endregion
		#region AddExit
		public Exit AddExit(LanguageElement parent)
		{
			return (Exit)AddNode(parent, BuildExit());
		}
		#endregion
	public virtual With AddWith(LanguageElement parent)
	{
	  return (With)AddNode(parent, BuildWith());
	}
	public virtual With AddWith(LanguageElement parent, object expression)
	{
	  return (With)AddNode(parent, BuildWith(expression));
	}
	public virtual With AddWith(LanguageElement parent, object expression, LanguageElementCollection block)
	{
	  return (With)AddNode(parent, BuildWith(expression, block));
	}
	public virtual With BuildWith()
	{
	  return new With();
	}
	public virtual With BuildWith(object expression)
	{
	  With withStatement = BuildWith();
	  withStatement.Expression = GetExpression(expression);
	  return withStatement;
	}
	public virtual With BuildWith(object expression, LanguageElementCollection block)
	{
	  return new With(GetExpression(expression), block);
	}
		#region AddReturn
		public Return AddReturn(LanguageElement parent)
		{
			return (Return)AddNode(parent, BuildReturn());
		}
		#endregion
		#region AddReturn(LanguageElement parent, object expression)
		public Return AddReturn(LanguageElement parent, object expression)
		{
			return (Return)AddNode(parent, BuildReturn(expression));
		}
		#endregion
		#region BuildAbort
		public virtual Abort BuildAbort()
		{
			return new Abort();
		}
		#endregion
		#region BuildBreak
		public virtual Break BuildBreak()
		{
			return new Break();
		}
		#endregion
		#region BuildContinue
		public virtual Continue BuildContinue()
		{
			return new Continue();
		}
		#endregion
		#region BuildExit
		public virtual Exit BuildExit()
		{
			return new Exit();
		}
		#endregion
		#region BuildReturn
		public virtual Return BuildReturn()
		{
			return new Return();
		}
		#endregion
		#region BuildReturn(object expression)
		public virtual Return BuildReturn(object expression)
		{
			Return lNewReturn = new Return();
			Expression lExpression = GetExpression(expression);
			if (lExpression != null)
			{
				lNewReturn.Expression = lExpression;
				AddDetailNode(lNewReturn, lExpression);
			}
			return lNewReturn;
		}
		#endregion
		#region BuildNamespaceReference(string namespaceName)
		public NamespaceReference BuildNamespaceReference(string namespaceName)
		{
			return new NamespaceReference(namespaceName);
		}
		#endregion
		#region BuildNamespaceReference(string aliasName, string expression)
		public NamespaceReference BuildNamespaceReference(string aliasName, string expression)
		{
			return new NamespaceReference(aliasName, expression);
		}
		#endregion
		#region BuildClass
		public virtual Class BuildClass(string className)
		{
			return new Class(className);
		}
		#endregion
		#region BuildStruct
		public virtual Struct BuildStruct(string structName)
		{
			return new Struct(structName);
		}
		#endregion
		#region BuildInterface
		public virtual Interface BuildInterface(string interfaceName)
		{
			return new Interface(interfaceName);
		}
		#endregion
		#region BuildInterfaceClass
		public virtual Interface BuildInterfaceClass(string interfaceName)
		{
			return BuildInterface(interfaceName);
		}
		#endregion
		#region BuildInterfaceStruct
		public virtual Interface BuildInterfaceStruct(string interfaceName)
		{
			return BuildInterface(interfaceName);
		}
		#endregion
		#region BuildDelegateDefinition(string delegateName)
		public virtual DelegateDefinition BuildDelegateDefinition(string delegateName)
		{
			return new DelegateDefinition(delegateName);
		}
		#endregion
		#region BuildDelegateDefinition(string delegateName, LanguageElementCollection parameters)
		public virtual DelegateDefinition BuildDelegateDefinition(string delegateName, LanguageElementCollection parameters)
		{
			DelegateDefinition delegateDefinition = new DelegateDefinition(delegateName);
			if (parameters != null)
				delegateDefinition.Parameters.AddRange(parameters);
			return delegateDefinition;
		}
		#endregion
		#region AddAssignment(LanguageElement parent, object leftSide, object assignment, AssignmentOperatorType assignmentOperatorType)
		public Assignment AddAssignment(LanguageElement parent, object identifier, object assignedValue, AssignmentOperatorType operatorType)
		{
			return (Assignment)AddNode(parent, BuildAssignment(identifier, assignedValue, operatorType));
		}
		#endregion
		#region AddAssignment(LanguageElement parent, object identifier, object assignedValue)
		public Assignment AddAssignment(LanguageElement parent, object identifier, object assignedValue)
		{
			return AddAssignment(parent, identifier, assignedValue, AssignmentOperatorType.Assignment);
		}
		#endregion
		#region AddAccessSpecifiers(AccessSpecifiedElement parent, AccessSpecifiers accessSpecifiers)
		public AccessSpecifiers AddAccessSpecifiers(AccessSpecifiedElement parent, AccessSpecifiers accessSpecifiers)
		{
			if (parent == null)
				return null;
			parent.AccessSpecifiers = accessSpecifiers;
			return accessSpecifiers;
		}
		#endregion
		#region AddAccessSpecifiers
		public AccessSpecifiers AddAccessSpecifiers(AccessSpecifiedElement parent, bool isVirtual, bool isOverride, bool isOverloads, bool isStatic)
		{			
			return AddAccessSpecifiers(parent, BuildAccessSpecifiers(isVirtual, isOverride, isOverloads, isStatic));
		}
		#endregion
		#region AddCase
		public Case AddCase(LanguageElement parent, object expression)
		{
			return (Case)AddNode(parent, BuildCase(expression));
		}
		#endregion
	public Case AddCase(LanguageElement parent)
	{
	  return (Case)AddNode(parent, BuildCase());
	}
	public Case AddCase(LanguageElement parent, bool isDefault)
	{
	  return (Case)AddNode(parent, BuildCase(isDefault));
	}
	public virtual Case BuildCase()
	{
	  return BuildCase(false);
	}
	public virtual Case BuildCase(bool isDefault)
	{
	  Case caseStatement = new Case();
	  caseStatement.IsDefault = isDefault;
	  return caseStatement;
	}
		#region AddCatch(LanguageElement parent, string exceptionType, string exceptionVariable)
		public Catch AddCatch(LanguageElement parent, string exceptionType, string exceptionVariable)
		{
			return (Catch)AddNode(parent, BuildCatch(exceptionType, exceptionVariable));
		}
		#endregion
		#region AddCatch(LanguageElement parent)
		public Catch AddCatch(LanguageElement parent)
		{
			return (Catch)AddNode(parent, BuildCatch());
		}
		#endregion
		#region AddDo
		public Do AddDo(LanguageElement parent, object condition)
		{
			return (Do)AddNode(parent, BuildDo(condition));
		}
		#endregion
		#region AddElse
		public Else AddElse(LanguageElement parent)
		{
			return (Else)AddNode(parent, BuildElse());
		}
		#endregion
		#region AddElseIf
		public ElseIf AddElseIf(LanguageElement parent, object expression)
		{
			return (ElseIf)AddNode(parent, BuildElseIf(expression));
		}
		#endregion
		#region AddFinally
		public Finally AddFinally(LanguageElement parent)
		{
			return (Finally)AddNode(parent, BuildFinally());
		}
		#endregion
		#region AddForEach
		public ForEach AddForEach(LanguageElement parent, string elementType, string elementVariable, object collection)
		{
			return (ForEach)AddNode(parent, BuildForEach(elementType, elementVariable, collection));
		}
		#endregion
		#region AddFor(Expression endCondition)
		public For AddFor(LanguageElement parent, Expression endCondition)
		{
			return (For)AddNode(parent, BuildFor(endCondition));
		}
		#endregion
		#region AddFor(string iteratorVar, Expression endCondition)
		public For AddFor(LanguageElement parent, string iteratorVar, Expression endCondition)
		{
			return (For)AddNode(parent, BuildFor(iteratorVar, endCondition));
		}
		#endregion
		#region AddInitializedVariable
		public InitializedVariable AddInitializedVariable(LanguageElement parent, string variableType, string variableName, object initialValue)
		{
			return (InitializedVariable)AddNode(parent, BuildInitializedVariable(variableType, variableName, initialValue));
		}
		#endregion
	#region AddInitializedVariable
	public virtual InitializedVariable AddInitializedVariable(LanguageElement parent, TypeReferenceExpression variableType, string variableName, Expression value)
	{
	  return (InitializedVariable)AddNode(parent, BuildInitializedVariable(variableType, variableName, value));
	}
		#endregion
	public virtual Const AddConst(LanguageElement parent, string variableType, string variableName, object expression)
	{
	  return (Const)AddNode(parent, BuildConst(variableType, variableName, expression));
	}
	public virtual Const BuildConst(string variableType, string variableName, object expression)
	{
	  return new Const(variableType, variableName, GetExpression(expression));
	}
	#region AddImplicitVariable
		public ImplicitVariable AddImplicitVariable(LanguageElement parent, string variableName, object initialValue)
		{
			return (ImplicitVariable)AddNode(parent, BuildImplicitVariable(variableName, initialValue));
		}
		#endregion
		#region AddIf
		public If AddIf(LanguageElement parent, object expression)
		{
			return (If)AddNode(parent, BuildIf(expression));
		}
		#endregion		
		#region AddLogicalOperation
		public LogicalOperation AddLogicalOperation(LanguageElement parent, object leftSide, LogicalOperator logicalOperator, object rightSide)
		{
			return (LogicalOperation)AddNode(parent, BuildLogicalOperation(leftSide, logicalOperator, rightSide));
		}
		#endregion
	public MethodCall AddMethodCall(LanguageElement parent, string name)
	{
	  return (MethodCall)AddNode(parent, BuildMethodCall(name));
	}
	public MethodCall AddMethodCall(LanguageElement parent, string name, ExpressionCollection arguments)
	{
	  return (MethodCall)AddNode(parent, BuildMethodCall(name, arguments));
	}
	public MethodCall AddMethodCall(LanguageElement parent, string name, string[] arguments, object qualifier)
		{
			return (MethodCall)AddNode(parent, BuildMethodCall(name, arguments, qualifier));
		}
		#region AddMethodCall
		public MethodCall AddMethodCall(LanguageElement parent, object source)
		{
			return (MethodCall)AddNode(parent, BuildMethodCall(source));
		}
		#endregion
		#region AddMethodCall
		public MethodCall AddMethodCall(LanguageElement parent, string name, string[] arguments)
		{
			return (MethodCall)AddNode(parent, BuildMethodCall(name, arguments));
		}	
		#endregion
	#region AddMethodCall
		public MethodCall AddMethodCall(LanguageElement parent, string name, ExpressionCollection arguments, object qualifier)
		{
			return (MethodCall)AddNode(parent, BuildMethodCall(name, arguments, qualifier));
		}
		#endregion
		#region AddRelationalOperation
		public RelationalOperation AddLogicalOperation(LanguageElement parent, object leftSide, RelationalOperator relationalOperator, object rightSide)
		{
			return (RelationalOperation)AddNode(parent, BuildRelationalOperation(leftSide, relationalOperator, rightSide));
		}
		#endregion
	public ConditionalExpression AddConditionalExpression(LanguageElement parent, object condition, object trueExpression, object falseExpression)
		{
			return (ConditionalExpression)AddNode(parent, BuildConditionalExpression(condition, trueExpression, falseExpression));
		}
	public ConditionalExpression BuildConditionalExpression(object condition, object trueExpression, object falseExpression)
	{
	  return new ConditionalExpression(GetExpression(condition), GetExpression(trueExpression), GetExpression(falseExpression));
	}
		#region AddSelect
		public Switch AddSelect(LanguageElement parent, object expression)
		{
			return AddSwitch(parent, expression);
		}
		#endregion
		#region AddSwitch
		public Switch AddSwitch(LanguageElement parent, object expression)
		{
			return (Switch)AddNode(parent, BuildSwitch(expression));
		}
		#endregion
		#region AddTry
		public Try AddTry(LanguageElement parent)
		{
			return (Try)AddNode(parent, BuildTry());
		}
		#endregion
		#region AddThisMethodCall
		public MethodCall AddThisMethodCall(LanguageElement parent, string name, ExpressionCollection arguments)
		{
			return (MethodCall)AddNode(parent, BuildThisMethodCall(name, arguments));
		}
		#endregion
		#region AddVariable
		public Variable AddVariable(LanguageElement parent, string variableType, string variableName)
		{
			return (Variable)AddNode(parent, BuildVariable(variableType, variableName));
		}
	public Variable AddVariable(LanguageElement parent, TypeReferenceExpression variableType, string variableName)
	{
	  return (Variable)AddNode(parent, BuildVariable(variableType, variableName));
	}
		#endregion
		#region AddWhile
		public While AddWhile(LanguageElement parent, object condition)
		{
			return (While)AddNode(parent, BuildWhile(condition));
		}
		#endregion	
		#region AddStatement
		public LanguageElement AddStatement(LanguageElement parent, object statement)
		{			
			return AddNode(parent, BuildStatement(statement));
		}
		#endregion
	public virtual Statement AddWrapStatement(LanguageElement parent, object expression)
	{
	  return (Statement)AddNode(parent, BuildWrapStatement(expression));
	}
	public virtual Statement BuildWrapStatement(object expression)
	{
	  Statement lStatement = new Statement();
	  lStatement.SourceExpression = GetExpression(expression);
	  return lStatement;
	}
		#region AddBaseMethodCall
		public virtual void AddBaseMethodCall(LanguageElement parent, string name, ExpressionCollection arguments)
		{
			AddNode(parent, BuildBaseMethodCall(name, arguments));
		}
		#endregion
		#region AddBaseMethodCall
		public virtual void AddBaseMethodCall(LanguageElement parent, string name, ExpressionCollection arguments, string codeBefore, string codeAfter)
		{
			if (parent == null)
				return;
			if (codeBefore != null && codeBefore.Length > 0)
				AddNode(parent, BuildStatement(codeBefore));
			AddBaseMethodCall(parent, name, arguments);
			if (codeAfter != null && codeAfter.Length > 0)
				AddNode(parent, BuildStatement(codeAfter));
		}
		#endregion
	public virtual Block AddBlock(LanguageElement parent)
	{
	  return (Block)AddNode(parent, BuildBlock());
	}
	public virtual Block BuildBlock()
	{
	  return new Block();
	}
	public virtual UnsafeStatement AddUnsafeStatement(LanguageElement parent)
	{
	  return (UnsafeStatement)AddNode(parent, BuildUnsafeStatement());
	}
	public virtual UnsafeStatement BuildUnsafeStatement()
	{
	  return new UnsafeStatement();
	}
	public virtual Fixed AddFixed(LanguageElement parent)
	{
	  return (Fixed)AddNode(parent, BuildFixed());
	}
	public virtual Fixed AddFixed(LanguageElement parent, object initializer)
	{
	  return (Fixed)AddNode(parent, BuildFixed(initializer));
	}
	public virtual Fixed BuildFixed()
	{
	  return new Fixed();
	}
	public virtual Fixed BuildFixed(object initializer)
	{
	  Fixed lFixed = new Fixed();
	  lFixed.AddInitializer(GetInitializer(initializer));
	  return lFixed;
	}
	public virtual UsingStatement AddUsingStatement(LanguageElement parent)
	{
	  return (UsingStatement)AddNode(parent, BuildUsingStatement());
	}
	public virtual UsingStatement AddUsingStatement(LanguageElement parent, object initializer)
	{
	  return (UsingStatement)AddNode(parent, BuildUsingStatement(initializer));
	}
	public virtual UsingStatement BuildUsingStatement()
	{
	  return new UsingStatement();
	}
	public virtual UsingStatement BuildUsingStatement(object initializer)
	{
	  UsingStatement lUsingStatement = new UsingStatement();
	  lUsingStatement.AddInitializer(GetExpression(initializer));
	  return lUsingStatement;
	}
	public virtual Checked AddChecked(LanguageElement parent)
	{
	  return (Checked)AddNode(parent, BuildChecked());
	}
	public virtual Checked BuildChecked()
	{
	  return new Checked();
	}
	public virtual CheckedExpression AddCheckedExpression(LanguageElement parent, object expression)
	{
	  return (CheckedExpression)AddNode(parent, BuildCheckedExpression(expression));
	}
	public virtual CheckedExpression BuildCheckedExpression(object expression)
	{
	  return new CheckedExpression(GetExpression(expression));
	}
	public virtual Unchecked AddUnchecked(LanguageElement parent)
	{
	  return (Unchecked)AddNode(parent, BuildUnchecked());
	}
	public virtual Unchecked BuildUnchecked()
	{
	  return new Unchecked();
	}
	public virtual UncheckedExpression AddUncheckedExpression(LanguageElement parent, object expression)
	{
	  return (UncheckedExpression)AddNode(parent, BuildUncheckedExpression(expression));
	}
	public virtual UncheckedExpression BuildUncheckedExpression(object expression)
	{
	  return new UncheckedExpression(GetExpression(expression));
	}
	public virtual CTypeExpression BuildCTypeExpression(TypeReferenceExpression type, object expression)
	{ 
	  return new CTypeExpression(type, GetExpression(expression)); 
	}
	public virtual Is BuildIs(object left, object right)
	{
	  return new Is(GetExpression(left), GetExpression(right));
	}
	public virtual IsNot BuildIsNot(object left, object right)
	{
	  return new IsNot(GetExpression(left), GetExpression(right));
	}
	protected virtual CaseClause AddCaseClause(Case parent, CaseClause caseClause)
	{ 
	  if (parent == null)
		AddTopElement(caseClause);
	  else
		parent.AddCaseClause(caseClause);
	  return caseClause;
	}
	public virtual CaseClause AddCaseClause(Case parent)
	{
	  return (CaseClause)AddCaseClause(parent, BuildCaseClause());
	}
	public virtual CaseClause AddCaseClause(Case parent, object startExpression)
	{
	  return (CaseClause)AddCaseClause(parent, BuildCaseClause(startExpression));
	}
	public virtual CaseClause AddCaseClause(Case parent, object startExpression, object endExpression)
	{
	  return (CaseClause)AddCaseClause(parent, BuildCaseClause(startExpression, endExpression));
	}
	public virtual CaseClause BuildCaseClause()
	{
	  return new CaseClause();
	}
	public virtual CaseClause BuildCaseClause(object startExpression)
	{
	  CaseClause caseClause = BuildCaseClause();
	  caseClause.StartExpression = GetExpression(startExpression);
	  return caseClause;
	}
	public virtual CaseClause BuildCaseClause(object startExpression, object endExpression)
	{
	  CaseClause caseClause = BuildCaseClause();
	  caseClause.StartExpression = GetExpression(startExpression);
	  Expression end = GetExpression(endExpression);
	  if (end != null)
	  {
		caseClause.EndExpression = end;
		caseClause.IsRangeCheckClause = true;
	  }
	  return caseClause;
	}
		#region AddInnerParam
		[Obsolete("Use AddInParam instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Param AddInnerParam(Set parent, string paramType, string name)
		{
			return AddInParam(parent, paramType, name);
		}
		#endregion
		#region AddInnerParam
		[Obsolete("Use AddInParam instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Param AddInnerParam(MemberWithParameters parent, string paramType, string name)
		{
			return AddInParam(parent, paramType, name);
		}
		#endregion
		#region AddInnerParam
		public Param AddInParam(Set parent, string paramType, string name)
		{			
			return (Param)AddDetailNode(parent, BuildInParam(paramType, name));
		}
		#endregion		
		#region AddInnerParam
		public Param AddInParam(MemberWithParameters parent, string paramType, string name)
		{			
			return AddParam(parent, BuildInParam(paramType, name));
		}
		#endregion
		#region AddRefParam
		public Param AddRefParam(MemberWithParameters parent, string paramType, string name)
		{
			return AddParam(parent, BuildRefParam(paramType, name));
		}
		#endregion
		#region AddOutParam
		public Param AddOutParam(MemberWithParameters parent, string paramType, string name)
		{
			return AddParam(parent, BuildOutParam(paramType, name));
		}
		#endregion
		#region AddParamsArray
		public Param AddParamArray(MemberWithParameters parent, string paramType, string name)
		{
			return AddParam(parent, BuildParamArray(paramType, name));
		}
		#endregion		
	public virtual ExtensionMethodParam AddExtensionMethodParam(MemberWithParameters parent, string paramType, string paramName)
		{
	  return (ExtensionMethodParam)AddParam(parent, BuildExtensionMethodParam(paramType, paramName));
		}
	public virtual ExtensionMethodParam BuildExtensionMethodParam(string paramType, string paramName)
	{
	  return new ExtensionMethodParam(paramType, paramName);
	}
	public virtual AttributeVariableInitializer AddAttributeVariableInitializer(MethodCall parent, object leftSide, object rightSide)
	{
	  return (AttributeVariableInitializer)AddMethodArgument(parent, BuildAttributeVariableInitializer(leftSide, rightSide));
	}
	public virtual AttributeVariableInitializer BuildAttributeVariableInitializer(object leftSide, object rightSide)
	{
	  AttributeVariableInitializer initializer  = new AttributeVariableInitializer();
	  initializer.LeftSide = GetExpression(leftSide);
	  initializer.RightSide = GetExpression(rightSide);
	  return initializer;
	}
	public virtual Expression AddMethodArgument(MethodCall parent, object argument)
	{
	  return (Expression)AddMethodArgument(parent, GetExpression(argument));
	}
	public virtual ArgumentDirectionExpression BuildArgumentDirectionExpression()
	{
	  return new ArgumentDirectionExpression();
	}
	public virtual ArgumentDirectionExpression BuildArgumentDirectionExpression(ArgumentDirection direction, Expression expression)
	{
	  return new ArgumentDirectionExpression(direction, expression);
	}
		#region AddProperty(LanguageElement parent, string memberType, string name)
		public Property AddProperty(LanguageElement parent, string memberType, string name)
		{
			return (Property)AddNode(parent, BuildProperty(memberType, name));
		}
		#endregion
		#region AddProperty(LanguageElement parent, string memberType, string name, Get getter, Set setter)
		public Property AddProperty(LanguageElement parent, string memberType, string name, Get getter, Set setter)
		{
			return (Property)AddNode(parent, BuildProperty(memberType, name, getter, setter));
		}
		#endregion
		#region AddMethod
		public Method AddMethod(LanguageElement parent, string memberType, string name)
		{
			return (Method)AddNode(parent, BuildMethod(memberType, name));
		}
		#endregion
		#region AddConstructor
		public Method AddConstructor(TypeDeclaration parent)
		{
			if (parent == null)
				return null;
			return (Method)AddNode(parent, BuildConstructor(parent.Name));
		}
		#endregion
	#region AddDestructor
	public Method AddDestructor(TypeDeclaration parent)
	{
	  if (parent == null)
		return null;
	  return (Method)AddNode(parent, BuildDestructor(parent.Name));
	}
	#endregion
		#region AddThisConstructorInitializer
		public ConstructorInitializer AddThisConstructorInitializer(Method parent, ExpressionCollection arguments)
		{
			if (parent == null || parent.MethodType != MethodTypeEnum.Constructor)
				return null;		
			return AddConstructorInitializer(parent, BuildThisConstructorInitializer(arguments));			
		}
		#endregion
	#region AddBaseConstructorInitializer(Method parent)
	public ConstructorInitializer AddBaseConstructorInitializer(Method parent)
	{
	  if (parent == null || parent.MethodType != MethodTypeEnum.Constructor)
		return null;
	  return AddConstructorInitializer(parent, BuildBaseConstructorInitializer());
	}
	#endregion
	#region AddBaseConstructorInitializer(Method parent, ExpressionCollection arguments)
		public ConstructorInitializer AddBaseConstructorInitializer(Method parent, ExpressionCollection arguments)
		{
			if (parent == null || parent.MethodType != MethodTypeEnum.Constructor)
				return null;			
			return AddConstructorInitializer(parent, BuildBaseConstructorInitializer(arguments));			
		}
		#endregion
		#region AddExpressionConstructorInitializer
		public ConstructorInitializer AddExpressionConstructorInitializer(Method parent, Expression expr, ExpressionCollection arguments)
		{
			if (parent == null || parent.MethodType != MethodTypeEnum.Constructor || expr == null)
				return null;			
			return AddConstructorInitializer(parent, BuildExpressionConstructorInitializer(expr, arguments));
		}
		#endregion
		#region AddGetter(Property parent)
		public Get AddGetter(Property parent)
		{
			return (Get)AddNode(parent, BuildGetter());
		}
		#endregion
		#region AddSetter(Property parent)
		public Set AddSetter(Property parent)
		{
			return (Set)AddNode(parent, BuildSetter());
		}
		#endregion
	public virtual EnumElement AddEnumElement(LanguageElement parent)
	{
	  return (EnumElement)AddNode(parent, BuildEnumElement());
	}
	public virtual EnumElement AddEnumElement(LanguageElement parent, string name)
	{
	  return (EnumElement)AddNode(parent, BuildEnumElement(name));
	}
	public virtual EnumElement AddEnumElement(LanguageElement parent, string name, object valueExpression)
	{
	  return (EnumElement)AddNode(parent, BuildEnumElement(name, valueExpression));
	}
	public virtual EnumElement BuildEnumElement()
	{
	  return new EnumElement();
	}
	public virtual EnumElement BuildEnumElement(string name)
	{
	  return new EnumElement(name);
	}
	public virtual EnumElement BuildEnumElement(string name, object valueExpression)
	{
	  return new EnumElement(name, GetExpression(valueExpression));
	}
	public virtual Event AddEvent(LanguageElement parent)
	{
	  return (Event)AddNode(parent, BuildEvent());
	}
	public virtual Event AddEvent(LanguageElement parent, string name)
	{
	  return (Event)AddNode(parent, BuildEvent(name));
	}
	public virtual Event AddEvent(LanguageElement parent, string eventName, string eventType)
	{
	  return (Event)AddNode(parent, BuildEvent(eventName, eventType));
	}
	public virtual Event AddEvent(LanguageElement parent, string eventName, TypeReferenceExpression eventType)
	{
	  return (Event)AddNode(parent, BuildEvent(eventName, eventType));
	}
	public virtual Event BuildEvent()
	{
	  return new Event();
	}
	public virtual Event BuildEvent(string name)
	{
	  return new Event(name);
	}
	public virtual Event BuildEvent(string eventName, string eventType)
	{
	  Event lEvent = BuildEvent(eventName);
	  lEvent.MemberType = eventType;
	  if (eventType != null)
	  {
		TypeReferenceExpression type = BuildTypeReference(eventType);
		if (type != null)
		{
		  lEvent.MemberTypeReference = type;
		  lEvent.AddDetailNode(type);
		}
	  }
	  return lEvent;
	}
	public virtual Event BuildEvent(string eventName, TypeReferenceExpression eventType)
	{
	  Event lEvent = BuildEvent(eventName);
	  lEvent.MemberType = eventType.ToString();
	  lEvent.MemberTypeReference = eventType;
	  lEvent.AddDetailNode(eventType);
	  return lEvent;
	}
	public virtual EventAdd AddEventAdd(LanguageElement parent)
	{
	  return (EventAdd)AddNode(parent, BuildEventAdd());
	}
	public virtual EventAdd BuildEventAdd()
	{
	  return new EventAdd();
	}
	public virtual EventRemove AddEventRemove(LanguageElement parent)
	{
	  return (EventRemove)AddNode(parent, BuildEventRemove());
	}
	public virtual EventRemove BuildEventRemove()
	{
	  return new EventRemove();
	}
	public virtual RaiseEvent AddRaiseEvent(LanguageElement parent)
	{
	  return (RaiseEvent)AddNode(parent, BuildRaiseEvent());
	}
	public virtual RaiseEvent AddRaiseEvent(LanguageElement parent, object expression)
	{
	  return (RaiseEvent)AddNode(parent, BuildRaiseEvent(expression));
	}
	public virtual RaiseEvent BuildRaiseEvent()
	{
	  return new RaiseEvent();
	}
	public virtual RaiseEvent BuildRaiseEvent(object expression)
	{
	  return new RaiseEvent(GetExpression(expression));
	}
	public virtual AddHandler AddAddHandler(LanguageElement parent)
	{
	  return (AddHandler)AddNode(parent, BuildAddHandler());
	}
	public virtual AddHandler AddAddHandler(LanguageElement parent, object expression, object address)
	{
	  return (AddHandler)AddNode(parent, BuildAddHandler(expression, address));
	}
	public virtual AddHandler BuildAddHandler()
	{
	  return new AddHandler();
	}
	public virtual AddHandler BuildAddHandler(object expression, object address)
	{
	  return new AddHandler(GetExpression(expression), GetExpression(address));
	}
	public virtual RemoveHandler AddRemoveHandler(LanguageElement parent)
	{
	  return (RemoveHandler)AddNode(parent, BuildRemoveHandler());
	}
	public virtual RemoveHandler AddRemoveHandler(LanguageElement parent, object expression, object address)
	{
	  return (RemoveHandler)AddNode(parent, BuildRemoveHandler(expression, address));
	}
	public virtual RemoveHandler BuildRemoveHandler()
	{
	  return new RemoveHandler();
	}
	public virtual RemoveHandler BuildRemoveHandler(object expression, object address)
	{
	  return new RemoveHandler(GetExpression(expression), GetExpression(address));
	}
		#region AddAttributeSection(CodeElement target)
		public AttributeSection AddAttributeSection(CodeElement target)
		{			
			return (AttributeSection)AddAttributeSection(target, BuildAttributeSection());
		}
		#endregion
		#region AddAttribute(AttributeSection parent, string name)
		public Attribute AddAttribute(AttributeSection parent, string name)
		{			
			return (Attribute)AddAttribute(parent, BuildAttribute(name));
		}
		#endregion
		#region AddAttribute(AttributeSection parent, object source, string name)
		public Attribute AddAttribute(AttributeSection parent, object qualifier, string name)
		{
			return (Attribute)AddAttribute(parent, BuildAttribute(qualifier, name));
		}
		#endregion		
		#region AddArgument
		public Expression AddArgument(Attribute parent, Expression arg)
		{
			if (parent == null)
				return (Expression)AddNode(null, arg);
			parent.Arguments.Add(arg);
			parent.AddDetailNode(arg);
			return arg;
		}
		#endregion
		#region AddSnippetCodeElement
		public SnippetCodeElement AddSnippetCodeElement(LanguageElement parent, string code)
		{
			return (SnippetCodeElement)AddNode(parent, BuildSnippetCodeElement(code));
		}
		#endregion
		#region AddComment
		public virtual Comment AddComment(CodeElement parent, string text, CommentType commentType)
		{
			return (Comment)AddComment(parent, BuildComment(text, commentType));
		}
		#endregion
	public virtual XmlDocComment AddXmlDocComment(CodeElement parent, string text)
	{
	  XmlDocComment lDocComment = BuildXmlDocComment(text);
	  if (lDocComment == null)
		return null;
	  if (parent == null)
		AddTopElement(lDocComment);
	  else
		parent.AddCommentNode(lDocComment);
	  return lDocComment;
	}
	public virtual XmlDocComment BuildXmlDocComment(string text)
	{
	  if (string.IsNullOrEmpty(text))
		return null;
	  XmlDocComment lDocComment = new XmlDocComment();
	  lDocComment.Name = text;
	  lDocComment.ParseXmlNodes();
	  return lDocComment;
	}
		#region BuildQualifiedElementReference
		public virtual ElementReferenceExpression BuildQualifiedElementReference(object source, string name)
		{
			Expression sourceExpr = GetExpression(source);
			if (sourceExpr == null)
				return new QualifiedElementReference(name);
			return new QualifiedElementReference(sourceExpr, name, SourceRange.Empty);
		}
		#endregion
		#region BuildMemberAccessReference
		public virtual ElementReferenceExpression BuildMemberAccessReference(object source, string name, MemberAccesOperatorType operatorType)
		{
			return BuildQualifiedElementReference(source, name);
		}
		#endregion
	#region BuildTypeReference
	public virtual TypeReferenceExpression BuildTypeReference(object source, string name)
	{
	  Expression sourceExpr = GetExpression(source);
	  TypeReferenceExpression typeRef = BuildTypeReference(name, false);
	  if (sourceExpr != null)
		typeRef.Qualifier = sourceExpr;
	  return typeRef;
	}
	public virtual TypeReferenceExpression BuildTypeReference(string name)
	{
	  return BuildTypeReference(name, true);
	}
	public virtual TypeReferenceExpression BuildTypeReference(string name, bool useSimpleTypeName)
	{
	  string typeName = useSimpleTypeName ? GetSimpleTypeName(name) : name;
	  return new TypeReferenceExpression(typeName);
	}
	public virtual TypeReferenceExpression BuildTypeReference(TypeReferenceType type, TypeReferenceExpression baseType)
	{
	  TypeReferenceExpression lTypeRef = new TypeReferenceExpression(type);
	  lTypeRef.SetBaseTypeAfterCreation(baseType);
	  return lTypeRef;
	}
	public virtual TypeReferenceExpression BuildTypeReference(TypeReferenceExpression type, int rank)
	{
	  return new TypeReferenceExpression(type, rank);
	}
	#endregion
	#region BuildElementReference
	public virtual ElementReferenceExpression BuildElementReference(string identifier)
		{
			return new ElementReferenceExpression(identifier);
		}
		#endregion
	public virtual MethodReferenceExpression BuildMethodReference(string name)
	{
	  return new MethodReferenceExpression(name);
	}
	public virtual MethodReferenceExpression BuildMethodReference(Expression source, string name)
	{
	  return new MethodReferenceExpression(source, name);
	}
	public virtual AddressOfExpression BuildAddressOfExpression()
	{
	  return new AddressOfExpression();
	}
	public virtual AddressOfExpression BuildAddressOfExpression(object expression)
	{
	  return new AddressOfExpression(GetExpression(expression));
	}
	#region BuildPrimitive(string primitiveValue)
	public virtual PrimitiveExpression BuildPrimitive(string primitiveValue)
		{
			return new PrimitiveExpression(primitiveValue);			
		}
		#endregion
		#region BuildPrimitiveFromObject(object expression)
		public virtual PrimitiveExpression BuildPrimitiveFromObject(object primitiveValue)
		{
			PrimitiveExpression lNewExpression = PrimitiveExpression.FromObject(primitiveValue);		
			return lNewExpression;
		}
		#endregion
		#region BuildSnippetExpression
		public virtual SnippetExpression BuildSnippetExpression(string code)
		{			
			return new SnippetExpression(code);
		}
		#endregion
		#region BuildSnippetStatement
		public virtual SnippetCodeStatement BuildSnippetStatement(string code)
		{			
			return new SnippetCodeStatement(code);
		}
		#endregion
	public virtual SnippetCodeStatement BuildSnippetStatement(string code, bool addStatementTerminator)
	{
	  return new SnippetCodeStatement(code, addStatementTerminator);
	}
	public virtual SnippetCodeStatement BuildSnippetStatement(string code, bool addStatementTerminator, bool addBlock)
	{
	  return new SnippetCodeStatement(code, addStatementTerminator, addBlock);
	}
		#region BuildBaseReferenceExpression
		public virtual BaseReferenceExpression BuildBaseReferenceExpression()
		{
			return new BaseReferenceExpression();
		}
		#endregion
		#region BuildThisReferenceExpression
		public virtual ThisReferenceExpression BuildThisReferenceExpression()
		{
			return new ThisReferenceExpression();
		}
		#endregion
	#region BuildMyClassExpression
	public virtual MyClassExpression BuildMyClassExpression()
	{
	  return new MyClassExpression();
	}
	#endregion
		#region BuildMethodCallExpression(object source)
		public virtual MethodCallExpression BuildMethodCallExpression(object source)
		{			
			Expression lSource = GetExpression(source);
			if (lSource == null)
				return null;
			MethodCallExpression lNewMethodCallExpression = new MethodCallExpression(lSource);			
			return lNewMethodCallExpression;
		}
		#endregion
		#region BuildMethodCallExpression
		public virtual MethodCallExpression BuildMethodCallExpression(object source, ExpressionCollection arguments)
		{			
			MethodCallExpression lNewMethodCallExpression = BuildMethodCallExpression(source);
			lNewMethodCallExpression.Arguments.AddRange(arguments);
			return lNewMethodCallExpression;
		}
	public virtual MethodCallExpression BuildMethodCallExpression(object source, string[] values)
	{
	  return BuildMethodCallExpression(source, GetSnippetExpressions(values));
	}
		#endregion
		#region BuildObjectCreationExpression
	public virtual ObjectCreationExpression BuildObjectCreationExpression(string typeName, string[] arguments)
	{
	  return BuildObjectCreationExpression(typeName, GetSnippetExpressions(arguments));
	}
	public virtual ObjectCreationExpression BuildObjectCreationExpression(TypeReferenceExpression typeReference, string[] arguments)
		{
	  return BuildObjectCreationExpression(typeReference, GetSnippetExpressions(arguments));
		}
		public virtual ObjectCreationExpression BuildObjectCreationExpression(string typeName, ExpressionCollection arguments)
		{
	  return BuildObjectCreationExpression(BuildTypeReference(typeName), arguments);
		}
	public virtual ObjectCreationExpression BuildObjectCreationExpression(TypeReferenceExpression typeReference, ExpressionCollection arguments)
	{
	  if (arguments == null || arguments.Count == 0)
		return new ObjectCreationExpression(typeReference);
	  return new ObjectCreationExpression(typeReference, arguments);
	}
		#endregion
		#region BuildManagedObjectCreationExpression
		public virtual ObjectCreationExpression BuildManagedObjectCreationExpression(string typeName, string[] arguments)
		{
			return BuildObjectCreationExpression(typeName, arguments);
		}
		#endregion
		#region BuildManagedObjectCreationExpression
		public virtual ObjectCreationExpression BuildManagedObjectCreationExpression(string typeName, ExpressionCollection arguments)
		{
			return BuildObjectCreationExpression(typeName, arguments);
		}
		#endregion
		#region BuildAssignmentExpression
		public virtual AssignmentExpression BuildAssignmentExpression(object left, object right, AssignmentOperatorType operatorType)
		{
			AssignmentExpression assignmentExpression = new AssignmentExpression();
			assignmentExpression.LeftSide = GetExpression(left);
			assignmentExpression.RightSide = GetExpression(right);
			assignmentExpression.AssignmentOperator = operatorType;
			return assignmentExpression;
		}
		#endregion
		#region BuildTypeCast
		public virtual TypeCastExpression BuildTypeCast(object type, object expression)
		{
			return new TypeCastExpression(GetTypeReferenceExpression(type), GetExpression(expression));
		}
		#endregion
		#region BuildAccessSpecifiers(bool isVirtual, bool isOverride, bool isOverloads, bool isStatic)
		public virtual AccessSpecifiers BuildAccessSpecifiers(bool isVirtual, bool isOverride, bool isOverloads, bool isStatic)
		{
			AccessSpecifiers lNewAccessSpecifiers = new AccessSpecifiers();
			lNewAccessSpecifiers.IsVirtual = isVirtual;
			lNewAccessSpecifiers.IsOverride = isOverride;
			lNewAccessSpecifiers.IsOverloads = isOverloads;
			lNewAccessSpecifiers.IsStatic = isStatic;
			return lNewAccessSpecifiers;
		}
		#endregion
		#region BuildAssignment(object leftSide, object assignment, AssignmentOperatorType assignmentOperatorType)
		public virtual Assignment BuildAssignment(object identifier, object assignedValue, AssignmentOperatorType operatorType)
		{
			Assignment lNewAssignment = new Assignment();
			lNewAssignment.LeftSide = GetExpression(identifier);
			lNewAssignment.Expression = GetExpression(assignedValue);
			lNewAssignment.AssignmentOperator = operatorType;
			return lNewAssignment;
		}
		#endregion
		#region BuildAssignment(object leftSide, object assignedValue)
		public virtual Assignment BuildAssignment(object identifier, object assignedValue)
		{
			return BuildAssignment(identifier, assignedValue, AssignmentOperatorType.Assignment);
		}
		#endregion
		#region BuildAttributeSection
		public virtual AttributeSection BuildAttributeSection()
		{
			AttributeSection lNewAttributeSection = new AttributeSection();
			return lNewAttributeSection;
		}
		#endregion
		#region BuildAttribute
		public virtual Attribute BuildAttribute(string name)
		{
			Attribute lNewAttribute = new Attribute();
			lNewAttribute.Name = name;			
			return lNewAttribute;
		}
		#endregion
		#region BuildAttribute
		public virtual Attribute BuildAttribute(object qualifier, string name)
		{
			Expression sourceExpr = GetExpression(qualifier);
			Attribute lNewAttribute = new Attribute();
			lNewAttribute.Name = name;
			if (sourceExpr != null)
				lNewAttribute.Qualifier = sourceExpr;
			return lNewAttribute;
		}
		#endregion		
		#region BuildCase
		public virtual Case BuildCase(object expression)
		{
			Case lNewCase = new Case();
			lNewCase.Expression = GetExpression(expression);
			return lNewCase;
		}
		#endregion
		#region BuildCatch(string exceptionType, string exceptionVariable)
		public virtual Catch BuildCatch(string exceptionType, string exceptionVariable)
		{
			Catch lNewCatch = new Catch();
			Variable ex = BuildVariable(exceptionType, exceptionVariable);
			if (ex != null)
			{
				lNewCatch.Exception = ex;
				lNewCatch.AddDetailNode(ex);
			}
			return lNewCatch;
		}
		#endregion
		#region BuildCatch()
		public virtual Catch BuildCatch()
		{
			return new Catch();
		}
		#endregion
		#region BuildDo
		public virtual Do BuildDo(object condition)
		{
			Do lNewDo = new Do();
			lNewDo.SetCondition(GetExpression(condition));
			return lNewDo;
		}
		#endregion
		#region BuildElse
		public virtual Else BuildElse()
		{
			return new Else();
		}
		#endregion
		#region BuildElseIf
		public virtual ElseIf BuildElseIf(object expression)
		{
			ElseIf lNewElseIf = new ElseIf();
			lNewElseIf.SetExpression(GetExpression(expression));
			return lNewElseIf;
		}
		#endregion
		#region BuildFinally
		public virtual Finally BuildFinally()
		{
			return new Finally();
		}
		#endregion
		#region BuildForEach
		public virtual ForEach BuildForEach(string elementType, string elementVariable, object collection)
		{
			ForEach lForEach = new ForEach();
			lForEach.AddDetailNode(BuildVariable(elementType, elementVariable));
			lForEach.AddDetailNode(GetExpression(collection));
			lForEach.Collection = collection.ToString();		
			return lForEach;
		}
		#endregion
		#region BuildFor(Expression endCondition)
		public virtual For BuildFor(Expression endCondition)
		{
			For lNewFor = new For();
			lNewFor.Condition = endCondition;
			return lNewFor;
		}
		#endregion
		#region BuildFor(string iteratorVar, Expression endCondition)
		public virtual For BuildFor(string iteratorVar, Expression endCondition)
		{
			For lNewFor = BuildFor(endCondition);
			InitializedVariable lInitializedVariable = BuildInitializedVariable(STR_SystemInt32, iteratorVar, "0");
			UnaryIncrement lUnaryIncrement = OpPlusPlus(iteratorVar);
			lNewFor.AddInitializer(lInitializedVariable);
			lNewFor.AddIncrementor(lUnaryIncrement);
			return lNewFor;
		}
		#endregion		
		#region BuildIf
		public virtual If BuildIf(object expression)
		{
			If lNewIf = new If();
			lNewIf.SetExpression(GetExpression(expression));
			return lNewIf;
		}
		#endregion
		#region BuildLogicalOperation
		public virtual LogicalOperation BuildLogicalOperation(object leftSide, LogicalOperator logicalOperator, object rightSide)
		{
			LogicalOperation lNewOperation = new LogicalOperation(GetExpression(leftSide), logicalOperator, GetExpression(rightSide));
			return lNewOperation;
		}
		#endregion
	#region BuildMethod
		public virtual Method BuildMethod(string memberType, string name)
		{
			Method lNewMethod = new Method(memberType, name);
	  SetMethodType(lNewMethod, memberType);
			return lNewMethod;
		}
		#endregion
		#region BuildConstructor
		public virtual Method BuildConstructor(string className)
		{
			Method lNewMethod = new Method(className);				
			lNewMethod.MethodType = MethodTypeEnum.Constructor;
			return lNewMethod;
		}
		#endregion
	#region BuildDestructor
	public virtual Method BuildDestructor(string className)
	{
	  Method lNewMethod = new Method(className);
	  lNewMethod.MethodType = MethodTypeEnum.Destructor;
	  return lNewMethod;
	}
	#endregion
		#region BuildThisConstructorInitializer
		public virtual ConstructorInitializer BuildThisConstructorInitializer(ExpressionCollection arguments)
		{
			ConstructorInitializer init = new ConstructorInitializer(); 
			init.Target = InitializerTarget.ThisClass;
			init.Arguments.AddRange(arguments);
			AddDetailNodes(init, arguments);
			return init;
		}
		#endregion
	#region BuildBaseConstructorInitializer
	public virtual ConstructorInitializer BuildBaseConstructorInitializer()
	{
	  ConstructorInitializer init = new ConstructorInitializer();
	  init.Target = InitializerTarget.Ancestor;
	  return init;
	}
	#endregion
	#region BuildBaseConstructorInitializer(ExpressionCollection arguments)
		public virtual ConstructorInitializer BuildBaseConstructorInitializer(ExpressionCollection arguments)
		{			
			ConstructorInitializer init = BuildBaseConstructorInitializer(); 
			init.Arguments.AddRange(arguments);
			AddDetailNodes(init, arguments);
			return init;
		}
		#endregion
		#region BuildExpressionConstructorInitializer
		public virtual ConstructorInitializer BuildExpressionConstructorInitializer(Expression expr, ExpressionCollection arguments)
		{			
			ConstructorInitializer init = new ConstructorInitializer();
			init.Target = InitializerTarget.Expression;
			AddNode(init, expr);
			init.Arguments.AddRange(arguments);
			AddDetailNodes(init, arguments);
			return init;
		}
		#endregion
		#region BuildConstructorInitializer
		public virtual ConstructorInitializer BuildConstructorInitializer(string name, ExpressionCollection arguments)
		{
			ConstructorInitializer initializer = new ConstructorInitializer();
			initializer.Target = InitializerTarget.Ancestor;
			initializer.Arguments.AddRange(arguments);
			AddDetailNodes(initializer, arguments);
			return initializer;
		}
		#endregion
	public virtual MethodCall BuildMethodCall(string name)
	{
	  return BuildMethodCall(name, new string[] { }, null);
	}
		#region BuildMethodCall
		public virtual MethodCall BuildMethodCall(object source)
		{
			MethodCall lNewMethodCall = new MethodCall();			
			AddSource(lNewMethodCall, source);
			return lNewMethodCall;
		}
		#endregion
	#region BuildMethodCall
	public virtual MethodCall BuildMethodCall(string name, string[] arguments)
	{
	  return BuildMethodCall(name, GetSnippetExpressions(arguments), null);
	}
	#endregion
	#region BuildMethodCall
	public virtual MethodCall BuildMethodCall(string name, string[] arguments, object qualifier)
	{
	  return BuildMethodCall(name, GetSnippetExpressions(arguments), qualifier);
	}
	#endregion
		#region BuildMethodCall
	public virtual MethodCall BuildMethodCall(string name, ExpressionCollection arguments)
		{
			return BuildMethodCall(name, arguments, null);
		}
		#endregion
		#region BuildMethodCall
		public virtual MethodCall BuildMethodCall(string name, ExpressionCollection arguments, object qualifier)
		{
			if (name == null)
				return null;
			MethodCall call = BuildMethodCall(new MethodReferenceExpression(name, SourceRange.Empty));
			if (arguments != null)
				call.Arguments.AddRange(arguments);
			if (qualifier == null)
				return call;
			IHasQualifier callReference = call.Qualifier as IHasQualifier;
			if (callReference == null)
				return call;
			AddSource(callReference, GetExpression(qualifier));
			return call;
		}
		#endregion
		#region BuildBaseMethodCall
		public virtual MethodCall BuildBaseMethodCall(string name, ExpressionCollection arguments)
		{
			return BuildMethodCall(name, arguments, BuildBaseReferenceExpression());
		}
		#endregion
		#region BuildThisMethodCall
		public virtual MethodCall BuildThisMethodCall(string name, ExpressionCollection arguments)
		{
			return BuildMethodCall(name, arguments, BuildThisReferenceExpression());
		}
		#endregion		
		#region BuildInnerParam
		[Obsolete("Use BuildInParam instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Param BuildInnerParam(string paramType, string name)
		{
			return BuildInParam(paramType, name);
		}
		#endregion
		#region BuildInnerParam
		public virtual Param BuildInParam(string paramType, string name)
		{
			return BuildParameter(paramType, name, ArgumentDirection.In);
		}				
		#endregion
		#region BuildRefParam
		public virtual Param BuildRefParam(string paramType, string name)
		{
			return BuildParameter(paramType, name, ArgumentDirection.Ref);
		}				
		#endregion
		#region BuildRelationalOperation
		public virtual RelationalOperation BuildRelationalOperation(object leftSide, RelationalOperator relationalOperator, object rightSide)
		{
			RelationalOperation lNewOperation = new RelationalOperation(GetExpression(leftSide), relationalOperator, GetExpression(rightSide));
			return lNewOperation;
		}
		#endregion
		#region BuildOutParam
		public virtual Param BuildOutParam(string paramType, string name)
		{
			return BuildParameter(paramType, name, ArgumentDirection.Out);
		}				
		#endregion
		#region BuildParamArray
		public virtual Param BuildParamArray(string paramType, string name)
		{
			return BuildParameter(paramType, name, ArgumentDirection.ParamArray);
		}				
		#endregion
		#region BuildParamArray
		public virtual Param BuildParameter(string paramType, string name, ArgumentDirection direction)
		{
			Param lNewParam = new Param(GetSimpleTypeName(paramType), name);
			lNewParam.Direction = direction;
			return lNewParam;
		}				
		#endregion		
		#region BuildProperty
		public virtual Property BuildProperty(string memberType, string name)
		{
			Property lNewProperty = new Property(memberType, name);	
			lNewProperty.SetAccessSpecifiers(new AccessSpecifiers());
			return lNewProperty;
		}
		#endregion
		#region BuildProperty
		public virtual Property BuildProperty(string memberType, string name, Get getter, Set setter)
		{
			Property lNewProperty = null;
			if (memberType != null && memberType != String.Empty)
				lNewProperty = new Property(memberType, name);
			else
				lNewProperty = new Property(name);
			if (getter != null)
				AddNode(lNewProperty, getter);
			if (setter != null)
				AddNode(lNewProperty, setter);
			lNewProperty.SetAccessSpecifiers(new AccessSpecifiers());
			return lNewProperty;
		}
		#endregion
		#region BuildGetter
		public virtual Get BuildGetter()
		{
			Get lNewGet = new Get();			
			return lNewGet;
		}
		#endregion
		#region BuildSetter
		public virtual Set BuildSetter()
		{
			Set lNewSet = new Set();			
			return lNewSet;
		}
		#endregion
		#region BuildSelect
		public Switch BuildSelect(object expression)		
		{
			return BuildSwitch(expression);
		}
		#endregion
		#region BuildSnippetCodeElement
		public SnippetCodeElement BuildSnippetCodeElement(string code)
		{			
			return new SnippetCodeElement(code);
		}
		#endregion
		#region BuildSwitch
		public virtual Switch BuildSwitch(object expression)
		{
			Switch lNewSwitch = new Switch();
			lNewSwitch.Expression = GetExpression(expression);
			return lNewSwitch;
		}
		#endregion
		#region BuildTry
		public virtual Try BuildTry()
		{
			return new Try();
		}
		#endregion
		#region BuildThrow
		public virtual Throw BuildThrow(Expression expression)
		{
			Throw @throw = new Throw();
			@throw.Expression = expression;
			@throw.AddDetailNode(expression);
			return @throw;
		}
		public virtual Throw BuildThrow(Object expression)
		{
			Throw @throw = new Throw();
			Expression exp = GetExpression(expression);
			@throw.Expression = GetExpression(exp);
			@throw.AddDetailNode(exp);
			return @throw;
		}
		#endregion
		#region BuildComment	
		public virtual Comment BuildComment(string text, CommentType commentType)
		{			 
			return BuildComment(text, 0, 1, commentType, 2);
		}
		protected virtual Comment BuildComment(string text, int startPos, int endPos, CommentType commentType)
		{			 
			return BuildComment(text, startPos, endPos, commentType, 2);
		}
		protected virtual Comment BuildComment(string text, int startPos, int endPos, CommentType commentType, int textStartOffset)
		{
			Comment comment = new Comment();
			comment.Name = text;
			comment.StartPos = startPos;
			comment.EndPos = endPos;
			comment.CommentType = commentType;
			comment.SetTextStartOffset(textStartOffset);
			return comment;
		}
		#endregion		
		#region BuildVariable
		public virtual Variable BuildVariable(string variableType, string variableName)
		{
			Variable lNewVariable = new Variable();
			lNewVariable.Name = variableName;
			string lSimpleTypeName = GetSimpleTypeName(variableType);
			lNewVariable.MemberType = lSimpleTypeName;
			if (lSimpleTypeName != null)
			{
				TypeReferenceExpression type = BuildTypeReference(lSimpleTypeName);
				if (type != null)
				{
					lNewVariable.MemberTypeReference = type;
					lNewVariable.AddDetailNode(type);
				}
			}
			return lNewVariable;
		}
		#endregion
		#region BuildVariable
		public virtual Variable BuildVariable(TypeReferenceExpression variableType, string variableName)
		{
			Variable lNewVariable = new Variable();
			lNewVariable.Name = variableName;
			if (variableType != null)
			{
				lNewVariable.MemberType = variableType.ToString();
				lNewVariable.MemberTypeReference = variableType;
				lNewVariable.AddDetailNode(variableType);
			}
			return lNewVariable;
		}
		#endregion
		#region BuildInitializedVariable
		public virtual InitializedVariable BuildInitializedVariable(string variableType, string variableName, object initialValue)
		{
			return BuildInitializedVariable(BuildTypeReference(variableType), variableName, GetExpression(initialValue));
		}
		#endregion
		#region BuildInitializedVariable
		public virtual InitializedVariable BuildInitializedVariable(TypeReferenceExpression variableType, string variableName, Expression value)
		{
			InitializedVariable lNewVariable = new InitializedVariable();
			lNewVariable.Name = variableName;
			if (variableType != null)
			{
				lNewVariable.MemberType = variableType.ToString();
				lNewVariable.MemberTypeReference = variableType;
				lNewVariable.AddDetailNode(variableType);
			}
			lNewVariable.Expression = value;
			lNewVariable.AddDetailNode(value);
			return lNewVariable;
		}
		#endregion
		#region BuildImplicitVariable
		public virtual ImplicitVariable BuildImplicitVariable(string variableName, object initialValue)
		{
			return BuildImplicitVariable(variableName, GetExpression(initialValue));
		}
		#endregion
		#region BuildImplicitVariable
		public virtual ImplicitVariable BuildImplicitVariable(string variableName, Expression value)
		{
			ImplicitVariable lNewVariable = new ImplicitVariable(variableName);
			lNewVariable.Expression = value;
			lNewVariable.AddDetailNode(value);
			return lNewVariable;
		}
		#endregion		
		#region BuildWhile
		public virtual While BuildWhile(object condition)
		{
			While lNewWhile = new While();
			lNewWhile.SetCondition(GetExpression(condition));
			return lNewWhile;
		}
		#endregion
		#region BuildStatement(object statement)
		public virtual LanguageElement BuildStatement(object statement)
		{
			LanguageElement lStatement = null;
			if (statement is Statement)
				lStatement = (Statement)statement;
			else if (statement is string)
				lStatement = new SnippetCodeStatement((string)statement);
			else if (statement is LanguageElement)
				lStatement = (LanguageElement)statement;
			return lStatement;
		}
		#endregion
		#region TopLevelElements
		public NodeList TopLevelElements
		{
			get
			{
				return _RootElement.Nodes;
			}
		}
		#endregion
	}
}
