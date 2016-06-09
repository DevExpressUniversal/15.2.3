#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Data.Summary;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Utils {
	public class XafSummaryItemsSerializer : SummaryItemsSerializer {
		public const string EmptyGroupSummary = "Empty";
		public XafSummaryItemsSerializer(ISummaryItemsOwner itemsOwner) : base(itemsOwner) { }
		public override void Deserialize(string data) {
			string groupSummary = GetGroupSummary(data);
			if(string.Compare(groupSummary, EmptyGroupSummary, true) == 0) return;
			try {
				base.Deserialize(groupSummary);
			}
			catch(Exception exception) {
				Tracing.Tracer.LogError(exception);
			}
		}
		protected virtual string GetGroupSummary(string groupSummary) {
			if(string.IsNullOrEmpty(groupSummary)) {
				return SummaryItemType.Count.ToString();
			}
			return groupSummary;
		}
		public override string Serialize() {
			if(ItemsOwner.GetItems().Count == 0) return EmptyGroupSummary;
			if(IsCountSummaryItemOnly()) return string.Empty;
			return base.Serialize();
		}
		bool IsCountSummaryItemOnly() {
			if(ItemsOwner.GetItems().Count == 1) {
				ISummaryItem item = ItemsOwner.GetItems()[0];
				if(item.SummaryType == SummaryItemType.Count && string.IsNullOrEmpty(item.FieldName) &&
					string.IsNullOrEmpty(item.DisplayFormat)) return true;
			}
			return false;
		}
	}
	public class SummaryModelSynchronizer<T, V> : ModelSynchronizer<T, V>
		where T : ISummaryItemsOwner
		where V : IModelListView {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool UseColumnDisplayFormatForSummaryItem = false;
		public SummaryModelSynchronizer(T summaryItemsOwner, V model)
			: base(summaryItemsOwner, model) {
		}
		protected override void ApplyModelCore() {
			DeserializeGroupSummary();
			if(UseColumnDisplayFormatForSummaryItem) {
				ApplyColumnDisplayFormatToGroupSummary();
			}
			RemoveGroupSummaryForProtectedColumns();
		}
		protected virtual void ApplyColumnDisplayFormatToGroupSummary() { }
		public override void SynchronizeModel() {
			SerializeGroupSummary();
		}
		protected virtual void RemoveGroupSummaryForProtectedColumns() {
		}
		private void DeserializeGroupSummary() {
			CreateSummaryItemsSerializer(Control).Deserialize(((IModelListView)Model).GroupSummary);
		}
		private void SerializeGroupSummary() {
			string data = CreateSummaryItemsSerializer(Control).Serialize();
			((IModelListView)Model).GroupSummary = !String.IsNullOrEmpty(data) ? data : null;
		}
		protected virtual SummaryItemsSerializer CreateSummaryItemsSerializer(ISummaryItemsOwner itemsOwner) {
			return new XafSummaryItemsSerializer(itemsOwner);
		}
	}
}
