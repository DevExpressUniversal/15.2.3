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
	using System.Collections.Generic;
	using DevExpress.Design.UI;
	class DataSourceSettingsConfiguratorViewModel : BaseDataAccessConfiguratorPageViewModel, IDataSourceSettingsConfiguratorViewModel {
		IDataSourceSettings settingsCore;
		public DataSourceSettingsConfiguratorViewModel(IViewModelBase parentViewModel, IDataAccessConfiguratorContext context)
			: base(parentViewModel, context) {
			this.settingsCore = InitSettings(context);
		}
		IDataSourceSettings InitSettings(IDataAccessConfiguratorContext context) {
			var configurationService = ServiceContainer.Resolve<IDataAccessConfigurationService>();
			return configurationService.InitSettings(ServiceContainer, context);
		}
		#region Properties
		public IEnumerable<IDataSourceProperty> PropertiesSource {
			get { return settingsCore; }
		}
		#endregion Properties
		protected override void OnLeave(IDataAccessConfiguratorContext context) {
			context.SettingsModel = null;
		}
		protected override void OnEnter(IDataAccessConfiguratorContext context) {
			this.settingsCore = InitSettings(context);
			if(context.SettingsModel != null)
				context.SettingsModel.Enter();
			RaisePropertyChanged("PropertiesSource");
		}
		protected override bool CalcIsCompleted(IDataAccessConfiguratorContext context) {
			return (context.SettingsModel != null) && string.IsNullOrEmpty(context.SettingsModel.Error);
		}
	}
}
