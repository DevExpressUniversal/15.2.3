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
using System.Collections;
using DevExpress.Data;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListSummarySettings {
		public static readonly DependencyProperty IsRecursiveProperty;
		static TreeListSummarySettings() {
			IsRecursiveProperty = DependencyPropertyManager.RegisterAttached("IsRecursive", typeof(bool), typeof(TreeListSummarySettings), new FrameworkPropertyMetadata(true));
		}
		public static bool GetIsRecursive(DependencyObject d) {
			return (bool)d.GetValue(TreeListSummarySettings.IsRecursiveProperty);
		}
		public static void SetIsRecursive(DependencyObject d, bool value) {
			d.SetValue(TreeListSummarySettings.IsRecursiveProperty, value);
		}
	}
	public abstract class TreeListSummaryValue {
		protected readonly object StartValue = new object();
		SummaryItemBase summaryItem;
		public TreeListSummaryValue(SummaryItemBase summaryItem) {
			this.summaryItem = summaryItem;
		}
		protected SummaryItemBase SummaryItem { get { return summaryItem; } }
		protected string FieldName { get { return SummaryItem.FieldName; } }
		public abstract object Value { get; }
		public virtual void Start(TreeListNode node) { }
		public virtual void Finish(TreeListNode node) { }
		public abstract void Calculate(TreeListNode node, bool summariesIgnoreNullValues);
		protected object GetNodeValue(TreeListNode node) {
			return node.DataProvider.GetNodeValue(node, FieldName);
		}
	}
	public class TreeListSummaryCountValue : TreeListSummaryValue {
		int count;
		public TreeListSummaryCountValue(SummaryItemBase summaryItem)
			: base(summaryItem) {
		}
		public override object Value { get { return count; } }
		public override void Start(TreeListNode node) {
			this.count = 0;
		}
		public override void Calculate(TreeListNode node, bool summariesIgnoreNullValues) {
			this.count++;
		}
	}
	public class TreeListSummaryMinValue : TreeListSummaryValue {
		object min;
		public TreeListSummaryMinValue(SummaryItemBase summaryItem)
			: base(summaryItem) {
				min = StartValue;
		}
		public override object Value { get { return min == StartValue ? null : min; } }
		public override void Calculate(TreeListNode node, bool summariesIgnoreNullValues) {
			object value = GetNodeValue(node);
			if(summariesIgnoreNullValues && value == null)
				return;
			if(min == StartValue || node.DataProvider.ValueComparer.Compare(value, Value) < 0)
				this.min = value;
		}
	}
	public class TreeListSummaryMaxValue : TreeListSummaryValue {
		object max;
		public TreeListSummaryMaxValue(SummaryItemBase summaryItem)
			: base(summaryItem) {
				max = StartValue;
		}
		public override object Value { get { return max == StartValue ? null : max; } }
		public override void Calculate(TreeListNode node, bool summariesIgnoreNullValues) {
			object value = GetNodeValue(node);
			if(summariesIgnoreNullValues && value == null)
				return;
			if(max == StartValue || node.DataProvider.ValueComparer.Compare(value, Value) > 0)
				this.max = value;
		}
	}
	public class TreeListSummarySumValue : TreeListSummaryValue {
		decimal sum;
		public TreeListSummarySumValue(SummaryItemBase summaryItem)
			: base(summaryItem) {
		}
		protected decimal Sum { get { return sum; } set { sum = value; } }
		public override object Value { get { return sum; } }
		public override void Calculate(TreeListNode node, bool summariesIgnoreNullValues) {
			object value = GetNodeValue(node);
			try {
				this.sum += Convert.ToDecimal(value);
			}
			catch {
			}
		}
	}
	public class TreeListSummaryAvgValue : TreeListSummarySumValue {
		int count;
		public TreeListSummaryAvgValue(SummaryItemBase summaryItem)
			: base(summaryItem) {
		}
		public override void Start(TreeListNode node) {
			base.Start(node);
			this.count = 0;
		}
		public override void Calculate(TreeListNode node, bool summariesIgnoreNullValues) {
			base.Calculate(node, summariesIgnoreNullValues);
			this.count++;
		}
		public override void Finish(TreeListNode node) {
			Sum = count == 0 ? 0 : Sum / count;
		}
	}
	public class TreeListSummaryCustomValue : TreeListSummaryValue {
		TreeListView view;
		object value;
		public TreeListSummaryCustomValue(SummaryItemBase summaryItem, TreeListView view)
			: base(summaryItem) {
			this.view = view;
		}
		public override object Value { get { return value; } }
		protected TreeListView View { get { return view; } }
		public override void Start(TreeListNode node) {
			RaiseEvent(node, CustomSummaryProcess.Start); 
		}
		public override void Calculate(TreeListNode node, bool summariesIgnoreNullValues) {
			RaiseEvent(node, CustomSummaryProcess.Calculate);
		}
		public override void Finish(TreeListNode node) {
			RaiseEvent(node, CustomSummaryProcess.Finalize);
		}
		void RaiseEvent(TreeListNode node, CustomSummaryProcess process) {
			TreeListCustomSummaryEventArgs e = new TreeListCustomSummaryEventArgs(node, SummaryItem, process, view.GetNodeValue(node, FieldName));
			e.TotalValue = Value;
			view.RaiseCustomSummary(e);
			this.value = e.TotalValue;
		}
	}
	public class TreeListSummarySortedList : TreeListSummaryValue {
		List<TreeListNode> nodes;
		SortedIndices result;
		public TreeListSummarySortedList(ServiceSummaryItem item) : base(item) {
			this.nodes = new List<TreeListNode>();
		}
		public override object Value {
			get { return result; }
		}
		public override void Calculate(TreeListNode node, bool summariesIgnoreNullValues) {
			object value = GetNodeValue(node);
			if(summariesIgnoreNullValues && value == null) return;
			nodes.Add(node);
		}
		public override void Finish(TreeListNode node) {
			int[] rowHandles = nodes.OrderBy((n) => GetNodeValue(n)).Select((n) => n.RowHandle).ToArray<int>();
			this.result = new SortedIndices(rowHandles);
		}
	}
	public class TreeListSummaryDateTimeAvarage : TreeListSummaryValue {
		Tuple<decimal, int> current;
		object result;
		public TreeListSummaryDateTimeAvarage(ServiceSummaryItem item) : base(item) { }
		public override object Value {
			get { return result; }
		}
		public override void Start(TreeListNode node) {
			this.current = new Tuple<decimal, int>((decimal)0, 0);
		}
		public override void Calculate(TreeListNode node, bool summariesIgnoreNullValues) {
			object value = GetNodeValue(node);
			this.current = new Tuple<decimal, int>(current.Item1 + (decimal)((DateTime)value).Ticks, current.Item2 + 1);
		}
		public override void Finish(TreeListNode node) {
			var total = (Tuple<decimal, int>)current;
			this.result = total.Item1 / total.Item2;
		}
	}
}
