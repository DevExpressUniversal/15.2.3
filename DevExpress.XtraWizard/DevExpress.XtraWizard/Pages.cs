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
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Drawing.Design;
using DevExpress.XtraWizard.Localization;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using System.ComponentModel.Design;
namespace DevExpress.XtraWizard {
	[ToolboxItem(false),
	Designer("DevExpress.XtraWizard.Design.WizardPageDesigner, " + AssemblyInfo.SRAssemblyWizardDesign, typeof(IDesigner)),
	TypeConverter(typeof(WizardSelectedPageConverter))]
	public class BaseWizardPage : ContainerControl {
		#region Events
		static readonly object pageValidating = new object();
		static readonly object pageCommit = new object();
		static readonly object pageRollback = new object();
		static readonly object pageInit = new object();
		[Category("Page Validation")]
		public event WizardPageValidatingEventHandler PageValidating {
			add { Events.AddHandler(pageValidating, value); }
			remove { Events.RemoveHandler(pageValidating, value); }
		}
		[Category("Page Validation")]
		public event EventHandler PageCommit {
			add { Events.AddHandler(pageCommit, value); }
			remove { Events.RemoveHandler(pageCommit, value); }
		}
		[Category("Page Validation")]
		public event EventHandler PageRollback {
			add { Events.AddHandler(pageRollback, value); }
			remove { Events.RemoveHandler(pageRollback, value); }
		}
		[Category("Page Validation")]
		public event EventHandler PageInit {
			add { Events.AddHandler(pageInit, value); }
			remove { Events.RemoveHandler(pageInit, value); }
		}
		#endregion 
		WizardControl owner;
		bool allowNext, allowBack, allowCancel, allowFinish;
		bool visible;
		public BaseWizardPage() {
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			owner = null;
			this.visible = true;
			this.Size = Size.Empty;
			this.CausesValidation = false;
			this.allowCancel = this.allowNext = this.allowBack = this.allowFinish = true;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Owner != null) Owner.Pages.Remove(this);
			}
			base.Dispose(disposing);
		}
		[DefaultValue(true)]
		public bool AllowNext {
			get { return allowNext; }
			set {
				if(AllowNext == value) return;
				allowNext = value;
				UpdateButtons();
			}
		}
		[DefaultValue(true)]
		public bool AllowBack {
			get { return allowBack; }
			set {
				if(AllowBack == value) return;
				allowBack = value;
				UpdateButtons();
			}
		}
		[DefaultValue(true)]
		public bool AllowCancel {
			get { return allowCancel; }
			set {
				if(AllowCancel == value) return;
				allowCancel = value;
				UpdateButtons();
			}
		}
		[DefaultValue(true)]
		public bool AllowFinish {
			get { return allowFinish; }
			set {
				if(AllowFinish == value) return;
				allowFinish = value;
				UpdateButtons();
			}
		}
		void UpdateButtons() { 
			if(IsSelectedPage)
				Owner.UpdateButtonsStates();
		}
		[Browsable(false)]
		public virtual WizardControl Owner { get { return owner; } }
		[Localizable(true), Browsable(true)]
		public override string Text {
			get { return base.Text; }
			set {
				base.Text = value;
				UpdateOwner();
			}
		}
		[DefaultValue(true)]
		public new bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				if(!Visible && IsSelectedPage)
					Owner.SelectVisiblePage();
				else
					UpdateOwner();
			}
		}
		internal bool VisibleInternal {
			get { return base.Visible && Visible; }
			set {
				base.Visible = value;
			}
		}
		[DefaultValue(false)]
		public new bool CausesValidation {
			get { return base.CausesValidation; }
			set { base.CausesValidation = value; }
		}
		protected void UpdateOwner() {
			if(IsSelectedPage) {
				Owner.LayoutChanged();
				Owner.Update();
			}
		}
		internal void SetWizardControl(WizardControl newOwner) {
			if(Owner != null && newOwner != null) return;
			this.owner = newOwner;
		}
		protected internal virtual WizardPageValidatingEventArgs RaisePageValidating(Direction direction) {
			WizardPageValidatingEventHandler handler = (WizardPageValidatingEventHandler)this.Events[pageValidating];
			WizardPageValidatingEventArgs e = new WizardPageValidatingEventArgs(direction);
			if(handler != null) handler(this, e);
			return e;
		}
		protected internal virtual void RaisePageCommit() {
			EventHandler handler = (EventHandler)this.Events[pageCommit];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaisePageRollback() {
			EventHandler hander = (EventHandler)this.Events[pageRollback];
			if(hander != null) hander(this, EventArgs.Empty);
		}
		protected internal virtual void RaisePageInit() {
			EventHandler handler = (EventHandler)this.Events[pageInit];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		#region hidden
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = DockStyle.None; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int TabIndex {
			get { return base.TabIndex; }
			set { base.TabIndex = 0; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = true; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override AnchorStyles Anchor {
			get { return base.Anchor; }
			set { base.Anchor = AnchorStyles.None; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Location {
			get { return base.Location; }
			set { base.Location = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor {
			get { return base.BackColor; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get {
				if(Owner != null && Owner.ViewInfo.IsWizardAeroStyle)
					return GetDefaultForeColor();
				return base.ForeColor;
			}
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font {
			get { return base.Font; }
			set { }
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(IsSelectedPage)
				Owner.Painter.DrawWizardPage(Owner.ViewInfo, this, e);
			else
				base.OnPaint(e);
		}
		protected bool IsSelectedPage { get { return Owner != null && Owner.SelectedPage == this; } }
		protected Color GetDefaultForeColor() {
			return LookAndFeelHelper.GetSystemColor(Owner.LookAndFeel, SystemColors.WindowText);
		}
		#endregion
	}
	public class BaseWelcomeWizardPage : BaseWizardPage {
		string proceedText;
		protected BaseWelcomeWizardPage() {
			this.proceedText = WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageProceedText); ;
		}
		[Localizable(true), Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)), Category("Appearance")]
		public virtual string ProceedText {
			get { return proceedText; }
			set {
				if(value == null) value = string.Empty;
				if(ProceedText == value) return;
				proceedText = value;
				UpdateOwner();
			}
		}
		bool ShouldSerializeProceedText() { return ProceedText != WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageProceedText); }
		void ResetProceedText() { ProceedText = WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageProceedText); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get {
				if(Owner != null)
					return GetDefaultForeColor();
				return base.ForeColor;
			}
			set { }
		}
	}
	public class WelcomeWizardPage : BaseWelcomeWizardPage {
		string introductionText;
		public WelcomeWizardPage() {
			this.introductionText = WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageIntroductionText);
			this.Text = WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageTitleText);
		}
		[Localizable(true), Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)), Category("Appearance")]
		public string IntroductionText {
			get { return introductionText; }
			set {
				if(value == null) value = string.Empty;
				if(IntroductionText == value) return;
				introductionText = value;
				UpdateOwner();
			}
		}
		bool ShouldSerializeIntroductionText() { return IntroductionText != WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageIntroductionText); }
		void ResetIntroductionText() { IntroductionText = WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageIntroductionText); }
		[Localizable(true)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		bool ShouldSerializeText() { return Text != WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageTitleText); }
		new void ResetText() { Text = WizardControl.GetLocalizedDefaultString(WizardStringId.WelcomePageTitleText); }
	}
	public class CompletionWizardPage : BaseWelcomeWizardPage {
		string finishText;
		public CompletionWizardPage() {
			base.ProceedText = WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageProceedText);
			this.finishText = WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageFinishText);
			this.Text = WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageTitleText);
		}
		[Localizable(true), Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)), Category("Appearance")]
		public override string ProceedText {
			get { return base.ProceedText; }
			set { base.ProceedText = value; }
		}
		bool ShouldSerializeProceedText() { return ProceedText != WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageProceedText); }
		void ResetProceedText() { ProceedText = WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageProceedText); }
		[Localizable(true), Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)), Category("Appearance")]
		public string FinishText {
			get { return finishText; }
			set {
				if(value == null) value = string.Empty;
				if(FinishText == value) return;
				finishText = value;
				UpdateOwner();
			}
		}
		bool ShouldSerializeFinishText() { return FinishText != WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageFinishText); }
		void ResetFinishText() { FinishText = WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageFinishText); }
		[Localizable(true)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		bool ShouldSerializeText() { return Text != WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageTitleText); }
		new void ResetText() { Text = WizardControl.GetLocalizedDefaultString(WizardStringId.CompletionPageTitleText); }
	}
	public class WizardPage : BaseWizardPage {
		string descriptionText;
		public WizardPage() {
			this.descriptionText = WizardControl.GetLocalizedDefaultString(WizardStringId.PageDescriptionText);
			this.Text = WizardControl.GetLocalizedDefaultString(WizardStringId.InteriorPageTitleText);
		}
		[Localizable(true), Category("Appearance")]
		public string DescriptionText {
			get { return descriptionText; }
			set {
				if(value == null) value = string.Empty;
				if(DescriptionText == value) return;
				descriptionText = value;
				UpdateOwner();
			}
		}
		bool ShouldSerializeDescriptionText() { return DescriptionText != WizardControl.GetLocalizedDefaultString(WizardStringId.PageDescriptionText); }
		void ResetDescriptionText() { DescriptionText = WizardControl.GetLocalizedDefaultString(WizardStringId.PageDescriptionText); }
		[Localizable(true)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		bool ShouldSerializeText() { return Text != WizardControl.GetLocalizedDefaultString(WizardStringId.InteriorPageTitleText); }
		new void ResetText() { Text = WizardControl.GetLocalizedDefaultString(WizardStringId.InteriorPageTitleText); }
	}
	[ListBindable(false)]
	public class WizardPageCollection : CollectionBase, IList {
		WizardControl wizardControl;
		internal int lastRemovedPageIndex = -1;
		public WizardPageCollection(WizardControl control) {
			this.wizardControl = control;
		}
		public event CollectionChangeEventHandler CollectionChanged;
		public virtual BaseWizardPage this[int index] { get { return List[index] as BaseWizardPage; } }
		public virtual WizardControl WizardControl { get { return wizardControl; } }
		protected virtual BaseWizardPage CreatePage() { return new BaseWizardPage(); }
		public virtual BaseWizardPage Add() { return Add(""); }
		public virtual BaseWizardPage Add(string text) {
			BaseWizardPage page = CreatePage();
			if(text != string.Empty) page.Text = text;
			Add(page);
			return page;
		}
		public virtual void AddRange(BaseWizardPage[] pages) {
			foreach(BaseWizardPage page in pages) Add(page);
		}
		public virtual void Add(BaseWizardPage page) {
			if(Contains(page)) return;
			List.Add(page);
		}
		public virtual bool Insert(int position, BaseWizardPage page) {
			if(List.Contains(page)) return false;
			List.Insert(position, page);
			return true;
		}
		public virtual bool Insert(BaseWizardPage cPage, BaseWizardPage page) {
			if(List.Contains(page) ||
				!List.Contains(cPage)) return false;
			return Insert(List.IndexOf(cPage), page);
		}
		public virtual void Remove(BaseWizardPage page) {
			if(List.Contains(page)) List.Remove(page);
		}
		public virtual bool Contains(BaseWizardPage page) { return List.Contains(page); }
		public virtual int IndexOf(BaseWizardPage page) { return List.IndexOf(page); }
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			BaseWizardPage page = value as BaseWizardPage;
			page.SetWizardControl(WizardControl);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, page));
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			BaseWizardPage page = value as BaseWizardPage;
			lastRemovedPageIndex = position;
			page.SetWizardControl(null);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, page));
		}
	}
	public class WizardSelectedPageConverter : ComponentConverter {
		public WizardSelectedPageConverter(Type type) : base(type) { }
		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value) {
			if(context == null || context.Instance == null) return false;
			WizardControl control = context.Instance as WizardControl;
			BaseWizardPage page = value as BaseWizardPage;
			return page == null || (control != null && page != null && page.Owner == control);
		}
	}
}
