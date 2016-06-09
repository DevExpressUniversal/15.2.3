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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.RangeControl.Internal {
	public interface IMapping {
		Size Size { get; }
		Point Location { get; }
		Point GetPoint(double x, double y);
		Point GetSnappedPoint(double x, double y, bool snapToPixels = true);
	}
	public class PointMapping : IMapping {
		public Size Size { get; private set; }
		public Point Location { get; private set; }
		public PointMapping(IMapping mapping, Point offset) {
			Location = Add(mapping.Location, offset);
			Size = new Size(mapping.Size.Width - offset.X, mapping.Size.Height - offset.Y);
		}
		private Point Add(Point p1, Point p2) {
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}
		public PointMapping(Point offset, Rect surfaceBounds) {
			Size surfaceSize = surfaceBounds.Size();
			Size = new Size(surfaceSize.Width - offset.X, surfaceSize.Height - offset.Y);
			Location = Add(surfaceBounds.Location, offset);
		}
		public Point GetPoint(double x, double y) {
			double rotatedY = Size.Height - y;
			return new Point(x + Location.X, rotatedY + Location.Y);
		}
		public Point GetSnappedPoint(double normalX, double normalY, bool snapToPixels = true) {
			double x = Size.Width * normalX;
			double y = Size.Height * normalY;
			Point point = GetPoint(x, y);
			return snapToPixels ? new Point(SnapToPixels(point.X), SnapToPixels(point.Y)) : point;
		}
		double SnapToPixels(double value) {
			return Math.Round(value);
		}
	}
}
