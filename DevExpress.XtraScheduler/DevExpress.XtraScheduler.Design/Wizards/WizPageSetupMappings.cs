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
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Design.Wizards {
	public partial class WizPageSetupMappings<T> : DevExpress.Utils.InteriorWizardPage where T : IPersistentObject {
		SetupMappingsWizard<T> wizard;
		public WizPageSetupMappings() {
			InitializeComponent();
		}
		public WizPageSetupMappings(SetupMappingsWizard<T> wizard) {
			if (wizard == null)
				Exceptions.ThrowArgumentNullException("wizard");
			this.wizard = wizard;
			InitializeComponent();
			this.lblDescription.Text = "You can let the wizard decide which field matches the corresponding property. If you click the Generate button, the Wizard specifies the most appropriate mappings. The Clear button deletes all mappings.";
			this.lblRequiredMappingDescription.Text = "* - required mappings";
			this.ucExtendedArea.Visible = false;
			this.wizard.InitializeWizardExtension(this.ucExtendedArea, UpdatePage);
			this.propertyGrid.SelectedObject = wizard.ObjectStorage.Mappings;
		}
		private void OnBtnGenerateMappingsClick(object sender, EventArgs e) {
			try {
				wizard.ClearMappings();
				wizard.GenerateMappings();
			} catch {
			}
			UpdatePage();
		}
		private void OnBtnClearMappingsClick(object sender, EventArgs e) {
			try {
				wizard.ClearMappings();
			} catch {
			}
			UpdatePage();
		}
		protected void UpdatePage() {
			this.propertyGrid.Refresh();
		}
		protected override string OnWizardNext() {
			this.wizard.ClearNotAllowedMappings();
			if (!ValidateMappings())
				return WizardForm.NoPageChange;
			return base.OnWizardNext();
		}
		protected override bool OnWizardFinish() {
			this.wizard.ClearNotAllowedMappings();
			return ValidateMappings() && base.OnWizardFinish();
		}
		protected internal virtual bool ValidateMappings() {
			StringBuilder errorMessage = new StringBuilder();
			if (!wizard.ValidateMappings(errorMessage))
				return ShowInvalidMappingsErrorMessage(errorMessage.ToString());
			else
				return true;
		}
		protected internal virtual bool ShowInvalidMappingsErrorMessage(string errorMessage) {
			return XtraMessageBox.Show(this, errorMessage, Wizard.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes;
		}
		private void propertyGrid_CustomPropertyDescriptors(object sender, DevExpress.XtraVerticalGrid.Events.CustomPropertyDescriptorsEventArgs e) {
			if (e.Context.PropertyDescriptor == null) {
				PropertyDescriptorCollection props = new PropertyDescriptorCollection(null);
				List<string> requiredNames = wizard.GetRequiredMappingNames();
				for (int i = 0; i < e.Properties.Count; i++) {
					PropertyDescriptor pd = e.Properties[i];
					string name = pd.Name;
					string requiredName = GetRequiredMappingName(requiredNames, name);
					if (IsRequiredMapping(requiredName, name) )
						props.Add(new CustomPropertyDescriptor(e.Source, requiredName, name));
					else if (CheckAdditionalRestriction(name)) {
						props.Add(new ReadonlyCustomPropertyDescriptor(e.Source, name, name));
					} else 
						props.Add(pd);
				}
				e.Properties = props;
			}
		}
		bool CheckAdditionalRestriction(string name) {
			return this.wizard.CheckAdditionalRestriction(name);
		}
		protected virtual bool IsRequiredMapping(string requiredName, string name) {
			return !string.Equals(name, requiredName);
		}
		protected virtual string GetRequiredMappingName(List<string> requiredNames, string name) {
			if (!requiredNames.Contains(name))
				return name;
			return AddRequiredSign(name);
		}
		protected virtual string AddRequiredSign(string name) {
			return String.Format("{0} *", name);
		}
	}
	class ReadonlyCustomPropertyDescriptor : CustomPropertyDescriptor {
		public ReadonlyCustomPropertyDescriptor(object source, string name, string targetPath)
			: base(source, name, targetPath) {
		}
		public override bool IsReadOnly { get { return true; } }
		public override object GetValue(object component) {
			return String.Empty;
		}
	}
	class CustomPropertyDescriptor : PropertyDescriptor {
		string name;
		PropertyDescriptor sourcePropertyDescriptor;
		object source;
		public CustomPropertyDescriptor(object source, string name, string targetPath)
			: base(name, null) {
			this.name = name;
			this.source = source;
			this.sourcePropertyDescriptor = TypeDescriptor.GetProperties(source)[targetPath];
			if (SourcePropertyDescriptor == null)
				throw new Exception("Can't bind to the source with the " + targetPath + " property");
		}
		public override string Name { get { return name; } }
		public override Type ComponentType { get { return SourcePropertyDescriptor.ComponentType; } }
		public override bool IsReadOnly { get { return SourcePropertyDescriptor.IsReadOnly; } }
		public override Type PropertyType { get { return SourcePropertyDescriptor.PropertyType; } }
		public override TypeConverter Converter { get { return SourcePropertyDescriptor.Converter; } }
		PropertyDescriptor SourcePropertyDescriptor { get { return sourcePropertyDescriptor; } }
		object Source { get { return source; } }
		public override object GetValue(object component)
		{
			return SourcePropertyDescriptor.GetValue(Source);
		}
		public override bool CanResetValue(object component)
		{
			return SourcePropertyDescriptor.CanResetValue(Source);
		}
		public override void ResetValue(object component)
		{
			SourcePropertyDescriptor.ResetValue(Source);
		}
		public override void SetValue(object component, object value)
		{
			SourcePropertyDescriptor.SetValue(Source, value);
		}
		public override bool ShouldSerializeValue(object component)
		{
			return SourcePropertyDescriptor.ShouldSerializeValue(Source);
		}
		public override object GetEditor(Type editorBaseType)
		{
			return SourcePropertyDescriptor.GetEditor(editorBaseType);
		}
	}
}
