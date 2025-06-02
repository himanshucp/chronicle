const toastrOptions = {
    closeButton: true,
    debug: false, // Disable debug in production for performance
    newestOnTop: false,
    progressBar: false,
    positionClass: "toast-top-right",
    preventDuplicates: false,
    showDuration: 300,
    hideDuration: 1000,
    timeOut: 5000,
    extendedTimeOut: 1000,
    showEasing: "swing",
    hideEasing: "linear",
    showMethod: "fadeIn",
    hideMethod: "fadeOut"
};


var Patterns = {
    namespace: function (name) {
        var parts = name.split(".");
        var ns = this;

        for (var i = 0, len = parts.length; i < len; i++) {
            ns[parts[i]] = ns[parts[i]] || {};
            ns = ns[parts[i]];
        }

        return ns;
    }
};

Patterns.namespace("Utils").Alert = (function () {

    var start = function () {

        $("#alert-success").fadeIn(1000).delay(3000).fadeOut(1000, function () {
            $(this).remove();
        });

        $("#alert-failure").fadeIn(1000).delay(5500).fadeOut(1000, function () {
            $(this).remove();
        });

        $("#alert-info").fadeIn(500).delay(6500).fadeOut(1000, function () {
            $(this).remove();
        });
    };

    return { start: start };

})();


Patterns.namespace("Utils").Delete = (function () {

    var start = function () {

        $('.js-delete').each(function () {

            var dependencies = $(this).data('dependencies');
            if (dependencies > 0) {
                $(this).prop('href', 'javascript:void(0);');
                $(this).prop('title', 'Cannot be deleted');
                $(this).attr('data-bs-toggle', 'tooltip');
                $(this).tooltip();
            }
        });

        // delete button. open modal delete confirmation box.

        $('.js-delete').on('click', function (e) {

            if ($(this).attr('href') != "javascript:void(0);") {
                var id = $(this).data('id');
                var returnUrl = $(this).data('return-url');

                // opens and populates modal delete box

                $('#delete-id').val(id);
                $('#delete-return-url').val(returnUrl);
                $('#delete-form').attr('action', $(this).attr("href"));
                $('#delete-modal').modal('show');
            }

            e.preventDefault();
            return false;
        });

        $('.js-submit-delete').on('click', function (e) {

            var url = $('#delete-form').attr('action');
            var id = $('#delete-id').val();
            var token = $('[name="__RequestVerificationToken"]').val();
            var data = { 'id': id, '__RequestVerificationToken': token };

            $.ajax({
                url: url,
                type: 'POST',
                data: data,
                error: function (e) {
                    alert('Sorry, an error occured');
                    location = location;
                },
                success: function (data) {
                    // redirect page or refresh same page

                    var returnUrl = $('#delete-return-url').val();

                    if (returnUrl) {
                        location = returnUrl;
                    } else {
                        location = location;
                    }
                }
            });
        });

        // set the proper referer url before editing

        $('.js-edit').on('click', function (e) {

            var url = window.location.href.split('?')[0];
            history.pushState({}, '', url + "?tab=details");

            return true;
        });
    };

    return { start: start };

})();



