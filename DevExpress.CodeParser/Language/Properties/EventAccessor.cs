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
using System.Collections;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class EventAccessor : Accessor, IHasParameters, IMethodElement, IMethodElementModifier
	{
		LanguageElementCollection _ParametersList;
		#region EventAccessor
		public EventAccessor()
		{
		}
		#endregion
		LanguageElementCollection ParametersList
		{
			get
			{
				if (_ParametersList == null)
					_ParametersList = new LanguageElementCollection();
				return _ParametersList;
			}
		}
		#region GetAccessorName
		protected override string GetAccessorName()
		{
			if (Parent is Event)
				return Parent.Name;
			return String.Empty;
		}
		#endregion		
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is EventAccessor))
				return;			
			EventAccessor lSource = (EventAccessor)source;
			_ParametersList = null;
			if (lSource._ParametersList != null)
			{
				_ParametersList = new LanguageElementCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _ParametersList, lSource._ParametersList);
				if (_ParametersList.Count == 0 && lSource._ParametersList.Count > 0)
					_ParametersList = lSource._ParametersList.DeepClone(options);
			}				
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			if ((Parent is Event) && ((Event)Parent).IsStatic)
				return ImageIndex.StaticMethod;
			return ImageIndex.Method;
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			if (_ParametersList != null)
				_ParametersList.Clear();
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			int lIndex = -1;
			if (_ParametersList != null)
				lIndex = _ParametersList.IndexOf(oldElement);
			if (lIndex >= 0)
			{
				_ParametersList.RemoveAt(lIndex);
				if (newElement != null)
					_ParametersList.Insert(lIndex, newElement);
			}
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
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
		public Event ParentEvent
		{
			get
			{
				return (Parent as Event);
			}
		}
		[Description("The name of the event that contains this accessor.")]
		[Category("Family")]
		public string EventName
		{
			get
			{
				return GetAccessorName();
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.EventAccessor;
			}
		}
		public LanguageElementCollection Parameters
		{
			get
			{
		return ParametersList;
			}
			set
			{
				_ParametersList = value;
			}
		}
		public int ParameterCount
		{
			get
			{
				if (_ParametersList == null)
					return 0;
				return _ParametersList.Count;
			}
		}
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
		bool IMethodElement.IsWebMethod
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
		bool IMethodElement.IsTypeInitializer
		{
			get
			{
				return false;
			}
		}
		MethodTypeEnum IMethodElement.MethodType
		{
			get
			{
				return MethodTypeEnum.Void;
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
		bool IMemberElement.IsDefaultVisibility
		{
			get
			{
				return false;
			}
		}
		bool IMemberElement.IsAbstract
		{
			get
			{
				return ParentEvent == null ? false : ParentEvent.IsAbstract;
			}			
		}
	bool IMemberElement.IsIterator
	{
	  get
	  {
		return false;
	  }
	}
		bool IMemberElement.IsVirtual
		{
			get
			{
				return ParentEvent == null ? false : ParentEvent.IsVirtual;
			}			
		}
		bool IMemberElement.IsOverride
		{
			get
			{
				return ParentEvent == null ? false : ParentEvent.IsOverride;
			}			
		}
		bool IMemberElement.IsNew
		{
			get
			{
				return ParentEvent == null ? false : ParentEvent.IsOverride;
			}
		}
		bool IMemberElement.IsPartial
		{
			get
			{
				return ParentEvent == null ? false : ParentEvent.IsPartial;
			}
		}
		bool IMemberElement.IsStatic
		{
			get
			{
				return ParentEvent == null ? false : ParentEvent.IsStatic;
			}			
		}
		bool IMemberElement.IsSealed
		{
			get
			{
				return ParentEvent == null ? false : ParentEvent.IsSealed;
			}			
		}
		bool IMemberElement.IsExplicitInterfaceMember
		{
			get
			{
				return ParentEvent == null ? false : ParentEvent.IsExplicitInterfaceMember;
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
				return ParentEvent == null ? null : ParentEvent.MemberTypeReference;
			}
		}
		#endregion
		#region IWithParameters Members
		IParameterElementCollection IWithParameters.Parameters
		{
			get
			{
				if (_ParametersList == null)
					return EmptyLiteElements.EmptyIParameterElementCollection;
				LiteParameterElementCollection lParameters = new LiteParameterElementCollection();
				lParameters.AddRange(_ParametersList);
				return lParameters;
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
