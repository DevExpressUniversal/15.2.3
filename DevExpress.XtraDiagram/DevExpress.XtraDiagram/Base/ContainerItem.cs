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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram {
	public partial class DiagramContainer : DiagramItem, IDiagramContainer {
		public DiagramContainer() {
		}
		public DiagramContainer(Rectangle bounds)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height) {
		}
		public DiagramContainer(int x, int y, int width, int height)
			: base(x, y, width, height) {
		}
		protected override IList<IDiagramItem> CreateItemCollection() {
			DiagramItemCollection items = new DiagramItemCollection(this);
			items.ListChanged += OnItemCollectionChanged;
			return items;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual new DiagramItemCollection Items { get { return (DiagramItemCollection)base.Items; } }
		protected virtual bool ShouldSerializeAdjustBoundsBehavior() {
			return AdjustBoundsBehavior != AdjustBoundaryBehavior.AutoAdjust;
		}
		protected virtual void ResetAdjustBoundsBehavior() { AdjustBoundsBehavior = AdjustBoundaryBehavior.AutoAdjust; }
		protected sealed override DiagramItemController CreateItemController() {
			return CreateContainerController();
		}
		protected virtual DiagramContainerController CreateContainerController() {
			return new DiagramContainerController(this);
		}
		protected internal override bool IsRoutable { get { return false; } }
		protected internal override string EditValue {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		protected virtual void OnItemCollectionChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemAdded) {
				DiagramItem item = Items[e.NewIndex];
				GetRootItem().DoIfNotNull(root => root.AddIntoContainer(item));
			}
			OnPropertiesChanged();
		}
		protected internal new DiagramContainerController Controller { get { return base.Controller as DiagramContainerController; } }
	}
}
