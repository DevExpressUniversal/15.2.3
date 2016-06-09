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
using System.Drawing.Design;
using System.Text.RegularExpressions;
using DevExpress.Web;
namespace DevExpress.Web.ASPxGauges {
	public class CustomCallbackClientSideEvents : ClientSideEvents {
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("BeginCallback");
			names.Add("EndCallback");
			names.Add("CallbackError");
		}
		[Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor)), NotifyParentProperty(true), 
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("CustomCallbackClientSideEventsBeginCallback"),
#endif
 DefaultValue("")]
		public string BeginCallback {
			get { return GetEventHandler("BeginCallback"); }
			set { SetEventHandler("BeginCallback", value); }
		}
		[Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor)), NotifyParentProperty(true), 
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("CustomCallbackClientSideEventsCallbackError"),
#endif
 DefaultValue("")]
		public string CallbackError {
			get { return GetEventHandler("CallbackError"); }
			set { SetEventHandler("CallbackError", value); }
		}
		[Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor)), NotifyParentProperty(true), 
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("CustomCallbackClientSideEventsEndCallback"),
#endif
 DefaultValue("")]
		public string EndCallback {
			get { return GetEventHandler("EndCallback"); }
			set { SetEventHandler("EndCallback", value); }
		}
	}
	public abstract class BasePropertyChangedHandler {
		public delegate void PropertyChangedAction(string propValue);
		Dictionary<string, PropertyChangedAction> actionsCore;
		public BasePropertyChangedHandler() {
			actionsCore = new Dictionary<string, PropertyChangedAction>();
			CreateActions();
		}
		public void ProcessPropertyChanged(string propName, string propValue) {
			PropertyChangedAction action = null;
			if(actionsCore.TryGetValue(propName, out action)) action(propValue);
		}
		protected Dictionary<string, PropertyChangedAction> Actions {
			get { return actionsCore; }
		}
		protected abstract void CreateActions();
		protected int GetIntValue(string value, int defaultValue) {
			int result = defaultValue;
			int.TryParse(value, out result);
			return result;
		}
		protected float GetFloatValue(string value, float defaultValue) {
			float result = defaultValue;
			float.TryParse(value, out result);
			return result;
		}
	}
	public static class CustomCallbackParameterParser {
		public static bool TryParse(CallbackEventArgsBase e, out string propName, out string propValue) {
			string pattern = "(?<propName>.*?)\\s*=\\s*(?<propValue>.*?)$";
			MatchCollection matches = Regex.Matches(e.Parameter, pattern);
			propName = propValue = null;
			if(matches.Count > 0) {
				propName = matches[0].Groups["propName"].Value;
				propValue = matches[0].Groups["propValue"].Value;
			}
			return propName != null;
		}
	}
}
