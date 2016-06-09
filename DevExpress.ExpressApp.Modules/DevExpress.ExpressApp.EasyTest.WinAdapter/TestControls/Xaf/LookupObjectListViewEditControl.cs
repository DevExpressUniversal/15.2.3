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
using System.Reflection;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Xaf {
	public class LookupObjectListViewEditControl : TestControlDXTextValidated<LookupEdit>, IControlAct {
		protected override void InternalSetText(string text) {
			try {
				CollectionSourceBase collection = control.GetPopupFormCollectionSource();
				object editValue = FindSetEditValueByDisplayText(control, collection, text);
				control.EditValue = editValue;
				control.IsModified = true; 
			}
			catch(Exception e) {
				throw new AdapterOperationException(e.Message);
			}
		}
		public object FindSetEditValueByDisplayText(ILookupEditTest lookupEditTest, CollectionSourceBase collection, string text) {
			object result = null;
			if (Object.ReferenceEquals(collection.Criteria[FilterController.FullTextSearchCriteriaName], CollectionSourceBase.EmptyCollectionCriteria)) {
				collection.Criteria[FilterController.FullTextSearchCriteriaName] = null;
			}
			List<string> items = new List<string>();
			if (collection.List != null) {
				for (int i = 0; i < collection.List.Count; ++i) {
					object obj = collection.List[i];
					string objDisplayText = lookupEditTest.GetDisplayText(obj);
					items.Add(objDisplayText);
					if (objDisplayText == text) {
						result = obj;
						break;
					}
				}
			}
			if (result == null) {
				if (items.Count == 0) {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotSetEditValueListIsEmpty, text));
				}
				else {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotSetEditValueAvailableValues, text, string.Join("\n", items.ToArray())));
				}
			}
			return result;
		}
		public LookupObjectListViewEditControl(LookupEdit control) : base(control) { }
		public void Act(string parameterValue) {
			if(string.IsNullOrEmpty(parameterValue)) {
				control.Focus();
				control.ShowPopup();
			}
			else if(string.Compare(parameterValue, "Clear", true) == 0) {
				control.Focus();
				control.GetType().GetMethod("OnPressButton", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(control, new object[] { new EditorButtonObjectInfoArgs(control.Properties.Buttons[0], control.Properties.Appearance) });
			}
			else {
				control.Focus();
				control.ShowPopup();
				ICommandAdapter commandAdapter = new WinEasyTestCommandAdapter();
				ITestControl dateNavControl = commandAdapter.CreateTestControl(TestControlType.Action, parameterValue);
				dateNavControl.GetInterface<IControlAct>().Act(null);
			}
		}
	}
}
