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

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using System.Collections.Generic;
namespace DevExpress.Web {
	public interface IHeaderFilterPopupOwner : ISkinOwner {
		bool ShowButtonPanel { get; }
		Unit PopupHeight { get; }
		Unit PopupWidth { get; }
		Unit PopupMinHeight { get; }
		Unit PopupMinWidth { get; }
		ResizingMode PopupResizeMode { get; }
		bool PopupCloseOnEscape { get; }
		AppearanceStyleBase ControlStyle { get; }
		AppearanceStyleBase ContentStyle { get; }
		AppearanceStyleBase FooterStyle { get; }
		ImageProperties SizeGrip { get; }
		ImageProperties SizeGripRtl { get; }
		string OkButtonText { get; }
		string CancelButtonText { get; }
		string OkButtonClickScript { get; }
		string CancelButtonClickScript { get; }
		HeaderFilterButtonPanelStyles ButtonPanelStyles { get; }
	}
	[ToolboxItem(false)]
	public class HeaderFilterPopup : DevExpress.Web.ASPxPopupControl {
		const int DefaultHeight = 200,
				  DefaultWidth = 180,
				  DefaultMinHeight = 100,
				  DefaultMinWidth = 180;
		const string ButtonPanelIDJSKey = "cpButtonPanelID",
					 OkButtonIDJSKey = "cpOkButtonID";
		public const string ControlID = "DXHFP";
		IHeaderFilterPopupOwner owner;
		HeaderFilterPopupFooterTemplate panelTemplate;
		public HeaderFilterPopup(IHeaderFilterPopupOwner owner)
			: base((ASPxWebControl)owner) {
			this.owner = owner;
			ID = HeaderFilterPopup.ControlID;
			ParentSkinOwner = Owner;
			ShowHeader = false;
			ShowFooter = true;
			AllowResize = true;
			ShowSizeGrip = ShowSizeGrip.True;
			PopupAnimationType = AnimationType.Fade;
			EnableViewState = false;
			EnableClientSideAPIInternal = true;
			FooterText = string.Empty;
			ContentOverflowX = CssOverflow.Hidden;
			ContentOverflowY = CssOverflow.Auto;
			ResizingMode = Owner.PopupResizeMode;
			CloseOnEscape = Owner.PopupCloseOnEscape;
			if(Owner.ShowButtonPanel) {
				this.panelTemplate = new HeaderFilterPopupFooterTemplate(Owner);
				FooterContentTemplate = PanelTemplate;
			}
		}
		protected IHeaderFilterPopupOwner Owner { get { return owner; } }
		protected HeaderFilterPopupFooterTemplate PanelTemplate { get { return panelTemplate; } }
		protected internal override Paddings GetContentPaddings(PopupWindow window) {
			return new Paddings(0);
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return false;
		}
		public new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected override void CreateControlHierarchy() {
			PopupHorizontalAlign = IsRightToLeft() ? PopupHorizontalAlign.RightSides : PopupHorizontalAlign.LeftSides;
			PopupVerticalAlign = PopupVerticalAlign.Below;
			MinHeight = !Owner.PopupMinHeight.IsEmpty ? Owner.PopupMinHeight : DefaultMinHeight;
			MinWidth = !Owner.PopupMinWidth.IsEmpty ? Owner.PopupMinWidth : DefaultMinWidth;
			base.CreateControlHierarchy();
			EnsureChildControlsRecursive(this, false); 
		}
		protected override void PrepareControlHierarchy() {
			PopupControlStyles.Style.CopyFrom(Owner.ControlStyle);
			ContentStyle.CopyFrom(Owner.ContentStyle);
			FooterStyle.CopyFrom(Owner.FooterStyle);
			SizeGripImage.CopyFrom(Owner.SizeGrip);
			SizeGripRtlImage.CopyFrom(Owner.SizeGripRtl);
			Height = !Owner.PopupHeight.IsEmpty ? Owner.PopupHeight : DefaultHeight;
			Width = !Owner.PopupWidth.IsEmpty ? Owner.PopupWidth : DefaultWidth;
			if(Owner.ShowButtonPanel) {
				JSProperties[ButtonPanelIDJSKey] = PanelTemplate.ButtonPanelClientID;
				JSProperties[OkButtonIDJSKey] = PanelTemplate.OkButtonClientID;
			}
			base.PrepareControlHierarchy();
		}
	}
	public class HeaderFilterButtonPanelStyles : StylesBase {
		const string
			OkButtonStyleName = "OkButton",
			CancelButtonStyleName = "CancelButton";
		public HeaderFilterButtonPanelStyles(ISkinOwner owner)
			: base(owner) {
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonControlStyle OkButton { 
			get { return (ButtonControlStyle)GetStyle(OkButtonStyleName); } 
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonControlStyle CancelButton { 
			get { return (ButtonControlStyle)GetStyle(CancelButtonStyleName); } 
		}
		[DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit ButtonSpacing {
			get { return GetUnitProperty("ButtonSpacing", Unit.Empty); }
			set { SetUnitProperty("ButtonSpacing", Unit.Empty, value); } 
		}
		#region Hide non-usable props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		#endregion
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(OkButtonStyleName, delegate() { return new ButtonControlStyle(); }));
			list.Add(new StyleInfo(CancelButtonStyleName, delegate() { return new ButtonControlStyle(); }));
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			HeaderFilterButtonPanelStyles src = source as HeaderFilterButtonPanelStyles;
			if(src != null) {
				if(ButtonSpacing != src.ButtonSpacing)
					ButtonSpacing = src.ButtonSpacing;
			}
		}
	}
}
namespace DevExpress.Web.Internal {
	public class HeaderFilterPopupFooterTemplate : ITemplate {
		IHeaderFilterPopupOwner owner;
		HeaderFilterButtonPanel panel;
		public HeaderFilterPopupFooterTemplate(IHeaderFilterPopupOwner owner) {
			this.owner = owner;
		}
		HeaderFilterButtonPanel Panel { get { return panel; } }
		public string ButtonPanelClientID {
			get { return Panel != null ? Panel.ClientID : ""; }
		}
		public string OkButtonClientID {
			get { return Panel != null ? Panel.OkButton.ClientID : ""; }
		}
		public void InstantiateIn(Control container) {
			this.panel = new HeaderFilterButtonPanel(this.owner);
			container.Controls.Add(Panel);
		}
	}
	public class HeaderFilterButtonPanel : InternalTable {
		const string ButtonPanelID = "P",
					 OkButtonID = "O",
					 CancelButtonID = "C",
					 CssClassPrefix = "dx",
					 SeparatorCssClassName = "HFBPS";
		IHeaderFilterPopupOwner popupOwner;
		ASPxButton okButton, cancelButton;
		WebControl separator;
		TableRow row;
		public HeaderFilterButtonPanel(IHeaderFilterPopupOwner popupOwner) {
			this.popupOwner = popupOwner;
			ID = ButtonPanelID;
		}
		IHeaderFilterPopupOwner PopupOwner { get { return popupOwner; } }
		public ASPxButton OkButton { get { return okButton; } }
		ASPxButton CancelButton { get { return cancelButton; } }
		WebControl Separator { get { return separator; } }
		TableRow Row { get { return row; } }
		protected override void CreateControlHierarchy() {
			this.row = RenderUtils.CreateTableRow();
			Rows.Add(Row);
			this.okButton = CreateButton("O");
			CreateSeparatorCell();
			this.cancelButton = CreateButton("C");
		}
		ASPxButton CreateButton(string id) {
			ASPxButton button = new ASPxButton();
			button.ID = id;
			button.ParentSkinOwner = PopupOwner;
			button.AutoPostBack = false;
			button.UseSubmitBehavior = false;
			CreateCell().Controls.Add(button);
			return button;
		}
		void CreateSeparatorCell() {
			TableCell cell = CreateCell();
			this.separator = RenderUtils.CreateDiv();
			cell.Controls.Add(this.separator);
		}
		TableCell CreateCell() {
			TableCell cell = new InternalTableCell();
			Row.Cells.Add(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CellSpacing = 0;
			CellPadding = 0;
			Separator.CssClass = StylesBase.CreateCssClassName(CssClassPrefix, SeparatorCssClassName, string.Empty);
			RenderUtils.SetStyleUnitAttribute(Separator, "width", PopupOwner.ButtonPanelStyles.ButtonSpacing);
			PrepareButton(OkButton, PopupOwner.OkButtonText, PopupOwner.OkButtonClickScript, PopupOwner.ButtonPanelStyles.OkButton);
			PrepareButton(CancelButton, PopupOwner.CancelButtonText, PopupOwner.CancelButtonClickScript, PopupOwner.ButtonPanelStyles.CancelButton);
		}
		void PrepareButton(ASPxButton button, string text, string clientClick, AppearanceStyleBase style) {
			button.Text = text;
			button.ClientSideEvents.Click = clientClick;
			button.ControlStyle.CopyFrom(style);
		}
	}
}
