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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.XtraEditors.ButtonsPanelControl {
	public interface IButtonsPanelControlAppearanceProvider : ISupportObjectChanged, IDisposable {
		AppearanceObject Normal { get; }
		AppearanceObject Hovered { get; }
		AppearanceObject Pressed { get; }
	}
	public class ButtonsPanelControlAppearanceProvider : BaseAppearanceCollection, IButtonsPanelControlAppearanceProvider {
		AppearanceObject normalCore;
		AppearanceObject hoveredCore;
		AppearanceObject pressedCore;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			normalCore = CreateAppearance("Normal");
			hoveredCore = CreateAppearance("Hovered");
			pressedCore = CreateAppearance("Pressed");
		}
		public AppearanceObject Normal { get { return normalCore; } }
		public AppearanceObject Hovered { get { return hoveredCore; } }
		public AppearanceObject Pressed { get { return pressedCore; } }
	}
	public class ButtonsPanelControlAppearance : BaseOwnerAppearance {
		IButtonsPanelControlAppearanceProvider ownerCollection;
		public ButtonsPanelControlAppearance(IButtonPanelControlAppearanceOwner owner)
			: base(owner) {
			ownerCollection = owner != null ? owner.CreateAppearanceProvider() : null;
			SubscribeCollection();
		}
		protected void SubscribeCollection() {
			if(ownerCollection == null) return;
			Normal.Changed += new EventHandler(OnApperanceChanged);
			Hovered.Changed += new EventHandler(OnApperanceChanged);
			Pressed.Changed += new EventHandler(OnApperanceChanged);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject Normal {
			get { return ownerCollection.Normal; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Hovered {
			get { return ownerCollection.Hovered; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Pressed {
			get { return ownerCollection.Pressed; }
		}
		void ResetNormal() { Normal.Reset(); }
		void ResetHovered() { Hovered.Reset(); }
		void ResetPressed() { Pressed.Reset(); }
		bool ShouldSerializeNormal() { return Normal.ShouldSerialize(); }
		bool ShouldSerializeHovered() { return Hovered.ShouldSerialize(); }
		bool ShouldSerializePressed() { return Pressed.ShouldSerialize(); }
		protected override void OnResetCore() {
			ResetNormal();
			ResetHovered();
			ResetPressed();
		}
		public override void Dispose() {
			base.Dispose();
			if(ownerCollection != null) {
				Normal.Changed -= new EventHandler(OnApperanceChanged);
				Hovered.Changed -= new EventHandler(OnApperanceChanged);
				Pressed.Changed -= new EventHandler(OnApperanceChanged);
			}
			DestroyAppearance(Normal);
			DestroyAppearance(Hovered);
			DestroyAppearance(Pressed);
		}
		public override string ToString() {
			return "AppearanceButton";
		}
	}
}
