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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Native;
using System.Windows.Data;
namespace DevExpress.Xpf.Diagram.Native {
	public sealed class DiagramPopupToolBar : ToolBarControl {
		public static readonly DependencyProperty PlacementTargetProperty;
		public static readonly DependencyProperty HorizontalOffsetProperty;
		static DiagramPopupToolBar() {
			DependencyPropertyRegistrator<DiagramPopupToolBar>.New()
				.Register(d => d.PlacementTarget, out PlacementTargetProperty, null)
				.Register(d => d.HorizontalOffset, out HorizontalOffsetProperty, 0.0)
			;
		}
		readonly Func<IEnumerable<IBarManagerControllerAction>> getToolBarItems;
		public bool IsOpen {
			get { return PopupContainer.IsOpen; }
			set { PopupContainer.IsOpen = value; }
		}
		public UIElement PlacementTarget {
			get { return (UIElement)GetValue(PlacementTargetProperty); }
			set { SetValue(PlacementTargetProperty, value); }
		}
		public double HorizontalOffset {
			get { return (double)GetValue(HorizontalOffsetProperty); }
			set { SetValue(HorizontalOffsetProperty, value); }
		}
		public event EventHandler Opened;
		PopupControlContainer PopupContainer { get { return popupContainer.Value; } }
		public DiagramPopupToolBar(Func<IEnumerable<IBarManagerControllerAction>> getToolBarItems) {
			this.getToolBarItems = getToolBarItems;
			IsMultiLine = true;
			AllowQuickCustomization = false;
			AllowCustomizationMenu = false;
			popupContainer = new Lazy<PopupControlContainer>(() => {
				var result = new PopupControlContainer();
				InitializePopupContainer(result);
				return result;
			});
		}
		void InitializePopupContainer(PopupControlContainer popupContainer) {
			popupContainer.Content = this;
			popupContainer.SetBinding(PopupControlContainer.PlacementTargetProperty, new Binding(PlacementTargetProperty.Name) { Source = this });
			popupContainer.SetBinding(PopupControlContainer.HorizontalOffsetProperty, new Binding(HorizontalOffsetProperty.Name) { Source = this });
			popupContainer.Placement = SystemParameters.MenuDropAlignment ? PlacementMode.Left : PlacementMode.Right;
			popupContainer.Opening += OnPopupContainerOpening;
			popupContainer.Opened += OnPopupContainerOpened;
			popupContainer.Closed += OnPopupContainerClosed;
		}
		void OnPopupContainerOpening(object sender, System.ComponentModel.CancelEventArgs e) {
			Items.Clear();
			getToolBarItems().ForEach(x => x.Execute(this));
		}
		void OnPopupContainerClosed(object sender, EventArgs e) {
			Items.Clear();
		}
		void OnPopupContainerOpened(object sender, EventArgs e) {
			PopupContainer.VerticalOffset = Mouse.GetPosition(PlacementTarget).Y - PopupContainer.Child.RenderSize.Height;
			if(Opened != null)
				Opened(this, e);
		}
		Lazy<PopupControlContainer> popupContainer;
	}
}
