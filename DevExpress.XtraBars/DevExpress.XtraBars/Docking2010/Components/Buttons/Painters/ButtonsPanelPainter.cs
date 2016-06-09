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

using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public class ActionBarButtonsPanelPainter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new ActionsBarButtonPainter();
		}
	}
	public class ActionBarButtonsSkinPanelPainter : BaseButtonsPanelSkinPainter {
		public ActionBarButtonsSkinPanelPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new ActionsBarButtonSkinPainter(Provider);
		}
	}
	public class PageGroupButtonsPanelSkinPainter : BaseButtonsPanelSkinPainter {
		public PageGroupButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new PageGroupButtonSkinPainter(Provider);
		}
	}
	public class TabbedGroupButtonsPanelTabPainter : BaseButtonsPanelOffice2000Painter { }
	public class TabbedGroupButtonsPanelTabSkinPainter : BaseButtonsPanelSkinPainter {
		public TabbedGroupButtonsPanelTabSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new TabbedGroupTabButtonSkinPainter(Provider);
		}
	}
	public interface ITabbedGroupTileButtonPainter {
		System.Windows.Forms.Padding ContentMargin { get; }
	}
	public class TabbedGroupButtonsPanelTilePainter : BaseButtonsPanelOffice2000Painter {
		public override BaseButtonPainter GetButtonPainter() {
			return new TabbedGroupTileButtonPainter();
		}
	}
	public class TabbedGroupButtonsPanelTileSkinPainter : BaseButtonsPanelSkinPainter {
		public TabbedGroupButtonsPanelTileSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new TabbedGroupTileButtonSkinPainter(Provider);
		}
	}
	public class OverviewButtonsPanelSkinPainter : BaseButtonsPanelSkinPainter {
		public OverviewButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new OverviewButtonSkinPainter(Provider);
		}
	}
	public class CustomHeaderButtonsPanelSkinPainter : BaseButtonsPanelSkinPainter {
		public CustomHeaderButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new CustomHeaderButtonSkinPainter(Provider);
		}
	}
}
namespace DevExpress.XtraBars.Docking2010 {
	public class ButtonsPanelPainter : BaseButtonsPanelPainter { }
	public class ButtonsPanelSkinPainter : BaseButtonsPanelSkinPainter {
		public ButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new ButtonSkinPainter(Provider);
		}
	}
	public class ButtonPanelOffice2000Painter : BaseButtonsPanelOffice2000Painter { }
	public class ButtonPanelOffice2003Painter : BaseButtonsPanelOffice2003Painter { }
	class ButtonSkinPainter : BaseButtonSkinPainter {
		public ButtonSkinPainter(ISkinProvider provider) : base(provider) { }
		protected void SetDefaultForeColor(BaseButtonInfo info) { }
		protected override void CheckForeColor(BaseButtonInfo info) {
			base.CheckForeColor(info);
			Color foreColor = GetSkin().Colors.GetColor("TabHeaderTextColor");
			if((info.State & (ObjectState.Hot | ObjectState.Pressed | ObjectState.Selected)) != 0)
				foreColor = GetSkin().CommonSkin.Colors.GetColor("ControlText");
			paintAppearanceCore = new FrozenAppearance();
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { info.PaintAppearance }, new AppearanceDefault(foreColor, Color.Empty));
		}
	}
}
