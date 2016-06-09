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

using DevExpress.Utils;
using System;
using System.ComponentModel;
namespace DevExpress.XtraToolbox {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ToolboxAppearance : BaseAppearanceCollection {
		public ToolboxAppearance() : base() { }
		public ToolboxAppearance(ToolboxControl ownerControl)
			: base() {
			this.ownerControl = ownerControl;
			this.item = CreateElementAppearance();
			this.group = CreateElementAppearance();
		}
		ToolboxControl ownerControl;
		AppearanceObject toolbox;
		ToolboxElementAppearance item, group;
		protected override void CreateAppearances() {
			this.toolbox = CreateAppearance("Toolbox");
			Toolbox.Changed += OnChanged;
		}
		protected ToolboxElementAppearance CreateElementAppearance() {
			return new ToolboxElementAppearance(this.ownerControl);
		}
		void ResetToolbox() { Toolbox.Reset(); }
		bool ShouldSerializeToolbox() { return Toolbox.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Toolbox { get { return toolbox; } }
		void ResetItem() { Item.Reset(); }
		bool ShouldSerializeItem() { return Item.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ToolboxElementAppearance Item { get { return item; } }
		void ResetGroup() { Group.Reset(); }
		bool ShouldSerializeGroup() { return Group.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ToolboxElementAppearance Group { get { return group; } }
		public override bool ShouldSerialize() {
			return base.ShouldSerialize() || Item.ShouldSerialize() || Group.ShouldSerialize();
		}
		public override void Reset() {
			base.Reset();
			Item.Reset();
			Group.Reset();
		}
		public override string ToString() {
			return string.Empty;
		}
		public override void Dispose() {
			if(Item != null)
				Item.Dispose();
			if(Group != null)
				Group.Dispose();
			DisposeAppearanceObject(Toolbox);
			base.Dispose();
		}
		void DisposeAppearanceObject(AppearanceObject app) {
			if(app == null)
				return;
			app.Changed -= OnChanged;
			app.Dispose();
		}
		protected void OnChanged(object sender, EventArgs e) {
			if(this.ownerControl != null)
				this.ownerControl.OnAppearanceChanged();
		}
		public ToolboxAppearance Clone() {
			ToolboxAppearance app = new ToolboxAppearance();
			app.toolbox = (AppearanceObject)Toolbox.Clone();
			app.group = Group.Clone();
			app.item = Item.Clone();
			return app;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ToolboxElementAppearance : BaseAppearanceCollection {
		public ToolboxElementAppearance() : base() { }
		public ToolboxElementAppearance(ToolboxControl ownerControl)
			: base() {
			this.ownerControl = ownerControl;
		}
		public ToolboxElementAppearance(ToolboxElementBase element)
			: base() {
			this.element = element;
		}
		ToolboxElementBase element;
		IToolboxControl ownerControl;
		IToolboxControl OwnerControl {
			get {
				if(this.element != null)
					return this.element.Owner;
				return this.ownerControl;
			}
		}
		AppearanceObject normal, hovered, pressed, disabled;
		protected override void CreateAppearances() {
			this.normal = CreateAppearance("Normal");
			this.hovered = CreateAppearance("Hovered");
			this.pressed = CreateAppearance("Pressed");
			this.disabled = CreateAppearance("Disabled");
			Normal.Changed += OnChanged;
			Hovered.Changed += OnChanged;
			Pressed.Changed += OnChanged;
			Disabled.Changed += OnChanged;
		}
		protected void OnChanged(object sender, EventArgs e) {
			if(OwnerControl == null) return;
			OwnerControl.LayoutChanged();
		}
		void ResetNormal() { Normal.Reset(); }
		bool ShouldSerializeNormal() { return Normal.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Normal { get { return normal; } }
		void ResetHovered() { Hovered.Reset(); }
		bool ShouldSerializeHovered() { return Hovered.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Hovered { get { return hovered; } }
		void ResetPressed() { Pressed.Reset(); }
		bool ShouldSerializePressed() { return Pressed.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Pressed { get { return pressed; } }
		void ResetDisabled() { Disabled.Reset(); }
		bool ShouldSerializeDisabled() { return Disabled.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Disabled { get { return disabled; } }
		public override string ToString() {
			return string.Empty;
		}
		public override void Dispose() {
			DisposeAppearanceObject(Normal);
			DisposeAppearanceObject(Pressed);
			DisposeAppearanceObject(Hovered);
			DisposeAppearanceObject(Disabled);
			base.Dispose();
		}
		void DisposeAppearanceObject(AppearanceObject app) {
			if(app == null)
				return;
			app.Changed -= OnChanged;
			app.Dispose();
		}
		public ToolboxElementAppearance Clone() {
			ToolboxElementAppearance app = new ToolboxElementAppearance();
			app.disabled = (AppearanceObject)Disabled.Clone();
			app.hovered = (AppearanceObject)Hovered.Clone();
			app.normal = (AppearanceObject)Normal.Clone();
			app.disabled = (AppearanceObject)Disabled.Clone();
			return app;
		}
	}
}
