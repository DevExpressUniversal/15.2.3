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
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Debugger;
using System.Activities.Presentation;
using System.Activities.Presentation.Debug;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.Toolbox;
using System.Activities.Presentation.View;
using System.Activities.Tracking;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.Workflow.Utils;
using DevExpress.XtraEditors;
using System.Activities.Presentation.Validation;
using System.Activities.Validation;
namespace DevExpress.ExpressApp.Workflow.Win {
	public class ActivityImageLoader {
		private AttributeTableBuilder attributeTableBuilder;
		public ActivityImageLoader() {
			attributeTableBuilder = new AttributeTableBuilder();
		}
		public void AssignImageToActivity(Type activityType, Image activityImage) {
			if(activityImage != null) {
				Type attributeType = typeof(System.Drawing.ToolboxBitmapAttribute);
				Type imageType = typeof(System.Drawing.Image);
				ConstructorInfo constructor = attributeType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { imageType, imageType }, null);
				ToolboxBitmapAttribute toolboxBitmapAttribute = constructor.Invoke(new object[] { activityImage, activityImage }) as System.Drawing.ToolboxBitmapAttribute;
				attributeTableBuilder.AddCustomAttributes(activityType, toolboxBitmapAttribute);
			}
		}
		public void UpdateMetadataStore() {
			MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
		}
	}
	public class WorkflowDesignerSelectionChangedEventArgs : EventArgs {
		public WorkflowDesignerSelectionChangedEventArgs(Selection selection) {
			Selection = selection;
		}
		public Selection Selection { get; private set; }
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class WorkflowDesignerControl : WorkflowDesignerControlBase {
		private XafWorkflowDesigner control;
		private IWorkflowDebuggerHelper debuggerHelper = null;
		private void designer_ModelChanged(object sender, EventArgs e) {
			if(XamlChanged != null) {
				XamlChanged(this, EventArgs.Empty);
			}
		}
		private void SetupToolbox(List<ActivityInformation> availableActivities) {
			ToolboxControl toolbox = (control.ToolboxBorder.Child as ToolboxControl);
			Guard.ArgumentNotNull(toolbox, "toolbox");
			Guard.ArgumentNotNull(availableActivities, "toolboxActivityTypes");
			toolbox.Categories.Clear();
			ActivityImageLoader bitmapLoader = new ActivityImageLoader();
			Dictionary<string, ToolboxCategory> categories = new Dictionary<string, ToolboxCategory>();
			foreach(ActivityInformation activityToolboxInfo in availableActivities) {
				string categoryName = activityToolboxInfo.Category;
				if(!categories.ContainsKey(categoryName)) {
					ToolboxCategory createdCategory = new ToolboxCategory(categoryName);
					categories.Add(categoryName, createdCategory);
				}
				categories[categoryName].Add(new ToolboxItemWrapper(activityToolboxInfo.ActivityType, activityToolboxInfo.DisplayName));
				if(activityToolboxInfo.Image != null) {
					bitmapLoader.AssignImageToActivity(activityToolboxInfo.ActivityType, activityToolboxInfo.Image);
				}
			}
			bitmapLoader.UpdateMetadataStore();
			foreach(ToolboxCategory category in categories.Values) {
				toolbox.Categories.Add(category);
			}
		}
		private void SelectionChangedHandler(Selection selection) {
			OnSelectionChanged(new WorkflowDesignerSelectionChangedEventArgs(selection));
		}
		private void View_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
			Mouse.OverrideCursor = null;
		}
		private void View_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
			Mouse.OverrideCursor = null;
		}
		protected virtual void OnSelectionChanged(WorkflowDesignerSelectionChangedEventArgs workflowDesignerSelectionChangedEventArgs) {
			if(SelectionChanged != null) {
				SelectionChanged(this, workflowDesignerSelectionChangedEventArgs);
			}
		}
		protected virtual IWorkflowDebuggerHelper CreateDebuggerHelper() {
			return new WorkflowDebuggerHelper(Designer);
		}
		protected override void BeforeLoadActivity() {
			if(Designer != null) {
				Designer.Context.Items.Unsubscribe<Selection>(SelectionChangedHandler);
				Designer.ModelChanged -= new EventHandler(designer_ModelChanged);
				Designer.View.MouseEnter -= new System.Windows.Input.MouseEventHandler(View_MouseEnter); 
				Designer.View.MouseLeave -= new System.Windows.Input.MouseEventHandler(View_MouseLeave); 
				ValidationService validationService = Designer.Context.Services.GetService<ValidationService>();
				if(validationService != null) {
					validationService.Settings.AdditionalConstraints.Clear();
				}
			}
			base.BeforeLoadActivity();
		}
		protected override void LoadActivity(string xaml) {
			base.LoadActivity(xaml);
			control.DesignerBorder.Child = Designer.View;
			control.PropertyGridExpander.Child = Designer.PropertyInspectorView;
			if(debuggerHelper == null) {
				debuggerHelper = CreateDebuggerHelper();
				Guard.ArgumentNotNull(debuggerHelper, "debuggerHelper");
			}
			else {
				debuggerHelper.Initialize(Designer);
			}
		}
		protected override void AfterLoadActivity() {
			base.AfterLoadActivity();
			Designer.Context.Items.Subscribe<Selection>(SelectionChangedHandler);
			Designer.ModelChanged += new EventHandler(designer_ModelChanged);
			Designer.View.MouseEnter += new System.Windows.Input.MouseEventHandler(View_MouseEnter); 
			Designer.View.MouseLeave += new System.Windows.Input.MouseEventHandler(View_MouseLeave); 
			debuggerHelper.UpdateBreakpoints();
			ValidationService validationService = Designer.Context.Services.GetService<ValidationService>();
			if(validationService != null) {
				validationService.Settings.AdditionalConstraints.Add(typeof(System.Activities.Statements.Sequence), new List<Constraint> { ConstraintHelper.SequenceHasNoVariablesOfNonSerializableObjectTypes() });
				validationService.Settings.AdditionalConstraints.Add(typeof(System.Activities.Statements.Flowchart), new List<Constraint> { ConstraintHelper.FlowchartHasNoVariablesOfNonSerializableObjectTypes() });
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(control != null) {
					control.ToolboxBorder.Child = null;
					control.DesignerBorder.Child = null;
					control.PropertyGridExpander.Child = null;
					control = null;
				}
				if(Designer != null) {
					Designer.Context.Items.Unsubscribe<Selection>(SelectionChangedHandler);
					Designer.ModelChanged -= new EventHandler(designer_ModelChanged);
				}
			}
			base.Dispose(disposing);
		}
		public WorkflowDesignerControl(List<ActivityInformation> activitiesInformation) {
			control = new XafWorkflowDesigner();
			Child = control;
			SetupToolbox(activitiesInformation);
		}
		public void HardUpdateLocationMappings() {
			string xaml = Xaml;
			SetXamlCore(xaml);
			debuggerHelper.UpdateSourceLocationMapping();
		}
		public XafWorkflowDesigner Control { get { return control; } }
		public IWorkflowDebuggerHelper DebuggerHelper { get { return debuggerHelper; } }
		public IObjectSpaceProvider ObjectSpaceProvider { get; set; }
		public event EventHandler XamlChanged;
		public event EventHandler<WorkflowDesignerSelectionChangedEventArgs> SelectionChanged;
	}
}
