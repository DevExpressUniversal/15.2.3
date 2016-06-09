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
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
	public class AppearanceAttribute : Attribute, IAppearanceRuleProperties {
		private FontStyle? fontStyle;
		private Color? fontColor;
		private Color? backColor;
		private ViewItemVisibility? visibility;
		private bool? enabled;
		private int priority;
		public AppearanceAttribute(string id) {
			Id = id;
			Context = AppearanceController.AppearanceContextAny;
			AppearanceItemType = AppearanceController.AppearanceViewItemType;
		}
		public AppearanceAttribute(string id, string criteria)
			: this(id) {
			Criteria = criteria;
		}
		public AppearanceAttribute(string id, AppearanceItemType appearanceItemType, string criteria)
			: this(id, criteria) {
			AppearanceItemType = appearanceItemType.ToString();
		}
		public string Id { get; set; }
		public FontStyle FontStyle {
			get {
				if(fontStyle.HasValue) {
					return fontStyle.Value;
				}
				return FontStyle.Regular;
			}
			set { fontStyle = value; }
		}
		public string FontColor {
			get {
				if(fontColor.HasValue) {
					return fontColor.ToString();
				}
				return null;
			}
			set {
				try {
					fontColor = (Color)new ColorConverter().ConvertFrom(value);
				}
				catch {
					fontColor = (Color)new ColorConverter().ConvertFromInvariantString(value);
				}
			}
		}
		public string BackColor {
			get {
				if(backColor.HasValue) {
					return backColor.ToString();
				}
				return null;
			}
			set {
				try {
					backColor = (Color)new ColorConverter().ConvertFrom(value);
				}
				catch {
					backColor = (Color)new ColorConverter().ConvertFromInvariantString(value);
				}
			}
		}
		public ViewItemVisibility Visibility {
			get {
				if(visibility.HasValue) {
					return visibility.Value;
				}
				return ViewItemVisibility.Show;
			}
			set { visibility = value; }
		}
		public bool Enabled {
			get {
				if(enabled.HasValue) {
					return enabled.Value;
				}
				return true;
			}
			set { enabled = value; }
		}
		#region IAppearanceRuleProperties Members
		public string TargetItems { get; set; }
		public string AppearanceItemType { get; set; }
		public string Criteria { get; set; }
		public string Method { get; set; }
		public string Context { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type DeclaringType { get { return null; } }
		#endregion
		#region IAppearance Members
		public int Priority {
			get { return priority; }
			set { priority = value; }
		}
		FontStyle? IAppearance.FontStyle {
			get { return fontStyle; }
			set { fontStyle = value; }
		}
		Color? IAppearance.FontColor {
			get { return fontColor; }
			set { fontColor = value; }
		}
		Color? IAppearance.BackColor {
			get { return backColor; }
			set { backColor = value; }
		}
		ViewItemVisibility? IAppearance.Visibility {
			get { return visibility; }
			set { visibility = value; }
		}
		bool? IAppearance.Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
		#endregion
	}
}
