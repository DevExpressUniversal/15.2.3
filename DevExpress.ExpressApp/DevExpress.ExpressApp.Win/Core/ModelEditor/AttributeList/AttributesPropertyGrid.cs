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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class AttributesPropertyGrid : PropertyGridControl, IModelAttributesPropertyEditorControl {
		private Font underlineFont;
		private int selectedObjectsUpdateLocked = 0;
		private EditorButton navigateButton;
		private ErrorMessages errorMessages_;
		public AttributesPropertyGrid() {
			this.Tag = "testfield=Properties;TESTTABLE=Properties";
			ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			this.OptionsView.MinRowAutoHeight = 20; 
			this.OptionsView.MaxRowAutoHeight = 20;
			AutoGenerateRows = true;
			this.CustomDrawRowHeaderCell += new DevExpress.XtraVerticalGrid.Events.CustomDrawRowHeaderCellEventHandler(AttributesPropertyGrid_CustomDrawRowHeaderCell);
			this.KeyDown += new KeyEventHandler(AttributesPropertyGrid_KeyDown);
			underlineFont = new Font(Font, FontStyle.Underline);
			this.OptionsBehavior.ResizeRowHeaders = false;
			this.OptionsBehavior.ResizeRowValues = false;
			this.OptionsBehavior.UseTabKey = false; 
			this.navigateButton = new EditorButton(ButtonPredefines.Glyph);
			this.navigateButton.ToolTip = "Open Related Object";
			DevExpress.ExpressApp.Utils.ImageInfo navigateImageInfo = ImageLoader.Instance.GetImageInfo("ModelEditor_Action_Open_Object");
			if(navigateImageInfo != null) {
				this.navigateButton.Image = navigateImageInfo.Image;
				navigateButton.Caption = "";
			}
			OptionsBehavior.UseDefaultEditorsCollection = true;
			DefaultEditors.Add(typeof(Color), new RepositoryItemColorEdit());
			DefaultEditors.Add(typeof(Color?), new RepositoryItemColorEdit());
		}
		public void OnNavigateToCalculatedProperty() {
			if(SelectedObject != null) {
				OnModelPropertyEditorActionExecute(ModelPropertyEditorAction.NavigateToCalculatedProperty);
			}
		}
		public void OnNavigateToNode() {
			if(SelectedObject != null) {
				CloseEditor();
				OnModelPropertyEditorActionExecute(ModelPropertyEditorAction.NavigateToNode);
			}
		}
		public void ResetValue() {
			if(FocusedRow != null && SelectedObject != null) {
				CloseEditor();
				OnModelPropertyEditorActionExecute(ModelPropertyEditorAction.ResetProperty);
				CloseEditor();
				DataModeHelper.InvalidateCache();
				OnRowChanged(FocusedRow, Rows, FocusedRow.Properties, RowChangeTypeEnum.Value);
			}
		}
		private void AttributesPropertyGrid_KeyDown(object sender, KeyEventArgs e) {
			if(e.Alt && e.KeyValue == (int)Keys.O) {
					OnNavigateToNode();
			}
			else {
				if(e.Alt && e.KeyValue == (int)Keys.N) {
						OnNavigateToCalculatedProperty();
				}
				if(e.Control && e.KeyValue == (int)Keys.Z) {
						ResetValue();
				}
			}
		}
		private void AttributesPropertyGrid_CustomDrawRowHeaderCell(object sender, DevExpress.XtraVerticalGrid.Events.CustomDrawRowHeaderCellEventArgs e) {
			ModelPropertyEditorCellProperty style = OnCalculateCellProperty(e.Row.Properties.FieldName);
			if(style == ModelPropertyEditorCellProperty.Default) {
				return;
			}
			if((style & ModelPropertyEditorCellProperty.Localization) == ModelPropertyEditorCellProperty.Localization) {
			}
			else {
				if((style & ModelPropertyEditorCellProperty.CalculateProperty | ModelPropertyEditorCellProperty.Required) == (ModelPropertyEditorCellProperty.CalculateProperty | ModelPropertyEditorCellProperty.Required)) {
					e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic | FontStyle.Bold | FontStyle.Underline);
					e.Appearance.ForeColor = GetHyperlinkTextColor(e.Focused);
				}
				else {
					if((style & ModelPropertyEditorCellProperty.CalculateProperty) == ModelPropertyEditorCellProperty.CalculateProperty) {
						e.Appearance.Font = underlineFont;
						e.Appearance.ForeColor = GetHyperlinkTextColor(e.Focused);
					}
					else {
						if((style & ModelPropertyEditorCellProperty.Required) == ModelPropertyEditorCellProperty.Required) {
							e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic | FontStyle.Bold);
						}
					}
				}
			}
		}
		private Color GetHyperlinkTextColor(bool cellFocused) {
			Color color = Color.Empty;
			if(!cellFocused) {
				if(LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin) {
					Skin skin = EditorsSkins.GetSkin(LookAndFeel);
					if(skin != null) {
						color = skin.Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor);
					}
				}
			}
			else {
				color = ViewInfo.PaintAppearance.FocusedRow.ForeColor;
			}
			if(color.IsEmpty) color = Color.Blue;
			return color;
		}
		private void AttributesPropertyGrid_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if(FocusedRow != null && SelectedObject != null) {
				if(e.Button.Kind == ButtonPredefines.Undo) {
					ResetValue();
				}
				else {
					if(e.Button.Kind == ButtonPredefines.Glyph) {
						OnNavigateToNode();
					}
				}
			}
		}
		private void Property_EditValueChanging(object sender, ChangingEventArgs e) {
			if(e.NewValue is string && e.NewValue.ToString() == CaptionHelper.NoneValue) {
				e.NewValue = null;
			}
		}
		private void result_Disposed(object sender, EventArgs e) {
			if(sender is RepositoryItemButtonEdit) {
				((RepositoryItemButtonEdit)sender).ButtonClick -= new ButtonPressedEventHandler(AttributesPropertyGrid_ButtonClick);
			}
			if(sender is RepositoryItemComboBox) {
				((RepositoryItemComboBox)sender).EditValueChanging -= new ChangingEventHandler(Property_EditValueChanging);
			}
			((RepositoryItem)sender).Disposed -= new EventHandler(result_Disposed);
		}
		protected override void OnActiveEditor_ValueChanging(object sender, ChangingEventArgs e) {
			base.OnActiveEditor_ValueChanging(sender, e);
			e.Cancel = OnPropertyChanging(FocusedRow.Properties.FieldName);
		}
		protected override BaseRow CreateEditorRow(string propertyName) {
			BaseRow result = base.CreateEditorRow(propertyName);
			RowProperties prop = result.Properties;
			if(SelectedObject != null) {
				prop.ReadOnly = (OnCalculateCellProperty(prop.FieldName) & ModelPropertyEditorCellProperty.ReadOnly) == ModelPropertyEditorCellProperty.ReadOnly;
				int imageIndex = OnCalculateItemImageIndex(prop.FieldName);
				if(imageIndex != -1) {
					prop.ImageIndex = imageIndex;
				}
			}
			return result;
		}
		protected override RepositoryItem CreateDefaultRowEdit(RowProperties p) {
			RepositoryItem result = base.CreateDefaultRowEdit(p);
			try {
				OnCustomFillRepositoryItem(ref result, p.FieldName);
			}
			catch(Exception ex) {
				OnHandleException(ex);
			}
			RepositoryItemButtonEdit edit = null; ;
			if(result is RepositoryItemButtonEdit) {
				edit = (RepositoryItemButtonEdit)result;
			}
			else {
				edit = new RepositoryItemButtonEdit();
				edit.ReadOnly = p.ReadOnly.HasValue ? p.ReadOnly.Value : p.IsSourceReadOnly;
				edit.Buttons.Clear();
				result = edit;
			}
			if(p.Row != FocusedRow) {
				foreach(EditorButton button in edit.Buttons) {
					button.Visible = false;
				}
				if((OnCalculateCellProperty(p.FieldName) & ModelPropertyEditorCellProperty.RefProperty) == ModelPropertyEditorCellProperty.RefProperty) {
					edit.Buttons.Add(navigateButton);
				}
			}
			else {
				if((OnCalculateCellProperty(p.FieldName) & ModelPropertyEditorCellProperty.RefProperty) == ModelPropertyEditorCellProperty.RefProperty) {
					edit.Buttons.Add(navigateButton);
				}
				foreach(EditorButton button in edit.Buttons) {
					button.Visible = true;
				}
			}
			if(result is RepositoryItemButtonEdit) {
				((RepositoryItemButtonEdit)result).ButtonClick += new ButtonPressedEventHandler(AttributesPropertyGrid_ButtonClick);
			}
			if(result is RepositoryItemComboBox) {
				((RepositoryItemComboBox)result).EditValueChanging += new ChangingEventHandler(Property_EditValueChanging);
			}
			result.Disposed += new EventHandler(result_Disposed);
			return result;
		}
		protected override void RowDoubleClick(BaseRow row) {
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			VGridHitInfo hitInfo = this.CalcHitInfo(e.Location);
			if(hitInfo.HitInfoType == HitInfoTypeEnum.HeaderCell && hitInfo.Row == FocusedRow) {
				OnMouseUp(e);
				OnNavigateToCalculatedProperty();
			}
			else {
				base.OnMouseDown(e);
			}
		}
		public override bool IsCellDefaultValue(RowProperties props, int recordIndex) {
			return (OnCalculateCellProperty(props.FieldName) & ModelPropertyEditorCellProperty.ModifiedProperty) != ModelPropertyEditorCellProperty.ModifiedProperty;
		}
		public override void CloseEditor() {
			string focusedRow = null;
			if(FocusedRow != null) {
				focusedRow = FocusedRow.Properties.FieldName;
			}
			try {
				base.CloseEditor();
			}
			finally {
				if(!string.IsNullOrEmpty(focusedRow)) {
					OnPropertyChanged(focusedRow);
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(selectedObjectsUpdateLocked == 0) {
				base.OnPaint(e);
			}
		}
		public virtual ModelPropertyEditorCellProperty OnCalculateCellProperty(string propertyName) {
			if(CalculateCellProperty != null) {
				CalculateCellPropertyEventArgs calculateCellStyleEventArgs = new CalculateCellPropertyEventArgs(SelectedObjects, propertyName);
				CalculateCellProperty(this, calculateCellStyleEventArgs);
				return calculateCellStyleEventArgs.CellStyle;
			}
			return ModelPropertyEditorCellProperty.Default;
		}
		protected virtual bool OnPropertyChanging(string propertyName) {
			if(PropertyChanging != null) {
				PropertyChangingEventArgs modelNodeEventArgs = new PropertyChangingEventArgs(FocusedRow.Properties.FieldName);
				PropertyChanging(this, modelNodeEventArgs);
				return modelNodeEventArgs.Cancel;
			}
			return false;
		}
		protected virtual void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChangingEventArgs modelNodeEventArgs = new PropertyChangingEventArgs(FocusedRow.Properties.FieldName);
				PropertyChanged(this, modelNodeEventArgs);
			}
		}
		protected virtual int OnCalculateItemImageIndex(string propertyName) {
			if(CalculateItemImageIndex != null) {
				CalculateItemImageIndexEventArgs e = new CalculateItemImageIndexEventArgs((ModelNode)SelectedObject, propertyName);
				CalculateItemImageIndex(this, e);
				return e.ImageIndex;
			}
			return -1;
		}
		protected virtual void OnCustomFillRepositoryItem(ref RepositoryItem result, string propertyName) {
			if(SelectedObject != null && CustomFillRepositoryItem != null) {
				CustomFillRepositoryItemEventArgs customFillRepositoryItemEventArgs = new CustomFillRepositoryItemEventArgs((ModelNode)SelectedObject, propertyName, result);
				CustomFillRepositoryItem(this, customFillRepositoryItemEventArgs);
				result = customFillRepositoryItemEventArgs.RepositoryItem;
			}
		}
		protected virtual void OnModelPropertyEditorActionExecute(ModelPropertyEditorAction action) {
			if(ModelPropertyEditorActionExecute != null) {
				ModelPropertyEditorActionExecute(this, new ModelPropertyEditorActionExecuteEventArgs(action));
			}
		}
		protected virtual void OnHandleException(Exception e) {
			if(HandleException != null) {
				HandleException(this, new CustomHandleExceptionEventArgs(e));
			}
		}
