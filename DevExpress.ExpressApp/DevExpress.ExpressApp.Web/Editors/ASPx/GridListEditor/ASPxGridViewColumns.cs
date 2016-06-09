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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	internal interface IActionHolder {
		ActionBase Action { get; }
	}
	public class XafGridViewCommandColumn : GridViewCommandColumn {
		public XafGridViewCommandColumn() {
			AllowDragDrop = DevExpress.Utils.DefaultBoolean.False;
		}
	}
	public class GridViewDataActionUrlColumn : GridViewDataHyperLinkColumn, IActionHolder, IDisposable {
		private ActionUrl actionUrl;
		public GridViewDataActionUrlColumn(ActionUrl actionUrl)
			: base() {
			this.Settings.AllowDragDrop = DevExpress.Utils.DefaultBoolean.False;
			this.actionUrl = actionUrl;
			Caption = actionUrl.Caption;
			PropertiesHyperLinkEdit.NavigateUrlFormatString = actionUrl.UrlFormatString;
			PropertiesHyperLinkEdit.Target = (actionUrl.OpenInNewWindow ? "_blank" : "");
			PropertiesHyperLinkEdit.TextField = actionUrl.TextFieldName;
			PropertiesHyperLinkEdit.TextFormatString = actionUrl.TextFormatString;
			FieldName = actionUrl.UrlFieldName;
		}
		public void Dispose() {
			actionUrl = null;
		}
		#region IActionHolder Members
		public ActionBase Action {
			get { return actionUrl; }
		}
		#endregion
	}
	public delegate void ProcessCommandDelegate(string commandArgs);
	public class GridViewDataActionColumn : GridViewCommandColumn, IActionHolder, IDisposable {
		private const int paddingValue = 7;
		private ActionBase action;
		private void SetAction(ActionBase action) {
			this.action = action;
			if(action != null) {
				GridViewCommandColumnCustomButton gridViewCommandColumnCustomButton = new GridViewCommandColumnCustomButton();
				ImageInfo imageInfo = ImageInfo.Empty;
				if(action.PaintStyle != ExpressApp.Templates.ActionItemPaintStyle.Caption && action.HasImage) {
					imageInfo = ImageLoader.Instance.GetImageInfo(action.ImageName);
				}
				if(!imageInfo.IsUrlEmpty) {
					this.ButtonType = GridCommandButtonRenderMode.Image;
					Width = imageInfo.Width + paddingValue * 2;
					gridViewCommandColumnCustomButton.Image.Url = imageInfo.ImageUrl;
					gridViewCommandColumnCustomButton.Image.ToolTip = action.ToolTip;
				}
				else {
					Width = Unit.Pixel(110);
					this.ButtonType = GridCommandButtonRenderMode.Link;
				}
				gridViewCommandColumnCustomButton.Text = DisplayValueHelper.GetHtmlDisplayText(action.Caption);
				CustomButtons.Add(gridViewCommandColumnCustomButton);
			}
		}
		public GridViewDataActionColumn() {
			this.AllowDragDrop = DevExpress.Utils.DefaultBoolean.False;
			ButtonType = GridCommandButtonRenderMode.Image;
			Caption = " ";
		}
		public GridViewDataActionColumn(ActionBase action) : this(){
			SetAction(action);
		}
		public void Dispose() {
			action = null;
		}
		#region IActionHolder Members
		public ActionBase Action {
			get { return action; }
			set {
				SetAction(value);
			}
		}
		#endregion
	}
	public class ASPxGridViewColumnModelSynchronizer : ColumnWrapperModelSynchronizer {
		public ASPxGridViewColumnModelSynchronizer(ColumnWrapper gridColumn, IModelColumn model, ColumnsListEditor listEditor)
			: base(gridColumn, model, listEditor) {
		}
		public ASPxGridViewColumnModelSynchronizer(ColumnWrapper gridColumn, IModelColumn model, CollectionSourceDataAccessMode viewDataAccessMode, bool isProtectedContentColumn)
			: base(gridColumn, model, viewDataAccessMode, isProtectedContentColumn) {
		}
		public override void SynchronizeControlWidth() { }
		protected override void ApplyModelCore() {
			base.ApplyModelCore();
			if(WebApplicationStyleManager.IsNewStyle && WebApplicationStyleManager.GridColumnsUpperCase && Control.Caption != null) {
				Control.Caption = Control.Caption.ToUpper();
			}
		}
	}
}
