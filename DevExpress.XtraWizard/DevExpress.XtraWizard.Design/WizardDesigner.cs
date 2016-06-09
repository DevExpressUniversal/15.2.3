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
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using System.Drawing.Drawing2D;
using DevExpress.XtraWizard.Localization;
using DevExpress.Utils.About;
namespace DevExpress.XtraWizard.Design {
	public class WizardControlDesigner : ParentControlDesigner {
		DesignerVerbCollection verbs;
		frmPageDesigner editor = null;
		DesignerVerb insertPage, removePage, addWelcomePage, addCompletionPage;
		public virtual WizardControl WizardControl { get { return Control as WizardControl; } }
		public BaseWizardPage SelectedPage { 
			get {
				if(WizardControl == null) return null;
				return WizardControl.SelectedPage; 
			} 
		}
		protected frmPageDesigner Editor {
			get { return editor; }
			set {
				if(Editor == value) return;
				if(Editor != null) Editor.Dispose();
				editor = value;
			}
		}
		protected static ArrayList Designers = new ArrayList();
		public WizardControlDesigner() {
			Designers.Add(this);
			this.verbs = null;
		}
		void CreateVerbs() {
			if(this.verbs != null) return;
			this.verbs = new DesignerVerbCollection();
			this.verbs.Add(new DesignerVerb("Choose Page", new EventHandler(OnVerb_ChoosePage)));
			this.verbs.Add(new DesignerVerb("Add Page", new EventHandler(OnVerb_AddPage)));
			this.insertPage = new DesignerVerb("Insert Page", new EventHandler(OnVerb_InsertPage));
			this.removePage = new DesignerVerb("Remove Page", new EventHandler(OnVerb_RemovePage));
			this.addWelcomePage = new DesignerVerb("Add Welcome Page", new EventHandler(OnVerb_AddWelcomePage));
			this.addCompletionPage = new DesignerVerb("Add Completion Page", new EventHandler(OnVerb_AddCopmletionPage));
			this.verbs.Add(insertPage);
			this.verbs.Add(removePage);
			this.verbs.Add(addWelcomePage);
			this.verbs.Add(addCompletionPage);
			DXSmartTagsHelper.CreateDefaultVerbs(this, this.verbs);
			UpdateVerbs();
		}
		protected virtual void OnVerb_ChoosePage(object sender, EventArgs e) {
			if(Editor == null) 
				editor = new frmPageDesigner(WizardControl);
			try {
				if(Editor.ShowDialog() == DialogResult.Cancel)
					WizardControl.SelectedPage = Editor.PrevSelectedPage;
				else Editor.ApplyPagePositions();
			}
			finally {
				Editor = null;
			}
		}
		protected virtual void OnVerb_AddWelcomePage(object sender, EventArgs e) {
			AddWelcomeWizardPage();
		}
		protected virtual void OnVerb_AddCopmletionPage(object sender, EventArgs e) {
			AddCompletionWizardPage();
		}
		protected virtual void OnVerb_AddPage(object sender, EventArgs e) {
			BaseWizardPage iPage = null;
			if(WizardControl.Pages.Count > 0 && WizardControl.Pages[WizardControl.Pages.Count - 1] is CompletionWizardPage)
				iPage = WizardControl.Pages[WizardControl.Pages.Count - 1];
			AddPage(iPage);
		}
		protected virtual void OnVerb_InsertPage(object sender, EventArgs e) {
			AddPage(SelectedPage);
		}
		protected virtual void OnVerb_RemovePage(object sender, EventArgs e) {
			if(SelectedPage == null || SelectedPage is WelcomeWizardPage || SelectedPage is CompletionWizardPage) return;
			SelectedPage.Dispose();
			SelectComponent(WizardControl.SelectedPage);
		}
		public override DesignerVerbCollection Verbs {
			get {
				CreateVerbs();
				return verbs;
			}
		}
		protected virtual void UpdateVerbs() {
			if(this.verbs == null) return;
			insertPage.Visible = !(SelectedPage is WelcomeWizardPage) && !(SelectedPage == null);
			removePage.Visible = SelectedPage != null && !(SelectedPage is WelcomeWizardPage) && !(SelectedPage is CompletionWizardPage);
			addWelcomePage.Visible = !WizardControl.IsWelcomePageCreated;
			addCompletionPage.Visible = !WizardControl.IsCompletionPageCreated;
		}
		IDesignerHost host;
		public override void Initialize(IComponent component) {
			DesignTimeHelper.UpdateDesignTimeLookAndFeel(component);
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			LoaderPatcherService.InstallService(host);
			UpdateVerbs();
			WizardControl.SelectedPageChanged += new WizardPageChangedEventHandler(OnWizardControl_SelectedPageChanged);
		}
		protected override void Dispose(bool disposing) {
			LoaderPatcherService.UnInstallService(host);
			this.host = null;
			if(disposing) {
				WizardControl.SelectedPageChanged -= new WizardPageChangedEventHandler(OnWizardControl_SelectedPageChanged);
			}
			base.Dispose(disposing);
		}
		protected ISelectionService SelectionService {
			get {
				return GetService(typeof(ISelectionService)) as ISelectionService;
			}
		}
		protected virtual void OnWizardControl_SelectedPageChanged(object sender, WizardPageChangedEventArgs e) {
			if(WizardControl.IsLockUpdate) return;
			SelectComponent(e.Page);
		}
		public override bool CanParent(Control control) {
			return (control is BaseWizardPage);
		}
		WizardPage AddPage(BaseWizardPage iPage) {
			if(iPage == null) {
				WizardPage page = new WizardPage();
				AddPageToWizard(page);
				return page;
			}
			else {
				if(iPage is WelcomeWizardPage) return null;
				WizardPage page = new WizardPage();
				if(InsertPageToWizard(iPage, page))
					return page;
			}
			return null;
		}
		void SelectComponent(Component p) {
			if(SelectionService != null) {
				SelectionService.SetSelectedComponents(new object[] { WizardControl }, SelectionTypes.Primary);
				if(p != null && p.Site != null) {
					SelectionService.SetSelectedComponents(new object[] { p });
				}
			}
			UpdateVerbs();
		}
		void AddPageToContainer(BaseWizardPage page) {
			WizardControl.Container.Add(page);
			WizardControl.Invalidate();
			SelectComponent(page);
		}
		bool InsertPageToWizard(BaseWizardPage iPage, BaseWizardPage page) {
			if(WizardControl.Pages.Insert(iPage, page)) {
				AddPageToContainer(page);
				return true;
			}
			return false;
		}
		void AddPageToWizard(BaseWizardPage page) {
			WizardControl.Pages.Add(page);
			AddPageToContainer(page);
		}
		public BaseWizardPage AddWelcomeWizardPage() {
			if(WizardControl.IsWelcomePageCreated) return null;
			WelcomeWizardPage page = new WelcomeWizardPage();
			if(WizardControl.Pages.Count == 0)
				AddPageToWizard(page);
			else InsertPageToWizard(WizardControl.Pages[0], page);
			return page;
		}
		public void AddCompletionWizardPage() {
			if(WizardControl.IsCompletionPageCreated) return;
			AddPageToWizard(new CompletionWizardPage());
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			Control.Text = WizardLocalizer.Active.GetLocalizedString(WizardStringId.WizardTitle);  
			BaseWizardPage page = AddWelcomeWizardPage();
			AddPage(null);
			AddCompletionWizardPage();
			if(page != null)
				WizardControl.SelectedPage = page;
		}
		protected bool IsSelected {
			get {
				if(SelectionService == null) return false;
				if(SelectionService.GetComponentSelected(WizardControl)) return true;
				BaseWizardPage page = SelectionService.PrimarySelection as BaseWizardPage;
				if(page != null && page.Owner == WizardControl) return true;
				return false;
			}
		}
		bool RefreshButtons(bool active)  {
			if(!active)
				WizardControl.RefreshDesignButtons();
			return active;
		}
		protected override bool GetHitTest(Point point) {
			if(!IsSelected) return false;
			point = WizardControl.PointToClient(point);
			WizardHitInfo hitInfo = WizardControl.CalcHitInfo(point);
			if(hitInfo.HitTest == WizardHitTest.NextButton || hitInfo.HitTest == WizardHitTest.PrevButton) 
				return RefreshButtons(true);
			return RefreshButtons(false);
		}
	}
	public class WizardPageDesigner : ScrollableControlDesigner {
		DesignerVerbCollection verbs;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			DesignerActionService service = this.GetService(typeof(DesignerActionService)) as DesignerActionService;
			if(service != null) 
				service.Remove(component);
		}
		void CreateVerbs() {
			if(this.verbs != null) return;
			this.verbs = new DesignerVerbCollection();
		}
		public override DesignerVerbCollection Verbs {
			get {
				CreateVerbs();
				return verbs;
			}
		}
		public override SelectionRules SelectionRules {
			get {
				return (SelectionRules.Visible | SelectionRules.Locked);
			}
		}
		public virtual BaseWizardPage Page {
			get { return Control as BaseWizardPage; }
		}
		public virtual WizardControl WizardControl {
			get {
				if(Page != null) return Page.Owner;
				return null;
			}
		}
		protected ISelectionService SelectionService {
			get {
				return GetService(typeof(ISelectionService)) as ISelectionService;
			}
		}
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			using(Pen pen = new Pen(SystemColors.ControlDark)) {
				pen.DashStyle = DashStyle.Dot;
				Rectangle clientRectangle = Control.ClientRectangle;
				clientRectangle.Width--;
				clientRectangle.Height--;
				pe.Graphics.DrawRectangle(pen, clientRectangle);
			}
			base.OnPaintAdornments(pe);
		}
	}
}
