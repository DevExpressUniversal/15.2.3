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
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Design {
	#region RepositoryItemFontStyle
	[
	UserRepositoryItem("RegisterFontStyleEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemFontStyle : RepositoryItemImageComboBox {
		static RepositoryItemFontStyle() { RegisterFontStyleEdit(); }
		public RepositoryItemFontStyle() {
		}
		#region RegisterFontStyleEdit
		public static void RegisterFontStyleEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(FontStyleEdit).Name, typeof(FontStyleEdit), typeof(RepositoryItemFontStyle), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(FontStyleEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected internal virtual void InitItems(string fontFamilyName) {
			BeginUpdate();
			try {
				PopulateItems(Items, fontFamilyName);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void PopulateItems(ImageComboBoxItemCollection target, string fontFamilyName) {
			target.Clear();
			if (String.IsNullOrEmpty(fontFamilyName)) {
				PopulateByDefaultItems(target);
				return;
			}
			try {
				FontFamily fontFamily = new FontFamily(fontFamilyName);
				if (fontFamily.IsStyleAvailable(FontStyle.Regular))
					target.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FontStyle_Regular), FontStyle.Regular, -1));
				if (fontFamily.IsStyleAvailable(FontStyle.Italic))
					target.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FontStyle_Italic), FontStyle.Italic, -1));
				if (fontFamily.IsStyleAvailable(FontStyle.Bold))
					target.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FontStyle_Bold), FontStyle.Bold, -1));
				if (fontFamily.IsStyleAvailable(FontStyle.Bold | FontStyle.Italic))
					target.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FontStyle_BoldItalic), FontStyle.Bold | FontStyle.Italic, -1));
			}
			catch {
			}
		}
		protected internal virtual void PopulateByDefaultItems(ImageComboBoxItemCollection target) {
			target.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FontStyle_Regular), FontStyle.Regular, -1));
			target.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FontStyle_Italic), FontStyle.Italic, -1));
			target.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FontStyle_Bold), FontStyle.Bold, -1));
			target.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FontStyle_BoldItalic), FontStyle.Bold | FontStyle.Italic, -1));
		}
	}
	#endregion
	#region FontStyleEdit
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public class FontStyleEdit : ImageComboBoxEdit, IBatchUpdateable, IBatchUpdateHandler {
		static FontStyleEdit() {
			RepositoryItemFontStyle.RegisterFontStyleEdit();
		}
		#region Fields
		string fontFamilyName;
		bool? fontBold;
		bool? fontItalic;
		bool deferredUpdateControl;
		BatchUpdateHelper batchUpdateHelper;
		#endregion
		public FontStyleEdit() {
			this.fontBold = null;
			this.fontItalic = null;
			this.EditValue = null;
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.Properties.NullText = String.Empty;
			Properties.InitItems(FontFamilyName);
		}
		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string FontFamilyName {
			get {
				if (String.IsNullOrEmpty(fontFamilyName))
					return String.Empty;
				else
					return fontFamilyName;
			}
			set {
				if (fontFamilyName == value)
					return;
				fontFamilyName = value;
				Properties.InitItems(fontFamilyName);
				UpdateControl();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? FontBold {
			get {
				return fontBold;
			}
			set {
				if (fontBold == value)
					return;
				fontBold = value;
				UpdateControl();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? FontItalic {
			get {
				return fontItalic;
			}
			set {
				if (fontItalic == value)
					return;
				fontItalic = value;
				UpdateControl();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemFontStyle Properties { get { return base.Properties as RepositoryItemFontStyle; } }
		public override string EditorTypeName { get { return GetType().Name; } }
		#endregion
		protected override void OnEditValueChanged() {
			if (EditValue == null) {
				this.fontItalic = null;
				this.fontBold = null;
			}
			else {
				FontStyle fontStyle = (FontStyle)EditValue;
				this.fontItalic = (fontStyle & FontStyle.Italic) == FontStyle.Italic;
				this.fontBold = (fontStyle & FontStyle.Bold) == FontStyle.Bold;
			}
			base.OnEditValueChanged();
		}
		#region UpdateControl
		protected internal virtual void UpdateControl() {
			if (IsUpdateLocked)
				this.deferredUpdateControl = true;
			else
				UpdateControlCore();
		}
		#endregion
		#region UpdateControlCore
		protected internal virtual void UpdateControlCore() {
			if (FontItalic == true && FontBold == true && IsFontStyleAllowable(FontStyle.Italic | FontStyle.Bold))
				EditValue = FontStyle.Italic | FontStyle.Bold;
			else if (FontItalic == true && IsFontStyleAllowable(FontStyle.Italic))
				EditValue = FontStyle.Italic;
			else if (FontBold == true && IsFontStyleAllowable(FontStyle.Bold))
				EditValue = FontStyle.Bold;
			else if (FontItalic == false && FontBold == false && IsFontStyleAllowable(FontStyle.Regular))
				EditValue = FontStyle.Regular;
			else
				EditValue = null;
		}
		#endregion
		#region IsFontStyleAllowable
		protected internal virtual bool IsFontStyleAllowable(FontStyle fontStyle) {
			return Properties.Items.GetItem(fontStyle) != null;
		}
		#endregion
		#region IBatchUpdateable implementation
		public new void BeginUpdate() {
			base.BeginUpdate();
			this.batchUpdateHelper.BeginUpdate();
		}
		public new void EndUpdate() {
			batchUpdateHelper.EndUpdate();
			base.EndUpdate();
		}
		public new void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
			base.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			deferredUpdateControl = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (this.deferredUpdateControl)
				UpdateControlCore();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
	}
	#endregion
}
