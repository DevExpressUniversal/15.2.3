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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class TypeReferenceExpressionWrapper : TypeReferenceExpression, IHasResolvePoint
	{
		LanguageElement _ResolvePoint;
		public TypeReferenceExpressionWrapper()
		{			
		}
		public TypeReferenceExpressionWrapper(TypeReferenceExpression typeReference)
		{
			ResolvePoint = typeReference;
		}
		IElement IHasResolvePoint.ResolvePoint
		{
			get
			{				
				return ResolvePoint;
			}
			set
			{
				ResolvePoint = value as LanguageElement;
			}
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;			
			base.CloneDataFrom(source, options);
			if (source is TypeReferenceExpressionWrapper)
				_ResolvePoint = ((TypeReferenceExpressionWrapper)source)._ResolvePoint;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypeReferenceExpressionWrapper lClone = new TypeReferenceExpressionWrapper();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		TypeReferenceExpression GetTypeReference()
		{
			return ResolvePoint as TypeReferenceExpression;
		}
		public override string ToString()
		{
			TypeReferenceExpression lType = GetTypeReference();
			if (lType != null)
				return lType.ToString();
			return String.Empty;
		}
	public override IElement Resolve(ISourceTreeResolver resolver)
	{
	  ITypeReferenceExpression typeRef = GetTypeReference();
	  if (typeRef == null)
		return null;
	  return resolver.ResolveType(typeRef);
	}
		public override string Name
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.Name;
				return String.Empty;
			}
			set
			{
			}
		}
		public override SourceRange NameRange
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.NameRange;
				return SourceRange.Empty;
			}
			set
			{
			}
		}		
		#region BaseType
		public override TypeReferenceExpression BaseType
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.BaseType;
				return null;
			}
			set
			{
			}
		}
		#endregion
		#region Rank
		public override int Rank
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.Rank;
				return 0;
			}
			set
			{
			}
		}
		#endregion
		public override ExpressionCollection ArrayBounds
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.ArrayBounds;
				return null;
			}
			set
			{
			}
		}
		#region IsArrayType
		public override bool IsArrayType
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.IsArrayType;
				return false;
			}
		}
		#endregion
		#region IsPointerType
		public override bool IsPointerType
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.IsPointerType;
				return false;
			}
		}
		#endregion
		public override bool IsConst
		{
			get
			{
				TypeReferenceExpression typeRef = GetTypeReference();
				if (typeRef != null)
					return typeRef.IsConst;
				return false;
			}
		}
		public override bool IsVolatile
		{
			get
			{
				TypeReferenceExpression typeRef = GetTypeReference();
				if (typeRef != null)
					return typeRef.IsVolatile;
				return false;
			}
		}		
		public override bool IsReferenceType
		{
			get
			{
				TypeReferenceExpression typeRef = GetTypeReference();
				if (typeRef != null)
					return typeRef.IsReferenceType;
				return false;
			}
		}
		public override bool IsManaged
		{
			get
			{
				TypeReferenceExpression typeRef = GetTypeReference();
				if (typeRef != null)
					return typeRef.IsManaged;
				return false;
			}
		}		
		public override TypeReferenceType TypeReferenceType
		{
			get
			{
				TypeReferenceExpression typeRef = GetTypeReference();
				if (typeRef != null)
					return typeRef.TypeReferenceType;
				return TypeReferenceType.None;
			}
		}
		public override bool IsNullable
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.IsNullable;
				return false;
			}
			set
			{
			}
		}
		public override bool IsUnbound
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.IsUnbound;
				return false;
			}
		}
		public LanguageElement ResolvePoint
		{
			get
			{
				return _ResolvePoint;
			}
			set
			{
				_ResolvePoint = value;
			}
		}
		public override bool IsGeneric
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.IsGeneric;
				return false;
			}
		}
		public override TypeReferenceExpressionCollection TypeArguments
		{
			get
			{
				TypeReferenceExpression lType = GetTypeReference();
				if (lType != null)
					return lType.TypeArguments;
				return null;
			}			
		}
		#region ITypeReferenceExpressionModifier Members
		public override ITypeReferenceExpression CreateResolvePoint()
		{
			if (ResolvePoint == null)
				return new TypeReferenceExpressionWrapper(this);
			return ((ITypeReferenceExpressionModifier)ResolvePoint).CreateResolvePoint();
		}
		#endregion
	}
}
