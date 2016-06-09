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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Data.WizardFramework;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public abstract class DataSourceWizardControlBase : Control {
		public const string PART_Wizard = "Wizard";
		public static readonly DependencyProperty PageTemplateSelectorProperty;
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		static readonly Action<DataSourceWizardControlBase, Action<IMessageBoxService>> messageBoxServiceAccessor;
		public static readonly DependencyProperty SplashScreenServiceTemplateProperty;
		static readonly Action<DataSourceWizardControlBase, Action<ISplashScreenService>> splashScreenServiceAccessor;
		public static readonly DependencyProperty OpenFileDialogServiceTemplateProperty;
		static readonly Action<DataSourceWizardControlBase, Action<IOpenFileDialogService>> openFileDialogServiceAccessor;
		public static readonly DependencyProperty QueryBuilderDialogServiceTemplateProperty;
		static readonly Action<DataSourceWizardControlBase, Action<IDialogService>> queryBuilderDialogServiceAccessor;
		public static readonly DependencyProperty PreviewDialogServiceTemplateProperty;
		static readonly Action<DataSourceWizardControlBase, Action<IDialogService>> previewDialogServiceAccessor;
		public static readonly DependencyProperty PasswordDialogServiceTemplateProperty;
		static readonly Action<DataSourceWizardControlBase, Action<IDialogService>> passwordDialogServiceAccessor;
		public static readonly DependencyProperty ChooseEFStoredProceduresDialogServiceTemplateProperty;
		static readonly Action<DataSourceWizardControlBase, Action<IDialogService>> chooseEFStoredProceduresDialogServiceAccessor;
		public static readonly DependencyProperty EnableCustomSqlProperty;
		static DataSourceWizardControlBase() {
			DependencyPropertyRegistrator<DataSourceWizardControlBase>.New()
				.Register(x => x.PageTemplateSelector, out PageTemplateSelectorProperty, new DataSourceWizardPageTemplateSelector())
				.Register(x => x.EnableCustomSql, out EnableCustomSqlProperty, false)
				.RegisterServiceTemplateProperty(d => d.MessageBoxServiceTemplate, out MessageBoxServiceTemplateProperty, out messageBoxServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.SplashScreenServiceTemplate, out SplashScreenServiceTemplateProperty, out splashScreenServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.OpenFileDialogServiceTemplate, out OpenFileDialogServiceTemplateProperty, out openFileDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.QueryBuilderDialogServiceTemplate, out QueryBuilderDialogServiceTemplateProperty, out queryBuilderDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.PreviewDialogServiceTemplate, out PreviewDialogServiceTemplateProperty, out previewDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.PasswordDialogServiceTemplate, out PasswordDialogServiceTemplateProperty, out passwordDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.ChooseEFStoredProceduresDialogServiceTemplate, out ChooseEFStoredProceduresDialogServiceTemplateProperty, out chooseEFStoredProceduresDialogServiceAccessor)
			;
		}
		public DataTemplateSelector PageTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PageTemplateSelectorProperty); }
			set { SetValue(PageTemplateSelectorProperty, value); }
		}
		public bool EnableCustomSql {
			get { return (bool)GetValue(EnableCustomSqlProperty); }
			set { SetValue(EnableCustomSqlProperty, value); }
		}
		protected void DoWithMessageBoxService(Action<IMessageBoxService> action) { messageBoxServiceAccessor(this, action); }
		public DataTemplate MessageBoxServiceTemplate {
			get { return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty); }
			set { SetValue(MessageBoxServiceTemplateProperty, value); }
		}
		protected void DoWithSplashScreenService(Action<ISplashScreenService> action) { splashScreenServiceAccessor(this, action); }
		public DataTemplate SplashScreenServiceTemplate {
			get { return (DataTemplate)GetValue(SplashScreenServiceTemplateProperty); }
			set { SetValue(SplashScreenServiceTemplateProperty, value); }
		}
		protected void DoWithOpenFileDialogService(Action<IOpenFileDialogService> action) { openFileDialogServiceAccessor(this, action); }
		public DataTemplate OpenFileDialogServiceTemplate {
			get { return (DataTemplate)GetValue(OpenFileDialogServiceTemplateProperty); }
			set { SetValue(OpenFileDialogServiceTemplateProperty, value); }
		}
		protected void DoWithQueryBuilderDialogService(Action<IDialogService> action) { queryBuilderDialogServiceAccessor(this, action); }
		public DataTemplate QueryBuilderDialogServiceTemplate {
			get { return (DataTemplate)GetValue(QueryBuilderDialogServiceTemplateProperty); }
			set { SetValue(QueryBuilderDialogServiceTemplateProperty, value); }
		}
		protected void DoWithPreviewDialogService(Action<IDialogService> action) { previewDialogServiceAccessor(this, action); }
		public DataTemplate PreviewDialogServiceTemplate {
			get { return (DataTemplate)GetValue(PreviewDialogServiceTemplateProperty); }
			set { SetValue(PreviewDialogServiceTemplateProperty, value); }
		}
		protected void DoWithPasswordDialogService(Action<IDialogService> action) { passwordDialogServiceAccessor(this, action); }
		public DataTemplate PasswordDialogServiceTemplate {
			get { return (DataTemplate)GetValue(PasswordDialogServiceTemplateProperty); }
			set { SetValue(PasswordDialogServiceTemplateProperty, value); }
		}
		protected void DoWithChooseEFStoredProceduresDialogService(Action<IDialogService> action) { chooseEFStoredProceduresDialogServiceAccessor(this, action); }
		public DataTemplate ChooseEFStoredProceduresDialogServiceTemplate {
			get { return (DataTemplate)GetValue(ChooseEFStoredProceduresDialogServiceTemplateProperty); }
			set { SetValue(ChooseEFStoredProceduresDialogServiceTemplateProperty, value); }
		}
	}
}
