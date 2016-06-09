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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.Controls.Primitives;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class SlideViewItemHeader : veContentControl, IControl, IClickableControl {
		public static readonly DependencyProperty IsHeaderInteractiveProperty;
		public static readonly DependencyProperty InteractiveHeaderTemplateProperty;
		public static readonly DependencyProperty IsStickyProperty;
		static SlideViewItemHeader() {
			var dProp = new DependencyPropertyRegistrator<SlideViewItemHeader>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("IsHeaderInteractive", ref IsHeaderInteractiveProperty, false,
				(d, e) => ((SlideViewItemHeader)d).OnIsInteractiveChanged((bool)e.NewValue));
			dProp.Register("InteractiveHeaderTemplate", ref InteractiveHeaderTemplateProperty, (DataTemplate)null);
			dProp.Register("IsSticky", ref IsStickyProperty, true);
		}
		public SlideViewItemHeader() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(SlideViewItemHeader);
#else
			ManipulationStarting += SlideViewItemHeader_ManipulationStarting;
#endif
			Controller = GetController();
		}
#if !SILVERLIGHT
		void SlideViewItemHeader_ManipulationStarting(object sender, System.Windows.Input.ManipulationStartingEventArgs e) {
			e.Cancel();
		}
#endif
		SlideViewHeaderPanel headerPanel;
		protected override void OnApplyTemplateComplete() {
			headerPanel = GetTemplateChild("PART_HeaderPanel") as SlideViewHeaderPanel;
			if (Controller is SlideViewItemHeaderController)
				((SlideViewItemHeaderController)Controller).UpdateState(false);
		}
		protected virtual void OnIsInteractiveChanged(bool newValue) {
			this.GoToState((bool)newValue ? "Interactive" : "Noninteractive");
		}
		internal void SetRelativeLocation(double scrollOffset, double headerOffset) {
			if(headerPanel == null) return;
			headerPanel.SetRelativeLocation(IsSticky ? scrollOffset : 0, headerOffset);
		}
		protected virtual Controller GetController() {
			return new SlideViewItemHeaderController(this);
		}
		protected void OnClick() {
			if (Owner is IClickableControl) {
				((IClickableControl)Owner).OnClick();
			}
			if (Click != null) Click(this, System.EventArgs.Empty);
		}
		protected internal SlideViewItem Owner { get; internal set; }
		public bool IsHeaderInteractive {
			get { return (bool)GetValue(IsHeaderInteractiveProperty); }
			set { SetValue(IsHeaderInteractiveProperty, value); }
		}
		public bool IsSticky {
			get { return (bool)GetValue(IsStickyProperty); }
			set { SetValue(IsStickyProperty, value); }
		}
		public DataTemplate InteractiveHeaderTemplate {
			get { return (DataTemplate)GetValue(InteractiveHeaderTemplateProperty); }
			set { SetValue(InteractiveHeaderTemplateProperty, value); }
		}
		#region IControl Members
		public FrameworkElement Control {
			get { return this; }
		}
		public Controller Controller { get; private set; }
		#endregion
		#region IClickableControl Members
		void IClickableControl.OnClick() {
			OnClick();
		}
		#endregion
		#region IClickable Members
		public event EventHandler Click;
		#endregion
		public class SlideViewItemHeaderController : ControlControllerBase {
			public SlideViewItemHeaderController(IControl control) : base(control) { }
			SlideViewItemHeader ClickableControl { get { return Control as SlideViewItemHeader; } }
			#region Keyboard and Mouse Handling
			protected override void OnMouseLeftButtonUp(DXMouseButtonEventArgs e) {
				bool isClick = IsMouseLeftButtonDown;
				if(isClick) {
					ClickableControl.OnClick();
					e.Handled = true;
				}
				base.OnMouseLeftButtonUp(e);
			}
			public override void UpdateState(bool useTransitions) {
				if(ClickableControl == null || ClickableControl.Owner == null || (ClickableControl.Owner.IsHeaderInteractive || !ClickableControl.IsEnabled)) {
					base.UpdateState(useTransitions);
				}
				if (ClickableControl != null)
					ClickableControl.GoToState((bool)ClickableControl.IsHeaderInteractive ? "Interactive" : "Noninteractive");
			}
			#endregion Keyboard and Mouse Handling
		}
	}
}
