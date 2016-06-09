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

using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010 {
	public class ButtonsPanel : BaseButtonsPanel {
		public ButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override void OnDispose() {
			ButtonClick = null;
			ButtonUnchecked = null;
			ButtonChecked = null;
			base.OnDispose();
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new ButtonsPanelViewInfo(this);
		}
		#region Events
		public event ButtonEventHandler ButtonClick;
		public event ButtonEventHandler ButtonUnchecked;
		public event ButtonEventHandler ButtonChecked;
		protected override void RaiseButtonClick(IBaseButton button) {
			if(button is IButton)
				RaiseButtonClick(button as IButton);
			if(button is Docking.CustomHeaderButton)
				((Docking.CustomHeaderButton)button).RaiseClick();
		}
		protected override void RaiseButtonChecked(IBaseButton button) {
			if(button is IButton)
				RaiseButtonChecked(button as IButton);
		}
		protected override void RaiseButtonUnchecked(IBaseButton button) {
			if(button is IButton)
				RaiseButtonUnchecked(button as IButton);
		}
		protected void RaiseButtonClick(IButton button) {
			Raise(ButtonClick, button);
		}
		protected void RaiseButtonChecked(IButton button) {
			Raise(ButtonChecked, button);
		}
		protected void RaiseButtonUnchecked(IButton button) {
			Raise(ButtonUnchecked, button);
		}
		protected void Raise(ButtonEventHandler handler, IButton button) {
			if(handler != null) handler(this, new ButtonEventArgs(button));
		}
		#endregion Events
	}
	public class OverviewButtonsPanel : ButtonsPanel {
		public OverviewButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new OverviewButtonsPanelViewInfo(this);
		}
	}
	public class TabbedGroupButtonsPanel : ButtonsPanel {
		public TabbedGroupButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new TabbedGroupButtonsPanelViewInfo(this);
		}
	}
}
