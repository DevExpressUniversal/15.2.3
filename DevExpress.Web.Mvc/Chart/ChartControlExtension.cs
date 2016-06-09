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
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.Web.Mvc {
	public class ChartControlExtension : ExtensionBase, IDisposable {
		static ChartControlExtension CreateExtension(ChartControlSettings settings, object dataObject) {
			return ExtensionsFactory.InstanceInternal.Chart(settings).Bind(dataObject);
		}
		#region Image Export
		public static void ExportToImage(ChartControlSettings settings, object dataObject, ImageFormat format, Stream stream) {
			ChartControlExtension extension = CreateExtension(settings, dataObject);
			extension.Control.ExportToImage(stream, format);
		}
		public static ActionResult ExportToImage(ChartControlSettings settings, object dataObject, ImageFormat format) {
			ChartControlExtension extension = CreateExtension(settings, dataObject);
			return ExportUtils.Export(extension, s => extension.Control.ExportToImage(s, format), null, true, format.ToString());
		}
		public static ActionResult ExportToImage(ChartControlSettings settings, object dataObject, string fileName, ImageFormat format) {
			ChartControlExtension extension = CreateExtension(settings, dataObject);
			return ExportUtils.Export(extension, s => extension.Control.ExportToImage(s, format), fileName, true, format.ToString());
		}
		public static ActionResult ExportToImage(ChartControlSettings settings, object dataObject, bool saveAsFile, ImageFormat format) {
			ChartControlExtension extension = CreateExtension(settings, dataObject);
			return ExportUtils.Export(extension, s => extension.Control.ExportToImage(s, format), null, saveAsFile, format.ToString());
		}
		public static ActionResult ExportToImage(ChartControlSettings settings, object dataObject, string fileName, bool saveAsFile, ImageFormat format) {
			ChartControlExtension extension = CreateExtension(settings, dataObject);
			return ExportUtils.Export(extension, s => extension.Control.ExportToImage(s, format), fileName, saveAsFile, format.ToString());
		}
		#endregion
		object dataObject;
		protected internal new MVCxChartControl Control {
			get { return (MVCxChartControl)base.Control; }
			protected set { base.Control = value; }
		}
		protected internal new ChartControlSettings Settings {
			get { return (ChartControlSettings)base.Settings; }
			protected set { base.Settings = value; }
		}
		public ChartControlExtension(ChartControlSettings settings) : base(settings) {
		}
		public ChartControlExtension(ChartControlSettings settings, ViewContext viewContext) : base(settings, viewContext) {
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CrosshairOptions.CommonLabelPosition = (CrosshairLabelPosition)Settings.CrosshairOptions.CommonLabelPosition.Clone();
			((IChartContainer)Control).Assign(Settings.Chart);
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.ClientVisible = Settings.ClientVisible;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.SaveStateOnCallbacks = Settings.SaveStateOnCallbacks;
			Control.SelectionMode = Settings.SelectionMode;
			Control.AlternateText = Settings.AlternateText;
			Control.DescriptionUrl = Settings.DescriptionUrl;
			Control.BinaryStorageMode = Settings.BinaryStorageMode;
			Control.LoadingPanelImage.Assign(Settings.LoadingPanelImage);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.ToolTipController.Assign(Settings.ToolTipController);
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.RightToLeft = Settings.RightToLeft;
			Control.BoundDataChanged += Settings.BoundDataChanged;
			Control.CustomDrawSeries += Settings.CustomDrawSeries;
			Control.CustomDrawSeriesPoint += Settings.CustomDrawSeriesPoint;
			Control.CustomPaint += Settings.CustomPaint;
			Control.CustomizeAutoBindingSettings += Settings.CustomizeAutoBindingSettings;
			Control.PivotChartingCustomizeLegend += Settings.PivotChartingCustomizeLegend;
			Control.PivotChartingCustomizeResolveOverlappingMode += Settings.PivotChartingCustomizeResolveOverlappingMode;
			Control.CustomizeSimpleDiagramLayout += Settings.CustomizeSimpleDiagramLayout;
			Control.PivotChartingCustomizeXAxisLabels += Settings.PivotChartingCustomizeXAxisLabels;
			Control.AxisScaleChanged += Settings.AxisScaleChanged;
			Control.AxisWholeRangeChanged += Settings.AxisWholeRangeChanged;
			Control.AxisVisualRangeChanged += Settings.AxisVisualRangeChanged;
			Control.PivotGridSeriesExcluded += Settings.PivotGridSeriesExcluded;
			Control.PivotGridSeriesPointsExcluded += Settings.PivotGridSeriesPointsExcluded;
			Control.CustomCallback += Settings.CustomCallbackInternal;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.CustomDrawAxisLabel += Settings.CustomDrawAxisLabel;
			Control.LegendItemChecked += Settings.LegendItemChecked;
			Control.SelectedItemsChanged += Settings.SelectedItemsChanged;
			Control.ObjectSelected += Settings.ObjectSelected;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxChartControl();
		}
		protected override void LoadPostDataInternal() {
			base.LoadPostDataInternal();
			BindInternal(this.dataObject);
		}
		public void Dispose() {
			if (Control != null) {
				Control.Dispose();
				Control = null;
			}
			if (Settings != null) {
				Settings.Dispose();
				Settings = null;
			}
		}
		public ChartControlExtension Bind(object dataObject) {
			this.dataObject = dataObject;
			BindInternal(dataObject);
			return this;
		}
	}
}
