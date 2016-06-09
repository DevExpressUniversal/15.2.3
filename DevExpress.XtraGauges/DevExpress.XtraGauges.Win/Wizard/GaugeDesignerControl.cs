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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Primitive;
namespace DevExpress.XtraGauges.Win.Wizard {
	[System.ComponentModel.ToolboxItem(false)]
	public class GaugeDesignerControl : Control {
		NavigationControl navigatorCore;
		CheckEdit showNavigatorCheckCore;
		SimpleButton prevButtonCore;
		SimpleButton nextButtonCore;
		SimpleButton finishButtonCore;
		SimpleButton cancelButtonCore;
		GaugeDesignerControlViewInfo viewInfoCore;
		BaseGaugeDesignerPage[] availablePagesCore;
		BaseGaugeDesignerPageCollection pagesCore;
		BaseGaugeDesignerPage selectedPageCore;
		IGauge gaugeCore;
		int selectedPageIndexCore;
		Brush BackgroundBrush;
		Pen linePen1;
		Pen linePen2;
		public GaugeDesignerControl() {
			SetStyle(
					ControlStyles.SupportsTransparentBackColor |
					ControlConstants.DoubleBuffer |
					ControlStyles.ResizeRedraw |
					ControlStyles.AllPaintingInWmPaint |
					ControlStyles.ResizeRedraw |
					ControlStyles.UserMouse |
					ControlStyles.UserPaint,
					true
				);
			OnCreate();
		}
		protected void OnCreate() {
			this.showNavigatorCheckCore = new CheckEdit();
			this.navigatorCore = new NavigationControl();
			this.prevButtonCore = new SimpleButton();
			this.nextButtonCore = new SimpleButton();
			this.finishButtonCore = new SimpleButton();
			this.cancelButtonCore = new SimpleButton();
			PrevButton.Text = "Prev";
			NextButton.Text = "Next";
			FinishButton.Text = "Finish";
			CancelButton.Text = "Cancel";
			PrevButton.Parent = this;
			NextButton.Parent = this;
			FinishButton.Parent = this;
			CancelButton.Parent = this;
			PrevButton.Click += OnPrevPageBtnClick;
			NextButton.Click += OnNextPageBtnClick;
			FinishButton.Click += OnFinishBtnClick;
			CancelButton.Click += OnCancelBtnClick;
			ShowNavigatorCheckEdit.Parent = this;
			ShowNavigatorCheckEdit.Text = "Show Navigator";
			ShowNavigatorCheckEdit.Checked = true;
			ShowNavigatorCheckEdit.EditValueChanged += OnShowNavigatorChanged;
			Navigator.Parent = this;
			Navigator.SetDesignerControl(this);
			Navigator.Nodes.AddRange(
						new PageNavigationNode[]{
							new PageNavigationNode(null,"Main",null),
							new PageNavigationNode(null,"Elements",null),
							new PageNavigationNode(null, "Bindings", null)
						}
					);
			this.Dock = DockStyle.Fill;
			this.TabStop = false;
			this.viewInfoCore = CreateViewInfo();
			this.pagesCore = new BaseGaugeDesignerPageCollection();
			this.selectedPageCore = null;
			Pages.CollectionChanged += OnPagesChanged;
			LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
			SetSelectedPageByIndex(0);
			UpdateStyle();
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			UpdateStyle();
		}
		void UpdateStyle() {
			Color bgColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
			Color lineColor1 = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlDark);
			Color lineColor2 = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlLightLight);
			this.BackgroundBrush = new SolidBrush(bgColor);
			this.linePen1 = new Pen(lineColor1);
			this.linePen2 = new Pen(lineColor2);
		}
		void OnShowNavigatorChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.selectedPageCore = null;
				if(ViewInfo != null) {
					ViewInfo.Dispose();
					viewInfoCore = null;
				}
				if(Pages != null) {
					Pages.CollectionChanged -= OnPagesChanged;
					Pages.Clear();
					Pages.Dispose();
					pagesCore = null;
				}
				if(Navigator != null) {
					Navigator.Parent = null;
					Navigator.Dispose();
					navigatorCore = null;
				}
				if(ShowNavigatorCheckEdit != null) {
					ShowNavigatorCheckEdit.EditValueChanged -= OnShowNavigatorChanged;
					ShowNavigatorCheckEdit.Parent = null;
					ShowNavigatorCheckEdit.Dispose();
					showNavigatorCheckCore = null;
				}
				if(ShowNavigatorCheckEdit != null) {
					ShowNavigatorCheckEdit.EditValueChanged -= OnShowNavigatorChanged;
					ShowNavigatorCheckEdit.Parent = null;
					ShowNavigatorCheckEdit.Dispose();
					showNavigatorCheckCore = null;
				}
				Cancel = null;
				Finish = null;
			}
			base.Dispose(disposing);
		}
		public event EventHandler Cancel;
		public event EventHandler Finish;
		public UserLookAndFeel LookAndFeel {
			get { return UserLookAndFeel.Default; }
		}
		public IGauge Gauge {
			get { return gaugeCore; }
		}
		public bool ShowNavigator {
			get { return ShowNavigatorCheckEdit.Checked; }
		}
		public bool ShowPrevNextButtons {
			get { return !ShowNavigatorCheckEdit.Checked; }
		}
		internal void SetGaugeCore(IGauge gauge) {
			this.gaugeCore = gauge;
		}
		protected internal NavigationControl Navigator {
			get { return navigatorCore; }
		}
		void OnPagesChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<BaseGaugeDesignerPage> e) {
			ResetAvailablePages();
			switch(e.ChangedType) {
				case ElementChangedType.ElementAdded: OnPageAdded(e.Element); break;
				case ElementChangedType.ElementRemoved: OnPageRemoved(e.Element); break;
			}
		}
		protected virtual void OnPageAdded(BaseGaugeDesignerPage page) {
			page.SetDesignerControl(this);
			Controls.Add(page);
			Navigator.Nodes[page.NavigationGroup].Nodes.Add(new PageNavigationNode(page, page.Caption, page.Image));
			Navigator.LayoutChanged();
			LayoutChanged();
		}
		protected virtual void OnPageRemoved(BaseGaugeDesignerPage page) {
			Controls.Remove(page);
			page.SetDesignerControl(null);
			LayoutChanged();
		}
		protected virtual GaugeDesignerControlViewInfo CreateViewInfo() {
			return new GaugeDesignerControlViewInfo(this);
		}
		public BaseGaugeDesignerPageCollection Pages {
			get { return pagesCore; }
		}
		public BaseGaugeDesignerPage FirstPage {
			get { return AvailablePages.Length > 0 ? AvailablePages[0] : null; }
		}
		public BaseGaugeDesignerPage LastPage {
			get { return AvailablePages.Length > 0 ? AvailablePages[AvailablePages.Length - 1] : null; }
		}
		protected internal BaseGaugeDesignerPage[] AvailablePages {
			get {
				if(availablePagesCore == null) availablePagesCore = GetAvailablePages();
				return availablePagesCore;
			}
		}
		protected void ResetAvailablePages() {
			this.availablePagesCore = null;
		}
		BaseGaugeDesignerPage[] GetAvailablePages() {
			List<BaseGaugeDesignerPage> availPages = new List<BaseGaugeDesignerPage>();
			Pages.Accept(
				delegate(BaseGaugeDesignerPage page) {
					if(page.IsAllowed && !page.IsHidden) availPages.Add(page);
				}
			);
			return availPages.ToArray();
		}
		protected CheckEdit ShowNavigatorCheckEdit {
			get { return showNavigatorCheckCore; }
		}
		protected SimpleButton PrevButton {
			get { return prevButtonCore; }
		}
		protected SimpleButton NextButton {
			get { return nextButtonCore; }
		}
		protected SimpleButton FinishButton {
			get { return finishButtonCore; }
		}
		protected SimpleButton CancelButton {
			get { return cancelButtonCore; }
		}
		public bool IsPrevAvail {
			get { return FirstPage != null && SelectedPage != FirstPage; }
		}
		public bool IsNextAvail {
			get { return LastPage != null && SelectedPage != LastPage; }
		}
		public int SelectedPageIndex {
			get {
				if(Pages.Count == 0) return -1;
				return selectedPageIndexCore;
			}
		}
		public BaseGaugeDesignerPage SelectedPage {
			get {
				if(selectedPageCore == null) selectedPageCore = FirstPage;
				return selectedPageCore;
			}
		}
		public void SetSelectedPage(BaseGaugeDesignerPage page) {
			ResetAvailablePages();
			if(AvailablePages.Length == 0) return;
			if(!Pages.Contains(page) || !page.IsAllowed) return;
			for(int i = 0; i < AvailablePages.Length; i++) {
				if(AvailablePages[i] == page) {
					this.selectedPageIndexCore = i;
					this.selectedPageCore = page;
					LayoutChanged();
					break;
				}
			}
		}
		protected internal void SetSelectedPageByIndex(int index) {
			ResetAvailablePages();
			if(AvailablePages.Length == 0 || selectedPageIndexCore == index) return;
			index = Math.Max(0, index);
			index = Math.Min(AvailablePages.Length - 1, index);
			this.selectedPageIndexCore = index;
			this.selectedPageCore = AvailablePages[index];
			LayoutChanged();
		}
		public void LayoutChanged() {
			ViewInfo.SetDirty();
			ViewInfo.CalcInfo(null, Bounds);
			ShowNavigatorCheckEdit.Bounds = ViewInfo.Rects.ShowNavigatorCheck;
			PrevButton.Bounds = ViewInfo.Rects.PrevButton;
			NextButton.Bounds = ViewInfo.Rects.NextButton;
			FinishButton.Bounds = ViewInfo.Rects.FinishButton;
			CancelButton.Bounds = ViewInfo.Rects.CancelButton;
			Navigator.Bounds = ViewInfo.Rects.Navigator;
			foreach(BaseGaugeDesignerPage page in Pages) {
				if(page != SelectedPage) {
					if(page.Visible) {
						page.Size = Size.Empty;
						page.Location = new Point(-10000, -10000);
						page.Visible = false;
					}
				}
			}
			if(SelectedPage != null) {
				SelectedPage.Visible = true;
				SelectedPage.Bounds = ViewInfo.Rects.Page;
				Form designerForm = this.FindForm();
				if(designerForm != null) {
					designerForm.Text = string.Format(
							"{0} - Element Designer", SelectedPage.Caption
						);
				}
				SelectedPage.LayoutChanged();
			}
			Invalidate();
		}
		protected internal GaugeDesignerControlViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				GraphicsInfoArgs ea = new GraphicsInfoArgs(cache, e.ClipRectangle);
				UpdateBeforePaint(ea);
				DrawBackground(ea);
			}
		}
		protected void UpdateBeforePaint(GraphicsInfoArgs e) {
			if(!ViewInfo.IsReady) ViewInfo.CalcInfo(e.Graphics, Bounds);
			if(PrevButton.Enabled != IsPrevAvail) PrevButton.Enabled = IsPrevAvail;
			if(NextButton.Enabled != IsNextAvail) NextButton.Enabled = IsNextAvail;
			string finishCaption = ShowPrevNextButtons ? "Finish" : "Ok";
			if(FinishButton.Text != finishCaption) FinishButton.Text = finishCaption;
		}
		protected void DrawBackground(GraphicsInfoArgs e) {
			e.Graphics.FillRectangle(BackgroundBrush, ViewInfo.Rects.Bounds);
			DrawStyleLine(e, ViewInfo.Rects.Footer.Location, new Point(ViewInfo.Rects.Footer.Right, ViewInfo.Rects.Footer.Top));
			DrawStyleLine(e, new Point(ViewInfo.Rects.Navigator.Right + 1, ViewInfo.Rects.Navigator.Top), new Point(ViewInfo.Rects.Navigator.Right + 1, ViewInfo.Rects.Navigator.Bottom));
		}
		void DrawStyleLine(GraphicsInfoArgs e, Point pt1, Point pt2) {
			bool horz = (pt1.Y == pt2.Y);
			if(LookAndFeel.Style != LookAndFeelStyle.Skin) {
				Point pt3 = new Point(horz ? pt1.X : pt1.X + 1, horz ? pt1.Y + 1 : pt1.Y);
				Point pt4 = new Point(horz ? pt2.X : pt2.X + 1, horz ? pt2.Y + 1 : pt2.Y);
				e.Graphics.DrawLine(linePen1, pt1, pt2);
				e.Graphics.DrawLine(linePen2, pt3, pt4);
			}
			else {
				SkinElementInfo info = new SkinElementInfo(
					CommonSkins.GetSkin(LookAndFeel)[horz ? CommonSkins.SkinLabelLine : CommonSkins.SkinLabelLineVert],
					new Rectangle(pt1, horz ? new Size(pt2.X - pt1.X, 2) : new Size(2, pt2.Y - pt1.Y))
					);
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		protected void OnCancelBtnClick(object sender, EventArgs ea) {
			if(Cancel != null) Cancel(this, EventArgs.Empty);
			CloseDesignerForm();
		}
		protected void OnFinishBtnClick(object sender, EventArgs ea) {
			if(Finish != null) Finish(this, EventArgs.Empty);
			CloseDesignerForm();
		}
		protected void OnPrevPageBtnClick(object sender, EventArgs ea) {
			SetSelectedPageByIndex(SelectedPageIndex - 1);
		}
		protected void OnNextPageBtnClick(object sender, EventArgs ea) {
			SetSelectedPageByIndex(SelectedPageIndex + 1);
		}
		void CloseDesignerForm() {
			Form designerForm = this.FindForm();
			if(designerForm != null) designerForm.Close();
		}
		public void RemoveDesignedElement(BaseElement<IRenderableElement> designedElement) {
			if(SelectedPage == null) return;
			BaseElement<IRenderableElement> elementToRemove = SelectedPage.GetElementByDesignedClone(designedElement);
			if(elementToRemove != null) {
				if(!SelectedPage.ProcessElementRemoveCommand(designedElement, elementToRemove)) return;
				if(!SelectedPage.IsAllowed) SetSelectedPageByIndex(0);
				Gauge.RemoveGaugeElement(elementToRemove);
				SelectedPage.UpdateContent();
				Navigator.UpdateContent();
			}
		}
		public BaseElement<IRenderableElement> DuplicateDesignedElement(BaseElement<IRenderableElement> designedElement) {
			BaseElement<IRenderableElement> duplicate = null;
			if(SelectedPage == null || !SelectedPage.ProcessElementDuplicateCommand(designedElement, out duplicate)) return null;
			if(duplicate != null) {
				Gauge.AddGaugeElement(duplicate);
				SelectedPage.UpdateContent();
				Navigator.UpdateContent();
			}
			return duplicate;
		}
	}
}
