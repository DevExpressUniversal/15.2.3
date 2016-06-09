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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Accessibility;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.ExpressApp.Win.Editors {
	public class IntegerPropertyEditor : DXPropertyEditor {
		private void SetupMaskType(RepositoryItemTextEdit item) {
			if(EditMaskType == EditMaskType.RegEx) {
				item.Mask.MaskType = MaskType.RegEx;
				item.Mask.UseMaskAsDisplayFormat = false;
			}
			else {
				item.Mask.MaskType = MaskType.Numeric;
				item.Mask.UseMaskAsDisplayFormat = true;
			}
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			RepositoryItemIntegerEdit repositoryItemIntegerEdit = (RepositoryItemIntegerEdit)item;
			SetupMaskType(repositoryItemIntegerEdit);
			repositoryItemIntegerEdit.Init(EditMask, DisplayFormat);
			repositoryItemIntegerEdit.AllowNullInput = AllowNull ? DefaultBoolean.True : DefaultBoolean.False;
			CreateRestrictions(item);
		}
		protected virtual void CreateRestrictions(RepositoryItem item) {
			RepositoryItemSpinEdit repositoryItemSpinEdit = item as RepositoryItemSpinEdit;
			if(repositoryItemSpinEdit != null) {
				Type memberType = GetUnderlyingType();
				FieldInfo maxValueField = memberType.GetField("MaxValue");
				if(maxValueField != null) {
					repositoryItemSpinEdit.MaxValue = (Decimal)Convert.ChangeType(maxValueField.GetValue(0), typeof(Decimal));
				}
				FieldInfo minValueField = memberType.GetField("MinValue");
				if(minValueField != null) {
					repositoryItemSpinEdit.MinValue = (Decimal)Convert.ChangeType(minValueField.GetValue(0), typeof(Decimal));
				}
			}
		}
		protected override object CreateControlCore() {
			return new IntegerEdit();
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemIntegerEdit();
		}
		public IntegerPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		public new SpinEdit Control {
			get { return (SpinEdit)base.Control; }
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
	}
	public class RepositoryItemIntegerEdit : RepositoryItemSpinEdit {
		protected internal const string EditorName = "IntegerEdit";
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(IntegerEdit),
					typeof(RepositoryItemIntegerEdit), typeof(BaseSpinEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.SpinEdit, typeof(BaseSpinEditAccessible)));
			}
		}
		static RepositoryItemIntegerEdit() {
			RepositoryItemIntegerEdit.Register();
		}
		public RepositoryItemIntegerEdit(string editMask, string displayFormat)
			: this() {
			Init(editMask, displayFormat);
		}
		public RepositoryItemIntegerEdit() {
			IsFloatValue = false;
			Mask.MaskType = MaskType.Numeric;
			Mask.UseMaskAsDisplayFormat = true;
			DisplayFormat.FormatType = FormatType.Numeric;
			fEditValueChangedFiringMode = EditValueChangedFiringMode.Default;
		}
		public virtual void Init(string editMask, string displayFormat) {
			if(!String.IsNullOrEmpty(editMask)) {
				Mask.EditMask = editMask;
			}
			if(!String.IsNullOrEmpty(displayFormat)) {
				Mask.UseMaskAsDisplayFormat = false;
				DisplayFormat.FormatString = displayFormat;
			}
		}
		public override string EditorTypeName { get { return EditorName; } }
	}
	[ToolboxItem(false)]
	public class IntegerEdit : SpinEdit {
		static IntegerEdit() {
			RepositoryItemIntegerEdit.Register();
		}
		public IntegerEdit() { }
		public IntegerEdit(string editMask, string displayFormat) {
			Height = WinPropertyEditor.TextControlHeight;
			((RepositoryItemIntegerEdit)this.Properties).Init(editMask, displayFormat);
		}
		public override string EditorTypeName { get { return RepositoryItemIntegerEdit.EditorName; } }
	}
}
