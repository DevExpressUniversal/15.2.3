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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard {
	class ChartUserControl: XtraUserControl {
		private void InitializeComponent() {
			this.SuspendLayout();
			this.Name = "ChartUserControl";
			this.ResumeLayout(false);
		}
	}
	class ChartPanelControl : Panel, ISupportInitialize {
		UserLookAndFeel lookAndFeel;
		AppearanceObject appearance = new AppearanceObject();
		public new BorderStyles BorderStyle {
			get { return BorderStyles.NoBorder; }
			set { }
		}
		public UserLookAndFeel LookAndFeel {
			get {
				if (lookAndFeel == null)
					lookAndFeel = new UserLookAndFeel(this);
				return lookAndFeel;
			}
		}
		public AppearanceObject Appearance {
			get { return appearance; }
		}
		public ChartPanelControl() {
			BackColor = Color.Transparent;
			base.BorderStyle = System.Windows.Forms.BorderStyle.None;
		}
		public virtual void BeginInit() {
		}
		public virtual void EndInit() {
		}
	}
	class ChartPreviewPanel : PanelControl {
		protected override bool IsDrawNoBorder { get { return false; } }
		public new BorderStyles BorderStyle {
			get { return BorderStyles.NoBorder; }
			set { }
		}
		public ChartPreviewPanel() {
			base.BorderStyle = BorderStyles.NoBorder;
		}
	}
	class HitTestTransparentPanelControl : Panel {
		const int WM_NCHITTEST = 0x84;
		const int HT_TRANSPARENT = -1;
		protected override void WndProc(ref Message m) {
			if (m.Msg == WM_NCHITTEST)
				m.Result = new IntPtr(HT_TRANSPARENT);
			else
				base.WndProc(ref m);
		}
	}
}
