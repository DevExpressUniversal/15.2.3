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
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using DevExpress.Utils.Design;
namespace DevExpress.Utils.Filtering {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public class CustomMetricsAttributeExpression : ICloneable {
		public CustomMetricsAttributeExpression() { }
		public CustomMetricsAttributeExpression(string path, Type type, AttributeInfo[] attributes) {
			this.Path = path;
			this.Type = type;
			this.Attributes = attributes;
		}
		#region ICustomMetricsAttributeExpression Members
		public string Path { get; set; }
		[TypeConverter("DevExpress.Utils.Design.CustomMetricAttributesTypeConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public Type Type { get; set; }
		public AttributeInfo[] Attributes { get; private set; }
		#endregion
		#region ICloneable
		public object Clone() {
			return new CustomMetricsAttributeExpression(Path, Type, Attributes);
		}
		#endregion
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public abstract class AttributeInfo {
		public AttributeInfo() { }
		public Attribute Attribute { get; protected set; }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public class DisplayAttributeInfo : AttributeInfo {
		public DisplayAttributeInfo(string name, string description, bool? autoGenerateFilter) {
			DisplayAttribute attribute = new DisplayAttribute();
			this.Name = attribute.Name = name;
			this.Description = attribute.Description = description;
			if(autoGenerateFilter.HasValue)
				this.AutoGenerateFilter = attribute.AutoGenerateField = autoGenerateFilter.Value;
			base.Attribute = attribute;
		}
		public string Name { get; set; }
		public string Description { get; set; }
		public bool? AutoGenerateFilter { get; set; }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public class DisplayFormatAttributeInfo : AttributeInfo {
		public DisplayFormatAttributeInfo(string dataFormatString) {
			DisplayFormatAttribute attribute = new DisplayFormatAttribute();
			this.DataFormatString = attribute.DataFormatString = dataFormatString;
			base.Attribute = attribute;
		}
		public string DataFormatString { get; set; }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public class FilterRangeAttributeInfo : AttributeInfo { }
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public class FilterLookupAttributeInfo : AttributeInfo { }
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public class FilterBooleanChoiceAttributeInfo : AttributeInfo { }
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public class FilterEnumChoiceAttributeInfo : AttributeInfo { }
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(UniversalTypeConverter))]
	public class AttributeInfoFactory {
		private AttributeInfoFactory() { }
		static AttributeInfoFactory instanceCore;
		public static AttributeInfoFactory Instance {
			get {
				if(instanceCore == null)
					instanceCore = new AttributeInfoFactory();
				return instanceCore;
			}
		}
		public AttributeInfo Create(Attribute attrubute) {
			if(attrubute is DisplayAttribute) {
				DisplayAttribute innerAttribute = attrubute as DisplayAttribute;
				return new DisplayAttributeInfo(innerAttribute.Name, innerAttribute.Description, innerAttribute.GetAutoGenerateFilter());
			}
			if(attrubute is DisplayFormatAttribute) {
				DisplayFormatAttribute innerAttribute = attrubute as DisplayFormatAttribute;
				return new DisplayFormatAttributeInfo(innerAttribute.DataFormatString);
			}
			if(attrubute is FilterRangeAttribute) {
				FilterRangeAttribute innerAttribute = attrubute as FilterRangeAttribute;
				return new FilterRangeAttributeInfo();
			}
			if(attrubute is FilterLookupAttribute) {
				FilterLookupAttribute innerAttribute = attrubute as FilterLookupAttribute;
				return new FilterLookupAttributeInfo();
			}
			if(attrubute is FilterBooleanChoiceAttribute) {
				FilterBooleanChoiceAttribute innerAttribute = attrubute as FilterBooleanChoiceAttribute;
				return new FilterBooleanChoiceAttributeInfo();
			}
			if(attrubute is FilterEnumChoiceAttribute) {
				FilterEnumChoiceAttribute innerAttribute = attrubute as FilterEnumChoiceAttribute;
				return new FilterEnumChoiceAttributeInfo();
			}
			return null;
		}
	}
	[Editor("DevExpress.Utils.Design.DXCollectionEditorBase, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor))]
	public class CustomMetricAttributesCollection : CollectionBase, IEnumerable<CustomMetricsAttributeExpression>, IDisposable {
		public CustomMetricAttributesCollection(FilteringUIContext filteringContext) {
			this.filteringContext = filteringContext;
		}
		FilteringUIContext filteringContext;
		public void Dispose() {
			filteringContext = null;
		}
		public EventHandler<CollectionChangeEventArgs> CollectionChanged;
		public void Add(CustomMetricsAttributeExpression expression) {
			((IList)this).Add(expression);
		}
		public void Remove(CustomMetricsAttributeExpression expression) {
			((IList)this).Remove(expression);
		}
		public void AddRange(CustomMetricsAttributeExpression[] expressions) {
			for(int i = 0; i < expressions.Length; i++)
				((IList)this).Add(expressions[i]);
		}
		public int IndexOf(CustomMetricsAttributeExpression expression) {
			return InnerList.IndexOf(expression);
		}
		public bool Contains(CustomMetricsAttributeExpression expression) {
			return InnerList.Contains(expression);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, oldValue as CustomMetricsAttributeExpression);
			RaiseCollectionChanged(CollectionChangeAction.Add, newValue as CustomMetricsAttributeExpression);
		}
		protected override void OnInsertComplete(int index, object value) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, value as CustomMetricsAttributeExpression);
		}
		protected override void OnRemoveComplete(int index, object value) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, value as CustomMetricsAttributeExpression);
		}
		protected override void OnClearComplete() {
			RaiseCollectionChanged(CollectionChangeAction.Refresh, null);
		}
		public CustomMetricsAttributeExpression this[int index] {
			get { return (CustomMetricsAttributeExpression)InnerList[index]; }
		}
		public CustomMetricsAttributeExpression[] ToArray() {
			CustomMetricsAttributeExpression[] array = new CustomMetricsAttributeExpression[Count];
			InnerList.CopyTo(array, 0);
			return array;
		}
		protected void RaiseCollectionChanged(CollectionChangeAction action, CustomMetricsAttributeExpression expression) {
			var handler = CollectionChanged;
			if(handler != null) handler(this, new CollectionChangeEventArgs(action, expression));
		}
		#region IEnumerable<CustomMetricsAttributeExpression> Members
		IEnumerator<CustomMetricsAttributeExpression> IEnumerable<CustomMetricsAttributeExpression>.GetEnumerator() {
			foreach(CustomMetricsAttributeExpression expression in InnerList)
				yield return expression;
		}
		#endregion
	}
}
