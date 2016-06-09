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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(true), NonVisualControl, DefaultProperty("ClientSideEvents"), DefaultEvent("CustomCallback"),
	Designer("DevExpress.Web.Design.ASPxHiddenFieldDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	ToolboxTabName(AssemblyInfo.DXTabComponents),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxHiddenField.bmp"),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxHiddenField")
	]
	public class ASPxHiddenField : ASPxWebComponent, IDictionary<string, object> {
		internal const string HiddenFieldScriptResourceName = WebScriptsResourcePath + "HiddenField.js";
		private const string InputElementIDSuffix = "I";
		private Dictionary<string, object> properties = new Dictionary<string, object>();
		private static readonly object CustomCallbackEvent = new object();
		public ASPxHiddenField()
			: base() {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object this[string propertyName] {
			get { return Properties[propertyName]; }
			set {
				HiddenFieldUtils.AssertPropertyNameIsValid(propertyName);
				if(Properties.ContainsKey(propertyName))
					Properties[propertyName] = value;
				else
					Properties.Add(propertyName, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHiddenFieldClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHiddenFieldClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public HiddenFieldClientSideEvents ClientSideEvents {
			get { return (HiddenFieldClientSideEvents)ClientSideEventsInternal; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Count {
			get { return Properties.Count; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableViewState {
			get { return false; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHiddenFieldSyncWithServer"),
#endif
		Category("Behavior"), DefaultValue(true), EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool SyncWithServer {
			get { return GetBoolProperty("SyncWithServer", true); }
			set { SetBoolProperty("SyncWithServer", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHiddenFieldCustomCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase CustomCallback
		{
			add { Events.AddHandler(CustomCallbackEvent, value); }
			remove { Events.RemoveHandler(CustomCallbackEvent, value); }
		}
		internal Dictionary<string, object> Properties {
			get { return properties; }
		}
		public void Add(string propertyName, object propertyValue) {
			Properties.Add(propertyName, propertyValue);
		}
		public void Clear() {
			Properties.Clear();
		}
		public bool Contains(string propertyName) {
			return Properties.ContainsKey(propertyName);
		}
		public object Get(string propertyName) {
			return this[propertyName];
		}
		public bool Remove(string propertyName) {
			return Properties.Remove(propertyName);
		}
		public void Set(string propertyName, object propertyValue) {
			this[propertyName] = propertyValue;
		}
		public bool TryGet(string propertyName, out object propertyValue) {
			return Properties.TryGetValue(propertyName, out propertyValue);
		}
		protected override void CreateControlHierarchy() {
			if(!DesignMode && SyncWithServer && IsEnabled() && !IsCallback) {
				Control field = RenderUtils.CreateHiddenFieldLiteral(UniqueID, "");
				Controls.Add(field);
			}
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new HiddenFieldClientSideEvents(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientHiddenField";
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected internal override bool IsCallBacksEnabled() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxHiddenField), HiddenFieldScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!SyncWithServer)
				stb.Append(localVarName + ".syncWithServer = false;\n");
			if(Properties.Count > 0) {
				SerializationResult serializationResult = HiddenFieldUtils.Serializer.SerializeToScript(Properties);
				stb.AppendFormat("{0}.properties = {1};\n", localVarName, serializationResult.PropertiesTree);
				if(SyncWithServer) {
					if(serializationResult.TypeInfoTable != "{}")
						stb.AppendFormat("{0}.typeInfoTable = {1};\n", localVarName, serializationResult.TypeInfoTable);
					if(serializationResult.TypeNameTable != "[]")
						stb.AppendFormat("{0}.typeNameTable = {1};\n", localVarName, serializationResult.TypeNameTable);
				}
			}
		}
		protected override object SaveViewState() {
			return null;
		}
		protected override void LoadViewState(object savedState) {
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents() && Events[CustomCallbackEvent] != null;
		}
		protected virtual void OnCustomCallback(CallbackEventArgsBase e) {
			CallbackEventHandlerBase handler = (CallbackEventHandlerBase)Events[CustomCallbackEvent];
			if(handler != null)
				handler(this, e);
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			CallbackEventArgsBase args = new CallbackEventArgsBase(eventArgument);
			OnCustomCallback(args);
		}
		protected override object GetCallbackResult() {
			return RenderUtils.GetRenderResult(this);
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			return false;
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(SyncWithServer)
				LoadPostDataFromRequest();
		}
		private void LoadPostDataFromRequest() {
			if(Request == null || Request.Params == null) return;
			if(ClientObjectState == null) return;
			string data = GetClientObjectStateValue<string>("data");
			this.properties = HiddenFieldUtils.Deserializer.Deserialize(data);
		}
		#region IDictionary<string,object> Members
		ICollection<string> IDictionary<string, object>.Keys {
			get { return Properties.Keys; }
		}
		ICollection<object> IDictionary<string, object>.Values {
			get { return Properties.Values; }
		}
		void IDictionary<string, object>.Add(string key, object value) {
			Set(key, value);
		}
		bool IDictionary<string, object>.ContainsKey(string key) {
			return Contains(key);
		}
		bool IDictionary<string, object>.TryGetValue(string key, out object value) {
			return TryGet(key, out value);
		}
		#endregion
		#region ICollection<KeyValuePair<string,object>> Members
		bool ICollection<KeyValuePair<string, object>>.IsReadOnly {
			get { return false; }
		}
		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) {
			Set(item.Key, item.Value);
		}
		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) {
			object actualValue;
			return TryGet(item.Key, out actualValue) ? object.Equals(actualValue, item.Value) : false;
		}
		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
			(Properties as ICollection<KeyValuePair<string, object>>).CopyTo(array, arrayIndex);
		}
		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) {
			object actualValue;
			if(TryGet(item.Key, out actualValue) && object.Equals(actualValue, item.Value)) {
				Remove(item.Key);
				return true;
			}
			return false;
		}
		#endregion
		#region IEnumerable / IEnumerable<KeyValuePair<string,object>> Members
		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
			return (Properties as IEnumerable<KeyValuePair<string, object>>).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (Properties as IEnumerable).GetEnumerator();
		}
		#endregion
	}
}
