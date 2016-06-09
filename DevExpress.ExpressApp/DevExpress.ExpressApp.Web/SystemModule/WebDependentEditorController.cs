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
using System.Text;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebDependentEditorController : DependentEditorController {
		public WebDependentEditorController() {
			TargetViewType = ViewType.Any;
		}
		protected override void RefreshDependentPropertyEditor(PropertyEditor masterEditor, IDependentPropertyEditor dependentEditor) {
			if(dependentEditor is PropertyEditor) {
				((PropertyEditor)dependentEditor).CurrentObject = masterEditor.CurrentObject;
			}
			base.RefreshDependentPropertyEditor(masterEditor, dependentEditor);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View is ListView && ((ListView)View).Editor is IDetailViewItemsHolder) {
				List<PropertyEditor> editors = new List<PropertyEditor>();
				foreach(ViewItem item in ((IDetailViewItemsHolder)((ListView)View).Editor).Items) {
					if(item is PropertyEditor) {
						editors.Add((PropertyEditor)item);
					}
				}
				ProcessEditors(editors);
			}
		}
	}
}