Patterns.namespace("Utils").Misc = (function () {

    var start = function () {

        // activate proper tab when returning to detail pages

        var tab = getUrlParameter("tab");

        if (tab) {
            $('[href="#' + tab + '"]').click();
        }

        var subtab = getUrlParameter("subtab");

        if (subtab) {
            $('[href="#' + subtab + '"]').click();
        }

        // center tab in detail page is clicked -> display different tab area


        $('.tabs a').on('click', function (e) {
            var tab = $(this).attr("href").substr(1);
            var url = window.location.href.split('?')[0];
            history.pushState({}, '', url + "?tab=" + tab);

            $(this).tab('show');
            e.preventDefault();
            return false;
        });

        // standard filter dropdown changed -> submit

        $('#Filter').on('change', function () {
            var filterValue = $('#Filter').val();
            $('#StandardFilter').val(filterValue);
            $('#Page').val(1);
            $(this).closest('form').submit();
        });

        // advanced filter button is clicked

        $('.js-filter').on('click', function () {
            $('#Page').val(1);
        });

        // Start export to Excel

        $('.js-export').on('click', function (e) {
            var url = $(this).attr("href");
            var form = $(this).closest('form');

            form.attr("action", url);
            form.submit();
            form.attr("action", "");

            e.preventDefault();
            return false;
        });



        // sort header is clicked -> submit

        $('[data-sort]').on('click', function () {
            var sort = $(this).data('sort');
            $("#Sort").val(sort);
            $("#Page").val(1);

            $(this).closest('form').submit();
        });

        // page button is clicked -> submit

        $('[data-page]').on('click', function () {
            var page = $(this).data('page');
            $("#Page").val(page);

            $(this).closest('form').submit();
        });

        // Filter toggles are clicked -> animate to different filter area

        $('.standard-toggle').on('click', function () {
            $('#standard-filter').slideDown();
            $('#advanced-filter').slideUp();
            $('#AdvancedFilter').val('False');

            $('.advanced-toggle').removeClass('active');
            $('.standard-toggle').addClass('active');
        });

        $('.advanced-toggle').on('click', function () {
            $('#standard-filter').slideUp();
            $('#advanced-filter').slideDown();
            $('#AdvancedFilter').val('True');

            $('.standard-toggle').removeClass('active');
            $('.advanced-toggle').addClass('active');
        });

        // Initialize popovers and tooltips

        document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (e) {
            new bootstrap.Tooltip(e, { html: true });
        });
        document.querySelectorAll('[data-bs-toggle="popover"]').forEach(function (e) {
            new bootstrap.Popover(e, { html: true });
        });

        // Initialize date picker

        /* $('.js-date-picker').datepicker({ format: 'm/d/yyyy', autoclose: true, orientation: 'bottom' });*/


        //FROM https://stackoverflow.com/questions/17965839/close-a-div-by-clicking-outside

        $('.search-popup').on('click', function (ev) {
            ev.stopPropagation(); // prevent event from bubbling up to the body and closing the popup
        });

        $(".search-link").click(function (e) {

            var popup = $('.search-popup');
            popup.fadeIn(300, function () { $(this).focus(); $('#q').focus(); });


            setTimeout(function () {
                $('body').on('click', function (ev) {
                    popup.fadeOut(300); // click anywhere to hide the popup; all click events will bubble up to the body if not prevented
                    $(this).off('click');
                });

            }, 500);



            e.preventDefault();
        });


        $('.timeline').each(function (index, el) {
            var height = $(this).parent().height();

            $(this).css('min-height', (height - 40) + 'px');
        });

        // When any checkbox is clicked set value to true/false to facilitate submission to server
        $(':checkbox').on('click', function () {
            if ($(this).is(":checked")) {
                $(this).val('true');
            } else {
                $(this).val('false');
            }
        });
    };


    var stopEvent = function (event) {
        event.preventDefault();
        event.stopPropagation();
        return false;
    }

    // get parameter value from query string

    var getUrlParameter = function (name) {

        var url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");

        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)");
        var results = regex.exec(url);

        if (!results) return null;
        if (!results[2]) return '';

        return decodeURIComponent(results[2].replace(/\+/g, " "));
    };

    var showEmailNotification = function () {

        toastr.success('Email send sucessfully')
    }

    var showSuccess = function () {

        toastr.success('sucessfully')
    }

    var convertDate = function (date) {

        const dateStr = date;
        const formattedDate = dateStr && moment(dateStr).isValid()
            ? moment(dateStr).format("YYYY-MM-DD")
            : '';

        return formattedDate;
    };

    var formValidation = function (formId, rules, messages, className) {

        $(`#${formId}`).validate({
            rules: rules,
            messages: messages,
            errorElement: 'span',
            errorPlacement: function (error, element) {
                error.addClass('invalid-feedback');
                element.closest(className).append(error);
            },
            highlight: function (element, errorClass, validClass) {
                $(element).addClass('is-invalid');
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).removeClass('is-invalid');
            }
        });

    }

    var resetForm = function (formSelector) {
        $(formSelector).trigger("reset");
    }

    return {
        start: start, getUrlParameter: getUrlParameter, emailNotification: showEmailNotification,
        showSuccess: showSuccess, stopEvent: stopEvent, formatDate: convertDate,
        initializeFormValidation: formValidation,
        resetForm: resetForm,
    };

})();



