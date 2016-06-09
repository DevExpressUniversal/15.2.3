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

using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxPivotGrid.Html {
	interface ICheckBoxWrapper {
		void SetChecked(bool? isChecked);
		void SetEnabled(bool isEnabled);
		void AddValueChangedHandler(string handler);
		string ID { get; set; }
		string Text { get; set; }
		Control Control { get; }
	}
	abstract class CheckBoxWrapper<T> : ICheckBoxWrapper
	where T : WebControl {
		readonly T checkBox;
		public CheckBoxWrapper(T checkBox) {
			this.checkBox = checkBox;
		}
		protected T CheckBox { get { return checkBox; } }
		public Control Control { get { return CheckBox; } }
		public string ID {
			get { return CheckBox.ID; }
			set { CheckBox.ID = value; }
		}
		public abstract string Text { get; set; }
		public abstract void SetChecked(bool? isChecked);
		public abstract void SetEnabled(bool isEnabled);
		public abstract void AddValueChangedHandler(string handler);
	}
	class ASPxCheckBoxWrapper : CheckBoxWrapper<ASPxCheckBox> {
		public ASPxCheckBoxWrapper(ASPxCheckBox checkBox) : base(checkBox) {
			checkBox.AllowGrayedByClick = false;
			checkBox.EncodeHtml = false;
		}
		public override string Text {
			get { return CheckBox.Text; }
			set { CheckBox.Text = value; }
		}
		public override void SetChecked(bool? isChecked) {
			if(isChecked == null)
				CheckBox.CheckState = CheckState.Indeterminate;
			else
				CheckBox.CheckState = isChecked.Value ? CheckState.Checked : CheckState.Unchecked;
		}
		public override void SetEnabled(bool isEnabled) {
			CheckBox.Enabled = isEnabled;
		}
		public override void AddValueChangedHandler(string handler) {
			CheckBox.ClientSideEvents.ValueChanged = string.Format("function(s, e) {{ {0} }}", handler);
		}
	}
	class NativeCheckBoxWrapper : CheckBoxWrapper<CheckBox> {
		public NativeCheckBoxWrapper(CheckBox checkBox) : base(checkBox) { }
		public override string Text {
			get { return CheckBox.Text; }
			set { CheckBox.Text = value; }
		}
		public override void SetChecked(bool? isChecked) {
			CheckBox.Checked = isChecked.HasValue ? isChecked.Value : false;
		}
		public override void SetEnabled(bool isEnabled) {
			CheckBox.Enabled = isEnabled;
		}
		public override void AddValueChangedHandler(string handler) {
			CheckBox.Attributes.Add("onclick", handler);
		}
	}
	class CheckBoxFactory {
		ASPxPivotGrid pivotGrid;
		bool isNative;
		public CheckBoxFactory(ASPxPivotGrid pivotGrid, bool isNative) {
			this.pivotGrid = pivotGrid;
			this.isNative = isNative;
		}
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected bool IsNative { get { return isNative; } }
		public ICheckBoxWrapper CreateCheckBox() {
			return IsNative ? CreateNativeCheckBox() : CreateASPxCheckBox();
		}
		public ICheckBoxWrapper CreateShowAllCheckBox() {
			ICheckBoxWrapper wrapper = CreateCheckBox();
			ASPxCheckBox aspxCheckBox = wrapper.Control as ASPxCheckBox;
			if(aspxCheckBox != null)
				aspxCheckBox.AllowGrayed = true;
			return wrapper;
		}
		ICheckBoxWrapper CreateNativeCheckBox() {
			return new NativeCheckBoxWrapper(new CheckBox());
		}
		ICheckBoxWrapper CreateASPxCheckBox() {
			ASPxCheckBox checkBox = new ASPxCheckBox();
			checkBox.ParentSkinOwner = PivotGrid;
			return new ASPxCheckBoxWrapper(checkBox);
		}
	}
}
