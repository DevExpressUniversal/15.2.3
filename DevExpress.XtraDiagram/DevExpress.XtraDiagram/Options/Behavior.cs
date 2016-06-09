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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Options {
	public class DiagramOptionsBehavior : DiagramOptionsBase {
		bool allowEmptySelection;
		double bringIntoViewMargin;
		double glueToConnectionPointDistance;
		double glueToItemDistance;
		bool snapToGrid;
		bool snapToItems;
		double snapToItemsDistance;
		ResizingMode resizingMode;
		double minDragDistance;
		DiagramTool activeTool;
		DefaultBoolean useTabNavigation;
		DocumentCapability skinGallery;
		readonly DiagramControl diagram;
		public DiagramOptionsBehavior(DiagramControl diagram) {
			this.allowEmptySelection = true;
			this.bringIntoViewMargin = 10d;
			this.glueToConnectionPointDistance = 15d;
			this.glueToItemDistance = 7d;
			this.snapToGrid = true;
			this.snapToItems = true;
			this.snapToItemsDistance = 10d;
			this.resizingMode = ResizingMode.Live;
			this.minDragDistance = 3d;
			this.activeTool = DiagramController.DefaultTool;
			this.useTabNavigation = DefaultBoolean.Default;
			this.skinGallery = DocumentCapability.Default;
			this.diagram = diagram;
		}
		[DefaultValue(true)]
		public bool AllowEmptySelection {
			get { return allowEmptySelection; }
			set {
				if(AllowEmptySelection == value)
					return;
				bool prevValue = AllowEmptySelection;
				OnChanging("AllowEmptySelection", AllowEmptySelection);
				allowEmptySelection = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("AllowEmptySelection", prevValue, AllowEmptySelection));
			}
		}
		public double BringIntoViewMargin {
			get { return bringIntoViewMargin; }
			set {
				if(BringIntoViewMargin == value)
					return;
				double prevValue = BringIntoViewMargin;
				OnChanging("BringIntoViewMargin", BringIntoViewMargin);
				bringIntoViewMargin = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("BringIntoViewMargin", prevValue, BringIntoViewMargin));
			}
		}
		bool ShouldSerializeBringIntoViewMargin() { return MathUtils.IsNotEquals(BringIntoViewMargin, 10d); }
		void ResetBringIntoViewMargin() { BringIntoViewMargin = 10d; }
		public double GlueToConnectionPointDistance {
			get { return glueToConnectionPointDistance; }
			set {
				if(GlueToConnectionPointDistance == value) return;
				double prevValue = GlueToConnectionPointDistance;
				OnChanging("GlueToConnectionPointDistance", GlueToConnectionPointDistance);
				glueToConnectionPointDistance = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("GlueToConnectionPointDistance", prevValue, GlueToConnectionPointDistance));
			}
		}
		bool ShouldSerializeGlueToConnectionPointDistance() { return MathUtils.IsNotEquals(GlueToConnectionPointDistance, 15d); }
		void ResetGlueToConnectionPointDistance() { GlueToConnectionPointDistance = 15d; }
		public double GlueToItemDistance {
			get { return glueToItemDistance; }
			set {
				if(GlueToItemDistance == value) return;
				double prevValue = GlueToItemDistance;
				OnChanging("GlueToItemDistance", GlueToItemDistance);
				glueToItemDistance = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("GlueToItemDistance", prevValue, GlueToItemDistance));
			}
		}
		bool ShouldSerializeGlueToItemDistance() { return MathUtils.IsNotEquals(GlueToItemDistance, 7d); }
		void ResetGlueToItemDistance() { GlueToItemDistance = 7d; }
		[DefaultValue(true)]
		public bool SnapToGrid {
			get { return snapToGrid; }
			set {
				if(SnapToGrid == value)
					return;
				bool prevValue = SnapToGrid;
				OnChanging("SnapToGrid", SnapToGrid);
				snapToGrid = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("SnapToGrid", prevValue, SnapToGrid));
			}
		}
		[DefaultValue(true)]
		public bool SnapToItems {
			get { return snapToItems; }
			set {
				if(SnapToItems == value) return;
				bool prevValue = SnapToItems;
				OnChanging("SnapToItems", SnapToItems);
				snapToItems = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("SnapToItems", prevValue, SnapToItems));
			}
		}
		public double SnapToItemsDistance {
			get { return snapToItemsDistance; }
			set {
				if(SnapToItemsDistance == value) return;
				double prevValue = SnapToItemsDistance;
				OnChanging("SnapToItemsDistance", SnapToItemsDistance);
				snapToItemsDistance = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("SnapToItemsDistance", prevValue, SnapToItemsDistance));
			}
		}
		bool ShouldSerializeSnapToItemsDistance() { return MathUtils.IsNotEquals(SnapToItemsDistance, 10d); }
		void ResetSnapToItemsDistance() { SnapToItemsDistance  = 10d; }
		[DefaultValue(ResizingMode.Live)]
		public ResizingMode ResizingMode {
			get { return resizingMode; }
			set {
				if(ResizingMode == value) return;
				ResizingMode prevValue = ResizingMode;
				OnChanging("ResizingMode", ResizingMode);
				resizingMode = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("ResizingMode", prevValue, ResizingMode));
			}
		}
		public double MinDragDistance {
			get { return minDragDistance; }
			set {
				if(MinDragDistance == value) return;
				double prevValue = MinDragDistance;
				OnChanging("MinDragDistance", MinDragDistance);
				minDragDistance = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("MinDragDistance", prevValue, MinDragDistance));
			}
		}
		bool ShouldSerializeMinDragDistance() { return MathUtils.IsNotEquals(MinDragDistance, 3d); }
		void ResetMinDragDistance() { MinDragDistance = 3d; }
		public DiagramTool ActiveTool {
			get { return activeTool ?? DiagramController.DefaultTool; }
			set {
				if(ActiveTool == value) return;
				DiagramTool prevValue = ActiveTool;
				OnChanging("ActiveTool", ActiveTool);
				activeTool = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("ActiveTool", prevValue, ActiveTool, activeToolChanged: true));
			}
		}
		bool ShouldSerializeActiveTool() { return ActiveTool != DiagramController.DefaultTool; }
		void ResetActiveTool() { ActiveTool = DiagramController.DefaultTool; }
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean UseTabNavigation {
			get { return useTabNavigation; }
			set {
				if(UseTabNavigation == value) return;
				DefaultBoolean prevValue = UseTabNavigation;
				OnChanging("UseTabNavigation", UseTabNavigation);
				useTabNavigation = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("UseTabNavigation", prevValue, UseTabNavigation));
			}
		}
		internal bool AllowTabNavigation() {
			return UseTabNavigation != DefaultBoolean.False;
		}
		[DefaultValue(DocumentCapability.Default)]
		public DocumentCapability SkinGallery {
			get { return skinGallery; }
			set {
				if(SkinGallery == value) return;
				DocumentCapability prevValue = SkinGallery;
				OnChanging("SkinGallery", SkinGallery);
				skinGallery = value;
				OnChanged(new DiagramBehaviorOptionChangedEventArgs("SkinGallery", prevValue, SkinGallery));
			}
		}
		public override void Assign(BaseOptions other) {
			DiagramOptionsBehavior options = (DiagramOptionsBehavior)other;
			BeginUpdate();
			try {
				base.Assign(options);
				this.allowEmptySelection = options.AllowEmptySelection;
				this.bringIntoViewMargin = options.BringIntoViewMargin;
				this.glueToConnectionPointDistance = options.GlueToConnectionPointDistance;
				this.glueToItemDistance = options.GlueToItemDistance;
				this.snapToGrid = options.SnapToGrid;
				this.snapToItems = options.SnapToItems;
				this.snapToItemsDistance = options.SnapToItemsDistance;
				this.resizingMode = options.ResizingMode;
				this.minDragDistance = options.MinDragDistance;
				this.activeTool = options.ActiveTool;
				this.useTabNavigation = options.UseTabNavigation;
				this.skinGallery = options.SkinGallery;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public enum DocumentCapability {
		Default = 0,
		Disabled = 1,
		Enabled = 2,
		Hidden = 3
	}
	public class DiagramBehaviorOptionChangedEventArgs : BaseOptionChangedEventArgs {
		readonly bool activeToolChanged;
		public DiagramBehaviorOptionChangedEventArgs(string name, object oldValue, object newValue, bool activeToolChanged = false) : base(name, oldValue, newValue) {
			this.activeToolChanged = activeToolChanged;
		}
		public bool IsActiveToolChanged { get { return activeToolChanged; } }
	}
}