Patterns.namespace("Utils").AjaxRequest = (function () {

    // Centralized AJAX GET request

    var start = function () {
        $('#qaError').html("");

    }

    var ajaxGetRequest = function (url, data, method, successCallback) {
        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: method === 'POST' ? data : data,
            success: successCallback,
            /* error: handleAjaxError*/
        });
    }

    var ajaxPostRequest = function (url, postData, successCallback) {
        $.ajax({
            url,
            type: 'POST',
            contentType: 'application/json',
            data: postData,
            success: successCallback,
            /* error: handleAjaxError*/
        });
    }


    // Handle AJAX errors
    var handleAjaxError = function (xhr, status, error) {
        console.error('Error fetching data:', error);
        $('#qaError').html('<p class="text-danger">An unexpected error occurred. Please try again later.</p>');
    }

    return {
        start: start,
        ajaxGetRequest: ajaxGetRequest,
        ajaxPostRequest: ajaxPostRequest,
        handleAjaxError: handleAjaxError,
    };

})();

Patterns.namespace("Utils").Import = (function () {

    var start = function () {

        //  $('[data-toggle="tooltip"]').tooltip();

        //$('#itemupload').on('click', function (e) {
        //    $('#DataloadItem').val('Accounts');
        //});

        $('#itemupload').on('change', function () {
            /* $('#image-loader').show();*/

            $(this).closest('form').submit();
        });

        $('#accept').on('click', function () {
            $('#accept-image-loader').show();
            return true;
        });

        $(".dropdown-menu li a").click(function () {
            var text = $(this).text();
            $('#type').val(text);
            $(this).closest("form").submit();
        });
    };

    return { start: start };

})();


