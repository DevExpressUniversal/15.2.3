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
  public class MarkupExtensionExpression : Expression, IMarkupExtensionExpression, IHasArguments, IHasQualifier
  {
	SourceRange _NameRange;
	ExpressionCollection _Arguments;
	ExpressionCollection _Initializers;
	Expression _Qualifier;
	public MarkupExtensionExpression()
	{
	  Name = string.Empty;
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  base.CloneDataFrom(source, options);
	  MarkupExtensionExpression markupExtensionExpression = source as MarkupExtensionExpression;
	  if (markupExtensionExpression == null)
		return;
	  if (markupExtensionExpression._Initializers != null)
	  {
		_Initializers = new ExpressionCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, markupExtensionExpression.DetailNodes, _Initializers, markupExtensionExpression._Initializers);
		if (_Initializers.Count == 0 && markupExtensionExpression._Initializers.Count > 0)
		  _Initializers = markupExtensionExpression._Initializers.DeepClone(options) as ExpressionCollection;
	  }
	  if (markupExtensionExpression._Arguments != null)
	  {
		_Arguments = new ExpressionCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, markupExtensionExpression.DetailNodes, _Arguments, markupExtensionExpression._Arguments);
		if (_Arguments.Count == 0 && markupExtensionExpression._Arguments.Count > 0)
		  _Arguments = markupExtensionExpression._Arguments.DeepClone(options) as ExpressionCollection;
	  }
	  if (markupExtensionExpression._Qualifier != null)
		Qualifier = markupExtensionExpression._Qualifier.Clone(options) as Expression;
	  _NameRange = markupExtensionExpression._NameRange;
	}
	public void AddArgument(Expression argument)
	{
	  if (argument == null)
		return;
	  if (_Arguments == null)
		_Arguments = new ExpressionCollection();
	  _Arguments.Add(argument);
	  AddDetailNode(argument);
	}
	public void AddInitializer(Expression initializer)
	{
	  if (initializer == null)
		return;
	  if (_Initializers == null)
		_Initializers = new ExpressionCollection();
	  _Initializers.Add(initializer);
	  AddDetailNode(initializer);
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  MarkupExtensionExpression clone = new MarkupExtensionExpression();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	public override IElement Resolve(ISourceTreeResolver resolver)
	{
	  return null;
	}
	public ExpressionCollection Arguments
	{
	  get { return _Arguments; }
	}	
	IExpressionCollection IWithArguments.Args
	{
	  get { return _Arguments; }
	}
	public int ArgumentCount
	{
	  get
	  {
		if (_Arguments == null)
		  return 0;
		return _Arguments.Count;
	  }
	}
	public int InitializerCount
	{
	  get
	  {
		if (_Initializers == null)
		  return 0;
		return _Initializers.Count;
	  }
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.MarkupExtensionExpression;
	  }
	}
	public ExpressionCollection Initializers
	{
	  get { return _Initializers; }
	}
	public override SourceRange NameRange
	{
	  get
	  {
		return _NameRange;
	  }
	  set
	  {
		_NameRange = value;
	  }
	}
	public Expression Qualifier
	{
	  get { return _Qualifier; }
	  set 
	  {
		ReplaceNode(_Qualifier, value);
		_Qualifier = value;
	  }
	}
	Expression IHasSource.Source
	{
	  get
	  {
		return Qualifier;
	  }
	  set
	  {
		Qualifier = value;
	  }
	}
	IExpression IWithSource.Source
	{
	  get
	  {
		return Qualifier;
	  }
	}
	int IHasArguments.ArgumentsCount
	{
	  get
	  {
		return ArgumentCount;
	  }
	}
	IExpressionCollection IObjectInitializerExpression.Initializers
	{
	  get { return Initializers; }
	}
  }
}
