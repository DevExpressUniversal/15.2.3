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
	class DataProcessingModeConfiguratorViewModel : BaseDataAccessConfiguratorPageViewModel, IDataProcessingModeConfiguratorViewModel {
		public DataProcessingModeConfiguratorViewModel(IViewModelBase parentViewModel, IDataAccessConfiguratorContext context)
			: base(parentViewModel, context) {
			ProcessingModes = InitModes(context);
			InitSelectedProcessingMode(context);
		}
		void InitSelectedProcessingMode(IDataAccessConfiguratorContext context) {
			if(context.ProcessingMode == null || !System.Linq.Enumerable.Contains(ProcessingModes, context.ProcessingMode)) {
				if(ProcessingModes != DataProcessingModesInfo.EmptyModes) {
					SelectedProcessingMode = System.Linq.Enumerable.FirstOrDefault(ProcessingModes);
				}
			}
			else SelectedProcessingMode = context.ProcessingMode;
		}
		IEnumerable<IDataProcessingMode> InitModes(IDataAccessConfiguratorContext context) {
			var configurationService = ServiceContainer.Resolve<IDataAccessConfigurationService>();
			return configurationService.InitProcessingModes(ServiceContainer, context);
		}
		#region Properties
		public IEnumerable<IDataProcessingMode> ProcessingModes {
			get;
			private set;
		}
		IDataProcessingMode selectedProcessingModeCore;
		public IDataProcessingMode SelectedProcessingMode {
			get { return selectedProcessingModeCore; }
			set { SetProperty(ref selectedProcessingModeCore, value, "SelectedProcessingMode", OnSelectedProcessingModeChanged); }
		}
		#endregion Properties
		void OnSelectedProcessingModeChanged() {
			UpdateIsCompleted();
		}
		protected override void OnLeave(IDataAccessConfiguratorContext context) {
			context.ProcessingMode = SelectedProcessingMode;
		}
		protected override void OnEnter(IDataAccessConfiguratorContext context) {
			ProcessingModes = InitModes(context);
			RaisePropertyChanged("ProcessingModes");
			InitSelectedProcessingMode(context);
		}
		protected override bool CalcIsCompleted(IDataAccessConfiguratorContext context) {
			return SelectedProcessingMode != null;
		}
	}
}