Patterns.namespace("CustomValidation").ValidateForm = (function () {

    var start = function () {
        // ... existing start function code remains the same ...
        // Configure the jQuery validator
        if ($.validator) {
            $.validator.setDefaults({
                highlight: function (element, errorClass, validClass) {
                    $(element)
                        .addClass(errorClass)
                        .removeClass(validClass);

                    // Add error class to parent form-group
                    $(element).closest('.form-group').addClass('has-validation-error');

                    // Add shake animation
                    $(element).addClass('shake-error');
                    setTimeout(function () {
                        $(element).removeClass('shake-error');
                    }, 600);
                },
                unhighlight: function (element, errorClass, validClass) {
                    $(element)
                        .removeClass(errorClass)
                        .addClass('is-valid');

                    // Only remove error class if no more errors
                    if ($(element).closest('form').find('.input-validation-error').length === 0) {
                        $(element).closest('.form-group').removeClass('has-validation-error');
                    }
                },
                errorElement: 'span',
                errorClass: 'field-validation-error',
                validClass: 'valid',
                errorPlacement: function (error, element) {
                    // Default placement is after the element
                    error.insertAfter(element);

                    // For radio buttons and checkboxes place after the parent
                    if (element.is(':radio') || element.is(':checkbox')) {
                        error.insertAfter(element.closest('.form-check'));
                    }
                    // For input groups place after the group
                    else if (element.closest('.input-group').length) {
                        error.insertAfter(element.closest('.input-group'));
                    }
                }
            });
        }

        // Handle ajax form submission with validation
        $('form[data-ajax="true"]').on('submit', function (e) {
            var form = $(this);

            // Run custom validations before standard validation
            if (!runCustomValidations(form)) {
                e.preventDefault();
                return false;
            }

            // Client-side validation
            if (!form.valid()) {
                e.preventDefault();
                return false;
            }

            // If valid, submit via ajax
            if (form.data('ajax') === true) {
                e.preventDefault();

                $.ajax({
                    url: form.attr('action'),
                    type: form.attr('method'),
                    data: form.serialize(),
                    success: function (result) {
                        // Handle success - check if there are validation errors from server
                        if (result.success) {
                            // Success logic
                            if (form.data('success-redirect')) {
                                window.location.href = form.data('success-redirect');
                            }
                        } else if (result.errors) {
                            // Show server-side validation errors
                            displayServerErrors(form, result.errors);
                        }
                    },
                    error: function (xhr) {
                        // Handle error
                        console.error('Form submission error:', xhr);
                    }
                });
            }
        });

        // Initialize custom validations
        initializeCustomValidations();
    };

    var customValidation = function () {
        $('.input-validation-error').each(function () {
            $(this).closest('.form-group').addClass('has-validation-error');
        });

        $('.input-validation-error:not(.validation-applied)').addClass('shake-error validation-applied');

        // Add check mark to valid fields after they've been changed
        $('input, select, textarea').on('change', function () {
            if ($(this).valid()) {
                $(this).addClass('is-valid');
            } else {
                $(this).removeClass('is-valid');
            }
        });
    };

    var displayServerErrors = function (form, errors) {
        // Clear previous errors
        form.find('.input-validation-error').removeClass('input-validation-error');
        form.find('.field-validation-error').remove();

        // Add new errors
        $.each(errors, function (key, value) {
            var element = form.find('[name="' + key + '"]');
            if (element.length) {
                element.addClass('input-validation-error shake-error');
                $('<span class="field-validation-error">' + value + '</span>').insertAfter(element);
                element.closest('.form-group').addClass('has-validation-error');
            }
        });
    };

    // =============== GENERIC CUSTOM VALIDATION SYSTEM ===============

    // Registry to store custom validation rules
    var customValidationRules = {};

    /**
     * Register a custom validation rule
     * @param {string} name - Name of the validation rule
     * @param {Object} config - Configuration object
     * @param {string} config.selector - jQuery selector for fields to validate
     * @param {Function} config.validator - Validation function that returns {isValid: boolean, errorMessage: string}
     * @param {string|Array} config.triggers - Events that trigger validation (default: ['change', 'blur'])
     * @param {Array} config.dependencies - Array of field selectors that this validation depends on
     */
    var registerValidationRule = function (name, config) {
        if (!config.selector || !config.validator) {
            console.error('Custom validation rule must have selector and validator properties');
            return;
        }

        customValidationRules[name] = {
            selector: config.selector,
            validator: config.validator,
            triggers: config.triggers || ['change', 'blur'],
            dependencies: config.dependencies || [],
            errorClass: config.errorClass || 'field-validation-error'
        };
    };

    /**
     * Apply validation styling to an element
     * @param {jQuery} element - The element to style
     * @param {string} errorMessage - The error message to display
     */
    var applyValidationError = function (element, errorMessage) {
        // Clear previous errors for this element
        clearValidationError(element);

        // Apply error styling
        element.addClass('input-validation-error shake-error');
        element.closest('.form-group').addClass('has-validation-error');

        // Add error message
        var errorSpan = $('<span class="field-validation-error">' + errorMessage + '</span>');

        // Use same placement logic as existing validation
        if (element.is(':radio') || element.is(':checkbox')) {
            errorSpan.insertAfter(element.closest('.form-check'));
        } else if (element.closest('.input-group').length) {
            errorSpan.insertAfter(element.closest('.input-group'));
        } else {
            errorSpan.insertAfter(element);
        }

        // Remove shake animation after timeout
        setTimeout(function () {
            element.removeClass('shake-error');
        }, 600);
    };

    /**
     * Clear validation errors from an element
     * @param {jQuery} element - The element to clear errors from
     */
    var clearValidationError = function (element) {
        element.removeClass('input-validation-error shake-error');
        element.siblings('.field-validation-error').remove();

        // Only remove form-group error class if no other errors exist
        var formGroup = element.closest('.form-group');
        if (formGroup.find('.input-validation-error').length === 0) {
            formGroup.removeClass('has-validation-error');
        }
    };

    /**
     * Run a specific custom validation rule
     * @param {string} ruleName - Name of the rule to run
     * @param {jQuery} context - Context to run validation within (default: document)
     * @returns {boolean} - Whether validation passed
     */
    var runValidationRule = function (ruleName, context) {
        context = context || $(document);
        var rule = customValidationRules[ruleName];
        if (!rule) {
            console.error('Validation rule "' + ruleName + '" not found');
            return true;
        }

        var isValid = true;
        var elements = context.find(rule.selector);

        elements.each(function () {
            var element = $(this);
            var result = rule.validator.call(this, element);

            if (result.isValid) {
                clearValidationError(element);
                element.addClass('is-valid');
            } else {
                applyValidationError(element, result.errorMessage);
                isValid = false;
            }
        });

        return isValid;
    };

    /**
     * Run all custom validations within a context
     * @param {jQuery} context - Context to run validations within (default: document)
     * @returns {boolean} - Whether all validations passed
     */
    var runCustomValidations = function (context) {
        context = context || $(document);
        var allValid = true;

        $.each(customValidationRules, function (ruleName) {
            if (!runValidationRule(ruleName, context)) {
                allValid = false;
            }
        });

        return allValid;
    };

    /**
     * Initialize event handlers for all registered custom validations
     */
    var initializeCustomValidations = function () {
        $.each(customValidationRules, function (ruleName, rule) {
            var triggers = Array.isArray(rule.triggers) ? rule.triggers.join(' ') : rule.triggers;

            // Attach event handlers to the main elements
            $(document).on(triggers, rule.selector, function () {
                var element = $(this);
                setTimeout(function () { // Small delay to ensure other events complete
                    runValidationRule(ruleName, element.closest('form'));
                }, 10);
            });

            // Attach event handlers to dependency elements if they exist
            if (rule.dependencies && rule.dependencies.length > 0) {
                $.each(rule.dependencies, function (index, depSelector) {
                    $(document).on(triggers, depSelector, function () {
                        var form = $(this).closest('form');
                        setTimeout(function () {
                            runValidationRule(ruleName, form);
                        }, 10);
                    });
                });
            }
        });
    };

    // =============== BUILT-IN VALIDATION RULES ===============

    // Date Range Validation Rule
    registerValidationRule('dateRange', {
        selector: '[data-validation="date-range-end"]',
        dependencies: ['[data-validation="date-range-start"]'],
        validator: function (element) {
            var endDateElement = element;
            var startDateSelector = endDateElement.data('validation-start-field') || '[data-validation="date-range-start"]';
            var startDateElement = endDateElement.closest('form').find(startDateSelector);

            var startDate = startDateElement.val();
            var endDate = endDateElement.val();

            if (startDate && endDate) {
                var start = new Date(startDate);
                var end = new Date(endDate);

                if (end <= start) {
                    return {
                        isValid: false,
                        errorMessage: endDateElement.data('validation-message') || 'End date must be greater than start date'
                    };
                }
            }

            return { isValid: true };
        }
    });

    // Number Range Validation Rule
    registerValidationRule('numberRange', {
        selector: '[data-validation="number-range"]',
        validator: function (element) {
            var value = parseFloat(element.val());
            var min = parseFloat(element.data('validation-min'));
            var max = parseFloat(element.data('validation-max'));

            if (!isNaN(value)) {
                if (!isNaN(min) && value < min) {
                    return {
                        isValid: false,
                        errorMessage: element.data('validation-message') || 'Value must be at least ' + min
                    };
                }
                if (!isNaN(max) && value > max) {
                    return {
                        isValid: false,
                        errorMessage: element.data('validation-message') || 'Value must be no more than ' + max
                    };
                }
            }

            return { isValid: true };
        }
    });

    // Custom Pattern Validation Rule
    registerValidationRule('customPattern', {
        selector: '[data-validation="custom-pattern"]',
        validator: function (element) {
            var value = element.val();
            var pattern = element.data('validation-pattern');

            if (value && pattern) {
                var regex = new RegExp(pattern);
                if (!regex.test(value)) {
                    return {
                        isValid: false,
                        errorMessage: element.data('validation-message') || 'Invalid format'
                    };
                }
            }

            return { isValid: true };
        }
    });

    // Conditional Required Validation Rule
    registerValidationRule('conditionalRequired', {
        selector: '[data-validation="conditional-required"]',
        dependencies: ['[data-validation-trigger]'],
        validator: function (element) {
            var triggerSelector = element.data('validation-trigger');
            var triggerValue = element.data('validation-trigger-value');
            var triggerElement = element.closest('form').find(triggerSelector);

            if (triggerElement.length > 0) {
                var currentTriggerValue = triggerElement.is(':checkbox') || triggerElement.is(':radio')
                    ? triggerElement.is(':checked').toString()
                    : triggerElement.val();

                if (currentTriggerValue == triggerValue) {
                    if (!element.val() || element.val().trim() === '') {
                        return {
                            isValid: false,
                            errorMessage: element.data('validation-message') || 'This field is required'
                        };
                    }
                }
            }

            return { isValid: true };
        }
    });

    // Public API
    return {
        start: start,
        customValidation: customValidation,
        displayServerErrors: displayServerErrors,
        // New generic validation methods
        registerValidationRule: registerValidationRule,
        runValidationRule: runValidationRule,
        runCustomValidations: runCustomValidations,
        clearValidationError: clearValidationError,
        applyValidationError: applyValidationError
    };

})();

