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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class DataFilterModelCollectionEditor : ChartEditorBase {
		CommandManager commandManager;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			DataFilterCollectionModel collection = Value as DataFilterCollectionModel;
			DesignerSeriesModel series = Instance as DesignerSeriesModel;
			if (series == null || collection == null)
				return null;
			commandManager = collection.CommandManager;
			commandManager.BeginTransaction();
			DataFilterCollectionForm form = new DataFilterCollectionForm(new DataFilterModelCollectionAccessor(collection));
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)((IOwnedElement)series.ChartElement).ChartContainer.RenderProvider.LookAndFeel;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			base.AfterShowDialog(form, dialogResult);
			commandManager.CommitTransaction();
			commandManager = null;
		}
	}
	public class DataFilterModelCollectionAccessor : IDataFilterCollectionAccessor {
		readonly DataFilterCollectionModel collection;
		public DataFilterModelCollectionAccessor(DataFilterCollectionModel collection) {
			this.collection = collection;
		}
		#region IDataFilterCollectionAccessor Members
		ConjunctionTypes IDataFilterCollectionAccessor.ConjunctionMode {
			get { return collection.ConjunctionMode; }
			set { collection.ConjunctionMode = value; }
		}
		object IDataFilterCollectionAccessor.this[int index] { get { return collection[index]; } }
		int IDataFilterCollectionAccessor.Count { get { return collection.Count; } }
		void IDataFilterCollectionAccessor.RemoveAt(int index) {
			DataFilterCollection filterCollection = (DataFilterCollection)collection.ChartCollection;
			DataFilter filter = filterCollection[index];
			collection.DeleteElement(filter);
		}
		int IDataFilterCollectionAccessor.Add(DataFilter item) {
			collection.AddNewElement(item);
			return collection.Count - 1;
		}
		#endregion
	}
}
