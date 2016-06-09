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
using System.Text;
using System.Web.UI;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using System.IO;
using System.IO.Compression;
using DevExpress.Web.Internal;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.Web.ASPxGauges.Gauges.Circular;
using DevExpress.Web.ASPxGauges.Gauges.Linear;
namespace DevExpress.Web.ASPxGauges.Base {
	[TypeConverter(typeof(ComponentCollectionObjectTypeConverter))]
	[Editor("DevExpress.Web.ASPxGauges.Design.ComponentCollectionTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class ComponentCollectionWeb<T> : BaseChangeableList<T>, IStateManagedHierarchyObjectCollection
		where T : class, INamed, IComponent, ISupportAcceptOrder, IStateManagedHierarchyObject {
		protected override void OnElementAdded(T element) {
			base.OnElementAdded(element);
			element.Disposed += OnComponentDisposed;
			if(((IStateManagedHierarchyObject)this).IsTrackingViewState)
				((IStateManagedHierarchyObject)element).TrackViewState();
		}
		protected override void OnElementRemoved(T element) {
			element.Disposed -= OnComponentDisposed;
			base.OnElementRemoved(element);
		}
		void OnComponentDisposed(object sender, EventArgs ea) {
			RaiseCollectionChanged(new DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<T>(sender as T, ElementChangedType.ElementDisposed));
			if(List != null) Remove(sender as T);
		}
		#region IStateManagedHierarchyObjectCollection
		IStateManagedHierarchyObject IStateManagedHierarchyObjectCollection.this[int i] {
			get { return List[i]; }
		}
		void IStateManagedHierarchyObjectCollection.Add(IStateManagedHierarchyObject obj) {
			base.Add(obj as T);
		}
		#endregion IStateManagedHierarchyObjectCollection
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectCollectionHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectCollectionHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectCollectionHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectCollectionExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectCollectionExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectCollectionExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	public class ComponentCollectionObjectTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			return "<Components...>";
		}
	}
	public static class ScaleAffinityHelper {
		public static void CheckScaleID<T>(BaseChangeableList<T> components)
			where T : class, IScaleComponent, IScaleDependentElement, ISupportAcceptOrder {
			for(int i = 0; i < components.Count; i++) {
				if(string.IsNullOrEmpty(components[i].ScaleID) && !components[i].Scale.IsEmpty) {
					components[i].ScaleID = ((INamed)components[i].Scale).Name;
				}
			}
		}
		public static void ResolveArcScaleAffinity<T>(BaseChangeableList<T> components, ArcScaleComponentCollection scales)
			where T : class, IArcScaleComponent, IScaleDependentElement, ISupportAcceptOrder {
			for(int i = 0; i < components.Count; i++) {
				if(!components[i].Scale.IsEmpty || string.IsNullOrEmpty(components[i].ScaleID)) continue;
				components[i].ArcScale = FindArcScaleByID(components[i].ScaleID, scales);
			}
		}
		public static void ResolveLinearScaleAffinity<T>(BaseChangeableList<T> components, LinearScaleComponentCollection scales)
			where T : class, ILinearScaleComponent, IScaleDependentElement, ISupportAcceptOrder {
			for(int i = 0; i < components.Count; i++) {
				if(!components[i].Scale.IsEmpty || string.IsNullOrEmpty(components[i].ScaleID)) continue;
				components[i].LinearScale = FindLinearScaleByID(components[i].ScaleID, scales);
			}
		}
		public static void ResolveIndicatorArcScaleAffinity<T>(BaseChangeableList<T> components, ArcScaleComponentCollection scales)
			where T : class, IScaleStateIndicator, IScaleDependentElement, ISupportAcceptOrder {
			for(int i = 0; i < components.Count; i++) {
				if(components[i].IndicatorScale != null || string.IsNullOrEmpty(components[i].ScaleID)) continue;
				components[i].IndicatorScale = FindArcScaleByID(components[i].ScaleID, scales);
			}
		}
		public static void ResolveIndicatorLinearScaleAffinity<T>(BaseChangeableList<T> components, LinearScaleComponentCollection scales)
			where T : class, IScaleStateIndicator, IScaleDependentElement, ISupportAcceptOrder {
			for(int i = 0; i < components.Count; i++) {
				if(components[i].IndicatorScale != null || string.IsNullOrEmpty(components[i].ScaleID)) continue;
				components[i].IndicatorScale = FindLinearScaleByID(components[i].ScaleID, scales);
			}
		}
		static IArcScale FindArcScaleByID(string id, ArcScaleComponentCollection scales) {
			for(int i = 0; i < scales.Count; i++) {
				if(scales[i].Name == id) return scales[i];
			}
			return ScaleFactory.EmptyArcScale;
		}
		static ILinearScale FindLinearScaleByID(string id, LinearScaleComponentCollection scales) {
			for(int i = 0; i < scales.Count; i++) {
				if(scales[i].Name == id) return scales[i];
			}
			return ScaleFactory.EmptyLinearScale;
		}
	}
}
