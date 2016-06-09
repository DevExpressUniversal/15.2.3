#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.ObjectModel;
namespace DevExpress.DashboardCommon.Native {
	public abstract class ElementContainer<TElement> where TElement : class, IEditNameProvider {
		readonly ReadOnlyCollection<TElement> elements;
		TElement activeElement;
		IEnumerable<TElement> ActualElements {
			get {
				foreach (TElement element in elements)
					if (!IsElementEmpty(element))
						yield return element;
			}
		}
		public IList<string> ElementNames {
			get {
				List<string> elementNames = new List<string>();
				foreach (TElement element in ActualElements)
					elementNames.Add(element.DisplayName);
				return elementNames;
			}
		}
		public virtual bool ElementSelectionEnabled { get { return ElementNames.Count > 1; } }
		public TElement ActiveElement {
			get { return activeElement; }
			private set {
				if(value != activeElement) {
					TElement oldActiveElement = activeElement;
					activeElement = value;
				}
			}
		}
		public int ActiveElementIndex {
			get {				
				int firstIndex = -1;
				int elementIndex = 0;
				foreach (TElement element in ActualElements) {
					if (element == activeElement)
						return elementIndex;
					if (firstIndex == -1)
						firstIndex = elementIndex;
					elementIndex++;
				}
				return firstIndex;
			}
			set {
				int elementIndex = 0;
				foreach (TElement element in ActualElements) {
					if (elementIndex == value) {
						ActiveElement = element;
						return;
					}
					elementIndex++;
				}
				ActiveElement = null;
			}
		}
		protected ElementContainer(ReadOnlyCollection<TElement> elements) {
			this.elements = elements;
		}
		public void ValidateActiveElement() {
			if (!elements.Contains(ActiveElement))
				ResetActiveElement();
		}
		public void ResetActiveElement() {
			ActiveElement = elements.Count > 0 ? elements[0] : null;
		}
		protected abstract bool IsElementEmpty(TElement element);
	}
	public class KpiElementContainer<TKpiElement> : ElementContainer<TKpiElement> where TKpiElement : KpiElement, new() {
		readonly KpiDashboardItem<TKpiElement> dashboardItem;
		public override bool ElementSelectionEnabled { get { return base.ElementSelectionEnabled && dashboardItem.SeriesDimensions.Count > 0; } }
		public KpiElementContainer(KpiDashboardItem<TKpiElement> dashboardItem, ReadOnlyCollection<TKpiElement> elements)
			: base(elements) {
			this.dashboardItem = dashboardItem;
		}
		protected override bool IsElementEmpty(TKpiElement element) {
			return element.DataItemsCount == 0;
		}
	}
	public class PieElementContainer : ElementContainer<Measure> {
		readonly PieInternal pie;
		public override bool ElementSelectionEnabled { get { return base.ElementSelectionEnabled && pie.AllowLayers; } }
		public PieElementContainer(PieInternal pie, ReadOnlyCollection<Measure> elements)
			: base(elements) {
			this.pie = pie;
		}
		protected override bool IsElementEmpty(Measure element) {
			return pie.IsValueEmpty(element);
		}
	}
	public class ChoroplethMapLayerElementContainer : ElementContainer<ChoroplethMap> {
		public ChoroplethMapLayerElementContainer(ReadOnlyCollection<ChoroplethMap> elements)
			: base(elements) {
		}
		protected override bool IsElementEmpty(ChoroplethMap element) {
			return element.DataItemsCount == 0;
		}
	}
}
