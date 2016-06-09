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
using DevExpress.XtraReports.Parameters;
using DevExpress.Utils;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public class NewParameterEditorPresenter {
		public static readonly Type DefaultParameterType = typeof(Int32);
		#region inner classes
		public class TypedLookUpValue<T> {
			public string Description { get; set; }
			public T Value { get; set; }
		}
		#endregion
		Parameter parameter;
		INewParameterEditorView view;
		INameCreationService nameCreationService;
		IExtensionsProvider extensionsProvider;
		IContainer container;
		public NewParameterEditorPresenter(Parameter parameter, INewParameterEditorView view, INameCreationService nameCreationService, IContainer container, IExtensionsProvider extensionsProvider) {
			Guard.ArgumentNotNull(parameter, "parameter");
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(nameCreationService, "nameCreationService");
			Guard.ArgumentNotNull(container, "container");
			this.parameter = parameter;
			this.view = view;
			this.nameCreationService = nameCreationService;
			this.extensionsProvider = extensionsProvider;
			this.container = container;
			Initialize();
		}
		void Initialize() {
			InitializeName();
			InitializeDescription();
			InitializeType();
			InitializeShowAtParametersPanel();
			InitializeStandardValuesSupported();
			InitializeMultiValueSupported();
			InitializeLookUpSettingsActiveTab();
			view.Submit += view_Submit;
			view.DataSourceChanged += RefreshSubmitEnabled;
			view.DisplayMemberChanged += RefreshSubmitEnabled;
			view.ValueMemberChanged += RefreshSubmitEnabled;
			view.ActiveTabChanged += RefreshSubmitEnabled;
			view.LookUpValuesChanged += RefreshSubmitEnabled;
			view.FilterStringChanged += RefreshSubmitEnabled;
		}
		void InitializeName() {
			string defaultName = nameCreationService.CreateName(container, typeof(Parameter));
			view.Name = defaultName;
			view.ValidateName += view_ValidateName;
		}
		void InitializeDescription() {
			string name = view.Name;
			view.Description = name.Length > 1 ? char.ToUpper(name[0]) + name.Substring(1) : name;
		}
		void InitializeType() {
			var contextName = extensionsProvider != null ? extensionsProvider.Extensions[ParameterEditorService.Guid] : null;
			Dictionary<Type,string> availableTypes = ParameterEditorService.GetParameterTypes(contextName);
			view.PopulateTypes(availableTypes);
			view.TypeChanged += view_TypeChanged;
			if (availableTypes.Count > 0) {
				Type type = DefaultParameterType;
				view.Type = type;
				view_TypeChanged(view, EventArgs.Empty);
			}			
		}
		void ResetDefaultValue() {
			view.DefaultValue = parameter.Value;
		}
		void InitializeShowAtParametersPanel() {
			view.ShowAtParametersPanel = true;
			view.ShowAtParametersPanelChanged += view_ShowAtParametersPanelChanged;
			view_ShowAtParametersPanelChanged(view, EventArgs.Empty);
		}
		void InitializeStandardValuesSupported() {
			view.StandardValuesSupported = false;
			view.StandardValuesSupportedChanged += view_StandardValuesSupportedChanged;
			view_StandardValuesSupportedChanged(view, EventArgs.Empty);
		}
		void InitializeMultiValueSupported() {
			view.MultiValue = false;
			view.MultiValueChanged += view_MultiValueChanged;
			view_MultiValueChanged(view, EventArgs.Empty);
		}
		void InitializeLookUpSettingsActiveTab() {
			view.LookUpSettingsActiveTab = LookUpSettingsTab.DynamicList;
		}
		void view_ValidateName(object sender, ValidationEventArgs e) {
			e.IsValid = nameCreationService.IsValidName(view.Name);
		}
		void view_TypeChanged(object sender, EventArgs e) {
			parameter.Type = view.Type;
			view.SetEditType(view.Type, parameter.MultiValue);
			ResetDefaultValue();
			ResetLookUpValues();
		}
		void view_StandardValuesSupportedChanged(object sender, EventArgs e) {
			view.EnableLookUpSettings(view.StandardValuesSupported, true);
			RefreshSubmitEnabled();
		}
		void view_ShowAtParametersPanelChanged(object sender, EventArgs e) {
			view.EnableLookUpSettings(view.ShowAtParametersPanel && view.StandardValuesSupported, view.ShowAtParametersPanel);
		}
		void view_MultiValueChanged(object sender, EventArgs e) {
			parameter.MultiValue = view.MultiValue;
			view.SetEditType(view.Type, parameter.MultiValue);
			ResetDefaultValue();
		}
		void RefreshSubmitEnabled(object sender, EventArgs e) {
			RefreshSubmitEnabled();
		}
		void RefreshSubmitEnabled() {
			bool enable = false;
			if(!view.StandardValuesSupported) {
				enable = true;
			} else if(view.LookUpSettingsActiveTab == LookUpSettingsTab.DynamicList) {
				enable = view.DataSource != null && !string.IsNullOrEmpty(view.ValueMember) && !string.IsNullOrEmpty(view.DisplayMember);
			} else if(view.LookUpSettingsActiveTab == LookUpSettingsTab.StaticList) {
				enable = view.LookUpValues != null && view.LookUpValues.Count > 0;
			}
			view.EnableSubmit(enable);
		}
		void ResetLookUpValues() {
			Type parameterType = view.Type;
			Type typedLookUpValueType = typeof(TypedLookUpValue<>).MakeGenericType(new[] { parameterType });
			Type lookupValuesListType = typeof(List<>).MakeGenericType(new[] { typedLookUpValueType });
			view.LookUpValues = (IList)Activator.CreateInstance(lookupValuesListType);
		}
		void view_Submit(object sender, EventArgs e) {
			parameter.Name = view.Name;
			parameter.Description = view.Description;
			parameter.Type = view.Type;
			parameter.Value = ConvertTo(view.DefaultValue, view.Type, view.MultiValue);
			parameter.Visible = view.ShowAtParametersPanel;
			if(view.ShowAtParametersPanel && view.StandardValuesSupported) {
				switch (view.LookUpSettingsActiveTab) { 
					case LookUpSettingsTab.DynamicList:
						parameter.LookUpSettings = new DynamicListLookUpSettings() { 
							DataSource = view.DataSource,
							DataAdapter = view.DataAdapter,
							DataMember = view.DataMember,
							DisplayMember = view.DisplayMember,
							ValueMember = view.ValueMember,
							FilterString = view.FilterString
						};
						break;
					case LookUpSettingsTab.StaticList:
						var staticListLookUpSettings = new StaticListLookUpSettings();
						foreach (object typedLookUpValue in view.LookUpValues) {
							Type typedLookUpValueType = typedLookUpValue.GetType();
							staticListLookUpSettings.LookUpValues.Add(new LookUpValue() {
								Value = typedLookUpValueType.GetProperty("Value").GetValue(typedLookUpValue, null),
								Description = (string)typedLookUpValueType.GetProperty("Description").GetValue(typedLookUpValue, null)
							});
						}
						parameter.LookUpSettings = staticListLookUpSettings;
						break;
					default: throw new NotSupportedException();
				}
			}
		}
		object ConvertTo(object value, Type type, bool multiValue) {
			Parameter.TryConvertValue(ref value, ParameterHelper.GetDefaultValue(type), type, multiValue, System.Globalization.CultureInfo.CurrentCulture);
			return value;
		}
	}
}
