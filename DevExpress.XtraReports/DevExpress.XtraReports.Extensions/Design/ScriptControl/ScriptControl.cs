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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design {
	[ToolboxItem(false)]
	public class ScriptControl : PanelControl {
		class ScriptsComboBoxItem : IComparable  {
			public string EventName { get; set; }
			public string DefaultSuffix { get; set; }
			public override string ToString() {
				return string.IsNullOrEmpty(DefaultSuffix) ? EventName : string.Format("{0}   ({1})", EventName, DefaultSuffix);
			}
			public override bool Equals(object obj) {
				return CompareTo(obj) == 0;
			}
			public override int GetHashCode() {
				return EventName == null ? 0 : EventName.GetHashCode();
			}
			public int CompareTo(object obj) {
				ScriptsComboBoxItem item = obj as ScriptsComboBoxItem;
				return item != null ? System.Collections.Comparer.Default.Compare(this.EventName, item.EventName) : -1;
			}
		}
		#region fields & properties
		MethodsHelper methodsHelper;
		ScriptsComponentsComboBoxControler controlsComboBox;
		ComboBoxEdit controlScriptsComboBox;
		SimpleButton simpleButton;
		TableLayoutPanel layoutPanel;
		TableLayoutPanel memoLayoutPanel;
		IServiceProvider serviceProvider;
		XtraReport report;
		IDXMenuManager menuManager;
		IScriptEditor syntaxEditor;
#if DEBUGTEST
		public SyntaxEditor Test_SyntaxEditor {
			get {
				return syntaxEditor as SyntaxEditor;
			}
		}
#endif
		IScriptEditor SyntaxEditor {
			get {
				if(syntaxEditor == null) {
					IScriptEditorService serv = serviceProvider.GetService(typeof(IScriptEditorService)) as IScriptEditorService;
					syntaxEditor = serv != null ? serv.CreateEditor(serviceProvider) : CreateSyntaxEditor(menuManager);
					Control control = (Control)syntaxEditor;
					if(control is ISupportInitialize)
						((ISupportInitialize)control).BeginInit();
					control.Dock = DockStyle.Fill;
					control.TabIndex = 0;
					control.Name = "syntaxEditor";
					syntaxEditor.Text = report.ScriptsSource;
					control.LostFocus += syntaxEditor_LostFocus;
					control.GotFocus += syntaxEditor_GotFocus;
					memoLayoutPanel.SuspendLayout();
					memoLayoutPanel.Controls.Add(control, 0, 0);
					memoLayoutPanel.ResumeLayout(true);
					if(control is ISupportInitialize)
						((ISupportInitialize)control).EndInit();
				}
				return syntaxEditor;
			}
		}
		IScriptEditor CreateSyntaxEditor(IDXMenuManager menuManager) {
			SyntaxEditor syntaxEditor = new SyntaxEditor( new SyntaxColors(LookAndFeel), ValidateScript);
			syntaxEditor.BeginInit();
			syntaxEditor.BeginUpdate();
			try {
				syntaxEditor.SyntaxHelper = new SyntaxHelper(syntaxEditor, new ScriptSource(report), serviceProvider);
				syntaxEditor.MenuManager = menuManager;
				syntaxEditor.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			} finally {
				syntaxEditor.EndUpdate();
				syntaxEditor.EndInit();
			}
			return syntaxEditor;
		}
		public new string Text {
			get { return SyntaxEditor.Text; }
			set { SyntaxEditor.Text = value; }
		}
		public int LinesCount {
			get { return SyntaxEditor.LinesCount; }
		}
		internal bool TextWasChanged { get { return syntaxEditor != null && syntaxEditor.Modified; } }
		IDesignerHost DesignerHost { get { return (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost)); } }
		#endregion
		public ScriptControl(XtraReport report, IServiceProvider serviceProvider, UserLookAndFeel userLookAndFeel, IDXMenuManager menuManager) {
			this.LookAndFeel.ParentLookAndFeel = userLookAndFeel;
			this.serviceProvider = serviceProvider;
			this.report = report;
			this.menuManager = menuManager;
			InitializeComponent();
			SubscribeEvents();
			controlsComboBox.UpdateItems();
			UpdateSelectedItemScripts();
			DesignerHost.AddService(typeof(ScriptControl), this);
			IToolShell toolShell = (IToolShell)serviceProvider.GetService(typeof(IToolShell));
			simpleButton.Enabled = toolShell != null;
		}
		void SubscribeEvents() {
			ISelectionService selectionService = serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
			if(selectionService != null)
				selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);
			IComponentChangeService comChangeServ = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(comChangeServ != null) {
				comChangeServ.ComponentAdded += new ComponentEventHandler(comChangeServ_ComponentAdded);
				comChangeServ.ComponentRemoved += new ComponentEventHandler(comChangeServ_ComponentAdded);
			}
			controlsComboBox.LocationChanged += new EventHandler(controlsComboBox_LocationChanged);
			controlScriptsComboBox.CloseUp += new CloseUpEventHandler(controlScriptsComboBox_CloseUp);
			controlScriptsComboBox.GotFocus += new EventHandler(controlScriptsComboBox_GotFocus);
			simpleButton.Click += new EventHandler(simpleButton_Click);
		}
		void UnsubscribeEvents() {
			ISelectionService selectionService = serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
			if(selectionService != null)
				selectionService.SelectionChanged -= new EventHandler(selectionService_SelectionChanged);
			IComponentChangeService comChangeServ = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(comChangeServ != null) {
				comChangeServ.ComponentAdded -= new ComponentEventHandler(comChangeServ_ComponentAdded);
				comChangeServ.ComponentRemoved -= new ComponentEventHandler(comChangeServ_ComponentAdded);
			}
			controlsComboBox.LocationChanged -= new EventHandler(controlsComboBox_LocationChanged);
			controlScriptsComboBox.CloseUp -= new CloseUpEventHandler(controlScriptsComboBox_CloseUp);
			controlScriptsComboBox.GotFocus -= new EventHandler(controlScriptsComboBox_GotFocus);
			simpleButton.Click -= new EventHandler(simpleButton_Click);
			if(syntaxEditor != null) {
				((Control)syntaxEditor).LostFocus -= new EventHandler(syntaxEditor_LostFocus);
				((Control)syntaxEditor).GotFocus -= new EventHandler(syntaxEditor_GotFocus);
			}
		}
		void InitializeComponent() {
			controlsComboBox = new ScriptsComponentsComboBoxControler(serviceProvider);
			controlScriptsComboBox = new ComboBoxEdit();
			simpleButton = new SimpleButton();
			methodsHelper = new MethodsHelper(new ScriptSource(report));
			layoutPanel = new TableLayoutPanel();
			memoLayoutPanel = new TableLayoutPanel();
			this.BeginInit();
			this.SuspendLayout();
			layoutPanel.SuspendLayout();
			memoLayoutPanel.SuspendLayout();
			((ISupportInitialize)controlsComboBox.Properties).BeginInit();
			((ISupportInitialize)controlScriptsComboBox.Properties).BeginInit();
			this.Controls.Add(memoLayoutPanel);
			this.Controls.Add(layoutPanel);
			layoutPanel.ColumnCount = 3;
			layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			layoutPanel.Controls.Add(controlsComboBox, 0, 0);
			layoutPanel.Controls.Add(controlScriptsComboBox, 1, 0);
			layoutPanel.Controls.Add(simpleButton, 2, 0);
			layoutPanel.Dock = DockStyle.Top;
			layoutPanel.Location = new Point(0, 0);
			layoutPanel.Name = "layoutPanel";
			layoutPanel.RowCount = 1;
			layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
			layoutPanel.TabIndex = 0;
			memoLayoutPanel.ColumnCount = 1;
			memoLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			memoLayoutPanel.Dock = DockStyle.Fill;
			memoLayoutPanel.Location = new Point(0, 0);
			memoLayoutPanel.Name = "memoLayoutPanel";
			memoLayoutPanel.RowCount = 1;
			memoLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
			memoLayoutPanel.TabIndex = 1;
			simpleButton.Location = new Point(481, 3);
			simpleButton.Dock = DockStyle.Fill;
			simpleButton.Name = "simpleButton";
			simpleButton.Size = new Size(75, 23);
			simpleButton.TabIndex = 0;
			simpleButton.Text = ReportLocalizer.GetString(ReportStringId.ScriptEditor_Validate);
			simpleButton.Image = XRBitmaps.Validate;
			simpleButton.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			controlsComboBox.Dock = DockStyle.Top;
			controlsComboBox.Location = new Point(0, 0);
			controlsComboBox.TabIndex = 0;
			controlsComboBox.Properties.DropDownRows = 30;
			controlsComboBox.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			controlScriptsComboBox.Dock = DockStyle.Top;
			controlScriptsComboBox.Location = new Point(0, 0);
			controlScriptsComboBox.Properties.Sorted = true;
			controlScriptsComboBox.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			controlScriptsComboBox.Properties.DropDownRows = 30;
			controlScriptsComboBox.Properties.Buttons.AddRange(new EditorButton[] {
			new EditorButton(ButtonPredefines.Combo)});
			FontStyle fontStyle = AppearanceObject.DefaultMenuFont.Style & ~FontStyle.Bold;
			controlScriptsComboBox.Font = new Font(new Font(AppearanceObject.DefaultMenuFont, fontStyle), fontStyle | FontStyle.Bold);
			controlScriptsComboBox.TabIndex = 0;
			controlScriptsComboBox.Name = "controlScripts";
			controlScriptsComboBox.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			InitializeByValidationText();
			((ISupportInitialize)controlsComboBox.Properties).EndInit();
			((ISupportInitialize)controlScriptsComboBox.Properties).EndInit();
			this.EndInit();
			this.ResumeLayout();
			layoutPanel.ResumeLayout(false);
			memoLayoutPanel.ResumeLayout(false);
		}
		void InitializeByValidationText() {
			int standartTextWidth = 45;
			int standartButtonWidth = 75;
			int textWidth = (int)XtraPrinting.Native.Measurement.MeasureString(simpleButton.Text, simpleButton.Font, GraphicsUnit.Pixel).Width;
			int realButtonMinWidth = (simpleButton as DevExpress.Utils.Controls.IXtraResizableControl).MinSize.Width;
			if(textWidth < standartTextWidth && realButtonMinWidth < standartButtonWidth)
				return;
			int offset = textWidth < standartTextWidth && realButtonMinWidth > standartButtonWidth ?
				realButtonMinWidth - standartButtonWidth : textWidth - standartTextWidth;
			controlsComboBox.Width -= offset;
			controlScriptsComboBox.Left -= offset;
			simpleButton.Left -= offset;
			simpleButton.Width += offset;
		}
		void selectionService_SelectionChanged(object sender, EventArgs e) {
			UpdateSelectedItemScripts();
		}
		void comChangeServ_ComponentAdded(object sender, ComponentEventArgs e) {
			controlsComboBox.UpdateItems();
			UpdateSelectedItemScripts();
		}
		void controlsComboBox_LocationChanged(object sender, EventArgs e) {
			layoutPanel.Height = controlsComboBox.Size.Height + 2 * controlsComboBox.Location.Y;
		}
		void controlScriptsComboBox_CloseUp(object sender, CloseUpEventArgs e) {
			ScriptsComboBoxItem selection = e.Value as ScriptsComboBoxItem;
			if(selection != null)
				SelectScript(null, selection.EventName);
		}
		void controlScriptsComboBox_GotFocus(object sender, EventArgs e) {
			controlsComboBox.SetSelectedComponent();
		}
		void simpleButton_Click(object sender, EventArgs e) {
			ValidateScript();
		}
		public void ValidateScript() {
			ScriptSourceCorrector helper = new ScriptSourceCorrector(report.ScriptLanguage, this.Text);
			this.Text = helper.CorrectSource;
			SaveScripts();
			SyntaxEditor.Modified = false;
			IToolItem toolItem = GetReportToolItem();
			if(toolItem != null) {
				toolItem.UpdateView();
				toolItem.ShowActivate();
			}
		}
		void syntaxEditor_LostFocus(object sender, EventArgs e) {
			SaveScripts();
		}
		void syntaxEditor_GotFocus(object sender, EventArgs e) {
			AdjustCommands();
			controlsComboBox.SetSelectedComponent();
		}
		internal virtual void AdjustCommands() {
			MenuCommandHandler menuCommandHandler = (MenuCommandHandler)serviceProvider.GetService(typeof(MenuCommandHandler));
			if(menuCommandHandler != null) {
				menuCommandHandler.DisableCommands();
				menuCommandHandler.LockCommands();
				menuCommandHandler.UnlockCommands(
					UICommands.NewReportWizard,
					UICommands.NewReport,
					UICommands.OpenFile,
					UICommands.SaveFile,
					UICommands.SaveFileAs,
					UICommands.SaveAll,
					UICommands.Close,
					UICommands.Exit,
					UICommands.Closing,
					ReportTabControlCommands.ShowScriptsTab,
					ReportTabControlCommands.ShowDesignerTab
				);
				menuCommandHandler.UpdateCommandStatus();
			}
		}
		IToolItem GetReportToolItem() {
			IToolShell toolShell = (IToolShell)serviceProvider.GetService(typeof(IToolShell));
			if(toolShell != null)
				return toolShell[ReportToolItemKindHelper.ErrorList];
			return null;
		}
		void UpdateSelectedItemScripts() {
			controlsComboBox.UpdateSelectedItem();
			controlScriptsComboBox.SelectedIndex = -1;
			controlScriptsComboBox.Properties.Items.Clear();
			XRScriptsBase scripts = GetControlScripts();
			if(scripts == null)
				return;
			controlScriptsComboBox.Properties.Items.AddRange(Array.ConvertAll(GetScriptsNames(scripts, x => x.DisplayName), s => GetScriptsComboBox(scripts, s)));
		}
		protected virtual string[] GetScriptsNames(XRScriptsBase scripts, Func<PropertyDescriptor, string> getPropertyName) {
			return scripts.GetScriptsNames(getPropertyName);
		}
		XRScriptsBase GetControlScripts() {
			IScriptable scriptContainer = controlsComboBox.GetSelectedComponent() as IScriptable;
			if(scriptContainer == null)
				return null;
			return scriptContainer.Scripts;
		}
		void SelectScript(XRScriptsBase sourceScripts, string eventName) {
			XRScriptsBase scripts = sourceScripts == null ? GetControlScripts() : sourceScripts;
			if(scripts == null)
				return;
			string procName = scripts.GetProcName(eventName);
			if(string.IsNullOrEmpty(procName)) {
				procName = scripts.GetDefaultPropertyValue(eventName);
				scripts.SetPropertyValue(eventName, procName);
			}
			SelectMethod(procName, scripts.GenerateDefaultEventScript(eventName, procName));
			SaveScripts();
		}
		void SelectMethod(string methodName, string methodCode) {
			if(!ContainsMethod(methodName))
				SyntaxEditor.AppendText("\r\n" + methodCode);
			SetCaretPosition(methodsHelper.GetMethodTopLine(this.Text, methodName), 0);
		}
		bool ContainsMethod(string methodName) {
			return methodsHelper.ContainsMethod(this.Text, methodName);
		}
		List<string> GetCompatibleMethodsNames(string methodCode) {
			return methodsHelper.GetCompatibleMethodsNames(this.Text, methodCode);
		}
		public void SetCaretPosition(int line, int column) {
			try {
				SyntaxEditor.SetCaretPosition(line, column);
			} catch(Exception ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
				string message = ReportStringId.ScriptEditor_ScriptHasBeenChanged.GetString();
				NotificationService.ShowException<XtraReport>(LookAndFeel, serviceProvider.GetOwnerWindow(), new Exception(message));
			}
		}
		public void ShowErrors(CompilerErrorCollection errors) {
			if(syntaxEditor == null && errors.Count == 0)
				return;
			SyntaxEditor.HighlightErrors(errors);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeEvents();
				if(DesignerHost != null)
					DesignerHost.RemoveService(typeof(ScriptControl));
			}
			base.Dispose(disposing);
		}
		public string SetSelectedScript(IScriptable scriptContainer, string eventName, bool createNewProcedureName) {
			XRScriptsBase scripts = scriptContainer != null ? scriptContainer.Scripts : GetControlScripts();
			if(scripts == null)
				return string.Empty;
			string procName = scripts.GetProcName(eventName);
			if(string.IsNullOrEmpty(procName))
				procName = scripts.GetDefaultPropertyValue(eventName);
			if(createNewProcedureName) {
				string baseName = scripts.GetDefaultPropertyValue(eventName);
				procName = baseName;
				int index = 1;
				while(ContainsMethod(procName))
					procName = String.Format("{0}_{1}", baseName, index++);
			}
			scripts.SetPropertyValue(eventName, procName);
			SelectScript(scripts, eventName);
			controlScriptsComboBox.SelectedIndex = controlScriptsComboBox.Properties.Items.IndexOf(GetScriptsComboBox(scripts, eventName));
			return procName;
		}
		static ScriptsComboBoxItem GetScriptsComboBox(XRScriptsBase scripts, string eventName) {
			ScriptsComboBoxItem item = new ScriptsComboBoxItem();
			item.EventName = eventName;
			System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentUICulture;
			if(ci.IetfLanguageTag != "en" && (ci.Parent == null || ci.Parent.IetfLanguageTag != "en")) {
				item.DefaultSuffix = scripts.GetDefaultSuffix(eventName);
			}
			return item;
		}
		internal string[] GetCompatibleMethodsNames(IScriptable scriptContainer, string eventName) {
			List<string> result = new List<string>();
			XRScriptsBase scripts = scriptContainer != null ? scriptContainer.Scripts : GetControlScripts();
			if(scripts == null)
				return result.ToArray();
			result.Add(DesignSR.DataGridNewString);
			result.AddRange(GetCompatibleMethodsNames(scripts.GenerateDefaultEventScript(eventName)));
			return result.ToArray();
		}
		public void SaveScripts() {
			if(report.ScriptsSource != this.Text && !DesignerHost.IsDebugging()) {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(report, "ScriptsSource");
				property.SetValue(report, this.Text);
				AdjustCommands();
			}
		}
	}
	public class ScriptSource : IScriptSource, IDisposable {
		XtraReport report;
		internal XtraReport SourceReport { get { return report; } }
		public ScriptSource(XtraReport report) {
			this.report = report;
		}
		#region IScriptSource Members
		public ScriptLanguage ScriptLanguage {
			get {
				return report.ScriptLanguage;
			}
			set {
				report.ScriptLanguage = value;
			}
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			report = null;
		}
		#endregion
	}
	public interface IScriptEditor {
		string Text { get; set; }
		int LinesCount { get; }
		bool Modified { get; set; }
		void AppendText(string text);
		void HighlightErrors(CompilerErrorCollection errors);
		void SetCaretPosition(int line, int column);
	}
	public interface IScriptEditorService {
		IScriptEditor CreateEditor(IServiceProvider serviceProvider);
	}
}
