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

using System.Collections.Generic;
using System.Windows;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core.Native;
using System;
using System.Linq;
#if !SILVERLIGHT
using System.ComponentModel;
#else
using DevExpress.Data.Browsing;
#endif
#if SILVERLIGHT
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
#endif
namespace DevExpress.Xpf.Core.Serialization {
	public enum ResetPropertyMode { 
		Auto,
		SetDefaultValue,
		ClearValue,
		ClearCollection,
		None,
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class XtraResetPropertyAttribute : Attribute {
		internal static readonly XtraResetPropertyAttribute DefaultInstance = new XtraResetPropertyAttribute();
		public XtraResetPropertyAttribute() :
			this(ResetPropertyMode.Auto) {
		}
		public XtraResetPropertyAttribute(ResetPropertyMode propertyResetType) {
			this.PropertyResetType = propertyResetType;
		}
		public ResetPropertyMode PropertyResetType { get; private set; }
	}
	public class StartDeserializingEventArgs : RoutedEventArgs {
		internal StartDeserializingEventArgs(object source, LayoutAllowEventArgs args)
			: base(DXSerializer.StartDeserializingEvent, source) {
			Args = args;
		}
		public LayoutAllowEventArgs Args { get; private set; }
	}
	public class EndDeserializingEventArgs : RoutedEventArgs {
		internal EndDeserializingEventArgs(object source, string restoredVersion)
			: base(DXSerializer.EndDeserializingEvent, source) {
			RestoredVersion = restoredVersion;
		}
		public string RestoredVersion { get; private set; }
	}
	public class DXSerializable {
		protected internal virtual XtraSerializableProperty CreateXtraSerializableAttrubute() {
			return new XtraSerializableProperty();
		}
	}
	public class DXSerializableContent : DXSerializable {
		protected internal override XtraSerializableProperty CreateXtraSerializableAttrubute() {
			return new XtraSerializableProperty(XtraSerializationVisibility.Content);
		}
	}
	public class DXSerializableCollection : DXSerializable {
		bool useCreateItem;
		bool useFindItem;
		bool clearCollection;
		public DXSerializableCollection(bool useCreateItem, bool useFindItem, bool clearCollection) {
			this.useCreateItem = useCreateItem;
			this.useFindItem = useFindItem;
			this.clearCollection = clearCollection;
		}
		protected internal override XtraSerializableProperty CreateXtraSerializableAttrubute() {
			return new XtraSerializableProperty(XtraSerializationVisibility.Collection, useCreateItem, useFindItem, clearCollection);
		}
	}
	public class CustomGetSerializablePropertiesEventArgs : RoutedEventArgs {
		readonly List<SerializablePropertyDescriptorPair> pairsList;
		readonly PropertyDescriptorCollection props;
		internal CustomGetSerializablePropertiesEventArgs(object source, List<SerializablePropertyDescriptorPair> pairsList, PropertyDescriptorCollection props)
			: base(DXSerializer.CustomGetSerializablePropertiesEvent, source) {
			this.pairsList = pairsList;
			this.props = props;
		}
		public void SetPropertySerializable(string propertyName, DXSerializable serializable) {
			SerializablePropertyDescriptorPair existingPair = FindExistingPair(propertyName);
			if(existingPair != null)
				pairsList.Remove(existingPair);
			PropertyDescriptor property = props[propertyName];
			if(serializable != null && property != null) {
				pairsList.Add(new SerializablePropertyDescriptorPair(property, serializable.CreateXtraSerializableAttrubute()));
			}
		}
#if !SILVERLIGHT
		public void SetPropertySerializable(DependencyProperty property, DXSerializable serializable) {
			SetPropertySerializable(property.Name, serializable);
		}
#endif
		SerializablePropertyDescriptorPair FindExistingPair(string propertyName) {
			return pairsList.Find((pair) => (pair.Property.Name == propertyName));
		}
	}
	public class CustomGetSerializableChildrenEventArgs : RoutedEventArgs {
		internal CustomGetSerializableChildrenEventArgs(object source)
			: base(DXSerializer.CustomGetSerializableChildrenEvent, source) {
			Children = new List<DependencyObject>();
		}
		public IList<DependencyObject> Children { get; private set; }
	}
	public class XtraPropertyInfoEventArgs : PropertyEventArgs {
		readonly XtraPropertyInfo info;
		public XtraPropertyInfoEventArgs(RoutedEvent routedEvent, object source, PropertyDescriptor property, XtraPropertyInfo info)
			: base(property, source, routedEvent) {
			this.info = info;
		}
		public XtraPropertyInfo Info { get { return info; } }
	}
	public class XtraItemRoutedEventArgs : RoutedEventArgs {
		readonly XtraItemEventArgs e;
		public XtraItemRoutedEventArgs(RoutedEvent routedEvent, object source, XtraItemEventArgs e)
			: base(routedEvent, source) {
			this.e = e;
		}
		public object Collection { get { return e.Collection; } }
		public XtraPropertyInfo Item { get { return e.Item; } }
		public OptionsLayoutBase Options { get { return e.Options; } }
		public object Owner { get { return e.Owner; } }
		public object RootObject { get { return e.RootObject; } }
		public int Index { get { return e.Index; } }
	}
	public class XtraClearCollectionEventArgs : XtraItemRoutedEventArgs {
		public XtraClearCollectionEventArgs(object source, XtraItemEventArgs e)
			: base(DXSerializer.ClearCollectionEvent, source, e) {
		}
	}
	public class XtraCreateCollectionItemEventArgs : XtraItemRoutedEventArgs {
		public XtraCreateCollectionItemEventArgs(object source, string propertyName, XtraItemEventArgs e)
			: base(DXSerializer.CreateCollectionItemEvent, source, e) {
			CollectionName = propertyName;
		}
		public object CollectionItem { get; set; }
		public string CollectionName { get; private set; }
	}
	public class XtraFindCollectionItemEventArgs : XtraItemRoutedEventArgs {
		public XtraFindCollectionItemEventArgs(object source, string propertyName, XtraItemEventArgs e)
			: base(DXSerializer.FindCollectionItemEvent, source, e) {
			CollectionName = propertyName;
		}
		public object CollectionItem { get; set; }
		public string CollectionName { get; private set; }
	}
	public class XtraShouldSerailizeCollectionItemEventArgs : XtraItemRoutedEventArgs {
		public XtraShouldSerailizeCollectionItemEventArgs(object source, XtraItemEventArgs e, object item)
			: base(DXSerializer.ShouldSerializeCollectionItemEvent, source, e) {
			ShouldSerailize = true;
			Value = item;
		}
		public bool ShouldSerailize { get; set; }
		public object Value { get; set; }
	}
	public class XtraCreateContentPropertyValueEventArgs : XtraItemRoutedEventArgs {
		public XtraCreateContentPropertyValueEventArgs(object source, PropertyDescriptor propertyDescriptor, XtraItemEventArgs e)
			: base(DXSerializer.CreateContentPropertyValueEvent, source, e) {
				PropertyDescriptor = propertyDescriptor;
		}
		public object PropertyValue { get; set; }
		public PropertyDescriptor PropertyDescriptor { get; private set; }
	}
	public abstract class PropertyEventArgs : RoutedEventArgs {
		public PropertyEventArgs(PropertyDescriptor property, object source, RoutedEvent routedEvent)
			: base(routedEvent, source) {
			Property = property;
		}
		DependencyPropertyDescriptor dependencyPropertyDescriptor;
		protected DependencyPropertyDescriptor GetDependencyPropertyDescriptor() {
			if(dependencyPropertyDescriptor == null)
				dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(Property);
			return dependencyPropertyDescriptor;
		}
		public PropertyDescriptor Property { get; private set; }
		public DependencyProperty DependencyProperty { 
			get {
				var descriptor = GetDependencyPropertyDescriptor();
				return (descriptor != null) ? descriptor.DependencyProperty : null; 
			} 
		}
	}
	public class AllowPropertyEventArgs : PropertyEventArgs {
		public AllowPropertyEventArgs(int id, bool isSerializing, PropertyDescriptor property, object source)
			: base(property, source, DXSerializer.AllowPropertyEvent) {
			Allow = true;
			PropertyId = id;
			IsSerializing = isSerializing;
		}
		public int PropertyId { get; private set; }
		public bool IsSerializing { get; private set; }
		public bool Allow { get; set; }
	}
	public class CustomShouldSerializePropertyEventArgs : PropertyEventArgs {
		public CustomShouldSerializePropertyEventArgs(PropertyDescriptor property, object source)
			: base(property, source, DXSerializer.CustomShouldSerializePropertyEvent) {
			CustomShouldSerialize = null;
		}
		public bool? CustomShouldSerialize { get; set; }
	}
	public class ResetPropertyEventArgs : PropertyEventArgs {
		public ResetPropertyEventArgs(ResetPropertyMode resetPropertyMode, PropertyDescriptor property, object source)
			: base(property, source, DXSerializer.ResetPropertyEvent) {
			this.ResetPropertyMode = resetPropertyMode;
		}
		public ResetPropertyMode ResetPropertyMode { get; set; }
	}
	public class BeforeLoadLayoutEventArgs : RoutedEventArgs {
		public BeforeLoadLayoutEventArgs(object source, string restoredVersion)
			: base(DXSerializer.BeforeLoadLayoutEvent, source) {
			RestoredVersion = restoredVersion;
			Allow = true;
		}
		public string RestoredVersion { get; private set; }
		public bool Allow { get; set; }
	}
	public class LayoutUpgradeEventArgs : RoutedEventArgs {
		public LayoutUpgradeEventArgs(object source, string restoredVersion)
			: base(DXSerializer.LayoutUpgradeEvent, source) {
			RestoredVersion = restoredVersion;
		}
		public string RestoredVersion { get; private set; }
	}
	public delegate void BeforeLoadLayoutEventHandler(
		object sender, BeforeLoadLayoutEventArgs e
	);
	public delegate void LayoutUpgradeEventHandler(
		object sender, LayoutUpgradeEventArgs e
	);
	public delegate void CustomGetSerializablePropertiesEventHandler(
		object sender, CustomGetSerializablePropertiesEventArgs e
	);
	public delegate void CustomGetSerializableChildrenEventHandler(
		object sender, CustomGetSerializableChildrenEventArgs e
	);
	public delegate void XtraPropertyInfoEventHandler(
		object sender, XtraPropertyInfoEventArgs e
	);
	public delegate void XtraItemRoutedEventHandler(
		object sender, XtraItemRoutedEventArgs e
	);
	public delegate void XtraCreateCollectionItemEventHandler(
		object sender, XtraCreateCollectionItemEventArgs e
	);
	public delegate void XtraFindCollectionItemEventHandler(
		object sender, XtraFindCollectionItemEventArgs e
	);
	public delegate void AllowPropertyEventHandler(
		object sender, AllowPropertyEventArgs e
	);
	public delegate void CustomShouldSerializePropertyEventHandler(
		object sender, CustomShouldSerializePropertyEventArgs e
	);
	public delegate void ResetPropertyEventHandler(
		object sender, ResetPropertyEventArgs e
	);
	public delegate void XtraShouldSerializeCollectionItemEventHandler(
		object sender, XtraShouldSerailizeCollectionItemEventArgs e
	);
	public delegate void XtraCreateContentPropertyValueEventHandler(
		object sender, XtraCreateContentPropertyValueEventArgs e
	);
	public delegate void StartDeserializingEventHandler(
		object sender, StartDeserializingEventArgs e
	);
	public delegate void EndDeserializingEventHandler(
		object sender, EndDeserializingEventArgs e
	);
}
