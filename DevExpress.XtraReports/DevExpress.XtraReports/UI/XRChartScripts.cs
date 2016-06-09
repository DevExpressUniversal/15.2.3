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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraReports.Native;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UI {
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts")]
	public class XRChartScripts : XRControlScripts {
		string customDrawSeries = String.Empty;
		string customDrawSeriesPoint = String.Empty;
		string customDrawCrosshair = String.Empty;
		string customDrawAxisLabel = String.Empty;
		string customPaint = String.Empty;
		string boundDataChanged = String.Empty;
		string pieSeriesPointExploded = String.Empty;
		string axisScaleChanged = String.Empty;
		string axisWholeRangeChanged = String.Empty;
		string axisVisualRangeChanged = String.Empty;
		public XRChartScripts(XRControl control)
			: base(control) {
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnCustomDrawSeries"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnCustomDrawSeries"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.CustomDrawSeries),
		XtraSerializableProperty,
		]
		public string OnCustomDrawSeries {
			get { return customDrawSeries; }
			set { customDrawSeries = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnCustomDrawSeriesPoint"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnCustomDrawSeriesPoint"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.CustomDrawSeriesPoint),
		XtraSerializableProperty,
		]
		public string OnCustomDrawSeriesPoint {
			get { return customDrawSeriesPoint; }
			set { customDrawSeriesPoint = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnCustomDrawCrosshair"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnCustomDrawCrosshair"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.CustomDrawCrosshair),
		XtraSerializableProperty,
		]
		public string OnCustomDrawCrosshair {
			get { return customDrawCrosshair; }
			set { customDrawCrosshair = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnCustomDrawAxisLabel"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnCustomDrawAxisLabel"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.CustomDrawAxisLabel),
		XtraSerializableProperty,
		]
		public string OnCustomDrawAxisLabel {
			get { return customDrawAxisLabel; }
			set { customDrawAxisLabel = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnCustomPaint"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.CustomPaint),
		XtraSerializableProperty,
		]
		public string OnCustomPaint {
			get { return customPaint; }
			set { customPaint = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnBoundDataChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnBoundDataChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.BoundDataChanged),
		XtraSerializableProperty,
		]
		public string OnBoundDataChanged {
			get { return boundDataChanged; }
			set { boundDataChanged = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnPieSeriesPointExploded"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnPieSeriesPointExploded"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.PieSeriesPointExploded),
		XtraSerializableProperty,
		]
		public string OnPieSeriesPointExploded {
			get { return pieSeriesPointExploded; }
			set { pieSeriesPointExploded = value; }
		}
		[
		Obsolete,
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public string OnDateTimeMeasurementUnitsCalculated {
			get { return string.Empty; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return textChanged; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnAxisScaleChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnAxisScaleChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.AxisScaleChanged),
		XtraSerializableProperty,
		]
		public string OnAxisScaleChanged {
			get { return axisScaleChanged; }
			set { axisScaleChanged = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnAxisWholeRangeChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnAxisWholeRangeChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.AxisWholeRangeChanged),
		XtraSerializableProperty,
		]
		public string OnAxisWholeRangeChanged {
			get { return axisWholeRangeChanged; }
			set { axisWholeRangeChanged = value; }
		}
		 [
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScriptsOnAxisVisualRangeChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChartScripts.OnAxisVisualRangeChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRChart), XRChart.EventNames.AxisVisualRangeChanged),
		XtraSerializableProperty,
		]
		public string OnAxisVisualRangeChanged {
			 get { return axisVisualRangeChanged; }
			 set { axisVisualRangeChanged = value; }
		}
	}
}
