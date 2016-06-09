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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using DevExpress.XtraReports.Localization;
using System.IO;
using System.Reflection;
using System.Net;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using System.Collections.Specialized;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native.TextRotation;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls), 
	XRDesigner("DevExpress.XtraReports.Design.XRLabelDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRLabelDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultBindableProperty("Text"),
	DefaultProperty("Text"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRLabel.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRLabel", "Label"),
	XRToolboxSubcategoryAttribute(0, 0),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRLabel.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRLabel.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRLabel : XRFieldEmbeddableControl {
		#region static
		static bool ShouldSerializeProperty(object obj, string propertyName, object propertyValue) {
			try {
				DefaultValueAttribute defaultValueAttribute = TypeDescriptor.GetProperties(obj)[propertyName].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
				return defaultValueAttribute != null && !Comparer.Equals(defaultValueAttribute.Value, propertyValue);
			} catch {
				return false;
			}
		}
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
		static Size MeasureTextCore(string text, float width, float dpi, BrickStyle bs, Measurer measurer) {
			float widthF = XRConvert.Convert(width, dpi, GraphicsDpi.Document);
			widthF = bs.Padding.DeflateWidth(widthF, GraphicsDpi.Document);
			SizeF size = SizeF.Empty;
			if(widthF > 0) {
				bs.StringFormat.SetTabStops(bs.CalculateTabStops(measurer));
				size = measurer.MeasureString(text, bs.Font, widthF, bs.StringFormat.Value, GraphicsUnit.Document);
			} else {
				size = measurer.MeasureString(text, bs.Font, GraphicsUnit.Document);
			}
			size.Height += XRConvert.Convert(2f, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Document);
			size = bs.Padding.Inflate(size, GraphicsDpi.Document);
			return System.Drawing.Size.Ceiling(XRConvert.Convert(size, GraphicsDpi.Document, dpi));
		}
		static string ReplaceVBCR(string str) {
			str = str.Replace("\r\n\r", "\r\r");
			str = str.Replace("\r\n", "\r");
			str = str.Replace("\n\r", "\r");
			str = str.Replace("\n", "\r");
			str = str.Replace("\r", "\r\n");
			return str;
		}
		#endregion
		new public static class EventNames {
			public const string SummaryCalculated = "SummaryCalculated";
			public const string SummaryGetResult = "SummaryGetResult";
			public const string SummaryReset = "SummaryReset";
			public const string SummaryRowChanged = "SummaryRowChanged";
		}
		internal const string SummaryProgressSign = "?";
		#region Fields & Properties
		float angle;
		bool multiline;
		bool autoWidth;
		XRSummary fSummary = new XRSummary();
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRLabelDefaultPadding")]
#endif
		public static PaddingInfo DefaultPadding {
			get { return textPadding; }
		}
		protected override PaddingInfo GetDefaultPadding(float dpi) {
			return new PaddingInfo(DefaultPadding, dpi);
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelScripts"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new XRLabelScripts Scripts { get { return (XRLabelScripts)fEventScripts; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelKeepTogether"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.KeepTogether"),
		DefaultValue(false),
		]
		public override bool KeepTogether { get { return base.KeepTogether; } set { base.KeepTogether = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelMultiline"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.Multiline"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(false),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool Multiline {
			get { return multiline; }
			set { multiline = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelAutoWidth"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.AutoWidth"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(false),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool AutoWidth {
			get { return autoWidth; }
			set { autoWidth = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelLines"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.Lines"),
		SRCategory(ReportStringId.CatData),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.Utils.UI.StringArrayEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		]
		public string[] Lines {
			get { return XRConvert.StringToStringArray(Text); }
			set { Text = XRConvert.StringArrayToString(value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelSummary"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.Summary"),
		Browsable(true),
		SRCategory(ReportStringId.CatData),
		Editor("DevExpress.XtraReports.Design.XRSummaryUIEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public XRSummary Summary {
			get { return fSummary; }
			set { fSummary = value; if(fSummary != null) fSummary.Control = this; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelCanGrow"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(true),
		XtraSerializableProperty,
		]
		public override bool CanGrow {
			get { return base.CanGrow; }
			set { base.CanGrow = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelCanShrink"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(true),
		XtraSerializableProperty,
		]
		public override bool CanShrink {
			get { return base.CanShrink; }
			set { base.CanShrink = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelProcessNullValues"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.ProcessNullValues"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override ValueSuppressType ProcessNullValues {
			get { return base.ProcessNullValues; }
			set { base.ProcessNullValues = value; }
		}
		[
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		]
		public override string NullValueText {
			get { return base.NullValueText; }
			set { base.NullValueText = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelProcessDuplicatesMode"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.ProcessDuplicatesMode"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override ProcessDuplicatesMode ProcessDuplicatesMode {
			get { return base.ProcessDuplicatesMode; }
			set { base.ProcessDuplicatesMode = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelProcessDuplicatesTarget"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.ProcessDuplicatesTarget"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override ProcessDuplicatesTarget ProcessDuplicatesTarget {
			get { return base.ProcessDuplicatesTarget; }
			set { base.ProcessDuplicatesTarget = value; }
		}
		[
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override StringTrimming TextTrimming {
			get { return base.TextTrimming; }
			set { base.TextTrimming = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLabelAngle"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.Angle"),
		DefaultValue(0f),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public float Angle {
			get { return angle; }
			set { angle = value; }
		}
		private bool HasAngle {
			get { return angle % 360 != 0; }
		}
		float RealAngle { get { return -angle; } }
		protected override bool HasPageSummary { get { return true; } }
		internal bool HasSummary { get { return Summary != null && Summary.Running != SummaryRunning.None; } }
		#endregion
		public XRLabel()
			: base() {
			Initialize();
		}
		internal XRLabel(Rectangle bounds)
			: base(bounds) {
			Initialize();
		}
		void Initialize() {
			fSummary.Control = this;
			KeepTogether = false;
		}
		protected override XRControlScripts CreateScripts() {
			return new XRLabelScripts(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				fSummary.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Events
		private static readonly object SummaryCalculatedEvent = new object();
		private static readonly object SummaryGetResultEvent = new object();
		private static readonly object SummaryResetEvent = new object();
		private static readonly object SummaryRowChangedEvent = new object();
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRLabelSummaryCalculated")]
#endif
		public event TextFormatEventHandler SummaryCalculated {
			add { Events.AddHandler(SummaryCalculatedEvent, value); }
			remove { Events.RemoveHandler(SummaryCalculatedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRLabelSummaryGetResult")]
#endif
		public event SummaryGetResultHandler SummaryGetResult {
			add { Events.AddHandler(SummaryGetResultEvent, value); }
			remove { Events.RemoveHandler(SummaryGetResultEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRLabelSummaryReset")]
#endif
		public event EventHandler SummaryReset {
			add { Events.AddHandler(SummaryResetEvent, value); }
			remove { Events.RemoveHandler(SummaryResetEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRLabelSummaryRowChanged")]
#endif
		public event EventHandler SummaryRowChanged {
			add { Events.AddHandler(SummaryRowChangedEvent, value); }
			remove { Events.RemoveHandler(SummaryRowChangedEvent, value); }
		}
		protected virtual void OnSummaryCalculated(TextFormatEventArgs e) {
			RunEventScript(SummaryCalculatedEvent, EventNames.SummaryCalculated, e);
			TextFormatEventHandler handler = (TextFormatEventHandler)Events[SummaryCalculatedEvent];
			if(handler != null && !DesignMode) handler(this, e);
		}
		protected internal virtual void OnSummaryGetResult(SummaryGetResultEventArgs e) {
			RunEventScript(SummaryGetResultEvent, EventNames.SummaryGetResult, e);
			SummaryGetResultHandler handler = (SummaryGetResultHandler)Events[SummaryGetResultEvent];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void OnSummaryReset(EventArgs e) {
			RunEventScript(SummaryResetEvent, EventNames.SummaryReset, e);
			EventHandler handler = (EventHandler)Events[SummaryResetEvent];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void OnSummaryRowChanged(EventArgs e) {
			RunEventScript(SummaryRowChangedEvent, EventNames.SummaryRowChanged, e);
			EventHandler handler = (EventHandler)Events[SummaryRowChangedEvent];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseSummaryCalculated(VisualBrick brick, string text, string format, object value) {
			TextFormatEventArgs e = new TextFormatEventArgs(text, format, value);
			OnSummaryCalculated(e);
			brick.SetAttachedValue(BrickAttachedProperties.SummaryInProgress, false);
			SetBrickText(brick, e.Text, e.Value);
			UpdateBrickBounds(brick);
		}
		#endregion
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeBoolean("Multiline", multiline);
			serializer.SerializeSingle("Angle", angle);
			serializer.Serialize("Summary", fSummary);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			multiline = serializer.DeserializeBoolean("Multiline", false);
			angle = serializer.DeserializeSingle("Angle", 0f);
			serializer.Deserialize("Summary", fSummary);
			Scripts.OnSummaryReset = fSummary.XRSummaryScripts.OnReset;
			Scripts.OnSummaryRowChanged = fSummary.XRSummaryScripts.OnRowChanged;
			Scripts.OnSummaryGetResult = fSummary.XRSummaryScripts.OnGetResult;
		}
		#endregion
		protected internal override string GetDisplayPropertyBindingString() {
			XRBinding binding = DataBindings[DisplayPropertyName];
			if(fSummary.Running == SummaryRunning.None || (binding == null && fSummary.Func.NeedsCalculation()))
				return base.GetDisplayPropertyBindingString();
			return fSummary.GetDisplayText(binding != null ? binding.DisplayColumnName : "UndefinedColumn");
		}
		protected override void ValidateBrick(VisualBrick brick, RectangleF bounds, PointF offset) {
			if(DesignMode) {
				SetDesignerBrickText(brick);
				brick.Text = GetMultilineText(brick.Text);
			} else if(Summary.Running.Exists()) {
				VisualBrick secondaryBrick = GetSecondaryBrick();
				if(secondaryBrick != null) {
					brick.Text = secondaryBrick.Text;
					brick.TextValue = secondaryBrick.TextValue;
				} else {
					brick.SetAttachedValue(BrickAttachedProperties.SummaryInProgress, true);
					brick.Text = SummaryProgressSign;
				}
				XRSummary summary = RootReport.Summaries.GetActualSummary(this);
				if(summary != null && summary.Running != SummaryRunning.Page && !summary.NeedsPageCalculation())
					summary.AddSummaryBrickOnReport(brick);
			} else
				brick.Text = GetMultilineText(brick.Text);
		}
		VisualBrick GetSecondaryBrick() {
			return Band != null && Band.WriteInfo != null ?
				Band.WriteInfo.GetSecondaryBrick(this) :
				null;
		}
		protected internal override object GetTextValue() {
			object result = base.GetTextValue();
			return result is string ? GetMultilineText((string)result) : result;
		}
		string GetMultilineText(string text) {
			if(!Multiline) {
				text = text.Replace("\r", String.Empty);
				text = text.Replace("\n", String.Empty);
				text = text.Replace(System.Environment.NewLine, String.Empty);
			}
			else
				text = ReplaceVBCR(text);
			return text;
		}
		protected internal Size MeasureText(string text, float width, BrickStyle bs) {
			Size size = System.Drawing.Size.Empty;
			if(string.IsNullOrEmpty(text) || HasAngle)
				return size;
			size = AutoWidth ?
				MeasureTextCore(text, bs.StringFormat.WordWrap ? width : float.MaxValue, Dpi, bs, RootReport.Measurer) :
				MeasureTextCore(text, width, Dpi, bs, RootReport.Measurer);
			if(text.Length == 0 && CanAutoHeight)
				size.Height = 0;
			return size;
		}
		protected bool ShouldSerializeSummary() {
			return ShouldSerializeProperty(fSummary, "Func", fSummary.Func) ||
				ShouldSerializeProperty(fSummary, "FormatString", fSummary.FormatString) ||
				ShouldSerializeProperty(fSummary, "Running", fSummary.Running) ||
				ShouldSerializeProperty(fSummary, "IgnoreNullValues", fSummary.IgnoreNullValues);			
		}
		protected internal override string GetTextValueFormatString() {
			return fSummary.Running != SummaryRunning.None ? fSummary.FormatString : base.GetTextValueFormatString();
		}
		protected internal override void UpdateBrickBounds(VisualBrick brick) {
			if(AutoWidth && !HasAngle)
				brick.SetBoundsWidth(MeasureBrickSize(brick).Width, Dpi);
			base.UpdateBrickBounds(brick);
		}
		protected override float CalculateBrickHeight(VisualBrick brick) {
			return UpdateAutoHeight(brick, MeasureBrickSize(brick).Height);
		}
		protected override void WriteContentToCore(XRWriteInfo writeInfo, VisualBrick brick) {
			base.WriteContentToCore(writeInfo, brick);
			if(Summary.Running != SummaryRunning.None)
				brick.SetAttachedValue<int>(BrickAttachedProperties.RowIndex, writeInfo.DocBand.RowIndex);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new LabelBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			SetTextProperties(brick);
			if(brick is LabelBrick)
				((LabelBrick)brick).Angle = Angle;
		}
		protected override void SetShrinkGrow(VisualBrick brick) {
			base.SetShrinkGrow(brick);
			if(AutoWidth == true && CanShrink == false && CanGrow == false && WordWrap == false) {
				VisualBrickHelper.SetCanGrow(brick, true);
				VisualBrickHelper.SetCanShrink(brick, true);
			}
		}
		protected override object GetMergeValue() {
			if(Summary != null && Summary.Running == SummaryRunning.Report) {
				return new DevExpress.Utils.MultiKey(this);
			} else {
				return base.GetMergeValue();
			}
		}
		internal SizeF MeasureBrickSize(VisualBrick brick) {
			RectangleF clientBounds = brick.Style.DeflateBorderWidth(GetBrickBounds(brick), Dpi);
			float width = ((LabelBrick)brick).IsVerticalTextMode ? clientBounds.Height : clientBounds.Width;
			SizeF size = SizeF.Empty;
			try {
				size = MeasureText(brick.Text, width, brick.Style);
			} catch {
				brick.Text = Localization.ReportLocalizer.GetString(Localization.ReportStringId.Msg_LargeText);
				size = MeasureText(brick.Text, width, brick.Style);
			}
			if(size.Height <= 0)
				return new SizeF(size.Width, 0);
			RectangleF rect = brick.Style.InflateBorderWidth(new RectangleF(PointF.Empty, size), Dpi);
			return rect.Size;
		}
		protected override TextEditMode TextEditMode {
			get {
				TextEditMode textEditMode = base.TextEditMode;
				if(textEditMode != TextEditMode.None) {
					return TextEditMode.Multiline;
				}
				return TextEditMode.None;
			}
		}
		protected override void AddToSummaryUpdater(VisualBrick brick, VisualBrick prototypeBrick) {
			if(Summary != null && Summary.Running.Exists() && Summary.Bricks.Contains(prototypeBrick)) {
				Summary.AddSummaryBrickOnReport(brick);
			}
		}
	}
}
