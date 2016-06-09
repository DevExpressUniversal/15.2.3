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
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class ScrollBarPresentation : Control, ILayoutElement {
		static ScrollBarPresentation() {
			FocusableProperty.OverrideMetadata(typeof(ScrollBarPresentation), new FrameworkPropertyMetadata(false));
		}
		readonly ScrollBarItem scrollBarItem;
		public ScrollBarItem ScrollBarItem { get { return scrollBarItem; } }
		ILayout ILayoutElement.Layout { get { return scrollBarItem == null ? null : scrollBarItem.Layout; } }
		internal ScrollBarPresentation(ScrollBarItem scrollBarItem) {
			DefaultStyleKey = typeof(ScrollBarPresentation);
			this.scrollBarItem = scrollBarItem;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (scrollBarItem != null) {
				ILayout layout = scrollBarItem.Layout;
				if (layout != null) {
					Rect bounds = layout.Bounds;
					Size size = new Size(bounds.Width, bounds.Height);
					return base.MeasureOverride(size);
				}
			}
			base.MeasureOverride(constraint);
			return new Size(0, 0);
		}
	}
}
