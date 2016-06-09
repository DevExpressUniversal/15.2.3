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
using System.Text;
using System.Windows.Controls;
using System.Windows;
namespace DevExpress.Xpf.Core {
	public abstract class SizeHelperBase {
		public static SizeHelperBase GetDefineSizeHelper(Orientation orientation) {
			return orientation == Orientation.Vertical ?
				VerticalSizeHelper.Instance :
				HorizontalSizeHelper.Instance;
		}
		public static SizeHelperBase GetSecondarySizeHelper(Orientation orientation) {
			return orientation == Orientation.Vertical ?
				HorizontalSizeHelper.Instance :
				VerticalSizeHelper.Instance;
		}
		protected SizeHelperBase() { }
		public abstract double GetDefineSize(Size size);
		public abstract void SetDefineSize(ref Size size, double value);
		public abstract double GetSecondarySize(Size size);
		public abstract void SetSecondarySize(ref Size size, double value);
		public abstract double GetDefinePoint(Point point);
		public abstract void SetDefinePoint(ref Point point, double value);
		public abstract double GetSecondaryPoint(Point point);
		public abstract void SetSecondaryPoint(ref Point point, double value);
		public abstract double GetMarginSpace(Thickness thickness);
		public abstract Size CreateSize(double defineSize, double secondarySize);
		public abstract Point CreatePoint(double definePoint, double secondaryPoint);
		public abstract double GetDefineSize(FrameworkElement elem);
	}
	public class VerticalSizeHelper : SizeHelperBase {
		public static readonly SizeHelperBase Instance = new VerticalSizeHelper();
		VerticalSizeHelper() { }
		public override double GetDefineSize(Size size) { return size.Height; }
		public override void SetDefineSize(ref Size size, double value) { size.Height = value; }
		public override double GetSecondarySize(Size size) { return size.Width; }
		public override void SetSecondarySize(ref Size size, double value) { size.Width = value; }
		public override double GetDefinePoint(Point point) { return point.Y; }
		public override void SetDefinePoint(ref Point point, double value) { point.Y = value; }
		public override double GetSecondaryPoint(Point point) { return point.X; }
		public override void SetSecondaryPoint(ref Point point, double value) { point.X = value; }
		public override Size CreateSize(double defineSize, double secondarySize) { return new Size(secondarySize, defineSize);  }
		public override Point CreatePoint(double definePoint, double secondaryPoint) { return new Point(secondaryPoint, definePoint); }
		public override double GetMarginSpace(Thickness thickness) { return thickness.Bottom + thickness.Top; }
		public override double GetDefineSize(FrameworkElement elem) { return elem == null ? 0 : elem.ActualHeight; }
	}
	public class HorizontalSizeHelper : SizeHelperBase {
		public static readonly SizeHelperBase Instance = new HorizontalSizeHelper();
		HorizontalSizeHelper() { }
		public override double GetDefineSize(Size size) { return size.Width; }
		public override void SetDefineSize(ref Size size, double value) { size.Width = value; }
		public override double GetSecondarySize(Size size) { return size.Height; }
		public override void SetSecondarySize(ref Size size, double value) { size.Height = value; }
		public override double GetDefinePoint(Point point) { return point.X; }
		public override void SetDefinePoint(ref Point point, double value) { point.X = value; }
		public override double GetSecondaryPoint(Point point) { return point.Y; }
		public override void SetSecondaryPoint(ref Point point, double value) { point.Y = value; }
		public override Size CreateSize(double defineSize, double secondarySize) { return new Size(defineSize, secondarySize); }
		public override Point CreatePoint(double definePoint, double secondaryPoint) { return new Point(definePoint, secondaryPoint); }
		public override double GetMarginSpace(Thickness thickness) { return thickness.Left + thickness.Right; }
		public override double GetDefineSize(FrameworkElement elem) { return elem == null ? 0 : elem.ActualWidth; }
	}
}
