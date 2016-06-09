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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraTab;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class SeriesViewControlBase : FilterTabsControl {
		SeriesViewBase view;
		SeriesBase series;
		Chart chart;
		protected SeriesViewBase View { get { return view; } }
		protected SeriesBase Series { get { return series; } }
		protected Chart Chart { get { return chart; } }
		public override XtraTabControl TabControl { get { return this.tbcPagesControl; } }
		public SeriesViewControlBase() {
			InitializeComponent();
		}
		public virtual void SelectHitTestElement(ChartElement element) {
		}
		public void Initialize(SeriesViewBase view, SeriesBase series, Chart chart, UserLookAndFeel lookAndFeel, CollectionBase filter, object selectedTabHandle) {
			this.view = view;
			this.series = series;
			this.chart = chart;
			InitializeCore(lookAndFeel, filter, selectedTabHandle);
		}
		protected SeriesViewPageTab GetPageTabByName(string pageTabName) {
			ViewType viewType = SeriesViewFactory.GetViewType(view);
			return (SeriesViewPageTab)Enum.Parse(typeof(SeriesViewPageTab), viewType.ToString() + pageTabName);
		}
	}
	internal abstract class SeriesViewBaseController {
		SeriesViewBase view;
		SeriesBase series;
		public SeriesViewBase View { get { return view; } }
		public SeriesBase Series { get { return series; } }
		public abstract bool ColorEachSupported { get; }
		public virtual bool ColorEach { get { return false; } set { } }
		public virtual bool ColorSupported { get { return true; } }
		public virtual FillStyleBase FillStyle { get { return null; } }
		public virtual LineStyle LineStyle { get { return null; } }
		public SeriesViewBaseController(SeriesViewBase view, SeriesBase series) {
			this.view = view;
			this.series = series;
		}
	}
}
