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

using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.DemoData.DemoParts;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using DevExpress.Xpf.Office.UI;
namespace DevExpress.Xpf.DemoBase.Internal {
	public partial class DemoBaseControlProductsPageView : DemoBaseControlPartView {
		public bool ShowFooter {
			get { return (bool)GetValue(ShowFooterProperty); }
			set { SetValue(ShowFooterProperty, value); }
		}
		public static readonly DependencyProperty ShowFooterProperty =
			DependencyProperty.Register("ShowFooter", typeof(bool), typeof(DemoBaseControlProductsPageView), new PropertyMetadata(true));
		public DataTemplate CustomViewTemplate {
			get { return (DataTemplate)GetValue(CustomViewTemplateProperty); }
			set { SetValue(CustomViewTemplateProperty, value); }
		}
		public static readonly DependencyProperty CustomViewTemplateProperty =
			DependencyProperty.Register("CustomViewTemplate", typeof(DataTemplate), typeof(DemoBaseControlProductsPageView), new PropertyMetadata(null));
		public DataTemplate CustomDemoTitleTemplate {
			get { return (DataTemplate)GetValue(CustomDemoTitleTemplateProperty); }
			set { SetValue(CustomDemoTitleTemplateProperty, value); }
		}
		public static readonly DependencyProperty CustomDemoTitleTemplateProperty =
			DependencyProperty.Register("CustomDemoTitleTemplate", typeof(DataTemplate), typeof(DemoBaseControlProductsPageView), new PropertyMetadata(null));
		public DemoBaseControlProductsPageView() {
			InitializeComponent();
			Loaded += DemoBaseControlProductsPageView_Loaded;
		}
		void DemoBaseControlProductsPageView_Loaded(object sender, RoutedEventArgs e) {
			DemoWindow.CtrlFClicked += (o, args) => filter.FocusEditor();
		}
		DemoWindow DemoWindow {
			get { return this.VisualParents().OfType<DemoWindow>().FirstOrDefault(); }
		}
		void OnClearFilterFocus(object sender, EventArgs e) {
			if (DemoWindow != null) {
				FocusManager.SetFocusedElement(DemoWindow, DemoWindow);
			}
		}
		void HoverAdorner_FullListLayout_MouseUp(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left) {
				isFullListLayout.IsChecked = true;
			}
		}
		void HoverAdorner_ProductLayout_MouseUp(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left) {
				isProductsLayout.IsChecked = true;
			}
		}
		void demoLinks_FilterStringChanged(object sender, EventArgs e) {
			isFullListLayout.IsChecked = true;
			scrollControl.NavigateToPage(0);
		}
	}
	public interface IGroupedLinksControlService {
		void Disable();
		void Enable();
	}
	public class GroupedLinksControlService : ServiceBase, IGroupedLinksControlService {
		GroupedLinksControl Control { get { return (GroupedLinksControl)AssociatedObject; } }
		public void Disable() {
			if (Control == null)
				return;
			Control.HideFlyout();
			Control.IsHitTestVisible = false;
		}
		public void Enable() {
			Control.Do(c => c.IsHitTestVisible = true);
		}
	}
}
