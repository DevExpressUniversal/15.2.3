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

using DevExpress.Diagram.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Diagram.Native {
	public static class FrameworkElementExtensions {
		public static Size GetSize(this FrameworkElement element) {
			return new Size(element.Width, element.Height);
		}
		public static Size GetActualSize(this FrameworkElement element) {
			return new Size(element.ActualWidth, element.ActualHeight);
		}
		public static Size GetMinSize(this FrameworkElement element) {
			return new Size(element.MinWidth, element.MinHeight);
		}
		public static void SetSize(this FrameworkElement element, Size size) {
			if(element.Width != size.Width)
				element.Width = size.Width;
			if(element.Height != size.Height)
				element.Height = size.Height;
		}
		public static Point GetPosition(this FrameworkElement element) {
			return new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
		}
		public static void SetPosition(this FrameworkElement element, Point position) {
			Canvas.SetLeft(element, position.X);
			Canvas.SetTop(element, position.Y);
		}
		public static void SetBounds(this FrameworkElement element, Rect rect) {
			element.SetBounds(rect.Location, rect.Size);
		}
		public static void SetBounds(this FrameworkElement element, Point position, Size size) {
			element.SetPosition(position);
			element.SetSize(size);
		}
		public static void SetRotateTransform(this FrameworkElement element, double angle, Point center) {
			element.RenderTransform = new RotateTransform(-angle, center.X, center.Y);
		}
	}
}
