#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public interface IFormatRuleCollection : IXmlSerializableElement, IEnumerable<DashboardItemFormatRule> {
		DashboardItemFormatRule this[int index] { get; }
		int Count { get; }
		event EventHandler Changed;
		DashboardItemFormatRule CreateRule();
		void BeginUpdate();
		void EndUpdate();
		void Add(DashboardItemFormatRule rule);
		void Insert(int index, DashboardItemFormatRule rule);
		int IndexOf(DashboardItemFormatRule rule);
		int IndexOfStyleSettings(IStyleSettings styleSettings);
		void Remove(DashboardItemFormatRule rule);
		void RemoveAt(int index);
		void Clear();
		IFormatRuleCollection Clone();
		bool Move(int oldIndex, int newIndex);
		void OnEndLoading();
		ConditionalFormattingModel CreateViewModel();
	}
}
namespace DevExpress.DashboardCommon {
	public abstract class FormatRuleCollection<T> : NamedItemCollection<T>, IFormatRuleCollection where T : DashboardItemFormatRule, new() {
		readonly CollectionPrefixNameGenerator<T> nameGenerator;
		event EventHandler Changed;
		protected FormatRuleCollection() {
			nameGenerator = new CollectionPrefixNameGenerator<T>(this);
			XmlSerializer = new RepositoryItemListXmlSerializerBase<IXmlSerializableElement, T>("FormatRules", XmlRepository.ConditionalFormattingRepository);
			CollectionChanged += (sender, e) => { 
				if(Changed != null)
					Changed(this, EventArgs.Empty);
			};
		}
		protected abstract IFormatRuleCollection CreateInstance();
		protected override string GetName(T item) {
			return item.Name;
		}
		protected internal virtual void OnEndLoading() {
			foreach(T rule in this) {
				rule.OnEndLoading();
			}
		}
		int IterateStyles(Func<IStyleSettings, int, bool> func) {
			int index = -1;
			int ruleIndex = -1;
			foreach(T rule in this) {
				if(rule.Checked) {
					ruleIndex++;
					IFormatCondition condition = rule.Condition;
					foreach(IStyleSettings style in condition.Styles) {
						if (style != null) {
							index++;
							if (!func(style, ruleIndex))
								return index;
						}
					}
				}
			}
			return -1;
		}
		#region IFormatRuleCollection Members
		void IFormatRuleCollection.BeginUpdate() {
			this.BeginUpdate();
		}
		void IFormatRuleCollection.EndUpdate() {
			this.EndUpdate();
		}
		DashboardItemFormatRule IFormatRuleCollection.this[int index] {
			get { return this[index]; }
		}
		int IFormatRuleCollection.Count {
			get { return this.Count; }
		}
		event EventHandler IFormatRuleCollection.Changed { add { Changed += value; } remove { Changed -= value; } }
		DashboardItemFormatRule IFormatRuleCollection.CreateRule() {
			return new T();
		}
		void IFormatRuleCollection.Add(DashboardItemFormatRule rule) {
			this.Add((T)rule);
		}
		void IFormatRuleCollection.Insert(int index, DashboardItemFormatRule rule) {
			this.Insert(index, (T)rule);
		}
		int IFormatRuleCollection.IndexOf(DashboardItemFormatRule rule) {
			return this.IndexOf((T)rule);
		}
		void IFormatRuleCollection.Remove(DashboardItemFormatRule rule) {
			this.Remove((T)rule);
		}
		void IFormatRuleCollection.RemoveAt(int index) {
			this.RemoveAt(index);
		}
		void IFormatRuleCollection.Clear() {
			this.ClearItems();
		}
		int IFormatRuleCollection.IndexOfStyleSettings(IStyleSettings styleSettings) {
			if(styleSettings == null) return -1;
			return IterateStyles((style, ruleIndex) => {
				return !object.Equals(style, styleSettings);
			});
		}
		IFormatRuleCollection IFormatRuleCollection.Clone() {
			IFormatRuleCollection clone = CreateInstance();
			foreach(T rule in this)
				clone.Add((T)rule.Clone());
			return clone;
		}
		bool IFormatRuleCollection.Move(int oldIndex, int newIndex) {
			if(oldIndex != newIndex && newIndex >= 0 && newIndex < Count) {
				T rule = this[oldIndex];
				RemoveAt(oldIndex);
				Insert(newIndex, rule);
				if(rule.Context != null)
					rule.Context.OnChanged(null);
				return true;
			} else {
				return false;
			}
		}
		void IFormatRuleCollection.OnEndLoading() {
			this.OnEndLoading();
		}
		ConditionalFormattingModel IFormatRuleCollection.CreateViewModel() {
			ConditionalFormattingModel viewModel = new ConditionalFormattingModel();
			List<StyleSettingsModel> styles = new List<StyleSettingsModel>();
			IList<FormatRuleModelBase> ruleModels = new List<FormatRuleModelBase>();
			foreach(T rule in this) {
				if(rule.Checked)
					ruleModels.Add(rule.CreateModel());
			}
			IterateStyles((style, ruleIndex) => {
				StyleSettingsModel styleModel = style.CreateViewModel();
				styleModel.RuleIndex = ruleIndex;
				styles.Add(styleModel);
				return true;
			});
			return new ConditionalFormattingModel() {
				FormatConditionStyleSettings = styles,
				RuleModels = ruleModels
			};
		}
		#endregion
		#region IEnumerable<DashboardItemFormatRule> Members
		IEnumerator<DashboardItemFormatRule> IEnumerable<DashboardItemFormatRule>.GetEnumerator() {
			return this.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
		#endregion
		#region IXmlSerializableElement Members
		void IXmlSerializableElement.SaveToXml(XElement element) {
			SaveToXml(element);
		}
		void IXmlSerializableElement.LoadFromXml(XElement element) {
			LoadFromXml(element);
		}
		#endregion
	}
}
