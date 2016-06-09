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

using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Reports.UserDesigner.GroupSort;
using DevExpress.Xpf.Reports.UserDesigner.GroupSort.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace DevExpress.Xpf.Reports.UserDesigner.GroupSort {
	public interface IGroupSortItem : INotifyPropertyChanged {
		int Id { get; set; }
		int ParentId { get; }
		GroupField GroupField { get; }
		BindingData BindingData { get; set; }
		XRColumnSortOrder SortOrder { get; set; }
		bool ShowHeader { get; set; }
		bool ShowFooter { get; set; }
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.GroupSort.Native {
	[POCOViewModel]
	public class GroupSortItem : IGroupSortItem {
		public static IGroupSortItem Create(IGroupSortFieldController controller, GroupField field, BindingData bindingData) {
			return ViewModelSource.Create(() => new GroupSortItem(controller, field, bindingData));
		}
		readonly IGroupSortFieldController controller;
		bool showHeader = false;
		bool showFooter = false;
		BindingData bindingData;
		protected GroupSortItem(IGroupSortFieldController controller, GroupField field, BindingData bindingData) {
			this.controller = controller;
			GroupField = field;
			this.bindingData = bindingData;
			SortOrder = field.SortOrder;
			UpdateInternal();
		}
		public virtual int Id { get; set; }
		public int ParentId { get { return Id - 1; } }
		[BindableProperty(OnPropertyChangedMethodName = "OnBindingDataChanged")]
		public virtual BindingData BindingData { get { return bindingData; } set { bindingData = value; } }
		public virtual GroupField GroupField { get; protected set; }
		[BindableProperty(OnPropertyChangedMethodName = "OnShowFooterChanged")]
		public virtual bool ShowFooter { get { return showHeader && showFooter; } set { showFooter = value; } }
		[BindableProperty(OnPropertyChangedMethodName = "OnShowHeaderChanged")]
		public virtual bool ShowHeader { get { return showHeader; } set { showHeader = value; } }
		public virtual XRColumnSortOrder SortOrder { get; set; }
		protected void OnBindingDataChanged() {
			controller.ChangeField(GroupField, BindingData);
		}
		protected void OnSortOrderChanged() {
			controller.ChangeSortOrder(GroupField, SortOrder);
		}
		protected void OnShowHeaderChanged() {
			controller.ShowGroupHeader(GroupField, ShowHeader);
		}
		protected void OnShowFooterChanged() {
			controller.ShowGroupFooter(GroupField, ShowFooter);
		}
		internal void UpdateInternal() {
			var group = GetGroup();
			if(group != null) {
				showHeader = group.Header != null;
				showFooter = group.Footer != null;
			}
			RaisePropertyChanged("");
		}
		XRGroup GetGroup() {
			var band = GroupField.Band;
			return band is GroupBand ? band.Report.Groups.FindGroupByBand((GroupBand)band) : null;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string name) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
