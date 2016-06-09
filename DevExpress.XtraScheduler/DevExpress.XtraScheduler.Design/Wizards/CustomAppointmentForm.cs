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
using DevExpress.XtraScheduler.Design.Wizards;
using System.ComponentModel;
using DevExpress.XtraScheduler.Design.ItemTemplates;
using System.Collections.Generic;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.Design.TasksSolution;
using System.Windows.Forms;
namespace DevExpress.XtraScheduler.Design {
	public enum CusomAppointmentFormType {
		Default,
		OutlookStyle
	}
	public class CustomAppointmentFormGenerator {
		delegate ProjectItemTemplate CreateProjectItemTemplateHandler(ProjectLanguage language);
		public const string SRCustomAppointmentFormWizardDescription = "Create Custom Appointment Form";
		IComponent component;
		SchedulerControl schedulerControl;
		public CustomAppointmentFormGenerator(IComponent component, SchedulerControl control) {
			this.component = component;
			this.schedulerControl = control;
		}
		public IComponent Component { get { return component; } }
		public SchedulerControl SchedulerControl { get { return schedulerControl; } }
		public void Execute() {
			CustomAppointmentFormWizardForm wizardForm = new CustomAppointmentFormWizardForm();
			if (wizardForm.ShowDialog() == DialogResult.OK) {
				using (ComponentChangeHelper helper = new ComponentChangeHelper(SchedulerControl, SRCustomAppointmentFormWizardDescription)) {
					GenerateForm(wizardForm.FormType);
				}
			}
		}
		public void GenerateForm(CusomAppointmentFormType formType) { 
			switch(formType) {
				case CusomAppointmentFormType.Default:
					GenerateCodeForAppointmentFormCustomization(CreateAppointmentFormProjectItemTemplate);
					break;
				case CusomAppointmentFormType.OutlookStyle:
					GenerateCodeForAppointmentFormCustomization(CreateOutlookAppointmentFormProjectItemTemplate);
					break;
			}
		}
		void GenerateCodeForAppointmentFormCustomization(CreateProjectItemTemplateHandler createProjectItemTemplateHandler) {
			ProjectManager project = new ProjectManager(Component.Site);
			ProjectItemTemplate projectItemInfo = createProjectItemTemplateHandler(project.Language);
			EnvDTE.ProjectItem insertedItem = project.AddProjectItem(projectItemInfo);
			if (insertedItem == null)
				return;
			List<EnvDTE.CodeElement> generatedClasses = DesignerCodeGenerationHelper.FindAllCodeClass(insertedItem.FileCodeModel.CodeElements);
			if (generatedClasses.Count <= 0)
				return;
			EnvDTE.CodeClass generatedAppointmentFormCodeClass = (EnvDTE.CodeClass)generatedClasses[0];
			IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
			DesignTimeEventHandlerHelper helper = new DesignTimeEventHandlerHelper(host);
			helper.UseExistingEventHandler = false;
			IEventHandlerCodeGenerator eventGenerator = new CustomAppointmentFormHandlerCodeGenerator(host, generatedAppointmentFormCodeClass.FullName, SchedulerControl);
			helper.GenerateEventHandler(Component, "EditAppointmentFormShowing", eventGenerator);
		}
		ProjectItemTemplate CreateAppointmentFormProjectItemTemplate(ProjectLanguage language) {
			return new AppointmentFormProjectItemTemplate(language);
		}
		ProjectItemTemplate CreateOutlookAppointmentFormProjectItemTemplate(ProjectLanguage language) {
			return new OutlookAppointmentFormItemTemplate(language);
		}
	}
}
