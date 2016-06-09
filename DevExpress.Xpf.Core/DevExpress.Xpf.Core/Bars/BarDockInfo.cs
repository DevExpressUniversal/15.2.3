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
using System.Windows;
using DevExpress.Utils.Serializing;
using System.Windows.Markup;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
using System.Linq;
namespace DevExpress.Xpf.Bars {
	public enum BarContainerType { None, Left, Top, Right, Bottom, Floating }
	public class BarDockInfo : DependencyObject {
		#region static
		public static readonly DependencyProperty RowProperty;
		public static readonly DependencyProperty ColumnProperty;
		public static readonly DependencyProperty OffsetProperty;
		public static readonly DependencyProperty ContainerNameProperty;
		public static readonly DependencyProperty ContainerProperty;
		public static readonly DependencyProperty FloatBarOffsetProperty;
		public static readonly DependencyProperty FloatBarWidthProperty;
		public static readonly DependencyProperty ShowHeaderInFloatBarProperty;
		public static readonly DependencyProperty ContainerTypeProperty;
		public static readonly DependencyProperty BarProperty;
		public const string FloatingContainerName = "PART_FloatingBarContainer";
		static readonly HashSet<DependencyProperty> serializableProperties;
		static BarDockInfo() {
			ColumnProperty = DependencyPropertyManager.Register("Column", typeof(int), typeof(BarDockInfo), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnColumnPropertyChanged)));
			RowProperty = DependencyPropertyManager.Register("Row", typeof(int), typeof(BarDockInfo), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnRowPropertyChanged)));
			OffsetProperty = DependencyPropertyManager.Register("Offset", typeof(double), typeof(BarDockInfo), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnOffsetPropertyChanged)));
			ContainerNameProperty = DependencyPropertyManager.Register("ContainerName", typeof(string), typeof(BarDockInfo), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnContainerNamePropertyChanged)));
			ContainerProperty = DependencyPropertyManager.Register("Container", typeof(BarContainerControl), typeof(BarDockInfo), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnContainerPropertyChanged)));
			FloatBarOffsetProperty = DependencyPropertyManager.Register("FloatBarOffset", typeof(Point), typeof(BarDockInfo), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnFloatBarOffsetPropertyChanged)));
			FloatBarWidthProperty = DependencyPropertyManager.Register("FloatBarWidth", typeof(double), typeof(BarDockInfo), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnFloatBarWidthPropertyChanged)));
			ShowHeaderInFloatBarProperty = DependencyPropertyManager.Register("ShowHeaderInFloatBar", typeof(bool), typeof(BarDockInfo), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowHeaderInFloatBarPropertyChanged)));
			ContainerTypeProperty = DependencyPropertyManager.Register("ContainerType", typeof(BarContainerType), typeof(BarDockInfo), new FrameworkPropertyMetadata(BarContainerType.Top, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnContainerTypePropertyChanged)));
			BarProperty = DependencyPropertyManager.Register("Bar", typeof(Bar), typeof(BarDockInfo), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBarPropertyChanged)));
			serializableProperties = new HashSet<DependencyProperty>() { 
				RowProperty, 
				ColumnProperty,
				OffsetProperty,
				ContainerNameProperty,
				FloatBarOffsetProperty,
				FloatBarWidthProperty,
				ShowHeaderInFloatBarProperty,
				ContainerTypeProperty,
			};
		}
		protected static void OnBarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnBarChanged(e);
		}
		protected static void OnContainerTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnContainerTypeChanged(e);
		}
		protected static void OnColumnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnColumnChanged(e);
		}
		protected static void OnRowPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnRowChanged(e);
		}
		protected static void OnOffsetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnOffsetChanged(e);
		}
		protected static void OnContainerNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnContainerNameChanged(e);
		}
		protected static void OnContainerPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnContainerChanged(e);
		}
		protected static void OnFloatBarOffsetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnFloatBarOffsetChanged(e);
		}
		protected static void OnFloatBarWidthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnFloatBarWidthChanged(e);
		}
		protected static void OnShowHeaderInFloatBarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarDockInfo)obj).OnShowHeaderInFloatBarChanged(e);
		}
		#endregion
		public event EventHandler ContainerTypeChanged;
		WeakList<EventHandler> handlersWeakContainerTypeChanged = new Bars.Native.WeakList<EventHandler>();
		public event EventHandler WeakContainerTypeChanged {
			add { handlersWeakContainerTypeChanged.Add(value); }
			remove { handlersWeakContainerTypeChanged.Remove(value); }
		}
		void RaiseWeakContainerTypeChanged(EventArgs args) {
			foreach (EventHandler e in handlersWeakContainerTypeChanged)
				e(this, args);
		}
		public BarDockInfo() {
			locker = new Locker();
			locker.Unlocked += OnUnlocked;
			Data = new BarDockInfoData(this);
		}		
		public BarDockInfo(Bar bar) : this() {
			Bar = bar;
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			AddCustomization(e.Property, e.OldValue, e.NewValue);
		}
		void AddCustomization(DependencyProperty property, object oldValue, object newValue) {
			if (serializableProperties.Contains(property)) {
				var manager = Bar.With(x => x.Manager);
				if (manager == null)
					return;
				manager.RuntimeCustomizations.Add(new RuntimePropertyCustomization(Bar) {
					Overwrite = true,
					PropertyName = "DockInfo." + property.Name,
					OldValue = oldValue,
					NewValue = newValue
				});
			}
		}
		protected internal BarDockInfoData Data { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoRow")]
#endif
		public int Row {
			get { return (int)GetValue(RowProperty); }
			set { SetValue(RowProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoColumn")]
#endif
		public int Column {
			get { return (int)GetValue(ColumnProperty); }
			set { SetValue(ColumnProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoOffset")]
#endif
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarDockInfoContainerName"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ContainerName {
			get { return (string)GetValue(ContainerNameProperty); }
			set { SetValue(ContainerNameProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoFloatBarOffset")]
#endif
		public Point FloatBarOffset {
			get { return (Point)GetValue(FloatBarOffsetProperty); }
			set { SetValue(FloatBarOffsetProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoFloatBarWidth")]
#endif
		public double FloatBarWidth {
			get { return (double)GetValue(FloatBarWidthProperty); }
			set { SetValue(FloatBarWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoShowHeaderInFloatBar")]
#endif
		public bool ShowHeaderInFloatBar {
			get { return (bool)GetValue(ShowHeaderInFloatBarProperty); }
			set { SetValue(ShowHeaderInFloatBarProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoShowCloseButtonInFloatBar")]
#endif
public bool ShowCloseButtonInFloatBar {
			get {
				if(Bar == null) return true;
				return Bar.IsAllowHide; 
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoContainerType")]
#endif
		public BarContainerType ContainerType {
			get { return (BarContainerType)GetValue(ContainerTypeProperty); }
			set { SetValue(ContainerTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarDockInfoContainer"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarContainerControl Container {
			get { return (BarContainerControl)GetValue(ContainerProperty); }
			set { SetValue(ContainerProperty, value); }
		}
		BarControl barControl;
		protected internal BarControl BarControl {
			get { return barControl; }
			set {
				if(BarControl == value)
					return;
				OnBarControlChanging();
				barControl = value;
				OnBarControlChanged();
			} 
		}
		protected virtual void OnBarControlChanging() {
			if(BarControl != null) {
				BarControl.Loaded -= OnBarControlLoaded;
				if(Bar.With(x=>x.ToolBar).With(x => x.BarControl)!=BarControl)
					BarControl.OnClear();
			}
		}
		protected virtual void UpdateBarControlState() {
			if(BarControl != null && Bar != null)
				BarControl.IsEnabled = Bar.IsEnabled;
		}
		protected virtual void OnBarControlChanged() {
			UpdateBarControlState();
			if(BarControl != null) {
				if (Bar.With(x => x.ToolBar).With(x => x.BarControl) == BarControl) {
					BarControl.UpdateBarControlProperties();
				}
				BarControl.Loaded += OnBarControlLoaded;
			}
		}
		protected virtual void OnBarControlLoaded(object sender, RoutedEventArgs e) {
			Bar.RaiseBarControlLoadedEvent();
		}
		internal bool ContinueDragging { get; set; }
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarDockInfoBar"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bar Bar {
			get { return (Bar)GetValue(BarProperty); }
			set { SetValue(BarProperty, value); }
		}
		Rect barRect = Rect.Empty;
		FloatingBarPopup floatingPopup;
		protected internal FloatingBarPopup FloatingPopup {
			get { return floatingPopup; }
			set {
				if (floatingPopup == value)
					return;
				var oldvalue = floatingPopup;
				floatingPopup = value;
				OnFloatingPopupChanged(oldvalue, value);
			}
		}
		void OnFloatingPopupChanged(FloatingBarPopup oldvalue, FloatingBarPopup newValue) {
			RemoveFloatingPopupFromLogicalTree(oldvalue, Bar);
			AddFloatingPopupToLogicalTree(newValue, Bar);
		}
		void AddFloatingPopupToLogicalTree(FloatingBarPopup value, Bar owner) {
			if (value == null || owner==null || value.Parent != null)
				return;
			((ILogicalChildrenContainer)owner).AddLogicalChild(value);
		}
		void RemoveFloatingPopupFromLogicalTree(FloatingBarPopup value, Bar owner) {
			if (value==null || owner == null || value.Parent != Bar)
				return;
			((ILogicalChildrenContainer)owner).RemoveLogicalChild(value);
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarDockInfoBarRect")]
#endif
		public Rect BarRect {
			get { return barRect; }
			internal set { barRect = value; }
		}
		protected internal Point DragStartPoint { get; set; }
		protected virtual void OnBarChanged(DependencyPropertyChangedEventArgs e) {
			Bar oldBar = (Bar)e.OldValue;
			Bar newBar = (Bar)e.NewValue;
			if (oldBar != null) {
				RemoveFloatingPopupFromLogicalTree(FloatingPopup, oldBar);
				BarNameScope.GetService<IElementRegistratorService>(oldBar).NameChanged(oldBar, BarRegistratorKeys.BarNameKey, ContainerName, String.Empty);
				BarNameScope.GetService<IElementRegistratorService>(oldBar).NameChanged(oldBar, BarRegistratorKeys.BarTypeKey, ContainerType, BarContainerType.None);
			}
			if (newBar != null) {
				AddFloatingPopupToLogicalTree(FloatingPopup, newBar);
				BarNameScope.GetService<IElementRegistratorService>(newBar).NameChanged(newBar, BarRegistratorKeys.BarNameKey, String.Empty, ContainerName);
				BarNameScope.GetService<IElementRegistratorService>(newBar).NameChanged(newBar, BarRegistratorKeys.BarTypeKey, BarContainerType.None, ContainerType);
			}
			UpdateBarControlState();
		}		
		protected virtual void OnColumnChanged(DependencyPropertyChangedEventArgs e) {
			BarControl.Do(x => x.OnLayoutPropertyChanged());
		}
		protected virtual void OnRowChanged(DependencyPropertyChangedEventArgs e) {
			BarControl.Do(x => x.OnLayoutPropertyChanged());
		}
		protected virtual void OnOffsetChanged(DependencyPropertyChangedEventArgs e) {
			BarControl.Do(x => x.OnLayoutPropertyChanged());
		}
		protected virtual void OnFloatBarWidthChanged(DependencyPropertyChangedEventArgs e) {
			if(Container != null && Container.IsFloating && Container.OwnerPopup != null) {
				if (FloatBarWidth != (double)BarDockInfo.FloatBarWidthProperty.GetMetadata(typeof(BarDockInfo)).DefaultValue) 
					Container.OwnerPopup.Width = FloatBarWidth >= 0d ? FloatBarWidth : 0d;
			}
		}
		protected virtual void OnFloatBarOffsetChanged(DependencyPropertyChangedEventArgs e) {
			if(Container == null)
				return;
			if(Container.IsFloating && Container.OwnerPopup != null) {
				Container.OwnerPopup.HorizontalOffset = FloatBarOffset.X;
				Container.OwnerPopup.VerticalOffset = FloatBarOffset.Y;
			}
		}
		protected virtual void OnShowHeaderInFloatBarChanged(DependencyPropertyChangedEventArgs e) {
			if(Container != null && Container.IsFloating && Container.OwnerPopup != null) {
				Container.OwnerPopup.ShowHeader = ShowHeaderInFloatBar;
			}
		}
		protected internal virtual void OnShowCloseButtonInFloatBarChanged() {
			if(Container != null && Container.IsFloating && Container.OwnerPopup != null) {
				Container.OwnerPopup.ShowCloseButton = ShowCloseButtonInFloatBar;
			}
		}
		protected virtual void OnContainerNameChanged(DependencyPropertyChangedEventArgs e) {			
			if (Bar != null)
				BarNameScope.GetService<IElementRegistratorService>(Bar).NameChanged(Bar, BarRegistratorKeys.BarNameKey, e.OldValue, e.NewValue);
			TryMakeFloating();
		}
		protected internal virtual void MakeBarFloating(bool shouldStartDrag) {
			if (ContainerType != BarContainerType.Floating && !shouldStartDrag)
				return;
			FloatingBarPopup popup = CreateFloatingBar();
			if(FloatBarWidth != (double)BarDockInfo.FloatBarWidthProperty.GetMetadata(typeof(BarDockInfo)).DefaultValue)
				popup.Width = FloatBarWidth;
			if (FloatBarOffset != (Point)BarDockInfo.FloatBarOffsetProperty.GetMetadata(typeof(BarDockInfo)).DefaultValue) {
				popup.VerticalOffset = FloatBarOffset.Y;
				popup.HorizontalOffset = FloatBarOffset.X;
			}			
			popup.ShouldStartDrag = shouldStartDrag;
			popup.IsOpen = Bar.Visible;
		}
		protected virtual FloatingBarPopup CreateFloatingBar() {
			FloatingBarPopup popup = BarLayoutCalculator2.CreateFloatingBar(this, FloatBarOffset);
			popup.ShowHeader = ShowHeaderInFloatBar;
			popup.ShowCloseButton = Bar.IsAllowHide;
			return popup;
		}
		protected bool IsFloating {
			get { return ContainerName == FloatingContainerName; }
		}
		readonly Locker locker;
		protected internal IDisposable Lock() {
			return locker.Lock();
		}
		protected virtual void OnUnlocked(object sender, EventArgs e) {
			if (Bar == null)
				return;
			BarNameScope.GetService<IElementRegistratorService>(Bar).Changed(Bar, BarRegistratorKeys.BarNameKey);
			BarNameScope.GetService<IElementRegistratorService>(Bar).Changed(Bar, BarRegistratorKeys.BarTypeKey);			
		}
		protected internal bool IsLocked { get { return locker.IsLocked; } }
		protected virtual void OnContainerChanged(DependencyPropertyChangedEventArgs e) {
			var oldContainer = (BarContainerControl)e.OldValue;
			try {
				if (oldContainer != null && Bar != null) {
					oldContainer.Unlink(Bar);
					if (oldContainer.IsFloating) {
						var fbp = (oldContainer as FloatingBarContainerControl).OwnerPopup;
						if (fbp != null)
							fbp.Bar = null;
					}
				}					
				if (Container == null)
					return;
				if (Bar != null)
					Container.Link(Bar);
					ContainerType = Container.ContainerType;
					ContainerName = Container.Name;
					AddCustomization(ContainerNameProperty, ContainerName, ContainerName);					
			} finally {
				BarControl.Do(x => x.UpdateBarControlProperties());
			}	   
		}		
		protected virtual void OnContainerTypeChanged(DependencyPropertyChangedEventArgs e) {
			if(ContainerTypeChanged != null)
				ContainerTypeChanged(this, new EventArgs());
			RaiseWeakContainerTypeChanged(new EventArgs());
			if (Bar != null) {
				BarNameScope.GetService<IElementRegistratorService>(Bar).NameChanged(Bar, BarRegistratorKeys.BarTypeKey, e.OldValue, e.NewValue);
				TryMakeFloating();					
			}			
		}
		protected virtual void TryMakeFloating() {
			if (Container != null && Container.IsFloating)
				return;
			if (ContainerType != BarContainerType.Floating)
				return;
			if (!String.IsNullOrEmpty(ContainerName) && ContainerName != FloatingContainerName)
				return;
			MakeBarFloating(false);
		}
		protected internal virtual int Compare(BarDockInfo info) {
			if(info.Row == Row && info.Column == Column && info.Offset == Offset) return 0;
			if(info.Row > Row) return -1;
			if(info.Row < Row) return 1;
			if(info.Column > Column) return -1;
			if(info.Column < Column) return 1;
			if(info.Offset > Offset) return -1;
			if(info.Offset < Offset) return 1;
			return 0;
		}		
		public void Reset() {
			ContainerType = BarContainerType.None;
			Container = null;
			Row = 0;
			Column = 0;
			Offset = 0;
		}				
	}
	public class BarDockInfoComparer : IComparer<BarDockInfo> {
		#region IComparer<BarDockInfo> Members
		public int Compare(BarDockInfo x, BarDockInfo y) {
			return x.Compare(y);
		}
		#endregion
	}
	public class DockInfoExtension : MarkupExtension {
		public BarContainerType ContainerType { get; set; }
		public string ContainerName { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public double Offset { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			BarDockInfo info = new BarDockInfo();
			info.ContainerType = ContainerType;
			info.ContainerName = ContainerName;
			info.Row = Row;
			info.Column = Column;
			info.Offset = Offset;
			return info;
		}
	}
}
namespace DevExpress.Xpf.Bars.Native {
	public class BarDockInfoData {		
		public int Row { get; set; }
		public int Column { get; set; }
		public double Offset { get; set; }
		public string ContainerName { get; set; }
		public BarContainerType ContainerType { get; set; }
		public BarDockInfo DockInfo { get; set; }
		public bool IsValid { get; set; }
		public BarDockInfoData(BarDockInfo dockInfo) {
			this.DockInfo = dockInfo;
			IsValid = false;
		}
		public void Save() {
			if (DockInfo.IsLocked)
				return;
			Row = DockInfo.Row;
			Column = DockInfo.Column;
			Offset = DockInfo.Offset;
			ContainerName = DockInfo.ContainerName;
			ContainerType = DockInfo.ContainerType;
			IsValid = true;
		}
		public bool Restore() {
			if (!IsValid)
				return false;
			using (DockInfo.Lock()) {
				DockInfo.Container.Unlink(DockInfo.Bar);
				DockInfo.Row = Row;
				DockInfo.Column = Column;
				DockInfo.Offset = Offset;
				DockInfo.ContainerName = ContainerName;
				DockInfo.ContainerType = ContainerType;
			}
			IsValid = false;
			return true;
		}
	}
}
