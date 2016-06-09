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
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	#region IntoExpression
	public class IntoExpression : QueryExpressionBase, IIntoExpression
	{
		QueryIdent _IntoTarget;
	public IntoExpression()
	{
	}
	public IntoExpression(QueryIdent intoTarget)
	{
	  SetIntoTarget(intoTarget);
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is IntoExpression))
				return;
			IntoExpression lSource = (IntoExpression)source;			
			if (lSource._IntoTarget != null)
			{
				_IntoTarget = ParserUtils.GetCloneFromNodes(this, lSource, lSource._IntoTarget) as QueryIdent;
				if (_IntoTarget == null)
					_IntoTarget = lSource._IntoTarget.Clone(options) as QueryIdent;
			}
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_IntoTarget == oldElement)
				_IntoTarget = (QueryIdent)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public void SetIntoTarget(QueryIdent intoTarget)
		{
			if (intoTarget == null)
				return;
			ReplaceDetailNode(_IntoTarget, intoTarget);
			_IntoTarget = intoTarget;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			IntoExpression lClone = new IntoExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public QueryIdent IntoTarget	
		{
			get
			{
				return _IntoTarget;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.IntoExpression;
			}
		}
		IQueryIdent IIntoExpression.IntoTarget
		{
			get
			{
				return IntoTarget;
			}
		}
		public override string ToString()
		{			
			if (_IntoTarget == null || _IntoTarget.ToString() == null)
				return "into";
			return String.Format("into {0}",_IntoTarget.ToString());
		}		
	}
	#endregion
}
