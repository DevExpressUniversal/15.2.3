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
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class SeparatePaneIndicator : Indicator, ISeparatePaneIndicator {
		string paneName;
		string axisYName;
		Axis2D actualAxisY;
		XYDiagramPaneBase actualPane;
		SeparatePaneIndicatorBehavior SeparatePaneIndicatorBehavior { get { return (SeparatePaneIndicatorBehavior)base.IndicatorBehavior; } }
		protected internal Axis2D ActualAxisY {
			get {
				if (actualAxisY != null)
					return actualAxisY;
				XYDiagram2D diagram = GetXYDiagram();
				if (diagram != null)
					return diagram.ActualAxisY;
				return null;
			}
			set {
				actualAxisY = value;
				if (actualAxisY != null)
					AxisYName = actualAxisY.Name;
			}
		}
		protected internal XYDiagramPaneBase ActualPane {
			get {
				if (actualPane != null)
					return actualPane;
				XYDiagram2D diagram = GetXYDiagram();
				if (diagram != null)
					return diagram.DefaultPane;
				return null;
			}
			set {
				actualPane = value;
				if (actualPane != null)
					PaneName = actualPane.Name;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string PaneName {
			get {
				XYDiagramPaneBase actualPane = ActualPane;
				if (actualPane == null) {
					XYDiagram2D diagram = GetXYDiagram();
					return diagram == null ? String.Empty : diagram.DefaultPane.Name;
				}
				return actualPane.Name;
			}
			set { paneName = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string AxisYName {
			get {
				Axis2D actualAxisY = ActualAxisY;
				if (actualAxisY == null) {
					XYDiagram2D diagram = GetXYDiagram();
					return diagram == null ? String.Empty : diagram.ActualAxisY.Name;
				}
				return actualAxisY.Name;
			}
			set { axisYName = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeparatePaneIndicatorPane"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeparatePaneIndicator.Pane"),
		TypeConverter(typeof(PaneTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.PaneTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public XYDiagramPaneBase Pane {
			get { return ActualPane; }
			set {
				if (value != ActualPane) {
					XYDiagram2D diagram = GetXYDiagram();
					if (diagram != null && value != diagram.DefaultPane && !diagram.Panes.Contains(value))
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectIndicatorPane));
					SendNotification(new ElementWillChangeNotification(this));
					PropertyUpdateInfo updateInfo = new PropertyUpdateInfo(base.Owner, "Pane");
					ActualPane = value;
					RaiseControlChanged(updateInfo);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeparatePaneIndicatorAxisY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeparatePaneIndicator.AxisY"),
		TypeConverter(typeof(AxisTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.SeriesViewAxisYTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public Axis2D AxisY {
			get { return ActualAxisY; }
			set {
				if (value != ActualAxisY) {
					XYDiagram2D diagram = GetXYDiagram();
					if (diagram != null && value != diagram.ActualAxisY && !diagram.ActualSecondaryAxesY.ContainsInternal(value))
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectIndicatorAxisY));
					SendNotification(new ElementWillChangeNotification(this));
					PropertyUpdateInfo<IAxisData> updateInfo = new PropertyUpdateInfo<IAxisData>(this, "AxisY", ActualAxisY, value);
					ActualAxisY = value;
					RaiseControlChanged(updateInfo);
				}
			}
		}
		protected SeparatePaneIndicator(string name) 
			: base(name) { }
		#region ISeparatePaneIndicator
		IPane ISeparatePaneIndicator.Pane { get { return ActualPane; } }
		#endregion
		#region IAffectsAxisRange
		MinMaxValues IAffectsAxisRange.GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return SeparatePaneIndicatorBehavior.GetFilteredMinMaxY(visualRangeOfOtherAxisForFiltering);
		}
		IAxisData IAffectsAxisRange.AxisYData { get { return ActualAxisY; } }
		#endregion
		#region ShouldSerialize && Reset
		bool ShouldSerializePaneName() {
			XYDiagramPaneBase actualPane = ActualPane;
			if (actualPane == null)
				return false;
			XYDiagram2D diagram = GetXYDiagram();
			return diagram == null || actualPane != diagram.DefaultPane;
		}
		bool ShouldSerializeAxisYName() {
			Axis2D actualAxisY = ActualAxisY;
			if (actualAxisY == null)
				return false;
			XYDiagram2D diagram = GetXYDiagram();
			return diagram == null || actualAxisY != diagram.ActualAxisY;
		}
		bool ShouldSerializePane() {
			return false;
		}
		bool ShouldSerializeAxisY() {
			return false;
		}
		void ResetPane() {
			Pane = null;
		}
		void ResetAxisY() {
			AxisY = null;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializePaneName() || ShouldSerializeAxisYName() || base.ShouldSerialize();
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "PaneName")
				return ShouldSerializePaneName();
			if (propertyName == "AxisYName")
				return ShouldSerializeAxisYName();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		XYDiagram2D GetXYDiagram() {
			return View != null ? View.GetXYDiagram() : null;
		}
		internal void OnEndLoading() {
			XYDiagram2D diagram = GetXYDiagram();
			if (diagram != null) {
				if (paneName != null)
					ActualPane = diagram.FindPaneByName(paneName) ?? diagram.DefaultPane;
				if (axisYName != null)
					ActualAxisY = diagram.FindAxisYByName(axisYName) ?? diagram.ActualAxisY;
			}
			else {
				ActualPane = null;
				ActualAxisY = null;
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SeparatePaneIndicator indicator = obj as SeparatePaneIndicator;
			if (indicator != null) {
				this.paneName = indicator.paneName;
				this.axisYName = indicator.axisYName;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public abstract class SeparatePaneIndicatorBehavior : IndicatorBehavior {
		protected abstract MinMaxValues MinMaxYByWholeXRange { get; }
		public SeparatePaneIndicatorBehavior(SeparatePaneIndicator indicator) 
			: base(indicator) { }
		protected MinMaxValues GetFilteredMinMaxY(LineStrip indicatorPoints, IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			bool isRangeEmpty = double.IsNaN(visualRangeOfOtherAxisForFiltering.Delta);
			if (isRangeEmpty)
				return MinMaxYByWholeXRange;
			if (indicatorPoints == null || indicatorPoints.Count == 0)
				return MinMaxValues.Empty;
			int minIndex = -1;
			for (int i = 0; i < indicatorPoints.Count; i++) {
				if (indicatorPoints[i].X >= visualRangeOfOtherAxisForFiltering.Min) {
					minIndex = i;
					break;
				}
			}
			int maxIndex = -1;
			for (int i = indicatorPoints.Count - 1; i > -1; i--) {
				if (indicatorPoints[i].X <= visualRangeOfOtherAxisForFiltering.Max) {
					maxIndex = i;
					break;
				}
			}
			if (minIndex == -1 || maxIndex == -1)
				return MinMaxValues.Empty;
			double minValue = indicatorPoints[minIndex].Y;
			double maxValue = indicatorPoints[minIndex].Y;
			for (int i = minIndex + 1; i < maxIndex; i++) {
				if (indicatorPoints[i].Y < minValue)
					minValue = indicatorPoints[i].Y;
				if (indicatorPoints[i].Y > maxValue)
					maxValue = indicatorPoints[i].Y;
			}
			return new MinMaxValues(minValue, maxValue);
		}
		public abstract MinMaxValues GetFilteredMinMaxY(IMinMaxValues visualRangeOfOtherAxisForFiltering);
	}
}
