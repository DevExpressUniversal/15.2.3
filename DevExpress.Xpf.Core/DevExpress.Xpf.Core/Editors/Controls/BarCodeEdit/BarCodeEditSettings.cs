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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System;
using System.Windows;
namespace DevExpress.Xpf.Editors.Settings {
	public class BarCodeEditSettings : BaseEditSettings {
		public static readonly DependencyProperty AutoModuleProperty;
		public static readonly DependencyProperty ModuleProperty;
		public static readonly DependencyProperty ShowTextProperty;
		public static readonly DependencyProperty VerticalTextAlignmentProperty;
		public static readonly DependencyProperty HorizontalTextAlignmentProperty;
		public static readonly DependencyProperty BinaryDataProperty;
		static BarCodeEditSettings() {
			Type ownerType = typeof(BarCodeEditSettings);
			AutoModuleProperty = DependencyPropertyManager.Register("AutoModule", typeof(bool), ownerType, new PropertyMetadata(true, InvalidateVisual));
			ModuleProperty = DependencyPropertyManager.Register("Module", typeof(double), ownerType, new PropertyMetadata(2.0, InvalidateVisual));
			ShowTextProperty = DependencyPropertyManager.Register("ShowText", typeof(bool), ownerType, new PropertyMetadata(true, InvalidateVisual));
			VerticalTextAlignmentProperty = DependencyPropertyManager.Register("VerticalTextAlignment", typeof(VerticalAlignment), ownerType, new PropertyMetadata(VerticalAlignment.Bottom, InvalidateVisual));
			HorizontalTextAlignmentProperty = DependencyPropertyManager.Register("HorizontalTextAlignment", typeof(HorizontalAlignment), ownerType, new PropertyMetadata(HorizontalAlignment.Left, InvalidateVisual));
			BinaryDataProperty = DependencyPropertyManager.Register("BinaryData", typeof(byte[]), ownerType, new PropertyMetadata(new byte[] { }, UpdateBarCodeGenerator));
			HorizontalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(EditSettingsHorizontalAlignment.Left, InvalidateStyle));
			VerticalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(VerticalAlignment.Top, InvalidateStyle));
			StyleSettingsProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, UpdateSymbology));
		}
		static void UpdateSymbology(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		static void InvalidateStyle(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		static void UpdateBarCodeGenerator(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		static void InvalidateVisual(DependencyObject d, DependencyPropertyChangedEventArgs e) {
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
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			BarCodeEdit editor = edit as BarCodeEdit;
			if(editor == null)
				return;
			SetValueFromSettings(AutoModuleProperty, () => editor.AutoModule = AutoModule);
			SetValueFromSettings(ModuleProperty, () => editor.Module = Module);
			SetValueFromSettings(ShowTextProperty, () => editor.ShowText = ShowText);
			SetValueFromSettings(VerticalTextAlignmentProperty, () => editor.VerticalTextAlignment = VerticalTextAlignment);
			SetValueFromSettings(HorizontalTextAlignmentProperty, () => editor.HorizontalTextAlignment = HorizontalTextAlignment);
			SetValueFromSettings(BinaryDataProperty, () => editor.BinaryData = BinaryData);
		}
	}
}
