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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq.Expressions;
using System.Linq;
using DevExpress.Xpf.Core.Design.Wizards;
using DevExpress.Design.SmartTags;
using Microsoft.Windows.Design;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#if SL
using Platform::System.Windows;
using Platform::DevExpress.Xpf.Editors.DataPager;
using Microsoft.Windows.Design.Model;
#else
using System.Windows;
#endif
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Editors.DateNavigator;
using Platform::DevExpress.Xpf.Editors.Popups.Calendar;
using Orientation = System.Windows.Controls.Orientation;
using SelectionMode = System.Windows.Controls.SelectionMode;
using Platform::DevExpress.Utils.Internal;
#if !SL
using DevExpress.Xpf.Core.Design.DataAccess.UI;
using DevExpress.Xpf.Editors.RangeControl;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Design.UI;
using System.Globalization;
using System.Windows.Markup;
using Microsoft.Windows.Design.Model;
#endif
namespace DevExpress.Xpf.Core.Design {
#if !SL
	public class ShowItemsSourceWizard : ICommand {
		public void Execute(object parameter) {
			ItemsSourceWizard.Run(XpfModelItem.ToModelItem((IModelItem)parameter));
		}
		public bool CanExecute(object parameter) {
			return true;
		}
		public event EventHandler CanExecuteChanged { add { } remove { } }
	}
#endif
	public class ShowMaskWizardCommand : ShowMaskWizardCommandCore {
		#region ICommand Members
		protected override IMaskProperties GetMaskProperties(IModelItem source) {
			TextEdit edit = source.View.PlatformObject as TextEdit;
			MaskProperties properties = new MaskProperties() {
				Mask = edit.Mask,
				MaskAutoComplete = edit.MaskAutoComplete,
#if !SL
				MaskBeepOnError = edit.MaskBeepOnError,
#endif
				MaskCulture = edit.MaskCulture,
				MaskIgnoreBlank = edit.MaskIgnoreBlank,
				MaskPlaceHolder = edit.MaskPlaceHolder,
				MaskSaveLiteral = edit.MaskSaveLiteral,
				MaskShowPlaceHolders = edit.MaskShowPlaceHolders,
				MaskType = edit.MaskType,
				MaskUseAsDisplayFormat = edit.MaskUseAsDisplayFormat,
				SupportedMaskTypes = EditorMaskHelper.GetSupportedMaskTypes(edit.GetType())
			};
			return properties;
		}
		#endregion
	}
	public abstract class PropertyLineCommandCore : ICommand {
		public bool CanExecute(object parameter) { return true; }
		public abstract void Execute(object parameter);
		public event EventHandler CanExecuteChanged { add { } remove { } }
		protected void OpenSmartTag(IModelItem editor) {
			ChangeSmartTagState(editor, true);
		}
		protected void CloseSmartTag(IModelItem editor) {
			ChangeSmartTagState(editor, false);
		}
		protected void ChangeSmartTagState(IModelItem editor, bool isOpen) {
			SmartTagDesignService service = GetSmartTagService(editor);
			if(service != null) service.IsSmartTagButtonPressed = isOpen;
		}
		protected static SmartTagDesignService GetSmartTagService(IModelItem editor) {
			EditingContext context = XpfModelItem.ToModelItem(editor).Context;
			return context != null ? XpfModelItem.ToModelItem(editor).Context.Services.GetService<SmartTagDesignService>() : null;
		}
	}
	public abstract class ShowMaskWizardCommandCore : PropertyLineCommandCore {
		public override void Execute(object parameter) {
			IModelItem editor = parameter as IModelItem;
			EditMaskWizardParams wizardParams = new EditMaskWizardParams() { Input = GetMaskProperties(editor) };
			EditMaskWizardWindow wizard = new EditMaskWizardWindow() { DataContext = wizardParams };
			EditMaskWizard maskWizard = new EditMaskWizard();
			maskWizard.DataContext = wizardParams;
			DesignDialogHelper.ShowDialog(wizard);
			if(wizardParams.Output != null) {
				PropertyInfo[] properties = typeof(IMaskProperties).GetProperties();
				foreach(PropertyInfo property in properties) {
					if(property.Name == "SupportedMaskTypes") continue;
					object v1 = property.GetValue(wizardParams.Input, null);
					object v2 = property.GetValue(wizardParams.Output, null);
					if(!object.Equals(v1, v2)) {
						if(property.Name == "MaskPlaceHolder" && ((char)v2) == ((char)0))
							editor.Properties[property.Name].ClearValue();
						else
							editor.Properties[property.Name].SetValue(v2);
					}
				}
			}
		}
		protected abstract IMaskProperties GetMaskProperties(IModelItem source);
		protected static MaskProperties ApplyMaskProperties(TextEditSettings settings, Type editorType) {
			MaskProperties properties = new MaskProperties() {
				Mask = settings.Mask,
				MaskAutoComplete = settings.MaskAutoComplete,
#if !SL
				MaskBeepOnError = settings.MaskBeepOnError,
#endif
				MaskCulture = settings.MaskCulture,
				MaskIgnoreBlank = settings.MaskIgnoreBlank,
				MaskPlaceHolder = settings.MaskPlaceHolder,
				MaskSaveLiteral = settings.MaskSaveLiteral,
				MaskShowPlaceHolders = settings.MaskShowPlaceHolders,
				MaskType = settings.MaskType,
				MaskUseAsDisplayFormat = settings.MaskUseAsDisplayFormat,
				SupportedMaskTypes = EditorMaskHelper.GetSupportedMaskTypes(editorType)
			};
			return properties;
		}
	}
	static class EditorMaskHelper {
		static Dictionary<Type, MaskType[]> supportedMaskTypes;
		static public MaskType[] GetSupportedMaskTypes(Type type) {
			if(supportedMaskTypes == null) CreateSupportedMaskTypes();
			if(supportedMaskTypes.Keys.Contains(type))
				return supportedMaskTypes[type];
			return new MaskType[] { };
		}
		static private void CreateSupportedMaskTypes() {
			supportedMaskTypes = new Dictionary<Type, MaskType[]>();
			supportedMaskTypes.Add(typeof(TextEdit), GetAllMaskTypes());
			supportedMaskTypes.Add(typeof(ButtonEdit), GetAllMaskTypes());
			supportedMaskTypes.Add(typeof(SpinEdit), GetNumericMaskType());
			supportedMaskTypes.Add(typeof(DateEdit), GetDateTimeMaskTypes());
		}
		static public MaskType[] GetAllMaskTypes() {
			List<MaskType> maskTypes = new List<MaskType>() { MaskType.None, MaskType.RegEx, MaskType.Regular, MaskType.Simple };
			maskTypes.AddRange(GetNumericMaskType());
			maskTypes.AddRange(GetDateTimeMaskTypes());
			return maskTypes.ToArray();
		}
		static public MaskType[] GetDateTimeMaskTypes() {
			return new MaskType[] { MaskType.DateTime, MaskType.DateTimeAdvancingCaret };
		}
		static public MaskType[] GetNumericMaskType() {
			return new MaskType[] { MaskType.Numeric };
		}
		public static Tuple<string, MaskType> GetSimpleNumericMask() {
			return new Tuple<string, MaskType>("99999", MaskType.Simple);
		}
		public static Tuple<string, MaskType> GetDateTimeMask() {
			return new Tuple<string, MaskType>("d", MaskType.DateTime);
		}
		public static Tuple<string, MaskType> GetFloatMask() {
			return new Tuple<string, MaskType>("f", MaskType.Numeric);
		}
	}
	public sealed class ButtonInfoPropertyLinesProvider : PropertyLinesProviderBase {
		public ButtonInfoPropertyLinesProvider() : base(typeof(ButtonInfo)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			if(viewModel.RuntimeSelectedItem.ItemType == typeof(ButtonInfo)) {
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ButtonInfo.ContentProperty)));
				lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ButtonInfo.CommandProperty)));
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ButtonInfo.CommandParameterProperty)));
			}
			return lines;
		}
	}
	public sealed class SpinButtonInfoPropertyLinesProvider : PropertyLinesProviderBase {
		public SpinButtonInfoPropertyLinesProvider() : base(typeof(SpinButtonInfo)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SpinButtonInfo.SpinStyleProperty), typeof(SpinStyle)));
			return lines;
		}
	}
