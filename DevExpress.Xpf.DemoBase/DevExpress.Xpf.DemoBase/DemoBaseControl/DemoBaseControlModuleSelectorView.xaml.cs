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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.DemoBase.DemoTesting;
using DevExpress.Xpf.DemoBase.Helpers;
namespace DevExpress.Xpf.DemoBase.Internal {
	interface ISupportModuleSelectorScrollViewer {
		Size MeasureConstraint { get; set; }
	}
	class ModuleSelectorScrollViewer : ScrollViewer {
		IColumnControl ColumnControl { get { return Content as IColumnControl; } }
		ISupportModuleSelectorScrollViewer TypedContent { get { return Content as ISupportModuleSelectorScrollViewer; } }
		protected override Size MeasureOverride(Size constraint) {
			if(TypedContent != null) {
				TypedContent.MeasureConstraint = constraint;
			}
			return base.MeasureOverride(constraint);
		}
		public ModuleSelectorScrollViewer() {
			ScrollChanged += ModuleSelectorScrollViewer_ScrollChanged;
		}
		void ModuleSelectorScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			if(ColumnControl != null) {
				ColumnControl.VisibleArea = new Rect(
					HorizontalOffset,
					0,
					ViewportWidth,
					ViewportHeight);
			}
		}
	}
	partial class DemoBaseControlModuleSelectorView : DemoBaseControlPartView {
		public DemoBaseControlModuleSelectorView() {
			InitializeComponent();
		}
	}
}
