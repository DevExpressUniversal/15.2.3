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

using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using System.IO;
using System.Collections;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Utils.Frames;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;
using System.Threading;
using DevExpress.Xpo.Helpers;
namespace DevExpress.Xpo.Design {
	public enum Language { CSharp, VisualBasic }
	public class AddClassesFromDbWizardSaveToFile : AddClassesFromDbWizard {
		public AddClassesFromDbWizardSaveToFile()
			: base() {
		}
		public override bool secondConfiguration {
			get {
				return true;
			}
		}
	}
	public class AddClassesFromDbWizard : IWizard {
		bool wizardOK;
		protected bool WizardOK {
			get { return wizardOK; }
			set { wizardOK = value; }
		}
		public virtual bool secondConfiguration { get { return false; } }
		Language language;
		protected Language Language { get { return language; } set { language = value; } }
		public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem) {
		}
		public void ProjectFinishedGenerating(EnvDTE.Project project) {
		}
		public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem) {
		}
		public void RunFinished() {
		}
		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
			XpoDefault.RegisterBonusProviders();
			if(runKind == WizardRunKind.AsNewItem) {
				try {
					WizardOK = false;
					if(secondConfiguration) {
						using(AddClassesFromDbForm wizard = new AddClassesFromDbForm(Language)) {
							if(wizard.ShowDialog() == DialogResult.OK) {
								SaveXPClassesForm saveXPForm = new SaveXPClassesForm();
								if(saveXPForm.ShowDialog() == DialogResult.OK) {
									wizard.Language = saveXPForm.Language;
									File.WriteAllText(saveXPForm.FilePath, wizard.Code);
								}
								if(File.Exists(saveXPForm.FilePath)) {
									WizardOK = true;
								}
							}
						}
					} else {
						using(AddClassesFromDbForm wizard = new AddClassesFromDbForm(Language)) {
							if(wizard.ShowDialog() == DialogResult.OK) {
								WizardOK = true;
								replacementsDictionary.Add("$code$", wizard.Code);
							}
						}
					}
				}
				catch(Exception ex) {
					XtraMessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}
		public bool ShouldAddProjectItem(string filePath) {
			return WizardOK;
		}
	}
	internal class SaveXPClassesForm {
		public string FilePath { get { return filePathText.Text; } }
		public Language Language {
			get {
				switch(languageBox.SelectedIndex) {
					case 0:
						return Language.CSharp;
					case 1:
						return Language.VisualBasic;
					default:
						throw new Exception("");
				}
			}
		}
		public SaveXPClassesForm() {
			InitializeForm();
		}
		public DialogResult ShowDialog() {
			return form.ShowDialog();
		}
		XtraForm form = new XtraForm();
		TextEdit filePathText = new DevExpress.XtraEditors.TextEdit();
		DevExpress.XtraEditors.ComboBoxEdit languageBox = new DevExpress.XtraEditors.ComboBoxEdit();
		SimpleButton filePathBrowseButton = new DevExpress.XtraEditors.SimpleButton();
		LabelControl languageLabel = new DevExpress.XtraEditors.LabelControl();
		LabelControl filePathLabel = new DevExpress.XtraEditors.LabelControl();
		SimpleButton saveButton = new DevExpress.XtraEditors.SimpleButton();
		SimpleButton cancelButton = new DevExpress.XtraEditors.SimpleButton();
		LabelControl lineLabel = new DevExpress.XtraEditors.LabelControl();
		void InitializeForm() {
			languageBox.Location = new System.Drawing.Point(24, 46);
			languageBox.Name = "languageBox";
			languageBox.Properties.Items.AddRange(new object[] {
			"C#",
			"Visual Basic"});
			languageBox.Size = new System.Drawing.Size(155, 20);
			languageBox.TabIndex = 3;
			languageBox.SelectedIndex = 0;
			filePathText.Location = new System.Drawing.Point(24, 99);
			filePathText.Name = "filePathText";
			filePathText.Size = new System.Drawing.Size(271, 20);
			filePathText.TabIndex = 4;
			filePathText.EditValueChanged += new EventHandler(filePathText_EditValueChanged);
			filePathBrowseButton.Location = new System.Drawing.Point(301, 97);
			filePathBrowseButton.Name = "filePathBrowseButton";
			filePathBrowseButton.Size = new System.Drawing.Size(75, 23);
			filePathBrowseButton.TabIndex = 5;
			filePathBrowseButton.Text = "&Browse...";
			filePathBrowseButton.Click += new EventHandler(filePathBrowseButton_Click);
			languageLabel.Location = new System.Drawing.Point(24, 27);
			languageLabel.Name = "languageLabel";
			languageLabel.Size = new System.Drawing.Size(110, 13);
			languageLabel.TabIndex = 3;
			languageLabel.Text = "&Programming language:";
			filePathLabel.Location = new System.Drawing.Point(24, 80);
			filePathLabel.Name = "filePathLabel";
			filePathLabel.Size = new System.Drawing.Size(117, 13);
			filePathLabel.TabIndex = 4;
			filePathLabel.Text = "&File name:";
			saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			saveButton.Location = new System.Drawing.Point(232, 144);
			saveButton.Name = "saveButton";
			saveButton.Size = new System.Drawing.Size(75, 23);
			saveButton.TabIndex = 6;
			saveButton.Text = "&Save";
			saveButton.Enabled = false;
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.Location = new System.Drawing.Point(313, 144);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(75, 23);
			cancelButton.TabIndex = 7;
			cancelButton.Text = "&Cancel";
			lineLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			lineLabel.Appearance.ForeColor = System.Drawing.Color.Transparent;
			lineLabel.Appearance.Options.UseForeColor = true;
			lineLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			lineLabel.LineVisible = true;
			lineLabel.Location = new System.Drawing.Point(0, 128);
			lineLabel.Name = "lineLabel";
			lineLabel.Size = new System.Drawing.Size(400, 5);
			lineLabel.TabIndex = 45;
			form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			form.ClientSize = new System.Drawing.Size(400, 179);
			form.Controls.Add(lineLabel);
			form.Controls.Add(cancelButton);
			form.Controls.Add(saveButton);
			form.Controls.Add(filePathLabel);
			form.Controls.Add(languageLabel);
			form.Controls.Add(filePathBrowseButton);
			form.Controls.Add(filePathText);
			form.Controls.Add(languageBox);
			form.AcceptButton = saveButton;
			form.CancelButton = cancelButton;
			form.Name = "resultSaveForm";
			form.Text = "Save persistent classes to a file";
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
		}
		void filePathText_EditValueChanged(object sender, EventArgs e) {
			saveButton.Enabled = !string.IsNullOrEmpty(filePathText.Text);
		}
		void filePathBrowseButton_Click(object sender, EventArgs e) {
			SaveFileDialog saveFile = new SaveFileDialog();
			switch(Language) {
				case Language.CSharp:
					saveFile.Filter = "C# files (*.cs)|*.cs";
					break;
				case Language.VisualBasic:
					saveFile.Filter = "Visual Basic files (*.vb)|*.vb";
					break;
				default:
					break;
			}
			if(saveFile.ShowDialog(form) == DialogResult.OK) {
				filePathText.Text = saveFile.FileName;
			}
		}
	}
	public class AddClassesFromDbWizardForCSharp : AddClassesFromDbWizard {
		public AddClassesFromDbWizardForCSharp()
			: base() {
			Language = Language.CSharp;
		}
	}
	public class AddClassesFromDbWizardForVisualBasic : AddClassesFromDbWizard {
		public AddClassesFromDbWizardForVisualBasic()
			: base() {
			Language = Language.VisualBasic;
		}
	}
	internal class PCGWizardPage : WizardPage {
		AddClassesFromDbForm owner;
		protected AddClassesFromDbForm Owner { get { return this.owner; } }
		public virtual string Description { get { return string.Empty; } }
		public PCGWizardPage(AddClassesFromDbForm owner)
			: base() {
			this.owner = owner;
		}
	}
	internal class ConnectionPage : PCGWizardPage, IConnectionPage {
		const string DefaultDatabaseName = "Database";
		RadioGroup serverType;
		LabelControl serverTypeLabel;
		RadioGroup authType;
		LabelControl authTypeLabel;
		ComboBoxEdit databaseList;
		LabelControl databaseListLabel;
		ProviderFactory[] factories;
		ButtonEdit fileName;
		LabelControl fileNameLabel;
		PanelControl panel;
		TextEdit password;
		LabelControl passwordLabel;
		ComboBoxEdit providersList;
		LabelControl providersListLabel;
		LabelControl customConStrLabel;
		TextEdit customConStr;
		TextEdit serverName;
		LabelControl serverNameLabel;
		TextEdit userName;
		CheckEdit ceStoredProcedures;
		CheckEdit ceConnectionHelper;
		LabelControl userNameLabel;
		int maxPanelHeight;
		protected int MaxPanelHeight { get { return maxPanelHeight; } }
		OpenFileDialog openDialog;
		OpenFileDialog OpenDialog {
			get {
				if(openDialog == null) {
					openDialog = new OpenFileDialog();
					openDialog.InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
				}
				return openDialog;
			}
		}
		public bool GenerateStoredProcedures {
			get {
				if(ceStoredProcedures == null) { return false; }
				return ceStoredProcedures.Checked;
			}
		}
		public bool GenerateConnectionHelper {
			get {
				if(ceConnectionHelper == null) { return false; }
				return ceConnectionHelper.Checked;
			}
		}
		public override string Description {
			get { return "Specify connection settings to the target database."; }
		}
		const string customConStrTag = "<Custom connection string>";
		public ConnectionPage(AddClassesFromDbForm owner)
			: base(owner) {
			panel = new PanelControl();
			panel.Parent = this;
			panel.AutoSize = true;
			panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel.Text = "";
			panel.BorderStyle = BorderStyles.NoBorder;
			providersListLabel = new LabelControl();
			providersListLabel.Parent = this.panel;
			providersListLabel.Location = new Point(0, 4);
			providersListLabel.Text = "&Provider:";
			providersList = new ComboBoxEdit();
			providersList.Parent = this.panel;
			providersList.Location = new Point(100, 0);
			providersList.Width = 316;
			providersList.SelectedIndexChanged += new EventHandler(this.providersList_SelectedIndexChanged);
			providersList.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			providersList.Properties.Sorted = true;
			factories = DataStoreBase.Factories;
			for(int i = 0; i < this.factories.Length; i++) {
				if(!this.factories[i].MeanSchemaGeneration)
					continue;
				providersList.Properties.Items.Add(this.factories[i].ProviderKey);
			}
			providersList.Properties.Items.Add(customConStrTag);
			serverTypeLabel = new LabelControl();
			serverTypeLabel.Parent = this.panel;
			serverTypeLabel.Text = "Server &Type:";
			serverType = new RadioGroup();
			serverType.Parent = this.panel;
			serverType.Width = this.providersList.Width;
			serverType.Properties.Items.Add(new RadioGroupItem(0, "Server"));
			serverType.Properties.Items.Add(new RadioGroupItem(1, "Embedded"));
			serverType.Properties.AutoHeight = false;
			serverType.Height = 50;
			serverType.SelectedIndex = 0;
			serverType.SelectedIndexChanged += new EventHandler(serverType_SelectedIndexChanged);
			serverNameLabel = new LabelControl();
			serverNameLabel.Parent = this.panel;
			serverNameLabel.Text = "&Server Name:";
			serverName = new TextEdit();
			serverName.Parent = this.panel;
			serverName.Width = this.providersList.Width;
			customConStrLabel = new LabelControl();
			customConStrLabel.Parent = this.panel;
			customConStrLabel.Text = "&Connection String:";
			customConStr = new TextEdit();
			customConStr.Parent = this.panel;
			customConStr.Width = this.providersList.Width;
			fileNameLabel = new LabelControl();
			fileNameLabel.Parent = this.panel;
			fileNameLabel.Text = "&Database:";
			fileName = new ButtonEdit();
			fileName.Parent = this.panel;
			fileName.Width = this.providersList.Width;
			fileName.ButtonClick += new ButtonPressedEventHandler(this.fileName_ButtonClick);
			authTypeLabel = new LabelControl();
			authTypeLabel.Parent = this.panel;
			authTypeLabel.Text = "&Authentication:";
			authType = new RadioGroup();
			authType.Parent = this.panel;
			authType.Width = this.providersList.Width;
			authType.Properties.Items.Clear();
			authType.Properties.AutoHeight = false;
			authType.Height = 50;
			authType.SelectedIndexChanged += new EventHandler(this.authType_SelectedIndexChanged);
			authType.SelectedIndex = 1;
			userNameLabel = new LabelControl();
			userNameLabel.Parent = this.panel;
			userNameLabel.Text = "User Na&me:";
			userName = new TextEdit();
			userName.Parent = this.panel;
			userName.Width = this.providersList.Width;
			passwordLabel = new LabelControl();
			passwordLabel.Parent = this.panel;
			passwordLabel.Text = "Pass&word:";
			password = new TextEdit();
			password.Parent = this.panel;
			password.Width = this.providersList.Width;
			password.Properties.PasswordChar = '*';
			databaseListLabel = new LabelControl();
			databaseListLabel.Parent = this.panel;
			databaseListLabel.Text = "&Database:";
			databaseList = new ComboBoxEdit();
			databaseList.Parent = this.panel;
			databaseList.Width = this.providersList.Width;
			databaseList.QueryPopUp += new CancelEventHandler(this.databaseList_QueryPopUp);
			databaseList.Properties.Sorted = true;
			ceStoredProcedures = new CheckEdit();
			ceStoredProcedures.Parent = this.panel;
			ceStoredProcedures.Text = "&Generate views and stored procedures for table access";
			ceStoredProcedures.Width = this.providersList.Width;
			ceConnectionHelper = new CheckEdit();
			ceConnectionHelper.Parent = this.panel;
			ceConnectionHelper.Text = "Generate connection &helper";
			ceConnectionHelper.Width = this.providersList.Width;
			Factory = new MSSqlProviderFactory();
			SetControls();
			maxPanelHeight = this.panel.Height;
			SetControls();
			ConnectionParameter.TryToLoadSettings(this);
		}
		protected bool IsServerbased {
			get {
				return Factory != null && Factory.IsServerbased && (!serverType.Visible || (serverType.Visible && serverType.SelectedIndex == 0));
			}
		}
		protected bool IsFilebased {
			get {
				return Factory != null && Factory.IsFilebased && (!serverType.Visible || (serverType.Visible && serverType.SelectedIndex == 1));
			}
		}
		public string DatabaseName {
			get {
				string databaseName = DatabaseNameCore;
				if(databaseName.Contains("\\") || databaseName.Contains("/") || databaseName.Contains("."))
					databaseName = GenerateDBName(Path.GetFileNameWithoutExtension(databaseName));
				return databaseName;
			}
		}
		protected string DatabaseNameCore {
			get {
				if(Factory != null) {
					if(IsServerbased) {
						if(Factory.HasMultipleDatabases)
							return databaseList.Text;
						else
							return serverName.Text;
					}
					if(IsFilebased)
						return fileName.Text;
				}
				return DefaultDatabaseName;
			}
		}
		internal static string GenerateDBName(string fileName) {
			if(!String.IsNullOrEmpty(fileName)) {
				char[] buf = new char[fileName.Length + 1];
				int c = 0;
				for(int i = 0; i < fileName.Length; i++)
					if((fileName[i] >= 'A' && fileName[i] <= 'Z') ||
						(fileName[i] >= 'a' && fileName[i] <= 'z') ||
						(fileName[i] >= '0' && fileName[i] <= '9')) {
						buf[c] = fileName[i];
						c++;
					}
				string result = new string(buf, 0, c);
				if(!string.IsNullOrEmpty(result))
					return result[0] >= '0' && result[0] <= '9' ? "Db" + result : result;
			}
			return string.Empty;
		}
		void serverType_SelectedIndexChanged(object sender, EventArgs e) {
			SetControls();
		}
		void authType_SelectedIndexChanged(object sender, EventArgs e) {
			SetUserNameEnabled();
		}
		void CenterPanel() {
			if(panel == null)
				return;
			panel.Location = new Point((base.Width - this.panel.Width) / 2, (base.Height - MaxPanelHeight) / 2);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			CenterPanel();
		}
		void databaseList_QueryPopUp(object sender, CancelEventArgs e) {
			this.databaseList.Properties.Items.Clear();
			if(this.Factory != null) {
				string server = this.serverName.Text;
				try {
					if(this.userName.Visible && this.userName.Enabled) {
						string[] databases = this.Factory.GetDatabases(server, this.userName.Text, this.password.Text);
						Array.Sort<string>(databases);
						databaseList.Properties.Items.AddRange(databases);
					} else {
						string[] databases = this.Factory.GetDatabases(server, "", "");
						Array.Sort<string>(databases);
						databaseList.Properties.Items.AddRange(databases);
					}
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(!disposing) {
				providersList.SelectedIndexChanged -= new EventHandler(this.providersList_SelectedIndexChanged);
				databaseList.QueryPopUp -= new CancelEventHandler(this.databaseList_QueryPopUp);
				authType.SelectedIndexChanged -= new EventHandler(this.authType_SelectedIndexChanged);
				serverType.SelectedIndexChanged -= new EventHandler(this.serverType_SelectedIndexChanged);
			}
			base.Dispose(disposing);
		}
		void fileName_ButtonClick(object sender, ButtonPressedEventArgs e) {
			OpenDialog.Filter = Factory.FileFilter;
			if(OpenDialog.ShowDialog() == DialogResult.OK)
				fileName.Text = OpenDialog.FileName;
			OpenDialog.Dispose();
		}
		void HideAllControls() {
			serverTypeLabel.Visible = false;
			serverType.Visible = false;
			serverNameLabel.Visible = false;
			serverName.Visible = false;
			fileNameLabel.Visible = false;
			fileName.Visible = false;
			databaseListLabel.Visible = false;
			databaseList.Visible = false;
			userName.Visible = false;
			userNameLabel.Visible = false;
			password.Visible = false;
			passwordLabel.Visible = false;
			ceStoredProcedures.Visible = false;
			ceConnectionHelper.Visible = false;
			authType.Visible = false;
			authType.Properties.Items.Clear();
			authTypeLabel.Visible = false;
			customConStrLabel.Visible = false;
			customConStr.Visible = false;
			SetUserNameEnabled();
			for(int i = 0; i < this.panel.Controls.Count; i++) {
				if(this.panel.Controls[i] is LabelControl) {
					panel.Controls[i].Top = 3;
				} else {
					panel.Controls[i].Top = 0;
				}
			}
		}
		protected override bool OnSetActive() {
			this.Owner.WizardButtons = WizardButton.Cancel | WizardButton.Next;
			return base.OnSetActive();
		}
		protected override string OnWizardNext() {
			if (IsFilebased) {
				if(string.IsNullOrEmpty(DatabaseName)) {
					XtraMessageBox.Show("Please select the database.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return WizardForm.NoPageChange;
				}
				if (!File.Exists(fileName.Text)) {
					XtraMessageBox.Show("The specified file is unavailable.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return WizardForm.NoPageChange;
				}
			}
			try {
				IDisposable[] objectsToDispose;
				if(this.Factory == null) {
					Owner.ConnectionString = GenerateConnectionHelper ? customConStr.Text : "";
					Owner.ConnectionProvider = (IDataStoreSchemaExplorerSp)XpoDefault.GetConnectionProvider(customConStr.Text, AutoCreateOption.SchemaAlreadyExists, out objectsToDispose);
				} else {
					Dictionary<string, string> parameters = ConnectionParameter.GetParamsDict(this);
					this.Owner.ConnectionString = GenerateConnectionHelper ? Factory.GetConnectionString(parameters) : "";
					this.Owner.ConnectionProvider = (IDataStoreSchemaExplorerSp)Factory.CreateProvider(parameters,
						AutoCreateOption.SchemaAlreadyExists, out objectsToDispose);
				}
				this.Owner.ObjectsToDisposeOnDisconect = objectsToDispose;
			}
			catch(Exception ex) {
				XtraMessageBox.Show("Unable to connect the database:\n" + ex.Message, "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return WizardForm.NoPageChange;
			}
			ConnectionParameter.SaveSettings(this);
			return WizardForm.NextPage;
		}
		void providersList_SelectedIndexChanged(object sender, EventArgs e) {
			this.SetControls();
		}
		string lastConStr;
		void SetControls() {
			try {
				HideAllControls();
				int indentLabel = 3;
				int indent = 3;
				int extraIndent = 8;
				int currentTop = providersList.Bounds.Bottom + indent;
				if(Factory != null) {
					if(Factory.IsServerbased && Factory.IsFilebased) {
						serverTypeLabel.Location = new Point(providersListLabel.Left, currentTop + indentLabel);
						serverTypeLabel.Visible = true;
						serverType.Location = new Point(providersList.Left, currentTop);
						serverType.Visible = true;
						currentTop = serverType.Bounds.Bottom + indent;
					}
					if((!serverType.Visible && Factory.IsServerbased) || (serverType.Visible && serverType.SelectedIndex == 0)) {
						serverNameLabel.Location = new Point(providersListLabel.Left, currentTop + indentLabel);
						serverNameLabel.Visible = true;
						serverName.Location = new Point(providersList.Left, currentTop);
						serverName.Visible = true;
						currentTop = serverName.Bounds.Bottom + indent;
					}
					if((!serverType.Visible && Factory.IsFilebased) || (serverType.Visible && serverType.SelectedIndex == 1)) {
						fileNameLabel.Location = new Point(providersListLabel.Left, currentTop + indentLabel);
						fileNameLabel.Visible = true;
						fileName.Location = new Point(providersList.Left, currentTop);
						fileName.Visible = true;
						currentTop = fileName.Bounds.Bottom + indent;
					}
					if(Factory.HasIntegratedSecurity && (Factory.HasUserName || Factory.HasPassword)) {
						authTypeLabel.Location = new Point(providersListLabel.Left, currentTop + indentLabel);
						authTypeLabel.Visible = true;
						authType.Location = new Point(providersList.Left, currentTop);
						authType.Visible = true;
						authType.Properties.Items.Add(new RadioGroupItem(0, "Windows authentication"));
						authType.Properties.Items.Add(new RadioGroupItem(1, "Server authentication"));
						authType.SelectedIndex = 0;
						SetUserNameEnabled();
						currentTop = authType.Bounds.Bottom + indent;
					}
					if(Factory.HasUserName) {
						currentTop += extraIndent;
						userNameLabel.Location = new Point(providersListLabel.Left, currentTop + indentLabel);
						userNameLabel.Visible = true;
						userName.Location = new Point(providersList.Left, currentTop);
						userName.Visible = true;
						currentTop = userName.Bounds.Bottom + indent;
					}
					if(Factory.HasPassword) {
						currentTop += Factory.HasUserName ? 0 : extraIndent;
						passwordLabel.Location = new Point(providersListLabel.Left, currentTop + indentLabel);
						passwordLabel.Visible = true;
						password.Location = new Point(providersList.Left, currentTop);
						password.Visible = true;
						currentTop = password.Bounds.Bottom + indent;
					}
					if(Factory.HasMultipleDatabases && (!serverType.Visible || serverType.SelectedIndex == 0)) {
						currentTop += extraIndent;
						databaseListLabel.Location = new Point(providersListLabel.Left, currentTop + indentLabel);
						databaseListLabel.Visible = true;
						databaseList.Location = new Point(providersList.Left, currentTop);
						databaseList.Visible = true;
						currentTop = databaseList.Bounds.Bottom + indent;
					}
					ceConnectionHelper.Location = new Point(providersList.Left - 1, currentTop);
					ceConnectionHelper.Visible = true;
					currentTop = ceConnectionHelper.Bounds.Bottom + indent;
					if(Factory.SupportStoredProcedures) {
						ceStoredProcedures.Location = new Point(providersList.Left - 1, currentTop);
						ceStoredProcedures.Visible = true;
						currentTop = ceStoredProcedures.Bounds.Bottom + indent;
					}
					lastConStr = Factory.GetConnectionString(ConnectionParameter.GetParamsDict(this));
				}
				if(providersList.Text == customConStrTag) {
					currentTop += extraIndent;
					customConStrLabel.Location = new Point(providersListLabel.Left, currentTop + indentLabel);
					customConStrLabel.Visible = true;
					customConStr.Location = new Point(providersList.Left, currentTop);
					customConStr.Visible = true;
					SetCustomConStrText();
					currentTop = customConStrLabel.Bounds.Bottom + indent;
					ceConnectionHelper.Location = new Point(providersList.Left - 1, currentTop);
					ceConnectionHelper.Visible = true;
					currentTop = ceConnectionHelper.Bounds.Bottom + indent;
					ceStoredProcedures.Location = new Point(providersList.Left, currentTop);
					ceStoredProcedures.Visible = true;
					currentTop = ceStoredProcedures.Bounds.Bottom + indent;
				}
				CenterPanel();
			}
			catch(Exception ex) {
				XtraMessageBox.Show("Form layout error:\n" + ex.Message, "Form Layout Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		void SetCustomConStrText() {
			customConStr.Text = lastConStr;
		}
		void SetUserNameEnabled() {
			this.userName.Enabled = (this.authType.SelectedIndex == 1) || (this.authType.Properties.Items.Count == 0);
			this.password.Enabled = (this.authType.SelectedIndex == 1) || (this.authType.Properties.Items.Count == 0);
		}
		internal ProviderFactory Factory {
			get {
				for(int i = 0; i < this.factories.Length; i++) {
					if(this.factories[i].ProviderKey == (string)this.providersList.SelectedItem)
						return this.factories[i];
				}
				return null;
			}
			set {
				string providerKey = value.ProviderKey;
				SetProvider(providerKey);
			}
		}
		void SetProvider(string providerKey) {
			for(int i = 0; i < this.providersList.Properties.Items.Count; i++) {
				if((string)this.providersList.Properties.Items[i] == providerKey) {
					this.providersList.SelectedIndex = i;
					return;
				}
			}
		}
		#region IConnectionPage Members
		bool IConnectionPage.IsServerbased { get { return this.IsServerbased; } }
		ProviderFactory IConnectionPage.Factory { get { return this.Factory; } }
		string IConnectionPage.FileName { 
			get { return this.fileName.Text; }
			set { this.fileName.Text = value; }
		}
		string IConnectionPage.UserName {
			get { return this.userName.Text; }
			set { this.userName.Text = value; }
		}
		string IConnectionPage.ServerName {
			get { return this.serverName.Text; }
			set { this.serverName.Text = value; }
		}
		string IConnectionPage.DatabaseName {
			get { return this.databaseList.Text; }
			set { this.databaseList.Text = value; }
		}
		bool IConnectionPage.AuthType {
			get { return (this.authType.SelectedIndex == 0) ? true : false; }
			set { this.authType.SelectedIndex = value?0:1; }
		}
		string IConnectionPage.Password { 
			get { return this.password.Text; }
			set { this.password.Text = value; }
		}
		string IConnectionPage.CustomConStrTag { get { return customConStrTag; } }
		string IConnectionPage.CustomConStr { get { return this.customConStr.Text; } }
		void IConnectionPage.SetProvider(string providerKey) { this.SetProvider(providerKey); }
		bool IConnectionPage.CeConnectionHelper {
			get { return this.ceConnectionHelper.Checked; }
			set { this.ceConnectionHelper.Checked = value; }
		}
		string IConnectionPage.LastConStr {
			get { return this.lastConStr; }
			set { this.lastConStr = value; }
		}
		#endregion
	}
	internal class Column : IComparable, IComparable<Column> {
		private string name = null;
		private bool selected = true;
		private bool enabled = true;
		public Column(string name, bool selected) {
			this.name = name;
			this.selected = selected;
		}
		public Column(string name, bool selected, bool enabled)
			: this(name, selected && enabled) {
			this.enabled = enabled;
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public bool Selected {
			get { return selected && enabled; }
			set { selected = value; }
		}
		public bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
		public int CompareTo(object obj) {
			return CompareTo(obj as Column);
		}
		public int CompareTo(Column other) {
			if(other == null)
				return -1;
			return string.Compare(name, other.name);
		}
	}
	internal class Columns : Dictionary<string, Column> { }
	internal class Table {
		private DevExpress.Xpo.Design.Columns columns;
		private string name;
		public Table(string name, DevExpress.Xpo.Design.Columns columns) {
			this.name = name;
			this.columns = columns;
		}
		public DevExpress.Xpo.Design.Columns Columns {
			get {
				return this.columns;
			}
			set {
				this.columns = value;
			}
		}
		public string Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}
	}
	internal class Tables : Dictionary<string, Table> {
	}
	internal class TableColumnsCheckContainer {
		IDataStoreSchemaExplorer connectionProvider;
		Tables tables = new Tables();
		Dictionary<string, DBTable> tablesDict = new Dictionary<string, DBTable>();
		public Columns GetTableColumns(string tableName) {
			return this.tables[tableName].Columns;
		}
		public string[] GetTableList() {
			string[] result = new string[this.tables.Count];
			int i = 0;
			foreach(KeyValuePair<string, Table> table in this.tables) {
				result[i] = table.Key;
				i++;
			}
			return result;
		}
		public void SetTableColumns(string tableName, Columns columns) {
			this.tables[tableName].Columns = columns;
		}
		public void Clear() {
			this.tables.Clear();
		}
		public DBTable GetStorageTable(string tableName) {
			DBTable result;
			if(tablesDict.TryGetValue(tableName, out result)) {
				return result;
			}
			return new DBTable();
		}
		public void SetUp(IDataStoreSchemaExplorer connectionProvider) {
			this.connectionProvider = connectionProvider;
			this.tables.Clear();
			string[] tableList = this.ConnectionProvider.GetStorageTablesList(false);
			DBTable[] tables = this.ConnectionProvider.GetStorageTables(tableList);
			for(int i = 0; i < tableList.Length; i++) {
				Columns columns = new Columns();
				tablesDict[tableList[i]] = tables[i];
				foreach(DBColumn column in tables[i].Columns) {
					columns.Add(column.Name, new Column(column.Name, true, column.ColumnType != DBColumnType.Unknown));
				}
				this.tables.Add(tableList[i], new Table(tableList[i], columns));
			}
		}
		private IDataStoreSchemaExplorer ConnectionProvider {
			get {
				return this.connectionProvider;
			}
		}
	}
	internal class ResultSetColumn : Column, IComparable<ResultSetColumn> {
		DBColumnType columnType;
		public ResultSetColumn(string name, DBColumnType columnType, bool selected, bool enabled)
			: base(name, selected, enabled) {
			this.columnType = columnType;
		}
		public ResultSetColumn(DBNameTypePair nameTypePair, bool selected, bool enabled)
			: this(nameTypePair.Name, nameTypePair.Type, selected, enabled) {
		}
		public DBColumnType ColumnType {
			get { return columnType; }
			set { columnType = value; }
		}
		public int CompareTo(ResultSetColumn other) {
			return CompareTo(other as Column);
		}
	}
	internal class ResultSetColumns : List<ResultSetColumn> { }
	internal class StoredProcedure {
		private ResultSetColumns resultSetColumns = new ResultSetColumns();
		private string name;
		public StoredProcedure(string name, ResultSetColumns resultSetColumns) {
			this.name = name;
			this.resultSetColumns = resultSetColumns;
		}
		public ResultSetColumns ResultSetColumns {
			get {
				return this.resultSetColumns;
			}
			set {
				this.resultSetColumns = value;
			}
		}
		public string Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}
	}
	internal class StoredProcedures : Dictionary<string, StoredProcedure> { }
	internal class SProcsResultSetCheckContainer {
		IDataStoreSchemaExplorerSp connectionProvider;
		StoredProcedures storedProcedures = new StoredProcedures();
		DBStoredProcedure[] sprocList;
		public ResultSetColumns GetResultSetColumns(string sprocName) {
			return storedProcedures[sprocName].ResultSetColumns;
		}
		public string[] GetSProcList() {
			string[] result = new string[this.storedProcedures.Count];
			int i = 0;
			foreach(KeyValuePair<string, StoredProcedure> sproc in this.storedProcedures) {
				result[i] = sproc.Key;
				i++;
			}
			return result;
		}
		public void SetSprocResultSet(string sprocName, ResultSetColumns resultSetColumns) {
			this.storedProcedures[sprocName].ResultSetColumns = resultSetColumns;
		}
		string CreateColumnName(string type, int columnNumber) {
			return string.Format(CultureInfo.InvariantCulture, "{0}_column_{1}", type, columnNumber);
		}
		public void Clear() {
			this.storedProcedures.Clear();
		}
		public void SetUp(IDataStoreSchemaExplorerSp connectionProvider) {
			this.connectionProvider = connectionProvider;
			this.storedProcedures.Clear();
			sprocList = this.ConnectionProvider.GetStoredProcedures();
			for(int i = 0; i < sprocList.Length; i++) {
				ResultSetColumns columns = new ResultSetColumns();
				if(sprocList[i].ResultSets.Count == 1) {
					for(int columnCounter = 0; columnCounter < sprocList[i].ResultSets[0].Columns.Count; columnCounter++) {
						string columnName = sprocList[i].ResultSets[0].Columns[columnCounter].Name;
						if(string.IsNullOrEmpty(columnName)) {
							columnName = CreateColumnName(sprocList[i].ResultSets[0].Columns[columnCounter].Type.ToString(), columnCounter);
						}
						columns.Add(new ResultSetColumn(columnName, sprocList[i].ResultSets[0].Columns[columnCounter].Type, true, sprocList[i].ResultSets[0].Columns[columnCounter].Type != DBColumnType.Unknown));
					}
				}
				storedProcedures[sprocList[i].Name] = new StoredProcedure(sprocList[i].Name, columns);
			}
		}
		public DBStoredProcedure[] SprocList {
			get {
				return sprocList;
			}
		}
		private IDataStoreSchemaExplorerSp ConnectionProvider {
			get {
				return this.connectionProvider;
			}
		}
	}
	internal class StructurePage : PCGWizardPage {
		StructureSPPage structureSPPage;
		internal StructureSPPage StructureSPPage {
			get { return structureSPPage; }
			set { structureSPPage = value; }
		}
		TableColumnsCheckContainer checkContainer = new TableColumnsCheckContainer();
		RenameableCheckedListBoxControl columns;
		SplitContainerControl container;
		bool sortColumns = false;
		int lastTablesIndex;
		RenameableCheckedListBoxControl tables;
		MenuItem sortColumnsMenuItem;
		XtraFrameCheckCaption frame1, frame2;
		Language language;
		ContextMenu contextMenu;
		[Browsable(false)]
		internal Language Language { get { return language; } set { language = value; } }
		protected PersistentClassGenerator Generator {
			get {
				switch(Language) {
					case Language.CSharp:
						return new CSharpPersistentClassGenerator();
					case Language.VisualBasic:
						return new VBPersistentClassGenerator();
				}
				return null;
			}
		}
		protected TableColumnsCheckContainer CheckContainer { get { return checkContainer; } }
		public override string Description {
			get { return "Select tables and columns for which persistent classes and their properties will be generated."; }
		}
		public StructurePage(AddClassesFromDbForm owner, Language language)
			: base(owner) {
			this.language = language;
			container = new SplitContainerControl();
			container.Padding = new Padding(12, 0, 12, 1);
			container.Parent = this;
			container.Bounds = new Rectangle(0, 0, base.ClientSize.Width, base.ClientSize.Height);
			container.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			container.BorderStyle = BorderStyles.NoBorder;
			container.SplitterPosition = (base.ClientSize.Width / 5) * 3;
			frame1 = new XtraFrameCheckCaption();
			frame1.Parent = container.Panel1;
			frame1.Dock = DockStyle.Fill;
			frame1.lbCaption.Text = "Tables:";
			frame1.cbCaption.Text = "Tables:";
			frame2 = new XtraFrameCheckCaption();
			frame2.Parent = container.Panel2;
			frame2.Dock = DockStyle.Fill;
			frame2.lbCaption.Text = "Columns:";
			frame2.cbCaption.Text = "Columns:";
			frame1.cbCaption.CheckStateChanged += cb_checkedChanged;
			frame2.cbCaption.CheckStateChanged += cb_checkedChanged;
			contextMenu = new ContextMenu(new MenuItem[] { 
				new MenuItem("Select all", new EventHandler(selectAllITem_Click)),
				new MenuItem("Unselect all", new EventHandler(unselectAllITem_Click)),
				sortColumnsMenuItem = new MenuItem("Sort alphabetically", new EventHandler(sortColumns_Click))
			});
			contextMenu.Popup += new EventHandler(contextMenu_Popup);
			tables = new RenameableCheckedListBoxControl();
			tables.Parent = frame1.pnlMain;
			tables.Bounds = new Rectangle(0, 0, this.tables.Parent.ClientSize.Width, this.tables.Parent.ClientSize.Height);
			tables.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			tables.MouseClick += new MouseEventHandler(tables_MouseClick);
			tables.SelectedIndexChanged += new EventHandler(this.tables_SelectedIndexChanged);
			tables.ItemChecking += new ItemCheckingEventHandler(tables_ItemChecking);			
			tables.ContextMenu = contextMenu;
			tables.IncrementalSearch = true;
			columns = new RenameableCheckedListBoxControl();
			columns.Parent = frame2.pnlMain;
			columns.Bounds = new Rectangle(0, 0, this.columns.Parent.ClientSize.Width, this.columns.Parent.ClientSize.Height);
			columns.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			columns.MouseClick += new MouseEventHandler(columns_MouseClick);
			columns.ItemChecking += new ItemCheckingEventHandler(columns_ItemChecking);
			columns.ContextMenu = contextMenu;
			columns.IncrementalSearch = true;
		}
		void cb_checkedChanged(object sender, EventArgs e) {			
			CheckEdit source = (CheckEdit)sender;
			RenameableCheckedListBoxControl list = null;			
			if (source == frame1.cbCaption) list = tables;
			else list = columns;
			if (source.Checked == true) {
				SelectAllItems(list);
				return;
			}			
			UnSelectAllItems(list);
		}
		void contextMenu_Popup(object sender, EventArgs e) {
			if(contextMenu.SourceControl == tables) {
				sortColumnsMenuItem.Visible = false;
			} else {
				sortColumnsMenuItem.Visible = true;
			}
		}
		public void SetColumnsSorting() {
			sortColumns = true;
			sortColumnsMenuItem.Enabled = false;
			tables_SelectedIndexChanged(this, EventArgs.Empty);
		}
		public static void MyCheckItem(RenameableCheckedListBoxControl control, MouseEventArgs e) {
			for(int i = 0; i < control.Items.Count; i++) {
				Rectangle r = control.GetItemRectangle(i);
				r = new Rectangle(r.Location, new Size(r.Size.Height, r.Size.Height));
				if(r.Contains(e.Location)) {
					if(control.Items[i].Enabled) {
						control.Items[i].InvertCheckState();
					} else {
						control.Items[i].CheckState = CheckState.Unchecked;
					}
					break;
				}
			}			
		}
		void tables_MouseClick(object sender, MouseEventArgs e) {
			MyCheckItem(tables, e);			
		}
		void tables_ItemChecking(object sender, ItemCheckingEventArgs e) {
			e.Cancel = true;
		}
		void columns_MouseClick(object sender, MouseEventArgs e) {
			for(int i = 0; i < columns.Items.Count; i++) {
				Rectangle r = columns.GetItemRectangle(i);
				r = new Rectangle(r.Location, new Size(r.Size.Height, r.Size.Height));
				if(r.Contains(e.Location)) {
					if(columns.Items[i].Enabled) {
						DBTable dbRefTable = checkContainer.GetStorageTable(GetTableName());
						if(dbRefTable.PrimaryKey.Columns.Contains((string)columns.Items[i].Value))
							columns.Items[i].CheckState = CheckState.Checked;
						else
							columns.Items[i].InvertCheckState();
					} else {
						columns.Items[i].CheckState = CheckState.Unchecked;
					}
					break;
				}
			}			
		}
		void columns_ItemChecking(object sender, ItemCheckingEventArgs e) {
			e.Cancel = true;
		}
		protected override void OnResize(EventArgs e) {
			if(container != null) {
				container.SplitterPosition = (base.ClientSize.Width / 5) * 3;
			}
			base.OnResize(e);
		}
		void sortColumns_Click(object sender, EventArgs e) {
			SetColumnsSorting();
		}
		void unselectAllITem_Click(object sender, EventArgs e) {
			RenameableCheckedListBoxControl control = GetListBoxFromMenuItem(sender);
			UnSelectAllItems(control);
		}
		void selectAllITem_Click(object sender, EventArgs e) {
			RenameableCheckedListBoxControl control = GetListBoxFromMenuItem(sender);
			SelectAllItems(control);
		}
		void SelectAllItems(RenameableCheckedListBoxControl control) {
			foreach (CheckedListBoxItem item in control.Items) {
				if (!item.Enabled) continue;
				item.CheckState = CheckState.Checked;
			}
		}
		void UnSelectAllItems(RenameableCheckedListBoxControl control) {
			DBTable dbRefTable = null;
			if (control == columns) {
				dbRefTable = checkContainer.GetStorageTable(GetTableName());
			}
			foreach (CheckedListBoxItem item in control.Items) {
				if (!item.Enabled) continue;
				if (dbRefTable != null && dbRefTable.PrimaryKey.Columns.Contains((string)item.Value)) continue;
				item.CheckState = CheckState.Unchecked;
			}
		}
		RenameableCheckedListBoxControl GetListBoxFromMenuItem(object sender) {
			MenuItem item = (MenuItem)sender;
			ContextMenu parent = (ContextMenu)item.Parent;
			RenameableCheckedListBoxControl listBox = (RenameableCheckedListBoxControl)parent.SourceControl;
			return listBox;
		}
		string GetTableName() {
			return GetName((string)tables.Items[tables.SelectedIndex].Value);
		}
		protected override void Dispose(bool disposing) {
			if(!disposing) {
				this.tables.SelectedIndexChanged -= new EventHandler(this.tables_SelectedIndexChanged);
			}
			base.Dispose(disposing);
		}
		public static string GetCaption(string name) {
			int endIndex = name.IndexOf('<');
			if(endIndex < 0) {
				return name;
			}
			return name.Substring(0, endIndex - 1);
		}
		public static string GetName(string name) {
			int startIndex = name.IndexOf('<');
			int endIndex = name.IndexOf('>');
			if(startIndex < 0) {
				return name;
			}
			return name.Substring(startIndex + 1, (endIndex - startIndex) - 1);
		}
		protected override bool OnKillActive() {
			return base.OnKillActive();
		}
		IDataStoreSchemaExplorerSp connectionProviderOld = null;
		protected override bool OnSetActive() {
			try {
				if(Owner.ConnectionProvider == null)
					return false;
				if(CheckContainer.GetTableList().Length > 0 && connectionProviderOld == Owner.ConnectionProvider) {
					return true;
				}
				string[] tableList = null;
				if(!FormProgress.ShowModal<string[]>(this, "Retrieving tables...", true, false, (setMax, setPos) => {
					CheckContainer.SetUp(ConnectionProvider);
					string[] tbls = CheckContainer.GetTableList();
					Array.Sort<string>(tbls);
					return tbls;
				}, out tableList)) {
					CheckContainer.Clear();
					tableList = new string[0];
					return false;
				}
				Owner.WizardButtons = WizardButton.Cancel | WizardButton.Next | WizardButton.Back;
				lastTablesIndex = -1;
				tables.Items.Clear();
				tables.ClearHints();
				tables.BeginUpdate();
				for(int i = 0; i < tableList.Length; i++) {
					string message;
					bool enabled = PersistentClassGenerator.CheckTable(ConnectionProvider, tableList[i], out message);
					tables.Items.Add(tableList[i], enabled);
					tables.Items[i].Enabled = enabled;
					if(!string.IsNullOrEmpty(message))
						tables.SetHint(tables.Items[i], message);
				}
				tables.EndUpdate();				
				tables_SelectedIndexChanged(tables, new EventArgs());
			}
			catch(Exception ex) {
				XtraMessageBox.Show("Unable to read the database schema:\n" + ex.Message, "Schema Read Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			return true;
		}
		private void SaveColumnsCheckedState(int lastTablesIndex) {
			Columns lastColumnList = new Columns();
			for(int i = 0; i < this.columns.Items.Count; i++) {
				lastColumnList.Add(GetName((string)this.columns.Items[i].Value), new Column(GetCaption((string)this.columns.Items[i].Value), this.columns.Items[i].CheckState == CheckState.Checked, this.columns.Items[i].Enabled));
			}
			this.checkContainer.SetTableColumns(GetName((string)this.tables.Items[lastTablesIndex].Value), lastColumnList);
		}
		protected override bool OnWizardFinish() {
			tables_SelectedIndexChanged(container, EventArgs.Empty);
			return base.OnWizardFinish();
		}
		protected override string OnWizardNext() {
			tables_SelectedIndexChanged(container, EventArgs.Empty);
			return base.OnWizardNext();
		}
		private void tables_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				columns.BeginUpdate();
				bool tableItemEnabled = false;
				if(this.tables.SelectedIndex >= 0) {
					if(this.lastTablesIndex >= 0) {
						this.SaveColumnsCheckedState(this.lastTablesIndex);
					}
					var tableItem = this.tables.Items[this.tables.SelectedIndex];
					tableItemEnabled = tableItem.Enabled;
					Columns columnList = this.checkContainer.GetTableColumns(GetName((string)tableItem.Value));
					List<string> columnNames = new List<string>(columnList.Keys);
					if(sortColumns) {
						columnNames.Sort();
					}
					this.columns.Items.Clear();
					foreach(string columnName in columnNames) {
						Column column = columnList[columnName];
						string name = (column.Name == columnName) ? columnName : (column.Name + " <" + columnName + ">");
						this.columns.Items.Add(name, column.Selected && tableItem.Enabled ? CheckState.Checked : CheckState.Unchecked, tableItemEnabled && column.Enabled);
					}
				} else {
					columns.Items.Clear();
				}
				this.lastTablesIndex = this.tables.SelectedIndex;
				columns.EnableRenaming = tableItemEnabled && columns.Items.Count > 0;
				columns.EndUpdate();
			}
			catch(Exception ex) {
				XtraMessageBox.Show("Columns fetch exception:\n" + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		void GenerateCode(out string code, out string log) {
			try {
				if(this.tables.SelectedIndex >= 0) {
					this.SaveColumnsCheckedState(this.tables.SelectedIndex);
				}
				StringWriter writer = new StringWriter();
				StringWriter logWriter = new StringWriter();
				Dictionary<string, bool> tableMask = GetTablesMask();
				if(structureSPPage == null) {
					Generator.Generate(writer, logWriter, this.ConnectionString, this.ConnectionProvider, tableMask, this.checkContainer, Owner.DatabaseName, Owner.ConnectionPage.GenerateStoredProcedures);
				} else {
					Dictionary<string, bool> sprocMasks = structureSPPage.GetSProcMask();
					Generator.Generate(writer, logWriter, this.ConnectionString, this.ConnectionProvider, tableMask, this.checkContainer, sprocMasks, structureSPPage.CheckContainer, Owner.DatabaseName, Owner.ConnectionPage.GenerateStoredProcedures);
				}
				code = writer.ToString();
				log = logWriter.ToString();
			}
			catch(Exception ex) {
				code = string.Empty;
				log = string.Empty;
				XtraMessageBox.Show("Code generation error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		string GetLog() {
			if(this.tables.SelectedIndex >= 0) {
				this.SaveColumnsCheckedState(this.tables.SelectedIndex);
			}
			StringWriter logWriter = new StringWriter();
			Dictionary<string, bool> tableMask = GetTablesMask();
			Generator.Check(logWriter, tableMask, this.checkContainer);
			return logWriter.ToString();
		}
		Dictionary<string, bool> GetTablesMask() {
			Dictionary<string, bool> tableMask = new Dictionary<string, bool>();
			for(int i = 0; i < this.tables.Items.Count; i++) {
				if(!tables.Items[i].Enabled)
					continue;
				string key = (string)this.tables.Items[i].Value;
				tableMask.Add(key, this.tables.Items[i].CheckState == CheckState.Checked);
			}
			return tableMask;
		}
		public string Code {
			get {
				string code, log;
				GenerateCode(out code, out log);
				return code;
			}
		}
		public string Log { get { return GetLog(); } }
		private IDataStoreSchemaExplorer ConnectionProvider {
			get {
				return this.Owner.ConnectionProvider;
			}
		}
		private string ConnectionString {
			get {
				return this.Owner.ConnectionString;
			}
		}
	}
	internal class StructureSPPage : PCGWizardPage {
		SProcsResultSetCheckContainer checkContainer = new SProcsResultSetCheckContainer();
		RenameableCheckedListBoxControl resultSet;
		SplitContainerControl container;
		int lastsprocIndex;
		RenameableCheckedListBoxControl storedProcedures;
		XtraFrameCheckCaption frame1, frame2;
		Language language;
		ContextMenu contextMenu;
		[Browsable(false)]
		internal Language Language { get { return language; } set { language = value; } }
		protected PersistentClassGenerator Generator {
			get {
				switch(Language) {
					case Language.CSharp:
						return new CSharpPersistentClassGenerator();
					case Language.VisualBasic:
						return new VBPersistentClassGenerator();
				}
				return null;
			}
		}
		internal SProcsResultSetCheckContainer CheckContainer { get { return checkContainer; } }
		internal Dictionary<string, bool> GetSProcMask() {
			Dictionary<string, bool> sprocMask = new Dictionary<string, bool>();
			for(int i = 0; i < this.storedProcedures.Items.Count; i++) {
				if(!this.storedProcedures.Items[i].Enabled)
					continue;
				string key = (string)this.storedProcedures.Items[i].Value;
				sprocMask.Add(key, this.storedProcedures.Items[i].CheckState == CheckState.Checked);
			}
			return sprocMask;
		}
		public override string Description {
			get { return "Select stored procedures and result set columns for which auxiliary non-persistent classes and their properties will be generated."; }
		}
		public StructureSPPage(AddClassesFromDbForm owner, Language language)
			: base(owner) {
			this.language = language;
			container = new SplitContainerControl();
			container.Padding = new Padding(12, 0, 12, 1);
			container.Parent = this;
			container.Bounds = new Rectangle(0, 0, base.ClientSize.Width, base.ClientSize.Height);
			container.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			container.BorderStyle = BorderStyles.NoBorder;
			container.SplitterPosition = (base.ClientSize.Width / 5) * 3;
			frame1 = new XtraFrameCheckCaption();
			frame1.Parent = container.Panel1;
			frame1.Dock = DockStyle.Fill;
			frame1.lbCaption.Text = "Stored Procedures:";
			frame1.cbCaption.Text = "Stored Procedures:";
			frame1.cbCaption.Checked = false;
			frame2 = new XtraFrameCheckCaption();
			frame2.Parent = container.Panel2;
			frame2.Dock = DockStyle.Fill;
			frame2.lbCaption.Text = "Result Set Columns:";
			frame1.cbCaption.Text = "Result Set Columns:";
			frame1.cbCaption.CheckStateChanged += cb_checkedChanged;
			frame2.cbCaption.CheckStateChanged += cb_checkedChanged;
			contextMenu = new ContextMenu(new MenuItem[] { 
				new MenuItem("Select all", new EventHandler(selectAllItem_Click)),
				new MenuItem("Unselect all", new EventHandler(unselectAllItem_Click))
			});
			storedProcedures = new RenameableCheckedListBoxControl();
			storedProcedures.Parent = frame1.pnlMain;
			storedProcedures.Bounds = new Rectangle(0, 0, this.storedProcedures.Parent.ClientSize.Width, this.storedProcedures.Parent.ClientSize.Height);
			storedProcedures.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			storedProcedures.MouseClick += new MouseEventHandler(storedProcedures_MouseClick);
			storedProcedures.SelectedIndexChanged += new EventHandler(this.storedProcedures_SelectedIndexChanged);
			storedProcedures.ItemChecking += new ItemCheckingEventHandler(storedProcedures_ItemChecking);
			storedProcedures.ContextMenu = contextMenu;
			storedProcedures.IncrementalSearch = true;
			resultSet = new RenameableCheckedListBoxControl();
			resultSet.Parent = frame2.pnlMain;
			resultSet.Bounds = new Rectangle(0, 0, this.resultSet.Parent.ClientSize.Width, this.resultSet.Parent.ClientSize.Height);
			resultSet.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			resultSet.MouseClick += new MouseEventHandler(resultSet_MouseClick);
			resultSet.ItemChecking += new ItemCheckingEventHandler(resultSet_ItemChecking);
			resultSet.ContextMenu = contextMenu;
			resultSet.IncrementalSearch = true;
		}
		void cb_checkedChanged(object sender, EventArgs e) {
			CheckEdit source = (CheckEdit)sender;
			RenameableCheckedListBoxControl list = null;
			if (source == frame1.cbCaption) list = storedProcedures;
			else list = resultSet;
			if (source.Checked == true) {
				SelectAllItems(list);
				return;
			}
			UnSelectAllItems(list);
		}
		void resultSet_MouseClick(object sender, MouseEventArgs e) {
			StructurePage.MyCheckItem(resultSet, e);
		}
		void resultSet_ItemChecking(object sender, ItemCheckingEventArgs e) {
			e.Cancel = true;
		}
		void storedProcedures_MouseClick(object sender, MouseEventArgs e) {
			StructurePage.MyCheckItem(storedProcedures, e);
		}
		void storedProcedures_ItemChecking(object sender, ItemCheckingEventArgs e) {
			e.Cancel = true;
		}
		protected override void OnResize(EventArgs e) {
			if(container != null) {
				container.SplitterPosition = (base.ClientSize.Width / 5) * 3;
			}
			base.OnResize(e);
		}
		void unselectAllItem_Click(object sender, EventArgs e) {
			RenameableCheckedListBoxControl control = GetListBoxFromMenuItem(sender);
			UnSelectAllItems(control);
		}
		void selectAllItem_Click(object sender, EventArgs e) {
			RenameableCheckedListBoxControl control = GetListBoxFromMenuItem(sender);
			SelectAllItems(control);			
		}
		void SelectAllItems(RenameableCheckedListBoxControl control) {
			foreach (CheckedListBoxItem item in control.Items) {
				if (!item.Enabled) continue;
				item.CheckState = CheckState.Checked;
			}
		}
		void UnSelectAllItems(RenameableCheckedListBoxControl control) {
			foreach (CheckedListBoxItem item in control.Items) {
				if (!item.Enabled) continue;
				item.CheckState = CheckState.Unchecked;
			}
		}
		RenameableCheckedListBoxControl GetListBoxFromMenuItem(object sender) {
			MenuItem item = (MenuItem)sender;
			ContextMenu parent = (ContextMenu)item.Parent;
			RenameableCheckedListBoxControl listBox = (RenameableCheckedListBoxControl)parent.SourceControl;
			return listBox;
		}
		string GetSPName() {
			return GetName((string)storedProcedures.Items[storedProcedures.SelectedIndex].Value);
		}
		protected override void Dispose(bool disposing) {
			if(!disposing) {
				this.storedProcedures.SelectedIndexChanged -= new EventHandler(this.storedProcedures_SelectedIndexChanged);
			}
			base.Dispose(disposing);
		}
		public static string GetName(string name) {
			int endIndex = name.IndexOf('<');
			if(endIndex < 0) {
				return name;
			}
			return name.Substring(0, endIndex - 1);
		}
		protected override bool OnKillActive() {
			storedProcedures_SelectedIndexChanged(container, new EventArgs());
			return base.OnKillActive();
		}
		IDataStoreSchemaExplorerSp connectionProviderOld = null;
		bool storeProceduresNotSupported = false;
		protected override bool OnSetActive() {
			try {
				if(Owner.ConnectionProvider == null)
					return false;
				if(storeProceduresNotSupported) {
					Owner.WizardButtons = WizardButton.Cancel | WizardButton.Finish | WizardButton.Back;
					this.Wizard.PressButton(WizardButton.Finish);
					return false;
				}
				if((CheckContainer.GetSProcList().Length > 0 && connectionProviderOld == ConnectionProvider)) {
					Owner.WizardButtons = WizardButton.Cancel | WizardButton.Finish | WizardButton.Back;
					return true;
				}
				connectionProviderOld = ConnectionProvider;
				string[] sprocList = null;
				try {
					if (!FormProgress.ShowModal<string[]>(this, "Retrieving stored procedures...", true, false, (setMax, setPos) => {
						CheckContainer.SetUp(ConnectionProvider);
						string[] splt = CheckContainer.GetSProcList();
						Array.Sort<string>(splt);
						return splt;
					}, out sprocList)) {
						CheckContainer.Clear();
						sprocList = new string[0];
						return false;
					}
				} catch (NotSupportedException) {
					Owner.WizardButtons = WizardButton.Cancel | WizardButton.Finish | WizardButton.Back;
					storeProceduresNotSupported = true;
					if (XtraMessageBox.Show(this, string.Format("The {0} does not support stored procedures.\nDo you want to generate persistent classes without stored procedures?", ConnectionProvider.GetType().Name), "Schema Read Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
						this.Wizard.PressButton(WizardButton.Finish);
					}
					return false;
				} catch (Exception iex) {
					Owner.WizardButtons = WizardButton.Cancel | WizardButton.Finish | WizardButton.Back;
					storeProceduresNotSupported = true;
					if (XtraMessageBox.Show(this, "Unable to read the database schema:\n" + iex.Message + "\n\nDo you want to generate persistent classes without stored procedures?", "Schema Read Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
						this.Wizard.PressButton(WizardButton.Finish);
					}
					return false;
				}
				Owner.WizardButtons = WizardButton.Cancel | WizardButton.Finish | WizardButton.Back;
				lastsprocIndex = -1;
				storedProcedures.Items.Clear();
				storedProcedures.ClearHints();
				storedProcedures.BeginUpdate();
				for(int i = 0; i < sprocList.Length; i++) {
					storedProcedures.Items.Add(sprocList[i]);
				}
				storedProcedures.EndUpdate();
				storedProcedures_SelectedIndexChanged(storedProcedures, new EventArgs());
			}
			catch(Exception ex) {
				XtraMessageBox.Show(this, "Unable to read the database schema:\n" + ex.Message, "Schema Read Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			return true;
		}
		private void SaveColumnsCheckedState(int lastTablesIndex) {
			ResultSetColumns lastColumnList = new ResultSetColumns();
			for(int i = 0; i < this.resultSet.Items.Count; i++) {
				lastColumnList.Add(new ResultSetColumn(((DBNameTypePairItem)this.resultSet.Items[i].Value).Name, ((DBNameTypePairItem)this.resultSet.Items[i].Value).Type, this.resultSet.Items[i].CheckState == CheckState.Checked, this.resultSet.Items[i].Enabled));
			}
			this.checkContainer.SetSprocResultSet((string)this.storedProcedures.Items[lastTablesIndex].Value, lastColumnList);
		}
		private void storedProcedures_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				bool spItemEnable = false;
				if(this.storedProcedures.SelectedIndex >= 0) {
					if(this.lastsprocIndex >= 0) {
						this.SaveColumnsCheckedState(this.lastsprocIndex);
					}
					var spItem = this.storedProcedures.Items[this.storedProcedures.SelectedIndex];
					spItemEnable = spItem.Enabled;
					ResultSetColumns resultSetColumns = this.checkContainer.GetResultSetColumns((string)spItem.Value);
					this.resultSet.Items.Clear();
					foreach(ResultSetColumn column in resultSetColumns) {
						this.resultSet.Items.Add(new DBNameTypePairItem(column.Name, column.ColumnType), column.Selected && spItem.Enabled ? CheckState.Checked : CheckState.Unchecked, spItemEnable && column.Enabled);
					}
					this.lastsprocIndex = this.storedProcedures.SelectedIndex;
				} else {
					resultSet.Items.Clear();
				}
				resultSet.EnableRenaming = spItemEnable && resultSet.Items.Count > 0;
			}
			catch(Exception ex) {
				XtraMessageBox.Show(this, "Columns fetch exception:\n" + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		protected override bool OnWizardFinish() {
			storedProcedures_SelectedIndexChanged(this, EventArgs.Empty);
			return base.OnWizardFinish();
		}
		private IDataStoreSchemaExplorerSp ConnectionProvider {
			get {
				return this.Owner.ConnectionProvider;
			}
		}
	}
	class DBNameTypePairItem : DBNameTypePair {
		public DBNameTypePairItem(string name, DBColumnType type) {
			this.Name = name;
			this.Type = type;
		}
		public override string ToString() {
			return string.Format("{0} <{1}>", Name, Type.ToString());
		}
	}
	internal class RenameableCheckedListBoxControl : CheckedListBoxControl {
		int oldSelectedIndex;
		bool suppressDialogKey;
		bool enableRenaming;
		Dictionary<CheckedListBoxItem, string> hints;
		public RenameableCheckedListBoxControl()
			: base() {
			oldSelectedIndex = SelectedIndex;
			enableRenaming = true;
			hints = new Dictionary<CheckedListBoxItem, string>();
			ShowToolTips = true;
		}
		public bool EnableRenaming { get { return enableRenaming; } set { enableRenaming = value; } }
		public bool EverythingChecked {
			get {
				for(int i = 0; i < Items.Count; i++) {
					if(!Items[i].Enabled)
						continue;
					if(!GetItemChecked(i))
						return false;
				}
				return true;
			}
		}		
		public void SetHint(CheckedListBoxItem item, string hint) {
			hints.Add(item, hint);
		}
		public void ClearHints() {
			hints.Clear();
		}
		public new void CheckAll() {
			for(int i = 0; i < Items.Count; i++) {
				if(!Items[i].Enabled)
					continue;
				SetItemChecked(i, true);
			}
		}
		public new void UnCheckAll() {
			for(int i = 0; i < Items.Count; i++)
				SetItemChecked(i, false);
		}
		protected override void SetSelectedIndexCore(int index) {
			base.SetSelectedIndexCore(index);
			oldSelectedIndex = SelectedIndex;
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			if(e.Button == MouseButtons.Left) {
				if(SelectedIndex < 0 || !GetItemRectangle(SelectedIndex).Contains(e.Location))
					return;
				if(oldSelectedIndex != SelectedIndex) {
					oldSelectedIndex = SelectedIndex;
					return;
				}
				Rectangle textRectangle = GetTextRectangle();
				if(!textRectangle.Contains(e.Location))
					return;
				CreateRenameEditor(textRectangle);
			}
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			int index = GetItemAtPoint(point);
			if(index >= 0 && hints.ContainsKey(Items[index])) {
				return new ToolTipControlInfo(Items[index], hints[Items[index]]);
			}
			return base.GetToolTipInfo(point);
		}
		protected int GetItemAtPoint(Point pt) {
			for(int i = 0; i < Items.Count; i++)
				if(GetItemRectangle(i).Contains(pt))
					return i;
			return -1;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			switch(e.KeyCode) {
				case Keys.F2:
					if(SelectedIndex >= 0)
						CreateRenameEditor(GetTextRectangle());
					break;
				case Keys.A:
					if(e.Control && !e.Alt && !e.Shift) {
						if(EverythingChecked)
							UnCheckAll();
						else
							CheckAll();
					}
					break;
			}
		}
		Rectangle GetTextRectangle() {
			Rectangle textRectangle = GetItemRectangle(SelectedIndex);
			textRectangle.X += 20;
			textRectangle.Width -= 20;
			return textRectangle;
		}
		void CreateRenameEditor(Rectangle textRectangle) {
			if(!EnableRenaming)
				return;
			TextEdit editor = new TextEdit();
			editor.Parent = this;
			editor.Tag = SelectedIndex;
			editor.Bounds = textRectangle;
			editor.EditValue = StructurePage.GetCaption(SelectedValue.ToString());
			editor.Focus();
			editor.LostFocus += new EventHandler(editor_LostFocus);
			editor.KeyDown += new KeyEventHandler(editor_KeyDown);
			editor.Disposed += new EventHandler(editor_Disposed);
			suppressDialogKey = true;
		}
		void editor_Disposed(object sender, EventArgs e) {
			suppressDialogKey = false;
		}
		void editor_KeyDown(object sender, KeyEventArgs e) {
			TextEdit editor = (TextEdit)sender;
			switch(e.KeyCode) {
				case Keys.Escape:
					editor.Dispose();
					break;
				case Keys.Enter:
					SetNewItemValue(editor);
					break;
			}
		}
		void SetNewItemValue(TextEdit editor) {
			string caption = (string)editor.EditValue,
				name = StructurePage.GetName(SelectedValue.ToString());
			if(SelectedValue is DBNameTypePairItem) {
				SetItemValue(new DBNameTypePairItem(caption, ((DBNameTypePairItem)SelectedValue).Type), (int)editor.Tag);
			} else {
				SetItemValue((caption != name) ? (caption + " <" + name + ">") : name, (int)editor.Tag);
			}
		}
		void editor_LostFocus(object sender, EventArgs e) {
			TextEdit editor = (TextEdit)sender;
			SetNewItemValue(editor);
			editor.Dispose();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(suppressDialogKey)
				return false;
			return base.ProcessDialogKey(keyData);
		}
	}
	internal class InputForm : XtraForm {
		private SimpleButton cancel;
		private LabelControl label = new LabelControl();
		private TextEdit name;
		private SimpleButton ok;
		public InputForm() {
			label.Parent = this;
			label.Location = new Point(0x10, 0x10);
			name = new TextEdit();
			name.Parent = this;
			name.Bounds = new Rectangle(this.label.Bounds.Left, this.label.Bounds.Bottom + 8, 200, this.name.Height);
			ok = new SimpleButton();
			ok.Parent = this;
			ok.Bounds = new Rectangle((this.name.Bounds.Right - 8) - 150, this.name.Bounds.Bottom + 0x10, 0x4b, 0x19);
			ok.Text = "OK";
			ok.DialogResult = DialogResult.OK;
			cancel = new SimpleButton();
			cancel.Parent = this;
			cancel.Bounds = new Rectangle(this.ok.Bounds.Right + 8, this.ok.Bounds.Top, this.ok.Width, this.ok.Height);
			cancel.Text = "Cancel";
			cancel.DialogResult = DialogResult.Cancel;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			AcceptButton = this.ok;
			CancelButton = this.cancel;
			Text = "Input Dialog";
			ClientSize = new Size(this.name.Bounds.Right + 0x10, this.ok.Bounds.Bottom + 0x10);
		}
		public static bool ShowDialog(string promt, ref string value) {
			InputForm dialog = new InputForm();
			dialog.label.Text = promt;
			dialog.name.Text = value;
			if((dialog.ShowDialog() == DialogResult.OK) && (value != dialog.name.Text)) {
				value = dialog.name.Text;
				dialog.Dispose();
				return true;
			}
			dialog.Dispose();
			return false;
		}
	}
	internal class XtraFrameCheckCaption : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		public CheckEdit cbCaption;
		public XtraFrameCheckCaption() {
			this.lbCaption.Visible = false;
			this.cbCaption = new CheckEdit();
			this.SuspendLayout();
			this.cbCaption.Left = 0;			
			this.cbCaption.Dock = System.Windows.Forms.DockStyle.Top;			
			this.cbCaption.Name = "cbCaption";
			this.cbCaption.Size = new System.Drawing.Size(400, 24);
			this.cbCaption.Checked = true;
			this.Controls.Add(cbCaption);
			this.ResumeLayout();			
		}
	}
}
