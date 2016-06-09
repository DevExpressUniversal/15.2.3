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
using System.Collections.Generic;
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class LambdaFunctionExpression : LambdaExpression, IHasType
  {
	TypeReferenceExpression _Type;
	public override void CleanUpOwnedReferences()
	{
	  _Type = null;
	  base.CleanUpOwnedReferences();
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  LambdaFunctionExpression clone = new LambdaFunctionExpression();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  LambdaFunctionExpression lambdaFunctionExpressionSource = source as LambdaFunctionExpression;
	  if (lambdaFunctionExpressionSource == null)
		return;
	  _Type = ParserUtils.GetCloneFromNodes(this, lambdaFunctionExpressionSource, lambdaFunctionExpressionSource._Type) as TypeReferenceExpression;
	  if (_Type == null && lambdaFunctionExpressionSource._Type != null)
		_Type = lambdaFunctionExpressionSource._Type.Clone(options) as TypeReferenceExpression;
	}
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Type == oldElement)
		_Type = (TypeReferenceExpression)newElement;
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
	public void SetType(TypeReferenceExpression type)
	{
	  if (_Type != null)
		RemoveDetailNode(_Type);
	  _Type = type;
	  if (_Type != null)
		AddDetailNode(_Type);
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.LambdaFunctionExpression;
	  }
	}
	public TypeReferenceExpression Type
	{
	  get { return _Type; }
	  set { SetType(value); }
	}
	public override bool IsFunction {
		get {
			return true;
		}
		set {
		}
	}
	#region IHasType Members
	bool IHasType.Is(string fullTypeName)
	{
	  if (_Type == null)
		return false;
	  return _Type.Is(fullTypeName);
	}
	bool IHasType.Is(ITypeElement type)
	{
	  if (_Type == null)
		return false;
	  return _Type.Is(type);
	}
	bool IHasType.Is(Type type)
	{
	  if (_Type == null)
		return false;
	  return _Type.Is(type);
	}
	bool IHasType.Is(ISourceTreeResolver resolver, string fullTypeName)
	{
	  if (_Type == null)
		return false;
	  return _Type.Is(resolver, fullTypeName);
	}
	ITypeReferenceExpression IHasType.Type
	{
	  get { return _Type; }
	}
	#endregion
  }
}
