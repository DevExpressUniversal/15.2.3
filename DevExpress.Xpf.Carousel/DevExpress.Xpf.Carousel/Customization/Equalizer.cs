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

#if customization
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Input;
namespace DevExpress.Xpf.Carousel {
	public class EqualizerCustomizerPanel : Panel {
		static EqualizerCustomizerPanel() {
			PointsProperty = DependencyProperty.Register("Points", typeof(double[]), typeof(EqualizerCustomizerPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));
		}
		public EqualizerCustomizerPanel() {
		}
		public static readonly DependencyProperty PointsProperty;
		public double[] Points {
			get { return (double[])GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); } 
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Children[0].Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			return base.ArrangeOverride(finalSize);
		}
		Slider[] sliders;
		protected virtual void RebuildChildren() {
			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			Children.Clear();
			sliders = new Slider[Points.Length];
			Children.Add(sp);
			for (int i = 0; i < Points.Length; i++) {
				RebuildBar(i, sp);
			}
		}
		protected void RebuildBar(int index, Panel container) {
			Slider sli = new Slider();
			sli.Minimum = -1;
			sli.Maximum = 1;
			sli.Value = Points[index];
			container.Children.Add(sli);
		}
	}
}
#endif
