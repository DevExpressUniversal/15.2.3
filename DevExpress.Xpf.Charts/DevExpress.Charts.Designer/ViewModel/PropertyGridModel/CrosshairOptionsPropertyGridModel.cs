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

using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartCrosshairOptionsPropertyGridModel : WpfChartElementPropertyGridModelBase {
		readonly CrosshairOptions crosshairOptions;
		WpfChartCrosshairLabelPositionPropertyGridModel crosshairLabelPosition;
		WpfChartLineStylePropertyGridModel argumentLineStyle;
		WpfChartLineStylePropertyGridModel valueLineStyle;
		protected internal CrosshairOptions CrosshairOptions { get { return crosshairOptions; } }
		[Category(Categories.Behavior)]
		public bool ShowArgumentLabels {
			get { return CrosshairOptions.ShowArgumentLabels; }
			set { SetProperty("ShowArgumentLabels", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowArgumentLine {
			get { return CrosshairOptions.ShowArgumentLine; }
			set { SetProperty("ShowArgumentLine", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowCrosshairLabels {
			get { return CrosshairOptions.ShowCrosshairLabels; }
			set { SetProperty("ShowCrosshairLabels", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowOnlyInFocusedPane {
			get { return CrosshairOptions.ShowOnlyInFocusedPane; }
			set { SetProperty("ShowOnlyInFocusedPane", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowValueLabels {
			get { return CrosshairOptions.ShowValueLabels; }
			set { SetProperty("ShowValueLabels", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowValueLine {
			get { return CrosshairOptions.ShowValueLine; }
			set { SetProperty("ShowValueLine", value); }
		}
		[Category(Categories.Behavior)]
		public CrosshairSnapMode SnapMode {
			get { return CrosshairOptions.SnapMode; }
			set { SetProperty("SnapMode", value); }
		}
		[Category(Categories.Behavior)]
		public CrosshairLabelMode CrosshairLabelMode {
			get { return CrosshairOptions.CrosshairLabelMode; }
			set { SetProperty("CrosshairLabelMode", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartCrosshairLabelPositionPropertyGridModel CommonLabelPosition {
			get { return crosshairLabelPosition; }
			set { SetProperty("CommonLabelPosition", value.NewCrosshairLabelPosition); }
		}
		[Category(Categories.Behavior)]
		public Brush ArgumentLineBrush {
			get { return CrosshairOptions.ArgumentLineBrush; }
			set { SetProperty("ArgumentLineBrush", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartLineStylePropertyGridModel ArgumentLineStyle {
			get { return argumentLineStyle; }
			set { SetProperty("ArgumentLineStyle", new LineStyle()); }
		}
		[Category(Categories.Behavior)]
		public Brush ValueLineBrush {
			get { return CrosshairOptions.ValueLineBrush; }
			set { SetProperty("ValueLineBrush", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartLineStylePropertyGridModel ValueLineStyle {
			get { return valueLineStyle; }
			set { SetProperty("ValueLineStyle", new LineStyle()); }
		}
		[Category(Categories.Behavior)]
		public bool HighlightPoints {
			get { return CrosshairOptions.HighlightPoints; }
			set { SetProperty("HighlightPoints", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowGroupHeaders {
			get { return CrosshairOptions.ShowGroupHeaders; }
			set { SetProperty("ShowGroupHeaders", value); }
		}
		[Category(Categories.Behavior)]
		public string GroupHeaderPattern {
			get { return CrosshairOptions.GroupHeaderPattern; }
			set { SetProperty("GroupHeaderPattern", value); }
		}
		public WpfChartCrosshairOptionsPropertyGridModel() : this(null, null, string.Empty) {
		}
		public WpfChartCrosshairOptionsPropertyGridModel(WpfChartModel chartModel, CrosshairOptions crosshairOptions, string propertyPath)
			: base(chartModel, propertyPath) {
				this.crosshairOptions = crosshairOptions;
				UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (CrosshairOptions == null)
				return;
			if (CrosshairOptions.CommonLabelPosition != null) {
				if (crosshairLabelPosition != null && CrosshairOptions.CommonLabelPosition != crosshairLabelPosition.CrosshairLabelPosition || crosshairLabelPosition == null) {
					if (CrosshairOptions.CommonLabelPosition is CrosshairFreePosition)
						crosshairLabelPosition = new WpfChartCrosshairFreePositionPropertyGridModel(ChartModel, CrosshairOptions.CommonLabelPosition, "CrosshairOptions.CommonLabelPosition.");
					else if (CrosshairOptions.CommonLabelPosition is CrosshairMousePosition)
						crosshairLabelPosition = new WpfChartCrosshairMousePositionPropertyGridModel(ChartModel, CrosshairOptions.CommonLabelPosition, "CrosshairOptions.CommonLabelPosition.");
				}
			}
			else
				crosshairLabelPosition = null;
			if (CrosshairOptions.ArgumentLineStyle != null) {
				if (argumentLineStyle != null && CrosshairOptions.ArgumentLineStyle != argumentLineStyle.LineStyle || argumentLineStyle == null)
					argumentLineStyle = new WpfChartLineStylePropertyGridModel(ChartModel, CrosshairOptions.ArgumentLineStyle, SetObjectPropertyCommand, "CrosshairOptions.ArgumentLineStyle.");
			}
			else
				argumentLineStyle = null;
			if (CrosshairOptions.ValueLineStyle != null) {
				if (valueLineStyle != null && CrosshairOptions.ValueLineStyle != valueLineStyle.LineStyle || valueLineStyle == null)
					valueLineStyle = new WpfChartLineStylePropertyGridModel(ChartModel, CrosshairOptions.ValueLineStyle, SetObjectPropertyCommand, "CrosshairOptions.ValueLineStyle.");
			}
			else
				valueLineStyle = null;
		}
	}
}
