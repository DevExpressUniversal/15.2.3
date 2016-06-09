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
	public class TypeParameter : CodeElement, ITypeParameter, ITypeParameterModifier
	{
		TypeParameterConstraintCollection _Constraints;
		ITypeReferenceExpression _ActivatedType;
		SourceRange _NameRange;
	TypeParameterDirection _Direction = TypeParameterDirection.None;
		protected TypeParameter()			
		{
		}
		public TypeParameter(string name)
			: this(name, SourceRange.Empty)
		{
		}
		public TypeParameter(string name, SourceRange range)
			: this(name, null, range)
		{
		}
		public TypeParameter(string name, TypeParameterConstraintCollection constraints,  SourceRange range)
		{
			InternalName = name;
			SetRange(range);
			SetConstraints(constraints);
		}
		private void GetConstraintsFromDetailNodes()
		{			
			for (int i = 0; i < DetailNodeCount; i++)
			{
				TypeParameterConstraint lConstraint = DetailNodes[i] as TypeParameterConstraint;
				if (lConstraint == null)
					continue;
				Constraints.Add(lConstraint);
			}
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
	  base.CloneDataFrom(source, options);
			TypeParameter tp = source as TypeParameter;
			if (tp == null)
				return;
			_NameRange = tp.NameRange;
	  _Direction = tp.Direction;
			GetConstraintsFromDetailNodes();
		}
		#endregion		
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypeParameter lClone = new TypeParameter();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override void CleanUpOwnedReferences()
		{
			if (_Constraints != null)
			{
				_Constraints.Clear();
				_Constraints = null;
			}
			base.CleanUpOwnedReferences();
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			int lIndex = _Constraints == null ? -1 : _Constraints.IndexOf(oldElement);
			if (lIndex >= 0)
			{
				_Constraints.RemoveAt(lIndex);
				if (newElement != null)
					_Constraints.Insert(lIndex, newElement);
			}
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		protected virtual void SetConstraints(TypeParameterConstraintCollection constraints)
		{
			ReplaceDetailNodes(_Constraints, constraints);
			_Constraints = constraints;
		}
		public override int GetImageIndex()
		{
			return ImageIndex.TypeParameter;
		}
	public void AddConstraint(TypeParameterConstraint constraint)
	{
	  if (constraint == null)
		return;
	  Constraints.Add(constraint);
	  AddDetailNode(constraint);
	}
	public void RemoveConstraint(TypeParameterConstraint constraint)
	{
	  if (constraint == null)
		return;
	  Constraints.Remove(constraint);
	  RemoveDetailNode(constraint);
	}
	public TypeParameterDirection Direction
	{
	  get
	  {
		return _Direction;
	  }
	  set
	  {
		_Direction = value;
	  }
	}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.TypeParameter;
			}
		}
		public TypeParameterConstraintCollection Constraints
		{
			get
			{
				if (_Constraints == null)
					_Constraints = new TypeParameterConstraintCollection();
				return _Constraints;
			}
			set
			{
				SetConstraints(value);
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
		protected override bool HasOuterRangeChildren
		{
			get
			{
				return true;
			}
		}
		#region ITypeParameter Members
		bool ITypeParameter.IsActivated
		{
			get
			{				
				return _ActivatedType != null;
			}
		}
		ITypeReferenceExpression ITypeParameter.ActivatedType 
		{
			get
			{				
				return _ActivatedType;
			}
		}
		ITypeParameterConstraintCollection ITypeParameter.Constraints
		{
			get
			{
				if (Constraints == null)
					return EmptyLiteElements.EmptyITypeParameterConstraintCollection;
				return Constraints;
			}
		}
		TypeParameterDirection ITypeParameter.Direction
		{
			get { return _Direction; }
		}
		#endregion			
	#region ITypeParameterModifier Members
	void ITypeParameterModifier.SetActivatedType(ITypeReferenceExpression type)
	{
	  _ActivatedType = type;
	}
	void ITypeParameterModifier.AddConstraint(ITypeParameterConstraint constraint)
	{
	  AddConstraint(constraint as TypeParameterConstraint);
	}
	void ITypeParameterModifier.RemoveConstraint(ITypeParameterConstraint constraint)
	{
	  RemoveConstraint(constraint as TypeParameterConstraint);
	}
	void ITypeParameterModifier.SetDirection(TypeParameterDirection direction)
	{
	  Direction = direction;
	}
	#endregion
  }
}
