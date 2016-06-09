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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design.Wizard;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design.Wizard {
	[System.ComponentModel.ToolboxItem(false)]
	partial class InheritedReportView : WizardViewBase, IInheritedReportPageView {
		List<TypeInfo> dataSourse = new List<TypeInfo>();
		IServiceProvider serviceProvider;
		#region IInheritedReportPageView Members
		public IServiceProvider ServiceProvider {
			get { return serviceProvider; }
			set {
				serviceProvider = value;
				UpdateLabels();
			}
		}
		string IWizardPageView.HeaderDescription {
			get { return "Select the ancestor report"; }
		}
		TypeInfo IInheritedReportPageView.SelectedItem {
			get {
				int index = listReports.GetNodeIndex(listReports.FocusedNode);
				return index >= 0 && index < dataSourse.Count ? dataSourse[index] : null;
			}
		}
		bool IInheritedReportPageView.CanSelectAssembly {
			get { return linkAssembly.Visible; }
			set {
				if(linkAssembly.Visible != value)
					linkAssembly.Visible = value; 
			}
		}
		void IInheritedReportPageView.AddItems(IList<TypeInfo> typeInfos) {
			if(typeInfos == null || typeInfos.Count == 0)
				return;
			if(dataSourse.Any<TypeInfo>(item => string.Equals(item.AssemblyLocation, typeInfos[0].AssemblyLocation, StringComparison.OrdinalIgnoreCase)))
				return;
			dataSourse.AddRange(typeInfos);
			listReports.DataSource = null;
			listReports.DataSource = dataSourse;
		}
		public event EventHandler SelectedItemChanged;
		#endregion
		public InheritedReportView() {
			InitializeComponent();
		}
		void UpdateLabels() {
			this.linkAssembly.Initialize(GetLookAndFeel());
			if(ServiceProvider != null) {
				IWizardService serv = ServiceProvider.GetService(typeof(IWizardService)) as IWizardService;
				if(serv != null) lbReportName.Text = "New report name: " + serv.ClassName;
			}
		}
		UserLookAndFeel GetLookAndFeel() {
			var lookAndFeelService = ServiceProvider != null ? ServiceProvider.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService : null;
			return lookAndFeelService != null ? lookAndFeelService.LookAndFeel : UserLookAndFeel.Default;
		}
		void listReports_MouseDoubleClick(object sender, MouseEventArgs e) {
			MoveForward();
		}
		void listReports_FocusedNodeChanged(object sender, XtraTreeList.FocusedNodeChangedEventArgs e) {
			if(SelectedItemChanged != null)
				SelectedItemChanged(this, EventArgs.Empty);
		}
		private void lbAssembly_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			 List<TypeInfo> dataSourse = listReports.DataSource as List<TypeInfo>;
			 if(dataSourse == null) dataSourse = new List<TypeInfo>();
			 if(dataSourse.Count > 0) {
				 string path = dataSourse[dataSourse.Count - 1].AssemblyLocation;
				 if(!string.IsNullOrEmpty(path))
					openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(path);
			 }
			if(openFileDialog.ShowDialog(FindForm()) != DialogResult.OK) 
				return;
			List<Type> types = ReportTypesProvider.GetTypeNamesFromAssembly(openFileDialog.FileName, typeof(XtraReport));
			List<TypeInfo> typeInfos = types.ConvertAll<TypeInfo>(new Converter<Type, TypeInfo>(delegate(Type item) {
				return new TypeInfo(item.FullName) { Assembly = item.Assembly, AssemblyLocation = item.Assembly.Location };
			}));
			if(dataSourse.Any<TypeInfo>(item => string.Equals(item.AssemblyLocation, typeInfos[0].AssemblyLocation, StringComparison.OrdinalIgnoreCase))) {
				XtraMessageBox.Show(GetLookAndFeel(), FindForm(), "This assembly was already added.", ReportStringId.UD_ReportDesigner.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			((IInheritedReportPageView)this).AddItems(typeInfos);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.lbReportName = new DevExpress.XtraEditors.LabelControl();
			this.linkAssembly = new DevExpress.XtraPrinting.Native.LinkLabelControl();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.listReports = new DevExpress.XtraTreeList.TreeList();
			this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.colNameSpace = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.colAssemblyLocation = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			((System.ComponentModel.ISupportInitialize)(this.listReports)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
			this.SuspendLayout();
			this.lbReportName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lbReportName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.lbReportName.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lbReportName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lbReportName.Location = new System.Drawing.Point(0, 363);
			this.lbReportName.Name = "lbReportName";
			this.lbReportName.Size = new System.Drawing.Size(578, 18);
			this.lbReportName.TabIndex = 2;
			this.lbReportName.Text = "New report name: ";
			this.linkAssembly.Dock = System.Windows.Forms.DockStyle.Top;
			this.linkAssembly.Location = new System.Drawing.Point(0, 0);
			this.linkAssembly.Name = "linkAssembly";
			this.linkAssembly.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this.linkAssembly.Size = new System.Drawing.Size(578, 19);
			this.linkAssembly.TabIndex = 0;
			this.linkAssembly.TabStop = true;
			this.linkAssembly.Text = "&Select the assembly that contains ancestor reports";
			this.linkAssembly.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbAssembly_LinkClicked);
			this.openFileDialog.Filter = "Assembly files (*.dll;*.exe)|*.dll;*.exe|All files (*.*)|*.*";
			this.openFileDialog.Title = "Open Assembly";
			this.listReports.Appearance.FocusedRow.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.listReports.Appearance.HideSelectionRow.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.listReports.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.listReports.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.colName,
			this.colNameSpace,
			this.colAssemblyLocation});
			this.listReports.DataSource = this.bindingSource1;
			this.listReports.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listReports.Location = new System.Drawing.Point(0, 19);
			this.listReports.Name = "listReports";
			this.listReports.OptionsBehavior.AllowExpandOnDblClick = false;
			this.listReports.OptionsView.ShowButtons = false;
			this.listReports.OptionsView.ShowIndicator = false;
			this.listReports.OptionsView.ShowRoot = false;
			this.listReports.Size = new System.Drawing.Size(578, 344);
			this.listReports.TabIndex = 3;
			this.listReports.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.listReports_FocusedNodeChanged);
			this.listReports.MouseDoubleClick += new MouseEventHandler(this.listReports_MouseDoubleClick);
			this.colName.Caption = "Report Name";
			this.colName.FieldName = "Name";
			this.colName.Name = "colName";
			this.colName.OptionsColumn.AllowEdit = false;
			this.colName.OptionsColumn.AllowFocus = false;
			this.colName.Visible = true;
			this.colName.VisibleIndex = 0;
			this.colName.Width = 20;
			this.colNameSpace.Caption = "Namespace";
			this.colNameSpace.FieldName = "NameSpace";
			this.colNameSpace.Name = "colNameSpace";
			this.colNameSpace.OptionsColumn.AllowEdit = false;
			this.colNameSpace.OptionsColumn.AllowFocus = false;
			this.colNameSpace.Visible = true;
			this.colNameSpace.VisibleIndex = 1;
			this.colNameSpace.Width = 20;
			this.colAssemblyLocation.Caption = "Location";
			this.colAssemblyLocation.FieldName = "AssemblyLocation";
			this.colAssemblyLocation.Name = "colAssemblyLocation";
			this.colAssemblyLocation.OptionsColumn.AllowEdit = false;
			this.colAssemblyLocation.OptionsColumn.AllowFocus = false;
			this.colAssemblyLocation.Visible = true;
			this.colAssemblyLocation.VisibleIndex = 2;
			this.colAssemblyLocation.Width = 120;
			this.bindingSource1.DataSource = typeof(DevExpress.XtraReports.Design.Wizard.TypeInfo);
			this.panelBaseContent.Controls.Add(this.listReports);
			this.panelBaseContent.Controls.Add(this.lbReportName);
			this.panelBaseContent.Controls.Add(this.linkAssembly);
			this.Name = "WizPageInheritedReport";
			this.Size = new System.Drawing.Size(578, 381);
			((System.ComponentModel.ISupportInitialize)(this.listReports)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
			this.ResumeLayout(false);
		}
		private DevExpress.XtraEditors.LabelControl lbReportName;
		private DevExpress.XtraPrinting.Native.LinkLabelControl linkAssembly;
		private XtraTreeList.TreeList listReports;
		private BindingSource bindingSource1;
		private System.ComponentModel.IContainer components;
		private XtraTreeList.Columns.TreeListColumn colName;
		private XtraTreeList.Columns.TreeListColumn colNameSpace;
		private XtraTreeList.Columns.TreeListColumn colAssemblyLocation;
		private OpenFileDialog openFileDialog;
	}
}
