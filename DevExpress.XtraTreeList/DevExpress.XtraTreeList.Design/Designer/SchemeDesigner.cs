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
using DevExpress.XtraTreeList.Design;
namespace DevExpress.XtraTreeList.Frames {
	[ToolboxItem(false)]
	[CLSCompliant(false)]
	public class SchemeDesigner : DevExpress.XtraEditors.Frames.SchemeDesignerBase {
		private System.ComponentModel.Container components = null;
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
		#endregion
		#region Init & Ctor
		public TreeList EditingTreeList { get { return EditingObject as TreeList; } }
		public SchemeDesigner() {
			InitializeComponent();
		}
		TreeList TreeListPattern;
		private XAppearances xs = null;
		XAppearances XS {
			get {
				if(xs == null) xs = new XAppearances("System");
				return xs;
			}
		}
		public override void InitComponent() {
			TreeListPattern = new TreeList();
			TreeListPattern.Dock = DockStyle.Fill;
			AddPreviewControl(TreeListPattern);
			XViews XV = new XViews(TreeListPattern);
			StyleList.Items.Clear();
			StyleList.Items.AddRange(XS.FormatNames);
			TreeListPattern.LookAndFeel.ParentLookAndFeel = EditingTreeList.LookAndFeel;
			TreeListPattern.OptionsView.EnableAppearanceEvenRow = EditingTreeList.OptionsView.EnableAppearanceEvenRow;
			TreeListPattern.OptionsView.EnableAppearanceOddRow = EditingTreeList.OptionsView.EnableAppearanceOddRow;
			TreeListPattern.Appearance.Assign(EditingTreeList.Appearance);
		}
		#endregion
		#region Editing
		protected override void LoadSchemePreview() {
			XS.LoadScheme(StyleList.SelectedItem.ToString(), TreeListPattern);
		}
		protected override void LoadScheme() {
			XS.LoadScheme(StyleList.SelectedItem.ToString(), EditingTreeList);
			EditingTreeList.SetDefaultRowHeight();
			if(StyleList.SelectedIndex == 0) EditingTreeList.FireChanged();
		}
		protected override void SetFormatNames(bool isEnabled) {
			XS.ShowNewStylesOnly = isEnabled;
			StyleList.Items.AddRange(XS.FormatNames);
		}
		#endregion
	}
}
