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

using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraMap {
	public abstract class MapOverlayItemBase : IChangedCallbackOwner, IMapStyleOwner {
		internal static readonly Point DefaultLocation = new Point(0, 0);
		internal static readonly Size DefaultSize = new Size(0, 0);
		static readonly Padding DefaultPadding = new Padding(0);
		const Orientation DefaultJoiningOrientation = Orientation.Horizontal;
		const ContentAlignment DefaultAlignment = ContentAlignment.TopLeft;
		const bool DefaultVisible = true;
		Action callback;
		Point location;
		Size size;
		Padding margin;
		Padding padding;
		Orientation joiningOrientation = DefaultJoiningOrientation;
		ContentAlignment alignment = DefaultAlignment;
		bool visible = DefaultVisible;
		readonly OverlayItemAppearance appearance;
		protected internal OverlayItemAppearance Appearance { get { return appearance; } }
		[Category(SRCategoryNames.Layout)]
		public Point Location {
			get { return location; }
			set {
				if(location == value)
					return;
				location = value;
				OnChanged();
			}
		}
		void ResetLocation() {
			Location = DefaultLocation;
		}
		bool ShouldSerializeLocation() {
			return Location != DefaultLocation;
		}
		[Category(SRCategoryNames.Layout)]
		public Size Size {
			get { return size; }
			set {
				if(size == value)
					return;
				size = value;
				OnChanged();
			}
		}
		void ResetSize() {
			Size = DefaultSize;
		}
		bool ShouldSerializeSize() {
			return Size != DefaultSize;
		}
		[Category(SRCategoryNames.Layout)]
		public Padding Margin {
			get { return margin; }
			set {
				if(margin == value)
					return;
				margin = value;
				OnChanged();
			}
		}
		void ResetMargin() {
			Margin = DefaultPadding;
		}
		bool ShouldSerializeMargin() {
			return Margin != DefaultPadding;
		}
		[Category(SRCategoryNames.Layout)]
		public Padding Padding {
			get { return padding; }
			set {
				if(padding == value)
					return;
				padding = value;
				OnChanged();
			}
		}
		void ResetPadding() {
			Padding = DefaultPadding;
		}
		bool ShouldSerializePadding() {
			return Padding != DefaultPadding;
		}
		[Category(SRCategoryNames.Layout),
		DefaultValue(DefaultJoiningOrientation)]
		public Orientation JoiningOrientation {
			get { return joiningOrientation; }
			set {
				if(joiningOrientation == value)
					return;
				joiningOrientation = value;
				OnChanged();
			}
		}
		[Category(SRCategoryNames.Layout),
		DefaultValue(DefaultAlignment)]
		public ContentAlignment Alignment {
			get { return alignment; }
			set {
				if(alignment == value)
					return;
				alignment = value;
				OnChanged();
			}
		}
		[Category(SRCategoryNames.Layout),
		DefaultValue(DefaultVisible)]
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value)
					return;
				visible = value;
				OnChanged();
			}
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundStyle BackgroundStyle { get { return Appearance.ItemStyle; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundStyle HotTrackedStyle { get { return Appearance.HotTrackedItemStyle; } }
		protected MapOverlayItemBase() {
			this.appearance = CreateAppearance();
		}
		#region IChangedCallbackOwner implementation
		void IChangedCallbackOwner.SetParentCallback(Action callback) {
			this.callback = callback;
		}
		#endregion
		#region IMapStyleOwner implementation
		void IMapStyleOwner.OnStyleChanged() {
			OnChanged();
		}
		#endregion
		protected virtual OverlayItemAppearance CreateAppearance() {
			return new OverlayItemAppearance(this);
		}
		protected void OnChanged() {
			if(callback != null)
				callback();
		}
		protected internal abstract OverlayItemViewInfoBase CreateViewinfo(InnerMap map, MapUIElementsPainter painter);
	}
}
