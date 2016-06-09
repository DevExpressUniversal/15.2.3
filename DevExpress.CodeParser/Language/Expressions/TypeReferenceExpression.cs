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
	public enum TypeReferenceType 
	{
		None,
		Array,
		Pointer,
		Reference,
	}
	public enum ArrayStorageClassQualifier
	{	
		None,
		Mutable,
		Auto,
		Register,
		Static,
		Const,
		Volatile,
		Extern
	}
	public enum GCReferenceType
	{
		None,
		GC,
		NoGC,
		Pin,
		Box
	}
	public class TypeReferenceExpression : ReferenceExpressionBase, ITypeReferenceExpression, ITypeReferenceExpressionModifier
	{
		private const int INT_MaintainanceComplexity = 1;
		bool _IsConst = false;
		bool _IsVolatile = false;
		bool _IsManaged = false;
		int _Rank;
		int _TypeArity;
		bool _IsUnbound;
		TextRange _NameRange;
		TypeReferenceType _Type = TypeReferenceType.None;
		Expression _BaseType;
		ExpressionCollection _ArrayBounds;
		bool _IsNullable = false;
		bool _IsDynamic = false;
		GCReferenceType _GCType = GCReferenceType.None;
		ArrayStorageClassQualifier _ArrayStorageClass = ArrayStorageClassQualifier.None;
		TypeReferenceExpression _BasedPointer;
		bool _IsTypeCharacter = false;
		#region TypeReferenceExpression
		public TypeReferenceExpression()
			: this(String.Empty)
		{			
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(string type)
		{
			InternalName = type;
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(string type, SourceRange range)
			: this(type)
		{
			SetRange(range);		
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(string type, TypeReferenceExpressionCollection typeArguments, SourceRange range)
			: this(type)
		{
			SetRange(range);
			SetTypeArguments(typeArguments);
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(TypeReferenceExpression type, int rank)
		{
			SetBaseType(type);
			SetRank(rank);			
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(TypeReferenceExpression type, int rank, ExpressionCollection bounds)
		{
			SetBaseType(type);
			SetRank(rank);
			SetArrayBounds(bounds);
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(TypeReferenceExpression type)
		{
			SetBaseType(type);
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(TypeReferenceExpression type, TypeReferenceType typeReference )
			:this(type)
		{			
			_Type = typeReference;
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(TypeReferenceType typeReference )		
		{			
			_Type = typeReference;
		}
		#endregion		
		#region TypeReferenceExpression
		public TypeReferenceExpression(int rank, ExpressionCollection bounds)
		{			
			SetRank(rank);
			SetArrayBounds(bounds);
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(TypeReferenceExpression type, bool isNullable)
		{
			SetBaseType(type);
			_Type = TypeReferenceType.None;
			_IsNullable = isNullable;
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(string name, SourceRange nameRange, TypeReferenceExpression type)
		{
			InternalName = name;
			_NameRange = nameRange;
			SetBaseType(type);
			_Type = TypeReferenceType.None;
		}
		#endregion
		#region TypeReferenceExpression
		public TypeReferenceExpression(string type, int typeArity, SourceRange range)
			: this (type)
		{
			SetRange(range);
			_TypeArity = typeArity;
		}
		#endregion
		protected void SetArrayBounds(ExpressionCollection bounds)
		{
			ReplaceDetailNodes(_ArrayBounds, bounds);
			_Type = TypeReferenceType.Array;
			_ArrayBounds = bounds;
		}		
		protected void SetBaseType(TypeReferenceExpression baseType)
		{
			ReplaceDetailNode(BaseType, baseType);
			_BaseType = baseType;
			_Type = TypeReferenceType.Pointer;
		}
		protected void SetRank(int rank)
		{
			_Rank = rank;
			_Type = TypeReferenceType.Array;
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
	  if (_BaseType == oldElement)
		_BaseType = (TypeReferenceExpression)newElement;
	  else if (ArrayBounds.Contains(oldElement as Expression))
		ArrayBounds.ReplaceExpression(oldElement as Expression, newElement as Expression);
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is TypeReferenceExpression))
				return;			
			TypeReferenceExpression lSource = (TypeReferenceExpression)source;
			_Rank = lSource.Rank;
			_TypeArity = lSource.TypeArity;
			_NameRange = lSource.NameRange;
			_IsNullable = lSource._IsNullable;
			_Type = lSource._Type;
			_GCType = lSource.GCType;
			_ArrayStorageClass = lSource.ArrayStorageClass;
			_IsConst = lSource.IsConst;
			_IsVolatile = lSource.IsVolatile;
			_IsManaged = lSource.IsManaged;
			_IsTypeCharacter = lSource._IsTypeCharacter;
			_IsUnbound = lSource._IsUnbound;
	  _IsDynamic = lSource._IsDynamic;
			if (lSource.BaseType != null)
			{
				_BaseType = ParserUtils.GetCloneFromNodes(this, lSource, lSource.BaseType) as TypeReferenceExpression;
				if (_BaseType == null)
					_BaseType = lSource.BaseType.Clone(options) as TypeReferenceExpression;
			} else
				if (lSource.Qualifier != null)
			{
				_BaseType = ParserUtils.GetCloneFromNodes(this, lSource, lSource.Qualifier) as Expression;
				if (_BaseType == null)
					_BaseType = lSource.Qualifier.Clone(options) as Expression;
			}
			if (lSource.BasedPointer != null)
			{
				_BasedPointer = ParserUtils.GetCloneFromNodes(this, lSource, lSource.BasedPointer) as TypeReferenceExpression;
				if (_BasedPointer == null)
					_BasedPointer = lSource.BasedPointer.Clone(options) as TypeReferenceExpression;
			}
			if (lSource._ArrayBounds != null)
			{
				_ArrayBounds = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _ArrayBounds, lSource._ArrayBounds);
				if (_ArrayBounds.Count == 0 && lSource._ArrayBounds.Count > 0)
					_ArrayBounds = lSource._ArrayBounds.DeepClone(options) as ExpressionCollection;
			}
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetTypeProperties(ITypeReferenceExpression type)
		{
			if (type == null)
				return;
			this.TypeReferenceType = type.TypeReferenceType;
			this.IsConst = type.IsConst;
			this.IsVolatile = type.IsVolatile;
			this.IsManaged = type.IsManaged;
			this.IsUnbound = type.IsUnbound;
			this.IsNullable = type.IsNullable;
			this.TypeArity = type.TypeArity;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetBaseTypeAfterCreation(TypeReferenceExpression baseType)
		{
			ReplaceDetailNode(BaseType, baseType);
			_BaseType = baseType;
		}
		#region ToString
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			lResult.Append(base.ToString());
			if (IsArrayType)
			{
				lResult.Append("[");
				for (int i = 0; i < _Rank - 1; i++)
					lResult.Append(",");
				lResult.Append("]");
			}
			if (IsConst)
				lResult.Append(" const");
			if (IsVolatile)
				lResult.Append(" volatile");
			bool isPointer = IsPointerType;
			bool isReferenceType = IsReferenceType;
			if (isPointer || isReferenceType)
			{
				if (isPointer)
				{
					if (IsManaged)
						lResult.Append("^");
					else
						lResult.Append("*");
				}
				if (isReferenceType)
				{
					if (IsManaged)
						lResult.Append("%");
					else
						lResult.Append("&");
				}
			}
			else if(IsConst || IsVolatile)
			{
				lResult.Append(" ");
			}
			if (IsNullable)
				lResult.Append("?");
			return GlobalStringStorage.Intern(lResult.ToString());
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.TypeReference;
		}
		#endregion		
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is TypeReferenceExpression)
			{
				TypeReferenceExpression lExpression = (TypeReferenceExpression)expression;
				if (IsArrayType)
				{
		  if (!lExpression.IsArrayType || Rank != lExpression.Rank)
			return false;
					if (BaseType == null)
						return lExpression.BaseType == null;
					return BaseType.IsIdenticalTo(lExpression.BaseType);
				}
				else if (IsPointerType)
				{
					if (BaseType == null)
						return false;
					return lExpression.IsPointerType && 
						BaseType.IsIdenticalTo(lExpression.BaseType);
				}
				else
				{
					bool lHasIdenticalNames = Name == lExpression.Name;
		  if (IsGeneric != lExpression.IsGeneric)
			return false;
					if (IsGeneric)
						return lExpression.IsGeneric &&
							lHasIdenticalNames &&
							TypeArguments.IsIdenticalTo(lExpression.TypeArguments);
					return lHasIdenticalNames;
				}
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
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_BaseType = null;
			if (_ArrayBounds != null)
			{
				_ArrayBounds.Clear();
				_ArrayBounds = null;
			}			
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypeReferenceExpression lClone = new TypeReferenceExpression();				
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public bool IsIdenticalTo(ITypeReferenceExpression typeRef)
		{
	  return StructuralParserServicesHolder.IsIdenticalTo(this, typeRef);
		}
		public ITypeReferenceExpression GetElementType()
		{
	  return StructuralParserServicesHolder.GetInnerType(this);
		}	
		public bool UsesTypeParameters(IGenericElement generic)
		{
	  return StructuralParserServicesHolder.UsesTypeParameters(this, generic);
		}
		public bool IsTypeParameter(IGenericElement generic)
		{
	  return StructuralParserServicesHolder.IsTypeParameter(this, generic);
		}
		public ITypeReferenceExpression CreateArrayReference(int rank)
		{
			return new TypeReferenceExpression(this, rank);
		}
		public override bool Is(string fullTypeName)
		{
	  ISourceTreeResolver resolver = StructuralParserServicesHolder.SourceTreeResolver;
			return Is(resolver, fullTypeName);
		}
		public override bool Is(ITypeElement type)
		{
			if (type == null)
				return false;
			return Is(type.FullName);
		}
		public override bool Is(Type type)
		{
			if (type == null)
				return false;
			return Is(type.FullName);
		}
		public override bool Is(ISourceTreeResolver resolver, string fullTypeName)
		{
			if (resolver == null)
				return false;
			ITypeElement typeElement = resolver.ResolveType(this) as ITypeElement;
			if (typeElement == null)
				return false;
			return typeElement.Is(resolver, fullTypeName);
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
				return LanguageElementType.TypeReferenceExpression;
			}
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
		public GCReferenceType GCType
		{
			get
			{
				return _GCType;
			}
			set
			{
				_GCType = value;
			}
		}
		#region BaseType
		public virtual TypeReferenceExpression BaseType
		{
			get
			{
				return _BaseType as TypeReferenceExpression;
			}
			set
			{
				SetBaseType(value);
			}
		}
		#endregion
		public override Expression Qualifier
		{
			get
			{
				return _BaseType;
			}
			set
			{
				_BaseType = value;
			}
		}
		#region Rank
		public virtual int Rank
		{
			get
			{
				return _Rank;
			}
			set
			{
				SetRank(value);
			}
		}
		#endregion
		public virtual ExpressionCollection ArrayBounds
		{
			get
			{
				if (_ArrayBounds == null)
					_ArrayBounds = new ExpressionCollection();
				return _ArrayBounds;
			}
			set
			{
				SetArrayBounds(value);
			}
		}
		#region IsArrayType
		public virtual bool IsArrayType
		{
			get
			{
				return _Type == TypeReferenceType.Array;
			}
		}
		#endregion
		#region IsPointerType
		public virtual bool IsPointerType
		{
			get
			{
				return _Type == TypeReferenceType.Pointer;
			}
		}
		#endregion
		#region IsConst
		public virtual bool IsConst
		{
			get
			{
				return _IsConst;
			}
			set
			{
				_IsConst = value;
			}
		}
		#endregion
		#region IsVolatile
		public virtual bool IsVolatile
		{
			get
			{
				return _IsVolatile;
			}
			set
			{
				_IsVolatile = value;
			}
		}
		#endregion
		#region IsManaged
		public virtual bool IsManaged
		{
			get
			{
				return _IsManaged;
			}
			set
			{
				_IsManaged = value;
			}
		}
		#endregion
		#region IsReferenceType
		public virtual bool IsReferenceType
		{
			get
			{
				return _Type == TypeReferenceType.Reference;
			}
		}
		#endregion		
		public virtual bool IsNullable
		{
			get
			{
				return _IsNullable;
			}
			set
			{
				_IsNullable = value;
			}
		}
		public virtual bool IsDynamic
		{
			get
			{
				return _IsDynamic;
			}
			set
			{
				_IsDynamic = value;
			}
		}
		public override int TypeArity
		{
			get
			{
				return _TypeArity;
			}
			set
			{
				_TypeArity = value;
			}
		}
		public virtual bool IsUnbound
		{
			get
			{
				return _IsUnbound;
			}
			set
			{
				_IsUnbound = value;
			}
		}
		public virtual TypeReferenceType TypeReferenceType
		{
			get
			{
				return _Type;
			}
			set
			{
				_Type = value;
			}
		}
		public bool HasElementType 
		{
			get
			{
				return TypeReferenceType != TypeReferenceType.None;
			}
		}
		public ArrayStorageClassQualifier ArrayStorageClass
		{
			get
			{
				return _ArrayStorageClass;
			}
			set
			{
				_ArrayStorageClass = value;
			}
		}
		public TypeReferenceExpression BasedPointer
		{
			get
			{
				return _BasedPointer;
			}
			set
			{
				_BasedPointer = value;
			}
		}
		public bool IsTypeCharacter
		{
			get
			{
				return _IsTypeCharacter;
			}
			set
			{
				_IsTypeCharacter = value;
			}
		}
		#region ITypeReferenceExpression Members
		ITypeReferenceExpression ITypeReferenceExpression.BaseType
		{
			get
			{
				return BaseType;
			}
		}
		int ITypeReferenceExpression.Rank
		{
			get
			{
				return Rank;
			}
		}
		IExpressionCollection ITypeReferenceExpression.ArrayBounds
		{
			get
			{
				if (_ArrayBounds == null)
					return EmptyLiteElements.EmptyIExpressionCollection;
				return _ArrayBounds;
			}
		}
		bool ITypeReferenceExpression.IsNullable
		{
			get
			{
				return IsNullable;
			}
		}
		bool ITypeReferenceExpression.IsDynamic
		{
			get
			{
				return IsDynamic;
			}
		}
		#endregion
		#region ITypeReferenceExpressionModifier Members
		public virtual ITypeReferenceExpression CreateResolvePoint()
		{
			return new TypeReferenceExpressionWrapper(this);
		}
		#endregion
	}
}
