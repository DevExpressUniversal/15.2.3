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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Serialization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraReports.Localization;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.NativeBricks;
namespace DevExpress.XtraReports.UI {
	#region XRRichTextBox
	[
	XRDesigner("DevExpress.XtraReports.Design.XRRichTextBoxDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRRichTextBoxDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRRichTextBox.bmp"),
	]
	public class XRRichTextBoxBase : XRRichTextBase, IRichTextBoxBrickOwner {
		#region static
		static void ForceCreateHandle(RichTextBox aRichTextBox) {
			IntPtr ignored = aRichTextBox.Handle;
		}
		#endregion
		#region RichTextBox wrappers
		public void LoadFile(string path) {
			richTextBox.LoadFile(path);
			UpdateInternalRtfText();
		}
		public void LoadFile(Stream data, RichTextBoxStreamType fileType) {
			richTextBox.LoadFile(data, fileType);
			UpdateInternalRtfText();
		}
		public void LoadFile(string path, RichTextBoxStreamType fileType) {
			richTextBox.LoadFile(path, fileType);
			UpdateInternalRtfText();
		}
		public void SaveFile(string path) {
			richTextBox.SaveFile(path);
		}
		public void SaveFile(Stream data, RichTextBoxStreamType fileType) {
			richTextBox.SaveFile(data, fileType);
		}
		public void SaveFile(string path, RichTextBoxStreamType fileType) {
			richTextBox.SaveFile(path, fileType);
		}
		public void Clear() {
			richTextBox.Clear();
		}
		#endregion
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeString(PropertyNames.Rtf, richTextBox.Rtf);
			serializer.SerializeBoolean(PropertyNames.Multiline, richTextBox.Multiline);
			serializer.SerializeBoolean(PropertyNames.DetectUrls, richTextBox.DetectUrls);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			richTextBox.Rtf = serializer.DeserializeString(PropertyNames.Rtf, "");
			richTextBox.Multiline = serializer.DeserializeBoolean(PropertyNames.Multiline, true);
			richTextBox.DetectUrls = serializer.DeserializeBoolean(PropertyNames.DetectUrls, true);
		}
		#endregion
		#region fields & properties
		RichTextBox richTextBox;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseKeepTogether"),
#endif
 DefaultValue(false)]
		public override bool KeepTogether { get { return base.KeepTogether; } set { base.KeepTogether = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseFont"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRRichTextBoxBase.Font"),
		XtraSerializableProperty,
		]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRRichTextBoxBase.BackColor"),
		XtraSerializableProperty,
		]
		public new Color BackColor {
			get { return base.BackColor; }
			set {
				richTextBox.BackColor = value;
				base.BackColor = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseMultiline"),
#endif
 DefaultValue(true),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRRichTextBoxBase.Multiline"),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool Multiline {
			get { return richTextBox.Multiline; }
			set { richTextBox.Multiline = value; }
		}
		public override string Rtf {
			get {
				ForceCreateHandle(richTextBox);
				return base.Rtf;
			}
			set {
				richTextBox.Rtf = value;
				base.Rtf = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseText"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Bindable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Text {
			get { return richTextBox.Text; }
			set {
				richTextBox.Text = value;
				UpdateInternalRtfText();
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseLines"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRRichTextBoxBase.Lines"),
		SRCategory(ReportStringId.CatData),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.Utils.UI.StringArrayEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		]
		public string[] Lines {
			get { return richTextBox.Lines; }
			set {
				richTextBox.Lines = value;
				UpdateInternalRtfText();
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseCanGrow"),
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
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseCanShrink"),
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
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBoxBaseDetectUrls"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRRichTextBoxBase.DetectUrls"),
		DefaultValue(true),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool DetectUrls {
			get { return richTextBox.DetectUrls; }
			set { richTextBox.DetectUrls = value; }
		}
		RichTextBox IRichTextBoxBrickOwner.RichTextBox { get { return richTextBox; } }
		#endregion
		public XRRichTextBoxBase()
			: base() {
			richTextBox = new RichTextBox();
			BackColor = Color.White;
		}
		private void UpdateInternalRtfText() {
			ForceCreateHandle(richTextBox);
			Rtf = richTextBox.Rtf;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(richTextBox != null) {
					richTextBox.Dispose();
					richTextBox = null;
				}
			}
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new RichTextBoxBrick((IRichTextBoxBrickOwner)this);
		}
		protected override void OnFontChanged() {
			richTextBox.Font = GetEffectiveFont();
			UpdateInternalRtfText();
		}
		internal protected override bool ShouldSerializeBackColor() {
			return BackColor != Color.White;
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			RichTextBoxBrick richTextBoxBrick = (RichTextBoxBrick)brick;
			if(ShowBindingString(Text)) {
				richTextBoxBrick.RtfText = GetDisplayPropertyBindingString();
			} else {
				richTextBoxBrick.RtfText = Rtf;
			}
			if (DXColor.IsTransparentOrEmpty(BackColor))
				brick.BackColor = Color.White;
			richTextBoxBrick.DetectUrls = DetectUrls;
		}
		protected internal override void GetStateFromBrick(VisualBrick brick) {
			base.GetStateFromBrick(brick);
			Rtf = ((RichTextBoxBrick)brick).RtfText;
		}
		protected override float CalculateBrickHeight(VisualBrick brick) {
			RectangleF clientBounds = XRConvert.Convert(brick.GetClientRectangle(GetBrickBounds(brick), Dpi),
				Dpi, GraphicsDpi.Pixel);
			int pixHeight = RichEditHelper.MeasureRtfInPixels(((RichTextBoxBrick)brick).RtfText, clientBounds,
				GetMinimumHeight(GraphicsDpi.Pixel));
			RectangleF rect = new RectangleF(0, 0, 1, pixHeight);
			rect = brick.Style.InflateBorderWidth(rect, GraphicsDpi.Pixel);
			return UpdateAutoHeight(brick, (int)XRConvert.Convert(rect.Height, GraphicsDpi.Pixel, Dpi));
		}
		protected internal override DevExpress.XtraReports.Native.MailMergeFieldInfosCalculator GetMailMergeFieldInfosCalculator() {
			return DevExpress.XtraReports.Native.RichTextMailMergeFieldInfosCalculator.Instance;
		}
		protected override TextEditMode TextEditMode {
			get {
				return TextEditMode.None;
			}
		}
	}
	#endregion // XRRichTextBox
	[
	Obsolete("The XRRichTextBox class is obsolete now. Use the XRRichText class instead."),
	]
	public class XRRichTextBox : XRRichTextBoxBase {
	}
}
