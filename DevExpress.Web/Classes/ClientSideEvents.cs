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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter)),
	Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class ClientSideEventsBase : PropertiesBase {
		private static Dictionary<Type, string[]> eventNames = new Dictionary<Type, string[]>();
		public ClientSideEventsBase() {
		}
		public ClientSideEventsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		private string[] EventNames {
			get {
				string[] names;
				if(!eventNames.TryGetValue(GetType(), out names)) {
					names = GetEventNames().ToArray();
					lock(eventNames)
						eventNames[GetType()] = names;
				}
				return names;
			}
		}
		public sealed override void Assign(PropertiesBase source) {
			ClientSideEvents clientSideEvents = source as ClientSideEvents;
			if(clientSideEvents != null) {
			foreach(string eventName in EventNames) {
				this.SetEventHandler(eventName, clientSideEvents.GetEventHandler(eventName));
			}
		}
		}
		public virtual string GetStartupScript(string objectVarName) {
			StringBuilder sb = new StringBuilder();
			foreach(string eventName in EventNames) {
				sb.Append(GetStartupScript(objectVarName, eventName));
			}
			return sb.ToString();
		}
		public string GetStartupScript(string objectVarName, string eventName) {
			return GetStartupScript(objectVarName, eventName, eventName);
		}
		public string GetStartupScript(string objectVarName, string eventName, string jsEventName) {
			if(GetEventHandler(eventName).Length > 0)
				return string.Format("{0}.{1}.AddHandler({2});\n", objectVarName, jsEventName, GetEventHandler(eventName));
			else
				return string.Empty;
		}
		public bool IsEmpty() {
			foreach(string eventName in EventNames) {
				if(GetEventHandler(eventName).Length > 0) 
					return false;
			}
			return true;
		}
		public override string ToString() {
			return string.Empty;
		}
		public string GetEventHandler(string eventName) {
			return GetStringProperty(eventName, "");
		}
		public void SetEventHandler(string eventName, string value) {
			SetStringProperty(eventName, "", value);
		}
		private List<string> GetEventNames() {
			List<string> eventNames = new List<string>();
			AddEventNames(eventNames);
			return eventNames;
		}
		protected virtual void AddEventNames(List<string> eventNames) {
		}
	}
	public class ClientSideEvents : ClientSideEventsBase {
		private const string InitEventName = "Init";
		public ClientSideEvents() {
		}
		public ClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ClientSideEventsInit"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Init {
			get { return GetEventHandler(InitEventName); }
			set { SetEventHandler(InitEventName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			names.Add(InitEventName);
		}
	}
	public class CallbackClientSideEventsBase : ClientSideEvents {
		[
#if !SL
	DevExpressWebLocalizedDescription("CallbackClientSideEventsBaseBeginCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BeginCallback {
			get { return GetEventHandler("BeginCallback"); }
			set { SetEventHandler("BeginCallback", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CallbackClientSideEventsBaseEndCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string EndCallback {
			get { return GetEventHandler("EndCallback"); }
			set { SetEventHandler("EndCallback", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CallbackClientSideEventsBaseCallbackError"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CallbackError {
			get { return GetEventHandler("CallbackError"); }
			set { SetEventHandler("CallbackError", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		protected virtual string CustomDataCallback {
			get { return GetEventHandler("CustomDataCallback"); }
			set { SetEventHandler("CustomDataCallback", value); }
		}
		public CallbackClientSideEventsBase() {
		}
		public CallbackClientSideEventsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("BeginCallback");
			names.Add("EndCallback");
			names.Add("CallbackError");
			names.Add("CustomDataCallback");
		}
	}
	public class ItemClickClientSideEvents : ClientSideEvents {
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemClickClientSideEventsItemClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemClick {
			get { return GetEventHandler("ItemClick"); }
			set { SetEventHandler("ItemClick", value); }
		}
		public ItemClickClientSideEvents() {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ItemClick");
		}
	}
}
