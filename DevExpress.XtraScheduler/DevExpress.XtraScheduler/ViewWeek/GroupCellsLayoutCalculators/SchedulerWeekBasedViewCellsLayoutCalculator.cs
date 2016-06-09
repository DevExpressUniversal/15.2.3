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
using System.Collections.Generic;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class SchedulerWeekBasedViewCellsLayoutCalculator : SchedulerViewCellsLayoutCalculator {
		List<SingleWeekViewInfo> weeks;
		protected SchedulerWeekBasedViewCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
			this.weeks = new List<SingleWeekViewInfo>();
		}
		#region Properties
		protected internal new WeekViewInfo ViewInfo { get { return (WeekViewInfo)base.ViewInfo; } }
		internal List<SingleWeekViewInfo> Weeks { get { return weeks; } }
		#endregion
		public override void CalcLayout(Rectangle bounds) {
			CalcWeeksLayout(bounds);
			ViewInfo.Weeks.AddRange(ViewInfo.PreliminaryLayoutResult.CellContainers);
		}
		public abstract void CreateWeeks();
		public abstract void CalcWeeksLayout(Rectangle bounds);
		protected internal virtual Rectangle[] GetAnchorBounds(SchedulerHeaderCollection headers) {
			int count = headers.Count;
			Rectangle[] anchorBounds = new Rectangle[count];
			for (int i = 0; i < count; i++)
				anchorBounds[i] = headers[i].AnchorBounds;
			return anchorBounds;
		}
	}
}
