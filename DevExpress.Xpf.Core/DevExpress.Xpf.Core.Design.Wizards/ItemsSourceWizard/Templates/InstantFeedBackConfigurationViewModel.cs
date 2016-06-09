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
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates {
	public class ServerModeConfigurationViewModelBase : ConfigurationViewModelBase { 
				#region static
		public static readonly DependencyProperty SelectedSortFieldNameProperty;
		public static readonly DependencyProperty SelectedSortDirectionProperty;
		public static readonly DependencyProperty IsDefaultSortingSelectedProperty;
		public static readonly DependencyProperty IsAscDirectionProperty;
		static ServerModeConfigurationViewModelBase() {
			Type ownerType = typeof(ServerModeConfigurationViewModelBase);
			SelectedSortFieldNameProperty = DependencyProperty.Register("SelectedSortFieldName", typeof(string), ownerType,
				new FrameworkPropertyMetadata((o, e) => ((ServerModeConfigurationViewModelBase)o).OnSelectedSortFieldNameChanged()));
			SelectedSortDirectionProperty = DependencyProperty.Register("SelectedSortDirection", typeof(string), ownerType, new FrameworkPropertyMetadata());
			IsDefaultSortingSelectedProperty = DependencyProperty.Register("IsDefaultSortingSelected", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsAscDirectionProperty = DependencyProperty.Register("IsAscDirection", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		#endregion
		public ServerModeConfigurationViewModelBase(List<DataTable> tables)
			: base(tables) { }
		public string SelectedSortFieldName {
			get { return (string)GetValue(SelectedSortFieldNameProperty); }
			set { SetValue(SelectedSortFieldNameProperty, value); }
		}
		public string SelectedSortDirection {
			get { return (string)GetValue(SelectedSortDirectionProperty); }
			set { SetValue(SelectedSortDirectionProperty, value); }
		}
		public bool IsDefaultSortingSelected {
			get { return (bool)GetValue(IsDefaultSortingSelectedProperty); }
			set { SetValue(IsDefaultSortingSelectedProperty, value); }
		}
		public bool IsAscDirection {
			get { return (bool)GetValue(IsAscDirectionProperty); }
			set { SetValue(IsAscDirectionProperty, value); }
		}
		public string DefaultSorting {
			get {
				string direction = IsAscDirection ? "ASC" : "DESC";
				return !string.IsNullOrEmpty(SelectedSortFieldName) ? String.Format("{0} {1}", SelectedSortFieldName, direction) : null;
			}
		}
		public IEnumerable<string> Directions { get { return new List<string>() { "ASC", "DESC" }; } }
		private void OnSelectedSortFieldNameChanged() {
			IsDefaultSortingSelected = true;
		}
	}
	public class InstantFeedBackConfigurationViewModel : ServerModeConfigurationViewModel {
		#region static
		public static readonly DependencyProperty AreSourceRowsThreadSafeProperty;
		static InstantFeedBackConfigurationViewModel() {
			Type ownerType = typeof(InstantFeedBackConfigurationViewModel);
			AreSourceRowsThreadSafeProperty = DependencyProperty.Register("AreSourceRowsThreadSafe", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
		}
		#endregion
		public InstantFeedBackConfigurationViewModel(List<DataTable> tables) : base(tables) { }
		public bool AreSourceRowsThreadSafe {
			get { return (bool)GetValue(AreSourceRowsThreadSafeProperty); }
			set { SetValue(AreSourceRowsThreadSafeProperty, value); }
		}
	}
	public class ServerModeConfigurationViewModel : ServerModeConfigurationViewModelBase {
		#region static
		public static readonly DependencyProperty SelectedKeyExpressionProperty;
		static ServerModeConfigurationViewModel() {
			Type ownerType = typeof(ServerModeConfigurationViewModel);
			SelectedKeyExpressionProperty = DependencyProperty.Register("SelectedKeyExpression", typeof(string), ownerType, new FrameworkPropertyMetadata());
		}
		#endregion
		public ServerModeConfigurationViewModel(List<DataTable> tables)
			: base(tables) { }
		public string SelectedKeyExpression {
			get { return (string)GetValue(SelectedKeyExpressionProperty); }
			set { SetValue(SelectedKeyExpressionProperty, value); }
		}
		public override void OnSelectedTableChanged() {
			base.OnSelectedTableChanged();
			if(string.IsNullOrEmpty(SelectedKeyExpression) && SelectedTable.KeyExpressions.Count > 0)
				SelectedKeyExpression = SelectedTable.KeyExpressions[0].Expression;
		}
	}
}
