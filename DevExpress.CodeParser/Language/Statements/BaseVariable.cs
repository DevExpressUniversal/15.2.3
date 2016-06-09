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
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class BaseVariable : Member, IVariableDeclarationStatement, IBaseVariable, IHasArrayNameModifier
	{
		private const int INT_MaintainanceComplexity = 1;
		#region private fields...
		string _FullTypeName;
		LanguageElementCollection _ArrayNameModifiers;
		NullableTypeModifier _NullableModifier = null;
		bool _IsImplicit;
		bool _IsReturnedValue;
		bool _IsLiteral = false;
		bool _IsInitOnly = false;
		SourceRange _OperatorRange = SourceRange.Empty;
		#endregion
		#region BaseVariable
		public BaseVariable()
		{
		}
		#endregion
	public IEnumerable<IElement> GetBlockChildren()
	{
	  foreach (object nodeObj in Nodes)
	  {
		IElement node = nodeObj as IElement;
		if (node != null)
		  yield return node;
	  }
	}
		Expression GetIterationExpression()
		{
			ForEach forEach = Parent as ForEach;
			if (forEach == null)
				return null;
			if (!IsDetailNode)
				return null;
			return forEach.Expression;
		}
		LanguageElement GetParentAspxFieldOrHtmlAttribute()
		{
			HtmlAttribute parentAttr = GetParent(LanguageElementType.HtmlAttribute) as HtmlAttribute;
			if (parentAttr != null)
				return parentAttr;
			Variable parentVar = GetParent(LanguageElementType.Variable, LanguageElementType.InitializedVariable) as Variable;
			if (parentVar != null && parentVar.IsAspxTag)
				return parentVar;
			return null;
		}
		#region FindAllReferences
		protected new virtual ArrayList FindAllReferences()
		{
	  return StructuralParserServicesHolder.FindAllReferencesForBaseVariable(this);
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_NullableModifier == oldElement)
				_NullableModifier = newElement as NullableTypeModifier;
			else if (_ArrayNameModifiers != null && _ArrayNameModifiers.Contains(oldElement))
				_ArrayNameModifiers.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is BaseVariable))
				return;			
			BaseVariable lSource = (BaseVariable)source;			
			_FullTypeName = lSource._FullTypeName;
			_IsInitOnly = lSource._IsInitOnly;
			_IsLiteral = lSource._IsLiteral;
			_OperatorRange = lSource.OperatorRange;
			NullableTypeModifier sourceNullableModifier = lSource._NullableModifier;
			if (sourceNullableModifier != null)
				_NullableModifier = sourceNullableModifier.Clone(options) as NullableTypeModifier;
	  if (lSource._ArrayNameModifiers != null)
			{
				_ArrayNameModifiers = new LanguageElementCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _ArrayNameModifiers, lSource._ArrayNameModifiers);
				if (_ArrayNameModifiers.Count == 0 && lSource._ArrayNameModifiers.Count > 0)
					_ArrayNameModifiers = lSource._ArrayNameModifiers.DeepClone(options) as LanguageElementCollection;
			}
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _OperatorRange = OperatorRange;
	}
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			base.OwnedReferencesTransfered();
		}
		#endregion
		#region GetValidVisibilities()
		public override MemberVisibility[] GetValidVisibilities()
		{
			return ValidVisibilities;
		}
		#endregion
		#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
		{
			BaseVariable lClone = new BaseVariable();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region HasUsedReferences
		public bool HasUsedReferences() 
		{ 			
			ArrayList lReferences = References;
			if (lReferences == null)
				return false;
			foreach(ElementReferenceExpression reference in References)
				if (reference.IsUsed)
					return true;
			return false;
		}
		#endregion
	public void AddArrayNameModifier(ArrayNameModifier modifier)
	{
	  if (modifier == null)
		return;
	  ArrayNameModifiers.Add(modifier);
	  AddDetailNode(modifier);
	}
	public void AddArrayNameModifiers(IEnumerable<ArrayNameModifier> modifiers)
	{
	  if (modifiers == null)
		return;
	  foreach (ArrayNameModifier modifier in modifiers)
		AddArrayNameModifier(modifier);
	}
		protected void SetFullTypeName(string value)
		{
			_FullTypeName = value;
		}
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public ArrayList References
		{
			get
			{
				return FindAllReferences();
			}
		}
		public string FullTypeName
		{
			get
			{
				return _FullTypeName;
			}
		}
		public bool IsField
		{
			get
			{
				return Parent is TypeDeclaration || Parent is SourceFile;
			}
		}
		public bool IsLocal
		{
			get
			{
				LanguageElement lParent = GetParentCodeBlock();
				if (lParent == null)
					lParent = GetParentProperty();
				if (lParent == null)
					lParent = GetParentAspxFieldOrHtmlAttribute();			
				return lParent != null;
			}
		}		
		public bool IsImplicit
		{
			get
			{
				return _IsImplicit;
			}
			set
			{
				_IsImplicit = value;
			}
		}
		public virtual bool IsFixedSizeBuffer
		{
			get
			{
				return false;
			}
		}
		public bool IsReturnedValue
		{
			get
			{
				return _IsReturnedValue;
			}
			set
			{
				_IsReturnedValue = value;
			}
		}
		public override MemberVisibility[] ValidVisibilities
		{
			get
			{
				if (IsLocal)
					return new MemberVisibility[] {};
				return base.ValidVisibilities;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Local;
			}
		}
		public LanguageElementCollection ArrayNameModifiers
		{
			get
			{
				if (_ArrayNameModifiers == null)
					_ArrayNameModifiers = new LanguageElementCollection();
				return _ArrayNameModifiers;
			}
		}
		public bool HasArrayNameModifiers
		{
			get
			{
				return _ArrayNameModifiers != null && _ArrayNameModifiers.Count > 0;
			}
		}
		[Description("Returns true if this variable declares bit field.")]
		[Category("Family")]
		public virtual bool IsBitField
		{
			get
			{
				return false;
			}
		}
		[Description("Gets or sets fixed size variable expression.")]
		[Category("Family")]
		public virtual Expression BitFieldSize
		{
			get
			{
				return null;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override IEnumerable AllVariables
		{
			get
			{
				return base.AllVariables;
			}
		}		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override IEnumerable AllStatements
		{
			get
			{
				return base.AllStatements;
			}
		}		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override IEnumerable AllFlowBreaks
		{
			get
			{
				return base.AllFlowBreaks;
			}
		}
		public bool IsLiteral
		{
			get
			{
				return _IsLiteral;
			}
			set
			{
				_IsLiteral = value;
			}
		}
		public bool IsInitOnly
		{
			get
			{
				return _IsInitOnly;
			}
			set
			{
				_IsInitOnly = value;
			}
		}
		public SourceRange OperatorRange
		{
			get
			{
				return GetTransformedRange(_OperatorRange);
			}
			set
			{
		ClearHistory();
				_OperatorRange = value;
			}
		}
		public bool HasIterationExpression 
		{
			get
			{
				return GetIterationExpression() != null;
			}
		}
		public IExpression IterationExpression
		{
			get
			{
				return GetIterationExpression();
			}
		}
		public virtual bool IsObjectCreationInit
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsAspxTag
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual bool IsRunAtServer
	{
	  get
	  {
		return false;
	  }
	  set
	  {
	  }
	}
		public NullableTypeModifier NullableModifier
		{
			get
			{
				return _NullableModifier;
			}
			set
			{
				_NullableModifier = value;
			}
		}
		#region IVariableDeclarationStatement Members
		bool IVariableDeclarationStatement.IsConst
		{
			get
			{				
				return false;
			}
		}
		IExpression IVariableDeclarationStatement.Expression
		{
			get
			{				
				return null;
			}
		}
		bool IVariableDeclarationStatement.IsBitField
		{
			get
			{
				return IsBitField;
			}
		}
		IExpression IVariableDeclarationStatement.BitFieldSize
		{
			get
			{
				return BitFieldSize;
			}
		}
		ICollection IHasArrayNameModifier.Modifiers
		{
			get
			{
				return _ArrayNameModifiers;
			}
		}
		#endregion
		#region IBaseVariable Members
		bool IBaseVariable.IsParameter
		{
			get
			{
				return false;
			}
		}
		IExpression IBaseVariable.NameQualifier
		{
			get
			{
				return NameQualifier;
			}
		}
		#endregion
	}
}
