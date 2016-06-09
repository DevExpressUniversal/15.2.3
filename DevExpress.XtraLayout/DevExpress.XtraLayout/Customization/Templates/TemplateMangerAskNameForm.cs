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
using System.Collections;
using System.Drawing;
using System.IO;
namespace DevExpress.XtraLayout.Customization.Templates {
	public partial class TemplateMangerAskNameForm : DevExpress.XtraEditors.XtraForm {
		TemplateManager templateManager;
		private LayoutControl templateLayout;
		public TemplateMangerAskNameForm(TemplateManager tManager, LayoutControl layout) {
			InitializeComponent();
			templateManager = tManager;
			templateLayout = layout;
			ClientSize = layoutControl.Root.MinSize;
		}
		private void simpleButton1_Click(object sender, EventArgs e) {
			if(templateNameTextEdit.Text != "") {
				TemplateManagerImplementorEventArgs evArgs = new TemplateManagerImplementorEventArgs() { TemplateManager = templateManager };
				(templateLayout as ITemplateManagerImplementor).RaiseSerializeControl(evArgs);
				string pathToUserXml = TemplateString.PathToTemplate;
				if(!Directory.Exists(pathToUserXml)) Directory.CreateDirectory(pathToUserXml);
				string fullPath = CreateTemplate(pathToUserXml);
				CheckTemplate(fullPath);
				if(templateLayout.CustomizationForm is CustomizationForm && (templateLayout.CustomizationForm as CustomizationForm).hiddenItemsList1 != null)
					(templateLayout.CustomizationForm as CustomizationForm).hiddenItemsList1.UpdateContent();
			}
		}
		private string CreateTemplate(string pathToUserXml) {
			string fullPath = GetUniqueFileName(pathToUserXml, templateNameTextEdit.Text);
			templateManager.CreateTemplate(templateNameTextEdit.Text, templateManager.Items, templateManager.ControlsInfo, fullPath);
			return fullPath;
		}
		void CheckTemplate(string path) {
			try {
				LayoutControl restoreCheckLC = new XtraLayout.LayoutControl();
				TemplateManager resotreTemplateManager = new TemplateManager();
				resotreTemplateManager.RestoreTemplatePreview(path, restoreCheckLC, new LayoutItemDragController(null, restoreCheckLC.Root, Point.Empty),true);
				restoreCheckLC.Dispose();
				Dispose();
			} catch(TemplateException e) {
				layoutControl.BeginUpdate();
				askNameGroup.Visibility = Utils.LayoutVisibility.Never;
				errorMessageGroup.Visibility = Utils.LayoutVisibility.Always;
				ClientSize = layoutControl.Root.MinSize;
				errorLabelControl.Text = ErrorString.Message+Environment.NewLine;
				detailsMemoEdit.Text = e.Message;
				Text = "Error Create Template";
				if(File.Exists(path)) File.Delete(path);
				layoutControl.EndUpdate();
			}
		}
	   public static string GetUniqueFileName(string pathToUserXml, string tryName) {
			Hashtable hashTable = new Hashtable();
			string[] fileNames = Directory.GetFiles(pathToUserXml, "*.xml", SearchOption.TopDirectoryOnly);
			foreach(string filename in fileNames) {
				if(!hashTable.ContainsKey(filename))
					hashTable.Add(filename, null);
			}
			string fullPath = String.Format("{0}{1}.xml", pathToUserXml, tryName);
			int count = 1;
			while(hashTable.ContainsKey(fullPath)) {
				fullPath = String.Format("{0}{1}({2}).xml", pathToUserXml, tryName, count);
				count++;
			}
			return fullPath;
		}
		private void cancelButton_Click(object sender, EventArgs e) {
			this.Dispose();
		}
		private void simpleButton1_Click_1(object sender, EventArgs e) {
			if(lciDetailsMemoEdit.Visibility == Utils.LayoutVisibility.Never) {
				lciDetailsMemoEdit.Visibility = Utils.LayoutVisibility.Always;
				Height += 165;
			} else {
				lciDetailsMemoEdit.Visibility = Utils.LayoutVisibility.Never;
				Height -= 165;
			}
		}
	}
	public static class ErrorString {
		public static string Message = @"The template code does not compile and so it cannot be saved. The error(s) may be raised if the template code contains links to external components (e.g., DataSet objects) or resources that cannot be included into the template. Please fix the error before saving the template. For example, if a control is bound to a DataSet, remove the binding." + Environment.NewLine;
		public static string ErrorPreview = @"Error : " + Environment.NewLine;
	}
}
