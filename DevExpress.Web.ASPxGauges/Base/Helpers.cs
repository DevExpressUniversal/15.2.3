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
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Web.UI;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using DevExpress.XtraGauges.Base;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
namespace DevExpress.Web.ASPxGauges.Base {
	public static class StateManagedHierarchyObjectExtension {
		public static void TrackViewState(IStateManagedHierarchyObject obj) {
			obj.TrackViewStateCore();
			ViewStateUtils.TrackObjectsViewState(obj.StateManagedObjects);
		}
		public static object SaveViewState(IStateManagedHierarchyObject obj) {
			return ViewStateUtils.SaveViewState(obj.SaveViewStateCore(), obj.StateManagedObjects);
		}
		public static void LoadViewState(IStateManagedHierarchyObject obj, object state) {
			if(state == null) return;
			object[] stateArray = state as object[];
			obj.LoadViewStateCore(((stateArray != null) && (stateArray.Length > 0)) ? stateArray[0] : null);
			ViewStateUtils.LoadObjectsViewState(stateArray, obj.StateManagedObjects);
		}
	}
	public static class StateManagedHierarchyObjectCollectionExtension {
		public static void TrackViewState(IStateManagedHierarchyObjectCollection obj) {
			obj.TrackViewStateCore();
			foreach(IStateManagedHierarchyObject o in obj) o.TrackViewState();
		}
		public static object SaveViewState(IStateManagedHierarchyObjectCollection collection) {
			object[] objArray = new object[collection.Count + 1];
			objArray[0] = collection.SaveViewStateCore();
			for(int i = 0; i < collection.Count; i++) {
				objArray[i + 1] = collection[i].SaveViewState();
			}
			return objArray;
		}
		public static void LoadViewState(IStateManagedHierarchyObjectCollection collection, object state) {
			object[] objArray = state as object[];
			if(objArray == null) return;
			collection.LoadViewStateCore(objArray[0]);
			for(int i = 0; i < collection.Count; i++) {
				state = ((i + 1) < objArray.Length) ? objArray[i + 1] : null;
				if(state != null) collection[i].LoadViewState(state);
			}
		}
	}
	public static class StateManagedObjectCollectionHelper {
		public static object TrackViewStateCore(object item) {
			return MakeSnapshot(item);
		}
		public static object SaveViewStateCore(object savedViewState, object item) {
			return GetDifferences(savedViewState as object[], item);
		}
		public static void LoadViewStateCore(object item, object state) {
			object[] objArray = state as object[];
			IStateManagedHierarchyObjectCollection collection = item as IStateManagedHierarchyObjectCollection;
			if(collection != null && objArray != null) {
				object[] existingItems = GetItems(collection);
				collection.Clear();
				int resultLength = Math.Max(objArray.Length, existingItems.Length);
				for(int i = 0; i < resultLength; i++) {
					object obj = (i < existingItems.Length) ? existingItems[i] : null;
					if(i<objArray.Length && objArray[i] != null) {
						obj = Activator.CreateInstance(TypeCache.GetType((int)objArray[i]));
					}
					if(obj != null) collection.Add(obj as IStateManagedHierarchyObject);
				}
			}
		}
		static object[] GetItems(IStateManagedHierarchyObjectCollection collection) {
			object[] objArray = new object[collection.Count];
			for(int i = 0; i < objArray.Length; i++)
				objArray[i] = collection[i];
			return objArray;
		}
		static object[] MakeSnapshot(object item) {
			object[] objArray = null;
			IStateManagedHierarchyObjectCollection collection = item as IStateManagedHierarchyObjectCollection;
			if(collection != null) {
				objArray = new object[collection.Count];
				for(int i = 0; i < objArray.Length; i++) {
					objArray[i] = TypeCache.GetTypeCode(collection[i].GetType());
				}
			}
			return objArray;
		}
		static object[] GetDifferences(object[] snapshot, object item) {
			List<object> differences = new List<object>(MakeSnapshot(item));
			if(snapshot != null && differences.Count == snapshot.Length) {
				for(int i = 0; i < snapshot.Length; i++) {
					if((int)differences[i] == (int)snapshot[i]) differences[i] = null;
				}
			}
			return differences.ToArray();
		}
	}
	public static class StateManagedObjectHelper {
		static StateManagedObjectSerializer serializerCore;
		static Type filterType;
		static StateManagedObjectHelper() {
			filterType = typeof(IStateManagedHierarchyObject);
			serializerCore = new StateManagedObjectSerializer();
		}
		public static object TrackViewStateCore(object item) {
			return MakeSnapshot(item);
		}
		public static object SaveViewStateCore(object savedViewState, object item) {
			object state = null;
			if(item != null && savedViewState is IXtraPropertyCollection) {
				IXtraPropertyCollection differences = GetDifferences(savedViewState as IXtraPropertyCollection, item);
				state = (differences == null || differences.Count == 0) ? null : Serializer.Serialize(differences);
			}
			return state;
		}
		public static void LoadViewStateCore(object item, object state) {
			string stateString = state as string;
			if(item != null && stateString != null) Serializer.Deserialize(item, stateString);
		}
		static IXtraPropertyCollection MakeSnapshot(object item) {
			IXtraPropertyCollection snapshot = new SerializeHelper().SerializeObject(item, OptionsLayoutBase.FullLayout);
			return FilterProperties(snapshot);
		}
		static IXtraPropertyCollection GetDifferences(IXtraPropertyCollection snapshot, object item) {
			return SerializationDiffCalculator.CalculateDiffCore(snapshot, MakeSnapshot(item));
		}
		static XtraPropertyInfoCollection FilterProperties(IXtraPropertyCollection props) {
			XtraPropertyInfoCollection result = new XtraPropertyInfoCollection();
			foreach(XtraPropertyInfo propInfo in props) {
				if(filterType.IsAssignableFrom(propInfo.PropertyType)) continue;
				result.Add(propInfo);
			}
			return result;
		}
		static StateManagedObjectSerializer Serializer {
			get { return serializerCore; }
		}
	}
	public class StateManagedObjectSerializer : BinaryXtraSerializer {
		const string AppLabel = "XtraGauge";
		public void Deserialize(object obj, string stateString) {
			using(MemoryStream sourceStream = new MemoryStream(Convert.FromBase64String(stateString), false)) {
				sourceStream.Seek(0, SeekOrigin.Begin);
				DeserializeObject(obj, sourceStream, AppLabel);
			}
		}
		public string Serialize(IXtraPropertyCollection properties) {
			string result = string.Empty;
			using(MemoryStream targetStream = new MemoryStream()) {
				Serialize(targetStream, properties, AppLabel);
				result = Convert.ToBase64String(targetStream.ToArray());
			}
			return result;
		}
	}
	internal class ViewStateInternal {
		bool isTrackingViewStateCore = false;
		object stateCore;
		public bool IsTrackingViewState {
			get { return isTrackingViewStateCore; }
		}
		public object State {
			get { return stateCore; }
		}
		public void TrackViewState() {
			this.isTrackingViewStateCore = true;
		}
		public void SaveStateSnapshot(object snapshot) {
			this.stateCore = snapshot;
		}
	}
	static class TypeCache {
		static Dictionary<int, Type> cache;
		static TypeCache() {
			cache = new Dictionary<int, Type>();
			cache.Add(TypeToken.Unknown, typeof(UnknownType));
			cache.Add(TypeToken.CircularGauge, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.CircularGauge));
			cache.Add(TypeToken.LinearGauge, typeof(DevExpress.Web.ASPxGauges.Gauges.Linear.LinearGauge));
			cache.Add(TypeToken.StateIndicatorGauge, typeof(DevExpress.Web.ASPxGauges.Gauges.State.StateIndicatorGauge));
			cache.Add(TypeToken.DigitalGauge, typeof(DevExpress.Web.ASPxGauges.Gauges.Digital.DigitalGauge));
			cache.Add(TypeToken.LabelComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.LabelComponent));
			cache.Add(TypeToken.ArcScaleComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.ArcScaleComponent));
			cache.Add(TypeToken.ArcScaleBackgroundLayerComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.ArcScaleBackgroundLayerComponent));
			cache.Add(TypeToken.ArcScaleNeedleComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.ArcScaleNeedleComponent));
			cache.Add(TypeToken.ArcScaleSpindleCapComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.ArcScaleSpindleCapComponent));
			cache.Add(TypeToken.ArcScaleMarkerComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.ArcScaleMarkerComponent));
			cache.Add(TypeToken.ArcScaleRangeBarComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.ArcScaleRangeBarComponent));
			cache.Add(TypeToken.ArcScaleEffectLayerComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.ArcScaleEffectLayerComponent));
			cache.Add(TypeToken.ArcScaleStateIndicatorComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Circular.ArcScaleStateIndicatorComponent));
			cache.Add(TypeToken.LinearScaleComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Linear.LinearScaleComponent));
			cache.Add(TypeToken.LinearScaleBackgroundLayerComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Linear.LinearScaleBackgroundLayerComponent));
			cache.Add(TypeToken.LinearScaleLevelComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Linear.LinearScaleLevelComponent));
			cache.Add(TypeToken.LinearScaleMarkerComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Linear.LinearScaleMarkerComponent));
			cache.Add(TypeToken.LinearScaleRangeBarComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Linear.LinearScaleRangeBarComponent));
			cache.Add(TypeToken.LinearScaleEffectLayerComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Linear.LinearScaleEffectLayerComponent));
			cache.Add(TypeToken.LinearScaleStateIndicatorComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Linear.LinearScaleStateIndicatorComponent));
			cache.Add(TypeToken.StateIndicatorComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.State.StateIndicatorComponent));
			cache.Add(TypeToken.DigitalBackgroundLayerComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Digital.DigitalBackgroundLayerComponent));
			cache.Add(TypeToken.DigitalEffectLayerComponent, typeof(DevExpress.Web.ASPxGauges.Gauges.Digital.DigitalEffectLayerComponent));
			cache.Add(TypeToken.ScaleLabel, typeof(DevExpress.Web.ASPxGauges.Gauges.ScaleLabelWeb));
			cache.Add(TypeToken.ArcScaleRange, typeof(DevExpress.Web.ASPxGauges.Gauges.ArcScaleRangeWeb));
			cache.Add(TypeToken.LinearScaleRange, typeof(DevExpress.Web.ASPxGauges.Gauges.LinearScaleRangeWeb));
			cache.Add(TypeToken.IndicatorState, typeof(DevExpress.Web.ASPxGauges.Gauges.IndicatorStateWeb));
			cache.Add(TypeToken.ScaleIndicatorState, typeof(DevExpress.Web.ASPxGauges.Gauges.ScaleIndicatorStateWeb));
		}
		public static int GetTypeCode(Type type) {
			return cache.ContainsValue(type) ? FindKey(type) : CacheType(type);
		}
		public static Type GetType(int typeCode){
			Type resultType = cache[TypeToken.Unknown];
			cache.TryGetValue(typeCode, out resultType);
			return resultType;
		}
		static int FindKey(Type type) {
			foreach(KeyValuePair<int, Type> pair in cache)
				if(pair.Value == type) return pair.Key;
			return TypeToken.Unknown;
		}
		static int CacheType(Type type) {
			int key = TypeToken.Unknown - cache.Count;
			cache.Add(key, type);
			return key;
		}
		class UnknownType { }
		static class TypeToken {
			public static int Unknown = -1;
			static int Gauge = 0x10000;
			static int Component = 0x100;
			static int CollectionItem = 0x200;
			static int ScaleCollectionItem = 0x400;
			static int Circular = 0x1000;
			static int Linear = 0x2000;
			static int Digital = 0x4000;
			static int StateIndicator = 0x8000;
			static int Scale = 0x0001;
			static int BackgroundLayer = 0x0002;
			static int EffectLayer = 0x0003;
			static int Needle = 0x0004;
			static int Level = 0x0005;
			static int Marker = 0x0006;
			static int RangeBar = 0x0007;
			static int Indicator = 0x0008;
			static int SpindleCap = 0x0009;
			static int Label = 0x000A;
			static int LabelItem = 0x000B;
			static int RangeItem = 0x000C;
			static int IndicatorStateItem = 0x000D;
			public static int CircularGauge = Gauge + Circular;
			public static int LinearGauge = Gauge + Linear;
			public static int DigitalGauge = Gauge + Digital;
			public static int StateIndicatorGauge = Gauge + StateIndicator;
			public static int LabelComponent = Component + Label;
			public static int ArcScaleComponent = Component + Circular + Scale;
			public static int ArcScaleBackgroundLayerComponent = Component + Circular + BackgroundLayer;
			public static int ArcScaleNeedleComponent = Component + Circular + Needle;
			public static int ArcScaleSpindleCapComponent = Component + Circular + SpindleCap;
			public static int ArcScaleMarkerComponent = Component + Circular + Marker;
			public static int ArcScaleRangeBarComponent = Component + Circular + RangeBar;
			public static int ArcScaleStateIndicatorComponent = Component + Circular + Indicator;
			public static int ArcScaleEffectLayerComponent = Component + Circular + EffectLayer;
			public static int LinearScaleComponent = Component + Linear + Scale;
			public static int LinearScaleBackgroundLayerComponent = Component + Linear + BackgroundLayer;
			public static int LinearScaleLevelComponent = Component + Linear + Level;
			public static int LinearScaleMarkerComponent = Component + Linear + Marker;
			public static int LinearScaleRangeBarComponent = Component + Linear + RangeBar;
			public static int LinearScaleStateIndicatorComponent = Component + Linear + Indicator;
			public static int LinearScaleEffectLayerComponent = Component + Linear + EffectLayer;
			public static int StateIndicatorComponent = Component + StateIndicator + Indicator;
			public static int DigitalBackgroundLayerComponent = Component + Digital + BackgroundLayer;
			public static int DigitalEffectLayerComponent = Component + Digital + EffectLayer;
			public static int ScaleLabel = ScaleCollectionItem + LabelItem;
			public static int ArcScaleRange = Circular + ScaleCollectionItem + RangeItem;
			public static int LinearScaleRange = Linear + ScaleCollectionItem + LabelItem;
			public static int IndicatorState = StateIndicator + CollectionItem + IndicatorStateItem;
			public static int ScaleIndicatorState = ScaleCollectionItem + IndicatorStateItem;
		}
	}
	internal static class StateConverterHelper {
		public static string FromObjectState(object objState) {
			string stateString = null;
			using(MemoryStream stateStream = new MemoryStream()) {
				new ObjectStateFormatter().Serialize(stateStream, objState);
				stateString = Convert.ToBase64String(stateStream.ToArray());
			}
			return stateString;
		}
		public static object FromStateString(string stateString) {
			object objState = null;
			byte[] streamBytes = Convert.FromBase64String(stateString);
			using(MemoryStream stateStream = new MemoryStream(streamBytes)) {
				objState = new ObjectStateFormatter().Deserialize(stateStream);
			}
			return objState;
		}
	}
	public enum ImageType { Default, Bmp, Jpeg, Png, Gif }
}
