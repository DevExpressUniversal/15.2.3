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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public class BandPanel : Panel {
		public static readonly DependencyProperty IsBandPanelOwnerProperty;
		public static readonly DependencyProperty BandNestingLevelProperty;
		public static readonly DependencyProperty MaxBandNestingLevelProperty;
		public static readonly DependencyProperty HeaderLevelWidthProperty;
		public static readonly DependencyProperty BandPanelGapProperty;
		static BandPanel() {
			DependencyPropertyRegistrator<BandPanel>.New()
				.RegisterAttached((FrameworkElement d) => GetIsBandPanelOwner(d), out IsBandPanelOwnerProperty, false)
				.Register(d => d.BandNestingLevel, out BandNestingLevelProperty, 0, FrameworkPropertyMetadataOptions.AffectsMeasure)
				.Register(d => d.MaxBandNestingLevel, out MaxBandNestingLevelProperty, 0, FrameworkPropertyMetadataOptions.AffectsMeasure)
				.Register(d => d.HeaderLevelWidth, out HeaderLevelWidthProperty, 5.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
				.Register(d => d.BandPanelGap, out BandPanelGapProperty, 0.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
			;
		}
		public static bool GetIsBandPanelOwner(FrameworkElement d) { return (bool)d.GetValue(IsBandPanelOwnerProperty); }
		public static void SetIsBandPanelOwner(FrameworkElement d, bool v) { d.SetValue(IsBandPanelOwnerProperty, v); }
		public double BandPanelGap {
			get { return (double)GetValue(BandPanelGapProperty); }
			set { SetValue(BandPanelGapProperty, value); }
		}
		public int BandNestingLevel {
			get { return (int)GetValue(BandNestingLevelProperty); }
			set { SetValue(BandNestingLevelProperty, value); }
		}
		public int MaxBandNestingLevel {
			get { return (int)GetValue(MaxBandNestingLevelProperty); }
			set { SetValue(MaxBandNestingLevelProperty, value); }
		}
		public double HeaderLevelWidth {
			get { return (double)GetValue(HeaderLevelWidthProperty); }
			set { SetValue(HeaderLevelWidthProperty, value); }
		}
		protected FrameworkElement Owner { get { return LayoutTreeHelper.GetVisualParents(this).OfType<FrameworkElement>().Where(x => GetIsBandPanelOwner(x)).FirstOrDefault(); } }
		protected override Size MeasureOverride(Size constraint) {
			var bandMarkerContainerWidth = GetBandMarkerContainerWidth();
			double maxWidth = 0;
			double maxHeight = 0;
			foreach(UIElement child in Children) {
				child.Measure(new Size(double.PositiveInfinity, constraint.Height));
				maxWidth = Math.Max(maxWidth, bandMarkerContainerWidth);
				maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
			}
			double width = double.IsInfinity(constraint.Width) ? maxWidth * Children.Count : constraint.Width;
			double height = double.IsInfinity(constraint.Height) ? maxHeight : constraint.Height;
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			var bandMarkerContainerWidth = GetBandMarkerContainerWidth();
			foreach(UIElement child in Children) {
				var anchorPoint = (bandMarkerContainerWidth / (MaxBandNestingLevel + 1)) * BandNestingLevel;
				double width = bandMarkerContainerWidth - anchorPoint;
				child.Arrange(new Rect(new Point(anchorPoint, 0.0), new Size(width, finalSize.Height)));
			}
			Owner.Do(x => MarginAdder.SetMargin2(x, new Thickness(-bandMarkerContainerWidth - BandPanelGap, 0.0, 0.0, 0.0)));
			return finalSize;
		}
		double GetBandMarkerContainerWidth() { return HeaderLevelWidth * (MaxBandNestingLevel + 1); }
	}
}
