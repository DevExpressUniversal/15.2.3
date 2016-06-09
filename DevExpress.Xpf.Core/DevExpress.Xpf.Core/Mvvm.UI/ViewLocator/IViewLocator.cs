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

#if !NETFX_CORE
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif
using System;
using System.Windows;
using System.ComponentModel;
namespace DevExpress.Mvvm.UI {
	public interface IViewLocator {
		object ResolveView(string name);
		Type ResolveViewType(string name);
	}
	public static class ViewLocatorExtensions {
#if !NETFX_CORE && !SILVERLIGHT
		public static DataTemplate CreateViewTemplate(this IViewLocator viewLocator, Type viewType) {
			Verify(viewLocator);
			if(viewType == null) throw new ArgumentNullException("viewType");
			DataTemplate res = null;
			if(!typeof(FrameworkElement).IsAssignableFrom(viewType) && !typeof(FrameworkContentElement).IsAssignableFrom(viewType))
				res = CreateFallbackViewTemplate(GetErrorMessage_CannotCreateDataTemplateFromViewType(viewType.Name));
			else {
				res = CreateViewTemplateCore(viewType);
				res.Seal();
			}
			return res;
		}
		static DataTemplate CreateViewTemplateCore(Type viewType) {
			var xaml = String.Format("<DataTemplate><v:{0} /></DataTemplate>", viewType.Name);
			var context = new System.Windows.Markup.ParserContext();
			context.XamlTypeMapper = new System.Windows.Markup.XamlTypeMapper(new string[0]);
			context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);
			context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
			context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
			context.XmlnsDictionary.Add("v", "v");
			return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml, context);
		}
		public static DataTemplate CreateViewTemplate(this IViewLocator viewLocator, string viewName) {
			Verify(viewLocator);
			Type viewType = viewLocator.ResolveViewType(viewName);
			if(viewType == null) return CreateFallbackViewTemplate(GetErrorMessage_CannotResolveViewType(viewName));
			return CreateViewTemplate(viewLocator, viewType);
		}
		internal static DataTemplate CreateFallbackViewTemplate(string errorText) {
			var factory = new FrameworkElementFactory(typeof(FallbackView));
			factory.SetValue(FallbackView.TextProperty, errorText);
			var res = new DataTemplate() { VisualTree = factory };
			res.Seal();
			return res;
		}
#endif
		internal static object CreateFallbackView(string errorText) {
			return new FallbackView() { Text = errorText };
		}
		internal static string GetErrorMessage_CannotResolveViewType(string name) {
			if(string.IsNullOrEmpty(name))
				return "ViewType is not specified.";
			if(ViewModelBase.IsInDesignMode)
				return string.Format("[{0}]", name);
			return string.Format("\"{0}\" type not found.", name);
		}
		internal static string GetErrorMessage_CannotCreateDataTemplateFromViewType(string name) {
			if(ViewModelBase.IsInDesignMode)
				return string.Format("[{0}]", name);
			return string.Format("Cannot create DataTemplate from the \"{0}\" view type.", name);
		}
		static void Verify(IViewLocator viewLocator) {
			if(viewLocator == null)
				throw new ArgumentNullException("viewLocator");
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public class FallbackView : Panel {
			public static readonly DependencyProperty TextProperty =
				DependencyProperty.Register("Text", typeof(string), typeof(FallbackView), 
				new PropertyMetadata(null, (d,e) => ((FallbackView)d).OnTextChanged()));
			public string Text {
				get { return (string)GetValue(TextProperty); }
				set { SetValue(TextProperty, value); }
			}
			TextBlock tb;
			public FallbackView() {
				tb = new TextBlock();
				if(ViewModelBase.IsInDesignMode) {
					tb.FontSize = 25;
					tb.Foreground = new SolidColorBrush(Colors.Red);
				} else {
					tb.FontSize = 18;
					tb.Foreground = new SolidColorBrush(Colors.Gray);
				}
				HorizontalAlignment = HorizontalAlignment.Center;
				VerticalAlignment = VerticalAlignment.Center;
				Children.Add(tb);
			}
			void OnTextChanged() {
				tb.Text = Text;
			}
			protected override Size MeasureOverride(Size availableSize) {
				tb.Measure(availableSize);
				return tb.DesiredSize;
			}
			protected override Size ArrangeOverride(Size finalSize) {
				tb.Arrange(new Rect(new Point(), finalSize));
				return finalSize;
			}
		}
	}
}
