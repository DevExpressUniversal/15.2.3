(function() {
    ASPx.HtmlEditorClasses.Controls.ToolbarComboBox = ASPx.CreateClass(ASPxClientComboBox, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.defaultCaption = "";

            this.BeforeFocus = new ASPxClientEvent();
            this.ItemClick = new ASPxClientEvent();
            this.beforeFocusLockCount = 0;
            this.commandName = "";
        },
        SetValue: function(value) {
            var isValueEmpty = !value || value == "null"; // Opera returns incorrect value - hack - B38309
            if(isValueEmpty && this.defaultCaption)
                value = this.defaultCaption;

            ASPxClientComboBox.prototype.SetValue.call(this, value);
            if(this.GetSelectedIndex() == -1) {
                ASPxClientComboBox.prototype.SetText.call(this, value);
            }
        },
        ForceRefocusEditor: function() {
            // The base method call leads to the selection loss
            this.RaiseBeforeFocus();
        },
        RaiseBeforeFocus: function() {
            if(this.beforeFocusLockCount == 0) {
                this.beforeFocusLockCount++;
                var args = new ASPxClientEventArgs();
                this.BeforeFocus.FireEvent(this, args);
            }
        },
        RaiseLostFocus: function() {
            ASPxClientComboBox.prototype.RaiseLostFocus.call(this);
            this.beforeFocusLockCount = 0;
        },
        OnListBoxItemMouseUp: function(evt) {
            ASPxClientComboBox.prototype.OnListBoxItemMouseUp.call(this, evt);
            this.RaiseItemClick();
        },
        RaiseItemClick: function() {
            var args = new ASPxClientEventArgs();
            this.ItemClick.FireEvent(this, args);
        },
        OnTextChanged: function() {
            ASPxClientComboBox.prototype.OnTextChanged.call(this);
            this.RaiseItemClick();
        },
        HideDropDownArea: function(isRaiseEvent) {
            ASPxClientComboBox.prototype.HideDropDownArea.call(this, isRaiseEvent);
            this.beforeFocusLockCount = 0;
        }
    });
    ASPx.HtmlEditorClasses.Controls.NativeToolbarComboBox = ASPx.CreateClass(ASPxClientNativeComboBox, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.defaultCaption = "";
            this.commandName = "";
        },
        SetValue: function(value) {
            if(!value && this.defaultCaption)
                value = this.defaultCaption;

            ASPxClientNativeComboBox.prototype.SetValue.call(this, value);
            if(this.GetSelectedIndex() == -1)
                ASPxClientNativeComboBox.prototype.SetText.call(this, value);
        },
        OnListBoxItemMouseUp: function() {
            ASPxClientNativeComboBox.prototype.OnListBoxItemMouseUp.call(this);
            this.RaiseItemClick();
        },
        RaiseItemClick: function() {
            var args = new ASPxClientEventArgs();
            this.ItemClick.FireEvent(this, args);
        }
    });

    ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox = ASPx.CreateClass(ASPx.HtmlEditorClasses.Controls.ToolbarComboBox, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);

            this.cssClasses = [];
            this.tagNames = [];

            this.cssClassesValueHashTable = {};
            this.tagNameCssClassesValueHashTable = {};
        },
        Initialize: function() {
            ASPxClientComboBox.prototype.Initialize.call(this);
            this.CreateTagNamesAndCssClassesHashTable();
        },
        GetValue: function() {
            return ASPx.HtmlEditorClasses.Controls.ToolbarComboBox.prototype.GetValue.call(this);
        },
        SetValue: function(value) {
            ASPx.HtmlEditorClasses.Controls.ToolbarComboBox.prototype.SetValue.call(this, value);
        }
    });
    ASPx.HtmlEditorClasses.Controls.NativeToolbarCustomCssComboBox = ASPx.CreateClass(ASPx.HtmlEditorClasses.Controls.NativeToolbarComboBox, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);

            this.cssClasses = [];
            this.tagNames = [];

            this.cssClassesValueHashTable = {};
            this.tagNameCssClassesValueHashTable = {};
        },
        Initialize: function() {
            ASPx.HtmlEditorClasses.Controls.NativeToolbarComboBox.prototype.Initialize.call(this);
            this.CreateTagNamesAndCssClassesHashTable();
        },
        GetValue: function() {
            return ASPx.HtmlEditorClasses.Controls.NativeToolbarComboBox.prototype.GetValue.call(this);
        },
        SetValue: function(value) {
            ASPx.HtmlEditorClasses.Controls.NativeToolbarComboBox.prototype.SetValue.call(this, value);
        }
    });
    ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox.prototype.CreateTagNamesAndCssClassesHashTable =
        ASPx.HtmlEditorClasses.Controls.NativeToolbarCustomCssComboBox.prototype.CreateTagNamesAndCssClassesHashTable = function() {
            for(var i = 0; i < this.tagNames.length; i++) {
                var tagName = this.tagNames[i];
                var cssClass = this.cssClasses[i];
                if(tagName) {
                    var key = ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox.GetKeyByTagNameAndCssClass(tagName, cssClass);
                    if(!ASPx.IsExists(this.tagNameCssClassesValueHashTable[key]))
                        this.tagNameCssClassesValueHashTable[key] = this.GetItem(i).value;
                }
                else if(cssClass) {
                    if(!ASPx.IsExists(this.cssClassesValueHashTable[cssClass]))
                        this.cssClassesValueHashTable[cssClass] = this.GetItem(i).value;
                }
            }
        }
    ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox.prototype.GetIndexByTagNameAndCssClass =
        ASPx.HtmlEditorClasses.Controls.NativeToolbarCustomCssComboBox.prototype.GetIndexByTagNameAndCssClass = function(tagName, cssClass) {
            var ret = null;
            if(tagName) {
                var key = ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox.GetKeyByTagNameAndCssClass(tagName, cssClass);
                ret = ASPx.IsExists(this.tagNameCssClassesValueHashTable[key]) ? this.tagNameCssClassesValueHashTable[key] : null;
            }
            if(cssClass && (ret == null))
                ret = ASPx.IsExists(this.cssClassesValueHashTable[cssClass]) ? this.cssClassesValueHashTable[cssClass] : null;
            return ret;
        }
    ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox.prototype.GetExtValueByIndex =
        ASPx.HtmlEditorClasses.Controls.NativeToolbarCustomCssComboBox.prototype.GetExtValueByIndex = function(index) {
            return { tagName: this.tagNames[index], cssClass: this.cssClasses[index] };
        }
    ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox.prototype.AreCustomTagsExist =
        ASPx.HtmlEditorClasses.Controls.NativeToolbarCustomCssComboBox.prototype.AreCustomTagsExist = function() {
            return (this.tagNames.length > 0) && (this.cssClasses.length > 0);
        }
    ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox.GetKeyByTagNameAndCssClass =
        ASPx.HtmlEditorClasses.Controls.NativeToolbarCustomCssComboBox.GetKeyByTagNameAndCssClass = function(tagName, cssClass) {
            return tagName + "|" + cssClass;
        }
    // ASPx.HtmlEditorClasses.Controls.ToolbarParagraphFormattingComboBox
    ASPx.HtmlEditorClasses.Controls.ToolbarParagraphFormattingComboBox = ASPx.CreateClass(ASPx.HtmlEditorClasses.Controls.ToolbarComboBox, {
        SetValue: function(value) {
            if(this.GetIndexByValue(value) == -1)
                value = this.TryGetDefaultFormatValue();

            ASPx.HtmlEditorClasses.Controls.ToolbarComboBox.prototype.SetValue.call(this, value);
        }
    });
    ASPxClientNativeToolbarParagraphFormattingComboBox = ASPx.CreateClass(ASPx.HtmlEditorClasses.Controls.NativeToolbarComboBox, {
        SetValue: function(value) {
            if(this.GetIndexByValue(value) == -1)
                value = this.TryGetDefaultFormatValue();

            ASPx.HtmlEditorClasses.Controls.NativeToolbarComboBox.prototype.SetValue.call(this, value);
        }
    });
    ASPx.HtmlEditorClasses.Controls.ToolbarParagraphFormattingComboBox.prototype.TryGetDefaultFormatValue =
        ASPxClientNativeToolbarParagraphFormattingComboBox.prototype.TryGetDefaultFormatValue = function() {
            var newValue = "";
            var defaultTags = ["p", "span"];

            for(var i = 0; i < defaultTags.length && newValue == ""; i++)
                if(this.GetIndexByValue(defaultTags[i]) != -1)
                    newValue = defaultTags[i];

            return newValue;
        }
    ASPx.HtmlEditorClasses.Controls.ToolbarParagraphFormattingComboBox.prototype.GetIndexByValue =
        ASPxClientNativeToolbarParagraphFormattingComboBox.prototype.GetIndexByValue = function(value) {
            var lb = this.GetListBoxControl();
            for(var i = 0; i < lb.GetItemCount() ; i++) {
                if(lb.GetItem(i).value == value)
                    return i;
            }
            return -1;
        }
    /*region* * * * * * * * * * * * * * *  ASPx.HtmlEditorClasses.Controls.ToolbarListBox  * * * * * * * * * * * * * * * */
    ASPx.HtmlEditorClasses.Controls.ToolbarListBox = ASPx.CreateClass(ASPxClientListBox, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        GetItem: function(index) {
            var item = ASPxClientListBox.prototype.GetItem.call(this, index);
            if(item) {
                item.text = ASPx.Str.Trim(item.text);
                return item;
            }
            return null;
        }
    });
    ASPx.HtmlEditorClasses.Controls.NativeToolbarListBox = ASPx.CreateClass(ASPxClientNativeListBox, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        GetItem: function(index) {
            var item = ASPxClientNativeListBox.prototype.GetItem.call(this, index);
            if(item) {
                item.text = ASPx.Str.Trim(item.text);
                return item;
            }
            return null;
        }
    });

})();