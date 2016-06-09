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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010 {
	public class WindowsUIButtonsPanel : ButtonsPanel, IButtonsPanel {
		public WindowsUIButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new WindowsUIButtonsPanelViewInfo(this);
		}
		protected override IButtonsPanelHandler CreateHandler() {
			return new WindowsUIButtonsPanelHandler(this);
		}
		public void Merge(WindowsUIButtonPanel buttonPanel) {
			Buttons.BeginUpdate();
			foreach(IBaseButton button in buttonPanel.Buttons) {
				if(!Buttons.Contains(button)) {
					button.SetMerged(this);
					Buttons.Add(button);
				}
			}
			Buttons.EndUpdate();
		}
		public void Unmerge() {
			Buttons.BeginUpdate();
			for(int i = Buttons.Count - 1; i >= 0; i--) {
				WindowsUIButton button = Buttons[i] as WindowsUIButton;
				WindowsUISeparator separator = Buttons[i] as WindowsUISeparator;
				if(button != null && button.NativeOwner != this) {
					Buttons.Remove(button);
					button.SetMerged(null);
				}
				else if(separator != null && separator.NativeOwner != this) {
					Buttons.Remove(separator);
					separator.SetMerged(null);
				}
			}
			Buttons.EndUpdate();
		}
		ContentAlignment contentAlignmentCore = ContentAlignment.MiddleCenter;
		[DefaultValue(ContentAlignment.MiddleRight), Category("Layout")]
		public new ContentAlignment ContentAlignment {
			get { return contentAlignmentCore; }
			set { SetValue(ref contentAlignmentCore, value, "ContentAlignment"); }
		}
	}
	public class WindowsUIButtonsPanelHandler : ButtonsPanelHandler {
		WindowsUIButtonPanel windowsUIButtonPanel;
		public WindowsUIButtonsPanelHandler(IButtonsPanel panel) : base(panel) {
			windowsUIButtonPanel = panel.Owner as WindowsUIButtonPanel;
		}
		protected bool CausesValidation { 
			get {
				if(Panel.Owner is Control)
					return (Panel.Owner as Control).CausesValidation;
				return false;
			}
		}
		protected ContainerControl ValidationContainer {
			get { return GetValidationContainer(); }
		}
		protected virtual ContainerControl GetValidationContainer(){
			if(Panel.Owner is Control) {
				Control parent = (Panel.Owner as Control).Parent;
				while(parent != null){
					if(parent is ContainerControl)
						return parent as ContainerControl;
					else
						parent = parent.Parent;
				}
			}
			return null;
		}
		protected override void SetHotState(ref BaseButtonInfo info, BaseButtonInfo value) {
			if(info == value) return;
			if(info != null) { StopHoverPeekTimer(); }
			if(value != null) { StartHoverPeekTimer(); }
			base.SetHotState(ref info, value);
		}
		Timer hoverPeekTimer;
		protected internal Timer HoverPeekTimer {
			get {
				if(hoverPeekTimer == null) {
					hoverPeekTimer = new Timer();
					hoverPeekTimer.Tick += hoverPeekTimer_Tick;
				}
				return hoverPeekTimer;
			}
		}
		void hoverPeekTimer_Tick(object sender, EventArgs e) {
			HoverPeekTimer.Stop();
			if(windowsUIButtonPanel != null && windowsUIButtonPanel.ShowPeekFormOnItemHover)
				windowsUIButtonPanel.ShowPeekForm(HotButton, HotInfo.Bounds);
		}
		internal void StopHoverPeekTimer() {
			HoverPeekTimer.Stop();
		}
		internal void StartHoverPeekTimer() {
			HoverPeekTimer.Interval = GetBeakFormShowInterval();
			HoverPeekTimer.Start();
		}
		const int defaultPeekFormShowDelay = 1500;
		int GetBeakFormShowInterval() {
			var panel =  Panel.Owner as WindowsUIButtonPanel;
			if(panel != null)
				return panel.PeekFormShowDelay;
			return defaultPeekFormShowDelay;
		}
		protected override void PerformClick(IBaseButton button) {
			if(CausesValidation && ValidationContainer != null)
				if(!ValidationContainer.ValidateChildren())
					return;
			if(button is WindowsUIButton)
				(button as WindowsUIButton).NativeOwner.PerformClick(button);
			else if(button is WindowsUISeparator)
				(button as WindowsUISeparator).NativeOwner.PerformClick(button);
			else
				base.PerformClick(button);
		}
	}
	public class CustomHeaderButtonsPanel : WindowsUIButtonsPanel {
		public CustomHeaderButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new CustomHeaderButtonsPanelViewInfo(this);
		}
		protected override BaseButtonCollection CreateButtons() {
			return new Views.WindowsUI.ContentContainerButtonCollection(this);
		}
	}
}
