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
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Entity.Model;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class ChooseEFContextPageView : WizardViewBase, IChooseEFContextPageView {
		public event EventHandler<BrowseForAssemblyEventArgs> BrowseForAssembly;
		public event EventHandler<ContextNameChangedEventArgs> ContextNameChanged;
		public ChooseEFContextPageView() {
			InitializeComponent();
			LocalizeComponent();
		}
		public string ContextName {
			get { return listBoxContext.SelectedItem != null ? listBoxContext.SelectedItem.ToString() : null; }
			set { listBoxContext.SelectedItem = value; }
		}
		public void Initialize() { }
		public void RefreshContextList(IEnumerable<IContainerInfo> containers) {
			if(containers == null)
				return;
			listBoxContext.BeginUpdate();
			listBoxContext.Items.Clear();
			listBoxContext.Items.AddRange(containers.Select(c => c.FullName).ToArray());
			if(ContextNameChanged != null)
				ContextNameChanged(this, new ContextNameChangedEventArgs((string)listBoxContext.SelectedItem));
			listBoxContext.EndUpdate();
		}
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseEFContext); }
		}
		#endregion
		protected void RaiseBrowseForAssembly(string assemblyPath) {
			if(BrowseForAssembly != null)
				BrowseForAssembly(this, new BrowseForAssemblyEventArgs(assemblyPath));
		}
		protected void RaiseContextNameChanged(string contextName) {
			if(ContextNameChanged != null)
				ContextNameChanged(this, new ContextNameChangedEventArgs(contextName));
		}
		protected virtual void OnButtonBrowseForAssemblyClick() {
			openDialog.Filter = "assemblies (*.dll)|*.dll|executable (*.exe)|*.exe";
			if(openDialog.ShowDialog() == DialogResult.OK)
				RaiseBrowseForAssembly(openDialog.FileName);
		}
		void LocalizeComponent() {
			buttonBrowseForAssembly.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Browse);
		}
		void buttonBrowseForAssembly_Click(object sender, EventArgs e) { OnButtonBrowseForAssemblyClick(); }
		void contextList_SelectedValueChanged(object sender, EventArgs e) {
			if(listBoxContext.SelectedItem == null)
				return;
			string contextName = listBoxContext.SelectedItem.ToString();
			RaiseContextNameChanged(contextName);
		}
		void contextList_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(listBoxContext.IndexFromPoint(e.Location) != -1)
				MoveForward();
		}
	}
}
