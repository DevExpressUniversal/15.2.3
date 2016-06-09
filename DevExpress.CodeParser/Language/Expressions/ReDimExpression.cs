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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class ReDimExpression : Expression, IReDimExpression
  {
	LanguageElementCollection _Modifiers = null;
	SourceRange _NameRange = SourceRange.Empty;
	Expression _Expression = null;
	public ReDimExpression()
	{
	}
	void SetExpression(Expression expression)
	{
	  if (_Expression != null)
		RemoveDetailNode(_Expression);
	  _Expression = expression;
	  if (_Expression != null)
		AddDetailNode(_Expression);
	}
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  ReDimExpression lClone = new ReDimExpression();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	public void AddModifiers(IEnumerable<LanguageElement> modifiers)
	{
	  if (modifiers == null)
		return;
	  foreach (ArrayNameModifier mod in modifiers)
		AddModifier(mod);
	}
	public void AddModifiers(IEnumerable<ArrayNameModifier> modifiers)
	{
	  if (modifiers == null)
		return;
	  foreach (ArrayNameModifier mod in modifiers)
		AddModifier(mod);
	}
	public void AddModifier(ArrayNameModifier modifier)
	{
	  if (modifier == null)
		return;
	  if (_Modifiers == null)
		_Modifiers = new LanguageElementCollection();
	  _Modifiers.Add(modifier);
	  AddDetailNode(modifier);
	}
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Expression == oldElement)
		_Expression = (Expression)newElement;
	  else if (_Modifiers != null && _Modifiers.Contains(oldElement))
		_Modifiers.Replace(oldElement, newElement);
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is ReDimExpression))
		return;
	  ReDimExpression reDimExp = (ReDimExpression)source;
	  _NameRange = reDimExp.NameRange;
	  if (reDimExp._Expression != null)
	  {
		_Expression = ParserUtils.GetCloneFromNodes(this, reDimExp, reDimExp._Expression) as Expression;
		if (_Expression == null)
		  _Expression = reDimExp._Expression.Clone(options) as Expression;
	  }
	  if (reDimExp._Expression != null)
	  {
		_Modifiers = new LanguageElementCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, reDimExp.DetailNodes, _Modifiers, reDimExp._Modifiers);
		if (_Modifiers.Count == 0 && reDimExp._Modifiers.Count > 0)
		{
		  _Modifiers = reDimExp._Modifiers.DeepClone(options);
		}
	  }
	}
	#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
	public override IElement Resolve(ISourceTreeResolver resolver)
	{
	  return null;
	}
	public override LanguageElementType ElementType
	{
	  get { return LanguageElementType.ReDimExpression; }
	}
	public LanguageElementCollection Modifiers
	{
	  get { return _Modifiers; }
	}
	public Expression Expression
	{
	  get { return _Expression; }
	  set { SetExpression(value); }
	}
	public override SourceRange NameRange
	{
	  get { return GetTransformedRange(_NameRange); }
	  set
	  {
		ClearHistory();
		_NameRange = value;
	  }
	}
	IExpression IReDimExpression.Expression
	{
	  get { return _Expression; }
	}
	#region IArrayInitializerExpression Members
	ICollection IReDimExpression.Modifiers
	{
	  get
	  {
		if (_Modifiers == null)
		  _Modifiers = new LanguageElementCollection();
		return _Modifiers;
	  }
	}
	#endregion
  }
}
