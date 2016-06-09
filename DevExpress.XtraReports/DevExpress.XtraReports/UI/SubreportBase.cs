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
using System.Drawing;
using DevExpress.XtraReports.Localization;
using System;
using System.ComponentModel.Design;
using System.Reflection;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.Printing;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
namespace DevExpress.XtraReports.UI {
	[
	DefaultProperty("ReportSource"),
	ToolboxItem(false),
	]
	public abstract class SubreportBase : XRControl, IXtraSupportCreateContentPropertyValue {
		#region fields & properties
		private XtraReport source;
		bool scriptsInitialized;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseReportSource"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.SubreportBase.ReportSource"),
		SRCategory(ReportStringId.CatData),
		Editor("DevExpress.XtraReports.Design.ReportSourceEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DevExpress.Utils.Serializing.XtraSerializableProperty(DevExpress.Utils.Serializing.XtraSerializationVisibility.Content, true, false, false, 0, DevExpress.Utils.Serializing.XtraSerializationFlags.Cached)
		]
		public XtraReport ReportSource {
			get {
				return source != null && source.IsDisposed ? null : source;
			}
			set {
				if(source != value) {
					scriptsInitialized = false;
					XtraReport oldValue = ReportSource;
					source = value;
					ValidateReportSource(oldValue);
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseScripts"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.SubreportBase.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new SubreportBaseScripts Scripts { get { return (SubreportBaseScripts)fEventScripts; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseText"),
#endif
		Bindable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseTextAlignment"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override TextAlignment TextAlignment { get { return base.TextAlignment; } set { base.TextAlignment = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseWordWrap"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap { get { return base.WordWrap; } set { base.WordWrap = value; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool KeepTogether { get { return false; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string NavigateUrl { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Target { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Bookmark { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override XRControl BookmarkParent { get { return null; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseStyles"),
#endif
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override XRControlStyles Styles { get { return base.Styles; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string StyleName { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseEvenStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string EvenStyleName { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseOddStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OddStyleName { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SubreportBaseDataBindings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override XRBindingCollection DataBindings { get { return base.DataBindings; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override PaddingInfo Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool CanPublish { get { return true; } set { } }
		protected internal override bool HasUndefinedBounds { get { return true; } }
		protected override bool CanHaveExportWarning { get { return false; } }
		protected internal override bool IsNavigateTarget { get { return false; } }
		#endregion
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event BindingEventHandler EvaluateBinding { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PrintOnPageEventHandler PrintOnPage { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseMove { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseDown { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseUp { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewClick { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewDoubleClick { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event DrawEventHandler Draw { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event HtmlEventHandler HtmlItemCreated { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		#endregion
		protected SubreportBase() {
		}
		protected override XRControlScripts CreateScripts() {
			return new SubreportBaseScripts(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void LoadSubreportLayout(string reportSourceTypeName, XtraReport reportLayout) {
			try {
				Type type = Type.GetType(reportSourceTypeName);
				if(type == null)
					type = TypeResolver.GetType(reportSourceTypeName);
				if(type != null) {
					XtraReport reportSource = Activator.CreateInstance(type) as XtraReport;
					reportSource.CopyFrom(reportLayout, true);
					ReportSource = reportSource;
				}
			} catch {
				ReportSource = reportLayout;
			}
		}
		protected virtual void ValidateReportSource(XtraReport reportOnException) {
			if(ReportSource != null) {
				if(ReportSource == RootReport) {
					ReportSource = reportOnException;
					ThrowInvalidReportSourceException();
				}
				Type reportType = DesignMode ? GetReportType() :
					Report != null ? Report.GetType() :
					null;
				if(reportType != null && Attribute.GetCustomAttribute(reportType, typeof(RootClassAttribute)) != null)
					reportType = null;
				ReportSource.MasterReport = RootReport;
			}
		}
		protected static void ThrowInvalidReportSourceException() {
			Exception ex = new Exception(ReportStringId.Msg_InvalidReportSource.GetString());
			DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
			throw ex;
		}
		protected internal override void OnReportInitialize() {
			base.OnReportInitialize();
			ValidateReportSource(null);
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			scriptsInitialized = false;
		}
		protected internal virtual void ForceReportSource() {
		}
		protected internal override void OnBeforePrint(System.Drawing.Printing.PrintEventArgs e) {
			base.OnBeforePrint(e);
			if(ReportSource != null)
				ReportSource.MasterReport = RootReport;
			if(!scriptsInitialized && ReportSource != null) {
				ReportSource.InitializeScripts();
				scriptsInitialized = true;
			}
		}
		protected internal override void OnReportEndDeserializing() {
			base.OnReportEndDeserializing();
			if(ReportSource != null)
				ReportSource.OnReportEndDeserializing();
		}
		protected override void WriteContentTo(XRWriteInfo writeInfo, VisualBrick brick) {
			if(writeInfo != null)
				if(ReportSource != null && brick is SubreportBrick) {
					System.Drawing.Printing.PrintEventArgs args = new System.Drawing.Printing.PrintEventArgs();
					ReportSource.OnBeforePrint(args);
					if(args.Cancel) {
						brick.IsVisible = false;
						IncrementProgressValue(writeInfo.PrintingSystem.ProgressReflector);
					} else {
						SelfGeneratedSubreportDocumentBand docBand = CreateDocumentBand();
						SubreportBuilder builder = new SubreportBuilder(ReportSource, docBand, Band is PageBand || Band is MarginBand);
						docBand.BandManager = builder;
						docBand.SetDataSource(ReportSource.DataSource);
						ReportSource.WriteToDocument(builder, writeInfo.PrintingSystem);
						((SubreportBrick)brick).DocumentBand = docBand;
					}
					base.WriteContentToCore(writeInfo, brick);
				} else {
					base.WriteContentToCore(writeInfo, CreateInitializedBrick(null, writeInfo));
					IncrementProgressValue(writeInfo.PrintingSystem.ProgressReflector);
				}
		}
		void IncrementProgressValue(ProgressReflector progressReflector) {
			XRProgressReflectorLogic logic = progressReflector.Logic as XRProgressReflectorLogic;
			if(logic != null) {
				logic.InitializeRange(this, 1);
				logic.MaximizeRange(this);
			}
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new SubreportBrick(this);
		}
		protected override float CalculateBrickHeight(VisualBrick brick) {
			return 0;
		}
		protected virtual SelfGeneratedSubreportDocumentBand CreateDocumentBand() {
			return new SelfGeneratedSubreportDocumentBand(ReportSource, 0f, this);
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			components.Add(ReportSource);
		}
		protected internal override void CalculatePageSummary(DocumentBand pageBand) {
			if(ReportSource != null && ReportSource.Summaries != null)
				ReportSource.CalculatePageSummary(pageBand);
		}
		protected internal override void FinishPageSummary() {
			if(ReportSource != null && ReportSource.Summaries != null)
				ReportSource.FinishPageSummary();
		}
		private Type GetReportType() {
			IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			Assembly asm = Assembly.GetAssembly(ReportSource.GetType());
			string type = "";
			if(designerHost != null && asm != null) {
				string rootComponentClassName = designerHost.RootComponentClassName;
				if(rootComponentClassName.IndexOf(".") > 0)
					type = rootComponentClassName;
				else
					type = String.Format("{0}.{1}", asm.GetName().Name, rootComponentClassName);
				return asm.GetType(type);
			} else
				return null;
		}
		bool ShouldSerializeReportSource() {
			return ReportSource != null;
		}
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
		protected internal override void CopyDataProperties(XRControl control) {
			base.CopyDataProperties(control);
			XtraReportBase report = ReportSource as XtraReportBase;
			if (report == null)
				return;
			SubreportBase subReport = control as SubreportBase;
			if (subReport == null)
				return;
			XtraReportBase source = subReport.ReportSource as XtraReportBase;
			if (source == null)
				return;
			report.CopyDataProperties(source);
		}
		#region IXtraSupportCreateContentPropertyValue Members
		object IXtraSupportCreateContentPropertyValue.Create(DevExpress.Utils.Serializing.XtraItemEventArgs e) {
			if(e.Item.Name == "ReportSource") {
				Type type = Type.GetType((string)e.Item.ChildProperties["ControlType"].Value);
				XtraReport report = type == null ? new XtraReport() : (XtraReport)Activator.CreateInstance(type);
				if(report != null)
					report.ClearContent();
				return report;
			}
			return null;
		}
		#endregion
	}
}
