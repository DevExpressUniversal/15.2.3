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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class TriFoldPrintViewInfoBuilder : SchedulerViewPrintBuilder {
		const int horizontalIndent = 10;
		SchedulerSinglePrintViewBuilderStrategy leftViewStrategy;
		SchedulerSinglePrintViewBuilderStrategy middleViewStrategy;
		SchedulerSinglePrintViewBuilderStrategy rightViewStrategy;
		SchedulerSinglePrintViewBuilderStrategy strategyWithMinimalDuration;
		public TriFoldPrintViewInfoBuilder(TriFoldPrintStyle printStyle, SchedulerControl control, GraphicsInfo gInfo)
			: base(printStyle, control, gInfo) {
			leftViewStrategy = CreateBuilderStrategy(printStyle.LeftSection);
			middleViewStrategy = CreateBuilderStrategy(printStyle.MiddleSection);
			rightViewStrategy = CreateBuilderStrategy(printStyle.RightSection);
			strategyWithMinimalDuration = SelectStrategyWithMinimalInterval();
		}
		protected override PageLayout Layout { get { return PageLayout.OnePage; } }
		protected new TriFoldPrintStyle PrintStyle { get { return (TriFoldPrintStyle)base.PrintStyle; } }
		protected internal override TimeInterval CalculateFirstInterval(DateTime startRangeDate, DateTime endRangeDate) {
			return strategyWithMinimalDuration.CalculateFirstInterval(startRangeDate, endRangeDate, Control.FirstDayOfWeek);
		}
		protected internal override ViewPart CalculateFirstViewPart() {
			return ViewPart.Both;
		}
		protected internal override TimeInterval CalculateNextInterval(TimeInterval currentInterval) {
			return strategyWithMinimalDuration.CalculateNextInterval(currentInterval);
		}
		protected override void CreateSingleIntervalViewInfo(CompositeViewInfo printViewInfo, Rectangle pageBounds) {
			ResourceBaseCollection resources = GetPrintedResources();
			Rectangle[] bounds = RectUtils.SplitHorizontally(pageBounds, 3);
			Rectangle leftBounds = bounds[0];
			int halfHorizontalIndent = horizontalIndent / 2;
			leftBounds.Width -= halfHorizontalIndent;
			Rectangle middleBounds = bounds[1];
			middleBounds.Inflate(-horizontalIndent / 2, 0);
			Rectangle rightBounds = new Rectangle(bounds[2].X + halfHorizontalIndent, bounds[2].Y, bounds[2].Width - halfHorizontalIndent, bounds[2].Height);
			IPrintableObjectViewInfo leftViewInfo = leftViewStrategy.CreateViewInfo(leftBounds, CurrentInterval, CurrentPart, resources);
			IPrintableObjectViewInfo middleViewInfo = middleViewStrategy.CreateViewInfo(middleBounds, CurrentInterval, CurrentPart, resources);
			IPrintableObjectViewInfo rightViewInfo = rightViewStrategy.CreateViewInfo(rightBounds, CurrentInterval, CurrentPart, resources);
			printViewInfo.AddChild(leftViewInfo);
			printViewInfo.AddChild(middleViewInfo);
			printViewInfo.AddChild(rightViewInfo);
		}
		SchedulerSinglePrintViewBuilderStrategy CreateBuilderStrategy(PrintStyleSectionKind section) {
			switch (section) {
				case PrintStyleSectionKind.DailyCalendar:
					return new DailyPrintViewBuilderStrategy(GInfo, Control, PrintStyle);
				case PrintStyleSectionKind.WeeklyCalendar:
					return new WeeklyTopToBottomPrintViewBuilderStrategy(GInfo, Control, PrintStyle);
				case PrintStyleSectionKind.MonthlyCalendar:
					return new MonthlyPrintViewBuilderStrategy(GInfo, Control, PrintStyle);
				default:
					XtraSchedulerDebug.Assert(false);
					return null;
			}
		}
		SchedulerSinglePrintViewBuilderStrategy SelectStrategyWithMinimalInterval() {
			int leftSection = (int)PrintStyle.LeftSection;
			int middleSection = (int)PrintStyle.MiddleSection;
			int rightSection = (int)PrintStyle.RightSection;
			int min = leftSection;
			SchedulerSinglePrintViewBuilderStrategy result = leftViewStrategy;
			if (middleSection < min) {
				result = middleViewStrategy;
				min = leftSection;
			}
			if (rightSection < min) {
				result = rightViewStrategy;
				min = rightSection;
			}
			return result;
		}
	}
}
