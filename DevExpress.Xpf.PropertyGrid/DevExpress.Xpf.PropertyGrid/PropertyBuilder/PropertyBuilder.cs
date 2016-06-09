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

#define NEW_DEFINITIONS_SEARCH
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Editors.Settings;
using System.Runtime.Serialization;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Editors.Settings.Extension;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PropertyGrid {
	public enum PropertyBuilderMode { DefinedOnly, AllProperties }	
	public class PropertyBuilder : FrameworkElement {
		public static readonly DependencyProperty DefinitionsProperty;
		static PropertyBuilder() {
			DefinitionsProperty = DependencyPropertyManager.Register("Definitions", typeof(PropertyDefinitionCollection), typeof(PropertyBuilder), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyBuilder)d).OnDefinitionsChanged((PropertyDefinitionCollection)e.OldValue, (PropertyDefinitionCollection)e.NewValue)));
		}
		readonly StandardDefinitionsProvider standardDefinitionsProvider;
		readonly PropertyGridControl propertyGrid;
		public StandardDefinitionsProvider StandardDefinitionsProvider {
			get { return standardDefinitionsProvider; }
		}
		public PropertyGridControl PropertyGrid { get { return propertyGrid; } }
		PropertyDefinitionCollection definitions;
		public PropertyDefinitionCollection Definitions {
			get { return definitions; }
			set { SetValue(DefinitionsProperty, value); }
		}
		protected override IEnumerator LogicalChildren {
			get { return new Core.Native.MergedEnumerator(StandardDefinitionsProvider.CreatedDefinitions, Definitions.GetEnumerator()); }
		}
		public void CheckAddCustomDefinition(PropertyDefinitionBase definition) {
			if (definition == null)
				return;
			if (Definitions.Contains(definition))
				AddLogicalChild(definition);
		}
		protected internal void InternalAddLogicalChild(object element) {
			AddLogicalChild(element);
		}
		protected internal void InternalRemoveLogicalChild(object element) {
			RemoveLogicalChild(element);
		}
		public void CheckRemoveCustomDefinition(PropertyDefinitionBase definition) {
			if (definition == null)
				return;
			if (definition.Parent == this)
				RemoveLogicalChild(definition);
		}				
		internal event PropertyBuilderChangedEventHandler Changed;
		public PropertyBuilder(PropertyGridControl propertyGrid) {
			this.propertyGrid = propertyGrid;
			standardDefinitionsProvider = new StandardDefinitionsProvider(this);
		}
		bool isInitializing = false;
		public override void BeginInit() {
			isInitializing = true;
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			this.UpdateLayout();
			isInitializing = false;
			RaiseChanged();
		}
		internal void RaiseChanged(PropertyDefinitionBase definition = null, PropertyBuilderChangeKind changeKind = PropertyBuilderChangeKind.Reset) {			
			if (!isInitializing && Changed != null) {
				Changed(this, new PropertyBuilderChangedEventArgs(definition, changeKind));
			}
		}
		protected virtual void OnDefinitionsChanged(PropertyDefinitionCollection oldValue, PropertyDefinitionCollection newValue) {
			definitions = newValue;
			if (oldValue != null) {
				oldValue.CollectionChanged -= OnDefinitionsCollectionChanged;
				ClearDefinitions(oldValue);
			}
			if (newValue != null) {
				newValue.CollectionChanged += OnDefinitionsCollectionChanged;
				SetUpDefinitions(newValue);
			}
			RaiseChanged();
		}
		public void ClearUnlinkedDefinitions() {
			StandardDefinitionsProvider.ClearUnlinkedDefinitions();
		}
		public void Invalidate(DataViewBase view, RowHandle handle) {
			StandardDefinitionsProvider.Invalidate(view, handle);
		}
		protected virtual void OnDefinitionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			ClearDefinitions(e.OldItems);
			SetUpDefinitions(Definitions);
			RaiseChanged();
		}
		protected internal void ClearDefinitions(IList collection) {
			if (collection == null)
				return;
			foreach (var element in collection.OfType<PropertyDefinitionBase>()) {
				element.Builder = null;
			}
		}
		protected internal void SetUpDefinitions(IList collection) {
			if (collection == null)
				return;
			foreach (var element in collection.OfType<PropertyDefinitionBase>()) {
				element.Builder = this;
			}
		}		
		protected internal virtual BaseEditSettings GetStandardSettings(DataViewBase view, RowHandle handle) {
			return (GetStandardDefinition(view, handle) as PropertyDefinition).With(x => x.EditSettings);
		}		
		protected internal PropertyDefinitionBase GetStandardDefinition(DataViewBase view, RowHandle handle) {
			return StandardDefinitionsProvider.GetStandardDefinition(view, handle);		  
		}
