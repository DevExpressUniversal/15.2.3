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

using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using DevExpress.Mvvm.Native;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DevExpress.Data;
using System.Windows.Markup;
using System.Windows.Data;
using System.Collections.Specialized;
using DevExpress.Xpf.Reports.UserDesigner.Editors.DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class FormatStringUITypeEditor : Control {
		public ICommand AddFormatCommand { get; private set; }
		public ICommand RemoveFormatCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }
#if DEBUGTEST
		internal
#endif
		string ActualFormat {
			get {
				return IsGeneralCategorySelected
					? string.Empty
					: !IsCustomFormats
					? SelectedStandardFormat
					: SelectedCustomFormat;
			}
		}
#if DEBUGTEST
		internal
#endif
			string ActualPreffix {
			get {
				return IsGeneralCategorySelected && !IsCustomFormats
				  ? GeneralFormatPrefix
				  : string.Empty;
			}
		}
#if DEBUGTEST
		internal
#endif
			string ActualSuffix {
			get {
				return IsGeneralCategorySelected && !IsCustomFormats
				  ? GeneralFormatSuffix
				  : string.Empty;
			}
		}
#if DEBUGTEST
		internal
#endif
			object ActualCategoryValue {
			get {
				return IsGeneralCategorySelected && !IsCustomFormats
					? "###"
					: SelectedCategory.Value;
			}
		}
		string InitialFormat { get; set; }
		string ResultFormat {
			get {
				return IsGeneralCategorySelected
				? ActualPreffix + "{0}" + ActualSuffix
				: MailMergeFieldInfo.MakeFormatString(ActualFormat);
			}
		}
		public static readonly DependencyProperty PrimarySelectionProperty;
		public static readonly DependencyProperty EditValueProperty;
		static readonly DependencyPropertyKey CategoriesPropertyKey;
		public static readonly DependencyProperty CategoriesProperty;
		public static readonly DependencyProperty SelectedCategoryProperty;
		public static readonly DependencyProperty SelectedStandardFormatProperty;
		public static readonly DependencyProperty SelectedCustomFormatProperty;
		public static readonly DependencyProperty IsCustomFormatsProperty;
		static readonly DependencyPropertyKey IsGeneralCategorySelectedPropertyKey;
		public static readonly DependencyProperty IsGeneralCategorySelectedProperty;
		public static readonly DependencyProperty GeneralFormatPrefixProperty;
		public static readonly DependencyProperty GeneralFormatSuffixProperty;
		public static readonly DependencyProperty NewFormatProperty;
		static readonly DependencyPropertyKey SampleTextPropertyKey;
		public static readonly DependencyProperty SampleTextProperty;
		public string EditValue {
			get { return (string)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public IEnumerable<FormatStringCategory> Categories {
			get { return (IEnumerable<FormatStringCategory>)GetValue(CategoriesProperty); }
			private set { SetValue(CategoriesPropertyKey, value); }
		}
		public FormatStringCategory SelectedCategory {
			get { return (FormatStringCategory)GetValue(SelectedCategoryProperty); }
			set { SetValue(SelectedCategoryProperty, value); }
		}
		public string SelectedStandardFormat {
			get { return (string)GetValue(SelectedStandardFormatProperty); }
			set { SetValue(SelectedStandardFormatProperty, value); }
		}
		public string SelectedCustomFormat {
			get { return (string)GetValue(SelectedCustomFormatProperty); }
			set { SetValue(SelectedCustomFormatProperty, value); }
		}
		public bool IsGeneralCategorySelected {
			get { return (bool)GetValue(IsGeneralCategorySelectedProperty); }
			private set { SetValue(IsGeneralCategorySelectedPropertyKey, value); }
		}
		public string GeneralFormatPrefix {
			get { return (string)GetValue(GeneralFormatPrefixProperty); }
			set { SetValue(GeneralFormatPrefixProperty, value); }
		}
		public string GeneralFormatSuffix {
			get { return (string)GetValue(GeneralFormatSuffixProperty); }
			set { SetValue(GeneralFormatSuffixProperty, value); }
		}
		public bool IsCustomFormats {
			get { return (bool)GetValue(IsCustomFormatsProperty); }
			set { SetValue(IsCustomFormatsProperty, value); }
		}
		public string SampleText {
			get { return (string)GetValue(SampleTextProperty); }
			private set { SetValue(SampleTextPropertyKey, value); }
		}
		public object PrimarySelection {
			get { return GetValue(PrimarySelectionProperty); }
			set { SetValue(PrimarySelectionProperty, value); }
		}
		public string NewFormat {
			get { return (string)GetValue(NewFormatProperty); }
			set { SetValue(NewFormatProperty, value); }
		}
		#region ctor & initializeCommands
		static FormatStringUITypeEditor() {
			DependencyPropertyRegistrator<FormatStringUITypeEditor>.New()
				.Register(owner => owner.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.RegisterReadOnly(owner => owner.Categories, out CategoriesPropertyKey, out CategoriesProperty, null)
				.Register(owner => owner.SelectedCategory, out SelectedCategoryProperty, null, d => d.OnCategoryChanged())
				.Register(owner => owner.SelectedStandardFormat, out SelectedStandardFormatProperty, null, (d, args) => d.UpdateSample())
				.Register(owner => owner.SelectedCustomFormat, out SelectedCustomFormatProperty, null, (d, args) => d.UpdateSample())
				.RegisterReadOnly(owner => owner.IsGeneralCategorySelected, out IsGeneralCategorySelectedPropertyKey, out IsGeneralCategorySelectedProperty, false)
				.Register(owner => owner.GeneralFormatPrefix, out GeneralFormatPrefixProperty, null, d => d.UpdateSample())
				.Register(owner => owner.GeneralFormatSuffix, out GeneralFormatSuffixProperty, null, d => d.UpdateSample())
				.RegisterReadOnly(owner => owner.SampleText, out SampleTextPropertyKey, out SampleTextProperty, null)
				.Register(owner => owner.PrimarySelection, out PrimarySelectionProperty, null, d => d.OnPrimarySelectionChanged())
				.Register(owner => owner.NewFormat, out NewFormatProperty, null)
				.Register(owner => owner.IsCustomFormats, out IsCustomFormatsProperty, false, d => d.UpdateSample())
				.OverrideDefaultStyleKey();
		}
		public FormatStringUITypeEditor() {
			InitializeCommands();
			InitialFormat = string.Empty;
			Categories = FormatStringCategory.GetCategories();
			SelectedCategory = Categories.First();
			Loaded += (s, e) => SelectInitialFormat();
		}
		void InitializeCommands() {
			AddFormatCommand = DelegateCommandFactory.Create(AddFormat, CanAddFormat);
			RemoveFormatCommand = DelegateCommandFactory.Create<string>(RemoveFormat, CanRemoveFormat);
			SaveCommand = DelegateCommandFactory.Create(Save, CanSave);
		}
		#endregion
		void Save() {
			if (!CanSave())
				return;
			EditValue = ResultFormat;
		}
		bool CanSave() {
			return IsValidFormat(ResultFormat);
		}
		void AddFormat() {
			if (!CanAddFormat())
				return;
			SelectedCategory.AddFormat(NewFormat);
			SelectedCustomFormat = NewFormat;
			NewFormat = null;
		}
		bool CanAddFormat() {
			return !string.IsNullOrEmpty(NewFormat) 
				&& !SelectedCategory.Return(x=> x.CustomFormats.Contains(NewFormat), ()=> false)
				&& IsValidFormat(MailMergeFieldInfo.MakeFormatString(NewFormat));
		}
		void RemoveFormat(string format) {
			if (!CanRemoveFormat(format))
				return;
			SelectedCategory.RemoveFormat(format);
			SelectedCustomFormat = SelectedCategory.CustomFormats.FirstOrDefault();
		}
		bool CanRemoveFormat(string format) {
			return SelectedCategory.Return(x => x.RegistryFormats.Contains(format), () => false);
		}
		void OnPrimarySelectionChanged() {
			var categories = FormatStringCategory.GetCategories();
			Categories = ShouldHideGeneralCategory()
				? categories.Where(x => x.Name != FormatStringCategory.GeneralCategoryName)
				: categories;
		}
		void OnEditValueChanged() {
			if (!string.IsNullOrEmpty(InitialFormat))
				return;
			InitialFormat = Parse(EditValue) ?? string.Empty;
		}
		void OnCategoryChanged() {
			if (SelectedCategory == null)
				return;
			if (SelectedCategory.Name != FormatStringCategory.GeneralCategoryName) {
				IsGeneralCategorySelected = false;
				SelectedStandardFormat = SelectedCategory.StandardFormats.First();
				SelectedCustomFormat = SelectedCategory.CustomFormats.First();
			} else {
				IsGeneralCategorySelected = true;
			}
			UpdateSample();
		}
		void SelectInitialFormat() {
			var category = Categories.FirstOrDefault(x => x.StandardFormats.Concat(x.CustomFormats).Contains(InitialFormat));
			if (category != null) {
				SelectedCategory = category;
				if (category.CustomFormats.Contains(InitialFormat)) {
					IsCustomFormats = true;
					SelectedCustomFormat = InitialFormat;
				} else if (category.StandardFormats.Contains(InitialFormat))
					SelectedStandardFormat = InitialFormat;
			} else if (System.Text.RegularExpressions.Regex.IsMatch(InitialFormat, @"(.)*\{0\}(.)*")) {
				SelectedCategory = Categories.First(x=> x.Name == FormatStringCategory.GeneralCategoryName);
				IsCustomFormats = false;
				var valueIndex = InitialFormat.IndexOf("{0}");
				GeneralFormatPrefix = InitialFormat.Substring(0, valueIndex);
				GeneralFormatSuffix = InitialFormat.Substring(valueIndex + 3);
			} else SelectedCategory = Categories.First();
		}
		static string Parse(string format) {
			int index = !string.IsNullOrEmpty(format) ? format.IndexOf("{0:") : -1;
			if (index >= 0) {
				int closeBraceIndex = FindCorrespondingFigure(format, index);
				if (closeBraceIndex >= 0)
					return format.Substring(0, index) + format.Substring(index + 3, closeBraceIndex - index - 3) + format.Substring(closeBraceIndex + 1);
			}
			return format;
		}
		static int FindCorrespondingFigure(string format, int openFigureIndex) {
			int count = format.Length;
			int weight = 1;
			for (int i = openFigureIndex + 1; i < count; i++) {
				if (format[i] == '{')
					weight++;
				else if (format[i] == '}')
					weight--;
				if (weight == 0)
					return i;
			}
			return -1;
		}
		void UpdateSample() {
			string formattedValue;
			try {
				formattedValue = string.Format(MailMergeFieldInfo.MakeFormatString(IsCustomFormats ? SelectedCustomFormat : SelectedStandardFormat), ActualCategoryValue);
			} catch {
				formattedValue = "Error: Illegal symbol(s)"; 
			}
			SampleText = ActualPreffix + formattedValue + ActualSuffix;
		}
		bool IsValidFormat(string format) {
			try {
				string.Format(format, string.Empty);
				return true;
			} catch {
				return false;
			}
		}
		bool ShouldHideGeneralCategory() {
			if (PrimarySelection == null)
				return false;
			var fieldEmbedableControl = PrimarySelection as XRFieldEmbeddableControl;
			if (fieldEmbedableControl == null)
				return false;
			return (PrimarySelection as XRFieldEmbeddableControl).Return(x => !x.HasDisplayDataBinding, () => false);
		}
	}
	public enum FormatType {
		Standard,
		Custom
	}
	public class FormatTypeToSelectedTabConverter : MarkupExtension, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var formatType = (FormatType)value;
			return formatType == FormatType.Standard ? 0 : 1;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			var selectedTabIndex = (int)value;
			return (FormatType)selectedTabIndex;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	namespace DevExpress.Xpf.Reports.UserDesigner.Editors.Native {
		public class FormatStringCategory {
			public const string GeneralCategoryName = "General";
			public const string DateTimeTypeName = "DateTime";
			public const string NumberTypeName = "Int32";
			#region static
			static string RegistryPath { get { return XtraPrinting.Native.NativeSR.RegistryPath + "CustomFormat\\"; } }
			public static IEnumerable<FormatStringCategory> GetCategories() {
				var categories = new List<FormatStringCategory>();
				using (var stream = AssemblyHelper.GetResourceStream(typeof(FormatStringUITypeEditor).Assembly, "ReportDesignerParts/PropertyEditors/formatstring.xml", true)) {
					var formatsData = XDocument.Load(stream);
					var categoryNodes = formatsData.Descendants("Category");
					categoryNodes.ForEach((categoryNode => {
						var type = Type.GetType(categoryNode.Attribute("type").Value);
						var name = categoryNode.Attribute("Name").Value;
						var value = type == typeof(DateTime) 
							? DateTime.Now : GetValue(categoryNode.Attribute("value").Value, type);
						var standardFormats = GetStandardFormats(categoryNode, type);
						var customFormats = GetCustomFormats(formatsData, type);
						var registryFormats = GetRegistryFormats(name);
						var category = new FormatStringCategory(name, type, value, standardFormats, customFormats, registryFormats);
						categories.Add(category);
					}));
				}
				return categories.ToArray();
			}
			static IEnumerable<string> GetRegistryFormats(string name) {
				return XtraPrinting.Native.RegistryHelper.LoadRegistryValue(RegistryPath, name);
			}
			static IEnumerable<string> GetStandardFormats(XElement category, Type type) {
				return type == typeof(DateTime)
					? CultureInfo.CurrentCulture.GetAllDateTimePatterns()
					: category.Elements().Select(format => format.Value);
			}
			static IEnumerable<string> GetCustomFormats(XDocument source, Type type) {
				return source.Descendants("CustomFormats")
					.Where(formatsCollection => Type.GetType(formatsCollection.Attribute("type").Value) == type)
					.SelectMany(formatsCollection => formatsCollection.Elements(), (formatsCollection, format) => format.Value);
			}
			static object GetValue(string value, Type type) {
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				return converter.ConvertTo(value, type);
			}
			#endregion
			readonly ObservableCollection<string> customFormats = new ObservableCollection<string>();
			readonly List<string> registryFormats = new List<string>();
			public string Name { get; private set; }
			public Type Type { get; private set; }
			public object Value { get; private set; }
			public IEnumerable<string> StandardFormats { get; private set; }
			public IEnumerable<string> CustomFormats { get { return customFormats; } }
			internal IEnumerable<string> RegistryFormats { get { return registryFormats; } }
			FormatStringCategory(string name, Type type, object value, IEnumerable<string> standardFormats, IEnumerable<string> customFormats, IEnumerable<string> registryFormats) {
				this.Name = name;
				this.Type = type;
				this.Value = value;
				StandardFormats = standardFormats;
				this.registryFormats.AddRange(registryFormats);
				customFormats.Concat(registryFormats).ForEach(x => this.customFormats.Add(x));
				this.customFormats.CollectionChanged += OnCollectionChanged;
			}
			void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
				e.NewItems.Do(i=> i.Cast<string>().ForEach(x => registryFormats.Add(x)));
				e.OldItems.Do(i=> i.Cast<string>().ForEach(x => { if (registryFormats.Contains(x)) registryFormats.Remove(x); }));
			}
			public void RemoveFormat(string format) {
				customFormats.Remove(format);
				XtraPrinting.Native.RegistryHelper.DeleteRegistryValue(format, RegistryPath, Name);
			}
			public void AddFormat(string format) {
				customFormats.Add(format);
				XtraPrinting.Native.RegistryHelper.AddRegistryValue(format, RegistryPath, Name);
			}
		}
	}
}
