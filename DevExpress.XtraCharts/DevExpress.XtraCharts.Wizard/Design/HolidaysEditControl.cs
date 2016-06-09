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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Design {
	public partial class HolidaysEditControl : XtraUserControl {
		IKnownDateCollectionAccessor collectionAccessor;
		IChartContainer chartContainer;
		bool isHolidays;
		public HolidaysEditControl() {
			InitializeComponent();
			Assembly assembly = Assembly.GetAssembly(typeof(Chart));
			btnAdd.Image = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraCharts.Design.Images.Add.png", assembly);
			btnRemove.Image = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraCharts.Design.Images.Delete.png", assembly);
			btnImport.Image = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraCharts.Design.Images.Open.png", assembly);
		}
		public void SetValue(IKnownDateCollectionAccessor collectionAccessor, IChartContainer chartContainer, bool isHolidays) {
			this.collectionAccessor = collectionAccessor;
			this.chartContainer = chartContainer;
			this.isHolidays = isHolidays;
			if (isHolidays)
				lblHolidays.Text = ChartLocalizer.GetString(ChartStringId.Holidays) + ":";
			else {
				lblHolidays.Text = ChartLocalizer.GetString(ChartStringId.ExactWorkdays) + ":";
				btnImport.Enabled = false;
			}
			lbHolidays.SortOrder = SortOrder.Ascending;
			FillList();
		}
		void FillList() {
			if (collectionAccessor.Count > 0) {
				lbHolidays.Items.BeginUpdate();
				lbHolidays.Items.Clear();
				foreach (KnownDate date in collectionAccessor)
					lbHolidays.Items.Add(date);
				lbHolidays.Items.EndUpdate();
				lbHolidays.SelectedIndex = 0;
			}
			else
				lbHolidays.Items.Clear();
			UpdateControls();
		}
		void UpdateControls() {
			btnRemove.Enabled = lbHolidays.SelectedItem != null;
		}
		void AddHoliday() {
			using (KnownDateForm form = new KnownDateForm(isHolidays)) {
				if (form.ShowDialog() == DialogResult.OK) {
					KnownDate date = new KnownDate(form.txtName.Text, form.dateEdit.DateTime);
					collectionAccessor.Add(date);
					lbHolidays.Items.Add(date);
					lbHolidays.SelectedItem = date;
					UpdateControls();
				}
			}
		}
		void RemoveHoliday() {
			KnownDate date = lbHolidays.SelectedItem as KnownDate;
			if (date != null) {
				collectionAccessor.Remove(date);
				int index = lbHolidays.SelectedIndex;
				lbHolidays.Items.Remove(date);
				int count = lbHolidays.Items.Count;
				if (count > 0)
					lbHolidays.SelectedIndex = index >= count ? count - 1 : index;
				UpdateControls();
			}
		}
		void lbHolidays_KeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Insert:
					AddHoliday();
					break;
				case Keys.Delete:
					RemoveHoliday();
					break;
			}
		}
		void btnAdd_Click(object sender, EventArgs e) {
			AddHoliday();
		}
		void btnRemove_Click(object sender, EventArgs e) {
			RemoveHoliday();
		}
		void btnImport_Click(object sender, EventArgs e) {
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = ChartLocalizer.GetString(ChartStringId.HolidaysImportFilter);
				dlg.FilterIndex = 1;
				dlg.DefaultExt = "*.xml";
				dlg.CheckFileExists = true;
				if (dlg.ShowDialog() == DialogResult.OK)
					try {
						using (HolidaysImportForm form = new HolidaysImportForm(HolidaysLoader.LoadHolidaysCollection(dlg.FileName)))
							if (form.ShowDialog() == DialogResult.OK) {
								collectionAccessor.Clear();
								collectionAccessor.AddRange(form.ImportedHolidays);
								FillList();
								UpdateControls();  
							}
					}
					catch (Exception exc) {
						chartContainer.ShowErrorMessage(exc.Message, ChartLocalizer.GetString(ChartStringId.Holidays));
					}
			}
		}
	}
	public interface IKnownDateCollectionAccessor : IEnumerable {
		int Count { get; }
		void Add(KnownDate item);
		void Remove(KnownDate item);
		void AddRange(KnownDate[] items);
		void Clear();
	}
}
