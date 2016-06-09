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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Localization;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Skins;
namespace DevExpress.XtraGrid.Controls {
	[ToolboxItem(false)]
	public partial class FindControl : XtraUserControl {
		ColumnView view;
		int lockUpdate = 0;
		bool allowAutoApply = true;
		public FindControl() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			InitializeComponent();
			WorkspaceManager.SetSerializationEnabled(layoutControl1, false);
			layoutControl1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			layoutControl1.HandleCreated += (s, e) =>
			{
				UpdateHeight(true);
			};
		}
		protected void UpdateHeight(bool onlyGrow) {
			if(onlyGrow) {
				if(Height < layoutControl1.Root.MinSize.Height + Padding.Bottom) Height = layoutControl1.Root.MinSize.Height + Padding.Bottom;
			}
			else {
				Height = layoutControl1.Root.MinSize.Height + Padding.Bottom;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			Draw(e);
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			Draw(e);
		}
		class GridGroupPanelPainterEx : ObjectPainter {
			UserLookAndFeel lookAndFeel;
			public GridGroupPanelPainterEx(UserLookAndFeel lookAndFeel) {
				this.lookAndFeel = lookAndFeel;
			}
			public override void DrawObject(ObjectInfoArgs e) {
				if(lookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					DrawNonSkin(e);
				else
					DrawSkin(e);
			}
			void DrawNonSkin(ObjectInfoArgs e) {
				e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
			}
			void DrawSkin(ObjectInfoArgs e) {
				SkinElement element = GridSkins.GetSkin(lookAndFeel)[GridSkins.SkinGridGroupPanel];
				AppearanceObject border = new FrozenAppearance();
				AppearanceObject content = new FrozenAppearance();
				content.BackColor = border.BackColor = LookAndFeelHelper.GetSystemColor(lookAndFeel, SystemColors.Control);
				if(element.Color.BackColor2 != Color.Empty) {
					Color backColor2 = element.Color.GetBackColor2();
					content.BackColor = backColor2;
					content.BackColor2 = backColor2;
					border.BackColor = content.BackColor;
				}
				Color borderColor = element.Properties.GetColor(GridSkins.OptFindPanelBorder);
				if(borderColor != Color.Empty)
					border.BackColor = borderColor;
				Rectangle bounds = e.Bounds;
				content.FillRectangle(e.Cache, bounds);
				bounds.Height = 1;
				bounds.Y = e.Bounds.Bottom - 1;
				border.FillRectangle(e.Cache, bounds);
			}
		}
		void Draw(PaintEventArgs e) {
			GridGroupPanelPainterEx painter = new GridGroupPanelPainterEx(LookAndFeel);
			StyleObjectInfoArgs se = new StyleObjectInfoArgs(new GraphicsCache(e));
			se.Bounds = ClientRectangle;
			painter.DrawObject(se);
		}
		[DefaultValue(true)]
		public bool AllowAutoApply {
			get { return allowAutoApply; }
			set { allowAutoApply = value; }
		}
		FindMode GetFindMode() {
			if(View == null) return FindMode.FindClick;
			if(View.OptionsFind.FindMode == FindMode.Default) {
				if((!View.IsServerMode && View.DataController.ListSourceRowCount < 10000) || View.IsAsyncServerMode)
					return FindMode.Always;
				return FindMode.FindClick;
			}
			return View.OptionsFind.FindMode;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected ColumnView View { get { return view; } }
		public FindControl(ColumnView view, object properties) : this() {
			this.view = view;
			AllowAutoApply = GetFindMode() == FindMode.Always;
			if(View.OptionsFind.AlwaysVisible || !View.OptionsFind.ShowCloseButton)
				lciCloseButton.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			LookAndFeel.ParentLookAndFeel = View.ElementsLookAndFeel;
			LookAndFeel.StyleChanged += new EventHandler(LookAndFeel_StyleChanged);
			teFind.MenuManager = View.GridControl.MenuManager;
			teFind.MouseWheel += new MouseEventHandler(teFind_MouseWheel);
			teFind.Enter += new EventHandler(teFind_Enter);
			teFind.Properties.NullValuePromptShowForEmptyValue = true;
			teFind.Properties.NullValuePrompt = view.OptionsFind.FindNullPrompt;
			teFind.Properties.ShowNullValuePromptWhenFocused = true;
			BackColor = GetBackColor();
			if(btFind.Text == "Find") btFind.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.FindControlFindButton);
			if(btClear.Text == "Clear") btClear.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.FindControlClearButton);
			if(!View.OptionsFind.ShowFindButton)
				this.lciFindButton.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			if(!View.OptionsFind.ShowClearButton)
				this.lciClearButton.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			if(view.IsDesignMode) {
				teFind.Enabled = btClear.Enabled = btFind.Enabled = false;
			}
			Padding = new Padding(0, 0, 0, 1);
			RestoreProperties(properties);
		}
		Color GetBackColor() {
			if(View.ElementsLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = GridSkins.GetSkin(View.ElementsLookAndFeel)[GridSkins.SkinGridGroupPanel];
				if(element.Color.BackColor2 != Color.Empty)
					return element.Color.GetBackColor2();
			}
			return LookAndFeelHelper.GetSystemColor(View.ElementsLookAndFeel, SystemColors.Control);
		}
		bool editorFocusing = false;
		void teFind_Enter(object sender, EventArgs e) {
			if(IsHandleCreated && !editorFocusing) {
				editorFocusing = true;
				BeginInvoke(new MethodInvoker(delegate() {
					if(View != null) {
						if(SaveEditingValue()) {
					if(!teFind.Focused) teFind.Focus();
						}
					}
					editorFocusing = false;
				}));
			}
		}
		bool SaveEditingValue() {
			if(View == null) return true;
			ColumnView view = View;
			if(View.GridControl != null && View.GridControl.FocusedView != null) view = (View.GridControl.FocusedView as ColumnView) ?? View;
			if(view.ActiveEditor != null) {
				GridEditorContainerHelper editorHelper = view.GridControl.EditorHelper;
				editorHelper.BeginAllowHideException();
				try {
					if(!view.ValidateEditor()) return false;
					view.CloseEditor();
					if(view.ActiveEditor != null) return false;
					if(!view.UpdateCurrentRow()) return false;
				}
				catch(HideException) {
					view.GridControl.Focus();
					return false;
				}
				finally {
					editorHelper.EndAllowHideException();
				}
			}
			return true;
		}
		void teFind_MouseWheel(object sender, MouseEventArgs e) {
			if(!teFind.IsPopupOpen) {
				XtraForm.ProcessSmartMouseWheel(this, e);
				DXMouseEventArgs.GetMouseArgs(e).Handled = true;
			}
		}
		public void FocusFindEdit() {
			if(!SaveEditingValue()) return;
			if(View.IsFinalizingSerialization) return;
			teFind.Focus();
		}
		void RestoreProperties(object properties) {
			if(properties == null) return;
			lockUpdate++;
			teFind.Properties.Items.AddRange(properties as ICollection);
			lockUpdate--;
		}
		internal object SaveProperties() {
			ArrayList mru = new ArrayList();
			mru.AddRange(teFind.Properties.Items);
			return mru;
		}
		bool touchUIMode = false;
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			if(IsHandleCreated)
				BeginInvoke(new MethodInvoker(CheckUpdatePanelHeightByTouchUIMode));
			BackColor = GetBackColor();
		}
		void CheckUpdatePanelHeightByTouchUIMode() {
			bool currentTouchUI = this.layoutControl1.LookAndFeel.GetTouchUI();
			if(touchUIMode != currentTouchUI) {
				touchUIMode = currentTouchUI;
				int prevHeight = Height;
				UpdateHeight(false);
				if(Height != prevHeight)
					View.LayoutChanged();
			}
		}
		void btFind_Click(object sender, EventArgs e) {
			ApplyFilter(true);
			FocusFindEdit();
		}
		void btClear_Click(object sender, EventArgs e) {
			if(!SaveEditingValue()) return;
			teFind.Text = string.Empty;
			ApplyFilter(false);
			FocusFindEdit();
		}
		void ApplyFilter(bool addToMru) {
			ApplyFilter(teFind.Text, addToMru);
		}
		void ApplyFilter(string text, bool addToMru) {
			if(lockUpdate != 0) return;
			View.ApplyFindFilter(text);
			if(addToMru && !string.IsNullOrEmpty(text)) {
				if(View.IsAsyncInProgress || View.DataRowCount > 0)
					teFind.Properties.Items.Add(text);
			}
		}
		private void teFind_EditValueChanged(object sender, EventArgs e) {
			if(AllowAutoApply) {
				StartAutoFilterTimer();
			}
		}
		void StartAutoFilterTimer() {
			autoFilterTimer.Stop();
			autoFilterTimer.Enabled = true;
			autoFilterTimer.Interval = View.OptionsFind.FindDelay;
			autoFilterTimer.Start();
		}
		private void btClose_Click(object sender, EventArgs e) {
			HideFindControl();
		}
		internal void SetFilterText(string filter) {
			lockUpdate++;
			teFind.Text = filter;
			lockUpdate--;
		}
		void teFind_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(teFind.IsPopupOpen) return;
			if(e.KeyData == Keys.Enter) {
				e.Handled = true;
				ApplyFilter(true);
			}
			if(e.KeyData == Keys.Down) {
				View.FocusedRowHandle = View.GetVisibleRowHandle(0);
				if(View.GridControl != null) View.GridControl.Focus();
				e.Handled = true;
			}
			if(e.KeyData == Keys.Escape) {
				if(teFind.Text == string.Empty) {
					HideFindControl();
					return;
				}
				teFind.Text = "";
				ApplyFilter(false);
			}
		}
		void HideFindControl() {
			if(View.OptionsFind.AlwaysVisible) return;
			View.HideFindPanel();
		}
		void autoFilterTimer_Tick(object sender, EventArgs e) {
			autoFilterTimer.Stop();
			if(!Visible || View.FindFilterText == teFind.Text) return;
			ApplyFilter(false);
		}
		public SimpleButton FindButton { get { return btFind; } }
		public SimpleButton ClearButton { get { return btClear; } }
		public MRUEdit FindEdit { get { return teFind; } }
		public void CalcButtonsBestFit() {
			Size size = btClear.CalcBestSize();
			if(btClear.MaximumSize.Width < size.Width) {
				lciClearButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
				btClear.MaximumSize = new Size(size.Width, btClear.MaximumSize.Height);
				lciClearButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Default;
			}
			size = btFind.CalcBestSize();
			if(btFind.MaximumSize.Width < size.Width) {
				lciFindButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
				btFind.MaximumSize = new Size(size.Width, btFind.MaximumSize.Height);
				lciFindButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Default;
			}
		}
	}
}
