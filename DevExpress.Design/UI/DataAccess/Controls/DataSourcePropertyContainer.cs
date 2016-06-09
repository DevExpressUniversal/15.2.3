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

namespace DevExpress.Design.DataAccess.UI {
	using System;
	using System.Windows;
	using DevExpress.Design.UI;
	public sealed class DataSourcePropertyContainer : System.Windows.Controls.ContentControl {
		public static readonly DependencyProperty SourceProperty;
		static DataSourcePropertyContainer() {
			var dProp = new DependencyPropertyRegistrator<DataSourcePropertyContainer>();
			dProp.Register("Source", ref SourceProperty, (IDataSourceProperty)null,
				(dObj, e) => ((DataSourcePropertyContainer)dObj).OnSourceChanged());
		}
		public DataSourcePropertyContainer() {
			Focusable = false;
		}
		public IDataSourceProperty Source {
			get { return (IDataSourceProperty)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		void OnSourceChanged() {
			ContentTemplate = DataSourcePropertyTemplateLoader.LoadTemplate(Source.GetCodeName());
		}
	}
	static class DataSourcePropertyTemplateLoader {
		static ResourceDictionary resources;
		static DataSourcePropertyTemplateLoader() {
			resources = new ResourceDictionary();
			string resourceUri = string.Format(
				"pack://application:,,,/DevExpress.Design.{0};component/UI/DataAccess/DataSourcePropertyTemplates.xaml",
				AssemblyInfo.VSuffixWithoutSeparator);
			resources.Source = new Uri(resourceUri, UriKind.Absolute);
		}
		public static DataTemplate LoadTemplate(DataSourcePropertyCodeName codeName) {
			return (DataTemplate)resources[GetTemplateName(codeName)];
		}
		static string GetTemplateName(DataSourcePropertyCodeName codeName) {
			return "DataSourceProperty" + codeName.ToString() + "Template";
		}
	}
}
namespace DevExpress.Design.UI {
	using System.Windows;
	public static class ListViewDoubleClickBehavior {
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			"Command", typeof(System.Windows.Input.ICommand), typeof(ListViewDoubleClickBehavior), new PropertyMetadata(null, OnCommandChanged));
		public static void SetCommand(DependencyObject dObj, System.Windows.Input.ICommand value) {
			dObj.SetValue(CommandProperty, value);
		}
		public static System.Windows.Input.ICommand GetCommand(DependencyObject dObj) {
			return (System.Windows.Input.ICommand)dObj.GetValue(CommandProperty);
		}
		static void OnCommandChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			var listBox = dObj as System.Windows.Controls.ListView;
			if(listBox != null) {
				if(e.NewValue is System.Windows.Input.ICommand)
					listBox.MouseDoubleClick += ListView_MouseDoubleClick;
				else
					listBox.MouseDoubleClick -= ListView_MouseDoubleClick;
			}
		}
		static void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs args) {
			var source = args.Source as System.Windows.Controls.ListBox;
			if(source == null) return;
			System.Windows.Input.ICommand command = GetCommand(source);
			if(command == null) return;
			FrameworkElement fe = args.OriginalSource as FrameworkElement;
			while(fe != null && fe != source) {
				var item = fe as System.Windows.Controls.ListViewItem;
				if(item != null) {
					command.Execute(item.Content);
					break;
				}
				fe = System.Windows.Media.VisualTreeHelper.GetParent(fe) as FrameworkElement;
			}
		}
	}
	public enum CtrlTabNavigation {
		Default, 
		Ignore
	}
	public static class TabControlBehavior {
		public static readonly DependencyProperty CtrlTabNavigationProperty = DependencyProperty.RegisterAttached(
			"CtrlTabNavigation", typeof(CtrlTabNavigation), typeof(TabControlBehavior), new PropertyMetadata(CtrlTabNavigation.Default, OnCtrlTabNavigationChanged));
		public static void SetCtrlTabNavigation(DependencyObject dObj, CtrlTabNavigation value) {
			dObj.SetValue(CtrlTabNavigationProperty, value);
		}
		public static CtrlTabNavigation GetCtrlTabNavigation(DependencyObject dObj) {
			return (CtrlTabNavigation)dObj.GetValue(CtrlTabNavigationProperty);
		}
		static void OnCtrlTabNavigationChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			var tabControl = dObj as System.Windows.Controls.TabControl;
			if(tabControl != null) {
				if(e.NewValue is CtrlTabNavigation)
					tabControl.PreviewKeyDown += tabControl_PreviewKeyDown;
				else
					tabControl.PreviewKeyDown -= tabControl_PreviewKeyDown;
			}
		}
		static void tabControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
			var source = e.Source as System.Windows.Controls.TabControl;
			if(source != null && e.Key == System.Windows.Input.Key.Tab) {
				bool isCtrlPressed = ((System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) != 0);
				if(isCtrlPressed)
					e.Handled = (GetCtrlTabNavigation(source) == CtrlTabNavigation.Ignore);
			}
		}
	}
}
