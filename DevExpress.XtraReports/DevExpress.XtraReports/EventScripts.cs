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
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using System.Collections.Generic;
using System.ComponentModel.Design;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UI 
{
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class XRControlEvents : XRScriptsBase {
		#region Fields & Properties
		protected string evaluateBinding = String.Empty;
		protected string afterPrint = String.Empty;
		protected string beforePrint = String.Empty;
		protected string printOnPage = String.Empty;
		protected string draw = String.Empty;
		protected string htmlItemCreated = String.Empty;
		protected string locationChanged = String.Empty;
		protected string parentChanged = String.Empty;
		protected string previewClick = String.Empty;
		protected string previewDoubleClick = String.Empty;
		protected string previewMouseDown = String.Empty;
		protected string previewMouseMove = String.Empty;
		protected string previewMouseUp = String.Empty;
		protected string sizeChanged = String.Empty;
		protected string textChanged = String.Empty;
		XRControl Control { get { return (XRControl)component; } }
		internal override ScriptLanguage ScriptLanguage {
			get { return Control != null ? Control.GetScriptLanguage() : ScriptLanguage.CSharp; }
		}
		internal override string ComponentName { get { return Control.Name; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnEvaluateBinding"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.EvaluateBinding),
		XtraSerializableProperty,
		]
		public virtual string OnEvaluateBinding { get { return evaluateBinding; } set { evaluateBinding = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnAfterPrint"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnAfterPrint"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.AfterPrint),
		XtraSerializableProperty,
		]
		public virtual string OnAfterPrint { get { return afterPrint; } set { afterPrint = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnBeforePrint"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnBeforePrint"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.BeforePrint),
		XtraSerializableProperty,
		]
		public virtual string OnBeforePrint { get { return beforePrint; } set { beforePrint = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnPrintOnPage"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnPrintOnPage"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.PrintOnPage),
		XtraSerializableProperty,
		]
		public virtual string OnPrintOnPage { get { return printOnPage; } set { printOnPage = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnDraw"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnDraw"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.Draw),
		XtraSerializableProperty,
		]
		public virtual string OnDraw { get { return draw; } set { draw = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnHtmlItemCreated"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnHtmlItemCreated"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.HtmlItemCreated),
		XtraSerializableProperty,
		]
		public virtual string OnHtmlItemCreated { get { return htmlItemCreated; } set { htmlItemCreated = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnLocationChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnLocationChanged"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.LocationChanged),
		XtraSerializableProperty,
		]
		public virtual string OnLocationChanged { get { return locationChanged; } set { locationChanged = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnParentChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnParentChanged"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.ParentChanged),
		XtraSerializableProperty,
		]
		public virtual string OnParentChanged { get { return parentChanged; } set { parentChanged = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnPreviewClick"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnPreviewClick"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.PreviewClick),
		XtraSerializableProperty,
		]
		public virtual string OnPreviewClick { get { return previewClick; } set { previewClick = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnPreviewDoubleClick"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnPreviewDoubleClick"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.PreviewDoubleClick),
		XtraSerializableProperty,
		]
		public virtual string OnPreviewDoubleClick { get { return previewDoubleClick; } set { previewDoubleClick = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnPreviewMouseDown"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnPreviewMouseDown"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.PreviewMouseDown),
		XtraSerializableProperty,
		]
		public virtual string OnPreviewMouseDown { get { return previewMouseDown; } set { previewMouseDown = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnPreviewMouseMove"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnPreviewMouseMove"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.PreviewMouseMove),
		XtraSerializableProperty,
		]
		public virtual string OnPreviewMouseMove { get { return previewMouseMove; } set { previewMouseMove = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnPreviewMouseUp"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnPreviewMouseUp"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.PreviewMouseUp),
		XtraSerializableProperty,
		]
		public virtual string OnPreviewMouseUp { get { return previewMouseUp; } set { previewMouseUp = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnSizeChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnSizeChanged"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.SizeChanged),
		XtraSerializableProperty,
		]
		public virtual string OnSizeChanged { get { return sizeChanged; } set { sizeChanged = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlEventsOnTextChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlEvents.OnTextChanged"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRControl), XRControl.EventNames.TextChanged),
		XtraSerializableProperty,
		]
		public virtual string OnTextChanged { get { return textChanged; } set { textChanged = value; } }
		#endregion
		public XRControlEvents() {
		}
		public XRControlEvents(XRControl control) : base(control) {
		}
	}
	[
	TypeConverter(typeof(DevExpress.XtraPrinting.Native.LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlScripts"),
	]
	public class XRControlScripts : XRControlEvents {
		public XRControlScripts(XRControl control) : base(control) {
		}
	}
	public class XRTableScripts : XRControlScripts {
		public XRTableScripts(XRControl control)
			: base(control) {
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnHtmlItemCreated { get { return htmlItemCreated; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return String.Empty; } set { } }
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLineScripts"),
	]
	public class XRLineScripts : XRControlScripts {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return String.Empty; } set { } }
		public XRLineScripts(XRControl control) : base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGaugeScripts"),
	]
	public class XRGaugeScripts : XRControlScripts {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return String.Empty; } set { } }
		public XRGaugeScripts(XRControl control)
			: base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSparklineScripts"),
	]
	public class XRSparklineScripts : XRControlScripts {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return String.Empty; } set { } }
		public XRSparklineScripts(XRControl control)
			: base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRShapeScripts"),
	]
	public class XRShapeScripts : XRControlScripts {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return String.Empty; } set { } }
		public XRShapeScripts(XRControl control)
			: base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPictureboxScripts"),
	]
	public class XRPictureboxScripts : XRControlScripts {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return String.Empty; } set { } }
		public XRPictureboxScripts(XRControl control)
			: base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPanelScripts"),
	]
	public class XRPanelScripts : XRControlScripts {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return String.Empty; } set { } }
		public XRPanelScripts(XRControl control)
			: base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabelScripts"),
	]
	public class XRLabelScripts : XRControlScripts {
		string summaryCalculated = string.Empty;
		string summaryGetResult = string.Empty;
		string summaryReset = string.Empty;
		string summaryRowChanged = string.Empty;
		public XRLabelScripts(XRControl control) : base(control) {
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelScriptsOnSummaryCalculated"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabelScripts.OnSummaryCalculated"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRLabel), XRLabel.EventNames.SummaryCalculated),
		XtraSerializableProperty,
		]
		public string OnSummaryCalculated { get { return summaryCalculated; } set { summaryCalculated = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelScriptsOnSummaryGetResult"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabelScripts.OnSummaryGetResult"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRLabel), XRLabel.EventNames.SummaryGetResult),
		XtraSerializableProperty,
		]
		public string OnSummaryGetResult { get { return summaryGetResult; } set { summaryGetResult = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelScriptsOnSummaryReset"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabelScripts.OnSummaryReset"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRLabel), XRLabel.EventNames.SummaryReset),
		XtraSerializableProperty,
		]
		public string OnSummaryReset { get { return summaryReset; } set { summaryReset = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelScriptsOnSummaryRowChanged"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabelScripts.OnSummaryRowChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRLabel), XRLabel.EventNames.SummaryRowChanged),
		XtraSerializableProperty,
		]
		public string OnSummaryRowChanged { get { return summaryRowChanged; } set { summaryRowChanged = value; } }
	}
	public class TruncatedControlScripts : XRControlScripts {
		#region Fields & Properties
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnEvaluateBinding { get { return evaluateBinding; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnDraw { get { return draw; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnHtmlItemCreated { get { return htmlItemCreated; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnPreviewClick { get { return previewClick; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnPreviewDoubleClick { get { return previewDoubleClick; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnPreviewMouseDown { get { return previewMouseDown; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnPreviewMouseMove { get { return previewMouseMove; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnPreviewMouseUp { get { return previewMouseUp; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnTextChanged { get { return textChanged; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnPrintOnPage { get { return printOnPage; } set { } }
		#endregion
		public TruncatedControlScripts(XRControl control) : base(control) {
		}
	}
	public class SubreportBaseScripts : TruncatedControlScripts {
		internal SubreportBaseScripts(SubreportBase control)
			: base(control) {
		}
	}
	public class XRPageBreakScripts : TruncatedControlScripts {
		#region Fields & Properties
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnSizeChanged { get { return sizeChanged; } set { } }
		#endregion
		internal XRPageBreakScripts(XRPageBreak control)
			: base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.BandScripts"),
	]
	public class BandScripts : TruncatedControlScripts {
		#region Fields & Properties
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnLocationChanged { get { return locationChanged; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnParentChanged { get { return parentChanged; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnPrintOnPage { get { return parentChanged; } set {} }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OnSizeChanged { get { return sizeChanged; } set { } }
		#endregion
		internal BandScripts(XRControl control)
			: base(control) {
		}
	}
	public class GroupBandScripts : BandScripts {
		#region Fields & Properties
		string bandLevelChanged = string.Empty;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupBandScriptsOnBandLevelChanged"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupBandScripts.OnBandLevelChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(GroupBand), GroupBand.EventNames.BandLevelChanged),
		XtraSerializableProperty,
		]
		public virtual string OnBandLevelChanged { get { return bandLevelChanged; } set { bandLevelChanged = value; } }
		#endregion
		internal GroupBandScripts(GroupBand control)
			: base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupHeaderBandScripts"),
	]
	public class GroupHeaderBandScripts : GroupBandScripts {
		string sortingSummaryGetResult = string.Empty;
		string sortingSummaryReset = string.Empty;
		string sortingSummaryRowChanged = string.Empty;
		internal GroupHeaderBandScripts(GroupHeaderBand control)
			: base(control) {
		}
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupHeaderBandScripts.OnSortingSummaryGetResult"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(GroupHeaderBand), GroupHeaderBand.EventNames.SortingSummaryGetResult),
		XtraSerializableProperty,
		]
		public string OnSortingSummaryGetResult { get { return sortingSummaryGetResult; } set { sortingSummaryGetResult = value; } }
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupHeaderBandScripts.OnSortingSummaryReset"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(GroupHeaderBand), GroupHeaderBand.EventNames.SortingSummaryReset),
		XtraSerializableProperty,
		]
		public string OnSortingSummaryReset { get { return sortingSummaryReset; } set { sortingSummaryReset = value; } }
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupHeaderBandScripts.OnSortingSummaryRowChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(GroupHeaderBand), GroupHeaderBand.EventNames.SortingSummaryRowChanged),
		XtraSerializableProperty,
		]
		public string OnSortingSummaryRowChanged { get { return sortingSummaryRowChanged; } set { sortingSummaryRowChanged = value; } }
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts"),
	]
	public class XtraReportBaseScripts : BandScripts {
		#region Fields & Properties
		protected string bandHeightChanged = String.Empty;
		protected string dataSourceRowChanged = String.Empty;
		string dataSourceDemanded = String.Empty;
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts.OnDataSourceDemanded"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XtraReportBase), XRControl.EventNames.DataSourceDemanded),
		XtraSerializableProperty,
		]
		public string OnDataSourceDemanded { get { return dataSourceDemanded; } set { dataSourceDemanded = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseScriptsOnBandHeightChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts.OnBandHeightChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XtraReportBase), XRControl.EventNames.BandHeightChanged),
		XtraSerializableProperty,
		]
		public string OnBandHeightChanged { get { return bandHeightChanged; } set { bandHeightChanged = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseScriptsOnDataSourceRowChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts.OnDataSourceRowChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XtraReportBase), XRControl.EventNames.DataSourceRowChanged),
		XtraSerializableProperty,
		]
		public string OnDataSourceRowChanged { get { return dataSourceRowChanged; } set { dataSourceRowChanged = value; } }
		#endregion
		internal XtraReportBaseScripts(XRControl control)
			: base(control) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts"),
	]
	public class XtraReportScripts : XtraReportBaseScripts {
		#region Fields & Properties
		protected string fillEmptySpace = String.Empty;
		string printProgress = String.Empty;
		string parametersRequestBeforeShow = String.Empty;
		string parametersRequestValueChanged = String.Empty;
		string parametersRequestSubmit = String.Empty;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScriptsOnFillEmptySpace"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts.OnFillEmptySpace"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XtraReport), XRControl.EventNames.FillEmptySpace),
		XtraSerializableProperty,
		]
		public string OnFillEmptySpace { get { return fillEmptySpace; } set { fillEmptySpace = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScriptsOnPrintProgress"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts.OnPrintProgress"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XtraReport), XRControl.EventNames.PrintProgress),
		XtraSerializableProperty,
		]
		public string OnPrintProgress { get { return printProgress; } set { printProgress = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScriptsOnParametersRequestBeforeShow"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts.OnParametersRequestBeforeShow"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XtraReport), XtraReport.EventNames.ParametersRequestBeforeShow),
		XtraSerializableProperty,
		]
		public string OnParametersRequestBeforeShow { get { return parametersRequestBeforeShow; } set { parametersRequestBeforeShow = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScriptsOnParametersRequestValueChanged"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts.OnParametersRequestValueChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XtraReport), XtraReport.EventNames.ParametersRequestValueChanged),
		XtraSerializableProperty,
		]
		public string OnParametersRequestValueChanged { get { return parametersRequestValueChanged; } set { parametersRequestValueChanged = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScriptsOnParametersRequestSubmit"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportScripts.OnParametersRequestSubmit"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XtraReport), XtraReport.EventNames.ParametersRequestSubmit),
		XtraSerializableProperty,
		]
		public string OnParametersRequestSubmit { get { return parametersRequestSubmit; } set { parametersRequestSubmit = value; } }
		#endregion
		internal XtraReportScripts(XRControl control) : base(control) {
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class XRSummaryEvents : XRScriptsBase {
		#region Fields & Properties
		string rowChanged = String.Empty;
		string getResult = String.Empty;
		string reset = String.Empty;
		internal override ScriptLanguage ScriptLanguage {
			get { return ScriptLanguage.CSharp; }
		}
		internal override string ComponentName { get { return string.Empty; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSummaryEvents.OnRowChanged"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		MethodScriptAttribute(typeof(XRSummary.EventMethods), XRSummary.EventNames.RowChanged),
		XtraSerializableProperty,
		]
		public string OnRowChanged { get { return rowChanged; } set { rowChanged = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSummaryEvents.OnGetResult"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		MethodScriptAttribute(typeof(XRSummary.EventMethods), XRSummary.EventNames.GetResult),
		XtraSerializableProperty,
		]
		public string OnGetResult { get { return getResult; } set { getResult = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSummaryEvents.OnReset"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		MethodScriptAttribute(typeof(XRSummary.EventMethods), XRSummary.EventNames.Reset),
		XtraSerializableProperty,
		]
		public string OnReset { get { return reset; } set { reset = value; } }
		#endregion
		public XRSummaryEvents() {
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class XRSummaryScripts : XRSummaryEvents {
		public XRSummaryScripts() {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedFieldScripts"),
	TypeConverter(typeof(DevExpress.XtraPrinting.Native.LocalizableObjectConverter)),
	]
	public class CalculatedFieldScripts : XRScriptsBase {
		#region Fields & Properties
		string getValue = String.Empty;
		CalculatedField CalculatedField { get { return (CalculatedField)component; } }
		internal override ScriptLanguage ScriptLanguage {
			get { return CalculatedField != null ? CalculatedField.Report.GetScriptLanguage() : ScriptLanguage.CSharp; }
		}
		internal override string ComponentName { get { return CalculatedField.Name; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("CalculatedFieldScriptsOnGetValue"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedFieldScripts.OnGetValue"),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(CalculatedField), CalculatedField.EventNames.GetValue),
		XtraSerializableProperty,
		]
		public string OnGetValue { get { return getValue; } set { getValue = value; } }
		#endregion
		public CalculatedFieldScripts(CalculatedField calculatedField) : base(calculatedField) {
		}
	}
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts"),
	]
	public class XRPivotGridScripts : TruncatedControlScripts {
		string customCellDisplayText = string.Empty;
		string customCellValue = string.Empty;
		string customColumnWidth = string.Empty;
		string customFieldSort = string.Empty;
		string customServerModeSort = string.Empty;
		string customFieldValueCells = string.Empty;
		string customGroupInterval = string.Empty;
		string customChartDataSourceData = string.Empty;
		string customChartDataSourceRows = string.Empty;
		string customRowHeight = string.Empty;
		string customSummary = string.Empty;
		string customUnboundFieldData = string.Empty;
		string fieldValueDisplayText = string.Empty;
		string prefilterCriteriaChanged = string.Empty;
		string printCell = string.Empty;
		string printFieldValue = string.Empty;
		string printHeader = string.Empty;
		internal XRPivotGridScripts(XRPivotGrid control)
			: base(control) {
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnFieldValueDisplayText"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnFieldValueDisplayText"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.FieldValueDisplayText),
		XtraSerializableProperty,
		]
		public string OnFieldValueDisplayText { get { return fieldValueDisplayText; } set { fieldValueDisplayText = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnPrintCell"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnPrintCell"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.PrintCell),
		XtraSerializableProperty,
		]
		public string OnPrintCell { get { return printCell; } set { printCell = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnPrintFieldValue"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnPrintFieldValue"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.PrintFieldValue),
		XtraSerializableProperty,
		]
		public string OnPrintFieldValue { get { return printFieldValue; } set { printFieldValue = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnPrintHeader"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnPrintHeader"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.PrintHeader),
		XtraSerializableProperty,
		]
		public string OnPrintHeader { get { return printHeader; } set { printHeader = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnCustomSummary"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomSummary"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomSummary),
		XtraSerializableProperty,
		]
		public string OnCustomSummary { get { return customSummary; } set { customSummary = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnCustomGroupInterval"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomGroupInterval"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomGroupInterval),
		XtraSerializableProperty,
		]
		public string OnCustomGroupInterval { get { return customGroupInterval; } set { customGroupInterval = value; } }
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomChartDataSourceData"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomChartDataSourceData),
		XtraSerializableProperty,
		]
		public string OnCustomChartDataSourceData { get { return customChartDataSourceData; } set { customChartDataSourceData = value; } }
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomChartDataSourceRows"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomChartDataSourceRows),
		XtraSerializableProperty,
		]
		public string OnCustomChartDataSourceRows { get { return customChartDataSourceRows; } set { customChartDataSourceRows = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnCustomCellDisplayText"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomCellDisplayText"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomCellDisplayText),
		XtraSerializableProperty,
		]
		public string OnCustomCellDisplayText { get { return customCellDisplayText; } set { customCellDisplayText = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnCustomCellValue"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomCellValue"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomCellValue),
		XtraSerializableProperty,
		]
		public string OnCustomCellValue { get { return customCellValue; } set { customCellValue = value; } }
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomColumnWidth"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomColumnWidth),
		XtraSerializableProperty,
		]
		public string OnCustomColumnWidth { get { return customColumnWidth; } set { customColumnWidth = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnCustomFieldSort"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomFieldSort"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomFieldSort),
		XtraSerializableProperty,
		]
		public string OnCustomFieldSort { get { return customFieldSort; } set { customFieldSort = value; } }
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomServerModeSort"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomServerModeSort),
		XtraSerializableProperty,
		]
		public string OnCustomServerModeSort { get { return customServerModeSort; } set { customServerModeSort = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnCustomFieldValueCells"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomFieldValueCells"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomFieldValueCells),
		XtraSerializableProperty,
		]
		public string OnCustomFieldValueCells { get { return customFieldValueCells; } set { customFieldValueCells = value; } }
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomRowHeight"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomRowHeight),
		XtraSerializableProperty,
		]
		public string OnCustomRowHeight { get { return customRowHeight; } set { customRowHeight = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnCustomUnboundFieldData"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomUnboundFieldData"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.CustomUnboundFieldData),
		XtraSerializableProperty,
		]
		public string OnCustomUnboundFieldData { get { return customUnboundFieldData; } set { customUnboundFieldData = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScriptsOnPrefilterCriteriaChanged"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGridScripts.OnPrefilterCriteriaChanged"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(XRPivotGrid), XRPivotGrid.EventNames.PrefilterCriteriaChanged),
		XtraSerializableProperty,
		]
		public string OnPrefilterCriteriaChanged { get { return prefilterCriteriaChanged; } set { prefilterCriteriaChanged = value; } }
	}
}
namespace DevExpress.XtraReports.Native {
	using System.Reflection;
	[AttributeUsage(AttributeTargets.Property)]
	public class MethodScriptAttribute : Attribute {
		MethodInfo mi; 
		string name;
		public string Name { get { return name; } }
		public MethodScriptAttribute(Type type, string name) {
			this.name = name;
			this.mi = GetMethodInfo(type);
		}
		protected virtual MethodInfo GetMethodInfo(Type type) {
			return type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
		}
		public string GetDefaultValue(string prefix, ScriptLanguage language) {
			return ScriptGenerator.GenerateMethod(mi, prefix + name, language);
		}
		public string GetDefaultValueByName(string procedureName, ScriptLanguage language) {
			return ScriptGenerator.GenerateMethod(mi, procedureName, language);
		}
	}
	public sealed class EventScriptAttribute : MethodScriptAttribute {
		public EventScriptAttribute(Type type,  string eventName) : base(type, eventName) {
		}
		protected override MethodInfo GetMethodInfo(Type type) {
			EventInfo ei = type.GetEvent(Name);
			return ei.EventHandlerType.GetMethod("Invoke");
		}
	}
}
