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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars;
using System.Windows.Controls.Primitives;
using System.Windows;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageGroupPopup : BarPopupBase {
		#region static
		public static readonly DependencyProperty PageGroupProperty;
		public static readonly DependencyProperty OwnerPageGroupControlProperty;
		static RibbonPageGroupPopup() {
			PageGroupProperty = DependencyPropertyManager.Register("PageGroup", typeof(RibbonPageGroup), typeof(RibbonPageGroupPopup),
				new PropertyMetadata(null, OnPageGroupPropertyChanged));
			OwnerPageGroupControlProperty = DependencyPropertyManager.Register("OwnerPageGroupControl", typeof(RibbonPageGroupControl), typeof(RibbonPageGroupPopup),
				new PropertyMetadata(null, OnOwnerPageGroupControlPropertyChanged));
		}
		static void OnPageGroupPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupPopup)obj).OnPageGroupChanged(e.OldValue as RibbonPageGroup);
		}
		static void OnOwnerPageGroupControlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupPopup)obj).OnOwnerPageGroupControlPropertyChanged(e.OldValue as RibbonPageGroupControl);
		}
		#endregion
		public RibbonPageGroupPopup() {
			IsBranchHeader = true;
		}
		protected override bool CanShowToolTip() { return true; }
		public RibbonPageGroupControl Content { get { return PopupContent as RibbonPageGroupControl; } }
		protected virtual void OnPageGroupChanged(RibbonPageGroup oldValue) {
			UpdatePopupContent();
		}
		private void OnOwnerPageGroupControlPropertyChanged(RibbonPageGroupControl ribbonPageGroupControl) {
			UpdatePopupContent();
		}
		public RibbonPageGroup PageGroup {
			get { return (RibbonPageGroup)GetValue(PageGroupProperty); }
			set { SetValue(PageGroupProperty, value); }
		}
		public RibbonPageGroupControl OwnerPageGroupControl {
			get { return (RibbonPageGroupControl)GetValue(OwnerPageGroupControlProperty); }
			set { SetValue(OwnerPageGroupControlProperty, value); }
		}
		protected override PopupBorderControl CreateBorderControl() {
			return new RibbonPageGroupPopupBorderControl();
		}
		protected override object CreatePopupContent() {
			RibbonPageGroupControl ctrl = new RibbonPageGroupControl();
			return ctrl;
		}
		protected virtual void UpdatePopupContent() {
			RibbonPageGroupControl ctrl = PopupContent as RibbonPageGroupControl;
			if(ctrl == null) return;
			ctrl.PageGroup = PageGroup;
			ctrl.Owner = OwnerPageGroupControl;
			if(OwnerPageGroupControl != null)
				ctrl.Ribbon = OwnerPageGroupControl.Ribbon;
		}		
		protected override void UpdatePlacement(System.Windows.UIElement control) {
			Placement = PlacementMode.Bottom; 
		}		
		protected override void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsOpenChanged(e);
			if(OwnerPageGroupControl != null && !OwnerPageGroupControl.IsMouseOver && (bool)e.NewValue == false) {
				OwnerPageGroupControl.ClosePopup();
			}
		}
	}
}
