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
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Gauges {
	public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
	[NonCategorized]
	public class ValueChangedEventArgs : EventArgs {
		double oldValue;
		double newValue;
		public double OldValue { get { return oldValue; } }
		public double NewValue { get { return newValue; } }
		internal ValueChangedEventArgs(double oldValue, double newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
	public delegate void IndicatorEnterEventHandler(object sender, IndicatorEnterEventArgs e);
	public delegate void IndicatorLeaveEventHandler(object sender, IndicatorLeaveEventArgs e);
	public delegate void CreepingLineAnimationCompletedEventHandler(object sender, EventArgs e);
	[NonCategorized]
	public class IndicatorEnterLeaveEventArgs : ValueChangedEventArgs {
		ValueIndicatorBase indicator;
		public ValueIndicatorBase Indicator { get { return indicator; } }
		internal IndicatorEnterLeaveEventArgs(ValueIndicatorBase indicator, double oldValue, double newvalue) : base(oldValue, newvalue) {
			this.indicator = indicator;
		}
	}
	[NonCategorized]
	public class IndicatorEnterEventArgs : IndicatorEnterLeaveEventArgs {
		internal IndicatorEnterEventArgs(ValueIndicatorBase indicator, double oldValue, double newvalue)
			: base(indicator, oldValue, newvalue) {
		}
	}
	[NonCategorized]
	public class IndicatorLeaveEventArgs : IndicatorEnterLeaveEventArgs {
		internal IndicatorLeaveEventArgs(ValueIndicatorBase indicator, double oldValue, double newvalue)
			: base(indicator, oldValue, newvalue) {
		}
	}
}