//Patterns.namespace("CustomValidation").ValidateForm = (function () {

//    var start = function () {

//        // activate proper tab when returning to detail pages

//        //customValidation();

//        // Configure the jQuery validator
//        if ($.validator) {
//            $.validator.setDefaults({
//                highlight: function (element, errorClass, validClass) {
//                    $(element)
//                        .addClass(errorClass)
//                        .removeClass(validClass);

//                    // Add error class to parent form-group
//                    $(element).closest('.form-group').addClass('has-validation-error');

//                    // Add shake animation
//                    $(element).addClass('shake-error');
//                    setTimeout(function () {
//                        $(element).removeClass('shake-error');
//                    }, 600);
//                },
//                unhighlight: function (element, errorClass, validClass) {
//                    $(element)
//                        .removeClass(errorClass)
//                        .addClass('is-valid');

//                    // Only remove error class if no more errors
//                    if ($(element).closest('form').find('.input-validation-error').length === 0) {
//                        $(element).closest('.form-group').removeClass('has-validation-error');
//                    }
//                },
//                errorElement: 'span',
//                errorClass: 'field-validation-error',
//                validClass: 'valid',
//                errorPlacement: function (error, element) {
//                    // Default placement is after the element
//                    error.insertAfter(element);

//                    // For radio buttons and checkboxes place after the parent
//                    if (element.is(':radio') || element.is(':checkbox')) {
//                        error.insertAfter(element.closest('.form-check'));
//                    }
//                    // For input groups place after the group
//                    else if (element.closest('.input-group').length) {
//                        error.insertAfter(element.closest('.input-group'));
//                    }
//                }
//            });
//        }

