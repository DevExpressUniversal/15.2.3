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

using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class DomainPanel : SimplePanel, IHitTestableElement {
		public static readonly DependencyProperty PaneProperty = DependencyPropertyManager.Register("Pane", typeof(Pane), typeof(DomainPanel));
		[
		Category(Categories.Common)
		]
		public Pane Pane {
			get { return (Pane)GetValue(PaneProperty); }
			set { SetValue(PaneProperty, value); }
		}
		#region IHitTestableElement implementation
		object IHitTestableElement.Element { get { return Pane.Diagram; } }
		object IHitTestableElement.AdditionalElement { get { return Pane; } }
		#endregion
	}
	public class CircularDomainPanel : SimplePanel, IHitTestableElement {
		public static readonly DependencyProperty DiagramProperty = DependencyPropertyManager.Register("Diagram", typeof(CircularDiagram2D), typeof(CircularDomainPanel));
		[
		Category(Categories.Common)
		]
		public CircularDiagram2D Diagram {
			get { return (CircularDiagram2D)GetValue(DiagramProperty); }
			set { SetValue(DiagramProperty, value); }
		}
		#region IHitTestableElement implementation
		object IHitTestableElement.Element { get { return Diagram; } }
		object IHitTestableElement.AdditionalElement { get { return null; } }
		#endregion
	}
}
