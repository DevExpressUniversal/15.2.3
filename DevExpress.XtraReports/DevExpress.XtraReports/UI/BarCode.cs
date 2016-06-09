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
using DevExpress.XtraReports.Serialization;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using System.Drawing.Design;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.XtraReports.UI
{
	[
	XRDesigner("DevExpress.XtraReports.Design.XRBarCodeDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRBarCodeDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultBindableProperty("Text"),
	DefaultProperty("Text"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRBarCode.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRBarCode", "BarCode"),
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	XRToolboxSubcategoryAttribute(1, 2),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRBarCode.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRBarCode.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRBarCode : XRFieldEmbeddableControl, IXtraSupportCreateContentPropertyValue {
		#region static
		internal static BarCodeGeneratorBase CloneGenerator(BarCodeGeneratorBase generator) {
			return (BarCodeGeneratorBase)((ICloneable)generator).Clone();
		}
		#endregion
		#region Fields & Properties
		float module = BarCodeBrick.DefaultModule;
		bool autoModule = BarCodeBrick.DefaultAutoModule;
		bool showText = BarCodeBrick.DefaultShowText;
		byte[] binaryData =  new byte[] { };
		TextAlignment alignment = BarCodeBrick.DefaultAlignment;
		const BarCodeSymbology DefaultBarCodeSymbology = BarCodeSymbology.Code128;
		BarCodeGeneratorBase generator = BarCodeGeneratorFactory.Create(DefaultBarCodeSymbology);
		DevExpress.XtraPrinting.BarCode.BarCodeOrientation orientation = BarCodeBrick.DefaultOrientation;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeDefaultPadding")
#else
	Description("")
#endif
		]
		public static PaddingInfo DefaultPadding {
			get { return new PaddingInfo(10, 10, 0, 0, GraphicsDpi.HundredthsOfAnInch); }
		}
		protected override PaddingInfo GetDefaultPadding(float dpi) {
			return new PaddingInfo(DefaultPadding, dpi);
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeBinaryData"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.BinaryData"),
		SRCategory(ReportStringId.CatData),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Bindable(true),
		TypeConverter(typeof(DevExpress.XtraReports.Design.BarCodeDataConverter)),
		Editor("DevExpress.XtraReports.Design.BarCodeDataEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		]
		public byte[] BinaryData {
			get { return binaryData; }
			set {
				binaryData = value;
				UpdateGenerator();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DefaultValue(BarCodeBrick.DefaultBinaryDataBase64),
		XtraSerializableProperty,
		]
		public string BinaryDataBase64 {
			get { return Convert.ToBase64String(BinaryData); }
			set { BinaryData = Convert.FromBase64String(value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeSymbology"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.Symbology"),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true, false, false, -1),
		]
		public BarCodeGeneratorBase Symbology {
			get { return generator; }
			set {
				if(value != null)
					generator = !IsDeserializing ? XRBarCode.CloneGenerator(value) : value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeModule"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.Module"),
		DefaultValue(BarCodeBrick.DefaultModule),
		TypeConverter(typeof(DevExpress.XtraReports.Design.BarCodeModuleConverter)),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public float Module {
			get { return module; }
			set { if(value > 0.0f) module = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeAutoModule"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.AutoModule"),
		DefaultValue(BarCodeBrick.DefaultAutoModule),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool AutoModule {
			get { return autoModule; }
			set { autoModule = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodePaddingInfo"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.PaddingInfo"),
		SRCategory(ReportStringId.CatAppearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Please use Padding property instead")
		]
		public PaddingInfo PaddingInfo {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.Alignment"),
		DefaultValue(BarCodeBrick.DefaultAlignment),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.TextAlignmentEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty,
		]
		public TextAlignment Alignment {
			get { return alignment; }
			set { alignment = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeBarCodeOrientation"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.BarCodeOrientation"),
		DefaultValue(BarCodeBrick.DefaultOrientation),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public DevExpress.XtraPrinting.BarCode.BarCodeOrientation BarCodeOrientation {
			get { return orientation; }
			set { orientation = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeTextAlignment"),
#endif
 DefaultValue(TextAlignment.BottomLeft)]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeShowText"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.ShowText"),
		DefaultValue(BarCodeBrick.DefaultShowText),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool ShowText {
			get { return showText; }
			set { showText = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap { get { return base.WordWrap; } set { base.WordWrap = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBarCodeProcessDuplicatesMode"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.ProcessDuplicatesMode"),
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
	DevExpressXtraReportsLocalizedDescription("XRBarCodeProcessDuplicatesTarget"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.ProcessDuplicatesTarget"),
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
	DevExpressXtraReportsLocalizedDescription("XRBarCodeProcessNullValues"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBarCode.ProcessNullValues"),
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
		public XRBarCode() : base() {
			base.TextAlignment = TextAlignment.BottomLeft;
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			module = serializer.DeserializeSingle("Module", BarCodeBrick.DefaultModule);
			alignment = (TextAlignment)serializer.DeserializeEnum("Alignment", typeof(TextAlignment), BarCodeBrick.DefaultAlignment);
			showText = serializer.DeserializeBoolean("ShowText", BarCodeBrick.DefaultShowText);
#pragma warning disable 0618
			orientation = (DevExpress.XtraPrinting.BarCode.BarCodeOrientation)serializer.DeserializeEnum("Orientation", typeof(XRBarCodeOrientation), (XRBarCodeOrientation)BarCodeBrick.DefaultOrientation);
#pragma warning restore 0618
			Type t = serializer.DeserializeType("CodeGeneratorType", null);
			if (t == null)
				return;
			generator = (BarCodeGeneratorBase)Activator.CreateInstance(t);
			if(generator is IXRSerializable)
				((IXRSerializable)generator).DeserializeProperties(serializer);
		}
		#endregion
		protected internal override void SyncDpi(float dpi) {
			if (dpi != Dpi) {
				module = XRConvert.Convert(module, Dpi, dpi);
			}
			base.SyncDpi(dpi);
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRBarCodeText")]
#endif
		public override string Text {
			get { return base.Text; }
			set {
				base.Text = value;
				UpdateGenerator();
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new BarCodeBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			SetTextProperties(brick);
			BarCodeBrick barCodeBrick = (BarCodeBrick)brick;
			barCodeBrick.BinaryData = BinaryData;
			barCodeBrick.Generator = XRBarCode.CloneGenerator(Symbology);
			barCodeBrick.ShowText = ShowText;
			barCodeBrick.Alignment = Alignment;
			barCodeBrick.Orientation = BarCodeOrientation;
			barCodeBrick.Module = XRConvert.Convert((float)Module, Dpi, GraphicsDpi.Document);
			barCodeBrick.AutoModule = AutoModule;
		}
		protected internal override void GetStateFromBrick(VisualBrick brick) {
			base.GetStateFromBrick(brick);
			BarCodeBrick barCodeBrick = (BarCodeBrick)brick;
			Module = XRConvert.Convert((float)barCodeBrick.Module, GraphicsDpi.Document, Dpi);
			ShowText = barCodeBrick.ShowText;
			Alignment = barCodeBrick.Alignment;
			BarCodeOrientation = barCodeBrick.Orientation;
			Symbology = barCodeBrick.Generator;
		}
		protected internal override void UpdateBrickBounds(VisualBrick brick) {
			if(IsEmptyValue(brick.Text) && brick.CanShrink &&
				(ProcessDuplicatesMode == ProcessDuplicatesMode.SuppressAndShrink || ProcessNullValues == ValueSuppressType.SuppressAndShrink)) {
				brick.SetBoundsHeight(0, Dpi);
			} 
			base.UpdateBrickBounds(brick);
		}
		void UpdateGenerator() {
			if(Symbology is BarCode2DGenerator) {
				BarCode2DGenerator barCode2DGenerator = Symbology as BarCode2DGenerator;
				barCode2DGenerator.Update(Text, BinaryData);
			}
		}
		bool ShouldSerializeBinaryData() {
			return binaryData != null && binaryData.Length != 0;
		}
		#region IXtraSupportCreateContentPropertyValue Members
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			if(e.Item.Name == "Symbology")
				return Activator.CreateInstance(typeof(BarCodeGeneratorBase).Assembly.GetType("DevExpress.XtraPrinting.BarCode." + (string)e.Item.ChildProperties["Name"].Value + "Generator"));
			return null;
		}
		#endregion
	}
}
