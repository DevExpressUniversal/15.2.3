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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class StyleFormatConditionFrame : FormatRulesBaseFrame {
		protected DevExpress.XtraGrid.Views.Base.ColumnView ColumnView { get { return EditingObject as DevExpress.XtraGrid.Views.Base.ColumnView; } }
		protected override string DescriptionText { get { return DevExpress.XtraGrid.Design.Properties.Resources.FormatConditionDescription; } }
		public StyleFormatConditionFrame() {
			AssingButtonsToolTip();
		}
		protected void AssingButtonsToolTip() {
			btAdd.ToolTipTitle = DevExpress.XtraGrid.Design.Properties.Resources.AddStyleConditionText;
			btAdd.ToolTip = DevExpress.XtraGrid.Design.Properties.Resources.AddStyleConditionDescription;
			btRemove.ToolTipTitle = DevExpress.XtraGrid.Design.Properties.Resources.RemoveStyleConditionText;
			btRemove.ToolTip = DevExpress.XtraGrid.Design.Properties.Resources.RemoveStyleConditionDescription;
			btnUp.ToolTipTitle = DevExpress.XtraGrid.Design.Properties.Resources.MoveItemTowardBeginningText;
			btnUp.ToolTip = DevExpress.XtraGrid.Design.Properties.Resources.MoveStyleConditionUpDescription;
			btnDown.ToolTipTitle = DevExpress.XtraGrid.Design.Properties.Resources.MoveItemTowardEndText;
			btnDown.ToolTip = DevExpress.XtraGrid.Design.Properties.Resources.MoveStyleConditionDownDescription;
		}
		protected override void DoConvert() {
			ColumnView.ConvertFormatConditionToFormatRules();
			base.DoConvert();
		}
		protected override bool AllowConvert {
			get {
				return ColumnView.FormatConditions.Count > 0;
			}
		}
		protected override bool ArrowsVisible {
			get {
				if(SampleFormatConditions != null) return false;
				return true; 
			}
		}
		protected override IFormatRuleCollection FormatConditions { get {return ColumnView != null ? ColumnView.FormatRules : null; } }
		protected override FormatRuleBase CreateFormatCondition() { return new GridFormatRule(); }
	}
}
