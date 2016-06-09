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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class Matrix8x14Presentation : SymbolPresentation {
	}
	public class DefaultMatrix8x14Presentation : Matrix8x14Presentation, IDefaultSymbolPresentation {
		public static readonly DependencyProperty FillActiveProperty = DependencyPropertyManager.Register("FillActive",
			typeof(Brush), typeof(DefaultMatrix8x14Presentation), new PropertyMetadata(null, FillActivePropertyChanged));
		public static readonly DependencyProperty FillInactiveProperty = DependencyPropertyManager.Register("FillInactive",
			typeof(Brush), typeof(DefaultMatrix8x14Presentation), new PropertyMetadata(null, FillInactivePropertyChanged));
		[Category(Categories.Presentation)]
		public Brush FillActive {
			get { return (Brush)GetValue(FillActiveProperty); }
			set { SetValue(FillActiveProperty, value); }
		}
		[Category(Categories.Presentation)]
		public Brush FillInactive {
			get { return (Brush)GetValue(FillInactiveProperty); }
			set { SetValue(FillInactiveProperty, value); }
		}
		static void FillActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DefaultMatrix8x14Presentation presentation = d as DefaultMatrix8x14Presentation;
			if (presentation != null)
				presentation.ActualFillActiveChanged();
		}
		static void FillInactivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DefaultMatrix8x14Presentation presentation = d as DefaultMatrix8x14Presentation;
			if (presentation != null)
				presentation.ActualFillInactiveChanged();
		}
		Brush DefaultFillActive { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x55, 0x55, 0x55)); } }
		Brush DefaultFillInactive { get { return new SolidColorBrush(Color.FromArgb(0x0F, 0x55, 0x55, 0x55)); } }
		public override string PresentationName { get { return "Default"; } }
		[Category(Categories.Presentation)]
		public Brush ActualFillActive { get { return FillActive != null ? FillActive : DefaultFillActive; } }
		[Category(Categories.Presentation)]
		public Brush ActualFillInactive { get { return FillInactive != null ? FillInactive : DefaultFillInactive; } }
		void ActualFillActiveChanged() {
			NotifyPropertyChanged("ActualFillActive");
		}
		void ActualFillInactiveChanged() {
			NotifyPropertyChanged("ActualFillInactive");
		}
		protected internal override PresentationControl CreateSymbolPresentationControl() {
			return new DefaultMatrix8x14Control();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultMatrix8x14Presentation();
		}
	}
	public class CustomMatrix8x14Presentation : Matrix8x14Presentation {
		public static readonly DependencyProperty ActiveSegmentTemplateProperty = DependencyPropertyManager.Register("ActiveSegmentTemplate",
		typeof(DataTemplate), typeof(CustomMatrix8x14Presentation));
		public static readonly DependencyProperty InactiveSegmentTemplateProperty = DependencyPropertyManager.Register("InactiveSegmentTemplate",
		typeof(DataTemplate), typeof(CustomMatrix8x14Presentation));
		[Category(Categories.Common)]
		public DataTemplate ActiveSegmentTemplate {
			get { return (DataTemplate)GetValue(ActiveSegmentTemplateProperty); }
			set { SetValue(ActiveSegmentTemplateProperty, value); }
		}
		[Category(Categories.Common)]
		public DataTemplate InactiveSegmentTemplate {
			get { return (DataTemplate)GetValue(InactiveSegmentTemplateProperty); }
			set { SetValue(InactiveSegmentTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom"; } }
		protected internal override PresentationControl CreateSymbolPresentationControl() {
			CustomMatrix8x14Control modelControl = new CustomMatrix8x14Control();
			modelControl.SetBinding(CustomMatrix8x14Control.ActiveSegmentTemplateProperty, new Binding("ActiveSegmentTemplate") { Source = this });
			modelControl.SetBinding(CustomMatrix8x14Control.InactiveSegmentTemplateProperty, new Binding("InactiveSegmentTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomMatrix8x14Presentation();
		}
	}
}
