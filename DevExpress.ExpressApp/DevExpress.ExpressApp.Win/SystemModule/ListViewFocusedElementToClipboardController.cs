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

using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Editors;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class ListViewFocusedElementToClipboardController : ViewController {
		private SimpleAction copyValue;
		private IFocusedElementCaptionProvider editor = null;
		public ListViewFocusedElementToClipboardController() : base() {
			TargetViewType = ViewType.ListView;
			copyValue = new SimpleAction(this, "CopyCellValue", "Menu");
			copyValue.ImageName = "Action_Copy_CellValue";
			copyValue.Shortcut = "CtrlShiftC";
			SetActionActive(false);
		}
		private void copyValue_Execute(object sender, SimpleActionExecuteEventArgs e) {
			Clipboard.SetDataObject(editor.FocusedElementCaption);
		}
		private void SetActionActive(bool active) {
			copyValue.Active["GetCurrentValueSupported"] = active;
		}
		protected override void OnActivated() {
			base.OnActivated();
			editor = ((ListView)View).Editor as IFocusedElementCaptionProvider;
			if(editor != null) {
				SetActionActive(true);
				copyValue.Execute += new SimpleActionExecuteEventHandler(copyValue_Execute);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(editor != null) {
				SetActionActive(false);
				copyValue.Execute -= new SimpleActionExecuteEventHandler(copyValue_Execute);
				editor = null;
			}
		}
		public SimpleAction CopyCellValueAction {
			get { return copyValue; }
		}
	}
}