#if !SL
	public class DateTimePickerPropertyLinesProvider : PropertyLinesProviderBase {
		public DateTimePickerPropertyLinesProvider() : base(typeof(DateTimePicker)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateTimePicker.DateTimeProperty)) { Mask = EditorMaskHelper.GetDateTimeMask() });
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateTimePicker.MinValueProperty)) { Mask = EditorMaskHelper.GetDateTimeMask() });
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateTimePicker.MaxValueProperty)) { Mask = EditorMaskHelper.GetDateTimeMask() });
			return lines;
		}
	}
	public sealed class RangeControlPropertyLinesProvider : PropertyLinesProviderBase {
		public RangeControlPropertyLinesProvider() : base(typeof(RangeControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => GetRangeControlClientProperty(viewModel));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.RangeStartProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.RangeEndProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.VisibleRangeStartProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.VisibleRangeEndProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.SelectionRangeStartProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.SelectionRangeEndProperty)));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.AllowSnapToIntervalProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.AllowScrollProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.AllowZoomProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.AllowImmediateRangeUpdateProperty)));
			return lines;
		}
		ItemListPropertyLineViewModel GetRangeControlClientProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			ItemListPropertyLineViewModel propertyLineViewModel =
				new ItemListPropertyLineViewModel(
					viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeControl.ClientProperty), typeof(IRangeControlClient), FindRangeControlClientTypes()) { AutoCompleteInfo = new AutoCompleteInfo() };
			propertyLineViewModel.PropertyChanged += OnClientPropertyChanged;
			return propertyLineViewModel;
		}
		void OnClientPropertyChanged(object sender, PropertyChangedEventArgs e) {
		}
		private IEnumerable<InstanceSourceBase> FindRangeControlClientTypes() {
			var type = typeof(IRangeControlClient);
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
			List<DXTypeInfo> types = new List<DXTypeInfo>();
			foreach(Assembly asm in assemblies) {
				var asmTypes = GetTypesSafe(asm);
				types.AddRange(asmTypes.Where(t => !t.IsAbstract && type.IsAssignableFrom(t)).Select(t => DXTypeInfo.FromType(t)).ToArray());
			}
			return DXTypeInfoInstanceSource.FromDXTypeInfoList(types);
		}
		private IEnumerable<Type> GetTypesSafe(Assembly assembly) {
			try {
				return assembly.GetTypes();
			} catch {
				return new Type[0];
			}
		}
	}
