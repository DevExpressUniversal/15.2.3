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
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Adorners {
	public abstract class DiagramAdornerTrackerBase {
		DiagramAdornerBase adorner;
		DiagramControl diagram;
		Func<DiagramControl, DiagramAdornerBase, DiagramAdornerBase, bool> filterCondition;
		public DiagramAdornerTrackerBase(DiagramControl diagram, Func<DiagramControl, DiagramAdornerBase, DiagramAdornerBase, bool> filterCondition) {
			this.adorner = null;
			this.diagram = diagram;
			this.filterCondition = filterCondition;
		}
		public void Track(DiagramAdornerBase nextAdorner) {
			if(!AcceptAdorner(nextAdorner)) return;
			if(this.adorner == null || !filterCondition(Diagram, this.adorner, nextAdorner)) {
				Reset();
				this.adorner = nextAdorner;
				return;
			}
			RaiseChanged(this.adorner, nextAdorner);
			this.adorner = nextAdorner;
		}
		protected abstract bool AcceptAdorner(DiagramAdornerBase adorner);
		public void Reset() {
			this.adorner = null;
		}
		protected DiagramControl Diagram { get { return diagram; } }
		protected void RaiseChanged(DiagramAdornerBase prev, DiagramAdornerBase next) {
			OnChanged(new DiagramAdornerControllerAdornerChangedEventArgs(prev, next));
		}
		protected void OnChanged(DiagramAdornerControllerAdornerChangedEventArgs e) {
			if(Changed != null) Changed(this, e);
		}
		public event DiagramAdornerControllerAdornerChangedEventHandler Changed;
	}
	public class DiagramSelectionTracker : DiagramAdornerTrackerBase {
		public DiagramSelectionTracker(DiagramControl diagram)
			: base(diagram, DefaultFilterCondition) {
		}
		protected override bool AcceptAdorner(DiagramAdornerBase adorner) {
			return adorner.AffectsSelection;
		}
		static Func<DiagramControl, DiagramAdornerBase, DiagramAdornerBase, bool> DefaultFilterCondition = (diagram, prevAdorner, nextAdorner) => {
			if(diagram.Controller.PrimarySelection == diagram.Page) return false;
			if(prevAdorner != null && prevAdorner.IsPreviewAdorner && prevAdorner.IsDestroyed) return false;
			if(prevAdorner.IsSelection() && nextAdorner.IsSelection()) {
				IDiagramSelectionSupports prev = (IDiagramSelectionSupports)prevAdorner;
				IDiagramSelectionSupports next = (IDiagramSelectionSupports)nextAdorner;
				if(!prev.IsMultiSelection && !next.IsMultiSelection) return false;
			}
			return true;
		};
	}
}
