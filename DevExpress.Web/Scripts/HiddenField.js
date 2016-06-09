/// <reference path="_references.js"/>

(function () {
var ASPxClientHiddenField = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        // Initialized by the server
        this.syncWithServer = true;
        this.properties = {};
        this.typeInfoTable = {};
        this.typeNameTable = [];
    },

    IsDOMDisposed: function() { 
        return this.syncWithServer ? !ASPx.IsExistsElement(this.GetStateHiddenField()) : false;
    },
    // Synchronization
    UpdateStateObject: function () {
        if(!this.syncWithServer) 
            return;

        var serializedData = ASPx.GetHiddenFieldSerializer().Serialize(this);
        this.UpdateStateObjectWithObject({ data: serializedData });
    },
    GetStateHiddenField: function() {
        return ASPx.GetElementById(this.uniqueID);
    },
    EscapeSpecialCharacters: function (str) {
        str = str.replace(/\\/g, "\\\\");
        var specialChars = {};
        for(var i = 0; i < str.length; i++) {
            var char = str.charAt(i);
            var charCode = char.charCodeAt(0);
            if(charCode < 32) {
                var hexCharCode = charCode.toString(16);
                specialChars[char] = "\\u" + "0000".substr(0, 4 - hexCharCode.length) + hexCharCode;
            }
        }
        for(var ch in specialChars)
            str = str.replace(new RegExp(ch, "g"), specialChars[ch]);
        return str;
    },

    // Callback
    PerformCallback: function (parameter) {
        this.CreateCallback(parameter);
    },
    OnCallback: function (result) {
        var callbackMarkupContainer = this.GetCallbackMarkupContainer();
        ASPx.SetInnerHtml(callbackMarkupContainer, result);
    },
    GetCallbackMarkupContainer: function () {
        var callbackMarkupContainer = ASPx.GetElementById(this.GetCallbackMarkupContainerID());
        if(!callbackMarkupContainer) {
            callbackMarkupContainer = this.CreateCallbackMarkupContainer();
            document.body.appendChild(callbackMarkupContainer);
        }
        return callbackMarkupContainer;
    },
    GetCallbackMarkupContainerID: function () {
        return this.name + ASPxClientHiddenField.CallbackMarkupContainerIDSuffix;
    },
    CreateCallbackMarkupContainer: function () {
        var callbackMarkupContainer = document.createElement("DIV");
        ASPx.SetElementDisplay(callbackMarkupContainer, false);
        callbackMarkupContainer.id = this.GetCallbackMarkupContainerID();
        return callbackMarkupContainer;
    },
    // API
    Add: function (propertyName, propertyValue) {
        var existentPropertyValue = this.Get(propertyName);
        if(typeof (existentPropertyValue) == "undefined")
            this.Set(propertyName, propertyValue);
        else
            alert("A property with the name '" + propertyName + "' has already been added.");

    },
    Get: function (propertyName) {
        var safeName = this.GetTopLevelPropertySafeName(propertyName);
        return this.properties[safeName];
    },
    Set: function (propertyName, propertyValue) {
        var safeName = this.GetTopLevelPropertySafeName(propertyName);
        if(typeof (propertyValue) == "undefined")
            this.Remove(propertyName);
        else
            this.properties[safeName] = propertyValue;
    },
    Remove: function (propertyName) {
        var safeName = this.GetTopLevelPropertySafeName(propertyName);
        delete this.properties[safeName];
        TypeInfoHelper.RemoveTypeInfoBranch(this.typeInfoTable, safeName);
    },
    Clear: function () {
        this.properties = {};
        this.typeInfoTable = {};
        this.typeNameTable = [];
    },
    Contains: function (propertyName) {
        var safeTopLevelPropertyName = this.GetTopLevelPropertySafeName(propertyName);
        for(var key in this.properties) {
            if(key == safeTopLevelPropertyName)
                return true;
        }
        return false;
    },

    // Utils
    GetTopLevelPropertySafeName: function (propertyName) {
        return ASPxClientHiddenField.TopLevelKeyPrefix + propertyName;
    }
});
ASPxClientHiddenField.Cast = ASPxClientControl.Cast;

ASPxClientHiddenField.InputElementIDSuffix = "_I";
ASPxClientHiddenField.CallbackMarkupContainerIDSuffix = "_D";
ASPxClientHiddenField.TopLevelKeyPrefix = "dxp";

