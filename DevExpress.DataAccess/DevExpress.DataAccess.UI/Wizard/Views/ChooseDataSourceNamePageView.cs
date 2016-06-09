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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.UI.Wizard.Views{
	[ToolboxItem(false)]
	public partial class ChooseDataSourceNamePageView : WizardViewBase, IChooseDataSourceNamePageView {
		public ChooseDataSourceNamePageView() {
			InitializeComponent();
		}
		public string DataSourceName { get { return textDataSourceName.Text; } set { textDataSourceName.Text = value; } }
		public void ShowErrorMessage() {
			string errorMessage = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardDataSourceNameExistsMessage);
			Point toolTipLocation = textDataSourceName.PointToScreen(new Point(0, 0));
			toolTipController.HideHint();
			if (Cursor.Current == null)
				Cursor.Current = Cursors.IBeam;
			toolTipController.ShowHint(errorMessage, toolTipLocation);
		}
		#region Overrides of XtraUserControl
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			textDataSourceName.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		#endregion
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseDataSourceName); }
		}
		#endregion
		private void dataSourceNameTextEdit_TextChanged(object sender, EventArgs e) {
			toolTipController.HideHint();
		}
	}
}
