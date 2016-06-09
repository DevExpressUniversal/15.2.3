#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.ComponentModel;
using DevExpress.Accessibility;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPivotGrid;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	class DataItemFilterRepositoryItemComboBox : RepositoryItemComboBox {
		public override string EditorTypeName { get { return DataItemFilterComboBoxEdit.EditorName; } }
		public FormatterBase Formatter { get; set; }
		public DataItemFilterRepositoryItemComboBox(FormatterBase formatter)
			: this(formatter, null) {
		}
		public DataItemFilterRepositoryItemComboBox(FormatterBase formatter, DataItemFilterComboBoxEdit ownerEdit) {
			Formatter = formatter;
			if(ownerEdit != null)
				SetOwnerEdit(ownerEdit);
		}
		public override BaseEdit CreateEditor() {
			return new DataItemFilterComboBoxEdit(Formatter);
		}
		public override BaseEditPainter CreatePainter() {
			return new ButtonEditPainter();
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new ComboBoxViewInfo(this);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(Formatter != null) {
				FilterItem item = editValue as FilterItem;
				return item != null ? item.DisplayText : Formatter.Format(editValue);
			}
			return base.GetDisplayText(format, editValue);
		}
		protected override Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new ComboBoxEditAccessible(this);
		}
	}
	[DXToolboxItem(false)]
	class DataItemFilterComboBoxEdit : ComboBoxEdit {
		public const string EditorName = "DimensionFilterComboBoxEdit";
		readonly FormatterBase formatter;
		public override string EditorTypeName { get { return EditorName; } }
		public override object EditValue {
			get { return base.EditValue; }
			set {
				FilterItem filterItem = value as FilterItem;
				base.EditValue = filterItem == null ? value : filterItem.Value;
			}
		}
		public DataItemFilterComboBoxEdit(FormatterBase formatter) {
			this.formatter = formatter;
			DataItemFilterRepositoryItemComboBox repositoryItem = Properties as DataItemFilterRepositoryItemComboBox;
			if(repositoryItem != null)
				repositoryItem.Formatter = formatter;
		}
		protected override void CreateRepositoryItem() {
			fProperties = new DataItemFilterRepositoryItemComboBox(formatter, this);
		}
	}
}
