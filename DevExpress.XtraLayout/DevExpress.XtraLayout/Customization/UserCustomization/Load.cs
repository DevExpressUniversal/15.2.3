#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
namespace DevExpress.XtraLayout.Customization {
	class Load : InteractionMethod {
		public Load(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.Button;
			Text = "Load";
			Image = UCIconsHelper.OpenFileIcon;
			Category = Customization.Category.URSLPanel;
		}
		private OpenFileDialog CreateOpenFileDialog() {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = "xml";
			dialog.Filter = "XML files (*.xml)|*.xml";
			if(Owner.GetOwner().OptionsCustomizationForm.DefaultRestoreDirectory != string.Empty) dialog.InitialDirectory = Owner.GetOwner().OptionsCustomizationForm.DefaultRestoreDirectory;
			return dialog;
		}
		public override bool CanExecute() {
			if(Owner != null && Owner.GetOwner() != null && Owner.GetOwner().OptionsCustomizationForm.ShowLoadButton) return true;
			return false;
		}
		public override void Execute() {
			if(Owner.GetOwner() != null) {
				OpenFileDialog dialog = CreateOpenFileDialog();
				DialogResult result = DialogResult.Cancel;
				try {
					result = dialog.ShowDialog();
					if(result == DialogResult.OK) {
#if DEBUG
						if(Control.ModifierKeys == (Keys.Control | Keys.Alt))
							(Owner.GetOwner() as LayoutControl).DiagnosticRestoreLayoutFromXml(dialog.FileName);
						else
#endif
							Owner.GetOwner().RestoreLayoutFromXml(dialog.FileName);
						if(Owner.GetOwner().UndoManager != null) Owner.GetOwner().UndoManager.Reset();
					}
				} catch {
					XtraMessageBox.Show("file not found");
				}
			}
		}
	}
}
