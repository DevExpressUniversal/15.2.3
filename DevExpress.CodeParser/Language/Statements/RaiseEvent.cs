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
	public class RaiseEvent : Statement
	{
		Expression _Expression;
		public RaiseEvent()
		{
		}
		public RaiseEvent(Expression expression)
			: this ()
		{
			_Expression = expression;
			AddDetailNode(expression);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is RaiseEvent))
				return;
			RaiseEvent lSource = (RaiseEvent)source;
			if (lSource._Expression != null)
			{				
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}			
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expression == oldElement)
				_Expression = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Event;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "RaiseEvent";
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			RaiseEvent lClone = new RaiseEvent();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.RaiseEvent;
			}
		}
		public Expression Expression
		{
			get
			{
				return _Expression;
			}
		}
	}
}
