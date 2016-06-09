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

using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid.Views.WinExplorer {
	public class WinExplorerViewAppearances : ColumnViewAppearances {
		AppearanceObject itemNormal, itemHovered, itemPressed, itemDisabled;
		AppearanceObject itemDescriptionNormal, itemDescriptionHovered, itemDescriptionPressed, itemDescriptionDisabled;
		AppearanceObject groupNormal, groupHovered, groupPressed;
		AppearanceObject emptySpace;
		public WinExplorerViewAppearances(BaseView view) : base(view) { }
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.itemNormal = CreateAppearance("ItemNormal");
			this.itemHovered = CreateAppearance("ItemHovered");
			this.itemPressed = CreateAppearance("ItemPressed");
			this.itemDisabled = CreateAppearance("ItemDisabled");
			this.itemDescriptionNormal = CreateAppearance("ItemDescriptionNormal");
			this.itemDescriptionHovered = CreateAppearance("ItemDescriptionHovered");
			this.itemDescriptionPressed = CreateAppearance("ItemDescriptionPressed");
			this.itemDescriptionDisabled = CreateAppearance("ItemDescriptionDisabled");
			this.groupNormal = CreateAppearance("GroupNormal");
			this.groupHovered = CreateAppearance("GroupHovered");
			this.groupPressed = CreateAppearance("GroupPressed");
			this.emptySpace = CreateAppearance("EmptySpace");
		}
		void ResetItemNormal() { ItemNormal.Reset(); }
		bool ShouldSerializeItemNormal() { return ItemNormal.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemNormal { get { return itemNormal; } }
		void ResetItemDisabled() { ItemDisabled.Reset(); }
		bool ShouldSerializeItemDisabled() { return ItemDisabled.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemDisabled { get { return itemDisabled; } }
		void ResetItemHovered() { ItemHovered.Reset(); }
		bool ShouldSerializeItemHovered() { return ItemHovered.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemHovered { get { return itemHovered; } }
		void ResetItemPressed() { ItemPressed.Reset(); }
		bool ShouldSerializeItemPressed() { return ItemPressed.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemPressed { get { return itemPressed; } }
		void ResetItemDescriptionNormal() { ItemDescriptionNormal.Reset(); }
		bool ShouldSerializeItemDescriptionNormal() { return ItemDescriptionNormal.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemDescriptionNormal { get { return itemDescriptionNormal; } }
		void ResetItemDescriptionHovered() { ItemDescriptionHovered.Reset(); }
		bool ShouldSerializeItemDescriptionHovered() { return ItemDescriptionHovered.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemDescriptionHovered { get { return itemDescriptionHovered; } }
		void ResetItemDescriptionPressed() { ItemDescriptionPressed.Reset(); }
		bool ShouldSerializeItemDescriptionPressed() { return ItemDescriptionPressed.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemDescriptionPressed { get { return itemDescriptionPressed; } }
		void ResetItemDescriptionDisabled() { ItemDescriptionDisabled.Reset(); }
		bool ShouldSerializeItemDescriptionDisabled() { return ItemDescriptionDisabled.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemDescriptionDisabled { get { return itemDescriptionDisabled; } }
		void ResetGroupNormal() { GroupNormal.Reset(); }
		bool ShouldSerializeGroupNormal() { return GroupNormal.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupNormal { get { return groupNormal; } }
		void ResetGroupHovered() { GroupHovered.Reset(); }
		bool ShouldSerializeGroupHovered() { return GroupHovered.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupHovered { get { return groupHovered; } }
		void ResetGroupPressed() { GroupPressed.Reset(); }
		bool ShouldSerializeGroupPressed() { return GroupPressed.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupPressed { get { return groupPressed; } }
		void ResetEmptySpace() { EmptySpace.Reset(); }
		bool ShouldSerializeEmptySpace() { return EmptySpace.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject EmptySpace { get { return emptySpace; } }
	}
}
