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
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class ArcScaleRangePresentation : LayerPresentation {
	}
	public abstract class PredefinedArcScaleRangePresentation : ArcScaleRangePresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PredefinedArcScaleRangePresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedArcScaleRangePresentation presentation = d as PredefinedArcScaleRangePresentation;
			if (presentation != null)
				presentation.ActualFillChanged();
		}
		protected abstract Brush DefaultFill { get; }
		[Category(Categories.Presentation)]
		public Brush ActualFill { get { return Fill != null ? Fill : DefaultFill; } }
		void ActualFillChanged() {
			NotifyPropertyChanged("ActualFill");
		}
	}
	public class DefaultArcScaleRangePresentation : PredefinedArcScaleRangePresentation {
		public override string PresentationName { get { return "Default Range"; } }
		 protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() {StartPoint = new Point(0.5,0), EndPoint = new Point(0.5,1), MappingMode =  BrushMappingMode.RelativeToBoundingBox};
				brush.GradientStops.Add(new GradientStop() {Color=Color.FromArgb(255, 157, 194, 97), Offset=1});
				brush.GradientStops.Add(new GradientStop() {Color=Color.FromArgb(255, 191, 226, 136)});				
				return brush;
			} 
		} 
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new DefaultArcScaleRangeControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultArcScaleRangePresentation();
		}
	}
	public class CustomArcScaleRangePresentation : ArcScaleRangePresentation {
		public static readonly DependencyProperty RangeTemplateProperty = DependencyPropertyManager.Register("RangeTemplate",
			typeof(ControlTemplate), typeof(CustomArcScaleRangePresentation));
		[Category(Categories.Common)]
		public ControlTemplate RangeTemplate {
			get { return (ControlTemplate)GetValue(RangeTemplateProperty); }
			set { SetValue(RangeTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Range"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			CustomPresentationControl modelControl = new CustomPresentationControl();
			modelControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("RangeTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomArcScaleRangePresentation();
		}
	}
}
