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

using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimeRulerRowBasedItemsCalculator {
		#region Fields
		ViewInfoItemCollection result = new ViewInfoItemCollection();
		int rowsCount;
		TimeRulerViewInfo ruler;
		DateTime[] actualTimes;
		#endregion
		protected TimeRulerRowBasedItemsCalculator(TimeRulerViewInfo ruler, DateTime[] actualTimes) {
			if (ruler == null)
				Exceptions.ThrowArgumentException("ruler", ruler);
			if (actualTimes == null)
				Exceptions.ThrowArgumentException("actualTimes", actualTimes);
			this.ruler = ruler;
			this.actualTimes = actualTimes;
		}
		#region Properties
		protected internal int RowsCount { get { return rowsCount; } set { rowsCount = value; } }
		protected internal ViewInfoItemCollection Result { get { return result; } }
		protected internal abstract AppearanceObject Appearance { get; }
		protected internal TimeRulerViewInfo Ruler { get { return ruler; } }
		protected internal DateTime[] ActualTimes { get { return actualTimes; } }
		#endregion
		public virtual void CalculateItems(Rectangle[] rowsBounds) {
			this.rowsCount = rowsBounds.Length;
			if (rowsCount != actualTimes.Length)
				Exceptions.ThrowInternalException();
			for (int i = 0; i < rowsCount; i++)
				CalculateRowItem(rowsBounds[i], i);
		}
		public virtual void CalculateRowItem(Rectangle rowBounds, int rowIndex) {
			if (ShouldCreateItem(rowIndex)) {
				ViewInfoItem item = CreateItem(rowBounds, rowIndex);
				Result.Add(item);
			}
		}
		protected internal abstract bool ShouldCreateItem(int rowIndex);
		protected internal abstract ViewInfoItem CreateItem(Rectangle rowBounds, int rowIndex);
	}
}
