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
using System.Windows.Forms;
using DevExpress.Utils.Extensions.Helpers;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	public partial class SetReportServiceReportNameView : UserControl, ISetReportServiceReportNameView {
		public event EventHandler ReportStorageParametersChanged;
		string IPageView.HeaderText {
			get { return "Remote report name"; }
		}
		string IPageView.DescriptionText {
			get { return "Specify the Report Service Uri and the report name"; }
		}
		public string ServiceUri {
			get {
				return uriEdit.Text;
			}
			set {
				if(uriEdit.Properties.Items.Contains(value))
					uriEdit.SelectedItem = value;
				else uriEdit.Text = value;
			}
		}
		public string ReportName {
			get {
				return reportNameEdit.Text;
			}
			set {
				reportNameEdit.Text = value;
			}
		}
		public SetReportServiceReportNameView() {
			InitializeComponent();
		}
		void OnReportStorageParametersChanged(object sender, EventArgs e) {
			ReportStorageParametersChanged.SafeRaise(this);
		}
		public void FillServices(IEnumerable<string> services) {
			uriEdit.FillItems(services);
		}
		public void FillReports(IEnumerable<string> reportTypeNames) {
			reportNameEdit.FillItems(reportTypeNames);
		}
	}
}
