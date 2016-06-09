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
	public class IndexerExpression: Expression, IHasQualifier, IIndexerExpression
	{
		const int INT_MaintainanceComplexity = 3;
		Expression _Source;
		ExpressionCollection _Arguments;
		IndexerExpression()
		{			
		}
		#region IndexerExpression
		public IndexerExpression(Expression source)
		{
		  SetSource(source);			
		}
		#endregion
		#region ComareIndexerArguments
		private bool ComareIndexerArguments(IndexerExpression expression)
		{
			if (expression == null)
				return false;
			if (Arguments == null && expression.Arguments == null)
				return true;
			if (Arguments == null)
				return false;
			return Arguments.IsIdenticalTo(expression.Arguments);
		}
		#endregion
	void SetArguments(ExpressionCollection args)
	{
	  if (_Arguments != null)
		RemoveDetailNodes(args);
	  _Arguments = args;
	  if (_Arguments != null)
		AddDetailNodes(args);
	}
		protected internal void SetSource(Expression source)
		{
	  if (_Source != null)
		RemoveNode(_Source);
	  InternalName = String.Empty;
			_Source = source;
	  if (_Source != null)
	  {
		InternalName = _Source.ToString();
		AddNode(_Source);
	  }
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is IndexerExpression))
				return;
			IndexerExpression lSource = (IndexerExpression)source;
			if (lSource._Source != null)
			{				
				_Source = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Source) as Expression;
				if (_Source == null)
					_Source = lSource._Source.Clone(options) as Expression;
			}
			if (lSource._Arguments != null)
			{
				_Arguments = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Arguments, lSource._Arguments);
				if (_Arguments.Count == 0 && lSource._Arguments.Count > 0)
					_Arguments = lSource._Arguments.DeepClone(options) as ExpressionCollection;
			}			
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string lResult = String.Empty;
			if (_Source != null)
				lResult += _Source.ToString();
			lResult += "[";
			if (_Arguments != null)
				lResult += _Arguments.ToString();
			lResult += "]";
			return lResult;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Indexer;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is IndexerExpression)
			{
				IndexerExpression lExpression = (IndexerExpression)expression;
				if (Qualifier == null)
					return ComareIndexerArguments(lExpression);
				else
					return Qualifier.IsIdenticalTo(lExpression.Qualifier) && ComareIndexerArguments(lExpression);
			}
			return false;
		}
		#endregion
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Source = null;
			if (_Arguments != null)
				_Arguments.Clear();
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Source == oldElement)
				_Source = (Expression)newElement;
			else if (_Arguments != null)
				_Arguments.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			IndexerExpression lClone = new IndexerExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	public void AddArgument(Expression arg)
	{
	  if (arg == null)
		return;
	  Arguments.Add(arg);
	  AddDetailNode(arg);
	}
	public void AddArguments(IEnumerable<Expression> arguments)
	{
	  if (arguments == null)
		return;
	  foreach (Expression expr in arguments)
		AddArgument(expr);
	}
		protected override int ThisMaintenanceComplexity
		{
	  get { return INT_MaintainanceComplexity; }
		}
		#region NeedsInvertParens
		public override bool NeedsInvertParens
		{
	  get { return false; }
		}
		#endregion
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.IndexerExpression; }
		}
		public override SourceRange NameRange
		{
	  get { return _Source == null ? SourceRange.Empty : _Source.InternalRange; }
		}
		#region Qualifier
		public Expression Qualifier
		{
	  get { return _Source; }
	  set { SetSource(value); }			
		}
		#endregion
		#region Source
		[Obsolete("Use Qualifier instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Expression Source
		{
	  get { return Qualifier; }
			set
			{
				if (Qualifier == value)
					return;
				Qualifier = value;
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
	  set { SetArguments(value); }
		}
		#endregion
		#region IIndexerExpression Members
		IExpressionCollection IIndexerExpression.Arguments
		{
			get
			{
				if (_Arguments == null)
					return EmptyLiteElements.EmptyIExpressionCollection;				
				return _Arguments;
			}
		}
		#endregion
		#region IWithSource Members
		IExpression IWithSource.Source
		{
			get
			{
				return Qualifier;
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
	}
}
