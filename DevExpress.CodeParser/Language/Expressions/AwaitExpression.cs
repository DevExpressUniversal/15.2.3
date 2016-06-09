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
  public class AwaitExpression : Expression, IAwaitExpression
  {
	Expression _SourceExpression;
	AwaitExpression()
	{
	}
	public AwaitExpression(Expression source)
	{
	  if (source != null)
		SourceExpression = source;
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  AwaitExpression awaitExpression = source as AwaitExpression;
	  if (awaitExpression == null || awaitExpression._SourceExpression == null)
		return;
	  _SourceExpression = ParserUtils.GetCloneFromNodes(this, awaitExpression, awaitExpression._SourceExpression) as Expression;
	  if (_SourceExpression == null)
		SourceExpression = awaitExpression._SourceExpression.Clone(options) as Expression;
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AwaitExpression clone = new AwaitExpression();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	public override IElement Resolve(ISourceTreeResolver resolver)
	{
	  if (resolver == null)
		return null;
	  return resolver.Resolve(this);
	}
	public override string ToString()
	{
	  string result = "await";
	  if (_SourceExpression == null)
		return result;
	  return string.Concat(result, " ", _SourceExpression);
	}
	public override void CleanUpOwnedReferences()
	{
	  SourceExpression = null;
	  base.CleanUpOwnedReferences();
	}
	public Expression SourceExpression
	{
	  get
	  {
		return _SourceExpression;
	  }
	  set
	  {
		ReplaceDetailNode(_SourceExpression, value);
		_SourceExpression = value;
	  }
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.AwaitExpression;
	  }
	}
	IExpression IAwaitExpression.SourceExpression
	{
	  get { return _SourceExpression; }
	}
  }
}
