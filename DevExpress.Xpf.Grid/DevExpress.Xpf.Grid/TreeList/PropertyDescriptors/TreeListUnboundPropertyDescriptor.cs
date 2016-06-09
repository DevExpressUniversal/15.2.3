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
using System.ComponentModel;
using System.Collections;
using DevExpress.Data;
using DevExpress.Data.Filtering.Helpers;
using System.Threading;
using DevExpress.Data.Filtering;
using System.Diagnostics;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListUnboundPropertyDescriptor : PropertyDescriptor {
		UnboundColumnInfo unboundInfo;
		TreeListDataProvider provider;
		ExpressionEvaluator evaluator;
		Type dataType;
		Exception evaluatorCreateException = null;
		protected internal TreeListUnboundPropertyDescriptor(TreeListDataProvider provider, UnboundColumnInfo unboundInfo)
			: base(unboundInfo.Name, null) {
			this.evaluator = null;
			this.provider = provider;
			this.unboundInfo = unboundInfo;
			this.dataType = UnboundInfo.DataType;
		}
		public override bool IsBrowsable { get { return unboundInfo.Visible; } }
		protected virtual ExpressionEvaluator CreateEvaluator() {
			return DataProvider.CreateExpressionEvaluator(UnboundInfo.Expression, out evaluatorCreateException);
		}
		protected ExpressionEvaluator Evaluator {
			get {
				if(evaluator == null)
					evaluator = CreateEvaluator();
				return evaluator;
			}
		}
		protected TreeListDataProvider DataProvider { get { return provider; } }
		public UnboundColumnInfo UnboundInfo { get { return unboundInfo; } }
		public override bool IsReadOnly { get { return UnboundInfo.ReadOnly; } }
		public override string Category { get { return string.Empty; } }
		public override Type PropertyType { get { return UnboundInfo.DataType; } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override bool ShouldSerializeValue(object component) { return false; }
		bool RequireValueConversion { get { return UnboundInfo.RequireValueConversion; } }
		public override object GetValue(object component) {
			object value = null;
			if(Evaluator != null) {
				value = GetEvaluatorValue((TreeListNode)component);
			}
			else {
				if(this.evaluatorCreateException != null) value = UnboundErrorObject.Value;
			}
			return DataProvider.GetUnboundData(component, Name, value);
		}
		public override void SetValue(object component, object value) {
			DataProvider.SetUnboundData(component, Name, value);
			DataProvider.OnNodeCollectionChanged((TreeListNode)component, NodeChangeType.Content);
		}
		protected virtual object GetEvaluatorValue(TreeListNode node) {
			object res = null;
			try {
				res = Convert(Evaluator.Evaluate(node));
			}
			catch {
				return UnboundErrorObject.Value;
			}
			return res;
		}
		protected object Convert(object value) {
			if(!RequireValueConversion || value == null) return value;
			if(IsErrorValue(value)) return value;
			try {
				Type type = value.GetType();
				if(type.Equals(dataType)) return value;
				return System.Convert.ChangeType(value, dataType, Thread.CurrentThread.CurrentCulture);
			}
			catch {
			}
			return null;
		}
		public static bool IsErrorValue(object value) {
			return object.ReferenceEquals(value, UnboundErrorObject.Value);
		}
	}
	public class TreeListDisplayTextPropertyDescriptor : PropertyDescriptor {
		TreeListDataProvider dataProvider;
		public TreeListDisplayTextPropertyDescriptor(TreeListDataProvider dataProvider, string name)
			: base(name, null) {
			this.dataProvider = dataProvider;
		}
		public TreeListDataProvider DataProvider { get { return dataProvider; } }
		public override bool IsReadOnly { get { return true; } }
		public override string Category { get { return string.Empty; } }
		public override Type PropertyType { get { return typeof(string); } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			return GetValueCore((TreeListNode)component);
		}
		protected virtual object GetValueCore(TreeListNode node) {
			return DataProvider.View.GetNodeDisplayText(node, Name, DataProvider.View.GetNodeValue(node, Name));
		}
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	public class TreeListSearchDisplayTextPropertyDescriptor : TreeListDisplayTextPropertyDescriptor {
		string originalName;
		public TreeListSearchDisplayTextPropertyDescriptor(TreeListDataProvider provider, string name)
			: base(provider, AddPrefix(name)) {
			originalName = name;
		}
		protected override object GetValueCore(TreeListNode node) {
			return DataProvider.View.GetNodeDisplayText(node, originalName, DataProvider.View.GetNodeValue(node, originalName));
		}
		static string AddPrefix(string name) {
			return String.Concat(DxFtsContainsHelper.DxFtsPropertyPrefix, name);
		}
	}
}
