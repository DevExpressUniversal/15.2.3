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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraDiagram.Bars {
	[ToolboxItem(false)]
	public abstract class DiagramColorChangeItemBase : DiagramCommandBarButtonItem, IPopupColorPickEdit {
		PopupColorBuilder popupBuilder;
		PopupControlContainer popup;
		RepositoryItemColorPickEdit properties;
		static readonly Rectangle smallGlyphColorRect = new Rectangle(1, 13, 14, 3);
		static readonly Rectangle largeGlyphColorRect = new Rectangle(1, 26, 30, 5);
		public DiagramColorChangeItemBase() {
			this.properties = CreateRepositoryItem();
			this.popupBuilder = CreatePopupBuilder();
			this.popup = CreatePopupControlContainer();
			this.popup.Popup += OnPopup;
			ButtonStyle = BarButtonStyle.DropDown;
			DropDownControl = Popup;
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
			Popup.Controls.Add(popupBuilder.TabControl);
		}
		protected RepositoryItemColorPickEdit CreateRepositoryItem() {
			RepositoryItemColorPickEdit properties = CreateRepositoryItemInstance();
			InitializeRepositoryItem(properties);
			return properties;
		}
		protected virtual void InitializeRepositoryItem(RepositoryItemColorPickEdit properties) {
			properties.AutomaticColor = Color.Empty;
			properties.AutomaticBorderColor = Color.Black;
		}
		protected virtual RepositoryItemColorPickEdit CreateRepositoryItemInstance() {
			return new RepositoryItemColorPickEdit();
		}
		protected virtual PopupColorBuilder CreatePopupBuilder() {
			return new DiagramChangeColorItemPopupBuilder(this);
		}
		protected virtual PopupControlContainer CreatePopupControlContainer() {
			return new PopupControlContainer();
		}
		Color color = Color.Empty;
		public Color Color {
			get { return color; }
			set {
				if(Color == value)
					return;
				color = value;
				OnColorChanged();
			}
		}
		protected virtual void OnColorChanged() {
			OnItemChanged();
			DrawColorRectangle();
		}
		protected override void UpdateItemGlyphs() {
			base.UpdateItemGlyphs();
			DrawColorRectangle();
		}
		protected virtual Rectangle SmallGlyphColorRect { get { return smallGlyphColorRect; } }
		protected virtual Rectangle LargeGlyphColorRect { get { return largeGlyphColorRect; } }
		protected virtual void DrawColorRectangle() {
			if(IsImageExist)
				DrawColorRectangle(Glyph, SmallGlyphColorRect);
			if(IsLargeImageExist)
				DrawColorRectangle(LargeGlyph, LargeGlyphColorRect);
		}
		protected virtual void DrawColorRectangle(Image image, Rectangle rect) {
			Color color = Color != Color.Empty ? Color : Color.Black;
			using(Graphics g = Graphics.FromImage(image)) {
				using(SolidBrush brush = new SolidBrush(color)) {
					g.FillRectangle(brush, rect);
				}
			}
		}
		protected virtual void OnPopup(object sender, EventArgs e) {
			UpdatePopupSize();
		}
		protected void UpdatePopupSize() {
			Popup.Size = popupBuilder.CalcContentSize();
		}
		protected override void InvokeCommand() {
			try {
				if(Control == null) return;
				Command command = CreateCommand();
				if(command != null) {
					ICommandUIState state = CreateCommandUIState(command);
					if(command.CanExecute())
						command.ForceExecute(state);
				}
				UpdateItemVisibility();
				CloseAllRibbonPopups();
				Control.Focus();
			}
			catch(Exception e) {
				if(!HandleException(e)) throw;
			}
		}
		protected void CloseAllRibbonPopups() {
			if(Ribbon != null && Ribbon.Manager != null) Ribbon.Manager.ClosePopupForms(false);
		}
		protected virtual ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<Color> value = new DefaultValueBasedCommandUIState<Color>();
			value.Value = Color;
			return value;
		}
		protected override ICommandUIState CreateButtonItemUIState() {
			return new ColorChangeItemUIState(this);
		}
		protected virtual void DoClosePopup() {
			if(Popup != null) Popup.HidePopup();
		}
		protected PopupControlContainer Popup { get { return popup; } }
		protected virtual void OnResultColorChanged() {
			Color = GetResultColor();
			InvokeCommand();
		}
		protected virtual Color GetResultColor() {
			return (Color)popupBuilder.ResultValue;
		}
		protected virtual ColorEditTabControlBase CreateTabControl() {
			return new PopupColorPickEditForm.ColorPickEditTabControl(this);
		}
		protected virtual ColorListBox CreateColorListBox() {
			return new ColorListBox();
		}
		#region IPopupColorPickEdit
		void IPopupColorPickEdit.ClosePopup(PopupCloseMode mode) {
			DoClosePopup();
		}
		void IPopupColorPickEdit.SetSelectedColorItem(ColorItem colorItem) { }
		void IPopupColorEdit.ClosePopup() {
			DoClosePopup();
		}
		void IPopupColorEdit.OnColorChanged() { OnResultColorChanged(); }
		ColorPickEditBase IPopupColorPickEdit.OwnerEdit { get { return null; } }
		ColorEditTabControlBase IPopupColorEdit.CreateTabControl() {
			return CreateTabControl();
		}
		Color IPopupColorEdit.Color { get { return GetResultColor(); } }
		RepositoryItemColorEdit IPopupColorEdit.Properties { get { return this.properties; } }
		ColorListBox IPopupColorEdit.CreateColorListBox() {
			return CreateColorListBox();
		}
		bool IPopupColorPickEdit.HasBorder { get { return false; } }
		UserLookAndFeel IPopupColorEdit.LookAndFeel {
			get { return Manager != null ? Manager.GetController().LookAndFeel : null; }
		}
		bool IPopupColorEdit.IsPopupOpen { get { return true; } }
		object IPopupColorEdit.EditValue { get { return Color; } }
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.popup != null) {
					this.popup.Popup -= OnPopup;
					this.popup.Dispose();
				}
				this.popup = null;
				if(this.properties != null) {
					this.properties.Dispose();
				}
				this.properties = null;
			}
			this.popupBuilder = null;
			base.Dispose(disposing);
		}
	}
	public class DiagramChangeColorItemPopupBuilder : PopupColorBuilderEx {
		public DiagramChangeColorItemPopupBuilder(IPopupColorPickEdit owner)
			: base(owner) {
		}
		public override Size CalcContentSize() {
			return Size.Add(base.CalcContentSize(), BorderSize);
		}
		protected virtual Size BorderSize { get { return new Size(2, 2); } }
		protected override ColorEditTabControlBase CreateTabControl() {
			ColorEditTabControlBase tab = base.CreateTabControl();
			tab.Dock = DockStyle.Fill;
			return tab;
		}
	}
	public class ColorChangeItemUIState : BarButtonItemUIState {
		readonly DiagramColorChangeItemBase item;
		public ColorChangeItemUIState(DiagramColorChangeItemBase item) : base(item) {
			this.item = item;
		}
		public virtual Color Color {
			get { return item.Color; }
			set { item.Color = value; }
		}
	}
}
