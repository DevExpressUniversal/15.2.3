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
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Grid.HitTest;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.Hierarchy;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid {
	public class CardDataPresenter : DataPresenterManipulation {
		CardView CardView { get { return (CardView)View; } }
		CardsHierarchyPanel CardsPanel { get { return Panel as CardsHierarchyPanel; } }
		protected internal override int GenerateItemsOffset { get { return CardsPanel != null ? CardsPanel.GenerateItemsOffset : ScrollOffset; } }
		protected override double Extent { get { return CardsPanel != null ? CardsPanel.Extent : base.Extent; } }
		protected override void OnDefineScrollInfoChangedCore() {
			base.OnDefineScrollInfoChangedCore();
			if(CardsPanel != null)
				CardsPanel.OnDefineScrollInfoChanged();
		}
		public CardDataPresenter() {
			GridViewHitInfoBase.SetHitTestAcceptor(this, new DataAreaTableViewHitTestAcceptor());
		}
		protected override FrameworkElement CreateContent() {
			return new CardsContainer() { DataPresenter = this };
		}
		protected override void UpdateViewCore() {
			if(Content != null)
				((FrameworkElement)Content).DataContext = View;
		}
		protected override Size GetMeasureSize(Size constraint) {
			return constraint;
		}
		protected override bool OnLayoutUpdatedCore() {
			return CardsPanel.Return(x => x.OnLayoutUpdated(), () => true) && base.OnLayoutUpdatedCore();
		}
		protected override VirtualDataStackPanelScrollInfo CreateScrollInfo() {
			VirtualDataStackPanelScrollInfo result = base.CreateScrollInfo();
			BindingOperations.SetBinding(result, VirtualDataStackPanelScrollInfo.OrientationProperty, new Binding(CardView.OrientationProperty.Name) { Source = View });
			return result;
		}
		protected override Size GetFirstElementSize() {
			if(CardsPanel == null || CardsPanel.RowsInfo == null || CardsPanel.RowsInfo.Count == 0)
				return Size.Empty;
			return CardsPanel.RowsInfo[0].RenderSize;
		}
		protected override double DefineDelta(Point translation, Size firstElementSize) {
			if(View.ViewBehavior.AllowPerPixelScrolling)
				return SizeHelper.GetDefinePoint(translation) / SizeHelper.GetDefineSize(firstElementSize);
			if(CardView.CardLayout == CardLayout.Columns)
				return (translation.X / firstElementSize.Width) * 3;
			else
				return (translation.Y / firstElementSize.Height) * 3;
		}
		protected override double GetTranslation(double delta, Size firstElementSize) {
			if(CardView.CardLayout == CardLayout.Columns)
				return View.ViewBehavior.AllowPerPixelScrolling ? delta : delta * firstElementSize.Width / 3;
			else
				return View.ViewBehavior.AllowPerPixelScrolling ? delta : delta * firstElementSize.Height / 3;
		}
		protected override void ChangeOffset(DataViewBehavior behavior, double delta, Point translation) {
			behavior.ChangeHorizontalOffsetBy(CardView.CardLayout == CardLayout.Columns ? - delta : - translation.X);
			behavior.ChangeVerticalOffsetBy(CardView.CardLayout == CardLayout.Columns ? - translation.Y : -delta);
		}
		protected override Point GetAccumulator(double translation) {
			return CardView.CardLayout == CardLayout.Columns ? new Point(translation, 0) : new Point(0, translation);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			return base.ArrangeOverride(arrangeBounds);
		}
		protected override DataControlScrollMode VerticalScrollModeCore {
			get {
				if(CardView.Orientation == Orientation.Horizontal)
					return DataControlScrollMode.Pixel;
				return View.ViewBehavior.AllowPerPixelScrolling ? DataControlScrollMode.RowPixel : DataControlScrollMode.Item; 
			} 
		}
		protected override DataControlScrollMode HorizontalScrollModeCore {
			get {
				if(CardView.Orientation == Orientation.Vertical)
					return DataControlScrollMode.Pixel;
				return View.ViewBehavior.AllowPerPixelScrolling ? DataControlScrollMode.RowPixel : DataControlScrollMode.Item; 
			}
		}
		protected override void OnMouseWheelDown() {
			if(!CanScrollDown())
				ScrollInfo.MouseWheelRight();
			else
				ScrollInfo.MouseWheelDown();
		}
		protected override void OnMouseWheelUp() {
			if(!CanScrollUp())
				ScrollInfo.MouseWheelLeft();
			else
				ScrollInfo.MouseWheelUp();
		}
		protected internal bool CanScrollDown() {
			return ScrollInfoCore.VerticalScrollInfo.Extent > ScrollInfoCore.VerticalScrollInfo.Viewport;
		}
		protected internal bool CanScrollUp() {
			return ScrollInfoCore.VerticalScrollInfo.Extent > ScrollInfoCore.VerticalScrollInfo.Viewport;
		}
	}
}
