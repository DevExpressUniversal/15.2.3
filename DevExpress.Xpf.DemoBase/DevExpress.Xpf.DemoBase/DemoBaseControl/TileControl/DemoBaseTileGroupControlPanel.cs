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

using DevExpress.Xpf.DemoBase.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
namespace DevExpress.Xpf.DemoBase {
	class DemoBaseTileGroupControlPanel : Panel {
		public bool IsSimple {
			get { return (bool)GetValue(IsSimpleProperty); }
			set { SetValue(IsSimpleProperty, value); }
		}
		public static readonly DependencyProperty IsSimpleProperty =
			DependencyProperty.Register("IsSimple", typeof(bool), typeof(DemoBaseTileGroupControlPanel), new PropertyMetadata(true));
		Size UnitTileSize {
			get {
				if(Children.Count == 0)
					return new Size(50, 50);
				UIElement container = (UIElement)Children[0];
				UIElement child = (UIElement)VisualTreeHelper.GetChild(container, 0);
				switch(DemoBaseTileControl.GetTileSize(child)) {
					case DemoBaseTileSize.Unit:
						return container.DesiredSize;
					case DemoBaseTileSize.Double:
						return new Size(container.DesiredSize.Width / 2, container.DesiredSize.Height);
					case DemoBaseTileSize.Quad:
						return new Size(container.DesiredSize.Width / 2, container.DesiredSize.Height / 2);
					default: Debug.Assert(false); return container.DesiredSize;
				}
			}
		}
		CustomTileLayoutCalculator calculator {
			get {
				if (IsSimple)
					return new OrdinarTileLayoutCalculator();
				else
					return new VerticalTileLayoutCalculator();
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(Children.Count == 0) {
				return new Size(0, 0);
			}
			foreach(UIElement child in Children) {
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			}
			foreach(UIElement child in Children) {
				Debug.Assert(child.DesiredSize.Width % UnitTileSize.Width < 0.001);
			}
			List<Tuple<FrameworkElement, CustomTileLayoutInfo>> map;
			List<CustomTileLayoutInfo> infos;
			GetInfos(out map, out infos, availableSize.Height);
			double width = infos.Max(i => i.Column + i.ColumnSpan) * UnitTileSize.Width;
			double height = infos.Max(i => i.Row + i.RowSpan) * UnitTileSize.Height;
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(!Children.Cast<UIElement>().Any()) {
				return finalSize;
			}
			List<Tuple<FrameworkElement, CustomTileLayoutInfo>> map;
			List<CustomTileLayoutInfo> infos;
			GetInfos(out map, out infos, finalSize.Height);
			foreach(var t in map) {
				FrameworkElement fe = t.Item1;
				CustomTileLayoutInfo info = t.Item2;
				double x = info.Column * UnitTileSize.Width;
				double y = info.Row * UnitTileSize.Height;
				Point curPos = PanelHelper.GetCurrentPosition(fe, this);
				var newPos = new Point(x, y);
				fe.Arrange(new Rect(curPos, fe.DesiredSize));
				PanelHelper.Animate(fe, curPos, newPos);
			}
			return finalSize;
		}
		private void GetInfos(out List<Tuple<FrameworkElement, CustomTileLayoutInfo>> map, out List<CustomTileLayoutInfo> infos, double height) {
			map = new List<Tuple<FrameworkElement, CustomTileLayoutInfo>>();
			foreach(FrameworkElement fe in Children) {
				int columnSpan = (int)Math.Round(fe.DesiredSize.Width / UnitTileSize.Width);
				int rowSpan = (int)Math.Round(fe.DesiredSize.Height / UnitTileSize.Height);
				map.Add(Tuple.Create(fe, new CustomTileLayoutInfo { ColumnSpan = columnSpan, RowSpan = rowSpan }));
			}
			infos = map.Select(t => t.Item2).ToList();
			int rowCount = Math.Max(1, (int)Math.Floor(height / UnitTileSize.Height));
			int columnCount = 2;
			if(infos.Count < 4 && !infos.Any(i => i.ColumnSpan == 2))
				columnCount = 3;
			calculator.CalculateLayout(infos, rowCount, columnCount);
		}
	}
}
