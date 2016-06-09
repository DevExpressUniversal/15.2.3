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
using System.Windows.Forms.Design;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public partial class DataMemberPickerContainer : XtraPanel {
		IWindowsFormsEditorService edSvc;
		object dataSource;
		string chartDataMember;
		string dataMember;
		DataContext dataContext;
		public string DataMember { get { return dataMember; } }
		public DataMemberPickerContainer(IWindowsFormsEditorService edSvc) {
			this.edSvc = edSvc;
			InitializeComponent();
		}
		public void Initialize(DataContext dataContext, object dataSource, string chartDataMember, string dataMember, IServiceProvider provider, ScaleType[] filterScaleTypes) {
			this.dataSource = dataSource;
			this.chartDataMember = chartDataMember;
			this.dataMember = dataMember;
			this.dataContext = dataContext;
			dataMemberPicker.SetFilterCriteria(filterScaleTypes, false);
			dataMemberPicker.SetServiceProvider(provider);
			dataMemberPicker.FillDataSource(dataSource, chartDataMember);
			dataMemberPicker.Start();			
			if (dataMember != null)
				dataMemberPicker.SelectDataMember(dataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, dataMember));
		}
		void dataMemberPicker_SelectionChanged(object sender, EventArgs e) {
			if (dataMemberPicker.IsDataMemberNode) {
				dataMember = BindingHelper.ExtractDataMember(dataMemberPicker.DataMember, chartDataMember);
				dataMember = (dataMember != String.Empty &&
					!BindingHelper.CheckDataMember(dataContext, dataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, dataMember))) ?
						null : dataMember;
				edSvc.CloseDropDown();
			}
		}
		void dataMemberPicker_TreeViewDoubleClick(object sender, EventArgs e) {
			if (dataMemberPicker.IsNoneNode) {
				dataMember = String.Empty;
				edSvc.CloseDropDown();
			}
			dataMemberPicker_SelectionChanged(sender, e);
		}
	}
}
