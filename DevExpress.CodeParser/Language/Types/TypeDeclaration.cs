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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class TypeDeclaration : AccessSpecifiedElement, ITypeElement, ITypeElementModifier
	{
		bool _IsPartial;
		TypeReferenceExpression _PrimaryAncestorType;
		TypeReferenceExpressionCollection _SecondaryAncestorTypes;
	bool _HasEndingSemicolon;
	bool _GenerateBlock = true;
		public TypeDeclaration()
		{
		}
	public TypeDeclaration(string name)
	{
	  InternalName = name;
	}
		bool CanHaveDeclarationInCurrentType(IElement element)
		{
			if (element == null || element is ITypeReferenceExpression)
				return false;
			return element is IWithSource && ((IWithSource)element).Source == null;				
		}
		protected virtual void SetPrimaryAncestorType(TypeReferenceExpression type)
		{
			ReplaceDetailNode(_PrimaryAncestorType, type);
			_PrimaryAncestorType = type;
		}
		protected virtual void SetSecondaryAncestorTypes(TypeReferenceExpressionCollection types)
		{
			ReplaceDetailNodes(SecondaryAncestorTypes, types);
			SecondaryAncestorTypes.Clear();
			SecondaryAncestorTypes.AddRange(types);
		}
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (oldElement == null)
		return;
	  if (_PrimaryAncestorType == oldElement)
	  {
		SetPrimaryAncestorType(newElement as TypeReferenceExpression);
		return;
	  }
	  if (_SecondaryAncestorTypes != null && _SecondaryAncestorTypes.Count != 0)
	  {
		int lElementIdx = SecondaryAncestorTypes.IndexOf(oldElement);
		if (lElementIdx >= 0)
		{
		  SecondaryAncestorTypes.RemoveAt(lElementIdx);
		  SecondaryAncestorTypes.Insert(lElementIdx, newElement);
		  ReplaceDetailNode(oldElement, newElement);
		  return;
		}
	  }
	  base.ReplaceOwnedReference(oldElement, newElement);
	}
		#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  TypeDeclaration typeDeclaration = source as TypeDeclaration;
	  if (typeDeclaration == null)
		return;
	  _IsPartial = typeDeclaration._IsPartial;
	  _HasEndingSemicolon = typeDeclaration._HasEndingSemicolon;
	  _GenerateBlock = typeDeclaration._GenerateBlock;
	  if (typeDeclaration._PrimaryAncestorType != null)
	  {
		_PrimaryAncestorType = ParserUtils.GetCloneFromNodes(this, typeDeclaration, typeDeclaration._PrimaryAncestorType) as TypeReferenceExpression;
		if (_PrimaryAncestorType == null)
		  _PrimaryAncestorType = typeDeclaration._PrimaryAncestorType.Clone(options) as TypeReferenceExpression;
	  }
	  if (typeDeclaration._SecondaryAncestorTypes == null)
		return;
	  _SecondaryAncestorTypes = new TypeReferenceExpressionCollection();
	  ParserUtils.GetClonesFromNodes(DetailNodes, typeDeclaration.DetailNodes, _SecondaryAncestorTypes, typeDeclaration._SecondaryAncestorTypes);
	  if (_SecondaryAncestorTypes.Count == 0 &&
		  typeDeclaration._SecondaryAncestorTypes.Count > 0)
		_SecondaryAncestorTypes = typeDeclaration._SecondaryAncestorTypes.DeepClone(options) as TypeReferenceExpressionCollection;
	}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypeDeclaration lClone = new TypeDeclaration();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public IMemberElement FindMember(string name)
		{
			return FindMember(name, true);
		}
		public IMemberElement FindMember(string name, bool searchInAncestors)
		{
	  return FindMember(name, ElementFilters.Member, searchInAncestors);
		}
		public IMemberElement FindMember(string name, IElementFilter filter, bool searchInAncestors)
		{
	  return StructuralParserServicesHolder.FindMember(this, name, filter, searchInAncestors);
		}
		public IMemberElementCollection FindMembers(string name)
		{
			return FindMembers(name, true);
		}
	public IMemberElementCollection FindMembers(ISourceTreeResolver resolver, string name)
	{
	  return FindMembers(resolver, name, ElementFilters.Member, true);
	}
		public IMemberElementCollection FindMembers(string name, bool searchInAncestors)
		{
	  return FindMembers(name, ElementFilters.Member, searchInAncestors);
		}
		public IMemberElementCollection FindMembers(string name, IElementFilter filter, bool searchInAncestors)
		{
	  return StructuralParserServicesHolder.FindMembers(this, name, filter, searchInAncestors);
		}
	public IMemberElementCollection FindMembers(ISourceTreeResolver resolver, string name, IElementFilter filter, bool searchInAncestors)
	{
	  return StructuralParserServicesHolder.FindMembers(resolver, this, name, filter, searchInAncestors);
	}
	public ITypeElement[] GetAllDescendants()
	{
	  return StructuralParserServicesHolder.GetAllDescendants(this);
	}
	public ITypeElement[] GetAllDescendants(IElement scope)
	{
	  return StructuralParserServicesHolder.GetAllDescendants(this, scope);
	}
	public ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver)
	{
	  return StructuralParserServicesHolder.GetAllDescendants(resolver, this);
	}
	public ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, IElement scope)
	{
	  return StructuralParserServicesHolder.GetAllDescendants(resolver, this, scope);
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
		public bool Is(string fullTypeName)
		{
			return ((ITypeElement)this).FullName == fullTypeName ||
				DescendsFrom(fullTypeName);
		}
		public bool Is(ITypeElement type)
		{
			if (type == null)
				return false;
			return Is(type.FullName);
		}
		public bool Is(Type type)
		{
			if (type == null)
				return false;
			return Is(type.FullName);
		}
		public bool Is(ISourceTreeResolver resolver, string fullTypeName)
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
		public override void CleanUpOwnedReferences()
		{
			_PrimaryAncestorType = null;
			if (_SecondaryAncestorTypes != null)
			{
				_SecondaryAncestorTypes.Clear();
				_SecondaryAncestorTypes = null;
			}
			base.CleanUpOwnedReferences();
		}
		public void AddSecondaryAncestorType(TypeReferenceExpression type)
		{
			if (type == null)
				return;
			SecondaryAncestorTypes.Add(type);
			AddDetailNode(type);
		}
		public void AddSecondaryAncestorTypes(TypeReferenceExpressionCollection types)
		{
			if (types == null)
				return;
			int lCount = types.Count;
			for (int i = 0; i < lCount; i++)
				AddSecondaryAncestorType(types[i]);
		}
		ITypeElement GetTypeElementFromCache(ITypeElement type)
		{
			if (type == null)
				return null;
			IProjectElement project = type.Project;
			if (project == null)
				return null;
			return project.FindElementByFullName(type.FullName) as ITypeElement;
		}
		bool TypeContainsDeclaration(ISourceTreeResolver resolver, ITypeElement type, IMemberElement declaration, SearchScope scope)
		{
	  ITypeElement declarationParent = declaration.Parent as ITypeElement;
	  if (declarationParent == null)
		return false;
			if (StructuralParserServicesHolder.DeclarationsMatch(resolver, type, declarationParent))
				return true;
			if (scope != SearchScope.AllPartialClasses)
				return false;
			ITypeElement typeElement = GetTypeElementFromCache(type);
			if (typeElement == null)
				return false;
	  ITypeElement declarationParentElement = GetTypeElementFromCache(declarationParent);
	  if (declarationParentElement == null)
		return false;
	  return StructuralParserServicesHolder.DeclarationsMatch(resolver, typeElement, declarationParentElement);
		}
		public bool Declares(IElement element)
		{
			return Declares(element, false);
		}
		public bool Declares(IElement element, bool searchBaseClasses)
		{
			return Declares(element, SearchScope.AllPartialClasses, searchBaseClasses);
		}
		public bool Declares(IElement element, SearchScope scope, bool searchBaseClasses)
		{
			if (element == null)
				return false;
	  IMemberElement declaration = StructuralParserServicesHolder.GetDeclaration(element) as IMemberElement;
			if (declaration == null)
				return false;
	  ISourceTreeResolver resolver = StructuralParserServicesHolder.SourceTreeResolver;
			if (TypeContainsDeclaration(resolver, this, declaration, scope))
				return true;			
			if (!searchBaseClasses || 
				(declaration.Visibility != MemberVisibility.Protected && 
				declaration.Visibility != MemberVisibility.ProtectedInternal &&
				declaration.Visibility != MemberVisibility.Public))
				return false;
			ITypeElement typeElement = GetTypeElementFromCache(this);
			if (typeElement == null)
				typeElement = this;
			ITypeElement[] baseTypes = StructuralParserServicesHolder.GetAllBaseTypes(typeElement);
			if (baseTypes == null)
				return false;
			int count = baseTypes.Length;
			for (int i = 0; i < count; i++)
				if (TypeContainsDeclaration(resolver, baseTypes[i], declaration, scope))
					return true;
			return false;
		}
	public bool HasDefaultConstructor
	{
	  get
	  {
		IEnumerable allMethod = AllMethods;
		foreach (IElement element in allMethod)
		{
		  IMethodElement methodElement = element as IMethodElement;
		  if (methodElement == null)
			continue;
		  if (methodElement.MethodType == MethodTypeEnum.Constructor)
			if (methodElement.Visibility != MemberVisibility.Private)
			  if (methodElement.Parameters.Count == 0)
				return true;
		}
		return false;
	  }
	}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.TypeDeclaration;
			}
		}
		public TypeReferenceExpression PrimaryAncestorType
		{
			get
			{
				return _PrimaryAncestorType;
			}
			set
			{
				SetPrimaryAncestorType(value);
			}
		}
		public TypeReferenceExpressionCollection SecondaryAncestorTypes
		{
			get
			{
				if (_SecondaryAncestorTypes == null)
					_SecondaryAncestorTypes = new TypeReferenceExpressionCollection();
				return _SecondaryAncestorTypes;					
			}
			set
			{
				SetSecondaryAncestorTypes(value);
			}
		}		
		public override bool IsNewContext
		{
			get 
			{
				return true;
			}
		}
		public string FullName
		{
			get
			{
				IProjectElement project = Project;
				if (project != null && project.SupportsRootNamespace)
					return RootNamespaceLocation;
				else
					return Location;
			}
		}
		public IEnumerable AllFields
		{
			get
			{
				return new ElementEnumerable(this, new FieldFilter());
			}
		}
		public IEnumerable AllConstants
		{
			get
			{
				return new ElementEnumerable(this, typeof(Const));
			}
		}
		public IEnumerable AllProperties
		{
			get
			{
				return new ElementEnumerable(this, typeof(Property));
			}
		}
		public IEnumerable AllMethods
		{
			get
			{
				return new ElementEnumerable(this, typeof(Method));
			}
		}
		public IEnumerable AllEvents
		{
			get
			{
				return new ElementEnumerable(this, typeof(Event));
			}
		}
		public IEnumerable AllTypes
		{
			get
			{
				return new ElementEnumerable(this, typeof(TypeDeclaration), true);
			}
		}
		public IEnumerable AllChildTypes
		{
			get
			{
				return new ElementEnumerable(this, typeof(TypeDeclaration));
			}
		}
		public IEnumerable AllMembers
		{
			get
			{
				return new ElementEnumerable(this, typeof(Member));
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
	ITypeReferenceExpression ITypeElement.PrimaryAncestor
		{
			get
			{
				return PrimaryAncestorType;
			}
		}
		ITypeReferenceExpressionCollection ITypeElement.SecondaryAncestors
		{
			get
			{
				if (_SecondaryAncestorTypes == null)
					return EmptyLiteElements.EmptyITypeReferenceExpressionCollection;
				return _SecondaryAncestorTypes;
			}
		}
		IMemberElementCollection ITypeElement.Members
		{
			get
			{
				if (Nodes == null)
					return EmptyLiteElements.EmptyIMemberElementCollection;
				LiteMemberElementCollection lMembers = new LiteMemberElementCollection();
				int count = NodeCount;
				for (int i = 0; i < count; i++)
				{
					object element = Nodes[i];
					if (element is IMemberElement)
						lMembers.Add(element);
				}
				return lMembers;
			}
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool IsTypeDeclaration
		{
			get
			{
				return true;
			}
		}
	public bool HasEndingSemicolon
	{
	  get
	  {
		return _HasEndingSemicolon;
	  }
	  set
	  {
		_HasEndingSemicolon = value;
	  }
	}
	public bool GenerateBlock
	{
	  get
	  {
		return _GenerateBlock;
	  }
	  set
	  {
		_GenerateBlock = value;
	  }
	}
	#region ITypeElementModifier Members
	void ITypeElementModifier.SetPrimaryAncestor(ITypeReferenceExpression type)
	{
	  SetPrimaryAncestorType(type as TypeReferenceExpression);
	}
	void ITypeElementModifier.AddSecondaryAncestor(ITypeReferenceExpression type)
	{
	  AddSecondaryAncestorType(type as TypeReferenceExpression);
	}
	void ITypeElementModifier.AddMember(IMemberElement member)
	{
	  AddNode(member as LanguageElement);
	}
	void ITypeElementModifier.RemoveMember(IMemberElement member)
	{
	  RemoveNode(member as LanguageElement);
	}
	#endregion
  }
}
