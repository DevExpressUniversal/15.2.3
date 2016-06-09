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
using System.Xml;
using DevExpress.XtraEditors.FeatureBrowser;
namespace DevExpress.XtraEditors.Design.TasksSolution {
	public class TaskNameItem {
		TaskItem item;
		string name;
		public TaskNameItem(TaskItem item, string name) {
			this.item = item;
			this.name = name;
		}
		public TaskItem Item { get { return item; } }
		public string Name { get { return name; } }
		public override string ToString() { return Name; } 
	}
	public class TaskNameItemCollection : CollectionBase, IComparer {
		public TaskNameItemCollection() { }
		public TaskNameItem this[int index] { get { return InnerList[index] as TaskNameItem; } }
		public void AddTaskItem(TaskItem item) {
			AddItem(item, item.Name);
			if(item.Names != null) {
				for(int i = 0; i < item.Names.Length; i ++)
					AddItem(item, item.Names[i]);
			}
		}
		public void Sort() {
			InnerList.Sort(this);
		}
		protected void AddItem(TaskItem item, string name) {
			if(item == null || name == null || name == string.Empty) return;
			InnerList.Add(new TaskNameItem(item, name));
		}
		int IComparer.Compare(object obj1, object obj2) {
			TaskNameItem item1 = obj1 as TaskNameItem;
			TaskNameItem item2 = obj2 as TaskNameItem;
			return string.Compare(item1.Name, item2.Name, true);
		}
	}
	public class TaskStepBase : CollectionBase {
		string id;
		string[] steps;
		string description;
		string name;
		public TaskStepBase(string id, string[] steps) {
			this.steps = steps;
			this.id = id;
		}
		public string Id { get { return id; } }
		public StepItem this[int index] { get { return InnerList[index] as StepItem; } }
		public string Description { get { return description; } set { description = value; } }
		public string Name { get { return name; } set { name = value; } }
		protected string[] StepsId { get { return steps; } }
		protected internal void ResolveSteps(Hashtable hash) {
			for(int i = 0; i < StepsId.Length; i ++) {
				StepItem item = hash[StepsId[i]] as StepItem;
				if(item != null)
					AddItem(item);
			}
		}
		protected void AddItem(StepItem item) {
			InnerList.Add(item);
		}
	}
	public class TaskItem : TaskStepBase {
		string[] names;
		string eventId;
		TaskEvent taskEvent;
		public TaskItem(string id, string[] steps) : base(id, steps) {
			this.eventId = string.Empty;
			this.taskEvent = null;
		}
		public string[] Names { get { return names; } set { names = value; } }
		public TaskEvent Event { get { return taskEvent; } }
		protected internal string EventId { get { return eventId; } set { eventId = value; } }
		protected internal void ResolveTaskEvent(Hashtable hash) {
			if(eventId != string.Empty)
				this.taskEvent = (TaskEvent)hash[eventId];
		}
	}
	public class StepItem : TaskStepBase {
		string startCondition;
		string readyCondition;
		string[] properties;
		string[] expandedPropertiesOnStart;
		string sourceProperty;
		string frame;
		public StepItem(string id, string[] steps) : base(id, steps) {
			this.properties = new string[] {};
			this.expandedPropertiesOnStart = new string[] {};
			this.sourceProperty = string.Empty;
			this.frame = string.Empty;
		}
		public string[] Properties { get { return properties; } set { properties = value; } }
		public string[] ExpandedPropertiesOnStart { get { return expandedPropertiesOnStart; } set { expandedPropertiesOnStart = value; } }
		public string Frame { get { return frame; } set { frame = value; } }
		public string SourceProperty { get { return sourceProperty; } set { sourceProperty = value; } }
		public string StartCondition { get { return startCondition; } set { startCondition = value; } }
		public string ReadyCondition { get { return readyCondition; } set { readyCondition = value; } }
	}
	public abstract class TaskItemsBase : CollectionBase {
		public TaskItemsBase() {}
		public TaskStepBase Add(string id, string[] steps) {
			TaskStepBase item = CreateItem(id, steps);
			InnerList.Add(item);
			return item;
		}
		protected abstract TaskStepBase CreateItem(string id, string[] steps);
		protected internal TaskStepBase GetItem(int index) { return InnerList[index] as TaskStepBase; }
	}
	public class TaskItems : TaskItemsBase {
		public TaskItems()	{ }
		public TaskItem this[int index] { get { return InnerList[index] as TaskItem; } }
		public TaskNameItemCollection CreateTaskNameItemCollection() {
			TaskNameItemCollection collection = new TaskNameItemCollection();
			for(int i = 0; i < Count; i ++)
				collection.AddTaskItem(this[i]);
			collection.Sort();
			return collection;
		}
		protected override TaskStepBase CreateItem(string id, string[] steps) {
			return new TaskItem(id, steps);
		}
	}
	public class StepItems : TaskItemsBase {
		public StepItems()	{}
		public StepItem this[int index] { get { return InnerList[index] as StepItem; } }
		protected override TaskStepBase CreateItem(string id, string[] steps) {
			return new StepItem(id, steps);
		}
	}
	public class TaskEventLanguages {
		Hashtable languages;
		public TaskEventLanguages() {
			this.languages = new Hashtable();
		}
		public string this[string name] {
			get { return languages[name] != null ? (string)languages[name] : string.Empty; }
			set { languages[name] = value; }
		}
	}
	public class TaskEvent : CollectionBase {
		string id;
		string name;
		string eventName;
		string description;
		string[] events;
		TaskEventLanguages languages;
		public TaskEvent(string id, string eventName, string[] events) {
			this.id = id;
			this.eventName = eventName;
			this.description = string.Empty;
			this.languages = new TaskEventLanguages();
			this.events = events;
		}
		public TaskEvent this[int index] { get { return InnerList[index] as TaskEvent; } }
		public void Add(TaskEvent ev) {
			InnerList.Add(ev);
		}
		public string Id { get { return id; } set { id = value; } }
		public string EventName { get { return eventName; } set { eventName = value; } }
		public string Name { get { return name; } set { name = value; } }
		public string Description { get { return description; } set { description = value; } }
		public TaskEventLanguages Languages { get { return languages; } }
		protected internal void ResolveEvents(Hashtable hash) {
			for(int i = 0; i < events.Length; i ++) {
				TaskEvent item = hash[events[i]] as TaskEvent;
				if(item != null)
					Add(item);
			}
		}
	}
	public abstract class TaskItemsCreatorBase : XmlFeaturesReaderBase {
		TaskItemsBase items;
		public TaskItemsCreatorBase(Type sourceType) : base(sourceType) {
			this.items = CreateCollection();
		}
		protected TaskItemsBase Items { get { return items; } }
		protected override void AddXmlNodeCore(XmlNode node) {
			AddItem(node);
		}
		protected abstract TaskItemsBase CreateCollection();
		protected internal abstract Hashtable CreateStepHashtable();
		protected virtual TaskStepBase AddItem(XmlNode node) {
			string id = GetNodeAttributeValue(node, "ID");
			string[] steps = GetSteps(node);
			TaskStepBase item = Items.Add(id, steps);
			item.Description = GetNodeDescription(node);;
			return item;
		}
		protected internal virtual void ResolveSteps() {
			Hashtable hashtable = CreateStepHashtable();
			for(int i = 0; i < Items.Count; i ++) {
				Items.GetItem(i).ResolveSteps(hashtable);
			}
		}
		protected string[] GetSteps(XmlNode node) {
			return GetArrayValues(node, "StepItems", "Id");
		}
	}
	public class TaskItemsCreator : TaskItemsCreatorBase {
		StepItemsCreator stepItemsCreator;
		EventItemsCreator eventItemsCreator;
		public TaskItemsCreator(Type sourceType) : base(sourceType)	{
			stepItemsCreator = new StepItemsCreator(sourceType);
			eventItemsCreator = new EventItemsCreator(sourceType);
		}
		public TaskItems LoadFromXmls(string[] xmlFiles) {
			LoadFromXmlFiles(xmlFiles);
			return Items as TaskItems;
		}
		protected override void LoadFromXmlFiles(string[] xmlFiles) {
			base.LoadFromXmlFiles(xmlFiles);
			ResolveSteps();
			ResolveEvents();
		}
		protected  internal override void LoadFromXmlDocument(XmlDocument doc) {
			base.LoadFromXmlDocument(doc);
			stepItemsCreator.LoadFromXmlDocument(doc);
			eventItemsCreator.LoadFromXmlDocument(doc);
		}
		protected internal override void ResolveSteps() {
			base.ResolveSteps();
			stepItemsCreator.ResolveSteps();
		}
		void ResolveEvents() {
			for(int i = 0; i < Items.Count; i ++)
				(Items as TaskItems)[i].ResolveTaskEvent(eventItemsCreator.Events);
			eventItemsCreator.ResolveEvents();
		}
		protected override string ItemTagName { get { return "Task";} }
		protected override TaskItemsBase CreateCollection() { return new TaskItems(); } 
		protected override TaskStepBase AddItem(XmlNode node) {
			TaskItem item = base.AddItem(node) as TaskItem;
			string name = GetNodeAttributeValue(node, "Name");
			item.Names = GetTaskNames(node, name);
			item.EventId = GetNodeAttributeValue(node, "EventId");
			return item;
		}
		protected internal override Hashtable CreateStepHashtable() {
			return stepItemsCreator.CreateStepHashtable();
		}
		string[] GetTaskNames(XmlNode node, string name) {
			ArrayList list = new ArrayList();
			list.Add(name);
			GetArrayValues(node, "Names", "Name", list);
			return (string[])list.ToArray(typeof(string));
		}
	}
	public class StepItemsCreator : TaskItemsCreatorBase {
		public StepItemsCreator(Type sourceType) : base(sourceType)	{}
		protected override string ItemTagName { get { return "Step";} }
		protected override TaskItemsBase CreateCollection() { return new StepItems(); } 
		protected internal override Hashtable CreateStepHashtable() {
			Hashtable table = new Hashtable();
			for(int i = 0; i < Items.Count; i ++) {
				table[Items.GetItem(i).Id] = Items.GetItem(i);
			}
			return table;
		}
		protected override TaskStepBase AddItem(XmlNode node) {
			StepItem item = base.AddItem(node) as StepItem;
			item.Name = GetNodeAttributeValue(node, "Name");
			item.SourceProperty = GetNodeAttributeValue(node, "SourceProperty");
			item.Properties = GetArrayValues(node, "Properties", "Name");
			item.ExpandedPropertiesOnStart = GetArrayValues(node, "ExpandedPropertiesOnStart", "Name");
			item.StartCondition = GetNodeAttributeValue(node, "StartCondition");
			item.ReadyCondition = GetNodeAttributeValue(node, "Ready");
			item.Frame = GetNodeAttributeValue(node, "Frame");
			return item;
		}
	}
	public class EventItemsCreator : XmlFeaturesReaderBase {
		Hashtable taskEvents;
		public EventItemsCreator(Type sourceType) : base(sourceType) {
			this.taskEvents = new Hashtable();
		}
		protected override string ItemTagName { get { return "Event"; } }
		public Hashtable Events { get { return taskEvents; } }
		public void ResolveEvents() {
			foreach(object obj in Events.Values) {
				(obj as TaskEvent).ResolveEvents(Events);
			}
		}
		protected override void AddXmlNodeCore(XmlNode node) {
			string id = GetNodeAttributeValue(node, "ID");
			string eventName = GetNodeAttributeValue(node, "EventName");
			string[] events = GetArrayValues(node, "EventItems", "Id");;
			TaskEvent item = new TaskEvent(id, eventName, events);
			this.taskEvents[item.Id] = item;
			item.Description = GetNodeDescription(node);
			item.Name = GetNodeAttributeValue(node, "Name");
			LoadLanguages(item, node);
		}
		void LoadLanguages(TaskEvent item, XmlNode node) {
			XmlNode lNode = this.FindChildNode(node, "Languages");
			if(lNode == null) return;
			for(int i = 0; i < lNode.ChildNodes.Count; i ++) {
				AddLanguage(item, lNode.ChildNodes[i]);
			}
		}
		void AddLanguage(TaskEvent item, XmlNode node) {
			string code = GetNodeInnerText(node);
			string language = GetNodeAttributeValue(node, "Name");
			if(language != string.Empty)
				item.Languages[language] = code;
		}
	}
}
