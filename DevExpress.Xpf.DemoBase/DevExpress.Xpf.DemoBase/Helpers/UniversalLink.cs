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
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.DemoData.Helpers;
using DevExpress.DemoData.Utils;
#if DEMOCENTER
using DevExpress.Internal.DXWindow;
#else
using DevExpress.Xpf.Core;
#endif
#if DEMOCENTER
namespace DevExpress.DemoCenter.Xpf.Helpers {
#else
namespace DevExpress.Xpf.DemoBase.Helpers {
#endif
	[ContentProperty("Content")]
	public class UniversalLinkBase : Control {
		#region Dependency Properties
		public static readonly DependencyProperty UriStringProperty;
		public static readonly DependencyProperty ContentProperty;
		static UniversalLinkBase() {
			Type ownerType = typeof(UniversalLinkBase);
			UriStringProperty = DependencyProperty.Register("UriString", typeof(string), ownerType, new PropertyMetadata(null, (d, e) => ((UniversalLinkBase)d).RaiseUriStringChanged(e)));
			ContentProperty = DependencyProperty.Register("Content", typeof(object), ownerType, new PropertyMetadata(null));
		}
		#endregion
		const string ResourceDictionaryXaml = @"
            <ResourceDictionary
                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                xmlns:local='clr-namespace:{0};assembly={1}'>
                
            {2}
            {3}
            </ResourceDictionary>";
		const string FocusableSetterXaml = @"
            <Setter Property='Focusable' Value='False' />";
		const string EmptyButtonXaml = @"
            <Style x:Key='EmptyButton' TargetType='Button'>
                <Setter Property='Template'>
                    <Setter.Value>
                        <ControlTemplate TargetType='Button'>
                            <Grid Background='#00FF0000' Cursor='Hand'>
                                <VisualStateManager.VisualStateGroups>
                                    <VisualStateGroup x:Name='CommonStates'>
                                        <VisualState x:Name='Disabled' />
                                        <VisualState x:Name='Normal' />
                                        <VisualState x:Name='MouseOver' />
                                        <VisualState x:Name='Pressed' />
                                    </VisualStateGroup>
                                </VisualStateManager.VisualStateGroups>
                                <ContentPresenter />
                            </Grid>
                        </ControlTemplate>
                    </Setter.Value>
                </Setter>
                <Setter Property='IsTabStop' Value='False' />
                {0}
            </Style>";
		const string TemplateXaml = @"
            <ControlTemplate x:Key='DefaultUniversalLinkBaseTemplate' TargetType='local:UniversalLinkBase'>
                <TextBlock Focusable='False'>
                    <Hyperlink x:Name='HyperLink' TextDecorations='{x:Null}' Focusable='False'>
                        <ContentPresenter Content='{TemplateBinding Content}' />
                    </Hyperlink>
                </TextBlock>
            </ControlTemplate>";
		static ControlTemplate defaultTemplate;
		DependencyObject activeElement;
		string target = "_blank";
		public UniversalLinkBase() {
			DefaultStyleKey = typeof(UniversalLinkBase);
			if(Template == null)
				Template = DefaultTemplate;
		}
		public static ControlTemplate DefaultTemplate {
			get {
				if(defaultTemplate == null) {
					Type ownerType = typeof(UniversalLinkBase);
					string assemblyName = AssemblyHelper.GetPartialName(ownerType.Assembly);
					string namespaceName = AssemblyHelper.GetNamespace(ownerType);
					string defaultTemplateXaml = string.Format(ResourceDictionaryXaml, namespaceName, assemblyName, string.Format(EmptyButtonXaml, FocusableSetterXaml), TemplateXaml);
					ResourceDictionary dictionary = (ResourceDictionary)XamlReaderHelper.Parse(defaultTemplateXaml);
					defaultTemplate = (ControlTemplate)dictionary["DefaultUniversalLinkBaseTemplate"];
				}
				return defaultTemplate;
			}
		}
		public string UriString { get { return (string)GetValue(UriStringProperty); } set { SetValue(UriStringProperty, value); } }
		public object Content { get { return (ImageSource)GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
		public string Target {
			get { return target; }
			set {
				target = value;
				BindActions(activeElement);
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			activeElement = GetTemplateChild("HyperLink");
			if(!string.IsNullOrEmpty(UriString))
				BindActions(activeElement);
		}
		public virtual void OnLinkClick(object sender, EventArgs e) { }
		protected virtual void BindActions(DependencyObject activeElement) { }
		protected void SubscribeToClick(DependencyObject target) {
			if(target == null) return;
			EventInfo eventInfo = target.GetType().GetEvent("Click");
			MethodInfo methodInfo = GetType().GetMethod("OnLinkClick", new Type[] { typeof(object), typeof(EventArgs) });
			if(methodInfo == null) return;
			Delegate delegateObject = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
			if(eventInfo != null && delegateObject != null) {
				eventInfo.RemoveEventHandler(target, delegateObject);
				eventInfo.AddEventHandler(target, delegateObject);
			}
		}
		protected void PrepareHyperLink(DependencyObject activeElement) {
			try {
				Hyperlink hyperLink = activeElement as Hyperlink;
				if(hyperLink == null) return;
				hyperLink.TargetName = Target;
				hyperLink.NavigateUri = new Uri(UriString);
			} catch { }
		}
		void RaiseUriStringChanged(DependencyPropertyChangedEventArgs e) {
			BindActions(activeElement);
		}
	}
	public class UniversalLink : UniversalLinkBase {
		protected override void BindActions(DependencyObject activeElement) {
			if(BrowserInteropHelper.IsBrowserHosted) {
				PrepareHyperLink(activeElement);
			} else {
				SubscribeToClick(activeElement);
			}
		}
		public override void OnLinkClick(object sender, EventArgs e) {
			DocumentPresenter.OpenLink(UriString, Target);
		}
	}
}
