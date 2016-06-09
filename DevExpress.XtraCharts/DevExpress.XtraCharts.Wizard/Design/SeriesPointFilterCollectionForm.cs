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
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public partial class SeriesPointFilterCollectionForm : XtraForm {
		readonly ISeriesPointFilterCollectionAccessor filtersAccessor;
		SeriesPointFilterCollectionForm() {
			InitializeComponent();
		}
		public SeriesPointFilterCollectionForm(SeriesPointFilterCollection filters)
			: this(new SeriesPointFilterCollectionAccessor(filters)) {
		}
		public SeriesPointFilterCollectionForm(ISeriesPointFilterCollectionAccessor filtersAccessor) : this() {
			this.filtersAccessor = filtersAccessor;
			for (int filterIndex = 0; filterIndex < filtersAccessor.Count; filterIndex++) {
				lbFilters.Items.Add(filtersAccessor[filterIndex].ToString());
			}
			if (filtersAccessor.Count > 0)
				lbFilters.SelectedIndex = 0;
			rgConjunction.SelectedIndex = filtersAccessor.ConjunctionMode == ConjunctionTypes.And ? 0 : 1;
		}
		void SeriesPointFilterCollectionForm_Load(object sender, EventArgs e) {
			lbFilters.Focus();
		}
		void lbFilters_SelectedIndexChanged(object sender, EventArgs e) {
			propertyGrid.SelectedObject = (lbFilters.SelectedIndex >= 0 && lbFilters.Items.Count > lbFilters.SelectedIndex) ? filtersAccessor[lbFilters.SelectedIndex] : null;
			btnRemove.Enabled = lbFilters.SelectedIndex >= 0;
		}
		void btnAdd_Click(object sender, EventArgs e) {
			SeriesPointFilter filter = new SeriesPointFilter(SeriesPointKey.Argument, DataFilterCondition.Equal, null);
			int index = filtersAccessor.Add(filter);
			if (index >= 0) {
				lbFilters.Items.Add(filter.ToString());
				lbFilters.SelectedIndex = index;
			}
		}
		void btnRemove_Click(object sender, EventArgs e) {
			int selectedIndex = lbFilters.SelectedIndex;
			if (selectedIndex >= 0) {
				filtersAccessor.RemoveAt(selectedIndex);
				lbFilters.Items.RemoveAt(selectedIndex);
				if (lbFilters.Items.Count > 0)
					lbFilters.SelectedIndex = selectedIndex >= lbFilters.Items.Count ? lbFilters.Items.Count - 1 : selectedIndex;
			}
		}
		void rgConjunction_SelectedIndexChanged(object sender, EventArgs e) {
			ConjunctionTypes newConjunctionMode = rgConjunction.SelectedIndex == 0 ? ConjunctionTypes.And : ConjunctionTypes.Or;
			if (newConjunctionMode != filtersAccessor.ConjunctionMode)
				filtersAccessor.ConjunctionMode = newConjunctionMode;
		}	   
	}
	public interface ISeriesPointFilterCollectionAccessor {
		int Count { get; }
		ConjunctionTypes ConjunctionMode { get; set; }
		object this[int index] { get; }
		int Add(SeriesPointFilter item);
		void RemoveAt(int index);
	}
	public class SeriesPointFilterCollectionAccessor : ISeriesPointFilterCollectionAccessor {
		readonly SeriesPointFilterCollection collection;
		public SeriesPointFilterCollectionAccessor(SeriesPointFilterCollection collection) {
			this.collection = collection;
		}
		#region ISeriesPointFilterCollectionAccessor Members
		int ISeriesPointFilterCollectionAccessor.Count { get { return collection.Count; } }
		object ISeriesPointFilterCollectionAccessor.this[int index] { get { return collection[index]; } }
		ConjunctionTypes ISeriesPointFilterCollectionAccessor.ConjunctionMode {
			get { return collection.ConjunctionMode; }
			set { collection.ConjunctionMode = value; }
		}
		int ISeriesPointFilterCollectionAccessor.Add(SeriesPointFilter item) {
			return collection.Add(item);
		}
		void ISeriesPointFilterCollectionAccessor.RemoveAt(int index) {
			collection.RemoveAt(index);
		}
		#endregion
	}
}
