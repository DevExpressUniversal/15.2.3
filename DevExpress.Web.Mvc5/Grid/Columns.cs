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
using System.Web.Mvc;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc.Internal {
	public class MVCxGridDataColumnAdapter : GridDataColumnAdapter {
		ModelMetadata metadata;
		int columnType = (int)MVCxGridColumnType.Default;
		enum MVCxGridColumnType { Default, TextBox, ButtonEdit, CheckBox, ComboBox, DateEdit, SpinEdit, TimeEdit, ColorEdit, DropDownEdit, Memo,
			BinaryImage, Image, HyperLink, ProgressBar, TokenBox }
		public MVCxGridDataColumnAdapter(IWebGridDataColumnAdapterOwner owner)
			: base(owner) {
		}
		public override string FieldName {
			get { return base.FieldName; }
			set {
				if(FieldName == value) return;
				if(Metadata == null && !String.IsNullOrEmpty(value))
					Metadata = ExtensionsHelper.GetMetadataForColumn(value);
				base.FieldName = value;
			}
		}
		public int ColumnType {
			get { return columnType; }
			set {
				columnType = value;
				ResetPropertiesEdit();
				ExtensionsHelper.ConfigureEditPropertiesByMetadata(PropertiesEdit, Metadata);
			}
		}
		public ModelMetadata Metadata {
			get { return metadata; }
			set {
				if(metadata == value) return;
				metadata = value;
				if(string.IsNullOrEmpty(Column.Caption))
					Column.Caption = Metadata.DisplayName;
				if(!ReadOnly)
					ReadOnly = Metadata.IsReadOnly;
				if(Column.Visible)
					Column.Visible = Metadata.ShowForDisplay;
				ExtensionsHelper.ConfigureEditPropertiesByMetadata(PropertiesEdit, Metadata);
			}
		}
		protected override bool IsPossibleCompareColumnValues() {
			var gridView = Grid as MVCxGridView;
			if(gridView != null && gridView.EnableCustomOperations)
				return true;
			return base.IsPossibleCompareColumnValues();
		}
		protected override EditPropertiesBase CreateEditProperties() {
			switch((MVCxGridColumnType)columnType) {
				case MVCxGridColumnType.TextBox:
					return new TextBoxProperties(Column);
				case MVCxGridColumnType.ButtonEdit:
					return new ButtonEditProperties(Column);
				case MVCxGridColumnType.CheckBox:
					return new CheckBoxProperties(Column);
				case MVCxGridColumnType.ComboBox:
					return new ComboBoxProperties(Column);
				case MVCxGridColumnType.DateEdit:
					return new DateEditProperties(Column);
				case MVCxGridColumnType.SpinEdit:
					return new SpinEditProperties(Column);
				case MVCxGridColumnType.TimeEdit:
					return new TimeEditProperties(Column);
				case MVCxGridColumnType.ColorEdit:
					return new ColorEditProperties(Column);
				case MVCxGridColumnType.DropDownEdit:
					return new DropDownEditProperties(Column);
				case MVCxGridColumnType.Memo:
					return new MemoProperties(Column);
				case MVCxGridColumnType.BinaryImage:
					return new MVCxBinaryImageEditProperties(Column);
				case MVCxGridColumnType.Image:
					return new ImageEditProperties(Column);
				case MVCxGridColumnType.HyperLink:
					return new HyperLinkProperties(Column);
				case MVCxGridColumnType.ProgressBar:
					return new ProgressBarProperties(Column);
				case MVCxGridColumnType.TokenBox:
					return new TokenBoxProperties(Column);
				default:
					return new TextBoxProperties(Column);
			}
		}
		public override void Assign(object source) {
			var src = source as MVCxGridDataColumnAdapter;
			if(src != null)
				ColumnType = src.ColumnType;
				Metadata = src.Metadata;
			base.Assign(source);
		}
	}
}
