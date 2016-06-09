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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	class CollectionViewSettingsModel : CollectionSettingsModelBase, ICollectionViewSettingsModel {
		public CollectionViewSettingsModel(ICollectionViewTypesProvider typesProvider, IDataSourceInfo info)
			: base(info) {
			this.selectedCultureCore = CultureInfo.CurrentCulture;
			Cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			CollectionViewTypes = typesProvider;
			OnSelectedCollectionViewTypeChanged();
		}
		protected sealed override Type GetKey() {
			return typeof(ICollectionViewSettingsModel);
		}
		public IEnumerable<CultureInfo> Cultures {
			get;
			private set;
		}
		CultureInfo selectedCultureCore;
		public CultureInfo SelectedCulture {
			get { return selectedCultureCore; }
			set { SetProperty(ref selectedCultureCore, value, "SelectedCulture"); }
		}
		public IEnumerable<string> CollectionViewTypes {
			get;
			private set;
		}
		string selectedCollectionViewTypeCore;
		public string SelectedCollectionViewType {
			get { return selectedCollectionViewTypeCore; }
			set { SetProperty(ref selectedCollectionViewTypeCore, value, "SelectedCollectionViewType", OnSelectedCollectionViewTypeChanged); }
		}
		protected virtual void OnSelectedCollectionViewTypeChanged() {
			AllowGrouping = !string.IsNullOrEmpty(SelectedCollectionViewType) && CalcAllowGrouping(SelectedCollectionViewType);
			AllowSorting = !string.IsNullOrEmpty(SelectedCollectionViewType) && CalcAllowSorting(SelectedCollectionViewType);
			AllowPaging = !string.IsNullOrEmpty(SelectedCollectionViewType) && CalcAllowPaging(SelectedCollectionViewType);
		}
		protected virtual bool CalcAllowGrouping(string collectionViewTypeName) {
			return Array.IndexOf(NoGroupingViews, collectionViewTypeName) == -1;
		}
		protected virtual bool CalcAllowSorting(string collectionViewTypeName) {
			return Array.IndexOf(NoSortingViews, collectionViewTypeName) == -1;
		}
		protected virtual bool CalcAllowPaging(string collectionViewTypeName) {
			return Array.IndexOf(PagingViews, collectionViewTypeName) != -1;
		}
		#region View Types
		static string[] NoGroupingViews = new string[] { "CollectionView" };
		static string[] NoSortingViews = new string[] { "CollectionView", "BindingListCollectionView" };
		static string[] PagingViews = new string[] { "PagedCollectionView" };
		#endregion View Types
	}
}
