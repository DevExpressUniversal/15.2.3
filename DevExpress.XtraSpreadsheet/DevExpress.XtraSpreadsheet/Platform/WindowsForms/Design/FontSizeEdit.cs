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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.Office.Model;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Design {
	#region SpreadsheetFontSizeEdit
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabSpreadsheet),
	Designer("DevExpress.XtraSpreadsheet.Design.XtraSpreadsheetComboBoxEditDesigner," + AssemblyInfo.SRAssemblySpreadsheetDesign)
	]
	public class SpreadsheetFontSizeEdit : ComboBoxEdit {
		static SpreadsheetFontSizeEdit() {
			RepositoryItemSpreadsheetFontSizeEdit.RegisterSpreadsheetFontSizeEdit();
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemSpreadsheetFontSizeEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemSpreadsheetFontSizeEdit Properties { get { return base.Properties as RepositoryItemSpreadsheetFontSizeEdit; } }
		public SpreadsheetControl Control {
			get {
				return Properties != null ? Properties.Control : null;
			}
			set {
				if (Properties != null)
					Properties.Control = value;
			}
		}
		public SpreadsheetFontSizeEdit() {
		}
	}
	#endregion
	#region RepositoryItemSpreadsheetFontSizeEdit
	[
	UserRepositoryItem("RegisterSpreadsheetFontSizeEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemSpreadsheetFontSizeEdit : RepositoryItemComboBox {
		static RepositoryItemSpreadsheetFontSizeEdit() {
			RegisterSpreadsheetFontSizeEdit();
		}
		public static void RegisterSpreadsheetFontSizeEdit() {
			EditorClassInfo info = EditorRegistrationInfo.Default.Editors[InternalEditorTypeName];
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(SpreadsheetFontSizeEdit), typeof(RepositoryItemSpreadsheetFontSizeEdit), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		SpreadsheetControl control;
		public RepositoryItemSpreadsheetFontSizeEdit() {
		}
		#region Properties
		[Localizable(false)]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		internal static string InternalEditorTypeName { get { return typeof(SpreadsheetFontSizeEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		public SpreadsheetControl Control {
			get { return control; }
			set {
				if (control == value)
					return;
				UnsubscribeSpreadsheetControlEvents();
				control = value;
				SubscribeSpreadsheetControlEvents();
				OnControlChanged();
			}
		}
		protected override bool ShouldSerializeItems() {
			if (Control == null)
				return base.ShouldSerializeItems();
			PredefinedFontSizeCollection fontSizes = Control.InnerControl.PredefinedFontSizeCollection;
			int count = fontSizes.Count;
			if (count != Items.Count)
				return true;
			for (int i = 0; i < count; i++)
				if (!Object.Equals(Items[i], fontSizes[i]))
					return true;
			return false;
		}
		#endregion
		protected internal virtual void PopulateItems() {
			Items.Clear();
			if (Control != null) {
				BeginUpdate();
				try {
					PredefinedFontSizeCollection fontSizes = Control.InnerControl.PredefinedFontSizeCollection;
					int count = fontSizes.Count;
					for (int i = 0; i < count; i++) {
						Items.Add(fontSizes[i]);
					}
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected virtual void OnControlChanged() {
			PopulateItems();
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
		protected internal virtual void SubscribeSpreadsheetControlEvents() {
			if (Control != null)
				Control.BeforeDispose += OnSpreadsheetControlBeforeDispose;
		}
		protected internal virtual void UnsubscribeSpreadsheetControlEvents() {
			if (Control != null)
				Control.BeforeDispose -= OnSpreadsheetControlBeforeDispose;
		}
		protected internal void OnSpreadsheetControlBeforeDispose(object sender, EventArgs e) {
			Control = null;
		}
	}
	#endregion
}
