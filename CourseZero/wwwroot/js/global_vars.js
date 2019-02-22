var g_auth_token = "";
var g_login = false;
var g_username = "";
var g_course;
var g_course_prefix = [];

//event
var search_available = new CustomEvent('Search');

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
