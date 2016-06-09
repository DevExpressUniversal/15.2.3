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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[Serializable]
  public class FunctionPointerTypeReference : TypeReferenceExpression, IFunctionPointerTypeReference
  {
	private AccessSpecifiers _AccessSpecifiers;	
	TypeReferenceExpression _ReturnType;
	LanguageElementCollection _Parameters;
	CallingConventionSpecifier _CallingConvention;
	#region FunctionPointerTypeReference
	protected FunctionPointerTypeReference()
	  : base()
	{
	  _CallingConvention = CallingConventionSpecifier.Default;
	  _AccessSpecifiers = new AccessSpecifiers();
	}
	#endregion
	#region FunctionPointerTypeReference
	public FunctionPointerTypeReference(TypeReferenceExpression returnType, LanguageElementCollection parameters, CallingConventionSpecifier callingConvention)
	  : this()
	{
	  SetReturnType(returnType);
	  SetParameters(parameters);
	  _CallingConvention = callingConvention;
	}
	#endregion
	void SetReturnType(TypeReferenceExpression returnType)
	{
	  if (_ReturnType != null)
		RemoveDetailNode(_ReturnType);
	  _ReturnType = returnType;
	  if (_ReturnType != null)
		AddDetailNode(_ReturnType);
	  if (_ReturnType != null)
		Name = _ReturnType.Name;
	}
	void SetParameters(LanguageElementCollection parameters)
	{
	  if (_Parameters != null)
		RemoveDetailNodes(_Parameters);
	  _Parameters = parameters;
	  if (_Parameters != null)
		AddDetailNodes(_Parameters);
	}
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  base.CloneDataFrom(source, options);
	  if (!(source is FunctionPointerTypeReference))
		return;
	  FunctionPointerTypeReference functionSource = (FunctionPointerTypeReference)source;
	  if (functionSource._ReturnType != null)
	  {
		_ReturnType = ParserUtils.GetCloneFromNodes(this, functionSource, functionSource._ReturnType) as TypeReferenceExpression;
		if (_ReturnType == null)
		  _ReturnType = functionSource._ReturnType.Clone(options) as TypeReferenceExpression;
	  }
	  if (functionSource._Parameters != null)
	  {
		_Parameters = new LanguageElementCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, functionSource.DetailNodes, _Parameters, functionSource._Parameters);
		if (_Parameters.Count == 0 && functionSource._Parameters.Count > 0)
		  _Parameters = functionSource._Parameters.DeepClone(options) as LanguageElementCollection;
	  }
	  _CallingConvention = functionSource.CallingConvention;
	  _AccessSpecifiers = functionSource._AccessSpecifiers.Clone();
	}
	#endregion
	#region ReplaceOwnedReference
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Parameters != null && _Parameters.Contains(oldElement as Expression))
		_Parameters.Replace(oldElement, newElement);
	  else if (newElement is TypeReferenceExpression && oldElement is TypeReferenceExpression && _ReturnType == (TypeReferenceExpression)oldElement)
		_ReturnType = (TypeReferenceExpression)newElement;
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  FunctionPointerTypeReference functionPointer = new FunctionPointerTypeReference();
	  functionPointer.CloneDataFrom(this, options);
	  return functionPointer;
	}
	#endregion
	#region ToString
	public override string ToString()
	{
	  StringBuilder builder = new StringBuilder();
	  if (ReturnType != null)
		builder.AppendFormat("{0} ( * )( ", ReturnType.ToString());
	  if (Parameters != null)
		builder.Append(Parameters.ToString());
	  builder.Append(")");
	  return builder.ToString();
	}
	#endregion
	public void AddParameter(Param parameter)
	{
	  Parameters.Add(parameter);
	  AddDetailNode(parameter);
	}
	public void AddParameters(IEnumerable<Param> parameters)
	{
	  foreach (Param parameter in parameters)
		AddParameter(parameter);
	}
	#region AccessSpecifiers
	public AccessSpecifiers AccessSpecifiers
	{
	  get { return _AccessSpecifiers; }
	}
	#endregion
	#region CallingConvention
	public CallingConventionSpecifier CallingConvention
	{
	  get
	  {
		return _CallingConvention;
	  }
	  set
	  {
		_CallingConvention = value;
	  }
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.FunctionPointerTypeReference;
	  }
	}
	#endregion
	#region Parameters
	public LanguageElementCollection Parameters
	{
	  get { return _Parameters; }
	  set { SetParameters(value); }
	}
	#endregion
	#region ParametersCount
	public int ParametersCount
	{
	  get
	  {
		if (_Parameters != null)
		  return _Parameters.Count;
		else
		  return 0;
	  }
	}
	#endregion
	#region ReturnType
	public TypeReferenceExpression ReturnType
	{
	  get { return _ReturnType; }
	  set { SetReturnType(value); }
	}
	#endregion
		#region IFunctionPointerTypeReference members
		#region IFunctionPointerTypeReference.Parameters
		IElementCollection IFunctionPointerTypeReference.Parameters
		{
			get
			{
		if (_Parameters == null)
		  return IElementCollection.Empty;
		IElementCollection parameters = new IElementCollection();
		parameters.AddRange(_Parameters);
		return parameters;
			}
		}
		#endregion
		#region IFunctionPointerTypeReference.ParametersCount
		int IFunctionPointerTypeReference.ParametersCount
		{
	  get { return ParametersCount; }
		}
		#endregion
		#region IFunctionPointerTypeReference.ReturnType
		ITypeReferenceExpression IFunctionPointerTypeReference.ReturnType
		{
	  get { return ReturnType; }
		}
		#endregion
		#endregion
	}
}
