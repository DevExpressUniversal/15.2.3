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
using System.Windows.Forms;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class HolidayModelCollectionEditor : ChartEditorBase {
		CommandManager commandManager;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			WorkdaysOptionsModel options = Instance as WorkdaysOptionsModel;
			KnownDateCollectionModel collection = Value as KnownDateCollectionModel;
			if (options == null || collection == null)
				return null;
			commandManager = options.CommandManager;
			commandManager.BeginTransaction();
			bool isHolidays = Object.ReferenceEquals(options.Holidays, collection);
			return new HolidaysCollectionForm(new KnownDateModelCollectionAccessor(collection), ((IOwnedElement)options.ChartElement).ChartContainer, isHolidays);
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			base.AfterShowDialog(form, dialogResult);
			commandManager.CommitTransaction();
			commandManager = null;
		}
	}
	public class KnownDateModelCollectionAccessor : IKnownDateCollectionAccessor {
		readonly KnownDateCollectionModel collection;
		public KnownDateModelCollectionAccessor(KnownDateCollectionModel collection) {
			this.collection = collection;
		}
		#region IKnownDateCollectionAccessor Members
		int IKnownDateCollectionAccessor.Count { get { return collection.ChartCollection.Count; } }
		void IKnownDateCollectionAccessor.Add(KnownDate knownDate) {
			collection.AddNewElement(knownDate);
		}
		void IKnownDateCollectionAccessor.Remove(KnownDate item) {
			collection.DeleteElement(item);
		}
		void IKnownDateCollectionAccessor.AddRange(KnownDate[] items) {
			foreach (KnownDate item in items) {
				collection.AddNewElement(item);
			}
		}
		void IKnownDateCollectionAccessor.Clear() {
			collection.ClearElements();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return collection.ChartCollection.GetEnumerator();
		}
		#endregion
	}
}