#if !NEW_DEFINITIONS_SEARCH
		protected internal static bool ComparePath(string first, string second) {
			var fstrings = FieldNameHelper.GetPath(first).ToList().Do(x=>RemoveCategoryPath(x));
			var sstrings = FieldNameHelper.GetPath(second).ToList().Do(x=>RemoveCategoryPath(x));			
			if (fstrings.Count != sstrings.Count)
				return false;
			for (int i = 0; i < fstrings.Count; i++) {
				if (fstrings[i] != sstrings[i] && !String.Equals(fstrings[i], "*") && !String.Equals(sstrings[i], "*"))
					return false;
			}
			return true;
		}
		private static void RemoveCategoryPath(List<string> fstrings) {
			if (fstrings.Count > 1 && fstrings[0].StartsWith("<") && fstrings[0].EndsWith(">"))
				fstrings.RemoveAt(0);
		}
#endif
#if NEW_DEFINITIONS_SEARCH
		enum DefinitionCheckResult {
			Failure,
			Success,
			Continue,
		}
		class DefinitionObject {
			const int strongPathMatch = 0x40000; 
			const int weakPathMatch = 0x20000; 
			const int collectionMatch = 0x10000; 
			const int pathMask = strongPathMatch | weakPathMatch;
			const int typeMask = 0xffff; 
			PropertyDefinitionBase definition;
			DefinitionObject parent;
			PropertyGridControl propertyGrid;
			public PropertyDefinitionBase Definition { get { return definition; } }
			public PropertyGridControl PropertyGrid { get { return propertyGrid; } }
			bool? containsCategoryInfo = null;
			protected bool ContainsCategoryInfo { get { return (bool)(containsCategoryInfo ?? (containsCategoryInfo = GetContainsCategoryInfo())); } }
			private bool? GetContainsCategoryInfo() {
				if (definition.ParentDefinition != null)
					return false;
				if (definition.SplittedPath == null)
					return false;
				if (definition.SplittedPath.Length == 0)
					return false;
				var first = definition.SplittedPath[0];
				if (first.Length == 0)
					return false;
				return first[0] == '<';
			}
			public DefinitionObject(PropertyGridControl propertyGrid, PropertyDefinitionBase definition, DefinitionObject parent = null) {
				this.definition = definition;
				this.parent = parent;
				this.propertyGrid = propertyGrid;
			}
			int checkAttempt = 0;
			Dictionary<int, int> iterationResult = new Dictionary<int, int>();
			public DefinitionCheckResult Check(BuilderFieldInfo fieldInfo, int iteration, ApplyingMode mode) {
				int matchResult = 0;
				try {					
					if (!definition.ApplyingMode.HasFlag(mode))
						return DefinitionCheckResult.Failure;
					bool definitionIsCategory = (definition is CategoryDefinition);
					if (definitionIsCategory != fieldInfo.IsCategory) {
						if (!ContainsCategoryInfo)
							checkAttempt--;
						return definitionIsCategory ? DefinitionCheckResult.Failure : DefinitionCheckResult.Continue;
					}						
					if ((definition is CollectionDefinition) && !fieldInfo.IsCollection) {
						return DefinitionCheckResult.Failure;
					}
					if (definition is CollectionDefinition && fieldInfo.IsCollection)
						matchResult = collectionMatch;
					var path = Definition.SplittedPath;
					if (path != null) {
						if (path.Length <= checkAttempt)
							return DefinitionCheckResult.Failure;
						var skippedDefinitionPath = path.Skip(checkAttempt).ToArray();
						var currentDefinitionPath = skippedDefinitionPath.First();
						var currentFieldName = fieldInfo.SplittedPath.Last();
						if (currentDefinitionPath != "*") {
							if (currentDefinitionPath != currentFieldName)
								return DefinitionCheckResult.Failure;
							matchResult |= strongPathMatch;
						} else {
							matchResult |= weakPathMatch;
						}
						if (skippedDefinitionPath.Length > 1)
							return DefinitionCheckResult.Continue;
					}
					int typeMatchValue = 0;
					var propertyDefinition = definition as PropertyDefinition;
					if (propertyDefinition != null) {
						var pDefType = propertyDefinition.Type;
						if (pDefType != null) {
							var fInfType = fieldInfo.Type;
							if (propertyDefinition.TypeMatchMode == TypeMatchMode.Direct) {
								if (pDefType == fInfType)
									typeMatchValue = 0;
								else if (TypeHelper.IsNullableOf(fInfType, pDefType))
									typeMatchValue = 1;
								else
									return ((matchResult & pathMask) == 0) ? DefinitionCheckResult.Continue : DefinitionCheckResult.Failure;
							}
							if (propertyDefinition.TypeMatchMode == TypeMatchMode.Extended) {
								bool checkInterface = pDefType.IsInterface;
								do {
									if (fInfType == null) {
										return ((matchResult & pathMask) == 0) ? DefinitionCheckResult.Continue : DefinitionCheckResult.Failure;
									}
									if (fInfType == pDefType)
										break;
									if(TypeHelper.IsNullableOf(fInfType, pDefType)) {
										typeMatchValue++;
										break;
									}
									if (checkInterface) {
										var assignableInterface = fInfType.GetInterfaces().FirstOrDefault(x => pDefType.IsAssignableFrom(x));
										if (assignableInterface != null) {
											int delta = 1;
											while (assignableInterface != pDefType && assignableInterface != null) {
												delta++;
												assignableInterface = assignableInterface.BaseType;
											}
											typeMatchValue += delta;
											break;
										}										
									}
									typeMatchValue++;
									fInfType = fInfType.BaseType;
								} while (true);
							}
						}
					}
					matchResult |= (~typeMatchValue) & typeMask;
					return DefinitionCheckResult.Success;
				} finally {
					checkAttempt++;
					iterationResult[iteration] = matchResult;
				}
			}
			public int GetCheckResult(int iteration) {
				int result = 0;
				if (iterationResult.TryGetValue(iteration, out result))
					return result;
				if (parent == null)
					return 0;
				return parent.GetCheckResult(iteration);
			}
			List<DefinitionObject> children;
			public IEnumerable<DefinitionObject> Children {
				get {
					if (children!=null)
						return children;
					children = new List<DefinitionObject>();
					foreach(var element in Definition.PropertyDefinitions) {
						children.Add(new DefinitionObject(propertyGrid, element, this));
					}
					var pDef = definition as PropertyDefinition;
					if (pDef != null && pDef.InsertDefinitionsFrom != null) {
						PropertyDefinitionBase insertFrom = pDef.InsertDefinitionsFrom;
						if (insertFrom is RootPropertyDefinition)
							insertFrom = PropertyGrid.RootPropertyDefinition;
						foreach (var element in insertFrom.PropertyDefinitions)
							children.Add(new DefinitionObject(propertyGrid, element, this));
					}
					return children;
				}
			}
		}
		struct BuilderFieldInfo {
			public Type Type;
			public string Path;
			public bool IsCategory;
			public bool IsCollection;
			public string[] SplittedPath;
			public BuilderFieldInfo(string path, Type type, bool isCategory,bool isCollection) {
				this.Path = path;
				this.Type = type;
				this.IsCategory = isCategory;
				this.IsCollection = isCollection;
				this.SplittedPath = FieldNameHelper.GetPath(path);
			}
		}
		List<BuilderFieldInfo> GetFieldInfos(DataViewBase view, RowHandle handle) {
			List<BuilderFieldInfo> infos = new List<BuilderFieldInfo>();
			while (handle != null && !handle.IsRoot) {
				infos.Insert(0, new BuilderFieldInfo(view.GetFieldNameByHandle(handle), view.GetPropertyType(handle), view.IsGroupRowHandle(handle), view.IsCollectionHandle(handle)));
				handle = view.GetParent(handle);
			}				
			return infos;
		}
		public virtual PropertyDefinitionBase GetDefinition(DataViewBase view, RowHandle handle, bool showCategories, bool getStandard = true) {
			if (handle.IsRoot)
				return null;
			if (Definitions.Count == 0) {
				return getStandard ? GetStandardDefinition(view, handle) : null;
			}
			var path = GetFieldInfos(view, handle);			
			List<DefinitionObject> candidates = new List<DefinitionObject>(Definitions.Count);
			List<DefinitionObject> definitions = new List<DefinitionObject>(Definitions.Count);
			for(int i = 0; i<Definitions.Count; i++) {
				definitions.Add(new DefinitionObject(PropertyGrid, Definitions[i], null));
			}
			var last = path.Last();
			var currentMode = view is CategoryDataView ? ApplyingMode.WhenGrouping : ApplyingMode.WhenNoGrouping;
			int iteration = -1;
			foreach (var fieldInfo in path) {
				iteration++;
				var isLast = fieldInfo.Equals(last);
				var step = definitions.ToList();
				definitions.Clear();
				foreach (var def in step) {
					DefinitionCheckResult checkResult = def.Check(fieldInfo, iteration, currentMode);
					if (checkResult == DefinitionCheckResult.Continue)
						definitions.Add(def);					
					if (checkResult == DefinitionCheckResult.Success) {
						if (isLast)
							candidates.Add(def);
						else
							definitions.AddRange(def.Children);
					}
				}
			}
			DefinitionObject defObj = null;
			for (int i = 0; i <= iteration; i++) {
				int maxValue = -1;				
				foreach(var element in candidates) {
					int cr = element.GetCheckResult(i);
					if (cr > maxValue) {
						maxValue = cr;
						defObj = element;
					}
				}
			}
			return defObj.With(x => x.Definition) ?? (getStandard ? GetStandardDefinition(view, handle) : null);
		}
