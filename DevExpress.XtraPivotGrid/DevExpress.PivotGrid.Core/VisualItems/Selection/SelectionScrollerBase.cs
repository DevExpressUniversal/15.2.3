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
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Utils;
#endif
namespace DevExpress.XtraPivotGrid.Selection {
	public abstract class SelectionScrollerBase {
		public const int MinScrollInterval = 50,
			MaxScrollInterval = 200,
			StartScrollInterval = 50,
			ScrollIntervalChangeStep = 20;
		Timer scrollTimer;
		Point scrollPoint;
		bool firstTick;
		int tickNum;
		Point leftTopCoordOffset;
		SelectionVisualItems visualItems;
		public SelectionScrollerBase(SelectionVisualItems visualItems) {
			this.visualItems = visualItems;
			this.scrollTimer = null;
			this.scrollPoint = Point.Empty;
			this.leftTopCoordOffset = Point.Empty;
		}
		protected abstract Point GetLeftTopCoordOffset(Point pt);
		protected abstract Point LeftTopCoord { get; set; }
		protected abstract Point GetCellAt(Point pt);
		protected virtual SelectionVisualItems VisualItems { get { return visualItems; } }
		public bool IsTimerActive {
			get { return this.scrollTimer != null && this.scrollTimer.Enabled; }
		}
		public void StartScrollTimer(Point pt) {
			this.scrollPoint = pt;
			this.leftTopCoordOffset = GetLeftTopCoordOffset(pt);
			if(!this.leftTopCoordOffset.IsEmpty) {
				if(scrollTimer != null) return;
				this.firstTick = true;
				this.tickNum = 0;
				this.scrollTimer = new Timer();
#if !SL
				this.scrollTimer.Interval = StartScrollInterval;
#else
				this.scrollTimer.Interval = TimeSpan.FromMilliseconds(StartScrollInterval);
#endif
				this.scrollTimer.Tick += OnScrollTimerElapsed;
				this.scrollTimer.Enabled = true;
			} else
				StopScrollTimer();
		}
		public void StopScrollTimer() {
			if(this.scrollTimer != null) {
				this.scrollTimer.Enabled = false;
				this.scrollTimer.Tick -= OnScrollTimerElapsed;
				this.scrollTimer.Dispose();
				this.scrollTimer = null;
			}
		}
#if DEBUGTEST
		public
#endif
		void OnScrollTimerElapsed(object sender, EventArgs e) {
			if(this.firstTick) {
				firstTick = false;
#if !SL
				this.scrollTimer.Interval = MaxScrollInterval;
#else
				this.scrollTimer.Interval = TimeSpan.FromMilliseconds(MaxScrollInterval);
#endif
			}
#if !SL
			if(this.scrollTimer != null && this.scrollTimer.Interval > MinScrollInterval) {
				this.scrollTimer.Interval -= ScrollIntervalChangeStep;
#else
			if(this.scrollTimer != null && this.scrollTimer.Interval.TotalMilliseconds > MinScrollInterval) {
				this.scrollTimer.Interval = TimeSpan.FromMilliseconds(this.scrollTimer.Interval.TotalMilliseconds - ScrollIntervalChangeStep);
#endif
			}
			DoScrollAndSelection(this.scrollPoint, this.leftTopCoordOffset);
		}
#if DEBUGTEST
		public
#endif
		void DoScrollAndSelection(Point pt, Point offset) {
			if(offset.IsEmpty) return;
			tickNum++;
			Point newLeftTopCoord = LeftTopCoord;
			if(tickNum % 3 == 0)
				newLeftTopCoord.X += offset.X;
			newLeftTopCoord.Y += offset.Y;
			LeftTopCoord = newLeftTopCoord;
			DoScroll(GetCellAt(pt));
		}
		protected virtual void DoScroll(Point pt) {
			VisualItems.OnCellMouseMove(pt);
		}
	}
}
