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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Documents;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.ExpressionEditor.Native;
using DevExpress.Xpf.Editors.Filtering;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
#if !SL
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
#endif
namespace DevExpress.Xpf.Grid.Native {
	public class CardViewBehavior : DataViewBehavior {
		DispatcherTimer scrollTimer;
		protected internal override DispatcherTimer ScrollTimer {
			get {
				return scrollTimer;
			}
		}
#if DEBUGTEST
		public
#endif
			static UseCardLightweightTemplates? DefaultUseLightweightTemplates = null;
		CardView CardView { get { return (CardView)View; } }
		public CardViewBehavior(DataViewBase view)
			: base(view) {
			scrollTimer = new DispatcherTimer();
			scrollTimer.Interval = TimeSpan.FromMilliseconds(10);
			scrollTimer.Tick += new EventHandler(OnScrollTimer_Tick);
		}
		internal override double HorizontalViewportCore { get { return CardView.CardsPanelViewPort; } }
		protected internal override bool AllowPerPixelScrolling { get { return CardView.ScrollMode == ScrollMode.Pixel; } }
		protected internal override RowData CreateRowDataCore(DataTreeBuilder treeBuilder, bool updateDataOnly) {
			return new CardData(treeBuilder);
		}
		protected internal override void OnTopRowIndexChangedCore() {
			switch(CardView.CardLayout) {
				case CardLayout.Rows:
					View.DataPresenter.SetVerticalOffsetForce(View.TopRowIndex);
					break;
				case CardLayout.Columns:
					View.DataPresenter.SetHorizontalOffsetForce(View.TopRowIndex);
					break;
			}
		}
		protected internal override bool OnVisibleColumnsAssigned(bool changed) {
			return changed;
		}
		protected internal override void UpdateCellData(ColumnsRowDataBase rowData) {
			rowData.ReuseCellDataNotVirtualized(x => x.CellData, (x, val) => x.CellData = (IList<GridColumnData>)val, View.VisibleColumnsCore);
		}
		internal override void UpdateSecondaryScrollInfoCore(double secondaryOffset, bool allowUpdateViewportVisibleColumns) {
			Point offset = SizeHelperBase.GetDefineSizeHelper(CardView.Orientation).CreatePoint(0, secondaryOffset);
			View.DataPresenter.ContentElement.Margin = new Thickness(offset.X, offset.Y, 0, 0);
		}
		internal override GridViewNavigationBase CreateRowNavigation() {
			return new CardViewRowNavigation(CardView);
		}
		internal override GridViewNavigationBase CreateCellNavigation() {
			return new CardViewCellNavigation(CardView);
		}
		protected internal override double GetFixedExtent() { return CardView.CardsPanelMaxSize; }
		#region CopyRows
		protected internal override void GetDataRowText(StringBuilder sb, int rowHandle) {
			CardView.GetDataRowTextCore(sb, rowHandle);
		}
		#endregion
		#region LightweightTemplates
		internal static DependencyProperty RegisterUseLightweightTemplatesProperty(Type ownerType) {
			return DependencyProperty.Register("UseLightweightTemplates", typeof(UseCardLightweightTemplates?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CardViewBehavior)((CardView)d).ViewBehavior).OnUseLightweightTemplatesChanged()));
		}
		UseCardLightweightTemplates ActualUseLightweightTemplates {
			get {
				return CardView.UseLightweightTemplates ?? ((CardView)View.RootView).UseLightweightTemplates ?? DefaultUseLightweightTemplates ?? UseCardLightweightTemplates.All; 
			}
		}
		internal bool canChangeUseLightweightTemplates = true;
		void ForbidChangeUseLightweightTemplatesProperty() {
			canChangeUseLightweightTemplates = false;
		}
		internal bool UseLightweightTemplatesHasFlag(UseCardLightweightTemplates flag) { return ActualUseLightweightTemplates.HasFlag(flag); }
		internal FrameworkElement CreateElement(Func<FrameworkElement> lightweightDelegate, Func<FrameworkElement> ordinaryDelegate, UseCardLightweightTemplates flag) {
			if(canChangeUseLightweightTemplates) {
				ForbidChangeUseLightweightTemplatesProperty();
			}
			if(UseLightweightTemplatesHasFlag(flag))
				return lightweightDelegate();
			else
				return ordinaryDelegate();
		}
		void OnUseLightweightTemplatesChanged() {
			if(!canChangeUseLightweightTemplates && !DataViewBase.DisableOptimizedModeVerification)
				throw new InvalidOperationException("Can't change the UseLightweightTemplates property after the GridControl has been initialized.");
			View.UpdateColumnsAppearance();
		}
		#endregion
		public virtual void OnScrollTimer_Tick(object sender, EventArgs e) {
			if(View == null || View.ScrollContentPresenter == null)
				return;
			if(LastMousePosition.X == double.NaN || LastMousePosition.Y == double.NaN)
				return;
			double vScrollDelta = 0, hScrollDelta = 0;
			DragScroll();
			if(MouseMoveSelection.CanScrollVertically) {
				if(LastMousePosition.Y < 0)
					vScrollDelta = -0.2;
				if(LastMousePosition.Y > View.ScrollContentPresenter.ActualHeight)
					vScrollDelta = 0.2;
			}
			if(MouseMoveSelection.CanScrollHorizontally) {
				if(LastMousePosition.X < 0)
					hScrollDelta = -0.2;
				if(LastMousePosition.X > View.ScrollContentPresenter.ActualWidth)
					hScrollDelta = 0.2;
			}
			if(hScrollDelta != 0)
				ChangeHorizontalOffsetBy(hScrollDelta);
			if(vScrollDelta != 0)
				ChangeVerticalOffsetBy(vScrollDelta);
			if(vScrollDelta != 0 || hScrollDelta != 0)
				View.EnqueueImmediateAction(() => MouseMoveSelection.UpdateSelection(View));
		}
		void DragScroll() {
			if(!DragDropScroller.IsDragging(View))
				return;
			if(LastMousePosition.X < 0)
				ChangeHorizontalOffsetBy(-1);
			if(LastMousePosition.X > View.ScrollContentPresenter.ActualWidth)
				ChangeHorizontalOffsetBy(1);
		}		
		MouseMoveSelectionCardBase mouseMoveSelection;
		MouseMoveSelectionCardBase MouseMoveSelection {
			get {return mouseMoveSelection ?? MouseMoveSelectionCardNone.Instance;}
			set {mouseMoveSelection = value;}
		}
		internal override void OnViewMouseLeave() {
			MouseMoveSelection.CaptureMouse(View);
		}
		internal override void OnViewMouseMove(MouseEventArgs e) {
			LastMousePosition = e.GetPosition(View.ScrollContentPresenter);
			MouseMoveSelection.UpdateSelection(View);
		}	
		internal override void ProcessMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.ProcessMouseLeftButtonUp(e);
			StopSelection();
		}
		internal override void OnMouseLeftButtonUp() {
			StopSelection();
		}
		internal override void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo) {
			base.OnAfterMouseLeftButtonDown(hitInfo);
			if(!View.IsEditing && Mouse.RightButton == MouseButtonState.Released) {
				MouseMoveSelection = GetMouseMoveSelection(hitInfo);
				MouseMoveSelection.OnMouseDown(View, hitInfo);
			}
		}
		protected internal override void StopSelection() {
			MouseMoveSelection.OnMouseUp(View);
			MouseMoveSelection = null;
			MouseMoveSelection.ReleaseMouseCapture(View);
		}
		MouseMoveSelectionCardBase GetMouseMoveSelection(IDataViewHitInfo hitInfo) {
			if(!View.AllowMouseMoveSelection)
				return MouseMoveSelectionCardNone.Instance;
			if(View.ShowSelectionRectangle) {
				CardViewHitInfo cardViewHitInfo = (CardViewHitInfo)hitInfo;
				if(IsRowModeSelection && (cardViewHitInfo.HitTest == CardViewHitTest.DataArea || hitInfo.InRow))
					return MouseMoveSelectionRectangleCard.Instance;
			}
			return MouseMoveSelectionCardNone.Instance;
		}
		bool IsRowModeSelection {
			get {
				return View.DataControl.SelectionMode == MultiSelectMode.Row;
			}
		}
	}   
}
