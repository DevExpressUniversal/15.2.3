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

using System.Windows;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class Diagram2D : Diagram {
		bool shouldUpdateLayout;
		protected override bool Is3DView { 
			get { return false; } 
		}
		public Diagram2D() {
			LayoutUpdated += delegate {
				if (shouldUpdateLayout) {
					shouldUpdateLayout = false;
					InvalidateDiagram();
				}
				RaiseCanExecutedChanged();
			};
		}
		protected override Size MeasureOverride(Size constraint) {
			shouldUpdateLayout = false;
			return base.MeasureOverride(constraint);
		}
		protected override bool ProcessChanging(ChartUpdate args) {
			shouldUpdateLayout = true;
			return base.ProcessChanging(args) && (args.Change & ChartElementChange.Diagram3DOnly) == 0;
		}
		protected internal override void UpdateSeriesItems() {
			base.UpdateSeriesItems();
			foreach (Series series in Series)
				if (series.ActualLabel != null)
					series.ActualLabel.UpdateItems();
		}
		protected override ISeriesItem GetSeriesItem(Series series) {
			return series.Item;
		}
		protected internal virtual void RaiseCanExecutedChanged() { }
		protected internal override void InvalidateDiagram() {
			CommonUtils.InvalidateMeasure(this);
		}
	}
}
