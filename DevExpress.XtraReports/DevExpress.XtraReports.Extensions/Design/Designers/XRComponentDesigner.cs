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
using System.Text;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Design.Commands;
namespace DevExpress.XtraReports.Design {
	public class XRComponentDesigner : XRComponentDesignerBase {
		DesignerActionListCollection actionLists;
		ReportCommandService reportCommandService;
		protected IDesignerHost fDesignerHost;
		protected IComponentChangeService changeService;
		#region Design style properties
		protected static readonly string[] filterPropertyNames = new string[] {
			XRComponentPropertyNames.StylePriority,
			XRComponentPropertyNames.DataBindings, 
			XRComponentPropertyNames.DynamicProperties, 
			XRComponentPropertyNames.BorderSide, 
			XRComponentPropertyNames.Tag, 
			XRComponentPropertyNames.ParentStyleUsing, 
			XRComponentPropertyNames.TextAlign,
			XRComponentPropertyNames.Text,
			XRComponentPropertyNames.WordWrap,
			XRComponentPropertyNames.NavigateUrl,
			XRComponentPropertyNames.Target,
			XRComponentPropertyNames.Dock,};
#if DEBUGTEST
		public
#else
		internal
#endif
 static readonly string[] stylePropertyNames = new string[] {
			XRComponentPropertyNames.BackColor,
			XRComponentPropertyNames.BorderColor, 
			XRComponentPropertyNames.Borders,
			XRComponentPropertyNames.BorderDashStyle,
			XRComponentPropertyNames.BorderWidth,
			XRComponentPropertyNames.Font,
			XRComponentPropertyNames.ForeColor,
			XRComponentPropertyNames.Padding,
			XRComponentPropertyNames.TextAlignment,
		};
		const string designPropertyPostfix = "_";
		public Color BackColor_ {
			get { return XRControl.GetEffectiveBackColor(); }
			set { XRControl.BackColor = value; }
		}
		public Color BorderColor_ {
			get { return XRControl.GetEffectiveBorderColor(); }
			set { XRControl.BorderColor = value; }
		}
		public BorderSide Borders_ {
			get { return XRControl.GetEffectiveBorders(); }
			set { XRControl.Borders = value; }
		}
		public BorderDashStyle BorderDashStyle_ {
			get { return XRControl.GetEffectiveBorderDashStyle(); }
			set { XRControl.BorderDashStyle = value; }
		}
		public float BorderWidth_ {
			get { return XRControl.GetEffectiveBorderWidth(); }
			set { XRControl.BorderWidth = value; }
		}
		public Font Font_ {
			get { return XRControl.GetEffectiveFont(); }
			set { XRControl.Font = value; }
		}
		public Color ForeColor_ {
			get { return XRControl.GetEffectiveForeColor(); }
			set { XRControl.ForeColor = value; }
		}
		public PaddingInfo Padding_ {
			get { return XRControl.GetEffectivePadding(); }
			set { XRControl.Padding = value; }
		}
		public TextAlignment TextAlignment_ {
			get { return XRControl.GetEffectiveTextAlignment(); }
			set { XRControl.TextAlignment = value; }
		}
		#endregion
		protected internal XRControl XRControl {
			get { return Component as XRControl; }
		}
		public virtual bool CanDrag {
			get { return true; }
		}
		public virtual bool CanDragInReportExplorer {
			get { return true; }
		}
		public override DesignerActionListCollection ActionLists {
			get {
				if(actionLists == null)
					actionLists = CreateActionLists();
				return actionLists;
			}
		}
		public XRComponentDesigner()
			: base() {
		}
		public bool IsEUD {
			get {
				return DesignToolHelper.IsEndUserDesigner(fDesignerHost);
			}
		}
		protected virtual bool TryGetCollectionName(IComponent c, out string name) {
			if(c is XRControl) {
				name = ReportDesignerHelper.GetDefaultCollectionName(Component);
				return true;
			} else {
				name = string.Empty;
				return false;
			}
		}
		public DesignerVerb GetDesignerVerb(string text) {
			for(int i = 0; i < Verbs.Count; i++) {
				if(Verbs[i].Text == text)
					return Verbs[i];
			}
			return null;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			fDesignerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			changeService = (IComponentChangeService)fDesignerHost.GetService(typeof(IComponentChangeService));
			reportCommandService = fDesignerHost.GetService(typeof(ReportCommandService)) as ReportCommandService;
			if(reportCommandService != null) {
				reportCommandService.CommandChanged += new ReportCommandEventHandler(OnCommandChanged);
				UpdateVerbsAndActions();
			}
		}
		protected internal virtual int AddComponent(IComponent c) {
			if(!(c is XRControl)) throw new InvalidOperationException();
			return XRControl.Controls.Add((XRControl)c);
		}
		protected internal virtual void OnCollectionChanging(IComponent c) {
			string name;
			if(TryGetCollectionName(c, out name))
				changeService.OnComponentChanging(Component, XRAccessor.GetPropertyDescriptor(Component, name));
		}
		protected internal virtual void OnCollectionChanged(IComponent c) {
			string name;
			if(TryGetCollectionName(c, out name))
				changeService.OnComponentChanged(Component, XRAccessor.GetPropertyDescriptor(Component, name), null, null);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(reportCommandService != null)
					reportCommandService.CommandChanged -= new ReportCommandEventHandler(OnCommandChanged);
			}
			base.Dispose(disposing);
		}
		protected internal virtual void OnContextMenu(int x, int y) {
			IMenuCommandService menuService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(menuService != null)
				menuService.ShowContextMenu(MenuCommandServiceCommands.SelectionMenu, x, y);
		}
		protected virtual DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection collection = new DesignerActionListCollection();
			RegisterActionLists(collection);
			return collection;
		}
		protected virtual void RegisterActionLists(DesignerActionListCollection list) {
		}
		protected DesignerVerb CreateXRDesignerVerb(string text, ReportCommand reportCommand) {
			return CreateXRDesignerVerb(text, reportCommand, true, true);
		}
		protected DesignerVerb CreateXRDesignerVerb(string text, ReportCommand reportCommand, bool enabled, bool includeInSmartTag) {
			return new XRDesignerVerb(text, OnXRDesignerVerbInvoke, reportCommand, enabled, includeInSmartTag);
		}
		void OnCommandChanged(object sender, ReportCommandEventArgs e) {
			UpdateVerbsAndActions();
		}
		void UpdateVerbsAndActions() {
			if(reportCommandService == null || Verbs == null)
				return;
			foreach(DesignerVerb verb in Verbs) {
				if(verb is XRDesignerVerb) {
					UserDesigner.CommandVisibility visibility = reportCommandService.GetCommandVisibility(CommandIDReportCommandConverter.GetReportCommand(((XRDesignerVerb)verb).CommandID));
					verb.Visible = ((visibility & UserDesigner.CommandVisibility.Verb) > 0);
				}
			}
		}
		public virtual void OnComponentChanged(ComponentChangedEventArgs e) {
			string propertyName = e.Member != null ? e.Member.Name : "";
			foreach(string stylePropertyName in stylePropertyNames)
				if(propertyName == GetStylePropertyNameCore(stylePropertyName)) {
					PropInfoAccessor.SetPropertyValue(((XRControl)e.Component).StylePriority, "Use" + stylePropertyName, false);
					return;
				}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			foreach(string stylePropertyName in stylePropertyNames) {
				if(properties.Contains(stylePropertyName))
					AddDesignStyleProperty(properties, stylePropertyName);
			}
		}
		void AddDesignStyleProperty(IDictionary properties, string propertyName) {
			string designPropertyName = GetStylePropertyName(propertyName);
			PropertyDescriptor designPropertyDescriptor = PropInfoAccessor.CreateProperty(this, designPropertyName, 
				GetAttributes(properties, propertyName, new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)));
			if(designPropertyDescriptor != null)
				properties[designPropertyName] = designPropertyDescriptor;
			HideOriginalProperty(properties, propertyName, designPropertyDescriptor.PropertyType);
		}
		void HideOriginalProperty(IDictionary properties, string propertyName, Type propertyType) {
			PropertyDescriptor propertyDescriptor = PropInfoAccessor.CreateProperty(Component, propertyName, propertyType, 
				GetAttributes(properties, propertyName, new BrowsableAttribute(false)));
			if(propertyDescriptor != null)
				properties[propertyName] = propertyDescriptor;
		}
		static public string GetStylePropertyName(string propertyName) {
			if(Array.IndexOf(stylePropertyNames, propertyName) >= 0)
				return GetStylePropertyNameCore(propertyName);
			return propertyName;
		}
		static string GetStylePropertyNameCore(string propertyName) {
			return propertyName + designPropertyPostfix;
		}
		protected Attribute[] GetAttributes(IDictionary properties, string propertyName, Attribute newAttribute) {
			List<Attribute> attributes = new List<Attribute>();
			attributes.Add(newAttribute);
			foreach(Attribute attribute in ((PropertyDescriptor)properties[propertyName]).Attributes)
				if(attribute.GetType() != newAttribute.GetType())
					attributes.Add(attribute);
			return attributes.ToArray();
		}
		#region Reset design style properties
		protected void ResetBackColor_() {
			XRControl.ResetBackColor();
		}
		protected void ResetBorderColor_() {
			XRControl.ResetBorderColor();
		}
		protected void ResetBorders_() {
			XRControl.ResetBorders();
		}
		protected void ResetBorderDashStyle_() {
			XRControl.ResetBorderDashStyle();
		}
		protected void ResetBorderWidth_() {
			XRControl.ResetBorderWidth();
		}
		protected void ResetFont_() {
			XRControl.ResetFont();
		}
		protected void ResetForeColor_() {
			XRControl.ResetForeColor();
		}
		protected void ResetPadding_() {
			XRControl.ResetPadding();
		}
		protected void ResetTextAlignment_() {
			XRControl.ResetTextAlignment();
		}
		#endregion
		#region ShouldSerialize style properties
		protected bool ShouldSerializeBackColor_() {
			return XRControl.ShouldSerializeBackColor();
		}
		protected bool ShouldSerializeBorderColor_() {
			return XRControl.ShouldSerializeBorderColor();
		}
		protected bool ShouldSerializeBorders_() {
			return XRControl.ShouldSerializeBorders();
		}
		protected bool ShouldSerializeBorderDashStyle_() {
			return XRControl.ShouldSerializeBorderDashStyle();
		}
		protected bool ShouldSerializeBorderWidth_() {
			return XRControl.ShouldSerializeBorderWidth();
		}
		protected bool ShouldSerializeFont_() {
			return XRControl.ShouldSerializeFont();
		}
		protected bool ShouldSerializeForeColor_() {
			return XRControl.ShouldSerializeForeColor();
		}
		protected bool ShouldSerializePadding_() {
			return XRControl.ShouldSerializePadding();
		}
		protected bool ShouldSerializeTextAlignment_() {
			return XRControl.ShouldSerializeTextAlignment();
		}
		#endregion
		protected internal virtual bool CanAddBand(BandKind bandKind) {
			return false;
		}
		public virtual IEnumerator GetEnumerator() {
			return ((IEnumerable)this.XRControl).GetEnumerator();
		}
		public virtual RectangleF GetBounds(Band band, GraphicsUnit unit) {
			return this.XRControl.GetBounds(band, unit);
		}	
	}
}
