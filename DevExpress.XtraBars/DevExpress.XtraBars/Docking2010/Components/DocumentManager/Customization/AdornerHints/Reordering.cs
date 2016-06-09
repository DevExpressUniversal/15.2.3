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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Customization {
	abstract class BaseTabHintPainter : AdornerPainter {
		protected int borderWidth = 6;
		protected void DrawTabHint(BaseTabHintInfoArgs e) {
			if(!e.TabHintVisible) return;
			e.Cache.FillRectangle(GetDockZoneBorderColor(e), e.Content);
			e.Cache.FillRectangle(GetDockZoneBorderColor(e), e.Header);
			e.Cache.FillRectangle(GetDockZoneBackColor(e), Rectangle.Inflate(e.Content, -borderWidth, -borderWidth));
			if(!e.Header.IsEmpty) {
				e.Cache.FillRectangle(GetDockZoneBackColor(e), CalcPageHeaderFillRect(e.HeaderLocation, e.Header));
			}
		}
		protected virtual Color GetDockZoneBorderColor(BaseTabHintInfoArgs e){
			return e.Appearance.BorderColor;
		}
		protected virtual Color GetDockZoneBackColor(BaseTabHintInfoArgs e) {
			return e.Appearance.BackColor;
		}
		Rectangle CalcPageHeaderFillRect(XtraTab.TabHeaderLocation headerLocation, Rectangle header) {
			switch(headerLocation) {
				case XtraTab.TabHeaderLocation.Left:
					return new Rectangle(header.Left + borderWidth, header.Top + borderWidth, header.Width, header.Height - borderWidth * 2);
				case XtraTab.TabHeaderLocation.Right:
					return new Rectangle(header.Left - borderWidth, header.Top + borderWidth, header.Width, header.Height - borderWidth * 2);
				case XtraTab.TabHeaderLocation.Bottom:
					return new Rectangle(header.Left + borderWidth, header.Top - borderWidth, header.Width - borderWidth * 2, header.Height);
				default:
					return new Rectangle(header.Left + borderWidth, header.Top + borderWidth, header.Width - borderWidth * 2, header.Height);
			}
		}
	}
	abstract class BaseTabHintInfoArgs : AdornerElementInfoArgs {
		AppearanceObject appearanceCore;
		Rectangle contentCore;
		Rectangle headerCore;
		XtraTab.TabHeaderLocation headerLocationCore;
		public BaseTabHintInfoArgs() {
			appearanceCore = new FrozenAppearance();
			Appearance.BackColor = Color.FromArgb(0xff, 0xA5, 0xC2, 0xE4);
			Appearance.BorderColor = Color.FromArgb(0xff, 0x57, 0x79, 0xAD);
		}
		public AppearanceObject Appearance {
			get { return appearanceCore; }
		}
		public XtraTab.TabHeaderLocation HeaderLocation {
			get { return headerLocationCore; }
			set { headerLocationCore = value; }
		}
		public Rectangle Content {
			get { return contentCore; }
			set { contentCore = value; }
		}
		public Rectangle Header {
			get { return headerCore; }
			set { headerCore = value; }
		}
		bool visibleCore;
		public bool TabHintVisible {
			get { return visibleCore; }
			set { visibleCore = value; }
		}
	}
	class ReorderingAdornerPainter : BaseTabHintPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DrawTabHint(e as ReorderingAdornerInfoArgs);
		}
		protected override Color GetDockZoneBorderColor(BaseTabHintInfoArgs e) {
			var ea = e as ReorderingAdornerInfoArgs;
			if(ea.Owner == null) return base.GetDockZoneBorderColor(e);
			return DrawSkinDockGuideHelper.GetColor(ea.Appearance.BorderColor, "DockZoneBorderColor", ea.Owner.ElementsLookAndFeel);
		}
		protected override Color GetDockZoneBackColor(BaseTabHintInfoArgs e) {
			var ea = e as ReorderingAdornerInfoArgs;
			if(ea.Owner == null) return base.GetDockZoneBackColor(e);
			return DrawSkinDockGuideHelper.GetColor(ea.Appearance.BackColor, "DockZoneBackColor", ea.Owner.ElementsLookAndFeel);
		}
	}
	class ReorderingAdornerInfoArgs : BaseTabHintInfoArgs {
		BaseView ownerCore;
		public ReorderingAdornerInfoArgs(BaseView owner) {
			ownerCore = owner;
		}
		public BaseView Owner {
			get { return ownerCore; }
		}
		protected override int CalcCore() { return 0; }
		protected override System.Collections.Generic.IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			return opaque ? new Rectangle[0] : new Rectangle[] { Header, Content };
		}
	}
	abstract class BaseStackGroupHintPainter : AdornerPainter {
		protected int borderWidth = 6;
		protected void DrawHint(BaseStackGroupHintInfoArgs e) {
			e.Cache.FillRectangle(GetDockZoneBorderColor(e), e.Content);
			e.Cache.FillRectangle(GetDockZoneBackColor(e), Rectangle.Inflate(e.Content, -borderWidth, -borderWidth));
		}
		protected virtual Color GetDockZoneBorderColor(BaseStackGroupHintInfoArgs e) {
			return e.Appearance.BorderColor;
		}
		protected virtual Color GetDockZoneBackColor(BaseStackGroupHintInfoArgs e) {
			return e.Appearance.BackColor;
		}
	}
	abstract class BaseStackGroupHintInfoArgs : AdornerElementInfoArgs {
		AppearanceObject appearanceCore;
		Rectangle contentCore;
		public BaseStackGroupHintInfoArgs() {
			appearanceCore = new FrozenAppearance();
			Appearance.BackColor = Color.FromArgb(0xff, 0xA5, 0xC2, 0xE4);
			Appearance.BorderColor = Color.FromArgb(0xff, 0x57, 0x79, 0xAD);
		}
		public AppearanceObject Appearance {
			get { return appearanceCore; }
		}
		public Rectangle Content {
			get { return contentCore; }
			set { contentCore = value; }
		}
	}
	class StackGroupDraggingAdornerPainter : BaseStackGroupHintPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DrawHint(e as StackGroupDraggingAdornerInfoArgs);
		}
		protected override Color GetDockZoneBorderColor(BaseStackGroupHintInfoArgs e) {
			var ea = e as StackGroupDraggingAdornerInfoArgs;
			if(ea.Owner == null) return base.GetDockZoneBorderColor(e);
			return DrawSkinDockGuideHelper.GetColor(ea.Appearance.BorderColor, "DockZoneBorderColor", ea.Owner.ElementsLookAndFeel);
		}
		protected override Color GetDockZoneBackColor(BaseStackGroupHintInfoArgs e) {
			var ea = e as StackGroupDraggingAdornerInfoArgs;
			if(ea.Owner == null) return base.GetDockZoneBackColor(e);
			return DrawSkinDockGuideHelper.GetColor(ea.Appearance.BackColor, "DockZoneBackColor", ea.Owner.ElementsLookAndFeel);
		}
	}
	class StackGroupDraggingAdornerInfoArgs : BaseStackGroupHintInfoArgs {
		BaseView ownerCore;
		public StackGroupDraggingAdornerInfoArgs(BaseView owner) {
			ownerCore = owner;
		}
		public BaseView Owner {
			get { return ownerCore; }
		}
		protected override int CalcCore() { return 0; }
		protected override System.Collections.Generic.IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			return opaque ? new Rectangle[0] : new Rectangle[] { Content };
		}
	} 
}
