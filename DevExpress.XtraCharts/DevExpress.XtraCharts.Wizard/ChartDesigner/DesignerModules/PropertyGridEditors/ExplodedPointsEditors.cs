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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class ExplodedPointsModelEditor : ChartEditorBase {
		CommandManager commandManager;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			ExplodedSeriesPointCollectionModel collection = Value as ExplodedSeriesPointCollectionModel;
			PieViewBaseModel view = Instance as PieViewBaseModel;
			if (collection == null || view == null)
				return null;
			commandManager = collection.CommandManager;
			commandManager.BeginTransaction();
			ExplodedPointsListForm form = new ExplodedPointsListForm(view.Parent.ChartElement as Series, new ExpoldedPointsModelCollectionAccessor(collection));
			IChartContainer chartContainer = ((IOwnedElement)collection.ChartCollection).ChartContainer;
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chartContainer.RenderProvider.LookAndFeel;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			base.AfterShowDialog(form, dialogResult);
			commandManager.CommitTransaction();
			commandManager = null;
		}
	}
	public class ExpoldedPointsModelCollectionAccessor : IExpoldedPointsCollectionAccessor {
		readonly ExplodedSeriesPointCollectionModel collection;
		public ExpoldedPointsModelCollectionAccessor(ExplodedSeriesPointCollectionModel collection) {
			this.collection = collection;
		}
		#region IExpoldedPointsCollectionAccessor Members
		void IExpoldedPointsCollectionAccessor.Add(SeriesPoint seriesPoint) {
			collection.AddNewElement(seriesPoint);
		}
		void IExpoldedPointsCollectionAccessor.Remove(SeriesPoint seriesPoint) {
			collection.DeleteElement(seriesPoint);
		}
		bool IExpoldedPointsCollectionAccessor.Contains(SeriesPoint seriesPoint) {
			return collection.ChartCollection.Contains(seriesPoint);
		}
		#endregion
	}
	public class SeriesPointFilterModelCollectionEditor : ChartEditorBase {
		CommandManager commandManager;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			SeriesPointFilterCollectionModel collection = Value as SeriesPointFilterCollectionModel;
			PieViewBaseModel view = Instance as PieViewBaseModel;
			if (collection == null || view == null)
				return null;
			commandManager = collection.CommandManager;
			commandManager.BeginTransaction();
			SeriesPointFilterCollectionForm form = new SeriesPointFilterCollectionForm(new SeriesPointFilterModelCollectionAccessor(collection));
			IChartContainer chartContainer = ((IOwnedElement)collection.ChartCollection).ChartContainer;
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chartContainer.RenderProvider.LookAndFeel;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			base.AfterShowDialog(form, dialogResult);
			commandManager.CommitTransaction();
			commandManager = null;
		}
	}
	public class SeriesPointFilterModelCollectionAccessor : ISeriesPointFilterCollectionAccessor {
		readonly SeriesPointFilterCollectionModel collection;
		public SeriesPointFilterModelCollectionAccessor(SeriesPointFilterCollectionModel collection) {
			this.collection = collection;
		}
		#region ISeriesPointFilterCollectionAccessor Members
		int ISeriesPointFilterCollectionAccessor.Count { get { return collection.Count; } }
		object ISeriesPointFilterCollectionAccessor.this[int index] { get { return collection[index]; } }
		ConjunctionTypes ISeriesPointFilterCollectionAccessor.ConjunctionMode {
			get { return collection.ConjunctionMode; }
			set { collection.ConjunctionMode = value; }
		}
		int ISeriesPointFilterCollectionAccessor.Add(SeriesPointFilter item) {
			collection.AddNewElement(item);
			return collection.Count - 1; 
		}
		void ISeriesPointFilterCollectionAccessor.RemoveAt(int index) {
			SeriesPointFilterCollection filterCollection = (SeriesPointFilterCollection)collection.ChartCollection;
			SeriesPointFilter filter = filterCollection[index];
			collection.DeleteElement(filter);
		}
		#endregion
	}
}
