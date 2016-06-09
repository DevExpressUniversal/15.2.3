#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.Drawing;
using DevExpress.Xpo.Metadata;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.Data.Filtering;
namespace DevExpress.ExpressApp.StateMachine.Xpo {
	[Appearance("XpoStateAppearance.AppearanceForAction", TargetItems = "BackColor; FontColor; FontStyle", Enabled = false, Criteria = "AppearanceItemType='Action'")]
	[System.ComponentModel.DisplayName("Appearance")]
	[ImageName("BO_Appearance")]
	public class XpoStateAppearance : StateMachineObjectBase, IAppearanceRuleProperties {
		private XpoState state;
		private System.Drawing.Color? backColor;
		private bool? enabled;
		private System.Drawing.Color? fontColor;
		private System.Drawing.FontStyle? fontStyle;
		private int priority;
		private ViewItemVisibility? visibility;
		private string appearanceItemType = "ViewItem";
		private string context = "Any";
		private string method;
		private string targetItems;
		public XpoStateAppearance(Session session)
			: base(session) {
		}
		[Browsable(false)]
		[Association("State-AppearanceDescriptions")]
		public XpoState State {
			get { return state; }
			set { SetPropertyValue("State", ref state, value); }
		}
		[MemberDesignTimeVisibility(false)]
		[Browsable(false)]
		public Type TargetObjectType {
			get { return State == null || State.StateMachine == null ? null : State.StateMachine.TargetObjectType; }
		}
		#region IAppearanceRuleProperties Members
		[ImmediatePostData]
		public string AppearanceItemType {
			get { return appearanceItemType; }
			set { SetPropertyValue("AppearanceItemType", ref appearanceItemType, value); }
		}
		public string Context {
			get { return context; }
			set { SetPropertyValue("Context", ref context, value); }
		}
		[Browsable(false)]
		[Size(SizeAttribute.Unlimited)]
		public string Criteria {
			get {
				if(State != null && State.StateMachine != null && State.StateMachine.StatePropertyName != null && State.Marker != null && State.Marker.Marker != null) {
					return new BinaryOperator(State.StateMachine.StatePropertyName.Name, State.Marker.Marker).ToString();
				} else {
					return "0=1";
				}
			}
			set { }
		}
		[Browsable(false)]
		public string Method {
			get { return method; }
			set { SetPropertyValue("Method", ref method, value); }
		}
		[RuleRequiredField("XpoStateAppearance.TargetItems", DefaultContexts.Save)]
		public string TargetItems {
			get { return targetItems; }
			set { SetPropertyValue("TargetItems", ref targetItems, value); }
		}
		[Browsable(false)]
		public Type DeclaringType { get { return TargetObjectType; } }
		#endregion
		#region IAppearance Members
		public int Priority {
			get { return priority; }
			set { SetPropertyValue("Priority", ref priority, value); }
		}
		[Persistent, ValueConverter(typeof(NullableColorConverter))]
		public System.Drawing.Color? FontColor {
			get { return fontColor; }
			set { SetPropertyValue("FontColor", ref fontColor, value); }
		}
		[Persistent, ValueConverter(typeof(NullableColorConverter))]
		public System.Drawing.Color? BackColor {
			get { return backColor; }
			set { SetPropertyValue("BackColor", ref backColor, value); }
		}
		public System.Drawing.FontStyle? FontStyle {
			get { return fontStyle; }
			set { SetPropertyValue("FontStyle", ref fontStyle, value); }
		}
		public bool? Enabled {
			get { return enabled; }
			set { SetPropertyValue("Enabled", ref enabled, value); }
		}
		public ViewItemVisibility? Visibility {
			get { return visibility; }
			set { SetPropertyValue("Visibility", ref visibility, value); }
		}
		#endregion
	}
	public class NullableColorConverter : ValueConverter {
		public override Type StorageType {
			get { return typeof(int); }
		}
		public override object ConvertToStorageType(object value) {
			if(((Color?)value).HasValue) {
				return (((Color?)value).Value).ToArgb();
			}
			return null;
		}
		public override object ConvertFromStorageType(object value) {
			if(value != null) {
				return Color.FromArgb((int)value);
			}
			return null;
		}
	}
}
