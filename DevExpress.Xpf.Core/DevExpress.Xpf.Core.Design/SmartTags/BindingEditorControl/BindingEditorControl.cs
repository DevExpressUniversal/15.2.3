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

using DevExpress.Design.UI;
using DevExpress.Utils;
using DevExpress.Utils.Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class BindingEditorControl : MvvmControlBase<BindingEditorControlMainViewModel, BindingEditorControlMainView>, IBindingEditorControl {
		#region Dependency Properties
		public static readonly DependencyProperty ResourcesProviderProperty =
			DependencyProperty.Register("ResourcesProvider", typeof(IBindingEditorControlResourcesProvider), typeof(BindingEditorControl), new PropertyMetadata(null,
				(d, e) => ((BindingEditorControl)d).OnResourcesProviderChanged(e)));
		public static readonly DependencyProperty RelativeSourceProviderProperty =
			DependencyProperty.Register("RelativeSourceProvider", typeof(IBindingEditorControlRelativeSourceProvider), typeof(BindingEditorControl), new PropertyMetadata(null,
				(d, e) => ((BindingEditorControl)d).OnRelativeSourceProviderChanged(e)));
		public static readonly DependencyProperty ElementsProviderProperty =
			DependencyProperty.Register("ElementsProvider", typeof(IBindingEditorControlElementsProvider), typeof(BindingEditorControl), new PropertyMetadata(null,
				(d, e) => ((BindingEditorControl)d).OnElementsProviderChanged(e)));
		public static readonly DependencyProperty DataContextProviderProperty =
			DependencyProperty.Register("DataContextProvider", typeof(IBindingEditorControlDataContextProvider), typeof(BindingEditorControl), new PropertyMetadata(null,
				(d, e) => ((BindingEditorControl)d).OnDataContextProviderChanged(e)));
		public static readonly DependencyProperty BindingSettingsProviderProperty =
			DependencyProperty.Register("BindingSettingsProvider", typeof(IBindingEditorControlBindingSettingsProvider), typeof(BindingEditorControl), new PropertyMetadata(null,
				(d, e) => ((BindingEditorControl)d).OnBindingSettingsProviderChanged(e)));
		public static readonly DependencyProperty BindingProperty =
			DependencyProperty.Register("Binding", typeof(BindingDescription), typeof(BindingEditorControl), new PropertyMetadata(null));
		public static readonly DependencyProperty CloseCommandProperty =
			DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(BindingEditorControl), new PropertyMetadata(null));
		public static readonly DependencyProperty CloseCommandParameterProperty =
			DependencyProperty.Register("CloseCommandParameter", typeof(object), typeof(BindingEditorControl), new PropertyMetadata(null));
		public static readonly DependencyProperty DefaultPageProperty =
			DependencyProperty.Register("DefaultPage", typeof(Func<BindingEditorControlPage>), typeof(BindingEditorControl), new PropertyMetadata(null,
				(d, e) => ((BindingEditorControl)d).OnDefaultPageChanged(e)));
		#endregion
		WeakEventHandler<ThePropertyChangedEventArgs<Func<BindingEditorControlPage>>, EventHandler<ThePropertyChangedEventArgs<Func<BindingEditorControlPage>>>> defaultPageChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<IBindingEditorControlResourcesProvider>, EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlResourcesProvider>>> resourcesProviderChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<IBindingEditorControlRelativeSourceProvider>, EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlRelativeSourceProvider>>> relativeSourceProviderChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<IBindingEditorControlElementsProvider>, EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlElementsProvider>>> elementsProviderChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<IBindingEditorControlDataContextProvider>, EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlDataContextProvider>>> dataContextProviderChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<IBindingEditorControlBindingSettingsProvider>, EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlBindingSettingsProvider>>> bindingSettingsProviderChanged;
		IBindingEditorControlResourcesProvider resourcesProvider;
		IBindingEditorControlRelativeSourceProvider relativeSourceProvider;
		IBindingEditorControlElementsProvider elementsProvider;
		IBindingEditorControlDataContextProvider dataContextProvider;
		IBindingEditorControlBindingSettingsProvider bindingSettingsProvider;
		public BindingEditorControl() {
			InitializeComponent();
		}
		public IBindingEditorControlResourcesProvider ResourcesProvider { get { return resourcesProvider; } set { SetValue(ResourcesProviderProperty, value); } }
		public IBindingEditorControlRelativeSourceProvider RelativeSourceProvider { get { return relativeSourceProvider; } set { SetValue(RelativeSourceProviderProperty, value); } }
		public IBindingEditorControlElementsProvider ElementsProvider { get { return elementsProvider; } set { SetValue(ElementsProviderProperty, value); } }
		public IBindingEditorControlDataContextProvider DataContextProvider { get { return dataContextProvider; } set { SetValue(DataContextProviderProperty, value); } }
		public IBindingEditorControlBindingSettingsProvider BindingSettingsProvider { get { return bindingSettingsProvider; } set { SetValue(BindingSettingsProviderProperty, value); } }
		public BindingDescription Binding { get { return (BindingDescription)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public ICommand CloseCommand { get { return (ICommand)GetValue(CloseCommandProperty); } set { SetValue(CloseCommandProperty, value); } }
		public object CloseCommandParameter { get { return GetValue(CloseCommandParameterProperty); } set { SetValue(CloseCommandParameterProperty, value); } }
		public Func<BindingEditorControlPage> DefaultPage { get { return (Func<BindingEditorControlPage>)GetValue(DefaultPageProperty); } set { SetValue(DefaultPageProperty, value); } }
		public event EventHandler<ThePropertyChangedEventArgs<Func<BindingEditorControlPage>>> DefaultPageChanged { add { defaultPageChanged += value; } remove { defaultPageChanged -= value; } }
		public event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlResourcesProvider>> ResourcesProviderChanged { add { resourcesProviderChanged += value; } remove { resourcesProviderChanged -= value; } }
		public event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlRelativeSourceProvider>> RelativeSourceProviderChanged { add { relativeSourceProviderChanged += value; } remove { relativeSourceProviderChanged -= value; } }
		public event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlElementsProvider>> ElementsProviderChanged { add { elementsProviderChanged += value; } remove { elementsProviderChanged -= value; } }
		public event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlDataContextProvider>> DataContextProviderChanged { add { dataContextProviderChanged += value; } remove { dataContextProviderChanged -= value; } }
		public event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlBindingSettingsProvider>> BindingSettingsProviderChanged { add { bindingSettingsProviderChanged += value; } remove { bindingSettingsProviderChanged -= value; } }
		protected override BindingEditorControlMainViewModel CreateMainViewModel() {
			return new BindingEditorControlMainViewModel(this);
		}
		void OnDefaultPageChanged(DependencyPropertyChangedEventArgs e) {
			defaultPageChanged.SafeRaise(this, new ThePropertyChangedEventArgs<Func<BindingEditorControlPage>>(e));
		}
		void OnResourcesProviderChanged(DependencyPropertyChangedEventArgs e) {
			resourcesProvider = (IBindingEditorControlResourcesProvider)e.NewValue;
			resourcesProviderChanged.SafeRaise(this, new ThePropertyChangedEventArgs<IBindingEditorControlResourcesProvider>(e));
		}
		void OnRelativeSourceProviderChanged(DependencyPropertyChangedEventArgs e) {
			relativeSourceProvider = (IBindingEditorControlRelativeSourceProvider)e.NewValue;
			relativeSourceProviderChanged.SafeRaise(this, new ThePropertyChangedEventArgs<IBindingEditorControlRelativeSourceProvider>(e));
		}
		void OnElementsProviderChanged(DependencyPropertyChangedEventArgs e) {
			elementsProvider = (IBindingEditorControlElementsProvider)e.NewValue;
			elementsProviderChanged.SafeRaise(this, new ThePropertyChangedEventArgs<IBindingEditorControlElementsProvider>(e));
		}
		void OnDataContextProviderChanged(DependencyPropertyChangedEventArgs e) {
			dataContextProvider = (IBindingEditorControlDataContextProvider)e.NewValue;
			dataContextProviderChanged.SafeRaise(this, new ThePropertyChangedEventArgs<IBindingEditorControlDataContextProvider>(e));
		}
		void OnBindingSettingsProviderChanged(DependencyPropertyChangedEventArgs e) {
			bindingSettingsProvider = (IBindingEditorControlBindingSettingsProvider)e.NewValue;
			bindingSettingsProviderChanged.SafeRaise(this, new ThePropertyChangedEventArgs<IBindingEditorControlBindingSettingsProvider>(e));
		}
	}
	public interface IBindingEditorControlResourcesProvider {
		IEnumerable<IBindingEditorControlResource> GetResources();
	}
	public interface IBindingEditorControlRelativeSourceProvider {
		IBindingEditorControlRelativeSourceSelf GetRelativeSourceSelf();
	}
	public interface IBindingEditorControlElementsProvider {
		IBindingEditorControlElement GetRootElement();
	}
	public interface IBindingEditorControlDataContextProvider {
		IBindingEditorControlDataContext GetDataContext();
	}
	public interface IBindingEditorControlBindingSource {
		IBindingEditorControlProperty RootProperty { get; }
	}
	public interface IBindingEditorControlDataContext : IBindingEditorControlBindingSource { }
	public interface IBindingEditorControlRelativeSourceSelf : IBindingEditorControlBindingSource { }
	public interface IBindingEditorControlProperty : IMvvmControlTreeNode<IBindingEditorControlProperty> {
		string PropertyName { get; }
		string PropertyType { get; }
		string ShortPropertyType { get; }
		bool IsClass { get; }
		bool IsPOCO { get; }
		bool Highlighted { get; }
	}
	public interface IBindingEditorControlElement : IMvvmControlTreeNode<IBindingEditorControlElement>, IBindingEditorControlBindingSource {
		string ElementName { get; }
		string ElementType { get; }
	}
	public interface IBindingEditorControlResource : IBindingEditorControlBindingSource {
		string Key { get; }
	}
	public interface IBindingEditorControlConverter {
		string Key { get; }
	}
	public interface IBindingEditorControlBindingSettingsProvider {
		IEnumerable<BindingSettingDescription<BindingMode>> GetModes(IBindingEditorControlBindingSource source, string path);
		IEnumerable<BindingSettingDescription<UpdateSourceTrigger>> GetUpdateSourceTriggers();
		IEnumerable<IBindingEditorControlConverter> GetConverters();
	}
	public class BindingSettingDescription<T> {
		public BindingSettingDescription(T value) : this(value, value) { }
		public BindingSettingDescription(T value, T actualValue) {
			Value = value;
			ActualValue = actualValue;
		}
		public T Value { get; private set; }
		public T ActualValue { get; private set; }
		public override string ToString() {
			return object.Equals(Value, ActualValue) ? Value.ToString() : string.Format("{0} ({1})", Value, ActualValue);
		}
	}
	public class BindingDescription : WpfBindableBase {
		IBindingEditorControlBindingSource source;
		string path;
		BindingSettingDescription<BindingMode> mode;
		bool updateSourceTriggerEnabled = true;
		BindingSettingDescription<UpdateSourceTrigger> updateSourceTrigger;
		IBindingEditorControlConverter converter;
		public IBindingEditorControlBindingSource Source {
			get { return source; }
			set { SetProperty(ref source, value, () => Source); }
		}
		public string Path {
			get { return path; }
			set { SetProperty(ref path, value, () => Path); }
		}
		public BindingSettingDescription<BindingMode> Mode {
			get { return mode; }
			set { SetProperty(ref mode, value, () => Mode, OnModeChanged); }
		}
		public bool UpdateSourceTriggerEnabled {
			get { return updateSourceTriggerEnabled; }
			private set { SetProperty(ref updateSourceTriggerEnabled, value, () => UpdateSourceTriggerEnabled); }
		}
		public BindingSettingDescription<UpdateSourceTrigger> UpdateSourceTrigger {
			get { return updateSourceTrigger; }
			set { SetProperty(ref updateSourceTrigger, value, () => UpdateSourceTrigger); }
		}
		public IBindingEditorControlConverter Converter {
			get { return converter; }
			set { SetProperty(ref converter, value, () => Converter); }
		}
		void OnModeChanged() {
			UpdateSourceTriggerEnabled = Mode.ActualValue == BindingMode.Default || Mode.ActualValue == BindingMode.TwoWay || Mode.ActualValue == BindingMode.OneWayToSource;
		}
	}
}
