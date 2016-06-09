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

using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class NumberingListForm : RichEditDialogBase {
		protected ASPxPageControl PageControl { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			PageControl = new ASPxPageControl();
			PageControl.ClientInstanceName = GetClientInstanceName("NumberingListPageControl");
			PageControl.ContentStyle.Paddings.Padding = Unit.Pixel(0);
			group.Items.CreateItem("NumberingListPageControl", PageControl).ShowCaption = Utils.DefaultBoolean.False;
			var bulletContainer = RenderUtils.CreateDiv("dxreDlgMainContainer", "dxreDlgNumberingList");
			bulletContainer.ClientIDMode = ClientIDMode.Static;
			bulletContainer.ID = RichEdit.ClientID + "_dxeAbstractNumberingList_" + (int)NumberingType.Bullet;
			var simpleContainer = RenderUtils.CreateDiv("dxreDlgMainContainer", "dxreDlgNumberingList");
			simpleContainer.ClientIDMode = ClientIDMode.Static;
			simpleContainer.ID = RichEdit.ClientID + "_dxeAbstractNumberingList_" + (int)NumberingType.Simple;
			var multiLevelContainer = RenderUtils.CreateDiv("dxreDlgMainContainer", "dxreDlgNumberingList");
			multiLevelContainer.ClientIDMode = ClientIDMode.Static;
			multiLevelContainer.ID = RichEdit.ClientID + "_dxeAbstractNumberingList_" + (int)NumberingType.MultiLevel;
			PageControl.AddTab("", "BulletTab", bulletContainer);
			PageControl.AddTab("", "SimpleTab", simpleContainer);
			PageControl.AddTab("", "MultiLevelTab", multiLevelContainer);
		}
		protected override void PopulateBottomItemControls(List<Control> controls) {
			ASPxButton customizeButton = CreateDialogButton("BtnCustomize", ASPxRichEditStringId.Numbering_Customize);
			customizeButton.ClientEnabled = false;
			controls.Add(customizeButton);
			base.PopulateBottomItemControls(controls);
		}
		protected override void Localize() {
			PageControl.LocalizeField("BulletTab", ASPxRichEditStringId.Numbering_Bulleted);
			PageControl.LocalizeField("SimpleTab", ASPxRichEditStringId.Numbering_Numbered);
			PageControl.LocalizeField("MultiLevelTab", ASPxRichEditStringId.Numbering_OutlineNumbered);
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgNumberingListForm";
		}
	}
}
