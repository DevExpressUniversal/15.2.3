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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Text;
using DevExpress.Utils.Text.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils.Controls;
using System.Drawing.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using System.Reflection;
namespace DevExpress.Utils {
	[TypeConverter(typeof(ContextAlignmentEnumConverter))]
	public enum ContextItemAlignment {
		TopNear = 1,
		TopFar = 2,
		BottomNear = 4,
		BottomFar = 5,
		MiddleTop = 3, 
		MiddleBottom = 6,
		NearBottom = 7, 
		NearTop = 8,
		NearCenter = 9,
		FarBottom = 10,
		FarTop = 11,
		FarCenter = 12,
		CenterNear = 13, 
		CenterFar = 14, 
		Center = 15,
		[Obsolete("Use TopNear instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		UpperLeft = 1,
		[Obsolete("Use BottomNear instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		LowerLeft = 4,
		[Obsolete("Use TopFar instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		UpperRight = 2,
		[Obsolete("Use BottomFar instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		LowerRight = 5,
	};
	public enum ContextItemState { Normal, Hot };
	public enum ContextItemVisibility { Auto, Hidden, Visible };
	public enum ContextButtonMode { Glyph, Caption, CaptionGlyph };
	public enum ContextAnimationType { Default, None, OpacityAnimation, OutAnimation, SequenceAnimation };
	public enum ContextAnchorStyle { None, Left, Top, Right, Bottom };
	public enum ContextItemHitTest { None, Glyph, Rating, Caption, TopPanel, BottomPanel, CenterPanel, TrackLine, Thumb, ZoomInButton, ZoomOutButton };
	enum ContextButtonsPanel { Top, Bottom, Near, Far, Center};
	#region IAnchored interface
	public enum AnchorAlignment { Default, Top, Left, Right, Bottom }
	public interface IAnchored {
		Rectangle AnchorBounds { get; }
		IAnchored AnchorElement { get; }
		AnchorAlignment AnchorAlignment { get; }
		int AnchorIndent { get; }
		Point AnchorOffset { get; }
		Rectangle Bounds { get; }
		bool IsRightToLeft { get; }
	}
	public static class AnchorLayoutHelper {
		public static List<IAnchored> CalcOrder(List<IAnchored> elements) {
			int count = elements.Count;
			List<IAnchored> result = new List<IAnchored>();
			IAnchored[,] array = new IAnchored[count, count];
			for(int i = 0; i < count; i++) {
				var elem = elements[i];
				int deep = 0;
				array[deep, i] = elem;
				var current = elem;
				while(current.AnchorElement != null) {
					deep++;
					array[deep, i] = current.AnchorElement;
					current = current.AnchorElement;
				}
			}
			for(int deep = count - 1; deep > -1; deep--) {
				for(int i = 0; i < count; i++) {
					var el = array[deep, i];
					if(el != null) result.Add(el);
				}
			}
			result = result.Distinct().ToList();
			return result;
		}
		public static Rectangle LayoutToAnchor(IAnchored element) {
			Rectangle result = element.Bounds;
			if(element.AnchorElement == null) return result;
			int deltaX, deltaY;
			var alignment = element.AnchorAlignment;
			if(element.IsRightToLeft && (alignment == AnchorAlignment.Right || alignment == AnchorAlignment.Left))
				alignment = alignment == AnchorAlignment.Left ? AnchorAlignment.Right : AnchorAlignment.Left;
			switch(alignment) {
				case AnchorAlignment.Left:
					deltaX = element.AnchorBounds.Left - element.Bounds.Right;
					result.X += deltaX - element.AnchorIndent + element.AnchorOffset.X;
					deltaY = element.AnchorBounds.Top - element.Bounds.Top;
					result.Y += deltaY + element.AnchorOffset.Y;
					break;
				case AnchorAlignment.Right:
					deltaX = element.AnchorBounds.Right - element.Bounds.Left;
					result.X += deltaX + element.AnchorIndent + element.AnchorOffset.X;
					deltaY = element.AnchorBounds.Top - element.Bounds.Top;
					result.Y += deltaY + element.AnchorOffset.Y;
					break;
				case AnchorAlignment.Top:
					if(element.IsRightToLeft)
						deltaX = element.AnchorBounds.Right - element.Bounds.Right;
					else
						deltaX = element.AnchorBounds.Left - element.Bounds.Left;
					result.X += deltaX + element.AnchorOffset.X;
					deltaY = element.AnchorBounds.Top - element.Bounds.Bottom;
					result.Y += deltaY - element.AnchorIndent + element.AnchorOffset.Y;
					break;
				case AnchorAlignment.Bottom:
				case AnchorAlignment.Default:
					if(element.IsRightToLeft) 
						deltaX = element.AnchorBounds.Right - element.Bounds.Right;
					else 
						deltaX = element.AnchorBounds.Left - element.Bounds.Left;
					result.X += deltaX + element.AnchorOffset.X;
					deltaY = element.AnchorBounds.Bottom - element.Bounds.Top;
					result.Y += deltaY + element.AnchorIndent + element.AnchorOffset.Y;
					break;
			}
			return result;
		}
	}
	#endregion
	internal static class ContextButtonConstants {
		public static int MaxRating = 5;	 
	}
	#region ContextItemHitInfo
	public class ContextItemHitInfo {
		protected Point fhitPoint;
		protected ContextItemHitTest fhitTest;
		protected ContextItemViewInfo fhitViewInfo;
		public ContextItemHitInfo(Point hitPoint) {
			this.fhitPoint = hitPoint;
		}
		public ContextItemHitInfo(Point hitPoint, ContextItemViewInfo hitViewInfo) {
			this.fhitPoint = hitPoint;
			this.fhitViewInfo = hitViewInfo;
		}
		public Point HitPoint { get { return fhitPoint; } }
		public ContextItemHitTest HitTest { get { return fhitTest; } }
		protected internal virtual ContextItemViewInfo HitViewInfo {
			get { return fhitViewInfo; }
			set { fhitViewInfo = value; }
		}
		protected internal virtual void SetHitPoint(Point newValue) { this.fhitPoint = newValue; }
		protected internal virtual void SetHitTest(ContextItemHitTest newValue) { this.fhitTest = newValue; }
		protected internal virtual void Clear() {
			this.fhitTest = ContextItemHitTest.None;
			this.fhitPoint = new Point(-10000, -10000);
		}
	}
	#endregion
	#region ContextItem
	public delegate void ContextItemClickEventHandler(object sender, ContextItemClickEventArgs e);
	public delegate void ContextButtonValueChangedEventHandler(object sender, ContextButtonValueEventArgs e);
	public class ContextItemClickEventArgs : EventArgs {
		public ContextItemClickEventArgs(ContextItem item, ContextItemViewInfo itemInfo) {
			Item = item;
			ItemInfo = itemInfo;
		}
		public ContextItemClickEventArgs(ContextItem item, ContextItemViewInfo itemInfo, object dataItem) : this(item, itemInfo) {
			DataItem = dataItem;
		}
		public ContextItem Item { get; private set; }
		public object DataItem { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ContextItemViewInfo ItemInfo { get; set; }
		public Rectangle ScreenBounds { get { return ItemInfo == null? Rectangle.Empty: ItemInfo.ViewInfo.OwnerControl.RectangleToScreen(ItemInfo.Bounds); } }
	}
	public class ContextButtonValueEventArgs : EventArgs {
		public ContextButtonValueEventArgs(ContextItem item, object newValue) {
			Item = item;
			Value = newValue;
		}
		public ContextItem Item { get; private set; }
		public object Value { get; private set; }
	}
	public delegate void ContextButtonToolTipEventHandler(object sender, ContextButtonToolTipEventArgs e);
	public class ContextButtonToolTipEventArgs : EventArgs {
		ContextItem item;
		object value;
		string text;
		public ContextButtonToolTipEventArgs(ContextItem item) {
			this.item = item;
			this.value = -1;
			this.text = String.Empty;
		}
		public ContextButtonToolTipEventArgs(ContextItem item, object value) {
			this.item = item;
			this.value = value;
			this.text = String.Empty;
		}
		public ContextItem Item { get { return item; } }
		public object Value { get { return value; } }
		public string Text {
			get { return text; }
			set {
				if(text == value) return;
				text = value;
			}
		}
	}
	public class ContextItem : ICloneable {
		AppearanceObject appearanceNormal;
		AppearanceObject appearanceHover;
		string toolTip = String.Empty;
		string toolTipTitle = String.Empty;
		ToolTipIconType toolTipIconType = ToolTipIconType.None;
		bool showToolTips = true;
		public ContextItem() { }
		DefaultBoolean allowGlyphSkinning = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				DefaultBoolean prev = AllowGlyphSkinning;
				allowGlyphSkinning = value;
				OnItemChanged("AllowGlyphSkinning", prev, AllowGlyphSkinning);
			}
		}
		void ResetAppearanceNormal() { AppearanceNormal.Reset(); }
		bool ShouldSerializeAppearanceNormal() { return AppearanceNormal.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public AppearanceObject AppearanceNormal {
			get {
				if(appearanceNormal == null) {
					appearanceNormal = new AppearanceObject();
					appearanceNormal.SizeChanged += OnAppearanceNormalChanged;
				}
				return appearanceNormal;
			}
		}
		void OnAppearanceNormalChanged(object sender, EventArgs e) {
			OnItemChanged("AppearanceNormal", null, AppearanceNormal);
		}
		void ResetAppearanceHover() { AppearanceHover.Reset(); }
		bool ShouldSerializeAppearanceHover() { return AppearanceHover.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public AppearanceObject AppearanceHover {
			get {
				if(appearanceHover == null) {
					appearanceHover = new AppearanceObject();
					appearanceHover.SizeChanged += OnAppearanceHoverChanged;
				}
				return appearanceHover;
			}
		}
		void OnAppearanceHoverChanged(object sender, EventArgs e) {
			OnItemChanged("AppearanceHover", null, AppearanceHover);
		}
		protected virtual void OnItemChanged(string propertyName, object oldValue, object newValue) {
			if(IsLockUpdate)
				return;
			if(CollectionOwner != null)
				CollectionOwner.OnItemChanged(this, propertyName, oldValue, newValue);
		}
		protected int LockUpdateCount { get; set; }
		protected internal bool IsLockUpdate { get { return LockUpdateCount > 0; } }
		public void BeginUpdate() {
			LockUpdateCount++;
		}
		public void EndUpdate() {
			if(LockUpdateCount == 0)
				return;
			LockUpdateCount--;
			if(LockUpdateCount == 0)
				OnItemChanged("", null, null);
		}
		public void CancelUpdate() {
			if(LockUpdateCount == 0)
				return;
			LockUpdateCount--;
		}
		Guid guid = Guid.NewGuid();
		bool ShouldSerializeId() { return Id != Guid.Empty; }
		[Browsable(false)]
		public Guid Id {
			get { return guid; }
			set {
				if(Id == value)
					return;
				Guid prev = Id;
				guid = value;
				OnItemChanged("Id", prev, Id);
			}
		}
		ContextAnimationType animationType = ContextAnimationType.Default;
		[DefaultValue(ContextAnimationType.Default)]
		public ContextAnimationType AnimationType {
			get { return animationType; }
			set {
				if(AnimationType == value)
					return;
				ContextAnimationType prev = AnimationType;
				animationType = value;
				OnItemChanged("AnimationType", prev, AnimationType);
			}
		}
		Image glyph;
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Glyph {
			get { return glyph; }
			set {
				if(Glyph == value)
					return;
				Image prev = Glyph;
				glyph = value;
				OnItemChanged("Glyph", prev, Glyph);
			}
		}
		Image hoverGlyph;
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image HoverGlyph {
			get { return hoverGlyph; }
			set {
				if(HoverGlyph == value)
					return;
				Image prev = HoverGlyph;
				hoverGlyph = value;
				OnItemChanged("HoverGlyph", prev, HoverGlyph);
			}
		}
		ContextItemAlignment alignment = ContextItemAlignment.TopNear;
		[DefaultValue(ContextItemAlignment.TopNear)]
		public virtual ContextItemAlignment Alignment {
			get { return alignment; }
			set {
				if(Alignment == value)
					return;
				ContextItemAlignment prev = Alignment;
				alignment = value;
				OnItemChanged("Alignment", prev, Alignment);
			}
		}
		protected virtual internal Orientation Orientation {
			get {
				if(AnchorElement != null) return AnchorElement.Orientation;
				if(Alignment == ContextItemAlignment.NearTop || Alignment == ContextItemAlignment.NearCenter || Alignment == ContextItemAlignment.NearBottom) return System.Windows.Forms.Orientation.Vertical;
				if(Alignment == ContextItemAlignment.FarTop || Alignment == ContextItemAlignment.FarCenter || Alignment == ContextItemAlignment.FarBottom) return System.Windows.Forms.Orientation.Vertical;
				return System.Windows.Forms.Orientation.Horizontal;
			}
		}
		Size size = Size.Empty;
		void ResetSize() { Size = Size.Empty; }
		bool ShouldSerializeSize() { return !Size.IsEmpty; }
		[Browsable(false)]
		public Size Size {
			get { return size; }
			set {
				value.Width = Math.Max(0, value.Width);
				value.Height = Math.Max(0, value.Height);
				if(Size == value)
					return;
				Size prev = Size;
				size = value;
				OnItemChanged("Size", prev, Size);
			}
		}
		string name = string.Empty;
		[DefaultValue("")]
		public string Name {
			get { return name; }
			set {
				if(Name == value)
					return;
				string prev = Name;
				name = value;
				OnItemChanged("Name", prev, Name);
			}
		}
		object tag;
		[DefaultValue(null)]
		public object Tag {
			get { return tag; }
			set { 
				if(Tag == value)
					return;
				object prev = Tag;
				tag = value;
				OnItemChanged("Tag", prev, Tag);
			}
		}
		bool enabled = true;
		[DefaultValue(true)]
		public bool Enabled {
			get { return enabled; }
			set { 
				if(Enabled == value) return;
				bool prev = enabled;
				enabled = value;
				OnItemChanged("Enabled", prev, Enabled);
			}
		}
		ContextItemVisibility? forcedVisibility = null;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetForcedVisibility(ContextItemVisibility? forcedVisibility) {
			this.forcedVisibility = forcedVisibility;
		}
		internal bool IsForcedVisibilitySet { get { return forcedVisibility.HasValue; } }
		ContextItemVisibility visibility = ContextItemVisibility.Auto;
		[DefaultValue(ContextItemVisibility.Auto)]
		public ContextItemVisibility Visibility {
			get {
				if(forcedVisibility.HasValue)
					return forcedVisibility.Value;
				return visibility; 
			}
			set {
				if(Visibility == value)
					return;
				ContextItemVisibility prev = Visibility;
				visibility = value;
				OnItemChanged("Visibility", prev, Visibility);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ContextItemCollection Collection {
			get;
			internal set;
		}
		protected IContextItemCollectionOwner CollectionOwner { get { return Collection == null ? null : Collection.Owner; } }
		int anchorIndent = 0;
		[DefaultValue(0)]
		public int AnchorIndent {
			get { return anchorIndent; }
			set {
				if(AnchorIndent == value)
					return;
				int prev = AnchorIndent;
				anchorIndent = value;
				OnItemChanged("AnchorIndent", prev, anchorIndent);
			}
		}
		Point anchorOffset = Point.Empty;
		void ResetAnchorOffset() { AnchorOffset = Point.Empty; }
		bool ShouldSerializeAnchorOffset() { return !AnchorOffset.IsEmpty; }
		public Point AnchorOffset {
			get { return anchorOffset; }
			set {
				if(AnchorOffset == value)
					return;
				Point prev = AnchorOffset;
				anchorOffset = value;
				OnItemChanged("AnchorOffset", prev, AnchorOffset);
			}
		}
		ContextItem anchorElement;
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		Editor("DevExpress.XtraEditors.Design.ContextItemAnchorElementEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor))]
		public ContextItem AnchorElement {
			get { return anchorElement; }
			set {
				if(AnchorElement == value)
					return;
				ContextItem prev = AnchorElement;
				anchorElement = value;
				OnAnchorElementChanged(prev, AnchorElement);
			}
		}
		[Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor))]
		public virtual SuperToolTip SuperTip {
			get;
			set;
		}
		protected virtual bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		public virtual void ResetSuperTip() { SuperTip = null; }
		[DefaultValue(""), Localizable(true)]
		public virtual string ToolTip {
			get { return toolTip; }
			set { 
				if(toolTip == value) return;
				toolTip = value;
			}
		}
		[DefaultValue(""), Localizable(true)]
		public virtual string ToolTipTitle {
			get { return toolTipTitle; }
			set { 
				if(toolTipTitle == value) return;
				toolTipTitle = value;
			}
		}
		[DefaultValue(ToolTipIconType.None), Localizable(true)]
		public virtual ToolTipIconType ToolTipIconType {
			get { return toolTipIconType; }
			set {
				if(toolTipIconType == value) return;
				toolTipIconType = value;
			}
		}
		[DefaultValue(true)]
		public virtual bool ShowToolTips {
			get { return showToolTips; }
			set {
				if(showToolTips == value) return;
				showToolTips = value;
			}
		}	   
		protected virtual void OnAnchorElementChanged(ContextItem prev, ContextItem current) {
			AnchorElementId = current == null? Guid.Empty: current.Id;
			OnItemChanged("AnchorElement", prev, current);
		}
		Guid anchorElementId = Guid.Empty;
		bool ShouldSerializeAnchorElementId() { return AnchorElementId != Guid.Empty; }
		[Browsable(false)]
		public Guid AnchorElementId {
			get { return anchorElementId; }
			set {
				if(AnchorElementId == value)
					return;
				Guid prev = AnchorElementId;
				anchorElementId = value;
				OnAnchorElementIdChanged(prev, AnchorElementId);
			}
		}
		private void OnAnchorElementIdChanged(Guid prev, Guid current) {
			if(current == Guid.Empty) {
				AnchorElement = null;
				return;
			}
			ContextItem item = GetAnchorElementById(current);
			if(item != null)
				AnchorElement = item;
			OnItemChanged("AnchorElementId", prev, current);
		}
		protected internal virtual ContextItem GetAnchorElementById(Guid anchorId) {
			if(Collection == null)
				return null;
			foreach(ContextItem item in Collection) {
				if(item.Id == anchorId) return item;
			}
			return null;
		}
		AnchorAlignment anchorAlignment = AnchorAlignment.Default;
		[DefaultValue(AnchorAlignment.Default)]
		public AnchorAlignment AnchorAlignment {
			get { return anchorAlignment; }
			set {
				if(AnchorAlignment == value)
					return;
				AnchorAlignment prev = AnchorAlignment;
				anchorAlignment = value;
				OnItemChanged("AnchorAlignment", prev, AnchorAlignment);
			}
		}
		protected internal virtual ContextItemViewInfo CreateViewInfo(ContextItemCollectionViewInfo collectionViewInfo) {
			return new ContextItemViewInfo(this, collectionViewInfo);
		}
		public event ContextItemClickEventHandler Click;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseContextItemClick(MouseEventArgs e, ContextItemViewInfo itemInfo) {
			if(Click == null) return;
			ContextItemClickEventArgs args = new ContextItemClickEventArgs(this, itemInfo);
			Click(this, args);
		}
		public event ContextButtonToolTipEventHandler CustomToolTip;
		protected internal virtual void RaiseCustomToolTip(ContextButtonToolTipEventArgs e) {
			if(CustomToolTip != null)
				CustomToolTip(this, e);
		}		 
		public object Clone() {
			ContextItem res = Create();
			res.Assign(this);
			return res;
		}
		protected virtual void Assign(ContextItem item) {
			BeginUpdate();
			try {
				Glyph = item.Glyph;
				HoverGlyph = item.HoverGlyph;
				Alignment = item.Alignment;
				Name = item.Name;
				Tag = item.Tag;
				Visibility = item.Visibility;
				AnchorIndent = item.AnchorIndent;
				AnchorOffset = item.AnchorOffset;
				AnchorElement = item.AnchorElement;
				AnchorElementId = item.AnchorElementId;
				AnchorAlignment = item.AnchorAlignment;
				AnimationType = item.AnimationType;
				Click += item.Click;
				Collection = item.Collection;
				CustomToolTip += item.CustomToolTip;
				Id = item.Id;
				AppearanceNormal.Assign(item.AppearanceNormal);
				AppearanceHover.Assign(item.AppearanceHover);
				AllowGlyphSkinning = item.AllowGlyphSkinning;
				ToolTip = item.ToolTip;
				SuperTip = item.SuperTip;
				ToolTipTitle = item.ToolTipTitle;
				ToolTipIconType = item.ToolTipIconType;
				ShowToolTips = item.ShowToolTips;
				Enabled = item.Enabled;
			}
			finally {
				CancelUpdate();
			}
		}
		protected virtual ContextItem Create() {
			System.Reflection.ConstructorInfo ci = GetType().GetConstructor(new Type[] { });
			return (ContextItem)ci.Invoke(new object[] { });
		}
		protected internal ContextItem OriginItem { get; set; }
	}
	public class ContextItemViewInfo : IAnchored {
		ContextItemPainter painter;
		ContextItemCollectionViewInfo collectionViewInfo;
		ContextItemHitInfo hitInfo, pressedInfo;
		public ContextItemViewInfo(ContextItem item, ContextItemCollectionViewInfo collectionViewInfo) {
			Item = item;
			this.collectionViewInfo = collectionViewInfo;
			this.hitInfo = this.pressedInfo = null;
			Opacity = ViewInfo.NormalItemOpacity;
		}
		protected internal virtual bool IsHovered {
			get { return ViewInfo.HoverItem == this; }
		}
		protected internal bool IsHorizontal { get { return Item.Orientation == Orientation.Horizontal; } }
		protected internal virtual bool IsRightToLeft { get { return ViewInfo.IsRightToLeft; } }
		public bool AllowGlyphColorize {
			get { 
				return Item.AllowGlyphSkinning != DefaultBoolean.Default? Item.AllowGlyphSkinning == DefaultBoolean.True: Options.AllowGlyphSkinning;
			}
		}
		public virtual ContextAnimationType GetAnimationType() {
			ContextAnimationType res = Item.AnimationType;
			if(res == ContextAnimationType.Default)
				res = Options.GetAnimationType();
			if(res == ContextAnimationType.SequenceAnimation)
				return ContextAnimationType.OpacityAnimation;
			return res;
		}
		List<ContextItemViewInfo> anchoredItems;
		protected internal List<ContextItemViewInfo> AnchoredItems { 
			get {
				if(anchoredItems == null)
					anchoredItems = new List<ContextItemViewInfo>();
				return anchoredItems;
			} 
		}
		protected virtual Image DefaultGlyph { get { return null; } }
		protected virtual Image DefaultHoverGlyph { get { return null; } }
		protected internal virtual Image Glyph {
			get { 
				if(Item.Glyph != null)
					return Item.Glyph;
				return DefaultGlyph;
			}
		}		
		public ContextItemHitInfo HitInfo {
			get { return hitInfo; }
			protected internal set { hitInfo = value; }
		}
		public ContextItemHitInfo PressedInfo {
			get { return pressedInfo; }
			protected internal set { pressedInfo = value; }
		}
		protected internal virtual ContextItemHitInfo CalcHitInfo(Point hitPoint) {
			ContextItemHitInfo hitInfo;
			if(!Bounds.Contains(hitPoint))
				hitInfo = null;
			else {
				hitInfo = new ContextItemHitInfo(hitPoint, this);
			}
			return hitInfo;
		}
		protected internal virtual Image HoverGlyph {
			get { 
				if(Item.HoverGlyph != null)
					return Item.HoverGlyph;
				return DefaultHoverGlyph;
			}
		}
		protected internal virtual Rectangle GlyphBounds {
			get { return Rectangle.Empty; }
		}
		ImageAttributes imageAttributes;
		protected ImageAttributes ImageAttributes {
			get {
				if(imageAttributes == null)
					imageAttributes = new ImageAttributes();
				return imageAttributes;
			}
		}
		protected internal ImageAttributes GetImageAttributes() {
			UpdateImageAttributesByOpacity();
			if(!AllowGlyphColorize)
				return ImageAttributes;
			Color foreColor = IsHovered? Item.AppearanceHover.ForeColor: Item.AppearanceNormal.ForeColor;
			return ImageColorizer.GetColoredAttributes(foreColor, (int)(255 * Opacity));
		}
		protected internal virtual float Opacity { get; set; }
		protected virtual void SetOpacityByVisibility(float currentOpacity) {
			switch(Item.Visibility) {
				case ContextItemVisibility.Auto:
					Opacity = currentOpacity;
					break;
				case ContextItemVisibility.Visible:
					if(!Item.Enabled) Opacity = ViewInfo.DisabledItemOpacity;
					else 
						Opacity = IsHovered ? ViewInfo.HoverItemOpacity : ViewInfo.NormalItemOpacity;
					break;
				case ContextItemVisibility.Hidden:
					Opacity = 0.0f;
					break;
			}
		}	   
		protected internal virtual void UpdateImageAttributesByOpacity() {
			ImageAttributes.SetColorMatrix(new ColorMatrix() { Matrix33 = Opacity });
		}
		protected internal virtual void UpdateOpacity() {
			float currentOpacity = ViewInfo.NormalItemOpacity;
			if(ViewInfo.StopAnimation)
				currentOpacity = ViewInfo.NormalItemOpacity;
			else {
				currentOpacity = ViewInfo.GetCurrentAnimationCoeff();
				if(!Item.Enabled)
					currentOpacity *= ViewInfo.DisabledItemOpacity;
				else 
					currentOpacity *= IsHovered ? ViewInfo.HoverItemOpacity : ViewInfo.NormalItemOpacity;			
			}
			if(Opacity == currentOpacity)
				return;
			SetOpacityByVisibility(currentOpacity);		  
		}
		public RectangleF GetAnimatedGlyphRect() {
			RectangleF start = new RectangleF(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2, 0, 0);
			RectangleF end = GlyphBounds;
			RectangleF rect = CalcOutAnimationRect(start, end, ViewInfo.GetCurrentAnimationCoeff());
			return rect;
		}
		public ContextItem Item { get; private set; }
		public Size Size { get; protected set; }
		public Rectangle Bounds { get; internal set; }
		public virtual void CalcViewInfo(Rectangle bounds) {
			Bounds = bounds;
		}
		protected internal virtual ContextItemPainter Painter {
			get {
				if(painter == null)
					painter = new ContextItemPainter(this);
				return painter;
			}
		}
		public ContextItemCollectionViewInfo ViewInfo { get { return collectionViewInfo; } }
		public ContextItemCollectionOptions Options { get { return ViewInfo.Options; } }
		public ContextItemCollection Collection { get { return ViewInfo.Collection; } }
		public virtual Rectangle RedrawBounds { get; internal set; }
		public bool IsNotFittedWidth { get; set; }
		public bool IsNotFittedHeight { get; set; }
		public bool IsAnimated {
			get {
				if(Item.Visibility == ContextItemVisibility.Visible)
					return false;
				return Item.Visibility == ContextItemVisibility.Auto && GetAnimationType() != ContextAnimationType.None;
			}
		}
		public virtual bool ShouldRedrawItem() { return false; }
		public virtual void OnItemClick(MouseEventArgs e) {
			Item.RaiseContextItemClick(e, this);
			ViewInfo.Owner.RaiseContextItemClick(new ContextItemClickEventArgs(Item, this));
		}
		public virtual void OnMouseUp(MouseEventArgs e) { }
		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual ContextButtonToolTipEventArgs GetCustomToolTipArgs(Point point) {
			return new ContextButtonToolTipEventArgs(Item);
		}
		public virtual ContextButtonToolTipEventArgs OnItemToolTip(Point point) {
			ContextButtonToolTipEventArgs e = GetCustomToolTipArgs(point);
			Item.RaiseCustomToolTip(e);
			ViewInfo.Owner.RaiseCustomContextButtonToolTip(e);
			return e;
		}
		protected virtual RectangleF CalcOutAnimationRect(RectangleF start, RectangleF end, float value) {
			return new RectangleF(
					start.X + value * (end.X - start.X),
					start.Y + value * (end.Y - start.Y),
					start.Width + value * (end.Width - start.Width),
					start.Height + value * (end.Height - start.Height));
		}
		protected IAnchored GetAnchorElement() {
			foreach(ContextItemViewInfo viewInfo in ViewInfo.Items) {
				if(viewInfo.Item == Item.AnchorElement)
					return viewInfo as IAnchored;
			}
			return null;
		}
		#region IAnchored
		Rectangle IAnchored.AnchorBounds {
			get {
				if(GetAnchorElement() != null)
					return GetAnchorElement().Bounds;
				else
					return Rectangle.Empty;
			}
		}
		IAnchored IAnchored.AnchorElement {
			get { return GetAnchorElement(); }
		}
		AnchorAlignment IAnchored.AnchorAlignment {
			get { return Item.AnchorAlignment; }
		}
		int IAnchored.AnchorIndent {
			get { return Item.AnchorIndent; }
		}
		Point IAnchored.AnchorOffset {
			get { return Item.AnchorOffset; }
		}
		Rectangle IAnchored.Bounds {
			get { return Bounds; }
		}
		bool IAnchored.IsRightToLeft {
			get { return IsRightToLeft; }
		}
		#endregion
		protected internal virtual void OnHoverOut() {
			ViewInfo.Owner.Redraw(RedrawBounds);
		}
		protected internal virtual void CalcBestSize() {
			Size = Glyph != null ? Glyph.Size : Size.Empty;
		}
		protected Rectangle OffsetRect(Rectangle rect, int deltaX, int deltaY) {
			rect.X += deltaX; rect.Y += deltaY;
			return rect;
		}
		protected internal virtual void MakeOffset(int deltaX, int deltaY) {
			Bounds = OffsetRect(Bounds, deltaX, deltaY);
			RedrawBounds = OffsetRect(RedrawBounds, deltaX, deltaY);
		}
	}
	public class ContextItemPainter {
		public ContextItemPainter(ContextItemViewInfo viewInfo) {
			ItemInfo = viewInfo;
		}
		public ContextItemViewInfo ItemInfo { get; private set; }
		public ContextItemCollection Collection { get { return ItemInfo.ViewInfo.Collection; } }
		public ContextItemCollectionOptions Options { get { return ItemInfo.ViewInfo.Options; } } 
		protected int GetItemOpacity() {
			float coeff = ItemInfo.ViewInfo.GetCurrentAnimationCoeff();
			coeff *= ItemInfo.IsHovered ? ItemInfo.ViewInfo.HoverItemOpacity : ItemInfo.ViewInfo.NormalItemOpacity;
			return (int)(coeff * 255);
		}
		protected virtual void DrawGlyphCore(ContextItemCollectionInfoArgs info, Image glyph, Rectangle rect) {
			if(!ItemInfo.IsAnimated) {
				if(ItemInfo.AllowGlyphColorize)
					info.Graphics.DrawImage(glyph, rect, 0, 0, glyph.Width, glyph.Height, GraphicsUnit.Pixel, ItemInfo.GetImageAttributes());
				else if(ItemInfo.Opacity == 1.0f)
					info.Graphics.DrawImage(glyph, rect);
				else
					info.Graphics.DrawImage(glyph, rect, 0, 0, glyph.Width, glyph.Height, GraphicsUnit.Pixel, ItemInfo.GetImageAttributes());		
			}
			else
				DrawAnimatedGlyph(info, glyph, rect);
		}
		protected virtual void DrawGlyphCore(ContextItemCollectionInfoArgs info, Image glyph, RectangleF rect) {
			if(ItemInfo.Opacity == 1.0f)
				info.Graphics.DrawImage(glyph, rect);
			else {
				PointF[] pt = new PointF[] { 
						new PointF(rect.X, rect.Y),
						new PointF(rect.Right, rect.Y),
						new PointF(rect.X, rect.Bottom)
					};
				info.Graphics.DrawImage(glyph, pt, new Rectangle(Point.Empty, glyph.Size), GraphicsUnit.Pixel, ItemInfo.GetImageAttributes());
			}
		}
		protected virtual void DrawGlyphCore(ContextItemCollectionInfoArgs info, Image glyph) {
			DrawGlyphCore(info, glyph, ItemInfo.GlyphBounds);
		}
		protected virtual void DrawGlyphCore(ContextItemCollectionInfoArgs info, SkinElementInfo skinInfo, Rectangle bounds) {
			if(skinInfo != null) skinInfo.Bounds = bounds;
			if(!ItemInfo.IsAnimated)
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, skinInfo);
			else
				DrawAnimatedGlyph(info, null, skinInfo.Bounds, skinInfo);
		}
		protected virtual void DrawGlyphCore(ContextItemCollectionInfoArgs info, SkinElementInfo skinInfo, RectangleF bounds) {
			DrawGlyphCore(info, skinInfo, Rectangle.Round(bounds));
		}
		protected virtual void DrawAnimatedGlyph(ContextItemCollectionInfoArgs info, Image glyph, Rectangle rect) {
			DrawAnimatedGlyph(info, glyph, rect, null);
		}
		protected virtual void DrawAnimatedGlyph(ContextItemCollectionInfoArgs info, Image glyph) {
			DrawAnimatedGlyph(info, glyph, ItemInfo.GlyphBounds);
		}
		protected virtual void DrawAnimatedGlyph(ContextItemCollectionInfoArgs info, Image glyph, Rectangle rect, SkinElementInfo skinInfo) {
			if(ItemInfo.Opacity == 0.0f)
				return;
			if(skinInfo != null) {
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, skinInfo);
				return;
			}
			if(ItemInfo.GetAnimationType() == ContextAnimationType.OpacityAnimation)
			   info.Graphics.DrawImage(glyph, rect, 0, 0, glyph.Width, glyph.Height, GraphicsUnit.Pixel, ItemInfo.GetImageAttributes());
			if(ItemInfo.GetAnimationType() == ContextAnimationType.OutAnimation) {
				RectangleF bounds = ItemInfo.GetAnimatedGlyphRect();
				PointF[] pt = new PointF[] { 
					new PointF(bounds.X, bounds.Y),
					new PointF(bounds.Right, bounds.Y),
					new PointF(bounds.X, bounds.Bottom)
				};
				if(skinInfo != null) {
					 ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, skinInfo);
				}
				info.Graphics.DrawImage(glyph, pt, new Rectangle(Point.Empty, glyph.Size), GraphicsUnit.Pixel, ItemInfo.GetImageAttributes());
			}
		}
		protected internal virtual void DrawItem(ContextItemCollectionInfoArgs info) { }
	}
	#endregion
	#region ContextButton
	public class ContextButton : ContextItem {   
		int maxWidth;
		int maxHeight;
		public ContextButton() : base() { }
		protected override void Assign(ContextItem item) {
			BeginUpdate();
			try {
				base.Assign(item);
				ContextButton button = item as ContextButton;
				if(button == null)
					return;
				MaxWidth = button.MaxWidth;
				MaxHeight = button.MaxHeight;
				Padding = button.Padding;
				AllowHtmlText = button.AllowHtmlText;
				HyperLinkColor = button.HyperLinkColor;
				HyperlinkClick += button.HyperlinkClick;
				Caption = button.Caption;
				Width = button.Width;
				Height = button.Height;
			}
			finally {
				CancelUpdate();
			}
		}
		string caption = string.Empty;
		[DefaultValue("")]
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value)
					return;
				string prev = Caption;
				caption = value;
				OnItemChanged("Caption", prev, Caption);
			}
		}
		private int width;
		[DefaultValue(0)]
		public int Width {
			get { return width; }
			set {
				value = Math.Max(0, value);
				if(Width == value)
					return;
				int prev = Width;
				width = value;
				OnItemChanged("Width", prev, Width);
			}
		}
		private int height;
		[DefaultValue(0)]
		public int Height {
			get { return height; }
			set {
				value = Math.Max(0, value);
				if(Height == value)
					return;
				int prev = Height;
				height = value;
				OnItemChanged("Height", prev, Height);
			}
		}
		[DefaultValue(0)]
		public int MaxWidth { 
			get { return maxWidth;}
			set {
				value = Math.Max(0, value);
				if(MaxWidth == value) 
					return;
				int prev = MaxWidth;
				maxWidth = value;
				OnItemChanged("MaxWidth", prev, MaxWidth);
			} 
		}
		[DefaultValue(0)]
		public int MaxHeight {
			get { return maxHeight; }
			set {
				value = Math.Max(0, value);
				if(MaxHeight == value) return;
				int prev = MaxHeight;
				maxHeight = value;
				OnItemChanged("MaxHeight", prev, MaxHeight);
			}
		}
		Padding padding = new Padding(0);
		void ResetPadding() { Padding = new Padding(0); }
		bool ShouldSerializePadding() { return !IsEmptyPadding; }
		internal bool IsEmptyPadding { get { return Padding == new Padding(0); } }
		public Padding Padding {
			get { return padding; }
			set {
				if(Padding == value)
					return;
				Padding prev = Padding;
				padding = value;
				OnItemChanged("Padding", prev, Padding);
			}
		}
		DefaultBoolean allowHtmlText = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value)
					return;
				DefaultBoolean prev = AllowHtmlText;
				allowHtmlText = value;
				OnItemChanged("AllowHtmlText", prev, AllowHtmlText);
			}
		}
		void ResetHyperLinkColor() { HyperLinkColor = Color.Empty; }
		bool ShouldSerializeHyperLinkColor() { return !HyperLinkColor.IsEmpty; }
		Color hyperLinkColor = Color.Empty;
		public Color HyperLinkColor {
			get { return hyperLinkColor; }
			set {
				if(hyperLinkColor == value)
					return;
				Color prev = hyperLinkColor;
				hyperLinkColor = value;
				OnItemChanged("HyperLinkColor", prev, HyperLinkColor);
			}
		}
		protected internal override ContextItemViewInfo CreateViewInfo(ContextItemCollectionViewInfo collectionViewInfo) {
			return new ContextButtonViewInfo(this, collectionViewInfo);
		}
		public event HyperlinkClickEventHandler HyperlinkClick;
		protected internal void RaiseHyperlinkClick(HyperlinkClickEventArgs e) {
			if(HyperlinkClick == null) return;
			HyperlinkClick(this, e);
		}
	}
	public class ContextButtonViewInfo : ContextItemViewInfo {
		ContextButtonPainter painter;
		AppearanceObject paintAppearanceNormal;
		AppearanceObject paintAppearanceHover;
		AppearanceDefault defaultAppearance;
		Size textSize = Size.Empty;
		Items2Panel panel;
		Rectangle glyphBounds;
		Rectangle captionBounds;
		public ContextButtonViewInfo(ContextButton button, ContextItemCollectionViewInfo collectionViewInfo) : base(button, collectionViewInfo) { }
		public ContextButton Button { get { return Item as ContextButton; } }
		internal void ResetSize() { Size = Size.Empty; } 
		protected internal override void CalcBestSize() {
			UpdateAppearances();
			Size = CalculateItemSize();				 
		}
		public string Caption { get { return Button.Caption; } }
		public ContextButtonMode Mode {
			get {
				if(String.IsNullOrEmpty(Caption) && Glyph != null)
					return ContextButtonMode.Glyph;
				if(Button.Glyph != null && !String.IsNullOrEmpty(Caption))
					return ContextButtonMode.CaptionGlyph;
				return ContextButtonMode.Caption;
			}
		}
		protected internal Rectangle CaptionBounds {
			get {
				if(Caption == null)
					captionBounds = Rectangle.Empty;			   
				else
					if(Button.Glyph == null) {
						captionBounds.Location = ContentBounds.Location;
					}			  
				return captionBounds;
			}
		}
		protected internal override Rectangle GlyphBounds { 
			get {
				if(Button.Glyph == null)
					glyphBounds = Rectangle.Empty;
				else
					if(String.IsNullOrEmpty(Caption))
						glyphBounds.Location = ContentBounds.Location;
				return glyphBounds; 
			} 
		}
		protected internal bool AllowHtmlText {
			get {
				if(!IsHorizontal) return false;
				if(Button.AllowHtmlText != DefaultBoolean.Default)
					return Button.AllowHtmlText == DefaultBoolean.True;
				return ViewInfo.Options.AllowHtmlText;
			}
		}
		protected internal override ContextItemHitInfo CalcHitInfo(Point hitPoint) {
			ContextItemHitInfo hitInfo = base.CalcHitInfo(hitPoint);
			if(hitInfo == null)
				return hitInfo;
			ContextItemHitTest hitTest;
			if(GlyphBounds.Contains(hitPoint))
				hitTest = ContextItemHitTest.Glyph;
			else
				hitTest = (CaptionBounds.Contains(hitPoint)) ? ContextItemHitTest.Caption : ContextItemHitTest.None;			 
			hitInfo.SetHitTest(hitTest);		   
			return hitInfo;
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			if(Button.Glyph != null && !String.IsNullOrEmpty(Caption)) {
				CalcGlyphAndTextBounds();
			}
			if(AllowHtmlText)
				UpdateStringInfo(PaintAppearanceNormal);   
		}
		protected string GetButtonCaption() {
			if(Caption == string.Empty) return " ";
			return Caption;
		}
		public virtual Rectangle ContentBounds {
			get {
				return new Rectangle(Bounds.X + Button.Padding.Left, Bounds.Y + Button.Padding.Top, Bounds.Width - Button.Padding.Horizontal, Bounds.Height - Button.Padding.Vertical);
			}
		}
		protected internal AppearanceObject PaintAppearanceNormal {
			get {
				if(paintAppearanceNormal == null) {
					paintAppearanceNormal = new AppearanceObject();
					paintAppearanceNormal.TextOptions.RightToLeft = IsRightToLeft;
				}
				return paintAppearanceNormal;
			}
			set { paintAppearanceNormal = value; }
		}
		protected AppearanceObject AppearanceNormal { get { return Button.AppearanceNormal; } }
		protected internal AppearanceObject PaintAppearanceHover {
			get {
				if(paintAppearanceHover == null)
					paintAppearanceHover = new AppearanceObject();
				paintAppearanceHover.TextOptions.RightToLeft = IsRightToLeft;
				return paintAppearanceHover;
			}
			set { paintAppearanceHover = value; }
		}
		protected AppearanceObject AppearanceHover { get { return Button.AppearanceHover; } }
		protected AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected UserLookAndFeel LookAndFeel { get { return ViewInfo.Owner.LookAndFeel; } }
		public StringInfo StringInfo { get; set; }		
		protected internal virtual bool HasHyperlink { get { return AllowHtmlText && StringInfo.HasHyperlink; } }
		public Size MaxContentSize { 
			get {
				int maxWidth = ViewInfo.Owner.DisplayBounds.Width - 2 * ViewInfo.Indent - Button.Padding.Horizontal;
				int maxHeight = ViewInfo.Owner.DisplayBounds.Height - 2 * ViewInfo.Indent - Button.Padding.Vertical;
				int maxContentWidth = (Button.MaxWidth == 0) ? maxWidth : Math.Min(Button.MaxWidth - Button.Padding.Horizontal, maxWidth);
				int maxContentHeight = (Button.MaxHeight == 0) ? maxHeight : Math.Min(Button.MaxHeight - Button.Padding.Vertical, maxHeight);
				Size maxContentSize = new Size(maxContentWidth, maxContentHeight);
				return maxContentSize;				
			}
		}
		public Size MaxItemSize {
			get {
				int maxItemWidth = ViewInfo.Owner.DisplayBounds.Width - 2 * ViewInfo.Indent;
				int maxItemHeight = ViewInfo.Owner.DisplayBounds.Height - 2 * ViewInfo.Indent;
				Size maxItemSize = new Size(maxItemWidth, maxItemHeight);
				return maxItemSize;  
			}
		}
		public Items2Panel Panel { 
			get {
				if(panel == null)
					panel = new Items2Panel();
				return panel;
			}
		}
		protected virtual void UpdatePanelProperties() {
			Panel.Content1Location = GetGlyphLocation();
			Panel.Content1HorizontalAlignment = ViewInfo.Owner.GetGlyphHorizontalAlignment(Button);
			Panel.Content1VerticalAlignment = ViewInfo.Owner.GetGlyphVerticalAlignment(Button);
			Panel.Content2HorizontalAlignment = ViewInfo.Owner.GetCaptionHorizontalAlignment(Button);
			Panel.Content2VerticalAlignment = ViewInfo.Owner.GetCaptionVerticalAlignment(Button);
			Panel.HorizontalIndent = ViewInfo.Owner.GetGlyphToCaptionIndent(Button);
			Panel.VerticalIndent = ViewInfo.Owner.GetGlyphToCaptionIndent(Button);
		}
		ItemLocation GetGlyphLocation(){
			if(ViewInfo.Owner.GetGlyphLocation(Button) != ItemLocation.Default)
				return ViewInfo.Owner.GetGlyphLocation(Button);
			if(!IsHorizontal)
				return ItemLocation.Top;
			return ItemLocation.Default;
		}
		protected void UpdateHtmlTextBounds(Rectangle newBounds) {
			StringInfo.SetBounds(newBounds);
			UpdateHtmlTextBlocksBounds(newBounds);		   
		}
		protected void UpdateHtmlTextBlocksBounds(Rectangle newBounds) {
			if(StringInfo.Blocks == null) return;		   
			UpdateHtmlTextBlocksLocation(newBounds.Location.X, newBounds.Location.Y);			
		}
		protected void UpdateHtmlTextBlocksLocation(int indentX, int indentY) { 
			for(int i = 0; i < StringInfo.BlocksBounds.Count; i++)
				StringInfo.BlocksBounds[i] = new Rectangle(new Point(StringInfo.BlocksBounds[i].X + indentX, StringInfo.BlocksBounds[i].Y + indentY), StringInfo.BlocksBounds[i].Size);
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault defAppearance = new AppearanceDefault(GetSystemColor(SystemColors.WindowText), Color.Empty, GetDefaultFont());
			return defAppearance;
		}
		protected virtual Color GetSystemColor(Color color) {
			return LookAndFeelHelper.GetSystemColor(ViewInfo.Owner.LookAndFeel, color);
		}
		protected virtual Font GetDefaultFont() {
			if(!IsSkinLookAndFeel) return AppearanceObject.DefaultFont;
			IAppearanceDefaultFontProvider fp = LookAndFeel as IAppearanceDefaultFontProvider;
			if(fp != null) return fp.DefaultFont ?? AppearanceObject.DefaultFont;
			return AppearanceObject.DefaultFont;
		}
		protected virtual bool IsSkinLookAndFeel {
			get {
				if(LookAndFeel == null) return false;
				return LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin;
			}
		}
		public void UpdateAppearances() {
			AppearanceHelper.Combine(PaintAppearanceNormal, AppearanceNormal, DefaultAppearance);
			AppearanceHelper.Combine(PaintAppearanceHover, AppearanceHover, DefaultAppearance);
		}
		protected virtual void CalcGlyphAndTextBounds() {
		   Panel.ArrangeItems(ContentBounds, this.glyphBounds.Size, this.captionBounds.Size, ref this.glyphBounds, ref this.captionBounds);
		}
		protected virtual Size CalculateItemSize() {
			Size itemSize = Size.Empty;
			switch(Mode) {
				case ContextButtonMode.Glyph:
					this.glyphBounds.Size = CalcGlyphSize();
					itemSize = this.glyphBounds.Size;
					break;
				case ContextButtonMode.Caption:
					this.captionBounds.Size = CalcTextSize(MaxContentSize);
					itemSize = this.captionBounds.Size;
				break;
				case ContextButtonMode.CaptionGlyph:
					itemSize = CalcTextGlyphSize();
				break;
				default:
				break;
			}
			itemSize = new Size(itemSize.Width + Math.Abs(Button.Padding.Horizontal), itemSize.Height + Math.Abs(Button.Padding.Vertical));
			if(Button.Width > 0)
				itemSize.Width = Button.Width;
			if(Button.Height > 0)
				itemSize.Height = Button.Height;
			return itemSize;
		}
		protected virtual Size CalcTextGlyphSize() {
			UpdatePanelProperties();
			this.glyphBounds.Size = CalcGlyphSize();
			Size maxSize = new Size();
			if(Panel.Content1Location == ItemLocation.Left || Panel.Content1Location == ItemLocation.Right)
				maxSize = new Size(MaxContentSize.Width - this.glyphBounds.Width, MaxContentSize.Height);
			else
				maxSize = new Size(MaxContentSize.Width, MaxContentSize.Height - this.glyphBounds.Height);			 
			this.captionBounds.Size = CalcTextSize(maxSize);
			Size panelSize = Panel.CalcBestSize(this.glyphBounds.Size, this.captionBounds.Size);
			if(panelSize.Width > MaxContentSize.Width || panelSize.Height > MaxContentSize.Height) {
				int diffWidth = 0;
				int diffHeight = 0;
				if(panelSize.Width > MaxContentSize.Width) {
					diffWidth = panelSize.Width - MaxContentSize.Width;
				}
				if(panelSize.Height > MaxContentSize.Height) {
					diffHeight = panelSize.Height - MaxContentSize.Height;
				}
				Size maxTextSize = new Size(captionBounds.Width - diffWidth, captionBounds.Height - diffHeight);
				this.captionBounds.Size = CalcTextSize(maxTextSize);				
				panelSize = Panel.CalcBestSize(glyphBounds.Size, captionBounds.Size);
				if(panelSize.Width < glyphBounds.Size.Width || panelSize.Height < glyphBounds.Height)
					glyphBounds.Size = Size.Empty;
			}
			return panelSize;
		}
		protected virtual Size CalcTextSize(Size maxTextSize) {
			Size textSize = Size.Empty;
			if(AllowHtmlText)
				textSize = CalcHtmlTextSize(maxTextSize);
			else
				textSize = CalcSimpleTextSize(maxTextSize);
			return textSize;
		}		
		protected virtual Size CalcHtmlTextSize(Size maxTextSize) {
			Size normalSize = CalcHtmlTextSizeForState(false, PaintAppearanceNormal, maxTextSize);
			Size hotSize = CalcHtmlTextSizeForState(true, PaintAppearanceHover, maxTextSize);
			Size maxSize = new Size(Math.Max(normalSize.Width, hotSize.Width), Math.Max(normalSize.Height, hotSize.Height));
			return maxSize;
		}
		protected virtual Size CalcHtmlTextSizeForState(bool isHovered, AppearanceObject appearance, Size maxTextSize) {
			ViewInfo.GInfo.AddGraphics(null);
			try {
					appearance.TextOptions.WordWrap = WordWrap.Wrap;
					return CalcHtmlTextSizeCore(ViewInfo.GInfo.Graphics, appearance, false, maxTextSize);
			}
			finally { ViewInfo.GInfo.ReleaseGraphics(); }
		}
		protected virtual Size CalcHtmlTextSizeCore(Graphics g, AppearanceObject appearance, bool assignStrInfo, Size maxTextSize) {
			StringInfo strInfo = null;
			Size textSize = Size.Empty;
			strInfo = StringPainter.Default.Calculate(g, appearance, appearance.TextOptions, GetHyperLinkColor(), GetButtonCaption(), maxTextSize.Width, null, this);			
			if(strInfo.Bounds.Size.Height > maxTextSize.Height) {
				strInfo = StringPainter.Default.Calculate(g, appearance, appearance.TextOptions, GetHyperLinkColor(), GetButtonCaption(), new Rectangle(0, 0, strInfo.Bounds.Size.Width, maxTextSize.Height), null, this);
				textSize = new Size(strInfo.Bounds.Size.Width, maxTextSize.Height);
			}
			else
				textSize = strInfo.Bounds.Size;
			if(assignStrInfo)
				StringInfo = strInfo;
			return textSize;	
		}		
		protected void UpdateStringInfo(AppearanceObject appearance) 
		{
			ViewInfo.GInfo.AddGraphics(null);
			try {
				appearance.TextOptions.WordWrap = WordWrap.Wrap;
				CalcHtmlTextSizeCore(ViewInfo.GInfo.Graphics, appearance, true, captionBounds.Size);
			}
			finally { 
				UpdateHtmlTextBounds(CaptionBounds);
				ViewInfo.GInfo.ReleaseGraphics(); }
		}	   
		protected virtual Color GetHyperLinkColor() {
			UserLookAndFeel lookAndFeel = ViewInfo.Owner.LookAndFeel;
			if(Button.HyperLinkColor != Color.Empty)
				return Button.HyperLinkColor;
			if(lookAndFeel != null)
				return EditorsSkins.GetSkin(lookAndFeel.ActiveLookAndFeel).Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor, Color.Blue);
			return Color.YellowGreen;
		}
		protected virtual Size CalcGlyphSize() {
			if(Button.Glyph != null)
				return Button.Glyph.Size;
			return Size.Empty;
		}
		protected virtual Size CalcSimpleTextSize(Size maxTextSize) {
			Size normalSize = CalcSimpleTextSizeCore(ContextItemState.Normal, maxTextSize);
			Size hotSize = CalcSimpleTextSizeCore(ContextItemState.Hot, maxTextSize);
			Size maxSize = new Size(Math.Max(normalSize.Width, hotSize.Width), Math.Max(normalSize.Height, hotSize.Height));					   
			return maxSize;
		}
		protected virtual Size CalcSimpleTextSizeCore(ContextItemState state, Size maxTextSize) {
			Size textSize = CalculateSimpleTextSize(maxTextSize.Width, maxTextSize.Height, state).ToSize();
			int height = Math.Min(maxTextSize.Height, textSize.Height);
			textSize = new Size(textSize.Width, height);
			return textSize;
		}
		protected virtual SizeF CalculateSimpleTextSize(int maxWidth, int maxHeight, ContextItemState state) {
			ViewInfo.GInfo.AddGraphics(null);
			try {
				if(IsHorizontal)
					if(!IsHovered)
						return PaintAppearanceNormal.CalcTextSize(ViewInfo.GInfo.Graphics, PaintAppearanceNormal.GetStringFormat(), GetButtonCaption(), maxWidth, maxHeight).ToSize();
					else
						return PaintAppearanceHover.CalcTextSize(ViewInfo.GInfo.Graphics, PaintAppearanceHover.GetStringFormat(), GetButtonCaption(), maxWidth, maxHeight).ToSize();
				else {
					using(StringFormat strFormat = (StringFormat)PaintAppearanceNormal.GetStringFormat().Clone()) {
						strFormat.FormatFlags = StringFormatFlags.DirectionVertical;
						return ViewInfo.GInfo.Graphics.MeasureString(GetButtonCaption(), paintAppearanceNormal.Font, maxWidth, strFormat);
					}
				}
			}
			finally { ViewInfo.GInfo.ReleaseGraphics(); }
		}	   
		void AppearanceNormal_SizeChanged(object sender, EventArgs e) {
			OnAppearanceSizeChanged();
		}
		void AppearanceNormal_PaintChanged(object sender, EventArgs e) {
			OnAppearancePaintChanged();
		}
		void AppearanceHover_SizeChanged(object sender, EventArgs e) {
			OnAppearanceSizeChanged();
		}
		void AppearanceHover_PaintChanged(object sender, EventArgs e) {
			OnAppearancePaintChanged();
		}
		void OnAppearanceSizeChanged() {
			if(Item.IsLockUpdate)
				return;
			UpdateAppearances();
			CalcBestSize();
			ViewInfo.RecalculateItems(Button.Alignment);
			ViewInfo.Owner.Redraw(ViewInfo.GetItemsBounds(Button.Alignment));
		}
		void OnAppearancePaintChanged() {
			if(Item.IsLockUpdate)
				return;
			UpdateAppearances();
			ViewInfo.Owner.Redraw(ContentBounds);
		}
		public override bool ShouldRedrawItem() {
			switch(Mode) {
				case ContextButtonMode.Glyph:
					return ShouldRedrawGlyphItem();
				case ContextButtonMode.Caption:
					return ShouldRedrawCaptionItem();
				case ContextButtonMode.CaptionGlyph:
					return ShouldRedrawCaptionGlyphItem();
				default:
					return false;			   
			}		 
		}
		protected bool ShouldRedrawGlyphItem() {
			if(!(PaintAppearanceNormal.IsEqual(PaintAppearanceHover))) return true;
			if(Button.Glyph != Button.HoverGlyph && Button.HoverGlyph != null) {
				RedrawBounds = ContentBounds;
				return true;
			}
			else
				return false;
		}
		protected bool ShouldRedrawCaptionItem() {
			bool shouldRedrawItem = !(PaintAppearanceNormal.IsEqual(PaintAppearanceHover));
			if(!shouldRedrawItem) return false;
			UpdateStringInfoForCurrentState();
			RedrawBounds = ContentBounds;
			return true;
		}
		protected bool ShouldRedrawCaptionGlyphItem() {
			bool shouldRedrawGlyph = Button.Glyph != Button.HoverGlyph && Button.HoverGlyph != null;
			bool shouldRedrawCaption = !(PaintAppearanceNormal.IsEqual(PaintAppearanceHover));
			bool shouldRedrawItem = shouldRedrawGlyph || shouldRedrawCaption;
			if(!shouldRedrawItem) return false;
			if(shouldRedrawGlyph && shouldRedrawCaption) {
				UpdateStringInfoForCurrentState();
				RedrawBounds = ContentBounds;
			}
			else {
				if(shouldRedrawGlyph) {
					RedrawBounds = GlyphBounds;
				}
				else {
					UpdateStringInfoForCurrentState();
					RedrawBounds = CaptionBounds;
				}
			}
			return true;
		}
		protected void UpdateStringInfoForCurrentState() {
			if(AllowHtmlText) {
				if(!IsHovered)
					UpdateStringInfo(PaintAppearanceNormal);
				else
					UpdateStringInfo(PaintAppearanceHover);
			}
		}
		protected internal override ContextItemPainter Painter {
			get {
				if(painter == null)
					painter = new ContextButtonPainter(this);
				return painter;
			}
		}
		public override void OnItemClick(MouseEventArgs e) {
			TryHyperlinkClick(e.Location);
			base.OnItemClick(e);
		}	 
		private void TryHyperlinkClick(Point point) {
			if(!HasHyperlink)
				return;			
			StringBlock block = StringInfo.GetLinkByPoint(point);
			if(block != null)
				Button.RaiseHyperlinkClick(new HyperlinkClickEventArgs() { Text = block.Text, Link = block.Link });
		}
		public RectangleF GetAnimatedBackgroundRect() {
			RectangleF start = new RectangleF(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2, 0, 0);
			RectangleF end = Bounds;
			RectangleF rect = CalcOutAnimationRect(start, end, ViewInfo.GetCurrentAnimationCoeff());
			return rect;
		}
		protected internal override void MakeOffset(int deltaX, int deltaY) {
			base.MakeOffset(deltaX, deltaY);
			if(AllowHtmlText) {
				StringInfo.UpdateLocation(new Point(deltaX, deltaY));
			}
		}
	}
	public class ContextButtonPainter : ContextItemPainter {
		public ContextButtonPainter(ContextButtonViewInfo viewInfo) : base(viewInfo) { }
		public ContextButtonViewInfo ButtonInfo { get { return ItemInfo as ContextButtonViewInfo; } }
		public ContextButton Button { get { return ButtonInfo.Button; } }
		protected AppearanceObject GetCurrentAppearance() {
			return ItemInfo.IsHovered ? ButtonInfo.PaintAppearanceHover : ButtonInfo.PaintAppearanceNormal;
		}
		protected void UpdateOpacity(AppearanceObject appearance, int opacity) {
			if(!appearance.BackColor.IsEmpty)
				appearance.BackColor = Color.FromArgb(opacity, appearance.BackColor);
			if(!appearance.BackColor2.IsEmpty)
				appearance.BackColor2 = Color.FromArgb(opacity, appearance.BackColor2);		   
		}
		protected virtual void DrawBackground(ContextItemCollectionInfoArgs info) {
			AppearanceObject appearance = GetCurrentAppearance();
			if(Options.GetAnimationType() != ContextAnimationType.None && Button.Visibility == ContextItemVisibility.Auto) {
				if(ButtonInfo.Mode == ContextButtonMode.Glyph && Button.IsEmptyPadding) return;
				if(Options.GetAnimationType() == ContextAnimationType.OpacityAnimation)
					UpdateOpacity(appearance, GetItemOpacity());
				if(Options.GetAnimationType() == ContextAnimationType.OutAnimation) {
					RectangleF rect = ButtonInfo.GetAnimatedBackgroundRect();
					Rectangle brushRect = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
					info.Graphics.FillRectangle(appearance.GetBackBrush(info.Cache, brushRect), rect);
					return;
				}
			}
			info.Graphics.FillRectangle(appearance.GetBackBrush(info.Cache, ButtonInfo.Bounds), ButtonInfo.Bounds);
		}
		protected internal override void DrawItem(ContextItemCollectionInfoArgs info) {
			if(ButtonInfo.IsNotFittedWidth || ButtonInfo.IsNotFittedHeight || ButtonInfo.Bounds.IsEmpty || ButtonInfo.Bounds.Size.IsEmpty) return;
			ItemInfo.UpdateOpacity();
			DrawBackground(info);			
			switch(ButtonInfo.Mode) {
				case ContextButtonMode.Glyph:
					DrawGlyph(info);
					break;
				case ContextButtonMode.Caption:
					DrawText(info);
					break;
				case ContextButtonMode.CaptionGlyph:
					DrawGlyph(info);
					DrawText(info);
					break;
				default:
					break;
			}	  
		}
		protected virtual void DrawGlyph(ContextItemCollectionInfoArgs info) {
			if(!ButtonInfo.IsHovered)
				DrawGlyphCore(info, ButtonInfo.Glyph);			 
			else {
				if(ButtonInfo.Button.HoverGlyph == null) 
					DrawGlyphCore(info, ButtonInfo.Glyph);			  
				else
					DrawGlyphCore(info, ButtonInfo.HoverGlyph);				 
			}			
		}
		protected virtual bool ShouldDrawText() { 
			if(ItemInfo.Item.Visibility == ContextItemVisibility.Visible)
				return true;
			if(ItemInfo.Item.Visibility == ContextItemVisibility.Hidden)
				return false;
			if(ItemInfo.GetAnimationType() == ContextAnimationType.OpacityAnimation ||
				ItemInfo.GetAnimationType() == ContextAnimationType.OutAnimation)
				return ItemInfo.Opacity > ItemInfo.ViewInfo.HoverItemOpacity * 0.5f;
			return true;
		}
		protected virtual void DrawText(ContextItemCollectionInfoArgs info) {
			AppearanceObject appearance = GetCurrentAppearance();
			if(ShouldDrawText()) {
				if(ButtonInfo.AllowHtmlText)
					StringPainter.Default.DrawString(info.Cache, ButtonInfo.StringInfo);
				else {
					if(ButtonInfo.IsHorizontal)
						appearance.DrawString(info.Cache, ButtonInfo.Caption, ButtonInfo.CaptionBounds);
					else
						appearance.DrawVString(info.Cache, ButtonInfo.Caption, appearance.Font, info.Cache.GetSolidBrush(appearance.GetForeColor()), ButtonInfo.CaptionBounds, appearance.GetStringFormat(), 90);
				}
			}
		}
	}
	#endregion
	#region CheckContextButton
	public class CheckContextButton : ContextButton {
		public CheckContextButton() : base() { }
		protected override void Assign(ContextItem item) {
			BeginUpdate();
			try {
				base.Assign(item);
				CheckContextButton button = item as CheckContextButton;
				if(button == null)
					return;
				Checked = button.Checked;
				CheckedGlyph = button.CheckedGlyph;
				HoverCheckedGlyph = button.HoverCheckedGlyph;
			}
			finally {
				CancelUpdate();
			}
		}
		bool isChecked = false;
		[DefaultValue(false)]
		public bool Checked {
			get { return isChecked; }
			set {
				if(Checked == value)
					return;
				bool prev = Checked;
				isChecked = value;
				OnItemChanged("Checked", prev, Checked);
			}
		}
		Image checkedGlyph;
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image CheckedGlyph {
			get { return checkedGlyph; }
			set {
				if(CheckedGlyph == value)
					return;
				Image prev = CheckedGlyph;
				checkedGlyph = value;
				OnItemChanged("CheckedGlyph", prev, CheckedGlyph);
			}
		}
		Image hoverCheckedGlyph;
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image HoverCheckedGlyph {
			get { return hoverCheckedGlyph; }
			set {
				if(HoverCheckedGlyph == value)
					return;
				Image prev = HoverCheckedGlyph;
				hoverCheckedGlyph = value;
				OnItemChanged("HoverCheckedGlyph", prev, HoverCheckedGlyph);
			}
		}
		protected internal override ContextItemViewInfo CreateViewInfo(ContextItemCollectionViewInfo collectionViewInfo) {
			return new CheckContextButtonViewInfo(this, collectionViewInfo);
		}
	}
	public class CheckContextButtonViewInfo : ContextItemViewInfo {
		CheckContextButtonPainter painter;
		public CheckContextButtonViewInfo(CheckContextButton owner, ContextItemCollectionViewInfo collectionViewInfo)
			: base(owner, collectionViewInfo) {
			Checked = owner.Checked;		   
		}
		Image GetSkinImage(int index) {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemCheckBox];
			return elem.Image.GetImages().Images[index];
		}
		protected internal override Rectangle GlyphBounds {
			get { return new Rectangle(Bounds.Location, Glyph.Size); }
		}
		Image defaultGlyph;
		protected override Image DefaultGlyph {
			get { 
				if(defaultGlyph == null)
					defaultGlyph = GetSkinImage(4);
				return defaultGlyph;
			}
		}
		Image defaultCheckedGlyph;
		protected virtual Image DefaultCheckedGlyph {
			get {
				if(defaultCheckedGlyph == null) {
					defaultCheckedGlyph = GetSkinImage(0);
				}
				return defaultCheckedGlyph;
			}
		}
		protected internal virtual Image CheckedGlyph {
			get {
				if(Button.CheckedGlyph != null)
					return Button.CheckedGlyph;
				return DefaultCheckedGlyph;
			}
		}
		Image defaultHoverCheckedGlyph;
		protected virtual Image DefaultHoverCheckedGlyph {
			get {
				if(defaultHoverCheckedGlyph == null)
					defaultHoverCheckedGlyph = GetSkinImage(1);
				return defaultHoverCheckedGlyph;
			}
		}
		Image defaultHoverGlyph;
		protected override Image DefaultHoverGlyph {
			get {
				if(defaultHoverGlyph == null)
					defaultHoverGlyph = GetSkinImage(5);
				return defaultHoverGlyph;
			}
		}
		protected internal virtual Image HoverCheckedGlyph {
			get {
				if(Button.HoverCheckedGlyph != null)
					return Button.HoverCheckedGlyph;
				return DefaultHoverCheckedGlyph;
			}
		}
		protected internal override ContextItemPainter Painter {
			get {
				if(painter == null)
					painter = new CheckContextButtonPainter(this);
				return painter;
			}
		}
		public bool Checked {
			get;
			set;
		}
		public CheckContextButton Button { get { return (CheckContextButton)Item; } }		
		public override bool ShouldRedrawItem() {
			if(Glyph == null) return false;
			if(!Checked) {
				if(Glyph != HoverGlyph && HoverGlyph != null) {
					RedrawBounds = Bounds;
					return true;
				}
			}
			else {
				if(CheckedGlyph != HoverCheckedGlyph && HoverCheckedGlyph != null) {
					RedrawBounds = Bounds;
					return true;
				}
			}   
			return false;
		}
		protected CheckContextButton CheckItem { get { return (CheckContextButton)Item; } }
		public override void OnItemClick(MouseEventArgs e) {
			Checked = !Checked;
			CheckItem.Checked = Checked;
			base.OnItemClick(e);
		}
		public RectangleF GetAnimatedRect() {
			RectangleF start = new RectangleF(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2, 0, 0);
			RectangleF end = Bounds;
			RectangleF rect = CalcOutAnimationRect(start, end, ViewInfo.GetCurrentAnimationCoeff());
			return rect;
		}
		protected internal override ContextItemHitInfo CalcHitInfo(Point hitPoint) {
			ContextItemHitInfo hitInfo = base.CalcHitInfo(hitPoint);
			if(hitInfo == null)
				return hitInfo;
			ContextItemHitTest hitTest = (GlyphBounds.Contains(hitPoint)) ? ContextItemHitTest.Glyph : ContextItemHitTest.None;
			hitInfo.SetHitTest(hitTest);
			return hitInfo;
		}
	}
	public class ContextItemCollectionInfoArgs : GraphicsInfoArgs {
		public ContextItemCollectionInfoArgs(ContextItemCollectionViewInfo viewInfo, GraphicsCache cache, Rectangle bounds) : base(cache, bounds) {
			ViewInfo = viewInfo;
		}
		public ContextItemCollectionInfoArgs(ContextItemCollectionViewInfo viewInfo, GraphicsInfoArgs info, Rectangle bounds) : base(info, bounds) {
			ViewInfo = viewInfo;
		}
		public ContextItemCollectionViewInfo ViewInfo { get; set; }
		public bool SuppressDrawAutoItems { get; set; }
	}
	public class CheckContextButtonPainter : ContextItemPainter {
		public CheckContextButtonPainter(CheckContextButtonViewInfo viewInfo) : base(viewInfo) { }
		public CheckContextButtonViewInfo CheckViewInfo { get { return ItemInfo as CheckContextButtonViewInfo; } }
		public CheckContextButton Button { get { return (CheckContextButton)(ItemInfo.Item); } }
		private void DrawAnimatedCheckButton(ContextItemCollectionInfoArgs info, Image glyph) {
			if(Options.GetAnimationType() == ContextAnimationType.OpacityAnimation)
				info.Graphics.DrawImage(glyph, ItemInfo.Bounds, 0, 0, ItemInfo.Bounds.Width, ItemInfo.Bounds.Height, GraphicsUnit.Pixel, ItemInfo.GetImageAttributes());
			if(Options.GetAnimationType() == ContextAnimationType.OutAnimation) {
				RectangleF r = CheckViewInfo.GetAnimatedRect();
				PointF[] pt = new PointF[] { 
					new PointF(r.X, r.Y),
					new PointF(r.Right, r.Y),
					new PointF(r.X, r.Bottom)
				};
				info.Graphics.DrawImage(glyph, pt, new RectangleF(0, 0, glyph.Width, glyph.Height), GraphicsUnit.Pixel, ItemInfo.GetImageAttributes());
			}
		}
		private void DrawCheckButton(ContextItemCollectionInfoArgs info, Image glyph, Image checkGlyph) {
			if(CheckViewInfo.Checked)
				DrawGlyphCore(info, checkGlyph);
			else
				DrawGlyphCore(info, glyph);
		}
		private void DrawHoverCheckButton(ContextItemCollectionInfoArgs info) {
			if(Button.HoverGlyph == null && Button.HoverCheckedGlyph == null) {
				DrawCheckButton(info, CheckViewInfo.HoverGlyph, CheckViewInfo.CheckedGlyph);
				return;
			}
			if(Button.HoverGlyph == null) {
				DrawCheckButton(info, CheckViewInfo.Glyph, CheckViewInfo.HoverCheckedGlyph);
				return;
			}
			if(Button.HoverCheckedGlyph == null) {
				DrawCheckButton(info, CheckViewInfo.HoverGlyph, CheckViewInfo.CheckedGlyph);
				return;
			}
			DrawCheckButton(info, CheckViewInfo.HoverGlyph, CheckViewInfo.HoverCheckedGlyph);
		}
		protected internal override void DrawItem(ContextItemCollectionInfoArgs info) {
			if(CheckViewInfo.IsNotFittedWidth || CheckViewInfo.IsNotFittedHeight|| CheckViewInfo.Bounds.IsEmpty) return;
			ItemInfo.UpdateOpacity();
			if(!ItemInfo.IsHovered)
				DrawCheckButton(info, CheckViewInfo.Glyph, CheckViewInfo.CheckedGlyph);
			else
				DrawHoverCheckButton(info);
		}		 
	}
	#endregion
	#region TrackBarContextButton
	enum TrackBarContextButtonHitTest { None, ZoomInButton, ZoomOutButton, Thumb, TrackLine }
	public class TrackBarContextButton : ContextItem {
		int minimum, maximum, value, middle;
		int trackWidth;
		int smallChange;
		bool showZoomButtons;
		bool allowUseMiddleValue;
		public TrackBarContextButton() {
			this.minimum = 0;
			this.maximum = 500;
			this.value = 100;
			this.showZoomButtons = true;
			this.trackWidth = -1;
			this.smallChange = 1;
			this.allowUseMiddleValue = false;
			this.middle = 100;
		}
		protected override void Assign(ContextItem item) {
			BeginUpdate();
			try {
				base.Assign(item);
				TrackBarContextButton trackBar = item as TrackBarContextButton;
				if(trackBar == null)
					return;
				Value = trackBar.Value;
				Minimum = trackBar.Minimum;
				Maximum = trackBar.Maximum;
				ShowZoomButtons = trackBar.ShowZoomButtons;
				TrackWidth = trackBar.TrackWidth;
				SmallChange = trackBar.SmallChange;
				Middle = trackBar.Middle;
				AllowUseMiddleValue = trackBar.AllowUseMiddleValue;
			}
			finally {
				CancelUpdate();
			}
		}
		protected internal override ContextItemViewInfo CreateViewInfo(ContextItemCollectionViewInfo collectionViewInfo) {
			return new TrackBarContextButtonViewInfo(this, collectionViewInfo);
		}
		[DefaultValue(0)]
		public int Minimum {
			get { return minimum; }
			set {
				if(Minimum == value) return;
				int prev = minimum;
				if(value > Maximum) value = Maximum;
				minimum = value;
				OnItemChanged("Minimum", prev, minimum);
				if(!(IsMiddleValidCore(Middle))) RefreshMiddleCore();
			}
		}
		[DefaultValue(10)]
		public int Maximum {
			get { return maximum; }
			set {
				if(Maximum == value) return;
				int prev = maximum;
				if(value < Minimum) value = Minimum;
				maximum = value;
				OnItemChanged("Maximum", prev, maximum);
				if(!(IsMiddleValidCore(Middle))) RefreshMiddleCore();
			}
		}
		[DefaultValue(100)]
		public int Value {
			get { return CheckValue(this.value); }
			set {
				if(Value == value) return;
				int prev = this.value;
				if(value < Minimum ) value = Minimum;
				if(value > Maximum) value = Maximum;
				this.value = value;
				OnItemChanged("Value", prev, value);
			}
		}
		protected internal virtual int CheckValue(int val) {
			if(val < Minimum) return Minimum;
			if(val > Maximum) return Maximum;
			return val;
		}
		[DefaultValue(true)]
		public bool ShowZoomButtons {
			get { return showZoomButtons; }
			set {
				if(ShowZoomButtons == value) return;
				showZoomButtons = value;
				OnItemChanged("ShowZoomButtons", !showZoomButtons, showZoomButtons);
			}
		}
		[DefaultValue(-1)]
		public int TrackWidth {
			get { return trackWidth; }
			set {
				if(TrackWidth == value) return;
				int prev = trackWidth;
				trackWidth = value;
				OnItemChanged("TrackWidth", prev, trackWidth);
			}
		}
		[DefaultValue(1)]
		public int SmallChange {
			get { return smallChange; }
			set {
				if(SmallChange == value) return;
				int prev = smallChange;
				smallChange = value;
				OnItemChanged("SmallChange", prev, smallChange);
			}
		}
		public int Middle {
			get { return middle; }
			set {
				if(Middle == value || !AllowUseMiddleValue) return;
				if(!IsMiddleValidCore(value)) {
					return;
				}
				int prev = middle;
				middle = value;
				OnItemChanged("Middle", prev, middle);
			}
		}
		[DefaultValue(false)]
		public bool AllowUseMiddleValue {
			get { return allowUseMiddleValue; }
			set {
				if(AllowUseMiddleValue == value) return;
				bool prev = allowUseMiddleValue;
				if(value && !IsMiddleValidCore(Middle)) RefreshMiddleCore();
				allowUseMiddleValue = value;
				OnItemChanged("AllowUseMiddleValue", prev, allowUseMiddleValue);
			}
		}
		protected virtual bool IsMiddleValidCore(int value) {
			return value < Maximum && value > Minimum;
		}
		protected virtual void RefreshMiddleCore() {
			if(AllowUseMiddleValue && Maximum - Minimum <= 1) {
				AllowUseMiddleValue = false;
				return;
			}
			Middle = (Maximum + Minimum) / 2;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image Glyph {
			get { return base.Glyph; }
			set { base.Glyph = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image HoverGlyph {
			get { return base.HoverGlyph; }
			set { base.HoverGlyph = value; }
		}
	}
	public class TrackBarContextButtonViewInfo : ContextItemViewInfo {
		Rectangle trackBounds, thumbBounds, zoomInBounds, zoomOutBounds, middleLineBounds, pointsRect;
		static int distanceFromButtonsToTrack = 5;
		TrackBarContextButtonPainter painter;
		int currentValue;
		public TrackBarContextButtonViewInfo(TrackBarContextButton owner, ContextItemCollectionViewInfo collectionViewInfo) 
			: base(owner, collectionViewInfo) {
				currentValue = owner.Value;
				this.zoomInBounds = this.zoomOutBounds = Rectangle.Empty;
				this.thumbBounds = this.trackBounds = this.middleLineBounds = Rectangle.Empty;
		}
		TrackBarContextButton TrackBarButton { get { return Item as TrackBarContextButton; } }
		public int CurrentValue {
			get { return currentValue; }
			set {
				if(CurrentValue == value) return;
				if(value < Button.Minimum) value = Button.Minimum;
				if(value > Button.Maximum) value = Button.Maximum;
				currentValue = value;
				OnCurrentValueChanged();
			}
		}
		void OnCurrentValueChanged() {
			Button.Value = CurrentValue;
		}
		public TrackBarContextButton Button { get { return (TrackBarContextButton)Item; } }
		protected internal override ContextItemPainter Painter {
			get {
				if(painter == null)
					painter = new TrackBarContextButtonPainter(this);
				return painter;
			}
		}
		#region Images
		protected internal Image GetThumbImage() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemTrackBarThumb];
			return elem.Image.GetImages().Images[0];
		}
		protected internal SkinElementInfo GetThumbInfo() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemTrackBarThumb];
			SkinElementInfo info = new SkinElementInfo(elem, ThumbBounds);
			info.ImageIndex = 0;
			if(GetAnimationType() == ContextAnimationType.OpacityAnimation) {
				info.Attributes = GetImageAttributes();
			}
			return info;
		}
		protected internal Image GetZoomInButtonImage() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemZoomInButton];
			int index = 0;
			if(HitInfo != null && HitInfo.HitTest == ContextItemHitTest.ZoomInButton) index = 1;
			if(PressedInfo != null && PressedInfo.HitTest == ContextItemHitTest.ZoomInButton) index = 2;
			return elem.Image.GetImages().Images[index];
		}
		protected internal Image GetZoomOutButtonImage() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemZoomOutButton];
			int index = 0;
			if(HitInfo != null && HitInfo.HitTest == ContextItemHitTest.ZoomOutButton) index = 1;
			if(PressedInfo != null && PressedInfo.HitTest == ContextItemHitTest.ZoomOutButton) index = 2;
			return elem.Image.GetImages().Images[index];
		}
		protected internal Image GetTrackImage() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemTrackBarTrack];
			return elem.Image.GetImages().Images[0];
		}
		protected internal SkinElementInfo GetTrackInfo() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemTrackBarTrack];
			SkinElementInfo info =  new SkinElementInfo(elem, TrackBounds);
			info.ImageIndex = 0;
			if(GetAnimationType() == ContextAnimationType.OpacityAnimation) {
				info.Attributes = GetImageAttributes();
			}
			return info;
		}
		protected internal Image GetMiddleLineImage() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemTrackBarMiddleLine];
			return elem.Image.GetImages().Images[0];
		}
		protected internal SkinElementInfo GetMiddleLineInfo() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemTrackBarMiddleLine];
			SkinElementInfo info = new SkinElementInfo(elem, MiddleLineBounds);
			info.ImageIndex = 0;
			if(GetAnimationType() == ContextAnimationType.OpacityAnimation) {
				info.Attributes = GetImageAttributes();
			}
			return info;
		}
		#endregion
		protected internal Rectangle TrackBounds {
			get { return trackBounds; }
		}
		protected internal Rectangle ThumbBounds {
			get { return thumbBounds; }
		}
		protected internal Rectangle ZoomInButtonBounds {
			get { return zoomInBounds; }
		}
		protected internal Rectangle ZoomOutButtonBounds {
			get { return zoomOutBounds; }
		}
		 protected internal Rectangle MiddleLineBounds {
			get { return middleLineBounds; }
		}
		protected internal int TrackWidth {
			get { return Button.TrackWidth == -1 ? 80 : Button.TrackWidth;}
		}
		protected Rectangle PointsRect {
			get { return pointsRect; }
			set { pointsRect = value; }
		}
		#region TrackBarHandler
		public bool IsThumbPressed { get { return PressedInfo != null && PressedInfo.HitTest == ContextItemHitTest.Thumb; } }
		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(PressedInfo.HitTest == ContextItemHitTest.Thumb) {
				Button.Value = CurrentValue;
			}
		}
		public override void OnMouseMove(MouseEventArgs e) {
			if(IsThumbPressed) {
				CurrentValue = Pos2Value(PointToClient(e.Location).X);
			}
			base.OnMouseMove(e);
		}
		protected internal override void OnHoverOut() {
			base.OnHoverOut();
			if(HitInfo != null)
				HitInfo.SetHitTest(ContextItemHitTest.None);
		}
		public override void OnItemClick(MouseEventArgs e) {
			base.OnItemClick(e);
			if(PressedInfo.HitTest == ContextItemHitTest.TrackLine) {
				CurrentValue = Pos2Value(PointToClient(e.Location).X);
			}
			if(PressedInfo.HitTest == ContextItemHitTest.ZoomInButton) { CurrentValue += Button.SmallChange; }
			if(PressedInfo.HitTest == ContextItemHitTest.ZoomOutButton) { CurrentValue -= Button.SmallChange; }
		}
		#endregion 
		protected internal override ContextItemHitInfo CalcHitInfo(Point hitPoint) {
			ContextItemHitInfo hitInfo;
			if(!Bounds.Contains(hitPoint)) {
				return null;
			}
			hitInfo = new ContextItemHitInfo(hitPoint, this);
			ContextItemHitTest hitTest;
			if(ThumbBounds.Contains(hitPoint)) 
				hitTest = ContextItemHitTest.Thumb;
			else if(TrackBounds.Contains(hitPoint))
				hitTest = ContextItemHitTest.TrackLine;
			else if(ZoomInButtonBounds.Contains(hitPoint))
				hitTest = ContextItemHitTest.ZoomInButton;
			else if(ZoomOutButtonBounds.Contains(hitPoint))
				hitTest = ContextItemHitTest.ZoomOutButton;
			else
				hitTest = ContextItemHitTest.None;
			hitInfo.SetHitTest(hitTest);
			return hitInfo;
		}
		public override void CalcViewInfo(Rectangle bounds) {
			CurrentValue = TrackBarButton.Value;
			base.CalcViewInfo(bounds);
			if(!IsHorizontal)
				bounds = VerticalToHorizontal(bounds);
			if(Button.ShowZoomButtons) {
				zoomInBounds = CalcZoomInButtonBounds(bounds);
				zoomOutBounds = CalcZoomOutButtonBounds(bounds);
			}
			trackBounds = CalcTrackBounds(bounds);
			PointsRect = CalcPointsRect();
			if(Button.AllowUseMiddleValue)
				middleLineBounds = CalcMiddleLineBounds();
			thumbBounds = CalcThumbBounds(bounds);
			if(!IsHorizontal)
				CalcVerticalRects(bounds);
		}
		void CalcVerticalRects(Rectangle bounds) {
			zoomInBounds = HorizontalToVertical(zoomInBounds);
			zoomOutBounds = HorizontalToVertical(zoomOutBounds);
			trackBounds = HorizontalToVertical(trackBounds);
			middleLineBounds = HorizontalToVertical(middleLineBounds);
			thumbBounds = HorizontalToVertical(thumbBounds);
		}
		Rectangle VerticalToHorizontal(Rectangle vertRect) {
			return new Rectangle(vertRect.X, vertRect.Y - vertRect.Width, vertRect.Height, vertRect.Width);
		}
		Rectangle HorizontalToVertical(Rectangle horzRect) {
			return new Rectangle(Bounds.Y - horzRect.Bottom + Bounds.X, horzRect.Left - Bounds.X + Bounds.Y, horzRect.Height, horzRect.Width);
		}
		Rectangle CalcMiddleLineBounds() {
			if(GetMiddleLineImage() == null || GetMiddleLineImage().Size.Width >= TrackBounds.Width) return Rectangle.Empty;
			Rectangle rect = Rectangle.Empty;
			rect.Size = GetMiddleLineImage().Size;
			rect.X = TrackBounds.X + (TrackBounds.Width - rect.Width) / 2;
			rect.Y = TrackBounds.Y + (TrackBounds.Height - rect.Height) / 2;
			return rect;
		}
		Rectangle CalcZoomOutButtonBounds(Rectangle bounds) {
			if(GetZoomOutButtonImage() == null) return Rectangle.Empty;
			Rectangle rect = Rectangle.Empty;
			rect.Size = GetZoomOutButtonImage().Size;
			rect.Y = bounds.Y + (bounds.Height - rect.Height) / 2;
			if(IsRightToLeft) rect.X = bounds.Right - rect.Width;
			else rect.X = bounds.X;
			return rect;
		}
		Rectangle CalcZoomInButtonBounds(Rectangle bounds) {
			if(GetZoomInButtonImage() == null) return Rectangle.Empty;
			Rectangle rect = Rectangle.Empty;
			rect.Size = GetZoomInButtonImage().Size;
			rect.Y = bounds.Y + (bounds.Height - rect.Height) / 2;
			if(IsRightToLeft) rect.X = bounds.X;
			else rect.X = bounds.Right - rect.Width;
			return rect;
		}
		Rectangle CalcThumbBounds(Rectangle bounds) {
			if(GetThumbImage() == null) return Rectangle.Empty;
			int thumbPos = Value2Pos(CurrentValue);
			Rectangle rect = new Rectangle();
			rect.Size = GetThumbImage().Size;
			rect.X = thumbPos - rect.Width / 2;
			rect.Y = bounds.Y + (bounds.Height - rect.Size.Height) / 2;
			return rect;
		}
		Rectangle CalcTrackBounds(Rectangle bounds) {
			if(GetTrackImage() == null) return Rectangle.Empty;
			Rectangle rect = new Rectangle();
			rect.Height = GetTrackImage().Size.Height;
			rect.Width = TrackWidth;
			rect.X = bounds.X + distanceFromButtonsToTrack + ZoomInButtonBounds.Width;
			rect.Y = bounds.Y + (bounds.Height - rect.Size.Height) / 2;
			return rect;
		}
		Rectangle CalcPointsRect() {
			return new Rectangle(TrackBounds.X + GetThumbImage().Width / 2, TrackBounds.Y, TrackBounds.Width - GetThumbImage().Width, TrackBounds.Height);
		}
		protected internal override void CalcBestSize() {
			int width = TrackWidth;
			width += 2 * distanceFromButtonsToTrack;
			int height = GetThumbImage().Height;
			if(Button.ShowZoomButtons) height = Math.Max(height, GetZoomOutButtonImage().Size.Height);
			height = Math.Max(height, GetMiddleLineImage().Size.Height);
			if(Button.ShowZoomButtons) width += GetZoomInButtonImage().Size.Width + GetZoomOutButtonImage().Size.Width;
			Size = IsHorizontal ? new System.Drawing.Size(width, height) : new Size(height, width);
		}
		int Value2Pos(int value) { 
			if(!Button.AllowUseMiddleValue)
				return CalcSimpleValueToPos(value);
			else return CalcMiddleValueToPos(value);
		}
		int CalcMiddleValueToPos(int value) {
			int distanceToTick;
			int res;
			if(value <= Button.Middle) {
				distanceToTick = (int)((float)PointsRect.Width / 2 * ((float)(value - Button.Minimum) / (float)(Button.Middle - Button.Minimum)) + 0.5f);
			}
			else {
				distanceToTick = (int)(float)PointsRect.Width / 2 + (int)((float)PointsRect.Width / 2 * ((float)(value - Button.Middle) / (float)(Button.Maximum - Button.Middle)) + 0.5f);
			}
			res = IsRightToLeft ? PointsRect.Right - distanceToTick : PointsRect.Left + distanceToTick;
			return res;
		}
		int CalcSimpleValueToPos(int value) {
			int res = 0;
			if(IsRightToLeft) {
				if(Button.Maximum == Button.Minimum)
					res = PointsRect.Right;
				else
					res = PointsRect.Left + (int)((float)PointsRect.Width * ((float)(Button.Maximum - value) / (float)(Button.Maximum - Button.Minimum)));
			}
			else {
				if(Button.Maximum == Button.Minimum)
					res = PointsRect.Left;
				else
					res = PointsRect.Left + (int)((float)PointsRect.Width * ((float)(value - Button.Minimum) / (float)(Button.Maximum - Button.Minimum)));
			}
			return res;
		}
		Point PointToClient(Point p) {
			if(IsHorizontal) return p;
			return new Point(p.Y - Bounds.Y + Bounds.X, Bounds.Y - p.X + Bounds.X);
		}
		int Pos2Value(int pos) {
			if (!Button.AllowUseMiddleValue) return CalcSimplePosToValue(pos, pointsRect);
			int distanceToTick = IsRightToLeft ? pointsRect.Right - (pos - pointsRect.Left) : pos;
			if(distanceToTick <= pointsRect.Left + (float)pointsRect.Width / 2) return Button.Minimum + (int)((((float)Button.Middle - (float)Button.Minimum) * ((float)distanceToTick - (float)pointsRect.Left) / ((float)pointsRect.Width / 2)) + 0.5f);
			return Button.Middle + (int)((((float)Button.Maximum - (float)Button.Middle) * ((float)distanceToTick - (float)pointsRect.Left - (float)pointsRect.Width / 2) / ((float)pointsRect.Width / 2)) + 0.5f);
		}
		private int CalcSimplePosToValue(int pos, Rectangle pointsRect) {
			if(IsRightToLeft)
				return Button.Minimum + (int)((float)(Button.Maximum - Button.Minimum) * (float)(pointsRect.Right - pos) / (float)pointsRect.Width + 0.5f);
			else
				return Button.Minimum + (int)((float)(Button.Maximum - Button.Minimum) * (float)(pos - pointsRect.Left) / (float)pointsRect.Width + 0.5f);
		}
		public RectangleF GetAnimatedRect(Rectangle bounds) {
			RectangleF start = new RectangleF((Bounds.Right + Bounds.X) / 2, (Bounds.Bottom + Bounds.Y) / 2, 0, 0);
			RectangleF end = bounds;
			RectangleF rect = CalcOutAnimationRect(start, end, ViewInfo.GetCurrentAnimationCoeff());
			return rect;
		}
		public override ContextButtonToolTipEventArgs GetCustomToolTipArgs(Point point) {
			if(!PointsRect.Contains(point)) return base.GetCustomToolTipArgs(point);
			int value = Pos2Value(PointToClient(point).X);
			ContextButtonToolTipEventArgs e = new ContextButtonToolTipEventArgs(Item, value);
			return e;
		}
		public override ContextButtonToolTipEventArgs OnItemToolTip(Point point) {
			ContextButtonToolTipEventArgs e = base.OnItemToolTip(point);
			e.Text = (e.Value.ToString() == "-1" && string.IsNullOrEmpty(e.Text)) ? e.Text : e.Value.ToString();
			return e;
		}
	}
	public class TrackBarContextButtonPainter : ContextItemPainter {
		public TrackBarContextButtonPainter(TrackBarContextButtonViewInfo viewInfo)
			: base(viewInfo) { }
		TrackBarContextButtonViewInfo TrackBarViewInfo { get { return ItemInfo as TrackBarContextButtonViewInfo; } }
		public TrackBarContextButton Button { get { return ItemInfo.Item as TrackBarContextButton; } }
		protected internal override void DrawItem(ContextItemCollectionInfoArgs info) {
			if(TrackBarViewInfo.IsNotFittedWidth || TrackBarViewInfo.IsNotFittedHeight || TrackBarViewInfo.Bounds.IsEmpty) return;
			base.DrawItem(info);
			TrackBarViewInfo.CalcViewInfo(TrackBarViewInfo.Bounds);
			TrackBarViewInfo.UpdateOpacity();
			if(ItemInfo.GetAnimationType() == ContextAnimationType.OutAnimation) {
				DrawTrackBarOutAnimation(info);
			}
			else {
				DrawGlyphCore(info, TrackBarViewInfo.GetZoomInButtonImage(), TrackBarViewInfo.ZoomInButtonBounds);
				DrawGlyphCore(info, TrackBarViewInfo.GetZoomOutButtonImage(), TrackBarViewInfo.ZoomOutButtonBounds);
				DrawGlyphCore(info, TrackBarViewInfo.GetTrackInfo(), TrackBarViewInfo.TrackBounds);
				DrawGlyphCore(info, TrackBarViewInfo.GetMiddleLineInfo(), TrackBarViewInfo.MiddleLineBounds);
				DrawGlyphCore(info, TrackBarViewInfo.GetThumbInfo(), TrackBarViewInfo.ThumbBounds);
			}
		}
		private void DrawTrackBarOutAnimation(ContextItemCollectionInfoArgs info) {
			DrawGlyphCore(info, TrackBarViewInfo.GetZoomInButtonImage(), TrackBarViewInfo.GetAnimatedRect(TrackBarViewInfo.ZoomInButtonBounds));
			DrawGlyphCore(info, TrackBarViewInfo.GetZoomOutButtonImage(), TrackBarViewInfo.GetAnimatedRect(TrackBarViewInfo.ZoomOutButtonBounds));
			DrawGlyphCore(info, TrackBarViewInfo.GetTrackInfo(), TrackBarViewInfo.GetAnimatedRect(TrackBarViewInfo.TrackBounds));
			DrawGlyphCore(info, TrackBarViewInfo.GetMiddleLineInfo(), TrackBarViewInfo.GetAnimatedRect(TrackBarViewInfo.MiddleLineBounds));
			DrawGlyphCore(info, TrackBarViewInfo.GetThumbInfo(), TrackBarViewInfo.GetAnimatedRect(TrackBarViewInfo.ThumbBounds));
		}
	}
	#endregion
	#region RatingContextButton
	public class RatingContextButton : ContextItem {
		RatingItemFillPrecision fillPrecision;
		int itemCount;
		public RatingContextButton() : base() {
			this.fillPrecision = RatingItemFillPrecision.Full;
			this.itemCount = 5;
		}
		protected override void Assign(ContextItem item) {
			BeginUpdate();
			try {
				base.Assign(item);
				RatingContextButton rating = item as RatingContextButton;
				if(rating == null)
					return;
				CheckedGlyph = rating.CheckedGlyph;
				Rating = rating.Rating;
				FillPrecision = rating.FillPrecision;
				ItemCount = rating.ItemCount;
			}
			finally {
				CancelUpdate();
			}
		}
		Image checkedGlyph;
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image CheckedGlyph {
			get { return checkedGlyph; }
			set {
				if(CheckedGlyph == value)
					return;
				Image prev = CheckedGlyph;
				checkedGlyph = value;
				OnItemChanged("CheckedGlyph", prev, CheckedGlyph);
			}
		}
		[DefaultValue(0)]
		public decimal Rating {
			get { return AdjustRating(editValue); }
			set {
				value = AdjustRating(value);
				if(Rating == value)
					return;
				decimal prev = Rating;
				editValue = value;
				OnItemChanged("Rating", prev, Rating);
			}
		}		
		decimal editValue = 0;
		protected internal decimal AdjustRating(Decimal value) {
			if(value < 0)
				return 0;
			if(value > Convert.ToDecimal(ItemCount))
				return Convert.ToDecimal(ItemCount);
			if(FillPrecision == RatingItemFillPrecision.Full)
				return Decimal.Floor(value);
			if(FillPrecision == RatingItemFillPrecision.Half) {
				decimal fraction = value - Decimal.Floor(value);
				if(fraction < 0.5m)
					return Decimal.Floor(value);
				else
					return Decimal.Floor(value) + 0.5m;
			}
			return value;
		}
		[DefaultValue(RatingItemFillPrecision.Full)]
		public RatingItemFillPrecision FillPrecision {
			get { return fillPrecision; }
			set {
				if(fillPrecision == value) return;
				RatingItemFillPrecision prev = fillPrecision;
				fillPrecision = value;
				OnItemChanged("FillPrecision", prev, fillPrecision);
			}
		}
		[DefaultValue(5)]
		public int ItemCount {
			get { return itemCount; }
			set {
				if(itemCount == value) return;
				int prev = itemCount;
				itemCount = value;
				OnItemChanged("ItemCount", prev, itemCount);
			}
		}
		protected internal override ContextItemViewInfo CreateViewInfo(ContextItemCollectionViewInfo collectionViewInfo) {
			return new RatingContextButtonViewInfo(this, collectionViewInfo);
		}	   
	}
	public class RatingContextButtonHitInfo : ContextItemHitInfo {
		decimal ratingHitValue;
		int ratingHitIndex; 
		public RatingContextButtonHitInfo(Point hitPoint) : base(hitPoint) { }
		public RatingContextButtonHitInfo(Point hitPoint, ContextItemViewInfo hitViewinfo) : base(hitPoint, hitViewinfo) { }
		public int RatingHitIndex { get { return ratingHitIndex; } }
		public decimal RatingHitValue { get { return ratingHitValue; } }
		protected internal virtual void SetRatingHitIndex(int newValue) { this.ratingHitIndex = newValue; }
		protected internal virtual void SetRatingHitValue(decimal value) { this.ratingHitValue = value; }
	}
	public class RatingContextButtonViewInfo : ContextItemViewInfo {
		RatingContextButtonPainter painter;
		RatingControlHelper helper;
		List<Rectangle> ratingRects;
		const int ratingIndent = 5;
		public RatingContextButtonViewInfo(RatingContextButton owner, ContextItemCollectionViewInfo collectionViewInfo)
			: base(owner, collectionViewInfo) {
			Rating = owner.Rating;
			ClearHitInfo();
		}
		protected internal RatingControlHelper Helper {
			get {
				if(helper == null)
					helper = CreateRatingControlHelper();
				return helper;
			}
		}
		protected virtual RatingControlHelper CreateRatingControlHelper() {
			return new RatingControlHelper(Button.Orientation, IsRightToLeft, Glyph.Size, false);
		}
		protected void ClearHitInfo() {
			if(RatingHitInfo == null) return;
			RatingHitInfo.Clear();
			RatingHitInfo.SetRatingHitIndex(-1);
			RatingHitInfo.SetRatingHitValue(0);
		}
		public override ContextAnimationType GetAnimationType() {
			ContextAnimationType res = Item.AnimationType;
			if(res == ContextAnimationType.Default)
				res = Options.GetAnimationType();
			return res;
		}
		protected internal override void OnHoverOut() {
			ClearHitInfo();
			base.OnHoverOut();
		}
		Image defaultGlyph = null;
		protected override Image DefaultGlyph {
			get {
				if(defaultGlyph == null) {
					SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemRatingIndicator];
					defaultGlyph = elem.Image.GetImages().Images[0];
				}
				return defaultGlyph;
			}
		}
		Image defaultCheckedGlyph = null;
		protected Image DefaultCheckedGlyph {
			get { 
				if(defaultCheckedGlyph == null) {
					SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemRatingIndicator];
					defaultCheckedGlyph = elem.Image.GetImages().Images[2];
				}
				return defaultCheckedGlyph;
			}	
		}
		protected internal Image CheckedGlyph {
			get {
				if(Button.CheckedGlyph != null)
					return Button.CheckedGlyph;
				return DefaultCheckedGlyph;
			}
		}
		Image defaultHotGlyph = null;
		protected override Image DefaultHoverGlyph {
			get {
				if(defaultHotGlyph == null) {
					SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinContextItemRatingIndicator];
					defaultHotGlyph = elem.Image.GetImages().Images[1];
				}
				return defaultHotGlyph;
			}
		}
		protected internal override void CalcBestSize() {
			base.CalcBestSize();
			Size = IsHorizontal ? new Size(Button.ItemCount * Size.Width + 2 * ratingIndent, Size.Height) : new Size(Size.Width, Button.ItemCount * Size.Height + 2 * ratingIndent);
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			CalculateRatingRects();
			CalcZeroRatingRect(bounds);
			CalcMaxRatingRect(bounds);
		}
		void CalcMaxRatingRect(Rectangle bounds) {
			Rectangle rect = Rectangle.Empty;
			Point loc = Point.Empty;
			if(Button.Orientation == Orientation.Horizontal) {
				loc = (!IsRightToLeft) ? new Point(RatingRects[Button.ItemCount - 1].Right, RatingRects[Button.ItemCount - 1].Y) : new Point(RatingRects[0].Left - ratingIndent, RatingRects[0].Y);
				rect = new Rectangle(loc, new Size(ratingIndent, RatingRects[0].Height));
			}
			else {
				loc = new Point(RatingRects[0].X, RatingRects[0].Top - ratingIndent);
				rect = new Rectangle(loc, new Size(RatingRects[0].Width, ratingIndent));
			}
			MaxRatingRect = rect;
		}
		void CalcZeroRatingRect(Rectangle bounds) {
			Rectangle rect = Rectangle.Empty;
			Point loc = Point.Empty;
			if(Button.Orientation == Orientation.Horizontal) {
				loc = (!IsRightToLeft) ? new Point(RatingRects[0].X - ratingIndent, RatingRects[0].Y) : new Point(RatingRects[RatingRects.Count - 1].Right, RatingRects[0].Y);
				rect = new Rectangle(loc, new Size(ratingIndent, RatingRects[0].Height));
			}
			else {
				loc = new Point(RatingRects[0].X, RatingRects[RatingRects.Count - 1].Bottom);
				rect = new Rectangle(loc, new Size(RatingRects[0].Width, ratingIndent));
			}
			ZeroRatingRect = rect;
		}
		public decimal Rating { get; internal set; }
		public List<Rectangle> RatingRects { 
			get { 
				if(ratingRects == null)
					ratingRects = new List<Rectangle>();
				return ratingRects;
			}
			private set {
				ratingRects = value;
			}
		}
		protected internal Rectangle ZeroRatingRect { get; set; }
		protected internal Rectangle MaxRatingRect { get; set; }
		public RatingContextButton Button { get { return (RatingContextButton)Item; } }
		protected virtual void CalculateRatingRects(){
			List<Rectangle> ratingRects = new List<Rectangle>();
			Size glyphSize = Glyph.Size;
			Point loc = new Point();
			for(int i = 0; i < Button.ItemCount; i++) {
				Rectangle ratingRect;
				if(i == 0)
					loc = IsHorizontal ? new Point(Bounds.Location.X + ratingIndent, Bounds.Location.Y) : new Point(Bounds.Location.X, Bounds.Location.Y + ratingIndent);
				else
					loc = IsHorizontal ? new Point(ratingRects[i - 1].Right, Bounds.Y) : new Point(Bounds.X, ratingRects[i - 1].Bottom); 
					ratingRect = new Rectangle(loc, glyphSize);
				ratingRects.Add(ratingRect);
			}
			RatingRects = ratingRects;
		}
		protected internal override void MakeOffset(int deltaX, int deltaY) {
			base.MakeOffset(deltaX, deltaY);
			if(RatingRects == null)
				return;
			for(int i = 0; i < RatingRects.Count; i++) {
				RatingRects[i] = OffsetRect(RatingRects[i], deltaX, deltaY);
			}
		}
		protected internal override ContextItemPainter Painter {
			get {
				if(painter == null)
					painter = new RatingContextButtonPainter(this);
				return painter;			   
			}
		}
		public RatingContextButtonHitInfo RatingHitInfo { get { return (RatingContextButtonHitInfo)HitInfo; } }
		protected internal override ContextItemHitInfo CalcHitInfo(Point point) {
			RatingContextButtonHitInfo hitInfo;
			if(!Bounds.Contains(point)) {
				hitInfo = null;
				return hitInfo;
			}
			hitInfo = new RatingContextButtonHitInfo(point, this);
			int index = GetRatingRectIndex(point);
			ContextItemHitTest hitTest = (index == -1) ? ContextItemHitTest.None : ContextItemHitTest.Rating;
			hitInfo.SetHitTest(hitTest);
			hitInfo.SetRatingHitIndex(GetRatingRectIndex(point));
			hitInfo.SetRatingHitValue(CalcRatingValue(point));
			ViewInfo.Owner.Redraw(Bounds);
			return hitInfo;
		}
		public override void OnItemClick(MouseEventArgs e) {
			decimal ratingBefore = Rating;
			Rating = CalcRatingValue(e.Location);
			if(ratingBefore == Rating) Rating = 0;
			Button.Rating = Rating;
			base.OnItemClick(e);
		}
		protected virtual decimal CalcRatingValue(Point point) {
			int index = GetRatingRectIndex(point);
			if(index == -1)
				if(ZeroRatingRect.Contains(point))
					return 0;
				else if(MaxRatingRect.Contains(point))
					return Button.ItemCount;
			if(Button.FillPrecision == RatingItemFillPrecision.Full) {
				if(Button.Orientation == Orientation.Horizontal && !IsRightToLeft)
					return index + 1;
				else return Button.ItemCount - index;
			}
			decimal value;
			if(Button.Orientation == Orientation.Horizontal)
				value = IsRightToLeft ? (Decimal)(RatingRects.Count - 1 - index) : (Decimal)index;
			else
				value = (Decimal)(RatingRects.Count - 1 - index);
			value = value + 1.0m;
			if(Button.FillPrecision == RatingItemFillPrecision.Half)
				return Helper.GetRatingRectHalfValue(RatingRects[index], point, value);
			else
				return Helper.GetRatingRectExactValue(RatingRects[index], point, value);
		}
		protected internal virtual int GetRatingRectIndex(Point point) {
			if(!Bounds.Contains(point))
				return -1;
			for(int i = 0; i < RatingRects.Count; i++)
				if(RatingRects[i].Contains(point)) return i;
			return -1;
		}
		public RectangleF GetAnimatedRect(Rectangle ratingSection, Rectangle ratingRect) {
			RectangleF start = new RectangleF((ratingRect.Right + ratingRect.X) / 2, (ratingRect.Bottom + ratingRect.Y) / 2, 0, 0);
			RectangleF end = ratingSection;
			RectangleF rect = CalcOutAnimationRect(start, end, ViewInfo.GetCurrentAnimationCoeff());
			return rect;
		}
		public ImageAttributes GetRatingAnimatedImageAttributes(float sectionOpacity) {
			ColorMatrix m = new ColorMatrix();
			m.Matrix33 = sectionOpacity;
			ImageAttributes attr = new ImageAttributes();
			attr.SetColorMatrix(m);
			return attr;
		}
		public override ContextButtonToolTipEventArgs GetCustomToolTipArgs(Point point) {
			decimal rating = CalcRatingValue(point);
			ContextButtonToolTipEventArgs e = new ContextButtonToolTipEventArgs(Item, rating);
			return e;
		}
		public override ContextButtonToolTipEventArgs OnItemToolTip(Point point) {
			ContextButtonToolTipEventArgs e = base.OnItemToolTip(point);
			e.Text = (!String.IsNullOrEmpty(e.Text)) ? e.Text : e.Value.ToString();
			return e;
		}
		protected internal virtual Image GetCurrentImage(int index, decimal ratingFloor, decimal hotFraction) { 
			Image curGlyph = new Bitmap(Glyph.Size.Width, Glyph.Size.Height);
			if(index < ratingFloor)
				curGlyph = CheckedGlyph;
			if(index == ratingFloor) {
				if(hotFraction > 0)
					curGlyph = Glyph;
				else
					curGlyph = CheckedGlyph;
			}
			if(index > ratingFloor)
				curGlyph = Glyph;
			return curGlyph;
		}
	}
	public class RatingContextButtonPainter : ContextItemPainter {
		public RatingContextButtonPainter(RatingContextButtonViewInfo viewInfo) : base(viewInfo) { }
		RatingContextButtonViewInfo RatingViewInfo { get { return ItemInfo as RatingContextButtonViewInfo; } }
		public RatingContextButton Button { get { return (ItemInfo.Item) as RatingContextButton; } }
		public List<float> RatingOpacities { get; private set; }
		protected internal override void DrawItem(ContextItemCollectionInfoArgs info) {
			if(RatingViewInfo.IsNotFittedWidth || RatingViewInfo.IsNotFittedHeight || RatingViewInfo.Bounds.IsEmpty) return;
			ItemInfo.UpdateOpacity();
			if(ItemInfo.GetAnimationType() == ContextAnimationType.SequenceAnimation)
				RatingOpacities = RatingViewInfo.ViewInfo.GetCurrentOpacities();
			DrawRatingButton(info, RatingViewInfo.IsAnimated);
		}
		protected virtual void DrawRatingButton(ContextItemCollectionInfoArgs info, bool animated) {
			int index;
			decimal hotValue = -2;
			if(RatingViewInfo.HitInfo != null && RatingViewInfo.HitInfo is RatingContextButtonHitInfo)
				hotValue = Convert.ToDecimal(((RatingContextButtonHitInfo)RatingViewInfo.HitInfo).RatingHitValue) - 1;
			for(int i = 0; i < RatingViewInfo.RatingRects.Count; i++) {
				index = i;
				if(Button.Orientation == Orientation.Vertical)
					index = RatingViewInfo.RatingRects.Count - 1 - i;
				else
					index = RatingViewInfo.IsRightToLeft ? RatingViewInfo.RatingRects.Count - 1 - i : i;
				DrawRatingItem(info, RatingViewInfo.RatingRects[i], index, hotValue);
			}
		}
		void DrawRatingItem(ContextItemCollectionInfoArgs info, Rectangle itemRect, int index, decimal hotValue) {
			if(DrawHotRatingItem(info, itemRect, index, RatingViewInfo.Rating, hotValue))
				return;
			DrawNormalStateItem(info, index, RatingViewInfo.Rating, itemRect);
		}
		protected virtual void DrawNormalStateItem(ContextItemCollectionInfoArgs info, int index, decimal rating, Rectangle itemRect) {
			Rectangle glyphRect = new Rectangle(Point.Empty, new Size(itemRect.Width, RatingViewInfo.Glyph.Size.Height));
			int ratingFloor = Convert.ToInt32(Decimal.Floor(rating));
			decimal ratingFraction = rating - ratingFloor;
			if(rating == 0) {
				DrawRatingGlyphCore(info, RatingViewInfo.Glyph, itemRect, glyphRect, index);
			}
			else {
				if(index <= ratingFloor - 1) {
					DrawRatingGlyphCore(info, RatingViewInfo.CheckedGlyph, itemRect, glyphRect, index);
					return;
				}
				if(ratingFraction > 0 && index == ratingFloor) {
					Rectangle glyphRatingRect = Rectangle.Empty;
					Rectangle glyphNormalRect = Rectangle.Empty;
					if(glyphRect.Width < RatingViewInfo.Glyph.Size.Width) {
						List<Rectangle> rects = RatingViewInfo.Helper.CalcPartialRatingAndNormalRects(itemRect, glyphRect, ratingFraction);
						glyphRatingRect = rects[0];
						glyphNormalRect = rects[1];
					}
					else {
						glyphRatingRect = RatingViewInfo.Helper.CalcFractionRect(glyphRect, ratingFraction);
						glyphNormalRect = RatingViewInfo.Helper.CalcNotFractionRect(glyphRect, glyphRatingRect);
					}
					if(glyphRatingRect != Rectangle.Empty) {
						Rectangle itemRatingRect = RatingViewInfo.Helper.OffsetRect(glyphRatingRect, itemRect.Location);
						DrawRatingGlyphCore(info, RatingViewInfo.CheckedGlyph, itemRatingRect, glyphRatingRect, index);
					}
					if(glyphNormalRect != Rectangle.Empty) {
						Rectangle itemNormalRect = RatingViewInfo.Helper.OffsetRect(glyphNormalRect, itemRect.Location);
						DrawRatingGlyphCore(info, RatingViewInfo.Glyph, itemNormalRect, glyphNormalRect, index);
					}
					return;
				}
				DrawRatingGlyphCore(info, RatingViewInfo.Glyph, itemRect, glyphRect, index);
			}
		}
		protected virtual bool DrawHotRatingItem(ContextItemCollectionInfoArgs info, Rectangle itemRect, int index, decimal rating, decimal hotValue) {
			Rectangle glyphRect = new Rectangle(Point.Empty, new Size(itemRect.Width, RatingViewInfo.Glyph.Height));
			int ratingFloor = Convert.ToInt32(Decimal.Floor(rating));
			decimal ratingFraction = rating - ratingFloor;
			int hotFloor = Convert.ToInt32(Decimal.Floor(hotValue));
			decimal hotFraction = hotValue - hotFloor;
			if(index <= hotFloor) {
				DrawRatingGlyphCore(info, RatingViewInfo.HoverGlyph, itemRect, glyphRect, index);
				return true;
			}
			if(hotFraction > 0 && index == hotFloor + 1) {
				Rectangle glyphHotRect = RatingViewInfo.Helper.GetGlyphHotRect(itemRect, glyphRect, hotFraction);
				Rectangle itemHotRect = RatingViewInfo.Helper.OffsetRect(glyphHotRect, itemRect.Location);
				DrawRatingGlyphCore(info, RatingViewInfo.HoverGlyph, itemHotRect, glyphHotRect, index);
				if(Button.FillPrecision == RatingItemFillPrecision.Half || (Button.FillPrecision == RatingItemFillPrecision.Exact && index != ratingFloor)) {
					Rectangle glyphNormalRect = RatingViewInfo.Helper.CalcNotFractionRect(glyphRect, glyphHotRect);
					Rectangle itemNormalRect = RatingViewInfo.Helper.OffsetRect(glyphNormalRect, itemRect.Location);
					Image curGlyph = RatingViewInfo.GetCurrentImage(index, ratingFloor, hotFraction);
					DrawRatingGlyphCore(info, curGlyph, itemNormalRect, glyphNormalRect, index);
					return true;
				}
				if(Button.FillPrecision == RatingItemFillPrecision.Exact && index == ratingFloor) {
					DrawHotRatingItemWithExactValue(info, itemRect, glyphRect, ratingFraction, hotFraction, glyphHotRect, index);
					return true;
				}
			}
			return false;
		}
		protected virtual void DrawHotRatingItemWithExactValue(ContextItemCollectionInfoArgs info, Rectangle itemRect, Rectangle glyphRect, decimal ratingFraction, decimal hotFraction, Rectangle glyphHotRect, int index) {
			Rectangle glyphEmptySection;
			Rectangle itemEmptySection;
			if(ratingFraction > hotFraction) {
				Rectangle fullGlyphRect = new Rectangle(Point.Empty, RatingViewInfo.Glyph.Size);
				Rectangle fullGlyphRatingRect = RatingViewInfo.Helper.CalcFractionRect(fullGlyphRect, ratingFraction);
				if(fullGlyphRatingRect.Right > glyphRect.Right)
					glyphRect.Width -= fullGlyphRatingRect.Width - glyphRect.Width;
				List<Rectangle> sections = RatingViewInfo.Helper.CalcGlyphRatingAndEmptySection(glyphRect, glyphHotRect, fullGlyphRatingRect);
				Rectangle imageRatingSection = sections[0];
				glyphEmptySection = sections[1];
				Rectangle itemRatingSection = RatingViewInfo.Helper.OffsetRect(imageRatingSection, itemRect.Location);
				DrawRatingGlyphCore(info, RatingViewInfo.CheckedGlyph, itemRatingSection, imageRatingSection, index);
			}
			else
				glyphEmptySection = RatingViewInfo.Helper.CalcGlyphEmptySection(glyphRect, glyphHotRect);
			itemEmptySection = RatingViewInfo.Helper.OffsetRect(glyphEmptySection, itemRect.Location);
			DrawRatingGlyphCore(info, RatingViewInfo.Glyph, itemEmptySection, glyphEmptySection, index);
		}
		protected virtual void DrawRatingGlyphCore(ContextItemCollectionInfoArgs info, Image glyph, Rectangle itemRect, Rectangle glyphRect, int index) {
			if(!ItemInfo.IsAnimated) {
				if(ItemInfo.AllowGlyphColorize)
					info.Cache.Paint.DrawImage(info.Graphics, glyph, itemRect, glyphRect, ItemInfo.GetImageAttributes());
				else if(ItemInfo.Opacity == 1.0f)
					info.Cache.Paint.DrawImage(info.Graphics, glyph, itemRect, glyphRect, null);
				else
					info.Cache.Paint.DrawImage(info.Graphics, glyph, itemRect, glyphRect, ItemInfo.GetImageAttributes());
			}
			else
				DrawAnimatedRatingGlyph(info, glyph, itemRect, glyphRect, index);
		}
		protected virtual void DrawAnimatedRatingGlyph(ContextItemCollectionInfoArgs info, Image glyph, Rectangle itemRect, Rectangle glyphRect, int index) {
			if(ItemInfo.Opacity == 0.0f)
				return;
			if(ItemInfo.GetAnimationType() == ContextAnimationType.OpacityAnimation)
				info.Cache.Paint.DrawImage(info.Graphics, glyph, itemRect, glyphRect, ItemInfo.GetImageAttributes());
			if(ItemInfo.GetAnimationType() == ContextAnimationType.OutAnimation) {
				RectangleF bounds = RatingViewInfo.GetAnimatedRect(itemRect, RatingViewInfo.RatingRects[RatingViewInfo.IsRightToLeft && Button.Orientation == Orientation.Horizontal || Button.Orientation == Orientation.Vertical? RatingViewInfo.RatingRects.Count - 1 - index : index]);
				PointF[] pt = new PointF[] { 
					new PointF(bounds.X, bounds.Y),
					new PointF(bounds.Right, bounds.Y),
					new PointF(bounds.X, bounds.Bottom)
				};
				info.Graphics.DrawImage(glyph, pt, glyphRect, GraphicsUnit.Pixel, ItemInfo.GetImageAttributes());
			}
			if(ItemInfo.GetAnimationType() == ContextAnimationType.SequenceAnimation) {
				if(RatingOpacities == null) return;
				info.Cache.Paint.DrawImage(info.Graphics, glyph, itemRect, glyphRect, RatingViewInfo.GetRatingAnimatedImageAttributes(RatingOpacities[index]));
				return;
			}
		}
	}
	public class RatingControlHelper : IDisposable {
		Orientation orientation;
		bool isRightToLeft;
		bool isDirectionReversed;
		Size glyphSize;
		public RatingControlHelper(Orientation orientation, bool isRightToLeft, Size glyphSize, bool isDirectionReversed) {
			this.orientation = orientation;
			this.glyphSize = glyphSize;
			this.isRightToLeft = isRightToLeft;
			this.isDirectionReversed = isDirectionReversed;
		}
		public RatingControlHelper() : this(Orientation.Horizontal, false, Size.Empty, false){
		}
		void IDisposable.Dispose() {
		}
		public Orientation RatingOrientation {
			get { return this.orientation; }
			set { this.orientation = value; }
		}
		public Size GlyphSize { get { return this.glyphSize; } set { this.glyphSize = value; } }
		public bool IsRightToLeft { get { return isRightToLeft; } set { this.isRightToLeft = value; } }
		public bool IsDirectionReversed { get { return isDirectionReversed; } set { this.isDirectionReversed = value; } }
		public virtual Decimal GetRatingRectHalfValue(Rectangle ratingRect, Point point, Decimal value) {
			ratingRect = new Rectangle(ratingRect.Location, new Size(GlyphSize.Width, ratingRect.Height));
			Rectangle firstHalf;
			if(RatingOrientation == Orientation.Horizontal) {
				int firstHalfX = (IsRightToLeft == IsDirectionReversed) ? ratingRect.X : ratingRect.Right - ratingRect.Width / 2;
				firstHalf = new Rectangle(new Point(firstHalfX, ratingRect.Y), new Size(ratingRect.Width / 2, ratingRect.Height));
			}
			else {
				int firstHalfY = (!IsDirectionReversed) ? ratingRect.Bottom - ratingRect.Height / 2 : ratingRect.Y;
				firstHalf = new Rectangle(new Point(ratingRect.X, firstHalfY), new Size(ratingRect.Width, ratingRect.Height / 2));
			}
			if(firstHalf.Contains(point))
				value -= 0.5m;
			return value;
		}
		public virtual Decimal GetRatingRectExactValue(Rectangle ratingRect, Point point, Decimal value) {
			if(!ratingRect.Contains(point)) return value;
			ratingRect = new Rectangle(ratingRect.Location, new Size(GlyphSize.Width, ratingRect.Height));
			if(RatingOrientation == Orientation.Horizontal) {
				decimal notRatingWidthInRect = (IsRightToLeft == IsDirectionReversed) ? (Decimal)(ratingRect.Right - point.X) : (Decimal)(point.X - ratingRect.Left);
				value -= notRatingWidthInRect / (Decimal)ratingRect.Width;
			}
			else {
				decimal notRatingHeightInRect = (!IsDirectionReversed) ? (Decimal)(point.Y - ratingRect.Top) : (Decimal)(ratingRect.Bottom - point.Y);
				value -= notRatingHeightInRect / (Decimal)ratingRect.Height;
			}
			return value;
		}
		public virtual Rectangle CalcFractionRect(Rectangle rect, decimal fraction) {
			Rectangle fractionRect;
			if(RatingOrientation == Orientation.Horizontal) {
				int fractionRectWidth = Convert.ToInt32(fraction * rect.Width);
				int fractionRectX = (IsRightToLeft == IsDirectionReversed) ? rect.X : rect.Right - fractionRectWidth;
				fractionRect = new Rectangle(new Point(fractionRectX, rect.Y), new Size(fractionRectWidth, rect.Height));
			}
			else {
				int fractionRectHeight = Convert.ToInt32(fraction * rect.Height);
				int fractionRectY = (!IsDirectionReversed) ? rect.Bottom - Convert.ToInt32(fraction * rect.Height) : rect.Y;
				fractionRect = new Rectangle(new Point(rect.X, fractionRectY), new Size(rect.Width, fractionRectHeight));
			}
			return fractionRect;
		}
		public virtual Rectangle CalcNotFractionRect(Rectangle rect, Rectangle fractionRect) {
			Rectangle resultRect;
			if(RatingOrientation == Orientation.Horizontal) {
				int resultRectX = (IsRightToLeft == IsDirectionReversed) ? rect.X + fractionRect.Width : rect.X;
				resultRect = new Rectangle(new Point(resultRectX, rect.Y), new Size(rect.Width - fractionRect.Width, rect.Height));
			}
			else {
				int resultRectY = (!IsDirectionReversed) ? rect.Y : rect.Y + fractionRect.Height;
				resultRect = new Rectangle(new Point(rect.X, resultRectY), new Size(rect.Width, rect.Height - fractionRect.Height));
			}
			return resultRect;
		}
		public virtual Rectangle GetGlyphHotRect(Rectangle itemRect, Rectangle glyphRect, decimal hotFraction) {
			Rectangle fullGlyphRect = new Rectangle(Point.Empty, GlyphSize);
			Rectangle fullGlyphHotRect = CalcFractionRect(fullGlyphRect, hotFraction);
			Rectangle itemHotRect = OffsetRect(fullGlyphHotRect, itemRect.Location);
			int hiddenWidth = (itemHotRect.Right > itemRect.Right) ? GlyphSize.Width - glyphRect.Width : 0;
			Rectangle glyphHotRect = new Rectangle(fullGlyphHotRect.Location, new Size(fullGlyphHotRect.Width - hiddenWidth, fullGlyphHotRect.Height));
			return glyphHotRect;
		}
		public virtual Rectangle OffsetRect(Rectangle rect, Point offset) {
			Rectangle result = rect;
			result.Offset(offset);
			return result;
		}
		public virtual List<Rectangle> CalcGlyphRatingAndEmptySection(Rectangle glyphRect, Rectangle glyphHotRect, Rectangle glyphRatingRect) {
			Size ratingSectionSize;
			Size emptySectionSize;
			Rectangle glyphRatingSection = Rectangle.Empty;
			Rectangle glyphEmptySection = Rectangle.Empty;
			List<Rectangle> sections = new List<Rectangle>();
			if(RatingOrientation == Orientation.Horizontal) {
				if(IsRightToLeft == IsDirectionReversed) {
					ratingSectionSize = new Size(glyphRatingRect.Right - glyphHotRect.Right, glyphRect.Height);
					glyphRatingSection = new Rectangle(new Point(glyphHotRect.Right, glyphRect.Y), ratingSectionSize);
					emptySectionSize = new Size(glyphRect.Right - glyphRatingSection.Right, glyphRect.Height);
					glyphEmptySection = new Rectangle(new Point(glyphRatingSection.Right, glyphRect.Y), emptySectionSize);
				}
				else {
					ratingSectionSize = new Size(glyphHotRect.Left - glyphRatingRect.Left, glyphRect.Height);
					glyphRatingSection = new Rectangle(glyphRatingRect.Location, ratingSectionSize);
					emptySectionSize = new Size(glyphRatingSection.Left - glyphRect.Left, glyphRect.Height);
					glyphEmptySection = new Rectangle(glyphRect.Location, emptySectionSize);
				}
			}
			if(RatingOrientation == Orientation.Vertical) {
				if(!IsDirectionReversed) {
					ratingSectionSize = new Size(glyphRect.Width, glyphHotRect.Top - glyphRatingRect.Top);
					glyphRatingSection = new Rectangle(glyphRatingRect.Location, ratingSectionSize);
					emptySectionSize = new Size(glyphRect.Width, glyphRatingSection.Top - glyphRect.Top);
					glyphEmptySection = new Rectangle(glyphRect.Location, emptySectionSize);
				}
				else {
					ratingSectionSize = new Size(glyphRect.Width, glyphRatingRect.Bottom - glyphHotRect.Bottom);
					glyphRatingSection = new Rectangle(new Point(glyphRect.X, glyphHotRect.Bottom), ratingSectionSize);
					emptySectionSize = new Size(glyphRect.Width, glyphRect.Bottom - glyphRatingSection.Bottom);
					glyphEmptySection = new Rectangle(new Point(glyphRect.X, glyphRatingSection.Bottom), emptySectionSize);
				}
			}
			sections.Add(glyphRatingSection);
			sections.Add(glyphEmptySection);
			return sections;
		}
		public virtual Rectangle CalcGlyphEmptySection(Rectangle glyphRect, Rectangle glyphHotRect) {
			Size emptySectionSize;
			Rectangle glyphEmptySection;
			if(RatingOrientation == Orientation.Horizontal) {
				if(IsRightToLeft == IsDirectionReversed) {
					emptySectionSize = new Size(glyphRect.Right - glyphHotRect.Right, glyphRect.Height);
					glyphEmptySection = new Rectangle(new Point(glyphHotRect.Right, glyphRect.Y), emptySectionSize);
				}
				else {
					emptySectionSize = new Size(glyphHotRect.Left - glyphRect.Left, glyphRect.Height);
					glyphEmptySection = new Rectangle(glyphRect.Location, emptySectionSize);
				}
			}
			else {
				if(!IsDirectionReversed) {
					emptySectionSize = new Size(glyphRect.Width, glyphHotRect.Top - glyphRect.Top);
					glyphEmptySection = new Rectangle(glyphRect.Location, emptySectionSize);
				}
				else {
					emptySectionSize = new Size(glyphRect.Width, glyphRect.Bottom - glyphHotRect.Bottom);
					glyphEmptySection = new Rectangle(new Point(glyphRect.X, glyphHotRect.Bottom), emptySectionSize);
				}
			}
			return glyphEmptySection;
		}
		public virtual List<Rectangle> CalcPartialRatingAndNormalRects(Rectangle itemRect, Rectangle glyphRect, decimal ratingFraction) {
			Rectangle fullGlyphRect = new Rectangle(Point.Empty, GlyphSize);
			Rectangle fullGlyphRatingRect = CalcFractionRect(fullGlyphRect, ratingFraction);
			Rectangle glyphRatingRect = Rectangle.Empty;
			Rectangle glyphNormalRect = Rectangle.Empty;
			List<Rectangle> rects = new List<Rectangle>();
			if(IsRightToLeft == IsDirectionReversed) {
				glyphRatingRect = (fullGlyphRatingRect.Width < glyphRect.Width) ? fullGlyphRatingRect : glyphRect;
				if(fullGlyphRatingRect.Width < glyphRect.Width)
					glyphNormalRect = CalcNotFractionRect(glyphRect, fullGlyphRatingRect);
			}
			else {
				int hiddenGlyphWidth = GlyphSize.Width - glyphRect.Width;
				int visibleGlyphRatingWidth = fullGlyphRatingRect.Width - hiddenGlyphWidth;
				Rectangle visibleGlyphRatingRect = new Rectangle(fullGlyphRatingRect.Location, new Size(visibleGlyphRatingWidth, fullGlyphRatingRect.Height));
				if(visibleGlyphRatingWidth > 0)
					glyphRatingRect = visibleGlyphRatingRect;
				glyphNormalRect = (visibleGlyphRatingWidth > 0) ? CalcNotFractionRect(glyphRect, glyphRatingRect) : glyphRect;
			}
			rects.Add(glyphRatingRect);
			rects.Add(glyphNormalRect);
			return rects;
		}
	}
	#endregion
	public delegate void ItemCollectionChangedEventHandler(object sender, ItemCollectionChangedEventArgs e);
	public class ItemCollectionChangedEventArgs : EventArgs {
		public ItemCollectionChangedEventArgs(int index, bool isRemoved) {
			Index = index;
			IsRemoved = isRemoved;
		}
		public int Index { get; private set; }
		public bool IsRemoved { get; private set; }
	}
	public interface IContextItemCollectionOptionsOwner {
		void OnOptionsChanged(string propertyName, object oldValue, object newValue);
		ContextAnimationType AnimationType { get; }
	}
	public class ContextItemCollectionOptions  {	   
		public ContextItemCollectionOptions() : this(null) { }
		public ContextItemCollectionOptions(IContextItemCollectionOptionsOwner owner) {
			Owner = owner;
		}
		protected IContextItemCollectionOptionsOwner Owner { get; private set; }
		float normalOpacity = 0.75f;
		[DefaultValue(0.75f)]
		public float NormalStateOpacity {
			get { return normalOpacity; }
			set {
				if(NormalStateOpacity == value)
					return;
				float prev = NormalStateOpacity;
				normalOpacity = CheckOpacity(value);
				OnOptionsChanged("NormalStateOpacity", prev, NormalStateOpacity);
			}
		}
		private float CheckOpacity(float value) {
			if(value > 1.0f) return 1.0f;
			if(value < 0) return 0;
			return value;
		}
		float hoverOpacity = 1.0f;
		[DefaultValue(1.0f)]
		public float HoverStateOpacity {
			get { return hoverOpacity; }
			set {
				if(HoverStateOpacity == value)
					return;
				float prev = HoverStateOpacity;
				hoverOpacity = CheckOpacity(value);
				OnOptionsChanged("HoverStateOpacity", prev, HoverStateOpacity);
			}
		}
		float disabledOpacity = 0.6f;
		[DefaultValue(0.6f)]
		public float DisabledStateOpacity {
			get { return disabledOpacity; }
			set {
				if(DisabledStateOpacity == value)
					return;
				float prev = DisabledStateOpacity;
				disabledOpacity = CheckOpacity(value);
				OnOptionsChanged("DisabledStateOpacity", prev, DisabledStateOpacity);
			}
		}
		bool allowGlyphSkinning = false;
		[DefaultValue(false)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				bool prev = AllowGlyphSkinning;
				allowGlyphSkinning = value;
				OnOptionsChanged("AllowGlyphColorize", prev, AllowGlyphSkinning);
			}
		}
		Padding topPanelPadding = new Padding(5);
		void ResetTopPanelPadding() { TopPanelPadding = new Padding(5); }
		bool ShouldSerializeTopPanelPadding() { return TopPanelPadding != new Padding(5); }
		public virtual Padding TopPanelPadding {
			get { return topPanelPadding; }
			set {
				if(TopPanelPadding == value)
					return;
				Padding prev = TopPanelPadding;
				topPanelPadding = value;
				OnOptionsChanged("TopPanelPadding", prev, TopPanelPadding);
			}
		}
		Padding bottomPanelPadding = new Padding(5);
		void ResetBottomPanelPadding() { BottomPanelPadding = new Padding(5); }
		bool ShouldSerializeBottomPanelPadding() { return BottomPanelPadding != new Padding(5); }
		public virtual Padding BottomPanelPadding {
			get { return bottomPanelPadding; }
			set {
				if(BottomPanelPadding == value)
					return;
				Padding prev = BottomPanelPadding;
				bottomPanelPadding = value;
				OnOptionsChanged("BottomPanelPadding", prev, BottomPanelPadding);
			}
		}
		Padding centerPanelPadding = new Padding(5);
		void ResetCenterPanelPadding() { CenterPanelPadding = new Padding(5); }
		bool ShouldSerializeCenterPanelPadding() { return CenterPanelPadding != new Padding(5); }
		public virtual Padding CenterPanelPadding {
			get { return centerPanelPadding; }
			set {
				if(CenterPanelPadding == value)
					return;
				Padding prev = CenterPanelPadding;
				centerPanelPadding = value;
				OnOptionsChanged("CenterPanelPadding", prev, CenterPanelPadding);
			}
		}
		Padding nearPanelPadding = new Padding(5);
		void ResetNearPanelPadding() { NearPanelPadding = new Padding(5); }
		bool ShouldSerializeNearPanelPadding() { return NearPanelPadding != new Padding(5); }
		public virtual Padding NearPanelPadding {
			get { return nearPanelPadding; }
			set {
				if(NearPanelPadding == value)
					return;
				Padding prev = NearPanelPadding;
				nearPanelPadding = value;
				OnOptionsChanged("NearPanelPadding", prev, NearPanelPadding);
			}
		}
		Padding farPanelPadding = new Padding(5);
		void ResetFarPanelPadding() { FarPanelPadding = new Padding(5); }
		bool ShouldSerializeFarPanelPadding() { return FarPanelPadding != new Padding(5); }
		public virtual Padding FarPanelPadding {
			get { return farPanelPadding; }
			set {
				if(FarPanelPadding == value)
					return;
				Padding prev = FarPanelPadding;
				farPanelPadding = value;
				OnOptionsChanged("FarPanelPadding", prev, FarPanelPadding);
			}
		}
		Color topPanelColor = Color.Empty;
		void ResetTopPanelColor() { TopPanelColor = Color.Empty; }
		bool ShouldSerializeTopPanelColor() { return !TopPanelColor.IsEmpty; }
		public Color TopPanelColor {
			get { return topPanelColor; }
			set {
				if(TopPanelColor == value)
					return;
				Color prev = TopPanelColor;
				topPanelColor = value;
				OnOptionsChanged("TopPanelColor", prev, TopPanelColor);
			}
		}
		Color bottomPanelColor = Color.Empty;
		void ResetBottomPanelColor() { BottomPanelColor = Color.Empty; }
		bool ShouldSerializeBottomPanelColor() { return !BottomPanelColor.IsEmpty; }
		public Color BottomPanelColor {
			get { return bottomPanelColor; }
			set {
				if(BottomPanelColor == value)
					return;
				Color prev = BottomPanelColor;
				bottomPanelColor = value;
				OnOptionsChanged("BottomPanelColor", prev, BottomPanelColor);
			}
		}
		Color centerPanelColor = Color.Empty;
		void ResetCenterPanelColor() { CenterPanelColor = Color.Empty; }
		bool ShouldSerializeCenterPanelColor() { return !CenterPanelColor.IsEmpty; }
		public Color CenterPanelColor {
			get { return centerPanelColor; }
			set {
				if(CenterPanelColor == value)
					return;
				Color prev = CenterPanelColor;
				centerPanelColor = value;
				OnOptionsChanged("CenterPanelColor", prev, CenterPanelColor);
			}
		}
		Color nearPanelColor = Color.Empty;
		void ResetNearPanelColor() { NearPanelColor = Color.Empty; }
		bool ShouldSerializeNearPanelColor() { return !NearPanelColor.IsEmpty; }
		public Color NearPanelColor {
			get { return nearPanelColor; }
			set {
				if(NearPanelColor == value)
					return;
				Color prev = NearPanelColor;
				nearPanelColor = value;
				OnOptionsChanged("NearPanelColor", prev, NearPanelColor);
			}
		}
		Color farPanelColor = Color.Empty;
		void ResetFarPanelColor() { FarPanelColor = Color.Empty; }
		bool ShouldSerializeFarPanelColor() { return !FarPanelColor.IsEmpty; }
		public Color FarPanelColor {
			get { return farPanelColor; }
			set {
				if(FarPanelColor == value)
					return;
				Color prev = FarPanelColor;
				farPanelColor = value;
				OnOptionsChanged("FarPanelColor", prev, FarPanelColor);
			}
		}
		int indent = 0;
		[DefaultValue(0)]
		public int Indent {
			get { return indent; }
			set {
				if(Indent == value)
					return;
				int prev = Indent;
				indent = value;
				OnOptionsChanged("Indent", prev, Indent);
			}
		}
		int animationTime = 200;
		[DefaultValue(200)]
		public int AnimationTime {
			get { return animationTime; }
			set {
				value = Math.Max(value, 0);
				if(AnimationTime == value) 
					return;
				int prev = AnimationTime;
				animationTime = value;
				OnOptionsChanged("AnimationTime", prev, AnimationTime);
			}
		}
		ContextAnimationType animationType = ContextAnimationType.Default;
		[DefaultValue(ContextAnimationType.Default)]
		public ContextAnimationType AnimationType {
			get { return animationType; }
			set {
				if(AnimationType == value)
					return;
				ContextAnimationType prev = AnimationType;
				animationType = value;
				OnOptionsChanged("AnimationType", prev, AnimationType);
			}
		}
		protected internal virtual ContextAnimationType GetAnimationType() {			
			return (AnimationType == ContextAnimationType.Default && Owner != null) ? Owner.AnimationType: AnimationType;
		}
		bool allowHtmlText = false;
		[DefaultValue(false)]
		public bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value)
					return;
				bool prev = AllowHtmlText;
				allowHtmlText = value;
				OnOptionsChanged("AllowHtmlText", prev, AllowHtmlText);
			}
		}
		bool showToolTips = true;
		[DefaultValue(true)]
		public bool ShowToolTips {
			get { return showToolTips; }
			set {
				if(showToolTips == value) return;
				showToolTips = value;
			}
		}
		DefaultBoolean allowHtmlTextInToolTip = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowHtmlTextInToolTip {
			get { return allowHtmlTextInToolTip; }
			set {
				if(allowHtmlTextInToolTip == value) return;
				allowHtmlTextInToolTip = value;
			}
		}
		protected virtual ContextItemCollectionOptions CreateOptions() { return new ContextItemCollectionOptions(); }
		public virtual ContextItemCollectionOptions Clone() {
			ContextItemCollectionOptions options = CreateOptions();
			options.Assign(this);
			return options;
		}
		public virtual void Assign(ContextItemCollectionOptions options) {
			this.allowHtmlText = options.AllowHtmlText;
			this.animationTime = options.AnimationTime;
			this.animationType = options.AnimationType;
			this.bottomPanelColor = options.BottomPanelColor;
			this.topPanelColor = options.TopPanelColor;
			this.centerPanelColor = options.CenterPanelColor;
			this.nearPanelColor = options.NearPanelColor;
			this.farPanelColor = options.FarPanelColor;
			this.bottomPanelPadding = options.BottomPanelPadding;
			this.indent = options.Indent;
			this.topPanelPadding = options.TopPanelPadding;
			this.centerPanelPadding = options.CenterPanelPadding;
			this.nearPanelPadding = options.NearPanelPadding;
			this.farPanelPadding = options.FarPanelPadding;
			this.showToolTips = options.ShowToolTips;
			this.allowHtmlTextInToolTip = options.AllowHtmlTextInToolTip;
		}
		protected virtual void OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			if(Owner != null)
				Owner.OnOptionsChanged(propertyName, oldValue, newValue);
		}
	}
	[Editor("DevExpress.XtraEditors.Design.ContextItemCollectionUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class ContextItemCollection : CollectionBase, ICloneable {
		public ContextItemCollection() : this(null) { }
		public ContextItemCollection(IContextItemCollectionOwner owner) {
			Owner = owner;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ContextItemCollectionOptions Options { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IContextItemCollectionOwner Owner { get; set; }
		public ContextItem this[int index] { get { return (ContextItem)List[index]; } set { List[index] = value; } }
		public ContextItem this[string name] {
			get {
				for(int i = 0; i < List.Count; i++) {
					ContextItem item = List[i] as ContextItem;
					if(item.Name == name)
						return item;
				}
				return null;
			}
		}
		public int IndexOf(ContextItem item) {  return List.IndexOf(item); }
		public int Add(ContextItem item) {
			return List.Add(item);
		}
		public void Insert(int index, ContextItem item) {
			List.Insert(index, item);
		}
		public void Remove(ContextItem item) {
			List.Remove(item);
		}
		public bool Contains(ContextItem item) { 
			return List.Contains(item); 
		}		
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			((ContextItem)value).Collection = this;
			RaiseItemCollectionChanged(index, false);
			OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() {
			if(Owner != null) {
				UpdateAnchorElements();
				Owner.OnCollectionChanged();
			}
		}
		private void UpdateAnchorElements() {
			if(!Owner.IsDesignMode)
				return;
			foreach(ContextItem item in this) {
				if(item.AnchorElementId != Guid.Empty && item.AnchorElement == null)
					item.AnchorElement = item.GetAnchorElementById(item.AnchorElementId);
			}
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			((ContextItem)value).Collection = null;
			RaiseItemCollectionChanged(index, true);
			OnCollectionChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged();
		}
		public object Clone() {
			ContextItemCollection res = new ContextItemCollection();
			foreach(ContextItem item in this) {
				res.Add((ContextItem)item.Clone());
			}
			return res;
		}
		public event ItemCollectionChangedEventHandler ItemCollectionChanged;
		protected virtual void RaiseItemCollectionChanged(int index, bool isRemoved) {
			if(ItemCollectionChanged == null) return;
			ItemCollectionChangedEventArgs args = new ItemCollectionChangedEventArgs(index, isRemoved);
			ItemCollectionChanged(this, args);	   
		}	  
	}
	public class ContextItemCollectionHandler {
		int lockUpdate;
		public ContextItemCollectionHandler() : this(null) { }
		public ContextItemCollectionHandler(ContextItemCollectionViewInfo collectionViewInfo) {
			ViewInfo = collectionViewInfo;
			this.lockUpdate = 0;
		}
		protected Cursor PrevCursor { get; set; }
		protected Cursor DefaultCursor { get { return Cursors.Arrow; } }
		public ContextItemCollectionViewInfo ViewInfo { get; set; }
		public virtual void UpdateItemsByMouse(Point location) {
			if(ViewInfo == null)
				return;
			if(ViewInfo.Owner.DesignMode)
				return;
			ViewInfo.MousePosition = location;
			if(ViewInfo.Owner.ActivationBounds.Contains(location)) {
				CheckViewInfo();
				ViewInfo.ShouldShowAutoItems = true;
			}
			else
				ViewInfo.ShouldShowAutoItems = false;
			ViewInfo.UpdateContextButtonHitInfo(location);
			UpdateCursor(location);
			ViewInfo.UpdateHyperlinkCursor(location);
		}
		private void UpdateCursor(Point location) {
			if(PrevCursor == null)
				PrevCursor = ViewInfo.OwnerControl.Cursor;
			if(ViewInfo.TopPanelBounds.Contains(location) ||
				ViewInfo.BottomPanelBounds.Contains(location) ||
				ViewInfo.CenterPanelBounds.Contains(location) ||
				ViewInfo.NearPanelBounds.Contains(location) ||
				ViewInfo.FarPanelBounds.Contains(location) ||
				ViewInfo.HoverItem != null)
				ViewInfo.SetCursor(DefaultCursor);
			else {
				ViewInfo.SetCursor(PrevCursor);
				PrevCursor = null;
			}
		}
		protected virtual void CheckViewInfo() {
			if(ViewInfo == null || ViewInfo.IsReady)
				return;
			ViewInfo.CalcItems();
		}
		public virtual bool OnMouseMove(MouseEventArgs e) {
			if(IsEmptyViewInfo)
				return false;
			UpdateItemsByMouse(e.Location);
			if(PressedItem != null && PressedItem.Item.Enabled)
				PressedItem.OnMouseMove(e);
			return true;
		}
		protected virtual Point GetCurrentMousePosition() { return Control.MousePosition; }
		public virtual bool OnMouseEnter(EventArgs e) {
			if(IsEmptyViewInfo)
				return false;
			ViewInfo.PrevCursor = null;
			UpdateItemsByMouse(ViewInfo.OwnerControl.PointToClient(GetCurrentMousePosition()));
			return true;
		}
		public virtual bool OnMouseLeave(EventArgs e) {
			if(IsEmptyViewInfo)
				return false;
			ViewInfo.MousePosition = ViewInfo.OwnerControl.PointToClient(GetCurrentMousePosition());
			ViewInfo.ShouldShowAutoItems = false;
			ViewInfo.UpdateContextButtonHitInfo(new Point(ViewInfo.Owner.DisplayBounds.Location.X - 10, ViewInfo.Owner.DisplayBounds.Location.Y - 10));
			ViewInfo.PrevCursor = null;
			return true;
		}
		ContextItemViewInfo pressedItem;
		protected ContextItemViewInfo PressedItem {
			get { return pressedItem; }
			set {
				if(PressedItem == value)
					return;
				ContextItemViewInfo prev = PressedItem;
				pressedItem = value;
				OnPressedItemChanged(prev, PressedItem);
			}
		}
		protected virtual void OnPressedItemChanged(ContextItemViewInfo prev, ContextItemViewInfo current) {
			if(prev != null)
				ViewInfo.Owner.Redraw(prev.RedrawBounds);
			if(current != null)
				ViewInfo.Owner.Redraw(current.RedrawBounds);
		}
		public virtual bool OnMouseUp(MouseEventArgs e) {
			if(IsEmptyViewInfo || lockUpdate != 0)
				return false;
			lockUpdate++;
			ContextItemViewInfo releasedItem = ViewInfo.GetItemViewInfoByLocation(e.Location);
			if(PressedItem != null && releasedItem == PressedItem && PressedItem.Item.Enabled) {
				PressedItem.OnItemClick(e);
				PressedItem.OnMouseUp(e);
				if(PressedItem.PressedInfo != null)
					PressedItem.PressedInfo.Clear();
				PressedItem = null;
				lockUpdate--;
				return true;
			}
			PressedItem = null;
			lockUpdate--;
			return false;
		}
		bool IsEmptyViewInfo { get { return ViewInfo == null || ViewInfo.Owner == null || ViewInfo.Collection == null || ViewInfo.Options == null; } }
		public virtual bool OnMouseDown(MouseEventArgs e) {
			if(IsEmptyViewInfo || lockUpdate != 0)
				return false;
			PressedItem = ViewInfo.GetItemViewInfoByLocation(e.Location);
			if(PressedItem != null && PressedItem.Item.Enabled) {
				ViewInfo.UpdateContextButtonPressedInfo(e.Location);
				ViewInfo.Owner.Redraw(PressedItem.RedrawBounds);
			}
			return PressedItem != null;
		}
		public virtual bool OnSizeChanged(EventArgs e) {
			if(ViewInfo == null)
				return false;
			ViewInfo.CalcItems();
			ViewInfo.Owner.Redraw();
			ViewInfo.Owner.Update();
			return true;
		}
		public virtual bool EnableInnerAnimation(bool enabled) {
			if(IsEmptyViewInfo)
				return false;
			if(enabled)
				ViewInfo.StopAnimation = false;
			else {
				ViewInfo.HoverItem = null;
				ViewInfo.ShouldShowAutoItems = false;
				ViewInfo.StopAnimation = true;
				ViewInfo.StopAllAnimations();			  
			}
			return true;
		}		
	}
	public class ContextItemCollectionViewInfo : ISupportXtraAnimation {
		ContextItemCollection collection;
		ContextItemCollectionOptions options;
		bool shouldShowAutoItems;
		bool stopAnimation;
		List<ContextItemViewInfo> viewInfos;
		internal int sideIndent;
		internal int verticalIndent;
		bool upperLineFilled = false;
		bool lowerLineFilled = false;
		bool centerLineFilled = false;
		bool nearLineFilled = false;
		bool farLineFilled = false;
		float prevFloatAnimationCoeff = 0;
		float prevWaveAnimationCoeff = 0;
		List<float> opacities;
		List<ContextItemViewInfo> upperLeftViewInfos;
		List<ContextItemViewInfo> middleTopViewInfos;
		List<ContextItemViewInfo> upperRightViewInfos;
		List<ContextItemViewInfo> lowerLeftViewInfos;
		List<ContextItemViewInfo> middleBottomViewInfos;
		List<ContextItemViewInfo> lowerRightViewInfos;
		List<ContextItemViewInfo> centerLeftViewInfos;
		List<ContextItemViewInfo> centerMiddleViewInfos;
		List<ContextItemViewInfo> centerRightViewInfos;
		List<ContextItemViewInfo> nearTopViewInfos;
		List<ContextItemViewInfo> nearCenterViewInfos;
		List<ContextItemViewInfo> nearBottomViewInfos;
		List<ContextItemViewInfo> farTopViewInfos;
		List<ContextItemViewInfo> farCenterViewInfos;
		List<ContextItemViewInfo> farBottomViewInfos;
		GraphicsInfo gInfo;
		public ContextItemCollectionViewInfo(ContextItemCollection coll, ContextItemCollectionOptions options, ISupportContextItems owner) {
			this.collection = coll;
			this.options = options;
			Owner = owner;	
			this.shouldShowAutoItems = false;
			this.stopAnimation = false;
			this.viewInfos = new List<ContextItemViewInfo>();
			this.gInfo = GraphicsInfo.Default;
			MousePosition = new Point(int.MinValue, int.MinValue);
		}
		public virtual GraphicsInfo GInfo { get { return gInfo; } }
		Rectangle OffsetRect(Rectangle rect, int deltaX, int deltaY) {
			rect.X += deltaX; rect.Y += deltaY;
			return rect;
		}
		public void MakeOffset(int deltaX, int deltaY) {
			TopPanelBounds = OffsetRect(TopPanelBounds, deltaX, deltaY);
			BottomPanelBounds = OffsetRect(BottomPanelBounds, deltaX, deltaY);
			CenterPanelBounds = OffsetRect(CenterPanelBounds, deltaX, deltaY);
			NearPanelBounds = OffsetRect(NearPanelBounds, deltaX, deltaY);
			FarPanelBounds = OffsetRect(FarPanelBounds, deltaX, deltaY);
			TopContentBounds = OffsetRect(TopContentBounds, deltaX, deltaY);
			BottomContentBounds = OffsetRect(BottomContentBounds, deltaX, deltaY);
			CenterContentBounds = OffsetRect(CenterContentBounds, deltaX, deltaY);
			NearContentBounds = OffsetRect(NearContentBounds, deltaX, deltaY);
			FarContentBounds = OffsetRect(FarContentBounds, deltaX, deltaY);
			foreach(ContextItemViewInfo itemInfo in Items) {
				itemInfo.MakeOffset(deltaX, deltaY);
			}
		}
		void Collection_ItemCollectionChanged(object sender, ItemCollectionChangedEventArgs e) {
			if(e.IsRemoved) {
				Items.RemoveAt(e.Index);
			}
			else
				Items.Insert(e.Index,Collection[e.Index].CreateViewInfo(this));
			CalcItems();
			Owner.Redraw();
			Owner.Update();
		}
		protected internal bool IsRightToLeft { 
			get { 
				if(Collection.Owner != null)
					return Collection.Owner.IsRightToLeft;
				return false;
			} 
		}
		protected internal bool HasAlwaysVisibleItems(List<ContextItemViewInfo> items) {
			foreach(ContextItemViewInfo item in items) {
				if(item.IsNotFittedHeight || item.IsNotFittedWidth)
					continue;
				if(item.Item.Visibility == ContextItemVisibility.Visible)
					return true;
			}
			return false;
		}
		protected internal bool HasUpperVisibleItems {
			get { 
				return HasVisibleItems(UpperLeftViewInfos) || HasVisibleItems(MiddleTopViewInfos) || HasVisibleItems(UpperRightViewInfos); 
			}
		}
		protected internal bool HasBottomVisibleItems {
			get {
				return HasVisibleItems(LowerLeftViewInfos) || HasVisibleItems(MiddleBottomViewInfos) || HasVisibleItems(LowerRightViewInfos);
			}
		}
		protected internal bool HasCenterVisibleItems {
			get {
				return HasVisibleItems(CenterLeftViewInfos) || HasVisibleItems(MiddleCenterViewInfos) || HasVisibleItems(CenterRightViewInfos);
			}
		}
		protected internal bool HasVisibleItems(List<ContextItemViewInfo> items) {
			foreach(ContextItemViewInfo item in items) {
				if(item.IsNotFittedHeight || item.IsNotFittedWidth)
					continue;
				if(item.Item.Visibility == ContextItemVisibility.Visible || 
					item.Item.Visibility == ContextItemVisibility.Auto && ShouldShowAutoItems)
					return true;
			}
			return false;
		}
		protected internal bool HasVisibleItems() {
			foreach(ContextItem item in Collection) {
				if(item.Visibility == ContextItemVisibility.Visible ||
					item.Visibility == ContextItemVisibility.Auto && ShouldShowAutoItems)
					return true;
			}
			return false;
		}
		public ContextItemCollection Collection { get { return collection; }}
		public ContextItemCollectionOptions Options { get { return options; } }
		public ISupportContextItems Owner { get; private set; }
		public bool ShouldShowAutoItems {
			get {
				return shouldShowAutoItems;
			}
			set {
				if(shouldShowAutoItems == value) return;
				shouldShowAutoItems = value;
				RedrawAutoItems();		
			}
		}
		public bool StopAnimation {
			get {
				return stopAnimation;
			}
			set {
				if(stopAnimation == value) return;
				stopAnimation = value;
			}
		}
		public List<ContextItemViewInfo> Items { get { return viewInfos; } }
		protected internal Cursor PrevCursor { get; set; }
		public Rectangle Bounds { get { return Owner.DisplayBounds; } }
		public Rectangle ContentBounds {
			get {
				Rectangle contentBounds = Bounds;
				contentBounds.Inflate(-Indent, -Indent);
				return contentBounds;
			}
		}
		public int Indent { get { return Options.Indent; } }
		public ISupportXtraAnimation XtraAnimationObject { get { return this; } }
		public float CurrentOpacity { get; internal set; }
		public float PrevOpacity {
			get { return prevFloatAnimationCoeff; }
			internal set { prevFloatAnimationCoeff = value; }
		}
		#region ItemsViewInfos
		public List<ContextItemViewInfo> UpperLeftViewInfos { 
			get {
				if(upperLeftViewInfos == null)
					upperLeftViewInfos = new List<ContextItemViewInfo>();
				return upperLeftViewInfos; 
			}
			internal set {
				if(upperLeftViewInfos == value) return;
				upperLeftViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> UpperRightViewInfos {
			get {
				if(upperRightViewInfos == null)
					upperRightViewInfos = new List<ContextItemViewInfo>();
				return upperRightViewInfos; 
			}
			internal set {
				if(upperRightViewInfos == value) return;
				upperRightViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> MiddleTopViewInfos {
			get {
				if(middleTopViewInfos == null)
					middleTopViewInfos = new List<ContextItemViewInfo>();
				return middleTopViewInfos; 
			}
			internal set {
				if(middleTopViewInfos == value) return;
				middleTopViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> MiddleBottomViewInfos {
			get {
				if(middleBottomViewInfos == null)
					middleBottomViewInfos = new List<ContextItemViewInfo>();
				return middleBottomViewInfos;
			}
			internal set {
				if(middleBottomViewInfos == value) return;
				middleBottomViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> LowerLeftViewInfos {
			get {
				if(lowerLeftViewInfos == null)
					lowerLeftViewInfos = new List<ContextItemViewInfo>();
				return lowerLeftViewInfos;
			}
			internal set {
				if(lowerLeftViewInfos == value) return;
				lowerLeftViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> LowerRightViewInfos {
			get {
				if(lowerRightViewInfos == null)
					lowerRightViewInfos = new List<ContextItemViewInfo>();
				return lowerRightViewInfos;
			}
			internal set {
				if(lowerRightViewInfos == value) return;
				lowerRightViewInfos = value;
			} 
		}
		public List<ContextItemViewInfo> CenterLeftViewInfos {
			get {
				if(centerLeftViewInfos == null)
					centerLeftViewInfos = new List<ContextItemViewInfo>();
				return centerLeftViewInfos;
			}
			internal set {
				if(centerLeftViewInfos == value) return;
				centerLeftViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> MiddleCenterViewInfos {
			get {
				if(centerMiddleViewInfos == null)
					centerMiddleViewInfos = new List<ContextItemViewInfo>();
				return centerMiddleViewInfos;
			}
			internal set {
				if(centerMiddleViewInfos == value) return;
				centerMiddleViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> CenterRightViewInfos {
			get {
				if(centerRightViewInfos == null)
					centerRightViewInfos = new List<ContextItemViewInfo>();
				return centerRightViewInfos;
			}
			internal set {
				if(centerRightViewInfos == value) return;
				centerRightViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> NearTopViewInfos {
			get {
				if(nearTopViewInfos == null)
					nearTopViewInfos = new List<ContextItemViewInfo>();
				return nearTopViewInfos;
			}
			internal set {
				if(nearTopViewInfos == value) return;
				nearTopViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> NearCenterViewInfos {
			get {
				if(nearCenterViewInfos == null)
					nearCenterViewInfos = new List<ContextItemViewInfo>();
				return nearCenterViewInfos;
			}
			internal set {
				if(nearCenterViewInfos == value) return;
				nearCenterViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> NearBottomViewInfos {
			get {
				if(nearBottomViewInfos == null)
					nearBottomViewInfos = new List<ContextItemViewInfo>();
				return nearBottomViewInfos;
			}
			internal set {
				if(nearBottomViewInfos == value) return;
				nearBottomViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> FarTopViewInfos {
			get {
				if(farTopViewInfos == null)
					farTopViewInfos = new List<ContextItemViewInfo>();
				return farTopViewInfos;
			}
			internal set {
				if(farTopViewInfos == value) return;
				farTopViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> FarCenterViewInfos {
			get {
				if(farCenterViewInfos == null)
					farCenterViewInfos = new List<ContextItemViewInfo>();
				return farCenterViewInfos;
			}
			internal set {
				if(farCenterViewInfos == value) return;
				farCenterViewInfos = value;
			}
		}
		public List<ContextItemViewInfo> FarBottomViewInfos {
			get {
				if(farBottomViewInfos == null)
					farBottomViewInfos = new List<ContextItemViewInfo>();
				return farBottomViewInfos;
			}
			internal set {
				if(farBottomViewInfos == value) return;
				farBottomViewInfos = value;
			}
		}
		#endregion
		protected virtual void RedrawAutoItems() {
			if(Owner.Options == null)
				return;
			if(Items.Count == 0) return;
			if(!Owner.Options.TopPanelColor.IsEmpty || !Owner.Options.BottomPanelColor.IsEmpty || !Owner.Options.CenterPanelColor.IsEmpty ||
				!Owner.Options.NearPanelColor.IsEmpty || !Owner.Options.FarPanelColor.IsEmpty) {
				Owner.Redraw();
				Owner.Update();
				return;
			}
			List<ContextItemViewInfo> viewInfos = GetViewInfosByVisibility(ContextItemVisibility.Auto);
			foreach(ContextItemViewInfo viewInfo in viewInfos)
				Owner.Redraw(viewInfo.RedrawBounds);
			Owner.Update();
		}
		protected virtual internal void CreateItemViewInfos(){
			viewInfos = new List<ContextItemViewInfo>();
			if(Collection == null)
				return;
			foreach(ContextItem item in Collection) {
				ContextItem res = Owner.CloneItems ? GetClonedItem(item) : item;
				if(item.Visibility != ContextItemVisibility.Hidden)
					viewInfos.Add(res.CreateViewInfo(this));
			}
		}
		private void CustomizeContextItems() {
			if(!Owner.CloneItems)
				return;
			foreach(ContextItemViewInfo itemInfo in Items) {
				itemInfo.Item.BeginUpdate();
				Owner.RaiseCustomizeContextItem(itemInfo.Item);
				CheckContextButton check = itemInfo.Item as CheckContextButton;
				if(check != null)
					((CheckContextButtonViewInfo)itemInfo).Checked = check.Checked;
				RatingContextButton rating = itemInfo.Item as RatingContextButton;
				if(rating != null)
					((RatingContextButtonViewInfo)itemInfo).Rating = rating.Rating;
				TrackBarContextButton trackBar = itemInfo.Item as TrackBarContextButton;
				if(trackBar != null)
					((TrackBarContextButtonViewInfo)itemInfo).CurrentValue = trackBar.Value;
				itemInfo.Item.CancelUpdate();
			}
		}
		protected virtual ContextItem GetClonedItem(ContextItem item) {
			ContextItem res = (ContextItem)item.Clone();
			res.OriginItem = item;
			return res;
		}
		protected virtual void ResetItemsSettings() {
			foreach(ContextItemViewInfo viewInfo in Items) {
				viewInfo.IsNotFittedWidth = false;
				viewInfo.IsNotFittedHeight = false;
			}
			ResetButtonSizes();
			this.upperLineFilled = false;
			this.lowerLineFilled = false;
			this.centerLineFilled = false;
			this.nearLineFilled = false;
			this.farLineFilled = false;
		}
		protected internal bool IsReady { get; set; }
		public void InvalidateViewInfo() { IsReady = false; }
		public virtual void CalcItems() {
			if(IsReady)
				return;
			GInfo.AddGraphics(null);
			try {
				CreateItemViewInfos();
				CustomizeContextItems();
				ResetItemsSettings();
				UpdateAnchorElements();
				CalcBestSize();
				CalcTopContentBounds();
				CalcBottomContentBounds();
				CalcUpperLeftItems();
				CalcUpperRightItems();
				CalcMiddleTopItems();
				CalcLowerLeftItems();
				CalcLowerRightItems();
				CalcMiddleBottomItems();
				CheckForIntersection();
				CalcTopPanelBounds();
				CalcBottomPanelBounds();
				CalcNearContentBounds();
				CalcFarContentBounds();
				CalcNearTopItems();
				CalcNearBottomItems();
				CalcNearCenterItems();
				CalcFarTopItems();
				CalcFarBottomItems();
				CalcFarCenterItems();
				CheckForIntersection();
				CalcNearPanelBounds();
				CalcFarPanelBounds();
				CalcCenterContentBounds();
				CalcCenterLeftItems();
				CalcCenterRightItems();
				CalcMiddleCenterItems();
				CheckForIntersection();
				CalcCenterPanelBounds();
				UpdateTopContentBounds();
				UpdateBottomContentBounds();
				UpdateCenterContentBounds();
				UpdateNearContentBounds();
				UpdateFarContentBounds();
				UpdateItemsByOutsideDisplayBounds();
				UpdateItemsVerticalAlignment();
				UpdateItemsHorizontalAlignment();
				if(IsRightToLeft)
					UpdateToRTL();
				CalcItemsViewInfo();
				IsReady = true;
			}
			finally { GInfo.ReleaseGraphics(); }
		}
		protected virtual void UpdateToRTL() {
			UpdateItemsToRTL();
			UpdatePanelsToRTL();
		}
		void UpdatePanelsToRTL() {
			NearPanelBounds = RotateRectToRTL(NearPanelBounds);
			FarPanelBounds = RotateRectToRTL(FarPanelBounds);
			CenterPanelBounds = RotateRectToRTL(CenterPanelBounds);
		}
		Rectangle RotateRectToRTL(Rectangle rect){
			return new Rectangle(Bounds.Right - (rect.Width + rect.X) + Bounds.X, rect.Y, rect.Width, rect.Height);
		}
		private void UpdateItemsByOutsideDisplayBounds() {
			if(!Owner.ShowOutsideDisplayBounds)
				return;
			ShiftItemsVertically(TopContentBounds, this.upperLeftViewInfos, -TopPanelBounds.Height);
			ShiftItemsVertically(TopContentBounds, this.upperRightViewInfos, -TopPanelBounds.Height);
			ShiftItemsVertically(TopContentBounds, this.middleTopViewInfos, -TopPanelBounds.Height);
			ShiftItemsVertically(BottomContentBounds, this.lowerLeftViewInfos, BottomPanelBounds.Height);
			ShiftItemsVertically(BottomContentBounds, this.lowerRightViewInfos, BottomPanelBounds.Height);
			ShiftItemsVertically(BottomContentBounds, this.middleBottomViewInfos, BottomPanelBounds.Height);
		}
		private void ShiftItemsVertically(Rectangle bounds, List<ContextItemViewInfo> items, int delta) {
			foreach(ContextItemViewInfo info in items) {
				info.Bounds = new Rectangle(info.Bounds.X, info.Bounds.Y + delta, info.Bounds.Width, info.Bounds.Height);
			}
		}
		protected virtual void UpdateAnchorElements() {
			foreach(ContextItemViewInfo item in Items) {
				if(item.Item.AnchorElementId != Guid.Empty) {
					item.Item.BeginUpdate();
					item.Item.AnchorElement = GetAnchorElementById(item.Item.AnchorElementId);
					item.Item.CancelUpdate();
				}
			}
		}
		protected virtual  ContextItem GetAnchorElementById(Guid anchorId) {
			foreach(ContextItemViewInfo item in Items) {
				if(item.Item.Id == anchorId)
					return item.Item;
			}
			return null;
		}
		protected virtual void UpdateBottomContentBounds() {
			BottomContentBounds = new Rectangle(BottomContentBounds.X, BottomPanelBounds.Y + Options.BottomPanelPadding.Top, BottomContentBounds.Width, BottomPanelBounds.Height - Options.BottomPanelPadding.Vertical);
		}
		protected virtual void UpdateTopContentBounds() {
			TopContentBounds = new Rectangle(TopContentBounds.X, TopPanelBounds.Y + Owner.Options.TopPanelPadding.Top, TopContentBounds.Width, TopPanelBounds.Height - Options.TopPanelPadding.Vertical);
		}
		protected virtual void UpdateCenterContentBounds() {
			CenterContentBounds = new Rectangle(CenterContentBounds.X, CenterPanelBounds.Y + Options.CenterPanelPadding.Top, CenterContentBounds.Width, CenterPanelBounds.Height - Options.CenterPanelPadding.Vertical);
		}
		protected virtual void UpdateNearContentBounds() {
			NearContentBounds = new Rectangle(NearPanelBounds.X + Options.NearPanelPadding.Left, NearContentBounds.Y, NearPanelBounds.Width - Options.NearPanelPadding.Horizontal, NearContentBounds.Height);
		}
		protected virtual void UpdateFarContentBounds() {
			FarContentBounds = new Rectangle(FarPanelBounds.X + Options.FarPanelPadding.Left, FarContentBounds.Y, FarPanelBounds.Width - Options.FarPanelPadding.Horizontal, FarContentBounds.Height);
		}
		void UpdateItemsToRTL() {
			foreach(ContextItemViewInfo itemInfo in Items) {
				itemInfo.Bounds = RotateRectToRTL(itemInfo.Bounds);
			}
		}
		protected internal virtual void CalcItemsViewInfo() {
			foreach(ContextItemViewInfo itemInfo in Items) {
				itemInfo.CalcViewInfo(itemInfo.Bounds);
			}
		}
		protected virtual void UpdateItemsVerticalAlignment() {
			UpdateItemsVerticalAlignment(TopContentBounds, this.upperLeftViewInfos, true);
			UpdateItemsVerticalAlignment(TopContentBounds, this.middleTopViewInfos, true);
			UpdateItemsVerticalAlignment(TopContentBounds, this.upperRightViewInfos, true);
			UpdateItemsVerticalAlignment(BottomContentBounds, this.lowerLeftViewInfos, false);
			UpdateItemsVerticalAlignment(BottomContentBounds, this.middleBottomViewInfos, false);
			UpdateItemsVerticalAlignment(BottomContentBounds, this.lowerRightViewInfos, false);
		}
		protected virtual void UpdateItemsHorizontalAlignment() {
			UpdateItemsHorizontalAlignment(NearContentBounds, this.nearTopViewInfos, true);
			UpdateItemsHorizontalAlignment(NearContentBounds, this.nearCenterViewInfos, true);
			UpdateItemsHorizontalAlignment(NearContentBounds, this.nearBottomViewInfos, true);
			UpdateItemsHorizontalAlignment(FarContentBounds, this.farTopViewInfos, false);
			UpdateItemsHorizontalAlignment(FarContentBounds, this.farCenterViewInfos, false);
			UpdateItemsHorizontalAlignment(FarContentBounds, this.farBottomViewInfos, false);
		}
		protected virtual int CalcVerticalOffset(Rectangle contentBounds, Rectangle totalItemBounds, ContextItemViewInfo itemInfo, bool shiftDown) {
			int offset = (contentBounds.Height - totalItemBounds.Height) / 2;
			if(!itemInfo.IsNotFittedWidth && !itemInfo.IsNotFittedHeight) {
				if(!shiftDown)
					offset = -offset;
			}
			else {
				if(offset > 0) {
					if(totalItemBounds.Top > contentBounds.Top)
						offset = -offset;
				}
				else {
					if(totalItemBounds.Top > contentBounds.Top)
						offset = -(totalItemBounds.Top - contentBounds.Top);
				}
			}
			return offset;
		}
		protected virtual bool IsVerticalAlignmentNeeded(List<ContextItemViewInfo> items) {
			foreach(ContextItemViewInfo itemInfo in items) {
				if(itemInfo.Item.AnchorElement != null)
					return false;
			}
			return true;
		}
		protected virtual void UpdateItemsVerticalAlignment(Rectangle bounds, List<ContextItemViewInfo> items, bool shiftDown) {
			if(!IsVerticalAlignmentNeeded(items)) 
				return;
			foreach(ContextItemViewInfo itemInfo in items) {			   
				int deltaHeight = (bounds.Height - itemInfo.Bounds.Height) / 2;
				if(!shiftDown)
					deltaHeight = -deltaHeight;
				itemInfo.Bounds = new Rectangle(itemInfo.Bounds.X, itemInfo.Bounds.Y + deltaHeight, itemInfo.Bounds.Width, itemInfo.Bounds.Height);			
			} 
		}
		protected virtual void UpdateItemsHorizontalAlignment(Rectangle bounds, List<ContextItemViewInfo> items, bool shiftSide) {
			if(!IsVerticalAlignmentNeeded(items))
				return;
			foreach(ContextItemViewInfo itemInfo in items) {
				int deltaWidth = (bounds.Width - itemInfo.Bounds.Width) / 2;
				if(!shiftSide)
					deltaWidth = -deltaWidth;
				itemInfo.Bounds = new Rectangle(itemInfo.Bounds.X + deltaWidth, itemInfo.Bounds.Y, itemInfo.Bounds.Width, itemInfo.Bounds.Height);
			}
		}
		protected virtual void CalcBestSize() {
			foreach(ContextItemViewInfo itemInfo in Items) {
				itemInfo.CalcBestSize();
			}
		}
		private void CalcTopContentBounds() {
			TopContentBounds = new Rectangle(Bounds.X + Options.TopPanelPadding.Left, Bounds.Y + Options.TopPanelPadding.Top, Bounds.Width - Options.TopPanelPadding.Horizontal, 0);
		}
		private void CalcBottomContentBounds() {
			BottomContentBounds = new Rectangle(Bounds.X + Options.BottomPanelPadding.Left, Bounds.Bottom - Options.BottomPanelPadding.Bottom, Bounds.Width - Options.BottomPanelPadding.Horizontal, 0);
		}
		private void CalcCenterContentBounds() {
			CenterContentBounds = new Rectangle(Bounds.X + NearPanelBounds.Width + Options.CenterPanelPadding.Left, (Bounds.Bottom + Bounds.Top)/2, Bounds.Width - NearPanelBounds.Width - FarPanelBounds.Width - Options.CenterPanelPadding.Horizontal, 0);
		}
		private void CalcNearContentBounds() {
			NearContentBounds = new Rectangle(Bounds.X + Options.NearPanelPadding.Left, Bounds.Y + TopPanelBounds.Height + Options.NearPanelPadding.Top, 0, Bounds.Height - TopPanelBounds.Height - BottomPanelBounds.Height - Options.NearPanelPadding.Vertical);
		}
		private void CalcFarContentBounds() {
			FarContentBounds = new Rectangle(Bounds.Right - Options.FarPanelPadding.Right, Bounds.Y + TopPanelBounds.Height + Options.FarPanelPadding.Top, 0, Bounds.Height - TopPanelBounds.Height - BottomPanelBounds.Height - Options.FarPanelPadding.Vertical);
		}
		protected virtual void CheckForIntersection() {
			List<ContextItemViewInfo> upperViewInfos = GetUpperViewInfos(); 
			List<ContextItemViewInfo> lowerViewInfos = GetLowerViewInfos();
			List<ContextItemViewInfo> centerViewInfos = GetCenterViewInfos();
			List<ContextItemViewInfo> nearViewInfos = GetNearViewInfos();
			List<ContextItemViewInfo> farViewInfos = GetFarViewInfos();
			int upperBottom = int.MinValue;
			foreach(ContextItemViewInfo upperViewInfo in upperViewInfos) {
				if(upperViewInfo.IsNotFittedWidth || upperViewInfo.IsNotFittedHeight) continue;
				Rectangle upperRect = upperViewInfo.Bounds;
				upperBottom = Math.Max(upperBottom, upperRect.Bottom + options.TopPanelPadding.Bottom);
			}
			int lowerTop = int.MaxValue;
			foreach(ContextItemViewInfo lowerViewInfo in lowerViewInfos) {
				if(lowerViewInfo.IsNotFittedWidth || lowerViewInfo.IsNotFittedHeight) continue;
				int lowerRectTop = lowerViewInfo.Bounds.Top - options.BottomPanelPadding.Top;
				if(lowerRectTop < upperBottom) {
					lowerViewInfo.IsNotFittedHeight = true;
					continue;
				}
				lowerTop = Math.Min(lowerTop, lowerRectTop);
			}
			foreach(ContextItemViewInfo centerViewInfo in centerViewInfos) {
				if(centerViewInfo.IsNotFittedHeight || centerViewInfo.IsNotFittedWidth) continue;
				Rectangle centerRect = centerViewInfo.Bounds;
				if(centerRect.Top - options.CenterPanelPadding.Top < upperBottom ||
					centerRect.Bottom + options.CenterPanelPadding.Bottom > lowerTop)
					centerViewInfo.IsNotFittedHeight = true;
			}
			int nearRight = int.MinValue;
			foreach(ContextItemViewInfo nearViewInfo in nearViewInfos) {
				if(nearViewInfo.IsNotFittedWidth || nearViewInfo.IsNotFittedHeight) continue;
				Rectangle nearRect = nearViewInfo.Bounds;
				nearRight = Math.Max(nearRight, nearRect.Right + options.NearPanelPadding.Right);
			}
			int farLeft = int.MaxValue;
			foreach(ContextItemViewInfo farViewInfo in farViewInfos) {
				if(farViewInfo.IsNotFittedWidth || farViewInfo.IsNotFittedHeight) continue;
				int farRectLeft = farViewInfo.Bounds.Left - options.FarPanelPadding.Left;
				if(farRectLeft < nearRight) {
					farViewInfo.IsNotFittedWidth = true;
					continue;
				}
				farLeft = Math.Min(farLeft, farRectLeft);
			}
		}
		protected virtual void CalcMiddleBottomItems() {
			MiddleBottomViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> middleBottomViewInfos = GetViewInfosByAlignment(ContextItemAlignment.MiddleBottom);
			if(middleBottomViewInfos.Count == 0) return;
			if(this.lowerLineFilled) {
				MarkAsNotFitted(middleBottomViewInfos);
				return;
			}
			CalcHorizontalMiddleItems(middleBottomViewInfos);		   
		}	  
		protected virtual void CalcLowerRightItems() {
			LowerRightViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> lowerRightAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.BottomFar);
			if(this.lowerLineFilled) {
				MarkAsNotFitted(lowerRightAlignedViewInfos);
				return;
			}
			int currentX = 0;
			for(int i = 0; i < lowerRightAlignedViewInfos.Count; i++) {
				if(this.lowerLineFilled) {
					MarkAsNotFittedItem(lowerRightAlignedViewInfos[i]);
					continue;
				}
				Size itemSize = lowerRightAlignedViewInfos[i].Size;				
				if(i == 0)
					lowerRightAlignedViewInfos[i].Bounds = new Rectangle(new Point(BottomContentBounds.Right - itemSize.Width, BottomContentBounds.Bottom - itemSize.Height), itemSize);
				else
					lowerRightAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX - Indent - itemSize.Width, BottomContentBounds.Bottom - itemSize.Height), itemSize);
				List<IAnchored> anchorGroup;
				currentX = CalcAnchorGroup(lowerRightAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					LowerRightViewInfos.Add((ContextItemViewInfo)item);
					if(item != lowerRightAlignedViewInfos[i])
						lowerRightAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}
		}
		protected virtual int CalcAnchorGroup(ContextItemViewInfo aligned, out List<IAnchored> anchorGroup) {
			anchorGroup = GetAnchorGroup(aligned);
			AdjustAnchorGroupLocation(anchorGroup);
			int current = CalcCurrentPosition(anchorGroup);
			return current;
		}
		protected virtual void CheckAnchorGroup(List<IAnchored> anchorGroup) {
			ContextItemViewInfo aligned = anchorGroup[0] as ContextItemViewInfo;
			ContextItemAlignment alignment = aligned.Item.Alignment;
			if(alignment == ContextItemAlignment.TopNear || alignment == ContextItemAlignment.BottomNear || alignment == ContextItemAlignment.CenterNear) 
				CheckLeftGroupForFitting(anchorGroup);
			if(alignment == ContextItemAlignment.TopFar || alignment == ContextItemAlignment.BottomFar || alignment == ContextItemAlignment.CenterFar)
				CheckRightGroupForFitting(anchorGroup);
			if(alignment == ContextItemAlignment.NearTop || alignment == ContextItemAlignment.FarTop)
				CheckTopGroupForFitting(anchorGroup);
			if(alignment == ContextItemAlignment.NearBottom || alignment == ContextItemAlignment.FarBottom)
				CheckBottomGroupForFitting(anchorGroup);
		}  
		protected virtual void CalcLowerLeftItems() {
			LowerLeftViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> lowerLeftAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.BottomNear);
			int currentX = 0;
			for(int i = 0; i < lowerLeftAlignedViewInfos.Count; i++) {
				if(this.lowerLineFilled) {
					lowerLeftAlignedViewInfos[i].IsNotFittedWidth = true;
					continue;
				}
				Size itemSize = lowerLeftAlignedViewInfos[i].Size;				
				if(i == 0)
					lowerLeftAlignedViewInfos[i].Bounds = new Rectangle(new Point(BottomContentBounds.Left, BottomContentBounds.Bottom - itemSize.Height), itemSize);
				else
					lowerLeftAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX + Indent, BottomContentBounds.Bottom - itemSize.Height), itemSize);
				List<IAnchored> anchorGroup;
				currentX = CalcAnchorGroup(lowerLeftAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					LowerLeftViewInfos.Add((ContextItemViewInfo)item);
					if(item != lowerLeftAlignedViewInfos[i])
						lowerLeftAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}
		}
		protected virtual void CalcMiddleCenterItems() {
			List<ContextItemViewInfo> middleCenterViewInfos = GetViewInfosByAlignment(ContextItemAlignment.Center);
			if(middleCenterViewInfos.Count == 0) return;
			if(this.centerLineFilled) {
				MarkAsNotFitted(middleCenterViewInfos);
				return;
			}
			CalcHorizontalMiddleItems(middleCenterViewInfos);
		}   
		protected virtual void CalcCenterLeftItems() { 
			CenterLeftViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> centerLeftAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.CenterNear);
			int currentX = 0;
			for(int i = 0; i < centerLeftAlignedViewInfos.Count; i++) {
				if(this.centerLineFilled) {
					centerLeftAlignedViewInfos[i].IsNotFittedWidth = true;
					continue;
				}
				Size itemSize = centerLeftAlignedViewInfos[i].Size;
				if(i == 0)
					centerLeftAlignedViewInfos[i].Bounds = new Rectangle(new Point(CenterContentBounds.Left, CenterContentBounds.Top - itemSize.Height / 2), itemSize);
				else
					centerLeftAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX + Indent, CenterContentBounds.Top - itemSize.Height / 2), itemSize);
				List<IAnchored> anchorGroup;
				currentX = CalcAnchorGroup(centerLeftAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					CenterLeftViewInfos.Add((ContextItemViewInfo)item);
					if(item != centerLeftAlignedViewInfos[i])
						centerLeftAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}
		}
		protected virtual void CalcCenterRightItems() {
			CenterRightViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> centerRightAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.CenterFar);
			if(this.centerLineFilled) {
				MarkAsNotFitted(centerRightAlignedViewInfos);
				return;
			}
			int currentX = 0;
			for(int i = 0; i < centerRightAlignedViewInfos.Count; i++) {
				if(this.lowerLineFilled) {
					MarkAsNotFittedItem(centerRightAlignedViewInfos[i]);
					continue;
				}
				Size itemSize = centerRightAlignedViewInfos[i].Size;
				if(i == 0)
					centerRightAlignedViewInfos[i].Bounds = new Rectangle(new Point(CenterContentBounds.Right - itemSize.Width, CenterContentBounds.Top - itemSize.Height / 2), itemSize);
				else
					centerRightAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX - Indent - itemSize.Width, CenterContentBounds.Top - itemSize.Height / 2), itemSize);
				List<IAnchored> anchorGroup;
				currentX = CalcAnchorGroup(centerRightAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					CenterRightViewInfos.Add((ContextItemViewInfo)item);
					if(item != centerRightAlignedViewInfos[i])
						centerRightAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}
		}
		protected virtual void CalcNearCenterItems() {
			NearCenterViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> nearCenterViewInfos = GetViewInfosByAlignment(ContextItemAlignment.NearCenter);
			if(nearCenterViewInfos.Count == 0) return;
			if(this.nearLineFilled) {
				MarkAsNotFitted(nearCenterViewInfos);
				return;
			}
			CalcVerticalCenterItems(nearCenterViewInfos); 
		}
		protected virtual void CalcFarCenterItems() {
			FarCenterViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> farCenterViewInfos = GetViewInfosByAlignment(ContextItemAlignment.FarCenter);
			if(farCenterViewInfos.Count == 0) return;
			if(this.farLineFilled) {
				MarkAsNotFitted(farCenterViewInfos);
				return;
			}
			CalcVerticalCenterItems(farCenterViewInfos);
		}
		protected virtual void CalcVerticalCenterItems(List<ContextItemViewInfo> centerViewInfos) {
			CalcVerticalIndent(centerViewInfos);
			CalcVerticalCenterItemsCore(centerViewInfos, true); 
		}
		protected virtual void CalcVerticalIndent(List<ContextItemViewInfo> viewInfos) {
			Rectangle panelContentBounds = Rectangle.Empty;
			if(viewInfos[0].Item.Alignment == ContextItemAlignment.NearCenter) panelContentBounds = NearContentBounds;
			if(viewInfos[0].Item.Alignment == ContextItemAlignment.FarCenter) panelContentBounds = FarContentBounds;
			this.verticalIndent = (panelContentBounds.Height - CalcIndent(viewInfos)) / 2;
		}
		protected virtual void CalcVerticalCenterItemsCore(List<ContextItemViewInfo> centerAlignedViewInfos, bool fillItems) {
			if(centerAlignedViewInfos.Count == 0) return;
			if(centerAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.NearCenter) {
				if(fillItems) {
					NearCenterViewInfos = new List<ContextItemViewInfo>();
				}
				int currentY = 0;
				for(int i = 0; i < centerAlignedViewInfos.Count; i++) {
					Size btnSize = centerAlignedViewInfos[i].Size;
					if(i == 0)
						centerAlignedViewInfos[i].Bounds = new Rectangle(new Point(NearContentBounds.Left, NearContentBounds.Y + this.verticalIndent), btnSize);
					else
						centerAlignedViewInfos[i].Bounds = new Rectangle(new Point(NearContentBounds.Left, currentY + Indent), btnSize);
					List<IAnchored> anchorGroup;
					currentY = CalcAnchorGroup(centerAlignedViewInfos[i], out anchorGroup); 
					if(fillItems) {
						foreach(IAnchored item in anchorGroup) {
							NearCenterViewInfos.Add((ContextItemViewInfo)item);
							if(item != centerAlignedViewInfos[i])
								centerAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
						}
					}
					CheckMiddleGroupForFitting(anchorGroup);
				}
			}
			else if(centerAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.FarCenter) {
				if(fillItems) {
					FarCenterViewInfos = new List<ContextItemViewInfo>();
				}
				int currentY = 0;
				for(int i = 0; i < centerAlignedViewInfos.Count; i++) {
					Size btnSize = centerAlignedViewInfos[i].Size;
					if(i == 0)
						centerAlignedViewInfos[i].Bounds = new Rectangle(new Point(FarContentBounds.Right - btnSize.Width, FarContentBounds.Y + this.verticalIndent), btnSize);
					else
						centerAlignedViewInfos[i].Bounds = new Rectangle(new Point(FarContentBounds.Right - btnSize.Width, currentY + Indent), btnSize);
					List<IAnchored> anchorGroup;
					currentY = CalcAnchorGroup(centerAlignedViewInfos[i], out anchorGroup);
					if(fillItems) {
						foreach(IAnchored item in anchorGroup) {
							FarCenterViewInfos.Add((ContextItemViewInfo)item);
							if(item != centerAlignedViewInfos[i])
								centerAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
						}
					}
					CheckMiddleGroupForFitting(anchorGroup);
				}
			}
			CheckMiddleItemsForFitting(centerAlignedViewInfos);
		}
		protected virtual void CalcNearTopItems() {
			NearTopViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> nearTopAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.NearTop);
			int currentY = NearContentBounds.Y;
			for(int i = 0; i < nearTopAlignedViewInfos.Count; i++) {
				if(this.nearLineFilled) {
					MarkAsNotFittedItem(nearTopAlignedViewInfos[i]);
					continue;
				}
				Size itemSize = nearTopAlignedViewInfos[i].Size;
				int currentIndent = i == 0 ? 0 : Indent;
				nearTopAlignedViewInfos[i].Bounds = new Rectangle(new Point(NearContentBounds.Left, currentY + currentIndent), itemSize);
				List<IAnchored> anchorGroup;
				currentY = CalcAnchorGroup(nearTopAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					NearTopViewInfos.Add((ContextItemViewInfo)item);
					if(item != nearTopAlignedViewInfos[i])
						nearTopAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}
		}
		protected virtual void CalcNearBottomItems() { 
			NearBottomViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> nearBottomAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.NearBottom);
			if(this.nearLineFilled) {
				MarkAsNotFitted(nearBottomAlignedViewInfos);
				return;
			}
			int currentY = 0;
			for(int i = 0; i < nearBottomAlignedViewInfos.Count; i++) {
				if(this.nearLineFilled) {
					MarkAsNotFittedItem(nearBottomAlignedViewInfos[i]);
					continue;
				}
				Size itemSize = nearBottomAlignedViewInfos[i].Size;
				if(i == 0)
					nearBottomAlignedViewInfos[i].Bounds = new Rectangle(new Point(NearContentBounds.Left, NearContentBounds.Bottom - itemSize.Height), itemSize);
				else
					nearBottomAlignedViewInfos[i].Bounds = new Rectangle(new Point(NearContentBounds.Left, currentY - Indent - itemSize.Height), itemSize);
				List<IAnchored> anchorGroup;
				currentY = CalcAnchorGroup(nearBottomAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					NearBottomViewInfos.Add((ContextItemViewInfo)item);
					if(item != nearBottomAlignedViewInfos[i])
						nearBottomAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}	
		}
		protected virtual void CalcFarTopItems() {
			FarTopViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> farTopAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.FarTop);
			int currentY = 0;
			for(int i = 0; i < farTopAlignedViewInfos.Count; i++) {
				if(this.farLineFilled) {
					farTopAlignedViewInfos[i].IsNotFittedHeight = true;
					continue;
				}
				Size itemSize = farTopAlignedViewInfos[i].Size;
				if(i == 0)
					farTopAlignedViewInfos[i].Bounds = new Rectangle(new Point(FarContentBounds.Right - itemSize.Width, FarContentBounds.Top), itemSize);
				else
					farTopAlignedViewInfos[i].Bounds = new Rectangle(new Point(FarContentBounds.Right - itemSize.Width, currentY + Indent), itemSize);
				List<IAnchored> anchorGroup;
				currentY = CalcAnchorGroup(farTopAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					FarTopViewInfos.Add((ContextItemViewInfo)item);
					if(item != farTopAlignedViewInfos[i])
						farTopAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}
		}
		protected virtual void CalcFarBottomItems() {
			FarBottomViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> farBottomAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.FarBottom);
			if(this.farLineFilled) {
				MarkAsNotFitted(farBottomAlignedViewInfos);
				return;
			}
			int currentY = 0;
			for(int i = 0; i < farBottomAlignedViewInfos.Count; i++) {
				if(this.farLineFilled) {
					MarkAsNotFittedItem(farBottomAlignedViewInfos[i]);
					continue;
				}
				Size itemSize = farBottomAlignedViewInfos[i].Size;
				if(i == 0)
					farBottomAlignedViewInfos[i].Bounds = new Rectangle(new Point(FarContentBounds.Right - itemSize.Width, FarContentBounds.Bottom - itemSize.Height), itemSize);
				else
					farBottomAlignedViewInfos[i].Bounds = new Rectangle(new Point(FarContentBounds.Right - itemSize.Width, currentY - Indent - itemSize.Height), itemSize);
				List<IAnchored> anchorGroup;
				currentY = CalcAnchorGroup(farBottomAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					FarBottomViewInfos.Add((ContextItemViewInfo)item);
					if(item != farBottomAlignedViewInfos[i])
						farBottomAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}
		}
		protected virtual void MarkAsNotFittedItem(ContextItemViewInfo viewInfo) {
			bool isHorizontal = viewInfo.Item.Orientation == Orientation.Horizontal;
			if(isHorizontal) viewInfo.IsNotFittedWidth = true;
			else viewInfo.IsNotFittedHeight = true;
			List<IAnchored> anchoredItems = GetAnchored(viewInfo, new List<IAnchored>());
			foreach(IAnchored anchored in anchoredItems)
				if(isHorizontal) ((ContextItemViewInfo)anchored).IsNotFittedWidth = true;
				else ((ContextItemViewInfo)anchored).IsNotFittedHeight = true;
		}
		protected virtual void MarkAsNotFitted(List<ContextItemViewInfo> viewInfos) {
			foreach(ContextItemViewInfo viewinfo in viewInfos)
				MarkAsNotFittedItem(viewinfo);			
		}
		protected virtual bool IsMiddleItemsNotFitted(ContextItemAlignment align, List<ContextItemViewInfo> leftItems, List<ContextItemViewInfo> rightItems) {
			if(this.sideIndent < 0) return true;
			Rectangle panelContentBounds = Rectangle.Empty;
			if(align == ContextItemAlignment.MiddleTop) panelContentBounds = TopContentBounds;
			if(align == ContextItemAlignment.MiddleBottom) panelContentBounds = BottomContentBounds;
			if(align == ContextItemAlignment.Center) panelContentBounds = CenterContentBounds;
			if(leftItems.Count > 0) {
				int leftMaxX = GetLeftMaxX(leftItems[0].Item.Alignment);
				if(panelContentBounds.Left + sideIndent < leftMaxX + Indent)
					return true;
			}
			if(rightItems.Count > 0) {
				int rightMinX = GetRightMinX(rightItems[0].Item.Alignment);
				if(panelContentBounds.Right - sideIndent > rightMinX - Indent)
					return true;
			}
			return false;
		}
		protected virtual bool IsCenterItemsNotFitted(ContextItemAlignment align, List<ContextItemViewInfo> topItems, List<ContextItemViewInfo> bottomItems) {
			if(this.verticalIndent < 0) return true;
			Rectangle panelContentBounds = Rectangle.Empty;
			if(align == ContextItemAlignment.NearCenter) panelContentBounds = NearContentBounds;
			if(align == ContextItemAlignment.FarCenter) panelContentBounds = FarContentBounds;
			if(topItems.Count > 0) {
				int topMaxY = GetTopMaxY(topItems[0].Item.Alignment);
				if(panelContentBounds.Top + verticalIndent < topMaxY + Indent)
					return true;
			}
			if(bottomItems.Count > 0) {
				int bottomMinY = GetBottomMinY(bottomItems[0].Item.Alignment);
				if(panelContentBounds.Bottom - verticalIndent > bottomMinY - Indent)
					return true;
			}
			return false;
		}
		protected virtual void CheckMiddleItemsForFitting(List<ContextItemViewInfo> middleAlignedViewInfos) {			
			if(middleAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.MiddleTop) {
				if(!IsMiddleItemsNotFitted( ContextItemAlignment.MiddleTop, UpperLeftViewInfos, UpperRightViewInfos)) return;
				if(!MarkAsNotFittedLastMiddleItem(MiddleTopViewInfos, true)) return;				
			}
			if(middleAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.MiddleBottom) {
				if(!IsMiddleItemsNotFitted(ContextItemAlignment.MiddleBottom, LowerLeftViewInfos, LowerRightViewInfos)) return;
				if(!MarkAsNotFittedLastMiddleItem(MiddleBottomViewInfos, true)) return;							 
			}
			if(middleAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.Center) {
				if(!IsMiddleItemsNotFitted(ContextItemAlignment.Center, CenterLeftViewInfos, CenterRightViewInfos)) return;
				if(!MarkAsNotFittedLastMiddleItem(MiddleCenterViewInfos, true)) return;
			}
			if(middleAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.NearCenter) {
				if(!IsCenterItemsNotFitted(ContextItemAlignment.NearCenter, NearTopViewInfos, NearBottomViewInfos)) return;
				if(!MarkAsNotFittedLastMiddleItem(NearCenterViewInfos, false)) return;
			}
			if(middleAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.FarCenter) {
				if(!IsCenterItemsNotFitted(ContextItemAlignment.FarCenter, FarTopViewInfos, FarBottomViewInfos)) return;
				if(!MarkAsNotFittedLastMiddleItem(FarCenterViewInfos, false)) return;
			}
			RecalcMiddleItems(middleAlignedViewInfos);
		}
		protected virtual void CalcMiddleTopItems() {
			MiddleTopViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> middleTopAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.MiddleTop);
			if(middleTopAlignedViewInfos.Count == 0) return;
			if(this.upperLineFilled) {
				MarkAsNotFitted(middleTopAlignedViewInfos);
				return;
			}
			CalcHorizontalMiddleItems(middleTopAlignedViewInfos);
		}
		protected virtual void CalcHorizontalMiddleItems(List<ContextItemViewInfo> middleViewInfos) {	   
			CalcSideIndent(middleViewInfos);
			CalcHorizontalMiddleItemsCore(middleViewInfos, true);
		}
		protected virtual void CalcHorizontalMiddleItemsCore(List<ContextItemViewInfo> middleAlignedViewInfos, bool fillItems) {
			if(middleAlignedViewInfos.Count == 0) return;
			if(middleAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.MiddleTop) {
				if(fillItems) {
				   MiddleTopViewInfos = new List<ContextItemViewInfo>();
				}
				int currentX = 0;
				for(int i = 0; i < middleAlignedViewInfos.Count; i++) {
					Size btnSize = middleAlignedViewInfos[i].Size;
					if(i == 0)
						middleAlignedViewInfos[i].Bounds = new Rectangle(new Point(TopContentBounds.X + this.sideIndent, TopContentBounds.Top), btnSize);
					else
						middleAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX + Indent, TopContentBounds.Top), btnSize);
						List<IAnchored> anchorGroup;
						currentX = CalcAnchorGroup(middleAlignedViewInfos[i], out anchorGroup);
						if(fillItems) {
							foreach(IAnchored item in anchorGroup) {
								MiddleTopViewInfos.Add((ContextItemViewInfo)item);
								if(item != middleAlignedViewInfos[i])
									middleAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
							}
						}
						CheckMiddleGroupForFitting(anchorGroup);				   
				}
			}
			else if(middleAlignedViewInfos[0].Item.Alignment == ContextItemAlignment.MiddleBottom) {
				if(fillItems) {
					MiddleBottomViewInfos = new List<ContextItemViewInfo>();
				}
				int currentX = 0;
				for(int i = 0; i < middleAlignedViewInfos.Count; i++) {
					Size btnSize = middleAlignedViewInfos[i].Size;
					if(i == 0)
						middleAlignedViewInfos[i].Bounds = new Rectangle(new Point(BottomContentBounds.X + this.sideIndent, BottomContentBounds.Bottom - btnSize.Height), btnSize);
					else
						middleAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX + Indent, BottomContentBounds.Bottom - btnSize.Height), btnSize);
					List<IAnchored> anchorGroup;
					currentX = CalcAnchorGroup(middleAlignedViewInfos[i], out anchorGroup);
					if(fillItems) {
						foreach(IAnchored item in anchorGroup) {
							MiddleBottomViewInfos.Add((ContextItemViewInfo)item);
							if(item != middleAlignedViewInfos[i])
								middleAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
						}
					}
					CheckMiddleGroupForFitting(anchorGroup);
				}
			}
			else {
				if(fillItems) {
					MiddleCenterViewInfos = new List<ContextItemViewInfo>();
				}
				int currentX = 0;
				for(int i = 0; i < middleAlignedViewInfos.Count; i++) {
					Size btnSize = middleAlignedViewInfos[i].Size;
					if(i == 0)
						middleAlignedViewInfos[i].Bounds = new Rectangle(new Point(CenterContentBounds.X + this.sideIndent, CenterContentBounds.Top - btnSize.Height / 2), btnSize);
					else
						middleAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX + Indent, CenterContentBounds.Top - btnSize.Height / 2), btnSize);
					List<IAnchored> anchorGroup;
					currentX = CalcAnchorGroup(middleAlignedViewInfos[i], out anchorGroup);
					if(fillItems) {
						foreach(IAnchored item in anchorGroup) {
							MiddleCenterViewInfos.Add((ContextItemViewInfo)item);
							if(item != middleAlignedViewInfos[i])
								middleAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
						}
					}
					CheckMiddleGroupForFitting(anchorGroup);
				}
			}
			CheckMiddleItemsForFitting(middleAlignedViewInfos);		  
		}
		protected virtual void CheckMiddleGroupForFitting(List<IAnchored> anchorGroup) {
			for(int j = 0; j < anchorGroup.Count; j++) {
				if(j == 0)
					CheckAlignedItemForFitting(anchorGroup[0] as ContextItemViewInfo);
				else
					CheckAnchoredItemForFitting(anchorGroup[j] as ContextItemViewInfo);
			}
		}
		protected virtual bool MarkAsNotFittedLastMiddleItem(List<ContextItemViewInfo> middleItems, bool isHorizontal) {
			int max = isHorizontal ? GetMaxX(middleItems) : GetMaxY(middleItems);
			if(max == -1) return false;
			List<ContextItemViewInfo> itemsWithMax = isHorizontal ? GetItemsWithMaxX(middleItems, max) : GetItemsWithMaxY(middleItems, max);
			foreach(ContextItemViewInfo viewInfo in itemsWithMax)
				if(isHorizontal) viewInfo.IsNotFittedWidth = true;
				else viewInfo.IsNotFittedHeight = true;
			return true;
		}
		protected virtual void RecalcMiddleItems(List<ContextItemViewInfo> middleAlignedViewInfos) {
			if(middleAlignedViewInfos[0].Item.Orientation == Orientation.Horizontal) {
				CalcSideIndent(middleAlignedViewInfos);
				CalcHorizontalMiddleItemsCore(middleAlignedViewInfos, false);
			}
			else {
				CalcVerticalIndent(middleAlignedViewInfos);
				CalcVerticalCenterItemsCore(middleAlignedViewInfos, false);
			}
		}
		protected virtual List<ContextItemViewInfo> GetItemsWithMaxX(List<ContextItemViewInfo> viewInfos, int maxX) {
			List<ContextItemViewInfo> result = new List<ContextItemViewInfo>();
			foreach(ContextItemViewInfo viewInfo in viewInfos) {
				if(viewInfo.Bounds.Right == maxX && !viewInfo.IsNotFittedWidth)
					result.Add(viewInfo);
			}
			return result;
		}
		protected virtual List<ContextItemViewInfo> GetItemsWithMaxY(List<ContextItemViewInfo> viewInfos, int maxY) {
			List<ContextItemViewInfo> result = new List<ContextItemViewInfo>();
			foreach(ContextItemViewInfo viewInfo in viewInfos) {
				if(viewInfo.Bounds.Bottom == maxY && !viewInfo.IsNotFittedHeight)
					result.Add(viewInfo);
			}
			return result;
		}
		protected virtual void MarkLineFilled(ContextItemViewInfo viewInfo) {
			if(viewInfo.Item.Alignment == ContextItemAlignment.TopFar || viewInfo.Item.Alignment == ContextItemAlignment.TopNear)
				this.upperLineFilled = true;
			else if(viewInfo.Item.Alignment == ContextItemAlignment.BottomFar || viewInfo.Item.Alignment == ContextItemAlignment.BottomNear)
				this.lowerLineFilled = true;
			else if(viewInfo.Item.Alignment == ContextItemAlignment.CenterFar || viewInfo.Item.Alignment == ContextItemAlignment.CenterNear)
				this.centerLineFilled = true;
			else if(viewInfo.Item.Alignment == ContextItemAlignment.NearTop || viewInfo.Item.Alignment == ContextItemAlignment.NearBottom)
				this.nearLineFilled = true;
			else
				this.farLineFilled = true;
		}
		protected virtual void CheckRightAlignedItemWidthForFitting(ContextItemViewInfo viewInfo, int leftMaxX) {
			if(leftMaxX > 0) {
				if(viewInfo.Bounds.Left - Indent <= leftMaxX) {
					MarkLineFilled(viewInfo);
					if(viewInfo.Bounds.Left - Indent < leftMaxX)
						viewInfo.IsNotFittedWidth = true;
					return;
				}
			}
			int left = GetPanelContentBounds(viewInfo).Left;
			if(viewInfo.Bounds.Left <= left) {
				MarkLineFilled(viewInfo);
				if(viewInfo.Bounds.Left < left)
					viewInfo.IsNotFittedWidth = true;
				return;
			}	   
		}
		protected virtual void CheckRightAlignedItemForFitting(ContextItemViewInfo viewInfo, int leftMaxX) {
			CheckRightAlignedItemWidthForFitting(viewInfo, leftMaxX);
			if(viewInfo.IsNotFittedWidth) return;
			CheckAlignedItemForFitting(viewInfo);		  
		}
		protected virtual void CalcUpperRightItems() {
			UpperRightViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> upperRightAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.TopFar);
			if(this.upperLineFilled) {				
				MarkAsNotFitted(upperRightAlignedViewInfos);
				return;
			}
			int currentX = 0;
			for(int i = 0; i < upperRightAlignedViewInfos.Count; i++) {
				if(this.upperLineFilled) {
					MarkAsNotFittedItem(upperRightAlignedViewInfos[i]);
					continue;
				}
				Size itemSize = upperRightAlignedViewInfos[i].Size;
				if(i == 0)
					upperRightAlignedViewInfos[i].Bounds = new Rectangle(new Point(TopContentBounds.Right - itemSize.Width, TopContentBounds.Top), itemSize);
				else
					upperRightAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX - Indent - itemSize.Width, TopContentBounds.Top), itemSize);
				List<IAnchored> anchorGroup;
				currentX = CalcAnchorGroup(upperRightAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					UpperRightViewInfos.Add((ContextItemViewInfo)item);
					if(item != upperRightAlignedViewInfos[i])
						upperRightAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);
			}	
		}		
		protected virtual void CheckRightGroupForFitting(List<IAnchored> anchorGroup) {
			ContextItemViewInfo aligned = anchorGroup[0] as ContextItemViewInfo;
			ContextItemAlignment leftAlignment;
			if(aligned.Item.Alignment == ContextItemAlignment.TopFar)
				leftAlignment = ContextItemAlignment.TopNear;
			else if(aligned.Item.Alignment == ContextItemAlignment.BottomFar)
					leftAlignment = ContextItemAlignment.BottomNear;			 
			else { 
				if(aligned.Item.Alignment != ContextItemAlignment.CenterFar)
					return;
				leftAlignment = ContextItemAlignment.CenterNear;
			}
			int maxX = GetLeftMaxX(leftAlignment);
			for(int j = 0; j < anchorGroup.Count; j++) {
				if(j == 0)
					CheckRightAlignedItemForFitting(aligned, maxX);
				else
					CheckRightAnchoredItemForFitting(aligned, anchorGroup[j] as ContextItemViewInfo, maxX);
			}
		}
		protected virtual void CheckRightAnchoredItemForFitting(ContextItemViewInfo alignedViewInfo, ContextItemViewInfo anchoredViewInfo, int leftMaxX) {
			CheckRightAnchoredItemWidthForFitting(alignedViewInfo, anchoredViewInfo, leftMaxX);
			if(anchoredViewInfo.IsNotFittedWidth) return;
			CheckAnchoredItemForFitting(anchoredViewInfo); 
		}	   
		protected virtual void CheckRightAnchoredItemWidthForFitting(ContextItemViewInfo alignedViewInfo, ContextItemViewInfo anchoredViewInfo, int leftMaxX) {
			if(leftMaxX > 0) {
				if(anchoredViewInfo.Bounds.Left - Indent <= leftMaxX) {
					if(alignedViewInfo.Item.Alignment == ContextItemAlignment.TopFar) {
						if(anchoredViewInfo.Bounds.Top == alignedViewInfo.Bounds.Top)
							this.upperLineFilled = true;
					}
					else if(alignedViewInfo.Item.Alignment == ContextItemAlignment.BottomFar) {
						if(anchoredViewInfo.Bounds.Bottom == alignedViewInfo.Bounds.Bottom)
							this.lowerLineFilled = true;
					}
					else
						if(anchoredViewInfo.Bounds.Top == alignedViewInfo.Bounds.Top && anchoredViewInfo.Bounds.Bottom == alignedViewInfo.Bounds.Bottom) 
							this.centerLineFilled = true;
					if(anchoredViewInfo.Bounds.Left - Indent < leftMaxX)
						anchoredViewInfo.IsNotFittedWidth = true;
					return;
				}				
			}
			int left = GetPanelContentBounds(anchoredViewInfo).Left;
			if(anchoredViewInfo.Bounds.Left <= left) {
				if(alignedViewInfo.Item.Alignment == ContextItemAlignment.TopFar) {
					if(anchoredViewInfo.Bounds.Top == alignedViewInfo.Bounds.Top)
						this.upperLineFilled = true;
				}
				else if(alignedViewInfo.Item.Alignment == ContextItemAlignment.BottomFar) {
					if(anchoredViewInfo.Bounds.Bottom == alignedViewInfo.Bounds.Bottom)
						this.lowerLineFilled = true;
				}
				else
					if(anchoredViewInfo.Bounds.Top == alignedViewInfo.Bounds.Top && anchoredViewInfo.Bounds.Bottom == alignedViewInfo.Bounds.Bottom) 
						this.centerLineFilled = true;
				if(anchoredViewInfo.Bounds.Left < left)
					anchoredViewInfo.IsNotFittedWidth = true;
				return;
			}
		}
		protected virtual void CheckBottomGroupForFitting(List<IAnchored> anchorGroup) {
			ContextItemViewInfo aligned = anchorGroup[0] as ContextItemViewInfo;
			ContextItemAlignment topAlignment;
			if(aligned.Item.Alignment == ContextItemAlignment.NearBottom)
				topAlignment = ContextItemAlignment.NearTop;
			else {
				if(aligned.Item.Alignment != ContextItemAlignment.FarBottom)
					return;
				topAlignment = ContextItemAlignment.FarTop;
			}
			int maxY = GetTopMaxY(topAlignment);
			for(int j = 0; j < anchorGroup.Count; j++) {
				if(j == 0)
					CheckBottomAlignedItemForFitting(aligned, maxY);
				else
					CheckBottomAnchoredItemForFitting(aligned, anchorGroup[j] as ContextItemViewInfo, maxY);
			}
		}
		protected virtual void CheckBottomAlignedItemForFitting(ContextItemViewInfo viewInfo, int topMaxY) {
			CheckBottomAlignedItemHeightForFitting(viewInfo, topMaxY);
			if(viewInfo.IsNotFittedHeight) return;
			CheckAlignedItemForFitting(viewInfo);
		}
		protected virtual void CheckBottomAlignedItemHeightForFitting(ContextItemViewInfo viewInfo, int topMaxY) { 
			if(topMaxY > 0) {
				if(viewInfo.Bounds.Top - Indent <= topMaxY) {
					MarkLineFilled(viewInfo);
					if(viewInfo.Bounds.Top - Indent < topMaxY)
						viewInfo.IsNotFittedHeight = true;
					return;
				}
			}
			int top = GetPanelContentBounds(viewInfo).Top;
			if(viewInfo.Bounds.Top <= top) {
				MarkLineFilled(viewInfo);
				if(viewInfo.Bounds.Top < top)
					viewInfo.IsNotFittedHeight = true;
				return;
			}
		}
		protected virtual void CheckBottomAnchoredItemForFitting(ContextItemViewInfo alignedViewInfo, ContextItemViewInfo anchoredViewInfo, int topMaxY) {
			CheckBottomAnchoredItemHeightForFitting(alignedViewInfo, anchoredViewInfo, topMaxY);
			if(anchoredViewInfo.IsNotFittedHeight) return;
			CheckAnchoredItemForFitting(anchoredViewInfo);
		}
		protected virtual void CheckBottomAnchoredItemHeightForFitting(ContextItemViewInfo alignedViewInfo, ContextItemViewInfo anchoredViewInfo, int topMaxY) {
			if(topMaxY > 0) {
				if(anchoredViewInfo.Bounds.Top - Indent <= topMaxY) {
					if(alignedViewInfo.Item.Alignment == ContextItemAlignment.NearBottom) {
						if(anchoredViewInfo.Bounds.Left == alignedViewInfo.Bounds.Left)
							this.nearLineFilled = true;
					}
					else if(alignedViewInfo.Item.Alignment == ContextItemAlignment.FarBottom) {
						if(anchoredViewInfo.Bounds.Right == alignedViewInfo.Bounds.Right)
							this.farLineFilled = true;
					}
					if(anchoredViewInfo.Bounds.Top - Indent < topMaxY)
						anchoredViewInfo.IsNotFittedHeight = true;
					return;
				}
			}
			int top = GetPanelContentBounds(anchoredViewInfo).Top;
			if(anchoredViewInfo.Bounds.Top <= top) {
				if(alignedViewInfo.Item.Alignment == ContextItemAlignment.NearBottom) {
					if(anchoredViewInfo.Bounds.Left == alignedViewInfo.Bounds.Left)
						this.nearLineFilled = true;
				}
				else if(alignedViewInfo.Item.Alignment == ContextItemAlignment.FarBottom) {
					if(anchoredViewInfo.Bounds.Right == alignedViewInfo.Bounds.Right)
						this.farLineFilled = true;
				}
				if(anchoredViewInfo.Bounds.Top < top)
					anchoredViewInfo.IsNotFittedHeight = true;
				return;
			}
		}
		protected virtual void CheckAnchoredItemForFitting(ContextItemViewInfo anchoredViewInfo) {
			if(anchoredViewInfo.Item.Orientation == Orientation.Horizontal) {
				if(anchoredViewInfo.Bounds.Top < TopContentBounds.Top)
					anchoredViewInfo.IsNotFittedHeight = true;
				if(anchoredViewInfo.Bounds.Bottom > BottomContentBounds.Bottom)
					anchoredViewInfo.IsNotFittedHeight = true;
			}
			else {
				if(anchoredViewInfo.Bounds.Left < NearContentBounds.Left)
					anchoredViewInfo.IsNotFittedWidth = true;
				if(anchoredViewInfo.Bounds.Right > FarContentBounds.Right)
					anchoredViewInfo.IsNotFittedWidth = true;
			}
		}
		protected virtual void CheckLeftAnchoredItemWidthForFitting(ContextItemViewInfo alignedViewInfo, ContextItemViewInfo anchoredViewInfo) { 
			int right = GetPanelContentBounds(anchoredViewInfo).Right;
			if(anchoredViewInfo.Bounds.Right >= right) {
				if(alignedViewInfo.Item.Alignment == ContextItemAlignment.TopNear) {
					if(alignedViewInfo.Bounds.Top == anchoredViewInfo.Bounds.Top) 
						this.upperLineFilled = true;					
				}
				else if(alignedViewInfo.Item.Alignment == ContextItemAlignment.BottomNear) {
					if(anchoredViewInfo.Bounds.Bottom == alignedViewInfo.Bounds.Bottom)
						this.lowerLineFilled = true;
				}
				else
					if(anchoredViewInfo.Bounds.Top == alignedViewInfo.Bounds.Top && anchoredViewInfo.Bounds.Bottom == alignedViewInfo.Bounds.Bottom) 
						this.centerLineFilled = true;
				if(anchoredViewInfo.Bounds.Right > right)
					anchoredViewInfo.IsNotFittedWidth = true;
			}
		}
		protected virtual void CheckLeftAnchoredItemForFitting(ContextItemViewInfo alignedViewInfo, ContextItemViewInfo anchoredViewInfo) {
			CheckLeftAnchoredItemWidthForFitting(alignedViewInfo, anchoredViewInfo);
			if(anchoredViewInfo.IsNotFittedWidth) return;
			CheckAnchoredItemForFitting(anchoredViewInfo);		  
		}
		protected virtual void CheckLeftGroupForFitting(List<IAnchored> anchorGroup) {
			for(int j = 0; j < anchorGroup.Count; j++) {
				if(j == 0)
					CheckLeftAlignedItemForFitting(anchorGroup[j] as ContextItemViewInfo);
				else
					CheckLeftAnchoredItemForFitting(anchorGroup[0] as ContextItemViewInfo, anchorGroup[j] as ContextItemViewInfo);
			}
		}
		protected virtual void CheckLeftAlignedItemForFitting(ContextItemViewInfo viewInfo) {
			CheckLeftAlignedItemWidthForFitting(viewInfo);
			if(viewInfo.IsNotFittedWidth) return;
			CheckAlignedItemForFitting(viewInfo);				   
		}
		protected virtual void CheckTopGroupForFitting(List<IAnchored> anchorGroup) {
			for(int j = 0; j < anchorGroup.Count; j++) {
				if(j == 0)
					CheckTopAlignedItemForFitting(anchorGroup[j] as ContextItemViewInfo);
				else
					CheckTopAnchoredItemForFitting(anchorGroup[0] as ContextItemViewInfo, anchorGroup[j] as ContextItemViewInfo);
			}
		}
		protected virtual void CheckTopAlignedItemForFitting(ContextItemViewInfo viewInfo) {
			CheckTopAlignedItemHeightForFitting(viewInfo);
			if(viewInfo.IsNotFittedHeight) return;
			CheckAlignedItemForFitting(viewInfo);
		}
		protected virtual void CheckTopAnchoredItemForFitting(ContextItemViewInfo alignedViewInfo, ContextItemViewInfo anchoredViewInfo) {
			CheckTopAnchoredItemHeightForFitting(alignedViewInfo, anchoredViewInfo);
			if(anchoredViewInfo.IsNotFittedHeight) return;
			CheckAnchoredItemForFitting(anchoredViewInfo);
		}
		protected virtual void CheckTopAlignedItemHeightForFitting(ContextItemViewInfo viewInfo) { 
			int bottom = GetPanelContentBounds(viewInfo).Bottom;
			if(viewInfo.Bounds.Bottom >= bottom) {
				MarkLineFilled(viewInfo);
			}
			if(viewInfo.Bounds.Bottom > bottom) {
				viewInfo.IsNotFittedHeight = true;
			}
		}
		protected virtual void CheckTopAnchoredItemHeightForFitting(ContextItemViewInfo alignedViewInfo, ContextItemViewInfo anchoredViewInfo) { 
			int bottom = GetPanelContentBounds(anchoredViewInfo).Bottom;
			if(anchoredViewInfo.Bounds.Bottom >= bottom) {
				if(alignedViewInfo.Item.Alignment == ContextItemAlignment.NearTop) {
					if(alignedViewInfo.Bounds.Left == anchoredViewInfo.Bounds.Left)
						this.nearLineFilled = true;
				}
				else if(alignedViewInfo.Item.Alignment == ContextItemAlignment.FarTop) {
					if(anchoredViewInfo.Bounds.Right == alignedViewInfo.Bounds.Right)
						this.farLineFilled = true;
				}
				if(anchoredViewInfo.Bounds.Bottom > bottom)
					anchoredViewInfo.IsNotFittedHeight = true;
			}
		}
		protected virtual Rectangle GetPanelContentBounds(ContextItemViewInfo viewInfo) {
			if(UpperLeftViewInfos.Contains(viewInfo) || MiddleTopViewInfos.Contains(viewInfo) || UpperRightViewInfos.Contains(viewInfo))
				return TopContentBounds;
			else if(LowerLeftViewInfos.Contains(viewInfo) || MiddleBottomViewInfos.Contains(viewInfo) || LowerRightViewInfos.Contains(viewInfo))
				return BottomContentBounds;
			else if(CenterLeftViewInfos.Contains(viewInfo) || MiddleCenterViewInfos.Contains(viewInfo) || CenterRightViewInfos.Contains(viewInfo))
				return CenterContentBounds;
			else if(NearTopViewInfos.Contains(viewInfo) || NearCenterViewInfos.Contains(viewInfo) || NearBottomViewInfos.Contains(viewInfo))
				return NearContentBounds;
			else return FarContentBounds;
		}
		protected virtual void CheckLeftAlignedItemWidthForFitting(ContextItemViewInfo viewInfo) { 
			int right = GetPanelContentBounds(viewInfo).Right;
			if(viewInfo.Bounds.Right >= right) {
				MarkLineFilled(viewInfo);
			}
			if(viewInfo.Bounds.Right > right) {
				viewInfo.IsNotFittedWidth = true;
			}					   
		}
		protected virtual void CheckAlignedItemForFitting(ContextItemViewInfo viewInfo) {
			if(viewInfo.Item.Alignment == ContextItemAlignment.TopNear || viewInfo.Item.Alignment == ContextItemAlignment.TopFar || viewInfo.Item.Alignment == ContextItemAlignment.MiddleTop) {
				if(viewInfo.Bounds.Bottom > BottomContentBounds.Bottom)
					viewInfo.IsNotFittedHeight = true;						 
			}
			if(viewInfo.Item.Alignment == ContextItemAlignment.BottomNear || viewInfo.Item.Alignment == ContextItemAlignment.BottomFar || viewInfo.Item.Alignment == ContextItemAlignment.MiddleBottom) {
				if(viewInfo.Bounds.Top < TopContentBounds.Top) 
					viewInfo.IsNotFittedHeight = true;							
			}
			if(viewInfo.Item.Alignment == ContextItemAlignment.CenterNear || viewInfo.Item.Alignment == ContextItemAlignment.CenterFar || viewInfo.Item.Alignment == ContextItemAlignment.Center) {
				if(viewInfo.Bounds.Bottom > BottomContentBounds.Top || viewInfo.Bounds.Top < TopContentBounds.Bottom)
					viewInfo.IsNotFittedHeight = true;
			}
			if(viewInfo.Item.Alignment == ContextItemAlignment.NearTop || viewInfo.Item.Alignment == ContextItemAlignment.NearCenter || viewInfo.Item.Alignment == ContextItemAlignment.NearBottom) {
				if(viewInfo.Bounds.Right > FarContentBounds.Right)
					viewInfo.IsNotFittedWidth = true;
			}
			if(viewInfo.Item.Alignment == ContextItemAlignment.FarTop || viewInfo.Item.Alignment == ContextItemAlignment.FarCenter || viewInfo.Item.Alignment == ContextItemAlignment.FarBottom) {
				if(viewInfo.Bounds.Left < NearContentBounds.Left)
					viewInfo.IsNotFittedWidth = true;
			}
		}
		protected int CalcLeftGroupMinX(List<IAnchored> group) {
			int minX = 0;
			for(int k = 0; k < group.Count; k++) {
				int x = ((ContextItemViewInfo)group[k]).Bounds.X;
				if(k == 0)
					minX = x;
				else {
					if(x < minX)
						minX = x;
				}
			}
			return minX;
		}
		protected int CalcRightGroupMaxX(List<IAnchored> group) {
			int maxX = 0;
			for(int k = 0; k < group.Count; k++) {
				int x = ((ContextItemViewInfo)group[k]).Bounds.X;
				if(k == 0)
					maxX = x;
				else {
					if(x > maxX)
						maxX = x;
				}
			}
			return maxX;
		}
		protected int CalcTopGroupMinY(List<IAnchored> group) { 
			int minY = 0;
			for(int k = 0; k < group.Count; k++) {
				int y = ((ContextItemViewInfo)group[k]).Bounds.Y;
				if(k == 0)
					minY = y;
				else {
					if(y < minY)
						minY = y;
				}
			}
			return minY;
		}
		protected int CalcBottomGroupMaxY(List<IAnchored> group) {
			int maxY = 0;
			for(int k = 0; k < group.Count; k++) {
				int y = ((ContextItemViewInfo)group[k]).Bounds.Y;
				if(k == 0)
					maxY = y;
				else {
					if(y > maxY)
						maxY = y;
				}
			}
			return maxY;
		}
		protected virtual void AdjustAnchorGroupLocation(List<IAnchored> group) { 
			if(group.Count == 1) return;
			ContextItemViewInfo aligned = group[0] as ContextItemViewInfo;
			if(aligned.Item.Alignment == ContextItemAlignment.TopNear || aligned.Item.Alignment == ContextItemAlignment.BottomNear || 
				aligned.Item.Alignment == ContextItemAlignment.MiddleTop || aligned.Item.Alignment == ContextItemAlignment.MiddleBottom ||
				aligned.Item.Alignment == ContextItemAlignment.CenterNear || aligned.Item.Alignment == ContextItemAlignment.Center) {
				int minX = CalcLeftGroupMinX(group);				 
				int delta = aligned.Bounds.X - minX;
				for(int m = 0; m < group.Count; m++) {
					ContextItemViewInfo viewInfo = ((ContextItemViewInfo)group[m]);
					viewInfo.Bounds = new Rectangle(new Point(viewInfo.Bounds.X + delta, viewInfo.Bounds.Y), viewInfo.Bounds.Size);
				}
			}
			if(aligned.Item.Alignment == ContextItemAlignment.TopFar || aligned.Item.Alignment == ContextItemAlignment.BottomFar || aligned.Item.Alignment == ContextItemAlignment.CenterFar) {
				int maxX = CalcRightGroupMaxX(group);				
				int delta = maxX - aligned.Bounds.X;
				for(int m = 0; m < group.Count; m++) {
					ContextItemViewInfo viewInfo = ((ContextItemViewInfo)group[m]);
					viewInfo.Bounds = new Rectangle(new Point(viewInfo.Bounds.X - delta, viewInfo.Bounds.Y), viewInfo.Bounds.Size);
				}
			}
			if(aligned.Item.Alignment == ContextItemAlignment.NearTop || aligned.Item.Alignment == ContextItemAlignment.NearCenter ||
				aligned.Item.Alignment == ContextItemAlignment.FarTop || aligned.Item.Alignment == ContextItemAlignment.FarCenter) {
				int minY = CalcTopGroupMinY(group);
				int delta = aligned.Bounds.Y - minY;
				for(int m = 0; m < group.Count; m++) {
					ContextItemViewInfo viewInfo = ((ContextItemViewInfo)group[m]);
					viewInfo.Bounds = new Rectangle(new Point(viewInfo.Bounds.X, viewInfo.Bounds.Y + delta), viewInfo.Bounds.Size);
				}
			}
			if(aligned.Item.Alignment == ContextItemAlignment.NearBottom || aligned.Item.Alignment == ContextItemAlignment.FarBottom) {
				int maxY = CalcBottomGroupMaxY(group);
				int delta = maxY - aligned.Bounds.Y;
				for(int m = 0; m < group.Count; m++) {
					ContextItemViewInfo viewInfo = ((ContextItemViewInfo)group[m]);
					viewInfo.Bounds = new Rectangle(new Point(viewInfo.Bounds.X, viewInfo.Bounds.Y - delta), viewInfo.Bounds.Size);
				}
			}
		}
		protected virtual int CalcCurrentXForRightItems(List<IAnchored> list) { 
			ContextItemViewInfo aligned = ((ContextItemViewInfo)list[0]);
			Rectangle alignedBounds = ((ContextItemViewInfo)list[0]).Bounds;
			int curX = -1;
			for(int k = 0; k < list.Count; k++) {
				Rectangle itemRect = ((ContextItemViewInfo)list[k]).Bounds;
				if(itemRect.Top != alignedBounds.Top) continue;			   
				int x = itemRect.Left;
				if(k == 0)
					curX = x;
				else {
					if(x < curX)
						curX = x;
				}
			}			
			return curX; 
		}
		protected virtual int CalcCurrentXForLeftItems(List<IAnchored> list) {
			ContextItemViewInfo aligned = ((ContextItemViewInfo)list[0]);
			Rectangle alignedBounds = ((ContextItemViewInfo)list[0]).Bounds;		   
			int curX = -1;
			for(int k = 0; k < list.Count; k++) {
				Rectangle itemRect = ((ContextItemViewInfo)list[k]).Bounds;
				if(itemRect.Top != alignedBounds.Top) continue;
				int x = itemRect.Right;
				if(k == 0)
					curX = x;
				else {
					if(x > curX)
						curX = x;
				}
			}
			return curX;	  
		}
		protected virtual int CalcCurrentYForTopItems(List<IAnchored> list) {
			ContextItemViewInfo aligned = ((ContextItemViewInfo)list[0]);
			Rectangle alignedBounds = ((ContextItemViewInfo)list[0]).Bounds;
			int curY = -1;
			for(int k = 0; k < list.Count; k++) {
				Rectangle itemRect = ((ContextItemViewInfo)list[k]).Bounds;
				if(itemRect.Left != alignedBounds.Left) continue;
				int y = itemRect.Top;
				if(k == 0)
					curY = y;
				else {
					if(y < curY)
						curY = y;
				}
			}
			return curY;
		}
		protected virtual int CalcCurrentYForBottomItems(List<IAnchored> list) {
			ContextItemViewInfo aligned = ((ContextItemViewInfo)list[0]);
			Rectangle alignedBounds = ((ContextItemViewInfo)list[0]).Bounds;
			int curY = -1;
			for(int k = 0; k < list.Count; k++) {
				Rectangle itemRect = ((ContextItemViewInfo)list[k]).Bounds;
				if(itemRect.Left != alignedBounds.Left) continue;
				int y = itemRect.Bottom;
				if(k == 0)
					curY = y;
				else {
					if(y > curY)
						curY = y;
				}
			}
			return curY;
		}
		protected virtual int CalcCurrentPosition(List<IAnchored> list) {
			ContextItemViewInfo aligned = ((ContextItemViewInfo)list[0]);
			if(aligned.Item.Alignment == ContextItemAlignment.TopNear || aligned.Item.Alignment == ContextItemAlignment.BottomNear || aligned.Item.Alignment == ContextItemAlignment.CenterNear || 
				 aligned.Item.Alignment == ContextItemAlignment.MiddleTop || aligned.Item.Alignment == ContextItemAlignment.MiddleBottom || aligned.Item.Alignment == ContextItemAlignment.Center) {
					 return CalcCurrentXForLeftItems(list);
			}
			if(aligned.Item.Alignment == ContextItemAlignment.TopFar || aligned.Item.Alignment == ContextItemAlignment.BottomFar || aligned.Item.Alignment == ContextItemAlignment.CenterFar) {
				return CalcCurrentXForRightItems(list);
			}
			if(aligned.Item.Alignment == ContextItemAlignment.NearTop || aligned.Item.Alignment == ContextItemAlignment.FarTop ||
				aligned.Item.Alignment == ContextItemAlignment.NearCenter || aligned.Item.Alignment == ContextItemAlignment.FarCenter) {
					return CalcCurrentYForTopItems(list);
			}
			if(aligned.Item.Alignment == ContextItemAlignment.NearBottom || aligned.Item.Alignment == ContextItemAlignment.FarBottom) {
				return CalcCurrentYForBottomItems(list);
			}
			return -1;
		}
		protected virtual List<IAnchored> CalcAnchorGroupBounds(List<IAnchored> anchorGroup) {
			anchorGroup = AnchorLayoutHelper.CalcOrder(anchorGroup);
			for(int j = 0; j < anchorGroup.Count; j++)
				((ContextItemViewInfo)anchorGroup[j]).Bounds = AnchorLayoutHelper.LayoutToAnchor(anchorGroup[j]);
			return anchorGroup;
		}
		protected virtual List<IAnchored> GetAnchorGroup(ContextItemViewInfo anchorViewInfo) {
			List<IAnchored> anchorGroup = new List<IAnchored>();
			anchorGroup.Add(anchorViewInfo);
			anchorGroup.AddRange(GetAnchored(anchorViewInfo, new List<IAnchored>()));
			anchorGroup = CalcAnchorGroupBounds(anchorGroup);		   
			return anchorGroup;
		}
		protected virtual void CalcUpperLeftItems() {
			UpperLeftViewInfos = new List<ContextItemViewInfo>();
			List<ContextItemViewInfo> upperLeftAlignedViewInfos = GetViewInfosByAlignment(ContextItemAlignment.TopNear);
			int currentX = TopContentBounds.X;
			for(int i = 0; i < upperLeftAlignedViewInfos.Count; i++) {
				if(this.upperLineFilled) {
					MarkAsNotFittedItem(upperLeftAlignedViewInfos[i]);					
					continue;
				}
				Size itemSize = upperLeftAlignedViewInfos[i].Size;
				int currentIndent = i == 0 ? 0 : Indent;
				upperLeftAlignedViewInfos[i].Bounds = new Rectangle(new Point(currentX + currentIndent, TopContentBounds.Top), itemSize);								  
				List<IAnchored> anchorGroup;
				currentX = CalcAnchorGroup(upperLeftAlignedViewInfos[i], out anchorGroup);
				foreach(IAnchored item in anchorGroup) {
					UpperLeftViewInfos.Add((ContextItemViewInfo)item);
					if(item != upperLeftAlignedViewInfos[i])
						upperLeftAlignedViewInfos[i].AnchoredItems.Add((ContextItemViewInfo)item);
				}
				CheckAnchorGroup(anchorGroup);								 
			}
		}
		protected virtual internal int GetLeftMaxX(ContextItemAlignment leftAlignment) {
			if(leftAlignment == ContextItemAlignment.TopNear) {
				return GetMaxX(UpperLeftViewInfos);
			}
			if(leftAlignment == ContextItemAlignment.BottomNear) {
				return GetMaxX(LowerLeftViewInfos);
			}
			if(leftAlignment == ContextItemAlignment.CenterNear) {
				return GetMaxX(CenterLeftViewInfos);
			}
			return -1;
		}
		protected virtual internal int GetRightMinX(ContextItemAlignment rightAlignment) {
			if(rightAlignment == ContextItemAlignment.TopFar)
				return GetMinX(UpperRightViewInfos);
			if(rightAlignment == ContextItemAlignment.BottomFar)
				return GetMinX(LowerRightViewInfos);
			if(rightAlignment == ContextItemAlignment.CenterFar)
				return GetMinX(CenterRightViewInfos);
			return -1;
		}
		protected virtual internal int GetTopMaxY(ContextItemAlignment topAlignment) {
			if(topAlignment == ContextItemAlignment.NearTop)
				return GetMaxY(NearTopViewInfos);
			if(topAlignment == ContextItemAlignment.FarTop)
				return GetMaxY(FarTopViewInfos);
			return -1;
		}
		protected virtual internal int GetBottomMinY(ContextItemAlignment bottomAlignment) { 
			if(bottomAlignment == ContextItemAlignment.NearBottom)
				return GetMinY(NearBottomViewInfos);
			if(bottomAlignment == ContextItemAlignment.FarBottom)
				return GetMinY(FarBottomViewInfos);
			return -1;
		}
		protected internal int GetMaxX(List<ContextItemViewInfo> list) {
			int maxX = -1;
			bool flag = false;
			for(int i = 0; i < list.Count; i++) {
				if(list[i].IsNotFittedWidth) continue;
				if(!flag) {
					maxX = list[i].Bounds.Right;
					flag = true;
				}
				else {
					if(list[i].Bounds.Right > maxX)
						maxX = list[i].Bounds.Right;
				}
			}
			return maxX;
		}
		protected internal int GetMinX(List<ContextItemViewInfo> list) {
			int minX = -1;
			bool flag = false;
			for(int i = 0; i < list.Count; i++) {
				if(list[i].IsNotFittedWidth) continue;
				if(!flag) {
					minX = list[i].Bounds.Left;
					flag = true;
				}
				else {
					if(list[i].Bounds.Left < minX)
						minX = list[i].Bounds.Left;
				}
			}
			return minX;
		}
		protected internal int GetMaxY(List<ContextItemViewInfo> list) {
			int maxY = -1;
			bool flag = false;
			for(int i = 0; i < list.Count; i++) {
				if(list[i].IsNotFittedHeight) continue;
				if(!flag) {
					maxY = list[i].Bounds.Bottom;
					flag = true;
				}
				else {
					if(list[i].Bounds.Bottom > maxY)
						maxY = list[i].Bounds.Bottom;
				}
			}
			return maxY;
		}
		protected internal int GetMinY(List<ContextItemViewInfo> list) {
			int minY = -1;
			bool flag = false;
			for(int i = 0; i < list.Count; i++) {
				if(list[i].IsNotFittedHeight) continue;
				if(!flag) {
					minY = list[i].Bounds.Top;
					flag = true;
				}
				else {
					if(list[i].Bounds.Top < minY)
						minY = list[i].Bounds.Top;
				}
			}
			return minY;
		}
		protected virtual internal Rectangle GetItemsBounds(ContextItemAlignment alignment) {
			List<ContextItemViewInfo> viewInfos = GetViewInfosByAlignment(alignment);
			Rectangle bounds = new Rectangle(new Point(viewInfos.Min(viewInfo => viewInfo.Bounds.X), viewInfos.Min(viewInfo => viewInfo.Bounds.Y)),
										   new Size(viewInfos[viewInfos.Count - 1].Bounds.Right - viewInfos[0].Bounds.Left, viewInfos.Max(viewInfo => viewInfo.Bounds.Height)));
			return bounds;				 
		}
		protected virtual internal Rectangle GetItemsBounds(List<ContextItemViewInfo> viewInfos) {
			Rectangle bounds = Rectangle.Empty;
			int x = viewInfos.Min(viewInfo => viewInfo.Bounds.X);
			int y = viewInfos.Min(viewInfo => viewInfo.Bounds.Y);
			int right = viewInfos.Max(viewInfo => viewInfo.Bounds.Right);
			int bottom = viewInfos.Max(viewInfo => viewInfo.Bounds.Bottom);
			bounds = new Rectangle(new Point(x,y), new Size(right - x, bottom - y));
			return bounds;
		}
		protected internal Rectangle TopContentBounds { get; private set; }
		protected internal Rectangle BottomContentBounds { get; private set; }
		protected internal Rectangle CenterContentBounds { get; private set; }
		protected internal Rectangle NearContentBounds { get; private set; }
		protected internal Rectangle FarContentBounds { get; private set; }
		protected internal Rectangle TopPanelBounds { get; private set; }
		protected internal Rectangle BottomPanelBounds { get; private set; }
		protected internal Rectangle CenterPanelBounds { get; private set; }
		protected internal Rectangle NearPanelBounds { get; private set; }
		protected internal Rectangle FarPanelBounds { get; private set; }
		protected virtual void CalcTopPanelBounds() {
			TopPanelBounds = CalcHorizontalPanelBounds(MiddleTopViewInfos, UpperLeftViewInfos, UpperRightViewInfos);
			if(Owner.ShowOutsideDisplayBounds)
				TopPanelBounds = new Rectangle(TopPanelBounds.X, TopPanelBounds.Y - TopPanelBounds.Height, TopPanelBounds.Width, TopPanelBounds.Height);
		}
		protected virtual void CalcBottomPanelBounds() {
			BottomPanelBounds = CalcHorizontalPanelBounds(MiddleBottomViewInfos, LowerLeftViewInfos, LowerRightViewInfos);
			if(Owner.ShowOutsideDisplayBounds)
				BottomPanelBounds = new Rectangle(BottomPanelBounds.X, BottomPanelBounds.Bottom, BottomPanelBounds.Width, BottomPanelBounds.Height);
		}
		protected virtual void CalcCenterPanelBounds() {
			CenterPanelBounds = CalcHorizontalPanelBounds(MiddleCenterViewInfos, CenterLeftViewInfos, CenterRightViewInfos); 
		}
		protected virtual void CalcNearPanelBounds() {
			NearPanelBounds = CalcVerticalPanelBounds(NearCenterViewInfos, NearTopViewInfos, NearBottomViewInfos);
			if(Owner.ShowOutsideDisplayBounds)
				NearPanelBounds = new Rectangle(NearPanelBounds.Left - NearPanelBounds.Width, NearPanelBounds.Y, NearPanelBounds.Width, NearPanelBounds.Height);
		}
		protected virtual void CalcFarPanelBounds() {
			FarPanelBounds = CalcVerticalPanelBounds(FarCenterViewInfos, FarTopViewInfos, FarBottomViewInfos);
			if(Owner.ShowOutsideDisplayBounds)
				FarPanelBounds = new Rectangle(FarPanelBounds.Right, FarPanelBounds.Y, FarPanelBounds.Width, FarPanelBounds.Height);
		}
		private Rectangle CalcHorizontalPanelBounds(List<ContextItemViewInfo> middleItems, List<ContextItemViewInfo> leftItems, List<ContextItemViewInfo> rightItems) {
			int top = int.MaxValue, bottom = int.MinValue;
			CalcHorizontalPanelBounds(middleItems, ref top, ref bottom);
			CalcHorizontalPanelBounds(leftItems, ref top, ref bottom);
			CalcHorizontalPanelBounds(rightItems, ref top, ref bottom);
			Rectangle res = Rectangle.Empty;
			if(top == int.MaxValue || bottom == int.MaxValue)
				return Rectangle.Empty;
			if(middleItems.Count != 0 && middleItems[0].Item.Alignment == ContextItemAlignment.MiddleTop ||
				leftItems.Count != 0 && leftItems[0].Item.Alignment == ContextItemAlignment.TopNear ||
				rightItems.Count != 0 && rightItems[0].Item.Alignment == ContextItemAlignment.TopFar) {
				res = new Rectangle(Bounds.X, Bounds.Top, Bounds.Width, bottom - Bounds.Top + Options.TopPanelPadding.Bottom);
			}
			else if(middleItems.Count != 0 && middleItems[0].Item.Alignment == ContextItemAlignment.MiddleBottom ||
				leftItems.Count != 0 && leftItems[0].Item.Alignment == ContextItemAlignment.BottomNear ||
				rightItems.Count != 0 && rightItems[0].Item.Alignment == ContextItemAlignment.BottomFar) {
				res = new Rectangle(Bounds.X, top - Options.BottomPanelPadding.Top, Bounds.Width, Bounds.Bottom - (top - Options.BottomPanelPadding.Top));
			}
			else
				res = new Rectangle(Bounds.X + NearPanelBounds.Width, top - Options.CenterPanelPadding.Top, Bounds.Width - FarPanelBounds.Width - NearPanelBounds.Width, bottom - top + Options.CenterPanelPadding.Vertical);
			return res;
		}
		private void CalcHorizontalPanelBounds(List<ContextItemViewInfo> items, ref int top, ref int bottom) {
			foreach(ContextItemViewInfo item in items) {
				if(item.IsNotFittedWidth || item.IsNotFittedHeight)
					continue;
				top = Math.Min(top, item.Bounds.Top);
				bottom = Math.Max(bottom, item.Bounds.Bottom);
			}
		}
		private Rectangle CalcVerticalPanelBounds(List<ContextItemViewInfo> centerItems, List<ContextItemViewInfo> topItems, List<ContextItemViewInfo> bottomItems) {
			int left = int.MaxValue, right = int.MinValue;
			CalcVerticalPanelBounds(centerItems, ref left, ref right);
			CalcVerticalPanelBounds(topItems, ref left, ref right);
			CalcVerticalPanelBounds(bottomItems, ref left, ref right);
			Rectangle res = Rectangle.Empty;
			if(left == int.MaxValue || right == int.MaxValue)
				return Rectangle.Empty;
			if(centerItems.Count != 0 && centerItems[0].Item.Alignment == ContextItemAlignment.NearCenter ||
				topItems.Count != 0 && topItems[0].Item.Alignment == ContextItemAlignment.NearTop ||
				bottomItems.Count != 0 && bottomItems[0].Item.Alignment == ContextItemAlignment.NearBottom) {
					res = new Rectangle(Bounds.X, Bounds.Y + TopPanelBounds.Height, right - Bounds.Left + Options.NearPanelPadding.Right, Bounds.Height - BottomPanelBounds.Height - TopPanelBounds.Bottom);
			}
			else if(centerItems.Count != 0 && centerItems[0].Item.Alignment == ContextItemAlignment.FarCenter ||
				topItems.Count != 0 && topItems[0].Item.Alignment == ContextItemAlignment.FarTop ||
				bottomItems.Count != 0 && bottomItems[0].Item.Alignment == ContextItemAlignment.FarBottom) {
					res = new Rectangle(left - Options.FarPanelPadding.Left, Bounds.Y + TopPanelBounds.Height, Bounds.Right - (left - Options.FarPanelPadding.Left), Bounds.Height - BottomPanelBounds.Height - TopPanelBounds.Bottom);
			}
			return res;
		}
		private void CalcVerticalPanelBounds(List<ContextItemViewInfo> items, ref int left, ref int right) {
			foreach(ContextItemViewInfo item in items) {
				if(item.IsNotFittedWidth || item.IsNotFittedHeight)
					continue;
				left = Math.Min(left, item.Bounds.Left);
				right = Math.Max(right, item.Bounds.Right);
			}
		}
		protected virtual internal void RecalculateItems(ContextItemAlignment alignment) { 
			switch(alignment){
				case ContextItemAlignment.TopNear: CalcUpperLeftItems();
					break;
				case ContextItemAlignment.MiddleTop: CalcMiddleTopItems();
					break;
				case ContextItemAlignment.TopFar: CalcUpperRightItems();
					break;
				case ContextItemAlignment.BottomNear: CalcLowerLeftItems();
					break;
				case ContextItemAlignment.MiddleBottom: CalcMiddleBottomItems();
					break;
				case ContextItemAlignment.BottomFar: CalcLowerRightItems();
					break;
				case ContextItemAlignment.CenterNear: CalcCenterLeftItems();
					break;
				case ContextItemAlignment.Center: CalcMiddleCenterItems();
					break;
				case ContextItemAlignment.CenterFar: CalcCenterRightItems();
					break;
				default:
					break;
			}
		}		
		protected virtual internal List<IAnchored> GetAnchored(ContextItemViewInfo viewInfo, List<IAnchored> list) {			
			foreach(ContextItemViewInfo vi in Items) {
				ContextItem item = vi.Item.AnchorElement;
				if(vi.Item.AnchorElement != null && vi.Item.AnchorElement == viewInfo.Item ) {
					vi.Bounds = new Rectangle(Point.Empty, vi.Size);
					list.Add(vi);
					GetAnchored(vi, list);
				}
			}
			return list;  
		}
		public virtual List<ContextItemViewInfo> GetViewInfosByAlignment(ContextItemAlignment align) {
			List<ContextItemViewInfo> alignedViewInfos = new List<ContextItemViewInfo>();
			foreach(ContextItemViewInfo viewInfo in Items) {
				if(viewInfo.Item.Alignment == align && viewInfo.Item.AnchorElement == null)
					alignedViewInfos.Add(viewInfo);
			}
			return alignedViewInfos;		
		}
		public virtual ContextItem GetItemByLocation(Point point) {
			ContextItemViewInfo vi = GetItemViewInfoByLocation(point);
			if(vi != null)
				return GetItemViewInfoByLocation(point).Item;
			return null;
		}
		public virtual ContextItemViewInfo GetItemViewInfoByLocation(Point point) {
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i].Bounds.Contains(point) && !viewInfos[i].IsNotFittedHeight && !viewInfos[i].IsNotFittedWidth && viewInfos[i].Item.Visibility != ContextItemVisibility.Hidden)
					return Items[i];
			}
			return null;
		}
		protected internal virtual void ForceItemRedraw(ContextItemViewInfo viewInfo) {
			if(!viewInfo.ShouldRedrawItem()) return;
			Owner.Redraw(viewInfo.RedrawBounds);
		}
		protected virtual ContextItemHitInfo CalcContextItemHitInfo(Point point) {
			ContextItemHitInfo hitInfo = null;
			foreach(ContextItemViewInfo vi in Items) {
				if(vi.IsNotFittedHeight || vi.IsNotFittedWidth || vi.Item.Visibility == ContextItemVisibility.Hidden) continue;
				hitInfo = vi.CalcHitInfo(point);
				if(hitInfo != null)
					break;
			}
			return hitInfo;
		}
		protected internal virtual ContextItemHitInfo CalcPanelHitInfo(Point point) {
			ContextItemHitInfo hitInfo = null;
			if(TopPanelBounds.Contains(point)) {
				hitInfo = new ContextItemHitInfo(point);
				hitInfo.SetHitTest(ContextItemHitTest.TopPanel);
				return hitInfo;
			}
			if(BottomPanelBounds.Contains(point)) {
				hitInfo = new ContextItemHitInfo(point);
				hitInfo.SetHitTest(ContextItemHitTest.BottomPanel);
				return hitInfo;
			}
			if(CenterPanelBounds.Contains(point)) {
				hitInfo = new ContextItemHitInfo(point);
				hitInfo.SetHitTest(ContextItemHitTest.CenterPanel);
				return hitInfo;
			}
			return hitInfo;
		}
		public virtual ContextItemHitInfo CalcHitInfo(Point point) {		  
			ContextItemHitInfo hitInfo = CalcContextItemHitInfo(point);
			if(hitInfo == null)
				hitInfo = CalcPanelHitInfo(point);		  
			return hitInfo;
		}
		public virtual void UpdateContextButtonHitInfo(Point point) {
			ContextItem item = GetItemByLocation(point);
			if(item == null || !item.Enabled) {
				HoverItem = null;
				return;
			}
			ContextItemHitInfo hitInfo = CalcHitInfo(point);
			HoverItem = (hitInfo != null) ? hitInfo.HitViewInfo : null;
			if(HoverItem != null) HoverItem.HitInfo = hitInfo;
		}
		public virtual void UpdateContextButtonPressedInfo(Point point) {
			ContextItemHitInfo pressedInfo = CalcHitInfo(point);
			if(pressedInfo != null && pressedInfo.HitViewInfo != null) pressedInfo.HitViewInfo.PressedInfo = pressedInfo;
		}
		public virtual List<ContextItemViewInfo> GetViewInfosByVisibility(ContextItemVisibility visibility) {
			List<ContextItemViewInfo> visibleViewInfos = new List<ContextItemViewInfo>();
			foreach(ContextItemViewInfo viewInfo in this.viewInfos) {
				if(viewInfo.Item.Visibility == visibility)
					visibleViewInfos.Add(viewInfo);
			}
			return visibleViewInfos;
		}
		protected virtual void CalcSideIndent(List<ContextItemViewInfo> viewInfos) { 
			Rectangle panelContentBounds = Rectangle.Empty;
			if(viewInfos[0].Item.Alignment == ContextItemAlignment.MiddleTop) panelContentBounds = TopContentBounds;
			if(viewInfos[0].Item.Alignment == ContextItemAlignment.MiddleBottom) panelContentBounds = BottomContentBounds;
			if(viewInfos[0].Item.Alignment == ContextItemAlignment.Center) panelContentBounds = CenterContentBounds;
			this.sideIndent = (Bounds.Width - CalcIndent(viewInfos)) / 2 - (panelContentBounds.Left - Bounds.Left);
		}
		protected virtual int CalcIndent(List<ContextItemViewInfo> viewInfos) {
			bool isHorizontal = viewInfos[0].Item.Orientation == Orientation.Horizontal;
			int indent = 0;
			foreach(ContextItemViewInfo viewInfo in viewInfos) {
				viewInfo.Bounds = new Rectangle(Point.Empty, viewInfo.Size);
				List<IAnchored> anchorGroup;
				CalcAnchorGroup(viewInfo, out anchorGroup);
				List<ContextItemViewInfo> middleItems = new List<ContextItemViewInfo>();
				foreach(IAnchored item in anchorGroup)
					middleItems.Add(item as ContextItemViewInfo);
				int min = isHorizontal ? GetMinX(middleItems) : GetMinY(middleItems);
				int max = isHorizontal ? GetMaxX(middleItems) : GetMaxY(middleItems);
				int width = max - min;
				indent += width;
			}
			indent += (viewInfos.Count - 1) * Indent;
			return indent;
		}
		protected internal void SetCursor(Cursor cursor) {
			if(cursor == null)
				return;
			ISupportContextItemsCursor cursorOwner = Owner.Control as ISupportContextItemsCursor;
			if(cursorOwner != null)
				cursorOwner.SetCursor(cursor);
			else
				Owner.Control.Cursor = cursor;
		}
		public virtual void UpdateHyperlinkCursor(Point point) {
			ContextItemViewInfo viewInfo = GetItemViewInfoByLocation(point);
			if (viewInfo == null || !(viewInfo is ContextButtonViewInfo)) return;		   
			ContextButtonViewInfo btnViewInfo = viewInfo as ContextButtonViewInfo;
			if(btnViewInfo.HasHyperlink) {
				StringBlock block = btnViewInfo.StringInfo.GetLinkByPoint(point);
				if(block == null) {
					SetCursor(PrevCursor);
					PrevCursor = null;
				}
				else {
					if(PrevCursor == null)
						PrevCursor = Owner.Control.Cursor;
					SetCursor(Cursors.Hand);
				}
			}		   
		}
		protected virtual void ResetButtonSizes() {
			foreach(ContextItemViewInfo viewInfo in Items)
				if(viewInfo is ContextButtonViewInfo) {
					ContextButtonViewInfo btnViewInfo = viewInfo as ContextButtonViewInfo;
					if(btnViewInfo.Mode != ContextButtonMode.Glyph)
						btnViewInfo.ResetSize();
				}
		}
		public List<ContextItemViewInfo> GetUpperViewInfos() {
			List<ContextItemViewInfo> list = new List<ContextItemViewInfo>();
			list.AddRange(UpperLeftViewInfos);
			list.AddRange(MiddleTopViewInfos);
			list.AddRange(UpperRightViewInfos);
			return list;
		}
		public List<ContextItemViewInfo> GetLowerViewInfos() {
			List<ContextItemViewInfo> list = new List<ContextItemViewInfo>();
			list.AddRange(LowerLeftViewInfos);
			list.AddRange(MiddleBottomViewInfos);
			list.AddRange(LowerRightViewInfos);
			return list;
		}
		public List<ContextItemViewInfo> GetCenterViewInfos() {
			List<ContextItemViewInfo> list = new List<ContextItemViewInfo>();
			list.AddRange(CenterLeftViewInfos);
			list.AddRange(MiddleCenterViewInfos);
			list.AddRange(CenterRightViewInfos);
			return list;
		}
		public List<ContextItemViewInfo> GetNearViewInfos() {
			List<ContextItemViewInfo> list = new List<ContextItemViewInfo>();
			list.AddRange(NearTopViewInfos);
			list.AddRange(NearCenterViewInfos);
			list.AddRange(NearBottomViewInfos);
			return list;
		}
		public List<ContextItemViewInfo> GetFarViewInfos() {
			List<ContextItemViewInfo> list = new List<ContextItemViewInfo>();
			list.AddRange(FarTopViewInfos);
			list.AddRange(FarCenterViewInfos);
			list.AddRange(FarBottomViewInfos);
			return list;
		}
		public virtual float GetCurrentAnimationCoeff() {		   
			float coeff = CalcAnimationCoeff();
			ContextFloatAnimationInfo info = XtraAnimator.Current.Get(XtraAnimationObject, FloatAnimationId) as ContextFloatAnimationInfo;
			if(info == null) return coeff;
			if(!info.IsStarted) 
				return CurrentOpacity;
			CurrentOpacity = info.Value;
			return CurrentOpacity;
		}	   
		public virtual List<float> GetCurrentOpacities() {
			float coeff = CalcWaveAnimationCoeff();
			WaveAnimationInfo info = XtraAnimator.Current.Get(XtraAnimationObject, WaveAnimationId) as WaveAnimationInfo;
			if(info == null) {
				SetOpacitiesByItemOpacity();
				return this.opacities;
			}
			if(!info.IsStarted) return opacities;
			this.opacities = info.Opacities;
			this.currentTime = info.CurrentTime;
			return this.opacities;
		}
		private void SetOpacitiesByItemOpacity() {
			if(this.opacities == null)
				return;
			for(int i = 0; i < this.opacities.Count; i++)
				this.opacities[i] = CurrentOpacity;
		}		
		float currentTime;
		public virtual float CalcWaveAnimationCoeff() {
			float coeff = CalcAnimationCoeffCore();
			if(!ShouldStartAnimation(coeff, prevWaveAnimationCoeff)) return coeff;
			StopWaveAnimation();
			StartWaveAnimation(this.currentTime, Options.AnimationTime * coeff);
			prevWaveAnimationCoeff = coeff;
			return coeff;
		}
		bool IsForcedVisibleItemsSet { 
			get {
				if(Items.Count == 0)
					return false;
				return Items[0].Item.IsForcedVisibilitySet;
			} 
		}
		protected virtual float CalcAnimationCoeff() {
			if(IsForcedVisibleItemsSet)
				return 1.0f;
			float coeff = CalcAnimationCoeffCore();
			if(!ShouldStartAnimation(coeff, prevFloatAnimationCoeff)) return coeff;
			if(Options.GetAnimationType() == ContextAnimationType.None) {
				return ShouldShowAutoItems ? 1.0f : 0.0f;
			}
			StopFloatAnimation();
			StartFloatAnimation(CurrentOpacity, coeff);
			prevFloatAnimationCoeff = coeff;
			return coeff;
		}
		protected internal virtual bool IsNotHiddenItemsExisted() {
			foreach(ContextItemViewInfo vi in Items)
				if(vi.Item.Visibility != ContextItemVisibility.Hidden)
					return true;
			return false;
		}
		protected virtual bool ShouldStartAnimation(float opacity, float prevAnimationCoeff) {
			return (Math.Abs(opacity - prevAnimationCoeff) > 0 && Items.Count > 0 && IsNotHiddenItemsExisted() && !StopAnimation);
		}
		ContextItemViewInfo hotItem;
		protected internal virtual ContextItemViewInfo HoverItem {
			get { return hotItem; }
			set {
				if(HoverItem == value)
					return;
				ContextItemViewInfo prev = HoverItem;
				hotItem = value;
				OnHoverItemChanged(prev, HoverItem);
			}
		}
		protected virtual void OnHoverItemChanged(ContextItemViewInfo prev, ContextItemViewInfo current) {
			if(prev != null) {
				prev.OnHoverOut();
			}
			if(current != null)
				Owner.Redraw(current.RedrawBounds);
		}
		protected internal Point MousePosition { get; set; }
		protected internal virtual float HideItemOpacity { get { return 0.0f; } }
		protected internal virtual float NormalItemOpacity { get { return Owner.Options.NormalStateOpacity; } }
		protected internal virtual float HoverItemOpacity { get { return Owner.Options.HoverStateOpacity; } }
		protected internal virtual float DisabledItemOpacity { get { return Owner.Options.DisabledStateOpacity; } }
		protected virtual internal float CalcAnimationCoeffCore() {
			if(Owner.Control == null)
				return 0.0f;
			Point pt = MousePosition;
			return Owner.ActivationBounds.Contains(pt)? 1.0f: 0.0f;
		}
		protected internal virtual void StopAllAnimations() {
			PrevOpacity = 0.0f;
			StopFloatAnimation();
			StopWaveAnimation();
		}
		protected virtual void StartFloatAnimation(float start, float end) {
			XtraAnimator.Current.AddAnimation(new ContextFloatAnimationInfo(XtraAnimationObject, FloatAnimationId, Options.AnimationTime, start, end));
		}
		protected virtual void StopFloatAnimation() {
			XtraAnimator.RemoveObject(XtraAnimationObject, FloatAnimationId);
		}
		object floatAnimationId = null;
		protected object FloatAnimationId {
			get {
				if(this.floatAnimationId == null) this.floatAnimationId = new object();
				return floatAnimationId;
			}
		}
		protected virtual void StartWaveAnimation(float start, float end) {
			XtraAnimator.Current.AddAnimation(new WaveAnimationInfo(XtraAnimationObject, GetAnimationItemCount(), WaveAnimationId, Options.AnimationTime, start, end));
		}
		int GetAnimationItemCount() {
			int count = 0;
			foreach(ContextItem item in Collection) {
				RatingContextButton rating = item as RatingContextButton;
				if(rating == null) continue;
				count = Math.Max(rating.ItemCount, count);
			}
			return count;
		}
		protected virtual void StopWaveAnimation() {
			XtraAnimator.RemoveObject(XtraAnimationObject, WaveAnimationId);
		}
		object waveAnimationId = null;
		protected object WaveAnimationId {
			get {
				if(this.waveAnimationId == null) this.waveAnimationId = new object();
				return waveAnimationId;
			}
		}		
		public Control OwnerControl {
			get { return Owner.Control; }
		}
		public bool CanAnimate {
			get { return true; }
		}
		public void Refresh() {
			IsReady = false;
			CalcItems();
		}
		public virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo info = null;
			ContextItemViewInfo vi = GetItemViewInfoByLocation(point);
			if(vi == null || !(vi.Item.ShowToolTips && Options.ShowToolTips) || !vi.Item.Enabled)
				return info;
			info = new ToolTipControlInfo();
			if(vi.Item.ToolTip.Length > 0) {
				info.Object = vi.Item;
				info.Text = vi.Item.ToolTip;
			}
			else {
				if(vi is RatingContextButtonViewInfo || vi is TrackBarContextButtonViewInfo) {
					ContextButtonToolTipEventArgs e = vi.OnItemToolTip(point);
					info.Object = e.Value;
					info.Text = e.Text;
				}
			}
			if(info.Object == null && vi.Item.SuperTip == null) return null;
			info.SuperTip = vi.Item.SuperTip;
			info.Title = vi.Item.ToolTipTitle;
			info.IconType = vi.Item.ToolTipIconType;
			info.AllowHtmlText = Options.AllowHtmlTextInToolTip;
			return info;
		}
		internal float GetPanelOpacity(ContextButtonsPanel contextButtonsPanel) {
			float opacity = GetCurrentAnimationCoeff(); 
			bool hasAlwaysVisibleItems = false;
			switch (contextButtonsPanel) {
				case ContextButtonsPanel.Bottom : 
					hasAlwaysVisibleItems = HasAlwaysVisibleItems(LowerLeftViewInfos) || HasAlwaysVisibleItems(MiddleBottomViewInfos) || HasAlwaysVisibleItems(LowerRightViewInfos);
					break;
				case ContextButtonsPanel.Center : 
					hasAlwaysVisibleItems = HasAlwaysVisibleItems(CenterLeftViewInfos) || HasAlwaysVisibleItems(MiddleCenterViewInfos) || HasAlwaysVisibleItems(CenterRightViewInfos);
					break;
				case ContextButtonsPanel.Far : 
					hasAlwaysVisibleItems = HasAlwaysVisibleItems(FarTopViewInfos) || HasAlwaysVisibleItems(FarBottomViewInfos) || HasAlwaysVisibleItems(FarCenterViewInfos);
					break;
				case ContextButtonsPanel.Near :
					hasAlwaysVisibleItems = HasAlwaysVisibleItems(NearTopViewInfos) || HasAlwaysVisibleItems(NearBottomViewInfos) || HasAlwaysVisibleItems(NearCenterViewInfos);
					break;
				case ContextButtonsPanel.Top : 
					hasAlwaysVisibleItems = HasAlwaysVisibleItems(UpperLeftViewInfos) || HasAlwaysVisibleItems(MiddleTopViewInfos) || HasAlwaysVisibleItems(UpperRightViewInfos);
					break;
			}
			if(hasAlwaysVisibleItems) return 1.0f;
			return opacity;
		}
		public Size GetButtonsSizeByAlignment(ContextItemAlignment alignment) {
			List<ContextItemViewInfo> vi;
			switch (alignment){
				case ContextItemAlignment.BottomFar: vi = LowerRightViewInfos; break;
				case ContextItemAlignment.BottomNear: vi = LowerLeftViewInfos; break;
				case ContextItemAlignment.Center: vi = MiddleCenterViewInfos; break;
				case ContextItemAlignment.CenterFar: vi = CenterRightViewInfos; break;
				case ContextItemAlignment.CenterNear: vi = CenterLeftViewInfos; break;
				case ContextItemAlignment.FarBottom: vi = FarBottomViewInfos; break;
				case ContextItemAlignment.FarCenter: vi = FarCenterViewInfos; break;
				case ContextItemAlignment.FarTop: vi = FarTopViewInfos; break;
				case ContextItemAlignment.MiddleBottom: vi = MiddleBottomViewInfos; break;
				case ContextItemAlignment.MiddleTop: vi = MiddleTopViewInfos; break;
				case ContextItemAlignment.NearBottom: vi = NearBottomViewInfos; break;
				case ContextItemAlignment.NearCenter: vi = NearCenterViewInfos; break;
				case ContextItemAlignment.NearTop: vi = NearTopViewInfos; break;
				case ContextItemAlignment.TopFar: vi = UpperRightViewInfos; break;
				case ContextItemAlignment.TopNear: 
				default: vi = UpperLeftViewInfos; break;
			}
			Rectangle res = Rectangle.Empty;
			foreach(ContextItemViewInfo itemInfo in vi) {
				if(itemInfo.Item.Visibility == ContextItemVisibility.Hidden) continue;
				if(res.IsEmpty) {
					res = itemInfo.Bounds;
					continue;
				}
				res.X = Math.Min(res.X, itemInfo.Bounds.X);
				res.Y = Math.Min(res.Y, itemInfo.Bounds.Y);
				res.Height = Math.Max(res.Height, itemInfo.Bounds.Bottom - res.Y);
				res.Width += itemInfo.Bounds.Width + Indent;
			}
			if(res.Width > 0) res.Width -= Indent;
			return res.Size;
		}
	}
	public interface ISupportContextItemsCursor {
		void SetCursor(Cursor cursor);
	}
	public interface ISupportContextItems {
		ContextItemCollection ContextItems { get; }
		ContextItemCollectionOptions Options { get; }
		Rectangle DisplayBounds { get; }
		Rectangle ActivationBounds { get; }
		Rectangle DrawBounds { get;}
		void Redraw();
		void Redraw(Rectangle rect);
		void Update();
		Control Control { get; }
		UserLookAndFeel LookAndFeel { get; }
		ItemLocation GetGlyphLocation(ContextButton btn);
		ItemHorizontalAlignment GetGlyphHorizontalAlignment(ContextButton btn);
		ItemVerticalAlignment GetGlyphVerticalAlignment(ContextButton btn);
		ItemHorizontalAlignment GetCaptionHorizontalAlignment(ContextButton btn);
		ItemVerticalAlignment GetCaptionVerticalAlignment(ContextButton btn);
		int GetGlyphToCaptionIndent(ContextButton btn);
		bool CloneItems { get; }
		void RaiseCustomizeContextItem(ContextItem item);
		void RaiseContextItemClick(ContextItemClickEventArgs e);
		void RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e);
		bool DesignMode { get; }
		bool ShowOutsideDisplayBounds { get; }
	}
	public interface IContextItemCollectionOwner {
		void OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue);
		void OnCollectionChanged();
		bool IsDesignMode { get; }
		bool IsRightToLeft { get; }
	}
	public class ContextItemCollectionPainter {
		public ContextItemCollectionPainter() { }
		public virtual void DrawPanels(ContextItemCollectionInfoArgs info) {
			if(info.ViewInfo.StopAnimation) return;
			if(ShouldDrawPanel(info.ViewInfo.Options.TopPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Top))) 
				DrawPanel(info, info.ViewInfo.TopPanelBounds, info.ViewInfo.Options.TopPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Top));			
			if(ShouldDrawPanel(info.ViewInfo.Options.BottomPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Bottom)))
				DrawPanel(info, info.ViewInfo.BottomPanelBounds, info.ViewInfo.Options.BottomPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Bottom));
			if(ShouldDrawPanel(info.ViewInfo.Options.CenterPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Center)))
				DrawPanel(info, info.ViewInfo.CenterPanelBounds, info.ViewInfo.Options.CenterPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Center));
			if(ShouldDrawPanel(info.ViewInfo.Options.NearPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Near)))
				DrawPanel(info, info.ViewInfo.NearPanelBounds, info.ViewInfo.Options.NearPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Near));
			if(ShouldDrawPanel(info.ViewInfo.Options.FarPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Far)))
				DrawPanel(info, info.ViewInfo.FarPanelBounds, info.ViewInfo.Options.FarPanelColor, info.ViewInfo.GetPanelOpacity(ContextButtonsPanel.Far));
		}
		protected bool ShouldDrawPanel(Color color, float opacity) {
			return color.A > 0 && opacity > 0.0f;
		}
		protected virtual void DrawPanel(ContextItemCollectionInfoArgs info, Rectangle rectangle, Color color, float opacity) {
			Color res = Color.FromArgb((int)(color.A * opacity), color.R, color.G, color.B);
			info.Cache.FillRectangle(info.Cache.GetSolidBrush(res), rectangle);
		}
		public virtual void Draw(ContextItemCollectionInfoArgs info) {
			CheckViewInfo(info);		 
			DrawPanels(info);
			DrawContextItems(info);
		}
		private void CheckViewInfo(ContextItemCollectionInfoArgs info) {
			if(!info.ViewInfo.IsReady)
				info.ViewInfo.CalcItems();
		}
		protected virtual void DrawContextItems(ContextItemCollectionInfoArgs info) {
			List<ContextItemViewInfo> viewInfos;
			if((info.ViewInfo.Options.GetAnimationType() == ContextAnimationType.None && !info.ViewInfo.ShouldShowAutoItems) || info.ViewInfo.StopAnimation) 
				viewInfos = info.ViewInfo.GetViewInfosByVisibility(ContextItemVisibility.Visible);
			else
				viewInfos = info.ViewInfo.Items; 
			for(int i = 0; i < viewInfos.Count; i++) {
				Image glyph = viewInfos[i].Item.Glyph;
				if(viewInfos[i].Item.Visibility == ContextItemVisibility.Hidden || 
					viewInfos[i].Item.Visibility == ContextItemVisibility.Auto && info.SuppressDrawAutoItems) continue;
				viewInfos[i].Painter.DrawItem(info);
			}
		} 
	}
	public class ContextFloatAnimationInfo : FloatAnimationInfo {
		public ContextFloatAnimationInfo(ISupportXtraAnimation obj, object animationId, int ms, float start, float end) : base(obj, animationId, ms, start, end) {
		}
		protected override void Invalidate() {
			ContextItemCollectionViewInfo collViewInfo = base.AnimatedObject as ContextItemCollectionViewInfo;
			ISupportContextItems owner = collViewInfo.Owner as ISupportContextItems;
			owner.Redraw(owner.DrawBounds);
		}
	}
	public class WaveAnimationInfo : BaseAnimationInfo {
		const int framesCount = 100;
		int itemCount;
		int animationTime;
		List<float> starts;
		List<float> opacities;
		private float start;
		private float end;
		public WaveAnimationInfo(ISupportXtraAnimation obj, int itemCount, object animationId, int animationTime,float start, float end)
			: base(obj, animationId, CalcTicksCount(animationTime), framesCount) {
			this.itemCount = itemCount;
			this.animationTime = animationTime;
			CalcStarts();
			this.start = start;
			this.end = end;
			IsStarted = false;
		}
		public List<float> Opacities {
			get {
				if(opacities == null)
					opacities = new List<float>();
				return opacities;
			}
		}
		public float CurrentTime { get; private set; }
		static int CalcTicksCount(int ms) {
			return (int)(ms * TimeSpan.TicksPerMillisecond / framesCount);
		}
		public override void FrameStep() {
			IsStarted = true;
			FrameStepCore(((float)(CurrentFrame)) / FrameCount);
			Invalidate();
		}
		protected virtual void FrameStepCore(float k) {
			CurrentTime = this.start + k * (this.end - this.start);
			CalcOpacities();
		}
		private void CalcStarts() {
			starts = new List<float>();
			float timeStep = (0.4f * this.animationTime)/ itemCount;
			for(int i = 0; i < itemCount; i++) {
				float start = i * timeStep;
				starts.Add(start);
			}
		}
		private void CalcOpacities() {
			Opacities.Clear();
			for(int i = 0; i < itemCount; i++) {
				float opacity = CalcItemOpacity(i);
				Opacities.Add(opacity);
			}
		}
		private float CalcItemOpacity(int i) {
			float duration = 0.6f * this.animationTime;
			if(CurrentTime < this.starts[i])
				return 0f;
			if(CurrentTime > this.starts[i] + duration)
				return 1f;
			float opacity = (CurrentTime - this.starts[i]) / duration;
			return opacity;
		}
		public bool IsStarted { get; private set; }
		protected virtual void Invalidate() {		 
			ContextItemCollectionViewInfo collViewInfo = base.AnimatedObject as ContextItemCollectionViewInfo;
			ISupportContextItems owner = collViewInfo.Owner as ISupportContextItems;
			owner.Redraw(owner.DrawBounds);
		}
	}
	public class ContextAlignmentEnumConverter : EnumConverter {
		public ContextAlignmentEnumConverter(Type type) : base(type) {
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value is ContextItemAlignment)
				return GetName((ContextItemAlignment)value);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected string GetName(ContextItemAlignment val) {
			foreach(FieldInfo item in typeof(ContextItemAlignment).GetFields()) {
				if(Attribute.IsDefined(item, typeof(BrowsableAttribute)) || item.FieldType != typeof(ContextItemAlignment)) continue;
				if(item.GetValue(item).Equals(val)) return item.Name;
			}
			return string.Empty;
		}
	}
	public class SimpleContextButton : ContextButton {
		[Browsable(false), DefaultValue(ContextItemAlignment.CenterFar), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ContextItemAlignment Alignment {
			get { return ContextItemAlignment.CenterFar; }
			set { }
		}
	}
	public class SimpleCheckContextButton : CheckContextButton {
		[Browsable(false), DefaultValue(ContextItemAlignment.CenterFar), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ContextItemAlignment Alignment {
			get { return ContextItemAlignment.CenterFar; }
			set { }
		}
	}
	public class SimpleTrackBarContextButton : TrackBarContextButton {
		[Browsable(false), DefaultValue(ContextItemAlignment.CenterFar), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ContextItemAlignment Alignment {
			get { return ContextItemAlignment.CenterFar; }
			set { }
		}
	}
	public class SimpleRatingContextButton : RatingContextButton {
		[Browsable(false), DefaultValue(ContextItemAlignment.CenterFar), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ContextItemAlignment Alignment {
			get { return ContextItemAlignment.CenterFar; }
			set { }
		}
	}
	public class SimpleContextItemCollectionOptions : ContextItemCollectionOptions {
		public SimpleContextItemCollectionOptions() : this(null) { }
		public SimpleContextItemCollectionOptions(IContextItemCollectionOptionsOwner owner) : base(owner) { }
		bool ShouldSerializeBottomPanelPadding() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Padding BottomPanelPadding {
			get { return new Padding(0); }
			set { }
		}
		bool ShouldSerializeBottomPanelColor() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Color BottomPanelColor {
			get { return base.BottomPanelColor; }
			set { base.BottomPanelColor = value; }
		}
		bool ShouldSerializeCenterPanelPadding() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Padding CenterPanelPadding {
			get { return base.CenterPanelPadding; }
			set { base.CenterPanelPadding = value; }
		}
		bool ShouldSerializeCenterPanelColor() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Color CenterPanelColor {
			get { return base.CenterPanelColor; }
			set { base.CenterPanelColor = value; }
		}
		bool ShouldSerializeFarPanelPadding() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Padding FarPanelPadding {
			get { return new Padding(0); }
			set { }
		}
		bool ShouldSerializeFarPanelColor() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Color FarPanelColor {
			get { return base.FarPanelColor; }
			set { base.FarPanelColor = value; }
		}
		bool ShouldSerializeNearPanelPadding() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Padding NearPanelPadding {
			get { return new Padding(0); }
			set { }
		}
		bool ShouldSerializeNearPanelColor() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Color NearPanelColor {
			get { return base.NearPanelColor; }
			set { base.NearPanelColor = value; }
		}
		bool ShouldSerializeTopPanelPadding() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Padding TopPanelPadding {
			get { return new Padding(0); }
			set { }
		}
		bool ShouldSerializeTopPanelColor() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Color TopPanelColor {
			get { return base.TopPanelColor; }
			set { base.TopPanelColor = value; }
		}
	}
}
public interface IContextItemProvider {
	DevExpress.Utils.ContextItem CreateContextItem(Type type);
}
namespace DevExpress.XtraEditors {
	public enum RatingItemFillPrecision { Full, Half, Exact }
}
