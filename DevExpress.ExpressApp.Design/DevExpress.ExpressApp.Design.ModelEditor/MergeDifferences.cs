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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using System.Windows.Forms;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Design.Commands;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
namespace DevExpress.ExpressApp.Design.ModelEditor {
	public class MergeDifferencesHelper : IMergeDifferences {
		IDisposable obj;
		public void MergeDifferences(_DTE dte, ProjectWrapper projectWrapper, string targetDiffFileName) {
			if(SaveAllDocuments(dte)) {
				try {
					ExtendModelInterfaceAdapter.LinksEnabled = false;
					DevExpress.ExpressApp.Model.Core.DesignerOnlyCalculator.IsRunFromDesigner = true;
					IServiceProvider serviceProvider = new ServiceProvider(dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
					XafTypesInfo.HardReset();
					DevExpress.ExpressApp.Xpo.XpoTypesInfoHelper.ForceInitialize();
					DynamicTypeService dynamicTypeService = (DynamicTypeService)serviceProvider.GetService(typeof(DynamicTypeService));
					IVsHierarchy pvHier;
					IVsSolution solution = serviceProvider.GetService(typeof(IVsSolution)) as IVsSolution;
					solution.GetProjectOfUniqueName(projectWrapper.FullName, out pvHier);
					ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService)dynamicTypeService.GetTypeDiscoveryService(pvHier);
					ITypeResolutionService typeResolutionService = dynamicTypeService.GetTypeResolutionService(pvHier);
					EFDesignTimeTypeInfoHelper.ForceInitialize(typeDiscoveryService);
					ModelLoader modelLoader = new ModelLoader(projectWrapper, XafTypesInfo.Instance, serviceProvider);
					IModelEditorController controller = null;
					try {
						controller = modelLoader.LoadModel(typeDiscoveryService, typeResolutionService, targetDiffFileName, out obj);
					}
					catch(Exception ex) {
						MergeDifferencesForm.HandleException(ex);
						return;
					}
					if(controller.ChooseMergeModuleAction.Items.Count == 0) {
						Messaging.GetMessaging(null).Show("There are no targets to merge into.", MergeDifferencesForm.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					controller.MergeDiffsMode = true;
					MergeDifferencesForm mergeDifferencesForm = new MergeDifferencesForm(controller, projectWrapper);
					mergeDifferencesForm.SetCaption(targetDiffFileName);
					mergeDifferencesForm.ShowDialog();
				}
				finally {
					ExtendModelInterfaceAdapter.LinksEnabled = true;
				}
			}
		}
		private bool SaveAllDocuments(_DTE dte) {
			bool isSavedAll = true;
			foreach(Document document in dte.Documents) {
				if(!document.Saved) {
					isSavedAll = false;
					break;
				}
			}
			if(!isSavedAll) {
				DialogResult dialogResult = Messaging.GetMessaging(null).Show("All unsaved Model customizations will be persisted. Do you want to proceed?", MergeDifferencesForm.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				if(dialogResult == DialogResult.OK) {
					dte.Documents.SaveAll();
					isSavedAll = true;
				}
			}
			return isSavedAll;
		}
		#region IDisposable Members
		public void Dispose() {
			if(obj != null) {
				obj.Dispose();
				obj = null;
			}
		}
		#endregion
	}
}
