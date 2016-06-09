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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Skins;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraTab.Registrator {
	public class PaintStyleCollection : CollectionBase {
		static PaintStyleCollection defaultPaintStyles;
		public static PaintStyleCollection DefaultPaintStyles { get { return defaultPaintStyles; } }
		static PaintStyleCollection() {
			defaultPaintStyles = new PaintStyleCollection();
			defaultPaintStyles.Add(new BaseViewInfoRegistrator());
			defaultPaintStyles.Add(new PropertyViewInfoRegistrator());
			defaultPaintStyles.Add(new FlatViewInfoRegistrator());
			defaultPaintStyles.Add(new Office2003ViewInfoRegistrator());
			defaultPaintStyles.Add(new WindowsXPViewInfoRegistrator());
			defaultPaintStyles.Add(new SkinViewInfoRegistrator());
		}
		public PaintStyleCollection() { }
		public BaseViewInfoRegistrator this[int index] { get { return List[index] as BaseViewInfoRegistrator; } }
		public BaseViewInfoRegistrator this[string viewName] { 
			get { 
				foreach(BaseViewInfoRegistrator vi in List) {
					if(vi.ViewName == viewName) return vi;
				}
				return null;
			}
		}
		public virtual int Add(BaseViewInfoRegistrator info) {
			if(info == null) return -1;
			int i = IndexOf(info);
			if(i != -1) return i;
			Remove(info.ViewName);
			return List.Add(info);
		}
		public virtual void Remove(BaseViewInfoRegistrator info) { List.Remove(info); }
		public virtual void Remove(string viewName) {
			BaseViewInfoRegistrator vi = this[viewName];
			if(vi != null) Remove(vi);
		}
		public virtual int IndexOf(BaseViewInfoRegistrator info) { return List.IndexOf(info); }
		public virtual bool Contains(BaseViewInfoRegistrator info) { return List.Contains(info); }
		public virtual BaseViewInfoRegistrator GetView(string viewName) {
			BaseViewInfoRegistrator vi = this[viewName];
			if(vi != null && !vi.CanUsePaintStyle) vi = null;
			if(vi == null) vi = this["Standard"];
			return vi;
		}
		protected override void OnInsertComplete(int position, object item) {
			base.OnInsertComplete(position, item);
			BaseViewInfoRegistrator vi = item as BaseViewInfoRegistrator;
			vi.Init();
		}
		public virtual BaseViewInfoRegistrator GetView(UserLookAndFeel lookAndFeel, string viewName) {
			if(viewName == BaseViewInfoRegistrator.DefaultViewName) {
				if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
				switch(lookAndFeel.ActiveStyle) {
					case ActiveLookAndFeelStyle.Office2003 : 
							viewName = "Office2003";
						break;
					case ActiveLookAndFeelStyle.WindowsXP : viewName = "WindowsXP"; break;
					case ActiveLookAndFeelStyle.Flat: viewName = "Flat"; break;
					case ActiveLookAndFeelStyle.Style3D : viewName = "Standard"; break;
					case ActiveLookAndFeelStyle.UltraFlat : viewName = "Flat"; break;
					case ActiveLookAndFeelStyle.Skin : viewName = "Skin"; break;
				}
			}
			return GetView(viewName);
		}
	}
	public enum TabPageAppearance { 
		TabControl, 
		PageClient,
		PageHeader, PageHeaderActive, PageHeaderPressed, PageHeaderHotTracked, PageHeaderDisabled, PageHeaderTabInactive,
		TabHeaderButton, TabHeaderButtonHot
	};
	public class BaseViewInfoRegistrator : IDisposable {
		public const string DefaultViewName = "Default";
		public override string ToString() { return ViewName; }
		public virtual string ViewName { get { return "Standard"; } }
		public virtual void Dispose() { }
		protected internal virtual void Init() {
		}
		public virtual bool CanUsePaintStyle { get { return true; } }
		public virtual BaseTabControlViewInfo CreateViewInfo(IXtraTab tabControl) { return new BaseTabControlViewInfo(tabControl); }
		public virtual BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new BaseTabHeaderViewInfo(viewInfo);
		}
		public virtual BaseTabHandler CreateHandler(IXtraTab tabControl) {
			return new BaseTabHandler(tabControl);
		}
		public virtual BaseTabPainter CreatePainter(IXtraTab tabControl) {
			return new Style3DTabPainter(tabControl);
		}
		public virtual ObjectPainter CreateClosePageButtonPainter(IXtraTab tabControl) {
			return EditorButtonHelper.GetPainter(BorderStyles.Style3D);
		}
		public virtual BaseButtonsPanelPainter CreateControlBoxPainter(IXtraTab tabControl) {
			return new TabButtonsPanelStyle3DPainter();
		}
		public virtual BorderPainter CreateTabControlBorderPainter() {
			return new EmptyBorderPainter();
		}
		public virtual BorderPainter CreatePageClientBorderPainter() {
			return new Border3DRaisedPainter();
		}
		public virtual BorderPainter CreateHeaderBorderPainter() {
			return new EmptyBorderPainter();
		}
		public virtual BorderPainter CreateHeaderRowBorderPainter() {
			return new EmptyBorderPainter();
		}
		protected internal virtual void RegisterDefaultAppearances(IXtraTab tabControl, Hashtable appearances) {
			appearances.Clear();
			appearances[TabPageAppearance.TabControl] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[TabPageAppearance.PageClient] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[TabPageAppearance.PageHeader] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[TabPageAppearance.PageHeaderActive] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[TabPageAppearance.PageHeaderPressed] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[TabPageAppearance.PageHeaderHotTracked] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[TabPageAppearance.PageHeaderDisabled] = new AppearanceDefault(SystemColors.GrayText, SystemColors.Control);
		}
	}
	public class PropertyViewInfoRegistrator : BaseViewInfoRegistrator {
		public override string ViewName { get { return "PropertyView"; } }
		public override BaseTabPainter CreatePainter(IXtraTab tabControl) {
			return new PropertyViewTabPainter(tabControl);
		}
		public override ObjectPainter CreateClosePageButtonPainter(IXtraTab tabControl) {
			return EditorButtonHelper.GetPainter(BorderStyles.UltraFlat);
		}
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new PropertyViewTabHeaderViewInfo(viewInfo); 
		}
		protected internal override void RegisterDefaultAppearances(IXtraTab tabControl, Hashtable appearances) {
			appearances.Clear();
			appearances[TabPageAppearance.TabControl] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageClient] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, SystemColors.Highlight, Color.Empty, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeader] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.GrayText, Color.Empty, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderActive] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, SystemColors.Highlight, Color.Empty, new Font(AppearanceObject.DefaultFont, FontStyle.Bold), HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderPressed] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderHotTracked] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderDisabled] = new AppearanceDefault(SystemColors.GrayText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
		}
		public override BorderPainter CreatePageClientBorderPainter() {
			return new PropertyViewClientBorderPainter();
		}
		public override BorderPainter CreateHeaderRowBorderPainter() {
			return new SideHeaderRowBorderPainter();
		}
	}
	public class FlatViewInfoRegistrator : BaseViewInfoRegistrator {
		public override string ViewName { get { return "Flat"; } }
		public override BaseTabPainter CreatePainter(IXtraTab tabControl) {
			return new FlatTabPainter(tabControl);
		}
		public override ObjectPainter CreateClosePageButtonPainter(IXtraTab tabControl) {
			return new EditorButtonPainter(new FlatTabHeaderButtonPainter());
		}
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new FlatTabHeaderViewInfo(viewInfo); 
		}
		public override BorderPainter CreatePageClientBorderPainter() {
			return new EmptyBorderPainter();
		}
		public override BorderPainter CreateTabControlBorderPainter() {
			return new HotFlatBorderPainter();
		}
		public override BorderPainter CreateHeaderRowBorderPainter() {
			return new FlatTabHeaderRowBorderPainter();
		}
		public override BaseButtonsPanelPainter CreateControlBoxPainter(IXtraTab tabControl) {
			return new TabButtonsPanelFlatPainter();
		}
		protected internal override void RegisterDefaultAppearances(IXtraTab tabControl, Hashtable appearances) {
			appearances.Clear();
			appearances[TabPageAppearance.TabControl] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageClient] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeader] = new AppearanceDefault(SystemColors.ControlDarkDark, SystemColors.Control, Color.Black, Color.Empty, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderActive] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.White, Color.Empty, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderPressed] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderHotTracked] = new AppearanceDefault(SystemColors.ControlDarkDark, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderDisabled] = new AppearanceDefault(SystemColors.GrayText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
		}
	}
	public class WindowsXPViewInfoRegistrator : BaseViewInfoRegistrator {
		public override string ViewName { get { return "WindowsXP"; } }
		public override ObjectPainter CreateClosePageButtonPainter(IXtraTab tabControl) {
			return EditorButtonHelper.GetWindowsXPPainter();
		}
		public override BaseTabPainter CreatePainter(IXtraTab tabControl) {
			return new WindowsXPTabPainter(tabControl);
		}
		public override BaseTabControlViewInfo CreateViewInfo(IXtraTab tabControl) { 
			return new WindowsXPTabControlViewInfo(tabControl); 
		}
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new WindowsXPTabHeaderViewInfo(viewInfo); 
		}
		protected internal override void RegisterDefaultAppearances(IXtraTab tabControl, Hashtable appearances) {
			appearances.Clear();
			appearances[TabPageAppearance.TabControl] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageClient] = new AppearanceDefault(SystemColors.ControlText, Color.Transparent, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeader] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderActive] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderPressed] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderHotTracked] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderDisabled] = new AppearanceDefault(SystemColors.GrayText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override BaseButtonsPanelPainter CreateControlBoxPainter(IXtraTab tabControl) {
			return new TabButtonsPanelWXPPainter(tabControl);
		}
	}
	public class Office2003ViewInfoRegistrator : BaseViewInfoRegistrator {
		public override string ViewName { get { return "Office2003"; } }
		public override BaseTabPainter CreatePainter(IXtraTab tabControl) { return new Office2003TabPainter(tabControl);}
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new Office2003TabHeaderViewInfo(viewInfo); 
		}
		public override ObjectPainter CreateClosePageButtonPainter(IXtraTab tabControl) {
			return new EditorButtonPainter(new Office2003TabHeaderButtonPainter());
		}
		public override BorderPainter CreatePageClientBorderPainter() {
			return new EmptyBorderPainter();
		}
		public override BorderPainter CreateTabControlBorderPainter() {
			return new Office2003BorderPainter(true);
		}
		public override BorderPainter CreateHeaderRowBorderPainter() {
			return new Office2003TabHeaderRowBorderPainter();
		}
		public override BaseButtonsPanelPainter CreateControlBoxPainter(IXtraTab tabControl) {
			return new TabButtonsPanelTab2003Painter();
		}
		protected internal override void RegisterDefaultAppearances(IXtraTab tabControl, Hashtable appearances) {
			Office2003Colors.Default.Init();
			appearances.Clear();
			appearances[TabPageAppearance.TabControl] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageClient] = 
				new AppearanceDefault(SystemColors.ControlText, Clr(Office2003Color.TabPageClient), HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeader] = 
				new AppearanceDefault(Clr(Office2003Color.TabPageForeColor), Clr(Office2003Color.TabBackColor1), Clr(Office2003Color.TabPageBorderColor), Clr(Office2003Color.TabBackColor2), LinearGradientMode.Vertical, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderActive] = 
				new AppearanceDefault(Clr(Office2003Color.TabPageForeColor), Clr(Office2003Color.TabPageBackColor1), Clr(Office2003Color.TabPageBorderColor), Clr(Office2003Color.TabPageBackColor2), LinearGradientMode.Vertical, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderPressed] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageHeaderHotTracked] = (AppearanceDefault)appearances[TabPageAppearance.PageHeader];
			appearances[TabPageAppearance.PageHeaderDisabled] = ((AppearanceDefault)appearances[TabPageAppearance.PageHeader]).Clone();
			((AppearanceDefault)appearances[TabPageAppearance.PageHeaderDisabled]).ForeColor = SystemColors.GrayText;
		}
		Color Clr(Office2003Color color) {
			return Office2003Colors.Default[color];
		}
	}
	public class SkinViewInfoRegistrator : BaseViewInfoRegistrator {
		public override string ViewName { get { return "Skin"; } }
		public override BaseTabPainter CreatePainter(IXtraTab tabControl) {
			return new SkinTabPainter(tabControl);
		}
		public override ObjectPainter CreateClosePageButtonPainter(IXtraTab tabControl) {
			return new SkinTabPageButtonPainter(tabControl.LookAndFeel);
		}
		public override BaseTabControlViewInfo CreateViewInfo(IXtraTab tabControl) {
			return new SkinTabControlViewInfo(tabControl);
		}
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new SkinTabHeaderViewInfo(viewInfo);
		}
		public override BaseButtonsPanelPainter CreateControlBoxPainter(IXtraTab tabControl) {
			return new TabButtonsPanelSkinPainter(tabControl.LookAndFeel);
		}
		SkinElement GetSkinElement(IXtraTab tabControl, string element) {
			return SkinManager.Default.GetSkin(SkinProductId.Tab, tabControl.LookAndFeel)[element];
		}
		public Skin GetSkin(IXtraTab tabControl) { return TabSkins.GetSkin(tabControl.LookAndFeel); }
		protected internal override void RegisterDefaultAppearances(IXtraTab tabControl, Hashtable appearances) {
			appearances.Clear();
			appearances[TabPageAppearance.TabControl] = new AppearanceDefault(SystemColors.ControlText, Color.Transparent, HorzAlignment.Center, VertAlignment.Center);
			appearances[TabPageAppearance.PageClient] = GetSkinElement(tabControl, TabSkins.SkinTabPane).Apply(new AppearanceDefault(SystemColors.ControlText, Color.Transparent, HorzAlignment.Center, VertAlignment.Center), tabControl.LookAndFeel);
			Font font = GetSkin(tabControl)[TabSkins.SkinTabHeader].GetFont(null, tabControl.LookAndFeel);
			var skinColors = GetSkin(tabControl).Colors;
			appearances[TabPageAppearance.PageHeader] = new AppearanceDefault(skinColors[TabSkinProperties.TabHeaderTextColor], Color.Transparent, font);
			Color pageHeaderActive = skinColors[TabSkinProperties.TabHeaderTextColorActive];
			Color pageHeaderTabInactive = skinColors.GetColor(TabSkinProperties.TabHeaderTextColorTabInactive, pageHeaderActive);
			appearances[TabPageAppearance.PageHeaderActive] = new AppearanceDefault(pageHeaderActive, Color.Transparent, font);
			appearances[TabPageAppearance.PageHeaderTabInactive] = new AppearanceDefault(pageHeaderTabInactive, Color.Transparent, font);
			appearances[TabPageAppearance.PageHeaderHotTracked] = new AppearanceDefault(skinColors[TabSkinProperties.TabHeaderTextColorHot], Color.Transparent, font);
			appearances[TabPageAppearance.PageHeaderPressed] = new AppearanceDefault(skinColors[TabSkinProperties.TabHeaderTextColorHot], Color.Transparent, font);
			appearances[TabPageAppearance.PageHeaderDisabled] = new AppearanceDefault(skinColors[TabSkinProperties.TabHeaderTextColorDisabled], Color.Transparent, font);
			appearances[TabPageAppearance.TabHeaderButton] = new AppearanceDefault(skinColors[TabSkinProperties.TabHeaderButtonTextColor], Color.Transparent, font);
			appearances[TabPageAppearance.TabHeaderButtonHot] = new AppearanceDefault(skinColors[TabSkinProperties.TabHeaderButtonTextColorHot], Color.Transparent, font);
		}
	}
}
