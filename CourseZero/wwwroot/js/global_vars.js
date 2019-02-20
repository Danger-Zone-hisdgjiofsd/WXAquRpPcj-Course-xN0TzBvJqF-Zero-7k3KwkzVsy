var g_auth_token = "";
var g_login = false;
var g_username = "";

function postJSON (url, data, onsuccess, onfail) 
{
    return jQuery.ajax({
    'type': 'POST',
    'url': url,
    'contentType': 'application/json',
    'data': JSON.stringify(data),
    'dataType': 'json',
    'success': onsuccess,
    'error':  onfail
    })
};
$(document).ready(function(){
    $("#Layout_loginbtn").on('test', function () {
        alert('Test was caught');
    }
    );
});
