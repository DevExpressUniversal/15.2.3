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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
namespace DevExpress.Xpf.Editors.Helpers {
	public class CollectionViewCurrentFilteredSortedDataView : LocalCurrentFilteredSortedDataView {
		ICollectionViewHelper server;
		string filterCriteria;
		string displayFilterCriteria;
		readonly IEnumerable<SortingInfo> actualSorting;
		readonly int groupCount;
		new DefaultDataView ListSource {
			get { return base.ListSource as DefaultDataView; }
		}
		public CollectionViewCurrentFilteredSortedDataView(object view, object handle, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, 
			string filterCriteria, string displayFilterCriteria)
			: base(view, handle, valueMember, displayMember, groups, sorts, filterCriteria, displayFilterCriteria) {
			this.filterCriteria = filterCriteria;
			this.displayFilterCriteria = displayFilterCriteria;
			List<SortingInfo> resultSorting = groups != null ? groups.Select(x => new SortingInfo(x.FieldName, ListSortDirection.Ascending)).ToList() : new List<SortingInfo>();
			resultSorting.AddRange(sorts);
			actualSorting = resultSorting;
			groupCount = groups.Count();
		}
		protected override void InitializeView(object source) {
			base.InitializeView(source);
			DefaultDataView view = (DefaultDataView)source;
			ICollectionView originalCollectionView = ((ICollectionViewHelper)view.ListSource).Collection;
			var collectionView = CollectionViewSource.GetDefaultView(originalCollectionView.Cast<object>().ToList());
			ICollectionViewHelper helper = new ICollectionViewHelper(collectionView, CollectionView.NewItemPlaceholder);
			var criteria = CriteriaOperator.And(new[] {CriteriaOperator.Parse(filterCriteria), CriteriaOperator.Parse(displayFilterCriteria)});
			helper.Apply(criteria, 
				actualSorting.Select(x => new ServerModeOrderDescriptor(new OperandProperty(x.FieldName), x.OrderBy == ListSortDirection.Descending)).ToList(), groupCount, null, null);
			server = helper;
		}
		protected override object CreateVisibleListWrapper() {
			return server.Collection;
		}
	}
}
