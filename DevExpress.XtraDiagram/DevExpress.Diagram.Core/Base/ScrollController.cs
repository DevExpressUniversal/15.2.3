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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
namespace DevExpress.Diagram.Core {
	public delegate Point OffsetCorrection(Point oldOffset, Rect oldExtent, Rect newExtent, Size oldViewport, Size newViewport, double oldZoom, double newZoom);
	public static class OffsetCorrections {
		public static OffsetCorrection Init(bool keepHorizontalOffset, bool keepVerticalOffset) {
			return (oldOffset, oldExtent, newExtent, oldViewport, newViewport, oldZoom, newZoom) =>
				InitCore(newExtent, newViewport).ValidateCorrection(keepHorizontalOffset, keepVerticalOffset);
		}
		static Point InitCore(Rect newExtent, Size newViewport) {
			return MathHelper.GetDifference(newViewport, newExtent.Size).ScalePoint(.5).OffsetPoint(newExtent.Location);
		}
		public static Point Viewport(Point oldOffset, Rect oldExtent, Rect newExtent, Size oldViewport, Size newViewport, double oldZoom, double newZoom) {
			var result = MathHelper.GetDifference(newViewport, oldViewport).ScalePoint(.5);
			result = result.OffsetPoint(MathHelper.GetOffset(oldExtent.Location, newExtent.Location));
			result = ValidateCorrection(oldOffset, newExtent, newViewport, result);
			return result;
		}
		public static OffsetCorrection Extent(bool keepHorizontalOffset, bool keepVerticalOffset) {
			return (oldOffset, oldExtent, newExtent, oldViewport, newViewport, oldZoom, newZoom) =>
				MathHelper.GetDifference(oldExtent.Size, newExtent.Size)
				.ScalePoint(.5)
				.OffsetPoint(MathHelper.GetOffset(oldExtent.Location, newExtent.Location))
				.ValidateCorrection(keepHorizontalOffset, keepVerticalOffset);
		}
		public static OffsetCorrection Zoom() {
			return (oldOffset, oldExtent, newExtent, oldViewport, newViewport, oldZoom, newZoom) =>
				Zoom(oldOffset, oldExtent, newExtent, oldViewport, newViewport, oldZoom, newZoom, oldViewport.ScaleSize(0.5).ToPoint());
		}
		public static OffsetCorrection GetZoomCorrection(Point zoomPoint) {
			return (oldOffset, oldExtent, newExtent, oldViewport, newViewport, oldZoom, newZoom) =>
				Zoom(oldOffset, oldExtent, newExtent, oldViewport, newViewport, oldZoom, newZoom, zoomPoint);
		}
		static Point Zoom(Point oldOffset, Rect oldExtent, Rect newExtent, Size oldViewport, Size newViewport, double oldZoom, double newZoom, Point zoomPoint) {
			var oldCenter = oldOffset.OffsetPoint(zoomPoint);
			var newCenter = oldCenter.ScalePoint(newZoom / oldZoom);
			var result = MathHelper.GetOffset(oldCenter, newCenter);
			result = ValidateCorrection(oldOffset, newExtent, newViewport, result);
			return result;
		}
		static Point ValidateCorrection(Point oldOffset, Rect newExtent, Size newViewport, Point offsetCorrection) {
			var extentResult = InitCore(newExtent, newViewport);
			if(newViewport.Width > newExtent.Width)
				offsetCorrection.X = extentResult.X - oldOffset.X;
			if(newViewport.Height > newExtent.Height)
				offsetCorrection.Y = extentResult.Y - oldOffset.Y;
			return offsetCorrection;
		}
		static Point ValidateCorrection(this Point offsetCorrection, bool keepHorizontalOffset, bool keepVerticalOffset) {
			if(keepHorizontalOffset)
				offsetCorrection = offsetCorrection.SetX(0);
			if(keepVerticalOffset)
				offsetCorrection = offsetCorrection.SetY(0);
			return offsetCorrection;
		}
	}
}
