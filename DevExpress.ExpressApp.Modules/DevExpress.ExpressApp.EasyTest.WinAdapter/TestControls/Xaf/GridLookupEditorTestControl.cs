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

using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Xaf {
	public class GridLookupEditorTestControl : TestControlDXTextValidated<GridLookUpEdit>, IControlAct {
		public GridLookupEditorTestControl(GridLookUpEdit control)
			: base(control) {
		}
		protected override void InternalSetText(string text) {
			try {
				IList collection = (IList)control.Properties.DataSource;
				control.EditValue = FindEditValueByDisplayText(collection, text);
			}
			catch(Exception e) {
				throw new AdapterOperationException(e.Message);
			}
		}
		public object FindEditValueByDisplayText(IList collection, string text) {
			object result = null;
			List<string> items = new List<string>();
			if(collection != null) {
				for(int i = 0; i < collection.Count; ++i) {
					object obj = collection[i];
					string objDisplayText = control.Properties.GetDisplayTextByKeyValue(obj);
					items.Add(objDisplayText);
					if(objDisplayText == text) {
						result = obj;
						break;
					}
				}
			}
			if(result == null) {
				if(items.Count == 0) {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotSetEditValueListIsEmpty, text));
				}
				else {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotSetEditValueAvailableValues, text, string.Join("\n", items.ToArray())));
				}
			}
			return result;
		}
		protected override string Validate(string text) {
			return "";
		}
		public void Act(string value) {
			if(value == "Clear") {
				control.EditValue = null;
			}
		}
	}
}