var TypeInfoHelper = ASPx.CreateClass(null, {
    constructor: function () {
        this.minUnknownTypeIndex = 1024;
        this.clientTypeConstructors = [
            null,
            Number,
            String,
            Date,
            Boolean,
            RegExp,
            Array,
            Object,
            Function
        ];
        this.clientTypeConstructorIndices = {};
        for(var i = 1; i < this.clientTypeConstructors.length; i++)
            this.clientTypeConstructorIndices[this.clientTypeConstructors[i]] = i;
    },

    // Public
    EnsureTypeInfoTableCompliant: function (value, typeInfoTable, typeInfoKey) {
        if(typeInfoKey == "")
            return;
        var typeCode = typeInfoTable[typeInfoKey];
        if(typeof (typeCode) == "undefined")
            typeCode = this.GetArrayItemTypeCode(typeInfoTable, typeInfoKey);
        if(typeof (typeCode) != "undefined") {
            if(!this.IsValueTypeInfoCompliant(value, typeCode))
                TypeInfoHelper.RemoveTypeInfoBranch(typeInfoTable, typeInfoKey);
            else
                return;
        }
        typeCode = this.CreateTypeCode(value);
        if(typeof (typeCode) != "undefined")
            typeInfoTable[typeInfoKey] = typeCode;
        else
            delete typeInfoTable[typeInfoKey];
    },
    GetArrayItemTypeCode: function (typeInfoTable, typeInfoKey) {
        var separator = "|";
        var index = typeInfoKey.lastIndexOf(separator);
        if(index < 0)
            return;
        itemTypeKey = typeInfoKey.substr(0, index) + "_itemType";
        var typeCode = typeInfoTable[itemTypeKey];
        if(typeof (typeCode) != "undefined")
            typeInfoTable[typeInfoKey] = typeCode;
        return typeCode;
    },
    IsAtomValue: function (value, typeCode) {
        return typeCode == 0 || !(this.IsListValue(value, typeCode) || this.IsDictionaryValue(value, typeCode));
    },
    IsListValue: function (value, typeCode) {
        return this.IsKnownTypeCode(typeCode) ? this.GetConstructor(typeCode) === Array : value.constructor === Array;
    },
    IsDictionaryValue: function (value, typeCode) {
        return this.IsKnownTypeCode(typeCode) ? this.GetConstructor(typeCode) === Object : value.constructor === Object;
    },
    GetArrayTypeCode: function () {
        return this.clientTypeConstructorIndices[Array] << 1;
    },
    GetStringTypeCode: function () {
        return this.clientTypeConstructorIndices[String] << 1;
    },
    IsStringTypeCode: function (typeCode) {
        return typeCode == this.GetStringTypeCode();
    },

    // Private
    IsValueTypeInfoCompliant: function (value, typeCode) {
        if(this.IsKnownTypeCode(typeCode))
            return value != null ? value.constructor === this.GetConstructor(typeCode) : this.IsNullable(typeCode);
        else
            return value == null || value.constructor === Array || value.constructor === Object;
    },
    CreateTypeCode: function (value) {
        if(value == null)
            return 1; /* Nullable with unknown ctor */
        var clientTypeIndex = this.clientTypeConstructorIndices[value.constructor];
        var lowerBit = Number(
            clientTypeIndex == this.clientTypeConstructorIndices[RegExp] ||
            clientTypeIndex == this.clientTypeConstructorIndices[Array] ||
            clientTypeIndex == this.clientTypeConstructorIndices[Object]
        );
        return typeof (clientTypeIndex) != "undefined" ? ((clientTypeIndex << 1) + lowerBit) : void (0);
    },
    IsNullable: function (typeCode) {
        return (typeCode & 1) > 0;
    },
    GetConstructor: function (typeCode) {
        return this.clientTypeConstructors[(typeCode >>> 1) & 7];
    },
    IsKnownTypeCode: function (typeCode) {
        return typeCode < this.minUnknownTypeIndex;
    }
});
TypeInfoHelper.RemoveTypeInfoBranch = function (typeInfoTable, typeInfoKeyPrefix) {
    for(var key in typeInfoTable) {
        if(key.indexOf(typeInfoKeyPrefix) == 0)
            delete typeInfoTable[key];
    }
};

