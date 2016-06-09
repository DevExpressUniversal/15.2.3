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
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraScheduler.Drawing {
	public class WeekViewGroupByResourceHeadersLayoutCalculator : WeekViewGroupHeadersLayoutCalculator {
		public WeekViewGroupByResourceHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
		protected override SchedulerHeaderCollection TopLevelHeaders { get { return ViewInfo.PreliminaryLayoutResult.ResourceHeaders; } }
		protected override SchedulerHeaderCollection SubLevelHeaders { get { return ViewInfo.PreliminaryLayoutResult.DateHeaders; } }
		public override void CalcLayout(Rectangle bounds) {
			SchedulerHeaderCollection groupSeparators = ViewInfo.PreliminaryLayoutResult.GroupSeparators;
			CalculateResourceHeadersBounds(bounds, false, new int[0]);
			SchedulerHeaderPreliminaryLayoutResultCollection groupSeparatorsPreliminaryResults = CalculateHeadersPreliminaryLayout(groupSeparators);
			CalcFinalLayout(groupSeparators, groupSeparatorsPreliminaryResults);
			CacheResourceHeadersSkinElementInfos(TopLevelHeaders);
			ViewInfo.ResourceHeaders.AddRange(TopLevelHeaders);
			ViewInfo.GroupSeparators.AddRange(groupSeparators);
		}
		protected internal override SchedulerHeaderCollection CreateHorizontalResourceHeaders(TimeInterval interval) {
			SchedulerHeaderCollection result = base.CreateHorizontalResourceHeaders(interval);
			SetLeftAndRightBorders(result);
			return result;
		}
		protected internal override SchedulerHeaderCollection CreateTopLevelHeaders() {
			return CreateHorizontalResourceHeaders(new TimeInterval(ViewInfo.VisibleIntervals.Start, ViewInfo.VisibleIntervals.End));
		}
		protected override SchedulerHeaderCollection CreateGroupSeparators() {
			return CreateVerticalGroupSeparators(ViewInfo.VisibleResources.Count - 1);
		}
		protected internal override SchedulerHeaderCollection CreateSubLevelHeaders() {
			return new SchedulerHeaderCollection();
		}
	}
}
