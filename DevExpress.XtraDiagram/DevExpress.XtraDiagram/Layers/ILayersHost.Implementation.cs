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
using System.Windows;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Platform;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
namespace DevExpress.XtraDiagram {
	public partial class DiagramControl : ILayersHost {
		#region ILayersHost
		LayersHostController ILayersHost.Controller { get { return LayersHostController; } }
		void ILayersHost.SetCursor(DiagramCursor cursor) {
			Cursor newCursor = cursor.ToWinCursor();
			if(!ReferenceEquals(Cursor, newCursor)) Cursor = newCursor;
		}
		void ILayersHost.FocusSurface() {
			Focus();
		}
		IInputElement ILayersHost.FindInputElement(Point displayPosition) {
			return DiagramViewInfo.FindInputElement(displayPosition.ToWinPoint());
		}
		Size ILayersHost.ViewportSize {
			get { return ViewInfo.ContentRect.Size.ToPlatformSize(); }
		}
		void ILayersHost.InvalidateScrollInfo() {
			LayoutChanged();
		}
		void ILayersHost.UpdateLayers(double zoom) {
			UpdateLayers();
		}
		IMouseArgs ILayersHost.CreatePlatformMouseArgs() { return CreatePlatformMouseArgs(); }
		#endregion
		protected internal virtual IMouseArgs CreatePlatformMouseArgs() {
			return new PlatformMouseArgs(DiagramHandler);
		}
		protected virtual void UpdateLayers() {
			if(LockScrollUpdate) return;
			this.lockScrollUpdate++;
			try {
				ScrollPos = LayersHostController.Offset.ToWinPoint();
			}
			finally {
				this.lockScrollUpdate--;
			}
		}
	}
}
