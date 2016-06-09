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
	public abstract class OperatorExpression : Expression, IOperatorExpression
	{
		SourceRange _NameRange;
		public OperatorExpression()
		{
		}
		public OperatorExpression(string operatorText)
			: this(SourceRange.Empty, operatorText)
		{
		}
		public OperatorExpression(Token operatorToken)
			: this(operatorToken.Range, operatorToken.EscapedValue)
		{
		}
		public OperatorExpression(SourceRange operatorRange, string operatorText)
		{
			SetOperatorRange(operatorRange);
			SetOperatorText(operatorText);
		}		
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is OperatorExpression))
				return;
			OperatorExpression lSource = (OperatorExpression)source;
			_NameRange = lSource.NameRange;			
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		#region SetOperatorRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetOperatorRange(SourceRange range)
		{
	  ClearHistory();
			_NameRange = range;
		}
		#endregion
		#region SetOperatorText
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetOperatorText(string text)
		{
			InternalName = text;
		}
		#endregion
		public override bool IsIdenticalTo(Expression expression)
		{
			if (!(expression is OperatorExpression))
				return false;
			OperatorExpression lOperatorExpression = (OperatorExpression)expression;
			return lOperatorExpression.InternalName == InternalName;
		}		
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
		}
		public SourceRange OperatorRange
		{
			get
			{
				return NameRange;
			}
		}
		public string OperatorText
		{
			get
			{
				return InternalName;
			}
			set
			{
				InternalName = value;
			}
		}
	}
}
