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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraGauges.Core.Base;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGauges.Win.Base;
namespace DevExpress.XtraGauges.Win.Wizard {
	public class GaugeDesignerControlViewInfo : BaseViewInfo {
		DesignerControlViewRects viewRectsCore;
		GaugeDesignerControl ownerCore;
		public GaugeDesignerControlViewInfo(GaugeDesignerControl owner)
			: base() {
			this.ownerCore = owner;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.viewRectsCore = new DesignerControlViewRects();
		}
		protected override void OnDispose() {
			this.viewRectsCore = null;
			this.ownerCore = null;
			base.OnDispose();
		}
		protected GaugeDesignerControl Owner {
			get { return ownerCore; }
		}
		public DesignerControlViewRects Rects {
			get { return viewRectsCore; }
		}
		protected override void CalcViewStates() { }
		protected override void CalcViewRects(Rectangle bounds) {
			Rects.Clear();
			Rects.Bounds = Rects.Page = bounds;
			int footerHeight = 44;
			Size btnSize = new Size(76, 22);
			int p = (footerHeight - btnSize.Height) / 2;
			int navigatorWidth = 150;
			if(bounds.Height > footerHeight) {
				if(Owner.ShowNavigator) {
					Rects.Navigator = new Rectangle(bounds.Left, bounds.Top, navigatorWidth, bounds.Height - footerHeight);
					Rects.Page = new Rectangle(bounds.Left + navigatorWidth + 3, bounds.Top, bounds.Width - navigatorWidth - 3, bounds.Height - footerHeight);
				}
				else {
					Rects.Navigator = new Rectangle(-10000, -10000, 0, 0);
					Rects.Page = new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height - footerHeight);
				}
				Rectangle footer = new Rectangle(bounds.Left, bounds.Top + bounds.Height - footerHeight, bounds.Width, footerHeight);
				Rectangle btn0 = new Rectangle(footer.Right - (btnSize.Width + 4) * 4 - (p - 5), footer.Top + p, btnSize.Width, btnSize.Height);
				Rectangle btn1 = new Rectangle(footer.Right - (btnSize.Width + 4) * 3 - (p - 5), footer.Top + p, btnSize.Width, btnSize.Height);
				Rectangle btn2 = new Rectangle(footer.Right - (btnSize.Width + 4) * 2 - (p - 5), footer.Top + p, btnSize.Width, btnSize.Height);
				Rectangle btn3 = new Rectangle(footer.Right - (btnSize.Width + 4) * 1 - (p - 5), footer.Top + p, btnSize.Width, btnSize.Height);
				Rectangle check = new Rectangle(footer.Left + p, footer.Top + p, btn0.Left - footer.Left - p - p, btnSize.Height);
				if(check.Width > navigatorWidth) check.Width = navigatorWidth;
				Rects.Footer = footer;
				Rects.ShowNavigatorCheck = check;
				if(footer.Width > ((btnSize.Width + 4) * 4 + p) + navigatorWidth) {
					if(Owner.ShowPrevNextButtons) Rects.PrevButton = btn0;
					if(Owner.ShowPrevNextButtons) Rects.NextButton = btn1;
					Rects.FinishButton = btn2;
					Rects.CancelButton = btn3;
				}
			}
		}
		public GaugeDesignerControlHitInfo CalcHitInfo(Point pt) {
			GaugeDesignerControlHitInfo hitInfo = new GaugeDesignerControlHitInfo(pt);
			if(hitInfo.CheckAndSetHitTest(Rects.Bounds, GaugeDesignerControlHitTest.Bounds)) {
				bool inFooter = hitInfo.CheckAndSetHitTest(Rects.Footer, GaugeDesignerControlHitTest.Footer);
				if(inFooter) {
					hitInfo.CheckAndSetHitTest(Rects.ShowNavigatorCheck, GaugeDesignerControlHitTest.NavigatorButton);
					hitInfo.CheckAndSetHitTest(Rects.PrevButton, GaugeDesignerControlHitTest.PrevButton);
					hitInfo.CheckAndSetHitTest(Rects.NextButton, GaugeDesignerControlHitTest.NextButton);
					hitInfo.CheckAndSetHitTest(Rects.FinishButton, GaugeDesignerControlHitTest.FinishButton);
					hitInfo.CheckAndSetHitTest(Rects.CancelButton, GaugeDesignerControlHitTest.CancelButton);
				} else hitInfo.CheckAndSetHitTest(Rects.Page, GaugeDesignerControlHitTest.Page);
			}
			return hitInfo;
		}
	}
	public class DesignerControlViewRects {
		public Rectangle Bounds;
		public Rectangle ShowNavigatorCheck;
		public Rectangle Navigator;
		public Rectangle Page;
		public Rectangle Footer;
		public Rectangle NextButton;
		public Rectangle PrevButton;
		public Rectangle FinishButton;
		public Rectangle CancelButton;
		public void Clear() {
			Bounds = Navigator = Page = Footer = 
			ShowNavigatorCheck = NextButton = PrevButton = 
			FinishButton = CancelButton
			=  Rectangle.Empty;
		}
	}
	public enum GaugeDesignerControlHitTest {
		None, Bounds, Navigator, NavigatorButton, Page, Footer, PrevButton, NextButton, CancelButton, FinishButton
	}
	public class GaugeDesignerControlHitInfo {
		public static readonly GaugeDesignerControlHitInfo Empty;
		static GaugeDesignerControlHitInfo() {
			Empty = new GaugeDesignerControlEmptyHitInfo();
		}
		class GaugeDesignerControlEmptyHitInfo : GaugeDesignerControlHitInfo {
			public GaugeDesignerControlEmptyHitInfo() : base(new Point(-10000, -10000)) { }
		}
		GaugeDesignerControlHitTest hitTestCore;
		Rectangle hitRectCore;
		Point hitPointCore;
		public GaugeDesignerControlHitInfo(Point hitPoint) {
			this.hitPointCore = hitPoint;
			this.hitTestCore = GaugeDesignerControlHitTest.None;
		}
		public Rectangle HitRect {
			get { return hitRectCore; }
		}
		public Point HitPoint {
			get { return hitPointCore; }
		}
		public GaugeDesignerControlHitTest HitTest {
			get { return hitTestCore; }
		}
		public bool IsEmpty {
			get { return this is GaugeDesignerControlEmptyHitInfo; }
		}
		public bool CheckAndSetHitTest(Rectangle bounds, GaugeDesignerControlHitTest hitTest) {
			bool contains = !bounds.IsEmpty && bounds.Contains(HitPoint);
			if(contains) {
				hitRectCore = bounds;
				hitTestCore = hitTest;
			}
			return contains;
		}
		public bool InBounds {
			get { return (HitTest == GaugeDesignerControlHitTest.Bounds) || InNavigator || InPage || InFooter; }
		}
		public bool InNavigator {
			get { return (HitTest == GaugeDesignerControlHitTest.Navigator); }
		}
		public bool InPage {
			get { return (HitTest == GaugeDesignerControlHitTest.Page); }
		}
		public bool InFooter {
			get { return (HitTest == GaugeDesignerControlHitTest.Footer) || InButton; }
		}
		public bool InButton {
			get { return InNavigatorButton || InPrevButton || InNextButton || InFinishButton || InCancelButton; }
		}
		public bool InNavigatorButton {
			get { return (HitTest == GaugeDesignerControlHitTest.NavigatorButton); }
		}
		public bool InPrevButton {
			get { return (HitTest == GaugeDesignerControlHitTest.PrevButton); }
		}
		public bool InNextButton {
			get { return (HitTest == GaugeDesignerControlHitTest.NextButton); }
		}
		public bool InFinishButton {
			get { return (HitTest == GaugeDesignerControlHitTest.FinishButton); }
		}
		public bool InCancelButton {
			get { return (HitTest == GaugeDesignerControlHitTest.CancelButton); }
		}
		public virtual bool IsEquals(GaugeDesignerControlHitInfo hitInfo) {
			return hitInfo!=null && !hitInfo.IsEmpty && this.HitTest == hitInfo.HitTest;
		}
	}
}
