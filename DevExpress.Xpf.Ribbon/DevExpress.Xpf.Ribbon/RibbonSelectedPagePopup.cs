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
using DevExpress.Xpf.Core;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Ribbon.Themes;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonSelectedPagePopup : RibbonPopupBase {
		#region static
		static RibbonSelectedPagePopup() {
		}
		#endregion
		#region props
		private RibbonControl ribbonCore = null;
		public RibbonControl Ribbon {
			get { return ribbonCore; }
			set {
				if(ribbonCore == value)
					return;
				RibbonControl oldValue = ribbonCore;
				ribbonCore = value;
				OnRibbonChanged(oldValue);
			}
		}
		#endregion
		public RibbonSelectedPagePopup() {
			ItemClickBehaviour = PopupItemClickBehaviour.CloseChildren;
			IsBranchHeader = true;
		}
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			UpdatePopupContent();
		}
		protected override PopupBorderControl CreateBorderControl() {
			return new RibbonSelectedPagePopupBorderControl();
		}
		protected override object CreatePopupContent() {
			return new RibbonSelectedPageContentPresenter();
		}
		protected internal virtual void RecreateEditors() {
			RibbonSelectedPageControl ctrl = PopupContent as RibbonSelectedPageControl;
			if(ctrl != null)
				ctrl.RecreateEditors();
		}
		protected virtual void UpdatePopupContent() {
			RibbonSelectedPageControl ctrl = PopupContent as RibbonSelectedPageControl;
			if(ctrl == null) return;
			ctrl.Ribbon = Ribbon;
			RibbonSelectedPagePopupBorderControl border = Child as RibbonSelectedPagePopupBorderControl;
			if(border != null) border.Ribbon = Ribbon;
		}		
		protected override void UpdatePlacement(System.Windows.UIElement control) {
			Placement = PlacementMode.Bottom;
		}
		protected override object OnIsOpenCoerce(object value) {
			if(Ribbon == null) return base.OnIsOpenCoerce(value);
			if((bool)value == false) {
				if(Ribbon.popupIsAlwaysOpen == true) {
					Ribbon.popupIsAlwaysOpen = false;
					return true;
				}				
			}
			return base.OnIsOpenCoerce(value);
		}
		protected override void OnClosed(System.EventArgs e) {
			base.OnClosed(e);
			Ribbon.IsMinimizedRibbonCollapsed = true;
		}
		internal bool wasOpened = false;
		protected override void OnOpened(System.EventArgs e) {
			base.OnOpened(e);
			wasOpened = true;
			if((PopupContent as RibbonSelectedPageControl) != null) {
				((RibbonSelectedPageControl)PopupContent).Reset();
			}
			Ribbon.IsMinimizedRibbonCollapsed = false;
		}
	}
	public class RibbonSelectedPagePopupBorderControl : RibbonPopupBorderControl, IToolTipPlacementTarget {
	   #region static
		public static readonly DependencyProperty ToolTipVerticalOffsetProperty = DependencyProperty.Register("ToolTipVerticalOffset", typeof(double), typeof(RibbonSelectedPagePopupBorderControl), new PropertyMetadata(0d));
		#endregion
		#region dep props
		public double ToolTipVerticalOffset {
			get { return (double)GetValue(ToolTipVerticalOffsetProperty); }
			set { SetValue(ToolTipVerticalOffsetProperty, value); }
		}
		#endregion
		public RibbonSelectedPagePopupBorderControl() {
			DefaultStyleKey = typeof(RibbonSelectedPagePopupBorderControl);
		}	  
		internal RibbonControl Ribbon { get; set; }
		#region IToolTipPlacementTarget
		DependencyObject IToolTipPlacementTarget.ExternalPlacementTarget {
			get { return this; }
		}
		double IToolTipPlacementTarget.HorizontalOffset {
			get { return 0; }
		}
		BarItemLinkControlToolTipHorizontalPlacement IToolTipPlacementTarget.HorizontalPlacement {
			get { return BarItemLinkControlToolTipHorizontalPlacement.RightAtTargetLeft; }
		}
		BarItemLinkControlToolTipPlacementTargetType IToolTipPlacementTarget.HorizontalPlacementTargetType {
			get { return BarItemLinkControlToolTipPlacementTargetType.Internal; }
		}
		double IToolTipPlacementTarget.VerticalOffset {
			get { return ToolTipVerticalOffset; }
		}
		BarItemLinkControlToolTipVerticalPlacement IToolTipPlacementTarget.VerticalPlacement {
			get { return BarItemLinkControlToolTipVerticalPlacement.BottomAtTargetBottom; }
		}
		BarItemLinkControlToolTipPlacementTargetType IToolTipPlacementTarget.VerticalPlacementTargetType {
			get { return BarItemLinkControlToolTipPlacementTargetType.External; }
		}
		#endregion
	}
}
