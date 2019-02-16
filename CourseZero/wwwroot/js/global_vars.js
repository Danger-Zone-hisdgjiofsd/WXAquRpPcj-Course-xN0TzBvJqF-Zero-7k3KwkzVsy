var g_auth_token = "";
var g_login = false;

function postJSON (url, data, onsuccss, onfail) 
{
    return jQuery.ajax({
    'type': 'POST',
    'url': url,
    'contentType': 'application/json',
    'data': (data),
    'dataType': 'json',
    'success': onsuccss,
    'error':  onfail
};