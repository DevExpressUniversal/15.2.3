#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.ExpressApp.Win.Editors {
	public class DatePropertyEditor : DXPropertyEditor {
		protected override object CreateControlCore() {
			return new DateTimeEdit();
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			RepositoryItemDateTimeEdit dateTimeEdit = (RepositoryItemDateTimeEdit)item;
			dateTimeEdit.Init(EditMask, DisplayFormat);
			dateTimeEdit.NullDate = AllowNull ? null : (object)DateTime.MinValue;
			dateTimeEdit.AllowNullInput = AllowNull ? DefaultBoolean.True : DefaultBoolean.Default;
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemDateTimeEdit();
		}
		public DatePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		public new DateEdit Control {
			get { return (DateEdit)base.Control; }
		}
	}
	public class RepositoryItemDateTimeEdit : RepositoryItemDateEdit {
		internal const string EditorName = "DateTimeEdit";
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(DateTimeEdit),
					typeof(RepositoryItemDateTimeEdit), typeof(DateEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.DateEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
			}
		}
		static RepositoryItemDateTimeEdit() {
			RepositoryItemDateTimeEdit.Register();
		}
		private void RepositoryItemDateTimeEdit_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e) {
			RepositoryItemDateTimeEdit repItem = null;
			if(sender is RepositoryItemDateTimeEdit) {
				repItem = (RepositoryItemDateTimeEdit)sender;
			} else if(sender is DateTimeEdit) {
				repItem = (RepositoryItemDateTimeEdit)((DateTimeEdit)sender).Properties;
			}
			if(repItem != null && e.Value == null) {
				e.Value = repItem.AllowNullInput == DefaultBoolean.True ? null : (object)DateTime.MinValue;
				e.Handled = true;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					this.ParseEditValue -= new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(RepositoryItemDateTimeEdit_ParseEditValue);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public override string EditorTypeName { get { return EditorName; } }
		public RepositoryItemDateTimeEdit(string editMask, string displayFormat)
			: this() {
			Init(editMask, displayFormat);
		}
		public RepositoryItemDateTimeEdit() {
			NullText = "";
			Mask.MaskType = MaskType.DateTime;
			Mask.UseMaskAsDisplayFormat = true;
			DisplayFormat.FormatType = FormatType.DateTime;
			ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.DoubleClick;
			ParseEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(RepositoryItemDateTimeEdit_ParseEditValue);
		}
		public void Init(string editMask, string displayFormat) {
			if(!string.IsNullOrEmpty(editMask)) {
				Mask.EditMask = editMask;
			}
			if(!string.IsNullOrEmpty(displayFormat)) {
				Mask.UseMaskAsDisplayFormat = false;
				DisplayFormat.FormatString = displayFormat;
			}
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class DateTimeEdit : DateEdit {
		static DateTimeEdit() {
			RepositoryItemDateTimeEdit.Register();
		}
		private bool canShowPopupOnEnter = true;
		protected override void OnKeyDown(KeyEventArgs e) {
			if(CanShowPopupOnEnter && !IsPopupOpen && e.KeyCode == Keys.Enter) {
				e.Handled = true;
				ShowPopup();
			}
			base.OnKeyDown(e);
		}
		public DateTimeEdit() { }
		public DateTimeEdit(string editMask, string displayFormat) {
			Height = WinPropertyEditor.TextControlHeight;
			((RepositoryItemDateTimeEdit)this.Properties).Init(editMask, displayFormat);
		}
		public override string EditorTypeName { get { return RepositoryItemDateTimeEdit.EditorName; } }
		public bool CanShowPopupOnEnter {
			get { return canShowPopupOnEnter; }
			set { canShowPopupOnEnter = value; }
		}
	}
}