#else
				List<FieldInfo> GetFullPath(DataController controller, DataViewBase view, RowHandle handle) {
			List<FieldInfo> fullPath = new List<FieldInfo>();
			foreach (var h in GetParentHandles(view, handle)) {
				var fi = new FieldInfo(controller, view, h);
				if (fullPath.Count != 0)
					fullPath[0].Parent = fi;
				fullPath.Insert(0, fi);
			}
			return fullPath;
		}
		List<PropertyDefinitionInfo> GetDefinitionInfosBranch(PropertyDefinitionInfo baseDefinition) {
			List<PropertyDefinitionInfo> infos = new List<PropertyDefinitionInfo>();
			while (baseDefinition != null) {
				infos.Insert(0, baseDefinition);
				baseDefinition = baseDefinition.Parent;
			}
			return infos;
		}
		class ConditionalIntegerComparer : IComparer<ConditionalInteger> {
			[ThreadStatic]
			static ConditionalIntegerComparer default_;
			public static ConditionalIntegerComparer Default {
				get {
					if (default_ == null) {
						default_ = new ConditionalIntegerComparer();
					}
					return default_;
				}
				set { default_ = value; }
			}
			public int Compare(ConditionalInteger x, ConditionalInteger y) {
				if (x.Condition != y.Condition) {
					if (x.IsStandard)
						return -1;
					if (y.IsStandard)
						return 1;
					if (x.Condition == PropertyDefinitionInfo.RelationModes.None)
						return -1;
					if (y.Condition == PropertyDefinitionInfo.RelationModes.None)
						return 1;
					if (x.Condition == PropertyDefinitionInfo.RelationModes.Type)
						return -1;
					if (y.Condition == PropertyDefinitionInfo.RelationModes.Type)
						return 1;
				}
				else {
					if (x.Condition == PropertyDefinitionInfo.RelationModes.None)
						return 0;
					if (x.Condition != PropertyDefinitionInfo.RelationModes.Type)
						return Math.Sign(x.Value - y.Value);
					else {
						for (int i = 0; i < Math.Min(x.Types.Length, y.Types.Length); i++) {
							if (x.Types[i] == y.Types[i])
								continue;
							return x.Types[i].IsSubclassOf(y.Types[i]) ? -1 : 1;
						}
						if (x.Types.Length != y.Types.Length)
							return x.Types.Length > y.Types.Length ? 1 : -1;
						var result = x.IsStandard ? -1 : (y.IsStandard ? 1 : 0);
						return result;
					}
				}
				int xValue = x.Value + x.Condition == PropertyDefinitionInfo.RelationModes.Path ? 1 : 0;
				int yValue = y.Value + y.Condition == PropertyDefinitionInfo.RelationModes.Path ? 1 : 0;
				return Math.Sign(x.Value - y.Value);
			}
		}
		class PropertyDefinitionEqualityComparer : IEqualityComparer<PropertyDefinitionBase> {
			[ThreadStatic]
			static PropertyDefinitionEqualityComparer _default = new PropertyDefinitionEqualityComparer();
			public static PropertyDefinitionEqualityComparer Default {
				get { return _default; }
			}
			public bool Equals(PropertyDefinitionBase x, PropertyDefinitionBase y) {
				return x != null && y != null
				 && x.ownTypeHashCode == y.ownTypeHashCode
				 && x.pathHashCode == y.pathHashCode
				 && x.typeHashCode == y.typeHashCode
				 && x.scopeHashCode == y.scopeHashCode;
			}
			public int GetHashCode(PropertyDefinitionBase obj) {
				return obj.GetHashCode();
			}
		}
		class FieldInfo {
			DataController controller;
			DataViewBase view;
			RowHandle handle;
			public bool IsCategory { get; private set; }
			public string Name { get; private set; }
			public string FullName { get; private set; }
			public Type Type { get; private set; }
			public FieldInfo Parent { get; set; }			
			public FieldInfo(DataController controller, DataViewBase view, RowHandle handle) {
				this.controller = controller;
				this.view = view;
				this.handle = handle;
				IsCategory = controller.IsGroupRowHandle(handle);
				Name = controller.GetDisplayName(handle);
				if (!IsCategory) {
					Type = controller.GetPropertyType(handle);					
				}
				FullName = view.GetFieldNameByHandle(handle);
			}
			public FieldInfo GetAncestor(Func<FieldInfo, bool> predicate) {
				var parent = Parent;
				while (parent != null) {
					if (predicate(parent))
						return parent;
					parent = parent.Parent;
				}
				return null;
			}
			public bool CheckCanApply(PropertyDefinitionBase definitionBase, string relativeDefinitionPath = null) {
				return CheckCanApplyIfCategory(definitionBase)
					&& CheckCanApplyIfCollection(definitionBase)
					&& CheckCanApplyByPath(definitionBase, relativeDefinitionPath)
					&& CheckCanApplyByType(definitionBase);				
			}
			public bool CheckCanApplyIfCategory(PropertyDefinitionBase definitionBase) {
				return IsCategory == definitionBase.IsCategory;
			}
			public bool CheckCanApplyByPath(PropertyDefinitionBase definitionBase, string relativeDefinitionPath = null) {
				string definitionPathModifier = String.IsNullOrEmpty(relativeDefinitionPath) ? "" : relativeDefinitionPath + ".";
				PropertyDefinition definition = definitionBase as PropertyDefinition;
				if (definition != null && !String.IsNullOrEmpty(definition.Scope)) {
					var parent = GetAncestor(fp => PropertyBuilder.ComparePath(fp.FullName, definitionPathModifier + definition.Scope));
					if (parent == null)
						return false;
					if (!String.IsNullOrEmpty(definition.Path)) {
						if (!PropertyBuilder.ComparePath(FullName, (parent.FullName + "." + definition.Path)))
							return false;
					}
					if (!CheckCanApplyByType(definition))
						return false;
					return true;
				}
				return String.IsNullOrEmpty(definitionBase.Path) || PropertyBuilder.ComparePath(FullName, definitionPathModifier + definitionBase.Path);
			}
			bool CheckCanApplyByType(PropertyDefinitionBase definitionBase) {
				PropertyDefinition definition = definitionBase as PropertyDefinition;
				return definition==null || definition.Type == null || Type == definition.Type || TypeHelper.IsNullableOf(Type, definition.Type);
			}
			bool CheckCanApplyIfCollection(PropertyDefinitionBase definitionBase) {
				if (definitionBase is CollectionDefinition)
					return view.CanUseCollectionEditor(handle);
				return true;	
			}
			public override string ToString() {
				return FullName;
			}
		}
		class ConditionalInteger {
			public ConditionalInteger(int index, int value, bool isStandard, PropertyDefinitionInfo.RelationModes condition, Type[] types) {
				this.Value = value;
				this.Condition = condition;
				this.Types = types;
				this.Index = index;
				this.IsStandard = isStandard;
			}
			public int Index { get; set; }
			public int Value { get; set; }
			public bool IsStandard { get; set; }
			public Type[] Types { get; set; }
			public PropertyDefinitionInfo.RelationModes Condition { get; set; }
		}
		class PropertyDefinitionInfo {
			public enum RelationModes { Path, Scope, Type, None }
			public PropertyDefinitionInfo Parent { get; private set; }
			public PropertyDefinitionBase Definition { get; private set; }
			public FieldInfo RelatedFieldInfo { get; private set; }
			public RelationModes RelationMode { get; private set; }
			public string RelatedInfoName { get; private set; }
			public List<FieldInfo> PossibleChildren { get; private set; }
			IList<PropertyDefinitionBase> childrenDefinitions = null;
			public IList<PropertyDefinitionBase> ChildrenDefinitions {
				get {
					if (childrenDefinitions == null) {
						int fake = 0;
						while (fake++ != 1) {
							if (!Definition.IsCategory) {
								PropertyDefinition pDef = (PropertyDefinition)Definition;
								if (pDef.InsertDefinitionsFrom == null) {
									childrenDefinitions = Definition.PropertyDefinitions;
									break;
								}
								IList<PropertyDefinitionBase> result = new List<PropertyDefinitionBase>();
								if (pDef.PropertyDefinitions != null) {
									for (int i = 0; i < pDef.PropertyDefinitions.Count; i++) {
										result.Add(pDef.PropertyDefinitions[i]);
									}
								}
								var collection = pDef.InsertDefinitionsFrom is RootPropertyDefinition ? Definition.Builder.Definitions : pDef.InsertDefinitionsFrom.PropertyDefinitions;
								if (collection != null) {
									for (int i = 0; i < collection.Count; i++) {
										if (!result.Contains(collection[i], PropertyDefinitionEqualityComparer.Default))
											result.Add(collection[i]);
									}
								}
								childrenDefinitions = result;
								break;
							}
							else {
								CategoryDefinition cDef = (CategoryDefinition)Definition;
								IList<PropertyDefinitionBase> result = new List<PropertyDefinitionBase>();
								if (cDef.PropertyDefinitions != null) {
									for (int i = 0; i < cDef.PropertyDefinitions.Count; i++) {
										result.Add(cDef.PropertyDefinitions[i]);
									}
								}								
								childrenDefinitions = result;
								break;
							}
						}
					}
					return childrenDefinitions;
				}
			}
			public PropertyDefinitionInfo(PropertyDefinitionBase definition, FieldInfo info, List<FieldInfo> allFields, PropertyDefinitionInfo parent) {
				RelationMode = RelationModes.None;
				Definition = definition;
				RelatedFieldInfo = info;
				RelatedInfoName = info.FullName;
				int index = allFields.IndexOf(info);
				Parent = parent;
				PossibleChildren = new List<FieldInfo>(allFields.Where((f, i) => i > index).ToArray());
				if ((Definition is PropertyDefinition) && ((PropertyDefinition)Definition).Type != null)
					RelationMode = RelationModes.Type;
				if ((Definition is PropertyDefinition) && !String.IsNullOrEmpty(((PropertyDefinition)Definition).Scope))
					RelationMode = RelationModes.Scope;
				if (!String.IsNullOrEmpty(Definition.Path))
					RelationMode = RelationModes.Path;
			}
		}
		public virtual PropertyDefinitionBase GetDefinition(DataController controller, DataViewBase view, RowHandle handle, bool showCategories, bool getStandard = true) {
			List<FieldInfo> fullPath = GetFullPath(controller, view, handle);
			if (fullPath.Count == 0)
				return getStandard ? GetStandardDefinition(controller, view, handle) : null;
			List<PropertyDefinitionInfo> uncheckedDefinitionInfos = new List<PropertyDefinitionInfo>();
			List<PropertyDefinitionInfo> checkedDefinitionInfos = new List<PropertyDefinitionInfo>();
			ApplyingMode currentMode = showCategories ? ApplyingMode.WhenGrouping : ApplyingMode.WhenNoGrouping;
			for (int i = Definitions.Count - 1; i >= 0; i--) {
				PropertyDefinitionBase baseDefinition = Definitions[i];
				if ((baseDefinition.ApplyingMode & currentMode) == 0) {
					continue;
				}
				foreach (var fieldInfo in fullPath.Where(fi => fi.CheckCanApply(baseDefinition)))
					uncheckedDefinitionInfos.Add(new PropertyDefinitionInfo(baseDefinition, fieldInfo, fullPath, null));
			}
			while (uncheckedDefinitionInfos.Count != 0) {
				for (int i = uncheckedDefinitionInfos.Count - 1; i >= 0; i--) {
					var definitionInfo = uncheckedDefinitionInfos[i];
					if (definitionInfo.PossibleChildren.Count == 0) {
						checkedDefinitionInfos.Add(definitionInfo);
						uncheckedDefinitionInfos.RemoveAt(i);
						continue;
					}
					if (definitionInfo.ChildrenDefinitions.Count == 0) {
						uncheckedDefinitionInfos.RemoveAt(i);
						continue;
					}
					for (int j = 0; j < definitionInfo.ChildrenDefinitions.Count; j++) {
						PropertyDefinitionBase definition = definitionInfo.ChildrenDefinitions[j];
						foreach (var fieldInfo in definitionInfo.PossibleChildren.Where(fi => fi.CheckCanApply(definition, definitionInfo.RelatedInfoName)))
							uncheckedDefinitionInfos.Add(new PropertyDefinitionInfo(definition, fieldInfo, definitionInfo.PossibleChildren, definitionInfo));
					}
					uncheckedDefinitionInfos.RemoveAt(i);
				}
			}
			var checkedDefinitions = checkedDefinitionInfos.Select(di => GetDefinitionInfosBranch(di)).ToList();
			var standardDefinition = getStandard ? GetStandardDefinition(controller, view, handle) : null;
			if (standardDefinition != null) {
				checkedDefinitions.Add(new List<PropertyDefinitionInfo> { new PropertyDefinitionInfo(standardDefinition, fullPath[fullPath.Count - 1], fullPath, null) });
			}
			if (checkedDefinitions.Count == 0)
				return null;
			while (checkedDefinitions.Count != 1) {
				List<ConditionalInteger> values = new List<ConditionalInteger>();
				for (int i = 0; i < checkedDefinitions.Count; i++) {
					var currentInfos = checkedDefinitions[i];
					int j = 0;
					for (; j < currentInfos.Count; j++) {
						if (currentInfos[j].RelationMode != currentInfos[0].RelationMode)
							break;
					}
					if (j == currentInfos.Count)
						j--;
					values.Add(new ConditionalInteger(i,
					fullPath.IndexOf(currentInfos[j].RelatedFieldInfo),
					currentInfos[0].Definition.isStandardDefinition,
					currentInfos[0].RelationMode,
					currentInfos.Where((cd, ind) => { return ind < j; }).Select(cd => (cd.Definition as PropertyDefinition).With(x => x.Type)).ToArray()));
					currentInfos.RemoveRange(0, j);
				}
				values = values.OrderByDescending(x => x, ConditionalIntegerComparer.Default).ToList();
				int l = 0;
				bool canRemove = false;
				for (; l < values.Count - 1; l++) {
					if (ConditionalIntegerComparer.Default.Compare(values[l], values[l + 1]) != 0) {
						canRemove = true;
						l++;
						break;
					}
				}
				if (canRemove || checkedDefinitions.All(x => x.Count <= 1)) {
					List<List<PropertyDefinitionInfo>> toRemove = new List<List<PropertyDefinitionInfo>>();
					for (l = values.Count - l; l < values.Count; l++) {
						toRemove.Add(checkedDefinitions[values[l].Index]);
					}
					foreach (var element in toRemove)
						checkedDefinitions.Remove(element);
				}
			}
			return checkedDefinitions[0].Last().Definition;
		}
