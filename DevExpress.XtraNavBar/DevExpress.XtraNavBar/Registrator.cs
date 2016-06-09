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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.XtraNavBar.Forms;
namespace DevExpress.XtraNavBar.ViewInfo
{
	[AttributeUsage(AttributeTargets.Class)]
	public class UserNavBarView : Attribute {
	}
	[TypeConverter("DevExpress.XtraNavBar.Design.BaseViewInfoRegistratorTypeConverter, " + AssemblyInfo.SRAssemblyNavBarDesign)]
	public class BaseViewInfoRegistrator {
		public BaseViewInfoRegistrator() {
			UpdateThemeColors();
		}
		public override string ToString() { return ViewName; } 
		public virtual string ViewName { get { return NavBarViewNames.SideBar3D; } }
		public virtual BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new BaseNavGroupPainter(navBar); }
		public virtual BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new BaseNavLinkPainter(navBar); }
		public virtual ObjectPainter CreateButtonPainter(NavBarControl navBar) { return new UpDownButtonObjectPainter(); }
		public virtual NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new NavBarViewInfo(navBar); }
		public virtual NavPaneForm CreateNavPaneForm(NavBarControl control) { return null; }
		protected internal virtual void UpdateThemeColors() {
		}
		public virtual AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { return defaultAppearance; }
		static AppearanceDefaultInfo[] defaultAppearance = new AppearanceDefaultInfo[] 
			{
			new AppearanceDefaultInfo("GroupBackground", new AppearanceDefault(Color.Black, SystemColors.ControlDark, SystemColors.ControlDark)),
			new AppearanceDefaultInfo("GroupHeader", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Item", new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(SystemColors.ControlDarkDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonDisabled", new AppearanceDefault(SystemColors.GrayText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
			};
	}
	public class FlatViewInfoRegistrator : BaseViewInfoRegistrator {
		public override string ViewName { get { return NavBarViewNames.FlatSideBar; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new FlatNavGroupPainter(navBar); }
	}
	public class XP1ViewInfoRegistrator : FlatViewInfoRegistrator  {
		protected virtual bool CanUseXP { get { return DevExpress.Utils.WXPaint.Painter.ThemesEnabled; } }
		public override string ViewName { get { return "XP1View"; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { 
			return new XPNavGroupPainter(navBar); 
		}
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateLinkPainter(navBar);
			return new XPNavLinkPainter(navBar); 
		}
		public override ObjectPainter CreateButtonPainter(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateButtonPainter(navBar);
			return new XPUpDownButtonObjectPainter(); 
		}
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateViewInfo(navBar);
			return new XPNavBarViewInfo(navBar); 
		}
		protected internal override void UpdateThemeColors() {
			defaultAppearance = null;
		}
		static AppearanceDefaultInfo[] defaultAppearance = null;
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { 
			if(defaultAppearance == null) defaultAppearance = CreateDefaultAppearance();
			return defaultAppearance; 
		}
		static AppearanceDefaultInfo[] CreateDefaultAppearance() {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo("GroupBackground", new AppearanceDefault(Color.Black, ControlPaint.LightLight(SystemColors.InactiveCaption), SystemColors.InactiveCaption, SystemColors.InactiveCaption)),
				new AppearanceDefaultInfo("GroupHeader", new AppearanceDefault(SystemColors.ActiveCaptionText, ControlPaint.LightLight(SystemColors.ActiveCaption), SystemColors.ActiveCaption, SystemColors.ActiveCaption, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("GroupHeaderPressed",	new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("GroupHeaderHotTracked",  new AppearanceDefault(Color.Empty, ControlPaint.LightLight(SystemColors.ActiveCaption), Color.Empty, ControlPaint.Light(SystemColors.ActiveCaption), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("GroupHeaderActive",	new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Item",			new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("ItemPressed",	new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("Background", new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
			};
		}
	}
	public class XP2ViewInfoRegistrator : Office1ViewInfoRegistrator  {
		protected virtual bool CanUseXP { get { return DevExpress.Utils.WXPaint.Painter.ThemesEnabled; } }
		public override string ViewName { get { return NavBarViewNames.XPSideBar; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateGroupPainter(navBar);
			return new XP2NavGroupPainter(navBar); 
		}
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateLinkPainter(navBar);
			return new XP2NavLinkPainter(navBar); 
		}
		public override ObjectPainter CreateButtonPainter(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateButtonPainter(navBar);
			return new XPUpDownButtonObjectPainter(); 
		}
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateViewInfo(navBar);
			return new XP2NavBarViewInfo(navBar); 
		}
	}
	public class Office1ViewInfoRegistrator : FlatViewInfoRegistrator {
		public override string ViewName { get { return "Office1View"; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new Office1NavGroupPainter(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new Office1NavLinkPainter(navBar); }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new Office1NavBarViewInfo(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { return defaultAppearanceOffice; }
		static internal AppearanceDefaultInfo[] defaultAppearanceOffice = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("GroupBackground", new AppearanceDefault(Color.Black, SystemColors.Control, SystemColors.Control)),
			new AppearanceDefaultInfo("GroupHeader", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Item", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(SystemColors.ControlDark, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(SystemColors.Control, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
			};
	}
	public class Office2ViewInfoRegistrator : FlatViewInfoRegistrator {
		public override string ViewName { get { return "Office2View"; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new Office1NavGroupPainter(navBar); }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new Office1NavBarViewInfo(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new Office2NavLinkPainter(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { return Office1ViewInfoRegistrator.defaultAppearanceOffice; }
	}
	public class Office3ViewInfoRegistrator : FlatViewInfoRegistrator {
		public override string ViewName { get { return "Office3View"; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new Office1NavGroupPainter(navBar); }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new Office3NavBarViewInfo(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new Office3NavLinkPainter(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { return Office1ViewInfoRegistrator.defaultAppearanceOffice; }
	}
	public class VSToolBoxViewInfoRegistrator : FlatViewInfoRegistrator {
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new VSToolBoxNavGroupPainter(navBar); }
		public override string ViewName { get { return "VSToolBoxView"; } }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new VSToolBoxNavBarViewInfo(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new VSToolBoxNavLinkPainter(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { return defaultAppearance; }
		static AppearanceDefaultInfo[] defaultAppearance = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("GroupBackground", new AppearanceDefault(Color.Black, SystemColors.Control, SystemColors.ControlDark)),
			new AppearanceDefaultInfo("GroupHeader", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("Item", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(SystemColors.Control, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonDisabled", new AppearanceDefault(SystemColors.GrayText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
			};
	}
	public class ExplorerBarViewInfoRegistrator : FlatViewInfoRegistrator {
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new ExplorerBarNavGroupPainter(navBar); }
		public override string ViewName { get { return NavBarViewNames.FlatExplorerBar; } }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new ExplorerBarNavBarViewInfo(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new ExplorerBarNavLinkPainter(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { return defaultAppearance; }
		static AppearanceDefaultInfo[] defaultAppearance = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("GroupBackground", new AppearanceDefault(Color.Black, SystemColors.Window, SystemColors.Control)),
			new AppearanceDefaultInfo("GroupHeader", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center) { FontStyleDelta = FontStyle.Bold }), 
			new AppearanceDefaultInfo("GroupHeaderPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center) { FontStyleDelta = FontStyle.Bold }),
			new AppearanceDefaultInfo("Item", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }), 
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }), 
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(SystemColors.Control, SystemColors.Window, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
		};
	}
	public class XPExplorerBarViewInfoRegistrator : ExplorerBarViewInfoRegistrator {
		protected virtual bool CanUseXP { get { return DevExpress.Utils.WXPaint.Painter.ThemesEnabled; } }
		public override string ViewName { get { return "XPExplorerBarView"; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateGroupPainter(navBar);
			return new XPExplorerBarNavGroupPainter(navBar); }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateViewInfo(navBar);
			return new XPExplorerBarNavBarViewInfo(navBar); 
		}
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { 
			if(!CanUseXP) return base.CreateLinkPainter(navBar);
			return new XPExplorerBarNavLinkPainter(navBar); }
	}
	public class NavBarViewNames {
		public static string 
			AdvExporerBar = "AdvExplorerBarView",
			FlatExplorerBar = "ExplorerBarView",
			NavigationPane = "NavigationPane",
			SideBar3D = "BaseView",
			FlatSideBar = "FlatView",
			XPSideBar = "XP2View",
			SkinExplorerBar = "SkinExplorerBarView",
			SkinNavigationPane = "SkinNavigationPane";
		}
	public class AdvExplorerBarViewInfoRegistrator : ExplorerBarViewInfoRegistrator {
		public override string ViewName { get { return NavBarViewNames.AdvExporerBar; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new AdvExplorerBarNavGroupPainter(navBar); }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new AdvExplorerBarNavBarViewInfo(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new AdvExplorerBarNavLinkPainter(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { 
			if(defaultAppearance == null) defaultAppearance = CreateAdvExplorerBarDefaultAppearance();
			return defaultAppearance;
		}
		static AppearanceDefaultInfo[] defaultAppearance;
		static AppearanceDefaultInfo[] CreateAdvExplorerBarDefaultAppearance() {
			return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("GroupBackground", new AppearanceDefault(Color.Black, Office2003Colors.Default[Office2003Color.NavBarGroupClientBackColor], HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeader", new AppearanceDefault(Color.Empty, Office2003Colors.Default[Office2003Color.NavBarGroupCaptionBackColor1], Color.Empty, Office2003Colors.Default[Office2003Color.NavBarGroupCaptionBackColor2], HorzAlignment.Near, VertAlignment.Center) { FontStyleDelta = FontStyle.Bold }), 
			new AppearanceDefaultInfo("GroupHeaderPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("Item", new AppearanceDefault(Color.Empty, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }),
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }),
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(Color.Silver, Office2003Colors.Default[Office2003Color.NavBarBackColor1], Color.Empty, Office2003Colors.Default[Office2003Color.NavBarBackColor2], LinearGradientMode.Vertical)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
		  };
		}
		protected internal override void UpdateThemeColors() {
			defaultAppearance = null;
		}
	}
	public class UltraFlatExplorerBarViewInfoRegistrator : ExplorerBarViewInfoRegistrator {
		public override string ViewName { get { return "UltraFlatExplorerBarView"; } }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new UltraFlatExplorerBarNavBarViewInfo(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new UltraFlatExplorerBarNavLinkPainter(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { return defaultAppearance; }
		static AppearanceDefaultInfo[] defaultAppearance = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("GroupBackground", new AppearanceDefault(Color.Black, SystemColors.Window, SystemColors.Control)),
			new AppearanceDefaultInfo("GroupHeader", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Item", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(SystemColors.GrayText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(SystemColors.Control, SystemColors.Window, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
		};
	}
	public class NavigationPaneViewInfoRegistrator : BaseViewInfoRegistrator {
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new NavigationPaneGroupPainter(navBar); }
		public override string ViewName { get { return NavBarViewNames.NavigationPane; } }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new NavigationPaneViewInfo(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new NavigationPaneLinkPainter(navBar); }
		public override NavPaneForm CreateNavPaneForm(NavBarControl control) { return new NavPaneForm(control); }
		static AppearanceDefaultInfo[] defaultAppearance;
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { 
			if(defaultAppearance == null) defaultAppearance = CreateNavPaneDefaultAppearance();
			return defaultAppearance;
		}
		static AppearanceDefaultInfo[] CreateNavPaneDefaultAppearance() {
			return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("NavigationPaneHeader", new AppearanceDefault(SystemColors.Window, ControlPaint.Light(Office2003Colors.Default[Office2003Color.NavBarNavPaneHeaderBackColor]), Color.Empty, ControlPaint.Dark(Office2003Colors.Default[Office2003Color.NavBarNavPaneHeaderBackColor], 0.05f), LinearGradientMode.Vertical, HorzAlignment.Near, VertAlignment.Center) { FontStyleDelta = FontStyle.Bold, FontSizeDelta = 2 }),
			new AppearanceDefaultInfo("GroupBackground", new AppearanceDefault(Color.Black, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeader", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1], Office2003Colors.Default[Office2003Color.NavPaneBorderColor], Office2003Colors.Default[Office2003Color.Button2], LinearGradientMode.Vertical, HorzAlignment.Near, VertAlignment.Center) { FontStyleDelta = FontStyle.Bold }),
			new AppearanceDefaultInfo("GroupHeaderPressed", new AppearanceDefault(Color.Empty, Office2003Colors.Default[Office2003Color.Button1Pressed], Color.Empty, Office2003Colors.Default[Office2003Color.Button2Pressed], LinearGradientMode.Vertical, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", new AppearanceDefault(Color.Empty, Office2003Colors.Default[Office2003Color.Button1Hot], Color.Empty, Office2003Colors.Default[Office2003Color.Button2Hot], LinearGradientMode.Vertical, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderActive", new AppearanceDefault(Color.Empty, Office2003Colors.Default[Office2003Color.Button2Pressed], Office2003Colors.Default[Office2003Color.NavPaneBorderColor], Office2003Colors.Default[Office2003Color.Button1Pressed], LinearGradientMode.Vertical, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("Item", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1], Office2003Colors.Default[Office2003Color.NavPaneBorderColor], Office2003Colors.Default[Office2003Color.Button2], LinearGradientMode.Vertical, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(Color.Empty, Office2003Colors.Default[Office2003Color.Button1Pressed], Color.Empty, Office2003Colors.Default[Office2003Color.Button2Pressed], LinearGradientMode.Vertical, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(Color.Empty, Office2003Colors.Default[Office2003Color.Button1Hot], Color.Empty, Office2003Colors.Default[Office2003Color.Button2Hot], LinearGradientMode.Vertical, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Office2003Colors.Default[Office2003Color.Button2Pressed], Office2003Colors.Default[Office2003Color.NavPaneBorderColor], Office2003Colors.Default[Office2003Color.Button1Pressed], LinearGradientMode.Vertical, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(SystemColors.Control, SystemColors.Window, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("NavPaneCollapsedCaption", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
		  };
		}
		protected internal override void UpdateThemeColors() {
			defaultAppearance = null;
		}
	}
	[TypeConverter("DevExpress.XtraNavBar.Design.StandardSkinViewInfoRegistratorTypeConverter, " + AssemblyInfo.SRAssemblyNavBarDesign)]
	public class StandardSkinExplorerBarViewInfoRegistrator : SkinExplorerBarViewInfoRegistrator, ISkinProvider {
		string skinName;
		public StandardSkinExplorerBarViewInfoRegistrator(string skinName) {
			this.skinName = skinName;
		}
		public string SkinName { get { return skinName; } }
		public override string ViewName { get { return "Skin:" + SkinName; } }
		public override Skin GetSkin(NavBarControl navBar) { return NavBarSkins.GetSkin(this); }
	}
	public class SkinExplorerBarViewInfoRegistrator : ExplorerBarViewInfoRegistrator {
		public override string ViewName { get { return NavBarViewNames.SkinExplorerBar; } }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new SkinExplorerBarNavGroupPainter(navBar); }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new SkinExplorerBarNavBarViewInfo(navBar); }
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new SkinExplorerBarNavLinkPainter(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { 
			return CreateSkinExplorerBarDefaultAppearance(navBar);
		}
		protected AppearanceDefaultInfo[] CreateSkinExplorerBarDefaultAppearance(NavBarControl navBar) {
			return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("GroupBackground", UpdateAppearance(navBar, NavBarSkins.SkinGroupClient, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("GroupHeader", UpdateAppearance(navBar, NavBarSkins.SkinGroupHeader, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center))), 
			new AppearanceDefaultInfo("GroupHeaderPressed", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupHeaderActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("Item", UpdateAppearance(navBar, NavBarSkins.SkinItem, new AppearanceDefault(Color.Empty, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(GetSkin(navBar)[NavBarSkins.SkinItem].GetForeColor(ObjectState.Pressed), Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }),
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(GetSkin(navBar)[NavBarSkins.SkinItem].GetForeColor(ObjectState.Hot), Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }),
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center) { FontStyleDelta = FontStyle.Underline }),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(Color.Empty, Color.Transparent)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
		  };
		}
		public virtual Skin GetSkin(NavBarControl navBar) { return NavBarSkins.GetSkin(navBar); }
		protected AppearanceDefault UpdateAppearance(NavBarControl navBar, string elementName, AppearanceDefault info) {
			SkinElement element = GetSkin(navBar)[elementName];
			element.Apply(info);
			return info;
		}
		protected internal override void UpdateThemeColors() {
		}
	}
	public class SkinNavigationPaneViewInfoRegistrator : NavigationPaneViewInfoRegistrator {
		public override string ViewName { get { return NavBarViewNames.SkinNavigationPane; } }
		public override NavBarViewInfo CreateViewInfo(NavBarControl navBar) { return new SkinNavigationPaneViewInfo(navBar); }
		public override NavPaneForm CreateNavPaneForm(NavBarControl control) {
			return new SkinNavPaneForm(control);
		}
		public override ObjectPainter CreateButtonPainter(NavBarControl navBar) { 
			return new SkinNavigationPaneScrollButtonPainter(navBar); 
		}
		public override BaseNavLinkPainter CreateLinkPainter(NavBarControl navBar) { return new SkinNavigationPaneLinkPainter(navBar); }
		public override BaseNavGroupPainter CreateGroupPainter(NavBarControl navBar) { return new SkinNavigationPaneGroupPainter(navBar); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(NavBarControl navBar) { 
			return CreateSkinPaneDefaultAppearance(navBar);
		}
		protected AppearanceDefaultInfo[] CreateSkinPaneDefaultAppearance(NavBarControl navBar) {
			return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("NavigationPaneHeader", UpdateAppearance(navBar, NavPaneSkins.SkinCaption, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("GroupBackground", UpdateAppearance(navBar, NavPaneSkins.SkinGroupClient, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("GroupHeader", UpdateAppearance(navBar, NavPaneSkins.SkinGroupButton, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center))), 
			new AppearanceDefaultInfo("GroupHeaderPressed", UpdatePressedAppearance(navBar, NavPaneSkins.SkinGroupButton, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("GroupHeaderHotTracked", UpdateHotAppearance(navBar, NavPaneSkins.SkinGroupButton, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("GroupHeaderActive", UpdateAppearance(navBar, NavPaneSkins.SkinGroupButtonSelected, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Near, VertAlignment.Center))), 
			new AppearanceDefaultInfo("Item", UpdateAppearance(navBar, NavPaneSkins.SkinItem, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
			new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault(GetSkin(navBar)[NavPaneSkins.SkinItem].GetForeColor(ObjectState.Pressed), Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemHotTracked", new AppearanceDefault(GetSkin(navBar)[NavPaneSkins.SkinItem].GetForeColor(ObjectState.Hot), Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault(SystemColors.ControlDark, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ItemActive", UpdateAppearance(navBar, NavPaneSkins.SkinItemSelected, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
			new AppearanceDefaultInfo("Background", new AppearanceDefault(Color.Empty, Color.Transparent)),
			new AppearanceDefaultInfo("Button", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonPressed", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("ButtonHotTracked", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Hint", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("LinkDropTarget", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("NavPaneCollapsedCaption", new AppearanceDefault(Color.Black, Color.Black, HorzAlignment.Center, VertAlignment.Center))
		  };
		}
		public virtual Skin GetSkin(NavBarControl navBar) { return NavPaneSkins.GetSkin(navBar); }
		protected AppearanceDefault UpdatePressedAppearance(NavBarControl navBar, string elementName, AppearanceDefault info) {
			AppearanceDefault res = UpdateAppearance(navBar, elementName, info);
			SkinElement element = GetSkin(navBar)[elementName];
			Color color = element.GetForeColor(ObjectState.Pressed);
			if(!color.IsEmpty)
				res.ForeColor = color;
			return res;
		}
		protected AppearanceDefault UpdateHotAppearance(NavBarControl navBar, string elementName, AppearanceDefault info) {
			AppearanceDefault res = UpdateAppearance(navBar, elementName, info);
			SkinElement element = GetSkin(navBar)[elementName];
			Color color = element.GetForeColor(ObjectState.Hot);
			if(!color.IsEmpty)
				res.ForeColor = color;
			return res;
		}
		protected AppearanceDefault UpdateAppearance(NavBarControl navBar, string elementName, AppearanceDefault info) {
			SkinElement element = GetSkin(navBar)[elementName];
			if(element.Color.GetBackColor() != Color.Empty) {
				info.BackColor = element.Color.GetBackColor();
				info.BackColor2 = element.Color.GetBackColor2();
				info.GradientMode = element.Color.GradientMode;
			}
			info = element.ApplyForeColorAndFont(info);
			return info;
		}
		protected internal override void UpdateThemeColors() { }
	}
	[TypeConverter("DevExpress.XtraNavBar.Design.StandardSkinViewInfoRegistratorTypeConverter, " + AssemblyInfo.SRAssemblyNavBarDesign)]
	public class StandardSkinNavigationPaneViewInfoRegistrator : SkinNavigationPaneViewInfoRegistrator, ISkinProvider {
		string skinName;
		public StandardSkinNavigationPaneViewInfoRegistrator(string skinName) {
			this.skinName = skinName;
		}
		public string SkinName { get { return skinName; } }
		public override string ViewName { get { return "SkinNav:" + SkinName; } }
		public override Skin GetSkin(NavBarControl navBar) { return NavPaneSkins.GetSkin(this); }
	}
}
