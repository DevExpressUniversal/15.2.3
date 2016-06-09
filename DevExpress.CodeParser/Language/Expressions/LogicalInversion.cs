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
	public class LogicalInversion : UnaryOperatorExpression, ILogicalInversionExpression
	{
		const int INT_MaintainanceComplexity = 5;
		#region LogicalInversion
		public LogicalInversion()
		{
		}
		#endregion
		#region LogicalInversion
		public LogicalInversion(Token token, Expression expression)
			: this(token, expression, false)
		{
		}
		#endregion
		#region LogicalInversion
		public LogicalInversion(Token token, Expression expression, bool isPost)
			: base(token, expression, isPost)
		{
		}
		#endregion
		#region LogicalInversion
		public LogicalInversion(Expression expression)
			: this(expression, false)
		{
		}
		#endregion
		#region LogicalInversion
		public LogicalInversion(Expression expression, bool isPost)
			: base(expression, isPost)
		{
		}
		#endregion
		protected override object EvaluateExpression()
		{
			if (Expression == null)
				return null;
			return !Expression.EvaluateAsBool();
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.LogicalInversion;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (Expression == null)
				return false;
			if (expression is LogicalInversion)
			{
				LogicalInversion lInversion = expression as LogicalInversion;
				bool lResult = Expression.IsIdenticalTo(lInversion.Expression);
				return lResult && IsPostOperator == lInversion.IsPostOperator;
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
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			LogicalInversion lClone = new LogicalInversion();
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
				return LanguageElementType.LogicalInversion;
			}
		}		
	}
}
