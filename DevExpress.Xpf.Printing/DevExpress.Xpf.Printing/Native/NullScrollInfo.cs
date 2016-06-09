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
using System.Windows.Media;
namespace DevExpress.Xpf.Printing.Native {
	class NullScrollInfo : ScrollInfoBase {
		public NullScrollInfo(FrameworkElement scrollablePageView)
			: base(scrollablePageView, null, new Thickness()) {
		}
		public override double ExtentHeight {
			get { return 0; }
		}
#if SL
		public override Rect MakeVisible(UIElement element, Rect rectangle) {
			throw new InvalidOperationException();
		}
#else
		public override Rect MakeVisible(Visual visual, Rect rectangle) {
			return rectangle;
		}
#endif
		protected override double ScrollablePageViewHeight {
			get { return 0; }
		}
		protected override double GetVerticalScrollOffset(ScrollMode scrollMode, ScrollDirection scrollDirection) {
			return 0;
		}
		public override void SetCurrentPageIndex() {
			throw new InvalidOperationException();
		}
		public override double GetTransformX() {
			throw new InvalidOperationException();
		}
		public override double GetTransformY() {
			throw new InvalidOperationException();
		}
	}
}
