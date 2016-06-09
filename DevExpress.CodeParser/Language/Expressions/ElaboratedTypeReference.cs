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
	public enum ElaboratedKind
	{
		Unknown,
		Class,
		Struct,
		Enum,
		Union,
		Type,
		RefClass,
		RefStruct,
		ValueClass,
		ValueStruct
	}
	public enum InheritanceModel
	{
		Default,
		Single,
		Multiple,
		Virtual
	}
	public class ElaboratedTypeReference : TypeReferenceExpression, IElaboratedTypeElement
	{
		ElaboratedKind _ElaboratedKind = ElaboratedKind.Unknown;
		InheritanceModel _InheritanceModel;
		Expression _SourceExpression;
		protected ElaboratedTypeReference()
		{
		}
		public ElaboratedTypeReference(string type)
			: base(type)
		{
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (source is ElaboratedTypeReference)
			{
				ElaboratedTypeReference expression = (ElaboratedTypeReference)source;
				if (expression.SourceExpression != null)
				{				
					_SourceExpression = ParserUtils.GetCloneFromNodes(this, expression, expression.SourceExpression) as Expression;
					if (_SourceExpression == null)
						_SourceExpression = expression.SourceExpression.Clone(options) as Expression;
				}
				_ElaboratedKind = expression.ElaboratedKind;
				_InheritanceModel = expression._InheritanceModel;
			}
		}
	void SetSourceExpression(Expression expr)
	{
	  if (_SourceExpression != null)
		RemoveDetailNode(_SourceExpression);
	  _SourceExpression = expr;
	  if (_SourceExpression != null)
		AddDetailNode(_SourceExpression);
	}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ElaboratedTypeReference expression = new ElaboratedTypeReference();
			expression.CloneDataFrom(this, options);
			return expression;
		}		
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.ElaboratedTypeReference; }
		}
		public ElaboratedKind ElaboratedKind
		{
	  get { return _ElaboratedKind; }
	  set { _ElaboratedKind = value; }
		}
		public Expression SourceExpression
		{
	  get { return _SourceExpression; }
	  set { SetSourceExpression(value); }
		}
		public InheritanceModel InheritanceModel
		{
	  get { return _InheritanceModel; }
	  set { _InheritanceModel = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool IsTypeDeclaration
		{
	  get { return false; }
		}
		#region IElaboratedTypeElement Members
		ElaboratedKind IElaboratedTypeElement.ElaboratedKind
		{
	  get { return _ElaboratedKind; }
		}
		IExpression IElaboratedTypeElement.Expression
		{
	  get { return _SourceExpression; }
		}
		#endregion
		#region ITypeElement Members
		ITypeReferenceExpression ITypeElement.PrimaryAncestor
		{
	  get { return null; }
		}
		ITypeReferenceExpressionCollection ITypeElement.SecondaryAncestors
		{
	  get { return null; }
		}
		IMemberElementCollection ITypeElement.Members
		{
	  get { return null; }
		}
	bool ITypeElement.IsTypeParameter
	{
	  get { return false; }
	}
	ITypeElement[] ITypeElement.GetAllDescendants()
	{
	  return null;
	}
	ITypeElement[] ITypeElement.GetAllDescendants(IElement scope)
	{
	  return null;
	}
	ITypeElement[] ITypeElement.GetAllDescendants(ISourceTreeResolver resolver)
	{
	  return null;
	}
	ITypeElement[] ITypeElement.GetAllDescendants(ISourceTreeResolver resolver, IElement scope)
	{
	  return null;
	}
	ITypeElement ITypeElement.GetBaseType()
		{
			return null;
		}
		ITypeElement ITypeElement.GetBaseType(ISourceTreeResolver resolver)
		{
			return null;
		}
		ITypeElement[] ITypeElement.GetBaseTypes()
		{
			return null;
		}
		ITypeElement[] ITypeElement.GetBaseTypes(ISourceTreeResolver resolver)
		{
			return null;
		}
		ITypeElement[] ITypeElement.GetDescendants()
		{
			return null;
		}
		ITypeElement[] ITypeElement.GetDescendants(IElement scope)
		{
			return null;
		}
		ITypeElement[] ITypeElement.GetDescendants(ISourceTreeResolver resolver)
		{
			return null;
		}
		ITypeElement[] ITypeElement.GetDescendants(ISourceTreeResolver resolver, IElement scope)
		{
			return null;
		}
		bool ITypeElement.Is(string fullTypeName)
		{
			return false;
		}
		bool ITypeElement.Is(ITypeElement parentElement)
		{
			return false;
		}
		bool ITypeElement.Is(Type type)
		{
			return false;
		}
		bool ITypeElement.Is(ISourceTreeResolver resolver, string fullTypeName)
		{
			return false;
		}
		bool ITypeElement.Is(ISourceTreeResolver resolver, ITypeElement parentElement)
		{
			return false;
		}
		bool ITypeElement.Is(ISourceTreeResolver resolver, Type type)
		{
			return false;
		}
		bool ITypeElement.DescendsFrom(string fullTypeName)
		{
			return false;
		}
		bool ITypeElement.DescendsFrom(ITypeElement parentElement)
		{
			return false;
		}
		bool ITypeElement.DescendsFrom(Type type)
		{
			return false;
		}
		bool ITypeElement.DescendsFrom(ISourceTreeResolver resolver, string fullTypeName)
		{
			return false;
		}
		bool ITypeElement.DescendsFrom(ISourceTreeResolver resolver, ITypeElement parentElement)
		{
			return false;
		}
		bool ITypeElement.DescendsFrom(ISourceTreeResolver resolver, Type type)
		{
			return false;
		}
		IMemberElement ITypeElement.FindMember(string name)
		{
			return null;
		}
		IMemberElement ITypeElement.FindMember(string name, bool searchInAncestors)
		{
			return null;
		}
		IMemberElement ITypeElement.FindMember(string name, IElementFilter filter, bool searchInAncestors)
		{
			return null;
		}
		IMemberElementCollection ITypeElement.FindMembers(string name)
		{
			return null;
		}
	IMemberElementCollection ITypeElement.FindMembers(ISourceTreeResolver resolver, string name)
	{
	  return null;
	}
		IMemberElementCollection ITypeElement.FindMembers(string name, bool searchInAncestors)
		{
			return null;
		}
		IMemberElementCollection ITypeElement.FindMembers(string name, IElementFilter filter, bool searchInAncestors)
		{
			return null;
		}
		#endregion
		#region IMemberElement Members
		string IMemberElement.GetOverrideCode()
		{
			return String.Empty;
		}
		string IMemberElement.GetOverrideCode(bool callBase)
		{
			return String.Empty;
		}
		string IMemberElement.GetOverrideCode(bool callBase, string codeBefore, string codeAfter)
		{
			return String.Empty;
		}
		IExpression IMemberElement.NameQualifier
		{
	  get { return null; }
		}
		MemberVisibility IMemberElement.Visibility
		{
	  get { return MemberVisibility.Illegal; }
		}
		bool IMemberElement.IsDefaultVisibility
		{
	  get { return false; }
		}
		bool IMemberElement.IsAbstract
		{
	  get { return false; }
		}
		bool IMemberElement.IsVirtual
		{
	  get { return false; }
		}
		bool IMemberElement.IsOverride
		{
	  get { return false; }
		}
		bool IMemberElement.IsNew
		{
	  get { return false; }
		}
		bool IMemberElement.IsPartial
		{
	  get { return false; }
		}
	bool IMemberElement.IsReadOnly
	{
	  get { return false; }
	}
		bool IMemberElement.IsStatic
		{
	  get { return false; }
		}
		bool IMemberElement.IsSealed
		{
	  get { return false; }
		}
		bool IMemberElement.IsExplicitInterfaceMember
		{
	  get { return false; }
		}
		bool IMemberElement.IsExtern
		{
	  get { return false; }
		}
	bool IMemberElement.IsIterator
	{
	  get
	  {
		return false;
	  }
	}
		string IMemberElement.Signature
		{
	  get { return String.Empty; }
		}
		IExpressionCollection IMemberElement.Implements
		{
	  get { return null; }
		}
		bool IMemberElement.HasDelimitedBlock
		{
			get { return false; }
		}
		#endregion
		#region IHasAttributes Members
		IAttributeElementCollection IHasAttributes.Attributes
		{
	  get { return null; }
		}
		#endregion
		#region IGenericElement Members
		ITypeParameterCollection IGenericElement.TypeParameters
		{
	  get { return null; }
		}
		bool IGenericElement.IsGeneric
		{
	  get { return false; }
		}
		bool IGenericElement.IsActivatedGeneric
		{
	  get { return false; }
		}
		IGenericElement IGenericElement.GenericTemplate
		{
	  get { return null; }
		}
		#endregion
	}
}
