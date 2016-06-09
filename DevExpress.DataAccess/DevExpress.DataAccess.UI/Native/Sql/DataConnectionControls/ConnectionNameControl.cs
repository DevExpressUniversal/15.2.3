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
using System.ComponentModel;
using System.Drawing;
using DevExpress.DataAccess.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls {
	[ToolboxItem(false)]
	public partial class ConnectionNameControl : XtraUserControl {
		bool isConnectionNameChanged;
		readonly Locker locker = new Locker();
		internal Point ConnectionNameLocation { get { return this.dataConnectionNameTextEdit.PointToScreen(new Point(0, 0)); } }
		public string ConnectionName
		{
			get { return this.dataConnectionNameTextEdit.Text; }
			set
			{
				this.locker.Lock();
				this.dataConnectionNameTextEdit.Text = value;
				this.locker.Unlock();
			}
		}
		public bool IsConnectionNameChanged { get { return this.isConnectionNameChanged; } set { this.isConnectionNameChanged = value; } }
		public ConnectionNameControl() {
			InitializeComponent();
			this.dataConnectionNameTextEdit.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		void dataConnectionNameTextEdit_EditValueChanging(object sender, ChangingEventArgs e) {
			if(!this.locker.IsLocked && !String.IsNullOrEmpty(e.OldValue.ToString()))
				this.isConnectionNameChanged = !Equals(e.NewValue, e.OldValue);
		}
	}
}
