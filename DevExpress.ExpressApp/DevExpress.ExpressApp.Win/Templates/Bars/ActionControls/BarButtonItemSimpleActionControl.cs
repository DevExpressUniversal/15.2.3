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
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates.Bars.ActionControls {
	public class BarButtonItemSimpleActionControl : BarItemActionControl<BarButtonItem>, ISimpleActionControl {
		private void BarItem_ItemClick(object sender, ItemClickEventArgs args) {
			RaiseExecute();
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			BarItem.ItemClick += BarItem_ItemClick;
		}
		protected void RaiseExecute() {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
			if(IsConfirmed() && Execute != null) {
				try {
					Execute(this, EventArgs.Empty);
				}
				catch(ActionExecutionException e) {
					Application.OnThreadException(e.InnerException);
				}
			}
		}
		public BarButtonItemSimpleActionControl() { }
		public BarButtonItemSimpleActionControl(string actionId, BarButtonItem item) : base(actionId, item) { }
		public event EventHandler<EventArgs> Execute;
	}
}
