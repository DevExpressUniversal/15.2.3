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

using DevExpress.XtraCharts.Localization;
using System.Collections.Generic;
using System;
namespace DevExpress.XtraCharts.Native {
	public class DummyRangeControlClient : ISupportRangeControl {
		bool ISupportRangeControl.IsValid { get { return false; } }
		string ISupportRangeControl.InvalidText { get { return ChartLocalizer.GetString(ChartStringId.InvalidRangeControlText); } }
		int ISupportRangeControl.TopIndent { get { return 0; } }
		int ISupportRangeControl.BottomIndent { get { return 0; } }
		void ISupportRangeControl.OnRangeControlChanged(object sender) { }
		NormalizedRange ISupportRangeControl.ValidateNormalRange(NormalizedRange range, RangeValidationBase validationBase) { return range; }
		List<object> ISupportRangeControl.GetValuesInRange(object min, object max, int scaleLength) { return null; }
		string ISupportRangeControl.ValueToString(double normalizedValue) { return ""; }
		string ISupportRangeControl.RulerToString(int rulerIndex) { return ""; }
		object ISupportRangeControl.ProjectBackValue(double normalOffset) { return null; }
		double ISupportRangeControl.ProjectValue(object value) { return 0; }
		void ISupportRangeControl.DrawContent(IRangeControlPaint paint) { }
		void ISupportRangeControl.RangeChanged(object minValue, object maxValue) { }
		bool ISupportRangeControl.CheckTypeSupport(Type type) { return true; }
		void ISupportRangeControl.Invalidate(bool redraw) { }
		object ISupportRangeControl.NativeValue(double normalOffset) { return null; }
		object ISupportRangeControl.GetOptions() { return null; }
	}
}
