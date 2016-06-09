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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Templates.Bars.Utils {
	public class RepositoryItemButtonEditAdapter : RepositoryItemTextEditAdapter {
		private const string ExecuteButtonId = "ExecuteButton";
		public static EditorButton FindExecuteButton(RepositoryItemButtonEdit repositoryItem) {
			Guard.ArgumentNotNull(repositoryItem, "repositoryItem");
			foreach(EditorButton button in repositoryItem.Buttons) {
				if(IsExecuteButton(button)) {
					return button;
				}
			}
			string message = string.Format("Cannot find the button with the 'ExecuteButton' Tag in the 'Buttons' collection of the '{0}' RepositoryItem.", repositoryItem.Name);
			throw new InvalidOperationException(message);
		}
		private static bool IsExecuteButton(EditorButton button) {
			return ExecuteButtonId.Equals(button.Tag);
		}
		private EditorButton CreateExecuteButton() {
			EditorButton button = new EditorButton();
			button.Kind = ButtonPredefines.Glyph;
			button.ImageLocation = ImageLocation.MiddleLeft;
			button.Tag = ExecuteButtonId;
			return button;
		}
		private EditorButton GetExecuteButton() {
			EditorButton button = FindExecuteButton(RepositoryItem);
			if(button == null) {
				throw new InvalidOperationException("Cannot get 'ExecuteButton' because it is null");
			}
			return button;
		}
		private void RepositoryItem_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if(IsExecuteButton(e.Button)) {
				BaseEdit baseEdit = (BaseEdit)sender;
				baseEdit.DoValidate();
				RaiseExecute(baseEdit.EditValue);
			}
		}
		public RepositoryItemButtonEditAdapter(RepositoryItemButtonEdit repositoryItem, Type parameterType) : base(repositoryItem, parameterType) { }
		public override void SetupRepositoryItem() {
			base.SetupRepositoryItem();
			if(ShowExecuteButton) {
				RepositoryItem.Buttons.Add(CreateExecuteButton());
				RepositoryItem.ButtonClick += RepositoryItem_ButtonClick;
			}
		}
		public override void SetExecuteButtonCaption(string caption) {
			if(ShowExecuteButton) {
				EditorButton executeButton = GetExecuteButton();
				executeButton.Caption = caption;
			}
		}
		public override void SetExecuteButtonImage(Image image) {
			if(ShowExecuteButton) {
				EditorButton executeButton = GetExecuteButton();
				executeButton.Image = image;
			}
		}
		public new RepositoryItemButtonEdit RepositoryItem {
			get { return (RepositoryItemButtonEdit)base.RepositoryItem; }
		}
	}
}
