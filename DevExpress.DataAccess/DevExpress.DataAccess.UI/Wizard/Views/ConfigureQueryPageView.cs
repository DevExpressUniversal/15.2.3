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
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class ConfigureQueryPageView : WizardViewBase, IConfigureQueryPageView {
		readonly QueryControlView tableQueryControl;
		readonly StoredProcControlView spQueryControl;
		public ConfigureQueryPageView(IDisplayNameProvider displayNameProvider, IServiceProvider propertyGridServices, ICustomQueryValidator customQueryValidator,  SqlWizardOptions options) {
			InitializeComponent();
			LocalizeComponent();
			tableQueryControl = new QueryControlView(!options.HasFlag(SqlWizardOptions.EnableCustomSql), options.HasFlag(SqlWizardOptions.QueryBuilderLight), displayNameProvider, propertyGridServices, customQueryValidator);
			spQueryControl = new StoredProcControlView();
			panel.Controls.Add(tableQueryControl);
			tableQueryControl.Focus();
			spQueryControl.ListDoubleClick += spQueryControl_ListDoubleClick;
			radioGroupQueryType.EditValueChanged += radioGroupQueryType_EditValueChanged;
			buttonQueryBuilder.Click += buttonQueryBuilder_Click;
		}
		ConfigureQueryPageView() : this(null, null, null, SqlWizardOptions.EnableCustomSql) { }
		protected override void OnCreateControl() {
			base.OnCreateControl();
			layoutControlButtons.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		public string SqlString {
			get { return tableQueryControl.SqlString; }
			set { tableQueryControl.SqlString = value; }
		}
		public int SelectedStoredProcedureIndex {
			get { return spQueryControl.SelectedIndex; }
			set { spQueryControl.SelectedIndex = value; }
		}
		public QueryType QueryType {
			get { return (bool)radioGroupQueryType.EditValue ? QueryType.TableOrCustomSql : QueryType.StoredProcedure; }
			set {
				radioGroupQueryType.EditValue = value == QueryType.TableOrCustomSql;
				SwitchControls();
			}
		}
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureQuery); }
		}
		#endregion
		public void Initialize(bool allowCustomSql, bool storedProceduresSupported) {
			tableQueryControl.Initialize(allowCustomSql);
			RadioGroupItem storedProcRadio = radioGroupQueryType.Properties.Items[1];
			storedProcRadio.Enabled = storedProceduresSupported;
		}
		public QueryBuilderRunnerBase CreateQueryBuilderRunner(IDBSchemaProvider schemaProvider, SqlDataConnection connection, IParameterService parameterService) {
			return tableQueryControl.CreateQueryBuilderRunner(schemaProvider, connection, parameterService);
		}
		public void InitializeStoredProcedures(IEnumerable<string> items) { spQueryControl.Init(items); }
		protected void RaiseChanged() {
			if(QueryTypeChanged != null)
				QueryTypeChanged(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			radioGroupQueryType.Properties.Items[0].Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureQuery_Query);
			radioGroupQueryType.Properties.Items[1].Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureQuery_StoredProcedure);
			buttonQueryBuilder.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_QueryBuilder);
		}
		void SwitchControls() {
			XtraUserControl switchTo;
			switch(QueryType) {
				case QueryType.TableOrCustomSql:
					panel.Controls.Remove(spQueryControl);
					switchTo = tableQueryControl;
					buttonQueryBuilder.Enabled = true;
					break;
				case QueryType.StoredProcedure:
					panel.Controls.Remove(tableQueryControl);
					switchTo = spQueryControl;
					buttonQueryBuilder.Enabled = false;
					break;
				default:
					throw new InvalidOperationException();
			}
			panel.Controls.Add(switchTo);
			switchTo.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			switchTo.Dock = DockStyle.Fill;
		}
		void buttonQueryBuilder_Click(object sender, EventArgs e) {
			if(RunQueryBuilder != null)
				RunQueryBuilder(this, EventArgs.Empty);
		}
		void spQueryControl_ListDoubleClick(object sender, MouseEventArgs e) {
			ListBoxControl listBox = (ListBoxControl)sender;
			if(listBox.IndexFromPoint(e.Location) != -1)
				this.MoveForward();
		}
		void radioGroupQueryType_EditValueChanged(object sender, EventArgs e) {
			RaiseChanged();
			SwitchControls();
		}
		public event EventHandler QueryTypeChanged;
		public event EventHandler RunQueryBuilder;
		public event EventHandler SqlStringChanged {
			add { tableQueryControl.Changed += value; }
			remove { tableQueryControl.Changed -= value; }
		}
		public event EventHandler StoredProcedureChanged {
			add { spQueryControl.Changed += value; }
			remove { spQueryControl.Changed -= value; }
		}
	}
}
