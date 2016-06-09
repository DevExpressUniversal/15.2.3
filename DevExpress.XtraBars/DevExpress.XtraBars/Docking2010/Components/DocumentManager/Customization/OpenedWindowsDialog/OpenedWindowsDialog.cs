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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Internal;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class OpenedWindowsDialog : ItemsManagementDialog {
		OpenedWindowsDialog() : base(null) {  }
		public OpenedWindowsDialog(BaseView view)
			: base(view) {
			Owner = DocumentsHostContext.GetForm(View.Manager);
			Text = GetLocalizedString(DocumentManagerStringId.OpenedWindowsDialogCaption);
			DataSource = DocumentCollection;
			DataListBox.SelectedItem = View.ActiveDocument;
			DataListBox.FilterPrefix = "true";
		}
		public IEnumerable<BaseDocument> DocumentCollection {
			get { return GetDocumentCollection(View.Manager); }
		}
		protected internal IEnumerable<BaseDocument> GetDocumentCollection(DocumentManager manager) {
			foreach(BaseDocument document in manager.ActivationInfo.DocumentActivationList) {
				if(CanAddDocument(document))
					yield return document;
			}
		}
		bool CanAddDocument(BaseDocument document) {
			bool result = (document != null) && !document.IsDisposing;
			var tabbedDocument = document as Views.Tabbed.Document;
			if(result && tabbedDocument != null)
				result &= tabbedDocument.Properties.CanShowInDocumentSelector;
			return result;
		}
		protected override void Dispose(bool disposing) {
			if(View != null && View.WindowsDialogProperties != null) {
				View.WindowsDialogProperties.NameColumnWidth = DataListBox.Properties.Columns[0].Width;
				View.WindowsDialogProperties.PathColumnWidth = DataListBox.Properties.Columns[1].Width;
				View.WindowsDialogProperties.Size = Size;
			}
			base.Dispose(disposing);
		}
		protected override void ApplyPanelResources(PanelControl panelControl, System.ComponentModel.ComponentResourceManager resources) {
			base.ApplyPanelResources(panelControl, resources);
			resources.ApplyResources(panelControl, "windowsPanelControl");
		}
		public BaseView View {
			get { return EditingObject as BaseView; }
		}
		public BaseDocument Result {
			get { return DataListBox.SelectedItem as BaseDocument; }
		}
		internal void UpdateDocuments(object documents) {
			DataSource = documents;
			DataListBox.Update();
		}
		protected override Size GetFormSize() {
			return GetViewProperty(view => view.WindowsDialogProperties.Size, new Size(400, 300));
		}
		protected override int GetCaptionColumnWidth() {
			return GetViewProperty(view => view.WindowsDialogProperties.NameColumnWidth, 0);
		}
		protected override int GetPathColumnWidth() {
			return GetViewProperty(view => view.WindowsDialogProperties.PathColumnWidth, 0);
		}
		string GetPathColumnCaption() {
			return GetViewProperty(view => view.WindowsDialogProperties.PathColumnCaption,
				GetLocalizedString(DocumentManagerStringId.OpenedWindowsDialogPathColumnCaption));
		}
		string GetNameColumnCaption() {
			return GetViewProperty(view => view.WindowsDialogProperties.NameColumnCaption,
				GetLocalizedString(DocumentManagerStringId.OpenedWindowsDialogNameColumnCaption));
		}
		protected string GetViewProperty(Func<BaseView, string> accessor, string defaultValue) {
			if(View != null && !string.IsNullOrEmpty(accessor(View)))
				return accessor(View);
			return defaultValue;
		}
		protected T GetViewProperty<T>(Func<BaseView, T?> accessor, T defaultValue)
			where T : struct {
			if(View != null)
				return accessor(View).GetValueOrDefault(defaultValue);
			return defaultValue;
		}
		protected override IEnumerable<BaseCommand> GetCommands() {
			return new BaseCommand[] { 
				new ActivateDocumentCommand(View, this),
				new CloseDocumentCommand(View, this),
				new CloseAllDocumentsCommand(View, this)
			};
		}
		protected override void InitColumns() {
			this.DataListBox.Properties.Columns.AddRange(new LookUpColumnInfo[] {
				new LookUpColumnInfo("Caption",  GetNameColumnCaption()) { Width = GetCaptionColumnWidth() },
				new LookUpColumnInfo("Footer", GetPathColumnCaption()) { Width = GetPathColumnWidth() },
				new LookUpColumnInfo("IsVisible", "Visible") { Visible = false }
			});
		}
	}
	public class ItemsManagementDialog : XtraForm {
		ItemsManagementDialog() { 
			InitializeComponent();
		}
		const int DefaultListWidth = 350;
		const int DefaultButtonHeight = 22;
		const int DefaultFormHeight = 300;
		IEnumerable<BaseCommand> commandsCore;
		protected ItemsManagementDialog(object editObject) {
			EditingObject = editObject;
			InitializeComponent();
			this.commandsCore = GetCommands();
			GenerateButtonsByCommands(Commands);
			InitColumns();
			Subscribe();
			SetActualSizes();
		}
		protected override void Dispose(bool disposing) {
			Unsubscribe();
			base.Dispose(disposing);
		}
		public IEnumerable<BaseCommand> Commands {
			get { return commandsCore; }
		}
		protected object EditingObject { get; set; }
		protected object DataSource {
			get { return documentListBox.Properties.DataSource; }
			set { documentListBox.Properties.DataSource = value; }
		}
		protected virtual IEnumerable<BaseCommand> GetCommands() {
			return new List<BaseCommand>();
		}
		protected virtual BaseCommand GetDefaultCommand() {
			return GetCommands().FirstOrDefault();
		}
		protected static internal string GetLocalizedString(DocumentManagerStringId id) {
			return DocumentManagerLocalizer.GetString(id);
		}
		protected IMultiColumnListBox DataListBox {
			get { return documentListBox; }
		}
		protected virtual object GetCurrentItem() {
			return documentListBox.SelectedItem;
		}
		protected void Subscribe() {
			if(documentListBox != null)
				documentListBox.ItemDoubleClick += OnItemDoubleClick;
			if(pnlCommands != null)
				pnlCommands.MouseWheel += OnMouseWheel;
			if(pnlList != null)
				pnlList.MouseWheel += OnMouseWheel;
		}
		protected void Unsubscribe() {
			if(documentListBox != null)
				documentListBox.ItemDoubleClick -= OnItemDoubleClick;
			if(pnlCommands != null)
				pnlCommands.MouseWheel -= OnMouseWheel;
			if(pnlList != null)
				pnlList.MouseWheel -= OnMouseWheel;
		}
		protected virtual void SetActualSizes() {
			Size defaultSize = CalcDefaultSize();
			Size savedSize = GetFormSize();
			MinimumSize = new Size(388, defaultSize.Height);
			if(savedSize.Width > defaultSize.Width || savedSize.Height > defaultSize.Height)
				Size = savedSize;
			else
				Size = defaultSize;
		}
		protected virtual Size CalcDefaultSize() {
			cmdBtnsBottomContainer.Controls[0].Size = cmdBtnsBottomContainer.DisplayRectangle.Size;
			Size topPanel = cmdBtnsTopContainer.GetMinSize();
			Size bottomPanel = cmdBtnsBottomContainer.GetMinSize();
			Size panelsSize = new Size(topPanel.Width, bottomPanel.Height + topPanel.Height);
			pnlCommands.Width = panelsSize.Width + pnlCommands.Padding.Horizontal;
			int height = (Size.Height - ClientSize.Height) + panelsSize.Height + pnlCommands.Padding.Vertical + 60;
			return new Size(Width, height);
		}
		protected virtual void ApplyPanelResources(PanelControl panelControl, ComponentResourceManager resources) {
			resources.ApplyResources(panelControl, "panelControl");
		}
		#region Windows Form Designer generated code
		protected virtual void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenedWindowsDialog));
			this.pnlCommands = new DevExpress.XtraEditors.PanelControl();
			this.pnlList = new DevExpress.XtraEditors.PanelControl();
			this.cmdBtnsBottomContainer = new StackPanelControl();
			this.cmdBtnsTopContainer = new StackPanelControl();
			this.documentListBox = DevExpress.XtraEditors.MultiColumnListBoxCreator.CreateMultiColumnListBox();
			((System.ComponentModel.ISupportInitialize)(this.pnlCommands)).BeginInit();
			this.pnlCommands.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlList)).BeginInit();
			this.pnlList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmdBtnsTopContainer)).BeginInit();
			this.cmdBtnsTopContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmdBtnsBottomContainer)).BeginInit();
			this.cmdBtnsBottomContainer.SuspendLayout();
			this.SuspendLayout();
			this.pnlCommands.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			ApplyPanelResources(pnlCommands, resources);
			this.pnlCommands.Name = "pnlCommands";
			this.pnlCommands.TabIndex = 10;
			this.pnlCommands.Controls.AddRange(new[] { cmdBtnsTopContainer, cmdBtnsBottomContainer });
			this.pnlList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlList.Controls.Add(this.documentListBox.MultiColumnListBox);
			this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlList.Name = "pnlList";
			this.pnlList.Padding = new Padding(12, 12, 0, 12);
			this.pnlList.TabIndex = 11;
			this.cmdBtnsTopContainer.ContentOrientation = System.Windows.Forms.Orientation.Vertical;
			this.cmdBtnsTopContainer.Size = new Size(1,1);
			this.cmdBtnsTopContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmdBtnsTopContainer.ItemIndent = 5;
			this.cmdBtnsTopContainer.TabIndex = 1;
			this.cmdBtnsBottomContainer.ContentOrientation = System.Windows.Forms.Orientation.Horizontal;
			this.cmdBtnsBottomContainer.Size = new Size(1, 1);
			this.cmdBtnsBottomContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.cmdBtnsBottomContainer.ItemIndent = 5;
			this.cmdBtnsBottomContainer.TabIndex = 0;
			this.documentListBox.MultiColumnListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.documentListBox.MultiColumnListBox.Location = new System.Drawing.Point(12, 12);
			this.documentListBox.MultiColumnListBox.Name = "documentListBox";
			this.documentListBox.MultiColumnListBox.Padding = new System.Windows.Forms.Padding(12, 12, 0, 12);
			this.documentListBox.MultiColumnListBox.Size = new System.Drawing.Size(488, 448);
			this.documentListBox.MultiColumnListBox.TabIndex = 9;
			this.documentListBox.Properties.SearchMode = SearchMode.AutoFilter;
			this.documentListBox.Properties.DisplayMember = "IsVisible";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Size = GetFormSize();
			this.Controls.Add(this.pnlList);
			this.Controls.Add(this.pnlCommands);
			this.StartPosition = FormStartPosition.CenterParent;
			this.Name = "openedWindowsDialog";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.pnlCommands)).EndInit();
			this.pnlCommands.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlList)).EndInit();
			this.pnlList.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmdBtnsTopContainer)).EndInit();
			this.cmdBtnsTopContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmdBtnsBottomContainer)).EndInit();
			this.cmdBtnsBottomContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		PanelControl pnlCommands;
		PanelControl pnlList;
		StackPanelControl cmdBtnsTopContainer;
		StackPanelControl cmdBtnsBottomContainer;
		IMultiColumnListBox documentListBox;
		#endregion
		protected void GenerateButtonsByCommands(IEnumerable<BaseCommand> commands) {
			foreach(BaseCommand command in commands) {
				var newButton = CreateCommandButton(Point.Empty, command);
				newButton.BindCommand(command, GetCurrentItem);
				documentListBox.SelectedItemChanged += command.RaiseCanExecuteChanged;
				cmdBtnsTopContainer.Controls.Add(newButton);
			}
			var okCommand = new OKCommand(this);
			var okButton = CreateCommandButton(Point.Empty, okCommand);
			okButton.BindCommand(okCommand);
			cmdBtnsBottomContainer.Controls.Add(okButton);
			okButton.Anchor |= AnchorStyles.Right;
			pnlCommands.Controls.Add(cmdBtnsTopContainer);
			pnlCommands.Controls.Add(cmdBtnsBottomContainer);
		}
		protected internal void Reparent(object editingObject) {
			EditingObject = editingObject;
			foreach(BaseCommand command in commandsCore)
				command.EditingObject = editingObject;
		}
		SimpleButton CreateCommandButton(Point location, BaseCommand command) {
			return new SimpleButton()
			{
				Text = command.Caption,
				Location = location,
				Margin = new Padding(2),
				Padding = new Padding(2),
				Size = new Size(pnlCommands.Width - pnlCommands.Padding.All * 2, DefaultButtonHeight)
			};
		}
		void OnMouseWheel(object sender, MouseEventArgs e) {
			DXMouseEventArgs ea = e as DXMouseEventArgs;
			if(ea != null)
				ea.Handled = false;
		}
		void OnItemDoubleClick(object sender, EventArgs e) {
			var defaultCommand = GetDefaultCommand();
			if(defaultCommand != null) {
				object item = GetCurrentItem();
				if(defaultCommand.CanExecute(item))
					defaultCommand.Execute(item);
			}
		}
		protected virtual Size GetFormSize() {
			return new Size(pnlCommands.Width + DefaultListWidth, DefaultFormHeight);
		}
		protected virtual int GetCaptionColumnWidth() { return 0; }
		protected virtual int GetPathColumnWidth() { return 0; }
		protected virtual void InitColumns() { }
	}
	public class WorkspacesDialog : ItemsManagementDialog {
		WorkspacesDialog() : base(null) {  }
		public WorkspacesDialog(WorkspaceManager workspaceManager)
			: base(workspaceManager) {
			Text = GetLocalizedString(DocumentManagerStringId.WorkspacesDialogCaption);
			DataSource = workspaceManager.Workspaces;
		}
		WorkspaceManager WorkspaceManager {
			get { return EditingObject as WorkspaceManager; }
		}
		protected override IEnumerable<BaseCommand> GetCommands() {
			return new BaseCommand[] { 
				new AcceptWorkspaceCommand(WorkspaceManager, this),
				new LoadCommand(WorkspaceManager, DataListBox.Update),
				new RemoveWorkspaceCommand(WorkspaceManager, DataListBox.Update),
				new RemoveAllWorkspacesCommand(WorkspaceManager, DataListBox.Update),
				new SaveCommand(WorkspaceManager, DataListBox.Update),
				new SaveAsCommand(WorkspaceManager, DataListBox.Update)
			};
		}
		protected override void InitColumns() {
			string NameColumnCaption = GetLocalizedString(DocumentManagerStringId.WorkspacesDialogNameColumnCaption);
			string PathColumnCaption = GetLocalizedString(DocumentManagerStringId.WorkspacesDialogPathColumnCaption);
			this.DataListBox.Properties.Columns.AddRange(new LookUpColumnInfo[] {
				new LookUpColumnInfo("Name",  NameColumnCaption),
				new LookUpColumnInfo("Path", PathColumnCaption)}
			);
		}
		protected override void ApplyPanelResources(PanelControl panelControl, System.ComponentModel.ComponentResourceManager resources) {
			base.ApplyPanelResources(panelControl, resources);
			resources.ApplyResources(panelControl, "workspacesPanelControl");
		}
		public IWorkspace Result {
			get { return DataListBox.SelectedItem as IWorkspace; }
		}
	}
	public abstract class BaseCommand {
		protected internal object EditingObject { get; set; }
		public virtual string Caption {
			get { return DocumentManagerLocalizer.GetString(CommandID); }
		}
		protected DocumentManagerStringId CommandID {
			get { return GetCommandID(); }
		}
		protected virtual DocumentManagerStringId GetCommandID() {
			return new DocumentManagerStringId();
		}
		public event EventHandler CanExecuteChanged;
		public void RaiseCanExecuteChanged(object sender, EventArgs e) {
			if(CanExecuteChanged != null)
				CanExecuteChanged(sender, e);
		}
		public bool CanExecute(object parameter) {
			return CanExecuteCore(parameter);
		}
		protected virtual bool CanExecuteCore(object parameter) {
			return false;
		}
		public void Execute(object parameter) {
			ExecuteCore(parameter);
		}
		protected virtual void ExecuteCore(object parameter) { }
	}
	class OKCommand : BaseCommand {
		public OKCommand(Form form) {
			Form = form;
		}
		protected Form Form {
			get;
			private set;
		}
		public override string Caption {
			get { return XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxOkButtonText); }
		}
		protected override bool CanExecuteCore(object parameter) {
			return true;
		}
		protected override void ExecuteCore(object parameter) {
			Form.DialogResult = DialogResult.OK;
		}
	}
	#region WorkspaceCommand
	class WorkspaceCommand : BaseCommand {
		Action updateRoutine;
		protected WorkspaceCommand(WorkspaceManager workspaceManager)
			: this(workspaceManager, null) {
		}
		protected WorkspaceCommand(WorkspaceManager workspaceManager, Action updateRoutine) {
			this.EditingObject = workspaceManager;
			this.updateRoutine = updateRoutine;
		}
		protected WorkspaceManager WorkspaceManager {
			get { return EditingObject as WorkspaceManager; }
		}
		protected override bool CanExecuteCore(object parameter) {
			return (WorkspaceManager != null) && (parameter is IWorkspace);
		}
		protected void DoUpdate() {
			if(updateRoutine != null)
				updateRoutine();
		}
	}
	class AcceptWorkspaceCommand : WorkspaceCommand {
		ItemsManagementDialog Dialog;
		public AcceptWorkspaceCommand(WorkspaceManager workspaceManager, ItemsManagementDialog dialog)
			: base(workspaceManager) {
			Dialog = dialog;
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.ApplyWorkspaceButtonText;
		}
		protected override bool CanExecuteCore(object parameter) {
			return base.CanExecuteCore(parameter) && (WorkspaceManager.Workspaces.Count != 0);
		}
		protected override void ExecuteCore(object parameter) {
			if(Dialog != null) Dialog.DialogResult = DialogResult.OK;
		}
	}
	class RemoveWorkspaceCommand : WorkspaceCommand {
		public RemoveWorkspaceCommand(WorkspaceManager workspaceManager, Action updateRoutine)
			: base(workspaceManager, updateRoutine) {
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.RemoveWorkspaceButtonText;
		}
		protected override void ExecuteCore(object parameter) {
			IWorkspace workspace = parameter as IWorkspace;
			string warningMessage = String.Format(DocumentManagerLocalizer.GetString(DocumentManagerStringId.WorkspaceRemoveWarningMessage), workspace.Name);
			if(XtraMessageBox.Show(warningMessage, Caption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
				WorkspaceManager.RemoveWorkspace(workspace.Name);
				DoUpdate();
			}
		}
	}
	class RemoveAllWorkspacesCommand : WorkspaceCommand {
		public RemoveAllWorkspacesCommand(WorkspaceManager workspaceManager, Action updateRoutine)
			: base(workspaceManager, updateRoutine) {
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.RemoveAllWorkspacesButtonText;
		}
		protected override bool CanExecuteCore(object parameter) {
			return (WorkspaceManager != null) && (WorkspaceManager.Workspaces.Count != 0);
		}
		protected override void ExecuteCore(object parameter) {
			string warningMessage = DocumentManagerLocalizer.GetString(DocumentManagerStringId.RemoveAllWorkspacesWarningMessage);
			if(XtraMessageBox.Show(warningMessage, Caption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
				for(int i = WorkspaceManager.Workspaces.Count - 1; i >= 0; i--)
					WorkspaceManager.RemoveWorkspace(WorkspaceManager.Workspaces[i].Name);
				DoUpdate();
			}
		}
	}
	class LoadCommand : WorkspaceCommand {
		public LoadCommand(WorkspaceManager workspaceManager, Action updateRoutine)
			: base(workspaceManager, updateRoutine) {
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.LoadWorkspaceButtonText;
		}
		protected override bool CanExecuteCore(object parameter) {
			return (WorkspaceManager != null);
		}
		protected override void ExecuteCore(object parameter) {
			OpenLoadFileDialog();
		}
		public void OpenLoadFileDialog() {
			using(FileDialog fileDialog = new OpenFileDialog()) {
				fileDialog.Filter = "Layouts (*.xml)|*.xml";
				fileDialog.Title = DocumentManagerLocalizer.GetString(DocumentManagerStringId.LoadWorkspaceFormCaption);
				fileDialog.CheckFileExists = true;
				if(fileDialog.ShowDialog() == DialogResult.OK) {
					string path = fileDialog.FileName;
					string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
					if(WorkspaceManager.LoadWorkspace(fileName, path))
						DoUpdate();
				}
			}
		}
	}
	class SaveCommand : SaveAsCommand {
		public SaveCommand(WorkspaceManager workspaceManager, Action updateRoutine)
			: base(workspaceManager, updateRoutine) {
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.SaveWorkspaceButtonText;
		}
		protected override void ExecuteCore(object parameter) {
			IWorkspace workspace = parameter as IWorkspace;
			if(workspace == null) {
				base.ExecuteCore(parameter);
				return;
			}
			string path = workspace.Path;
			if(path != null) {
				WorkspaceManager.CaptureWorkspace(workspace.Name);
				if(WorkspaceManager.SaveWorkspace(workspace.Name, path))
					DoUpdate();
			}
			else base.ExecuteCore(parameter);
		}
	}
	class SaveAsCommand : WorkspaceCommand {
		public SaveAsCommand(WorkspaceManager workspaceManager, Action updateRoutine)
			: base(workspaceManager, updateRoutine) {
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.SaveWorkspaceAsButtonText;
		}
		protected override bool CanExecuteCore(object parameter) {
			return (WorkspaceManager != null);
		}
		protected override void ExecuteCore(object parameter) {
			OpenSaveFileDialog(parameter as IWorkspace);
		}
		public void OpenSaveFileDialog(IWorkspace workspace) {
			using(FileDialog fileDialog = new SaveFileDialog()) {
				fileDialog.Filter = "Layouts (*.xml)|*.xml";
				fileDialog.Title = DocumentManagerLocalizer.GetString(DocumentManagerStringId.SaveWorkspaceFormCaption);
				string workspaceName = "newWorkspace";
				if(workspace != null) {
					workspaceName = workspace.Name;
					workspaceName = workspaceName.TrimEnd(new char[] { '.' });
				}
				fileDialog.FileName = workspaceName;
				if(fileDialog.ShowDialog() == DialogResult.OK) {
					string path = fileDialog.FileName;
					string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
					if(WorkspaceManager.SaveWorkspace(fileName, path, true))
						DoUpdate();
				}
			}
		}
	}
	#endregion
	#region DocumentCommand
	class DocumentCommand : BaseCommand {
		ItemsManagementDialog Dialog;
		public DocumentCommand(BaseView view, ItemsManagementDialog dialog) {
			EditingObject = view;
			Dialog = dialog;
		}
		protected BaseView View {
			get { return EditingObject as BaseView; }
		}
		protected override bool CanExecuteCore(object parameter) {
			return (View != null) && (View.Documents.Count != 0) && (parameter is BaseDocument);
		}
		protected void DialogOK() {
			if(Dialog != null) Dialog.DialogResult = DialogResult.OK;
		}
		protected void DialogCloseIfViewEmpty() {
			DialogCloseIfViewEmpty(View.Manager);
		}
		protected void DialogCloseIfViewEmpty(DocumentManager manager) {
			if(Dialog == null) return;
			var owd = Dialog as OpenedWindowsDialog;
			if(owd == null || manager == null || manager.IsDisposing)
				return;
			var documents = owd.GetDocumentCollection(manager).ToList();
			if(documents.Count == 0) {
				if(Dialog.Owner != null)
					Dialog.Owner.RemoveOwnedForm(Dialog);
				Dialog.Close();
			}
			else owd.UpdateDocuments(documents);
		}
		protected void DialogCheckOwnerBeforeClosing(BaseDocument document, BaseView view) {
			if(Dialog == null) return;
			var hostContext = view.Manager.GetDocumentsHostContext();
			if(hostContext != null) {
				if(view.Documents.Count == 1 && view.Documents.Contains(document) && document.Properties.CanClose) {
					if(view.Manager != hostContext.startupManager) {
						if(Dialog.Owner != null)
							Dialog.Owner.RemoveOwnedForm(Dialog);
						Dialog.Owner = DocumentsHostContext.GetForm(hostContext.startupManager);
						Dialog.Reparent(hostContext.startupManager.View);
					}
				}
			}
		}
		protected void DialogCheckOwnerBeforeClosingAll(BaseView view) {
			if(Dialog == null) return;
			var hostContext = view.Manager.GetDocumentsHostContext();
			if(hostContext != null) {
				if(view.Documents.All(d => d.Properties.CanClose)) {
					if(view.Manager != hostContext.startupManager) {
						if(Dialog.Owner != null)
							Dialog.Owner.RemoveOwnedForm(Dialog);
						Dialog.Owner = DocumentsHostContext.GetForm(hostContext.startupManager);
						Dialog.Reparent(hostContext.startupManager.View);
					}
				}
			}
		}
	}
	class ActivateDocumentCommand : DocumentCommand {
		public ActivateDocumentCommand(BaseView view, ItemsManagementDialog dialog)
			: base(view, dialog) {
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.ActivateDocumentButtonText;
		}
		protected override bool CanExecuteCore(object parameter) {
			BaseDocument document = parameter as BaseDocument;
			return base.CanExecuteCore(parameter) && document.CanActivate();
		}
		protected override void ExecuteCore(object parameter) {
			DialogOK();
		}
	}
	class CloseDocumentCommand : DocumentCommand {
		public CloseDocumentCommand(BaseView view, ItemsManagementDialog dialog)
			: base(view, dialog) {
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.CloseDocumentButtonText;
		}
		protected override bool CanExecuteCore(object parameter) {
			BaseDocument document = parameter as BaseDocument;
			return base.CanExecuteCore(parameter) && document.Properties.CanClose;
		}
		protected override void ExecuteCore(object parameter) {
			BaseDocument document = parameter as BaseDocument;
			string warningMessage = string.Format(DocumentManagerLocalizer.GetString(DocumentManagerStringId.CloseDocumentWarningMessage), document.Caption);
			if(XtraMessageBox.Show(warningMessage, Caption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
				BaseView actualView = View.Controller.GetView(document);
				if(actualView != null) {
					var context = actualView.Manager.GetDocumentsHostContext();
					DialogCheckOwnerBeforeClosing(document, actualView);
					if(actualView.Controller.Close(document)) {
						if(context != null)
							DialogCloseIfViewEmpty(actualView.Manager ?? context.startupManager);
						else
							DialogCloseIfViewEmpty(actualView.Manager);
					}
				}
			}
		}
	}
	class CloseAllDocumentsCommand : DocumentCommand {
		public CloseAllDocumentsCommand(BaseView view, ItemsManagementDialog dialog)
			: base(view, dialog) {
		}
		protected override DocumentManagerStringId GetCommandID() {
			return DocumentManagerStringId.CloseAllDocumentsButtonText;
		}
		protected override bool CanExecuteCore(object parameter) {
			return (View != null) && View.CanCloseAll();
		}
		protected override void ExecuteCore(object parameter) {
			string warningMessage = DocumentManagerLocalizer.GetString(DocumentManagerStringId.CloseAllDocumentsWarningMessage);
			if(XtraMessageBox.Show(warningMessage, Caption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
				DialogCheckOwnerBeforeClosingAll(View);
				if(View.Controller.CloseAllDocumentsAndHosts())
					DialogCloseIfViewEmpty();
			}
		}
	}
	#endregion
}
