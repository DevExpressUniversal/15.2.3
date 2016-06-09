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
	using DevExpress.Design.UI;
	abstract class NewDataSourceMessageWindowViewModelBase : MessageWindowViewModel {
		string title;
		protected NewDataSourceMessageWindowViewModelBase(IDXDesignWindowViewModel designWindowViewModel, DataAccessLocalizerStringId titleId)
			: base(designWindowViewModel) {
				title = (designWindowViewModel != null) ? designWindowViewModel.Title : GetString(titleId);
		}
		public override string Title {
			get { return title; }
		}
		public override string ButtonText { 
			get { return GetString(DataAccessLocalizerStringId.NewDataSourceButtonText); } 
		}
		public override bool? OwnerWindowResult {
			get { return false; }
		}
		protected static string GetString(DataAccessLocalizerStringId titleId) {
			return DataAccessLocalizer.GetString(titleId);
		}
	}
	class NewDataSourceMessageWindowViewModel : NewDataSourceMessageWindowViewModelBase {
		public NewDataSourceMessageWindowViewModel(IDXDesignWindowViewModel designWindowViewModel)
			: base(designWindowViewModel, DataAccessLocalizerStringId.NewDataSourceTitle) {
		}
		public override string Message {
			get { return string.Format(GetString(DataAccessLocalizerStringId.NewDataSourceMessageFormat), Title); }
		}
	}
	class NewSQLDataSourceMessageWindowViewModel : NewDataSourceMessageWindowViewModelBase {
		public NewSQLDataSourceMessageWindowViewModel(IDXDesignWindowViewModel designWindowViewModel)
			: base(designWindowViewModel, DataAccessLocalizerStringId.NewDataSourceTitleSQLDataSource) {
		}
		public override string Message {
			get { return GetString(DataAccessLocalizerStringId.NewDataSourceMessageFormatSQLDataSource); }
		}
	}
	class NewExcelDataSourceMessageWindowViewModel : NewDataSourceMessageWindowViewModelBase {
		public NewExcelDataSourceMessageWindowViewModel(IDXDesignWindowViewModel designWindowViewModel)
			: base(designWindowViewModel, DataAccessLocalizerStringId.NewDataSourceTitleExcelDataSource) {
		}
		public override string Message {
			get { return GetString(DataAccessLocalizerStringId.NewDataSourceMessageFormatExcelDataSource); }
		}
	}
}
