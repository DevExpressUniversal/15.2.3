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
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[ListBindable(false)]
	public class ValueDataMemberCollection : CollectionBase, IOwnedElement {
		const char separator = ';';
		SeriesBase owner;
		internal SeriesBase Owner { get { return owner; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ValueDataMemberCollectionItem")]
#endif
		public string this[int index] {
			get { return (string)List[index]; }
			set {
				if (value != this[index]) {
					CheckDataMember(value);
					Changing();
					List[index] = value;
					Changed();
				}
			}
		}
		internal ValueDataMemberCollection(string str) {
			str.Trim();
			string[] array = str.Split(separator);
			for (int i = 0; i < array.Length; i++)
				InnerList.Add(array[i].Trim());
		}
		internal ValueDataMemberCollection(SeriesBase owner) {
			this.owner = owner;
			InnerList.Add("");
		}
		#region IOwnedElement implementation
		IOwnedElement IOwnedElement.Owner { get { return Owner; } }
		IChartContainer IOwnedElement.ChartContainer { get { return Owner.ChartContainer; } }
		#endregion
		internal void Full(params string[] coll) {
			InnerList.Clear();
			InnerList.AddRange(coll);
		}
		internal void SetDimension(int dimension) {
			if (dimension > Count)
				for (int i = Count; i < dimension; i++)
					InnerList.Add(String.Empty);
			else if (dimension < Count)
				InnerList.RemoveRange(dimension, Count - dimension);
		}
		internal bool IsEmpty() {
			foreach (string str in InnerList)
				if (str != null && str != String.Empty)
					return false;
			return true;
		}
		internal string[] GetArray() {
			string[] array = new string[InnerList.Count];
			InnerList.CopyTo(array, 0);
			return array;
		}
		internal void CheckDataMember(string dataMember) {
			if (!String.IsNullOrEmpty(dataMember) && owner != null && !owner.Loading)
				owner.CheckDataMember(dataMember, owner.ValueScaleType);
		}
		internal void Changing() {
			if (this.owner != null)
				this.owner.SendNotification(new ChartElement.ElementWillChangeNotification(this));
		}
		internal void Changed() {
			if (this.owner != null)
				this.owner.BindingChanged();
		}
		public virtual void Assign(ValueDataMemberCollection collection) {
			this.InnerList.Clear();
			for (int i = 0; i < collection.InnerList.Count; i++)
				InnerList.Add(collection[i]);
		}
		public void AddRange(params string[] coll) {
			ArrayList oldValues = new ArrayList(InnerList);
			try {
				InnerList.Clear();
				for (int i = 0; i < coll.Length; i++)
					CheckDataMember(coll[i]);
				Changing();
				InnerList.AddRange(coll);
				Changed();
			}
			catch {
				InnerList.Clear();
				InnerList.AddRange(oldValues);
				throw;
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			ValueDataMemberCollection collection = obj as ValueDataMemberCollection;
			if (collection != null && collection.InnerList.Count == InnerList.Count) {
				for (int i = 0; i < InnerList.Count; i++)
					if (collection.InnerList[i] != InnerList[i])
						return false;
				return true;
			}
			else
				return false;
		}
		public override string ToString() {
			return string.Join(new string(separator, 1), GetArray());
		}
	}
}
