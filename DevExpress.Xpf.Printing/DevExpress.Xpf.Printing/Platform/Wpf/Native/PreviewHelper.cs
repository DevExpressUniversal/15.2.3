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
using System.Windows;
using System.Windows.Interop;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Printing.Native {
#if DEBUGTEST
	public  
#endif
	abstract class PreviewHelper {
		protected IDocumentPreviewModel PreviewModel { get; set; }
		protected abstract object DocumentSource { get; }
		public FloatingContainer ShowDocumentPreview(FrameworkElement owner, string title) {
			Guard.ArgumentNotNull(owner, "owner");
			return ShowFloatingDocumentPreview(owner, title, Xpf.DocumentViewer.CommandBarStyle.Bars);
		}
		public FloatingContainer ShowRibbonDocumentPreview(FrameworkElement owner, string title) {
			Guard.ArgumentNotNull(owner, "owner");
			return ShowFloatingDocumentPreview(owner, title, Xpf.DocumentViewer.CommandBarStyle.Ribbon);
		}
		FloatingContainer ShowFloatingDocumentPreview(FrameworkElement owner, string title, DevExpress.Xpf.DocumentViewer.CommandBarStyle commandBarStyle) {
			DocumentPreviewControl preview = new DocumentPreviewControl();
			preview.DocumentSource = DocumentSource;
			preview.CommandBarStyle = commandBarStyle;
			FloatingContainer container = FloatingContainerFactory.Create(FloatingMode.Window);
			container.Owner = owner;
			container.Content = preview;
			Thickness margin = new Thickness(30);
			container.FloatLocation = new Point(margin.Left, margin.Top);
			container.FloatSize = new Size(owner.ActualWidth - (margin.Left + margin.Right), owner.ActualHeight - (margin.Top + margin.Bottom));
			container.Caption = title ?? PrintingLocalizer.GetString(PrintingStringId.PrintPreviewWindowCaption);
			container.Hidden += new RoutedEventHandler(FloatingDocumentPreview_Hidden);
			LogicalTreeIntruder.AddLogicalChild(owner, container);
			CreateDocumentIfEmpty();
			container.IsOpen = true;
			return container;
		}
		void FloatingDocumentPreview_Hidden(object sender, RoutedEventArgs e) {
			FloatingContainer container = (FloatingContainer)sender;
			container.Hidden -= FloatingDocumentPreview_Hidden;
			LogicalTreeIntruder.RemoveLogicalChild(container);
			StopPageBuilding();
		}
		public Window ShowDocumentPreview(Window owner, string title) {
			Window preview = CreateDocumentPreviewWindow(owner, title);
			CreateDocumentIfEmpty();
			preview.Show();
			return preview;
		}
		public Window ShowRibbonDocumentPreview(Window owner, string title) {
			Window preview = CreateRibbonDocumentPreviewWindow(owner, title);
			CreateDocumentIfEmpty();
			preview.Show();
			return preview;
		}
		public void ShowDocumentPreviewDialog(Window owner, string title) {
			Window preview = CreateDocumentPreviewWindow(owner, title);
			CreateDocumentIfEmpty();
			preview.ShowDialog();
			StopPageBuilding();
		}
		public void ShowRibbonDocumentPreviewDialog(Window owner, string title) {
			Window preview = CreateRibbonDocumentPreviewWindow(owner, title);
			CreateDocumentIfEmpty();
			preview.ShowDialog();
			StopPageBuilding();
		}
		internal Window CreateDocumentPreviewWindow(Window owner, string title) {
			return CreatePreviewWindow(owner, title, Xpf.DocumentViewer.CommandBarStyle.Bars);
		}
		internal Window CreateRibbonDocumentPreviewWindow(Window owner, string title) {
			return CreatePreviewWindow(owner, title, Xpf.DocumentViewer.CommandBarStyle.Ribbon);
		}
		DXWindow CreatePreviewWindow(Window owner, string title, Xpf.DocumentViewer.CommandBarStyle commandBarStyle) {
			DocumentPreviewControl preview = new DocumentPreviewControl();
			preview.DocumentSource = DocumentSource;
			preview.CommandBarStyle = commandBarStyle;
			DXWindow window = new DXWindow();
			window.Owner = owner;
			window.Content = preview;
			window.Title = title ?? PrintingLocalizer.GetString(PrintingStringId.PrintPreviewWindowCaption);
			window.Closed += (s, e) => StopPageBuilding();
			return window;
		}
		protected abstract void CreateDocumentIfEmpty();
		protected abstract void StopPageBuilding();
	}
}
