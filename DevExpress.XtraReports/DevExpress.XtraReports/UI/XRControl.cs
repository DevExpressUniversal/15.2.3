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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Pdf;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel.Design;
using System.Reflection;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraReports.Native.Presenters;
using DevExpress.XtraReports.Native.LayoutView;
using DevExpress.XtraPrinting.BrickExporters;
using System.Collections.ObjectModel;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(false),
	XRDesigner("DevExpress.XtraReports.Design.XRControlDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRControlDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultBindableProperty("Tag"),
	DefaultProperty("BackColor"),
	DefaultCollectionName("Controls"),
	ToolboxItemFilter(Design.AttributeSR.ToolboxItemFilter, ToolboxItemFilterType.Require),
	DesignerSerializer("DevExpress.XtraReports.Design.XRControlCodeDomSerializer," + AssemblyInfo.SRAssemblyReportsExtensionsFull, AttributeConstants.CodeDomSerializer),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRControl.bmp"),
	System.ComponentModel.DefaultEventAttribute("BeforePrint"),
	]
	public class XRControl : Component, IXRSerializable, IEnumerable, IBrickOwner, IScriptable, IXtraSupportDeserializeCollectionItem, IXtraSerializable {
		#region inner classes
		[
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStylesConverter)),
		Editor("DevExpress.XtraReports.Design.XRControlStylesEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor))
		]
		public class XRControlStyles : IDisposable {
			protected XRControl control;
			internal XRControl Owner {
				get {
					return control;
				}
			}
			[
			DefaultValue(null),
			Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			RefreshProperties(RefreshProperties.All),
			TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
			DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.XRControlStyles.EvenStyle"),
			]
			public virtual XRControlStyle EvenStyle {
				get { return control.EvenStyleCore; }
				set { control.EvenStyleCore = value; }
			}
			[
			DefaultValue(null),
			Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			RefreshProperties(RefreshProperties.All),
			TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
			DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.XRControlStyles.OddStyle"),
			]
			public virtual XRControlStyle OddStyle {
				get { return control.OddStyleCore; }
				set { control.OddStyleCore = value; }
			}
			[
			DefaultValue(null),
			Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			RefreshProperties(RefreshProperties.All),
			TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
			DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.XRControlStyles.Style"),
			]
			public virtual XRControlStyle Style {
				get { return control.StyleCore; }
				set { control.StyleCore = value; }
			}
			public XRControlStyles(XRControl control) {
				System.Diagnostics.Debug.Assert(control != null);
				this.control = control;
			}
			public void Dispose() {
				this.control = null;
			}
		}
		public class XRBandStyles : XRControlStyles {
			[
			Browsable(false),
			]
			public override XRControlStyle EvenStyle {
				get { return base.EvenStyle; }
				set { base.EvenStyle = value; }
			}
			[
			Browsable(false)
			]
			public override XRControlStyle OddStyle {
				get { return base.OddStyle; }
				set { base.OddStyle = value; }
			}
			public XRBandStyles(XRControl control)
				: base(control) {
			}
		}
		public static class EventNames {
			public const string HandlerPrefix = "On";
			public const string EvaluateBinding = "EvaluateBinding";
			public const string BeforePrint = "BeforePrint";
			public const string AfterPrint = "AfterPrint";
			public const string PrintOnPage = "PrintOnPage";
			public const string LocationChanged = "LocationChanged";
			public const string FillEmptySpace = "FillEmptySpace";
			public const string SizeChanged = "SizeChanged";
			public const string TextChanged = "TextChanged";
			public const string ParentChanged = "ParentChanged";
			public const string Draw = "Draw";
			public const string HtmlItemCreated = "HtmlItemCreated";
			public const string PreviewClick = "PreviewClick";
			public const string PreviewDoubleClick = "PreviewDoubleClick";
			public const string PreviewMouseMove = "PreviewMouseMove";
			public const string PreviewMouseDown = "PreviewMouseDown";
			public const string PreviewMouseUp = "PreviewMouseUp";
			public const string WinControlChanged = "WinControlChanged";
			public const string PrintProgress = "PrintProgress";
			public const string BandHeightChanged = "BandHeightChanged";
			public const string BandLevelChanged = "BandLevelChanged";
			public const string DataSourceRowChanged = "DataSourceRowChanged";
			public const string DataSourceDemanded = "DataSourceDemanded";
			public const string ParametersRequestBeforeShow = "ParametersRequestBeforeShow";
			public const string ParametersRequestValueChanged = "ParametersRequestValueChanged";
			public const string ParametersRequestSubmit = "ParametersRequestSubmit";
			public const string DesignerLoaded = "DesignerLoaded";
			public const string SaveComponents = "SaveComponents";
		}
		protected class XRControlParentInfo : IDisposable {
			XtraReport rootReport;
			XtraReportBase report;
			Band band;
			XRControl control;
			int nestedLevel = int.MinValue;
			public XRControlParentInfo(XRControl control) {
				if(control == null)
					throw new ArgumentNullException("control");
				this.control = control;
			}
			public Band GetBand() {
				if(band == null)
					band = (Band)GetParent(typeof(Band));
				return band;
			}
			public XtraReportBase GetReport() {
				if(report == null)
					report = (XtraReportBase)GetParent(typeof(XtraReportBase));
				return report;
			}
			public XtraReport GetRootReport() {
				if(rootReport == null)
					rootReport = (XtraReport)GetParent(typeof(XtraReport));
				return rootReport;
			}
			public int GetNestedLevel() {
				if(nestedLevel == int.MinValue) {
					nestedLevel = -1;
					XRControl parent = control.Parent;
					while(parent != null) {
						parent = parent.Parent;
						nestedLevel++;
					}
				}
				return nestedLevel;
			}
			XRControl GetParent(Type type) {
				return FindParent(parent => type.IsAssignableFrom(parent.GetType()));
			}
			public XRControl FindParent(Predicate<XRControl> predicate) {
				for(XRControl parent = control.fParent; parent != null; parent = parent.Parent)
					if(predicate(parent))
						return parent;
				return null;
			}
			public void Clear() {
				band = null;
				report = null;
				rootReport = null;
				nestedLevel = int.MinValue;
			}
			public void Dispose() {
				Clear();
				control = null;
			}
		}
		#endregion
		#region static
		protected static readonly Version v8_1 = new Version(8, 1);
		protected static readonly Version v9_1 = new Version(9, 1);
		protected static readonly Version v9_3 = new Version(9, 3);
		protected static readonly Version v10_2 = new Version(10, 2);
		protected static readonly Version v12_2 = new Version(12, 2);
		static PaddingInfo defaultSnapLineMargin = new PaddingInfo(0, 0, 0, 0, 100);
		static PaddingInfo defaultSnapLinePadding = new PaddingInfo(10, 10, 10, 10, 100);
		static int defaultRowSpan = 1;
		static readonly object initialValue = new object();
		static readonly int bitVisible = BitVector32.CreateMask();
		static readonly int bitWordWrap = BitVector32.CreateMask(bitVisible);
		static readonly int bitBoundsChanging = BitVector32.CreateMask(bitWordWrap);
		static readonly int bitIgnoreEvenOddStyles = BitVector32.CreateMask(bitBoundsChanging);
		static readonly int bitIsDisposed = BitVector32.CreateMask(bitIgnoreEvenOddStyles);
		static readonly int bitLockedInUserDesigner = BitVector32.CreateMask(bitIsDisposed);
		static readonly int bitKeepTogether = BitVector32.CreateMask(bitLockedInUserDesigner);
		static readonly int bitIsDisposing = BitVector32.CreateMask(bitKeepTogether);
		static readonly int bitCanPublish = BitVector32.CreateMask(bitIsDisposing);
		static Hashtable brushHT = new Hashtable();
		const ValueSuppressType defaultProcessNullValues = ValueSuppressType.Leave;
		const ProcessDuplicatesMode defaultProcessDuplicatesMode = ProcessDuplicatesMode.Leave;
		const ProcessDuplicatesTarget defaultProcessDuplicatesTarget = ProcessDuplicatesTarget.Value;
		const StringTrimming defaultTextTrimming = StringTrimming.Character;
		const int minWidthPx = 2;
		const int minHeightPx = 2;
		protected static PaddingInfo textPadding = new PaddingInfo(2, 2, 0, 0, GraphicsDpi.DeviceIndependentPixel);
		internal static int GetMinimumWidth(float dpi) {
			return XRConvert.Convert(minWidthPx, GraphicsDpi.DeviceIndependentPixel, dpi);
		}
		internal static int GetMinimumHeight(float dpi) {
			return XRConvert.Convert(minHeightPx, GraphicsDpi.DeviceIndependentPixel, dpi);
		}
		internal static bool SpecifiedEquals(BoundsSpecified specified, params BoundsSpecified[] items) {
			foreach(BoundsSpecified item in items)
				if(specified == item) return true;
			return false;
		}
		internal static bool SpecifiedContains(BoundsSpecified specified, params BoundsSpecified[] items) {
			foreach(BoundsSpecified item in items)
				if((specified & item) > 0) return true;
			return false;
		}
		internal static Brush GetBrush(Color color) {
			int key = color.GetHashCode();
			lock(brushHT) {
				if(!brushHT.Contains(key))
					brushHT.Add(key, new SolidBrush(color));
			}
			return (Brush)brushHT[key];
		}
		internal static RectangleF ApplyPadding(RectangleF rect, PaddingInfo padding, float rectDpi) {
			return padding.Deflate(rect, rectDpi);
		}
		internal static XRControl FromStreamInternal(Stream stream, string name) {
			XtraReport sourceReport = ReportCompiler.Compile(stream);
			FieldInfo fi = sourceReport.GetType().GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			return fi != null ? (XRControl)fi.GetValue(sourceReport) : sourceReport;
		}
		static void AddAttachedStyle(List<XRControlStyle> attachedStyles, XRControlStyle style) {
			if(style != null && !attachedStyles.Contains(style))
				attachedStyles.Add(style);
		}
		static void AddAttachedRules(List<FormattingRule> attachedRules, FormattingRuleCollection rules) {
			foreach(FormattingRule rule in rules)
				if(rule != null && !attachedRules.Contains(rule))
					attachedRules.Add(rule);
		}
		static XRControlStyle ValidateStyle(XRControlStyle style) {
			return style != null && style.IsDisposed ? null : style;
		}
		[System.Security.SecuritySafeCritical]
		protected static SoapFormatter CreateSoapFormatter() {
			SoapFormatter formatter = new SoapFormatter();
			formatter.AssemblyFormat = FormatterAssemblyStyle.Full;
			formatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
			return formatter;
		}
		protected internal static RectangleF ApplyOffset(RectangleF rect, GraphicsUnit pageUnit) {
			float offset = XRConvert.Convert(2, GraphicsUnit.Pixel, pageUnit);
			rect.Inflate(-offset, 0);
			return rect;
		}
		protected static RectangleF ApplyPadding(RectangleF clientRect, GraphicsUnit pageUnit, PaddingInfo padding) {
			return ApplyPadding(clientRect, padding, GraphicsDpi.UnitToDpi(pageUnit));
		}
		static void CollectStyle(XRControlStyle resultStyle, XRControl control, StyleProperty properties) {
			int rowIndex = control.Report != null ? control.Report.GetEffectiveRowIndex() : -1;
			XRControlStyle ssStyle = control.GetStyleSheetStyle(rowIndex);
			XRControlStyle conditionStyle;
			if(control.formattingRules.TryGetEffectiveStyle(out conditionStyle))
				conditionStyle.ApplyProperties(resultStyle, conditionStyle.SetProperties, ref properties);
			if(ssStyle != null) {
				ssStyle.ApplyProperties(resultStyle, ssStyle.SetProperties & control.stylePriority.SetProperties, ref properties);
				control.fStyle.ApplyProperties(resultStyle, control.fStyle.SetProperties, ref properties);
				ssStyle.ApplyProperties(resultStyle, ssStyle.SetProperties, ref properties);
			} else
				control.fStyle.ApplyProperties(resultStyle, control.fStyle.SetProperties, ref properties);
			if(properties != StyleProperty.None && control.Parent != null)
				CollectStyle(resultStyle, control.Parent, properties);
		}
		#endregion
		#region Events
		private static readonly object LocationChangedEvent = new object();
		private static readonly object SizeChangedEvent = new object();
		private static readonly object TextChangedEvent = new object();
		private static readonly object DrawEvent = new object();
		private static readonly object BrickCreatedEvent = new object();
		private static readonly object HtmlItemCreatedEvent = new object();
		private static readonly object PreviewMouseMoveEvent = new object();
		private static readonly object PreviewMouseDownEvent = new object();
		private static readonly object PreviewMouseUpEvent = new object();
		private static readonly object PreviewClickEvent = new object();
		private static readonly object PreviewDoubleClickEvent = new object();
		static readonly object BeforePrintInternalEvent = new object();
		static readonly object BeforePrintEvent = new object();
		static readonly object AfterPrintEvent = new object();
		static readonly object PrintOnPageEvent = new object();
		static readonly object ParentChangedEvent = new object();
		static readonly object EvaluateBindingEvent = new object();
		static readonly object DisposingEvent = new object();
		internal event EventHandler Disposing {
			add { Events.AddHandler(DisposingEvent, value); }
			remove { Events.RemoveHandler(DisposingEvent, value); }
		}
		void RaiseDisposing(EventArgs e) {
			EventHandler handler = (EventHandler)Events[DisposingEvent];
			if(handler != null)
				handler(this, e);
		}
		public virtual event BindingEventHandler EvaluateBinding {
			add { Events.AddHandler(EvaluateBindingEvent, value); }
			remove { Events.RemoveHandler(EvaluateBindingEvent, value); }
		}
		protected internal virtual void OnEvaluateBinding(BindingEventArgs e) {
			RunEventScript(EvaluateBindingEvent, EventNames.EvaluateBinding, e);
			BindingEventHandler handler = (BindingEventHandler)Events[EvaluateBindingEvent];
			if(handler != null && !DesignMode)
				handler(this, e);
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlParentChanged"),
#endif
		]
		public virtual event ChangeEventHandler ParentChanged {
			add { Events.AddHandler(ParentChangedEvent, value); }
			remove { Events.RemoveHandler(ParentChangedEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlBeforePrint"),
#endif
		]
		public virtual event PrintEventHandler BeforePrint {
			add { Events.AddHandler(BeforePrintEvent, value); }
			remove { Events.RemoveHandler(BeforePrintEvent, value); }
		}
		internal event PrintEventHandler BeforePrintInternal {
			add { Events.AddHandler(BeforePrintInternalEvent, value); }
			remove { Events.RemoveHandler(BeforePrintInternalEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlAfterPrint"),
#endif
		]
		public virtual event EventHandler AfterPrint {
			add { Events.AddHandler(AfterPrintEvent, value); }
			remove { Events.RemoveHandler(AfterPrintEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlPrintOnPage"),
#endif
		]
		public virtual event PrintOnPageEventHandler PrintOnPage {
			add { Events.AddHandler(PrintOnPageEvent, value); }
			remove { Events.RemoveHandler(PrintOnPageEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlPreviewMouseMove"),
#endif
		]
		public virtual event PreviewMouseEventHandler PreviewMouseMove {
			add { Events.AddHandler(PreviewMouseMoveEvent, value); }
			remove { Events.RemoveHandler(PreviewMouseMoveEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlPreviewMouseDown"),
#endif
		]
		public virtual event PreviewMouseEventHandler PreviewMouseDown {
			add { Events.AddHandler(PreviewMouseDownEvent, value); }
			remove { Events.RemoveHandler(PreviewMouseDownEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlPreviewMouseUp"),
#endif
		]
		public virtual event PreviewMouseEventHandler PreviewMouseUp {
			add { Events.AddHandler(PreviewMouseUpEvent, value); }
			remove { Events.RemoveHandler(PreviewMouseUpEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlPreviewClick"),
#endif
		]
		public virtual event PreviewMouseEventHandler PreviewClick {
			add { Events.AddHandler(PreviewClickEvent, value); }
			remove { Events.RemoveHandler(PreviewClickEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlPreviewDoubleClick"),
#endif
		]
		public virtual event PreviewMouseEventHandler PreviewDoubleClick {
			add { Events.AddHandler(PreviewDoubleClickEvent, value); }
			remove { Events.RemoveHandler(PreviewDoubleClickEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlDraw"),
#endif
		]
		public virtual event DrawEventHandler Draw {
			add { Events.AddHandler(DrawEvent, value); }
			remove { Events.RemoveHandler(DrawEvent, value); }
		}
		internal event DrawEventHandler DrawInternal {
			add { Events.AddHandler(DrawEvent, value); }
			remove { Events.RemoveHandler(DrawEvent, value); }
		}
		internal event BrickEventHandlerBase BrickCreated {
			add { Events.AddHandler(BrickCreatedEvent, value); }
			remove { Events.RemoveHandler(BrickCreatedEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlHtmlItemCreated"),
#endif
		]
		public virtual event HtmlEventHandler HtmlItemCreated {
			add { Events.AddHandler(HtmlItemCreatedEvent, value); }
			remove { Events.RemoveHandler(HtmlItemCreatedEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlLocationChanged"),
#endif
		]
		public virtual event ChangeEventHandler LocationChanged {
			add { Events.AddHandler(LocationChangedEvent, value); }
			remove { Events.RemoveHandler(LocationChangedEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlSizeChanged"),
#endif
		]
		public virtual event ChangeEventHandler SizeChanged {
			add { Events.AddHandler(SizeChangedEvent, value); }
			remove { Events.RemoveHandler(SizeChangedEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlTextChanged"),
#endif
		]
		public virtual event EventHandler TextChanged {
			add { Events.AddHandler(TextChangedEvent, value); }
			remove { Events.RemoveHandler(TextChangedEvent, value); }
		}
		protected internal virtual void OnBeforePrint(PrintEventArgs e) {
			if(DesignMode) return;
			PrintEventHandler handler2 = (PrintEventHandler)Events[BeforePrintInternalEvent];
			if(handler2 != null) handler2(this, e);
			RunEventScript(BeforePrintEvent, EventNames.BeforePrint, e);
			PrintEventHandler handler = (PrintEventHandler)Events[BeforePrintEvent];
			if(handler != null) handler(this, e);
			if(!GetEffectiveVisible() || NeedSuppressValue(ValueSuppressType.Suppress, ProcessDuplicatesMode.Suppress)) e.Cancel = true;
		}
		protected T CreatePresenter<T>(Function<T> runtimeCreator, Function<T> designCreator, Function<T> layoutViewCreator) {
			switch(RootReport.BrickPresentation) {
				case BrickPresentation.Runtime:
					return runtimeCreator();
				case BrickPresentation.Design:
					return designCreator();
				case BrickPresentation.LayoutView:
					return layoutViewCreator();
			}
			throw new NotSupportedException();
		}
		protected internal virtual void OnAfterPrint(EventArgs e) {
			RunEventScript(AfterPrintEvent, EventNames.AfterPrint, e);
			EventHandler handler = (EventHandler)Events[AfterPrintEvent];
			if(handler != null && !DesignMode) handler(this, e);
		}
		protected virtual void OnPrintOnPage(PrintOnPageEventArgs e) {
			RunEventScript(PrintOnPageEvent, EventNames.PrintOnPage, e);
			PrintOnPageEventHandler handler = (PrintOnPageEventHandler)Events[PrintOnPageEvent];
			if(handler != null && !DesignMode) handler(this, e);
			if(!Visible) e.Cancel = true;
		}
		protected virtual void OnParentChanged(ChangeEventArgs e) {
			RunEventScript(ParentChangedEvent, EventNames.ParentChanged, e);
			ChangeEventHandler handler = (ChangeEventHandler)Events[ParentChangedEvent];
			if(handler != null) handler(this, e);
			if(Parent != null) {
				SyncDpi(Parent.Dpi);
				UpdateLayout();
			}
			if(Band != null) Band.UpdateHeightOnChangeContent();
		}
		protected virtual void OnPreviewMouseMove(PreviewMouseEventArgs e) {
			RunEventScript(PreviewMouseMoveEvent, EventNames.PreviewMouseMove, e);
			PreviewMouseEventHandler handler = (PreviewMouseEventHandler)this.Events[PreviewMouseMoveEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnPreviewMouseDown(PreviewMouseEventArgs e) {
			RunEventScript(PreviewMouseDownEvent, EventNames.PreviewMouseDown, e);
			PreviewMouseEventHandler handler = (PreviewMouseEventHandler)this.Events[PreviewMouseDownEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnPreviewMouseUp(PreviewMouseEventArgs e) {
			RunEventScript(PreviewMouseUpEvent, EventNames.PreviewMouseUp, e);
			PreviewMouseEventHandler handler = (PreviewMouseEventHandler)this.Events[PreviewMouseUpEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnPreviewClick(PreviewMouseEventArgs e) {
			RunEventScript(PreviewClickEvent, EventNames.PreviewClick, e);
			PreviewMouseEventHandler handler = (PreviewMouseEventHandler)this.Events[PreviewClickEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnPreviewDoubleClick(PreviewMouseEventArgs e) {
			RunEventScript(PreviewDoubleClickEvent, EventNames.PreviewDoubleClick, e);
			PreviewMouseEventHandler handler = (PreviewMouseEventHandler)this.Events[PreviewDoubleClickEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnLocationChanged(ChangeEventArgs e) {
			RunEventScript(LocationChangedEvent, EventNames.LocationChanged, e);
			ChangeEventHandler handler = (ChangeEventHandler)Events[LocationChangedEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnSizeChanged(ChangeEventArgs e) {
			RunEventScript(SizeChangedEvent, EventNames.SizeChanged, e);
			ChangeEventHandler handler = (ChangeEventHandler)Events[SizeChangedEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnTextChanged(EventArgs e) {
			RunEventScript(TextChangedEvent, EventNames.TextChanged, e);
			EventHandler handler = (EventHandler)Events[TextChangedEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnDraw(DrawEventArgs e) {
			RunEventScript(DrawEvent, EventNames.Draw, e);
			DrawEventHandler handler = (DrawEventHandler)this.Events[DrawEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnHtmlItemCreated(HtmlEventArgs e) {
			RunEventScript(HtmlItemCreatedEvent, EventNames.HtmlItemCreated, e);
			HtmlEventHandler handler = (HtmlEventHandler)this.Events[HtmlItemCreatedEvent];
			if(handler != null) handler(this, e);
		}
		protected void OnBrickCreated(BrickEventArgsBase e) {
			BrickEventHandlerBase handler = (BrickEventHandlerBase)this.Events[BrickCreatedEvent];
			if(handler != null) handler(this, e);
		}
		internal void RaiseAfterPrint(EventArgs e) {
			foreach(XRControl child in Controls)
				child.RaiseAfterPrint(e);
			OnAfterPrint(e);
		}
#if DEBUGTEST
		internal
#endif
		protected bool RunEventScript<E>(object eventId, string eventName, E e) where E : EventArgs {
			return !DesignMode && !ReportIsLoading && RootReport.EventsScriptManager.RunEventScript<E>(eventId, eventName, this, e);
		}
		internal bool ContainsEventScript(string eventName) {
			return RootReport != null && XREventsScriptManager.ContainsEventScript(this, EventNames.HandlerPrefix + eventName);
		}
		internal void RaisePreviewMouseMove(PreviewMouseEventArgs e) {
			OnPreviewMouseMove(e);
		}
		internal void RaisePreviewMouseDown(PreviewMouseEventArgs e) {
			OnPreviewMouseDown(e);
		}
		internal void RaisePreviewMouseUp(PreviewMouseEventArgs e) {
			OnPreviewMouseUp(e);
		}
		internal void RaisePreviewClick(PreviewMouseEventArgs e) {
			OnPreviewClick(e);
		}
		internal void RaisePreviewDoubleClick(PreviewMouseEventArgs e) {
			OnPreviewDoubleClick(e);
		}
		#endregion
		#region Fields & Properties
		BitVector32 flags = new BitVector32();
		FormattingRuleCollection formattingRules;
		bool isDeserializing;
		bool isSerializing;
		protected XRControlCollection fXRControls;
		protected XRControl fParent;
		XRControlParentInfo parentInfo;
		private XRBindingCollection dataBindings;
		protected RectangleF fBounds;
		string text = string.Empty;
		string nullValueText = string.Empty;
		string xlsxFormatString;
		string navigateUrl = string.Empty;
		string target = string.Empty;
		protected string fBookmark = string.Empty;
		XRControl bookmarkParent;
		protected bool fCanGrow = true;
		protected bool fCanShrink;
		int suspendLayoutCount;
		object tag = string.Empty;
		string name = string.Empty;
		protected XRControlStyle fStyle;
		protected XRControlStyles fStyles;
		StyleUsing parentStyleUsing;
		StylePriority stylePriority;
		private float dpi = GraphicsDpi.HundredthsOfAnInch;
		string evenStyleName = String.Empty;
		string oddStyleName = String.Empty;
		string styleName = String.Empty;
		protected XRControlEvents fEventScripts;
		VerticalAnchorStyles fAnchorVertical;
		HorizontalAnchorStyles fAnchorHorizontal;
		ConvertHelper convertHelper;
		ValueSuppressType processNullValues = defaultProcessNullValues;
		ProcessDuplicatesMode processDuplicatesMode = defaultProcessDuplicatesMode;
		ProcessDuplicatesTarget processDuplicatesTarget = defaultProcessDuplicatesTarget;
		StringTrimming textTrimming = defaultTextTrimming;
		protected object previousValue;
		object previousTag;
		int mergeGroupIndex;
		int rowSpan = defaultRowSpan;
		PaddingInfo snapLineMargin = defaultSnapLineMargin;
		PaddingInfo snapLinePadding = defaultSnapLinePadding;
		UrlResolver urlResolver;
		ItemLinkCollection formattingRuleLinks;
		ComponentStateBag stateBag;
		internal ComponentStateBag StateBag {
			get {
				if(stateBag == null)
					stateBag = new ComponentStateBag();
				return stateBag;
			}
		}
		internal XRControlStyle InnerStyle {
			get { return fStyle; }
		}
		[
		XtraSerializableProperty(-1),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public virtual string ControlType {
			get {
				Type type = GetType();
				if(type.Assembly == Assembly.GetAssembly(typeof(XRControl)))
					return type.Name;
				return type.AssemblyQualifiedName;
			}
		}
		[Browsable(false)]
		public bool IsSingleChild {
			get {
				return Parent != null && Parent.Controls.Count == 1 && Parent.Controls.Contains(this);
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlFormattingRules"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.FormattingRules"),
		Editor("DevExpress.XtraReports.Design.FormattingRulesEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		SRCategory(ReportStringId.CatAppearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public virtual FormattingRuleCollection FormattingRules { get { return formattingRules; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public ItemLinkCollection FormattingRuleLinks { get { return formattingRuleLinks; } }
		internal virtual IList VisibleComponents {
			get {
				return new object[] { };
			}
		}
		internal virtual bool BoundsChanging {
			get { return flags[bitBoundsChanging]; }
			set { flags[bitBoundsChanging] = value; }
		}
		protected internal virtual int DefaultWidth {
			get { return DefaultSizes.Control.Width; }
		}
		protected internal virtual int DefaultHeight {
			get { return DefaultSizes.Control.Height; }
		}
		Rectangle DefaultBounds {
			get { return new Rectangle(0, 0, DefaultWidth, DefaultHeight); }
		}
		XRControlStyle EvenStyleCore {
			get { return GetStyle(EvenStyleName); }
			set {
				evenStyleName = RegisterStyle(value);
				ValidateStyleName(value, evenStyleName);
			}
		}
		XRControlStyle OddStyleCore {
			get { return GetStyle(OddStyleName); }
			set {
				oddStyleName = RegisterStyle(value);
				ValidateStyleName(value, oddStyleName);
			}
		}
		XRControlStyle StyleCore {
			get { return GetStyle(StyleName); }
			set {
				styleName = RegisterStyle(value);
				ValidateStyleName(value, styleName);
			}
		}
		static void ValidateStyleName(XRControlStyle style, string styleName) {
			if(style != null && string.IsNullOrEmpty(styleName))
				throw new Exception("Invalid style name.");
		}
		protected bool IsDeserializing { get { return isDeserializing; } }
		internal bool IsSerializing { get { return isSerializing; } }
		[
		SRCategory(ReportStringId.CatLayout),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlSnapLineMargin"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.SnapLineMargin"),
		XtraSerializableProperty,
		]
		public virtual PaddingInfo SnapLineMargin {
			get { return new PaddingInfo(snapLineMargin, dpi); }
			set { snapLineMargin = value; }
		}
		[
		SRCategory(ReportStringId.CatLayout),
		Browsable(false),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlSnapLinePadding"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.SnapLinePadding"),
		XtraSerializableProperty,
		]
		public virtual PaddingInfo SnapLinePadding {
			get { return new PaddingInfo(snapLinePadding, dpi); }
			set { snapLinePadding = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlAnchorVertical"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.AnchorVertical"),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public virtual VerticalAnchorStyles AnchorVertical { get { return fAnchorVertical; } set { fAnchorVertical = value; } }
		bool ShouldSerializeAnchorVertical() {
			return fAnchorVertical != DefaultAnchorVertical;
		}
		void ResetAnchorVertical() {
			fAnchorVertical = DefaultAnchorVertical;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlAnchorHorizontal"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.AnchorHorizontal"),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public virtual HorizontalAnchorStyles AnchorHorizontal { get { return fAnchorHorizontal; } set { fAnchorHorizontal = value; } }
		bool ShouldSerializeAnchorHorizontal() {
			return fAnchorHorizontal != DefaultAnchorHorizontal;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlLockedInUserDesigner"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.LockedInUserDesigner"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public virtual bool LockedInUserDesigner {
			get { return flags[bitLockedInUserDesigner]; }
			set { flags[bitLockedInUserDesigner] = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlKeepTogether"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.KeepTogether"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public virtual bool KeepTogether { get { return flags[bitKeepTogether]; } set { flags[bitKeepTogether] = value; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool HasChildren { get { return (fXRControls != null && fXRControls.Count > 0); } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual XRControl Parent { get { return fParent; } set { SetParent(value); } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public int Index {
			get { return ControlContainer != null ? ((IList)ControlContainer).IndexOf(this) : -1; }
			set { ControlContainer.SetChildIndexInternal(this, value); }
		}
		[Browsable(false)]
		public virtual bool CanHaveChildren { get { return false; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public XRControlCollection Controls { get { return fXRControls; } }
		public IEnumerable<T> AllControls<T>() where T : XRControl {
			foreach(XRControl item in AllControls(_ => true)) {
				if(item is T)
					yield return (T)item;
			}
		}
		internal virtual IEnumerable<XRControl> AllControls(Predicate<XRControl> enumChildren) {
			foreach(XRControl item in Controls) {
				if(enumChildren(item)) {
					foreach(XRControl item2 in item.AllControls(enumChildren))
						yield return item2;
				}
				yield return item;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlText"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Text"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(""),
		Bindable(true),
		Localizable(true),
		XtraSerializableProperty,
		]
		public virtual string Text {
			get { return text; }
			set {
				if(text != value) {
					text = value;
					if(text == null)
						text = String.Empty;
					OnTextChanged(EventArgs.Empty);
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.NullValueText"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public virtual string NullValueText {
			get { return nullValueText; }
			set { nullValueText = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlXlsxFormatString"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.XlsxFormatString"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(null),
		XtraSerializableProperty,
		]
		public virtual string XlsxFormatString {
			get { return xlsxFormatString; }
			set { xlsxFormatString = value; }
		}
		[
		Obsolete("The Dock property is now obsolete and isn't used at all. Note that you can achieve a similar functionality using the AnchorVertical property."),
		DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public XRDockStyle Dock { get { return XRDockStyle.None; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlTextAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.TextAlignment"),
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.XtraReports.Design.TextAlignmentEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		Localizable(true),
		XtraSerializableProperty,
		]
		public virtual TextAlignment TextAlignment {
			get { return fStyle.TextAlignment; }
			set { fStyle.TextAlignment = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlTextTrimming"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.TextTrimming"),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(defaultTextTrimming),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual StringTrimming TextTrimming {
			get { return textTrimming; }
			set { textTrimming = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlWordWrap"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.WordWrap"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public virtual bool WordWrap { get { return flags[bitWordWrap]; } set { flags[bitWordWrap] = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlNavigateUrl"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.NavigateUrl"),
		Bindable(true),
		SRCategory(ReportStringId.CatNavigation),
		DefaultValue(""),
		TypeConverter(typeof(DevExpress.XtraReports.Design.NavigateUrlConverter)),
		XtraSerializableProperty,
		]
		public virtual string NavigateUrl { get { return navigateUrl; } set { navigateUrl = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlTarget"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Target"),
		SRCategory(ReportStringId.CatNavigation),
		DefaultValue(""),
		TypeConverter(typeof(DevExpress.XtraReports.Design.TargetConverter)),
		XtraSerializableProperty,
		]
		public virtual string Target { get { return target; } set { target = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlBookmark"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Bookmark"),
		Bindable(true),
		SRCategory(ReportStringId.CatNavigation),
		XtraSerializableProperty,
		]
		public virtual string Bookmark { get { return fBookmark; } set { fBookmark = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlBookmarkParent"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.BookmarkParent"),
		SRCategory(ReportStringId.CatNavigation),
		DefaultValue(null),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlReferenceConverter)),
		Editor("DevExpress.XtraReports.Design.ParentBookmarkEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public virtual XRControl BookmarkParent {
			get {
				return bookmarkParent != null && !bookmarkParent.IsDisposed ? bookmarkParent : null;
			}
			set {
				for(XRControl control = value; control != null; control = control.BookmarkParent)
					if(control == this)
						throw new Exception(Localization.ReportLocalizer.GetString(Localization.ReportStringId.Msg_CyclicBookmarks));
				bookmarkParent = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlDataBindings"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.DataBindings"),
		SRCategory(ReportStringId.CatData),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		ParenthesizePropertyName(true),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public virtual XRBindingCollection DataBindings { get { return dataBindings; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual RectangleF BoundsF {
			get { return fBounds; }
			set { SetBounds(value.X, value.Y, value.Width, value.Height, BoundsSpecified.All); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Rectangle Bounds {
			get { return Rectangle.Round(BoundsF); }
			set { BoundsF = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Width")
		]
		public virtual float WidthF {
			get { return BoundsF.Width; }
			set { SetBounds(LeftF, TopF, value, HeightF, BoundsSpecified.Width); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int Width {
			get { return (int)Math.Round(WidthF); }
			set { WidthF = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Height"),
		]
		public virtual float HeightF {
			get { return BoundsF.Height; }
			set { SetBounds(LeftF, TopF, WidthF, value, BoundsSpecified.Height); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int Height {
			get { return (int)Math.Round(HeightF); }
			set { HeightF = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlSizeF"),
#endif
		SRCategory(ReportStringId.CatLayout),
		Localizable(true),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Size"),
		TypeConverter(typeof(DevExpress.Utils.Design.SizeFTypeConverter)),
		XtraSerializableProperty,
		]
		public virtual SizeF SizeF {
			get { return BoundsF.Size; }
			set { SetBounds(LeftF, TopF, value.Width, value.Height, BoundsSpecified.Size); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Size Size {
			get { return Size.Round(SizeF); }
			set { SizeF = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual PointF LocationF {
			get { return BoundsF.Location; }
			set { SetBounds(value.X, value.Y, WidthF, HeightF, BoundsSpecified.Location); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlLocationF"),
#endif
		Localizable(true),
		SRCategory(ReportStringId.CatLayout),
		EditorBrowsable(EditorBrowsableState.Never),
		TypeConverter(typeof(PointFloatConverterForDisplay)),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Location"),
		XtraSerializableProperty,
		]
		public virtual PointFloat LocationFloat { get { return new PointFloat(LocationF); } set { LocationF = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Point Location {
			get { return Point.Round(LocationF); }
			set { LocationF = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual float LeftF {
			get { return BoundsF.Left; }
			set { SetBounds(value, TopF, WidthF, HeightF, BoundsSpecified.X); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int Left {
			get { return (int)Math.Round(LeftF); }
			set { LeftF = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual float TopF {
			get { return BoundsF.Top; }
			set { SetBounds(LeftF, value, WidthF, HeightF, BoundsSpecified.Y); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int Top {
			get { return (int)Math.Round(TopF); }
			set { TopF = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual float RightF { get { return BoundsF.Right; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int Right {
			get { return (int)Math.Round(RightF); }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual float BottomF { get { return BoundsF.Bottom; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int Bottom {
			get { return (int)Math.Round(BottomF); }
		}
#if DEBUGTEST
		[Browsable(false)]
		public
#else
		internal
#endif
 virtual PointF RightBottomF {
			get { return new PointF(RightF, BottomF); }
		}
		protected virtual bool CanChangeZOrder { get { return true; } }
		protected virtual bool CanHaveExportWarning { get { return true; } }
		protected internal virtual RectangleF ClientRectangleF { get { return new RectangleF(0, 0, this.BoundsF.Width, this.BoundsF.Height); } }
		protected internal virtual bool CanDrawBackground { get { return true; } }
		protected internal virtual bool HasUndefinedBounds { get { return false; } }
		protected virtual bool NeedCalcContainerHeight { get { return false; } }
		protected internal virtual bool IsNavigateTarget { get { return true; } }
		protected internal float ActualBorderWidth { get { return GraphicsUnitConverter.Convert(GetEffectiveBorderWidth(), GraphicsDpi.Pixel, Dpi); } }
		protected internal virtual XRControl OverlappingParent { get { return Parent; } }
		internal protected virtual VerticalAnchorStyles DefaultAnchorVertical { get { return VerticalAnchorStyles.None; } }
		internal protected virtual HorizontalAnchorStyles DefaultAnchorHorizontal { get { return HorizontalAnchorStyles.None; } }
		bool IsDisposing {
			get { return flags[bitIsDisposing]; }
			set { flags[bitIsDisposing] = value; }
		}
		[Browsable(false)]
		public bool IsDisposed {
			get { return flags[bitIsDisposed]; }
			private set { flags[bitIsDisposed] = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.CanGrow"),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.CanGrowCanShrinkConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		DefaultValue(true),
		]
		public virtual bool CanGrow {
			get {
				return fCanGrow && AnchorAllowsShrinkGrow;
			}
			set { fCanGrow = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.CanShrink"),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.CanGrowCanShrinkConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		DefaultValue(false),
		]
		public virtual bool CanShrink {
			get {
				return fCanShrink && AnchorAllowsShrinkGrow;
			}
			set { fCanShrink = value; }
		}
		internal bool AnchorAllowsShrinkGrow {
			get {
				return this.AnchorVertical != VerticalAnchorStyles.Bottom && this.AnchorVertical != VerticalAnchorStyles.Both;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(GraphicsDpi.HundredthsOfAnInch),
		Localizable(true),
		XtraSerializableProperty,
		]
		public virtual float Dpi { get { return dpi; } set { dpi = value; } }
		[
		DefaultValue(""),
		Browsable(false),
		XtraSerializableProperty(-1),
		]
		public string Name {
			get { return Site != null ? Site.Name : name; }
			set {
				if(Site == null || string.Equals(Site.Name, value))
					name = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyles"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Styles"),
		]
		public virtual XRControlStyles Styles { get { return fStyles; } }
		[
		Browsable(false),
		XtraSerializableProperty,
		]
		public virtual string EvenStyleName { get { return evenStyleName; } set { evenStyleName = value != null ? value : ""; } }
		[
		Browsable(false),
		XtraSerializableProperty,
		]
		public virtual string OddStyleName { get { return oddStyleName; } set { oddStyleName = value != null ? value : ""; } }
		[
		Browsable(false),
		XtraSerializableProperty,
		]
		public virtual string StyleName { get { return styleName; } set { styleName = value != null ? value : ""; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStylePriority"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.StylePriority"),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public virtual StylePriority StylePriority {
			get { return stylePriority; }
		}
		#region style properties
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlParentStyleUsing"),
#endif
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.ParentStyleUsing"),
		]
		public virtual StyleUsing ParentStyleUsing { get { return parentStyleUsing; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlFont"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.XtraReports.Design.XRFontEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		Localizable(true),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Font"),
		TypeConverter(typeof(FontTypeConverter)),
		XtraSerializableProperty,
		]
		public virtual Font Font {
			get { return fStyle.Font.Validate(Site); }
			set {
				Font oldValue = fStyle.Font;
				fStyle.Font = value.Validate(Site);
				if(fStyle.Font != oldValue)
					OnFontChanged();
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlForeColor"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		Localizable(true),
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.ForeColor"),
	   XtraSerializableProperty,
		]
		public virtual Color ForeColor {
			get { return fStyle.ForeColor; }
			set {
				Color oldForeColor = fStyle.ForeColor;
				fStyle.ForeColor = value;
				if(fStyle.ForeColor != oldForeColor)
					OnForeColorChanged();
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlBackColor"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		Localizable(true),
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.BackColor"),
		XtraSerializableProperty,
		]
		public virtual Color BackColor {
			get { return fStyle.BackColor; }
			set { fStyle.BackColor = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlPadding"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Padding"),
		XtraSerializableProperty,
		]
		public virtual PaddingInfo Padding {
			get { return fStyle.Padding; }
			set {
				fStyle.Padding = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlBorderColor"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		Localizable(true),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.BorderColor"),
		XtraSerializableProperty,
		]
		public virtual Color BorderColor {
			get { return fStyle.BorderColor; }
			set { fStyle.BorderColor = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlBorders"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
	   Editor("DevExpress.XtraReports.Design.BordersEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Borders"),
		XtraSerializableProperty,
		]
		public virtual BorderSide Borders {
			get { return fStyle.Borders; }
			set {
				BorderSide oldValue = fStyle.Borders;
				fStyle.Borders = value;
				if(fStyle.Borders != oldValue)
					OnBordersChanged();
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlBorderWidth"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.BorderWidth"),
	   XtraSerializableProperty,
		]
		public virtual float BorderWidth {
			get { return fStyle.BorderWidth; }
			set {
				if(value < 0)
					throw (new ArgumentOutOfRangeException("BorderWidth"));
				float oldValue = fStyle.BorderWidth;
				fStyle.BorderWidth = value;
				if(BorderWidth != oldValue)
					OnBorderWidthChanged();
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlBorderDashStyle"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.BorderDashStyle"),
	   XtraSerializableProperty,
		]
		public virtual BorderDashStyle BorderDashStyle {
			get { return fStyle.BorderDashStyle; }
			set { fStyle.BorderDashStyle = value; }
		}
		internal bool IgnoreEvenOddStyles {
			get { return flags[bitIgnoreEvenOddStyles]; }
			set { flags[bitIgnoreEvenOddStyles] = value; }
		}
		#endregion style properties
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlTag"),
#endif
		SRCategory(ReportStringId.CatData),
		DefaultValue(""),
		TypeConverter(typeof(System.ComponentModel.StringConverter)),
		Bindable(true),
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Tag"),
	   XtraSerializableProperty,
		]
		public object Tag { get { return tag; } set { tag = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlVisible"),
#endif
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Localizable(true),
		XtraSerializableProperty,
		]
		public virtual bool Visible { get { return flags[bitVisible]; } set { flags[bitVisible] = value; } }
		internal bool GetEffectiveVisible() {
			DefaultBoolean formattingVisible = formattingRules.GetEffectiveVisible();
			return formattingVisible == DefaultBoolean.Default ? Visible : formattingVisible == DefaultBoolean.True;
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.CanPublish"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public virtual bool CanPublish { get { return flags[bitCanPublish]; } set { flags[bitCanPublish] = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlScripts"),
#endif
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControl.Scripts"),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public XRControlScripts Scripts { get { return (XRControlScripts)fEventScripts; } }
		internal bool Suspended { get { return suspendLayoutCount > 0; } }
		protected internal bool CanAutoHeight { get { return fCanGrow || fCanShrink; } }
		protected internal virtual bool ReportIsLoading { get { return RootReport == null || RootReport.Loading; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual XtraReport RootReport { get { return GetRootReport(); } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual XtraReportBase Report { get { return GetReport(); } }
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(defaultProcessNullValues),		
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		TypeConverter(typeof(DevExpress.XtraReports.Design.ProcessNullValuesTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual ValueSuppressType ProcessNullValues {
			get { return processNullValues; }
			set { processNullValues = value; }
		}
		[
		Obsolete("The DevExpress.XtraReports.UI.ProcessDuplicates property is now obsolete. Use the DevExpress.XtraReports.UI.ProcessDuplicatesTarget and DevExpress.XtraReports.UI.ProcessDuplicatesMode properties instead."),
		SRCategory(ReportStringId.CatBehavior),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual ValueSuppressType ProcessDuplicates {
			get {
				if(processDuplicatesMode == ProcessDuplicatesMode.Leave) return ValueSuppressType.Leave;
				else if(processDuplicatesMode == ProcessDuplicatesMode.Suppress) return ValueSuppressType.Suppress;
				else if(processDuplicatesMode == ProcessDuplicatesMode.SuppressAndShrink) return ValueSuppressType.SuppressAndShrink;
#pragma warning disable 0618
				else if(processDuplicatesTarget == ProcessDuplicatesTarget.Tag) return ValueSuppressType.MergeByTag;
				else return ValueSuppressType.MergeByValue;
#pragma warning restore 0618
			}
			set {
				if(value == ValueSuppressType.Leave) processDuplicatesMode = ProcessDuplicatesMode.Leave;
				else if(value == ValueSuppressType.Suppress) processDuplicatesMode = ProcessDuplicatesMode.Suppress;
				else if(value == ValueSuppressType.SuppressAndShrink) processDuplicatesMode = ProcessDuplicatesMode.SuppressAndShrink;
				else {
					processDuplicatesMode = ProcessDuplicatesMode.Merge;
#pragma warning disable 0618
					if(value == ValueSuppressType.MergeByTag) processDuplicatesTarget = ProcessDuplicatesTarget.Tag;
#pragma warning restore 0618
				}
			}
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(defaultProcessDuplicatesMode),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(DevExpress.XtraReports.Design.ProcessDuplicatesModeTypeConverter)),
		]
		public virtual ProcessDuplicatesMode ProcessDuplicatesMode {
			get { return processDuplicatesMode; }
			set { processDuplicatesMode = value; }
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(defaultProcessDuplicatesTarget),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual ProcessDuplicatesTarget ProcessDuplicatesTarget {
			get { return processDuplicatesTarget; }
			set { processDuplicatesTarget = value; }
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]		
		public virtual int RowSpan {
			get { return rowSpan; }
			set { rowSpan = value; }
		}
		[Browsable(false)]
		public virtual Band Band { get { return GetBand(); } }
		internal int NestedLevel { get { return parentInfo.GetNestedLevel(); } }
		protected virtual bool HasPageSummary { get { return false; } }
		internal virtual XRControlCollectionBase ControlContainer { get { return fParent != null ? fParent.Controls : null; } }
		internal virtual XRControl PrintableParent { get { return fParent; } }
		internal virtual BorderSide VisibleContourBorders { get { return BorderSide.All; } }
		internal CalculatedFieldCollection ReportCalculatedFields {
			get { return RootReport != null ? RootReport.CalculatedFields : null; }
		}
		internal FormattingRuleSheet ReportFormattingRuleSheet {
			get { return RootReport.FormattingRuleSheet; }
		}
		internal virtual XRControl LocationParent {
			get { return Parent; }
		}
		internal virtual XRControl RightBottomParent {
			get { return Parent; }
		}
		internal bool ActualLockedInUserDesigner {
			get {
				bool parentActualLocked = Parent == null ? false : Parent.ActualLockedInUserDesigner;
				return LockedInUserDesigner || parentActualLocked;
			}
		}
		internal RectangleF BoundsRelativeToBand {
			get { return GetBounds(Band); }
		}
		internal UrlResolver UrlResolver {
			get {
				if(urlResolver == null) {
					if(RootReport == null)
						return UrlResolver.Instance;
					urlResolver = ((IServiceProvider)RootReport).GetService(typeof(UrlResolver)) as UrlResolver;
				}
				return urlResolver;
			}
		}
		readonly BookmarkHelper bookmarkHelper = new BookmarkHelper();
		internal BookmarkHelper BookmarkHelper { get { return bookmarkHelper; } }
		#endregion
		public XRControl() {
			Initialize(DefaultBounds);
		}
		internal XRControl(RectangleF bounds) {
			Initialize(bounds);
		}
		void Initialize(RectangleF bounds) {
			CanPublish = true;
			Visible = true;
			WordWrap = true;
			KeepTogether = true;
			fBookmark = String.Empty;
			this.fBounds = bounds;
			this.fAnchorVertical = DefaultAnchorVertical;
			fEventScripts = CreateScripts();
			fXRControls = CreateChildControls();
			fXRControls.CollectionChanged += OnControlCollectionChanged;
			dataBindings = new XRBindingCollection(this);
			parentStyleUsing = new StyleUsing();
			fStyles = CreateStyles();
			ResetStylePriority();
			fStyle = new XRControlStyle();
			parentInfo = new XRControlParentInfo(this);
			formattingRules = new FormattingRuleCollection(this);
			formattingRuleLinks = new ItemLinkCollection();
		}
		protected virtual void OnControlCollectionChanged(object sender, CollectionChangeEventArgs e) {
		}
		protected virtual XRControlStyles CreateStyles() {
			return new XRControlStyles(this);
		}
		protected virtual XRControlScripts CreateScripts() {
			return new XRControlScripts(this);
		}
		internal virtual ArrayList GetPrintableControls() {
			return new ArrayList(Controls);
		}
		internal virtual void SetDataBindingWhenDroppedFromTheFieldList(XRBinding binding) {
			DataBindings.Add(binding);
		}
		internal RectangleF GetClipppedBandBounds(GraphicsUnit unit) {
			return XRConvert.Convert(GetClippedBandBounds(), this.Dpi, GraphicsDpi.UnitToDpi(unit));
		}
		RectangleF GetClippedBandBounds() {
			XRControl ctl = this;
			RectangleF rect = this.BoundsF;
			while(!(ctl.PrintableParent is Band) && ctl.PrintableParent != null) {
				ctl = ctl.PrintableParent;
				rect.Offset(ctl.BoundsF.Location);
				rect = RectangleF.Intersect(rect, ctl.BoundsF);
			}
			return rect;
		}
		internal virtual RectangleF GetBounds(Band band) {
			return PrintableParent.RectangleFToBand(BoundsF);
		}
		internal RectangleF GetBounds(Band band, GraphicsUnit unit) {
			return XRConvert.Convert(GetBounds(band), Dpi, GraphicsDpi.UnitToDpi(unit));
		}
		public virtual void RemoveInvalidBindings(Predicate<XRBinding> predicate) {
			using(XRDataContext dataContext = new XRDataContext(ReportCalculatedFields, true)) {
				RemoveInvalidBindingsCore(dataContext, predicate);
			}
		}
		protected virtual void AdjustDataSource() {
			foreach(XRBinding item in DataBindings)
				item.AdjustDataSourse();
			foreach(XRControl item in Controls)
				item.AdjustDataSource();
		}
		protected void RemoveInvalidBindingsCore(XRDataContext dataContext, Predicate<XRBinding> predicate) {
			for(int i = DataBindings.Count - 1; i >= 0; i--) {
				XRBinding binding = DataBindings[i];
				if(!binding.IsValidDataSource(dataContext) && (predicate == null || predicate(binding)))
					DataBindings.RemoveAt(i);
			}
			foreach(XRControl item in Controls)
				item.RemoveInvalidBindingsCore(dataContext, predicate);
		}
		protected internal virtual void ValidateDataSource(object newSource, object oldSource, string dataMember) {
		}
		protected internal virtual bool HasBindings() {
			return DataBindings.Count > 0;
		}
		protected virtual void OnDisposing() {
			if(IsDisposing)
				return;
			IsDisposing = true;
			RaiseDisposing(EventArgs.Empty);
			if(fXRControls != null) {
				foreach(XRControl control in fXRControls)
					control.OnDisposing();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(IsDisposed)
					return;
				OnDisposing();
				IsDisposing = false;
				IsDisposed = true;
				base.Dispose(disposing);
				if(ControlContainer != null && ((IList)ControlContainer).Contains(this))
					((IList)ControlContainer).Remove(this);
				if(fStyle != null) {
					fStyle.Dispose();
				}
				if(fXRControls != null) {
					fXRControls.Dispose();
					fXRControls = null;
				}
				if(fEventScripts != null) {
					fEventScripts.Dispose();
					fEventScripts = null;
				}
				if(parentInfo != null) {
					parentInfo.Dispose();
					parentInfo = null;
				}
				if(dataBindings != null) {
					dataBindings.Dispose();
					dataBindings = null;
				}
				if(fStyles != null) {
					fStyles.Dispose();
					fStyles = null;
				}
				if(formattingRules != null) {
					formattingRules.Clear();
					formattingRules = null;
				}
				if(stateBag != null) {
					stateBag.Clear();
					stateBag = null;
				}
			} else
				base.Dispose(disposing);
		}
		void SetStyle(XRControlStyle style) {
			if(fStyle != null && !Object.ReferenceEquals(style, fStyle))
				fStyle.Dispose();
			fStyle = style;
		}
		internal void AssignStyle(XRControlStyle newStyle) {
			newStyle.ChangeStyle(AssignStyle);
		}
		void AssignStyle(StyleProperty property, XRControlStyle style) {
			if(!style.IsSet(property))
				fStyle.Reset(property);
			else {
				fStyle.SetValue(property, style.GetValue(property));
				StylePriority.Set(property, false);
			}
		}
		protected virtual void SetParent(XRControl value) {
			if(fParent != value && value != null)
				value.Controls.Add(this);
		}
		protected virtual XRControlCollection CreateChildControls() {
			return new XRControlCollection(this);
		}
		public void SuspendLayout() {
			suspendLayoutCount += 1;
		}
		public void ResumeLayout() {
			if(suspendLayoutCount > 0) suspendLayoutCount -= 1;
		}
		public virtual void PerformLayout() {
			suspendLayoutCount = 0;
			UpdateLayout();
		}
		public XRControl FindControl(string name, bool ignoreCase) {
			foreach(XRControl item in AllControls<XRControl>()) {
				if(String.Compare(item.Name, name, ignoreCase) == 0)
					return item;
			}
			return null;
		}
		protected internal virtual void OnReportInitialize() {
			ClearParentInfo();
			if(RootReport.VersionLessThen(v8_1))
				fStyle.ChangeStyle(ConvertStyle);
			if(RootReport.VersionLessThen(v9_3))
				Scripts.ConvertScripts(RootReport);
			for(int i = 0; i < Controls.Count; i++)
				Controls[i].OnReportInitialize();
		}
		internal virtual IComponent[] GetNonSerializableComponents() {
			return new IComponent[] { };
		}
		protected virtual void CollectAssociatedComponents(DesignItemList components) {
			if(Controls.Count > 0) {
				components.AddRange(Controls);
				for(int i = 0; i < Controls.Count; i++)
					Controls[i].CollectAssociatedComponents(components);
			}
		}
		internal DesignItemList GetAssociatedComponents() {
			DesignItemList components = new DesignItemList(RootReport);
			CollectAssociatedComponents(components);
			return components;
		}
		#region Serialization
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
			SerializeProperties(serializer);
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			DeserializeProperties(serializer);
		}
		IList IXRSerializable.SerializableObjects {
			get { return SerializableObjects; }
		}
		bool GetKeepTogetherDefault() {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
			foreach(PropertyDescriptor property in properties)
				if(property != null && property.DisplayName == "KeepTogether")
					return (bool)((DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)]).Value;
			return false;
		}
		protected virtual void SerializeProperties(XRSerializer serializer) {
			serializer.SerializeBoolean("Visible", Visible);
			if(tag is string) serializer.SerializeString("Tag", (string)tag);
			serializer.SerializeString("Name", Name);
			serializer.Serialize("Style", fStyle);
			serializer.Serialize("ParentStyleUsing", ParentStyleUsing);
			serializer.SerializeSingle("Dpi", dpi);
			serializer.SerializeString("EvenStyleName", EvenStyleName, String.Empty);
			serializer.SerializeString("OddStyleName", OddStyleName, String.Empty);
			serializer.SerializeString("StyleName", StyleName, String.Empty);
			serializer.SerializeInteger("BindingItemCount", dataBindings.Count);
			for(int i = 0; i < dataBindings.Count; i++) {
				serializer.Serialize("BindingItem" + i, dataBindings[i]);
			}
			serializer.SerializeRectangle("Bounds", Bounds);
			serializer.SerializeString("Text", text);
			serializer.SerializeString("NavigateUrl", navigateUrl);
			serializer.SerializeString("Target", target);
			serializer.SerializeString("Bookmark", fBookmark, String.Empty);
			serializer.SerializeXRControlReference("BookmarkParent", BookmarkParent);
			serializer.SerializeBoolean("CanGrow", fCanGrow);
			serializer.SerializeBoolean("CanShrink", fCanShrink);
			serializer.SerializeBoolean("WordWrap", WordWrap);
			serializer.SerializeBoolean("KeepTogether", KeepTogether);
			serializer.Serialize("EventsScript", fEventScripts);
		}
		protected virtual void DeserializeProperties(XRSerializer serializer) {
			Visible = serializer.DeserializeBoolean("Visible", true);
			tag = serializer.DeserializeString("Tag", "");
			name = serializer.DeserializeString("Name", "");
			serializer.Deserialize("Style", fStyle);
			serializer.Deserialize("ParentStyleUsing", ParentStyleUsing);
			dpi = serializer.DeserializeSingle("Dpi", GraphicsDpi.HundredthsOfAnInch);
			EvenStyleName = serializer.DeserializeString("EvenStyleName", null);
			OddStyleName = serializer.DeserializeString("OddStyleName", null);
			StyleName = serializer.DeserializeString("StyleName", null);
			dataBindings.Clear();
			int count = serializer.DeserializeInteger("BindingItemCount", 0);
			for(int i = 0; i < count; i++) {
				XRBinding binding = new XRBinding();
				serializer.Deserialize("BindingItem" + i, binding);
				dataBindings.Add(binding);
			}
			fBounds = serializer.DeserializeRectangle("Bounds", DefaultBounds);
			text = serializer.DeserializeString("Text", "");
			fBookmark = serializer.DeserializeString("Bookmark", "");
			serializer.DeserializeXRControlReference(this, "BookmarkParent");
			navigateUrl = serializer.DeserializeString("NavigateUrl", "");
			target = serializer.DeserializeString("Target", "");
			bool autoHeight = serializer.DeserializeBoolean("AutoHeight", false);
			fCanGrow = serializer.DeserializeBoolean("CanGrow", autoHeight);
			fCanShrink = serializer.DeserializeBoolean("CanShrink", autoHeight);
			WordWrap = serializer.DeserializeBoolean("WordWrap", true);
			KeepTogether = serializer.DeserializeBoolean("KeepTogether", GetKeepTogetherDefault());
			serializer.DeserializeEventHandlers(new string[] {EventNames.BeforePrint, 
																		 EventNames.AfterPrint, 
																		 EventNames.PrintOnPage,
																		 EventNames.LocationChanged, 
																		 EventNames.SizeChanged, 
																		 EventNames.TextChanged, 
																		 EventNames.Draw, 
																		 EventNames.HtmlItemCreated, 
																		 EventNames.ParentChanged,
																		 EventNames.PreviewClick,
																		 EventNames.PreviewDoubleClick,
																		 EventNames.PreviewMouseMove,
																		 EventNames.PreviewMouseDown,
																		 EventNames.PreviewMouseUp
																	 }, this);
			serializer.Deserialize("EventsScript", fEventScripts);
		}
		protected virtual IList SerializableObjects {
			get { return Controls; }
		}
		internal Hashtable GetEventHandlers() {
			Hashtable handlers = new Hashtable();
			Type type = GetType();
			System.Reflection.EventInfo[] events = type.GetEvents();
			foreach(System.Reflection.EventInfo eventInfo in events) {
				FieldInfo fieldInfo = eventInfo.DeclaringType.GetField(eventInfo.Name + "Event", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic);
				if(fieldInfo != null) {
					Delegate handler = Events[fieldInfo.GetValue(this)];
					if(handler != null)
						handlers[eventInfo] = handler;
				}
			}
			return handlers;
		}
		internal void LoadLayoutInternal(Stream stream) {
			CopyProperties(FromStreamInternal(stream, Name));
		}
		protected internal virtual void CopyProperties(XRControl control) {
			Controls.Clear();
			Controls.AddRange((XRControl[])new ArrayList(control.Controls).ToArray(typeof(XRControl)));
			XtraReport.CopyProperties(control, this);
		}
		protected internal virtual void CopyDataProperties(XRControl control) {
			if(Controls.Count == control.Controls.Count) {
				for(int i = 0; i < Controls.Count; i++)
					Controls[i].CopyDataProperties(control.Controls[i]);
			}
			for(int i = 0; i < DataBindings.Count; i++)
				DataBindings[i].CopyDataProperties(control.DataBindings[i]);
		}
		protected internal virtual void SaveLayoutInternal(Stream stream) {
			DesignItemList components = GetAssociatedComponents();
			components.Add(this);
			CodeGeneratorAccessorBase.Instance.GenerateCSharpCode(components, stream);
		}
		[System.Security.SecuritySafeCritical]
		protected IFormatter CreateLoadFormatter(ISerializationSurrogate surrogate) {
			SoapFormatter formatter = CreateSoapFormatter();
			SurrogateSelector ss = new SurrogateSelector();
			ss.AddSurrogate(GetType(), new StreamingContext(StreamingContextStates.All), surrogate);
			formatter.SurrogateSelector = ss;
			return formatter;
		}
		#endregion
		#region ShouldSerialize & Reset
		protected virtual bool ShouldSerializeLocation() {
			return true;
		}
		protected virtual bool ShouldSerializeBookmark() {
			return !String.IsNullOrEmpty(fBookmark);
		}
		bool ShouldSerializeStyleName() {
			return this.Styles.Style != null;
		}
		bool ShouldSerializeEvenStyleName() {
			return this.Styles.EvenStyle != null;
		}
		bool ShouldSerializeOddStyleName() {
			return this.Styles.OddStyle != null;
		}
		bool ShouldSerializeFormattingRules() {
			return formattingRules.Count != 0;
		}
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
		bool ShouldSerializeStylePriority() {
			return stylePriority.ShouldSerialize();
		}
		void ResetStylePriority() {
			stylePriority = new StylePriority();
		}
		void ResetStyleName() {
			this.Styles.Style = null;
		}
		void ResetEvenStyleName() {
			this.Styles.EvenStyle = null;
		}
		void ResetOddStyleName() {
			this.Styles.OddStyle = null;
		}
		#region ShouldSerialize style properties
		internal protected virtual bool ShouldSerializeBackColor() {
			return fStyle.IsSetBackColor;
		}
		internal protected virtual bool ShouldSerializeBorderColor() {
			return fStyle.IsSetBorderColor;
		}
		internal protected virtual bool ShouldSerializeBorderDashStyle() {
			return fStyle.IsSetBorderDashStyle;
		}
		internal protected virtual bool ShouldSerializeBorders() {
			return fStyle.IsSetBorders;
		}
		internal protected virtual bool ShouldSerializeBorderWidth() {
			return fStyle.IsSetBorderWidth;
		}
		internal protected virtual bool ShouldSerializeFont() {
			return fStyle.IsSetFont;
		}
		internal protected virtual bool ShouldSerializeForeColor() {
			return fStyle.IsSetForeColor;
		}
		internal protected virtual bool ShouldSerializePadding() {
			return fStyle.IsSetPadding;
		}
		internal protected virtual bool ShouldSerializeTextAlignment() {
			return fStyle.IsSetTextAlignment;
		}
		protected bool ShouldSerializeSnapLineMargin() {
			return snapLineMargin != defaultSnapLineMargin;
		}
		protected void ResetSnapLineMargin() {
			snapLineMargin = defaultSnapLineMargin;
		}
		protected bool ShouldSerializeSnapLinePadding() {
			return snapLinePadding != defaultSnapLinePadding;
		}
		protected void ResetSnapLinePadding() {
			snapLinePadding = defaultSnapLinePadding;
		}
		#endregion
		#region Reset style properties
		public virtual void ResetBackColor() {
			fStyle.ResetBackColor();
		}
		public void ResetBorderColor() {
			fStyle.ResetBorderColor();
		}
		public virtual void ResetBorders() {
			fStyle.ResetBorders();
		}
		public virtual void ResetBorderDashStyle() {
			fStyle.ResetBorderDashStyle();
		}
		public void ResetBorderWidth() {
			fStyle.ResetBorderWidth();
		}
		public void ResetFont() {
			fStyle.ResetFont();
		}
		public void ResetForeColor() {
			fStyle.ResetForeColor();
		}
		public virtual void ResetPadding() {
			fStyle.ResetPadding();
		}
		public void ResetTextAlignment() {
			fStyle.ResetTextAlignment();
		}
		#endregion
		protected virtual void ResetBookmark() {
			fBookmark = String.Empty;
		}
		protected virtual void ResetDpi() {
			dpi = GraphicsDpi.HundredthsOfAnInch;
		}
		#endregion
		bool ShouldSerializeXlsxFormatString() {
			return !string.IsNullOrEmpty(XlsxFormatString);
		}
		internal virtual int CompareTabOrder(XRControl xrControl) {
			return this.TopF != xrControl.TopF ? Comparer.Default.Compare(this.TopF, xrControl.TopF) :
				Comparer.Default.Compare(this.LeftF, xrControl.LeftF);
		}
		internal virtual bool IsInsideBand(Band band) {
			return Comparer.Equals(this.Band, band);
		}
		internal ScriptLanguage GetScriptLanguage() {
			XtraReport report = RootReport;
			return (report == null) ? ScriptLanguage.CSharp : report.ScriptLanguage;
		}
		protected virtual void OnBorderWidthChanged() {
		}
		protected virtual void OnBordersChanged() {
		}
		protected virtual void OnFontChanged() {
		}
		protected virtual void OnForeColorChanged() {
		}
		protected internal bool CanAddComponent(XRControl control) {
			if(control == null)
				return false;
			if(Comparer.Equals(control, this))
				return false;
			if(control.IsOwnerOf(this))
				return false;
			return CanAddControl(control.GetType(), control);
		}
		protected internal virtual void SwapWith(XRControl item) {
			if(!ReferenceEquals(ControlContainer, item.ControlContainer))
				throw new ArgumentException("item");
			int index = Index;
			Index = item.Index;
			item.Index = index;
		}
		protected internal virtual bool CanAddControl(Type componentType, XRControl control) {
			if(!CanHaveChildren)
				return false;
			Type[] invalidTypes = new Type[] { typeof(XRTableRow), typeof(XRTableCell), typeof(SubreportBase), typeof(XRPageBreak), typeof(XRPivotGrid), typeof(XRTableOfContents) };
			foreach(Type type in invalidTypes) {
				if(type.IsAssignableFrom(componentType))
					return false;
			}
			return true;
		}
		internal bool IsOwnerOf(XRControl control) {
			if(control != null) {
				XRControl owner = control.Parent;
				while(owner != null) {
					if(owner == this)
						return true;
					owner = owner.Parent;
				}
			}
			return false;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return fXRControls.GetEnumerator();
		}
		protected virtual Band GetBand() {
			return parentInfo != null ? parentInfo.GetBand() : null;
		}
		protected virtual XtraReportBase GetReport() {
			return parentInfo != null ? parentInfo.GetReport() : null;
		}
		protected virtual XtraReport GetRootReport() {
			return parentInfo != null ? parentInfo.GetRootReport() : null;
		}
		protected internal virtual void UpdateLayout() {
		}
		protected internal virtual void OnParentBoundsChanged(XRControl parent, RectangleF oldBounds, RectangleF newBounds) {
			if(AnchorVertical != VerticalAnchorStyles.Top && AnchorVertical != VerticalAnchorStyles.None) {
				float deltaHeight = newBounds.Height - oldBounds.Height;
				if(this.AnchorVertical == VerticalAnchorStyles.Bottom) {
					if(FloatsComparer.Default.FirstGreaterSecond(parent.BottomF, BoundsF.Bottom + deltaHeight) ||
						FloatsComparer.Default.FirstEqualsSecond(parent.BottomF, BoundsF.Bottom + deltaHeight))
						this.BoundsF = RectHelper.OffsetRectF(this.BoundsF, 0, deltaHeight);
				} else
					this.HeightF += deltaHeight;
			}
			if(AnchorHorizontal != HorizontalAnchorStyles.Left && AnchorHorizontal != HorizontalAnchorStyles.None) {
				if(this.AnchorHorizontal == HorizontalAnchorStyles.Right)
					this.BoundsF = RectHelper.OffsetRectF(this.BoundsF, newBounds.Width - oldBounds.Width, 0);
				else
					this.WidthF += newBounds.Width - oldBounds.Width;
			}
		}
		protected internal virtual void SyncDpi(float dpi) {
			if(dpi != this.dpi)
				fBounds = XRConvert.Convert(fBounds, Dpi, dpi);
			fStyle.SyncDpi(dpi);
			foreach(XRControl control in fXRControls)
				control.SyncDpi(dpi);
			this.dpi = dpi;
		}
		public void SendToBack() {
			if(CanChangeZOrder && ControlContainer != null)
				Index = ControlContainer.Count - 1;
		}
		public void BringToFront() {
			if(CanChangeZOrder && ControlContainer != null)
				Index = 0;
		}
		protected internal virtual void CalculatePageSummary(DocumentBand pageBand) {
		}
		protected internal virtual void FinishPageSummary() {
		}
		protected internal virtual void InitializeScripts() {
			RootReport.EventsScriptManager.AddVariable(ValidateName(Name), this);
			Scripts.Initialize();
			foreach(XRControl control in fXRControls)
				control.InitializeScripts();
		}
		string ValidateName(string name) {
			return string.IsNullOrEmpty(name) ? name : name.Replace(" ", "_");
		}
		protected virtual void BeforeReportPrint() {
			foreach(XRControl control in fXRControls)
				control.BeforeReportPrint();
			RestoreInitialPropertyValues();
			foreach(XRBinding binding in dataBindings)
				binding.NullFields();
			SaveInitialPropertyValues();
			CreatePresenter().BeforeReportPrint();
		}
		protected virtual void AfterReportPrint() {
			foreach(XRControl control in fXRControls)
				control.AfterReportPrint();
			RestoreInitialPropertyValues();
		}
		protected internal virtual void SaveInitialPropertyValues() {
			foreach(XRBinding binding in DataBindings)
				binding.SaveInitialPropertyValue();
		}
		protected internal virtual void RestoreInitialPropertyValues() {
			foreach(XRBinding binding in DataBindings)
				binding.RestoreInitialPropertyValue();
			if(stateBag != null) {
				stateBag.Restore();
				stateBag.Clear();
			}
		}
		protected virtual void SetBrickText(VisualBrick brick, string text, object textValue) {
			brick.Text = text;
			brick.TextValue = textValue;
			UpdateMergeValue(brick, text);
		}
		static void UpdateMergeValue(VisualBrick brick, string text) {
			object mergeValue = null;
			if(brick.TryGetAttachedValue(BrickAttachedProperties.MergeValue, out mergeValue) && mergeValue is UpdatableBrickGroupInfo) {
				UpdatableBrickGroupInfo updatableBrickGroupInfo = mergeValue as UpdatableBrickGroupInfo;
				if(updatableBrickGroupInfo != null) {
					updatableBrickGroupInfo.UpdateValue(text);
				}
			}
		}
		protected virtual bool RaisePrintOnPage(VisualBrick brick, int pageIndex, int pageCount) {
			if(DesignMode || brick.BrickOwner != this)
				return true;
			if(Band != null && brick.BookmarkInfo.HasBookmark) {
				BookmarkHelper.SetCurrentBrick(brick, Band);
				if(BookmarkParent != null) {
					BookmarkParent.BookmarkHelper.SetChildBookmarkBrick(brick, Band);
				}
			}
			if(RootReport == null ||
				((PrintOnPageEventHandler)Events[PrintOnPageEvent] == null &&
				 !XREventsScriptManager.ContainsEventScript(this, EventNames.HandlerPrefix + EventNames.PrintOnPage)))
				return true;
			VisualBrick currentBrick = CreateInitializedBrickWithoutValidation(null, new XRWriteInfo(brick.PrintingSystem));
			bool currentVisibility = Visible;
			object currentTextValue = brick.TextValue;
			BookmarkInfo currentBookmarkParentInfo = brick.BookmarkInfo.ParentInfo;
			VisualBrick currentBookmarkParentBrick = brick.BookmarkInfo.ParentBrick;
			IgnoreEvenOddStyles = true;
			Visible = true;
			try {
				GetStateFromBrick(brick);
				PrintOnPageEventArgs args = new PrintOnPageEventArgs(pageIndex, pageCount);
				OnPrintOnPage(args);
				if(args.Cancel)
					return false;
				PutStateToBrickConditional(brick, brick.PrintingSystem);
				SetBrickText(brick, brick.Text, currentTextValue);
				brick.BookmarkInfo.ParentInfo = currentBookmarkParentInfo;
				brick.BookmarkInfo.ParentBrick = currentBookmarkParentBrick;
				return true;
			} finally {
				GetStateFromBrick(currentBrick);
				IgnoreEvenOddStyles = false;
				currentBrick.Dispose();
				Visible = currentVisibility;
			}
		}
		internal void AssignParent(XRControl parent, bool notify) {
			System.Diagnostics.Debug.Assert(!Comparer.Equals(parent, this));
			if(parent != fParent) {
				ClearParentInfo();
				XRControl oldParent = fParent;
				fParent = parent;
				if(notify && !ReportIsLoading)
					OnParentChanged(new ChangeEventArgs(oldParent, parent));
			}
		}
		void ClearParentInfo() {
			parentInfo.Clear();
			foreach(XRControl control in Controls)
				control.ClearParentInfo();
		}
		protected virtual void WriteContentToCore(XRWriteInfo writeInfo, VisualBrick brick) {
			writeInfo.DocBand.Bricks.Add(brick);
		}
		protected internal void WriteContentTo(XRWriteInfo writeInfo) {
			VisualBrick brick = GetPrintableBrick(writeInfo);
			WriteContentTo(writeInfo, brick);
			RaiseAfterPrint(EventArgs.Empty);
		}
		protected virtual void WriteContentTo(XRWriteInfo writeInfo, VisualBrick brick) {
			if(brick != null)
				WriteContentToCore(writeInfo, brick);
		}
		internal RectangleF RectangleFToBand(RectangleF r) {
			r.Location = PointToBand(r.Location);
			return r;
		}
		internal RectangleF RectangleFToBand(RectangleF r, float outDpi) {
			return XRConvert.Convert(RectangleFToBand(r), Dpi, outDpi);
		}
		PointF PointToBand(PointF pt) {
			XRControl ctl = this;
			while(!(ctl is Band)) {
				pt.X += ctl.BoundsF.Left;
				pt.Y += ctl.BoundsF.Top;
				ctl = ctl.PrintableParent;
			}
			return pt;
		}
		internal RectangleF RectangleFFromBand(RectangleF r) {
			r.Location = PointFromBand(r.Location);
			return r;
		}
		PointF PointFromBand(PointF pt) {
			XRControl ctl = this;
			while(!(ctl is Band)) {
				pt.X -= ctl.BoundsF.Left;
				pt.Y -= ctl.BoundsF.Top;
				ctl = ctl.Parent;
			}
			return pt;
		}
		internal PointF SnapPoint(PointF value) {
			PointF result = PointToBand(value);
			result = Divider.GetDivisibleValue(result, this.RootReport.GridSizeF);
			return PointFromBand(result);
		}
		internal float ValidateRight(float right, float step) {
			return right <= LocationF.X ? right + step : right;
		}
		internal float ValidateBottom(float bottom, float step) {
			return ValidateBottom(bottom, RightBottomParent, step);
		}
		internal float ValidateBottom(float bottom, XRControl bottomParent, float step) {
			return bottom <= LocationF.Y && LocationParent == bottomParent ?
				ValidateBottom(bottom + step, bottomParent, step) :
				bottom;
		}
		protected virtual VisualBrick GetPrintableBrick(XRWriteInfo writeInfo) {
			PrintEventArgs args = new PrintEventArgs();
			OnBeforePrint(args);
			if(args.Cancel)
				return null;
			VisualBrick[] childrenBricks;
			if(Controls.Count > 0) {
				List<VisualBrick> childrenList = new List<VisualBrick>();
				foreach(XRControl control in Controls) {
					VisualBrick brick = control.GetPrintableBrick(writeInfo);
					if(brick != null)
						childrenList.Insert(0, brick);
				}
				childrenBricks = childrenList.ToArray();
			} else
				childrenBricks = new VisualBrick[0];
			VisualBrick initBrick = CreateInitializedBrick(childrenBricks, writeInfo);
			return initBrick;
		}
		protected internal virtual VisualBrick GetDesignerBrick(PrintingSystemBase ps, XRWriteInfo writeInfo) {
			List<VisualBrick> childData = new List<VisualBrick>();
			for(int i = Controls.Count - 1; i >= 0; i--) {
				VisualBrick item = Controls[i].GetDesignerBrick(ps, writeInfo);
				if(item != null)
					childData.Add(item);
			}
			return CreateInitializedBrick(childData.ToArray(), writeInfo);
		}
		void IBrickOwner.AddToSummaryUpdater(VisualBrick brick, VisualBrick prototypeBrick) {
			AddToSummaryUpdater(brick, prototypeBrick);
		}
		protected virtual void AddToSummaryUpdater(VisualBrick brick, VisualBrick prototypeBrick) { }
		internal VisualBrick CreateInitializedBrick(VisualBrick[] childrenBricks, XRWriteInfo writeInfo) {
			VisualBrick brick = CreateInitializedBrickWithoutValidation(childrenBricks, writeInfo);
			ValidateBrick(brick, writeInfo.Bounds, writeInfo.Offset);
			OnBrickCreated(new BrickEventArgsBase(brick));
			return brick;
		}
		VisualBrick CreateInitializedBrickWithoutValidation(VisualBrick[] childrenBricks, XRWriteInfo writeInfo) {
			VisualBrick brick = CreateBrick(childrenBricks != null ? childrenBricks : new VisualBrick[0]);
			VisualBrickHelper.SetBrickBounds(brick, BoundsF, Dpi);
			PutStateToBrickConditional(brick, writeInfo.PrintingSystem);
			VisualBrickHelper.InitializeBrick(brick, writeInfo.PrintingSystem, brick.Rect);
			return brick;
		}
		protected virtual void ValidateBrick(VisualBrick brick, RectangleF bounds, PointF offset) {
		}
		protected virtual VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new VisualBrick(this);
		}
		void PutStateToBrickConditional(VisualBrick brick, PrintingSystemBase ps) {
			if(NeedSuppressValue(ValueSuppressType.SuppressAndShrink, ProcessDuplicatesMode.SuppressAndShrink) && !DesignMode) {
				ReassignStyle(brick, new BrickStyle());
				VisualBrickHelper.SetCanShrink(brick, true);
			} else
				PutStateToBrick(brick, ps);
			if(!HasChildren && ProcessDuplicatesMode == ProcessDuplicatesMode.Merge) {
				object mergeValue = GetMergeValue();
				if(mergeValue != null) {
					brick.SetAttachedValue(BrickAttachedProperties.MergeValue, mergeValue);
				}
			}
			brick.CanPublish = CanPublish;
		}
		protected virtual object GetMergeValue() {
			MultiColumn mc;
			if(Band.TryGetMultiColumn(Band, out mc) && mc.Layout == ColumnLayout.AcrossThenDown)
				return null;
			if(processDuplicatesMode == ProcessDuplicatesMode.Merge 
				&& (ProcessDuplicatesTarget == ProcessDuplicatesTarget.Tag && !NeedSuppressDuplicatesByTag()
				  || ProcessDuplicatesTarget == ProcessDuplicatesTarget.Value && !NeedSuppressDuplicatesByValue()))
				mergeGroupIndex++;
			object mergeValue = null;
			if(ProcessDuplicatesMode == ProcessDuplicatesMode.Merge) {
				if(ProcessDuplicatesTarget == ProcessDuplicatesTarget.Tag) {
					mergeValue = new MultiKey(this, Tag, mergeGroupIndex);
				} else {
					mergeValue = new UpdatableBrickGroupInfo(this, GetValueForMergeKey(), mergeGroupIndex);
				}
			}
			return mergeValue;
		}
		protected void SetTextProperties(VisualBrick brick) {
			brick.Text = !IsEmptyValue(Text) ? Text : NullValueText;
			brick.TextValue = GetTextValue();
			brick.TextValueFormatString = GetTextValueFormatString();
			brick.XlsxFormatString = xlsxFormatString;
		}
		protected internal virtual void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			ReassignStyle(brick, ps.Document.State == DocumentState.PostProcessing ?
				(BrickStyle)fStyle.Clone() :
				GetEffectiveXRStyle());
			if(!CanDrawBackground)
				brick.Style.BackColor = Color.Transparent;
			brick.Url = UrlResolver.ResolveUrl(NavigateUrl);
			brick.Target = Target;
			if(this.RootReport.NavigationManager.TargetsContains(this))
				brick.AnchorName = Name;
			brick.Value = Tag;
			brick.BookmarkInfo = CreateBookmarkInfo();
			SetShrinkGrow(brick);
			CreatePresenter().PutStateToBrick(brick, ps);
		}
		static void ReassignStyle(VisualBrick brick, BrickStyle newStyle) {
			if(!brick.IsInitialized) {
				BrickStyle initial = brick.Style;
				brick.Style = newStyle;
				initial.Dispose();
			} else {
				brick.Style = newStyle;
			}
		}
		protected virtual ControlPresenter CreatePresenter() {
			return CreatePresenter<ControlPresenter>(
				delegate() { return new ControlPresenter(this); },
				delegate() { return new ControlPresenter(this); },
				delegate() { return new LayoutViewControlPresenter(this); }
			);
		}
		protected virtual void SetShrinkGrow(VisualBrick brick) {
			VisualBrickHelper.SetCanGrow(brick, CanGrow);
			VisualBrickHelper.SetCanShrink(brick, CanShrink);
		}
		protected BookmarkInfo CreateBookmarkInfo() {
			BookmarkInfo parentInfo = BookmarkParent == null ? null : BookmarkParent.CreateBookmarkInfo();
			if(string.IsNullOrEmpty(Bookmark) && parentInfo == null)
				return ((IBrickOwner)this).EmptyBookmarkInfo;
			return new BookmarkInfo(this, Bookmark, parentInfo);
		}
		protected internal virtual void GetStateFromBrick(VisualBrick brick) {
			SetStyle((XRControlStyle)brick.Style.Clone());
			Text = brick.Text;
			NavigateUrl = brick.Url;
			Tag = brick.Value;
			Bookmark = brick.BookmarkInfo.Bookmark;
			CanGrow = VisualBrickHelper.GetCanGrow(brick);
			CanShrink = VisualBrickHelper.GetCanShrink(brick);
		}
		protected internal virtual void UpdateBrickBounds(VisualBrick brick) {
			brick.SetBoundsHeight(CalculateBrickHeight(brick), Dpi);
		}
		protected virtual float CalculateBrickHeight(VisualBrick brick) {
			return GetBrickBounds(brick).Height;
		}
		protected RectangleF GetBrickBounds(Brick brick) {
			return GraphicsUnitConverter.Convert(brick.InitialRect, GraphicsDpi.Document, Dpi);
		}
		#region IBrickOwner implementation
		BrickOwnerType IBrickOwner.BrickOwnerType { get { return BrickOwnerType; } }
		protected virtual BrickOwnerType BrickOwnerType { get { return BrickOwnerType.Control; } }
		TextEditMode IBrickOwner.TextEditMode { get { return TextEditMode; } }
		protected virtual TextEditMode TextEditMode { get { return TextEditMode.Unavailable; } }
		ControlLayoutRules IBrickOwner.LayoutRules { get { return LayoutRules; } }
		protected virtual ControlLayoutRules LayoutRules { get { return ControlLayoutRules.AllSizeable | ControlLayoutRules.Moveable; } }
		bool IBrickOwner.IsImageEditable { get { return IsImageEditable; } }
		protected virtual bool IsImageEditable { get { return false; } }
		bool? IBrickOwner.HasImage { get { return HasImage; } }
		protected virtual bool? HasImage { get { return null; } }
		string IBrickOwner.ControlsUnityName { get { return ControlsUnityName; } }
		protected virtual string ControlsUnityName { get { return null; } }
		bool IBrickOwner.IsNavigationLink {
			get {
				return !string.IsNullOrEmpty(this.NavigateUrl) && this.Target == "_self";
			}
		}
		bool IBrickOwner.IsNavigationTarget {
			get {
				return RootReport != null ? RootReport.NavigationManager.TargetsContains(this) : false;
			}
		}
		bool IBrickOwner.NeedCalcContainerHeight { get { return NeedCalcContainerHeight; } }
		bool IBrickOwner.HasPageSummary { get { return HasPageSummary; } }
		ConvertHelper IBrickOwner.ConvertHelper {
			get {
				if(convertHelper == null)
					convertHelper = new SmartConvertHelper();
				return convertHelper;
			}
		}
		BookmarkInfo IBrickOwner.EmptyBookmarkInfo {
			get { return BookmarkInfo.Empty; }
		}
		bool IBrickOwner.CanCacheImages { get { return Events[DrawEvent] == null && !ScriptManagerContainsEventScript(EventNames.HandlerPrefix + EventNames.Draw); } }
		bool IBrickOwner.InSubreport { get { return RootReport.MasterReport != null; } }
		bool IBrickOwner.IsCrossbandControl { get { return IsCrossbandControl; } }
		bool IBrickOwner.LockedInDesigner { get { return LockedInUserDesigner; } }
		protected virtual bool IsCrossbandControl { get { return false; } }
		bool ScriptManagerContainsEventScript(string eventName) {
			return RootReport != null && XREventsScriptManager.ContainsEventScript(this, eventName);
		}
		bool IBrickOwner.IsSeparableVert(bool isBrickSeparableVert) {
			return !KeepTogether;
		}
		bool IBrickOwner.IsSeparableHorz(bool isBrickSeparableHorz) {
			return true;
		}
		void IBrickOwner.RaiseDraw(VisualBrick brick, IGraphics gr, RectangleF bounds) {
			OnDraw(new DrawEventArgs(gr, bounds, brick));
		}
		void IBrickOwner.RaiseHtmlItemCreated(VisualBrick brick, IScriptContainer scriptContainer, DXHtmlContainerControl contentCell) {
			OnHtmlItemCreated(new HtmlEventArgs(scriptContainer, contentCell, brick));
		}
		void IBrickOwner.UpdateBrickBounds(VisualBrick brick) {
			UpdateBrickBounds(brick);
		}
		void IBrickOwner.RaiseSummaryCalculated(VisualBrick brick, string text, string format, object value) {
			RaiseSummaryCalculated(brick, text, format, value);
		}
		bool IBrickOwner.RaiseAfterPrintOnPage(VisualBrick brick, int pageNumber, int pageCount) {
			if(RaisePrintOnPage(brick, pageNumber, pageCount))
				return true;
			brick.IsVisible = false;
			return false;
		}
		Font IBrickOwner.GetActualFont() {
			return GetEffectiveFont();
		}
		Color IBrickOwner.GetActualForeColor() {
			return GetEffectiveForeColor();
		}
		Color IBrickOwner.GetActualBackColor() {
			return GetEffectiveBackColor();
		}
		Color IBrickOwner.GetActualBorderColor() {
			return GetEffectiveBorderColor();
		}
		float IBrickOwner.GetActualBorderWidth() {
			return GetEffectiveBorderWidth();
		}
		BorderDashStyle IBrickOwner.GetActualBorderDashStyle() {
			return GetEffectiveBorderDashStyle();
		}
		BorderSide IBrickOwner.GetActualBorderSide() {
			return GetEffectiveBorders();
		}
		TextAlignment IBrickOwner.GetActualTextAlignment() {
			return GetEffectiveTextAlignment();
		}
		object IBrickOwner.RealControl { get { return RealControl; } }
		#endregion
		internal void UpdateBinding(XRDataContext dataContext, ImagesContainer images) {
			previousValue = Report != null && Report.DataBrowser.Position > 0 ? GetValueForSuppress() : initialValue;
			previousTag = Report != null && Report.DataBrowser.Position > 0 ? Tag : initialValue;
			UpdateBindingCore(dataContext, images);
			for(int i = Controls.Count - 1; i >= 0; i--)
				Controls[i].UpdateBinding(dataContext, images);
		}
		protected virtual void UpdateBindingCore(XRDataContext dataContext, ImagesContainer images) {
			foreach(XRBinding binding in dataBindings) {
				binding.UpdatePropertyValue(dataContext, images);
			}
		}
		protected internal virtual object GetTextValue() {
			XRBinding binding = DataBindings["Text"];
			return binding != null ? binding.ColumnValue : null;
		}
		protected internal virtual string GetTextValueFormatString() {
			XRBinding binding = DataBindings["Text"];
			return binding != null ? binding.FormatString : null;
		}
		static protected float InflateWidth(float width, float dx) {
			return width + 2 * dx;
		}
		public virtual Image ToImage() {
			return ToImageCore(null);
		}
		public virtual Image ToImage(System.Drawing.Text.TextRenderingHint textRenderingHint) {
			return ToImageCore(gr => gr.TextRenderingHint = textRenderingHint);
		}
		Image ToImageCore(Action<Graphics> callback) {
			using(PrintingSystemBase ps = new PrintingSystemBase()) {
				VisualBrick brick = GetDesignerBrick(ps, new XRWriteInfo(ps));
				if(brick != null) {
					VisualBrickExporter exporter = ExportersFactory.CreateExporter(brick) as VisualBrickExporter;
					if(exporter != null) {
						RectangleF rect = new RectangleF(PointF.Empty, XRConvert.Convert(SizeF, Dpi, GraphicsDpi.Pixel));
						BrickImageProviderBase imageProvider = exporter.CreateImageProvider();
						return imageProvider.CreateImage(ps, rect, GraphicsDpi.Pixel, callback);
					}
				}
			}
			return null;
		}
		internal virtual XRControl RealControl {
			get {
				return this;
			}
		}
		internal bool IsRealControl {
			get {
				return this == RealControl;
			}
		}
		protected virtual void SetBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			SetBoundsAndUpdateLayout(x, y, width, height, specified);
		}
		protected void SetBoundsAndUpdateLayout(float x, float y, float width, float height, BoundsSpecified specified) {
			SetBoundsCore(x, y, width, height, specified);
			UpdateLayout();
		}
		internal void SetBoundsCore(RectangleF rect) {
			SetBoundsCore(rect.X, rect.Y, rect.Width, rect.Height, BoundsSpecified.All);
		}
		internal void SetBoundsCore(PointF location, SizeF size) {
			SetBoundsCore(location.X, location.Y, size.Width, size.Height, BoundsSpecified.All);
		}
		internal void SetBoundsCore(float x, float y, float width, float height) {
			SetBoundsCore(x, y, width, height, BoundsSpecified.All);
		}
		internal void SetBoundsCore(float x, float y, float width, float height, BoundsSpecified specified) {
			if(BoundsChanging)
				return;
			BoundsChanging = true;
			RectangleF oldValue = fBounds;
			if(SpecifiedContains(specified, BoundsSpecified.X))
				fBounds.X = Math.Max(0, x);
			if(SpecifiedContains(specified, BoundsSpecified.Y))
				fBounds.Y = Math.Max(0, y);
			if(SpecifiedContains(specified, BoundsSpecified.Width))
				fBounds.Width = Math.Max(GetMinimumWidth(), width);
			if(SpecifiedContains(specified, BoundsSpecified.Height))
				fBounds.Height = Math.Max(GetMinimumHeight(), height);
			if(!ReportIsLoading) {
				if(oldValue.Location != fBounds.Location)
					OnLocationChanged(new ChangeEventArgs(oldValue.Location, fBounds.Location));
				if(oldValue.Size != fBounds.Size)
					OnSizeChanged(new ChangeEventArgs(oldValue.Size, fBounds.Size));
				if(oldValue != fBounds)
					OnBoundsChanged(oldValue, BoundsF);
				UpdateBandHeight();
			}
			BoundsChanging = false;
		}
		protected virtual void OnBoundsChanged(RectangleF oldBounds, RectangleF newBounds) {
			foreach(XRControl item in this.Controls)
				item.OnParentBoundsChanged(this, oldBounds, newBounds);
		}
#if DEBUG
		public
#else
		internal
#endif
 Size GetMinSize() {
			return new Size(GetMinimumWidth(), GetMinimumHeight());
		}
		protected virtual int GetMinimumWidth() {
			return GetMinimumWidth(Dpi);
		}
		internal protected virtual int GetMinimumHeight() {
			return GetMinimumHeight(Dpi);
		}
		protected float UpdateAutoHeight(VisualBrick brick, float newHeight) {
			float brickHeight = VisualBrickHelper.GetBrickBounds(brick, Dpi).Height;
			bool updateBounds = false;
			if(VisualBrickHelper.GetCanGrow(brick))
				updateBounds = newHeight > brickHeight;
			if(VisualBrickHelper.GetCanShrink(brick) && !updateBounds)
				updateBounds = newHeight < brickHeight;
			return updateBounds ? newHeight : brickHeight;
		}
		internal protected void UpdateBandHeight() {
			if(Parent is Band) ((Band)Parent).UpdateHeight();
		}
		protected internal virtual int GetWeightingFactor() {
			return 0;
		}
		protected virtual void RaiseSummaryCalculated(VisualBrick brick, string text, string format, object value) {
		}
		internal BorderSide GetVisibleBorders(Band band) {
			if(this.LocationParent == band && this.RightBottomParent != band) {
				return BorderSide.Left | BorderSide.Top | BorderSide.Right;
			} else if(this.LocationParent != band && this.RightBottomParent == band) {
				return BorderSide.Left | BorderSide.Right | BorderSide.Bottom;
			} else if(this.LocationParent != band && this.RightBottomParent != band) {
				return BorderSide.Left | BorderSide.Right;
			} else
				return BorderSide.All;
		}
		#region style functions
		internal RectangleF DeflateBorderWidth(RectangleF rect) {
			return DeflateBorderWidth(rect, Dpi);
		}
		internal RectangleF DeflateBorderWidth(RectangleF rect, float dpi) {
			float borderWidth = XRConvert.Convert(GetEffectiveBorderWidth(), GraphicsDpi.Pixel, dpi);
			return BrickStyle.DeflateBorderWidth(rect, GetEffectiveBorders(), borderWidth);
		}
		internal RectangleF InflateBorderWidth(RectangleF rect) {
			return InflateBorderWidth(rect, Dpi);
		}
		internal RectangleF InflateBorderWidth(RectangleF rect, float dpi) {
			float borderWidth = XRConvert.Convert(GetEffectiveBorderWidth(), GraphicsDpi.Pixel, dpi);
			return BrickStyle.InflateBorderWidth(rect, GetEffectiveBorders(), borderWidth);
		}
		protected internal virtual XRControlStyle GetEffectiveXRStyle() {
			if(RootReport == null)
				return fStyle;
			XRControlStyle style = new XRControlStyle();
			CollectStyle(style, this, StyleProperty.All);
			Font validFont = style.FontInPoints.Validate(Site);
			if(validFont != style.Font)
				style.SetBaseFont(validFont);
			style.StringFormat = BrickStringFormat.Create(style.TextAlignment, WordWrap, TextTrimming);
			style.SyncDpi(Dpi);
			return style;
		}
		#region Effective style property values
		public Color GetEffectiveBackColor() {
			return (Color)GetEffectiveValue(StyleProperty.BackColor);
		}
		public Color GetEffectiveBorderColor() {
			return (Color)GetEffectiveValue(StyleProperty.BorderColor);
		}
		public virtual BorderDashStyle GetEffectiveBorderDashStyle() {
			return (BorderDashStyle)GetEffectiveValue(StyleProperty.BorderDashStyle);
		}
		public BorderSide GetEffectiveBorders() {
			return (BorderSide)GetEffectiveValue(StyleProperty.Borders);
		}
		public float GetEffectiveBorderWidth() {
			return (float)GetEffectiveValue(StyleProperty.BorderWidth);
		}
		public virtual Font GetEffectiveFont() {
			Font font = (Font)GetEffectiveValue(StyleProperty.Font);
			return font.Validate(Site);
		}
		public virtual Color GetEffectiveForeColor() {
			return (Color)GetEffectiveValue(StyleProperty.ForeColor);
		}
		public PaddingInfo GetEffectivePadding() {
			return new PaddingInfo((PaddingInfo)GetEffectiveValue(StyleProperty.Padding), Dpi);
		}
		public TextAlignment GetEffectiveTextAlignment() {
			return (TextAlignment)GetEffectiveValue(StyleProperty.TextAlignment);
		}
		#endregion
		internal void ClearStyleNamesRecursive(params string[] styleNames) {
			foreach(string styleName in styleNames) {
				if(styleName == this.StyleName)
					this.StyleName = string.Empty;
				else if(styleName == this.OddStyleName)
					this.OddStyleName = string.Empty;
				else if(styleName == this.EvenStyleName)
					this.EvenStyleName = string.Empty;
			}
			foreach(XRControl item in this.Controls)
				item.ClearStyleNamesRecursive(styleNames);
		}
		protected internal virtual bool HasExportWarning() {
			if(!CanHaveExportWarning)
				return false;
			return IntersectsWithChildren(3) || IntersectsWithParent(3) || IntersectsWithSiblings(3);
		}
		protected virtual bool IntersectsWithSiblings(int digits) {
			RectangleF rect = GetClippedBandBounds();
			foreach(XRControl sibling in PrintableParent.GetPrintableControls()) {
				if(sibling == this || !sibling.CanHaveExportWarning)
					continue;
				if(RectHelper.RectangleFIntersects(rect, sibling.GetClippedBandBounds(), digits))
					return true;
			}
			return false;
		}
		protected virtual bool IntersectsWithChildren(int digits) {
			RectangleF rect = GetClippedBandBounds();
			foreach(XRControl child in this.GetPrintableControls()) {
				if(!child.CanHaveExportWarning)
					continue;
				RectangleF childRect = RectangleFToBand(child.BoundsF);
				if(!RectHelper.RectangleFContains(rect, childRect, digits))
					return true;
			}
			return false;
		}
		protected virtual BorderSide GetCorrectBorders() { 
			return GetEffectiveBorders();
		}
		protected virtual bool IntersectsWithParent(int digits) {
			if(!PrintableParent.CanHaveExportWarning)
				return false;
			RectangleF rect = PrintableParent.RectangleFToBand(BoundsF);
			RectangleF parentRect = PrintableParent.GetClippedBandBounds();
			RectangleF parentRect2 = PrintableParent.RectangleFToBand(PrintableParent.ClientRectangleF);
			parentRect2 = SubtractBorders(parentRect2, PrintableParent.ActualBorderWidth, PrintableParent.GetCorrectBorders());
			parentRect.Intersect(parentRect2);
			if(!RectHelper.RectangleFContains(parentRect, rect, digits))
				return true;
			return false;
		}
		static RectangleF SubtractBorders(RectangleF rect, float borderWidth, BorderSide borders) {
			if((borders & BorderSide.Left) > 0) {
				rect.X += borderWidth;
				rect.Width -= borderWidth;
			}
			if((borders & BorderSide.Right) > 0) {
				rect.Width -= borderWidth;
			}
			if((borders & BorderSide.Top) > 0) {
				rect.Y += borderWidth;
				rect.Height -= borderWidth;
			}
			if((borders & BorderSide.Bottom) > 0) {
				rect.Height -= borderWidth;
			}
			return rect;
		}
		internal protected XRControlStyle GetStyle(string styleName) {
			return (RootReport != null && !string.IsNullOrEmpty(styleName)) ?
				ValidateStyle(RootReport.StyleContainer[styleName]) : null;
		}
		XRControlStyle GetStyleSheetStyle(int rowIndex) {
			XRControlStyle result = null;
			if(rowIndex >= 0 && !IgnoreEvenOddStyles && !DesignMode && ((Band is DetailBand) || (Band is SubBand && Band.Parent is DetailBand)))
				result = rowIndex % 2 == 0 ? EvenStyleCore : OddStyleCore;
			return result != null ? result : StyleCore;
		}
		protected string RegisterStyle(XRControlStyle style) {
			if(RootReport != null && style != null) {
				if(style.Owner == null)
					RootReport.StyleSheet.Add(style);
				return style.Name;
			}
			return string.Empty;
		}
		internal virtual void SyncStyleName(string oldName, string newName) {
			if(Object.Equals(oldName, newName))
				return;
			if(newName == null)
				newName = "";
			if(evenStyleName == oldName)
				evenStyleName = newName;
			if(oddStyleName == oldName)
				oddStyleName = newName;
			if(styleName == oldName)
				styleName = newName;
			for(int i = 0; i < Controls.Count; i++)
				Controls[i].SyncStyleName(oldName, newName);
		}
		protected internal bool IsAttachedStyle(XRControlStyle style) {
			List<XRControlStyle> attachedStyles = new List<XRControlStyle>();
			FillAttachedStyles(attachedStyles);
			return style != null && attachedStyles.Contains(style);
		}
		protected internal bool IsAttachedRule(FormattingRule rule) {
			List<FormattingRule> attachedRules = new List<FormattingRule>();
			FillAttachedRules(attachedRules);
			return rule != null && attachedRules.Contains(rule);
		}
		bool IsParent(XRControl parent) {
			return this == RootReport ? false : Parent == parent || Parent.IsParent(parent);
		}
		protected internal virtual bool HasPrintingWarning() {
			return FloatsComparer.Default.FirstGreaterSecond(GetClippedBandBounds().Right, Band.ClientRectangleF.Width);
		}
		protected void FillAttachedStyles(List<XRControlStyle> attachedStyles) {
			for(int i = 0; i < Controls.Count; i++)
				Controls[i].FillAttachedStyles(attachedStyles);
			if(attachedStyles == null)
				return;
			AddAttachedStyle(attachedStyles, Styles.EvenStyle);
			AddAttachedStyle(attachedStyles, Styles.OddStyle);
			AddAttachedStyle(attachedStyles, Styles.Style);
		}
		protected void FillAttachedRules(List<FormattingRule> attachedRules) {
			for(int i = 0; i < Controls.Count; i++)
				Controls[i].FillAttachedRules(attachedRules);
			if(formattingRules == null)
				return;
			AddAttachedRules(attachedRules, formattingRules);
		}
		object GetEffectiveValue(StyleProperty styleProperty) {
			using(XRControlStyle style = new XRControlStyle()) {
				CollectStyle(style, this, styleProperty);
				return style.GetValue(styleProperty);
			}
		}
		void ConvertStyle(StyleProperty property, XRControlStyle style) {
			if(ParentStyleUsing.IsSet(property))
				style.Reset(property);
			else if(!style.IsSet(property) && property == StyleProperty.Padding)
				style.Padding = GetDefaultPadding(Dpi);
			else if(!style.IsSet(property))
				XRControlStyle.Default.CopyProperties(style, property);
		}
		protected virtual PaddingInfo GetDefaultPadding(float dpi) {
			return new PaddingInfo(XRControlStyle.Default.Padding, dpi);
		}
		#endregion
		#region Suppressing values
		bool NeedSuppressValue(ValueSuppressType suppressType, ProcessDuplicatesMode duplicatesMode) {
			return HasChildren ? false : (processNullValues == suppressType && NeedSuppressNullValue())
				|| (processDuplicatesMode == duplicatesMode 
				&& ((processDuplicatesTarget == ProcessDuplicatesTarget.Tag && NeedSuppressDuplicatesByTag()) || ( processDuplicatesTarget == ProcessDuplicatesTarget.Value && NeedSuppressDuplicatesByValue())));
		}
		protected virtual object GetValueForSuppress() {
			return Text;
		}
		protected virtual object GetValueForMergeKey() {
			return GetValueForSuppress();
		}
		protected virtual bool NeedSuppressNullValue() {
			return IsEmptyValue(Text);
		}
		protected virtual bool NeedSuppressDuplicatesByValue() {
			return IsEmptyValue(previousValue) || IsEmptyValue(GetValueForSuppress()) ? false : object.Equals(previousValue, GetValueForSuppress());
		}
		bool NeedSuppressDuplicatesByTag() {
			return object.Equals(previousTag, Tag);
		}
		protected virtual bool IsEmptyValue(object value) {
			if(value == null || value == DBNull.Value)
				return true;
			string stringValue = value as string;
			if(stringValue != null && stringValue.Length == 0)
				return true;
			return false;
		}
		#endregion
		#region IScriptable Members
		XRScriptsBase IScriptable.Scripts {
			get { return fEventScripts; }
		}
		#endregion
		protected internal virtual bool Snapable {
			get { return true; }
		}
		protected internal virtual bool SupportSnapLines {
			get { return RootReport == null || RootReport.SnappingMode == SnappingMode.SnapLines; }
		}
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return CreateCollectionItem(propertyName, e);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			SetIndexCollectionItem(propertyName, e);
		}
		protected virtual void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Controls)
				Controls.Add(e.Item.Value as XRControl);
			else if(propertyName == XtraReportsSerializationNames.DataBindings)
				DataBindings.Add(e.Item.Value as XRBinding);
			else if(propertyName == XtraReportsSerializationNames.FormattingRuleLinks)
				FormattingRuleLinks.Add(e.Item.Value as ItemLink);
		}
		protected virtual object CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Controls)
				return CreateControl(e);
			if(propertyName == XtraReportsSerializationNames.DataBindings)
				return CreateBinding(e.Item.ChildProperties);
			if(propertyName == XtraReportsSerializationNames.FormattingRuleLinks)
				return new ItemLink();
			return null;
		}
		XRBinding CreateBinding(IXtraPropertyCollection properites) {
			string propertyName = properites["PropertyName"] != null ? properites["PropertyName"].Value as string : string.Empty;
			string dataMember = properites["DataMember"] != null ? properites["DataMember"].Value as string : string.Empty;
			return new XRBinding(propertyName, null, dataMember);
		}
		protected XRControl CreateControl(XtraItemEventArgs e) {
			if(e.Item.ChildProperties["ControlType"] != null)
				return (XRControl)Activator.CreateInstance(XRControlExtensions.GetObjectType((string)e.Item.ChildProperties["ControlType"].Value));
			return new XRControl();
		}
		#endregion
		#region IXtraSerializable Members
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			OnEndDeserializing(restoredVersion);
		}
		protected virtual void OnEndDeserializing(string restoredVersion) {
			isDeserializing = false;
			if(this is ISupportInitialize)
				((ISupportInitialize)this).EndInit();
		}
		protected virtual void OnStartSerializing() {
			this.isSerializing = true;
			FormattingRuleLinks.Clear();
			FormattingRuleLinks.CreateItemsFrom(FormattingRules);
		}
		void IXtraSerializable.OnEndSerializing() {
			this.isSerializing = false;
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			OnStartDeserializing(e);
		}
		protected virtual void OnStartDeserializing(LayoutAllowEventArgs e) {
			isDeserializing = true;
			if(this is ISupportInitialize)
				((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnStartSerializing() {
			OnStartSerializing();
		}
		protected internal virtual void OnReportEndDeserializing() {
			FormattingRuleLinks.ApplyItemsTo(FormattingRules);
			FormattingRuleLinks.Clear();
		}
		#endregion
	}
}
namespace DevExpress.XtraReports.UI {
	using System.Linq;
	public static class XRControlStyleExtensions {
		internal static void ApplyStyleUsings(this IEnumerable<XRControlStyle> styles) {
			foreach(XRControlStyle style in styles)
				if(style != null)
					style.ApplyStyleUsing();
		}
		internal static void ClearDirty(this IEnumerable<XRControlStyle> styles) {
			foreach(XRControlStyle style in styles)
				if(style != null)
					style.Dirty = false;
		}
		internal static string[] GetNames(this IEnumerable<XRControlStyle> styles) {
			HashSet<string> names = new HashSet<string>();
			foreach(XRControlStyle item in styles)
				names.Add(item.Name);
			return names.ToArray<string>();
		}
		public static void ApplyProperties(this XRControlStyle src, XRControlStyle dest, StyleProperty srcProperties, ref StyleProperty properties) {
			StyleProperty destProperties = srcProperties & properties;
			if(destProperties != StyleProperty.None) {
				src.CopyProperties(dest, destProperties);
				properties &= ~destProperties;
			}
		}
	}
}
namespace System.Drawing {
	using DevExpress.XtraReports.Localization;
	static class FontExtensions {
		public static Font Validate(this Font font, ISite site) {
			if(font == null)
				throw new Exception(ReportStringId.Msg_WarningFontNameCantBeEmpty.GetString());
			if(site == null)
				return font;
			try {
				int ignoredHeight = font.Height;
				return font;
			} catch {
				return new Font(font, font.Style);
			}
		}
	}
}
