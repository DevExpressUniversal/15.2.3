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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraDiagram.Base {
	[Editor("DevExpress.XtraDiagram.Design.DiagramConnectorPointCollectionEditor," + AssemblyInfo.SRAssemblyDiagramDesignFull, typeof(UITypeEditor))]
	public class DiagramConnectorPointCollection : Collection<PointFloat> {
		int lockUpdate;
		public DiagramConnectorPointCollection() {
			this.lockUpdate = 0;
		}
		public DiagramConnectorPointCollection(IList<PointF> pointList) : base() {
			AddRange(pointList);
		}
		public DiagramConnectorPointCollection(IList<Point> pointList) : base() {
			AddRange(pointList);
		}
		public DiagramConnectorPointCollection(IList<PointFloat> pointList) : base() {
			AddRange(pointList);
		}
		public virtual void AddRange(IEnumerable<Point> points) {
			AddRange(points.Select(point => new PointFloat(point.X, point.Y)));
		}
		public virtual void AddRange(IEnumerable<PointF> points) {
			AddRange(points.Select(point => new PointFloat(point.X, point.Y)));
		}
		public virtual void AddRange(IEnumerable<PointFloat> points) {
			BeginUpdate();
			try {
				foreach(PointFloat point in points) {
					Add(point);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override void ClearItems() {
			base.ClearItems();
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void InsertItem(int position, PointFloat point) {
			base.InsertItem(position, point);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, position));
		}
		protected override void RemoveItem(int position) {
			base.RemoveItem(position);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, position));
		}
		protected override void SetItem(int index, PointFloat point) {
			base.SetItem(index, point);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}
		#region ListChanged
		public event ListChangedEventHandler ListChanged;
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if(lockUpdate != 0) return;
			if(ListChanged != null) ListChanged(this, e);
		}
		#endregion
	}
}
