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
using DevExpress.Skins.Info;
using DevExpress.Utils.Design;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.XtraSplashScreen;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	public partial class SetReportServerEndpointView : UserControl, ISetReportServerEndpointView {
		string IPageView.HeaderText {
			get { return "Client Endpoint Configuration"; }
		}
		string IPageView.DescriptionText {
			get { return "Select a client configuration endpoint name that will be stored in the App.config file"; }
		}
		public SetReportServerEndpointView() {
			InitializeComponent();
			SplashScreenManager.RegisterUserSkin(new SkinBlobXmlCreator("DevExpress Design",
				"DevExpress.Utils.Design.", typeof(XtraDesignForm).Assembly, null));
		}
		#region ISetReportServerEndpointView Members
		public event EventHandler EndpointChanged;
		public event EventHandler GenerateEndpointsChanged;
		public string Endpoint {
			get { return endpointEdit.SelectedItem as string; }
			set {
				if(endpointEdit.Properties.Items.Contains(value)) {
					endpointEdit.SelectedItem = value;
				} else {
					endpointEdit.Text = value;
				}
			}
		}
		public bool GenerateEndpoints {
			get {
				return generateEndpoints.Checked;
			}
			set {
				generateEndpoints.Checked = value;
			}
		}
		public void FillEndpoints(IEnumerable<string> endpoints) {
			endpointEdit.FillItems(endpoints);
		}
		#endregion
		void endpointEdit_TextChanged(object sender, EventArgs e) {
			EndpointChanged.SafeRaise(this);
		}
		void generateEndpoints_CheckedChanged(object sender, EventArgs e) {
			endpointEdit.Enabled = generateEndpoints.Checked;
			GenerateEndpointsChanged.SafeRaise(this);
		}
	}
}
