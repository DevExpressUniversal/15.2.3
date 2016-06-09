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
using DevExpress.Office.Services.Implementation;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region RealTimeDataManager
	public class RealTimeDataManager : IDisposable {
		#region Fields
		readonly DocumentModel documentModel;
		readonly Dictionary<string, RealTimeDataApplication> applications;
		Stack<RealTimeTopicList> currentCellInitialTopicListStack;
		Stack<RealTimeTopicList> currentTopicListStack;
		Stack<RealTimeDataCalculationMode> calculationModeStack;
		volatile bool updateRequested;
		Timer timer;
		#endregion
		public RealTimeDataManager(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.applications = new Dictionary<string, RealTimeDataApplication>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			currentCellInitialTopicListStack = new Stack<RealTimeTopicList>();
			currentTopicListStack = new Stack<RealTimeTopicList>();
			calculationModeStack = new Stack<RealTimeDataCalculationMode>();
		}
		void timer_Tick(object sender, EventArgs e) {
			if (updateRequested) {
				updateRequested = false;
				ProcessDataChanges();
			}
		}
		internal Dictionary<string, RealTimeDataApplication> Applications { get { return applications; } }
		public VariantValue GetValue(string applicationId, string serverName, string[] parameters) {
			RealTimeDataApplication application = PrepareApplication(applicationId, serverName, true);
			RealTimeTopic topic = application.RegisterTopic(parameters);
			if (topic != null) {
				application.RefreshDataIfExists();
				InitTimer();
				currentTopicListStack.Peek().Add(topic);
				return topic.CachedValue;
			}
			return VariantValue.ErrorValueNotAvailable;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
		void InitTimer() {
			if (timer != null || !documentModel.VisualControlAssigned)
				return;
			timer = new Timer();
#if !SL
			timer.Interval = 500;
#else
			this.timer.Interval = TimeSpan.FromMilliseconds(500);
#endif
			timer.Tick += timer_Tick;
			timer.Start();
		}
		public RealTimeDataApplication PrepareApplication(string applicationId, string serverName, bool autoStart) {
			string key = CreateApplicationKey(applicationId, serverName);
			RealTimeDataApplication application = GetApplication(key);
			if (application == null)
				application = CreateApplication(key, applicationId, serverName, autoStart);
			return application;
		}
		RealTimeDataApplication CreateApplication(string key, string applicationId, string serverName, bool autoStart) {
			RealTimeDataApplication result = new RealTimeDataApplication(documentModel, applicationId, serverName);
			if (autoStart && result.Start())
				result.DataChanged += OnDataChanged;
			applications[key] = result;
			return result;
		}
		public void RegisterApplication(RealTimeDataApplication application) {
			string key = CreateApplicationKey(application.ApplicationId, application.ServerName);
			this.applications.Add(key, application);
		}
		public void UnregisterTopic(RealTimeTopic topic) {
			RealTimeDataApplication application = topic.Application;
			if (application == null)
				return;
			application.UnregisterTopic(topic);
			if (!application.HasTopics)
				UnregisterApplication(application);
		}
		internal RealTimeDataApplication GetApplication(string applicationId, string serverName) {
			return GetApplication(CreateApplicationKey(applicationId, serverName));
		}
		RealTimeDataApplication GetApplication(string key) {
			RealTimeDataApplication result;
			if (!applications.TryGetValue(key, out result))
				return null;
			else
				return result;
		}
		public static string CreateApplicationKey(string applicationId, string serverName) {
			if (String.IsNullOrEmpty(serverName))
				return applicationId;
			else
				return "//" + serverName + "/" + applicationId;
		}
		#region UnregisterApplication
		void UnregisterApplication(RealTimeDataApplication application) {
			applications.Remove(CreateApplicationKey(application.ApplicationId, application.ServerName));
			UnregisterApplicationCore(application);
		}
		void UnregisterApplicationCore(RealTimeDataApplication application) {
			application.DataChanged -= OnDataChanged;
			application.Dispose();
		}
		#endregion
		#region IDisposable implementation
		~RealTimeDataManager() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (timer != null) {
					timer.Tick -= timer_Tick;
					timer.Stop();
					timer.Dispose();
					timer = null;
				}
				foreach (string key in applications.Keys)
					UnregisterApplicationCore(applications[key]);
				applications.Clear();
			}
		}
		#endregion
		void OnDataChanged(object sender, EventArgs e) {
			updateRequested = true;
			if (!documentModel.VisualControlAssigned) {
				RealTimeDataApplication application = sender as RealTimeDataApplication;
				if (application != null)
					ProcessDataChanges(application);
				else
					ProcessDataChanges();
			}
		}
		#region ProcessDataChanges
		void ProcessDataChanges() {
			documentModel.BeginUpdate();
			try {
				foreach (RealTimeDataApplication application in Applications.Values) {
					ProcessDataChangesCore(application);
				}
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		void ProcessDataChanges(RealTimeDataApplication application) {
			documentModel.BeginUpdate();
			try {
				ProcessDataChangesCore(application);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		void ProcessDataChangesCore(RealTimeDataApplication application) {
			foreach (ICell cell in application.GetReferencedCells()) {
				documentModel.CalculationChain.MarkupDependentsForRecalculation(cell);
			}
		}
		#endregion
		#region Notifications
		public void OnStartExpressionEvaluation() {
			calculationModeStack.Push(RealTimeDataCalculationMode.Expression);
			currentTopicListStack.Push(new RealTimeTopicList());
		}
		public void OnEndExpressionEvaluation() {
			calculationModeStack.Pop();
			if (calculationModeStack.Count <= 0 || calculationModeStack.Peek() == RealTimeDataCalculationMode.Expression) {
				RealTimeTopicList calledTopics = currentTopicListStack.Pop();
				ClearNonReferencedTopics(calledTopics);
			}
		}
		public void OnStartDefinedNameEvaluation() {
			calculationModeStack.Push(RealTimeDataCalculationMode.DefinedName);
		}
		public void OnEndDefinedNameEvaluation() {
			calculationModeStack.Pop();
			RealTimeTopicList calledTopics = currentTopicListStack.Pop();
			if (currentTopicListStack.Count > 0)
				currentTopicListStack.Peek().AddRange(calledTopics);
		}
		public void OnStartCellCalculation(ICell cell) {
			RealTimeTopicList cellTopicList = GetTopicsByCell(cell);
			currentCellInitialTopicListStack.Push(cellTopicList);
			calculationModeStack.Push(RealTimeDataCalculationMode.CellValue);
		}
		public void OnEndCellCalculation(ICell cell) {
			if (currentTopicListStack.Count <= 0)
				return;
			RealTimeTopicList newTopicsList = currentTopicListStack.Pop();
			RealTimeTopicList previousTopicsList = currentCellInitialTopicListStack.Pop();
			foreach (RealTimeTopic previousTopic in previousTopicsList)
				previousTopic.UnregisterCell(cell);
			foreach (RealTimeTopic newTopic in newTopicsList)
				newTopic.RegisterCell(cell);
			ClearNonReferencedTopics(previousTopicsList);
			calculationModeStack.Pop();
		}
		#endregion
		void ClearNonReferencedTopics(RealTimeTopicList list) {
			foreach (RealTimeTopic topic in list)
				if (topic.ReferenceCount <= 0)
					UnregisterTopic(topic);
		}
		RealTimeTopicList GetTopicsByCell(ICell cell) {
			RealTimeTopicList result = new RealTimeTopicList();
			foreach (RealTimeDataApplication application in applications.Values) {
				application.LookupTopicsByCell(cell, result);
			}
			return result;
		}
		public void UnregisterCell(ICell cell) {
			RealTimeTopicList topicList = GetTopicsByCell(cell);
			foreach (RealTimeTopic topic in topicList) {
				UnregisterCellFromTopic(cell, topic);
			}
		}
		void UnregisterCellFromTopic(ICell cell, RealTimeTopic topic) {
			topic.UnregisterCell(cell);
			if (topic.ReferenceCount <= 0)
				UnregisterTopic(topic);
		}
		public void OnEndSetContent() {
			bool applicationStarted = false;
			foreach (RealTimeDataApplication application in applications.Values) {
				if (application.Start()) {
					application.DataChanged += OnDataChanged;
					application.ConnectTopicsToService();
					applicationStarted = true;
				}
			}
			if (applicationStarted) {
				InitTimer();
				ProcessDataChanges();
			}
		}
		internal void OnEndPaint() {
			if (timer != null) {
				timer.Stop();
				timer.Start();
			}
		}
	}
	#endregion
	#region RealTimeDataApplication
	public class RealTimeDataApplication : IDisposable {
		#region Fields
		readonly DocumentModel documentModel;
		string applicationId;
		readonly Dictionary<string, RealTimeTopic> topics;
		string serverName;
		IRTDService service;
		int topicId;
		#endregion
		public RealTimeDataApplication(DocumentModel documentModel, string applicationId, string serverName)
			: this(documentModel) {
			this.applicationId = applicationId;
			this.serverName = serverName;
			this.topics = new Dictionary<string, RealTimeTopic>();
		}
		public RealTimeDataApplication(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public bool HasTopics { get { return topics.Count > 0; } }
		internal Dictionary<string, RealTimeTopic> Topics { get { return topics; } }
		public string ApplicationId { get { return applicationId; } set { applicationId = value; } }
		public string ServerName { get { return serverName; } set { serverName = value; } }
		#region Events
		#region DataChanged
		EventHandler onDataChanged;
		volatile bool refreshRequired;
		public event EventHandler DataChanged { add { onDataChanged += value; } remove { onDataChanged -= value; } }
		protected virtual void RaiseDataChanged() {
			if (onDataChanged != null)
				onDataChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		~RealTimeDataApplication() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				Stop();
				DisposeService();
			}
		}
		public IEnumerable<ICell> GetReferencedCells() {
			foreach (RealTimeTopic topic in topics.Values) {
				foreach (ICell cell in topic.ReferencedCells) {
					yield return cell;
				}
			}
		}
		public void Stop() {
			if (service != null) {
				service.DataChanged -= OnDataChanged;
				foreach (string key in topics.Keys)
					DisconnectFromService(topics[key].Id);
				service.Stop();
				topics.Clear();
				topicId = 0;
			}
		}
		void DisposeService() {
			if (service != null) {
				IDisposable disposable = service as IDisposable;
				if (disposable != null)
					disposable.Dispose();
				service = null;
			}
		}
		#endregion
		public bool Start() {
			IRTDServiceFactory factory = documentModel.GetService<IRTDServiceFactory>();
			if (factory == null)
				return false;
			this.service = factory.CreateRTDService();
			bool connectionSuccessfull = service.Connect(applicationId, serverName);
			if (!connectionSuccessfull)
				DisposeService();
			else
				service.DataChanged += OnDataChanged;
			return connectionSuccessfull;
		}
		public RealTimeTopic RegisterTopic(string[] parameters) {
			string key = CalculateTopicKey(parameters);
			RealTimeTopic topic;
			if (!topics.TryGetValue(key, out topic)) {
				topic = new RealTimeTopic(this, topicId, parameters);
				TryConnectTopicToService(topic);
				topicId++;
				topics[key] = topic;
				RefreshData();
			}
			return topic;
		}
		#region UnregisterTopic
		public void UnregisterTopic(RealTimeTopic topic) {
			string key = CalculateTopicKey(topic.Parameters);
			UnregisterTopicCore(key, topic);
		}
		public void UnregisterTopic(string[] parameters) {
			string key = CalculateTopicKey(parameters);
			RealTimeTopic topic;
			if (!topics.TryGetValue(key, out topic))
				return;
			UnregisterTopicCore(key, topic);
		}
		protected internal void UnregisterTopicCore(string key, RealTimeTopic topic) {
			if (topic.ReferenceCount <= 0) {
				DisconnectFromService(topic.Id);
				topics.Remove(key);
			}
		}
		#endregion
		internal void ConnectTopicsToService() {
			if (service == null)
				return;
			foreach (RealTimeTopic topic in topics.Values)
				TryConnectTopicToService(topic);
			RefreshData();
		}
		bool TryConnectTopicToService(RealTimeTopic topic) {
			if (service == null)
				return false;
			object[] parameters = new object[topic.Parameters.Length];
			topic.Parameters.CopyTo(parameters, 0);
			if (!service.ConnectData(topic.Id, parameters))
				return false;
			return true;
		}
		void DisconnectFromService(int topicId) {
			if (service == null)
				return;
			service.DisconnectData(topicId);
		}
		public void RefreshDataIfExists() {
			if (refreshRequired)
				RefreshData();
		}
		void RefreshData() {
			refreshRequired = false;
			if (service == null)
				return;
			Dictionary<int, VariantValue> values = service.GetData();
			foreach (string key in topics.Keys) {
				RealTimeTopic topic = topics[key];
				VariantValue value;
				if (values.TryGetValue(topic.Id, out value))
					topic.CachedValue = value;
			}
		}
		void OnDataChanged(object sender, EventArgs e) {
			refreshRequired = true;
			RaiseDataChanged();
		}
		string CalculateTopicKey(string[] parameters) {
			StringBuilder result = new StringBuilder();
			int count = parameters.Length;
			for (int i = 0; i < count; i++) {
				result.Append("//");
				result.Append(parameters[i]);
			}
			return result.ToString();
		}
		internal RealTimeTopic GetTopic(string[] parameters) {
			string key = CalculateTopicKey(parameters);
			RealTimeTopic topic = null;
			topics.TryGetValue(key, out topic);
			return topic;
		}
		public void LookupTopicsByCell(ICell cell, RealTimeTopicList where) {
			foreach (RealTimeTopic topic in topics.Values) {
				if (topic.ReferencedCells.Contains(cell))
					where.Add(topic);
			}
		}
		public RealTimeTopic AddTopic(string[] parameters) {
			string key = CalculateTopicKey(parameters);
			RealTimeTopic topic;
			if (!topics.TryGetValue(key, out topic)) {
				topic = new RealTimeTopic(this, topicId, parameters);
				topicId++;
				topics[key] = topic;
			}
			return topic;
		}
	}
	#endregion
	#region RealTimeTopic
	public class RealTimeTopic {
		readonly RealTimeDataApplication application;
		readonly int id;
		readonly string[] parameters;
		readonly List<ICell> referencedCells;
		VariantValue cachedValue = VariantValue.ErrorValueNotAvailable;
		public RealTimeTopic(RealTimeDataApplication application, int id, string[] parameters) {
			this.id = id;
			this.parameters = parameters;
			this.referencedCells = new List<ICell>();
			this.application = application;
		}
		public int Id { get { return id; } }
		public string[] Parameters { get { return parameters; } }
		public int ReferenceCount { get { return referencedCells.Count; } }
		public List<ICell> ReferencedCells { get { return referencedCells; } }
		public VariantValue CachedValue { get { return cachedValue; } set { cachedValue = value; } }
		public RealTimeDataApplication Application { get { return application; } }
		public void UnregisterCell(ICell cell) {
			for (int i = this.ReferencedCells.Count - 1; i >= 0; i--) {
				if (object.ReferenceEquals(this.ReferencedCells[i], cell))
					this.ReferencedCells.RemoveAt(i);
			}
		}
		public void RegisterCell(ICell cell) {
			this.ReferencedCells.Add(cell);
		}
	}
	#endregion
	#region RealTimeTopicList
	public class RealTimeTopicList : List<RealTimeTopic> {
	}
	#endregion
	#region RealTimeDataCalculationMode
	public enum RealTimeDataCalculationMode {
		CellValue,
		DefinedName,
		Expression,
	}
	#endregion
	#region VolatileDependecyType
	public enum VolatileDependecyType {
		OLAPFormulas,
		RealTimeData,
	}
	#endregion
	#region VolatileDependecyValueType
	public enum VolatileDependecyValueType {
		Boolean,
		Error,
		RealNumber,
		String
	}
	#endregion
}
