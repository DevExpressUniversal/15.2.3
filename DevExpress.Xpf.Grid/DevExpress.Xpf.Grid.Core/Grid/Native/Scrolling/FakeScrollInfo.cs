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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#if SL
using Visual = System.Windows.UIElement;
#endif
namespace DevExpress.Xpf.Grid.Native {
	class FakeScrollInfo : IScrollInfo {
		#region IScrollInfo Members
		public bool CanHorizontallyScroll { get; set; }
		public bool CanVerticallyScroll { get; set; }
		public double ExtentHeight {
			get { return 0d; }
		}
		public double ExtentWidth {
			get { return 0d; }
		}
		public double HorizontalOffset {
			get { return 0d; }
		}
		public void LineDown() {
		}
		public void LineLeft() {
		}
		public void LineRight() {
		}
		public void LineUp() {
		}
		public Rect MakeVisible(Visual visual, Rect rectangle) {
			return Rect.Empty;
		}
		public void MouseWheelDown() {
		}
		public void MouseWheelLeft() {
		}
		public void MouseWheelRight() {
		}
		public void MouseWheelUp() {
		}
		public void PageDown() {
		}
		public void PageLeft() {
		}
		public void PageRight() {
		}
		public void PageUp() {
		}
		public ScrollViewer ScrollOwner { get; set; }
		public void SetHorizontalOffset(double offset) {
		}
		public void SetVerticalOffset(double offset) {
		}
		public double VerticalOffset {
			get { return 0d; }
		}
		public double ViewportHeight {
			get { return 0d; }
		}
		public double ViewportWidth {
			get { return 0d; }
		}
		#endregion
	}
}
