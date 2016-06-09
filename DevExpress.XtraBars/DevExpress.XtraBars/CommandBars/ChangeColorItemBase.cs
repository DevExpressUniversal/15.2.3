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
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.Office.UI.Internal;
using DevExpress.XtraRichEdit.Design.Internal;
namespace DevExpress.Office.UI {
	#region ChangeColorItemBase (abstract class)
	public abstract class ChangeColorItemBase<TControl, TCommandId> : ControlCommandBarButtonItem<TControl, TCommandId>, IOfficePopupColorEdit, IPopupColorBuilderProvider
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		static readonly Color defaultSelectedColor = Color.Empty;
		#region Fields
		Color color = defaultSelectedColor;
		PopupControlContainer container;
		PopupColorBuilder popupColorBuilder;
		RepositoryItemColorEdit properties;
		Rectangle smallGlyphColorRect = new Rectangle(1, 13, 14, 3);
		Rectangle largeGlyphColorRect = new Rectangle(1, 26, 30, 5);
		#endregion
		protected ChangeColorItemBase() {
		}
		protected ChangeColorItemBase(BarManager manager)
			: base(manager) {
		}
		protected ChangeColorItemBase(string caption)
			: base(caption) {
		}
		protected ChangeColorItemBase(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return container; } set { } }
		public virtual RepositoryItemColorEdit Properties { get { return properties; } }
		protected Color Color { get { return color; } }
		public Color SelectedColor {
			get { return color; }
			set {
				if (color == value)
					return;
				color = value;
				DrawColorRectangle();
			}
		}
		bool ShouldSerializeSelectedColor() {
			return SelectedColor != defaultSelectedColor;
		}
		void ResetSelectedColor() {
			SelectedColor = defaultSelectedColor;
		}
		protected virtual Rectangle SmallGlyphColorRect { get { return smallGlyphColorRect; } }
		protected virtual Rectangle LargeGlyphColorRect { get { return largeGlyphColorRect; } }
		PopupColorBuilder IPopupColorBuilderProvider.PopupColorBuilder { get { return popupColorBuilder; } }
		protected internal override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		protected virtual bool UseColorPickEdit { get { return true; } }
		protected abstract string DefaultColorButtonCaption { get; }
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				UserLookAndFeel.Default.StyleChanged -= OnDefaultStyleChanged;
				if (container != null) {
					this.container.LostFocus -= OnContainerLostFocus;
					this.container.Popup -= OnContainerPopup;
					container.Dispose();
					container = null;
				}
				if (properties != null) {
					properties.Dispose();
					properties = null;
				}
				if (popupColorBuilder != null) {
					IDisposable disposablePopupColorBuilder = popupColorBuilder as IDisposable;
					if (disposablePopupColorBuilder != null) {
						disposablePopupColorBuilder.Dispose();
					}
					popupColorBuilder = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override Keys GetDefaultShortcutKeys() {
			return Keys.None;
		}
		protected internal override void Initialize() {
			base.Initialize();
			if (UseColorPickEdit) {
				RepositoryItemOfficeColorPickEdit pickEdit = new RepositoryItemOfficeColorPickEdit();
				if (!String.IsNullOrEmpty(DefaultColorButtonCaption))
					pickEdit.AutomaticColorButtonCaption = DefaultColorButtonCaption;
				this.properties = pickEdit;
				this.popupColorBuilder = new OfficeColorPickEditPopupColorBuilder(this);
			}
			else {
				this.properties = new RepositoryItemOfficeColorEdit();
				this.popupColorBuilder = new OfficePopupColorBuilder(this);
			}
			InitializePopupControl();
		}
		protected virtual void InitializePopupControl() {
			this.container = new ChangeColorItemPopupControlContainer(this);
			this.container.LostFocus += OnContainerLostFocus;
			this.container.Popup += OnContainerPopup;
			UserLookAndFeel.Default.StyleChanged += OnDefaultStyleChanged;
			this.container.Size = this.popupColorBuilder.CalcContentSize();
			this.container.Controls.Add(this.popupColorBuilder.EmbeddedControl);
		}
		void OnDefaultStyleChanged(object sender, EventArgs e) {
			RefreshContainerLookAndFeel();
		}
		void OnContainerPopup(object sender, EventArgs e) {
			RefreshContainer();
		}
		void OnContainerLostFocus(object sender, EventArgs e) {
			if (this.popupColorBuilder.LockFocus)
				this.container.CloseOnOuterMouseClick = false;
			else
				this.container.CloseOnOuterMouseClick = true;
		}
		protected virtual void RefreshContainer() {
			PopupColorBuilderEx popupBuilder = this.popupColorBuilder as PopupColorBuilderEx;
			if(popupBuilder != null)
				popupBuilder.OnBeforeShowPopup();
			Size size = this.popupColorBuilder.CalcContentSize();
			this.container.Size = size;
			SetParentSize(this.container, size);
		}
		protected virtual void RefreshContainerLookAndFeel() {
			this.container.Controls.Clear();
			this.container.Controls.Add(this.popupColorBuilder.EmbeddedControl);
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if (this.popupColorBuilder != null)
				this.popupColorBuilder.RefreshLookAndFeel();
		}
		protected internal override void UpdateItemGlyphs() {
			base.UpdateItemGlyphs();
			DrawColorRectangle();
		}
		protected virtual void SetParentSize(Control control, Size size) {
			Control parent = control.Parent;
			while (parent != null) {
				parent.Size = size;
				parent = parent.Parent;
			}
		}
		protected virtual void DrawColorRectangle(Image image, Rectangle rect) {
			Color fillColor = Color != Color.Empty ? Color : Color.Black;
			DrawColorRectangleCore(image, rect, fillColor);
		}
		protected virtual void DrawColorRectangleCore(Image image, Rectangle rect, Color color) {
			using (Graphics gr = Graphics.FromImage(image)) {
				using (SolidBrush brush = new SolidBrush(color)) {
					gr.FillRectangle(brush, rect);
				}
			}
		}
		protected virtual void OnInternalColorChanged() {
			InvokeCommandCore();
			DrawColorRectangle();
		}
		protected virtual void DrawColorRectangle() {
			if (IsImageExist)
				DrawColorRectangle(Glyph, SmallGlyphColorRect);
			if (IsLargeImageExist)
				DrawColorRectangle(LargeGlyph, LargeGlyphColorRect);
		}
		protected internal override void InvokeCommand() {
			if (!ActAsDropDown)
				InvokeCommandCore();
		}
		protected internal virtual void InvokeCommandCore() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected virtual ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<Color> value = new DefaultValueBasedCommandUIState<Color>();
			value.Value = Color;
			return value;
		}
		#region IPopupColorEdit Members
		void IPopupColorEdit.ClosePopup() {
			if (Manager != null) {
				Manager.SelectionInfo.OnCloseAll(BarMenuCloseType.AllExceptMiniToolbars | BarMenuCloseType.KeepPopupContainer);
			}
			if (Ribbon != null && Ribbon.Manager != null) {
				Ribbon.Manager.ClosePopupForms();
			}
			container.HidePopup();
		}
		Color IPopupColorEdit.Color {
			get { return color; }
		}
		object IPopupColorEdit.EditValue {
			get { return color; }
		}
		bool IPopupColorEdit.IsPopupOpen {
			get { return container.Visible; }
		}
		DevExpress.LookAndFeel.UserLookAndFeel IPopupColorEdit.LookAndFeel {
			get { return Manager != null ? Manager.GetController().LookAndFeel : null; }
		}
		RepositoryItemColorEdit IPopupColorEdit.Properties {
			get { return Properties; }
		}
		void IPopupColorEdit.OnColorChanged() {
			this.color = (Color)popupColorBuilder.ResultValue;
			OnInternalColorChanged();
		}
		ColorEditTabControlBase IPopupColorEdit.CreateTabControl() {
			return new ColorEditTabControlBase();
		}
		ColorListBox IPopupColorEdit.CreateColorListBox() {
			return new ColorListBox();
		}
		void IOfficePopupColorEdit.OnDefaultButtonClick() {
			this.color = Color.Empty;
			OnInternalColorChanged();
		}
		#endregion
		#region IPopupColorPickEdit Members
		void IPopupColorPickEdit.ClosePopup(PopupCloseMode mode) {
			((IPopupColorEdit)this).ClosePopup();
		}
		ColorPickEditBase IPopupColorPickEdit.OwnerEdit {
			get { return null; }
		}
		void IPopupColorPickEdit.SetSelectedColorItem(ColorItem colorItem) { }
		bool IPopupColorPickEdit.HasBorder { get { return false; } }
		#endregion
	}
	#endregion
}
namespace DevExpress.Office.UI.Internal {
	public interface IPopupColorBuilderProvider {
		PopupColorBuilder PopupColorBuilder { get; }
	}
	#region ChangeColorItemPopupControlContainer
	class ChangeColorItemPopupControlContainer : PopupControlContainer {
		readonly IPopupColorBuilderProvider owner;
		public ChangeColorItemPopupControlContainer(IPopupColorBuilderProvider owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public IPopupColorBuilderProvider Owner { get { return owner; } }
		protected override PopupContainerBarControl CreatePopupContainerBarControl(BarManager manager) {
			return new ChangeColorItemPopupContainerBarControl(manager, this);
		}
	}
	#endregion
	#region ChangeColorItemPopupContainerBarControl
	class ChangeColorItemPopupContainerBarControl : PopupContainerBarControl {
		public ChangeColorItemPopupContainerBarControl(BarManager manager, PopupControlContainer popup)
			: base(manager, popup) {
		}
		public override bool ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			ChangeColorItemPopupControlContainer container = PopupControl as ChangeColorItemPopupControlContainer;
			if (container != null && container.Owner != null) {
				PopupColorBuilder builder = container.Owner.PopupColorBuilder;
				if (builder != null && builder.LockFocus)
					return false;
			}
			return base.ShouldCloseOnOuterClick(control, e);
		}
		public override bool CanOpenAsChild(IPopup popup) { 
			if (popup is SubMenuBarControl)
				return false;
			return base.CanOpenAsChild(popup);
		}
	}
	#endregion
}
