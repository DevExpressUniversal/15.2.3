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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ViewModeDataItemTemplate : DataItemTemplateBase {
		public ViewModeDataItemTemplate(WebPropertyEditor propertyEditor, IDataItemTemplateInfoProvider dataItemTemplateInfoProvider)
			: base(propertyEditor, dataItemTemplateInfoProvider) {
			propertyEditor.ViewEditMode = ViewEditMode.View;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected override Control CreateControl(WebColumnBase column, object obj) {
			Control control = null;
			IDataColumnInfo columnInfo = DataItemTemplateInfoProvider.GetColumnInfo(column) as IDataColumnInfo;
			if(columnInfo != null) {
				if(obj != null && PropertyEditor != null) {
					PropertyEditor.CreateControl();
					ICustomBehaviorInListView customBehaviorInListView = PropertyEditor as ICustomBehaviorInListView;
					if(customBehaviorInListView != null) {
						customBehaviorInListView.CustomizeForListView();
					}
					PropertyEditor.CurrentObject = obj;
					PropertyEditor.ReadValue();
					control = (Control)PropertyEditor.Control;
					PropertyEditor.SetControlAlignment(WebAlignmentProvider.GetAlignment(columnInfo.MemberInfo.MemberType));
				}
				CustomCreateCellControlEventArgs args = new CustomCreateCellControlEventArgs(PropertyEditor, columnInfo.Model.PropertyName, obj);
				OnCustomCreateCellControl(args);
				if(args.Handled) {
					return args.CellControl;
				}
				if(control == null) {
					object propertyValue = obj == null ? null : columnInfo.MemberInfo.GetValue(obj);
					control = DisplayValueHelper.GetLiteralControl(propertyValue == null ? CaptionHelper.NullValueText : propertyValue.ToString());
				}
			}
			return control;
		}
	}
}
