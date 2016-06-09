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
	public class ExpressionCollectionBase : LanguageElementCollectionBase
	{
		#region IsIdenticalTo(ExpressionCollectionBase collection)
		public bool IsIdenticalTo(ExpressionCollectionBase collection)
		{
			if (collection == null)
				return false;
			if (Count != collection.Count)
				return false;
			for (int i = 0; i < Count; i ++)
			{
				Expression lThis = this[i] as Expression;
				Expression lExpression = collection[i] as Expression;
				if (lThis == null && lExpression != null)
					return false;
				if (!lThis.IsIdenticalTo(lExpression))
					return false;
			}
			return true;
		}
		#endregion
		#region ReplaceExpression(Expression oldExpression, Expression newExpression)
		public void ReplaceExpression(Expression oldExpression, Expression newExpression)
		{
			int index = IndexOf(oldExpression);
	  if (index < 0)
				return;
	  RemoveAt(index);
	  if (newExpression != null)
		Insert(index, newExpression);
		}
		#endregion
		protected override NodeList CreateInstance()
		{
			return new ExpressionCollectionBase();			
		}
	}
}