#endif
		public static IEnumerable<RowHandle> GetParentHandles(DataViewBase view, string fieldName) {
			return GetParentHandles(view, view.GetHandleByFieldName(fieldName));
		}
		public static IEnumerable<RowHandle> GetParentHandles(DataViewBase view, RowHandle handle) {
			if (handle.IsInvalid || handle.IsRoot) yield break;
			var lhandle = handle;
			while (lhandle != null && lhandle != RowHandle.Root) {
				yield return lhandle;
				lhandle = view.GetParent(lhandle);
			}
		}
		public PropertyGridSortMode GetActualSortMode(RowHandle handle, RowDataGenerator generator) {
			if (handle == RowHandle.Root)
				return PropertyGridSortMode.Unspecified;
			var definition = generator.RowDataFromHandle(handle).With(x => x.Definition);
			while (definition != null) {
				if (definition.ChildrenSortMode != PropertyGridSortMode.Unspecified)
					return definition.ChildrenSortMode;
				definition = definition.ParentDefinition;
			}
			return PropertyGridSortMode.Unspecified;
		}
	}
	internal delegate void PropertyBuilderChangedEventHandler(object sender, PropertyBuilderChangedEventArgs args);
	internal enum PropertyBuilderChangeKind {
		CoreProperties,
		MenuProperties,
		VisualClientProperties,
		Reset,
	}
	internal class PropertyBuilderChangedEventArgs : EventArgs {
		public PropertyDefinitionBase Definition { get; set; }
		public PropertyBuilderChangeKind ChangeKind { get; set; }
		public PropertyBuilderChangedEventArgs(PropertyDefinitionBase definition, PropertyBuilderChangeKind kind) {
			this.Definition = definition;
			this.ChangeKind = kind;
		}
	}
	internal static class TypeHelper {
		public static bool IsNullableOf(Type nullable, Type argument) {
			if (!nullable.IsValueType || argument == null)
				return false;
			return Nullable.GetUnderlyingType(nullable) == argument;
		}
	}	
}
