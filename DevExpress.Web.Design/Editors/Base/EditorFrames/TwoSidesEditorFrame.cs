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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Skins.Info;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
namespace DevExpress.Web.Design {
	public abstract class TwoSidesEditorFrame : EditFrameBase {
		DefaultBoolean isCreateBottomPanel = DefaultBoolean.Default;
		const int LabelCaptionHeight = 36;
		const int TopPanelHeight = 48;
		bool showTopPanel = true;
		public bool IsCreateBottomPanel {
			get {
				if(isCreateBottomPanel == DefaultBoolean.Default)
					return false;
				return isCreateBottomPanel == DefaultBoolean.True;
			}
			set { isCreateBottomPanel = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		public virtual bool ShowTopPanel {
			get { return showTopPanel; }
			set { showTopPanel = value; }
		}
		public SplitContainerControl MainSplitContainer { get; private set; }
		protected PanelControl TopPanel { get; private set; }
		protected PanelControl RightPanel { get; set; }
		protected PanelControl LeftPanel { get; private set; }
		protected PanelControl BottomPanel { get; private set; }
		protected SimpleButton ButtonOk { get; set; }
		protected SimpleButton ButtonCancel { get; set; }
		protected bool HasLabelCaption { get { return lbCaption != null && !string.IsNullOrEmpty(lbCaption.Text); } }
		protected bool PostponeControlsCreated { get; set; }
		static readonly object postponeCreateControls = new object();
		protected event EventHandler PostponeCreateControls {
			add { Events.AddHandler(postponeCreateControls, value); }
			remove { Events.RemoveHandler(postponeCreateControls, value); }
		}
		public override void StoreGlobalProperties(DevExpress.Utils.Design.PropertyStore globalStore) {
			base.StoreGlobalProperties(globalStore);
			StoreLocalProperties(globalStore);
		}
		public override void RestoreGlobalProperties(DevExpress.Utils.Design.PropertyStore globalStore) {
			base.RestoreGlobalProperties(globalStore);
			RestoreLocalProperties(globalStore);
		}
		public override void StoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			base.StoreLocalProperties(localStore);
			localStore.AddProperty(GetPropertyPath("MainSplitterPosition"), MainSplitContainer.SplitterPosition);
		}
		public override void RestoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			MainSplitContainer.SplitterPosition = localStore.RestoreIntProperty(GetPropertyPath("MainSplitterPosition"), 250);
		}
		protected override void CreateInnerControls() {
			SuspendLayout();
			TopPanel = CreateTopPanel();
			MainPanel.Dock = DockStyle.None;
			CreateSplittedPanels();
			CreateBottomPanel();
			ResumeLayout();
			Load += (s, e) => { RaisePostponeCreateControls(null); };
			PostponeCreateControls += (s, e) => { PostponedCreateInnerControls(); };
		}
		PanelControl CreateTopPanel() {
			var topPanel = DesignTimeFormHelper.CreatePanel(this, "TopPanel", DockStyle.Top);
			topPanel.BorderStyle = BorderStyles.NoBorder;
			topPanel.Padding = topPanel.Margin = new System.Windows.Forms.Padding(0);
			topPanel.Height = TopPanelHeight;
			return topPanel;
		}
		protected void RaisePostponeCreateControls(EventArgs e) {
			if(PostponeControlsCreated)
				return;
			MainPanel.Visible = false;
			TopPanel.Visible = false;
			var handler = (EventHandler)Events[postponeCreateControls];
			if(handler != null)
				handler(this, e);
			PostponeControlsCreated = true;
			MainPanel.Visible = true;
			TopPanel.Visible = ShowTopPanel;
			if(!TopPanel.Visible)
				TopPanel.Height = 0;
		}
		void PostponedCreateInnerControls() {
			if(HasLabelCaption) {
				SuspendLayout();
				lbCaption.Parent = TopPanel;
				lbCaption.Dock = DockStyle.Top;
				lbCaption.Height = LabelCaptionHeight;
				TopPanel.Height += LabelCaptionHeight;
				ResumeLayout(false);
			};
		}
		protected void CreateSplittedPanels() {
			MainSplitContainer = new SplitContainerControl() { Name = "MainSplitContainer", Dock = DockStyle.Fill, Parent = MainPanel, FixedPanel = SplitFixedPanel.Panel1 };
			CreateRightPanelPart();
			LeftPanel = MainSplitContainer.Panel1;
			LeftPanel.Name = "MainSplitContainer_LeftPanel";
		}
		protected virtual void CreateRightPanelPart() {
			RightPanel = MainSplitContainer.Panel2;
			RightPanel.Name = "MainSplitContainer_RightPanel";
		}
		protected override void OnSizeChanged() {
			RecalcMainPanelSize();
		}
		protected void RecalcMainPanelSize() {
			MainPanel.Width = Width;
			MainPanel.Top = TopPanel.Top + TopPanel.Height;
			MainPanel.Height = (BottomPanel != null ? BottomPanel.Top : Height) - MainPanel.Top;
		}
		void CreateBottomPanel() {
			if(!IsCreateBottomPanel)
				return;
			BottomPanel = DesignTimeFormHelper.CreatePanel(MainPanel, "BottomPanel", DockStyle.Bottom);
			BottomPanel.Height = 40;
			BottomPanel.TabIndex = 4;
			var buttonSize = new Size(80, 23);
			var top = (BottomPanel.Height - buttonSize.Height) / 2;
			var location = new Point(BottomPanel.Width - (buttonSize.Width + 17) * 2, top);
			ButtonOk = DesignTimeFormHelper.CreateButton(BottomPanel, buttonSize, location, 0, "&OK", DialogResult.OK, null);
			location.X += buttonSize.Width + 17;
			ButtonCancel = DesignTimeFormHelper.CreateButton(BottomPanel, buttonSize, location, 1, "&Cancel", DialogResult.Cancel, null);
		}
	}
	public abstract class EditFrameBase : XtraFrame, ISaveChangesFrame, IEmbeddedFrame {
		protected static string BasePath = "ASPxClasses";
		SplashScreenManager managerWaitingForm;
		public EditFrameBase()
			: base() {
			InitializeFrame();
		}
		protected PanelControl MainPanel { get { return this.pnlMain; } }
		protected abstract string FrameName { get; }
		protected string PropertyStorePath { get { return string.Format("{0}\\{1}", BasePath, FrameName); } }
		protected IContainer Components { get; private set; }
		protected EmbeddedFrameInit EmbeddedFrameInitObject { get; set; }
		protected SplashScreenManager ManagerWaitingForm {
			get {
				if(managerWaitingForm == null) {
					managerWaitingForm = new SplashScreenManager((UserControl)this, typeof(WaitingForm), false, false, ParentType.Form);
					var blob = new SkinBlobXmlCreator(this.LookAndFeel.SkinName, "DevExpress.Utils.Design.", typeof(XtraDesignForm).Assembly, null);
					SplashScreenManager.RegisterUserSkin(blob);
				}
				return managerWaitingForm;
			}
		}
		void InitializeFrame() {
			Name = string.Format("{0}_{1}", FrameName, Guid.NewGuid().ToString());
			Components = new Container();
			CreateInnerControls();
			SizeChanged += (s, e) => OnSizeChanged();
		}
		protected virtual void OnSizeChanged() { }
		protected virtual void CreateInnerControls() { }
		protected string GetPropertyPath(string name) {
			return string.Format("{0}\\{1}", PropertyStorePath, name);
		}
		void ISaveChangesFrame.SaveChanges() {
			SaveChanges();
		}
		protected virtual void SaveChanges() {
		}
		protected Font GetDialogFont(IServiceProvider provider) {
			return provider != null ? DesignUtils.GetDialogFont(provider) : Font;
		}
		Control IEmbeddedFrame.Control { get { return null; } }
		void IEmbeddedFrame.SelectProperty(string propertyName) { }
		void IEmbeddedFrame.SetPropertyGridSortMode(PropertySort propertySort) { }
		void IEmbeddedFrame.InitEmbeddedFrame(EmbeddedFrameInit frameInit) {
			OnInitEmbeddedFrame(frameInit);
		}
		void IEmbeddedFrame.RefreshPropertyGrid() {
			OnRefreshPropertyGrid();
		}
		void IEmbeddedFrame.ShowPropertyGridToolBar(bool show) {
			OnShowPropertyGridToolBar(show);
		}
		protected virtual void OnShowPropertyGridToolBar(bool show) { }
		bool initFlag;
		protected virtual void OnInitEmbeddedFrame(EmbeddedFrameInit frameInit) {
			if(initFlag)
				return;
			EmbeddedFrameInitObject = frameInit;
			DoInitFrame();
			initFlag = true;
		}
		protected virtual void OnRefreshPropertyGrid() { }
	}
	public class ConfirmResult {
		public ConfirmResult(DialogResult dialogResult, bool needConfirmation) {
			Dialogresult = dialogResult;
			NeedConfirmation = needConfirmation;
		}
		public DialogResult Dialogresult { get; set; }
		public bool NeedConfirmation { get; set; }
	}
	public class ConfirmMessageBox : DesignTimeForm {
		public ConfirmMessageBox(string message, IServiceProvider provider)
			: this(message, true, provider) {
		}
		public ConfirmMessageBox(string message, bool showFormAgainVisible, IServiceProvider provider)
			: base() {
			Size = new System.Drawing.Size(400, 200);
			CenterToScreen();
			ShowFormAgainVisible = showFormAgainVisible;
			Message = message;
			ServiceProvider = provider;
			Load += (s, e) => {
				InitializeForm();
				CenterToScreen();
			};
		}
		Panel BottomPanel { get; set; }
		CheckBox CheckBoxShowFormAgain { get; set; }
		SimpleButton ButtonOk { get; set; }
		SimpleButton ButtonCancel { get; set; }
		LabelControl LabelConfirmMessage { get; set; }
		IServiceProvider ServiceProvider { get; set; }
		string Message { get; set; }
		bool ShowConfirmDisableCheckbox { get; set; }
		bool ShowFormAgainVisible { get; set; }
		bool ShowFormAgain {
			get {
				if(ShowConfirmDisableCheckbox)
					return !ShowFormAgainVisible || !CheckBoxShowFormAgain.Checked;
				return true;
			}
		}
		void InitializeForm() {
			var backColor = BackColor;
			ControlBox = false;
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			Name = "ConfirmMessageBox";
			Text = "Warning";
			BackColor = Color.FromKnownColor(KnownColor.White);
			CreateBottomPanel();
			CreateLabelConfirmMessage();
			BottomPanel.BackColor = backColor;
			UpdateFormSize();
			CommonDesignerServiceRegisterHelper.SetEscapeBtnUpService(ServiceProvider, () => {
				Close();
				CommonDesignerServiceRegisterHelper.RemoveEscapeBtnUpService(ServiceProvider);
				return true;
			});
		}
		void UpdateFormSize() {
			var width = LabelConfirmMessage.Width < 450 ? LabelConfirmMessage.Width : 450;
			if(width < 350)
				width = 350;
			var height = ShowFormAgainVisible ? LabelConfirmMessage.Bottom + 140 : LabelConfirmMessage.Bottom + 120;
			Size = new Size(width, height);
		}
		void CreateBottomPanel() {
			BottomPanel = new Panel();
			Controls.Add(BottomPanel);
			BottomPanel.Dock = DockStyle.Bottom;
			if(ShowConfirmDisableCheckbox) {
				BottomPanel.Height = 70;
				BottomPanel.TabIndex = 4;
				CreateCheckBoxShowFormAgain();
			} else {
				BottomPanel.Height = 40;
				BottomPanel.TabIndex = 4;
			}
			var buttonSize = new Size(80, 23);
			var top = BottomPanel.Height - buttonSize.Height;
			var location = new Point(BottomPanel.Width - (buttonSize.Width + 12) * 2, top);
			ButtonOk = DesignTimeFormHelper.CreateButton(BottomPanel, buttonSize, location, 0, "&OK", DialogResult.OK, null);
			AcceptButton = ButtonOk;
			location.X += buttonSize.Width + 12;
			ButtonCancel = DesignTimeFormHelper.CreateButton(BottomPanel, buttonSize, location, 1, "&Cancel", DialogResult.Cancel, null);
			CancelButton = ButtonCancel;
		}
		private void CreateLabelConfirmMessage() {
			LabelConfirmMessage = new LabelControl();
			Controls.Add(LabelConfirmMessage);
			LabelConfirmMessage.Text = Message;
			LabelConfirmMessage.MinimumSize = new System.Drawing.Size(350, 30);
			LabelConfirmMessage.AutoSizeMode = LabelAutoSizeMode.Vertical;
			LabelConfirmMessage.BackColor = Color.FromKnownColor(KnownColor.Transparent);
			LabelConfirmMessage.Padding = new Padding(10);
			LabelConfirmMessage.Appearance.TextOptions.VAlignment = VertAlignment.Center;
		}
		private void CreateCheckBoxShowFormAgain() {
			if(!ShowFormAgainVisible)
				return;
			CheckBoxShowFormAgain = new CheckBox();
			BottomPanel.Controls.Add(CheckBoxShowFormAgain);
			CheckBoxShowFormAgain.Text = "Do not show a confirm message for this edit form";
			CheckBoxShowFormAgain.Width = BottomPanel.Width;
			CheckBoxShowFormAgain.Location = new Point(10, 10);
			CheckBoxShowFormAgain.Checked = false;
		}
		public static ConfirmResult Show(string message, IServiceProvider serviceProvider) {
			return Show(message, true, null);
		}
		public static ConfirmResult Show(string message, bool showConfirmDisableCheckbox, IServiceProvider serviceProvider) {
			var confirmBox = new ConfirmMessageBox(message, true, serviceProvider) { ShowConfirmDisableCheckbox = showConfirmDisableCheckbox };
			return new ConfirmResult(confirmBox.ShowDialog(), confirmBox.ShowFormAgain);
		}
	}
	public interface ISaveChangesFrame {
		void SaveChanges();
	}
	public static class DesignTimeFormHelper {
		const int SplitterMinExtra = 230;
		const int SplitterWidth = 5;
		public static PanelControl CreatePanel(Control parent, string name, AnchorStyles anchor, int top, int height) {
			var result = CreatePanel(parent, name, DockStyle.None);
			result.Anchor = anchor;
			result.Height = height;
			return result;
		}
		public static PanelControl CreatePanel(Control parent, string name, DockStyle dockStyle) {
			var panel = new DevExpress.XtraEditors.PanelControl();
			panel.Name = name;
			panel.Parent = parent;
			panel.Dock = dockStyle;
			panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			panel.TabStop = false;
			return panel;
		}
		public static SplitterControl CreateSplitterControl(Control container, DockStyle dockStyle, int minExtra = SplitterMinExtra, int width = SplitterWidth) {
			var result = new SplitterControl();
			container.Controls.Add(result);
			result.Dock = dockStyle;
			result.MinExtra = minExtra;
			result.Width = width;
			result.TabStop = false;
			return result;
		}
		public static SimpleButton CreateButton(Control parent, Size size, Point location, int tabIndex, string text, EventHandler onClick) {
			return CreateButton(parent, size, location, tabIndex, text, DialogResult.None, onClick);
		}
		public static SimpleButton CreateButton(Control parent, Size size, Point location, int tabIndex, string text, DialogResult dialogResult, EventHandler onClick) {
			var button = new SimpleButton();
			parent.Controls.Add(button);
			button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			button.Size = size;
			button.Location = location;
			if(tabIndex > 0)
				button.TabIndex = tabIndex;
			if(dialogResult != DialogResult.None)
				button.DialogResult = dialogResult;
			button.Text = text;
			if(onClick != null)
				button.Click += onClick;
			return button;
		}
		public static LabelControl CreateLabel(Control parent, string name, string text, Point location) {
			return new LabelControl() { Name = name, Location = location, Text = text, Parent = parent };
		}
		public static int GetTextWidth(Control control, string text, Font font) {
			return GetTextSize(control, text, font).Width;
		}
		public static Size GetTextSize(Control control, string text, Font font) {
			return control.CreateGraphics().MeasureString(text, font).ToSize();
		}
		public static DialogResult ShowMessage(IServiceProvider provider, string message, string caption, MessageBoxButtons buttons) {
			if(provider != null) {
				var service = (IUIService)provider.GetService(typeof(IUIService));
				if(service != null)
					return service.ShowMessage(message, caption, buttons);
			}
			return MessageBox.Show(message, caption, buttons);
		}
	}
	public class WaitingForm : WaitForm {
		ProgressPanel ProgressPanel { get; set; }
		TableLayoutPanel TableLayoutPanel { get; set; }
		public WaitingForm() {
			InitializeComponent();
			this.ProgressPanel.AutoHeight = true;
		}
		public override void ProcessCommand(Enum cmd, object arg) {
			base.ProcessCommand(cmd, arg);
		}
		void InitializeComponent() {
			SuspendLayout();
			CreateTableLayoutPanel();
			CreateProgressPanel();
			InitializeForm();
			ResumeLayout(false);
			PerformLayout();
		}
		private void InitializeForm() {
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			ClientSize = new Size(246, 73);
			DoubleBuffered = true;
			Name = "WaitingForm";
			StartPosition = FormStartPosition.Manual;
		}
		private void CreateTableLayoutPanel() {
			TableLayoutPanel = new TableLayoutPanel();
			TableLayoutPanel.Name = "tableLayoutPanel";
			Controls.Add(TableLayoutPanel);
			TableLayoutPanel.AutoSize = true;
			TableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			TableLayoutPanel.BackColor = Color.Transparent;
			TableLayoutPanel.ColumnCount = 1;
			TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 100F));
			TableLayoutPanel.Dock = DockStyle.Fill;
			TableLayoutPanel.Location = new Point(0, 0);
			TableLayoutPanel.Padding = new Padding(0, 14, 0, 14);
			TableLayoutPanel.RowCount = 1;
			TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			TableLayoutPanel.Size = new Size(246, 73);
			TableLayoutPanel.TabIndex = 1;
		}
		private void CreateProgressPanel() {
			ProgressPanel = new ProgressPanel();
			ProgressPanel.Name = "progressPanel";
			TableLayoutPanel.Controls.Add(ProgressPanel, 0, 0);
			ProgressPanel.Appearance.BackColor = Color.Transparent;
			ProgressPanel.Appearance.Options.UseBackColor = true;
			ProgressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			ProgressPanel.ImageHorzOffset = 20;
			ProgressPanel.Location = new Point(0, 17);
			ProgressPanel.Margin = new Padding(0, 3, 0, 3);
			ProgressPanel.Size = new Size(246, 39);
			ProgressPanel.TabIndex = 0;
		}
	}
	public class DesignTimeForm : XtraDesignForm {
		public DesignTimeForm()
			: base() {
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			MaximizeBox = MinimizeBox = CloseBox = false;
			Padding = new Padding(0);
		}
	}
}
