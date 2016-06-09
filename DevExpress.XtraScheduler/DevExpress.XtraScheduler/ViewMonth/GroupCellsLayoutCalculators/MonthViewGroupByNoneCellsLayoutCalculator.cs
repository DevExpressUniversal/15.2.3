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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class MonthViewGroupByNoneCellsLayoutCalculator : MonthViewGroupCellsLayoutCalculator {
		ResourceBaseCollection visibleResources;
		public MonthViewGroupByNoneCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
		protected override ResourceBaseCollection VisibleResources {
			get {
				if (this.visibleResources == null) {
					this.visibleResources = new ResourceBaseCollection();
					this.visibleResources.Add(ResourceBase.Empty);
				}
				return this.visibleResources;
			}
		}
		protected internal override int CalculateResourceColorIndex(int visibleIndex) {
			return 0;
		}
		protected override Rectangle CalcWeekBounds(Rectangle bounds, int weekNumber) {
			return Rectangle.FromLTRB(bounds.Left, ViewInfo.WeekDaysHeaders[0].AnchorBounds.Bottom, bounds.Right, bounds.Bottom); ;
		}
		protected override SchedulerHeaderCollection GetHeadersByWeekNumber(int weekNumber) {
			return ViewInfo.WeekDaysHeaders;
		}
	}
}
