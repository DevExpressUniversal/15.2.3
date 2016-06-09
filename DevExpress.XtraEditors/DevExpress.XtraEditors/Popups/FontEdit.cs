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
using System.Text;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using System.Drawing;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemFontEdit : RecentlyUsedItemsComboBox {
		bool showSymbolPreview;
		bool showOnlyRegularStyleFonts;
		public RepositoryItemFontEdit() {
			DropDownRows = 10;
			showSymbolPreview = true;
			showOnlyRegularStyleFonts = true;
			InitFontItems();
		}
		public override string EditorTypeName { get { return "FontEdit"; } }
		protected void InitFontItems() {
			FontServiceBase.FillFontFamilies(this, ShowOnlyRegularStyleFonts);
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemFontEdit source = item as RepositoryItemFontEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.showSymbolPreview = source.ShowSymbolFontPreview;
				this.showOnlyRegularStyleFonts = source.ShowOnlyRegularStyleFonts;
			}
			finally {
				EndUpdate();
			}
		}
		protected override bool ShouldSerializeDropDownRows() {
			return DropDownRows != 10;
		}
		protected override void ResetDropDownRows() {
			DropDownRows = 10;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override TextEditStyles TextEditStyle {
			get {
				if(OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Bars) 
					return TextEditStyles.Standard;
				return TextEditStyles.DisableTextEditor;
			}
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DevExpress.Utils.FormatInfo DisplayFormat { get { return base.DisplayFormat; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DevExpress.Utils.FormatInfo EditFormat { get { return base.EditFormat; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DevExpress.XtraEditors.Mask.MaskProperties Mask { get { return base.Mask; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemFontEditShowSymbolFontPreview"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(true)]
		public virtual bool ShowSymbolFontPreview {
			get { return this.showSymbolPreview;  }
			set {
				if(ShowSymbolFontPreview == value) return;
				this.showSymbolPreview = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemFontEditShowOnlyRegularStyleFonts"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(true)]
		public virtual bool ShowOnlyRegularStyleFonts {
			get { return this.showOnlyRegularStyleFonts; }
			set {
				if(ShowOnlyRegularStyleFonts == value) return;
				this.showOnlyRegularStyleFonts = value;
				if(this.Items.Count > 0 && !IsDesignMode) {
					this.resentlyItemsHolder.ClearItems();
					InitFontItems();
				}
				OnPropertiesChanged();
			}
		}
		protected override void DrawFontBoxItem(ListBoxDrawItemEventArgs e, RepositoryItem item) {
			FontItemPaintHelper.DrawFontBoxItem(e, item, ShowSymbolFontPreview);
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(7), SmartTagProperty("RecentlyUsedItemCount", "")]
		public virtual int RecentlyUsedItemCount {
			get { return resentlyItemsHolder.MaxRecentlyUsedItemCount; }
			set {
				if(RecentlyUsedItemCount == value) return;
				this.resentlyItemsHolder.MaxRecentlyUsedItemCount = value;
				this.resentlyItemsHolder.RefreshItems();
				OnPropertiesChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DevExpress.Utils.DefaultBoolean ShowToolTipForTrimmedText { get { return DevExpress.Utils.DefaultBoolean.False; } set { } }
		[Browsable(false)]
		public override SimpleContextItemCollectionOptions ContextButtonOptions {
			get { return base.ContextButtonOptions; }
		}
		[Browsable(false)]
		public override ContextItemCollection ContextButtons {
			get { return base.ContextButtons; }
		}
	}
}
namespace DevExpress.XtraEditors {
	[Designer("DevExpress.XtraEditors.Design.FontEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Displays available fonts in the drop-down window."),
	 DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagFilter(typeof(FontEditFilter)),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "FontEdit")
	]
	public class FontEdit : DevExpress.XtraEditors.ComboBoxEdit {
		int scrollIndex = -1;
		public FontEdit() { }
		public override string EditorTypeName { get { return "FontEdit"; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemFontEdit Properties {
			get { return base.Properties as RepositoryItemFontEdit; }
		}
		protected override int GetNewScrollIndex(int delta) {
			return scrollIndex + delta;
		}
		public override int SelectedIndex {
			get {
				return Properties.Items.IndexOf(SelectedItem);
			}
			set {
				if(value < 0 || value >= Properties.Items.Count) {
					scrollIndex = -1;
					SelectedItem = null;
				}
				else {
					scrollIndex = value;
					SelectedItem = Properties.Items[value];
				}
			}
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(SelectedItem == null) return;
		}
		public override void ClosePopup() {
			base.ClosePopup();
			if(SelectedItem == null) return;
			Properties.StoreRecentItem(SelectedItem);
			scrollIndex = SelectedIndex;
		}
		string value = string.Empty;
		protected override void OnValidated(EventArgs e) {
			base.OnValidated(e);
			if(SelectedIndex == -1) 
				EditValue = value;
			else
				value = Text;
		}
	}
}
