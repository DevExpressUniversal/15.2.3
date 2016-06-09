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

using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using System;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Grid.EditForm {
	public abstract class EditFormCellDataBase : IEditFormLayoutItem {
		public int Column { get; set; }
		public int Row { get; set; }
		public bool StartNewRow { get; internal set; }
		public EditFormLayoutItemType ItemType { get { return ItemTypeCore; } }
		protected abstract EditFormLayoutItemType ItemTypeCore { get; }
		int columnSpanCore = 1;
		public int ColumnSpan {
			get { return columnSpanCore; }
			set {
				if(columnSpanCore != value) {
					columnSpanCore = CoerceSpanValue(value);
				}
			}
		}
		int rowSpanCore = 1;
		public int RowSpan {
			get { return rowSpanCore; }
			set {
				if(rowSpanCore != value) {
					rowSpanCore = CoerceSpanValue(value);
				}
			}
		}
		static int CoerceSpanValue(int spanValue) {
			return spanValue > 0 ? spanValue : 1;
		}
		internal protected virtual void Assign(EditFormColumnSource source) {
			if(source == null)
				return;
			if(source.ColumnSpan.HasValue)
				ColumnSpan = source.ColumnSpan.Value;
			if(source.RowSpan.HasValue)
				RowSpan = source.RowSpan.Value;
			StartNewRow = source.StartNewRow;
		}
	}
	public class EditFormCellData : EditFormCellDataBase, INotifyPropertyChanged {
		internal const string ValidationErrorPropertyName = "ValidationError";
		public BaseEditSettings EditSettings { get; internal set; }
		public string FieldName { get; internal set; }
		public DataTemplate EditorTemplate { get; internal set; }
		public int VisibleIndex { get; internal set; }
		public EditFormRowData RowData { get; set; }
		public bool ReadOnly { get; internal set; }
		protected override EditFormLayoutItemType ItemTypeCore { get { return EditFormLayoutItemType.Editor; } }
		BaseValidationError validationErrorCore;
		public BaseValidationError ValidationError {
			get { return validationErrorCore; }
			internal set {
				if(validationErrorCore != value) {
					validationErrorCore = value;
					RaisePropertyChanged(ValidationErrorPropertyName);
				}
			}
		}
		internal bool HasInnerError { get; set; }
		object valueCore;
		public object Value {
			get { return valueCore; }
			internal set {
				if(valueCore != value) {
					valueCore = value;
					if(!isValueInited) {
						initialValue = value;
						isValueInited = true;
					}
					RaiseValueChangedEvent();
				}
			}
		}
		object initialValue;
		bool isValueInited;
		internal void ResetValue() {
			if(isValueInited)
				Value = initialValue;
		}
		public delegate void ValueChangedEventHandler(object sender, EventArgs e);
		public event ValueChangedEventHandler ValueChangedEvent;
		protected virtual void RaiseValueChangedEvent() {
			if(ValueChangedEvent != null)
				ValueChangedEvent(this, new EventArgs());
		}
		internal protected override void Assign(EditFormColumnSource source) {
			base.Assign(source);
			FieldName = source.FieldName;
			EditSettings = source.EditSettings;
			EditorTemplate = source.EditorTemplate;
			ReadOnly = source.ReadOnly;
		}
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	public class EditFormCaptionData : EditFormCellDataBase {
		public object Caption { get; internal set; }
		protected override EditFormLayoutItemType ItemTypeCore { get { return EditFormLayoutItemType.Caption; } }
		internal protected override void Assign(EditFormColumnSource source) {
			base.Assign(source);
			Caption = source.Caption;
		}
	}
	public class EditFormColumnSource {
		public object Caption { get; set; }
		public int? ColumnSpan { get; set; }
		public int? RowSpan { get; set; }
		public BaseEditSettings EditSettings { get; set; }
		public DataTemplate EditorTemplate { get; set; }
		public string FieldName { get; set; }
		public bool StartNewRow { get; set; }
		public int VisibleIndex { get; set; }
		public bool ReadOnly { get; set; }
		bool visibleCore = true;
		public bool Visible {
			get { return visibleCore; }
			set {
				if(visibleCore != value)
					visibleCore = value;
			}
		}
	}
}
