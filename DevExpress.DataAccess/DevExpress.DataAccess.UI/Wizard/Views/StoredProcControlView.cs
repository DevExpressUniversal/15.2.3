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
using System.Linq;
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class StoredProcControlView : XtraUserControl {
		public StoredProcControlView() {
			InitializeComponent();
			LocalizeComponent();
		}
		#region Implementation of IStoredProcControlView
		public int SelectedIndex {
			get {
				return listBoxStoredProcedures.SelectedIndex;
			}
			set {
				if(listBoxStoredProcedures.SelectedIndex == value)
					return;
				listBoxStoredProcedures.SelectedIndexChanged -= listBoxStoredProcedures_SelectedIndexChanged;
				listBoxStoredProcedures.SelectedIndex = value;
				listBoxStoredProcedures.SelectedIndexChanged += listBoxStoredProcedures_SelectedIndexChanged;
			}
		}
		public void Init(IEnumerable<string> items) {
			listBoxStoredProcedures.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			listBoxStoredProcedures.SelectedIndexChanged -= listBoxStoredProcedures_SelectedIndexChanged;
			listBoxStoredProcedures.Items.Clear();
			object[] itemsArr = items as string[] ?? items.Cast<object>().ToArray();
			if(itemsArr.Length > 0) {
				listBoxStoredProcedures.Items.AddRange(itemsArr);
				listBoxStoredProcedures.SelectionMode = SelectionMode.One;
			}
			else {
				listBoxStoredProcedures.Items.Add(DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardEmptyStoredProceduresListMessage));
				listBoxStoredProcedures.SelectionMode = SelectionMode.None;
			}
			listBoxStoredProcedures.SelectedIndexChanged += listBoxStoredProcedures_SelectedIndexChanged;
		}
		public event EventHandler Changed;
		#endregion
		void LocalizeComponent() {
			labelCaption.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.StoredProcControl_Caption);
		}
		void listBoxStoredProcedures_SelectedIndexChanged(object sender, EventArgs e) {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		public event MouseEventHandler ListDoubleClick {
			add { listBoxStoredProcedures.MouseDoubleClick += value; }
			remove { listBoxStoredProcedures.MouseDoubleClick -= value; }
		}
	}
}
