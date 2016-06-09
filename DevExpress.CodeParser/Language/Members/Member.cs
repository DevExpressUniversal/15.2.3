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
using System.Collections.Specialized;
using System.ComponentModel;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{	
	public abstract class Member : AccessSpecifiedElement, IHasType
	{
		#region protected fields...
		string _Signature = String.Empty;
		SourceRange _TypeRange;
		string _MemberType;
		TypeReferenceExpression _MemberTypeReference;
		SourceRange _AsRange = SourceRange.Empty;
		#endregion
		#region private fields ...
		PostponedParsingData _PostponedData;
		StringCollection _Implements;
		ExpressionCollection _ImplementsExpressions;
		ExpressionCollection _HandlesExpressions;
		bool _IsExplicitInterfaceMember;
		SourceRange _ConstRange = SourceRange.Empty;
		#endregion
	bool _Unclosed = false;
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool Unclosed
	{
	  get
	  {
		return _Unclosed;
	  }
	  set
	  {
		_Unclosed = value;
	  }
	}
		#region Member
		public Member(): base()
		{
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			_MemberTypeReference = null;
			_ImplementsExpressions = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
	  if (_MemberTypeReference == oldElement)
	  {
		_MemberTypeReference = (TypeReferenceExpression)newElement;
		if (_MemberTypeReference == null)
		  _MemberType = String.Empty;
		else
		  _MemberType = _MemberTypeReference.FullSignature;
	  }
	  else if (_ImplementsExpressions != null && _ImplementsExpressions.Contains(oldElement))
		_ImplementsExpressions.ReplaceExpression(oldElement as Expression, newElement as Expression);
	  else if (_HandlesExpressions != null && _HandlesExpressions.Contains(oldElement))
		_HandlesExpressions.ReplaceExpression(oldElement as Expression, newElement as Expression);
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region GetFullSignature
		protected string GetFullSignature()
		{
			string lParentPath = ParentPath;
			string lSignatureSegment = GetSignature();
			string lFullPath;
			if (lSignatureSegment != String.Empty)
				if (lParentPath != String.Empty)
					lFullPath = lParentPath + "/" + lSignatureSegment;
				else
					lFullPath = lSignatureSegment;
			else 
				lFullPath = lParentPath;
			return lFullPath;
		}
		#endregion
		#region GetSignature
		protected virtual string GetSignature()
		{
			return PathSegment;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Member))
				return;			
			Member lSource = (Member)source;
			_Signature = lSource._Signature;
			_TypeRange = lSource.TypeRange;
	  _AsRange = lSource.AsRange;
	  _ConstRange = lSource.ConstRange;
	  _Unclosed = lSource.Unclosed;
			_MemberType = lSource._MemberType;
			_MemberTypeReference = ParserUtils.GetCloneFromNodes(this, lSource, lSource._MemberTypeReference) as TypeReferenceExpression;
			if (_MemberTypeReference == null && lSource._MemberTypeReference != null)
				_MemberTypeReference = lSource._MemberTypeReference.Clone(options) as TypeReferenceExpression;
			if (lSource._Implements != null)
			{
				_Implements = new StringCollection();
				for (int i = 0; i < lSource._Implements.Count; i++)
					_Implements.Add(lSource._Implements[i]);
			}
			if (lSource._ImplementsExpressions != null)
			{
				_ImplementsExpressions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _ImplementsExpressions, lSource._ImplementsExpressions);
				if (_ImplementsExpressions.Count == 0 && lSource._ImplementsExpressions.Count > 0)
					_ImplementsExpressions = lSource._ImplementsExpressions.DeepClone(options) as ExpressionCollection;
			}
			if (lSource._HandlesExpressions != null)
			{
				_HandlesExpressions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _HandlesExpressions, lSource._HandlesExpressions);
				if (_HandlesExpressions.Count == 0 && lSource._HandlesExpressions.Count > 0)
					_HandlesExpressions = lSource._HandlesExpressions.DeepClone(options) as ExpressionCollection;
			}
			_IsExplicitInterfaceMember = lSource._IsExplicitInterfaceMember;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _TypeRange = TypeRange;
	  _AsRange = AsRange;
	  _ConstRange = ConstRange;
	}
		protected override IExpressionCollection GetImplementExpressions()
		{
			return ImplementsExpressions;
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
	  if (IsStatic)
		return ImageIndex.StaticField;
	  else if (this is IVariableDeclarationStatement && ((IVariableDeclarationStatement)this).IsFixedSizeBuffer)
		return ImageIndex.FixedField;
	  else
		return ImageIndex.Field;
		}
		#endregion
		#region GetTypeName
		public override string GetTypeName()
		{
			return MemberType;
		}
		#endregion
		public void AddImplementsExpression(Expression expression)
		{
			if (expression == null)
				return;
			string str = expression.ToString();
			if (_ImplementsExpressions == null)
				_ImplementsExpressions = new ExpressionCollection();
			_ImplementsExpressions.Add(expression);
			Implements.Add(str);
			AddDetailNode(expression);
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public string GetNameWithoutImplementsQualifier()
	{
	  return ImplementsCount > 0 ? ImplementsExpressions[0].Name : Name;
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveImplementsExpression(Expression expression)
		{
			if (expression == null)
				return;
			string str = expression.ToString();
			if (_ImplementsExpressions != null)
				_ImplementsExpressions.Remove(expression);
			Implements.Remove(str);
			RemoveDetailNode(expression);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddHandlesExpression(Expression expression)
		{
			if (expression == null)
				return;
			if (_HandlesExpressions == null)
				_HandlesExpressions = new ExpressionCollection();
			_HandlesExpressions.Add(expression);
			AddDetailNode(expression);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveHandlesExpression(Expression expression)
		{
			if (expression == null)
				return;
			if (_HandlesExpressions != null)
				_HandlesExpressions.Remove(expression);
			RemoveDetailNode(expression);
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetMemberTypeReference(TypeReferenceExpression type)
	{
	  if (type == null)
		return;
	  MemberTypeReference = type;
	  AddDetailNode(type);
	}
		public virtual bool Is(string fullTypeName)
		{
			if (MemberTypeReference == null)
				return false;
			return MemberTypeReference.Is(fullTypeName);
		}
		public virtual bool Is(ITypeElement type)
		{
			if (MemberTypeReference == null)
				return false;
			return MemberTypeReference.Is(type);
		}
		public virtual bool Is(Type type)
		{
			if (MemberTypeReference == null)
				return false;
			return MemberTypeReference.Is(type);
		}
		public virtual bool Is(ISourceTreeResolver resolver, string fullTypeName)
		{
			if (MemberTypeReference == null)
				return false;
			return MemberTypeReference.Is(resolver, fullTypeName);
		}
		protected override PostponedParsingData PostponedData 
		{
			get { return _PostponedData; }
			set { _PostponedData = value; }
		}
		#region MemberType
		[Description("The type of this member or method.")]
		[Category("Details")]
		[DefaultValue("")]
		public string MemberType
		{
			get
			{
				return _MemberType;
			}
			set
			{
		if (_MemberType == value)
					return;
				_MemberType = value;
			}
		}
		#endregion
		#region MemberTypeReference
		[Description("The type of this member or method.")]
		[Category("Details")]
		[DefaultValue("")]
		public TypeReferenceExpression MemberTypeReference
		{
			get
			{
				return _MemberTypeReference;
			}
			set
			{
				_MemberTypeReference = value;
			}
		}
		#endregion
		#region TypeRange
		[Description("The SourceRange of this member type.")]
		[Category("Details")]
		public SourceRange TypeRange
		{
			get
			{
				return GetTransformedRange(_TypeRange);
			}
			set
			{
		ClearHistory();
				_TypeRange = value;
			}
		}
		#endregion
		#region Signature
		[Description("The signature of this member.")]
		[Category("Details")]
		public string Signature
		{
			get
			{
				if (_Signature == String.Empty)
					_Signature = GetFullSignature();
				return _Signature;
			}
		}
		#endregion
		[Description("Returns true if this member is explicitly declared interface member.")]
		[Category("Details")]
		public override bool IsExplicitInterfaceMember
		{
			get
			{
				return _IsExplicitInterfaceMember;
			}
			set
			{
				_IsExplicitInterfaceMember = value;
			}
		}
		[Category("Details")]
		public StringCollection Implements
		{
			get
			{
				if (_Implements == null)
					_Implements = new StringCollection();
				return _Implements;
			}
		}
		[Category("Details")]
		public int ImplementsCount
		{
			get
			{
				if (_Implements == null)
					return 0;
				return _Implements.Count;
			}
		}
		[Category("Details")]
		public ExpressionCollection ImplementsExpressions
		{
			get
			{
				return _ImplementsExpressions;
			}
		}		
		[Category("Details")]
		public ExpressionCollection HandlesExpressions
		{
			get
			{
				return _HandlesExpressions;
			}
		}		
		public SourceRange ConstRange
		{
			get
			{
				return GetTransformedRange(_ConstRange);
			}
			set
			{
		ClearHistory();
				_ConstRange = value;
			}
		}
		public SourceRange AsRange
		{
			get
			{
				return GetTransformedRange(_AsRange);
			}
			set
			{
		ClearHistory();
				_AsRange = value;
			}
		}
		#region VisibilityIsFixed
		[Description("True if the visibility of this member can not be changed (e.g., illegal, param, local, or if this is a static constructor).")]
		[Category("Access")]
		public override bool VisibilityIsFixed
		{
			get
			{
				if (IsExplicitInterfaceMember || InsideInterface)
					return true;
				else
					return base.VisibilityIsFixed;
			}
		}
		#endregion
		#region IHasType Members
		ITypeReferenceExpression IHasType.Type
		{
			get
			{
				return MemberTypeReference;
			}
		}
		#endregion    
	#region SetAccessSpecifiers
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void SetAccessSpecifiers(AccessSpecifiers specifiers)
	{	  
	  base.SetAccessSpecifiers(specifiers);
	  if (AccessSpecifiers == null)
		return;	  
	  CheckVisibilities();
	}
	#endregion
	private void CheckVisibilities()
	{
	  if (!IsVirtual && !IsAbstract)
		return;
	  MemberVisibility[] validVisibilities = this.ValidVisibilities;
	  if (validVisibilities == null)
		return;
	  if (validVisibilities.Length == 0)
		return;
	  ArrayList array = new ArrayList();
	  foreach (MemberVisibility visibility in validVisibilities)
		if (visibility == MemberVisibility.Private)
		  continue;
		else
		  array.Add(visibility);
	  this.ValidVisibilities = (MemberVisibility[])array.ToArray(typeof(MemberVisibility));
	}
	}
}
