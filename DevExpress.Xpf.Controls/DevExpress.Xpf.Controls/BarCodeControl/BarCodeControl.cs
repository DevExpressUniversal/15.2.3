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

#if !WINRT && !WP && !WP
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Base;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Controls.Internal;
using System;
#else
using DevExpress.Core.Native;
using DevExpress.UI.Xaml.Controls.Native;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System;
#endif
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using System.Windows;
using DevExpress.Utils;
#if !WINRT && !WP
namespace DevExpress.Xpf.Controls {
#else
namespace DevExpress.UI.Xaml.Controls {
#endif
	public interface IFullBarCodeData: IBarCodeData {
		byte[] BinaryData { get; }
	}
#if !WINRT && !WP
	[Obsolete("Warning! The DevExpress.Xpf.Controls.BarCodeControl class is now obsolete. Please use the DevExpress.Xpf.Editors.BarCodeEdit class instead.")]
#else
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon)]
#endif
	public class BarCodeControl : Control, IFullBarCodeData {
		public static readonly DependencyProperty AutoModuleProperty;
		public static readonly DependencyProperty ModuleProperty;
		public static readonly DependencyProperty ShowTextProperty;
		public static readonly DependencyProperty SymbologyProperty;
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty VerticalTextAlignmentProperty;
		public static readonly DependencyProperty HorizontalTextAlignmentProperty;
		public static readonly DependencyProperty BinaryDataProperty;
#if WINRT || WP
		public static readonly new DependencyProperty HorizontalContentAlignmentProperty;
		public static readonly new DependencyProperty VerticalContentAlignmentProperty;
		public static readonly new DependencyProperty ForegroundProperty;
#endif
		static BarCodeControl() {
			var dProp = new DependencyPropertyRegistrator<BarCodeControl>();
			dProp.Register("AutoModule", ref AutoModuleProperty, false, InvalidateVisual);
			dProp.Register("Module", ref ModuleProperty, 2.0, InvalidateVisual);
			dProp.Register("ShowText", ref ShowTextProperty, true, InvalidateVisual);
			dProp.Register<SymbologyBase>("Symbology", ref SymbologyProperty, null, UpdateSymbology);
			dProp.Register("Text", ref TextProperty, string.Empty, InvalidateVisual);
			dProp.Register("VerticalTextAlignment", ref VerticalTextAlignmentProperty, VerticalAlignment.Bottom, InvalidateVisual);
			dProp.Register("HorizontalTextAlignment", ref HorizontalTextAlignmentProperty, HorizontalAlignment.Left, InvalidateVisual);
			dProp.Register("BinaryData", ref BinaryDataProperty, new byte[] { }, InvalidateVisual);
#if !WINRT && !WP
			dProp.OverrideFrameworkMetadata(HorizontalContentAlignmentProperty, HorizontalAlignment.Left, InvalidateStyle);
			dProp.OverrideFrameworkMetadata(VerticalContentAlignmentProperty, VerticalAlignment.Top, InvalidateStyle);
			dProp.OverrideFrameworkMetadata(ForegroundProperty, new SolidColorBrush(Colors.Black), InvalidateStyle);
#else
			dProp.Register("HorizontalContentAlignment", ref HorizontalContentAlignmentProperty, HorizontalAlignment.Left, InvalidateStyle);
			dProp.Register("VerticalContentAlignment", ref VerticalContentAlignmentProperty, VerticalAlignment.Top, InvalidateStyle);
			dProp.Register("Foreground", ref ForegroundProperty, new SolidColorBrush(Colors.Black), InvalidateStyle);
#endif
		}
		static void UpdateSymbology(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCodeControl)d).UpdateSymbology(e);
		}
		static void InvalidateVisual(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCodeControl)d).BarCodeInvalidateVisual();
		}
		static void InvalidateStyle(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCodeControl)d).BarCodeInvalidateStyle();
		}
		protected BarCodePainter barCodePainter;
		protected Grid gridRoot;
		protected BrickStyle brickStyle = new BrickStyle();
		public BarCodeControl() {
			DefaultStyleKey = typeof(BarCodeControl);
		}
		public bool AutoModule {
			get { return (bool)GetValue(AutoModuleProperty); }
			set { SetValue(AutoModuleProperty, value); }
		}
		public double Module {
			get { return (double)GetValue(ModuleProperty); }
			set { SetValue(ModuleProperty, value); }
		}
		public bool ShowText {
			get { return (bool)GetValue(ShowTextProperty); }
			set { SetValue(ShowTextProperty, value); }
		}
		public SymbologyBase Symbology {
			get { return (SymbologyBase)GetValue(SymbologyProperty); }
			set { SetValue(SymbologyProperty, value); }
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public VerticalAlignment VerticalTextAlignment {
			get { return (VerticalAlignment)GetValue(VerticalTextAlignmentProperty); }
			set { SetValue(VerticalTextAlignmentProperty, value); }
		}
		public HorizontalAlignment HorizontalTextAlignment {
			get { return (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty); }
			set { SetValue(HorizontalTextAlignmentProperty, value); }
		}
		public byte[] BinaryData {
			get { return (byte[])GetValue(BinaryDataProperty); }
			set { SetValue(BinaryDataProperty, value); }
		}
#if WINRT || WP
		public new HorizontalAlignment HorizontalContentAlignment {
			get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
			set { SetValue(HorizontalContentAlignmentProperty, value); }
		}
		public new VerticalAlignment VerticalContentAlignment {
			get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
			set { SetValue(VerticalContentAlignmentProperty, value); }
		}
		public new Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		protected
#else
		public 
#endif
		override void OnApplyTemplate() {
			base.OnApplyTemplate();
			barCodePainter = (BarCodePainter)GetTemplateChild("Part_BarCodePainter");
			gridRoot = (Grid)GetTemplateChild("Part_RootGrid");
			if(Symbology != null)
				gridRoot.Children.Add(Symbology);
			barCodePainter.Symbology = this.Symbology;
			barCodePainter.BarCodeData = this;
			BarCodeInvalidateStyle();
		}
		protected internal virtual void BarCodeInvalidateStyle() {
			var textAligment = AligmentHelper.GetFullAligment(VerticalTextAlignment, HorizontalTextAlignment);
			brickStyle.BackColor = WindowsFormsHelper.ConvertBrushToColor(this.Background);
			brickStyle.ForeColor = WindowsFormsHelper.ConvertBrushToColor(this.Foreground);
			brickStyle.StringFormat = BrickStringFormat.Create(textAligment, true);
			brickStyle.TextAlignment = textAligment;
			brickStyle.Padding = new PaddingInfo((int)Padding.Left, (int)Padding.Right, (int)Padding.Top, (int)Padding.Bottom, GraphicsDpi.Pixel);
			BarCodeInvalidateVisual();
		}
		protected internal virtual void BarCodeInvalidateVisual() {
			if(barCodePainter != null)
				barCodePainter.InvalidateBarCodePainter();
		}
		protected internal virtual void UpdateSymbology(DependencyPropertyChangedEventArgs e) {
			SymbologyBase symbologyBase = (SymbologyBase)e.NewValue;
			if(gridRoot != null) {
				if(e.OldValue is UIElement)
					gridRoot.Children.Remove(e.OldValue as UIElement);
				gridRoot.Children.Add(symbologyBase);
			}
			if(barCodePainter != null)
				barCodePainter.Symbology = symbologyBase;
			symbologyBase.BarCodeControl = this;
			BarCodeInvalidateVisual();
		}
		#region IBarCodeData
		XtraPrinting.TextAlignment IBarCodeData.Alignment { get { return AligmentHelper.GetFullAligment(VerticalContentAlignment, HorizontalContentAlignment); } }
		bool IBarCodeData.AutoModule { get { return AutoModule; } }
		double IBarCodeData.Module { get { return Module; } }
		BarCodeOrientation IBarCodeData.Orientation { get { return BarCodeOrientation.Normal; } }
		bool IBarCodeData.ShowText { get { return ShowText; } }
		BrickStyle IBarCodeData.Style { get { return brickStyle; } }
		string IBarCodeData.Text { get { return Text != null ? Text : string.Empty; } }
		byte[] IFullBarCodeData.BinaryData { get { return BinaryData; } }
		#endregion
	}
}
