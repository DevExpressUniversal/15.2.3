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
using System.Drawing;
using System.Linq;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Drawing {
	public class DayViewCellsPreliminaryLayoutResult {
		TimeOfDayInterval alignedVisibleTime;
		int timeIntervalsCount;
		int minRowHeight;
		Rectangle columnsBounds;
		Rectangle rowsBounds;
		Rectangle rowsBoundsWithExtendedCells;
		bool scrollBarVisible;
		int allDayAreaHeight;
		int statusLineWidth;
		Rectangle allDayAreaBounds;
		public DayViewCellsPreliminaryLayoutResult() {
			minRowHeight = 1;
		}
		public int MinRowHeight {
			get { return minRowHeight; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("MinRowHeight", value);
				minRowHeight = value;
			}
		}
		public TimeOfDayInterval AlignedVisibleTime {
			get { return alignedVisibleTime; }
			set { alignedVisibleTime = value; }
		}
		public int AllDayAreaHeight {
			get { return allDayAreaHeight; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("AllDayAreaHeight", value);
				allDayAreaHeight = value;
			}
		}
		public int StatusLineWidth {
			get { return statusLineWidth; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("StatusLineWidth", value);
				statusLineWidth = value;
			}
		}
		public int TimeIntervalsCount {
			get { return timeIntervalsCount; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("TimeIntervalsCount", value);
				timeIntervalsCount = value;
			}
		}
		public Rectangle ColumnsBounds { get { return columnsBounds; } set { columnsBounds = value; } }
		public Rectangle RowsBounds { get { return rowsBounds; } set { rowsBounds = value; } }
		public Rectangle RowsBoundsWithExtendedCells { get { return rowsBoundsWithExtendedCells; } set { rowsBoundsWithExtendedCells = value; } }
		public bool DateTimeScrollBarVisible { get { return scrollBarVisible; } set { scrollBarVisible = value; } }
		public Rectangle AllDayAreaBounds { get { return allDayAreaBounds; } set { allDayAreaBounds = value; } }
	}
}
