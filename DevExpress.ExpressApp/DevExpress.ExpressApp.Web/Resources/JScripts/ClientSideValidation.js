function SetEditorErrorImage(editor, imageUrl) {
    var errorImg = document.getElementById(editor.name + "_EI");
    var errorIconCssClass = "XafInplaceValidationErrorIcon";
    if (errorImg) {
        errorImg.src = imageUrl;
        ASPx.AddClassNameToElement(errorImg, errorIconCssClass);
    }
}
function SetCellErrorImage(grid, rowIndex, columnIndex, cssClassName, imageUrl) {
    var dataRow = grid.GetRow(rowIndex);
    if (dataRow) {
        var cell = grid.GetDataCell(dataRow, columnIndex);
        if (cell) {
            window.setTimeout(function () {
                var errorImages = ASPx.GetNodesByPartialClassName(cell, cssClassName);
                if (errorImages.length > 0) {
                    var errorImage = errorImages[0];
                    errorImage.src = imageUrl;
                    if (errorImage.className.indexOf("xafGridCellError") < 0) {
                        errorImage.className += " xafGridCellError";
                    }
                }
            }, 0);
        }
    }
}
function SetEditorIsValid(editor, defaultImageUrl) {
    if (editor.cpValidationText) {
        editor.SetErrorText(editor.cpValidationText, false);
        SetEditorErrorImage(editor, editor.cpValidationImage);
        editor.SetIsValid(false);
    }
    else {
        if (!editor.GetIsValid()) {
            SetEditorErrorImage(editor, defaultImageUrl);
        }
    }
}
function ApplyValidationCssClass(editor, isValid) {
    var editorElement = editor.GetMainElement();
    var validationCssClass = "ValidationFailed";
    if (editorElement) {
        if (isValid) {
            ASPx.RemoveClassNameFromElement(editorElement, validationCssClass);
        }
        else {
            ASPx.AddClassNameToElement(editorElement, validationCssClass);
        }
    }
}
function RuleValidatorBase(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues) {
    this.errorMessage = errorMessage;
    this.errorImageName = errorImageName;
    this.invertResult = invertResult;
    this.skipNullOrEmptyValues = skipNullOrEmptyValues;
    this.GetErrorMessage = function () {
        return errorMessage;
    }
    this.GetErrorImageName = function () {
        return errorImageName;
    }
    this.ValidateInternal = function (value) { }
    this.Validate = function (value) {
        if (skipNullOrEmptyValues && value === null) {
            return true;
        }
        var isValid = this.ValidateInternal(value);
        if (this.invertResult) {
            isValid = !isValid;
        }
        return isValid;
    }
}
function RuleRequiredFieldValidator(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues) {
    this.base = RuleValidatorBase;
    this.base(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues);
    this.ValidateInternal = function (value) {
        return value && value !== null;
    }
}
function RuleRegularExpressionValidator(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues, pattern) {
    this.base = RuleValidatorBase;
    this.base(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues);
    this.regExp = new RegExp(pattern);
    this.ValidateInternal = function (value) {
        var stringEditorValue = value == null ? "" : value;
        return this.regExp.test(stringEditorValue);
    }
}
function RuleRangeValidator(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues, minimumValue, maximumValue) {
    this.base = RuleValidatorBase;
    this.base(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues);
    this.ValidateInternal = function (value) {
        return value >= minimumValue && value <= maximumValue;
    }
}
function StringStartsWith(value, pattern) {
    return value.indexOf(pattern) == 0;
}
function StringEndsWith(value, pattern) {
    return value.indexOf(pattern, value.length - pattern.length) !== -1;
}
function StringContains(value, pattern) {
    return value.indexOf(pattern) > -1;
}
function StringEquals(value, pattern) {
    return value === pattern;
}
function StringNotEquals(value, pattern) {
    return value !== pattern;
}
function RuleStringComparisonValidator(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues, operandValue, operator, ignoreCase) {
    this.base = RuleValidatorBase;
    this.base(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues);
    this.operandValue = operandValue;
    this.operator = operator;
    this.ValidateInternal = function (value) {
        if ((typeof value) === 'string' || value instanceof String) {
            var preparedEditorValue = ignoreCase ? value.toUpperCase() : value;
            var preparedOperandValue = ignoreCase ? this.operandValue.toUpperCase() : this.operandValue;
            return operator(preparedEditorValue, preparedOperandValue);
        }
        else {
            return false;
        }
    }
}
function RuleValueComparisonValidator(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues, operandValue, operator) {
    this.base = RuleValidatorBase;
    this.base(errorMessage, errorImageName, invertResult, skipNullOrEmptyValues);
    this.operator = operator;
    this.ValidateInternal = function (value) {
        return operator(value, operandValue);
    }
}
function ClientSideValidator(editorValue, editorErrorText) {
    var value = editorValue;
    var isValid = true;
    var errorText = editorErrorText;
    var imageNames = [];
    var imageUrls = {};
    var ruleValidators = [];
    this.AddValidator = function (validator) {
        ruleValidators.push(validator);
    }
    this.Validate = function () {
        ruleValidators.forEach(function (validator, i, array) {
            var ruleValid = validator.Validate(editorValue);
            if (!ruleValid) {
                isValid = isValid && ruleValid;
                if (errorText) {
                    errorText += "\r\n";
                }
                errorText += validator.errorMessage;
                var errorImageName = validator.GetErrorImageName();
                if (imageNames.indexOf(errorImageName) < 0) {
                    imageNames.push(errorImageName);
                }
            }
        });
    }
    this.GetIsValid = function () {
        return isValid;
    }
    this.GetErrorMessage = function () {
        return errorText;
    }
    this.GetErrorImageName = function () {
        if (imageNames.indexOf("Error") > -1) {
            return "Error";
        }
        else if (imageNames.indexOf("Warning") > -1) {
            return "Warning";
        }
        else if (imageNames.indexOf("Information") > -1) {
            return "Information";
        }
        return "";
    }
    this.SetErrorImageUrl = function (imageName, imageUrl) {
        imageUrls[imageName] = imageUrl;
    }
    this.SetErrorText = function (text) {
        errorText = text;
    }
    this.ClearImageNames = function () {
        imageNames.length = 0;
    }
    this.GetErrorImageUrl = function () {
        if (imageUrls[this.GetErrorImageName()]) {
            return imageUrls[this.GetErrorImageName()];
        }
        else {
            return imageUrls["Error"];
        }
    }    
}