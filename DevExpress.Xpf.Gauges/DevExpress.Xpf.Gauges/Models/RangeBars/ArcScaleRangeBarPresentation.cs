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
	public abstract class ArcScaleRangeBarPresentation : ValueIndicatorPresentation {		
	}
	public abstract class PredefinedArcScaleRangeBarPresentation : ArcScaleRangeBarPresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PredefinedArcScaleRangeBarPresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedArcScaleRangeBarPresentation presentation = d as PredefinedArcScaleRangeBarPresentation;
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
	public class DefaultArcScaleRangeBarPresentation : PredefinedArcScaleRangeBarPresentation {
		public override string PresentationName { get { return "Default Range Bar"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 239, 194, 217)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new DefaultArcScaleRangeBarControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultArcScaleRangeBarPresentation();
		}
	}
	public class CustomArcScaleRangeBarPresentation : ArcScaleRangeBarPresentation {
		public static readonly DependencyProperty RangeBarTemplateProperty = DependencyPropertyManager.Register("RangeBarTemplate",
			typeof(ControlTemplate), typeof(CustomArcScaleRangeBarPresentation));
		[Category(Categories.Common)]
		public ControlTemplate RangeBarTemplate {
			get { return (ControlTemplate)GetValue(RangeBarTemplateProperty); }
			set { SetValue(RangeBarTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Range Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			CustomValueIndicatorPresentationControl presentationControl = new CustomValueIndicatorPresentationControl();
			presentationControl.SetBinding(CustomValueIndicatorPresentationControl.TemplateProperty, new Binding("RangeBarTemplate") { Source = this });
			return presentationControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomArcScaleRangeBarPresentation();
		}
	}
}
