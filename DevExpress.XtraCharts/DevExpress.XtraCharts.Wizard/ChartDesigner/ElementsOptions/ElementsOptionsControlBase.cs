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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class ElementsOptionsControlBase : XtraUserControl, IView, IMessageListener{
		PropertyLinksCollection propertyLinks;
		List<IMessageListener> listeners;
		DesignerChartElementModelBase model;
		Dictionary<Control, PropertyLink> propertyLinkDictionary;
		DXCollection<TitleElement> titles;
		ActivityGraph graph;
		internal ActivityGraph Graph { get { return graph; } }
		internal DesignerChartElementModelBase Model { get { return model; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(CollectionTypeConverter))
		]
		public PropertyLinksCollection PropertyLinks {
			get { return this.propertyLinks; }
			set { propertyLinks = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(CollectionTypeConverter))
		]
		public DXCollection<TitleElement> Titles {
			get { return titles; }
			set { titles = value; }
		}
		public ElementsOptionsControlBase()
			: base() {
			propertyLinks = new PropertyLinksCollection();
			listeners = new List<IMessageListener>();
			propertyLinkDictionary = new Dictionary<Control, PropertyLink>();
			titles = new DXCollection<TitleElement>();
			InitializeComponent();
		}
		#region IView
		void IView.LoadModel(DesignerChartElementModelBase model) {
			LoadModel(model);
		}
		void IView.UpdateData() {
			UpdateView();
		}
		void IView.UpdateActivity(string parameter) {
			UpdateActivity(parameter);
		}
		void IView.AddListener(IMessageListener listener) {
			if (!listeners.Contains(listener))
				listeners.Add(listener);
		}
		void IView.RemoveListener(IMessageListener listener) {
			listeners.Remove(listener);
		}
		#endregion
		#region IMessageListener
		void IMessageListener.AcceptMessage(ViewMessage message) {
			List<IMessageListener> tempListeners = new List<IMessageListener>();
			tempListeners.AddRange(listeners);
			foreach (IMessageListener listener in tempListeners)
				listener.AcceptMessage(message);
		}
		#endregion
		#region ShouldSerialize
		bool ShouldSerializePropertyLinks() {
			return propertyLinks.Count > 0;
		}
		#endregion
		List<ActivityMessage> GetActivityMessages(string parameter) {
			if (graph == null)
				return null;
			return string.IsNullOrEmpty(parameter) ? graph.CalculateActivity() : graph.CalculateActivityChanges(parameter);
		}
		T GetControl<T>(ControlCollection controls, string text) where T : Control {
			if (controls == null)
				return null;
			foreach (PropertyLink link in propertyLinks) {
				if (link.PropertyName == text && link.Editor.GetType().Equals(typeof(T)))
					return link.Editor as T;
			}
			return null;
		}
		protected virtual void UpdateActivity(string parameter) {
			List<ActivityMessage> messages = GetActivityMessages(parameter);
			if (messages == null)
				return;
			foreach (ActivityMessage message in messages) {
				foreach (PropertyLink link in propertyLinks) {
					if (link.JoinedName == message.Name) {
						link.SetEditorActivity(message);
						continue;
					}
				}
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			UpdateColors();
		}
		protected void AddDependence<T>(string mainProperty, string childProperty, Func<T, bool> func) {
			AddDependence<T>(mainProperty, childProperty, EditorActivity.Enable, func);
		}
		protected void AddDependence<T>(string mainProperty, string childProperty, EditorActivity activity, Func<T, bool> func) {
			if(graph == null)
				graph = new ActivityGraph(this);
			graph.AddEdge<T>(mainProperty, childProperty, activity, func);
		}
		internal virtual void UpdateView() {
			foreach (PropertyLink link in propertyLinks) {
				object value = model.GetProperty(link.PropertyFullName);
				link.UpdateEditor(value);
			}
		}
		internal virtual void LoadModel(DesignerChartElementModelBase model) {
			this.model = model;
			propertyLinkDictionary.Clear();
			foreach (PropertyLink link in propertyLinks) {
				object value = model.GetProperty(link.PropertyFullName);
				link.UpdateEditor(value);
				link.SetListener(this);
				IModelBinded bindedEditor = link.Editor as IModelBinded;
				if (bindedEditor != null)
					bindedEditor.SetModel(model);
				propertyLinkDictionary.Add(link.Editor, link);
			}
			UpdateActivity(string.Empty);
		}
		internal void UpdateColors() {
			Color titleColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor(CommonColors.DisabledText);
			Color prefixColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor(CommonColors.HighlightText);
			foreach (TitleElement title in titles) {
				title.PrefixColor = prefixColor;
				title.TitleColor = titleColor;
			}
		}
		internal void LoadEditorsTitle() {
			if (DesignMode)
				return;
			ModelBinding modelAttribute = ReflectionHelper.GetAttribute<ModelBinding>(this.GetType());
			if (modelAttribute == null)
				return;
			ModelOf chartElementAttribute = ReflectionHelper.GetAttribute<ModelOf>(modelAttribute.ModelType);
			if (chartElementAttribute == null)
				return;
			Type chartElementType = chartElementAttribute.ChartElementType;
			foreach (PropertyLink link in propertyLinks) {
				PropertyInfo chartElementProperty = chartElementType.GetProperty(link.PropertyName);
				if (chartElementProperty == null)
					continue;
				DXDisplayNameAttribute nameAttribute = ReflectionHelper.GetAttribute<DXDisplayNameAttribute>(chartElementProperty);
				if (nameAttribute == null)
					continue;
				link.DisplayName = nameAttribute.GetLocalizedDisplayName() + ":";
			}
		}
		public T GetControl<T>(string text) where T : Control {
			return GetControl<T>(this.Controls, text);
		}
	}
	public class PropertyLinksCollection : DXCollection<PropertyLink> {
	}
}
