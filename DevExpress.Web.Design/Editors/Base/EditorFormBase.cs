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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class EditorFormBase : DesignTimeForm {
		protected static int RelatedControlsSpacing = 6;
		protected static int NonRelatedControlsSpacing = 11;
		private static string BasePath = "Software\\Developer Express .NET\\ASPxClasses\\Designer";
		private const FormWindowState DefaultFormState = FormWindowState.Normal;
		private const int DefaultFormWidth = 200;
		private const int DefaultFormHeight = 200;
		private const int MessageHeight = 30;
		private const int MessagePadding = 20;
		protected static int[] TabOrder = new int[] { 1, 2, 3, 4, 5 };
		private bool fCancelChanges = false;
		private IDesignerHost fDesignerHost = null;
		private IDesigner fDesigner = null;
		private IComponent fComponent;
		protected Panel fLeftPanel = null;
		private IServiceProvider fServiceProvider;
		private ITypeDescriptorContext fContext;
		private object fPropertyValue;
		private Label fMessageLabel = null;
		protected Button fOkButton = null;
		protected Button fCancelButton = null; 
		protected virtual Size FormDefaultSize { get { return new Size(510, 500); } }
		protected virtual Size FormMinimumSize { get { return new Size(500, 500); } } 
		protected string EditingPropertyName {
			get {
				if (Context != null && Context.PropertyDescriptor != null)
					return Context.PropertyDescriptor.Name;
				return GetPropertyNameShowingInFormCaption();
			}
		}
		protected object PropertyValue {
			get { return fPropertyValue; }
		}
		protected ITypeDescriptorContext Context {
			get { return fContext; }
		}
		protected IServiceProvider ServiceProvider {
			get { return fServiceProvider; }
		}
		protected object Component {
			get { return fComponent; }
		}
		protected IDesignerHost DesignerHost {
			get {
				if (fDesignerHost == null)
					fDesignerHost = ServiceProvider != null ? (IDesignerHost)ServiceProvider.GetService(typeof(IDesignerHost)) : null;
				return fDesignerHost;
			}
		}
		protected IDesigner Designer {
			get {
				if (fDesigner == null)
					fDesigner = DesignerHost != null ? DesignerHost.GetDesigner((Component as IComponent)) : null;
				return fDesigner;
			}
		}
		protected virtual string Caption {
			get { return StringResources.EditorFormBase_DefaultCaption; }
		}
		protected bool IsChangesCanceled {
			get { return fCancelChanges; }
		}
		public EditorFormBase(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base() {
			fComponent = component as IComponent;
			fContext = context;
			fServiceProvider = provider;
			fPropertyValue = propertyValue;
			fCancelChanges = false;
			InitializeForm();
			RestoreFormProperties();
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			AddControls();
			AssignControls();
			SaveUndoData();
			RefreshMessage();
			RestoreControlsProperties();
			SetTabIndexes();
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			if ((e.Cancel) || (e.CloseReason == CloseReason.UserClosing))
				CancelChanges();
			base.OnFormClosing(e);
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			SaveFormProperties();
			SaveControlsProperties();
			base.OnClosing(e);
		}
		protected virtual Font GetTextBoxFont() {
			return new Font("Courier New", 10F);
		}
		protected virtual void AssignControls() {
		}
		protected virtual void SaveUndoData() {
		}
		protected virtual void Undo() {
		}
		protected virtual string GetPropertyNameShowingInFormCaption() {
			return "";
		}
		protected virtual void RestoreControlsProperties() { }
		protected virtual void SaveControlsProperties() { }
		protected virtual void SetTabIndexes() {
			fOkButton.TabStop = true;
			fOkButton.TabIndex = TabOrder[3];
			fCancelButton.TabStop = true;
			fCancelButton.TabIndex = TabOrder[4];
		}
		protected virtual void RestoreFormProperties() {
			PropertyStore store = GetPropertyStore();
			string state = store.LoadString("State", "");
			Rectangle bounds = store.LoadRectangle("Bounds");
			if (!bounds.IsEmpty)
				Bounds = bounds;
			else
				Size = FormDefaultSize;
			if (state == "") {
				WindowState = DefaultFormState;
				StartPosition = FormStartPosition.CenterScreen;
			}
			else
				if (state == FormWindowState.Maximized.ToString())
					WindowState = FormWindowState.Maximized;
			if (!bounds.IsEmpty && bounds.X < 0 || bounds.Y < 0 || bounds.X > Screen.PrimaryScreen.Bounds.Right ||
				bounds.Y > Screen.PrimaryScreen.Bounds.Bottom) {
				WindowState = DefaultFormState;
				StartPosition = FormStartPosition.CenterScreen;
				Size = bounds.Size;
			}
		}
		protected virtual void SaveFormProperties() {
			PropertyStore store = GetPropertyStore();
			if (WindowState != FormWindowState.Maximized)
				store.SaveRectangle("Bounds", Bounds);
			store.SaveString("State", WindowState.ToString());
		}
		protected virtual void InitializeForm() {
			StartPosition = FormStartPosition.Manual;
			ShowIcon = false;
			MaximizeBox = true;
			MinimizeBox = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Show;
			Width = 520;
			Height = 400;
			Font = DesignUtils.GetDialogFont(ServiceProvider);			
			Padding = new Padding(NonRelatedControlsSpacing, NonRelatedControlsSpacing - 6, NonRelatedControlsSpacing, NonRelatedControlsSpacing - 5);
			Text = GetFormCaption();
			MinimumSize = FormMinimumSize;
		}
		protected virtual void AddControlsToMainPanel(Panel mainPanel) {
		}
		protected virtual void AddMessageStrings(List<string> messages) {
		}
		protected virtual void SaveChanges() {
			if(!NotifyCollectionChanges && IsCollectionChanged) ComponentChanged(false);
		}
		protected virtual void CancelChanges() {
			fCancelChanges = true;
			Undo();
			ComponentChanged(true);
		}
		protected virtual void ComponentChanging() {
			if(Designer is ASPxWebControlDesigner) {
				ASPxWebControlDesigner webControlDesigner = (ASPxWebControlDesigner)Designer;
				webControlDesigner.ComponentChanging();
			}
		}
		protected void ComponentChanged(bool checkChanged) {
			ComponentChanged(checkChanged, false);
		}
		protected virtual bool NotifyCollectionChanges { get { return true; } }
		bool isCollectionChanged = false;
		protected bool IsCollectionChanged {
			get { return isCollectionChanged; }
			set { isCollectionChanged = value; }
		}
		protected virtual void ComponentChanged(bool checkChanged, bool isCollectionChangeNotification) {
			if(Designer is ASPxWebControlDesigner) {
				ASPxWebControlDesigner webControlDesigner = (ASPxWebControlDesigner)Designer;
				if(!NotifyCollectionChanges && isCollectionChangeNotification) {
					this.IsCollectionChanged = true;
					return;
				}
				if(!checkChanged || webControlDesigner.IsComponentChanged())
					webControlDesigner.ComponentChanged();
			}
			else if(ServiceProvider is IEditFormNotificationOwner)
				((IEditFormNotificationOwner)ServiceProvider).Changed(EditingPropertyName); 
		}
		protected virtual string GetFormCaption() { 
			string formCaption = "";
			if (Designer is ASPxWebControlDesigner) 
				formCaption = ((ASPxWebControlDesigner)Designer).GetFormCaption(EditingPropertyName);
			return (formCaption != "") ? formCaption : Caption;
		}
		protected virtual string GetOkButtonText() {
			return "OK";
		}
		protected virtual string GetCancelButtonText() {
			return "Cancel";
		}
		public virtual Bitmap CreateBitmapFromResources(string resourceName) {
			return CreateBitmapFromResources(resourceName, typeof(EditorFormBase).Assembly);
		}
		public Bitmap CreateBitmapFromResources(string resourceName, System.Reflection.Assembly assembly) {
			System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
			Bitmap image = (Bitmap)Bitmap.FromStream(stream);
			return image;
		}
		protected virtual DialogResult GetDialogResultOkBtn() {
			return DialogResult.OK;
		}
		protected virtual string GetPropertyStorePathPrefix() {
			throw new Exception(StringResources.Serializer_OperationNotImplemented);
		}
		protected PropertyStore GetPropertyStore() {
			return new PropertyStore(BasePath + "\\" + GetPropertyStorePathPrefix());
		}
		protected void SetOkButtonEnable(bool value) {
			fOkButton.Enabled = value;
		}
		protected void RefreshMessage() {
			List<string> messages = new List<string>();
			AddMessageStrings(messages);
			fMessageLabel.Text = "";
			for (int i = 0; i < messages.Count; i++)
				fMessageLabel.Text += messages[i] + "\n";
		}
		private void AddControls() {
			AddMainPanel();
			AddButtonsPanel();
		}
		private void AddMainPanel() {
			Panel panelMain = new Panel();
			Controls.Add(panelMain);
			panelMain.Dock = DockStyle.Fill;
			panelMain.Padding = new Padding(panelMain.Padding.Left, panelMain.Padding.Top,
				panelMain.Padding.Right, NonRelatedControlsSpacing);
			AddControlsToMainPanel(panelMain);
		}
		private void AddButtonsPanel() {
			Panel panelButtons = new Panel();
			panelButtons.Size = new Size(this.Width, MessageHeight);
			DesignUtils.CheckLargeFontSize(panelButtons);
			AddButtons(panelButtons);
			AddMessageLabel(panelButtons);
			panelButtons.Dock = DockStyle.Bottom;
			Controls.Add(panelButtons);
		}
		protected virtual void AddButtons(Panel parentPanel) {
			fOkButton = ControlCreator.CreateButton(GetOkButtonText());
			fCancelButton = ControlCreator.CreateButton(GetCancelButtonText());
			DesignUtils.CheckLargeFontSize(fOkButton);
			DesignUtils.CheckLargeFontSize(fCancelButton);
			SetCommonProperties(fOkButton);
			SetCommonProperties(fCancelButton);
			fCancelButton.Location = new Point(parentPanel.Width - fCancelButton.Width,
				(parentPanel.Height - fCancelButton.Height) / 2);
			fOkButton.Location = new Point(fCancelButton.Location.X - fOkButton.Width - RelatedControlsSpacing,
				fCancelButton.Location.Y);
			fOkButton.DialogResult = GetDialogResultOkBtn();
			fOkButton.Click += new EventHandler(OnOkButtonClick);
			fCancelButton.DialogResult = DialogResult.Cancel;
			fCancelButton.Click += new EventHandler(OnCancelButtonClick);
			AcceptButton = fOkButton;
			CancelButton = fCancelButton;
			parentPanel.Controls.Add(fOkButton);
			parentPanel.Controls.Add(fCancelButton);
		}
		private void AddMessageLabel(Panel parentPanel) {
			fMessageLabel = new Label();
			fMessageLabel.AutoSize = false;
			fMessageLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			fMessageLabel.ForeColor = Color.Red;
			fMessageLabel.Size = new Size(fOkButton.Location.X - MessagePadding, MessageHeight);
			float scaleFactor = DesignUtils.GetScaleFactor();
			if (scaleFactor != 1)
				fMessageLabel.Size = new Size(fMessageLabel.Size.Width, DesignUtils.CorrectSize(fMessageLabel.Size, scaleFactor).Height);
			fMessageLabel.Location = new Point(0, fOkButton.Location.Y);
			parentPanel.Controls.Add(fMessageLabel);
		}
		private void SetCommonProperties(Button button) {
			button.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
		}
		private void OnOkButtonClick(object sender, EventArgs e) {
			SaveChanges();
		}
		private void OnCancelButtonClick(object sender, EventArgs e) {
			CancelChanges();
		}
	}
	public abstract class TwoSidesFormBase: EditorFormBase {
		private Size fFormSize;
		protected Panel fRightPanel = null;
		protected Splitter fSplitter = null;
		protected virtual DockStyle LeftPanelDockStyle { get { return DockStyle.Fill; } }
		protected virtual int LeftPanelDefaultWidth { get { return 230; } }
		protected virtual int LeftPanelMinimizeWidth { get { return 230; } }
		protected virtual int LeftPanelDefaultHeight { get { return -1; } }
		protected virtual int RigthPanelDefaultWidth { get {  return 250; } }
		protected virtual int RightPanelMinimizeWidth { get { return 150; } }
		protected virtual DockStyle RightPanelDockStyle { get { return DockStyle.Right; } }
		protected virtual DockStyle SplitterDefaultStyle { get { return DockStyle.Right; } }
		public TwoSidesFormBase(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
		}
		protected override void AddControlsToMainPanel(Panel mainPanel) {
			base.AddControlsToMainPanel(mainPanel);
			fSplitter = new Splitter();
			fSplitter.Dock = SplitterDefaultStyle;
			fRightPanel = new Panel();
			fLeftPanel = new Panel();
			fLeftPanel.Width = LeftPanelMinimizeWidth;
			if (LeftPanelDefaultHeight > 0)
				fLeftPanel.Height = LeftPanelDefaultHeight;
			fLeftPanel.Dock = LeftPanelDockStyle;
			fLeftPanel.MinimumSize = new Size(LeftPanelMinimizeWidth, 0);
			fRightPanel.Dock = RightPanelDockStyle;
			fRightPanel.MinimumSize = new Size(RightPanelMinimizeWidth, 10);
			AddControlsToLeftPanel(fLeftPanel);
			AddControlsToRightPanel(fRightPanel);
			if (LeftPanelDockStyle == DockStyle.Fill) {
				fSplitter.MinExtra = LeftPanelMinimizeWidth + fSplitter.Width;
				mainPanel.Controls.Add(fLeftPanel);
				mainPanel.Controls.Add(fSplitter);
				mainPanel.Controls.Add(fRightPanel);
			}
			else {
				fSplitter.MinExtra = RightPanelMinimizeWidth + fSplitter.Width;
				mainPanel.Controls.Add(fRightPanel);
				mainPanel.Controls.Add(fSplitter);
				mainPanel.Controls.Add(fLeftPanel);
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if ((fLeftPanel != null) && (fLeftPanel.Size.Width <= LeftPanelMinimizeWidth)) {
				 if (fFormSize.Width > Size.Width)
					fRightPanel.Size = new Size(fRightPanel.Size.Width - fFormSize.Width + Size.Width, fRightPanel.Size.Height);
			}
			fFormSize = Size;
		}
		protected override void RestoreControlsProperties() {
			int splitPosition = RestoreIntProperty("Splitter", 0);
			if (splitPosition == 0)
				RestoreDefaultProperty();
			else
				fSplitter.SplitPosition = splitPosition;
			base.RestoreControlsProperties();
		}
		protected override void SaveControlsProperties() {
			SaveIntProperty("Splitter", fSplitter.SplitPosition);
			base.SaveControlsProperties();
		}
		protected virtual void AddControlsToLeftPanel(Panel leftPanel) { }
		protected virtual void AddControlsToRightPanel(Panel rightPanel) { }
		protected virtual int GetDefaultSplitPosition() {
			return Size.Width / 2;
		}
		protected virtual void RestoreDefaultProperty() {
			fRightPanel.Size = new Size(RigthPanelDefaultWidth, fRightPanel.Size.Height);
		}
		protected bool RestoreBoolProperty(string propertyName, bool defaultValue) {
			PropertyStore store = GetPropertyStore();
			int value = store.LoadInt(propertyName, -1);
			if (value == -1)
				return defaultValue;
			else
				return value == 1;
		}
		protected int RestoreIntProperty(string propertyName, int defaultValue) {
			PropertyStore store = GetPropertyStore();
			return store.LoadInt(propertyName, defaultValue);
		}
		protected string RestoreStringProperty(string propertyName, string defaultValue) {
			PropertyStore store = GetPropertyStore();
			return store.LoadString(propertyName, defaultValue);
		}
		protected void SaveBoolProperty(string propertyName, bool value) {
			PropertyStore store = GetPropertyStore();
			store.SaveInt(propertyName, value ? 1 : 0);
		}
		protected void SaveIntProperty(string propertyName, int value) {
			PropertyStore store = GetPropertyStore();
			store.SaveInt(propertyName, value);
		}
		protected void SaveStringProperty(string propertyName, string value) {
			PropertyStore store = GetPropertyStore();
			store.SaveString(propertyName, value);
		}
	}
}
