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

using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler {
	public class ViewNavigatorStyles : StylesBase {
		public const string ButtonStyleName = "Button";
		public ViewNavigatorStyles(ISkinOwner owner)
			: base(owner) {			
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ButtonControlStyle Button {
			get { return (ButtonControlStyle)GetStyle(ButtonStyleName); }
		}
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ButtonSpacing {
			get { return GetUnitProperty("ButtonSpacing", Unit.Empty); }
			set { SetUnitProperty("ButtonSpacing", Unit.Empty, value); }
		}
		protected override string GetCssClassNamePrefix() {
			return "dxsc";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ButtonStyleName, delegate() { return new ButtonControlStyle(); }));
		}
		protected internal new AppearanceStyleBase GetDefaultControlStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ViewNavigator));
			return style;
		}
		protected internal ButtonControlStyle GetButtonStyle() {
			ButtonControlStyle style = new ButtonControlStyle();
			style.CopyFrom(Button);
			return style;
		}		
		protected internal Unit GetButtonSpacing() {
			return ButtonSpacing.IsEmpty ? 1 : ButtonSpacing;
		}
		public override void Reset() {
			base.Reset();
			ButtonSpacing = Unit.Empty;
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			ViewNavigatorStyles src = source as ViewNavigatorStyles;
			if(src != null) {
				ButtonSpacing = src.ButtonSpacing;
			}			
		}		
	}
}
