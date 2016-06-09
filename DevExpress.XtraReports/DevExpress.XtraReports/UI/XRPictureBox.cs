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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	XRDesigner("DevExpress.XtraReports.Design.XRPictureBoxDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRPictureBoxDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultBindableProperty("Image"),
	DefaultProperty("Image"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRPictureBox.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRPictureBox", "PictureBox"),
	XRToolboxSubcategoryAttribute(0, 3),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRPictureBox.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRPictureBox.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRPictureBox : XRControl {
		#region static
		static SizeF MeasureImage(Image image, BorderSide borders, float borderWidth, float dpi, float scaleFactor) {
			if(image == null)
				return SizeF.Empty;
			float doubleBorderWidth = borders != BorderSide.None ? (borderWidth * 2) : 0;
			SizeF imageSize = PSNativeMethods.GetResolutionImageSize(image);
			imageSize = MathMethods.Scale(imageSize, scaleFactor);
			imageSize = imageSize + new SizeF(doubleBorderWidth, doubleBorderWidth);
			return XRConvert.Convert(imageSize, GraphicsDpi.DeviceIndependentPixel, dpi);
		}
		const ImageAlignment defaultImageAlignment = ImageAlignment.Default;
		#endregion
		#region fields & properties
		Image image;
		string imageUrl = string.Empty;
		string sourceDirectory = string.Empty;
		ImageSizeMode sizing = ImageSizeMode.Normal;
		ImageAlignment imageAlignment = defaultImageAlignment;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxFont"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxForeColor"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRPictureBoxPadding")]
#endif
		public override PaddingInfo Padding {
			get {
				return base.Padding;
			}
			set {
				base.Padding = value;
				UpdateSize();
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxText"),
#endif
		Bindable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
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
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxTextAlignment"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxWordWrap"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[
		Bindable(true),
		RefreshProperties(RefreshProperties.Repaint),
		SRCategory(ReportStringId.CatData),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxImage"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPictureBox.Image"),
		Editor("DevExpress.XtraReports.Design.ImageEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageTypeConverter)),
		DevExpress.Utils.Serializing.XtraSerializableProperty
		]
		public Image Image {
			get { 
				if(image == null && !string.IsNullOrEmpty(imageUrl))
					SetImage(GetImage(imageUrl));
				return image;
			}
			set {
				SetImage(value);
				imageUrl = String.Empty;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxImageUrl"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPictureBox.ImageUrl"),
		SRCategory(ReportStringId.CatData),
		Bindable(true),
		RefreshProperties(RefreshProperties.Repaint),
		Editor("DevExpress.XtraReports.Design.ImageFileNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DefaultValue(""),
		DevExpress.Utils.Serializing.XtraSerializableProperty,
		]
		public string ImageUrl {
			get { return imageUrl; }
			set {
				if(value == null)
					value = String.Empty;
				if(value != imageUrl) {
					imageUrl = value;
					image = null;
				}
			}
		}
		internal string SourceDirectory {
			get {
				return !string.IsNullOrEmpty(sourceDirectory) ? sourceDirectory :
					".\\";
			}
			set { sourceDirectory = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxSizing"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPictureBox.Sizing"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(ImageSizeMode.Normal),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.ImageSizingTypeConverter)),
		DevExpress.Utils.Serializing.XtraSerializableProperty,
		]
		public ImageSizeMode Sizing {
			get { return sizing; }
			set {
				if(value == ImageSizeMode.CenterImage) {
					sizing = ImageSizeMode.Normal;
					imageAlignment = ImageAlignment.MiddleCenter;
				} else {
					sizing = value; UpdateSize();
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxImageAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPictureBox.ImageAlignment"),
		TypeConverter(typeof(DevExpress.XtraReports.Design.ImageAlignmentTypeConverter)),
		Editor("DevExpress.XtraReports.Design.ImageAlignmentEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(defaultImageAlignment),
		DevExpress.Utils.Serializing.XtraSerializableProperty,
		]
		public ImageAlignment ImageAlignment {
			get { return imageAlignment; }
			set { imageAlignment = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxProcessDuplicatesMode"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPictureBox.ProcessDuplicatesMode"),
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
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxProcessDuplicatesTarget"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPictureBox.ProcessDuplicatesTarget"),
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
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPictureBoxProcessNullValues"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPictureBox.ProcessNullValues"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override ValueSuppressType ProcessNullValues {
			get { return base.ProcessNullValues; }
			set { base.ProcessNullValues = value; }
		}
		#endregion
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		#endregion
		public XRPictureBox()
			: base() {
		}
		protected override XRControlScripts CreateScripts() {
			return new XRPictureboxScripts(this);
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeValue("Image", image);
			serializer.SerializeEnum("Sizing", sizing);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			image = (Image)serializer.DeserializeValue("Image", typeof(Image), null);
			sizing = (ImageSizeMode)serializer.DeserializeEnum("Sizing", typeof(ImageSizeMode), ImageSizeMode.Normal);
		}
		#endregion
		protected internal override void SyncDpi(float dpi) {
			base.SyncDpi(dpi);
			UpdateSize();
		}
		protected override void SetBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			if(sizing == ImageSizeMode.AutoSize) {
				width = WidthF;
				height = HeightF;
			}
			base.SetBounds(x, y, width, height, specified);
		}
		protected override void OnBorderWidthChanged() {
			base.OnBorderWidthChanged();
			UpdateSize();
		}
		protected override void OnBordersChanged() {
			base.OnBordersChanged();
			UpdateSize();
		}
		void UpdateSize() {
			if(sizing == ImageSizeMode.AutoSize)
				UpdateSizeCore(Image);
		}
		void UpdateSizeCore(Image image) {
			if(image != null) {
				SizeF size = MeasureImage(image, Borders, BorderWidth, Dpi, 1.0f);
				size = Padding.Inflate(size, Dpi);
				if(SizeF != size) {
					SetBoundsCore(0, 0, size.Width, size.Height, BoundsSpecified.Size);
					UpdateBandHeight();
				}
			}
		}
		void SetImage(Image image) {
			if(image != this.image) {
				this.image = image;
				if(DesignMode)
					UpdateSize();
			}
		}
		Image GetImage(string url) {
			return ImageResolver.GetImage(url, SourceDirectory, UrlResolver);
		}
		bool ShouldSerializeImage() {
			return String.IsNullOrEmpty(imageUrl) && image != null;
		}
		void ResetImage() {
			image = null;
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new ImageBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			ImageBrick imageBrick = (ImageBrick)brick;
			if(!string.IsNullOrEmpty(imageUrl)) {
				Image image = ps.Images.GetImageByKey(imageUrl);
				if(image == null) {
					image = GetImage(imageUrl);
					if(image != null)
						ps.Images.Add(imageUrl, image);
				}
				imageBrick.Image = image;
			} else 
				imageBrick.Image = Image;
			imageBrick.HtmlImageUrl = UrlResolver.ResolveUrl(ImageUrl);
			imageBrick.SizeMode = Sizing;
			imageBrick.ImageAlignment = ImageAlignment;
			imageBrick.UseImageResolution = true;
		}
		protected internal override void GetStateFromBrick(VisualBrick brick) {
			base.GetStateFromBrick(brick);
			ImageBrick imageBrick = (ImageBrick)brick;
			Image = imageBrick.Image;
			ImageUrl = imageBrick.HtmlImageUrl;
		}
		protected internal override void UpdateBrickBounds(VisualBrick brick) {
			Image brickImage = ((ImageBrick)brick).Image;
			if(brickImage != null && Sizing == ImageSizeMode.AutoSize) {
				SizeF size = MeasureImage(brickImage, brick.Sides, brick.BorderWidth, Dpi, brick.PrintingSystem.Document.ScaleFactor);
				size = brick.Padding.Inflate(size, Dpi);
				if(size.Height != 0 || size.Width != 0)
					brick.SetBounds(new RectangleF(GetBrickBounds(brick).Location, size), Dpi);
			}
			if(IsEmptyValue(brickImage) && brick.CanShrink && 
				(ProcessDuplicatesMode == ProcessDuplicatesMode.SuppressAndShrink || ProcessNullValues == ValueSuppressType.SuppressAndShrink)) {
				brick.SetBoundsHeight(0, Dpi);
			}
		}
		protected override ControlLayoutRules LayoutRules {
			get {
				if(Sizing == ImageSizeMode.AutoSize)
					return ControlLayoutRules.Moveable;
				else
					return base.LayoutRules;
			}
		}
		protected override bool IsImageEditable {
			get {
				return DataBindings[XRComponentPropertyNames.Image] == null
					&& DataBindings[XRComponentPropertyNames.ImageUrl] == null;
			}
		}
		protected override bool? HasImage {
			get {
				return Image != null ||
					!string.IsNullOrEmpty(ImageUrl) ||
					DataBindings[XRComponentPropertyNames.Image] != null ||
					DataBindings[XRComponentPropertyNames.ImageUrl] != null;
			}
		}
		protected override object GetValueForSuppress() {
			return Image;
		}
		protected override object GetValueForMergeKey() {
			return DevExpress.Utils.Zip.Adler32.CalculateChecksum(PSConvert.ImageToArray(GetValueForSuppress() as Image));
		}
		protected override bool NeedSuppressDuplicatesByValue() {
			return IsEmptyValue(previousValue) || IsEmptyValue(GetValueForSuppress()) ? false :			   
				ImageComparer.Equals(previousValue as Image, GetValueForSuppress() as Image);
		}
		protected override bool NeedSuppressNullValue() {
			return IsEmptyValue(Image);
		}
		protected override bool IsEmptyValue(object value) {
			return value == null || value == DBNull.Value;
		}
#if DEBUGTEST
		public Image Test_GetImage(string url) {
			return GetImage(url);
		}
#endif
	}
}
