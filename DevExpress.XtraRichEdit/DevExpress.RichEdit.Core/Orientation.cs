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
using System.Drawing;
namespace DevExpress.XtraRichEdit.Internal {
	#region IOrientation
	public interface IOrientation {
		int GetPrimaryCoordinate(Point point);
		int GetSecondaryCoordinate(Point point);
		Point CreatePoint(int primary, int secondary);
		float GetNearPrimaryCoordinate(RectangleF bounds);
		float GetFarPrimaryCoordinate(RectangleF bounds);
		float GetPrimaryCoordinateExtent(RectangleF bounds);
		int GetNearPrimaryCoordinate(Rectangle bounds);
		int GetFarPrimaryCoordinate(Rectangle bounds);
		RectangleF SetNearPrimaryCoordinate(RectangleF bounds, float value);
	}
	#endregion
	#region HorizontalOrientation
	public class HorizontalOrientation : IOrientation {
		public int GetPrimaryCoordinate(Point point) {
			return point.X;
		}
		public int GetSecondaryCoordinate(Point point) {
			return point.Y;
		}
		public Point CreatePoint(int primary, int secondary) {
			return new Point(primary, secondary);
		}
		public float GetNearPrimaryCoordinate(RectangleF bounds) {
			return bounds.Left;
		}
		public float GetFarPrimaryCoordinate(RectangleF bounds) {
			return bounds.Right;
		}
		public float GetPrimaryCoordinateExtent(RectangleF bounds) {
			return bounds.Width;
		}
		public int GetNearPrimaryCoordinate(Rectangle bounds) {
			return bounds.Left;
		}
		public int GetFarPrimaryCoordinate(Rectangle bounds) {
			return bounds.Right;
		}
		public RectangleF SetNearPrimaryCoordinate(RectangleF bounds, float value) {
			bounds.X = value;
			return bounds;
		}
	}
	#endregion
	#region VerticalOrientation
	public class VerticalOrientation : IOrientation {
		public int GetPrimaryCoordinate(Point point) {
			return point.Y;
		}
		public int GetSecondaryCoordinate(Point point) {
			return point.X;
		}
		public Point CreatePoint(int primary, int secondary) {
			return new Point(secondary, primary);
		}
		public float GetNearPrimaryCoordinate(RectangleF bounds) {
			return bounds.Top;
		}
		public float GetFarPrimaryCoordinate(RectangleF bounds) {
			return bounds.Bottom;
		}
		public float GetPrimaryCoordinateExtent(RectangleF bounds) {
			return bounds.Height;
		}
		public int GetNearPrimaryCoordinate(Rectangle bounds) {
			return bounds.Top;
		}
		public int GetFarPrimaryCoordinate(Rectangle bounds) {
			return bounds.Bottom;
		}
		public RectangleF SetNearPrimaryCoordinate(RectangleF bounds, float value) {
			bounds.Y = value;
			return bounds;
		}
	}
	#endregion
}
