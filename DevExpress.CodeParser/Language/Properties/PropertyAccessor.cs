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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class PropertyAccessor : Accessor, IMethodElement, IHasParameters, IMethodElementModifier
  {
	TypeReferenceExpression _AccessorType;
	bool _IsPrototype = false;
	#region PropertyAccessor
	public PropertyAccessor()
	{
	}
	#endregion
	#region GetAccessorName
	protected override string GetAccessorName()
	{
	  if (Parent is Property)
		return Parent.Name;
	  return String.Empty;
	}
	#endregion
	#region GetCyclomaticComplexity
	public override int GetCyclomaticComplexity()
	{
	  return 1 + GetChildCyclomaticComplexity();
	}
	#endregion
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  if ((Parent is Property) && ((Property)Parent).IsStatic)
		return ImageIndex.StaticMethod;
	  return ImageIndex.Method;
	}
	#endregion
	public void AddParameter(Param parameter)
	{
	  if (parameter == null)
		return;
	  Parameters.Add(parameter);
	  AddDetailNode(parameter);
	}
	public void AddParameters(LanguageElementCollection parameters)
	{
	  if (parameters == null)
		return;
	  foreach (LanguageElement parameter in parameters)
		AddParameter(parameter as Param);
	}
	public void RemoveParameter(Param parameter)
	{
	  if (parameter == null)
		return;
	  Parameters.Remove(parameter);
	  RemoveDetailNode(parameter);
	}
	public void RemoveParameters(LanguageElementCollection parameters)
	{
	  if (parameters == null)
		return;
	  foreach (LanguageElement parameter in parameters)
		RemoveParameter(parameter as Param);
	}
	public void InsertParameter(int index, Param parameter)
	{
	  if (parameter == null)
		return;
	  Parameters.Insert(index, parameter);
	  InsertDetailNode(index, parameter);
	}
	public Property ParentProperty
	{
	  get
	  {
		return (Parent as Property);
	  }
	}
	public TypeReferenceExpression AcessorType
	{
	  get
	  {
		return _AccessorType;
	  }
	  set
	  {
		_AccessorType = value;
	  }
	}
	public override LanguageElementType ElementType
	{
	  get 
	  {
		return LanguageElementType.PropertyAccessor;
	  }
	}
	public bool IsPrototype
	{
	  get
	  {
		return _IsPrototype;
	  }
	  set
	  {
		_IsPrototype = value;
	  }
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is PropertyAccessor))
		return;			
	  PropertyAccessor lSource = (PropertyAccessor)source;
	  _AccessorType = lSource.AcessorType;
	  _IsPrototype = lSource.IsPrototype;
	}
	#region Type
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected virtual ITypeReferenceExpression Type
	{
	  get
	  {
		return null;
	  }
	}
	#endregion
	#region PropertyName
	[Description("The name of the property that contains this accessor.")]
	[Category("Family")]
		public string PropertyName
		{
			get
			{
				return GetAccessorName();
			}
		}
	#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]		
		public virtual MethodTypeEnum MethodType
		{
			get
			{
				return MethodTypeEnum.Void;
			}
		}
	#region GetValidVisibilities
	public override MemberVisibility[] GetValidVisibilities()
	{
	  if (ParentProperty != null)
	  {
		MemberVisibility parentVisibility = ParentProperty.Visibility;
		if (parentVisibility == MemberVisibility.Illegal)
		  parentVisibility = ParentProperty.GetDefaultVisibility();
		List<MemberVisibility> result;
		switch (parentVisibility)
		{
		  case MemberVisibility.Private:
			return new MemberVisibility[] { };
		  case MemberVisibility.Protected:
		  case MemberVisibility.Internal:
			result = new List<MemberVisibility>(new MemberVisibility[] { MemberVisibility.Private });
			break;
		  case MemberVisibility.ProtectedInternal:
			result = new List<MemberVisibility>(new MemberVisibility[] { MemberVisibility.Private, MemberVisibility.Protected, MemberVisibility.Internal });
			break;
		  case MemberVisibility.Public:
			result = new List<MemberVisibility>(new MemberVisibility[] { MemberVisibility.Private, MemberVisibility.Protected, MemberVisibility.Internal, MemberVisibility.ProtectedInternal });
			break;
		  default:
			return new MemberVisibility[] { };
		}
		if (ParentProperty.IsAbstract || ParentProperty.IsVirtual || ParentProperty.IsOverride)
		  result.Remove(MemberVisibility.Private);
		return result.ToArray();
	  }
	  else
	  {
		return base.GetValidVisibilities();
	  }
	}
	#endregion
		#region IMethodElement Members
	bool IMethodElement.IsAsynchronous
	{
	  get
	  {
		return false;
	  }
	}
		bool IMethodElement.IsExtensionMethod()
		{
			return false;
		}
	bool IMethodElement.IsExtensionMethod(ISourceTreeResolver resolver)
	{
	  return false;
	}
		bool IMethodElement.IsMainProcedure()
		{
			return false;
		}
   bool IMethodElement.IsInitializeComponent()
	{
	  return false;
	}
		string IMethodElement.Lib
		{
			get
			{
				return null;
			}
		}
		string IMethodElement.Alias
		{
			get
			{
				return null;
			}
		}
		bool IMethodElement.IsConstructor
		{
			get
			{
				return false;
			}
		}
	bool IMethodElement.IsSerializationConstructor
	{
	  get
	  {
		return false;
	  }
	}
		bool IMethodElement.IsDestructor
		{
			get
			{
				return false;
			}
		}
		bool IMethodElement.IsWebMethod
		{
			get
			{
				return false;
			}
		}
		bool IMethodElement.IsTypeInitializer
		{
			get
			{
				return false;
			}
		}
		IBaseVariable IMethodElement.ImplicitVariable
		{
			get
			{
				return ImplicitVariable;
			}
		}
		bool IMethodElement.IsMemberFunctionConst
		{
			get
			{
				return false;
			}			
		}
		IExpressionCollection IMethodElement.HandlesExpressions
		{
			get
			{
				return null;
			}
		}
		IExpressionCollection IMethodElement.ImplementsExpressions
		{
			get
			{
				return null;
			}
		}
		bool IMethodElement.IsClassOperator
		{
			get { return false; }
		}
		bool IMethodElement.IsImplicitCast
		{
			get { return false; }
		}
		bool IMethodElement.IsExplicitCast
		{
			get { return false; }
		}
		#endregion
		#region IMemberElement Members
	bool IMemberElement.IsIterator
	{
	  get
	  {
		return false;
	  }
	}
		bool IMemberElement.IsDefaultVisibility
		{
			get
			{
				return false;
			}
		}
		string IMemberElement.GetOverrideCode()
		{
			return String.Empty;
		}
		string IMemberElement.GetOverrideCode(bool callBase)
		{
			return String.Empty;
		}
		string IMemberElement.GetOverrideCode(bool callBase, string beforeCode, string afterCode)
		{
			return String.Empty;
		}
		IAttributeElementCollection IHasAttributes.Attributes
		{
			get
			{
				if (Attributes == null)
					return EmptyLiteElements.EmptyIAttributeElementCollection;
				LiteAttributeElementCollection lAttribures = new LiteAttributeElementCollection();
				lAttribures.AddRange(Attributes);
				return lAttribures;
			}
		}		
		MemberVisibility IMemberElement.Visibility
		{
			get
			{
				return Visibility;
			}			
		}
		bool IMemberElement.IsAbstract
		{
			get
			{
				return ParentProperty == null ? false : ParentProperty.IsAbstract;
			}			
		}
		bool IMemberElement.IsVirtual
		{
			get
			{
				return ParentProperty == null ? false : ParentProperty.IsVirtual;
			}			
		}
		bool IMemberElement.IsOverride
		{
			get
			{
				return ParentProperty == null ? false : ParentProperty.IsOverride;
			}			
		}
		bool IMemberElement.IsNew
		{
			get
			{
				return ParentProperty == null ? false : ParentProperty.IsNew;
			}			
		}
		bool IMemberElement.IsPartial
		{
			get
			{
				return ParentProperty == null ? false : ParentProperty.IsPartial;
			}
		}
		bool IMemberElement.IsStatic
		{
			get
			{
				return ParentProperty == null ? false : ParentProperty.IsStatic;
			}			
		}
		bool IMemberElement.IsSealed
		{
			get
			{
				return ParentProperty == null ? false : ParentProperty.IsSealed;
			}			
		}
		bool IMemberElement.IsExplicitInterfaceMember
		{
			get
			{
				return ParentProperty == null ? false : ParentProperty.IsExplicitInterfaceMember;
			}			
		}
		IExpression IMemberElement.NameQualifier
		{
			get
			{
				return null;
			}
		}
		string IMemberElement.Signature
		{
			get
			{
				return String.Empty;
			}
		}
		IExpressionCollection IMemberElement.Implements
		{
			get
			{
				return null;
			}
		}
		#endregion		
		#region IGenericElement Members
		bool IGenericElement.IsGeneric
		{
			get
			{
				return false;
			}
		}
		ITypeParameterCollection IGenericElement.TypeParameters
		{
			get
			{
				return EmptyLiteElements.EmptyITypeParameterCollection;
			}
		}
		IGenericElement IGenericElement.GenericTemplate
		{
			get
			{
				return null;
			}
		}
		bool IGenericElement.IsActivatedGeneric
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region IHasType Members
		ITypeReferenceExpression IHasType.Type
		{
			get
			{
		return Type;
			}
		}
		#endregion
		#region IWithParameters Members
		IParameterElementCollection IWithParameters.Parameters
		{
			get
			{
		int count = DetailNodeCount;
		if (count == 0)
		  return EmptyLiteElements.EmptyIParameterElementCollection;
		LiteParameterElementCollection result = new LiteParameterElementCollection();		
		for (int i = 0; i < count; i++)
		  if (DetailNodes[i] is IParameterElement)
			result.Add(DetailNodes[i]);
		return result;
			}
		}
		#endregion
	#region IHasParameters Members
	public LanguageElementCollection Parameters
	{
	  get
	  {
		LanguageElementCollection result = new LanguageElementCollection();
		int count = DetailNodeCount;
		for (int i = 0; i < count; i++)
		  if (DetailNodes[i] is Param)
			result.Add(DetailNodes[i]);
		return result;
	  }
	}
	public int ParameterCount
	{
	  get
	  {
		LanguageElementCollection parameters = Parameters;
		return parameters != null ? parameters.Count : 0;		
	  }
	}
	#endregion
	#region IWithParametersModifier
	void IWithParametersModifier.AddParameter(IParameterElement parameter)
	{
	  AddParameter(parameter as Param);
	}
	void IWithParametersModifier.RemoveParameter(IParameterElement parameter)
	{
	  RemoveParameter(parameter as Param);
	}
	void IWithParametersModifier.InsertParameter(int index, IParameterElement parameter)
	{
	  InsertParameter(index, parameter as Param);
	}
	#endregion
	#region IGenericElementModifier
	void IGenericElementModifier.AddTypeParameter(ITypeParameter typeParameter)
	{
	}
	void IGenericElementModifier.InsertTypeParameter(int index, ITypeParameter typeParameter)
	{
	}
	void IGenericElementModifier.RemoveTypeParameter(ITypeParameter typeParameter)
	{
	}
	#endregion
	#region IMethodElementModifier Members
	void IMethodElementModifier.SetIsClassOperator(bool isClassOperator)
	{
	}
	void IMethodElementModifier.SetIsExplicitCast(bool isExplicitCast)
	{
	}
	void IMethodElementModifier.SetIsImplicitCast(bool isImplicitCast)
	{
	}
	#endregion
  }
}
