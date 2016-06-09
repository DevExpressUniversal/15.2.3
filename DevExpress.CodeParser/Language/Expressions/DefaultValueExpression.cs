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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class DefaultValueExpression : Expression, IDefaultValueExpression
  {
	const int INT_MaintainanceComplexity = 3;
	TypeReferenceExpression _TypeReference;
	DefaultValueExpression()
	{
	}
	public DefaultValueExpression(TypeReferenceExpression type)
	  : this(type, SourceRange.Empty)
	{
	}
	public DefaultValueExpression(TypeReferenceExpression type, SourceRange range)
	{
	  SetTypeReference(type);
	  SetRange(range);
	}
	protected void SetTypeReference(TypeReferenceExpression type)
	{
	  if (_TypeReference != null)
		RemoveDetailNode(_TypeReference);
	  _TypeReference = type;
	  if (_TypeReference != null)
		AddDetailNode(_TypeReference);
	}
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_TypeReference == oldElement)
		_TypeReference = newElement as TypeReferenceExpression;
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  DefaultValueExpression defaultValueExpression = source as DefaultValueExpression;
	  if (defaultValueExpression == null || defaultValueExpression._TypeReference == null)
		return;
	  _TypeReference = ParserUtils.GetCloneFromNodes(this, defaultValueExpression, defaultValueExpression._TypeReference) as TypeReferenceExpression;
	  if (_TypeReference == null)
		_TypeReference = defaultValueExpression._TypeReference.Clone(options) as TypeReferenceExpression;
	}
	#endregion
	public override IElement Resolve(ISourceTreeResolver resolver)
	{
	  if (resolver != null)
		return resolver.Resolve(TypeReference);
	  return null;
	}
	public override string ToString()
	{
	  string typeRef = _TypeReference == null ? string.Empty : _TypeReference.ToString();
	  return string.Concat("default(", typeRef, ")");
	}
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  DefaultValueExpression clone = new DefaultValueExpression();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	public override void CleanUpOwnedReferences()
	{
	  _TypeReference = null;
	  base.CleanUpOwnedReferences();
	}
	protected override int ThisMaintenanceComplexity
	{
	  get { return INT_MaintainanceComplexity; }
	}
	public override LanguageElementType ElementType
	{
	  get { return LanguageElementType.DefaultValueExpression; }
	}
	public TypeReferenceExpression TypeReference
	{
	  get { return _TypeReference; }
	  set { SetTypeReference(value); }
	}
	#region IDefaultValueExpression Members
	ITypeReferenceExpression IDefaultValueExpression.TypeReference
	{
	  get { return TypeReference; }
	}
	#endregion
  }
}
