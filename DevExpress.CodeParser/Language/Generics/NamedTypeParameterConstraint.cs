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
	public class NamedTypeParameterConstraint : TypeParameterConstraint, INamedTypeParameterConstraint
	{
		TypeReferenceExpression _TypeReference;
		protected NamedTypeParameterConstraint()			
		{
		}
		public NamedTypeParameterConstraint(TypeReferenceExpression typeReference)
			: base(String.Empty, typeReference.Range)
		{
			SetTypeReference(typeReference);
		}
		void GetTypeReferenceFromDetailNodes()
		{
			if (DetailNodeCount == 0)
				return;
			TypeReferenceExpression lTypeReference = DetailNodes[0] as TypeReferenceExpression; 
			if (lTypeReference != null)
				_TypeReference = lTypeReference;
		}
		protected virtual void SetTypeReference(TypeReferenceExpression typeReference)
		{
			ReplaceDetailNode(_TypeReference, typeReference);
			_TypeReference = typeReference;
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_TypeReference == oldElement && newElement is TypeReferenceExpression)
				_TypeReference = (TypeReferenceExpression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);			
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is NamedTypeParameterConstraint))
				return;			
			GetTypeReferenceFromDetailNodes();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			NamedTypeParameterConstraint lClone = new NamedTypeParameterConstraint();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override void CleanUpOwnedReferences()
		{
			_TypeReference = null;
			base.CleanUpOwnedReferences();
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.NamedTypeParameterConstraint;
			}
		}
		public TypeReferenceExpression TypeReference
		{
			get
			{
				return _TypeReference;
			}
			set
			{
				SetTypeReference(value);
			}
		}
		#region INamedTypeParameterConstraint Members
		ITypeReferenceExpression INamedTypeParameterConstraint.TypeReference
		{
			get
			{
				return TypeReference;
			}
		}
		#endregion
	}
}
