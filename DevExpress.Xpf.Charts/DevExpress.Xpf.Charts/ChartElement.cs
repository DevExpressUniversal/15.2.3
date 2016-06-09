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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Charts {
	public abstract class ChartNonVisualElement : DXFrameworkContentElement, IChartElement {
		readonly Locker changedLocker = new Locker();
		IChartElement owner;
		internal Locker ChangedLocker { get { return changedLocker; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		#region IChartElement implementation
		IChartElement IOwnedElement.Owner {
			get { return owner; }
			set { ChangeOwner(owner, value); }
		}
		ViewController INotificationOwner.Controller { 
			get { return owner == null ? null : owner.Controller; } 
		}
		void IChartElement.AddChild(object child) {
		}
		void IChartElement.RemoveChild(object child) {
		}
		bool IChartElement.Changed(ChartUpdate args) {
			if (!changedLocker.IsLocked) {
				changedLocker.Lock();
				try {
					if (owner != null)
						owner.Changed(args);
				}
				finally {
					changedLocker.Unlock();
				}
			}
			return true;
		}
		#endregion
		protected internal virtual void ChangeOwner(IChartElement oldOwner, IChartElement newOwner) {
			try {
				changedLocker.Lock();
				if (oldOwner != null)
					oldOwner.RemoveChild(this);
				owner = newOwner;
				if (newOwner != null)
					newOwner.AddChild(this);
			}
			finally {
				changedLocker.Unlock();
			}
		}
	}
	public abstract class ChartElementBase : Control {
		static ChartElementBase() {
			FocusableProperty.OverrideMetadata(typeof(ChartElementBase), new FrameworkPropertyMetadata(false));
		}
	}
	public abstract class ChartElement : ChartElementBase, IChartElement {
		readonly List<object> logicalChildren = new List<object>();
		readonly Locker changedLocker = new Locker();
		IChartElement owner;
		protected virtual ViewController Controller { 
			get { return owner == null ? null : owner.Controller; } 
		}
		protected virtual void AddChild(object child) {
			if (!logicalChildren.Contains(child)) {
				logicalChildren.Add(child);
				AddLogicalChild(child);
			}
		}
		protected virtual void RemoveChild(object child) {
			if (logicalChildren.Contains(child)) {
				logicalChildren.Remove(child);
				RemoveLogicalChild(child);
			}
		}
		protected override IEnumerator LogicalChildren { 
			get { return logicalChildren.GetEnumerator(); } 
		}
		internal Locker ChangedLocker { get { return changedLocker; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		#region Hidden Properties
		[DXBrowsable(false), DXHelpExclude(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double Height { get { return base.Height; } set { base.Height = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalAlignment { get { return base.HorizontalAlignment; } set { base.HorizontalAlignment = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsEnabled { get { return base.IsEnabled; } set { base.IsEnabled = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsHitTestVisible { get { return base.IsHitTestVisible; } set { base.IsHitTestVisible = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsTabStop { get { return base.IsTabStop; } set { base.IsTabStop = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double MaxHeight { get { return base.MaxHeight; } set { base.MaxHeight = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double MaxWidth { get { return base.MaxWidth; } set { base.MaxWidth = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double MinHeight { get { return base.MinHeight; } set { base.MinHeight = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int TabIndex { get { return base.TabIndex; } set { base.TabIndex = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalAlignment { get { return base.VerticalAlignment; } set { base.VerticalAlignment = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double Width { get { return base.Width; } set { base.Width = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FlowDirection FlowDirection { get { return base.FlowDirection; } set { base.FlowDirection = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool UseLayoutRounding { get { return base.UseLayoutRounding; } set { base.UseLayoutRounding = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BindingGroup BindingGroup { get { return base.BindingGroup; } set { base.BindingGroup = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ClipToBounds { get { return base.ClipToBounds; } set { base.ClipToBounds = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ForceCursor { get { return base.ForceCursor; } set { base.ForceCursor = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool OverridesDefaultStyle { get { return base.OverridesDefaultStyle; } set { base.OverridesDefaultStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool SnapsToDevicePixels { get { return base.SnapsToDevicePixels; } set { base.SnapsToDevicePixels = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Uid { get { return base.Uid; } set { base.Uid = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style FocusVisualStyle { get { return base.FocusVisualStyle; } set { base.FocusVisualStyle = value; } }
		#endregion
		#region IChartElement implementation
		IChartElement IOwnedElement.Owner { get { return owner; } set { ChangeOwner(owner, value); } }
		void IChartElement.AddChild(object child) {
			AddChild(child);
		}
		void IChartElement.RemoveChild(object child) {
			RemoveChild(child);
		}
		bool IChartElement.Changed(ChartUpdate updateInfo) {
			if (!changedLocker.IsLocked) {
				changedLocker.Lock();
				try {
					if (ProcessChanging(updateInfo)) {
						if(owner != null)
							return owner.Changed(updateInfo);
						return true;
					}
					return false; 
				}
				finally {
					changedLocker.Unlock();
				}
			}
			return true;
		}
		ViewController INotificationOwner.Controller { get { return Controller; } }
		#endregion
		protected virtual bool ProcessChanging(ChartUpdate updateInfo) { return true; }
		protected internal virtual void ChangeOwner(IChartElement oldOwner, IChartElement newOwner) {
			if (oldOwner == newOwner)
				return;
			try {
				changedLocker.Lock();
				if (oldOwner != null)
					oldOwner.RemoveChild(this);
				owner = newOwner;
				if (newOwner != null)
					newOwner.AddChild(this);
			}
			finally {
				changedLocker.Unlock();
			}
		}
		public override void BeginInit() {
			base.BeginInit();
			changedLocker.Lock();
		}
		public override void EndInit() {
			base.EndInit();
			changedLocker.Unlock();
		}
	}
	public class ChartItemsControl : ItemsControl {
		static ChartItemsControl() {
			FocusableProperty.OverrideMetadata(typeof(ChartItemsControl), new FrameworkPropertyMetadata(false));
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ItemCollection Items {
			get { return base.Items; }
		}
		protected void DetachOldItemsParents() { }
		public override void OnApplyTemplate() {
			DetachOldItemsParents();
			base.OnApplyTemplate();
		}
	}
	public class ChartContentControl : ContentControl {
		static ChartContentControl() {
			FocusableProperty.OverrideMetadata(typeof(ChartContentControl), new FrameworkPropertyMetadata(false));
		}
	}
	internal static class ChartElementHelper {
		public static void ChangeOwner(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IChartElement ownerElement = d as IChartElement;
			if (ownerElement != null && !Object.ReferenceEquals(e.NewValue, e.OldValue)) {
				IChartElement oldElement = e.OldValue as IChartElement;
				if (oldElement != null)
					oldElement.Owner = null;
				IChartElement newElement = e.NewValue as IChartElement;
				if (newElement != null)
					newElement.Owner = ownerElement;
			}
		}
		public static void Update(DependencyObject d) {
			Update(d, ChartElementChange.None);
		}
		public static void Update(DependencyObject d, ChartElementChange change) {
			Update(d, new ChartUpdate(d, change));			
		}
		public static void Update(DependencyObject d, ChartUpdate updateInfo) {
			Update(d as IChartElement, updateInfo);
		}
		public static void Update(DependencyObject d, ChartUpdateInfoBase updateInfo) {
			Update(d, new ChartUpdate(updateInfo));
		}
		public static void Update(IChartElement element, ChartElementChange change) {
			Update(element, new ChartUpdate(element, change));
		}
		public static void Update(IChartElement element, ChartUpdate updateInfo) {
			if (element != null) {
				ViewController controller = element.Controller;
				if (element.Changed(updateInfo) && controller != null) {
					controller.Notify(updateInfo);
				}
			}
		}
		public static void Update(IChartElement element, ChartUpdateInfoBase updateInfo) {
			Update(element, new ChartUpdate(updateInfo));
		}
		public static void Update(IChartElement element, ChartUpdateInfoBase updateInfo, ChartElementChange change) {
			Update(element, new ChartUpdate(change, updateInfo));
		}
		public static void Update(DependencyObject d, ChartUpdateInfoBase updateInfo, ChartElementChange change) {
			Update(d as IChartElement, updateInfo, change);
		}
		public static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e, ChartElementChange change) {
			if (e.NewValue != e.OldValue)
				Update(d, change);
		}
		public static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Update(d, e, ChartElementChange.None);
		}
		public static void UpdateWithClearDiagramCache(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Update(d, e, ChartElementChange.ClearDiagramCache);
		}
		public static void UpdateWithClearDiagramCache(DependencyObject d) {
			Update(d, ChartElementChange.ClearDiagramCache);
		}
		public static void UpdateChartPaletteWithClearDiagramCache(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Update(d, ChartElementChange.UpdateChartPalette | ChartElementChange.ClearDiagramCache);
		}
		public static void UpdateSeriesBindingWithClearDiagramCache(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Update(d, ChartElementChange.UpdateSeriesBinding | ChartElementChange.ClearDiagramCache);
		}
		public static void ChangeOwnerAndUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e, ChartElementChange change) {
			ChangeOwner(d, e);
			Update(d, e, change);
		}
		public static void ChangeOwnerAndUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e, ChartUpdateInfoBase updateInfo) {
			ChangeOwner(d, e);
			Update(d, updateInfo);
		}
		public static void ChangeOwnerAndUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChangeOwnerAndUpdate(d, e, ChartElementChange.None);
		}
		public static void ChangeOwnerAndUpdateWithClearDiagramCache(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache);
		}
		public static void ChangeOwnerAndUpdateXYDiagram2DItems(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.UpdateXYDiagram2DItems);
		}
		public static T CreateInstance<T>(IChartElement owner) where T : IOwnedElement, new() {
			T t = new T();
			t.Owner = owner;
			return t;
		}
		public static OwnerType GetOwner<OwnerType>(IOwnedElement element) where OwnerType : class, IChartElement {
			if (element.Owner == null)
				return null;
			IChartElement owner = element.Owner;
			while (owner != null) {
				Type type = typeof(OwnerType);
				if (type.IsAssignableFrom(owner.GetType()))
					return owner as OwnerType;
				owner = owner.Owner;
			}
			return null;
		}
	}
	public class ChartContentPresenter : ContentPresenter { }
	public class ChartDoubleCollectionConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class EmptyUpdateInfo : ChartUpdateInfoBase {
		public EmptyUpdateInfo(object sender) : base(sender) { }
	}
	public class ChartUpdate {
		ChartElementChange change;
		public ChartElementChange Change {
			get { return change; }
			set { change = value; }
		}
		public ChartUpdateInfoBase UpdateInfo { get; private set; }
		public bool ShouldUpdateSeriesBinding { get { return (change & ChartElementChange.UpdateSeriesBinding) != 0; } }
		public bool ShouldUpdateChartBinding { get { return (change & ChartElementChange.UpdateChartBinding) != 0; } }
		public bool ShouldUpdateAutoSeries { get { return (change & ChartElementChange.UpdateAutoSeries) != 0; } }
		public bool ShouldUpdateAutoSeriesProperties { get { return (change & ChartElementChange.UpdateAutoSeriesProperties) != 0; } }
		public ChartUpdate(object sender, ChartElementChange change) : this(change, new EmptyUpdateInfo(sender)) { 
		}
		public ChartUpdate(ChartUpdateInfoBase updateInfo)
			: this(ChartElementChange.None, updateInfo) {
		}
		public ChartUpdate(ChartElementChange change, ChartUpdateInfoBase updateInfo) { 
			this.change = change;
			this.UpdateInfo = updateInfo;
		}
	}
	[Flags]
	public enum ChartElementChange {
		None = 0,
		ClearDiagramCache = 1,
		CollectionModified = 2,
		UpdateSeriesBinding = 8,
		UpdateChartBinding = 16,
		UpdateAutoSeries = 32,
		UpdateAutoSeriesProperties = 64,
		UpdateXYDiagram2DItems = 128,
		Diagram3DOnly = 256,
		UpdateActualPanes = 512,
		UpdatePanesItems = 1024,
		UpdateIndicators = 2048,
		UpdateChartPalette = 4096
	}	
}
