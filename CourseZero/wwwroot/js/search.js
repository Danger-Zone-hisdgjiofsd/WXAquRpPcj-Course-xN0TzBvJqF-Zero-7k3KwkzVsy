var course;
var course_prefix = [];
var text;

function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}

function preprocess_prefix_data(obj) { 
    var tmp = "";
    var i;
    for (i = 0; i < obj.length; i++) { 
        if (obj[i].Prefix != tmp) {
            course_prefix.push(obj[i].Prefix);
            tmp = obj[i].Prefix;
        }
    }
    course_prefix = course_prefix.filter(onlyUnique);
    course_prefix.sort();
    course_prefix.forEach(add_option_to_course_prefix);
    console.log(text);
}

function add_option_to_course_prefix(value) { 
    text += "<option>" + value + "</option>"
}

function get_CourseCode_prefix() { 
    var msg_to_send = {
        "auth_token": null
    }
    msg_to_send.auth_token = g_auth_token;
    postJSON("/api/General/GetAllCourses", msg_to_send, function (obj) {
        course = obj;
        console.log(course);
        preprocess_prefix_data(course);
        document.getElementById("coursecode-prefix").innerHTML = text;
    });
}

$(document).ready(function () {
    $("#Layout_advancedsearch").click(function (event) {
        event.preventDefault();
        get_CourseCode_prefix();
    });
});