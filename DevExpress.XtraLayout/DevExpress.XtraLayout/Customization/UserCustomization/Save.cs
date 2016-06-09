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
namespace DevExpress.XtraLayout.Customization {
	class Save : InteractionMethod {
		public Save(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.Button;
			Text = "Save";
			Image = UCIconsHelper.SaveIcon;
			Category = Customization.Category.URSLPanel;
		}
		public override bool CanExecute() {
			if(Owner != null && Owner.GetOwner() != null && Owner.GetOwner().OptionsCustomizationForm.ShowSaveButton) return true;
			return false;
		}
		public override void Execute() {
			SaveFileDialog dialog = CreateSaveFileDialog();
			if(Owner.GetOwner() != null) {
				DialogResult result = dialog.ShowDialog();
				if(result == DialogResult.OK)
					try {
					   Owner.GetOwner().SaveLayoutToXml(dialog.FileName);
					} catch(Exception ex) {
						DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
					}
			}
		}
	   private SaveFileDialog CreateSaveFileDialog() {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = "xml";
			dialog.Filter = "XML files (*.xml)|*.xml";
			if(Owner.GetOwner().OptionsCustomizationForm.DefaultSaveDirectory != string.Empty) dialog.InitialDirectory = Owner.GetOwner().OptionsCustomizationForm.DefaultSaveDirectory;
			return dialog;
		}
	}
}
