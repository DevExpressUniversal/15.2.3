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
using System.Reflection;
using DevExpress.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public struct InsertedItem {
		readonly int index;
		readonly ChartModelElement item;
		public InsertedItem(int index, ChartModelElement item) {
			this.index = index;
			this.item = item;
		}
		public int Index {
			get { return index; }
		}
		public ChartModelElement Item {
			get { return item; }
		}
	}
	public abstract class ChartModelElement : NotifyPropertyChangedObject {
		readonly object chartElement;
		readonly ChartModelElement parent;
		PropertyGridModelBase propertyGridModel;
		public object ChartElement {
			get { return chartElement; }
		}
		public abstract IEnumerable<ChartModelElement> Children {
			get;
		}
		public ChartModelElement Parent {
			get { return parent; }
		}
		public PropertyGridModelBase PropertyGridModel {
			get { return propertyGridModel; }
			protected set {
				if (propertyGridModel != value) {
					propertyGridModel = value;
					OnPropertyChanged("PropertyGridModel");
				}
			}
		}
		public ChartModelElement Root {
			get {
				if (parent == null)
					return this;
				return parent.Root;
			}
		}
		public virtual ChartModelElement SelectionOverride {
			get { return this; }
		}
		public ChartModelElement(ChartModelElement parent, object chartElement) {
			this.chartElement = chartElement;
			this.parent = parent;
		}
		protected virtual void UpdateChildren() { }
		protected void WriteUnsupportedPropertySetWarning(string propertyName, string className) {
#if !DEBUGTEST
			ChartDebug.Fail("Attempt to set unsupported property: '" + propertyName + "' in a class: '" + className + "'.");
#else
			if (!SupressWriteUnsupportedPropertySetWarning)
				ChartDebug.Fail("Attempt to set unsupported property: '" + propertyName + "' in a class: '" + className + "'.");
#endif
		}
		protected string GetDataMemberName(object dataSource, string dataMember) {
			DevExpress.Data.Browsing.DataContext actualDataContext = new DevExpress.Data.Browsing.DataContext();
			try {
				if (String.IsNullOrEmpty(dataMember))
					return String.Empty;
				return actualDataContext.GetDataMemberDisplayName(dataSource, String.Empty, BindingProcedure.ConvertToActualDataMember(String.Empty, dataMember));
			}
			finally {
				actualDataContext.Dispose();
			}
		}
		public T GetParent<T>() where T : ChartModelElement {
			ChartModelElement current = Parent;
			while (current != null) {
				if (current is T)
					return (T)current;
				current = current.Parent;
			}
			return null;
		}
		public ChartModelElement PerformChildSearch(object chartElement) {
			ChartModelElement founded = null;
			if (chartElement != null) {
				foreach (ChartModelElement child in Children) {
					if (child.chartElement == chartElement) {
						founded = child;
						break;
					}
				}
			}
			return founded;
		}
		public ChartModelElement PerformFullSearch(object chartElement) {
			if (chartElement != null) {
				if (this.chartElement == chartElement)
					return this;
				ChartModelElement founded = null;
				if (Children != null)
					foreach (ChartModelElement child in Children) {
						if (child != null) {
							founded = child.PerformFullSearch(chartElement);
							if (founded != null)
								break;
						}
					}
				return founded;
			}
			return null;
		}
		public void RecursivelyUpdateChildren() {
			UpdateChildren();
			if (Children != null)
				foreach (var child in Children)
					if (child != null)
						child.RecursivelyUpdateChildren();
		}
		public void InvalidatePropertyGridModel() {
			if (propertyGridModel != null) {
				propertyGridModel.Update();
				PropertyGridModelBase model = PropertyGridModel;
				PropertyGridModel = null;
				PropertyGridModel = model;
			}
		}
		public void UpdatePropertyGridModel(PropertyGridModelBase propertyGridModel) {
			this.propertyGridModel = propertyGridModel;
		}
		public object GetChartElementProperty(string propertyName) {
			object oldValue = String.IsNullOrEmpty(propertyName) ? ChartElement : CommandUtils.GetObjectProperty(ChartElement, propertyName);
			return oldValue;
		}
		public object SetChartElementProperty(string propertyName, object value) {
			object oldValue = CommandUtils.GetObjectProperty(ChartElement, propertyName);
			CommandUtils.SetObjectProperty(ChartElement, propertyName, value);
			return oldValue;
		}
	}
}
