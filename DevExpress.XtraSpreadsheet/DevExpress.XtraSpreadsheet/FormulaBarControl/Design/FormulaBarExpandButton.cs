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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraSpreadsheet {
	[DXToolboxItem(false)]
	public partial class FormulaBarExpandButton : ButtonEdit, IToolTipControlClient {
		FormulaBarExpandButtonToolTipService toolTipService;
		bool isExpand;
		readonly FormulaBarCellInplaceEditor owner;
		public FormulaBarExpandButton(FormulaBarCellInplaceEditor owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			InitializeComponent();
			this.toolTipService = new FormulaBarExpandButtonToolTipService(this);
			this.isExpand = false;
			this.Cursor = Cursors.Arrow;
		}
		public bool IsExpand { get { return isExpand; } set { isExpand = value; } }
		public Keys Shortcut { 
			get {
				return owner.GetShortcut(RichEditCommandId.CollapseOrExpandFormulaBar);
			} 
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			return toolTipService.CalculateToolTipInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		protected override void OnClick(EventArgs e) {
			isExpand = !isExpand;
			base.OnClick(e);
		}
	}
}
