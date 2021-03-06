function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}

function preprocess_prefix_data(obj) { 
    var tmp = "";
    var i;
    for (i = 0; i < obj.length; i++) { 
        if (obj[i].Prefix != tmp) {
            g_course_prefix.push(obj[i].Prefix);
            tmp = obj[i].Prefix;
        }
    }
    g_course_prefix = g_course_prefix.filter(onlyUnique);
    g_course_prefix.sort();
    //console.log(g_course_prefix); 
}

function get_CourseCode_prefix() { 
    var msg = {
        "auth_token": null
    }
    msg.auth_token = g_auth_token;
    postJSON("/api/General/GetAllCourses", msg, function (obj) {
        g_course = obj;
        //console.log(g_course);
        preprocess_prefix_data(g_course);
    });
}

function add_option_to_prefix() { 
    var text;
    for (i = 0; i < g_course_prefix.length; i++) {
        text += "<option>" + g_course_prefix[i] + "</option>";
    }
    //console.log(text);
    document.getElementById("coursecode-prefix").innerHTML = text;
}

function addtags() {
    $('#addTag_in_advancedsearch').click(function (event) {
        event.preventDefault();
        var sel = document.getElementById("coursecode-prefix");
        var cprefix = sel.options[sel.selectedIndex].text;
        var cnum = document.getElementById("coursecode-id").value;
        //console.log(cprefix + cnum);
        $('#coursecodetags').tagsinput('add', cprefix + cnum);
    });
}

$(document).ready(function (e) {
    addtags();

    $('.datepicker').datepicker({
        daysOfWeekHighlighted: "0",
        autoclose: true,
        todayHighlight: true,
    });


    document.addEventListener('Search', function (e) {
        get_CourseCode_prefix();
        document.getElementById('Layout_searchinput').disabled = false;
        document.getElementById('Layout_searchselect').disabled = false;
        document.getElementById('Layout_searchbtn').disabled = false;
        
    }, false); 
    if (g_auth_token != "") {
        add_option_to_prefix();
        $("#coursecode-prefix").selectpicker({
        });
    }
});
