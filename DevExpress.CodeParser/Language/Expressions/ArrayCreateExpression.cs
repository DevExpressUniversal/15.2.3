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
	public enum ArrayKindModifier
	{
		None,
		GC,
		NoGC
	};
	public class ArrayCreateExpression : Expression, IArrayCreateExpression
	{
		const int INT_MaintainanceComplexity = 6;
		#region private fields...
		TypeReferenceExpression _BaseType;
		ExpressionCollection _Dimensions;
		ArrayInitializerExpression _Initializer;
		bool _HasNOGCModifier = false;
		bool _IsStackAlloc = false;
		ArrayKindModifier _ArrayKind = ArrayKindModifier.None;
		#endregion
		#region ArrayCreateExpression
		public ArrayCreateExpression()
		{			
		}
		#endregion
		#region ArrayCreateExpression(TypeReferenceExpression type)
		public ArrayCreateExpression(TypeReferenceExpression type)
	  : this(type, null, null)
		{
		}
		#endregion
		#region ArrayCreateExpression(TypeReferenceExpression type, ExpressionCollection dimensions)
		public ArrayCreateExpression(TypeReferenceExpression type, ExpressionCollection dimensions)
			: this(type, dimensions, null)
		{
		}
		#endregion
		#region ArrayCreateExpression(TypeReferenceExpression type, ExpressionCollection dimensions, ArrayInitializerExpression initializer)
		public ArrayCreateExpression(TypeReferenceExpression type, ExpressionCollection dimensions, ArrayInitializerExpression initializer)
		{
	  SetBaseType(type);
	  if (dimensions != null)
		AddDimensions(dimensions);
	  if (initializer != null)
			  SetInitializer(initializer);
		}
		#endregion
		#region CompareDimensions
		private bool CompareDimensions(ArrayCreateExpression expression)
		{
			if (expression == null)
				return false;
			if (Dimensions == null && expression.Dimensions == null)
				return true;
			if (Dimensions == null || expression.Dimensions == null)
				return false;
			return Dimensions.IsIdenticalTo(expression.Dimensions);
		}
		#endregion
		#region CompareInitializer
		private bool CompareInitializer(ArrayCreateExpression expression)
		{
			if (expression == null)
				return false;
			if (Initializer == null && expression.Initializer == null)
				return true;
			if (Initializer == null || expression.Initializer == null)
				return false;
			return Initializer.IsIdenticalTo(expression.Initializer);
		}
		#endregion
	void SetBaseType(TypeReferenceExpression baseType)
	{
	  if (_BaseType != null)
		RemoveNode(_BaseType);
	  _BaseType = baseType;
	  if (_BaseType != null)
		AddNode(_BaseType);
	  if (_BaseType != null)
		InternalName = _BaseType.ToString();
	}
	void SetInitializer(ArrayInitializerExpression initializer)
	{
	  if (_Initializer != null)
		RemoveDetailNode(_Initializer);
	  _Initializer = initializer;
	  if (_Initializer != null)
		AddDetailNode(_Initializer);
	}
	void SetDimensions(ExpressionCollection dimensions)
	{
	  if (_Dimensions != null)
		RemoveDetailNodes(_Dimensions);
	  _Dimensions = dimensions;
	  if (_Dimensions != null)
		AddDetailNodes(_Dimensions);
	}
		#region CloneDataFrom
		protected  override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ArrayCreateExpression))
				return;
			ArrayCreateExpression lSource = (ArrayCreateExpression)source;
			_IsStackAlloc = lSource._IsStackAlloc;
			if (lSource.BaseType != null)
			{				
				_BaseType = ParserUtils.GetCloneFromNodes(this, lSource, lSource._BaseType) as TypeReferenceExpression;
				if (_BaseType == null)
					_BaseType = lSource._BaseType.Clone(options) as TypeReferenceExpression;
			}
			if (lSource._Dimensions != null)
			{
				_Dimensions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Dimensions, lSource._Dimensions);
				if (_Dimensions.Count == 0 && lSource._Dimensions.Count > 0)
					_Dimensions = lSource._Dimensions.DeepClone(options) as ExpressionCollection;
			}
			if (lSource._Initializer != null)
			{				
				_Initializer = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Initializer) as ArrayInitializerExpression;
				if (_Initializer == null)
					_Initializer = lSource._Initializer.Clone(options) as ArrayInitializerExpression;
			}
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string lResult = "new ";
			if (_BaseType != null)
				lResult += _BaseType.ToString();
			if (_Dimensions != null)
			{
				lResult += "[";
				lResult += _Dimensions.ToString();
				lResult += "]";
			}
			if (_Initializer != null)
				lResult += _Initializer.ToString();
			return lResult;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ArrayCreateExpression;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is ArrayCreateExpression)
			{
				ArrayCreateExpression lExpression = expression as ArrayCreateExpression;
				if (BaseType == null)
					return false;
				if (!BaseType.IsIdenticalTo((Expression)lExpression.BaseType))
					return false;
				if (!CompareDimensions(lExpression))
					return false;
				if (!CompareInitializer(lExpression))
					return false;
				return true;
			}
			return false;
		}
		#endregion
		#region Resolve
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_BaseType = null;
			_Initializer = null;
			if (_Dimensions != null)
				_Dimensions.Clear();
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_BaseType == oldElement)
				_BaseType = (TypeReferenceExpression)newElement;
			else if (_Initializer == oldElement)
				_Initializer = (ArrayInitializerExpression)newElement;
			else if (_Dimensions != null)
				_Dimensions.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ArrayCreateExpression lClone = new ArrayCreateExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	public void AddDimension(Expression dimension)
	{
	  Dimensions.Add(dimension);
	  AddDetailNode(dimension);
	}
	public void AddDimensions(IEnumerable<Expression> dimensions)
	{
	  if (dimensions == null)
		return;
	  foreach (Expression dimension in dimensions)
		AddDimension(dimension);
	}
		#region ThisMaintenanceComplexity
		protected override int ThisMaintenanceComplexity
		{
	  get { return INT_MaintainanceComplexity; }
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.ArrayCreateExpression; }
		}
		#endregion
		#region BaseType
		public TypeReferenceExpression BaseType
		{
	  get { return _BaseType; }
	  set { SetBaseType(value); }
		}
		#endregion
		#region Dimensions
		public ExpressionCollection Dimensions
		{
			get
			{
		if (_Dimensions == null)
		  _Dimensions = new ExpressionCollection();
				return _Dimensions;
			}
	  set { SetDimensions(value); }
		}
		#endregion
		#region Initializer
		public ArrayInitializerExpression Initializer
		{
	  get { return _Initializer; }
	  set { SetInitializer(value); }
		}
		#endregion
		#region NameRange
		public override SourceRange NameRange
		{
	  get { return _BaseType == null ? SourceRange.Empty : _BaseType.InternalRange; }
		}
		#endregion
		#region HasNOGCModifier
		public bool HasNOGCModifier
		{
	  get { return _HasNOGCModifier; }
	  set { _HasNOGCModifier = value; }
		}
		#endregion
		#region ArrayKind
		public ArrayKindModifier ArrayKind
		{
	  get { return _ArrayKind; }
	  set { _ArrayKind = value; }
		}
		#endregion
	#region IsStackAlloc
	public bool IsStackAlloc
	{
	  get { return _IsStackAlloc; }
	  set { _IsStackAlloc = value; }
	}
	#endregion
		#region IArrayCreateExpression Members
		ITypeReferenceExpression IArrayCreateExpression.BaseType
		{
	  get { return BaseType; }
		}
		IExpressionCollection IArrayCreateExpression.Dimensions
		{
			get
			{
				if (_Dimensions == null)
					return EmptyLiteElements.EmptyIExpressionCollection;
				return _Dimensions;
			}
		}
		IArrayInitializerExpression IArrayCreateExpression.Initializer
		{
	  get { return Initializer; }
		}
		#endregion
	}
}
