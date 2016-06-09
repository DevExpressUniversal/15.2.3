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
using DevExpress.Data;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Utils.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	public partial class FiltersView : FilterView, IFiltersView {
		public FiltersView(IWin32Window owner, UserLookAndFeel lookAndFeel)
			: this(owner, lookAndFeel, null, null, null) {}
		public FiltersView(IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider,
			QueryFilterControl queryFilterControl, QueryFilterControl groupQueryFilterControl)
			: this(owner, lookAndFeel, displayNameProvider, queryFilterControl, groupQueryFilterControl, null) {}
		public FiltersView(IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider,
			FilterControl filterControl, FilterControl filterControlGroup, IParameterService parameterService)
			: this(owner, lookAndFeel, displayNameProvider, filterControl, filterControlGroup, parameterService, null) {}
		public FiltersView(IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider,
			FilterControl filterControl, FilterControl filterControlGroup, IParameterService parameterService,
			IServiceProvider propertyGridServices)
			: base(owner, lookAndFeel, displayNameProvider, filterControl, parameterService, propertyGridServices) {
			this.filterControlGroup = filterControlGroup ??
									  new QueryFilterControl(parameterService, propertyGridServices) {
										  MenuManager = barManager
									  };
			LookAndFeel.ParentLookAndFeel = LookAndFeel;
			InitializeComponent();
			LocalizeComponent();
		}
		FiltersView() {
			InitializeComponent();
			LocalizeComponent();
		}
		#region Implementation of IFiltersView
		public string GroupFilterString {
			get { return this.filterControlGroup.FilterString; }
			set { this.filterControlGroup.FilterString = value; }
		}
		public int SkipRecords {
			get { return RecordsLimitationEnabled ? int.Parse(textSkip.Text) : 0; }
			set { textSkip.EditValue = value; }
		}
		public int TopRecords {
			get { return RecordsLimitationEnabled ? int.Parse(textTop.Text) : 0; }
			set { textTop.EditValue = value; }
		}
		public bool RecordsLimitationEnabled {
			get { return checkTopAndSkip.Checked; }
			set { checkTopAndSkip.Checked = value; }
		}
		public bool TopEditEnabled {
			get { return textTop.Enabled; }
			set { textTop.Enabled = value; }
		}
		public bool SkipEditEnabled {
			get { return textSkip.Enabled; }
			set { textSkip.Enabled = value; }
		}
		public bool GroupFilterEnabled {
			get { return tabbedGroupFilter.TabPages[1].Enabled; }
			set { tabbedGroupFilter.TabPages[1].Enabled = value; }
		}
		public event EventHandler ChangeRecordsLimitationState {
			add { checkTopAndSkip.CheckStateChanged += value; }
			remove { checkTopAndSkip.CheckStateChanged -= value; }
		}
		public void SetGroupColumnsInfo(IEnumerable<FilterViewTableInfo> info) {
			this.filterControlGroup.SetFilterColumnsCollection(new QueryBuilderFilterColumnCollection(info,
				DisplayNameProvider));
			this.filterControlGroup.SetDefaultColumn(
				FindFirstSelectableNode(this.filterControlGroup.FilterColumns.Cast<FilterColumn>()));
		}
		public override void SetExistingParameters(IEnumerable<IParameter> parameters) {
			base.SetExistingParameters(parameters);
			this.filterControlGroup.SourceControl = this.filterControl.SourceControl;
		}
		#endregion
		protected override bool ProcessDialogKey(Keys keyData) {
			if((keyData == Keys.Escape) && textTop.DoValidate() && textSkip.DoValidate()) {
				Close();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		void LocalizeComponent() {
			this.buttonCancel.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Cancel);
			this.buttonOk.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_OK);
			this.labelTopSkipText.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.FiltersView_TopAndSkipText);
			this.layoutGroupGroupFilter.Text =
				DataAccessUILocalizer.GetString(DataAccessUIStringId.FiltersView_GroupFilter);
			this.layoutGroupFilter.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.FiltersView_Filter);
			this.checkTopAndSkip.Properties.Caption =
				DataAccessUILocalizer.GetString(DataAccessUIStringId.FiltersView_CheckText);
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.FiltersView);
		}
		void buttonOk_Click(object sender, EventArgs e) {
			RaiseOk();
		}
		void numTextEdits_Validating(object sender, CancelEventArgs e) {
			int value;
			e.Cancel = !int.TryParse(((TextEdit) sender).Text, out value) || value < 0;
		}
		void buttonCancel_Click(object sender, EventArgs e) {
			RaiseCancel();
		}
		void FiltersView_FormClosing(object sender, FormClosingEventArgs e) {
			RaiseCancel();
		}
	}
}
