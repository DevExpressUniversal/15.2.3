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
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.XtraGrid.Views.Card {
	public class CardQuickCustomizationForm : BaseViewPopupActivator {
		const int QuickCustomizeWidth = 200, FormHeight = 200;
		ArrayList editors;
		PopupContainerControl containerControl;
		static Size imageSize = new Size(11, 12);
		const int imageSortAsc = 1, imageSortDesc = 0, imageNonSort = 2;
		public CardQuickCustomizationForm(CardView view) : base(view) {
			this.editors = new ArrayList();
			this.containerControl = new PopupContainerControl();
			this.containerControl.AutoScroll = true;
		}
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			RepositoryItemPopupContainerEdit containerEdit;
			containerEdit = new ViewRepositoryItemPopupContainerEdit(this);
			containerEdit.PopupSizeable = false;
			containerEdit.PopupControl = this.containerControl;
			containerEdit.CloseOnLostFocus = false;
			containerEdit.CloseOnOuterMouseClick = false;
			return containerEdit;
		}
		[ThreadStatic]
		static ImageList images = null;
		static ImageList Images {
			get {
				if(images == null) {
					images = DevExpress.Utils.ResourceImageHelper.CreateImageListFromResources("DevExpress.XtraGrid.Images.CardCaption.bmp", typeof(CardQuickCustomizationForm).Assembly, imageSize);
				}
				return images;
			}
		}
		public override void Dispose() {
			base.Dispose();
			this.editors.Clear();
			this.containerControl.Dispose();
			View.DestroyFilterPopup();
		}
		protected override void OnActivator_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e) {
			View.DestroyCustomizationForm();
			base.OnActivator_CloseUp(sender, e);
		}
		protected ArrayList Editors { get { return editors; } }
		public PopupContainerControl ContainerControl { get { return containerControl; } }
		public new CardView View { get { return base.View as CardView; } }
		public virtual void CreateColumns() {
			foreach(GridColumn column in View.Columns) {
				if(column.OptionsColumn.ShowInCustomizationForm) CreateColumnInfo(column);
			}
			if(Editors.Count > 0) {
				int maxBottom = (Editors[Editors.Count - 1] as BaseEdit).Bottom;
				int bottom = Math.Min(maxBottom, FormHeight);
				ContainerControl.Height = bottom;
				for(int n = 0; n < Editors.Count; n++) {
					BaseEdit edit = Editors[n] as BaseEdit;
					edit.Width = ContainerControl.DisplayRectangle.Width - (maxBottom > FormHeight ? SystemInformation.VerticalScrollBarWidth : 0);
				}
			}
		}
		public override bool CanShow { get { return Editors.Count > 0; } }
		protected virtual ButtonEdit CreateColumnInfo(GridColumn column) {
			ButtonEdit be = new ButtonEdit();
			be.Text = column.GetTextCaption();
			be.LookAndFeel.Assign(View.ElementsLookAndFeel);
			be.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			be.Properties.Buttons.Clear();
			be.Properties.Buttons.Add(CreateEditorVisibleButton(be, column.Visible));
			be.Properties.Buttons.Add(CreateEditorButton(GridLocalizer.Active.GetLocalizedString(GridStringId.CardViewQuickCustomizationButtonSort), GetSortImage(column.SortOrder), false));
			be.Properties.Buttons.Add(CreateEditorButton(GridLocalizer.Active.GetLocalizedString(GridStringId.CardViewQuickCustomizationButtonFilter), -1, false));
			be.Tag = column;
			UpdateEditor(be, column);
			be.ButtonClick += new ButtonPressedEventHandler(OnCustomizeButtonPressed);
			be.Width = QuickCustomizeWidth;
			AddToControl(be);
			Editors.Add(be);
			return be;
		}
		void AddToControl(BaseEdit be) {
			int lastTop = ContainerControl.Controls.Count == 0 ? 0 : ContainerControl.Controls[ContainerControl.Controls.Count - 1].Bottom;
			ContainerControl.Controls.Add(be);
			be.Top = lastTop > 0 ? lastTop + 1 : 0;
		}
		public virtual void UpdateColumns() {
			foreach(ButtonEdit edit in Editors) {
				UpdateEditor(edit, edit.Tag as GridColumn);
			}
		}
		protected virtual void UpdateEditor(ButtonEdit edit, GridColumn column) {
			edit.Properties.BeginUpdate();
			try {
				edit.Text = column.GetCustomizationCaption();
				edit.Properties.Buttons[0].Kind = column.Visible ? ButtonPredefines.OK : ButtonPredefines.Glyph;
				edit.Properties.Buttons[1].Image = GetButtonImage(GetSortImage(column.SortOrder));
				edit.Properties.Buttons[1].Enabled = View.CanSortColumn(column);
				FontStyle fs = IsFiltered(column) ? FontStyle.Underline : FontStyle.Regular;
				if(edit.Properties.Buttons[2].Appearance.Font.Style != fs)
					edit.Properties.Buttons[2].Appearance.Font = new Font(edit.Properties.Buttons[2].Appearance.Font, fs);
				edit.Properties.Buttons[2].Enabled = View.IsColumnAllowFilter(column);
			} finally {
				edit.Properties.EndUpdate();
			}
		}
		protected virtual void OnCustomizeButtonPressed(object sender, ButtonPressedEventArgs e) {
			ButtonEdit be = sender as ButtonEdit;
			GridColumn column = be.Tag as GridColumn;
			int index = be.Properties.Buttons.IndexOf(e.Button);
			switch(index) {
				case 0 : 
					column.Visible = !column.Visible; break;
				case 1 : 
					OnSortButtonPressed(column);
					break;
				case 2 : 
					OnFilterButtonPressed(be, column);
					break;
			}
			UpdateEditor(be, column);
		}
		protected virtual void OnFilterButtonPressed(BaseEdit be, GridColumn column) {
			Rectangle bounds = be.ClientRectangle; 
			View.ShowFilterPopup(column, bounds, be, this);
		}
		protected virtual void OnSortButtonPressed(GridColumn column) {
			View.DoMouseSortColumn(column, Control.ModifierKeys);
			UpdateColumns();
		}
		protected virtual bool IsFiltered(GridColumn column) { return column.FilterInfo.Type != ColumnFilterType.None; }
		EditorButton CreateEditorButton(string caption, int imageIndex, bool isLeft) {
			EditorButton button = new EditorButton(ButtonPredefines.Glyph);
			button.Caption = caption;
			button.IsLeft = isLeft;
			button.Image = imageIndex >= 0 ? GetButtonImage(imageIndex) : null;
			if(button.Image != null && button.Caption.Length > 0)
				button.ImageLocation = ImageLocation.Default;
			return button;
		}
		EditorButton CreateEditorVisibleButton(ButtonEdit be, bool visible) {
			EditorButtonPainter painter = EditorButtonHelper.GetPainter(BorderStyles.Default, be.LookAndFeel);
			EditorButton button = new EditorButton(ButtonPredefines.OK);
			Size size = painter.CalcKindSize(new EditorButtonObjectInfoArgs(button, null));
			button.Image = new Bitmap(size.Width, 1);
			button.IsLeft = true;
			button.Kind = visible ? ButtonPredefines.OK : ButtonPredefines.Glyph;
			return button;
		}
		protected virtual int GetSortImage(ColumnSortOrder order) {
			switch(order) {
				case ColumnSortOrder.Ascending : return imageSortAsc;
				case ColumnSortOrder.Descending : return imageSortDesc;
			}
			return imageNonSort;
		}
		protected virtual Image GetButtonImage(int index) { 
			if(View.ElementsLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				ImageCollection collection = GridSkins.GetSkin(View.ElementsLookAndFeel)[GridSkins.SkinSortShape].Image.GetImages();
				if(index == 2) {
					return new Bitmap(collection.ImageSize.Width, collection.ImageSize.Height);
				}
				return collection.Images[index];
			}
			return Images.Images[index]; 
		}
		ButtonEdit GetEdit(GridColumn column) {
			foreach(ButtonEdit edit in Editors) {
				if(edit.Tag == column) return edit;
			}
			return null;
		}
		public class ViewRepositoryItemPopupContainerEdit : RepositoryItemPopupContainerEdit {
			CardQuickCustomizationForm customizationForm;
			public ViewRepositoryItemPopupContainerEdit(CardQuickCustomizationForm customizationForm) {
				this.customizationForm = customizationForm;
				LookAndFeel.Assign(this.customizationForm.View.LookAndFeel);
			}
			public override BaseEdit CreateEditor() {
				return new ViewPopupContainerEdit(this.customizationForm);
			}
		}
		[ToolboxItem(false)]
		public class ViewPopupContainerEdit : PopupContainerEdit {
			CardQuickCustomizationForm customizationForm = null;
			public ViewPopupContainerEdit() { }
			public ViewPopupContainerEdit(CardQuickCustomizationForm customizationForm) {
				this.customizationForm = customizationForm;
			}
			public ColumnView View { get { return customizationForm != null ? customizationForm.View : null; } }
			public override bool EditorContainsFocus {
				get {
					if(base.EditorContainsFocus) return true;
					if(View != null && View.FilterPopup != null) {
						return true;
					}
					return false;
				}
			}
		}
	}
}
