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
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard.ChartTitleControls {
	internal partial class TitleControl : FilterTabsControl {
		DockableTitle title;
		AddTempTitleDelegate addTempTitleMethod;
		public override DevExpress.XtraTab.XtraTabControl TabControl { get { return xtraTabControl1; } }
		public TitleControl() {
			InitializeComponent();
		}
		public void Initialize(DockableTitle title, AddTempTitleDelegate method, UserLookAndFeel lookAndFeel, CollectionBase filter) {
			addTempTitleMethod = method;
			Initialize(title, lookAndFeel, filter, null);
		}
		public void Initialize(DockableTitle title, UserLookAndFeel lookAndFeel, CollectionBase filter, object selectedTabHandle) {
			this.title = title;
			InitializeCore(lookAndFeel, filter, selectedTabHandle);
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			addTempTitleMethod = null;
			txtText.EditValue = title.Text;
			titleGeneralControl.Initialize(title, addTempTitleMethod);
			UpdateControls();
		}
		protected override void InitializeTags() {
			tbGeneral.Tag = TitlePageTab.General;
			tbText.Tag = TitlePageTab.Text;
		}
		void UpdateControls() {
			txtText.Enabled = title.Visibility != Utils.DefaultBoolean.False;
		}
		void AddTitle() {
			if (addTempTitleMethod != null)
				addTempTitleMethod(title);
		}
		private void txtText_EditValueChanged(object sender, EventArgs e) {
			title.Text = txtText.EditValue.ToString();
			AddTitle();
		}
	}
	internal delegate void AddTempTitleDelegate(DockableTitle title);
}
