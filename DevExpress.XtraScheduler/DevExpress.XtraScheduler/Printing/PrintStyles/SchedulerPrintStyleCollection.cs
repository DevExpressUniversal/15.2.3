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

using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	[ListBindable(BindableSupport.No)]
	public class SchedulerPrintStyleCollection : UserInterfaceObjectCollection<SchedulerPrintStyle>, IXtraSerializable2 {
		Hashtable htKind = new Hashtable();
		internal SchedulerPrintStyleCollection(bool loadDefaults) {
			if (loadDefaults)
				LoadDefaults();
		}
		public SchedulerPrintStyleCollection()
			: this(true) {
		}
		public new SchedulerPrintStyle this[int index] { get { return GetItem(index); } }
		public SchedulerPrintStyle this[SchedulerPrintStyleKind kind] { get { return (SchedulerPrintStyle)htKind[kind]; } }
		internal Hashtable HtKind { get { return htKind; } }
		protected Hashtable KindHash { get { return htKind; } }
		protected override SchedulerPrintStyle GetItem(int index) {
			return InnerList[index];
		}
		public void DisposeCollectionElements() {
			int count = this.Count;
			for (int i = 0; i < count; i++)
				this[i].Dispose();
			Clear();
		}
		public override void LoadDefaults() {
			Clear();
			AddRange(new SchedulerPrintStyle[] {
												   new DailyPrintStyle(),
												   new WeeklyPrintStyle(),
												   new MonthlyPrintStyle(),
												   new TriFoldPrintStyle(),
												   new CalendarDetailsPrintStyle(),
												   new MemoPrintStyle()
											   });
		}
		public bool IsDisplayNameExists(string displayName) {
			return FindDisplayName(displayName) >= 0;
		}
		public void DeleteStyle(SchedulerPrintStyle style) {
			int index = List.IndexOf(style);
			if (index != -1)
				List.RemoveAt(index);
		}
		public void ToXml(Stream stream, string appName) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.SerializeObject(this, stream, appName);
		}
		public void ToXml(string path, string appName) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.SerializeObject(this, path, appName);
		}
		public void FromXml(Stream stream, string appName) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.DeserializeObject(this, stream, appName);
		}
		public void FromXml(string path, string appName) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.DeserializeObject(this, path, appName);
		}
		public bool IsEqual(SchedulerPrintStyleCollection printStyleCollection) {
			int count = this.Count;
			if (printStyleCollection == null || printStyleCollection.Count != count)
				return false;
			for (int i = 0; i < count; i++)
				if (!printStyleCollection[i].Equals(this[i]))
					return false;
			return true;
		}
		protected override void OnInsertComplete(int index, SchedulerPrintStyle value) {
			base.OnInsertComplete(index, value);
			KindHash[value.Kind] = value;
		}
		protected override void OnRemoveComplete(int index, SchedulerPrintStyle value) {
			base.OnRemoveComplete(index, value);
			if (!KindHash.Contains(value.Kind))
				return;
			KindHash.Remove(value.Kind);
			int count = this.Count;
			for (int i = 0; i < count; i++)
				if (value.Kind == this[i].Kind)
					KindHash[value.Kind] = this[i];
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			KindHash.Clear();
		}
		protected internal override UserInterfaceObjectCollection<SchedulerPrintStyle> CreateDefaultContent() {
			return new SchedulerPrintStyleCollection();
		}
		protected internal override SchedulerPrintStyle CloneItem(SchedulerPrintStyle item) {
			return null;
		}
		protected internal override SchedulerPrintStyle CreateItemInstance(object id, string displayName, string menuCaption) {
			return null;
		}
		protected internal override object ProvideDefaultId() {
			return null;
		}
		int FindDisplayName(string displayName) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (this[i].DisplayName == displayName)
					return i;
			}
			return -1;
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			List<XtraPropertyInfo> items = new List<XtraPropertyInfo>();
			int count = Count;
			SerializeHelper helper = new SerializeHelper();
			for (int i = 0; i < count; i++) {
				XtraPropertyInfo pInfo = new XtraPropertyInfo("Kind", typeof(SchedulerPrintStyleKind), this[i].Kind, true);
				pInfo.ChildProperties.AddRange(helper.SerializeObject(this[i], new DevExpress.Utils.OptionsLayoutBase()));
				pInfo.ChildProperties.AddRange(SchedulerPrintStyle.SerializePageSettings(this[i]));
				items.Add(pInfo);
			}
			return items.ToArray();
		}
		void IXtraSerializable2.Deserialize(IList list) {
			SchedulerPrintStyleCollection newStyles = new SchedulerPrintStyleCollection(false);
			if (DeserializeOldXml(list))
				return;
			EnumConverter enumConverter = new EnumConverter(typeof(SchedulerPrintStyleKind));
			DeserializeHelper helper = new DeserializeHelper();
			foreach (XtraPropertyInfo property in list) {
				if (property.Name != "Kind")
					return;
				if (property.Value == null)
					return;
				object kind = enumConverter.ConvertFrom(property.Value);
				if (kind == null)
					return;
				XtraPropertyInfo baseStylePropertyInfo = helper.FindProperty(property.ChildProperties, "BaseStyle");
				bool baseStyle = true;
				if (baseStylePropertyInfo != null) {
					object result = baseStylePropertyInfo.ValueToObject(typeof(bool));
					if (result != null)
						baseStyle = (bool)result;
				}
				SchedulerPrintStyle printStyle = SchedulerPrintStyle.CreateInstance((SchedulerPrintStyleKind)kind, baseStyle);
				XtraPropertyInfo colorConverterPropertyInfo = helper.FindProperty(property.ChildProperties, "ColorConverter");
				if (colorConverterPropertyInfo != null) {
					SchedulerPrintStyle.DeserializeColorConverter(colorConverterPropertyInfo, printStyle);
					((IList)property.ChildProperties).Remove(colorConverterPropertyInfo);
				}
				XtraPropertyInfo pageSettingsPropertyInfo = helper.FindProperty(property.ChildProperties, "PageSettings");
				if (pageSettingsPropertyInfo != null) {
					SchedulerPrintStyle.DeserializePageSettings(pageSettingsPropertyInfo.ChildProperties, printStyle);
					((IList)property.ChildProperties).Remove(pageSettingsPropertyInfo);
				}
				helper.DeserializeObject(printStyle, property.ChildProperties, new DevExpress.Utils.OptionsLayoutBase());
				newStyles.Add(printStyle);
			}
			this.Clear();
			this.AddRange(newStyles);
		}
		bool DeserializeOldXml(IList list) {
			if (list.Count > 2 || list.Count <= 0)
				return false;
			SchedulerPrintStyleCollection newStyles = new SchedulerPrintStyleCollection(false);
			IXtraPropertyCollection pageSettingsProperties = null;
			IXtraPropertyCollection styleCollection = null;
			foreach (XtraPropertyInfo property in list) {
				if (property.Name == "PageSettingsCollection")
					pageSettingsProperties = property.ChildProperties;
				if (property.Name == "Collection")
					styleCollection = property.ChildProperties;
			}
			if (styleCollection == null)
				return false;
			int styleCount = styleCollection.Count;
			int pageSettingsCount = pageSettingsProperties.Count;
			EnumConverter enumConverter = new EnumConverter(typeof(SchedulerPrintStyleKind));
			DeserializeHelper helper = new DeserializeHelper();
			for (int i = 0; i < styleCount; i++) {
				XtraPropertyInfo styleProperty = styleCollection[i];
				XtraPropertyInfo kindProperty = helper.FindProperty(styleProperty.ChildProperties, "Kind");
				object kind = enumConverter.ConvertFrom(kindProperty.Value);
				if (kind == null)
					return false;
				XtraPropertyInfo baseStylePropertyInfo = helper.FindProperty(styleProperty.ChildProperties, "BaseStyle");
				bool baseStyle = true;
				if (baseStylePropertyInfo != null) {
					object result = baseStylePropertyInfo.ValueToObject(typeof(bool));
					if (result != null)
						baseStyle = (bool)result;
				}
				SchedulerPrintStyle printStyle = SchedulerPrintStyle.CreateInstance((SchedulerPrintStyleKind)kind, baseStyle);
				XtraPropertyInfo colorConverterPropertyInfo = helper.FindProperty(styleProperty.ChildProperties, "ColorConverter");
				if (colorConverterPropertyInfo != null) {
					SchedulerPrintStyle.DeserializeColorConverter(colorConverterPropertyInfo, printStyle);
					((IList)styleProperty.ChildProperties).Remove(colorConverterPropertyInfo);
				}
				XtraPropertyInfo printTimePropertyInfo = helper.FindProperty(styleProperty.ChildProperties, "PrintTime");
				if (printTimePropertyInfo != null) {
					XtraPropertyInfo startProperty = helper.FindProperty(printTimePropertyInfo.ChildProperties, "Start");
					XtraPropertyInfo durationProperty = helper.FindProperty(printTimePropertyInfo.ChildProperties, "Duration");
					if (startProperty != null && durationProperty != null) {
						TimeSpan start = (TimeSpan)startProperty.ValueToObject(typeof(TimeSpan));
						TimeSpan duration = (TimeSpan)durationProperty.ValueToObject(typeof(TimeSpan));
						TimeSpan end = start + duration;
						PropertyInfo pInfo = printStyle.GetType().GetProperty("PrintTime");
						if (pInfo != null)
							pInfo.SetValue(printStyle, new TimeOfDayInterval(start, end), null);
					}
					((IList)styleProperty.ChildProperties).Remove(printTimePropertyInfo);
				}
				helper.DeserializeObject(printStyle, styleProperty.ChildProperties, new DevExpress.Utils.OptionsLayoutBase());
				if (i < pageSettingsCount)
					SchedulerPrintStyle.DeserializePageSettings(pageSettingsProperties[i].ChildProperties, printStyle);
				newStyles.Add(printStyle);
			}
			this.Clear();
			this.AddRange(newStyles);
			return true;
		}
	}
}
