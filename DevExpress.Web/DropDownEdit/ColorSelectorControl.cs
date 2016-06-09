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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.DropDownEdit {
	public class ColorSelectorControl : ASPxInternalWebControl {
		protected internal static string ColorPickerIdPostfix = "CP";
		protected internal static string OkButtonIdPostfix = "OB";
		protected internal static string CancelButtonIdPostfix = "CB";
		public ColorSelectorControl(ColorNestedControl colorNestedControl)
			: base() {
			ColorNestedControl = colorNestedControl;
		}
		protected WebControl MainDiv { get; private set; }
		protected ColorPicker ColorPicker { get; private set; }
		protected WebControl ButtonsPanelDiv { get; private set; }
		protected WebControl CancelButton { get; private set; }
		protected WebControl OkButton { get; private set; }
		protected ColorNestedControl ColorNestedControl { get; private set; }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MainDiv = null;
			ColorPicker = null;
			ButtonsPanelDiv = null;
			OkButton = null;
			CancelButton = null;
		}
		protected override void CreateControlHierarchy() {
			MainDiv = RenderUtils.CreateDiv();
			MainDiv.ID = ColorNestedControl.GetColorSelectorMainDivID();
			Controls.Add(MainDiv);
			CreateColorPicker();
			CreateButtonsPanel();
		}
		protected virtual void CreateColorPicker() {
			ColorPicker = new ColorPicker();
			ColorPicker.ID = ColorPickerIdPostfix;
			ColorPicker.ParentSkinOwner = ColorNestedControl;
			MainDiv.Controls.Add(ColorPicker);
		}
		protected virtual void CreateButtonsPanel() {
			ButtonsPanelDiv = RenderUtils.CreateDiv();
			OkButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			OkButton.Attributes.Add("type", "button");
			OkButton.Attributes.Add("value", ColorNestedControl.Properties.OkButtonText);
			CancelButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			CancelButton.Attributes.Add("type", "button");
			CancelButton.Attributes.Add("value", ColorNestedControl.Properties.CancelButtonText);
			ButtonsPanelDiv.Controls.Add(OkButton);
			ButtonsPanelDiv.Controls.Add(CancelButton);
			MainDiv.Controls.Add(ButtonsPanelDiv);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareButtonsPanelDiv();
			ColorNestedControl.GetColorSelectorMainDivStyle().AssignToControl(MainDiv);
		}
		protected virtual void PrepareButtonsPanelDiv() {
			ColorNestedControl.GetButtonsPanelDivStyle().AssignToControl(ButtonsPanelDiv);
			OkButton.ID = OkButtonIdPostfix;
			CancelButton.ID = CancelButtonIdPostfix;
			ColorNestedControl.GetOkButtonStyle().AssignToControl(OkButton);
			ColorNestedControl.GetOkButtonStyle().AssignToControl(CancelButton);
		}
	}
}
