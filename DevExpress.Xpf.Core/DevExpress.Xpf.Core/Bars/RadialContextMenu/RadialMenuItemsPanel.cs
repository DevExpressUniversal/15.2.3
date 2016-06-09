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
using System.Windows.Shapes;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System.Linq;
using System.Collections.Generic;
namespace DevExpress.Xpf.Bars {
	public class RadialMenuItemsPanel : Panel {
		#region static
		public static readonly DependencyProperty AngleProperty = DependencyProperty.RegisterAttached("Angle", typeof(double), typeof(RadialMenuItemsPanel), new PropertyMetadata(0d));
		public static readonly DependencyProperty FirstSectorIndexProperty = DependencyProperty.Register("FirstSectorIndex", typeof(int), typeof(RadialMenuItemsPanel), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange));
		public static double GetAngle(DependencyObject obj) {
			return (double)obj.GetValue(AngleProperty);
		}
		public static void SetAngle(DependencyObject obj, double value) {
			obj.SetValue(AngleProperty, value);
		}
		#endregion
		#region dep props
		public int FirstSectorIndex {
			get { return (int)GetValue(FirstSectorIndexProperty); }
			set { SetValue(FirstSectorIndexProperty, value); }
		}
		#endregion
		protected internal double SectorGap { get { return this.VisualParents().OfType<RadialMenuControl>().FirstOrDefault().Return(p => p.SectorGap, () => 0d); } }
		protected double SectorBottomMargin { get { return Math.Round(SectorGap / Math.Sin(Math.PI / SectorCount)); } }
		protected internal int SectorCount {
			get {
				return Math.Max(InternalChildren.Count, this.VisualParents().OfType<RadialMenuControl>().FirstOrDefault().With(c=>c.Popup as RadialContextMenu).Return(p => p.MinSectorCount, () => 8));
			}
		}
		protected Size MaxItemSize { get; set; }
		Size CalcMaxItemSize(Size availableSize) {
			Size availableItemSize = RadialMenuMeasurerHelper.CalcAvailableItemSize(availableSize, 2 * Math.PI / SectorCount);
			if(availableItemSize.Height.IsNumber()) {
				availableItemSize.Height = Math.Max(0, availableItemSize.Height - SectorBottomMargin);
			}
			Size maxItemSize = new Size();
			foreach(UIElement child in InternalChildren) {
				child.Measure(availableItemSize);
				maxItemSize.Width = Math.Max(child.DesiredSize.Width, maxItemSize.Width);
				maxItemSize.Height = Math.Max(child.DesiredSize.Height, maxItemSize.Height);
			}
			maxItemSize.Height += SectorBottomMargin;
			return maxItemSize;
		}
		protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize) {
			MaxItemSize = CalcMaxItemSize(availableSize);
			return new Size(2d * MaxItemSize.Height, 2d * MaxItemSize.Height);
		}
		protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize) {			
			double w = 360d / SectorCount;
			int startSectorIndex = Math.Min(FirstSectorIndex, SectorCount);
			if(startSectorIndex == -1) {
				startSectorIndex = SectorCount - InternalChildren.Count / 2;
			}
			int currentSectorIndex = startSectorIndex;
			Size itemFinalSize = RadialMenuMeasurerHelper.CalcAvailableItemSize(finalSize, 2 * Math.PI / SectorCount);
			itemFinalSize.Width = Math.Max(itemFinalSize.Width, MaxItemSize.Width);
			itemFinalSize.Height = Math.Max(itemFinalSize.Height, MaxItemSize.Height);
			foreach(BarItemLinkInfo child in InternalChildren.OfType<BarItemLinkInfo>()) {
				if(child.Visibility == System.Windows.Visibility.Collapsed ||
					(child.LinkControl != null && child.LinkControl.Visibility == System.Windows.Visibility.Collapsed))
					continue;
				if(child.LinkControl != null) {
					int sectorIndex = GetItemSectorIndex(child.LinkControl);
					if(sectorIndex != -1)
						currentSectorIndex = startSectorIndex + sectorIndex;
					double angle = currentSectorIndex * w;
					if(angle >= 360)
						angle -= 360;
					SetAngle(child.LinkControl, angle);
					child.Arrange(new Rect(new Point((finalSize.Width - itemFinalSize.Width) / 2, finalSize.Height / 2 - itemFinalSize.Height), itemFinalSize));
					currentSectorIndex++;					
				}
			}
			return finalSize;
		}
		int GetItemSectorIndex(BarItemLinkControlBase linkControl) {
			return linkControl is BarItemLinkControl ? ((BarItemLinkControl)linkControl).ActualSectorIndex : -1;
		}
	}
}
