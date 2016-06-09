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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	[
	Designer("DevExpress.XtraEditors.Design.BarCodeControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	Description("Allows you to display various barcodes."),
	DefaultProperty("Text"),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "BarCodeControl"),
	DXToolboxItem(DXToolboxItemKind.Regular),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon)
	]
	public class BarCodeControl : BaseControl {
		BaseControlPainter painter;
		BaseControlViewInfo viewInfo;
		BarCodeGeneratorBase generator = BarCodeGeneratorFactory.Create(BarCodeSymbology.Code128);
		bool autoModule = BarCodeBrick.DefaultAutoModule;
		HorzAlignment horzAlignment = DevExpress.Utils.HorzAlignment.Near;
		VertAlignment vertAlignment = DevExpress.Utils.VertAlignment.Top;
		HorzAlignment horzTextAlignment = DevExpress.Utils.HorzAlignment.Near;
		VertAlignment vertTextAlignment = DevExpress.Utils.VertAlignment.Bottom;
		double module = 2.0;
		BarCodeOrientation orientation = BarCodeOrientation.Normal;
		bool showText = BarCodeBrick.DefaultShowText;
		BrickStyle style;
		byte[] binaryData = new byte[] { };
		public BarCodeControl() {
			style = new DevExpress.XtraPrinting.BrickStyle() {
				ForeColor = this.ForeColor,
				BackColor = this.BackColor,
				Padding = ToPaddingInfo(Padding),
			};
			SetTextAlignment(vertTextAlignment, horzTextAlignment);
		}
		protected override Size DefaultSize {
			get {
				return new Size(100, 23);
			}
		}
		void OnBarcodeControlChanged() {
			Invalidate();
		}
		[
		DXCategory(CategoryName.Data),
		RefreshProperties(RefreshProperties.Repaint),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Bindable(true),
		TypeConverter("DevExpress.XtraEditors.Design.BarCodeDataConverter," + AssemblyInfo.SRAssemblyEditorsDesign),
		Editor("DevExpress.XtraEditors.Design.BarCodeDataEditor," + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)),
		]
		public byte[] BinaryData {
			get { return binaryData; }
			set {
				binaryData = value;
				UpdateGenerator(Symbology);
				OnBarcodeControlChanged();
			}
		}
		bool ShouldSerializeBinaryData() {
			return binaryData != null && binaryData.Length != 0;
		}
		void ResetBinaryData() {
			BinaryData = new byte[] { };
		}
		void UpdateGenerator(BarCodeGeneratorBase generator) {
			if(generator is BarCode2DGenerator) {
				BarCode2DGenerator barCode2DGenerator = Symbology as BarCode2DGenerator;
				barCode2DGenerator.Update(Text, BinaryData);
			}
		}
		[
		DXCategory(CategoryName.Behavior),
		DefaultValue(BarCodeSymbology.Code128),
		RefreshProperties(RefreshProperties.All), SmartTagProperty("Symbology", "", SmartTagActionType.RefreshBoundsAfterExecute)
		]
		public BarCodeGeneratorBase Symbology {
			get {
				return generator;
			}
			set {
				if(value != null) {
					generator = value;
					UpdateGenerator(generator);
					OnBarcodeControlChanged();
				}
			}
		}
		[DefaultValue(DevExpress.Utils.HorzAlignment.Near),
		DXCategory(CategoryName.Appearance)]
		public HorzAlignment HorizontalTextAlignment {
			get {
				return horzTextAlignment;
			}
			set {
				if(horzTextAlignment != value) {
					horzTextAlignment = value;
					SetTextAlignment(VerticalTextAlignment, value);
					OnBarcodeControlChanged();
				}
			}
		}
		[DefaultValue(DevExpress.Utils.VertAlignment.Bottom),
		DXCategory(CategoryName.Appearance)]
		public VertAlignment VerticalTextAlignment {
			get {
				return vertTextAlignment;
			}
			set {
				if(vertTextAlignment != value) {
					vertTextAlignment = value;
					SetTextAlignment(value, HorizontalAlignment);
					OnBarcodeControlChanged();
				}
			}
		}
		void SetTextAlignment(VertAlignment vertAlignment, HorzAlignment horzAlignment) {
			TextAlignment value = TextAlignmentConverter.ToTextAlignment(vertAlignment, horzAlignment);
			if(style.TextAlignment != value) {
				style.TextAlignment = value;
				style.StringFormat = BrickStringFormat.Create(value, true);
			}
		}
		[DefaultValue(DevExpress.Utils.HorzAlignment.Near),
		DXCategory(CategoryName.Appearance)]
		public HorzAlignment HorizontalAlignment {
			get {
				return horzAlignment;
			}
			set {
				if(horzAlignment != value) {
					horzAlignment = value;
					OnBarcodeControlChanged();
				}
			}
		}
		[DefaultValue(DevExpress.Utils.VertAlignment.Top),
		DXCategory(CategoryName.Appearance)]
		public VertAlignment VerticalAlignment {
			get {
				return vertAlignment;
			}
			set {
				if(vertAlignment != value) {
					vertAlignment = value;
					OnBarcodeControlChanged();
				}
			}
		}
		[DefaultValue(BarCodeBrick.DefaultAutoModule),
		DXCategory(CategoryName.Behavior), SmartTagProperty("Auto Module", "", 0, SmartTagActionType.RefreshBoundsAfterExecute)]
		public bool AutoModule {
			get {
				return autoModule;
			}
			set {
				if(autoModule != value) {
					autoModule = value;
					OnBarcodeControlChanged();
				}
			}
		}
		[DefaultValue(BarCodeOrientation.Normal),
		DXCategory(CategoryName.Behavior), SmartTagProperty("Orientation", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public BarCodeOrientation Orientation {
			get {
				return orientation;
			}
			set {
				if(orientation != value) {
					orientation = value;
					OnBarcodeControlChanged();
				}
			}
		}
		[
		DefaultValue(BarCodeBrick.DefaultShowText),
		DXCategory(CategoryName.Behavior), SmartTagProperty("ShowText", "", 1, SmartTagActionType.RefreshBoundsAfterExecute)]
		public bool ShowText {
			get {
				return showText;
			}
			set {
				if(showText != value) {
					showText = value;
					OnBarcodeControlChanged();
				}
			}
		}
		[DefaultValue(2.0),
		DXCategory(CategoryName.Behavior), SmartTagProperty("Module", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public double Module {
			get { return module; }
			set {
				if(module != value) {
					module = value;
					OnBarcodeControlChanged();
				}
			}
		}
		protected override void OnFontChanged(EventArgs e) {
			base.OnFontChanged(e);
			style.Font = Font;
		}
		protected override void OnBackColorChanged(EventArgs e) {
			base.OnBackColorChanged(e);
			style.BackColor = BackColor;
		}
		protected override void OnForeColorChanged(EventArgs e) {
			base.OnForeColorChanged(e);
			style.ForeColor = ForeColor;
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			style.Padding = ToPaddingInfo(Padding);
		}
		static PaddingInfo ToPaddingInfo(Padding padding) {
			return new PaddingInfo(padding.Left, padding.Right, padding.Top, padding.Bottom, GraphicsDpi.Pixel);
		}
		protected internal override BaseControlPainter Painter { 
			get {
				if(painter == null)
					painter = new BarCodeControlPainter();
				return painter;
			}
		}
		protected internal override BaseControlViewInfo ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = new BarCodeControlViewInfo(this, style);
				return viewInfo;
			}
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			OnBarcodeControlChanged();
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	static class TextAlignmentConverter {
		static Dictionary<MultiKey, TextAlignment> dict = new Dictionary<MultiKey, TextAlignment>();
		static TextAlignmentConverter() {
			dict.Add(CreateKey(VertAlignment.Top, HorzAlignment.Near), TextAlignment.TopLeft);
			dict.Add(CreateKey(VertAlignment.Top, HorzAlignment.Default), TextAlignment.TopLeft);
			dict.Add(CreateKey(VertAlignment.Top, HorzAlignment.Center), TextAlignment.TopCenter);
			dict.Add(CreateKey(VertAlignment.Top, HorzAlignment.Far), TextAlignment.TopRight);
			dict.Add(CreateKey(VertAlignment.Center, HorzAlignment.Near), TextAlignment.MiddleLeft);
			dict.Add(CreateKey(VertAlignment.Center, HorzAlignment.Default), TextAlignment.MiddleLeft);
			dict.Add(CreateKey(VertAlignment.Center, HorzAlignment.Center), TextAlignment.MiddleCenter);
			dict.Add(CreateKey(VertAlignment.Center, HorzAlignment.Far), TextAlignment.MiddleRight);
			dict.Add(CreateKey(VertAlignment.Bottom, HorzAlignment.Near), TextAlignment.BottomLeft);
			dict.Add(CreateKey(VertAlignment.Bottom, HorzAlignment.Default), TextAlignment.BottomLeft);
			dict.Add(CreateKey(VertAlignment.Bottom, HorzAlignment.Center), TextAlignment.BottomCenter);
			dict.Add(CreateKey(VertAlignment.Bottom, HorzAlignment.Far), TextAlignment.BottomRight);
			dict.Add(CreateKey(VertAlignment.Default, HorzAlignment.Near), TextAlignment.TopLeft);
			dict.Add(CreateKey(VertAlignment.Default, HorzAlignment.Default), TextAlignment.TopLeft);
			dict.Add(CreateKey(VertAlignment.Default, HorzAlignment.Center), TextAlignment.TopCenter);
			dict.Add(CreateKey(VertAlignment.Default, HorzAlignment.Far), TextAlignment.TopRight);
		}
		static MultiKey CreateKey(VertAlignment vertAligment, HorzAlignment horzAligment) {
			return new MultiKey(vertAligment, horzAligment);
		}
		public static TextAlignment ToTextAlignment(VertAlignment vertAligment, HorzAlignment horzAligment) {
			TextAlignment value;
			return dict.TryGetValue(CreateKey(vertAligment, horzAligment), out value) ? value : TextAlignment.TopLeft;
		}
	}
	class BarCodeControlViewInfo : BaseControlViewInfo, IBarCodeData {
		BrickStyle style;
		BarCodeControl Control { get { return (BarCodeControl)Owner; } }
		public BarCodeGeneratorBase Symbology { get { return Control.Symbology; } }
		public BarCodeControlViewInfo(BarCodeControl owner, BrickStyle style)
			: base(owner) {
			this.style = style;
		}
		#region IBarCodeData Members
		TextAlignment IBarCodeData.Alignment {
			get { return TextAlignmentConverter.ToTextAlignment(Control.VerticalAlignment, Control.HorizontalAlignment); }
		}
		bool IBarCodeData.AutoModule {
			get { return Control.AutoModule; }
		}
		double IBarCodeData.Module {
			get { return Control.Module; }
		}
		BarCodeOrientation IBarCodeData.Orientation {
			get { return Control.Orientation; }
		}
		bool IBarCodeData.ShowText {
			get { return Control.ShowText; }
		}
		BrickStyle IBarCodeData.Style {
			get { return style; }
		}
		string IBarCodeData.Text {
			get { return Control.Text; }
		}
		#endregion
	}
	class BarCodeControlPainter : BaseControlPainter {
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
			GdiGraphicsWrapper graphics = new GdiGraphicsWrapper(info.Graphics);
			BarCodeControlViewInfo viewInfo = (BarCodeControlViewInfo)info.ViewInfo;
			viewInfo.Symbology.DrawContent(graphics, viewInfo.ClientRect, viewInfo);
		}
	}
}
