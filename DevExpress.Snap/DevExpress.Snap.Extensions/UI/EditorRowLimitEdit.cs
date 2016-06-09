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
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraEditors.Controls;
using DevExpress.Snap;
using System.Collections.Generic;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Extensions.UI;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Design {
	[
	DXToolboxItem(false),
	]
	public class EditorRowLimitEdit : ComboBoxEdit {
		static EditorRowLimitEdit() {
			RepositoryItemEditorRowLimitEdit.RegisterEditorRowLimitEdit();
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemEditorRowLimitEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemEditorRowLimitEdit Properties { get { return base.Properties as RepositoryItemEditorRowLimitEdit; } }
		public SnapControl Control {
			get {
				return Properties != null ? Properties.Control : null;
			}
			set {
				if (Properties != null)
					Properties.Control = value;
			}
		}
		public EditorRowLimitEdit() {
		}		   
	}
	[
	UserRepositoryItem("RegisterEditorRowLimitEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemEditorRowLimitEdit : RepositoryItemComboBox {
		static readonly List<EditorRowLimitCommandValue> PredefinedEditorRowLimitCollection;
		static RepositoryItemEditorRowLimitEdit() {
			RegisterEditorRowLimitEdit();
			PredefinedEditorRowLimitCollection = new List<EditorRowLimitCommandValue>();
			PredefinedEditorRowLimitCollection.Add(new EditorRowLimitCommandValue(0));
			PredefinedEditorRowLimitCollection.Add(new EditorRowLimitCommandValue(5));
			PredefinedEditorRowLimitCollection.Add(new EditorRowLimitCommandValue(10));
			PredefinedEditorRowLimitCollection.Add(new EditorRowLimitCommandValue(15));
			PredefinedEditorRowLimitCollection.Add(new EditorRowLimitCommandValue(20));
		}
		public static void RegisterEditorRowLimitEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(EditorRowLimitEdit), typeof(RepositoryItemEditorRowLimitEdit), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		SnapControl control;
		#region Properties
		[Localizable(false)]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		internal static string InternalEditorTypeName { get { return typeof(EditorRowLimitEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		public SnapControl Control {
			get { return control; }
			set {
				if (control == value)
					return;
				UnsubscribeSnapControlEvents();
				control = value;
				SubscribeSnapControlEvents();
				OnControlChanged();
			}
		}
		protected override bool ShouldSerializeItems() {			
			int count = PredefinedEditorRowLimitCollection.Count;
			if (count != Items.Count)
				return true;
			for (int i = 0; i < count; i++)
				if (!Object.Equals(Items[i], PredefinedEditorRowLimitCollection[i]))
					return true;
			return false;			
		}
		#endregion        
		protected internal virtual void PopulateItems() {
			Items.Clear();
			if (Control != null) {
				BeginUpdate();
				try {
					int count = PredefinedEditorRowLimitCollection.Count;
					for (int i = 0; i < count; i++) {
						Items.Add(PredefinedEditorRowLimitCollection[i]);
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
		protected internal virtual void SubscribeSnapControlEvents() {
			if (Control != null)
				Control.BeforeDispose += OnSnapControlBeforeDispose;
		}
		protected internal virtual void UnsubscribeSnapControlEvents() {
			if (Control != null)
				Control.BeforeDispose -= OnSnapControlBeforeDispose;
		}
		protected internal void OnSnapControlBeforeDispose(object sender, EventArgs e) {
			Control = null;
		}		
	}
}
