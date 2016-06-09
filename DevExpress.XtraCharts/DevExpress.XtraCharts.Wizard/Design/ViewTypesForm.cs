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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public class ViewTypesForm : ItemKindChoosingForm {
		string editValue;
		static int lastSelectedIndex = 0;
		public string EditValue {
			get { return editValue; }
			set {
				editValue = value;
				foreach (ListViewItem item in listView.Items)
					item.Selected = item.Text == value;
			}
		}
		public ViewTypesForm() {
			InitializeComponent();
		}
		void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewTypesForm));
			this.SuspendLayout();
			resources.ApplyResources(this.listView, "listView");
			resources.ApplyResources(this.btnCancel, "btnCancel");
			resources.ApplyResources(this.btnOk, "btnOk");
			resources.ApplyResources(this.separator, "separator");
			resources.ApplyResources(this, "$this");
			this.Name = "ViewTypesForm";
			this.ResumeLayout(false);
		}
		protected override void CloseForm() {
			ListView.SelectedListViewItemCollection coll = listView.SelectedItems;
			if (coll.Count > 0) {
				lastSelectedIndex = listView.Items.IndexOf(coll[0]);
				editValue = coll[0].Text;
				DialogResult = DialogResult.OK;
			}
		}
		protected override void Initialize() {
			Image[] images = SeriesViewFactory.SeriesViewImages;
			foreach (Image image in images)
				imageList.Images.Add(image);
			string[] names = SeriesViewFactory.StringIDs;
			for (int i = 0; i < names.Length; i++)
				listView.Items.Add(new ListViewItem(names[i], i));
			listView.Items[lastSelectedIndex].Selected = true;
		}
	}
}
