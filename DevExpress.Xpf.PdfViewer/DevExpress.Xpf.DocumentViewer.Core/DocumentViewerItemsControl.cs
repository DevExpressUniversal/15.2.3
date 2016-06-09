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

using DevExpress.Xpf.Core.Native;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.DocumentViewer {
	public class DocumentViewerItemsControl : ItemsControl {
		protected internal ItemsPresenter ItemsPresenter { get; private set; }
		protected internal ScrollViewer ScrollViewer { get; private set; }
		protected internal DocumentViewerPanel Panel { get; private set; }
		public DocumentViewerItemsControl() {
			DefaultStyleKey = typeof(DocumentViewerItemsControl);
			Loaded += OnLoaded;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			Panel = VisualTreeHelper.GetChild(ItemsPresenter, 0) as DocumentViewerPanel;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new PageControl();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			var page = item as PageWrapper;
			if (page == null)
				return;
			((PageControl)element).DataContext = page;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			((PageControl)element).DataContext = null;
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is PageControl;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ItemsPresenter = (ItemsPresenter)GetTemplateChild("PART_ItemsPresenter");
			ScrollViewer = (ScrollViewer)GetTemplateChild("PART_ScrollViewer");
		}
	}
}
