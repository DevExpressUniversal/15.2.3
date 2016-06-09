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
using DevExpress.Data.Filtering;
using DevExpress.Snap.Extensions.Native;
using DevExpress.XtraEditors.Helpers;
namespace DevExpress.Snap.Extensions.UI {
	public class DateFilterControlUserControl : OkCancelUserControl {
		readonly PopupOutlookDateFilterControl popupOutlookDateFilterControl;
		readonly IFilterOwner filterOwner;
		readonly string dataFieldName;
		DateFilterControlUserControl() { }
		public DateFilterControlUserControl(IFilterOwner filterOwner, string dataFieldName, IEnumerable<object> values, CriteriaOperator currentFilter) {
			this.filterOwner = filterOwner;
			this.dataFieldName = dataFieldName;
			popupOutlookDateFilterControl = new PopupOutlookDateFilterControl(null);
			popupOutlookDateFilterControl.Cache = new DateFilterInfoCache();
			popupOutlookDateFilterControl.Cache.Init(new List<object>(values).ToArray());
			popupOutlookDateFilterControl.Field = new SNFilterColumn(dataFieldName);
			popupOutlookDateFilterControl.Init(null, currentFilter);
			popupOutlookDateFilterControl.CreateControls();
			Height += popupOutlookDateFilterControl.Height;
			Width = popupOutlookDateFilterControl.Width;
			Controls.Add(popupOutlookDateFilterControl);
		}
		protected override void btnOK_Click(object sender, EventArgs e) {
			popupOutlookDateFilterControl.ApplyFilter();
			DateFilterResult result = popupOutlookDateFilterControl.CalcFilterResult();
			if (result != null)
				filterOwner.ApplyFilter(dataFieldName, result.FilterCriteria);
			base.btnOK_Click(sender, e);
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateFilterControlUserControl));
			this.okCancelButtonsPanel.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "DateFilterControlUserControl";
			this.okCancelButtonsPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
	}
}
