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
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemMemoExEdit : RepositoryItemBlobBaseEdit {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemMemoExEdit Properties { get { return this; } }
		bool _wordWrap, _acceptsReturn, _acceptsTab;
		ScrollBars _scrollBars;
		[ThreadStatic]
		static object defaultImages;
		public RepositoryItemMemoExEdit() {
			this._scrollBars = ScrollBars.None;
			this._wordWrap = true;
			this._acceptsReturn = true;
			this._acceptsTab = true;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			return CreateTextBrick(info);
		}
		protected internal override object DefaultImages {
			get {
				if(defaultImages == null)
					defaultImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.MemoEx.bmp", typeof(RepositoryItemMemoExEdit).Assembly, new Size(16, 13), Color.Magenta);
				return defaultImages;
			}
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "MemoExEdit"; } }
		public override void Assign(RepositoryItem item) {
			RepositoryItemMemoExEdit source = item as RepositoryItemMemoExEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this._acceptsReturn = source.AcceptsReturn;
				this._acceptsTab = source.AcceptsTab;
				this._scrollBars = source.ScrollBars;
				this._wordWrap = source.WordWrap;
			} finally {
				EndUpdate();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoExEditWordWrap"),
#endif
 DefaultValue(true)]
		public bool WordWrap {
			get { return _wordWrap; }
			set {
				if(WordWrap == value) return;
				_wordWrap = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoExEditAcceptsReturn"),
#endif
 DefaultValue(true)]
		public bool AcceptsReturn {
			get { return _acceptsReturn; }
			set {
				if(AcceptsReturn == value) return;
				_acceptsReturn = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoExEditAcceptsTab"),
#endif
 DefaultValue(true)]
		public bool AcceptsTab {
			get { return _acceptsTab; }
			set {
				if(AcceptsTab == value) return;
				_acceptsTab = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoExEditScrollBars"),
#endif
 DefaultValue(ScrollBars.None), SmartTagProperty("Scroll Bars", "")]
		public ScrollBars ScrollBars {
			get { return _scrollBars; }
			set {
				if(ScrollBars == value) return;
				_scrollBars = value;
				OnPropertiesChanged();
			}
		}
	}
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free),
	 Designer("DevExpress.XtraEditors.Design.MemoExEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows multi-line text to be edited in a drop-down window."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(MemoExEditActions), "Lines", "Edit multi-line text"),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "MemoExEdit")
	]
	public class MemoExEdit : BlobBaseEdit {
		public MemoExEdit() {
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "MemoExEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MemoExEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemMemoExEdit Properties { get { return base.Properties as RepositoryItemMemoExEdit; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new MemoExPopupForm(this);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MemoExEditLines"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public string[] Lines {
			get { return LinesConverter.TextToLines(Text); }
			set { Text = LinesConverter.LinesToText(value); }
		}
		protected internal override bool AllowPopupTabOut { get { return false; } }
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class MemoExEditViewInfo : BlobBaseEditViewInfo {
		public MemoExEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		public override int ImageIndex {
			get {
				int res = DisplayText == null || DisplayText.Length == 0 ? 1 : 0;
				if(ImageCollection.GetImageListImageCount(Images) == 1) res = 0;
				return res;
			}
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	public class MemoExPopupForm : BlobBasePopupForm {
		MemoEdit memo;
		public MemoExPopupForm(MemoExEdit ownerEdit)
			: base(ownerEdit) {
			this.memo = CreateMemoEdit();
			this.memo.BorderStyle = BorderStyles.NoBorder;
			this.memo.Properties.Appearance.Assign(ownerEdit.Properties.AppearanceDropDown);
			this.memo.scrollHelper.LookAndFeel = OwnerEdit.LookAndFeel;
			this.memo.MenuManager = OwnerEdit.MenuManager;
			this.memo.Visible = false;
			this.memo.Modified += new EventHandler(OnMemo_Modified);
			this.Controls.Add(memo);
			CreateSeparatorLine();
			UpdateMemo();
		}
		protected virtual MemoEdit CreateMemoEdit() {
			return new MemoEdit();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Memo != null) {
					this.memo.Modified -= new EventHandler(OnMemo_Modified);
					this.memo.Dispose();
					this.memo = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override Control EmbeddedControl { get { return Memo; } }
		protected virtual MemoEdit Memo { get { return memo; } }
		[Browsable(false)]
		public new MemoExEdit OwnerEdit { get { return base.OwnerEdit as MemoExEdit; } }
		protected virtual void UpdateMemo() {
			Memo.Properties.BeginUpdate();
			try {
				Memo.Properties.Appearance.Assign(ViewInfo.PaintAppearanceContent);
				Memo.Properties.ReadOnly = OwnerEdit.Properties.ReadOnly;
				Memo.Properties.MaxLength = OwnerEdit.Properties.MaxLength;
				Memo.Properties.AcceptsReturn = OwnerEdit.Properties.AcceptsReturn;
				Memo.Properties.AcceptsTab = OwnerEdit.Properties.AcceptsTab;
				Memo.Properties.ScrollBars = OwnerEdit.Properties.ScrollBars;
				Memo.Properties.WordWrap = OwnerEdit.Properties.WordWrap;
				Memo.Properties.CharacterCasing = OwnerEdit.Properties.CharacterCasing;
			} finally {
				Memo.Properties.EndUpdate();
			}
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyData == (Keys.Control | Keys.Enter)) {
				e.Handled = true;
				OwnerEdit.ClosePopup();
				return;
			}
			base.ProcessKeyDown(e);
		}
		public override void ShowPopupForm() {
			BeginControlUpdate();
			try {
				Memo.EditValue = OwnerEdit.EditValue;
				Memo.IsModified = false;
			} finally {
				EndControlUpdate();
			}
			base.ShowPopupForm();
			Memo.scrollHelper.UpdateScrollBars();
			FocusFormControl(Memo);
		}
		protected virtual void OnMemo_Modified(object sender, EventArgs e) {
			if(IsControlUpdateLocked) return;
			OkButton.Enabled = true;
		}
		[DXCategory(CategoryName.Appearance)]
		public override object ResultValue { get { return Memo.EditValue; } }
		protected override void OnVisibleChanged(EventArgs e) {
			if(Visible) UpdateMemo();
			base.OnVisibleChanged(e);
		}
	}
}
