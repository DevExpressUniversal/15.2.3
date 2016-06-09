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

extern alias Platform;
using DevExpress.Design.UI;
using DevExpress.Xpf.Core.Design.Wizards;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Editors;
using System.Reflection;
namespace DevExpress.Xpf.Core.Design {
	class TextEditEditMaskCommand : WpfDelegateCommand<ModelItem> {
		public TextEditEditMaskCommand() : base(EditMask, false) { }
		static void EditMask(ModelItem editor) {
			EditMaskWizardParams wizardParams = new EditMaskWizardParams() { Input = (IMaskProperties)editor.View.PlatformObject };
			EditMaskWizardWindow wizard = new EditMaskWizardWindow() { DataContext = wizardParams };
			DesignDialogHelper.ShowDialog(wizard);
			if (wizardParams.Output != null) {
				PropertyInfo[] properties = typeof(IMaskProperties).GetProperties();
				foreach (PropertyInfo property in properties) {
					if (property.Name == "SupportedMaskTypes") continue;
					object v1 = property.GetValue(wizardParams.Input, null);
					object v2 = property.GetValue(wizardParams.Output, null);
					if (!object.Equals(v1, v2))
						if (property.Name == "MaskPlaceHolder" && ((char)v2) == ((char)0))
							editor.Properties[property.Name].ClearValue();
						else
							editor.Properties[property.Name].SetValue(v2);
				}
			}
		}
	}
	class TextEditContextMenuProvider : PrimarySelectionContextMenuProvider {
		public TextEditContextMenuProvider() {
			MenuAction action = new MenuAction("Edit mask...");
			action.Execute += (sender, e) => {
				new TextEditEditMaskCommand().Execute(e.Selection.PrimarySelection);
			};
			Items.Add(action);
			UpdateItemStatus += OnUpdateItemStatus;
		}
		void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			IMaskProperties properties = (IMaskProperties)e.Selection.PrimarySelection.View.PlatformObject;
			((MenuAction)Items[0]).Visible = properties.SupportedMaskTypes != null;
		}
	}
}
