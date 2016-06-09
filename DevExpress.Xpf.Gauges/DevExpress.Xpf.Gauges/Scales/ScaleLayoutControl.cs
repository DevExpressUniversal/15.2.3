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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[NonCategorized]
	public class ScaleLayoutControl : Control {
		public static readonly DependencyProperty ScaleProperty = DependencyPropertyManager.Register("Scale",
			typeof(Scale), typeof(ScaleLayoutControl), new PropertyMetadata(ScalePropertyChanged));
		public Scale Scale {
			get { return (Scale)GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		static void ScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = e.NewValue as Scale;
			if (scale != null)
				scale.LayoutControl = d as ScaleLayoutControl;
			scale = e.OldValue as ArcScale;
			if (scale != null)
				scale.LayoutControl = null;
		}
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			if (Scale != null) {
				ScaleLayout layout = Scale.CalculateLayout(constraint);
				Clip = layout.Clip;
				constraint = new Size(layout.InitialBounds.Width, layout.InitialBounds.Height);				
			}
			else
				Clip = null;
			return constraint;
		}
		public ScaleLayoutControl() {
			DefaultStyleKey = typeof(ScaleLayoutControl);
		}
	}
}
