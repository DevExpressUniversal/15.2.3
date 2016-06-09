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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class SpinButtonCell : ButtonCell {
		private string onDblClickScript = "";
		public SpinButtonCell()
			: base() {
		}
		public SpinButtonCell(string text)
			: base(text) {
		}
		public SpinButtonCell(string text, ImagePropertiesBase image, ImagePosition imagePosition)
			: base(text, image, imagePosition) {
		}
		public SpinButtonCell(string text, ImagePropertiesBase image, ImagePosition imagePosition,
			TemplateContainerBase templateContainer)
			: base(text, image, imagePosition, templateContainer) {
		}
		public string OnDblClickScript {
			get { return onDblClickScript; }
			set { onDblClickScript = value; }
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (Enabled && (OnDblClickScript != ""))
				RenderUtils.SetStringAttribute(this, "ondblclick", OnDblClickScript);
		}
	}
	public class SpinIncDecButton : SpinButtonCell {
		public SpinIncDecButton()
			: base() {
		}
		public SpinIncDecButton(string text)
			: base(text) {
		}
		public SpinIncDecButton(string text, ImagePropertiesBase image, ImagePosition imagePosition)
			: base(text, image, imagePosition) {
		}
		public SpinIncDecButton(string text, ImagePropertiesBase image, ImagePosition imagePosition,
			TemplateContainerBase templateContainer)
			: base(text, image, imagePosition, templateContainer) {
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
	}
	public class SpinEditControl : ButtonEditControl {
		TableCell spinIncDecButtonsCell = null;
		public SpinEditControl(ASPxSpinEditBase editor)
			: base(editor) {
		}
		protected new ASPxSpinEditBase Edit {
			get { return (ASPxSpinEditBase)base.Edit; }
		}
		protected TableCell SpinIncDecButtonsCell {
			get { return spinIncDecButtonsCell; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.spinIncDecButtonsCell = null;
		}
		protected override void CreateButtonCells(ButtonsPosition position, TableRow row) {
			CreateButtonCells(position, button => {
				SpinButtonExtended buttonEx = button as SpinButtonExtended;
				if(buttonEx != null)
					CreateSpinButtonCell(buttonEx, row);
				else
					CreateButtonCell(row, button, GetButtonTemplateContainer(button));
			});
		}
		protected void CreateSpinButtonCell(SpinButtonExtended button, TableRow row) {
			ButtonCell btnCell = null;
			if(button.ButtonKind == SpinButtonKind.Increment || button.ButtonKind == SpinButtonKind.Decrement) {
				if(SpinIncDecButtonsCell == null) {
					this.spinIncDecButtonsCell = RenderUtils.CreateTableCell();
					row.Cells.Add(SpinIncDecButtonsCell);
				}
				btnCell = new SpinIncDecButton(button.Text, Edit.GetButtonImage(button),
					button.ImagePosition, GetButtonTemplateContainer(button));
				SpinIncDecButtonsCell.Controls.Add(btnCell);
			}
			else {
				btnCell = new SpinButtonCell(button.Text, Edit.GetButtonImage(button),
					button.ImagePosition, GetButtonTemplateContainer(button));
				row.Cells.Add(btnCell);
			}
			ButtonCells.Add(button, btnCell);
		}
		protected override void PrepareButton(EditButton button, ButtonCell cell) {
			base.PrepareButton(button, cell);
			SpinButtonCell spinButtonCell = cell as SpinButtonCell;
			if(spinButtonCell != null) {
				spinButtonCell.OnDblClickScript = Edit.GetButtonOnDblClick(button);
				if(cell is SpinIncDecButton && Edit.SpinButtonsInternal.ShowLargeIncrementButtons)
					RenderUtils.AppendDefaultDXClassName(spinButtonCell, EditorStyles.SpinEditHasLargeButtonsSystemClassName);
			}
			RenderUtils.SetPreventSelectionAttribute(cell);
		}
		protected override void PrepareButtonClientSideEventHandlers(EditButton button, ButtonCell cell) {
			if(!Browser.Platform.IsWebKitTouchUI || button is ClearButton)
				base.PrepareButtonClientSideEventHandlers(button, cell);
			else 
				RenderUtils.SetStringAttribute(cell, TouchUtils.TouchMouseUpEventName, Edit.GetButtonOnClick(button));
		}
		protected override void PrepareInputControl(InputControl input) {
			base.PrepareInputControl(input);
			input.ID = ASPxTextEdit.InputControlSuffix;
		}
		protected override bool RequireHideDefaultIEClearButton() {
			return base.RequireHideDefaultIEClearButton() || Edit is ASPxTimeEdit;
		}
		protected TemplateContainerBase GetButtonTemplateContainer(EditButton button) {
			ITemplate buttonTemplate = Edit.GetButtonTemplate();
			TemplateContainerBase templateContainer = null;
			if (buttonTemplate != null) {
				templateContainer = new TemplateContainerBase(button.Index, button);
				buttonTemplate.InstantiateIn(templateContainer);
			}
			return templateContainer;
		}
	}
}
