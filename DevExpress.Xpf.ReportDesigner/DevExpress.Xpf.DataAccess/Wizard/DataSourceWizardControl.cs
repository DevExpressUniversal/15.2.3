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
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Data;
using System.Collections.ObjectModel;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class DataSourceWizardControl : DataSourceWizardControlBase {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ModelProperty;
		public static readonly DependencyProperty ExtensionsProperty;
		public static readonly DependencyProperty ParameterServiceProperty;
		internal static void RegisterMetadata() {
			MetadataHelper.AddMetadata<ParameterMetadata>();
		}
		static DataSourceWizardControl() {
			DataAccessAssemblyLoader.Load();
			DependencyPropertyRegistrator<DataSourceWizardControl>.New()
				.Register(d => d.Model, out ModelProperty, null)
				.Register(d => d.Extensions, out ExtensionsProperty, null)
				.Register(d => d.ParameterService, out ParameterServiceProperty, null)
			;
		}
		public DataSourceWizardControl() {
			this.SetDefaultStyleKey(typeof(DataSourceWizardControl));
		}
		public DataSourceWizardModelBase Model {
			get { return (DataSourceWizardModelBase)GetValue(ModelProperty); }
			private set { SetValue(ModelProperty, value); }
		}
		public IDataSourceWizardExtensions Extensions {
			get { return (IDataSourceWizardExtensions)GetValue(ExtensionsProperty); }
			set { SetValue(ExtensionsProperty, value); }
		}
		public IParameterService ParameterService {
			get { return (IParameterService)GetValue(ParameterServiceProperty); }
			set { SetValue(ParameterServiceProperty, value); }
		}
		protected virtual DataSourceWizardModelBase CreateModel(WizardController controller) {
			return DataSourceWizardModel.Create(new DataSourceWizardModelParameters(controller, DoWithMessageBoxService, DoWithSplashScreenService, DoWithOpenFileDialogService, DoWithQueryBuilderDialogService, DoWithPreviewDialogService, DoWithPasswordDialogService, DoWithChooseEFStoredProceduresDialogService), EnableCustomSql, Extensions, ParameterService);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			var wizard = (Wizard)GetTemplateChild(PART_Wizard);
			if (wizard == null)
				Model = null;
			else
				SetBinding(ModelProperty, new Binding() { Path = new PropertyPath(Wizard.ControllerProperty), Source = wizard, Mode = BindingMode.OneWay, Converter = new WizardControllerToWizardModelConverter(this) });
		}
		sealed class WizardControllerToWizardModelConverter : IValueConverter {
			readonly DataSourceWizardControl owner;
			public WizardControllerToWizardModelConverter(DataSourceWizardControl owner) {
				this.owner = owner;
			}
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
				return ((WizardController)value).With(x => owner.CreateModel(x));
			}
			object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
				throw new NotSupportedException();
			}
		}
	}
}
