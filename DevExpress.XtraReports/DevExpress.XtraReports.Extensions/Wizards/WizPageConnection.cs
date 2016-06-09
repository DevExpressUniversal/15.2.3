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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using System.Xml;
using System.Data.Common;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Wizards;
using DevExpress.LookAndFeel.DesignService;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Design
{
	public class WizPageConnection : DevExpress.Utils.InteriorWizardPage
	{
		protected System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label lblConnection;
		protected DevExpress.XtraEditors.ComboBoxEdit cbConnections;
		private DevExpress.XtraEditors.SimpleButton btnNewConnection;
		private System.ComponentModel.IContainer components = null;
		protected NewStandardReportWizard fWizard;
		protected IDesignerHost designerHost;
		protected DevExpress.XtraEditors.SimpleButton btnConnectionStringExpander;
		protected System.Windows.Forms.Label lbConnectionStringCaption;
		protected DevExpress.XtraEditors.MemoEdit meConnectionString;
		Connections connections = new Connections();
		ConfigFileHelper fileHelper = new ConfigFileHelper("Connections.xml");
		public WizPageConnection(XRWizardRunnerBase runner) 
			: this() {
			fWizard = (NewStandardReportWizard)runner.Wizard;
			this.designerHost = fWizard.DesignerHost;
		}
		WizPageConnection(){
			InitializeComponent();
			headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizTopDataConnection.gif", typeof(LocalResFinder));
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageConnection));
			this.meConnectionString = new DevExpress.XtraEditors.MemoEdit();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblConnection = new System.Windows.Forms.Label();
			this.cbConnections = new DevExpress.XtraEditors.ComboBoxEdit();
			this.btnNewConnection = new DevExpress.XtraEditors.SimpleButton();
			this.btnConnectionStringExpander = new DevExpress.XtraEditors.SimpleButton();
			this.lbConnectionStringCaption = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.meConnectionString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbConnections.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			resources.ApplyResources(this.meConnectionString, "meConnectionString");
			this.meConnectionString.Name = "meConnectionString";
			this.meConnectionString.Properties.ReadOnly = true;
			this.meConnectionString.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			resources.ApplyResources(this.lblDescription, "lblDescription");
			this.lblDescription.Name = "lblDescription";
			resources.ApplyResources(this.lblConnection, "lblConnection");
			this.lblConnection.Name = "lblConnection";
			this.cbConnections.EnterMoveNextControl = true;
			resources.ApplyResources(this.cbConnections, "cbConnections");
			this.cbConnections.Name = "cbConnections";
			this.cbConnections.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbConnections.Properties.Buttons"))))});
			this.cbConnections.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbConnections.SelectedIndexChanged += new System.EventHandler(this.cbConnections_SelectedIndexChanged);
			resources.ApplyResources(this.btnNewConnection, "btnNewConnection");
			this.btnNewConnection.Name = "btnNewConnection";
			this.btnNewConnection.Click += new System.EventHandler(this.btnNewConnection_Click);
			this.btnConnectionStringExpander.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)), true);
			this.btnConnectionStringExpander.Appearance.Options.UseFont = true;
			resources.ApplyResources(this.btnConnectionStringExpander, "btnConnectionStringExpander");
			this.btnConnectionStringExpander.Name = "btnConnectionStringExpander";
			this.btnConnectionStringExpander.Click += new System.EventHandler(this.btnConnectionStringExpander_Click);
			resources.ApplyResources(this.lbConnectionStringCaption, "lbConnectionStringCaption");
			this.lbConnectionStringCaption.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.lbConnectionStringCaption.Name = "lbConnectionStringCaption";
			this.Controls.Add(this.meConnectionString);
			this.Controls.Add(this.lbConnectionStringCaption);
			this.Controls.Add(this.btnConnectionStringExpander);
			this.Controls.Add(this.btnNewConnection);
			this.Controls.Add(this.cbConnections);
			this.Controls.Add(this.lblConnection);
			this.Controls.Add(this.lblDescription);
			this.Name = "WizPageConnection";
			this.Controls.SetChildIndex(this.lblDescription, 0);
			this.Controls.SetChildIndex(this.lblConnection, 0);
			this.Controls.SetChildIndex(this.cbConnections, 0);
			this.Controls.SetChildIndex(this.btnNewConnection, 0);
			this.Controls.SetChildIndex(this.btnConnectionStringExpander, 0);
			this.Controls.SetChildIndex(this.lbConnectionStringCaption, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			this.Controls.SetChildIndex(this.meConnectionString, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.meConnectionString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbConnections.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected override bool OnSetActive() {
			FillConnections();
			return true;
		}
		protected override void UpdateWizardButtons() {
			if (!String.IsNullOrEmpty(cbConnections.Text))
				Wizard.WizardButtons = WizardButton.Back | WizardButton.DisabledFinish | WizardButton.Next;
			else
				Wizard.WizardButtons = WizardButton.Back | WizardButton.DisabledFinish;
		}
		void FillConnections() {
			string filePath = fileHelper.GetLoadFilePath();
			if(!string.IsNullOrEmpty(filePath)) {
				connections.Clear();
				cbConnections.Properties.Items.Clear();
				LoadConnectionStrings(filePath);
				foreach(Connection connection in connections) {
					cbConnections.Properties.Items.Add(connection.Name);
				}
				cbConnections.SelectedIndex = 0;
				if(connections.Count > 0) {
					meConnectionString.Text = connections[0].ConnectionString;
				}
			}
		}
		protected override string OnWizardNext() {
			try {
				using(DbConnection ignoredTestConnection = ConnectionStringHelper.CreateDBConnection(meConnectionString.Text)) {
					fWizard.ConnectionString = meConnectionString.Text;
					SaveConnectionStrings();
				}
				return DevExpress.Utils.WizardForm.NextPage;
			}
			catch(Exception e) {
				XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(fWizard.DesignerHost), e.Message, "XtraReports");
				return DevExpress.Utils.WizardForm.NoPageChange;
			}
		}
		void SaveConnectionStrings() {
			string filePath = fileHelper.GetSaveFilePath();
			if(string.IsNullOrEmpty(filePath))
				return;	
			XmlTextWriter xwriter = new XmlTextWriter(filePath, System.Text.Encoding.Default);
			xwriter.Formatting = Formatting.Indented;
			xwriter.WriteStartDocument(true);
			xwriter.WriteStartElement("Connections");
			foreach(Connection connection in connections) {
				xwriter.WriteStartElement("Connection");
				xwriter.WriteElementString("Name", connection.Name);
				xwriter.WriteElementString("ConnectionString", connection.ConnectionString);
				xwriter.WriteEndElement();
			}
			xwriter.WriteEndElement();
			xwriter.WriteEndDocument();
			xwriter.Close();
			fileHelper.DeletePreviousFile();
		}
		void LoadConnectionStrings(string fileName) {
			XmlDocument xmlDocument = new XmlDocument();
			try {
				xmlDocument.Load(fileName);
			}
			catch {}
			XmlNodeList nodes = xmlDocument.GetElementsByTagName("Connection");
			foreach(XmlNode node in nodes) {
				try {
					connections.Add(node["Name"].InnerText, node["ConnectionString"].InnerText);
				}
				catch {}
			}
		}
		protected virtual void btnNewConnection_Click(object sender, EventArgs e) {
			string connectionString = string.Empty;
			Dictionary<Control, bool> controls = new Dictionary<Control, bool>();
			foreach(Control control in Wizard.Controls) {
				controls[control] = control.Enabled;
				control.Enabled = false;
			}
			try {
				connectionString = Native.ConnectionStringHelper.GetConnectionString();
			} finally {
				foreach(Control control in Wizard.Controls) {
					control.Enabled = controls[control];
				}
			}
			if(!String.IsNullOrEmpty(connectionString)) {
				int index = connections.IndexOf(connectionString);
				if(index < 0) {
					string connectionName = Native.ConnectionStringHelper.GetConnectionName(connectionString);
					index = connections.Add(connectionName, connectionString);
					cbConnections.Properties.Items.Add(connectionName);
				}
				cbConnections.SelectedIndex = index;
				meConnectionString.Text = connectionString;
			}
			UpdateWizardButtons();
		}
		private void btnConnectionStringExpander_Click(object sender, System.EventArgs e) {
			meConnectionString.Visible = !meConnectionString.Visible;
			btnConnectionStringExpander.Text = meConnectionString.Visible ? "-" : "+";
		}
		private void cbConnections_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(cbConnections.SelectedIndex >= 0 && connections.Count > 0) {
				meConnectionString.Text = connections[cbConnections.SelectedIndex].ConnectionString;
			}
		}
	}
	public class Connection {
		string name;
		string connectionString;
		public Connection(string name, string connectionString) {
			this.name = name;
			this.connectionString = connectionString;
		}
		public string ConnectionString { get { return connectionString; } 
		}
		public string Name { get { return name; } 
		}
	}
	public class Connections : CollectionBase {
		public Connections() {
		}
		public int Add(Connection connection) {
			return InnerList.Add(connection);
		}
		public int Add(string name, string connectionString) {
			return Add(new Connection(name, connectionString));
		}
		public Connection this[int index] { get { return (Connection)InnerList[index]; } 
		}
		public bool Contains(string connectionString) {
			foreach(Connection conn in this) {
				if(conn.ConnectionString.Equals(connectionString))
					return true;
			}
			return false;
		}
		public int IndexOf(string connectionString) {
			for(int i=0; i < Count; i++) {
				if(this[i].ConnectionString.Equals(connectionString)) 
					return i;
			}
			return -1;
		}
	}
}
