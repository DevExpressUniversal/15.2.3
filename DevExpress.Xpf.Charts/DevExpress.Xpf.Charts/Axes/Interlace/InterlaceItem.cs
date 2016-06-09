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

using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	public enum InterlaceType {
		Rectangle,
		Polygon,
		CounterclockwisePie,
		ClockwisePie,
		Ellipse
	}
	[NonCategorized]
	public class InterlaceGeometry {
		readonly InterlaceType type;
		readonly List<Point> points;
		readonly List<Point> holePoints;
		readonly Rect bounds;
		public InterlaceType Type { get { return type; } }
		public List<Point> Points { get { return points; } }
		public List<Point> HolePoints { get { return holePoints; } }
		public Rect Bounds { get { return bounds; } }
		public InterlaceGeometry(InterlaceType type, List<Point> points, List<Point> holePoints, Rect bounds) {
			this.type = type;
			this.points = points;
			this.holePoints = holePoints;
			this.bounds = bounds;
		}
	}
	[NonCategorized]
	public class InterlaceItem : NotifyPropertyChangedObject{
		readonly AxisBase axis;
		InterlaceGeometry geometry;
		public AxisBase Axis { get { return axis; } }
		public InterlaceGeometry Geometry {
			get { return geometry; }
			set {
				geometry = value;
				OnPropertyChanged("Geometry");
			}
		}
		public InterlaceItem(AxisBase axis) {
			this.axis = axis;
		}
	}
}
