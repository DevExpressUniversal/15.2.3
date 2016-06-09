#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Native;
using System.Collections;
using DevExpress.Utils;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public partial class DataSourceSelector : DashboardUserControl, IDataSourceSelectorView {
		class DataSourceComboBoxItem {
			public IDashboardDataSource DataSource { get; private set; }
			public DataSourceComboBoxItem(IDashboardDataSource dataSource) {
				Guard.ArgumentNotNull(dataSource, "dataSource");
				DataSource = dataSource;
			}
			public override string ToString() {
				return DataSource.Name;
			}
			public override bool Equals(object obj) {
				DataSourceComboBoxItem item = obj as DataSourceComboBoxItem;
				return item != null && item.DataSource == DataSource;
			}
			public override int GetHashCode() {
				return DataSource.GetHashCode();
			}
		}
		ImageCollection imageCollection = new ImageCollection();
		Locker eventLocker = new Locker();
		protected override IEnumerable<object> Children { get { return new object[] { cbDataSource, cbDataMember, lblDataMember, lblDataSource }; } }
		public IDashboardDataSource SelectedDataSource {
			get {
				DataSourceComboBoxItem cbItem = cbDataSource.SelectedItem as DataSourceComboBoxItem;
				if(cbItem != null)
					return cbItem.DataSource;
				return null;
			}
			set {
				cbDataSource.SelectedItem = null;
				cbDataSource.SelectedItem = value == null ? null : new DataSourceComboBoxItem(value); 
			}
		}
		public string SelectedDataMember {
			get {
				ImageComboBoxItem cbItem = cbDataMember.SelectedItem as ImageComboBoxItem;
				if(cbItem != null)
					return cbItem.Description;
				return null;
			}
			set {
				cbDataMember.SelectedItem = cbDataMember.Properties.Items.FirstOrDefault(item => item.Value.ToString() == value);
			}
		}
		public event EventHandler SelectedDataSourceChanged;
		public event EventHandler SelectedDataMemberChanged;
		public DataSourceSelector() {
			InitializeComponent();
			imageCollection.AddImage(ImageHelper.GetImage("NotServerModeDataMember"));
			imageCollection.AddImage(ImageHelper.GetImage("ServerModeDataMember"));
		}
		void OnDataSourceSelectedIndexChanged(object sender, EventArgs e) {
			if(SelectedDataSourceChanged != null && !eventLocker.IsLocked)
				SelectedDataSourceChanged(this, EventArgs.Empty);
		}
		void OnDataMemberSelectedIndexChanged(object sender, EventArgs e) {
			if(SelectedDataMemberChanged != null && !eventLocker.IsLocked)
				SelectedDataMemberChanged(this, EventArgs.Empty);
		}
		void IDataSourceSelectorView.BeginUpdate() {
			eventLocker.Lock();
		}
		void IDataSourceSelectorView.EndUpdate() {
			eventLocker.Unlock();
		}
		void IDataSourceSelectorView.RefreshDataSources(DataSourceCollection dataSources) {
			ComboBoxItemCollection items = cbDataSource.Properties.Items;
			items.Clear();
			foreach(IDashboardDataSource dataSource in dataSources)
				items.Add(new DataSourceComboBoxItem(dataSource));
		}
		bool HasImages(IEnumerable<ImageComboBoxItem> dataMembers) {
			return dataMembers.FirstOrDefault(dataMember => dataMember.ImageIndex != -1) != null;
		}
		void IDataSourceSelectorView.RefreshDataMembers(IEnumerable<ImageComboBoxItem> dataMembers) {
			cbDataMember.Properties.Items.Clear();
			if(dataMembers != null && dataMembers.Count() > 0) {
				cbDataMember.Properties.SmallImages = HasImages(dataMembers) ? imageCollection : null;
				pnlDataMember.Visible = true;
				cbDataMember.Properties.Items.AddRange(dataMembers.ToArray());
				cbDataMember.SelectedIndex = 0;
				Height = pnlDataMember.Bottom;
				if(SelectedDataSource is DashboardSqlDataSource)
					lblDataMember.Text = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceQuery);
				else
					lblDataMember.Text = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceDataMember);
			} else {
				pnlDataMember.Visible = false;
				cbDataMember.SelectedItem = null;
				Height = cbDataMember.Bottom;
			}
		}
		void IDataSourceSelectorView.RenameSelectedItem() {
			DataSourceComboBoxItem selectedItem = cbDataSource.SelectedItem as DataSourceComboBoxItem;
			ComboBoxItemCollection items = cbDataSource.Properties.Items;
			int index = items.IndexOf(selectedItem);
			items.RemoveAt(index);
			items.Insert(index, selectedItem);
			cbDataSource.SelectedIndex = index;
			cbDataSource.Invalidate();
		}
	}
}
