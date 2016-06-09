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

using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraEditors.ButtonPanel {
	public class BaseTabButtonsPanelPainter : BaseButtonsPanelPainter {
		public void DrawCheckedButton(ObjectInfoArgs e) {
			DrawCheckedButtons(e.Cache, e as IButtonsPanelViewInfo);
		}
		public void DrawCloseButton(ObjectInfoArgs e) {
			DrawButton(e.Cache, e as IButtonsPanelViewInfo, typeof(TabCloseButton));
		}
		public void DrawPinButton(ObjectInfoArgs e) {
			DrawButton(e.Cache, e as IButtonsPanelViewInfo, typeof(TabPinButton));
		}
		protected virtual void DrawButton(GraphicsCache cache, IButtonsPanelViewInfo info, System.Type type) {
			BaseButtonPainter painter = GetButtonPainter();
			if(info.Buttons == null) return;
			foreach(BaseButtonInfo button in info.Buttons) {
				if(button.Button.GetType() == type) {
					DrawButton(cache, info, painter, button);
				}
			}
		}
		protected virtual void DrawCheckedButtons(GraphicsCache cache, IButtonsPanelViewInfo info) {
			BaseButtonPainter painter = GetButtonPainter();
			if(info.Buttons == null) return;
			foreach(BaseButtonInfo button in info.Buttons) {
				if(!button.Button.IsChecked.HasValue) continue;
				if(button.Button.IsChecked.HasValue && button.Button.IsChecked.Value) continue;
				DrawButton(cache, info, painter, button);
			}
		}
		protected void DrawButton(GraphicsCache cache, IButtonsPanelViewInfo info, BaseButtonPainter painter, BaseButtonInfo button) {
			button.State = CalcButtonState(button.Button, info.Panel);
			ObjectPainter.DrawObject(cache, painter, button);
		}
	}
	public class TabButtonsPanelSkinPainter : BaseTabButtonsPanelPainter {
		ISkinProvider providerCore;
		public TabButtonsPanelSkinPainter(ISkinProvider provider) {
			providerCore = provider;
		}
		public ISkinProvider Provider {
			get { return providerCore; }
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new TabButtonSkinPainter(Provider);
		}
	}
	public class TabButtonsPanelStyle3DPainter : BaseTabButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new TabButtonStyle3DPainter();
		}
	}
	public class TabButtonsPanelFlatPainter : BaseTabButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new TabButtonFlatPainter();
		}
	}
	public class TabButtonsPanelTab2003Painter : BaseTabButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new TabButtonOffice2003Painter();
		}
	}
	public class TabButtonsPanelWXPPainter : BaseTabButtonsPanelPainter {
		XtraTab.IXtraTab tabControlCore;
		public TabButtonsPanelWXPPainter(XtraTab.IXtraTab tabControl) {
			tabControlCore = tabControl;
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new TabButtonWXPPainter(tabControlCore);
		}
	}
}
