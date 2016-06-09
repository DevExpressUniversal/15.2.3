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
using System.Drawing;
using System.Collections;
namespace DevExpress.Utils.Animation {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IRingWaitingIndicatorProperties : IDotWaitingIndicatorProperties {
		int RingAnimationDiameter { get; set; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILineWaitingIndicatorProperties : IDotWaitingIndicatorProperties {
		int LineAnimationElementHeight { get; set; }
		LineAnimationElementType LineAnimationElementType { get; set; }
	}
	public interface IDotWaitingIndicatorProperties : IWaitingIndicatorProperties {
		int FrameCount { get; set; }
		int FrameInterval { get; set; }
		AppearanceObject Appearance { get; }
		float AnimationAcceleration { get; set; }
		float AnimationSpeed { get; set; }
		Image AnimationElementImage { get; set; }
		int AnimationElementCount { get; set; }
		ContentAlignment ContentAlignment { get; set; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IWaitingIndicatorProperties : IDisposable, DevExpress.Utils.Base.IBaseProperties {
		AppearanceObject AppearanceCaption { get; }
		AppearanceObject AppearanceDescription { get; }
		string Caption { get; set; }
		string Description { get; set; }
		bool ShowDescription { get; set; }
		bool ShowCaption { get; set; }
		int CaptionToDescriptionDistance { get; set; }
		int ImageToTextDistance { get; set; }
		Size ContentMinSize { get; set; }
		bool AllowBackground { get; set; }
		int ImageOffset { get; set; }
	}
	public class DotWaitingIndicatorProperties : WaitingIndicatorProperties, IDotWaitingIndicatorProperties {
		AppearanceObject appearanceCore;
		DotWaitingIndicatorProperties parentPropertiesCore;
		protected override void EnsureParentPropertiesCore(WaitingIndicatorProperties parentProperties) {
			base.EnsureParentPropertiesCore(parentProperties);
			if(parentProperties is DotWaitingIndicatorProperties)
				parentPropertiesCore = parentProperties as DotWaitingIndicatorProperties;
		}
		protected override void FillPropertyBagWithDefaultValues(ref Hashtable properties) {
			base.FillPropertyBagWithDefaultValues(ref properties);
			properties.Add("FrameCount", 38000);
			properties.Add("FrameInterval", 1000);
			properties.Add("AnimationAcceleration", 7.0);
			properties.Add("AnimationSpeed", 5.5);
			properties.Add("AnimationElementImage", null);
			properties.Add("AnimationElementCount", 5);
			properties.Add("ContentAlignment", ContentAlignment.MiddleLeft);
		}
		[Category("Behavior"), DefaultValue(ContentAlignment.MiddleLeft)]
		public ContentAlignment ContentAlignment {
			get { return (ContentAlignment)GetValue("ContentAlignment"); }
			set {
				SetValue("ContentAlignment", value);
			}
		}
		[Category("Appearance"), DefaultValue(38000)]
		public int AnimationElementCount {
			get { return (int)GetValue("AnimationElementCount"); }
			set {
				if(value <= 0) throw new ArgumentException();
				SetValue("AnimationElementCount", value);
			}
		}
		[Category("Appearance"), DefaultValue(38000)]
		public int FrameCount {
			get { return (int)GetValue("FrameCount"); }
			set {
				if(value <= 0) throw new ArgumentException();
				SetValue("FrameCount", value);
			}
		}
		[Category("Appearance"), DefaultValue(1000)]
		public int FrameInterval {
			get { return (int)GetValue("FrameInterval"); }
			set {
				if(value <= 0) throw new ArgumentException();
				SetValue("FrameInterval", value);
			}
		}
		[Category("Appearance"), DefaultValue((float)7.0)]
		public float AnimationAcceleration {
			get { return (float)Convert.ToDouble(GetValue("AnimationAcceleration")); }
			set {
				if(value <= 0) throw new ArgumentException();
				SetValue("AnimationAcceleration", value);
			}
		}
		[Category("Appearance"), DefaultValue((float)5.5)]
		public float AnimationSpeed {
			get { return (float)Convert.ToDouble(GetValue("AnimationSpeed")); }
			set {
				if(value <= 0) throw new ArgumentException();
				SetValue("AnimationSpeed", value);
			}
		}
		[Category("Appearance"), DefaultValue(null)]
		public Image AnimationElementImage {
			get { return (Image)GetValue("AnimationElementImage"); }
			set { SetValue("AnimationElementImage", value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance {
			get {
				if(parentPropertiesCore != null) { return parentPropertiesCore.Appearance; }
				if(appearanceCore == null) {
					appearanceCore = CreateAppearance();
					appearanceCore.Changed += OnAppearanceChanged;
				}
				return appearanceCore;
			}
		}
		public override void Dispose() {
			base.Dispose();
			appearanceCore.Changed -= OnAppearanceChanged;
			Ref.Dispose(ref appearanceCore);
		}
	}
	public class LineWaitingIndicatorProperties : DotWaitingIndicatorProperties, ILineWaitingIndicatorProperties {
		public LineWaitingIndicatorProperties() : base() { }
		int dotHeightCore = 10;
		LineAnimationElementType elementTypeCore = LineAnimationElementType.Circle;
		public void EnsureParentProperties(DotWaitingIndicatorProperties parentProperties) { EnsureParentPropertiesCore(parentProperties); }
		[Category("Appearance"), DefaultValue(10)]
		public int LineAnimationElementHeight {
			get { return dotHeightCore; }
			set {
				if(value < 0) throw new ArgumentException();
				if(dotHeightCore == value) return;
				dotHeightCore = value;
				RaiseChanged(new EventArgs());
			}
		}
		[Category("Appearance"), DefaultValue(LineAnimationElementType.Circle)]
		public LineAnimationElementType LineAnimationElementType {
			get { return elementTypeCore; }
			set {
				if(elementTypeCore == value) return;
				elementTypeCore = value;
				RaiseChanged(new EventArgs());
			}
		}
	}
	public class RingWaitingIndicatorProperties : DotWaitingIndicatorProperties, IRingWaitingIndicatorProperties {
		public RingWaitingIndicatorProperties() : base() { }
		int ringDiameterCore = 40;
		public void EnsureParentProperties(DotWaitingIndicatorProperties parentProperties) { EnsureParentPropertiesCore(parentProperties); }
		[Category("Appearance"), DefaultValue(40)]
		public int RingAnimationDiameter {
			get { return ringDiameterCore; }
			set {
				if(value < 0) throw new ArgumentException();
				if(ringDiameterCore == value) return;
				ringDiameterCore = value;
				RaiseChanged(new EventArgs());
			}
		}
	}
	public class WaitingIndicatorProperties : IWaitingIndicatorProperties {
		const int defaultImageToTextDistance = 8;
		const int defaultCaptionToDescriptionDistance = 0;
		const int defaultImageOffset = 0;
		AppearanceObject appearanceCaptionCore;
		AppearanceObject appearanceDescriptionCore;
		event EventHandler waitingIndicatorPropertiesChangedCore;
		WaitingIndicatorProperties parentPropertiesCore;
		Hashtable propertyBag;
		Hashtable defaultPropertyBag;
		public WaitingIndicatorProperties() {
			propertyBag = new Hashtable();
			defaultPropertyBag = new Hashtable();
			FillPropertyBagWithDefaultValues(ref propertyBag);
			FillPropertyBagWithDefaultValues(ref defaultPropertyBag);
			lockUpdate = 0;
		}
		protected virtual void FillPropertyBagWithDefaultValues(ref Hashtable properties) {
			properties.Add("ShowCaption", true);
			properties.Add("ShowDescription", true);
			properties.Add("Description", string.Empty);
			properties.Add("Caption", string.Empty);
			properties.Add("ImageToTextDistance", defaultImageToTextDistance);
			properties.Add("CaptionToDescriptionDistance", defaultCaptionToDescriptionDistance);
			properties.Add("ImageOffset", defaultImageOffset);
			properties.Add("AllowBackground", true);
		}
		protected Hashtable DefaultPropertyBag { get { return defaultPropertyBag; } }
		protected Hashtable PropertyBag { get { return propertyBag; } }
		protected void SetValue(string propertyName, object value) {
			PropertyBag[propertyName] = value;
			RaiseChanged(new EventArgs());
		}
		protected object GetValue(string propertyName) { return ParentProperties == null ? PropertyBag[propertyName] : ParentProperties.PropertyBag[propertyName]; }
		protected WaitingIndicatorProperties ParentProperties {
			get { return parentPropertiesCore; }
		}
		protected virtual void EnsureParentPropertiesCore(WaitingIndicatorProperties parentProperties) {
			parentPropertiesCore = parentProperties;
			parentPropertiesCore.Changed += OnParentWaitingIndicatorPropertiesChanged;
		}
		void OnParentWaitingIndicatorPropertiesChanged(object sender, EventArgs e) {
			RaiseChanged(e);
		}
		protected virtual void RaiseChanged(EventArgs e) {
			if(waitingIndicatorPropertiesChangedCore == null || IsUpdateLocked) return;
			waitingIndicatorPropertiesChangedCore(this, e);
		}
		public event EventHandler Changed {
			add { waitingIndicatorPropertiesChangedCore += value; }
			remove { waitingIndicatorPropertiesChangedCore -= value; }
		}
		protected AppearanceObject CreateAppearance() {
			return new AppearanceObject();
		}
		#region ILoadingIndicatorProperties Members
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceCaption {
			get {
				if(parentPropertiesCore != null) { return parentPropertiesCore.AppearanceCaption; }
				if(appearanceCaptionCore == null) {
					appearanceCaptionCore = CreateAppearance();
					appearanceCaptionCore.Changed += OnAppearanceChanged;
				}
				return appearanceCaptionCore;
			}
		}
		protected void OnAppearanceChanged(object sender, EventArgs e) {
			RaiseChanged(new EventArgs());
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceDescription {
			get {
				if(parentPropertiesCore != null) { return parentPropertiesCore.AppearanceDescription; }
				if(appearanceDescriptionCore == null) {
					appearanceDescriptionCore = CreateAppearance();
					appearanceDescriptionCore.Changed += OnAppearanceChanged;
				}
				return appearanceDescriptionCore;
			}
		}
		[DefaultValue(null), Category("Appearance"), Localizable(true)]
		public string Caption {
			get { return (string)GetValue("Caption"); }
			set { SetValue("Caption", value); }
		}
		[DefaultValue(null), Category("Appearance"), Localizable(true)]
		public string Description {
			get { return (string)GetValue("Description"); }
			set { SetValue("Description", value); } 
		}
		[DefaultValue(true), Category("Appearance")]
		public bool ShowDescription {
			get { return (bool)GetValue("ShowDescription"); }
			set { SetValue("ShowDescription", value); } 
		}
		[DefaultValue(true), Category("Appearance")]
		public bool ShowCaption {
			get { return (bool)GetValue("ShowCaption"); }
			set { SetValue("ShowCaption", value); } 
		}
		[DefaultValue(true), Category("Appearance")]
		public bool AllowBackground {
			get { return (bool)GetValue("AllowBackground"); }
			set { SetValue("AllowBackground", value); }
		}
		[Category("Layout")]
		public Size ContentMinSize { get; set; }
		[Category("Layout"), DefaultValue(0)]
		public int CaptionToDescriptionDistance {
			get { return (int)GetValue("CaptionToDescriptionDistance"); }
			set {
				if(value < 0) throw new ArgumentException("CaptionToDescriptionDistance");
				SetValue("CaptionToDescriptionDistance", value); 
			}
		}		
		[Category("Layout"), DefaultValue(8)]
		public int ImageToTextDistance {
			get { return (int)GetValue("ImageToTextDistance"); }
			set {
				if(value < 0) throw new ArgumentException("ImageToTextDistance");
				SetValue("ImageToTextDistance", value); 
			}
		}
		[Category("Layout"), DefaultValue(0)]
		public int ImageOffset {
			get { return (int)GetValue("ImageOffset"); }
			set {
				if(value < 0) throw new ArgumentException("ImageOffset");
				SetValue("ImageOffset", value);
			}
		}
		bool ShouldSerializeContentMinSize() { return !ContentMinSize.IsEmpty; }
		void ResetContentMinSize() { ContentMinSize = Size.Empty; }
		public virtual void Update() {
			RaiseChanged(new EventArgs());
		}
		bool isDisposing;
		public virtual void Dispose() {
			if(isDisposing) return;
			isDisposing = true;
			if(appearanceDescriptionCore != null)
				appearanceDescriptionCore.Changed -= OnAppearanceChanged;
			if(appearanceCaptionCore != null)
				appearanceCaptionCore.Changed -= OnAppearanceChanged;
			Ref.Dispose(ref appearanceDescriptionCore);
			Ref.Dispose(ref appearanceCaptionCore);
			RaiseDisposed(new EventArgs());
		}		
		#endregion
		#region IBaseProperties Members
		void Base.IBaseProperties.Assign(Base.IPropertiesProvider source) {
			WaitingIndicatorProperties newProperties = source as WaitingIndicatorProperties;
			this.propertyBag.Clear();
			foreach(var key in newProperties.PropertyBag.Keys) {
				this.propertyBag.Add(key, newProperties.PropertyBag[key]);
			}
		}
		Base.IBaseProperties Base.IBaseProperties.Clone() {
			WaitingIndicatorProperties newProperties = new WaitingIndicatorProperties();
			(newProperties as Base.IBaseProperties).Assign(this);
			return newProperties;
		}
		bool Base.IBaseProperties.ShouldSerialize() {
			foreach(var key in propertyBag.Keys) {
				if(!(this as Base.IBaseProperties).IsDefault((string)key))
					return false;
			}
			return true; 
		}
		void Base.IBaseProperties.Reset() {
			FillPropertyBagWithDefaultValues(ref propertyBag);
		}
		T Base.IBaseProperties.GetValue<T>(string property) {
			return (T)propertyBag[property];
		}
		bool Base.IBaseProperties.IsDefault(string property) {
			return propertyBag[property].Equals(defaultPropertyBag[property]);
		}
		T Base.IBaseProperties.GetDefaultValue<T>(string property) {
			return (T)defaultPropertyBag[property];
		}
		bool Base.IBaseProperties.IsContentProperty(string property) { return false; }
		T Base.IBaseProperties.GetContent<T>(string property) {
			return (T)propertyBag[property];
		}
		#endregion
		#region IBaseObject Members
		bool Base.IBaseObject.IsDisposing {
			get { return isDisposing; }
		}
		public event EventHandler Disposed;
		protected void RaiseDisposed(EventArgs e) {
			if(Disposed != null) Disposed(this, e);
		}
		#endregion
		#region IPropertiesProvider Members
		IDictionaryEnumerator Base.IPropertiesProvider.GetProperties() {
			return propertyBag.GetEnumerator();
		}
		#endregion
		#region ISupportBatchUpdate Members
		public bool IsUpdateLocked {
			get { return lockUpdate != 0; }
		}
		int lockUpdate;
		public void BeginUpdate() { lockUpdate++; }
		public void EndUpdate() {
			if(lockUpdate == 0) return;
			if(--lockUpdate == 0)
				Update();
		}
		public void CancelUpdate() {
			if(lockUpdate == 0) return;
			lockUpdate = 0;
		}
		#endregion
	}
}
