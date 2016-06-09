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
	public class ElementReferenceExpression : ReferenceExpressionBase, IElementReferenceExpression
	{
		const int INT_MaintainanceComplexity = 1;
		TextRange _NameRange;
		bool _IsModified;		
		bool _IsKey = false;
		protected ElementReferenceExpression()
		{			
		}
		#region ElementReferenceExpression
		public ElementReferenceExpression(string name)
		{
			InternalName = name;
		}
		#endregion
		public ElementReferenceExpression(string name, SourceRange range)
			: this(name)
		{
			SetRange(range);
			_NameRange = range;
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ElementReferenceExpression))
				return;
			ElementReferenceExpression lSource = (ElementReferenceExpression)source;
			_IsModified = lSource._IsModified;
			_NameRange = lSource.NameRange;
			_IsKey = lSource.IsKey;
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
			return ImageIndex.ElementReference;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
	  if (!base.IsIdenticalTo(expression))
		return false;
			if (expression is ElementReferenceExpression)
			{
				ElementReferenceExpression lExpression = (ElementReferenceExpression)expression;
				if (Qualifier == null)
				{
					return lExpression.Qualifier == null && 
						Name == lExpression.Name;
				}
				else
					return Qualifier.IsIdenticalTo(lExpression.Qualifier) && Name == lExpression.Name;
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
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ElementReferenceExpression lClone = new ElementReferenceExpression();
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
		#region NeedsInvertParens
		public override bool NeedsInvertParens
		{
			get
			{
				return false;
			}
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ElementReferenceExpression;
			}
		}
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
			set
			{
		ClearHistory();
				_NameRange = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Expression Qualifier
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException("Can't set Qualifier property for ElementReferenceExpression. Qualifier property can be set on QualifiedElementReference");
			}
		}
		public virtual bool HasCleanReferences
		{
			get
			{
				return true;
			}
		}
		public bool IsModified
		{
			get
			{
				return _IsModified;
			}
			set
			{
				_IsModified = value;
			}
		}
		public bool IsKey
		{
			get
			{
				return _IsKey;
			}
			set
			{
				_IsKey = value;
			}
		}
		public bool IsUsed
		{			
			get
			{
				return !(InsideAssignment || InsideOutArgumentDirection);
			}
		}		
		#region IElementReferenceExpression Members
		bool IElementReferenceExpression.IsModified
		{
			get
			{
				return IsModified;
			}
		}
		#endregion
	}
}
