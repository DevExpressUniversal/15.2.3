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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{	
	public abstract class ReferenceExpressionBase: Expression, IHasQualifier, IReferenceExpression
	{
		TypeReferenceExpressionCollection _TypeArguments;
		Expression _NameQualifier;
	bool _IsMulOperator;
		public ReferenceExpressionBase()
		{
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ReferenceExpressionBase))
				return;						
			ReferenceExpressionBase lReferenceBase = (ReferenceExpressionBase)source;
			_TypeArguments = new TypeReferenceExpressionCollection();
	  _IsMulOperator = lReferenceBase._IsMulOperator;
			if (lReferenceBase._NameQualifier != null)
				_NameQualifier = lReferenceBase._NameQualifier.Clone() as Expression;
			if (lReferenceBase._TypeArguments != null)
			{
				ParserUtils.GetClonesFromNodes(DetailNodes, lReferenceBase.DetailNodes, _TypeArguments, lReferenceBase._TypeArguments);
				if (_TypeArguments.Count == 0 && lReferenceBase._TypeArguments.Count > 0)
					_TypeArguments = lReferenceBase._TypeArguments.DeepClone(options) as TypeReferenceExpressionCollection;
			}
		}
		#endregion
		#region ReplaceOwnedReference
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
	  if (TypeArguments.Contains(oldElement as Expression))
		TypeArguments.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void SetTypeArguments(TypeReferenceExpressionCollection typeArguments)
		{
	  if (_TypeArguments != null)
		RemoveDetailNodes(_TypeArguments);
			_TypeArguments = typeArguments;
	  if (_TypeArguments != null)
		AddDetailNodes(_TypeArguments);
		}
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{			
			if (_TypeArguments != null)
			{
				_TypeArguments.Clear();
				_TypeArguments = null;
			}
			base.CleanUpOwnedReferences();
		}
		#endregion
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			if (Qualifier != null)
			{
				lResult.Append(Qualifier.ToString());
				if (InternalName != null && InternalName.Length > 0)
				{
					if (Qualifier is MemberAccessExpression)
						lResult.AppendFormat("{0}", InternalName);
					else
						lResult.AppendFormat(".{0}", InternalName);
				}
			}
			else if (InternalName != null && InternalName.Length > 0)
				lResult.Append(InternalName);
			if (IsGeneric)
			{
				int lCount = TypeArguments.Count;
				lResult.Append("<");
				for (int i = 0; i < lCount; i++)
				{
					lResult.Append(TypeArguments[i].ToString());
					if (i < lCount - 1)
						lResult.Append(", ");
				}
				lResult.Append(">");
			}
			return lResult.ToString();
		}
		internal void GetDataFrom(BaseElement source)
		{
			CloneDataFrom(source, ElementCloneOptions.Default);
		}
		public QualifiedElementReference CreateQualifiedElementReference()
		{
			QualifiedElementReference result = new QualifiedElementReference();
			result.GetDataFrom(this);
			return result;
		}
	#region IsIdenticalTo
	public override bool IsIdenticalTo(Expression expression)
	{
	  if (expression == null)
		return false;
	  ReferenceExpressionBase reference = expression as ReferenceExpressionBase;
	  if (reference != null)
	  {
		TypeReferenceExpressionCollection typeArguments = TypeArguments;
		if (typeArguments == null || typeArguments.Count == 0)
		  return reference.TypeArguments == null || reference.TypeArguments.Count == 0;
		return typeArguments.IsIdenticalTo(reference.TypeArguments);
	  }
	  return false;
	}
	#endregion
	public void AddTypeArgument(TypeReferenceExpression typeArgument)
	{
	  if (typeArgument == null)
		return;
	  TypeArguments.Add(typeArgument);
	  AddDetailNode(typeArgument);
	}
	public void AddTypeArguments(IEnumerable<TypeReferenceExpression> typeArguments)
	{
	  if (typeArguments == null)
		return;
	  foreach (TypeReferenceExpression typeArgument in typeArguments)
		AddTypeArgument(typeArgument);
	}
		public abstract Expression Qualifier { get; set; }
		[Obsolete("Use Qualifier instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]		
		public Expression Source
		{
	  get { return Qualifier; }
	  set { Qualifier = value; }
		}
		public virtual bool IsGeneric
		{
	  get { return _TypeArguments != null && _TypeArguments.Count > 0; }
		}
		public virtual TypeReferenceExpressionCollection TypeArguments
		{
			get
			{
				if (_TypeArguments == null)
					_TypeArguments = new TypeReferenceExpressionCollection();
				return _TypeArguments;
			}
			set
			{
				SetTypeArguments(value);
			}
		}
		public bool IsTypeArgument
		{
			get
			{
				ReferenceExpressionBase parent = ParentGenericReference;
				if (parent == null)
					return false;
				return parent.TypeArguments != null && parent.TypeArguments.Contains(this);
			}
		}
		public ReferenceExpressionBase ParentGenericReference
		{
			get
			{
				if (!(Parent is ReferenceExpressionBase))
					return null;
				ReferenceExpressionBase parent = (ReferenceExpressionBase)Parent;
				if (!parent.IsGeneric)
					return null;
				return parent;
			}
		}
		public virtual int TypeArity
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public Expression NameQualifier
		{
	  get { return _NameQualifier; }
	  set { _NameQualifier = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsMulOperator
	{
	  get { return _IsMulOperator; }
	  set { _IsMulOperator = value; }
	}
		#region IGenericExpression Members
		ITypeReferenceExpressionCollection IGenericExpression.TypeArguments
		{
			get
			{
				if (_TypeArguments == null)
					return EmptyLiteElements.EmptyITypeReferenceExpressionCollection;
				return _TypeArguments;
			}
		}		
		#endregion
		#region IWithSource Members
		IExpression IWithSource.Source
		{
			get
			{
				return Qualifier;
			}
		}		
		#endregion		
		#region IReferenceExpression Members
		IExpression IReferenceExpression.NameQualifier
		{
	  get { return _NameQualifier; }
		}
		public string FullSignature 
		{ 
			get
			{
				string signaturePart = GetSignaturePart();
				if (Qualifier == null)
					return signaturePart;
				string sourceSignature = String.Empty;
				if (Qualifier != null)
		{
		  if(Qualifier is IReferenceExpression)
			sourceSignature = ((IReferenceExpression)Qualifier).FullSignature;
		  if (Qualifier is IBaseReferenceExpression || Qualifier is IThisReferenceExpression)
			sourceSignature = Qualifier.Name;
		}
		if (string.IsNullOrEmpty(sourceSignature))
		  return signaturePart;
				return String.Format("{0}.{1}", sourceSignature, signaturePart);
			}
		}
		#endregion
	}
}
