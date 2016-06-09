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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
using System.Collections.ObjectModel;
namespace DevExpress.XtraSpreadsheet.Commands {
	public class ShowDocumentPropertiesCommand : SpreadsheetMenuItemSimpleCommand {
		public ShowDocumentPropertiesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FileShowDocumentProperties; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ShowDocumentProperties; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ShowDocumentPropertiesDescription; } }
		public override string ImageName { get { return "DocumentProperties"; } }
		ModelDocumentApplicationProperties ApplicationProperties { get { return DocumentModel.DocumentApplicationProperties; } }
		ModelDocumentCoreProperties CoreProperties { get { return DocumentModel.DocumentCoreProperties; } }
		ModelDocumentCustomProperties CustomProperties { get { return DocumentModel.DocumentCustomProperties; } }
		#endregion
		protected internal override void ExecuteCore() {
			Control.ShowDocumentPropertiesForm(CreateViewModel());
		}
		public DocumentPropertiesViewModel CreateViewModel() {
			DocumentPropertiesViewModel viewModel = new DocumentPropertiesViewModel(Control);
			viewModel.Application = ApplicationProperties.Application;
			viewModel.Manager = ApplicationProperties.Manager;
			viewModel.Company = ApplicationProperties.Company;
			viewModel.Version = ApplicationProperties.Version;
			viewModel.Title = CoreProperties.Title;
			viewModel.Subject = CoreProperties.Subject;
			viewModel.Creator = CoreProperties.Creator;
			viewModel.Keywords = CoreProperties.Keywords;
			viewModel.Description = CoreProperties.Description;
			viewModel.LastModifiedBy = CoreProperties.LastModifiedBy;
			viewModel.Category = CoreProperties.Category;
			viewModel.Created = DateTimeToString(CoreProperties.Created);
			viewModel.Modified = DateTimeToString(CoreProperties.Modified);
			viewModel.Printed = DateTimeToString(CoreProperties.LastPrinted);
			return viewModel;
		}
		public static string DateTimeToString(DateTime value) {
			if (value == DateTime.MinValue)
				return String.Empty;
			return value.ToLongDateString() + " " + value.ToLongTimeString();
		}
		public bool Validate(DocumentPropertiesViewModel viewModel) {
			return true;
		}
		public void ApplyChanges(DocumentPropertiesViewModel viewModel) {
			bool isBuiltInPropsChanged = IsBuiltInPropertiesChanged(viewModel);
			bool isCustomPropsChanged = IsCustomPropertiesChanged(viewModel);
			ApplyApplicationProperties(viewModel);
			ApplyCoreProperties(viewModel);
			ApplyCustomProperties(viewModel);
			InnerControl.RaiseDocumentPropertiesChanged(isBuiltInPropsChanged, isCustomPropsChanged);
		}
		void ApplyApplicationProperties(DocumentPropertiesViewModel viewModel) {
			ModelDocumentApplicationProperties properties = ApplicationProperties;
			properties.BeginUpdate();
			try {
				properties.Application = viewModel.Application;
				properties.Manager = viewModel.Manager;
				properties.Company = viewModel.Company;
				properties.Version = viewModel.Version;
			}
			finally {
				properties.EndUpdate();
			}
		}
		void ApplyCoreProperties(DocumentPropertiesViewModel viewModel) {
			ModelDocumentCoreProperties properties = CoreProperties;
			properties.BeginUpdate();
			try {
				properties.Title = viewModel.Title;
				properties.Subject = viewModel.Subject;
				properties.Creator = viewModel.Creator;
				properties.Keywords = viewModel.Keywords;
				properties.Description = viewModel.Description;
				properties.LastModifiedBy = viewModel.LastModifiedBy;
				properties.Category = viewModel.Category;
			}
			finally {
				properties.EndUpdate();
			}
		}
		void ApplyCustomProperties(DocumentPropertiesViewModel viewModel) {
			ModelDocumentCustomProperties properties = CustomProperties;
			properties.BeginUpdate();
			try {
				properties.Clear();
				ObservableCollection<CustomDocumentPropertyViewModel> propertiesViewModel = viewModel.CustomPropertiesDataSource;
				int count = propertiesViewModel.Count;
				for (int i = 0; i < count; i++)
					AppendCustomProperty(properties, propertiesViewModel[i]);
			}
			finally {
				properties.EndUpdate();
			}
		}
		void AppendCustomProperty(ModelDocumentCustomProperties properties, CustomDocumentPropertyViewModel model) {
			properties[model.Name] = model.Value.Clone();
		}
		bool IsBuiltInPropertiesChanged(DocumentPropertiesViewModel viewModel) {
			ModelDocumentApplicationProperties applicationProperties = ApplicationProperties;
			ModelDocumentCoreProperties coreProperties = CoreProperties;
			return applicationProperties.Application != viewModel.Application ||
				applicationProperties.Manager != viewModel.Manager ||
				applicationProperties.Company != viewModel.Company ||
				applicationProperties.Version != viewModel.Version ||
				coreProperties.Title != viewModel.Title ||
				coreProperties.Subject != viewModel.Subject ||
				coreProperties.Creator != viewModel.Creator ||
				coreProperties.Keywords != viewModel.Keywords ||
				coreProperties.Description != viewModel.Description ||
				coreProperties.LastModifiedBy != viewModel.LastModifiedBy ||
				coreProperties.Category != viewModel.Category;
		}
		bool IsCustomPropertiesChanged(DocumentPropertiesViewModel viewModel) {
			ModelDocumentCustomProperties properties = CustomProperties;
			ObservableCollection<CustomDocumentPropertyViewModel> propertiesViewModel = viewModel.CustomPropertiesDataSource;
			int count = propertiesViewModel.Count;
			if (properties.Count != count)
				return true;
			for (int i = 0; i < count; i++)
				if (properties[propertiesViewModel[i].Name] != propertiesViewModel[i].Value)
					return true;
			return false;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive);
			ApplyWorkbookProtection(state, WorkbookProtection.LockStructure);
		}
	}
}
