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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[ListBindable(BindableSupport.No)]
	[TypeConverter(typeof(CollectionTypeConverter))]
	[Editor("DevExpress.XtraReports.Design.FormattingRuleSheetEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor))]
	public class FormattingRuleSheet : Collection<FormattingRule>, IDisposable {
		#region Fields & Properties
		readonly XtraReport report;
		public FormattingRule this[string name] {
			get { return this.FirstOrDefault(rule => rule.Name == name); }
		}
		internal XtraReport Report {
			get { return report; }
		}
		#endregion
		#region Constructors
		public FormattingRuleSheet(XtraReport report) {
			this.report = report;
		}
		#endregion
		#region Methods
		public void Dispose() {
			for(int i = this.Count - 1; i >= 0; i--)
				this[i].Dispose();
			Clear();
		}
		public void AddRange(FormattingRule[] items) {
			foreach(FormattingRule item in items)
				Add(item);
		}
		internal void CopyFrom(FormattingRuleSheet source) {
			FormattingRule[] items = new FormattingRule[source.Count];
			source.CopyTo(items, 0);
			Clear();
			AddRange(items);
		}
		internal string[] GetNames() {
			List<string> names = new List<string>();
			foreach(FormattingRule rule in this)
				names.Add(rule.Name);
			return names.ToArray();
		}
		protected override void ClearItems() {
			foreach(FormattingRule rule in this)
				UnsubscribeRuleEvent(rule);
			base.ClearItems();
		}
		protected override void InsertItem(int index, FormattingRule item) {
			if(item != null && !Contains(item)) {
				if(report != null)
					item.Formatting.SyncDpi(report.Dpi);
				base.InsertItem(index, item);
				item.Owner = this;
				SubscribeRuleEvent(item);
			}
		}
		protected override void RemoveItem(int index) {
			FormattingRule rule = this[index];
			base.RemoveItem(index);
			UnsubscribeRuleEvent(rule);
			rule.Dispose();
		}
		void OnRuleDisposed(object sender, EventArgs e) {
			FormattingRule rule = (FormattingRule)sender;
			if(rule != null && Contains(rule)) {
				Remove(rule);
				UnsubscribeRuleEvent(rule);
			}
		}
		void SubscribeRuleEvent(FormattingRule rule) {
			rule.Disposed += new EventHandler(OnRuleDisposed);
		}
		void UnsubscribeRuleEvent(FormattingRule rule) {
			rule.Disposed -= new EventHandler(OnRuleDisposed);
		}
		#endregion
	}
}
