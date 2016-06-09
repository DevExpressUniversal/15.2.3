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
using DevExpress.DemoData.Helpers;
using System.Windows;
namespace DevExpress.Xpf.DemoBase.Helpers {
	class TopTextPanel : LayoutPanel {
		#region Dependency Properties
		public static readonly DependencyProperty TopChildProperty;
		public static readonly DependencyProperty MainChildProperty;
		static TopTextPanel() {
			Type ownerType = typeof(TopTextPanel);
			TopChildProperty = DependencyProperty.Register("TopChild", typeof(UIElement), ownerType, new PropertyMetadata(null, RaiseTopChildChanged));
			MainChildProperty = DependencyProperty.Register("MainChild", typeof(UIElement), ownerType, new PropertyMetadata(null, RaiseMainChildChanged));
		}
		UIElement topChildValue = null;
		UIElement mainChildValue = null;
		static void RaiseTopChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TopTextPanel)d).topChildValue = (UIElement)e.NewValue;
			((TopTextPanel)d).RaiseTopChildChanged(e);
		}
		static void RaiseMainChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TopTextPanel)d).mainChildValue = (UIElement)e.NewValue;
			((TopTextPanel)d).RaiseMainChildChanged(e);
		}
		#endregion
		RectClass topChildRect = new RectClass();
		RectClass mainChildRect = new RectClass();
		public UIElement TopChild { get { return topChildValue; } set { SetValue(TopChildProperty, value); } }
		public UIElement MainChild { get { return mainChildValue; } set { SetValue(MainChildProperty, value); } }
		protected override Size MeasureOverride(Size availableSize) {
			mainChildRect.Size = availableSize;
			MeasureChild(mainChildRect, MainChild, true, true);
			Size mainSize = mainChildRect.Size;
			topChildRect.Size = GetSize(mainSize.Width, availableSize.Height);
			MeasureChild(topChildRect, TopChild, true, false);
			mainChildRect.Size = GetSize(mainSize.Width, mainSize.Height - topChildRect.Height);
			MeasureChild(mainChildRect, MainChild, true, true);
			topChildRect.Size = GetSize(mainChildRect.Width, mainSize.Height - mainChildRect.Height);
			MeasureChild(topChildRect, TopChild, true, true);
			topChildRect.Location = new Point();
			mainChildRect.Location = new Point(0.0, topChildRect.Height);
			return GetSize(mainChildRect.Width, mainSize.Height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			ArrangeChild(mainChildRect, MainChild);
			ArrangeChild(topChildRect, TopChild);
			return finalSize;
		}
		void RaiseTopChildChanged(DependencyPropertyChangedEventArgs e) { UpdateChild(e.OldValue, e.NewValue, 2); }
		void RaiseMainChildChanged(DependencyPropertyChangedEventArgs e) { UpdateChild(e.OldValue, e.NewValue, 1); }
	}
}
