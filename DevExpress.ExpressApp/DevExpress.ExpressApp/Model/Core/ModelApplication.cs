#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model.Core {
	public class ModelMultipleMasterStoreItem {
		ModelNode master;
		bool isMasterCalculated;
		public ModelMultipleMasterStoreItem() {}
		public ModelNode Master {
			get { return master; }
			set {
				master = value;
				IsMasterCalculated = value != null;
			}
		}
		public bool IsMasterCalculated {
			get { return isMasterCalculated; }
			set { isMasterCalculated = value; }
		}
	}
	public abstract class ModelMultipleMasterStore {
		static ModelMultipleMasterStore instance;
		public static ModelMultipleMasterStore Instance {
			get { return instance; }
			set { instance = value; }
		}
		public ModelMultipleMasterStore() {
		}
		public ModelMultipleMasterStoreItem GetMasterItem(ModelNode node) {
			if(node == null) return null;
			Dictionary<ModelNode, ModelMultipleMasterStoreItem> store = MasterStore;
			ModelMultipleMasterStoreItem item;
			store.TryGetValue(node, out item);
			if(item == null) {
				item = new ModelMultipleMasterStoreItem();
				store[node] = item;
			}
			return item;
		}
		protected abstract Dictionary<ModelNode, ModelMultipleMasterStoreItem> MasterStore { get; }
	}
	public class ModelApplicationBase : ModelNode, IModelSources, IModelApplicationServices, ICloneable {
		const string ErrorAspectNameIsNotSupported = "Aspect name '{0}' is not supported.";
		public class UnusableModelApplication : ModelApplicationBase {
			public UnusableModelApplication(ModelNodeInfo nodeInfo) : base(nodeInfo, "Application") { }
		}
		private bool hasMultipleMasters;
		private int currentAspectIndex;
		private List<string> aspects;
		private bool isLoading;
		private ICurrentAspectProvider currentAspectProvider;
		private readonly Dictionary<string, int> aspectsRelations;
		private int[] aspectIndexRelations;
		private ModelApplicationBase unusableModel;
		private int creatingMasterNodeCounter;
		private int masterCounter;
		private Dictionary<string, Version> schemaModuleInfos;
		private Int32 version;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelApplicationBase(ModelNodeInfo nodeInfo, string nodeId)
			: base(nodeInfo, "Application") {
			aspectsRelations = new Dictionary<string, int>();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int LayersCount { get { return Layers.Count; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void MergeWith(ModelApplicationBase layer) {
			MergeAspects(layer);
			base.MergeWith(layer);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasMultipleMasters {
			get { return hasMultipleMasters; }
			set { hasMultipleMasters = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLoading { get { return isLoading; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ModelApplicationBase LastLayer {
			get {
				return (ModelApplicationBase)base.LastLayer;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Int32 Version {
			get { return version; }
			set { version = value; }
		}
		protected override void SetMaster(ModelNode value) {
			base.SetMaster(value);
			this.masterCounter += value != null ? 1 : -1;
		}
		protected internal void StartLoading() { this.isLoading = true; }
		protected internal void StopLoading() { this.isLoading = false; }
		protected int CurrentAspectIndex { get { return currentAspectIndex; } }
#if DEBUG
		public
#else  
		protected internal 
#endif
 List<string> Aspects {
			get {
				if(aspects == null) {
					aspects = new List<string>();
				}
				return aspects;
			}
		}
		protected override bool GetIsSlave() {
			return masterCounter > 0;
		}
		public IEnumerable<string> GetAspectNames() {
			return Aspects;
		}
		protected override int GetRootCurrentAspectIndex() {
			ModelApplicationBase master = GetMasterApplication();
			return master != null ? master.CurrentAspectIndex : CurrentAspectIndex;
		}
		public string CurrentAspect {
			get {
				lock(TypesInfo.lockObject) {
					ModelApplicationBase master = GetMasterApplication();
					if(master != null) return master.CurrentAspect;
					return CurrentAspectIndex > 0 ? Aspects[CurrentAspectIndex - 1] : string.Empty;
				}
			}
		}
		public void SetCurrentAspect(string value) {
			lock (TypesInfo.lockObject) {
				SetCurrentAspect(value, false);
			}
		}
		private void SetCurrentAspect(string value, bool forceChanged) {
			if(CurrentAspect == value && !forceChanged) return;
			ModelApplicationBase target = GetMasterApplicationOrThis();
			target.SetCurrentAspectRecurcive(value);
		}
		public ICurrentAspectProvider CurrentAspectProvider {
			get { return GetMasterApplicationOrThis().currentAspectProvider; }
			set { GetMasterApplicationOrThis().SetCurrentAspectProvider(value); }
		}
		void SetCurrentAspectProvider(ICurrentAspectProvider value) {   
			if(currentAspectProvider != value) {
				if(currentAspectProvider != null) {
					currentAspectProvider.CurrentAspectChanged -= new EventHandler(currentAspectProvider_CurrentAspectChanged);
				}
				if(value != null) {
					value.CurrentAspectChanged += new EventHandler(currentAspectProvider_CurrentAspectChanged);
				}
			}
			currentAspectProvider = value;
			if(currentAspectProvider != null && CurrentAspect != currentAspectProvider.CurrentAspect) {
				SetCurrentAspectRecurcive(currentAspectProvider.CurrentAspect);
			}
		}
		void SetCurrentAspectRecurcive(string aspect) {
			lock (TypesInfo.lockObject) {
				currentAspectIndex = GetAspectIndex(aspect);
				if(Layers != null) {
					foreach(ModelApplicationBase layer in Layers) {
						layer.SetCurrentAspectRecurcive(aspect);
					}
				}
			}
		}
		void currentAspectProvider_CurrentAspectChanged(object sender, EventArgs e) {
			SetCurrentAspectRecurcive(currentAspectProvider.CurrentAspect);
		}
		bool IsValidAspectName(string aspectName) {
			try {
				new System.Globalization.CultureInfo(aspectName, false);
				return true;
			}
			catch(SystemException) {
				return false;
			}
		}
		public int GetAspectIndex(string aspect) {
			if(aspect == CaptionHelper.DefaultLanguage || string.IsNullOrEmpty(aspect)) {
				return 0;
			}
			int index = Aspects.IndexOf(aspect);
			if(index >= 0) {
				return index + 1;
			}
			else {
				return GetFirstAvailableParentIndex(aspect, Aspects);
			}
		}
		private int GetFirstAvailableParentIndex(string aspect, List<string> availableAspects) {
			int result = 0;
			if(aspectsRelations.TryGetValue(aspect, out result)) {
				return result;
			}
			string currentAspect = aspect;
			while(true) {
				currentAspect = CaptionHelper.GetParentAspect(currentAspect);
				int index = availableAspects.IndexOf(currentAspect);
				if(index >= 0) {
					aspectsRelations[aspect] = index + 1;
					return index + 1;
				}
				if(currentAspect == CaptionHelper.DefaultLanguage || string.IsNullOrEmpty(currentAspect)) {
					aspectsRelations[aspect] = 0;
					return 0;
				}
			}
		}
		public void AddAspect(string aspect) {
			Guard.ArgumentNotNull(aspect, "aspect");
			if(!IsValidAspectName(aspect)) {	
				throw new ArgumentException(string.Format(ErrorAspectNameIsNotSupported, aspect), "aspect");
			}
			ModelApplicationBase masterApplication = GetMasterApplication();
			if(masterApplication != null) {
				masterApplication.AddAspectCore(aspect);
			}
			AddAspectCore(aspect);
		}
		protected void AddAspectCore(string aspect) {
			if(!string.IsNullOrEmpty(aspect) && !Aspects.Contains(aspect)) {
				aspects.Add(aspect);
				aspectIndexRelations = null;
				aspectsRelations.Clear();
			}
			if(IsMaster) {
				foreach(ModelApplicationBase layer in Layers) {
					layer.AddAspectCore(aspect);
				}
			}
		}
		public int AspectCount { get { return Aspects.Count + 1; } }
		public string GetAspect(int aspectIndex) {
			if(aspectIndex > 0) {
				if(aspectIndex >= AspectCount)
					throw new ArgumentOutOfRangeException("aspectIndex", string.Format("There are {0} aspects in the model.", AspectCount));
				return Aspects[aspectIndex - 1];
			}
			return string.Empty;
		}
		public void RemoveAspect(string aspect) {
			if(GetMasterApplication() != null) {
				GetMasterApplication().RemoveAspectCore(aspect);
			}
			else {
				RemoveAspectCore(aspect);
			}
			if(IsMaster) {
				foreach(ModelApplicationBase layer in Layers) {
					layer.RemoveAspectCore(aspect);
				}
			}
		}
		protected void RemoveAspectCore(string aspect) {
			if(Aspects.Contains(aspect)) aspects.Remove(aspect);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelApplicationBase Clone() {
			ModelApplicationBase clone = CreateModelApplication();
			AssignTo(clone);
			return clone;
		}
		protected override ModelApplicationBase GetOrCreateUnusableModel() {
			if(unusableModel == null) {
				unusableModel = new UnusableModelApplication(NodeInfo);
				unusableModel.MergeAspectToLayer(this);
			}
			return unusableModel;
		}
		internal ModelApplicationBase UnusableModel { get { return unusableModel; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelApplicationBase CalculateUnusableModel() {
			if(IsMaster) {
				return LastLayer.CalculateUnusableModel();
			}
			else {
				if(!IsSeparate && !IsInFirstLayer) {
					UpdateUnusableNodes();
				}
				return UnusableModel;
			}
		}
		internal override void InsertLayerAtInternal(ModelNode layer, int insertIndex) {
			MergeAspects(layer);
			base.InsertLayerAtInternal(layer, insertIndex);
		}
		protected void AssignTo(ModelApplicationBase app) {
			base.AssignTo(app);
			if(app != null) {
				app.Aspects.AddRange(Aspects);
				app.CurrentAspectProvider = CurrentAspectProvider;
				app.SetCurrentAspect(CurrentAspect);
				app.version = version;
			}
		}
		protected ModelApplicationBase GetMasterApplication() {
			ModelNode masterNode = Master;
			while(masterNode != null) {
				ModelNode node = masterNode.Master;
				if(node == null) break;
				masterNode = node;
			}
			return masterNode as ModelApplicationBase;
		}
		protected ModelApplicationBase GetMasterApplicationOrThis() {
			ModelApplicationBase result = GetMasterApplication();
			return result != null ? result : this;
		}
		protected ModelApplicationBase CreateModelApplication() {
			return CreatorInstance.CreateModelApplication();
		}
		protected virtual void MergeAspects(ModelNode newLayer) {
			ModelApplicationBase newModel = newLayer as ModelApplicationBase;
			MergeAspectToLayer(newModel);
			if(IsMaster) {
				if(newModel.AspectCount > 1) {
					foreach(ModelNode layer in Layers) {
						ModelApplicationBase model = layer as ModelApplicationBase;
						model.MergeAspectToLayer(newLayer);
					}
				}
				if(AspectCount > 1) {
					int[] indices = new int[newModel.AspectCount];
					indices[0] = 0;
					for(int i = 1; i < indices.Length; i++) {
						indices[i] = Aspects.IndexOf(newModel.GetAspect(i)) + 1;
					}
					newModel.MoveAspects(indices);
					newModel.aspects = new List<string>(Aspects);
				}
			}
		}
		protected void MergeAspectToLayer(ModelNode layer) {
			ModelApplicationBase model = layer as ModelApplicationBase;
			if(model.AspectCount > 1) {
				foreach(string aspect in model.Aspects) {
					AddAspect(aspect);
				}
			}
		}
		protected override int GetRootParentAspectIndex(int aspectIndex) {
			List<string> aspectsCore = Master == null ? Aspects : ((ModelApplicationBase)Master).Aspects;
			if(aspectIndex >= aspectsCore.Count + 1 || aspectIndex == 0)
				return 0;
			if(this.aspectIndexRelations == null || this.aspectIndexRelations.Length <= aspectIndex) {
				this.aspectIndexRelations = new int[aspectIndex + 1];
				for(int i = 0; i < this.aspectIndexRelations.Length; i++) {
					this.aspectIndexRelations[i] = -1;
				}
			}
			if(this.aspectIndexRelations[aspectIndex] < 0) {
				string aspect = aspectsCore[aspectIndex - 1];
				int calculatedParentIndex = GetFirstAvailableParentIndex(aspect, aspectsCore);
				this.aspectIndexRelations[aspectIndex] = calculatedParentIndex;
			}
			return this.aspectIndexRelations[aspectIndex];
		}
		protected override bool IsCreatingMasterNodesInThisLayer { get { return this.creatingMasterNodeCounter > 0; } }
		protected override void StartCreateMasterNodes() { this.creatingMasterNodeCounter++; }
		protected override void EndCreateMasterNodes() { this.creatingMasterNodeCounter--; }
		#region IModeApplicationServices Members
		Version IModelApplicationServices.GetModuleVersion(string moduleName) {
			Guard.ArgumentNotNullOrEmpty(moduleName, "moduleName");
			Version result;
			return schemaModuleInfos != null && schemaModuleInfos.TryGetValue(moduleName, out result) ? result : null;
		}
		void IModelApplicationServices.SetSchemaModuleInfos(Dictionary<string, Version> schemaModuleInfos) {
			this.schemaModuleInfos = new Dictionary<string, Version>(schemaModuleInfos);
		}
		#endregion
		#region ICloneable Members
		object ICloneable.Clone() {
			return Clone();
		}
		#endregion
		#region IModelSources Members
		IEnumerable<Type> boModelTypes;
		IEnumerable<Controller> controllers;
		EditorDescriptors editorDescriptor;
		IEnumerable<IXafResourceLocalizer> localizerTypes;
		private IEnumerable<ModuleBase> modules;
		IEnumerable<Type> IModelSources.BOModelTypes {
			get {
				IEnumerable<Type> result = boModelTypes != null ? boModelTypes : GetMasterApplicationOrThis().boModelTypes;
				return result != null ? result : Type.EmptyTypes;
			}
			set { boModelTypes = value; }
		}
		IEnumerable<Controller> IModelSources.Controllers {
			get {
				IEnumerable<Controller> result = controllers != null ? controllers : GetMasterApplicationOrThis().controllers;
				return result != null ? result : new Controller[] { };
			}
			set { controllers = value; }
		}
		EditorDescriptors IModelSources.EditorDescriptors {
			get {
				EditorDescriptors result = editorDescriptor != null ? editorDescriptor : GetMasterApplicationOrThis().editorDescriptor;
				return result != null ? result : new EditorDescriptors(new EditorDescriptor[] { });
			}
			set { editorDescriptor = value; }
		}
		IEnumerable<IXafResourceLocalizer> IModelSources.Localizers {
			get {
				IEnumerable<IXafResourceLocalizer> result = localizerTypes != null ? localizerTypes : GetMasterApplicationOrThis().localizerTypes;
				return result != null ? result : new IXafResourceLocalizer[] { };
			}
			set { localizerTypes = value; }
		}
		IEnumerable<ModuleBase> IModelSources.Modules {
			get {
				IEnumerable<ModuleBase> result = modules != null ? modules : GetMasterApplicationOrThis().modules;
				return result != null ? result : ModuleBase.EmptyModules;
			}
			set { modules = value; }
		}
		#endregion
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ModelApplicationLayerIds {
		internal const string Application = "Application";
		internal const string AfterSetup = "After Setup";
		internal const string UnchangedMasterPart = "Unchanged Master Part";
		internal const string UserDiffs = "UserDiff";
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string Generator = "Generator Layer";
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ModelApplicationHelper {
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void AddLayer(ModelApplicationBase model, ModelApplicationBase layer) {
			Guard.ArgumentNotNull(model, "model");
			Guard.ArgumentNotNull(layer, "layer");
			model.AddLayerInternal(layer);
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void RemoveLayer(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			if(model.LayersCount > 0) {
				model.RemoveLayerInternal(model.LastLayer);
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelApplicationBase CreateModel(ITypesInfo typesInfo, IEnumerable<Type> boModelTypes, IEnumerable<ModuleBase> modules, DevExpress.ExpressApp.Core.ControllersManager controllersManager, IEnumerable<Type> localizerTypes, IEnumerable<string> aspects, string modelAssebmlyFile, ModelStoreBase modelDifferenceStore) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			Guard.ArgumentNotNull(boModelTypes, "boModelTypes");
			Guard.ArgumentNotNull(modules, "modules");
			Guard.ArgumentNotNull(controllersManager, "controllersManager");
			Guard.ArgumentNotNull(localizerTypes, "localizerTypes");
			Guard.ArgumentNotNull(aspects, "aspects");
			ApplicationModelManager manager = new ApplicationModelManager();
			manager.Setup(typesInfo, boModelTypes, modules, controllersManager.Controllers, localizerTypes, aspects, modelDifferenceStore, modelAssebmlyFile);
			ModelApplicationBase model = manager.CreateModelApplication(new ModelApplicationBase[0]);
			return model;
		}
	}
}
