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
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Objects;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.InternalItems;
namespace DevExpress.XtraBars.ViewInfo {
	public class DockedBarControlOffice2000ViewInfo : DockedBarControlViewInfo {
		public DockedBarControlOffice2000ViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		public override void UpdateControlRegion(Control control) {
			control.Region = null;
		}
		public override int VertIndent { 
			get { 
				if(IsStatusBar) return 0;
				return base.VertIndent;
			}
		}
	}
	public class FloatingBarControlFormOffice2000ViewInfo : FloatingBarControlFormViewInfo {
		public FloatingBarControlFormOffice2000ViewInfo(BarManager manager, BarDrawParameters parameters, ControlForm controlForm) : base(manager, parameters, controlForm) {
		}
		public override TitleBarEl CreateTitleBarInstance() { return new FloatingBarTitleBarOffice2000El(ControlForm.Manager); }
		protected override Size CalcBorderSize() { return new Size(3, 3); }
		protected override int CalcContentIndent(BarIndent indent) {
			return 2;
		}
	}
	public class FloatingBarTitleBarOffice2000El : FloatingBarTitleBarEl {
		protected override Size ButtonImageSize { get { return new Size(11, 13); } }
		public FloatingBarTitleBarOffice2000El(BarManager manager) : base(manager) {
			Border.Border = BarItemBorderStyle.None;
			Font = new Font(Control.DefaultFont, FontStyle.Bold);
			SelectedForeColor = ForeColor = SystemColors.HighlightText;
			SelectedBackColor = BackColor = SystemColors.Highlight;
			BarControl.DefaultLinkState = BarLinkState.Highlighted;
		}
		protected override void UpdateBrush() {
			CurrentBackColor = SystemColors.Control;
		}
	}
}
