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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class SchedulerSingleViewPrintBuilder : SchedulerViewPrintBuilder {
		SchedulerSinglePrintViewBuilderStrategy builderStrategy;
		public SchedulerSingleViewPrintBuilder(PrintStyleWithResourceOptions printStyle, SchedulerControl control, GraphicsInfo gInfo)
			: base(printStyle, control, gInfo) {
			builderStrategy = SchedulerSinglePrintViewBuilderStrategy.CreateStrategy(GInfo, Control, PrintStyle);
		}
		protected override void CreateSingleIntervalViewInfo(CompositeViewInfo printViewInfo, Rectangle currentPageBounds) {
			ResourceBaseCollection resources = GetPrintedResources();
			IPrintableObjectViewInfo singleViewInfo = builderStrategy.CreateViewInfo(currentPageBounds, CurrentInterval, CurrentPart, resources);
			printViewInfo.AddChild(singleViewInfo);
			printViewInfo.AddChild(new PageBreakViewInfo(currentPageBounds.Bottom));
		}
		protected override PageLayout Layout { get { return builderStrategy.Layout; } }
		protected internal override ViewPart CalculateFirstViewPart() {
			return builderStrategy.CalculateFirstViewPart();
		}
		protected internal override TimeInterval CalculateFirstInterval(DateTime startRangeDate, DateTime endRangeDate) {
			return builderStrategy.CalculateFirstInterval(startRangeDate, endRangeDate, Control.FirstDayOfWeek);
		}
		protected internal override TimeInterval CalculateNextInterval(TimeInterval currentInterval) {
			return builderStrategy.CalculateNextInterval(currentInterval);
		}
	}
}
