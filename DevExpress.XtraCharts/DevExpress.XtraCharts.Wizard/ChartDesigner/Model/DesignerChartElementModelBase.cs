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
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using System;
using System.Reflection;
namespace DevExpress.XtraCharts.Designer.Native {
	public enum EditorActivity {
		Enable,
		Visible
	}
	public abstract class DesignerChartElementModelBase : IMessageListener, IChartElementDesignerModel {
		readonly CommandManager commandManager;
		readonly SetDataMemberCommand setDataMemberCommand;
		readonly List<DesignerChartElementModelBase> children;
		readonly Dictionary<string, DesignerChartElementModelBase> childrenDictionary;
		readonly Chart chart;
		ChartCollectionBaseModel parentCollection;
		DesignerChartElementModelBase parent;
		public static bool NeedRecreate(object model, object element) {
			if (model == null)
				return true;
			ChartCollectionBaseModel collectionModel = model as ChartCollectionBaseModel;
			if (collectionModel != null)
				return collectionModel.ChartCollection != element;
			DesignerChartElementModelBase chartElementModel = model as DesignerChartElementModelBase;
			if (chartElementModel != null)
				return chartElementModel.ChartElement != element;
			return true;
		}
		protected internal CommandManager CommandManager { get { return commandManager; } }
		protected internal virtual bool HasOptionsControl { get { return false; } }
		protected internal abstract ChartElement ChartElement { get; }
		protected internal virtual object Element { get { return null; } }
		protected internal List<DesignerChartElementModelBase> Children { get { return children; } }
		protected internal virtual string ChartTreeImageKey { get { return string.Empty; } }
		internal Chart Chart { get { return chart; } }
		internal ChartCollectionBaseModel ParentCollection {
			get { return parentCollection; }
			set { parentCollection = value; }
		}
		internal DesignerChartElementModelBase Parent {
			get { return parent; }
			set { parent = value; }
		}
		internal virtual string ChartTreeText { get { return this.ToString(); } }
		[
		Category("Misc"),
		TypeConverter(typeof(StringConverter))
		]
		public string Tag {
			get { return ChartElement != null ? ChartElement.Tag == null ? "" : ChartElement.Tag.ToString() : null; }
			set {
				if (ChartElement != null)
					SetProperty("Tag", value);
			}
		}
		public DesignerChartElementModelBase(CommandManager commandManager) {
			this.commandManager = commandManager;
			this.setDataMemberCommand = new SetDataMemberCommand(commandManager);
			this.children = new List<DesignerChartElementModelBase>();
			childrenDictionary = new Dictionary<string, DesignerChartElementModelBase>();
		}
		public DesignerChartElementModelBase(CommandManager commandManager, Chart chart)
			: this(commandManager) {
			this.chart = chart;
		}
		#region IMessageListener implementation
		public void AcceptMessage(ViewMessage message) {
			ProcessMessage(message);
		}
		#endregion
		#region IChartElementDesignerModel implementation
		object IChartElementDesignerModel.SourceElement { get { return ChartElement != null ? ChartElement : Element; } }
		#endregion
		protected void SetProperty(string propertyName, object value) {
			SetPropertyCommandParameter parameter = new SetPropertyCommandParameter(this, propertyName, value);
			commandManager.SetPropertyCommand.Execute(parameter);
		}
		protected void BatchSetProperties(List<KeyValuePair<string, object>> propertiesValues) {
			BatchSetObjectPropertyCommand batchSetPropertyCommand = new BatchSetObjectPropertyCommand(CommandManager);
			List<SetPropertyCommandParameter> parameters = new List<SetPropertyCommandParameter>();
			foreach (KeyValuePair<string, object> propertyValue in propertiesValues)
				parameters.Add(new SetPropertyCommandParameter(this, propertyValue.Key, propertyValue.Value));
			batchSetPropertyCommand.Execute(parameters);
		}
		internal T FindParent<T>() where T : DesignerChartElementModelBase {
			DesignerChartElementModelBase currentParent = this.parent;
			while (currentParent != null && !(currentParent is T)) {
				currentParent = currentParent.Parent;
			}
			return (T)currentParent;
		}
		protected internal virtual bool IsSupportsDataControl(bool isDesignTime) {
			return false;
		}
		protected object GetProperty(string propertyName) {
			PropertyInfo property = this.GetType().GetProperty(propertyName);
			if (property == null)
				return null;
			return property.GetValue(this, null);
		}
		protected object GetProperty(string[] path, int index) {
			if (index == path.Length - 1)
				return GetProperty(path[index]);
			else {
				DesignerChartElementModelBase child = GetChild(path[index]);
				if (child != null)
					return child.GetProperty(path, index + 1);
			}
			return null;
		}
		protected virtual void ProcessMessage(ViewMessage message) {
			string name = string.Join(".", message.FullName);
			SetProperty(name, message.Value);
		}
		protected virtual void AddChildren() {
		}
		protected virtual void ClearChildren() {
			Children.Clear();
		}
		protected virtual PropertyInfo GetChildProperty(string name) {
			return this.GetType().GetProperty(name);
		}
		protected virtual bool NeedUpdate() {
			return false;
		}
		public DesignerChartElementModelBase FindElementModel(object chartElement) {
			if (ChartElement == chartElement)
				return this;
			foreach (DesignerChartElementModelBase child in children) {
				DesignerChartElementModelBase childModel = child.FindElementModel(chartElement);
				if (childModel != null)
					return childModel;
			}
			return null;
		}
		public ChartCollectionBaseModel FindCollectionModel(ChartCollectionBase collection) {
			ChartCollectionBaseModel collectionModel = this as ChartCollectionBaseModel;
			if (collectionModel != null && collectionModel.ChartCollection == collection)
				return collectionModel;
			foreach (DesignerChartElementModelBase child in children) {
				ChartCollectionBaseModel childModel = child.FindCollectionModel(collection);
				if (childModel != null)
					return childModel;
			}
			return null;
		}
		public virtual void Update() {
			foreach (DesignerChartElementModelBase child in Children) {
				child.Parent = this;
				child.Update();
			}
		}
		public override string ToString() {
			return ChartElement != null ? ChartElement.ToString() : base.ToString();
		}
		public virtual ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return null;
		}
		public virtual List<DataMemberInfo> GetDataMembersInfo() {
			return null;
		}
		public void SetDataMember(DataMemberInfo dataMemberInfo, string dataMember) {
			setDataMemberCommand.Execute(new SetDataMemberCommandParameter(dataMemberInfo.PropertyName, dataMember, this));
		}
		public object GetProperty(string[] path) {
			return GetProperty(path, 0);
		}
		public DesignerChartElementModelBase GetChild(string name) {
			DesignerChartElementModelBase child = null;
			if (NeedUpdate())
				this.childrenDictionary.Clear();
			if (!childrenDictionary.ContainsKey(name)) {
				PropertyInfo propertyInfo = GetChildProperty(name);
				if (propertyInfo == null)
					return null;
				child = propertyInfo.GetValue(this, null) as DesignerChartElementModelBase;
				if (child != null)
					childrenDictionary.Add(name, child);
			}
			else
				child = childrenDictionary[name];
			return child;
		}
	}
}
