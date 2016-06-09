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
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Utils;
using System.Windows.Interop;
#if !SL
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class RowViewBuilder {
		readonly IVisualTreeRoot visualTreeRoot;
		internal readonly StackPanel panel;
		internal readonly Dictionary<DataTemplate, ContentPresenter> presenterDictionary = new Dictionary<DataTemplate, ContentPresenter>();
		public RowViewBuilder() 
			: this(CreateVisualTreeRoot()) {
		}
		internal RowViewBuilder(IVisualTreeRoot visualTreeRoot) {
			Guard.ArgumentNotNull(visualTreeRoot, "visualTreeRoot");
			this.visualTreeRoot = visualTreeRoot;
			panel = new StackPanel();
			visualTreeRoot.Content = panel;
		}
		static IVisualTreeRoot CreateVisualTreeRoot() {
			return new PopupVisualTreeRoot();
		}
		public FrameworkElement Create(DataTemplate rowTemplate, RowContent rowContent) {
			Guard.ArgumentNotNull(rowTemplate, "rowTemplate");
			Guard.ArgumentNotNull(rowContent, "rowContent");
			visualTreeRoot.Active = true;
			ContentPresenter presenter;
			if(!presenterDictionary.TryGetValue(rowTemplate, out presenter)) {
				presenter = new ContentPresenter();
#if SL
				presenter.UseLayoutRounding = false;
#endif
				presenter.HorizontalAlignment = HorizontalAlignment.Center;
				presenter.VerticalAlignment = VerticalAlignment.Center;
				presenter.ContentTemplate = rowTemplate;
				presenter.Content = new RowContent();
				Viewbox viewbox = new Viewbox() { Child = presenter, Width = 0, Height = 0 };
				presenterDictionary.Add(rowTemplate, presenter);
				panel.Children.Add(viewbox);
			}
			RowContent presenterContent = (RowContent)presenter.Content;
			presenterContent.Content = rowContent.Content;
			presenterContent.UsablePageWidth = rowContent.UsablePageWidth;
			presenterContent.UsablePageHeight = rowContent.UsablePageHeight;
			presenterContent.IsEven = rowContent.IsEven;
			DevExpress.Xpf.Core.LayoutUpdatedHelper.GlobalLocker.DoLockedAction(presenter.UpdateLayout);
			return presenter;
		}
		public void Clear() {
			visualTreeRoot.Active = false;
			panel.Children.Clear();
			presenterDictionary.Clear();
		}
	}
	interface IVisualTreeRoot {
		bool Active { get; set; }
		UIElement Content { get; set; }
	}
	class PopupVisualTreeRoot : IVisualTreeRoot {
		readonly Popup popup = new Popup() { Width = 0, Height = 0 };
		public bool Active {
			get {
				return popup.IsOpen;
			}
			set {
				popup.IsOpen = value;
			}
		}
		public UIElement Content {
			get {
				return popup.Child;
			}
			set {
				popup.Child = value;
			}
		}
	}
#if !SL
	class FloatingContainerVisualTreeRoot : IVisualTreeRoot {
		readonly FloatingContainer container;
		public FloatingContainerVisualTreeRoot() {
			container = FloatingContainerFactory.Create(FloatingMode.Window);
			container.Owner = (FrameworkElement)Application.Current.MainWindow.Content;
			container.Width = 0;
			container.Height = 0;
			container.ContainerFocusable = false;
		}
		public bool Active {
			get {
				return container.IsOpen;
			}
			set {
				container.IsOpen = value;
			}
		}
		public UIElement Content {
			get {
				return (UIElement)container.Content;
			}
			set {
				container.Content = value;
			}
		}
	}
#endif
}