#endif
	public sealed class DateNavigatorPropertyLinesProvider : PropertyLinesProviderBase {
		public DateNavigatorPropertyLinesProvider() : base(typeof(DateNavigator)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateNavigator.CalendarViewProperty), typeof(DateNavigatorCalendarView)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateNavigator.FocusedDateProperty)) { Mask = EditorMaskHelper.GetDateTimeMask() });
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateNavigator.IsMultiSelectProperty)));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			return lines;
		}
	}
	public class DateEditCalendarPropertyLinesProvider : PropertyLinesProviderBase {
		public DateEditCalendarPropertyLinesProvider() : base(typeof(DateEditCalendar)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateEditCalendar.DateTimeProperty)) { Mask = EditorMaskHelper.GetDateTimeMask() });
			return lines;
		}
	}
	public class BaseEditPropertyProvider : PropertyLinesProviderBase {
		public BaseEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected virtual bool AddEditValueProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return true; }
		protected virtual PropertyTarget EditValuePropertyTarget(FrameworkElementSmartTagPropertiesViewModel viewModel) { return PropertyTarget.ExceptGrid; }
		protected virtual bool IsEditValueReadOnly(FrameworkElementSmartTagPropertiesViewModel viewModel) { return false; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			if(AddEditValueProperty(viewModel))
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.EditValueProperty)) { IsReadOnly = IsEditValueReadOnly(viewModel) }, EditValuePropertyTarget(viewModel));
			return lines;
		}
		protected void AddMaskProperties(SmartTagLineViewModelFactoryList lines, FrameworkElementSmartTagPropertiesViewModel viewModel, MaskType[] types) {
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => TextEdit.MaskProperty)) { Command = GetShowMaskWizardComand(), CommandParameter = viewModel.RuntimeSelectedItem }, PropertyTarget.All);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, GetPropertyName(() => TextEdit.MaskTypeProperty), typeof(MaskType), null, values => types), PropertyTarget.ExceptGrid);
		}
		ICommand GetShowMaskWizardComand() {
			return (Target & PropertyTarget.Grid) != 0 ? (ICommand)new ShowGridMaskWizardCommand() : new ShowMaskWizardCommand();
		}
		protected virtual IEnumerable<InstanceSourceBase> GetStyleSettings() {
			return new InstanceSourceBase[] { };
		}
		protected object GetEditorByModelItem(IModelItem item) {
			if(item.View == null)
				return item.GetCurrentValue() as BaseEditSettings;
			return item.View.PlatformObject;
		}
		protected SmartTagLineViewModelFactory GetStyleSettingsProperty(FrameworkElementSmartTagPropertiesViewModel viewModel, Type settingsBaseType, PropertyTarget propertyTarget) {
			return new SmartTagLineViewModelFactory(() => new ItemListPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.StyleSettingsProperty), settingsBaseType, GetStyleSettings()), propertyTarget);
		}
	}
	public class TextEditPropertyProvider : BaseEditPropertyProvider {
		public TextEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => TextEditBase.TextProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.IsReadOnlyProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.AllowNullInputProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.NullTextProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.NullValueProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			AddMaskProperties(lines, viewModel, EditorMaskHelper.GetAllMaskTypes());
			return lines;
		}
	}
	public class ButtonEditPropertyProvider : BaseEditPropertyProvider {
		public ButtonEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => TextEditBase.TextProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.IsReadOnlyProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new NullableBooleanPropertyLineViewModel(viewModel, GetPropertyName(() => ButtonEdit.IsTextEditableProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.AllowNullInputProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.NullTextProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.NullValueProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			AddMaskProperties(lines, viewModel, EditorMaskHelper.GetAllMaskTypes());
			return lines;
		}
	}
	public class PasswordBoxEditPropertyProvider : BaseEditPropertyProvider {
		public PasswordBoxEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => TextEditBase.TextProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.IsReadOnlyProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.AllowNullInputProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.NullTextProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => BaseEdit.NullValueProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PasswordBoxEditSettings.PasswordCharProperty)) { ItemsSource = new object[] { '*', (char)9679, 'x', '#', 'o' } }, PropertyTarget.All);
			return lines;
		}
	}
	public class SpinEditPropertyProvider : BaseEditPropertyProvider {
		public SpinEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SpinEdit.MinValueProperty)) { Mask = EditorMaskHelper.GetFloatMask() }, PropertyTarget.All);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SpinEdit.MaxValueProperty)) { Mask = EditorMaskHelper.GetFloatMask() }, PropertyTarget.All);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BaseEdit.IsReadOnlyProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new NullableBooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ButtonEdit.IsTextEditableProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SpinEdit.AllowRoundOutOfRangeValueProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			AddMaskProperties(lines, viewModel, EditorMaskHelper.GetNumericMaskType());
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SpinEdit.IncrementProperty)), PropertyTarget.All);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SpinEdit.ShowEditorButtonsProperty)), PropertyTarget.ExceptGrid);
			return lines;
		}
	}
	public class CheckEditPropertyProvider : BaseEditPropertyProvider {
		public CheckEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => CheckEdit.IsCheckedProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BaseEdit.IsReadOnlyProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => CheckEdit.ContentProperty)), PropertyTarget.All);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => CheckEdit.IsThreeStateProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => CheckEdit.ClickModeProperty), typeof(ClickMode)), PropertyTarget.ExceptGrid);
			return lines;
		}
	}
	public class DateEditPropertyProvider : BaseEditPropertyProvider {
		public DateEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
#if !SL
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, GetPropertyName(() => DateEdit.StyleSettingsProperty), typeof(BaseComboBoxStyleSettings), GetDateEditStyleSettings()), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
#endif
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateEdit.DateTimeProperty)) { Mask = EditorMaskHelper.GetDateTimeMask() }, PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateEdit.MinValueProperty)) { Mask = EditorMaskHelper.GetDateTimeMask() }, PropertyTarget.All);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DateEdit.MaxValueProperty)) { Mask = EditorMaskHelper.GetDateTimeMask() }, PropertyTarget.All);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BaseEdit.IsReadOnlyProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new NullableBooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ButtonEdit.IsTextEditableProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SpinEdit.AllowRoundOutOfRangeValueProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			AddMaskProperties(lines, viewModel, EditorMaskHelper.GetNumericMaskType());
			return lines;
		}
