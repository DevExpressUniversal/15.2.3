#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ColumnModelNodesComparer : ModelNodesComparer {
		Dictionary<int, int?> indexCache = new Dictionary<int, int?>();
		public ColumnModelNodesComparer() {
		}
		protected override int? GetModelNodeIndex(IModelNode node) {
			int modelNodeHashCode = node.GetHashCode();
			int? result;
			if(!indexCache.TryGetValue(modelNodeHashCode, out result)) {
				result = node.Index;
				indexCache.Add(modelNodeHashCode, result);
			}
			return result;
		}
		protected override string GetModelNodeDisplayValue(IModelNode node) {
			return ((IModelLayoutElement)node).Id;
		}
	}
	public class ModelBandLayoutNodesComparer : ModelLayoutElementNodesComparer<IModelBandedLayoutItem> {
		public ModelBandLayoutNodesComparer(bool compareByVisibleIndex)
			: base(compareByVisibleIndex) {
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelLayoutElementNodesComparer<T> : ColumnModelNodesComparer, IComparer<T> where T : IModelLayoutElement {
		bool compareByIndex;
		public ModelLayoutElementNodesComparer(bool compareByIndex) {
			this.compareByIndex = compareByIndex;
		}
		protected override bool ShouldCompareByIndex() {
			return compareByIndex;
		}
		#region IComparer<T> Members
		public virtual int Compare(T x, T y) {
			return base.Compare((IModelNode)x, (IModelNode)y);
		}
		public override int Compare(IModelNode node1, IModelNode node2) {
			return Compare((T)node1, (T)node2);
		}
		#endregion
	}
}
