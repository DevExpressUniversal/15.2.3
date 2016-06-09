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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.FeatureBrowser;
namespace DevExpress.XtraEditors.Design.TasksSolution {
	[ToolboxItem(false)]
	public abstract class TasksSolutionFormBase : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		XtraTab.XtraTabControl tabControl;
		SplitContainerControl splitContainerMain;
		Panel pnlTaskTop;
		Panel pnlTaskList;
		ListBoxControl lbTasks;
		TaskNameItemCollection taskNameItems;
		LabelInfo lblTaskDescription;
		Label lblTaskFilter;
		TextEdit edTaskFilter;
		SimpleButton btnStartTask;
		Label lbAdditionalDirectory;
		ButtonEdit beAdditionDirectory;
		Label lbSearchOntheWeb;
		ButtonEdit beSearchOntheWeb;
		Hashtable stepFrames;
		string additionalDirectory;
		private System.ComponentModel.Container components = null;
		public TasksSolutionFormBase() {
			this.additionalDirectory = string.Empty;
			this.stepFrames = new Hashtable();
			this.taskNameItems = null;
			CreateControls();
			AddStepFrames();
		}
		protected abstract string[] GetXmlFiles();
		protected virtual string ShortProductName { get { return string.Empty; } }
		protected virtual string KnoledgeBasePath { get { return "http://www.devexpress.com/Support/KnowledgeBase/SearchResults.xml?kbss="; } }
		protected virtual void AddStepFrames() {
			AddStepFrame(string.Empty, typeof(FeatureBrowserDefaultPageBase));
			AddStepFrame("Default", typeof(FeatureBrowserDefaultPageBase));
		}
		protected void AddStepFrame(string name, Type stepFrameType) {
			this.stepFrames[name] = stepFrameType;
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			LoadTasks();
		}
		protected void LoadTasks() {
			ArrayList fileList = new ArrayList(GetXmlFiles());
			if(AdditionalDirectory != string.Empty) {
				string[] files = System.IO.Directory.GetFiles(AdditionalDirectory, "*.xml");
				for(int i = 0; i < files.Length; i ++)
					fileList.Add(files[i]);
			}
			TaskItems taskItems = new TaskItemsCreator(EditingObject.GetType()).LoadFromXmls((string[])fileList.ToArray(typeof(string)));
			taskNameItems = taskItems.CreateTaskNameItemCollection();
			FillTasksListBox();
		}
		public override void StoreLocalProperties(PropertyStore localStore) {
			base.StoreGlobalProperties(localStore);
			localStore.AddProperty("TaskSolutionAdditionalDirectory", AdditionalDirectory);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			AdditionalDirectory = (string)localStore.RestoreProperty("TaskSolutionAdditionalDirectory", string.Empty);
		}
		protected string AdditionalDirectory {
			get { return additionalDirectory; }
			set {
				additionalDirectory = value;
				if(beAdditionDirectory != null)
					beAdditionDirectory.Text = value;
			}
		}
		void CreateControls() {
			tabControl = new XtraTab.XtraTabControl();
			tabControl.Parent = this.pnlMain;
			tabControl.TabPages.Add("General");
			tabControl.TabPages.Add("Additional");
			tabControl.Dock = DockStyle.Fill;
			CreateGeneralTabControls(tabControl.TabPages[0]);
			CreateAdditionalTabControls(tabControl.TabPages[1]);
		}
		void CreateGeneralTabControls(Control tabPage) {
			splitContainerMain = new SplitContainerControl();
			splitContainerMain.Parent = tabPage;
			splitContainerMain.Dock = DockStyle.Fill;
			splitContainerMain.Horizontal = false;
			splitContainerMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainerMain.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainerMain.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainerMain.FixedPanel = SplitFixedPanel.None;
			splitContainerMain.SplitterPosition = Height  * 3 / 5;
			splitContainerMain.BringToFront();
			splitContainerMain.Panel1.DockPadding.All = 2;
			pnlTaskList = new Panel();
			pnlTaskList.Parent = splitContainerMain.Panel1;
			pnlTaskList.Dock = DockStyle.Fill;
			lbTasks = new ListBoxControl();
			lbTasks.Parent = pnlTaskList;
			lbTasks.Left = 0;
			lbTasks.Top = 0;
			lbTasks.SelectedIndexChanged += new EventHandler(OnListTaskSelectedIndexChanged);
			lbTasks.DoubleClick += new EventHandler(OnListTaskDoubleClick);
			lbTasks.KeyDown += new KeyEventHandler(OnListTaskKeyDown);
			btnStartTask = new SimpleButton();
			btnStartTask.Parent = pnlTaskList;
			btnStartTask.Top = 0;
			btnStartTask.Width = 80;
			btnStartTask.Text = "Run";
			btnStartTask.Click += new EventHandler(OnButtonStartTaskClick);
			pnlTaskTop = new Panel();
			pnlTaskTop.Parent = splitContainerMain.Panel1;
			pnlTaskTop.Height = 30;
			pnlTaskTop.Dock = DockStyle.Top;
			lblTaskFilter = new Label();
			lblTaskFilter.Parent = pnlTaskTop;
			lblTaskFilter.Left = 0;
			lblTaskFilter.Text = "Filter by ";
			lblTaskFilter.AutoSize = true;
			edTaskFilter = new TextEdit();
			edTaskFilter.Parent = pnlTaskTop;
			edTaskFilter.Left = lblTaskFilter.Right + 5;
			edTaskFilter.TextChanged += new EventHandler(OnTaskFilterTextChanged);
			lblTaskDescription = new LabelInfo();
			lblTaskDescription.Parent = splitContainerMain.Panel2;
			lblTaskDescription.Dock = DockStyle.Fill;
		}
		void CreateAdditionalTabControls(Control tabPage) {
			lbAdditionalDirectory = new Label();
			lbAdditionalDirectory.Text = "Additional tasks location:";
			beAdditionDirectory = new ButtonEdit();
			SetAdditionControlsLayout(tabPage, lbAdditionalDirectory, beAdditionDirectory, 0);
			beAdditionDirectory.Properties.ReadOnly = true;
			beAdditionDirectory.ButtonClick += new ButtonPressedEventHandler(OnAdditionalDirrectoryButtonClick);
			lbSearchOntheWeb = new Label();
			lbSearchOntheWeb.Text = "Search in knowledge base on www.devexpress.com:";
			beSearchOntheWeb = new ButtonEdit();
			SetAdditionControlsLayout(tabPage, lbSearchOntheWeb, beSearchOntheWeb, 1);
			this.beSearchOntheWeb.Properties.Buttons[0].Enabled = false;
			beSearchOntheWeb.ButtonClick += new ButtonPressedEventHandler(OnSearchOntheWebButtonClick);
			beSearchOntheWeb.TextChanged += new EventHandler(OnSearchOnWebTextChanged);
			beSearchOntheWeb.KeyDown += new KeyEventHandler(OnSearchOnWebKeyDown);
		}
		void SetAdditionControlsLayout(Control parent, Label label, Control edit, int index) {
			const int itemHeight = 30;
			label.Parent = parent;
			label.AutoSize = true;
			label.Left = 2;
			label.Top = itemHeight * index + (itemHeight - label.Height) / 2;
			edit.Parent = parent;
			edit.Left = label.Right + 2;
			edit.Top = itemHeight * index + (itemHeight - edit.Height) / 2;
		}
		protected override void OnLoad(EventArgs e) {
			btnStartTask.Left = pnlTaskList.ClientSize.Width - btnStartTask.Width;
			btnStartTask.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			lbTasks.Width = btnStartTask.Left - lbTasks.Left - 5;
			lbTasks.Height = pnlTaskList.ClientSize.Height;
			lbTasks.Anchor =  AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			edTaskFilter.Top = (pnlTaskTop.ClientSize.Height - edTaskFilter.Height) / 2;
			lblTaskFilter.Top = edTaskFilter.Top + (edTaskFilter.Height - lblTaskFilter.Height) / 2;
			edTaskFilter.Width = lbTasks.Right - edTaskFilter.Left;
			edTaskFilter.Anchor = AnchorStyles.Right | AnchorStyles.Left;
			beAdditionDirectory.Width = tabControl.TabPages[1].ClientSize.Width - beAdditionDirectory.Left - 2;
			beAdditionDirectory.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			beSearchOntheWeb.Width = tabControl.TabPages[1].ClientSize.Width - beSearchOntheWeb.Left - 2;
			beSearchOntheWeb.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			base.OnLoad(e);
		}
		protected TaskItem SelectedItem { get { return lbTasks.SelectedItem != null ? ((TaskNameItem)lbTasks.SelectedItem).Item : null; } }
		void FillTasksListBox() {
			if(taskNameItems == null) return;
			string[] filters = edTaskFilter.Text.ToUpper().Split(new char[] {' '});
			lbTasks.BeginUpdate();
			try {
				lbTasks.Items.Clear();
				for(int i = 0; i < taskNameItems.Count; i ++) {
					AddTaskToListBox(filters, taskNameItems[i]);
				}
			}
			finally {
				lbTasks.EndUpdate();
			}
			if(lbTasks.Items.Count > 0)
				lbTasks.SelectedIndex = 0;
			else OnSelectedItemChanged();
		}
		void AddTaskToListBox(string[] filters, TaskNameItem taskNameItem) {
			if(IsFilterApply(taskNameItem.Name.ToUpper(), filters))
				lbTasks.Items.Add(taskNameItem);
		}
		bool IsFilterApply(string name, string[] filters) {
			if(filters.Length == 0) return true;
			for(int i = 0; i < filters.Length; i ++)
				if(filters[i] != string.Empty && name.IndexOf(filters[i]) < 0)
					return false;
			return true;
		}
		void OnListTaskSelectedIndexChanged(object sender, EventArgs e) {
			OnSelectedItemChanged();
		}
		void OnTaskFilterTextChanged(object sender, EventArgs e) {
			FillTasksListBox();
		}
		void OnButtonStartTaskClick(object sender, EventArgs e) {
			Setup();
		}
		void OnListTaskDoubleClick(object sender, EventArgs e) {
			Setup();
		}
		void OnListTaskKeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter)
				Setup();
		}
		public class FolderBrowserEx : System.Windows.Forms.Design.FolderNameEditor {
			System.Windows.Forms.Design.FolderNameEditor.FolderBrowser folderBrowser;
			public FolderBrowserEx() {
				folderBrowser = new System.Windows.Forms.Design.FolderNameEditor.FolderBrowser();
			}																			   
			public System.Windows.Forms.DialogResult ShowDialog() {
				return folderBrowser.ShowDialog();
			}
			public string DirectoryPath { get { return folderBrowser.DirectoryPath; } }
		}
		void OnAdditionalDirrectoryButtonClick(object sender, ButtonPressedEventArgs e) {
			FolderBrowserEx browser = new FolderBrowserEx();
			if(browser.ShowDialog() == DialogResult.OK) {
				AdditionalDirectory = browser.DirectoryPath;
				LoadTasks();
			}
		}
		void OnSearchOntheWebButtonClick(object sender, ButtonPressedEventArgs e) {
			SearchOntheWeb();
		}
		void OnSearchOnWebTextChanged(object sender, EventArgs e) {
			this.beSearchOntheWeb.Properties.Buttons[0].Enabled = this.beSearchOntheWeb.Text.Length > 0;
		}
		void OnSearchOnWebKeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter)
				SearchOntheWeb();
		}
		void SearchOntheWeb() {
			if(beSearchOntheWeb.Text == string.Empty) return;
			string[] filters = beSearchOntheWeb.Text.Split(new char[] {' '});
			string filterString = string.Empty;
			for(int i = 0; i < filters.Length; i ++) {
				filterString += filters[i];
				if(i < filters.Length - 1)
					filterString += '+';
			}
			string productFilter = ShortProductName != string.Empty ? "&prod=" + ShortProductName : string.Empty;
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = KnoledgeBasePath + filterString + productFilter;
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			process.Start();
		}
		void OnSelectedItemChanged() {
			btnStartTask.Enabled = SelectedItem != null;
			lblTaskDescription.SuspendLayout();
			try {
				lblTaskDescription.Texts.Clear();
				if(SelectedItem != null)
					lblTaskDescription.Texts.Add(SelectedItem.Description);
			}
			finally {
				lblTaskDescription.ResumeLayout();
			}
		}
		void Setup() {
			if(SelectedItem == null) return;
			TaskRunner runner = new TaskRunner(SelectedItem, EditingObject, this, this.stepFrames);
			runner.Run();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing )	{
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
	}
}
