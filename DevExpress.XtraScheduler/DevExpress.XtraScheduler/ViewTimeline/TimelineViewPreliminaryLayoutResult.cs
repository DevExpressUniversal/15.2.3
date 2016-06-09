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

using System.Drawing;
namespace DevExpress.XtraScheduler.Drawing {
	public class TimelineViewPreliminaryLayoutResult : ViewInfoBasePreliminaryLayoutResult {
		SchedulerHeaderLevel baseLevel;
		SchedulerHeaderLevelCollection upperLevels;
		TimeScaleLevelCollection levels;
		int visibleIntervalCount;
		public TimelineViewPreliminaryLayoutResult() {
			this.upperLevels = new SchedulerHeaderLevelCollection();
			this.levels = new TimeScaleLevelCollection();
		}
		public SchedulerHeaderLevel BaseLevel {
			get { return this.baseLevel; }
			set { this.baseLevel = value; }
		}
		public SchedulerHeaderLevelCollection UpperLevels { get { return this.upperLevels; } }
		public TimeScaleLevelCollection Levels { get { return this.levels; } }
		public int VisibleIntervalsCount {
			get { return this.visibleIntervalCount; }
			set { this.visibleIntervalCount = value; }
		}
		protected override void Dispose(bool disposing) {
			if (baseLevel != null) {
				foreach (SchedulerHeader header in baseLevel.Headers)
					header.Dispose();
				baseLevel = null;
			}
			foreach(SchedulerHeaderLevel level in upperLevels) {
				foreach (SchedulerHeader header in level.Headers)
					header.Dispose();
			}
			upperLevels.Clear();
			upperLevels = null;
			base.Dispose(disposing);
		}
	}
}
