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

using System;
using System.Collections;
using System.ComponentModel;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using System.Drawing;
using DevExpress.XtraEditors.Repository;
using System.Globalization;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections.Generic;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class ParameterEditorLine: IDisposable {
		RepositoryItem repositoryItemValue;
		GridView parametersEditorGridView;
		GridControl parametersEditorGridControl;	   
		DashboardParameterViewModel viewModel;
		object parameterValue;
		bool isNull;
		public RepositoryItem RepositoryItemValue { get { return repositoryItemValue; } }
		public string Name {
			get {
				if(String.IsNullOrWhiteSpace(viewModel.Description))
					return viewModel.Name;
				else return viewModel.Description;
			}
		}
		public object IsNull {
			get {
				if(viewModel.AllowNull)
					return isNull;
				else
					return "n/a"; 
			}
			set {
				isNull = (bool)value;
				RaiseValueChanged();
			}
		}
		public object Value {
			get { return isNull ? null : parameterValue; }
			set {
				this.parameterValue = value;
				isNull = value == null;
				RaiseValueChanged();
			}
		}
		public DashboardParameterViewModel ViewModel { get { return viewModel; }  }
		public event EventHandler ValueChanged;
		public ParameterEditorLine(DashboardParameterViewModel parameterViewModel, object parameterValue, XtraGrid.Views.Grid.GridView parametersEditorGridView, XtraGrid.GridControl parametersEditorGridControl) {
			this.viewModel = parameterViewModel;
			this.Value = parameterValue;
			this.parametersEditorGridView = parametersEditorGridView;
			this.parametersEditorGridControl = parametersEditorGridControl;
			GetRepositoryEditor();
		}
		public void Dispose() {
			if(repositoryItemValue != null) {
				repositoryItemValue.Dispose();
				repositoryItemValue.EditValueChanged -= RepositoryItemOnValueChanged;
				repositoryItemValue = null;
			}
		}
		public Parameter GetActualParameter() {
			return new Parameter() {
				Name = ViewModel.Name,
				Type = GetParameterType(),
				Value = viewModel.AllowMultiselect ? Value : ConvertValue(Value),
				AllowNull = ViewModel.AllowNull
			};
		}
		internal RepositoryItem GetRepositoryEditor() {
			if(ViewModel.Values != null && ViewModel.Values.Count > 0) {
				if(ViewModel.AllowMultiselect) {
					repositoryItemValue = new RepositoryItemCheckedComboBoxEdit() {
						PopupFormMinSize = new Size(50, 0),
						PopupFormSize = new Size(150, 0),
						DropDownRows = 15,
						ShowToolTipForTrimmedText = Utils.DefaultBoolean.True,
						ValueMember = "Value",
						EditValueType = EditValueTypeCollection.List,
						ForceUpdateEditValue = DevExpress.Utils.DefaultBoolean.True,
						NullText = string.Empty,
						DisplayMember = ViewModel.ContainsDisplayMember ? "DisplayText" : "Value",
						DataSource = ViewModel.Values,
					};
				}
				else {
					RepositoryItemLookUpEdit repositoryItem = new RepositoryItemLookUpEdit(){
						PopupFormMinSize = new Size(50, 0),
						DropDownRows = 15,
						ShowHeader = false,
						NullText = string.Empty,
						UseDropDownRowsAsMaxCount = true,
						ValueMember = "Value" ,
						DisplayMember = ViewModel.ContainsDisplayMember ? "DisplayText" : "Value",
						DataSource = ViewModel.Values,
					};
					repositoryItem.Columns.Add(new LookUpColumnInfo(ViewModel.ContainsDisplayMember ? "DisplayText" : "Value", 0));
					repositoryItemValue = repositoryItem;
				}
			}
			else if(ViewModel.Type == ParameterType.Bool) {
				repositoryItemValue = new RepositoryItemCheckEdit();
			}
			else if(ViewModel.Type == ParameterType.DateTime) {
				repositoryItemValue = new RepositoryItemDateEdit();
			}
			else if(ViewModel.Type == ParameterType.Int) {
				RepositoryItemSpinEdit repositoryItem = new RepositoryItemSpinEdit();
				if(GetParameterType()==typeof(Int16)){
					repositoryItem.MaxValue = Int16.MaxValue;
					repositoryItem.MinValue = Int16.MinValue;
				}
				else if(GetParameterType()==typeof(Int32)){
					repositoryItem.MaxValue = Int32.MaxValue;
					repositoryItem.MinValue = Int32.MinValue;
				}
				else if(GetParameterType() == typeof(Int64)) {
					repositoryItem.MaxValue = Int64.MaxValue;
					repositoryItem.MinValue = Int64.MinValue;
				}
				repositoryItem.IsFloatValue = false;
				repositoryItemValue = repositoryItem;
			}
			else if(ViewModel.Type == ParameterType.Float) {
				repositoryItemValue = new RepositoryItemSpinEdit(){
					IsFloatValue = true,
					Increment = (decimal)0.1,
				};
			}
			else if(ViewModel.Type == ParameterType.Guid) {
				RepositoryItemTextEdit repositoryItem = new RepositoryItemTextEdit();
				repositoryItem.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
				repositoryItem.Mask.EditMask = "00000000-0000-0000-0000-000000000000";
				repositoryItem.Mask.UseMaskAsDisplayFormat = true;
				repositoryItemValue = repositoryItem;
			}
			else {
				repositoryItemValue = new RepositoryItemTextEdit();
			}
			if(repositoryItemValue.EditorTypeName != "TextEdit")
				repositoryItemValue.EditValueChanged += RepositoryItemOnValueChanged;
			parametersEditorGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemValue });
			return repositoryItemValue;
		}
		void RaiseValueChanged() {
			if(ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}
		object ConvertValue(object value) {
			try {
				return ConvertValue(value, value.GetType(), GetParameterType());
			}
			catch {
				return null;
			}
		}
		object ConvertValue(object value, Type fromType, Type toType) {
			if(fromType == toType)
				return value;
			TypeConverter converter = TypeDescriptor.GetConverter(toType);
			try {
				if(converter.CanConvertFrom(fromType))
					return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			}
			catch { }
			converter = TypeDescriptor.GetConverter(fromType);
			if(converter.CanConvertTo(toType))
				return converter.ConvertTo(null, CultureInfo.InvariantCulture, value, toType);
			return Convert.ChangeType(value, toType, CultureInfo.InvariantCulture);
		}
		Type GetParameterType() {
			switch(ViewModel.Type) {
				case ParameterType.Bool:
					return typeof(bool);
				case ParameterType.DateTime:
					return typeof(DateTime);
				case ParameterType.Int:
					return typeof(Int64);
				case ParameterType.Float:
					return typeof(double);
				case ParameterType.String:
					return typeof(string);
				case ParameterType.Guid:
					return typeof(Guid);
				default:
					throw new NotSupportedException("ParameterType");
			}
		}
		void RepositoryItemOnValueChanged(object sender, EventArgs e) {
			parametersEditorGridView.CloseEditor();
		}
	}
}
