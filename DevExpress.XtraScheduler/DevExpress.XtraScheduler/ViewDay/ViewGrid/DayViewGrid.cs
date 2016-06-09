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
using DevExpress.XtraScheduler.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Native {
	public class DayViewGrid : ViewLogicalGrid {
		const int DefaultTapZoneSize = 2;
		DayView view;
		FixedStepScrollDeltaCalculator fixedStepDeltaCalculator;
		public DayViewGrid(DayView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.fixedStepDeltaCalculator = new FixedStepScrollDeltaCalculator(DefaultTapZoneSize);
		}
		protected DayView View { get { return view; } }
		protected FixedStepScrollDeltaCalculator FixedStepDeltaCalculator { get { return fixedStepDeltaCalculator; } }
		public override ScrollDeltaInfo CalculateVerticalDelta(int physicalOffset) {
			ScrollDeltaInfo delta = new ScrollDeltaInfo();
			SchedulerControl scheduler = view.Control;
			int prevCellIndx = (int)scheduler.DateTimeScrollBarObject.ScrollBarAdapter.Value;
			int currentTopRowCellHeight = View.ViewInfo.Rows[prevCellIndx].Bounds.Height;
			delta.LogicalDelta = -physicalOffset / currentTopRowCellHeight;
			int nextCellIndx = prevCellIndx + delta.LogicalDelta;
			nextCellIndx = (int)Math.Max(scheduler.DateTimeScrollBarObject.ScrollBarAdapter.Minimum, nextCellIndx);
			nextCellIndx = (int)Math.Min(scheduler.DateTimeScrollBarObject.ScrollBarAdapter.Maximum, nextCellIndx);
			for (int indx = (int)Math.Min(prevCellIndx, nextCellIndx); indx < Math.Max(prevCellIndx, nextCellIndx); indx++) {
				delta.PhysicalDelta += view.ViewInfo.Rows[indx].Bounds.Height;
			}
			delta.PhysicalDelta *= Math.Sign(delta.LogicalDelta);
			return delta;
		}
		public override ScrollDeltaInfo CalculateHorizontalDelta(int physicalOffset) {
			return FixedStepDeltaCalculator.CalculateDelta(physicalOffset);
		}
		public override void HorizontalOverScroll(int delta) {
			if (delta == 0)
				return;
			SchedulerCommand command = null;
			SchedulerControl control = View.Control;
			if (delta > 0)
				command = new NavigateViewForwardCommand(control);
			else
				command = new NavigateViewBackwardCommand(control);
			command.Execute();
		}
	}
}
