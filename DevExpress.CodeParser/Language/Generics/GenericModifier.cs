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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class GenericModifier : CodeElement
	{
		TypeParameterCollection _TypeParameters;
		public GenericModifier()
	  :this(new TypeParameterCollection())
		{
		}
		public GenericModifier(TypeParameterCollection types)
			: this(types, SourceRange.Empty)
		{
		}
		public GenericModifier(TypeParameterCollection types, SourceRange range)
		{
			_TypeParameters = types;
			AddDetailNodes(_TypeParameters);
			SetRange(range);
		}
		private void GetTypeParametersFromDetailNodes()
		{
			_TypeParameters = new TypeParameterCollection();
			for (int i = 0; i < DetailNodeCount; i++)
			{
				TypeParameter lParam = DetailNodes[i] as TypeParameter;
				if (lParam == null)
					continue;
				_TypeParameters.Add(lParam);
			}
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is GenericModifier))
				return;			
			GetTypeParametersFromDetailNodes();			
		}
		#endregion
		public override void CleanUpOwnedReferences()
		{
			_TypeParameters = null;
			base.CleanUpOwnedReferences();
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_TypeParameters != null && _TypeParameters.Contains(oldElement))
				_TypeParameters.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			GenericModifier lClone = new GenericModifier();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	public void AddTypeParameter(TypeParameter typeParameter)
	{
	  if (typeParameter == null)
		return;
	  TypeParameters.Add(typeParameter);
	  AddDetailNode(typeParameter);
	}
	public void InsertTypeParameter(int index, TypeParameter typeParameter)
	{
	  if (typeParameter == null)
		return;
	  TypeParameters.Insert(index, typeParameter);
	  InsertDetailNode(index, typeParameter);
	}
	public void RemoveTypeParameter(TypeParameter typeParameter)
	{
	  if (typeParameter == null)
		return;
	  TypeParameters.Remove(typeParameter);
	  RemoveDetailNode(typeParameter);
	}
	public override string ToString()
		{
			StringBuilder lBuilder = new StringBuilder();
			lBuilder.Append("<");
			if (_TypeParameters != null)
				lBuilder.Append(_TypeParameters.ToString());
			lBuilder.Append(">");
			return lBuilder.ToString();
		}
		public TypeParameterCollection TypeParameters
		{
			get
			{
				return _TypeParameters;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.GenericModifier;
			}
		}
		protected override bool HasOuterRangeChildren
		{
			get
			{
				return true;
			}
		}
	}
	public class ComplexGenericModifier : GenericModifier
	{
		LanguageElementCollection _GenericModifiers;
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ComplexGenericModifier))
				return;			
			ComplexGenericModifier lSource = source as ComplexGenericModifier;
			if (lSource._GenericModifiers != null)
				_GenericModifiers = lSource._GenericModifiers.DeepClone() as LanguageElementCollection;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ComplexGenericModifier;
			}
		}
		public LanguageElementCollection GenericModifiers
		{
			get
			{
				if (_GenericModifiers == null)
					_GenericModifiers = new LanguageElementCollection();
				return _GenericModifiers;
			}
		}
		public int GenericModifiersCount
		{
			get
			{
				if (_GenericModifiers == null)
					return 0;
				else
					return _GenericModifiers.Count;
			}
		}
	}
}
