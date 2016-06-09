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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Collections;
namespace DevExpress.XtraEditors.Design {
	public partial class ValidationRulesEditorForm : XtraForm {
		public static string DefaultErrorText = Properties.Resources.DefaultValidatingErrorText;
		IDesignerHost designerHost;
		IComponentChangeService componentChangeService;
		DXValidationProvider validationProvider;
		Hashtable validatingComponents = new Hashtable();
		Hashtable designedComponents = new Hashtable();
		Hashtable validationRuleCache = new Hashtable();
		public ValidationRulesEditorForm() {
			InitializeComponent();
		}
		public ValidationRulesEditorForm(IDesignerHost designerHost, DXValidationProvider validationProvider, ICollection selectedComponents) {
			this.designerHost = designerHost;
			this.validationProvider = validationProvider;
			this.componentChangeService = (IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService));
			this.InitializeComponent();
			this.FillComponentsList();
			this.FillValidatingComponentsList(selectedComponents);
			this.FillValidationRulesCache();
			this.Initialize(selectedComponents);
		}
		protected virtual void FillComponentsList() {
			if(designerHost == null || designerHost.Container == null) return;
			FillCollectionCore(designerHost.Container.Components, designedComponents);
		}
		protected virtual void FillValidatingComponentsList(ICollection selectedComponents) {
			if(selectedComponents != null && selectedComponents.Count > 0) {
				FillCollectionCore(selectedComponents, validatingComponents);
			}
			else {
				if(designerHost != null && designerHost.Container != null) {
					FillCollectionCore(designerHost.Container.Components, validatingComponents);
				}
			}
			if(validatingComponents.Count > 0) controlsComboBox.Properties.Items.AddRange(validatingComponents.Keys);
		}
		void FillCollectionCore(ICollection componentSource, Hashtable componentTarget) {
			if(componentSource == null) return;
			foreach(IComponent component in componentSource) {
				if(component == null || !validationProvider.CanExtend(component)) continue;
				componentTarget.Add(TypeDescriptor.GetComponentName(component), component);
			}
		}
		protected virtual void FillValidationRulesCache() {
			if(designerHost == null || designerHost.Container == null || designerHost.Container.Components == null) return;
			foreach(IComponent component in designerHost.Container.Components) {
				if(component == null || !validationProvider.CanExtend(component)) continue;
				ValidationRuleBase rule = validationProvider.GetValidationRule(component as Control);
				if(rule != null) validationRuleCache.Add(TypeDescriptor.GetComponentName(component), rule);
			}
		}
		protected virtual void Initialize(ICollection selectedComponents) {
			controlsComboBox.Text = Properties.Resources.EmptyCaption;
			if(validatingComponents.Count > 0) controlsComboBox.Text = Properties.Resources.SelectCaption;
			noValidationCheck.Checked = true;
			if(validatingComponents.Count == 1) {
				controlsComboBox.SelectedIndex = 0;
				controlsComboBox.Properties.ReadOnly = true;
			}
			else if(selectedComponents != null && selectedComponents.Count > 1 && validatingComponents.Count > 1) {
				controlsComboBox.SelectedIndex = 0;
			}
			else {
				noValidationCheck.Enabled = comparedValidationCheck.Enabled = condtionValidationCheck.Enabled = false;
			}
		}
		private void controlsComboBoxEdit_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			if(validationRuleCache.Contains((string)e.Item))
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
		}
		private void controlsComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			string selectedControlName = GetSelectedComponentName();
			if(selectedControlName == null) return;
			noValidationCheck.Enabled = comparedValidationCheck.Enabled = condtionValidationCheck.Enabled = true;
			if(!validationRuleCache.Contains(selectedControlName)) {
				noValidationCheck.Checked = true;
				return;
			}
			ValidationRuleBase rule = validationRuleCache[selectedControlName] as ValidationRuleBase;
			if(rule is ConditionValidationRule) {
				condtionValidationCheck.Checked = true;
				AddConditionValidationRuleToCache();
			}
			if(rule is CompareAgainstControlValidationRule) {
				comparedValidationCheck.Checked = true;
				AddCompareAgainstControlValidationRuleToCache();
			}
		}
		private void CheckedChanged(object sender, EventArgs e) {
			if(noValidationCheck.Checked) {
				RemoveValidationRuleFromCache();
				labelControl2.Enabled = false;
				rulePropertiesGrid.Enabled = false;
			}
			else if(comparedValidationCheck.Checked) {
				labelControl2.Enabled = true;
				rulePropertiesGrid.Enabled = true;
				AddCompareAgainstControlValidationRuleToCache();
			}
			else {
				labelControl2.Enabled = true;
				rulePropertiesGrid.Enabled = true;
				AddConditionValidationRuleToCache();
			}
		}
		protected string GetSelectedComponentName() {
			return controlsComboBox.SelectedItem.ToString();
		}
		protected virtual void AddCompareAgainstControlValidationRuleToCache() {
			string name = GetSelectedComponentName();
			CompareAgainstControlValidationRule rule = null;
			if(validationRuleCache.Contains(name)) {
				rule = validationRuleCache[name] as CompareAgainstControlValidationRule;
				if(rule == null) {
					validationRuleCache.Remove(name);
					rule = CreateCompareAgainstControlValidationRule(name, validationProvider);
				}
			}
			else {
				rule = CreateCompareAgainstControlValidationRule(name, validationProvider);
			}
			if(rule != null) {
				validationRuleCache[name] = rule;
				CheckCompareAgainstControlValidationRule(rule);
			}
			rulePropertiesGrid.SelectedObject = rule as CompareAgainstControlValidationRule;
			rulePropertiesGrid.Refresh();
		}
		void CheckCompareAgainstControlValidationRule(CompareAgainstControlValidationRule rule) {
			Hashtable designedComponentsCopy = designedComponents.Clone() as Hashtable;
			designedComponentsCopy.Remove(GetSelectedComponentName());
			(rule as IValidatingControlCollection).Controls = new ArrayList(designedComponentsCopy.Values);
		}
		protected virtual CompareAgainstControlValidationRule CreateCompareAgainstControlValidationRule(string componentName, DXValidationProvider provider) {
			CompareAgainstControlValidationRule rule = new CompareAgainstControlValidationRule(componentName + "validationRule");
			rule.ErrorText = DefaultErrorText;
			CheckCompareAgainstControlValidationRule(rule);
			return rule;
		}
		protected virtual void AddConditionValidationRuleToCache() {
			string name = GetSelectedComponentName();
			ConditionValidationRule rule = null;
			if(validationRuleCache.Contains(name)) {
				rule = validationRuleCache[name] as ConditionValidationRule;
				if(rule == null) {
					validationRuleCache.Remove(name);
					rule = CreateConditionValidationRule(name);
				}
			}
			else {
				rule = CreateConditionValidationRule(name);
			}
			if(rule != null) validationRuleCache[name] = rule;
			rulePropertiesGrid.SelectedObject = rule as ConditionValidationRule;
			rulePropertiesGrid.Refresh();
		}
		protected virtual void RemoveValidationRuleFromCache() {
			string name = GetSelectedComponentName();
			if(validationRuleCache.Contains(name)) validationRuleCache.Remove(name);
			rulePropertiesGrid.SelectedObject = null;
			rulePropertiesGrid.Refresh();
		}
		protected virtual ConditionValidationRule CreateConditionValidationRule(string componentName) {
			ConditionValidationRule rule = new ConditionValidationRule(componentName + "validationRule", ConditionOperator.None);
			rule.ErrorText = DefaultErrorText;
			return rule;
		}
		private void okButton_Click(object sender, EventArgs e) {
			foreach(DictionaryEntry entry in validationRuleCache) {
				IComponent component = validatingComponents[entry.Key] as IComponent;
				if(component == null || entry.Value == null) continue;
				componentChangeService.OnComponentChanging(component, null);
				validationProvider.SetValidationRule(component as Control, entry.Value as ValidationRuleBase);
				componentChangeService.OnComponentChanged(component, null, null, null);
			}
			foreach(DictionaryEntry entry in validatingComponents) {
				string componentName = (string)entry.Key;
				if(validationRuleCache.Contains(componentName)) continue;
				IComponent component = entry.Value as IComponent;
				if(component != null) {
					componentChangeService.OnComponentChanging(component, null);
					validationProvider.SetValidationRule(component as Control, null);
					componentChangeService.OnComponentChanged(component, null, null, null);
				}
			}
			Close();
		}
	}
}
