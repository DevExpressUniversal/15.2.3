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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	public abstract class ModelControl : ChartElementBase {
		public static readonly DependencyProperty FlippedProperty = DependencyPropertyManager.Register("Flipped",
			typeof(bool), typeof(ModelControl), new PropertyMetadata(true));
		public static readonly DependencyProperty PresentationDataProperty = DependencyPropertyManager.Register("PresentationData",
			typeof(SeriesPointPresentationData), typeof(ModelControl), new PropertyMetadata(null));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ModelControlFlipped"),
#endif
		Category(Categories.Behavior)
		]
		public bool Flipped {
			get { return (bool)GetValue(FlippedProperty); }
			set { SetValue(FlippedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ModelControlPresentationData"),
#endif
		Category(Categories.Presentation)
		]
		public SeriesPointPresentationData PresentationData {
			get { return (SeriesPointPresentationData)GetValue(PresentationDataProperty); }
			set { SetValue(PresentationDataProperty, value); }
		}
		public ModelControl() {
			SetBinding(Control.DataContextProperty, new Binding("PresentationData") { Source = this });
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			VisualStateManager.GoToState(this, "MouseOver", true);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			VisualStateManager.GoToState(this, "Normal", true);
		}
		internal virtual void SetPointItemBinding(SeriesPointItem pointItem) {
			if (pointItem != null) {
				SetBinding(PresentationDataProperty, new Binding("PresentationData") { Source = pointItem });
				SetBinding(OpacityProperty, new Binding("Opacity") { Source = pointItem });
			}
			else {
				ClearValue(PresentationDataProperty);
				ClearValue(OpacityProperty);
			}
		}
		internal Transform GetFlippedTransform(Size size) {
			if (Flipped)
				return new ScaleTransform() { CenterY = size.Height / 2, ScaleY = -1 };
			return new MatrixTransform() { Matrix = Matrix.Identity };			
		}
	}
	public abstract class PredefinedModelControl : ModelControl {
		public static readonly DependencyProperty PointColorProperty = DependencyPropertyManager.Register("PointColor",
			typeof(Color), typeof(PredefinedModelControl), new PropertyMetadata(ColorChanged));
		[
		Category(Categories.Presentation)
		]
		public Color PointColor {
			get { return (Color)GetValue(PointColorProperty); }
			set { SetValue(PointColorProperty, value); }
		}
		static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedModelControl modelControl = d as PredefinedModelControl;
			if (modelControl != null) {
				modelControl.UpdateBrushes((Color)e.NewValue);
			}
		}
		public PredefinedModelControl() : base() {
			SetBinding(PointColorProperty, new Binding("PresentationData.PointColor") { Source = this });
		}
		protected virtual void UpdateBrushes(Color color) { }
	}
	public class CustomModelControl : ModelControl {
		public CustomModelControl() {
			DefaultStyleKey = typeof(CustomModelControl);
		}
	}
}
