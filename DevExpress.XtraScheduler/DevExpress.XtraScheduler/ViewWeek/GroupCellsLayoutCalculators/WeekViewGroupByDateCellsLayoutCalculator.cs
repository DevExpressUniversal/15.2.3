﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
namespace DevExpress.XtraScheduler.Drawing {
	public class WeekViewGroupByDateCellsLayoutCalculator : WeekViewGroupCellsLayoutCalculator {
		public WeekViewGroupByDateCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
		protected override ResourceBaseCollection VisibleResources { get { return ViewInfo.VisibleResources; } }
		protected override SingleWeekViewInfo CreateWeek(Resource resource, SchedulerColorSchema colorSchema) {
			return new HorizontalSingleWeekViewInfo(ViewInfo, resource, View.VisibleStart, colorSchema);
		}
		protected override Rectangle CalcWeekBounds(Rectangle bounds, int weekNumber) {
			Rectangle resourceAnchorBounds = ViewInfo.ResourceHeaders[weekNumber].AnchorBounds;
			Rectangle rect = Rectangle.FromLTRB(resourceAnchorBounds.Right, resourceAnchorBounds.Top, bounds.Right, resourceAnchorBounds.Bottom);
			if (weekNumber > 0)
				rect = AdjustWeekVerticallyDown(rect.Left, rect.Top, rect.Right, rect.Bottom);
			return rect;
		}
		protected override SchedulerHeaderCollection GetHeadersByWeekNumber(int weekNumbere) {
			return ViewInfo.WeekDaysHeaders;
		}
	}
}
