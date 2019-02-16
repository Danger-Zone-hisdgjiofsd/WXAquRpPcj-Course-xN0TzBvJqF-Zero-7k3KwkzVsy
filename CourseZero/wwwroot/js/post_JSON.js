$.postJSON = function (url, data) {
    return jQuery.ajax({
    'type': 'POST',
    'url': url,
    'contentType': 'application/json',
    'data': (data),
    'dataType': 'json',
    'success': function () {
					var obj = arguments[0];
					console.log(obj['display_message']);
					alert(obj['display_message']);
                },
    'error': function () {
                    var obj = arguments[0].responseText;
                    console.log(arguments);
                    console.log(arguments[0].responseText);
                    console.log(typeof arguments[0].responseText);
                    alert(obj);

                    }}

                );
        };