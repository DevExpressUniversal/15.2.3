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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Items {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), ToolboxItem(false)]
	public class ButtonEditWithClearButton : ButtonEdit {
		static ButtonEditWithClearButton() {
			RepositoryItemButtonEditWithClearButton.Register();
		}
		public override string EditorTypeName { get { return RepositoryItemButtonEditWithClearButton.EditorName; } }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), ToolboxItem(false)]
	public class RepositoryItemButtonEditWithClearButton : RepositoryItemButtonEdit {
		static RepositoryItemButtonEditWithClearButton() {
			RepositoryItemButtonEditWithClearButton.Register();
		}
		internal static string EditorName {
			get { return typeof(ButtonEditWithClearButton).FullName; }
		}
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(ButtonEditWithClearButton),
					typeof(RepositoryItemButtonEditWithClearButton), typeof(ButtonEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.ButtonEdit, typeof(DevExpress.Accessibility.ButtonEditAccessible)));
			}
		}
		internal void SetClearButtonVisibility(bool val) {
			foreach(EditorButton button in Buttons) {
				if(button.Kind == ButtonPredefines.Delete) {
					button.Visible = val;
				}
			}
		}
		protected override void RaiseButtonClick(ButtonPressedEventArgs e) {
			if(e.Button.Kind == ButtonPredefines.Delete) {
				OwnerEdit.EditValue = null;
				foreach(EditorButton button in Buttons) {
					if(button.Tag != null && button.Tag.ToString() == ParametrizedActionItemControlFactory.GoButtonID) {
						RaiseButtonClick(new ButtonPressedEventArgs(button));
						return;
					}
				}
			}
			base.RaiseButtonClick(e);
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			SetClearButtonVisibility(!string.IsNullOrEmpty((string)OwnerEdit.EditValue));
		}
		public override string EditorTypeName { get { return EditorName; } }
		public RepositoryItemButtonEditWithClearButton() {
			Buttons.Clear();
			EditorButton clearButton = new DevExpress.XtraEditors.Controls.EditorButton(ButtonPredefines.Delete);
			clearButton.Visible = false;
			this.Buttons.Add(clearButton);
		}
		public void RaiseButtonClick(EditorButton button) {
			RaiseButtonClick(new ButtonPressedEventArgs(button));
		}
	}
}
