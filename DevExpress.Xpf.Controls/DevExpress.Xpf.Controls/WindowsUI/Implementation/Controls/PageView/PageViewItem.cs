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
using System.Windows.Data;
using System;
using System.Windows.Media;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.UIAutomation;
namespace DevExpress.Xpf.WindowsUI {
	public class PageViewItem : veHeaderedContentSelectorItem, IControl {
		#region static
		static PageViewItem() {
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PageViewItem), new FrameworkPropertyMetadata(typeof(PageViewItem)));
#endif
		}
		#endregion
		public PageViewItem() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(PageViewItem);
#endif
			Controller = CreateController();
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			Controller.UpdateState(false);
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			base.OnIsSelectedChanged(isSelected);
			Controller.UpdateState(false);
			Panel parentPanel = LayoutHelper.FindParentObject<Panel>(this);
			if(parentPanel != null) {
				parentPanel.InvalidateMeasure();
			}
		}
		protected virtual ControlControllerBase CreateController() {
			return new PageViewItemController(this);
		}
		public ControlControllerBase Controller { get; private set; }
		protected internal override ClickMode SelectionMode {
			get { return ClickMode.Release; }
		}
		#region IControl Members
		FrameworkElement IControl.Control { get { return this; } }
		Controller IControl.Controller { get { return Controller; } }
		#endregion
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new PageViewItemAutomationPeer(this);
		}
		#endregion
		public class PageViewItemController : ControlControllerBase {
			public PageViewItemController(PageViewItem control)
				: base(control) {
			}
			public new PageViewItem Control { get { return (PageViewItem)base.Control; } }
			public override void UpdateState(bool useTransitions) {
				base.UpdateState(useTransitions);
				string stateName = "EmptySelectedState";
				VisualStateManager.GoToState(Control, stateName, useTransitions);
				if(Control.IsSelected)
					stateName = "Selected";
				else
					stateName = "Unselected";
				VisualStateManager.GoToState(Control, stateName, useTransitions);
			}
			protected override void OnMouseLeftButtonUp(DevExpress.Xpf.Core.DXMouseButtonEventArgs e) {
				var isClick = IsMouseLeftButtonDown && IsMouseEntered;
				base.OnMouseLeftButtonUp(e);
				if(isClick && Control.IsMouseOver) {
					Control.InvokeSelectInParentContainer();
				}
			}
		}
	}
}
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class PageViewItemHeaderPresenter : veContentControl {
	}
}
