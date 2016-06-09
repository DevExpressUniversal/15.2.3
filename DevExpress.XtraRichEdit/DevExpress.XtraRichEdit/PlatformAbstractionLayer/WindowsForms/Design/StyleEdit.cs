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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Design {
	#region RichEditStyleEdit
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit),
	Designer("DevExpress.XtraRichEdit.Design.XtraRichEditComboBoxEditDesigner," + AssemblyInfo.SRAssemblyRichEditDesign)
	]
	public class RichEditStyleEdit : ComboBoxEdit {
		static RichEditStyleEdit() {
			RepositoryItemRichEditStyleEdit.RegisterRichEditStyleEdit();
		}
		public RichEditStyleEdit() {
		}
		#region Properties
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemRichEditStyleEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemRichEditStyleEdit Properties { get { return base.Properties as RepositoryItemRichEditStyleEdit; } }
		public RichEditControl Control {
			get { return Properties != null ? Properties.Control : null; }
			set {
				if (Properties != null)
					Properties.Control = value;
			}
		}
		#endregion
	}
	#endregion
	#region RepositoryItemRichEditStyleEdit
	[
	UserRepositoryItem("RegisterRichEditStyleEdit"),
	System.Runtime.InteropServices.ComVisible(false),
	]
	public class RepositoryItemRichEditStyleEdit : RepositoryItemComboBox {
		#region Fields
		RichEditControl control;
		ParagraphStyleCollection paragraphStyles;
		CharacterStyleCollection characterStyles;
		#endregion
		static RepositoryItemRichEditStyleEdit() {
			RegisterRichEditStyleEdit();
		}
		public static void RegisterRichEditStyleEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(RichEditStyleEdit), typeof(RepositoryItemRichEditStyleEdit), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		public RepositoryItemRichEditStyleEdit() {
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(RichEditStyleEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		#region Control
		public RichEditControl Control {
			get { return control; }
			set {
				if (control == value)
					return;
				if (control != null)
					UnsubscribeEvents();
				control = value;
				if (control != null) {
					OnControlChanged();
					SubscribeEvents();
				}
			}
		}
		#endregion
		#region Items
		[Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		protected internal override bool ShouldSerializeItems() {
			return false;
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeEvents() {
			Control.DocumentLoaded += StyleCollectionChanged;
			Control.EmptyDocumentCreated += StyleCollectionChanged;
			Control.DocumentModel.DocumentCleared += OnDocumentCleared;
			SubscribeCollectionsChangedEvents();
		}
		protected internal virtual void SubscribeCollectionsChangedEvents() {
			characterStyles.CollectionChanged += StyleCollectionChanged;
			paragraphStyles.CollectionChanged += StyleCollectionChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Control.DocumentLoaded -= StyleCollectionChanged;
			Control.EmptyDocumentCreated -= StyleCollectionChanged;
			Control.DocumentModel.DocumentCleared -= OnDocumentCleared;
			UnsubscribeCollectionsChangedEvents();
		}
		protected internal virtual void UnsubscribeCollectionsChangedEvents() {
			characterStyles.CollectionChanged -= StyleCollectionChanged;
			paragraphStyles.CollectionChanged -= StyleCollectionChanged;
		}
		void OnDocumentCleared(object sender, EventArgs e) {
			UnsubscribeCollectionsChangedEvents();
			SetStylesCollections();
			SubscribeCollectionsChangedEvents();
			StyleCollectionChanged(sender, e);
		}
		void SetStylesCollections() {
			characterStyles = Control.DocumentModel.CharacterStyles;
			paragraphStyles = Control.DocumentModel.ParagraphStyles;
		}
		void StyleCollectionChanged(object sender, EventArgs e) {
			if (!DesignMode)
				PopulateItems();
		}
		protected virtual void OnControlChanged() {
			if (control == null)
				return;
			SetStylesCollections();
			PopulateItems();
		}
		void PopulateItems() {
			BeginUpdate();
			Items.BeginUpdate();
			try {
				Items.Clear();
				if (Control != null) {
					AddParagraphStyles();
					AddCharacterStyles();
				}
			}
			finally {
				Items.EndUpdate();
				CancelUpdate();
				LayoutChanged();
			}
		}
		protected internal virtual bool IsStyleVisible(IStyle style) {
			return !(style.Deleted || style.Hidden || style.Semihidden);
		}
		protected internal virtual void AddParagraphStyles() {
			ParagraphStyleCollection paragraphStyles = Control.DocumentModel.ParagraphStyles;
			int count = paragraphStyles.Count;
			for (int i = 0; i < count; i++) {
				ParagraphStyle paragraphStyle = paragraphStyles[i];
				if (IsStyleVisible(paragraphStyle)) {
					Items.Add(new RichEditStyleItem(Control.DocumentModel, new ParagraphStyleFormatting(paragraphStyle.Id)));
				}
			}
		}
		protected internal virtual void AddCharacterStyles() {
			CharacterStyleCollection characterStyles = Control.DocumentModel.CharacterStyles;
			int count = characterStyles.Count;
			for (int i = 0; i < count; i++) {
				CharacterStyle characterStyle = characterStyles[i];
				if (IsStyleVisible(characterStyle) && !characterStyle.HasLinkedStyle)
					Items.Add(new RichEditStyleItem(Control.DocumentModel, new CharacterStyleFormatting(characterStyle.Id)));
			}
		}
		public override void BeginInit() {
			base.BeginInit();
			Items.Clear();
		}
		public override void EndInit() {
			if (Items.Count <= 0)
				PopulateItems();
			base.EndInit();
		}
	}
	#endregion
	#region RichEditStyleItem
	public class RichEditStyleItem : IXtraRichEditFormatting {
		readonly DocumentModel documentModel;
		readonly IXtraRichEditFormatting formatting;
		public RichEditStyleItem(DocumentModel documentModel, IXtraRichEditFormatting formatting) {
			this.documentModel = documentModel;
			this.formatting = formatting;
		}
		public IXtraRichEditFormatting Formatting { get { return formatting; } }
		public override string ToString() {
			return Formatting.GetLocalizedCaption(this.documentModel);
		}
		public override bool Equals(object obj) {
			RichEditStyleItem styleItem = obj as RichEditStyleItem;
			if (Object.ReferenceEquals(styleItem, null))
				return false;
			return Formatting.Equals(styleItem.Formatting);
		}
		public override int GetHashCode() {
			return Formatting.GetHashCode();
		}
		public static bool operator ==(RichEditStyleItem item1, RichEditStyleItem item2) {
			if (Object.ReferenceEquals(item1, item2))
				return true;
			if (Object.ReferenceEquals(item1, null) || Object.ReferenceEquals(item2, null))
				return false;
			return item1.Equals(item2);
		}
		public static bool operator !=(RichEditStyleItem item1, RichEditStyleItem item2) {
			return !(item1 == item2);
		}
		bool IXtraRichEditFormatting.AllowSelectionExpanding {
			get { return Formatting.AllowSelectionExpanding; }
		}
		void IXtraRichEditFormatting.Apply(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end) {
			Formatting.Apply(documentModel, start, end);
		}
		string IXtraRichEditFormatting.GetCaption(DocumentModel documentModel) {
			return Formatting.GetCaption(documentModel);
		}
		string IXtraRichEditFormatting.GetLocalizedCaption(DocumentModel documentModel) {
			return Formatting.GetLocalizedCaption(documentModel);
		}
	}
	#endregion
	#region RichEditTableStyleEdit
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit),
	Designer("DevExpress.XtraRichEdit.Design.XtraRichEditComboBoxEditDesigner," + AssemblyInfo.SRAssemblyRichEditDesign)
	]
	public class RichEditTableStyleEdit : ComboBoxEdit {
		static RichEditTableStyleEdit() {
			RepositoryItemRichEditTableStyleEdit.RegisterRichEditTableStyleEdit();
		}
		public RichEditTableStyleEdit() {
		}
		#region Properties
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemRichEditTableStyleEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemRichEditTableStyleEdit Properties { get { return base.Properties as RepositoryItemRichEditTableStyleEdit; } }
		public RichEditControl Control {
			get { return Properties != null ? Properties.Control : null; }
			set {
				if (Properties != null)
					Properties.Control = value;
			}
		}
		#endregion
	}
	#endregion
	#region RichEditTableCellStyleEdit
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit),
	Designer("DevExpress.XtraRichEdit.Design.XtraRichEditComboBoxEditDesigner," + AssemblyInfo.SRAssemblyRichEditDesign)
	]
	public class RichEditTableCellStyleEdit : ComboBoxEdit {
		static RichEditTableCellStyleEdit() {
			RepositoryItemRichEditTableCellStyleEdit.RegisterRichEditTableCellStyleEdit();
		}
		public RichEditTableCellStyleEdit() {
		}
		#region Properties
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemRichEditTableCellStyleEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemRichEditTableCellStyleEdit Properties { get { return base.Properties as RepositoryItemRichEditTableCellStyleEdit; } }
		public RichEditControl Control {
			get { return Properties != null ? Properties.Control : null; }
			set {
				if (Properties != null)
					Properties.Control = value;
			}
		}
		#endregion
	}
	#endregion
	public abstract class RepositoryItemRichEditTableStyleEditBase : RepositoryItemComboBox {
		#region Fields
		RichEditControl control;
		#endregion
		static RepositoryItemRichEditTableStyleEditBase() {
		}
		protected RepositoryItemRichEditTableStyleEditBase() {
		}
		#region Properties
		#region Control
		public RichEditControl Control {
			get { return control; }
			set {
				if (control == value)
					return;
				if (control != null)
					UnsubscribeEvents();
				control = value;
				if (control != null) {
					OnControlChanged();
					SubscribeEvents();
				}
			}
		}
		#endregion
		#region Items
		[Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		protected internal override bool ShouldSerializeItems() {
			return false;
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeEvents() {
			Control.DocumentLoaded += StyleCollectionChanged;
			Control.EmptyDocumentCreated += StyleCollectionChanged;
			Control.DocumentModel.DocumentCleared += OnDocumentCleared;
			SubscribeCollectionsChangedEvents();
		}
		protected internal virtual void UnsubscribeEvents() {
			Control.DocumentLoaded -= StyleCollectionChanged;
			Control.EmptyDocumentCreated -= StyleCollectionChanged;
			Control.DocumentModel.DocumentCleared -= OnDocumentCleared;
			UnsubscribeCollectionsChangedEvents();
		}
		protected internal abstract void SubscribeCollectionsChangedEvents();
		protected internal abstract void UnsubscribeCollectionsChangedEvents();
		void OnDocumentCleared(object sender, EventArgs e) {
			UnsubscribeCollectionsChangedEvents();
			SetStylesCollections();
			SubscribeCollectionsChangedEvents();
			StyleCollectionChanged(sender, e);
		}
		protected internal abstract void SetStylesCollections();
		protected internal abstract void AddStyles();
		protected internal void StyleCollectionChanged(object sender, EventArgs e) {
			if (!DesignMode)
				PopulateItems();
		}
		protected virtual void OnControlChanged() {
			if (control == null)
				return;
			SetStylesCollections();
			PopulateItems();
		}
		void PopulateItems() {
			BeginUpdate();
			Items.BeginUpdate();
			try {
				Items.Clear();
				if (Control != null)
					AddStyles();
			}
			finally {
				Items.EndUpdate();
				EndUpdate();
			}
		}
		protected internal virtual bool IsStyleVisible(IStyle style) {
			return !(style.Deleted || style.Hidden || style.Semihidden);
		}
		public override void BeginInit() {
			base.BeginInit();
			Items.Clear();
		}
		public override void EndInit() {
			if (Items.Count <= 0)
				PopulateItems();
			base.EndInit();
		}
	}
	#region RepositoryItemRichEditTableStyleEdit
	[
	UserRepositoryItem("RegisterRichEditTableStyleEdit"),
	System.Runtime.InteropServices.ComVisible(false),
	]
	public class RepositoryItemRichEditTableStyleEdit : RepositoryItemRichEditTableStyleEditBase {
		#region Fields
		TableStyleCollection tableStyles;
		#endregion
		static RepositoryItemRichEditTableStyleEdit() {
			RegisterRichEditTableStyleEdit();
		}
		public static void RegisterRichEditTableStyleEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(RichEditTableStyleEdit), typeof(RepositoryItemRichEditTableStyleEdit), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(RichEditTableStyleEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		#endregion
		protected internal override void SubscribeCollectionsChangedEvents() {
			tableStyles.CollectionChanged += StyleCollectionChanged;
		}
		protected internal override void UnsubscribeCollectionsChangedEvents() {
			tableStyles.CollectionChanged -= StyleCollectionChanged;
		}
		protected internal override void SetStylesCollections() {
			tableStyles = Control.DocumentModel.TableStyles;
		}
		protected internal override void AddStyles() {
			TableStyleCollection tableStyles = Control.DocumentModel.TableStyles;
			int count = tableStyles.Count;
			for (int i = 0; i < count; i++) {
				TableStyle tableStyle = tableStyles[i];
				if (IsStyleVisible(tableStyle)) {
					Items.Add(new TableStyleFormatting(tableStyle.Id));
				}
			}
		}
	}
	#endregion
	#region RepositoryItemRichEditTableCellStyleEdit
	[
	UserRepositoryItem("RegisterRichEditTableCellStyleEdit"),
	System.Runtime.InteropServices.ComVisible(false),
	]
	public class RepositoryItemRichEditTableCellStyleEdit : RepositoryItemRichEditTableStyleEditBase {
		#region Fields
		TableCellStyleCollection tableCellStyles;
		#endregion
		static RepositoryItemRichEditTableCellStyleEdit() {
			RegisterRichEditTableCellStyleEdit();
		}
		public static void RegisterRichEditTableCellStyleEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(RichEditTableCellStyleEdit), typeof(RepositoryItemRichEditTableCellStyleEdit), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(RichEditTableCellStyleEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		#endregion
		protected internal override void SubscribeCollectionsChangedEvents() {
			tableCellStyles.CollectionChanged += StyleCollectionChanged;
		}
		protected internal override void UnsubscribeCollectionsChangedEvents() {
			tableCellStyles.CollectionChanged -= StyleCollectionChanged;
		}
		protected internal override void SetStylesCollections() {
			tableCellStyles = Control.DocumentModel.TableCellStyles;
		}
		protected internal override void AddStyles() {
			TableCellStyleCollection tableCellStyles = Control.DocumentModel.TableCellStyles;
			int count = tableCellStyles.Count;
			for (int i = 0; i < count; i++) {
				TableCellStyle tableCellStyle = tableCellStyles[i];
				if (IsStyleVisible(tableCellStyle)) {
					Items.Add(new TableCellStyleFormatting(tableCellStyle.Id));
				}
			}
		}
	}
	#endregion
}
