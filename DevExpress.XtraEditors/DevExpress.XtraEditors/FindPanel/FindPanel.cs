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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
namespace DevExpress.Utils {
	[ToolboxItem(false)]
	public partial class FindPanel : XtraUserControl {
		const int BorderWidth = 1;
		readonly Size DefaultButtonSize = new Size(65, 20), DefaultCloseButtonSize = new Size(16, 16);
		FindPanelOwnerBase owner;
		int lockUpdateFindFilter = 0;
		bool allowAutoApply = true;
		public FindPanel(FindPanelOwnerBase owner) {
			this.Visible = false;
			InitializeComponent();			
			this.owner = owner;
			Owner.FindPanel = this;
			LookAndFeel.StyleChanged += (s, e) => UpdateBackColor();
			HandleCreated += (s, e) =>
			{
				int requiredHeight = teFind.Height + tableLayoutPanel1.Padding.Vertical + Padding.Vertical;
				if(Height < requiredHeight)
					Height = requiredHeight;
			};
			UpdateBackColor();
		}
		public FindPanelOwnerBase Owner { get { return owner; } }
		public bool AllowAutoApply {
			get { return allowAutoApply; }
			set { allowAutoApply = value; }
		}
		public SimpleButton FindButton { get { return btnFind; } }
		public SimpleButton ClearButton { get { return btnClear; } }
		public SimpleButton CloseButton { get { return btnClose; } }
		public MRUEdit FindEdit { get { return teFind; } }
		public void AdjustButtonSize(SimpleButton button) {
			AdjustButtonSizeCore(button, DefaultButtonSize);
		}
		public void AdjustCloseButtonSize() {
			AdjustButtonSizeCore(CloseButton, DefaultCloseButtonSize);
		}
		protected void AdjustButtonSizeCore(SimpleButton button, Size defaultSize) {
			Size size = button.CalcBestSize();
			button.Size = new Size(Math.Max(defaultSize.Width, size.Width), Math.Max(defaultSize.Height, size.Height));
		}
		void UpdateBackColor() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = GetElement();
				if(element != null && element.Color.BackColor2 != Color.Empty) {
					BackColor = element.Color.GetBackColor2();
					return;
				}
			}
			BackColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
		}
		void StartUpdateTimer() {
			updateTimer.Stop();
			updateTimer.Enabled = true;
			Owner.UpdateTimer(updateTimer);
			updateTimer.Start();
		}
		void OnUpdateTimerTick(object sender, EventArgs e) {
			updateTimer.Stop();
			if(!Visible)
				return;
			ApplyFindFilter(false);
		}
		void btnClose_Click(object sender, EventArgs e) {
			Owner.HideFindPanel();
		}
		void btnFind_Click(object sender, EventArgs e) {
			ApplyFindFilter(true);
			Owner.FocusFindEditor();
		}
		void btnClear_Click(object sender, EventArgs e) {
			teFind.Text = string.Empty;
			ApplyFindFilter(false);
			Owner.FocusFindEditor();
		}
		void teFind_EditValueChanged(object sender, EventArgs e) {
			if(AllowAutoApply)
				StartUpdateTimer();
		}
		bool editorFocusing = false;
		void teFind_Enter(object sender, EventArgs e) {
			if(IsHandleCreated && !editorFocusing) {
				editorFocusing = true;
				BeginInvoke(new MethodInvoker(() =>
				{
					Owner.FocusFindEditor();
					editorFocusing = false;
				}));
			}
		}
		void teFind_KeyDown(object sender, KeyEventArgs e) {
			if(teFind.IsPopupOpen) return;
			if(e.KeyData == Keys.Enter) {
				e.Handled = true;
				ApplyFindFilter(true);
			}
			if(e.KeyData == Keys.Escape) {
				if(teFind.Text == string.Empty) {
					Owner.HideFindPanel();
					return;
				}
				teFind.Text = "";
				ApplyFindFilter(false);
			}
			if(e.KeyData == Keys.Down) {
				Owner.FocusOwner();
				e.Handled = true;
			}
		}
		void ApplyFindFilter(bool addToMru) {
			ApplyFindFilter(FindEdit.Text, addToMru);
		}
		void ApplyFindFilter(string text, bool addToMru) {
			if(lockUpdateFindFilter != 0)
				return;
			if(Owner.ApplyFindFilter(text))
				return;
			if(addToMru && !string.IsNullOrEmpty(text))
				teFind.Properties.Items.Add(text);
		}
		public void SetFilterText(string filter) {
			lockUpdateFindFilter++;
			try {
				teFind.Text = filter;
			}
			finally {
				lockUpdateFindFilter--;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e))
				DrawBorder(cache);
		}
		SkinElement GetElement() {
			return GridSkins.GetSkin(LookAndFeel)[GridSkins.SkinGridGroupPanel];
		}
		Color GetBorderColor() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = GetElement();
				if(element != null)
					return element.Properties.GetColor(GridSkins.OptFindPanelBorder);
			}
			return Color.Empty;
		}
		void DrawBorder(GraphicsCache cache) {
			Color borderColor = GetBorderColor();
			if(borderColor != Color.Empty) {
				AppearanceObject border = new AppearanceObject() { BackColor = borderColor };
				border.FillRectangle(cache, new Rectangle(ClientRectangle.X, ClientRectangle.Bottom - BorderWidth, ClientRectangle.Width, BorderWidth));
			}
		}
	}
	[ToolboxItem(false)]
	public class RTLPanel : Panel {	   
		protected override CreateParams CreateParams {
			get {
				CreateParams CP = base.CreateParams;
				if(RightToLeft == RightToLeft.Yes)
					CP.ExStyle = (CP.ExStyle | 0x400000 | 0x100000);
				return CP;
			}
		}
	}	
}