#if !SL
		IEnumerable<InstanceSourceBase> GetDateEditStyleSettings() {
			return DXTypeInfoInstanceSource.FromDXTypeInfoList(new DXTypeInfo[] {
				DXTypeInfo.FromType(typeof(DateEditCalendarStyleSettings)),
				DXTypeInfo.FromType(typeof(DateEditPickerStyleSettings))
			});
		}
#endif
	}
	public class MemoEditPropertyProvider : BaseEditPropertyProvider {
		public MemoEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override bool AddEditValueProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return false; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => MemoEdit.ShowIconProperty)), PropertyTarget.All);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => MemoEdit.MemoTextWrappingProperty), typeof(TextWrapping)), PropertyTarget.All);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PopupBaseEditSettings.PopupMaxWidthProperty)), PropertyTarget.Grid);
			return lines;
		}
	}
	public abstract class ItemsSourceSupportPropertyProvider : BaseEditPropertyProvider {
		IModelSubscribedEvent onItemsSourceChangedSubscribedEvent;
		IModelItem currentItem;
		public ItemsSourceSupportPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			if(Target == PropertyTarget.ExceptGrid) {
				if(currentItem != null) currentItem.UnsubscribeFromPropertyChanged(onItemsSourceChangedSubscribedEvent);
				currentItem = viewModel.RuntimeSelectedItem;
				if(currentItem != null) onItemsSourceChangedSubscribedEvent = currentItem.SubscribeToPropertyChanged(new EventHandler(OnItemsSourceChanged));
			}
			viewModel.PropertyChanged += viewModel_PropertyChanged;
			return base.GetPropertiesImpl(viewModel);
		}
		ObjectPropertyLineViewModel DisplayMemberLine { get; set; }
		ObjectPropertyLineViewModel ValueMemberLine { get; set; }
		protected ObjectPropertyLineViewModel CreateDisplayMemberProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			DisplayMemberLine = new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.DisplayMemberProperty)) {
				ItemsSource = GetItemsSourcePropertiesNames(GetEditorByModelItem(viewModel.RuntimeSelectedItem))
			};
			return DisplayMemberLine;
		}
		void viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "ItemsSource") {
				var source = GetItemsSourcePropertiesNames(GetEditorByModelItem(DisplayMemberLine.Context.ModelItem));
				DisplayMemberLine.ItemsSource = source;
				ValueMemberLine.ItemsSource = source;
			}
		}
		protected ObjectPropertyLineViewModel CreateValueMemberProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			ValueMemberLine = new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.ValueMemberProperty)) {
				ItemsSource = GetItemsSourcePropertiesNames(GetEditorByModelItem(viewModel.RuntimeSelectedItem))
			};
			return ValueMemberLine;
		}
		IEnumerable<string> GetItemsSourcePropertiesNames(object edit) {
			return GetPropertiesNames(edit, GetPropertyName(() => LookUpEditBase.ItemsSourceProperty));
		}
		internal static IEnumerable<string> GetPropertiesNames(object edit, string propertyName) {
			if(edit == null) return new List<string>();
			PropertyDescriptorCollection editProps = TypeDescriptor.GetProperties(edit);
			PropertyDescriptor itemsSourceDesc = editProps.Find(propertyName, false);
			var value = itemsSourceDesc.GetValue(edit);
			if(value == null) return new List<string>();
			if((value as ICollectionView) != null) return new List<string>();
			Type genericType = null;
			Type valueType = value.GetType();
			if(valueType != null && valueType.IsGenericType) {
				Type[] genericArgs = valueType.GetGenericArguments();
				genericType = genericArgs.Length > 0 ? genericArgs[0] : null;
			}
			PropertyDescriptorCollection props = genericType == null ? TypeDescriptor.GetProperties(value) : TypeDescriptor.GetProperties(genericType);
			List<string> members = new List<string>();
			foreach(PropertyDescriptor desc in props) {
				members.Add(desc.Name);
			}
			return members;
		}
		void OnItemsSourceChanged(object sender, EventArgs a) {
			PropertyChangedEventArgs e = (PropertyChangedEventArgs)a;
			if(e.PropertyName == "ItemsSource") {
				ModelItem item = sender as ModelItem;
				object edit = null;
				if(item != null)
					edit = item.View.PlatformObject;
				List<string> members = (List<string>)GetItemsSourcePropertiesNames(edit);
				DisplayMemberLine.ItemsSource = members;
				ValueMemberLine.ItemsSource = members;
			}
		}
	}
