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

using DevExpress.LookAndFeel;
namespace DevExpress.DashboardWin.Native {
	public partial class TextEditForm : DashboardForm {
		public string Rtf { get { return richEditControl.RtfText; } }
		public TextEditForm() {
			InitializeComponent();
		}
		public TextEditForm(string rtf, UserLookAndFeel lookAndFeel) : this() {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			richEditControl.RtfText = rtf;
			InitBars();
		}
		void InitBars() {
			illustrationsBar.Visible = false;
			symbolsBar.Visible = false;
			linksBar.Visible = false;
			editingBar.Visible = false;
			floatingPictureArrangeBar.Visible = false;
			tablesBar.Visible = false;
			tableStylesBar.Visible = false;
			tableDrawBordersBar.Visible = false;
			tableAlignmentBar.Visible = false;
			tableRowsAndColumnsBar.Visible = false;
			tableCellSizeBar.Visible = false;
			tableMergeBar.Visible = false;
			tableTableBar.Visible = false;
		}
	}
}
