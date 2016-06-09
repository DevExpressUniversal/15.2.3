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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner {
	public abstract class ReportDesignerViewBase : Control<ReportDesignerViewBase>, IReportDesignerView {
		static readonly DependencyPropertyKey DesignerPropertyKey;
		public static readonly DependencyProperty DesignerProperty;
		public static readonly DependencyProperty TemplateRootStyleProperty;
		public static readonly DependencyProperty WindowServiceTemplateProperty;
		static readonly Action<ReportDesignerViewBase, Func<ReportDesignerViewBase, FrameworkElement>, Action<IWindowService>> windowServiceAccessor;
		static readonly DependencyPropertyKey ActiveDocumentViewSourcePropertyKey;
		public static readonly DependencyProperty ActiveDocumentViewSourceProperty;
		static readonly DependencyPropertyKey ActiveDocumentPropertyKey;
		public static readonly DependencyProperty ActiveDocumentProperty;
		public static readonly DependencyProperty ShowNewDocumentButtonProperty;
		public static readonly DependencyProperty ShowPreviewButtonProperty;
		static readonly DependencyPropertyKey RibbonPropertyKey;
		public static readonly DependencyProperty RibbonProperty;
		static ReportDesignerViewBase() {
			AssemblyInitializer.Init();
			DependencyPropertyRegistrator<ReportDesignerViewBase>.New()
				.RegisterServiceTemplateProperty(d => d.WindowServiceTemplate, out WindowServiceTemplateProperty, out windowServiceAccessor)
				.RegisterReadOnly(d => d.Designer, out DesignerPropertyKey, out DesignerProperty, null)
				.Register(d => d.TemplateRootStyle, out TemplateRootStyleProperty, null, (d, value) => d.CoerceTemplateRootStyle(value))
				.RegisterReadOnly(d => d.ActiveDocumentViewSource, out ActiveDocumentViewSourcePropertyKey, out ActiveDocumentViewSourceProperty, null, (d, e) => d.OnActiveDocumentViewSourceChanged(e))
				.RegisterReadOnly(d => d.ActiveDocument, out ActiveDocumentPropertyKey, out ActiveDocumentProperty, null, (d, e) => d.OnActiveDocumentChanged(e))
				.Register(d => d.ShowNewDocumentButton, out ShowNewDocumentButtonProperty, true)
				.Register(d => d.ShowPreviewButton, out ShowPreviewButtonProperty, true)
				.RegisterReadOnly(d => d.Ribbon, out RibbonPropertyKey, out RibbonProperty, null)
			;
		}
		public ReportDesignerViewBase() {
			activeDocumentViewSourceTracker = new ReportDesignerDocumentViewSourceDocumentTracker(x => ActiveDocument = x);
		}
		protected void DoWithWindowService(Func<ReportDesignerViewBase, FrameworkElement> getServiceOwner, Action<IWindowService> action) { windowServiceAccessor(this, getServiceOwner, action); }
		public DataTemplate WindowServiceTemplate {
			get { return (DataTemplate)GetValue(WindowServiceTemplateProperty); }
			set { SetValue(WindowServiceTemplateProperty, value); }
		}
		public ReportDesigner Designer {
			get { return (ReportDesigner)GetValue(DesignerProperty); }
			private set { SetValue(DesignerPropertyKey, value); }
		}
		void IReportDesignerView.AttachReportDesigner(ReportDesigner designer) {
			Designer = designer;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Style TemplateRootStyle {
			get { return (Style)GetValue(TemplateRootStyleProperty); }
			set { SetValue(TemplateRootStyleProperty, value); }
		}
		Style CoerceTemplateRootStyle(Style value) {
			if(value == null) return null;
			var designerPropertySetter = value.Setters.OfType<Setter>().Where(x => x.Property == ReportDesigner.DesignerProperty).FirstOrDefault();
			if(designerPropertySetter != null) return value;
			Style style = new Style(value.TargetType, value.BasedOn);
			foreach(var setter in value.Setters)
				style.Setters.Add(setter);
			style.Setters.Add(new Setter(ReportDesigner.DesignerProperty, new Binding { Path = new PropertyPath(DesignerProperty), Source = this, Mode = BindingMode.OneWay }));
			return style;
		}
		void IReportDesignerView.ShowWindow(FrameworkElement owner) {
			if(Designer == null)
				throw new InvalidOperationException();
			if(!IsInitialized) {
				BeginInit();
				EndInit();
			}
			DoWithWindowService(_ => owner, windowService => {
				windowService.Show(new ReportDesignerWindowDataContext(Designer));
			});
		}
		public ReportDesignerDocumentViewSource ActiveDocumentViewSource {
			get { return (ReportDesignerDocumentViewSource)GetValue(ActiveDocumentViewSourceProperty); }
			protected set { SetValue(ActiveDocumentViewSourcePropertyKey, value); }
		}
		public ReportDesignerDocument ActiveDocument {
			get { return (ReportDesignerDocument)GetValue(ActiveDocumentProperty); }
			protected set { SetValue(ActiveDocumentPropertyKey, value); }
		}
		public object Ribbon {
			get { return GetValue(RibbonProperty); }
			protected set { SetValue(RibbonPropertyKey, value); }
		}
		readonly ReportDesignerDocumentViewSourceDocumentTracker activeDocumentViewSourceTracker;
		protected virtual void OnActiveDocumentViewSourceChanged(DependencyPropertyChangedEventArgs e) {
			activeDocumentViewSourceTracker.Update(e);
			if(ActiveDocumentViewSourceChanged != null)
				ActiveDocumentViewSourceChanged(this, e);
		}
		protected virtual void OnActiveDocumentChanged(DependencyPropertyChangedEventArgs e) {
			if(ActiveDocumentChanged != null)
				ActiveDocumentChanged(this, e);
		}
		public event DependencyPropertyChangedEventHandler ActiveDocumentViewSourceChanged;
		public event DependencyPropertyChangedEventHandler ActiveDocumentChanged;
		ReportDesignerDocumentViewSource IReportDesignerView.OpenDocument(ReportDesignerDocument document, Action<ReportDesignerDocument> onDocumentOpened) {
			return OpenDocument(document, onDocumentOpened);
		}
		protected abstract ReportDesignerDocumentViewSource OpenDocument(ReportDesignerDocument document, Action<ReportDesignerDocument> onDocumentOpened);
		void IReportDesignerView.DestroyDocument(ReportDesignerDocument document) {
			DestroyDocument(document);
		}
		protected abstract void DestroyDocument(ReportDesignerDocument document);
		public bool ShowNewDocumentButton {
			get { return (bool)GetValue(ShowNewDocumentButtonProperty); }
			set { SetValue(ShowNewDocumentButtonProperty, value); }
		}
		public bool ShowPreviewButton {
			get { return (bool)GetValue(ShowPreviewButtonProperty); }
			set { SetValue(ShowPreviewButtonProperty, value); }
		}
		void IReportDesignerView.ActivateDocument(ReportDesignerDocument document) {
			ActivateDocument(document);
		}
		protected abstract void ActivateDocument(ReportDesignerDocument document);
	}
	public class ReportDesignerWindowStyleExtension : MarkupExtension {
		static Style style;
		public static Style Style {
			get {
				if(style == null) {
					style = new Style(typeof(DXWindow));
					style.Setters.Add(new Setter(DXWindow.UseLayoutRoundingProperty, true));
					style.Setters.Add(new Setter(DXWindow.WidthProperty, 1280.0));
					style.Setters.Add(new Setter(DXWindow.HeightProperty, 860.0));
					style.Setters.Add(new Setter(Interaction.BehaviorsTemplateProperty, XamlTemplateHelper.CreateDataTemplate(typeof(ReportDesignerWindowBehaviorsTemplate))));
				}
				return style;
			}
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Style;
		}
	}
	public class ReportDesignerWindowViewTemplateExtension : MarkupExtension {
		static DataTemplate template;
		public static DataTemplate Template {
			get {
				if(template == null)
					template = XamlTemplateHelper.CreateDataTemplate(typeof(ReportDesignerWindowViewTemplate));
				return template;
			}
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Template;
		}
	}
}
