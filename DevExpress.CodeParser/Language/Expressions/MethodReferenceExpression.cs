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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class MethodReferenceExpression : ReferenceExpressionBase, IMethodReferenceExpression
	{
		const int INT_MaintainanceComplexity = 3;
		#region private fields...
		Expression _Source;
		TextRange _NameRange;
		#endregion
		#region MethodReferenceExpression
		protected MethodReferenceExpression()
		{			
		}
		#endregion
		#region MethodReferenceExpression
		public MethodReferenceExpression(string name)
			: this(name, SourceRange.Empty)
		{
		}
		#endregion
		#region MethodReferenceExpression
		public MethodReferenceExpression(string name, SourceRange namerange)
		{
			InternalName = name;
			_NameRange = namerange;
		}
		#endregion
		#region MethodReferenceExpression
		public MethodReferenceExpression(Expression source, string name)
			: this(source, name, SourceRange.Empty)
		{
		}
		#endregion
		#region MethodReferenceExpression
		public MethodReferenceExpression(Expression source, string name, SourceRange namerange)
			: this(name, namerange)
		{
	  SetSource(source);
		}
		#endregion
	void SetSource(Expression source)
	{
	  if (_Source != null)
		RemoveNode(_Source);
	  _Source = source;
	  if (_Source != null)
		AddNode(_Source);
	}
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Source == oldElement)
				_Source = (Expression)newElement;
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
			if (!(source is MethodReferenceExpression))
				return;
			MethodReferenceExpression lSource = (MethodReferenceExpression)source;
			_NameRange = lSource.NameRange;
			if (lSource._Source != null)
			{				
				_Source = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Source) as Expression;
				if (_Source == null)
					_Source = lSource._Source.Clone(options) as Expression;
			}			
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.MethodReference;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
	  if (!base.IsIdenticalTo(expression))
		return false;
			if (expression is MethodReferenceExpression)
			{
				MethodReferenceExpression lExpression = (MethodReferenceExpression)expression;
				if (Qualifier == null)
					return Name == lExpression.Name;
				else
					return Qualifier.IsIdenticalTo(lExpression.Qualifier) && Name == lExpression.Name;
			}
			return false;
		}
		#endregion
		#region Resolve
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Source = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			MethodReferenceExpression lClone = new MethodReferenceExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.MethodReferenceExpression;
			}
		}
		#region Source
		public override Expression Qualifier
		{
			get
			{
				return _Source;
			}
			set
			{
		SetSource(value);
			}
		}
		#endregion
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
		}
	}
}
