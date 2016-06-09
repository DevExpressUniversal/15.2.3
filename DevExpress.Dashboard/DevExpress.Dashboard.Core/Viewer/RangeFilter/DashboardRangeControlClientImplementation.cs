#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardCommon.Viewer {
	class DashboardRangeControlClientImplementation : IRangeControlClientExtension {
		readonly IDashboardChartControl chartControl;
		readonly RangeFilterLabelsConfigurator labelsConfigurator;
		List<object> ruler;
		IRangeControlClientExtension Client { get { return chartControl.RangeControlClient; } }
		bool IRangeControlClient.IsCustomRuler { get { return Client.IsCustomRuler; } }
		int IRangeControlClient.RangeBoxTopIndent { get { return Client.RangeBoxTopIndent; } }
		int IRangeControlClient.RangeBoxBottomIndent { get { return Client.RangeBoxBottomIndent; } }
		object IRangeControlClient.RulerDelta { get { return Client.RulerDelta; } }
		double IRangeControlClient.NormalizedRulerDelta { get { return Client.NormalizedRulerDelta; } }
		bool IRangeControlClient.IsValid { get { return Client.IsValid; } }
		string IRangeControlClient.InvalidText { get { return Client.InvalidText; } }
		event ClientRangeChangedEventHandler IRangeControlClient.RangeChanged {
			add { Client.RangeChanged += value; }
			remove { Client.RangeChanged -= value; }
		}
		public DashboardRangeControlClientImplementation(IDashboardChartControl chartControl, RangeFilterLabelsConfigurator labelsConfigurator) {
			this.chartControl = chartControl;
			this.labelsConfigurator = labelsConfigurator;
		}
		bool IRangeControlClient.IsValidType(Type type) {
			return Client.IsValidType(type);
		}
		bool IRangeControlClient.SupportOrientation(Orientation orientation) {
			return Client.SupportOrientation(orientation);
		}
		object IRangeControlClient.GetOptions() {
			return Client.GetOptions();
		}
		List<object> IRangeControlClient.GetRuler(RulerInfoArgs e) {
			ruler = Client.GetRuler(e);
			return ruler;
		}
		string IRangeControlClient.RulerToString(int ruleIndex) {
			XYDiagram diagram = chartControl.Diagram as XYDiagram;
			if (diagram == null || ruler == null || ruleIndex < 0 || ruleIndex >= ruler.Count)
				return Client.RulerToString(ruleIndex);
			return labelsConfigurator.GetArgumentPresentationByValue(ruler[ruleIndex]);
		}
		object IRangeControlClient.GetValue(double normalizedValue) {
			return Client.GetValue(normalizedValue);
		}
		double IRangeControlClient.GetNormalizedValue(object value) {
			return GetNormalizedValueInternal(value);
		}
		double IRangeControlClient.ValidateScale(double newScale) {
			return Client.ValidateScale(newScale);
		}
		void IRangeControlClient.ValidateRange(NormalizedRangeInfo info) {
			if(info.ChangedBound == ChangedBoundType.Both || info.Range.Maximum != info.Range.Minimum)
				Client.ValidateRange(info);
		}
		void IRangeControlClient.Calculate(Rectangle contentRect) {
			Client.Calculate(contentRect);
		}
		void IRangeControlClient.DrawContent(RangeControlPaintEventArgs e) {
			Client.DrawContent(e);
		}
		bool IRangeControlClient.DrawRuler(RangeControlPaintEventArgs e) {
			return Client.DrawRuler(e);
		}
		void IRangeControlClient.UpdateHotInfo(RangeControlHitInfo hitInfo) {
			Client.UpdateHotInfo(hitInfo);
		}
		void IRangeControlClient.UpdatePressedInfo(RangeControlHitInfo hitInfo) {
			Client.UpdatePressedInfo(hitInfo);
		}
		void IRangeControlClient.OnRangeControlChanged(IRangeControl rangeControl) {
			Client.OnRangeControlChanged(rangeControl);
		}
		void IRangeControlClient.OnRangeChanged(object rangeMinimum, object rangeMaximum) {
		}
		void IRangeControlClient.OnResize() {
			Client.OnResize();
		}
		void IRangeControlClient.OnClick(RangeControlHitInfo hitInfo) {
			Client.OnClick(hitInfo);
		}
		string IRangeControlClient.ValueToString(double normalizedValue) {
			if (!double.IsNaN(normalizedValue)) {
				object nativeValue = Client.NativeValue(normalizedValue);
				if (nativeValue != null) {
					if (IsNumericType(nativeValue))
						nativeValue = Math.Round(Convert.ToDouble(nativeValue));
					return labelsConfigurator.GetArgumentPresentationByValue(nativeValue);
				}
			}
			return string.Empty;
		}
		object IRangeControlClientExtension.NativeValue(double normalizedValue) {
			return NativeValueInternal(normalizedValue);
		}
		double GetNormalizedValueInternal(object value) {
			return Client.GetNormalizedValue(value);
		}
		object NativeValueInternal(double normalizedValue) {
			return Client.NativeValue(normalizedValue);
		}
		bool IsNumericType(object value) {
			return (value is Byte) ||
				   (value is SByte) ||
				   (value is Int16) ||
				   (value is UInt16) ||
				   (value is Int32) ||
				   (value is UInt32) ||
				   (value is Int64) ||
				   (value is UInt64) ||
				   (value is Single) ||
				   (value is Double) ||
				   (value is Decimal);
		}
	}
}
