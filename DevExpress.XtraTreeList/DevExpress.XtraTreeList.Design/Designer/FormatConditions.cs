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
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraTreeList.StyleFormatConditions;
namespace DevExpress.XtraTreeList.Frames {
	[ToolboxItem(false)]
	public class FormatConditionFrame : FormatRulesBaseFrame {
		public FormatConditionFrame() {
		}
		public TreeList TreeList { get { return EditingObject as TreeList; } }
		protected override IFormatRuleCollection FormatConditions { get { return TreeList != null ? TreeList.FormatRules : null; } }
		protected override FormatRuleBase CreateFormatCondition() { return new TreeListFormatRule(); }
		protected override string DescriptionText { get { return "You can add, remove or modify items to apply styles to columns or entire rows based upon a custom format rule."; } }
		protected override bool ArrowsVisible {
			get {
				if(SampleFormatConditions != null) return false;
				return true;
			}
		}
		protected override bool AllowConvert {
			get {
				return  TreeList != null && TreeList.FormatConditions.Count > 0;
			}
		}
		protected override void DoConvert() {
			if(TreeList != null)
				TreeList.ConvertFormatConditionToFormatRules();
			base.DoConvert();
		}
	}
}
