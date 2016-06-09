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

using DevExpress.Xpf.Charts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts.RangeControlClient.Native {
	public class RangeClientGridLinesItem : RangeClientItem, IRangeClientItem {
		readonly IList<double> values = new List<double>();
		Size size;
		Geometry geometry;
		SolidColorBrush brush;
		[Category(Categories.Data)]
		public Geometry Geometry {
			get { return geometry; }
			private set {
				if (geometry != value) {
					geometry = value;
					RaisePropertyChanged("Geometry");
				}
			}
		}
		[Category(Categories.Data)]
		public bool IsMaster { get { return true; } }
		[Category(Categories.Data)]
		public Point Location {
			get { return new Point(0, 0); }
		}
		[Category(Categories.Data)]
		public Size Size {
			get { return Visible ? size : new Size(0, 0); }
		}
		[Category(Categories.Data)]
		public bool Visible { get; set; }
		[Category(Categories.Data)]
		public SolidColorBrush Brush {
			get { return brush; }
			set {
				if (brush != value) {
					brush = value;
					RaisePropertyChanged("Brush");
				}
			}
		}
		public RangeClientGridLinesItem() {
			Visible = true;
			Brush = new SolidColorBrush(Colors.Black);
		}
		void IRangeClientItem.CalculateLayout(IRangeClientScaleMap map, Size availableSize, Size desiredSize) {
			size = availableSize;
			PathGeometry pathGeometry = new PathGeometry() { Figures = new PathFigureCollection() };
			foreach (double value in values) {
				double x = Math.Round(map.GetScreenPoint(value)) + 0.5;
				PathFigure figure = new PathFigure() { StartPoint = new Point(x, 0), Segments = new PathSegmentCollection() };
				figure.Segments.Add(new LineSegment() { Point = new Point(x, availableSize.Height) });
				pathGeometry.Figures.Add(figure);
			}
			Geometry = pathGeometry;
		}
		internal void UpdateGrid(IEnumerable<double> gridValues) {
			values.Clear();
			foreach (double value in gridValues)
				values.Add(value);
		}
	}
}
