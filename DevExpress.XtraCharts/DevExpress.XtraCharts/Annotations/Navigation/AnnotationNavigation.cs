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
using System.Drawing;
using DevExpress.Charts.Native;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public interface IAnnotationDragElement {
		void DoSelect();
	}
	public class AnnotationNavigation {
		readonly AnnotationRepository annotationRepository;		
		public AnnotationNavigation(AnnotationRepository annotationRepository) {
			this.annotationRepository = annotationRepository;			
		}
		AnnotationOperation CreateOperation(IAnnotationDragPoint dragPoint) {
			return dragPoint != null ? dragPoint.CreateOperation() : null;
		}
		void ResetSelect() {
			foreach (Annotation annotation in annotationRepository)
				annotation.RuntimeOperationSelect = false;
		}
		public bool CanDrag() {
			foreach (Annotation annotation in annotationRepository)
				if (annotation.CanDrag())
					return true;
			return false;
		}
		public void DoSelect(IAnnotationDragElement element, IChartContainer container) {
			if (CanDrag()) {
				ResetSelect();
				if (element != null)
					element.DoSelect();
				if (container.RenderProvider != null)
					container.RenderProvider.Invalidate();
				container.Chart.ClearCache();
			}
		}
		public bool PerformDragging(int x, int y, int dx, int dy, IAnnotationDragPoint dragPoint) {
			AnnotationOperation operation = CreateOperation(dragPoint);
			if (operation == null)
				return false;
			operation.Run(x, y, dx, dy);
			return true;
		}		
	}
}
