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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public abstract class ColumnChooserControllerBase : ObjectViewController {
		private bool canAddListProperty;
		private bool multiSelect;
		private ColumnChooserExtenderBase columnChooserExtender = null;
		public ColumnChooserControllerBase()
			: base() {
		}
		public bool CanAddListProperty {
			get {
				if(columnChooserExtender != null) {
					return columnChooserExtender.CanAddListProperty;
				}
				else {
					return canAddListProperty;
				}
			}
			set {
				canAddListProperty = value;
				if(columnChooserExtender != null) {
					columnChooserExtender.CanAddListProperty = value;
				}
			}
		}
		public bool MultiSelect {
			get {
				if(columnChooserExtender != null) {
					return columnChooserExtender.MultiSelect;
				}
				else {
					return multiSelect;
				}
			}
			set {
				multiSelect = value;
				if(columnChooserExtender != null) {
					columnChooserExtender.MultiSelect = value;
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(columnChooserExtender != null) {
				columnChooserExtender.CustomAddColumn -= columnChooserExtender_AddColumn;
				columnChooserExtender.CustomRemoveSelectedColumn -= columnChooserExtender_RemoveSelectedColumn;
				columnChooserExtender.GetCustomUsedProperties -= columnChooserExtender_GetCustomUsedProperties;
				columnChooserExtender.CustomDisplayedTypeInfo -= columnChooserExtender_CustomDisplayedTypeInfo;
				columnChooserExtender.CustomActiveListBox -= columnChooserExtender_CustomActiveListBox;
				columnChooserExtender.Dispose();
				columnChooserExtender = null;
			}
		}
		protected ColumnChooserExtenderBase ColumnChooserExtender {
			get {
				if(columnChooserExtender == null) {
					CreateColumnChooserExtenderCore();
				}
				return columnChooserExtender;
			}
		}
		protected void CreateColumnChooserExtenderCore() {
			columnChooserExtender = CreateColumnChooserExtender();
			if(columnChooserExtender != null) {
				columnChooserExtender.CustomAddColumn += columnChooserExtender_AddColumn;
				columnChooserExtender.CustomRemoveSelectedColumn += columnChooserExtender_RemoveSelectedColumn;
				columnChooserExtender.GetCustomUsedProperties += columnChooserExtender_GetCustomUsedProperties;
				columnChooserExtender.CustomDisplayedTypeInfo += columnChooserExtender_CustomDisplayedTypeInfo;
				columnChooserExtender.CustomActiveListBox += columnChooserExtender_CustomActiveListBox;
			}
		}
		protected virtual ColumnChooserExtenderBase CreateColumnChooserExtender() {
			ColumnChooserExtenderBase result = new ColumnChooserExtenderBase(View.Model);
			result.CanAddListProperty = canAddListProperty;
			result.MultiSelect = multiSelect;
			return result;
		}
		protected void CreateButtons() {
			ColumnChooserExtender.CreateButtons();
		}
		protected void InsertButtons() {
			CreateButtons();
			AddButtonsToCustomizationForm();
		}
		protected void DeleteButtons() {
			if(columnChooserExtender != null) {
				columnChooserExtender.DeleteButtons();
			}
		}
		protected virtual void AddButtonsToCustomizationForm() {
			ColumnChooserExtender.AddButtonsToCustomizationForm();
		}
		protected abstract Control ActiveListBox { get; }
		protected abstract List<string> GetUsedProperties();
		protected abstract ITypeInfo DisplayedTypeInfo { get; }
		protected abstract void AddColumn(string propertyName);
		protected abstract void RemoveSelectedColumn();
		protected BaseButton AddButton {
			get { return ColumnChooserExtender.AddButton; }
		}
		protected BaseButton RemoveButton {
			get { return ColumnChooserExtender.RemoveButton; }
		}
		private void columnChooserExtender_RemoveSelectedColumn(object sender, EventArgs e) {
			RemoveSelectedColumn();
		}
		private void columnChooserExtender_AddColumn(object sender, ColumnChooserObserverAddColumnEventArgs e) {
			AddColumn(e.PropertyName);
		}
		private void columnChooserExtender_GetCustomUsedProperties(object sender, GetCustomUsedPropertiesEventArgs e) {
			e.UsedProperties.AddRange(GetUsedProperties());
		}
		private void columnChooserExtender_CustomDisplayedTypeInfo(object sender, CustomDisplayedTypeInfoEventArgs e) {
			e.DisplayedTypeInfo = DisplayedTypeInfo;
		}
		private void columnChooserExtender_CustomActiveListBox(object sender, CustomActiveListBoxEventArgs e) {
			e.ActiveListBox = ActiveListBox;
		}
	}
}
