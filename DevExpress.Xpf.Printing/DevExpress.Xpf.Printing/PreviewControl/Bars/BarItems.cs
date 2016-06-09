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
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Printing.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Data;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Printing.PreviewControl.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing.PreviewControl.Bars {
	public class PreviewBarButtonItem : DocumentViewerBarButtonItem { }
	public abstract class PreviewBarCheckItem : BarCheckItem {
		public PreviewBarCheckItem() {
			DefaultStyleKey = typeof(PreviewBarCheckItem);
		}
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			var command = newCommand as DevExpress.Xpf.DocumentViewer.CommandBase;
			if(command == null) {
				ClearValue(StyleProperty);
				return;
			}
			DataContext = command;
		}
	}
	public class SaveBarItem : DocumentViewerBarButtonItem {
		static SaveBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(SaveBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.S, ModifierKeys.Control)));
		}
	}
	public class ParametersBarCheckItem : PreviewBarCheckItem { }
	public class DocumentMapBarCheckItem : PreviewBarCheckItem { }
	public class PrintBarItem : DocumentViewerBarButtonItem {
		static PrintBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(PrintBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.P, ModifierKeys.Control)));
		}
	}
	public class PrintDirectBarItem : DocumentViewerBarButtonItem { }
	public class PageSetupBarItem : DocumentViewerBarButtonItem { }
	public class ScaleBarItem : DocumentViewerBarButtonItem { }
	public class FirstPageBarItem : DocumentViewerBarButtonItem { }
	public class LastPageBarItem : DocumentViewerBarButtonItem { }
	public class FindTextBarItem : DocumentViewerBarButtonItem {
		static FindTextBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(FindTextBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.F, ModifierKeys.Control)));
		}
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
		}
	}
	public class ExportSplitItem : BarSplitButtonItem {
		PopupMenu PopupMenu { get { return PopupControl as PopupMenu; } }
		public ExportSplitItem() {
			DefaultStyleKey = typeof(ExportSplitItem);
			PopupControl = CreatePopup();
		}
		protected virtual IPopupControl CreatePopup() {
			return new PopupMenu() { Tag = "Export" };
		}
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			BindingOperations.ClearBinding(PopupMenu, PopupMenu.ItemLinksSourceProperty);
			PopupMenu.ClearValue(PopupMenu.ItemTemplateProperty);
			if(newCommand == null) {
				ClearValue(StyleProperty);
				return;
			}
			DataContext = newCommand;
			Binding formats = new Binding("Commands");
			PopupMenu.SetBinding(PopupMenu.ItemLinksSourceProperty, formats);
			PopupMenu.ItemTemplate = FindResource(new NewDocumentViewerThemeKeyExtension() { ResourceKey = NewDocumentViewerThemeKeys.PopupMenuItemTemplate }) as DataTemplate;
		}
	}
	public class SendSplitItem : ExportSplitItem {
		protected override IPopupControl CreatePopup() {
			return new PopupMenu() { Tag = "Send" };
		}
	}
	public class WatermarkBarItem : DocumentViewerBarButtonItem { }
	public class CopyBarItem : DocumentViewerBarButtonItem {
		static CopyBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(CopyBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.C, ModifierKeys.Control)));
		}
	}
	public class SelectToolBarItem : PreviewBarCheckItem {
		public SelectToolBarItem() {
			DefaultStyleKey = typeof(SelectToolBarItem);
		}
	}
	public class HandToolBarItem : PreviewBarCheckItem {
		public HandToolBarItem() {
			DefaultStyleKey = typeof(HandToolBarItem);
		}
	}
	public class PageNumberEditItem : BarEditItem {
		public static readonly DependencyProperty SettingsSourceProperty;
		PageNumberBehavior behavior;
		static PageNumberEditItem() {
			SettingsSourceProperty = DependencyProperty.Register("SettingsSource", typeof(DocumentViewerControl), typeof(PageNumberEditItem), new FrameworkPropertyMetadata(null, (d, e) => ((PageNumberEditItem)d).OnSourceChanged()));
		}
		public PageNumberEditItem() {
			DefaultStyleKey = typeof(PageNumberEditItem);
			EditSettings = new TextEditSettings() {
				HorizontalContentAlignment = EditSettingsHorizontalAlignment.Center,
				MaskType = Editors.MaskType.RegEx,
				Mask = @"\d{0,6}",
				AllowNullInput = true
			};
			behavior = new PageNumberBehavior();
			Interaction.GetBehaviors(this).Add(behavior);			
		}
		public DocumentViewerControl SettingsSource {
			get { return GetValue(SettingsSourceProperty) as DocumentViewerControl; }
			set { SetValue(SettingsSourceProperty, value); }
		}
		void OnSourceChanged() {
			DataContext = SettingsSource;
			behavior.FocusTarget = SettingsSource;
		}
	}
	public class ExportFormatToItemNameConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(values.Length != 2)
				throw new NotSupportedException();
			return string.Format("b{0}{1}", values[0], values[1]);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ProgressEditItem : BarEditItem {
		public IProgressSettings ProgressSettings {
			get { return (IProgressSettings)GetValue(ProgressSettingsProperty); }
			set { SetValue(ProgressSettingsProperty, value); }
		}
		public static readonly DependencyProperty ProgressSettingsProperty;
		static ProgressEditItem() {
			ProgressSettingsProperty = DependencyPropertyRegistrator.Register<ProgressEditItem, IProgressSettings>(x => x.ProgressSettings, null, (d, o, n) => ((ProgressEditItem)d).OnProgressSettingsChanged(n));
		}
		public ProgressEditItem() {
			DefaultStyleKey = typeof(ProgressEditItem);
			this.EditSettings = new ProgressBarEditSettings();
		}
		void OnProgressSettingsChanged(IProgressSettings settings) {
			BindingOperations.ClearBinding(EditSettings, BaseEditSettings.StyleSettingsProperty);
			DataContext = settings;
			var binding = new Binding("ProgressType") { Converter = new ProgressTypeToEditSettingsConverter(), FallbackValue = new ProgressBarStyleSettings() };
			BindingOperations.SetBinding(EditSettings, BaseEditSettings.StyleSettingsProperty, binding);
		}
	}
	public class StopPageBuildItem : DocumentViewerBarButtonItem {
		public StopPageBuildItem() {
			DefaultStyleKey = typeof(StopPageBuildItem);
		}
	}
}
