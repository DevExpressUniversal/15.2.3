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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Design {
	#region RichEditFontSizeEdit
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit),
	Designer("DevExpress.XtraRichEdit.Design.XtraRichEditComboBoxEditDesigner," + AssemblyInfo.SRAssemblyRichEditDesign)
	]
	public class RichEditFontSizeEdit : ComboBoxEdit {
		static RichEditFontSizeEdit() {
			RepositoryItemRichEditFontSizeEdit.RegisterRichEditFontSizeEdit();
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemRichEditFontSizeEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemRichEditFontSizeEdit Properties { get { return base.Properties as RepositoryItemRichEditFontSizeEdit; } }
		public RichEditControl Control {
			get {
				return Properties != null ? Properties.Control : null;
			}
			set {
				if (Properties != null)
					Properties.Control = value;
			}
		}
		public RichEditFontSizeEdit() {
		}
	}
	#endregion
	#region RepositoryItemRichEditFontSizeEdit
	[
	UserRepositoryItem("RegisterRichEditFontSizeEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemRichEditFontSizeEdit : RepositoryItemComboBox {
		static RepositoryItemRichEditFontSizeEdit() {
			RegisterRichEditFontSizeEdit();
		}
		public static void RegisterRichEditFontSizeEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(RichEditFontSizeEdit), typeof(RepositoryItemRichEditFontSizeEdit), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		RichEditControl control;
		public RepositoryItemRichEditFontSizeEdit() {
		}
		#region Properties
		[Localizable(false)]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		internal static string InternalEditorTypeName { get { return typeof(RichEditFontSizeEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		public RichEditControl Control {
			get { return control; }
			set {
				if (control == value)
					return;
				UnsubscribeRichEditControlEvents();
				control = value;
				SubscribeRichEditControlEvents();
				OnControlChanged();
			}
		}
		protected internal override bool ShouldSerializeItems() {
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
		protected internal virtual void SubscribeRichEditControlEvents() {
			if (Control != null)
				Control.BeforeDispose += OnRichEditControlBeforeDispose;
		}
		protected internal virtual void UnsubscribeRichEditControlEvents() {
			if (Control != null)
				Control.BeforeDispose -= OnRichEditControlBeforeDispose;
		}
		protected internal void OnRichEditControlBeforeDispose(object sender, EventArgs e) {
			Control = null;
		}
	}
	#endregion
}
