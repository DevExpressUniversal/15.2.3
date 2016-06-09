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
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using System.Windows.Shapes;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
namespace DevExpress.Xpf.Editors.Filtering {
	public class FilterControlFocusVisualHelper {
		FrameworkElement focusedElement;
		Canvas focusVisualContainer;
		Style focusVisualStyle;
		public FilterControlFocusVisualHelper(Canvas focusVisualContainer, Style focusVisualStyle) {
			this.focusVisualContainer = focusVisualContainer;
			this.focusVisualStyle = focusVisualStyle;
		}
		public FrameworkElement FocusedElement {
			get { return focusedElement; }
			set {
				if(value == focusedElement) return;
				FrameworkElement oldValue = focusedElement;
				focusedElement = value;
				FocusedElementChanged(oldValue);
			}
		}
		FrameworkElement CreateFocusVisual() {
			ContentControl result = new ContentControl() { Style = focusVisualStyle, IsHitTestVisible = false };
#if !SL
			result.Focusable = false;
#else
			result.IsTabStop = false;
#endif
			return result;
		}
		void FocusedElementChanged(FrameworkElement oldValue) {
			if(oldValue != null) {
				HideFocusVisual();
				oldValue.LayoutUpdated -= OnFocusedElementLayoutUpdated;
			}
			if(FocusedElement != null) {
				ShowFocusVisual();
				FocusedElement.LayoutUpdated += OnFocusedElementLayoutUpdated;
			}
		}
		void HideFocusVisual() {
			if(focusVisualContainer != null)
				focusVisualContainer.Children.Clear();
		}
		void OnFocusedElementLayoutUpdated(object sender, System.EventArgs e) {
#if SL
			if(!FocusedElement.IsInVisualTree()) {
				FocusedElement = null;
				return;
			}
#endif
			UpdateFocusVisualBounds();
		}
		void ShowFocusVisual() {
			if(focusVisualContainer == null) return;
#if !SL
			FocusedElement.FocusVisualStyle = null;
#endif
			FrameworkElement focusVisual = CreateFocusVisual();
			focusVisualContainer.Children.Add(focusVisual);
			UpdateFocusVisualBounds();
		}
		void UpdateFocusVisualBounds() {
			Rect bounds = FocusedElement.GetBounds(focusVisualContainer);
			FrameworkElement focusVisual = (FrameworkElement)focusVisualContainer.Children[0];
			Canvas.SetLeft(focusVisual, bounds.Left);
			Canvas.SetTop(focusVisual, bounds.Top);
			focusVisual.Width = FocusedElement.ActualWidth;
			focusVisual.Height = FocusedElement.ActualHeight;
		}
	}
}
