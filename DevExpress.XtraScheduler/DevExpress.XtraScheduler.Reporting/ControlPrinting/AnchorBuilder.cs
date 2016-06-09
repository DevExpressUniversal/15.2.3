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
using DevExpress.XtraScheduler.Drawing;
using System.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public enum AnchorDirection { Horizontal, Vertical };
	public class AnchorInfo {
		AnchorBase anchor;
		bool isFirstAnchor;
		public AnchorInfo(AnchorBase anchor, bool firstAnchor) {
			Guard.ArgumentNotNull(anchor, "anchor");
			this.anchor = anchor;
			this.isFirstAnchor = firstAnchor;			
		}
		public AnchorBase Anchor { get { return anchor; } }
		public AnchorCollection InnerAnchors { get { return Anchor.InnerAnchors; } }
		public Rectangle Bounds { get { return Anchor.Bounds; } }
		public bool IsFirstAnchor { get { return isFirstAnchor; } }		
	}
	#region AnchorCollection
	public class AnchorCollection : DXCollection<AnchorBase>, ISchedulerObjectAnchorCollection {
		public AnchorCollection() {
			UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
		public Rectangle[] GetAnchorBounds() {
			return GetAnchorBounds(Count);
		}
		public Rectangle[] GetAnchorBounds(int count) {
			XtraSchedulerDebug.Assert(count <= this.Count);
			Rectangle[] result = new Rectangle[count];
			for (int i = 0; i < count; i++)
				result[i] = this[i].Bounds;
			return result;
		}
		public TimeIntervalCollection GetAnchorIntervals() {
			TimeIntervalCollection result = new TimeIntervalCollection();
			for (int i = 0; i < Count; i++)
				result.Add(this[i].Interval.Clone());
			return result;
		}
		public AnchorInfo GetAnchorInfo(int index) {
			bool isFirst = index == 0;			
			return new AnchorInfo(this[index], isFirst);
		}
		#region ISchedulerObjectAnchorCollection Members
		ISchedulerObjectAnchor ISchedulerObjectAnchorCollection.this[int index] {
			get { return this[index]; }
		}
		#endregion
	}
	#endregion
	#region ResourceAnchorCollection
	public class ResourceAnchorCollection : AnchorCollection {
		Resource resource;
		public ResourceAnchorCollection(Resource resource) {
			Guard.ArgumentNotNull(resource, "resource");
			this.resource = resource;
		}
		public Resource Resource { get { return resource; } }
		protected override int AddCore(AnchorBase value) {
			if (ResourceBase.MatchIds(Resource, value.Resource))
				return base.AddCore(value);
			else
				return -1;
		}
	}
	#endregion
	#region AnchorBase
	public abstract class AnchorBase : ISchedulerObjectAnchor, ICloneable {
		Rectangle bounds;
		TimeInterval interval;
		Resource resource;
		AnchorCollection innerAnchors;
		protected AnchorBase() {
			Initialize();
		}
		public Rectangle Bounds { get { return bounds; } set { this.bounds = value; } }
		public TimeInterval Interval { get { return interval; } set { this.interval = value; } }
		public Resource Resource { get { return resource; } set { this.resource = value; } }
		public AnchorCollection InnerAnchors { get { return innerAnchors; } }
		protected internal virtual void Initialize() {
			this.bounds = Rectangle.Empty;
			this.interval = TimeInterval.Empty;
			this.resource = ResourceBase.Empty;
			this.innerAnchors = new AnchorCollection();
		}
		#region ICloneable Members
		object ICloneable.Clone() {
			return Clone();
		}
		public AnchorBase Clone() {
			return CloneCore(true);
		}	   
		protected internal abstract AnchorBase CreateInstance();
		protected internal virtual AnchorBase CloneCore(bool cloneInnerAnchors) {
			AnchorBase clone = CreateInstance();
			clone.Resource = Resource;
			clone.Interval = Interval.Clone();
			clone.Bounds = Bounds;
			if (cloneInnerAnchors)
				clone.InnerAnchors.AddRange(CloneInnerAnchors());
			else
				clone.InnerAnchors.AddRange(InnerAnchors);
			return clone;
		}
		protected internal virtual AnchorCollection CloneInnerAnchors() {
			AnchorCollection result = new AnchorCollection();
			int count = InnerAnchors.Count;
			for (int i = 0; i < count; i++)
				result.Add(InnerAnchors[i].CloneCore(false));
			return result;
		}
		#endregion
		public virtual void CalculateInnerAnchorsBounds() {
			int count = InnerAnchors.Count;
			if (count == 0)
				return;
			Rectangle[] bounds = CalcualteInnerAnchorsBoundsCore();
			XtraSchedulerDebug.Assert(count == bounds.Length);
			for (int i = 0; i < count; i++)
				InnerAnchors[i].Bounds = bounds[i];
		}	   
		protected abstract Rectangle[] CalcualteInnerAnchorsBoundsCore();		
	}
	#endregion
	#region HorizontalAnchor
	public class HorizontalAnchor : AnchorBase {
		protected internal override AnchorBase CreateInstance() {
			return new HorizontalAnchor();
		}
		protected override Rectangle[] CalcualteInnerAnchorsBoundsCore() {
			return RectUtils.SplitHorizontally(Bounds, InnerAnchors.Count);
		}		
	}
	#endregion
	#region VerticalAnchor
	public class VerticalAnchor : AnchorBase {
		protected internal override AnchorBase CreateInstance() {
			return new VerticalAnchor();
		}
		protected override Rectangle[] CalcualteInnerAnchorsBoundsCore() {
			return RectUtils.SplitVertically(Bounds, InnerAnchors.Count);
		}
	}
	#endregion
	#region AnchorCreationInfo
	public class AnchorCreationInfo {
		#region static
		static AnchorCreationInfo defaultInfo;
		static AnchorCreationInfo() {
			defaultInfo = new AnchorCreationInfo();
		}
		public static AnchorCreationInfo Default { get { return defaultInfo; } }
		#endregion
		Rectangle controlBounds;
		AnchorCollection masterAnchors;
		ResourcePrintObjectsCollection resourcePrintObjects;
		AnchorDirection direction;
		public AnchorCreationInfo() {
		}
		public AnchorCreationInfo(AnchorCollection masterAnchors, ResourcePrintObjectsCollection printObjects, Rectangle controlBounds, AnchorDirection direction) {
			if (masterAnchors == null)
				Exceptions.ThrowArgumentNullException("masterAnchors");
			if (printObjects == null)
				Exceptions.ThrowArgumentNullException("printObjects");
			this.controlBounds = controlBounds;
			this.masterAnchors = masterAnchors;
			this.resourcePrintObjects = printObjects;
			this.direction = direction;
		}
		public Rectangle ControlBounds { get { return controlBounds; } }
		public AnchorCollection MasterAnchors { get { return masterAnchors; } }
		public ResourcePrintObjectsCollection ResourcePrintObjects { get { return resourcePrintObjects; } }
		internal AnchorDirection PrintDirection { get { return direction; } }
	}
	#endregion
	#region AnchorBuilderBase
	public abstract class AnchorBuilderBase {
		public AnchorCollection CreateAnchors(AnchorCreationInfo creationInfo) {
			if (creationInfo == null)
				Exceptions.ThrowArgumentNullException("creationInfo");
			if (creationInfo.ResourcePrintObjects.Count == 0)
				return new AnchorCollection();
			return CreateAnchorsCore(creationInfo);
		}
		protected internal virtual void InitializeAnchor(AnchorBase anchor, PrintObject printObject) {
			TimeInterval interval = printObject.Interval;
			Resource resource = printObject.Resource;
			if (!interval.Equals(TimeInterval.Empty))
				anchor.Interval = interval.Clone();
			if (resource != ResourceBase.Empty)
				anchor.Resource = resource;
		}
		protected internal virtual AnchorCollection CloneAnchors(AnchorCollection anchors) {
			AnchorCollection result = new AnchorCollection();
			foreach (AnchorBase anchor in anchors)
				result.Add(anchor.Clone());
			return result;
		}
		protected internal abstract AnchorCollection CreateAnchorsCore(AnchorCreationInfo creationInfo);
		public virtual void ApplyVerticalAnchorsIndent(AnchorCollection vAnchors, int firstAnchorIndent, int lastAnchorIndent) {
			if (vAnchors.Count == 0)
				return;
			ApplyTopIndent(vAnchors, firstAnchorIndent);
			ApplyBottomIndent(vAnchors, lastAnchorIndent);
		}
		public virtual void ApplyHorizontalAnchorsIndent(AnchorCollection hAnchors, int firstAnchorIndent, int lastAnchorIndent) {
			if (hAnchors.Count == 0)
				return;
			ApplyLeftIndent(hAnchors, firstAnchorIndent);
			ApplyRightIndent(hAnchors, lastAnchorIndent);
		}
		protected internal virtual void ApplyLeftIndent(AnchorCollection hAnchors, int indent) {
			AnchorBase anchor = hAnchors[0];
			IncreaseAnchorWidth(anchor, indent);
			RecalcInnerAnchorsBounds(anchor, indent, 0);
			int count = hAnchors.Count;
			for (int i = 1; i < count; i++) {
				anchor = hAnchors[i];
				OffsetAnchorBounds(anchor, indent, 0);
				RecalcInnerAnchorsBounds(anchor, indent, 0);
			}
		}
		protected internal virtual void ApplyTopIndent(AnchorCollection vAnchors, int indent) {
			AnchorBase anchor = vAnchors[0];
			IncreaseAnchorHeight(anchor, indent);
			RecalcInnerAnchorsBounds(anchor, 0, indent);
			int count = vAnchors.Count;
			for (int i = 1; i < count; i++) {
				anchor = vAnchors[i];
				OffsetAnchorBounds(anchor, 0, indent);
				RecalcInnerAnchorsBounds(anchor, 0, indent);
			}
		}
		protected internal virtual void ApplyRightIndent(AnchorCollection hAnchors, int indent) {
			int count = hAnchors.Count;
			AnchorBase anchor = hAnchors[count - 1];
			IncreaseAnchorWidth(anchor, indent);
		}
		protected internal virtual void ApplyBottomIndent(AnchorCollection vAnchors, int indent) {
			int count = vAnchors.Count;
			AnchorBase anchor = vAnchors[count - 1];
			IncreaseAnchorHeight(anchor, indent);
		}
		protected internal virtual void IncreaseAnchorHeight(AnchorBase anchor, int value) {
			Rectangle bounds = anchor.Bounds;
			bounds.Height += value;
			anchor.Bounds = bounds;
		}
		protected internal virtual void IncreaseAnchorWidth(AnchorBase anchor, int value) {
			Rectangle bounds = anchor.Bounds;
			bounds.Width += value;
			anchor.Bounds = bounds;
		}
		protected internal virtual void OffsetAnchorBounds(AnchorBase anchor, int hOffset, int vOffset) {
			Rectangle bounds = anchor.Bounds;
			bounds.Offset(hOffset, vOffset);
			anchor.Bounds = bounds;
		}
		protected internal virtual void RecalcInnerAnchorsBounds(AnchorBase anchor, int hIndent, int vIndent) {
			int count = anchor.InnerAnchors.Count;
			for (int i = 0; i < count; i++) {
				Rectangle bounds = anchor.InnerAnchors[i].Bounds;
				bounds.Offset(hIndent, vIndent);
				anchor.InnerAnchors[i].Bounds = bounds;
			}
		}
	}
	#endregion
	#region FitAnchorBuilderBase
	public abstract class FitAnchorBuilderBase : AnchorBuilderBase {
		protected internal override AnchorCollection CreateAnchorsCore(AnchorCreationInfo creationInfo) {
			AnchorCollection result = new AnchorCollection();
			AnchorCollection rootAnchors = CreateRootAnchors(creationInfo);
			int count = rootAnchors.Count;
			for (int i = 0; i < count; i++) {
				AnchorCollection masterAnchors = CreateMasterAnchors(rootAnchors[i], creationInfo.ResourcePrintObjects.Count);
				InitializeMasterAnchorsResource(masterAnchors, creationInfo.ResourcePrintObjects);
				CreateInnerAnchors(masterAnchors, creationInfo.ResourcePrintObjects);
				result.AddRange(masterAnchors);
			}
			return result;
		}
		protected internal virtual AnchorCollection CreateMasterAnchors(AnchorBase rootAnchor, int count) {
			CreateInnerAnchorsCore(rootAnchor, count);
			return rootAnchor.InnerAnchors;
		}
		protected internal virtual void InitializeMasterAnchorsResource(AnchorCollection masterAnchors, ResourcePrintObjectsCollection resourcePrintObjects) {
			XtraSchedulerDebug.Assert(masterAnchors.Count == resourcePrintObjects.Count);
			int count = masterAnchors.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resourcePrintObjects[i].Resource;
				if (resource != ResourceBase.Empty)
					masterAnchors[i].Resource = resource;
			}
		}
		protected internal virtual void CreateInnerAnchors(AnchorCollection masterAnchors, ResourcePrintObjectsCollection resourcePrintObjects) {
			XtraSchedulerDebug.Assert(masterAnchors.Count == resourcePrintObjects.Count);
			int count = masterAnchors.Count;
			for (int i = 0; i < count; i++) {
				ResourcePrintObjects resourcePrintObject = resourcePrintObjects[i];
				CreateInnerAnchors(masterAnchors[i], resourcePrintObject.PrintObjects);
			}
		}
		protected internal virtual void CreateInnerAnchors(AnchorBase masterAnchor, PrintObjectCollection printObjects) {
			CreateInnerAnchorsCore(masterAnchor, printObjects.Count);
			InitializeAnchors(masterAnchor.InnerAnchors, printObjects);
		}
		protected internal virtual void CreateInnerAnchorsCore(AnchorBase anchor, int count) {
			anchor.InnerAnchors.Clear();
			for (int i = 0; i < count; i++) 
				anchor.InnerAnchors.Add(CreateInnerAnchor(anchor));			
			anchor.CalculateInnerAnchorsBounds();
		}
		internal virtual AnchorBase CreateInnerAnchor(AnchorBase parentAnchor) {
			AnchorBase result = parentAnchor.CreateInstance();
			result.Resource = parentAnchor.Resource;
			result.Interval = parentAnchor.Interval;
			return result;
		}
		protected internal virtual void InitializeAnchors(AnchorCollection anchorCollection, PrintObjectCollection printObjects) {
			XtraSchedulerDebug.Assert(anchorCollection.Count == printObjects.Count);
			int count = anchorCollection.Count;
			for (int i = 0; i < count; i++)
				InitializeAnchor(anchorCollection[i], printObjects[i]);
		}
		protected internal abstract AnchorCollection CreateRootAnchors(AnchorCreationInfo creationInfo);
	}
	#endregion
	#region FitRelatedAnchorBuilder
	public class FitRelatedAnchorBuilder : FitAnchorBuilderBase {
		protected internal override AnchorCollection CreateRootAnchors(AnchorCreationInfo creationInfo) {
			AnchorCollection result = new AnchorCollection();
			foreach (AnchorBase anchor in creationInfo.MasterAnchors)
				result.AddRange(CloneAnchors(anchor.InnerAnchors));
			return result;
		}
	}
	#endregion
	#region SnapRelatedAnchorBuilder
	public class SnapRelatedAnchorBuilder : AnchorBuilderBase {
		protected internal override AnchorCollection CreateAnchorsCore(AnchorCreationInfo creationInfo) {
			return CloneAnchors(creationInfo.MasterAnchors);
		}
	}
	#endregion
	#region NonRelatedAnchorBuilderBase
	public abstract class NonRelatedAnchorBuilderBase : FitAnchorBuilderBase {
		protected internal override AnchorCollection CreateRootAnchors(AnchorCreationInfo creationInfo) {
			AnchorBase rootAnchor = CreateAnchorInstance(creationInfo.PrintDirection);
			rootAnchor.Bounds = CalculateRootAnchorBounds(creationInfo);
			AnchorCollection result = new AnchorCollection();
			result.Add(rootAnchor);
			return result;
		}
		protected internal virtual AnchorBase CreateAnchorInstance(AnchorDirection direction) {
			if (direction == AnchorDirection.Horizontal)
				return new HorizontalAnchor();
			else
				return new VerticalAnchor();
		}
		protected internal abstract Rectangle CalculateRootAnchorBounds(AnchorCreationInfo creationInfo);
	}
	#endregion
	#region FitNonRelatedAnchorBuilder
	public class FitNonRelatedAnchorBuilder : NonRelatedAnchorBuilderBase {
		protected internal override Rectangle CalculateRootAnchorBounds(AnchorCreationInfo creationInfo) {
			return creationInfo.ControlBounds;
		}
	}
	#endregion
	#region TileNonRelatedAnchorBuilder
	public class TileNonRelatedAnchorBuilder : NonRelatedAnchorBuilderBase {
		protected internal override Rectangle CalculateRootAnchorBounds(AnchorCreationInfo creationInfo) {
			int printObjectsCount = CalculateTotalPrintObjectsCount(creationInfo.ResourcePrintObjects);
			Size size = CalculateRootAnchorSize(printObjectsCount, creationInfo.ControlBounds, creationInfo.PrintDirection);
			return new Rectangle(creationInfo.ControlBounds.Location, size);
		}
		protected internal virtual int CalculateTotalPrintObjectsCount(ResourcePrintObjectsCollection resourcePrintObjects) {
			int result = 0;
			int count = resourcePrintObjects.Count;
			for (int i = 0; i < count; i++)
				result += resourcePrintObjects[i].PrintObjects.Count;
			return result;
		}
		protected internal virtual Size CalculateRootAnchorSize(int printObjectsCount, Rectangle controlBounds, AnchorDirection direction) {
			Size controlSize = controlBounds.Size;
			if (direction == AnchorDirection.Horizontal)
				return new Size(controlSize.Width * printObjectsCount, controlSize.Height);
			else
				return new Size(controlSize.Width, controlSize.Height * printObjectsCount);
		}
	}
	#endregion
	public enum AnchorBuilderType { FitRelated, SnapRelated, FitNonRelated, TileNonRelated }
	#region AnchorBuilderManager
	public class AnchorBuilderManager {
		Dictionary<AnchorBuilderType, AnchorBuilderBase> builders;
		public AnchorBuilderManager() {
			builders = new Dictionary<AnchorBuilderType, AnchorBuilderBase>();
			RegisterAnchorBuilders();
		}
		protected internal Dictionary<AnchorBuilderType, AnchorBuilderBase> Builders { get { return builders; } }
		protected internal virtual void RegisterAnchorBuilders() {
			Builders[AnchorBuilderType.FitRelated] = new FitRelatedAnchorBuilder();
			Builders[AnchorBuilderType.SnapRelated] = new SnapRelatedAnchorBuilder();
			Builders[AnchorBuilderType.FitNonRelated] = new FitNonRelatedAnchorBuilder();
			Builders[AnchorBuilderType.TileNonRelated] = new TileNonRelatedAnchorBuilder();
		}
		public AnchorBuilderBase GetHorizontalAnchorBuilder(ReportViewControlBase viewControl) {
			AnchorBuilderType builderType = CalculateHorizontalBuilderType(viewControl);
			return Builders[builderType];
		}
		public AnchorBuilderBase GetVerticalAnchorBuilder(ReportViewControlBase viewControl) {
			AnchorBuilderType builderType = CalculateVerticalBuilderType(viewControl);
			return Builders[builderType];
		}
		protected internal virtual AnchorBuilderType CalculateHorizontalBuilderType(ReportViewControlBase viewControl) {
			if (IsRelatedHorizontally(viewControl))
				return CalculateRelatedBuilderType(viewControl.LayoutOptionsHorizontal.AnchorType);
			else
				return CalculateNonRelatedBuilderType(viewControl.LayoutOptionsHorizontal.LayoutType);
		}
		protected internal virtual AnchorBuilderType CalculateVerticalBuilderType(ReportViewControlBase viewControl) {
			if (IsRelatedVertically(viewControl))
				return CalculateRelatedBuilderType(viewControl.LayoutOptionsVertical.AnchorType);
			else
				return CalculateNonRelatedBuilderType(viewControl.LayoutOptionsVertical.LayoutType);
		}
		protected internal virtual bool IsRelatedHorizontally(ReportViewControlBase viewControl) {
			return viewControl.LayoutOptionsHorizontal.MasterControl != null;
		}
		protected internal virtual bool IsRelatedVertically(ReportViewControlBase viewControl) {
			return viewControl.LayoutOptionsVertical.MasterControl != null;
		}
		protected internal virtual AnchorBuilderType CalculateRelatedBuilderType(ControlContentAnchorType layoutType) {
			return layoutType == ControlContentAnchorType.Fit ? AnchorBuilderType.FitRelated : AnchorBuilderType.SnapRelated;
		}
		protected internal virtual AnchorBuilderType CalculateNonRelatedBuilderType(ControlContentLayoutType layoutType) {
			return layoutType == ControlContentLayoutType.Fit ? AnchorBuilderType.FitNonRelated : AnchorBuilderType.TileNonRelated;
		}
	}
	#endregion
}
