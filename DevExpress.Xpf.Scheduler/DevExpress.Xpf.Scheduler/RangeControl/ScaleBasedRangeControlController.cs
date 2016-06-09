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
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Editors.RangeControl.Internal;
using System.Collections.Generic;
using DevExpress.XtraScheduler;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Drawing.Components;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Utils;
namespace DevExpress.Xpf.Scheduler.Native {
	public class ScaleBasedRangeControlController : ScaleBasedRangeControlControllerBase {
		DateTimeValueNormalizer valueNormalizer;
		public ScaleBasedRangeControlController() {
			this.valueNormalizer = new DateTimeValueNormalizer();
		}
		#region DateTime to value conversion
		public override DateTime GetValue(double normalizedValue) {
			double interval = MaximumComparable - MinimumComparable;
			double comparableValue = MinimumComparable + normalizedValue * interval;
			return (DateTime)this.valueNormalizer.GetRealValue(comparableValue);
		}
		public override double GetNormalizedValue(object value) {
			DateTime dt = Convert.ToDateTime(value);
			double comparableValue = this.valueNormalizer.GetComparableValue(dt);
			double interval = MaximumComparable - MinimumComparable;
			return (comparableValue - MinimumComparable) / interval;
		}
		public override double GetComparableValue(DateTime value) {
			return this.valueNormalizer.GetComparableValue(value);
		}
		public override DateTime GetRealValue(double comparableValue) {
			return (DateTime)this.valueNormalizer.GetRealValue(comparableValue);
		}
		protected virtual TimeSpan GetRealDuration(double comparableDuration) {
			return this.valueNormalizer.GetRealDuration(comparableDuration);
		}
		#endregion
		public double GetComparableFromNormalValue(double normalValue) {
			return normalValue * (MaximumComparable - MinimumComparable) + MinimumComparable;
		}
		public TimeSpan GetBaseScaleSlotDuration(DateTime selectedRangeStart) {
			DateTime start = BaseScale.Floor(selectedRangeStart);
			DateTime end = BaseScale.GetNextDate(start);
			return end - start;
		}
		public void ValidateTotalRange(DateTime visibleStart, DateTime visibleEnd) {
			if (visibleStart > Minimum && visibleEnd < Maximum)
				return;
			TimeSpan total = Maximum - Minimum;
			TimeSpan visible = visibleEnd - visibleEnd;
			TimeSpan half = TimeSpan.FromTicks((total - visible).Ticks / 2);
			UpdateTotalRange(visibleStart - half, visibleEnd + half);
		}
	}
}
