#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	public partial class DataSourceBrowser : DashboardUserControl, IDataSourceBrowserView {
		readonly DataSourceSelector dataSourceSelector;
		readonly DraggableDataFieldsBrowser dataFieldsBrowser;
		public bool AllowGlyphSkinning {
			get { return dataFieldsBrowser.AllowGlyphSkinning; }
			set { dataFieldsBrowser.AllowGlyphSkinning = value; }
		}		
		protected override IEnumerable<object> Children { get { return new object[] { dataSourceSelector, separator1, dataFieldsBrowser }; } }
		public DataSourceBrowser() {
			InitializeComponent();
			Dock = DockStyle.Fill;
			dataSourceSelector = new DataSourceSelector() { Dock = DockStyle.Top };
			dataFieldsBrowser = new DraggableDataFieldsBrowser() { Dock = DockStyle.Fill };
			Controls.Add(this.dataSourceSelector);
			panelDataFieldsBrowser.Controls.Add(this.dataFieldsBrowser);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region IDataSourceBrowserView members
		IDataSourceSelectorView IDataSourceBrowserView.SelectorView { get { return dataSourceSelector; } }
		IDataFieldsBrowserView IDataSourceBrowserView.FieldsBrowserView { get { return dataFieldsBrowser; } }
		#endregion
	}
}
