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

using DevExpress.Data.Camera.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.Data.Camera {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DeviceVideoSettings : INotifyPropertyChanged {
		ICameraDeviceClient _deviceClient;
		List<DeviceVideoProperty> propsList = new List<DeviceVideoProperty>();
		public DeviceVideoSettings(ICameraDeviceClient client) {
			_deviceClient = client;
			InitProps();
		}
		void InitProps() {
			propsList.Add(Brightness = new DeviceVideoProperty(this, "Brightness"));
			propsList.Add(Contrast = new DeviceVideoProperty(this, "Contrast"));
			propsList.Add(Hue = new DeviceVideoProperty(this, "Hue"));
			propsList.Add(Saturation = new DeviceVideoProperty(this, "Saturation"));
			propsList.Add(Sharpness = new DeviceVideoProperty(this, "Sharpness"));
			propsList.Add(Gamma = new DeviceVideoProperty(this, "Gamma"));
			propsList.Add(ColorEnable = new DeviceVideoProperty(this, "ColorEnable"));
			propsList.Add(WhiteBalance = new DeviceVideoProperty(this, "WhiteBalance"));
			propsList.Add(BacklightCompensation = new DeviceVideoProperty(this, "BacklightCompensation"));
			propsList.Add(Gain = new DeviceVideoProperty(this, "Gain"));
		}
		public DeviceVideoProperty Brightness { get; private set; }
		public DeviceVideoProperty Contrast { get; private set; }
		public DeviceVideoProperty Hue { get; private set; }
		public DeviceVideoProperty Saturation { get; private set; }
		public DeviceVideoProperty Sharpness { get; private set; }
		public DeviceVideoProperty Gamma { get; private set; }
		public DeviceVideoProperty ColorEnable { get; private set; }
		public DeviceVideoProperty WhiteBalance { get; private set; }
		public DeviceVideoProperty BacklightCompensation { get; private set; }
		public DeviceVideoProperty Gain { get; private set; }
		public void ResetToDefaults() {
			foreach(DeviceVideoProperty property in propsList) {
				property.ResetToDefault();
			}
		}
		CameraDeviceBase Device {
			get {
				if(_deviceClient != null) {
					return _deviceClient.Device;
				}
				return null;
			}
		}
		bool DeviceIsAvailable {
			get { return Device != null; }
		}
		internal void SetProp(string propName, int val) {
			if(DeviceIsAvailable)
				Device.SetVideoProcessingPropertyByName(propName, val);
		}
		internal int GetPropMin(string name) {
			if(DeviceIsAvailable)
				return Device.GetVideoProcessingPropertyByName(name, false, "Min");
			return 0;
		}
		internal int GetPropMax(string name) {
			if(DeviceIsAvailable)
				return Device.GetVideoProcessingPropertyByName(name, false, "Max");
			return 0;
		}
		internal int GetPropDefault(string name) {
			if(DeviceIsAvailable)
				return Device.GetVideoProcessingPropertyByName(name, false, "Default");
			return 0;
		}
		internal int GetPropSteppingDelta(string name) {
			if(DeviceIsAvailable)
				return Device.GetVideoProcessingPropertyByName(name, false, "SteppingDelta");
			return 0;
		}
		internal int GetPropValue(string name) {
			if(DeviceIsAvailable)
				return Device.GetVideoProcessingPropertyByName(name, true, "Value");
			return 0;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string property) {
			PropertyChangedEventHandler pc = PropertyChanged;
			if (pc != null)
				pc(this, new PropertyChangedEventArgs(property));
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DeviceVideoProperty : INotifyPropertyChanged {
		internal DeviceVideoProperty(DeviceVideoSettings settings, string name) {
			this.settings = settings;
			this.Name = name;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		readonly DeviceVideoSettings settings;
		[Browsable(false)]
		public string Name { get; private set; }
		public int Min { get { return settings.GetPropMin(Name); } }
		public int Max { get { return settings.GetPropMax(Name); } }
		public int SteppingDelta { get { return settings.GetPropSteppingDelta(Name); } }
		public int Default { get { return settings.GetPropDefault(Name); } }
		public int Value {
			get { return settings.GetPropValue(Name); }
			set {
				settings.SetProp(Name, value);
				RaisePropertyChanged("Value");
			}
		}
		void RaisePropertyChanged(string property) {
			PropertyChangedEventHandler pc = PropertyChanged;
			if (pc != null)
				pc(this, new PropertyChangedEventArgs(property));
		}
		public void ResetToDefault() {
			this.Value = this.Default;
		}
	}
}
