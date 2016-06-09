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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.Xpo.Design {
	public partial class ODataVersionSelectForm : XtraForm {
		const string odataVersionEmpty = "_._._._";
		const string odataVersionInitial = "5.6.1.0";
		string odataVersionValue = odataVersionEmpty;
		public string SelectedODataVersion { get { return odataVersion.Text; } }
		public bool UseORM { get { return useOrmCheckBox.Checked; } }
		public bool DownloadOData { 
			get { return downloadOData.SelectedIndex == 0; }
			set {
				if(value)
					downloadOData.SelectedIndex = 0;
				else
					downloadOData.SelectedIndex = 1;
				downloadOData.Enabled = false;
			}
		}
		const string DescriptionStart = "OData Service for WCF Data Services ";
		public ODataVersionSelectForm() {
			InitializeComponent();
		}
		private void downloadOData_SelectedIndexChanged(object sender, EventArgs e) {
			odataVersion.Enabled = downloadOData.SelectedIndex == 1;
			if(odataVersion.Enabled) {
				if(odataVersionValue == odataVersionEmpty)
					odataVersionValue = odataVersionInitial;
				odataVersion.EditValue = odataVersionValue;
			} else {
				odataVersionValue = (string)odataVersion.EditValue;
				odataVersion.EditValue = odataVersionEmpty;
			}
		}
	}
}