var HiddenFieldSerializer = ASPx.CreateClass(null, {
    constructor: function () {
        this.typeInfoHelper = new TypeInfoHelper();
        this.separator = "|";
        this.sentinel = "#";
        this.charCodes = this.CreateCharCodeList(["a", "z", "0", "9", "_", "$"]);
    },
    Serialize: function (hiddenField) {
        var sb = [];
        this.SerializeCore(hiddenField.typeNameTable, "", sb, null, null, null, false);
        this.SerializeCore(hiddenField.properties, "", sb, hiddenField.typeInfoTable, hiddenField.typeNameTable, ASPxClientHiddenField.TopLevelKeyPrefix, true);
        return sb.join("");
    },
    SerializeCore: function (value, pathInPropertiesTree, serializedData, typeInfoTable, typeNameTable, keyNamePrefix, validateKeys) {
        var metaTablesDefined = typeInfoTable != null && typeNameTable != null;
        var typeCode = null;
        if(metaTablesDefined) {
            // Properties are being serialized
            this.typeInfoHelper.EnsureTypeInfoTableCompliant(value, typeInfoTable, pathInPropertiesTree);
            typeCode = typeInfoTable[pathInPropertiesTree];
        } else {
            // Metatable itself is being serialized, so it's either a metatable (array) or its item (string)
            typeCode = value.constructor === Array ?
                this.typeInfoHelper.GetArrayTypeCode() : this.typeInfoHelper.GetStringTypeCode();
        }
        if(typeof (typeCode) != "undefined")
            serializedData.push(typeCode);
        serializedData.push(this.separator);
        if(typeof (typeCode) == "undefined" || this.typeInfoHelper.IsDictionaryValue(value, typeCode)) {
            for(var key in value) {
                var serializableKey = key;
                if(keyNamePrefix.length > 0)
                    serializableKey = serializableKey.slice(keyNamePrefix.length);
                if(validateKeys)
                    this.AssertKeyIsValid(serializableKey);
                serializedData.push(serializableKey);
                serializedData.push(this.separator);
                this.SerializeCore(value[key], pathInPropertiesTree.length > 0 ? (pathInPropertiesTree + this.separator + key) : key,
                    serializedData, typeInfoTable, typeNameTable, "", validateKeys);
            }
            serializedData.push(this.sentinel);
        } else if(this.typeInfoHelper.IsListValue(value, typeCode)) {
            for(var i = 0; i < value.length; i++)
                this.SerializeCore(value[i], pathInPropertiesTree.length > 0 ? (pathInPropertiesTree + this.separator + i) : i,
                    serializedData, typeInfoTable, typeNameTable, "", validateKeys);
            serializedData.push(this.sentinel);
        } else if(this.typeInfoHelper.IsAtomValue(value, typeCode))
            this.SerializeAtomValue(value, serializedData, typeCode);
    },
    SerializeAtomValue: function (value, sb, typeCode) {
        var valueStr = this.SerializeAtomValueCore(value, typeCode);
        sb.push(valueStr.length.toString());
        sb.push(this.separator);
        sb.push(valueStr);
    },
    SerializeAtomValueCore: function (value, typeCode) {
        var isString = this.typeInfoHelper.IsStringTypeCode(typeCode);
        if(value == null)
            return isString ? "0" : "";
        else {
            if(isString) {
                return "1" + value.replace(/\r/g, ""); // Q239689 & B200946
            } else {
                var ctor = value.constructor;
                if(ctor === String /* (Char) */)
                    return value;
                else if(ctor === Boolean)
                    return value ? "1" : "0";
                else if(ctor === Number)
                    return value.toString();
                else if(ctor === Date)
                    return String(ASPx.DateUtils.ToLocalTime(value).valueOf());
                else if(ctor === RegExp) {
                    var options = "";
                    if(value.ignoreCase)
                        options += "i";
                    if(value.multiline)
                        options += "m";
                    return options + "," + value.source;
                }
            }
        }
        alert("Unable to serialize value " + value.toString() + " (Constructor: " + value.constructor.toString() + ").");
    },

    // Utils
    AssertKeyIsValid: function (key) {
        if(!key)
            alert("Empty key.");
    },
    CreateCharCodeList: function (chars) {
        var charCodes = {};
        for(var i = 0; i < chars.length; i++) {
            var ch = chars[i];
            charCodes[ch] = ch.charCodeAt(0);
        }
        return charCodes;
    },
    IsLowercaseLetterCharCode: function (charCode) {
        return charCode >= this.charCodes["a"] && charCode <= this.charCodes["z"];
    }
});

var hiddenFieldSerializer;
ASPx.GetHiddenFieldSerializer = function() {
    if(!hiddenFieldSerializer)
        hiddenFieldSerializer = new HiddenFieldSerializer();
    return hiddenFieldSerializer;
}

ASPx.TypeInfoHelper = TypeInfoHelper;

window.ASPxClientHiddenField = ASPxClientHiddenField;
})();