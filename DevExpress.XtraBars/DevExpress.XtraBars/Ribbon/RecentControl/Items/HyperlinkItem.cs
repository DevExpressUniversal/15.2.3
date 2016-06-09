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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon {
	public class RecentHyperlinkItem : RecentLabelItem {
		static object openLink = new object();
		ProcessWindowStyle browserWindowStyle;
		bool linkVisited;
		Color linkColor, visitedColor;
		string link;
		public RecentHyperlinkItem()
			: base() {
			this.browserWindowStyle = ProcessWindowStyle.Normal;
			this.linkVisited = false;
			this.linkColor = Color.Empty;
			this.visitedColor = Color.Empty;
			this.link = string.Empty;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image Glyph { get { return base.Glyph; } set { base.Glyph = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image GlyphDisabled { get { return base.GlyphDisabled; } set { base.GlyphDisabled = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image GlyphHover { get { return base.GlyphHover; } set { base.GlyphHover = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image GlyphPressed {
			get { return base.GlyphPressed; }
			set { base.GlyphPressed = value; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RecentHyperlinkItemBrowserWindowStyle"),
#endif
 DefaultValue(ProcessWindowStyle.Normal), SmartTagProperty("Browser Window Style", "")]
		public virtual ProcessWindowStyle BrowserWindowStyle {
			get { return browserWindowStyle; }
			set {
				if(BrowserWindowStyle == value) return;
				browserWindowStyle = value;
				OnItemChanged();
			}
		}
		[DefaultValue(false)]
		public bool LinkVisited {
			get { return linkVisited; }
			set {
				if(LinkVisited == value)
					return;
				linkVisited = value;
				OnItemChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RecentHyperlinkItemLinkColor")
#else
	Description("")
#endif
]
		public Color LinkColor {
			get { return linkColor; }
			set {
				if(LinkColor == value) return;
				linkColor = value;
				OnItemChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), Description("")]
		public Color VisitedColor {
			get { return visitedColor; }
			set {
				if(VisitedColor == value) return;
				visitedColor = value;
				OnItemChanged();
			}
		}
		[DefaultValue(""), DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RecentHyperlinkItemLink")
#else
	Description("")
#endif
]
		public string Link {
			get { return link; }
			set { link = value; }
		}
		[DefaultValue(RecentLabelStyles.Medium), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override RecentLabelStyles Style {
			get { return RecentLabelStyles.Medium; }
			set { base.Style = value; }
		}
		public void ShowBrowser() {
			if(Link != string.Empty) ShowBrowser(Link);
			else ShowBrowser(Caption);
		}
		public virtual void ShowBrowser(object linkValue) {
			OpenLinkEventArgs e = new OpenLinkEventArgs(linkValue);
			RaiseOpenLink(e);
			if(!CanShowBrowser(e)) return;
			Process process = new Process();
			process.StartInfo.FileName = (e.EditValue == null ? string.Empty : e.EditValue.ToString());
			process.StartInfo.WindowStyle = BrowserWindowStyle;
			try {
				process.Start();
			}
			catch { }
		}
		protected virtual bool CanShowBrowser(OpenLinkEventArgs e) {
			return !(e.Handled || e.EditValue == null || e.EditValue == DBNull.Value);
		}
		protected override RecentItemViewInfoBase CreateItemViewInfo() {
			return new RecentHyperLinkItemViewInfo(this);
		}
		protected override RecentItemPainterBase CreateItemPainter() {
			return new RecentHyperLinkItemPainter();
		}
		protected override RecentItemHandlerBase CreateItemHandler() {
			return new RecentHyperLinkItemHandler(this);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RecentHyperlinkItemOpenLink"),
#endif
 DXCategory(CategoryName.Events)]
		public event OpenLinkEventHandler OpenLink {
			add { this.Events.AddHandler(openLink, value); }
			remove { this.Events.RemoveHandler(openLink, value); }
		}
		protected internal virtual void RaiseOpenLink(OpenLinkEventArgs e) {
			OpenLinkEventHandler handler = (OpenLinkEventHandler)this.Events[openLink];
			if(handler != null) handler(this, e);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RecentHyperLinkItemViewInfo : RecentLabelItemViewInfo {
		public RecentHyperLinkItemViewInfo(RecentHyperlinkItem item)
			: base(item) {
		}
		public new RecentHyperlinkItem Item { get { return base.Item as RecentHyperlinkItem; } }
		protected override BaseRecentItemAppearanceCollection ControlAppearances {
			get {
				return (Item.RecentControl.Appearances as RecentAppearanceCollection).LabelItem.Label;
			}
		}
		protected override void UpdateDefaults(DevExpress.Utils.AppearanceObject app) {
			base.UpdateDefaults(app);
			app.ForeColor = GetLinkColorCore();
		}
		protected override DevExpress.Utils.AppearanceDefaultInfo[] GetAppearanceDefaultInfo() {
			AppearanceDefault appearance = new AppearanceDefault();
			appearance.Font = new Font("SegoeUI", 11.0f, FontStyle.Underline);
			return new AppearanceDefaultInfo[]{
				new AppearanceDefaultInfo("ItemNormal", appearance),
				new AppearanceDefaultInfo("ItemHovered", appearance),
				new AppearanceDefaultInfo("ItemPressed", appearance)
			};
		}
		Color GetLinkColorCore() {
			if(Item.LinkVisited) return GetVisitedColor();
			return GetLinkColor();
		}
		Color GetLinkColor() {
			if(Item.LinkColor == Color.Empty)
				return GetSkinColor(EditorsSkins.SkinHyperlinkTextColor);
			return Item.LinkColor;
		}
		Color GetSkinColor(string name) {
			return EditorsSkins.GetSkin(Item.RecentControl.LookAndFeel.ActiveLookAndFeel).Colors.GetColor(name);
		}
		Color GetVisitedColor() {
			if(Item.VisitedColor == Color.Empty)
				return GetSkinColor(EditorsSkins.SkinHyperlinkTextColorVisited);
			return Item.VisitedColor;
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class RecentHyperLinkItemPainter : RecentTextGlyphItemPainterBase {
	}
}
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RecentHyperLinkItemHandler : RecentLabelItemHandler {
		public RecentHyperLinkItemHandler(RecentHyperlinkItem item) : base(item) { }
		public new RecentHyperlinkItem Item { get { return base.Item as RecentHyperlinkItem; } }
		public override bool OnMouseUp(MouseEventArgs e) {
			Item.ShowBrowser();
			return base.OnMouseUp(e);
		}
	}
}
