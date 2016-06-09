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
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
namespace DevExpress.XtraDiagram.Adorners {
	public class DiagramInplaceEditorAdorner : DiagramItemAdornerBase {
		Action onDestroy;
		Size bestSize;
		public DiagramInplaceEditorAdorner(IDiagramItem item, Size bestSize, Action onDestroy) : base(item) {
			this.bestSize = bestSize;
			this.onDestroy = onDestroy;
		}
		public void OnDestroy() {
			onDestroy();
		}
		public void SetBestSize(Size size) {
			if(!IsAutoSize) return;
			this.bestSize = size;
			Rectangle rect = CalcBestDisplayBounds(DisplayBounds.GetCenterPoint());
			UpdateDisplayBounds(rect);
		}
		protected override DiagramElementBounds CalcAdornerBounds() {
			DiagramElementBounds bounds = base.CalcAdornerBounds();
			if(IsAutoSize)
				bounds.SetDisplayRect(CalcBestDisplayBounds(bounds.DisplayBounds.Location));
			return bounds;
		}
		protected Rectangle CalcBestDisplayBounds(Point point) {
			Rectangle rect = new Rectangle(point, Size.Empty);
			return Rectangle.Inflate(rect, BestSize.Width / 2 + 1, BestSize.Height / 2 + 1);
		}
		protected bool IsAutoSize {
			get { return double.IsNaN(PlatformBounds.Width) && double.IsNaN(PlatformBounds.Height); }
		}
		public Size BestSize { get { return bestSize; } }
		public override DiagramAdornerPainterBase GetPainter() {
			return new DiagramInplaceEditorAdornerPainter();
		}
		public override DiagramAdornerObjectInfoArgsBase GetDrawArgs() {
			return new DiagramInplaceEditorAdornerObjectInfoArgs();
		}
		public override bool AffectsOnInplaceEditing { get { return true; } }
	}
}