//        // Handle ajax form submission with validation
//        $('form[data-ajax="true"]').on('submit', function (e) {
//            var form = $(this);

//            // Client-side validation
//            if (!form.valid()) {
//                e.preventDefault();
//                return false;
//            }

//            // If valid, submit via ajax
//            if (form.data('ajax') === true) {
//                e.preventDefault();

//                $.ajax({
//                    url: form.attr('action'),
//                    type: form.attr('method'),
//                    data: form.serialize(),
//                    success: function (result) {
//                        // Handle success - check if there are validation errors from server
//                        if (result.success) {
//                            // Success logic
//                            if (form.data('success-redirect')) {
//                                window.location.href = form.data('success-redirect');
//                            }
//                        } else if (result.errors) {
//                            // Show server-side validation errors
//                            displayServerErrors(form, result.errors);
//                        }
//                    },
//                    error: function (xhr) {
//                        // Handle error
//                        console.error('Form submission error:', xhr);
//                    }
//                });
//            }
//        });



//    };

//    var customValidation = function () {

//        //alert(customValidation);
//        $('.input-validation-error').each(function () {
//            $(this).closest('.form-group').addClass('has-validation-error');
//        });

//        $('.input-validation-error:not(.validation-applied)').addClass('shake-error validation-applied');

//        // Add check mark to valid fields after they've been changed
//        $('input, select, textarea').on('change', function () {
//            if ($(this).valid()) {
//                $(this).addClass('is-valid');
//            } else {
//                $(this).removeClass('is-valid');
//            }
//        });
//    }  

