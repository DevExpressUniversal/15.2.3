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
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using System.Windows;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageGroupsControl : ItemsControl {
		#region static        
		public static readonly DependencyProperty PageProperty;
		public RibbonPage Page {
			get { return (RibbonPage)GetValue(PageProperty); }
			set { SetValue(PageProperty, value); }
		}
		static RibbonPageGroupsControl() {			
			PageProperty = DependencyPropertyManager.Register("Page", typeof(RibbonPage), typeof(RibbonPageGroupsControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPagePropertyChanged)));
		}
		static protected void OnPagePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupsControl)o).OnPageChanged(e.OldValue as RibbonPage);
		}
		#endregion
		#region dep props
		#endregion
		#region props
		public RibbonPagesControl PagesControl {
			get { return ItemsControl.ItemsControlFromItemContainer(this) as RibbonPagesControl; }
		}	   
		#endregion
		public RibbonPageGroupsControl() {
			DefaultStyleKey = typeof(RibbonPageGroupsControl);
		}
		protected internal bool IsMeasured { get; private set; }
		protected internal bool IsArranged { get; private set; }
		public RibbonControl Ribbon { get { return PagesControl == null ? null : PagesControl.Ribbon; } }
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is RibbonPageGroupControl;
		}
		protected override System.Windows.DependencyObject GetContainerForItemOverride() {
			return new RibbonPageGroupControl();
		}
		protected override void PrepareContainerForItemOverride(System.Windows.DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			RibbonPageGroupControl pageGroupControl = (RibbonPageGroupControl)element;
			pageGroupControl.Ribbon = Ribbon;
			pageGroupControl.LinksHolder = (RibbonPageGroup)item;
			pageGroupControl.PageGroup = (RibbonPageGroup)item;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			RibbonPageGroupControl pageGroupControl = (RibbonPageGroupControl)element;
			pageGroupControl.Ribbon = null;
			pageGroupControl.LinksHolder = null;
			pageGroupControl.PageGroup = null;
		}
		protected virtual void OnPagesControlChanged(RibbonPagesControl oldValue) {
		}
		protected virtual void OnPageChanged(RibbonPage oldValue) {
			if(oldValue != null)
				oldValue.PageGroupsControls.Remove(this);
			if(Page != null)
				Page.PageGroupsControls.Add(this);
			if(oldValue != null) {
				ItemsSource = null;
			}
			ItemsSource = Page == null ? null : Page.ActualGroupsCore;
		}
		protected internal virtual void RecreateEditors() {
			for(int i = 0; i < Items.Count; i++) {
				RibbonPageGroupControl group = (RibbonPageGroupControl)ItemContainerGenerator.ContainerFromIndex(i);
				if(group == null)
					continue;
				group.RecreateEditors();
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			IsMeasured = true;
			return base.MeasureOverride(constraint);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			IsArranged = true;
			return base.ArrangeOverride(arrangeBounds);			
		}
	}
}
