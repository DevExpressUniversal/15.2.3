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
using DevExpress.Utils.Drawing;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class WeekViewGroupCellsLayoutCalculator : SchedulerWeekBasedViewCellsLayoutCalculator {
		protected WeekViewGroupCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
		protected abstract ResourceBaseCollection VisibleResources { get; }
		protected internal override void CalculatePreliminaryLayout() {
			Weeks.Clear();
			ViewInfo.PreliminaryLayoutResult.CellContainers.Clear();
			CreateWeeks();
			CalcWeeksPreliminaryLayout();
			ViewInfo.PreliminaryLayoutResult.CellContainers.AddRange(Weeks);
		}
		public override void CreateWeeks() {
			for (int i = 0; i < VisibleResources.Count; i++) {
				Resource resource = VisibleResources[i];
				SingleWeekViewInfo week = CreateWeek(resource, GetColorSchema(resource, i));
				InitializeConainterBorders(week);
				Weeks.Add(week);
			}
		}
		public override void CalcWeeksLayout(Rectangle bounds) {
			for (int i = 0; i < ViewInfo.PreliminaryLayoutResult.CellContainers.Count; i++) {
				SingleWeekViewInfo week = ViewInfo.PreliminaryLayoutResult.CellContainers[i] as SingleWeekViewInfo;
				week.Bounds = CalcWeekBounds(bounds, i);
				week.CalcLayout(ViewInfo.GetWeekDates(week.Interval.Start), GetAnchorBounds(GetHeadersByWeekNumber(i)), Cache, (SchedulerHeaderPainter)Painter);
			}
		}
		protected internal VerticalSingleWeekViewInfo CreateVerticalResourceWeek(Resource resource, DateTime start, SchedulerColorSchema colorSchema) {
			return ViewInfo.CreateResourceWeek(resource, start, colorSchema);
		}
		protected internal virtual void InitializeConainterBorders(SchedulerViewCellContainer container) {
			ResetContainerBorders(container);
		}
		protected void CalcWeeksPreliminaryLayout() {
			for (int i = 0; i < Weeks.Count; i++)
				Weeks[i].CalcPreliminaryLayout(ViewInfo.GetWeekDates(Weeks[i].Interval.Start), Cache, (SchedulerHeaderPainter)Painter, i > 0, i < Weeks.Count - 1);
		}
		protected abstract SingleWeekViewInfo CreateWeek(Resource resource, SchedulerColorSchema colorSchema);
		protected abstract Rectangle CalcWeekBounds(Rectangle bounds, int weekNumber);
		protected abstract SchedulerHeaderCollection GetHeadersByWeekNumber(int weekNumber);
	}
}
