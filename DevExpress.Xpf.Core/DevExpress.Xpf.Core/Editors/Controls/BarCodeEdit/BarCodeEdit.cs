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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
namespace DevExpress.Xpf.Editors {
	public interface IFullBarCodeData: IBarCodeData {
		byte[] BinaryData { get; }
	}
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfCommon)]
	public class BarCodeEdit : BaseEdit, IFullBarCodeData {
		public static readonly DependencyProperty AutoModuleProperty;
		public static readonly DependencyProperty ModuleProperty;
		public static readonly DependencyProperty ShowTextProperty;
		public static readonly DependencyProperty VerticalTextAlignmentProperty;
		public static readonly DependencyProperty HorizontalTextAlignmentProperty;
		public static readonly DependencyProperty BinaryDataProperty;
		static BarCodeEdit() {
			Type ownerType = typeof(BarCodeEdit);
			AutoModuleProperty = DependencyPropertyManager.Register("AutoModule", typeof(bool), ownerType, new PropertyMetadata(true, InvalidateVisual));
			ModuleProperty = DependencyPropertyManager.Register("Module", typeof(double), ownerType, new PropertyMetadata(2.0, InvalidateVisual));
			ShowTextProperty = DependencyPropertyManager.Register("ShowText", typeof(bool), ownerType, new PropertyMetadata(true, InvalidateVisual));
			VerticalTextAlignmentProperty = DependencyPropertyManager.Register("VerticalTextAlignment", typeof(VerticalAlignment), ownerType, new PropertyMetadata(VerticalAlignment.Bottom, InvalidateVisual));
			HorizontalTextAlignmentProperty = DependencyPropertyManager.Register("HorizontalTextAlignment", typeof(HorizontalAlignment), ownerType, new PropertyMetadata(HorizontalAlignment.Left, InvalidateVisual));
			BinaryDataProperty = DependencyPropertyManager.Register("BinaryData", typeof(byte[]), ownerType, new PropertyMetadata(new byte[] { }, InvalidateVisual));
			HorizontalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(HorizontalAlignment.Left, InvalidateStyle));
			VerticalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(VerticalAlignment.Top, InvalidateStyle));
			ForegroundProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), InvalidateStyle));
			StyleSettingsProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, UpdateSymbology));
			EditValueProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, InvalidateVisual));
		}
		static void UpdateSymbology(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCodeEdit)d).UpdateSymbology(e);
		}
		static void InvalidateVisual(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCodeEdit)d).BarCodeInvalidateVisual();
		}
		static void InvalidateStyle(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCodeEdit)d).BarCodeInvalidateStyle();
		}
		protected internal BarCodePainter BarCodePainter { get; set; }
		protected Grid gridRoot;
		protected BrickStyle brickStyle = new BrickStyle();
		public BarCodeEdit() {
			this.SetDefaultStyleKey(typeof(BarCodeEdit));
		}
		public new BarCodeStyleSettings StyleSettings {
			get { return (BarCodeStyleSettings)base.StyleSettings; }
			set { base.StyleSettings = value; }
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new BarCodeEditStrategy(this);
		}
		protected internal override Type StyleSettingsType {
			get { return typeof(BarCodeStyleSettings); }
		}
		protected internal override BaseEditStyleSettings CreateStyleSettings() {
			return new QRCodeStyleSettings();
		}
		new BarCodePropertyProvider PropertyProvider { get { return base.PropertyProvider as BarCodePropertyProvider; } }
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new BarCodePropertyProvider(this);
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
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			BarCodePainter = (BarCodePainter)GetTemplateChild("Part_BarCodePainter");
			gridRoot = (Grid)GetTemplateChild("Part_RootGrid");
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
			if(BarCodePainter != null)
				BarCodePainter.InvalidateBarCodePainter();
		}
		protected internal virtual void UpdateSymbology(DependencyPropertyChangedEventArgs e) {
			BarCodeStyleSettings symbologyBase = (BarCodeStyleSettings)e.NewValue;
			if(gridRoot != null) {
				if(e.OldValue is UIElement)
					gridRoot.Children.Remove(e.OldValue as UIElement);
				if(symbologyBase != null)
					gridRoot.Children.Add(symbologyBase);
			}
			if(symbologyBase == null)
				return;
			if(BarCodePainter != null)
				BarCodePainter.Symbology = symbologyBase;
			symbologyBase.BarCodeEdit = this;
			BarCodeInvalidateVisual();
		}
		#region IBarCodeData
		XtraPrinting.TextAlignment IBarCodeData.Alignment { get { return AligmentHelper.GetFullAligment(VerticalContentAlignment, HorizontalContentAlignment); } }
		bool IBarCodeData.AutoModule { get { return AutoModule; } }
		double IBarCodeData.Module { get { return Module; } }
		BarCodeOrientation IBarCodeData.Orientation { get { return BarCodeOrientation.Normal; } }
		bool IBarCodeData.ShowText { get { return ShowText; } }
		BrickStyle IBarCodeData.Style { get { return brickStyle; } }
		string IBarCodeData.Text { get { return EditValue as string ?? string.Empty; } }
		byte[] IFullBarCodeData.BinaryData { get { return BinaryData; } }
		#endregion
	}
}
