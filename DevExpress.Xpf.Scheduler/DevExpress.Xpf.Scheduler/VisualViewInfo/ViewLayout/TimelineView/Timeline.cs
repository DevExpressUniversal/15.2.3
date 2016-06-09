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
using System.Collections.Generic;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualTimeline : VisualSimpleResourceInterval {
		#region VerticalCellContainer
		public static readonly DependencyProperty CellContainerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeline, VisualHorizontalCellContainer>("CellContainer", null);
		public VisualHorizontalCellContainer CellContainer { get { return (VisualHorizontalCellContainer)GetValue(CellContainerProperty); } set { SetValue(CellContainerProperty, value); } }
		#endregion
		protected override void CopyFromCore(ICellContainer source) {
			TimelineCellContainer cellContainer = (TimelineCellContainer)source;
			if (CellContainer == null)
				CellContainer = new VisualHorizontalCellContainer();
			((ISupportCopyFrom<ICellContainer>)CellContainer).CopyFrom(source);
			this.View = cellContainer.View;
			this.IntervalStart = cellContainer.Interval.Start;
			this.IntervalEnd = cellContainer.Interval.End;
			DateTime intervalCellsStart = IntervalStart;
			DateTime intervalCellsEnd = IntervalEnd;
			if (cellContainer.CellCount > 0) {
				intervalCellsStart = cellContainer.Cells[0].Interval.Start;
				intervalCellsEnd = cellContainer.Cells[cellContainer.CellCount - 1].Interval.End;
			}
			if (IntervalCells.Start != intervalCellsStart && IntervalCells.End != intervalCellsEnd)
				IntervalCells = new TimeInterval(intervalCellsStart, intervalCellsEnd);
			if (Brushes == null)
				Brushes = new VisualResourceBrushes();
			Brushes.CopyFrom(cellContainer.Brushes, (object)source.Resource.Id);
		}
		public override void CopyAppointmentsFrom(ICellContainer cellContainer) {
			if (CellContainer == null)
				return;
			CellContainer.CopyAppointmentsFrom(cellContainer);
		}
	}
}
