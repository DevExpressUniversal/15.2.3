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
	public class DayViewPreliminaryLayoutResult : ViewInfoBasePreliminaryLayoutResult {
		SchedulerHeaderPreliminaryLayoutResultCollection dateHeadersPreliminaryLayoutResult;
		SchedulerHeaderPreliminaryLayoutResultCollection groupSeparatorsPreliminaryLayoutResult;
		SchedulerHeaderPreliminaryLayoutResultCollection resourceHeadersPreliminaryLayoutResult;
		SchedulerHeaderCollection dateHeaders;
		DayViewCellsPreliminaryLayoutResult cellsPreliminaryLayoutResult;
		int dateHeadersHeight;
		int resourceHeadersHeight;
		int totalHeadersHeight;
		CellsLayerInfos cellsLayerInfos;
		Rectangle extendedCellsBounds;
		int extendedRowCount;
		DayViewRowCollection rows;
		AllDayAreaScrollContainer allDayAreaScrollContainer;
		DayViewInfo viewInfo;
		Rectangle viewInfoBounds;
		public DayViewPreliminaryLayoutResult(DayViewInfo viewInfo){
			dateHeaders = new SchedulerHeaderCollection();
			cellsPreliminaryLayoutResult = new DayViewCellsPreliminaryLayoutResult();
			cellsLayerInfos = new CellsLayerInfos();
			rows = new DayViewRowCollection();
			this.allDayAreaScrollContainer = new AllDayAreaScrollContainer();
			this.viewInfo = viewInfo;
			this.viewInfo.SubsribeScrollContainerEvents(this.allDayAreaScrollContainer);
		}
		public SchedulerHeaderPreliminaryLayoutResultCollection DateHeadersPreliminaryLayoutResult {
			get { return dateHeadersPreliminaryLayoutResult; }
			set { dateHeadersPreliminaryLayoutResult = value; }
		}
		public CellsLayerInfos CellsLayerInfos {
			get { return cellsLayerInfos; }
			set { cellsLayerInfos = value; }
		}
		public SchedulerHeaderPreliminaryLayoutResultCollection GroupSeparatorsPreliminaryLayoutResult {
			get { return groupSeparatorsPreliminaryLayoutResult; }
			set { groupSeparatorsPreliminaryLayoutResult = value; }
		}
		public SchedulerHeaderPreliminaryLayoutResultCollection ResourceHeadersPreliminaryLayoutResult {
			get { return resourceHeadersPreliminaryLayoutResult; }
			set { resourceHeadersPreliminaryLayoutResult = value; }
		}
		public Rectangle ViewInfoBounds {
			get { return this.viewInfoBounds; }
			set { this.viewInfoBounds = value; }
		}
		public SchedulerHeaderCollection DateHeaders { get { return dateHeaders; } }
		public DayViewCellsPreliminaryLayoutResult CellsPreliminaryLayoutResult { get { return cellsPreliminaryLayoutResult; } }
		public DayViewRowCollection Rows { get { return this.rows; } }
		public int DateHeadersHeight {
			get { return dateHeadersHeight; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("DateHeadersHeight", value);
				dateHeadersHeight = value;
			}
		}
		public int ResourceHeadersHeight {
			get { return resourceHeadersHeight; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("ResourceHeadersHeight", value);
				resourceHeadersHeight = value;
			}
		}
		public int TotalHeadersHeight {
			get { return totalHeadersHeight; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("TotalHeadersHeight", value);
				totalHeadersHeight = value;
			}
		}
		public AllDayAreaScrollContainer AllDayAreaScrollContainer { get { return this.allDayAreaScrollContainer; } }
		internal Rectangle ExtendedCellsBounds { get { return extendedCellsBounds; } set { extendedCellsBounds = value; } }
		internal int ExtendedRowCount {
			get { return extendedRowCount; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("ExtendedRowCount", value);
				extendedRowCount = value;
			}
		}
		protected override void Dispose(bool disposing) {
			if (this.dateHeaders != null) {
				for (int i = 0; i < this.dateHeaders.Count; i++)
					this.dateHeaders[i].Dispose();
				this.dateHeaders.Clear();
				this.dateHeaders = null;
			}
			if (this.allDayAreaScrollContainer != null) {
				viewInfo.UnsubsribeScrollContainerEvents(this.allDayAreaScrollContainer);
				this.allDayAreaScrollContainer.Dispose();
				this.allDayAreaScrollContainer = null;
			}
			base.Dispose(disposing);
		}
		protected internal virtual void CalculateAllDayAreaBounds() {
			Rectangle columnsBounds = cellsPreliminaryLayoutResult.ColumnsBounds;
			cellsPreliminaryLayoutResult.AllDayAreaBounds = RectUtils.GetTopSideRect(columnsBounds, cellsPreliminaryLayoutResult.AllDayAreaHeight);
		}
		internal void SetViewInfo(DayViewInfo viewInfo) {
			if (this.viewInfo != null)
				this.viewInfo.UnsubsribeScrollContainerEvents(this.allDayAreaScrollContainer);
			this.viewInfo = viewInfo;
			this.viewInfo.SubsribeScrollContainerEvents(this.allDayAreaScrollContainer);
		}
	}
}
