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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public class SelectionCanvas : Control {
		#region static
		public static readonly DependencyProperty SelectionRectangleProperty;
		static SelectionCanvas() {
			Type ownerType = typeof(SelectionCanvas);
			SelectionRectangleProperty = DependencyPropertyManager.Register("SelectionRectangle", typeof(SelectionRectangle), ownerType);
		}
		public SelectionRectangle SelectionRectangle {
			get { return (SelectionRectangle)GetValue(SelectionRectangleProperty); }
			set { SetValue(SelectionRectangleProperty, value); }
		}
		#endregion
		const string PartSelectionCanvas = "PART_SelectionCanvas";
		Canvas Selection { get; set; }
		public SelectionCanvas() {
			this.SetDefaultStyleKey(typeof(SelectionCanvas));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Selection = GetTemplateChild(PartSelectionCanvas) as Canvas;
		}
	}
	public class SelectionRectangle : BindableBase {
		public double Width { get { return Math.Abs(StartPoint.X - AnchorPoint.X); } }
		public double Height { get { return Math.Abs(StartPoint.Y - AnchorPoint.Y); } }
		public double X { get { return Math.Min(StartPoint.X, AnchorPoint.X); } }
		public double Y { get { return Math.Min(StartPoint.Y, AnchorPoint.Y); } }
		public bool IsEmpty { get { return Width.AreClose(0d) && Height.AreClose(0d); } }
		public Rect Rectangle { get { return new Rect(X + HorizontalOffset, Y + VerticalOffset, Width, Height); } }
		Size ViewportSize { get; set; }
		public Point StartPoint { get; private set; }
		public Point AnchorPoint { get; private set; }
		double HorizontalOffset { get; set; }
		double VerticalOffset { get; set; }
		public SelectionRectangle(Point startPoint, Size viewportSize) {
			ViewportSize = viewportSize;
			SetStartPoint(startPoint);
		}
		public void AddStartXOffset(double dx) {
			StartPoint = new Point(StartPoint.X + dx, StartPoint.Y);
		}
		public void AddStartYOffset(double dy) {
			StartPoint = new Point(StartPoint.X, StartPoint.Y + dy);
		}
		public void AddAnchorXOffset(double dx) {
			SetAnchorPoint(new Point(AnchorPoint.X + dx, AnchorPoint.Y));
		}
		public void AddAnchorYOffset(double dy) {
			SetAnchorPoint(new Point(AnchorPoint.X, AnchorPoint.Y + dy));
		}
		public void SetPointPosition(Point point) {
			SetAnchorPoint(point);
		}
		Point CoerceAnchorPoint(Point point) {
			return new Point(Math.Max(-HorizontalOffset, Math.Min(point.X, ViewportSize.Width - HorizontalOffset)), Math.Max(-VerticalOffset, Math.Min(point.Y, ViewportSize.Height - VerticalOffset)));
		}
		void SetAnchorPoint(Point p) {
			AnchorPoint = CoerceAnchorPoint(p);
			RaisePropertiesChanged(() => X, () => Y, () => Width, () => Height);
		}
		public void SetViewport(Size size) {
			ViewportSize = size;
			SetAnchorPoint(AnchorPoint);
		}
		public void SetStartPoint(Point point) {
			StartPoint = point;
			Reset();
		}
		public void Reset() {
			SetAnchorPoint(StartPoint);
		}
		public void SetVerticalOffset(double offset, bool coerceStart) {
			double dy = offset - VerticalOffset;
			VerticalOffset = offset;
			if(coerceStart)
				StartPoint = new Point(StartPoint.X, StartPoint.Y - dy);
		}
		public void SetHorizontalOffset(double offset, bool coerceStart) {
			double dx = offset - HorizontalOffset;
			HorizontalOffset = offset;
			if(coerceStart)
				StartPoint = new Point(StartPoint.X - dx, StartPoint.Y);
		}
	}
}
