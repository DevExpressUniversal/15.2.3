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
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class ChooseExcelFileDataRangePageView : WizardViewBase, IChooseExcelFileDataRangePageView {
		public ChooseExcelFileDataRangePageView() {
			InitializeComponent();
		}
		#region Overrides of WizardViewBase
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseExcelFileDataRange); } 
		}
		#endregion
		#region Implementation of IChooseExcelFileDataRangePageView
		public ListBoxItem SelectedItem {
			get { return (ListBoxItem)listBoxItems.SelectedItem; }
			set { listBoxItems.SelectedItem = value; }
		}
		public void Initialize(ListBoxItem[] items) {
			ImageCollection images = new ImageCollection();
			images.AddImage(GetImage("worksheet.png"), DataAccessUILocalizer.GetString(DataAccessUIStringId.ExcelDataSourceWizard_WorksheetItem));
			images.AddImage(GetImage("definedname.png"), DataAccessUILocalizer.GetString(DataAccessUIStringId.ExcelDataSourceWizard_DefinedNameItem));
			images.AddImage(GetImage("table.png"), DataAccessUILocalizer.GetString(DataAccessUIStringId.ExcelDataSourceWizard_TableItem));
			listBoxItems.ImageList = images;
			listBoxItems.ImageIndexMember = "ImageIndex";
			listBoxItems.DataSource = items;
		}
		public event EventHandler Changed;
		#endregion
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		Image GetImage(string name) {
			return ResourceImageHelper.CreateImageFromResources("DevExpress.DataAccess.UI.Wizard.Images." + name, GetType().Assembly);
		}
		#region UI Event handlers
		void lbItems_SelectedIndexChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		void lbItems_MouseDoubleClick(object sender, MouseEventArgs e) {
			ImageListBoxControl listBox = (ImageListBoxControl)sender;
			if(listBox.IndexFromPoint(e.Location) != -1)
				MoveForward();
		}
		#endregion
	}
}