#if !SL
	public class SparkLineEditPropertyProvider : BaseEditPropertyProvider {
		public SparkLineEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(GetStyleSettingsProperty(viewModel, typeof(SparklineStyleSettings), PropertyTarget.All));
			return lines;
		}
		protected override IEnumerable<InstanceSourceBase> GetStyleSettings() {
			return DXTypeInfoInstanceSource.FromDXTypeInfoList(new DXTypeInfo[] {
				DXTypeInfo.FromType(typeof(LineSparklineStyleSettings)),
				DXTypeInfo.FromType(typeof(AreaSparklineStyleSettings)),
				DXTypeInfo.FromType(typeof(BarSparklineStyleSettings)),
			});
		}
	}
#endif
	public class LookUpEditPropertyProviderBase : ItemsSourceSupportPropertyProvider {
		public LookUpEditPropertyProviderBase(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.SelectedIndexProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.SelectedItemProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
#if !SL
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.ItemsSourceProperty)) { Command = new ShowItemsSourceWizard(), CommandParameter = viewModel.RuntimeSelectedItem }, PropertyTarget.All);
#endif
			lines.Add(() => CreateDisplayMemberProperty(viewModel), PropertyTarget.All);
			lines.Add(() => CreateValueMemberProperty(viewModel), PropertyTarget.All);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.IsSynchronizedWithCurrentItemProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.AllowCollectionViewProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.AutoCompleteProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new NullableBooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.IncrementalFilteringProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.ImmediatePopupProperty)), PropertyTarget.ExceptGrid);
			return lines;
		}
	}
	public class ComboBoxEditPropertyProvider : LookUpEditPropertyProviderBase {
		public ComboBoxEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(GetStyleSettingsProperty(viewModel, typeof(BaseComboBoxStyleSettings), PropertyTarget.ExceptGrid));
			return lines;
		}
		protected override IEnumerable<InstanceSourceBase> GetStyleSettings() {
			return DXTypeInfoInstanceSource.FromDXTypeInfoList(new DXTypeInfo[] {
				DXTypeInfo.FromType(typeof(ComboBoxStyleSettings)),
				DXTypeInfo.FromType(typeof(CheckedComboBoxStyleSettings)),
				DXTypeInfo.FromType(typeof(RadioComboBoxStyleSettings)),
			});
		}
	}
	public class ListBoxEditPropertyProvider : ItemsSourceSupportPropertyProvider {
		public ListBoxEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.SelectedIndexProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.SelectedItemProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
#if !SL
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.ItemsSourceProperty)) { Command = new ShowItemsSourceWizard(), CommandParameter = viewModel.RuntimeSelectedItem }, PropertyTarget.All);
#endif
			lines.Add(() => CreateDisplayMemberProperty(viewModel), PropertyTarget.All);
			lines.Add(() => CreateValueMemberProperty(viewModel), PropertyTarget.All);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.IsSynchronizedWithCurrentItemProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LookUpEditBase.AllowCollectionViewProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ListBoxEdit.SelectionModeProperty), typeof(SelectionMode)), PropertyTarget.ExceptGrid);
			lines.Add(GetStyleSettingsProperty(viewModel, typeof(ListBoxEditStyleSettings), PropertyTarget.ExceptGrid));
			return lines;
		}
		protected override IEnumerable<InstanceSourceBase> GetStyleSettings() {
			return DXTypeInfoInstanceSource.FromDXTypeInfoList(new DXTypeInfo[] {
				DXTypeInfo.FromType(typeof(CheckedListBoxEditStyleSettings)),
				DXTypeInfo.FromType(typeof(ListBoxEditStyleSettings)),
				DXTypeInfo.FromType(typeof(RadioListBoxEditStyleSettings)),
			});
		}
	}
	public class FontEditPropertyProvider : LookUpEditPropertyProviderBase {
		public FontEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, GetPropertyName(() => FontEdit.FontProperty), typeof(BaseComboBoxStyleSettings), GetFontFamiles()) { AutoCompleteInfo = new AutoCompleteInfo() }, PropertyTarget.All);
			return lines;
		}
		IEnumerable<InstanceSourceBase> GetFontFamiles() {
			return ObjectInstanceSource.FromList(GetFonts());
		}
		private IEnumerable<object> GetFonts() {
			return GetSystemFonts();
		}
		static List<FontFamily> GetSystemFonts() {
			List<FontFamily> result = new List<FontFamily>();
#if SL
			foreach(string fontFamily in FontManager.GetFontFamilyNames()) {
#else
			var lang = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
			foreach(FontFamily fontFamily in Fonts.SystemFontFamilies) {
				if(IsValid(fontFamily)) {
#endif
#if !SL
					var name = fontFamily.FamilyNames.ContainsKey(lang) ? fontFamily.FamilyNames[lang] : fontFamily.ToString();
#else
				{
					var name = fontFamily.ToString();
#endif
					result.Add(new FontFamily(name));
				}
			}
			result.Sort(new FontFamilyComparer());
			return result;
		}
#if !SL
		static bool IsValid(FontFamily fontFamily) {
			GlyphTypeface glyphTypeface;
			foreach(Typeface typeface in fontFamily.GetTypefaces()) {
				try {
					typeface.TryGetGlyphTypeface(out glyphTypeface);
				} catch {
					return false;
				}
			}
			return true;
		}
#endif
		class FontFamilyComparer : IComparer<FontFamily> {
			int IComparer<FontFamily>.Compare(FontFamily x, FontFamily y) {
				return x.ToString().CompareTo(y.ToString());
			}
		}
	}
	public class RangeBaseEditPropertyProvider : BaseEditPropertyProvider {
		public RangeBaseEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected virtual bool AddContentDisplayModeProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return true; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => RangeBaseEdit.ValueProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => RangeBaseEdit.MinimumProperty)) { Mask = EditorMaskHelper.GetFloatMask() }, PropertyTarget.All);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => RangeBaseEdit.MaximumProperty)) { Mask = EditorMaskHelper.GetFloatMask() }, PropertyTarget.All);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => RangeBaseEdit.SmallStepProperty)) { Mask = EditorMaskHelper.GetFloatMask() }, PropertyTarget.All);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => RangeBaseEdit.LargeStepProperty)) { Mask = EditorMaskHelper.GetFloatMask() }, PropertyTarget.All);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, GetPropertyName(() => RangeBaseEdit.OrientationProperty), typeof(Orientation)), PropertyTarget.All);
			return lines;
		}
	}
	public sealed class ProgressBarEditPropertyProvider : RangeBaseEditPropertyProvider {
		public ProgressBarEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, GetPropertyName(() => ProgressBarEdit.StyleSettingsProperty), typeof(BaseProgressBarStyleSettings), GetProgressBarStyleSettings()) { AutoCompleteInfo = new AutoCompleteInfo() }, PropertyTarget.All);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			if (AddContentDisplayModeProperty(viewModel))
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, GetPropertyName(() => ProgressBarEdit.ContentDisplayModeProperty), typeof(ContentDisplayMode)), PropertyTarget.All);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, GetPropertyName(() => ProgressBarEdit.IsPercentProperty)), PropertyTarget.All);
			return lines;
		}
		IEnumerable<InstanceSourceBase> GetProgressBarStyleSettings() {
			return DXTypeInfoInstanceSource.FromDXTypeInfoList(new DXTypeInfo[] {
				DXTypeInfo.FromType(typeof(ProgressBarStyleSettings)), 
				DXTypeInfo.FromType(typeof(ProgressBarMarqueeStyleSettings))
			});
		}
	}
	public sealed class TrackBarEditPropertyProvider : RangeBaseEditPropertyProvider {
		public TrackBarEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override bool AddContentDisplayModeProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return false; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => TrackBarEdit.SelectionStartProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => TrackBarEdit.SelectionEndProperty)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, GetPropertyName(() => TrackBarEdit.StyleSettingsProperty), typeof(TrackBarStyleSettings), GetTrackBarStyleSettings()) { AutoCompleteInfo = new AutoCompleteInfo() }, PropertyTarget.All);
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, GetPropertyName(() => TrackBarEdit.TickPlacementProperty), typeof(TickPlacement)), PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => TrackBarEdit.TickFrequencyProperty)), PropertyTarget.All);
			return lines;
		}
		IEnumerable<InstanceSourceBase> GetTrackBarStyleSettings() {
			return DXTypeInfoInstanceSource.FromDXTypeInfoList(new DXTypeInfo[] {
				DXTypeInfo.FromType(typeof(TrackBarStyleSettings)), 
				DXTypeInfo.FromType(typeof(TrackBarZoomStyleSettings)), 
				DXTypeInfo.FromType(typeof(TrackBarRangeStyleSettings)), 
				DXTypeInfo.FromType(typeof(TrackBarZoomRangeStyleSettings))
			});
		}
	}
	public class ImageEditPropertyProvider : BaseEditPropertyProvider {
		public ImageEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override bool IsEditValueReadOnly(FrameworkElementSmartTagPropertiesViewModel viewModel) { return true; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
#if !SL
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, GetPropertyName(() => ImageEdit.SourceProperty)), PropertyTarget.ExceptGrid);
#else
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => ImageEdit.SourceProperty)), PropertyTarget.ExceptGrid);
#endif
			lines.Add(() => new SeparatorLineViewModel(viewModel), PropertyTarget.ExceptGrid);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, GetPropertyName(() => ImageEdit.ShowMenuProperty)), PropertyTarget.All);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, GetPropertyName(() => ImageEdit.ShowMenuModeProperty), typeof(ShowMenuMode)), PropertyTarget.All);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, GetPropertyName(() => ImageEdit.StretchProperty), typeof(Stretch)), PropertyTarget.All);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, GetPropertyName(() => ImageEdit.ShowLoadDialogOnClickModeProperty), typeof(ShowLoadDialogOnClickMode)), PropertyTarget.Grid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => ImageEditSettings.MaxWidthProperty)), PropertyTarget.Grid);
			return lines;
		}
	}
	public class CalcEditPropertyProvider : BaseEditPropertyProvider {
		public CalcEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override PropertyTarget EditValuePropertyTarget(FrameworkElementSmartTagPropertiesViewModel viewModel) { return PropertyTarget.PropertyGrid | PropertyTarget.Bar; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => Calculator.ValueProperty)) { Mask = EditorMaskHelper.GetFloatMask() }, PropertyTarget.ExceptGrid);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => Calculator.PrecisionProperty)) { Mask = EditorMaskHelper.GetSimpleNumericMask() }, PropertyTarget.All);
			return lines;
		}
	}
	public class ColorEditPropertyProvider : BaseEditPropertyProvider {
		public ColorEditPropertyProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
#if !SL
			lines.Add(() => new ColorPropertyLineViewModel(viewModel, GetPropertyName(() => ColorEdit.ColorProperty)), PropertyTarget.ExceptGrid);
#else
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, GetPropertyName(() => ColorEdit.ColorProperty)), PropertyTarget.ExceptGrid);
#endif
			return lines;
		}
	}
#if SL
	public class DataPagerPropertyLinesProvider : PropertyLinesProviderBase {
		public DataPagerPropertyLinesProvider() : base(typeof(DataPager)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataPager.PageIndexProperty)) { Mask = EditorMaskHelper.GetSimpleNumericMask() });
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataPager.PagedSourceProperty)));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataPager.DisplayModeProperty), typeof(DataPagerDisplayMode)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataPager.CanChangePageProperty)));
			return lines;
		}
	}
#endif
}
