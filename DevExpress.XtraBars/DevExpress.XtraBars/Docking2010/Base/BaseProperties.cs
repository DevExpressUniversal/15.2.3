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
using System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Docking2010.Base {
	public abstract class BaseProperties : DevExpress.Utils.Base.BaseProperties {
		protected override bool AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			if(IsSerializing)
				return !IsDefault(propertyName) || IsContentProperty(propertyName);
			Views.OptionsLayout opt = options as Views.OptionsLayout;
			return (opt == null) || opt.CanRestoreProperty(propertyName, id);
		}
		protected override void ResetProperties(OptionsLayoutBase options) {
			if(IsDeserializing) {
				Views.OptionsLayout opt = options as Views.OptionsLayout;
				if(opt == null || opt.RestoreAllProperties) {
					ResetCore();
					return;
				}
				if(opt.RestoreUIProperties)
					ResetUIProperties();
			}
		}
		protected void ResetUIProperties() {
			if(propertyBag.Count == 0) return;
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
			DictionaryEntry[] entries = new DictionaryEntry[propertyBag.Count];
			propertyBag.CopyTo(entries, 0);
			for(int i = 0; i < entries.Length; i++) {
				if(!Views.OptionsLayout.IsUIProperty((string)entries[i].Key, props)) continue;
				propertyBag.Remove(entries[i].Key);
			}
		}
	}
	public abstract class BaseDefaultProperties : DevExpress.Utils.Base.BaseDefaultProperties {
		public BaseDefaultProperties(DevExpress.Utils.Base.IBaseProperties parentProperties) : base(parentProperties) { }
		protected override bool AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			if(IsSerializing)
				return !IsDefault(propertyName) || IsContentProperty(propertyName);
			Views.OptionsLayout opt = options as Views.OptionsLayout;
			return (opt == null) || opt.CanRestoreProperty(propertyName, id);
		}
		protected override void ResetProperties(OptionsLayoutBase options) {
			if(IsDeserializing) {
				Views.OptionsLayout opt = options as Views.OptionsLayout;
				if(opt == null || opt.RestoreAllProperties) {
					ResetCore();
					return;
				}
				if(opt.RestoreUIProperties)
					ResetUIProperties();
			}
		}
		protected void ResetUIProperties() {
			if(propertyBag.Count == 0) return;
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
			DictionaryEntry[] entries = new DictionaryEntry[propertyBag.Count];
			propertyBag.CopyTo(entries, 0);
			for(int i = 0; i < entries.Length; i++) {
				if(!Views.OptionsLayout.IsUIProperty((string)entries[i].Key, props)) continue;
				propertyBag.Remove(entries[i].Key);
			}
		}
	}
}
#if DEBUGTEST
namespace DevExpress.XtraBars.Docking2010.Tests {
	using System.Text;
	using NUnit.Framework;
	using DevExpress.Utils.Base;
	using DevExpress.Utils.Design;
	interface ITestProperties : IBaseProperties {
		int IntProperty { get; set; }
		int IntProperty100 { get; set; }
		bool BooleanTrueProperty { get; set; }
		bool BooleanFalseProperty { get; set; }
	}
	interface ITestDefaultProperties : IBaseDefaultProperties {
		int? IntProperty { get; set; }
		int? IntProperty100 { get; set; }
		DefaultBoolean BooleanTrueProperty { get; set; }
		DefaultBoolean BooleanFalseProperty { get; set; }
		int ActualIntProperty { get; }
		int ActualIntProperty100 { get; }
		bool ActualBooleanTrueProperty { get; }
		bool ActualBooleanFalseProperty { get; }
	}
	static class PropertiesTestHelper {
		public static void DoTest0(IBaseProperties source, Type propertiesType) {
			StringBuilder errors = new StringBuilder();
			Assert.IsFalse(source.ShouldSerialize());
			var sourceProperties = TypeDescriptor.GetProperties(source);
			var targetProperties = TypeDescriptor.GetProperties(propertiesType);
			foreach(PropertyDescriptor pd in targetProperties) {
				PropertyDescriptor srcPD = sourceProperties[pd.Name];
				Assert.IsNotNull(srcPD);
				Assert.AreEqual(srcPD.PropertyType, pd.PropertyType, "PropertyType mismatch: " + pd.Name);
				Assert.AreEqual(srcPD.IsReadOnly, pd.IsReadOnly, "IsReadOnly mismatch: " + pd.Name);
				if(srcPD.ShouldSerializeValue(source))
					errors.AppendLine(srcPD.Name + " - should serialize");
				if(!srcPD.IsReadOnly) {
					if(!source.IsContentProperty(pd.Name)) {
						var mInfo = typeof(IBaseProperties).GetMethod("GetDefaultValue", new Type[] { typeof(string) });
						mInfo = mInfo.MakeGenericMethod(new Type[] { pd.PropertyType });
						object defaultValue = mInfo.Invoke(source, new object[] { pd.Name });
						Assert.AreEqual(defaultValue, srcPD.GetValue(source), "GetDefaultValue<> mismatch: " + pd.Name);
					}
					else {
						var mInfo = typeof(IBaseProperties).GetMethod("GetContent", new Type[] { typeof(string) });
						mInfo = mInfo.MakeGenericMethod(new Type[] { pd.PropertyType });
						object content = mInfo.Invoke(source, new object[] { pd.Name });
						Assert.AreEqual(content, srcPD.GetValue(source), "GetContent<> mismatch: " + pd.Name);
					}
					if(System.ComponentModel.CategoryAttribute.Default.Category == srcPD.Category)
						errors.AppendLine(srcPD.Name + " - category is not set");
				}
			}
			Assert.IsFalse(source.ShouldSerialize());
			Assert.AreEqual(0, errors.Length, errors.ToString());
		}
		public static void DoTest1(IBaseDefaultProperties source, Type propertiesType) {
			StringBuilder errors = new StringBuilder();
			Assert.IsFalse(source.ShouldSerialize());
			var sourceProperties = TypeDescriptor.GetProperties(source);
			var targetProperties = TypeDescriptor.GetProperties(propertiesType);
			foreach(PropertyDescriptor pd in targetProperties) {
				PropertyDescriptor srcPD = sourceProperties[pd.Name];
				Assert.IsNotNull(srcPD);
				if(!srcPD.IsReadOnly) {
					if(srcPD.PropertyType == typeof(DevExpress.Utils.DefaultBoolean)) {
						try {
							source.GetActualValue<DevExpress.Utils.DefaultBoolean, bool>(srcPD.Name);
						}
						catch { errors.AppendLine(srcPD.Name + " - actual value converter error"); }
					}
					if(srcPD.PropertyType.IsGenericType && srcPD.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
						Type t = srcPD.PropertyType.GetGenericArguments()[0];
						try {
							sourceProperties["Actual" + pd.Name].GetValue(source);
						}
						catch { errors.AppendLine(srcPD.Name + " - actual value converter error"); }
					}
				}
			}
			Assert.IsFalse(source.ShouldSerialize());
			Assert.AreEqual(0, errors.Length, errors.ToString());
		}
		public static void DoTest2(IBaseDefaultProperties source, Type propertiesType) {
			DoTest2Core(source, source.ParentProperties, propertiesType);
		}
		public static void DoTest3(IBaseDefaultProperties source, IBaseProperties parent, Type propertiesType) {
			DoTest2Core(source, parent, propertiesType);
		}
		public static void CheckDXDescriptionAttribute(Type type) {
			StringBuilder errors = new StringBuilder();
			var sourceProperties = TypeDescriptor.GetProperties(type);
			foreach(PropertyDescriptor pd in sourceProperties) {
				if(pd.IsBrowsable && pd.ComponentType.FullName != null && pd.ComponentType.FullName.Contains("DevExpress")) {
					DXDescriptionAttribute attribute = GetAttributeByType(pd, typeof(DXDescriptionAttribute)) as DXDescriptionAttribute;
					if(attribute != null) {
						if(!attribute.Description.Equals(pd.ComponentType.FullName + "," + pd.Name))
							errors.AppendLine(attribute.Description + " - incorrect DXDescriptionAttribute");
					}
					else
						errors.AppendLine(type.Name + "." + pd.Name + " - non set DXDescriptionAttribute");
				}
			}
			Assert.AreEqual(0, errors.Length, errors.ToString());
		}
		static Attribute GetAttributeByType(PropertyDescriptor pd, Type attributeType) {
			foreach(Attribute attribute in pd.Attributes) {
				if(attribute.GetType() == attributeType) {
					return attribute;
				}
			}
			return null;
		}
		static void DoTest2Core(IBaseDefaultProperties source, IBaseProperties parent, Type propertiesType) {
			StringBuilder errors = new StringBuilder();
			Assert.IsFalse(source.ShouldSerialize());
			var sourceProperties = TypeDescriptor.GetProperties(source);
			var targetProperties = TypeDescriptor.GetProperties(propertiesType);
			var parentProperties = TypeDescriptor.GetProperties(parent);
			foreach(PropertyDescriptor pd in targetProperties) {
				PropertyDescriptor srcPD = sourceProperties[pd.Name];
				Assert.IsNotNull(srcPD);
				if(!srcPD.IsReadOnly) {
					PropertyDescriptor parentPD = parentProperties[pd.Name];
					if(srcPD.PropertyType == typeof(DevExpress.Utils.DefaultBoolean)) {
						var attribute = parentPD.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
						try {
							object actualValue = (attribute != null) ? attribute.Value :
								GetValue(parent, parentPD.PropertyType, pd.Name);
							bool value = source.GetActualValue<DevExpress.Utils.DefaultBoolean, bool>(srcPD.Name);
							if(!object.Equals(actualValue, value))
								errors.AppendLine(srcPD.Name + " - actual value is differ than value specified parent attribute");
						}
						catch { errors.AppendLine(srcPD.Name + " - actual value access error"); }
					}
					if(srcPD.PropertyType.IsGenericType && srcPD.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
						Type t = srcPD.PropertyType.GetGenericArguments()[0];
						var attribute = parentPD.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
						try {
							object actualValue = (attribute != null) ? attribute.Value :
								GetValue(parent, parentPD.PropertyType, pd.Name);
							object value = sourceProperties["Actual" + pd.Name].GetValue(source);
							if(!object.Equals(actualValue, value))
								errors.AppendLine(srcPD.Name + " - actual value is differ form parent attribute");
						}
						catch { errors.AppendLine(srcPD.Name + " - actual value error"); }
					}
				}
				Assert.IsFalse(source.ShouldSerialize());
				Assert.AreEqual(0, errors.Length, errors.ToString());
			}
		}
		static object GetValue(IBaseProperties source, Type propertyType, string property) {
			var mInfo = typeof(IBaseProperties).GetMethod("GetValue", new Type[] { typeof(string) });
			mInfo = mInfo.MakeGenericMethod(new Type[] { propertyType });
			return mInfo.Invoke(source, new object[] { property });
		}
	}
	class TestProperties : DevExpress.XtraBars.Docking2010.Base.BaseProperties, ITestProperties {
		public TestProperties() {
			SetDefaultValueCore("BooleanTrueProperty", true);
			SetDefaultValueCore("IntProperty100", 100);
		}
		[DefaultValue(0), System.ComponentModel.Category("Test")]
		public int IntProperty {
			get { return GetValueCore<int>("IntProperty"); }
			set { SetValueCore("IntProperty", value); }
		}
		[DefaultValue(100), System.ComponentModel.Category("Test")]
		public int IntProperty100 {
			get { return GetValueCore<int>("IntProperty100"); }
			set { SetValueCore("IntProperty100", value); }
		}
		[DefaultValue(false), System.ComponentModel.Category("Test")]
		public bool BooleanFalseProperty {
			get { return GetValueCore<bool>("BooleanFalseProperty"); }
			set { SetValueCore("BooleanFalseProperty", value); }
		}
		[DefaultValue(true), System.ComponentModel.Category("Test")]
		public bool BooleanTrueProperty {
			get { return GetValueCore<bool>("BooleanTrueProperty"); }
			set { SetValueCore("BooleanTrueProperty", value); }
		}
		internal int changedCount;
		protected override void OnUpdateObjectCore() {
			base.OnUpdateObjectCore();
			changedCount++;
		}
		protected override IBaseProperties CloneCore() {
			return new TestProperties();
		}
	}
	class TestDefaultProperties : DevExpress.XtraBars.Docking2010.Base.BaseDefaultProperties, ITestDefaultProperties {
		public TestDefaultProperties(ITestProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("BooleanFalseProperty", DefaultBoolean.Default);
			SetDefaultValueCore("BooleanTrueProperty", DefaultBoolean.Default);
			SetConverter("IntProperty100", GetNullableValueConverter(100));
			SetConverter("BooleanFalseProperty", GetDefaultBooleanConverter(false));
			SetConverter("BooleanTrueProperty", GetDefaultBooleanConverter(true));
		}
		[DefaultValue(null), System.ComponentModel.Category("Test"), TypeConverter(typeof(NullableTypeConverter))]
		public int? IntProperty {
			get { return GetValueCore<int?>("IntProperty"); }
			set { SetValueCore("IntProperty", value); }
		}
		[DefaultValue(null), System.ComponentModel.Category("Test"), TypeConverter(typeof(NullableTypeConverter))]
		public int? IntProperty100 {
			get { return GetValueCore<int?>("IntProperty100"); }
			set { SetValueCore("IntProperty100", value); }
		}
		[DefaultValue(DefaultBoolean.Default), System.ComponentModel.Category("Test")]
		public DefaultBoolean BooleanFalseProperty {
			get { return GetValueCore<DefaultBoolean>("BooleanFalseProperty"); }
			set { SetValueCore("BooleanFalseProperty", value); }
		}
		[DefaultValue(DefaultBoolean.Default), System.ComponentModel.Category("Test")]
		public DefaultBoolean BooleanTrueProperty {
			get { return GetValueCore<DefaultBoolean>("BooleanTrueProperty"); }
			set { SetValueCore("BooleanTrueProperty", value); }
		}
		internal int changedCount;
		protected override void OnUpdateObjectCore() {
			base.OnUpdateObjectCore();
			changedCount++;
		}
		protected override IBaseProperties CloneCore() {
			return new TestDefaultProperties(ParentProperties as ITestProperties);
		}
		public int ActualIntProperty { get { return GetActualValueFromNullable<int>("IntProperty"); } }
		public int ActualIntProperty100 { get { return GetActualValueFromNullable<int>("IntProperty100"); } }
		public bool ActualBooleanFalseProperty { get { return GetActualValue<DefaultBoolean, bool>("BooleanFalseProperty"); } }
		public bool ActualBooleanTrueProperty { get { return GetActualValue<DefaultBoolean, bool>("BooleanTrueProperty"); } }
	}
	[TestFixture]
	public class BasePropertiesTests {
		[Test]
		public void Test00_Default() {
			TestProperties properties = new TestProperties();
			Assert.AreEqual(0, properties.IntProperty);
			Assert.AreEqual(100, properties.IntProperty100);
			Assert.AreEqual(false, properties.BooleanFalseProperty);
			Assert.AreEqual(true, properties.BooleanTrueProperty);
		}
		[Test]
		public void Test01_ShouldSerialize() {
			TestProperties properties = new TestProperties();
			Assert.IsFalse(properties.ShouldSerialize());
			var descriptors = TypeDescriptor.GetProperties(properties);
			Assert.IsFalse(descriptors["IntProperty"].ShouldSerializeValue(properties));
			Assert.IsFalse(descriptors["IntProperty100"].ShouldSerializeValue(properties));
			Assert.IsFalse(descriptors["BooleanFalseProperty"].ShouldSerializeValue(properties));
			Assert.IsFalse(descriptors["BooleanTrueProperty"].ShouldSerializeValue(properties));
		}
		[Test]
		public void Test02_Changing() {
			TestProperties properties = new TestProperties();
			using(BatchUpdate.Enter(properties)) {
				properties.BooleanTrueProperty = false;
				properties.BooleanFalseProperty = true;
				properties.IntProperty = 5;
				properties.IntProperty100 = 105;
				Assert.AreEqual(0, properties.changedCount);
			}
			Assert.AreEqual(1, properties.changedCount);
			Assert.AreEqual(5, properties.IntProperty);
			Assert.AreEqual(105, properties.IntProperty100);
			Assert.AreEqual(true, properties.BooleanFalseProperty);
			Assert.AreEqual(false, properties.BooleanTrueProperty);
		}
		[Test]
		public void Test03() {
			TestProperties properties = new TestProperties();
			PropertiesTestHelper.DoTest0(properties, typeof(ITestProperties));
		}
	}
	[TestFixture]
	public class BaseDefaultPropertiesTests {
		[Test]
		public void Test00_Default() {
			TestProperties parentProperties = new TestProperties();
			TestDefaultProperties properties = new TestDefaultProperties(parentProperties);
			Assert.AreEqual(null, properties.IntProperty);
			Assert.AreEqual(null, properties.IntProperty100);
			Assert.AreEqual(DefaultBoolean.Default, properties.BooleanFalseProperty);
			Assert.AreEqual(DefaultBoolean.Default, properties.BooleanTrueProperty);
			Assert.AreEqual(0, properties.ActualIntProperty);
			Assert.AreEqual(100, properties.ActualIntProperty100);
			Assert.AreEqual(false, properties.ActualBooleanFalseProperty);
			Assert.AreEqual(true, properties.ActualBooleanTrueProperty);
		}
		[Test]
		public void Test01_ShouldSerialize() {
			TestProperties parentProperties = new TestProperties();
			TestDefaultProperties properties = new TestDefaultProperties(parentProperties);
			Assert.IsFalse(properties.ShouldSerialize());
			var descriptors = TypeDescriptor.GetProperties(properties);
			Assert.IsFalse(descriptors["IntProperty"].ShouldSerializeValue(properties));
			Assert.IsFalse(descriptors["IntProperty100"].ShouldSerializeValue(properties));
			Assert.IsFalse(descriptors["BooleanFalseProperty"].ShouldSerializeValue(properties));
			Assert.IsFalse(descriptors["BooleanTrueProperty"].ShouldSerializeValue(properties));
		}
		[Test]
		public void Test02_Changing() {
			TestProperties parentProperties = new TestProperties();
			TestDefaultProperties properties = new TestDefaultProperties(parentProperties);
			using(BatchUpdate.Enter(properties)) {
				properties.BooleanTrueProperty = DefaultBoolean.False;
				properties.BooleanFalseProperty = DefaultBoolean.True;
				properties.IntProperty = 5;
				properties.IntProperty100 = 105;
				Assert.AreEqual(0, properties.changedCount);
			}
			Assert.AreEqual(1, properties.changedCount);
			Assert.AreEqual(5, properties.IntProperty);
			Assert.AreEqual(105, properties.IntProperty100);
			Assert.AreEqual(DefaultBoolean.True, properties.BooleanFalseProperty);
			Assert.AreEqual(DefaultBoolean.False, properties.BooleanTrueProperty);
		}
		[Test]
		public void Test02_ChangingParent() {
			TestProperties parentProperties = new TestProperties();
			TestDefaultProperties properties = new TestDefaultProperties(parentProperties);
			using(BatchUpdate.Enter(parentProperties)) {
				parentProperties.BooleanTrueProperty = false;
				parentProperties.BooleanFalseProperty = true;
				parentProperties.IntProperty = 5;
				parentProperties.IntProperty100 = 105;
				Assert.AreEqual(0, parentProperties.changedCount);
			}
			Assert.AreEqual(0, properties.changedCount);
			Assert.AreEqual(1, parentProperties.changedCount);
			Assert.AreEqual(null, properties.IntProperty);
			Assert.AreEqual(null, properties.IntProperty100);
			Assert.AreEqual(DefaultBoolean.Default, properties.BooleanFalseProperty);
			Assert.AreEqual(DefaultBoolean.Default, properties.BooleanTrueProperty);
			Assert.AreEqual(5, properties.ActualIntProperty);
			Assert.AreEqual(105, properties.ActualIntProperty100);
			Assert.AreEqual(true, properties.ActualBooleanFalseProperty);
			Assert.AreEqual(false, properties.ActualBooleanTrueProperty);
		}
		[Test]
		public void Test03_DetachedDefault() {
			TestDefaultProperties properties = new TestDefaultProperties(null);
			Assert.AreEqual(null, properties.IntProperty);
			Assert.AreEqual(null, properties.IntProperty100);
			Assert.AreEqual(DefaultBoolean.Default, properties.BooleanFalseProperty);
			Assert.AreEqual(DefaultBoolean.Default, properties.BooleanTrueProperty);
			Assert.AreEqual(0, properties.ActualIntProperty);
			Assert.AreEqual(100, properties.ActualIntProperty100);
			Assert.AreEqual(false, properties.ActualBooleanFalseProperty);
			Assert.AreEqual(true, properties.ActualBooleanTrueProperty);
		}
		public void Test04_ChangingDetached() {
			TestDefaultProperties properties = new TestDefaultProperties(null);
			using(BatchUpdate.Enter(properties)) {
				properties.BooleanTrueProperty = DefaultBoolean.False;
				properties.BooleanFalseProperty = DefaultBoolean.True;
				properties.IntProperty = 5;
				properties.IntProperty100 = 105;
				Assert.AreEqual(0, properties.changedCount);
			}
			Assert.AreEqual(1, properties.changedCount);
			Assert.AreEqual(5, properties.IntProperty);
			Assert.AreEqual(105, properties.IntProperty100);
			Assert.AreEqual(DefaultBoolean.True, properties.BooleanFalseProperty);
			Assert.AreEqual(DefaultBoolean.False, properties.BooleanTrueProperty);
			Assert.AreEqual(5, properties.ActualIntProperty);
			Assert.AreEqual(105, properties.ActualIntProperty100);
			Assert.AreEqual(true, properties.ActualBooleanFalseProperty);
			Assert.AreEqual(false, properties.ActualBooleanTrueProperty);
			using(BatchUpdate.Enter(properties)) {
				properties.BooleanTrueProperty = DefaultBoolean.True;
				properties.BooleanFalseProperty = DefaultBoolean.False;
			}
			Assert.AreEqual(DefaultBoolean.False, properties.BooleanFalseProperty);
			Assert.AreEqual(DefaultBoolean.True, properties.BooleanTrueProperty);
			Assert.AreEqual(false, properties.ActualBooleanFalseProperty);
			Assert.AreEqual(true, properties.ActualBooleanTrueProperty);
		}
		[Test]
		public void Test05() {
			var parentProperties = new TestProperties();
			var defaultProperties1 = new TestDefaultProperties(null);
			var defaultProperties2 = new TestDefaultProperties(parentProperties);
			PropertiesTestHelper.DoTest0(defaultProperties1, typeof(ITestDefaultProperties));
			PropertiesTestHelper.DoTest1(defaultProperties1, typeof(ITestDefaultProperties));
			PropertiesTestHelper.DoTest2(defaultProperties2, typeof(ITestDefaultProperties));
			PropertiesTestHelper.DoTest3(defaultProperties1, parentProperties, typeof(ITestDefaultProperties));
		}
	}
}
#endif
