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
using System.ComponentModel;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ObjectCreationExpression : Expression, IObjectCreationExpression, IHasParens, IHasArguments, IWithArgumentsModifier
	{
		const int INT_MaintainanceComplexity = 5;
		#region private fields...
		TypeReferenceExpression _ObjectType;
		ExpressionCollection _Arguments;
		bool _HasNOGCModifier = false;
		TextRangeWrapper _ParensRange;
		ObjectInitializerExpression _ObjectInitializer;
		#endregion
		#region ObjectCreationExpression
		protected ObjectCreationExpression()
		{			
		}
		#endregion
		#region ObjectCreationExpression
		public ObjectCreationExpression(TypeReferenceExpression type)
		{
	  SetObjectType(type);
			if (type != null)
				InternalName = type.Name;
		}
		#endregion
		#region ObjectCreationExpression
		public ObjectCreationExpression(TypeReferenceExpression type, ExpressionCollection arguments)
			: this(type)
		{
	  SetArguments(arguments);
		}
		#endregion
	void SetObjectType(TypeReferenceExpression type)
	{
	  if (_ObjectType != null)
		RemoveNode(_ObjectType);
	  _ObjectType = type;
	  if (_ObjectType != null)
		AddNode(_ObjectType);
	}
	void SetObjectInitializer(ObjectInitializerExpression initializer)
	{
	  if (_ObjectInitializer != null)
		RemoveDetailNode(_ObjectInitializer);
	  _ObjectInitializer = initializer;
	  if (_ObjectInitializer != null)
		AddDetailNode(_ObjectInitializer);
	}
	void SetArguments(ExpressionCollection arguments)
	{
	  if (_Arguments != null)
		RemoveDetailNodes(_Arguments);
	  _Arguments = arguments;
	  if (_Arguments != null)
		AddDetailNodes(_Arguments);
	}
		#region ToString
		public override string ToString()
		{
			string lResult = "new ";
			if (_ObjectType != null)
				lResult += _ObjectType.ToString();
			lResult += "(";
			if (_Arguments != null)
				lResult += _Arguments.ToString();
			lResult += ")";
			return lResult;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ObjectCreation;
		}
		#endregion
		#region ComareArguments
		private bool ComareArguments(ObjectCreationExpression expression)
		{
			if (expression == null)
				return false;
			if (Arguments == null && expression.Arguments == null)
				return true;
			if (Arguments == null)
				return false;
			return Arguments.IsIdenticalTo(expression.Arguments);
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is ObjectCreationExpression)
			{
				ObjectCreationExpression lExpression = expression as ObjectCreationExpression;
				if (ObjectType == null)
					return false;
				if (!ObjectType.IsIdenticalTo(lExpression.ObjectType))
					return false;
				if (!ComareArguments(lExpression))
					return false;
				if (ObjectInitializer == null && lExpression.ObjectInitializer == null)
				return true;
				return ObjectInitializer.IsIdenticalTo(lExpression.ObjectInitializer);
			}
			return false;
		}
		#endregion
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParensRange(Token parenOpen, Token parenClose)
		{
			SetParensRange(new SourceRange(parenOpen.Range.Start, parenClose.Range.End));			
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParensRange(SourceRange range)
		{
	  ClearHistory();
			if (range.IsEmpty)
				_ParensRange = null;
			else
				_ParensRange = range;
		}
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _ParensRange = ParensRange;
	}
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_ObjectType = null;
			_ObjectInitializer = null;
			if (_Arguments != null)
				_Arguments.Clear();
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_ObjectType == oldElement)
				_ObjectType = (TypeReferenceExpression)newElement;
			else if (_Arguments != null)
				_Arguments.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else if (_ObjectInitializer == oldElement)
				_ObjectInitializer = newElement as ObjectInitializerExpression;
			else	
				base.ReplaceOwnedReference(oldElement, newElement);
		}	
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ObjectCreationExpression))
				return;
			ObjectCreationExpression lSource = (ObjectCreationExpression)source;
			_HasNOGCModifier = lSource._HasNOGCModifier;
	  if (lSource._ParensRange != null)
		_ParensRange = lSource.ParensRange;
			if (lSource._ObjectType != null)
			{				
				_ObjectType = ParserUtils.GetCloneFromNodes(this, lSource, lSource._ObjectType) as TypeReferenceExpression;
				if (_ObjectType == null)
					_ObjectType = lSource._ObjectType.Clone(options) as TypeReferenceExpression;
			}
			if (lSource._Arguments != null)
			{
				_Arguments = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Arguments, lSource._Arguments);
				if (_Arguments.Count == 0 && lSource._Arguments.Count > 0)
					_Arguments = lSource._Arguments.DeepClone(options) as ExpressionCollection;
			}
			if (lSource._ObjectInitializer != null)
			{				
				_ObjectInitializer = ParserUtils.GetCloneFromNodes(this, lSource, lSource._ObjectInitializer) as ObjectInitializerExpression;
				if (_ObjectInitializer == null)
					_ObjectInitializer = lSource._ObjectInitializer.Clone(options) as ObjectInitializerExpression;
			}
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ObjectCreationExpression clone = new ObjectCreationExpression();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		#endregion
	public void AddArgument(Expression arg)
	{
	  if (arg == null)
		return;
	  Arguments.Add(arg);
	  AddDetailNode(arg);
	}
	public void AddArguments(IEnumerable<Expression> arguments)
	{
	  if (arguments == null)
		return;
	  foreach (Expression expr in arguments)
		AddArgument(expr);
	}
	public void RemoveArgument(Expression arg)
	{
	  if (arg == null)
		return;
	  Arguments.Remove(arg);
	  RemoveDetailNode(arg);
	}
	public void InsertArgument(int index, Expression arg)
	{
	  if (arg == null)
		return;
	  Arguments.Insert(index, arg);
	  InsertDetailNode(index, arg);
	}
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ObjectCreationExpression;
			}
		}
		#region ObjectType
		public TypeReferenceExpression ObjectType
		{
			get
			{
				return _ObjectType;
			}
			set
			{
				SetObjectType(value);
			}
		}
		#endregion
		#region Arguments
		public ExpressionCollection Arguments
		{
			get
			{
				if (_Arguments == null)
					_Arguments = new ExpressionCollection();
				return _Arguments;
			}
			set
			{
		SetArguments(value);
			}
		}
		#endregion
		public override SourceRange NameRange
		{
			get
			{
				return _ObjectType == null ? SourceRange.Empty : _ObjectType.InternalRange;
			}
		}
		public ObjectInitializerExpression ObjectInitializer
		{
			get
			{
				return _ObjectInitializer;
			}
			set
			{
		SetObjectInitializer(value);
			}
		}
		#region CanBeStatement
		public override bool CanBeStatement
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region HasNOGCModifier
		public bool HasNOGCModifier
		{
			get
			{
				return _HasNOGCModifier;
			}
			set
			{
				_HasNOGCModifier = value;
			}
		}
		#endregion
		#region IObjectCreationExpression Members
		ITypeReferenceExpression IObjectCreationExpression.ObjectType
		{
			get
			{
				return ObjectType;
			}
		}
		IObjectInitializerExpression IObjectCreationExpression.ObjectInitializer
		{
			get
			{
				return ObjectInitializer;
			}
		}
		IExpressionCollection IObjectCreationExpression.Arguments
		{
			get
			{
				if (Arguments == null)
					return EmptyLiteElements.EmptyIExpressionCollection;
				return Arguments;
			}
		}
		#endregion
		#region IHasParens Members
		public SourceRange ParensRange
		{
			get
			{
				if (_ParensRange == null)
					return SourceRange.Empty;
				return _ParensRange;
			}
		}
		#endregion
	#region IHasArguments Members
	public int ArgumentsCount
	{
	  get
	  {
		if (_Arguments == null)
		  return 0;
		return _Arguments.Count;
	  }
	}
	#endregion
	#region IWithArguments Members
	IExpressionCollection IWithArguments.Args
	{
	  get { return _Arguments; }
	}
	#endregion
	#region IWithArgumentsModifier Members
	void IWithArgumentsModifier.AddArgument(IExpression argument)
	{
	  AddArgument(argument as Expression);
	}
	void IWithArgumentsModifier.InsertArgument(int index, IExpression argument)
	{
	  InsertArgument(index, argument as Expression);
	}
	void IWithArgumentsModifier.RemoveArgument(IExpression argument)
	{
	  RemoveArgument(argument as Expression);
	}
	#endregion
  }
}
