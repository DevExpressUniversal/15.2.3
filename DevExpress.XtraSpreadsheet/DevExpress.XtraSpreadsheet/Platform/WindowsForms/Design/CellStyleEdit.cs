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
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Design {
	#region SpreadsheetCellStyleEdit
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabSpreadsheet),
	Designer("DevExpress.XtraSpreadsheet.Design.XtraSpreadsheetComboBoxEditDesigner," + AssemblyInfo.SRAssemblySpreadsheetDesign)
	]
	public class SpreadsheetCellStyleEdit : ComboBoxEdit {
		static SpreadsheetCellStyleEdit() {
			RepositoryItemSpreadsheetCellStyleEdit.RegisterSpreadsheetCellStyleEdit();
		}
		public SpreadsheetCellStyleEdit() {
		}
		#region Properties
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemSpreadsheetCellStyleEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemSpreadsheetCellStyleEdit Properties { get { return base.Properties as RepositoryItemSpreadsheetCellStyleEdit; } }
		public SpreadsheetControl Control {
			get { return Properties != null ? Properties.Control : null; }
			set {
				if (Properties != null)
					Properties.Control = value;
			}
		}
		#endregion
	}
	#endregion
	#region RepositoryItemSpreadsheetCellStyleEdit
	[
	UserRepositoryItem("RegisterSpreadsheetCellStyleEdit"),
	System.Runtime.InteropServices.ComVisible(false),
	]
	public class RepositoryItemSpreadsheetCellStyleEdit : RepositoryItemComboBox {
		#region Fields
		SpreadsheetControl control;
		CellStyleCollection cellStyles;
		#endregion
		static RepositoryItemSpreadsheetCellStyleEdit() {
			RegisterSpreadsheetCellStyleEdit();
		}
		public static void RegisterSpreadsheetCellStyleEdit() {
			EditorClassInfo info = EditorRegistrationInfo.Default.Editors[InternalEditorTypeName];
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(SpreadsheetCellStyleEdit), typeof(RepositoryItemSpreadsheetCellStyleEdit), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		public RepositoryItemSpreadsheetCellStyleEdit() {
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(SpreadsheetCellStyleEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		#region Control
		public SpreadsheetControl Control {
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
		protected override bool ShouldSerializeItems() {
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
			cellStyles.CollectionChanged += StyleCollectionChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Control.DocumentLoaded -= StyleCollectionChanged;
			Control.EmptyDocumentCreated -= StyleCollectionChanged;
			Control.DocumentModel.DocumentCleared -= OnDocumentCleared;
			UnsubscribeCollectionsChangedEvents();
		}
		protected internal virtual void UnsubscribeCollectionsChangedEvents() {
			cellStyles.CollectionChanged -= StyleCollectionChanged;
		}
		void OnDocumentCleared(object sender, EventArgs e) {
			UnsubscribeCollectionsChangedEvents();
			SetStylesCollections();
			SubscribeCollectionsChangedEvents();
			StyleCollectionChanged(sender, e);
		}
		void SetStylesCollections() {
			cellStyles = Control.DocumentModel.StyleSheet.CellStyles;
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
				if (Control != null)
					AddCellStyles();
			}
			finally {
				Items.EndUpdate();
				EndUpdate();
			}
		}
		protected internal virtual void AddCellStyles() {
			int count = cellStyles.Count;
			for (int i = 0; i < count; i++) {
				CellStyleBase cellStyle = cellStyles[i];
				if (IsStyleVisible(cellStyle)) {
					Items.Add(cellStyle);
				}
			}
		}
		protected internal virtual bool IsStyleVisible(CellStyleBase style) {
			return !style.IsHidden;
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
}
