#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon {
	public abstract class FormatConditionStyleBase : FormatConditionBase {
		const string XmlStyleSettings = "StyleSettings";
		const string DefaultPredefinedName = "";
		StyleSettingsBase styleSettings;
		[
		DefaultValue(null)
		]
		public StyleSettingsBase StyleSettings { 
			get { return styleSettings; } 
			set {
				if(StyleSettings == value)
					return;
				if(value != null && value.Owner != null && value.Owner != this) {
					throw new InvalidOperationException("StyleSettings already has an Owner.");
				}
				if(HasStyleSettings)
					styleSettings.Owner = null;
				styleSettings = value;
				if(HasStyleSettings)
					styleSettings.Owner = this;
				OnChanged();
			} 
		}
		protected override IEnumerable<StyleSettingsBase> Styles { get { yield return StyleSettings; } }
		protected bool HasStyleSettings { get { return StyleSettings != null; } }
		public override bool IsValid { get { return base.IsValid && StyleSettings != null; } }
		protected FormatConditionStyleBase() {
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			ConditionalFormattingSerializer.Save(StyleSettings, element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			this.StyleSettings = ConditionalFormattingSerializer.LoadFirst<StyleSettingsBase>(element);
		}
		protected override void AssignCore(FormatConditionBase obj) {
			var source = obj as FormatConditionStyleBase;
			if (source != null && source.StyleSettings != null) {
				StyleSettings = (StyleSettingsBase)((IStyleSettings)source.StyleSettings).Clone(); 
			}
			else
				StyleSettings = null;
		}
		protected override IStyleSettings CalcStyleSettingCore(IFormatConditionValueProvider valueProvider) {
			return IsFitCore(valueProvider) ? StyleSettings : null;
		}
	}
}
