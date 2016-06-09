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
using DevExpress.Web.ASPxGauges.Base;
using System.Collections;
namespace DevExpress.Web.ASPxGauges.Gauges {
	public class BaseGaugeBuilder : BaseComponentBuilder<BaseGaugeWeb> {
		protected override bool AllowFilterProperties { get { return false; } }
		protected override void PreFilterAttributes(IDictionary attributes) {
			CheckNameAndID(attributes);
		}
		protected void CheckNameAndID(IDictionary attributes) {
			object propName = "Name";
			object propValue = null; 
			object id = null;
			IDictionary propertiesToRemove = new Hashtable();
			foreach(DictionaryEntry entry in attributes) {
				string key = (string)entry.Key;
				if(string.Equals(key,(string)propName)) {
					propValue = entry.Value;
					continue;
				}
				if(string.Equals(key,"ID")) {
					id = entry.Value;
					propertiesToRemove.Add(key, id);
					continue;
				}
			}
			ClearAttributes(attributes, propertiesToRemove);
			propValue = (propValue != null) ? propValue : id;
			if(!attributes.Contains(propName))
				attributes.Add(propName, propValue);
			else
				attributes[propName] = propValue;
		}
		void ClearAttributes(IDictionary attributes, IDictionary propertiesToRemove) {
			foreach(DictionaryEntry entry in propertiesToRemove) 
				attributes.Remove(entry.Key);
		}
	}
	public class LabelComponentBuilder : BaseComponentBuilder<LabelComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"Position","Size","FormatString","AllowHTMLString",
				"Text","TextOrientation",
				"AppearanceBackground-BorderBrush","AppearanceBackground-BorderWidth","AppearanceBackground-ContentBrush",
				"AppearanceText-Font","AppearanceText-Format","AppearanceText-Spacing","AppearanceText-TextBrush"
			};
		}
	}
}
