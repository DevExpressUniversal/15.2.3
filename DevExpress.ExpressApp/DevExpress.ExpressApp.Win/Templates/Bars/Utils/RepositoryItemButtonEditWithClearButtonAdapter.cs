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
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.ExpressApp.Win.Templates.Bars.Utils {
	public class RepositoryItemButtonEditWithClearButtonAdapter : RepositoryItemButtonEditAdapter {
		private BarEditItem barItem;
		private void RepositoryItem_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if(e.Button.Kind == ButtonPredefines.Delete) {
				RaiseExecute(((BaseEdit)sender).EditValue);
			}
		}
		private void barItem_EditValueChanged(object sender, EventArgs e) {
			RefreshClearButtonVisibility();
		}
		private void barItem_ShownEditor(object sender, ItemClickEventArgs e) {
			RefreshClearButtonVisibility();
		}
		private void barItem_HiddenEditor(object sender, ItemClickEventArgs e) {
			RefreshClearButtonVisibility();
		}
		private void RefreshClearButtonVisibility() {
			bool isClearButtonVisible = !string.IsNullOrEmpty(barItem.EditValue as string);
			RepositoryItem.SetClearButtonVisibility(isClearButtonVisible);
		}
		public RepositoryItemButtonEditWithClearButtonAdapter(BarEditItem barItem, RepositoryItemButtonEditWithClearButton repositoryItem, Type parameterType)
			: base(repositoryItem, parameterType) {
			Guard.ArgumentNotNull(barItem, "barItem");
			this.barItem = barItem;
		}
		public override void SetupRepositoryItem() {
			base.SetupRepositoryItem();
			RepositoryItem.ButtonClick += RepositoryItem_ButtonClick;
			barItem.EditValueChanged += barItem_EditValueChanged;
			barItem.ShownEditor += barItem_ShownEditor;
			barItem.HiddenEditor += barItem_HiddenEditor;
		}
		public new RepositoryItemButtonEditWithClearButton RepositoryItem {
			get { return (RepositoryItemButtonEditWithClearButton)base.RepositoryItem; }
		}
	}
}