#if DebugTest
		public RepositoryItem DebugTest_CreateDefaultRowEdit(RowProperties p) {
			return CreateDefaultRowEdit(p);
		}
		public ErrorMessages DebugTest_ErrorMessages {
			get { return errorMessages_; }
		}
#endif
		#region IModelAttributesPropertyEditor Members
		public void SetImages(ImageCollection imageCollection) {
			this.ImageList = imageCollection;
		}
		public void SetErrorMessages(ErrorMessages errorMessages) {
			this.errorMessages_ = errorMessages;
			if(errorMessages_ != null && !errorMessages_.IsEmpty) {
				foreach(BaseRow row in VisibleRows) {
					var message = errorMessages_.GetMessage(row.Properties.FieldName, SelectedObject);
					if(message != null) {
						this.SetRowError(row.Properties, message.Message, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
					}
				}
			}
			else {
				this.ErrorInfo.ClearErrors();
			}
		}
		public string FocusedPropertyName {
			get { return FocusedRow != null ? FocusedRow.Properties.FieldName : string.Empty; }
			set {
				Focus();
				FocusedRow = VisibleRows[value];
				if(CustomFocusedPropertyChanged != null) {
					CustomFocusedPropertyChanged(this, EventArgs.Empty);
				}
			}
		}
		public bool EditorErrorState {
			get {
				return ActiveEditor != null ? string.IsNullOrEmpty(ActiveEditor.ErrorText) : true;
			}
		}
		public void RefreshValues() {
			DataModeHelper.InvalidateCache();
			Refresh();
		}
		public void BeginSelectedObjectsUpdate() {
			selectedObjectsUpdateLocked++;
		}
		public void EndSelectedObjectsUpdate() {
			selectedObjectsUpdateLocked--;
		}
		private Dictionary<ModelNode, ModelTreeListNode> selectedObjectsCore = new Dictionary<ModelNode, ModelTreeListNode>();
		ModelTreeListNode IModelAttributesPropertyEditorControl.SelectedObject {
			get {
				if(selectedObjectsCore.Count > 0) {
					List<ModelTreeListNode> selectedNodes = new List<ModelTreeListNode>();
					foreach(KeyValuePair<ModelNode, ModelTreeListNode> item in selectedObjectsCore) {
						selectedNodes.Add(item.Value);
					}
					return selectedNodes[0];
				}
				else {
					return null;
				}
			}
			set {
				try {
					BeginSelectedObjectsUpdate();
					selectedObjectsCore.Clear();
					if(value != null) {
						selectedObjectsCore[value.ModelNode] = value;
						base.SelectedObject = value.ModelNode;
					}
					else {
						base.SelectedObject = null;
					}
				}
				finally {
					EndSelectedObjectsUpdate();
				}
			}
		}
		ModelTreeListNode[] IModelAttributesPropertyEditorControl.SelectedObjects {
			get {
				List<ModelTreeListNode> selectedNodes = new List<ModelTreeListNode>();
				foreach(KeyValuePair<ModelNode, ModelTreeListNode> item in selectedObjectsCore) {
					selectedNodes.Add(item.Value);
				}
				return selectedNodes.ToArray();
			}
			set {
				try {
					selectedObjectsCore.Clear();
					BeginSelectedObjectsUpdate();
					if(value != null) {
						List<ModelNode> selectedNodes = new List<ModelNode>();
						foreach(ModelTreeListNode selectedTreeListNode in value) {
							selectedNodes.Add(selectedTreeListNode.ModelNode);
							selectedObjectsCore[selectedTreeListNode.ModelNode] = selectedTreeListNode;
						}
						base.SelectedObjects = selectedNodes.ToArray();
					}
					else {
						base.SelectedObjects = value;
					}
				}
				finally {
					EndSelectedObjectsUpdate();
				}
			}
		}
		public event EventHandler<CustomFillRepositoryItemEventArgs> CustomFillRepositoryItem;
		public event EventHandler<PropertyChangingEventArgs> PropertyChanging;
		public event EventHandler<PropertyChangingEventArgs> PropertyChanged;
		public event EventHandler<CalculateCellPropertyEventArgs> CalculateCellProperty;
		public event EventHandler<CalculateItemImageIndexEventArgs> CalculateItemImageIndex;
		public event EventHandler<ModelPropertyEditorActionExecuteEventArgs> ModelPropertyEditorActionExecute;
		public event EventHandler<CustomHandleExceptionEventArgs> HandleException;
		internal event EventHandler CustomFocusedPropertyChanged;
		#endregion
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			navigateButton = null;
		}
	}
}
