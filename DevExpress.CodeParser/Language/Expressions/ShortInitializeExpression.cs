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
  public class ShortInitializeExpression : Expression, IHasArguments, IShortInitializeExpression, IHasParens, IWithArgumentsModifier
  {
	private const int INT_MaintainanceComplexity = 3;
	#region private fields...
	ExpressionCollection _Arguments;
	TextRangeWrapper _ParensRange;
	#endregion
	#region ShortInitializeExpression
	public ShortInitializeExpression()
	{
	}
	#endregion
	void SetArguments(ExpressionCollection arguments)
	{
	  if (_Arguments != null)
		RemoveDetailNodes(_Arguments);
	  _Arguments = arguments;
	  if (_Arguments != null)
		AddDetailNodes(_Arguments);
	}
	#region ReplaceOwnedReference
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Arguments != null)
		_Arguments.ReplaceExpression(oldElement as Expression, newElement as Expression);
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
	#endregion
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is ShortInitializeExpression))
		return;
	  ShortInitializeExpression lSource = (ShortInitializeExpression)source;
	  if (lSource._Arguments != null)
	  {
		_Arguments = new ExpressionCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Arguments, lSource._Arguments);
		if (_Arguments.Count == 0 && lSource._Arguments.Count > 0)
		  _Arguments = lSource._Arguments.DeepClone(options) as ExpressionCollection;
	  }
	  if (lSource._ParensRange != null)
		_ParensRange = lSource.ParensRange;
	}
	#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _ParensRange = ParensRange;
	}
	#region ToString
	public override string ToString()
	{
	  string lResult = String.Empty;
	  lResult += "(";
	  if (_Arguments != null)
		lResult += _Arguments.ToString();
	  lResult += ")";
	  return lResult;
	}
	#endregion
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  return ImageIndex.Expression;
	}
	#endregion
	#region Resolve
	public override IElement Resolve(ISourceTreeResolver resolver)
	{
	  return null;
	}
	#endregion
	#region OwnedReferencesTransfered
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void OwnedReferencesTransfered()
	{
	  if (_Arguments != null)
	  {
		_Arguments.Clear();
		_Arguments = null;
	  }
	  base.OwnedReferencesTransfered();
	}
	#endregion
	#region CleanUpOwnedReferences
	public override void CleanUpOwnedReferences()
	{
	  if (_Arguments != null)
	  {
		for (int i = 0; i < _Arguments.Count; i++)
		  _Arguments[i].CleanUpOwnedReferences();
		_Arguments.Clear();
		_Arguments = null;
	  }
	  base.CleanUpOwnedReferences();
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  ShortInitializeExpression lClone = new ShortInitializeExpression();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetParensRange(Token parenOpen, Token parenClose)
	{
	  SetParensRange(new SourceRange(parenOpen.Range.Start, parenClose.Range.End));
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetParensRange(SourceRange range)
	{
	  ClearHistory();
	  _ParensRange = range;
	}
	public void AddArgument(Expression arg)
	{
	  Arguments.Add(arg);
	  AddDetailNode(arg);
	}
	public void AddArguments(IEnumerable<Expression> arguments)
	{
	  foreach (Expression arg in arguments)
		AddArgument(arg);
	}
	public void RemoveArgument(Expression arg)
	{
	  Arguments.Remove(arg);
	  RemoveDetailNode(arg);
	}
	public void InsertArgument(int index, Expression arg)
	{
	  Arguments.Insert(index, arg);
	  InsertDetailNode(index, arg);
	}
	#region ThisMaintenanceComplexity
	protected override int ThisMaintenanceComplexity
	{
	  get
	  {
		return INT_MaintainanceComplexity;
	  }
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.ShortInitializeExpression;
	  }
	}
	#endregion
	#region Arguments
	public ExpressionCollection Arguments
	{
	  get
	  {
		if (_Arguments == null)
		  _Arguments = new ExpressionCollection();
		return _Arguments;
	  }
	  set
	  {
		SetArguments(value);
	  }
	}
	#endregion
	#region CanBeStatement
	public override bool CanBeStatement
	{
	  get
	  {
		return false;
	  }
	}
	#endregion
	#region IsGeneric
	public bool IsGeneric
	{
	  get
	  {
		return false;
	  }
	}
	#endregion
	#region IShortInitializeExpression Members
	IExpressionCollection IShortInitializeExpression.Arguments
	{
	  get
	  {
		if (_Arguments == null)
		  return EmptyLiteElements.EmptyIExpressionCollection;
		return _Arguments;
	  }
	}
	#endregion
	#region IHasParens Members
	public SourceRange ParensRange
	{
	  get
	  {
		if (_ParensRange == null)
		  return SourceRange.Empty;
		return GetTransformedRange(_ParensRange);
	  }
	}
	#endregion
	#region IHasArguments Members
	ExpressionCollection IHasArguments.Arguments
	{
	  get
	  {
		return Arguments;
	  }
	}
	public int ArgumentsCount
	{
	  get
	  {
		if (_Arguments == null)
		  return 0;
		return _Arguments.Count;
	  }
	}
	#endregion
	#region IWithArguments Members
	IExpressionCollection IWithArguments.Args
	{
	  get
	  {
		return Arguments;
	  }
	}
	#endregion
	#region IWithArgumentsModifier Members
	void IWithArgumentsModifier.AddArgument(IExpression argument)
	{
	  AddArgument(argument as Expression);
	}
	void IWithArgumentsModifier.InsertArgument(int index, IExpression argument)
	{
	  InsertArgument(index, argument as Expression);
	}
	void IWithArgumentsModifier.RemoveArgument(IExpression argument)
	{
	  RemoveArgument(argument as Expression);
	}
	#endregion
  }
}
