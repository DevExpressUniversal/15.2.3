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
	public class DelegateDefinition : MemberWithParameters, IDelegateElement, ITypeElementModifier, IWithParametersModifier
	{
	IElementCollection _DelegateMethods = null;
	  #region DelegateDefinition
	  public DelegateDefinition()
	  {
	  }
	  #endregion
		#region DelegateDefinition(string name)
		public DelegateDefinition(string name): this()
		{
			InternalName = name;			
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			DelegateDefinition lClone = new DelegateDefinition();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Delegate;
		}
		#endregion
		#region GetDefaultVisibility
		public override MemberVisibility GetDefaultVisibility()
		{
			return MemberVisibility.Local;
		}
		#endregion		
	public ITypeElement[] GetAllDescendants()
	{
	  return StructuralParserServicesHolder.GetDescendants(this);
	}
	public ITypeElement[] GetAllDescendants(IElement scope)
	{
	  return StructuralParserServicesHolder.GetDescendants(this, scope);
	}
	public ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver)
	{
	  return StructuralParserServicesHolder.GetDescendants(resolver, this);
	}
	public ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, IElement scope)
	{
	  return StructuralParserServicesHolder.GetDescendants(resolver, this, scope);
	}
		public ITypeElement GetBaseType()
		{
			return GetBaseType(StructuralParserServicesHolder.SourceTreeResolver);
		}
		public ITypeElement GetBaseType(ISourceTreeResolver resolver)
		{
			ITypeElement[] baseTypes = GetBaseTypes(resolver);
			if (baseTypes != null && baseTypes.Length > 0)
				return baseTypes[0];
			return null;
		}
		public ITypeElement[] GetBaseTypes()
		{
	  return GetBaseTypes(StructuralParserServicesHolder.SourceTreeResolver);
		}
		public ITypeElement[] GetBaseTypes(ISourceTreeResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			return resolver.ResolveBaseTypes(this);
		}
		public ITypeElement[] GetDescendants()
		{
	  return StructuralParserServicesHolder.GetDescendants(this);
		}
		public ITypeElement[] GetDescendants(IElement scope)
		{
	  return StructuralParserServicesHolder.GetDescendants(this, scope);
		}
		public ITypeElement[] GetDescendants(ISourceTreeResolver resolver)
		{
	  return StructuralParserServicesHolder.GetDescendants(resolver, this);
		}
		public ITypeElement[] GetDescendants(ISourceTreeResolver resolver, IElement scope)
		{
	  return StructuralParserServicesHolder.GetDescendants(resolver, this, scope);
		}
		public override bool Is(string fullTypeName)
		{
			return ((ITypeElement)this).FullName == fullTypeName ||
				DescendsFrom(fullTypeName);
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
			return ((ITypeElement)this).FullName == fullTypeName ||
				DescendsFrom(resolver, fullTypeName);
		}
		public bool Is(ISourceTreeResolver resolver, ITypeElement type)
		{
			if (type == null)
				return false;
			return Is(resolver, type.FullName);
		}
		public bool Is(ISourceTreeResolver resolver, Type type)
		{
			if (type == null)
				return false;
			return Is(resolver, type.FullName);
		}
		public bool DescendsFrom(string fullTypeName)
		{
	  return StructuralParserServicesHolder.DescendsFrom(this, fullTypeName);
		}
		public bool DescendsFrom(ITypeElement type)
		{
			if (type == null)
				return false;
			return DescendsFrom(type.FullName);
		}
		public bool DescendsFrom(Type type)
		{
			if (type == null)
				return false;
			return DescendsFrom(type.FullName);
		}
		public bool DescendsFrom(ISourceTreeResolver resolver, string fullTypeName)
		{
	  return StructuralParserServicesHolder.DescendsFrom(resolver, this, fullTypeName);
		}
		public bool DescendsFrom(ISourceTreeResolver resolver, ITypeElement type)
		{
			if (type == null)
				return false;
			return DescendsFrom(resolver, type.FullName);
		}
		public bool DescendsFrom(ISourceTreeResolver resolver, Type type)
		{
			if (type == null)
				return false;
			return DescendsFrom(resolver, type.FullName);
		}
	private TypeReferenceExpression CreateTypeReference(string name)
	{
	  TypeReferenceExpression typeRef = new TypeReferenceExpression(name);
	  return typeRef;
	}
	private void AddDelegateParameters(LanguageElement parent, LanguageElementCollection parameters)
	{
	  for (int i = 0; i < this.ParameterCount; i++)
	  {
		LanguageElement parameter = this.Parameters[i].Clone() as LanguageElement;
		if (parameter != null)
		{
		  parameters.Add(parameter);
		  parent.AddDetailNode(parameter);
		}
	  }
	}
	private void AddParameter(LanguageElement parent, LanguageElementCollection parameters, string type, string name)
	{
	  Param param = new Param(type, name);
	  param.MemberTypeReference = CreateTypeReference(type);
	  param.AddDetailNode(param.MemberTypeReference);
	  parameters.Add(param);
	  parent.AddDetailNode(param);
	}
	private MethodPrototype GetMethodPrototype(TypeReferenceExpression returnType, string name, MethodTypeEnum methodType)
	{
	  if (returnType == null)
		return null;
	  MethodPrototype result = new MethodPrototype(returnType.FullSignature, name);
	  result.MethodType = methodType;
	  result.MemberTypeReference = returnType;
	  result.AddDetailNode(returnType);
	  result.SetParent(this);
	  return result;
	}
	private MethodPrototype GetInvokeMethod()
	{
	  MethodPrototype result = GetMethodPrototype(CreateTypeReference("void"), "Invoke", MethodTypeEnum.Void);
			if (MemberTypeReference != null && !string.IsNullOrEmpty(MemberTypeReference.FullSignature))
				result = GetMethodPrototype(MemberTypeReference.Clone() as TypeReferenceExpression, "Invoke", MethodTypeEnum.Function);
			else
				if (!string.IsNullOrEmpty(MemberType))
					result = GetMethodPrototype(CreateTypeReference(MemberType), "Invoke", MethodTypeEnum.Function);
	  result.SetParent(this);
	  AddDelegateParameters(result, result.Parameters);
	  return result;
	}
	private MethodPrototype GetBeginInvokeMethod()
	{
	  MethodPrototype result = GetMethodPrototype(CreateTypeReference("IAsyncResult"), "BeginInvoke", MethodTypeEnum.Function);
	  result.SetParent(this);
	  AddDelegateParameters(result, result.Parameters);
	  AddParameter(result, result.Parameters, "AsyncCallback", "callback");
	  AddParameter(result, result.Parameters, "object", "@object");
	  return result;
	}
	private MethodPrototype GetEndInvokeMethod()
	{
	  MethodPrototype result = GetMethodPrototype(CreateTypeReference("void"), "EndInvoke", MethodTypeEnum.Function);
			if (MemberTypeReference != null && !string.IsNullOrEmpty(MemberTypeReference.FullSignature))
				result = GetMethodPrototype(MemberTypeReference.Clone() as TypeReferenceExpression, "EndInvoke", MethodTypeEnum.Function);
			else
				if (!string.IsNullOrEmpty(MemberType))
					result = GetMethodPrototype(CreateTypeReference(MemberType), "EndInvoke", MethodTypeEnum.Function);
			result.SetParent(this);
			AddOutAndRefParameters(result, Parameters);
	  AddParameter(result, result.Parameters, "IAsyncResult", "result");
	  return result;
	}
		private void AddOutAndRefParameters(MethodPrototype result, LanguageElementCollection parameters)
		{
			if (result == null || parameters == null)
				return;
			foreach (Param param in parameters)
				if (param.Direction == ArgumentDirection.Out || param.Direction == ArgumentDirection.Ref)
				{
					Param cloned = param.Clone() as Param;
					result.Parameters.Add(cloned);
					result.AddDetailNode(cloned);
				}
		}
	private IElementCollection GetDelegateMethods()
	{
	  IElementCollection result = new IElementCollection();
	  result.Add(GetInvokeMethod());
	  result.Add(GetBeginInvokeMethod());
	  result.Add(GetEndInvokeMethod());
	  return result;
	}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Delegate;
			}
		}
	public IElementCollection DelegateMethods
	{
	  get
	  {
		if (_DelegateMethods == null)
		  _DelegateMethods = GetDelegateMethods();
		return _DelegateMethods;
	  }
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool IsTypeDeclaration
		{
			get
			{
				return true;
			}
		}
	#region ITypeElement Members
	bool ITypeElement.IsTypeParameter
	{ 
	  get
	  {
		return false;
	  }
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
		ITypeReferenceExpression ITypeElement.PrimaryAncestor
		{
			get
			{
				return null;
			}
		}
		ITypeReferenceExpressionCollection ITypeElement.SecondaryAncestors
		{
			get
			{
				return EmptyLiteElements.EmptyITypeReferenceExpressionCollection;
			}
		}
		IMemberElementCollection ITypeElement.Members
		{
			get
			{
				return EmptyLiteElements.EmptyIMemberElementCollection;
			}
		}
		#endregion
	#region ITypeElementModifier Members
	void ITypeElementModifier.AddMember(IMemberElement member)
	{
	}
	void ITypeElementModifier.RemoveMember(IMemberElement member)
	{
	}
	void ITypeElementModifier.AddSecondaryAncestor(ITypeReferenceExpression type)
	{
	}
	void ITypeElementModifier.SetPrimaryAncestor(ITypeReferenceExpression type)
	{
	}
	#endregion
  }
}
