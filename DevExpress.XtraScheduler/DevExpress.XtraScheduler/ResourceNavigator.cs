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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.NavigatorButtons;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraScheduler.UI {
	#region ResourceNavigatorControl
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class ResourceNavigatorControl : XtraUserControl {
		#region Fields
		SchedulerResourceNavigator navigator;
		ScrollBarBase scrollBar;
		ContainerPanelControl containerPanel;
		int cornerSize;
		bool vertical;
		FieldInfo layoutSuspendCount;
		#endregion
		public ResourceNavigatorControl() {
			this.layoutSuspendCount = typeof(Control).GetField("layoutSuspendCount", BindingFlags.Instance | BindingFlags.NonPublic);
			this.Visible = false;
			this.navigator = new SchedulerResourceNavigator();
			navigator.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			this.containerPanel = new ContainerPanelControl();
			ContainerPanel.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			ContainerPanel.BorderStyle = BorderStyles.NoBorder;
			ContainerPanel.Dock = DockStyle.Fill;
			Controls.Add(ContainerPanel);
			this.navigator.BorderStyle = BorderStyles.NoBorder;
			this.navigator.Visible = true;
			this.navigator.ShowToolTips = true;
			ContainerPanel.Controls.Add(navigator);
			CreateScrollBar();
			SubscribeScrollbarEvents();
		}
		#region Properties
		internal ToolTipController ToolTipController { get { return Navigator.ToolTipController; } set { Navigator.ToolTipController = value; } }
		public SchedulerResourceNavigator Navigator { get { return navigator; } }
		[Category(SRCategoryNames.Behavior), DefaultValue(true)]
		public bool ShowToolTips { get { return Navigator.ShowToolTips; } set { Navigator.ShowToolTips = value; } }
		public ScrollBarBase ScrollBar { get { return scrollBar; } }
		public int CornerSize {
			get { return cornerSize; }
			set {
				if (cornerSize == value)
					return;
				cornerSize = value;
				RecalcLayout();
			}
		}
		public bool Vertical {
			get { return vertical; }
			set {
				if (vertical == value)
					return;
				UnsubscribeScrollbarEvents();
				DestroyScrollBar();
				vertical = value;
				UpdateNavigatorOrientation(vertical);
				CreateScrollBar();
				SubscribeScrollbarEvents();
				RecalcLayout();
			}
		}
		protected virtual void UpdateNavigatorOrientation(bool vertical) {
			Navigator.Orientation = vertical ? Orientation.Vertical : Orientation.Horizontal;
		}
		protected internal bool IsLayoutSuspended { get { return (byte)layoutSuspendCount.GetValue(this) > 0; } }
		protected internal ContainerPanelControl ContainerPanel { get { return containerPanel; } }
		#endregion
		#region Events
		#region ScrollValueChanged
		EventHandler onScrollValueChanged;
		protected internal event EventHandler ScrollValueChanged { add { onScrollValueChanged += value; } remove { onScrollValueChanged -= value; } }
		protected internal virtual void RaiseScrollValueChanged(object sender, EventArgs e) {
			if (onScrollValueChanged != null) {
				if (IsScrollPossible())
					onScrollValueChanged(sender, e);
				navigator.Buttons.UpdateButtonsState();
			}
		}
		#endregion
		#region ScrollHappend
		ScrollEventHandler onScrollHappend;
		protected internal event ScrollEventHandler ScrollHappend { add { onScrollHappend += value; } remove { onScrollHappend -= value; } }
		protected internal virtual void RaiseScrollHappend(object sender, ScrollEventArgs e) {
			if (onScrollHappend != null)
				onScrollHappend(sender, e);
		}
		#endregion
		#endregion
		#region SubscribeScrollbarEvents
		protected internal virtual void SubscribeScrollbarEvents() {
			scrollBar.ValueChanged += new EventHandler(RaiseScrollValueChanged);
			scrollBar.Scroll += new ScrollEventHandler(OnScrollBarScroll);
		}
		#endregion
		#region UnsubscribeScrollbarEvents
		protected internal virtual void UnsubscribeScrollbarEvents() {
			scrollBar.ValueChanged -= new EventHandler(RaiseScrollValueChanged);
			scrollBar.Scroll -= new ScrollEventHandler(OnScrollBarScroll);
		}
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (navigator != null) {
						navigator.Dispose();
						navigator = null;
					}
					if (scrollBar != null) {
						UnsubscribeScrollbarEvents();
						DestroyScrollBar();
					}
					if (containerPanel != null) {
						containerPanel.Dispose();
						containerPanel = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		void OnScrollBarScroll(object sender, ScrollEventArgs e) {
			if (IsScrollPossible())
				RaiseScrollHappend(sender, e);
		}
		bool IsScrollPossible() {
			if (scrollBar == null)
				return false;
			if (scrollBar.LargeChange > scrollBar.Maximum)
				return false;
			else
				return true;
		}
		protected internal virtual void CreateScrollBar() {
			if (Vertical)
				this.scrollBar = new DevExpress.XtraEditors.VScrollBar();
			else
				this.scrollBar = new DevExpress.XtraEditors.HScrollBar();
			scrollBar.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			this.scrollBar.ScrollBarAutoSize = true;
			this.scrollBar.Visible = true;
			ContainerPanel.Controls.Add(scrollBar);
		}
		protected internal virtual void DestroyScrollBar() {
			this.scrollBar.Dispose();
			this.scrollBar = null;
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			RecalcLayout();
		}
		protected internal virtual void RecalcLayout() {
			if (!IsLayoutSuspended)
				RecalcLayoutCore();
		}
		protected internal virtual void RecalcLayoutCore() {
			SuspendLayout();
			try {
				SizeToScrollBar();
				if (Vertical)
					RecalcLayoutVertical();
				else
					RecalcLayoutHorizontal();
			}
			finally {
				ResumeLayout();
			}
		}
		protected internal virtual void SizeToScrollBar() {
			SuspendLayout();
			try {
				if (Vertical)
					this.Width = Math.Max(scrollBar.Width, navigator.Width);
				else
					this.Height = Math.Max(scrollBar.Height, navigator.Height);
			}
			finally {
				ResumeLayout();
			}
		}
		protected internal virtual Size CalcMinimalSize() {
			if (Vertical)
				return new Size(Math.Max(scrollBar.Width, navigator.Width), 0);
			else
				return new Size(0, Math.Max(scrollBar.Height, navigator.Height));
		}
		protected internal virtual void RecalcLayoutHorizontal() {
			Rectangle scrollBarBounds = new Rectangle(Point.Empty, new Size(Width, scrollBar.Height));
			scrollBarBounds.Width -= cornerSize;
			Rectangle navigatorBounds = new Rectangle(new Point(scrollBarBounds.Right - navigator.Width - 1, 0), 
				   new Size(navigator.MinSize.Width, scrollBarBounds.Height));
			scrollBarBounds.Width -= navigatorBounds.Width + 1;
			navigatorBounds = CenterVertically(navigatorBounds);
			scrollBarBounds = CenterVertically(scrollBarBounds);
			scrollBar.Bounds = scrollBarBounds;
			navigator.Bounds = navigatorBounds;
		}
		protected internal virtual void RecalcLayoutVertical() {
			Rectangle scrollBarBounds = new Rectangle(Point.Empty, new Size(scrollBar.Width, Height));
			scrollBarBounds.Height -= cornerSize;
			Rectangle navigatorBounds = new Rectangle(new Point(0, scrollBarBounds.Bottom - navigator.Height - 1),  
				new Size(scrollBarBounds.Width, navigator.MinSize.Height));
			scrollBarBounds.Height -= navigatorBounds.Height + 1;
			navigatorBounds = CenterHorizontally(navigatorBounds);
			scrollBarBounds = CenterHorizontally(scrollBarBounds);
			scrollBar.Bounds = scrollBarBounds;
			navigator.Bounds = navigatorBounds;
		}
		protected internal virtual Rectangle CenterVertically(Rectangle bounds) {
			int topPadding = (Height - bounds.Height) / 2;
			bounds.Y = topPadding;
			return bounds;
		}
		protected internal virtual Rectangle CenterHorizontally(Rectangle bounds) {
			int leftPadding = (Width - bounds.Width) / 2;
			bounds.X = leftPadding;
			return bounds;
		}
	}
	#endregion
	#region SchedulerResourceNavigator
	[DXToolboxItem(false)]
	public class SchedulerResourceNavigator : ControlNavigator, INavigatorOwner {
		Orientation orientation;
		public SchedulerResourceNavigator() {
			orientation = Orientation.Horizontal;
		}
		protected override NavigatorButtonsBase CreateButtons() {
			return new SchedulerResourceNavigatorButtons(this);
		}
		public Orientation Orientation { get { return orientation; } set { orientation = value; } } 
		Orientation INavigatorOwner.Orientation { get { return orientation; } }
	}
	#endregion
	#region SchedulerResourceNavigatorButtonCollection
	public class SchedulerResourceNavigatorButtonCollection : ControlNavigatorButtonCollection {
		public SchedulerResourceNavigatorButtonCollection(SchedulerResourceNavigatorButtons buttons)
			: base(buttons) {
			if (buttons == null)
				Exceptions.ThrowArgumentException("buttons", buttons);
		}
		protected override void CreateButtons(NavigatorButtonsBase buttons) {
			AddButton(new SchedulerResourceNavigatorFirstButtonHelper(buttons));
			AddButton(new SchedulerResourceNavigatorPrevPageButtonHelper(buttons));
			AddButton(new SchedulerResourceNavigatorPrevButtonHelper(buttons));
			AddButton(new SchedulerResourceNavigatorNextButtonHelper(buttons));
			AddButton(new SchedulerResourceNavigatorNextPageButtonHelper(buttons));
			AddButton(new SchedulerResourceNavigatorLastButtonHelper(buttons));
			AddButton(new SchedulerResourceNavigatorAppendButtonHelper(buttons));
			AddButton(new SchedulerResourceNavigatorRemoveButtonHelper(buttons));
		}
	}
	#endregion
	#region SchedulerResourceNavigatorSupportsVerticalHelper (abstract class)
	public abstract class SchedulerResourceNavigatorSupportsVerticalHelper : ControlNavigatorButtonHelperBase {
		#region Fields
		internal const int VerticalIndexOffset = 13;
		#endregion
		protected SchedulerResourceNavigatorSupportsVerticalHelper(NavigatorButtonsBase buttons)
			: base(buttons) {
			if (buttons == null)
				Exceptions.ThrowArgumentException("buttons", buttons);
		}
		#region Properties
		protected internal virtual bool Vertical {
			get {
				ResourceNavigator navigator = (ResourceNavigator)this.Control;
				return navigator.Vertical;
			}
		}
		public override int DefaultIndex {
			get {
				if (Vertical)
					return VerticalIndexOffset + base.DefaultIndex;
				else
					return base.DefaultIndex;
			}
		}
		#endregion
	}
	#endregion
	#region Button Helpers / Handlers
	#region SchedulerResourceNavigatorFirstButtonHelper
	public class SchedulerResourceNavigatorFirstButtonHelper : SchedulerResourceNavigatorSupportsVerticalHelper {
		public SchedulerResourceNavigatorFirstButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.First; } }
		public override string Hint { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_FirstVisibleResources); } }
	}
	#endregion
	#region SchedulerResourceNavigatorPrevPageButtonHelper
	public class SchedulerResourceNavigatorPrevPageButtonHelper : SchedulerResourceNavigatorSupportsVerticalHelper {
		public SchedulerResourceNavigatorPrevPageButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.PrevPage; } }
		public override string Hint { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_PrevVisibleResourcesPage); } }
	}
	#endregion
	#region SchedulerResourceNavigatorPrevButtonHelper
	public class SchedulerResourceNavigatorPrevButtonHelper : SchedulerResourceNavigatorSupportsVerticalHelper {
		public SchedulerResourceNavigatorPrevButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Prev; } }
		public override string Hint { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_PrevVisibleResources); } }
	}
	#endregion
	#region SchedulerResourceNavigatorNextButtonHelper
	public class SchedulerResourceNavigatorNextButtonHelper : SchedulerResourceNavigatorSupportsVerticalHelper {
		public SchedulerResourceNavigatorNextButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Next; } }
		public override string Hint { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_NextVisibleResources); } }
	}
	#endregion
	#region SchedulerResourceNavigatorNextPageButtonHelper
	public class SchedulerResourceNavigatorNextPageButtonHelper : SchedulerResourceNavigatorSupportsVerticalHelper {
		public SchedulerResourceNavigatorNextPageButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.NextPage; } }
		public override string Hint { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_NextVisibleResourcesPage); } }
	}
	#endregion
	#region SchedulerResourceNavigatorLastButtonHelper
	public class SchedulerResourceNavigatorLastButtonHelper : SchedulerResourceNavigatorSupportsVerticalHelper {
		public SchedulerResourceNavigatorLastButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Last; } }
		public override string Hint { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_LastVisibleResources); } }
	}
	#endregion
	#region SchedulerResourceNavigatorAppendButtonHelper
	public class SchedulerResourceNavigatorAppendButtonHelper : ControlNavigatorButtonHelperBase {
		public SchedulerResourceNavigatorAppendButtonHelper(NavigatorButtonsBase buttons)
			: base(buttons) {
			if (buttons == null)
				Exceptions.ThrowArgumentException("buttons", buttons);
		}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Append; } }
		public override string Hint { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_IncreaseVisibleResourcesCount); } }
	}
	#endregion
	#region SchedulerResourceNavigatorRemoveButtonHelper
	public class SchedulerResourceNavigatorRemoveButtonHelper : ControlNavigatorButtonHelperBase {
		public SchedulerResourceNavigatorRemoveButtonHelper(NavigatorButtonsBase buttons)
			: base(buttons) {
			if (buttons == null)
				Exceptions.ThrowArgumentException("buttons", buttons);
		}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Remove; } }
		public override string Hint { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_DecreaseVisibleResourcesCount); } }
	}
	#endregion
	#endregion
	public class SchedulerResourceNavigatorButtonsViewInfo : NavigatorButtonsViewInfo {
		public SchedulerResourceNavigatorButtonsViewInfo(SchedulerResourceNavigatorButtons buttons)
			: base(buttons) {
		}
		bool Vertical {
			get {
				SchedulerResourceNavigatorButtons buttons = (SchedulerResourceNavigatorButtons)this.Buttons;
				ResourceNavigator navigator = (ResourceNavigator)buttons.Control;
				return navigator.Vertical;
			}
		}
		public override void CheckSize(ref Size size) {
			if (Vertical)
				CheckSizeVertical(ref size);
			else
				base.CheckSize(ref size);
		}
		protected override void ApplyViewInfoMinSize(NavigatorObjectViewInfo viewInfo, ref int minWidth, ref int minHeight) {
			if (Vertical) {
				Size minSize = viewInfo.MinSize;
				minHeight += minSize.Height;
				if (minSize.Width > minWidth)
					minWidth = minSize.Width;
			}
			else
				base.ApplyViewInfoMinSize(viewInfo, ref minWidth, ref minHeight);
		}
		void CheckSizeVertical(ref Size size) {
			Clear();
			NavigatorObjectViewInfoCollection objectCollection = new NavigatorObjectViewInfoCollection();
			int minWidth = 0;
			int minHeight = 0;
			int difY = 0;
			Rectangle bounds = Bounds;
			ArrayList buttonList = CreateButtonList();
			for (int i = 0; i < buttonList.Count; i++)
				AddButtonViewInfo(buttonList[i] as NavigatorButtonBase, objectCollection, ref minWidth, ref minHeight);
			if (objectCollection.Count <= 0) return;
			int heightIncCount = 0;
			if (size.Height < minHeight)
				size.Height = minHeight;
			if (size.Width < minWidth)
				size.Width = minWidth;
			else {
				difY = (size.Height - minHeight) / objectCollection.Count;
				heightIncCount = size.Height - (minHeight + difY * objectCollection.Count);
			}
			int top = bounds.Left;
			for (int i = 0; i < objectCollection.Count; i++) {
				int y = top;
				Rectangle r = new Rectangle(bounds.X, y, size.Width, objectCollection[i].MinSize.Height + difY);
				if (heightIncCount-- > 0) {
					r.Height++;
				}
				Rectangle button = r;
				objectCollection[i].Bounds = button;
				top = r.Bottom;
			}
		}
	}
	[DXToolboxItem(false)]
	public class ContainerPanelControl : PanelControl {
		public ContainerPanelControl() {
		}
		protected override bool IsDrawNoBorder { get { return false; } }
	}
}
namespace DevExpress.XtraScheduler {
	#region ResourceNavigator
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ResourceNavigator : INavigatableControl {
		#region Fields
		bool isDisposed;
		ResourceNavigatorVisibility visibility = ResourceNavigatorVisibility.Auto;
		ResourceNavigatorControl navigatorControl;
		SchedulerControl schedulerControl;
		bool visible;
		bool vertical;
		#endregion
		public ResourceNavigator(SchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentException("control", control);
			this.schedulerControl = control;
			this.navigatorControl = CreateNavigatorControl();
			UpdatePaintStyle();
			this.navigatorControl.RecalcLayout();
			SubscribeNavigatorEvents();
		}
		#region Events
		#region ScrollValueChanged
		EventHandler onScrollValueChanged;
		protected internal event EventHandler ScrollValueChanged { add { onScrollValueChanged += value; } remove { onScrollValueChanged -= value; } }
		protected internal virtual void RaiseScrollValueChanged(object sender, EventArgs e) {
			if (onScrollValueChanged != null)
				onScrollValueChanged(sender, e);
		}
		#endregion
		#region VisibilityChanged
		EventHandler onVisibilityChanged;
		protected internal event EventHandler VisibilityChanged { add { onVisibilityChanged += value; } remove { onVisibilityChanged -= value; } }
		protected internal virtual void RaiseVisibilityChanged() {
			if (onVisibilityChanged != null)
				onVisibilityChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ScrollHappend
		ScrollEventHandler onScrollHappend;
		protected internal event ScrollEventHandler ScrollHappend { add { onScrollHappend += value; } remove { onScrollHappend -= value; } }
		protected internal virtual void RaiseScrollHappend(object sender, ScrollEventArgs e) {
			if (onScrollHappend != null)
				onScrollHappend(sender, e);
		}
		#endregion
		#endregion
		#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		internal ToolTipController ToolTipController { get { return NavigatorControl.ToolTipController; } set { NavigatorControl.ToolTipController = value; } }
		internal SchedulerControl SchedulerControl { get { return schedulerControl; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceNavigatorControl NavigatorControl { get { return navigatorControl; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool Visible {
			get { return visible; }
			set {
				if (visible == value)
					return;
				visible = value;
				navigatorControl.Visible = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool Vertical {
			get { return vertical; }
			set {
				if (vertical == value)
					return;
				vertical = value;
				this.navigatorControl.Vertical = value;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourceNavigatorVisibility"),
#endif
DefaultValue(ResourceNavigatorVisibility.Auto)]
		public ResourceNavigatorVisibility Visibility {
			get { return visibility; }
			set {
				if (visibility == value)
					return;
				visibility = value;
				RaiseVisibilityChanged();
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourceNavigatorButtons"),
#endif
Category(SRCategoryNames.Behavior), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ControlNavigatorButtons Buttons {
			get {
				if (navigatorControl == null)
					return null;
				SchedulerResourceNavigator nav = navigatorControl.Navigator;
				if (nav == null)
					return null;
				return nav.Buttons;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourceNavigatorShowToolTips"),
#endif
Category(SRCategoryNames.Behavior), DefaultValue(true)]
		public bool ShowToolTips { get { return NavigatorControl.ShowToolTips; } set { NavigatorControl.ShowToolTips = value; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (navigatorControl != null) {
					UnsubscribeNavigatorEvents();
					navigatorControl.Dispose();
					navigatorControl = null;
				}
				schedulerControl = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ResourceNavigator() {
			Dispose(false);
		}
		#endregion
		#region SubscribeNavigatorEvents
		protected internal virtual void SubscribeNavigatorEvents() {
			this.navigatorControl.ScrollValueChanged += new EventHandler(RaiseScrollValueChanged);
			this.navigatorControl.ScrollHappend += new ScrollEventHandler(RaiseScrollHappend);
		}
		#endregion
		#region UnsubscribeNavigatorEvents
		protected internal virtual void UnsubscribeNavigatorEvents() {
			this.navigatorControl.ScrollValueChanged -= new EventHandler(RaiseScrollValueChanged);
			this.navigatorControl.ScrollHappend -= new ScrollEventHandler(RaiseScrollHappend);
		}
		#endregion
		protected internal virtual ResourceNavigatorControl CreateNavigatorControl() {
			ResourceNavigatorControl control = new ResourceNavigatorControl();
			control.Navigator.NavigatableControl = this;
			return control;
		}		
		protected internal virtual void UpdatePaintStyle() {
			navigatorControl.LookAndFeel.ParentLookAndFeel = schedulerControl.PaintStyle.UserLookAndFeel;
			navigatorControl.RecalcLayout();
		}
		#region INavigatableControl implementation
		void INavigatableControl.AddNavigator(INavigatorOwner owner) {
		}
		void INavigatableControl.RemoveNavigator(INavigatorOwner owner) {
		}
		int INavigatableControl.RecordCount { get { return 0; } }
		int INavigatableControl.Position { get { return 0; } }
		bool INavigatableControl.IsActionEnabled(NavigatorButtonType type) {
			return schedulerControl.IsResourceNavigatorActionEnabled(type);
		}
		void INavigatableControl.DoAction(NavigatorButtonType type) {
			schedulerControl.ExecuteResourceNavigatorAction(type);
		}
		#endregion
	}
	#endregion
	#region SchedulerResourceNavigatorButtons
	[TypeConverter("System.ComponentModel.ExpandableObjectConverter, System")]
	public class SchedulerResourceNavigatorButtons : ControlNavigatorButtons {
		public SchedulerResourceNavigatorButtons(SchedulerResourceNavigator navigator)
			: base(navigator) {
			if (navigator == null)
				Exceptions.ThrowArgumentException("navigator", navigator);
		}
		protected override NavigatorButtonCollectionBase CreateNavigatorButtonCollection() {
			return new SchedulerResourceNavigatorButtonCollection(this);
		}
		protected override NavigatorButtonsViewInfo CreateViewInfo() {
			return new SchedulerResourceNavigatorButtonsViewInfo(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override NavigatorButton Edit { get { return base.Edit; } }
		protected override bool ShouldSerializeEdit() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override NavigatorButton EndEdit { get { return base.EndEdit; } }
		protected override bool ShouldSerializeEndEdit() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override NavigatorButton CancelEdit { get { return base.CancelEdit; } }
		protected override bool ShouldSerializeCancelEdit() { return false; }
	}
	#endregion
}