//    var displayServerErrors = function(form, errors) {
//        // Clear previous errors
//        form.find('.input-validation-error').removeClass('input-validation-error');
//        form.find('.field-validation-error').remove();

//        // Add new errors
//        $.each(errors, function (key, value) {
//            var element = form.find('[name="' + key + '"]');
//            if (element.length) {
//                element.addClass('input-validation-error shake-error');
//                $('<span class="field-validation-error">' + value + '</span>').insertAfter(element);
//                element.closest('.form-group').addClass('has-validation-error');
//            }
//        });
//    }

//    return {
//        start: start, customValidation: customValidation, displayServerErrors: displayServerErrors,
//    };

//})();

// activate potential alerts when opening page

$(function () {
    Patterns.Utils.Misc.start();
    Patterns.Utils.AjaxRequest.start()
    //Patterns.Utils.Alert.start();
    Patterns.Utils.Delete.start();
    Patterns.CustomValidation.ValidateForm.start();
    //Patterns.Hlb.App.start();
    //app.openPage();
});



function deleteRecord(id, e) {


    if ($(this).attr('href') != "javascript:void(0);") {
        var id = $(e).data('id');

        
        var returnUrl = $(e).data('return-url');
        var content = $(e).data('name');
        //alert(returnUrl);

        $('#delete-id').val(id);
        $('#delete-return-url').val(returnUrl);
        $('#delete-form').attr('action', $(e).attr("href"));
       



        Swal.fire({
            title: 'Are you sure?',
            text: "You want to delete ! " + content,
            icon: 'question',
            showCloseButton: true,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes!',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                initDelete();

            } else {
                return false;
            }
        });

    }

    e.preventDefault();
    return false;

}


function initDelete() {

    var url = $('#delete-form').attr('action');
    var id = $('#delete-id').val();

    var token = $('[name="__RequestVerificationToken"]').val();
    var data = { 'id': id, '__RequestVerificationToken': token };

    alert(JSON.stringify(data));

    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        error: function (e) {
            Swal.fire({
                icon: 'error',
                text: 'Error Occured While Processing the request..',
                showCloseButton: true,
            });
            location = location;
        },
        success: function (data) {
            // redirect page or refresh same page

            var returnUrl = $('#delete-return-url').val();

            if (data.StatusCode != 200) {

                Swal.fire({
                    icon: 'error',
                    text: data.Message,
                    showCloseButton: true,
                });
                return;
            }
            if (data.StatusCode == 200) {

                toastr.success(data.Message);
                location = returnUrl;

            }

            //if (returnUrl) {

            //} else {
            //    location = location;
            //}
        }
    });
}



