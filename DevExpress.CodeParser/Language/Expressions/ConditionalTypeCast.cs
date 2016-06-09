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
	public class ConditionalTypeCast : BinaryOperatorExpression, IConditionalTypeCast
	{
		const int INT_MaintainanceComplexity = 4;
		bool _IsIfOperator = true;
		#region ConditionalTypeCast
		private ConditionalTypeCast()
		{}
		#endregion
		#region ConditionalTypeCast
		public ConditionalTypeCast(Expression left, Token token, Expression right)
			: base (left, token, right)
		{
		}
		#endregion
		#region ConditionalTypeCast
		public ConditionalTypeCast(Expression left, Expression right)
			: base (left, right)
		{
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
			ConditionalTypeCast lClone = new ConditionalTypeCast();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			ConditionalTypeCast lSource = source as ConditionalTypeCast;
			if (lSource == null)
				return;
			_IsIfOperator = lSource._IsIfOperator;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.RelationalOperation;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
	  get { return INT_MaintainanceComplexity; }
		}
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.ConditionalTypeCast; }
		}
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is ConditionalTypeCast)
			{
				ConditionalTypeCast lExpression = (ConditionalTypeCast)expression;
				if (LeftSide == null || !LeftSide.IsIdenticalTo(lExpression.LeftSide))
					return false;
				if (RightSide == null || !RightSide.IsIdenticalTo(lExpression.RightSide))
					return false;
				return true;
			}
			return false;
		}
		#endregion
		public bool IsIfOperator
		{
	  get { return _IsIfOperator; }
	  set { _IsIfOperator = value; }
		}
	}
}
