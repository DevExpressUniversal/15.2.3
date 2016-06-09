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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using System.ComponentModel;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.XtraEditors.ViewInfo;
using System.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid.FilterDropDown {
	[ToolboxItem(false)]
	public class SearchBox : ComboBoxEdit {
		internal static EditorClassInfo CreateEditorClassInfo(EditorClassInfo baseInfo) {
			EditorClassInfo classInfo = new EditorClassInfo("SearchBox", typeof(SearchBox),
				typeof(RepositoryItemSearchBox), baseInfo.ViewInfoType,
				baseInfo.Painter, baseInfo.DesignTimeVisible);
			return classInfo;
		}
		EditorClassInfo classInfo;
		ActionDelayer saveDelayer;
		public SearchBox()
			: base() {
			UpdateButtonsVisibility();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.saveDelayer != null)
					this.saveDelayer.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override EditorClassInfo EditorClassInfo {
			get {
				if(classInfo == null)
					classInfo = CreateEditorClassInfo(base.EditorClassInfo);
				return classInfo;
			}
		}
		protected ActionDelayer SaveDelayer {
			get {
				if(saveDelayer == null)
					saveDelayer = new ActionDelayer(SaveEditValue);
				return saveDelayer;
			}
		}
		public new RepositoryItemSearchBox Properties { get { return (RepositoryItemSearchBox)base.Properties; } }
		[
		RefreshProperties(RefreshProperties.All), Category(CategoryName.Data)
		]
		public override object EditValue {
			get { return base.EditValue; }
			set {
				if(!(value is string) && value != null)
					return;
				base.EditValue = value;
				UpdateButtonsVisibility();
			}
		}
		public string SearchText {
			get { return (string)EditValue; }
			set { EditValue = value; }
		}
		protected virtual void UpdateButtonsVisibility() {
			Properties.ResetButton.Visible = !string.IsNullOrEmpty(SearchText);
			Properties.DropDownButton.Visible = Properties.Items.Count > 0;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(ee.Handled) {
				base.OnMouseDown(ee);
				return;
			}
			EditHitInfo hitInfo = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
			if(e.Button == MouseButtons.Left && hitInfo.HitTest == EditHitTest.Button) {
				EditorButtonObjectInfoArgs args = hitInfo.HitObject as EditorButtonObjectInfoArgs;
				if(OnButtonClick(args))
					ee.Handled = true;
			}
			base.OnMouseDown(ee);
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.Handled) return;
			switch(e.KeyData) {
				case Keys.Escape:
					OnResetButtonClick();
					e.Handled = true;
					break;
			}
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			SaveDelayer.Invoke(Properties.MruSaveDelay);
		}
		protected virtual void SaveEditValue() {
			string editValue = Convert.ToString(EditValue).Trim();
			if(!string.IsNullOrEmpty(editValue) && !Properties.Items.Contains(editValue))
				Properties.Items.Insert(0, editValue);
		}
		protected virtual bool OnButtonClick(EditorButtonObjectInfoArgs args) {
			if(args != null && args.Button == Properties.ResetButton) {
				OnResetButtonClick();
				return true;
			}
			return false;
		}
		protected virtual void OnResetButtonClick() {
			SearchText = null;
		}
		internal void OnMruSaveDelayChanged() {
			if(this.saveDelayer != null)
				this.saveDelayer.Dispose();
			this.saveDelayer = null;
		}
	}
	public class RepositoryItemSearchBox : RepositoryItemComboBox {
		EditorClassInfo classInfo;
		int mruSaveDelay;
		public RepositoryItemSearchBox()
			: base() {
			Buttons.Insert(0, new EditorButton(ButtonPredefines.Delete));
			ImmediatePopup = true;
			this.mruSaveDelay = 5000;
		}
		protected new ComboBoxItemCollection Items { get { return base.Items; } }
		protected new EditorButtonCollection Buttons { get { return base.Buttons; } }
		[Browsable(false)]
		public new SearchBox OwnerEdit { get { return (SearchBox)base.OwnerEdit; } }
		public virtual EditorButton ResetButton { get { return Buttons[0]; } }
		public virtual EditorButton DropDownButton { get { return Buttons[1]; } }
		[XtraSerializableProperty]
		public int MruSaveDelay {
			get { return mruSaveDelay; }
			set {
				mruSaveDelay = value;
				OwnerEdit.OnMruSaveDelayChanged();
			}
		}
		public override int ActionButtonIndex {
			get { return 1; }
			set { throw new ArgumentException("This property can't be edited"); }
		}
		public override string NullValuePrompt {
			get { return PivotGridLocalizer.GetString(PivotGridStringId.SearchBoxText); }
			set { throw new ArgumentException("This property can't be edited"); }
		}
		public override bool NullValuePromptShowForEmptyValue {
			get { return true; }
			set { throw new ArgumentException("This property can't be edited"); }
		}
		protected override EditorClassInfo EditorClassInfo {
			get {
				if(classInfo == null)
					classInfo = SearchBox.CreateEditorClassInfo(base.EditorClassInfo);
				return classInfo;
			}
		}
	}
}
