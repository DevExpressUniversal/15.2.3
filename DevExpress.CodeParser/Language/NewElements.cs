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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class NestedMethod : Block
	{
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.NestedMethod;
			}
		}
	}
	#region Expressions	
	public class ExpressionTypeArgument : TypeReferenceExpression
	{
		Expression _SourceExpression = null;
		public ExpressionTypeArgument()
		{
		}
		public ExpressionTypeArgument(Expression sourceExpression)
		{
			if (sourceExpression != null)
			{
				_SourceExpression = sourceExpression;
				this.SetRange(sourceExpression.Range);
				AddNode(_SourceExpression);
				InternalName = _SourceExpression.Name;
			}
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ExpressionTypeArgument))
				return;
			ExpressionTypeArgument sourceElement = (ExpressionTypeArgument)source;
			_SourceExpression = sourceElement._SourceExpression;
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_SourceExpression == oldElement)
				_SourceExpression = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ExpressionTypeArgument expression = new ExpressionTypeArgument();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override string ToString()
		{
			if (_SourceExpression != null)
				return _SourceExpression.ToString ();
			else
				return base.ToString();
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ExpressionTypeArgument;
			}
		}
		public Expression SourceExpression
		{
			get
			{
				return _SourceExpression;
			}
			set
			{
				_SourceExpression = value;
			}
		}
	}
	public class ParametrizedArrayCreateExpression : ArrayCreateExpression
	{
		ExpressionCollection _NewOperatorArguments = null;
		protected ParametrizedArrayCreateExpression()
		{
		}
		public ParametrizedArrayCreateExpression(TypeReferenceExpression type, ExpressionCollection dimensions, ExpressionCollection newArguments)
			: base (type, dimensions)
		{
			_NewOperatorArguments = newArguments;
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ParametrizedArrayCreateExpression))
				return;
			ParametrizedArrayCreateExpression sourceElement = (ParametrizedArrayCreateExpression)source;
			if (sourceElement._NewOperatorArguments != null)
			{
				_NewOperatorArguments = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, sourceElement.DetailNodes, _NewOperatorArguments, sourceElement._NewOperatorArguments);
				if (_NewOperatorArguments.Count == 0 && sourceElement._NewOperatorArguments.Count > 0)
					_NewOperatorArguments = sourceElement._NewOperatorArguments.DeepClone(options) as ExpressionCollection;
			}
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_NewOperatorArguments != null && _NewOperatorArguments.Contains(oldElement as Expression))
				_NewOperatorArguments.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ParametrizedArrayCreateExpression expression = new ParametrizedArrayCreateExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ParametrizedArrayCreateExpression;
			}
		}
		public ExpressionCollection NewOperatorArguments
		{
			get
			{
				return _NewOperatorArguments;
			}
			set
			{
				_NewOperatorArguments = value;
			}
		}
	}	
	public class DeleteArrayExpression : DeleteExpression
	{
		protected	DeleteArrayExpression()
		{
		}
		public DeleteArrayExpression(Expression expression)
			: base(expression)
		{
			Name = "delete []";
		}
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			return null;
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			DeleteArrayExpression expression = new DeleteArrayExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.DeleteArrayExpression;
			}
		}
	}
	public class  ManagedObjectCreationExpression : ObjectCreationExpression
	{
		ExpressionCollection _Dimensions = null;
		ArrayInitializerExpression _ArrayInitializers = null;
		ArrayKindModifier _ArrayKind = ArrayKindModifier.None;
		protected ManagedObjectCreationExpression()
		{
		}
		public ManagedObjectCreationExpression(TypeReferenceExpression type)
			: base(type)
		{
			this.Name = "gcnew";
		}
		public ManagedObjectCreationExpression(TypeReferenceExpression type, ExpressionCollection ctorArguments)
			: base(type, ctorArguments)
		{
			this.Name = "gcnew";
		}
		public ManagedObjectCreationExpression(TypeReferenceExpression type, ExpressionCollection ctorArguments, ArrayInitializerExpression arrayinit)
			: this(type, ctorArguments)
		{
			_ArrayInitializers = arrayinit;
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ManagedObjectCreationExpression))
				return;
			ManagedObjectCreationExpression sourceElement = (ManagedObjectCreationExpression)source;
			_ArrayKind = sourceElement._ArrayKind;
			if (sourceElement._ArrayInitializers != null)
			{				
				_ArrayInitializers = ParserUtils.GetCloneFromNodes(this, sourceElement, sourceElement._ArrayInitializers) as ArrayInitializerExpression;
				if (_ArrayInitializers == null)
					_ArrayInitializers = sourceElement._ArrayInitializers.Clone(options) as ArrayInitializerExpression;
			}
			if (sourceElement._Dimensions != null)
			{
				_Dimensions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, sourceElement.DetailNodes, _Dimensions, sourceElement._Dimensions);
				if (_Dimensions.Count == 0 && sourceElement._Dimensions.Count > 0)
					_Dimensions = sourceElement._Dimensions.DeepClone(options) as ExpressionCollection;
			}
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_ArrayInitializers == oldElement)
				_ArrayInitializers = newElement as ArrayInitializerExpression;
			else if (_Dimensions != null && _Dimensions.Contains(oldElement as Expression))
				_Dimensions.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ManagedObjectCreationExpression expression = new ManagedObjectCreationExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ManagedObjectCreationExpression;
			}
		}
		public ArrayInitializerExpression ArrayInitializers
		{
			get
			{
				return _ArrayInitializers;
			}
			set
			{
				_ArrayInitializers = value;
			}
		}
		public ExpressionCollection Dimensions
		{
			get
			{
				return _Dimensions;
			}
			set
			{
				_Dimensions = value;
			}
		}
		public ArrayKindModifier ArrayKind
		{
			get
			{
				return _ArrayKind;
			}
			set
			{
				_ArrayKind = value;
			}
		}
	}	
	public class CppQualifiedElementReference : QualifiedElementReference
	{
		protected CppQualifiedElementReference()
		{
		}
		public CppQualifiedElementReference(Expression source, string name, SourceRange namerange)
			: base(source, name, namerange)
		{
		}
		protected virtual string GetNameWithQualifier()
		{
			return String.Format("{0}::", Qualifier.ToString());
		}
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (Qualifier != null)
			{
				if (Qualifier is QualifiedAliasExpression)
					builder.Append(Qualifier.ToString());
				else
					builder.AppendFormat(GetNameWithQualifier());
				if (Name != null && Name.Length > 0)
					builder.Append(Name);
			}
			else if (Name != null && Name.Length > 0)
				builder.Append(Name);
			if (IsGeneric)
			{
				int count = TypeArguments.Count;
				builder.Append("<");
				for (int i = 0; i < count; i++)
				{
					builder.Append(TypeArguments[i].ToString());
					if (i < (count - 1))
						builder.Append(", ");
				}
				builder.Append(">");
			}
			return builder.ToString();
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			CppQualifiedElementReference expression = new CppQualifiedElementReference();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.CppQualifiedElementReference;
			}
		}
	}
	public class QualifiedNestedReference : CppQualifiedElementReference
	{
		protected QualifiedNestedReference()
		{
		}
		public QualifiedNestedReference(Expression source, string name, SourceRange namerange)
			: base(source, name, namerange)
		{
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			QualifiedNestedReference expression = new QualifiedNestedReference();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.QualifiedNestedReference;
			}
		}
	}
	public class QualifiedNestedTypeReference : TypeReferenceExpression
	{		
		protected QualifiedNestedTypeReference()
		{
		}
		public QualifiedNestedTypeReference(TypeReferenceExpression source, string name, SourceRange namerange)
			: base(name, namerange, source)
		{
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			QualifiedNestedTypeReference expression = new QualifiedNestedTypeReference();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override string ToString()
		{
			StringBuilder builder1 = new StringBuilder();
			if (this.Qualifier != null)
			{
				if (this.Qualifier is QualifiedAliasExpression)
				{
					builder1.AppendFormat("{0}", this.Qualifier.ToString());
				}
				else
				{
					builder1.AppendFormat("{0}::", this.Qualifier.ToString());
				}
				if ((this.Name != null) && (this.Name.Length > 0))
				{
					builder1.Append(this.Name);
				}
			}
			else if ((this.Name != null) && (this.Name.Length > 0))
			{
				builder1.AppendFormat(this.Name, new object[0]);
			}
			return builder1.ToString();
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.QualifiedNestedTypeReference;
			}
		}
	}
	public class PointerElementReference : CppQualifiedElementReference
	{
		protected PointerElementReference()
		{
		}
		public PointerElementReference(Expression source, string name, SourceRange namerange)
			: base(source, name, namerange)
		{			
		}
		protected override string GetNameWithQualifier()
		{
			return String.Format("{0}->", Qualifier.ToString());
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			PointerElementReference expression = new PointerElementReference();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.PointerElementReference;
			}
		}
	}
	public abstract class CppMethodReferenceExpression : MethodReferenceExpression
	{
		protected CppMethodReferenceExpression()
		{
		}
		public CppMethodReferenceExpression(Expression source, string name, SourceRange namerange)
			: base(source, name, namerange)
		{
		}
		protected abstract string GetNameWithQualifier();
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (Qualifier != null)
			{
				if (!(Qualifier is QualifiedAliasExpression))
					builder.Append(Qualifier.ToString());
				if (Name != null && Name.Length > 0)
					builder.Append(GetNameWithQualifier());
			}
			else if (Name != null && Name.Length > 0)
				builder.Append(Name);
			if (IsGeneric)
			{
				int count = this.TypeArguments.Count;
				builder.Append("<");
				for (int i = 0; i < count; i++)
				{
					builder.Append(TypeArguments[i].ToString());
					if (i < (count - 1))
						builder.Append(", ");
				}
				builder.Append(">");
			}
			return builder.ToString();
		}
	}
	public class PointerMethodReference : CppMethodReferenceExpression
	{
		protected PointerMethodReference()
		{
		}
		public PointerMethodReference(Expression source, string name, SourceRange namerange)
			: base(source, name, namerange)
		{
		}
		protected override string GetNameWithQualifier()
		{
			return String.Format("->{0}", Name);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			PointerMethodReference expression = new PointerMethodReference();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.PointerMethodReference;
			}
		}
	}
	public class QualifiedMethodReference : CppMethodReferenceExpression
	{
		protected QualifiedMethodReference()
		{
		}
		public QualifiedMethodReference(Expression source, string name, SourceRange namerange)
			: base(source,name, namerange)
		{				
		}
		protected override string GetNameWithQualifier()
		{
			return String.Format("::{0}", Name);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			QualifiedMethodReference clone = new QualifiedMethodReference();				
			clone.CloneDataFrom(this, options);
			return clone;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.QualifiedMethodReference;
			}
		}
	}
	public abstract class CppTypeCastExpression : TypeCastExpression
	{
		protected abstract string GetString();
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (TypeReference != null)
				builder.AppendFormat(GetString());
			if (Target != null)
				builder.AppendFormat("( {0} )" , Target.ToString());
			return builder.ToString();
		}
	}
	public class StaticCastExpression : CppTypeCastExpression
	{
		public StaticCastExpression()
		{
		}
		protected override string GetString()
		{
			return String.Format("static_cast < {0} > ", TypeReference.ToString());
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			StaticCastExpression expression = new StaticCastExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.StaticCastExpression;
			}
		}
	}
	public class SafeCastExpression : CppTypeCastExpression
	{
		public SafeCastExpression()
		{
		}
		protected override string GetString()
		{
			return String.Format("safe_cast < {0} > ", TypeReference.ToString());
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			SafeCastExpression expression = new SafeCastExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.SafeCastExpression;
			}
		}
	}
	public class DynamicCastExpression : CppTypeCastExpression
	{
		public DynamicCastExpression()
		{
		}
		protected override string GetString()
		{
			return String.Format("dynamic_cast < {0} > ", TypeReference.ToString());
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			DynamicCastExpression expression = new DynamicCastExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.DynamicCastExpression;
			}
		}
	}
	public class TryCastExpression : CppTypeCastExpression
	{
		public TryCastExpression()
		{
		}
		protected override string GetString()
		{
			return String.Format("__try_cast < {0} > ", TypeReference.ToString());
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TryCastExpression expression = new TryCastExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.TryCastExpression;
			}
		}
	}
	public class ReinterpretCastExpression : CppTypeCastExpression
	{
		public ReinterpretCastExpression()
		{
		}
		protected override string GetString()
		{
			return String.Format("reinterpret_cast < {0} > ", TypeReference.ToString());
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ReinterpretCastExpression expression = new ReinterpretCastExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ReinterpretCastExpression;
			}
		}
	}
	public class ConstCastExpression : CppTypeCastExpression
	{
		public ConstCastExpression()
		{
		}
		protected override string GetString()
		{
			return String.Format("const_cast < {0} > ", TypeReference.ToString());
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ConstCastExpression expression = new ConstCastExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ConstCastExpression;
			}
		}
	}
	public class TemplateExpression : ElementReferenceExpression
	{
		Expression _Qualifier = null;
		public TemplateExpression()
		{
		}
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("template");
			if (Qualifier != null)
				builder.Append(Qualifier.ToString());
			return builder.ToString();
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is DeleteExpression))
				return;
			TemplateExpression sourceElement = (TemplateExpression)source;
			if (sourceElement._Qualifier != null)
			{				
				_Qualifier = ParserUtils.GetCloneFromNodes(this, sourceElement, sourceElement._Qualifier) as Expression;
				if (_Qualifier == null)
					_Qualifier = sourceElement._Qualifier.Clone(options) as Expression;
			}			
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Qualifier == oldElement)
				_Qualifier = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TemplateExpression expression = new TemplateExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.TemplateExpression;
			}
		}
		public override Expression Qualifier
		{
			get
			{
				return _Qualifier;
			}
			set
			{
				_Qualifier = value;
			}
		}
	}
	#endregion
	#region Statements
	#region AssemblerStatement
	public enum AssemblerStatementType
	{
		OldSyntaxAsmSingle, 
		OldSyntaxAsmDouble, 
		NewSyntaxAsm		
	}
	public class AssemblerStatement : Statement
	{
		#region Fields...
		AssemblerStatementType _StatementType;
		String _AssemblerCode;
		#endregion
		#region Constructors...
		public AssemblerStatement() : base()
		{
			_AssemblerCode = String.Empty;
		}
		public AssemblerStatement(String assemblerCode) : this()
		{
			if (assemblerCode != null)
				_AssemblerCode = assemblerCode;
		}
		public AssemblerStatement(String assemblerCode, AssemblerStatementType statementType):this(assemblerCode)
		{
			_StatementType = statementType;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is AssemblerStatement))
				return;
			AssemblerStatement sourceElement = (AssemblerStatement)source;
			_AssemblerCode = sourceElement.AssemblerCode;
			_StatementType = sourceElement.StatementType;
		}
		#endregion
		#region Properties...
		public string AssemblerCode
		{
			get
			{
				return _AssemblerCode;
			}
			set
			{
				_AssemblerCode = value;
			}
		}
		public AssemblerStatementType StatementType
		{
			get
			{
				return _StatementType;
			}
			set
			{
				_StatementType = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AssemblerStatement;
			}
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			AssemblerStatement statement = new AssemblerStatement();
			statement.CloneDataFrom(this, options);
			return statement;
		}
		#endregion
	}
	#endregion
	#region Except
	public class Except : ParentingStatement
	{
		#region Fields...
		Expression _ExceptionExpression;
		#endregion
		#region Constructors...
		public Except() : base()
		{
		}
		public Except(Expression exceptionExpression) : this()
		{
			_ExceptionExpression = exceptionExpression;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Except))
				return;
			Except sourceElement = (Except)source;
			ExceptionExpression = sourceElement.ExceptionExpression;
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_ExceptionExpression != null && _ExceptionExpression == oldElement as Expression)
				_ExceptionExpression = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region Properties...
		public Expression ExceptionExpression
		{
			get
			{
				return _ExceptionExpression;
			}
			set
			{
				_ExceptionExpression = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.Except;
			}
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Except statement = new Except();
			statement.CloneDataFrom(this, options);
			return statement;
		}
		#endregion
	}
	#endregion
	#endregion
	#region Type params
	public abstract class CppTypeParameter : TypeParameter
	{
		SourceRange _NameRange = SourceRange.Empty;
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
	  base.CloneDataFrom(source, options);
			if (!(source is CppTypeParameter))
				return;
			CppTypeParameter sourceElement = (CppTypeParameter)source;
			_NameRange = sourceElement.NameRange;
		}
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
			set
			{
		ClearHistory();
				_NameRange = value;
			}
		}
	}
	public class ClassTypeParameter : CppTypeParameter
	{
		public ClassTypeParameter()
		{
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ClassTypeParameter expression = new ClassTypeParameter();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ClassTypeParameter;
			}
		}
	}
	public class TypenameTypeParameter : CppTypeParameter
	{
		public TypenameTypeParameter()
		{
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypenameTypeParameter expression = new TypenameTypeParameter();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.TypenameTypeParameter;
			}
		}
	}
	public class ValueClassTypeParameterConstraint : TypeParameterConstraint
	{
		protected ValueClassTypeParameterConstraint()
		{
		}
		public ValueClassTypeParameterConstraint(string name, SourceRange range) :base(name,range)
		{
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ValueClassTypeParameterConstraint expression = new ValueClassTypeParameterConstraint();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ValueClassTypeParameterConstraint;
			}
		}
	}
	public class ValueStructTypeParameterConstraint : TypeParameterConstraint
	{
		protected ValueStructTypeParameterConstraint()
		{
		}
		public ValueStructTypeParameterConstraint(string name, SourceRange range) :base(name,range)
		{
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ValueStructTypeParameterConstraint expression = new ValueStructTypeParameterConstraint();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ValueStructTypeParameterConstraint;
			}
		}
	}
	#endregion
	#region templates...
	public class TemplateModifier : CodeElement
	{
		bool _IsExport = false;
		LanguageElementCollection _Parameters = null;
		public TemplateModifier()
		{
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is TemplateModifier))
				return;
			TemplateModifier sourceElement = (TemplateModifier)source;
			_IsExport = sourceElement._IsExport;
			if (sourceElement._Parameters != null)
			{				
				_Parameters = ParserUtils.GetCloneFromNodes(this, sourceElement, sourceElement._Parameters) as LanguageElementCollection;
				if (_Parameters == null)
					_Parameters = sourceElement._Parameters.DeepClone(options) as LanguageElementCollection;
			}
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Parameters != null && _Parameters.Contains(oldElement as Expression))
				_Parameters.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TemplateModifier expression = new TemplateModifier();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.TemplateModifier;
			}
		}		
		public bool IsExport
		{
			get
			{
				return _IsExport;
			}
			set
			{
				_IsExport = value;
			}
		}
		public LanguageElementCollection Parameters
		{
			get
			{
				return _Parameters;
			}
			set
			{
				_Parameters = value;
			}
		}
		public int ParametersCount
		{
			get
			{
				if (_Parameters != null)
					return _Parameters.Count;
				else
					return 0;
			}
		}
		public override string ToString()
		{
			StringBuilder lStrBuilder = new StringBuilder();
			if (this.IsExport)
				lStrBuilder.Append("export ");
			lStrBuilder.Append("template ");
			if (this._Parameters != null)
				lStrBuilder.AppendFormat("<{0}>",_Parameters.ToString());
			return lStrBuilder.ToString();
		}
	}
	public class TemplateParameter : CodeElement, ITemplateParameter
	{
		Expression _DefaultValue = null;
		public TemplateParameter()
			: this(String.Empty, null)
		{
		}
		public TemplateParameter(string name, Expression defaultValue)
		{
			InternalName = name;
			_DefaultValue = defaultValue;
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is TemplateParameter))
				return;
			TemplateParameter sourceElement = (TemplateParameter)source;
			if (sourceElement._DefaultValue != null)
			{				
				_DefaultValue = ParserUtils.GetCloneFromNodes(this, sourceElement, sourceElement._DefaultValue) as Expression;
				if (_DefaultValue == null)
					_DefaultValue = sourceElement._DefaultValue.Clone(options) as Expression;
			}			
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_DefaultValue == oldElement)
				_DefaultValue = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TemplateParameter expression = new TemplateParameter();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.Unknown;
			}
		}		
		public Expression DefaultValue
		{
			get
			{
				return _DefaultValue;
			}
			set
			{
				_DefaultValue = value;
			}
		}
		IExpression ITemplateParameter.DefaultValue
		{
			get
			{
				return _DefaultValue;
			}
		}
		ITypeReferenceExpression ITypeParameter.ActivatedType
		{
			get
			{
				return null;
			}
		}
		ITypeParameterConstraintCollection ITypeParameter.Constraints
		{
			get
			{
				return EmptyLiteElements.EmptyITypeParameterConstraintCollection;
			}
		}
		bool ITypeParameter.IsActivated
		{
			get
			{
				return false;
			}
		}
		TypeParameterDirection ITypeParameter.Direction
		{
			get { return TypeParameterDirection.None; }
		}
	}
	public class NamedTemplateParameter : TemplateParameter, INamedTemplateParameter
	{
		protected NamedTemplateParameter()
		{		
		}
		public NamedTemplateParameter(string name, Expression defaultValue)
			: base(name, defaultValue)
		{		
		}
		public override string ToString()
		{
			StringBuilder lStrBuilder = new StringBuilder();
			lStrBuilder.Append("typename ");
			if (this.Name != String.Empty)
				lStrBuilder.Append(this.Name);
			if (this.DefaultValue != null)
				lStrBuilder.AppendFormat(" = {0}",this.DefaultValue.ToString());
			return lStrBuilder.ToString();
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			NamedTemplateParameter expression = new NamedTemplateParameter();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.NamedTemplateParameter;
			}
		}
		TypeParameterDirection ITypeParameter.Direction
		{
			get { return TypeParameterDirection.None; }
		}
	}
	public class ClassTemplateParameter : TemplateParameter, IClassTemplateParameter
	{
		protected ClassTemplateParameter()
		{		
		}
		public ClassTemplateParameter(string name, Expression defaultValue)
			: base(name, defaultValue)
		{		
		}
		public override string ToString()
		{
			StringBuilder lStrBuilder = new StringBuilder();
			lStrBuilder.Append("class ");
			if (this.Name != String.Empty)
				lStrBuilder.Append(this.Name);
			if (this.DefaultValue != null)
				lStrBuilder.AppendFormat(" = {0}",this.DefaultValue.ToString());
			return lStrBuilder.ToString();
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ClassTemplateParameter expression = new ClassTemplateParameter();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ClassTemplateParameter;
			}
		}
		TypeParameterDirection ITypeParameter.Direction
		{
			get { return TypeParameterDirection.None; }
		}
	}
	public class NestedTemplateParameter : TemplateParameter, INestedTemplateParameter
	{
		LanguageElementCollection _Parameters = null;
		protected NestedTemplateParameter()
		{
		}
		public NestedTemplateParameter(string name, Expression defaultValue, LanguageElementCollection parameters)
			: base(name, defaultValue)
		{
			_Parameters = parameters;
		}				
		public override string ToString()
		{
			StringBuilder lStrBuilder = new StringBuilder();
			lStrBuilder.Append("template ");
			if (this._Parameters != null)
				lStrBuilder.AppendFormat("<{0}> ",_Parameters.ToString());
			lStrBuilder.Append("class ");
			if (this.Name != String.Empty)
				lStrBuilder.Append(this.Name);
			if (this.DefaultValue != null)
				lStrBuilder.AppendFormat(" = {0}",this.DefaultValue.ToString());
			return lStrBuilder.ToString();
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is TemplateModifier))
				return;
			NestedTemplateParameter sourceElement = (NestedTemplateParameter)source;
			if (sourceElement._Parameters != null)
			{				
				_Parameters = ParserUtils.GetCloneFromNodes(this, sourceElement, sourceElement._Parameters) as LanguageElementCollection;
				if (_Parameters == null)
					_Parameters = sourceElement._Parameters.DeepClone(options) as LanguageElementCollection;
			}
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Parameters != null && _Parameters.Contains(oldElement as Expression))
				_Parameters.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			NestedTemplateParameter expression = new NestedTemplateParameter();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.NestedTemplateParameter;
			}
		}
		public LanguageElementCollection Parameters
		{
			get
			{
				return _Parameters;
			}
			set
			{
				_Parameters = null;
			}
		}
		public int ParametersCount
		{
			get
			{
				if (_Parameters != null)
					return _Parameters.Count;
				else
					return 0;
			}
		}
		IElementCollection INestedTemplateParameter.Parameters
		{
			get
			{
				if (_Parameters == null)
					return EmptyLiteElements.EmptyIElementCollection;
				IElementCollection parameters = new IElementCollection();
				parameters.AddRange(_Parameters);
				return parameters;
			}
		}
	}
	#endregion
	#region other declarations
	public class UsingDeclaration : NamespaceReference
	{
		Expression _Declaration;
		protected UsingDeclaration()
		{
		}
		public UsingDeclaration(Expression declaration)
			: base()
		{
			_Declaration = declaration;
			if (_Declaration != null)
				InternalName = _Declaration.ToString();
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is UsingDeclaration))
				return;
			UsingDeclaration sourceElement = (UsingDeclaration)source;
			if (sourceElement._Declaration != null)
			{				
				_Declaration = ParserUtils.GetCloneFromNodes(this, sourceElement, sourceElement._Declaration) as Expression;
				if (_Declaration == null)
					_Declaration = sourceElement._Declaration.Clone(options) as Expression;
			}			
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Declaration == oldElement)
				_Declaration = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			UsingDeclaration expression = new UsingDeclaration();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.UsingDeclaration;
			}
		}
		public Expression Declaration
		{
			get
			{
				return _Declaration;
			}
			set
			{
				_Declaration = value;
			}
		}
	}
	public class AccessDeclaration : CodeElement
	{
		Expression _Declaration  = null;
		MemberVisibility _Visibility = MemberVisibility.Illegal;
		protected AccessDeclaration()
		{
		}
		public AccessDeclaration(Expression declaration)
		{
			if (declaration != null)
			{
				_Declaration = declaration;
				Name = declaration.ToString();
			}
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is AccessDeclaration))
				return;
			AccessDeclaration sourceElement = (AccessDeclaration)source;
			_Visibility = sourceElement.Visibility;
			if (sourceElement._Declaration != null)
			{				
				_Declaration = ParserUtils.GetCloneFromNodes(this, sourceElement, sourceElement._Declaration) as Expression;
				if (_Declaration == null)
					_Declaration = sourceElement._Declaration.Clone(options) as Expression;
			}	
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Declaration == oldElement)
				_Declaration = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			AccessDeclaration expression = new AccessDeclaration();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AccessDeclaration;
			}
		}
		public Expression Declaration
		{
			get
			{
				return _Declaration;
			}
			set 
			{
				_Declaration = value;
			}
		}
		public MemberVisibility Visibility
		{
			get
			{
				return _Visibility;
			}
			set
			{
				_Visibility = value;
			}
		}
	}
	public class CppAttributeSection : AttributeSection
	{
		AttributeTargetType _TargetType = AttributeTargetType.None;
		public CppAttributeSection()
		{
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is CppAttributeSection))
				return;
			CppAttributeSection sourceElement = (CppAttributeSection)source;
			_TargetType = sourceElement._TargetType;
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			CppAttributeSection clone = new CppAttributeSection();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		public AttributeTargetType TargetType 
		{
			get
			{
				return _TargetType;
			}
			set
			{
				_TargetType = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.CppAttributeSection;
			}
		}
	}
	public class ComplexConstructorInitializer : ConstructorInitializer
	{
		public ComplexConstructorInitializer()
		{		
		}		
		public override string ToString()
		{
			StringBuilder lBuilder = new StringBuilder();			
			if(this.DetailNodes != null && this.DetailNodes.Count !=0)
			{
				bool IsFirstBoot = true;
				for(int i=0; i<this.DetailNodes.Count; i++)
				{
					if(IsFirstBoot)
						IsFirstBoot = false;
					else
						lBuilder.Append(",");
					lBuilder.AppendFormat("{0}",this.DetailNodes[i].ToString());
				}
			}
			return lBuilder.ToString();
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ComplexConstructorInitializer clone = new ComplexConstructorInitializer();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ComplexConstructorInitializer;
			}
		}
	}
	public class ExternDeclaration : DelimiterCapableBlock, IExternDeclaration
	{
		bool _HasBrackets = false;
		public ExternDeclaration()
		{
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ExternDeclaration))
				return;
			ExternDeclaration sourceElement = (ExternDeclaration)source;
			_HasBrackets = sourceElement._HasBrackets;
		}
		public override string ToString()
		{
			return "extern";
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ExternDeclaration clone = new ExternDeclaration();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return  LanguageElementType.ExternDeclaration;
			}
		}
		public bool HasBrackets 
		{
			get
			{
				return _HasBrackets;
			}
			set
			{
				_HasBrackets = true;
			}
		}
	}
	public enum MemberVisibilityType
	{
		Public,
		Private,
		Protected		
	}
	public class MemberVisibilitySpecifier : CodeElement
	{
		MemberVisibility _MemberVisibility;
		public MemberVisibilitySpecifier()
		{
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is MemberVisibilitySpecifier))
				return;
			MemberVisibilitySpecifier sourceElement = (MemberVisibilitySpecifier)source;
			_MemberVisibility = sourceElement._MemberVisibility;
		}
		public override string ToString()
		{
			return base.ToString();
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			MemberVisibilitySpecifier clone = new MemberVisibilitySpecifier();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.MemberVisibilitySpecifier;
			}
		}
		public MemberVisibility MemberVisibility
		{
			get
			{
				return _MemberVisibility;
			}
			set
			{
				_MemberVisibility = value;
			}
		}
	}
	#endregion
}
