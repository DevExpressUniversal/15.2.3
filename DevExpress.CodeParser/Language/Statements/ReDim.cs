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
  public class ReDim : Statement
  {
	bool _HasPreserve;
	ExpressionCollection _Expressions;
	#region ReDim
	public ReDim()
	{
	}
	public ReDim(bool hasPreserve, ExpressionCollection expressions)
	{
	  _HasPreserve = hasPreserve;
	  SetExpressionsModifiedFlag(expressions, false);
	  _Expressions = expressions;
	  AddDetailNodes(expressions);
	}
	#endregion
	void SetExpressionsModifiedFlag(ExpressionCollection expressions)
	{
	  SetExpressionsModifiedFlag(expressions, true);
	}
	void SetExpressionsModifiedFlag(ExpressionCollection expressions, bool force)
	{
	  if (expressions == null)
		return;
	  for (int i = 0; i < expressions.Count; i++)
	  {
		Expression lExpression = expressions[i];
		if (lExpression == null)
		  continue;
		if (force)
		{
		  if (!(lExpression is IndexerExpression))
			continue;
		}
		LanguageElement lStart = lExpression.GetStartElement();
		if (!(lStart is ElementReferenceExpression))
		  continue;
		ElementReferenceExpression lReference = (ElementReferenceExpression)lStart;
		lReference.IsModified = true;
	  }
	}
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is ReDim))
		return;
	  ReDim lSource = (ReDim)source;
	  _HasPreserve = lSource._HasPreserve;
	  if (lSource._Expressions != null)
	  {
		_Expressions = new ExpressionCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Expressions, lSource._Expressions);
		if (_Expressions.Count == 0 && lSource._Expressions.Count > 0)
		  _Expressions = lSource._Expressions.DeepClone(options) as ExpressionCollection;
	  }
	}
	#endregion
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Expressions != null && _Expressions.Contains(oldElement))
		_Expressions.Replace(oldElement, newElement);
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  return ImageIndex.Statement;
	}
	#endregion
	#region ToString
	public override string ToString()
	{
	  string lResult = "ReDim";
	  if (_HasPreserve)
		lResult += " Preserve";
	  return lResult;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  ReDim lClone = new ReDim();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.ReDim;
	  }
	}
	#region HasPreserve
	public bool HasPreserve
	{
	  get
	  {
		return _HasPreserve;
	  }
	}
	#endregion
	#region Expressions
	public ExpressionCollection Expressions
	{
	  get
	  {
		return _Expressions;
	  }
	}
	#endregion
  }
}
